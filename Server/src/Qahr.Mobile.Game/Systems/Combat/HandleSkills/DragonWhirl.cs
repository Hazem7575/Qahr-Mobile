using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DragonWhirl
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.DragonWhirl:
                        {
                            if (!CheckAttack.CheckFloors.CheckGuildWar(user, Attack.X, Attack.Y))
                            {
                                break;
                            }

                            Attack.AtkType = (ushort)MsgAttackPacket.AttackID.InMoveSpell;
                            Attack.KilledMonster = (ushort)Role.Flags.SpellID.DragonWhirl;

                            byte Distance = (byte)Calculate.Base.GetDistance(Attack.X, Attack.Y, user.Player.X, user.Player.Y);
                            if (user.Player.Map == 1038)
                            {
                                if (Distance >= DBSpell.Range)
                                    break;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Distance, 0);

                            uint Experience = 0;
                            byte LineRange = 4;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
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
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
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
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
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
                            }
                            Attack.SpellID = 0;
                            Attack.Damage = 0;
                            user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            ushort X = user.Player.X;
                            ushort Y = user.Player.Y;
                            Line.GetNewCoords(ref X, ref Y);

                            Attack.X = X;
                            Attack.Y = Y;


                            user.Map.View.MoveTo<Role.IMapObj>(user.Player, X, Y);
                            user.Player.X = X;
                            user.Player.Y = Y;

                            user.Player.View.Role(false, stream.InteractionCreate(Attack));
                            break;
                        }
                }
            }
        }
    }
}
