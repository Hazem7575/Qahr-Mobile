using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Attack
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (user.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison)
            {
                if (Calculate.Base.Rate(20) || Attack.SpellID == (ushort)Role.Flags.SpellID.Poison)
                {
                    Poison.Execute(user, Attack, stream, DBSpells);
                    return;
                }
            }
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.AblazeBlade:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BladeTempest:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 90 / 100;
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        if (distance < 3)
                                        {
                                        }
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
                    case (ushort)Role.Flags.SpellID.ElementalArrow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                             , 0, 0, 0, ClientSpell.ID
                             , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Role.IMapObj target;
                            List<byte> CanUse = new List<byte>();
                            uint SpellEffect = 0;

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {

                                    user.MyArchives.Handel(attacked, true);
                                }
                            }

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;

                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    user.MyArchives.Handel(attacked, true);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.MyArchives.Handel(attacked, true);
                                }

                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.FireArrow))
                                CanUse.Add(1);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.IceArrow))
                                CanUse.Add(2);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                                CanUse.Add(3);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                                CanUse.Add(4);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                                CanUse.Add(5);
                            if (CanUse.Count != 0)
                                SpellEffect = CanUse[Pool.GetRandom.Next(0, CanUse.Count)];

                            MsgSpell.bomb = SpellEffect;
                            Updates.IncreaseExperience.Up(stream, user, 1000);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;

                        }
                    case (ushort)Role.Flags.SpellID.LightningKylin:
                    case (ushort)Role.Flags.SpellID.LightningKylinPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , Attack.OpponentUID, 0, 0, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    if (AnimationObj.Damage >= DBSpell.DamageOnHuman)
                                        AnimationObj.Damage = 22000;
                                    #region KylinSigilBurial
                                    Role.Instance.Ninja.Item item;
                                    if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.KylinSigilBurial, out item))
                                    {
                                        user.Player.KylinSigilGate++;
                                        if (user.Player.KylinSigilGate >= 2)
                                        {
                                            AnimationObj.Damage += (AnimationObj.Damage * (uint)item.DBItem.Damage / 100) / 100;
                                        }
                                    }
                                    #endregion
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    #region KylinSigilSeal
                                    if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.KylinSigilSeal, out item))
                                    {
                                        int Rate = item.DBItem.Power / 100;
                                        if (user.Player.BattlePower > attacked.BattlePower)
                                        {
                                            int Bp = user.Player.BattlePower - attacked.BattlePower;
                                            Rate += Bp * 5;
                                            if (Rate > 100)
                                                Rate = 100;
                                        }
                                        else if (attacked.BattlePower > user.Player.BattlePower)
                                        {
                                            int Bp = attacked.BattlePower - user.Player.BattlePower;
                                            Rate -= Bp * 5;
                                            if (Rate < 0)
                                                Rate = 0;

                                        }
                                        if (Role.Core.Rate(Rate))
                                        {
                                            if (!attacked.ContainFlag((MsgUpdate.Flags)279))
                                            {
                                                attacked.AddFlag((MsgUpdate.Flags)279, (int)(item.DBItem.Damage / 100), true);
                                                attacked.SendUpdate(stream, (MsgUpdate.Flags)279, (item.DBItem.Damage / 100), 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                            }


                                        }

                                    }
                                    #endregion
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DeathSigh:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , Attack.OpponentUID, 0, 0, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (official Death's Sigh 900% to monsters = power): AnimationObj.Damage = AnimationObj.Damage * 400 / 150;
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 120 / 100;
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
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
                            user.Player.AddFlag(MsgUpdate.Flags.DeathSigh, (int)DBSpell.Duration, true);
                            user.Player.DeathSighPassive = (uint)DBSpell.Damage2;
                            user.Player.DeathSighActive = (uint)DBSpell.Damage3;
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.DeathSigh, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.MonsterHunter:
                        {


                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , Attack.OpponentUID, 0, 0, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    Updates.IncreaseExperience.Up(stream, user, Experience);
                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                }
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.TripleBlasts:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                    {
                                        MsgSpell clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                        {
                                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                            {
                                                Database.MagicType.Magic spell;
                                                if (Spells.TryGetValue(clientspell.Level, out spell))
                                                {
                                                    if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                    {
                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                    {
                                        MsgSpell clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                        {
                                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                            {
                                                Database.MagicType.Magic spell;
                                                if (Spells.TryGetValue(clientspell.Level, out spell))
                                                {
                                                    if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                    {
                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
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
                    case (ushort)Role.Flags.SpellID.FlyingMoon:
                        {
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = AnimationObj.Damage * 40 / 100;
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.TwofoldBlades:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (official tooltip: TwofoldBlades 110-130%, SuperTwofoldBlade 112-135%): AnimationObj.Damage = AnimationObj.Damage * 80 / 100; // تعديل الدمج
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuperTwofoldBlade:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.MoveY = (uint)distance;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                    }
                                    else
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= (DBSpell.Range - 2))
                                    {
                                        if (attacked.ContainFlag(MsgUpdate.Flags.HeavensWrath))
                                        {
                                            return;
                                        }
                                        if (attacked.ContainFlag(MsgUpdate.Flags.Infinity))
                                        {
                                            return;
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj, false, 1);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official tooltip: TwofoldBlades 110-130%, SuperTwofoldBlade 112-135%): AnimationObj.Damage = AnimationObj.Damage * 80 / 100; // تعديل الدمج

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                        #region GapingWounds
                                        MsgSpell Owner_spellss = null;
                                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds))
                                        {
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GapingWounds, out Owner_spellss))
                                            {

                                                Database.MagicType.Magic GapingWounds = Pool.Magic[Owner_spellss.ID][Owner_spellss.Level];
                                                if (Calculate.Base.Rate(30))
                                                {
                                                    InteractQuery InteractQuery = new InteractQuery();
                                                    InteractQuery.SpellID = Owner_spellss.ID;
                                                    InteractQuery.SpellLevel = Owner_spellss.Level;
                                                    InteractQuery.X = user.Player.X;
                                                    InteractQuery.Y = user.Player.Y;
                                                    InteractQuery.UID = user.Player.UID;
                                                    InteractQuery.OpponentUID = attacked.UID;
                                                    MsgAttackPacket.ProcescMagic(user, stream, InteractQuery, true);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (attacked.ContainFlag(MsgUpdate.Flags.HeavensWrath))
                                        {
                                            return;
                                        }
                                        if (attacked.ContainFlag(MsgUpdate.Flags.Infinity))
                                        {
                                            return;
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        AnimationObj.MoveY = (uint)distance;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                        #region GapingWounds
                                        MsgSpell Owner_spellss = null;
                                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds))
                                        {
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GapingWounds, out Owner_spellss))
                                            {

                                                Database.MagicType.Magic GapingWounds = Pool.Magic[Owner_spellss.ID][Owner_spellss.Level];
                                                if (Calculate.Base.Rate(30))
                                                {
                                                    InteractQuery InteractQuery = new InteractQuery();
                                                    InteractQuery.SpellID = Owner_spellss.ID;
                                                    InteractQuery.SpellLevel = Owner_spellss.Level;
                                                    InteractQuery.X = user.Player.X;
                                                    InteractQuery.Y = user.Player.Y;
                                                    InteractQuery.UID = user.Player.UID;
                                                    InteractQuery.OpponentUID = attacked.UID;
                                                    MsgAttackPacket.ProcescMagic(user, stream, InteractQuery, true);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.MoveY = (uint)distance;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                    }
                                    else
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);

                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.EagleEye:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.BlackSpotEagle = DateTime.Now;
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    if (attacked.BlackSpot)
                                    {
                                        if (Calculate.Base.Rate(80))
                                        {
                                            user.Player.BlackSpotEagle = DateTime.Now;
                                            MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                    , 0, user.Player.X, user.Player.Y, 11130
                                    , ClientSpell.Level, 0);
                                            RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                            RemoveCloudDown.SetStream(stream);
                                            RemoveCloudDown.JustMe(user);
                                        }
                                    }
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    if (attacked.BlackSpot)
                                    {
                                        user.Player.BlackSpotEagle = DateTime.Now;
                                        MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                , 0, user.Player.X, user.Player.Y, 11130
                                , 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);



                                }
                                Updates.IncreaseExperience.Up(stream, user, Experience);
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.RapidFire:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 20 / 2000; // Abdullah Khaled Done (Attack)

                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Abdullah Khaled Done (Attack)
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            user.MyArchives.StarFlow(user, user.Player.UID, Attack.OpponentUID);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.MortalWound:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 20 / 250; // Abdullah Khaled Done (Attack)
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 30 / 225; // Abdullah Khaled Done (Attack)
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            user.MyArchives.StarFlow(user, user.Player.UID, Attack.OpponentUID);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DevouringStrike:
                        {
                            if (!Database.AtributesStatus.IsThunderStriker(user.Player.Class)) break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpell.OpponentUID = attacked.UID;
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpell.OpponentUID = attacked.UID;
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    if (AnimationObj.Damage >= DBSpell.DamageOnHuman)
                                        AnimationObj.Damage = 22000;
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpell.OpponentUID = attacked.UID;
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            user.Player.ThunderStrikerEnergy += 20;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonFury:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {


                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    if (user.Player.BattlePower > attacked.BattlePower)
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.DragonFury, (int)DBSpell.Duration, true);
                                        attacked.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DragonFury, DBSpell.Duration
           , (uint)33, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DragonFury, true);
                                    }

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {


                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpeedKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                  , 0, Attack.X, Attack.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.AirSweep;

                            List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
                                , Attack.Y, user.Map, Math.Min((byte)DBSpell.Range, (byte)Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y)));
                            if (coord == null || coord.Count == 0) return;

                            MsgSpell.X = (ushort)coord[coord.Count - 1].X;
                            MsgSpell.Y = (ushort)coord[coord.Count - 1].Y;
                            if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord[coord.Count - 1].X, coord[coord.Count - 1].Y))
                            {
                                break;
                            }

                            user.Map.View.MoveTo<Role.IMapObj>(user.Player, MsgSpell.X, MsgSpell.Y);
                            user.Player.X = MsgSpell.X;
                            user.Player.Y = MsgSpell.Y;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 5, coord))
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
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.Map != 1039)
                                    {
                                        if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 0, coord))
                                            break;
                                    }
                                    user.Shift(attacked.X, attacked.Y, stream);
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ViolentKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.StormKick;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    //   user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.Tornado:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);                                    
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    if (attacked.Boss > 0 && attacked.Alive)
                                        MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, user);


                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                                {

                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // تعديل دمج الاسكل
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);


                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {

                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Fire:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                }
                            }

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {

                                var attacked = target as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 13)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 40 / 100; // تعديل دمج الاسكل
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        Sering.Proces(user, attacked, stream);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 13)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
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
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }

                    case (ushort)Role.Flags.SpellID.Thunder:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                }
                            }

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {

                                var attacked = target as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 13)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 40 / 100; // تعديل دمج الاسكل
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                        Sering.Proces(user, attacked, stream);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 13)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
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
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HeavenBlade:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    if (attacked.Boss > 0 && attacked.Alive)
                                        MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, user);


                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                                {

                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip/MagicType value): AnimationObj.Damage = AnimationObj.Damage * 80 / 100; // تعديل دمج الاسكل
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);


                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {

                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.StormKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.StormKick;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y, stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);

                            break;
                        }
                    default:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                }
            }
            else
            {
                if (ClientSpell == null)
                    return;
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.EagleEye:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.BlackSpotEagle = DateTime.Now;
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell) && attacked.Alive)
                                {



                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    if (attacked.BlackSpot)
                                    {
                                        user.Player.BlackSpotEagle = DateTime.Now;
                                        MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, 11130, 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                {

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    if (attacked.BlackSpot)
                                    {
                                        user.Player.BlackSpotEagle = DateTime.Now;
                                        MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                , 0, user.Player.X, user.Player.Y, 11130
                                , 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }

                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);



                                }
                                Updates.IncreaseExperience.Up(stream, user, Experience);
                            }

                            break;
                        }
                }
            }
        }

    }

}
