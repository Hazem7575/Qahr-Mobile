// ============================================================================
//  [AnniversaryOtherworld2024 2026-10-15 COMPLETE] Anniversary Cake Gala 2024 - server rules on an IBag (harness-testable).
//  Window / NPC scripts: AnniversaryOtherworld2024Ui.cs.
//
//  OFFICIAL (client): Common Sugar Bean 3348129 = +1000 Gala Points and 1 Beast Pack, Super Sugar Bean 3348130 = +5000 and
//  5 Beast Packs (itemtype texts); 5 Sugar Bean Chips 3348128 = 1 Sugar Bean; Gala Point stage rewards at 10000 / 20000 /
//  30000 / 50000 / 70000 / 200000 / 450000 / 650000 / 1550000 and every 50000 more (luaUiRes_EN["1483_1"][10391].Total,
//  rewards OtherworldAdventure_Config[10391].Rank.Total[1]); "Visiting Vault Palace = 2 Super Gourds" (Gala Daily Quest,
//  tAnniversary2024Npc_DelText[36841][Text142]); Maisie's Treasure Exchange = EmoneySaleShop[36841] (materials, limits);
//  Theodore's Anniversary Store = EmoneySaleShop[36846] (CP prices, limits).
//  DERIVED: which Beast Pack a bean gives (uniform Forest/Coast/Ruins), that beans are used from the bag (the Lounge needs
//  captured beasts, whose maps are not in the client), limits are per character per festival run.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.AnnivKit;

namespace Qahr.Mobile.AnniversaryOtherworld2024
{
    public sealed class GalaGood
    {
        public Prize Item; public uint CostItem; public uint CostCount; public uint Cps; public int Limit;
        public bool IsCps { get { return Cps > 0; } }
    }

    public static class CakeGalaCore
    {
        public const string Festival = "AnniversaryOtherworld2024";
        public const uint SuperGourd = 3600703, CommonBean = 3348129, SuperBean = 3348130, BeanChip = 3348128, Lollipop = 3348123;
        public static readonly uint[] BeastPacks = { 3348158, 3348159, 3348160 };// Forest / Coast / Ruins
        public static readonly Store Store = new Store("AnniversaryOtherworld2024");
        public static Func<int, int> RandHook;
        static int Rand(int n) { var h = RandHook; if (h != null) return h(n); return n <= 1 ? 0 : (int)Lottery.LotteryCommon.Below((ulong)n); }

        public static readonly long[] Stages = { 10000, 20000, 30000, 50000, 70000, 200000, 450000, 650000, 1550000 };
        public const long RepeatStep = 50000;
        public static readonly List<Prize>[] StageRewards =
        {
            new List<Prize> { Prize.Parse("3322762 0 1 3 10080 1"), Prize.Parse("3348123 0 50") },
            new List<Prize> { Prize.Parse("730004 0 1 3 7200 1"), Prize.Parse("3348123 0 100") },
            new List<Prize> { Prize.Parse("4060001 0 100 3"), Prize.Parse("3348123 0 150") },
            new List<Prize> { Prize.Parse("3009002 0 1 0 2880 1"), Prize.Parse("3348123 0 200") },
            new List<Prize> { Prize.Parse("3009103 0 1"), Prize.Parse("3348123 0 300") },
            new List<Prize> { Prize.Parse("3335924 0 1"), Prize.Parse("3348123 0 500") },
            new List<Prize> { Prize.Parse("4060001 0 1000 3"), Prize.Parse("3348123 0 800") },
            new List<Prize> { Prize.Parse("4060001 0 1500 3"), Prize.Parse("3348123 0 1000") },
            new List<Prize> { Prize.Parse("3335926 0 1"), Prize.Parse("3348123 0 1500") },
            new List<Prize> { Prize.Parse("3348123 0 50"), Prize.Parse("3009103 0 1") },
        };

        /// <summary>Maisie's Treasure Exchange (official EmoneySaleShop[36841]).</summary>
        public static readonly GalaGood[] Exchange =
        {
            new GalaGood { Item = new Prize(3348122, 1, false), CostItem = 4200011, CostCount = 1, Limit = 0 },
            new GalaGood { Item = new Prize(3348120, 1, false), CostItem = 4200009, CostCount = 1, Limit = 0 },
            new GalaGood { Item = new Prize(3348121, 1, false), CostItem = 4200010, CostCount = 1, Limit = 0 },
            new GalaGood { Item = new Prize(3348129, 1, false), CostItem = SuperGourd, CostCount = 5, Limit = 0 },
            new GalaGood { Item = new Prize(3348094, 1, false), CostItem = Lollipop, CostCount = 19999, Limit = 3 },
            new GalaGood { Item = new Prize(3348096, 1, false), CostItem = Lollipop, CostCount = 9999, Limit = 3 },
            new GalaGood { Item = new Prize(3348091, 1, false), CostItem = Lollipop, CostCount = 14999, Limit = 3 },
            new GalaGood { Item = new Prize(3348093, 1, false), CostItem = Lollipop, CostCount = 3999, Limit = 3 },
            new GalaGood { Item = new Prize(3321046, 1, false), CostItem = Lollipop, CostCount = 180, Limit = 100 },
            new GalaGood { Item = new Prize(3009002, 1, false), CostItem = Lollipop, CostCount = 250, Limit = 100 },
            new GalaGood { Item = new Prize(3321044, 1, false), CostItem = Lollipop, CostCount = 20, Limit = 0 },
        };
        /// <summary>Theodore's Anniversary Store (official EmoneySaleShop[36846], CPs).</summary>
        public static readonly GalaGood[] CpStore =
        {
            new GalaGood { Item = new Prize(3348101, 1, false), Cps = 150, Limit = 0 },
            new GalaGood { Item = new Prize(3348102, 1, false), Cps = 30, Limit = 0 },
            new GalaGood { Item = new Prize(3348094, 1, false), Cps = 19999, Limit = 100 },
            new GalaGood { Item = new Prize(3348095, 1, false), Cps = 9999, Limit = 100 },
            new GalaGood { Item = new Prize(3348091, 1, false), Cps = 14999, Limit = 100 },
            new GalaGood { Item = new Prize(3348092, 1, false), Cps = 3999, Limit = 100 },
        };

        // ------------------------------------------------------------------ state
        public static void Touch(uint uid, string name)
        {
            int run = Kit.Run(Festival);
            if (run != 0 && Store.GetI(uid, "run") != run)
            {
                Store.Set(uid, "run", run); Store.Set(uid, "pts", 0); Store.Set(uid, "s10", 0);
                for (int i = 1; i <= 9; i++) Store.Set(uid, "s" + i, 0);
                for (int i = 0; i < 16; i++) { Store.Set(uid, "x" + i, 0); Store.Set(uid, "y" + i, 0); }
            }
            if (!string.IsNullOrEmpty(name)) Store.Set(uid, "name", name);
        }
        public static long Points(uint uid) { return Math.Max(0, Store.GetL(uid, "pts")); }

        // ------------------------------------------------------------------ Sugar Beans
        /// <summary>Uses one Sugar Bean from the bag: Gala Points + Beast Pack(s). Space first, bean taken, state, packs.</summary>
        public static BeanResult UseBean(uint uid, string name, IBag bag, uint bean)
        {
            int pts = bean == CommonBean ? 1000 : bean == SuperBean ? 5000 : 0;
            int packs = bean == CommonBean ? 1 : bean == SuperBean ? 5 : 0;
            if (pts == 0) return new BeanResult { Error = "" };
            lock (Store.Sync)
            {
                Touch(uid, name);
                if (bag.Count(bean) < 1) return new BeanResult { Error = "You do not have enough Sugar Beans in your inventory." };
                var list = new List<Prize>();
                for (int i = 0; i < packs; i++)
                {
                    uint pk = BeastPacks[Rand(BeastPacks.Length)];
                    var same = list.FirstOrDefault(p => p.Item == pk);
                    if (same != null) same.Count++; else list.Add(new Prize(pk, 1, false));
                }
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) return new BeanResult { Error = "Please make " + slots + " space(s) in your inventory first." };
                if (!bag.Take(bean, 1)) return new BeanResult { Error = "You do not have enough Sugar Beans in your inventory." };
                long np = Store.Add(uid, "pts", pts);
                Store.Commit();
                foreach (var p in list) bag.Give(p);
                var r = new BeanResult { Ok = true, Points = np, Added = pts };
                r.Given.AddRange(list);
                return r;
            }
        }
        public sealed class BeanResult { public bool Ok; public string Error; public long Points; public int Added; public readonly List<Prize> Given = new List<Prize>(); }

        /// <summary>5 Sugar Bean Chips -> 1 Common Sugar Bean (official item text).</summary>
        public static bool CombineChips(IBag bag, out string err)
        {
            err = null;
            if (bag.Count(BeanChip) < 5) { err = "You don`t have enough Sugar Bean Chips in your inventory."; return false; }
            var p = new Prize(CommonBean, 1, false);
            if (!bag.HasSpace(Kit.Slots(new[] { p }))) { err = "Please make 1 space(s) in your inventory first."; return false; }
            if (!bag.Take(BeanChip, 5)) { err = "You don`t have enough Sugar Bean Chips in your inventory."; return false; }
            bag.Give(p);
            return true;
        }

        // ------------------------------------------------------------------ Gala Point stage rewards
        /// <summary>0 locked, 1 claimable, 2 claimed (stage 10: claimable while claimed &lt; earned).</summary>
        public static int StageState(uint uid, int stage, out int claimed10, out int earned10)
        {
            claimed10 = earned10 = 0;
            long pts = Points(uid);
            if (stage >= 1 && stage <= 9) return Store.GetI(uid, "s" + stage) == 1 ? 2 : pts >= Stages[stage - 1] ? 1 : 0;
            if (stage == 10)
            {
                long over = pts - Stages[8];
                earned10 = over >= RepeatStep ? (int)Math.Min(10000, over / RepeatStep) : 0;
                claimed10 = Math.Max(0, Store.GetI(uid, "s10"));
                return claimed10 < earned10 ? 1 : claimed10 > 0 ? 2 : 0;
            }
            return 0;
        }
        public static List<Prize> ClaimStage(uint uid, string name, IBag bag, int stage, out string err)
        {
            err = null;
            if (stage < 1 || stage > 10) { err = ""; return null; }
            lock (Store.Sync)
            {
                Touch(uid, name);
                int c, e;
                if (StageState(uid, stage, out c, out e) != 1) { err = "You can`t claim this reward yet."; return null; }
                var list = StageRewards[stage - 1].Select(p => p.Clone()).ToList();
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) { err = "Your inventory is full. Please make " + slots + " space(s) first!"; return null; }
                if (stage == 10) Store.Set(uid, "s10", c + 1); else Store.Set(uid, "s" + stage, 1);
                Store.Commit();
                foreach (var p in list) bag.Give(p);
                return list;
            }
        }

        // ------------------------------------------------------------------ Treasure Exchange / Anniversary Store
        public static int Bought(uint uid, bool cps, int idx) { return Math.Max(0, Store.GetI(uid, (cps ? "y" : "x") + idx)); }
        public static Prize Buy(uint uid, string name, IBag bag, bool cps, int idx, out string err)
        {
            err = null;
            var goods = cps ? CpStore : Exchange;
            if (idx < 0 || idx >= goods.Length) { err = ""; return null; }
            var g = goods[idx];
            lock (Store.Sync)
            {
                Touch(uid, name);
                int bought = Bought(uid, cps, idx);
                if (g.Limit > 0 && bought >= g.Limit) { err = "You have reached the exchange limit of this item."; return null; }
                var p = g.Item.Clone();
                int slots = Kit.Slots(new[] { p });
                if (!bag.HasSpace(slots)) { err = "Your inventory is full. Please make " + slots + " space(s) first!"; return null; }
                if (cps)
                {
                    if (bag.Cps < g.Cps || !bag.TakeCps(g.Cps)) { err = "You don`t have " + g.Cps + " CPs."; return null; }
                }
                else
                {
                    if (bag.Count(g.CostItem) < g.CostCount || !bag.Take(g.CostItem, g.CostCount)) { err = "You don`t have " + g.CostCount + " " + Kit.ItemName(g.CostItem) + "."; return null; }
                }
                Store.Set(uid, (cps ? "y" : "x") + idx, bought + 1);
                Store.Commit();
                bag.Give(p);
                return p;
            }
        }

        // ------------------------------------------------------------------ Gala Daily Quest: Visiting Vault Palace (2 Super Gourds)
        public static Prize VisitVault(uint uid, string name, IBag bag, out string err)
        {
            err = null;
            lock (Store.Sync)
            {
                Touch(uid, name);
                if (Store.GetDaily(uid, "vp") != 0) return null;// already paid today: just travel
                var p = new Prize(SuperGourd, 2, false);
                if (!bag.HasSpace(Kit.Slots(new[] { p }))) { err = "Please make 1 space(s) in your inventory first to receive the daily visit reward."; return null; }
                Store.SetDaily(uid, "vp", 1);
                Store.Commit();
                bag.Give(p);
                return p;
            }
        }

        public static List<KeyValuePair<string, long>> Top(int n)
        {
            int run = Kit.Run(Festival);
            lock (Store.Sync)
                return Store.All("pts").Where(kv => kv.Value > 0 && Store.GetI(kv.Key, "run") == run).OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(n)
                    .Select(kv => new KeyValuePair<string, long>(Store.Get(kv.Key, "name", kv.Key.ToString()), kv.Value)).ToList();
        }
    }
}
