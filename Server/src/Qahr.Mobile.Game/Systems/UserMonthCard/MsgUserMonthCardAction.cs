//using Qahr.Mobile.Client;
//using ProtoBuf;
//using System;

//namespace Qahr.Mobile.Game.MsgServer
//{
//    public unsafe static partial class MsgBuilder
//    {
//        public static void GetUserMonthCardAction(this ServerSockets.Packet stream, out MsgUserMonthCardAction.ProtoStructure pQuery)
//        {
//            pQuery = new MsgUserMonthCardAction.ProtoStructure();
//            pQuery = stream.ProtoBufferDeserialize<MsgUserMonthCardAction.ProtoStructure>(pQuery);
//        }
//        public static ServerSockets.Packet CreateUserMonthCardAction(this ServerSockets.Packet stream, MsgUserMonthCardAction.ProtoStructure obj)
//        {
//            stream.InitWriter();
//            stream.ProtoBufferSerialize(obj);
//            stream.Finalize(GamePackets.MsgUserMonthCardAction);
//            return stream;
//        }
//    }
//    public class MsgUserMonthCardAction
//    {
//        [Flags]
//        public enum Types : uint
//        {
//            Buy = 0,
//        }
//        [ProtoContract]
//        public class ProtoStructure
//        {
//            [ProtoMember(1, IsRequired = true)]
//            public Types type;

//            [ProtoMember(2, IsRequired = true)]
//            public byte ID;
           
//        }
//        [PacketAttribute(GamePackets.MsgUserMonthCardAction)]
//        public unsafe static void Process(GameClient client, ServerSockets.Packet stream)
//        {
//            ProtoStructure Info;
//            stream.GetUserMonthCardAction(out Info);
//            switch (Info.type)
//            {
//                case Types.Buy:
//                    {
                    
//                        client.Send(stream.CreateUserMonthCardAction(Info));
//                        break;
//                    }
//            }
//        }
//    }
//}