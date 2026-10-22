using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        // [Unanswered B4 2026-10-14] 2274 is protobuf CMsgFlushExpPB on the 7957 client, not a raw struct.
        // Client Process (0x10c1cc6): f4==0 -> weapon-skill lookup by id f3 (maps +0x1d40/54/68, same as MsgWeaponSkill 2127),
        // sets skill.exp(+0x10)=f1, skill.needExp(+0x14)=f2, then refreshes UI 0x3ff. Same field order as the old struct.
        public static unsafe ServerSockets.Packet UpdateProfExperienceCreate(this ServerSockets.Packet stream, uint Experience, uint NeededExperience, uint ID)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new MsgFlushExpProto { Experience = Experience, NeededExperience = NeededExperience, ID = ID, Type = MsgFlushExpProto.TypeWeaponSkill });
            stream.Finalize(GamePackets.MsgFlushExp);
            return stream;
        }
    }

    // [Unanswered B4 2026-10-14] client CMsgFlushExpPB (msgflushexp.pb.cc): 1..4 all uint32.
    [ProtoBuf.ProtoContract]
    public class MsgFlushExpProto
    {
        public const uint TypeWeaponSkill = 0;
        [ProtoBuf.ProtoMember(1, IsRequired = true)]
        public uint Experience;
        [ProtoBuf.ProtoMember(2, IsRequired = true)]
        public uint NeededExperience;
        [ProtoBuf.ProtoMember(3, IsRequired = true)]
        public uint ID;
        [ProtoBuf.ProtoMember(4, IsRequired = true)]
        public uint Type;
    }
}
