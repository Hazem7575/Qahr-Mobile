using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DispatchXp
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                if (ClientSpell.ID == (ushort)Role.Flags.SpellID.DragonRoar)
                {
                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                        , 0, Attack.X, Attack.Y, ClientSpell.ID
                        , ClientSpell.Level, ClientSpell.UseSpellSoul);

                    bool IncreaseExp = false;

                    if (user.Team != null)
                    {
                        IncreaseExp = true;

                        foreach (var target in user.Team.GetMembers())
                        {
                            if (target.Player.UID == user.Player.UID)
                                continue;
                            if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) <= DBSpell.MaxTargets)
                            {
                                int MaxStamina = (byte)(target.Player.HeavenBlessing > 0 ? 150 : 100);
                                target.Player.Stamina = (byte)Math.Min(MaxStamina, (int)(target.Player.Stamina + (int)(DBSpell.Damage)));
                                target.Player.SendUpdate(stream,target.Player.Stamina, MsgUpdate.DataType.Stamina);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                    }

                    MsgSpell.SetStream(stream); 
                    MsgSpell.Send(user);

                    if (IncreaseExp)
                        Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 200, DBSpells);
                  
                    return;
                }
                else
                {
                    if (!user.Player.RemoveFlag(MsgUpdate.Flags.XPList))
                        return;
                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                    bool IncreaseExp = false;
                    if (user.Player.UID == Attack.OpponentUID)
                    {
                        IncreaseExp = true;
                        user.Player.XPCount += 20;
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                    }
                    else
                    {
                        Role.IMapObj target;
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                        {
                            IncreaseExp = true;
                            Role.Player attacked = target as Role.Player;
                            attacked.XPCount += 20;
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                        }
                    }

                    if (user.Team != null)
                    {
                        IncreaseExp = true;
                        foreach (var target in user.Team.GetMembers())
                        {
                            if (target.Player.UID == user.Player.UID)
                                continue;
                            if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                            {
                                target.Player.XPCount += 20;
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                    }

                    MsgSpell.SetStream(stream);
                    MsgSpell.Send(user);

                    if (IncreaseExp)
                        Updates.UpdateSpell.CheckUpdate(stream,user, Attack, 200, DBSpells);
                }
            }
        }
    }
}
