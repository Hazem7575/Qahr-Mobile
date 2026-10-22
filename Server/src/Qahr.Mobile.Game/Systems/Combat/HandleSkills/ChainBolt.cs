using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class ChainBolt
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ChainBolt:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.RemoveFlag(MsgUpdate.Flags.XPList))
                            {
                                if (!user.Player.ContainFlag(MsgUpdate.Flags.ChaintBolt))
                                    user.Player.OpenXpSkill(MsgUpdate.Flags.ChaintBolt, 50);
                                else
                                {
                                    user.Player.RemoveFlag(MsgUpdate.Flags.ChaintBolt);
                                    user.Player.OpenXpSkill(MsgUpdate.Flags.ChaintBolt, 50);
                                }
                                return;
                            }
                            uint Experience = 0;

                        
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                MsgSpell.X = attacked.X;
                                MsgSpell.Y = attacked.Y;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    Experience+=   ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                 
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                 
                                }

                                foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    if (attobj.UID == Attack.OpponentUID)
                                        continue;
                                    attacked = attobj as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);;
                                        }
                                    }
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                MsgSpell.X = attacked.X;
                                MsgSpell.Y = attacked.Y;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                                foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if (attobj.UID == Attack.OpponentUID)
                                        continue;
                                    attacked = attobj as Role.Player;
                                    if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                          
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                MsgSpell.X = attacked.X;
                                MsgSpell.Y = attacked.Y;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                   
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                  
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                                foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    if (attobj.UID == Attack.OpponentUID)
                                        continue;
                                    attacked = attobj as Role.SobNpc;
                                    if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                           
                                            Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                           
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            if (MsgSpell.Targets.Count != 0)
                            {
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
