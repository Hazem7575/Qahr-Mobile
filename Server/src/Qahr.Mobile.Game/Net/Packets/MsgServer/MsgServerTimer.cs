using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        [ProtoContract]
        public class MsgDataProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public uint Year; // [ProtoFix 2026-10-14] client CMsgDataPB uint32 (was int)
            [ProtoMember(3, IsRequired = true)]
            public uint Month; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(4, IsRequired = true)]
            public uint DayOfYear; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(5, IsRequired = true)]
            public uint DayOfMonth; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(6, IsRequired = true)]
            public uint Hour; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(7, IsRequired = true)]
            public uint Minute; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(8, IsRequired = true)]
            public uint Second; // [ProtoFix 2026-10-14] client uint32 (was int)
        }
        public static unsafe ServerSockets.Packet ServerTimerCreate(this ServerSockets.Packet stream)
        {
            stream.InitWriter();
            // QahrMobile: same fields + #9 = unix seconds (client CHero::SetServerTimeStamp; Weekly Cards / Daily Packs compare
            // their times with it, it was 0 before). The type lives in Mobile\MobileWeekCard.cs so this works without a restart.
            stream.ProtoBufferSerialize(global::Qahr.Mobile.Phone.MobileWeekCard.Time());

            stream.Finalize(GamePackets.MsgData);
            return stream;
        }
        public static unsafe ServerSockets.Packet ServerInfoCreate(this ServerSockets.Packet stream, uint Vigor)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new MsgDataProto() { Year = Vigor, Type = 2 }); // [ProtoFix 2026-10-14] no (int) cast: Vigor > 2^31 no longer goes negative

            stream.Finalize(GamePackets.MsgData);
            return stream;
        }
        // [Unanswered B5 2026-10-14] 2227 type 4 answer: client CMsgData::Process type 4 calls
        // FUN_0117c24f(f2 = key, f3 = value). Fields 4..8 are written as 0 (IsRequired) and ignored for type 4.
        public static unsafe ServerSockets.Packet ServerDataValueCreate(this ServerSockets.Packet stream, uint type, uint key, uint value)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new MsgDataProto() { Type = type, Year = key, Month = value });
            stream.Finalize(GamePackets.MsgData);
            return stream;
        }
        [ProtoContract]
        public class NirvanaPacket
        {
            [ProtoMember(1, IsRequired = true)]
            public ulong ActionID;
            [ProtoMember(2, IsRequired = true)]
            public ulong DeathTime;
        }
        public static unsafe ServerSockets.Packet NirvanaRune(this ServerSockets.Packet stream, uint DeathTime)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new NirvanaPacket()
            {
                ActionID = 5,
                DeathTime = DeathTime,
            });
            stream.Finalize((ushort)GamePackets.MsgData);//2227
            return stream;
        }
        public static unsafe ServerSockets.Packet ServerVerCreate(this ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write(0);//Server Type (1=Classic)
            stream.Write(0);//Potency Type
            stream.Finalize(GamePackets.MsgServerInfo);
            return stream;
        }

    }
}
