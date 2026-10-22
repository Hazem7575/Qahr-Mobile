using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DragonCyclone
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.DragonCyclone:
                        {
                          
                            
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                            {

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                                user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                                user.Player.OpenXpSkill(MsgUpdate.Flags.DragonCyclone, (int)DBSpell.Duration);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            else if (user.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);

                                List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
                                    , Attack.Y, user.Map, Math.Min((byte)DBSpell.Range, (byte)Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y)));
                                if (coord == null || coord.Count == 0) return;

                                MsgSpell.X = (ushort)coord[coord.Count - 1].X;
                                MsgSpell.Y = (ushort)coord[coord.Count - 1].Y;

                                if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord[coord.Count - 1].X, coord[coord.Count - 1].Y))
                                {
                                    break;
                                }
                               
                                user.Shift(MsgSpell.X, MsgSpell.Y,stream, false);
                                uint Experience = 0;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                    {
                                        if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 2, coord))
                                            continue;

                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                          
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 30 / 100;
                                            Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                    {
                                        if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 2, coord))
                                            continue;
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
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                    {
                                        if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 2, coord))
                                            continue;
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                       
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            
                                           
                                            Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                                Updates.IncreaseExperience.Up(stream,user, Experience);
                                Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.SendRole(user);
                                MsgSpell.Send(user);
                              
                            }
                            break;
                        }
                }
            }
        }
    }
}
