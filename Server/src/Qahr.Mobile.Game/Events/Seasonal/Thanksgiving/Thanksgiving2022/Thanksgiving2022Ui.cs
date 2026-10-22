// ============================================================================
//  [Thanksgiving2022 2026-10-14 / COMPLETE 2026-10-15] Global Thanksgiving 2022 "Battlefront of Subjugation"
//  (ERP519620, Nov. 22 - Dec. 5, luaUiRes_EN[1303].Rank.ActivityTime), client tMsg_Info["GlobalThanksgiving"] = 10133.
//
//  Official client (luaui\dialog\dlgglobalthanksgiving, luagui\1303.dat + 1304.dat, luaUiRes_EN[1303]/["1304-x"],
//  Server_Key tThanksgiving2022JuDian / tThanksGivingPage2022 / tThanksgivingPrayer / tFragments[3339557]):
//    main     DlgGlobalThanksgivingMain_BindData {Points,ItemNum={silver,golden},CoolingTime={5 big forts},Monster,Unlock,AutoFight}
//             + DlgGlobalThanksgivingMain_UpdateData {MonsterReward={forts},RandomPlace={4 flags}}
//    forts    23 forts, big = 4/8/12/17/23. Silver Warrant (3600430) enters a small fort, Golden Warrant (3600429) a big one;
//             missing warrant -> buy for 30 / 120 CPs (Msg100/101). Big forts open 00:00 Nov. 22/23/24/25 (Locked[]).
//             Capture rewards (official texts): fort 1 = 10 Thanksgiving Coins + 5 Turkey Feathers; small = 15 Points
//             + 10 Coins + 5 Feathers; big = 60 Points + 40 Coins + 20 Feathers. First capture -> Small / Big 1st
//             Conquest Box (3339605 / 3339606, luagui preview). Mop-up = the same rewards for 1 warrant (15 / 60 CPs if
//             missing, tThanksGivingPage2022[NoSweepItem]); big forts have a cooldown; "Mop up all" = CPs, ignores the
//             cooldown, 8% off (luaUiRes AutoFight).
//    random   4 random forts (Warbler / Chaos Goblin / Turkey Treasury / Wandering Spirit) -> Fire Warbler Box 3339558,
//             Chaos Goblin Box 3338899, Hidden Treasure Box 3338900, Grateful Box 3338901 (Msg76/80/87/93).
//    store    Hunter Supply Store (Turkey Feathers 3338895, Dialog_Info["1304-1-other"].shopData) + 1 Hunter Puzzle per
//             item after N exchanges ("Exchanging each item for a certain number of times will give you 1 garment puzzle").
//    depot    Military Depot = DlgGThanksGivingDraw (Thanksgiving Coins 3338896 -> random treasures, preview
//             Dialog_Info["1304-4-other"]) + Hunter Puzzles at 10/30/50/80/120 draws (Dialog_Info["1304-3-other"]).
//             The depot's draw buttons call msg 10319 ThanksgivingMainNpc_ChouJiang -> ThanksgivingTribe2023Ui delegates
//             here while the player's last opened Thanksgiving window was this one (SharedOwner).
//    stages   Thanksgiving Points stage rewards: DlgGlobalThanksgivingRank (500/1500/2500/5000/10000/30000/50000/77777,
//             then every 2000, up to 500 times; items = Dialog_Info["1303-rank-item"].Total).
//    daily    2 Silver Warrants once a day on opening the window (Msg107/112 "log in again to get 2 Silver Warrants").
//    puzzle   Hunter Puzzle 3339557: 25 -> Turkey Hunter (B) 199775, 50 -> Turkey Hunter (Terminator) 199785.
//  DERIVED (Infinity[DB]\Thanksgiving2022\thanksgiving2022.json): the battle map (12660 TurkeyRefuge, the official
//  Thanksgiving fort map ids are not in the client), wave sizes, time limits that the texts don't give, cooldown, draw
//  odds + cost, shop limits / puzzle targets, box contents, monster looks/stats.
//  NOT on this server: the cross-server parts (Sharpen Weapon, Turkey Counsellor) and the global/local ranking pages.
//
//  Hooks: Base\LuaUi\LuaPush.cs HandleCallback (msg 10133); MsgItemUsuagePacket UseItem (HolidayBoxes).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Thanksgiving2022
{
    public static class Thanksgiving2022Ui
    {
        public const uint LuaMsg = 10133;
        public const string Festival = "Thanksgiving2022";
        public const uint SilverWarrant = 3600430, GoldenWarrant = 3600429, Coin = 3338896, Feather = 3338895, Puzzle = 3339557;
        public const uint SmallFirstBox = 3339605, BigFirstBox = 3339606;
        public const uint WarblerBox = 3339558, GoblinBox = 3338899, TreasureBox = 3338900, GratefulBox = 3338901;
        public const uint HunterB = 199775, HunterTerminator = 199785;
        public static readonly int[] BigForts = { 4, 8, 12, 17, 23 };
        public const string Ended = "The Global Thanksgiving event has ended. See you next time!";

        // ------------------------------------------------------------------ monsters (Festivals\Thanksgiving2022\monsters)
        public const uint JealousImp = 995001, JealousPhantom = 995002, Jealous5thBro = 995003, BroClone5 = 995004,
            GluttonousRat = 995005, GluttonousSnake = 995006, Glutton4thSis = 995007, GreedyImp = 995008, BigGreedyImp = 995009,
            Greedy3rdBro = 995010, RagingBull = 995011, Furious2ndSis = 995012, SisClone2 = 995013, ArrogantImp = 995014,
            MagicArray = 995015, ArrogantBigBro = 995016, WarblerTroop = 995017, WarblerLeader = 995018, ChaosGoblin = 995019,
            TreasurePuppet = 995020, WanderingSpirit = 995021;

        public sealed class Fort
        {
            public int No; public bool Big; public int Seconds; public string Start, Win, Fail;
            public List<BattleWave> Waves = new List<BattleWave>();
            public int Points, Coins, Feathers; public uint Box;// Box: random forts only
        }
        static string K(int i) { return Texts.ContainsKey(i) ? Texts[i] : ""; }
        // official Server_Key tThanksgiving2022JuDian[Msg][n] (EN)
        static readonly Dictionary<int, string> Texts = new Dictionary<int, string>
        {
            {1,"Jealous Imps are coming at you! Please clear all of their forts within 200 seconds!"},
            {2,"Congrats! You`ve killed all the imps, taken Fort 1, and received 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {3,"Jealous Imps are showing off!\nPlease improve your strength before you come back!"},
            {4,"Jealous Imps are coming at you from everywhere! Please kill 15 imps within 30 seconds!"},
            {5,"Jealous Phantom rushed out from the dark! Hurry up and defeat it!"},
            {6,"Another wave of Jealous Imps are coming! Please kill 15 imps within 30 seconds!"},
            {7,"You`ve defeated Jealous Phantom, taken Fort 2, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {8,"Jealous Imps are coming at you from everywhere! Please kill 15 imps within 30 seconds!"},
            {9,"Jealous Phantom is revived! Hurry up and defeat it!"},
            {10,"Another wave of Jealous Imps are coming! Please kill 15 imps within 30 seconds!"},
            {11,"You`ve defeated Jealous Phantom, taken Fort 3, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {12,"Be careful of Jealous 5th Turkey Brother`s clone charm!"},
            {13,"Jealous 5th Turkey Brother cast the clone charm! Please defeat his clone!"},
            {14,"You`ve defeated the clone, taken the main fort, and received 60 Thanksgiving Points, 40 Thanksgiving Coins and 20 Turkey Feathers."},
            {15,"Failed to defeat Jealous 5th Turkey Brother. Please improve your strength before you come back!"},
            {16,"Gluttonous Rat is coming at you! Go kill it!"},
            {17,"Gluttonous Rat has run away, and you`ve taken the fort and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {18,"Please improve your strength before you come back!"},
            {19,"There are too many Gluttonous Rats. Please stay alert!"},
            {20,"2 Gluttonous Rats are coming at you! Be careful!"},
            {21,"More Gluttonous Rats are coming! Hurry up and defeat them!"},
            {22,"The number of Gluttonous Rats is increasing. Hurry up and kill them!"},
            {23,"A big wave of Gluttonous Rats are coming! Be careful!"},
            {25,"Gluttonous Rats are escaping! You`ve taken the fort and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {26,"Gluttonous Snakes always appear in groups. Please stay alert!"},
            {27,"2 Gluttonous Snakes are coming this way! Hurry up and defeat them!"},
            {28,"Many Gluttonous Snakes are coming! Go kill them!"},
            {29,"There are more Gluttonous Snakes! Hurry up and kill them!"},
            {30,"A big wave of Gluttonous Snakes are coming at you! Be careful!"},
            {32,"Gluttonous Snakes are escaping! You`ve taken the fort and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {33,"Gluttonous 4th Turkey Sister takes you as food! You must defeat her first!"},
            {34,"Gluttonous 4th Turkey Sister has called for help! Be careful!"},
            {35,"You`ve defeated Gluttonous 4th Turkey Sister, taken the main fort, and received 60 Thanksgiving Points, 40 Thanksgiving Coins and 20 Turkey Feathers."},
            {36,"Failed to defeat Gluttonous 4th Turkey Sister. Please work harder."},
            {37,"You`re surrounded by Greedy Imps! Go kill them!"},
            {38,"Greedy Imps have become bigger! Please stay alert!"},
            {39,"Failed to defeat Greedy Imps. Please work harder."},
            {40,"You`ve defeated Greedy Imps, taken Fort %s, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {41,"Greedy 3rd Turkey Brother is good at casting spells. Please stay alert!"},
            {42,"Greedy 3rd Turkey Brother has got many minions. Be careful!"},
            {43,"You`ve defeated Greedy 3rd Turkey Brother, taken the main fort, and received 60 Thanksgiving Points, 40 Thanksgiving Coins and 20 Turkey Feathers."},
            {44,"Failed to defeat Greedy 3rd Turkey Brother. Please work harder."},
            {45,"Raging Bulls are trying to destroy everything! You must defeat them all within 200 seconds!"},
            {46,"Failed to defeat all Raging Bulls. Please work harder."},
            {47,"You`ve stopped Raging Bulls, taken Fort %s, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {48,"Raging Bulls are making a lot of trouble! Please expel them all!"},
            {49,"Failed to defeat all bulls. Please work harder."},
            {54,"Furious 2nd Turkey Sister is really strong! Please go all out!"},
            {55,"Her clone has disappeared. Hurry up and defeat Furious 2nd Turkey Sister!"},
            {56,"You`ve defeated Furious 2nd Turkey Sister, taken the fort, and received 60 Thanksgiving Points, 40 Thanksgiving Coins and 20 Turkey Feathers."},
            {57,"Please kill all Arrogant Imps in the fort within 500 seconds!"},
            {58,"3rd Brother`s Greed is guarding the frontline. Defeat it to take the fort!"},
            {59,"You`ve defeated all Arrogant Imps, taken the fort, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {60,"Failed to defeat all Arrogant Imps. Please work harder."},
            {61,"Please kill 200 Arrogant Imps within 500 seconds!"},
            {62,"You`ve killed 200 Arrogant Imps, taken the fort, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {63,"Failed to defeat 200 Arrogant Imps. Please work harder."},
            {64,"Arrogant Imps are guarding something. Please defeat them and take a look!"},
            {66,"You`ve closed the teleporter, taken the fort, and received 15 Thanksgiving Points, 10 Thanksgiving Coins and 5 Turkey Feathers."},
            {67,"Arrogant Imps` reinforcements have arrived! Hurry up and defeat them!"},
            {69,"Please improve your strength before you come back!"},
            {70,"Please destroy the randomly appeared array [5 times] within 500 seconds to weaken Big Turkey Brother, and then defeat him!"},
            {71,"The magic array has been destroyed. Hurry up and defeat Big Turkey Brother!"},
            {72,"Please improve your strength before you come back!"},
            {73,"You`ve defeated Arrogant Big Turkey Brother, taken the fort, and received 60 Thanksgiving Points, 40 Thanksgiving Coins and 20 Turkey Feathers."},
            {74,"Don`t underestimate the Warbler Troop!"},
            {75,"Warbler Leader is coming at you! Be careful!"},
            {76,"You`ve defeated Warbler Leader, taken the fort, and received 1 Fire Warbler Box."},
            {77,"Please improve your strength before you come back!"},
            {78,"You`re affected by Super Chaos. Go pick some Spiritual Fruits and eat them to recover."},
            {80,"Your chaotic state has been cured, and you feel refreshed. Congrats! You`ve taken the fort and received 1 Chaos Goblin Box."},
            {81,"There are treasures in the treasure box and the treasure puppet. Go take a look!"},
            {87,"Congrats! You received 1 Hidden Treasure Box."},
            {88,"What a pity! You failed to find 15 treasures. Please work harder."},
            {89,"You can`t see anything in the dark room. You must defeat the Wandering Spirit and light up the Soul Lamp to get out."},
            {93,"Following the light of the lamp, you`ve found the way out! You received 1 Grateful Box."},
            {99,"The current fort has not been unlocked yet. Please capture the previous fort first!"},
            {100,"You still need 1 Silver Warrant to enter the fort.\nDo you want to pay 30 CPs to buy one?"},
            {101,"You still need 1 Golden Warrant to enter the fort.\nDo you want to pay 120 CPs to buy one?"},
            {102,"You don`t have enough CPs to buy the warrant."},
            {103,"Please clear up your inventory first."},
            {107,"Your inventory is full. Please clear it up and log in again to get 2 [Silver Warrants]."},
            {108,"The current fort will be open after 00:00, Nov. 23. Please be patient."},
            {109,"The current fort will be open after 00:00, Nov. 24. Please be patient."},
            {110,"The current fort will be open after 00:00, Nov. 25. Please be patient."},
            {111,"Congrats! You`ve taken the fort! Go claim your rewards on the interface!"},
            {112,"You received 2 Silver Warrants!"},
            {113,"The current fort is under cooldown. Please be patient."},
            {116,"Your inventory is almost full. Please clear it up and log in again to get the fort taken rewards."},
        };

        static BattleWave W(uint m, int n, int text) { return new BattleWave(m, n, text > 0 ? K(text) : null); }
        static Fort F(int no, bool big, int secs, int start, int win, int fail, params BattleWave[] waves)
        {
            var f = new Fort { No = no, Big = big, Seconds = secs, Start = start > 0 ? K(start) : null, Win = K(win).Replace("%s", no.ToString(CultureInfo.InvariantCulture)), Fail = K(fail) };
            f.Waves.AddRange(waves);
            if (no == 1) { f.Coins = 10; f.Feathers = 5; }
            else if (big) { f.Points = 60; f.Coins = 40; f.Feathers = 20; }
            else { f.Points = 15; f.Coins = 10; f.Feathers = 5; }
            return f;
        }
        /// <summary>The 23 forts: official texts in order; wave sizes / time limits not in the texts are DERIVED.</summary>
        public static readonly Fort[] Forts =
        {
            null,
            F(1, false, 200, 1, 2, 3, W(JealousImp, 12, 0)),
            F(2, false, 200, 0, 7, 3, W(JealousImp, 15, 4), W(JealousPhantom, 1, 5), W(JealousImp, 15, 6)),
            F(3, false, 200, 0, 11, 3, W(JealousImp, 15, 8), W(JealousPhantom, 1, 9), W(JealousImp, 15, 10)),
            F(4, true, 300, 12, 14, 15, W(Jealous5thBro, 1, 0), W(BroClone5, 2, 13)),
            F(5, false, 200, 0, 17, 18, W(GluttonousRat, 1, 16), W(GluttonousRat, 6, 0)),
            F(6, false, 200, 19, 25, 18, W(GluttonousRat, 2, 20), W(GluttonousRat, 4, 21), W(GluttonousRat, 6, 22), W(GluttonousRat, 10, 23)),
            F(7, false, 200, 26, 32, 18, W(GluttonousSnake, 2, 27), W(GluttonousSnake, 5, 28), W(GluttonousSnake, 8, 29), W(GluttonousSnake, 12, 30)),
            F(8, true, 300, 33, 35, 36, W(Glutton4thSis, 1, 0), W(GluttonousSnake, 10, 34)),
            F(9, false, 200, 37, 40, 39, W(GreedyImp, 15, 0), W(BigGreedyImp, 3, 38)),
            F(10, false, 200, 37, 40, 39, W(GreedyImp, 15, 0), W(BigGreedyImp, 4, 38)),
            F(11, false, 200, 37, 40, 39, W(GreedyImp, 18, 0), W(BigGreedyImp, 5, 38)),
            F(12, true, 300, 41, 43, 44, W(Greedy3rdBro, 1, 0), W(GreedyImp, 12, 42)),
            F(13, false, 200, 45, 47, 46, W(RagingBull, 15, 0)),
            F(14, false, 200, 45, 47, 46, W(RagingBull, 18, 0)),
            F(15, false, 200, 48, 47, 49, W(RagingBull, 10, 0), W(RagingBull, 10, 0)),
            F(16, false, 200, 48, 47, 49, W(RagingBull, 12, 0), W(RagingBull, 12, 0)),
            F(17, true, 300, 54, 56, 72, W(SisClone2, 2, 0), W(Furious2ndSis, 1, 55)),
            F(18, false, 500, 57, 59, 60, W(ArrogantImp, 25, 0)),
            F(19, false, 500, 57, 59, 60, W(ArrogantImp, 20, 0), W(BigGreedyImp, 1, 58)),
            F(20, false, 500, 61, 62, 63, W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0),
                                          W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 0)),
            F(21, false, 500, 64, 66, 69, W(ArrogantImp, 20, 0), W(ArrogantImp, 15, 67)),
            F(22, false, 500, 64, 66, 69, W(ArrogantImp, 20, 0), W(ArrogantImp, 20, 67)),
            F(23, true, 500, 70, 73, 72, W(MagicArray, 1, 0), W(MagicArray, 1, 0), W(MagicArray, 1, 0), W(MagicArray, 1, 0), W(MagicArray, 1, 0), W(ArrogantBigBro, 1, 71)),
        };
        /// <summary>Random forts 1-4 (btn_random_1..4): DERIVED kill versions of the official mini-games, official texts + boxes.</summary>
        public static readonly Fort[] RandomForts =
        {
            null,
            new Fort { No = 101, Seconds = 300, Start = K(74), Win = K(76), Fail = K(77), Box = WarblerBox, Waves = { W(WarblerTroop, 15, 0), W(WarblerLeader, 1, 75) } },
            new Fort { No = 102, Seconds = 300, Start = K(78), Win = K(80), Fail = K(69), Box = GoblinBox, Waves = { W(ChaosGoblin, 15, 0), W(ChaosGoblin, 10, 0) } },
            new Fort { No = 103, Seconds = 300, Start = K(81), Win = K(87), Fail = K(88), Box = TreasureBox, Waves = { W(TreasurePuppet, 15, 0) } },
            new Fort { No = 104, Seconds = 300, Start = K(89), Win = K(93), Fail = K(69), Box = GratefulBox, Waves = { W(WanderingSpirit, 1, 0), W(WanderingSpirit, 1, 0) } },
        };

        // ------------------------------------------------------------------ config (derived values)
        public sealed class Config
        {
            public int MinLevel = 80;// official tThanksNPC2022_DelText[NoLev] "Please level up to 80 first."
            public uint BattleMap = 12660; public ushort BattleX = 221, BattleY = 245; public int Spread = 9;
            public int SilverEnterCps = 30, GoldenEnterCps = 120, SilverSweepCps = 15, GoldenSweepCps = 60;// official texts
            public int BigCooldownSeconds = 1800;// derived
            public int AutoFightPercent = 92;// official "8% off"
            public int DailySilver = 2;// official Msg112
            public int DrawCost1 = 20, DrawCost10 = 200;// client test data (1304-3) - derived
            public int[] DrawMilestones = { 10, 30, 50, 80, 120 };// official Dialog_Info["1304-3-other"].draw_time
            public int[] StageThresholds = { 500, 1500, 2500, 5000, 10000, 30000, 50000, 77777 };// official luaUiRes_EN[1303].Rank.Total
            public int StageRepeatStep = 2000, StageRepeatMax = 500;// official Total[9] / [91]
            public List<List<HItem>> StageItems = new List<List<HItem>>();
            public List<ShopGood> Shop = new List<ShopGood>();
            public List<HKit.Prize> Depot = new List<HKit.Prize>();
            public Dictionary<uint, List<HKit.Prize>> BoxTables = new Dictionary<uint, List<HKit.Prize>>();
            public Dictionary<uint, int> BoxRolls = new Dictionary<uint, int>();
            public Dictionary<uint, List<HItem>> BoxFixed = new Dictionary<uint, List<HItem>>();// official item tips (client itemtype)
            public Dictionary<uint, int> BoxChi = new Dictionary<uint, int>();
        }
        public sealed class ShopGood { public HItem Item; public int Cost; public int Limit; public int Target; }

        static Config _cfg; static string _stamp;
        public static string CfgFile { get { return Path.Combine(HKit.DbDir(Festival), "thanksgiving2022.json"); } }
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
                    try { MyConsole.WriteLine("[Thanksgiving2022] thanksgiving2022.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = Defaults();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static List<HItem> Items(params string[] s) { return s.Select(HItem.Parse).ToList(); }
        static HKit.Prize P(string s, int w, bool rare = false) { return new HKit.Prize { Item = HItem.Parse(s), Weight = w, Rare = rare }; }
        public static Config Defaults()
        {
            var c = new Config();
            // official Dialog_Info["1303-rank-item"].Total[1..9]
            string[][] st =
            {
                new[] { "730004 0 1 3", "3346153 0 2" }, new[] { "3322762 0 2 3", "3346153 0 4" }, new[] { "3009002 0 1", "3346153 0 5" },
                new[] { "3009101 0 1", "3346153 0 5" }, new[] { "3325989 0 1", "3346153 0 5" }, new[] { "3342255 0 6", "3346153 0 6" },
                new[] { "3342255 0 8", "3346153 0 7" }, new[] { "3342255 0 10", "3346153 0 8" }, new[] { "3009103 0 1", "3346153 0 1" },
            };
            foreach (var s in st) c.StageItems.Add(s.Select(HItem.Parse).ToList());
            // official Dialog_Info["1304-1-other"].shopData (cost in Turkey Feathers); limit / puzzle target DERIVED
            c.Shop.Add(new ShopGood { Item = HItem.Parse("3314207 0 1"), Cost = 120, Limit = 20, Target = 4 });
            c.Shop.Add(new ShopGood { Item = HItem.Parse("3009001 0 1"), Cost = 4, Limit = 200, Target = 4 });
            c.Shop.Add(new ShopGood { Item = HItem.Parse("4060001 0 1 3"), Cost = 2, Limit = 500, Target = 4 });
            c.Shop.Add(new ShopGood { Item = HItem.Parse("3328247 0 1"), Cost = 24, Limit = 50, Target = 4 });
            c.Shop.Add(new ShopGood { Item = HItem.Parse("730001 0 1 3"), Cost = 44, Limit = 30, Target = 6 });
            // Military Depot pool = official preview Dialog_Info["1304-4-other"] without the 2023 Tribe currencies; weights DERIVED
            c.Depot.Add(P("3321077 0 5 3", 140)); c.Depot.Add(P("3009102 0 1", 120)); c.Depot.Add(P("3342773 0 3", 140));
            c.Depot.Add(P("3342771 0 2", 90)); c.Depot.Add(P("3314207 0 1 3", 110)); c.Depot.Add(P("3009103 0 1", 70));
            c.Depot.Add(P("3314248 0 1 3", 90)); c.Depot.Add(P("3345048 0 1", 40)); c.Depot.Add(P("3342775 0 1", 25));
            c.Depot.Add(P("3342776 0 1 3", 5, true));
            // box contents = OFFICIAL item tips (client itemtype descriptions); timed items (5-day / 2-day) are given as
            // their normal bound item (this server has no expiring stones) - DERIVED.
            c.BoxFixed[SmallFirstBox] = Items("730002 0 1 3", "3328247 0 20");                  // 1 5-day +2 Stone (B), 20 Awakening Spirits
            c.BoxFixed[BigFirstBox] = Items("3009001 0 2 3", "4060001 0 10 3", "3339557 0 1 3"); // 2 2-day Bright Star Stones, 10 Universal Rune Essences (B), 1 Hunter Puzzle (B)
            c.BoxFixed[WarblerBox] = Items("730003 0 1 3", "3009001 0 2 3", "3321077 0 3 3");   // 1 5-day +3 Stone (B), 2 2-day Bright Star Stones, 3 Leopard Chi Liquor (B)
            c.BoxFixed[GoblinBox] = Items("4060001 0 20 3", "3003126 0 3 3", "3314212 0 1 3");  // 20 Universal Rune Essences (B), 3 Senior Training Pills (B), 1 Ginseng Essence (B)
            c.BoxFixed[TreasureBox] = Items("3008221 0 2", "730003 0 2 3");                     // 2 Olive Jades, 2 5-day +3 Stones (B)
            c.BoxFixed[GratefulBox] = Items("3009102 0 2", "4060001 0 6 3");                    // 2 Moon Fruits, 6 Universal Rune Essences (B) (+ 300 Chi Points)
            c.BoxChi[GratefulBox] = 300;
            return c;
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.MinLevel = HKit.Int(d, "minLevel", c.MinLevel);
            var bm = FL.Obj(FL.Val(d, "battleMap"));
            if (bm != null) { c.BattleMap = (uint)HKit.Int(bm, "map", (int)c.BattleMap); c.BattleX = (ushort)HKit.Int(bm, "x", c.BattleX); c.BattleY = (ushort)HKit.Int(bm, "y", c.BattleY); c.Spread = HKit.Int(bm, "spread", c.Spread); }
            c.BigCooldownSeconds = Math.Max(0, HKit.Int(d, "bigFortCooldownSeconds", c.BigCooldownSeconds));
            c.AutoFightPercent = Math.Max(1, Math.Min(100, HKit.Int(d, "autoFightPercent", c.AutoFightPercent)));
            c.DrawCost1 = Math.Max(1, HKit.Int(d, "depotCost1", c.DrawCost1));
            c.DrawCost10 = Math.Max(1, HKit.Int(d, "depotCost10", c.DrawCost10));
            var shop = FL.Arr(FL.Val(d, "store"));
            if (shop != null && shop.Count > 0)
            {
                var l = new List<ShopGood>();
                foreach (var o in shop.Select(FL.Obj).Where(o => o != null))
                {
                    var it = HKit.Items(new List<object> { FL.Val(o, "item") });
                    if (it.Count == 1) l.Add(new ShopGood { Item = it[0], Cost = Math.Max(1, HKit.Int(o, "cost", 1)), Limit = Math.Max(1, HKit.Int(o, "limit", 1)), Target = Math.Max(1, HKit.Int(o, "puzzleAfter", 4)) });
                }
                if (l.Count > 0) c.Shop = l.Take(5).ToList();
            }
            var depot = Pool(FL.Val(d, "depot")); if (depot != null) c.Depot = depot;
            var boxes = FL.Obj(FL.Val(d, "boxes"));
            if (boxes != null)
                foreach (var kv in boxes)
                {
                    uint id; if (!uint.TryParse(kv.Key, out id)) continue;
                    var bo = FL.Obj(kv.Value); if (bo == null) continue;
                    var fx = HKit.Items(FL.Val(bo, "fixed"));
                    if (fx.Count > 0) { c.BoxFixed[id] = fx; c.BoxTables.Remove(id); }
                    var t = Pool(FL.Val(bo, "table"));
                    if (t != null) { c.BoxTables[id] = t; c.BoxRolls[id] = Math.Max(1, Math.Min(10, HKit.Int(bo, "rolls", 1))); c.BoxFixed.Remove(id); }
                    if (FL.Val(bo, "chi") != null) c.BoxChi[id] = Math.Max(0, HKit.Int(bo, "chi", 0));
                }
        }
        public static List<HKit.Prize> Pool(object o)
        {
            var a = FL.Arr(o);
            if (a == null || a.Count == 0) return null;
            var l = new List<HKit.Prize>();
            foreach (var e in a.Select(FL.Obj).Where(e => e != null))
            {
                var it = HKit.Items(new List<object> { FL.Val(e, "item") });
                if (it.Count == 1) l.Add(new HKit.Prize { Item = it[0], Weight = Math.Max(0, HKit.Int(e, "weight", 1)), Rare = FL.Bool(e, "rare") == true });
            }
            return l.Count > 0 ? l : null;
        }

        // ------------------------------------------------------------------ state
        public static readonly PlayerStore Store = new PlayerStore(Festival);
        public static Festivals.FestivalManager.RunState RunOverride;
        static Festivals.FestivalManager.RunState Run { get { if (RunOverride != null) return RunOverride; try { return Festivals.FestivalManager.Running(Festival); } catch { return null; } } }
        public static bool Running { get { return Run != null; } }
        /// <summary>Progress belongs to one season (a yearly run); a forced test run in another year starts fresh.</summary>
        static string Season { get { var r = Run; return "s" + (r != null ? r.Started.Year : HKit.Now.Year) + ":"; } }
        static string Key(string k) { return Season + k; }
        static long Get(uint uid, string k) { return Store.Get(uid, Key(k)); }

        /// <summary>Which Thanksgiving window a player opened last (the shared depot / rank windows call msg 10319).</summary>
        public static readonly ConcurrentDictionary<uint, string> SharedOwner = new ConcurrentDictionary<uint, string>();

        // ------------------------------------------------------------------ callbacks (msg 10133, and shared 10319 funcs)
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { HKit.Msg(c, t); }

        static bool Mine(string func)
        {
            return func.StartsWith("ThanksGivingPage2022_", StringComparison.Ordinal) || func.StartsWith("Thanksgiving2022JuDian_", StringComparison.Ordinal)
                || func.StartsWith("ThanksgivingPrayer_", StringComparison.Ordinal);
        }
        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse(a[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : -1; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10133).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Mine(func)) return false;
            if (ThanksgivingTribe2023.ThanksgivingTribe2023Ui.OnShared2022(c, func, args)) return true;// the shared exchange window while the 2023 Tribe Fight owns it
            if (HKit.Throttled("tg22", c.Player.UID, 350)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please level up to " + cfg.MinLevel + " first."); return true; }
            SharedOwner[c.Player.UID] = Festival;
            switch (func)
            {
                case "ThanksGivingPage2022_OpenPage":
                case "ThanksGivingPage2022_OpenSign":
                    DailyGift(c, cfg);
                    Open(c, cfg);
                    return true;
                case "Thanksgiving2022JuDian_EnterJuDian": EnterFort(c, cfg, Arg(args, 0)); return true;
                case "Thanksgiving2022JuDian_EnterSJJD": EnterRandom(c, cfg, Arg(args, 0)); return true;
                case "ThanksGivingPage2022_SingleSweep": Sweep(c, cfg, Arg(args, 0)); return true;
                case "ThanksGivingPage2022_Sweep": AutoFight(c, cfg); return true;
                case "ThanksGivingPage2022_FirstReward": FirstReward(c, cfg, Arg(args, 0)); return true;
                case "ThanksGivingPage2022_ClickExchange": OpenStore(c, cfg); return true;
                case "ThanksGivingPage2022_OpenStore": OpenStore(c, cfg); return true;
                case "ThanksGivingPage2022_ExchangeItemNum": StoreMax(c, cfg, Arg(args, 0)); return true;
                case "ThanksGivingPage2022_ExchangeItem": StoreBuy(c, cfg, Arg(args, 0), Arg(args, 1)); return true;
                case "ThanksGivingPage2022_RewardPic": StorePuzzle(c, cfg, Arg(args, 0)); return true;
                case "ThanksgivingPrayer_OpenMain": OpenDepot(c, cfg); return true;
                case "ThanksgivingPrayer_StageReward": OpenDepot(c, cfg); return true;// draw-count rewards are paid automatically
                case "ThanksGivingPage2022_ClikStage": OpenStages(c, cfg); return true;
            }
            return true;
        }

        /// <summary>The msg-10319 funcs of the shared depot / stage windows (called by ThanksgivingTribe2023Ui when this
        /// festival owns the player's window). True = answered.</summary>
        public static bool OnShared(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null) return false;
            string owner;
            if (!Running || !SharedOwner.TryGetValue(c.Player.UID, out owner) || owner != Festival) return false;
            switch (func)
            {
                case "ThanksgivingMainNpc_ChouJiang":
                case "ThanksgivingMainNpc_StageGet":
                case "ThanksgivingMainNpc_OpenRank":
                case "ThanksGivingPage2022_OpenSign":
                case "ThanksGivingPage2022_OpenStore":
                    break;
                default: return false;
            }
            if (HKit.Throttled("tg22", c.Player.UID, 350)) return true;
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please level up to " + cfg.MinLevel + " first."); return true; }
            switch (func)
            {
                case "ThanksgivingMainNpc_ChouJiang": DepotDraw(c, cfg, Arg(args, 0)); break;
                case "ThanksgivingMainNpc_StageGet": StageGet(c, cfg, Arg(args, 0)); break;
                case "ThanksgivingMainNpc_OpenRank":
                    if (args.Length > 0 && (args[0] == "3" || args[0] == "4")) Msg(c, "Cross-server events aren`t available on this server.");
                    else if (args.Length > 0 && (args[0] == "1" || args[0] == "5")) { }// the access window's "Go" also sends OpenSign / OpenStore
                    else Msg(c, "The Turkey Hunt rankings aren`t available on this server.");
                    break;
                case "ThanksGivingPage2022_OpenSign": DailyGift(c, cfg); Open(c, cfg); break;
                case "ThanksGivingPage2022_OpenStore": OpenStore(c, cfg); break;
            }
            return true;
        }

        // ------------------------------------------------------------------ main window
        /// <summary>Big forts opened by date (official: forts 4 and 8 from Nov. 22, then 12 at 00:00 Nov. 23, 17 on Nov. 24,
        /// 23 on Nov. 25 = Locked[2..5]). Returns how many big forts are open (1-5).</summary>
        public static int Unlocked(DateTime now)
        {
            int d = (now.Date - new DateTime(now.Year, 11, 22)).Days;
            if (d < 0 || d > 13) return 5;// started by hand outside the official dates: everything open
            return Math.Min(5, 1 + d + (d >= 3 ? 1 : 0));
        }
        static int BigIndex(int fort) { return Array.IndexOf(BigForts, fort) + 1; }// 1..5, 0 = small
        static long CooldownLeft(uint uid, int fort)
        {
            long until = Get(uid, "cd" + fort);
            long now = Unix(HKit.Now);
            return until > now ? until - now : 0;
        }
        static long Unix(DateTime t) { return (long)(t.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds; }

        public static string MainData(Client.GameClient c, Config cfg)
        {
            uint uid = c.Player.UID;
            int fort = (int)Get(uid, "fort");
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['Points']=").Append(Get(uid, "pts")).Append(',');
            sb.Append("['ItemNum']={").Append(HKit.Count(c, SilverWarrant)).Append(',').Append(HKit.Count(c, GoldenWarrant)).Append("},");
            sb.Append("['CoolingTime']={").Append(string.Join(",", BigForts.Select(f => CooldownLeft(uid, f).ToString(CultureInfo.InvariantCulture)))).Append("},");
            sb.Append("['Monster']=").Append(Math.Min(24, fort + 1)).Append(",['Unlock']=").Append(Math.Min(4, Unlocked(HKit.Now)));
            sb.Append(",['AutoFight']=").Append(AutoFightPrice(cfg, fort));
            return sb.Append("}}").ToString();
        }
        public static string UpdateData(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            int fort = (int)Get(uid, "fort");
            var rewards = Enumerable.Range(1, Math.Min(23, fort)).Where(n => Get(uid, "first" + n) == 0).ToList();
            var sb = new StringBuilder("{['Code']=200,['Data']={['MonsterReward']={");
            sb.Append(string.Join(",", rewards)).Append("},['RandomPlace']={");
            sb.Append(string.Join(",", Enumerable.Range(1, 4).Select(k => RandomOpen(uid, k) ? "1" : "0")));
            return sb.Append("}}}").ToString();
        }
        static void Open(Client.GameClient c, Config cfg)
        {
            DeliverPending(c);
            Lua(c, "DlgGlobalThanksgivingMain_BindData|" + MainData(c, cfg));
            Lua(c, "DlgGlobalThanksgivingMain_UpdateData|" + UpdateData(c));
        }
        static int AutoFightPrice(Config cfg, int fort)
        {
            int sum = 0;
            for (int n = 1; n <= Math.Min(23, fort); n++) sum += BigIndex(n) > 0 ? cfg.GoldenSweepCps : cfg.SilverSweepCps;
            return sum * cfg.AutoFightPercent / 100;
        }

        /// <summary>2 Silver Warrants once a day (official login gift, Msg112 / Msg107).</summary>
        static void DailyGift(Client.GameClient c, Config cfg)
        {
            if (cfg.DailySilver <= 0) return;
            uint uid = c.Player.UID;
            string k = Key("sign" + HKit.Today);
            if (Store.Get(uid, k) != 0) return;
            var give = new List<HItem> { new HItem(SilverWarrant, cfg.DailySilver, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, K(107)); return; }
            if (!Store.Claim(uid, k)) return;
            string got = HKit.Give(c, give);
            HKit.Log("DAILY", Festival, c, got);
            Msg(c, K(112));
        }

        // ------------------------------------------------------------------ forts
        /// <summary>Fort rewards: points to the store, coins + feathers pending in the store and paid out at once (when the
        /// bag is full they wait for the next window open, official Msg116).</summary>
        static void Reward(Client.GameClient c, Fort f, int times, string why)
        {
            uint uid = c.Player.UID;
            Store.Tx(uid, m =>
            {
                Inc(m, Key("pts"), f.Points * times);
                Inc(m, Key("pc"), f.Coins * times);
                Inc(m, Key("pf"), f.Feathers * times);
                return 0;
            });
            HKit.Log("FORT", Festival, c, why + " fort " + f.No + " x" + times + " +" + f.Points * times + " pts (+" + f.Coins * times + " coins, +" + f.Feathers * times + " feathers pending)");
            DeliverPending(c);
        }
        static void Inc(Dictionary<string, long> m, string k, long d) { long v; m.TryGetValue(k, out v); m[k] = v + d; }
        static void DeliverPending(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            long coins = Get(uid, "pc"), feathers = Get(uid, "pf");
            var boxes = new[] { WarblerBox, GoblinBox, TreasureBox, GratefulBox }.Where(b => Get(uid, "pbox" + b) > 0).ToList();
            if (coins <= 0 && feathers <= 0 && boxes.Count == 0) return;
            var give = new List<HItem>();
            if (coins > 0) give.Add(new HItem(Coin, (int)Math.Min(coins, 100000), true));
            if (feathers > 0) give.Add(new HItem(Feather, (int)Math.Min(feathers, 100000), true));
            foreach (var b in boxes) give.Add(new HItem(b, (int)Math.Min(100, Get(uid, "pbox" + b)), true));
            if (!HKit.HasRoom(c, give)) { Msg(c, K(116)); return; }
            // take the pending amounts out of the store first (saved), then hand them out
            bool ok = Store.Tx(uid, m =>
            {
                foreach (var g in give)
                {
                    string k = g.Id == Coin ? Key("pc") : g.Id == Feather ? Key("pf") : Key("pbox" + g.Id);
                    long v; m.TryGetValue(k, out v);
                    if (v < g.Count) return false;
                }
                foreach (var g in give)
                {
                    string k = g.Id == Coin ? Key("pc") : g.Id == Feather ? Key("pf") : Key("pbox" + g.Id);
                    m[k] = m[k] - g.Count;
                }
                return true;
            });
            if (!ok) return;
            string got = HKit.Give(c, give);
            HKit.Log("FORT-PAY", Festival, c, got);
        }

        static string Locked(int fort, DateTime now)
        {
            int bi = BigIndex(fort);
            if (bi <= 1) return null;
            if (bi <= Unlocked(now)) return null;
            return bi == 2 ? K(108) : bi == 3 ? K(109) : K(110);
        }
        /// <summary>Pays the warrant (or its CP price after a confirming second click). Null = paid.</summary>
        static string PayWarrant(Client.GameClient c, bool big, int cps, string confirmToken, string askText)
        {
            uint w = big ? GoldenWarrant : SilverWarrant;
            if (HKit.Count(c, w) >= 1) return HKit.Take(c, w, 1) ? null : "Please try again.";
            if (!HKit.Confirmed(c.Player.UID, confirmToken)) return askText + "\n(Click again within 15 seconds to pay " + cps + " CPs.)";
            if (HKit.Cps(c) < cps) return K(102);
            return HKit.TakeCps(c, (uint)cps) ? null : K(102);
        }

        static void EnterFort(Client.GameClient c, Config cfg, int n)
        {
            if (n < 1 || n > 23) return;
            uint uid = c.Player.UID;
            int fort = (int)Get(uid, "fort");
            if (n <= fort) { Msg(c, "You`ve already taken this fort. Use Mop-up to collect its rewards again."); return; }
            if (n > fort + 1) { Msg(c, K(99)); return; }
            var f = Forts[n];
            string lk = Locked(n, HKit.Now); if (lk != null) { Msg(c, lk); return; }
            if (f.Big && CooldownLeft(uid, n) > 0) { Msg(c, K(113)); return; }
            string why = HolidayBattles.CanStart(c); if (why != null) { Msg(c, why); return; }
            if (!HolidayBattles.CityMaps.Contains(c.Player.Map)) { Msg(c, "Please go to a city first, then enter the fort."); return; }
            string pay = PayWarrant(c, f.Big, f.Big ? cfg.GoldenEnterCps : cfg.SilverEnterCps, "tg22enter" + n, f.Big ? K(101) : K(100));
            if (pay != null) { Msg(c, pay); return; }
            HKit.Log("FORT-ENTER", Festival, c, "fort " + n);
            string err = HolidayBattles.Start(c, Spec(cfg, f), n, (run, ok) => FortDone(run, ok, f));
            if (err != null) { Msg(c, err); HKit.Log("FORT-ENTER-FAIL", Festival, c, err); }
        }
        static BattleSpec Spec(Config cfg, Fort f)
        {
            return new BattleSpec { Festival = Festival, Name = "Fort " + f.No, Map = cfg.BattleMap, X = cfg.BattleX, Y = cfg.BattleY, Spread = cfg.Spread, Seconds = f.Seconds, StartText = f.Start, FailText = f.Fail, Waves = f.Waves };
        }
        static void FortDone(BattleRun run, bool ok, Fort f)
        {
            Client.GameClient c = HolidayBattles.World.Online(run.Owner);
            if (!ok) { HKit.Log("FORT-FAIL", Festival, c, "fort " + f.No); return; }
            if (c == null || c.Player == null) return;
            // the capture is recorded first (only the next fort counts), then the rewards
            bool first = Store.Tx(run.Owner, m =>
            {
                long cur; m.TryGetValue(Key("fort"), out cur);
                if (f.No != cur + 1) return false;
                m[Key("fort")] = f.No;
                if (f.Big) m[Key("cd" + f.No)] = Unix(HKit.Now) + Cfg.BigCooldownSeconds;
                return true;
            });
            if (!first) return;
            Reward(c, f, 1, "capture");
            HKit.Center(c, f.Win);
            Msg(c, K(111));
        }

        static bool RandomOpen(uint uid, int k)
        {
            if (k < 1 || k > 4) return false;
            return Get(uid, "fort") >= BigForts[k - 1] && Get(uid, "rnd" + k + "_" + HKit.Today) == 0;
        }
        static void EnterRandom(Client.GameClient c, Config cfg, int k)
        {
            uint uid = c.Player.UID;
            if (!RandomOpen(uid, k)) { Msg(c, "This random fort isn`t available right now."); return; }
            string why = HolidayBattles.CanStart(c); if (why != null) { Msg(c, why); return; }
            if (!HolidayBattles.CityMaps.Contains(c.Player.Map)) { Msg(c, "Please go to a city first, then enter the fort."); return; }
            string dayKey = Key("rnd" + k + "_" + HKit.Today);
            if (!Store.Claim(uid, dayKey)) return;// once a day (DERIVED), claimed before entering
            var f = RandomForts[k];
            HKit.Log("RANDOM-ENTER", Festival, c, "random " + k);
            string err = HolidayBattles.Start(c, Spec(cfg, f), k, (run, ok) =>
            {
                var cc = HolidayBattles.World.Online(run.Owner);
                if (!ok || cc == null) { HKit.Log("RANDOM-FAIL", Festival, cc, "random " + k); return; }
                Store.Add(run.Owner, Key("pbox" + f.Box), 1);// pending first (saved), then paid out
                HKit.Center(cc, f.Win);
                DeliverPending(cc);
            });
            if (err != null) { Msg(c, err); Store.Set(uid, dayKey, 0); }
        }

        static void Sweep(Client.GameClient c, Config cfg, int n)
        {
            uint uid = c.Player.UID;
            int fort = (int)Get(uid, "fort");
            if (fort < 1) { Msg(c, "Please go capture Fort 1 first. After capturing, the mop-up will be available."); return; }
            if (n < 1 || n > fort) { Msg(c, "You haven`t captured this fort yet, so it can`t be mopped up."); return; }
            var f = Forts[n];
            if (f.Big && CooldownLeft(uid, n) > 0) { Msg(c, "Failed to mop up. The cooldown time is not over yet."); return; }
            var give = new List<HItem> { new HItem(Coin, f.Coins), new HItem(Feather, f.Feathers) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            string ask = f.Big ? "You still need 1 Golden Warrant to mop up this fort." : "You still need 1 Silver Warrant to mop up this fort.";
            string pay = PayWarrant(c, f.Big, f.Big ? cfg.GoldenSweepCps : cfg.SilverSweepCps, "tg22sweep" + n, ask);
            if (pay != null) { Msg(c, pay); return; }
            if (f.Big) Store.Set(uid, Key("cd" + n), Unix(HKit.Now) + cfg.BigCooldownSeconds);
            Reward(c, f, 1, "mop-up");
            Msg(c, "You mopped up Fort " + n + " and received " + (f.Points > 0 ? f.Points + " Thanksgiving Points, " : "") + f.Coins + " Thanksgiving Coins and " + f.Feathers + " Turkey Feathers.");
            Open(c, cfg);
        }
        static void AutoFight(Client.GameClient c, Config cfg)
        {
            uint uid = c.Player.UID;
            int fort = (int)Math.Min(23, Get(uid, "fort"));
            if (fort < 1) { Msg(c, "Please go capture Fort 1 first. After capturing, the mop-up will be available."); return; }
            int price = AutoFightPrice(cfg, fort);
            int coins = 0, feathers = 0;
            for (int n = 1; n <= fort; n++) { coins += Forts[n].Coins; feathers += Forts[n].Feathers; }
            var give = new List<HItem> { new HItem(Coin, coins), new HItem(Feather, feathers) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            if (!HKit.Confirmed(uid, "tg22auto" + fort + ":" + price)) { Msg(c, "Are you sure you want to pay " + price + " CPs to mop up all captured forts?\n(Click again within 15 seconds to confirm.)"); return; }
            if (HKit.Cps(c) < price || !HKit.TakeCps(c, (uint)price)) { Msg(c, "You don`t have enough CPs to mop up."); return; }
            for (int n = 1; n <= fort; n++) Reward(c, Forts[n], 1, "mop-up-all(" + price + "cps)");
            Msg(c, "You mopped up " + fort + " fort(s).");
            Open(c, cfg);
        }
        static void FirstReward(Client.GameClient c, Config cfg, int n)
        {
            uint uid = c.Player.UID;
            if (n < 1 || n > 23 || n > Get(uid, "fort")) { Msg(c, "You haven`t captured this fort yet. Capture it first to claim the First Conquest Reward."); return; }
            var give = new List<HItem> { new HItem(Forts[n].Big ? BigFirstBox : SmallFirstBox, 1, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            if (!Store.Claim(uid, Key("first" + n))) { Msg(c, "You`ve already claimed this reward."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("FIRST", Festival, c, "fort " + n + " -> " + got);
            Lua(c, "DlgGlobalThanksgivingReward_BindData|{['Code']=200,['Data']={['ItemInfo']=" + HKit.LuaItems(give) + "}}");
            Open(c, cfg);
        }

        // ------------------------------------------------------------------ Hunter Supply Store (Turkey Feathers)
        static string GoodLua(uint uid, ShopGood g, int i)
        {
            long bought = Get(uid, "shop" + i), got = Get(uid, "shopr" + i);
            return "'" + g.Item.Lua() + "," + g.Cost + " " + Math.Max(0, g.Limit - bought) + " " + Math.Min(bought, g.Target) + " " + g.Target + (got != 0 ? " 1" : "") + "'";
        }
        static string Money(Client.GameClient c, uint item) { return "'" + item + " 0 1," + HKit.Count(c, item) + "'"; }
        static void OpenStore(Client.GameClient c, Config cfg)
        {
            uint uid = c.Player.UID;
            var goods = cfg.Shop.Select((g, i) => GoodLua(uid, g, i + 1));
            Lua(c, "ThanksgivingExchangeShop_OpenViewDatas|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, Feather) + ",['GoodItems']={" + string.Join(",", goods) + "},['RewardItem']='" + Puzzle + " 0 1'}}");
        }
        static void StoreMax(Client.GameClient c, Config cfg, int i)
        {
            if (i < 1 || i > cfg.Shop.Count) return;
            var g = cfg.Shop[i - 1];
            long left = Math.Max(0, g.Limit - Get(c.Player.UID, "shop" + i));
            uint have = HKit.Count(c, Feather);
            long max = Math.Min(left, have / (uint)g.Cost);
            Lua(c, "DlgGThanksGivingExchangeConfirmUi_ReturnMaxNum|{['Code']=200,['Data']={['Maxnum']=" + Math.Max(1, max) + ",['Coinnum']=" + have + "}}");
        }
        static void StoreBuy(Client.GameClient c, Config cfg, int i, int num)
        {
            if (i < 1 || i > cfg.Shop.Count || num < 1 || num > 9999) return;
            uint uid = c.Player.UID;
            var g = cfg.Shop[i - 1];
            long left = g.Limit - Get(uid, "shop" + i);
            if (num > left) { Msg(c, "You can exchange this item " + Math.Max(0, left) + " more time(s)."); return; }
            long cost = (long)g.Cost * num;
            if (HKit.Count(c, Feather) < cost) { Msg(c, "Failed to exchange. You don`t have enough Turkey Feathers."); return; }
            var give = new List<HItem> { new HItem(g.Item.Id, g.Item.Count * num, g.Item.Bound) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            bool ok = Store.Tx(uid, m => { long b; m.TryGetValue(Key("shop" + i), out b); if (b + num > g.Limit) return false; m[Key("shop" + i)] = b + num; return true; });
            if (!ok) return;
            if (!HKit.Take(c, Feather, (uint)cost)) { Store.Add(uid, Key("shop" + i), -num); return; }
            string got = HKit.Give(c, give);
            HKit.Log("STORE", Festival, c, "paid " + cost + " feathers -> " + got);
            Lua(c, "ThanksgivingExchangeShop_ExchangeResultReturn|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, Feather) + ",['GoodIndex']=" + i + ",['GoodItem']=" + GoodLua(uid, g, i) + "}}");
            Msg(c, "You received " + got + ".");
        }
        static void StorePuzzle(Client.GameClient c, Config cfg, int i)
        {
            if (i < 1 || i > cfg.Shop.Count) return;
            uint uid = c.Player.UID;
            var g = cfg.Shop[i - 1];
            if (Get(uid, "shop" + i) < g.Target) { Msg(c, "Exchange this item " + g.Target + " times to claim 1 Hunter Puzzle."); return; }
            var give = new List<HItem> { new HItem(Puzzle, 1, true) };
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return; }
            if (!Store.Claim(uid, Key("shopr" + i))) { Msg(c, "You`ve already claimed this reward."); return; }
            string got = HKit.Give(c, give);
            HKit.Log("STORE-PUZZLE", Festival, c, "item " + i + " -> " + got);
            Lua(c, "ThanksgivingExchangeShop_GetRewardReturn|{['Code']=200,['Data']={['GoodIndex']=" + i + ",['GoodItem']=" + GoodLua(uid, g, i) + "}}");
        }

        // ------------------------------------------------------------------ Military Depot (Thanksgiving Coins)
        static void OpenDepot(Client.GameClient c, Config cfg)
        {
            Lua(c, "GThanksGivingDraw_OpenViewDatas|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, Coin) + ",['DrawCost']={" + cfg.DrawCost1 + "," + cfg.DrawCost10 + "}}}");
        }
        static void DepotDraw(Client.GameClient c, Config cfg, int times)
        {
            if (times != 1 && times != 10) return;
            uint uid = c.Player.UID;
            int cost = times == 1 ? cfg.DrawCost1 : cfg.DrawCost10;
            if (HKit.Count(c, Coin) < cost) { Msg(c, "You don`t have enough Thanksgiving Coins. Go earn some!"); return; }
            var prizes = new List<HKit.Prize>();
            for (int i = 0; i < times; i++) { var p = HKit.Draw(cfg.Depot); if (p != null) prizes.Add(p); }
            var give = prizes.Select(p => p.Item).ToList();
            long before = Get(uid, "draws"), after = before + times;
            var miles = cfg.DrawMilestones.Where(m => m > before && m <= after).ToList();
            var extra = miles.Select(m => new HItem(Puzzle, 1, true)).ToList();
            if (!HKit.HasRoom(c, give.Concat(extra))) { Msg(c, "Your inventory is full. Please clear it up first."); return; }
            if (!HKit.Take(c, Coin, (uint)cost)) return;
            Store.Add(uid, Key("draws"), times);
            string got = HKit.Give(c, give);
            if (extra.Count > 0) got += ", " + HKit.Give(c, extra) + " (draw " + string.Join("/", miles) + ")";
            HKit.Log("DEPOT", Festival, c, "x" + times + " paid " + cost + " coins -> " + got);
            var rnd = new List<int>();
            for (int i = 0; i < times; i++) rnd.Add(1 + HKit.Roll(10));
            string box = times == 1 ? rnd[0].ToString(CultureInfo.InvariantCulture) : "{" + string.Join(",", rnd.Distinct()) + "}";
            Lua(c, "GThanksGivingDraw_DrawResultReturn|{['Code']=200,['Data']={['HaveMoney']=" + Money(c, Coin) + ",['Type']=" + times + ",['BoxId']=" + box + ",['Rewards']=" + HKit.LuaItems(give.Concat(extra)) + "}}");
        }

        // ------------------------------------------------------------------ Thanksgiving Points stage rewards
        static int StagesReached(Config cfg, long pts) { return cfg.StageThresholds.Count(t => pts >= t); }
        static long RepeatEarned(Config cfg, long pts)
        {
            if (cfg.StageThresholds.Length == 0) return 0;
            long top = cfg.StageThresholds[cfg.StageThresholds.Length - 1];
            return pts <= top ? 0 : Math.Min(cfg.StageRepeatMax, (pts - top) / Math.Max(1, cfg.StageRepeatStep));
        }
        public static string StageData(uint uid, Config cfg)
        {
            long pts = Get(uid, "pts");
            int reached = StagesReached(cfg, pts);
            var can = new List<int>(); var no = new List<int>(); var done = new List<int>();
            for (int i = 1; i <= cfg.StageThresholds.Length; i++)
            {
                if (Get(uid, "stage" + i) != 0) done.Add(i);
                else if (i <= reached) can.Add(i); else no.Add(i);
            }
            int rep = cfg.StageThresholds.Length + 1;
            long repLeft = RepeatEarned(cfg, pts) - Get(uid, "stage" + rep);
            if (repLeft > 0) can.Add(rep); else if (Get(uid, "stage" + rep) >= cfg.StageRepeatMax) done.Add(rep); else no.Add(rep);
            return "{['Code']=200,['Data']={['CanClaim']={" + string.Join(",", can) + "},['NoClaim']={" + string.Join(",", no) + "},['Claimed']={" + string.Join(",", done) + "}}}";
        }
        static void OpenStages(Client.GameClient c, Config cfg)
        {
            Lua(c, "DlgGlobalThanksgivingRank_BindData|" + StageData(c.Player.UID, cfg));
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
                if (pts < cfg.StageThresholds[i - 1]) { Msg(c, "You need " + cfg.StageThresholds[i - 1] + " Thanksgiving Points to claim this reward."); return; }
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

        // ------------------------------------------------------------------ boxes / puzzle
        public static void RegisterBoxes()
        {
            foreach (var b in new[] { SmallFirstBox, BigFirstBox, WarblerBox, GoblinBox, TreasureBox, GratefulBox })
            {
                uint box = b;
                HolidayBoxes.Register(box, c =>
                {
                    var cfg = Cfg;
                    List<HItem> give; List<HKit.Prize> t; int rolls, chi;
                    if (cfg.BoxFixed.TryGetValue(box, out give)) give = give.ToList();
                    else if (cfg.BoxTables.TryGetValue(box, out t)) { if (!cfg.BoxRolls.TryGetValue(box, out rolls)) rolls = 1; give = HolidayBoxes.Roll(t, rolls); }
                    else return false;
                    cfg.BoxChi.TryGetValue(box, out chi);
                    return HolidayBoxes.Open(c, Festival, box, give, null, chi);
                });
            }
            HolidayBoxes.Register(Puzzle, UsePuzzle);
        }
        /// <summary>Hunter Puzzle (official tFragments[3339557]): 50 -> Turkey Hunter (Terminator), 25 -> Turkey Hunter (B).</summary>
        static bool UsePuzzle(Client.GameClient c)
        {
            uint have = HKit.Count(c, Puzzle);
            uint need; uint garment;
            if (have >= 50) { need = 50; garment = HunterTerminator; }
            else if (have >= 25) { need = 25; garment = HunterB; }
            else { Msg(c, "Collect 25 puzzles to combine into a 3-star garment, Turkey Hunter (B). Collect 50 puzzles to combine into a 5-star garment, Turkey Hunter (Terminator). You have " + have + "."); return true; }
            var give = new List<HItem> { new HItem(garment, 1, true) };
            if (!HKit.ItemExists(garment)) { Msg(c, "This garment isn`t available on this server."); return true; }
            if (!HKit.HasRoom(c, give)) { Msg(c, "Please clear up your inventory first."); return true; }
            if (!HKit.Take(c, Puzzle, need)) return true;
            string got = HKit.Give(c, give);
            HKit.Log("PUZZLE", Festival, c, need + " puzzles -> " + got);
            Msg(c, "You combined " + need + " Hunter Puzzles into " + got + ".");
            return true;
        }
    }
}
