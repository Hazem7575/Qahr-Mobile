using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class ScurvyBomb
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                uint Experience = 0;
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
                            byte UseStamina = 6;
                            attacked.AddFlag(MsgUpdate.Flags.ScurvyBomb, (int)DBSpell.Duration, true);
                            //time, dmg(remove Stamina,
                            attacked.UseStamina = UseStamina;
                            attacked.SendUpdate(stream, (MsgUpdate.Flags)20, UseStamina, 0, DBSpell.Level, MsgUpdate.DataType.Fatigue, false);
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
                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                        {
                            if (num < 1) break;
                            num--;
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
            }
        }
    }
}
