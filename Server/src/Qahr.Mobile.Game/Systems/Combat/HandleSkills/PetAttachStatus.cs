using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
   public class PetAttachStatus
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.Thunderbolt:
                       {
                           user.Send(stream.InteractionCreate(Attack));

                           if (user.Player.ContainFlag(MsgUpdate.Flags.RevengeTail))
                           {
                               user.Player.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                               break;
                           }
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);




                           foreach (var mob in user.Player.View.Roles(Role.MapObjectType.Monster))
                           {
                               var monster = mob as Game.MsgMonster.MonsterRole;
                               if (monster.IsFloor)
                               {
                                   if (!monster.ContainFlag(MsgUpdate.Flags.Thunderbolt))
                                       monster.AddFlag(MsgUpdate.Flags.Thunderbolt, (int)DBSpell.Duration, true);
                                   monster.Family.AttackSpeed = 500;
                                   MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(mob.UID, 0, MsgAttackPacket.AttackEffect.None));
                               }
                           }


                           

                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                           break;
                       }
               }
           }
       }
    }
}
