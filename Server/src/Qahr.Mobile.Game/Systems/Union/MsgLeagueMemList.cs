using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueMemList
    {
       public static unsafe ServerSockets.Packet LeagueMemListCreate(this ServerSockets.Packet stream, uint GuildID, uint LeaderID, uint Count, ushort PageCount, byte DwParam)
       {
           stream.InitWriter();

           stream.Write(GuildID);
           stream.Write(LeaderID);
           stream.Write(Count);
           stream.Write(PageCount);
           stream.Write(DwParam);
           return stream;
       }
       public static unsafe ServerSockets.Packet AddItemLeagueMemList(this ServerSockets.Packet stream, uint Exploits,
           uint Class, ushort Level, uint Mesh, bool Online, string Name, uint MilitaryRank,
           uint NobilityRank, uint UID, ushort BattlePower)
       {
           stream.Write(Exploits);
           stream.Write(Class);
           stream.Write(Level);
           stream.Write(Mesh);
           stream.Write((byte)(Online ? 1 : 0));
           stream.Write(Name, 32);
           stream.Write(MilitaryRank);
           stream.Write(NobilityRank);
           stream.Write(UID);
           stream.Write(BattlePower);
           return stream;
       }

       public static unsafe ServerSockets.Packet LeagueMemListFinalize(this ServerSockets.Packet stream)
       {
           stream.Finalize(GamePackets.MsgLeagueMemList);
           return stream;
       }

    }
}
