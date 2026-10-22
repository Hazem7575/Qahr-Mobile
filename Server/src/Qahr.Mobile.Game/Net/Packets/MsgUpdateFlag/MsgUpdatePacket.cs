using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    [ProtoContract]
    public class NewUpdateProto
    {
        [ProtoMember(1, IsRequired = true)]
        public uint UID;
        [ProtoMember(2, IsRequired = true)]
        public List<NewUpdateQuery> Updates = new List<NewUpdateQuery>();
        [ProtoContract]
        public class NewUpdateQuery
        {
            [ProtoMember(1)]
            public MsgUpdate.DataType Type;
            [ProtoMember(2)]
            public ulong Value;
            [ProtoMember(3)]
            public ulong[] Values;
            [ProtoMember(4)]
            public ulong Flag;
            [ProtoMember(5)]
            public ulong Time;
            [ProtoMember(6)]
            public ulong Damage;
            [ProtoMember(7)]
            public ulong SpellLevel;
            [ProtoMember(8)]
            public ulong RestoringHP;
            [ProtoMember(9)]
            public ulong ManaRecoveryTime;
        }
        public ServerSockets.Packet ToArray()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(this);
                stream.Finalize((ushort)GamePackets.MsgUserAttrib);
                return stream;
            }
        }
        public static implicit operator ServerSockets.Packet(NewUpdateProto obj)
        {
            return obj.ToArray();
        }
    }
    public unsafe static partial class MsgBuilder
    {
        [ProtoContract]
        public class UpdateProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2)]
            public UpdateQuery[] Updates;
        }
        [ProtoContract]
        public class UpdateQuery
        {
            [ProtoMember(1)]
            public MsgUpdate.DataType Type;
            [ProtoMember(2)]
            public ulong Value;
            [ProtoMember(3)]
            public ulong[] Values;
            [ProtoMember(4)]
            public uint Flag;
            [ProtoMember(5)]
            public uint Time;
            [ProtoMember(6)]
            public int Damage;
            [ProtoMember(7)]
            public int SpellLevel;
            [ProtoMember(8)]
            public long Damage2;
            [ProtoMember(9)]
            public long SpellLevel2;
        }
        public static void GetUpdatePacket(this ServerSockets.Packet stream, out MsgUpdate.DataType ID, out ulong Value)
        {
            ID = MsgUpdate.DataType.Hitpoints;
            Value = 0;
            UpdateProto proto = new UpdateProto();
            stream.ProtoBufferDeserialize<UpdateProto>(proto);
            if (proto.Updates[0] != null)
            {
                ID = proto.Updates[0].Type;
                Value = proto.Updates[0].Value;
            }
        }
    }


    public unsafe class MsgUpdate
    {
        public MsgBuilder.UpdateProto Info;
        public List<MsgBuilder.UpdateQuery> myUpdates;
        public class OnlineTraining
        {
            public const byte
            Show = 0,
            InTraining = 1,
            Review = 2,
            IncreasePoints = 3,
            ReceiveExperience = 4,
            Remove = 5;
        }
        public class CreditGifts
        {
            public const byte
                Show = 0,
                CanClaim = 1,
                Claim = 5,
                ShowSpecialItems = 6;
        }
        [Flags]
        public enum Flags : int
        {
            Normal = 3,//0x0,
            FlashingName = 0,
            Poisoned = 1,
            Invisible = 2,
            XPList = 4,
            Dead = 5,
            TeamLeader = 6,
            StarOfAccuracy = 7,
            MagicShield = 8,
            Shield = 8,
            Stigma = 9,
            Ghost = 10,
            FadeAway = 11,
            RedName = 14,
            BlackName = 15,
            ReflectMelee = 17,
            Superman = 18,
            Ball = 19,
            RageofWar = 20,
            Focused = 21,
            Invisibility = 22,
            Cyclone = 23,
            Dodge = 26,
            Fly = 27,
            Intensify = 28,
            CastPray = 30,
            Praying = 31,
            Cursed = 32,
            HeavenBlessing = 33,
            TopGuildLeader = 34,
            TopDeputyLeader = 35,
            MonthlyPKChampion = 36,
            WeeklyPKChampion = 37,
            TopWarrior = 38,
            TopTrojan = 39,
            TopArcher = 40,
            TopWaterTaoist = 41,
            TopFireTaoist = 42,
            TopNinja = 43,
            ShurikenVortex = 46,
            FatalStrike = 47,
            Flashy = 48,
            Ride = 50,
            TopSpouse = 51,
            Accelerated = 52,
            Deceleration = 53,
            Frightened = 54,
            HeavenSparkle = 55,
            IncMoveSpeed = 56,
            GodlyShield = 57,
            Dizzy = 58,
            Stun = 58,
            Freeze = 59,
            Confused = 60,
            Top8Weekly = 63,
            Top4Weekly = 64,
            Top2Weekly = 65,
            ChaintBolt = 92,
            AzureShield = 93,
            ScurvyBomb = 96,//that is use for abuse.
            TyrantAura = 98,
            FendAura = 100,
            MetalAura = 102,
            WoodAura = 104,
            WaterAura = 106,
            FireAura = 108,
            EartAura = 110,
            SoulShackle = 111,
            Oblivion = 112,
            ShieldBlock = 113,
            TopMonk = 114,
            TopPirate = 122,
            CTF_Flag = 118,
            PoisonStar = 119,
            CannonBarrage = 120,
            BlackbeardsRage = 121,
            DefensiveStance = 126,
            MagicDefender = 128,
            RemoveName = 129,
            PurpleBall = 131,
            BlueBall = 132,
            PathOfShadow = 145,
            BladeFlurry = 146,
            KineticSpark = 147,
            AutoHunt = 148,
            SuperCyclone = 150,


            TopSuperGuildWarFiveStars = 151,
            TopSuperGuildWarThreeStars = 152,
            TopSuperGuildWarOneStar = 153,
            rygh_hglx = 174,//top
            rygh_syzs = 175,//top
            bdeltoid_cyc = 205,
            _p_6_targst = 206,
            rygh_hglx1 = 207,
            GoldBrickNormal = 161,
            GoldBrickRefined = 162,
            GoldBrickUnique = 163,
            GoldBrickElite = 164,

            DragonFlow = 148,//20

            TopDragonLee = 154,////26
            DragonFury = 158,//30
            DragonCyclone = 159,//31
            DragonSwing = 160,//32
            Goldbrick = 165,
            TopMrConquer = 166,
            TopMrsConquer = 167,
            lianhuaran01 = 168,
            lianhuaran02 = 169,
            lianhuaran03 = 170,
            lianhuaran04 = 171,
            FullPowerWater = 172,
            FullPowerFire = 173,
            ShieldBreak = 176, // 20% at change
            DivineGuard = 177,
            Backfire = 179,
            ScarofEarth = 180,
            ManiacDance = 181,
            Pounce = 182,

            Omnipotence = 192,
            Bleed = 193,
            WindWalkerFan = 194,

            HealingSnow = 196,
            ChillingSnow = 197,
            xChillingSnow = 198,
            Thunderbolt = 199,
            FreezingPelter = 200,
            xFreezingPelter = 201,
            RevengeTail = 202,
            TopWindWalker = 203,
            ShadowofChaser = 204,

            Sacrifice = 211,
            FineRain1 = 212,
            FineRain2 = 213,
            Rampage = 214,
            Infinity = 215,
            Absolution = 216,
            IronShield = 217,
            Slayer = 218,
            FrostArrows = 219,
            ImmortalForce = 220,
            NeptuneCurse = 222,
            PortadorRuneDuel = 223,
            SeparacionDuel = 224,
            Duel = 225,
            RiseofTaoism = 227,
            IronGuard = 228,
            BloodTide = 229,
            CrackStar = 230,
            CrackStarNegative = 231,
            TopThunderstriker = 232,
            AttackUp = 233,
            UndyingImprinting = 234,
            ThunderRampage = 235,
            HeavensWrath = 236,
            LightningShieldActivated = 237,
            SparkShieldActivated = 238,
            LightningShield = 240,
            SparkShield = 241,
            DevouringStrike = 242,
            Overwhelm = 243,
            TidalWave = 245,
            Quench = 246,
            RisingPhoenix = 247,
            SoulChain = 248,
            ActiveWeapon = 249,
            CelestialDance = 250,
            AxeShadow = 251,
            DeathSigh = 253,
            CleanSweep = 254,
            SacredBlessing = 255,
            HawksAmbition = 256,
            SongofPhoenix = 257,
            DeadlyStrike = 258,
            HookMoon = 259,
            HookMoonCaster = 260,//Fake
            ShadowFist = 261,
            CounterPunch = 262,
            FireCurse = 263,
            StarRaid = 264,
            FlashShield = 265,
            FuryStrike = 266,
            SpaceLeap = 267,
            Retaliation = 268,
            //Archive Ninja
            Overall = 267,
            Fire = 268,
            Water = 269,
            Earth = 270,
            Wind = 271,
            Lightning = 272,
            SageMode = 273,
            MudWall = 274,
            Flame_Sigil_Rage = 280,
            HellVortex = 281,
            InfiniteMist = 282,
            SlashSeal = 283,
            WildSigilBurning = 284,
            PrisonSigilFutility = 285,
            WhirlSigilAura = 286,
            BonePulse = 287,
            PaperDance = 288,
            HeavensWonder = 289,
            LotusDemon = 291,
            SierraBeast = 292,
            ShieldofTruth = 297,
            SoulReap = 300,
            Immersion = 303,
            Insouciance = 304,
            WildDash = 305,
            DragonPierceCrit = 307,
            DragonPierceTortoise = 308,
            DragonPierceBreak = 309,
            DragonPierceStamina = 310,
            ArmorOfImmunity = 311,
            Return = 312,
            DisableBlock = 313,
            RuneShackle = 314,
            DrainingHP = 315,
            FireArrow = 317,
            IceArrow = 318,
            PoisonArrow = 319,
            ThunderArrow = 320,
            WindArrow = 321,
            IntensifyStatus = 329,
            AttackArrowBlades = 332,
            ActiveArrowBlades = 333,
            Revenge = 334,
            DefenseDecreasing = 335,
            StarFlowEx = 336,
            StarFlow = 337,
            NoXp = 338,
            WindElementEffect = 339,
            Hunter = 344,
            ArchShadow = 346,
            StoneCracker = 352,
            HoverFather = 353,
            VenomMyth = 355,
            Solid = 356,
            FlowKnack = 361,
            MagneticLight = 363,
            SolidBulwark = 364,
            Substitution = 365,
            SubstitutionAttack = 366,
            WeepStorm = 367,
            WildPhoenix = 368,
            FloraWard = 370,
            NobleSpirit = 372,
            WackeSpirit = 374,
            HolyProtection = 376,
            FantasyKnack = 377,
            CrackMantra1 = 378,
            CrackMantra2 = 379,
            FlameShield = 380,
            DivineEmptiness = 381,
            DeadwoodCurse = 382,
            PhoenixBlessing = 383,
            CrackMantra = 384,
            FrontBreak = 385,
            SuperPower = 386,
            Oracle = 387,
            FrostPostion = 388,
            FrostStop = 389,
            Bash = 390,
            Corrosion = 391,
            Numb = 392,
            Luck = 393,
            Diabolize = 394,
            SandMist = 395,
            CaptiveArrow = 396,
            Barrier = 397,
            Vast = 398,
            Tide = 399,
            Torrent = 400,
            ThunderPirate = 401,
            Storm = 402,
            Thrash = 403,
            ThunderLord = 404,
            ThunderLordActive = 405,
            Splash = 406,
            Shell = 407,
            Revelator = 408,
            Fusing = 409,
            ColdBloodline = 410,
            Tardiness = 411,
            Sense = 412,
            Armed = 413,
            Dark = 417,
            Sailing = 419,
            BeastMastery = 420,
            WaveBreak = 422,
            StarChainFire = 425,
            StarChainWater = 426,
            StarChainWaterInvisable = 427,
            SuanNiHeart = 428,
            Commanding = 429,
            DivingDragon = 430,
            Kunpeng_Soaring = 431,
            DragonShift = 433,
            KunpengHeart = 434,
            KunpengHurricane = 435,
            IgnoresDamageReductionCausedByStrengthDifference = 437,
            Spreadyourwings = 435,
            EntrancingTones = 439,
            DivineAnnihilation = 440,
            SupremeLeadership = 441,
            KungFuPanda = 442,
            BlazeGaze = 443,
            SkySoarer = 444,
            Riposte = 445,
            ChaoticDance = 446,
            Unk1 = 447,
            Unk2 = 448,
            RocketBoost = 450,
            DropMonster = 452,
            VIPProtection = 453,
            Unk3 = 454,
            FearlessFighter = 456,
            GrowFromHurt = 457,
            HurricaneSweep = 458,
            Whirlwind = 458,
            SpeactatorCheer = 459,
            HarshTune = 463,
            SwordShot = 465,
            EchoingSwords = 466,
            Unk4 = 468,
            ImmortalDestroyer = 469,
            Unk5 = 470,
            Unk6 = 471,
            ArcherNormalATK = 474,
            ManiacDance1 = 471,
            SunsetShine = 473,
            Yuanshen = 464,
            DivineArrival = 477,
            SwordBody = 478,
            AmazingSpeed = 479,
            AmazingSpeedActive = 480,
            BenefitShower = 481,
            CityRazing = 482,
            Disorder = 486,
            ApePistol = 487,
            BearsCare = 489,
            VajraRing = 505,
            ZenStaff = 503,
            QuellingRobe = 504,
            FlowerTouch = 506,
            VioletBowl = 502,
            BoneForging = 501,
            SilentBreath = 496,
            BellShield = 497,
            ClawStrike = 495,
            GreatAwakening = 508,
            GrandReverence = 510,
            GrandDoctrine = 511,
            TathagataPalm = 909,
            LoveForEver = 492,
            HeartLock = 513,
            SuperSpeedGun = 529,
            MoonwardLeap = 530,
            SkyStep = 532,
            SwallowDive = 533,
            FleetingShadow = 534,
            SheathParry = 535,
            SkyStepFlag = 536,
            SkyStepFlag2 = 537,
            BloodThirst = 539,
            Dune = 25,
            ConceptionVessal = 554,
            MirrorStrike = 555,
            DreadSlash = 557,
            RemoteHit = 558,
            MercyPrayer = 559,
            WarlordSpear = 560,


        }
        [Flags]
        public enum DataType : int
        {
            Hitpoints2 = -1,
            Hitpoints = 0,
            MaxHitpoints = 1,
            Mana = 2,
            MaxMana = 3,
            Money = 4,
            Experience = 5,
            PKPoints = 6,
            Class = 7,
            Stamina = 8,
            WHMoney = 9,
            Atributes = 10,
            Mesh = 11,
            Level = 12,
            Spirit = 13,
            Vitality = 14,
            Strength = 15,
            Agility = 16,
            HeavensBlessing = 17,
            DoubleExpTimer = 18,
            CursedTimer = 20,
            Reborn = 22,
            VirtutePoints = 23,
            StatusFlag = 25,
            HairStyle = 26,
            XPCircle = 27,
            LuckyTimeTimer = 28,
            ConquerPoints = 29,
            OnlineTraining = 31,
            ExtraBattlePower = 36,
            ArsenalBp = 37,
            Merchant = 38,
            VIPLevel = 39,
            QuizPoints = 40,
            EnlightPoints = 41,
            ClanShareBp = 42,
            GuildBattlePower = 44,
            BoundConquerPoints = 45,
            RaceShopPoints = 47,
            Contestant = 48,
            AzureShield = 49,
            FirsRebornClass = 51,
            SecondRebornClass = 50,
            Team = 52,
            SoulShackle = 54,
            Fatigue = 55,
            DefensiveStance = 56,

            IncreaseMStrike = 60,
            IncreasePStrike = 59,
            IncreaseImunity = 61,
            IncreaseBreack = 62,
            IncreaseAntiBreack = 63,
            IncreaseMaxHp = 64,
            IncreasePAttack = 65,
            IncreaseMAttack = 66,
            IncreaseFinalPDamage = 67,
            IncreaseFinalMDamage = 68,
            IncreaseFinalPAttack = 69,
            IncreaseFinalMAttack = 70,
            MainFlag = 71,
            ExpProtection = 73,

            DragonSwing = 75,
            DragonFury = 74,
            InnerPowerPotency = 77,
            AppendIcon = 78,
            InventorySash = 79,
            InventorySashMax = 80,
            ExploitsRank = 82,
            UnionRank = 83,
            LeagueContribution = 84,
            Anger = 90,
            DominoCoins = 91,
            HuntingBonus = 100,
            XPList = 101,
            FineRain = 102,
            Infinity = 103,
            XPDuration = 104,
            Slayer = 105,
            Absolution = 106,
            ForbiddenPirate = 107,
            SkillCountdown = 108,
            UndyingImprinting = 111,
            DevouringStrike = 112,
            TidalWave = 113,
            Quench = 114,
            RisingPhoenix = 118,
            CelestialDance = 119,
            ArchiveSkill = 120,
            ClassExperience = 121,
            SageModeLevel = 123,
            FlameofDestruction = 124,
            MyDontion = 125,
            DonationPoints = 126,
            CyanJadeRing = 127,
            Return = 132,
            HoverData = 133,
            ArrowBlades = 137,
            Bloodthirst_Elan = 138,
            StarFlow = 136,
            FlowerRank = 144,
            SuanniCommand = 145,
            DuneEnergy = 164,
            Disorder = 486,
            ApePistol = 487,
            BearsCare = 489,
        }

        public ServerSockets.Packet Append(ServerSockets.Packet stream, ulong Damage)//Yuanshen
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = DataType.AppendIcon,
                Flag = (uint)Qahr.Mobile.Role.Enums.Flag.EonspiritCurrentEnergy,
                Time = 0,
                Damage = (int)Damage,
                SpellLevel = 0,
                Damage2 = 0,
                SpellLevel2 = 0,
            });
            return stream;
        }
        public ServerSockets.Packet AppendDune(ServerSockets.Packet stream, int Value, long Value2, long Value3)//DuneWanderer
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = DataType.DuneEnergy,
                SpellLevel = Value,
                Damage2 = Value2,
                SpellLevel2 = Value3,
            });
            return stream;
        }
        public unsafe MsgUpdate(ServerSockets.Packet stream, uint UID, int count = 1)
        {
            Info = new MsgBuilder.UpdateProto() { UID = UID };
            myUpdates = new List<MsgBuilder.UpdateQuery>();
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, ulong Value)
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = ID,
                Value = Value
            });

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, long Value)
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = ID,
                Value = (ulong)Value
            });

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint Flag, uint Time, uint Dmg, uint Level)
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = ID,
                Flag = Flag,
                Time = Time,
                Damage = (int)Dmg,
                SpellLevel = (int)Level
            });

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint Flag, uint Time, int Dmg, int Level)
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = ID,
                Flag = Flag,
                Time = Time,
                Damage = Dmg,
                SpellLevel = Level
            });

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint Flag, uint Time, int Dmg, int Level, long Dmg2, long Level2)
        {
            myUpdates.Add(new MsgBuilder.UpdateQuery()
            {
                Type = ID,
                Flag = Flag,
                Time = Time,
                Damage = Dmg,
                SpellLevel = Level,
                Damage2 = Dmg2,
                SpellLevel2 = Level2
            });

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint[] Value)
        {
            ulong[] myValues = new ulong[Value.Length];
            for (int i = 0; i < myValues.Length; i++)
            {
                myValues[i] = Value[i];
            }
            if (myValues != null)
            {
                myUpdates.Add(new MsgBuilder.UpdateQuery()
                {
                    Type = ID,
                    Values = myValues
                });
            }
            return stream;
        }

        public ServerSockets.Packet GetArray(ServerSockets.Packet stream)
        {
            if (myUpdates != null)
                Info.Updates = myUpdates.ToArray();
            else
                Info.Updates = null;

            if (Info.Updates == null || Info.Updates.Length == 0)
                return null;

            foreach (var update in Info.Updates)
            {
                if (update.Values == null)
                    update.Values = new ulong[0];
            }

            // [ProtoFix 2026-10-14] reset the writer: callers reuse the incoming stream (e.g. MsgAction 2099 CompleteLogin),
            // and without this the 2099 body (#1,#3,#9,#10,#12-15) was left in front of the 2046 body -> unknown fields on the client.
            stream.InitWriter();
            stream.ProtoBufferSerialize(Info);
            stream.Finalize(GamePackets.MsgUserAttrib);

            return stream;
        }


    }
}
