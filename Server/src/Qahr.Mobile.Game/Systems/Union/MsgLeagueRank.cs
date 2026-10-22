using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueRank
    {
       public enum ActionID : ushort
       {
           ShowTop3Union = 0,
           ShowAllUnions = 1
       }
       public static unsafe void GetLeagueRank(this ServerSockets.Packet stream,out ActionID type,out ushort count
           ,out ushort Page,out byte dwparam,out ushort PageCount)
       {
           type = (ActionID) stream.ReadUInt16();
           count = stream.ReadUInt16();
           Page = stream.ReadUInt16();
           dwparam = stream.ReadUInt8();
           PageCount = stream.ReadUInt16();
       }
       public static unsafe void GetItemLeagueRank(this ServerSockets.Packet stream, out uint ServerID, out uint GoldBricks, out string Name, out string LeaderName)
       {
           ServerID = stream.ReadUInt32();
           GoldBricks = stream.ReadUInt32();
           Name = stream.ReadCString(16);
           LeaderName = stream.ReadCString(16);
       }

       public static unsafe ServerSockets.Packet LeagueRankCreate(this ServerSockets.Packet stream, ActionID type, ushort count
           ,ushort Page,byte dwparam, ushort PageCount  )
       {
           stream.InitWriter();
           stream.Write((ushort)type);
           stream.Write(count);
           stream.Write(Page);
           stream.Write(dwparam);
           stream.Write(PageCount);
           return stream;
       }


       public static unsafe ServerSockets.Packet LeagueRankCreate(this ServerSockets.Packet stream, ActionID type, uint pagecount
           , byte dwparam, uint count, ushort dwparam2)
        {
            stream.InitWriter();
            stream.Write((ushort)type);
            stream.Write(pagecount);
            stream.Write(dwparam);
            stream.Write(count);
            stream.Write(dwparam2);
            return stream;
        }
       public static unsafe ServerSockets.Packet AddItemLeagueRank(this ServerSockets.Packet stream, uint GoldBricks, string Name, string LeaderName, uint dwparam)
        {
            stream.Write(GoldBricks);
            stream.Write(Name, 32);
            stream.Write(LeaderName, 32);
            stream.Write(dwparam);
            return stream;
        }

       public static unsafe ServerSockets.Packet AddItemLeagueRank(this ServerSockets.Packet stream,uint ServerID, uint GoldBricks, string Name, string LeaderName)
       {
           stream.Write(ServerID);
           stream.Write(GoldBricks);
           stream.Write(Name, 32);
           stream.Write(LeaderName, 32);
          
           return stream;
       }
       public static unsafe ServerSockets.Packet InterServerLeagueRankFinalize(this ServerSockets.Packet stream)
       {
           stream.Finalize(MsgInterServer.PacketTypes.InterServer_UnionRanks);
           return stream;
       }
       public static unsafe ServerSockets.Packet LeagueRankFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgLeagueRank);
            return stream;
        }
    }
}
