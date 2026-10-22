using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class AddBlackSpot
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            bool Added = false;
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                uint Experience = 0;
                uint count = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            if (attacked.BlackSpot)
                                continue;
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            if (attacked.Alive)
                            {
                                count++;
                                attacked.BlackSpot = true;
                                attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)DBSpell.Duration);
                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                Added = true;
                            }
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                        {
                            if (count == 5)
                                break;
                            if (attacked.BlackSpot)
                                continue;
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            if (attacked.Alive)
                            {
                                count--;
                                attacked.BlackSpot = true;
                                attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)DBSpell.Duration);
                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                Added = true;
                            }
                        }
                    }
                }
                Updates.IncreaseExperience.Up(stream, user, Experience);
                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);
                if (Added)
                {
                    MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                    , 0, user.Player.X, user.Player.Y, 11130
                                    , 4, 0);
                    RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                    RemoveCloudDown.SetStream(stream);
                    RemoveCloudDown.JustMe(user);
                }
            }
        }
    }
}