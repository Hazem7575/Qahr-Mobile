// QahrMobile: Guild / World auction ("vendue") of the Conquer Online Mobile client - wire contracts.
// Field numbers and types come from libCQ2ClientEn.so (k*FieldNumber constants + SerializeWithCachedSizes:
// WriteUInt32 / WriteUInt64 / WriteBytes). See docs\GUILD_AUCTION.md (conquer-mobile) for the meaning of every field.
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.GuildAuction
{
    /// <summary>CVendueItemInfoPB: one lot. Client copy: CMsgItemVendue::Process -> CAuctionMgr::UpdateVendueItem.</summary>
    [ProtoContract]
    public class VendueItemInfoPB
    {
        [ProtoMember(1)] public ulong AutoId;        // lot id (key of the client's list)
        [ProtoMember(2)] public uint VendueType;     // activity: 1 Guild Defense, 2 Guild Onslaught, 3 World Auction (STR_VENDUE_ACTIVITY_TYPE_n)
        [ProtoMember(3)] public uint IdVendue;       // row id of ini\cq_vendue_item.dat (item, prices, money type); unknown id = lot ignored
        [ProtoMember(4)] public uint Flag;           // 1 = the receiving player has bid on this lot
        [ProtoMember(5)] public uint IdUser;         // current top bidder uid (0 = no bid yet: the start price is shown)
        [ProtoMember(6)] public uint Price;          // current top bid
        [ProtoMember(7)] public ulong BeginStamp;    // unix seconds (compared with CHero::CalcServerTimeStamp)
        [ProtoMember(8)] public ulong EndStamp;      // unix seconds
        [ProtoMember(9)] public string LastName;     // top bidder name
    }

    /// <summary>10417 CMsgItemVendue (server id 2084 MsgItemVendue). f1 is required by the client (IsInitialized).
    /// C->S: f1 0 = list request (f2 auction type), 1 = bid (f2 type, f5[0] = AutoId, IdVendue, IdUser, Price).
    /// S->C: f1 0 = list, 1 = one lot changed (f3 NRet: 1 bid ok / 0 "the current bid has changed", tip only when IdUser = me),
    ///       2 = "list changed": the client re-sends the list request for f2.</summary>
    [ProtoContract]
    public class ItemVenduePB
    {
        [ProtoMember(1, IsRequired = true)] public uint Action;
        [ProtoMember(2)] public uint JoinType;       // 0 = World Auction, 1 = Guild Auction
        [ProtoMember(3)] public uint Ret;
        [ProtoMember(4)] public uint MsgOrder;       // not read by the client
        [ProtoMember(5)] public List<VendueItemInfoPB> Info = new List<VendueItemInfoPB>();
    }

    /// <summary>CVendueItemListInfoPB: one finished sale (auction record row).</summary>
    [ProtoContract]
    public class VendueRecordPB
    {
        [ProtoMember(1)] public uint IdVendue;
        [ProtoMember(2)] public uint IdUser;         // winner (row highlighted when it is the viewer)
        [ProtoMember(3)] public uint MoneyType;      // 1 CPs, 2 bound CPs, 1002 phase points
        [ProtoMember(4)] public uint MoneyValue;     // = fixed price -> "Fixed Price", else "Auction Price"
        [ProtoMember(5)] public ulong Time;          // decimal MMddHHmmss (shown as %02d/%02d %02d:%02d:%02d)
        [ProtoMember(6)] public string LastName;
    }

    /// <summary>10418 CMsgItemVendueList (no id on the PC server: handled under the same id 10418).
    /// C->S: f1 type, f2 page (0 on first open, then page+1 on scroll), f3 rows per page (10).
    /// S->C: f1 type, f2 page, f4 total rows, f5 rows.</summary>
    [ProtoContract]
    public class VendueRecordListPB
    {
        [ProtoMember(1)] public uint JoinType;
        [ProtoMember(2)] public uint PageNo;
        [ProtoMember(3)] public uint PerPage;
        [ProtoMember(4)] public uint TotalAmount;
        [ProtoMember(5)] public List<VendueRecordPB> Info = new List<VendueRecordPB>();
    }

    /// <summary>10224 CMsgData (server 2227) action 0 with field 9 U64TimeStamp. The client's server clock
    /// (CHero::CalcServerTimeStamp, used by the auction countdowns) is only set from field 9, which the server's
    /// own MsgDataProto does not carry, so it would count from 0. Fields 2-8 = the same date as ServerTimerCreate.</summary>
    [ProtoContract]
    public class TimeSyncPB
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2, IsRequired = true)] public uint Year;
        [ProtoMember(3, IsRequired = true)] public uint Month;
        [ProtoMember(4, IsRequired = true)] public uint DayOfYear;
        [ProtoMember(5, IsRequired = true)] public uint DayOfMonth;
        [ProtoMember(6, IsRequired = true)] public uint Hour;
        [ProtoMember(7, IsRequired = true)] public uint Minute;
        [ProtoMember(8, IsRequired = true)] public uint Second;
        [ProtoMember(9)] public ulong TimeStamp;
    }
}
