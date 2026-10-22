// [Auction 2026-10-14] Wire contracts of the 7957 Auction House (field numbers / types = ProtoStudio data\schema.json,
// meaning = Ghidra RE of Conquer.exe 7957, see AuctionModule.cs header).
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.AuctionHouse
{
    /// <summary>One row of the browse list (2399 f7). Client copy @0x117c5ce -> 96-byte row (key = f3).</summary>
    [ProtoContract]
    public class AhItemInfoPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;   // required, not read by the client
        [ProtoMember(2)] public uint f2;                      // not read
        [ProtoMember(3)] public uint f3;                      // listing id (row key)
        [ProtoMember(4)] public ulong f4;                     // row +4 (u16), always 0 here
        [ProtoMember(5)] public uint f5;                      // server id (0 = "local" row -> client wants MsgItemInfo mode 12)
        [ProtoMember(6)] public uint f6;                      // seller uid (Buy button disabled on own rows)
        [ProtoMember(7)] public uint f7;                      // row +0x10, unused
        [ProtoMember(8)] public uint f8;                      // unit price (CPs)
        [ProtoMember(9)] public uint f9;                      // remaining seconds
        [ProtoMember(10)] public uint f10;                    // itemtype
        [ProtoMember(11)] public int f11;                     // amount
        [ProtoMember(12)] public uint f12;                    // bless        (tip item +0x254)
        [ProtoMember(13)] public uint f13;                    // durability   (FUN_01140c8b)
        [ProtoMember(14)] public uint f14;                    // max durability (tip item +0x64)
        [ProtoMember(15)] public uint f15;                    // relic/attribute 1..5 (FUN_01145ba7)
        [ProtoMember(16)] public uint f16;
        [ProtoMember(17)] public uint f17;
        [ProtoMember(18)] public uint f18;
        [ProtoMember(19)] public int f19;
        [ProtoMember(20)] public int f20;                     // mythsoul exp (FUN_01144e1d)
        [ProtoMember(21)] public int f21;                     // mutation id  (FUN_01144e36)
        [ProtoMember(22)] public int f22;                     // upsurge id   (FUN_01144e46)
    }

    /// <summary>2399 CMsgAuctionItem. C->S (@0x116ab51): f1 cross flag, f3 page (0 based), f4 class, f5 view filter, f6 name.
    /// S->C (@0x117c5ce): f2 total results, f3 page, f7 rows.</summary>
    [ProtoContract]
    public class AhAuctionItemPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public ulong f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public string f6;
        [ProtoMember(7)] public List<AhItemInfoPB> f7 = new List<AhItemInfoPB>();
    }

    [ProtoContract]
    public class AhDirListItemPB
    {
        [ProtoMember(1, IsRequired = true)] public ulong f1;  // sub class id
        [ProtoMember(2, IsRequired = true)] public uint f2;   // active listings
        [ProtoMember(3)] public uint f3;                      // lowest unit price
    }

    /// <summary>2012 CMsgAuctionDirList. C->S (@0xe6c022): f1 cross flag, f2 class. S->C (@0xe843dd): echo + f3 rows.</summary>
    [ProtoContract]
    public class AhDirListPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2, IsRequired = true)] public ulong f2;
        [ProtoMember(3)] public List<AhDirListItemPB> f3 = new List<AhDirListItemPB>();
    }

    /// <summary>2186 CMsgNosuch (cross-server auction actions): f1 action, f2 u32 data, f3 u64 data.</summary>
    [ProtoContract]
    public class AhNosuchPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public List<uint> f2 = new List<uint>();
        [ProtoMember(3)] public List<ulong> f3 = new List<ulong>();
    }

    /// <summary>2415 CMsgMyNosuchQuery: my listings (f1 = 0 rows) - client @0xf0d518.</summary>
    [ProtoContract]
    public class AhMyItemPB
    {
        [ProtoMember(1)] public uint f1;                      // 0 = selling row (1 = buy request row, not used)
        [ProtoMember(2)] public uint f2;                      // listing id
        [ProtoMember(3)] public ulong f3;                     // row +4 (u16)
        [ProtoMember(4)] public uint f4;                      // server id
        [ProtoMember(5)] public uint f5;                      // seller uid
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public uint f7;                      // unit price
        [ProtoMember(8)] public uint f8;                      // remaining seconds (0 = expired)
        [ProtoMember(9)] public uint f9;                      // itemtype
        [ProtoMember(10)] public int f10;                     // amount
        [ProtoMember(11)] public uint f11;                    // bless
        [ProtoMember(12)] public uint f12;                    // durability
        [ProtoMember(13)] public uint f13;                    // max durability
        [ProtoMember(14)] public uint f14;                    // attributes 1..5
        [ProtoMember(15)] public uint f15;
        [ProtoMember(16)] public uint f16;
        [ProtoMember(17)] public uint f17;
        [ProtoMember(18)] public int f18;
        [ProtoMember(19)] public int f19;                     // mythsoul exp
        [ProtoMember(20)] public int f20;                     // mutation id
        [ProtoMember(21)] public int f21;                     // upsurge id
    }

    [ProtoContract]
    public class AhMyQueryPB
    {
        [ProtoMember(1)] public List<AhMyItemPB> f1 = new List<AhMyItemPB>();
    }

    /// <summary>2564 CMsgAuctionTimeOutList (@0x117c797): f1 page, f2 total, f3 item ids (details follow as MsgItemInfo mode 15).</summary>
    [ProtoContract]
    public class AhTimeOutListPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public List<uint> f3 = new List<uint>();
    }
}
