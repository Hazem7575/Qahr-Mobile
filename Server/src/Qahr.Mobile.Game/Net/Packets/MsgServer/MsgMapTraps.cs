using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgMapTraps
    {
       [PacketAttribute(GamePackets.MsgTransportor)]
       public static unsafe void MapTraps(Client.GameClient client, ServerSockets.Packet stream)
       {
           switch (client.Player.Map)
           {
               case 1036:
                   {
                       if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 247, 144) < 18)
                       {

                           client.Player.MessageBox("A~Thrilling~Spook~is~surprisingly~strong.~You`d~better~team~up~and~try~to~kill~it~with~a~few~others!", 
                               new Action<Client.GameClient>(p =>
                           {
                                  var map = Pool.ServerMaps[2090];
                                   uint dinamicid = p.Map.GenerateDynamicID();
                                   if (p.Team != null)
                                   {
                                       p.Team.TeleportTeam(2090, 32, 30, dinamicid, c => Role.Core.GetDistance(p.Player.X, p.Player.Y, c.Player.X, c.Player.Y) <= 18);
                                   }
                                   p.Teleport(32, 30, 2090, dinamicid);
                           
                          }), null, 0);
                       }
                       break;
                   }
           }
       }
    }
}
