// ============================================================================
//  [MayAdventure2023 2026-10-15] Anniversary Adventure (ERP536530, 5.16-5.29) - the server rules, on an IBag so the
//  harness runs the same code with a fake bag. The window / callbacks are in MayAdventure2023Ui.cs.
//
//  Official (client): 4 boards x 20 cells, cell kinds from Dialog_Info["1380-other"].MapData1..4 (1 plain, 2 tall
//  marker, 3 treasure), moving 1 / 5 cells costs 1 / 5 Courage Tokens 3341961 (CostProp, item text), every adventure =
//  30 Courage Points (Server_Key tAnniversaryAdventure_DelText[MsgBox][Reward]), Courage Pearls 3341960 are the
//  adventure reward and buy supplies (Courage Medals 3341903, Prop_Options_4/6), Courage Medals buy shop goods
//  (DlgMayAdventureShop, +3/+5/+7/+8 Stones named in the shop lua), board reward preview = Dialog_Info["1380-2-other"],
//  rank reward preview = Dialog_Info["1379-rank-item"].Rank[10238], requirement 2nd rebirth, adventures only in
//  Twin City, not in a team / booth / blue name ([MsgBox][IntoMap/IntoTeam/IntoBooth/IntoBlue]).
//  Derived (Data\MayAdventure2023\mayadventure.json): every weight / count of the pools, the daily move limit,
//  shop prices and limits, Courage Points per supply draw, tokens of the daily sign-in / Active Points packs.
//
//  Event state (board position, Courage Points, shop purchases) belongs to one festival run: a new run starts fresh.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.AnnivKit;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.MayAdventure2023
{
    public sealed class ShopGood
    {
        public Prize Item; public uint Cost; public int Limit;// Limit 0 = unlimited
    }

    public sealed class MayCfg
    {
        public string Festival = "MayAdventure2023";
        public int MinReborn = 2;                       // official ActBtnConfig[1] NeedMetempsychosis
        public uint Token = 3341961, Pearl = 3341960, Medal = 3341903;
        public uint RequiredMap = 1002;                 // official [MsgBox][IntoMap] "outside Twin City"
        public int PointsPerAdventure = 30;             // official [MsgBox][Reward]
        // ---- derived
        public int DailyMoves = 60;
        public int PointsPerDraw = 10;
        public int SignInTokens = 2, Ap100Tokens = 3, Ap200Tokens = 5;
        public List<Prize>[] CellPools = new List<Prize>[4];// [1] plain, [2] marker, [3] treasure
        public List<Prize> DrawPool = new List<Prize>();
        public List<ShopGood> Shop = new List<ShopGood>();
        /// <summary>Official rank rewards (Dialog_Info["1379-rank-item"].Rank[10238], EN) and the official tiers No.1 /
        /// No.2~3 / No.4~6 / No.7~10 (rank window art). Paid once per run, the next time the player opens the window.</summary>
        public static readonly int[] TierLast = { 1, 3, 6, 10 };
        public List<Prize>[] RankRewards =
        {
            new List<Prize> { Prize.Parse("3335933 0 1"), Prize.Parse("3341712 0 1"), Prize.Parse("712019 0 150"), Prize.Parse("4060001 0 2000 3") },
            new List<Prize> { Prize.Parse("3335927 0 1"), Prize.Parse("3341714 0 1"), Prize.Parse("712019 0 100"), Prize.Parse("4060001 0 1500 3") },
            new List<Prize> { Prize.Parse("3341714 0 1"), Prize.Parse("712019 0 80"), Prize.Parse("4060001 0 1000 3") },
            new List<Prize> { Prize.Parse("3341717 0 1"), Prize.Parse("712019 0 60"), Prize.Parse("4060001 0 800 3") },
        };
        public static int Tier(int rank) { for (int i = 0; i < TierLast.Length; i++) if (rank >= 1 && rank <= TierLast[i]) return i; return -1; }

        public static MayCfg Default()
        {
            var c = new MayCfg();
            c.CellPools[1] = new List<Prize> { new Prize(3341960, 1, true, 50), new Prize(3328247, 3, true, 30), new Prize(3341903, 1, true, 20) };
            c.CellPools[2] = new List<Prize> { new Prize(3341960, 3, true, 40), new Prize(3341903, 3, true, 30), new Prize(3328247, 3, true, 15), new Prize(723694, 1, true, 15) };
            c.CellPools[3] = new List<Prize> { new Prize(3341960, 3, true, 30), new Prize(723694, 1, true, 25), new Prize(3341903, 3, true, 20), new Prize(730004, 1, true, 10), new Prize(3341965, 1, true, 10), new Prize(200677, 1, true, 5) };
            c.DrawPool = new List<Prize> { new Prize(3341903, 1, true, 45), new Prize(3341903, 2, true, 25), new Prize(3341903, 3, true, 10), new Prize(3341965, 1, true, 15), new Prize(3328247, 5, true, 5) };
            c.Shop = new List<ShopGood>
            {
                new ShopGood { Item = new Prize(730003, 1, true), Cost = 30, Limit = 10 },
                new ShopGood { Item = new Prize(730005, 1, true), Cost = 150, Limit = 5 },
                new ShopGood { Item = new Prize(730007, 1, true), Cost = 600, Limit = 2 },
                new ShopGood { Item = new Prize(730008, 1, true), Cost = 1200, Limit = 1 },
                new ShopGood { Item = new Prize(723694, 1, true), Cost = 50, Limit = 5 },
                new ShopGood { Item = new Prize(3341965, 1, true), Cost = 20, Limit = 20 },
                new ShopGood { Item = new Prize(3328247, 10, true), Cost = 5, Limit = 0 },
                new ShopGood { Item = new Prize(200677, 1, true), Cost = 400, Limit = 1 },
            };
            return c;
        }

        public static MayCfg Read(Dictionary<string, object> d)
        {
            var c = Default();
            if (d == null) return c;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            c.MinReborn = Math.Max(0, Math.Min(10, Kit.Int(d, "minReborn", c.MinReborn)));
            c.DailyMoves = Math.Max(1, Math.Min(10000, Kit.Int(d, "dailyMoves", c.DailyMoves)));
            c.PointsPerDraw = Math.Max(0, Math.Min(10000, Kit.Int(d, "pointsPerDraw", c.PointsPerDraw)));
            c.SignInTokens = Math.Max(0, Math.Min(100, Kit.Int(d, "signInTokens", c.SignInTokens)));
            c.Ap100Tokens = Math.Max(0, Math.Min(100, Kit.Int(d, "ap100Tokens", c.Ap100Tokens)));
            c.Ap200Tokens = Math.Max(0, Math.Min(100, Kit.Int(d, "ap200Tokens", c.Ap200Tokens)));
            var cells = FL.Obj(FL.Val(d, "cellPools"));
            if (cells != null)
                for (int k = 1; k <= 3; k++) { var p = Kit.ReadPool(FL.Val(cells, k.ToString())); if (p.Count > 0) c.CellPools[k] = p; }
            var dp = Kit.ReadPool(FL.Val(d, "drawPool")); if (dp.Count > 0) c.DrawPool = dp;
            var shop = FL.Arr(FL.Val(d, "shop"));
            if (shop != null)
            {
                var l = new List<ShopGood>();
                foreach (var o in shop)
                {
                    var g = FL.Obj(o); if (g == null) continue;
                    var it = Prize.Parse(FL.Str(g, "item")); uint cost = FL.U(FL.Val(g, "cost"));
                    if (it == null || cost == 0 || cost > 1000000) continue;
                    l.Add(new ShopGood { Item = it, Cost = cost, Limit = Math.Max(0, Math.Min(100000, Kit.Int(g, "limit", 0))) });
                }
                if (l.Count > 0) c.Shop = l;
            }
            return c;
        }
    }

    public sealed class Outcome
    {
        public bool Ok; public string Error;
        public readonly List<Prize> Given = new List<Prize>();
        public int FromMap, FromStep, ToMap, ToStep; public long Points;
        public static Outcome Fail(string e) { return new Outcome { Ok = false, Error = e }; }
    }

    public static class MayAdventureCore
    {
        public static readonly Store Store = new Store("MayAdventure2023");
        /// <summary>Final top 10 of every finished run: row key = run number, u1..u10 = uids.</summary>
        public static readonly Store Ranks = new Store("MayAdventure2023", "ranks.txt");

        /// <summary>Official cell kinds (Dialog_Info["1380-other"].MapData1..4, 3rd value; missing = 1).</summary>
        static readonly int[][] Cells =
        {
            new[] { 1,1,1,2,1,1,3,1,1,2,1,1,1,1,2,1,1,1,1,3 },
            new[] { 1,1,3,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,1,3 },
            new[] { 1,1,3,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,1,3 },
            new[] { 1,1,3,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,1,3 },
        };
        public static int CellKind(int map, int step) { return map >= 1 && map <= 4 && step >= 1 && step <= 20 ? Cells[map - 1][step - 1] : 1; }

        // ------------------------------------------------------------------ state (event scoped)
        /// <summary>Resets the event-scoped values when the festival run changed. Caller holds Store.Sync.</summary>
        public static void Touch(MayCfg cfg, uint uid, string name)
        {
            int run = Kit.Run(cfg.Festival);
            if (run != 0) SettleOld(run);
            if (run != 0 && Store.GetI(uid, "run") != run)
            {
                Store.Set(uid, "run", run);
                Store.Set(uid, "map", 1); Store.Set(uid, "step", 1); Store.Set(uid, "pts", 0);
                foreach (var i in Enumerable.Range(0, 64)) Store.Set(uid, "b" + i, 0);
            }
            if (!string.IsNullOrEmpty(name)) Store.Set(uid, "name", name);
        }
        public static int Map(uint uid) { int m = Store.GetI(uid, "map", 1); return m < 1 || m > 4 ? 1 : m; }
        public static int Step(uint uid) { int s = Store.GetI(uid, "step", 1); return s < 1 || s > 20 ? 1 : s; }
        public static long Points(uint uid) { return Math.Max(0, Store.GetL(uid, "pts")); }

        // ------------------------------------------------------------------ move (Begin </N>1 | </N>5)
        public static Outcome Move(MayCfg cfg, uint uid, string name, IBag bag, int n)
        {
            if (n != 1 && n != 5) return Outcome.Fail("");
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                long moves = Store.GetDaily(uid, "mv");
                if (moves + n > cfg.DailyMoves) return Outcome.Fail("The adventure times have reached the limit today.");
                if (bag.Count(cfg.Token) < n) return Outcome.Fail("Failed to adventure for lack of Courage Tokens.");
                int map = Map(uid), step = Step(uid);
                var o = new Outcome { FromMap = map, FromStep = step };
                for (int i = 0; i < n; i++)
                {
                    step++;
                    if (step > 20) { step = 1; map = map % 4 + 1; }
                    var pool = cfg.CellPools[CellKind(map, step)] ?? cfg.CellPools[1];
                    var p = Kit.PickOne(pool);
                    if (p != null) o.Given.Add(p);
                }
                int slots = Kit.Slots(o.Given);
                if (!bag.HasSpace(slots)) return Outcome.Fail("Please make " + slots + " space(s) in your inventory first.");
                if (!bag.Take(cfg.Token, (uint)n)) return Outcome.Fail("Failed to adventure for lack of Courage Tokens.");
                // paid: record the new state BEFORE the prizes go out
                Store.Set(uid, "map", map); Store.Set(uid, "step", step);
                o.Points = Store.Add(uid, "pts", (long)cfg.PointsPerAdventure * n);
                Store.SetDaily(uid, "mv", moves + n);
                Store.Set(uid, "last", string.Join(" ", o.Given.Select(g => g.Item + ":" + g.Count + ":" + (g.Bound ? 1 : 0))));
                Store.Commit();
                o.ToMap = map; o.ToStep = step;
                foreach (var g in o.Given) bag.Give(g);
                o.Ok = true;
                return o;
            }
        }

        /// <summary>The prizes of the last move (shown after the board animation; display only, nothing granted).</summary>
        public static List<Prize> Last(uint uid)
        {
            var l = new List<Prize>();
            foreach (var t in Store.Get(uid, "last").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = t.Split(':'); uint id, n;
                if (p.Length == 3 && uint.TryParse(p[0], out id) && uint.TryParse(p[1], out n)) l.Add(new Prize(id, n, p[2] == "1"));
            }
            return l;
        }
        public static void ClearLast(uint uid) { lock (Store.Sync) { if (Store.Get(uid, "last") != "") { Store.Set(uid, "last", ""); Store.Commit(); } } }

        // ------------------------------------------------------------------ supplies (Treasure Hunt, 1 Courage Pearl)
        public static Outcome Draw(MayCfg cfg, uint uid, string name, IBag bag)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                if (bag.Count(cfg.Pearl) < 1) return Outcome.Fail("Failed to adventure for lack of Courage Pearls.");
                var p = Kit.PickOne(cfg.DrawPool);
                if (p == null) return Outcome.Fail("Treasure Hunt is closed now.");
                int slots = Kit.Slots(new[] { p });
                if (!bag.HasSpace(slots)) return Outcome.Fail("Please make " + slots + " space(s) in your inventory first.");
                if (!bag.Take(cfg.Pearl, 1)) return Outcome.Fail("Failed to adventure for lack of Courage Pearls.");
                var o = new Outcome();
                o.Points = Store.Add(uid, "pts", cfg.PointsPerDraw);
                Store.Commit();
                bag.Give(p);
                o.Given.Add(p);
                o.Ok = true;
                return o;
            }
        }

        // ------------------------------------------------------------------ Anniversary Store (Courage Medals)
        public static int Bought(uint uid, int idx) { return Math.Max(0, Store.GetI(uid, "b" + idx)); }
        public static Outcome Buy(MayCfg cfg, uint uid, string name, IBag bag, int idx)
        {
            if (idx < 0 || idx >= cfg.Shop.Count || idx >= 64) return Outcome.Fail("");
            var g = cfg.Shop[idx];
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                int bought = Bought(uid, idx);
                if (g.Limit > 0 && bought >= g.Limit) return Outcome.Fail("You have reached the exchange limit of this item.");
                if (bag.Count(cfg.Medal) < g.Cost) return Outcome.Fail("Failed to adventure for lack of Courage Medals.");
                int slots = Kit.Slots(new[] { g.Item });
                if (!bag.HasSpace(slots)) return Outcome.Fail("Please make " + slots + " space(s) in your inventory first.");
                if (!bag.Take(cfg.Medal, g.Cost)) return Outcome.Fail("Failed to adventure for lack of Courage Medals.");
                Store.Set(uid, "b" + idx, bought + 1);
                Store.Commit();
                var p = g.Item.Clone();
                bag.Give(p);
                var o = new Outcome { Ok = true };
                o.Given.Add(p);
                return o;
            }
        }

        // ------------------------------------------------------------------ ways to get Courage Tokens (once per day each)
        /// <summary>way 1 = Daily Sign-in Pack, way 2 = 100 / 200 Active Points Packs (both when reached).</summary>
        public static Outcome Way(MayCfg cfg, uint uid, string name, IBag bag, int way, uint activityPoints)
        {
            lock (Store.Sync)
            {
                Touch(cfg, uid, name);
                uint tokens = 0; var keys = new List<string>();
                if (way == 1)
                {
                    if (Store.GetDaily(uid, "sign") != 0) return Outcome.Fail("You have already claimed today`s Daily Sign-in Pack.");
                    tokens = (uint)cfg.SignInTokens; keys.Add("sign");
                }
                else if (way == 2)
                {
                    if (activityPoints >= 100 && Store.GetDaily(uid, "ap100") == 0) { tokens += (uint)cfg.Ap100Tokens; keys.Add("ap100"); }
                    if (activityPoints >= 200 && Store.GetDaily(uid, "ap200") == 0) { tokens += (uint)cfg.Ap200Tokens; keys.Add("ap200"); }
                    if (keys.Count == 0)
                        return Outcome.Fail(activityPoints < 100 ? "Reach 100 Active Points today to claim the Active Points Pack." :
                                           activityPoints < 200 ? "You have claimed the 100 Active Points Pack today. Reach 200 Active Points for the next one." :
                                           "You have already claimed today`s Active Points Packs.");
                }
                else return Outcome.Fail("");
                var p = new Prize(cfg.Token, Math.Max(1u, tokens), true);
                if (tokens == 0) { foreach (var k in keys) Store.SetDaily(uid, k, 1); Store.Commit(); return new Outcome { Ok = true }; }
                int slots = Kit.Slots(new[] { p });
                if (!bag.HasSpace(slots)) return Outcome.Fail("Please make " + slots + " space(s) in your inventory first.");
                foreach (var k in keys) Store.SetDaily(uid, k, 1);
                Store.Commit();
                bag.Give(p);
                var o = new Outcome { Ok = true };
                o.Given.Add(p);
                return o;
            }
        }

        // ------------------------------------------------------------------ rank settlement of finished runs
        /// <summary>Freezes the final top 10 of every older run that has rows left (before any row of it is reset).
        /// Caller holds Store.Sync.</summary>
        static void SettleOld(int current)
        {
            var runs = Store.All("run").Select(kv => (int)kv.Value).Where(r => r > 0 && r < current).Distinct().ToList();
            bool changed = false;
            lock (Ranks.Sync)
            {
                foreach (int r in runs)
                {
                    if (Ranks.GetI((uint)r, "done") == 1) continue;
                    var top = Store.All("pts").Where(kv => kv.Value > 0 && Store.GetI(kv.Key, "run") == r)
                        .OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(10).ToList();
                    for (int i = 0; i < top.Count; i++) Ranks.Set((uint)r, "u" + (i + 1), top[i].Key);
                    Ranks.Set((uint)r, "done", 1);
                    changed = true;
                }
                if (changed) Ranks.Commit();
            }
        }
        /// <summary>The finished runs in which this player placed in the top 10 and has not been paid: (run, rank).</summary>
        public static List<KeyValuePair<int, int>> PendingRanks(MayCfg cfg, uint uid)
        {
            var l = new List<KeyValuePair<int, int>>();
            int cur = Kit.Run(cfg.Festival);
            lock (Store.Sync)
            lock (Ranks.Sync)
                foreach (var kv in Ranks.All("done"))
                {
                    int r = (int)kv.Key;
                    if (r <= 0 || r == cur || Store.GetI(uid, "rk" + r) == 1) continue;
                    for (int i = 1; i <= 10; i++) if (Ranks.GetL(kv.Key, "u" + i) == uid) { l.Add(new KeyValuePair<int, int>(r, i)); break; }
                }
            return l;
        }
        /// <summary>Pays the official rank rewards of finished runs (space first, flag + commit, then give).</summary>
        public static Outcome ClaimRanks(MayCfg cfg, uint uid, IBag bag)
        {
            var o = new Outcome { Ok = true };
            foreach (var pr in PendingRanks(cfg, uid))
            {
                int tier = MayCfg.Tier(pr.Value);
                if (tier < 0) continue;
                var list = cfg.RankRewards[tier].Where(p => p != null).Select(p => p.Clone()).ToList();
                lock (Store.Sync)
                {
                    if (Store.GetI(uid, "rk" + pr.Key) == 1) continue;
                    int slots = Kit.Slots(list);
                    if (!bag.HasSpace(slots)) { o.Error = "Please make " + slots + " space(s) in your inventory first to receive your Courage Rankings reward."; return o; }
                    Store.Set(uid, "rk" + pr.Key, 1);
                    Store.Commit();
                }
                foreach (var p in list) bag.Give(p);
                o.Given.AddRange(list);
                o.Points = pr.Value;// last paid rank (message)
            }
            return o;
        }

        // ------------------------------------------------------------------ Courage Rankings (this run)
        public static List<KeyValuePair<string, long>> Top(MayCfg cfg, int n)
        {
            int run = Kit.Run(cfg.Festival);
            lock (Store.Sync)
                return Store.All("pts").Where(kv => kv.Value > 0 && Store.GetI(kv.Key, "run") == run)
                    .OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(n)
                    .Select(kv => new KeyValuePair<string, long>(Store.Get(kv.Key, "name", kv.Key.ToString()), kv.Value)).ToList();
        }
        public static int RankOf(MayCfg cfg, uint uid)
        {
            int run = Kit.Run(cfg.Festival);
            lock (Store.Sync)
            {
                long mine = Store.GetI(uid, "run") == run ? Points(uid) : 0;
                if (mine <= 0) return 0;
                return 1 + Store.All("pts").Count(kv => kv.Key != uid && Store.GetI(kv.Key, "run") == run && (kv.Value > mine || (kv.Value == mine && kv.Key < uid)));
            }
        }
    }
}
