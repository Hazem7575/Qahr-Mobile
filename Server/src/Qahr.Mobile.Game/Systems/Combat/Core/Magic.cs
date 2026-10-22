using Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate
{
    public class Magic
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, int counter = 1)
        {

            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            bool update = false;
            bool Strike = false;
            #region Flooritem
            if (monster.IsFloor)
            {
                SpellObj.Damage = 1;
                return;
            }
            #endregion

            #region MAttack
            if (player.Owner.GodDmgMode)
            {
                SpellObj.Damage = 2000000000;
                return;
            }
            int Damage = (int)player.Owner.AjustMagicAttack();
            #endregion
            #region PhoenixGem
            if (player.Owner.Status.MagicPercent > 0)
            {
                Damage += (int)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            }
            #endregion
            int IncDamagee = 0;
            #region Boss Killer

            byte BossKillerL = 0;

            if (player.Owner.Rune.IsEquipped("BossKiller", ref BossKillerL)
                || player.Owner.Rune.IsEquipped("Battle Reaper", ref BossKillerL))
            {
                if (monster.Boss > 0)
                {
                    uint[] BossBonus =
        {
            0,
            220, // +1
            230, // +2
            240, // +3
            250, // +4
            260, // +5
            270, // +6
            280, // +7
            290, // +8
            300  // +9
        };

                    if (BossKillerL >= 1 && BossKillerL <= 9)
                    {
                        Damage = (int)((long)Damage * BossBonus[BossKillerL] / 100);

                        player.Owner.SendSysMesage(string.Format(
                            "You activated Boss Killer (Lv.{0}) - Boss Damage +{1}%",
                            BossKillerL,
                            BossBonus[BossKillerL]));
                    }
                }
            }

            #endregion
            #region DBSpell(Edit)
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Thunder)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireRing)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireCircle)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireMeteor)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Bomb)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DivineAttraction)
                {
                    Damage += (int)(uint)DBSpell.DamageOnHuman;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SubstitutionAttack)
                {
                    Damage += (int)(uint)DBSpell.DamageOnHuman;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HeavenBlade)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChainBolt)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Vulcano)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }


            }
            #endregion
            #region MTD
            if (!update)
            {
                Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? (uint)DBSpell.Damage + IncDamagee : Program.ServerConfig.PhysicalDamage + IncDamagee), 100);
            }
            #endregion
            #region CriticalStrike
            if (Role.Core.Rate((int)(player.Owner.Status.SkillCStrike / 100) / 3))
            {
                if (player.Owner.AjustMCriticalStrike() > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    Damage = Base.MulDiv((int)Damage, (int)(50), 100);
                    Strike = true;
                }
            }
            #endregion

            //#region LuckyStrike(X2)
            //if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike / 100))
            //{
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //        {
            //            Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //            Id = player.UID
            //        }), true);
            //    }
            //    SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //    Damage = Base.MulDiv((int)Damage, 200, 100);
            //}
            //#endregion

            #region DashRate
            if (!Strike)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 200, 100);
                }

            }
            #endregion
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            #region Damage(INC)
            SpellObj.Damage += (uint)player.getFan(false);

            SpellObj.Damage = (uint)Base.CalcDamageUser2Monster((int)SpellObj.Damage, monster.Family.Defense, player.Level, monster.Level, false);
            SpellObj.Damage = (uint)Base.AdjustMinDamageUser2Monster((int)SpellObj.Damage, player.Owner);
            SpellObj.Damage += player.Owner.Status.MagicDamageIncrease;
            #endregion

            SpellObj.Damage = (uint)(SpellObj.Damage * 5);



            #region Monster(Guard)
            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            #endregion
            if (monster.Family.ID == 7562)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 2699)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 2700)
                SpellObj.Damage = 1;


            #region NewUpdate(Reducation) && DetiyLand
            if (monster.Boss > 0)
                SpellObj.Damage /= 3;
            #endregion

            #region WildPhoenix
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire || DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado || DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix) && player.ContainFlag(MsgUpdate.Flags.WildPhoenix))
                    {
                        Database.MagicType.Magic WildPhoenix = Pool.Magic[(ushort)Role.Flags.SpellID.WildPhoenix][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WildPhoenix].Level];
                        SpellObj.Damage += (uint)(SpellObj.Damage * WildPhoenix.DamageOnHuman) / 100;
                    }
                }
            }
            #endregion

            #region Monster(Damage)//1
            if (monster.Family.Defense2 == 0)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 5385)
                SpellObj.Damage = 0;
            if (monster.Family.ID == 3636)
                SpellObj.Damage = 1;

            #region Bow Special Damage

            bool HasSpecialBow = false;
            MsgGameItem EquipItem = null;

            // Normal Right Weapon
            if (player.Owner.Equipment.TryGetEquip(
                Role.Flags.ConquerItem.RightWeapon,
                out EquipItem))
            {
                if (EquipItem != null &&
                    Database.ItemType.IsBow(EquipItem.ITEM_ID))
                {
                    HasSpecialBow = true;
                }
            }

            // Alternate Right Weapon
            if (!HasSpecialBow &&
                player.Owner.Equipment.TryGetEquip(
                    Role.Flags.ConquerItem.AleternanteRightWeapon,
                    out EquipItem))
            {
                if (EquipItem != null &&
                    Database.ItemType.IsBow(EquipItem.ITEM_ID))
                {
                    HasSpecialBow = true;
                }
            }

            // Special Monsters
            if (HasSpecialBow)
            {
                uint monsterId = monster.Family.ID;

                if (monsterId == 4220 || // Nemesis Tyrant
                    monsterId == 4171 || // Snow Banshee
                    monsterId == 4212 || // Thrilling Spook
                    monsterId == 3976 || // Bloody Banshee
                    monsterId == 3977 || // Chilling Spook
                    monsterId == 3971 || // Dragon Wraith
                    monsterId == 3970 || // Queen Of Evil
                    monsterId == 3978)   // Nether Tyrant
                {
                    SpellObj.Damage = 10000;


                }
            }

            #endregion


            if (monster.Family.ID == 40124)
                SpellObj.Damage = 1;
            if (monster.Name == "AncientDragon") SpellObj.Damage = 1;
            else if (monster.Name == "WarDevil") SpellObj.Damage = 1;
            else if (monster.Name.Contains("Sprite")) SpellObj.Damage = 1;
            else if (monster.Family.ID == 7562) SpellObj.Damage = 1;
            else if (monster.Family.ID == 2699) SpellObj.Damage = 1;
            else if (monster.Family.ID == 2700) SpellObj.Damage = 1;
            if (monster.Name.Contains("GhostReaver") || monster.Name.Contains("ThunderApeKing"))
            {
                SpellObj.Damage = 1;
            }
            #endregion


        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            #region !IsFire
            if (target.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                #region RedRune
                #region BurningSky
                if (player.Owner.Rune.IsEquipped("BurningSky"))
                {
                    if (target.ContainFlag(MsgUpdate.Flags.Fly) || target.ContainFlag(MsgUpdate.Flags.FatalStrike) || target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance) || target.ContainFlag(MsgUpdate.Flags.ImmortalForce))
                    {
                        SpellObj.Damage += 30000;
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.BurningSky;
                    }

                }
                #endregion
                #endregion
                else
                {
                    SpellObj.Damage = 1;
                }
                return;
            }
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                #region RedRune
                #region BurningSky
                if (player.Owner.Rune.IsEquipped("BurningSky"))
                {
                    if (target.ContainFlag(MsgUpdate.Flags.Fly) || target.ContainFlag(MsgUpdate.Flags.FatalStrike) || target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance))
                    {
                        SpellObj.Damage += 30000;
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.BurningSky;
                    }

                }
                #endregion
                #endregion
                else
                {
                    SpellObj.Damage = 1;
                }
                return;
            }
            if (target.ContainFlag(MsgUpdate.Flags.MagicDefender))
            {
                int Power = player.BattlePower - target.BattlePower;
                if (Power < 20)
                {
                    SpellObj.Damage = 1;
                    SpellObj.Effect = MsgAttackPacket.AttackEffect.Imunity;
                    return;
                }
            }
            #endregion



            #region Calculator(MAttack)
            if (player.Owner.GodDmgMode)
            {
                SpellObj.Damage = 2000000000;
                return;
            }
            SpellObj.Damage = player.Owner.AjustMagicAttack();
            #endregion

            #region lianhuaran
            if (target.ContainFlag(MsgUpdate.Flags.lianhuaran01))
                SpellObj.Damage += 800;
            else if (target.ContainFlag(MsgUpdate.Flags.lianhuaran02))
                SpellObj.Damage += 1500;
            else if (target.ContainFlag(MsgUpdate.Flags.lianhuaran03))
                SpellObj.Damage += 2000;
            #region DBSpell(Edit)
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Thunder)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireRing)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireCircle)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireMeteor)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Bomb)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DivineAttraction)
                {
                    SpellObj.Damage += (uint)DBSpell.DamageOnHuman;

                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HeavenBlade)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChainBolt)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Vulcano)
                {
                    SpellObj.Damage += (uint)DBSpell.GDamage;
                }

            }
            // [CombatAudit 2026-10-14] removed custom cap "if (Damage > 50000) Damage -= 10000" (official tooltips: magic skill = M-Attack + power, e.g. Tornado "M-Attack: +1166")
            if (DBSpell.ID == (ushort)Role.Flags.SpellID.SubstitutionAttack)
            {
                SpellObj.Damage += (uint)DBSpell.DamageOnHuman;

            }
            #endregion
            #endregion

            #region PhoenixGem
            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage += (SpellObj.Damage * player.Owner.Status.MagicPercent) / 100;

            }
            #endregion

            #region Blessed
            SpellObj.Damage -= (uint)(SpellObj.Damage * target.Owner.Status.ItemBless / 100);
            #endregion

            #region MagicDefence
            uint MagicDefence = target.Owner.Status.MagicDefence;
            uint MagicPercent = target.Owner.Status.MDefence;
            #region InvisbleArrowPenetration
            uint Enhance = 0;
            if (player.Owner.PerfectionStatus.InvisibleArrow > 0)
            {
                if (Base.Rate(player.Owner.PerfectionStatus.InvisibleArrow))
                {
                    if (player.Owner.Status.Penetration > 0)
                    {
                        Enhance = 5;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.InvisbleArrow,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                        }
                    }
                }
            }
            #endregion
            if (MagicPercent > (player.Owner.Status.Penetration / 100) + Enhance)
                MagicPercent -= (player.Owner.Status.Penetration / 100) + Enhance;
            else
                MagicPercent = 1;
            MagicDefence += (MagicDefence * MagicPercent) / 100;
            #endregion

            SpellObj.Damage = Calculate.Base.CalcaulateDeffence(SpellObj.Damage, MagicDefence);

            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, target.Owner.Status.MagicDamageDecrease);
            #region TortoiseBreaker
            int reduction = Base.MulDiv((int)target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem), 80, 100);
            byte torReduction = 0;
            if (player.Owner.Rune.IsEquipped("TortoiseBreaker", ref torReduction) || player.Owner.Rune.IsEquipped("Tortoise Predator", ref torReduction))
                reduction = (int)((double)reduction * (double)torReduction / 100d);
            SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, (int)(100 - Math.Min(47, reduction)), 100);
            #endregion

            bool onbreak = false;

            bool update = false;

            bool Strike = false;

            #region TailedBeast
            if (target.Owner.Beasts.Activated && Base.Rate(1))
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.TailedBeast;
            #endregion

            #region AjustMCriticalStrike
            if (player.Owner.AjustMCriticalStrike() > 0)
            {
                if (Base.GetRefinery(player.Owner.AjustMCriticalStrike() / 100, target.Owner.AjustImunity() / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = (uint)(SpellObj.Damage * 1.5);
                    update = true;
                    Strike = true;
                }
                #region CoreStrike
                int rateM = player.Owner.PerfectionStatus.CoreStrike;
                if (player.Owner.PerfectionStatus.CoreStrike >= 17)
                    rateM = 20;
                if (Base.Rate(rateM))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.CoreStrike,
                            Id = player.UID,
                            dwParam = player.UID
                        }), true);
                    }

                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = (uint)(SpellObj.Damage * 1.5);
                    update = true;
                }
                #endregion
            }
            #endregion

            //#region LuckyStrike(X2)
            //if (!update && Base.Rate(player.Owner.PerfectionStatus.LuckyStrike / 100))
            //{
            //    if (Base.Rate(target.Owner.PerfectionStatus.StrikeLock))
            //    {
            //        if (!(Base.Rate(player.Owner.PerfectionStatus.AbsoluteLuck) && player.Owner.PrestigeLevel > target.Owner.PrestigeLevel))
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //                {
            //                    Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
            //                    Id = player.UID
            //                }), true);
            //            }
            //        }
            //        else
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //                {
            //                    Effect = MsgRefineEffect.RefineEffects.AbsoluteLuck,
            //                    Id = player.UID
            //                }), true);
            //            }
            //            SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //            SpellObj.Damage = (uint)(SpellObj.Damage * 2);
            //            update = true;
            //        }
            //    }
            //    else
            //    {
            //        using (var rec = new ServerSockets.RecycledPacket())
            //        {
            //            var stream = rec.GetStream();
            //            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //            {
            //                Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //                Id = player.UID
            //            }), true);
            //        }
            //        SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //        SpellObj.Damage = (uint)(SpellObj.Damage * 2);
            //        update = true;
            //    }
            //}
            //#endregion

            #region Breakthrough
            if (!update && player.Owner.AjustBreakthrough() > 0)
            {
                //  if (Base.GetRefinery(player.Owner.AjustBreakthrough() / 10 , target.Owner.AjustAntiBreack() / 10))
                {
                    onbreak = true;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;

                }
            }
            #endregion

            #region DashRate
            if (player.Owner.Status.DashRate > 0 && !update)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    SpellObj.Damage = (uint)(SpellObj.Damage * 1.25);
                }

            }
            #endregion

            #region EditDamage!Break
            if (onbreak == false)
            {
                var olddamage = SpellObj.Damage;
                SpellObj.Damage = (uint)Database.Disdain.UserAttackUser(player, target, (int)SpellObj.Damage);
                if (SpellObj.Damage < olddamage)
                {
                    SpellObj.Damage += olddamage / 2;
                }
            }
            #endregion

            #region GetDefence
            if (target.Reborn > 0)
            {
                SpellObj.Damage = (uint)Base.BigMulDiv((int)SpellObj.Damage, 7000, Client.GameClient.DefaultDefense2);
            }
            #endregion

            #region AzureShield&&Judgment
            byte itemLevell = 0;
            byte chanceIgnoreAzure5 = 0;
            if (player.Owner.Rune.IsEquipped("Judgment", ref itemLevell))
            {
                chanceIgnoreAzure5 = (byte)(10 + (itemLevell * 5));
                if (itemLevell == 9) chanceIgnoreAzure5 = 60;
            }
            if (!Calculate.Base.Rate(chanceIgnoreAzure5))
            {
                if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                {
                    if (SpellObj.Damage > target.AzureShieldDefence)
                    {
                        Calculate.AzureShield.CreateDmg(player, target, target.AzureShieldDefence);
                        target.RemoveFlag(MsgUpdate.Flags.AzureShield);
                        SpellObj.Damage -= target.AzureShieldDefence;

                    }
                    else
                    {
                        target.AzureShieldDefence -= (ushort)SpellObj.Damage;
                        Calculate.AzureShield.CreateDmg(player, target, SpellObj.Damage);
                        SpellObj.Damage = 1;
                    }
                }
            }
            #endregion

            #region Block
            int Block = (int)(target.Owner.Status.Block / 100);
            Block += (int)((target.ShieldBlockDamage * Block) / 100);
            uint Change = (uint)Math.Min(70, Block / 2);
            if (player.Owner.PerfectionStatus.ShieldBreak > 0)
            {
                if (Base.Rate(player.Owner.PerfectionStatus.ShieldBreak))
                {
                    if (!target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.SendUpdate(stream, MsgUpdate.Flags.ShieldBreak, 15, 0, 0, MsgUpdate.DataType.AzureShield);
                        }
                        target.AddFlag(MsgUpdate.Flags.ShieldBreak, 15, true);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.ShiledBreak,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                        }
                    }
                }
            }
            #endregion

            #region ShieldBreak
            if (target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
            {
                if (Change > 20)
                {
                    Change -= 20;
                }
                else
                {
                    Change = 0;
                }

            }
            #endregion

            #region WhettedBlade
            byte rate = 0;
            byte WhettedBlade = 0;
            if (player.Owner.Rune.IsEquipped("Whetted Blade", ref WhettedBlade) || player.Owner.Rune.IsEquipped("Honed Soul", ref WhettedBlade))
            {
                switch (WhettedBlade)
                {
                    case 1: rate = 1; break;
                    case 2: rate = 2; break;
                    case 3: rate = 3; break;
                    case 4: rate = 4; break;
                    case 5: rate = 5; break;
                    case 6: rate = 6; break;
                    case 7: rate = 7; break;
                    case 8: rate = 8; break;
                    case 9: rate = 10; break;
                }
            }
            if (!Calculate.Base.Rate((byte)rate))
            {
                if (Base.Rate((byte)Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;

                }
            }
            #endregion

            #region BlueRune
            #region RiseofTaoism
            if (player.ContainFlag(MsgUpdate.Flags.RiseofTaoism))
            {
                SpellObj.Damage += player.RiseofTaoismExtraMDamage;
            }

            #endregion
            #endregion

            #region RedRune
            #region BurningSky
            if (player.Owner.Rune.IsEquipped("BurningSky"))
            {
                if (target.ContainFlag(MsgUpdate.Flags.Fly))
                {
                    SpellObj.Damage += 30000;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.BurningSky;
                }

            }
            #endregion
            #endregion

            #region YellowRune
            #region LifeDrain
            byte LifeDrainL = 0;
            if (player.Owner.Rune.IsEquipped("LifeDrain", ref LifeDrainL))
            {

                uint IncHP = 0;
                if (Calculate.Base.Rate(10))
                {
                    switch (LifeDrainL)
                    {
                        case 1: IncHP = 10000; break;
                        case 2: IncHP = 12000; break;
                        case 3: IncHP = 14000; break;
                        case 4: IncHP = 15000; break;
                        case 5: IncHP = 17000; break;
                        case 6: IncHP = 18000; break;
                        case 7: IncHP = 20000; break;
                        case 8: IncHP = 25000; break;
                        case 9: IncHP = 30000; break;
                    }
                    player.HitPoints += (int)IncHP;
                    if (player.HitPoints > player.Owner.AjustMaxHitpoints())
                        player.HitPoints = (int)player.Owner.AjustMaxHitpoints();
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.SendString(stream, (MsgStringPacket.StringID)30, true, "hxdf_hf");
                    }
                    if (IncHP > 1)
                    {
                        SpellObj.Damage /= 4;
                        return;
                    }
                }
            }
            #endregion
            #region DivineShield
            byte DivineShieldL = 0;
            if (target.Owner.Rune.IsEquipped("DivineShield", ref DivineShieldL))
            {
                double percent = 0;
                percent = (byte)(DivineShieldL * 5);
                if (DivineShieldL == 9)
                    percent = 50;
                if (Role.Core.Rate(35))
                {
                    SpellObj.Damage -= (uint)((double)SpellObj.Damage * percent / 100d);
                }
            }
            #endregion
            #region Curse Block Rune
            if (target != null && target.Owner != null && player != null && player.Owner != null)
            {
                byte CurseLevel = 0;

                if (target.Owner.Rune != null &&
                    target.Owner.Rune.IsEquipped("CurseBlock", ref CurseLevel))
                {
                    int chance = 0;

                    switch (CurseLevel)
                    {
                        case 1: chance = 10; break;
                        case 2: chance = 12; break;
                        case 3: chance = 14; break;
                        case 4: chance = 16; break;
                        case 5: chance = 18; break;
                        case 6: chance = 20; break;
                        case 7: chance = 22; break;
                        case 8: chance = 25; break;
                        case 9: chance = 30; break;
                    }

                    if (DBSpell != null)
                    {
                        if (Role.Core.Rate(chance))
                        {
                            int reduce = 40;

                            if (player.BattlePower > target.BattlePower)
                                reduce += 10;

                            SpellObj.Damage -= (uint)((SpellObj.Damage * reduce) / 100);

                            target.Owner.SendSysMesage(string.Format(
    "You activated Curse Block (Lv.{0}) - {1}% chance: -{2}% damage",
    CurseLevel,
    chance,
    reduce));
                        }
                    }
                }
            }
            #endregion
            #region Witchery
            byte WitcheryL = 0;
            if (player.Owner.Rune.IsEquipped("Witchery", ref WitcheryL)
                && SpellObj.Damage < 15000)
            {
                uint IncD = 0;
                switch (WitcheryL)
                {
                    case 1: IncD = 2000; break;
                    case 2: IncD = 2300; break;
                    case 3: IncD = 2600; break;
                    case 4: IncD = 3000; break;
                    case 5: IncD = 3500; break;
                    case 6: IncD = 4000; break;
                    case 7: IncD = 4500; break;
                    case 8: IncD = 5000; break;
                    case 9: IncD = 6000; break;
                }
                SpellObj.Damage += IncD;
                if (SpellObj.Damage > 15000)
                    SpellObj.Damage = 15000;
            }
            #endregion
            #endregion

            #region WildPhoenix
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire || DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado || DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix) && player.ContainFlag(MsgUpdate.Flags.WildPhoenix))
                    {
                        Database.MagicType.Magic WildPhoenix = Pool.Magic[(ushort)Role.Flags.SpellID.WildPhoenix][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WildPhoenix].Level];
                        SpellObj.Damage += (uint)(SpellObj.Damage * WildPhoenix.Damage2) / 100;
                    }
                }
            }
            #endregion

            #region FireofHell
            if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCurse) && player.Owner.Rune.IsEquipped("FireCurse"))
            {
                MsgSpell fireCurse;
                if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FireCurse, out fireCurse))
                {
                    var fireCurseDB = Pool.Magic[(ushort)Role.Flags.SpellID.FireCurse][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.FireCurse].Level];
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                    {
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(player.UID
             , target.UID, target.X, target.Y, fireCurse.ID
             , fireCurse.Level, fireCurse.UseSpellSoul);

                            FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = (uint)fireCurseDB.DamageOnHuman, Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = target.UID });
                            ReceiveAttack.Player.Execute(FireCurseMsgSpell.Targets.FirstOrDefault(), player.Owner, target);
                            FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(player.Owner);
                        }
                    }
                    else
                    {
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(player.UID
             , target.UID, target.X, target.Y, fireCurse.ID
             , fireCurse.Level, fireCurse.UseSpellSoul);

                            FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = (uint)fireCurseDB.Damage2, Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = target.UID });
                            ReceiveAttack.Player.Execute(FireCurseMsgSpell.Targets.FirstOrDefault(), player.Owner, target);
                            FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(player.Owner);
                        }
                    }
                }
            }

            #endregion
            #region Iron Bone
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows || DBSpell.ID == (ushort)Role.Flags.SpellID.NebulousHunt
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.FireCurse
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WeepStorm
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.FlameofDestruction
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterShockwave)
                {
                    byte AdamantL = 0;
                    byte Rate = 0;
                    if (player.Owner.Rune.IsEquipped("Iron Bone", ref AdamantL) || player.Owner.Rune.IsEquipped("Invincible Resolve", ref AdamantL))
                    {
                        switch (AdamantL)
                        {
                            case 1: Rate = 20; break;
                            case 2: Rate = 25; break;
                            case 3: Rate = 30; break;
                            case 4: Rate = 35; break;
                            case 5: Rate = 40; break;
                            case 6: Rate = 45; break;
                            case 7: Rate = 50; break;
                            case 8: Rate = 55; break;
                            case 9: Rate = 60; break;
                        }
                        if (Base.Rate(Rate))
                        {
                            uint decreaseDamage = SpellObj.Damage * 40 / 100;
                            if (SpellObj.Damage > decreaseDamage)
                                SpellObj.Damage -= decreaseDamage;
                        }

                    }
                }
            }

            #endregion
            MsgYuanshen.SwordBody(target.Owner, SpellObj);
            MsgSpellAnimation.SpellObj InRedirect;
            if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                SpellObj = InRedirect;
        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            bool update = false;
            bool Breakdown = false;
            bool Strike = false;
            #region Calculator(Attack)
            int Damage = (int)player.Owner.Status.MagicAttack;

            Damage += (int)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            #endregion
            int IncDamagee = 0;
            #region Breakdown
            byte BreakdownL = 0;

            if (player.Owner.Rune.IsEquipped("Breakdown", ref BreakdownL) || player.Owner.Rune.IsEquipped("Battle Reaper", ref BreakdownL))
            {
                IncDamagee += (int)(110 + BreakdownL * 10);
                Breakdown = true;
            }
            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
            {
                IncDamagee += 100;

            }
            #endregion
            #region DBSpell(Edit)
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Thunder)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireRing)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireCircle)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireMeteor)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Bomb)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DivineAttraction)
                {
                    Damage += (int)(uint)DBSpell.DamageOnHuman;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SubstitutionAttack)
                {
                    Damage += (int)(uint)DBSpell.DamageOnHuman;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HeavenBlade)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChainBolt)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Vulcano)
                {
                    Damage += (int)(uint)DBSpell.GDamage;
                    Damage = Base.MulDiv((int)Damage, (int)((IncDamagee != 0) ? (uint)IncDamagee : Program.ServerConfig.PhysicalDamage), 100);
                    update = true;
                }

            }
            #endregion
            #region MTD
            if (!update)
            {
                Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? (uint)DBSpell.Damage + IncDamagee : Program.ServerConfig.PhysicalDamage + IncDamagee), 100);
            }
            #endregion

            #region CriticalStrike
            if (Role.Core.Rate((int)(player.Owner.Status.SkillCStrike / 100) / 3))
            {
                if (player.Owner.AjustMCriticalStrike() > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    Damage = Base.MulDiv((int)Damage, (int)(50), 100);
                    Strike = true;
                }
            }
            #endregion

            //#region LuckyStrike(X2)
            //if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike / 100))
            //{
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //        {
            //            Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //            Id = player.UID
            //        }), true);
            //    }
            //    SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //    Damage = Base.MulDiv((int)Damage, 200, 100);
            //}
            //#endregion

            #region DashRate
            if (!Strike)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 200, 100);
                }

            }
            #endregion

            SpellObj.Damage = (uint)Math.Max(1, Damage);



            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, 0);
            #region WildPhoenix
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Fire || DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado || DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell)
                {
                    if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix) && player.ContainFlag(MsgUpdate.Flags.WildPhoenix))
                    {
                        Database.MagicType.Magic WildPhoenix = Pool.Magic[(ushort)Role.Flags.SpellID.WildPhoenix][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WildPhoenix].Level];
                        SpellObj.Damage += (uint)(SpellObj.Damage * WildPhoenix.DamageOnHuman) / 100;
                    }
                }
            }
            #endregion
            if (Breakdown)
                SpellObj.Effect = MsgAttackPacket.AttackEffect.BreakDown;

        }
    }
}