using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueRobOpt
    {
       [ProtoContract]
       public class RobOpt
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Type { get; set; }

           [ProtoMember(2, IsRequired = true)]
           public uint Unknown2 { get; set; }

           // [ProtoFix 2026-10-14] client CMsgLeagueRobOptPB #3 is repeated CMsgLeagueRobOptServerPB (was a single uint)
           [ProtoMember(3, IsRequired = false)]
           public List<RobOptServer> ID { get; set; } = new List<RobOptServer>();

           // [ProtoFix 2026-10-14] client CMsgLeagueRobOptPB has no #4: the name now travels inside #3 (RobOptServer.Name = f2).
           public string Name { get; set; } // was [ProtoMember(4, IsRequired = false)] // [ProtoFix 2026-10-14]
       }

       // [ProtoFix 2026-10-14] matches client CMsgLeagueRobOptServerPB (1 required uint32, 2/3 optional string)
       [ProtoContract]
       public class RobOptServer
       {
           [ProtoMember(1, IsRequired = true)]
           public uint ID { get; set; }

           [ProtoMember(2)]
           public string Name { get; set; }

           [ProtoMember(3)]
           public string Name2 { get; set; }
       }

       public static unsafe ServerSockets.Packet CreateLeagueRobOpt(this ServerSockets.Packet stream, RobOpt obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgLeagueRobOpt);

           return stream;
       }
       public static unsafe void GetLeagueRobOpt(this ServerSockets.Packet stream, out RobOpt pQuery)
       {
           pQuery = new RobOpt();
           pQuery = stream.ProtoBufferDeserialize<RobOpt>(pQuery);
       }
    }
}
