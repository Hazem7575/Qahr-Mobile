using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class HeavensWrath
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.HeavensWrath:
                        {
                            if (!Database.AtributesStatus.IsThunderStriker(user.Player.Class)) break;
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            if (!user.Player.ContainFlag(MsgUpdate.Flags.HeavensWrath) && !user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, 0, 0, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None) { Hit = 0 });
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.HeavensWrath, (int)DBSpell.Duration, true);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);
                            }
                            else if (user.Player.ContainFlag(MsgUpdate.Flags.HeavensWrath))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, user.Player.X, user.Player.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul, 1);
                                uint Experience = 0;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 7)
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 7)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }

                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 7)
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                                Updates.IncreaseExperience.Up(stream, user, Experience);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            }
                            break;
                        }
                }
            }
        }
    }
}