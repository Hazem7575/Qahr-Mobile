// ============================================================================
//  [AprilFools2023 2026-10-15] April Fools 2023 "Tricky Wit" - the Trick Maze (client tMsg_Info["HalloweenMaze"] = 10113;
//  the CN files are the Halloween 2022 maze ERP516962, the EN client text is April Fools 2023: Trick Maze / Lyon / Mad Clown).
//
//  Official client data used (luaui\dialog\dlghalloweenmaze, luaUiRes_EN 1286 / 1286-maze / 1344 / 1344-3, Server_Key
//  tHalloweenPage_DelText / tHalloween2022Part / tHalloweenNPC_DelText, itemtype "Trick Maze" items):
//    main      DlgHalloweenMazeMenuMainUi_OpenViewData|{Code=200,Data={Points,InHall}}            (HalloweenPage_MainView)
//    entrance  HalloweenPage_Entrance / HalloweenNPC_EnterSealed -> the "Floor %s Entrance" dialog (seal / explore / reset / mop up)
//    maze map  DlgHalloweenMazeMainUi_UpdateViewData|{Floor,Organize,Enter,PassRomms,LastPassRoom,TransRoom,HaveKey,PlayerRoom,
//              JokerPiece,JokerPieceState,NewJokerPiece,BossRoomLock,JokerEvent}  (halloweenmazeconfig: 3 layouts x 19 rooms, gates 100-900)
//    rooms     HalloweenPage_EnterCheck</N>room : 1 Room (3-10 Trick Amulet Fragments), 2 Corridor (-), 3 Magic Chess (8-15 Trick
//              Amulets + Ritual Blessings 5/10/20), 4 Forbidden Area (5-8 Trick Amulets, Clown Key first time, Ordeal Light, Secret
//              Passage), 20 Mystery Room (Clown Box 5/5/10/10/20/20/30/30/50 with a Clown Key, Crystal Device unseals the clown room),
//              5 Clown Room (10-15 Trick Amulets + the floor's Clown Mask + 1 Mad Clown soul fragment the first time)
//    Mad Clown Halloween2022Part_EnterFinalBoss (9 fragments): 50 Trick Amulets, 10 Joker Knight Fragments, 6 Playful Player Fragments
//    points    DlgFoolsDayPointsRankUi (FoolsDayPointsAndRankConfig["10113"]): stage rewards 500..77777 (+ every 5000), claimed on
//              msg 10311 HalloweenNpc2023_ClimbStageReward (only while this window was opened by Tricky Wit, see OnStageLua)
//  The fights take place in a private copy of NightmareMaze 11414 (the Ordeal Maze floors 11944-11952 are not in any client
//  GameMap.dat). Monster looks / stats, room spots, counts, Dungeon Breaker odds and points are DERIVED: Data\AprilFools2023\config.json.
//
//  Hooks: LuaPush.HandleCallback (10113, and 10311 for the stage claim), Procesor (virtual dialogs + Lyon), PoolProcesses (tick),
//         MsgItemUsuagePacket.UseItem (Trick Amulet / fragments / masks / packs) - see FoolsKit.cs.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.AprilFools2023
{
    public sealed class Af23Config
    {
        public string Festival = "AprilFools2023";
        public int MinLevel = 80;
        public uint MazeMap = 11414;               // DERIVED: NightmareMaze (client GameMap.dat 11414 = 2021qixi_new)
        public uint ExitMap = 1036; public ushort ExitX = 203, ExitY = 196;
        public HashSet<uint> EnterFrom = new HashSet<uint> { 1002, 1036 };
        public int CopyMinutes = 60;
        public int PointsPerAmulet = 10;           // DERIVED: Trick Points per Trick Amulet used in Dungeon Breaker
        public int ChessLength = 20, GunJump = 3, ChessSoldiers = 3;
        public uint Minion1 = 23209, Minion2 = 992302, Minion3 = 992303, Soldier = 992304, Queen = 992305, Patriarch = 992306,
            Knight = 992307, King = 992308, Master = 992309, Crystal = 992310, ClownFirst = 992311, MadClown = 992320;
        public int RoomMinions = 6, CorridorMinions = 4, Masters = 3;
        public List<Reward> Breaker = new List<Reward>
        {
            new Reward(3341223, 1, false, 20),   // Maze Key ("Get it from Dungeon Breaker" - official item text)
            new Reward(3341225, 2, false, 30),   // Trick Amulet Fragment
            new Reward(3600461, 1, false, 15),   // Maze Pearl
            new Reward(3341270, 1, false, 10),   // Perfect Dice
            new Reward(3341304, 1, false, 15),   // Joker Knight Fragment
            new Reward(3341305, 1, false, 10),   // Playful Player Fragment
        };
        // room spots on NightmareMaze (walk-checked on 2021qixi_new.dmap, open radius 7): [0] gate, [1..19] rooms, [20] Mystery Room, [21] Mad Clown
        public ushort[][] Spots =
        {
            new ushort[]{172,76}, new ushort[]{212,78}, new ushort[]{240,98}, new ushort[]{188,106}, new ushort[]{218,124}, new ushort[]{252,130},
            new ushort[]{182,140}, new ushort[]{90,156}, new ushort[]{220,158}, new ushort[]{122,168}, new ushort[]{86,190}, new ushort[]{148,192},
            new ushort[]{230,202}, new ushort[]{168,224}, new ushort[]{130,230}, new ushort[]{274,232}, new ushort[]{222,236}, new ushort[]{248,260},
            new ushort[]{360,262}, new ushort[]{282,266}, new ushort[]{214,316}, new ushort[]{256,320},
        };
    }

    public static partial class AprilFools2023Ui
    {
        public const uint LuaMsg = 10113, StageMsg = 10311;
        public const string Festival = "AprilFools2023", Tag = "AF23";
        public const uint TrickAmulet = 3341221, TrickFragment = 3341225, MazeKey = 3341223, MazePearl = 3600461, ClownKey = 3341089,
            PerfectDice = 3341270, JokerFragment = 3341304, PlayfulFragment = 3341305, MaskFirst = 3600462;
        public const uint LyonNpc = 8892001;
        public const uint DlgEntrance = 8892090, DlgChess = 8892091, DlgExit = 8892092, DlgBox = 8892093, DlgBreaker = 8892094, DlgRank = 8892095, DlgCombine = 8892096, DlgLyon = 8892097;
        public const string Ended = "The Tricky Wit event has ended. See you next time!";

        public static readonly Store State = new Store(Festival);

        // ------------------------------------------------------------------ official texts (Server_Key / luaUiRes_EN)
        static readonly string[] Clowns = { "Depressed Clown", "Vain Clown", "Flirting Clown", "Bulimic Clown", "Greedy Clown", "Lazy Clown", "Raging Clown", "Jealous Clown", "Arrogant Clown" };
        static readonly string[] XcBoss =
        {
            "Depressed Clown sighed and attacked you slowly. Please defeat him!",
            "Vain Clown is showing off by rolling his diamond ring. You decided to teach him a lesson.",
            "Flirting Clown is trying to seduce you. Please be careful.",
            "Bulimic Clown is trying to eat you up. Go defeat him!",
            "Greedy Clown is coming at you and trying to steal your treasures. Go defeat him!",
            "Lazy Clown is quite sleepy. Hurry up and defeat him!",
            "Raging Clown is quite angry. Be careful!",
            "Jealous Clown is quite aggressive. Please stay alert!",
            "Arrogant Clown wants to fight against you with only one hand. Go defeat him!",
        };
        static readonly string[] FloorText =
        {
            "You can hear the sigh of the clown. All heroes entering the maze will be cursed by Depressed Clown. Those who are cursed can only enter this floor.",
            "You can hear the sound of gold coins... All heroes entering the maze will be cursed by Vain Clown. Those who are cursed can only enter this floor.",
            "This light is too dim in the maze and vipers are crawling slowly... All heroes entering the maze will be cursed by Flirting Clown. Those who are cursed can only enter this floor.",
            "The smell of rotten food is coming from the maze, but Bulimic Clown is enjoying it. All heroes entering the maze will be cursed by Bulimic Clown. Those who are cursed can only enter this floor.",
            "Someone is grinning in the maze. All heroes entering the maze will be cursed by Greedy Clown. Those who are cursed can only enter this floor.",
            "The snoring sound is coming from the maze... All heroes entering the maze will be cursed by Lazy Clown. Those who are cursed can only enter this floor.",
            "Those creatures in the maze are quite aggressive because of Raging Clown. All heroes entering the maze will be cursed by Raging Clown. Those who are cursed can only enter this floor.",
            "Jealous Clown is watching everything in the maze. All heroes entering the maze will be cursed by Jealous Clown. Those who are cursed can only enter this floor.",
            "The monsters in the maze or even a small plant seems to have learned to look down on people. All heroes entering the maze will be cursed by Arrogant Clown. Those who are cursed can only enter this floor.",
        };
        static readonly string[] FloorReward =
        {
            "10-15 Trick Amulets and 5 bottles of Leopard Chi Liquor (B)", "10-15 Trick Amulets and 1 Ginseng Essence (B)", "10-15 Trick Amulets and 1 +3 Stone (B)",
            "10-15 Trick Amulets and 1 Bright Star Stone", "10-15 Trick Amulets and 10 Universal Rune Essences (B)", "10-15 Trick Amulets and 1 Moon Fruit",
            "10-15 Trick Amulets and 2 Awakening Item Packs", "10-15 Trick Amulets and 1 3KK Contribution Medal (B)", "10-15 Trick Amulets and 3 Peonies (B)x2/3 Marbles (B)x2",
        };
        static readonly uint[] ClownBox = { 5, 5, 10, 10, 20, 20, 30, 30, 50 };// tHalloween2022Part_DelText[31318] Treasure 1-9

        // ------------------------------------------------------------------ maze layouts (luaui halloweenmazeconfig.lua)
        sealed class Room { public int Id, Type; public int[] Connect; }
        static readonly Dictionary<int, Room>[] Layouts = BuildLayouts();
        static Dictionary<int, Room>[] BuildLayouts()
        {
            var l = new Dictionary<int, Room>[4];
            l[1] = Parse("1:1:2|2:1:1,3,4,5|3:1:2|4:2:2,6|5:2:2|6:2:4,7,10|7:1:6,8,9|8:2:7|9:1:7|10:3:6,11,13,19|11:1:10,12,14|12:1:11|13:2:10,14,15,16|14:1:11,13|15:4:13|16:2:13,17,18|17:1:16|18:1:16|19:5:10|100:0:1|400:0:18|700:0:15");
            l[2] = Parse("1:1:2|2:2:1,3|3:1:2,4,5|4:1:3,5|5:2:3,4,6,7,8|6:1:5,7,8|7:1:5,6,8|8:2:5,6,7,9|9:3:8,10,13,19|10:1:9,11,12|11:1:10|12:2:10|13:2:9,14,15|14:4:13,17|15:2:13,16|16:1:15|17:1:18,14|18:1:17|19:5:9|200:0:11|500:0:15,17|800:0:1");
            l[3] = Parse("1:1:2|2:1:1,3|3:2:2,4|4:2:3,5|5:1:4,6|6:1:5,7|7:1:6,8,9,12,19|8:1:7|9:2:7,11,12,19|10:1:11|11:1:9,10|12:3:7,9,13,14,15,19|13:2:12,14|14:4:12,13,15|15:2:12,14,16|16:1:15,17|17:2:18,16|18:1:17|19:5:7,9,12|300:0:2,4|600:0:5|900:0:16");
            return l;
        }
        static Dictionary<int, Room> Parse(string s)
        {
            var d = new Dictionary<int, Room>();
            foreach (var part in s.Split('|'))
            {
                var p = part.Split(':');
                d[int.Parse(p[0])] = new Room { Id = int.Parse(p[0]), Type = int.Parse(p[1]), Connect = p[2].Split(',').Select(int.Parse).ToArray() };
            }
            return d;
        }
        public static int Layout(int floor) { return ((Math.Max(1, Math.Min(9, floor)) - 1) % 3) + 1; }
        public static int RoomType(int floor, int room)
        {
            Room r;
            return Layouts[Layout(floor)].TryGetValue(room, out r) ? r.Type : 0;
        }
        /// <summary>true when the room touches the floor gate or a passed room.</summary>
        public static bool Reachable(int floor, int room, ICollection<int> passed)
        {
            var lay = Layouts[Layout(floor)];
            Room gate;
            if (lay.TryGetValue(floor * 100, out gate) && gate.Connect.Contains(room)) return true;
            foreach (int p in passed)
            {
                Room r;
                if (lay.TryGetValue(p, out r) && r.Connect.Contains(room)) return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ config
        static Af23Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, Festival, "config.json"); } }
        public static Af23Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(CfgFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = new Af23Config();
                    if (fi.Exists) ReadCfg(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[AprilFools2023] config.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Af23Config();
                }
                return _cfg;
            }
        }
        static int I(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(1000000, FL.U(v)); }
        static void ReadCfg(Af23Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.MinLevel = I(d, "minLevel", c.MinLevel);
            c.MazeMap = (uint)I(d, "mazeMap", (int)c.MazeMap);
            c.CopyMinutes = I(d, "copyMinutes", c.CopyMinutes);
            c.PointsPerAmulet = I(d, "pointsPerAmulet", c.PointsPerAmulet);
            c.ChessLength = Math.Max(8, I(d, "chessLength", c.ChessLength));
            c.GunJump = I(d, "gunJump", c.GunJump);
            c.ChessSoldiers = I(d, "chessSoldiers", c.ChessSoldiers);
            c.RoomMinions = Math.Max(1, I(d, "roomMinions", c.RoomMinions));
            c.CorridorMinions = Math.Max(1, I(d, "corridorMinions", c.CorridorMinions));
            c.Masters = Math.Max(1, I(d, "masters", c.Masters));
            var ex = FL.Arr(FL.Val(d, "exit"));
            if (ex != null && ex.Count == 3) { c.ExitMap = FL.U(ex[0]); c.ExitX = (ushort)FL.U(ex[1]); c.ExitY = (ushort)FL.U(ex[2]); }
            var pool = FL.Arr(FL.Val(d, "dungeonBreaker"));
            if (pool != null)
            {
                var l = new List<Reward>();
                foreach (var o in pool)
                {
                    var r = o as Dictionary<string, object>;
                    if (r == null) continue;
                    uint id = FL.U(FL.Val(r, "item")), n = FL.U(FL.Val(r, "count")), w = FL.U(FL.Val(r, "weight"));
                    if (id != 0 && Kit.Known(id)) l.Add(new Reward(id, Math.Max(1, n), FL.Bool(r, "bound") == true, Math.Max(1, w)));
                }
                if (l.Count > 0) c.Breaker = l;
            }
            var spots = FL.Arr(FL.Val(d, "spots"));
            if (spots != null && spots.Count >= 22)
            {
                var s = new ushort[spots.Count][];
                for (int i = 0; i < spots.Count; i++)
                {
                    var p = FL.Arr(spots[i]);
                    if (p == null || p.Count < 2) return;
                    s[i] = new[] { (ushort)FL.U(p[0]), (ushort)FL.U(p[1]) };
                }
                c.Spots = s;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }
        static void Lua(Client.GameClient c, string call) { Kit.Lua(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { Kit.Msg(c, t); }
        static void Tip(Client.GameClient c, string t) { Kit.Center(c, t); }

        static bool Mine(string func)
        {
            return func.StartsWith("HalloweenPage_", StringComparison.Ordinal) || func.StartsWith("Halloween2022Part_", StringComparison.Ordinal)
                || func.StartsWith("HalloweenNPC_", StringComparison.Ordinal);
        }

        // ================================================================== player state
        public sealed class P
        {
            public uint Uid;
            public long Points, StageExtra;
            public List<int> Stages = new List<int>(), Passed = new List<int>(), Box = new List<int>(), ClownKeys = new List<int>(), Joker = new List<int>();
            public int Floor = 1, Open, Last, Room, Key, Lock = 1, Trans, Secret, Mad, Guide;
        }
        public static P Load(uint uid)
        {
            var s = State;
            var p = new P { Uid = uid };
            p.Points = s.GetLong(uid, "pts");
            p.StageExtra = s.GetLong(uid, "stgx");
            p.Stages = s.GetList(uid, "stg");
            p.Floor = (int)Math.Max(1, Math.Min(9, s.GetLong(uid, "fl", 1)));
            p.Open = (int)s.GetLong(uid, "open");
            p.Passed = s.GetList(uid, "pass");
            p.Last = (int)s.GetLong(uid, "last");
            p.Room = (int)s.GetLong(uid, "room");
            p.Key = (int)s.GetLong(uid, "key");
            p.Lock = (int)s.GetLong(uid, "lock", 1);
            p.Trans = (int)s.GetLong(uid, "trans");
            p.Secret = (int)s.GetLong(uid, "sec");
            p.Box = s.GetList(uid, "box");
            p.ClownKeys = s.GetList(uid, "ckey");
            p.Joker = s.GetList(uid, "joker");
            p.Mad = (int)s.GetLong(uid, "mad");
            p.Guide = (int)s.GetLong(uid, "guide");
            return p;
        }
        public static void Save(P p)
        {
            State.Set(p.Uid, Store.KV("pts", p.Points), Store.KV("stgx", p.StageExtra), Store.KV("stg", p.Stages), Store.KV("fl", p.Floor),
                Store.KV("open", p.Open), Store.KV("pass", p.Passed), Store.KV("last", p.Last), Store.KV("room", p.Room), Store.KV("key", p.Key),
                Store.KV("lock", p.Lock), Store.KV("trans", p.Trans), Store.KV("sec", p.Secret), Store.KV("box", p.Box), Store.KV("ckey", p.ClownKeys),
                Store.KV("joker", p.Joker), Store.KV("mad", p.Mad), Store.KV("guide", p.Guide));
        }
        static void ResetFloor(P p)
        {
            p.Passed.Clear(); p.Last = 0; p.Room = 0; p.Key = 0; p.Lock = 1; p.Trans = 0; p.Secret = 0; p.Box.Clear();
        }

        // ================================================================== callbacks (msg 10113)
        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10113).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Mine(func)) return false;
            if (func == "HalloweenPage_SpecialEventTip") return true;// JokerEvent is never set on this server
            if (Kit.Throttled(c.Player.UID, "af23", 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return true; }
            lock (Kit.Lock(c.Player.UID))
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var p = Load(c.Player.UID);
                    switch (func)
                    {
                        case "HalloweenPage_MainView":
                            Lua(c, "DlgHalloweenMazeMenuMainUi_OpenViewData|{['Code']=200,['Data']={['Points']=" + p.Points + ",['InHall']=0" + (p.Guide == 0 ? ",['Guide']=1" : "") + "}}");
                            if (p.Guide == 0) { p.Guide = 1; State.Set(p.Uid, "guide", 1); }
                            return true;
                        case "HalloweenPage_Entrance":
                        case "HalloweenNPC_EnterSealed":
                            Entrance(c, stream, p, cfg);
                            return true;
                        case "HalloweenPage_EnterCheck":
                            {
                                int room;
                                if (args.Length == 0 || !int.TryParse(args[0], out room) || room < 0 || room > 20) return true;
                                EnterRoom(c, stream, p, cfg, room);
                                return true;
                            }
                        case "Halloween2022Part_EnterFinalBoss":
                            FinalBoss(c, stream, p, cfg);
                            return true;
                        case "Halloween2022Part_EnterMengYan":
                            Entrance(c, stream, p, cfg);
                            return true;
                        case "HalloweenNPC_RoulteOpen":
                            Breaker(c, stream, cfg);
                            return true;
                        case "HalloweenPage_FindShop":
                            Msg(c, "The Trick Store (Joy Shard exchange) has no official price list on this server yet. Submit Clown Masks to Lyon by right-clicking them.");
                            return true;
                        case "HalloweenPage_OpenRankInterface":
                            _pointsOpen[c.Player.UID] = DateTime.Now;
                            Lua(c, "DlgFoolsDayPointsRankUi_UpdateViewData|" + StageData(p, true));
                            return true;
                    }
                }
            return true;
        }

        // ================================================================== stage rewards (FoolsDayPointsAndRankConfig["10113"])
        static readonly Dictionary<uint, DateTime> _pointsOpen = new Dictionary<uint, DateTime>();
        static readonly long[] StagePoints = { 500, 1500, 2500, 5000, 10000, 30000, 50000, 77777 };
        const long RepeatFrom = 77777, RepeatEvery = 5000;
        static List<Reward> StageReward(int i)
        {
            switch (i)
            {
                case 1: return new List<Reward> { Reward.Timed(730004, 1, true, 7200), new Reward(3345618, 1) };      // "730004 0 1 3 7200 1"
                case 2: return new List<Reward> { Reward.Timed(3322762, 2, true, 10080), new Reward(3345618, 1) };   // "3322762 0 2 3 10080 1"
                case 3: return new List<Reward> { Reward.Timed(3009002, 1, false, 2880), new Reward(3345618, 2) };   // "3009002 0 1 0 2880 1"
                case 4: return new List<Reward> { new Reward(3009103, 1), new Reward(3345618, 2) };
                case 5: return new List<Reward> { new Reward(3335924, 1), new Reward(3345618, 3) };
                case 6: return new List<Reward> { new Reward(3342255, 6), new Reward(3345618, 3) };
                case 7: return new List<Reward> { new Reward(3342255, 10), new Reward(3345618, 5) };
                case 8: return new List<Reward> { new Reward(3342255, 20), new Reward(3345618, 10) };
                case 9: return new List<Reward> { new Reward(3009103, 1), new Reward(3345618, 1) };
            }
            return new List<Reward>();
        }
        static long RepeatAvailable(P p) { return p.Points < RepeatFrom + RepeatEvery ? 0 : (p.Points - RepeatFrom) / RepeatEvery - p.StageExtra; }
        static string StageData(P p, bool open)
        {
            var claimed = new List<int>(); var can = new List<int>(); var no = new List<int>();
            for (int i = 1; i <= 8; i++)
            {
                if (p.Stages.Contains(i)) claimed.Add(i);
                else if (p.Points >= StagePoints[i - 1]) can.Add(i);
                else no.Add(i);
            }
            if (RepeatAvailable(p) > 0) can.Add(9); else no.Add(9);
            Func<List<int>, string> t = l => "{" + string.Join(",", l) + "}";
            var data = "['Claimed']=" + t(claimed) + ",['NoClaim']=" + t(no) + ",['CanClaim']=" + t(can);
            if (open) return "{['Code']=200,['Data']={['ActivityId']='10113',['LastDialog']='DlgHalloweenMazeMenuMainUi'," + data + "}}";
            return "{['Code']=200,['Data']={" + data + "}}";
        }

        /// <summary>LuaPush (msg 10311 SenluoHalloween): the stage claim / leaderboard of DlgFoolsDayPointsRankUi, only while
        /// Tricky Wit runs and this player opened the window from Tricky Wit in the last 10 minutes (the window is shared with
        /// the Halloween 2023 Macabre Realm). True = answered.</summary>
        public static bool OnStageLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || (func != "HalloweenNpc2023_ClimbStageReward" && func != "HalloweenNpc2023_LinkLeaderboard")) return false;
            if (!Running) return false;
            DateTime at;
            lock (_pointsOpen) if (!_pointsOpen.TryGetValue(c.Player.UID, out at) || (DateTime.Now - at).TotalMinutes > 10) return false;
            if (Kit.Throttled(c.Player.UID, "af23stage", 600)) return true;
            if (func == "HalloweenNpc2023_LinkLeaderboard") { Ranking(c); return true; }
            int idx;
            if (args.Length == 0 || !int.TryParse(args[0], out idx) || idx < 1 || idx > 9) return true;
            lock (Kit.Lock(c.Player.UID))
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var p = Load(c.Player.UID);
                    bool ok = idx == 9 ? RepeatAvailable(p) > 0 : !p.Stages.Contains(idx) && p.Points >= StagePoints[idx - 1];
                    if (!ok) { Msg(c, "You can`t claim this reward yet."); return true; }
                    var rw = StageReward(idx);
                    if (!Kit.HasSpace(c, Kit.Slots(rw))) { Msg(c, "Your inventory is full. Please make " + Kit.Slots(rw) + " space(s) first."); return true; }
                    if (idx == 9) { p.StageExtra++; State.Set(p.Uid, "stgx", p.StageExtra); }
                    else { p.Stages.Add(idx); State.Set(p.Uid, Store.KV("stg", p.Stages)); }
                    string got = Kit.Give(c, rw, stream, Festival, "stage " + idx);
                    Msg(c, "Congrats! You received " + got + ".");
                    Kit.Lua(c, "DlgFoolsDayPointsRankUi_GetRewardsReturnData|" + StageData(p, false), StageMsg);
                }
            return true;
        }

        /// <summary>Local Tricker Rankings (real data: Trick Points of this server's players).</summary>
        static void Ranking(Client.GameClient c)
        {
            var rows = new List<KeyValuePair<uint, long>>();
            try
            {
                foreach (var kv in State.All("pts")) { long pts; if (long.TryParse(kv.Value, out pts) && pts > 0) rows.Add(new KeyValuePair<uint, long>(kv.Key, pts)); } // [EventData 2026-10-15] players.txt -> EventStore (MySQL)
            }
            catch { }
            rows = rows.OrderByDescending(r => r.Value).Take(10).ToList();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var d = Kit.Dialog(c, stream, DlgRank, (cc, s, o, i) => true);
                d.AddText("Tricker Rankings (local, Trick Points):\n");
                if (rows.Count == 0) d.AddText("Nobody has earned Trick Points yet.\n");
                int n = 1;
                foreach (var r in rows) { d.AddText("No." + n + "  " + NameOf(r.Key) + "  " + r.Value + "\n"); n++; }
                d.AddText("You need at least 10000 points to get listed on the ranking rewards.");
                d.AddOption("I see.", 255).FinalizeDialog();
            }
        }
        static string NameOf(uint uid)
        {
            Client.GameClient o;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out o) && o.Player != null) return o.Player.Name;
            return State.Get(uid, "name", "#" + uid);
        }

        // ================================================================== entrance (Floor %s Entrance)
        static bool CanEnterFrom(Client.GameClient c, Af23Config cfg)
        {
            return cfg.EnterFrom.Contains(c.Player.Map) || c.Player.Map == cfg.MazeMap;
        }
        static void Entrance(Client.GameClient c, ServerSockets.Packet stream, P p, Af23Config cfg)
        {
            if (!CanEnterFrom(c, cfg)) { Msg(c, "Please go to the Market in Twin City to try again."); return; }
            State.Set(p.Uid, "name", c.Player.Name);
            int f = p.Floor;
            var d = Kit.Dialog(c, stream, DlgEntrance, OnEntrance);
            d.AddText("Floor " + f + " Entrance:\n" + FloorText[f - 1] + "\n");
            d.AddText("------*Maze Lord: " + Clowns[f - 1] + "*------\nReward: " + FloorReward[f - 1] + "\n");
            d.AddText(Clowns[f - 1] + " is Mad Clown`s emotion incarnate. Only by defeating it can you break the curse and enter other floors.\n");
            bool cleared = p.Key == 3;
            if (p.Open == 0)
                d.AddOption(f == 1 && !p.Joker.Any() ? "Break maze seal." : "Break maze seal. (1 Maze Key)", 1);
            else
                d.AddOption("Explore the maze.", 2);
            if (cleared && f < 9) d.AddOption("Enter Floor " + (f + 1) + ". (1 Maze Key)", 3);
            if (p.Open != 0) d.AddOption("Reset the maze. (20 Maze Pearls)", 4);
            if (p.Open != 0 && p.Joker.Contains(f) && !cleared) d.AddOption("Mop it up. (21 Maze Pearls)", 5);
            if (p.Joker.Count >= 9 && p.Mad == 0) d.AddOption("Challenge Mad Clown.", 6);
            d.AddOption("View detailed maze rewards.", 7);
            if (c.Player.Map == cfg.MazeMap) d.AddOption("Return to Twin City.", 8);
            d.AddOption("Got it.", 255).FinalizeDialog();
        }
        static bool OnEntrance(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!Running) { Msg(c, Ended); return true; }
            if (Kit.Throttled(c.Player.UID, "af23dlg", 300)) return true;
            var cfg = Cfg;
            lock (Kit.Lock(c.Player.UID))
            {
                var p = Load(c.Player.UID);
                int f = p.Floor;
                switch (option)
                {
                    case 1:// unseal the current floor
                        {
                            if (p.Open != 0) break;
                            bool free = f == 1 && !p.Joker.Any();
                            if (!free && !Kit.Take(c, MazeKey, 1, stream))
                            {
                                Msg(c, "The entrance is sealed. You can`t enter the next floor. You need to use the Maze Key to break the seal. Use Trick Amulets in Dungeon Breaker to collect it.");
                                return true;
                            }
                            p.Open = 1; ResetFloor(p); Save(p);
                            Kit.Log(Festival, c, "UNSEAL", "floor " + f + (free ? " (free)" : " (Maze Key)"));
                            Msg(c, free ? "You carried the Maze Key from Lyon and arrived at the Maze Hall." : "You passed through the gate of seal and arrived on Floor " + f + " of Trick Maze.");
                            Explore(c, p, cfg);
                            return true;
                        }
                    case 2:
                        if (p.Open == 0) break;
                        Explore(c, p, cfg);
                        return true;
                    case 3:// next floor
                        {
                            if (p.Key != 3 || f >= 9) break;
                            if (!Kit.Take(c, MazeKey, 1, stream))
                            {
                                Msg(c, "You need to use the Maze Key to break the seal. Use Trick Amulets in Dungeon Breaker to collect it.");
                                return true;
                            }
                            p.Floor = f + 1; p.Open = 1; ResetFloor(p); Save(p);
                            Kit.Log(Festival, c, "UNSEAL", "floor " + p.Floor + " (Maze Key)");
                            Arena.CloseAll(Tag + c.Player.UID, false);
                            Msg(c, "You passed through the gate of seal and arrived on Floor " + p.Floor + " of Trick Maze.");
                            Explore(c, p, cfg);
                            return true;
                        }
                    case 4:// reset
                        {
                            if (p.Open == 0) break;
                            var cp = Arena.Of(c.Player.UID);
                            if (cp != null && cp.Data != null) { Msg(c, "Please defeat the existing clowns first!"); return true; }
                            if (!Kit.Take(c, MazePearl, 20, stream)) { Msg(c, "You need to collect 20 Maze Pearls to reorganize the maze."); return true; }
                            ResetFloor(p); Save(p);
                            Kit.Log(Festival, c, "RESET", "floor " + f);
                            Msg(c, "Trick Maze has been reset. You can go explore it now.");
                            Explore(c, p, cfg);
                            return true;
                        }
                    case 5:// mop up a floor whose clown was already defeated once
                        {
                            if (p.Open == 0 || !p.Joker.Contains(f) || p.Key == 3) break;
                            var cp = Arena.Of(c.Player.UID);
                            if (cp != null && cp.Data != null) { Msg(c, "Please defeat the existing clowns first!"); return true; }
                            var rw = MopUp(p);
                            int slots = Kit.Slots(rw);
                            if (!Kit.HasSpace(c, slots)) { Msg(c, "Your inventory is full. Please prepare " + slots + " space(s) first."); return true; }
                            if (!Kit.Take(c, MazePearl, 21, stream)) { Msg(c, "You don`t have 21 Maze Pearls to hire explorers to explore the maze for you."); return true; }
                            var lay = Layouts[Layout(f)];
                            p.Passed = lay.Keys.Where(k => k < 100).ToList(); p.Last = 19; p.Key = 3; p.Lock = 0; p.Secret = 1; Save(p);
                            string got = Kit.Give(c, rw, stream, Festival, "mopup floor " + f);
                            Msg(c, "The explorers swept Floor " + f + " for you. You received " + got + ".");
                            return true;
                        }
                    case 6:
                        FinalBoss(c, stream, p, cfg);
                        return true;
                    case 7:
                        {
                            var d = Kit.Dialog(c, stream, DlgEntrance, OnEntrance);
                            d.AddText("------Maze Rewards------\n[Way to Obtain]    [Reward]\nCorridor    /\nEmpty Room    3-10 Trick Amulet Fragments\nForbidden Area    5-8 Trick Amulets\n");
                            d.AddText("Magic Chess    8-15 Trick Amulets\nClown Room    10-15 Trick Amulets, Clown Masks\nMystery Room    5-50 Trick Amulets\n");
                            d.AddOption("Previous.", 9).AddOption("Got it.", 255).FinalizeDialog();
                            return true;
                        }
                    case 8:
                        Arena.Leave(c);
                        if (c.Player.Map == cfg.MazeMap) Arena.Move(c, cfg.ExitX, cfg.ExitY, cfg.ExitMap, 0);
                        return true;
                    case 9:
                        Entrance(c, stream, p, cfg);
                        return true;
                }
            }
            return true;
        }
        static List<Reward> MopUp(P p)
        {
            var lay = Layouts[Layout(p.Floor)];
            uint frag = 0, amu = 0;
            foreach (var r in lay.Values)
            {
                if (r.Id >= 100 || p.Passed.Contains(r.Id)) continue;
                if (r.Type == 1) frag += (uint)Kit.Next(3, 10);
                else if (r.Type == 3) amu += (uint)Kit.Next(8, 15);
                else if (r.Type == 4) amu += (uint)Kit.Next(5, 8);
                else if (r.Type == 5) amu += (uint)Kit.Next(10, 15);
            }
            if (p.Secret == 0) amu += ClownBox[Kit.Next(0, ClownBox.Length - 1)];
            var l = new List<Reward>();
            if (amu > 0) l.Add(new Reward(TrickAmulet, amu));
            if (frag > 0) l.Add(new Reward(TrickFragment, frag));
            l.Add(new Reward(MaskFirst + (uint)(p.Floor - 1), 1));
            return l;
        }

        // ================================================================== the maze map
        static ushort[] Spot(Af23Config cfg, int i) { return cfg.Spots[Math.Max(0, Math.Min(cfg.Spots.Length - 1, i))]; }
        static Copy EnsureCopy(Client.GameClient c, P p, Af23Config cfg, int spot)
        {
            var cp = Arena.Of(c.Player.UID);
            var s = Spot(cfg, spot);
            if (cp != null && cp.Tag == Tag && cp.Map == cfg.MazeMap)
            {
                if (!Arena.Inside(c, cp) || spot >= 0) Arena.Goto(c, cp, s[0], s[1]);
                return cp;
            }
            cp = Arena.Enter(c, cfg.MazeMap, s[0], s[1], Tag, cfg.CopyMinutes, cfg.ExitMap, cfg.ExitX, cfg.ExitY);
            if (cp == null) Msg(c, "Sorry, the room is inaccessible now.");
            return cp;
        }
        static void Explore(Client.GameClient c, P p, Af23Config cfg)
        {
            var cp = Arena.Of(c.Player.UID);
            int spot = p.Room == 20 ? 20 : p.Room;
            if (cp == null || !Arena.Inside(c, cp)) { if (EnsureCopy(c, p, cfg, spot) == null) return; Msg(c, "You have arrived at F" + p.Floor + " of Ordeal Maze."); }
            SendMaze(c, p, false);
        }
        public static string MazeData(P p, bool newPiece)
        {
            var sb = new StringBuilder("DlgHalloweenMazeMainUi_UpdateViewData|{['Code']=200,['Data']={");
            sb.Append("['Floor']=").Append(p.Floor).Append(",['Organize']=").Append(Layout(p.Floor)).Append(",['Enter']=").Append(p.Floor);
            sb.Append(",['PassRomms']={").Append(string.Join(",", p.Passed)).Append("},['LastPassRoom']=").Append(p.Last);
            if (p.Trans > 0) sb.Append(",['TransRoom']=").Append(p.Trans);
            sb.Append(",['HaveKey']=").Append(p.Key).Append(",['PlayerRoom']=").Append(p.Room);
            sb.Append(",['JokerPiece']=").Append(Math.Min(9, p.Joker.Count)).Append(",['JokerPieceState']=").Append(p.Mad != 0 ? 0 : 1);
            if (newPiece) sb.Append(",['NewJokerPiece']=1");
            sb.Append(",['BossRoomLock']=").Append(p.Lock).Append(",['JokerEvent']=0,['PlayEffect']={}}}");
            return sb.ToString();
        }
        static void SendMaze(Client.GameClient c, P p, bool newPiece) { Lua(c, MazeData(p, newPiece)); }

        /// <summary>HalloweenPage_EnterCheck</N>room (0 = gate, 20 = Mystery Room).</summary>
        static void EnterRoom(Client.GameClient c, ServerSockets.Packet stream, P p, Af23Config cfg, int room)
        {
            if (p.Open == 0) { Msg(c, "The entrance is sealed. You need to use the Maze Key to break the seal."); return; }
            if (c.Player.Map != cfg.MazeMap && !cfg.EnterFrom.Contains(c.Player.Map)) { Msg(c, "Please go to the Market in Twin City to try again."); return; }
            var cp = Arena.Of(c.Player.UID);
            var run = cp != null ? cp.Data as Encounter : null;
            if (run != null)
            {
                if (run.Room == room)
                {
                    EnsureCopy(c, p, cfg, room == 20 ? 20 : room);
                    if (run.Kind == 3) ChessDialog(c, stream, run);
                    else if (run.Kind == 20 && run.Step == 0) BoxDialog(c, stream, p);
                    return;
                }
                Msg(c, "Please defeat the existing clowns first!");
                return;
            }
            if (room == 0) { EnsureCopy(c, p, cfg, 0); p.Room = 0; State.Set(p.Uid, "room", 0); SendMaze(c, p, false); return; }
            if (room == 20)
            {
                if (p.Secret != 0) { Tip(c, "This area has been explored. There is no need to enter again."); return; }
                if (p.Trans == 0) { Msg(c, "The Secret Passage is hidden in a random room. Only the real masters in the Forbidden Area may know the location."); return; }
                if (!p.Passed.Contains(p.Trans)) { Tip(c, "The Secret Passage to the Mystery Room has appeared. Go find the Secret Passage."); return; }
            }
            else
            {
                int type = RoomType(p.Floor, room);
                if (type == 0) return;
                if (p.Passed.Contains(room)) { Tip(c, "This area has been explored. There is no need to enter again."); return; }
                if (!Reachable(p.Floor, room, p.Passed)) { Tip(c, "The room is locked. You can`t enter."); return; }
                if (type == 5)
                {
                    if (p.Lock != 0) { Msg(c, "The device to unseal the clown room is hidden in the Mystery Room. Please go explore the Mystery Room first."); return; }
                    if (p.Key != 1) { Tip(c, "You haven`t found the Ordeal Light. Please go to the theater to obtain it."); return; }
                }
            }
            cp = EnsureCopy(c, p, cfg, room);
            if (cp == null) return;
            p.Room = room; State.Set(p.Uid, "room", room);
            StartEncounter(c, stream, cp, p, cfg, room);
        }

        // ------------------------------------------------------------------ encounters
        public sealed class Encounter
        {
            public int Room, Kind, Floor, Step;
            // magic chess
            public int Square, Blessings, KingSquare;
            public HashSet<int> BlessSq = new HashSet<int>(), GunSq = new HashSet<int>(), SoldierSq = new HashSet<int>(), ExitDone = new HashSet<int>();
        }
        static void StartEncounter(Client.GameClient c, ServerSockets.Packet stream, Copy cp, P p, Af23Config cfg, int room)
        {
            int kind = room == 20 ? 20 : RoomType(p.Floor, room);
            var run = new Encounter { Room = room, Kind = kind, Floor = p.Floor };
            cp.Data = run;
            var s = Spot(cfg, room);
            Arena.ClearMobs(cp);
            switch (kind)
            {
                case 1:// trick room
                    for (int i = 0; i < cfg.RoomMinions; i++) Arena.Spawn(cp, Minion(cfg, i), s[0], s[1], 5, stream);
                    Tip(c, "Mad Clown unleashed some monsters to cause a disturbance. Please defeat the monsters and ruin Mad Clown`s plan.");
                    break;
                case 2:// trap corridor
                    for (int i = 0; i < cfg.CorridorMinions; i++) Arena.Spawn(cp, Minion(cfg, i), s[0], s[1], 5, stream);
                    Tip(c, "This corridor is full of traps! Rascal, Tickler and Coward blocked the exit. Hurry up and defeat them all!");
                    break;
                case 3:// magic chess
                    SetupChess(run, cfg);
                    Tip(c, "A weird chess game, a Tricky Dice... Only by winning the chess game can you get out of the maze.");
                    ChessDialog(c, stream, run);
                    return;
                case 4:// forbidden area: the masters
                    for (int i = 0; i < cfg.Masters; i++) Arena.Spawn(cp, cfg.Master, s[0], s[1], 4, stream);
                    Tip(c, "The Forbidden Area is full of masters. Defeat them!");
                    break;
                case 20:// mystery room: the Clown Box first
                    Tip(c, "The treasure boxes are shining brightly. Go choose one and open it!");
                    BoxDialog(c, stream, p);
                    return;
                case 5:// clown room
                    Arena.Spawn(cp, cfg.ClownFirst + (uint)(p.Floor - 1), s[0], s[1], 2, stream);
                    Tip(c, XcBoss[p.Floor - 1]);
                    break;
            }
            if (Arena.Alive(cp) == 0 && Arena.SpawnOverride == null)
            {
                Msg(c, "Sorry, the room is inaccessible now.");
                cp.Data = null;
                return;
            }
            Arena.Arm(cp, Cleared);
        }
        static uint Minion(Af23Config cfg, int i) { return i % 3 == 0 ? cfg.Minion1 : i % 3 == 1 ? cfg.Minion2 : cfg.Minion3; }

        /// <summary>The wave of the current room is dead (owner's tick).</summary>
        static void Cleared(Client.GameClient c, Copy cp)
        {
            var run = cp.Data as Encounter;
            if (run == null) return;
            if (!Running) { cp.Data = null; return; }
            lock (Kit.Lock(c.Player.UID))
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var p = Load(c.Player.UID);
                    if (p.Floor != run.Floor || p.Open == 0) { cp.Data = null; return; }
                    var cfg = Cfg;
                    if (run.Kind == 3) { ChessCleared(c, stream, cp, p, cfg, run); return; }
                    if (run.Kind == 20 && run.Step == 1)
                    {
                        // the Crystal Device is destroyed: the clown room is unsealed
                        p.Secret = 1; p.Lock = 0; Save(p); cp.Data = null;
                        Tip(c, "The Crystal is cracked and the seal has been broken. Now you may enter the clown room to challenge " + Clowns[p.Floor - 1] + ".");
                        SendMaze(c, p, false);
                        return;
                    }
                    if (run.Kind == 99) { MadClownDown(c, stream, cp, p); return; }
                    if (p.Passed.Contains(run.Room)) { cp.Data = null; return; }
                    var rw = new List<Reward>();
                    bool newPiece = false;
                    switch (run.Kind)
                    {
                        case 1: rw.Add(new Reward(TrickFragment, (uint)Kit.Next(3, 10))); break;
                        case 4:
                            rw.Add(new Reward(TrickAmulet, (uint)Kit.Next(5, 8)));
                            if (!p.ClownKeys.Contains(p.Floor)) rw.Add(new Reward(ClownKey, 1));
                            break;
                        case 5:
                            rw.Add(new Reward(TrickAmulet, (uint)Kit.Next(10, 15)));
                            rw.Add(new Reward(MaskFirst + (uint)(p.Floor - 1), 1));
                            break;
                    }
                    int slots = Kit.Slots(rw);
                    if (!Kit.HasSpace(c, slots))
                    {
                        // keep the room open: the reward is paid when the player comes back with space (the wave is re-armed)
                        Msg(c, "Your inventory is full. Please prepare " + slots + " space(s) first, then click the room again.");
                        cp.Data = null;
                        return;
                    }
                    p.Passed.Add(run.Room); p.Last = run.Room;
                    if (run.Kind == 4)
                    {
                        if (!p.ClownKeys.Contains(p.Floor)) p.ClownKeys.Add(p.Floor);
                        if (p.Key == 0) p.Key = 1;
                        if (p.Trans == 0)
                        {
                            var lay = Layouts[Layout(p.Floor)];
                            var cand = lay.Values.Where(r => r.Id < 100 && r.Type != 5 && r.Type != 4 && !p.Passed.Contains(r.Id)).Select(r => r.Id).ToList();
                            if (cand.Count == 0) cand = lay.Values.Where(r => r.Id < 100 && r.Type != 5).Select(r => r.Id).ToList();
                            p.Trans = cand[Kit.Next(0, cand.Count - 1)];
                        }
                    }
                    if (run.Kind == 5)
                    {
                        p.Key = 3;
                        if (!p.Joker.Contains(p.Floor)) { p.Joker.Add(p.Floor); newPiece = true; }
                    }
                    Save(p);
                    cp.Data = null;
                    string got = Kit.Give(c, rw, stream, Festival, "room " + run.Room + " type " + run.Kind + " floor " + p.Floor);
                    switch (run.Kind)
                    {
                        case 1: Msg(c, "You managed to solve the clown`s prank. You received " + got + ". The Knight Plume has appeared. Hurry up and go to the next area."); break;
                        case 2: Msg(c, "The corridor monsters have been cleared and the Knight Plume has arrived! Put it on and head to other areas!"); break;
                        case 4:
                            Msg(c, "You defeated the masters of the Forbidden Area and received " + got + ".");
                            Tip(c, "You have got the Ordeal Light. The Secret Passage to the Mystery Room has appeared. Go find the Secret Passage.");
                            break;
                        case 5:
                            Msg(c, newPiece
                                ? (p.Joker.Count >= 9 ? "You participated in defeating " + Clowns[p.Floor - 1] + " and received a Mad Clown`s soul fragment. Now Mad Clown`s soul is complete. Go challenge him in the maze!"
                                                      : "You participated in defeating " + Clowns[p.Floor - 1] + " and received a Mad Clown`s soul fragment. Please keep collecting more.")
                                : "You`ve defeated " + Clowns[p.Floor - 1] + ".");
                            Msg(c, "Congrats! You defeated " + Clowns[p.Floor - 1] + " and passed Floor " + p.Floor + "! You received " + got + ".");
                            break;
                    }
                    SendMaze(c, p, newPiece);
                }
        }

        // ------------------------------------------------------------------ Mystery Room: Clown Box + Crystal Device
        static void BoxDialog(Client.GameClient c, ServerSockets.Packet stream, P p)
        {
            var d = Kit.Dialog(c, stream, DlgBox, OnBox);
            d.AddText("Clown Box:\n    The box is shining brightly!\n    ------------*Maze Treasure*------------\n");
            for (int i = 0; i < ClownBox.Length; i++) d.AddText("    Treasure " + (i + 1) + ": " + ClownBox[i] + " Trick Amulets" + (p.Box.Contains(i + 1) ? " (taken)" : "") + "\n");
            d.AddText("    Note: Every box has got 1 treasure inside, and every treasure can be claimed once.\n");
            d.AddOption("Open it. (1 Clown Key)", 1).AddOption("Go to other areas.", 255).FinalizeDialog();
        }
        static bool OnBox(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (option != 1 || !Running) return true;
            if (Kit.Throttled(c.Player.UID, "af23box", 800)) return true;
            var cfg = Cfg;
            lock (Kit.Lock(c.Player.UID))
            {
                var cp = Arena.Of(c.Player.UID);
                var run = cp != null ? cp.Data as Encounter : null;
                var p = Load(c.Player.UID);
                if (run == null || run.Kind != 20 || run.Step != 0 || !Arena.Inside(c, cp) || p.Secret != 0 || p.Floor != run.Floor) return true;
                if (Kit.Count(c, ClownKey) == 0) { Msg(c, "You don`t have a Clown Key to open the box. You can go challenge the masters in the Forbidden Area to collect it."); return true; }
                var left = Enumerable.Range(1, ClownBox.Length).Where(i => !p.Box.Contains(i)).ToList();
                if (left.Count == 0) { p.Box.Clear(); left = Enumerable.Range(1, ClownBox.Length).ToList(); }
                int pick = left[Kit.Next(0, left.Count - 1)];
                var rw = new List<Reward> { new Reward(TrickAmulet, ClownBox[pick - 1]) };
                if (!Kit.HasSpace(c, Kit.Slots(rw))) { Msg(c, "Please make some spaces in your inventory first."); return true; }
                if (!Kit.Take(c, ClownKey, 1, stream)) return true;
                p.Box.Add(pick); Save(p);
                string got = Kit.Give(c, rw, stream, Festival, "clown box " + pick + " floor " + p.Floor);
                Msg(c, "You received " + got + " from the box. 1 Crystal Device has appeared in front of you. Destroy it and see what will happen!");
                if (ClownBox[pick - 1] >= 30) Kit.World(c.Player.Name + " opened the Clown Box and received " + ClownBox[pick - 1] + " Trick Amulet(s)! How lucky!");
                run.Step = 1;
                var s = Spot(cfg, 20);
                Arena.Spawn(cp, cfg.Crystal, s[0], s[1], 2, stream);
                if (Arena.Alive(cp) == 0 && Arena.SpawnOverride == null)
                {
                    // no crystal type (festival types not registered): unseal at once
                    p.Secret = 1; p.Lock = 0; Save(p); cp.Data = null; cp.Armed = false;
                    SendMaze(c, p, false);
                    return true;
                }
                Arena.Arm(cp, Cleared);
            }
            return true;
        }

        // ------------------------------------------------------------------ Magic Chess
        static void SetupChess(Encounter run, Af23Config cfg)
        {
            int n = cfg.ChessLength;
            var free = Enumerable.Range(2, n - 2).ToList();
            Func<int> take = () => { int i = Kit.Next(0, free.Count - 1); int v = free[i]; free.RemoveAt(i); return v; };
            for (int i = 0; i < 5 && free.Count > 0; i++) run.BlessSq.Add(take());
            for (int i = 0; i < 4 && free.Count > 0; i++) run.GunSq.Add(take());
            for (int i = 0; i < cfg.ChessSoldiers && free.Count > 0; i++) run.SoldierSq.Add(take());
            run.KingSquare = Kit.Next(1, 4);
        }
        static void ChessDialog(Client.GameClient c, ServerSockets.Packet stream, Encounter run)
        {
            if (run.Square >= Cfg.ChessLength) { ExitDialog(c, stream, run); return; }
            if (run.Step == 1) { Msg(c, "The soldier is guarding the Tricky Dice. Defeat him to roll the dice."); return; }
            var b = run.BlessSq.OrderBy(x => x).ToList(); var g = run.GunSq.OrderBy(x => x).ToList();
            var d = Kit.Dialog(c, stream, DlgChess, OnChess);
            d.AddText("Tricky Dice:\n    ----------------Magic Chess----------------\n");
            d.AddText("    Ritual Blessings are scattered on Square " + string.Join(", ", b) + "\n    Gun Carriers are scattered on Square " + string.Join(", ", g) + "\n");
            d.AddText("    1. You can move forward as you roll the Tricky Dice.\n    2. You`ll meet different episodes. Deal with them to move on.\n");
            d.AddText("    3. Find the King and defeat him at the finish line to win the chess game and get out of the maze.\n");
            d.AddText("      |You`re on Square " + run.Square + " of " + Cfg.ChessLength + "|  Ritual Blessings: " + run.Blessings + "\n");
            d.AddOption("Roll the dice.", 1);
            if (Kit.Count(c, PerfectDice) > 0) d.AddOption("Roll the Perfect Dice.", 2);
            d.AddOption("Not now.", 255).FinalizeDialog();
        }
        static bool OnChess(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!Running || option == 255) return true;
            if (Kit.Throttled(c.Player.UID, "af23dice", 700)) return true;
            lock (Kit.Lock(c.Player.UID))
            {
                var cp = Arena.Of(c.Player.UID);
                var run = cp != null ? cp.Data as Encounter : null;
                if (run == null || run.Kind != 3 || !Arena.Inside(c, cp) || run.Step != 0 || run.Square >= Cfg.ChessLength) return true;
                int steps;
                if (option == 1) steps = Kit.Next(1, 6);
                else if (option == 2)
                {
                    var d = Kit.Dialog(c, stream, DlgChess, OnChess);
                    d.AddText("Perfect Dice:\n    Use the Perfect Dice to choose the steps you want to move forward.\n");
                    for (byte i = 1; i <= 6; i++) d.AddOption(i + (i == 1 ? " step." : " steps."), (byte)(10 + i));
                    d.AddOption("Not now.", 255).FinalizeDialog();
                    return true;
                }
                else if (option >= 11 && option <= 16)
                {
                    if (!Kit.Take(c, PerfectDice, 1, stream)) return true;
                    steps = option - 10;
                    Kit.Log(Festival, c, "PERFECTDICE", steps.ToString());
                }
                else return true;
                Move(c, stream, cp, run, steps, true);
            }
            return true;
        }
        static void Move(Client.GameClient c, ServerSockets.Packet stream, Copy cp, Encounter run, int steps, bool canJump)
        {
            var cfg = Cfg;
            run.Square = Math.Min(cfg.ChessLength, run.Square + steps);
            int sq = run.Square;
            if (sq >= cfg.ChessLength) { Msg(c, "Bang! You moved " + steps + " square(s) forward. You`ve reached the finish line!"); ExitDialog(c, stream, run); return; }
            if (run.SoldierSq.Remove(sq))
            {
                Msg(c, "You moved " + steps + " square(s) forward and encountered a Soldier. The soldier is guarding the Tricky Dice. Defeat him to roll the dice.");
                var s = Spot(cfg, run.Room);
                Arena.Spawn(cp, cfg.Soldier, s[0], s[1], 3, stream);
                run.Step = 1;
                if (Arena.Alive(cp) == 0 && Arena.SpawnOverride == null) { run.Step = 0; ChessDialog(c, stream, run); return; }
                Arena.Arm(cp, Cleared);
                return;
            }
            if (canJump && run.GunSq.Remove(sq))
            {
                Msg(c, "You moved " + steps + " square(s) forward and encountered a Gun Carrier. You went inside the cannon!");
                Move(c, stream, cp, run, cfg.GunJump, false);
                return;
            }
            if (run.BlessSq.Remove(sq))
            {
                run.Blessings++;
                Msg(c, "You moved " + steps + " square(s) forward and encountered a Ritual Blessing. You received " + run.Blessings + " Ritual Blessing(s). Collect 3 to activate the blessings.");
                if (run.Blessings == 3) Msg(c, "You`ve picked up 3 Ritual Blessings, so the blessings are workings now. You`ll receive extra 5 Trick Amulets if you win the chess game. Pick up more to win more!");
                else if (run.Blessings == 4) Msg(c, "You`ve picked up 4 Ritual Blessings, so the blessings are doubled. You`ll receive extra 10 Trick Amulets if you win the chess game. Pick up more to win more!");
                else if (run.Blessings >= 5) Msg(c, "You`ve picked up 5 Ritual Blessings, so the blessings are doubled. You`ll receive extra 20 Trick Amulets if you win the chess game.");
            }
            else Msg(c, "Bang! You moved " + steps + " square(s) forward.");
            ChessDialog(c, stream, run);
        }
        static readonly string[] ExitNames = { "", "top left square", "bottom square", "top right square", "lower right square" };
        static void ExitDialog(Client.GameClient c, ServerSockets.Packet stream, Encounter run)
        {
            if (run.Step != 0) return;
            var d = Kit.Dialog(c, stream, DlgExit, OnExit);
            d.AddText("Maze Exit:\n    The King is on the four squares at the edge of the chessboard.\n    Only by defeating the King can you win the chess game and break the seal of the exit.\n");
            for (byte i = 1; i <= 4; i++) if (!run.ExitDone.Contains(i)) d.AddOption("Go to " + ExitNames[i] + ".", i);
            d.AddOption("Not now.", 255).FinalizeDialog();
        }
        static bool OnExit(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!Running || option < 1 || option > 4) return true;
            if (Kit.Throttled(c.Player.UID, "af23dice", 700)) return true;
            var cfg = Cfg;
            lock (Kit.Lock(c.Player.UID))
            {
                var cp = Arena.Of(c.Player.UID);
                var run = cp != null ? cp.Data as Encounter : null;
                if (run == null || run.Kind != 3 || !Arena.Inside(c, cp) || run.Step != 0 || run.Square < cfg.ChessLength || run.ExitDone.Contains(option)) return true;
                run.ExitDone.Add(option);
                uint fam; string text;
                if (option == run.KingSquare) { fam = cfg.King; text = "Hurry up and defeat the King to win the chess game."; run.Step = 3; }
                else
                {
                    int other = run.ExitDone.Count(x => x != run.KingSquare);
                    fam = other == 1 ? cfg.Queen : other == 2 ? cfg.Patriarch : cfg.Knight;
                    text = other == 1 ? "You`ve arrived on the Queen square. Please defeat the Queen." : other == 2 ? "You`ve arrived on the Patriarch square. Please defeat the Patriarch." : "You`ve arrived on the Knight square. Please defeat the Knight.";
                    run.Step = 2;
                }
                var s = Spot(cfg, run.Room);
                Arena.Spawn(cp, fam, s[0], s[1], 2, stream);
                Tip(c, text);
                if (Arena.Alive(cp) == 0 && Arena.SpawnOverride == null) { run.Step = 0; Msg(c, "Sorry, the room is inaccessible now."); return true; }
                Arena.Arm(cp, Cleared);
            }
            return true;
        }
        static void ChessCleared(Client.GameClient c, ServerSockets.Packet stream, Copy cp, P p, Af23Config cfg, Encounter run)
        {
            if (run.Step == 1)
            {
                run.Step = 0;
                Msg(c, "You`ve defeated the soldier, and now you can roll the Tricky Dice.");
                ChessDialog(c, stream, run);
                return;
            }
            if (run.Step == 2)
            {
                run.Step = 0;
                Msg(c, "You`ve defeated the piece. Please find the King.");
                ExitDialog(c, stream, run);
                return;
            }
            if (run.Step != 3) return;
            // the King is down: the chess game is won
            if (p.Passed.Contains(run.Room)) { cp.Data = null; return; }
            uint bonus = run.Blessings >= 5 ? 20u : run.Blessings == 4 ? 10u : run.Blessings == 3 ? 5u : 0u;
            var rw = new List<Reward> { new Reward(TrickAmulet, (uint)Kit.Next(8, 15) + bonus) };
            if (!Kit.HasSpace(c, Kit.Slots(rw)))
            {
                // re-arm on the King square so the player can come back with space
                run.Step = 0; run.ExitDone.Remove(run.KingSquare);
                Msg(c, "Your inventory is full. Please make some spaces first, then click the chess room again.");
                return;
            }
            p.Passed.Add(run.Room); p.Last = run.Room; Save(p);
            cp.Data = null;
            string got = Kit.Give(c, rw, stream, Festival, "chess room " + run.Room + " floor " + p.Floor + " blessings " + run.Blessings);
            Msg(c, "Congrats! You win! You received " + got + ". Please put on the Knight Plume and enter another room.");
            SendMaze(c, p, false);
        }

        // ------------------------------------------------------------------ Mad Clown
        static void FinalBoss(Client.GameClient c, ServerSockets.Packet stream, P p, Af23Config cfg)
        {
            if (p.Joker.Count < 9) { Msg(c, "You haven`t collected all the clown`s soul fragments."); return; }
            if (p.Mad != 0) { Msg(c, "You`ve defeated Mad Clown. You can`t enter again."); return; }
            if (!CanEnterFrom(c, cfg)) { Msg(c, "Please go to the Market in Twin City to try again."); return; }
            var cp = Arena.Of(c.Player.UID);
            if (cp != null && cp.Data != null) { Msg(c, "Please defeat the existing clowns first!"); return; }
            cp = EnsureCopy(c, p, cfg, 21);
            if (cp == null) return;
            var s = Spot(cfg, 21);
            cp.Data = new Encounter { Room = 21, Kind = 99, Floor = p.Floor };
            Arena.ClearMobs(cp);
            Arena.Spawn(cp, cfg.MadClown, s[0], s[1], 2, stream);
            if (Arena.Alive(cp) == 0 && Arena.SpawnOverride == null) { cp.Data = null; Msg(c, "Sorry, the room is inaccessible now."); return; }
            Tip(c, "Mad Clown is grinning wildly and coming at you!");
            Arena.Arm(cp, Cleared);
        }
        static void MadClownDown(Client.GameClient c, ServerSockets.Packet stream, Copy cp, P p)
        {
            cp.Data = null;
            if (p.Mad != 0) return;
            var rw = new List<Reward> { new Reward(TrickAmulet, 50), new Reward(JokerFragment, 10), new Reward(PlayfulFragment, 6) };
            if (!Kit.HasSpace(c, Kit.Slots(rw)))
            {
                Msg(c, "Your inventory is full. Please make some spaces before challenging the boss!");
                return;
            }
            p.Mad = 1; Save(p);
            string got = Kit.Give(c, rw, stream, Festival, "Mad Clown");
            Msg(c, "You`ve defeated Mad Clown and received " + got + ".");
            Kit.World(c.Player.Name + " defeated Mad Clown and received 50 Trick Amulets, 10 Joker Knight Fragments and 6 Playful Player Fragments.");
            SendMaze(c, p, false);
        }

        // ------------------------------------------------------------------ Dungeon Breaker (Trick Amulets -> clown treasures + points)
        static void Breaker(Client.GameClient c, ServerSockets.Packet stream, Af23Config cfg)
        {
            var p = Load(c.Player.UID);
            var d = Kit.Dialog(c, stream, DlgBreaker, OnBreaker);
            d.AddText("Dungeon Breaker:\n    Use Trick Amulets to get random clown treasures and earn Trick Points.\n");
            d.AddText("    Trick Amulets owned: " + Kit.Count(c, TrickAmulet) + "    Trick Points: " + p.Points + "\n");
            d.AddText("    Treasures: " + string.Join(", ", cfg.Breaker.Select(r => r.Count + " " + Kit.Name(r.Item))) + "\n");
            d.AddOption("Use 1 Trick Amulet.", 1).AddOption("Use 10 Trick Amulets.", 2).AddOption("Let me think about it.", 255).FinalizeDialog();
        }
        static bool OnBreaker(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!Running || (option != 1 && option != 2)) return true;
            if (Kit.Throttled(c.Player.UID, "af23breaker", 700)) return true;
            UseAmulets(c, stream, option == 2 ? 10 : 1);
            Breaker(c, stream, Cfg);
            return true;
        }
        /// <summary>n Trick Amulets -> n clown treasures + points. Also the right-click of a Trick Amulet (n = 1).</summary>
        public static void UseAmulets(Client.GameClient c, ServerSockets.Packet stream, int n)
        {
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return; }
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Count(c, TrickAmulet) < n) { Msg(c, "You don`t have enough Trick Amulets to join in."); return; }
                var picks = new List<Reward>();
                for (int i = 0; i < n; i++) { var r = Kit.Pick(cfg.Breaker, x => x.Weight); if (r != null) picks.Add(new Reward(r.Item, r.Count, r.Bound)); }
                int slots = Kit.Slots(picks);
                if (!Kit.HasSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first."); return; }
                if (!Kit.Take(c, TrickAmulet, (uint)n, stream)) return;
                var p = Load(c.Player.UID);
                long add = (long)n * cfg.PointsPerAmulet;
                p.Points += add;
                State.Set(p.Uid, Store.KV("pts", p.Points), Store.KV("name", c.Player.Name));
                string got = Kit.Give(c, picks, stream, Festival, "breaker x" + n);
                Msg(c, "Congrats! You successfully got " + got + ". Trick Points +" + add + " (total " + p.Points + ").");
            }
        }

        // ------------------------------------------------------------------ Lyon (festival NPC, Twin City Market)
        public static bool OnLyon(Client.GameClient c, ServerSockets.Packet stream)
        {
            if (!Running) return false;
            var p = Load(c.Player.UID);
            var d = Kit.Dialog(c, stream, DlgLyon, OnLyonOpt);
            d.AddText("Lyon:\nThe maze is filled with the clown`s emotions and tricks!\nIf you`re brave enough to explore the maze and challenge the clowns,\nI`ll give you something nice in return!\n");
            d.AddText("Requirement: Level " + Cfg.MinLevel + "\n    |Trick Points Owned: " + p.Points + "|\n");
            d.AddOption("Challenge Trick Maze.", 1).AddOption("Dungeon Breaker.", 2).AddOption("View Tricker Rankings.", 3).AddOption("Come later.", 255).FinalizeDialog();
            return true;
        }
        static bool OnLyonOpt(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!Running) { Msg(c, Ended); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Trick Maze is full of danger. Please level up to " + cfg.MinLevel + " first."); return true; }
            lock (Kit.Lock(c.Player.UID))
            {
                if (option == 1) Entrance(c, stream, Load(c.Player.UID), cfg);
                else if (option == 2) Breaker(c, stream, cfg);
                else if (option == 3) Ranking(c);
            }
            return true;
        }

        // ------------------------------------------------------------------ tick: the festival ended while the player is in the maze
        public static void Tick(Client.GameClient c)
        {
            var cp = Arena.Of(c.Player.UID);
            if (cp == null || cp.Tag != Tag) return;
            if (!Running) { Msg(c, Ended); Arena.Close(cp, true); }
        }
    }
}
