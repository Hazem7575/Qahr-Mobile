using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class BombLine
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.GaleBomb:
                        {
                            if (user.Player.Map == 10250)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Deity Land .");
                                return;
                            }
                            if (user.Player.Map == 10137)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Dragon Island .");
                                return;
                            }
                            if (user.Player.Map == 1038)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Guild War .");
                                return;
                            }
                            if (user.Player.Map == 12879 || user.Player.Map == 12099 || user.Player.Map == 13140 || user.Player.Map == 11996 || user.Player.Map == 1138 || user.Player.Map == 10504 || user.Player.Map == 6521 || user.Player.Map == 6521 || user.Player.Map == 2057 || user.Player.Map == 2071)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In This Map .");
                                return;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , 0, Attack.X, Attack.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            int num = 0;

                            switch (ClientSpell.Level)
                            {
                                case 0:
                                case 1:
                                    num = 3;
                                    break;
                                case 2:
                                case 3:
                                    num = 4;
                                    break;
                                default:
                                    num = 5;
                                    break;
                            }
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.Hit = 1;
                                        AnimationObj.MoveX = target.X;
                                        AnimationObj.MoveY = target.Y;
                                        Pool.ServerMaps[attacked.Map].Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

                                        user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                        attacked.X = (ushort)AnimationObj.MoveX;
                                        attacked.Y = (ushort)AnimationObj.MoveY;

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);


                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);


                                        AnimationObj.Hit = 1;
                                        AnimationObj.MoveX = targer.X;
                                        AnimationObj.MoveY = targer.Y;
                                        byte NatureShieldL = 0;
                                        if (Role.Core.Rate(50))
                                        {
                                            if (attacked.Owner.Rune.IsEquipped("NatureShield", ref NatureShieldL) || attacked.Owner.Rune.IsEquipped("Indestructible Balance", ref NatureShieldL))
                                            {
                                                byte Rate = 0;
                                                switch (NatureShieldL)
                                                {
                                                    case 1: Rate = 5; break;
                                                    case 2: Rate = 10; break;
                                                    case 3: Rate = 15; break;
                                                    case 4: Rate = 20; break;
                                                    case 5: Rate = 30; break;
                                                    case 6: Rate = 40; break;
                                                    case 7: Rate = 50; break;
                                                    case 8: Rate = 60; break;
                                                    case 9: Rate = 70; break;
                                                }
                                                if (Calculate.Base.Rate(Rate))
                                                {
                                                    AnimationObj.Hit = 0;
                                                    AnimationObj.Effect = MsgAttackPacket.AttackEffect.NatureShield;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                attacked.Owner.Map.Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

                                                if (!CheckAttack.CheckFloors.CheckGuildWar(attacked.Owner, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY))
                                                {
                                                    continue;
                                                }

                                                user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                                attacked.X = (ushort)AnimationObj.MoveX;
                                                attacked.Y = (ushort)AnimationObj.MoveY;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                attacked.View.Role(false, null);
                                                user.Player.AttackStamp = DateTime.Now.AddSeconds(2);

                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        if (user.Player.Map == 1038 || user.Player.Map == 1138)
                                        {
                                            return;
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.HitInMele = false;
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DivineAttraction:
                        {
                            if (user.Player.Map == 10250)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Divine Attraction In Deity Land .");
                                return;
                            }
                            if (user.Player.Map == 10137)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Divine Attraction In Dragon Island .");
                                return;
                            }
                            if (user.Player.Map == 1038)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Guild War .");
                                return;
                            }
                            if (user.Player.Map == 12879 || user.Player.Map == 12099 || user.Player.Map == 13140 || user.Player.Map == 11996 || user.Player.Map == 1138 || user.Player.Map == 10504 || user.Player.Map == 6521 || user.Player.Map == 6521 || user.Player.Map == 2057 || user.Player.Map == 2057 || user.Player.Map == 2071)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In This Map .");
                                return;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (attacked.Boss < 1)
                                        {
                                            AnimationObj.MoveX = Attack.X;
                                            AnimationObj.MoveY = Attack.Y;
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        byte NatureShieldL = 0;
                                        if (Role.Core.Rate(50))
                                        {
                                            if (attacked.Owner.Rune.IsEquipped("NatureShield", ref NatureShieldL)
                                                || attacked.Owner.Rune.IsEquipped("Indestructible Balance", ref NatureShieldL))
                                            {
                                                int Rate = 0;
                                                switch (NatureShieldL)
                                                {
                                                    case 1: Rate = 5; break;
                                                    case 2: Rate = 10; break;
                                                    case 3: Rate = 15; break;
                                                    case 4: Rate = 20; break;
                                                    case 5: Rate = 30; break;
                                                    case 6: Rate = 40; break;
                                                    case 7: Rate = 50; break;
                                                    case 8: Rate = 60; break;
                                                    case 9: Rate = 70; break;
                                                }
                                                if (Calculate.Base.Rate(Rate))
                                                {
                                                    AnimationObj.Hit = 0;
                                                    AnimationObj.Effect = MsgAttackPacket.AttackEffect.NatureShield;
                                                    AnimationObj.MoveX = attacked.X;
                                                    AnimationObj.MoveY = attacked.Y;
                                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                                }
                                                else
                                                {
                                                    var rate = 100;
                                                    if (attacked.BattlePower > user.Player.BattlePower)
                                                    {
                                                        var diff = attacked.BattlePower - user.Player.BattlePower;
                                                        rate -= 10 * diff;
                                                        rate = Math.Max(0, rate);
                                                    }
                                                    if (Role.Core.Rate(rate))
                                                    {
                                                        AnimationObj.MoveX = Attack.X;
                                                        AnimationObj.MoveY = Attack.Y;
                                                    }
                                                    else
                                                    {
                                                        AnimationObj.MoveX = attacked.X;
                                                        AnimationObj.MoveY = attacked.Y;
                                                    }
                                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                                }
                                            }
                                            else
                                            {
                                                var rate = 100;
                                                if (attacked.BattlePower > user.Player.BattlePower)
                                                {
                                                    var diff = attacked.BattlePower - user.Player.BattlePower;
                                                    rate -= 10 * diff;
                                                    rate = Math.Max(0, rate);
                                                }
                                                if (Role.Core.Rate(rate))
                                                {
                                                    AnimationObj.MoveX = Attack.X;
                                                    AnimationObj.MoveY = Attack.Y;
                                                }
                                                else
                                                {
                                                    AnimationObj.MoveX = attacked.X;
                                                    AnimationObj.MoveY = attacked.Y;
                                                }
                                                MsgSpell.Targets.Enqueue(AnimationObj);

                                            }

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.MoveX = Attack.X;
                                        AnimationObj.MoveY = Attack.Y;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
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
