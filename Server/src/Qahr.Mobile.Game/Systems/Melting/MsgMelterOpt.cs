using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgMelterOpt
    {
        [ProtoContract]
        public class MsgMelterOptProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2)]
            public uint EntityID;
            [ProtoMember(3)]
            public string EntityName;
            [ProtoMember(4, IsRequired = true)]
            public Item[] Items;
        }
        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public ulong ID; // [ProtoFix 2026-10-14] client CMsgMelterInfoPB.ID is uint64 (was uint)
            [ProtoMember(3, IsRequired = true)]
            public bool Bound;
            [ProtoMember(4, IsRequired = true)]
            public bool AnnounceMessage;
        }
        public static unsafe ServerSockets.Packet CreateMelterOpt(this ServerSockets.Packet stream, MsgMelterOptProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgMelterOpt);
            return stream;
        }
        public static unsafe void GetMelterOpt(this ServerSockets.Packet stream, out MsgMelterOptProto pQuery)
        {
            pQuery = new MsgMelterOptProto();
            pQuery = stream.ProtoBufferDeserialize<MsgMelterOptProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgMelterOpt)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.CreateBoxDialog("Sorry, the Melting System is now closed.");
            return;
        }
    }
}