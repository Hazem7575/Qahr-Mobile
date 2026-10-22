using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueImperialCourtList
    {
       public enum ActionID : byte
       {
           CoreOfficials = 0,
           Officials = 1

       }
       public static unsafe ServerSockets.Packet LeagueImperialCourtListCreate(this ServerSockets.Packet stream, ActionID dwparam, byte count)
       {
           stream.InitWriter();
           stream.Write((byte)dwparam);
           stream.Write(count);
           return stream;
       }
       public static unsafe ServerSockets.Packet AddItemLeagueImperialCourtList(this ServerSockets.Packet stream, uint BattlePower
           , uint Mesh, uint Exploits, uint NobilityRank, uint UID, ushort Level, uint Class, ushort UnionRank, byte Online, string Name)
       {
           stream.Write(BattlePower);
           stream.Write(Mesh);
           stream.Write(Exploits);
           stream.Write(NobilityRank);
           stream.Write(UID);
           stream.Write(Level);
           stream.Write(Class);
           stream.Write(UnionRank);
           stream.Write(Online);
           stream.Write(Name, 32);
           return stream;
       }
       public static unsafe ServerSockets.Packet LeagueImperialCourtListFinalize(this ServerSockets.Packet stream)
       {
           stream.Finalize(GamePackets.MsgLeagueImperialCourtList);
           return stream;
       }
    }
}
