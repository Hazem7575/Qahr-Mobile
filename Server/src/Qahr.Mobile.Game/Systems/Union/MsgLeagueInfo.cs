using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueInfo
    {
       public static unsafe ServerSockets.Packet LeagueInfoCreate(this ServerSockets.Packet stream, ulong Treasury, uint GoldBricks
           , uint Stipend, string Name, string LeaderName, string Bulletin, string Title, string PlunderTarget, string InvadingUnion
           )
       {
           stream.InitWriter();
           stream.Write(Treasury);
           stream.Write(GoldBricks);
           stream.Write(Stipend);
           stream.Write(Database.GroupServerList.MyServerInfo.ID);//serverid 1
           stream.Write(Database.GroupServerList.MyServerInfo.ID);//serverid 2
           stream.Write(Name, 32);
           stream.Write(LeaderName, 32);
           stream.Write(Bulletin, 512);
           stream.Write(Title, 20);
           stream.Write(PlunderTarget, 32);
           stream.Write(InvadingUnion, 32);

           stream.Finalize(GamePackets.MsgLeagueInfo);
           return stream;
       }
    }
}
