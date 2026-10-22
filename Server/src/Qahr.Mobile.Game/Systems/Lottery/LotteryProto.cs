// [Lottery 2026-10-14] Wire contracts (Conquer.exe 7957, ProtoStudio schema.json).
//   2005 CMsgGlobalLotteryPB          {1 Type, 2 PrizesType, 3 GUI, 4 Time, 5 Remain(string), 6 repeated CBigPrizeInfoPB{1,2}}
//   2391 CMsgGlobalLotteryRankListPB  {1 Page, 2 TotalPages, 3 RegistedCount, 4 repeated CPrizeInfoPB{1 PickItem, 2 Name}}
//   2102 CMsgCardsLotteryOptPB        {1 action (required), 2 type (required), 3 card, 4 repeated CLotteryInfoPB{1,2}}
//   2069 CMsgCardsLotteryRankListPB   {1 (required), 2 type, 3 total pages, 4 page, 5 repeated CRankInfoPB{2 pool id, 3 name}}
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Lottery
{
    [ProtoContract]
    public class LtPairPB// CBigPrizeInfoPB / CLotteryInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
    }

    [ProtoContract]
    public class LtGlobalLotteryPB// 2005
    {
        [ProtoMember(1)] public uint Type;
        [ProtoMember(2)] public uint PrizesType;
        [ProtoMember(3)] public uint GUI;
        [ProtoMember(4)] public uint Time;
        [ProtoMember(5)] public string Remain;
        [ProtoMember(6)] public List<LtPairPB> TotalCount = new List<LtPairPB>();
    }

    [ProtoContract]
    public class LtPickerPB// CPrizeInfoPB
    {
        [ProtoMember(1)] public uint PickItem;
        [ProtoMember(2)] public string Name;
    }

    [ProtoContract]
    public class LtGlobalLotteryRankPB// 2391
    {
        [ProtoMember(1)] public uint Page;
        [ProtoMember(2)] public uint TotalPages;
        [ProtoMember(3)] public uint RegistedCount;
        [ProtoMember(4)] public List<LtPickerPB> Pickers = new List<LtPickerPB>();
    }

    [ProtoContract]
    public class LtCardsOptPB// 2102
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2, IsRequired = true)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public List<LtPairPB> f4 = new List<LtPairPB>();
    }

    [ProtoContract]
    public class LtRankInfoPB// CRankInfoPB
    {
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public string f3;
    }

    [ProtoContract]
    public class LtCardsRankPB// 2069
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public List<LtRankInfoPB> f5 = new List<LtRankInfoPB>();
    }
}
