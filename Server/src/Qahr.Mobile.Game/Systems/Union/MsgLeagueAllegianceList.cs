using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueAllegianceList
    {
       public static unsafe ServerSockets.Packet LeagueAllegianceListCreate(this ServerSockets.Packet stream, uint Count, ushort Page, ushort PageCount, byte DwParam)
        {
            stream.InitWriter();
            stream.Write(Count);//4
            stream.Write(Page);//8
            stream.Write(PageCount);//10
            stream.Write(DwParam);//12
            return stream;
        }
     
       public static unsafe ServerSockets.Packet AddItemLeagueAllegianceList(this ServerSockets.Packet stream, ulong Fund,
          uint UID, uint GoldBricks, string Name, string LeaderName, string RecruitDeclaration, byte IsKingdom)
        {

            stream.Write((ulong)Fund);//13
            stream.Write(UID);//21
            stream.Write(GoldBricks);//25
            stream.Write(Name,32);//29
            stream.Write(LeaderName,32);//61
            stream.Write(RecruitDeclaration, 512);//93
            return stream;//605 (entry = 592 bytes)
        }
       public static unsafe ServerSockets.Packet LeagueAllegianceListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgLeagueAllegianceList);
            return stream;
        }
    }
}
