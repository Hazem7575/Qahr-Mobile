using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgGoldLeaguePoint
    {

       [ProtoContract]
       public class GoldLeaguePoint
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Points { get; set; }

           [ProtoMember(2, IsRequired = true)]
           public uint HistoryPoints { get; set; }
       }
       public static unsafe ServerSockets.Packet CreateGoldLeaguePoint(this ServerSockets.Packet stream, GoldLeaguePoint obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgGoldLeaguePoint);

           return stream;
       }
       public static unsafe void GetGoldLeaguePoint(this ServerSockets.Packet stream, out GoldLeaguePoint pQuery)
       {
           pQuery = new GoldLeaguePoint();
           pQuery = stream.ProtoBufferDeserialize<GoldLeaguePoint>(pQuery);
       }
       [PacketAttribute(GamePackets.MsgGoldLeaguePoint)]
       private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           user.Send(stream.CreateGoldLeaguePoint(new GoldLeaguePoint()
           {
               Points = user.Player.ChampionPoints,
               HistoryPoints = user.Player.HistoryChampionPoints
           }));
       }
    }
}
