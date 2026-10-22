using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Sector
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ScatterFire:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);

                            uint Experience = 0;
                            Role.IMapObj lastAttacked = null;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;

                                if (SpellSector.Inside(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // ScatterFire - 10,000 Damage
                                        // Only for the specified monsters
                                        if (attacked.Family.ID == 4220 ||
                                            attacked.Family.ID == 4171 ||
                                            attacked.Family.ID == 4212 ||
                                            attacked.Family.ID == 3976 ||
                                            attacked.Family.ID == 3977 ||
                                            attacked.Family.ID == 3971 ||
                                            attacked.Family.ID == 3970 ||
                                            attacked.Family.ID == 3978)
                                        {
                                            AnimationObj.Damage = 10000;
                                        }
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        lastAttacked = attacked;
                                    }
                                }
                            }


                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (SpellSector.Inside(attacked.X, attacked.Y, user.nSaveMele))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        AnimationObj.Hit = 0;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        lastAttacked = attacked;
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (SpellSector.Inside(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        lastAttacked = attacked;
                                    }
                                }

                            }

                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.MyArchives.StarFlow(user, user.Player.UID, Attack.OpponentUID);
                            MsgSpell clientspell;
                            if (lastAttacked != null && user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.StarRaid, out clientspell))
                            {
                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.StarRaid, out Spells))
                                {
                                    Database.MagicType.Magic spell;
                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                    {
                                        if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                        {
                                            Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                            Attack.X = lastAttacked.X;
                                            Attack.Y = lastAttacked.Y;
                                            AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                        }

                                    }
                                }
                            }

                            break;
                        }
                    default:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (SpellSector.Inside(attacked.X, attacked.Y)
                                    && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5)
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
                                if (SpellSector.Inside(attacked.X, attacked.Y) && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5 || user.nSaveMele)
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
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (SpellSector.Inside(attacked.X, attacked.Y) && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5)
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
                            break;
                        }
                }
            }
        }

    }
}
