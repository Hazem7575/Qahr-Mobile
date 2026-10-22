using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class AddMana
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.Meditation:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
     , 0, Attack.X, Attack.Y, ClientSpell.ID
     , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (ClientSpell.Level == 2)
                                DBSpell.Damage = 1020;
                            uint Damage = 0;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, user.Status.MaxMana, (uint)user.Player.Mana);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage, MsgAttackPacket.AttackEffect.None));
                                user.Player.Mana += (ushort)Damage;
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)DBSpell.Damage, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                }
            }
        }
    }
}
