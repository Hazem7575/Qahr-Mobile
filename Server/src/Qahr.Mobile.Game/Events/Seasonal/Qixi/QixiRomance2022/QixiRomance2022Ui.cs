// ============================================================================
//  [QixiRomance2022 2026-10-15] Global Qixi 2022 "Romantic Season" (Ethereal Harp / Orphee), client msg 10044
//  tMsg_Info["QiXiFestival"], lua dialogs luaui\dialog\dlgqixifestival (Dialog_Info 1266-1270).
//  Runs while the festival QixiRomance2022 (Data\Festivals\QixiRomance2022) runs.
//
//  PLAYABLE (server-validated, config Data\QixiRomance2022\qixiromance2022.json, state players.txt):
//    main window  Qixi2022Page_MainView -> DlgQiXiFestivalMain_OpenUI {points, star energy, open starfield, total, unlock fx,
//                 harp repaired, red dots, guide} from the player's real state.
//    repair       Qixi2022Page_FixView -> DlgQiXiFestivalRepair_OpenUI; Qixi2022Page_Fix</N>part</N>1|2|3 = repair /
//                 maintain / maintain all: 1 harp material each (Phoenix Wood 3600413 .. Mermaid Tear 3600416), a reward
//                 from the official preview list (Dialog_Info 1267-preview-award) + Love Points.
//    play         Qixi2022Page_PlaySong</N>1..3: 1/3/10 Holy Plectrums 3338068 (1st First Love of the day free), a prize
//                 from the official pool (Dialog_Info 1267-award-pool) -> DlgQiXiFestivalRepair_LotteryResults.
//    Love Points  Qixi2022Page_Rank</N>3 (window "claim") or Weaver Girl -> official milestone rewards (Text314-321).
//    rankings     Qixi2022Page_Rank</N>1|2 -> local top 10 (>= 5000 points, official Text428).
//    memories     Qixi2022Page_MemoryView -> DlgQiXiFestivalMemoirs_OpenUI with the real Memory Stone count.
//    NPCs         Weaver Girl 30788 + Orphee 30789 in Twin City (festival npcs.txt; their dialogs are below, via QixiHooks).
//                 Orphee: flowers/gifts -> harp materials (official table, 150 items a day).
//    items        Silk 3336394 (30/50 -> Love Vow / Love Vow (Eternal)), Apollo`s Blessing 3336395 (90/60 -> Golden Plume
//                 (Guardian) / Love Sound), and the boxes 3336393, 3336415, 3336366, 3336411-3336414 (QixiHooks.UseItem).
//  NOT ON THIS SERVER (message, nothing taken): the Lyra starfields A-D (cross-server; GameMapEx 11840-11843 have no doc in
//    GameMap.dat) and the memory instances (GameMapEx 11845-11852, no doc) -> Star Energy / Star Beasts / Memory Stones.
//  Hooks: LuaPush.HandleCallback (msg 10044, 2026-10-14 line), QixiHooks (Procesor + UseItem, 2026-10-15 lines).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.QixiKit;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.QixiRomance2022
{
    public static class QixiRomance2022Ui
    {
        public const uint LuaMsg = 10044;// tMsg_Info["QiXiFestival"]
        public const uint WeaverGirl = 8872001, Orphee = 8872002;// festival ids of the official NPCs 30788 / 30789 (Server_Key tTanabataFestivalNpc), npcs.txt
        public const uint Silk = 3336394, Blessing = 3336395, Plectrum = 3338068, MemoryStone = 3336365;
        public const uint RepairPack = 3336393, PerformancePack = 3336415, MemoryPack = 3336366;
        public const uint LoveSoundBox = 3336411, GoldenPlumeBox = 3336412, LoveVowBox = 3336413, LoveVowEternalBox = 3336414;
        public static readonly uint[] Materials = { 3600413, 3600414, 3600415, 3600416 };// Phoenix Wood, Fairy Flower, Whale Baleen, Mermaid Tear
        static readonly string[] PartNames = { "Harp Body", "Harp Accessory", "Harp Strings", "Harp Soul" };// luaUiRes[1266][1002]

        // ------------------------------------------------------------------ config
        public sealed class Flower { public uint Item; public uint Per; public uint Gives; }
        public sealed class Milestone { public long Points; public List<Grant> Items = new List<Grant>(); }
        public sealed class Config
        {
            public string Festival = "QixiRomance2022";
            public int MinLevel = 80;
            public int RepairStep = 4, RepairPoints = 100, MaintainPoints = 100, PlayPointsPerPlectrum = 100, OneKeyMax = 50;
            public List<int> SongCost = new List<int> { 1, 3, 10 };
            public List<Grant> RepairPool = new List<Grant>();
            public List<List<Grant>> SongPools = new List<List<Grant>>();
            public List<Milestone> Milestones = new List<Milestone>();
            public int OverflowEvery = 1600;
            public List<Grant> OverflowItems = new List<Grant>();
            public int RankMin = 5000, FlowerDailyLimit = 150;
            public List<Flower> Flowers = new List<Flower>();
            public List<int> RepairPackWeights = new List<int> { 25, 25, 25, 25 };
            public List<Grant> PerformancePack = new List<Grant>(), MemoryPackPool = new List<Grant>();
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "QixiRomance2022", "qixiromance2022.json"); } }
        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(CfgFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[QixiRomance2022] qixiromance2022.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            c.MinLevel = Math.Max(1, Math.Min(255, Kit.Int(d, "minLevel", c.MinLevel)));
            c.RepairStep = Math.Max(1, Math.Min(100, Kit.Int(d, "repairStep", c.RepairStep)));
            c.RepairPoints = Math.Max(0, Kit.Int(d, "repairPoints", c.RepairPoints));
            c.MaintainPoints = Math.Max(0, Kit.Int(d, "maintainPoints", c.MaintainPoints));
            c.PlayPointsPerPlectrum = Math.Max(0, Kit.Int(d, "playPointsPerPlectrum", c.PlayPointsPerPlectrum));
            c.OneKeyMax = Math.Max(1, Math.Min(200, Kit.Int(d, "oneKeyMax", c.OneKeyMax)));
            var sc = Kit.Ints(d, "songCost", c.SongCost); if (sc.Count == 3 && sc.All(x => x >= 1 && x <= 100)) c.SongCost = sc;
            c.RepairPool = Kit.ReadPool(FL.Val(d, "repairPool"));
            var sp = FL.Arr(FL.Val(d, "songPools"));
            if (sp != null) foreach (var o in sp) c.SongPools.Add(Kit.ReadPool(o));
            var ms = FL.Arr(FL.Val(d, "milestones"));
            if (ms != null)
                foreach (var o in ms)
                {
                    var md = FL.Obj(o); if (md == null) continue;
                    var m = new Milestone { Points = Kit.Int(md, "points", 0), Items = Kit.ReadPool(FL.Val(md, "items")) };
                    if (m.Points > 0) c.Milestones.Add(m);
                }
            c.Milestones = c.Milestones.OrderBy(m => m.Points).ToList();
            c.OverflowEvery = Math.Max(1, Kit.Int(d, "overflowEvery", c.OverflowEvery));
            c.OverflowItems = Kit.ReadPool(FL.Val(d, "overflowItems"));
            c.RankMin = Math.Max(0, Kit.Int(d, "rankMin", c.RankMin));
            c.FlowerDailyLimit = Math.Max(0, Kit.Int(d, "flowerDailyLimit", c.FlowerDailyLimit));
            var fa = FL.Arr(FL.Val(d, "flowers"));
            if (fa != null)
                foreach (var o in fa)
                {
                    var fd = FL.Obj(o); if (fd == null) continue;
                    var fl = new Flower { Item = FL.U(FL.Val(fd, "item")), Per = FL.U(FL.Val(fd, "per")), Gives = FL.U(FL.Val(fd, "gives")) };
                    if (fl.Item != 0 && fl.Per > 0 && fl.Per <= 1000 && fl.Gives > 0 && fl.Gives <= 1000) c.Flowers.Add(fl);
                }
            var rp = Kit.Ints(d, "repairPackWeights", c.RepairPackWeights); if (rp.Count == 4) c.RepairPackWeights = rp;
            c.PerformancePack = Kit.ReadPool(FL.Val(d, "performancePack"));
            c.MemoryPackPool = Kit.ReadPool(FL.Val(d, "memoryPack"));
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }
        public static readonly PlayerStore Store = new PlayerStore("QixiRomance2022");
        static void Msg(Client.GameClient c, string t) { Kit.Msg(c, t); }
        static void Lua(Client.GameClient c, string call) { Kit.Lua(c, call, LuaMsg); }

        // ------------------------------------------------------------------ per-player state (one season per year)
        // keys: y season year, pt Love Points, part 1..4, prog 0..100, rep 0/1, mc milestones claimed, ov overflow units, seen
        static void Season(uint uid)
        {
            string y = Kit.Now().Year.ToString(CultureInfo.InvariantCulture);
            if (Store.Get(uid, "y") == y) return;
            foreach (var k in new[] { "pt", "prog", "rep", "mc", "ov" }) Store.Set(uid, k, 0);
            Store.Set(uid, "part", 1); Store.Set(uid, "seen", 1); Store.Set(uid, "y", y);
        }
        public static long Points(uint uid) { lock (Store.Sync) { Season(uid); return Store.GetL(uid, "pt"); } }
        static int Part(uint uid) { return Math.Max(1, Math.Min(4, Store.GetI(uid, "part", 1))); }
        static bool Repaired(uint uid) { return Store.GetI(uid, "rep") == 1; }
        static int OpenStarfield(uint uid) { return Repaired(uid) ? 4 : Part(uid); }
        static bool FreeToday(uint uid) { return Store.GetDaily(uid, "free") == 0; }
        static bool MilestoneClaimable(Config cfg, uint uid)
        {
            long pt = Store.GetL(uid, "pt"); int mc = Store.GetI(uid, "mc");
            if (mc < cfg.Milestones.Count && pt >= cfg.Milestones[mc].Points) return true;
            return OverflowDue(cfg, uid) > 0;
        }
        static long OverflowDue(Config cfg, uint uid)
        {
            if (cfg.Milestones.Count == 0 || cfg.OverflowItems.Count == 0 || Store.GetI(uid, "mc") < cfg.Milestones.Count) return 0;
            long top = cfg.Milestones[cfg.Milestones.Count - 1].Points;
            long units = Math.Max(0, (Store.GetL(uid, "pt") - top) / cfg.OverflowEvery);
            return Math.Max(0, units - Store.GetL(uid, "ov"));
        }

        static readonly HashSet<string> Funcs = new HashSet<string>(StringComparer.Ordinal)
        {
            "Qixi2022Page_MainView", "Qixi2022Page_Rank", "QixiFestivalCross2022_EnterOS", "Qixi2022Page_FixView", "Qixi2022Page_MemoryView",
            "Qixi2022Page_Fix", "Qixi2022Page_PlaySong", "QiXiMemoryPart_Event",
        };

        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse((a[i] ?? "").Trim(), out v) ? v : -1; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10044).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Funcs.Contains(func)) return false;
            if (Kit.Throttled("qx22", c.Player.UID, 300)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, "You can`t enter the starfield of Lyra at the moment. Please come back during the event time."); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "You must reach level " + cfg.MinLevel + " to enter the starfield of Lyra. Please level up first."); return true; }
            uint uid = c.Player.UID;
            switch (func)
            {
                case "Qixi2022Page_MainView": OpenMain(c); return true;
                case "Qixi2022Page_FixView": OpenRepair(c, 0); return true;
                case "Qixi2022Page_MemoryView": OpenMemoirs(c); return true;
                case "QixiFestivalCross2022_EnterOS":
                    {
                        int i = Arg(args, 0);
                        lock (Store.Sync) { Season(uid); if (i < 1 || i > OpenStarfield(uid)) return true; }
                        Msg(c, "The starfields of Lyra are cross-server maps and aren`t open on this server. Repair materials come from Orphee in Twin City (flowers and gifts).");
                        return true;
                    }
                case "QiXiMemoryPart_Event":
                    if (Kit.Count(c, MemoryStone) == 0) Msg(c, "You must carry the Memory Stone to enter. Do you want to defeat Star Beasts in different starfields to get some?");
                    else Msg(c, "Orphee`s memories aren`t open on this server yet. Keep your Memory Stones.");
                    return true;
                case "Qixi2022Page_Fix": Fix(c, Arg(args, 0), Arg(args, 1)); return true;
                case "Qixi2022Page_PlaySong": Play(c, Arg(args, 0)); return true;
                case "Qixi2022Page_Rank":
                    {
                        int i = Arg(args, 0);
                        if (i == 3) ClaimMilestones(c, null);
                        else if (i == 1 || i == 2) ShowRank(c);
                        return true;
                    }
            }
            return true;
        }

        // ------------------------------------------------------------------ windows
        public static string MainData(Client.GameClient c)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            lock (Store.Sync)
            {
                Season(uid);
                long pt = Store.GetL(uid, "pt");
                int open = OpenStarfield(uid);
                int fx = 0;
                if (open > Store.GetI(uid, "seen", 1)) { fx = 1; Store.Set(uid, "seen", open); Store.Commit(); }
                bool repRed = Repaired(uid) ? (FreeToday(uid) || Kit.Count(c, Plectrum) > 0 || Materials.Any(m => Kit.Count(c, m) > 0))
                                            : Kit.Count(c, Materials[Part(uid) - 1]) > 0;
                int memRed = 0;
                int welfareRed = MilestoneClaimable(cfg, uid) ? 1 : 0;
                // [1] points [2] Star Energy (no starfields here) [3] open starfield [4] total [5] unlock effect [6] harp repaired [7] reds [8] guide
                return "{['Code']=200,['Data']={" + pt + ",0," + open + "," + pt + "," + fx + "," + (Repaired(uid) ? 1 : 0) + ",{" + (repRed ? 1 : 0) + "," + memRed + "," + welfareRed + "},-1}}";
            }
        }
        static void OpenMain(Client.GameClient c) { Lua(c, "DlgQiXiFestivalMain_OpenUI|" + MainData(c)); }

        /// <summary>DlgQiXiFestivalRepair data (page 0 = keep, 1 repair, 2 play).</summary>
        public static string RepairData(Client.GameClient c, int page)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            lock (Store.Sync)
            {
                Season(uid);
                bool rep = Repaired(uid);
                int part = rep ? 4 : Part(uid);
                int prog = rep ? 100 : Math.Max(0, Math.Min(100, Store.GetI(uid, "prog")));
                int left = rep ? 0 : (100 - prog + cfg.RepairStep - 1) / cfg.RepairStep;
                uint pl = Kit.Count(c, Plectrum);
                var mats = Materials.Select(m => Kit.Count(c, m)).ToArray();
                var red = new int[4];
                for (int i = 0; i < 4; i++) red[i] = mats[i] > 0 && (rep || i + 1 <= part) ? 1 : 0;
                bool free = FreeToday(uid);
                int playRed = rep && (free || pl >= 1) ? 0 : 1;// 0 = show
                long pt = Store.GetL(uid, "pt");
                return "{['Code']=200,['Data']={" + (rep ? 1 : 0) + "," + pl + "," + part + "," + prog + "," + left + "," + pt + "," + pt +
                       ",{" + string.Join(",", mats) + "}," + (MilestoneClaimable(cfg, uid) ? 1 : 0) + ",{" + string.Join(",", red) + "}," +
                       (free ? 0 : 1) + ",0," + playRed + ",-1," + (page == 1 || page == 2 ? page : (rep ? 2 : 1)) + "}}";
            }
        }
        static void OpenRepair(Client.GameClient c, int page) { Lua(c, "DlgQiXiFestivalRepair_OpenUI|" + RepairData(c, page)); }
        static void RefreshRepair(Client.GameClient c, int page) { Lua(c, "DlgQiXiFestivalRepair_RefreshRepair|" + RepairData(c, page)); }

        public static string MemoirsData(Client.GameClient c)
        {
            // [1] the 7 memories {id, 0 locked / 1 open / 2 done}: the 1st is open (official start), the rest unlock in order
            // [2] reward preview: the official first-time reward Orphee Memory Pack 3336366, [3] Memory Stones in the bag, [4] red dot
            var sb = new StringBuilder("{['Code']=200,['Data']={{");
            for (int i = 1; i <= 7; i++) sb.Append(i > 1 ? "," : "").Append("{").Append(i).Append(",").Append(i == 1 ? 1 : 0).Append("}");
            sb.Append("},{'3336366 0 1 0'},").Append(Kit.Count(c, MemoryStone)).Append(",0}}");
            return sb.ToString();
        }
        static void OpenMemoirs(Client.GameClient c) { Lua(c, "DlgQiXiFestivalMemoirs_OpenUI|" + MemoirsData(c)); }

        // ------------------------------------------------------------------ repair / maintain
        /// <summary>mode 1 repair, 2 maintain, 3 maintain all (official buttons btn_xiufu / btn_weihu / btn_yijianweihu).</summary>
        public static string Fix(Client.GameClient c, int partIdx, int mode)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            if (partIdx < 1 || partIdx > 4 || mode < 1 || mode > 3) return "bad";
            if (cfg.RepairPool.Count == 0) { Msg(c, "Repairing the Ethereal Harp isn`t available right now."); return "nopool"; }
            uint mat = Materials[partIdx - 1];
            var won = new List<Grant>();
            int used = 0, gained = 0, step = 0;
            lock (Store.Sync)
            {
                Season(uid);
                bool rep = Repaired(uid);
                int part = Part(uid);
                int prog = Store.GetI(uid, "prog");
                if (mode == 1)
                {
                    if (rep || partIdx != part || prog >= 100) return "notrepairable";
                }
                else if (!(rep || partIdx < part)) { Msg(c, "You must repair " + PartNames[partIdx - 1] + " before you maintain it."); return "notrepaired"; }
                uint have = Kit.Count(c, mat);
                int n = mode == 3 ? (int)Math.Min(have, (uint)cfg.OneKeyMax) : 1;
                if (have < 1 || n < 1) { Msg(c, "You don`t have " + Kit.ItemName(mat) + ". Collect some in the starfields or from Orphee in Twin City."); return "nomat"; }
                for (int i = 0; i < n; i++) { int k = Kit.Pick(cfg.RepairPool); if (k >= 0) won.Add(cfg.RepairPool[k]); }
                if (!Kit.HasSpace(c, Merge(won))) { Msg(c, "Please clear up your inventory first."); return "nospace"; }
                if (!Kit.Take(c, mat, (uint)n)) return "nomat";
                used = n;
                if (mode == 1)
                {
                    step = Math.Min(cfg.RepairStep, 100 - prog);
                    prog += step;
                    if (prog >= 100)
                    {
                        if (part < 4) { Store.Set(uid, "part", part + 1); Store.Set(uid, "prog", 0); }
                        else { Store.Set(uid, "prog", 100); Store.Set(uid, "rep", 1); }
                    }
                    else Store.Set(uid, "prog", prog);
                    gained = cfg.RepairPoints;
                }
                else gained = cfg.MaintainPoints * n;
                Store.Add(uid, "pt", gained);
                Store.Commit();
                Kit.Give(c, Merge(won).ToList(), cfg.Festival, (mode == 1 ? "repair " : "maintain ") + PartNames[partIdx - 1] + " x" + n + " (-" + n + " " + mat + ", +" + gained + " pts)");
            }
            RefreshRepair(c, 1);
            if (won.Count == 1)
            {
                Lua(c, "DlgQiXiFestival_MessageTip|{" + Kit.Q(won[0].ClientString()) + "," + (mode == 1 ? 1 : 2) + "," + gained + "," + step + "}");
                Lua(c, "DlgQiXiFestivalRepair_ReceiveAward|{" + Kit.Q(won[0].ClientString()) + "}");
            }
            else
            {
                Lua(c, "DlgQiXiFestivalRepair_ReceiveAward|{" + string.Join(",", Merge(won).Take(10).Select(g => Kit.Q(g.ClientString()))) + "}");
                Msg(c, "Maintained " + PartNames[partIdx - 1] + " " + used + " time(s): +" + gained + " Love Points.");
            }
            return "ok";
        }
        static IEnumerable<Grant> Merge(List<Grant> gs)
        {
            return gs.GroupBy(g => g.Item + "|" + g.Bound + "|" + g.LifeMinutes + "|" + g.Bless)
                     .Select(x => x.Count() == 1 ? x.First() : new Grant(x.First().Item, (uint)x.Sum(g => g.Count), x.First().Bound, x.First().LifeMinutes, x.First().Bless));
        }

        // ------------------------------------------------------------------ play the harp
        public static string Play(Client.GameClient c, int song)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            if (song < 1 || song > 3 || cfg.SongPools.Count < song || cfg.SongPools[song - 1].Count == 0) return "bad";
            Grant g;
            lock (Store.Sync)
            {
                Season(uid);
                if (!Repaired(uid)) { Msg(c, "To play Ethereal Harp, it needs to be fully repaired first."); return "notrepaired"; }
                bool free = song == 1 && FreeToday(uid);
                uint cost = free ? 0u : (uint)cfg.SongCost[song - 1];
                if (cost > 0 && Kit.Count(c, Plectrum) < cost) { Msg(c, "You need " + cost + " Holy Plectrum(s) to play this song."); return "noplectrum"; }
                var pool = cfg.SongPools[song - 1];
                int k = Kit.Pick(pool); if (k < 0) return "bad";
                g = pool[k];
                if (!Kit.HasSpace(c, new[] { g })) { Msg(c, "Please clear up your inventory first."); return "nospace"; }
                if (cost > 0 && !Kit.Take(c, Plectrum, cost)) return "noplectrum";
                if (free) Store.SetDaily(uid, "free", 1);
                long pts = (long)cfg.PlayPointsPerPlectrum * Math.Max(1u, cost);
                Store.Add(uid, "pt", pts);
                Store.Commit();
                Kit.Give(c, new[] { g }, cfg.Festival, "play song " + song + (free ? " (free)" : " (-" + cost + " plectrums)") + " +" + pts + " pts");
            }
            RefreshRepair(c, 2);
            Lua(c, "DlgQiXiFestivalRepair_LotteryResults|{" + Kit.Q(g.ClientString()) + "}");
            return "ok";
        }

        // ------------------------------------------------------------------ Love Point rewards + rankings
        /// <summary>Claims every reached milestone (and the overflow units past the last one). Idempotent: the claimed count is saved before the grant.</summary>
        public static string ClaimMilestones(Client.GameClient c, Action<string> say)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            say = say ?? (t => Msg(c, t));
            lock (Store.Sync)
            {
                Season(uid);
                long pt = Store.GetL(uid, "pt");
                int mc = Store.GetI(uid, "mc");
                var give = new List<Grant>();
                int newMc = mc;
                while (newMc < cfg.Milestones.Count && pt >= cfg.Milestones[newMc].Points) { give.AddRange(cfg.Milestones[newMc].Items); newMc++; }
                long ov = 0;
                if (newMc >= cfg.Milestones.Count && cfg.Milestones.Count > 0 && cfg.OverflowItems.Count > 0)
                {
                    long units = Math.Max(0, (pt - cfg.Milestones[cfg.Milestones.Count - 1].Points) / cfg.OverflowEvery);
                    ov = Math.Min(20, Math.Max(0, units - Store.GetL(uid, "ov")));// at most 20 units a click (bag space)
                    for (int i = 0; i < ov; i++) give.AddRange(cfg.OverflowItems);
                }
                if (give.Count == 0)
                {
                    long next = newMc < cfg.Milestones.Count ? cfg.Milestones[newMc].Points
                              : cfg.Milestones.Count == 0 ? 0 : cfg.Milestones[cfg.Milestones.Count - 1].Points + (Store.GetL(uid, "ov") + 1) * cfg.OverflowEvery;
                    say("You don`t have a reward to claim. Reach " + next + " points to claim rewards of the next stage.");
                    return "none";
                }
                var merged = Merge(give).ToList();
                if (!Kit.HasSpace(c, merged)) { say("Please clear up your inventory first."); return "nospace"; }
                Store.Set(uid, "mc", newMc);
                if (ov > 0) Store.Add(uid, "ov", ov);
                Store.Commit();
                Kit.Give(c, merged, cfg.Festival, "Love Point rewards " + mc + "->" + newMc + (ov > 0 ? " +" + ov + " overflow" : "") + " at " + pt + " pts");
                say("You claimed your Love Point rewards (" + pt + " Love Points).");
                return "ok";
            }
        }

        public static List<KeyValuePair<string, long>> Top(int n)
        {
            var cfg = Cfg;
            string y = Kit.Now().Year.ToString(CultureInfo.InvariantCulture);
            var rows = Store.All("pt").Where(kv => kv.Value >= cfg.RankMin && Store.Get(kv.Key, "y") == y).OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(n).ToList();
            return rows.Select(kv => new KeyValuePair<string, long>(NameOf(kv.Key), kv.Value)).ToList();
        }
        static string NameOf(uint uid)
        {
            Client.GameClient o;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out o) && o.Player != null) return o.Player.Name;
            string n = Store.Get(uid, "name");
            return string.IsNullOrEmpty(n) ? uid.ToString() : n;
        }
        static void ShowRank(Client.GameClient c)
        {
            lock (Store.Sync) { Store.Set(c.Player.UID, "name", c.Player.Name); }
            var top = Top(10);
            Msg(c, "------ Love Point Rankings (local) ------");
            if (top.Count == 0) Msg(c, "Nobody is ranked yet. Collect at least " + Cfg.RankMin + " Love Points to get ranked on the local rankings.");
            for (int i = 0; i < top.Count; i++) Msg(c, (i + 1) + ". " + top[i].Key + "  " + top[i].Value);
            Msg(c, "Your Love Points: " + Points(c.Player.UID));
        }

        // ------------------------------------------------------------------ NPC dialogs (QixiHooks.OnNpc)
        public static bool OwnsNpc(uint npc) { return npc == WeaverGirl || npc == Orphee; }
        public static void OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npc, byte opt)
        {
            var cfg = Cfg;
            var d = new Game.MsgNpc.Dialog(c, stream);
            if (npc == WeaverGirl)
            {
                if (c.Player.Level < cfg.MinLevel && opt == 0)
                {
                    Hooks.Text(d, "Weaver Girl: Finally, the annual Qixi is here again! I`ll celebrate this romantic season with you! Requirement: Level " + cfg.MinLevel);
                    d.AddOption("Come back at level " + cfg.MinLevel + ".", 255).AddAvatar(10).FinalizeDialog();
                    return;
                }
                switch (opt)
                {
                    case 1: OpenMain(c); return;
                    case 2:
                        {
                            long pt = Points(c.Player.UID);
                            int mc; lock (Store.Sync) mc = Store.GetI(c.Player.UID, "mc");
                            var sb = new StringBuilder("------ Love Points Rewards ------\n");
                            for (int i = 0; i < cfg.Milestones.Count; i++)
                                sb.Append(i < mc ? "(claimed) " : "").Append(cfg.Milestones[i].Points).Append(": ").Append(string.Join(", ", cfg.Milestones[i].Items.Select(g => g.Count + " " + Kit.ItemName(g.Item)))).Append("\n");
                            sb.Append("After reaching ").Append(cfg.Milestones.Count > 0 ? cfg.Milestones.Last().Points : 0).Append(" points, every ").Append(cfg.OverflowEvery).Append(" points will get you 1 L1 Mythsoul and 1 Apollo`s Blessing.\n");
                            sb.Append("Love Points: ").Append(pt);
                            Hooks.Text(d, sb.ToString());
                            bool can; lock (Store.Sync) can = MilestoneClaimable(cfg, c.Player.UID);
                            d.AddOption(can ? "Claim all rewards. (Claimable)" : "Claim all rewards.", 10).AddOption("Collect Love Points.", 11).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                            return;
                        }
                    case 10: { string r = null; ClaimMilestones(c, t => r = t); Hooks.Text(d, r ?? ""); d.AddOption("OK.", 255).AddAvatar(10).FinalizeDialog(); return; }
                    case 11: OpenRepair(c, 1); return;
                    case 3:
                        {
                            lock (Store.Sync) Store.Set(c.Player.UID, "name", c.Player.Name);
                            var top = Top(10);
                            var sb = new StringBuilder("------ Love Point Rankings ------\n");
                            if (top.Count == 0) sb.Append("Nobody is ranked yet.\n");
                            for (int i = 0; i < top.Count; i++) sb.Append(i + 1).Append(". ").Append(top[i].Key).Append("  ").Append(top[i].Value).Append("\n");
                            sb.Append("Collect at least ").Append(cfg.RankMin).Append(" Love Points to get ranked on the local rankings.");
                            Hooks.Text(d, sb.ToString());
                            d.AddOption("I see.", 255).AddAvatar(10).FinalizeDialog();
                            return;
                        }
                    default:
                        Hooks.Text(d, "Weaver Girl: Finally, the annual Qixi is here again! I`ll celebrate this romantic season with you! Requirement: Level " + cfg.MinLevel +
                                      ". Duration: Aug. 2 - Aug. 22. Rewards: Love Sound, Love Vow (Eternal), Golden Plume (Guardian), etc.");
                        d.AddOption("Start the romantic journey.", 1).AddOption("Love Point Rewards.", 2).AddOption("View Point Rankings.", 3).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                        return;
                }
            }
            // Orphee
            switch (opt)
            {
                case 1:
                    {
                        long used = 0; lock (Store.Sync) used = Store.GetDaily(c.Player.UID, "fl");
                        Hooks.Text(d, "------ Material Exchange ------\n9 Jades/Tulips: 1 harp repair material\n20 Tins of Beer/Orchids: 1 harp repair material\n20 Lilies/Love Letters: 1 harp repair material\n20 Kisses/Red Roses: 1 harp repair material\n(bigger bouquets give more: 99 = 5, 999 = 50; 99 Jades/Tulips = 11)\n" +
                                      "Submit items to exchange for the harp repair materials today (" + used + "/" + cfg.FlowerDailyLimit + ").");
                        d.AddOption("Use all to exchange.", 10).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                        return;
                    }
                case 10:
                    {
                        string r = ExchangeFlowers(c);
                        Hooks.Text(d, r); d.AddOption("OK.", 255).AddAvatar(10).FinalizeDialog();
                        return;
                    }
                default:
                    Hooks.Text(d, "Orphee: I wanna prepare a surprise for my wife, so I need to collect many flowers and gifts. If you can collect them for me, I`ll give you some harp repair materials in return to speed up repairing. Love Points Owned: " + Points(c.Player.UID));
                    d.AddOption("Exchange flowers/gifts for materials.", 1).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                    return;
            }
        }

        /// <summary>Orphee "Use all to exchange.": flowers/gifts -> harp materials (official table, 150 items a day).</summary>
        public static string ExchangeFlowers(Client.GameClient c)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            lock (Store.Sync)
            {
                Season(uid);
                if (!Running) return "The Qixi event has come to an end.";
                long used = Store.GetDaily(uid, "fl");
                long left = cfg.FlowerDailyLimit - used;
                if (left <= 0) return "You can`t exchange for more. Please come back tomorrow.";
                var take = new List<KeyValuePair<uint, uint>>();
                long gives = 0, submitted = 0;
                foreach (var f in cfg.Flowers)
                {
                    uint have = Kit.Count(c, f.Item);
                    long sets = Math.Min(have / f.Per, (left - submitted) / f.Per);
                    if (sets <= 0) continue;
                    take.Add(new KeyValuePair<uint, uint>(f.Item, (uint)(sets * f.Per)));
                    submitted += sets * f.Per; gives += sets * f.Gives;
                    if (submitted >= left) break;
                }
                if (gives == 0) return "You don`t have gifts to exchange for the harp repair materials!";
                gives = Math.Min(gives, 2000);
                var grant = new List<Grant>();
                if (!Repaired(uid)) grant.Add(new Grant(Materials[Part(uid) - 1], (uint)gives));
                else
                {
                    var cnt = new uint[4];
                    for (int i = 0; i < gives; i++) cnt[(int)Kit.Below(4)]++;
                    for (int i = 0; i < 4; i++) if (cnt[i] > 0) grant.Add(new Grant(Materials[i], cnt[i]));
                }
                if (!Kit.HasSpace(c, grant)) return "Please clear up your inventory first.";
                foreach (var t in take) if (!Kit.Take(c, t.Key, t.Value)) return "You don`t have gifts to exchange for the harp repair materials!";
                Store.SetDaily(uid, "fl", used + submitted);
                Store.Commit();
                Kit.Give(c, grant, cfg.Festival, "Orphee flowers " + string.Join(",", take.Select(t => t.Key + "x" + t.Value)) + " -> " + gives + " materials");
                return "Orphee gave you " + string.Join(", ", grant.Select(g => g.Count + " " + Kit.ItemName(g.Item))) + ". Today: " + (used + submitted) + "/" + cfg.FlowerDailyLimit + ".";
            }
        }

        // ------------------------------------------------------------------ items (QixiHooks.UseItem / item dialogs)
        public static bool OwnsItem(uint id)
        {
            return id == Silk || id == Blessing || id == RepairPack || id == PerformancePack || id == MemoryPack
                || id == LoveSoundBox || id == GoldenPlumeBox || id == LoveVowBox || id == LoveVowEternalBox;
        }
        public static bool UseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            var cfg = Cfg;
            switch (item.ITEM_ID)
            {
                case Silk:
                    {
                        c.ActiveNpc = Silk;
                        var d = new Game.MsgNpc.Dialog(c, stream);
                        Hooks.Text(d, "The Weaver Girl`s fine silk can be used to make beautiful garments. Consume 30 pieces of silk to make a 3-star garment, Love Vow (B). Consume 50 pieces of silk to make a 5-star garment, Love Vow (Eternal).");
                        d.AddOption("3-star Love Vow (B).", 1).AddOption("5-star Love Vow (Eternal).", 2).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                        return true;
                    }
                case Blessing:
                    {
                        c.ActiveNpc = Blessing;
                        var d = new Game.MsgNpc.Dialog(c, stream);
                        Hooks.Text(d, "It symbolizes the blessing of the God of Sun. Consume 90 Apollo`s Blessings to summon a permanent 1% Blessed Mount Armor, Golden Plume (Guardian). Consume 60 Apollo`s Blessings to summon a pair of permanent Love Sound Wings.");
                        d.AddOption("Golden Plume (Guardian). (90 blessings)", 1).AddOption("Love Sound Wings. (60 blessings)", 2).AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
                        return true;
                    }
                case RepairPack:
                    {
                        int k = Kit.PickW(cfg.RepairPackWeights.Select(x => (long)x).ToList());
                        return OpenBox(c, item, k < 0 ? null : new[] { new Grant(Materials[k], 1) });
                    }
                case PerformancePack: { int k = Kit.Pick(cfg.PerformancePack); return OpenBox(c, item, k < 0 ? null : new[] { cfg.PerformancePack[k] }); }
                case MemoryPack: { int k = Kit.Pick(cfg.MemoryPackPool); return OpenBox(c, item, k < 0 ? null : new[] { cfg.MemoryPackPool[k] }); }
                case LoveVowBox: return OpenBox(c, item, new[] { new Grant(199475, 1, true, 0, 1) });           // 1% Blessed Love Vow (B)
                case LoveVowEternalBox: return OpenBox(c, item, new[] { new Grant(199485, 1, false, 0, 1) });   // 1% Blessed Love Vow (Eternal)
                case GoldenPlumeBox: return OpenBox(c, item, new[] { new Grant(205008, 1, false, 0, 1) });      // 1% Blessed Golden Plume (Guardian)
                case LoveSoundBox:
                    {
                        if (c.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.LoveSound)) { Msg(c, "You`ve got 1 Love Sound. You can`t get another one."); return true; }
                        lock (Store.Sync)
                        {
                            Game.MsgServer.MsgGameItem cur;
                            if (!c.Inventory.TryGetItem(item.UID, out cur) || cur.ITEM_ID != LoveSoundBox) return true;
                            if (cur.StackSize > 1) c.Inventory.RemoveStackItem(cur.UID, 1, stream);
                            else c.Inventory.Update(cur, Role.Instance.AddMode.REMOVE, stream);
                            c.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.LoveSound, stream);
                        }
                        Kit.Log(cfg.Festival, "grant", c, "opened Love Sound box -> wings Love Sound 6074");
                        Msg(c, "Congrats! You`ve summoned 1 Love Sound.");
                        return true;
                    }
            }
            return false;
        }
        /// <summary>Removes ONE box (re-read from the bag) then adds its contents. Space first.</summary>
        static bool OpenBox(Client.GameClient c, Game.MsgServer.MsgGameItem item, Grant[] gs)
        {
            if (gs == null || gs.Length == 0 || gs.Any(g => g == null)) { Msg(c, "This pack is empty."); return true; }
            lock (Store.Sync)
            {
                Game.MsgServer.MsgGameItem cur;
                if (!c.Inventory.TryGetItem(item.UID, out cur) || cur.ITEM_ID != item.ITEM_ID) return true;
                if (!Kit.HasSpace(c, gs)) { Msg(c, "Please clear up your inventory first."); return true; }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    if (cur.StackSize > 1) c.Inventory.RemoveStackItem(cur.UID, 1, rec.GetStream());
                    else c.Inventory.Update(cur, Role.Instance.AddMode.REMOVE, rec.GetStream());
                }
                Kit.Give(c, gs, Cfg.Festival, "opened " + item.ITEM_ID);
            }
            Msg(c, "You received " + string.Join(", ", gs.Select(g => g.Count + " " + Kit.ItemName(g.Item))) + ".");
            return true;
        }

        /// <summary>Reply of the Silk / Blessing item dialogs (ActiveNpc = the item id).</summary>
        public static void OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint itemId, byte opt)
        {
            if (opt != 1 && opt != 2) return;
            uint need, box; string what;
            if (itemId == Silk) { need = opt == 1 ? 30u : 50u; box = opt == 1 ? LoveVowBox : LoveVowEternalBox; what = opt == 1 ? "3-star garment, Love Vow (B)" : "5-star garment, Love Vow (Eternal)"; }
            else { need = opt == 1 ? 90u : 60u; box = opt == 1 ? GoldenPlumeBox : LoveSoundBox; what = opt == 1 ? "Golden Plume (Guardian)" : "Love Sound"; }
            lock (Store.Sync)
            {
                if (Kit.Count(c, itemId) < need) { Msg(c, itemId == Silk ? "Failed to make a garment due to insufficient silk." : "Failed to summon the reward due to insufficient blessings."); return; }
                var g = new[] { new Grant(box, 1, box == LoveVowBox) };
                if (!Kit.HasSpace(c, g)) { Msg(c, itemId == Silk ? "Please clear up your inventory first." : "Please clear up your inventory before you summon the reward."); return; }
                if (!Kit.Take(c, itemId, need)) return;
                Kit.Give(c, g, Cfg.Festival, "used " + need + " x " + itemId + " -> " + box);
            }
            Msg(c, itemId == Silk ? "Congrats! You`ve made 1 " + what + "!" : "Congrats! You`ve summoned 1 " + what + ".");
        }
    }
}
