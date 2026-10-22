using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DirectAttack
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                uint Experience = 0;
                Role.IMapObj target;
                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                           
                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                            MsgSpell.Targets.Enqueue(AnimationObj);

                            foreach (Role.IMapObj targ in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                var attacked2 = targ as MsgMonster.MonsterRole;
                                if (attacked != attacked2 && Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked2.X, attacked2.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked2.X, attacked2.Y, 0))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked2, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj2;
                                            Calculate.Physical.OnMonster(user.Player, attacked2, DBSpell, out AnimationObj2);
                                            AnimationObj2.Damage = Calculate.Base.CalculateSoul(AnimationObj2.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj2, user, attacked2);
                                            MsgSpell.Targets.Enqueue(AnimationObj2);
                                        }
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                        }
                    }
                }
                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                {
                    var attacked = target as Role.Player;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= DBSpell.Range)
                    {

                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                           
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);

                            foreach (Role.IMapObj targ in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                Role.Player attacked2 = targ as Role.Player;
                                if (attacked != attacked2 && Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked2.X, attacked2.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked2.X, attacked2.Y, 0))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked2, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj2;
                                            Calculate.Physical.OnPlayer(user.Player, attacked2, DBSpell, out AnimationObj2);
                                            AnimationObj2.Damage = Calculate.Base.CalculateSoul(AnimationObj2.Damage, ClientSpell.UseSpellSoul);
                                            
                                            ReceiveAttack.Player.Execute(AnimationObj2, user, attacked2);
                                            MsgSpell.Targets.Enqueue(AnimationObj2);

                                        }
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                        }
                    }

                }
                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                {
                    var attacked = target as Role.SobNpc;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= DBSpell.Range)
                    {

                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                           
                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);



                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                        }
                    }

                }
                Updates.IncreaseExperience.Up(stream, user, Experience);
                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
            }
        }
    }
}
