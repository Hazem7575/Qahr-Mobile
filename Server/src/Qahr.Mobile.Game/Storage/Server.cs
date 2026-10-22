using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Database;
using Qahr.Mobile.Insults;

namespace Qahr.Mobile
{
    public static class Pool
    {

        public static class Constants
        {
            public static readonly List<uint> ProtectMapSpells = new List<uint>() { };
            public static readonly ushort[] PriceUpdatePorf = new ushort[] { 36000, 36000, 36000, 36000, 36000, 36000, 18367, 12328, 7377, 6164, 3688, 3082, 3082, 3082, 3082, 2670, 1825, 1251, 866, 704 };
            public static readonly List<uint> MapCounterHits = new List<uint>() { 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 1005, 6000 };
            public static readonly List<uint> NoDropItems = new List<uint>() { 3071, 31254, 31255, 31256, 31257, 31254, 31255, 31256, 12288, 9991, 9992, 9993, 9990, 12602, 12275, 3030, 10741, 66666, 33333, 22222, 88888, 1036, 10364, 12288, 9991, 9992, 9993, 12546, 13249, 12602, 13140, 12546, 12800, 11331, 12799, 11996, 12665, 12601, 12816, 12099, 1138, 1764, 9855, 3954, 3820 };
            public static readonly List<uint> SkillsNotAvailableHere = new List<uint>()
{
    5051, 5053, 5054, 5055, 5056, 5066,
    5057, 5058, 5059, 5060, 5061, 5062,
    5062, 5063, 5064, 5065, 22330, 22331,
    22332, 22333, 22334, 22335, 1518, 1508,
    6011, 20083, 20082, 20084, 20086, 12546,13249,12602,13140,12546,12800,11331,12799,11996,12665,12601,12816,12099, 66666,33333,22222,88888, 12602, 12275
};
            public static readonly List<uint> RemoveRide = new List<uint>()
{
    5051, 5053, 5054, 5055, 5056, 5066,
    5057, 5058, 5059, 5060, 5061, 5062,
    5062, 5063, 5064, 5065, 22330, 22331,
    22332, 22333, 22334, 22335, 1518, 1508,
    6011, 20083, 20082, 20084, 20086, 12546,13249,12602,13140,12546,12800,11331,12799,11996,12665,12601,12816,12099, 66666,33333,22222,88888, 12602, 12275
}.Concat(new List<uint>()
{
    20081, 20082, 20083, 20084, 3825, 6011, 5342, 3825, 6521, 9988, 
    6891, 5599, 8521, 1005, 9855, 3053, 10230, 1860, 10741, 22330, 22331, 3030,
    22332, 22333, 22334, 1860, 1858, 8881, 8880, 9855, 12546,13249,12602,13140,12546,12800,11331,12799,11996,12665,12601,12816,12099, 66666,33333,22222,88888, 12602, 12275
}).Distinct().ToList();
            public static readonly List<uint> FreePkMap = new List<uint>() { 3071, 2071, 31254, 31255, 31256, 31257, 10133, 11222, 12288, 9991, 9992, 9993, 9990, 3071, 3030, 10741, 66666, 33333, 22222, 88888, 10133, 1036, 10364, 12288, 9991, 9992, 9993, 12546, 12602, 12275, 13249, 12602, 13140, 12546, 12800, 11331, 12799, 11996, 12665, 12601, 12816, 12099, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 1138, 12020, 12021, 12022, 12066, 12067, 12025, 10001, 15757, 1483, 1002, 1518, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5051, 5053, 5054, 5055, 5056, 5057, 5058, 5053, 5054, 5055, 5056, 5057, 5058, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 6011, 3998, 6784, 5342, 8009, 2353, 5339, 6964, 3071, 11447, 6546, 3935, 8437, 9988, 5599, 2071, 7357, 6521, 3825, 5342, 6891, 8521, 12099, 6729, 6000, 6001, 1505, 1005, 1038, 9855, 1508/*PkWar*/, 1357, Game.MsgTournaments.MsgCaptureTheFlag.MapID };
            public static readonly List<uint> BlockAttackMap = new List<uint>() { 3071, 31254, 31255, 31256, 31257, 10133, 12288, 9991, 9992, 9993, 9990, 3030, 1036, 10364, 12288, 9991, 9992, 9993, 10001, 11447, 10445, 10446, 10447, 10448, 10449, 10450, 10451, 10452, 10453, 10454, 10455, 15757, 15758, 15759, 2353, 3825, 6072, 3830, 5040, 3820, 1004, 3831, 3832, 3834, 3826, 3827, 3828, 3829, 10, 9995, 1068, 4020, 4003, 4006, 4008, 4009, 1860, 1858, 1780, 1779/*Ghost Map*/, 9972, 1806, 1002, 1002, 3954, 3081, 1036, 1008, 601, 1006, 10741, 1039, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, (uint)Game.MsgTournaments.MsgSteedRace.Maps.DungeonRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IceRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IslandRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.LavaRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.MarketRace };
            public static readonly List<uint> BlockTeleportMap = new List<uint>() { 10001, 22330, 22331, 22333, 22334, 22335, 5050, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070, 601, 6000, 5040, 6001, 1005, 9855, 1858, 1860, 3852, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, 1768 };
        }
        public unsafe class SendGlobalPacket
        {
            public unsafe void Enqueue(ServerSockets.Packet data)
            {
                // [perf] dropped an unused Values.ToArray() copy; live lock-free enumeration (no .Values snapshot)
                foreach (var kv in Pool.GamePoll)
                {
                    var user = kv.Value;
                    user.Send(data);
                }
            }
        }
        public static List<LvLottery> LotteryList = new List<LvLottery>(100);
        public class LvLottery
        {
            public uint ItemID;
            public string Name;
            public uint GateID;
            public uint RanksItem;
        }
        public static LotteryTable Lottery = new LotteryTable();
        public static List<Smelt> SmeltingSuccesses = new List<Smelt>(10);
        public class Smelt
        {
            public string Prize, Name;
            public byte Furnace;
        }

        public static List<byte> SmeltingSessions = new List<byte>(10);
        public static string ReverseString(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = "";
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }
        public static Dictionary<uint, HuntCoins> HuntCoinsMap = new Dictionary<uint, HuntCoins>();
        public class HuntCoins
        {
            public string Name;
            public uint Coins;
            public int Rank;
        }
        public static Dictionary<uint, DeitylandSacrificeRanking> DeitylandSacrificeRankings = new Dictionary<uint, DeitylandSacrificeRanking>();
        public class DeitylandSacrificeRanking
        {
            public string Prize, Name;
            public uint Jades;
        }
        public static DateTime GoldenTreeExpirationDate = DateTime.FromBinary(0);
        public static uint GoldenTreeClaimed = 0;
        public static uint MaxAvaliableGoldenTreeClaim = 0;
        public static Dictionary<uint, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>> ClientAgates = new Dictionary<uint, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>>();
        public static Time32 smeltFloorStamp;
        public static uint lostlady = 0;
        public static List<Game.MsgServer.MsgMelterRankList.Instance> MelterRankList = new List<Game.MsgServer.MsgMelterRankList.Instance>();
        public static Qahr.Mobile.Shared.Cryptography.TransferCipher transferCipher;
        public static Role.Instance.Nobility.NobilityRanking NobilityRanking = new Role.Instance.Nobility.NobilityRanking();
        public static Role.Instance.ChiRank ChiRanking = new Role.Instance.ChiRank();
        public static Role.Instance.Flowers.FlowersRankingToday FlowersRankToday = new Role.Instance.Flowers.FlowersRankingToday();
        public static Role.Instance.Flowers.FlowerRanking GirlsFlowersRanking = new Role.Instance.Flowers.FlowerRanking();
        public static Role.Instance.Flowers.FlowerRanking BoysFlowersRanking = new Role.Instance.Flowers.FlowerRanking(false);
        public static DateTime ServerSave = new DateTime();
        public static DateTime ResetRandom = new DateTime();
        public static System.FastRandom Random = new System.FastRandom();
        public static System.SafeRandom GetRandom = new System.SafeRandom();
        public static System.RandomLite LiteRandom = new System.RandomLite();
        public static DateTime DragonIslandBansheeHour, DragonIslandSpookHour, DragonIslandNemsisHour;
        public static Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>> LevelInfo = new Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>>();
        public static ConcurrentDictionary<uint, TheCrimeTable> TheCrimePoll = new ConcurrentDictionary<uint, TheCrimeTable>();
        public static Database.ActivityTask ActivityTasks = new ActivityTask();
        public static InfoHeroReward TableHeroRewards = new InfoHeroReward();
        public static List<uint> RedeemActivated = new List<uint>();
        public static Dictionary<ushort, List<ushort>> WeaponSpells = new Dictionary<ushort, List<ushort>>();
        public static MagicType Magic = new MagicType();
        public static SubProfessionInfo SubClassInfo = new SubProfessionInfo();
        public static Dictionary<uint, Game.MsgMonster.MonsterFamily> MonsterFamilies = new Dictionary<uint, Game.MsgMonster.MonsterFamily>();
        public static ConcurrentDictionary<uint, List<string>> MonsterMap = new ConcurrentDictionary<uint, List<string>>();
        public static System.Counter ITEM_Counter = new System.Counter(1);
        public static System.Counter Inbox_Counter = new System.Counter(1);
        public static Rifinery RifineryItems;
        public static Qahr.Mobile.CachedAttributeInvocation<System.Action<Qahr.Mobile.Client.GameClient, Qahr.Mobile.ServerSockets.Packet>, Qahr.Mobile.PacketAttribute, ushort> MsgInvoker;
        public static InsultManager Insults = new InsultManager();
        public static RefinaryBoxes DBRerinaryBoxes;
        public static ItemType ItemsBase;
        public static MythTable.MythSoulEXP Item;
        public static MapDictionary<uint, Role.GameMap> ServerMaps;
        public static ConcurrentDictionary<uint, Client.GameClient> GamePoll;

        public static ConcurrentDictionary<uint, Client.GameClient> DisconnectPool = new ConcurrentDictionary<uint, Client.GameClient>();
        public static ConcurrentDictionary<uint, Game.MsgNpc.Npc> NpcSever = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();
        public static ConcurrentDictionary<uint, Role.SobNpc> SobNpcSever = new ConcurrentDictionary<uint, Role.SobNpc>();

        public static List<int> NameUsed;
        public static RebornInfomations RebornInfo;
        public static ArenaTable Arena = new ArenaTable();
        public static SendGlobalPacket SendGlobalPackets;
        public static object TeamArena = new object();
        public static System.Counter ClientCounter = new System.Counter(1000000);
        public static System.Counter DominoCounter = new System.Counter(300000000);
        public static ConfiscatorTable QueueContainer = new ConfiscatorTable();
        public static System.SafeDictionary<uint, Game.MsgServer.MsgGameItem> ChatItems = new System.SafeDictionary<uint, Game.MsgServer.MsgGameItem>();
        public static int RandFromGivingNums(params int[] nums)
        {
            return nums[GetRandom.Next(0, nums.Length)];
        }
        public static uint RandFromGivingNums(uint[] nums)
        {
            if (nums == null || nums.Length == 0) return 0;
            return nums[GetRandom.Next(0, nums.Length)];
        }
    }
    public class Server
    {
        public static bool FullLoading = false;
        public static void SendGlobalPacket(ServerSockets.Packet data)
        {
            // [perf] dropped an unused Values.ToArray() copy; live lock-free enumeration (no .Values snapshot)
            foreach (var kv in Pool.GamePoll)
            {
                var user = kv.Value;
                user.Send(data);
            }
        }

        public static uint ResetServerDay = 0;
        public static unsafe void Reset()
        {
            if (DateTime.Now.DayOfYear != ResetServerDay)
            {
                try
                {
                    Pool.Arena.ResetArena();

                    foreach (var flowerclient in Role.Instance.Flowers.ClientPoll.Values)
                    {
                        foreach (var flower in flowerclient)
                            flower.Amount2day = 0;
                    }
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        foreach (var client in Pool.GamePoll.Values)
                        {


                            client.Player.TodayChampionPoints = 0;

                            client.Player.ChangeEpicTrojan = client.Player.ChangeArrayEpicTrojan =
                    client.Player.ChangeMr_MirrorEpicTrojan = client.Player.ChangeGeneralPakEpicTrojan = 0;
                            client.Player.CanChangeEpicMaterial = client.Player.CanChangeArrayEpicMaterial =
                                client.Player.CanChangeMr_MirrorEpicMaterial = client.Player.CanChangGeneralPakMaterial = 1;

                            client.Player.UseChiToken = 0;

                            client.Player.RamdanBag = 0;
                            client.Player.TowerOfMysterychallenge = 3;
                            client.Player.TOMChallengeToday = 0;
                            client.Player.TowerOfMysteryChallengeFlag = 0;
                            client.Player.TOMSelectChallengeToday = 0;
                            client.Player.ClaimTowerAmulets = 0;
                            client.Player.TOMClaimTeamReward = 0;
                            client.Player.TOMRefreshReward = 0;
                            client.Player.QuestGUI.RemoveQuest(6126);

                            client.RelicBoostDone = 0;

                            client.Player.OpenHousePack = 0;
                            client.MyExchangeShop.Reset();
                            if (client.Player.DailyMonth == 0)
                                client.Player.DailyMonth = (byte)DateTime.Now.Month;
                            if (client.Player.DailyMonth != DateTime.Now.Month)
                            {
                                client.Player.DailySignUpRewards = 0;
                                client.Player.DailySignUpDays = 0;
                                client.Player.DailyMonth = (byte)DateTime.Now.Month;
                            }
                            if (client.Player.MyJiangHu != null)
                            {
                                client.Player.MyJiangHu.FreeCourse = 30000;
                                client.Player.MyJiangHu.FreeTimeToday = 0;
                                client.Player.MyJiangHu.RoundBuyPoints = 0;
                            }
                            client.Player.ArenaKills = client.Player.ArenaDeads = 0;
                            client.Player.HitShoot = client.Player.MisShoot = 0;

                            client.Player.DbTry = false;
                            client.Player.LotteryEntries = 0;
                            client.Player.Day = DateTime.Now.DayOfYear;
                            // daily activeness reset for players online at midnight (login path: ServerDatabase.ResetingEveryDay)
                            if (client.Activeness != null)
                            {
                                client.Activeness.ResetDay();
                                client.Activeness.CheckTasks(stream);
                                client.Activeness.IncreaseTask(1);
                                client.Activeness.IncreaseTask(13);
                                client.Activeness.IncreaseTask(25);
                                client.Activeness.UpdateActivityPoints(stream);
                                client.Activeness.UpdateClaimRewards(stream);
                            }
                            client.Player.BDExp = 0;
                            client.Player.TCCaptainTimes = 0;
                            client.Player.ExpBallUsed = 0;
                            client.DemonExterminator.FinishToday = 0;

                            if (client.Player.MyChi != null)
                            {
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            }
                            client.Player.Flowers.FreeFlowers = 1;
                            foreach (var flower in client.Player.Flowers)
                                flower.Amount2day = 0;


                            if (client.Player.Flowers.FreeFlowers > 0)
                            {
                                client.Send(stream.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                                    ? Game.MsgServer.MsgFlower.FlowerAction.FlowerSender
                                    : Game.MsgServer.MsgFlower.FlowerAction.Flower
                                    , 0, 0, client.Player.Flowers.FreeFlowers));
                            }

                            if (client.Player.Level >= 90)
                            {
                                client.Player.Enilghten = ServerDatabase.CalculateEnlighten(client.Player);
                                client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                            }

                            client.Player.BuyKingdomDeeds = 0;
                            client.Player.QuestGUI.RemoveQuest(35024);
                            client.Player.QuestGUI.RemoveQuest(35007);
                            client.Player.QuestGUI.RemoveQuest(35025);
                            client.Player.QuestGUI.RemoveQuest(35028);
                            client.Player.QuestGUI.RemoveQuest(35034);

                            //---- reset Quests
                            client.Player.QuestGUI.RemoveQuest(6390);
                            client.Player.QuestGUI.RemoveQuest(6329);
                            client.Player.QuestGUI.RemoveQuest(6245);
                            client.Player.QuestGUI.RemoveQuest(6049);
                            client.Player.QuestGUI.RemoveQuest(6366);
                            client.Player.QuestGUI.RemoveQuest(6014);
                            client.Player.QuestGUI.RemoveQuest(2375);
                            client.Player.QuestGUI.RemoveQuest(6126);
                            // [QuestFix 2026-10-13] Bird Island dailies were never reset
                            client.Player.QuestGUI.RemoveQuest(6129);
                            client.Player.QuestGUI.RemoveQuest(6130);
                            client.Player.QuestGUI.RemoveQuest(6131);
                            client.Player.QuestGUI.ResetBountyHallDay();// [QuestFix 2026-10-13] Bounty Hall daily cap + finished bounties
                            global::Qahr.Mobile.QuestEngine.QuestEngineCore.ResetRepeatables(client, -1);// [QuestEngine] daily/weekly engine quests (midnight: one day passed)
                            client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                                = client.Player.DailyMagnoliaItemId
                                = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                            //
                        }
                    }
                    ResetServerDay = (uint)DateTime.Now.DayOfYear;
                    // كتابة Day فورًا في shell.ini (نفس مسار/مفتاح SaveDatabase) عشان الريست مايتكررش لو وقع قبل الحفظ الدوري
                    try
                    {
                        string dayBaseDir = AppDomain.CurrentDomain.BaseDirectory;
                        if (!dayBaseDir.EndsWith("\\")) dayBaseDir += "\\";
                        WindowsAPI.IniFile DayIni = new WindowsAPI.IniFile("");
                        DayIni.FileName = dayBaseDir + "shell.ini";
                        DayIni.Write<uint>("Database", "Day", ResetServerDay);
                    }
                    catch (Exception exDay) { MyConsole.WriteLine(exDay.ToString()); }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
        }

        public static void SaveDBPayers()
        {
            if (!Program.ServerConfig.IsInterServer)
            {
                SaveDatabase();
            }
        }

        static readonly System.Diagnostics.Stopwatch startupClock = System.Diagnostics.Stopwatch.StartNew();
        static readonly List<KeyValuePair<string, long>> startupSteps = new List<KeyValuePair<string, long>>();
        static void Step(string name, Action action)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            BootScreen.StepStart(name);
            action();
            BootScreen.StepDone();
            startupSteps.Add(new KeyValuePair<string, long>(name, sw.ElapsedMilliseconds));
        }
        static string ShortStep(string k)
        {
            if (k.EndsWith(".Load")) k = k.Substring(0, k.Length - 5);
            if (k.StartsWith("Database.")) k = k.Substring(9);
            return k;
        }
        static void ReportStartup()
        {
            startupSteps.Sort((a, b) => b.Value.CompareTo(a.Value));
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Startup " + startupClock.ElapsedMilliseconds + " ms");
            foreach (var s in startupSteps) sb.AppendLine(s.Value.ToString().PadLeft(7) + " ms  " + s.Key);
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "startup_times.txt", sb.ToString());
            int shown = 0;
            var slow = new System.Text.StringBuilder();
            for (int i = 0; i < startupSteps.Count && i < 3; i++)
                slow.Append(i == 0 ? "" : " · ").Append(ShortStep(startupSteps[i].Key)).Append(' ').Append(startupSteps[i].Value).Append("ms");
            BootScreen.Slowest = slow.ToString();
            foreach (var s in startupSteps) { if (shown++ == 5) break; MyConsole.WriteLine("  slow load: " + s.Key + " " + s.Value + " ms", ConsoleColor.DarkGray); }
        }
        public static void Initialize()
        {
            BootScreen.Begin("Conquer Online Server  ·  Version 7957", "Edit By Mohy ElDin");
            MyConsole.WriteLine("Please Wait The Sourse is Loading ...", ConsoleColor.White);
            WindowsAPI.IniCache.Begin();
            WindowsAPI.IniCache.TrustRecent = true;// [FastBoot 2026-10-15] boot only, IniCache.End() switches it off
            Pool.ServerMaps = new MapDictionary<uint, Role.GameMap>();
            Pool.GamePoll = new ConcurrentDictionary<uint, Client.GameClient>();

            Pool.NameUsed = new List<int>();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!baseDir.EndsWith("\\")) baseDir += "\\";
            WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile(baseDir + "shell.ini", true);
            // [Central config 2026-10-17] IP / game port / name read from the ONE central ServerConfig.ini
            // [Server]; fall back to this server's shell.ini [ServerInfo] only if a key is missing there.
            Program.ServerConfig.IPAddres = SharedConfig.String("Server", "IP", IniFile.ReadString("ServerInfo", "AddresIP", ""));
            Program.ServerConfig.GamePort = (ushort)SharedConfig.Int("Server", "GamePort", IniFile.ReadUInt16("ServerInfo", "Game_Port", 0));
            Program.ServerConfig.ServerName = SharedConfig.String("Server", "ServerName", IniFile.ReadString("ServerInfo", "ServerName", ""));
            // [Central config 2026-10-17] central ServerConfig.ini first; shell.ini is the fallback for each key.
            Program.ServerConfig.IsInterServer = SharedConfig.Int("Server", "IsInterServer", IniFile.ReadBool("ServerInfo", "IsInterServer", false) ? 1 : 0) != 0;

            Program.ServerConfig.Port_BackLog = (ushort)SharedConfig.Int("Network", "BackLog", IniFile.ReadUInt16("InternetPort", "BackLog", 100));
            Program.ServerConfig.Port_ReceiveSize = (ushort)SharedConfig.Int("Network", "ReceiveSize", IniFile.ReadUInt16("InternetPort", "ReceiveSize", 4096));
            Program.ServerConfig.Port_SendSize = (ushort)SharedConfig.Int("Network", "SendSize", IniFile.ReadUInt16("InternetPort", "SendSize", 2048));

            Program.ServerConfig.DbLocation = SharedConfig.String("Server", "DataFolder", IniFile.ReadString("Database", "Location", ""));
            Program.ServerConfig.CO2Folder = SharedConfig.String("Server", "CO2Folder", IniFile.ReadString("Database", "CO2FOLDER", ""));
            Program.ServerConfig.LoadRates();// [LiveRates 2026-10-17] read ServerConfig.ini [Rates] (live via "reloadrates")
            // Least-privilege default: fall back to a limited DB user, never root, when shell.ini omits it.
            // The actual user is still read from shell.ini, so existing configs are untouched.
            // DB connection: read from the ONE central ServerConfig.ini [Database]; fall back to this
            // server's shell.ini [MySql] only if the central file is missing (so nothing breaks).
            Qahr.Mobile.DBFunctionality.DataHolder.CreateConnection(
                SharedConfig.String("Database", "Host", IniFile.ReadString("MySql", "Host", "localhost")),
                SharedConfig.String("Database", "User", IniFile.ReadString("MySql", "User", "co_game")),
                SharedConfig.String("Database", "Password", IniFile.ReadString("MySql", "Password", "")),
                SharedConfig.String("Database", "Name", IniFile.ReadString("MySql", "Database", "mohe2121")),
                SharedConfig.Int("Database", "Port", 3306));
            // [DB migration] pick the player-data backend (shell.ini [Storage] PlayerBackend = File | Dual | MySql) and create the table if needed.
            global::Qahr.Mobile.Database.Storage.PlayerStorage.Init();
            // [مضاد التكرار] العدّادات بتتحفظ كل 10 دقايق بس؛ لو السيرفر وقع، الأغراض الجديدة كانت بتاخد
            // UIDs لأغراض موجودة فعلًا في شنط لاعبين. نقفز بهامش أمان عند كل إقلاع ونكتب القيمة فورًا،
            // فحتى لو وقع تاني قبل أول حفظ، الإقلاع الجاي يقفز تاني ومستحيل UID يتكرر.
            const uint UidSafetyMargin = 100000;
            Pool.ITEM_Counter.Set(IniFile.ReadUInt32("Database", "ItemUID", 0) + UidSafetyMargin);
            uint nextitem = Pool.ITEM_Counter.Next;
            Pool.ClientCounter.Set(IniFile.ReadUInt32("Database", "ClientUID", 1000000));
            Pool.DominoCounter.Set(IniFile.ReadUInt32("Database", "DominoUID", 300000000) + UidSafetyMargin);
            IniFile.Write<uint>("Database", "ItemUID", Pool.ITEM_Counter.Count);
            IniFile.Write<uint>("Database", "DominoUID", Pool.DominoCounter.Count);
            uint nextclient = Pool.ClientCounter.Next;
            // [FastBoot 2026-10-15] QuestInfo.Init only reads Questinfo.ini and publishes QuestInfo.AllQuests/NpcQuests at its
            // end (reference swap); nothing before the "QuestInfo.Init" step reads those tables, so it runs on a pool thread
            // meanwhile (~120 ms off the boot). The step below waits for it, so everything after sees the same tables.
            var questInfoBoot = System.Threading.Tasks.Task.Run(() => QuestInfo.Init());
            // [FastBoot 2026-10-15] same for Pool.Magic.Load: it only fills Pool.Magic + Pool.WeaponSpells (magictype.txt,
            // MagicEffect.ini) and no step before "Pool.Magic.Load" reads them (only the attack/login code does).
            var magicBoot = System.Threading.Tasks.Task.Run(() => Pool.Magic.Load());
            // [FastBoot 2026-10-17] ChiTable + InnerPowerTable are I/O-bound (scan DbLocation\Users\ ini files + read
            // NeiGongInfo.ini/InnerPower.txt) into their own per-UID tables; ChiPool/ChiRanking are static (ready), and no
            // step before their loads below reads them (verified L416-535). Prefetch on pool threads; the steps below wait.
            var chiBoot = System.Threading.Tasks.Task.Run(() => ChiTable.Load());
            var innerPowerBoot = System.Threading.Tasks.Task.Run(() => { InnerPowerTable.LoadDBInformation(); InnerPowerTable.Load(); });
            Step("Game.MsgTournaments.MsgSchedules.Create", () => Game.MsgTournaments.MsgSchedules.Create());
            ResetServerDay = IniFile.ReadUInt32("Database", "Day", 0);
            Step("Database.Shops.ChampionShop.Load", () => Database.Shops.ChampionShop.Load());
            Step("Database.Shops.EShopFile.Load", () => Database.Shops.EShopFile.Load());
            Step("Database.Shops.EShopV2File.Load", () => Database.Shops.EShopV2File.Load());
            Step("Database.Shops.HonorShop.Load", () => Database.Shops.HonorShop.Load());
            Step("Database.Shops.RacePointShop.Load", () => Database.Shops.RacePointShop.Load());
            Step("Database.Shops.ShopFile.Load", () => Database.Shops.ShopFile.Load());
            Step("Database.NinjaRank.Load", () => Database.NinjaRank.Load());
            Step("Database.WarriorRank.Load", () => Database.WarriorRank.Load());
            Step("Database.ArcherRank.Load", () => Database.ArcherRank.Load());
            Step("Database.LeeLongRank.Load", () => Database.LeeLongRank.Load());
            Step("Database.DuneWandererRank.Load", () => Database.DuneWandererRank.Load());
            Step("Database.MonkRanks.Load", () => Database.MonkRanks.Load());
            Step("Database.WaterRank.Load", () => Database.WaterRank.Load());
            Step("Database.AstredgeRank.Load", () => Database.AstredgeRank.Load());
            Step("Database.FireRank.Load", () => Database.FireRank.Load());
            Step("Database.PirateRank.Load", () => Database.PirateRank.Load());
            Step("Database.RuneRank.Load", () => Database.RuneRank.Load());
            Step("Database.HWRank.Load", () => Database.HWRank.Load());
            Step("Database.PrestigeRanking.Load", () => Database.PrestigeRanking.Load());
            // [modularised] ShareVIP load now runs from Modules\Custom\ShareVIP\ShareVIPModule.OnLoad.
            Pool.MsgInvoker = new Qahr.Mobile.CachedAttributeInvocation<System.Action<Qahr.Mobile.Client.GameClient, Qahr.Mobile.ServerSockets.Packet>, Qahr.Mobile.PacketAttribute, ushort>(PacketAttribute.Translator);
            Pool.RebornInfo = new RebornInfomations();
            Step("Pool.RebornInfo.Load", () => Pool.RebornInfo.Load());
            Pool.ItemsBase = new ItemType();
            Pool.RifineryItems = new Rifinery();
            Pool.DBRerinaryBoxes = new RefinaryBoxes();
            Step("Pool.ItemsBase.Loading", () => Pool.ItemsBase.Loading());

            Step("HeroGatheringRank.Load", () => HeroGatheringRank.Load());
            Step("Role.KOBoard.KOBoardRanking.Load", () => Role.KOBoard.KOBoardRanking.Load());

            Step("RankItems.LoadAllItems", () => RankItems.LoadAllItems());
            Step("syndicate_level.Load", () => syndicate_level.Load());
            Step("syn_formtype.Load", () => syn_formtype.Load());
            Step("cq_syn_skill_type.Load", () => cq_syn_skill_type.Load());
            Step("SystemBanned.Load", () => SystemBanned.Load());
            Step("SystemBannedPC.Load", () => SystemBannedPC.Load());
            Step("SystemBannedAccount.Load", () => SystemBannedAccount.Load());
            Step("Server.LoadExpInfo", () => Server.LoadExpInfo());
            Step("DataCore.AtributeStatus.Load", () => DataCore.AtributeStatus.Load());
            Step("Role.GameMap.LoadMaps", () => Role.GameMap.LoadMaps());
            Step("YuanshenAttr.Load", () => YuanshenAttr.Load());
            Step("YuanshenLevUP.Load", () => YuanshenLevUP.Load());
            Step("YuanshenLottery.Load", () => YuanshenLottery.Load());
            Step("YuanshenRank.Load", () => YuanshenRank.Load());
            Step("Server.LoadMonsters", () => Server.LoadMonsters());
            Step("FileDrops.Load", () => Game.MsgMonster.FileDrops.Load());// [FileDrops] Infinity[DB]\drops.txt (missing = off)
            Step("AttackSpeed.Load", () => global::Qahr.Mobile.Combat.AttackSpeedConfig.Load());// [AttackSpeed] Infinity[DB]\attackspeed.txt (missing = off)
            Step("Tranformation.Int", () => Tranformation.Int());
            Step("Pool.Magic.Load", () => magicBoot.GetAwaiter().GetResult());// [FastBoot 2026-10-15] started in the background above
            Step("QuestInfo.Init", () => questInfoBoot.GetAwaiter().GetResult());// [FastBoot 2026-10-15] started in the background above; waits + rethrows its exception here
            global::Qahr.Mobile.QuestEngine.QuestGrammar.BootSpawnMemo = true;// [FastBoot 2026-10-15] Program.Main switches it off after the festivals' rebuild
            Step("QuestEngine.Load", () => global::Qahr.Mobile.QuestEngine.QuestEngineCore.Load());// [QuestEngine]
            Step("Pool.SubClassInfo.Load", () => Pool.SubClassInfo.Load());
            Step("BeastsAtrribute.Load", () => BeastsAtrribute.Load());
            Step("Database.HairfaceStorageTable.Load", () => Database.HairfaceStorageTable.Load());
            Step("RuneTable.Load", () => RuneTable.Load());
            Step("MythTable.LoadMyth", () => MythTable.LoadMyth());
            Step("dict_collection.Load", () => dict_collection.Load());
            Step("dict_lottery.Load", () => dict_lottery.Load());
            Step("Database.HundredWeapons.Load", () => Database.HundredWeapons.Load());
            Step("NinjaFile.Load", () => NinjaFile.Load());
            Step("Gouyuxuzuotype.Load", () => Gouyuxuzuotype.Load());
            Step("MeltingTypeTable.Load", () => MeltingTypeTable.Load());
            Step("CombatGear.Load", () => CombatGear.Load());
            Step("god_weapons_type.Load", () => god_weapons_type.Load());
            Step("god_weapons_material.Load", () => god_weapons_material.Load());
            Step("god_weapons_exp.Load", () => god_weapons_exp.Load());
            Step("daoqi_dict_type.Load", () => daoqi_dict_type.Load());
            Step("FateExpTable.Load", () => FateExpTable.Load());
            Step("SpiritTable.Load", () => SpiritTable.Load());
            Step("SwordAncestorType.Load", () => SwordAncestorType.Load());
            Step("ChiTable.Load", () => chiBoot.GetAwaiter().GetResult());// [FastBoot 2026-10-17] prefetched above
            Step("FlowersTable.Load", () => FlowersTable.Load());
            Step("NobilityTable.Load", () => NobilityTable.Load());
            Step("Role.Instance.Associate.Load", () => Role.Instance.Associate.Load());
            Step("CoatStorage.Load", () => CoatStorage.Load());
            //Game.MsgTournaments.MsgSchedules.UnionWar.Create();
            Step("Game.MsgTournaments.MsgSchedules.GuildWar.CreateFurnitures", () => Game.MsgTournaments.MsgSchedules.GuildWar.CreateFurnitures());
            Step("Game.MsgTournaments.MsgSchedules.GuildWar.Load", () => Game.MsgTournaments.MsgSchedules.GuildWar.Load());
            Step("Game.MsgTournaments.MsgSchedules.SuperGuildWar.CreateFurnitures", () => Game.MsgTournaments.MsgSchedules.SuperGuildWar.CreateFurnitures());
            Step("Game.MsgTournaments.MsgSchedules.SuperGuildWar.Load", () => Game.MsgTournaments.MsgSchedules.SuperGuildWar.Load());
            Step("Game.MsgTournaments.MsgSchedules.EliteGuildWar.Load", () => Game.MsgTournaments.MsgSchedules.EliteGuildWar.Load());

            Step("Game.MsgTournaments.MsgWarOfPlayers.Load", () => Game.MsgTournaments.MsgWarOfPlayers.Load());
            //Game.MsgTournaments.MsgNewKingsWar.Load();
            //Game.MsgTournaments.MsgSchedules.EmperorWar.Load();
            //Game.MsgTournaments.MsgSchedules.TopWarScore.Load();
            //Game.MsgTournaments.MsgSchedules.ClanPhoenix.Load();


            Step("Coat_Color_Rule.Load", () => Coat_Color_Rule.Load());
            Step("LeagueTable.Load", () => LeagueTable.Load());
            Step("GuildTable.Load", () => GuildTable.Load());
            Step("Database.ClanTable.Load", () => Database.ClanTable.Load());
            Step("JianHuTable.LoadStatus", () => JianHuTable.LoadStatus());
            Step("JianHuTable.LoadJiangHu", () => JianHuTable.LoadJiangHu());
            Step("TaskRewards.Load", () => TaskRewards.Load());
            // [FastBoot 2026-10-17] InnerPowerTable.LoadDBInformation runs inside innerPowerBoot (prefetched above, awaited at InnerPowerTable.Load)
            Step("ExchangeShopGoods.Load", () => ExchangeShopGoods.Load());
            Step("ExchangeShopGoodsEx.Load", () => ExchangeShopGoodsEx.Load());
            Step("new_shop_goods.Load", () => new_shop_goods.Load());
            // [modularised] QuizShow load now runs from Modules\Custom\QuizShow\QuizShowModule.OnLoad.
            Step("Game.MsgTournaments.MsgSchedules.ClassPkWar.Load", () => Game.MsgTournaments.MsgSchedules.ClassPkWar.Load());
            Step("Game.MsgTournaments.MsgSchedules.ElitePkTournament.Load", () => Game.MsgTournaments.MsgSchedules.ElitePkTournament.Load());
            Step("Game.MsgTournaments.MsgSchedules.TeamPkTournament.Load", () => Game.MsgTournaments.MsgSchedules.TeamPkTournament.Load());
            Step("Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Load", () => Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Load());
            Step("TitleStorage.LoadDBInformation", () => TitleStorage.LoadDBInformation());
            Step("ItemRefineUpgrade.Load", () => ItemRefineUpgrade.Load());
            Step("Database.ProfessionTable.Load", () => Database.ProfessionTable.Load());
            Step("TheCrimeTable.Load", () => TheCrimeTable.Load());
            Step("Pool.ActivityTasks.Load", () => Pool.ActivityTasks.Load());
            Step("Role.Statue.Load", () => Role.Statue.Load());
            Step("Pool.TableHeroRewards.LoadInformations", () => Pool.TableHeroRewards.LoadInformations());
            Step("Database.Disdain.Load", () => Database.Disdain.Load());
            // [modularised] RechargeShop load now runs from Modules\Custom\RechargeShop\RechargeShopModule.OnLoad.
            Step("Booths.Load", () => Booths.Load());
            Step("Pool.Arena.Load", () => Pool.Arena.Load());
            Step("InnerPowerTable.Load", () => innerPowerBoot.GetAwaiter().GetResult());// [FastBoot 2026-10-17] prefetched above (incl. LoadDBInformation)
            Step("Database.TutorInfo.Load", () => Database.TutorInfo.Load());
            Step("InfoDemonExterminators.Create", () => InfoDemonExterminators.Create());
            Step("Pool.QueueContainer.Load", () => Pool.QueueContainer.Load());
            Step("LoadDeitylandSacrifice", () => LoadDeitylandSacrifice());
            Step("GroupServerList.Load", () => GroupServerList.Load());
            // [modularised] VoteSystem load now runs from Modules\Custom\VoteSystem\VoteSystemModule.OnLoad.
            Step("Database.RanksTable.Initialize", () => Database.RanksTable.Initialize());
            Step("RankMonster.Load", () => RankMonster.Load());
            Step("Server.LoadDatabase", () => Server.LoadDatabase());
            Step("Pool.Insults.Load", () => Pool.Insults.Load());
            Step("Modules.ModuleManager.LoadAll", () => global::Qahr.Mobile.Modules.ModuleManager.LoadAll());
            WindowsAPI.IniCache.End();
            ReportStartup();
        }

        public static void LoadMapName(uint id)
        {

            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "GameMapEx.ini"))
            {
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("GameMapEx.ini");
                Pool.ServerMaps[id].Name = ini.ReadString(id.ToString(), "Name", Program.ServerConfig.ServerName);
            }
        }
        public static void LoadExpInfo()
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "levexp.txt"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "levexp.txt"))
                {
                    while (true)
                    {
                        string GetLine = read.ReadLine();
                        if (GetLine == null) return;
                        string[] line = GetLine.Split(' ');
                        DBLevExp exp = new DBLevExp();
                        exp.Action = (DBLevExp.Sort)byte.Parse(line[0]);
                        exp.Level = byte.Parse(line[1]);
                        exp.Experience = ulong.Parse(line[2]);
                        exp.UpLevTime = int.Parse(line[3]);
                        exp.MentorUpLevTime = int.Parse(line[4]);

                        if (!Pool.LevelInfo.ContainsKey(exp.Action))
                            Pool.LevelInfo.Add(exp.Action, new Dictionary<byte, DBLevExp>());

                        Pool.LevelInfo[exp.Action].Add(exp.Level, exp);

                    }
                }
            }
            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
        public static void LoadMonsters(uint id = 0)
        {
            try
            {
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                if (System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\MonsterSpawns\\" + id.ToString() + "\\"))
                    foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\MonsterSpawns\\" + id.ToString() + "\\"))
                    {
                        Game.MsgMonster.MobCollection colletion = new Game.MsgMonster.MobCollection(id);
                        if (colletion.ReadMap())
                        {
                            foreach (string ffile in System.IO.Directory.GetFiles(fmap))
                            {
                                ini.FileName = ffile;
                                colletion.LocationSpawn = ffile;

                                uint ID = ini.ReadUInt32("cq_generator", "npctype", 0);

                                Game.MsgMonster.MonsterFamily famil;
                                if (!Pool.MonsterFamilies.TryGetValue(ID, out famil))
                                {
                                    continue;
                                }
                                if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                                    continue;
                                Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                                Monster.SpawnX = ini.ReadUInt16("cq_generator", "bound_x", 0);
                                Monster.SpawnY = ini.ReadUInt16("cq_generator", "bound_y", 0);
                                Monster.MaxSpawnX = (ushort)(Monster.SpawnX + ini.ReadUInt16("cq_generator", "bound_cx", 0));
                                Monster.MaxSpawnY = (ushort)(Monster.SpawnY + ini.ReadUInt16("cq_generator", "bound_cy", 0));
                                Monster.MapID = ini.ReadUInt32("cq_generator", "mapid", 0);
                                Monster.SpawnCount = ini.ReadByte("cq_generator", "max_per_gen", 0);//"maxnpc", 0);//max_per_gen", 0);
                                Monster.rest_secs = ini.ReadInt32("cq_generator", "rest_secs", 0);


                                if (Monster.MapID == 1011 || Monster.MapID == 3071 || Monster.MapID == 1770 || Monster.MapID == 1771 || Monster.MapID == 1772
                                    || Monster.MapID == 1773 || Monster.MapID == 1774 || Monster.MapID == 1775 || Monster.MapID == 1777
                                    || Monster.MapID == 1782 || Monster.MapID == 1785 || Monster.MapID == 1786 || Monster.MapID == 1787
                                    || Monster.MapID == 1794)
                                    Monster.SpawnCount = ini.ReadByte("cq_generator", "maxnpc", 0);


                                colletion.Add(Monster);
                            }
                        }
                    }
                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void LoadMyMonsters(uint id)
        {
            try
            {
                Game.MsgMonster.MobCollection colletion = new Game.MsgMonster.MobCollection(id);
                if (colletion.ReadMap())
                {
                    string[] text = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "MonsterSpawns\\myMonsters.txt");
                    foreach (string line in text)
                    {
                        string[] data = line.Split(' ');
                        ushort MapID = (ushort)long.Parse(data[1]);
                        if (id != MapID) continue;
                        uint monsterID = (uint)long.Parse(data[9]);
                        ushort CircleDiameter = (ushort)long.Parse(data[6]);
                        ushort X = (ushort)long.Parse(data[2]);
                        ushort Y = (ushort)long.Parse(data[3]);
                        ushort XPlus = (ushort)long.Parse(data[4]);
                        ushort YPlus = (ushort)long.Parse(data[5]);
                        int Amount = (int)long.Parse(data[8]);
                        uint respawn = (uint)long.Parse(data[7]);

                        Game.MsgMonster.MonsterFamily famil;
                        if (!Pool.MonsterFamilies.TryGetValue(monsterID, out famil))
                        {
                            continue;
                        }
                        if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                            continue;
                        Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                        Monster.SpawnX = X;
                        Monster.SpawnY = Y;
                        Monster.MaxSpawnX = (ushort)(X + XPlus);
                        Monster.MaxSpawnY = (ushort)(Y + YPlus);
                        Monster.MapID = MapID;
                        Monster.SpawnCount = (byte)Amount;
                        Monster.rest_secs = (int)respawn;
                        colletion.Add(Monster);
                    }
                }
                //  LoadMapMonsters("Monsters1015.txt");
                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void LoadNewMonsters(uint id)
        {
            try
            {
                Game.MsgMonster.MobCollection colletion = new Game.MsgMonster.MobCollection(id);
                if (colletion.ReadMap())
                {
                    string[] text = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "MonsterSpawns\\NewMonster.txt");
                    foreach (string line in text)
                    {
                        string[] data = line.Split(' ');
                        try
                        {
                            ushort MapID = ushort.Parse(data[1]);
                            if (id != MapID) continue;
                            ushort X = (ushort)long.Parse(data[2]);
                            ushort Y = (ushort)long.Parse(data[3]);
                            ushort XPlus = (ushort)long.Parse(data[4]);
                            ushort YPlus = (ushort)long.Parse(data[5]);
                            uint respawn = (uint)long.Parse(data[6]);
                            int Amount = (int)long.Parse(data[7]);
                            uint monsterID = (uint)long.Parse(data[8]);
                            Game.MsgMonster.MonsterFamily famil;
                            if (!Pool.MonsterFamilies.TryGetValue(monsterID, out famil))
                            {
                                continue;
                            }
                            if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                                continue;
                            Game.MsgMonster.MonsterFamily Monster = famil.Copy();
                            Monster.SpawnX = X;
                            Monster.SpawnY = Y;
                            Monster.MaxSpawnX = (ushort)(X + XPlus);
                            Monster.MaxSpawnY = (ushort)(Y + YPlus);
                            Monster.MapID = MapID;
                            Monster.SpawnCount = (byte)Amount;
                            Monster.rest_secs = (int)respawn;
                            colletion.Add(Monster);
                        }
                        catch
                        {
                        }


                    }
                }

                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        /// <summary>
        /// [HotReload] One Monsters\*.ini file -> MonsterFamily, exactly as LoadMonsters reads it (null = invalid file,
        /// reported on the console). createLinks=false skips the item-generator/settings registration (used by a
        /// live reload, which only copies the values into the existing family). Shared with HotReload.DataHotReload.
        /// </summary>
        public static Game.MsgMonster.MonsterFamily ParseMonsterFamily(WindowsAPI.IniFile ini, string fname, bool createLinks = true)
        {
            ini.FileName = fname;
            Game.MsgMonster.MonsterFamily Family = new Game.MsgMonster.MonsterFamily();
            Family.ID = ini.ReadUInt32("cq_monstertype", "id", 0);
            Family.Name = ini.ReadString("cq_monstertype", "name", "INVALID_MOB");

            Family.Level = ini.ReadUInt16("cq_monstertype", "level", 0);
            Family.MaxAttack = ini.ReadInt32("cq_monstertype", "attack_max", 0);
            Family.MinAttack = ini.ReadInt32("cq_monstertype", "attack_min", 0);
            if (Family.Name == "INVALID_MOB" || Family.Level == 0 || Family.ID == 0 || Family.MinAttack > Family.MaxAttack)
            {
                MyConsole.WriteLine("[Error] Error Reading Monster File: " + fname + "", ConsoleColor.DarkRed);
                return null;
            }
            Family.Defense = ini.ReadUInt16("cq_monstertype", "defence", 0);
            Family.Mesh = ini.ReadUInt16("cq_monstertype", "lookface", 0);
            Family.MaxHealth = ini.ReadInt32("cq_monstertype", "life", 0);
            Family.ViewRange = Qahr.Mobile.Phone.MobileMonsterTune.ViewRange(ini, 16); // QahrMobile: attack_user=0 -> passive (MobileMonsterTune.cs)
            Family.AttackRange = ini.ReadSByte("cq_monstertype", "attack_range", 0);
            Family.Dodge = ini.ReadByte("cq_monstertype", "dodge", 0);
            Family.DropBoots = ini.ReadByte("cq_monstertype", "drop_shoes", 0);
            Family.DropNecklace = ini.ReadByte("cq_monstertype", "drop_necklace", 0);
            Family.DropRing = ini.ReadByte("cq_monstertype", "drop_ring", 0);
            Family.DropArmet = ini.ReadByte("cq_monstertype", "drop_armet", 0);
            Family.DropArmor = ini.ReadByte("cq_monstertype", "drop_armor", 0);
            Family.DropShield = ini.ReadByte("cq_monstertype", "drop_shield", 0);
            Family.DropWeapon = ini.ReadByte("cq_monstertype", "drop_weapon", 0);
            Family.DropMoney = ini.ReadUInt16("cq_monstertype", "drop_money", 0);
            Family.DropHPItem = ini.ReadUInt32("cq_monstertype", "drop_hp", 0);
            Family.DropMPItem = ini.ReadUInt32("cq_monstertype", "drop_mp", 0);
            Family.Boss = ini.ReadByte("cq_monstertype", "Boss", 0);
            Family.Defense2 = ini.ReadInt32("cq_monstertype", "defence2", 0);
            if (Family.Boss != 0)
                Family.AttackRange = 3;

            Family.MoveSpeed = ini.ReadInt32("cq_monstertype", "move_speed", 0);
            Family.AttackSpeed = ini.ReadInt32("cq_monstertype", "attack_speed", 0);
            Family.SpellId = ini.ReadUInt32("cq_monstertype", "magic_type", 0);

            Family.ExtraCritical = ini.ReadUInt32("cq_monstertype", "critical", 0);
            Family.ExtraBreack = ini.ReadUInt32("cq_monstertype", "break", 0);

            Family.extra_battlelev = ini.ReadInt32("cq_monstertype", "extra_battlelev", 0);
            Family.extra_exp = ini.ReadInt32("cq_monstertype", "extra_exp", 0);
            Family.extra_damage = ini.ReadInt32("cq_monstertype", "extra_damage", 0);


            if (Family.Boss == 0 && Family.MaxAttack > 3000)
            {
                Family.MaxAttack = Family.MaxAttack / 2;
                Family.MinAttack = Family.MinAttack / 2;
            }


            if (Family.ID == 5054) // Monster Map Auto Hunt (1 - 2 - 3 - 4 - 5)
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 4220) // Nemesis Tyrant
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 4171) // Snow Banshee
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 4212) // Thrilling Spook
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3978) // Nether Tyrant
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3976) // Bloody Banshee
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3977) // Chilling Spook
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3971) // Dragon Wraith
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3636) // Godzilla Monster
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 2700) // Golden Octopus
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3604) // Hunt Map ( 1 )
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3612) // Hunt Map ( 2 )
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3148) // Hunt Map ( 3 )
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            if (Family.ID == 3155) // Hunt Map ( 4 )
            {
                Family.MinAttack = 1;
                Family.MaxAttack = 1;
            }

            Family.DropSpecials = new Game.MsgMonster.SpecialItemWatcher[ini.ReadInt32("SpecialDrop", "Count", 0)];
            for (int i = 0; i < Family.DropSpecials.Length; i++)
            {
                string[] Data = ini.ReadString("SpecialDrop", i.ToString(), "", 32).Split(',');

                Family.DropSpecials[i] = new Game.MsgMonster.SpecialItemWatcher(uint.Parse(Data[0]), int.Parse(Data[1]));
            }

            if (createLinks)
            {
                Family.CreateItemGenerator();
                Family.CreateMonsterSettings();
            }
            return Family;
        }
        public static void LoadMonsters()
        {
            try
            {
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Monsters\\"))
                {
                    Game.MsgMonster.MonsterFamily Family = ParseMonsterFamily(ini, fname); // [HotReload] shared parser
                    if (Family == null)
                        continue;
                    try
                    {
                        Pool.MonsterFamilies.Add(Family.ID, Family);
                    }
                    catch { MyConsole.WriteLine("Error In File " + fname); }
                }
                foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\MonsterSpawns\\"))
                {
                    uint tMapID;
                    if (!uint.TryParse(fmap.Remove(0, (Program.ServerConfig.DbLocation + "\\MonsterSpawns\\").Length), out tMapID))
                        continue;
                    if (tMapID == 10364 || tMapID == 1038 || tMapID == 15757)
                        continue;
                    Game.MsgMonster.MobCollection colletion = new Game.MsgMonster.MobCollection(tMapID);
                    if (colletion.ReadMap())
                    {
                        foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                        {
                            foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                            {
                                ini.FileName = ffile;
                                colletion.LocationSpawn = ffile;

                                uint ID = ini.ReadUInt32("cq_generator", "npctype", 0);

                                Game.MsgMonster.MonsterFamily famil;
                                if (!Pool.MonsterFamilies.TryGetValue(ID, out famil))
                                {
                                    continue;
                                }
                                if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                                    continue;
                                Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                                Monster.SpawnX = ini.ReadUInt16("cq_generator", "bound_x", 0);
                                Monster.SpawnY = ini.ReadUInt16("cq_generator", "bound_y", 0);
                                Monster.MaxSpawnX = (ushort)(Monster.SpawnX + ini.ReadUInt16("cq_generator", "bound_cx", 0));
                                Monster.MaxSpawnY = (ushort)(Monster.SpawnY + ini.ReadUInt16("cq_generator", "bound_cy", 0));
                                Monster.MapID = ini.ReadUInt32("cq_generator", "mapid", 0);
                                Monster.SpawnCount = ini.ReadByte("cq_generator", "max_per_gen", 0);//"maxnpc", 0);//max_per_gen", 0);
                                Monster.rest_secs = ini.ReadInt32("cq_generator", "rest_secs", 0);


                                if (Monster.MapID == 1011 || Monster.MapID == 3071 || Monster.MapID == 1770 || Monster.MapID == 1771 || Monster.MapID == 1772
                                    || Monster.MapID == 1773 || Monster.MapID == 1774 || Monster.MapID == 1775 || Monster.MapID == 1777
                                    || Monster.MapID == 1782 || Monster.MapID == 1785 || Monster.MapID == 1786 || Monster.MapID == 1787
                                    || Monster.MapID == 1794)
                                    Monster.SpawnCount = ini.ReadByte("cq_generator", "maxnpc", 0);
                                colletion.Add(Monster);
                            }
                        }
                    }
                }

                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void LoadMapMonsters(string file)
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "MonsterSpawns\\" + file + ""))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "MonsterSpawns\\" + file + ""))
                {
                    while (true)
                    {

                        string aline = read.ReadLine();
                        if (aline != null && aline != "")
                        {
                            try
                            {
                                string[] line = aline.Split(',');
                                string name = line[2];
                                if (name.Contains("Titan"))
                                    continue;
                                ushort X = ushort.Parse(line[3]);
                                ushort Y = ushort.Parse(line[4]);
                                uint Map = uint.Parse(line[5]);
                                var GMap = Pool.ServerMaps[Map];
                                if (GMap == null) return;
                                if (GMap.MonstersColletion == null)
                                {
                                    GMap.MonstersColletion = new Game.MsgMonster.MobCollection(GMap.ID);
                                }
                                else if (GMap.MonstersColletion.DMap == null)
                                    GMap.MonstersColletion.DMap = GMap;
                                foreach (var _monster in Pool.MonsterFamilies.Values)
                                {

                                    if (_monster.Name == name)
                                    {

                                        Game.MsgMonster.MonsterFamily Monster = _monster.Copy();

                                        Monster.SpawnX = X;
                                        Monster.SpawnY = Y;
                                        Monster.MaxSpawnX = (ushort)(X + 1);
                                        Monster.MaxSpawnY = (ushort)(Y + 1);
                                        Monster.MapID = GMap.ID;
                                        Monster.SpawnCount *= 100;



                                        Game.MsgMonster.MonsterRole rolemonster = GMap.MonstersColletion.Add(Monster, false, 0, true);
                                        break;
                                    }


                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                break;
                            }
                        }
                        else
                            break;

                    }
                }
            }
        }

        public unsafe static void AddMapMonster(ServerSockets.Packet stream, Role.GameMap map, uint ID, ushort x, ushort y, ushort max_x, ushort max_y, byte count, uint DinamicID = 0, bool RemoveOnDead = false
        , Game.MsgFloorItem.MsgItemPacket.EffectMonsters m_effect = Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None, string streffect = "")
        {
            if (map.MonstersColletion == null)
            {
                map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            }
            if (map.MonstersColletion.ReadMap())
            {

                Game.MsgMonster.MonsterFamily famil;
                if (Pool.MonsterFamilies.TryGetValue(ID, out famil))
                {
                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                    Monster.SpawnX = x;
                    Monster.SpawnY = y;
                    Monster.MaxSpawnX = (ushort)(x + max_x);
                    Monster.MaxSpawnY = (ushort)(y + max_y);
                    Monster.MapID = map.ID;
                    Monster.SpawnCount = count;
                    Game.MsgMonster.MonsterRole rolemonster = map.MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);

                    if (rolemonster == null)
                    {
                        // MyConsole.WriteLine("[Error] Add Map Monster (" + ID.ToString() + ") In Class [ Server ] .", ConsoleColor.White);
                        return;
                    }


                    Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                    {
                        ObjId = rolemonster.UID,
                        Type = Game.MsgServer.ActionType.ReviveMonster,
                        PositionX = rolemonster.X,
                        PositionY = rolemonster.Y
                    };
                    rolemonster.Send(stream.ActionCreate(action));
                    rolemonster.Send(rolemonster.GetArray(stream, false));

                    if (streffect != null)
                    {
                        rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, streffect);
                    }



                    if (m_effect != Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None && rolemonster != null)
                    {
                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                        effect.m_UID = (uint)m_effect;
                        effect.m_X = rolemonster.X;
                        effect.m_Y = rolemonster.Y;
                        effect.DropType = MsgDropID.Earth;
                        rolemonster.Send(stream.ItemPacketCreate(effect));
                        rolemonster.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, "glebesword");
                    }
                    if (rolemonster.HitPoints > 65535)
                    {
                        Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, rolemonster.UID, 1);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, rolemonster.Family.MaxHealth);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, rolemonster.HitPoints);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                    }
                }
            }
        }
        public unsafe static bool AddFloor(ServerSockets.Packet stream, Role.GameMap map, uint ID, ushort x, ushort y, ushort spelllevel, Database.MagicType.Magic dbspell, Client.GameClient Owner, uint GuildID, uint OwnerUID, uint DinamicID = 0, string Name = "", bool RemoveOnDead = true)
        {
            try
            {
                if (map.MonstersColletion == null)
                {
                    map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                }
                if (map.MonstersColletion.ReadMap())
                {

                    Game.MsgMonster.MonsterFamily famil;
                    if (Pool.MonsterFamilies.TryGetValue(1, out famil))
                    {
                        Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                        Monster.SpawnX = x;
                        Monster.SpawnY = y;
                        Monster.MaxSpawnX = (ushort)(x + 1);
                        Monster.MaxSpawnY = (ushort)(y + 1);
                        Monster.MapID = map.ID;
                        Monster.SpawnCount = 1;
                        Game.MsgMonster.MonsterRole rolemonster = map.MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);
                        if (rolemonster == null)
                        {
                            //invalid x ,y
                            return false;
                        }
                        rolemonster.Family.ID = ID;
                        rolemonster.IsFloor = true;
                        rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(7);
                        rolemonster.Family.Settings = Game.MsgMonster.MonsterSettings.Lottus;

                        rolemonster.FloorPacket = new MsgItemPacket();
                        rolemonster.FloorPacket.m_UID = rolemonster.UID;
                        rolemonster.FloorPacket.m_ID = ID;
                        rolemonster.FloorPacket.m_X = x;
                        rolemonster.FloorPacket.m_Y = y;
                        rolemonster.FloorPacket.MaxLife = 25;
                        rolemonster.FloorPacket.Life = 25;
                        rolemonster.FloorPacket.DropType = MsgDropID.Effect;
                        rolemonster.FloorPacket.m_Color = 13;
                        rolemonster.FloorPacket.ItemOwnerUID = OwnerUID;
                        rolemonster.FloorPacket.GuildID = GuildID;
                        rolemonster.FloorPacket.FlowerType = 2;//2;
                        rolemonster.FloorPacket.Timer = Role.Core.TqTimer(rolemonster.FloorStampTimer);
                        rolemonster.FloorPacket.Name = Name;

                        rolemonster.DBSpell = dbspell;
                        rolemonster.Family.MaxHealth = 25;
                        rolemonster.HitPoints = 25;
                        rolemonster.OwnerFloor = Owner;
                        rolemonster.SpellLevel = spelllevel;


                        if (rolemonster == null)
                        {
                            Console.WriteLine("Eror monster spawn. Server.");
                            return false;
                        }
                        map.View.EnterMap<Role.IMapObj>(rolemonster);
                        rolemonster.Send(rolemonster.GetArray(stream, false));
                        return true;
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return false;

        }


        public unsafe static void LoadDatabase()
        {
            try
            {
                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                {
                    WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile(fname);
                    IniFile.FileName = fname;
                    string name = IniFile.ReadString("Character", "Name", "");
                    Pool.NameUsed.Add(name.GetHashCode());
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }

        public unsafe static void SaveDatabase()
        {
            if (!FullLoading)
                return;
            try
            {

                Role.Instance.Clan.ProcessChangeNames();
                Role.Instance.Guild.ProcessChangeNames();

                Save(new Action(Database.JianHuTable.SaveJiangHu));
                Save(new Action(Role.Instance.Associate.Save));
                Save(new Action(Database.GuildTable.Save));
                Save(new Action(Database.ClanTable.Save));
                Save(new Action(Pool.QueueContainer.Save));
                Save(new Action(SaveDeitylandSacrifice));
                Save(new Action(Game.MsgTournaments.MsgSchedules.GuildWar.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Save));
                Save(new Action(TheCrimeTable.Save));
                Save(new Action(Pool.Arena.Save));
                Save(new Action(YuanshenRank.Save));
                Save(new Action(HWRank.Save));
                Save(new Action(NinjaRank.Save));
                Save(new Action(WarriorRank.Save));
                Save(new Action(ArcherRank.Save));
                Save(new Action(WaterRank.Save));
                Save(new Action(MonkRanks.Save));
                Save(new Action(FireRank.Save));
                Save(new Action(PirateRank.Save));
                Save(new Action(LeeLongRank.Save));
                Save(new Action(DuneWandererRank.Save));
                Save(new Action(AstredgeRank.Save));
                Save(new Action(HeroGatheringRank.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.ClassPkWar.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.ElitePkTournament.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.TeamPkTournament.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Save));
                Save(new Action(SystemBanned.Save));
                Save(new Action(SystemBannedPC.Save));
                Save(new Action(TableBetDiceRank.Save));
                Save(new Action(RankMonster.Save));
                Save(new Action(SystemBannedAccount.Save));
                Save(new Action(InnerPowerTable.Save));
                // [modularised] ShareVIP save now runs from Modules\Custom\ShareVIP\ShareVIPModule.OnSave.
                Save(new Action(LeagueTable.Save));
                // [modularised] RechargeShop save now runs from Modules\Custom\RechargeShop\RechargeShopModule.OnSave.
                Save(new Action(RankItems.SaveRanks));
                Save(new Action(Role.Statue.Save));
                Save(new Action(PrestigeRanking.Save));
                Save(new Action(Role.KOBoard.KOBoardRanking.Save));
                Save(new Action(MsgInterServer.Instance.CrossElitePKTournament.Save));
                Save(new Action(Pool.Insults.Save));
                Save(new Action(RuneRank.Save));
                WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile("");
                // Must match the READ path (Initialize reads from BaseDirectory). Using GetCurrentDirectory
                // here caused the counter to be written to a different file than it is read from, which
                // re-used UIDs on every restart whenever the working directory differed from BaseDirectory.
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!baseDir.EndsWith("\\")) baseDir += "\\";
                IniFile.FileName = baseDir + "shell.ini";
                IniFile.Write<uint>("Database", "ItemUID", Pool.ITEM_Counter.Count);
                IniFile.Write<uint>("Database", "DominoUID", Pool.DominoCounter.Count);
                IniFile.Write<uint>("Database", "ClientUID", Pool.ClientCounter.Count);
                IniFile.Write<uint>("Database", "Day", ResetServerDay);


            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        // DeityAltar: حفظ/تحميل الـ Jades المضحّى بيها (كانت في الذاكرة بس وبتضيع مع الريستارت)
        public static void SaveDeitylandSacrifice()
        {
            try
            {
                KeyValuePair<uint, Pool.DeitylandSacrificeRanking>[] snapshot;
                lock (Pool.DeitylandSacrificeRankings)
                    snapshot = Pool.DeitylandSacrificeRankings.ToArray();
                using (Database.DBActions.Write _wr = new Database.DBActions.Write("DeitylandSacrifice.txt"))
                {
                    foreach (var obj in snapshot)
                    {
                        if (obj.Value == null) continue;
                        var writer = new Database.DBActions.WriteLine('/');
                        writer.Add(obj.Key).Add(obj.Value.Jades).Add((obj.Value.Name ?? "").Replace("/", ""));
                        _wr.Add(writer.Close());
                    }
                    _wr.Execute(Database.DBActions.Mode.Open);
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void LoadDeitylandSacrifice()
        {
            try
            {
                using (Database.DBActions.Read r = new Database.DBActions.Read("DeitylandSacrifice.txt"))
                {
                    if (r.Reader(false)) // false: الملف مش موجود أول مرة = عادي
                    {
                        int count = r.Count;
                        for (int x = 0; x < count; x++)
                        {
                            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(r.ReadString(""), '/');
                            uint uid = reader.Read((uint)0);
                            uint jades = reader.Read((uint)0);
                            string name = reader.Read("");
                            if (uid == 0 || Pool.DeitylandSacrificeRankings.ContainsKey(uid))
                                continue;
                            Pool.DeitylandSacrificeRankings.Add(uid, new Pool.DeitylandSacrificeRanking() { Name = name, Jades = jades, Prize = "" });
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        public static void Save(Action obj)
        {
            try
            {
                obj.Invoke();
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        /// <summary>
        /// [HotReload] Reads portals.ini and calls onPortal(section map id, portal) for each portal, exactly as
        /// LoadPortals always read it. Throws on a malformed file (same as before).
        /// Shared by LoadPortals and HotReload.DataHotReload.
        /// </summary>
        public static void ParsePortals(System.IO.TextReader read, Action<ushort, Role.Portal> onPortal)
        {
            while (true)
            {
                string lines = read.ReadLine();
                if (lines == null)
                    break;
                ushort Map = ushort.Parse(lines.Split('[')[1].ToString().Split(']')[0]);
                ushort Count = ushort.Parse(read.ReadLine().Split('=')[1]);
                for (ushort x = 0; x < Count; x++)
                {
                    Role.Portal portal = new Role.Portal();
                    string[] line = read.ReadLine().Split('=')[1].Split(' ');
                    portal.MapID = ushort.Parse(line[0]);
                    portal.X = ushort.Parse(line[1]);
                    portal.Y = ushort.Parse(line[2]);

                    string[] dline = read.ReadLine().Split('=')[1].Split(' ');
                    portal.Destiantion_MapID = ushort.Parse(dline[0]);
                    portal.Destiantion_X = ushort.Parse(dline[1]);
                    portal.Destiantion_Y = ushort.Parse(dline[2]);
                    onPortal(Map, portal);
                }
            }
        }
        public static void LoadPortals(uint id = 0)
        {

            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "portals.ini"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "portals.ini"))
                {
                    ParsePortals(read, (Map, portal) =>
                    {
                        if (id != 0 && id != Map) return;
                        if (Pool.ServerMaps.Base.ContainsKey(portal.MapID))
                            Pool.ServerMaps[portal.MapID].Portals.Add(portal);
                    });
                }
            }
            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
    }
}