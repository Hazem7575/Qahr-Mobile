// ============================================================================
//  [NewYear2023 2026-10-14 / COMPLETE 2026-10-15] The official 2023 Spring Festival window (client msg 10170
//  tMsg_Info["SpringFestival2023"], luaui\dialog\dlgnewyear, Dialog_Info 1329, "Happy Year of the Rabbit"), shown while
//  the festival LunarNewYear2023 (quests 5442-5452) runs. Button DlgNewYearBtn = festival.json icon_show / icon_hide.
//
//  Everything the official window offers:
//    lanterns  SpringFestival2023Story_ClickToEnter</N>1..3 -> the 3 spring stories (quest engine, Festivals\LunarNewYear2023)
//    dailies   SpringFestivalDaily2023_DoEvent</N>1..5 (Advlist): Festival Shopping (Grocers 31133-31137 sell the goods,
//              official Shop.dat), New Year Prayer (Nuwa / Emperor Huang / Emperor Yan on the 2023 event map 12019),
//              Troubled Residents (10 Piggish monsters), Bustling New Year (Firework Tubes on 5 maps), New Year Packet
//              (3 silver). Each gives 1 Small Spring Bag once a day (official texts tSpringFestivalDaily2023[Msg]).
//    cards     SpringInterface_PubView / _GetbookPage: Class Cards, 8 classes x 5 cards, unlocked by Spring Greeting Cards
//              3600447 (monster hunting); a duplicate gives 1 Laurel Branch (official tSpringInterface_DelText[MsgBox]).
//    gift      SpringInterface_RewardView / _GetRewardPage: Fluffy Bunny 199885 (3 stories, 5 classes, 8000 points).
//    wishes    SpringFestival2023Main_*: DlgValentinesDayMainUI DlgID 1329 (SpringFestival2023Wish.cs).
//    points    New Year Points = 32 per wish (official DrawInfo), Score of the window.
//  Official data: luaUiRes[1329], Dialog_Info 1329-other, valentinesdayinfo 1329 tables, Server_Key, itemtype texts.
//  Derived (config Infinity[DB]\NewYear2023\springfestival2023.json): wheel weights, stage thresholds, class-card drop
//  chance / daily cap, class reward, NPC spots that the client has no data for. See Festivals\LunarNewYear2023\README_AR.md.
//
//  State per character: Infinity[DB]\NewYear2023\players.json (SpringStore, saved before any grant).
//  Hooks: LuaPush.HandleCallback (one tagged line -> SpringHooks.OnLua), Procesor (SpringHooks.OnNpc / OnItemDialog),
//         MsgItemUsuagePacket.UseItem (SpringHooks.OnUse), MonsterRole.Dead (SpringHooks.OnKill).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using G = Qahr.Mobile.NewYear2023.SpringGive;
using K = Qahr.Mobile.NewYear2023.SpringKit;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.NewYear2023
{
    public static class SpringFestival2023
    {
        public const uint LuaMsg = 10170;

        // ---- official item ids (itemtype)
        public const uint LaurelBranch = 3340340, SpringIngot = 3340356, GrandBag = 3340333, SmallBag = 3600446, GreetingCard = 3600447;
        public const uint FluffyBunny = 199885;
        public static readonly uint[] Goods = { 3340325, 3340326, 3340327, 3340328, 3340329 };// Hazel Nut, Lantern, Firecracker, Cake, Silk
        public static readonly uint[] Grocers = { 31133, 31134, 31135, 31136, 31137 };      // Shop.dat "Spring Festival Store", 1 good each

        public sealed class Line
        {
            public uint First, Last, Map;
            public Line(uint first, uint last, uint map) { First = first; Last = last; Map = map; }
        }
        public sealed class Spot { public uint Map; public ushort X, Y; public Spot(uint m, ushort x, ushort y) { Map = m; X = x; Y = y; } }

        public sealed class Config
        {
            public string Festival = "LunarNewYear2023";
            public int MinLevel = 80;
            public uint HubNpc = 31153;// GiltSwirl: its festival teleports (quests.json npcTeleports) are reused
            public Line[] Lines = { new Line(5442, 5446, 12021), new Line(5447, 5449, 12022), new Line(5450, 5452, 12023) };
            public HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036, 12019, 12021, 12022, 12023, 12024 };
            public int GiftScoreTarget = 8000;   // official "Get 8000 New Year Points in total."
            public int GiftClassTarget = 5;      // official "Activate all cards of 5 classes."
            public int PointsPerWish = 32;       // official DrawInfo
            // derived
            public int CardDropPermille = 30;    // Spring Greeting Card per kill (monster level >= player level - 20)
            public int CardDailyCap = 10;
            public uint ClassRewardItem = GrandBag; public int ClassRewardCount = 1;
            public int[] LaurelGrand = { 270, 500 }, LaurelSmall = { 25, 50 };// official item texts
            public uint[] FireMaps = { 1002, 1000, 1011, 1015, 1020 };// official tSpringFestivalDaily2023 EventMap
            public Spot PrayerArrive = new Spot(12019, 220, 250);
            public Spot PigArrive = new Spot(12021, 335, 265); public Spot PigBox = new Spot(12021, 325, 255); public ushort PigRange = 20;
            public int PigCount = 10, PigMinutes = 15;  // official "10 %s ate the meat on the sly!"
            public uint[] PigTypes = { 998305, 998306, 998307, 998308, 998309 };// Piggish Bird/Snake/Rat/Bat/Gorilla (festival monsters)
            public Spot[] GrocerSpots = { new Spot(1002, 382, 390), new Spot(1002, 384, 392), new Spot(1002, 386, 394), new Spot(1002, 388, 396), new Spot(1002, 390, 398) };
            public uint GrocerMesh = 9920;   // npc.ini NpcType992 DesertCityGrocer (no Spring grocer look in the client)
            public uint PacketSilver = 3;    // official "You gave out 3 Silver"
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "NewYear2023", "springfestival2023.json"); } }
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
                    var d = fi.Exists ? FL.ReadJson(fi.FullName) : null;
                    Read(c, d);
                    SpringFestival2023Wish.ReadConfig(d);
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[NewYear2023] springfestival2023.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }
        static int[] Range(object v, int[] def)
        {
            var a = FL.Arr(v);
            if (a == null || a.Count != 2) return def;
            int lo = (int)Math.Min(100000, FL.U(a[0])), hi = (int)Math.Min(100000, FL.U(a[1]));
            return hi >= lo && hi > 0 ? new[] { lo, hi } : def;
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
            var hub = FL.Val(d, "hubNpc"); if (hub != null) c.HubNpc = FL.U(hub);
            var gs = FL.Val(d, "giftScoreTarget"); if (gs != null) c.GiftScoreTarget = (int)Math.Min(1000000, FL.U(gs));
            var gc = FL.Val(d, "giftClassTarget"); if (gc != null) c.GiftClassTarget = (int)Math.Max(1, Math.Min(8, FL.U(gc)));
            var pw = FL.Val(d, "pointsPerWish"); if (pw != null) c.PointsPerWish = (int)Math.Min(10000, FL.U(pw));
            var cd = FL.Val(d, "cardDropPermille"); if (cd != null) c.CardDropPermille = (int)Math.Min(1000, FL.U(cd));
            var cc = FL.Val(d, "cardDailyCap"); if (cc != null) c.CardDailyCap = (int)Math.Min(1000, FL.U(cc));
            var cr = FL.Val(d, "classRewardItem"); if (cr != null && FL.U(cr) != 0) c.ClassRewardItem = FL.U(cr);
            var crn = FL.Val(d, "classRewardCount"); if (crn != null) c.ClassRewardCount = (int)Math.Max(1, Math.Min(100, FL.U(crn)));
            c.LaurelGrand = Range(FL.Val(d, "laurelGrandBag"), c.LaurelGrand);
            c.LaurelSmall = Range(FL.Val(d, "laurelSmallBag"), c.LaurelSmall);
            var pc = FL.Val(d, "piggishCount"); if (pc != null) c.PigCount = (int)Math.Max(1, Math.Min(30, FL.U(pc)));
            var from = FL.Arr(FL.Val(d, "teleportFromMaps"));
            if (from != null) c.TeleportFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
            var lines = FL.Arr(FL.Val(d, "lines"));
            if (lines != null && lines.Count == 3)
            {
                var l = new Line[3];
                for (int i = 0; i < 3; i++)
                {
                    var o = lines[i] as Dictionary<string, object>;
                    if (o == null) return;
                    l[i] = new Line(FL.U(FL.Val(o, "firstQuest")), FL.U(FL.Val(o, "lastQuest")), FL.U(FL.Val(o, "map")));
                    if (l[i].First == 0 || l[i].Last == 0 || l[i].Map == 0) return;
                }
                c.Lines = l;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // ------------------------------------------------------------------ per-character state
        static SpringStore _store;
        public static SpringStore Store { get { return _store ?? (_store = new SpringStore("NewYear2023")); } }
        public static void ResetForTests() { _store = null; _last.Clear(); _lastEnter.Clear(); SpringFestival2023Wish.ResetForTests(); }

        /// <summary>Season (festival run year) and day housekeeping before any read: a new season clears the event
        /// progress (points, wishes, cards, gift), a new day clears the daily quests and the card drop counter.</summary>
        public static void Touch(uint uid)
        {
            int season = K.Now.Year;
            try { var r = Festivals.FestivalManager.Running(Cfg.Festival); if (r != null) season = r.Started.Year; } catch { }
            Store.Roll(uid, "season", season, "s_");
            Store.Roll(uid, "day", K.Today, "d_");
        }

        // ------------------------------------------------------------------ callbacks (msg 10170)
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastEnter = new ConcurrentDictionary<uint, long>();
        static void Msg(Client.GameClient c, string t) { K.Msg(c, t); }
        public static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        // official texts
        public const string TxtOver = "The event is over. I`m looking forward to meeting you next time.";// tSpringInterface_DelText[MsgBox][NoTime]
        public const string TxtLevel = "Please upgrade to level 80 first.";
        public const string TxtSpace = "Please make at least {0} space(s) in your inventory first.";// [NoMoreSpace]
        static readonly string[] EventName = { "Spring Festival Shopping", "New Year Prayer", "Troubled Residents", "Bustling New Year", "New Year Packet" };
        static readonly string[] EventText =
        {
            "Miss Bunny wants to do Spring Festival Shopping. Go to help her!",
            "You must have a good prospect for the new year. Let`s pray for a happy new year!",
            "Villagers are worried since some monsters always come to eat the meat on the sly. Go to help them!",
            "[{0}]`s fireworks are ready! Go to set off fireworks together!",
            "Children in Twin City are celebrating the Spring Festival. Go to give them some red packets!",
        };
        static readonly Dictionary<uint, string> FireMapName = new Dictionary<uint, string> { { 1002, "Wind Plain" }, { 1000, "Desert City" }, { 1011, "Maple Forest" }, { 1015, "Bird Island" }, { 1020, "Love Canyon" } };
        public static readonly string[] PigName = { "Piggish Bird", "Piggish Snake", "Piggish Rat", "Piggish Bat", "Piggish Gorilla" };
        public static readonly string[] PrayerName = { "Nuwa", "Emperor Huang", "Emperor Yan" };

        /// <summary>True = answered (msg 10170).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (func == "SpringInterface_Web") return true;// the magnifier opens an official web page: nothing to do
            if (func.StartsWith("SpringFestival2023Main_", StringComparison.Ordinal)) return SpringFestival2023Wish.OnLua(c, LuaMsg, func, args);
            if (func != "SpringInterface_MainView" && func != "SpringFestival2023Story_ClickToEnter" && !func.StartsWith("SpringInterface_", StringComparison.Ordinal)
                && func != "SpringFestivalDaily2023_DoEvent") return false;
            if (K.Throttled(_last, c.Player.UID, 400)) return true;
            var cfg = Cfg;
            bool running = Running;
            int a0 = 0;
            if (args != null && args.Length > 0) int.TryParse(args[0], out a0);
            switch (func)
            {
                case "SpringInterface_MainView":
                    if (!running) { Lua(c, "DlgNewYearMainUi_MainOpen|{['Code']=101}"); return true; }
                    if (c.Player.Level < cfg.MinLevel) { Lua(c, "DlgNewYearMainUi_MainOpen|{['Code']=102}"); return true; }
                    Lua(c, MainData(c, cfg));
                    return true;
                case "SpringFestival2023Story_ClickToEnter":
                    {
                        if (!running) { Msg(c, TxtOver); return true; }
                        if (a0 < 1 || a0 > 3) return true;
                        if (K.Throttled(_lastEnter, c.Player.UID, 3000)) return true;
                        Enter(c, cfg, a0 - 1);
                        return true;
                    }
                case "SpringInterface_RewardView":
                    if (!running) { Lua(c, "DlgNewYearGiftsUI_MainOpen|{['Code']=401}"); return true; }
                    if (c.Player.Level < cfg.MinLevel) { Lua(c, "DlgNewYearGiftsUI_MainOpen|{['Code']=402}"); return true; }
                    Lua(c, GiftsData(c, cfg));
                    return true;
                case "SpringInterface_GetRewardPage":
                    using (var rec = new ServerSockets.RecycledPacket()) ClaimGift(c, cfg, rec.GetStream());
                    return true;
                case "SpringInterface_PubView":
                    if (!running) { Lua(c, "DlgNewYearPubUI_MainOpen|{['Code']=301}"); return true; }
                    if (c.Player.Level < cfg.MinLevel) { Lua(c, "DlgNewYearPubUI_MainOpen|{['Code']=302}"); return true; }
                    Lua(c, PubData(c, cfg, a0 < 1 || a0 > 8 ? 1 : a0, 0));
                    return true;
                case "SpringInterface_GetbookPage":
                    using (var rec = new ServerSockets.RecycledPacket()) ClaimClass(c, cfg, a0, rec.GetStream());
                    return true;
                case "SpringFestivalDaily2023_DoEvent":
                    using (var rec = new ServerSockets.RecycledPacket()) DoEvent(c, cfg, a0, rec.GetStream());
                    return true;
            }
            return true;// another SpringInterface_* call of this window: nothing to do
        }

        // ------------------------------------------------------------------ data of the windows
        static bool Finished(Client.GameClient c, uint q)
        {
            try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; }
        }
        static bool Taken(Client.GameClient c, uint q)
        {
            try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; }
        }
        public static int StoriesDone(Client.GameClient c, Config cfg) { return cfg.Lines.Count(l => Finished(c, l.Last)); }
        public static long Points(uint uid) { return Store.Get(uid, "s_points"); }
        public static int CardMask(uint uid, int cls) { return (int)(Store.Get(uid, "s_card" + cls) & 31); }
        public static int ClassesDone(uint uid) { int n = 0; for (int i = 1; i <= 8; i++) if (CardMask(uid, i) == 31) n++; return n; }
        static bool ClassClaimable(uint uid, int i) { return CardMask(uid, i) == 31 && (Store.Get(uid, "s_classgot") & (1L << i)) == 0; }
        static bool GiftReady(Client.GameClient c, Config cfg)
        {
            uint u = c.Player.UID;
            return StoriesDone(c, cfg) >= 3 && ClassesDone(u) >= cfg.GiftClassTarget && Points(u) >= cfg.GiftScoreTarget;
        }
        static int DailyState(uint uid, int i) { return (Store.Get(uid, "d_done") & (1L << i)) != 0 ? 2 : 1; }

        public static string MainData(Client.GameClient c, Config cfg, string fn = "DlgNewYearMainUi_MainOpen")
        {
            uint u = c.Player.UID;
            Touch(u);
            var sb = new StringBuilder(fn + "|{['Code']=100,['Data']={['Score']=" + Points(u) + ",['Actlist']={");
            for (int i = 0; i < 3; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['State']=").Append(Finished(c, cfg.Lines[i].Last) ? 3 : 2).Append(",['ActTime']=0,['PlayIndex']=0}");
            }
            sb.Append("},['Advlist']={");
            for (int i = 0; i < 5; i++) { if (i > 0) sb.Append(','); sb.Append("{['State']=").Append(DailyState(u, i)).Append(",['PlayAdvEff']=0}"); }
            sb.Append("},['ItemInfo']='").Append(LaurelBranch).Append(" 0 ").Append(G.Count(c, LaurelBranch)).Append('\'');
            int pub = 0; for (int i = 1; i <= 8; i++) if (ClassClaimable(u, i)) pub = 1;
            int gifs = GiftReady(c, cfg) && Store.Get(u, "s_gift") == 0 ? 1 : 0;
            return sb.Append(",['Pubflag']=").Append(pub).Append(",['Rewardflag']=").Append(SpringFestival2023Wish.StageFlag(u))
                .Append(",['Gifsflag']=").Append(gifs).Append(",['GuideId']=1}}").ToString();
        }
        public static string GiftsData(Client.GameClient c, Config cfg, string fn = "DlgNewYearGiftsUI_MainOpen")
        {
            uint u = c.Player.UID;
            Touch(u);
            return fn + "|{['Code']=400,['Data']={['Tasklist']={{['FinishNum']=" + StoriesDone(c, cfg)
                + ",['TargetNum']=3},{['FinishNum']=" + Math.Min(cfg.GiftClassTarget, ClassesDone(u)) + ",['TargetNum']=" + cfg.GiftClassTarget
                + "},{['FinishNum']=" + Points(u) + ",['TargetNum']=" + cfg.GiftScoreTarget + "}},['IsReceive']=" + (Store.Get(u, "s_gift") != 0 ? 1 : 0) + "}}";
        }
        public static string PubData(Client.GameClient c, Config cfg, int cls, int playIndex, string fn = "DlgNewYearPubUI_MainOpen")
        {
            uint u = c.Player.UID;
            Touch(u);
            int mask = CardMask(u, cls);
            var sb = new StringBuilder(fn + "|{['Code']=300,['Data']={['AtlasIndex']=" + cls + ",['FlagList']={");
            for (int i = 1; i <= 8; i++) { if (i > 1) sb.Append(','); sb.Append(ClassClaimable(u, i) ? 1 : 0); }
            sb.Append("},['Atlaslist']={");
            for (int i = 0; i < 5; i++) { if (i > 0) sb.Append(','); sb.Append((mask & (1 << i)) != 0 ? 1 : 0); }
            return sb.Append("},['ItemInfo']='").Append(cfg.ClassRewardItem).Append(" 0 ").Append(cfg.ClassRewardCount)
                .Append("',['PlayIndex']=").Append(playIndex).Append("}}").ToString();
        }

        // ------------------------------------------------------------------ gift (Fluffy Bunny)
        static void ClaimGift(Client.GameClient c, Config cfg, ServerSockets.Packet stream)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            uint u = c.Player.UID;
            Touch(u);
            if (!GiftReady(c, cfg)) { Msg(c, "You haven`t met all requirements yet."); return; }
            var items = new List<G.GItem> { G.I(FluffyBunny) };
            int slots = G.Slots(items);
            if (!G.HaveSpace(c, slots)) { Msg(c, string.Format(TxtSpace, slots)); return; }
            if (!Store.Update(u, g => g("s_gift") != 0 ? null : new Dictionary<string, long> { { "s_gift", 1 } })) { Msg(c, "You have claimed the reward."); return; }
            G.Give(c, items, stream, "LunarNewYear2023/FluffyBunny");
            Msg(c, "Congrats! You received 1 Fluffy Bunny (Snow).");
            Lua(c, GiftsData(c, cfg, "DlgNewYearGiftsUI_UpdateView"));
        }

        // ------------------------------------------------------------------ Class Cards
        static void ClaimClass(Client.GameClient c, Config cfg, int cls, ServerSockets.Packet stream)
        {
            if (!Running) { Lua(c, "DlgNewYearPubUI_UpdataView|{['Code']=301}"); return; }
            if (c.Player.Level < cfg.MinLevel) { Lua(c, "DlgNewYearPubUI_UpdataView|{['Code']=302}"); return; }
            if (cls < 1 || cls > 8) return;
            uint u = c.Player.UID;
            Touch(u);
            if (CardMask(u, cls) != 31) { Msg(c, "Collect 5 Class Cards to claim it."); return; }
            var items = new List<G.GItem> { G.I(cfg.ClassRewardItem, cfg.ClassRewardCount) };
            int slots = G.Slots(items);
            if (!G.HaveSpace(c, slots)) { Msg(c, string.Format(TxtSpace, slots)); return; }
            if (!Store.Update(u, g => (g("s_classgot") & (1L << cls)) != 0 ? null : new Dictionary<string, long> { { "s_classgot", g("s_classgot") | (1L << cls) } }))
            { Msg(c, "You have claimed the reward."); return; }
            G.Give(c, items, stream, "LunarNewYear2023/ClassCards" + cls);
            Msg(c, "You received " + cfg.ClassRewardCount + " " + G.ItemName(cfg.ClassRewardItem) + ".");
            Lua(c, PubData(c, cfg, cls, 0, "DlgNewYearPubUI_UpdataView"));
        }

        /// <summary>Spring Greeting Card 3600447: "Use to activate 1 Class Card." A card already unlocked gives 1 Laurel Branch.</summary>
        public static void UseGreetingCard(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            var cfg = Cfg;
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            uint u = c.Player.UID;
            Touch(u);
            int cls = K.Rand(1, 8), card = K.Rand(0, 4);
            bool dup = (CardMask(u, cls) & (1 << card)) != 0;
            if (dup && !G.HaveSpace(c, 1)) { Msg(c, string.Format(TxtSpace, 1)); return; }
            if (!G.TakeOne(c, item, stream)) return;
            if (!dup)
            {
                Store.Update(u, g => new Dictionary<string, long> { { "s_card" + cls, g("s_card" + cls) | (1L << card) } });
                Msg(c, "Congrats! You unlocked a new Class Card.");
                Lua(c, PubData(c, cfg, cls, card + 1));
            }
            else
            {
                G.Give(c, new[] { G.I(LaurelBranch) }, stream, "LunarNewYear2023/CardDuplicate");
                Msg(c, "The Class Card has been unlocked and you received 1 Laurel Branch.");
            }
        }

        /// <summary>MonsterRole.Dead: Spring Greeting Cards from monster hunting (daily cap, official "GetFull" text).</summary>
        public static void OnHunt(Client.GameClient c, Game.MsgMonster.MonsterRole mob)
        {
            var cfg = Cfg;
            if (cfg.CardDropPermille <= 0 || !Running || c.Player.Level < cfg.MinLevel) return;
            if (mob.Level + 20 < c.Player.Level) return;
            if (K.Dice() * 1000 >= cfg.CardDropPermille) return;
            uint u = c.Player.UID;
            Touch(u);
            if (!G.HaveSpace(c, 1)) return;
            bool full = false;
            if (!Store.Update(u, g =>
            {
                if (g("d_cards") >= cfg.CardDailyCap) { full = g("d_full") == 0; return full ? new Dictionary<string, long> { { "d_full", 1 } } : null; }
                return new Dictionary<string, long> { { "d_cards", g("d_cards") + 1 } };
            }) || full)
            {
                if (full) Msg(c, "You have got many Spring Greeting Cards through hunting monsters and cannot get more.");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
                G.Give(c, new[] { G.I(GreetingCard) }, rec.GetStream(), "LunarNewYear2023/GreetingCardDrop");
            Msg(c, "You received 1 Spring Greeting Card.");
        }

        // ------------------------------------------------------------------ the 5 daily quests
        static bool GiveSmallBag(Client.GameClient c, ServerSockets.Packet stream, int idx)
        {
            var items = new[] { G.I(SmallBag) };
            if (!G.HaveSpace(c, 1)) { Msg(c, "Your inventory is full. Please make at least 1 space(s) in your inventory first."); return false; }
            bool ok = Store.Update(c.Player.UID, g => (g("d_done") & (1L << idx)) != 0 ? null : new Dictionary<string, long> { { "d_done", g("d_done") | (1L << idx) } });
            if (!ok) { Msg(c, "You have completed the quest. Go to check other quests."); return false; }
            G.Give(c, items, stream, "LunarNewYear2023/Daily" + (idx + 1));
            return true;
        }
        static bool CanTravel(Client.GameClient c, Config cfg)
        {
            var p = c.Player;
            if (!p.Alive || p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map)) { Msg(c, "Please go to Twin City or Market and try again."); return false; }
            return true;
        }
        static void Move(Client.GameClient c, Spot s)
        {
            if (Pool.ServerMaps[s.Map] == null) { Msg(c, "This place isn`t open yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, s.X, s.Y, s.Map, 0); else c.Teleport(s.X, s.Y, s.Map, 0);
        }

        public static void DoEvent(Client.GameClient c, Config cfg, int ev, ServerSockets.Packet stream)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            if (ev < 1 || ev > 5) return;
            uint u = c.Player.UID;
            Touch(u);
            int i = ev - 1;
            if (DailyState(u, i) == 2) { Msg(c, "You have completed the quest. Go to check other quests."); return; }
            switch (ev)
            {
                case 1:
                    {
                        long want = Store.Get(u, "d_shop");
                        if (want < 1 || want > 5)
                        {
                            int pick = K.Rand(1, 5);
                            Store.Set(u, "d_shop", pick);
                            Msg(c, EventText[0]);
                            Msg(c, "Buy 1 [" + G.ItemName(Goods[pick - 1]) + "] from the Grocer next to GiltSwirl in Twin City, then bring it to Miss Bunny here.");
                            return;
                        }
                        uint good = Goods[want - 1];
                        if (G.Count(c, good) < 1) { Msg(c, "Please complete the quest " + EventName[0] + " first: buy 1 [" + G.ItemName(good) + "] from the Grocer next to GiltSwirl in Twin City."); return; }
                        if (!G.HaveSpace(c, 1)) { Msg(c, "Your inventory is full. Please make at least 1 space(s) in your inventory first."); return; }
                        if (!G.Take(c, good, 1, stream)) return;
                        if (GiveSmallBag(c, stream, i)) Msg(c, "Miss Bunny received the goods you bought and gave you 1 Small Spring Bag.");
                        break;
                    }
                case 2:
                    if (!CanTravel(c, cfg)) return;
                    Msg(c, EventText[1]);
                    Move(c, cfg.PrayerArrive);
                    Msg(c, "This is a place for immortals to pray. You might as well join them to pray!");
                    return;
                case 3:
                    {
                        long t = Store.Get(u, "d_pig"), left = Store.Get(u, "d_pigleft");
                        if (t < 1 || t > 5)
                        {
                            t = K.Rand(1, 5); left = cfg.PigCount;
                            long tt = t, ll = left;
                            Store.Update(u, g => new Dictionary<string, long> { { "d_pig", tt }, { "d_pigleft", ll } });
                        }
                        if (left <= 0)// all driven away but the bag was full: pay now
                        {
                            if (GiveSmallBag(c, stream, i)) Msg(c, "Villagers thanked you a lot and gave you 1 Small Spring Bag.");
                            return;
                        }
                        if (!CanTravel(c, cfg)) return;
                        Msg(c, EventText[2]);
                        Move(c, cfg.PigArrive);
                        uint type = cfg.PigTypes[t - 1];
                        int need = (int)left - SpringWorld.Live(u, "ny23pig");
                        for (int k = 0; k < need; k++) SpringWorld.Spawn(u, "ny23pig", type, cfg.PigBox.Map, cfg.PigBox.X, cfg.PigBox.Y, cfg.PigRange, cfg.PigMinutes);
                        Msg(c, string.Format("{0} {1} ate the meat on the sly! Go to teach them a lesson!", left, PigName[t - 1]));
                        return;
                    }
                case 4:
                    {
                        long m = Store.Get(u, "d_fire");
                        if (m < 1 || m > cfg.FireMaps.Length) { m = K.Rand(1, cfg.FireMaps.Length); Store.Set(u, "d_fire", m); }
                        string name; FireMapName.TryGetValue(cfg.FireMaps[m - 1], out name);
                        Msg(c, string.Format(EventText[3], name ?? ("map " + cfg.FireMaps[m - 1])));
                        return;
                    }
                case 5:
                    {
                        if (c.Player.Money < cfg.PacketSilver) { Msg(c, "You don`t have enough silver."); return; }
                        if (!G.HaveSpace(c, 1)) { Msg(c, "Your inventory is full. Please make at least 1 space(s) in your inventory first."); return; }
                        if ((Store.Get(u, "d_done") & (1L << i)) != 0) return;
                        c.Player.Money -= cfg.PacketSilver;
                        try { c.Player.SendUpdate(stream, c.Player.Money, MsgUpdate.DataType.Money); } catch { }
                        Msg(c, EventText[4]);
                        if (GiveSmallBag(c, stream, i)) Msg(c, "You gave out " + cfg.PacketSilver + " Silver and received 1 Small Spring Bag!");
                        break;
                    }
            }
            Lua(c, MainData(c, cfg, "DlgNewYearMainUi_UpdateView"));
        }

        /// <summary>A kill of one of our Piggish monsters (personal spawn of "Troubled Residents").</summary>
        public static void OnPigKilled(SpringWorld.MobRec rec)
        {
            uint u = rec.Owner;
            Client.GameClient owner;
            if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(u, out owner) || owner == null || owner.Player == null) return;
            Touch(u);
            long left = 0;
            if (!Store.Update(u, g => { if (g("d_pigleft") <= 0) return null; left = g("d_pigleft") - 1; return new Dictionary<string, long> { { "d_pigleft", left } }; })) return;
            long t = Store.Get(u, "d_pig");
            string name = t >= 1 && t <= 5 ? PigName[t - 1] : "Piggish";
            Msg(owner, string.Format("You drove away {0} {1}. There are {2} remaining.", 1, name, left));
            if (left > 0 || !Running) return;
            using (var r = new ServerSockets.RecycledPacket())
                if (GiveSmallBag(owner, r.GetStream(), 2)) Msg(owner, "Villagers thanked you a lot and gave you 1 Small Spring Bag.");
        }

        // festival npcs.txt ids (Festivals\LunarNewYear2023\npcs.txt)
        public static readonly uint[] PrayerNpcs = { 8840013, 8840014, 8840015 };
        public static readonly uint[] TubeNpcs = { 8840016, 8840017, 8840018, 8840019, 8840020, 8840021, 8840022, 8840023, 8840024, 8840025 };

        /// <summary>Clicks on the daily NPCs (festival npcs.txt): prayer statues and firework tubes. True = answered.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option)
        {
            int pray = Array.IndexOf(PrayerNpcs, npcid), tube = Array.IndexOf(TubeNpcs, npcid);
            if (pray < 0 && tube < 0) return false;
            if (option != 0) return true;
            var cfg = Cfg;
            if (!Running) { K.Box(c, stream, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { K.Box(c, stream, TxtLevel); return true; }
            uint u = c.Player.UID;
            Touch(u);
            if (pray >= 0)
            {
                if (DailyState(u, 1) == 2) { K.Box(c, stream, "You have completed the quest. Go to check other quests."); return true; }
                Msg(c, "Praying...");
                if (GiveSmallBag(c, stream, 1)) K.Box(c, stream, PrayerName[pray] + " heard your prayer and gave you 1 [Small Spring Bag].");
                return true;
            }
            if (DailyState(u, 3) == 2) { K.Box(c, stream, "Fireworks here have been used. Go to try other fireworks."); return true; }
            long m = Store.Get(u, "d_fire");
            if (m < 1 || m > cfg.FireMaps.Length) { K.Box(c, stream, "Fireworks here are not ready yet. Please wait a while."); return true; }
            uint want = cfg.FireMaps[m - 1];
            if (c.Player.Map != want)
            {
                string name; FireMapName.TryGetValue(want, out name);
                K.Box(c, stream, string.Format("Fireworks here are not ready yet. Go to {0} to have a try!", name ?? ("map " + want)));
                return true;
            }
            Msg(c, "Igniting it...");
            if (GiveSmallBag(c, stream, 3)) K.Box(c, stream, "Fireworks lit up the whole city! You received 1 Small Spring Bag.");
            return true;
        }

        /// <summary>Timer: the Grocers (official shop ids 31133-31137) stand only while the festival runs.</summary>
        public static void SyncWorld()
        {
            var cfg = Cfg;
            bool on = Running;
            for (int i = 0; i < Grocers.Length && i < cfg.GrocerSpots.Length; i++)
            {
                var s = cfg.GrocerSpots[i];
                if (on) SpringWorld.PlaceNpc(Grocers[i], Role.Flags.NpcType.Shop, cfg.GrocerMesh, s.Map, s.X, s.Y);
                else if (SpringWorld.HasNpc(Grocers[i], s.Map)) SpringWorld.RemoveNpc(Grocers[i], s.Map);
            }
            if (!on) SpringWorld.DespawnAll(r => r.Tag == "ny23pig");
        }

        // ------------------------------------------------------------------ lanterns (spring stories)
        /// <summary>Lantern i: the hub NPC's festival teleport to that story map + the line's first quest (the engine accept path).</summary>
        static void Enter(Client.GameClient c, Config cfg, int i)
        {
            var p = c.Player;
            var line = cfg.Lines[i];
            if (p.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            if (Finished(c, line.Last)) { Msg(c, "You have completed this event. Please participate in other events."); return; }
            if (!p.Alive || p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map))
            {
                Msg(c, "Please go to Twin City or Market and try again.");
                return;
            }
            var t = QuestEngine.QuestEngineCore.Current;
            if (t == null) { Msg(c, "This story isn`t open yet."); return; }
            QuestEngine.NpcTeleport tp = null;
            List<QuestEngine.NpcTeleport> tps;
            if (t.NpcTeleports.TryGetValue(cfg.HubNpc, out tps)) tp = tps.FirstOrDefault(x => x != null && x.Map == line.Map && x.Instance == null);
            QuestEngine.EngineQuest first;
            t.Quests.TryGetValue(line.First, out first);

            // the first quest is not taken yet: accept it like its begin NPC does (its onAccept may move the player itself)
            if (first != null && !Taken(c, line.First))
            {
                uint mapBefore = p.Map;
                string name, why;
                if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, first.BeginNpc, line.First, out name, out why))
                {
                    Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                    return;
                }
                Msg(c, "[Quest] New quest: " + name + ".");
                if (p.Map != mapBefore) return;
            }
            if (tp == null) { Msg(c, "This story isn`t open yet."); return; }
            if (tp.MinLevel > 0 && p.Level < tp.MinLevel) { Msg(c, "Please upgrade to level " + tp.MinLevel + " first."); return; }
            if (tp.RequireQuest != 0 && !Taken(c, tp.RequireQuest)) { Msg(c, "Talk to Miss Bunny first."); return; }
            if (p.Map == tp.Map) return;
            if (Pool.ServerMaps[tp.Map] == null) { Msg(c, "This story isn`t open yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, tp.X, tp.Y, tp.Map, 0); else c.Teleport(tp.X, tp.Y, tp.Map, 0);
        }
    }
}
