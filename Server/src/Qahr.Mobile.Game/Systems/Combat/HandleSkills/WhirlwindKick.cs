using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class WhirlwindKick
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , (uint)Calculate.Base.MyRandom.Next(DBSpell.MaxTargets / 2, DBSpell.MaxTargets * 2), Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                uint Experience = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 4)
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
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 4)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            // [CombatAudit 2026-10-14] removed custom nerf (official tooltip 80-110% = power): AnimationObj.Damage = AnimationObj.Damage * 60 / 100;
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                            byte NatureShieldL = 0;
                            if (attacked.Owner.Rune.IsEquipped("NatureShield", ref NatureShieldL) || attacked.Owner.Rune.IsEquipped("Indestructible Balance", ref NatureShieldL))
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
                                    AnimationObj.Effect = MsgAttackPacket.AttackEffect.NatureShield;
                                    AnimationObj.Hit = 0;
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    continue;
                                }
                            }
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            user.Player.SpellAttackStamp = DateTime.Now.AddSeconds(2);
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                {
                    var attacked = targer as Role.SobNpc;
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 4)
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
        }
    }
}