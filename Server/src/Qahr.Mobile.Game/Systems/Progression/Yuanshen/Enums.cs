using System;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Role
{
    public class Enums
    {
        [System.Flags]
        public enum EonspiritItem : ushort
        {
            Null = 0,
            ConquerorXiangYu = 1,
            HuntressArtemis = 2,
            WarmasterLadyEmpyrean = 3,
            VictoriousBuddha = 4,
            IronArmoredTitan = 5,
            DesertNightblad = 6,
            FrostWolf = 7,
            Thunderstorm = 8,
            GoldenDragon = 9,
            Phoenix = 10,
        }
        [System.Flags]
        public enum EonspiritPosition : ushort
        {
            Inventory = 0,
            EonspiritInventory = 221,
            EonspiritActive = 222,
            EonspiritUnActive = 223,
        }
        [System.Flags]
        public enum DataType : uint
        {
            ArchiveSkill = 120,
        }
        [System.Flags]
        public enum Flag : int
        {
            Whirlwind = 458,
            DivineArrival = 464,
            SwordShot = 465,
            EchoingSwords = 466,
            ImmortalDestroyer = 469,
            DestructivePower = 470,
            ManiacDance = 471,
            SunsetShine = 473,
            SuperFlash = 473,
            ArcherNormalATK = 474,
            EonspiritCurrentEnergy = 477,
            SwordBody = 478,
            AmazingSpeedActive = 479,//بتشتغل لما الاسكلة تتفعل باسيف
            AmazingSpeed = 480,//بتاخد قيم اللير التالت
            BenefitShower = 481,
            CityRazing = 482,
            GreatAwakening = 508,
            GrandReverence = 510,
            GrandDoctrine = 511,
            TathagataPalm = 909,
        }
        //Rank Type 76
        [System.Flags]
        public enum RankType : uint
        {
            TotalRanking = 280000000,
        }
        [System.Flags]
        public enum SpellID : ushort
        {
            GreatAwakening = 25440,//6//508
            GrandReverence = 25450,//5//510
            GrandDoctrine = 25480,//6//511
            PalmHill = 25410,//21//4860
            TathagataPalm = 25430,//90//909
            //Warrior Item
            /************************/
            Warth = 24790,
            CityRazing = 24810,
            DivineArrival = 24540,
            WarriorNormalATK = 22840,
            CrescentChop = 24380,//116
            CrescentChopEnhanced = 24390,//116
            Whirlwind = 24730,
            ManiacDance = 24740,

            /************************/
            //Archer Item
            /************************/
            AmazingSpeed = 24840,
            AirBlocker = 24850,
            ArcherNormalATK = 24690,
            SuperFlash = 24700,
            BurningSun = 24710,
            SunsetShine = 24680,
            /************************/
            //Taoist Item
            SwordBody = 24820,
            BenefitShower = 24830,
            TaoistDivinityNormalATK = 24420,
            SwordShot = 24450,
            EchoingSwords = 24500,
            ImmortalDestroyer = 24520,
            /************************/
            HeavenBlade = 24870,
            DivineProtection = 24800,
            ToughBow = 24770,
            DestructivePower = 24720,
            NotableMajesty = 24460,
            SpectatorCheer = 24470,
            HarshTune = 24489,
            DragonFlight = 24499,
        }
        public enum MsgTypesNew : ushort
        {
            MsgTexasNpcInfo = 2458,
            MsgAction = 2099,
            MsgTexasInteractive = 2182,
            MsgShowHandEnter = 2262,
            MsgShowHandExit = 2424,
            MsgShowHandDealtCard = 2414,
            MsgShowHandCallAction = 2040,
            MsgShowHandActivePlayer = 2217,
            MsgShowHandLostInfo = 2359,
            MsgShowHandKick = 2169,
            MsgShowHandGameResult = 2256,
            MsgShowHandLayCard = 2080,
            MsgShowHandTrusteeship = 2172,
            MsgTexasPersonalInfo = 2082,
            MsgTexasExMatchFieldList = 2216,
            MsgTexasExInteractive = 2243
        }
        public enum CardsType : byte
        {
            Heart = 0,
            Spade = 1,
            Club = 2,
            Diamond = 3
        }

        public enum PlayerType : byte
        {
            Player = 1,
            Watcher = 2
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
            ShowDown = 5
        }

        public enum TableType : byte
        {
            TexasHoldem = 1,
            ShowHand = 2
        }

        public enum TableUpdate : byte
        {
            Statue = 233,
            Chips = 234,
            PlayerCount = 235
        }

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
    }
}