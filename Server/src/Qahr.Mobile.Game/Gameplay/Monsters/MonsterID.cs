using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgMonster
{
    public static class MonsterID
    {
        
        public static string MonsterSpawnInformationPath = Environment.CurrentDirectory + "\\MonsterSpawn\\";
        public static uint GetID(string Name)
        {
            if (Name == "Pheasant")
                return 1;
            if (Name == "Turtledove")
                return 2;
            if (Name == "Robin")
                return 3;
            if (Name == "Apparition")
                return 4;
            if (Name == "Poltergeist")
                return 5;
            if (Name == "WingedSnake")
                return 6;
            if (Name == "Bandit")
                return 7;
            if (Name == "Ratling")
                return 8;
            if (Name == "FireSpirit")
                return 9;
            if (Name == "Macaque")
                return 10;
            if (Name == "GiantApe")
                return 11;
            if (Name == "ThunderApe")
                return 12;
            if (Name == "Snakeman")
                return 13;
            if (Name == "SandMonster")
                return 14;
            if (Name == "HillMonster")
                return 15;
            if (Name == "RockMonster")
                return 16;
            if (Name == "BladeGhost")
                return 17;
            if (Name == "Birdman")
                return 18;
            if (Name == "HawKing")
                return 19;
            if (Name == "TombBat")
                return 20;
            if (Name == "BattleGhost")
                return 25;
            if (Name == "SmartMonkey")
                return 30;
            if (Name == "RoaringApe")
                return 31;
            if (Name == "SnowApe")
                return 32;
            if (Name == "CrazySnake")
                return 33;
            if (Name == "SandDevil")
                return 34;
            if (Name == "MountMonster")
                return 35;
            if (Name == "StoneGrinder")
                return 36;
            if (Name == "BladeMonster")
                return 37;
            if (Name == "BirdKing")
                return 38;
            if (Name == "SilverHawk")
                return 39;
            if (Name == "Robber")
                return 40;
            if (Name == "MonolithMonster")
                return 51;
            if (Name == "BanditL97")
                return 55;
            if (Name == "BloodyBat")
                return 56;
            if (Name == "BullMonster")
                return 57;
            if (Name == "RedDevilL117")
                return 58;
            if (Name == "RockMonsterL15")
                return 59;
            if (Name == "HeavyGhostL23")
                return 64;
            if (Name == "WingedSnakeL28")
                return 65;
            if (Name == "BanditL33")
                return 66;
            if (Name == "FireRatL38")
                return 67;
            if (Name == "FireSpiritL43")
                return 68;
            if (Name == "MacaqueL48")
                return 69;
            if (Name == "GiantApeL53")
                return 70;
            if (Name == "ThunderApeL58")
                return 71;
            if (Name == "SnakemanL63")
                return 72;
            if (Name == "SandMonsterL68")
                return 73;
            if (Name == "HillMonsterL73")
                return 74;
            if (Name == "RockMonsterL78")
                return 75;
            if (Name == "BladeGhostL83")
                return 76;
            if (Name == "BirdmanL88")
                return 77;
            if (Name == "HawkL93")
                return 78;
            if (Name == "BanditL98")
                return 79;
            if (Name == "TombBatL103")
                return 80;
            if (Name == "BloodyBatL108")
                return 81;
            if (Name == "BullMonsterL113")
                return 82;
            if (Name == "RedDevilL118")
                return 83;
            if (Name == "Banditti")
                return 84;
            if (Name == "SeniorBandit")
                return 85;
            if (Name == "VampireBat")
                return 86;
            if (Name == "VampireKing")
                return 87;
            if (Name == "BloodyDevil")
                return 88;
            if (Name == "DevilKing")
                return 89;
            if (Name == "FlyingBat")
                return 90;
            if (Name == "BloodyKing")
                return 91;
            if (Name == "BullDevil")
                return 92;
            if (Name == "GreenDevil")
                return 93;
            if (Name == "Skeleton")
                return 100;
            if (Name == "GhostWarrior")
                return 101;
            if (Name == "SkeletonKing")
                return 102;
            if (Name == "GhostAres")
                return 103;
            if (Name == "RockMonsterL17")
                return 105;
            if (Name == "RockMonsterL42")
                return 106;
            if (Name == "RockMonsterL72")
                return 107;
            if (Name == "RockMonsterL92")
                return 108;
            if (Name == "WindSnake")
                return 110;
            if (Name == "AlienBandit")
                return 111;
            if (Name == "FireRat")
                return 112;
            if (Name == "BlazerSpirit")
                return 113;
            if (Name == "MinMacaque")
                return 120;
            if (Name == "ElfApe")
                return 121;
            if (Name == "SlowApe")
                return 122;
            if (Name == "SnakeMonster")
                return 123;
            if (Name == "SandElf")
                return 124;
            if (Name == "LapidatingMob")
                return 125;
            if (Name == "ToughMonster")
                return 126;
            if (Name == "EvilBlade")
                return 127;
            if (Name == "DevilKingL120")
                return 200;
            if (Name == "DevilKingL122")
                return 201;
            if (Name == "Guard1")
                return 900;
            if (Name == "Guard2")
                return 910;
            if (Name == "Guard2")
                return 911;
            if (Name == "HugeSnake")
                return 1400;
            if (Name == "BanditLeader")
                return 1401;
            if (Name == "HugeSpirit")
                return 1402;
            if (Name == "CateranSoldier")
                return 1403;
            if (Name == "SeniorCateran")
                return 1404;
            if (Name == "CateranLeader")
                return 1405;
            if (Name == "ChiefCateran")
                return 1406;
            if (Name == "HugeApe")
                return 1407;
            if (Name == "SeniorApe")
                return 1408;
            if (Name == "AlienApe")
                return 1409;
            if (Name == "SeniorSnakeman")
                return 1410;
            if (Name == "Serpent")
                return 1411;
            if (Name == "SeniorSerpent")
                return 1412;
            if (Name == "AlienSerpent")
                return 1413;
            if (Name == "Basilisk")
                return 1414;
            if (Name == "Goblin")
                return 1610;
            if (Name == "Robot")
                return 2000;
            if (Name == "Robot")
                return 2001;
            if (Name == "Robot")
                return 2002;
            if (Name == "Robot")
                return 2003;
            if (Name == "WaterElf")
                return 2005;
            if (Name == "WaterElf")
                return 2006;
            if (Name == "WaterElf")
                return 2007;
            if (Name == "WaterElf")
                return 2008;
            if (Name == "WaterElf")
                return 2009;
            if (Name == "Robot")
                return 2010;
            if (Name == "Robot")
                return 2011;
            if (Name == "Robot")
                return 2012;
            if (Name == "Robot")
                return 2013;
            if (Name == "DivineHare")
                return 2020;
            if (Name == "DivineHare")
                return 2021;
            if (Name == "DivineHare")
                return 2022;
            if (Name == "DivineHare")
                return 2023;
            if (Name == "DivineHare")
                return 2024;
            if (Name == "NightDevil")
                return 2030;
            if (Name == "NightDevil")
                return 2031;
            if (Name == "NightDevil")
                return 2032;
            if (Name == "NightDevil")
                return 2033;
            if (Name == "NightDevil")
                return 2034;
            if (Name == "WaterElf")
                return 2040;
            if (Name == "WaterElf")
                return 2041;
            if (Name == "WaterElf")
                return 2042;
            if (Name == "WaterElf")
                return 2043;
            if (Name == "Pheasant")
                return 2100;
            if (Name == "Robin")
                return 2101;
            if (Name == "Apparition")
                return 2102;
            if (Name == "DisGoblin")
                return 2104;
            if (Name == "UndeadPuppet")
                return 2105;
            if (Name == "GhostWolf")
                return 2106;
            if (Name == "DisMessenger")
                return 2107;
            if (Name == "Phantom")
                return 2108;
            if (Name == "FlyingDemon")
                return 2109;
            if (Name == "Bat")
                return 2131;
            if (Name == "Mink")
                return 2132;
            if (Name == "ArenaSentinel1")
                return 2133;
            if (Name == "ArenaSentinel2")
                return 2134;
            if (Name == "ArenaSentinel3")
                return 2135;
            if (Name == "ArenaSentinel4")
                return 2136;
            if (Name == "ArenaSentinel5")
                return 2137;
            if (Name == "ArenaSentinel6")
                return 2138;
            if (Name == "ArenaSentinel7")
                return 2139;
            if (Name == "CastleBandit")
                return 2140;
            if (Name == "CloudtheLustful")
                return 2141;
            if (Name == "Gangster")
                return 2142;
            if (Name == "Vulture")
                return 2143;
            if (Name == "Devil")
                return 2146;
            if (Name == "VenomousApe")
                return 2147;
            if (Name == "ApeLeader")
                return 2148;
            if (Name == "JadePhantom")
                return 2149;
            if (Name == "KidCatcher")
                return 2150;
            if (Name == "KidHunter")
                return 2151;
            if (Name == "KidSiezer")
                return 2152;
            if (Name == "FishElf")
                return 2153;
            if (Name == "MouseElf")
                return 2154;
            if (Name == "KidRipper")
                return 2155;
            if (Name == "TroutElf")
                return 2156;
            if (Name == "BloodyShawn")
                return 2157;
            if (Name == "ViciousRat")
                return 2158;
            if (Name == "MausoleumGeneral1")
                return 2159;
            if (Name == "YangFeng")
                return 2160;
            if (Name == "MausoleumGeneral2")
                return 2161;
            if (Name == "MausoleumSentry")
                return 2162;
            if (Name == "CapriceLeader")
                return 2163;
            if (Name == "TombBandit")
                return 2164;
            if (Name == "TombBanditLeader")
                return 2165;
            if (Name == "MausoleumGeneral3")
                return 2166;
            if (Name == "Viper")
                return 2167;
            if (Name == "Brigand")
                return 2168;
            if (Name == "CrimsonClaw")
                return 2169;
            if (Name == "CrimsonViper")
                return 2170;
            if (Name == "JadeCourtesan")
                return 2171;
            if (Name == "PoemGhost")
                return 2172;
            if (Name == "LyraGhost")
                return 2173;
            if (Name == "MausoleumGeneral4")
                return 2178;
            if (Name == "DesertCondor")
                return 2179;
            if (Name == "EvilSpirit")
                return 2180;
            if (Name == "JinxTombBat")
                return 2189;
            if (Name == "BanditLeaderL97")
                return 2224;
            if (Name == "BanditCourier")
                return 2225;
            if (Name == "BanditDeliverer")
                return 2226;
            if (Name == "MadBull")
                return 2231;
            if (Name == "BullMaster")
                return 2232;
            if (Name == "WeirdPumpkin")
                return 2237;
            if (Name == "LittleWhirl")
                return 2324;
            if (Name == "BetterLuck")
                return 2325;
            if (Name == "BetterFly")
                return 2326;
            if (Name == "VeryLoud")
                return 2327;
            if (Name == "IronHead")
                return 2328;
            if (Name == "Happy")
                return 2329;
            if (Name == "YellowOxGuard")
                return 2340;
            if (Name == "HeavenOx")
                return 2351;
            if (Name == "GoldenOx")
                return 2352;
            if (Name == "SilverOx")
                return 2353;
            if (Name == "RedFox")
                return 2354;
            if (Name == "BlueFox")
                return 2355;
            if (Name == "Guard2")
                return 2367;
            if (Name == "MaleGhost")
                return 2380;
            if (Name == "CaveThief")
                return 2390;
            if (Name == "ForestThief")
                return 2391;
            if (Name == "DesertThief")
                return 2392;
            if (Name == "SerpentEnvoy")
                return 2410;
            if (Name == "IcySerpent")
                return 2411;
            if (Name == "SerpentLord")
                return 2412;
            if (Name == "SerpentKing")
                return 2413;
            if (Name == "FrostSerpent")
                return 2414;
            if (Name == "BladeDevilEnvoy")
                return 2415;
            if (Name == "IcyBladeDevil")
                return 2416;
            if (Name == "BladeDevilLord")
                return 2417;
            if (Name == "BladeDevilKing")
                return 2418;
            if (Name == "FrostBladeDevil")
                return 2419;
            if (Name == "Demon")
                return 2420;
            if (Name == "AncientDemon")
                return 2421;
            if (Name == "FloodDemon")
                return 2422;
            if (Name == "MaleGhost1")
                return 2430;
            if (Name == "FemaleGhost1")
                return 2431;
            if (Name == "HeavenDemon")
                return 2435;
            if (Name == "ChaosDemon")
                return 2436;
            if (Name == "SacredDemon")
                return 2437;
            if (Name == "AuroraDemon")
                return 2438;
            if (Name == "RockBehemoth")
                return 2440;
            if (Name == "RockLeader")
                return 2441;
            if (Name == "RockLackey")
                return 2442;
            if (Name == "GuardPotion")
                return 2443;
            if (Name == "SluggishPotion")
                return 2445;
            if (Name == "DizzyHammer")
                return 2446;
            if (Name == "RestorePotion")
                return 2447;
            if (Name == "ScreamBomb")
                return 2448;
            if (Name == "SuperExclamationMark")
                return 2449;
            if (Name == "SuperQuestionMark")
                return 2450;
            if (Name == "SpiritPotion")
                return 2451;
            if (Name == "ExcitementPotion")
                return 2452;
            if (Name == "ChaosBomb")
                return 2453;
            if (Name == "SuperExcitementPotion")
                return 2454;
            if (Name == "DarkLady")
                return 2460;
            if (Name == "DarkElf")
                return 2461;
            if (Name == "NightmareLady")
                return 2465;
            if (Name == "GrottoLady")
                return 2466;
            if (Name == "SnakeChief")
                return 2473;
            if (Name == "SnakeHubbub")
                return 2474;
            if (Name == "HeadlessGeneral")
                return 2478;
            if (Name == "HeadlessSoldier")
                return 2479;
            if (Name == "Merchant")
                return 2493;
            if (Name == "BanditMinion")
                return 2494;
            if (Name == "MightyBandit")
                return 2495;
            if (Name == "Macaque1")
                return 2611;
            if (Name == "GiantApe1")
                return 2612;
            if (Name == "SandMonster1")
                return 2613;
            if (Name == "RockMonster1")
                return 2614;
            if (Name == "EvilMerman")
                return 2615;
            if (Name == "FlyingSnake")
                return 2616;
            if (Name == "MutatedDragon")
                return 2617;
            if (Name == "BattleScorpion")
                return 2618;
            if (Name == "RagingBull")
                return 2619;
            if (Name == "RoseFinch")
                return 2620;
            if (Name == "FreakStalker")
                return 2621;
            if (Name == "GrislyBat")
                return 2622;
            if (Name == "GhostGeneral")
                return 2623;
            if (Name == "FrostBanshee")
                return 2624;
            if (Name == "ChaoticWizard")
                return 2625;
            if (Name == "AvianFighter")
                return 2626;
            if (Name == "SoulEater")
                return 2627;
            if (Name == "DreamEater")
                return 2628;
            if (Name == "FallenGiant")
                return 2629;
            if (Name == "Deer")
                return 2680;
            if (Name == "Wraith")
                return 2684;
            if (Name == "WraithLord")
                return 2685;
            if (Name == "OceanGhost")
                return 2686;
            if (Name == "DemonGhost")
                return 2687;
            if (Name == "SilverOctopus")
                return 2699;
            if (Name == "GoldenOctopus")
                return 2700;
            if (Name == "SuperQuestionMark")
                return 2712;
            if (Name == "PhantomBeast")
                return 2738;
            if (Name == "VolcanoBeast")
                return 2739;
            if (Name == "ChaosGuard")
                return 2751;
            if (Name == "ThiefLeader")
                return 2996;
            if (Name == "PlainsThief")
                return 2997;
            if (Name == "OddMonster")
                return 3000;
            if (Name == "NorthMonster")
                return 3001;
            if (Name == "SouthMonster")
                return 3002;
            if (Name == "WestMonster")
                return 3003;
            if (Name == "EastMonster")
                return 3004;
            if (Name == "FuryGhost")
                return 3005;
            if (Name == "CrazyGhost")
                return 3006;
            if (Name == "RoaringGhost")
                return 3007;
            if (Name == "ScaryGhost")
                return 3008;
            if (Name == "BloodGhost")
                return 3009;
            if (Name == "IronBird")
                return 3010;
            if (Name == "CopperBird")
                return 3011;
            if (Name == "SilverBird")
                return 3012;
            if (Name == "GoldenBird")
                return 3013;
            if (Name == "FastBird")
                return 3014;
            if (Name == "BrutalBandit")
                return 3015;
            if (Name == "WildBandit")
                return 3016;
            if (Name == "CruelBandit")
                return 3017;
            if (Name == "SingleEye")
                return 3018;
            if (Name == "TyrantBandit")
                return 3019;
            if (Name == "AberrantEvil")
                return 3020;
            if (Name == "CecropsBoss")
                return 3021;
            if (Name == "Minotaur")
                return 3022;
            if (Name == "Jumpy")
                return 3023;
            if (Name == "EvilMonkey")
                return 3024;
            if (Name == "Pan")
                return 3025;
            if (Name == "EvilHawk")
                return 3026;
            if (Name == "GlumMonster")
                return 3027;
            if (Name == "ShakeMonster")
                return 3028;
            if (Name == "IronEagle")
                return 3029;
            if (Name == "Cateran")
                return 3031;
            if (Name == "PoisonousRat")
                return 3032;
            if (Name == "StoneBandit")
                return 3033;
            if (Name == "FowlEmpress")
                return 3037;
            if (Name == "SugarplumFairy")
                return 3039;
            if (Name == "RatKing1")
                return 3042;
            if (Name == "FireRat1")
                return 3043;
            if (Name == "ToxicRat1")
                return 3044;
            if (Name == "HeresySnakeman")
                return 3050;
            if (Name == "L50DevilClone")
                return 5951;
            if (Name == "L60DevilClone")
                return 5952;
            if (Name == "L70DevilClone")
                return 5953;
            if (Name == "L80DevilClone")
                return 5954;
            if (Name == "PumpkinMonster")
                return 3058;
            if (Name == "PumpkinMonster2")
                return 3059;
            if (Name == "IceGhoul")
                return 3066;
            if (Name == "StoneGhoul")
                return 3067;
            if (Name == "PointGhoul")
                return 3068;
            if (Name == "RayGhoul")
                return 3069;
            if (Name == "GreyGhoul")
                return 3070;
            if (Name == "BloodGhoul")
                return 3071;
            if (Name == "BloomGhoul")
                return 3072;
            if (Name == "IceMinion")
                return 3073;
            if (Name == "StoneMinion")
                return 3074;
            if (Name == "PointMinion")
                return 3075;
            if (Name == "RayMinion")
                return 3076;
            if (Name == "GreyMinion")
                return 3077;
            if (Name == "BloodMinion")
                return 3078;
            if (Name == "BloomMinion")
                return 3079;
            if (Name == "DivineHare")
                return 3081;
            if (Name == "GoldenHare")
                return 3082;
            if (Name == "SilverHare")
                return 3083;
            if (Name == "HareGuard")
                return 3084;
            if (Name == "FireSnakeSpirit")
                return 3100;
            if (Name == "ToughSnake")
                return 3101;
            if (Name == "SnakeKing")
                return 3102;
            if (Name == "WaterSnake")
                return 3103;
            if (Name == "GuildBeast")
                return 3120;
            if (Name == "GanodermaL72")
                return 3130;
            if (Name == "GanodermaL70")
                return 3131;
            if (Name == "GanodermaL74")
                return 3132;
            if (Name == "GanodermaL76")
                return 3133;
            if (Name == "TitanL77")
                return 3134;
            if (Name == "TitanL75")
                return 3135;
            if (Name == "TitanL79")
                return 3136;
            if (Name == "TitanL81")
                return 3137;
            if (Name == "Slinger")
                return 3141;
            if (Name == "GoldGhost")
                return 3142;
            if (Name == "Gibbon")
                return 3143;
            if (Name == "Bladeling")
                return 3144;
            if (Name == "AgileRat")
                return 3145;
            if (Name == "NagaLord")
                return 3146;
            if (Name == "BlueBird")
                return 3147;
            if (Name == "FiendBat")
                return 3148;
            if (Name == "Talon")
                return 3149;
            if (Name == "MinotaurL120")
                return 3155;
            if (Name == "Howler")
                return 3156;
            if (Name == "CuteBunny")
                return 3310;
            if (Name == "PlayfulBunny")
                return 3311;
            if (Name == "CuteBunny")
                return 3312;
            if (Name == "KungFuBunny")
                return 3320;
            if (Name == "Piglet")
                return 3321;
            if (Name == "KungFuBunny")
                return 3322;
            if (Name == "StoneMonster")
                return 3560;
            if (Name == "Turkey")
                return 3561;
            if (Name == "800poundTurkey")
                return 3562;
            if (Name == "FireSnake")
                return 3563;
            if (Name == "SandMonster2")
                return 3564;
            if (Name == "FishA")
                return 3579;
            if (Name == "MysticLady")
                return 3580;
            if (Name == "BlueCock")
                return 3582;
            if (Name == "MagentaCock")
                return 3583;
            if (Name == "Siren")
                return 3584;
            if (Name == "BrownCock")
                return 3585;
            if (Name == "FishB")
                return 3586;
            if (Name == "ValeDemon")
                return 3600;
            if (Name == "MudDemon")
                return 3601;
            if (Name == "AbyssDemon")
                return 3602;
            if (Name == "ShadowKing")
                return 3603;
            if (Name == "SingingSerpent")
                return 3604;
            if (Name == "RoaringSerpent")
                return 3605;
            if (Name == "CryingSerpent")
                return 3606;
            if (Name == "SerpentSpirit")
                return 3607;
            if (Name == "BlueFiend")
                return 3608;
            if (Name == "RedFiend")
                return 3609;
            if (Name == "WhiteFiend")
                return 3610;
            if (Name == "FiendLord")
                return 3611;
            if (Name == "WindApe")
                return 3612;
            if (Name == "RainApe")
                return 3613;
            if (Name == "LightningApe")
                return 3614;
            if (Name == "FuryApe")
                return 3615;
            if (Name == "WoodHades")
                return 3616;
            if (Name == "WaterHades")
                return 3617;
            if (Name == "EarthHades")
                return 3618;
            if (Name == "HadesLord")
                return 3619;
            if (Name == "FuryBat")
                return 3620;
            if (Name == "CruelBat")
                return 3621;
            if (Name == "ViciousBat")
                return 3622;
            if (Name == "DemonBat")
                return 3623;
            if (Name == "BoneSkeleton")
                return 3624;
            if (Name == "LameSkeleton")
                return 3625;
            if (Name == "CarrionSkeleton")
                return 3626;
            if (Name == "FurySkeleton")
                return 3627;
            if (Name == "AngryBeast")
                return 3628;
            if (Name == "SensitiveBeast")
                return 3629;
            if (Name == "CrazyBeast")
                return 3630;
            if (Name == "FearlessBeast")
                return 3631;
            if (Name == "HillSpirit")
                return 3632;
            if (Name == "SwiftDevil")
                return 3633;
            if (Name == "Banshee")
                return 3634;
            if (Name == "CleansingDevil")
                return 3635;
            if (Name == "Andrew")
                return 3636;
            if (Name == "Peter")
                return 3637;
            if (Name == "Philip")
                return 3638;
            if (Name == "Timothy")
                return 3639;
            if (Name == "Daphne")
                return 3640;
            if (Name == "Victoria")
                return 3641;
            if (Name == "Wayne")
                return 3642;
            if (Name == "Theodore")
                return 3643;
            if (Name == "Satan")
                return 3644;
            if (Name == "BeastSatan")
                return 3645;
            if (Name == "FurySatan")
                return 3646;
            if (Name == "PurpleRooster")
                return 3706;
            if (Name == "GreenRooster")
                return 3707;
            if (Name == "BlueRooster")
                return 3708;
            if (Name == "MagentaRooster")
                return 3709;
            if (Name == "CrimeWraith")
                return 4020;
            if (Name == "BloodyPluto")
                return 4021;
            if (Name == "InfernoPluto")
                return 4022;
            if (Name == "StormPluto")
                return 4023;
            if (Name == "VolcanoPluto")
                return 4024;
            if (Name == "CyclonePluto")
                return 4025;
            if (Name == "RockPluto")
                return 4026;
            if (Name == "PhantomPluto")
                return 4027;
            if (Name == "EvilPluto")
                return 4028;
            if (Name == "UltimatePluto")
                return 4030;
            if (Name == "FurySkeleton")
                return 4031;
            if (Name == "ElfKing1")
                return 4032;
            if (Name == "BloodyBat1")
                return 4033;
            if (Name == "HillMonsterKin")
                return 4034;
            if (Name == "BirdKing1")
                return 4035;
            if (Name == "WingedSnakeKin")
                return 4036;
            if (Name == "PinkPiglet")
                return 4058;
            if (Name == "PinkPiglet")
                return 4060;
            if (Name == "Vampire")
                return 4082;
            if (Name == "Vampire")
                return 4083;
            if (Name == "Vampire")
                return 4084;
            if (Name == "Vampire")
                return 4085;
            if (Name == "Vampire")
                return 4086;
            if (Name == "Vampire")
                return 4087;
            if (Name == "Vampire")
                return 4088;
            if (Name == "ChickenThief")
                return 4090;
            if (Name == "Locust")
                return 4091;
            if (Name == "CranberryRobber")
                return 4104;
            if (Name == "DisguisedBandit")
                return 4109;
            if (Name == "ThunderAuk")
                return 4123;
            if (Name == "RockingEvil")
                return 4124;
            if (Name == "RamaSpook")
                return 4125;
            if (Name == "DementedSoul")
                return 4126;
            if (Name == "Executioner")
                return 4127;
            if (Name == "SharpBeast")
                return 4128;
            if (Name == "SoulCrusher")
                return 4129;
            if (Name == "FireHawk")
                return 4130;
            if (Name == "BlazeDevil")
                return 4131;
            if (Name == "SkeletonWarrior")
                return 4132;
            if (Name == "Lucifer")
                return 4133;
            if (Name == "FrostSnake")
                return 4134;
            if (Name == "FireRatKing")
                return 4135;
            if (Name == "SoulDrainer")
                return 4136;
            if (Name == "DreadThunder")
                return 4137;
            if (Name == "BloodyVampire")
                return 4138;
            if (Name == "DarkDelilah")
                return 4139;
            if (Name == "ChaosOx")
                return 4140;
            if (Name == "MoonShade")
                return 4141;
            if (Name == "HeavenTiger")
                return 4144;
            if (Name == "GoldenTiger")
                return 4145;
            if (Name == "SilverTiger")
                return 4146;
            if (Name == "TigerRoarer")
                return 4147;
            if (Name == "LavaBeast")
                return 4151;
            if (Name == "TeratoDragon")
                return 4152;
            if (Name == "SwordMaster")
                return 4170;
            if (Name == "SnowBanshee")
                return 4171;
            if (Name == "ThrillingSpook")
                return 4172;
            if (Name == "GrislySpecter")
                return 4173;
            if (Name == "EscapedRatling")
                return 4174;
            if (Name == "RatlingKing")
                return 4175;
            if (Name == "ApeTyrant")
                return 4176;
            if (Name == "SnakemanElder")
                return 4177;
            if (Name == "GiantMacaque")
                return 4178;
            if (Name == "HeresyElder")
                return 4179;
            if (Name == "Webster`sApe")
                return 4180;
            if (Name == "WearyApeL58")
                return 4181;
            if (Name == "RagingApe")
                return 4182;
            if (Name == "WearyRagingApe")
                return 4183;
            if (Name == "HeresySnakeman1")
                return 4185;
            if (Name == "WearyGiantApe")
                return 4186;
            if (Name == "RobberLeader")
                return 4187;
            if (Name == "WearyApeL53")
                return 4192;
            if (Name == "WearyApeKing")
                return 4193;
            if (Name == "Yaksa")
                return 4194;
            if (Name == "AoBing")
                return 4195;
            if (Name == "DragonKing")
                return 4196;
            if (Name == "AngryBladeGhost")
                return 4197;
            if (Name == "HawkLeader")
                return 4198;
            if (Name == "RagingApe")
                return 4199;
            if (Name == "SavageApe")
                return 4200;
            if (Name == "FuriousApe")
                return 4201;
            if (Name == "RagingMacaque")
                return 4202;
            if (Name == "SavageMonkey")
                return 4203;
            if (Name == "FlameApe")
                return 4204;
            if (Name == "HollowBeast")
                return 4211;
            if (Name == "ThrillingSpook")
                return 4212;
            if (Name == "NemesisTyrant")
                return 4220;
            if (Name == "UndeadSoldier")
                return 5045;
            if (Name == "Phantom")
                return 5046;
            if (Name == "UndeadSpearman")
                return 5049;
            if (Name == "Revenant")
                return 5050;
            if (Name == "Eidolon")
                return 5051;
            if (Name == "Temptress")
                return 5053;
            if (Name == "Centicore")
                return 5054;
            if (Name == "HellWraith")
                return 5055;
            if (Name == "HellTroll")
                return 5056;
            if (Name == "Naga")
                return 5057;
            if (Name == "Syren")
                return 5058;
            if (Name == "UltimatePluto")
                return 5059;
            if (Name == "HellWraith")
                return 5060;
            if (Name == "ToughHorn")
                return 6000;
            if (Name == "ToughHorn")
                return 6001;
            if (Name == "ToughHorn")
                return 6002;
            if (Name == "ToughHorn")
                return 6003;
            if (Name == "ToughHorn")
                return 6004;
            if (Name == "ToughHorn")
                return 6005;
            if (Name == "ToughHorn")
                return 6006;
            if (Name == "LonelyTyrant")
                return 6010;
            if (Name == "Ghost")
                return 6011;
            if (Name == "MonkeyMonster")
                return 6015;
            if (Name == "Cecrops")
                return 6016;
            if (Name == "ApeMonster")
                return 6017;
            if (Name == "Python")
                return 6018;
            if (Name == "DemonicFlagman")
                return 6030;
            if (Name == "WolfPackLord")
                return 6031;
            if (Name == "FallenOrnithoid")
                return 6032;
            if (Name == "DevilSentry")
                return 7000;
            if (Name == "WarriorDevil")
                return 7001;
            if (Name == "TrojanDevil")
                return 7002;
            if (Name == "ArcherDevil")
                return 7003;
            if (Name == "FireDevil")
                return 7004;
            if (Name == "WaterDevil")
                return 7005;
            if (Name == "CoinsStealer")
                return 7022;
            if (Name == "RedFox")
                return 7026;
            if (Name == "AlienBat")
                return 7030;
            if (Name == "BloodyRat")
                return 7031;
            if (Name == "CyanBat")
                return 7032;
            if (Name == "Macaque(E)")
                return 7043;
            if (Name == "SmartMonkey(E)")
                return 7044;
            if (Name == "SandMonster(E)")
                return 7045;
            if (Name == "SandDevil(E)")
                return 7046;
            if (Name == "Birdman(E)")
                return 7047;
            if (Name == "BirdKing(E)")
                return 7048;
            if (Name == "TombBat(E)")
                return 7049;
            if (Name == "FlyingBat(E)")
                return 7050;
            if (Name == "SwordMaster(E)")
                return 7051;
            if (Name == "ThrillingSpookE")
                return 7052;
            if (Name == "Macaque(E)")
                return 7055;
            if (Name == "Macaque(E)")
                return 7056;
            if (Name == "Macaque(E)")
                return 7057;
            if (Name == "Macaque(E)")
                return 7058;
            if (Name == "SmartMonkey(E)")
                return 7059;
            if (Name == "SmartMonkey(E)")
                return 7060;
            if (Name == "SmartMonkey(E)")
                return 7061;
            if (Name == "SmartMonkey(E)")
                return 7062;
            if (Name == "SandMonster(E)")
                return 7063;
            if (Name == "SandMonster(E)")
                return 7064;
            if (Name == "SandMonster(E)")
                return 7065;
            if (Name == "SandMonster(E)")
                return 7066;
            if (Name == "SandDevil(E)")
                return 7067;
            if (Name == "SandDevil(E)")
                return 7068;
            if (Name == "SandDevil(E)")
                return 7069;
            if (Name == "SandDevil(E)")
                return 7070;
            if (Name == "Birdman(E)")
                return 7071;
            if (Name == "Birdman(E)")
                return 7072;
            if (Name == "Birdman(E)")
                return 7073;
            if (Name == "Birdman(E)")
                return 7074;
            if (Name == "BirdKing(E)")
                return 7075;
            if (Name == "BirdKing(E)")
                return 7076;
            if (Name == "BirdKing(E)")
                return 7077;
            if (Name == "BirdKing(E)")
                return 7078;
            if (Name == "TombBat(E)")
                return 7079;
            if (Name == "TombBat(E)")
                return 7080;
            if (Name == "TombBat(E)")
                return 7081;
            if (Name == "TombBat(E)")
                return 7082;
            if (Name == "FlyingBat(E)")
                return 7083;
            if (Name == "FlyingBat(E)")
                return 7084;
            if (Name == "FlyingBat(E)")
                return 7085;
            if (Name == "FlyingBat(E)")
                return 7086;
            if (Name == "SwordMaster(E)")
                return 7087;
            if (Name == "SwordMaster(E)")
                return 7088;
            if (Name == "SwordMaster(E)")
                return 7089;
            if (Name == "SwordMaster(E)")
                return 7090;
            if (Name == "ThrillingSpookE")
                return 7091;
            if (Name == "ThrillingSpookE")
                return 7092;
            if (Name == "ThrillingSpookE")
                return 7093;
            if (Name == "ThrillingSpookE")
                return 7094;
            if (Name == "GoldGhost.Lv108")
                return 7103;
            if (Name == "Slinger.Lv101")
                return 7104;
            if (Name == "Bladeling.Lv110")
                return 7105;
            if (Name == "AgileRat.Lv112")
                return 7106;
            if (Name == "BlueBird.Lv115")
                return 7107;
            if (Name == "FiendBat.Lv117")
                return 7108;
            if (Name == "Minotaur.Lv120")
                return 7109;
            if (Name == "FurySerpent")
                return 7110;
            if (Name == "L93Brigand")
                return 7115;
            if (Name == "L98Robber")
                return 7116;
            if (Name == "L103Aggressor")
                return 7117;
            if (Name == "L108Robot")
                return 7118;
            if (Name == "L113BloodPirate")
                return 7119;
            if (Name == "L118CruelPirate")
                return 7120;
            if (Name == "L123FierceRonin")
                return 7121;
            if (Name == "L128FuryRonin")
                return 7122;
            if (Name == "L132Puppet")
                return 7123;
            if (Name == "L135SoulWarrior")
                return 7124;
            if (Name == "Demon1")
                return 7152;
            if (Name == "AncientDemon")
                return 7153;
            if (Name == "FloodDemon")
                return 7154;
            if (Name == "HeavenDemon")
                return 7155;
            if (Name == "ChaosDemon")
                return 7156;
            if (Name == "NaughtyMonkey")
                return 7224;
            if (Name == "CommonIronChest(Normal)")
                return 7750;
            if (Name == "EliteIronChest(Normal)")
                return 7751;
            if (Name == "JoyFox")
                return 7285;
            if (Name == "TreasureFox")
                return 7286;
            if (Name == "SandMonsterS1")
                return 7288;
            if (Name == "BloodyDevilS1")
                return 7289;
            if (Name == "HawkDefender")
                return 7316;
            if (Name == "GhostReaver")
                return 7483;
            if (Name == "EvilMonkMisery")
                return 7484;
            if (Name == "FlameDevastator")
                return 7485;
            if (Name == "AwakeFlameDevastator")
                return 7486;
            if (Name == "FuriousFlameDevastator")
                return 7487;
            if (Name == "CitySoldier")
                return 7488;
            if (Name == "LoveFox")
                return 7554;
            if (Name == "CranePrincess")
                return 7555;
            if (Name == "FairySorceress")
                return 7556;
            if (Name == "WarPanda")
                return 7557;
            if (Name == "PandaBattlemaster")
                return 7558;
            if (Name == "VoltaicWarg")
                return 7559;
            if (Name == "LavaTiger")
                return 7560;
            if (Name == "GoldBox")
                return 7562;
            if (Name == "NightmareCaptain")
                return 7563;
            if (Name == "PurpleBanshee")
                return 7565;
            if (Name == "ShadowClone")
                return 7570;
            if (Name == "AxeApparition")
                return 7608;
            if (Name == "IronChariot")
                return 7609;
            if (Name == "Horse-facedHead")
                return 7610;
            if (Name == "UndeadWarrior")
                return 7611;
            if (Name == "UndeadSpearman")
                return 7612;
            if (Name == "UndeadCommander")
                return 7613;
            if (Name == "GiantAxeDemon")
                return 7614;
            if (Name == "TwinAxeBeast")
                return 7615;
            if (Name == "DarkSerpentKing")
                return 7616;
            if (Name == "PsychicChest")
                return 7617;
            if (Name == "Ripper")
                return 7714;
            if (Name == "ChaosRipper")
                return 7715;
            if (Name == "SuperChaosDefender")
                return 7722;
            if (Name == "ChaosIncarnation")
                return 7731;
            if (Name == "HumanMessenger")
                return 8100;
            if (Name == "WingedSnakeMsgr")
                return 8101;
            if (Name == "BanditMessenger")
                return 8102;
            if (Name == "RatMessenger")
                return 8103;
            if (Name == "EidolonEnvoy")
                return 8104;
            if (Name == "MonkeyMessenger")
                return 8105;
            if (Name == "GiantApeMsgr")
                return 8106;
            if (Name == "ThunderApeMsgr")
                return 8107;
            if (Name == "SnakemanMsgr")
                return 8108;
            if (Name == "SandMonsterMsgr")
                return 8109;
            if (Name == "HillMonsterMsgr")
                return 8110;
            if (Name == "RockMonsterMsgr")
                return 8111;
            if (Name == "BladeGhostMsgr")
                return 8112;
            if (Name == "BirdmanMsgr")
                return 8113;
            if (Name == "HawkMessenger")
                return 8114;
            if (Name == "RobberMessenger")
                return 8115;
            if (Name == "BatMessenger")
                return 8116;
            if (Name == "BloodyBatMsgr")
                return 8117;
            if (Name == "BullMessenger")
                return 8118;
            if (Name == "DevilMessenger")
                return 8119;
            if (Name == "HumanAide")
                return 8200;
            if (Name == "WingedSnakeAide")
                return 8201;
            if (Name == "BanditAide")
                return 8202;
            if (Name == "RatAide")
                return 8203;
            if (Name == "EidolonWarden")
                return 8204;
            if (Name == "MonkeyAide")
                return 8205;
            if (Name == "GiantApeAide")
                return 8206;
            if (Name == "ThunderApeAide")
                return 8207;
            if (Name == "SnakemanAide")
                return 8208;
            if (Name == "SandMonsterAide")
                return 8209;
            if (Name == "HillMonsterAide")
                return 8210;
            if (Name == "RockMonsterAide")
                return 8211;
            if (Name == "BladeGhostAide")
                return 8212;
            if (Name == "BirdmanAide")
                return 8213;
            if (Name == "HawkAide")
                return 8214;
            if (Name == "RobberAide")
                return 8215;
            if (Name == "BatAide")
                return 8216;
            if (Name == "BloodyBatAide")
                return 8217;
            if (Name == "BullAide")
                return 8218;
            if (Name == "DevilAide")
                return 8219;
            if (Name == "GhostKing")
                return 8300;
            if (Name == "WingedSnakeKing")
                return 8301;
            if (Name == "BanditKing")
                return 8302;
            if (Name == "RatKing")
                return 8303;
            if (Name == "EidolonKing")
                return 8304;
            if (Name == "MonkeyKing")
                return 8305;
            if (Name == "GiantApeKing")
                return 8306;
            if (Name == "ThunderApeKing")
                return 8307;
            if (Name == "SnakemanKing")
                return 8308;
            if (Name == "SandMonsterKing")
                return 8309;
            if (Name == "HillMonsterKing")
                return 8310;
            if (Name == "RockMonsterKing")
                return 8311;
            if (Name == "BladeGhostKing")
                return 8312;
            if (Name == "BirdmanKing")
                return 8313;
            if (Name == "HawkBoss")
                return 8314;
            if (Name == "RobberKing")
                return 8315;
            if (Name == "BatKing")
                return 8316;
            if (Name == "BloodyWing")
                return 8317;
            if (Name == "BullKing")
                return 8318;
            if (Name == "EvilLord")
                return 8319;
            if (Name == "MeteorDove")
                return 8415;
            if (Name == "MeteorDove")
                return 8416;
            if (Name == "EvilGhost")
                return 8417;
            if (Name == "NightDevil")
                return 8418;
            if (Name == "WaterDevil")
                return 8419;
            if (Name == "WaterDevil")
                return 8420;
            if (Name == "WaterDevil")
                return 8421;
            if (Name == "WaterDevil")
                return 8422;
            if (Name == "WaterLord")
                return 8500;
            if (Name == "IronEscort")
                return 9000;
            if (Name == "CopperEscort")
                return 9001;
            if (Name == "SilverEscort")
                return 9002;
            if (Name == "GoldEscort")
                return 9003;
            if (Name == "GhostBat")
                return 9005;
            if (Name == "FastBat")
                return 9006;
            if (Name == "SwiftBat")
                return 9007;
            if (Name == "MagicBat")
                return 9008;
            if (Name == "GhostBatBoss")
                return 9010;
            if (Name == "FastBatBoss")
                return 9011;
            if (Name == "SwiftBatBoss")
                return 9012;
            if (Name == "MagicBatBoss")
                return 9013;
            if (Name == "EvilBatA")
                return 9015;
            if (Name == "EvilBatB")
                return 9016;
            if (Name == "EvilBatC")
                return 9017;
            if (Name == "EvilBatD")
                return 9018;
            if (Name == "FireRatA")
                return 9020;
            if (Name == "FireRatB")
                return 9021;
            if (Name == "FireRatC")
                return 9022;
            if (Name == "FireRatD")
                return 9023;
            if (Name == "SkeletonA")
                return 9025;
            if (Name == "SkeletonB")
                return 9026;
            if (Name == "SkeletonC")
                return 9027;
            if (Name == "SkeletonD")
                return 9028;
            if (Name == "BigCrystal")
                return 7860;
            if (Name == "SmallCrystal")
                return 7861;
            if (Name == "WarriorofRage")
                return 7854;
            if (Name == "ThunderDragon")
                return 7859;
            if (Name == "GuanYu")
                return 4421;
            if (Name == "AzureDragon")
                return 7885;
            if (Name == "WhiteTiger")
                return 7886;
            if (Name == "VermilionBird")
                return 7887;
            if (Name == "BlackTurtle")
                return 7888;
            if (Name == "AzureDragonDefender")
                return 7889;
            if (Name == "WhiteTigerDefender")
                return 7890;
            if (Name == "VermilionBirdDefender")
                return 7891;
            if (Name == "BlackTurtleDefender")
                return 7892;
            if (Name == "HungryGoblin")
                return 7881;
            if (Name == "Maniac")
                return 7882;
            if (Name == "AlluringWitch")
                return 7883;
            if (Name == "DarkCrystalofSky")
                return 7884;
            if (Name == "DarkCrystalofWind")
                return 7971;
            if (Name == "ThunderCrystal")
                return 7972;
            if (Name == "DarkCrystalofFire")
                return 7973;
            if (Name == "RealmSoldier")
                return 7896;
            if (Name == "MilitaryOfficerofRealm")
                return 7897;
            if (Name == "KylinChariot")
                return 7898;
            if (Name == "AzureDragon")
                return 7901;
            if (Name == "Lv.1GeneralCat")
                return 7903;
            if (Name == "Thundercloud")
                return 4264;
            if (Name == "SuperIronChest(Normal)")
                return 7752;
            if (Name == "VeteranTrojan")
                return 4255;
            if (Name == "EagleArcher")
                return 4256;
            if (Name == "DhyanaMonk")
                return 4257;
            if (Name == "FireTaoist")
                return 4258;
            if (Name == "ExpertDragonWarrior")
                return 4259;
            if (Name == "MiddleNinja")
                return 4260;
            if (Name == "PirateGunner")
                return 4261;
            if (Name == "BrassWarrior")
                return 4262;
            if (Name == "TigerTrojan")
                return 4281;
            if (Name == "TigerArcher")
                return 4282;
            if (Name == "DharmaMonk")
                return 4283;
            if (Name == "FireWizard")
                return 4284;
            if (Name == "EliteDragonWarrior")
                return 4285;
            if (Name == "DarkNinja")
                return 4286;
            if (Name == "Quartermaster")
                return 4287;
            if (Name == "SilverWarrior")
                return 4288;
            if (Name == "DragonTrojan")
                return 4289;
            if (Name == "DragonArcher")
                return 4290;
            if (Name == "PrajnaMonk")
                return 4291;
            if (Name == "FireMaster")
                return 4292;
            if (Name == "MasterDragonWarrior")
                return 4293;
            if (Name == "MysticNinja")
                return 4294;
            if (Name == "PirateCaptain")
                return 4295;
            if (Name == "GoldWarrior")
                return 4296;
            if (Name == "TrojanMaster")
                return 4297;
            if (Name == "ArcherMaster")
                return 4298;
            if (Name == "NirvanaMonk")
                return 4299;
            if (Name == "FireSaint")
                return 4300;
            if (Name == "KingDragonWarrior")
                return 4301;
            if (Name == "NinjaMaster")
                return 4302;
            if (Name == "PirateLord")
                return 4303;
            if (Name == "WarriorMaster")
                return 4304;
            if (Name == "EpicIronChest(Normal)")
                return 7753;
            if (Name == "CommonBronzeChest(Normal)")
                return 7754;
            if (Name == "EliteBronzeChest(Normal)")
                return 7755;
            if (Name == "SuperBronzeChest(Normal)")
                return 7756;
            if (Name == "EpicBronzeChest(Normal)")
                return 7757;
            if (Name == "CommonIronKnight(Normal)")
                return 7758;
            if (Name == "EliteIronKnight(Normal)")
                return 7759;
            if (Name == "SuperIronKnight(Normal)")
                return 7760;
            if (Name == "EpicIronKnight(Normal)")
                return 7761;
            if (Name == "CommonBronzeKnight(Normal)")
                return 7762;
            if (Name == "EliteBronzeKnight(Normal)")
                return 7763;
            if (Name == "SuperBronzeKnight(Normal)")
                return 7764;
            if (Name == "EpicBronzeKnight(Normal)")
                return 7765;
            if (Name == "PhantomofDarkKing")
                return 7766;
            if (Name == "PhantomofDarkLord")
                return 7767;
            if (Name == "CommonBronzeChest(Hard)")
                return 7768;
            if (Name == "EliteBronzeChest(Hard)")
                return 7769;
            if (Name == "SuperBronzeChest(Hard)")
                return 7770;
            if (Name == "EpicBronzeChest(Hard)")
                return 7771;
            if (Name == "CommonSilverChest(Hard)")
                return 7772;
            if (Name == "EliteSilverChest(Hard)")
                return 7773;
            if (Name == "SuperSilverChest(Hard)")
                return 7774;
            if (Name == "EpicSilverChest(Hard)")
                return 7775;
            if (Name == "CommonBronzeKnight(Hard)")
                return 7776;
            if (Name == "EliteBronzeKnight(Hard)")
                return 7777;
            if (Name == "SuperBronzeKnight(Hard)")
                return 7778;
            if (Name == "EpicBronzeKnight(Hard)")
                return 7779;
            if (Name == "CommonSilverKnight(Hard)")
                return 7780;
            if (Name == "EliteSilverKnight(Hard)")
                return 7781;
            if (Name == "SuperSilverKnight(Hard)")
                return 7782;
            if (Name == "EpicSilverKnight(Hard)")
                return 7783;
            if (Name == "PhantomofBloodKing")
                return 7784;
            if (Name == "PhantomofBloodLord")
                return 7785;
            if (Name == "CommonSilverChest(Expert)")
                return 7786;
            if (Name == "EliteSilverChest(Expert)")
                return 7787;
            if (Name == "SuperSilverChest(Expert)")
                return 7788;
            if (Name == "EpicSilverChest(Expert)")
                return 7789;
            if (Name == "CommonGoldenChest(Expert)")
                return 7790;
            if (Name == "EliteGoldenChest(Expert)")
                return 7791;
            if (Name == "SuperGoldenChest(Expert)")
                return 7792;
            if (Name == "EpicGoldenChest(Expert)")
                return 7793;
            if (Name == "CommonSilverKnight(Expert)")
                return 7794;
            if (Name == "EliteSilverKnight(Expert)")
                return 7795;
            if (Name == "SuperSilverKnight(Expert)")
                return 7796;
            if (Name == "EpicSilverKnight(Expert)")
                return 7797;
            if (Name == "CommonGoldenKnight(Expert)")
                return 7798;
            if (Name == "EliteGoldenKnight(Expert)")
                return 7799;
            if (Name == "SuperGoldenKnight(Expert)")
                return 7800;
            if (Name == "EpicGoldenKnight(Expert)")
                return 7801;
            if (Name == "PhantomofEvilKing")
                return 7802;
            if (Name == "PhantomofEvilLord")
                return 7803;
            if (Name == "WeedBug")
                return 7741;
            if (Name == "KnightBen")
                return 3284;
            if (Name == "KnightArno")
                return 3283;
            if (Name == "SolarFighter(F1)")
                return 7841;
            if (Name == "ShadowFighter(F2)")
                return 7842;
            if (Name == "NightFighter(F3)")
                return 7843;
            if (Name == "FlameElite(F4)")
                return 7844;
            if (Name == "WindElite(F5)")
                return 7845;
            if (Name == "SkyElite(F6)")
                return 7846;
            if (Name == "RuthlessHero(F7)")
                return 7847;
            if (Name == "UnknownHero(F8)")
                return 7848;
            if (Name == "ArrogantHero(F9)")
                return 7849;
            if (Name == "CruelConqueror(F10)")
                return 7850;
            if (Name == "Floor3Dominator")
                return 7851;
            if (Name == "Floor6Dominator")
                return 7852;
            if (Name == "Floor9Dominator")
                return 7853;
            if (Name == "Chest(F1)")
                return 7863;
            if (Name == "Chest(F2)")
                return 7864;
            if (Name == "Chest(F3)")
                return 7865;
            if (Name == "Chest(F4)")
                return 7866;
            if (Name == "Chest(F5)")
                return 7867;
            if (Name == "Chest(F6)")
                return 7868;
            if (Name == "Chest(F7)")
                return 7869;
            if (Name == "Chest(F8)")
                return 7870;
            if (Name == "Chest(F9)")
                return 7871;
            if (Name == "Chest(F10)")
                return 7872;
            if (Name == "DBSprite")
                return 3981;
            if (Name == "MeteorSprite")
                return 3979;
            if (Name == "380-StarWarDevil")
                return 4713;
            if (Name == "BloodyBanshee")
                return 3976;
            if (Name == "ChillingSpook")
                return 3977;
            if (Name == "NetherTyrant")
                return 3978;
            if (Name == "350-StarWarDevil")
                return 4712;
            if (Name == "300-StarWarDevil")
                return 4711;
            if (Name == "WarDevil")
                return 4710;
            if (Name == "380-StarWaterDevil")
                return 4709;
            if (Name == "350-StarWaterDevil")
                return 3975;
            if (Name == "300-StarWaterDevil")
                return 3974;
            if (Name == "WaterDevil")
                return 3973;
            if (Name == "QueenofEvil")
                return 3970;
            if (Name == "DragonWraith")
                return 3971;
            if (Name == "Patient")
                return 3285;
            if (Name == "Assassin")
                return 3287;
            if (Name == "ChiefAssassin")
                return 3988;
            if (Name == "Blacksmith")
                return 3286;
            if (Name == "CentralPheasant")
                return 3987;
            if (Name == "ThirstyGhost")
                return 3989;
            if (Name == "1-StarDevilA")
                return 4404;
            if (Name == "1-StarDevilB")
                return 4405;
            if (Name == "2-StarDevilA")
                return 4406;
            if (Name == "2-StarDevilB")
                return 4407;
            if (Name == "3-StarDevilA")
                return 4408;
            if (Name == "3-StarDevilB")
                return 4409;
            if (Name == "4-StarDevilA")
                return 4410;
            if (Name == "4-StarDevilB")
                return 4411;
            if (Name == "5-StarDevilA")
                return 4412;
            if (Name == "5-StarDevilB")
                return 4413;
            if (Name == "6-StarDevilA")
                return 4414;
            if (Name == "6-StarDevilB")
                return 4415;
            if (Name == "DarkGhost(1F)")
                return 4464;
            if (Name == "NightVampire(1F)")
                return 4465;
            if (Name == "UnderworldShadow(1F)")
                return 4466;
            if (Name == "DarkGhost(2F)")
                return 4467;
            if (Name == "NightVampire(2F)")
                return 4468;
            if (Name == "UnderworldShadow(2F)")
                return 4469;
            if (Name == "DarkGhost(3F)")
                return 4470;
            if (Name == "NightVampire(3F)")
                return 4471;
            if (Name == "UnderworldShadow(3F)")
                return 4472;
            if (Name == "DarkGhost(4F)")
                return 4473;
            if (Name == "NightVampire(4F)")
                return 4474;
            if (Name == "UnderworldShadow(4F)")
                return 4475;
            if (Name == "DarkGhost(5F)")
                return 4476;
            if (Name == "NightVampire(5F)")
                return 4477;
            if (Name == "UnderworldShadow(5F)")
                return 4478;
            if (Name == "DarkGhost(6F)")
                return 4479;
            if (Name == "NightVampire(6F)")
                return 4480;
            if (Name == "UnderworldShadow(6F)")
                return 4481;
            if (Name == "BlossomSprite")
                return 4327;
            if (Name == "TreeElf")
                return 4328;
            if (Name == "TreeElf")
                return 4348;
            if (Name == "TreeElf")
                return 4349;
            if (Name == "TreeElf")
                return 4350;
            if (Name == "TreeElf")
                return 4351;
            if (Name == "1-StarDevilD")
                return 4580;
            if (Name == "1-StarDevilC")
                return 4579;
            if (Name == "Archsaurus")
                return 2844;
            if (Name == "JiChang")
                return 4422;
            if (Name == "ZhuanZhu")
                return 4423;
            if (Name == "ZhouBotong")
                return 4424;
            if (Name == "GoldenWheel")
                return 4425;
            if (Name == "GreenFaceZombie")
                return 4352;
            if (Name == "1-StarDevilE")
                return 4581;
            if (Name == "1-StarDevilF")
                return 4582;
            if (Name == "2-StarDevilC")
                return 4583;
            if (Name == "2-StarDevilD")
                return 4584;
            if (Name == "2-StarDevilE")
                return 4585;
            if (Name == "2-StarDevilF")
                return 4586;
            if (Name == "3-StarDevilC")
                return 4587;
            if (Name == "3-StarDevilD")
                return 4588;
            if (Name == "3-StarDevilE")
                return 4589;
            if (Name == "3-StarDevilF")
                return 4590;
            if (Name == "4-StarDevilC")
                return 4591;
            if (Name == "4-StarDevilD")
                return 4592;
            if (Name == "4-StarDevilE")
                return 4593;
            if (Name == "4-StarDevilF")
                return 4594;
            if (Name == "5-StarDevilC")
                return 4595;
            if (Name == "5-StarDevilD")
                return 4596;
            if (Name == "5-StarDevilE")
                return 4597;
            if (Name == "5-StarDevilF")
                return 4598;
            if (Name == "6-StarDevilC")
                return 4599;
            if (Name == "6-StarDevilD")
                return 4600;
            if (Name == "6-StarDevilE")
                return 4601;
            if (Name == "6-StarDevilF")
                return 4602;
            if (Name == "Level50Devil")
                return 3366;
            if (Name == "Level60Devil")
                return 3367;
            if (Name == "Level70Devil")
                return 3368;
            if (Name == "Level80Devil")
                return 3369;
            if (Name == "Level90Devil")
                return 3370;
            if (Name == "Level100Devil")
                return 3371;
            if (Name == "Level110Devil")
                return 3372;
            if (Name == "Level115Devil")
                return 3373;
            if (Name == "Level120Devil")
                return 3374;
            if (Name == "Level125Devil")
                return 3375;
            if (Name == "Level130Devil")
                return 3376;
            if (Name == "Level135Devil")
                return 3377;
            if (Name == "Level140Devil")
                return 3378;
            if (Name == "InfernalSpecter")
                return 2843;
            if (Name == "ChiSprite")
                return 3982;
            if (Name == "LuckyTortoiseChest")
                return 4615;
            if (Name == "LuckyToughDrillChest")
                return 4614;
            if (Name == "LuckyStoneChest")
                return 4613;
            if (Name == "GearSprite")
                return 3986;
            if (Name == "SilverSprite")
                return 3985;
            if (Name == "DragonSoulSprite")
                return 3984;
            if (Name == "StoneSprite")
                return 3983;
            if (Name == "Solon")
                return 4420;
            if (Name == "KirigakureSaiz")
                return 4419;
            if (Name == "ChenZhen")
                return 4418;
            if (Name == "Jiumozhi")
                return 4417;
            if (Name == "QiuChuji")
                return 4416;
            if (Name == "RelicsSprite")
                return 3992;
            if (Name == "HoYuanjia")
                return 4426;
            if (Name == "SanadaYukimura")
                return 4427;
            if (Name == "Marshall")
                return 4428;
            if (Name == "LvBu")
                return 4429;
            if (Name == "YangYouji")
                return 4430;
            if (Name == "JingKe")
                return 4431;
            if (Name == "Wuyazi")
                return 4432;
            if (Name == "MasterKu")
                return 4433;
            if (Name == "FangShiyu")
                return 4434;
            if (Name == "SarutobiSasuke")
                return 4435;
            if (Name == "WillTurner")
                return 4436;
            if (Name == "LiYuanba")
                return 4437;
            if (Name == "HuaRong")
                return 4438;
            if (Name == "LiKui")
                return 4439;
            if (Name == "WangChongyang")
                return 4440;
            if (Name == "MasterYideng")
                return 4441;
            if (Name == "HuangFeihong")
                return 4442;
            if (Name == "Naganobu")
                return 4443;
            if (Name == "Barbosa")
                return 4444;
            if (Name == "XiangYu")
                return 4445;
            if (Name == "HuangZhong")
                return 4446;
            if (Name == "ZhangFei")
                return 4447;
            if (Name == "ZhangSanfeng")
                return 4448;
            if (Name == "SweepingMonk")
                return 4449;
            if (Name == "YeWen")
                return 4450;
            if (Name == "FumaKotarou")
                return 4451;
            if (Name == "LuFei")
                return 4452;
            if (Name == "XinTian")
                return 4453;
            if (Name == "LiGuang")
                return 4454;
            if (Name == "GuoJing")
                return 4455;
            if (Name == "Dongbin")
                return 4456;
            if (Name == "Damo")
                return 4457;
            if (Name == "LiXiaolong")
                return 4458;
            if (Name == "HattoriHanzo")
                return 4459;
            if (Name == "CaptainJack")
                return 4460;
            if (Name == "KingChiYou")
                return 4461;
            if (Name == "HouYi")
                return 4462;
            if (Name == "XiaoFeng")
                return 4463;
            if (Name == "QiuChuji")
                return 4483;
            if (Name == "Jiumozhi")
                return 4484;
            if (Name == "ChenZhen")
                return 4485;
            if (Name == "KirigakureSaiz")
                return 4486;
            if (Name == "Solon")
                return 4487;
            if (Name == "GuanYu")
                return 4488;
            if (Name == "JiChang")
                return 4489;
            if (Name == "ZhuanZhu")
                return 4490;
            if (Name == "ZhouBotong")
                return 4491;
            if (Name == "GoldenWheel")
                return 4492;
            if (Name == "HoYuanjia")
                return 4493;
            if (Name == "SanadaYukimura")
                return 4494;
            if (Name == "Marshall")
                return 4495;
            if (Name == "LvBu")
                return 4496;
            if (Name == "YangYouji")
                return 4497;
            if (Name == "JingKe")
                return 4498;
            if (Name == "Wuyazi")
                return 4499;
            if (Name == "MasterKu")
                return 4500;
            if (Name == "FangShiyu")
                return 4501;
            if (Name == "SarutobiSasuke")
                return 4502;
            if (Name == "WillTurner")
                return 4503;
            if (Name == "LiYuanba")
                return 4504;
            if (Name == "HuaRong")
                return 4505;
            if (Name == "LiKui")
                return 4506;
            if (Name == "WangChongyang(90%)")
                return 4507;
            if (Name == "MasterYideng(90%)")
                return 4508;
            if (Name == "HuangFeihong(90%)")
                return 4509;
            if (Name == "Naganobu(90%)")
                return 4510;
            if (Name == "Barbosa(90%)")
                return 4511;
            if (Name == "XiangYu(90%)")
                return 4512;
            if (Name == "HuangZhong(90%)")
                return 4513;
            if (Name == "ZhangFei(90%)")
                return 4514;
            if (Name == "ZhangSanfeng(90%)")
                return 4515;
            if (Name == "SweepingMonk(90%)")
                return 4516;
            if (Name == "YeWen(90%)")
                return 4517;
            if (Name == "FumaKotarou(90%)")
                return 4518;
            if (Name == "LuFei(90%)")
                return 4519;
            if (Name == "XinTian(90%)")
                return 4520;
            if (Name == "LiGuang(90%)")
                return 4521;
            if (Name == "GuoJing(90%)")
                return 4522;
            if (Name == "Dongbin(90%)")
                return 4523;
            if (Name == "Damo(90%)")
                return 4524;
            if (Name == "LiXiaolong(90%)")
                return 4525;
            if (Name == "HattoriHanzo(90%)")
                return 4526;
            if (Name == "CaptainJack(90%)")
                return 4527;
            if (Name == "KingChiYou(90%)")
                return 4528;
            if (Name == "HouYi(90%)")
                return 4529;
            if (Name == "XiaoFeng(90%)")
                return 4530;
            if (Name == "WangChongyang(70%)")
                return 4531;
            if (Name == "MasterYideng(70%)")
                return 4532;
            if (Name == "HuangFeihong(70%)")
                return 4533;
            if (Name == "Naganobu(70%)")
                return 4534;
            if (Name == "Barbosa(70%)")
                return 4535;
            if (Name == "XiangYu(70%)")
                return 4536;
            if (Name == "HuangZhong(70%)")
                return 4537;
            if (Name == "ZhangFei(70%)")
                return 4538;
            if (Name == "ZhangSanfeng(70%)")
                return 4539;
            if (Name == "SweepingMonk(70%)")
                return 4540;
            if (Name == "YeWen(70%)")
                return 4541;
            if (Name == "FumaKotarou(70%)")
                return 4542;
            if (Name == "LuFei(70%)")
                return 4543;
            if (Name == "XinTian(70%)")
                return 4544;
            if (Name == "LiGuang(70%)")
                return 4545;
            if (Name == "GuoJing(70%)")
                return 4546;
            if (Name == "Dongbin(70%)")
                return 4547;
            if (Name == "Damo(70%)")
                return 4548;
            if (Name == "LiXiaolong(70%)")
                return 4549;
            if (Name == "HattoriHanzo(70%)")
                return 4550;
            if (Name == "CaptainJack(70%)")
                return 4551;
            if (Name == "KingChiYou(70%)")
                return 4552;
            if (Name == "HouYi(70%)")
                return 4553;
            if (Name == "XiaoFeng(70%)")
                return 4554;
            if (Name == "WangChongyang(100%)")
                return 4555;
            if (Name == "MasterYideng(100%)")
                return 4556;
            if (Name == "HuangFeihong(100%)")
                return 4557;
            if (Name == "Naganobu(100%)")
                return 4558;
            if (Name == "Barbosa(100%)")
                return 4559;
            if (Name == "XiangYu(100%)")
                return 4560;
            if (Name == "HuangZhong(100%)")
                return 4561;
            if (Name == "ZhangFei(100%)")
                return 4562;
            if (Name == "ZhangSanfeng(100%)")
                return 4563;
            if (Name == "SweepingMonk(100%)")
                return 4564;
            if (Name == "YeWen(100%)")
                return 4565;
            if (Name == "FumaKotarou(100%)")
                return 4566;
            if (Name == "LuFei(100%)")
                return 4567;
            if (Name == "XinTian(100%)")
                return 4568;
            if (Name == "LiGuang(100%)")
                return 4569;
            if (Name == "GuoJing(100%)")
                return 4570;
            if (Name == "Dongbin(100%)")
                return 4571;
            if (Name == "Damo(100%)")
                return 4572;
            if (Name == "LiXiaolong(100%)")
                return 4573;
            if (Name == "HattoriHanzo(100%)")
                return 4574;
            if (Name == "CaptainJack(100%)")
                return 4575;
            if (Name == "KingChiYou(100%)")
                return 4576;
            if (Name == "HouYi(100%)")
                return 4577;
            if (Name == "XiaoFeng(100%)")
                return 4578;
            if (Name == "DivineStoneBeast")
                return 4717;
            if (Name == "DivineStarBeast")
                return 4718;
            if (Name == "DivineChiBeast")
                return 4719;
            if (Name == "NightmareGeneral")
                return 4720;
            if (Name == "CrushbonePrisoner")
                return 2846;
            if (Name == "CaptainJack")
                return 3049;
            if (Name == "InternHusky")
                return 4721;
            if (Name == "Lv.1Husky")
                return 4722;
            if (Name == "Lv.2Husky")
                return 4723;
            if (Name == "Lv.3Husky")
                return 4724;
            if (Name == "Lv.4Husky")
                return 4725;
            if (Name == "Lv.5Husky")
                return 4726;
            if (Name == "Lv.6Husky")
                return 4727;
            if (Name == "InternSamoyed")
                return 4728;
            if (Name == "Lv.1Samoyed")
                return 4729;
            if (Name == "Lv.2Samoyed")
                return 4730;
            if (Name == "Lv.3Samoyed")
                return 4731;
            if (Name == "Lv.4Samoyed")
                return 4732;
            if (Name == "Lv.5Samoyed")
                return 4733;
            if (Name == "Lv.6Samoyed")
                return 4734;
            if (Name == "InternGoldenRetriever")
                return 4735;
            if (Name == "Lv.1GoldenRetriever")
                return 4736;
            if (Name == "Lv.2GoldenRetriever")
                return 4737;
            if (Name == "Lv.3GoldenRetriever")
                return 4738;
            if (Name == "Lv.4GoldenRetriever")
                return 4739;
            if (Name == "Lv.5GoldenRetriever")
                return 4740;
            if (Name == "Lv.6GoldenRetriever")
                return 4741;
            if (Name == "LoveMessenger")
                return 3118;
            if (Name == "LoveThief")
                return 3119;
            if (Name == "BloodboilPrisoner")
                return 2847;
            if (Name == "BrutalSoulPrisoner")
                return 2848;
            if (Name == "AngryPrisonerHead")
                return 2849;
            if (Name == "HorrorPrisonerHead")
                return 2850;
            if (Name == "SolarPrisonerHead")
                return 2851;
            if (Name == "IceCreamRobber")
                return 3315;
            if (Name == "TreasureBoxDefender")
                return 4277;
            if (Name == "IronDragon")
                return 4808;
            if (Name == "SnowSpider")
                return 4809;
            if (Name == "GhostDevourer")
                return 4810;
            if (Name == "StormTroll")
                return 4811;
            if (Name == "IronDragon")
                return 4812;
            if (Name == "SnowSpider")
                return 4813;
            if (Name == "GhostDevourer")
                return 4814;
            if (Name == "StormTroll")
                return 4815;
            if (Name == "InfernalPhoenix")
                return 4816;
            if (Name == "MineDevil")
                return 4806;
            if (Name == "BloodSnake")
                return 4824;
            if (Name == "LuckyAutumnDemon")
                return 3497;
            if (Name == "MineTyrant(BOSS)")
                return 4807;
            if (Name == "LuckyAutumnAncientDemon")
                return 3498;
            if (Name == "LuckyAutumnFloodDemon")
                return 3499;
            if (Name == "LuckyAutumnHeavenDemon")
                return 3500;
            if (Name == "LuckyAutumnChaosDemon")
                return 3501;
            if (Name == "LuckyAutumnAuroraDemon")
                return 3502;
            if (Name == "LuckyAutumnTaiChiDemon")
                return 3503;
            if (Name == "DemonKnight")
                return 3824;
            if (Name == "DemonLord")
                return 3825;
            if (Name == "DemonSwordsman")
                return 4757;
            if (Name == "SeniorGreyFalcon")
                return 3711;
            if (Name == "SeniorSorrowFalcon")
                return 3712;
            if (Name == "SeniorInfernalSerpent")
                return 3713;
            if (Name == "SeniorSandWraith")
                return 3714;
            if (Name == "SeniorSoulEater")
                return 3715;
            if (Name == "SeniorEvilWhisperer")
                return 3716;
            if (Name == "SeniorTwistedDevil")
                return 3717;
            if (Name == "SeniorHeartseeker")
                return 3718;
            if (Name == "SeniorFrostDevil")
                return 3719;
            if (Name == "SeniorSoullessDevil")
                return 3720;
            if (Name == "SeniorFallenFalcon")
                return 3721;
            if (Name == "SeniorDustBat")
                return 3722;
            if (Name == "SeniorFleshTearer")
                return 3723;
            if (Name == "SeniorBoneBiter")
                return 3724;
            if (Name == "SeniorTerrorSerpent")
                return 3725;
            if (Name == "SeniorHeartBreaker")
                return 3726;
            if (Name == "SeniorMarrowEater")
                return 3727;
            if (Name == "SeniorBrainEater")
                return 3728;
            if (Name == "SeniorRocketBat")
                return 3729;
            if (Name == "SeniorCorpseSwallower")
                return 3730;
            if (Name == "SeniorShinraWraith")
                return 3731;
            if (Name == "SeniorDeathWraith")
                return 3732;
            if (Name == "SeniorFieryWraith")
                return 3733;
            if (Name == "SeniorFlyingWraith")
                return 3734;
            if (Name == "SeniorWickedWraith")
                return 3735;
            if (Name == "SeniorViolentSpirit")
                return 3736;
            if (Name == "SeniorHorrorWraith")
                return 3737;
            if (Name == "SeniorDisasterWraith")
                return 3738;
            if (Name == "SeniorMiseryWraith")
                return 3739;
            if (Name == "SeniorCloudSpecter")
                return 3740;
            if (Name == "SeniorLostSpecter")
                return 3741;
            if (Name == "SeniorDiseaseSpecter")
                return 3742;
            if (Name == "SeniorRoaringSpecter")
                return 3743;
            if (Name == "SeniorShadowSpecter")
                return 3744;
            if (Name == "SeniorGrievousSpecter")
                return 3745;
            if (Name == "SeniorSkyTearer")
                return 3746;
            if (Name == "SeniorWanderingSpecter")
                return 3747;
            if (Name == "SeniorGriefSpecter")
                return 3748;
            if (Name == "SeniorEvilSpecter")
                return 3749;
            if (Name == "SeniorDarkSpecter")
                return 3750;
            if (Name == "SeniorThunderFiend")
                return 3751;
            if (Name == "SeniorSteelFiend")
                return 3752;
            if (Name == "SeniorCopperFiend")
                return 3753;
            if (Name == "SeniorRockFiend")
                return 3754;
            if (Name == "SeniorSwiftPhantom")
                return 3755;
            if (Name == "SeniorRainbowPhantom")
                return 3756;
            if (Name == "SeniorScaledPhantom")
                return 3757;
            if (Name == "SeniorIronPhantom")
                return 3758;
            if (Name == "SeniorRockPhantom")
                return 3759;
            if (Name == "SeniorGoldenRockDevil")
                return 3760;
            if (Name == "SeniorToxicRockDevil")
                return 3761;
            if (Name == "SeniorWildRockDevil")
                return 3762;
            if (Name == "SeniorBrutalRockDevil")
                return 3763;
            if (Name == "SeniorDarkRockDevil")
                return 3764;
            if (Name == "SeniorFlameGhost")
                return 3765;
            if (Name == "SeniorThunderGhost")
                return 3766;
            if (Name == "SeniorWindGhost")
                return 3767;
            if (Name == "SeniorFireGhost")
                return 3768;
            if (Name == "SeniorForstGhost")
                return 3769;
            if (Name == "SeniorSharpGhost")
                return 3770;
            if (Name == "SeniorHeartlessSoldier")
                return 3771;
            if (Name == "SeniorViciousSoldier")
                return 3772;
            if (Name == "SeniorDarkSoldier")
                return 3773;
            if (Name == "SeniorEvilLightSoldier")
                return 3774;
            if (Name == "SeniorEvilFlyingSoldier")
                return 3775;
            if (Name == "SeniorEvilSkySoldier")
                return 3776;
            if (Name == "SeniorEvilWindSoldier")
                return 3777;
            if (Name == "SeniorEvilCloudSoldier")
                return 3778;
            if (Name == "SeniorEvilThunderSoldier")
                return 3779;
            if (Name == "SeniorEvilFrostSoldier")
                return 3780;
            if (Name == "SeniorSoulCrusher")
                return 3781;
            if (Name == "SeniorBoneCrusher")
                return 3782;
            if (Name == "SeniorThunderCrusher")
                return 3783;
            if (Name == "SeniorSkyCrusher")
                return 3784;
            if (Name == "SeniorFireCrusher")
                return 3785;
            if (Name == "SeniorIceDevil")
                return 3786;
            if (Name == "SeniorDoomBat")
                return 3787;
            if (Name == "SeniorLavaDevil")
                return 3788;
            if (Name == "SeniorIron-wingedDevil")
                return 3789;
            if (Name == "SeniorSizzlingDevil")
                return 3790;
            if (Name == "SeniorFlameDestroyer")
                return 3791;
            if (Name == "SeniorHeavenBat")
                return 3792;
            if (Name == "SeniorEarthDestroyer")
                return 3793;
            if (Name == "SeniorDominatingDestroyer")
                return 3794;
            if (Name == "SeniorConquestDestroyer")
                return 3795;
            if (Name == "SeniorSkyDestroyer")
                return 3796;
            if (Name == "SeniorSolarDestroyer")
                return 3797;
            if (Name == "SeniorPowerDestroyer")
                return 3798;
            if (Name == "SeniorStarDestroyer")
                return 3799;
            if (Name == "SeniorWarDestroyer")
                return 3800;
            if (Name == "SeniorSoulMessenger")
                return 3801;
            if (Name == "SeniorSkyMessenger")
                return 3802;
            if (Name == "SeniorMountainMessenger")
                return 3803;
            if (Name == "SeniorPeakPredator")
                return 3804;
            if (Name == "SeniorMountainPredator")
                return 3805;
            if (Name == "SeniorWildPredator")
                return 3806;
            if (Name == "SeniorSkyPredator")
                return 3807;
            if (Name == "SeniorStarEvilLord")
                return 3808;
            if (Name == "SeniorLunarEvilLord")
                return 3809;
            if (Name == "SeniorSolarEvilLord")
                return 3810;
            if (Name == "GreyFalcon")
                return 3827;
            if (Name == "SorrowFalcon")
                return 3828;
            if (Name == "InfernalSerpent")
                return 3829;
            if (Name == "SandWraith")
                return 3830;
            if (Name == "SoulEater")
                return 3831;
            if (Name == "EvilWhisperer")
                return 3832;
            if (Name == "TwistedDevil")
                return 3833;
            if (Name == "Heartseeker")
                return 3834;
            if (Name == "FrostDevil")
                return 3835;
            if (Name == "SoullessDevil")
                return 3836;
            if (Name == "FallenFalcon")
                return 3837;
            if (Name == "DustBat")
                return 3838;
            if (Name == "FleshTearer")
                return 3839;
            if (Name == "BoneBiter")
                return 3840;
            if (Name == "TerrorSerpent")
                return 3841;
            if (Name == "HeartBreaker")
                return 3842;
            if (Name == "MarrowEater")
                return 3843;
            if (Name == "BrainEater")
                return 3844;
            if (Name == "RocketBat")
                return 3845;
            if (Name == "CorpseSwallower")
                return 3846;
            if (Name == "ShinraWraith")
                return 3847;
            if (Name == "DeathWraith")
                return 3848;
            if (Name == "FieryWraith")
                return 3849;
            if (Name == "FlyingWraith")
                return 3850;
            if (Name == "WickedWraith")
                return 3851;
            if (Name == "ViolentSpirit")
                return 3852;
            if (Name == "HorrorWraith")
                return 3853;
            if (Name == "DisasterWraith")
                return 3854;
            if (Name == "MiseryWraith")
                return 3855;
            if (Name == "CloudSpecter")
                return 3856;
            if (Name == "LostSpecter")
                return 3857;
            if (Name == "DiseaseSpecter")
                return 3858;
            if (Name == "RoaringSpecter")
                return 3859;
            if (Name == "ShadowSpecter")
                return 3860;
            if (Name == "GrievousSpecter")
                return 3861;
            if (Name == "SkyTearer")
                return 3862;
            if (Name == "WanderingSpecter")
                return 3863;
            if (Name == "GriefSpecter")
                return 3864;
            if (Name == "EvilSpecter")
                return 3865;
            if (Name == "DarkSpecter")
                return 3866;
            if (Name == "ThunderFiend")
                return 3867;
            if (Name == "SteelFiend")
                return 1138;
            if (Name == "CopperFiend")
                return 3869;
            if (Name == "RockFiend")
                return 3870;
            if (Name == "SwiftPhantom")
                return 3871;
            if (Name == "RainbowPhantom")
                return 11725;
            if (Name == "ScaledPhantom")
                return 3873;
            if (Name == "IronPhantom")
                return 3874;
            if (Name == "RockPhantom")
                return 3875;
            if (Name == "GoldenRockDevil")
                return 3876;
            if (Name == "ToxicRockDevil")
                return 3877;
            if (Name == "WildRockDevil")
                return 3878;
            if (Name == "BrutalRockDevil")
                return 3879;
            if (Name == "DarkRockDevil")
                return 3880;
            if (Name == "FlameGhost")
                return 3881;
            if (Name == "ThunderGhost")
                return 3882;
            if (Name == "WindGhost")
                return 3883;
            if (Name == "FireGhost")
                return 3884;
            if (Name == "ForstGhost")
                return 3885;
            if (Name == "SharpGhost")
                return 3886;
            if (Name == "HeartlessSoldier")
                return 3887;
            if (Name == "ViciousSoldier")
                return 3888;
            if (Name == "DarkSoldier")
                return 3889;
            if (Name == "EvilLightSoldier")
                return 3890;
            if (Name == "EvilFlyingSoldier")
                return 3891;
            if (Name == "EvilSkySoldier")
                return 3892;
            if (Name == "EvilWindSoldier")
                return 3893;
            if (Name == "EvilCloudSoldier")
                return 3894;
            if (Name == "EvilThunderSoldier")
                return 3895;
            if (Name == "EvilFrostSoldier")
                return 3896;
            if (Name == "SoulCrusher")
                return 3897;
            if (Name == "BoneCrusher")
                return 3898;
            if (Name == "ThunderCrusher")
                return 3899;
            if (Name == "SkyCrusher")
                return 3900;
            if (Name == "FireCrusher")
                return 3901;
            if (Name == "IceDevil")
                return 3902;
            if (Name == "DoomBat")
                return 3903;
            if (Name == "LavaDevil")
                return 3904;
            if (Name == "Iron-wingedDevil")
                return 3905;
            if (Name == "SizzlingDevil")
                return 3906;
            if (Name == "FlameDestroyer")
                return 3907;
            if (Name == "HeavenBat")
                return 3908;
            if (Name == "EarthDestroyer")
                return 3909;
            if (Name == "DominatingDestroyer")
                return 3910;
            if (Name == "ConquestDestroyer")
                return 3911;
            if (Name == "SkyDestroyer")
                return 3912;
            if (Name == "SolarDestroyer")
                return 3913;
            if (Name == "PowerDestroyer")
                return 3914;
            if (Name == "StarDestroyer")
                return 3915;
            if (Name == "WarDestroyer")
                return 3916;
            if (Name == "SoulMessenger")
                return 3917;
            if (Name == "SkyMessenger")
                return 3918;
            if (Name == "MountainMessenger")
                return 3919;
            if (Name == "PeakPredator")
                return 3920;
            if (Name == "MountainPredator")
                return 3921;
            if (Name == "WildPredator")
                return 3922;
            if (Name == "SkyPredator")
                return 3923;
            if (Name == "StarEvilLord")
                return 3924;
            if (Name == "LunarEvilLord")
                return 3925;
            if (Name == "SolarEvilLord")
                return 3926;
            if (Name == "ArmoredDemon")
                return 4828;
            if (Name == "FlamePhoenix")
                return 4829;
            if (Name == "GreedySoulPrisoner")
                return 2845;
            if (Name == "RagingPumpkin")
                return 1022;
            if (Name == "Thor`sPhantom")
                return 4953;
            if (Name == "Thor`sPhantom")
                return 4952;
            if (Name == "Thor`sPhantom")
                return 4951;
            if (Name == "MineDevil[Championship]")
                return 4817;
            if (Name == "Predator")
                return 5555;
            if (Name == "MysteryChest")
                return 4912;
            if (Name == "BeastslayerChiBox")
                return 4913;
            if (Name == "BeastslayerStarBox")
                return 4914;
            if (Name == "Beastslayer+StoneBox")
                return 4915;
            if (Name == "BeastslayerCPs(B)Box")
                return 4916;
            if (Name == "BeastslayerChiChest")
                return 4917;
            if (Name == "BeastslayerStarChest")
                return 4918;
            if (Name == "Beastslayer+StoneChest")
                return 4919;
            if (Name == "BeastslayerCPs(B)Chest")
                return 4920;
            if (Name == "LuckyXmasDemon")
                return 4928;
            if (Name == "LuckyXmasAncientDemon")
                return 4929;
            if (Name == "LuckyXmasFloodDemon")
                return 4930;
            if (Name == "LuckyXmasHeavenDemon")
                return 4931;
            if (Name == "LuckyXmasChaosDemon")
                return 4932;
            if (Name == "LuckyXmasAuroraDemon")
                return 4933;
            if (Name == "LuckyXmasTaiChiDemon")
                return 4934;
            if (Name == "Thor`sPhantom")
                return 4954;
            if (Name == "Thor`sPhantom")
                return 4955;
            if (Name == "[ThorShrine]Beautician")
                return 4956;
            if (Name == "[ThorShrine]MasterGuo")
                return 4957;
            if (Name == "[ThorShrine]SolarSaint")
                return 4958;
            if (Name == "[ThorShrine]CloudSaint")
                return 4959;
            if (Name == "[ThorShrine]TowerKeeper")
                return 4960;
            if (Name == "SharpOwlet")
                return 3403;
            if (Name == "WingedDevil")
                return 3404;
            if (Name == "BloodLeopard")
                return 3405;
            if (Name == "BloodthirstyBat")
                return 3406;
            if (Name == "GhostOwlet")
                return 3407;
            if (Name == "RoaringDevil")
                return 3408;
            if (Name == "EliteSandDemon")
                return 3409;
            if (Name == "SoulBiter")
                return 3410;
            if (Name == "SoulBurner")
                return 3411;
            if (Name == "BlindNightmare")
                return 3412;
            if (Name == "GiantRat")
                return 3413;
            if (Name == "SkyBreaker")
                return 3414;
            if (Name == "SkyBurner")
                return 3415;
            if (Name == "EliteSkyDevourer")
                return 3416;
            if (Name == "InfernoBull")
                return 3417;
            if (Name == "NetherworldBull")
                return 3418;
            if (Name == "StoneDemon")
                return 3419;
            if (Name == "StoneBiter")
                return 3420;
            if (Name == "InfernoBullDemon")
                return 3421;
            if (Name == "NetherworldBullDemon")
                return 3422;
            if (Name == "EliteCrystalBreaker")
                return 3423;
            if (Name == "RagedBirdman")
                return 3424;
            if (Name == "ClawedBirdman")
                return 3425;
            if (Name == "StarDevourer")
                return 3426;
            if (Name == "EliteSoulDevourer")
                return 3427;
            if (Name == "GreenTailAssassin")
                return 3460;
            if (Name == "AirborneAxeDemon")
                return 3461;
            if (Name == "GhostFace")
                return 3462;
            if (Name == "NetherworldSupplier")
                return 3463;
            if (Name == "NetherBeast(Lucis`Mount)")
                return 4961;
            if (Name == "NetherBeast(Minos`Mount)")
                return 4962;
            if (Name == "NetherBeast(Karura`sMount)")
                return 4963;
            if (Name == "NetherBeast(Pando`sMount)")
                return 4964;
            if (Name == "NetherWraith")
                return 4965;
            if (Name == "Lucis")
                return 4966;
            if (Name == "Karura")
                return 4967;
            if (Name == "Pando")
                return 4968;
            if (Name == "Minos")
                return 4969;
            if (Name == "NetherBrute")
                return 4970;
            if (Name == "NetherLordHader")
                return 4971;
            if (Name == "FireSpecter")
                return 4972;
            if (Name == "AncientDragon")
                return 2984;
            if (Name == "InfernalDragon(L130)")
                return 3532;
            if (Name == "InfernalDragon(L135)")
                return 3533;
            if (Name == "InfernalDragon(L140)")
                return 3534;
            if (Name == "InfernalDragon(L145)")
                return 3535;
            if (Name == "Dinomonster")
                return 3478;
            if (Name == "Nidhogg")
                return 3479;
            if (Name == "HouYi(260BP)")
                return 5061;
            if (Name == "HouYi(270BP)")
                return 5062;
            if (Name == "HouYi(280BP)")
                return 5063;
            if (Name == "HouYi(290BP)")
                return 5064;
            if (Name == "HouYi(300BP)")
                return 5065;
            if (Name == "HouYi(310BP)")
                return 5066;
            if (Name == "HouYi(320BP)")
                return 5067;
            if (Name == "HouYi(330BP)")
                return 5068;
            if (Name == "HouYi(340BP)")
                return 5069;
            if (Name == "HouYi(350BP)")
                return 5070;
            if (Name == "HouYi(360BP)")
                return 5071;
            if (Name == "HouYi(370BP)")
                return 5072;
            if (Name == "HouYi(380BP)")
                return 5073;
            if (Name == "HouYi(390BP)")
                return 5074;
            if (Name == "HouYi(400BP)")
                return 5075;
            if (Name == "HouYi(405BP)")
                return 5076;
            if (Name == "HouYi(410BP)")
                return 5077;
            if (Name == "HouYi(415BP)")
                return 5078;
            if (Name == "HouYi(420BP)")
                return 5079;
            if (Name == "HouYi(425BP)")
                return 5080;
            if (Name == "HouYi(430BP)")
                return 5081;
            if (Name == "HouYi(435BP)")
                return 5082;
            if (Name == "HouYi(440BP)")
                return 5083;
            if (Name == "HouYi(445BP)")
                return 5084;
            if (Name == "HouYi(450BP)")
                return 5085;
            if (Name == "KingChiYou(260BP)")
                return 5086;
            if (Name == "KingChiYou(270BP)")
                return 5087;
            if (Name == "KingChiYou(280BP)")
                return 5088;
            if (Name == "KingChiYou(290BP)")
                return 5089;
            if (Name == "KingChiYou(300BP)")
                return 5090;
            if (Name == "KingChiYou(310BP)")
                return 5091;
            if (Name == "KingChiYou(320BP)")
                return 5092;
            if (Name == "KingChiYou(330BP)")
                return 5093;
            if (Name == "KingChiYou(340BP)")
                return 5094;
            if (Name == "KingChiYou(350BP)")
                return 5095;
            if (Name == "KingChiYou(360BP)")
                return 5096;
            if (Name == "KingChiYou(370BP)")
                return 5097;
            if (Name == "KingChiYou(380BP)")
                return 5098;
            if (Name == "KingChiYou(390BP)")
                return 5099;
            if (Name == "KingChiYou(400BP)")
                return 5100;
            if (Name == "KingChiYou(405BP)")
                return 5101;
            if (Name == "KingChiYou(410BP)")
                return 5102;
            if (Name == "KingChiYou(415BP)")
                return 5103;
            if (Name == "KingChiYou(420BP)")
                return 5104;
            if (Name == "KingChiYou(425BP)")
                return 5105;
            if (Name == "KingChiYou(430BP)")
                return 5106;
            if (Name == "KingChiYou(435BP)")
                return 5107;
            if (Name == "KingChiYou(440BP)")
                return 5108;
            if (Name == "KingChiYou(445BP)")
                return 5109;
            if (Name == "KingChiYou(450BP)")
                return 5110;
            if (Name == "LiXiaolong(260BP)")
                return 5111;
            if (Name == "LiXiaolong(270BP)")
                return 5112;
            if (Name == "LiXiaolong(280BP)")
                return 5113;
            if (Name == "LiXiaolong(290BP)")
                return 5114;
            if (Name == "LiXiaolong(300BP)")
                return 5115;
            if (Name == "LiXiaolong(310BP)")
                return 5116;
            if (Name == "LiXiaolong(320BP)")
                return 5117;
            if (Name == "LiXiaolong(330BP)")
                return 5118;
            if (Name == "LiXiaolong(340BP)")
                return 5119;
            if (Name == "LiXiaolong(350BP)")
                return 5120;
            if (Name == "LiXiaolong(360BP)")
                return 5121;
            if (Name == "LiXiaolong(370BP)")
                return 5122;
            if (Name == "LiXiaolong(380BP)")
                return 5123;
            if (Name == "LiXiaolong(390BP)")
                return 5124;
            if (Name == "LiXiaolong(400BP)")
                return 5125;
            if (Name == "LiXiaolong(405BP)")
                return 5126;
            if (Name == "LiXiaolong(410BP)")
                return 5127;
            if (Name == "LiXiaolong(415BP)")
                return 5128;
            if (Name == "LiXiaolong(420BP)")
                return 5129;
            if (Name == "LiXiaolong(425BP)")
                return 5130;
            if (Name == "LiXiaolong(430BP)")
                return 5131;
            if (Name == "LiXiaolong(435BP)")
                return 5132;
            if (Name == "LiXiaolong(440BP)")
                return 5133;
            if (Name == "LiXiaolong(445BP)")
                return 5134;
            if (Name == "LiXiaolong(450BP)")
                return 5135;
            if (Name == "CaptainJack(260BP)")
                return 5136;
            if (Name == "CaptainJack(270BP)")
                return 5137;
            if (Name == "CaptainJack(280BP)")
                return 5138;
            if (Name == "CaptainJack(290BP)")
                return 5139;
            if (Name == "CaptainJack(300BP)")
                return 5140;
            if (Name == "CaptainJack(310BP)")
                return 5141;
            if (Name == "CaptainJack(320BP)")
                return 5142;
            if (Name == "CaptainJack(330BP)")
                return 5143;
            if (Name == "CaptainJack(340BP)")
                return 5144;
            if (Name == "CaptainJack(350BP)")
                return 5145;
            if (Name == "CaptainJack(360BP)")
                return 5146;
            if (Name == "CaptainJack(370BP)")
                return 5147;
            if (Name == "CaptainJack(380BP)")
                return 5148;
            if (Name == "CaptainJack(390BP)")
                return 5149;
            if (Name == "CaptainJack(400BP)")
                return 5150;
            if (Name == "CaptainJack(405BP)")
                return 5151;
            if (Name == "CaptainJack(410BP)")
                return 5152;
            if (Name == "CaptainJack(415BP)")
                return 5153;
            if (Name == "CaptainJack(420BP)")
                return 5154;
            if (Name == "CaptainJack(425BP)")
                return 5155;
            if (Name == "CaptainJack(430BP)")
                return 5156;
            if (Name == "CaptainJack(435BP)")
                return 5157;
            if (Name == "CaptainJack(440BP)")
                return 5158;
            if (Name == "CaptainJack(445BP)")
                return 5159;
            if (Name == "CaptainJack(450BP)")
                return 5160;
            if (Name == "Damo(260BP)")
                return 5161;
            if (Name == "Damo(270BP)")
                return 5162;
            if (Name == "Damo(280BP)")
                return 5163;
            if (Name == "Damo(290BP)")
                return 5164;
            if (Name == "Damo(300BP)")
                return 5165;
            if (Name == "Damo(310BP)")
                return 5166;
            if (Name == "Damo(320BP)")
                return 5167;
            if (Name == "Damo(330BP)")
                return 5168;
            if (Name == "Damo(340BP)")
                return 5169;
            if (Name == "Damo(350BP)")
                return 5170;
            if (Name == "Damo(360BP)")
                return 5171;
            if (Name == "Damo(370BP)")
                return 5172;
            if (Name == "Damo(380BP)")
                return 5173;
            if (Name == "Damo(390BP)")
                return 5174;
            if (Name == "Damo(400BP)")
                return 5175;
            if (Name == "Damo(405BP)")
                return 5176;
            if (Name == "Damo(410BP)")
                return 5177;
            if (Name == "Damo(415BP)")
                return 5178;
            if (Name == "Damo(420BP)")
                return 5179;
            if (Name == "Damo(425BP)")
                return 5180;
            if (Name == "Damo(430BP)")
                return 5181;
            if (Name == "Damo(435BP)")
                return 5182;
            if (Name == "Damo(440BP)")
                return 5183;
            if (Name == "Damo(445BP)")
                return 5184;
            if (Name == "Damo(450BP)")
                return 5185;
            if (Name == "HattoriHanzo(260BP)")
                return 5186;
            if (Name == "HattoriHanzo(270BP)")
                return 5187;
            if (Name == "HattoriHanzo(280BP)")
                return 5188;
            if (Name == "HattoriHanzo(290BP)")
                return 5189;
            if (Name == "HattoriHanzo(300BP)")
                return 5190;
            if (Name == "HattoriHanzo(310BP)")
                return 5191;
            if (Name == "HattoriHanzo(320BP)")
                return 5192;
            if (Name == "HattoriHanzo(330BP)")
                return 5193;
            if (Name == "HattoriHanzo(340BP)")
                return 5194;
            if (Name == "HattoriHanzo(350BP)")
                return 5195;
            if (Name == "HattoriHanzo(360BP)")
                return 5196;
            if (Name == "HattoriHanzo(370BP)")
                return 5197;
            if (Name == "HattoriHanzo(380BP)")
                return 5198;
            if (Name == "HattoriHanzo(390BP)")
                return 5199;
            if (Name == "HattoriHanzo(400BP)")
                return 5200;
            if (Name == "HattoriHanzo(405BP)")
                return 5201;
            if (Name == "HattoriHanzo(410BP)")
                return 5202;
            if (Name == "HattoriHanzo(415BP)")
                return 5203;
            if (Name == "HattoriHanzo(420BP)")
                return 5204;
            if (Name == "HattoriHanzo(425BP)")
                return 5205;
            if (Name == "HattoriHanzo(430BP)")
                return 5206;
            if (Name == "HattoriHanzo(435BP)")
                return 5207;
            if (Name == "HattoriHanzo(440BP)")
                return 5208;
            if (Name == "HattoriHanzo(445BP)")
                return 5209;
            if (Name == "HattoriHanzo(450BP)")
                return 5210;
            if (Name == "XiaoFeng(260BP)")
                return 5211;
            if (Name == "XiaoFeng(270BP)")
                return 5212;
            if (Name == "XiaoFeng(280BP)")
                return 5213;
            if (Name == "XiaoFeng(290BP)")
                return 5214;
            if (Name == "XiaoFeng(300BP)")
                return 5215;
            if (Name == "XiaoFeng(310BP)")
                return 5216;
            if (Name == "XiaoFeng(320BP)")
                return 5217;
            if (Name == "XiaoFeng(330BP)")
                return 5218;
            if (Name == "XiaoFeng(340BP)")
                return 5219;
            if (Name == "XiaoFeng(350BP)")
                return 5220;
            if (Name == "XiaoFeng(360BP)")
                return 5221;
            if (Name == "XiaoFeng(370BP)")
                return 5222;
            if (Name == "XiaoFeng(380BP)")
                return 5223;
            if (Name == "XiaoFeng(390BP)")
                return 5224;
            if (Name == "XiaoFeng(400BP)")
                return 5225;
            if (Name == "XiaoFeng(405BP)")
                return 5226;
            if (Name == "XiaoFeng(410BP)")
                return 5227;
            if (Name == "XiaoFeng(415BP)")
                return 5228;
            if (Name == "XiaoFeng(420BP)")
                return 5229;
            if (Name == "XiaoFeng(425BP)")
                return 5230;
            if (Name == "XiaoFeng(430BP)")
                return 5231;
            if (Name == "XiaoFeng(435BP)")
                return 5232;
            if (Name == "XiaoFeng(440BP)")
                return 5233;
            if (Name == "XiaoFeng(445BP)")
                return 5234;
            if (Name == "XiaoFeng(450BP)")
                return 5235;
            if (Name == "Dongbin(260BP)")
                return 5236;
            if (Name == "Dongbin(270BP)")
                return 5237;
            if (Name == "Dongbin(280BP)")
                return 5238;
            if (Name == "Dongbin(290BP)")
                return 5239;
            if (Name == "Dongbin(300BP)")
                return 5240;
            if (Name == "Dongbin(310BP)")
                return 5241;
            if (Name == "Dongbin(320BP)")
                return 5242;
            if (Name == "Dongbin(330BP)")
                return 5243;
            if (Name == "Dongbin(340BP)")
                return 5244;
            if (Name == "Dongbin(350BP)")
                return 5245;
            if (Name == "Dongbin(360BP)")
                return 5246;
            if (Name == "Dongbin(370BP)")
                return 5247;
            if (Name == "Dongbin(380BP)")
                return 5248;
            if (Name == "Dongbin(390BP)")
                return 5249;
            if (Name == "Dongbin(400BP)")
                return 5250;
            if (Name == "Dongbin(405BP)")
                return 5251;
            if (Name == "Dongbin(410BP)")
                return 5252;
            if (Name == "Dongbin(415BP)")
                return 5253;
            if (Name == "Dongbin(420BP)")
                return 5254;
            if (Name == "Dongbin(425BP)")
                return 5255;
            if (Name == "Dongbin(430BP)")
                return 5256;
            if (Name == "Dongbin(435BP)")
                return 5257;
            if (Name == "Dongbin(440BP)")
                return 5258;
            if (Name == "Dongbin(445BP)")
                return 5259;
            if (Name == "Dongbin(450BP)")
                return 5260;
            if (Name == "S1LavaDragonSpirit")
                return 4937;
            if (Name == "S2LavaDragonSpirit")
                return 4938;
            if (Name == "S3LavaDragonSpirit")
                return 4939;
            if (Name == "S4LavaDragonSpirit")
                return 4940;
            if (Name == "S5LavaDragonSpirit")
                return 4941;
            if (Name == "S6LavaDragonSpirit")
                return 4942;
            if (Name == "S7LavaDragonSpirit")
                return 4943;
            if (Name == "S1NetherDragonSpirit")
                return 4944;
            if (Name == "S2NetherDragonSpirit")
                return 4945;
            if (Name == "S3NetherDragonSpirit")
                return 4946;
            if (Name == "S4NetherDragonSpirit")
                return 4947;
            if (Name == "S5NetherDragonSpirit")
                return 4948;
            if (Name == "S6NetherDragonSpirit")
                return 4949;
            if (Name == "ChaosSerpent")
                return 4988;
            if (Name == "FireworkThief")
                return 4744;
            if (Name == "ImperialChest")
                return 3554;
            if (Name == "GoldenChest")
                return 3555;
            if (Name == "AzureChest")
                return 3556;
            if (Name == "RubyChest")
                return 3557;
            if (Name == "LuckyChest")
                return 3558;
            if (Name == "Pheasant")
                return 4833;
            if (Name == "Turtledove")
                return 4834;
            if (Name == "Robin")
                return 4835;
            if (Name == "Apparition")
                return 4836;
            if (Name == "WingedSnake")
                return 4838;
            if (Name == "Bandit")
                return 4839;
            if (Name == "Ratling")
                return 4840;
            if (Name == "FireSpirit")
                return 4841;
            if (Name == "Macaque")
                return 4842;
            if (Name == "GiantApe")
                return 4843;
            if (Name == "ThunderApe")
                return 4844;
            if (Name == "Snakeman")
                return 4845;
            if (Name == "SandMonster")
                return 4846;
            if (Name == "HillMonster")
                return 4847;
            if (Name == "RockMonster")
                return 4848;
            if (Name == "BladeGhost")
                return 4849;
            if (Name == "Birdman")
                return 4850;
            if (Name == "Hawkman")
                return 4851;
            if (Name == "BirdmanL88")
                return 4852;
            if (Name == "HawkL93")
                return 4853;
            if (Name == "BanditL98")
                return 4854;
            if (Name == "TombBatL103")
                return 4855;
            if (Name == "BloodyBatL108")
                return 4856;
            if (Name == "BullMonsterL113")
                return 4857;
            if (Name == "RedDevilL118")
                return 4858;
            if (Name == "SerpentEnvoy")
                return 4859;
            if (Name == "IcySerpent")
                return 4860;
            if (Name == "SkySerpent")
                return 4862;
            if (Name == "FrostSerpent")
                return 4863;
            if (Name == "DarkElf")
                return 4865;
            if (Name == "SnakeHubbub")
                return 4867;
            if (Name == "CastleBandit")
                return 4868;
            if (Name == "Vulture")
                return 4869;
            if (Name == "GiantApeL53")
                return 4870;
            if (Name == "HillMonsterL73")
                return 4871;
            if (Name == "TombBat")
                return 4873;
            if (Name == "BanditL97")
                return 4874;
            if (Name == "BloodyBat")
                return 4875;
            if (Name == "BullMonster")
                return 4876;
            if (Name == "RedDevilL117")
                return 4877;
            if (Name == "VampireBat")
                return 4878;
            if (Name == "BloodyDevil")
                return 4879;
            if (Name == "PheasantLord")
                return 4880;
            if (Name == "WraithKing")
                return 4881;
            if (Name == "Thief")
                return 4882;
            if (Name == "MacaqueKing")
                return 4883;
            if (Name == "BloodfireSnakeman")
                return 4884;
            if (Name == "ThunderApeL85")
                return 4885;
            if (Name == "SandKing`sMessenger")
                return 4886;
            if (Name == "HillKing")
                return 4887;
            if (Name == "BladeKing")
                return 4888;
            if (Name == "ThunderHawkKing")
                return 4889;
            if (Name == "IslandBanditLeader")
                return 4890;
            if (Name == "BloodyKing")
                return 4891;
            if (Name == "ShadowKing")
                return 4892;
            if (Name == "FuryBullKing")
                return 4893;
            if (Name == "SerpentPhantom")
                return 4894;
            if (Name == "SerpentWraith")
                return 4895;
            if (Name == "DarkElfKing")
                return 4896;
            if (Name == "CaptainSnake")
                return 4897;
            if (Name == "GoldenPheasantKing")
                return 4898;
            if (Name == "PlainsThief")
                return 4899;
            if (Name == "SnakemanKing")
                return 4900;
            if (Name == "SandMonsterKing")
                return 4901;
            if (Name == "WaterDevilKing")
                return 4902;
            if (Name == "DungeonGhostKing")
                return 4903;
            if (Name == "SerpentKing")
                return 4904;
            if (Name == "DarkKing")
                return 4905;
            if (Name == "DarkSnakeKing")
                return 4906;
            if (Name == "RuthlessAsura")
                return 4907;
            if (Name == "FlameGiant")
                return 4908;
            if (Name == "ShadowSpider")
                return 4909;
            if (Name == "DoomDragon")
                return 4910;
            if (Name == "SilentDevil")
                return 3991;
            if (Name == "DreamBeast")
                return 3504;
            if (Name == "AzureDragon`sPhantom")
                return 3505;
            if (Name == "AzureDragon")
                return 3506;
            if (Name == "InfernoDragon")
                return 3528;
            if (Name == "Flerken")
                return 3529;
            if (Name == "FieryVulture")
                return 4986;
            if (Name == "WildMonk(1F)")
                return 5000;
            if (Name == "WildMonk(2F)")
                return 5001;
            if (Name == "WildMonk(3F)")
                return 5002;
            if (Name == "ShadowHunter(4F)")
                return 5003;
            if (Name == "ShadowHunter(5F)")
                return 5004;
            if (Name == "ShadowHunter(6F)")
                return 5005;
            if (Name == "SwordsmanofAbyss(7F)")
                return 5006;
            if (Name == "SwordsmanofAbyss(8F)")
                return 5007;
            if (Name == "SwordsmanofAbyss(9F)")
                return 5008;
            if (Name == "ThundercloudTaoist")
                return 5009;
            if (Name == "Holylight")
                return 5010;
            if (Name == "Shadowmoon")
                return 5011;
            if (Name == "Shura")
                return 5012;
            if (Name == "HellHawk")
                return 1081;
            if (Name == "HellHawkKing")
                return 1044;
            if (Name == "MindofEvil")
                return 5360;
            if (Name == "EvilSword")
                return 5361;
            if (Name == "SwordManiac")
                return 4990;
            if (Name == "P1InfiniteDementor")
                return 5369;
            if (Name == "P2InfiniteDementor")
                return 5370;
            if (Name == "P3InfiniteDementor")
                return 5393;
            if (Name == "P4InfiniteDementor")
                return 5394;
            if (Name == "P5InfiniteDementor")
                return 5395;
            if (Name == "UltimateDementor")
                return 5396;
            if (Name == "LostGhost")
                return 4989;
            if (Name == "GhostGirl")
                return 4992;
            if (Name == "GhostCalf")
                return 4993;
            if (Name == "GhostBird")
                return 4994;
            if (Name == "WickedDragon")
                return 1047;
            if (Name == "FlameDevil")
                return 3546;
            if (Name == "IceDevil")
                return 3547;
            if (Name == "GoldenPhoenix")
                return 3537;
            if (Name == "MegaFish")
                return 3538;
            if (Name == "StarBeast")
                return 3539;
            if (Name == "AngryBlademaster")
                return 4991;
            if (Name == "AncientCorpse")
                return 2990;
            if (Name == "HeavenSpirit")
                return 5397;
            if (Name == "EarthSpirit")
                return 5398;
            if (Name == "SkyDevil")
                return 3379;
            if (Name == "Pride")
                return 5362;
            if (Name == "Envy")
                return 5363;
            if (Name == "Wrath")
                return 5364;
            if (Name == "Sloth")
                return 5365;
            if (Name == "Greed")
                return 5366;
            if (Name == "Gluttony")
                return 5367;
            if (Name == "Lust")
                return 5368;
            if (Name == "Trojan(Male)")
                return 4995;
            if (Name == "Trojan(Female)")
                return 4996;
            if (Name == "DragonSpirit")
                return 4997;
            if (Name == "RustyGhost")
                return 1054;
            if (Name == "RustyDevil")
                return 1055;
            if (Name == "JasperSnake")
                return 5433;
            if (Name == "NightmareBanshee")
                return 5434;
            if (Name == "DreamEater")
                return 5435;
            if (Name == "GreyGhost")
                return 1092;
            if (Name == "VampireViscount")
                return 5445;
            if (Name == "GhostWitch")
                return 5446;
            if (Name == "AquaElf")
                return 5447;
            if (Name == "FrostTiger")
                return 5448;
            if (Name == "SandDemon")
                return 5449;
            if (Name == "VampireEarl")
                return 5450;
            if (Name == "ShadowWitch")
                return 5451;
            if (Name == "StormElf")
                return 5452;
            if (Name == "FlameTiger")
                return 5453;
            if (Name == "FrostGiant")
                return 5454;
            if (Name == "VampireDuke")
                return 5455;
            if (Name == "DarkQueen")
                return 5456;
            if (Name == "DarkElf")
                return 5457;
            if (Name == "DemonTiger")
                return 5458;
            if (Name == "BlazeLord")
                return 5459;
            if (Name == "Demon")
                return 5693;
            if (Name == "Demon")
                return 5694;
            if (Name == "Demon")
                return 5695;
            if (Name == "Demon")
                return 5696;
            if (Name == "FireballRate")
                return 1097;
            if (Name == "TempestBeast")
                return 1098;
            if (Name == "FlowerThief")
                return 5509;
            if (Name == "Rabbit")
                return 5740;
            if (Name == "WoodcutterWoo")
                return 5741;
            if (Name == "RabbitGirl")
                return 5742;
            if (Name == "Invader")
                return 5699;
            if (Name == "HeadOfInvaders")
                return 1152;
            if (Name == "L70ThirstyWolf")
                return 5274;
            if (Name == "L78IceApe")
                return 5275;
            if (Name == "L85MesmericHawk")
                return 5276;
            if (Name == "L93DustBeast")
                return 5277;
            if (Name == "L101ChillyBird")
                return 5278;
            if (Name == "L105HillBeast")
                return 5279;
            if (Name == "L109Ghost")
                return 5280;
            if (Name == "L113FlameRat")
                return 5281;
            if (Name == "L117SoulBanshee")
                return 5282;
            if (Name == "L121BatBanshee")
                return 5283;
            if (Name == "L123GhostHerald")
                return 5284;
            if (Name == "L126GoblinSoul")
                return 5285;
            if (Name == "L128BluishSnake")
                return 5286;
            if (Name == "L130FireBeast")
                return 5287;
            if (Name == "L133FlyingSnake")
                return 5288;
            if (Name == "L136Apparition")
                return 5289;
            if (Name == "L138Ghoul")
                return 5290;
            if (Name == "L139DarkSoldier")
                return 5291;
            if (Name == "L141BloodBat")
                return 5292;
            if (Name == "L143VenomousApe")
                return 5293;
            if (Name == "L144BitterSoul")
                return 5294;
            if (Name == "L146SoulBeast")
                return 5295;
            if (Name == "L148WingedEvil")
                return 5296;
            if (Name == "L115BluishSnake")
                return 5297;
            if (Name == "L120Condor")
                return 5298;
            if (Name == "L125GreenSnake")
                return 5299;
            if (Name == "L130MadBull")
                return 5300;
            if (Name == "L135TarzanApe")
                return 5301;
            if (Name == "L140BridledHawk")
                return 5302;
            if (Name == "L145HollowEvil")
                return 5303;
            if (Name == "L150BulkyGiant")
                return 5304;
            if (Name == "NightWolf")
                return 5305;
            if (Name == "GiantApe(Lev78)")
                return 5306;
            if (Name == "GoldEagle")
                return 5307;
            if (Name == "SandTroll")
                return 5308;
            if (Name == "WindBird")
                return 5309;
            if (Name == "MutantEvil")
                return 5310;
            if (Name == "GhostKing(Lev109)")
                return 5311;
            if (Name == "ShadowRat")
                return 5312;
            if (Name == "SpiritEnvoy")
                return 5313;
            if (Name == "BatWitch")
                return 5314;
            if (Name == "GhostGeneral(Lev123)")
                return 5315;
            if (Name == "InfernalSpirit")
                return 5316;
            if (Name == "BluishSnakeKing")
                return 5317;
            if (Name == "IcyBeast")
                return 5318;
            if (Name == "FlyingSnakeKing")
                return 5319;
            if (Name == "FieryGhostHead")
                return 5320;
            if (Name == "VampireGhost")
                return 5321;
            if (Name == "SplitterEvil")
                return 5322;
            if (Name == "BloodBatKing")
                return 5323;
            if (Name == "BlueScale")
                return 5324;
            if (Name == "BindingBeast")
                return 5325;
            if (Name == "RequiemBeast")
                return 5326;
            if (Name == "WingedEvilKing")
                return 5327;
            if (Name == "BlueSnakeKing")
                return 5328;
            if (Name == "ShriekingBird")
                return 5329;
            if (Name == "GreenSnakeKing")
                return 5330;
            if (Name == "FerociousBull")
                return 5331;
            if (Name == "NorthApeKing")
                return 5332;
            if (Name == "GoldHawkKing")
                return 5333;
            if (Name == "HowlingKing")
                return 5334;
            if (Name == "MountainLord")
                return 5335;
            if (Name == "L70Dralion")
                return 5336;
            if (Name == "L80TopExorcist")
                return 5337;
            if (Name == "L90FieryBird")
                return 5338;
            if (Name == "L100GoldenHorn")
                return 5339;
            if (Name == "L105ShadowBeast")
                return 5340;
            if (Name == "L110RoarWraith")
                return 5341;
            if (Name == "L115RedSerpent")
                return 5342;
            if (Name == "L120FireTroll")
                return 5343;
            if (Name == "L123Devourer")
                return 5344;
            if (Name == "L126EvilSteed")
                return 5345;
            if (Name == "L129AzureSnake")
                return 5346;
            if (Name == "L132DoomEnvoy")
                return 5347;
            if (Name == "L135AxeTroll")
                return 5348;
            if (Name == "L137EvilLord")
                return 5349;
            if (Name == "L139AzureKylin")
                return 5350;
            if (Name == "L141SpectreKing")
                return 5351;
            if (Name == "L143CloudSpirit")
                return 5352;
            if (Name == "L145Sirius")
                return 5353;
            if (Name == "L147BloodyEvil")
                return 5354;
            if (Name == "EvilBloodyBanshee")
                return 5355;
            if (Name == "EvilNetherTyrant")
                return 5356;
            if (Name == "EvilChillingSpook")
                return 5357;
            if (Name == "EvilDragonWraith")
                return 5358;
            if (Name == "QueenOfEvil")
                return 5359;
            if (Name == "L70ThirstyWolf")
                return 5014;
            if (Name == "L78IceApe")
                return 5015;
            if (Name == "L85MesmericHawk")
                return 5016;
            if (Name == "L93DustBeast")
                return 5017;
            if (Name == "L101ChillyBird")
                return 5018;
            if (Name == "L105HillBeast")
                return 5019;
            if (Name == "L109Ghost")
                return 5020;
            if (Name == "L113FlameRat")
                return 5021;
            if (Name == "L117SoulBanshee")
                return 5022;
            if (Name == "L121BatBanshee")
                return 5023;
            if (Name == "L123GhostHerald")
                return 5024;
            if (Name == "L126GoblinSoul")
                return 5025;
            if (Name == "L128BluishSnake")
                return 5026;
            if (Name == "L130FireBeast")
                return 5027;
            if (Name == "L133FlyingSnake")
                return 5028;
            if (Name == "L136Apparition")
                return 5029;
            if (Name == "L138Ghoul")
                return 5030;
            if (Name == "L139DarkSoldier")
                return 5031;
            if (Name == "L141BloodBat")
                return 5032;
            if (Name == "L143VenomousApe")
                return 5033;
            if (Name == "L144BitterSoul")
                return 5034;
            if (Name == "L146SoulBeast")
                return 5035;
            if (Name == "L148WingedEvil")
                return 5036;
            if (Name == "L115BluishSnake")
                return 5037;
            if (Name == "L120Condor")
                return 5038;
            if (Name == "L125GreenSnake")
                return 5039;
            if (Name == "L130MadBull")
                return 5040;
            if (Name == "L135TarzanApe")
                return 5041;
            if (Name == "L140BridledHawk")
                return 5042;
            if (Name == "L145HollowEvil")
                return 5043;
            if (Name == "L150BulkyGiant")
                return 5044;
            if (Name == "L85Thief")
                return 5697;
            if (Name == "L130Thief")
                return 5698;
            if (Name == "LonelyCloudBeast")
                return 4250;
            if (Name == "DistributorTransform")
                return 5552;
            if (Name == "3-starAcePheasant")
                return 1161;
            if (Name == "3-starWingedLord")
                return 1162;
            if (Name == "3-starWaterTerror")
                return 1163;
            if (Name == "3-starCruelAsura")
                return 1164;
            if (Name == "3-starSoulSlayer")
                return 1165;
            if (Name == "3-starDarkGlutton")
                return 1166;
            if (Name == "3-starAlienDragon")
                return 1167;
            if (Name == "3-starSnowSpider")
                return 1168;
            if (Name == "3-starFlameGiant")
                return 1169;
            if (Name == "LordofFear")
                return 1170;
            if (Name == "LordofHorror")
                return 1171;
            if (Name == "LordofDread")
                return 1172;
            if (Name == "LordofShock")
                return 1173;
            if (Name == "DemonSprite")
                return 1174;
            if (Name == "MichaelMyers")
                return 5747;
            if (Name == "4-StarFightingZombie")
                return 1253;
            if (Name == "4-StarDroughtZombie")
                return 1254;
            if (Name == "4-StarDemonie")
                return 1255;
            if (Name == "4-StarWerewolf")
                return 1256;
            if (Name == "4-StarBloodQueen")
                return 1257;
            if (Name == "5-StarWhiteGhost")
                return 1258;
            if (Name == "5-StarBloodyZombie")
                return 1259;
            if (Name == "5-StarPharaohMummy")
                return 1260;
            if (Name == "5-StarGhostCaptain")
                return 1261;
            if (Name == "GreatPheasant(Shifted)")
                return 1265;
            if (Name == "WingedLord(Shifted)")
                return 1266;
            if (Name == "WaterTerror(Shifted)")
                return 1267;
            if (Name == "RuthlessAsura(Shifted)")
                return 1268;
            if (Name == "SoulStrangler(Shifted)")
                return 1269;
            if (Name == "DarkGlutton(Shifted)")
                return 1270;
            if (Name == "AlienDragon(Shifted)")
                return 1271;
            if (Name == "ShadowSpider(Shifted)")
                return 1272;
            if (Name == "FlameGiant(Shifted)")
                return 1273;
            if (Name == "2-StarSnakemanL63")
                return 3249;
            if (Name == "1-StarWingedSnakeL28")
                return 3129;
            if (Name == "1-StarPheasant")
                return 3597;
            if (Name == "1-StarFireSpiritL43")
                return 3598;
            if (Name == "1-StarSandMonsterL68")
                return 3599;
            if (Name == "1-StarBladeGhost")
                return 4011;
            if (Name == "2-StarBattleGhost")
                return 4012;
            if (Name == "2-StarThunderApe")
                return 4013;
            if (Name == "2-StarTitanL77")
                return 4014;
            if (Name == "2-StarBloodyKing")
                return 4015;
            if (Name == "2-StarHawKing")
                return 4016;
            if (Name == "2-StarBullDevil")
                return 4017;
            if (Name == "2-StarDevilKing")
                return 4018;
            if (Name == "DemonMyers")
                return 4019;
            if (Name == "Gump(200BP)")
                return 5704;
            if (Name == "Gump(210BP)")
                return 5705;
            if (Name == "Gump(220BP)")
                return 5706;
            if (Name == "Gump(230BP)")
                return 5707;
            if (Name == "Gump(240BP)")
                return 5708;
            if (Name == "Gump(250BP)")
                return 5709;
            if (Name == "Gump(260BP)")
                return 5710;
            if (Name == "Gump(270BP)")
                return 5711;
            if (Name == "Gump(280BP)")
                return 5712;
            if (Name == "Gump(290BP)")
                return 5713;
            if (Name == "Gump(300BP)")
                return 5714;
            if (Name == "Gump(310BP)")
                return 5715;
            if (Name == "Gump(320BP)")
                return 5716;
            if (Name == "Gump(330BP)")
                return 5717;
            if (Name == "Gump(340BP)")
                return 5718;
            if (Name == "Gump(350BP)")
                return 5719;
            if (Name == "Gump(360BP)")
                return 5720;
            if (Name == "Gump(370BP)")
                return 5721;
            if (Name == "Gump(380BP)")
                return 5722;
            if (Name == "Gump(390BP)")
                return 5723;
            if (Name == "Gump(400BP)")
                return 5724;
            if (Name == "Gump(405BP)")
                return 5725;
            if (Name == "Gump(410BP)")
                return 5726;
            if (Name == "Gump(415BP)")
                return 5727;
            if (Name == "Gump(420BP)")
                return 5728;
            if (Name == "Gump(425BP)")
                return 5729;
            if (Name == "Gump(430BP)")
                return 5730;
            if (Name == "Gump(435BP)")
                return 5731;
            if (Name == "Gump(440BP)")
                return 5732;
            if (Name == "Gump(445BP)")
                return 5733;
            if (Name == "Gump(450BP)")
                return 5734;
            if (Name == "Gump(455BP)")
                return 5735;
            if (Name == "Gump(460BP)")
                return 5736;
            if (Name == "Gump(465BP)")
                return 5737;
            if (Name == "Gump(470BP)")
                return 5738;
            if (Name == "Gump(475BP)")
                return 5739;
            if (Name == "YellowTurkey")
                return 5803;
            if (Name == "GrayTurkey")
                return 5804;
            if (Name == "OrangeTurkey")
                return 5805;
            if (Name == "RedTurkey")
                return 5806;
            if (Name == "TurkeyBandit")
                return 5807;
            if (Name == "TurkeyRogue")
                return 5808;
            if (Name == "TurkeyKnight")
                return 5809;
            if (Name == "TurkeySwordman")
                return 5810;
            if (Name == "TurkeyLeader")
                return 5811;
            if (Name == "GiantTurkey")
                return 5812;
            if (Name == "GrayTurkey")
                return 5813;
            if (Name == "OrangeTurkey")
                return 5814;
            if (Name == "RedTurkey")
                return 5815;
            if (Name == "TurkeyBandit")
                return 5816;
            if (Name == "TurkeyRogue")
                return 5817;
            if (Name == "TurkeyKnight")
                return 5818;
            if (Name == "TurkeySwordman")
                return 5819;
            if (Name == "TurkeyLeader")
                return 5820;
            if (Name == "GiantTurkey")
                return 5821;
            if (Name == "AquaRoarBeast")
                return 1148;
            if (Name == "Dragon")
                return 1149;
            if (Name == "VenomousSnake")
                return 1150;
            if (Name == "ChestGenie")
                return 3045;
            if (Name == "Level1CelestialKylin")
                return 5822;
            if (Name == "Level2CelestialKylin")
                return 5823;
            if (Name == "Level3CelestialKylin")
                return 5824;
            if (Name == "Level4CelestialKylin")
                return 5825;
            if (Name == "Level5CelestialKylin")
                return 5826;
            if (Name == "Level6CelestialKylin")
                return 5827;
            if (Name == "Level7CelestialKylin")
                return 5828;
            if (Name == "Level8CelestialKylin")
                return 5829;
            if (Name == "Level9CelestialKylin")
                return 5830;
            if (Name == "Level10CelestialKylin")
                return 5831;
            if (Name == "Level11CelestialKylin")
                return 5832;
            if (Name == "Level12CelestialKylin")
                return 5833;
            if (Name == "Level13CelestialKylin")
                return 5834;
            if (Name == "Level14CelestialKylin")
                return 5835;
            if (Name == "Level15CelestialKylin")
                return 5836;
            if (Name == "FightPhantom")
                return 5947;
            if (Name == "DeathPhantom")
                return 5946;
            if (Name == "BloodPhantom")
                return 5945;
            if (Name == "L135DevilPhantom")
                return 5944;
            if (Name == "L130DevilPhantom")
                return 5943;
            if (Name == "L125DevilPhantom")
                return 5942;
            if (Name == "L120DevilPhantom")
                return 5941;
            if (Name == "L115DevilPhantom")
                return 5940;
            if (Name == "L110DevilPhantom")
                return 5939;
            if (Name == "L100DevilPhantom")
                return 5938;
            if (Name == "L90DevilPhantom")
                return 5937;
            if (Name == "L80DevilPhantom")
                return 5936;
            if (Name == "L70DevilPhantom")
                return 5935;
            if (Name == "L60DevilPhantom")
                return 5934;
            if (Name == "L50DevilPhantom")
                return 5933;
            if (Name == "Scylla")
                return 5950;
            if (Name == "Scylla")
                return 5859;
            if (Name == "FightDevil[Hard]")
                return 5858;
            if (Name == "DeathDevil[Hard]")
                return 5857;
            if (Name == "BloodDevil[Hard]")
                return 5856;
            if (Name == "L135Devil[Hard]")
                return 5855;
            if (Name == "L130Devil[Hard]")
                return 5854;
            if (Name == "L125Devil[Hard]")
                return 5853;
            if (Name == "L120Devil[Hard]")
                return 5852;
            if (Name == "L115Devil[Hard]")
                return 5851;
            if (Name == "L110Devil[Hard]")
                return 5850;
            if (Name == "L100Devil[Hard]")
                return 5849;
            if (Name == "L90Devil[Hard]")
                return 5848;
            if (Name == "L80Devil[Hard]")
                return 5847;
            if (Name == "L70Devil[Hard]")
                return 5846;
            if (Name == "L60Devil[Hard]")
                return 5845;
            if (Name == "L50Devil[Hard]")
                return 5844;
            if (Name == "FightDevil")
                return 5843;
            if (Name == "DeathDevil")
                return 5842;
            if (Name == "BloodDevil")
                return 5841;
            if (Name == "L90DevilClone")
                return 5955;
            if (Name == "L100DevilClone")
                return 5956;
            if (Name == "L110DevilClone")
                return 5957;
            if (Name == "L115DevilClone")
                return 5958;
            if (Name == "L120DevilClone")
                return 5959;
            if (Name == "L125DevilClone")
                return 5960;
            if (Name == "L130DevilClone")
                return 5961;
            if (Name == "L135DevilClone")
                return 5962;
            if (Name == "BloodClone")
                return 5963;
            if (Name == "DeathClone")
                return 5964;
            if (Name == "FightClone")
                return 5965;
            if (Name == "FuryL50Devil")
                return 5966;
            if (Name == "FuryL60Devil")
                return 5967;
            if (Name == "FuryL70Devil")
                return 5968;
            if (Name == "FuryL80Devil")
                return 5969;
            if (Name == "FuryL90Devil")
                return 5970;
            if (Name == "FuryL100Devil")
                return 5971;
            if (Name == "FuryL110Devil")
                return 5972;
            if (Name == "FuryL115Devil")
                return 5973;
            if (Name == "FuryL120Devil")
                return 5974;
            if (Name == "FuryL125Devil")
                return 5975;
            if (Name == "FuryL130Devil")
                return 5976;
            if (Name == "FuryL135Devil")
                return 5977;
            if (Name == "FuryBloodDevil")
                return 5978;
            if (Name == "FuryDeathDevil")
                return 5979;
            if (Name == "FuryFightDevil")
                return 5980;
            if (Name == "KetherNinja")
                return 5913;
            if (Name == "L1GiantDemon")
                return 6630;
            if (Name == "L1RockTurtle")
                return 6639;
            if (Name == "L2RockTurtle")
                return 6640;
            if (Name == "L3RockTurtle")
                return 6641;
            if (Name == "L4RockTurtle")
                return 6642;
            if (Name == "L5RockTurtle")
                return 6643;
            if (Name == "L6RockTurtle")
                return 6644;
            if (Name == "L7RockTurtle")
                return 6645;
            if (Name == "L8RockTurtle")
                return 6646;
            if (Name == "L9RockTurtle")
                return 6647;
            if (Name == "L10RockTurtle")
                return 6648;
            if (Name == "L11RockTurtle")
                return 6649;
            if (Name == "L12RockTurtle")
                return 6650;
            if (Name == "L13RockTurtle")
                return 6651;
            if (Name == "L14RockTurtle")
                return 6652;
            if (Name == "L15RockTurtle")
                return 6653;
            if (Name == "L16RockTurtle")
                return 6654;
            if (Name == "L17RockTurtle")
                return 6655;
            if (Name == "L18RockTurtle")
                return 6656;
            if (Name == "L19RockTurtle")
                return 6657;
            if (Name == "L20RockTurtle")
                return 6658;
            if (Name == "L1LotusDemon")
                return 6659;
            if (Name == "L2LotusDemon")
                return 6660;
            if (Name == "L3LotusDemon")
                return 6661;
            if (Name == "L4LotusDemon")
                return 6662;
            if (Name == "L5LotusDemon")
                return 6663;
            if (Name == "L6LotusDemon")
                return 6664;
            if (Name == "L7LotusDemon")
                return 6665;
            if (Name == "L8LotusDemon")
                return 6666;
            if (Name == "L9LotusDemon")
                return 6667;
            if (Name == "L10LotusDemon")
                return 6668;
            if (Name == "L11LotusDemon")
                return 6669;
            if (Name == "L12LotusDemon")
                return 6670;
            if (Name == "L13LotusDemon")
                return 6671;
            if (Name == "L14LotusDemon")
                return 6672;
            if (Name == "L15LotusDemon")
                return 6673;
            if (Name == "L16LotusDemon")
                return 6674;
            if (Name == "L17LotusDemon")
                return 6675;
            if (Name == "L18LotusDemon")
                return 6676;
            if (Name == "L19LotusDemon")
                return 6677;
            if (Name == "L20LotusDemon")
                return 6678;
            if (Name == "L1SierraBeast")
                return 6679;
            if (Name == "L2SierraBeast")
                return 6680;
            if (Name == "L3SierraBeast")
                return 6681;
            if (Name == "L4SierraBeast")
                return 6682;
            if (Name == "L5SierraBeast")
                return 6683;
            if (Name == "L6SierraBeast")
                return 6684;
            if (Name == "L7SierraBeast")
                return 6685;
            if (Name == "L8SierraBeast")
                return 6686;
            if (Name == "L9SierraBeast")
                return 6687;
            if (Name == "L10SierraBeast")
                return 6688;
            if (Name == "L11SierraBeast")
                return 6689;
            if (Name == "L12SierraBeast")
                return 6690;
            if (Name == "L13SierraBeast")
                return 6691;
            if (Name == "L14SierraBeast")
                return 6692;
            if (Name == "L15SierraBeast")
                return 6693;
            if (Name == "L16SierraBeast")
                return 6694;
            if (Name == "L17SierraBeast")
                return 6695;
            if (Name == "L18SierraBeast")
                return 6696;
            if (Name == "L19SierraBeast")
                return 6697;
            if (Name == "L20SierraBeast")
                return 6698;
            return 0;
        }
    }
}
