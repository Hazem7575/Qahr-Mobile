// [GiftCenter 2026-10-14] Wire contracts of the official Gift Center (Conquer.exe 7957, ProtoStudio schema.json).
//   2423 CMsgPresentPB          {1 int32 action, 2 uint32, 3 int32, 4 uint32, 5 uint32}
//   2050 CMsgPresentListPB      {1 int32 list type, 2 bool clear, 3 bool last chunk, 4 repeated CMsgPresentRecordPB}
//   2090 CMsgPresentUserPB      {1 int32 action, 2 uint32, 3 uint32, 4 uint32}
//   CMsgPresentRecordPB         {1 id, 2 counterpart uid, 3 counterpart name, 4 counterpart mesh, 5 kind, 6 item id,
//                                7 text id, 8 u64 unix time, 9 bool claimed, 10 bool new, 11 u64 yyyymmddHHMMSS}
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.GiftCenter
{
    [ProtoContract]
    public class GcPresentPB// 2423
    {
        [ProtoMember(1, IsRequired = true)] public int f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public int f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
    }

    [ProtoContract]
    public class GcRecordPB// CMsgPresentRecordPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public string f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public uint f7;
        [ProtoMember(8)] public ulong f8;
        [ProtoMember(9)] public bool f9;
        [ProtoMember(10)] public bool f10;
        [ProtoMember(11)] public ulong f11;
    }

    [ProtoContract]
    public class GcPresentListPB// 2050
    {
        [ProtoMember(1, IsRequired = true)] public int f1;
        [ProtoMember(2)] public bool f2;
        [ProtoMember(3)] public bool f3;
        [ProtoMember(4)] public List<GcRecordPB> f4 = new List<GcRecordPB>();
    }

    [ProtoContract]
    public class GcPresentUserPB// 2090
    {
        [ProtoMember(1, IsRequired = true)] public int f1;
        [ProtoMember(2, IsRequired = true)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
    }
}
