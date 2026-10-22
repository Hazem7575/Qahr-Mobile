using Qahr.Mobile.Role;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class HellVortex
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                Attack.X = user.Player.X;
                Attack.Y = user.Player.Y;
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul, 1);
                uint Experience = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 2)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
                            , attacked.X, attacked.Y, 2, user.Map);
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                            if (attacked.Family.Settings == MsgMonster.MonsterSettings.Guard)
                            {
                                AnimationObj.MoveX = (uint)attacked.X;
                                AnimationObj.MoveY = (uint)attacked.Y;
                            }
                            else if (attacked.Boss == 0)
                            {
                                user.Map.View.MoveTo<Role.IMapObj>(attacked, coord.X, coord.Y);
                                attacked.X = (ushort)coord.X;
                                attacked.Y = (ushort)coord.Y;
                                AnimationObj.MoveX = (uint)coord.X;
                                AnimationObj.MoveY = (uint)coord.Y;
                                attacked.UpdateMonsterView(user.Player.View, stream);
                            }
                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 2)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                        {
                            Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
                       , attacked.X, attacked.Y, 2, user.Map);
                            if (coord.X == 0) break;
                            if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord.X, coord.Y))
                                continue;
                            user.Map.View.MoveTo<Role.IMapObj>(attacked, coord.X, coord.Y);
                            attacked.X = (ushort)coord.X;
                            attacked.Y = (ushort)coord.Y;
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            AnimationObj.MoveX = (uint)coord.X;
                            AnimationObj.MoveY = (uint)coord.Y;
                            attacked.View.Role(false, null);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            attacked.AttackStamp = DateTime.Now.AddSeconds(3);
                            attacked.Protect = DateTime.Now.AddSeconds(3);
                        }
                    }
                }
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);
            }
        }
    }
}