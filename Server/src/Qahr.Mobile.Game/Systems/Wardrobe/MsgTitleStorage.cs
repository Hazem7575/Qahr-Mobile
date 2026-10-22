using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgTitleStorage
    {
        public enum AllTitle : uint
        {

            Overlord = 1,
            Unbeatable = 10,
            StormGirl = 11,
            ChosenOne = 9,
            SupremeHonor = 2000,
            RisingStar = 2001,
            Victor = 2002,
            Conqueror = 2003,
            Talent = 2004,
            Fashionist = 2005,
            SwiftChaser = 2006,
            MonkeyRider = 2013,
            SolarRider = 2014,
            LunarRider = 2015,
            SaintRider = 2016,
            Grandmaster = 2018,
            Fairy = 2020,
            Goddess = 2021,
            Beauty = 2022,
            Scholarz = 2023,
            Handsome = 2024,
            Wise = 2025,
            Superman = 2026,
            Scholar = 2027,
            EarthKnight = 2028,
            GloryKnight = 2029,
            SkyKnight = 2030,
            Paladin = 2031,
            BigFan = 2032,
            EuroCollector = 2033,
            Invincible = 2034,
            Legendary = 2035,
            Peerless = 2036,
            Outstanding = 2037,
            Expert = 2038,
            PeerlessBeauty = 2040,
            SuperIdol = 2041,
            MostRadiant = 2044,
            MostAttractive = 2045,
            PokerKing = 2046,
            PokerLord = 2047,
            PokerMaster = 2048,
            PokerStar = 2049,
            ImmortalFate = 2050,
            SuccessortoLege = 2051,
            FashionIcon = 2052,
            FashionIdol = 2053,
            FashionMaster = 2054,
            PlayfulPlayer = 2309,
            SlamDunkLeade = 2056,
            HonoredHero = 2057,
            KingTeam = 2059,
            DominatorTeam = 2060,
            PowerTeam = 2061,
            EliteTeam = 2062,
            LoveJoker = 2063,
            SweetFairy = 2064,
            LovesMadness = 2065,
            MirrorMaster = 2077,
            TripodMaster = 2078,
            BeadsMaster = 2079,
            SealMaster = 2080,
            BellMaster = 2081,
            Professional = 2082,
            EliteShooter = 2083,
            KingofFootball = 2084,
            HansomeBoy = 2085,
            AttractiveGentleman = 2086,
            BeautifulLady = 2087,
            BrightestKing = 2088,
            CharmingPrince = 2089,
            BrightestQueen = 2090,
            CharmingPrincess = 2091,
            FairyGirl = 2092,
            Sweetness = 2100,
            Handsomeness = 2101,
            SuperBeauty = 2102,
            SuperBrilliance = 2103,
            MasterofJustice = 2123,
            HeroicPride = 2124,
            TopMaster = 2125,
            WorldExplorer = 2126,
            Trainee = 2127,
            ExpertNew = 2128,
            Elite = 2129,
            Master = 2130,
            GrandmasterNew = 2131,
            LegendaryNew = 2132,
            NoblePionner = 2133,
            Tycoon = 2134,
            Magnifico = 2135,
            Millionaire = 2136,
            LuckyGirl = 2138,
            BrightBoy = 2139,
            StunningBeauty = 2140,
            BrilliantMan = 2141,
            WhyAmISoBeautiful = 2142,
            WhyAmISoHandsome = 2143,
            OverlordofConquer = 2145,
            WorldDiceMaster = 2148,
            DiceChampionChina = 2149,
            DiceChampionEnglish = 2150,
            ReyEspaoldeDados = 2151,
            Unk = 2152,
            NewRuler = 2158,
            TurkeyTerminator = 2162,
            Unstoppable = 2163,
            WidelyKnown = 2164,
            SacredDragon = 2165,
            SacredPhoenix = 2166,
            SacredTiger = 2167,
            SacredTortoise = 2168,
            Matchless = 2169,
            DivineSage = 2170,
            ForeverBroke = 2174,
            ToxicKingKirby = 2175,
            RatTitle = 2176,
            FairLady = 2179,
            FineGentleman = 2180,
            LoveIgniter = 2181,
            Heartthrob = 2182,
            HeartStealer = 2183,
            UniqueBeauty = 2184,
            FoolFun = 2185,
            CapsuleMaster = 2188,
            SupremeGoldVIP = 2189,
            SupremePlatinumVIP = 2190,
            SupremeDiamondVIP = 2191,
            ExclusiveLove = 2194,
            ImpressiveCharm = 2196,
            EnduringLove = 2195,
            SereneGrace = 2197,
            FreeAsWind = 2198,
            CharmingAsSunglow = 2199,
            DragonLord = 2200,
            DragonSlayer = 2201,
            CyberPrivilege = 2203,
            IllusionMaster = 2204,
            SuperbWarrior = 2206,
            OxFortune = 2210,
            ThornFighter = 2211,
            RoseMaiden = 2212,
            KamenRider = 2213,
            FancyPrincess = 2214,
            DarknightDuke = 2215,
            PerfectPeak = 2216,
            AzureQueen = 2217,
            SupremeTycoon = 2218,
            ClownMe = 2220,
            AnniversaryTitle = 2221,
            SuperiorClan = 2224,
            CuteHero = 2225,
            CuteConqueror = 2226,
            SilverDistributor = 2227,
            BraveExplorer = 2232,
            Adventurer = 2233,
            PromiseofSky = 2235,
            LoveofSky = 2236,
            AquilaSong = 2237,
            LyraMelody = 2238,
            OdeofStar = 2239,
            VowofStar = 2241,
            BrightHop = 2246,
            DuskyGlory = 2247,
            ThanksgivingGourmet = 2252,
            TigerMinded = 2254,
            TalentedMonopoly = 2255,
            BeaPear = 2264,
            Pearoff = 2265,
            LikeYouBerryMuch = 2266,
            MyBerryGirl = 2267,
            SweetGrapefruit = 2268,
            SweetFool = 2269,
            EarthConqueror = 2270,
            BloomofYouth = 2273,
            DragonTamer = 2275,
            MasterofSports = 2278,
            PetAthlete = 2279,
            TheLittlePrince = 2280,
            Romeo = 2281,
            Apollo = 2282,
            TheRose = 2283,
            Juliet = 2284,
            Daphne = 2285,
            UtterOrdealRed = 2289,
            UtterOrdealGreen = 2292,
            ThreeMusketurkeersBrave = 2294,
            ThreeMusketurkeersFlurry = 2295,
            Rabbicute = 2296,
            RomanticMelody = 2298,
            DevotedNote = 2299,
            LoveAria = 2300,
            LoveSonata = 2301,


            CosmosDragon = 2311,
            MarineWhopper = 2312,
            SavannaLion = 2313,
            StormFalcon = 2314,
            Richabbit = 2297,
            Daydreamer = 2318,
            Younesop = 2331,
            FairFox = 2329,
            FreePasser = 2330,
            MoononCloud = 2325,
            SnowinWind = 2326,
            SweetJill = 2327,
            DashingDandy = 2328,
            StarDreamveil = 8046,
            DemonWingsGory = 6098,
            DemonWingsFiery = 6099,
            ForeverYoung = 2343,
            DesertShadow = 2344,

            FancyVictor = 2342,
            WhiteSwanConcerto = 2372,
            BlackSwanSymphony = 2373,
            WhereLoveBelongs = 2374,
            WhereHeartLies = 2375,
            GracefulasWater = 2376,


            EmperorStar = 2356,
            HeavenlyDamageStar = 2357,
            HeavenlyCourageStar = 2358,
            HeavenlyPowerStar = 2359,
            HeavenlySolitaryStar = 2360,
            HeavenlyWealthStar = 2361,
            HeavenlyMysteryStar = 2362,
            HeavenlyUnityStar = 2363,
            HeavenlyValorStar = 2364,
            HeavenlyVirtueStar = 2365,
            HeavenlyForceStar = 2366,
            HeavenlyProwessStar = 2367,
            HeavenlyNobleStar = 2368,
            HeavenlyLeisureStar = 2369,
            HeavenlyMightStar = 2370,
            HeavenlyHulkStar = 2371,
            MasterMind = 2288,
            VibrantasFire = 2377,
            ThrivingDragon = 2355,
            Dragonling = 2404,
            Starrider = 2407,
            Moonstrider = 2408,
            ShootingStar = 2409,
            GleamingMoon = 2410,
            StarSerenade = 2411,
            DragonFavored = 2379,
            TalentedGoosie = 2389,
            DragonicJoy = 2399,
            MoonMelody = 2412,
            DemonVanquisher = 2402,
            FiendQueller = 2403,


            DestinedOne = 2421,
            FinaleFeast = 2425,
            EpicCarnival = 2430,
            SupremeRuler = 2432,
            SupremeRenownedFigureTitle = 2427,
            PrestigeRenownedFigureTitle = 2428,
            RenownedFigureTitle = 2429,
            LatteSweetheart = 2440,
            MochaGentleman = 2441,
            RococoPrincess = 2442,
            BaroqueDuke = 2443,


            WingsofSolarDra = 4001,
            GlitterFeather = 6000,
            WingsofInfernal = 6001,
            RadiantWings = 6002,
            StarlightWings = 6003,
            MoonlightWings = 6004,
            FairyWings = 6005,
            VioletCloudWing = 6007,
            VioletLightning = 6008,
            WingsofPlanet = 6009,
            Supreme = 6011,
            WingsofRomance = 6012,
            EmeraldGlowWing = 6013,
            OrangeGlowWings = 6014,
            FlameGlowWings = 6015,
            BrightGlowWings = 6016,
            SirenSong = 6017,
            WingsofHeart = 6020,
            ButterflyWing = 6021,
            LoveWings = 6022,
            VioletWings = 6023,
            FragrantWings = 6025,
            WingsofSlaughter = 6030,
            CelestialWings = 6031,
            HollyBlueWings = 6032,
            SpringFestivalWings = 6033,
            NewYearWings = 6034,
            LastingLove = 6036,
            DestinedLove = 6037,
            FlutteringFlag2 = 6038,
            FlutteringFlag = 6039,
            FrostPhoenixWings = 6040,
            MegaPhoenixWings = 6041,
            FloralWings = 6043,
            SilverButterfly = 6044,
            TipsyDreamGold = 6045,
            TipsyDream = 6046,
            SpotlessFeather = 6047,
            ShimmeringStarBrilliant = 6048,
            ShimmeringStar = 6049,
            HolyWings = 6051,
            FrostyHeartPlume = 6052,
            FrostyPlume = 6053,
            FrostyBlossomPlume = 6054,
            FlamePapilio = 6056,
            FlamePapilioGolden = 6058,
            LoveSoundHeaven = 6073,
            LoveSound = 6074,
            StarryMoon = 6075,
            LaurelJade = 6076,
            SilentSpreadWings = 6078,
            SilentWings = 6079,
            AmazingXmas = 6082,
            KnightPlume = 6084,
            GoldenPapilio = 6086,
            SparklingSummer = 6088,
            MorningFeather = 6102,
            MorningFeatherGlory = 6103,
            IvoryGleam = 6106,
            DemonWingsWithered = 6107,
            FrostbladeWingsIce = 6097,


            EbonyDreamVow = 6108,
            BalloonTravelColorful = 6109,
            DreamMelodyMystic = 6114,
            AbyssalFlame = 6121,
            CrimsonwingsBirth = 6126,

            DreamweavePlumeCrystal = 6101,
            IrisWhisper = 6129,
            GildedRadianceMirage = 6132,

            MonthlyPKChampion = 8001,
            WeeklyPKChampion = 8002,
            MostValuableNewcomer = 8004,
            No1Warrior = 8005,
            No1Trojan = 8006,
            No1Archer = 8007,
            No1WaterTaoist = 8008,
            No1FireTaoist = 8009,
            No1Ninja = 8010,
            No1Monk = 8011,
            No1Pirate = 8012,
            No1DragonWarrior = 8013,
            No1Windwalker = 8014,
            No1Thunderstriker = 8015,
            COMrUniverseChampion = 8016,
            COMrUniverseRunner = 8017,
            COMrUniverse2ndRunner = 8018,
            COMsUniverseChampion = 8019,
            COMsUniverseRunnerup = 8020,
            COMsUniverse2ndRunnerup = 8021,
            StrongestClan = 8022,
            GlobalLoveStar = 8023,
            GlobalLoveNumen = 8024,
            GlobalLoveAngel = 8025,

            GiftedBeauty = 8026,
            GiftedScholar = 8027,
            WorldsBestGuildLeader = 8028,
            WorldsBestDeputyLeader = 8029,
            CSPKL120GroupChampion = 8030,
            CSPKL120GroupRunnerup = 8031,
            CSPKL120Group2ndRunnerup = 8032,
            CSPKL120GroupTop8 = 8033,
            CSPKL130GroupChampion = 8034,
            CSPKL130GroupRunnerup = 8035,
            CSPKL130Group2ndRunnerup = 8036,
            CSPKL130GroupTop8 = 8037,
            FirstLoveHalo = 8038,
            GalaxySkyHalo = 8039,
            ArrowofLoveHalo = 8040,
            PhoenixFlyHalo = 8041,
            CSTeamPKChampionHalo = 8043,
            Cillionaire = 8044,
            FlutteringPhoenix = 8045,
            AspiredImmortalLotus = 8047,
            CelestialRainbow = 8049,
            LivelySpringBunny = 8050,
            ButterflyWhisper = 8051,
            MarvelousLand = 8052,
            FantasticWander = 8056,
            Cillionaire1 = 8060,
            SupremeRenownedFigureHalo = 8063,
            PrestigeRenownedFigureHalo = 8064,
            RenownedFigureHalo = 8065,

            AstralFlutter = 8071,


            LotusGrace1 = 9001,
            LotusGrace = 9002,
            SakuraGrace = 9005,
            BlazesFootprint1 = 9006,
            BlazesFootprint = 9007,
            ZookosFootprint = 9008,
            BloodwingSpecter = 9009,
            LuckyStarshine = 9010,


            BlazesAction3 = 9503,
            SupremeAlone2 = 9500,
            SupremeAlone = 9501,
            FallingSakura = 9502,
            BlazesAction = 9504,
            ZookosAction = 9505,
            MaleficMoon = 9506,
            Sandsong = 9508,


        }
        public enum TitleType : uint
        {
            #region Title
            Overlord = 1,
            Unbeatable = 10,
            StormGirl = 11,
            ChosenOne = 9,
            SupremeHonor = 2000,
            RisingStar = 2001,
            Victor = 2002,
            Conqueror = 2003,
            Talent = 2004,
            Fashionist = 2005,
            SwiftChaser = 2006,
            MonkeyRider = 2013,
            SolarRider = 2014,
            LunarRider = 2015,
            SaintRider = 2016,
            Grandmaster = 2018,
            Fairy = 2020,
            Goddess = 2021,
            Beauty = 2022,
            Scholarz = 2023,
            Handsome = 2024,
            Wise = 2025,
            Superman = 2026,
            Scholar = 2027,
            EarthKnight = 2028,
            GloryKnight = 2029,
            SkyKnight = 2030,
            Paladin = 2031,
            BigFan = 2032,
            EuroCollector = 2033,
            Invincible = 2034,
            Legendary = 2035,
            Peerless = 2036,
            Outstanding = 2037,
            Expert = 2038,
            PeerlessBeauty = 2040,
            SuperIdol = 2041,
            MostRadiant = 2044,
            MostAttractive = 2045,
            PokerKing = 2046,
            PokerLord = 2047,
            PokerMaster = 2048,
            PokerStar = 2049,
            ImmortalFate = 2050,
            SuccessortoLege = 2051,
            FashionIcon = 2052,
            FashionIdol = 2053,
            FashionMaster = 2054,
            PlayfulPlayer = 2309,
            SlamDunkLeade = 2056,
            HonoredHero = 2057,
            KingTeam = 2059,
            DominatorTeam = 2060,
            PowerTeam = 2061,
            EliteTeam = 2062,
            LoveJoker = 2063,
            SweetFairy = 2064,
            LovesMadness = 2065,
            MirrorMaster = 2077,
            TripodMaster = 2078,
            BeadsMaster = 2079,
            SealMaster = 2080,
            BellMaster = 2081,
            Professional = 2082,
            EliteShooter = 2083,
            KingofFootball = 2084,
            HansomeBoy = 2085,
            AttractiveGentleman = 2086,
            BeautifulLady = 2087,
            BrightestKing = 2088,
            CharmingPrince = 2089,
            BrightestQueen = 2090,
            CharmingPrincess = 2091,
            FairyGirl = 2092,
            Sweetness = 2100,
            Handsomeness = 2101,
            SuperBeauty = 2102,
            SuperBrilliance = 2103,
            MasterofJustice = 2123,
            HeroicPride = 2124,
            TopMaster = 2125,
            WorldExplorer = 2126,
            Trainee = 2127,
            ExpertNew = 2128,
            Elite = 2129,
            Master = 2130,
            GrandmasterNew = 2131,
            LegendaryNew = 2132,
            NoblePionner = 2133,
            Tycoon = 2134,
            Magnifico = 2135,
            Millionaire = 2136,
            LuckyGirl = 2138,
            BrightBoy = 2139,
            StunningBeauty = 2140,
            BrilliantMan = 2141,
            WhyAmISoBeautiful = 2142,
            WhyAmISoHandsome = 2143,
            OverlordofConquer = 2145,
            WorldDiceMaster = 2148,
            DiceChampionChina = 2149,
            DiceChampionEnglish = 2150,
            ReyEspaoldeDados = 2151,
            Unk = 2152,
            NewRuler = 2158,
            TurkeyTerminator = 2162,
            Unstoppable = 2163,
            WidelyKnown = 2164,
            SacredDragon = 2165,
            SacredPhoenix = 2166,
            SacredTiger = 2167,
            SacredTortoise = 2168,
            Matchless = 2169,
            DivineSage = 2170,
            ForeverBroke = 2174,
            ToxicKingKirby = 2175,
            RatTitle = 2176,
            FairLady = 2179,
            FineGentleman = 2180,
            LoveIgniter = 2181,
            Heartthrob = 2182,
            HeartStealer = 2183,
            UniqueBeauty = 2184,
            FoolFun = 2185,
            CapsuleMaster = 2188,
            SupremeGoldVIP = 2189,
            SupremePlatinumVIP = 2190,
            SupremeDiamondVIP = 2191,
            ExclusiveLove = 2194,
            ImpressiveCharm = 2196,
            EnduringLove = 2195,
            SereneGrace = 2197,
            FreeAsWind = 2198,
            CharmingAsSunglow = 2199,
            DragonLord = 2200,
            DragonSlayer = 2201,
            CyberPrivilege = 2203,
            IllusionMaster = 2204,
            SuperbWarrior = 2206,
            OxFortune = 2210,
            ThornFighter = 2211,
            RoseMaiden = 2212,
            KamenRider = 2213,
            FancyPrincess = 2214,
            DarknightDuke = 2215,
            PerfectPeak = 2216,
            AzureQueen = 2217,
            SupremeTycoon = 2218,
            ClownMe = 2220,
            AnniversaryTitle = 2221,
            SuperiorClan = 2224,
            CuteHero = 2225,
            CuteConqueror = 2226,
            SilverDistributor = 2227,
            BraveExplorer = 2232,
            Adventurer = 2233,
            PromiseofSky = 2235,
            LoveofSky = 2236,
            AquilaSong = 2237,
            LyraMelody = 2238,
            OdeofStar = 2239,
            VowofStar = 2241,
            BrightHop = 2246,
            DuskyGlory = 2247,
            ThanksgivingGourmet = 2252,
            TigerMinded = 2254,
            TalentedMonopoly = 2255,
            BeaPear = 2264,
            Pearoff = 2265,
            LikeYouBerryMuch = 2266,
            MyBerryGirl = 2267,
            SweetGrapefruit = 2268,
            SweetFool = 2269,
            EarthConqueror = 2270,
            BloomofYouth = 2273,
            DragonTamer = 2275,
            MasterofSports = 2278,
            PetAthlete = 2279,
            TheLittlePrince = 2280,
            Romeo = 2281,
            Apollo = 2282,
            TheRose = 2283,
            Juliet = 2284,
            Daphne = 2285,
            UtterOrdealRed = 2289,
            UtterOrdealGreen = 2292,
            ThreeMusketurkeersBrave = 2294,
            ThreeMusketurkeersFlurry = 2295,
            Rabbicute = 2296,
            RomanticMelody = 2298,
            DevotedNote = 2299,
            LoveAria = 2300,
            LoveSonata = 2301,


            CosmosDragon = 2311,
            MarineWhopper = 2312,
            SavannaLion = 2313,
            StormFalcon = 2314,
            Richabbit = 2297,
            Daydreamer = 2318,
            Younesop = 2331,
            FairFox = 2329,
            FreePasser = 2330,
            MoononCloud = 2325,
            SnowinWind = 2326,
            SweetJill = 2327,
            DashingDandy = 2328,
            StarDreamveil = 8046,
            DemonWingsGory = 6098,
            DemonWingsFiery = 6099,
            ForeverYoung = 2343,
            DesertShadow = 2344,

            FancyVictor = 2342,
            WhiteSwanConcerto = 2372,
            BlackSwanSymphony = 2373,
            WhereLoveBelongs = 2374,
            WhereHeartLies = 2375,
            GracefulasWater = 2376,


            EmperorStar = 2356,
            HeavenlyDamageStar = 2357,
            HeavenlyCourageStar = 2358,
            HeavenlyPowerStar = 2359,
            HeavenlySolitaryStar = 2360,
            HeavenlyWealthStar = 2361,
            HeavenlyMysteryStar = 2362,
            HeavenlyUnityStar = 2363,
            HeavenlyValorStar = 2364,
            HeavenlyVirtueStar = 2365,
            HeavenlyForceStar = 2366,
            HeavenlyProwessStar = 2367,
            HeavenlyNobleStar = 2368,
            HeavenlyLeisureStar = 2369,
            HeavenlyMightStar = 2370,
            HeavenlyHulkStar = 2371,
            MasterMind = 2288,
            VibrantasFire = 2377,
            ThrivingDragon = 2355,
            Dragonling = 2404,
            Starrider = 2407,
            Moonstrider = 2408,
            ShootingStar = 2409,
            GleamingMoon = 2410,
            StarSerenade = 2411,
            DragonFavored = 2379,
            TalentedGoosie = 2389,
            DragonicJoy = 2399,
            MoonMelody = 2412,
            DemonVanquisher = 2402,
            FiendQueller = 2403,


            DestinedOne = 2421,
            FinaleFeast = 2425,
            EpicCarnival = 2430,
            SupremeRuler = 2432,
            SupremeRenownedFigureTitle = 2427,
            PrestigeRenownedFigureTitle = 2428,
            RenownedFigureTitle = 2429,
            LatteSweetheart = 2440,
            MochaGentleman = 2441,
            RococoPrincess = 2442,
            BaroqueDuke = 2443,

            Dreamcrafter = 2446,

            #endregion


        }
        public enum WingsType : uint
        {
            #region Wings

            WingsofSolarDra = 4001,
            GlitterFeather = 6000,
            WingsofInfernal = 6001,
            RadiantWings = 6002,
            StarlightWings = 6003,
            MoonlightWings = 6004,
            FairyWings = 6005,
            VioletCloudWing = 6007,
            VioletLightning = 6008,
            WingsofPlanet = 6009,
            Supreme = 6011,
            WingsofRomance = 6012,
            EmeraldGlowWing = 6013,
            OrangeGlowWings = 6014,
            FlameGlowWings = 6015,
            BrightGlowWings = 6016,
            SirenSong = 6017,
            WingsofHeart = 6020,
            ButterflyWing = 6021,
            LoveWings = 6022,
            VioletWings = 6023,
            FragrantWings = 6025,
            WingsofSlaughter = 6030,
            CelestialWings = 6031,
            HollyBlueWings = 6032,
            SpringFestivalWings = 6033,
            NewYearWings = 6034,
            LastingLove = 6036,
            DestinedLove = 6037,
            FlutteringFlag2 = 6038,
            FlutteringFlag = 6039,
            FrostPhoenixWings = 6040,
            MegaPhoenixWings = 6041,
            FloralWings = 6043,
            SilverButterfly = 6044,
            TipsyDreamGold = 6045,
            TipsyDream = 6046,
            SpotlessFeather = 6047,
            ShimmeringStarBrilliant = 6048,
            ShimmeringStar = 6049,
            HolyWings = 6051,
            FrostyHeartPlume = 6052,
            FrostyPlume = 6053,
            FrostyBlossomPlume = 6054,
            FlamePapilio = 6056,
            FlamePapilioGolden = 6058,
            LoveSoundHeaven = 6073,
            LoveSound = 6074,
            StarryMoon = 6075,
            LaurelJade = 6076,
            SilentSpreadWings = 6078,
            SilentWings = 6079,
            AmazingXmas = 6082,
            KnightPlume = 6084,
            GoldenPapilio = 6086,
            SparklingSummer = 6088,
            MorningFeather = 6102,
            MorningFeatherGlory = 6103,
            IvoryGleam = 6106,
            DemonWingsWithered = 6107,
            FrostbladeWingsIce = 6097,


            EbonyDreamVow = 6108,
            BalloonTravelColorful = 6109,
            DreamMelodyMystic = 6114,
            AbyssalFlame = 6121,
            CrimsonwingsBirth = 6126,

            DreamweavePlumeCrystal = 6101,
            IrisWhisper = 6129,
            GildedRadianceMirage = 6132,
            FireflyFlutter = 6137,
            StarburnBloom = 6143,
            GoldenPapilioFancy = 6086,
            #endregion
        }
        public enum HaloType : uint
        {
            #region Halo
            MonthlyPKChampion = 8001,
            WeeklyPKChampion = 8002,
            MostValuableNewcomer = 8004,
            No1Warrior = 8005,
            No1Trojan = 8006,
            No1Archer = 8007,
            No1WaterTaoist = 8008,
            No1FireTaoist = 8009,
            No1Ninja = 8010,
            No1Monk = 8011,
            No1Pirate = 8012,
            No1DragonWarrior = 8013,
            No1Windwalker = 8014,
            No1Thunderstriker = 8015,
            COMrUniverseChampion = 8016,
            COMrUniverseRunner = 8017,
            COMrUniverse2ndRunner = 8018,
            COMsUniverseChampion = 8019,
            COMsUniverseRunnerup = 8020,
            COMsUniverse2ndRunnerup = 8021,
            StrongestClan = 8022,
            GlobalLoveStar = 8023,
            GlobalLoveNumen = 8024,
            GlobalLoveAngel = 8025,

            GiftedBeauty = 8026,
            GiftedScholar = 8027,
            WorldsBestGuildLeader = 8028,
            WorldsBestDeputyLeader = 8029,
            CSPKL120GroupChampion = 8030,
            CSPKL120GroupRunnerup = 8031,
            CSPKL120Group2ndRunnerup = 8032,
            CSPKL120GroupTop8 = 8033,
            CSPKL130GroupChampion = 8034,
            CSPKL130GroupRunnerup = 8035,
            CSPKL130Group2ndRunnerup = 8036,
            CSPKL130GroupTop8 = 8037,
            FirstLoveHalo = 8038,
            GalaxySkyHalo = 8039,
            ArrowofLoveHalo = 8040,
            PhoenixFlyHalo = 8041,
            CSTeamPKChampionHalo = 8043,
            Cillionaire = 8044,
            FlutteringPhoenix = 8045,
            AspiredImmortalLotus = 8047,
            CelestialRainbow = 8049,
            LivelySpringBunny = 8050,
            ButterflyWhisper = 8051,
            MarvelousLand = 8052,
            FantasticWander = 8056,
            Cillionaire1 = 8060,
            SupremeRenownedFigureHalo = 8063,
            PrestigeRenownedFigureHalo = 8064,
            RenownedFigureHalo = 8065,

            AstralFlutter = 8071,
            FantasyCastle = 8073,

            #endregion
        }
        public enum Footprint : uint
        {
            #region Foot Prints
            LotusGrace1 = 9001,
            LotusGrace = 9002,
            SakuraGrace = 9005,
            BlazesFootprint1 = 9006,
            BlazesFootprint = 9007,
            ZookosFootprint = 9008,
            BloodwingSpecter = 9009,
            LuckyStarshine = 9010,
            FireflyFlit = 9013,
            PhoenixGrace = 9014,
            DancingPetal = 9016,
            AuspiciousGlow = 9019,
            SnowbloomStep = 9017,
            MoonKiss = 9022,
            Bloomjump = 9021,
            RadiantReturn = 9023,
            AdoringWaltz = 10101,
            FushengBurningRipple = 9024,
            Abysswalk = 9026,
            StarryFeatherfall = 9029,
            BlazingTrail = 9033,

            #endregion
        }
        public enum HaloAction : uint
        {
            #region Actions
            SupremeAlone2 = 9500,
            SupremeAlone = 9501,
            FallingSakura = 9502,
            BlazesAction3 = 9503,
            BlazesAction = 9504,
            ZookosAction = 9505,
            MaleficMoon = 9506,
            Sandsong = 9508,
            PhoenixBloom = 9509,
            MelodiousStroll = 9511,
            LegacyBreeze = 9540,
            ZADICOAction = 9541,
            FushengLurkingAbyss = 9542,
            HeatwaveCheer = 9544,
            VibrantSummer = 9546,
            #endregion
        }
        public enum KillEffects : uint
        {
            #region Kill Effects
            FeralRoar = 10000,
            DuskFinale = 10001,
            JasmineSnowfallFu = 10002,
            CloudRacer = 10003,
            DragonFlight = 10005,

            #endregion
        }
        [ProtoContract]
        public class TitleStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public Action ActionID;
            [ProtoMember(2, IsRequired = true)]
            public uint dwparam1;
            [ProtoMember(3, IsRequired = true)]
            public uint dwparam2;
            [ProtoMember(4, IsRequired = true)]
            public uint dwparam3;
            [ProtoMember(5, IsRequired = true)]
            public uint dwparam4;
            // [ProtoFix 2026-10-14] client CMsgTitleStoragePB #6 is REPEATED CMsgTitleInfoPB (was a single Title).
            // The wire member is now the list; the old single "Title" stays as a [ProtoIgnore] helper over it,
            // so all existing "x.Title = new Title(); x.Title.ID = ..." code keeps working unchanged.
            [ProtoMember(6)]
            public List<Title> Titles;

            [ProtoIgnore]
            public Title Title
            {
                get { return (Titles != null && Titles.Count > 0) ? Titles[Titles.Count - 1] : null; }
                set { Titles = value == null ? null : new List<Title> { value }; }
            }
        }
        [ProtoContract]
        public class Title
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ID;
            [ProtoMember(2, IsRequired = true)]
            public uint SubId;
            [ProtoMember(3, IsRequired = true)]
            public uint dwparam1;
            [ProtoMember(4, IsRequired = true)]
            public ulong TotalSeconds;
        }
        [Flags]
        public enum Action : uint
        {
            UpdateScore = 0,
            UseTitle = 1,
            RemoveTitle = 3,
            Equip = 4,
            UnEquip = 5,
            FullLoad = 6
        }
        private static string GetFoorPrintsName(uint id)
        {
            switch (id)
            {
                case 9017: return "Snowbloom Step";
                case 9023: return "Radiant Return";
                case 9001: return "Lotus Grace";
                case 9005: return "Sakura Grace";
                case 9006: return "Blaze`s Footprint";
                case 9007: return "Zooko`s Footprint";
                case 9009: return "Bloodwing Specter";
                case 9010: return "Lucky Starshine";
                case 9013: return "Firefly Flit";
                case 9014: return "Phoenix Grace";
                case 9016: return "Dancing Petal";
                case 9019: return "Auspicious Glow";
                case 9021: return "Bloomjump";
                case 9022: return "Moon Kiss";
                case 10101: return "AdoringWaltz";
                case 9024: return "Fusheng Burning Ripple";
                default: return "Unknown";
            }
        }
        private static string GetActionsName(uint id)
        {
            switch (id)
            {
                case 9540: return "Legacy Breeze";
                case 9500: return "Supreme Alone";
                case 9502: return "Falling Sakura";
                case 9503: return "Blaze`s Action";
                case 9504: return "Zooko`s Action";
                case 9009: return "Moonblood Oath";
                case 9508: return "Sandsong";
                case 9509: return "Phoenix Bloom";
                case 9511: return "Melodious Stroll";
                case 9541: return "ZADICO Exclusive Action";
                case 9542: return "Fusheng Lurking Abyss";
                default: return "Unknown";
            }
        }
        private static string GetKillersEffectsName(uint id)
        {
            switch (id)
            {
                case 10000: return "Feral Roar";
                case 10001: return "Dusk Finale";
                case 10002: return "Jasmine Snowfall (Fu)";
                case 10003: return "Cloud Racer";
                case 10005: return "Dragon Flight";
                default: return "Unknown";
            }
        }
        public static unsafe ServerSockets.Packet CreateTitleStorage(this ServerSockets.Packet stream, TitleStorage obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgTitleStorage);
            return stream;
        }
        public static unsafe void GetTitleStorage(this ServerSockets.Packet stream, out TitleStorage pQuery)
        {
            pQuery = new TitleStorage();
            pQuery = stream.ProtoBufferDeserialize<TitleStorage>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgTitleStorage)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            TitleStorage pQuery;
            stream.GetTitleStorage(out pQuery);

            switch (pQuery.ActionID)
            {
                case Action.Equip:
                    {
                        if (client.Player.Footprint.Contains((Footprint)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialFootprintID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.Player.SpecialFootprintScore = dbtitle.Score;
                                client.SendSysMesage(GetFoorPrintsName(pQuery.dwparam2) + " has been successfully activated");
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                if (dbtitle.ID >= 4001)
                                    pQuery.Title.dwparam1 = 1;
                                else
                                    pQuery.Title.dwparam1 = dbtitle.Score;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        if (client.Player.HaloAction.Contains((HaloAction)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialHaloAction = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.Player.SpecialHaloActionScore = dbtitle.Score;
                                client.SendSysMesage(GetActionsName(pQuery.dwparam2) + " has been successfully activated");
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                if (dbtitle.ID >= 4001)
                                    pQuery.Title.dwparam1 = 1;
                                else
                                    pQuery.Title.dwparam1 = dbtitle.Score;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        if (client.Player.SpecialTitles.Contains((TitleType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialTitleScore = dbtitle.Score;
                                client.Player.SpecialTitleID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                if (dbtitle.ID >= 4001)
                                    pQuery.Title.dwparam1 = 1;
                                else
                                    pQuery.Title.dwparam1 = dbtitle.Score;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.WingsTitles.Contains((WingsType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialWingID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                if (dbtitle.ID >= 4001)
                                    pQuery.Title.dwparam1 = 1;
                                else
                                    pQuery.Title.dwparam1 = dbtitle.Score;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.KillEffectsAction.Contains((KillEffects)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {


                                client.Player.SpecialHaloID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.SendSysMesage(GetKillersEffectsName(pQuery.dwparam2) + " has been successfully activated");
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                pQuery.Title.dwparam1 = 1;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }

                        }
                        else if (client.Player.HaloTitles.Contains((HaloType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialHaloID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                                client.Send(stream.CreateTitleStorage(pQuery));
                                pQuery.ActionID = Action.UseTitle;
                                pQuery.Title = new Title();
                                pQuery.Title.ID = pQuery.dwparam2;
                                pQuery.Title.SubId = pQuery.dwparam3;
                                if (dbtitle.ID >= 4001)
                                    pQuery.Title.dwparam1 = 1;
                                else
                                    pQuery.Title.dwparam1 = dbtitle.Score;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }

                        }

                        break;
                    }
                case Action.UnEquip:
                    {
                        if (client.Player.SpecialTitles.Contains((TitleType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialTitleScore = client.Player.SpecialTitleID = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.WingsTitles.Contains((WingsType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {

                                client.Player.SpecialWingID = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.HaloTitles.Contains((HaloType)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialTitleScore = client.Player.SpecialHaloID = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }

                        else if (client.Player.Footprint.Contains((Footprint)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialFootprintScore = client.Player.SpecialFootprintID = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.KillEffectsAction.Contains((KillEffects)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialTitleScore = client.Player.SpecialHaloID = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else if (client.Player.HaloAction.Contains((HaloAction)pQuery.dwparam2))
                        {
                            Database.TitleStorage dbtitle;
                            if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                            {
                                client.Player.SpecialHaloAction = client.Player.SpecialHaloActionScore = 0;
                                client.Send(stream.CreateTitleStorage(pQuery));
                                client.Player.View.Clear(stream);
                                client.Player.View.Role(false);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        break;
                    }
            }
            client.PrestrigeEntry.UpdateInfo(client);
        }

    }
}
