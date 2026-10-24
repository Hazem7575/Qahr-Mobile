// ============================================================================
//  [HalloweenSenluo2023 2026-10-15] Macabre Realm data: the editable DERIVED numbers (Data\HalloweenSenluo2023\
//  config.json, read on change) and the per-character / server-wide state (players.json, written atomically).
//  Everything official (item ids, texts, maps, NPC looks) lives in the code with its source; only numbers the client
//  does not carry (odds, amounts, costs, progress steps) are in config.json, each marked "derived".
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.HalloweenSenluo2023
{
    public sealed class HsConfig
    {
        // ---- costs / amounts (all derived unless noted)
        public int MinLevel = 80;              // official (ActBtnConfig[13] NeedLevel, Server_Key Text116 "Requirement: Level 80")
        public uint BuyFlameCps = 30;           // CastleBeat2023_BuyItem 2 (item text: "purchased from CP Mall")
        public uint BuyCardCps = 60;            // CastleBeat2023_BuyItem 1
        public int BloodPassFlames = 2;        // Macabre Flames for passing the Blood Altar
        public int SideroadPassFlames = 2;     // Macabre Flames for the correct Death Sideroad exit
        public int BloodKeyChance = 20;        // % per corpse search that the Secret Key appears
        public int BloodKeyGuaranteeAt = 8;    // the key appears at the latest on this search
        public int[] BloodOutcome = { 30, 25, 15, 10, 20 };// bag / supply pack / Black Succubus / ghost / nothing (weights)
        public int SideroadServants = 6;       // servants per passage
        public int ZoneMinutes = 30;           // copy lifetime
        public int HallFlameCost = 1;          // Macabre Flames per palace challenge / mop-up
        public int HallMobs = 6;               // combat monsters before the Count
        public int[] SealOdds = { 30, 30, 30, 30 };// % seal per Count kill (palace detail "Drop Rate")
        public int[] HallOpenDay = { 0, 2, 4, 6 };// days after the festival start each hall opens
        public int ProgressPerPass = 5;        // server-wide passing progress % per first daily pass of a player
        public int RewardPoints = 5000;        // official (luaUiRes 1435-1 msgtip4: "heroes with 5000 Macabre Points can claim rewards")
        public int LordCardCost = 1;           // official item text: Destiny Card "Use to challenge freed Mighty Lord"
        public int[] DangerSteps = { 6, 16, 31, 46, 61, 76, 91, 106 };// official luaUiRes[1435].levelDesc L2..L9 thresholds
        public int RankClaimDay = 6;           // ranking boxes can be claimed on/after this day of November (event ends 11.06)
        public int HallMinutes = 20, LordMinutes = 20;

        public static HsConfig Current = new HsConfig();
        static DateTime _stamp = DateTime.MinValue, _next = DateTime.MinValue;
        public static string Folder { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "HalloweenSenluo2023"); } }
        public static string File_ { get { return Path.Combine(Folder, "config.json"); } }

        public static HsConfig Get()
        {
            var now = DateTime.UtcNow;
            if (now < _next) return Current;
            _next = now.AddSeconds(5);
            try
            {
                if (!File.Exists(File_)) return Current;
                var st = File.GetLastWriteTimeUtc(File_);
                if (st == _stamp) return Current;
                _stamp = st;
                var d = Festivals.FestivalLoader.ReadJson(File_);
                var c = new HsConfig();
                c.MinLevel = I(d, "minLevel", c.MinLevel);
                c.BuyFlameCps = (uint)I(d, "buyFlameCps", (int)c.BuyFlameCps);
                c.BuyCardCps = (uint)I(d, "buyCardCps", (int)c.BuyCardCps);
                c.BloodPassFlames = Clamp(I(d, "bloodPassFlames", c.BloodPassFlames), 0, 20);
                c.SideroadPassFlames = Clamp(I(d, "sideroadPassFlames", c.SideroadPassFlames), 0, 20);
                c.BloodKeyChance = Clamp(I(d, "bloodKeyChance", c.BloodKeyChance), 1, 100);
                c.BloodKeyGuaranteeAt = Clamp(I(d, "bloodKeyGuaranteeAt", c.BloodKeyGuaranteeAt), 1, 15);
                c.BloodOutcome = Ints(d, "bloodOutcome", c.BloodOutcome, 5);
                c.SideroadServants = Clamp(I(d, "sideroadServants", c.SideroadServants), 1, 20);
                c.ZoneMinutes = Clamp(I(d, "zoneMinutes", c.ZoneMinutes), 5, 120);
                c.HallFlameCost = Clamp(I(d, "hallFlameCost", c.HallFlameCost), 1, 20);
                c.HallMobs = Clamp(I(d, "hallMobs", c.HallMobs), 1, 20);
                c.SealOdds = Ints(d, "sealOdds", c.SealOdds, 4).Select(x => Clamp(x, 0, 100)).ToArray();
                c.HallOpenDay = Ints(d, "hallOpenDay", c.HallOpenDay, 4).Select(x => Clamp(x, 0, 60)).ToArray();
                c.ProgressPerPass = Clamp(I(d, "progressPerPass", c.ProgressPerPass), 1, 100);
                c.RewardPoints = Clamp(I(d, "rewardPoints", c.RewardPoints), 0, 10000000);
                c.LordCardCost = Clamp(I(d, "lordCardCost", c.LordCardCost), 1, 10);
                c.RankClaimDay = Clamp(I(d, "rankClaimDay", c.RankClaimDay), 1, 30);
                c.HallMinutes = Clamp(I(d, "hallMinutes", c.HallMinutes), 5, 120);
                c.LordMinutes = Clamp(I(d, "lordMinutes", c.LordMinutes), 5, 120);
                Current = c;
            }
            catch (Exception e) { MyConsole.WriteLine("[HalloweenSenluo2023] config.json ignored: " + e.Message, ConsoleColor.Yellow); }
            return Current;
        }
        static int Clamp(int v, int lo, int hi) { return v < lo ? lo : v > hi ? hi : v; }
        static int I(Dictionary<string, object> d, string k, int def)
        {
            var v = Festivals.FestivalLoader.Val(d, k);
            if (v == null) return def;
            int r; return int.TryParse(v.ToString().Split('.')[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? r : def;
        }
        static int[] Ints(Dictionary<string, object> d, string k, int[] def, int len)
        {
            var a = Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(d, k));
            if (a == null || a.Count != len) return def;
            var r = new int[len];
            for (int i = 0; i < len; i++) { int x; if (!int.TryParse((a[i] ?? "").ToString().Split('.')[0], out x) || x < 0) return def; r[i] = x; }
            return r;
        }
    }

    /// <summary>One character's Macabre Realm state (one festival run).</summary>
    public sealed class HsPlayer
    {
        public uint Uid;
        public string Name = "";
        public long Points;               // Macabre Points (= Macabre Coins obtained)
        public string Day = "";           // yyyy-MM-dd of the daily fields below
        public int Zone;                  // 0 none / 1 Death Sideroad / 2 Blood Altar (chosen today)
        public bool ZonePassed;           // today's zone passed (Macabre Realm open)
        public int Correct;               // Death Sideroad: today's correct passage 1..3
        public int Tried;                 // Death Sideroad: passages already cleared today (bits)
        public int Searches;              // Blood Altar: corpse searches this run
        public bool Key;                  // Blood Altar: Secret Key picked up
        public int HallDaily;             // halls passed today by challenge (bits, progress counted once per day)
        public int HallPassed;            // halls ever passed (bits: mop-up allowed)
        public int Seals;                 // seals (bits 1..4)
        public int Claimed;               // hall server-wide rewards claimed (bits)
        public int LordCount;             // Mighty Lord challenges (danger level)
        public bool LordBeaten;
        public bool RankClaimed;
        public readonly List<HsGrant> Pending = new List<HsGrant>();// rewards that did not fit in the bag
    }

    public static class HsState
    {
        public const string Festival = "HalloweenSenluo2023";
        static readonly object Sync = new object();
        static Dictionary<uint, HsPlayer> _players = new Dictionary<uint, HsPlayer>();
        static int _run = -1;
        public static int[] Progress = new int[4];          // server-wide passing progress % per hall
        public static string[] FirstName = new string[4], FirstGang = new string[4];
        static bool _loaded;
        public static string File_ { get { return Path.Combine(HsConfig.Folder, "players.json"); } }
        /// <summary>Test seam: the festival run number (null = FestivalManager).</summary>
        public static Func<int> RunOverride;

        static int CurrentRun()
        {
            var o = RunOverride;
            if (o != null) return o();
            try { var r = Festivals.FestivalManager.Running(Festival); return r != null ? r.Run : _run; } catch { return _run; }
        }
        public static string Today { get { return HsKit.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); } }

        static void Load()
        {
            if (_loaded) return;
            _loaded = true;
            try
            {
                if (!File.Exists(File_)) return;
                var d = Festivals.FestivalLoader.ReadJson(File_);
                _run = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "run"));
                var g = Festivals.FestivalLoader.Obj(Festivals.FestivalLoader.Val(d, "global"));
                var pr = Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(g, "progress"));
                if (pr != null) for (int i = 0; i < 4 && i < pr.Count; i++) Progress[i] = Math.Min(100, (int)Festivals.FestivalLoader.U(pr[i]));
                var fn = Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(g, "firstName"));
                var fg = Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(g, "firstGang"));
                for (int i = 0; i < 4; i++)
                {
                    FirstName[i] = fn != null && i < fn.Count && fn[i] != null ? fn[i].ToString() : null;
                    FirstGang[i] = fg != null && i < fg.Count && fg[i] != null ? fg[i].ToString() : null;
                    if (FirstName[i] == "") FirstName[i] = null;
                    if (FirstGang[i] == "") FirstGang[i] = null;
                }
                var ps = Festivals.FestivalLoader.Obj(Festivals.FestivalLoader.Val(d, "players"));
                if (ps != null)
                    foreach (var kv in ps)
                    {
                        var p = Festivals.FestivalLoader.Obj(kv.Value);
                        uint uid; if (p == null || !uint.TryParse(kv.Key, out uid)) continue;
                        var s = new HsPlayer { Uid = uid };
                        s.Name = Festivals.FestivalLoader.Str(p, "name") ?? "";
                        s.Points = (long)Festivals.FestivalLoader.UL(Festivals.FestivalLoader.Val(p, "points"));
                        s.Day = Festivals.FestivalLoader.Str(p, "day") ?? "";
                        s.Zone = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "zone"));
                        s.ZonePassed = Festivals.FestivalLoader.Bool(p, "zonePassed") == true;
                        s.Correct = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "correct"));
                        s.Tried = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "tried"));
                        s.Searches = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "searches"));
                        s.Key = Festivals.FestivalLoader.Bool(p, "key") == true;
                        s.HallDaily = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "hallDaily"));
                        s.HallPassed = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "hallPassed"));
                        s.Seals = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "seals"));
                        s.Claimed = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "claimed"));
                        s.LordCount = (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(p, "lordCount"));
                        s.LordBeaten = Festivals.FestivalLoader.Bool(p, "lordBeaten") == true;
                        s.RankClaimed = Festivals.FestivalLoader.Bool(p, "rankClaimed") == true;
                        var pend = Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(p, "pending"));
                        if (pend != null)
                            foreach (var o in pend)
                            {
                                var gd = Festivals.FestivalLoader.Obj(o);
                                if (gd == null) continue;
                                var hg = new HsGrant(Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(gd, "id")), (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(gd, "count")), Festivals.FestivalLoader.Bool(gd, "bound") != false, (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(gd, "minutes")));
                                if (hg.Id != 0 && hg.Count > 0 && hg.Count <= 100000) s.Pending.Add(hg);
                            }
                        _players[uid] = s;
                    }
            }
            catch (Exception e) { MyConsole.WriteLine("[HalloweenSenluo2023] players.json ignored: " + e.Message, ConsoleColor.Yellow); }
        }

        /// <summary>A new festival run wipes the event state (the official event is yearly; points/ranking restart).</summary>
        static void CheckRun()
        {
            int run = CurrentRun();
            if (run < 0 || run == _run) return;
            if (_run >= 0 || _players.Count > 0) { _players.Clear(); Progress = new int[4]; FirstName = new string[4]; FirstGang = new string[4]; }
            _run = run;
            SaveLocked();
        }

        /// <summary>The player's state (created on first use, daily fields reset on a new day). Callers lock on Lock.</summary>
        public static HsPlayer Of(Client.GameClient c)
        {
            lock (Sync)
            {
                Load(); CheckRun();
                HsPlayer p;
                if (!_players.TryGetValue(c.Player.UID, out p)) { p = new HsPlayer { Uid = c.Player.UID }; _players[c.Player.UID] = p; }
                p.Name = c.Player.Name ?? p.Name;
                string today = Today;
                if (p.Day != today)
                {
                    p.Day = today; p.Zone = 0; p.ZonePassed = false; p.Correct = 0; p.Tried = 0; p.Searches = 0; p.Key = false; p.HallDaily = 0;
                }
                return p;
            }
        }
        public static object Lock { get { return Sync; } }

        public static void Save() { lock (Sync) SaveLocked(); }
        static void SaveLocked()
        {
            try
            {
                Directory.CreateDirectory(HsConfig.Folder);
                var root = new Dictionary<string, object>();
                root["_readme"] = "Written by the GameServer [HalloweenSenluo2023] (Macabre Realm). Do not edit while the server runs.";
                root["run"] = _run;
                var g = new Dictionary<string, object>();
                g["progress"] = Progress.Cast<object>().ToList();
                g["firstName"] = FirstName.Select(x => (object)(x ?? "")).ToList();
                g["firstGang"] = FirstGang.Select(x => (object)(x ?? "")).ToList();
                root["global"] = g;
                var ps = new Dictionary<string, object>();
                foreach (var p in _players.Values)
                {
                    var d = new Dictionary<string, object>();
                    d["name"] = p.Name; d["points"] = p.Points; d["day"] = p.Day; d["zone"] = p.Zone; d["zonePassed"] = p.ZonePassed;
                    d["correct"] = p.Correct; d["tried"] = p.Tried; d["searches"] = p.Searches; d["key"] = p.Key; d["hallDaily"] = p.HallDaily;
                    d["hallPassed"] = p.HallPassed; d["seals"] = p.Seals; d["claimed"] = p.Claimed; d["lordCount"] = p.LordCount;
                    d["lordBeaten"] = p.LordBeaten; d["rankClaimed"] = p.RankClaimed;
                    d["pending"] = p.Pending.Select(g => (object)new Dictionary<string, object> { { "id", g.Id }, { "count", g.Count }, { "bound", g.Bound }, { "minutes", g.Minutes } }).ToList();
                    ps[p.Uid.ToString(CultureInfo.InvariantCulture)] = d;
                }
                root["players"] = ps;
                string tmp = File_ + ".tmp";
                File.WriteAllText(tmp, AdminApi.AdminJson.Write(root), new UTF8Encoding(false));
                if (File.Exists(File_)) File.Replace(tmp, File_, null); else File.Move(tmp, File_);
            }
            catch (Exception e) { MyConsole.WriteLine("[HalloweenSenluo2023] could not save players.json: " + e.Message, ConsoleColor.Yellow); }
        }

        /// <summary>Top n by Macabre Points (ties: lower uid first).</summary>
        public static List<HsPlayer> Top(int n)
        {
            lock (Sync)
            {
                Load(); CheckRun();
                return _players.Values.Where(p => p.Points > 0).OrderByDescending(p => p.Points).ThenBy(p => p.Uid).Take(n).ToList();
            }
        }
        public static int RankOf(uint uid)
        {
            lock (Sync)
            {
                Load(); CheckRun();
                HsPlayer me; if (!_players.TryGetValue(uid, out me) || me.Points <= 0) return 0;
                return 1 + _players.Values.Count(p => p.Points > me.Points || (p.Points == me.Points && p.Uid < me.Uid));
            }
        }

        /// <summary>Danger level 1..9 from the Mighty Lord challenge count (official table luaUiRes[1435].levelDesc).</summary>
        public static int Danger(int lordCount)
        {
            var steps = HsConfig.Get().DangerSteps;
            int lv = 1;
            foreach (int s in steps) if (lordCount >= s) lv++;
            return Math.Min(9, lv);
        }

        /// <summary>Harness: forget everything (memory only).</summary>
        public static void ResetForTests() { lock (Sync) { _players.Clear(); Progress = new int[4]; FirstName = new string[4]; FirstGang = new string[4]; _run = -1; _loaded = true; } }
    }
}
