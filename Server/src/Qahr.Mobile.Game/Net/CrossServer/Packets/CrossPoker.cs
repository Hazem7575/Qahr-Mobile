//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Qahr.Mobile.MsgInterServer.Packets
//{
//    public static class CrossPoker
//    {
//        public static unsafe ServerSockets.Packet InterCrossPoker(this ServerSockets.Packet stream, uint TableID)
//        {
//            stream.InitWriter();
//            stream.Write(TableID);

//            stream.Finalize(PacketTypes.InterServer_CrossPoker);
//            return stream;
//        }
//        public static unsafe void GetInterCrossPoker(this ServerSockets.Packet stream, out uint TableID)
//        {
//            TableID = stream.ReadUInt32();

//        }
//    }
//}
