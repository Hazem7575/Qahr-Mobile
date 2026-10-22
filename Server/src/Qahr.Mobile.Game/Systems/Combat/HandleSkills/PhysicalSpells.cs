using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class PhysicalSpells
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {

                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);
                uint Experience = 0;
                Role.IMapObj target;
                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 3)
                    {


                        for (byte x = 0; x < 3; x++)
                        {
                            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                            {
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                AnimationObj.Damage = AnimationObj.Damage * 3;

                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);

                            }
                        }
                    }
                }
                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                {
                    var attacked = target as Role.Player;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 1)
                    {


                        for (byte x = 0; x < 3; x++)
                        {
                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                            {
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                            }
                        }
                    }

                }
                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                {
                    var attacked = target as Role.SobNpc;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 3)
                    {


                        for (byte x = 0; x < 3; x++)
                        {
                            if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                            {
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                AnimationObj.Damage = AnimationObj.Damage * 3;

                                Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                            }
                        }
                    }

                }
                Updates.IncreaseExperience.Up(stream, user, Experience);
                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

                if (ClientSpell.ID == (ushort)Role.Flags.SpellID.Windstorm && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloomofDeath))
                {
                    var attacked = target as Role.Player;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 1)
                    {
                        Dictionary<ushort, Database.MagicType.Magic> Spells;
                        if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.BloomofDeath, out Spells))
                        {
                            Attack.SpellID = (ushort)Role.Flags.SpellID.BloomofDeath;
                            Database.MagicType.Magic spell;
                            if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                            {
                                AttackHandler.LayTrap.Execute(user, Attack, stream, Spells);
                            }
                        }
                    }
                }

            }
        }
    }
}