// ============================================================================
//  [BleakValleyVillage 2026-10-15 COMPLETE] Celebration Realm (Global Anniversary 2023, ERP 4.25-5.15) - the server rules
//  on an IBag (harness-testable). Window / NPC scripts: BleakValleyVillageUi.cs.
//
//  OFFICIAL (client: luaUiRes_EN[1259], luaUiRes_EN[1273][10218], MidAutumn_Config[10218], Dialog_Info["1359-other"],
//  Server_Key tAniversaryNpc_DelText[36552..36560], itemtype texts):
//    Bonfire     45 Spark Logs per enhance -> 30 Bonfire EXP / 30 Anniversary Points; level 1..5 at 30/210/960/2040/3540;
//                bonfire level unlocks zones (OpenTips 4:L1 5:L2 6:L3 8:L4 9:L5 11:L3); over 20000 points the
//                Faith Crystal chance doubles.
//    Points      cumulative rewards at 1000/3000/5000/10000/30000/45000/60000/100000 and every 3000 above 100000.
//    Grit Points +1 every 30 minutes, +200 at 0:00 every day; a reputation quest costs 100; claiming a challenge
//                reward costs 1; Anniversary Bread (1/2/100) adds Grit Points; packs cost 50/100 Grit to open.
//    Reputation  Foison Village (Bleak Valley): levels at 50/100/150/200/300 Reputation -> 50/100/150/200/300 Spark Logs;
//                the level gives a Log Sale Pack discount (Log Sale Pack = 40-50 Spark Logs).
//  DERIVED (Infinity[DB]\BleakValleyVillage\bleakvalley.json): start Grit 200, Faith Crystal chance, Reputation per
//  quest, Log Sale Pack price / discount / daily limit, Dragonette expedition time and rewards, Silver Key chance.
//  Event state belongs to one festival run (a new run starts fresh); the cumulative / reputation claims are per run.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.AnnivKit;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.BleakValleyVillage
{
    public sealed class BvCfg
    {
        public string Festival = "BleakValleyVillage";
        public int MinLevel = 80;                                   // official "Requirement: Level 80"
        public uint SparkLog = 3600479, FaithCrystal = 3600481, LogSalePack = 3341479;
        public int SparkPerTry = 45, PointsPerTry = 30;             // official Rules 1
        public int[] Grades = { 30, 210, 960, 2040, 3540 };         // official GradeTip
        public Dictionary<int, int> ZoneGrade = new Dictionary<int, int> { { 4, 1 }, { 5, 2 }, { 6, 3 }, { 8, 4 }, { 9, 5 }, { 11, 3 }, { 7, 1 } };// official OpenTips (7 derived)
        public int GritPer30Min = 1, GritDaily = 200, QuestGrit = 100, ClaimGrit = 1;// official
        public int[] CumThresholds = { 1000, 3000, 5000, 10000, 30000, 45000, 60000, 100000 };// official
        public int CumRepeatStep = 3000;                            // official tier 9
        public List<Prize>[] CumRewards =
        {
            new List<Prize> { Prize.Parse("730004 0 1 3 7200 1"), Prize.Parse("4060001 0 50 3") },
            new List<Prize> { Prize.Parse("3322762 0 2 3 10080 1"), Prize.Parse("4060001 0 100 3") },
            new List<Prize> { Prize.Parse("3009002 0 1 0 2880 1"), Prize.Parse("4060001 0 150 3") },
            new List<Prize> { Prize.Parse("3009104 0 1"), Prize.Parse("3600481 0 1") },
            new List<Prize> { Prize.Parse("3325989 0 1"), Prize.Parse("3600481 0 2") },
            new List<Prize> { Prize.Parse("4060001 0 1000 3"), Prize.Parse("3600481 0 3") },
            new List<Prize> { Prize.Parse("4060001 0 1500 3"), Prize.Parse("3600481 0 5") },
            new List<Prize> { Prize.Parse("3326002 0 1"), Prize.Parse("3600481 0 10") },
            new List<Prize> { Prize.Parse("3009100 0 50"), Prize.Parse("3600481 0 1") },
        };// official MidAutumn_Config[10218].Rank.Total
        public int[] RepLevels = { 50, 100, 150, 200, 300 };        // official [36555] Text224-228
        public uint[] RepLogs = { 50, 100, 150, 200, 300 };         // official
        // ---- derived
        public int StartGrit = 200;
        public int FaithPerMille = 50;                              // 5% per enhance (x2 over 20000 points, official)
        public int FaithDoubleAt = 20000;                           // official Tip
        public int RepPerQuest = 10;
        public uint LogSaleCps = 99; public int LogSaleDiscountPct = 5; public int LogSaleDaily = 10;
        public int DragonMinutes = 120; public uint DragonLogsMin = 30, DragonLogsMax = 60; public int DragonFaithPct = 20;
        public int SilverKeyPct = 30;
        public uint SilverKey = 3341362;

        public static BvCfg Default() { return new BvCfg(); }
        public static BvCfg Read(Dictionary<string, object> d)
        {
            var c = new BvCfg();
            if (d == null) return c;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            c.MinLevel = Clamp(Kit.Int(d, "minLevel", c.MinLevel), 1, 255);
            c.StartGrit = Clamp(Kit.Int(d, "startGrit", c.StartGrit), 0, 100000);
            c.FaithPerMille = Clamp(Kit.Int(d, "faithPerMille", c.FaithPerMille), 0, 1000);
            c.RepPerQuest = Clamp(Kit.Int(d, "reputationPerQuest", c.RepPerQuest), 0, 1000);
            c.LogSaleCps = (uint)Clamp(Kit.Int(d, "logSaleCps", (int)c.LogSaleCps), 1, 100000);
            c.LogSaleDiscountPct = Clamp(Kit.Int(d, "logSaleDiscountPctPerLevel", c.LogSaleDiscountPct), 0, 15);
            c.LogSaleDaily = Clamp(Kit.Int(d, "logSaleDaily", c.LogSaleDaily), 0, 1000);
            c.DragonMinutes = Clamp(Kit.Int(d, "dragonMinutes", c.DragonMinutes), 1, 1440);
            c.DragonLogsMin = (uint)Clamp(Kit.Int(d, "dragonLogsMin", (int)c.DragonLogsMin), 1, 1000);
            c.DragonLogsMax = (uint)Clamp(Kit.Int(d, "dragonLogsMax", (int)c.DragonLogsMax), (int)c.DragonLogsMin, 1000);
            c.DragonFaithPct = Clamp(Kit.Int(d, "dragonFaithPct", c.DragonFaithPct), 0, 100);
            c.SilverKeyPct = Clamp(Kit.Int(d, "silverKeyPct", c.SilverKeyPct), 0, 100);
            return c;
        }
        static int Clamp(int v, int lo, int hi) { return Math.Max(lo, Math.Min(hi, v)); }

        public int Grade(long pts) { int g = 0; for (int i = 0; i < Grades.Length; i++) if (pts >= Grades[i]) g = i + 1; return g; }
        public bool ZoneOpen(int zone, int grade) { int need; return zone <= 2 || (ZoneGrade.TryGetValue(zone, out need) && grade >= need); }
        public int RepLevel(long rep) { int l = 0; for (int i = 0; i < RepLevels.Length; i++) if (rep >= RepLevels[i]) l = i + 1; return l; }
    }

    public sealed class BvResult
    {
        public bool Ok; public string Error; public bool LevelUp; public int Show;
        public readonly List<Prize> Given = new List<Prize>();
        public long Value;
        public static BvResult Fail(string e) { return new BvResult { Error = e }; }
    }

    public static class BleakValleyCore
    {
        public static readonly Store Store = new Store("BleakValleyVillage");
        public static Func<int, int> RandHook;                     // harness: deterministic random
        static int Rand(int n) { var h = RandHook; if (h != null) return h(n); return n <= 1 ? 0 : (int)Lottery.LotteryCommon.Below((ulong)n); }
        public static Func<DateTime> Now = () => DateTime.Now;

        // ------------------------------------------------------------------ event state + Grit Points
        /// <summary>Resets the event state on a new run and adds the Grit Points earned since the last visit
        /// (official: +1 / 30 min, +200 at 0:00). Caller holds Store.Sync.</summary>
        public static void Touch(BvCfg cfg, uint uid, string name)
        {
            int run = Kit.Run(cfg.Festival);
            var now = Now();
            long min = (long)(now - new DateTime(2020, 1, 1)).TotalMinutes;
            long day = (long)(now.Date - new DateTime(2020, 1, 1)).TotalDays;
            if (run != 0 && Store.GetI(uid, "run") != run)
            {
                foreach (var k in new[] { "pts", "rep", "dt", "dg", "c9" }) Store.Set(uid, k, 0);
                for (int i = 1; i <= 9; i++) Store.Set(uid, "c" + i, 0);
                for (int i = 1; i <= 5; i++) Store.Set(uid, "rl" + i, 0);
                Store.Set(uid, "run", run);
                Store.Set(uid, "gp", cfg.StartGrit);
                Store.Set(uid, "gt", min);
                Store.Set(uid, "gd", day);
            }
            long gt = Store.GetL(uid, "gt", min), gd = Store.GetL(uid, "gd", day);
            if (min > gt)
            {
                long halfHours = (min - gt) / 30;
                if (halfHours > 0) { Store.Add(uid, "gp", halfHours * cfg.GritPer30Min); Store.Set(uid, "gt", gt + halfHours * 30); }
            }
            if (day > gd) { Store.Add(uid, "gp", (day - gd) * cfg.GritDaily); Store.Set(uid, "gd", day); }
            if (!string.IsNullOrEmpty(name)) Store.Set(uid, "name", name);
        }
        public static long Grit(uint uid) { return Math.Max(0, Store.GetL(uid, "gp")); }
        public static long Points(uint uid) { return Math.Max(0, Store.GetL(uid, "pts")); }
        public static long Rep(uint uid) { return Math.Max(0, Store.GetL(uid, "rep")); }
        /// <summary>Takes Grit Points (false = not enough, nothing taken). Caller holds Store.Sync.</summary>
        public static bool TakeGrit(uint uid, int n) { if (n <= 0) return true; long g = Grit(uid); if (g < n) return false; Store.Set(uid, "gp", g - n); return true; }

        // ------------------------------------------------------------------ bonfire (ChkDraw)
        public static BvResult Enhance(BvCfg cfg, uint uid, string name, IBag bag)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                if (bag.Count(cfg.SparkLog) < cfg.SparkPerTry) return BvResult.Fail("Add more Spark Logs.");
                long pts = Points(uid);
                int perMille = cfg.FaithPerMille * (pts >= cfg.FaithDoubleAt ? 2 : 1);
                Prize faith = Rand(1000) < perMille ? new Prize(cfg.FaithCrystal, 1, false) : null;
                if (faith != null && !bag.HasSpace(1)) return BvResult.Fail("Please make 1 space(s) in your inventory first.");
                if (!bag.Take(cfg.SparkLog, (uint)cfg.SparkPerTry)) return BvResult.Fail("Add more Spark Logs.");
                int before = cfg.Grade(pts);
                long np = Store.Add(uid, "pts", cfg.PointsPerTry);
                Store.Commit();
                var r = new BvResult { Ok = true, Value = np, LevelUp = cfg.Grade(np) > before };
                if (faith != null) { bag.Give(faith); r.Given.Add(faith); }
                return r;
            }
        }

        // ------------------------------------------------------------------ cumulative Anniversary Points rewards
        /// <summary>Tier state: 0 locked, 1 claimable, 2 claimed (tier 9: claimable while claimed &lt; earned).</summary>
        public static int CumState(BvCfg cfg, uint uid, int tier, out int claimed9, out int earned9)
        {
            long pts = Points(uid);
            claimed9 = earned9 = 0;
            if (tier >= 1 && tier <= 8)
            {
                if (Store.GetI(uid, "c" + tier) == 1) return 2;
                return pts >= cfg.CumThresholds[tier - 1] ? 1 : 0;
            }
            if (tier == 9)
            {
                long over = pts - cfg.CumThresholds[7];
                earned9 = over >= cfg.CumRepeatStep ? (int)Math.Min(1000, over / cfg.CumRepeatStep) : 0;
                claimed9 = Math.Max(0, Store.GetI(uid, "c9"));
                return claimed9 < earned9 ? 1 : (claimed9 > 0 ? 2 : 0);
            }
            return 0;
        }
        public static BvResult ClaimCum(BvCfg cfg, uint uid, string name, IBag bag, int tier)
        {
            if (tier < 1 || tier > 9) return BvResult.Fail("");
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                int c9, e9;
                if (CumState(cfg, uid, tier, out c9, out e9) != 1) return BvResult.Fail("You can`t claim this reward yet.");
                var list = cfg.CumRewards[tier - 1].Where(p => p != null).Select(p => p.Clone()).ToList();
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) return BvResult.Fail("Please make " + slots + " space(s) in your inventory first.");
                if (tier == 9) Store.Set(uid, "c9", c9 + 1); else Store.Set(uid, "c" + tier, 1);
                Store.Commit();
                var r = new BvResult { Ok = true };
                foreach (var p in list) { bag.Give(p); r.Given.Add(p); }
                return r;
            }
        }

        // ------------------------------------------------------------------ reputation (Foison Village)
        public static readonly uint[][] VillagerQuests =
        {
            new uint[] { 7653, 7654, 7655 },   // Village Head (official chk: Talk to Village Head first)
            new uint[] { 7656, 7657, 7658 },   // Yana
            new uint[] { 7659, 7660, 7661 },   // Butcher Liu
            new uint[] { 7662, 7663, 7664 },   // Sister Lian
            new uint[] { 7665, 7666, 7667 },   // David Liu
        };
        public static readonly string[] Villagers = { "Village Head", "Yana", "Butcher Liu", "Sister Lian", "David Liu" };
        public static bool IsVillageQuest(uint q) { return q >= 7653 && q <= 7667; }
        static string Today { get { return Now().ToString("yyyyMMdd"); } }

        /// <summary>Records a quest accepted through the Reputation Officer today (credit needs it).</summary>
        public static void MarkAccepted(uint uid, uint quest) { lock (Store.Sync) { Store.Set(uid, "acc" + quest, Today); Store.Commit(); } }
        public static bool AcceptedToday(uint uid, uint quest) { return Store.Get(uid, "acc" + quest) == Today; }
        /// <summary>Reputation for a finished village quest: once per accept (official quest reward = reputation points).</summary>
        public static bool CreditQuest(BvCfg cfg, uint uid, uint quest)
        {
            if (!IsVillageQuest(quest)) return false;
            lock (Store.Sync)
            {
                string acc = Store.Get(uid, "acc" + quest);
                if (acc == "" || Store.Get(uid, "rc" + quest) == acc) return false;// not accepted at the officer, or already credited
                Store.Set(uid, "rc" + quest, acc);
                Store.Add(uid, "rep", cfg.RepPerQuest);
                Store.Commit();
                return true;
            }
        }
        public static BvResult ClaimRep(BvCfg cfg, uint uid, string name, IBag bag)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                int lvl = cfg.RepLevel(Rep(uid));
                var due = new List<int>();
                for (int i = 1; i <= lvl; i++) if (Store.GetI(uid, "rl" + i) != 1) due.Add(i);
                if (due.Count == 0) return BvResult.Fail("No claimable reputation rewards. Please upgrade the reputation first.");
                uint logs = 0; foreach (int i in due) logs += cfg.RepLogs[i - 1];
                var p = new Prize(cfg.SparkLog, logs, false);
                int slots = Kit.Slots(new[] { p });
                if (!bag.HasSpace(slots)) return BvResult.Fail("Please make " + slots + " space(s) in your inventory first.");
                foreach (int i in due) Store.Set(uid, "rl" + i, 1);
                Store.Commit();
                bag.Give(p);
                var r = new BvResult { Ok = true }; r.Given.Add(p);
                return r;
            }
        }
        /// <summary>Log Sale Pack price with the reputation discount (derived).</summary>
        public static uint LogSalePrice(BvCfg cfg, uint uid) { int l = cfg.RepLevel(Rep(uid)); return (uint)Math.Max(1, cfg.LogSaleCps * (100 - l * cfg.LogSaleDiscountPct) / 100); }
        public static BvResult BuyLogSale(BvCfg cfg, uint uid, string name, IBag bag)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                long today = Store.GetDaily(uid, "ls");
                if (today >= cfg.LogSaleDaily) return BvResult.Fail("You have bought all Log Sale Packs for today.");
                uint price = LogSalePrice(cfg, uid);
                if (!bag.HasSpace(1)) return BvResult.Fail("Please make 1 space(s) in your inventory first.");
                if (bag.Cps < price) return BvResult.Fail("You don`t have " + price + " CPs.");
                if (!bag.TakeCps(price)) return BvResult.Fail("You don`t have " + price + " CPs.");
                Store.SetDaily(uid, "ls", today + 1);
                Store.Commit();
                var p = new Prize(cfg.LogSalePack, 1, false);
                bag.Give(p);
                var r = new BvResult { Ok = true, Value = price }; r.Given.Add(p);
                return r;
            }
        }

        // ------------------------------------------------------------------ Dragon Remains: Dragonette expedition
        /// <summary>Seconds until the Dragonette returns (&gt;0 out, 0 = back and waiting, -1 = at home).</summary>
        public static long DragonLeft(uint uid)
        {
            long dt = Store.GetL(uid, "dt");
            if (dt <= 0) return -1;
            long now = (long)(Now() - new DateTime(2020, 1, 1)).TotalSeconds;
            return Math.Max(0, dt - now);
        }
        public static BvResult DragonGo(BvCfg cfg, uint uid, string name)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                if (!cfg.ZoneOpen(7, cfg.Grade(Points(uid)))) return BvResult.Fail("The fog is still so heavy that you cannot see the whole picture of this place.");
                if (DragonLeft(uid) >= 0) return BvResult.Fail(DragonLeft(uid) > 0 ? "Dragonette hasn`t returned. Try later!" : "Dragonette has just returned. Go to find it!");
                long now = (long)(Now() - new DateTime(2020, 1, 1)).TotalSeconds;
                Store.Set(uid, "dt", now + cfg.DragonMinutes * 60L);
                Store.Commit();
                return new BvResult { Ok = true, Value = cfg.DragonMinutes * 60L };
            }
        }
        public static BvResult DragonClaim(BvCfg cfg, uint uid, string name, IBag bag)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                if (DragonLeft(uid) != 0) return BvResult.Fail(DragonLeft(uid) > 0 ? "Dragonette hasn`t returned. Try later!" : "");
                if (Grit(uid) < cfg.ClaimGrit) return BvResult.Fail("You need " + cfg.ClaimGrit + " Grit Point(s) to claim the reward.");
                var list = new List<Prize> { new Prize(cfg.SparkLog, cfg.DragonLogsMin + (uint)Rand((int)(cfg.DragonLogsMax - cfg.DragonLogsMin + 1)), false) };
                if (Rand(100) < cfg.DragonFaithPct) list.Add(new Prize(cfg.FaithCrystal, 1, false));
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) return BvResult.Fail("Please make " + slots + " space(s) in your inventory first.");
                TakeGrit(uid, cfg.ClaimGrit);
                Store.Set(uid, "dt", 0);
                Store.Commit();
                var r = new BvResult { Ok = true, Show = 1 + Rand(20) };
                foreach (var p in list) { bag.Give(p); r.Given.Add(p); }
                return r;
            }
        }

        // ------------------------------------------------------------------ Grit boxes / breads (official item texts)
        /// <summary>Opens a pack that costs Grit Points, or eats bread that adds Grit Points. Box taken after every check.</summary>
        public static BvResult OpenGritItem(BvCfg cfg, uint uid, string name, IBag bag, uint item)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                if (bag.Count(item) < 1) return BvResult.Fail("You don`t have this item.");
                int addGrit = item == 3600480 ? 1 : item == 3341478 ? 2 : item == 3341700 ? 100 : 0;
                if (addGrit > 0)
                {
                    if (!bag.Take(item, 1)) return BvResult.Fail("You don`t have this item.");
                    long g = Store.Add(uid, "gp", addGrit);
                    Store.Commit();
                    return new BvResult { Ok = true, Value = g };
                }
                int cost; var list = new List<Prize>();
                switch (item)
                {
                    case 3600492: cost = 50; list.Add(new Prize(cfg.SparkLog, 100, false)); break;                     // Jade Serpent Box
                    case 3600494: cost = 50; list.Add(new Prize(cfg.SparkLog, 120 + (uint)Rand(61), false)); break;    // Anaconda Pack
                    case 3341509: cost = 100; list.Add(new Prize(cfg.SparkLog, 120 + (uint)Rand(61), false));          // Lucky Debris Pack
                        if (Rand(100) < cfg.SilverKeyPct) list.Add(new Prize(cfg.SilverKey, 1, false)); break;
                    default: return BvResult.Fail("");
                }
                if (Grit(uid) < cost) return BvResult.Fail("You need " + cost + " Grit Points to open it.");
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) return BvResult.Fail("Please make " + slots + " space(s) in your inventory first.");
                if (!bag.Take(item, 1)) return BvResult.Fail("You don`t have this item.");
                TakeGrit(uid, cost);
                Store.Commit();
                var r = new BvResult { Ok = true, Value = Grit(uid) };
                foreach (var p in list) { bag.Give(p); r.Given.Add(p); }
                return r;
            }
        }

        // ------------------------------------------------------------------ Anniversary Revelry rankings (this run)
        public static List<KeyValuePair<string, long>> Top(BvCfg cfg, int n)
        {
            int run = Kit.Run(cfg.Festival);
            lock (Store.Sync)
                return Store.All("pts").Where(kv => kv.Value > 0 && Store.GetI(kv.Key, "run") == run)
                    .OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(n)
                    .Select(kv => new KeyValuePair<string, long>(Store.Get(kv.Key, "name", kv.Key.ToString()), kv.Value)).ToList();
        }
    }
}
