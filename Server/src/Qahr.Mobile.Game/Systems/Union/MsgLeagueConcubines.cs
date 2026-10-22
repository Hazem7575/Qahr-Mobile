using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueConcubines
    {

       [ProtoContract]
       public class LeagueImperialHarem
       {
           [ProtoMember(1, IsRequired = true)]
           public int type;

           [ProtoMember(2)]
           public ImperialHarem[] ImperialHarems;
       
       }
       [ProtoContract(SkipConstructor=true)]
       public class ImperialHarem
       {
           
           public ImperialHarem( Role.Instance.Union.Member member)
           {
               UID = member.UID;
               Name = member.Name;
               Class = member.Class;
               Level = (byte)member.Level;
               if (member.IsOnline)
                   BattlePower = (uint)member.Owner.Player.BattlePower;
               Exploits = member.Exploits;
               NobilityRank = (uint)member.NobilityRank;
               Online = (byte)(member.IsOnline ? 1 : 0);
               Mesh = member.Mesh;
               Rank = (uint)member.Rank;
           }
           [ProtoMember(1, IsRequired = true)]
           public uint UID;
           [ProtoMember(2, IsRequired = true)]
           public uint Rank;
           [ProtoMember(3, IsRequired = true)]
           public string Name;
           [ProtoMember(4, IsRequired = true)]
           public uint Class;
           [ProtoMember(5, IsRequired = true)]
           public byte Level;
           [ProtoMember(6, IsRequired = true)]
           public uint BattlePower;
           [ProtoMember(7, IsRequired = true)]
           public uint Exploits;
           [ProtoMember(8, IsRequired = true)]
           public uint NobilityRank;
           [ProtoMember(9, IsRequired = true)]
           public uint Online;
           [ProtoMember(10, IsRequired = true)]
           public uint Mesh;

       }
       public static unsafe ServerSockets.Packet CreateLeagueConcubines(this ServerSockets.Packet stream, LeagueImperialHarem obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgLeagueConcubines);
           return stream;
       }
    }
}
