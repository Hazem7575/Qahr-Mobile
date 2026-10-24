// ============================================================================
//  HotReload - apply edits made with the ScriptHazem tools (MapStudio / ItemStudio / MonsterStudio)
//  to the RUNNING server, without a restart and without players relogging.
//
//  What can be reloaded (each one is its own command / AdminApi op):
//      reloadnpcs      Npcs.txt         -> NpcHotReload (diff + despawn/spawn for players in view)
//      reloadportals   portals.ini      -> every loaded map's portal list is replaced
//      reloadshops     shops\*          -> Shop.dat, emoneyshop.ini + emoneyShopV2.txt, Honor/RacePoint/GoldenLeague
//      reloaditems     itemtype.txt     -> values of EXISTING item types updated in place (never item instances)
//      reloadmonsters  Monsters\*.ini   -> monster type values updated in place (+ live monsters' stats)
//      reloadmapcells  map\map\*.dmap   -> server-side walkability of ONE loaded map (arg: map id)
//      reloadquests    Questinfo.ini + QuestEngine\quests.json -> QuestInfo tables + the quest engine ([QuestEngine])
//      reloaddrops     drops.txt        -> file drop switch / rates ([FileDrops], Game\Monster\FileDrops.cs)
//      reloadall       npcs + portals + shops + items + monsters + drops
//  Triggers: server console (reloadnpcs ...), in-game owner command (@reloadnpcs ...), AdminApi op
//  (same names, returns the summary JSON) and an optional FileSystemWatcher (Data\hotreload.txt).
//
//  Safety rules:
//  - One reload at a time (lock Sync). Every reload is wrapped: an error aborts it and is logged; it never
//    throws into the caller (console / packet thread / AdminApi thread).
//  - Files are read with ReadStable(): size + md5 must be identical on two reads a moment apart (a tool that is
//    still writing the file makes the reload retry, then abort). A file that does not parse aborts the WHOLE
//    reload before anything is changed.
//  - Shared tables are replaced by publishing a fully built new object (reference swap) or by updating fields of
//    existing objects in place; no Dictionary that packet threads read is mutated while they may read it.
//  - Player data is never touched.
//  Every reload is appended to Data\hotreload_log.txt.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.HotReload
{
    /// <summary>An expected failure (bad file, file still being written...). Its message is shown to the user.</summary>
    public sealed class HotReloadError : Exception
    {
        public HotReloadError(string message) : base(message) { }
    }

    /// <summary>The outcome of one reload: a one-line summary plus counts and notes.</summary>
    public sealed class ReloadResult
    {
        public string Op;
        public bool Ok = true;
        public string Message = "";
        public string Error;
        public long ElapsedMs;
        public readonly Dictionary<string, int> Counts = new Dictionary<string, int>();
        public readonly List<string> Notes = new List<string>();

        public ReloadResult(string op) { Op = op; }

        public void Add(string key, int n = 1)
        {
            int v;
            Counts.TryGetValue(key, out v);
            Counts[key] = v + n;
        }
        public int Get(string key)
        {
            int v;
            return Counts.TryGetValue(key, out v) ? v : 0;
        }
        public void Note(string text)
        {
            if (Notes.Count < 60)
                Notes.Add(text);
            else if (Notes.Count == 60)
                Notes.Add("... (more notes in hotreload_log.txt are cut)");
        }

        public Dictionary<string, object> ToJson()
        {
            var d = new Dictionary<string, object>();
            d["ok"] = Ok;
            d["op"] = Op;
            if (Ok) d["message"] = Message;
            else d["error"] = Error ?? Message;
            var counts = new Dictionary<string, object>();
            foreach (var kv in Counts) counts[kv.Key] = kv.Value;
            d["counts"] = counts;
            d["notes"] = Notes.Cast<object>().ToList();
            d["ms"] = ElapsedMs;
            return d;
        }
    }

    public static class HotReloadManager
    {
        /// <summary>Only one reload runs at a time (console, GM command, AdminApi and the file watcher share it).</summary>
        public static readonly object Sync = new object();
        private static readonly object _logLock = new object();

        public static readonly string[] Ops = { "reloadnpcs", "reloadportals", "reloadshops", "reloaditems", "reloadmonsters", "reloadmapcells", "reloadquests", "reloaddrops", "reloadatkspeed", "reloadrates", "reloadall" };// [QuestEngine] reloadquests, [FileDrops] reloaddrops, [AttackSpeed] reloadatkspeed, [LiveRates] reloadrates

        public static bool IsReloadOp(string op)
        {
            return op != null && Array.IndexOf(Ops, op.Trim().ToLowerInvariant()) >= 0;
        }

        internal static string DbFile(string name)
        {
            string db = Program.ServerConfig.DbLocation ?? "";
            if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
            return db + name;
        }

        // ------------------------------------------------------------------ run
        /// <summary>
        /// Runs one reload op ("reloadnpcs", "npcs", ...). Never throws. arg = map id for reloadmapcells,
        /// force = allow an NPC reload that removes more than half of the loaded maps' NPCs.
        /// </summary>
        public static ReloadResult Run(string op, string arg = null, bool force = false)
        {
            op = (op ?? "").Trim().ToLowerInvariant();
            if (op.StartsWith("@")) op = op.Substring(1);
            if (!op.StartsWith("reload")) op = "reload" + op;
            if (op == "reloadall")
                return RunAll(force);

            var result = new ReloadResult(op);
            var clock = System.Diagnostics.Stopwatch.StartNew();
            bool taken = false;
            try
            {
                Monitor.TryEnter(Sync, 60000, ref taken);
                if (!taken)
                    throw new HotReloadError("another reload is still running; try again in a moment.");
                switch (op)
                {
                    case "reloadnpcs": NpcHotReload.Reload(result, force); break;
                    case "reloadportals": DataHotReload.ReloadPortals(result); break;
                    case "reloadshops": DataHotReload.ReloadShops(result); break;
                    case "reloaditems": DataHotReload.ReloadItems(result); break;
                    case "reloadmonsters": DataHotReload.ReloadMonsters(result); break;
                    case "reloadquests": global::Qahr.Mobile.QuestEngine.QuestEngineCore.Reload(result); break;// [QuestEngine] Questinfo.ini + QuestEngine\quests.json
                    case "reloaddrops": global::Qahr.Mobile.Game.MsgMonster.FileDrops.Reload(result); break;// [FileDrops] drops.txt
                    case "reloadatkspeed": global::Qahr.Mobile.Combat.AttackSpeedConfig.Reload(result); break;// [AttackSpeed] attackspeed.txt
                    case "reloadrates":// [LiveRates 2026-10-17] re-read ServerConfig.ini [Rates] (IniCache picks up the file change)
                        Program.ServerConfig.LoadRates();
                        result.Message = "UserExpRate=" + Program.ServerConfig.UserExpRate + " ExpRateSpell=" + Program.ServerConfig.ExpRateSpell + " ExpRateProf=" + Program.ServerConfig.ExpRateProf;
                        break;
                    case "reloadmapcells":
                        {
                            uint map;
                            if (!uint.TryParse((arg ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out map) || map == 0)
                                throw new HotReloadError("usage: reloadmapcells <map id>");
                            DataHotReload.ReloadMapCells(result, map);
                            break;
                        }
                    default:
                        throw new HotReloadError("unknown reload '" + op + "'. Use: " + string.Join(", ", Ops));
                }
            }
            catch (HotReloadError e)
            {
                result.Ok = false;
                result.Error = e.Message;
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Error = "server error: " + e.Message;
                MyConsole.SaveException(e);
            }
            finally
            {
                if (taken) Monitor.Exit(Sync);
            }
            result.ElapsedMs = clock.ElapsedMilliseconds;
            Report(result);
            return result;
        }

        private static ReloadResult RunAll(bool force)
        {
            var all = new ReloadResult("reloadall");
            var clock = System.Diagnostics.Stopwatch.StartNew();
            var parts = new List<string>();
            foreach (string op in new[] { "reloadnpcs", "reloadportals", "reloadshops", "reloaditems", "reloadmonsters", "reloaddrops", "reloadatkspeed" })
            {
                ReloadResult r = Run(op, null, force);
                foreach (var kv in r.Counts) all.Add(op.Substring(6) + "." + kv.Key, kv.Value);
                if (r.Ok) parts.Add(op.Substring(6) + ": " + r.Message);
                else
                {
                    all.Ok = false;
                    parts.Add(op.Substring(6) + ": FAILED - " + r.Error);
                }
                foreach (string n in r.Notes) all.Note(op.Substring(6) + ": " + n);
            }
            all.Message = string.Join(" | ", parts);
            if (!all.Ok) all.Error = all.Message;
            all.ElapsedMs = clock.ElapsedMilliseconds;
            return all;
        }

        // ------------------------------------------------------------------ console / log
        private static void Report(ReloadResult r)
        {
            try
            {
                string head = "[HotReload] " + r.Op + ": " + (r.Ok ? r.Message : "FAILED - " + r.Error) + " (" + r.ElapsedMs + " ms)";
                MyConsole.WriteLine(head, r.Ok ? ConsoleColor.Green : ConsoleColor.Yellow);
                for (int i = 0; i < r.Notes.Count && i < 8; i++)
                    MyConsole.WriteLine("    " + r.Notes[i], ConsoleColor.DarkGray);
                if (r.Notes.Count > 8)
                    MyConsole.WriteLine("    ... " + (r.Notes.Count - 8) + " more in hotreload_log.txt", ConsoleColor.DarkGray);
            }
            catch { }
            try
            {
                var sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                  .Append(" | ").Append(r.Ok ? "OK  " : "FAIL").Append(" | ").Append(r.Op).Append(" | ")
                  .Append(r.Ok ? r.Message : r.Error).Append(" | ").Append(r.ElapsedMs).Append(" ms").Append(Environment.NewLine);
                foreach (string n in r.Notes)
                    sb.Append("        ").Append(n.Replace("\r", " ").Replace("\n", " ")).Append(Environment.NewLine);
                lock (_logLock)
                    File.AppendAllText(DbFile("hotreload_log.txt"), sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        // ------------------------------------------------------------------ file helpers
        /// <summary>
        /// Reads a whole file only when it is stable: the same length and md5 on two reads 150 ms apart.
        /// Retries while a tool is still writing it; throws HotReloadError when it never settles or is missing.
        /// </summary>
        public static byte[] ReadStable(string path, bool allowEmpty = false)
        {
            if (!File.Exists(path))
                throw new HotReloadError(Path.GetFileName(path) + " not found (" + path + ").");
            string lastError = null;
            for (int attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    byte[] a = ReadShared(path);
                    Thread.Sleep(150);
                    byte[] b = ReadShared(path);
                    if (a.Length == b.Length && Md5(a) == Md5(b))
                    {
                        if (a.Length == 0 && !allowEmpty)
                            throw new HotReloadError(Path.GetFileName(path) + " is empty - not applied (a tool may be in the middle of saving it).");
                        return a;
                    }
                    lastError = "the file changed while it was being read";
                }
                catch (HotReloadError) { throw; }
                catch (IOException e) { lastError = e.Message; }
                catch (UnauthorizedAccessException e) { lastError = e.Message; }
                Thread.Sleep(300);
            }
            throw new HotReloadError(Path.GetFileName(path) + " is still being written or is locked (" + lastError + "); nothing was changed. Try again.");
        }

        private static byte[] ReadShared(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var ms = new MemoryStream(fs.CanSeek ? (int)Math.Min(fs.Length, int.MaxValue) : 4096))
            {
                fs.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static string Md5(byte[] data)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(data));
        }

        /// <summary>Text reader over file bytes, decoded like File.OpenText / File.ReadAllLines (UTF-8, BOM aware).</summary>
        public static StreamReader TextOf(byte[] data)
        {
            return new StreamReader(new MemoryStream(data, false), Encoding.UTF8, true);
        }

        /// <summary>Waits until the file is stable (see ReadStable) without keeping its bytes.</summary>
        public static void WaitStable(string path, bool allowEmpty = true)
        {
            ReadStable(path, allowEmpty);
        }

        /// <summary>Ids of the maps that are loaded right now (maps load lazily, on first visit).</summary>
        public static List<uint> LoadedMapIds()
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    lock (Role.GameMap.EnterObj) // GameMap.LoadMap adds maps under this lock
                        return Pool.ServerMaps.Base.Keys.ToList();
                }
                catch (InvalidOperationException) { Thread.Sleep(20); }
            }
            return new List<uint>();
        }

        /// <summary>A loaded map, WITHOUT triggering a lazy map load (Pool.ServerMaps[id] would load it).</summary>
        public static Role.GameMap LoadedMap(uint id)
        {
            Role.GameMap map;
            return Pool.ServerMaps.TryGetValue(id, out map) ? map : null;
        }

        // ------------------------------------------------------------------ watcher (optional)
        private static readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private static readonly Dictionary<string, Timer> _debounce = new Dictionary<string, Timer>();
        private static int _debounceMs = 1000;

        /// <summary>
        /// Reads Data\hotreload.txt (optional). Everything is OFF by default:
        ///   watch_npcs=1  watch_portals=1  watch_shops=1  watch_items=1  watch_monsters=1  debounce_ms=1000
        /// When a watched file changes, the matching reload runs ~debounce_ms after the last change.
        /// </summary>
        public static void Start()
        {
            try
            {
                string cfg = DbFile("hotreload.txt");
                if (!File.Exists(cfg))
                {
                    MyConsole.WriteLine("[HotReload] ready (console/@/AdminApi: reloadnpcs, reloadportals, reloadshops, reloaditems, reloadmonsters, reloaddrops, reloadall). Auto-watch off.", ConsoleColor.DarkGray);
                    return;
                }
                var on = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string raw in File.ReadAllLines(cfg))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || eq < 0) continue;
                    string k = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string v = line.Substring(eq + 1).Trim().ToLowerInvariant();
                    bool yes = v == "1" || v == "true" || v == "yes" || v == "on";
                    if (k == "debounce_ms") { int ms; if (int.TryParse(v, out ms)) _debounceMs = Math.Max(300, Math.Min(10000, ms)); }
                    else if (k.StartsWith("watch_") && yes) on.Add(k.Substring(6));
                }
                string db = DbFile("");
                if (on.Contains("npcs")) Watch(db, "Npcs.txt", false, "reloadnpcs");
                if (on.Contains("portals")) Watch(db, "portals.ini", false, "reloadportals");
                if (on.Contains("shops")) Watch(db + "shops", "*.*", false, "reloadshops");
                if (on.Contains("items")) Watch(db, "itemtype.txt", false, "reloaditems");
                if (on.Contains("monsters")) Watch(db + "Monsters", "*.ini", false, "reloadmonsters");
                if (on.Contains("drops")) Watch(db, "drops.txt", false, "reloaddrops");// [FileDrops] watch_drops=1
                if (on.Contains("atkspeed")) Watch(db, "attackspeed.txt", false, "reloadatkspeed");// [AttackSpeed] watch_atkspeed=1
                MyConsole.WriteLine("[HotReload] auto-watch: " + (on.Count == 0 ? "off" : string.Join(", ", on)) + " (debounce " + _debounceMs + " ms).", ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[HotReload] watcher not started: " + e.Message, ConsoleColor.Yellow);
            }
        }

        private static void Watch(string folder, string filter, bool subdirs, string op)
        {
            if (!Directory.Exists(folder)) return;
            var w = new FileSystemWatcher(folder, filter);
            w.IncludeSubdirectories = subdirs;
            w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
            FileSystemEventHandler h = (s, e) =>
            {
                // the tools write a backup next to the file (Npcs.txt.bak_...) - ignore anything that is not the data file
                string name = e.Name ?? "";
                if (name.IndexOf(".bak", StringComparison.OrdinalIgnoreCase) >= 0 || name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase))
                    return;
                Schedule(op);
            };
            w.Changed += h;
            w.Created += h;
            w.Renamed += (s, e) => h(s, e);
            w.EnableRaisingEvents = true;
            _watchers.Add(w);
        }

        private static void Schedule(string op)
        {
            lock (_debounce)
            {
                Timer t;
                if (_debounce.TryGetValue(op, out t))
                {
                    t.Change(_debounceMs, Timeout.Infinite);
                    return;
                }
                t = new Timer(_ =>
                {
                    try { Run(op); }
                    catch { }
                }, null, _debounceMs, Timeout.Infinite);
                _debounce[op] = t;
            }
        }

        // ------------------------------------------------------------------ in-game command
        /// <summary>
        /// For the owner's @reload... chat command: runs the reload off the player's socket thread
        /// and sends the summary back to that player.
        /// </summary>
        public static void RunForPlayer(Client.GameClient client, string op, string arg)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                ReloadResult r = Run(op, arg, false);
                try
                {
                    if (client != null && client.Socket != null && client.Socket.Alive)
                    {
                        client.SendSysMesage("[HotReload] " + r.Op + ": " + (r.Ok ? r.Message : "FAILED - " + r.Error));
                        for (int i = 0; i < r.Notes.Count && i < 4; i++)
                            client.SendSysMesage("  " + r.Notes[i]);
                    }
                }
                catch { }
            });
        }
    }
}
