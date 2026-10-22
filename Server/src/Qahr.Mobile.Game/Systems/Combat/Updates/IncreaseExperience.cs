using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Updates
{
   public class IncreaseExperience
    {
       public unsafe static void Up(ServerSockets.Packet stream, Client.GameClient user, uint Damage)
       {
           if (Damage == 0)
               return;
           if (user.Player.ContainFlag(MsgUpdate.Flags.Oblivion))
           {
               user.ExpOblivion += Damage * 4;
           }
           else
               user.IncreaseExperience(stream,Damage);

           if (user.Player.HeavenBlessing > 0)
           {
               user.Player.HuntingBlessing += Damage / 10;
           }
       }
       

    }
}
