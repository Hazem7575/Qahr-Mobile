using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
  public class SectorPasive
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                 
                    case (ushort)Role.Flags.SpellID.ScarofEarth:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul,0);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 5, 0);

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Line.InLine(attacked.X, attacked.Y, 1, true))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (attacked.Alive)
                                        {
                                            attacked.AddFlag(MsgUpdate.Flags.ScarofEarth, 12, true, 2);
                                            attacked.ScarofEarthl = DBSpell;
                                            attacked.AttackerScarofEarthl = user;
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Line.InLine(attacked.X, attacked.Y, 1, true))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        AnimationObj.Hit = 0;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                       if (Role.Core.Rate(100 - 5 * (attacked.BattlePower - user.Player.BattlePower)))
                                        {
                                            if (attacked.Alive)
                                            {
                                                attacked.AddFlag(MsgUpdate.Flags.ScarofEarth, 12, true, 2);
                                                attacked.ScarofEarthl = DBSpell;
                                                attacked.AttackerScarofEarthl = user;
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Line.InLine(attacked.X, attacked.Y, 1, true))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                       
                                       
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user, true);
                            break;
                        }
                }
            }
        }
    }
}
