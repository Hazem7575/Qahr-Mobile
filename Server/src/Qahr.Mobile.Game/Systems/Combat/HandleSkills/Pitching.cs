using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Pitching
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.Pitching:
                        {
                           
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , Attack.OpponentUID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                MsgSpell.X = attacked.X;
                                MsgSpell.Y = attacked.Y;

                                int Rate = DBSpell.Rate;

                                if (attacked.BattlePower > user.Player.BattlePower)
                                {

                                    int Bp = attacked.BattlePower - user.Player.BattlePower;
                                    Rate = Math.Max(0, (Rate - Bp));
                                }
                                if (Role.Core.Rate(Rate))
                                {
                                    if (attacked.OnXPSkill() != MsgUpdate.Flags.Normal)
                                        attacked.RemoveFlag(attacked.OnXPSkill());
                                    attacked.RemoveFlag(MsgUpdate.Flags.XPList);
                                    attacked.XPCount = 0;
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(Attack.OpponentUID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(Attack.OpponentUID, 0, MsgAttackPacket.AttackEffect.None) { Hit = 0 });

                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                }
            }
        }
    }
}
