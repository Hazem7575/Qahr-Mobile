using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: the three protobuf messages of the client's Dune Wanderer window (CFDlgXiaKeJueXingMain). Mobile-only
    /// ids (no server id, not in Data\mobile_ids.txt), so they go out unchanged. Field numbers = the client's
    /// CMsg*PB::k*FieldNumber constants; IsRequired = always on the wire (the client tests the has-bit of IdUser).
    /// Read by DuneNet (receive) and every Dune* system (send).
    /// </summary>
    public static class DunePackets
    {
        public const ushort Glyph = 10395;      // CMsgWenZhang: glyph bag, talents
        public const ushort Insight = 10396;    // CMsgWenZhangLottery: Insight draw, wishlist
        public const ushort Meridians = 10438;  // CMsgMeridians: meridians, acupoints, points, glyph holes
    }

    // ------------------------------------------------------------------ 10438 CMsgMeridians
    [ProtoContract]
    public class MeridiansMsg
    {
        [ProtoMember(1, IsRequired = true)] public uint Action;
        [ProtoMember(2)] public int Ret;
        [ProtoMember(3, IsRequired = true)] public uint IdUser;
        [ProtoMember(4)] public uint Data;
        [ProtoMember(5)] public uint Order;
        [ProtoMember(6)] public List<MeridianInfo> Meridians = new List<MeridianInfo>();
        [ProtoMember(7)] public List<AcupointInfo> Acupoints = new List<AcupointInfo>();
    }

    [ProtoContract]
    public class MeridianInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint Id;
        [ProtoMember(2, IsRequired = true)] public int Level;
        [ProtoMember(3, IsRequired = true)] public uint OpenHoles;// bit0..2 = hole 1..3
        [ProtoMember(4, IsRequired = true)] public uint Glyph1;
        [ProtoMember(5, IsRequired = true)] public uint Glyph2;
        [ProtoMember(6, IsRequired = true)] public uint Glyph3;
    }

    [ProtoContract]
    public class AcupointInfo
    {
        [ProtoMember(1, IsRequired = true)] public int Type;
        [ProtoMember(2, IsRequired = true)] public int Quality;
    }

    // ------------------------------------------------------------------ 10396 CMsgWenZhangLottery (CHundredWeaponLotteryInfo)
    [ProtoContract]
    public class InsightMsg
    {
        [ProtoMember(1, IsRequired = true)] public uint Action;
        [ProtoMember(2)] public uint Data;
        [ProtoMember(3)] public uint Order;
        [ProtoMember(4)] public List<DrawResult> Results = new List<DrawResult>();
        [ProtoMember(5)] public List<WishInfo> Wishes = new List<WishInfo>();
        [ProtoMember(6, IsRequired = true)] public int Pool;// 4 Yang state, 5 Yin state (the client's jackpot type)
        [ProtoMember(7)] public uint DrawNum;
    }

    [ProtoContract]
    public class DrawResult
    {
        [ProtoMember(1, IsRequired = true)] public uint RewardId;// hundred_soul_lottery row
        [ProtoMember(2)] public uint GlyphType;
        [ProtoMember(3)] public uint Life;
        [ProtoMember(4)] public uint Attack;
        [ProtoMember(5)] public uint Defence;
        [ProtoMember(6)] public uint MagicDefence;
        [ProtoMember(7)] public uint AttrType;
        [ProtoMember(8)] public uint AttrValue;
        [ProtoMember(12)] public uint GuaranteeNum;
    }

    [ProtoContract]
    public class WishInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;// 6 acupoint, 3 glyph
        [ProtoMember(2, IsRequired = true)] public uint TypeEx;// meridian, or glyph suit
        [ProtoMember(3, IsRequired = true)] public uint Amount;// request: slot 1/2; reply: 1 = obtained today
    }

    // ------------------------------------------------------------------ 10395 CMsgWenZhang
    [ProtoContract]
    public class GlyphMsg
    {
        [ProtoMember(1, IsRequired = true)] public uint Action;
        [ProtoMember(2, IsRequired = true)] public uint IdUser;
        [ProtoMember(3)] public uint Order;// bit0 = first packet (clear), bit1 = last packet (refresh)
        [ProtoMember(4)] public bool Ret;
        [ProtoMember(5)] public List<GlyphInfo> Glyphs = new List<GlyphInfo>();
        [ProtoMember(7)] public List<TalentInfo> Talents = new List<TalentInfo>();
    }

    [ProtoContract]
    public class GlyphInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint Id;
        [ProtoMember(2)] public uint Type;
        [ProtoMember(3)] public uint Life;
        [ProtoMember(4)] public uint Attack;
        [ProtoMember(5)] public uint Defence;
        [ProtoMember(6)] public uint MagicDefence;
        [ProtoMember(7)] public uint AttrType;
        [ProtoMember(8)] public uint AttrValue;
        [ProtoMember(9)] public uint Pos;// 0 = bag; meridian*10 + hole when embedded
    }

    [ProtoContract]
    public class TalentInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint Tree;
        [ProtoMember(2, IsRequired = true)] public uint ActiveNodes;// bit k = node k+1
        [ProtoMember(3, IsRequired = true)] public uint Points;
    }
}
