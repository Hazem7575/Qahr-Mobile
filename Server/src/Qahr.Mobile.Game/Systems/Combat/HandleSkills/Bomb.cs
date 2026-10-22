using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Bomb
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (Attack.SpellID == (ushort)Role.Flags.SpellID.Poison)
            {
                Poison.Execute(user, Attack, stream, DBSpells);
                return;
            }

            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                // [Audit M4] The AoE/bomb center (Attack.X/Attack.Y) is chosen by the client.
                // Reject it if it is farther from the CASTER than the spell's range (+2 tiles
                // of slack for latency). Without this the player can detonate a bomb anywhere
                // on the map and hit distant groups the caster is nowhere near.
                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y) > DBSpell.Range + 2)
                    return;

                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.SuanniRoar: // Done ...
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte MaxStamina = (byte)(user.Player.HeavenBlessing > 0 ? 150 : 100);
                            if (user.Player.Stamina < MaxStamina)
                            {
                                user.Player.Stamina += (ushort)DBSpell.DamageOnMonster;
                                user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        var Rate = DBSpell.DamageOnMonster;
                                        if (attacked.BattlePower > user.Player.BattlePower)
                                        {

                                            int Bp = attacked.BattlePower - user.Player.BattlePower;
                                            Rate = Math.Min(0, Bp * (int)(DBSpell.Damage3 / 100));
                                        }
                                        else if (user.Player.BattlePower > attacked.BattlePower)
                                        {
                                            int Bp = user.Player.BattlePower - attacked.BattlePower;
                                            Rate = Math.Min(100, Bp * (int)(DBSpell.Damage3 / 100));
                                        }
                                        if (Calculate.Base.Rate(Rate))
                                        {
                                            attacked.AddSpellFlag((MsgUpdate.Flags)DBSpell.Status, (int)DBSpell.Duration, true);
                                            attacked.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
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
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    #region ArchiveNinja

                    case (ushort)Role.Flags.SpellID.WaterPrison:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        attacked.Owner.Player.FutilityDg = (uint)DBSpell.DamageOnHuman;
                                        #region PrisonSigilFatigue
                                        Role.Instance.Ninja.Item item;

                                        if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.PrisonSigilFatigue, out item))
                                        {
                                            byte level = 0;
                                            int Rate = 0;
                                            if (attacked.Owner.Rune.IsEquipped("Consolidation", ref level))
                                            {
                                                switch (level)
                                                {
                                                    case 1: Rate = 30; break;
                                                    case 2: Rate = 35; break;
                                                    case 3: Rate = 40; break;
                                                    case 4: Rate = 45; break;
                                                    case 5: Rate = 55; break;
                                                    case 6: Rate = 60; break;
                                                    case 7: Rate = 65; break;
                                                    case 8: Rate = 70; break;
                                                    case 9: Rate = 80; break;
                                                }
                                                if (!Role.Core.Rate(Rate))
                                                {
                                                    if (attacked.Stamina > item.DBItem.Power)
                                                        attacked.Stamina -= (ushort)item.DBItem.Power;
                                                    else
                                                        attacked.Stamina = 0;
                                                    attacked.SendUpdate(stream, (long)attacked.Stamina, MsgUpdate.DataType.Stamina);
                                                }
                                            }


                                        }
                                        #endregion
                                        #region PrisonSigilFutility
                                        if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.PrisonSigilFutility, out item))
                                        {
                                            attacked.Owner.Player.FutilityDg += (uint)item.DBItem.Power;
                                        }
                                        #endregion
                                        if (attacked.AddFlag(MsgUpdate.Flags.PrisonSigilFutility, (int)DBSpell.Duration, true))
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.PrisonSigilFutility, DBSpell.Duration, (uint)attacked.Owner.Player.FutilityDg, (uint)attacked.Owner.Player.FutilityDg, MsgUpdate.DataType.ArchiveSkill, true);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);


                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WaterPrisonPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
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
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);


                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion
                    case (ushort)Role.Flags.SpellID.ThunderStrike:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official monster damage = MagicType power (e.g. ThunderStrike 800%), now uncapped in calc): AnimationObj.Damage = AnimationObj.Damage * 400 / 100;
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        //if (attacked.Alive)
                                        //    attacked.AddFlag(MsgUpdate.Flags.Dizzy, 3, true);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 120 / 100; // Mohe Done (Attack)
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        if (attacked.Alive)
                                            if (attacked.Alive && DBSpell.Level >= 8)
                                            {
                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 2, true);
                                            }
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 500 / 100;
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
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
                    case (ushort)Role.Flags.SpellID.SacredBlessing:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official monster damage = MagicType power (e.g. ThunderStrike 800%), now uncapped in calc): AnimationObj.Damage = AnimationObj.Damage * 400 / 100;
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 80 / 100;

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        if (attacked.Alive)
                                        {

                                            attacked.BleedDamage = 3000;
                                            attacked.AddSpellFlag(MsgUpdate.Flags.Bleed, 5, true, 1);
                                        }

                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 500 / 100;

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.SacredBlessing, (int)DBSpell.Duration, false);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DeadlyStrike:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official monster damage = MagicType power (e.g. ThunderStrike 800%), now uncapped in calc): AnimationObj.Damage = AnimationObj.Damage * 400 / 100;
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        if (attacked.Alive)
                                        {
                                            attacked.AddFlag(MsgUpdate.Flags.DeadlyStrike, (int)DBSpell.Duration, true);
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.DeadlyStrike, (uint)DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                            attacked.DeadlyStrikePower = (uint)DBSpell.Damage3;
                                        }
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 500 / 100;

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
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
                    case (ushort)Role.Flags.SpellID.AngerofStomper:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 3)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 1)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 3)
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
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WindstormBattleaxe:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4 || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // ÊÚÏíá ÇáÏãÌ
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
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
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.AttackUp, 30, true);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FireofHell:
                    case (ushort)Role.Flags.SpellID.FireCircle:
                        {
                            MsgSpell fireCurse;
                            Database.MagicType.Magic fireCurseDB = null;
                            uint firecurseDamage = 0;
                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FireCurse, out fireCurse))
                            {
                                fireCurseDB = Pool.Magic[(ushort)Role.Flags.SpellID.FireCurse][user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.FireCurse].Level];
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FireofHell && firecurseDamage > 0)
                                        {
                                            using (var recFc = new ServerSockets.RecycledPacket())
                                            {
                                                var streamm = recFc.GetStream();
                                                MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , attacked.UID, attacked.X, attacked.Y, fireCurse.ID
                                 , fireCurse.Level, fireCurse.UseSpellSoul);
                                                firecurseDamage += (uint)(fireCurseDB.Damage2);
                                                if (firecurseDamage > 10000) firecurseDamage = 10000;
                                                FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = firecurseDamage, Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = attacked.UID });
                                                ReceiveAttack.Monster.Execute(streamm, FireCurseMsgSpell.Targets.FirstOrDefault(), user, attacked);
                                                FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(user);
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 60 / 100; // Mohe Done (Attack)
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        Sering.Proces(user, attacked, stream);

                                        if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FireofHell && firecurseDamage > 0)
                                        {
                                            using (var recFc = new ServerSockets.RecycledPacket())
                                            {
                                                var streamm = recFc.GetStream();
                                                MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , attacked.UID, attacked.X, attacked.Y, fireCurse.ID
                                 , fireCurse.Level, fireCurse.UseSpellSoul);
                                                firecurseDamage += (uint)(DBSpell.Damage2);
                                                if (firecurseDamage > 10000) firecurseDamage = 10000;
                                                FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = firecurseDamage, Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = attacked.UID });
                                                ReceiveAttack.Player.Execute(FireCurseMsgSpell.Targets.FirstOrDefault(), user, attacked);
                                                FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(user);
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FireofHell && firecurseDamage > 0)
                                        {
                                            using (var recFc = new ServerSockets.RecycledPacket())
                                            {
                                                var streamm = recFc.GetStream();
                                                MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , attacked.UID, attacked.X, attacked.Y, fireCurse.ID
                                 , fireCurse.Level, fireCurse.UseSpellSoul);
                                                firecurseDamage += (uint)(DBSpell.Damage2);
                                                if (firecurseDamage > 10000) firecurseDamage = 10000;
                                                FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = firecurseDamage, Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = attacked.UID });
                                                ReceiveAttack.Npc.Execute(stream, FireCurseMsgSpell.Targets.FirstOrDefault(), user, attacked);
                                                FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(user);
                                            }
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Lightning:
                    case (ushort)Role.Flags.SpellID.Vulcano:
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                                return;
                            if (!AtributesStatus.IsTaoist(user.Player.Class))
                                return;
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 18)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 18)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 60 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 18)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

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
                    case (ushort)Role.Flags.SpellID.StarFlow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 30 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }

                    case (ushort)Role.Flags.SpellID.ArrowRain:
                        {
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 30 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
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
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Hercules:
                        {
                                user.Player.AttackStamp = DateTime.Now.AddMilliseconds(750);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                uint Experience = 0;

                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 70 / 100; // ÊÚÏíá ÇáÏãÌ
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }

                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                        
                    case (ushort)Role.Flags.SpellID.RadiantPalm:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 150 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                    case (ushort)Role.Flags.SpellID.Rage:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 50 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                    case (ushort)Role.Flags.SpellID.SpeedLightning:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 20 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                    case (ushort)Role.Flags.SpellID.Celestial:
                        {
                            #region Skill XP Wind Walker
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)) // Skill XP Wind Walker
                                user.Player.RemoveFlag(MsgUpdate.Flags.Omnipotence); // Skill XP Wind Walker
                            #endregion
                            #region Skill XP Thunder Striker
                            if (user.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)) // Skill XP Thunder Striker
                                user.Player.RemoveFlag(MsgUpdate.Flags.ThunderRampage); // Skill XP Thunder Striker
                            #endregion
                            #region Skill XP Warrior
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Superman)) // Skill XP Warrior
                                user.Player.RemoveFlag(MsgUpdate.Flags.Superman); // Skill XP Warrior
                            if (user.Player.ContainFlag(MsgUpdate.Flags.ManiacDance)) // Skill XP Warrior
                                user.Player.RemoveFlag(MsgUpdate.Flags.ManiacDance); // Skill XP Warrior
                            #endregion
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);


                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 85 / 100;
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 300 / 100;
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.CelestialDance, (uint)DBSpell.Duration, 0, 0, MsgUpdate.DataType.CelestialDance);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.CelestialDance, (int)DBSpell.Duration, true);
                            user.Player.CelestialDancePower = (uint)(DBSpell.DamageOnHuman % 1000);
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BloodyScythe:
                        {
                            if (DateTime.Now > user.Player.StampBloodyScytle)
                            {
                                user.Player.StampBloodyScytle = DateTime.Now.AddMilliseconds(500);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                uint Experience = 0;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
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
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 6)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 30 / 100; // Mohe Done (Attack)
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }

                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
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
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }

                    default:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
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
