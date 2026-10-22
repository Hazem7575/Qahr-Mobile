// ============================================================================
//  [NewYear2023 COMPLETE 2026-10-15] Laurel Branch wishes of the 2023 Spring Festival (Miss Bunny) and the event's boxes.
//
//  Wishes = the shared draw window DlgValentinesDayMainUI with DlgID "1329" (luaui dlgvalentinesday\valentinesdayinfo):
//    open   10170 SpringFestival2023Main_MainOpen            -> DlgValentinesDay_MainOpen|{Code=100,Data={DlgID='1329',...}}
//    wish   10250 SpringFestival2023Main_Draw</N>1|10         -> DlgValentinesDay_DrawBack|{Code=200,...}
//    stage  10250 SpringFestival2023Main_Phase</N>1..8        -> DlgValentinesDay_GetRewardBack|{Code=400,...}
//    after the wheel the client asks 10139 Lover2025Kua_DrawCongratulation -> DlgValentinesDay_RewardServer (only when
//    this player has a pending result of ours; otherwise the call falls through to CastleMystery / SummerWhales).
//    rank   10250 SpringFestival2023Main_LinkRank, store 10250 SpringFestival2023Main_Exchange (Ingot Store = GiltSwirl's
//    exchanges, Festivals\LunarNewYear2023\rewards.json), "+" 10139 SpringFestival2023Main_GetDrawItem</N>1..3.
//  OFFICIAL: cost 32 / 320 Laurel Branches 3340340 (DlgDrawCostItem), "Make wishes to get 2 ingots, 32 pts, and 1 reward"
//  (DrawInfo), the 20 wheel items (DlgDrawItemList 1329), the 8 stage packs (DlgRewardItemlist 1329), level 80, texts.
//  DERIVED (config wheelWeights / stageAt): the wheel weights (by the official slot colours) and the stage thresholds.
//
//  Boxes (itemtype texts, official contents): Grand Spring Bag 270-500 / Small Spring Bag 25-50 Laurel Branches,
//  Spring Growth Bag (S)/(L) and Classic Spring Pack (one random reward of the listed ones), Spring Selection Box /
//  Spring Garment Box / Spring Collection Box (choose one, Server_Key tSpringFestival2023Main_DelText options),
//  Spring Ingot Pack 5-80 Spring Ingots, Spring Envoy Packet 333-888 CPs (B) (08:00-21:00), Spring Greeting Card.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using G = Qahr.Mobile.NewYear2023.SpringGive;
using K = Qahr.Mobile.NewYear2023.SpringKit;
using SF = Qahr.Mobile.NewYear2023.SpringFestival2023;

namespace Qahr.Mobile.NewYear2023
{
    public static class SpringFestival2023Wish
    {
        public const string DlgId = "1329";
        public const uint MsgSummer23 = 10250, MsgValentines = 10139;
        public const uint GrowthS = 3340357, GrowthL = 3340358, ClassicPack = 3340359, SelectionBox = 3340360, IngotPack = 3340361,
            GarmentBox = 3340362, EnvoyPacket = 3340331, CollectionBox = 3340386;

        // ---- official tables (client valentinesdayinfo.dat, DlgID 1329)
        public static readonly string[] Wheel =
        {
            "3336146 0 5", "3340357 0 1", "3340358 0 1", "3340359 0 1", "199885 0 1 0 0 0 0 0 1",
            "3340360 0 1", "3340331 0 1", "199885 0 1 3 0 0 0 0 1", "3340361 0 1", "3340357 0 1",
            "3340358 0 1", "3336146 0 5", "3340359 0 1", "3340361 0 1", "199885 0 1 0 0 0 0 0 1",
            "3340331 0 1", "3340357 0 1", "199885 0 1 3 0 0 0 0 1", "3340331 0 1", "3340361 0 1",
        };
        public static readonly string[][] Stages =
        {
            new[] { "730004 0 1 3 7200 1", "3340356 0 100" },
            new[] { "4060001 0 100 3", "3340356 0 150" },
            new[] { "3009002 0 1 0 2880 1", "3340356 0 200" },
            new[] { "3009104 0 1", "3340356 0 300" },
            new[] { "3328247 0 500", "3340356 0 500" },
            new[] { "3340359 0 1", "3340356 0 1000" },
            new[] { "3340362 0 1", "3340356 0 1500" },
            new[] { "3340356 0 100" },
        };
        public const int CostOne = 32, CostTen = 320;// DlgDrawCostItem 1329
        public const int IngotsPerWish = 2;          // DrawInfo

        // ---- derived (config)
        // slot colours (DlgDrawItemBg 1329): item1 white, item2 green, item3 blue, item4 purple, item5 yellow
        static long[] _weights = { 15, 300, 140, 60, 2, 20, 140, 3, 60, 300, 140, 15, 60, 60, 2, 140, 300, 3, 140, 60 };
        static long[] _stageAt = { 10, 20, 40, 60, 100, 150, 200, 300 };
        public static long[] Weights { get { var _ = SF.Cfg; return _weights; } }
        public static long[] StageAt { get { var _ = SF.Cfg; return _stageAt; } }

        internal static void ReadConfig(Dictionary<string, object> d)
        {
            long[] w = { 15, 300, 140, 60, 2, 20, 140, 3, 60, 300, 140, 15, 60, 60, 2, 140, 300, 3, 140, 60 };
            long[] s = { 10, 20, 40, 60, 100, 150, 200, 300 };
            if (d != null)
            {
                var a = FL.Arr(FL.Val(d, "wheelWeights"));
                if (a != null && a.Count == Wheel.Length) w = a.Select(x => (long)Math.Min(1000000, FL.U(x))).ToArray();
                var b = FL.Arr(FL.Val(d, "stageAt"));
                if (b != null && b.Count == Stages.Length)
                {
                    var t = b.Select(x => (long)Math.Max(1, Math.Min(1000000, FL.U(x)))).ToArray();
                    bool asc = true; for (int i = 1; i < t.Length; i++) if (t[i] < t[i - 1]) asc = false;
                    if (asc) s = t;
                }
            }
            _weights = w; _stageAt = s;
        }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, Tuple<long, string[]>> _pending = new ConcurrentDictionary<uint, Tuple<long, string[]>>();
        static readonly ConcurrentDictionary<uint, long> _open = new ConcurrentDictionary<uint, long>();// the 1329 window is open
        public static void ResetForTests() { _last.Clear(); _pending.Clear(); _open.Clear(); }
        static long Tick { get { return Environment.TickCount & 0x7FFFFFFF; } }
        static void Msg(Client.GameClient c, string t) { K.Msg(c, t); }
        static void Lua(Client.GameClient c, string call) { SF.Lua(c, call); }
        static SpringStore Store { get { return SF.Store; } }

        // official texts (luaUiRes[1329])
        const string TxtOver = "The event is over, so you cannot make wishes now.";
        const string TxtNoBranch = "You don`t have enough Laurel Branches. Please go to open bags or click on `+` to view how to get branches.";
        const string TxtSpace = "Please make {0} space(s) in your inventory first.";
        const string TxtStageLow = "Failed to claim the reward since you don`t have enough wishing chances.";

        public static int Bit(int i) { return 1 << i; }
        public static long[] EffectiveWeights()
        {
            var cw = Weights;
            var w = new long[Wheel.Length];
            for (int i = 0; i < w.Length; i++)
            {
                var g = G.Parse(Wheel[i]);
                w[i] = g != null && G.Exists(g.Id) && i < cw.Length ? Math.Max(0, cw[i]) : 0;// a slot whose item this server lacks never wins
            }
            return w;
        }
        static string RewardList(uint uid, out int flag)
        {
            long draws = Store.Get(uid, "s_wishes"), got = Store.Get(uid, "s_stages");
            var at = StageAt;
            var sb = new StringBuilder("{");
            flag = 0;
            for (int i = 0; i < Stages.Length; i++)
            {
                bool claimed = (got & Bit(i)) != 0;
                if (!claimed && draws >= at[i]) flag = 1;
                if (i > 0) sb.Append(',');
                sb.Append('{').Append(at[i]).Append(',').Append(claimed ? 1 : 0).Append('}');
            }
            return sb.Append('}').ToString();
        }
        public static int StageFlag(uint uid) { int f; RewardList(uid, out f); return f; }

        public static string OpenData(Client.GameClient c)
        {
            SF.Touch(c.Player.UID);
            int flag; string rl = RewardList(c.Player.UID, out flag);
            return "DlgValentinesDay_MainOpen|{['Code']=100,['Data']={['DlgID']=" + K.Q(DlgId) + ",['ModelType']=0,['CostItem']=" + G.Count(c, SF.LaurelBranch)
                + ",['ChangeItem']=" + G.Count(c, SF.SpringIngot) + ",['DrawTimes']=" + Store.Get(c.Player.UID, "s_wishes") + ",['Rewardflag']=" + flag
                + ",['RewardList']=" + rl + ",['DrawList']={}}}";
        }

        /// <summary>Make n (1 or 10) wishes. Returns the error text or null.</summary>
        public static string Draw(Client.GameClient c, ServerSockets.Packet stream, int n)
        {
            if (n != 1 && n != 10) return "?";
            if (!SF.Running) return TxtOver;
            if (c.Player.Level < SF.Cfg.MinLevel) return SF.TxtLevel;
            uint u = c.Player.UID;
            SF.Touch(u);
            int cost = n == 1 ? CostOne : CostTen;
            if (G.Count(c, SF.LaurelBranch) < cost) return TxtNoBranch;
            var w = EffectiveWeights();
            var picks = new List<int>();
            for (int i = 0; i < n; i++) { int k = K.Pick(w); if (k < 0) return "Wishing isn`t available right now."; picks.Add(k); }
            var items = picks.Select(k => G.Parse(Wheel[k])).ToList();
            items.Add(G.I(SF.SpringIngot, IngotsPerWish * n));
            int slots = G.Slots(items);
            if (!G.HaveSpace(c, slots)) return string.Format(TxtSpace, slots);
            // 1. cost, 2. state, 3. grant
            if (!G.Take(c, SF.LaurelBranch, cost, stream)) return TxtNoBranch;
            long draws = 0; int pts = SF.Cfg.PointsPerWish * n;
            Store.Update(u, g =>
            {
                draws = g("s_wishes") + n;
                return new Dictionary<string, long> { { "s_wishes", draws }, { "s_points", g("s_points") + pts } };
            });
            G.Give(c, items, stream, "LunarNewYear2023/Wish");
            _pending[u] = Tuple.Create(Tick, picks.Select(k => Wheel[k]).ToArray());
            int flag; RewardList(u, out flag);
            Lua(c, "DlgValentinesDay_DrawBack|{['Code']=200,['Data']={['DlgID']=" + K.Q(DlgId) + ",['CostItem']=" + G.Count(c, SF.LaurelBranch) + ",['DrawTimes']=" + draws
                + ",['DrawList']={" + string.Join(",", picks.Select(k => (k + 1).ToString())) + "},['ChangeItem']=" + G.Count(c, SF.SpringIngot) + ",['Rewardflag']=" + flag + "}}");
            return null;
        }

        /// <summary>Claim stage reward idx (1-based). Returns the error text or null.</summary>
        public static string ClaimStage(Client.GameClient c, ServerSockets.Packet stream, int idx)
        {
            if (idx < 1 || idx > Stages.Length) return "?";
            if (!SF.Running) return "The event is over, so you cannot make wishes now.";
            if (c.Player.Level < SF.Cfg.MinLevel) return SF.TxtLevel;
            uint u = c.Player.UID;
            SF.Touch(u);
            int i = idx - 1;
            var items = Stages[i].Select(G.Parse).Where(x => x != null && G.Exists(x.Id)).ToList();
            int slots = G.Slots(items);
            if (!G.HaveSpace(c, slots)) return string.Format(TxtSpace, slots);
            string err = null;
            var at = StageAt;
            bool ok = Store.Update(u, g =>
            {
                long got = g("s_stages");
                if ((got & Bit(i)) != 0) { err = "You have claimed the reward."; return null; }
                if (g("s_wishes") < at[i]) { err = TxtStageLow; return null; }
                return new Dictionary<string, long> { { "s_stages", got | Bit(i) } };
            });
            if (!ok) return err ?? TxtStageLow;
            G.Give(c, items, stream, "LunarNewYear2023/WishStage" + idx);
            int flag; string rl = RewardList(u, out flag);
            Lua(c, "DlgValentinesDay_GetRewardBack|{['Code']=400,['Data']={['DlgID']=" + K.Q(DlgId) + ",['RewardList']=" + rl + ",['Rewardflag']=" + flag + "}}");
            return null;
        }

        /// <summary>The client's "show what I won" call after the wheel. False = not ours.</summary>
        public static bool OnCongratulation(Client.GameClient c)
        {
            Tuple<long, string[]> p;
            if (!_pending.TryRemove(c.Player.UID, out p)) return false;
            long now = Tick;
            if (now - p.Item1 < 0 || now - p.Item1 > 120000) return false;
            Lua(c, "DlgValentinesDay_RewardServer|{['Code']=200,['Data']={" + string.Join(",", p.Item2.Select(K.Q)) + "}}");
            return true;
        }

        /// <summary>Lua calls of the wish window (10170 / 10250 / 10139). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            uint u = c.Player.UID;
            if (msgId == MsgValentines && func == "Lover2025Kua_DrawCongratulation") return OnCongratulation(c);
            long opened;
            // (the wish window's own "+" is hard-wired by the client to Summer2023's guide call: left to that module; the
            //  Laurel Branch sources are on the main window's "+" = SpringFestival2023Main_GetDrawItem below)
            if (!func.StartsWith("SpringFestival2023Main_", StringComparison.Ordinal)) return false;
            if (K.Throttled(_last, u, 400)) return true;
            int n = 0;
            if (args != null && args.Length > 0) int.TryParse(args[0], out n);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                string err = null;
                switch (func)
                {
                    case "SpringFestival2023Main_MainOpen":
                    case "SpringFestival2023Main_Link":
                        if (!SF.Running) { err = "The event is over, so you cannot make wishes now."; break; }
                        if (c.Player.Level < SF.Cfg.MinLevel) { err = SF.TxtLevel; break; }
                        _open[u] = Tick;
                        Lua(c, OpenData(c));
                        break;
                    case "SpringFestival2023Main_Draw": err = Draw(c, s, n); break;
                    case "SpringFestival2023Main_Phase": err = ClaimStage(c, s, n); break;
                    case "SpringFestival2023Main_LinkRank":
                        err = "Make wishes to climb the Bunny Spring Rankings! Your New Year Points: " + SF.Points(u) + "."; break;
                    case "SpringFestival2023Main_Exchange":
                        err = "Talk to GiltSwirl in Twin City (387,382) to exchange Spring Ingots in the Ingot Store."; break;
                    case "SpringFestival2023Main_GetDrawItem":
                        _open.TryRemove(u, out opened);
                        if (n == 1) { SF.OnLua(c, "SpringInterface_MainView", new string[0]); return true; }
                        if (n == 3) { SF.OnLua(c, "SpringInterface_PubView", new[] { "1", "0" }); return true; }
                        err = "Cross-server quests aren`t available on this server."; break;
                    default:
                        return true;
                }
                if (err != null && err != "?") Msg(c, err);
            }
            return true;
        }

        // ------------------------------------------------------------------ boxes (item use / item dialogs)
        public static readonly uint[] Usable = { SF.GrandBag, SF.SmallBag, GrowthS, GrowthL, ClassicPack, SelectionBox, IngotPack, GarmentBox, EnvoyPacket, CollectionBox, SF.GreetingCard };
        public static bool IsOurs(uint id) { return Array.IndexOf(Usable, id) >= 0; }

        const uint CpsMarker = 0;// a reward line with Id 0 = CPs (B)
        // official item texts (itemtype): "a random reward from ..." / "choose one from ..."
        static readonly G.GItem[] GrowthSPool = { G.I(4060001, 5, true), G.I(3009001), G.I(3341259, 1, true), G.I(3009101), G.I(730002, 1, true) };
        static readonly G.GItem[] GrowthLPool = { G.I(4060001, 100, true), G.I(3009002), G.I(3341260, 3, true), G.I(3009103, 2), G.I(730005, 1, true) };
        static readonly G.GItem[] ClassicPool = { G.I(3333997, 1, true), G.I(723694, 1, true), G.I(723695, 1, true), G.I(3318571, 1, true), new G.GItem { Id = CpsMarker, Count = 3000, Bound = true }, G.I(3341261, 1, true) };
        static readonly string[] ClassicNames = { "Rare Yellow Rune Pack (B).", "Permanent Stone (B).", "Big Permanent Stone (B).", "+6 Rare Steed Pack (B).", "3000 CPs (B).", "100KK Contribution Medal (B)." };
        // Spring Garment Box: permanent 1% Blessed garment / Mount Armor; the "new spring garment" of 2023 = Fluffy Bunny 199895
        static readonly uint[] GarmentIds = { 195575, 196595, 199895, 200691, 200761 };
        static readonly string[] GarmentNames = { "Commander Robe (Wealth).", "Ruby Aroma (Youth).", "New Year Garment.", "Iridescent Ox (Luck).", "Blaze Kylin (Solar)." };
        static readonly uint[] CollectionIds = { 4300000, 4300001, 4300002, 4300003, 4300004, 4310000, 4310001, 4310002 };
        static readonly string[] CollectionNames = { "Pistil Aroma.", "Lotus Dew.", "Begonia Frost.", "Snowsweet Melody.", "Peach Bloom.", "Ethereal Luster.", "Fancy Carrier.", "Droplet Glow." };

        static string Describe(G.GItem g) { return g.Id == CpsMarker ? g.Count + " CPs (B)" : g.Count + " " + G.ItemName(g.Id) + (g.Bound ? " (B)" : ""); }

        /// <summary>Opens one unit of `item` and grants `give` (space checked first, the box taken before the grant).</summary>
        static bool Open(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream, G.GItem give, string why)
        {
            var items = give.Id == CpsMarker ? new List<G.GItem>() : new List<G.GItem> { give };
            int slots = G.Slots(items);
            if (!G.HaveSpace(c, slots)) { Msg(c, string.Format(SF.TxtSpace, Math.Max(1, slots))); return false; }
            if (!G.TakeOne(c, item, stream)) return false;
            if (give.Id == CpsMarker) G.BoundCps(c, give.Count, why);
            else G.Give(c, items, stream, why);
            Msg(c, "You received " + Describe(give) + ".");
            return true;
        }

        /// <summary>MsgItemUsuagePacket.UseItem. True = handled (always for our ids).</summary>
        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null || !IsOurs(item.ITEM_ID)) return false;
            if (K.Throttled(_last, c.Player.UID, 300)) return true;
            var cfg = SF.Cfg;
            switch (item.ITEM_ID)
            {
                case SF.GrandBag: Open(c, item, stream, G.I(SF.LaurelBranch, K.Rand(cfg.LaurelGrand[0], cfg.LaurelGrand[1])), "LunarNewYear2023/GrandSpringBag"); return true;
                case SF.SmallBag: Open(c, item, stream, G.I(SF.LaurelBranch, K.Rand(cfg.LaurelSmall[0], cfg.LaurelSmall[1])), "LunarNewYear2023/SmallSpringBag"); return true;
                case IngotPack: Open(c, item, stream, G.I(SF.SpringIngot, K.Rand(5, 80)), "LunarNewYear2023/SpringIngotPack"); return true;
                case GrowthS: Open(c, item, stream, GrowthSPool[K.Rand(0, GrowthSPool.Length - 1)], "LunarNewYear2023/GrowthBagS"); return true;
                case GrowthL: Open(c, item, stream, GrowthLPool[K.Rand(0, GrowthLPool.Length - 1)], "LunarNewYear2023/GrowthBagL"); return true;
                case ClassicPack: Open(c, item, stream, ClassicPool[K.Rand(0, ClassicPool.Length - 1)], "LunarNewYear2023/ClassicSpringPack"); return true;
                case EnvoyPacket:
                    {
                        int h = K.Now.Hour;
                        if (h < 8 || h >= 21) { Msg(c, "Please use Spring Envoy Packets from 8:00 to 21:00 every day."); return true; }
                        Open(c, item, stream, new G.GItem { Id = CpsMarker, Count = K.Rand(333, 888), Bound = true }, "LunarNewYear2023/SpringEnvoyPacket");
                        return true;
                    }
                case SF.GreetingCard: SF.UseGreetingCard(c, item, stream); return true;
                case SelectionBox:
                case GarmentBox:
                case CollectionBox:
                    c.ActiveNpc = item.ITEM_ID;
                    ShowChoice(c, stream, item.ITEM_ID, item.Bound != 0);
                    return true;
            }
            return false;
        }

        static void ShowChoice(Client.GameClient c, ServerSockets.Packet stream, uint id, bool bound)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("Please choose the reward you want.\n");
            string[] names = id == SelectionBox ? ClassicNames : id == GarmentBox ? GarmentNames : CollectionNames;
            for (int i = 0; i < names.Length; i++)
                d.AddOption(id == GarmentBox && bound ? names[i].TrimEnd('.') + " (B)." : names[i], (byte)(i + 1));
            d.AddOption("Let me think about it.", 255).AddAvatar(0).FinalizeDialog();
        }

        /// <summary>Procesor hook: the answer of one of our choose-one dialogs (npcid = item id). True = handled.</summary>
        public static bool OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null) return false;
            if (npcid != SelectionBox && npcid != GarmentBox && npcid != CollectionBox) return false;
            if (option == 255) return true;
            if (K.Throttled(_last, c.Player.UID, 300)) return true;
            var item = c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == npcid).OrderByDescending(i => i.Bound).FirstOrDefault();
            if (item == null) { Msg(c, "You don`t have this item in your inventory."); return true; }
            bool bound = item.Bound != 0;
            int k = option - 1;
            if (npcid == SelectionBox)
            {
                if (k < 0 || k >= ClassicPool.Length) return true;
                Open(c, item, stream, ClassicPool[k], "LunarNewYear2023/SpringSelectionBox");
            }
            else if (npcid == GarmentBox)
            {
                if (k < 0 || k >= GarmentIds.Length) return true;
                Open(c, item, stream, new G.GItem { Id = GarmentIds[k], Count = 1, Bound = bound, Bless = 1 }, "LunarNewYear2023/SpringGarmentBox");
            }
            else
            {
                if (k < 0 || k >= CollectionIds.Length) return true;
                Open(c, item, stream, new G.GItem { Id = CollectionIds[k], Count = 1, Bound = bound }, "LunarNewYear2023/SpringCollectionBox");
            }
            return true;
        }
    }
}
