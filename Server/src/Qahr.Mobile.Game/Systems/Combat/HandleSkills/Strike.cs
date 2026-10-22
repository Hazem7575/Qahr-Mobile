using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Strike
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.Circle:
                        {
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                     , 0, Attack.X, Attack.Y, ClientSpell.ID
                                     , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {
                                            unsafe
                                            {
                                                InteractQuery _attack = new InteractQuery();
                                                _attack.Damage = (int)AnimationObj.Damage;
                                                _attack.X = attacked.X;
                                                _attack.Y = attacked.Y;
                                                _attack.OpponentUID = attacked.UID;
                                                _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                _attack.UID = user.Player.UID;
                                                _attack.Effect = (uint)AnimationObj.Effect;
                                                user.Send(stream.InteractionCreate(_attack));
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {
                                            unsafe
                                            {
                                                InteractQuery _attack = new InteractQuery();
                                                _attack.Damage = (int)AnimationObj.Damage;
                                                _attack.X = attacked.X;
                                                _attack.Y = attacked.Y;
                                                _attack.OpponentUID = attacked.UID;
                                                _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                _attack.UID = user.Player.UID;
                                                _attack.Effect = (uint)AnimationObj.Effect;
                                                user.Send(stream.InteractionCreate(_attack));
                                            }
                                        }
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {
                                            unsafe
                                            {
                                                InteractQuery _attack = new InteractQuery();
                                                _attack.Damage = (int)AnimationObj.Damage;
                                                _attack.X = attacked.X;
                                                _attack.Y = attacked.Y;
                                                _attack.OpponentUID = attacked.UID;
                                                _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                _attack.UID = user.Player.UID;
                                                _attack.Effect = (uint)AnimationObj.Effect;
                                                user.Send(stream.InteractionCreate(_attack));
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user, user.OnAutoAttack ? true : false);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Strike:
                        {


                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                }
            }
        }
    }
}
