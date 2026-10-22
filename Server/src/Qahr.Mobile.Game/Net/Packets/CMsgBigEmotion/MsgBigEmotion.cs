using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgBigEmotion
    {
        [ProtoContract]
        public class MsgBigEmotionProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint PlayerUID;
            [ProtoMember(2, IsRequired = true)]
            public uint EmotionID;
            // [ProtoFix 2026-10-14] client CMsgBigEmotionPB has only #1 PlayerUID and #2 EmotionID
            // (Ghidra: its Process reads only those two). Members 3-10 were always-zero required fields; not serialized any more.
            public uint Member3; // was [ProtoMember(3, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member4; // was [ProtoMember(4, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member5; // was [ProtoMember(5, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member6; // was [ProtoMember(6, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member7; // was [ProtoMember(7, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member8; // was [ProtoMember(8, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member9; // was [ProtoMember(9, IsRequired = true)] // [ProtoFix 2026-10-14]
            public uint Member10; // was [ProtoMember(10, IsRequired = true)] // [ProtoFix 2026-10-14]
        }
        public static unsafe void GetBigEmotion(this ServerSockets.Packet stream, out MsgBigEmotionProto pQuery)
        {
            pQuery = new MsgBigEmotionProto();
            pQuery = stream.ProtoBufferDeserialize(pQuery);
        }
        public static unsafe ServerSockets.Packet CreateBigEmotion(this ServerSockets.Packet stream, MsgBigEmotionProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize((ushort)GamePackets.MsgBigEmotion);
            return stream;
        }
        [Packet((ushort)GamePackets.MsgBigEmotion)]
        private static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgBigEmotionProto Info;
            stream.GetBigEmotion(out Info);

            if (client.HairfaceStorage.Exists(Info.EmotionID, MsgHairfaceStorage.Type.Emoji))
            {
                Info.PlayerUID = client.Player.UID;
                client.Send(stream.CreateBigEmotion(Info));
                client.Player.View.SendView(stream.CreateBigEmotion(Info), true);
            }

        }
    }
}