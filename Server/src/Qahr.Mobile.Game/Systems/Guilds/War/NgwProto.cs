// [NewGuildWar 2026-10-14] 2061 CMsgNewSynWar contracts (match ProtoStudio schema.json / out\cs\Msgnewsynwar.cs).
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.NewGuildWar
{
    [ProtoContract]
    public class NewSynWarListPB// CMsgNewSynWarListPB: 1 id, 2 value (u64, required), 3 name (required)
    {
        [ProtoMember(1)] public uint Id;
        [ProtoMember(2, IsRequired = true)] public ulong Value;
        [ProtoMember(3, IsRequired = true)] public string Name = "";
    }

    /// <summary>2061. Action map from the client Process (@0x1031693):
    ///   0 = in-war guild ranking (f19) for the HUD           1 = own points f3 + hero ranking (f19)
    ///   2 = info dialog reply (f2 0 = overview, 2 = Guild Pillar page) - the only requests the client sends
    ///   3 = a hero's points (f5 uid, f3 points)              4 = dead / prison timer (f3 0 = dead, 1 = prison)
    ///   5 = war start (screen effect + end countdown f3 s)   6 = countdown refresh (f3 s left, f18 s to start)
    ///   7 = Honor Pillar status f13 (1 refreshed, 2 destroyed)  8 = gate repaired  9 = buff pillar taken
    ///   10 = gate destroyed (f3 gate id 3000 left / 3001 right, f15 guild allowed to repair)</summary>
    [ProtoContract]
    public class NewSynWarPB
    {
        [ProtoMember(1, IsRequired = true)] public uint Action;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public ulong f3;
        [ProtoMember(4)] public string f4;     // gate A holder name (A = f5)
        [ProtoMember(5)] public uint f5;       // gate A id (3000 = left gate) / repair-door id
        [ProtoMember(6)] public uint f6;       // gate A holder guild id
        [ProtoMember(7)] public string f7;     // gate B holder name
        [ProtoMember(8)] public uint f8;       // gate B holder guild id
        [ProtoMember(9)] public string f9;     // buff pillar (f10) holder name
        [ProtoMember(10)] public uint f10;     // buff pillar id (100009 = Exorcism Pillar)
        [ProtoMember(11)] public string f11;   // other buff pillar holder name
        [ProtoMember(12)] public string f12;   // Guild Pillar No.1 guild
        [ProtoMember(13)] public uint f13;     // Honor Pillar status 0 / 1 / 2
        [ProtoMember(14)] public string f14;   // Honor Pillar guild
        [ProtoMember(15)] public uint f15;
        [ProtoMember(16)] public bool f16;     // refresh list
        [ProtoMember(17)] public bool f17;     // sort list
        [ProtoMember(18)] public uint f18;     // seconds until the war starts
        [ProtoMember(19)] public List<NewSynWarListPB> f19 = new List<NewSynWarListPB>();
    }
}
