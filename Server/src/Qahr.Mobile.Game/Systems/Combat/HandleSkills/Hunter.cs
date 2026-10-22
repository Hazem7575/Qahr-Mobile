using Qahr.Mobile.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Hunter
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.DragonHeart:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            byte MaxStamina = (byte)(user.Player.HeavenBlessing > 0 ? 150 : 100);
                            if (user.Player.Stamina < MaxStamina)
                            {
                                user.Player.Stamina += 10;
                                user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Inchstrength:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , user.Player.UID, 0, 0, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BeastShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.BestShield));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Hunter:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , user.Player.UID, 0, 0, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.Hunter, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.Hunter, DBSpell.Duration, (uint)DBSpell.Damage2, 0, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ArchShadow:
                        {
                           
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        if (!attacked.ContainFlag(MsgUpdate.Flags.ArchShadow))
                                        {
                                            attacked.AddSpellFlag(MsgUpdate.Flags.ArchShadow, (int)DBSpell.Duration, true);
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.ArchShadow, DBSpell.Duration, (uint)DBSpell.Damage, 0, MsgUpdate.DataType.ArchiveSkill);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                }
            }
            
        }
    }
}
