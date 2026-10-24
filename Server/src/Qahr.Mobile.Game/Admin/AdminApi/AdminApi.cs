// ============================================================================
//  AdminApi - local player-administration endpoint used by tools\PlayerStudio.
//
//  OFF unless Data\adminapi.txt exists (see adminapi.sample.txt):
//      port=5899
//      secret=<at least 16 random characters>
//  The listener binds 127.0.0.1 ONLY (never the LAN/Internet). One request per TCP connection:
//      -> one line of JSON  {"secret":"...","op":"online", ...}\n
//      <- one line of JSON  {"ok":true, ...}  or  {"ok":false,"error":"..."}
//  Every request must carry the secret (wrong secret = rejected + 0.5 s delay).
//  Requests are handled one at a time on one background thread, so admin actions never run in
//  parallel with each other. Like the in-game GM chat commands (MsgMessage.ChatCommands, which run
//  on the GM's socket thread and change OTHER players directly), each action calls the same game
//  methods those commands use: UpdateLevel, Inventory.Add/AddItemWitchStack, Teleport, Revive,
//  SystemBannedAccount, SendGlobalPacket, ...
//  Every action is appended to Data\adminapi_log.txt.
//  Offline edits (level/money/CPs/position only) go through the same IniFile + OnIniChanged path the
//  @ChatBanned command uses, under ServerDatabase.SavingObj, only when the player is in neither
//  GamePoll nor DisconnectPool, and after a timestamped backup in Data\BackUp\AdminApi\.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        public const string Version = "1.0";
        private static TcpListener _listener;
        private static Thread _thread;
        private static byte[] _secret;
        private static int _port;
        private static readonly object _logLock = new object();
        private const int MaxRequestBytes = 512 * 1024;

        public static bool Running { get { return _listener != null; } }

        private static string Db { get { return Program.ServerConfig.DbLocation; } }

        private static string DbFile(string name)
        {
            string db = Db ?? "";
            if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
            return db + name;
        }

        // ------------------------------------------------------------------ start
        public static void Start()
        {
            try
            {
                string cfg = DbFile("adminapi.txt");
                if (!File.Exists(cfg))
                {
                    MyConsole.WriteLine("[AdminApi] disabled (no adminapi.txt in the DB folder).", ConsoleColor.DarkGray);
                    return;
                }
                int port = 5899;
                string secret = null;
                foreach (string raw in File.ReadAllLines(cfg))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || eq < 0) continue;
                    string k = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string v = line.Substring(eq + 1).Trim();
                    if (k == "port") int.TryParse(v, out port);
                    else if (k == "secret") secret = v;
                }
                if (string.IsNullOrEmpty(secret) || secret.Length < 16 || secret.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
                {
                    MyConsole.WriteLine("[AdminApi] NOT started: adminapi.txt needs secret=<at least 16 random characters>.", ConsoleColor.Yellow);
                    return;
                }
                if (port < 1024 || port > 65535)
                {
                    MyConsole.WriteLine("[AdminApi] NOT started: port must be 1024-65535.", ConsoleColor.Yellow);
                    return;
                }
                _secret = Encoding.UTF8.GetBytes(secret);
                _port = port;
                _listener = new TcpListener(IPAddress.Loopback, port);
                _listener.Start(16);
                _thread = new Thread(Loop) { IsBackground = true, Name = "AdminApi" };
                _thread.Start();
                MyConsole.WriteLine("[AdminApi] listening on 127.0.0.1:" + port + " (local only, PlayerStudio).", ConsoleColor.Cyan);
                Log("server", "-", "AdminApi started on 127.0.0.1:" + port, true);
            }
            catch (Exception e)
            {
                _listener = null;
                MyConsole.WriteLine("[AdminApi] failed to start: " + e.Message, ConsoleColor.Yellow);
            }
        }

        private static void Loop()
        {
            while (true)
            {
                TcpClient c = null;
                try
                {
                    c = _listener.AcceptTcpClient();
                    var ep = c.Client.RemoteEndPoint as IPEndPoint;
                    if (ep == null || !IPAddress.IsLoopback(ep.Address)) { c.Close(); continue; }
                    c.ReceiveTimeout = 5000;
                    c.SendTimeout = 5000;
                    Handle(c);
                }
                catch (ThreadAbortException) { return; }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                finally
                {
                    try { if (c != null) c.Close(); } catch (Exception err) { MyConsole.SaveException(err); }
                }
            }
        }

        private static void Handle(TcpClient c)
        {
            NetworkStream ns = c.GetStream();
            var all = new MemoryStream();
            var chunk = new byte[8192];
            var started = System.Diagnostics.Stopwatch.StartNew();
            // Read until the first line is complete. Keep EVERY byte (an HTTP POST's headers+body come
            // after the first line); the one-line-JSON protocol sends nothing past its single line.
            string firstLine = null;
            while (firstLine == null)
            {
                if (started.ElapsedMilliseconds > 10000) return; // a stuck/slow local client must not block the admin thread
                int n = ns.Read(chunk, 0, chunk.Length);
                if (n <= 0) break;
                all.Write(chunk, 0, n);
                byte[] cur = all.GetBuffer();
                int len = (int)all.Length;
                int nl = Array.IndexOf(cur, (byte)'\n', 0, len);
                if (nl >= 0)
                {
                    int end = nl;
                    if (end > 0 && cur[end - 1] == (byte)'\r') end--;
                    firstLine = Encoding.UTF8.GetString(cur, 0, end);
                }
                if (all.Length > MaxRequestBytes) { Reply(ns, Fail("request too large")); return; }
            }
            if (firstLine == null) return;

            // [Dashboard 2026-10-17] the same loopback port also answers a browser so the monitoring
            // page needs no extra process/port. A browser sends "GET/HEAD/POST ... HTTP/1.1"; everything
            // else stays the one-line-JSON protocol PlayerStudio uses.
            if (firstLine.StartsWith("GET ", StringComparison.Ordinal) || firstLine.StartsWith("HEAD ", StringComparison.Ordinal) || firstLine.StartsWith("POST ", StringComparison.Ordinal))
            {
                try { HandleHttp(ns, firstLine, all.ToArray(), started); } catch (Exception e) { MyConsole.SaveException(e); }
                return;
            }

            string text = firstLine.Trim();
            Dictionary<string, object> result;
            Dictionary<string, object> req = null;
            try { req = AdminJson.Parse(text) as Dictionary<string, object>; } catch (Exception err) { MyConsole.SaveException(err); }
            if (req == null) { Reply(ns, Fail("bad request (expected one JSON object)")); return; }

            object sec;
            if (!req.TryGetValue("secret", out sec) || !(sec is string) || !SecretEquals((string)sec))
            {
                Thread.Sleep(500); // slows down guessing (requests are handled one at a time)
                Log("denied", "-", "wrong or missing secret", false);
                Reply(ns, Fail("unauthorized"));
                return;
            }
            var args = new AdminArgs(req);
            string op = (args.Str("op") ?? "").Trim().ToLowerInvariant();
            try
            {
                result = Dispatch(op, args);
                if (result == null) result = Ok();
            }
            catch (AdminError e)
            {
                result = Fail(e.Message);
                if (op != "ping" && op != "online" && op != "player" && op != "search")
                    Log(op, Target(args), "FAILED: " + e.Message, false);
            }
            catch (Exception e)
            {
                result = Fail("server error: " + e.Message);
                Log(op, Target(args), "ERROR: " + e, false);
                MyConsole.SaveException(e);
            }
            Reply(ns, result);
        }

        private static bool SecretEquals(string given)
        {
            byte[] g = Encoding.UTF8.GetBytes(given ?? "");
            int diff = g.Length ^ _secret.Length;
            for (int i = 0; i < _secret.Length; i++)
                diff |= _secret[i] ^ (i < g.Length ? g[i] : (byte)0);
            return diff == 0;
        }

        private static void Reply(NetworkStream ns, Dictionary<string, object> obj)
        {
            byte[] b = Encoding.UTF8.GetBytes(AdminJson.Write(obj) + "\n");
            ns.Write(b, 0, b.Length);
            ns.Flush();
        }

        // [Storage backfill 2026-10-16] one-time bulk mirror of every player's .bin files into the 7 DB tables.
        private static Dictionary<string, object> MirrorBin()
        {
            int n = global::Qahr.Mobile.Database.Storage.PlayerBinaryStorage.MirrorAllFromFiles();
            var d = Ok("binary-mirrored " + n + " player(s) from .bin files to DB");
            d["players"] = n;
            Log("mirrorbin", "-", d["message"].ToString(), true);
            return d;
        }

        // [DB-only 2026-10-17] one-time copy of every player's .bin into player_blobs (the fileless reload source).
        private static Dictionary<string, object> BlobBackfill()
        {
            int n = global::Qahr.Mobile.Database.Storage.PlayerBinaryStorage.BackfillBlobsFromFiles();
            var d = Ok("blob-backfilled " + n + " .bin file(s) to player_blobs");
            d["files"] = n;
            Log("blobbackfill", "-", d["message"].ToString(), true);
            return d;
        }

        private static Dictionary<string, object> BlobVerify(AdminArgs a)
        {
            uint uid = (uint)a.Need("uid", 1, uint.MaxValue);
            string store = a.Str("store", "PlayersItems");
            var d = Ok();
            foreach (var kv in global::Qahr.Mobile.Database.Storage.PlayerBinaryStorage.VerifyBlob(uid, store))
                d[kv.Key] = kv.Value;
            return d;
        }

        private static Dictionary<string, object> Ok(string message = null)
        {
            var d = new Dictionary<string, object>();
            d["ok"] = true;
            if (message != null) d["message"] = message;
            return d;
        }

        private static Dictionary<string, object> Fail(string error)
        {
            var d = new Dictionary<string, object>();
            d["ok"] = false;
            d["error"] = error;
            return d;
        }

        private static string Target(AdminArgs a)
        {
            if (a.Has("uid")) return "uid=" + a.Str("uid");
            if (a.Has("name")) return "name=" + a.Str("name");
            return "-";
        }

        // ------------------------------------------------------------------ log
        public static void Log(string op, string target, string details, bool ok)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | " + (ok ? "OK  " : "FAIL") +
                              " | " + op + " | " + target + " | " + (details ?? "").Replace("\r", " ").Replace("\n", " ") + Environment.NewLine;
                lock (_logLock)
                    File.AppendAllText(DbFile("adminapi_log.txt"), line, Encoding.UTF8);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }

        // ------------------------------------------------------------------ dispatch
        private static Dictionary<string, object> Dispatch(string op, AdminArgs a)
        {
            switch (op)
            {
                case "ping": return Ping();
                case "status": return Status(); // [Dashboard 2026-10-17] live metrics for the monitoring page
                case "online": return Online();
                case "player": return PlayerInfo(a);
                case "search": return Search(a);
                case "setlevel": return SetLevel(a, false);
                case "maxlevel": return SetLevel(a, true);
                case "addexp": return AddExp(a);
                case "money": return Money(a);
                case "cps": return Cps(a, false);
                case "boundcps": return Cps(a, true);
                case "vip": return Vip(a);
                case "reborn": return Reborn(a);
                case "giveitem": return GiveItem(a);
                case "removeitem": return RemoveItem(a);
                case "clearinventory": return ClearInventory(a);
                case "packages": return Packages();
                case "givepackage": return GivePackage(a);
                case "teleport": return TeleportOp(a);
                case "bring": return Bring(a);
                case "revive": return Revive(a);
                case "kick": return Kick(a);
                case "ban": return Ban(a);
                case "unban": return Unban(a);
                case "bans": return Bans();
                case "chatban": return ChatBan(a, true);
                case "unchatban": return ChatBan(a, false);
                case "freeze": return Freeze(a, true);
                case "unfreeze": return Freeze(a, false);
                case "message": return Message(a);
                case "broadcast": return Broadcast(a);
                case "learnskill": return LearnSkill(a);
                case "removeskill": return RemoveSkill(a);
                case "save": return SaveOp(a);
                case "massgive": return MassGive(a);
                case "massmessage": return MassMessage(a);
                // [GmStudio] GM chat commands from the tool (Admin\AdminApi\AdminApi.Gm.cs)
                case "listgms": return ListGms();
                case "nearby": return Nearby(a);// AdminApi.Mobile.cs
                case "createguild": return CreateGuild(a);// AdminApi.Mobile.cs
                case "clientpacket": return ClientPacket(a);
                case "pkcheck": return PkCheck(a);// QahrMobile: AdminApi.Tester2.cs// QahrMobile: AdminApi.Tester2.cs (feed a packet as if the client sent it, dev only)
                case "runcommand": return RunCommand(a);
                // [HotReload] live reload of the data files edited by MapStudio / ItemStudio / MonsterStudio
                case "reloadnpcs":
                case "reloadportals":
                case "reloadshops":
                case "reloaditems":
                case "reloadmonsters":
                case "reloadmapcells":
                case "reloadquests":// [QuestEngine]
                case "reloaddrops":// [FileDrops]
                case "reloadatkspeed":// [AttackSpeed] attackspeed.txt (AttackStudio "apply live")
                case "reloadrates":// [LiveRates 2026-10-17] re-read ServerConfig.ini [Rates] with no restart
                case "reloadall": return HotReloadOp(op, a);
                // [Festivals] Admin\AdminApi\AdminApi.Festivals.cs
                case "festivals": return FestivalsList();
                case "festival": return FestivalOp(a);
                case "reloadfestivals": return FestivalsReload();
                case "mirrorbin": return MirrorBin(); // [Storage backfill 2026-10-16] one-time DB mirror of all players' .bin files
                case "blobbackfill": return BlobBackfill(); // [DB-only 2026-10-17] one-time copy of every .bin into player_blobs
                case "blobverify": return BlobVerify(a);    // [DB-only 2026-10-17] compare a player's .bin vs its DB blob
                case "festtest": return FestTestOp(a); // [FestTest 2026-10-15] dev-only festival test driver, Admin\AdminApi\AdminApi.FestTest.cs
                case "eventdata": return EventDataOp(a); // [EventData 2026-10-15] Admin\AdminApi\AdminApi.EventData.cs
                case "compensate": return CompensateOp(a); // [Compensation 2026-10-15] Admin\AdminApi\AdminApi.Compensate.cs
                default: throw new AdminError("unknown op '" + op + "'");
            }
        }

        private static Dictionary<string, object> HotReloadOp(string op, AdminArgs a)
        {
            var r = global::Qahr.Mobile.HotReload.HotReloadManager.Run(op, a.Str("map"), a.Bool("force"));
            Log(op, a.Has("map") ? "map=" + a.Str("map") : "-", r.Ok ? r.Message : "FAILED: " + r.Error, r.Ok);
            return r.ToJson();
        }

        // ------------------------------------------------------------------ helpers
        private static int MaxLevel
        {
            get
            {
                try
                {
                    Dictionary<byte, Database.DBLevExp> t;
                    if (Pool.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out t) && t.Count > 0)
                        return Math.Min(140, (int)t.Keys.Max());
                }
                catch (Exception err) { MyConsole.SaveException(err); }
                return 140;
            }
        }

        private static IEnumerable<Client.GameClient> OnlineClients()
        {
            foreach (var c in Pool.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && c.FullLoading && c.Socket != null && c.Socket.Alive)
                    yield return c;
        }

        private static Client.GameClient FindOnline(AdminArgs a)
        {
            if (a.Has("uid"))
            {
                uint uid = (uint)a.Need("uid", 1, uint.MaxValue);
                Client.GameClient c;
                if (Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading)
                    return c;
                return null;
            }
            string name = a.Str("name");
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'uid' or 'name'");
            foreach (var c in OnlineClients())
                if (string.Equals(c.Player.Name, name, StringComparison.OrdinalIgnoreCase))
                    return c;
            return null;
        }

        private static Client.GameClient NeedOnline(AdminArgs a)
        {
            var c = FindOnline(a);
            if (c == null) throw new AdminError("player is not online");
            return c;
        }

        private static string Who(Client.GameClient c)
        {
            return c.Player.Name + " (" + c.Player.UID + ")";
        }

        /// <summary>Same guarded save as the periodic save in ThreadPool (Base\Threading\ThreadPool.cs).</summary>
        private static void SafeSave(Client.GameClient c)
        {
            try
            {
                lock (Database.ServerDatabase.SavingObj)
                {
                    if (Program.ExitRequested) return;
                    if (c.OnInterServer || c.Socket == null || !c.Socket.Alive || !c.FullLoading) return;
                    Database.ServerDatabase.SaveClient(c);
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        private static Dictionary<string, object> Row(Client.GameClient c)
        {
            var p = c.Player;
            var d = new Dictionary<string, object>();
            d["uid"] = p.UID;
            d["name"] = p.Name;
            d["level"] = p.Level;
            d["cls"] = p.Class;
            d["reborn"] = p.Reborn;
            d["map"] = p.Map;
            d["x"] = p.X;
            d["y"] = p.Y;
            d["money"] = p.Money;
            d["cps"] = p.ConquerPoints;
            d["boundCps"] = p.BoundConquerPoints;
            d["vip"] = p.VipLevel;
            d["bp"] = SafeInt(() => p.BattlePower);
            d["hp"] = p.HitPoints;
            d["maxHp"] = c.Status != null ? c.Status.MaxHitpoints : 0u;
            d["alive"] = p.Alive;
            d["gm"] = c.IsGm;
            d["bot"] = c.Fake;
            return d;
        }

        private static int SafeInt(Func<int> f) { try { return f(); } catch (Exception err) { MyConsole.SaveException(err); return 0; } }

        private static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && db != null && db.Name != null)
                return db.Name.Replace('~', ' ');
            return "Item " + id;
        }

        private static Dictionary<string, object> ItemObj(Game.MsgServer.MsgGameItem it)
        {
            var d = new Dictionary<string, object>();
            d["uid"] = it.UID;
            d["id"] = it.ITEM_ID;
            d["name"] = ItemName(it.ITEM_ID);
            d["pos"] = it.Position;
            d["plus"] = it.Plus;
            d["bless"] = it.Bless;
            d["enchant"] = it.Enchant;
            d["s1"] = (byte)it.SocketOne;
            d["s2"] = (byte)it.SocketTwo;
            d["bound"] = it.Bound != 0;
            d["stack"] = it.StackSize;
            d["locked"] = it.Locked != 0;
            return d;
        }

        // ------------------------------------------------------------------ read ops
        private static Dictionary<string, object> Ping()
        {
            var d = Ok();
            d["server"] = Program.ServerConfig.ServerName;
            d["version"] = Version;
            d["online"] = OnlineClients().Count();
            d["maxLevel"] = MaxLevel;
            d["time"] = DateTime.Now;
            return d;
        }

        // ------------------------------------------------------------------ status (dashboard)
        // [Dashboard 2026-10-17] one snapshot of live server health for the monitoring page.
        // Cheap by design: no sampling thread, CPU% is measured between two status calls.
        private static readonly object _cpuLock = new object();
        private static long _cpuTicks;       // proc.TotalProcessorTime.Ticks at the previous status call
        private static long _cpuStampTicks;  // DateTime.UtcNow.Ticks at the previous status call

        private static Dictionary<string, object> Status()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var d = Ok();
            d["server"] = Program.ServerConfig.ServerName;
            d["version"] = Version;
            d["time"] = DateTime.Now;

            try { d["uptimeSec"] = (long)(DateTime.Now - proc.StartTime).TotalSeconds; } catch { d["uptimeSec"] = 0L; }

            d["online"] = OnlineClients().Count();
            d["maxLevel"] = MaxLevel;

            d["ramMb"] = proc.WorkingSet64 / (1024 * 1024);
            try { d["gcMb"] = GC.GetTotalMemory(false) / (1024 * 1024); } catch (Exception err) { MyConsole.SaveException(err); }

            // CPU% over the wall-time since the previous status call, across all cores.
            double cpu = 0;
            try
            {
                long nowTicks = DateTime.UtcNow.Ticks;
                long cpuNow = proc.TotalProcessorTime.Ticks;
                lock (_cpuLock)
                {
                    if (_cpuStampTicks != 0)
                    {
                        double wall = nowTicks - _cpuStampTicks;
                        if (wall > 0) cpu = (cpuNow - _cpuTicks) / (wall * Environment.ProcessorCount) * 100.0;
                    }
                    _cpuStampTicks = nowTicks;
                    _cpuTicks = cpuNow;
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            if (cpu < 0) cpu = 0; else if (cpu > 100) cpu = 100;
            d["cpuPct"] = Math.Round(cpu, 1);
            d["cores"] = Environment.ProcessorCount;

            // Cumulative counters; the dashboard diffs them between polls for per-second rates.
            d["bytesIn"] = Interlocked.Read(ref PerfMetrics.BytesIn);
            d["bytesOut"] = Interlocked.Read(ref PerfMetrics.BytesOut);
            d["packetsIn"] = Interlocked.Read(ref PerfMetrics.PacketsIn);
            d["packetsOut"] = Interlocked.Read(ref PerfMetrics.PacketsOut);
            d["saves"] = Interlocked.Read(ref PerfMetrics.Saves);
            long ticks = Interlocked.Read(ref PerfMetrics.Ticks);
            d["ticks"] = ticks;
            d["tickAvgMs"] = ticks > 0 ? Math.Round(Interlocked.Read(ref PerfMetrics.TickUsTotal) / (double)ticks / 1000.0, 3) : 0.0;
            d["tickMaxMs"] = Math.Round(Interlocked.Read(ref PerfMetrics.TickUsMax) / 1000.0, 3);
            d["dbQueued"] = Interlocked.Read(ref PerfMetrics.DbJobsQueued);
            d["dbDone"] = Interlocked.Read(ref PerfMetrics.DbJobsDone);
            d["dbFailed"] = Interlocked.Read(ref PerfMetrics.DbJobsFailed);

            d["netEngine"] = SharedConfig.String("Network", "NetEngine", "Legacy");
            d["backend"] = SharedConfig.String("Storage", "PlayerBackend", "File");
            d["writesEnabled"] = SharedConfig.Int("AdminApi", "AllowWrites", 0) == 1; // [Dashboard writes] live toggle

            // Whole roster size straight from MySQL (for the online/total ratio on the dashboard).
            try { d["dbPlayers"] = global::Qahr.Mobile.Database.Storage.PlayerStorage.CountPlayers(); } catch { d["dbPlayers"] = 0; }

            // Live rates (deliverable #1): the values "reloadrates" updates without a restart.
            var rates = new Dictionary<string, object>();
            rates["userExp"] = Program.ServerConfig.UserExpRate;
            rates["expSpell"] = Program.ServerConfig.ExpRateSpell;
            rates["expProf"] = Program.ServerConfig.ExpRateProf;
            d["rates"] = rates;

            return d;
        }

        // ------------------------------------------------------------------ HTTP (browser dashboard)
        //   GET /                       -> the dashboard page
        //   GET /api/status?secret=...  -> Status() as JSON (same secret as the TCP protocol)
        private static void HandleHttp(NetworkStream ns, string firstLine, byte[] already, System.Diagnostics.Stopwatch started)
        {
            string method = "GET", path = "/", query = "";
            string[] parts = firstLine.Split(' ');
            if (parts.Length >= 2)
            {
                method = parts[0].ToUpperInvariant();
                string url = parts[1];
                int q = url.IndexOf('?');
                if (q >= 0) { path = url.Substring(0, q); query = url.Substring(q + 1); }
                else path = url;
            }
            path = path.ToLowerInvariant();

            if (method == "POST")
            {
                // Write actions arrive here (HMAC-signed JSON body). See ExecWrite.
                string body = ReadHttpBody(ns, already, started);
                if (path == "/api/exec")
                {
                    Dictionary<string, object> res;
                    try { res = ExecWrite(body); }
                    catch (AdminError e) { res = Fail(e.Message); }
                    catch (Exception e) { res = Fail("server error: " + e.Message); }
                    HttpSend(ns, 200, "application/json; charset=utf-8", AdminJson.Write(res));
                    return;
                }
                HttpSend(ns, 404, "text/plain; charset=utf-8", "not found");
                return;
            }

            // GET / HEAD (read only)
            if (path == "/api/status" || path == "/status.json")
            {
                if (!HttpSecretOk(query)) { HttpSend(ns, 401, "application/json; charset=utf-8", AdminJson.Write(Fail("unauthorized"))); return; }
                string json;
                try { json = AdminJson.Write(Status()); }
                catch (Exception e) { json = AdminJson.Write(Fail("server error: " + e.Message)); }
                HttpSend(ns, 200, "application/json; charset=utf-8", json);
                return;
            }
            if (path == "/api/players")
            {
                if (!HttpSecretOk(query)) { HttpSend(ns, 401, "application/json; charset=utf-8", AdminJson.Write(Fail("unauthorized"))); return; }
                string json;
                try { json = AdminJson.Write(PlayersList(QueryArg(query, "search"), (int)ToLong(QueryArg(query, "limit"), 100), QueryArg(query, "online") == "1")); }
                catch (Exception e) { json = AdminJson.Write(Fail("server error: " + e.Message)); }
                HttpSend(ns, 200, "application/json; charset=utf-8", json);
                return;
            }
            if (path == "/api/packages")
            {
                if (!HttpSecretOk(query)) { HttpSend(ns, 401, "application/json; charset=utf-8", AdminJson.Write(Fail("unauthorized"))); return; }
                string json;
                try { json = AdminJson.Write(Packages()); }
                catch (Exception e) { json = AdminJson.Write(Fail("server error: " + e.Message)); }
                HttpSend(ns, 200, "application/json; charset=utf-8", json);
                return;
            }
            if (path == "/" || path == "/api")
            {
                // No UI is served from the game source. This is an API only; build the panel separately
                // and point it at these endpoints.
                var info = Ok();
                info["service"] = "Qahr AdminApi (HTTP, loopback only)";
                info["version"] = Version;
                info["endpoints"] = new List<object>
                {
                    "GET  /api/status?secret=...",
                    "GET  /api/players?secret=...&search=&limit=&online=1",
                    "GET  /api/packages?secret=...",
                    "POST /api/exec   body {payload, sig}  (HMAC-SHA256 of payload with the secret)"
                };
                HttpSend(ns, 200, "application/json; charset=utf-8", AdminJson.Write(info));
                return;
            }
            HttpSend(ns, 404, "text/plain; charset=utf-8", "not found");
        }

        // Reads the rest of an HTTP request (headers + body) from the stream, honouring Content-Length.
        private static string ReadHttpBody(NetworkStream ns, byte[] already, System.Diagnostics.Stopwatch started)
        {
            var buf = new MemoryStream();
            if (already != null && already.Length > 0) buf.Write(already, 0, already.Length);
            var chunk = new byte[8192];
            int headerEnd = IndexOfDoubleCrlf(buf.GetBuffer(), (int)buf.Length);
            while (headerEnd < 0)
            {
                if (started.ElapsedMilliseconds > 10000) return "";
                int n = ns.Read(chunk, 0, chunk.Length);
                if (n <= 0) return "";
                buf.Write(chunk, 0, n);
                if (buf.Length > MaxRequestBytes) return "";
                headerEnd = IndexOfDoubleCrlf(buf.GetBuffer(), (int)buf.Length);
            }
            int clen = ParseContentLength(Encoding.ASCII.GetString(buf.GetBuffer(), 0, headerEnd));
            while ((int)buf.Length - headerEnd < clen)
            {
                if (started.ElapsedMilliseconds > 10000) break;
                int n = ns.Read(chunk, 0, chunk.Length);
                if (n <= 0) break;
                buf.Write(chunk, 0, n);
                if (buf.Length > MaxRequestBytes) break;
            }
            byte[] data = buf.GetBuffer();
            int have = (int)buf.Length - headerEnd;
            int take = Math.Min(clen, have);
            if (take <= 0) return "";
            return Encoding.UTF8.GetString(data, headerEnd, take);
        }

        private static int IndexOfDoubleCrlf(byte[] b, int len)
        {
            for (int i = 0; i + 3 < len; i++)
                if (b[i] == 13 && b[i + 1] == 10 && b[i + 2] == 13 && b[i + 3] == 10) return i + 4;
            return -1;
        }

        private static int ParseContentLength(string headers)
        {
            foreach (string line in headers.Split('\n'))
            {
                string t = line.Trim();
                int c = t.IndexOf(':');
                if (c > 0 && t.Substring(0, c).Trim().Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    int v;
                    if (int.TryParse(t.Substring(c + 1).Trim(), out v) && v >= 0) return Math.Min(v, MaxRequestBytes);
                }
            }
            return 0;
        }

        private static string QueryArg(string query, string key)
        {
            foreach (string kv in (query ?? "").Split('&'))
            {
                int eq = kv.IndexOf('=');
                if (eq < 0) { if (kv == key) return ""; continue; }
                if (kv.Substring(0, eq) == key)
                {
                    try { return Uri.UnescapeDataString(kv.Substring(eq + 1)); } catch (Exception err) { MyConsole.SaveException(err); return kv.Substring(eq + 1); }
                }
            }
            return null;
        }

        private static long ToLong(string s, long def)
        {
            long v;
            return long.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def;
        }

        // ------------------------------------------------------------------ secure write (dashboard)
        // Write actions are gated three ways: (1) loopback-only listener, (2) ServerConfig.ini
        // [AdminApi] AllowWrites=1, (3) every request is HMAC-SHA256 signed with the adminapi secret
        // (the secret itself never travels) and carries a fresh timestamp (replay window 60s).
        // The op must be on the WriteWhitelist; it then runs through the SAME Dispatch the TCP tool uses.
        // Deliberately tiny: the dashboard only adds CPs and hands out a ready-made package.
        private static readonly HashSet<string> WriteWhitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "cps", "boundcps", "givepackage"
        };

        private static Dictionary<string, object> ExecWrite(string rawBody)
        {
            var outer = AdminJson.Parse(rawBody) as Dictionary<string, object>;
            if (outer == null) return Fail("bad request (expected JSON {payload,sig})");
            object pO, sO;
            string payload = outer.TryGetValue("payload", out pO) ? pO as string : null;
            string sig = outer.TryGetValue("sig", out sO) ? sO as string : null;
            if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(sig)) return Fail("missing payload/sig");

            if (!FixedEquals(HmacHex(payload), sig))
            {
                Thread.Sleep(300);
                Log("exec", "-", "bad signature", false);
                return Fail("unauthorized (signature)");
            }

            var req = AdminJson.Parse(payload) as Dictionary<string, object>;
            if (req == null) return Fail("bad payload");
            var args = new AdminArgs(req);
            double ts = (double)args.Long("ts", 0);
            double nowMs = (DateTime.UtcNow - EpochUtc).TotalMilliseconds;
            if (ts <= 0 || Math.Abs(nowMs - ts) > 60000) return Fail("stale request (check clock / replay)");
            if (!FirstUseOfSignature(sig)) { Log("exec", "-", "replayed request refused", false); return Fail("replayed request"); }

            if (SharedConfig.Int("AdminApi", "AllowWrites", 0) != 1)
            {
                Log("exec", "-", "writes disabled (AllowWrites=0)", false);
                return Fail("writes are disabled: set [AdminApi] AllowWrites=1 in ServerConfig.ini");
            }

            string op = (args.Str("op") ?? "").Trim().ToLowerInvariant();
            if (!WriteWhitelist.Contains(op)) return Fail("op '" + op + "' is not allowed from the dashboard");

            var result = Dispatch(op, args);
            return result ?? Ok();
        }

        private static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // A signed request is valid for 60 s (its "ts"); each signature is accepted once, so a captured
        // "cps" / "givepackage" request cannot be sent again inside that window.
        private static readonly Dictionary<string, DateTime> UsedSignatures = new Dictionary<string, DateTime>();

        private static bool FirstUseOfSignature(string sig)
        {
            DateTime now = DateTime.UtcNow;
            lock (UsedSignatures)
            {
                if (UsedSignatures.Count > 1000)
                    foreach (var old in UsedSignatures.Where(kv => kv.Value < now).Select(kv => kv.Key).ToList())
                        UsedSignatures.Remove(old);
                DateTime until;
                if (UsedSignatures.TryGetValue(sig, out until) && until >= now)
                    return false;
                UsedSignatures[sig] = now.AddSeconds(120);
                return true;
            }
        }

        private static string HmacHex(string msg)
        {
            using (var h = new System.Security.Cryptography.HMACSHA256(_secret))
            {
                byte[] sig = h.ComputeHash(Encoding.UTF8.GetBytes(msg ?? ""));
                var sb = new StringBuilder(sig.Length * 2);
                foreach (byte b in sig) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static bool FixedEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        // ------------------------------------------------------------------ players list (MySQL + online flag)
        // The roster straight from the `players` table, each row flagged whether that character is connected
        // to the game. The DB `Online` column is reconciled every ~15s; we overlay the live GamePoll set here
        // so the dashboard is accurate instantly even between reconciles.
        private static Dictionary<string, object> PlayersList(string search, int limit, bool onlineOnly)
        {
            // uid -> live client, so online rows carry accurate bag space + CPs for the pre-charge warnings.
            var live = new Dictionary<uint, Client.GameClient>();
            foreach (var c in OnlineClients())
                try { live[c.Player.UID] = c; } catch (Exception err) { MyConsole.SaveException(err); }

            var rows = new List<object>();
            foreach (var row in global::Qahr.Mobile.Database.Storage.PlayerStorage.ListPlayers((search ?? "").Trim(), limit))
            {
                object uidO; uint uid = 0;
                if (row.TryGetValue("uid", out uidO)) { try { uid = Convert.ToUInt32(uidO); } catch (Exception err) { MyConsole.SaveException(err); } }
                Client.GameClient c;
                bool on = live.TryGetValue(uid, out c);
                row["online"] = on;
                if (on)
                {
                    try { row["cps"] = c.Player.ConquerPoints; row["boundCps"] = c.Player.BoundConquerPoints; } catch (Exception err) { MyConsole.SaveException(err); }
                    int free = BagFree(c);
                    row["bagUsed"] = BagMax - free;
                    row["bagFree"] = free;
                    row["bagMax"] = BagMax;
                }
                if (onlineOnly && !on) continue;
                rows.Add(row);
            }

            var d = Ok();
            d["players"] = rows;
            d["online"] = live.Count;                                                                 // authoritative live count
            d["total"] = global::Qahr.Mobile.Database.Storage.PlayerStorage.CountPlayers();            // whole roster in MySQL
            d["shown"] = rows.Count;
            d["maxLevel"] = MaxLevel;
            d["bagMax"] = BagMax;
            d["cpsCap"] = CpsCap;
            return d;
        }

        private const int BagMax = 40;   // same capacity the give-item / give-package checks use

        private static int BagFree(Client.GameClient c)
        {
            try { return Math.Max(0, BagMax - c.Inventory.ClientItems.Count); }
            catch (Exception err) { MyConsole.SaveException(err); return 0; }
        }

        private static bool HttpSecretOk(string query)
        {
            foreach (string kv in (query ?? "").Split('&'))
            {
                int eq = kv.IndexOf('=');
                if (eq <= 0) continue;
                if (kv.Substring(0, eq) == "secret")
                {
                    try { return SecretEquals(Uri.UnescapeDataString(kv.Substring(eq + 1))); }
                    catch (Exception err) { MyConsole.SaveException(err); return false; }
                }
            }
            return false;
        }

        private static void HttpSend(NetworkStream ns, int code, string contentType, string body)
        {
            byte[] b = Encoding.UTF8.GetBytes(body ?? "");
            string status = code == 200 ? "200 OK" : code == 401 ? "401 Unauthorized" : code == 404 ? "404 Not Found" : code.ToString();
            var head = new StringBuilder();
            head.Append("HTTP/1.1 ").Append(status).Append("\r\n");
            head.Append("Content-Type: ").Append(contentType).Append("\r\n");
            head.Append("Content-Length: ").Append(b.Length.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            head.Append("Cache-Control: no-store\r\n");
            head.Append("Connection: close\r\n\r\n");
            byte[] h = Encoding.ASCII.GetBytes(head.ToString());
            ns.Write(h, 0, h.Length);
            ns.Write(b, 0, b.Length);
            ns.Flush();
        }


        private static Dictionary<string, object> Online()
        {
            var list = new List<object>();
            foreach (var c in OnlineClients().OrderByDescending(x => x.Player.Level))
            {
                try { list.Add(Row(c)); } catch (Exception err) { MyConsole.SaveException(err); }
            }
            var d = Ok();
            d["players"] = list;
            d["maxLevel"] = MaxLevel;
            return d;
        }

        private static Dictionary<string, object> PlayerInfo(AdminArgs a)
        {
            var c = FindOnline(a);
            if (c == null)
            {
                uint uid = ResolveOfflineUid(a);
                var off = ReadOfflineIni(uid);
                if (off == null) throw new AdminError("player not found");
                var r = Ok();
                r["online"] = false;
                r["player"] = off;
                r["banned"] = BanInfo(uid);
                return r;
            }
            var p = c.Player;
            var row = Row(c);
            row["exp"] = p.Experience;
            row["str"] = p.Strength;
            row["agi"] = p.Agility;
            row["vit"] = p.Vitality;
            row["spi"] = p.Spirit;
            row["atr"] = p.Atributes;
            row["pk"] = p.PKPoints;
            row["mana"] = p.Mana;
            row["maxMana"] = c.Status != null ? c.Status.MaxMana : 0u;
            row["vipExpire"] = p.ExpireVip;
            row["firstClass"] = p.FirstClass;
            row["secondClass"] = p.SecoundeClass;
            row["guild"] = p.MyGuild != null ? p.MyGuild.GuildName : "";
            row["ip"] = SafeStr(() => c.Socket.RemoteIp);
            row["chatBanned"] = p.IsBannedChat;
            row["frozen"] = SafeBool(() => p.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Freeze));
            var inv = new List<object>();
            foreach (var it in c.Inventory.ClientItems.Values.ToArray().OrderBy(i => i.Position).ThenBy(i => i.UID))
                inv.Add(ItemObj(it));
            var eq = new List<object>();
            foreach (var it in c.Equipment.ClientItems.Values.ToArray().OrderBy(i => i.Position))
                eq.Add(ItemObj(it));
            var spells = new List<object>();
            foreach (var s in c.MySpells.ClientSpells.Values.ToArray().OrderBy(s => s.ID))
            {
                var sd = new Dictionary<string, object>();
                sd["id"] = s.ID;
                sd["level"] = s.Level;
                sd["name"] = SpellName(s.ID);
                spells.Add(sd);
            }
            row["inventory"] = inv;
            row["inventoryMax"] = 40;
            row["equipment"] = eq;
            row["skills"] = spells;
            var d = Ok();
            d["online"] = true;
            d["player"] = row;
            d["banned"] = BanInfo(p.UID);
            return d;
        }

        private static string SpellName(ushort id)
        {
            try
            {
                Dictionary<ushort, Database.MagicType.Magic> m;
                if (Pool.Magic != null && Pool.Magic.TryGetValue(id, out m) && m.Count > 0)
                {
                    var first = m.Values.First();
                    if (!string.IsNullOrEmpty(first.Name)) return first.Name.Replace('~', ' ');
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            return "Skill " + id;
        }

        private static string SafeStr(Func<string> f) { try { return f() ?? ""; } catch (Exception err) { MyConsole.SaveException(err); return ""; } }
        private static bool SafeBool(Func<bool> f) { try { return f(); } catch (Exception err) { MyConsole.SaveException(err); return false; } }

        private static object BanInfo(uint uid)
        {
            Database.SystemBannedAccount.Client b;
            if (!Database.SystemBannedAccount.BannedPoll.TryGetValue(uid, out b) || b == null) return null;
            var d = new Dictionary<string, object>();
            var start = DateTime.FromBinary(b.StartBan);
            d["hours"] = b.Hours;
            d["since"] = start;
            d["until"] = start.AddHours(b.Hours);
            d["reason"] = b.Reason ?? "";
            return d;
        }

        private static Dictionary<string, object> Search(AdminArgs a)
        {
            string q = (a.Str("q") ?? "").Trim();
            int limit = (int)Math.Max(1, Math.Min(500, a.Long("limit", 200)));
            var list = new List<object>();
            var seen = new HashSet<uint>();
            foreach (var c in OnlineClients())
            {
                if (q.Length > 0 && c.Player.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0 && c.Player.UID.ToString() != q) continue;
                var r = Row(c);
                r["online"] = true;
                list.Add(r);
                seen.Add(c.Player.UID);
            }
            string dir = DbFile("Users");
            if (Directory.Exists(dir))
            {
                int scanned = 0;
                foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                {
                    if (list.Count >= limit || ++scanned > 50000) break;
                    uint uid;
                    if (!uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid) || seen.Contains(uid)) continue;
                    var kv = ReadIniCharacter(f);
                    if (kv == null) continue;
                    string name = Get(kv, "Name", "");
                    if (q.Length > 0 && name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0 && uid.ToString() != q) continue;
                    var r = OfflineRow(uid, kv);
                    list.Add(r);
                }
            }
            var d = Ok();
            d["players"] = list;
            return d;
        }

        // ------------------------------------------------------------------ offline (ini) helpers
        // last write time of the .ini files this API changed (so two offline edits in a row are not refused)
        private static readonly Dictionary<uint, DateTime> _offlineWrites = new Dictionary<uint, DateTime>();

        private static string UserIni(uint uid) { return DbFile("Users\\" + uid + ".ini"); }

        private static Dictionary<string, string> ReadIniCharacter(string path)
        {
            try
            {
                string text;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var sr = new StreamReader(fs, WindowsAPI.IniFile.FileEncoding, true))
                    text = sr.ReadToEnd();
                var kv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                bool inChar = false;
                foreach (string raw in text.Split('\n'))
                {
                    string line = raw.Trim();
                    if (line.StartsWith("["))
                    {
                        inChar = line.Equals("[Character]", StringComparison.OrdinalIgnoreCase);
                        continue;
                    }
                    if (!inChar) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim();
                    if (!kv.ContainsKey(k)) kv[k] = line.Substring(eq + 1).Trim();
                }
                return kv;
            }
            catch (Exception err) { MyConsole.SaveException(err); return null; }
        }

        private static string Get(Dictionary<string, string> kv, string k, string def)
        {
            string v;
            return kv.TryGetValue(k, out v) ? v : def;
        }

        private static long GetL(Dictionary<string, string> kv, string k)
        {
            long v;
            return long.TryParse(Get(kv, k, "0"), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }

        private static Dictionary<string, object> OfflineRow(uint uid, Dictionary<string, string> kv)
        {
            var d = new Dictionary<string, object>();
            d["uid"] = uid;
            d["name"] = Get(kv, "Name", "?");
            d["level"] = GetL(kv, "Level");
            d["cls"] = GetL(kv, "Class");
            d["reborn"] = GetL(kv, "Reborn");
            d["map"] = GetL(kv, "Map");
            d["x"] = GetL(kv, "X");
            d["y"] = GetL(kv, "Y");
            ulong money;
            d["money"] = ulong.TryParse(Get(kv, "Money", "0"), out money) ? money : 0UL;
            d["cps"] = GetL(kv, "ConquerPoints");
            d["boundCps"] = GetL(kv, "BoundConquerPoints");
            d["vip"] = GetL(kv, "VipLevel");
            d["online"] = false;
            return d;
        }

        private static Dictionary<string, object> ReadOfflineIni(uint uid)
        {
            string f = UserIni(uid);
            if (!File.Exists(f)) return null;
            var kv = ReadIniCharacter(f);
            if (kv == null) return null;
            var d = OfflineRow(uid, kv);
            d["exp"] = Get(kv, "Experience", "0");
            d["str"] = GetL(kv, "Strength");
            d["agi"] = GetL(kv, "Agility");
            d["vit"] = GetL(kv, "Vitaliti");
            d["spi"] = GetL(kv, "Spirit");
            d["atr"] = GetL(kv, "Atributes");
            d["pk"] = GetL(kv, "PkPoints");
            long vt = GetL(kv, "VipTime");
            try { d["vipExpire"] = DateTime.FromBinary(vt); } catch (Exception err) { MyConsole.SaveException(err); }
            d["chatBanned"] = Get(kv, "IsBannedChat", "False").Equals("True", StringComparison.OrdinalIgnoreCase);
            d["lastSaved"] = File.GetLastWriteTime(f);
            return d;
        }

        private static uint ResolveOfflineUid(AdminArgs a)
        {
            if (a.Has("uid")) return (uint)a.Need("uid", 1, uint.MaxValue);
            string name = a.Str("name");
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'uid' or 'name'");
            string dir = DbFile("Users");
            if (Directory.Exists(dir))
                foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                {
                    var kv = ReadIniCharacter(f);
                    uint uid;
                    if (kv != null && string.Equals(Get(kv, "Name", ""), name, StringComparison.OrdinalIgnoreCase)
                        && uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid))
                        return uid;
                }
            throw new AdminError("player '" + name + "' not found");
        }

        /// <summary>
        /// Writes [Character] keys of an OFFLINE player. Only when the player is in neither GamePoll nor
        /// DisconnectPool (logout save finished), under the save lock, after a timestamped backup.
        /// Same IniFile.Write + PlayerStorage.OnIniChanged path as the @ChatBanned GM command.
        /// </summary>
        private static Dictionary<string, object> OfflineWrite(AdminArgs a, string op, Dictionary<string, string> values)
        {
            if (!a.Bool("allowOffline")) throw new AdminError("player is not online (offline edit not confirmed)");
            uint uid = ResolveOfflineUid(a);
            string f = UserIni(uid);
            if (!File.Exists(f)) throw new AdminError("player file not found");
            lock (Database.ServerDatabase.SavingObj)
            {
                if (Program.ExitRequested) throw new AdminError("server is shutting down");
                if (Pool.GamePoll.ContainsKey(uid) || Pool.DisconnectPool.ContainsKey(uid))
                    throw new AdminError("player is online or still being saved - try again in a few seconds");
                DateTime mtime = File.GetLastWriteTimeUtc(f), ours;
                bool lastWriteWasOurs = _offlineWrites.TryGetValue(uid, out ours) && ours == mtime;
                if (!lastWriteWasOurs && (DateTime.UtcNow - mtime).TotalSeconds < 10)
                    throw new AdminError("player file was saved a moment ago - try again in a few seconds");
                string bakDir = DbFile("BackUp\\AdminApi");
                Directory.CreateDirectory(bakDir);
                string bak = Path.Combine(bakDir, uid + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture) + ".ini");
                File.Copy(f, bak, false);
                var ini = new WindowsAPI.IniFile("\\Users\\" + uid + ".ini");
                foreach (var kv in values)
                    ini.WriteString("Character", kv.Key, kv.Value);
                Database.Storage.PlayerStorage.OnIniChanged(uid);
                _offlineWrites[uid] = File.GetLastWriteTimeUtc(f);
                string details = string.Join(", ", values.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + " (offline, backup " + Path.GetFileName(bak) + ")";
                Log(op, "uid=" + uid, details, true);
                return Ok("offline player " + uid + ": " + details);
            }
        }

        // ------------------------------------------------------------------ level / exp
        private static Dictionary<string, object> SetLevel(AdminArgs a, bool max)
        {
            int maxLevel = MaxLevel;
            int level = max ? maxLevel : (int)a.Need("level", 1, maxLevel);
            var c = FindOnline(a);
            if (c == null)
                return OfflineWrite(a, max ? "maxlevel" : "setlevel", new Dictionary<string, string> { { "Level", level.ToString() }, { "Experience", "0" } });
            int old = c.Player.Level;
            using (var rec = new ServerSockets.RecycledPacket())
                c.UpdateLevel(rec.GetStream(), (ushort)level, true);
            SafeSave(c);
            Log(max ? "maxlevel" : "setlevel", Who(c), "level " + old + " -> " + level, true);
            return Ok(Who(c) + ": level " + old + " -> " + level);
        }

        private static Dictionary<string, object> AddExp(AdminArgs a)
        {
            var c = NeedOnline(a);
            ulong amount = a.ULong("amount");
            if (amount == 0) throw new AdminError("'amount' must be > 0");
            int maxLevel = MaxLevel;
            int old = c.Player.Level;
            Dictionary<byte, Database.DBLevExp> table;
            if (!Pool.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out table)) throw new AdminError("level table not loaded");
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var p = c.Player;
                if (p.Level >= maxLevel) throw new AdminError("player is already at max level " + maxLevel);
                // same level-up loop as GameClient.IncreaseExperience, without exp rates/gem/map multipliers
                p.Experience += amount;
                Database.DBLevExp row;
                while (p.Level < maxLevel && table.TryGetValue((byte)p.Level, out row) && row.Experience > 0 && p.Experience >= row.Experience)
                {
                    p.Experience -= row.Experience;
                    c.UpdateLevel(stream, (ushort)(p.Level + 1));
                }
                if (p.Level >= maxLevel) p.Experience = 0;
                c.UpdateRebornLastLevel(stream);
                p.SendUpdate(stream, (long)p.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
            }
            SafeSave(c);
            Log("addexp", Who(c), "+" + amount + " exp, level " + old + " -> " + c.Player.Level, true);
            return Ok(Who(c) + ": +" + amount.ToString("N0", CultureInfo.InvariantCulture) + " exp, level " + old + " -> " + c.Player.Level);
        }

        // ------------------------------------------------------------------ money / cps / vip / reborn
        private const ulong MoneyCap = 100000000000000UL;   // 100 trillion - far below the Money setter's overflow guard
        private const long CpsCap = 2147000000;               // ConquerPoints setter moves anything above this to CpsBank

        private static ulong Apply(string mode, ulong current, ulong amount, ulong cap)
        {
            switch ((mode ?? "set").ToLowerInvariant())
            {
                case "add": return Math.Min(cap, current + Math.Min(amount, cap));
                case "sub": return amount >= current ? 0 : current - amount;
                case "set": return Math.Min(cap, amount);
                default: throw new AdminError("'mode' must be set, add or sub");
            }
        }

        private static Dictionary<string, object> Money(AdminArgs a)
        {
            ulong amount = a.ULong("amount");
            string mode = a.Str("mode", "set");
            var c = FindOnline(a);
            if (c == null)
            {
                if (mode != "set") throw new AdminError("offline players: only 'set' is supported");
                return OfflineWrite(a, "money", new Dictionary<string, string> { { "Money", Math.Min(amount, MoneyCap).ToString(CultureInfo.InvariantCulture) } });
            }
            ulong old = c.Player.Money;
            ulong nv = Apply(mode, old, amount, MoneyCap);
            c.Player.Money = nv;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Player.SendUpdate(rec.GetStream(), c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
            SafeSave(c);
            Log("money", Who(c), mode + " " + amount + ": " + old + " -> " + nv, true);
            return Ok(Who(c) + ": money " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + nv.ToString("N0", CultureInfo.InvariantCulture));
        }

        private static Dictionary<string, object> Cps(AdminArgs a, bool bound)
        {
            ulong amount = a.ULong("amount");
            string mode = a.Str("mode", "set");
            string op = bound ? "boundcps" : "cps";
            var c = FindOnline(a);
            if (c == null)
            {
                if (mode != "set") throw new AdminError("offline players: only 'set' is supported");
                return OfflineWrite(a, op, new Dictionary<string, string> { { bound ? "BoundConquerPoints" : "ConquerPoints", Math.Min(amount, (ulong)CpsCap).ToString(CultureInfo.InvariantCulture) } });
            }
            if (bound)
            {
                ulong old = (ulong)Math.Max(0, c.Player.BoundConquerPoints);
                ulong nv = Apply(mode, old, amount, (ulong)int.MaxValue);
                c.Player.BoundConquerPoints = (int)nv; // setter sends the update packet
                SafeSave(c);
                Log(op, Who(c), mode + " " + amount + ": " + old + " -> " + nv, true);
                return Ok(Who(c) + ": bound CPs " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + nv.ToString("N0", CultureInfo.InvariantCulture));
            }
            else
            {
                ulong old = c.Player.ConquerPoints;
                ulong nv = Apply(mode, old, amount, (ulong)CpsCap);
                c.Player.ConquerPoints = (uint)nv; // setter logs + sends the update packet
                SafeSave(c);
                Log(op, Who(c), mode + " " + amount + ": " + old + " -> " + nv, true);
                return Ok(Who(c) + ": CPs " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + nv.ToString("N0", CultureInfo.InvariantCulture));
            }
        }

        private static Dictionary<string, object> Vip(AdminArgs a)
        {
            var c = NeedOnline(a);
            int level = (int)a.Need("level", 0, 7);
            int days = (int)a.Long("days", 30);
            if (level > 0 && (days < 1 || days > 3650)) throw new AdminError("'days' must be 1-3650");
            var p = c.Player;
            if (level == 0)
            {
                p.VipLevel = 0;
                p.ExpireVip = DateTime.Now;
            }
            else
            {
                // same as the @vip GM command: extend from now, or from the current expiry if still active
                p.ExpireVip = DateTime.Now > p.ExpireVip ? DateTime.Now.AddDays(days) : p.ExpireVip.AddDays(days);
                p.VipLevel = (byte)level;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                p.SendUpdate(stream, p.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                p.UpdateVip(stream);
            }
            if (level > 0)
                c.SendSysMesage("You received VIP " + level + " (" + days + " days).", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            SafeSave(c);
            string msg = level == 0 ? "VIP removed" : "VIP " + level + " until " + p.ExpireVip.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            Log("vip", Who(c), msg, true);
            return Ok(Who(c) + ": " + msg);
        }

        private static Dictionary<string, object> Reborn(AdminArgs a)
        {
            var c = NeedOnline(a);
            int count = (int)a.Need("count", 0, 2);
            int old = c.Player.Reborn;
            c.Player.Reborn = (byte)count;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Player.SendUpdate(rec.GetStream(), c.Player.Reborn, Game.MsgServer.MsgUpdate.DataType.Reborn);
            SafeSave(c);
            Log("reborn", Who(c), "reborn " + old + " -> " + count, true);
            return Ok(Who(c) + ": reborn " + old + " -> " + count + " (the player should relog to see everything updated)");
        }

        // ------------------------------------------------------------------ items
        private sealed class ItemSpec
        {
            public uint Id;
            public int Amount = 1;
            public byte Plus, Bless, Enchant;
            public Role.Flags.Gem S1 = Role.Flags.Gem.NoSocket, S2 = Role.Flags.Gem.NoSocket;
            public bool Bound;
            public Database.ItemType.DBItem Db;
            public bool Stackable { get { return Db.StackSize > 1; } }
            public int Slots { get { return Stackable ? (Amount + Db.StackSize - 1) / Db.StackSize : Amount; } }
            public string Describe()
            {
                return Amount + " x " + ItemName(Id) + " [" + Id + "]" + (Plus > 0 ? " +" + Plus : "") + (Bless > 0 ? " -" + Bless : "") +
                       (Enchant > 0 ? " e" + Enchant : "") + (S1 != Role.Flags.Gem.NoSocket ? " s1=" + (byte)S1 : "") +
                       (S2 != Role.Flags.Gem.NoSocket ? " s2=" + (byte)S2 : "") + (Bound ? " (bound)" : "");
            }
        }

        private static Role.Flags.Gem Gem(long v, string key)
        {
            if (v < 0 || v > 255 || !Enum.IsDefined(typeof(Role.Flags.Gem), (byte)v)) throw new AdminError("'" + key + "' is not a valid gem/socket value");
            return (Role.Flags.Gem)(byte)v;
        }

        private static ItemSpec ParseItem(AdminArgs a)
        {
            var s = new ItemSpec();
            s.Id = (uint)a.Need("id", 1, uint.MaxValue);
            if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(s.Id, out s.Db) || s.Db == null)
                throw new AdminError("item id " + s.Id + " does not exist in the server's item table");
            s.Plus = (byte)a.Long("plus", 0);
            s.Bless = (byte)a.Long("bless", 0);
            s.Enchant = (byte)Math.Max(0, Math.Min(255, a.Long("enchant", 0)));
            if (a.Long("plus", 0) < 0 || a.Long("plus", 0) > 12) throw new AdminError("'plus' must be 0-12");
            if (a.Long("bless", 0) < 0 || a.Long("bless", 0) > 7) throw new AdminError("'bless' must be 0-7");
            s.S1 = Gem(a.Long("socket1", 0), "socket1");
            s.S2 = Gem(a.Long("socket2", 0), "socket2");
            s.Bound = a.Bool("bound");
            int maxAmount = s.Stackable ? 40 * s.Db.StackSize : 40;
            long amount = a.Long("amount", 1);
            if (amount < 1 || amount > maxAmount) throw new AdminError("'amount' must be 1-" + maxAmount + " for this item");
            s.Amount = (int)amount;
            if (s.Stackable && (s.Plus > 0 || s.Bless > 0 || s.Enchant > 0 || s.S1 != Role.Flags.Gem.NoSocket || s.S2 != Role.Flags.Gem.NoSocket))
                throw new AdminError("stackable items cannot have plus/bless/enchant/sockets");
            return s;
        }

        /// <summary>Gives one item spec. Space must have been checked by the caller.</summary>
        private static bool GiveSpec(Client.GameClient c, ItemSpec s, ServerSockets.Packet stream)
        {
            if (s.Stackable)
                return c.Inventory.AddItemWitchStack(s.Id, 0, (ushort)s.Amount, stream, s.Bound);
            return c.Inventory.Add(stream, s.Id, (byte)s.Amount, s.Plus, s.Bless, s.Enchant, s.S1, s.S2, s.Bound);
        }

        private static string GiveItemTo(Client.GameClient c, ItemSpec s, bool notify)
        {
            if (!c.Inventory.HaveSpace((byte)Math.Min(255, s.Slots)))
                throw new AdminError(Who(c) + ": not enough inventory space (" + c.Inventory.ClientItems.Count + "/40, needs " + s.Slots + ")");
            bool ok;
            using (var rec = new ServerSockets.RecycledPacket())
                ok = GiveSpec(c, s, rec.GetStream());
            if (!ok) throw new AdminError(Who(c) + ": the game refused to add the item");
            if (notify)
                c.SendSysMesage("You received " + s.Amount + " " + ItemName(s.Id) + (s.Bound ? " (bound)" : "") + " from the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            return s.Describe();
        }

        private static Dictionary<string, object> GiveItem(AdminArgs a)
        {
            var c = NeedOnline(a);
            var s = ParseItem(a);
            string what = GiveItemTo(c, s, a.Bool("notify", true));
            SafeSave(c);
            Log("giveitem", Who(c), what, true);
            return Ok(Who(c) + ": gave " + what);
        }

        private static Dictionary<string, object> RemoveItem(AdminArgs a)
        {
            var c = NeedOnline(a);
            uint itemUid = (uint)a.Need("itemUid", 1, uint.MaxValue);
            Game.MsgServer.MsgGameItem it;
            if (!c.Inventory.TryGetItem(itemUid, out it)) throw new AdminError("item uid " + itemUid + " is not in the inventory (equipped items cannot be removed here)");
            string what = ItemName(it.ITEM_ID) + " [" + it.ITEM_ID + "] uid " + it.UID + (it.StackSize > 1 ? " x" + it.StackSize : "") + (it.Plus > 0 ? " +" + it.Plus : "");
            using (var rec = new ServerSockets.RecycledPacket())
                c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, rec.GetStream(), true);
            SafeSave(c);
            Log("removeitem", Who(c), what, true);
            return Ok(Who(c) + ": removed " + what);
        }

        private static Dictionary<string, object> ClearInventory(AdminArgs a)
        {
            var c = NeedOnline(a);
            int n = c.Inventory.ClientItems.Count;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Inventory.Clear(rec.GetStream());
            SafeSave(c);
            Log("clearinventory", Who(c), n + " items removed", true);
            return Ok(Who(c) + ": inventory cleared (" + n + " items)");
        }

        // ------------------------------------------------------------------ packages (Data\packages.json)
        private sealed class PackageDef
        {
            public string Name, Description;
            public ulong Money;
            public ulong Cps;
            public ulong BoundCps;
            public List<ItemSpec> Items = new List<ItemSpec>();
        }

        private static List<PackageDef> LoadPackages(bool strict, List<string> problems)
        {
            string f = DbFile("packages.json");
            var result = new List<PackageDef>();
            if (!File.Exists(f)) return result;
            var root = AdminJson.Parse(File.ReadAllText(f, Encoding.UTF8)) as Dictionary<string, object>;
            if (root == null) throw new AdminError("packages.json: root must be an object");
            object listObj;
            var list = root.TryGetValue("packages", out listObj) ? listObj as List<object> : null;
            if (list == null) return result;
            foreach (var po in list)
            {
                var pd = po as Dictionary<string, object>;
                if (pd == null) continue;
                var pa = new AdminArgs(pd);
                var p = new PackageDef { Name = pa.Str("name", ""), Description = pa.Str("description", "") };
                if (p.Name.Length == 0) continue;
                try
                {
                    p.Money = pa.ULong("money");
                    p.Cps = pa.ULong("cps");
                    p.BoundCps = pa.ULong("boundCps");
                    var items = pa.List("items");
                    if (items != null)
                        foreach (var io in items)
                        {
                            var id = io as Dictionary<string, object>;
                            if (id == null) continue;
                            p.Items.Add(ParseItem(new AdminArgs(id)));
                        }
                }
                catch (AdminError e)
                {
                    if (strict) throw new AdminError("package '" + p.Name + "': " + e.Message);
                    if (problems != null) problems.Add(p.Name + ": " + e.Message);
                    continue;
                }
                result.Add(p);
            }
            return result;
        }

        private static Dictionary<string, object> Packages()
        {
            var problems = new List<string>();
            var list = new List<object>();
            foreach (var p in LoadPackages(false, problems))
            {
                var d = new Dictionary<string, object>();
                d["name"] = p.Name;
                d["description"] = p.Description;
                d["money"] = p.Money;
                d["cps"] = p.Cps;
                d["boundCps"] = p.BoundCps;
                d["slots"] = p.Items.Sum(i => i.Slots);
                d["items"] = p.Items.Select(i => (object)i.Describe()).ToList();
                list.Add(d);
            }
            var r = Ok();
            r["packages"] = list;
            r["problems"] = problems;
            return r;
        }

        private static string GivePackageTo(Client.GameClient c, PackageDef pk)
        {
            int slots = pk.Items.Sum(i => i.Slots);
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(255, slots)))
                throw new AdminError(Who(c) + ": not enough inventory space for '" + pk.Name + "' (" + c.Inventory.ClientItems.Count + "/40, needs " + slots + ")");
            var given = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var s in pk.Items)
                    given.Add((GiveSpec(c, s, stream) ? "" : "FAILED ") + s.Describe());
                if (pk.Money > 0)
                {
                    c.Player.Money = Math.Min(MoneyCap, c.Player.Money + pk.Money);
                    c.Player.SendUpdate(stream, c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                    given.Add(pk.Money + " money");
                }
            }
            if (pk.Cps > 0)
            {
                c.Player.ConquerPoints = (uint)Math.Min((ulong)CpsCap, (ulong)c.Player.ConquerPoints + pk.Cps);
                given.Add(pk.Cps + " CPs");
            }
            if (pk.BoundCps > 0)
            {
                c.Player.BoundConquerPoints = (int)Math.Min((ulong)int.MaxValue, (ulong)Math.Max(0, c.Player.BoundConquerPoints) + pk.BoundCps);
                given.Add(pk.BoundCps + " bound CPs");
            }
            c.SendSysMesage("You received the package \"" + pk.Name + "\" from the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            return "'" + pk.Name + "': " + string.Join("; ", given.ToArray());
        }

        private static PackageDef FindPackage(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'package'");
            var pk = LoadPackages(true, null).FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            if (pk == null) throw new AdminError("package '" + name + "' not found in packages.json");
            return pk;
        }

        private static Dictionary<string, object> GivePackage(AdminArgs a)
        {
            var c = NeedOnline(a);
            var pk = FindPackage(a.Str("package"));
            string what = GivePackageTo(c, pk);
            SafeSave(c);
            Log("givepackage", Who(c), what, true);
            return Ok(Who(c) + ": gave package " + what);
        }

        // ------------------------------------------------------------------ teleport
        private static Dictionary<string, object> TeleportOp(AdminArgs a)
        {
            uint map = (uint)a.Need("map", 1, uint.MaxValue);
            ushort x = (ushort)a.Need("x", 0, ushort.MaxValue);
            ushort y = (ushort)a.Need("y", 0, ushort.MaxValue);
            if (!Pool.ServerMaps.ContainsKey(map)) throw new AdminError("map " + map + " is not loaded on the server");
            bool walkable = true;
            try { walkable = Pool.ServerMaps[map].ValidLocation(x, y); } catch (Exception err) { MyConsole.SaveException(err); }
            if (!walkable && !a.Bool("force"))
                throw new AdminError("(" + x + "," + y + ") is not walkable on map " + map + " - pick another spot or use force");
            var c = FindOnline(a);
            if (c == null)
                return OfflineWrite(a, "teleport", new Dictionary<string, string> { { "Map", map.ToString() }, { "X", x.ToString() }, { "Y", y.ToString() } });
            string from = c.Player.Map + " (" + c.Player.X + "," + c.Player.Y + ")";
            c.Teleport(x, y, map);
            bool moved = c.Player.Map == map;
            Log("teleport", Who(c), from + " -> " + map + " (" + x + "," + y + ")" + (moved ? "" : " [the game blocked it]"), moved);
            if (!moved) throw new AdminError(Who(c) + ": the game blocked the teleport (jail/shackle/event map or reborn requirement)");
            return Ok(Who(c) + ": teleported to map " + map + " (" + x + "," + y + ")");
        }

        private static Dictionary<string, object> Bring(AdminArgs a)
        {
            var c = NeedOnline(a);
            uint toUid = (uint)a.Need("toUid", 1, uint.MaxValue);
            Client.GameClient t;
            if (!Pool.GamePoll.TryGetValue(toUid, out t) || t == null || t.Player == null) throw new AdminError("target player is not online");
            if (t == c) throw new AdminError("choose a different target player");
            c.Teleport(t.Player.X, t.Player.Y, t.Player.Map, t.Player.DynamicID);
            Log("bring", Who(c), "-> " + Who(t) + " at " + t.Player.Map + " (" + t.Player.X + "," + t.Player.Y + ")", true);
            return Ok(Who(c) + " moved to " + Who(t));
        }

        // ------------------------------------------------------------------ life / discipline
        private static Dictionary<string, object> Revive(AdminArgs a)
        {
            var c = NeedOnline(a);
            bool wasDead = !c.Player.Alive;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                if (wasDead)
                    c.Player.Revive(rec.GetStream());
                else
                {
                    c.Player.HitPoints = (int)c.Status.MaxHitpoints;
                    c.Player.Mana = (ushort)Math.Min(ushort.MaxValue, c.Status.MaxMana);
                }
            }
            Log("revive", Who(c), wasDead ? "revived" : "healed", true);
            return Ok(Who(c) + (wasDead ? ": revived" : ": HP/MP restored"));
        }

        private static Dictionary<string, object> Kick(AdminArgs a)
        {
            var c = NeedOnline(a);
            string why = a.Str("reason", "");
            if (why.Length > 0)
                c.SendSysMesage("You were disconnected by the staff: " + why, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
            c.Socket.Disconnect();
            Log("kick", Who(c), why, true);
            return Ok(Who(c) + " kicked");
        }

        private static Dictionary<string, object> Ban(AdminArgs a)
        {
            long hours = a.Need("hours", 1, 876000);
            string reason = a.Str("reason", "");
            var c = FindOnline(a);
            uint uid;
            string name;
            if (c != null) { uid = c.Player.UID; name = c.Player.Name; }
            else
            {
                uid = ResolveOfflineUid(a);
                var off = ReadOfflineIni(uid);
                name = off != null ? (string)off["name"] : "?";
            }
            Database.SystemBannedAccount.RemoveBan(uid);
            Database.SystemBannedAccount.AddBan(uid, name, (uint)hours, reason);
            try { Database.SystemBannedAccount.Save(); } catch (Exception e) { MyConsole.SaveException(e); }
            if (c != null)
            {
                c.SendSysMesage("Your account was banned by the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.white);
                c.Socket.Disconnect();
            }
            Log("ban", name + " (" + uid + ")", hours + " hours, reason: " + reason, true);
            return Ok(name + " (" + uid + ") banned for " + hours + " hours" + (c != null ? " and kicked" : ""));
        }

        private static Dictionary<string, object> Unban(AdminArgs a)
        {
            uint uid = a.Has("uid") ? (uint)a.Need("uid", 1, uint.MaxValue) : ResolveOfflineUid(a);
            if (!Database.SystemBannedAccount.RemoveBanS(uid)) throw new AdminError("uid " + uid + " is not banned");
            try { Database.SystemBannedAccount.Save(); } catch (Exception e) { MyConsole.SaveException(e); }
            Log("unban", "uid=" + uid, "ban removed", true);
            return Ok("uid " + uid + " unbanned");
        }

        private static Dictionary<string, object> Bans()
        {
            var list = new List<object>();
            foreach (var b in Database.SystemBannedAccount.BannedPoll.Values.ToArray())
            {
                var d = new Dictionary<string, object>();
                var start = DateTime.FromBinary(b.StartBan);
                d["uid"] = b.UID;
                d["name"] = b.Name ?? "";
                d["hours"] = b.Hours;
                d["since"] = start;
                d["until"] = start.AddHours(b.Hours);
                d["reason"] = b.Reason ?? "";
                list.Add(d);
            }
            var r = Ok();
            r["bans"] = list;
            return r;
        }

        private static Dictionary<string, object> ChatBan(AdminArgs a, bool ban)
        {
            var c = NeedOnline(a);
            var p = c.Player;
            if (ban)
            {
                long minutes = a.Long("minutes", 60);
                if (minutes <= 0)
                    p.PermenantBannedChat = true;
                else
                    p.BannedChatStamp = DateTime.Now.AddMinutes(Math.Min(minutes, 5256000));
                p.IsBannedChat = true;
            }
            else
            {
                p.BannedChatStamp = DateTime.Now;
                p.IsBannedChat = false;
                p.PermenantBannedChat = false;
            }
            SafeSave(c);
            string what = ban ? (p.PermenantBannedChat ? "chat banned permanently" : "chat banned until " + p.BannedChatStamp.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)) : "chat ban removed";
            Log(ban ? "chatban" : "unchatban", Who(c), what, true);
            return Ok(Who(c) + ": " + what);
        }

        private static Dictionary<string, object> Freeze(AdminArgs a, bool freeze)
        {
            var c = NeedOnline(a);
            if (freeze)
            {
                int seconds = (int)Math.Max(10, Math.Min(86400 * 7, a.Long("seconds", 86400)));
                c.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Freeze, seconds, true); // same as @freeze
                Log("freeze", Who(c), seconds + " s", true);
                return Ok(Who(c) + " frozen for " + seconds + " s");
            }
            c.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Freeze);
            Log("unfreeze", Who(c), "", true);
            return Ok(Who(c) + " unfrozen");
        }

        // ------------------------------------------------------------------ messages
        private static string CleanText(AdminArgs a)
        {
            string text = (a.Str("text") ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            if (text.Length == 0) throw new AdminError("missing 'text'");
            if (text.Length > 250) text = text.Substring(0, 250);
            return text;
        }

        private static Dictionary<string, object> Message(AdminArgs a)
        {
            var c = NeedOnline(a);
            string text = CleanText(a);
            if (a.Bool("popup"))
                c.CreateBoxDialog(text);
            else
                c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            Log("message", Who(c), text, true);
            return Ok("message sent to " + Who(c));
        }

        private static Game.MsgServer.MsgMessage.ChatMode BroadcastMode(string mode)
        {
            switch ((mode ?? "center").ToLowerInvariant())
            {
                case "center": return Game.MsgServer.MsgMessage.ChatMode.Center;
                case "broadcast": return Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage;
                case "system": return Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem;
                case "talk": return Game.MsgServer.MsgMessage.ChatMode.System;
                default: throw new AdminError("'mode' must be center, broadcast, system or talk");
            }
        }

        private static Dictionary<string, object> Broadcast(AdminArgs a)
        {
            string text = CleanText(a);
            string mode = a.Str("mode", "center");
            var cm = BroadcastMode(mode);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(text, "ALLUSERS", Game.MsgServer.MsgMessage.MsgColor.red, cm).GetArray(stream));
            }
            Log("broadcast", "all", mode + ": " + text, true);
            return Ok("broadcast sent (" + mode + ")");
        }

        private static Dictionary<string, object> MassMessage(AdminArgs a)
        {
            string text = CleanText(a);
            bool popup = a.Bool("popup");
            int n = 0;
            foreach (var c in OnlineClients())
            {
                if (c.Fake) continue;
                try
                {
                    if (popup) c.CreateBoxDialog(text);
                    else c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                    n++;
                }
                catch (Exception err) { MyConsole.SaveException(err); }
            }
            Log("massmessage", "all", n + " players: " + text, true);
            return Ok("message sent to " + n + " players");
        }

        // ------------------------------------------------------------------ skills
        private static Dictionary<string, object> LearnSkill(AdminArgs a)
        {
            var c = NeedOnline(a);
            ushort id = (ushort)a.Need("skill", 1, ushort.MaxValue);
            ushort level = (ushort)a.Need("level", 0, 20);
            if (Pool.Magic == null || !Pool.Magic.ContainsKey(id)) throw new AdminError("skill " + id + " does not exist in MagicType");
            using (var rec = new ServerSockets.RecycledPacket())
                c.MySpells.Add(rec.GetStream(), id, level, 0, 0, 0); // same call as the @spell GM command
            SafeSave(c);
            Log("learnskill", Who(c), SpellName(id) + " [" + id + "] level " + level, true);
            return Ok(Who(c) + ": learned " + SpellName(id) + " level " + level);
        }

        private static Dictionary<string, object> RemoveSkill(AdminArgs a)
        {
            var c = NeedOnline(a);
            ushort id = (ushort)a.Need("skill", 1, ushort.MaxValue);
            if (!c.MySpells.ClientSpells.ContainsKey(id)) throw new AdminError("the player does not have skill " + id);
            using (var rec = new ServerSockets.RecycledPacket())
                c.MySpells.Remove(id, rec.GetStream());
            SafeSave(c);
            Log("removeskill", Who(c), SpellName(id) + " [" + id + "]", true);
            return Ok(Who(c) + ": removed " + SpellName(id));
        }

        private static Dictionary<string, object> SaveOp(AdminArgs a)
        {
            if (a.Bool("all"))
            {
                int n = 0;
                foreach (var c in OnlineClients()) { if (!c.Fake) { SafeSave(c); n++; } }
                Log("save", "all", n + " players saved", true);
                return Ok(n + " players saved");
            }
            var one = NeedOnline(a);
            SafeSave(one);
            Log("save", Who(one), "", true);
            return Ok(Who(one) + " saved");
        }

        // ------------------------------------------------------------------ mass
        private static Dictionary<string, object> MassGive(AdminArgs a)
        {
            ItemSpec item = null;
            PackageDef pk = null;
            if (a.Has("package")) pk = FindPackage(a.Str("package"));
            else item = ParseItem(a);
            int minLevel = (int)a.Long("minLevel", 0);
            int okCount = 0;
            var failed = new List<object>();
            foreach (var c in OnlineClients())
            {
                if (c.Fake || c.Player.Level < minLevel) continue;
                try
                {
                    if (pk != null) GivePackageTo(c, pk);
                    else GiveItemTo(c, item, true);
                    SafeSave(c);
                    okCount++;
                }
                catch (AdminError e) { failed.Add(e.Message); }
                catch (Exception e) { failed.Add(Who(c) + ": " + e.Message); }
            }
            string what = pk != null ? "package '" + pk.Name + "'" : item.Describe();
            Log("massgive", "all (minLevel " + minLevel + ")", what + " -> " + okCount + " ok, " + failed.Count + " failed", true);
            var r = Ok("gave " + what + " to " + okCount + " players" + (failed.Count > 0 ? ", " + failed.Count + " failed" : ""));
            r["failed"] = failed;
            return r;
        }
    }
}
