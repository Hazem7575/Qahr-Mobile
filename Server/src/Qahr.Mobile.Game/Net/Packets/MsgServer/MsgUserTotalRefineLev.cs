using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgUserTotalRefineLev
    {
       [ProtoContract]
       public class TotalRefineLev
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Type;
           [ProtoMember(2, IsRequired = true)]
           public uint UID;
           [ProtoMember(3, IsRequired = true)]
           public uint Level;
       }
       public static unsafe ServerSockets.Packet UserTotalRefineLevCreate(this ServerSockets.Packet stream, TotalRefineLev obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgUserTotalRefineLev);
           return stream;
       }
    }
}
