// ============================================================================
//  [DreamTheater] the parts of the official 2024 "Dream Theater / Swan Lake Romance" event that are not plain quests.
//  Spec: tools\Festivals\DreamTheater_OFFICIAL_AR.md. Data: Infinity[DB]\Festivals\DreamTheater\ (theater.json + quests.json).
//
//  - click mini-games of the festival quests (official texts of client ini\Server_Key.lua tPlotEditorPlotEvent_DelText):
//      5739 herbs ("Collecting.....", 3 herb NPCs)            -> QuestEngine event "dtHerb"
//      5744 horses ("Taming the horse...", kicked / tamed)     -> event "dtHorse", then the quest is finished here (FinishNpcId -1)
//      cleaning 5752/5796-5801 ("Cleaning...", 4 spots)         -> event "dtClean", finished here when 3 spots are clean
//      5754 reading glasses ("Searching...", 2 corners)         -> event "dtGlasses"
//      5760 Knight`s Covenant: Jenkins (theater) starts the duel -> KnightJenkins spawned in the player's private copy
//  - theater management (the official "Dream Theater" window is gone from the client -> NPC dialogs):
//      Agnes: story, Go to Dream Theater (a private copy of map 12430), Wish Fountain (Wish Silver Coin = 1 wish = +20 Theater
//      Points + 1 reward, Wish Gold Coin = 10 wishes), Upgrade (Theater Funds), Theater Points rewards (milestones), local ranking
//      (>= 30000 points, settled Mar 25 08:00, claimed at Agnes until the event ends).
//      Odette (theater): the daily performance after the two daily contracts -> Settlement (Theater Points, Funds, Silver Coins).
//  Everything works only while the festival "DreamTheater" runs. Choices come only from this module's own dialog state; every
//  reward checks bag space first and is recorded before it is given (state Infinity[DB]\Festivals\_dreamtheater\state.json).
//
//  Hooks (one line each, outside Base\Festivals / Base\QuestEngine):
//      MY System\Program.cs  before FestivalManager.Start()  -> DreamTheaterModule.Init();
//      Game\MsgNpc\Procesor.cs before FestivalManager.OnNpc   -> if (DreamTheaterModule.OnNpc(...)) return;
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.DreamTheater
{
    public static partial class DreamTheaterModule// [LuaUi] the official window part: DreamTheaterLua.cs
    {
        public const string Key = "DreamTheater";
        public const uint Agnes = 8830000, Leon = 8830003, Odette = 8830004;
        public const uint UntamedHorse = 8830010, DocileHorse = 8830011, TheaterJenkins = 32624;
        static readonly uint[] HerbNpcs = { 32609, 8830012, 8830013 };
        static readonly string[] HerbMsg = {
            "You have collected the Balloon Flower. Please go and collect the next herb!",
            "You have collected the Figwort. Please go and collect the next herb!",
            "You have collected the herb. Please go and collect the next herb!" };
        static readonly uint[] CleanNpcs = { 8830014, 8830015, 8830016, 8830017 };
        static readonly string[] CleanMsg = {
            "You have cleaned up the Dirty Floor! Go on!", "You have cleaned up the Messy Stage! Go on!",
            "You have cleaned up the Stained Desk! Go on!", "You have cleaned up the Dirty Floor! Go on!" };
        static readonly uint[] GlassNpcs = { 8830018, 8830019 };
        const uint QHerb = 5739, QHorse = 5744, QHorseNext = 5745, QGlasses = 5754, QDuel = 5760;
        public const string EvHerb = "dtHerb", EvHorse = "dtHorse", EvClean = "dtClean", EvGlasses = "dtGlasses";

        /// <summary>Test seams (null / 3000 on the server).</summary>
        public static Func<DateTime> ClockOverride;
        public static int PickDelayMs = 3000;
        public static Func<bool> RunningOverride;
        static DateTime Now { get { var f = ClockOverride; return f != null ? f() : DateTime.Now; } }

        static readonly object _lock = new object();
        static Timer _timer;
        static bool _inited;

        // ------------------------------------------------------------------ init / config
        public static void Init()
        {
            try
            {
                lock (_lock)
                {
                    foreach (var k in new[] { EvHerb, EvHorse, EvClean, EvGlasses }) QuestEngine.QuestGrammar.KnownEvents.Add(k);
                    if (_inited) return;
                    LoadState();
                    if (_timer == null && RunningOverride == null) _timer = new Timer(_ => Tick(), null, 30000, 30000);
                    _inited = true;
                }
                MyConsole.WriteLine("[DreamTheater] module ready (theater management, Wish Fountain, click mini-games).", ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[DreamTheater] init failed (module off): " + e.Message, ConsoleColor.Yellow);
                try { MyConsole.SaveException(e); } catch { }
            }
        }

        public sealed class Level { public int Lv, Points, Silver; public ulong UpgradeFunds, Funds; }
        public sealed class RItem { public uint Id; public ushort Count; public int Weight; public bool Bound; public uint Minutes; }
        public sealed class Milestone { public ulong Points; public List<RItem> Items = new List<RItem>(); }
        public sealed class RankReward { public int From, To; public string Box = ""; public List<RItem> Items = new List<RItem>(); }
        public sealed class Segment { public int Index; public uint Start, End; }
        public sealed class VelvetGood { public int Group; public uint Item, Cost; }
        public sealed class Config
        {
            // [DreamTheater 2026-10-15 video pass] official window contracts / settlement / items (theater.json)
            public List<Segment> Segments = new List<Segment>();
            public string DailyAfterRecruit = "alternate";
            public int PointsPerMember = 30, SilverPerMember = 1;
            public uint TwinkleCoin = 3347478, LoveVelvet = 3347321;
            public Dictionary<uint, List<RItem>> Boxes = new Dictionary<uint, List<RItem>>();
            public List<VelvetGood> VelvetGoods = new List<VelvetGood>();
            public uint VelvetMakeUpCps = 163, VelvetChi = 100;
            public Dictionary<uint, int> CoinChi = new Dictionary<uint, int>();
            public Dictionary<uint, int[]> FlowerPacks = new Dictionary<uint, int[]>();
            public ushort MinLevel = 80;
            public uint TheaterMap = 12430; public ushort EntryX = 139, EntryY = 94;
            public uint ExitMap = 1002; public ushort ExitX = 373, ExitY = 441;
            public uint UnlockQuest = 5737;
            public List<uint> Members = new List<uint>(), CleanQuests = new List<uint>(), TroubleDone = new List<uint>();
            public List<Level> Levels = new List<Level>();
            public ulong PerformanceFunds = 100000; public int MemberBonusPercent = 10;
            public uint SilverCoin = 3600697, GoldCoin = 3347476;
            public int WishPoints = 20;
            public List<RItem> WishPool = new List<RItem>();
            public List<Milestone> Milestones = new List<Milestone>();
            public ulong ExtraEvery = 5000; public List<RItem> ExtraItems = new List<RItem>();
            public ulong RankMinPoints = 30000; public int SettleMonth = 3, SettleDay = 25, SettleMinute = 480;
            public List<RankReward> RankRewards = new List<RankReward>();
            public Level LevelOf(int lv) { return Levels.FirstOrDefault(l => l.Lv == lv) ?? Levels.LastOrDefault() ?? new Level { Lv = 1, Points = 300, Silver = 8 }; }
            public int MaxLevel { get { return Levels.Count == 0 ? 1 : Levels.Max(l => l.Lv); } }
        }
        static Config _cfg; static string _cfgStamp;
        static string Folder { get { return Path.Combine(Program.ServerConfig.DbLocation, "Festivals", Key); } }
        static string StateFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "Festivals", "_dreamtheater", "state.json"); } }

        public static Config Cfg
        {
            get
            {
                string f = Path.Combine(Folder, "theater.json");
                try
                {
                    var fi = new FileInfo(f);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _cfgStamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists) ReadConfig(c, Festivals.FestivalLoader.ReadJson(f));
                    if (c.Levels.Count == 0) c.Levels.Add(new Level { Lv = 1, Points = 300, Silver = 8 });
                    _cfg = c; _cfgStamp = stamp;
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[DreamTheater] theater.json: " + e.Message, ConsoleColor.Yellow);
                    if (_cfg == null) { _cfg = new Config(); _cfg.Levels.Add(new Level { Lv = 1, Points = 300, Silver = 8 }); }
                }
                return _cfg;
            }
        }
        static uint U(object o) { return Festivals.FestivalLoader.U(o); }
        static ulong UL(object o) { return Festivals.FestivalLoader.UL(o); }
        static object V(Dictionary<string, object> d, string k) { return Festivals.FestivalLoader.Val(d, k); }
        static List<object> A(object o) { return o as List<object> ?? new List<object>(); }
        static List<RItem> Items(object o)
        {
            var l = new List<RItem>();
            foreach (var e in A(o)) { var d = e as Dictionary<string, object>; if (d == null) continue; uint id = U(V(d, "id")); if (id == 0) continue; l.Add(new RItem { Id = id, Count = (ushort)Math.Max(1, Math.Min(10000, V(d, "count") == null ? 1 : U(V(d, "count")))), Weight = (int)Math.Max(1, V(d, "weight") == null ? 1 : U(V(d, "weight"))), Bound = Equals(V(d, "bound"), true), Minutes = V(d, "minutes") == null ? 0 : U(V(d, "minutes")) }); }
            return l;
        }
        static void ReadConfig(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            if (V(d, "minLevel") != null) c.MinLevel = (ushort)U(V(d, "minLevel"));
            if (V(d, "theaterMap") != null) { c.TheaterMap = U(V(d, "theaterMap")); c.EntryX = (ushort)U(V(d, "entryX")); c.EntryY = (ushort)U(V(d, "entryY")); }
            if (V(d, "exitMap") != null) { c.ExitMap = U(V(d, "exitMap")); c.ExitX = (ushort)U(V(d, "exitX")); c.ExitY = (ushort)U(V(d, "exitY")); }
            if (V(d, "unlockQuest") != null) c.UnlockQuest = U(V(d, "unlockQuest"));
            c.Members = A(V(d, "members")).Select(U).Where(x => x != 0).ToList();
            c.CleanQuests = A(V(d, "cleanQuests")).Select(U).Where(x => x != 0).ToList();
            c.TroubleDone = A(V(d, "troubleDone")).Select(U).Where(x => x != 0).ToList();
            foreach (var e in A(V(d, "levels"))) { var l = e as Dictionary<string, object>; if (l == null) continue; c.Levels.Add(new Level { Lv = (int)U(V(l, "level")), Points = (int)U(V(l, "points")), Silver = (int)U(V(l, "silver")), UpgradeFunds = UL(V(l, "upgradeFunds")), Funds = V(l, "funds") == null ? 100000UL : UL(V(l, "funds")) }); }
            foreach (var e in A(V(d, "recruitSegments"))) { var s = e as Dictionary<string, object>; if (s == null) continue; c.Segments.Add(new Segment { Index = (int)U(V(s, "index")), Start = U(V(s, "start")), End = U(V(s, "end")) }); }
            c.Segments = c.Segments.Where(s => s.Index >= 1 && s.Index <= 7 && s.Start != 0 && s.End != 0).OrderBy(s => s.Index).ToList();
            if (V(d, "dailyAfterRecruit") is string) c.DailyAfterRecruit = (string)V(d, "dailyAfterRecruit");
            if (V(d, "pointsPerMember") != null) c.PointsPerMember = (int)U(V(d, "pointsPerMember"));
            if (V(d, "silverPerMember") != null) c.SilverPerMember = (int)U(V(d, "silverPerMember"));
            if (V(d, "twinkleCoin") != null) c.TwinkleCoin = U(V(d, "twinkleCoin"));
            if (V(d, "loveVelvet") != null) c.LoveVelvet = U(V(d, "loveVelvet"));
            var boxes = V(d, "boxes") as Dictionary<string, object>;
            if (boxes != null) foreach (var kv in boxes) { uint id; if (uint.TryParse(kv.Key, out id) && id != 0) c.Boxes[id] = Items(kv.Value); }
            foreach (var e in A(V(d, "loveVelvetGoods"))) { var g = e as Dictionary<string, object>; if (g == null) continue; var vg = new VelvetGood { Group = (int)U(V(g, "group")), Item = U(V(g, "item")), Cost = U(V(g, "cost")) }; if (vg.Item != 0 && vg.Cost > 0) c.VelvetGoods.Add(vg); }
            if (V(d, "velvetMakeUpCps") != null) c.VelvetMakeUpCps = U(V(d, "velvetMakeUpCps"));
            if (V(d, "velvetChiPoints") != null) c.VelvetChi = U(V(d, "velvetChiPoints"));
            var chi = V(d, "coinChiAfterEvent") as Dictionary<string, object>;
            if (chi != null) foreach (var kv in chi) { uint id; if (uint.TryParse(kv.Key, out id)) c.CoinChi[id] = (int)U(kv.Value); }
            var packs = V(d, "flowerPacks") as Dictionary<string, object>;
            if (packs != null) foreach (var kv in packs) { uint id; var pd = kv.Value as Dictionary<string, object>; if (uint.TryParse(kv.Key, out id) && pd != null) c.FlowerPacks[id] = new[] { (int)U(V(pd, "min")), (int)U(V(pd, "max")) }; }
            c.Levels = c.Levels.Where(l => l.Lv > 0).OrderBy(l => l.Lv).ToList();
            if (V(d, "performanceFunds") != null) c.PerformanceFunds = UL(V(d, "performanceFunds"));
            if (V(d, "memberBonusPercent") != null) c.MemberBonusPercent = (int)U(V(d, "memberBonusPercent"));
            if (V(d, "silverCoin") != null) c.SilverCoin = U(V(d, "silverCoin"));
            if (V(d, "goldCoin") != null) c.GoldCoin = U(V(d, "goldCoin"));
            if (V(d, "wishPoints") != null) c.WishPoints = (int)U(V(d, "wishPoints"));
            c.WishPool = Items(V(d, "wishPool"));
            foreach (var e in A(V(d, "milestones"))) { var m = e as Dictionary<string, object>; if (m == null) continue; c.Milestones.Add(new Milestone { Points = UL(V(m, "points")), Items = Items(V(m, "items")) }); }
            c.Milestones = c.Milestones.Where(m => m.Points > 0).OrderBy(m => m.Points).Take(8).ToList();
            if (V(d, "extraEvery") != null) c.ExtraEvery = Math.Max(1, UL(V(d, "extraEvery")));
            c.ExtraItems = Items(V(d, "extraItems"));
            if (V(d, "rankMinPoints") != null) c.RankMinPoints = UL(V(d, "rankMinPoints"));
            string settle = V(d, "rankSettle") as string;
            if (!string.IsNullOrEmpty(settle))
            {
                DateTime t;
                if (DateTime.TryParseExact("2024-" + settle.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out t)) { c.SettleMonth = t.Month; c.SettleDay = t.Day; c.SettleMinute = t.Hour * 60 + t.Minute; }
            }
            foreach (var e in A(V(d, "rankRewards"))) { var r = e as Dictionary<string, object>; if (r == null) continue; c.RankRewards.Add(new RankReward { From = (int)U(V(r, "from")), To = (int)U(V(r, "to")), Box = (V(r, "box") as string) ?? "", Items = Items(V(r, "items")) }); }
        }

        // ------------------------------------------------------------------ per-character state
        public sealed class PState
        {
            public uint Uid; public string Name = "";
            public int Level = 1; public ulong Funds, Points; public long PointsTicks;
            public int PerfDay; public bool Intro, RankPaid; public int ExtraClaimed;
            public HashSet<ulong> Claimed = new HashSet<ulong>();
            // [DreamTheater 2026-10-15] the window's daily contract: ContractIdx = TaskIndex taken (1-7 recruitment, 8 cleaning,
            // 9 troubles), ContractDone once its quest(s) finished; the performance consumes it. PlayNum = performances so far.
            public int ContractIdx, PlayNum; public bool ContractDone, GuideDone;
            public HashSet<string> Marks = new HashSet<string>();// herb / horse / clean / glasses spots already done (survive relogs and restarts)
        }
        static long _seasonStartTicks;// first run start of this season (the ranking settles only for a run that started before the settle time)
        static bool _mailed;           // ranking mails sent for this season
        static Dictionary<uint, PState> _players = new Dictionary<uint, PState>();
        static int _season;
        static List<uint> _frozen;// ranking frozen at the settle time (uids in rank order)
        static bool _dirty;

        static int Season()
        {
            var r = Festivals.FestivalManager.Running(Key);
            return (r != null ? r.Started : Now).Year;
        }
        /// <summary>The player's state for the running season (a new season = a new event edition starts from zero).</summary>
        public static PState State(Client.GameClient c)
        {
            lock (_lock)
            {
                int s = Season();
                if (s != _season) { _players = new Dictionary<uint, PState>(); _frozen = null; _season = s; _seasonStartTicks = 0; _mailed = false; _dirty = true; }
                PState p;
                if (!_players.TryGetValue(c.Player.UID, out p)) { p = new PState { Uid = c.Player.UID }; _players[p.Uid] = p; }
                p.Name = c.Player.Name ?? p.Name;
                return p;
            }
        }
        static void LoadState()
        {
            try
            {
                if (!File.Exists(StateFile)) return;
                var d = Festivals.FestivalLoader.ReadJson(StateFile);
                if (d == null) return;
                _season = (int)U(V(d, "season"));
                _seasonStartTicks = (long)UL(V(d, "seasonStart"));
                _mailed = Equals(V(d, "mailed"), true);
                var fr = V(d, "frozen") as List<object>;
                if (fr != null) _frozen = fr.Select(U).ToList();
                foreach (var e in A(V(d, "players")))
                {
                    var p = e as Dictionary<string, object>; if (p == null) continue;
                    var s = new PState { Uid = U(V(p, "uid")), Name = (V(p, "name") as string) ?? "", Level = Math.Max(1, (int)U(V(p, "level"))), Funds = UL(V(p, "funds")), Points = UL(V(p, "points")), PointsTicks = (long)UL(V(p, "pointsTicks")), PerfDay = (int)U(V(p, "perfDay")), Intro = Equals(V(p, "intro"), true), RankPaid = Equals(V(p, "rankPaid"), true), ExtraClaimed = (int)U(V(p, "extraClaimed")) };
                    foreach (var m in A(V(p, "claimed"))) s.Claimed.Add(UL(m));
                    s.ContractIdx = (int)U(V(p, "contract")); s.ContractDone = Equals(V(p, "contractDone"), true); s.PlayNum = (int)U(V(p, "playNum")); s.GuideDone = Equals(V(p, "guide"), true);
                    foreach (var m in A(V(p, "marks"))) { var ms = m as string; if (!string.IsNullOrEmpty(ms)) s.Marks.Add(ms); }
                    if (s.Uid != 0) _players[s.Uid] = s;
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[DreamTheater] state.json: " + e.Message, ConsoleColor.Yellow); }
        }
        static string Js(string s) { return "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\""; }
        /// <summary>Writes the state (temp file + replace). Called after every reward; also by the 30 s timer when dirty.</summary>
        public static void SaveState()
        {
            lock (_lock)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.Append("{\"_readme\":\"Written by the GameServer [DreamTheater] module. Do not edit while the server runs.\",\"season\":").Append(_season)
                      .Append(",\"seasonStart\":").Append(_seasonStartTicks).Append(",\"mailed\":").Append(_mailed ? "true" : "false");
                    if (_frozen != null) sb.Append(",\"frozen\":[").Append(string.Join(",", _frozen)).Append("]");
                    sb.Append(",\"players\":[");
                    bool first = true;
                    foreach (var p in _players.Values)
                    {
                        if (!first) sb.Append(","); first = false;
                        sb.Append("{\"uid\":").Append(p.Uid).Append(",\"name\":").Append(Js(p.Name)).Append(",\"level\":").Append(p.Level).Append(",\"funds\":").Append(p.Funds)
                          .Append(",\"points\":").Append(p.Points).Append(",\"pointsTicks\":").Append(p.PointsTicks).Append(",\"perfDay\":").Append(p.PerfDay)
                          .Append(",\"intro\":").Append(p.Intro ? "true" : "false").Append(",\"rankPaid\":").Append(p.RankPaid ? "true" : "false").Append(",\"extraClaimed\":").Append(p.ExtraClaimed)
                          .Append(",\"claimed\":[").Append(string.Join(",", p.Claimed)).Append("]")
                          .Append(",\"contract\":").Append(p.ContractIdx).Append(",\"contractDone\":").Append(p.ContractDone ? "true" : "false")
                          .Append(",\"playNum\":").Append(p.PlayNum).Append(",\"guide\":").Append(p.GuideDone ? "true" : "false")
                          .Append(",\"marks\":[").Append(string.Join(",", p.Marks.Select(Js))).Append("]}");
                    }
                    sb.Append("]}");
                    string f = StateFile;
                    Directory.CreateDirectory(Path.GetDirectoryName(f));
                    string tmp = f + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
                    if (File.Exists(f)) File.Replace(tmp, f, null); else File.Move(tmp, f);
                    _dirty = false;
                }
                catch (Exception e) { MyConsole.WriteLine("[DreamTheater] save failed: " + e.Message, ConsoleColor.Yellow); }
            }
        }
        static void Tick()
        {
            try
            {
                var r = Festivals.FestivalManager.Running(Key);
                if (r != null) lock (_lock) { Season(); if (_seasonStartTicks == 0 || r.Started.Ticks < _seasonStartTicks) { _seasonStartTicks = r.Started.Ticks; _dirty = true; } }
                FreezeIfDue();// [DreamTheater 2026-10-15] also after the event closed: the official ranking settles 1 minute after the end
                MailRanking();
                if (_dirty) SaveState(); SweepDuels();
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        static void Log(Client.GameClient c, string what)
        {
            try { Festivals.FestivalManager.Log("THEATER", Key, c.Player.UID + " " + c.Player.Name + " lv" + c.Player.Level + " | " + what); } catch { }
        }

        // ------------------------------------------------------------------ helpers
        public static bool Running { get { var o = RunningOverride; return o != null ? o() : Festivals.FestivalManager.IsRunning(Key); } }
        static QuestEngine.EngineQuest Q(uint id) { var t = QuestEngine.QuestEngineCore.Current; QuestEngine.EngineQuest q; return t != null && t.Quests.TryGetValue(id, out q) ? q : null; }
        static uint SlotValue(Client.GameClient c, uint id, byte slot = 1)
        {
            uint[] cur; c.Player.QuestGUI.GetQuestObjectives(id, out cur);
            return cur != null && slot >= 1 && slot <= cur.Length ? cur[slot - 1] : 0;
        }
        static bool Active(Client.GameClient c, uint id) { return c.Player.QuestGUI.IsActiveQuest(id); }
        static bool Finished(Client.GameClient c, uint id) { return c.Player.QuestGUI.CheckQuest(id, QS.Finished); }
        static void Box(Client.GameClient c, ServerSockets.Packet stream, string text)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            Text(d, text);
            d.AddOption("OK.", 255).AddAvatar(10).FinalizeDialog();
        }
        static void Text(Game.MsgNpc.Dialog d, string text)
        {
            text = (text ?? "").Trim();
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 200);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n - 1, n); if (sp > 100) n = sp + 1; }
                d.AddText(text.Substring(0, n));
                text = text.Substring(n);
            }
        }
        static void Msg(Client.GameClient c, string text) { try { c.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static uint CountItem(Client.GameClient c, uint id)
        {
            uint n = 0;
            foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u;
            return n;
        }
        static bool TakeItem(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (CountItem(c, id) < count) return false;
            uint left = count;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).ToArray())
            {
                if (left == 0) break;
                uint stack = item.StackSize > 1 ? item.StackSize : 1u;
                if (stack > left) { item.StackSize = (ushort)(stack - left); item.Mode = Role.Flags.ItemMode.Update; item.Send(c, stream); left = 0; }
                else { left -= stack; item.StackSize = 1; c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream); }
            }
            return true;
        }
        static int SlotsFor(IEnumerable<RItem> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i.Id, out db)) continue;
                n += i.Minutes > 0 ? i.Count : db.StackSize > 1 ? (i.Count + db.StackSize - 1) / db.StackSize : i.Count;
            }
            return n;
        }
        static string ItemName(uint id) { return QuestEngine.QuestGrammar.ItemName(id); }
        static string Give(Client.GameClient c, IEnumerable<RItem> items, ServerSockets.Packet stream)
        {
            var parts = new List<string>();
            foreach (var g in items)
            {
                if (Pool.ItemsBase == null || !Pool.ItemsBase.ContainsKey(g.Id)) continue;
                try
                {
                    bool ok = true;
                    if (g.Minutes > 0) { for (int i = 0; i < Math.Min((int)g.Count, 40); i++) c.Inventory.AddTime(stream, g.Id, 0, 0, 0, g.Bound, g.Minutes * 60); }
                    else ok = c.Inventory.AddItemWitchStack(g.Id, 0, g.Count, stream, g.Bound);
                    if (ok) parts.Add(ItemName(g.Id) + (g.Count > 1 ? " x" + g.Count : "")); else parts.Add("FAILED " + g.Id);
                }
                catch (Exception e) { parts.Add("FAILED " + g.Id + " (" + e.Message + ")"); }
            }
            return string.Join(", ", parts);
        }
        static bool Space(Client.GameClient c, int slots) { return slots <= 0 || c.Inventory.HaveSpace((byte)Math.Min(40, slots)); }
        static void Move(Client.GameClient c, ushort x, ushort y, uint map, uint dyn)
        {
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, x, y, map, dyn); else c.Teleport(x, y, map, dyn);
        }

        // ------------------------------------------------------------------ dialog state
        sealed class Dlg { public uint Npc; public readonly HashSet<byte> Opts = new HashSet<byte>(); }
        static readonly ConcurrentDictionary<uint, Dlg> _dlg = new ConcurrentDictionary<uint, Dlg>();
        static Dlg NewDlg(Client.GameClient c, uint npc) { var d = new Dlg { Npc = npc }; _dlg[c.Player.UID] = d; return d; }
        static Game.MsgNpc.Dialog Opt(Game.MsgNpc.Dialog d, Dlg st, string text, byte code) { st.Opts.Add(code); return d.AddOption(text.Length > 60 ? text.Substring(0, 57) + "..." : text, code); }

        /// <summary>Procesor hook (the NPC is already in the player's view). True = answered; false = the festival / engine continue.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null || npcid < 32606) return false;
            try { if (option == 0 && Running && Reenter(c, npcid)) return true; } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            bool ours = npcid == Agnes || npcid == Odette || npcid == UntamedHorse || npcid == DocileHorse || npcid == TheaterJenkins
                        || HerbNpcs.Contains(npcid) || CleanNpcs.Contains(npcid) || GlassNpcs.Contains(npcid);
            if (!ours) return false;
            try
            {
                if (!Running) return false;
                if (option == 0)
                {
                    Dlg old; _dlg.TryRemove(c.Player.UID, out old);
                    if (npcid == Agnes) { AgnesMain(c, stream); return true; }
                    if (npcid == Odette) return OdetteMain(c, stream);
                    if (npcid == TheaterJenkins) return JenkinsMain(c, stream);
                    if (HerbNpcs.Contains(npcid)) return Herb(c, stream, npcid);
                    if (npcid == UntamedHorse || npcid == DocileHorse) return Horse(c, stream, npcid);
                    if (CleanNpcs.Contains(npcid)) return Clean(c, stream, npcid);
                    if (GlassNpcs.Contains(npcid)) return Glasses(c, stream, npcid);
                    return false;
                }
                Dlg st;
                if (!_dlg.TryGetValue(c.Player.UID, out st) || st.Npc != npcid || !st.Opts.Contains(option)) return false;
                _dlg.TryRemove(c.Player.UID, out st);
                if (option == 255) return true;
                if (npcid == Agnes) AgnesChoice(c, stream, option);
                else if (npcid == Odette) OdetteChoice(c, stream, option);
                else if (npcid == TheaterJenkins) JenkinsChoice(c, stream, option);
                return true;
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                return false;
            }
        }

        /// <summary>[DreamTheater 2026-10-15 video pass] Role\RoleView.CanSee hook for map 12431 (the theater copies): the official
        /// dynamic NPCs (tMasterAndTheatre2024 [Sys][DynaNpc][1..7]) - an actor appears once recruited, the cleaning spots only
        /// during a cleaning contract, the two corners only during 5754 (videos qiOK 3:00 / 9ZTy 9:10: only the recruited actors
        /// stand in the theater). Players, monsters and the other NPCs are not affected.</summary>
        public static bool TheaterNpcVisible(Client.GameClient c, Role.IMapObj obj)
        {
            try
            {
                if (c == null || c.Player == null || obj == null || obj.ObjType != Role.MapObjectType.Npc) return true;
                uint id = obj.UID;
                uint member = id == 32619 ? 5737u : id == 32620 ? 5740u : id == 32621 ? 5742u : id == 32622 ? 5745u : id == 32623 ? 5746u : id == 32624 ? 5749u : id == 32625 ? 5751u : 0u;
                if (member != 0) return Finished(c, member);
                if (CleanNpcs.Contains(id)) return Cfg.CleanQuests.Any(q => Active(c, q));
                if (GlassNpcs.Contains(id)) return Active(c, QGlasses);
                return true;
            }
            catch { return true; }
        }

        /// <summary>[DreamTheater 2026-10-15] The fight / taming quests run in private copies (Questinfo maps 1002566 / 1002565 /
        /// 1002560). A player who left the copy before the end (scroll, death, timeout) goes back in by talking to the NPC of that
        /// spot again: the Pine (5803 Black Bears), Oliver (5744 horses), Jenkins (5748 Birdmen). Only while the quest is active
        /// and its objective is not complete.</summary>
        static bool Reenter(Client.GameClient c, uint npcid)
        {
            uint q = npcid == 32607 ? 5803u : npcid == 32612 ? 5744u : npcid == 32616 ? 5748u : 0u;
            if (q == 0 || !Active(c, q)) return false;
            uint need = q == 5803 ? 5u : q == 5744 ? 2u : 20u;
            if (SlotValue(c, q) >= need) return false;// complete: the engine finishes it
            if (c.Player.DynamicID != 0) return false;// already inside a copy
            if (!QuestEngine.QuestEngineCore.ReenterInstance(c, q)) return false;
            Msg(c, q == 5803 ? "Some Black Bears spring out to snatch your Pine! Defeat the Black Bears!" : q == 5744 ? "The grass here is rich and the horses are strong. Go to find a suitable horse for Oliver!" : "The Birdmen outside the city are causing trouble. Please assist Jenkins in clearing the Birdmen.");
            Log(c, "re-entered the copy of " + q);
            return true;
        }

        // ------------------------------------------------------------------ Agnes
        const string Duration = "Duration: 0:00 (US)/8:00 (EU), Mar. 5 - 7:59 (US)/15:59 (EU), Mar. 25";
        static void AgnesMain(Client.GameClient c, ServerSockets.Packet stream)
        {
            // [DreamTheater 2026-10-15 video pass] the official Agnes script tWhiteDay2024NPC[32649] (videos qiOK 1:16-1:24, 4:38, 6:06):
            // level < 80 -> Text111-115 + "Level up first."; first talk -> the story (Text211..261) and "Revive the theater," opens the
            // Dream Theater window with its first-use guide; later -> Text311 + the guide texts + Go to Dream Theater (the window) /
            // Go to Swan Lake (locked) / View Theater Points and ranking rewards (the window's reward page).
            var cfg = Cfg;
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, Agnes);
            if (c.Player.Level < cfg.MinLevel)
            {
                Text(d, "Agnes: The romantic and beautiful opera `Swan Lake Romance` will be performed at [Dream Theater]. If you can assist me in rebuilding the theater, I will share the profits earned from the performances with you. " + Duration + ". Requirement: Level " + cfg.MinLevel + ". Reward: Swan Sonata (Purity), Beloved Bridal (Vow), Ebony Dream (Vow), and more.");
                Opt(d, st, "Level up first.", 255).AddAvatar(10).FinalizeDialog();
                return;
            }
            var p = State(c);
            if (!p.Intro)
            {
                Text(d, "Agnes: Break the barrier of time and remove the curse on love. As the inheritor of Dream Theater, will bring the most beautiful and romantic opera in the world to the stage! This masterpiece will surely sweep the entire city! Just thinking about it is exciting!");
                Opt(d, st, "What opera is this?", 161).AddAvatar(10).FinalizeDialog();
                return;
            }
            Text(d, "Agnes: Dream Theater, with its century-long history, must innovate and adapt to the changing times in order to win back the hearts of the audience. As long as you manage it with all your heart, Dream Theater will definitely make a comeback.\n");
            Text(d, "--------------Dream Theater Guide------------\n1. You can come to take daily quests to ensure smooth performances.\n2. After completing the daily quests, you can start the performance. Once the performance is over, you will receive Theater Funds and Theater Points.\n3. Theater Funds can be used to repair and upgrade the theater.\n");
            Text(d, "--------------Guide for Reviving Dream Theater------------\n1. After the daily performance, you can receive a large amount of Theater Points and Theater Funds.\n2. Increasing the number of performers and renovating the theater will increase the Theater Points obtained after the performance.\n3. Make a wish at the Wish Fountain using Gold/Silver Coins to increase the Theater Points.\n");
            Text(d, "Accumulated Theater Points can be used to compete on the local/regional rankings. Invite friends to team up and manage the theater for a chance to get listed on the team ranking! " + Duration + ". Requirement: Level " + cfg.MinLevel + ".");
            Opt(d, st, "Go to Dream Theater.", 171);
            Opt(d, st, "Go to Swan Lake (locked).", 172);
            Opt(d, st, "View Theater Points and ranking rewards.", 174).AddAvatar(10).FinalizeDialog();
        }

        static void AgnesChoice(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            var p = State(c);
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, Agnes);
            switch (option)
            {
                case 161:
                    Text(d, "Agnes: The inspiration for the story comes from my dear brother Leon, a handsome and charming opera actor who falls in love with a cursed Black Swan princess, but is too shy to... Ah! I cannot be a spoiler! I have asked you to come because I need your help!");
                    Opt(d, st, "How can I help?", 162).AddAvatar(10).FinalizeDialog(); return;
                case 162:
                    Text(d, "Agnes: You can help me with so many things! I inherited the Dream Theater from my father, but I have been engrossed in creating, leaving no time to manage the theater. As a result, the theater is unable to make ends meet and is on the brink of bankruptcy. It wasn`t until my brother Leon and I encountered that beautiful Black Swan princess...");
                    Opt(d, st, "It sounds like an adventure...", 163).AddAvatar(10).FinalizeDialog(); return;
                case 163:
                    Text(d, "Agnes: After bringing Princess Odette back to the theater, I employed the most powerful witch to allow her to maintain a human form. However, the power of the curse is too strong, and the human form can only be maintained for a few months. The evil curse will eventually consume the princess`s last free soul.");
                    Opt(d, st, "That`s so cruel!", 164).AddAvatar(10).FinalizeDialog(); return;
                case 164:
                    Text(d, "Agnes: Princess Odette hopes to become the heroine in my story and perform on the stage of Dream Theater, showcasing the dance of the Black Swan princess. Unfortunately, I am not a qualified theater manager. Despite several attempts, I have been unable to revive Dream Theater. Would you be willing to help me revive the theater and fulfill Princess Odette`s final wish?");
                    Opt(d, st, "Yes!", 165).AddAvatar(10).FinalizeDialog(); return;
                case 165:
                    Text(d, "Agnes: That`s wonderful! With your assistance, Dream Theater will definitely regain its former glory!");
                    Opt(d, st, "Revive the theater,", 166).AddAvatar(10).FinalizeDialog(); return;
                case 166:
                    _dlg.TryRemove(c.Player.UID, out st);
                    lock (_lock) { p.Intro = true; _dirty = true; }
                    LuaOpenMain(c);
                    return;
                case 171:
                    _dlg.TryRemove(c.Player.UID, out st);
                    LuaOpenMain(c);
                    return;
                case 172:
                    _dlg.TryRemove(c.Player.UID, out st);
                    Box(c, stream, "Only when Princess Odette obtains true love will a miracle occur, opening the path to Swan Lake.");
                    return;
                case 174:
                    _dlg.TryRemove(c.Player.UID, out st);
                    OnLua(c, "WhiteDayLua_OpenRankAndStage", null);
                    return;
                default:
                    _dlg.TryRemove(c.Player.UID, out st);
                    return;
            }
        }

        static void EnterTheater(Client.GameClient c)
        {
            var cfg = Cfg;
            // [FestivalAudit 2026-10-14] the window's "Enter Theater" (DreamTheaterLua LuaEnter) works from anywhere, and Leon
            // sends the player back to Twin City: never out of a locked map (jail 6000 / bot jail 10001 / arena / events,
            // Pool.Constants.BlockTeleportMap), a private copy of another system, or while dead (same rule as the other
            // remote teleports, Audit H4).
            var pl = c.Player;
            if (!pl.Alive || pl.Map == 6001 || pl.Map == 1038 || pl.Map == Game.MsgTournaments.MsgClassPKWar.MapID
                || Pool.Constants.BlockTeleportMap.Contains(pl.Map) || (pl.DynamicID != 0 && pl.Map != cfg.TheaterMap))
            {
                Msg(c, "[Dream Theater] You can't go to the theater from here.");
                return;
            }
            var gm = Pool.ServerMaps[cfg.TheaterMap];
            if (gm == null) { Msg(c, "[Dream Theater] The theater is not open yet."); return; }
            uint dyn = gm.GenerateDynamicID();// a private copy: only this player (and the dynamic festival NPCs) in it
            Move(c, cfg.EntryX, cfg.EntryY, cfg.TheaterMap, dyn);
            Msg(c, "Welcome to Dream Theater! You can freely explore the theater!");
            Log(c, "enter theater copy " + dyn);
        }

        // ---- Wish Fountain
        static void WishMenu(Client.GameClient c, ServerSockets.Packet stream, Game.MsgNpc.Dialog d, Dlg st, PState p)
        {
            var cfg = Cfg;
            Text(d, "The Wish Fountain was left by the love goddess Aphrodite. When true love appears, the magic of the love goddess reappears in Pegg City. By stopping at the fountain and throwing coins into it, you can receive the favor of the love goddess and obtain many rewards.");
            Text(d, "Wish Silver Coin: 1 wish (+" + cfg.WishPoints + " Theater Points). Wish Gold Coin: 10 wishes (+" + (cfg.WishPoints * 10) + "). You have " + CountItem(c, cfg.SilverCoin) + " Silver and " + CountItem(c, cfg.GoldCoin) + " Gold Coin(s).");
            Opt(d, st, "Throw coins (Wish Silver Coin).", 180);
            Opt(d, st, "Throw coins (Wish Gold Coin).", 181);
            Opt(d, st, "Back.", 177).AddAvatar(10).FinalizeDialog();
        }
        static readonly Random _rnd = new Random();
        static List<RItem> Merge(List<RItem> l)
        {
            return l.GroupBy(x => new { x.Id, x.Bound, x.Minutes }).Select(g => new RItem { Id = g.Key.Id, Bound = g.Key.Bound, Minutes = g.Key.Minutes, Count = (ushort)Math.Min(10000, g.Sum(x => (int)x.Count)) }).ToList();
        }
        static RItem Roll(List<RItem> pool)
        {
            int total = pool.Sum(x => x.Weight);
            if (total <= 0) return null;
            int r; lock (_rnd) r = _rnd.Next(total);
            foreach (var x in pool) { if (r < x.Weight) return x; r -= x.Weight; }
            return pool[pool.Count - 1];
        }
        /// <summary>One coin into the fountain: the coin is taken, the Theater Points and the rewards are recorded and given.</summary>
        public static string Wish(Client.GameClient c, ServerSockets.Packet stream, bool gold)
        {
            List<RItem> won;
            if (stream == null) { using (var rec = new ServerSockets.RecycledPacket()) return WishCore(c, rec.GetStream(), gold, false, out won); }
            return WishCore(c, stream, gold, true, out won);
        }
        static string WishCore(Client.GameClient c, ServerSockets.Packet stream, bool gold, bool box, out List<RItem> wonOut)
        {
            var cfg = Cfg;
            string result;
            wonOut = new List<RItem>();
            lock (_lock)
            {
                uint coin = gold ? cfg.GoldCoin : cfg.SilverCoin;
                int wishes = gold ? 10 : 1;
                var won = new List<RItem>();
                for (int i = 0; i < wishes; i++) { var r = Roll(cfg.WishPool); if (r != null) won.Add(r); }
                if (CountItem(c, coin) < 1) result = "You don`t have a " + ItemName(coin) + ".";
                else if (!Space(c, SlotsFor(Merge(won)))) result = "Please make " + SlotsFor(Merge(won)) + " free space(s) in your inventory first.";
                else if (!TakeItem(c, coin, 1, stream)) result = "You don`t have a " + ItemName(coin) + ".";
                else
                {
                    FreezeIfDue();
                    var p = State(c);
                    ulong add = (ulong)(cfg.WishPoints * wishes);
                    p.Points += add; p.PointsTicks = Now.Ticks; _dirty = true;
                    SaveState();
                    won = Merge(won);// one stack per item kind (10 wishes of the same item = one bag slot)
                    string given = Give(c, won, stream);
                    wonOut = won;
                    Log(c, "wish " + (gold ? "gold" : "silver") + " +" + add + " pts (now " + p.Points + ") got [" + given + "]");
                    result = "You received: " + given + ". Increased " + add + " Theater Points for the Dream Theater (" + p.Points + ").";
                }
            }
            if (box) Box(c, stream, result);
            return result;
        }

        // ---- Upgrade
        static void UpgradeMenu(Client.GameClient c, Game.MsgNpc.Dialog d, Dlg st, PState p)
        {
            var cfg = Cfg;
            if (p.Level >= cfg.MaxLevel) { Text(d, "Dream Theater has reached its highest level (Lv." + p.Level + ")."); Opt(d, st, "Back.", 177).AddAvatar(10).FinalizeDialog(); return; }
            var a = cfg.LevelOf(p.Level); var b = cfg.LevelOf(p.Level + 1);
            Text(d, "Theater Upgrade: The ancient Dream Theater, which has a history of a hundred years, has many fans and loyal customers. After the theater is upgraded, it will be renovated and attract more audience, bringing more profits.");
            Text(d, "Lv" + a.Lv + " -> Lv" + b.Lv + ". Upgrade Effect: Silver Coin Bonus " + a.Silver + " -> " + b.Silver + ", Theater Point Bonus " + a.Points + " -> " + b.Points + ". Consumed Fund: " + p.Funds + "/" + a.UpgradeFunds + ".");
            Opt(d, st, "Upgrade.", 183);
            Opt(d, st, "Back.", 177).AddAvatar(10).FinalizeDialog();
        }
        public static string Upgrade(Client.GameClient c, ServerSockets.Packet stream)
        {
            if (stream == null) { using (var rec = new ServerSockets.RecycledPacket()) return UpgradeCore(c, rec.GetStream(), false); }
            return UpgradeCore(c, stream, true);
        }
        static string UpgradeCore(Client.GameClient c, ServerSockets.Packet stream, bool box)
        {
            var cfg = Cfg;
            string result;
            lock (_lock)
            {
                var p = State(c);
                var a = cfg.LevelOf(p.Level);
                if (p.Level >= cfg.MaxLevel) result = "Dream Theater has reached its highest level.";
                else if (p.Funds < a.UpgradeFunds) result = "Failed to upgrade due to insufficient funds. You can earn some Theater Funds from daily performances.";
                else
                {
                    p.Funds -= a.UpgradeFunds; p.Level++; _dirty = true;
                    SaveState();
                    var b = cfg.LevelOf(p.Level);
                    Log(c, "upgrade to Lv" + p.Level + " (-" + a.UpgradeFunds + " funds)");
                    result = "Upgraded! Dream Theater is now Lv." + p.Level + ": Silver Coin Bonus " + b.Silver + ", Theater Point Bonus " + b.Points + ".";
                }
            }
            if (box) Box(c, stream, result);
            return result;
        }

        // ---- Theater Points rewards (milestones)
        static void PointsMenu(Client.GameClient c, Game.MsgNpc.Dialog d, Dlg st, PState p)
        {
            var cfg = Cfg;
            var sb = new StringBuilder("Theater Points: " + p.Points + ". ");
            for (int i = 0; i < cfg.Milestones.Count; i++)
            {
                var m = cfg.Milestones[i];
                sb.Append("With " + m.Points + ": " + string.Join(" + ", m.Items.Select(x => ItemName(x.Id) + (x.Count > 1 ? " x" + x.Count : ""))) + (p.Claimed.Contains(m.Points) ? " [claimed]. " : ". "));
            }
            ulong top = cfg.Milestones.Count > 0 ? cfg.Milestones[cfg.Milestones.Count - 1].Points : 0;
            if (top > 0 && cfg.ExtraItems.Count > 0) sb.Append("With " + top / 1000 + "K+ pts, for every " + cfg.ExtraEvery / 1000 + "K more pts: " + string.Join(" + ", cfg.ExtraItems.Select(x => ItemName(x.Id))) + ".");
            Text(d, sb.ToString());
            int shown = 0;
            for (int i = 0; i < cfg.Milestones.Count && shown < 6; i++)
            {
                var m = cfg.Milestones[i];
                if (p.Points >= m.Points && !p.Claimed.Contains(m.Points)) { Opt(d, st, "Claim: with " + m.Points + " Theater Points.", (byte)(190 + i)); shown++; }
            }
            if (ExtraDue(p) > 0) Opt(d, st, "Claim: " + top / 1000 + "K+ reward (x" + ExtraDue(p) + ").", 198);
            Opt(d, st, "Back.", 177).AddAvatar(10).FinalizeDialog();
        }
        static int ExtraDue(PState p)
        {
            var cfg = Cfg;
            if (cfg.Milestones.Count == 0 || cfg.ExtraItems.Count == 0) return 0;
            ulong top = cfg.Milestones[cfg.Milestones.Count - 1].Points;
            if (p.Points < top + cfg.ExtraEvery) return 0;
            int due = (int)Math.Min(1000, (p.Points - top) / cfg.ExtraEvery);
            return Math.Max(0, due - p.ExtraClaimed);
        }
        /// <summary>Claims one milestone row (190-197) or one 120K+ step (198). Recorded before the items are given.</summary>
        public static string ClaimMilestone(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (stream == null) { using (var rec = new ServerSockets.RecycledPacket()) return ClaimMilestoneCore(c, rec.GetStream(), option, false); }
            return ClaimMilestoneCore(c, stream, option, true);
        }
        static string ClaimMilestoneCore(Client.GameClient c, ServerSockets.Packet stream, byte option, bool box)
        {
            var cfg = Cfg;
            string result;
            lock (_lock)
            {
                var p = State(c);
                List<RItem> items; string what;
                if (option == 198)
                {
                    if (ExtraDue(p) <= 0) { result = "Nothing to claim yet."; goto done; }
                    items = cfg.ExtraItems; what = "extra " + (p.ExtraClaimed + 1);
                    if (!Space(c, SlotsFor(items))) { result = "Please make " + SlotsFor(items) + " free space(s) in your inventory first."; goto done; }
                    p.ExtraClaimed++;
                }
                else
                {
                    int i = option - 190;
                    if (i < 0 || i >= cfg.Milestones.Count) { result = "That reward is gone."; goto done; }
                    var m = cfg.Milestones[i];
                    if (p.Points < m.Points) { result = "Failed to claim rewards for lack of Theater Points. You can earn more Theater Points through [Upgrading the Theater] and [Daily Performances]!"; goto done; }
                    if (p.Claimed.Contains(m.Points)) { result = "You already claimed this reward."; goto done; }
                    items = m.Items; what = "milestone " + m.Points;
                    if (!Space(c, SlotsFor(items))) { result = "Please make " + SlotsFor(items) + " free space(s) in your inventory first."; goto done; }
                    p.Claimed.Add(m.Points);
                }
                _dirty = true;
                SaveState();
                string given = Give(c, items, stream);
                Log(c, "claim " + what + " got [" + given + "]");
                result = "You received: " + given + ".";
            }
        done:
            if (box) Box(c, stream, result);
            return result;
        }

        // ---- local ranking
        /// <summary>The settle time of the running edition (the year the run started).</summary>
        static DateTime SettleTime()
        {
            var cfg = Cfg;
            int y = _season > 0 ? _season : Now.Year;
            return new DateTime(y, cfg.SettleMonth, Math.Min(cfg.SettleDay, DateTime.DaysInMonth(y, cfg.SettleMonth))).AddMinutes(cfg.SettleMinute);
        }
        /// <summary>Settled = the run started before the settle time and it is past now (a run forced on after Mar 25 never settles).</summary>
        static bool Settled
        {
            get
            {
                var r = Festivals.FestivalManager.Running(Key);
                DateTime started = SettledStartOverride ?? (_seasonStartTicks > 0 ? new DateTime(_seasonStartTicks) : r != null ? r.Started : DateTime.MaxValue);
                var s = SettleTime();
                return started <= s && Now >= s;
            }
        }
        /// <summary>Test seam: the run start when the festival is not really running (harness).</summary>
        public static DateTime? SettledStartOverride;
        /// <summary>Top 10 (>= rankMinPoints). After the settle time the order is frozen once (later points do not count).</summary>
        /// <summary>Freezes the top 10 once the settle time has passed (called before any point gain, by the timer and the menus).</summary>
        static void FreezeIfDue()
        {
            var cfg = Cfg;
            lock (_lock)
            {
                Season();
                if (_frozen != null || !Settled) return;
                _frozen = _players.Values.Where(p => p.Points >= cfg.RankMinPoints).OrderByDescending(p => p.Points).ThenBy(p => p.PointsTicks).Take(10).Select(p => p.Uid).ToList();
                _dirty = true;
                SaveState();
            }
        }
        /// <summary>Top 10 (>= rankMinPoints). After the settle time the frozen order (later points do not count).</summary>
        public static List<PState> Ranking()
        {
            var cfg = Cfg;
            FreezeIfDue();
            lock (_lock)
            {
                if (Settled && _frozen != null)
                    return _frozen.Select(u => { PState p; return _players.TryGetValue(u, out p) ? p : null; }).Where(p => p != null).ToList();
                return _players.Values.Where(p => p.Points >= cfg.RankMinPoints).OrderByDescending(p => p.Points).ThenBy(p => p.PointsTicks).Take(10).ToList();
            }
        }
        static void RankMenu(Client.GameClient c, Game.MsgNpc.Dialog d, Dlg st, PState p)
        {
            var cfg = Cfg;
            var rank = Ranking();
            int mine = rank.FindIndex(x => x.Uid == p.Uid) + 1;
            var sb = new StringBuilder("Local Dream Ranking: ");
            if (rank.Count == 0) sb.Append("No one has achieved a ranking. ");
            for (int i = 0; i < rank.Count; i++) sb.Append("No." + (i + 1) + " " + rank[i].Name + " " + rank[i].Points + (i + 1 < rank.Count ? ", " : ". "));
            sb.Append("My Rank: " + (mine > 0 ? "No." + mine : "Unranked") + " (" + p.Points + "). You need at least " + cfg.RankMinPoints + " Theater Points to get listed on the local ranking. The ranking will be settled at " + SettleTime().ToString("HH:mm, MMM. d", CultureInfo.InvariantCulture) + ", and rewards will be given according to your rank.");
            Text(d, sb.ToString());
            Text(d, "Rank Prizes: " + string.Join("; ", cfg.RankRewards.Select(r => (r.From == r.To ? "No." + r.From : "No." + r.From + "-" + r.To) + " " + r.Box)) + ".");
            if (Settled && mine > 0 && !p.RankPaid) Opt(d, st, "Claim my ranking reward (No." + mine + ").", 184);
            Opt(d, st, "Back.", 177).AddAvatar(10).FinalizeDialog();
        }
        /// <summary>[DreamTheater 2026-10-15] Official tWhiteDayLua Email: once the ranking is frozen every listed player gets a system mail
        /// (title "Dream Ranking", Content[n]) with 1 Dream Splendid/Starry/Glowy/Lucky Box, through the mailbox engine
        /// (Base\AdminApi\AdminApi.Compensate.cs CompEngine: once per player in MySQL, queued for offline players). Sent once a season.</summary>
        public static int MailRanking()
        {
            var cfg = Cfg;
            List<PState> rank;
            lock (_lock)
            {
                if (_mailed || _frozen == null || !Settled) return 0;
                rank = _frozen.Select(u => { PState p; return _players.TryGetValue(u, out p) ? p : null; }).Where(p => p != null).ToList();
                _mailed = true; _dirty = true;
            }
            int sent = 0;
            for (int i = 0; i < rank.Count && i < 10; i++)
            {
                int no = i + 1;
                var rr = cfg.RankRewards.FirstOrDefault(r => no >= r.From && no <= r.To);
                if (rr == null || rr.Items.Count == 0) continue;
                try
                {
                    var camp = new Compensation.Campaign
                    {
                        Id = "DreamTheater" + _season + "r" + no,
                        Title = "Dream Ranking",
                        Body = "You were ranked No." + no + " on the Local Dream Ranking and received 1 [" + rr.Box + "].",
                        Created = DateTime.Now, Expires = DateTime.Now.AddDays(30), CreatedBy = "DreamTheater", TargetText = rank[i].Name + " (No." + no + ")", Targets = 1,
                    };
                    foreach (var it in rr.Items) camp.Reward.Items.Add(new Compensation.CompItem { Id = it.Id, Count = it.Count, Bound = it.Bound });
                    string err = Compensation.CompEngine.Create(camp);
                    if (err != null) { MyConsole.WriteLine("[DreamTheater] ranking mail No." + no + ": " + err, ConsoleColor.Yellow); continue; }
                    Compensation.CompEngine.SendAll(camp, new List<KeyValuePair<uint, string>> { new KeyValuePair<uint, string>(rank[i].Uid, rank[i].Name) });
                    lock (_lock) { rank[i].RankPaid = true; _dirty = true; }
                    try { Festivals.FestivalManager.Log("THEATER", Key, rank[i].Uid + " " + rank[i].Name + " | ranking No." + no + " (" + rank[i].Points + " pts) mailed " + rr.Box); } catch { }
                    sent++;
                }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            }
            SaveState();
            return sent;
        }
        /// <summary>Dev test seam (AdminApi festtest "dtday"): moves the module clock n days forward (0 = back to the real clock).</summary>
        public static string TestShiftDays(int n)
        {
            if (n <= 0) { ClockOverride = null; return "clock real"; }
            ClockOverride = () => DateTime.Now.AddDays(n);
            return "clock +" + n + " day(s): " + Now.ToString("yyyy-MM-dd");
        }
        /// <summary>Dev test seam (AdminApi festtest "dtpoints"): sets the player's Theater Points (ranking / milestone tests).</summary>
        public static string TestSetPoints(Client.GameClient c, long pts)
        {
            var p = State(c);
            lock (_lock) { p.Points = (ulong)Math.Max(0, pts); p.PointsTicks = Now.Ticks; _dirty = true; }
            SaveState();
            return "points " + p.Points;
        }
        /// <summary>Dev test seam (AdminApi festtest "dtsettle"): treats the running edition as settled now (freeze + mails).</summary>
        public static string TestSettle()
        {
            SettledStartOverride = DateTime.MinValue;
            var old = ClockOverride;
            try
            {
                var st = SettleTime().AddMinutes(1);
                ClockOverride = () => st;
                FreezeIfDue();
                return "frozen " + (_frozen == null ? 0 : _frozen.Count) + ", mailed " + MailRanking();
            }
            finally { ClockOverride = old; SettledStartOverride = null; }
        }
        public static string ClaimRank(Client.GameClient c, ServerSockets.Packet stream)
        {
            if (stream == null) { using (var rec = new ServerSockets.RecycledPacket()) return ClaimRankCore(c, rec.GetStream(), false); }
            return ClaimRankCore(c, stream, true);
        }
        static string ClaimRankCore(Client.GameClient c, ServerSockets.Packet stream, bool box)
        {
            var cfg = Cfg;
            string result;
            lock (_lock)
            {
                var p = State(c);
                var rank = Ranking();
                int mine = rank.FindIndex(x => x.Uid == p.Uid) + 1;
                var rr = cfg.RankRewards.FirstOrDefault(r => mine >= r.From && mine <= r.To);
                if (!Settled) result = "The ranking is not settled yet.";
                else if (mine <= 0 || rr == null) result = "You are not on the local ranking.";
                else if (p.RankPaid) result = "You already received your ranking reward.";
                else if (!Space(c, SlotsFor(rr.Items))) result = "Please make " + SlotsFor(rr.Items) + " free space(s) in your inventory first.";
                else
                {
                    p.RankPaid = true; _dirty = true;
                    SaveState();
                    string given = Give(c, rr.Items, stream);
                    Log(c, "rank No." + mine + " (" + p.Points + " pts) got [" + given + "]");
                    result = "You were ranked No." + mine + " on the Local Dream Ranking and received the contents of 1 [" + rr.Box + "]: " + given + ".";
                }
            }
            if (box) Box(c, stream, result);
            return result;
        }

        // ------------------------------------------------------------------ Odette: the daily performance
        static int Today { get { var n = Now; return n.Year * 10000 + n.Month * 100 + n.Day; } }

        // ------------------------------------------------------------------ the window's daily contract (official TaskIndex)
        static readonly uint[] TroubleQuests = { 5753, 5754, 5755, 5756, 5757, 5758, 5759, 5760, 5761 };
        /// <summary>The middle quests of a recruitment segment (5736/5803 of Irwin, 5739 of Daisy, 5744 of Oliver, 5748 of Jenkins).</summary>
        static uint[] SegmentMiddle(int idx)
        {
            return idx == 1 ? new uint[] { 5736, 5803 } : idx == 2 ? new uint[] { 5739 } : idx == 3 ? new uint[] { 5744 } : idx == 6 ? new uint[] { 5748 } : new uint[0];
        }
        /// <summary>The quest(s) of a contract are done: recruitment k = its segment end; 8 = a cleaning quest; 9 = a trouble quest.</summary>
        static bool ContractQuestsDone(Client.GameClient c, int idx)
        {
            var cfg = Cfg;
            if (idx >= 1 && idx <= 7) { var s = cfg.Segments.FirstOrDefault(x => x.Index == idx); return s != null && Finished(c, s.End); }
            if (idx == 8) return cfg.CleanQuests.Any(q => Finished(c, q));
            if (idx == 9) return cfg.TroubleDone.Any(q => Finished(c, q));
            return false;
        }
        /// <summary>Some quest of the contract is running or finished.</summary>
        static bool ContractQuestsLive(Client.GameClient c, int idx)
        {
            var cfg = Cfg;
            if (idx >= 1 && idx <= 7)
            {
                var s = cfg.Segments.FirstOrDefault(x => x.Index == idx);
                if (s == null) return false;
                return Active(c, s.Start) || Finished(c, s.Start) || Active(c, s.End) || Finished(c, s.End) || SegmentMiddle(idx).Any(q => Active(c, q) || Finished(c, q));
            }
            if (idx == 8) return cfg.CleanQuests.Any(q => Active(c, q) || Finished(c, q));
            if (idx == 9) return TroubleQuests.Any(q => Active(c, q) || Finished(c, q));
            return false;
        }
        static bool ContractQuestsActive(Client.GameClient c, int idx)
        {
            var cfg = Cfg;
            if (idx >= 1 && idx <= 7) { var s = cfg.Segments.FirstOrDefault(x => x.Index == idx); return s != null && (Active(c, s.Start) || Active(c, s.End) || SegmentMiddle(idx).Any(q => Active(c, q))); }
            if (idx == 8) return cfg.CleanQuests.Any(q => Active(c, q));
            if (idx == 9) return TroubleQuests.Any(q => Active(c, q));
            return false;
        }
        /// <summary>Brings the contract record up to date: done once its quest(s) finished; a cleaning / trouble contract whose
        /// daily quest was reset (or a recruitment that was abandoned) is dropped so Agnes offers a new one.</summary>
        static void Refresh(Client.GameClient c, PState p)
        {
            lock (_lock)
            {
                if (p.ContractIdx == 0 && p.PerfDay != Today)
                {
                    // a contract quest already running that has no record (taken before this version / by another path): adopt it
                    for (int i = 1; i <= 9 && p.ContractIdx == 0; i++)
                        if (ContractQuestsLive(c, i) && !ContractQuestsDone(c, i) && ContractQuestsActive(c, i)) { p.ContractIdx = i; p.ContractDone = false; _dirty = true; }
                    return;
                }
                if (p.ContractIdx == 0 || p.ContractDone) return;
                if (ContractQuestsDone(c, p.ContractIdx)) { p.ContractDone = true; _dirty = true; return; }
                if (!ContractQuestsLive(c, p.ContractIdx)) { p.ContractIdx = 0; _dirty = true; }
            }
        }
        /// <summary>The next recruitment Agnes can offer (1-7), 0 = all recruited, -1 = one is still running.</summary>
        static int NextRecruit(Client.GameClient c)
        {
            foreach (var s in Cfg.Segments)
            {
                if (Finished(c, s.End)) continue;
                return ContractQuestsLive(c, s.Index) ? -1 : s.Index;
            }
            return 0;
        }
        /// <summary>Official TaskIndex of DlgWhiteDayMain: 1-7 recruitment, 8 cleaning, 9 troubles, 0 = nothing to take now
        /// (a contract is taken and not yet performed, or today's performance is over).</summary>
        static int TaskIndex(Client.GameClient c)
        {
            var p = State(c);
            Refresh(c, p);
            if (p.ContractIdx != 0 || p.PerfDay == Today) return 0;
            int r = NextRecruit(c);
            if (r != 0) return r < 0 ? 0 : r;
            if (Cfg.DailyAfterRecruit == "cleaning") return 8;
            if (Cfg.DailyAfterRecruit == "troubles") return 9;
            return ((int)(Now.Date - new DateTime(2000, 1, 1)).TotalDays) % 2 == 0 ? 8 : 9;// DERIVED alternate
        }
        /// <summary>The performance is ready: that day's contract is done and today's performance was not given yet.</summary>
        static bool PerformanceReady(Client.GameClient c)
        {
            var p = State(c);
            Refresh(c, p);
            lock (_lock) return p.ContractIdx != 0 && p.ContractDone && p.PerfDay != Today && Finished(c, Cfg.UnlockQuest);
        }
        static int Members(Client.GameClient c) { return Cfg.Members.Count(q => Finished(c, q)); }
        static ulong PerfPoints(Client.GameClient c, PState p) { var cfg = Cfg; return (ulong)Math.Max(0, cfg.LevelOf(p.Level).Points + cfg.PointsPerMember * Members(c)); }
        static bool OdetteMain(Client.GameClient c, ServerSockets.Packet stream)
        {
            // [DreamTheater 2026-10-15 video pass] official Odette script tWhiteDay2024Event[32653] (Text111-113 / Option111 / Text1211-1212).
            // The performance itself is the window's "Enter Theater" (videos qiOK 3:00 / 9ZTy 7:30), not an Odette option.
            if (!Finished(c, Cfg.UnlockQuest)) return false;
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, Odette);
            Text(d, "Odette: [Love is everywhere, like the sunlight, like the air, mysterious and noble...] Sorry, I was still immersed in the opera. It was rude of me.");
            Opt(d, st, "Your performance was fantastic!", 200);
            Opt(d, st, "Goodbye.", 255).AddAvatar(10).FinalizeDialog();
            return true;
        }
        static void OdetteChoice(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (option != 200) return;
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, Odette);
            Text(d, "Odette: Thank you for your compliment. I will continue to work hard! May I ask why you came here?");
            Opt(d, st, "Goodbye.", 255).AddAvatar(10).FinalizeDialog();
        }
        /// <summary>The daily performance (window "Settle"): once a day after that day's contract. Settlement (videos qiOK 3:14-3:56,
        /// 9ZTy 7:50 / 8:56): Theater Points = level points + 30 per member (split 50/30/20), Theater Funds of the level, and
        /// Wish Silver Coins = level silver + 1 per member (DlgWhiteDayActFinishTwo).</summary>
        public static string Perform(Client.GameClient c, ServerSockets.Packet stream)
        {
            if (stream == null) { using (var rec = new ServerSockets.RecycledPacket()) return PerformCore(c, rec.GetStream(), false); }
            return PerformCore(c, stream, true);
        }
        /// <summary>Last settlement per player for the second settlement window (members silver, level silver, play count).</summary>
        static readonly ConcurrentDictionary<uint, int[]> _lastSettle = new ConcurrentDictionary<uint, int[]>();
        static string PerformCore(Client.GameClient c, ServerSockets.Packet stream, bool box)
        {
            var cfg = Cfg;
            string result;
            bool ready = PerformanceReady(c);
            lock (_lock)
            {
                var p = State(c);
                var lv = cfg.LevelOf(p.Level);
                int members = Members(c);
                int silverMembers = Math.Max(0, cfg.SilverPerMember * members);
                var coins = new List<RItem> { new RItem { Id = cfg.SilverCoin, Count = (ushort)Math.Max(0, lv.Silver + silverMembers) } };
                if (!Finished(c, cfg.UnlockQuest)) result = "The performance has not started today. You can go and talk to Agnes first!";
                else if (p.PerfDay == Today) result = "Today`s performance is over. Please come back tomorrow!";
                else if (!ready) result = "Complete the tasks before the performance.";
                else if (!Space(c, SlotsFor(coins))) result = "Please make " + SlotsFor(coins) + " free space(s) in your inventory first.";
                else
                {
                    FreezeIfDue();
                    ulong pts = PerfPoints(c, p);
                    p.PerfDay = Today; p.Points += pts; p.PointsTicks = Now.Ticks; p.Funds += lv.Funds; p.PlayNum++;
                    p.ContractIdx = 0; p.ContractDone = false; _dirty = true;
                    SaveState();
                    string given = coins[0].Count > 0 ? Give(c, coins, stream) : "";
                    _lastSettle[c.Player.UID] = new[] { silverMembers, lv.Silver, p.PlayNum };
                    Log(c, "performance Lv" + p.Level + " members=" + members + " +" + pts + " pts +" + lv.Funds + " funds play#" + p.PlayNum + " got [" + given + "]");
                    result = "This Performance: Marvelous. Theater Points Increased " + pts + ", Funds Obtained " + lv.Funds + "." + (given.Length > 0 ? " Received: " + given + "." : "");
                }
            }
            if (box) Box(c, stream, result);
            return result;
        }

        // ------------------------------------------------------------------ click mini-games
        // [DreamTheater 2026-10-15] the spots already done live in the player's saved state (relog / restart cannot reset them)
        static bool Has(Client.GameClient c, string key) { var p = State(c); lock (_lock) return p.Marks.Contains(key); }
        static void Mark(Client.GameClient c, string key) { var p = State(c); lock (_lock) { p.Marks.Add(key); _dirty = true; } SaveState(); }
        static void Forget(Client.GameClient c, string prefix) { var p = State(c); lock (_lock) { if (p.Marks.RemoveWhere(k => k.StartsWith(prefix, StringComparison.Ordinal)) > 0) _dirty = true; } }

        /// <summary>The official progress bar ("Collecting.....") then the effect, if the player stood still next to the NPC.</summary>
        static void Pick(Client.GameClient c, ServerSockets.Packet stream, string bar, uint npc, Action<ServerSockets.Packet> effect)
        {
            if (c.Player.ActivePick) return;
            int delay = PickDelayMs;
            if (delay <= 0) { effect(stream); return; }
            uint map = c.Player.Map, dyn = c.Player.DynamicID; ushort x = c.Player.X, y = c.Player.Y;
            c.Player.AddPick(stream, bar, (ushort)Math.Max(1, delay / 1000));
            System.Threading.Tasks.Task.Delay(delay).ContinueWith(_ =>
            {
                try
                {
                    if (c.Player == null || !c.Player.ActivePick || c.Player.Map != map || c.Player.DynamicID != dyn || c.Player.X != x || c.Player.Y != y) return;
                    c.Player.ActivePick = false;
                    if (!Running) return;
                    using (var rec = new ServerSockets.RecycledPacket()) effect(rec.GetStream());
                }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            });
        }

        // 5739: 3 herbs
        static bool Herb(Client.GameClient c, ServerSockets.Packet stream, uint npc)
        {
            if (!Active(c, QHerb)) return false;
            uint have = SlotValue(c, QHerb);
            if (have >= 3) return false;// complete: the engine shows "Complete" at the Balloon Flower
            if (have == 0) Forget(c, "herb:");
            string key = "herb:" + npc;
            if (Has(c, key)) { Box(c, stream, "You have collected this herb. Please go and collect the next herb!"); return true; }
            int idx = Array.IndexOf(HerbNpcs, npc);
            Pick(c, stream, "Collecting.....", npc, s =>
            {
                if (!Active(c, QHerb) || Has(c, key)) return;
                Mark(c, key);
                QuestEngine.QuestEngineCore.OnEvent(c, EvHerb);
                c.CreateBoxDialog(SlotValue(c, QHerb) >= 3 ? "These herbs are enough to cure Daisy`s voice. Please go and give them to Daisy!" : HerbMsg[Math.Max(0, idx)]);
            });
            return true;
        }

        // 5744: the first horse kicks you away, the other one is tamed; the quest ends at the horse (Questinfo FinishNpcId -1)
        static bool Horse(Client.GameClient c, ServerSockets.Packet stream, uint npc)
        {
            if (!Active(c, QHorse)) return false;
            uint have = SlotValue(c, QHorse);
            if (have == 0) Forget(c, "horse:");
            if (Has(c, "horse:" + npc)) { Box(c, stream, "The Untamed Horses here are difficult to tame. Go and see elsewhere!"); return true; }
            Pick(c, stream, "Taming the horse...", npc, s =>
            {
                if (!Active(c, QHorse) || Has(c, "horse:" + npc)) return;
                Mark(c, "horse:" + npc);
                QuestEngine.QuestEngineCore.OnEvent(c, EvHorse);
                if (SlotValue(c, QHorse) < 2) { c.CreateBoxDialog("This horse is very unruly and kicked you away. It seems unsuitable for Oliver. Let`s go find a gentler horse!"); return; }
                FinishHere(c, s, QHorse, QHorseNext, "You have successfully tamed this horse. Now give it to Oliver!");
            });
            return true;
        }

        // cleaning: 3 of the 4 spots, then the quest ends on the spot
        static bool Clean(Client.GameClient c, ServerSockets.Packet stream, uint npc)
        {
            var cfg = Cfg;
            uint qid = cfg.CleanQuests.FirstOrDefault(q => Active(c, q));
            if (qid == 0) return false;
            string day = "clean:" + Today + ":";
            if (SlotValue(c, qid) == 0) Forget(c, "clean:");
            if (Has(c, day + npc)) { Box(c, stream, "You have cleaned this place. Go elsewhere to clean!"); return true; }
            int idx = Array.IndexOf(CleanNpcs, npc);
            Pick(c, stream, "Cleaning...", npc, s =>
            {
                if (!Active(c, qid) || Has(c, day + npc)) return;
                Mark(c, day + npc);
                QuestEngine.QuestEngineCore.OnEvent(c, EvClean);
                if (SlotValue(c, qid) < 3) { c.CreateBoxDialog(CleanMsg[Math.Max(0, idx)]); return; }
                FinishHere(c, s, qid, 0, "Thanks to you, the Dream Theater has been completely refreshed! The performance can be prepared now!");
            });
            return true;
        }

        // 5754: the reading glasses are in one of the two corners
        static bool Glasses(Client.GameClient c, ServerSockets.Packet stream, uint npc)
        {
            if (!Active(c, QGlasses)) return false;
            uint have = SlotValue(c, QGlasses);
            if (have == 0) Forget(c, "glass:");
            if (have >= 2) { Box(c, stream, "After searching for a while, you found a pair of reading glasses. Quickly hand it over to Irwin!"); return true; }
            if (Has(c, "glass:" + npc)) { Box(c, stream, "This place has been searched. Let`s look for it somewhere else!"); return true; }
            Pick(c, stream, "Searching...", npc, s =>
            {
                if (!Active(c, QGlasses) || Has(c, "glass:" + npc)) return;
                Mark(c, "glass:" + npc);
                QuestEngine.QuestEngineCore.OnEvent(c, EvGlasses);
                c.CreateBoxDialog(SlotValue(c, QGlasses) >= 2 ? "After searching for a while, you found a pair of reading glasses. Quickly hand it over to Irwin!" : "It seems that this place has been cleaned up. Let`s go to search the next place!");
            });
            return true;
        }

        /// <summary>Engine finish of a quest whose official finish is "on the spot" (no finish NPC), then the next quest.</summary>
        static void FinishHere(Client.GameClient c, ServerSockets.Packet stream, uint id, uint next, string text)
        {
            var q = Q(id);
            string msg = "";
            if (q == null || !QuestEngine.QuestEngineCore.TryFinish(c, q, stream, 0, out msg)) { c.CreateBoxDialog(q == null ? "That quest is gone." : msg); return; }
            Log(c, "finished " + id + " on the spot: " + msg);
            if (next != 0)
            {
                Database.QuestInfo.DBQuest dbq; string why;
                if (Database.QuestInfo.AllQuests.TryGetValue(next, out dbq) && QuestEngine.QuestEngineCore.CanAccept(c, dbq, out why))
                {
                    c.Player.QuestGUI.Accept(dbq, 0);
                    if (c.Player.QuestGUI.IsActiveQuest(next)) QuestEngine.QuestEngineCore.AfterPanelAccept(c, next);
                }
            }
            c.CreateBoxDialog(text + (msg.Length > 0 ? " " + msg : ""));
        }

        // ------------------------------------------------------------------ 5760 Knight`s Covenant (duel)
        sealed class Duel { public Game.MsgMonster.MonsterRole Mob; public DateTime Expires; }
        static readonly ConcurrentDictionary<uint, Duel> _duels = new ConcurrentDictionary<uint, Duel>();
        static bool DuelAlive(Client.GameClient c) { Duel d; return _duels.TryGetValue(c.Player.UID, out d) && d.Mob != null && d.Mob.Alive && Now < d.Expires; }
        static bool JenkinsMain(Client.GameClient c, ServerSockets.Packet stream)
        {
            if (!Active(c, QDuel) || SlotValue(c, QDuel, 2) >= 1 || DuelAlive(c)) return false;// the engine handles progress / finish
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, TheaterJenkins);
            Text(d, "Jenkins: Do you still remember our promise?");
            Opt(d, st, "Of course I do!", 210);
            Opt(d, st, "Later.", 255).AddAvatar(10).FinalizeDialog();
            return true;
        }
        static void JenkinsChoice(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, TheaterJenkins);
            if (option == 210)
            {
                Text(d, "Jenkins: These days, I have been practicing swordsmanship after rehearsing for the opera. Now I am ready to challenge you!");
                Opt(d, st, "I accept your challenge!", 211);
                Opt(d, st, "Later.", 255).AddAvatar(10).FinalizeDialog();
                return;
            }
            if (option == 211)
            {
                _dlg.TryRemove(c.Player.UID, out st);
                if (!Active(c, QDuel) || DuelAlive(c)) return;
                Box(c, stream, SpawnDuel(c, stream) ? "Jenkins accepted your challenge! Defeat him!" : "Not now. Try again a little later.");
            }
        }
        /// <summary>Test seam: the harness has no running map.</summary>
        public static Func<Client.GameClient, bool> SpawnDuelOverride;
        static bool SpawnDuel(Client.GameClient c, ServerSockets.Packet stream)
        {
            var o = SpawnDuelOverride;
            if (o != null) return o(c);
            var fam = Pool.MonsterFamilies == null ? null : Pool.MonsterFamilies.Values.FirstOrDefault(f => f != null && string.Equals(f.Name, "KnightJenkins", StringComparison.OrdinalIgnoreCase));
            var map = c.Map;
            if (fam == null || map == null) return false;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return false;
            var copy = fam.Copy();
            copy.SpawnX = (ushort)Math.Max(1, c.Player.X - 2); copy.SpawnY = (ushort)Math.Max(1, c.Player.Y - 2);
            copy.MaxSpawnX = (ushort)(copy.SpawnX + 4); copy.MaxSpawnY = (ushort)(copy.SpawnY + 4);
            copy.MapID = map.ID; copy.SpawnCount = 1;
            Game.MsgMonster.MonsterRole mob = null;
            for (int i = 0; i < 4 && mob == null; i++) mob = map.MonstersColletion.Add(copy, true, c.Player.DynamicID, true);
            if (mob == null) return false;
            mob.RemoveOnDead = true;
            _duels[c.Player.UID] = new Duel { Mob = mob, Expires = Now.AddMinutes(10) };
            try
            {
                mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                mob.Send(mob.GetArray(stream, false));
            }
            catch { }
            Log(c, "duel KnightJenkins spawned (copy " + c.Player.DynamicID + ")");
            return true;
        }
        static void SweepDuels()
        {
            foreach (var kv in _duels.ToArray())
            {
                var d = kv.Value;
                Client.GameClient owner;
                bool gone = d.Mob == null || !d.Mob.Alive;
                bool expired = Now > d.Expires || !Pool.GamePoll.TryGetValue(kv.Key, out owner) || owner.Player == null || !owner.Player.QuestGUI.IsActiveQuest(QDuel) || !Running;
                if (!gone && !expired) continue;
                Duel r;
                if (!_duels.TryRemove(kv.Key, out r) || gone) continue;
                try
                {
                    var mob = r.Mob;
                    mob.RemoveOnDead = true;
                    bool was = mob.Alive;
                    mob.HitPoints = 0;
                    using (var rec = new ServerSockets.RecycledPacket())
                        mob.Send(rec.GetStream().ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                    var gm = mob.GMap;
                    if (gm != null) { gm.View.LeaveMap<Role.IMapObj>(mob); if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false); }
                }
                catch { }
            }
        }
    }
}
