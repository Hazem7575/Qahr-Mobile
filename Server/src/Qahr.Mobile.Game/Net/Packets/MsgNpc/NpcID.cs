using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgNpc
{
    public enum NpcID : uint
    {
        ShadowCastleEvent = 999990,
        InfinityWar = 999991,
        DarkFlagBase = 999992,
        LuckyGoldenBoxes = 999993,
        FlagsWar = 999994,
        EliteGuildWar = 999995,
        MortalKnightsWar = 999996,
        SquidwardOctopus = 999997,

        NpcAllCollections = 531111,
        //FairyMaria = 800012,
        #region All Small Tournaments
        AllTournamentsInfinity = 900000,
        #endregion
        GodzillaNPC = 955551,
        ExchangeDB = 2072,
        TeleportNewMap = 324328,
        KillersNewWar = 631287,
        InfinityWarPrize = 631288,
        TitanWarPrize = 500009,
        MortalKnightsWarPrize = 500008,
        HelpDeskGame = 65511155,
        UnionWarNPC = 63333,
        ScoreGuildWarNPC = 64444,
        ChangeLordTokensNPC = 6222222,
        JadesKylins = 800000,
        JialManger = 54654,
        SubClassFixedNPC = 631111,
        LuoTingyu = 34721,
        YeGuzhou = 34720,
        OpenArchivess = 34719,

        SusanooArchiveNinja = 25850,
        NobilityNPC = 9994111,
        ShopCoins = 53112244,
        NpcBagCPs = 999991,
        BigMonsters = 5663211,
        ExchangeMoneyandCPs = 953977,
        PromotionFull = 953911,
        NPCLevel130 = 19011,
        rareRuns = 1131306,
        ArchivesMonk = 997314,//10448MapID   

        NewEventsSignUP = 113136,
        TopsPrize = 2,
        LandOfMolesVIP = 324324,
        clearinventory = 5154,
        GoToTwinCity = 11000157,

        ServerEvents = 51540,


        OpenEonspiritSystem = 6311447,
        QuestTwin = 120001,
        QuestPhoienx = 120002,
        QuestApe = 120003,
        QuestDesert = 120004,
        QuestBird = 120005,
        DragonMassagner = 23746,
        HuntLevelAndCps = 23056,
        HuntLevelAndCps2 = 23058,
        OpenHalo = 500333,
        Solo = 29251,
        OnlinePoints = 50032,
        WorldPoints = 50033,
        DonatePointsNpc = 500034,
        AllNewArchives = 50034,
        Caesar = 193531,
        CoatWarehouseSale = 32173,
        StarDragonBall = 19351,
        ConquerLeter = 19352,
        SellRunes = 19353,
        StudyPoints = 52302,
        PointsChange = 52301,
        BoundItemPromotion = 19350,
        SeniorAssistant = 17573,
        MerchantClerk = 1880,
        GuildChief = 4362,
        CPCastle = 201012,
        ChangeAnimaForCps = 1000548,
        ChangeRuneForCps = 1000547,
        UpdateLevelStuff = 20084122,
        BestFighter = 20081010,
        OnePunch = 20081011,
        Lastblood = 20081012,
        KillerEvent = 20081018,
        PrizeKillerEvent = 20081019,
        TeamKiller = 20081016,
        PrizeBestFighter = 20081013,
        PrizeOnePunch = 20081014,
        PrizeLastblood = 20081015,
        PrizeTeamKiller = 20081017,
        Teleport = 200810,
        FullInnerPower = 123100,
        WorldInnerPower = 112200,
        ClanWarTwin = 121321,
        ClanWarPhoienx = 121322,
        ClanWarApe = 121323,
        ClanWarDesert = 121324,
        ClanWarBird = 121325,
        BotJail = 220341,
        DemonNewBox = 22034,
        DemonNewBoxOut = 20000,
        RecoverSkill = 22035,
        UnLockAnimaLocked = 9539,
        PokerA = 23940,
        PokerB = 23941,
        GrandWinner = 23942,
        TournamentSteward = 22065,
        BallonAnima = 22066,
        RelicFuse = 215,
        CrystalMerchant = 7676,
        FiendLairHerald = 7675,
        Longine = 28967,
        BountyHall = 26228,
        HideEffect = 3232324,
        ShopCollection = 315432,
        TaoOpen = 315420,
        LeeLongArchives = 225421,
        PirateOpen = 225420,
        #region ItemLottery
        LilyCard = 3331782,
        PowerBoosterPack = 3319466,
        LuxuryRelicChest = 3322219,
        RuneStarStonePack = 3331667,
        RareSteedPack = 3318571,
        RareSteedPack8 = 3318567,
        #endregion
        PeachJade = 27058,
        DelicateFoxEars = 3320764,
        LoveHat = 3320765,
        MysteryFruit = 3001044,
        TomatoUmbrella = 3318834,
        HotTurkey = 3318841,
        OceanSecret = 3318842,
        PhoenixLegend = 3318848,
        GraspofLove = 3318849,
        ThunderGloryGem = 3311693,
        #region EpicArcher
        MysticAltar = 200013,
        TalentStatue = 200014,
        UniversalLoveStatue = 200010,
        UniversalBank = 23492,
        UniversalBanktwo = 289951,
        MoChu = 200008,
        AntifatalismStatue = 200007,
        EpicArcher = 3324521,
        #endregion
        #region EpicNinja
        EpicNinja = 3305415,
        OneStDivineShadow = 10660,
        TwoStDivineShadow = 10661,
        ThreeStDivineShadow = 10662,
        OneStSwapperStarry = 10663,
        TwoStSwapperStarry = 10664,
        ThreeStSwapperStarry = 10665,
        YinYangStele = 10657,
        ElementalGuard = 10658,
        WheelOfLife = 10659,
        #endregion
        AntiwarStatue = 200006,
        BlackShadow1 = 200011,
        BlackShadow2 = 200026,
        BlackShadow3 = 200016,
        CommanderYee = 200039,
        #region EpicPirate
        EpicPirate = 3307449,
        Mermaid = 21426,
        JackSparrow = 21427,
        Luffy = 21449,
        OneStarTreasureMap = 3307443,
        TwoStarTreasureMap = 3307444,
        ThreeStarTreasureMap = 3307445,
        FourStarTreasureMap = 3307446,
        FiveStarTreasureMap = 3307447,
        #endregion
        EpicTrojan = 3305414,
        EpicMonk = 3007564,


        ArchivesWarrior = 200111,
        HuaMulan = 27641,
        Ares = 27642,
        Tyre = 27643,
        GeneralXiao = 27644,

        ArchivesArcher = 35540,
        TaoBless = 35630,
        MoLian = 35541,
        LuBan = 35670,
        MasterYan = 35672,
        MoDi = 35673,
        BotHandler = 3600246,
        ChiShop = 15478,
        HideNSeek = 81700,
        BankCps = 545670,
        ClassFull = 465830,
        ChiSuper = 46872,
        ChiFull = 46873,
        GameSquid = 84189,
        NewServerReward = 81089,
        Star3GarmenMount = 21214,
        Star5GarmenMount = 21215,
        ChargeChi = 728156,
        Relic = 3314913,
        ChangeToken = 3001304,
        ArchivesNinja = 548205,
        ClassSkillTeleporter = 142205,
        TitleAndWings = 5569,
        Leveling = 81703,
        NewPromotion2 = 65787,
        ArenaRoom = 1310,

        HuntingMaps = 1995,
        NewPlayer = 57812,
        NewPlayer2 = 578123,
        EU = 52100,
        OutRoomFB5M = 1311,
        OutRoomN5M = 1312,
        OutRoomN10M = 5061,
        OutRoomN25M = 5062,
        BotHunt = 3001304,
        FullStuff = 66598,
        L = 101010,
        FullAccountNightMareCo = 54598,
        OutRoomN50M = 5063,
        HpSeller = 7751,
        OutRoomFB10M = 5053,
        OutRoomFB25M = 5054,
        OutRoomFB50M = 5055,

        NewEvent = 770008,
        NightMareNpc = 71943,
        NpcSha7nGame = 641374,
        RewardPoints = 301856,
        DonationMoneybotjail = 641373,
        itemRewordChristmas = 43877,
        GiftBackUp = 63197,
        SecurityAnima = 489123,
        InnerPower = 78954,
        InnerPower2 = 78955,
        Buyanimanpc = 489138,
        SwordMaster = 24312,
        OpenArchives = 243197,
        Teleportall = 15641,
        BuyEpicPirate = 78156,
        ShopRilec = 456456,
        CliamCup = 54613,
        FullAccountChi = 465124,
        FullAccount = 465123,
        Prizes = 50045,
        updateanima = 456447,


        EgyptCoEvents = 5595,
        NobilityWar = 254,
        WarOfPlayers = 6906,
        CPCastleEvent = 500006,
        //TreasureThiefEvent = 500005,
        GoldBoxWar = 500003,
        ScorePlayer = 100004,
        Count = 2000,
        WarFighters = 9706,
        ClassPoleWar = 65189,
        NobilityPoleWar = 45189,
        GuildPole = 32189,
        Emperrors = 6525,
        FindTheSpy = 126789,
        BackWarFighters = 52417,
        CustomArenaNPC = 9132,
        KingsWar = 65259,
        GuildWarScore = 54541,
        ChampionWarr = 54241,
        SuperGuildWarx3 = 8622,
        WindwalkerGuard = 20028,
        ElderPower = 20029,
        SisterYung = 20030,
        SisterYung2 = 2000589,
        ElderPower2 = 200124,
        DukeofHell = 200002,
        //CobbWind = 20500,
        Glowtownportal = 336644,
        Matchmaker = 30000,

        DragonIslandConductress = 20508,
        GlowtownPortal1 = 20571,
        GlowtownPortal2 = 20572,
        GlowtownPortal3 = 20573,
        IronTong = 20499,
        CobbWind = 20500,
        JadeCourtesan = 4586,
        DayBox = 4592,
        DarkBox = 4593,
        NightBox = 4594,
        MrLoneliness = 4595,
        SparrowPillar = 4587,
        DragonPillar = 4588,
        NobilityDonor = 2541,
        ReallotPoints = 2542,
        TigerPillar = 4589,
        LeopardPillar = 4590,
        HerbalistChou = 4584,
        VillagerChou = 4582,
        KunlunWanderer = 4583,
        Platform = 4591,
        Shop2 = 64359,
        MoneyMilyaers = 5317,
        MoneyChang = 53279,
        DancePK = 6017,
        Stuff = 77839,

        Cauldron = 4578,
        BlockStone = 4603,
        Plum = 4572,
        RuanBrother = 4568,
        RuanGoodMan = 4570,
        RuanBetterMan = 4571,
        DreamDoor = 4577,
        SeasonDoor = 4576,
        NetPeddler = 4579,
        YeSheng = 4569,
        TimeDoor = 4575,
        TreasureBox = 4580,
        TreasureBox1 = 4600,
        TreasureBox2 = 4601,
        TreasureBox3 = 4602,
        YangYun2 = 4567,
        YangYun1 = 4566,
        GhoulKong = 4573,
        SugarTang = 4565,

        MrWonder = 10850,

        NaughtyBoy = 8269,

        Celestine = 20005,

        OldBeggar = 4704,

        MasterMoMo = 4702,

        Louis = 4651,
        Shark = 4652,

        MrFree = 3216,

        LeadWrangler = 8616,
        ChiMaster = 7781,
        Sage = 7780,
        Warlock = 7779,
        MartialArtist = 7778,
        ApothecarySubClass = 7777,
        Performer = 7776,

        SubClassManager = 8591,

        ArenaGuard = 4432,
        IronsmithChou = 4434,
        PrizeOfficer2 = 10223,

        BirdExorcist = 300010,
        CommanderKerry = 4707,

        CommanderAid1 = 4708,
        CommanderAid2 = 4709,
        CommanderAid3 = 4710,
        CommanderAid4 = 4711,

        Felix = 4705,
        GuideZhang = 4706,


        PoorXiao = 4700,
        DoctorKnowitAll = 4701,

        GeneralPakMap3 = 10603,
        MrMirrorMap3 = 10604,
        DivineJadeMap3 = 10605,


        GeneralPakMap2 = 10600,
        MrMirrorMap2 = 10601,
        DivineJadeMap2 = 10602,

        EpicTrojanDivineJade = 10606,
        MonkMisery = 10583,
        MrMirror = 10582,
        MrMirror2 = 10619,
        GeneralPak = 10581,

        MonkMisery1 = 10584,

        PaksGhost = 10579,


        Epic2MonkMisery = 10585,
        Epic2ArrayEye1 = 10589,

        ServerTransfer = 15702,

        ArenaManagerWang = 1881,
        Agate1 = 1882,
        Agate2 = 1883,
        Agate3 = 1884,
        Agate4 = 1885,
        Agate5 = 1886,
        Agate6 = 1887,
        Agate7 = 1888,

        PrizeCenterTeleporter1 = 9619,
        PrizeCenterTeleporter2 = 9620,
        PrizeCenterTeleporter3 = 9621,
        PrizeCenterTeleporter4 = 9622,

        MammonEnvoy = 9623,

        PrizeCenterTeleporter5 = 9650,
        PrizeCenterTeleporter6 = 9651,
        PrizeCenterTeleporter7 = 9652,
        PrizeCenterTeleporter8 = 9653,
        HuntCoins = 51198,
        FlameAltar = 19165,

        WoundedBrightTribesman = 19183,
        BrightGuard = 19184,

        LavaFlower1 = 19239,
        LavaFlower2 = 19172,
        LavaFlower3 = 19173,
        LavaFlower4 = 19174,
        LavaFlower5 = 19176,
        LavaFlower6 = 19177,
        LavaFlower7 = 19238,

        WhiteHerb1 = 19167,
        WhiteHerb2 = 19168,
        WhiteHerb3 = 19169,
        WhiteHerb4 = 19170,
        WhiteHerb5 = 19171,
        WhiteHerb6 = 19175,

        ChingYan = 19164,
        ChongYan = 19162,
        PakYan = 19160,
        RemainofBrightTribesman = 19161,
        BrokenForgeFurnace = 19163,

        TowerOfMysteryLayerChange = 19139,
        TowerofMysteryChallenge1 = 200020,
        TowerofMysteryChallenge2 = 200025,
        TowerofMysteryChallenge3 = 200018,
        TowerofMysteryChallenge4 = 200024,

        CloudSweeper = 19128,
        TowerofMysteryConductor = 19231,
        TowerofMysteryConductor2 = 19194,
        TowerKeeper = 19127,
        GuidofFieryDragon = 19166,


        BronzePhoenixCup = 19504,
        SilverPheoenixCup = 19505,
        GoldenPhoenixCup = 19506,
        HolyPhoenixCup = 19507,

        CSElitePkManager = 19424,
        IdealDesgner = 19436,
        AstralPheonix = 19452,

        HosuuNpcs = 25415,

        MoonMaiden = 8296,
        PkReset = 52003,
        NewChi = 54103,
        Crystal = 19121,
        KingdomMissionEnvoy = 17400,
        RealmEnvoy = 18787,


        VoteManager = 52300,

        Level140 = 19039,
        ShopP7 = 54678,

        ExplosiveDevide1 = 10951,
        ExplosiveDevide2 = 10950,
        ExplosiveDevide3 = 10949,
        ExplosiveDevide4 = 10948,

        Crystal1 = 19121,
        Crystal2 = 19122,
        Crystal3 = 19123,
        Crystal4 = 19124,
        Crystal5 = 19125,




        PokerMillionaireLee = 19111,

        FarmerLynn = 4718,
        Carolyn = 4719,
        Harvey = 4621,
        AuntPeach = 4633,

        StoneColumn = 4622,
        ObscureWarrior = 4623,
        StoneColumn1 = 4626,
        StoneColumn2 = 4624,
        StoneColumn3 = 4625,
        StoneColumn4 = 4627,


        Dark_MoMo = 4526,
        Dark_SweetyPuddy = 4513,
        Dark_SweetyLily = 4514,
        Dark_SweetyMindy = 4515,
        Dark_SweetyDay = 4516,
        Dark_SweetyCuty = 4517,
        Dark_FiendAltar1 = 4522,
        Dark_FiendAltar2 = 4523,
        Dark_FiendAltar3 = 4524,
        Dark_FiendAltar4 = 4525,


        Ghost_MoMo = 4506,
        Ghost_SweetyPuddy = 4507,
        Ghost_SweetyLily = 4508,
        Ghost_SweetyMindy = 4509,
        Ghost_SweetyDay = 4510,
        Ghost_SweetyCuty = 4511,
        Ghost_FiendAltar1 = 4518,
        Ghost_FiendAltar2 = 4519,
        Ghost_FiendAltar3 = 4520,
        Ghost_FiendAltar4 = 4521,


        TaoistShine = 4505,
        JerkWang = 4504,
        AuntZhang = 4503,
        GeneralArmand = 8539,

        GreenSnake = 30115,
        Mulan = 8542,
        Harriet = 8541,
        Alvis = 8538,

        BiVillageHead = 8537,

        AdjutantLi = 8562,
        OldZhang = 8540,
        SaltedFish = 8545,
        FishingNet = 8544,


        SoldiersMother = 8536,

        SoldierZhang = 8535,

        SouthwestIsland = 8559,
        WestIsland = 8558,
        NorthwestIsland = 8561,
        NorthIsland = 8557,
        NortheastIsland = 8556,
        CentralIsland = 8560,

        CookYuan = 8528,

        SoldierGuan = 8529,
        SoldierXu = 8530,
        SoldierZheng = 8531,

        SoldierFei = 8532,






        MysteryTaoist = 8533,
        TruthTaoist = 8527,

        WhiteChrysanthemum = 8516,
        Jasmine = 8517,
        Lily = 8515,
        WillowLeaf = 8518,


        ViceGeneral = 8534,
        StrangeBox = 8522,
        XimenQing = 8514,

        CityGeneral = 8521,
        WindTaoist = 8519,
        WealthyWan = 8524,
        WealthyWansWife = 8523,
        LittleBen = 8520,
        ArmorerYu = 8513,
        IslandGeneral = 8525,

        BIViceCaptain = 8512,
        Xi_er = 8526,

        XuFan = 8511,
        BICastellan = 8510,

        WitheredTree1 = 8429,
        WitheredTree2 = 8430,
        WitheredTree3 = 8431,

        Lotus = 8428,
        SpringSoldierZhao = 8449,
        SoldierMu = 8442,
        TaoistSpring = 8448,

        ElderJiang = 8450,
        ConvoyViceLeader = 8444,

        SpringViceGeneralOu = 8446,

        SculptorHe = 8447,
        SpringGeneralXu = 8445,
        ConvoyLeaderGu = 8443,
        HanCheng = 8441,
        st1TreeSeed = 8451,
        nd2TreeSeed = 8452,
        rd3TreeSeed = 8453,

        DCWanYing = 8440,
        // DCGeneralZhuGe =8442,
        IronsmithLi = 8438,
        ViceGeneralDong = 8439,

        KeYulunsFollower = 8435,
        KeYulun = 8434,


        VipQuest = 52002,

        Arthur = 3601,
        DCViceGenera = 8426,
        TaoistYu = 8427,
        GeneralZhuGe = 8437,
        DCViceCaptain = 8425,
        HeresySnakemanLeader = 30105,
        MountRetailer = 5517,
        CaptorCooke = 8298,
        SnakemanLeader = 30101,
        HarmonyTaoist = 8297,

        ArdenteTaoist = 8293,
        TempestTaoist = 8294,
        BreezePupil = 8295,
        CliffFlower1 = 8288,
        CliffFlower2 = 8300,
        Apothecary = 8304,
        PoisonMaster = 8299,

        KingOfTheHill = 52006,
        SkillTournament = 52005,

        FreezeWar = 52004,
        Football = 52001,
        OldQuarrier = 422,
        Norbert = 421,

        //task manager
        ThiefWong = 4436,
        ThiefChen = 4437,
        VeteranHong = 4464,
        ZhaoJian = 4465,
        ScholarWu = 4482,
        PharmacistMuMu = 4483,
        BianQing = 4484,
        ApprenticeLuo = 4466,
        Minstrel = 4500,
        DealerShen = 4501,
        ShenJunior = 4502,
        OfficerBao = 4485,
        DoctorLi = 4486,
        GeneralAmber = 4487,
        PainterFengKang = 4489,
        GuardLi = 4490,
        DuSan = 4468,
        GeneralJudd = 1611,
        PharmacistDong = 4470,



        ACCastellan = 8590,
        Lydia = 8289,
        StoneApe = 8290,
        CarpenterJack = 8291,
        JackDaniel = 8292,

        AC_Lieutenant = 8287,
        Revenant1 = 4491,
        Revenant2 = 4492,
        Revenant3 = 4493,
        Revenant4 = 4494,
        Revenant5 = 4495,

        CrystalRiddler = 3664,
        Blessed1 = 350161,
        GreatMerchant = 432,

        RedPowder = 4471,
        OrangePowder = 4472,
        YellowPowder = 4473,
        GreenPowder = 4474,
        BluePowder = 4475,
        IndigoPowder = 4476,
        PurplePowder = 4477,


        FindBox = 52000,
        FindBoxLeave = 4026320,

        SuperGuildWar = 25015,
        PokerMillinaireLee = 19111,
        PokerCasinoHostess = 6298,
        PokerWarehouseman = 7762,
        PokerCpsCasino = 6299,

        ArtisanLuo = 4467,
        XuLiang = 7992,
        KungFuKing = 17222,
        TCViceCaptain = 7991,
        PeachTree = 7997,
        PeachTree2 = 7996,
        RuHua = 7993,
        Fortuneteller = 600050,
        MasterHao = 7994,
        YuLin = 7995,
        YuJing = 8267,
        ForestGuvernor = 8266,
        IntelligenceAgent = 8270,
        BanditBoss = 8272,//,2,4406,1011,227,404
        PhoenixCastellan = 8271,
        VillageHead = 8273,
        BoldBoy = 8274,
        FurDealer = 8275,
        HunterWong = 8268,
        WineKiddo = 4596,
        MilitiaTiger = 8278,
        MilitiaDragon = 8279,
        MilitiaCaptain = 8277,
        MilitiaLeopard = 8280,
        WarrantOfArrest = 8281,


        NameRegister = 7935,
        DrYinYang = 15805,

        //daily
        #region DailyQuest Level 50
        HeavenlyTreasureChest = 9894,
        MythicTreasureChest = 9893,
        FairyTreasureChest = 9892,
        #endregion

        Censer1 = 9954,
        Censer2 = 9953,
        Cense3 = 9952,
        GoodManLiu = 9891,
        DailyQuestEnvoy = 9998,
        //
        DesertGuardian = 301,
        MagnoliaEnvoy = 9897,
        HeavenlyMaster = 8233,

        EartMaster = 200012,
        //
        FairyCloud = 16050,
        MysterCheast1 = 16051,
        MysterCheast2 = 16052,
        MysterCheast3 = 16053,
        MysterCheast4 = 16054,
        MysterCheast5 = 16055,

        //-------------------------


        //epic quest taoist------
        RefinedPureOven = 25010,
        ElitePureOver = 25011,
        SuperPureOver = 25012,
        SunKing = 25013,
        PureTaoist = 25014,
        //------------------------
        //epic quest monk------------
        FortuneArhat = 25001,
        AltarCleanser = 25002,
        VicotryBuddha = 25003,
        WhiteDragon = 25004,
        GoldenCicada = 25005,

        //------------------
        InnerPowerNpc = 18786,
        Shelby = 300000,
        TcBoxer = 101611,
        ApeBoxer = 101623,
        DesertBoxer = 101625,
        PheonixBoxer = 101627,
        BirdBoxer = 101629,

        TCCaptain = 2001,
        PCCaptain = 2002,
        AMCaptain = 2003,
        DCCaptain = 2004,
        BCaptain = 2005,
        MCaptain = 2006,

        SkillTeamPkManager = 8592,
        TeamPkManager = 8158,

        SteedRace = 6001,
        KillerOfElite = 780,
        ExtremePk = 4699,
        TeamDeathMatch = 777,
        DragonWar = 778,
        //Top = 779,
        HeroOfGame = 779,//
        DragonYuKoon = 20823,
        GarmentsDealer = 50029,
        MountsDealer = 50030,
        MarketShop = 50031,
        OnlineHour = 50032,
        BetArena = 20580,
        Arena = 15090,
        Schedules = 50035,
        Beautician = 10020,
        GeneralOfDefense = 23091,
        Autumn = 19317,
        ArtisanZhuRong = 22559,
        AccessoriesDealer = 50048,
        BeauticianDealer = 50049,
        HuntingArea = 50060,
        CloudBeast = 1503,
        Ambassador = 22972,
        Loni = 24038,
        MysterySage = 23021,
        TwinDisCityMain = 3215,
        DisCityMain = 9280,
        DisCityMap1 = 4026314,
        DisCityMap2 = 4026315,
        DicCityMap4 = 4026316,
        ServerRewards = 781,
        DBShower = 666,
        TeleportBosses = 552233,
        PkWar = 31,
        PkWarQuit = 4026313,

        CaptureTheFlag = 8713,
        EquipmentBlacksmith = 9071,

        GodlyArtisan = 10065,

        BattleFieldMain = 10123,
        BattleFieldMap1 = 4026308,
        BattleFieldMap2 = 4026309,
        BattleFieldMap3 = 4026310,

        GWJail = 4026307,

        ExitElitePk = 168052,
        StartNpc = 4026306,

        PrizeOfficer = 47,
        MarketCpAdmin = 2071,
        TaskmasterChang = 8796,
        SellItems = 30015,

        MarialDealer = 15987,
        JailJoin = 43,
        JailWarden = 42,
        WardenZhang = 1002,

        JailCPAdmin = 1000,

        LoveStone = 390,
        OflineTGNpc = 3836,
        ClanNpc = 5533,


        KitMerchant = 7863,
        SkillEraser = 15988,
        FrozenGrottoGeneral = 6144,


        TwinCityConductress = 10050,

        TwinCityConductress1 = 10150,
        TwinCityConductress2 = 10250,
        TwinCityConductress3 = 10350,

        PheonixCityConductress = 10052,
        DesertCityConductress = 10051,
        ApeCityConductress = 10053,
        BirdCityConductress = 10056,
        MarketConductress = 45,
        BoxerConductor = 180,
        WHTwin = 8,
        WHMarket = 44,
        wHPheonix = 10012,
        WHDesert = 10011,
        WHApe = 10028,
        WHBird = 10027,
        WHPokerTwin = 7762,
        WHStone = 4101,
        WHPoker = 25761,
        BlackSmith = 5,//5,32,50,1002,324,230
        TheStorekeeper = 1,
        Pharmacist = 3,
        Armorer = 4,
        KungfuBoy = 5673,
        KungfuMaster = 4488,
        BarberSha = 10002,
        GuildCreator = 10003,
        MailManager = 80540,

        JiangHuNpc = 15745,

        PromotionThunderstriker = 23599,//10449ID
        Thunderstriker = 24270,

        PromotionLeeLong = 17126,//10453MapID
        Dragon = 24266,

        PromotionPirate = 9391,//10447MapID
        Pirate = 24273,

        PromotionWindWalker = 19634,//10454MapID
        WindWalker = 24269,

        PromotionTrojan = 10022,//10446MapID
        Trojan = 24271,

        PromotionArcher = 400,//10451MapID
        Archer = 24265,

        PromotionWarrior = 10001,//10452MapID
        Warrior = 24267,

        PromotionNinja = 4972,//10445MapID
        Ninja = 24268,

        PromotionMonk = 8314,//10448MapID   
        Monk = 24272,

        PromotionTaoist = 10000,//10450MapID
        Taoist = 24274,

        MountTrainer = 5600,
        OpenAstredgeSystem = 7927222,

        HelpDeskInfinity = 29680,

        RebirthMaster = 9072,

        MarketLadyLuck = 923,
        LotteryLadyLuck = 924,
        LotteryCollectorWong = 3952,
        CollectorZhao = 2070,
        UnknowMan = 3825,
        Shopboy2 = 5004,
        GarmentShopKeeper = 6002,
        ConductressNewTwinCity1 = 999100,
        ConductressNewTwinCity2 = 999200,
        ConductressNewTwinCity3 = 999300,
        ConductressNewTwinCity4 = 999400,
        ConductressNewTwinCity5 = 999500,
        ConductressNewTwinCity6 = 999600,
        ConductressNewTwinCity7 = 999700,
        ConductressNewTwinCity8 = 999800,
        ConductressNewTwinCity9 = 999900,
        Confiscator = 4450,
        JailConfiscator = 4542,
        CloudSaint = 2000,
        Ose = 35631,
        MasterXuan = 3085,
        SurgeonMiracle = 3381,
        Costumer = 683,
        ArenaNpc = 10021,
        Shopboy = 10063,
        Tinter = 832662,
        FurnitureStore = 30161,
        HouseAdmin = 30156,
        Class6House = 18950,


        GuildConductor = 380,
        GuildGateKeeper = 7000,

        FlameTaoist = 4452,

        Flame1 = 4453,
        Flame2 = 4454,
        Flame3 = 4455,
        Flame4 = 4456,
        Flame5 = 4457,
        Flame6 = 4458,
        Flame7 = 4459,
        Flame8 = 4460,
        Flame9 = 4461,
        Flame10 = 4462,

        LeftGate = 516074,
        RightGate = 516075,
        SuperGuildWar_CasteleRightGate = 516077,
        SuperGuildWar_CasteleLeftGate = 516076,
        SuperGuildWar_LeftGate = 516078,
        SuperGuildWar_MidleGate = 516079,
        SuperGuildWar_RightGate = 516080,
        ExitSuperGuildWar = 4026311,
        ExitCaptureTheFlag = 4026312,

        GuildConductor1 = 101621,
        TeleGuild1 = 101620,
        GuildConductor2 = 101619,
        TeleGuild2 = 101618,
        GuildConductor3 = 101617,
        TeleGuild3 = 101616,
        GuildConductor4 = 101615,
        TeleGuild4 = 101614,
        GuildOfficer = 101897,

        DesertFrozenGroto = 21116,
        FrozenFroto4Teleporter = 13124,

        FrozenGrottoGuardian5 = 13125,


        Simon = 1152,

        Lab1 = 1153,
        Lab2 = 1154,
        Lab3 = 1155,
        Lab4 = 1156,

        CasinoHostess = 16915,
        GameManager = 6297,
        GameManager2 = 62990,
        Stanley = 3623,
        SwapperStarry = 832663,
        ElementalPool = 832664,
        P7SacredTreasuresEnvoy = 16849,
        NemesysConductort = 9464,

        SarckSoldier = 16833,
        SarckSoldierEnctrance = 16834,
        SarckSoldierSuppluPoint1 = 16852,
        SarckSoldierSuppluPoint2 = 16853,
        SarckSoldierSuppluPoint3 = 16854,
        SarckSoldierSuppluPoint4 = 16855,
        SarckSoldierSuppluPoint5 = 16856,

        SuperMok = 10580,
        DivineJade = 16850,

        ClassPkEnvoy = 662,
        ClassPkWar = 16851,

        ElitePk = 7879,

        DragonFurnaceFusing = 23735,
        DragonFurnaceTwin = 23736,
        DragonFurnaceForging = 23737,
        LuckySmeltingList = 23582,
        SolarFurnace = 23580,
        LunarFurnace = 23581,



        SelectP7WeaponSoulPack = 3004247,
        SelectP7EquipmentSoulPack = 3004248,
        SelectSacredRefineryPack = 3004249,
        Steed1 = uint.MaxValue - 4,
        Steed3 = uint.MaxValue - 5,
        Steed6 = uint.MaxValue - 6,
        DailyItem1 = uint.MaxValue - 7,
        DailyNormalSpiritBead = uint.MaxValue - 8,
        DailyRefinedSpiritBead = uint.MaxValue - 9,
        DailyUniqueSpiritBead = uint.MaxValue - 10,
        DailyEliteSpiritBead = uint.MaxValue - 11,
        DailySuperSpiritBead = uint.MaxValue - 12,
        Level43UniqueRingPack = uint.MaxValue - 13,
        NobleSteedPack = uint.MaxValue - 14,
        RareSteedPack6 = uint.MaxValue - 15,
        DazzlingDiamondBox = UInt32.MaxValue - 16,
        TempestSecretLetter = uint.MaxValue - 17,
        SashFragment_Realm = uint.MaxValue - 18,
        GarmentPacket = uint.MaxValue - 19,
        MountPacket = uint.MaxValue - 20,
        MountPacket2 = uint.MaxValue - 21,
        AccesoryPacket = uint.MaxValue - 22,
        GarmentPacket2 = uint.MaxValue - 23,
        AccesoryPacket2 = uint.MaxValue - 24,
        MountPacket3 = uint.MaxValue - 25,
        GoldPrizeToken = 963963,

        BlackFridayGarmentPack = uint.MaxValue - 27,
        BlackFridayMountPack = uint.MaxValue - 28,
        BlackFridayAccesory = uint.MaxValue - 29,

        Steed1Pack = uint.MaxValue - 30,
        Steed3Pack = uint.MaxValue - 31,
        ChiToken = 728156,
        TaiChiDemonBox = 3303365,
        HeavenDemonBox = uint.MaxValue - 32,
        ChaosDemonBox = uint.MaxValue - 33,
        SacredDemonBox = uint.MaxValue - 34,
        AuroraDemonBox = uint.MaxValue - 35,
        DemonBox = uint.MaxValue - 36,
        AncientDemonBox = uint.MaxValue - 37,
        FloodDemonBox = uint.MaxValue - 38,
        SuperHeadgearPack = uint.MaxValue - 39,
        RingPack = uint.MaxValue - 40,
        ClothingPack = uint.MaxValue - 41,
        PowerBook = uint.MaxValue - 42,


        Level50UniqueWeaponPack = uint.MaxValue - 43,
        Level52UniqueHeadgearPack = uint.MaxValue - 44,
        Level55EliteWeaponPack = uint.MaxValue - 45,
        Level67EliteHeadgearPack = uint.MaxValue - 46,
        L60UniqueGearPack = uint.MaxValue - 1000,


        HellConquer_US = 52101,
        OurCoqnuer_EU = 52102,
        OurCoqnuer_US = 52103,
        Top1 = 106696,
        BlackNameWinner,
        JiangMaker = 15532,
        ChargeJiang = 728173,
        JiangSuper = 710173,
        JiangFull = 710174,
        ChiLevel99 = 710175,
        Top2 = 780,
        Top3 = 781,
        Top4 = 782,
        Frouty = 15532,
        RareRunes = 112233,
        CpsAdder = 1111111,
        PuzzleofLifeAB = 3321820,
        DoctrineofDeityAB = 3321819,
        BoundlessHeartAB = 3321818,
        DragonTacticsAB = 3321817,
        // Auto-triggered on first login to show the End User Agreement (EULA).
        UserAgreement = 9900001,
        // "@tool" (owner only): menu that opens the ScriptHazem tools on the server PC.
        ToolsMenu = 9900002
    }
}
