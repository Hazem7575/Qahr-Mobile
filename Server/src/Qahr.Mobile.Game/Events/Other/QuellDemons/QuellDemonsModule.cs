// ============================================================================
//  [QuellDemons 2026-10-14] the official "Quell Demons" boss event (Infernix Ruins), window DlgCoolClothesBossUi (Dialog 1274).
//
//  Official sources (client 7957):
//    - ini\monster.dat (3-Way, key DAC0BDDE x3): 20349 EliteDemon (life 10, lv130), 20350 GhostGeneral / 20351 HerculesAx /
//      20352 XuanyuanMaster (life 300,000,000, lv135, BattleLev 340), 20353 HighestRuler (life 600,000,000, lv140, BattleLev 350,
//      RankingAward=1 "Prize from Damage Rankings"). 23791 (Devourer) is NOT in monster.dat.
//    - luaUiRes["1274"] cxsj1-5 + Server_Key tNewServiceCupExclusive[27002] Text2016-2019: Highest Ruler 20:00 + 21:30 (escapes
//      1 hour later), Ghost General :00, Hercules Ax :20, Xuanyuan Master :40 (not again while one is alive), Devourer Sunday
//      22:00 after 50,000 demons in a week.
//    - Server_Key tNewServerBossAct: spawn points (257,251) (104,172) (323,374) (280,153) (378,262) in the Infernix Ruins, the
//      broadcasts, the mail titles, the ChgMap rules (Twin City / Market only, <= 100M silver, "The place is currently calm").
//    - Dialog_Info["1274-other"]: GoldCupItem 2100253 (Infernix Trophy), Server_Key: 200 Infernix Crystals (3332119).
//  Everything else (map 11885, attack/defence, reward amounts) is DERIVED and lives in Data\QuellDemons\quelldemons.json.
//  The boss lookfaces are not in any client source: a boss whose lookface is 0 in the config is not spawned.
//
//  Server -> client: LuaPush (MsgAction 2099 / 258), dwParam 10053:
//    DlgCoolClothesBossUi_OpenMainView / _UpdateMainView|{['Code']=100,['Data']={['ServerType']=,['BossIndex']=,['BossTime']=,
//        ['AllBossState']={5 x 0|1},['KillNum']=(index 4 only)}}
//    DlgGoldenCupUi_OpenMain|{['Code']=200,['Data']={['IsSynthesized']=0|1,['ItemInfo']='3332119 0 n 0',['PlayerName']='x'|0}}
//  Client -> server (10053): NewServerBossAct_OpenInterface, _UpateInterface</N>i, _CupInterface, _GotoExchangShop, _ChgMap</N>i.
//
//  Hooks (one line each, tagged // [QuellDemons 2026-10-14]):
//    MY System\Program.cs               before FestivalManager.Start()      -> QuellDemonsModule.Init();
//    Base\LuaUi\LuaPush.cs              OpenQuellDemons (first line)        -> if (QuellDemonsModule.Open(client)) return true;
//    Base\LuaUi\LuaPush.cs              HandleCallback (before 10053 branch)-> if (msgId == 10053 && QuellDemonsModule.OnLua(...)) return true;
//    MY Attack\ReceiveAttack\Monster.cs before monster.Dead(...)            -> QuellDemonsModule.OnKill(client, monster);
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgMonster;

namespace Qahr.Mobile.QuellDemons
{
    public static class QuellDemonsModule
    {
        public const uint LuaMsg = 10053;
        public const int BossCount = 5;

        // ------------------------------------------------------------------ test seams (null on the server)
        public static Func<DateTime> ClockOverride;
        /// <summary>Harness: spawns without a map (family, x, y) -> a monster object.</summary>
        public static Func<MonsterFamily, ushort, ushort, MonsterRole> SpawnOverride;
        /// <summary>Harness: receives every world broadcast.</summary>
        public static Action<string> BroadcastOverride;
        /// <summary>Harness: receives every message to one player.</summary>
        public static Action<Client.GameClient, string> TellOverride;
        /// <summary>Harness: teleports (client, map, x, y).</summary>
        public static Action<Client.GameClient, uint, ushort, ushort> TeleportOverride;
        /// <summary>Harness: no real timer.</summary>
        public static bool NoTimer;
        static DateTime Now { get { var f = ClockOverride; return f != null ? f() : DateTime.Now; } }

        static readonly object _lock = new object();
        static Timer _timer;
        static bool _inited;
        static int _ticking;

        // ------------------------------------------------------------------ config
        public sealed class RItem { public uint Id; public uint Count; }
        public sealed class Reward
        {
            public int From, To;
            public string MailTitle = "", MailContent = "";
            public uint Title; public ulong Donation, Silver;
            public List<RItem> Items = new List<RItem>();
            public bool BroadcastCrystals;
        }
        public sealed class BossCfg
        {
            public int Index; public string Name = ""; public uint Monster; public ushort Lookface;
            public ushort X, Y;
            public List<int> Times = new List<int>();// minutes of the day
            public int Minute = -1;// minute of every hour
            public int Weekday = -1, WeekTime = -1, KillsNeeded;
            public int EscapeMinutes, Elites;
            public int LowHpPercent; public List<int> LowHpSummon = new List<int>(); public int LowHpSummonCount = 1;
            public bool Ranked;
            public List<RItem> Drops = new List<RItem>();
        }
        public sealed class Config
        {
            public bool Enabled = true;
            public int ServerType = 3; public ushort MinLevel = 80;
            public uint Map = 11885;
            public List<uint> ChgMapFrom = new List<uint> { 1002, 1036 };
            public ulong ChgMapMaxSilver = 100000000;
            public uint EliteMonster = 20349; public int EliteCount = 3; public ushort EliteLookface;
            public int WeeklyResetDay = (int)DayOfWeek.Monday;
            public int SpawnGraceMinutes = 3;
            public uint CrystalItem = 3332119, TrophyItem = 2100253; public int TrophyCrystals = 200;
            public uint ExchangeShopNpc = 36460, ExchangeShopId = 194;
            public BossCfg[] Bosses = new BossCfg[BossCount];
            public List<Reward> RankRewards = new List<Reward>();
            public Reward JoinReward = new Reward(), EliteLastHit = new Reward();
            public string MailSender = "Infernix";
            public uint MinDamageForReward = 1;
            public BossCfg Boss(int i) { return i >= 0 && i < BossCount ? Bosses[i] : null; }
        }
        static Config _cfg; static string _cfgStamp;
        public static string Folder { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "QuellDemons"); } }
        static string ConfigFile { get { return Path.Combine(Folder, "quelldemons.json"); } }
        static string StateFile { get { return Path.Combine(Folder, "state.json"); } }
        static string LogFile { get { return Path.Combine(Folder, "quelldemons_log.txt"); } }

        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(ConfigFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _cfgStamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists) ReadConfig(c, Festivals.FestivalLoader.ReadJson(ConfigFile));
                    for (int i = 0; i < BossCount; i++) if (c.Bosses[i] == null) c.Bosses[i] = new BossCfg { Index = i };
                    _cfg = c; _cfgStamp = stamp;
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[QuellDemons] quelldemons.json: " + e.Message, ConsoleColor.Yellow);
                    if (_cfg == null) { _cfg = new Config(); for (int i = 0; i < BossCount; i++) _cfg.Bosses[i] = new BossCfg { Index = i }; _cfg.Enabled = false; }
                }
                return _cfg;
            }
        }
        static uint U(object o) { return Festivals.FestivalLoader.U(o); }
        static ulong UL(object o) { return Festivals.FestivalLoader.UL(o); }
        static object V(Dictionary<string, object> d, string k) { return Festivals.FestivalLoader.Val(d, k); }
        static string S(Dictionary<string, object> d, string k) { var v = V(d, k); return v == null ? null : v.ToString(); }
        static List<object> A(object o) { return o as List<object> ?? new List<object>(); }
        static bool B(Dictionary<string, object> d, string k, bool def) { var b = Festivals.FestivalLoader.Bool(d, k); return b ?? def; }
        static int Hm(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return -1;
            var p = s.Trim().Split(':'); int h, m;
            if (p.Length != 2 || !int.TryParse(p[0], out h) || !int.TryParse(p[1], out m) || h < 0 || h > 23 || m < 0 || m > 59) return -1;
            return h * 60 + m;
        }
        static int Day(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return -1;
            DayOfWeek d; return Enum.TryParse(s.Trim(), true, out d) ? (int)d : -1;
        }
        static List<RItem> Items(object o)
        {
            var l = new List<RItem>();
            foreach (var e in A(o))
            {
                var d = e as Dictionary<string, object>; if (d == null) continue;
                uint id = U(V(d, "id")); uint n = V(d, "count") == null ? 1 : U(V(d, "count"));
                if (id == 0 || n == 0) continue;
                l.Add(new RItem { Id = id, Count = Math.Min(n, 100000u) });
            }
            return l;
        }
        static Reward ReadReward(Dictionary<string, object> r)
        {
            var x = new Reward();
            if (r == null) return x;
            x.From = (int)U(V(r, "from")); x.To = (int)U(V(r, "to"));
            x.MailTitle = S(r, "mailTitle") ?? ""; x.MailContent = S(r, "mailContent") ?? "";
            x.Title = U(V(r, "title")); x.Donation = UL(V(r, "donation")); x.Silver = UL(V(r, "silver"));
            x.Items = Items(V(r, "items"));
            x.BroadcastCrystals = B(r, "broadcastCrystals", false);
            return x;
        }
        static void ReadConfig(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.Enabled = B(d, "enabled", true);
            if (V(d, "serverType") != null) c.ServerType = (int)Math.Max(1, Math.Min(3, U(V(d, "serverType"))));
            if (V(d, "minLevel") != null) c.MinLevel = (ushort)U(V(d, "minLevel"));
            if (V(d, "map") != null) c.Map = U(V(d, "map"));
            if (V(d, "chgMapFrom") != null) c.ChgMapFrom = A(V(d, "chgMapFrom")).Select(U).Where(x => x != 0).ToList();
            if (V(d, "chgMapMaxSilver") != null) c.ChgMapMaxSilver = UL(V(d, "chgMapMaxSilver"));
            if (V(d, "eliteMonster") != null) c.EliteMonster = U(V(d, "eliteMonster"));
            if (V(d, "eliteCount") != null) c.EliteCount = (int)Math.Min(10, U(V(d, "eliteCount")));
            c.EliteLookface = (ushort)U(V(d, "eliteLookface"));
            int wd = Day(S(d, "weeklyResetDay")); if (wd >= 0) c.WeeklyResetDay = wd;
            if (V(d, "spawnGraceMinutes") != null) c.SpawnGraceMinutes = (int)Math.Min(30, U(V(d, "spawnGraceMinutes")));
            if (V(d, "crystalItem") != null) c.CrystalItem = U(V(d, "crystalItem"));
            if (V(d, "trophyItem") != null) c.TrophyItem = U(V(d, "trophyItem"));
            if (V(d, "trophyCrystals") != null) c.TrophyCrystals = (int)Math.Max(1, U(V(d, "trophyCrystals")));
            if (V(d, "exchangeShopNpc") != null) c.ExchangeShopNpc = U(V(d, "exchangeShopNpc"));
            if (V(d, "exchangeShopId") != null) c.ExchangeShopId = U(V(d, "exchangeShopId"));
            foreach (var e in A(V(d, "bosses")))
            {
                var b = e as Dictionary<string, object>; if (b == null) continue;
                int i = (int)U(V(b, "index")); if (i < 0 || i >= BossCount) continue;
                var x = new BossCfg { Index = i };
                x.Name = S(b, "name") ?? ""; x.Monster = U(V(b, "monster")); x.Lookface = (ushort)U(V(b, "lookface"));
                x.X = (ushort)U(V(b, "x")); x.Y = (ushort)U(V(b, "y"));
                foreach (var t in A(V(b, "times"))) { int m = Hm(t as string); if (m >= 0) x.Times.Add(m); }
                if (V(b, "minute") != null) x.Minute = (int)Math.Min(59, U(V(b, "minute")));
                x.Weekday = Day(S(b, "weekday")); x.WeekTime = Hm(S(b, "time"));
                x.KillsNeeded = (int)U(V(b, "killsNeeded"));
                x.EscapeMinutes = (int)U(V(b, "escapeMinutes")); x.Elites = (int)Math.Min(10, U(V(b, "elites")));
                x.LowHpPercent = (int)Math.Min(99, U(V(b, "lowHpPercent")));
                x.LowHpSummon = A(V(b, "lowHpSummon")).Select(o => (int)U(o)).Where(o => o > 0 && o < BossCount && o != i).ToList();
                if (V(b, "lowHpSummonCount") != null) x.LowHpSummonCount = (int)Math.Max(1, Math.Min(4, U(V(b, "lowHpSummonCount"))));
                x.Ranked = B(b, "ranked", false);
                x.Drops = Items(V(b, "drops"));
                c.Bosses[i] = x;
            }
            foreach (var e in A(V(d, "rankRewards"))) { var r = e as Dictionary<string, object>; if (r != null) c.RankRewards.Add(ReadReward(r)); }
            c.RankRewards = c.RankRewards.Where(r => r.From >= 1).OrderBy(r => r.From).ToList();
            c.JoinReward = ReadReward(V(d, "joinReward") as Dictionary<string, object>);
            c.EliteLastHit = ReadReward(V(d, "eliteLastHit") as Dictionary<string, object>);
            if (!string.IsNullOrEmpty(S(d, "mailSender"))) c.MailSender = S(d, "mailSender");
            if (V(d, "minDamageForReward") != null) c.MinDamageForReward = U(V(d, "minDamageForReward"));
        }

        // ------------------------------------------------------------------ persistent state (Data\QuellDemons\state.json)
        public sealed class Pending
        {
            public string Id = ""; public uint Uid; public string Name = "";
            public string MailTitle = "", MailContent = "";
            public uint Title; public ulong Donation, Silver;
            public List<RItem> Items = new List<RItem>();
            public bool BroadcastCrystals; public uint Crystals;
            public DateTime Created;
        }
        static string _weekKey = ""; static int _weekKills;
        static string _trophyOwner = "";
        static readonly Dictionary<uint, KeyValuePair<string, int>> _crystals = new Dictionary<uint, KeyValuePair<string, int>>();
        static readonly List<Pending> _pending = new List<Pending>();
        static readonly Dictionary<int, DateTime> _lastSlot = new Dictionary<int, DateTime>();
        static long _serial;
        static bool _dirty;

        public static int WeekKills { get { lock (_lock) { RollWeek(); return _weekKills; } } }
        public static int PendingCount(uint uid) { lock (_lock) return _pending.Count(p => p.Uid == uid); }

        static string WeekKeyOf(DateTime t)
        {
            int back = ((int)t.DayOfWeek - Cfg.WeeklyResetDay + 7) % 7;
            return t.Date.AddDays(-back).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        static void RollWeek()
        {
            string k = WeekKeyOf(Now);
            if (k != _weekKey) { if (_weekKey.Length > 0) Log("week " + _weekKey + " ended with " + _weekKills + " demon(s)"); _weekKey = k; _weekKills = 0; _dirty = true; }
        }

        static void LoadState()
        {
            _pending.Clear(); _crystals.Clear(); _lastSlot.Clear();
            if (!File.Exists(StateFile)) return;
            var d = Festivals.FestivalLoader.ReadJson(StateFile);
            if (d == null) return;
            _weekKey = S(d, "weekKey") ?? ""; _weekKills = (int)U(V(d, "weekKills"));
            _trophyOwner = S(d, "trophyOwner") ?? ""; _serial = (long)UL(V(d, "serial"));
            var cr = V(d, "crystals") as Dictionary<string, object>;
            if (cr != null) foreach (var kv in cr) { var o = kv.Value as Dictionary<string, object>; uint uid; if (o != null && uint.TryParse(kv.Key, out uid)) _crystals[uid] = new KeyValuePair<string, int>(S(o, "name") ?? "", (int)U(V(o, "count"))); }
            var ls = V(d, "lastSlot") as Dictionary<string, object>;
            if (ls != null) foreach (var kv in ls) { int i; DateTime t; if (int.TryParse(kv.Key, out i) && DateTime.TryParseExact(kv.Value as string ?? "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t)) _lastSlot[i] = t; }
            foreach (var e in A(V(d, "pending")))
            {
                var p = e as Dictionary<string, object>; if (p == null) continue;
                var x = new Pending
                {
                    Id = S(p, "id") ?? "", Uid = U(V(p, "uid")), Name = S(p, "name") ?? "",
                    MailTitle = S(p, "mailTitle") ?? "", MailContent = S(p, "mailContent") ?? "",
                    Title = U(V(p, "title")), Donation = UL(V(p, "donation")), Silver = UL(V(p, "silver")),
                    Items = Items(V(p, "items")), BroadcastCrystals = B(p, "broadcastCrystals", false), Crystals = U(V(p, "crystals")),
                };
                DateTime t; x.Created = DateTime.TryParseExact(S(p, "created") ?? "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t) ? t : Now;
                if (x.Uid != 0 && x.Id.Length > 0 && !_pending.Any(q => q.Id == x.Id)) _pending.Add(x);
            }
        }
        static void SaveState()
        {
            try
            {
                var d = new Dictionary<string, object>();
                d["weekKey"] = _weekKey; d["weekKills"] = _weekKills; d["trophyOwner"] = _trophyOwner; d["serial"] = _serial;
                var cr = new Dictionary<string, object>();
                foreach (var kv in _crystals) cr[kv.Key.ToString()] = new Dictionary<string, object> { { "name", kv.Value.Key }, { "count", kv.Value.Value } };
                d["crystals"] = cr;
                var ls = new Dictionary<string, object>();
                foreach (var kv in _lastSlot) ls[kv.Key.ToString()] = kv.Value;
                d["lastSlot"] = ls;
                var pl = new List<object>();
                foreach (var p in _pending)
                    pl.Add(new Dictionary<string, object> {
                        { "id", p.Id }, { "uid", p.Uid }, { "name", p.Name }, { "mailTitle", p.MailTitle }, { "mailContent", p.MailContent },
                        { "title", p.Title }, { "donation", p.Donation }, { "silver", p.Silver }, { "broadcastCrystals", p.BroadcastCrystals }, { "crystals", p.Crystals },
                        { "created", p.Created },
                        { "items", p.Items.Select(i => (object)new Dictionary<string, object> { { "id", i.Id }, { "count", i.Count } }).ToList() } });
                d["pending"] = pl;
                Directory.CreateDirectory(Folder);
                string tmp = StateFile + ".tmp";
                File.WriteAllText(tmp, AdminApi.AdminJson.Write(d), new UTF8Encoding(false));
                if (File.Exists(StateFile)) File.Replace(tmp, StateFile, null); else File.Move(tmp, StateFile);
                _dirty = false;
            }
            catch (Exception e) { MyConsole.WriteLine("[QuellDemons] state save failed: " + e.Message, ConsoleColor.Yellow); }
        }
        static void Log(string s)
        {
            try { Directory.CreateDirectory(Folder); File.AppendAllText(LogFile, Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "  " + s + Environment.NewLine, Encoding.UTF8); } catch { }
        }

        // ------------------------------------------------------------------ live monsters
        sealed class Live { public MonsterRole Mob; public DateTime Spawned; public bool Summoned; public long Serial; }
        static readonly Live[] _live = new Live[BossCount];
        sealed class Elite { public MonsterRole Mob; public int Boss; }
        static readonly Dictionary<uint, Elite> _elites = new Dictionary<uint, Elite>();

        public static bool IsAlive(int index) { lock (_lock) { var l = index >= 0 && index < BossCount ? _live[index] : null; return l != null && l.Mob != null && l.Mob.Alive; } }
        public static MonsterRole BossMob(int index) { lock (_lock) { var l = index >= 0 && index < BossCount ? _live[index] : null; return l == null ? null : l.Mob; } }
        public static int EliteCountAlive { get { lock (_lock) return _elites.Values.Count(e => e.Mob != null && e.Mob.Alive); } }

        // ------------------------------------------------------------------ init / timer
        public static void Init()
        {
            try
            {
                lock (_lock)
                {
                    if (_inited) return;
                    LoadState();
                    RollWeek();
                    var c = Cfg;
                    for (int i = 0; i < BossCount; i++)// don't replay slots missed while the server was down (beyond the grace window)
                        if (!_lastSlot.ContainsKey(i)) _lastSlot[i] = Now.AddMinutes(-c.SpawnGraceMinutes);
                    if (_timer == null && !NoTimer) _timer = new Timer(_ => Tick(), null, 10000, 5000);
                    _inited = true;
                }
                var cfg = Cfg;
                var missing = cfg.Bosses.Where(b => b.Monster != 0 && b.Lookface == 0).Select(b => b.Name).ToList();
                MyConsole.WriteLine("[QuellDemons] ready: map " + cfg.Map + ", ServerType " + cfg.ServerType + ", week kills " + _weekKills + ", pending rewards " + _pending.Count
                    + (missing.Count > 0 ? " | NO LOOKFACE (not spawned): " + string.Join(", ", missing) : ""), missing.Count > 0 ? ConsoleColor.Yellow : ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[QuellDemons] init failed (module off): " + e.Message, ConsoleColor.Yellow);
                MyConsole.SaveException(e);
            }
        }

        /// <summary>Every 5 s: week roll, schedule, low HP, escape, missed kills, reward delivery.</summary>
        public static void Tick()
        {
            if (Interlocked.Exchange(ref _ticking, 1) == 1) return;
            try
            {
                var c = Cfg;
                var now = Now;
                lock (_lock)
                {
                    RollWeek();
                    if (c.Enabled)
                        for (int i = 0; i < BossCount; i++) ScheduleBoss(c, c.Bosses[i], now);
                    for (int i = 0; i < BossCount; i++) WatchBoss(c, i, now);
                    foreach (var kv in _elites.ToArray()) if (kv.Value.Mob == null || !kv.Value.Mob.Alive) _elites.Remove(kv.Key);
                    for (int i = _pending.Count - 1; i >= 0; i--)
                        if ((now - _pending[i].Created).TotalDays > 30) { Log("expired reward " + _pending[i].Id + " (" + _pending[i].Name + ")"); _pending.RemoveAt(i); _dirty = true; }
                }
                DeliverAll();
                lock (_lock) if (_dirty) SaveState();
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            finally { Interlocked.Exchange(ref _ticking, 0); }
        }

        /// <summary>The scheduled appearance times of one boss in [from, to].</summary>
        static IEnumerable<DateTime> Slots(BossCfg b, DateTime from, DateTime to)
        {
            for (var day = from.Date.AddDays(-1); day <= to.Date; day = day.AddDays(1))
            {
                var list = new List<DateTime>();
                foreach (int m in b.Times) list.Add(day.AddMinutes(m));
                if (b.Minute >= 0) for (int h = 0; h < 24; h++) list.Add(day.AddHours(h).AddMinutes(b.Minute));
                if (b.Weekday >= 0 && b.WeekTime >= 0 && (int)day.DayOfWeek == b.Weekday) list.Add(day.AddMinutes(b.WeekTime));
                foreach (var t in list.OrderBy(t => t)) if (t >= from && t <= to) yield return t;
            }
        }
        public static DateTime NextSlot(int index, DateTime now)
        {
            var b = Cfg.Boss(index);
            if (b == null) return DateTime.MaxValue;
            foreach (var t in Slots(b, now.AddSeconds(1), now.AddDays(8))) return t;
            return DateTime.MaxValue;
        }
        static void ScheduleBoss(Config c, BossCfg b, DateTime now)
        {
            if (b == null || b.Monster == 0) return;
            DateTime last; _lastSlot.TryGetValue(b.Index, out last);
            DateTime due = DateTime.MinValue;
            foreach (var t in Slots(b, now.AddMinutes(-c.SpawnGraceMinutes), now)) if (t > last) due = t;
            if (due == DateTime.MinValue) return;
            _lastSlot[b.Index] = due; _dirty = true;
            var l = _live[b.Index];
            if (l != null && l.Mob != null && l.Mob.Alive) { Log(b.Name + " " + due.ToString("HH:mm") + ": still alive, not respawned"); return; }
            if (b.KillsNeeded > 0 && _weekKills < b.KillsNeeded) { Log(b.Name + " " + due.ToString("HH:mm") + ": only " + _weekKills + "/" + b.KillsNeeded + " demons this week"); return; }
            SpawnBoss(c, b, "schedule " + due.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }
        static void WatchBoss(Config c, int i, DateTime now)
        {
            var l = _live[i]; var b = c.Bosses[i];
            if (l == null || l.Mob == null) return;
            if (!l.Mob.Alive)// died without the kill hook (should not happen) -> just forget it
            {
                _live[i] = null; return;
            }
            if (b.EscapeMinutes > 0 && now >= l.Spawned.AddMinutes(b.EscapeMinutes))
            {
                Log(b.Name + " escaped after " + b.EscapeMinutes + " min (HP " + l.Mob.HitPoints + ")");
                Remove(l.Mob); _live[i] = null; return;
            }
            if (b.LowHpPercent > 0 && !l.Summoned && l.Mob.Family != null && l.Mob.Family.MaxHealth > 0
                && (ulong)l.Mob.HitPoints * 100UL < (ulong)l.Mob.Family.MaxHealth * (ulong)b.LowHpPercent)
            {
                l.Summoned = true;
                var free = b.LowHpSummon.Where(h => { var hl = _live[h]; return hl == null || hl.Mob == null || !hl.Mob.Alive; }).OrderBy(h => Program.GetRandom.Next()).Take(b.LowHpSummonCount).ToList();
                foreach (int h in free)
                    if (SpawnBoss(c, c.Bosses[h], "low HP summon by " + b.Name, true))
                        Broadcast(LowBloodText(h));
            }
        }

        // ------------------------------------------------------------------ monster types
        static readonly Dictionary<uint, MonsterFamily> _families = new Dictionary<uint, MonsterFamily>();
        /// <summary>Our monster type (Data\QuellDemons\monsters\&lt;id&gt;-*.ini), registered in Pool.MonsterFamilies when missing
        /// (festival/hot reload replace that dictionary).</summary>
        public static MonsterFamily Family(uint id)
        {
            MonsterFamily f;
            if (!_families.TryGetValue(id, out f))
            {
                string dir = Path.Combine(Folder, "monsters");
                string file = Directory.Exists(dir) ? Directory.GetFiles(dir, id + "-*.ini").FirstOrDefault() : null;
                if (file == null) return null;
                f = global::Qahr.Mobile.Server.ParseMonsterFamily(new WindowsAPI.IniFile(""), file);
                if (f == null || f.ID != id) return null;
                _families[id] = f;
            }
            try
            {
                var cur = Pool.MonsterFamilies;
                if (cur != null && !cur.ContainsKey(id))
                {
                    var next = new Dictionary<uint, MonsterFamily>(cur);
                    next[id] = f;
                    Pool.MonsterFamilies = next;
                }
            }
            catch { }
            return f;
        }

        static MonsterRole SpawnOne(Config c, MonsterFamily fam, ushort lookface, ushort x, ushort y, int spread)
        {
            var copy = fam.Copy();
            copy.Mesh = lookface;
            copy.SpawnX = (ushort)Math.Max(1, x - spread); copy.SpawnY = (ushort)Math.Max(1, y - spread);
            copy.MaxSpawnX = (ushort)(x + spread + 1); copy.MaxSpawnY = (ushort)(y + spread + 1);
            copy.MapID = c.Map; copy.SpawnCount = 1;
            var o = SpawnOverride;
            if (o != null) return o(copy, x, y);
            Role.GameMap map = Pool.ServerMaps[c.Map];
            if (map == null) return null;
            if (map.MonstersColletion == null) map.MonstersColletion = new MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;
            MonsterRole mob = null;
            for (int i = 0; i < 6 && mob == null; i++) mob = map.MonstersColletion.Add(copy, true, 0, true);
            if (mob == null) return null;
            mob.RemoveOnDead = true;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                    mob.Send(mob.GetArray(stream, false));
                }
            }
            catch { }
            return mob;
        }
        static bool SpawnBoss(Config c, BossCfg b, string why, bool summoned = false)
        {
            var fam = Family(b.Monster);
            if (fam == null) { Log(b.Name + " (" + b.Monster + "): no monster type (QuellDemons\\monsters) - not spawned (" + why + ")"); return false; }
            if (b.Lookface == 0) { Log(b.Name + " (" + b.Monster + "): lookface 0 (not in the client) - not spawned (" + why + ")"); return false; }
            var mob = SpawnOne(c, fam, b.Lookface, b.X, b.Y, 1);
            if (mob == null) { Log(b.Name + ": spawn failed on map " + c.Map + " (" + why + ")"); return false; }
            _live[b.Index] = new Live { Mob = mob, Spawned = Now, Serial = ++_serial };
            _dirty = true;
            Log(b.Name + " spawned uid " + mob.UID + " at " + mob.X + "," + mob.Y + " (" + why + ")");
            Broadcast(SpawnText(b));
            if (b.Elites > 0 && c.EliteCount > 0) SpawnElites(c, b.Index, b.X, b.Y, Math.Min(b.Elites, c.EliteCount));
            return true;
        }
        static void SpawnElites(Config c, int boss, ushort x, ushort y, int n)
        {
            var fam = Family(c.EliteMonster);
            if (fam == null || c.EliteLookface == 0) { Log("Elite Demons not spawned (" + (fam == null ? "no monster type" : "eliteLookface 0") + ")"); return; }
            for (int i = 0; i < n; i++)
            {
                var m = SpawnOne(c, fam, c.EliteLookface, x, y, 4);
                if (m != null) _elites[m.UID] = new Elite { Mob = m, Boss = boss };
            }
        }
        static void Remove(MonsterRole mob)
        {
            try
            {
                bool was = mob.Alive;
                mob.RemoveOnDead = true;
                mob.HitPoints = 0;
                if (SpawnOverride != null) return;
                using (var rec = new ServerSockets.RecycledPacket())
                    mob.Send(rec.GetStream().ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                var gm = mob.GMap;
                if (gm != null) { gm.View.LeaveMap<Role.IMapObj>(mob); if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false); }
            }
            catch { }
        }

        // ------------------------------------------------------------------ kill hook (MY Attack\ReceiveAttack\Monster.cs, before monster.Dead)
        public static void OnKill(Client.GameClient killer, MonsterRole monster)
        {
            if (killer == null || killer.Player == null || monster == null || !monster.Alive) return;
            try
            {
                int boss = -1; Live live = null; Elite elite = null;
                lock (_lock)
                {
                    for (int i = 0; i < BossCount; i++)
                        if (_live[i] != null && ReferenceEquals(_live[i].Mob, monster)) { boss = i; live = _live[i]; _live[i] = null; break; }
                    if (boss < 0 && _elites.TryGetValue(monster.UID, out elite) && ReferenceEquals(elite.Mob, monster)) _elites.Remove(monster.UID);
                    else elite = null;
                    if (boss < 0 && elite == null) return;// not ours (or already handled: the entry is removed first -> idempotent)
                    RollWeek();
                    _weekKills++;
                    _dirty = true;
                }
                var c = Cfg;// only for our own monsters (every kill on the server passes through this hook)
                if (elite != null)
                {
                    Log("Elite Demon killed by " + killer.Player.Name + " (" + killer.Player.UID + ")");
                    if (c.EliteLastHit.Items.Count > 0 || c.EliteLastHit.Donation > 0 || c.EliteLastHit.Silver > 0)
                        AddPending("elite-" + monster.UID + "-" + killer.Player.UID, killer.Player.UID, killer.Player.Name, c.EliteLastHit, "Elite Demon");
                    lock (_lock) SaveState();
                    DeliverAll();
                    return;
                }
                var b = c.Bosses[boss];
                // ranking snapshot: the killing blow was scored by Monster.cs just before this hook
                List<MsgBossHarmRankingEntry> hunters;
                lock (monster.scoreObj)
                    hunters = monster.Hunters == null ? new List<MsgBossHarmRankingEntry>()
                        : monster.Hunters.Values.Where(h => h != null && h.HunterScore >= c.MinDamageForReward).OrderByDescending(h => h.HunterScore).ToList();
                Log(b.Name + " killed by " + killer.Player.Name + " (" + killer.Player.UID + "), " + hunters.Count + " hunter(s): "
                    + string.Join(", ", hunters.Take(5).Select(h => h.HunterName + "=" + h.HunterScore)));
                string key = "b" + boss + "-" + live.Serial;
                for (int r = 0; r < hunters.Count; r++)
                {
                    var h = hunters[r];
                    Reward rw = b.Ranked ? c.RankRewards.FirstOrDefault(x => r + 1 >= x.From && (x.To == 0 || r + 1 <= x.To)) : c.JoinReward;
                    if (rw == null) continue;
                    AddPending(key + "-" + h.HunterUID, h.HunterUID, h.HunterName, rw, b.Name);
                }
                Broadcast(DeathText(b));
                Drops(killer, monster, b);
                lock (_lock) SaveState();
                DeliverAll();
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        static void Drops(Client.GameClient killer, MonsterRole monster, BossCfg b)
        {
            if (b.Drops.Count == 0 || SpawnOverride != null || killer.Map == null) return;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var d in b.Drops)
                        for (uint i = 0; i < Math.Min(d.Count, 30u); i++) monster.DropItemID2(killer, d.Id, stream);
                }
            }
            catch { }
        }
        static void AddPending(string id, uint uid, string name, Reward rw, string bossName)
        {
            if (uid == 0) return;
            lock (_lock)
            {
                if (_pending.Any(p => p.Id == id)) return;
                var p = new Pending
                {
                    Id = id, Uid = uid, Name = name ?? "",
                    MailTitle = (rw.MailTitle ?? "").Replace("{boss}", bossName), MailContent = (rw.MailContent ?? "").Replace("{boss}", bossName),
                    Title = rw.Title, Donation = rw.Donation, Silver = rw.Silver, BroadcastCrystals = rw.BroadcastCrystals,
                    Items = rw.Items.Select(i => new RItem { Id = i.Id, Count = i.Count }).ToList(), Created = Now,
                };
                p.Crystals = (uint)p.Items.Where(i => i.Id == Cfg.CrystalItem).Sum(i => (long)i.Count);
                _pending.Add(p);
                _dirty = true;
                Log("reward queued " + id + " for " + name + " (" + uid + ")");
            }
        }

        // ------------------------------------------------------------------ reward delivery (in-game mail; only while the player is online)
        static void DeliverAll()
        {
            List<Pending> todo;
            lock (_lock) todo = _pending.ToList();
            foreach (var p in todo)
            {
                Client.GameClient c;
                if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(p.Uid, out c) || c == null || c.Player == null || !c.FullLoading || c.Warehouse == null || c.Inventory == null) continue;
                Deliver(c, p);
            }
        }
        /// <summary>Removes the reward from the queue and saves BEFORE anything is given (a second call / relog finds nothing).</summary>
        static void Deliver(Client.GameClient c, Pending p)
        {
            lock (_lock)
            {
                if (!_pending.Remove(p)) return;
                if (c.Player.UID != p.Uid) { _pending.Add(p); return; }
                SaveState();
            }
            var cfg = Cfg;
            var parts = new List<string>();
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    bool moneySent = false;
                    foreach (var it in p.Items)
                    {
                        Database.ItemType.DBItem db;
                        if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(it.Id, out db)) { Log("deliver " + p.Id + ": unknown item " + it.Id); continue; }
                        uint left = it.Count;
                        int mails = 0;
                        while (left > 0 && mails < 50)
                        {
                            uint stack = db.StackSize > 1 ? Math.Min(left, (uint)db.StackSize) : 1;
                            uint uidItem = c.Inventory.AddInbox(stream, it.Id, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, (ushort)(stack > 1 ? stack : 0));
                            if (uidItem == 0) break;
                            new PrizeInfo(c, p.MailTitle, cfg.MailSender, p.MailContent, 30 * 24 * 3600, moneySent ? 0 : p.Silver, 0, uidItem, 0, 1);
                            moneySent = true;
                            left -= stack; mails++;
                        }
                        parts.Add(db.Name.Replace('~', ' ') + " x" + (it.Count - left));
                    }
                    if (!moneySent && p.Silver > 0) new PrizeInfo(c, p.MailTitle, cfg.MailSender, p.MailContent, 30 * 24 * 3600, p.Silver, 0, 0, 0, 1);
                    if (p.Silver > 0) parts.Add(p.Silver + " Silver");
                    if (p.Donation > 0 && c.Player.Nobility != null)
                    {
                        c.Player.Nobility.Donation += p.Donation;
                        try { c.Send(stream.NobilityIconCreate(c.Player.Nobility)); Pool.NobilityRanking.UpdateRank(c.Player.Nobility); } catch { }
                        parts.Add(p.Donation + " Donation");
                    }
                    if (p.Title != 0 && Database.TitleStorage.Titles.ContainsKey(p.Title))
                    {
                        try { c.Player.AddSpecialTitle((MsgTitleStorage.TitleType)p.Title, stream); } catch { }
                        parts.Add("title " + Database.TitleStorage.Titles[p.Title].Name.Replace('~', ' '));
                    }
                }
                if (p.Crystals > 0) NoteCrystals(c);
                if (p.BroadcastCrystals && p.Crystals > 0) Broadcast(c.Player.Name + " obtained " + p.Crystals + " Infernix Crystals, making everyone envious! [Link Go~quell~the~demon###3 ffff0000 </F>NewServerBossAct_OpenInterface]");
                Tell(c, "[" + p.MailTitle + "] check your mailbox: " + string.Join(", ", parts) + ".");
                Log("delivered " + p.Id + " to " + c.Player.Name + " (" + c.Player.UID + "): " + string.Join(", ", parts));
            }
            catch (Exception e)
            {
                Log("deliver " + p.Id + " FAILED after dequeue: " + e.Message);
                MyConsole.SaveException(e);
            }
        }

        // ------------------------------------------------------------------ Infernix Crystals / Trophy (the golden cup handbook)
        /// <summary>Infernix Crystals the player holds: bag + unclaimed mail attachments (warehouse 1000).</summary>
        static int HeldCrystals(Client.GameClient c, out int bag)
        {
            bag = 0; int mail = 0; uint id = Cfg.CrystalItem;
            try { bag = c.Inventory.GetCountItem(id); } catch { }
            try
            {
                if (c.Warehouse != null && c.Warehouse.ClientItems != null && c.Warehouse.ClientItems.ContainsKey(1000))
                    foreach (var i in c.Warehouse.ClientItems[1000].Values) if (i.ITEM_ID == id) mail += i.StackSize > 1 ? i.StackSize : 1;
            }
            catch { }
            return bag + mail;
        }
        static void NoteCrystals(Client.GameClient c)
        {
            int bag; int n = HeldCrystals(c, out bag);
            lock (_lock)
            {
                if (n > 0) _crystals[c.Player.UID] = new KeyValuePair<string, int>(c.Player.Name, n); else _crystals.Remove(c.Player.UID);
                _dirty = true;
            }
        }
        public static string CupData(Client.GameClient c)
        {
            var cfg = Cfg;
            int n; bool hasTrophy = false;
            int held = HeldCrystals(c, out n);
            try { hasTrophy = c.Inventory.GetCountItem(cfg.TrophyItem) > 0; } catch { }
            string leader;
            lock (_lock)
            {
                if (hasTrophy && _trophyOwner.Length == 0) { _trophyOwner = c.Player.Name; _dirty = true; Log("Infernix Trophy owner: " + c.Player.Name); }
                if (held > 0) _crystals[c.Player.UID] = new KeyValuePair<string, int>(c.Player.Name, held); else _crystals.Remove(c.Player.UID);
                _dirty = true;
                var top = _crystals.Where(kv => kv.Value.Value > 0).OrderByDescending(kv => kv.Value.Value).ThenBy(kv => kv.Key).FirstOrDefault();
                leader = top.Value.Value > 0 ? top.Value.Key : null;
            }
            var sb = new StringBuilder("{['Code']=200,['Data']={['IsSynthesized']=");
            sb.Append(_trophyOwner.Length > 0 ? 1 : 0);
            if (n > 0) sb.Append(",['ItemInfo']=").Append(LuaUi.LuaPush.Q(cfg.CrystalItem + " 0 " + Math.Min(n, cfg.TrophyCrystals) + " 0"));
            sb.Append(",['PlayerName']=").Append(leader != null ? LuaUi.LuaPush.Q(leader) : "0");
            return sb.Append("}}").ToString();
        }

        // ------------------------------------------------------------------ window (LuaPush hooks)
        public static string MainData(int index)
        {
            var c = Cfg; var now = Now;
            bool[] alive = new bool[BossCount];
            DateTime[] spawned = new DateTime[BossCount];
            lock (_lock)
                for (int i = 0; i < BossCount; i++) { var l = _live[i]; alive[i] = l != null && l.Mob != null && l.Mob.Alive; if (alive[i]) spawned[i] = l.Spawned; }
            int shown = c.ServerType == 1 ? BossCount : BossCount - 1;// ServerType 2/3 hide the Devourer button
            if (index < 0 || index >= BossCount)
            {
                index = -1;
                for (int i = 0; i < shown && index < 0; i++) if (alive[i]) index = i;
                if (index < 0)
                {
                    index = 1;
                    for (int i = 0; i < shown; i++) if (NextSlot(i, now) < NextSlot(index, now)) index = i;
                }
            }
            long secs;
            var b = c.Bosses[index];
            if (alive[index] && b.EscapeMinutes > 0) secs = (long)Math.Ceiling((spawned[index].AddMinutes(b.EscapeMinutes) - now).TotalSeconds);
            else { var t = NextSlot(index, now); secs = t == DateTime.MaxValue ? 0 : (long)Math.Ceiling((t - now).TotalSeconds); }
            secs = Math.Max(1, Math.Min(secs, 8 * 86400));
            var sb = new StringBuilder("{['Code']=100,['Data']={['ServerType']=");
            sb.Append(c.ServerType).Append(",['BossIndex']=").Append(index).Append(",['BossTime']=").Append(secs).Append(",['AllBossState']={");
            for (int i = 0; i < BossCount; i++) { if (i > 0) sb.Append(','); sb.Append(alive[i] ? 1 : 0); }
            sb.Append('}');
            if (index == 4) sb.Append(",['KillNum']=").Append(WeekKills);
            return sb.Append("}}").ToString();
        }

        static readonly Dictionary<uint, DateTime> _rate = new Dictionary<uint, DateTime>();
        static bool Rate(Client.GameClient c, string what, int ms)
        {
            uint key = c.Player.UID ^ (uint)what.GetHashCode();
            lock (_rate)
            {
                DateTime last; var now = Now;
                if (_rate.TryGetValue(key, out last) && (now - last).TotalMilliseconds < ms) return false;
                _rate[key] = now;
                if (_rate.Count > 20000) _rate.Clear();
                return true;
            }
        }

        /// <summary>LuaPush.OpenQuellDemons hook. True = answered here.</summary>
        public static bool Open(Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            try
            {
                var c = Cfg;
                if (!c.Enabled) return false;
                if (!Rate(client, "open", 300)) return true;
                if (client.Player.Level < c.MinLevel) { Tell(client, "You should reach at least Level " + c.MinLevel + " before you can participate in the Quell Demons event."); return true; }
                LuaUi.LuaPush.Send(client, "DlgCoolClothesBossUi_OpenMainView|" + MainData(-1), LuaMsg);
                return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); return false; }
        }

        /// <summary>LuaPush.HandleCallback hook for msg 10053. True = answered here.</summary>
        public static bool OnLua(Client.GameClient client, string func, string[] args)
        {
            if (client == null || client.Player == null || func == null) return false;
            var c = Cfg;
            if (!c.Enabled) return false;
            try
            {
                switch (func)
                {
                    case "NewServerBossAct_UpateInterface":
                        {
                            if (!Rate(client, "upd", 200) || client.Player.Level < c.MinLevel) return true;
                            int i;
                            if (args == null || args.Length == 0 || !int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out i) || i < 0 || i >= BossCount) i = -1;
                            LuaUi.LuaPush.Send(client, "DlgCoolClothesBossUi_UpdateMainView|" + MainData(i), LuaMsg);
                            return true;
                        }
                    case "NewServerBossAct_CupInterface":
                        {
                            if (!Rate(client, "cup", 500)) return true;
                            if (client.Player.Level < c.MinLevel) { Tell(client, "You should reach at least Level " + c.MinLevel + " before you can view the Trophy Manual."); return true; }
                            LuaUi.LuaPush.Send(client, "DlgGoldenCupUi_OpenMain|" + CupData(client), LuaMsg);
                            return true;
                        }
                    case "NewServerBossAct_GotoExchangShop":
                        {
                            if (!Rate(client, "shop", 1000)) return true;
                            if (client.Player.Level < c.MinLevel) { Tell(client, "You should reach at least Level " + c.MinLevel + " before you can participate in the Quell Demons event."); return true; }
                            if (client.MyExchangeShop == null) return true;
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MyExchangeShop.Open(rec.GetStream(), c.ExchangeShopNpc, c.ExchangeShopId);
                            return true;
                        }
                    case "NewServerBossAct_ChgMap":
                    case "NewServerBossAct_GotoMap":
                        {
                            if (!Rate(client, "chg", 3000)) return true;
                            int i;
                            if (args == null || args.Length == 0 || !int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) return true;
                            if (func == "NewServerBossAct_GotoMap") i -= 5;// broadcast links: </N>6/7/8 = Ghost General / Hercules Ax / Xuanyuan Master
                            if (i < 0 || i >= BossCount) return true;
                            ChgMap(client, c, i);
                            return true;
                        }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); return true; }
            return false;
        }

        static void ChgMap(Client.GameClient client, Config c, int i)
        {
            if (client.Player.Level < c.MinLevel) { Tell(client, "You should reach at least Level " + c.MinLevel + " before you can participate in the Quell Demons event."); return; }
            if (client.Player.Map == c.Map) { Tell(client, "You are already in the Infernix Ruins."); return; }
            if (!c.ChgMapFrom.Contains(client.Player.Map)) { Tell(client, "Failed to teleport. Please go to Twin City or the Market to get teleported for the Quell Demons event."); return; }
            if (client.Player.Money > c.ChgMapMaxSilver) { Tell(client, "You are carrying more than " + (c.ChgMapMaxSilver / 1000000) + " million Silver. Please store the Silver in the warehouse before coming."); return; }
            if (!client.Player.Alive) return;
            var b = c.Bosses[i];
            if (!IsAlive(i)) { Tell(client, "The place is currently calm. Please wait for the demons to appear before going into battle."); return; }
            ushort x = b.X, y = b.Y;
            if (TeleportOverride == null)
            {
                try
                {
                    var map = Pool.ServerMaps[c.Map];
                    for (int t = 0; t < 30; t++)
                    {
                        ushort tx = (ushort)(b.X + Program.GetRandom.Next(-6, 7)), ty = (ushort)(b.Y + Program.GetRandom.Next(-6, 7));
                        if (map != null && map.ValidLocation(tx, ty)) { x = tx; y = ty; break; }
                    }
                }
                catch { }
            }
            Tell(client, "PK failure deducts 30% of your Silver. Remote warehouse, flying, and Evocation are unavailable here, and kills raise PK Points.");
            Log(client.Player.Name + " (" + client.Player.UID + ") -> Infernix Ruins " + x + "," + y + " for " + b.Name);
            var o = TeleportOverride;
            if (o != null) o(client, c.Map, x, y);
            else client.Teleport(x, y, c.Map);
        }

        // ------------------------------------------------------------------ texts (client ini\Server_Key.lua tNewServerBossAct)
        static readonly string[] SpawnTexts = {
            "The Highest Ruler has respawned at (257,251) in the Infernix Ruins. Go slay the demon and share massive rewards![Link Go~quell~the~Highest~Ruler###3 ffff0000 </F>NewServerBossAct_OpenInterface]",
            "The Ghost General has respawned at (104,172) in the Infernix Ruins. Go slay the demon and share massive rewards![Link Go~quell~the~demon###3 ffff0000 </F>NewServerBossAct_OpenInterface]",
            "The Hercules Ax has respawned at (323,374) in the Infernix Ruins. Go slay the demon and share massive rewards![Link Go~quell~the~demon###3 ffff0000 </F>NewServerBossAct_OpenInterface]",
            "The Xuanyuan Master has respawned at (280,153) in the Infernix Ruins. Go slay the demon and share massive rewards![Link Go~quell~the~demon###3 ffff0000 </F>NewServerBossAct_OpenInterface]",
            "The Devourer has respawned at (378,262) in the Infernix Ruins. Go slay the demon and share massive rewards![Link Go~quell~the~Devourer###3 ffff0000 </F>NewServerBossAct_OpenInterface]" };
        static string SpawnText(BossCfg b) { return b.Index >= 0 && b.Index < SpawnTexts.Length ? SpawnTexts[b.Index].Replace("(257,251)", "(" + b.X + "," + b.Y + ")").Replace("(104,172)", "(" + b.X + "," + b.Y + ")").Replace("(323,374)", "(" + b.X + "," + b.Y + ")").Replace("(280,153)", "(" + b.X + "," + b.Y + ")").Replace("(378,262)", "(" + b.X + "," + b.Y + ")") : ""; }
        static string DeathText(BossCfg b)
        {
            return "With the efforts of all the heroes, the " + b.Name + " has been defeated, dropping a large number of treasures in its escape. [Link Go~pick~treasures###3 ffff0000 </F>NewServerBossAct_OpenInterface]";
        }
        static string LowBloodText(int helper)
        {
            var n = Cfg.Bosses[helper].Name;
            return "Being severely injured, the Highest Ruler is calling the " + n + " to come to his aid! Go eliminate them and share massive rewards![Link Go~quell~the~" + n.Replace(' ', '~') + "###3 ffff0000 </F>NewServerBossAct_GotoMap</N>" + (helper + 5) + "]";
        }

        static void Broadcast(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var o = BroadcastOverride;
            if (o != null) { o(text); return; }
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    global::Qahr.Mobile.Server.SendGlobalPacket(new MsgMessage(text, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(rec.GetStream()));
            }
            catch { }
        }
        static void Tell(Client.GameClient c, string text)
        {
            var o = TellOverride;
            if (o != null) { o(c, text); return; }
            try { c.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }

        /// <summary>Console / GM helper: spawn a boss now (ignores the schedule). Returns a short status.</summary>
        public static string ForceSpawn(int index)
        {
            var c = Cfg; var b = c.Boss(index);
            if (b == null) return "bad index";
            lock (_lock)
            {
                if (IsAliveNoLock(index)) return b.Name + " is already alive";
                bool ok = SpawnBoss(c, b, "forced");
                if (_dirty) SaveState();
                return ok ? b.Name + " spawned" : b.Name + " NOT spawned (see QuellDemons\\quelldemons_log.txt)";
            }
        }
        static bool IsAliveNoLock(int index) { var l = _live[index]; return l != null && l.Mob != null && l.Mob.Alive; }
    }
}
