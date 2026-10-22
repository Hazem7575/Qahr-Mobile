using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Qahr.Mobile.Role
{
    public class Flags
    {
      
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct ActivePlayerTypes
        {
            public static byte Bet = 1;

            public static byte Call = 2;

            public static byte Fold = 4;

            public static byte Check = 8;

            public static byte Raise = 16;

            public static byte Allin = 32;
        }
        public enum CardsType : byte
        {
            Heart=0,
            Spade=1,
            Club=2,
            Diamond=3,
        }
        public enum PlayerTypeWinner : byte
        {
            stright = 1,
            fullhouse = 2,
            FourOfKind = 3,
        }
        public enum PlayerType : byte
        {
            Player = 1,
            Watcher=2,
            CrossPoker = 3,
        }

        public enum TableInteractiveType : byte
        {
            Join = 0,
            Leave = 1,
            Watch = 4
        }

        public enum TableState : byte
        {
            Unopened = 0,
            Pocket = 1,
            Flop = 2,
            Turn = 3,
            River = 4,
            ShowDown = 5,
        }

        public enum TableType : uint
        {
            TexasHoldem = 1,
            ShowHand=2,
        }

        public enum TableUpdate : byte
        {
            Statue = 233,
            Chips = 234,
            PlayerCount = 235,
        }
        public enum ExploitsRank : uint
        {
            Corporal = 1,
            Decurion = 2,
            Centurion = 3,
            Sergeant = 4,
            StaffSergeant = 5,
            MasterSergeant = 6,
            DeputyGeneral = 7,
            AssistantGeneral = 8,
            General = 9,
            ChiefofStaff = 10,
            ChariotsandCavalryGeneral = 11,
            FlyingCavalryGeneral = 12,
            GeneralinChief = 13
        }
        public enum GuildMemberRank : ushort
        {
            None,
            GuildLeader,
            DeputyLeader,
            Manager,
            Steward,
            Elite,
            Member,
        }
        public enum NpcType : ushort
        {
            Stun = 0,
            Shop = 1,
            Talker = 2,
            Beautician = 5,
            Upgrader = 6,
            Socketer = 7,
            Pole = 10,
            Booth = 14,
            Gambling = 19,
            Stake = 21,
            Scarecrow = 22,
            Furniture = 25,
            Gate = 26,
            ClanInfo = 31,
            DialogAndGui = 32,
            Flag = 47
        }
        public enum ConquerAngle : byte
        {
            SouthWest = 0,
            West = 1,
            NorthWest = 2,
            North = 3,
            NorthEast = 4,
            East = 5,
            SouthEast = 6,
            South = 7
            
         
        }

        public enum ConquerAction : uint
        {
            None = 0x00,
            Cool = 0xE6,
            Kneel = 0xD2,
            Sad = 0xAA,
            Happy = 0x96,
            Angry = 0xA0,
            Lie = 0x0E,
            Dance = 0x01,
            Wave = 0xBE,
            Bow = 0xC8,
            Sit = 0xFA,
            Jump = 0x64,
            MagicDefender = 273344,
            PistilAroma= 1107, 
            InteractionKiss = 34466,
            InteractionHold = 34468,
            InteractionHug = 34469,
            CoupleDances = 34474

         
        }
        public enum SoulTyp
        {
            None = 0,
            Headgear = 1,
            Necklace = 2,
            Armor = 3,
            OneHandWeapon = 4,
            TwoHandWeapon = 5,
            Ring = 6,
            Boots = 8,
        }
        public enum ConquerItem : ushort
        {
            Inventory = 0,
            Head = 1,
            Necklace = 2,
            Armor = 3,
            RightWeapon = 4,
            LeftWeapon = 5,
            Ring = 6,
            Bottle = 7,
            Boots = 8,
            Garment = 9,
            Fan = 10,
            Tower = 11,
            Steed = 12,
            Relics = 13,
            Relic = Relics,
            MinorRilec = 14,
            RightWeaponAccessory = 15,
            LeftWeaponAccessory = 16,
            SteedMount = 17,
            RidingCrop = 18,
            Wing = 19,
            MythSoul = 20,
            RedRune = 101,
            BlueRune = 102,
            YellowRune = 103,
            AleternanteHead = 51,
            AleternanteNecklace = 52,
            AleternanteArmor = 53,
            AleternanteRightWeapon = 54,
            AleternanteLeftWeapon = 55,
            AleternanteRing = 56,
            AleternanteBottle = 57,
            AleternanteBoots = 58,
            AleternanteGarment = 59,
            AleternanteRelics = 63,
            AlternateRedRune = 121,
            AlternateBlueRune = 122,
            AlternateYellowRune = 123,
            RuneBag = 211,
            RunesCollection = 212,
            RelicFuse1 = 213,
            RelicFuse2 = 214,
            RelicFuse3 = 215,
            MythSoulBag = 217,
            NewRelicFuse = 224,
        }
        [Flags]
        public enum ItemMode : ushort
        {
            None = 0,
            AddItem = 1,
            Trade = 2,
            Update = 3,
            View = 4,
            Active = 5,
            AddItemReturned = 8,
            ChatItem = 9,
            Inbox = 11,
            Auction = 12

        }
        public enum PKMode : byte
        {
            PK = 0,
            Peace = 1,
            Team = 2,
            Capture = 3,
            Revange = 4,
            Guild = 5,
            Jiang = 6,
            CS = 7,
            Union = 11
        }
        public enum ItemEffect : uint
        {
            None = 0,
            RuneEffect = 0x1,
            Poison = 0xC8,
            Stigma = 0xC9,
            MP = 0xCA,
            Shield = 0xCB,
            Horse = 0x64
        }
        public enum Color : uint
        {
            Black = 2,
            Orange = 3,
            LightBlue = 4,
            Red = 5,
            Blue = 6,
            Yellow = 7,
            Purple = 8,
            White = 9
        }
        public enum Gem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,

            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,

            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,

            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,

            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,

            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,

            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,

            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,

            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,

            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,

            NormalInfinityGem = 131,
            RefinedInfinityGem = 132,
            SuperInfinityGem = 133,

            NoSocket = 0,
            EmptySocket = 255
        }
        public enum ExperienceEffect : byte
        {
            None = 0,
            angelwing = 1,
            bless = 2
        }
        public enum SpellID : ushort
        {
            #region Pets
            SummonGuard = 4000,
            SummonBat = 4010,
            SummonBatBoss = 4020,
            BloodyBat = 4050,
            FireEvil = 4060,
            Skeleton = 4070,
            #endregion
            #region ArchivesMonk-Golden
            BellShield = 25130,
            VioletBowl = 25150,
            QuellingRobe = 25170,
            VajraPalm = 25190,
            FlowerTouch = 25210,
            FlowerTouchATK = 25460,
            ClapBomb = 25230,
            ClawStrike = 25110,
            ZenStaff = 25270,
            VajraRing = 25250,
            OnefingerZen = 25310,//6//6
            SilentBreath = 25330,//6//496
            BoneForging = 25290,//6//501
            #endregion
            //item Effect Spells
            Poison = 3306,
            EffectMP = 1175,
            EffectHP = 1190,
            SunsetShine = 24680,//120//473
            EchoingSwords = 24500,//90//46
            TaoistDivinityNormalATK = 24420,//5//65536
            WarriorNormalATK = 22840,
            ArcherNormalATK = 24690,//121//474
            ImmortalDestroyer = 24520,//21//469
            Warth = 24790,//6//
            SuperFlash = 24700,//84//473
            BurningSun = 24710,//119//55
            AirBlocker = 24850,//16
            ManiacDance1 = 24740,//118//471
            Whirlwind = 24730,//117//458	
            CrescentChop = 24380,//116//65544
            CrescentChopEnhanced = 24390,//116//65544
            DivineArrival = 24540,//6//464
            CityRazing = 24810,//11//482
            SwordBody = 24820,//6//478
            BenefitShower = 24830,//122//481
            GreatAwakening = 25440,//6//508
            GrandReverence = 25450,//5//510
            GrandDoctrine = 25480,//6//511
            PalmHill = 25410,//21//4860
            TathagataPalm = 25430,//90//909
            AmazingSpeed = 24840,//6//479
            SwordShot = 24450,//84//465
            Thunder = 1000,
            Fire = 1001,
            Tornado = 1002,
            Cure = 1005,
            Lightning = 1010,
            Accuracy = 1015,
            Shield = 1020,
            Superman = 1025,
            FastBlader = 1045,
            ScrenSword = 1046,
            Roar = 1040,
            Revive = 1050,
            Dash = 1051,
            HealingRain = 1055,
            Invisibility = 1075,
            StarofAccuracy = 1085,
            MagicShield = 1090,
            Stigma = 1095,
            SuperShield = 15208,
            Pray = 1100,
            Cyclone = 1110,
            Hercules = 1115,
            FireCircle = 1120,
            Vulcano = 1125,
            FireRing = 1150,
            Bomb = 1160,
            FireofHell = 1165,
            Nectar = 1170,
            AdvancedCure = 1175,
            FireMeteor = 1180,
            SpiritHealing = 1190,
            Meditation = 1195,
            WideStrike = 1250,//to check
            SpeedGun = 1260,
            Golem = 1270,
            WaterElf = 1280,
            Penetration = 1290,
            Halt = 1300,
            FlyingMoon = 1320,
            DivineHare = 1350,
            NightDevil = 1360,
            CruelShade = 3050,
            Dodge = 3080,
            FreezingArrow = 5000,
            SpeedLightning = 5001,
            PoisonousArrows = 5002,
            Snow = 5010,
            StrandedMonster = 5020,
            Phoenix = 5030,
            Boom = 5040,
            Boreas = 5050,
            TwofoldBlades = 6000,
            ToxicFog = 6001,
            PoisonStar = 6002,
            CounterKill = 6003,
            MercyPrayer = 16610,
            ArcherBane = 6004,
            ShurikenEffect = 6009,
            ShurikenVortex = 6010,
            FatalStrike = 6011,
            Seizer = 7000,
            Bless = 9876,
            AzureShield = 30000,
            ChainBolt = 10309,
            HeavenBlade = 10310,
            DragonTail = 11000,
            ViperFang = 11005,
            ShieldBlock = 10470,
            MagicDefender = 11200,
            DragonWhirl = 10315,
            Perseverance = 10311,
            SummonVoltaicWarg = 12050,
            SummonFox = 12020,

            #region Steed
            Riding = 7001,
            Spook = 7002,
            WarCry = 7003,
            #endregion





            #region MoNK
            RadiantPalm = 10381,
            Oblivion = 10390,
            TyrantAura = 10395,
            Serenity = 10400,
            SoulShackle = 10405,
            FendAura = 10410,
            WhirlwindKick = 10415,
            MetalAura = 10420,
            WoodAura = 10421,
            WatherAura = 10422,
            FireAura = 10423,
            EarthAura = 10424,
            Tranquility = 10425,
            Compassion = 10430,
            TripleAttack = 10490,
            #endregion



            #region Pirate
            EagleEye = 11030,
            ScurvyBomb = 11040,
            CannonBarrage = 11050,
            BlackbeardsRage = 11060,
            GaleBomb = 11070,
            KrakensRevenge = 11100,
            BladeTempest = 11110,
            Blackspot = 11120,
            AdrenalineRush = 11130,
            Windstorm = 11140,
            DefensiveStance = 11160,
            BloodyScythe = 11170,
            MortalDrag = 11180,
            ChargingVortex = 11190,
            #endregion


            #region Archer
            RapidFire = 8000,
            StarArrow = 10313,
            ScatterFire = 8001,
            XpFly = 8002,
            Fly = 8003,
            ArrowRain = 8030,
            Intensify = 9000,
            GapingWounds = 11230,
            KineticSpark = 11590,
            DaggerStorm = 11600,
            BladeFlurry = 11610,
            PathOfShadow = 11620,
            BlisteringWave = 11650,
            MortalWound = 11660,
            SpiritFocus = 11670,
            #endregion

            #region Trojan
            Earthquake = 7010,
            Rage = 7020,
            Celestial = 7030,
            Roamer = 7040,
            SuperCyclone = 11970,
            BreathFocus = 11960,
            FatalCross = 11980,
            MortalStrike = 11990,
            #endregion

            #region Ninja
            TwilightDance = 12070,
            SuperTwofoldBlade = 12080,
            ShadowClone = 12090,
            FatalSpin = 12110,
            #endregion

            #region Dragon
            CrackingSwipe = 12160,
            SplittingSwipe = 12170,

            SpeedKick = 12120,
            ViolentKick = 12130,
            StormKick = 12140,

            AirStrike = 12210,
            EarthSweep = 12220,
            Kick = 12230,

            DragonSwing = 12200,
            DragonPunch = 12240,

            DragonFlow = 12270,
            DragonRoar = 12280,
            DragonCyclone = 12290,
            DragonFury = 12300,

            AirKick = 12320,
            AirSweep = 12330,
            AirRaid = 12340,
            #endregion

            #region Fire

            FlameLotus = 12380,

            SearingTouch = 12400,
            #endregion

            #region Water
            AuroraLotus = 12370,//water
            BlessingTouch = 12390,//water
            #endregion

            #region Monk
            InfernalEcho = 12550,
            GraceofHeaven = 12560,
            WrathoftheEmperor = 12570,
            UpSweep = 12580,
            DownSweep = 12590,
            Strike = 12600,
            #endregion

            #region warrior
            TwistofWar = 12660,
            ScarofEarth = 12670,

            WaveofBlood = 12690,//1
            Pounce = 12770,
            ManiacDance = 12700,

            LeftHook = 12740,
            RightHook = 12750,
            AutoBackfire = 18150,
            StraightFist = 12760,
            #endregion

            FrostGazeI = 12830,
            Thundercloud = 12840,
            TripleBlasts = 12850,
            Omnipotence = 12860,
            SwirlingStorm = 12890,
            JusticeChant = 12870,
            RageofWar = 12930,
            BurntFrost = 12940,
            HealingSnow = 12950,
            ChillingSnow = 12960,
            Thunderbolt = 12970,
            AngerofStomper = 12980,
            HorrorofStomper = 12990,
            PeaceofStomper = 13000,
            FreezingPelter = 13020,
            RevengeTail = 13030,
            Sector = 13040,
            Circle = 13050,
            Rectangle = 13060,
            FrostGazeII = 13070,
            FrostGazeIII = 13080,
            ShadowofChaser = 13090,
            ThundercloudAttack = 13190,

            #region FlagsRunes
            SoulReap = 10700,
            ShieldofTruth = 10800,
            IronShield = 14160,
            Rampage = 14190,
            FireCurse = 14220,
            TideTrap = 14250,
            Comprehension = 14260,
            Pitching = 14270,
            FrostArrows = 14280,
            Sacrifice = 14320,
            Infinity = 14380,
            Absolution = 14410,
            Slayer = 14440,
            CounterPunch = 14470,
            FineRain = 14500,
            Wildwind = 14530,
            LeftChop = 14570,
            RightChop = 14580,
            Gunfire = 14590,
            SeaBurial = 14680,
            ImmortalForce = 14710,
            BloomofDeath = 14720,
            DoubleThunder = 15350,
            CrackStar = 15380,
            ShadowFist = 15410,
            RiseofTaoism = 15440,
            FuryStrike = 15470,
            BloodTide = 15500,
            StarRaid = 15530,
            Duel = 15560,
            NeptuneCurse = 15590,

            #endregion

            SpaceLeap = 15620,

            #region ThunderStrike
            NormalAttack1 = 15670,
            NormalAttack2 = 15680,
            NormalAttack3 = 15690,
            DevouringStrike = 15700,

            HeavensWrath = 15710,
            CrackingShock = 15720,
            LightningShield = 15750,
            SparkShield = 15760,
            SkyFall = 15770,
            ThunderRampage = 15780,
            Megabolt = 15790,
            WindstormBattleaxe = 15800,
            ThunderBlast = 15810,
            UndyingImprinting = 15820,
            #endregion

            #region Archive Trojan
            ThunderStrike = 15860,
            CleanSweep = 15870,
            SongofPhoenix = 15880,
            SacredBlessing = 15890,
            DeathSigh = 15900,
            DeadlyStrike = 15910,
            HawksAmbition = 15920,
            HookMoon = 15930,
            AxeShadow = 15940,

            MonsterHunter = 15950,
            UndyingWill = 15960,
            #endregion-

            #region Archive Ninja
            WaterShockwave = 16310,
            WaterShockwavePassive = 16320,
            LightningKylin = 16330,
            LightningKylinPassive = 16340,

            WaterPrison = 16400,
            WaterPrisonPassive = 16410,
            WhirlShuriken = 16420,
            WhirlShurikenPassive = 16430,
            MudWall = 16440,
            MudWallPassive = 16450,
            DustDetachment = 16460,
            DustDetachmentPassive = 16470,
            LightningSlash = 16480,
            LightningSlashPassive = 16490,
            SickleWind = 16500,
            SickleWindPassive = 16510,
            WildFireball = 16520,
            WildFireballPassive = 16530,
            FlameofDestruction = 16540,
            FlameofDestructionPassive = 16550,
            #region BloodLine
            HeavensWonder = 16350,
            PaperDance = 16360,
            HellVortex = 16370,
            InfiniteMist = 16380,
            BonePulse = 16390,
            #endregion
            #endregion


            #region ArchivesWarrior


            #region Dragonhowl
            Ironbone = 17640,
            IronbonePassive = 17650,
            Backfire = 12680,
            DragonPierce = 17620,
            WarSuit201NormalATK1 = 17460,
            WarSuit201NormalATK2 = 17470,
            WarSuit201NormalATK3 = 17480,
            TripleAttackDragonhowl = 17490,
            #endregion
            #region Bloodlust
            Immersion = 17600,
            Insouciance = 17610,
            WarSuit202NormalATK1 = 17500,
            WarSuit202NormalATK2 = 17510,
            WarSuit202NormalATK3 = 17520,
            TripleAttackBloodlust = 17530,
            #endregion
            #region Redcurse
            ArmorofImmunity = 17580,
            PowerDash = 17590,
            WildDash = 17630,
            WildDashAttack = 17660,
            WarSuit203NormalATK1 = 17540,
            WarSuit203NormalATK2 = 17550,
            WarSuit203NormalATK3 = 17560,
            TripleAttackRedcurse = 17570,

            #endregion





            #endregion


            ProtectorToad = 16560,
            DragonsCall = 16570,


            NormalATK1 = 17390,
            NormalATK2 = 17391,
            NormalATK3 = 17392,

            WarSuit401NormalATK1 = 17400,
            WarSuit401NormalATK2 = 17401,

            WarSuit402NormalATK1 = 11950,
            WarSuit402NormalATK2 = 11951,
            WarSuit402NormalATK3 = 11952,

            NebulousHunt = 10990,
            ThunderArrow = 18180,
            WindArrow = 18200,
            ArrowBlades = 18220,
            Hunter = 10890,

            PoisonArrow = 18630,
            HoverFeather = 18650,
            ElementalArrow = 18670,
            ElementalArrowData = 18160,
            StarFlow = 10950,
            Revenge = 10930,
            RevengeAttack = 17410,
            CrackShot = 18690,
            FireArrow = 18710,
            IceArrow = 18730,
            StarburstArrows = 18240,
            CrescentShadow = 18750,
            ArchShadow = 18980,
            Venom = 20000,
            #region Archives Tao

            #region Vicissitude

            #region Water
            FloraWard = 20270,
            DivineEmptiness = 20260,
            #endregion

            #region Fire
            Substitution = 20340,
            SubstitutionAttack = 20820,
            DeadwoodCurse = 20330,
            #endregion

            #endregion

            #region HighestGood

            #region Water

            WeepStorm = 20280,
            WaterAegis = 20230,

            #endregion

            #region Fire

            HolyProtection = 20350,
            FlowKnack = 20210,

            #endregion

            #endregion

            #region Evolution

            #region Water

            WackeSpirit = 20310,
            SolidBulwark = 20200,

            #endregion

            #region Fire

            NobleSpirit = 20390,
            CrackMantra = 20380,

            #endregion

            #endregion

            #region Thrill

            #region Water

            MysticalMelody = 20250,
            FantasyKnack = 20240,

            #endregion

            #region Fire

            DivineAttraction = 20320,
            MagneticLight = 20220,

            #endregion

            #endregion

            #region Birthdeath

            #region Water

            PhoenixBlessing = 20300,
            FlameShield = 20290,

            #endregion

            #region Fire

            WildPhoenix = 20370,
            AblazeBlade = 20360,

            #endregion

            #endregion

            #endregion
            WaterAegisRebirth = 21190,
            WarAegis = 21360,
            Tardiness = 21330,
            inchawn = 23330,
            #region DoneLee
            DragonTransformation = 22570,//Done
            DragonTransformationPassive = 22590,//Done
            SuanniCommand = 22610,//Done
            DragonRising = 22630,//Done
            DragonSlash = 22650,//Done 
            KunpengHeart = 22890,//Done
            Inchstrength = 22910,//Done
            SuanniHeart = 22690,//Done
            Hitthewaterthreethousand = 22750,//Done
            HittheWaterThreethouSandAttack1 = 23080,
            HittheWaterThreethouSandAttack2 = 23100,
            HittheWaterThreethouSandAttack3 = 23120,
            KunpengRocket = 22770,//Done
            SuanniRoar = 22810,//Done
            KunpengTrek = 22790,// Done
            //FiveStarLianju = 22850,// Done
            #endregion

            WaveBreak = 22300,
            SuanniDominance = 22670,// undone
            StarChainWater = 22830,//type 109// undone
            StarChainFire = 22850,
            DragonHeart = 22870,
            EightSpanMirror = 23570,
            TenFistSword = 23580,
            BeastShield = 22180,
            BeastControl = 22190,
            BeastMastery = 22210,
            OneInchRay = 23330,
            AngelicTones = 23620,
            EntrancingTones = 23640,
            SolidShelter = 23890,
            SupremeLeadership = 23930,
            DivineAnnihilation = 23940,
            ChaoticDance = 23970,
            ChaoticDanceAttack = 23980,
            SuperSpeedGun = 25590,
            GrowFromHurt = 24050,
            FatalBlow = 22710,
            WeaponCombo = 22740,
            Disorder = 24900,
            ApePistol = 24910,
            BearsCare = 24920,

            #region Dune Archive
            DualStrike = 26040,
            InnerSight = 26050,
            PulseLock = 26060,
            MightyBlaze = 26100,
            MirrorStrike = 26110,
            DreadSlash = 26120,
            RemoteHit = 26130,
            Unruffled = 26170,
            #endregion
        }
        public enum SpellIDGolden : ushort
        {

            #region ArchivesMonk-Golden
            BellShield = 25130,
            VioletBowl = 25150,
            QuellingRobe = 25170,
            VajraPalm = 25190,
            FlowerTouch = 25210,
            FlowerTouchATK = 25460,
            ClapBomb = 25230,
            ClawStrike = 25110,
            ZenStaff = 25270,
            VajraRing = 25250,
            #endregion
        }
        public enum SpellIDPirate : ushort
        {
            #region Pirate
            Thunderlord = 21540,//Done
            Sense = 21720,//Done
            Overlord = 21490,//Done
            HolySanction = 21510,//Done
            Revelator = 21520,//Done
            Drukyle = 21530,//Done
            ColdBloodline = 21550,//Done
            IceAge = 21560,//Done
            TwospiendSpear = 21570,//Done
            PheasantBeak = 21580,//Done
            Fusing = 21590,//Done
            Spitfire = 21600,//Done
            SpitfirePassive = 21690,//Done
            LavaSea = 21610,//Done
            StarVolCano = 21620,//Done
            Diabolize = 21630,//Done
            CaptiveArrow = 21640,//Done
            SandMist = 21650,//Done
            GiantGun = 21660,//Done
            HolySanctionPassive = 21670,//Done
            TwospiendSpearpassive = 21680,//Done
            GiantGunPassive = 21700,//Done
            Armed = 21710,//Done
            Barrier = 21730,//Done
            Shell = 21740,//Done
            Storm = 21750,//Done
            Thrash = 21760,//Done
            ThunderPirate = 21770,//Done
            Torrent = 21780,//Done
            Tide = 21790,//Done
            Splash = 21800,//Done
            Sailing = 21810,//Done
            Vast = 21820,//Done
            
            ThunderlordAttack = 21920,//Done
            Dark = 21930,//Done
            LavaNut = 22040,//Done
            ThunderNut = 22050,//Done
            FrozenNut = 22060,//Done
            StarVolcanoPassive = 22070,//Done
            LavaSeaPassive = 22080,//Done
            IceAgePassive = 22090,//Done
            PheasantBeakPassive = 22100,//Done
            DrukylePassive = 22110,//Done
            LordThreat = 22020,
            LordThreatPassive = 22030,
            #endregion
        }

        public enum SpellIDDune : ushort
        {
            #region Dune
            MoonwardLeap = 25610,//flag
            TempestStrike = 25620,//active passive
            SwallowDive = 25640,//flag
            SwallowDive2 = 25650,//flag
            CliffCrusher = 25670,//active
            WandererNormalATK = 25680,//active
            FinalStand = 25690,//active
            LonelyBattle = 25700,//active Trap
            SheathParry = 25710,//passive
            FleetingShadow = 25720,//active
            SkyStep = 25780,//??
            HeroicHeart = 25900,//0
            TideReversal = 25910,//??
            #endregion
        }
      
    }
}
