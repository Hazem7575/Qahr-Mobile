using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
   public class CheckLineSpells
    {
       public static bool CheckUp(Client.GameClient user, ushort spellid)
       {
           if (Program.FBMap.Contains(user.Player.Map))
           {
               if (spellid != 1045 && spellid != 1046 && spellid != 12350 && spellid != 12930)
               {
                   user.SendSysMesage("You have to use manual linear skills (FastBlade/ScentSword/RageOfWar/DragonSlash) in [FB/SS] Room");
                   return false;
               }
           }
           return true;
       }
    }
}