// [NewGuildWar 2026-10-14] New Guild War (CMsgNewSynWar 2061). A different event from the classic pole war
// (MsgTournaments.MsgGuildWar), run on the same Guild War map (client lua NewSynWar.lua: SynMapDoc 1038, Guild Pillar id 810).
//
// Official rules (client text Cn_Res.ini STR_NEW_SYN_WARINFO_*, lua NewSynWar.lua):
//   - 10,000 points in total, the guild with the most points wins; every participating guild gets a share of a
//     10,000 Guild CP fund in proportion to its points; the winning guild leader gets 3,000 CPs extra.
//   - Guild Pillar (810): 3,000 points shared by the damage each guild deals; its No.1 guild owns both gates.
//   - Left / Right Gate: 500 points each -> the Guild Pillar No.1, or the guild that breaches the gate.
//   - Honor Pillar: 3,000 points, appears 25 min after the start for 5 min, 120,000,000 HP; the guild with the most
//     damage on it holds the points; it restores the moment it is destroyed.
//   - Hero points (the remaining 3,000): killing a hero robs 80% of his points; a dead hero who does not revive within
//     30 s, a hero who leaves the map and a hero who quits his guild lose their points back to the pool.
//   - Schedule (lua): Sunday (NewSynWarData 0) 14:30, Honor Pillar 14:55 - 15:00.
// Derived (client has no server values): war length 30 min, 20 pool points per enemy kill (once per victim per minute),
// HUD refresh 10 s, pre-war countdown 10 min, Honor Pillar uid / look, no buff pillars / gate repair / soul shackle
// (no server objects for them on this map). Sign-up: there is no sign-up UI in this client - every guild whose
// members fight on the map takes part (guild level >= MinGuildLevel).
// Config: Infinity[DB]\NewGuildWar\config.ini   State: Infinity[DB]\NewGuildWar\state.txt   Log: log.txt
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.NewGuildWar
{
    public sealed class NgwConfig
    {
        public int Enabled = 1;
        public int Day = 0;                 // official lua NewSynWarData (0 = Sunday)
        public int StartHHMM = 1430;        // official lua NewSynWarTime
        public int DurationMin = 30;        // derived (Honor Pillar window ends 30 min after the start)
        public int BossOffsetMin = 25;      // official lua BossStartTime 1455
        public int BossMinutes = 5;         // official lua BossEndTime 1500 / STR_NEW_SYN_WARINFO_BOSS_1
        public int BossHp = 120000000;      // official STR_NEW_SYN_WARINFO_BOSS_1
        public int BossX = 77, BossY = 77;  // official lua BossPosX / BossPosY
        public uint BossUid = 19810;        // derived
        public uint Map = 1038;             // official lua SynMapDoc
        public int PillarPoints = 3000;     // official
        public int GatePoints = 500;        // official (each gate)
        public int HonorPoints = 3000;      // official
        public int HeroPool = 3000;         // official (10,000 - 7,000)
        public int CpFund = 10000;          // official STR_NEW_SYN_WARINFO_AWARD_INFO1
        public int WinnerLeaderCps = 3000;  // official STR_NEW_SYN_WARINFO_AWARD_INFO1
        public int RobPercent = 80;         // official STR_NEW_SYN_WARINFO_HERO_SCORE2
        public int DeadRecycleSec = 30;     // official STR_NEW_SYN_WARINFO_HERO_SCORE2
        public int KillGrant = 20;          // derived
        public int KillGrantCooldownSec = 60; // derived
        public int PreCountdownMin = 10;    // derived
        public int HudIntervalSec = 10;     // derived
        public int RankRows = 10;           // derived
        public int MinGuildLevel = 1;       // derived

        public static NgwConfig Load(string file)
        {
            var c = new NgwConfig();
            try
            {
                if (!File.Exists(file)) return c;
                foreach (var raw in File.ReadAllLines(file))
                {
                    var l = raw.Split('#')[0].Trim();
                    int eq = l.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = l.Substring(0, eq).Trim().ToLowerInvariant();
                    long v;
                    if (!long.TryParse(l.Substring(eq + 1).Trim(), out v)) continue;
                    foreach (var f in typeof(NgwConfig).GetFields())
                    {
                        if (f.Name.ToLowerInvariant() != k) continue;
                        if (v < 0) v = 0;
                        if (f.FieldType == typeof(int)) f.SetValue(c, (int)Math.Min(v, int.MaxValue));
                        else if (f.FieldType == typeof(uint)) f.SetValue(c, (uint)Math.Min(v, uint.MaxValue));
                    }
                }
                c.Day = Math.Min(6, c.Day);
                c.DurationMin = Math.Max(1, Math.Min(600, c.DurationMin));
                c.RobPercent = Math.Min(100, c.RobPercent);
                c.RankRows = Math.Max(1, Math.Min(30, c.RankRows));
                c.HudIntervalSec = Math.Max(2, c.HudIntervalSec);
                if (c.BossHp < 1) c.BossHp = 1;
            }
            catch (Exception e) { NgwEngine.Error(e); }
            return c;
        }
    }

    public sealed class NgwGuildResult
    {
        public uint GuildId;
        public string Name = "";
        public long Points, PillarDamage;
        public uint CpShare;
    }

    public static class NgwEngine
    {
        public const ushort IdNewSynWar = 2061;
        public const uint LeftGateId = 3000, RightGateId = 3001, BuffOneId = 100009;// lua IsLeftDoor / IsBuffOne

        // ------------------------------------------------------------------ seams
        public static string RootOverride;
        public static Func<DateTime> ClockOverride;
        public static DateTime Now { get { var c = ClockOverride; return c != null ? c() : DateTime.Now; } }
        /// <summary>Test seam: packets "sent" by the engine go here instead of the socket when set.</summary>
        public static Action<uint, byte[]> SendOverride;

        public static string Root
        {
            get
            {
                if (!string.IsNullOrEmpty(RootOverride)) return RootOverride;
                string db = Program.ServerConfig.DbLocation;
                if (string.IsNullOrEmpty(db)) db = Path.Combine(Environment.CurrentDirectory, "Infinity[DB]");
                return Path.Combine(db, "NewGuildWar");
            }
        }

        static NgwConfig _cfg;
        public static NgwConfig Config { get { var c = _cfg; if (c == null) { c = NgwConfig.Load(Path.Combine(Root, "config.ini")); _cfg = c; } return c; } }
        public static void ReloadConfig() { _cfg = null; }

        // ------------------------------------------------------------------ war state (guarded by Sync)
        sealed class Hero { public uint Uid, GuildId; public string Name = ""; public long Points; public Dictionary<uint, DateTime> Granted = new Dictionary<uint, DateTime>(); }

        static readonly object Sync = new object();
        static bool _active;
        static string _warId = "";
        static DateTime _start, _end;
        static readonly Dictionary<uint, long> _pillarDmg = new Dictionary<uint, long>();
        static readonly Dictionary<uint, long> _bossDmg = new Dictionary<uint, long>();
        static readonly Dictionary<uint, Hero> _heroes = new Dictionary<uint, Hero>();
        static readonly HashSet<uint> _onMap = new HashSet<uint>();
        static uint _leftBreachedBy, _rightBreachedBy;
        static long _pool;
        static int _bossStatus;// 0 not refreshed, 1 refreshed, 2 destroyed (window over)
        static Role.SobNpc _boss;
        static DateTime _lastHud;

        // persisted
        static string _paidWarId = "";
        static uint _pendingLeaderUid; static int _pendingLeaderCps; static string _pendingWarId = "";
        static List<NgwGuildResult> _lastResults = new List<NgwGuildResult>();
        static bool _stateLoaded;

        static Timer _timer;
        static int _ticking;

        public static bool Active { get { lock (Sync) return _active; } }

        // ------------------------------------------------------------------ start (Program.cs hook)
        public static void Start()
        {
            try
            {
                LoadState();
                _timer = new Timer(_ => Tick(), null, 5000, 1000);
                var c = Config;
                MyConsole.WriteLine("[NewGuildWar] " + (c.Enabled != 0 ? "on" : "off") + " - " + (DayOfWeek)c.Day + " " + (c.StartHHMM / 100).ToString("00") + ":" + (c.StartHHMM % 100).ToString("00")
                    + " for " + c.DurationMin + " min on map " + c.Map + " (Infinity[DB]\\NewGuildWar\\config.ini).", ConsoleColor.Cyan);
            }
            catch (Exception e) { Error(e); }
        }

        public static void Error(Exception e) { try { MyConsole.SaveException(e); } catch { } }

        static readonly object _logLock = new object();
        public static void Log(string line)
        {
            try
            {
                lock (_logLock)
                {
                    Directory.CreateDirectory(Root);
                    File.AppendAllText(Path.Combine(Root, "log.txt"), Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine, new UTF8Encoding(false));
                }
            }
            catch { }
        }

        // ------------------------------------------------------------------ persistence
        static string StateFile { get { return Path.Combine(Root, "state.txt"); } }

        public static void LoadState()
        {
            lock (Sync)
            {
                _stateLoaded = true;
                _lastResults = new List<NgwGuildResult>();
                if (!File.Exists(StateFile)) return;
                foreach (var l in File.ReadAllLines(StateFile, Encoding.UTF8))
                {
                    var p = l.Split('\t');
                    if (p.Length >= 2 && p[0] == "PaidWarId") _paidWarId = p[1];
                    else if (p.Length >= 4 && p[0] == "PendingLeader") { uint.TryParse(p[1], out _pendingLeaderUid); int.TryParse(p[2], out _pendingLeaderCps); _pendingWarId = p[3]; }
                    else if (p.Length >= 6 && p[0] == "R")
                    {
                        var r = new NgwGuildResult();
                        uint.TryParse(p[1], out r.GuildId); long.TryParse(p[2], out r.Points); long.TryParse(p[3], out r.PillarDamage); uint.TryParse(p[4], out r.CpShare);
                        r.Name = p[5];
                        _lastResults.Add(r);
                    }
                }
            }
        }

        static void SaveState_NoLock()
        {
            var lines = new List<string>
            {
                "PaidWarId\t" + _paidWarId,
                "PendingLeader\t" + _pendingLeaderUid + "\t" + _pendingLeaderCps + "\t" + _pendingWarId,
            };
            foreach (var r in _lastResults)
                lines.Add(string.Join("\t", "R", r.GuildId, r.Points, r.PillarDamage, r.CpShare, (r.Name ?? "").Replace('\t', ' ')));
            Directory.CreateDirectory(Root);
            string tmp = StateFile + ".tmp";
            File.WriteAllLines(tmp, lines, new UTF8Encoding(false));
            if (File.Exists(StateFile)) File.Replace(tmp, StateFile, StateFile + ".bak", true);
            else File.Move(tmp, StateFile);
        }

        // ------------------------------------------------------------------ schedule
        public static string WarIdFor(DateTime start) { return start.ToString("yyyyMMddHHmm"); }

        /// <summary>The war window that contains <paramref name="now"/> (or the next one when not inside).</summary>
        public static bool Window(DateTime now, out DateTime start, out DateTime end)
        {
            var c = Config;
            start = now.Date.AddHours(c.StartHHMM / 100).AddMinutes(c.StartHHMM % 100);
            int add = ((c.Day - (int)now.DayOfWeek) + 7) % 7;
            start = start.AddDays(add);
            end = start.AddMinutes(c.DurationMin);
            if (add == 0 && now >= end) { start = start.AddDays(7); end = start.AddMinutes(c.DurationMin); }
            return now >= start && now < end;
        }

        static Qahr.Mobile.Role.GameMap WarMap
        {
            get
            {
                try { return Pool.ServerMaps.ContainsKey(Config.Map) ? Pool.ServerMaps[Config.Map] : null; } catch { return null; }
            }
        }

        static Role.SobNpc Furniture(Role.SobNpc.StaticMesh which)
        {
            try
            {
                var gw = Game.MsgTournaments.MsgSchedules.GuildWar;
                if (gw == null || gw.Furnitures == null) return null;
                Role.SobNpc n;
                return gw.Furnitures.TryGetValue(which, out n) ? n : null;
            }
            catch { return null; }
        }
        static bool ClassicRunning
        {
            get
            {
                try { var gw = Game.MsgTournaments.MsgSchedules.GuildWar; return gw != null && gw.Proces != Game.MsgTournaments.ProcesType.Dead; }
                catch { return false; }
            }
        }

        // ------------------------------------------------------------------ tick
        public static void Tick()
        {
            if (Interlocked.Exchange(ref _ticking, 1) == 1) return;
            try
            {
                if (!_stateLoaded) LoadState();
                var c = Config;
                var now = Now;
                DateTime s, e;
                bool inWindow = Window(now, out s, out e);
                bool active = Active;
                if (c.Enabled != 0 && inWindow && !active && _paidWarId != WarIdFor(s) && !ClassicRunning)
                    BeginWar(s, e);
                else if (active && (now >= _end || c.Enabled == 0))
                    FinishWar();
                if (Active) WarTick(now);
                else PreWarTick(now, s);
                DeliverLeaderReward();
            }
            catch (Exception ex) { Error(ex); }
            finally { Interlocked.Exchange(ref _ticking, 0); }
        }

        /// <summary>Starts a war now (window [start, end)). Public for the GM / test harness.</summary>
        public static void BeginWar(DateTime start, DateTime end)
        {
            lock (Sync)
            {
                if (_active) return;
                _active = true;
                _warId = WarIdFor(start);
                _start = start; _end = end;
                _pillarDmg.Clear(); _bossDmg.Clear(); _heroes.Clear(); _onMap.Clear();
                _leftBreachedBy = _rightBreachedBy = 0;
                _pool = Config.HeroPool;
                _bossStatus = 0;
                _lastHud = DateTime.MinValue;
            }
            Log("start war=" + _warId + " end=" + end.ToString("HH:mm"));
            WithStream(stream =>
            {
                if (!ClassicRunning)
                    try { Game.MsgTournaments.MsgSchedules.GuildWar.ResetFurnitures(stream); } catch (Exception ex) { Error(ex); }
                Announce(stream, "The New Guild War has started on the Guild War map! The guild with the most points wins.");
            });
            try { Game.MsgTournaments.MsgSchedules.SendInvitation("New Guild War", "Guild CP Fund", 200, 254, (ushort)Config.Map, 0, 60, Game.MsgServer.MsgStaticMessage.Messages.GuildWar); } catch { }
        }

        static void Announce(ServerSockets.Packet stream, string text)
        {
            if (SendOverride != null) { Log("announce " + text); return; }
            try
            {
                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(text, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(text, Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }
            catch (Exception e) { Error(e); }
        }

        static void WithStream(Action<ServerSockets.Packet> a)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                a(rec.GetStream());
        }

        static IEnumerable<Client.GameClient> PlayersOnMap()
        {
            uint map = Config.Map;
            foreach (var kv in Pool.GamePoll)
            {
                var c = kv.Value;
                if (c == null || c.Player == null || !c.FullLoading || c.Fake) continue;
                if (c.Player.Map == map) yield return c;
            }
        }

        static void SendTo(Client.GameClient c, ServerSockets.Packet p)
        {
            if (SendOverride != null)
            {
                var b = PacketBytes(p);
                SendOverride(c != null && c.Player != null ? c.Player.UID : 0, b);
                return;
            }
            c.Send(p);
        }
        public static unsafe byte[] PacketBytes(ServerSockets.Packet p)
        {
            var b = new byte[p.Size];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)p.Memory, b, 0, p.Size);
            return b;
        }

        public static ServerSockets.Packet Build(ServerSockets.Packet stream, NewSynWarPB pb)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pb);
            stream.Finalize(IdNewSynWar);
            return stream;
        }

        static void PreWarTick(DateTime now, DateTime nextStart)
        {
            var c = Config;
            if (c.Enabled == 0 || c.PreCountdownMin <= 0) return;
            double left = (nextStart - now).TotalSeconds;
            if (left <= 0 || left > c.PreCountdownMin * 60) return;
            if (now.Second % 30 != 0) return;// refresh the start countdown twice a minute
            WithStream(stream =>
            {
                foreach (var p in PlayersOnMap())
                    SendTo(p, Build(stream, new NewSynWarPB { Action = 6, f3 = 0, f18 = (uint)left }));
            });
        }

        static void WarTick(DateTime now)
        {
            var c = Config;
            var map = WarMap;
            WithStream(stream =>
            {
                // pillar restores the moment it is destroyed (classic code leaves it at 0 HP when its own war is off)
                var pole = Furniture(Role.SobNpc.StaticMesh.Pole);
                if (pole != null && pole.HitPoints <= 0) RestoreHp(stream, pole);

                // Honor Pillar window
                DateTime bossStart = _start.AddMinutes(c.BossOffsetMin), bossEnd = bossStart.AddMinutes(c.BossMinutes);
                if (_boss == null && _bossStatus == 0 && now >= bossStart && now < bossEnd && map != null) SpawnBoss(stream, map);
                if (_boss != null && now >= bossEnd) { DespawnBoss(stream); lock (Sync) _bossStatus = 2; }
                if (_boss != null && _boss.HitPoints <= 0)
                {
                    BroadcastMap(stream, new NewSynWarPB { Action = 7, f13 = 2 });
                    RestoreHp(stream, _boss);
                    BroadcastMap(stream, new NewSynWarPB { Action = 7, f13 = 1 });
                }

                // presence: new arrivals get the countdown, leavers / guild changes / long-dead lose their points
                var present = new HashSet<uint>();
                foreach (var p in PlayersOnMap())
                {
                    present.Add(p.Player.UID);
                    bool fresh;
                    lock (Sync) fresh = _onMap.Add(p.Player.UID);
                    if (fresh) SendTo(p, Build(stream, new NewSynWarPB { Action = 5, f3 = (ulong)Math.Max(0, (_end - now).TotalSeconds) }));
                }
                lock (Sync)
                {
                    _onMap.RemoveWhere(u => !present.Contains(u));
                    foreach (var h in _heroes.Values.ToList())
                    {
                        if (h.Points <= 0) continue;
                        var cl = OnlineClient(h.Uid);
                        bool lose = cl == null || !present.Contains(h.Uid) || cl.Player.GuildID != h.GuildId || cl.Player.MyGuild == null;
                        if (!lose && !cl.Player.Alive && (now - cl.Player.DeadStamp).TotalSeconds >= c.DeadRecycleSec) lose = true;
                        if (lose)
                        {
                            _pool += h.Points;
                            Log("recycle uid=" + h.Uid + " points=" + h.Points);
                            h.Points = 0;
                        }
                    }
                }

                if ((now - _lastHud).TotalSeconds >= c.HudIntervalSec)
                {
                    _lastHud = now;
                    var guilds = GuildRanking().Take(c.RankRows).Select(r => new NewSynWarListPB { Id = r.GuildId, Value = (ulong)Math.Max(0, r.Points), Name = r.Name ?? "" }).ToList();
                    var heroes = HeroRanking(c.RankRows);
                    foreach (var p in PlayersOnMap())
                    {
                        SendTo(p, Build(stream, new NewSynWarPB { Action = 0, f2 = 0, f16 = true, f17 = true, f19 = guilds }));
                        SendTo(p, Build(stream, new NewSynWarPB { Action = 1, f3 = (ulong)HeroPoints(p.Player.UID), f16 = true, f19 = heroes }));
                    }
                }
            });
        }

        static Client.GameClient OnlineClient(uint uid)
        {
            Client.GameClient c;
            return Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null ? c : null;
        }

        static void RestoreHp(ServerSockets.Packet stream, Role.SobNpc npc)
        {
            npc.HitPoints = npc.MaxHitPoints;
            if (SendOverride != null) return;
            var upd = new Game.MsgServer.MsgUpdate(stream, npc.UID, 2);
            stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, (long)npc.HitPoints);
            stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, (long)npc.MaxHitPoints);
            stream = upd.GetArray(stream);
            foreach (var p in PlayersOnMap()) p.Send(stream);
        }

        static void SpawnBoss(ServerSockets.Packet stream, Role.GameMap map)
        {
            var c = Config;
            var pole = Furniture(Role.SobNpc.StaticMesh.Pole);
            ushort x = (ushort)c.BossX, y = (ushort)c.BossY;
            if (!FindFreeTile(map, ref x, ref y)) { Log("boss: no walkable tile near " + c.BossX + "," + c.BossY); lock (Sync) _bossStatus = 2; return; }
            if (map.View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, c.BossUid) != null) { Log("boss: uid " + c.BossUid + " already on the map"); lock (Sync) _bossStatus = 2; return; }
            var npc = new Role.SobNpc();
            npc.UID = c.BossUid;
            npc.ObjType = Role.MapObjectType.SobNpc;
            npc.Map = c.Map;
            npc.X = x; npc.Y = y;
            npc.Mesh = pole != null ? pole.Mesh : Role.SobNpc.StaticMesh.Pole;
            npc.Type = Role.Flags.NpcType.Pole;
            npc.Sort = pole != null ? pole.Sort : (ushort)17;
            npc.Name = "HonorPillar";
            npc.MaxHitPoints = c.BossHp;
            npc.HitPoints = c.BossHp;
            map.View.EnterMap<Role.IMapObj>(npc);
            map.SetFlagNpc(x, y);
            lock (Sync) { _boss = npc; _bossStatus = 1; }
            Log("boss spawned at " + x + "," + y);
            if (SendOverride == null)
                foreach (var p in PlayersOnMap()) p.Send(npc.GetArray(stream, false));
            BroadcastMap(stream, new NewSynWarPB { Action = 7, f13 = 1 });
            Announce(stream, "The Honor Pillar has appeared in the New Guild War!");
        }

        static bool FindFreeTile(Role.GameMap map, ref ushort x, ref ushort y)
        {
            for (int r = 0; r <= 6; r++)
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx <= 0 || ny <= 0) continue;
                        try
                        {
                            if (map.IsFlagPresent(nx, ny, Role.MapFlagType.Valid)) { x = (ushort)nx; y = (ushort)ny; return true; }
                        }
                        catch { }
                    }
            return false;
        }

        static void DespawnBoss(ServerSockets.Packet stream)
        {
            Role.SobNpc npc;
            lock (Sync) { npc = _boss; _boss = null; }
            if (npc == null) return;
            var map = WarMap;
            try
            {
                if (map != null)
                {
                    map.View.LeaveMap<Role.IMapObj>(npc);
                    map.RemoveFlagNpc(npc.X, npc.Y);
                }
                if (SendOverride == null)
                {
                    var pk = Game.MsgServer.MsgDataPacket.ActionCreate(stream, new Game.MsgServer.ActionQuery { ObjId = npc.UID, Type = Game.MsgServer.ActionType.RemoveEntity });
                    foreach (var p in PlayersOnMap()) p.Send(pk);
                }
            }
            catch (Exception e) { Error(e); }
            Log("boss removed");
        }

        static void BroadcastMap(ServerSockets.Packet stream, NewSynWarPB pb)
        {
            foreach (var p in PlayersOnMap()) SendTo(p, Build(stream, pb));
        }

        // ------------------------------------------------------------------ scoring
        static bool Eligible(Role.Instance.Guild g)
        {
            return g != null && g.Info != null && g.Info.Level >= (uint)Config.MinGuildLevel;
        }

        static uint TopBy(Dictionary<uint, long> d)
        {
            uint best = 0; long bv = 0;
            foreach (var kv in d) if (kv.Value > bv || (kv.Value == bv && bv > 0 && kv.Key < best)) { best = kv.Key; bv = kv.Value; }
            return bv > 0 ? best : 0;
        }

        static string GuildName(uint gid)
        {
            Role.Instance.Guild g;
            return gid != 0 && Role.Instance.Guild.GuildPoll.TryGetValue(gid, out g) && g != null ? (g.GuildName ?? "") : "";
        }

        /// <summary>Current points per guild (official split), highest first.</summary>
        public static List<NgwGuildResult> GuildRanking()
        {
            var c = Config;
            var pts = new Dictionary<uint, long>();
            var dmg = new Dictionary<uint, long>();
            lock (Sync)
            {
                long total = _pillarDmg.Values.Sum();
                foreach (var kv in _pillarDmg)
                {
                    dmg[kv.Key] = kv.Value;
                    if (total > 0) Add(pts, kv.Key, (long)Math.Floor((double)c.PillarPoints * kv.Value / total));
                }
                uint leader = TopBy(_pillarDmg);
                uint left = _leftBreachedBy != 0 ? _leftBreachedBy : leader;
                uint right = _rightBreachedBy != 0 ? _rightBreachedBy : leader;
                if (left != 0) Add(pts, left, c.GatePoints);
                if (right != 0) Add(pts, right, c.GatePoints);
                uint honor = TopBy(_bossDmg);
                if (honor != 0) Add(pts, honor, c.HonorPoints);
                foreach (var h in _heroes.Values) if (h.Points > 0 && h.GuildId != 0) Add(pts, h.GuildId, h.Points);
            }
            return pts.Where(kv => kv.Value > 0).Select(kv =>
            {
                long d; dmg.TryGetValue(kv.Key, out d);
                return new NgwGuildResult { GuildId = kv.Key, Name = GuildName(kv.Key), Points = kv.Value, PillarDamage = d };
            }).OrderByDescending(r => r.Points).ThenByDescending(r => r.PillarDamage).ThenBy(r => r.GuildId).ToList();
        }
        static void Add(Dictionary<uint, long> d, uint k, long v) { long o; d.TryGetValue(k, out o); d[k] = o + v; }

        static List<NewSynWarListPB> HeroRanking(int rows)
        {
            lock (Sync)
                return _heroes.Values.Where(h => h.Points > 0).OrderByDescending(h => h.Points).ThenBy(h => h.Uid).Take(rows)
                    .Select(h => new NewSynWarListPB { Id = h.Uid, Value = (ulong)h.Points, Name = h.Name ?? "" }).ToList();
        }

        public static long HeroPoints(uint uid)
        {
            lock (Sync) { Hero h; return _heroes.TryGetValue(uid, out h) ? h.Points : 0; }
        }
        public static long PoolLeft { get { lock (Sync) return _pool; } }

        // ------------------------------------------------------------------ combat hooks
        /// <summary>CanAttackNpc hook: null = not ours (fall through to the normal checks).</summary>
        public static bool? CanAttack(Client.GameClient client, Role.SobNpc npc)
        {
            try
            {
                if (npc == null || client == null || client.Player == null) return null;
                if (!Active || npc.Map != Config.Map) return null;
                bool ours = npc == Furniture(Role.SobNpc.StaticMesh.Pole) || npc == _boss
                    || npc == Furniture(Role.SobNpc.StaticMesh.LeftGate) || npc == Furniture(Role.SobNpc.StaticMesh.RightGate);
                if (!ours) return null;
                if (!Eligible(client.Player.MyGuild)) return false;
                return npc.HitPoints > 0;
            }
            catch (Exception e) { Error(e); return null; }
        }

        /// <summary>ReceiveAttack.Npc hook: called with the final damage before it is applied.</summary>
        public static void OnSobDamage(Client.GameClient client, Role.SobNpc npc, uint damage)
        {
            try
            {
                if (npc == null || client == null || client.Player == null || damage == 0) return;
                if (!Active || npc.Map != Config.Map || npc.HitPoints <= 0) return;
                var g = client.Player.MyGuild;
                if (!Eligible(g)) return;
                uint gid = g.Info.GuildID;
                long dealt = Math.Min((long)damage, (long)npc.HitPoints);
                bool kills = damage >= (uint)npc.HitPoints;
                uint gate = 0;
                lock (Sync)
                {
                    if (!_active) return;
                    if (npc == Furniture(Role.SobNpc.StaticMesh.Pole)) Add(_pillarDmg, gid, dealt);
                    else if (_boss != null && npc == _boss) Add(_bossDmg, gid, dealt);
                    else if (npc == Furniture(Role.SobNpc.StaticMesh.LeftGate)) { if (kills && _leftBreachedBy == 0) { _leftBreachedBy = gid; gate = LeftGateId; } }
                    else if (npc == Furniture(Role.SobNpc.StaticMesh.RightGate)) { if (kills && _rightBreachedBy == 0) { _rightBreachedBy = gid; gate = RightGateId; } }
                }
                if (gate != 0)
                {
                    Log("gate " + gate + " breached by guild " + gid);
                    WithStream(stream => BroadcastMap(stream, new NewSynWarPB { Action = 10, f3 = gate, f15 = uint.MaxValue }));// f15: nobody gets the repair button
                }
            }
            catch (Exception e) { Error(e); }
        }

        /// <summary>Player.Dead hook (after the death is final). killer may be null.</summary>
        public static void OnPlayerDead(Role.Player victim, Role.Player killer)
        {
            try
            {
                if (victim == null || !Active) return;
                var c = Config;
                if (victim.Map != c.Map) return;
                long victimLeft = 0, killerNow = 0;
                bool changed = false;
                lock (Sync)
                {
                    Hero v;
                    _heroes.TryGetValue(victim.UID, out v);
                    if (killer != null && killer.UID != victim.UID && killer.Map == c.Map && Eligible(killer.MyGuild) && killer.GuildID != victim.GuildID)
                    {
                        Hero k;
                        if (!_heroes.TryGetValue(killer.UID, out k)) { k = new Hero { Uid = killer.UID }; _heroes[killer.UID] = k; }
                        if (k.GuildId != killer.GuildID) { _pool += k.Points; k.Points = 0; }// guild changed -> old points back to the pool
                        k.GuildId = killer.GuildID; k.Name = killer.Name ?? "";
                        long rob = v != null && v.Points > 0 ? v.Points * c.RobPercent / 100 : 0;
                        if (rob > 0) { v.Points -= rob; k.Points += rob; changed = true; }
                        DateTime last;
                        bool grant = victim.MyGuild != null && (!k.Granted.TryGetValue(victim.UID, out last) || (Now - last).TotalSeconds >= c.KillGrantCooldownSec);
                        if (grant && _pool > 0)
                        {
                            long gpts = Math.Min(_pool, (long)c.KillGrant);
                            _pool -= gpts; k.Points += gpts; k.Granted[victim.UID] = Now; changed = true;
                        }
                        killerNow = k.Points;
                    }
                    victimLeft = v != null ? v.Points : 0;
                }
                if (!changed && victimLeft <= 0) return;
                WithStream(stream =>
                {
                    if (victimLeft > 0 && victim.Owner != null) SendTo(victim.Owner, Build(stream, new NewSynWarPB { Action = 4, f3 = 0 }));// dead timer
                    if (killer != null && killer.Owner != null && killerNow > 0)
                        SendTo(killer.Owner, Build(stream, new NewSynWarPB { Action = 3, f5 = killer.UID, f3 = (ulong)killerNow }));
                });
            }
            catch (Exception e) { Error(e); }
        }

        // ------------------------------------------------------------------ finish + rewards
        public static void FinishWar()
        {
            List<NgwGuildResult> results;
            string warId;
            lock (Sync)
            {
                if (!_active) return;
                warId = _warId;
            }
            results = GuildRanking();
            var c = Config;
            long sum = results.Sum(r => r.Points);
            foreach (var r in results) r.CpShare = sum > 0 ? (uint)Math.Floor((double)c.CpFund * r.Points / sum) : 0;
            var winner = results.FirstOrDefault();

            bool pay;
            lock (Sync)
            {
                _active = false;
                pay = _paidWarId != warId;
                if (pay)
                {
                    // mark paid + store results BEFORE granting anything (a crash can never pay twice)
                    _paidWarId = warId;
                    _lastResults = results;
                    _pendingLeaderUid = 0; _pendingLeaderCps = 0; _pendingWarId = "";
                    if (winner != null && c.WinnerLeaderCps > 0)
                    {
                        Role.Instance.Guild wg;
                        if (Role.Instance.Guild.GuildPoll.TryGetValue(winner.GuildId, out wg) && wg != null && wg.Info != null && wg.Info.Leaderid != 0)
                        { _pendingLeaderUid = wg.Info.Leaderid; _pendingLeaderCps = c.WinnerLeaderCps; _pendingWarId = warId; }
                    }
                    try { SaveState_NoLock(); }
                    catch (Exception e) { Error(e); }
                }
                _heroes.Clear(); _onMap.Clear();
            }
            if (pay)
            {
                foreach (var r in results)
                {
                    Role.Instance.Guild g;
                    if (r.CpShare == 0 || !Role.Instance.Guild.GuildPoll.TryGetValue(r.GuildId, out g) || g == null || g.Info == null) continue;
                    lock (g) g.Info.ConquerPointFund += r.CpShare;
                    try { g.SendMessajGuild("New Guild War: our guild scored " + r.Points + " points and received " + r.CpShare + " CPs into the Guild Fund."); } catch { }
                    Log("reward war=" + warId + " guild=" + r.GuildId + " (" + r.Name + ") points=" + r.Points + " cpFund+=" + r.CpShare);
                }
            }
            WithStream(stream =>
            {
                DespawnBoss(stream);
                if (!ClassicRunning)
                    try { Game.MsgTournaments.MsgSchedules.GuildWar.ResetFurnitures(stream); } catch (Exception e) { Error(e); }
                Announce(stream, winner != null
                    ? "The New Guild War is over! " + winner.Name + " wins with " + winner.Points + " points."
                    : "The New Guild War is over! No guild scored any points.");
                var rows = results.Take(c.RankRows).Select(r => new NewSynWarListPB { Id = r.GuildId, Value = (ulong)r.Points, Name = r.Name ?? "" }).ToList();
                BroadcastMap(stream, new NewSynWarPB { Action = 0, f16 = true, f17 = true, f19 = rows });
            });
            lock (Sync) _bossStatus = 0;
            Log("finish war=" + warId + " winner=" + (winner != null ? winner.GuildId + " " + winner.Name + " " + winner.Points : "none") + " paid=" + pay);
            DeliverLeaderReward();
        }

        /// <summary>Winner leader's 3,000 CPs: cleared + saved first, then granted through Player.ConquerPoints (logged).</summary>
        static void DeliverLeaderReward()
        {
            Client.GameClient cl = null;
            int cps = 0; string war = "";
            lock (Sync)
            {
                if (_pendingLeaderUid == 0 || _pendingLeaderCps <= 0) return;
                cl = OnlineClient(_pendingLeaderUid);
                if (cl == null || !cl.FullLoading) return;
                cps = _pendingLeaderCps; war = _pendingWarId;
                _pendingLeaderUid = 0; _pendingLeaderCps = 0; _pendingWarId = "";
                try { SaveState_NoLock(); }
                catch (Exception e) { Error(e); return; }
            }
            cl.Player.ConquerPoints += (uint)cps;
            Log("leader-reward war=" + war + " uid=" + cl.Player.UID + " cps=" + cps);
            try { cl.SendSysMesage("New Guild War: as the leader of the winning guild you received " + cps + " CPs.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        // ------------------------------------------------------------------ 2061 info requests (f1 = 2, f2 = 0 | 2)
        public static NewSynWarPB Answer(Client.GameClient c, NewSynWarPB req)
        {
            if (req == null || req.Action != 2) return null;// the client only asks with action 2; the rest are pushes
            uint mode = req.f2 == 2 ? 2u : 0u;
            var cfg = Config;
            var ans = new NewSynWarPB { Action = 2, f2 = mode, f16 = true, f17 = true };
            bool active = Active;
            if (mode == 2)
            {
                ans.f3 = (ulong)Math.Max(0, PoolLeft);
                List<KeyValuePair<uint, long>> dmg;
                lock (Sync) dmg = active ? _pillarDmg.OrderByDescending(k => k.Value).ThenBy(k => k.Key).Take(cfg.RankRows).ToList() : new List<KeyValuePair<uint, long>>();
                if (active)
                    foreach (var kv in dmg) ans.f19.Add(new NewSynWarListPB { Id = kv.Key, Value = (ulong)(kv.Value / 10000), Name = GuildName(kv.Key) });// shown as "%I64dW"
                else
                    lock (Sync) foreach (var r in _lastResults.OrderByDescending(r => r.PillarDamage).Take(cfg.RankRows)) ans.f19.Add(new NewSynWarListPB { Id = r.GuildId, Value = (ulong)(r.PillarDamage / 10000), Name = r.Name ?? "" });
                return ans;
            }
            ans.f3 = (ulong)Math.Max(0, HeroPoints(c != null && c.Player != null ? c.Player.UID : 0));
            ans.f5 = LeftGateId; ans.f10 = BuffOneId;
            ans.f4 = ""; ans.f7 = ""; ans.f9 = ""; ans.f11 = ""; ans.f12 = ""; ans.f14 = "";
            if (active)
            {
                uint leader, left, right, honor; int boss;
                lock (Sync)
                {
                    leader = TopBy(_pillarDmg);
                    left = _leftBreachedBy != 0 ? _leftBreachedBy : leader;
                    right = _rightBreachedBy != 0 ? _rightBreachedBy : leader;
                    honor = TopBy(_bossDmg);
                    boss = _bossStatus;
                }
                ans.f6 = left; ans.f4 = GuildName(left);
                ans.f8 = right; ans.f7 = GuildName(right);
                ans.f12 = GuildName(leader);
                ans.f13 = (uint)boss;
                ans.f14 = GuildName(honor);
                foreach (var r in GuildRanking().Take(cfg.RankRows)) ans.f19.Add(new NewSynWarListPB { Id = r.GuildId, Value = (ulong)r.Points, Name = r.Name ?? "" });
            }
            else
            {
                lock (Sync) foreach (var r in _lastResults.Take(cfg.RankRows)) ans.f19.Add(new NewSynWarListPB { Id = r.GuildId, Value = (ulong)r.Points, Name = r.Name ?? "" });
            }
            return ans;
        }

        public static ServerSockets.Packet Reply(Client.GameClient c, ServerSockets.Packet stream)
        {
            NewSynWarPB req;
            try { req = stream.ProtoBufferDeserialize(new NewSynWarPB()); } catch { return null; }
            var ans = Answer(c, req);
            return ans == null ? null : Build(stream, ans);
        }

        // ------------------------------------------------------------------ test seams
        public static void ResetForTests()
        {
            lock (Sync)
            {
                _active = false; _warId = ""; _pillarDmg.Clear(); _bossDmg.Clear(); _heroes.Clear(); _onMap.Clear();
                _leftBreachedBy = _rightBreachedBy = 0; _pool = 0; _bossStatus = 0; _boss = null;
                _paidWarId = ""; _pendingLeaderUid = 0; _pendingLeaderCps = 0; _pendingWarId = ""; _lastResults = new List<NgwGuildResult>();
                _stateLoaded = false; _cfg = null;
            }
        }
        /// <summary>Test seam: credit pillar / boss damage or hero points without combat objects.</summary>
        public static void TestCredit(uint guildId, long pillar, long boss, uint heroUid, long heroPts, string heroName)
        {
            lock (Sync)
            {
                if (pillar > 0) Add(_pillarDmg, guildId, pillar);
                if (boss > 0) Add(_bossDmg, guildId, boss);
                if (heroUid != 0)
                {
                    Hero h; if (!_heroes.TryGetValue(heroUid, out h)) { h = new Hero { Uid = heroUid }; _heroes[heroUid] = h; }
                    h.GuildId = guildId; h.Name = heroName ?? ""; h.Points += heroPts; _pool -= heroPts;
                }
            }
        }
        public static void TestBreach(bool left, uint guildId) { lock (Sync) { if (left) _leftBreachedBy = guildId; else _rightBreachedBy = guildId; } }
        public static uint PendingLeaderUid { get { lock (Sync) return _pendingLeaderUid; } }
        public static string PaidWarId { get { lock (Sync) return _paidWarId; } }
    }
}
