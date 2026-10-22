using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Qahr.Mobile.MsgInterServer.Packets;
using ProtoBuf;
using Qahr.Mobile.DBFunctionality;
namespace Qahr.Mobile.Game.MsgServer
{
    using System.IO;
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, MsgDataPacket.DataAttribute, ushort>;
    using Qahr.Mobile.Game.MsgFloorItem;
    using Qahr.Mobile.Game.MsgNpc;
    using Qahr.Mobile.Database;
    using Qahr.Mobile.Role.Instance;
    using System.Runtime.InteropServices.ComTypes;

    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data);

    public class CustomCommands
    {
        public const ushort
            ExitQuestion = 1,
            Minimize = 2,
            ShowReviveButton = 1053,
            FlowerPointer = 1067,
            Enchant = 1091,
            LoginScreen = 1153,
            SelectRecipiet = 30,
            JoinGuild = 34,
            MakeFriend = 38,
            ChatWhisper = 40,
            CloseClient = 43,
            HotKey = 53,
            Furniture = 54,
            TQForum = 79,
            PathFind = 97,
            LockItem = 102,
            ShowRevive = 1053,
            HideRevive = 1054,
            StatueMaker = 1066,
            GambleOpen = 1077,
            GambleClose = 1078,
            Compose = 1086,
            Craft1 = 1088,
            Craft2 = 1089,
            Warehouse = 1090,
            ShoppingMallShow = 1100,
            ShoppingMallHide = 1101,
            NoOfflineTraining = 1117,
            Interact = 1122,
            CenterClient = 1155,
            ClaimCP = 1197,
            ClaimAmount = 1198,
            MerchantApply = 1201,
            MerchantDone = 1202,
            RedeemEquipment = 1233,
            ClaimPrize = 1234,
            RepairAll = 1239,
            FlowerIcon = 1244,
            SendFlower = 1246,
            ReciveFlower = 1248,
            WarehouseVIP = 1272,
            UseExpBall = 1288,
            HackProtection = 1298,
            HideGUI = 1307,
            Inscribe = 3059,
            BuyPrayStone = 3069,
            HonorStore = 3104,
            Opponent = 3107,
            CountDownQualifier = 3109,
            QualifierStart = 3111,
            ItemsReturnedShow = 3117,
            ItemsReturnedWindow = 3118,
            ItemsReturnedHide = 3119,
            QuestFinished = 3147,
            QuestPoint = 3148,
            QuestPointSparkle = 3164,
            StudyPointsUp = 3192,
            Updates = 3218,
            IncreaseLineage = 3227,
            HorseRacingStore = 3245,
            GuildPKTourny = 3249,
            QuitPK = 3251,
            Spectators = 3252,
            CardPlayOpen = 3270,
            CardPlayClost = 3271,
            ArtifactPurification = 3344,
            SafeguardConvoyShow = 3389,
            SafeguardConvoyHide = 3390,
            RefineryStabilization = 3392,
            ArtifactStabilization = 3398,
            SmallChat = 3406,
            NormalChat = 3407,
            Reincarnation = 3439;
    }
    public class DialogCommands
    {
        public const ushort
            Compose = 1,
            Craft = 2,
            Warehouse = 4,
            DetainRedeem = 336,
            DetainClaim = 337,
            VIPWarehouse = 341,
            Breeding = 368,
            PurificationWindow = 455,
            StabilizationRifinery = 448,
            StabilizationWindow = 459,
            TalismanUpgrade = 347,
            GemComposing = 422,
            OpenSockets = 425,
            Blessing = 426,
            TortoiseGemComposing = 438,
            RefineryStabilization = 448,

            HorseRacingStore = 464,
            Reincarnation = 485,
            ChangeName = 489,
            GarmentShop = 502,
            DegradeEquip = 506,
            Browse = 572,
            JiangHuSetName = 617,
            SendTwinCityTime = 538,
            BrowseAuction = 572,
            HowGetStudy = 595,
            TheFactionWar = 599, //. 1072 packet
            ResetSecundaryPassword = 639,
            NewLottery = 656, // Packet . 2804
            CreateUnion = 693,
            SetKingdomTitle = 700,
            Hairstyle = 741,
            ChangeNameUnion = 723,
            RuneDecompose = 904,
            EmbedRune = 930,
            TrojanWeaponArchiveLook = 978,
              GouYuArchiveArt = 1209,
            TrojanWeaponArchive = 980;

    }
    public class ActionType
    {
        public const ushort RemoveTrap = 434,
        UpdateSpell = 252,
        UpdateProf = 253,
        DragonBall = 165,
        OpenGuiNpc = 160,
        AutoPatcher = 162,
        Description = 181,
        CountDown = 159,
        HideGui = 158,
        ChangeLookface = 151,
        SetLocation = 74,
        Hotkeys = 75,
        ConfirmAssociates = 76,
        NewChatTalking = 456,
        ConfirmProficiencies = 77,
        ConfirmSpells = 78,
        ChangeDirection = 79,
        ChangeStance = 81,
        ChangeMap = 85,
        AnimaCrest = 258,
        Teleport = 86,
        Leveled = 92,
        Revive = 94,
        DeleteCharacter = 95,
        SetPkMode = 96,
        ConfirmGuild = 97,
        LocationTeamLieder = 101,
        RequestEntity = 102,
        SetMapColor = 104,
        TeamSearchForMember = 106,
        RemoveSpell = 109,
        StartVendor = 111,
        StopVending = 114,
        OpenCustom = 116,
        ViewEquipment = 117,
        EndTransformation = 118,
        EndFly = 120,
        ViewEnemyInfo = 123,
        OpenDialog = 126,
        CompleteLogin = 132,
        ReviveMonster = 134,
        RemoveEntity = 135,
        Jump = 137,
        Ghost = 145,
        ViewFriendInfo = 148,
        ChangeFace = 151,
        TradePartnerInfo = 152,
        AbortMagic = 163,
        Bulletin = 166,
        PokerTeleporter = 167,
        FlashStep = 156,
        Away = 161,
        Pick = 164,
        ClikerON = 171,
        ClikerEntry = 172,
        SetAppearanceType = 178,
        PoketTableBet = 234,
        AllowAnimation = 251,
        CreditGifts = 255,
        UpdateInventorySash = 256,
        QuerySpawn = 310,
        QueryEquipment = 408,
        BeginSteedRace = 401,
        FinishSteedRace = 402,
        SubmitGoldBrick = 436,
        AddBlackList = 440,
        RemoveBlackList = 441,
        DrawStory = 443,
        PetAttack = 447,
        DuelAction = 449,
        PortalInvite = 451,
        PortalConfirm = 452,
        SaizoNpc = 456,
        InspiredPirate = 457,
        ViewNinjaInspired = 461,
        GiveOrder = 466,
        ReturnReward = 470,
        ExchangeClan = 479,
        ShowFriend = 482,
        ShowPartner = 483,
        GetPrizeFlower = 485,
        DunePower = 521;


    }
    [ProtoContract]
    public class ActionQuery
    {
        [ProtoMember(1, IsRequired = true)]
        public uint ObjId;
        [ProtoMember(2)]
        public uint TargetUID;
        [ProtoMember(3)]
        public uint dwParam;
        [ProtoMember(4)]
        public int dwParam2;
        [ProtoMember(5)]
        public ulong dwParam3;
        [ProtoMember(6)]
        public bool SucDone;
        [ProtoMember(7)]
        public uint TargetPositionX;
        [ProtoMember(8)]
        public uint TargetPositionY;
        [ProtoMember(9, IsRequired = true)]
        public uint Timestamp;
        [ProtoMember(10)]
        public uint NpcID; // [ProtoFix 2026-10-14] client CMsgActionPB #10 is uint32 (was int)
        [ProtoMember(11)]
        public uint CheatLogData1;
        [ProtoMember(12, IsRequired = true)]
        public ushort Type;
        [ProtoMember(13, IsRequired = true)]
        public ushort Fascing;
        [ProtoMember(14, IsRequired = true)]
        public uint PositionX;
        [ProtoMember(15, IsRequired = true)]
        public uint PositionY;
        [ProtoMember(16)]
        public uint CheatLogData2;
        [ProtoMember(17)]
        public uint MapID;
        [ProtoMember(18)]
        public uint CheatLogData3;
        [ProtoMember(19)]
        public int ScheduleTime;
        [ProtoMember(20)]
        public int LinkIndex;
        [ProtoMember(21)]
        public uint CheatLogData4;
        [ProtoMember(22)]
        public int RemainTime;
        [ProtoMember(23)]
        public bool MountCharge;
        [ProtoMember(24)]
        public string[] Strings;

    }
    // Shared diagnostic log for unhandled packets/subtypes -> <exe folder>\unknown_packets.txt.
    // ServerConfig.ini [Diagnostics] LogUnknownPackets=1 (default 0 = off). Each key is written at most once per minute with its count.
    public static class UnknownPacketLog
    {
        public static readonly bool Enabled = SharedConfig.Int("Diagnostics", "LogUnknownPackets", 0) != 0;
        private class Entry { public int Count; public DateTime LastWrite; }
        private static readonly Dictionary<string, Entry> Keys = new Dictionary<string, Entry>();
        private static readonly object Sync = new object();
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unknown_packets.txt");

        public static void Log(string key, string player, string detail = null)
        {
            if (!Enabled) return;
            try
            {
                lock (Sync)
                {
                    DateTime now = DateTime.Now;
                    Entry e;
                    if (!Keys.TryGetValue(key, out e))
                    {
                        if (Keys.Count > 5000) Keys.Clear();
                        e = new Entry() { LastWrite = DateTime.MinValue };
                        Keys[key] = e;
                    }
                    e.Count++;
                    if ((now - e.LastWrite).TotalSeconds < 60) return;
                    File.AppendAllText(FilePath, now.ToString("yyyy-MM-dd HH:mm:ss") + " " + key + " count=" + e.Count
                        + " player=" + (player ?? "?") + (detail != null ? " " + detail : "") + Environment.NewLine);
                    e.Count = 0;
                    e.LastWrite = now;
                }
            }
            catch { }
        }
        public static string Cut(string s, int max = 80)
        {
            if (s == null) return "";
            return s.Length > max ? s.Substring(0, max) : s;
        }
    }
    public unsafe static class MsgDataPacket
    {
        public static unsafe ServerSockets.Packet ActionPick(this ServerSockets.Packet stream, uint UID, ushort dwparam2, ushort timer, string text)
        {
            return ActionCreate(stream, new ActionQuery() { ObjId = UID, dwParam = 220, Type = ActionType.Pick, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), Fascing = dwparam2, RemainTime = timer, Strings = new string[1] { text } });
        }

        public static unsafe void Action(this ServerSockets.Packet stream, out ActionQuery pQuery)
        {
            pQuery = new ActionQuery();
            pQuery = stream.ProtoBufferDeserialize<ActionQuery>(pQuery);
        }
        public static unsafe ServerSockets.Packet ActionCreate(this ServerSockets.Packet stream, ActionQuery pQuery)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pQuery);
            stream.Finalize(GamePackets.DataMap);
            return stream;
        }
        public class DataAttribute : Attribute
        {
            public static readonly Func<DataAttribute, ushort> Translator = (a) => a.Type;
            public ushort Type { get; private set; }
            public DataAttribute(ushort type)
            {
                this.Type = type;
            }
        }

        public static ActionInvoker invoker = new ActionInvoker(DataAttribute.Translator);

        [PacketAttribute(GamePackets.DataMap)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                Client.GameClient InGame;
                if (Pool.GamePoll.TryGetValue(user.Player.UID, out  InGame))
                {
                    if (InGame.Socket != user.Socket)
                    {
                        user.Socket.Disconnect();
                        InGame.Socket.Disconnect();
                        return;
                    }
                }
                else
                {
                    user.Socket.Disconnect();
                    return;
                }
                ActionQuery data;
                stream.Action(out data);

                Tuple<DataAttribute, ProcessAction> processFolded;
                if (invoker.TryGetInvoker(data.Type, out processFolded))
                {
                    // MyConsole.WriteLine("find  " + data.Type + " ");
                    processFolded.Item2(user, stream, data);
                }
                else
                {
                    // QahrMobile: Welfare > Daily Packs (467 buy/free, 468 week reward, 469 buy all) has no DataAttribute;
                    // routed from here so it works without a restart (466/470 come through GiveOrder/ReturnReward).
                    if (global::Qahr.Mobile.Phone.MobileDailyGift.Handle(user, stream, data))
                        return;

#if TEST
                        if (user.ProjectManager)
                            MyConsole.WriteLine("DataMap not find  " + data.Type + " ");
#endif
                    if (UnknownPacketLog.Enabled)
                    {
                        switch (data.Type)
                        {
                            // client anti-cheat reports
                            case 127: case 129: case 131: case 133: case 136: case 139:
                            case 141: case 142: case 144: case 150: case 173:
                                UnknownPacketLog.Log("CHEAT-REPORT 2099/" + data.Type + " " + user.Player.Name, user.Player.Name,
                                    "data=" + data.CheatLogData1 + "," + data.CheatLogData2 + "," + data.CheatLogData3 + "," + data.CheatLogData4);
                                break;
                            default:
                                UnknownPacketLog.Log("2099/" + data.Type, user.Player.Name);
                                break;
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        [DataAttribute(ActionType.SubmitGoldBrick)]
        public unsafe static void SubmitGoldBrick(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.InUnion)
            {
               
            }
        }
        [DataAttribute(ActionType.Bulletin)]
        public unsafe static void Bulletin(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (!client.Player.Alive)
                return;
            if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                  || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
              || client.Player.Map == 1806
                  || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                  || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
          || client.Player.Map == 1768        
          || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507)
            {
                return;
            }

            if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                || client.Player.Map == 6001)
            {

                return;
            }
            if (Constants.BlockTeleportMap.Contains(client.Player.Map))
                return;
            //bulletin
            switch (data.dwParam)
            {

                case 68://The Standout
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        if (client.Player.Reborn == 2)
                            client.Teleport(94, 410, 10137);
                        else client.SendSysMesage("You are not strong enough to explore the Dragon Island. Please come when you get 2nd-reborn.");
                        break;
                    }
                case 1165://Daily Quest
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(274, 470, 1002);
                        break;
                    }
                case 25://DietyLand
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        if (client.Player.Reborn == 2)
                            client.Teleport(453, 483, 10137);
                        else client.SendSysMesage("You should reach the required level of this event to enable the teleport function.");
                        break;
                    }
                case 105://house Map
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(344, 528, 1002);

                        break;
                    }
                case 91://Cross-server
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(320, 324, 1002);
                        break;
                    }
                case 104: //Tower of Mystery
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(258, 473, 1002);

                        break;
                    }
                case 8: //Dis City
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(273, 482, 1002);
                        break;
                    }
                case 98: //Evil Labyrinth
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(213, 379, 1002);
                        break;
                    }
                case 82: //Class PK War
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(452, 294, 1002);
                        break;
                    }
                case 1170: //Blood Championship
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(346, 329, 1002);
                        break;
                    }
                case 18: case 19: case 20: //lower level tiers (Activity.ini)
                case 21: //Skill Team PK Tournament
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(448, 272, 1002);
                        break;
                    }
                case 9: case 10: case 11: //lower level tiers (Activity.ini)
                case 12: //Elite PK Tournament
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(438, 272, 1002);
                        break;
                    }
                case 267: //Chasm Of Chaos P7 Soul Again, Map 
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(451, 483, 10137);
                        break;
                    }
                case 1: //Horse Racing
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(388, 471, 1002);
                        break;
                    }
                case 16: //FlameLit GuildWar Exp's Fast ! In TC, Map 
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(249, 217, 1002);

                        break;
                    }
                case 17: // GuildWarPK CpS's Fast ! In TC, Map  //Battlefield Heroes
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(301, 155, 1002);
                        break;
                    }
                case 38: //Decisive Strike
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(261, 439, 1002);
                        break;
                    }
                case 15: //Capture the Flag
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(320, 318, 1002);

                        break;
                    }
                case 14: //Weekly PK War
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(421, 279, 1002);

                        break;
                    }
                case 22: //Treasure in the Blue
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(306, 550, 1002);

                        break;
                    }
                case 2: case 3: case 4: //lower level tiers (Activity.ini)
                case 5: //Team PK Tournament
                    {
                        if (client.Player.Map == 10137 || client.Player.Map == 10250)
                        {
                            client.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }
                        client.Teleport(443, 271, 1002);

                        break;
                    }

            }
        }
        [DataAttribute(ActionType.RemoveBlackList)]
        public unsafe static void RemoveBlackList(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            string TargetName = data.Strings.FirstOrDefault();
            foreach (var user in Pool.GamePoll.Values)
            {
                if (user.Player.Name == TargetName)
                {
                    data.ObjId = user.Player.UID;
                    break;
                }
            }
            data.dwParam = 1;
            data.Strings = new string[1] { TargetName };
            data.ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.AddBlackList)]
        public unsafe static void AddBlackList(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            string TargetName = data.Strings.FirstOrDefault();
            foreach (var user in Pool.GamePoll.Values)
            {
                if (user.Player.Name == TargetName)
                {
                    data.ObjId = user.Player.UID;
                    break;
                }
            }
            data.dwParam = 1;
            data.Strings = new string[1] { TargetName };
            data.ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));

        }
        [DataAttribute(ActionType.PortalInvite)]
        public unsafe static void PortalInvite(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (Pool.GamePoll.ContainsKey(data.dwParam))
            {
                var owner = Pool.GamePoll[data.dwParam];

                // [Audit H4] A player may only jump to a SpaceLeap portal that belongs to
                // themselves or to a member of their own team. This replaces "teleport to
                // ANY online player's portal with no invite".
                bool authorized = owner.Player.UID == client.Player.UID
                    || (client.Team != null && client.Team.IsTeamMember(owner.Player.UID))
                    || (owner.Team != null && owner.Team.IsTeamMember(client.Player.UID));
                if (!authorized)
                    return;

                // [Audit H4] must be alive.
                if (!client.Player.Alive)
                    return;

                // [Audit H4] cannot escape a locked map (prison / arena / qualifier / event).
                if (Pool.Constants.BlockTeleportMap.Contains(client.Player.Map))
                    return;

                if (owner.Player.FloorSpells.ContainsKey((ushort)Role.Flags.SpellID.SpaceLeap) && owner.Player.FloorSpells[(ushort)Role.Flags.SpellID.SpaceLeap].Spells.Count > 0)
                {

                    var floor = owner.Player.FloorSpells[(ushort)Role.Flags.SpellID.SpaceLeap].Spells.FirstOrDefault();

                    // [Audit H4] and cannot teleport INTO a blocked map either.
                    if (Pool.Constants.BlockTeleportMap.Contains(floor.FloorPacket.MapID))
                        return;

                    ushort X = floor.FloorPacket.m_X, Y = floor.FloorPacket.m_Y;
                    Pool.ServerMaps[floor.FloorPacket.MapID].GetRandCoord(ref X, ref Y, 7);
                    client.Teleport(X, Y, floor.FloorPacket.MapID);

                    owner.Send(msg.ActionCreate(new ActionQuery() { TargetUID = owner.Player.UID, Type = ActionType.PortalConfirm, ObjId = client.Player.UID, Timestamp = (uint)Time32.timeGetTime().GetHashCode() }));
                }
            }

        }
        [DataAttribute(ActionType.PetAttack)]
        public unsafe static void PetAttack(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
            {
                var pet = obj as Game.MsgMonster.MonsterRole;
                if (pet.UID == data.NpcID)
                {
                    // [Audit M2] the "pet" must actually belong to this player - otherwise any
                    // monster or another player's pet could be used as an attack source.
                    if (pet.OwnerUID != client.Player.UID)
                        return;
                    if (DateTime.Now >= pet.AttackSpeed)
                    {
                        uint Experience = 0;
                        Role.IMapObj target;
                        if (client.Player.View.TryGetValue(data.TargetUID, out target, Role.MapObjectType.Monster))
                        {
                            // [Audit M2] target must be within the pet's reach.
                            if (Role.Core.GetDistance(pet.X, pet.Y, target.X, target.Y) > 18)
                                return;
                            var DBSpell = Pool.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, DBSpell))
                            {

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                             , 0, attacked.X, attacked.Y, DBSpell.ID
                                             , DBSpell.Level, 0);


                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, attacked, null, out AnimationObj
                                    , (byte)(pet.ContainFlag(MsgUpdate.Flags.Thunderbolt) ? 2 : 0));
                                AnimationObj.Damage = (uint)(AnimationObj.Damage * Pool.Magic[(ushort)Role.Flags.SpellID.Thundercloud][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Thundercloud].Level].Damage2 / 100);
                                AnimationObj.Damage = MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                Experience += MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(msg, AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                                pet.AttackSpeed = DateTime.Now.AddMilliseconds(pet.Family.AttackSpeed);
                            }

                        }
                        else if (client.Player.View.TryGetValue(data.TargetUID, out target, Role.MapObjectType.Player))
                        {
                            // [Audit M2] target must be within the pet's reach.
                            if (Role.Core.GetDistance(pet.X, pet.Y, target.X, target.Y) > 18)
                                return;
                            var DBSpell = Pool.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            var attacked = target as Role.Player;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, DBSpell))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                            , 0, attacked.X, attacked.Y, DBSpell.ID
                                            , DBSpell.Level, 0);
                                int extra = DBSpell.Damage2;
                                MsgSpell myspell;
                                if (pet.ContainFlag(MsgUpdate.Flags.Thunderbolt) && client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Thunderbolt, out myspell))
                                    extra = 10 * (myspell.Level + 1);

                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = (uint)(AnimationObj.Damage * Pool.Magic[(ushort)Role.Flags.SpellID.Thundercloud][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Thundercloud].Level].Damage2 / 100);
                                AnimationObj.Damage = MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                                pet.AttackSpeed = DateTime.Now.AddMilliseconds(pet.Family.AttackSpeed);
                            }
                        }
                        else if (client.Player.View.TryGetValue(data.TargetUID, out target, Role.MapObjectType.SobNpc))
                        {
                            // [Audit M2] target must be within the pet's reach.
                            if (Role.Core.GetDistance(pet.X, pet.Y, target.X, target.Y) > 18)
                                return;
                            var DBSpell = Pool.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            var attacked = target as Role.SobNpc;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, DBSpell))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                         , 0, attacked.X, attacked.Y, DBSpell.ID
                                         , DBSpell.Level, 0);

                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, attacked, null, out AnimationObj);
                                AnimationObj.Damage = (uint)(AnimationObj.Damage * Pool.Magic[(ushort)Role.Flags.SpellID.Thundercloud][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Thundercloud].Level].Damage2 / 100);
                                Experience += MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(msg, AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                                pet.AttackSpeed = DateTime.Now.AddMilliseconds(pet.Family.AttackSpeed);
                            }
                        }
                        MsgServer.AttackHandler.Updates.IncreaseExperience.Up(msg, client, Experience);
                    }
                }
            }
        }

        /// <summary>
        /// Safely resolves the ClikerEntry target monster for a quest kill.
        /// Returns true only when the client-supplied UID is an actual monster in view,
        /// is NOT a boss/guard, is close enough to the player (server-side position),
        /// and matches the quest monster name. This stops killing arbitrary monsters
        /// (bosses / event mobs) in a loop. [Audit C1]
        /// </summary>
        private static bool TryGetQuestMonster(Client.GameClient client, ActionQuery data, string requiredName, bool nameContains, out Game.MsgMonster.MonsterRole monster)
        {
            monster = null;
            Role.IMapObj target;
            if (!client.Player.View.TryGetValue(data.dwParam, out target, Role.MapObjectType.Monster))
                return false;
            var mob = target as Game.MsgMonster.MonsterRole;
            if (mob == null || mob.Name == null)
                return false;
            // never allow clicker-kills on bosses / guards / call-pets
            if (mob.Boss != 0 || (mob.Family != null && mob.Family.Boss != 0))
                return false;
            // must be adjacent/near the player's SERVER-side position (anti teleport-kill)
            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, mob.X, mob.Y) > 3)
                return false;
            if (nameContains)
            {
                if (!mob.Name.Contains(requiredName))
                    return false;
            }
            else
            {
                if (mob.Name != requiredName)
                    return false;
            }
            monster = mob;
            return true;
        }

        [DataAttribute(ActionType.ClikerEntry)]
        public unsafe static void ClikerEntry(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            //dwparam target.
            if (client.Player.QuestGUI.CheckQuest(6131, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Game.MsgMonster.MonsterRole monster;
                if (TryGetQuestMonster(client, data, "BanditL98", true, out monster))
                {
                    client.Player.QuestGUI.IncreaseQuestObjectives(msg, 6131, 0, 1);
                    client.Player.QuestGUI.IncreaseQuestObjectives(msg, 6131, 1, 1);

                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);

                    data.ObjId = data.dwParam;
                    data.dwParam = 0;
                    data.Type = 174;
                    client.Send(msg.ActionCreate(data));
                }
            }
            else if (client.Player.QuestGUI.CheckQuest(1355, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Game.MsgMonster.MonsterRole monster;
                if (TryGetQuestMonster(client, data, "Thunder", true, out monster))
                {
                    client.SendSysMesage("Now you can report back to Poison Master.", MsgMessage.ChatMode.System);
                    client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1355, 1);

                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);

                    data.ObjId = data.dwParam;
                    data.dwParam = 0;
                    data.Type = 174;
                    client.Send(msg.ActionCreate(data));
                }
            }
            else if (client.Player.QuestGUI.CheckQuest(1487, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Game.MsgMonster.MonsterRole monster;
                if (TryGetQuestMonster(client, data, "RockMonsterL78", false, out monster))
                {
                    client.CreateBoxDialog("The~Level~78~Rock~Monsters~are~afraid~of~the~Cactus~and~run~away.~Hurry~to~report~to~Convoy~Vice~Leader~Ling.");
                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);
                    client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1487, 1);
                }
            }
            else if (client.Player.QuestGUI.CheckQuest(1330, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Game.MsgMonster.MonsterRole monster;
                if (TryGetQuestMonster(client, data, "ThunderApe", false, out monster))
                {
                    if (!client.Player.ActivePick)
                    {
                        client.Player.QuestCaptureType = 1;
                        client.Player.AddPick(msg, "Capture~Thunder~Ape.", 2);
                    }

                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);

                    data.ObjId = data.dwParam;
                    data.dwParam = 0;
                    data.Type = 174;
                    client.Send(msg.ActionCreate(data));
                }
                else if (TryGetQuestMonster(client, data, "ThunderApeL58", false, out monster))
                {
                    if (!client.Player.ActivePick)
                    {
                        client.Player.QuestCaptureType = 2;
                        client.Player.AddPick(msg, "Capture~Thunder~ApeL58.", 2);
                    }

                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);

                    data.ObjId = data.dwParam;
                    data.dwParam = 0;
                    data.Type = 174;
                    client.Send(msg.ActionCreate(data));
                }
            }
        }
        [DataAttribute(ActionType.PokerTeleporter)]
        public unsafe static void PokerTeleporter(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                     || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                 || client.Player.Map == 1806
                     || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                     || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
             || client.Player.Map == 1768
            
             || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 12022 || client.Player.Map == 1508 || client.Player.Map == 1507)
            {
#if Arabic
                                       client.SendSysMesage("The Poker room is not allowed on this map.");
#else
                client.SendSysMesage("The Poker room is not allowed on this map.");
#endif

                return;
            }

            if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                || client.Player.Map == 6001)
            {
#if Arabic
                   client.SendSysMesage("The Poker room is not allowed on this map.");
#else
                client.SendSysMesage("The Poker room is not allowed on this map.");
#endif

                return;
            }
            if (Pool.Constants.BlockTeleportMap.Contains(client.Player.Map))
                return;
            switch (data.dwParam)
            {
                case 4://roulete
                    {
#if Roullete
                        client.Teleport(56, 65, 3852);
#endif
                        break;
                    }
#if Poker
                case 2://cp room
                    {
                        client.Teleport(58, 66, 1860);
                        break;
                    }
                case 1://gold room
                    {
                        client.Teleport(157, 119, 1858);
                        break;
                    }
#endif
                case 3://one bandit
                    {
                       // client.Teleport(240, 241, 1036);
                        break;
                    }
            }

        }
        [DataAttribute(ActionType.CreditGifts)]
        public unsafe static void CreditGifts(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if ((client.Player.MainFlag & Role.Player.MainFlagType.CanClaim) == Role.Player.MainFlagType.CanClaim)
            {
                if ((client.Player.MainFlag & Role.Player.MainFlagType.ClaimGift) != Role.Player.MainFlagType.ClaimGift)
                {
                    if (client.Inventory.HaveSpace(9))
                    {
                        //client.Inventory.Add(msg, 4200012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        //client.Inventory.Add(msg, 4200012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                        //client.Player.MainFlag |= Role.Player.MainFlagType.ClaimGift;
                        //client.Player.MainFlag |= Role.Player.MainFlagType.ShowSpecialItems;
                        //client.Player.SendUpdate(msg, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag);

                        //client.Player.SendUpdate(msg, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag);
                    }
                }
            }
           
        }
        [DataAttribute(ActionType.UpdateInventorySash)]
        public unsafe static void UpdateInventorySash(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }
            if (client.Player.ConquerPoints >= 90 && client.Player.InventorySashCount < 300)
            {
                if (client.InTrade) return;
                client.Player.InventorySashCount += 1;
                client.Player.ConquerPoints -= 90;
                client.Player.UpdateInventorySash(msg);
            }
            else
            {
#if Arabic
                 client.SendSysMesage("You need 90 CPs in your inventory to open 1 slot in your sash.");
#else
                client.SendSysMesage("You need 90 CPs in your inventory to open 1 slot in your sash.");
#endif

            }
        }
        [DataAttribute(ActionType.Away)]
        public unsafe static void Away(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (data.ObjId == client.Player.UID)
            {
                if (client.Player.Away == 1)
                    client.Player.Away = 0;
                else
                    client.Player.Away = 1;
            }
            else
                client.Player.Away = 0;

            client.Send(msg.ActionCreate(data));
            client.Player.View.SendView(client.Player.GetArray(msg, false), false);
        }
        [DataAttribute(ActionType.TradePartnerInfo)]
        public unsafe static void TradePartnerInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (Pool.GamePoll.TryGetValue(data.dwParam, out Target))
            {
                client.Send(msg.TradePartnerInfoCreate(Target));
            }
        }
        [DataAttribute(ActionType.SetAppearanceType)]
        public unsafe static void SetAppearanceType(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.Player.AparenceType = data.dwParam;
            data.ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ViewEnemyInfo)]
        public unsafe static void ViewEnemyInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (Pool.GamePoll.TryGetValue(data.dwParam, out Target))
            {
                client.Send(msg.CreateViewRelationInfo(Target, true));
                Phone.MobileOnly.SendFriendInfo(client, Target.Player, true);// QahrMobile: the mobile shows CMsgFriendInfo 10185, not MsgRelationInfo 2079
            }
        }
        //[DataAttribute(ActionType.FinishSteedRace)]
        //public unsafe static void FinishSteedRace(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        //{
        //    if (client.Player.Map == MsgTournaments.MsgSteedRace.MAPID)
        //        MsgTournaments.MsgSchedules.SteedRace.FinishRace(client);
        //}
        [DataAttribute(ActionType.ViewFriendInfo)]
        public unsafe static void ViewFriendInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (Pool.GamePoll.TryGetValue(data.dwParam, out Target))
            {
                client.Send(msg.CreateViewRelationInfo(Target, false));
                Phone.MobileOnly.SendFriendInfo(client, Target.Player, false);// QahrMobile: the mobile shows CMsgFriendInfo 10185, not MsgRelationInfo 2079
            }
        }
        [DataAttribute(ActionType.Ghost)]
        public unsafe static void Ghost(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.Alive == false)
            {
                client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "ghost");
                data.ObjId = client.Player.UID;
                data.PositionX = client.Player.X;
                data.PositionY = client.Player.Y;
                client.Player.View.SendView(msg.ActionCreate(data), true);
            }
        }
        [DataAttribute(ActionType.StartVendor)]
        public unsafe static void StartVendor(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (!client.IsVendor)
            {
                client.MyVendor = new Role.Instance.Vendor(client);
                client.MyVendor.CreateVendor(msg);
                data.dwParam = client.MyVendor.VendorUID;
                data.PositionX = client.MyVendor.VendorNpc.X;
                data.PositionY = client.MyVendor.VendorNpc.Y;
                client.Send(msg.ActionCreate(data));
            }
        }
        [DataAttribute(ActionType.Revive)]
        public unsafe static void Revive(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            bool CTFRandomRevive = data.dwParam == 2;
            if (CTFRandomRevive)
            {
                if (client.Player.Map == 3935 && client.OnInterServer)
                {
                    foreach (var server in Database.GroupServerList.GroupServers.Values)
                    {
                        if (server.ID == client.Player.ServerID)
                        {
                            client.Teleport((ushort)server.X, (ushort)server.Y, (ushort)server.MapID);
                            break;
                        }
                    }
                    return;
                }
                return;
            }

            if (client.InQualifier())
            {
                return;
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                return;
            byte deathtime = 20;
            byte itemLevel = 0;
            if (client.Rune.IsEquipped("Nirvana", ref itemLevel))
            {
                switch (itemLevel)
                {
                    case 1: deathtime -= 5; break;
                    case 2: deathtime -= 6; break;
                    case 3: deathtime -= 7; break;
                    case 4: deathtime -= 8; break;
                    case 5: deathtime -= 9; break;
                    case 6: deathtime -= 10; break;
                    case 7: deathtime -= 11; break;
                    case 8: deathtime -= 12; break;
                    case 9: deathtime -= 15; break;
                }

            }

            if (DateTime.Now > client.Player.DeadStamp.AddSeconds(deathtime))
            {
                if (!client.Player.Alive)
                {
                    if (client.Player.Map == 10364 || client.Player.Map == 13249 || client.Player.Map == 12602 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 10741 || client.Player.Map == 12546 || client.Player.Map == 3071 || client.Player.Map == 10741 || client.Player.Map == 10133 || client.Player.Map == 12602 || client.Player.Map == 12546 || client.Player.Map == 12799 || client.Player.Map == 12816 || client.Player.Map == 13140 || client.Player.Map == 12800 || client.Player.Map == 66666 || client.Player.Map == 33333 || client.Player.Map == 22222 || client.Player.Map == 88888)
                    {
                        client.Player.Revive(msg);
                        client.Teleport(410, 354, 1002);
                        return;
                    }
                    bool ReviveHere = data.dwParam == 1;
                    if (ReviveHere && client.Player.HeavenBlessing > 0)
                    {
                        if (client.OnInterServer)
                        {
                            if (client.InQualifier())
                            {
                                return;
                            }
                            if (client.Player.Map == 3935)
                            {
                                foreach (var server in Database.GroupServerList.GroupServers.Values)
                                {
                                    if (server.ID == client.Player.ServerID)
                                    {
                                        client.Teleport((ushort)(server.X + +Pool.GetRandom.Next(-4, 5)), (ushort)(server.Y + Pool.GetRandom.Next(-4, 5)), (ushort)server.MapID);
                                        break;
                                    }
                                }
                            }
                        }
                        // [Audit M6] "Revive here" must be denied on every tournament / PK /
                        // event map, not just a handful. BlockAttackMap already lists these
                        // (incl. dynamic instances via DynamicID), so reuse it.
                        if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID
                            || client.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID
                            || client.Player.Map == MsgTournaments.MsgCaptureTheFlag.MapID
                            || client.Player.Map == 6011
                            || (client.Player.DynamicID == 0 && Pool.Constants.BlockAttackMap.Contains(client.Player.Map))
                            || (client.Player.DynamicID != 0 && Pool.Constants.BlockAttackMap.Contains(client.Player.DynamicID)))
                            return;
                        client.Player.Revive(msg);
                        return;
                    }
                    else
                    {
                        if (client.Player.Map == 9855 || client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009)
                        {
                            client.Teleport(85, 75, 4020);
                            return;
                        }
                        if (client.Player.Map == 8009)
                        {
                            client.Teleport(84, 137, 8009);
                            return;
                        }
                        if (client.Player.Map == 3998)
                        {
                            client.Teleport(106, 383, 3998);
                            return;
                        }
                        if (client.OnInterServer)
                        {
                            if (client.Player.Map == 10137)
                            {
                                client.Teleport(95, 409, 10137);
                                return;
                            }
                        }
                        if (client.Player.Map == 3935 && client.OnInterServer)
                        {
                            foreach (var server in Database.GroupServerList.GroupServers.Values)
                            {
                                if (server.ID == client.Player.ServerID)
                                {
                                    client.Teleport((ushort)(server.X + +Pool.GetRandom.Next(-4, 5)), (ushort)(server.Y + Pool.GetRandom.Next(-4, 5)), (ushort)server.MapID);
                                    break;
                                }
                            }
                            return;
                        }
                        if (client.Player.OnMyOwnServer == false)
                        {
                            client.Teleport((ushort)410, 354, 1002);
                            return;
                        }
                       
   
                     

                        if (client.Player.Map == 1762 || client.Player.Map == 1927 || client.Player.Map == 1999 || client.Player.Map == 2054 || client.Player.Map == 2055)
                        {
                            client.Teleport(477, 640, 1000);
                            return;
                        }
                        if (client.Player.Map == 10250)
                        {
                            client.Teleport(1014, 1288, 10250);
                            return;
                        }
                        if (client.Player.Map == 6011)
                        {
                            client.Teleport(410, 354, 1002);
                            return;
                        }
                        if (client.Player.Map == 1038)// gw map
                        {
                            client.Teleport(410, 354, 1002, 0, true, true);
                        }
                        else if (client.Player.Map == 3868)//super gw
                        {
                            if (MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(410, 354, 1002);
                            else
                            {
                                var map = Pool.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else
                        {
                            if (client.Map.Reborn_Map != client.Player.Map)
                            {
                                if (client.Map.Reborn_Map == 0)
                                {
                                    client.Teleport(410, 354, 1002);
                                    return;
                                }
                                var map = Pool.ServerMaps[client.Map.Reborn_Map];
                                if (map == null)// reborn map not loadable (not in GameMap.dat)
                                {
                                    client.Teleport(410, 354, 1002);
                                    return;
                                }
                                client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                            }
                            else
                            {
                                if (client.Map.Reborn_X != 0)
                                    client.Teleport(client.Map.Reborn_X, client.Map.Reborn_Y, client.Map.Reborn_Map);
                                else
                                {
                                    var map = Pool.ServerMaps[client.Map.Reborn_Map];
                                    if (map.Reborn_X != 0)
                                        client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                    else
                                    {
                                        map = Pool.ServerMaps[map.Reborn_Map];
                                        if (map.Reborn_X != 0)
                                            client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                        else
                                            client.Teleport(410, 354, 1002);
                                    }
                                }
                            }
                        }
                        if (client.Player.X == 0 || client.Player.Y == 0)//invalid map reborn
                            client.Teleport(410, 354, 1002);
                    }
                }
            }
        }
            
        [DataAttribute(ActionType.EndTransformation)]
        public unsafe static void EndTransformation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.OnTransform)
            {
                if (client.Player.TransformInfo != null)
                {
                    client.Player.TransformInfo.Stamp = DateTime.Now;
                }
            }
        }
        [DataAttribute(ActionType.UpdateSpell)]
        public unsafe static void UpdateSpell(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }

            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }
            MsgSpell Spell;
            if (client.MySpells.ClientSpells.TryGetValue((ushort)data.dwParam, out Spell))
            {
                Dictionary<ushort, Database.MagicType.Magic> DbSpells;
                if (Pool.Magic.TryGetValue(Spell.ID, out DbSpells))
                {
                    if (Spell.Level < DbSpells.Count - 1)
                    {
                        if (DbSpells.ContainsKey(Spell.Level))
                        {
                            decimal cpCost = DbSpells[Spell.Level].CpsCost;
                            cpCost = ((cpCost / 2) - (cpCost / 2 / 10)) / 10;

                            int max = Math.Max((int)Spell.Experience, 1);
                            int percentage = (int)Math.Ceiling((decimal)(100 - (int)(max / Math.Max((DbSpells[Spell.Level].Experience / 100), 1))));
                            cpCost = Math.Ceiling((decimal)(cpCost * percentage / 100));
                            if (cpCost < 0) cpCost = 0;
                            else if (cpCost > int.MaxValue) cpCost = int.MaxValue;
                            if (client.Player.BoundConquerPoints >= cpCost)
                            {
                                if (client.InTrade) return;
                                client.Player.BoundConquerPoints -= (int)cpCost;

                                client.MySpells.Add(msg, Spell.ID, (ushort)(Spell.Level + 1), Spell.SoulLevel, Spell.PreviousLevel, Spell.Experience);
                            }
                            else
                            {
                                if (client.Player.ConquerPoints >= cpCost)
                                {
                                    if (client.InTrade) return;
                                    client.Player.ConquerPoints -= (uint)cpCost;

                                    client.MySpells.Add(msg, Spell.ID, (ushort)(Spell.Level + 1), Spell.SoulLevel, Spell.PreviousLevel, Spell.Experience);
                                }
                            }
                          
                            
                        }
                    }
                }
            }
        }
        [DataAttribute(ActionType.UpdateProf)]
        public unsafe static void UpdateProf(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }

            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }
            MsgProficiency prof;
            if (client.MyProfs.ClientProf.TryGetValue(data.dwParam, out prof))
            {
                if (prof.Level < 20)
                {
                    uint cpCost = 0;
                    #region Costs
                    switch (prof.Level)
                    {
                        case 1: cpCost = 100; break;
                        case 2: cpCost = 120; break;
                        case 3: cpCost = 140; break;
                        case 4: cpCost = 160; break;
                        case 5: cpCost = 180; break;
                        case 6: cpCost = 200; break;
                        case 7: cpCost = 220; break;
                        case 8: cpCost = 240; break;
                        case 9: cpCost = 260; break;
                        case 10: cpCost = 280; break;
                        case 11: cpCost = 300; break;
                        case 12: cpCost = 320; break;
                        case 13: cpCost = 340; break;
                        case 14: cpCost = 360; break;
                        case 15: cpCost = 380; break;
                        case 16: cpCost = 400; break;
                        case 17: cpCost = 800; break;
                        case 18: cpCost = 1000; break;
                        case 19: cpCost = 2000; break;
                    }
                    #endregion
                    uint needExperience = BaseFunc.ProficiencyLevelExperience((byte)prof.Level);
                    int max = Math.Max((int)prof.Experience, 1);
                    int percentage = 100 - (int)(max / (needExperience / 100));
                    cpCost = (uint)(cpCost * percentage / 100);
                    if (client.Player.ConquerPoints >= cpCost)
                    {
                        client.Player.ConquerPoints -= (uint)cpCost;

                        client.MyProfs.Add(msg, prof.ID, prof.Level + 1, prof.Experience, prof.PreviouseLevel);
                        client.SendSysMesage("You have paid " + cpCost + " Conquer Points to increase the weapon proficiency!", MsgMessage.ChatMode.TopLeftSystem, MsgMessage.MsgColor.red);
                    }
                }
            }
        }
        [DataAttribute(ActionType.StopVending)]
        public unsafe static void StopVending(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.IsVendorr = false;
            if (client.IsVendor)
                client.MyVendor.StopVending(msg);
        }
        [DataAttribute(ActionType.EndFly)]
        public unsafe static void EndFly(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.Player.RemoveFlag(MsgUpdate.Flags.Fly);
            client.Player.RemoveFlag(MsgUpdate.Flags.Infinity);
            client.Player.RemoveFlag(MsgUpdate.Flags.HeavensWrath);
            client.Player.RemoveFlag(MsgUpdate.Flags.HoverFather);
  
        }
        [DataAttribute(ActionType.TeamSearchForMember)]
        public unsafe static void TeamSearchForMember(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Team != null)
            {
                Client.GameClient Member;
                if (client.Team.TryGetMember(data.dwParam, out Member))
                {
                    if (Member.Player.Map == client.Player.Map)
                    {
                        data.PositionX = Member.Player.X;
                        data.PositionY = Member.Player.Y;
                        client.Send(msg.ActionCreate(data));
                    }
                }
            }
        }
        [DataAttribute(ActionType.SetLocation)]
        public unsafe static void SetLocation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation)
            {
#if !Roullete
                if (client.Player.Map == 3852 || client.Player.Map == 3845)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
#endif
                if (client.Player.Map == 3053)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6379) // Map Quest Archive Trojan [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6380) // Map Quest Archive Trojan [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6381) // Map Quest Archive Trojan [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10764) // Map Quest Archive Ninja [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11041) // Map Quest Archive Warrior [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11039) // Map Quest Archive Archer [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11040) // Map Quest Archive Archer [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 9999) // Map Quest Archive Archer [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11222) // Map Knights War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 2071) // Map Flags War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11567) // Map Quest Archive Taoist [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11568) // Map Quest Archive Taoist [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10266) // Map Quest Archive Pirate [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11780) // Map Quest Archive Pirate [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 7777) // Map Quest Archive Pirate [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 9997) // Map Quest Archive Pirate [4]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3983) // Map Quest Epic Monk [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3933) // Map Quest Epic Taoist [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 7733) // Map Quest Archive Dragon Warrior [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 7734) // Map Quest Archive Dragon Warrior [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11140) // Map Quest Archive Dragon Warrior [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3849) // Map Quest Epic Ninja [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3850) // Map Quest Epic Ninja [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3851) // Map Quest Epic Ninja [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11257) // Map Quest Epic Archer [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10271) // Map Quest Epic Pirate [1]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10267) // Map Quest Epic Pirate [2]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10268) // Map Quest Epic Pirate [3]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10269) // Map Quest Epic Pirate [4]
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 2057) // Map Capture The Flag
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8832) // Fiend Lair Permit (F11)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8833) // Fiend Lair Permit (F12)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8834) // Fiend Lair Permit (F13)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8835) // Fiend Lair Permit (F14)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8836) // Fiend Lair Permit (F15)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8837) // Fiend Lair Permit (F16)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8838) // Fiend Lair Permit (F17)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8839) // Fiend Lair Permit (F18)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 1770) // Class PK War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 1508) // Weekly PK War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                //////////
                if (client.Player.Map == 700) // Last~Man~Stand - Killer~Of~Elite
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 2071) // Elite~Guild~War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6729) // War~Of~Players
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6546) // Champions~Of~War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 9931) // Treasure~Thief
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 1505) // Skill~Tournament
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6333) // Flags War
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10741) // Gold Box
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3030) // Bunny Chase
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10133) // CP Castle
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3071) // Octopus
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 9955) // Sunrise Event
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 1036) // Market
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3071) // Octopos Events
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 4000) // Last Man New Map
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 9911) // Map King 30 Days
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                ///////////////////////////
                // All Events //
                if (client.Player.Map == 11)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 7)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 5)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 15)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 14)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 8)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 20)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 1730)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10134)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12099)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10133)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 22348)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12344)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10070)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10504)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12344)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 6521)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 95152)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                /////////////////////////////
                if (client.Player.Map == 12667)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12275)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12602)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10741)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 2071)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 3030)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10133)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                #region All New Wars Edit By Mohy ElDin ...
                if (client.Player.Map == 12546)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 13249)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12602)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 13140)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12546)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12800)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11331)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12799)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 11996)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12665)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12601)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12816)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12099)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                #endregion
                if (client.Player.Map == 1036)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                if (client.Player.Map == 13249)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12546)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10741)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10133)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12602)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12546)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12799)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12816)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 13140)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 12800)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 10364)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                if (client.Player.Map == 66666)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 33333)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 22222)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                if (client.Player.Map == 88888)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                if (client.Player.Map == 12602)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                if (client.Player.Map == 12275)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }

                if (!Role.GameMap.CheckMap(client.Player.Map))
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
            back:
                Role.GameMap.EnterMap((int)client.Player.Map);
                if (Pool.ServerMaps.TryGetValue(client.Player.Map, out client.Map))
                {

                    client.ClientFlag |= Client.ServerFlag.SetLocation;
                    client.Map.Enquer(client);

                    // [User Agreement] On a character's first ever login, show the EULA.
                    // Must click "I Agree" to continue; "I Disagree" disconnects. Once
                    // accepted it is stored by UserAgreementDB and never shown again.
                    if (!Qahr.Mobile.Game.MsgNpc.UserAgreementDB.HasAgreed(client.Player.UID))
                    {
                        client.ActiveNpc = (uint)Qahr.Mobile.Game.MsgNpc.NpcID.UserAgreement;
                        Qahr.Mobile.Game.MsgNpc.UserAgreementDB.MarkPending(client.Player.UID); // [AgreementFix 2026-10-15]
                        using (var agRec = new ServerSockets.RecycledPacket())
                            Qahr.Mobile.Game.MsgNpc.NpcHandler.UserAgreementDialog(
                                client, agRec.GetStream(), 0, "", (uint)Qahr.Mobile.Game.MsgNpc.NpcID.UserAgreement);
                    }

                    if (client.Player.HitPoints == 0)
                    {
                        client.Player.HitPoints = 1;

                        if (client.Player.Map == 1038)// gw map
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(410, 354, 1002);
                            else
                            {
                                var map = Pool.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else if (client.Player.Map == 1138)//super gw
                        {
                            if (MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(410, 354, 1002);
                            else
                            {
                                var map = Pool.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else
                        {
                            if (client.Player.Map == 601)
                                client.Teleport(410, 354, 1002);
                            else
                            {
                                if (client.Map.Reborn_X != 0)
                                    client.Teleport(client.Map.Reborn_X, client.Map.Reborn_Y, client.Map.Reborn_Map);
                                else
                                {
                                    Role.GameMap map;// ;= Pool.ServerMaps[client.Map.Reborn_Map];
                                    Role.GameMap.EnterMap(client.Map.Reborn_Map);
                                    if (Pool.ServerMaps.TryGetValue(client.Map.Reborn_Map, out map))
                                    {
                                        if (map.Reborn_X != 0)
                                            client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                        else
                                        {
                                            map = Pool.ServerMaps[map.Reborn_Map];
                                            if (map.Reborn_X != 0)
                                                client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                            else
                                                client.Teleport(410, 354, 1002);
                                        }
                                    }
                                    else
                                    {

                                        client.Teleport(410, 354, 1002);
                                    }
                                }
                                if (client.Player.X == 0 || client.Player.Y == 0)//invalid map reborn
                                    client.Teleport(410, 354, 1002);
                            }
                        }
                    }

                    data.ObjId = client.Map.ID;
                    if (client.Map.BaseID != 0)
                        data.dwParam = client.Map.BaseID;
                    else
                        data.dwParam = client.Player.Map;
                    data.PositionX = client.Player.X;
                    data.PositionY = client.Player.Y;
                    data.Fascing = (ushort)client.Player.Angle;
                    client.Send(msg.ActionCreate(data));




                    client.Player.ClearPreviouseCoord();
                    client.Player.View.Role();



                }
                else
                {
                    client.Player.Map = 1002;
                    client.Player.X = 410;
                    client.Player.Y = 354;
                    Pool.ServerMaps[1002].Enquer(client);
                    goto back;
                }

                client.Send(msg.MapStatusCreate(client.Map.ID, client.Map.ID, client.Map.TypeStatus));
                Phone.MobileProtocol.SendFunctionFlags(client, msg);// QahrMobile: menu buttons (guild, lineage, ...)

                //client.Send(msg.WeatherCreate(MsgWeather.WeatherType.Snow, 1000, 3, 0, 0));
                if (client.Player.Map == 3846)
                {

                    ActionQuery datam = new ActionQuery()
                    {
                        ObjId = client.Player.UID,
                        Type = ActionType.SetMapColor,
                        dwParam = 16755370,
                        PositionX = client.Player.X,
                        PositionY = client.Player.Y
                    };

                    client.Send(msg.ActionCreate(datam));
                }
            }


        }
        [DataAttribute(ActionType.ChangeMap)]
        public unsafe static void ChangeMap(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
           
            ushort Portal_X = (ushort)data.PositionX;
            ushort Portal_Y = (ushort)data.PositionY;
            foreach (var portal in client.Map.Portals)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, portal.X, portal.Y) < 7)
                {

                    client.Teleport(portal.Destiantion_X, portal.Destiantion_Y, portal.Destiantion_MapID);
                    client.Send(msg.ActionCreate(data));
                    return;
                }
            }
            if (client.ProjectManager)
            {
                client.SendSysMesage("Invalid Portal : X = " + Portal_X + ", Y= " + Portal_Y + " Map = " + client.Player.Map + "", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            }
            client.Teleport(410, 354, 1002);
        }
        [DataAttribute(ActionType.ChangeLookface)]
        public unsafe static void ChangeLookface(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.InTrade)
                return;

            if (client.IsVendor)
                return;
            
            if (client.Player.Money >= 500)
            {
                uint newface = data.dwParam;
                if (client.Player.Body > 2000)
                {
                    newface = newface < 200 ? newface + 200 : newface;
                    client.Player.Face = (ushort)newface;
                }
                else
                {
                    newface = newface > 200 ? newface - 200 : newface;
                    client.Player.Face = (ushort)newface;
                }
                client.Player.Money -= 500;
                client.Send(msg.ActionCreate(data));
                client.Player.SendUpdate(msg, client.Player.Money, MsgUpdate.DataType.Money);
            }
            else
            {
#if Arabic
                client.SendSysMesage("You do not have 500 silvers with you.", MsgMessage.ChatMode.TopLeft);
#else
                client.SendSysMesage("You do not have 500 silvers with you.", MsgMessage.ChatMode.TopLeft);
#endif

            }
        }
        [DataAttribute(ActionType.RequestEntity)]
        public unsafe static void RequestEntity(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Role.IMapObj obj;
            if (client.Player.View.TryGetValue(data.dwParam, out obj, Role.MapObjectType.Player))
            {
                var pClient = (obj as Role.Player).Owner;
                if (pClient.IsWatching() || pClient.Player.Invisible)
                    return;
                client.Send(obj.GetArray(msg, false));
            }
            else if (client.Player.View.TryGetValue(data.dwParam, out obj, Role.MapObjectType.Monster))
            {
                client.Send(obj.GetArray(msg, false));
            }
        }

        [DataAttribute(ActionType.QuerySpawn)]
        public unsafe static void QuerySpawn(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (Pool.GamePoll.TryGetValue(data.dwParam, out Target))
            {

                client.Send(Target.Player.GetArray(msg, true));
            }

        }
        [DataAttribute(ActionType.ViewEquipment)]
        public unsafe static void ViewEquipment(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (!Pool.GamePoll.TryGetValue(data.dwParam, out Target))
                return;
            if (Target.Equipment == null || Target.Equipment.CurentEquip == null)
                return;

            // كل قسم مفصول لوحده: قبل كده أي استثناء في أي قسم كان بيوقف الباقي كله
            // فالنافذة ماكانتش بتفتح خالص من غير ما حد يعرف السبب.
            if (Target.PrestigePoints != null)
            {
                MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = Target.Player.Level;
                info.UID = Target.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[Target.PrestigePoints.Length];
                for (int x = 0; x < Target.PrestigePoints.Length; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = Target.PrestigePoints[x];
                }
                client.Send(msg.UserAbilityScoreCreate(info));
            }

            foreach (var item in Target.Equipment.CurentEquip)
            {
                if (item != null)
                {
                    client.Send(msg.ItemViewCreate(Target.Player.UID, 0, item, MsgItemView.ActionMode.ViewEquip));
                    try { item.SendItemExtra(client, msg); }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
            }

            try
            {
                if (Target.EonspiritSystem != null)
                    foreach (var Yuanshen in Target.EonspiritSystem.Values)
                    {
                        if (Yuanshen != null && (Yuanshen.Position == 222 || Yuanshen.Position == 223))
                            client.Send(msg.ItemViewCreate(Target.Player.UID, 0, Yuanshen, MsgItemView.ActionMode.Yuanshen));
                    }
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            try
            {
                if (Target.MinorRilec != null)
                    foreach (var relic in Target.MinorRilec.Values)
                    {
                        if (relic != null)
                            client.Send(msg.ItemViewCreate(Target.Player.UID, 0, relic, MsgItemView.ActionMode.RelicFuseNew));
                    }
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            try
            {
                if (Target.Rune != null && Target.Rune.Objects != null)
                    foreach (var rune in Target.Rune.Objects)
                    {
                        if (rune != null)
                            client.Send(msg.ItemViewCreate(Target.Player.UID, 0, rune, MsgItemView.ActionMode.Rune));
                    }
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            try
            {
                Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                packet.ID = MsgStringPacket.StringID.ViewEquipSpouse;
                packet.UID = client.Player.UID;
                packet.Strings = new string[1] { Target.Player.Spouse ?? "None" };
                client.Send(msg.StringPacketCreate(packet));
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            try
            {
                if (Target.Status != null)
                    client.Send(msg.StatusCreate(Target.Status));
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            Target.SendSysMesage(client.Player.Name + " is checking your equipment.", MsgMessage.ChatMode.System);
        }
        [DataAttribute(ActionType.Hotkeys)]
        public unsafe static void Hotkeys(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.Send(msg.ActionCreate(data));

        }
        
        public static double CalculateJumpStamp(short distance)
        {
            return (int)(distance * 220);
        }
        [DataAttribute(ActionType.Jump)]
        public unsafe static void PlayerJump(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.IsVendorr)
            {
                client.Pullback();
                client.IsVendorr = false;
                return;
            }
            if (data.ObjId != client.Player.UID)
                return;


            if (client.Player.ReviveProtection)
            {
                client.Player.ReviveProtection = false;
                client.Player.ProtectAttack(0);
            }


            MsgLoader.SpeedProtect.FallSpeedProtect(client);
            if (client.Player.ContainFlag((MsgUpdate.Flags)491))
                client.Player.RemoveFlag((MsgUpdate.Flags)491);
            if (client.Player.Away == 1)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                    client.Player.KingDomExploits = client.Player.KingDomExploits;
                }
            }
            MsgLoader.SpeedProtect.JumpProdect(client);
            if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ManiacDance)
                || client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ShurikenVortex)
                || client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DefensiveStance)
                || (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy)
                && client.Player.StunJump.AddMilliseconds(900) > DateTime.Now))
            {
                var Stun = new ActionQuery();
                Stun.ObjId = client.Player.UID;
                Stun.PositionX = client.Player.X;
                Stun.PositionY = client.Player.Y;
                Stun.Type = 108;
                client.Send(msg.ActionCreate(Stun));
                return;
            }

            client.OnAutoAttack = false;

            client.Player.RemoveBuffersMovements(msg);
            if (client.Player.InUseIntensify)
            {
                client.Player.InUseIntensify = false;
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    client.Send(streamm.ActionCreate(new ActionQuery() { ObjId = client.Player.UID, TargetUID = client.Player.FocusClientSpell.ID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), Type = 103 }));
                }
            }
            if (!client.Player.Alive && !client.Player.ContainFlag(MsgUpdate.Flags.NeptuneCurse))
            {
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, client.Player.Dead_X, client.Player.Dead_Y);
                if (client.Player.Dead_X == 0 || client.Player.Dead_Y == 0)
                {
                    client.Player.X = 410;
                    client.Player.Y = 354;
                }
                else
                {
                    client.Player.X = client.Player.Dead_X;
                    client.Player.Y = client.Player.Dead_Y;
                }

                InteractQuery action = new InteractQuery()
                {
                    X = client.Player.Dead_X,
                    AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                    Y = client.Player.Dead_Y,
                    OpponentUID = client.Player.UID
                };
                client.Player.View.SendView(msg.InteractionCreate(action), true);

                return;
            }

            ushort JumpX = (ushort)data.TargetPositionX;
            ushort JumpY = (ushort)data.TargetPositionY;



            if (client.Map == null)
            {
                client.Teleport(410, 354, 1002);
                return;

            }

            // [Audit H1] Server-side jump validation on the TARGET tile, measured from the
            // server-known position (never a client-sent position). Blocks teleport-jumps
            // through walls / into locked areas and caps the jump distance + rate.
            {
                short jumpDistance = Role.Core.GetDistance(client.Player.X, client.Player.Y, JumpX, JumpY);

                // max jump distance (official ~18) with a small margin for buffs/latency
                if (jumpDistance > 20)
                {
                    client.Pullback();
                    return;
                }

                // target tile must be walkable and inside the map bounds
                // (guarded with bounds>0 so a cell-less map never blocks all jumps)
                if (client.Map.bounds.Width > 0 && !client.Map.ValidLocation(JumpX, JumpY))
                {
                    client.Pullback();
                    return;
                }

                // altitude / wall check on EVERY map (not only 1002)
                if (client.Map.Altitude != null
                    && client.Player.X < client.Map.bounds.Width && client.Player.Y < client.Map.bounds.Height)
                {
                    if (client.Map.Altitude[JumpX, JumpY] - client.Map.Altitude[client.Player.X, client.Player.Y] > 200)
                    {
                        client.SendSysMesage("Illegal jumping detected.");
                        client.Pullback();
                        return;
                    }
                }

                // [FIX] Removed the lastJumpTime "rate backstop" here. lastJumpTime holds the
                // previous jump's *landing* time, computed as distance*220ms (CalculateJumpStamp),
                // which for a long jump sits 3-4 seconds in the future. The old check pulled the
                // player back whenever a new jump arrived while that window hadn't elapsed, so
                // normal chained/long jumps (common in Twin City) were silently rejected
                // ("jump stops as if hitting a wall, then continues"). Jump-rate anti-cheat is
                // already handled by SpeedProtect.JumpProdect above; teleport-jumps are still
                // blocked by the distance/altitude/ValidLocation checks. lastJumpTime is still
                // set below (line ~2700) because combat (MsgAttackPacket) uses it to know when a
                // target is airborne.
            }

            if (client.Player.Map == 1038)
            {
                if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidJump(client.TerainMask, out client.TerainMask, JumpX, JumpY))
                {

                    client.SendSysMesage("Illegal jumping over the gates detected.");

                    client.Pullback();
                    return;
                }

            }
            if (client.Player.Map == 1002)
            {
                if (JumpX >= client.Map.bounds.Width || JumpY >= client.Map.bounds.Height || client.Player.X >= client.Map.bounds.Width || client.Player.Y >= client.Map.bounds.Height)
                {
                    client.Pullback();
                    return;//done
                }

                if (client.Map.Altitude[JumpX, JumpY] - client.Map.Altitude[client.Player.X, client.Player.Y] > 200)// If the target tallness is greater than current tallness by 200 point
                {
                    MyConsole.WriteLine("[WARNING] " + client.Player.Name + " has been jumped to a high place!");
                    MyConsole.WriteLine("Map: " + client.Player.Map + ", X:" + JumpX + ", Y:" + JumpY);
                    client.SendSysMesage("Illegal jumping detected.");
                    client.Pullback();

                    return;
                }
            }

            #region ObjInteraction
            if (client.Player.ObjInteraction != null)
            {

                InterActionWalk inter = new InterActionWalk()
                {
                    Mode = MsgInterAction.Action.Jump,
                    X = JumpX,
                    Y = JumpY,
                    UID = client.Player.UID,
                    OponentUID = client.Player.ObjInteraction.Player.UID
                };
                client.Player.View.SendView(msg.InterActionWalk(&inter), true);


                client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
                client.Player.Action = Role.Flags.ConquerAction.InteractionHold;
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, JumpX, JumpY);
                client.Player.X = JumpX;
                client.Player.Y = JumpY;
                client.Player.CanUseSectorSkill = true;
                client.Player.View.Role(false, null);

                client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, JumpX, JumpY);
                client.Player.ObjInteraction.Player.X = JumpX;
                client.Player.ObjInteraction.Player.Y = JumpY;
                client.Player.ObjInteraction.Player.Action = Role.Flags.ConquerAction.Jump;
                client.Player.ObjInteraction.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
                client.Player.ObjInteraction.Player.View.Role(false, null);

                return;
            }
            #endregion
            short Distance = Role.Core.GetDistance(client.Player.X, client.Player.Y, JumpX, JumpY);
            client.Player.lastJumpTime = DateTime.Now.AddMilliseconds(CalculateJumpStamp(Distance));

            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
            {
                uint vigor = (ushort)(1.5F * (Distance / 2));
                if (client.Vigor >= vigor)
                    client.Vigor -= vigor;
                else
                    client.Vigor = 0;


                client.Send(msg.ServerInfoCreate(client.Vigor));

            }
            data.dwParam3 = client.Player.Map;
            client.Player.View.SendView(msg.ActionCreate(data), true);
            client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
            client.Player.CanUseSectorSkill = true;
            client.Player.Action = Role.Flags.ConquerAction.Jump;
            client.Map.View.MoveTo<Role.IMapObj>(client.Player, JumpX, JumpY);
            client.Player.X = JumpX;
            client.Player.Y = JumpY;
            client.Player.View.Role(false, null);
            if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);
            //if (MsgTournaments.MsgSchedules.GuildWar != null)
            //    MsgTournaments.MsgSchedules.GuildWar.ChechMove(client);


            if (client.Player.Map == 10445 || client.Player.Map == 10446 || client.Player.Map == 10447 || client.Player.Map == 10448 || client.Player.Map == 10449 || client.Player.Map == 10450 || client.Player.Map == 10451 || client.Player.Map == 10452 || client.Player.Map == 10453 || client.Player.Map == 10454 || client.Player.Map == 10455)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 66, 90) <= 4)
                {
                    client.Teleport(58, 65, 1004);
                }
            }
            if (client.Player.Map == 1013)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 58, 22) <= 2)
                {
                    client.Teleport(81, 40, 1014);
                }

            }

            if (client.Player.Map == 1016)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 52, 86) <= 2)
                {
                    client.Teleport(134, 86, 1014);
                }

            }
            if (client.Player.Map == 10622)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 43, 31) <= 4)
                {
                    client.Player.MessageBox("Are you sure you want return to Twin City?", p => { p.Teleport(410, 354, 1002); p.SendSysMesage(" you were teleported back to Twin City."); });
                }
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
            #region ShadowClones
            if (client.Player.MyClones.Count > 0)
            {
                foreach (var clone in client.Player.MyClones.GetValues())
                {
                    clone.Owner.Player.X = client.Player.X;
                    clone.Owner.Player.Y = client.Player.Y;
                    clone.Owner.Player.Angle = client.Player.Angle;
                }
            }
            #endregion
            if (client.Player.Map == 10428 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 61, 66) <= 2)
            {
                client.Player.MessageBox("Are you sure you want to leave the alien world and return to Twin City?", p => { p.Teleport(410, 354, 1002); p.SendSysMesage("With a flash of light, you were teleported back to Twin City."); });
            }
            if (client.Player.Map == 11041 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 87, 76) <= 2)
            {
                client.Player.MessageBox("Are you sure you want return to Twin City?", p => { p.Teleport(410, 354, 1002); p.SendSysMesage(" you were teleported back to Twin City."); });
            }
            if (client.Player.Map == 11223 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 179, 173) <= 2)
            {
                client.Player.MessageBox("Are you sure you want return to Twin City?", p => { p.Teleport(410, 354, 1002); p.SendSysMesage(" you were teleported back to Twin City."); });
            }
            if (client.SaveMele != null)
            {
                if (client.SaveMele.OnAutoAttack)
                {
                    if (client.SaveMele.AutoAttack != null && client.SaveMele.AutoAttack.OpponentUID == client.Player.UID)
                    {
                        client.onSaveMele = true;
                        client.SaveMeleStamp = DateTime.Now.AddMilliseconds(1);
                    }
                }
            }
            else
            {
                Client.GameClient Client;
                if (client.SaveMele != null)
                {
                    if (Pool.GamePoll.TryGetValue(client.SaveMele.Player.UID, out Client))
                    {
                        if (Client.SaveMele != null)
                        {
                            Client.SaveMele.nSaveMele = false;
                            Client.SaveMele = null;

                        }
                    }
                }
            }
        }

        [DataAttribute(ActionType.DeleteCharacter)]
        public unsafe static void DeleteCharacter(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.CreateBoxDialog("Sorry, The Delete Accounts System is Temporarily Closed.");
            return;
            client.Player.Delete = true;
            client.Socket.Disconnect();
        }
        [DataAttribute(ActionType.ChangeDirection)]
        public unsafe static void ChangeDirection(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.OnAutoAttack = false;
            client.Player.Angle = (Role.Flags.ConquerAngle)data.Fascing;
#if TEST
               Console.WriteLine("Direction . " + data.Fascing + " " + client.Player.Angle.ToString()+ "");
#endif
            client.Player.View.SendView(msg.ActionCreate(data), true);
            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
        }
        [DataAttribute(ActionType.ChangeStance)]
        public unsafe static void ChangeStance(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.OnAutoAttack = false;
            client.OnChangeActionTime = DateTime.Now;
            client.Player.Action = (Role.Flags.ConquerAction)data.dwParam;

            if (client.Player.Action == Role.Flags.ConquerAction.Cool)
            {

                if (client.Equipment.FullSuper)
                    data.dwParam = (uint)(data.dwParam | (uint)(client.Player.Class * 0x10000 + 0x1000000));
                else if (client.Equipment.SuperArmor)
                    data.dwParam = (uint)(data.dwParam | (uint)(client.Player.Class * 0x10000));
            }
            data.ObjId = client.Player.UID;
            data.dwParam = (ushort)client.Player.Action;
            if (client.Player.Action == Role.Flags.ConquerAction.Cool)
                client.Send(msg.CreateDragonSkinInfo(new MsgDragonSkin.MsgDragonSkinProto()
                {
                    Type = MsgDragonSkin.ActionID.Effect,
                    UID = client.Player.UID,
                    DragonSkinRecord = client.DragonSkin.GetStyle()
                }));
            client.Player.View.SendView(msg.ActionCreate(data), false);
            if (client.Player.Action == Role.Flags.ConquerAction.PistilAroma)
            {
                var UseIt = client.Collection.items.Values.Where(p => p.Use == 1).FirstOrDefault();
                if (UseIt != null)
                    client.Player.PistilAromaID = UseIt.ID;
                client.Player.View.SendView(msg.ActionCreate(data), false);
            }

            if (client.Player.ContainFlag(MsgUpdate.Flags.CastPray))
            {
                foreach (var user in client.Player.View.Roles(Role.MapObjectType.Player))
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, user.X, user.Y) <= 4)
                    {
                        data.Timestamp = user.UID;
                        client.Player.View.SendView(msg.ActionCreate(data), true);
                    }
                }
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
            if (DateTime.Now > client.Player.SpellAttackStamp.AddMilliseconds(client.Equipment.AttackSpeed(true)))
            {
                client.Player.SpellAttackStamp = DateTime.Now;
                client.Player.SpellAttackStamp.AddMilliseconds(client.Equipment.AttackSpeed(true));
            }
        }
        [DataAttribute(ActionType.SetPkMode)]
        public unsafe static void SetPkMode(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            // [Audit H6] Only accept defined PK modes, and allow Jiang mode ONLY when the
            // player actually has an active JiangHu. This stops arbitrary clients switching
            // to Jiang (which the attack code treats specially) to bypass safe-zone rules.
            Role.Flags.PKMode requested = (Role.Flags.PKMode)data.dwParam;
            switch (requested)
            {
                case Role.Flags.PKMode.PK:
                case Role.Flags.PKMode.Peace:
                case Role.Flags.PKMode.Team:
                case Role.Flags.PKMode.Capture:
                case Role.Flags.PKMode.Revange:
                case Role.Flags.PKMode.Guild:
                case Role.Flags.PKMode.CS:
                case Role.Flags.PKMode.Union:
                    break;
                case Role.Flags.PKMode.Jiang:
                    if (client.Player.MyJiangHu == null)
                        return;
                    break;
                default:
                    return;
            }

            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
                client.SendSysMesage("You have temporarily quit the Jiang Hu!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            client.Player.PkMode = requested;
            data.dwParam3 = data.dwParam;
            client.Send(msg.ActionCreate(data));
            if (client.Player.PkMode == Role.Flags.PKMode.PK)
                client.SendSysMesage("Free PK mode. You can attack monster and all players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Capture)
                client.SendSysMesage("Capture PK mode. You can only attack monsters, black-name and blue-name players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Peace)
                client.SendSysMesage("Peace mode. You can only attack monsters.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Team)
                client.SendSysMesage("Team PK mode. You can attack monster and all players except your teammates or guild.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Revange)
                client.SendSysMesage("Revenge PK mode. You can attack monster and all Your Enemy List Players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Guild)
                client.SendSysMesage("Guild PK mode. You can attack monster and all Your Guild's Enemies", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#if Jiang
            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
            {
                if (client.Player.MyJiangHu != null)
                {
                    client.Player.MyJiangHu.ActiveJiangMode(client);
                    client.SendSysMesage("You have returned to the Jiang Hu !", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                }
            }
#endif
        }
        [DataAttribute(ActionType.ConfirmAssociates)]
        public unsafe static void ConfirmAssociates(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmSpells)]
        public unsafe static void ConfirmSpells(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.MySpells.SendAll(msg);
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmProficiencies)]
        public unsafe static void ConfirmProficiencies(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.MyProfs.SendAll(msg);
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmGuild)]
        public unsafe static void ConfirmGuild(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
            {
                client.Player.SendString(msg, MsgStringPacket.StringID.GuildName, client.Player.MyGuild.Info.GuildID, true, client.Player.MyGuild.GuildName + " " + client.Player.MyGuild.Info.LeaderName + " " + client.Player.MyGuild.Info.Level.ToString() + " " + client.Player.MyGuild.Members.Count.ToString() + " 83");
                client.Player.MyGuild.SendThat(client.Player);
                client.Player.SendString(msg, MsgStringPacket.StringID.GuildName, client.Player.MyGuild.Info.GuildID, true, client.Player.MyGuild.GuildName + " " + client.Player.MyGuild.Info.LeaderName + " " + client.Player.MyGuild.Info.Level.ToString() + " " + client.Player.MyGuild.Members.Count.ToString() + " 83");
                client.Player.MyGuild.SendGuildAlly(msg, true, client);
                client.Player.MyGuild.SendGuilEnnemy(msg, true, client);
                client.Player.GuildBattlePower = client.Player.MyGuild.ShareMemberPotency(client.Player.MyGuildMember.Rank);
            }
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.AllowAnimation)]
        public unsafe static void AllowAnimation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            if (client.Player.InUnion)
            {
                client.Player.MyUnion.SendMyInfo(msg, client);
            }
         
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
            data.ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));
        }

        [DataAttribute(ActionType.CompleteLogin)]
        public unsafe static void CompleteLogin(Client.GameClient client, ServerSockets.Packet msg, ActionQuery data)
        {
            client.Player.CompleteLogin = true;
            if (client.OnInterServer)
            {
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                if (client.Player.InitTransfer == 1)
                {
                    client.Player.InitTransfer = 2;
                    Console.WriteLine("Complete Transfer for " + client.Player.Name);
                    client.Send(msg.InterServerCheckTransferCreate(3, client.Player.UID));
                    lock (Pool.NameUsed)
                    {
                        if (Pool.NameUsed.Contains(client.Player.Name.GetHashCode()))
                        {
                            if (!Pool.NameUsed.Contains(client.Player.UID.ToString().GetHashCode()))
                            {
                                Pool.NameUsed.Add(client.Player.UID.ToString().GetHashCode());
                            }
                            client.Player.Name = client.Player.UID.ToString();
                        }
                    }

                }
            }
            else
            {

                client.Player.UpdateVip(msg);
                if (client.Player.MyJiangHu != null)
                    client.Player.MyJiangHu.LoginClient(msg, client);
                else if (client.Player.Reborn == 2)
                {
                    client.Send(msg.JiangHuInfoCreate(MsgJiangHuInfo.JiangMode.IconBar, "0"));
                }
                client.MyWardrobe.SendToClient(msg);
             
                if (client.Player.GuildID == 0 && client.Player.Level > 20)
                {
                    client.SendSysMesage("Why not join a guild. and find some companions in this turbulent world!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                }
                client.CreatePrestigePoints();

                MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = client.Player.Level;
                info.UID = client.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[client.PrestigePoints.Length];
                for (int x = 0; x < client.PrestigePoints.Length; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = client.PrestigePoints[x];

                }
                client.Send(msg.UserAbilityScoreCreate(info));
            }
            client.Player.CompleteLogin = true;
        }
        [DataAttribute(ActionType.GiveOrder)]
        public unsafe static void GiveOrder(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            // QahrMobile: on the phone 466 is the Daily Packs query (CDailyGiftMgr::QueryGift), not a guild-war order.
            if (global::Qahr.Mobile.Phone.MobileDailyGift.Handle(user, msg, data))
                return;
            foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
            {
                if (client.Player.Map == 1038 || client.Player.Map == 6001)
                {
                    if (client.Player.GuildID == user.Player.GuildID && client.Player.UID != user.Player.UID)
                    {
                        client.Send(msg.ActionCreate(data));
                    }
                }
            }
        }
        [DataAttribute(ActionType.ExchangeClan)]
        public unsafe static void ExchangeClan(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (user.IsVendor)
            {
                user.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (user.InTrade)
            {
                user.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }
            uint Count = (uint)(data.dwParam3 / 130000);
            if (user.Player.Money >= data.dwParam3)
            {
                user.Player.Money -= data.dwParam3;
                user.Player.CyanJadeRing += Count;
                user.Send(msg.ActionCreate(data));
            }

        }
        [DataAttribute(ActionType.ShowFriend)]
        public unsafe static void ShowFriend(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (user.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Friends))
            {
                var Info = new MsgFriendInfo.FriendProtoStructure() { Type = 0, unk1 = 1, Items = new List<MsgFriendInfo.FriendProtoStructure.ConstructsProto>() };
                foreach (var typ in user.Player.Associate.Associat)
                {
                    foreach (var mem in typ.Value.Values)
                    {
                        if (typ.Key == Role.Instance.Associate.Friends)
                        {
                            Client.GameClient Targer;
                            if (Pool.GamePoll.TryGetValue(mem.UID, out Targer))
                                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto() { UID = Targer.Player.UID, Online = 1, Mesh = Targer.Player.Mesh, Level = Targer.Player.Level, Name = Targer.Player.Name, Description = Targer.Player.Description });
                            else
                                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto() { UID = mem.UID, Mesh = mem.Mesh, Level = mem.Level, Online = 0, Name = mem.Name, Description = mem.Description });
                        }
                    }
                }
                user.Send(msg.CreateFriendInfo(Info));
            }
        }
        [DataAttribute(ActionType.ShowPartner)]
        public unsafe static void ShowPartner(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (user.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Partener))
            {
                var Info = new MsgFriendInfo.FriendProtoStructure() { Type = 1, unk1 = 1, Items = new List<MsgFriendInfo.FriendProtoStructure.ConstructsProto>() };
                foreach (var typ in user.Player.Associate.Associat)
                {
                    foreach (var mem in typ.Value.Values)
                    {
                        if (typ.Key == Role.Instance.Associate.Partener)
                        {
                            Client.GameClient Targer;
                            if (Pool.GamePoll.TryGetValue(mem.UID, out Targer))
                                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto() { UID = Targer.Player.UID, Mesh = Targer.Player.Mesh, Level = Targer.Player.Level, Online = 1, Name = Targer.Player.Name, Description = Targer.Player.Description });
                            else
                                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto() { UID = mem.UID, Mesh = mem.Mesh, Level = mem.Level, Online = 0, Name = mem.Name, Description = mem.Description });
                        }
                    }
                }
                user.Send(msg.CreateFriendInfo(Info));
            }
        }
        [DataAttribute(ActionType.Description)]
        public unsafe static void Description(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            data.ObjId = user.Player.UID;
            user.Player.Description = data.Strings[0].ToString();
            user.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.AnimaCrest)]
        public unsafe static void AnimaCrest(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (global::Qahr.Mobile.Phone.MobileQuestGuide.OnLuaCall(user, data.Strings))// QahrMobile: main-story Lua calls
                return;
            if (data.Strings.Contains("</F>User_OpenDialog</N>3833</N>3</N>0</N>4200001"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 3833;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.NpcID = 4200001;
                data.Type = 126;
                data.Fascing = 3;
                data.PositionX = user.Player.X;
                data.PositionY = user.Player.Y;
                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_OpenMain"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;

                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=2,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };

                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>1"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 1;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=1,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };

                user.Send(msg.ActionCreate(data));
            }
            // Lake Boss is unreachable (OpenMain is never pushed, no entry state/cost exists): no teleport, just close the dialog.
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>1"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }//
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>2"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 2;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=2,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };

                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>2"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }//
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>3"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 3;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=3,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };
                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>3"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }//
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>4"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 4;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=4,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };
                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>4"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }//
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>5"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 5;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=5,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };

                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>5"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }//
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_SwitchInterface</N>6"))
            {
                data.ObjId = user.Player.UID;
                data.dwParam = 10053;
                data.Type = 258;
                data.Fascing = 6;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.Strings = new string[1] { "DlgVeteranBossUi_OpenMainView|{['Data']={['BossIndex']=6,['BossTime']=0,['AllBossState']={0,1,0,0,0,1,1}}}" };

                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>LakeMonsterDrop_FindMonster</N>6"))
            {
                data.ObjId = user.Player.UID;
                data.Strings = new string[1] { "DlgVeteranBossUi_Close" };
                user.Send(msg.ActionCreate(data));
            }
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10152</S>ZongFang_ActOpen"))
            {
                data.ObjId = user.Player.UID;
                data.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                data.NpcID = 10152;
                data.Type = 126;
                data.Fascing = 3;
                data.PositionX = user.Player.X;
                data.PositionY = user.Player.Y;
                user.Send(msg.ActionCreate(data));
            }
            // [LuaUi] top bar refresh (the client asks when the Fgui button bar becomes visible)
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10107</S>ButtonStorage_Refresh"))
            {
                global::Qahr.Mobile.LuaUi.LuaPush.SendTopBar(user, true);
            }
            // [LuaUi] Quell Demons top icon (1274) -> the same window as LakeMonsterDrop_OpenMain
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10053</S>NewServerBossAct_OpenInterface"))
            {
                global::Qahr.Mobile.LuaUi.LuaPush.OpenQuellDemons(user);
            }
            // [LuaUi][DreamTheater] Dream Theater top icon (DlgWhiteDayBtn)
            else if (data.Strings.Contains("</F>ClientManager_Callback</N>10369</S>WhiteDayLua_OpenMain"))
            {
                global::Qahr.Mobile.DreamTheater.DreamTheaterModule.LuaOpenMain(user);
            }
            // [LuaUi] the other callbacks of the Lua windows this server answers (Dream Theater window, festival icons)
            else if (global::Qahr.Mobile.LuaUi.LuaPush.HandleCallback(user, data))
            {
            }
            else if (UnknownPacketLog.Enabled)
            {
                string func = data.Strings != null && data.Strings.Length > 0 ? data.Strings[0] : "";
                UnknownPacketLog.Log("2099/258 " + UnknownPacketLog.Cut(func), user.Player.Name);
            }
        }
        [DataAttribute(ActionType.ReturnReward)]
        public unsafe static void ReturnReward(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            // QahrMobile: on the phone 470 is Daily Packs "Buy All (7 Days)" (CDailyGiftMgr::ActionBuy7AllDailyGift).
            // Without this the phone got the free PC return reward below on every tap.
            if (global::Qahr.Mobile.Phone.MobileDailyGift.Handle(user, msg, data))
                return;
            if (data.dwParam == 1)
            {
                if (user.Inventory.HaveSpace(2))
                {
                    user.Inventory.Add(msg, 730005, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                    user.Inventory.Add(msg, 3322761, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                    user.Send(msg.ActionCreate(data));
                }
            }
            else
            {
                if (user.Inventory.HaveSpace(2))
                {
                    user.Inventory.AddItemWitchStack(3003126, 0, 30, msg, true);
                    user.Inventory.AddItemWitchStack(3002926, 0, 30, msg, true);
                    user.Inventory.AddItemWitchStack(3002030, 0, 30, msg, true);
                    user.Send(msg.ActionCreate(data));
                }
            }
            

        }
        [DataAttribute(ActionType.GetPrizeFlower)]
        public unsafe static void GetPrizeFlower(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (!user.Player.HaloTitles.Contains((MsgTitleStorage.HaloType)data.PositionX))
            {
                user.Player.AddSpecialHalo((Game.MsgServer.MsgTitleStorage.HaloType)data.PositionX, msg);
            }
            if (user.Player.HaloTitles.Contains((MsgTitleStorage.HaloType)data.PositionX))
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue(data.PositionX, out dbtitle))
                {
                    user.Player.FairyForm = data.dwParam2;

                    user.Player.SendUpdate(msg, 0, 0, user.Player.TodayFlowerType, data.dwParam2, MsgUpdate.DataType.FlowerRank, true);

                    if (data.dwParam2 > 0)
                    {
                        user.Player.SpecialTitleScore = dbtitle.Score;
                        user.Player.SpecialHaloID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                        var pQuery = new MsgTitleStorage.TitleStorage();
                        pQuery.ActionID = MsgTitleStorage.Action.Equip;
                        pQuery.dwparam2 = data.PositionX;
                        pQuery.dwparam3 = data.PositionY;
                        user.Send(msg.CreateTitleStorage(pQuery));
                        pQuery.ActionID = MsgTitleStorage.Action.UseTitle;
                        pQuery.Title = new MsgTitleStorage.Title();
                        pQuery.Title.ID = pQuery.dwparam2;
                        pQuery.Title.SubId = pQuery.dwparam3;
                        pQuery.Title.dwparam1 = 1;
                        user.Send(msg.CreateTitleStorage(pQuery));
                        user.Player.View.SendView(user.Player.GetArray(msg, false), false);
                    }
                    else 
                    {

                        user.Player.SpecialTitleScore = user.Player.SpecialHaloID = 0;
                        var pQuery = new MsgTitleStorage.TitleStorage();
                        pQuery.ActionID = MsgTitleStorage.Action.UnEquip;
                        pQuery.dwparam2 = data.PositionX;
                        pQuery.dwparam3 = data.PositionY;
                        user.Send(msg.CreateTitleStorage(pQuery));
                        user.Player.View.Clear(msg);
                        user.Player.View.Role(false);
                        user.Player.View.SendView(user.Player.GetArray(msg, false), false);
                    }
                }
            }
                      
        }
        [DataAttribute(ActionType.DunePower)]
        public unsafe static void DunePower(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (user.Player.ContainFlag(MsgUpdate.Flags.BloodThirst))
            {
                user.Player.RemoveFlag(MsgUpdate.Flags.BloodThirst);
            }

            user.Player.AddSpellFlag(MsgUpdate.Flags.Dune, 10, true);
            user.Player.AddSpellFlag(MsgUpdate.Flags.BloodThirst, 10, true);
            user.Player.SendUpdate(msg, MsgUpdate.Flags.ConceptionVessal, 10, 1, 0, 10, 0, MsgUpdate.DataType.ArchiveSkill);
            user.Player.SendUpdate(msg, MsgUpdate.Flags.BloodThirst, 10, 65, 0, MsgUpdate.DataType.ArchiveSkill);
            user.Player.DuneEnergy = 0;
            user.Player.DuneEnergy2 = 0;
            user.Player.DuneEnergy3 = 0;
            Game.MsgServer.AttackHandler.DuneSkills.UpdateEnergy(user);
            user.Equipment.QueryEquipment(user.Equipment.Alternante);
            user.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.DuelAction)]
        public unsafe static void DuelAction(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            if (user.Player.ContainFlag(MsgUpdate.Flags.PortadorRuneDuel))
                user.Player.RemoveFlag(MsgUpdate.Flags.PortadorRuneDuel);
            var Duel = Pool.Magic[(ushort)Role.Flags.SpellID.Duel][user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Duel].Level];
            Client.GameClient attacked;
            if (Pool.GamePoll.TryGetValue(user.Player.DuelUID, out attacked))
            {
                user.Player.DuelUID = 0;
                attacked.Player.AddFlag(MsgUpdate.Flags.Duel, (int)Duel.DamageOnHuman, true);
                attacked.Player.DuelPercent = (byte)Duel.Damage;
            }
            user.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ViewNinjaInspired)]
        public unsafe static void ViewNinjaInspired(Client.GameClient user, ServerSockets.Packet msg, ActionQuery data)
        {
            Client.GameClient Target;
            if (Pool.GamePoll.TryGetValue((uint)data.TargetUID, out Target))
            {
                if (Target.MyNinja != null)
                {
                    Qahr.Mobile.Game.MsgServer.MsgGouYuInfo.MsgGouYuInfoProto.Create(user, Target.MyNinja, Qahr.Mobile.Game.MsgServer.MsgGouYuInfo.MsgGouYuInfoProto.TypeID.View, Target.Player.UID);
                    MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(user, Target.MyNinja, Target.Player.UID);
                }
                if (Target.HundredWeapons != null)
                {
                    if (Pool.GamePoll.ContainsKey(data.TargetUID))
                        user.Send(msg.CreateHundredWeaponsInfo(Pool.GamePoll[data.TargetUID], MsgHundredWeaponsInfo.ActionID.RequestInfo));
                    else
                        user.Send(msg.CreateHundredWeaponsInfo(null, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                }
                if (Target.MyArchives != null
                    && (AtributesStatus.IsMonk(Target.Player.Class)
                    || AtributesStatus.IsThunderStriker(Target.Player.Class) 
                    || AtributesStatus.IsWindWalker(Target.Player.Class)))
                {

                    Archives.Item[] MyArchvies = null;
                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Suanni).ToArray();



                    if (MyArchvies != null)
                    {
                        Qahr.Mobile.Game.MsgServer.MsgCombatGear.ProtoStructure obj = new Qahr.Mobile.Game.MsgServer.MsgCombatGear.ProtoStructure();
                        obj.Type = (byte)Qahr.Mobile.Game.MsgServer.MsgCombatGear.ProtoStructure.Action.View;
                        obj.UID = Target.Player.UID;
                        obj.Items = new Qahr.Mobile.Game.MsgServer.MsgCombatGear.ProtoStructure.ConstructsProto[Target.MyArchives.Items.Count];
                        int i = 0; foreach (var Item in MyArchvies)
                        {
                            obj.Items[i] = new Qahr.Mobile.Game.MsgServer.MsgCombatGear.ProtoStructure.ConstructsProto();
                            obj.Items[i].ID = (ushort)Item.ItemID;
                            obj.Items[i].Level = (byte)Item.Level;
                            obj.Items[i].Progress = Item.Progress;
                            obj.Items[i].Hash = Item.Hash;
                            obj.Items[i].dwParam = Item.dwParam;
                            obj.Items[i].jade1 = (uint)Item.Jades[0].JadeID;
                            obj.Items[i].jade2 = (uint)Item.Jades[1].JadeID;
                            obj.Items[i].jade3 = (uint)Item.Jades[2].JadeID;
                            obj.Items[i].jade4 = (uint)Item.Jades[3].JadeID;
                            obj.Items[i].jade5 = (uint)Item.Jades[4].JadeID;
                            obj.Items[i].jade6 = (uint)Item.Jades[5].JadeID;
                            obj.Items[i].MasteryPoints = Item.MasteryPoints;
                            obj.Items[i].Debris = Item.Debris;
                            i++;
                        }
                        user.Send(msg.CreateArchives(obj));

                    }
                }

            }
            user.Send(msg.ActionCreate(data));
        }
    }
}