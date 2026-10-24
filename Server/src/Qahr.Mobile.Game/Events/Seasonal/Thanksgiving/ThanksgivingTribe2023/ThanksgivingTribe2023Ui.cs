// ============================================================================
//  [ThanksgivingTribe2023 2026-10-14 / COMPLETE 2026-10-15] Global Thanksgiving 2023 "Tribe Fight" (ERP561753, 11.21-12.04),
//  client tMsg_Info["Thanksgiving23"] = 10319. Shown while the festival ThanksgivingTribe2023 runs.
//
//  Official client (luaui\dialog\thanksgiving23, luagui\1445.dat + 1303/1304, luaUiRes_EN[1445], itemtype tips):
//    main    DlgThanksgivingTribeMainUi_OpenMainUi {RemainTime,TokenItem (Hunting Crystal 3346119),ActivityPoints (Hunting
//            Points),PropItem={Tribe Gold Coin 3346120, Tribe Silver Coin 3346121, Treasure Compass 3346122},Starstone
//            (Stellar Runestone 3600664),Moonstone (Lunar Runestone 3346118),TribePointA/B,MonsterLv={4},PlayerIndex}
//    board   68 cells (Dialog_Info["1445-MapData"]): start, 14 treasure cells (type 3), 4 hunting points (type 4: cells
//            8/25/44/59, bosses with 5 levels, BossImg), the rest normal cells. UpdateMapDatas {State=0 first,MapDatas={Index,
//            Event 2 encounter|3 advanced treasure|4 hunting|5 luxury treasure,ItemInfo,EventIndex 1-3 (eventTips1-3)}}.
//    throw   Thanksgiving2023Richman_RollDice (Stellar Runestone, random steps), _RollDiceGuDing</N>1-6 (Lunar Runestone,
//            "move forward as you wish"), _TenRollDice (10 Stellar). Reply DlgThanksgivingTribeMainUi_ThrowReturnData
//            {CurIndex,Point,Starstone,Moonstone,EventHappen,MonsterLv}; after the walk the client calls _Event.
//            Rules (luaUiRes_EN[1445].rules): Hunting Points for every step + the same number of Hunting Crystals.
//    tribes  _HandIn</N>1|2 submits Hunting Crystals to the Dusk (1) / Dawn (2) Tribe (TribePointA / B).
//    hunt    Tribe Treasure Hunt = DlgGThanksGivingDraw (Treasure Compass), pool = Dialog_Info["1304-4-other"] (official
//            preview: Hunting Crystals, Tribe Silver Coins, Thorn Crowns, stones, ...).
//    stages  Hunting Points stage rewards (ThanksgivingMainNpc_OpenJieDuanReward -> DlgGlobalThanksgivingRank, thresholds
//            luaUiRes_EN[1303].Rank.Total, items Dialog_Info["1303-rank-item"].Total).
//    Timo    Tribe Merchant Timo exchange (Tribe Silver Coins; Gold Coins count 10 Silver each) in the official exchange
//            window ThanksgivingExchangeShop_* (its buttons call msg 10133 -> Thanksgiving2022Ui delegates here).
//    crowns  Thorn Crown (3346153, tip): 30 -> Patrolling Shadow (B)/Blazing Sun (B) box, 50 -> the 5-star (Ruling)/(Dawn) box;
//            the boxes (3346148/49/51/52, tips) open into the garments 191895/191915/191935/191925.
//    boxes   Platinum / Gold / Blackgold Treasure Box (3346145-47) and Compass Treasure Box (3346144): official tips.
//  DERIVED (Data\ThanksgivingTribe2023\thanksgivingtribe2023.json): points per step, cell / treasure / encounter
//  rewards and odds, boss level rewards, runestone CP prices, draw cost + odds, Timo goods (from the official preview) and
//  prices, encounter battles (map 12660, monsters in Festivals\ThanksgivingTribe2023\monsters).
//  NOT on this server: Arena (CS), global / local / tribe ranking rewards (mailed by the official server at the end).
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10319), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Thanksgiving2022;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.ThanksgivingTribe2023
{
    public static class ThanksgivingTribe2023Ui
    {
        public const uint LuaMsg = 10319;
        public const string Festival = "ThanksgivingTribe2023";
        public const uint HuntingCrystal = 3346119, TribeGoldCoin = 3346120, TribeSilverCoin = 3346121, TreasureCompass = 3346122,
                          StellarRunestone = 3600664, LunarRunestone = 3346118, ThornCrown = 3346153;
        public const uint CompassBox = 3346144, PlatinumBox = 3346145, GoldBox = 3346146, BlackgoldBox = 3346147;
        public const uint BlazingSunBox = 3346148, PatrollingShadowBox = 3346149, BlazingSunDawnBox = 3346151, PatrollingShadowRulingBox = 3346152;
        public const uint BlazingSun = 191915, PatrollingShadow = 191895, BlazingSunDawn = 191925, PatrollingShadowRuling = 191935;
        public const uint AlertTurkey = 995030, GoldEater = 995031, AltarSpirit = 995032, AltarGuardian = 995033;
        public const string Ended = "The Thanksgiving Tribe Fight has ended. See you next Thanksgiving!";
        public const int Cells = 68;
        public static readonly int[] TreasureCells = { 4, 9, 12, 17, 22, 27, 32, 37, 42, 47, 52, 57, 62, 67 };// type 3 in 1445-MapData
        public static readonly int[] HuntCells = { 8, 25, 44, 59 };// type 4

        // ------------------------------------------------------------------ config
        public sealed class Config
        {
            public int MinLevel = 80;// ActBtnConfig[14] NeedLevel
            public int PointsPerStep = 10;// derived (rules: "a lot of Hunting Points each time you move forward" + equal crystals)
            public int StellarCps = 10, LunarCps = 30;// derived ("pay a few CPs to buy")
            public int DrawCost1 = 1, DrawCost10 = 10;// Treasure Compasses, derived
            public int EncountersPerLap = 3;// derived
            public uint BattleMap = 12660; public ushort BattleX = 221, BattleY = 245;
            public int[] StageThresholds = { 500, 1500, 2500, 5000, 10000, 30000, 50000, 77777 };
            public int StageRepeatStep = 2000, StageRepeatMax = 500;
            public List<List<HItem>> StageItems = new List<List<HItem>>();
            public List<HKit.Prize> Normal = new List<HKit.Prize>(), Advanced = new List<HKit.Prize>(), Luxury = new List<HKit.Prize>(), Draw = new List<HKit.Prize>();
            public int LuxuryChance = 15;// % of treasure cells that are luxury, derived
            public List<HItem> EncounterReward = new List<HItem>();
            public List<Thanksgiving2022Ui.ShopGood> Shop = new List<Thanksgiving2022Ui.ShopGood>();
            public int GoldToSilver = 10;
        }
        static Config _cfg; static string _stamp;
        public static string CfgFile { get { return Path.Combine(HKit.DbDir(Festival), "thanksgivingtribe2023.json"); } }
        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(CfgFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = Defaults();
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[ThanksgivingTribe2023] config: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = Defaults();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static HKit.Prize P(string s, int w) { return new HKit.Prize { Item = HItem.Parse(s), Weight = w }; }
        public static Config Defaults()
        {
            var c = new Config();
            string[][] st =
            {
                new[] { "730004 0 1 3", "3346153 0 2" }, new[] { "3322762 0 2 3", "3346153 0 4" }, new[] { "3009002 0 1", "3346153 0 5" },
                new[] { "3009101 0 1", "3346153 0 5" }, new[] { "3325989 0 1", "3346153 0 5" }, new[] { "3342255 0 6", "3346153 0 6" },
                new[] { "3342255 0 8", "3346153 0 7" }, new[] { "3342255 0 10", "3346153 0 8" }, new[] { "3009103 0 1", "3346153 0 1" },
            };
            foreach (var s in st) c.StageItems.Add(s.Select(HItem.Parse).ToList());
            // normal cells (Event 1): small tribe goods - DERIVED from the items of the official preview
            c.Normal.Add(P("3346121 0 1", 40)); c.Normal.Add(P("3346121 0 2", 25)); c.Normal.Add(P("3346119 0 20", 20));
            c.Normal.Add(P("3346122 0 1", 10)); c.Normal.Add(P("3346118 0 1", 5));
            // advanced / luxury treasure cells (Event 3 / 5): the official Treasure Hunt preview (1304-4-other) - weights DERIVED
            c.Advanced.Add(P("3321077 0 5 3", 25)); c.Advanced.Add(P("3009102 0 1", 20)); c.Advanced.Add(P("3342773 0 3", 25));
            c.Advanced.Add(P("3346121 0 50", 15)); c.Advanced.Add(P("3346119 0 60", 15));
            c.Luxury.Add(P("3342771 0 2", 30)); c.Luxury.Add(P("3009103 0 1", 20)); c.Luxury.Add(P("3314248 0 1 3", 20));
            c.Luxury.Add(P("3346153 0 1", 15)); c.Luxury.Add(P("3345048 0 1", 10)); c.Luxury.Add(P("3342775 0 1", 5));
            // Tribe Treasure Hunt pool = official preview Dialog_Info["1304-4-other"].rewardList, weights DERIVED
            c.Draw.Add(P("3346119 0 60", 120)); c.Draw.Add(P("3346153 0 1", 60)); c.Draw.Add(P("3321077 0 5 3", 110));
            c.Draw.Add(P("3346121 0 50", 110)); c.Draw.Add(P("3009102 0 1", 90)); c.Draw.Add(P("3346121 0 60", 90));
            c.Draw.Add(P("3342773 0 3", 110)); c.Draw.Add(P("3342771 0 2", 70)); c.Draw.Add(P("3346121 0 66", 80));
            c.Draw.Add(P("3314207 0 1 3", 70)); c.Draw.Add(P("3009103 0 1", 50)); c.Draw.Add(P("3314248 0 1 3", 60));
            c.Draw.Add(P("3345048 0 1", 25)); c.Draw.Add(P("3342775 0 1", 15)); c.Draw.Add(P("3345054 0 1", 15));
            c.Draw.Add(P("3342776 0 1 3", 3));
            c.EncounterReward = new List<HItem> { HItem.Parse("3346122 0 2"), HItem.Parse("3346120 0 5") };// derived
            // Timo (Tribe Silver Coins): goods from the official preview, prices / limits DERIVED; 1 Thorn Crown per goods target
            c.Shop.Add(new Thanksgiving2022Ui.ShopGood { Item = HItem.Parse("3342773 0 1"), Cost = 15, Limit = 200, Target = 10 });
            c.Shop.Add(new Thanksgiving2022Ui.ShopGood { Item = HItem.Parse("3321077 0 1 3"), Cost = 10, Limit = 200, Target = 10 });
            c.Shop.Add(new Thanksgiving2022Ui.ShopGood { Item = HItem.Parse("3342771 0 1"), Cost = 60, Limit = 50, Target = 5 });
            c.Shop.Add(new Thanksgiving2022Ui.ShopGood { Item = HItem.Parse("3009103 0 1"), Cost = 120, Limit = 20, Target = 3 });
            c.Shop.Add(new Thanksgiving2022Ui.ShopGood { Item = HItem.Parse("3342775 0 1"), Cost = 600, Limit = 5, Target = 1 });
            return c;
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.MinLevel = HKit.Int(d, "minLevel", c.MinLevel);
            c.PointsPerStep = Math.Max(1, HKit.Int(d, "pointsPerStep", c.PointsPerStep));
            c.StellarCps = Math.Max(1, HKit.Int(d, "stellarCps", c.StellarCps));
            c.LunarCps = Math.Max(1, HKit.Int(d, "lunarCps", c.LunarCps));
            c.DrawCost1 = Math.Max(1, HKit.Int(d, "drawCost1", c.DrawCost1));
            c.DrawCost10 = Math.Max(1, HKit.Int(d, "drawCost10", c.DrawCost10));
            c.EncountersPerLap = Math.Max(0, Math.Min(10, HKit.Int(d, "encountersPerLap", c.EncountersPerLap)));
            c.LuxuryChance = Math.Max(0, Math.Min(100, HKit.Int(d, "luxuryChance", c.LuxuryChance)));
            c.GoldToSilver = Math.Max(1, HKit.Int(d, "goldToSilver", c.GoldToSilver));
            var bm = FL.Obj(FL.Val(d, "battleMap"));
            if (bm != null) { c.BattleMap = (uint)HKit.Int(bm, "map", (int)c.BattleMap); c.BattleX = (ushort)HKit.Int(bm, "x", c.BattleX); c.BattleY = (ushort)HKit.Int(bm, "y", c.BattleY); }
            var p = Thanksgiving2022Ui.Pool(FL.Val(d, "normalCell")); if (p != null) c.Normal = p;
            p = Thanksgiving2022Ui.Pool(FL.Val(d, "advancedTreasure")); if (p != null) c.Advanced = p;
            p = Thanksgiving2022Ui.Pool(FL.Val(d, "luxuryTreasure")); if (p != null) c.Luxury = p;
            p = Thanksgiving2022Ui.Pool(FL.Val(d, "treasureHunt")); if (p != null) c.Draw = p;
            var er = HKit.Items(FL.Val(d, "encounterReward")); if (er.Count > 0) c.EncounterReward = er;
            var shop = FL.Arr(FL.Val(d, "timo"));
            if (shop != null && shop.Count > 0)
            {
                var l = new List<Thanksgiving2022Ui.ShopGood>();
                foreach (var o in shop.Select(FL.Obj).Where(o => o != null))
                {
                    var it = HKit.Items(new List<object> { FL.Val(o, "item") });
                    if (it.Count == 1) l.Add(new Thanksgiving2022Ui.ShopGood { Item = it[0], Cost = Math.Max(1, HKit.Int(o, "cost", 1)), Limit = Math.Max(1, HKit.Int(o, "limit", 1)), Target = Math.Max(1, HKit.Int(o, "crownAfter", 5)) });
                }
                if (l.Count > 0) c.Shop = l.Take(5).ToList();
            }
        }

        // ------------------------------------------------------------------ state
        public static readonly PlayerStore Store = new PlayerStore(Festival);
        public static Festivals.FestivalManager.RunState RunOverride;
        static Festivals.FestivalManager.RunState Run { get { if (RunOverride != null) return RunOverride; try { return Festivals.FestivalManager.Running(Festival); } catch { return null; } } }
        public static bool Running { get { return Run != null; } }
        static string Season { get { var r = Run; return "s" + (r != null ? r.Started.Year : HKit.Now.Year) + ":"; } }
        static string Key(string k) { return Season + k; }
        static long Get(uint uid, string k) { return Store.Get(uid, Key(k)); }
        const uint Global = 0;// tribe totals live under uid 0

        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { HKit.Msg(c, t); }
        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse(a[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : -1; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10319).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (Thanksgiving2022Ui.OnShared(c, func, args)) return true;// the 2022 depot / stage windows also call msg 10319
            bool known = func.StartsWith("Thanksgiving2023Richman_", StringComparison.Ordinal) || func.StartsWith("ThanksgivingMainNpc_", StringComparison.Ordinal)
                      || func.StartsWith("ThanksgivingKua2023_", StringComparison.Ordinal);
            if (!known) return false;
            if (HKit.Throttled("tb23", c.Player.UID, 300)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Your level is less than " + cfg.MinLevel + ". Please level up first!"); return true; }
            Thanksgiving2022Ui.SharedOwner[c.Player.UID] = Festival;
            switch (func)
            {
                case "Thanksgiving2023Richman_OpenView": Open(c, cfg); return true;
                case "Thanksgiving2023Richman_RollDice": Throw(c, cfg, 0, 1); return true;
                case "Thanksgiving2023Richman_TenRollDice": Throw(c, cfg, 0, 10); return true;
                case "Thanksgiving2023Richman_RollDiceGuDing":
                    {
                        int p = Arg(args, 0);
                        if (p < 1 || p > 6) { Msg(c, "Failed to move forward for lack of Lunar Runestones. You can obtain some by buying packs or exchanging Animae for them in the Exchange Store."); return true; }
                        Throw(c, cfg, p, 1);
                        return true;
                    }
                case "Thanksgiving2023Richman_Event": Event(c, cfg); return true;
                case "Thanksgiving2023Richman_HandIn": HandIn(c, cfg, Arg(args, 0)); return true;
                case "Thanksgiving2023Richman_Buy": Buy(c, cfg, Arg(args, 0)); return true;
                case "ThanksgivingMainNpc_OpenChoujian": OpenDraw(c, cfg); return true;
                case "ThanksgivingMainNpc_ChouJiang": Draw(c, cfg, Arg(args, 0)); return true;
                case "Thanksgiving2023Richman_OpenExchangeShop": OpenShop(c, cfg); return true;
                case "ThanksgivingMainNpc_OpenJieDuanReward": Lua(c, "DlgGlobalThanksgivingRank_BindData|" + StageData(c.Player.UID, cfg)); return true;
                case "ThanksgivingMainNpc_StageGet": StageGet(c, cfg, Arg(args, 0)); return true;
                case "Thanksgiving2023Richman_OpenRank":
                case "ThanksgivingMainNpc_OpenRank":
                    Msg(c, "Dusk Tribe: " + Store.Get(Global, Key("tribe1")) + " points. Dawn Tribe: " + Store.Get(Global, Key("tribe2")) + " points. You submitted "
                        + Get(c.Player.UID, "give1") + " / " + Get(c.Player.UID, "give2") + " Hunting Crystals. (Ranking rewards are not given on this server.)");
                    return true;
                case "ThanksgivingKua2023_EnterDouKua": Msg(c, "The Arena (CS) isn`t available on this server."); return true;
                case "ThanksgivingKua2023_OpenEmoneyShop": Msg(c, "Please buy Treasure Compasses in the CP Mall."); return true;
            }
            return true;
        }

        /// <summary>The msg-10133 funcs of the shared exchange window while the player's Thanksgiving window is this one.</summary>
        public static bool OnShared2022(Client.GameClient c, string func, string[] args)
        {
            string owner;
            if (c == null || c.Player == null || !Running || !Thanksgiving2022Ui.SharedOwner.TryGetValue(c.Player.UID, out owner) || owner != Festival) return false;
            switch (func)
            {
                case "ThanksGivingPage2022_ExchangeItemNum":
                case "ThanksGivingPage2022_ExchangeItem":
                case "ThanksGivingPage2022_RewardPic":
                case "ThanksGivingPage2022_OpenPage":
                    break;
                default: return false;
            }
            if (HKit.Throttled("tb23", c.Player.UID, 300)) return true;
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) return true;
            switch (func)
            {
                case "ThanksGivingPage2022_ExchangeItemNum": ShopMax(c, cfg, Arg(args, 0)); break;
                case "ThanksGivingPage2022_ExchangeItem": ShopBuy(c, cfg, Arg(args, 0), Arg(args, 1)); break;
                case "ThanksGivingPage2022_RewardPic": ShopCrown(c, cfg, Arg(args, 0)); break;
                case "ThanksGivingPage2022_OpenPage": Open(c, cfg); break;
            }
            return true;
        }

        // ------------------------------------------------------------------ main window + board
        static long RemainSeconds()
        {
            try
            {
                var d = Festivals.FestivalManager.Find(Festival);
                if (d == null || d.Schedule == null || d.Schedule.Type != Festivals.ScheduleType.Yearly) return 0;
                var s = d.Schedule;
                var now = HKit.Now;
                var end = new DateTime(now.Year, s.YEndMonth, s.YEndDay).AddMinutes(s.YEndMin);
                if (end <= now) end = end.AddYears(1);
                long sec = (long)(end - now).TotalSeconds;
                return sec > 0 && sec < 60L * 86400 ? sec : 0;
            }
            catch { return 0; }
        }
        static string Item(Client.GameClient c, uint id) { return "'" + id + " 0 1," + HKit.Count(c, id) + "'"; }
        static int Pos(uint uid) { int p = (int)Get(uid, "pos"); return p < 1 || p > Cells ? 1 : p; }
        static string Levels(uint uid) { return "{" + string.Join(",", Enumerable.Range(1, 4).Select(i => Math.Max(1, Math.Min(5, Get(uid, "boss" + i))))) + "}"; }
        public static string MainData(Client.GameClient c, bool full)
        {
            uint uid = c.Player.UID;
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            if (full)
            {
                sb.Append("['Guide']=0,");
                long remain = RemainSeconds();
                if (remain > 0) sb.Append("['RemainTime']=").Append(remain).Append(',');
            }
            sb.Append("['Reds']={0,0,0},['TokenItem']=").Append(Item(c, HuntingCrystal));
            sb.Append(",['ActivityPoints']=").Append(Get(uid, "pts"));
            sb.Append(",['PropItem']={").Append(Item(c, TribeGoldCoin)).Append(',').Append(Item(c, TribeSilverCoin)).Append(',').Append(Item(c, TreasureCompass)).Append('}');
            sb.Append(",['Starstone']=").Append(HKit.Count(c, StellarRunestone)).Append(",['Moonstone']=").Append(HKit.Count(c, LunarRunestone));
            sb.Append(",['TribePointA']=").Append(Store.Get(Global, Key("tribe1"))).Append(",['TribePointB']=").Append(Store.Get(Global, Key("tribe2")));
            if (full) sb.Append(",['MonsterLv']=").Append(Levels(uid)).Append(",['PlayerIndex']=").Append(Pos(uid));
            return sb.Append("}}").ToString();
        }

        /// <summary>The board of one lap: treasure items per treasure cell + encounter cells (seeded by player + lap, so it
        /// is stable without storing it).</summary>
        public sealed class Board { public Dictionary<int, HItem> Treasure = new Dictionary<int, HItem>(); public HashSet<int> Luxury = new HashSet<int>(); public Dictionary<int, int> Encounter = new Dictionary<int, int>(); }
        public static Board BoardOf(uint uid, Config cfg)
        {
            long lap = Get(uid, "lap");
            var rng = new Random(unchecked((int)(uid * 7919u + (uint)lap * 104729u + 17u)));
            var b = new Board();
            foreach (int cell in TreasureCells)
            {
                bool lux = rng.Next(100) < cfg.LuxuryChance;
                var pool = lux ? cfg.Luxury : cfg.Advanced;
                int total = pool.Sum(p => Math.Max(0, p.Weight)), r = total > 0 ? rng.Next(total) : 0;
                HKit.Prize pick = null;
                foreach (var p in pool) { int w = Math.Max(0, p.Weight); if (r < w) { pick = p; break; } r -= w; }
                if (pick == null && pool.Count > 0) pick = pool[0];
                if (pick != null) { b.Treasure[cell] = pick.Item; if (lux) b.Luxury.Add(cell); }
            }
            var free = Enumerable.Range(2, Cells - 1).Where(i => !TreasureCells.Contains(i) && !HuntCells.Contains(i)).ToList();
            for (int k = 0; k < cfg.EncountersPerLap && free.Count > 0; k++)
            {
                int i = rng.Next(free.Count);
                b.Encounter[free[i]] = 1 + rng.Next(3);
                free.RemoveAt(i);
            }
            return b;
        }
        static bool CellUsed(uint uid, int cell) { return Get(uid, "used" + Get(uid, "lap") + "_" + cell) != 0; }
        static void SendBoard(Client.GameClient c, Config cfg)
        {
            uint uid = c.Player.UID;
            var b = BoardOf(uid, cfg);
            bool first = true;
            var cells = new List<string>();
            foreach (var kv in b.Treasure) if (!CellUsed(uid, kv.Key)) cells.Add("['Index']=" + kv.Key + ",['Event']=" + (b.Luxury.Contains(kv.Key) ? 5 : 3) + ",['ItemInfo']='" + kv.Value.Lua() + "'");
            foreach (var kv in b.Encounter) if (!CellUsed(uid, kv.Key)) cells.Add("['Index']=" + kv.Key + ",['Event']=2,['EventIndex']=" + kv.Value);
            foreach (int h in HuntCells) cells.Add("['Index']=" + h + ",['Event']=4");
            foreach (var s in cells)
            {
                Lua(c, "DlgThanksgivingTribeMainUi_UpdateMapDatas|{['Code']=200,['Data']={" + (first ? "['State']=0," : "") + "['MapDatas']={" + s + "}}}");
                first = false;
            }
        }
        static void Open(Client.GameClient c, Config cfg)
        {
            PayOwed(c);
            Lua(c, "DlgThanksgivingTribeMainUi_OpenMainUi|" + MainData(c, true));
            SendBoard(c, cfg);
        }
        static void Refresh(Client.GameClient c) { Lua(c, "DlgThanksgivingTribeMainUi_UpdatePropData|" + MainData(c, false)); }

        // ------------------------------------------------------------------ throws
        static int CellType(Board b, int cell)
        {
            if (HuntCells.Contains(cell)) return 4;
            if (b.Treasure.ContainsKey(cell)) return b.Luxury.Contains(cell) ? 5 : 3;
            if (b.Encounter.ContainsKey(cell)) return 2;
            return 1;
        }
        /// <summary>One or ten throws. fixedPoint 1-6 = Lunar Runestone, 0 = Stellar Runestone (random).</summary>
        static void Throw(Client.GameClient c, Config cfg, int fixedPoint, int times)
        {
            uint uid = c.Player.UID;
            uint stone = fixedPoint > 0 ? LunarRunestone : StellarRunestone;
            if (Get(uid, "pend") != 0) { Event(c, cfg); }// the last landing was not claimed (window closed): claim it first
            if (HKit.Count(c, stone) < (uint)times)
            {
                Msg(c, fixedPoint > 0 ? "Failed to move forward for lack of Lunar Runestones. You can obtain some by buying packs or exchanging Animae for them in the Exchange Store."
                                      : "You don`t have enough Stellar Runestones (" + times + " needed). You can buy them with CPs.");
                return;
            }
            int start = Pos(uid), cur = start, steps = 0;
            var auto = new List<string>();
            int lastType = 1;
            for (int t = 0; t < times; t++)
            {
                if (!HKit.Take(c, stone, 1)) break;
                int p = fixedPoint > 0 ? fixedPoint : 1 + HKit.Roll(6);
                steps += p;
                int dest = cur + p;
                bool lap = dest > Cells;
                if (lap) dest -= Cells;
                // points + crystals for every step, position and the landing (pending) recorded first
                long pts = (long)p * cfg.PointsPerStep;
                Store.Tx(uid, m =>
                {
                    Inc(m, Key("pts"), pts);
                    m[Key("pos")] = dest;
                    if (lap) Inc(m, Key("lap"), 1);
                    m[Key("pend")] = dest;
                    return 0;
                });
                HKit.Give(c, new[] { new HItem(HuntingCrystal, (int)pts, true) });
                HKit.Log("THROW", Festival, c, (fixedPoint > 0 ? "lunar " : "stellar ") + p + " -> cell " + dest + (lap ? " (new lap)" : "") + " +" + pts + " pts/crystals");
                cur = dest;
                lastType = CellType(BoardOf(uid, cfg), dest);
                if (times > 1 && t < times - 1)
                {
                    string r = Resolve(c, cfg, true);// ten throws: every landing but the last is claimed on the way
                    if (!string.IsNullOrEmpty(r)) auto.Add(r);
                }
            }
            if (auto.Count > 0) Msg(c, "On the way: " + string.Join("; ", auto) + ".");
            Lua(c, "DlgThanksgivingTribeMainUi_ThrowReturnData|{['Code']=200,['Data']={['CurIndex']=" + start + ",['Point']=" + Math.Min(steps, Cells - 1) + ",['Starstone']=" + HKit.Count(c, StellarRunestone)
                + ",['Moonstone']=" + HKit.Count(c, LunarRunestone) + ",['EventHappen']=" + lastType + ",['MonsterLv']=" + Levels(uid) + "}}");
            if (steps >= Cells - 1 || start + steps > Cells) SendBoard(c, cfg);
        }
        static void Inc(Dictionary<string, long> m, string k, long d) { long v; m.TryGetValue(k, out v); m[k] = v + d; }

        /// <summary>Client "_Event" after the walk: the landing cell's event (once per landing).</summary>
        static void Event(Client.GameClient c, Config cfg)
        {
            string r = Resolve(c, cfg, false);
            if (!string.IsNullOrEmpty(r)) Msg(c, r);
            Refresh(c);
        }
        static string Resolve(Client.GameClient c, Config cfg, bool onTheWay)
        {
            uint uid = c.Player.UID;
            int cell = (int)Get(uid, "pend");
            if (cell < 1 || cell > Cells) return null;
            var b = BoardOf(uid, cfg);
            int type = CellType(b, cell);
            string usedKey = Key("used" + Get(uid, "lap") + "_" + cell);
            List<HItem> give = new List<HItem>();
            string what;
            if (type == 2 && !CellUsed(uid, cell))
            {
                if (onTheWay) { Store.Set(uid, Key("pend"), 0); return null; }// an encounter only starts where you stop
                int ev = b.Encounter[cell];
                string why = HolidayBattles.CanStart(c);
                if (why == null && !HolidayBattles.CityMaps.Contains(c.Player.Map)) why = "Please go to a city first to start the adventure.";
                if (why != null) { Store.Set(uid, Key("pend"), 0); return why + " The adventure was skipped."; }
                // claimed before the battle starts (no repeat by reopening the window)
                if (!Store.Tx(uid, m => { long v; if (m.TryGetValue(usedKey, out v) && v != 0) return false; m[usedKey] = 1; m[Key("pend")] = 0; return true; })) return null;
                StartEncounter(c, cfg, ev);
                return null;
            }
            if (type == 4)
            {
                int boss = Array.IndexOf(HuntCells, cell) + 1;
                long lv = Store.Tx(uid, m => { long v; m.TryGetValue(Key("boss" + boss), out v); if (v < 1) v = 1; long n = Math.Min(5, v + 1); m[Key("boss" + boss)] = n; m[Key("pend")] = 0; return n; });
                give.Add(new HItem(TribeSilverCoin, (int)(5 * lv), true));// derived
                if (lv == 5) give.Add(new HItem(TribeGoldCoin, 2, true));// derived: a max-level hunt also pays gold
                what = "the hunting point (boss level " + lv + ")";
                Lua(c, "DlgThanksgivingTribeMainUi_BossUplvData|{['Code']=200,['Data']={['BossId']=" + boss + ",['BossLv']=" + lv + "}}");
            }
            else if ((type == 3 || type == 5) && !CellUsed(uid, cell))
            {
                if (!Store.Tx(uid, m => { long v; if (m.TryGetValue(usedKey, out v) && v != 0) return false; m[usedKey] = 1; m[Key("pend")] = 0; return true; })) return null;
                give.Add(b.Treasure[cell]);
                what = type == 5 ? "a luxury treasure" : "an advanced treasure";
                Lua(c, "DlgThanksgivingTribeMainUi_OneceUpdateMap|{['Code']=200,['Data']={['MapDatas']={['Index']=" + cell + ",['Event']=1}}}");
            }
            else
            {
                var p = HKit.Draw(cfg.Normal);
                Store.Set(uid, Key("pend"), 0);
                if (p != null) give.Add(p.Item);
                what = "a treasure";
            }
            if (give.Count == 0) return null;
            if (!HKit.HasRoom(c, give))
            {
                foreach (var g in give) Store.Add(uid, Key("owe" + g.Id), g.Count);// kept for the next open
                return "Your inventory is full. Your reward is kept for you.";
            }
            string got = HKit.Give(c, give);
            HKit.Log("CELL", Festival, c, "cell " + cell + " " + what + " -> " + got);
            return "You found " + what + ": " + got;
        }

        static void StartEncounter(Client.GameClient c, Config cfg, int ev)
        {
            var spec = new BattleSpec { Festival = Festival, Map = cfg.BattleMap, X = cfg.BattleX, Y = cfg.BattleY, Spread = 9, Seconds = 300 };
            switch (ev)
            {
                case 1: spec.Name = "Moonlight Treasure"; spec.StartText = "The Moonlight Treasure has always been guarded by my follower, Alert Turkey. If you can take the treasure from Alert Turkey, I will give it to you.";
                    spec.Waves.Add(new BattleWave(AlertTurkey, 3)); break;
                case 2: spec.Name = "Gold Eaters"; spec.StartText = "Those Gold Eaters have hidden the key in darkness. Only by lighting two lanterns can its hiding place be revealed. Please obtain the pearls from Gold Eaters and light the lanterns!";
                    spec.Waves.Add(new BattleWave(GoldEater, 12)); spec.Waves.Add(new BattleWave(GoldEater, 12)); break;
                default: spec.Name = "Tribe Altar"; spec.StartText = "There are countless spirits in the Tribe Altar guarding the most precious treasure.";
                    spec.Waves.Add(new BattleWave(AltarSpirit, 15)); spec.Waves.Add(new BattleWave(AltarGuardian, 1)); break;
            }
            spec.FailText = "You failed the adventure. Keep hunting!";
            HKit.Log("ENCOUNTER", Festival, c, spec.Name);
            string err = HolidayBattles.Start(c, spec, ev, (run, ok) =>
            {
                var cc = HolidayBattles.World.Online(run.Owner);
                if (!ok || cc == null) return;
                var give = Cfg.EncounterReward;
                if (!HKit.HasRoom(cc, give)) { foreach (var g in give) Store.Add(run.Owner, Key("owe" + g.Id), g.Count); HKit.Msg(cc, "Your inventory is full. Your reward is kept for you."); return; }
                string got = HKit.Give(cc, give);
                HKit.Log("ENCOUNTER-WIN", Festival, cc, spec.Name + " -> " + got);
                HKit.Center(cc, ev == 1 ? "I haven`t encountered such a formidable hero in a long time! I will fulfill my promise, and the Moonlight Treasure is yours!" : "Well done, hero! You received " + got + ".");
            });
            if (err != null) Msg(c, err);
        }
        static void PayOwed(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            var owed = Store.Snapshot(uid).Where(kv => kv.Key.StartsWith(Key("owe")) && kv.Value > 0).ToList();
            if (owed.Count == 0) return;
            var give = owed.Select(kv => new HItem(uint.Parse(kv.Key.Substring(Key("owe").Length)), (int)Math.Min(100000, kv.Value), true)).ToList();
            if (!HKit.HasRoom(c, give)) return;
            if (!Store.Tx(uid, m => { foreach (var kv in owed) { long v; m.TryGetValue(kv.Key, out v); if (v < kv.Value) return false; } foreach (var kv in owed) m[kv.Key] -= Math.Min(100000, kv.Value); return true; })) return;
            HKit.Log("OWED", Festival, c, HKit.Give(c, give));
        }

        // ------------------------------------------------------------------ tribes / runestones
        static void HandIn(Client.GameClient c, Config cfg, int tribe)
        {
            if (tribe != 1 && tribe != 2) return;
            uint uid = c.Player.UID;
            uint n = HKit.Count(c, HuntingCrystal);
            if (n == 0) { Msg(c, "You have no Hunting Crystals to submit."); return; }
            if (!HKit.Take(c, HuntingCrystal, n)) return;
            Store.Add(Global, Key("tribe" + tribe), n);
            Store.Add(uid, Key("give" + tribe), n);
            HKit.Log("HANDIN", Festival, c, n + " crystals -> " + (tribe == 1 ? "Dusk" : "Dawn"));
            Msg(c, "You submitted " + n + " Hunting Crystals to the " + (tribe == 1 ? "Dusk" : "Dawn") + " Tribe.");
            Refresh(c);
        }
        static void Buy(Client.GameClient c, Config cfg, int which)
        {
            uint item; int price;
            if (which == 1) { item = StellarRunestone; price = cfg.StellarCps; }
            else if (which == 2) { item = LunarRunestone; price = cfg.LunarCps; }
            else return;
            var give = new List<HItem> { new HItem(item, 1, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Your inventory is full. Please clear it up first."); return; }
            if (!HKit.Confirmed(c.Player.UID, "tb23buy" + which)) { Msg(c, "Pay " + price + " CPs for 1 " + HKit.Name(item) + "? Click again within 15 seconds to confirm."); return; }
            if (HKit.Cps(c) < price || !HKit.TakeCps(c, (uint)price)) { Msg(c, "You don`t have enough CPs."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("BUY", Festival, c, price + " CPs -> " + got);
            Msg(c, "You bought " + got + ".");
            Refresh(c);
        }

        // ------------------------------------------------------------------ Tribe Treasure Hunt (Treasure Compass)
        static void OpenDraw(Client.GameClient c, Config cfg)
        {
            Lua(c, "GThanksGivingDraw_OpenViewDatas|{['Code']=200,['Data']={['ActivityPoints']=" + Get(c.Player.UID, "pts") + ",['PropItem']={" + Item(c, TribeGoldCoin) + "," + Item(c, TribeSilverCoin)
                + "},['HaveMoney']=" + Item(c, TreasureCompass) + ",['DrawCost']={" + cfg.DrawCost1 + "," + cfg.DrawCost10 + "}}}");
        }
        static void Draw(Client.GameClient c, Config cfg, int times)
        {
            if (times != 1 && times != 10) return;
            int cost = times == 1 ? cfg.DrawCost1 : cfg.DrawCost10;
            if (HKit.Count(c, TreasureCompass) < cost) { Msg(c, "You don`t have enough Treasure Compasses."); return; }
            var give = new List<HItem>();
            for (int i = 0; i < times; i++) { var p = HKit.Draw(cfg.Draw); if (p != null) give.Add(p.Item); }
            if (!HKit.HasRoom(c, give)) { Msg(c, "Your inventory is full. Please clear it up first."); return; }
            if (!HKit.Take(c, TreasureCompass, (uint)cost)) return;
            string got = HKit.Give(c, give);
            HKit.Log("HUNT", Festival, c, "x" + times + " -> " + got);
            var rnd = Enumerable.Range(0, times).Select(i => 1 + HKit.Roll(10)).ToList();
            string box = times == 1 ? rnd[0].ToString(CultureInfo.InvariantCulture) : "{" + string.Join(",", rnd.Distinct()) + "}";
            Lua(c, "GThanksGivingDraw_DrawResultReturn|{['Code']=200,['Data']={['HaveMoney']=" + Item(c, TreasureCompass) + ",['Type']=" + times + ",['BoxId']=" + box + ",['Rewards']=" + HKit.LuaItems(give) + "}}");
        }

        // ------------------------------------------------------------------ Timo (Tribe Silver Coins, Gold Coin = 10 Silver)
        static long Wallet(Client.GameClient c, Config cfg) { return HKit.Count(c, TribeSilverCoin) + (long)HKit.Count(c, TribeGoldCoin) * cfg.GoldToSilver; }
        static string GoodLua(uint uid, Thanksgiving2022Ui.ShopGood g, int i)
        {
            long bought = Get(uid, "shop" + i), got = Get(uid, "shopr" + i);
            return "'" + g.Item.Lua() + "," + g.Cost + " " + Math.Max(0, g.Limit - bought) + " " + Math.Min(bought, g.Target) + " " + g.Target + (got != 0 ? " 1" : "") + "'";
        }
        static string Money(Client.GameClient c, Config cfg) { return "'" + TribeSilverCoin + " 0 1," + Wallet(c, cfg) + "'"; }
        static void OpenShop(Client.GameClient c, Config cfg)
        {
            uint uid = c.Player.UID;
            Lua(c, "ThanksgivingExchangeShop_OpenViewDatas|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, cfg) + ",['GoodItems']={" + string.Join(",", cfg.Shop.Select((g, i) => GoodLua(uid, g, i + 1))) + "},['RewardItem']='" + ThornCrown + " 0 1'}}");
        }
        static void ShopMax(Client.GameClient c, Config cfg, int i)
        {
            if (i < 1 || i > cfg.Shop.Count) return;
            var g = cfg.Shop[i - 1];
            long left = Math.Max(0, g.Limit - Get(c.Player.UID, "shop" + i)), have = Wallet(c, cfg);
            Lua(c, "DlgGThanksGivingExchangeConfirmUi_ReturnMaxNum|{['Code']=200,['Data']={['Maxnum']=" + Math.Max(1, Math.Min(left, have / g.Cost)) + ",['Coinnum']=" + have + "}}", 10133);
        }
        static void Lua(Client.GameClient c, string call, uint msg) { LuaUi.LuaPush.Send(c, call, msg); }
        /// <summary>Pays silver first, then gold (10 silver each, change returned in silver).</summary>
        static bool PayCoins(Client.GameClient c, Config cfg, long cost)
        {
            uint silver = HKit.Count(c, TribeSilverCoin);
            if (silver >= cost) return HKit.Take(c, TribeSilverCoin, (uint)cost);
            long rest = cost - silver;
            long gold = (rest + cfg.GoldToSilver - 1) / cfg.GoldToSilver;
            long change = gold * cfg.GoldToSilver - rest;
            if (HKit.Count(c, TribeGoldCoin) < gold) return false;
            if (silver > 0 && !HKit.Take(c, TribeSilverCoin, silver)) return false;
            if (!HKit.Take(c, TribeGoldCoin, (uint)gold)) { HKit.Give(c, new[] { new HItem(TribeSilverCoin, (int)silver, true) }); return false; }
            if (change > 0) HKit.Give(c, new[] { new HItem(TribeSilverCoin, (int)change, true) });
            return true;
        }
        static void ShopBuy(Client.GameClient c, Config cfg, int i, int num)
        {
            if (i < 1 || i > cfg.Shop.Count || num < 1 || num > 9999) return;
            uint uid = c.Player.UID;
            var g = cfg.Shop[i - 1];
            long left = g.Limit - Get(uid, "shop" + i);
            if (num > left) { Msg(c, "You can exchange this item " + Math.Max(0, left) + " more time(s)."); return; }
            long cost = (long)g.Cost * num;
            if (Wallet(c, cfg) < cost) { Msg(c, "You don`t have enough Tribe Coins."); return; }
            var give = new List<HItem> { new HItem(g.Item.Id, g.Item.Count * num, g.Item.Bound), new HItem(TribeSilverCoin, cfg.GoldToSilver, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            give.RemoveAt(1);
            if (!Store.Tx(uid, m => { long b; m.TryGetValue(Key("shop" + i), out b); if (b + num > g.Limit) return false; m[Key("shop" + i)] = b + num; return true; })) return;
            if (!PayCoins(c, cfg, cost)) { Store.Add(uid, Key("shop" + i), -num); Msg(c, "You don`t have enough Tribe Coins."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("TIMO", Festival, c, "paid " + cost + " silver -> " + got);
            Lua(c, "ThanksgivingExchangeShop_ExchangeResultReturn|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, cfg) + ",['GoodIndex']=" + i + ",['GoodItem']=" + GoodLua(uid, g, i) + "}}", 10133);
            Msg(c, "You received " + got + ".");
        }
        static void ShopCrown(Client.GameClient c, Config cfg, int i)
        {
            if (i < 1 || i > cfg.Shop.Count) return;
            uint uid = c.Player.UID;
            var g = cfg.Shop[i - 1];
            if (Get(uid, "shop" + i) < g.Target) { Msg(c, "Exchange this item " + g.Target + " times to claim 1 Thorn Crown."); return; }
            var give = new List<HItem> { new HItem(ThornCrown, 1, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            if (!Store.Claim(uid, Key("shopr" + i))) { Msg(c, "You`ve already claimed this reward."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("TIMO-CROWN", Festival, c, "item " + i + " -> " + got);
            Lua(c, "ThanksgivingExchangeShop_GetRewardReturn|{['Code']=200,['Data']={['GoodIndex']=" + i + ",['GoodItem']=" + GoodLua(uid, g, i) + "}}", 10133);
        }

        // ------------------------------------------------------------------ Hunting Points stage rewards
        static long RepeatEarned(Config cfg, long pts)
        {
            long top = cfg.StageThresholds[cfg.StageThresholds.Length - 1];
            return pts <= top ? 0 : Math.Min(cfg.StageRepeatMax, (pts - top) / Math.Max(1, cfg.StageRepeatStep));
        }
        public static string StageData(uint uid, Config cfg)
        {
            long pts = Get(uid, "pts");
            var can = new List<int>(); var no = new List<int>(); var done = new List<int>();
            for (int i = 1; i <= cfg.StageThresholds.Length; i++)
            {
                if (Get(uid, "stage" + i) != 0) done.Add(i);
                else if (pts >= cfg.StageThresholds[i - 1]) can.Add(i); else no.Add(i);
            }
            int rep = cfg.StageThresholds.Length + 1;
            if (RepeatEarned(cfg, pts) > Get(uid, "stage" + rep)) can.Add(rep); else if (Get(uid, "stage" + rep) >= cfg.StageRepeatMax) done.Add(rep); else no.Add(rep);
            return "{['Code']=200,['Data']={['CanClaim']={" + string.Join(",", can) + "},['NoClaim']={" + string.Join(",", no) + "},['Claimed']={" + string.Join(",", done) + "}}}";
        }
        static void StageGet(Client.GameClient c, Config cfg, int i)
        {
            uint uid = c.Player.UID;
            int rep = cfg.StageThresholds.Length + 1;
            if (i < 1 || i > rep || i > cfg.StageItems.Count) return;
            var give = cfg.StageItems[i - 1];
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            long pts = Get(uid, "pts");
            bool ok;
            if (i < rep)
            {
                if (pts < cfg.StageThresholds[i - 1]) { Msg(c, "You need " + cfg.StageThresholds[i - 1] + " Hunting Points to claim this reward."); return; }
                ok = Store.Claim(uid, Key("stage" + i));
            }
            else
            {
                long earned = RepeatEarned(cfg, pts);
                ok = Store.Tx(uid, m => { long v; m.TryGetValue(Key("stage" + rep), out v); if (v >= earned) return false; m[Key("stage" + rep)] = v + 1; return true; });
            }
            if (!ok) { Msg(c, "You`ve already claimed this reward."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("STAGE", Festival, c, "stage " + i + " (" + pts + " pts) -> " + got);
            Lua(c, "DlgGlobalThanksgivingRank_BindData|" + StageData(uid, cfg));
        }

        // ------------------------------------------------------------------ boxes / Thorn Crown (official item tips)
        public static void RegisterBoxes()
        {
            HolidayBoxes.Register(PlatinumBox, c => HolidayBoxes.Open(c, Festival, PlatinumBox, new List<HItem> { new HItem(LunarRunestone, 4, true), new HItem(StellarRunestone, 2, true) }));
            HolidayBoxes.Register(GoldBox, c => HolidayBoxes.Open(c, Festival, GoldBox, new List<HItem> { new HItem(LunarRunestone, 8, true), new HItem(StellarRunestone, 4, true) }));
            HolidayBoxes.Register(BlackgoldBox, c => HolidayBoxes.Open(c, Festival, BlackgoldBox, new List<HItem> { new HItem(LunarRunestone, 16, true), new HItem(StellarRunestone, 8, true) }));
            HolidayBoxes.Register(CompassBox, c => HolidayBoxes.Open(c, Festival, CompassBox, new List<HItem> { new HItem(TreasureCompass, 1 + HKit.Roll(4), true) }));// "randomly get 1-4"
            HolidayBoxes.Register(BlazingSunBox, c => HolidayBoxes.Open(c, Festival, BlazingSunBox, new List<HItem> { new HItem(BlazingSun, 1, false) }));
            HolidayBoxes.Register(PatrollingShadowBox, c => HolidayBoxes.Open(c, Festival, PatrollingShadowBox, new List<HItem> { new HItem(PatrollingShadow, 1, false) }));
            HolidayBoxes.Register(BlazingSunDawnBox, c => HolidayBoxes.Open(c, Festival, BlazingSunDawnBox, new List<HItem> { new HItem(BlazingSunDawn, 1, false) }));
            HolidayBoxes.Register(PatrollingShadowRulingBox, c => HolidayBoxes.Open(c, Festival, PatrollingShadowRulingBox, new List<HItem> { new HItem(PatrollingShadowRuling, 1, false) }));
            HolidayBoxes.Register(ThornCrown, UseCrown);
        }
        /// <summary>Thorn Crown (tip): 50 -> 5-star (Ruling)/(Dawn) box, 30 -> 3-star box. Dawn Tribe contributors get Blazing Sun,
        /// everyone else Patrolling Shadow (DERIVED choice; the Conversion Potion converts them officially).</summary>
        static bool UseCrown(Client.GameClient c)
        {
            uint have = HKit.Count(c, ThornCrown);
            bool dawn = Get(c.Player.UID, "give2") > Get(c.Player.UID, "give1");
            uint need, box;
            if (have >= 50) { need = 50; box = dawn ? BlazingSunDawnBox : PatrollingShadowRulingBox; }
            else if (have >= 30) { need = 30; box = dawn ? BlazingSunBox : PatrollingShadowBox; }
            else { Msg(c, "30 crowns can be combined into 1 permanent garment Patrolling Shadow (B)/Blazing Sun (B), and 50 crowns into 1 permanent 5-star garment Patrolling Shadow (Ruling)/Blazing Sun (Dawn). You have " + have + "."); return true; }
            var give = new List<HItem> { new HItem(box, 1, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return true; }
            if (!HKit.Take(c, ThornCrown, need)) return true;
            string got = HKit.Give(c, give);
            HKit.Log("CROWN", Festival, c, need + " crowns -> " + got);
            Msg(c, "You combined " + need + " Thorn Crowns into " + got + ".");
            return true;
        }
    }
}
