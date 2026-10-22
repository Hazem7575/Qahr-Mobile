using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe static class NpcReply
    {
        [ProtoContract]
        public class NpcRequestProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ushort Action;
            [ProtoMember(2, IsRequired = true)]
            public uint NpcID;
            [ProtoMember(3, IsRequired = true)]
            public byte OptionID;
            [ProtoMember(4, IsRequired = true)]
            public byte InteractType;
            // [ProtoFix 2026-10-14] client CMsgNpcPB #5 is uint32 (varint), not string. A string member made
            // protobuf-net reject the packet whenever the client sent #5. Nullable keeps "absent" = null input.
            [ProtoMember(5)]
            public uint? Input;
        }
        public enum InteractTypes : byte
        {
            Dialog = 1,
            Option = 2,
            Input = 3,
            Avatar = 4,
            MessageBox = 6,
            Finish = 100,
            Page = 113,
        }

        public static unsafe void GetNpcRequest(this ServerSockets.Packet stream, out uint npcID, out byte option, out byte type, out NpcServerReplay.Mode action, out string input)
        {

            var proto = new NpcRequestProto();
            proto = stream.ProtoBufferDeserialize<NpcRequestProto>(proto);
            npcID = proto.NpcID;
            type = proto.InteractType;
            input = proto.Input.HasValue ? proto.Input.Value.ToString() : null; // [ProtoFix 2026-10-14] numeric #5 -> NPC string input
            option = proto.OptionID;
            action = (NpcServerReplay.Mode)proto.Action;
        }
        public static unsafe ServerSockets.Packet NpcReplyCreate(this ServerSockets.Packet stream, InteractTypes interactType
            , string text
            , ushort InputMaxLength
            , byte OptionID
            , bool display = true)
        {
            stream.InitWriter();
            stream.Write(0);
            stream.Write(InputMaxLength);
            stream.Write((byte)OptionID);
            stream.Write((byte)interactType);
            if (display)
                stream.Write(text);

            stream.Finalize(GamePackets.MsgTaskDialog);
            return stream;
        }
    }
}
