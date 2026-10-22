using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueMainRank
    {
       [Flags]
       public enum RankType : byte
       {
           Union,
           Kingdom
       }
       [ProtoContract]
       public class MsgUnionRank
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Type;
           [ProtoMember(2, IsRequired = true)]
           public uint UID;
           [ProtoMember(3, IsRequired = true)]
           public uint UnionID;
           [ProtoMember(4, IsRequired = true)]
           public uint PlayerRank;
           [ProtoMember(5, IsRequired = true)]
           public RankType UnionRank;
           [ProtoMember(6, IsRequired = true)]
           public uint ServerID;
           [ProtoMember(7, IsRequired = true)]
           public string Name;
       }

       public static unsafe ServerSockets.Packet CreateLeagueMainRank(this ServerSockets.Packet stream, MsgUnionRank obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgOverheadLeagueInfo);
           return stream;
       }
       public static unsafe void GetLeagueMainRank(this ServerSockets.Packet stream, out MsgUnionRank pQuery)
       {
           pQuery = new MsgUnionRank();
           pQuery = stream.ProtoBufferDeserialize<MsgUnionRank>(pQuery);
       }

    }
}
