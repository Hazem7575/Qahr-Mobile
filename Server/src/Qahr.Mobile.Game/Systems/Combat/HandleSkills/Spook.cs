using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
   public class Spook
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                Attack.SpellID = (ushort)Role.Flags.SpellID.Spook;

                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                          , ClientSpell.Level, ClientSpell.UseSpellSoul);

                Role.IMapObj target;
                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                {
                    Role.Player attacked = target as Role.Player;

                    if (!attacked.ContainFlag(MsgUpdate.Flags.Ride))
                        return;

                    bool CanAttack = user.Equipment.SteedPlus > attacked.Owner.Equipment.SteedPlus;
                    if (user.Equipment.SteedPlus == attacked.Owner.Equipment.SteedPlus)
                        CanAttack = user.Equipment.SteedPlusPorgres > attacked.Owner.Equipment.SteedPlusPorgres;

                    if (CanAttack)
                    {
                        attacked.RemoveFlag(MsgUpdate.Flags.Ride);
                    
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = attacked.UID, Hit = 1 });

                        Attack.OpponentUID = attacked.UID;
                        Attack.X = attacked.X;
                        Attack.Y = attacked.Y;
                        user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                    }
                }


                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

               

            }
        }
    }
}
