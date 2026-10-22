using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
  public  class ManiacDance
    {
      public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
      {
          Database.MagicType.Magic DBSpell;
          MsgSpell ClientSpell;
          if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
          {
              switch (ClientSpell.ID)
              {
                  case (ushort)Role.Flags.SpellID.ManiacDance:
                      {
                          if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                          {
                              user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                              break;
                          }
                          if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                              break;
                          MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                          , ClientSpell.Level, ClientSpell.UseSpellSoul);

                          user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                          user.Player.OpenXpSkill(MsgUpdate.Flags.ManiacDance, 15, 1);

                          MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                          MsgSpell.SetStream(stream);
                          MsgSpell.Send(user);
                          user.Player.ManiacDanceStamp= DateTime.Now.AddMilliseconds(user.Equipment.AttackSpeed(false));
                          break;
                      }
              }
          }
      }
    }
}
