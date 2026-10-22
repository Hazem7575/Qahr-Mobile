using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile
{
    public static class Constants
    {
        public static readonly List<uint> ProtectMapSpells = new List<uint>() { };
        public static readonly ushort[] PriceUpdatePorf = new ushort[] { 36000, 36000, 36000, 36000, 36000, 36000, 18367, 12328, 7377, 6164, 3688, 3082, 3082, 3082, 3082, 2670, 1825, 1251, 866, 704 };
        public static readonly List<uint> MapCounterHits = new List<uint>() { 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 1005, 6000 };
        public static readonly List<uint> NoDropItems = new List<uint>() { 3071, 31254, 31255, 31256, 12288, 9991, 9992, 9993, 9990, 1036, 10364, 1764, 9855, 3954, 3820 };
        public static readonly List<uint> TreadeOrShop = new List<uint>() { 3053, 1858, 1860 };
        public static readonly List<uint> FreePkMap = new List<uint>() { 3071, 2071, 31254, 31255, 31256, 31257, 10133, 11222, 12288, 9991, 9992, 9993, 9990, 3071, 3030, 10741, 66666, 33333, 22222, 88888, 10133, 1036, 10364, 12288, 9991, 9992, 9993, 12546, 13249, 12602, 13140, 12546, 12800, 11331, 12602, 12275, 12799, 11996, 12665, 12601, 12816, 12099, 12667, 12605, 12602, 10741, 2071, 3030, 10133, 11, 7, 5, 15, 14, 12, 8, 20, 1730, 10134, 12099, 10133, 22348, 12344, 10070, 10504, 12344, 6521, 95152, 6781, 5661, 4447, 4448, 4449, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 4458, 1730, 1486, 1484, 1485, 14850, 1518, 1508, 11241, 15757, 15758, 15759, 15760, 15761, 15762, 15763, 15764, 15765, 15766, 15767, 15768, 10005, 10006, 10061, 10007, 10020, 10008, 10040, 10041, 10042, 10043, 10044, 10045, 1483, 1002, 1040, 10203, 15147, 1518, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 4057, 5053, 5054, 5055, 5056, 5057, 5058, 5053, 5054, 5055, 5056, 5057, 5058, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 6011, 3998, 6784, 5342, 8009, 2353, 5339, 6964, 3071, 6546, 3935, 8437, 9988, 5599, 2071, 7357, 6521, 3825, 5342, 6891, 8521, 12099, 6729, 1138, 6000, 6001, 1505, 1005, 1038, 9855, 1508/*PkWar*/, Game.MsgTournaments.MsgSuperGuildWar.MapID, Game.MsgTournaments.MsgCaptureTheFlag.MapID };
        public static readonly List<uint> BlockAttackMap = new List<uint>() { 3071, 31254, 31255, 31256, 31257, 10133, 12288, 9991, 9992, 9993, 9990, 3030, 1036, 10364, 12288, 9991, 9992, 9993, 10835,16785,11196,3030,11127,
            16786,3860, 3954, 15757, 15758, 15759, 15760, 15761, 15762, 15762, 15763, 15764, 15765, 15766, 15767, 15768, 10061, 10005, 10006, 10762, 10007, 10020, 10008, 10040, 10041, 10042, 10764, 10604, 10043, 10044, 10045, 2353, 10203, 15147, 1040, 3825, 6072, 3830, 5040, 3820, 1004, 3831, 3832, 3834, 3826, 3827, 3828, 3829, 3833, 9995, 1068, 4020, 9855, 4003, 4006, 4008, 4009, 1860, 3053, 1780, 1779/*Ghost Map*/, 9972, 1806, 1002, 3954, 3081, 1036, 1008, 601, 1006, 3030, 1039, 4000, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, (uint)Game.MsgTournaments.MsgSteedRace.Maps.DungeonRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IceRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IslandRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.LavaRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.MarketRace };
        public static readonly List<uint> BlockTeleportMap = new List<uint>() { 22330, 22331, 22333, 22334, 22335, 5050, 4057, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070, 601, 6000, 5040, 6001, 1005, 9855, 3053, 1860, 3852, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, 1768 };
        public static List<string> SuitRank = new List<string>
	{
		"2c".ToLower(),
		"3c".ToLower(),
		"4c".ToLower(),
		"5c".ToLower(),
		"6c".ToLower(),
		"7c".ToLower(),
		"8c".ToLower(),
		"9c".ToLower(),
		"Tc".ToLower(),
		"Jc".ToLower(),
		"Qc".ToLower(),
		"Kc".ToLower(),
		"Ac".ToLower(),
		"2d".ToLower(),
		"3d".ToLower(),
		"4d".ToLower(),
		"5d".ToLower(),
		"6d".ToLower(),
		"7d".ToLower(),
		"8d".ToLower(),
		"9d".ToLower(),
		"Td".ToLower(),
		"Jd".ToLower(),
		"Qd".ToLower(),
		"Kd".ToLower(),
		"Ad".ToLower(),
		"2h".ToLower(),
		"3h".ToLower(),
		"4h".ToLower(),
		"5h".ToLower(),
		"6h".ToLower(),
		"7h".ToLower(),
		"8h".ToLower(),
		"9h".ToLower(),
		"Th".ToLower(),
		"Jh".ToLower(),
		"Qh".ToLower(),
		"Kh".ToLower(),
		"Ah".ToLower(),
		"2s".ToLower(),
		"3s".ToLower(),
		"4s".ToLower(),
		"5s".ToLower(),
		"6s".ToLower(),
		"7s".ToLower(),
		"8s".ToLower(),
		"9s".ToLower(),
		"Ts".ToLower(),
		"Js".ToLower(),
		"Qs".ToLower(),
		"Ks".ToLower(),
		"As".ToLower()
	};
        public static List<string> HighCard = new List<string>
	{
		"2".ToLower(),
		"3".ToLower(),
		"4".ToLower(),
		"5".ToLower(),
		"6".ToLower(),
		"7".ToLower(),
		"8".ToLower(),
		"9".ToLower(),
		"T".ToLower(),
		"J".ToLower(),
		"Q".ToLower(),
		"K".ToLower(),
		"A".ToLower()
	};
    }
}
