using Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate
{
    public class Physical
    {

        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);

            #region FloorItem
            if (monster.IsFloor)
            {
                SpellObj.Damage = 2;
                return;
            }
            #endregion

            #region Calculator(Attack)
            if (player.Owner.GodDmgMode)
            {
                SpellObj.Damage = 2000000000;
                return;
            }
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            #endregion

            #region Defense(Monster)


            var rawDefense = monster.Family.Defense;
            if (monster.Boss > 0)
                Damage -= (int)Damage * 95 / 100;
            Damage = Math.Max(0, Damage - rawDefense);
            #endregion



            // [CombatAudit 2026-10-14] official Superman (MagicType power 31000 = 1000%): "increase the damage to monsters by 10 times" -> x10 (was x13: Damage += Damage * (10 + 2))
            if (player.ContainFlag(MsgUpdate.Flags.Superman))
                Damage = Damage * 10;
            bool update = false;
            #region MTD
            if (MultipleDamage != 0)
            {
                Damage = Damage * MultipleDamage;
            }
            else
            {
                // [CombatAudit 2026-10-14] official Omnipotence tooltip "(350% on monsters)" = status_data1 - 30000
                if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(DBSpell.Damage2 % 1000), 100);
                    update = true;
                }
                // [CombatAudit 2026-10-14] official Overlord tooltip "(50%..75% DMG to human targets, 200% DMG to monsters)" = status_data0 - 30000
                if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Overlord)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                // [CombatAudit 2026-10-14] official BellShield STR_AWAKEN_MAGIC_DESC_25130 "([status_data1]-30000)% to monsters" (power is the aura reduction, not damage)
                if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.BellShield)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(DBSpell.Damage2 % 1000), 100);
                    update = true;
                }
                if (!update)
                    Damage = Base.MulDiv((int)Damage, (int)Base.MonsterPower(DBSpell), 100); // [CombatAudit 2026-10-14] uncapped official power vs monsters
            }

            #endregion
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

            #region Damage(INC)
            Damage += player.getFan(false);

            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);

            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);

            if (monster.Family.Defense2 > 0)
                Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);
            #endregion

            SpellObj.Damage = (uint)Math.Max(1, Damage);

            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.IronbonePassive
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit201NormalATK1
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit201NormalATK2
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit201NormalATK3
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackDragonhowl
    )
                {
                    Damage *= 9;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SacredBlessing
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.Celestial
                    )
                {
                    Damage /= 80;

                }
            }
            if (Database.AtributesStatus.IsTrojan(player.Class) && player.Owner.HundredWeapons.Unlocked)
                Damage /= 3;
            if (player.Owner.MyArchives.isOpen(Archives.TypeID.StoneCracker) || player.Owner.MyArchives.isOpen(Archives.TypeID.ThornCutter) || player.Owner.MyArchives.isOpen(Archives.TypeID.ColdMoon))
                Damage /= 3;
            #region CriticalStrike
            if (Base.GetRefinery(player.Owner.AjustCriticalStrike() / 100, monster.Family.ExtraCritical / 100))
            {
                if (player.Owner.AjustCriticalStrike() / 100 > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = (uint)(SpellObj.Damage * 1.5);
                    update = true;
                }
            }
            #endregion
            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
            {
                SpellObj.Damage = (uint)((double)SpellObj.Damage * 250 / 100d);

            }

            #region DashRate
            if (player.Owner.Status.DashRate > 0)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 125, 100);
                }

            }
            #endregion

            #region Destroy

            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit) || player.Owner.MyArchives.isOpen(Archives.TypeID.Dragonhowl) && player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit) && (SpellObj.Effect & MsgAttackPacket.AttackEffect.CriticalStrike) == MsgAttackPacket.AttackEffect.CriticalStrike)
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.Destroy;
                SpellObj.Damage += (uint)Base.MulDiv((int)SpellObj.Damage, 125, 100);
            }
            #endregion


            #region Monster(Guard)
            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            #endregion

            #region BonePulse
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                {
                    if (!player.ContainFlag(MsgUpdate.Flags.BonePulse) && player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BonePulse][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BonePulse].Level];
                            if (player.Rate(Spell.Damage2))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(player.UID, player.UID, player.X, player.Y, Spell.ID, Spell.Level, 0);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(player.Owner);
                                    if (player.AddFlag(MsgUpdate.Flags.BonePulse, (int)Spell.Duration, true))
                                        player.SendUpdate(stream, MsgUpdate.Flags.BonePulse, (uint)Spell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //#region Lucky Strike (X2)

            //if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
            //{
            //    if (Role.Core.Rate(42))
            //    {
            //        using (var rec = new ServerSockets.RecycledPacket())
            //        {
            //            var stream = rec.GetStream();
            //            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //            {
            //                Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //                Id = player.UID
            //            }), true);
            //        }
            //        SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 50, 100);

            //    }
            //}

            //#endregion
            #region XpSkill
            if (player.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                if (monster.Boss == 0)
                    Damage *= 10;
                else
                    Damage *= 3;
            }

            if (player.ContainFlag(MsgUpdate.Flags.Oblivion))
                Damage = Base.MulDiv((int)Damage, 200, 100);

            if (monster.Boss == 1)
            {
                if (player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage += 5000;
            }
            if (monster.Boss == 1)
            {
                if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                    SpellObj.Damage *= 3;
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
            #endregion

            MsgAttackPacket.HitInMele(player, DBSpell);
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, bool StackOver = false, int IncreaseAttack = 0)
        {

            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            MsgYuanshen.SwordBody(target.Owner, SpellObj);
            #region ShurikenVortex
            #region YellowRune(Megaquake)
            byte RateMegaquake = 0;
            byte Megaquake = 0;
            if (player.Owner.Rune.IsEquipped("Megaquake", ref Megaquake))
            {
                switch (Megaquake)
                {
                    case 1: RateMegaquake = 1; break;
                    case 2: RateMegaquake = 2; break;
                    case 3: RateMegaquake = 3; break;
                    case 4: RateMegaquake = 4; break;
                    case 5: RateMegaquake = 5; break;
                    case 6: RateMegaquake = 6; break;
                    case 7: RateMegaquake = 7; break;
                    case 8: RateMegaquake = 8; break;
                    case 9: RateMegaquake = 9; break;
                    case 10: RateMegaquake = 10; break;
                    case 11: RateMegaquake = 12; break;
                    case 12: RateMegaquake = 14; break;
                    case 13: RateMegaquake = 16; break;
                    case 14: RateMegaquake = 18; break;
                    case 15: RateMegaquake = 20; break;
                    case 16: RateMegaquake = 22; break;
                    case 17: RateMegaquake = 24; break;
                    case 18: RateMegaquake = 26; break;
                    case 19: RateMegaquake = 28; break;
                    case 20: RateMegaquake = 30; break;
                    case 21: RateMegaquake = 33; break;
                    case 22: RateMegaquake = 36; break;
                    case 23: RateMegaquake = 39; break;
                    case 24: RateMegaquake = 42; break;
                    case 25: RateMegaquake = 45; break;
                    case 26: RateMegaquake = 50; break;
                    case 27: RateMegaquake = 55; break;
                }
            }
            #endregion
            if (!Base.Rate(RateMegaquake))
            {
                if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance))
                {
                    SpellObj.Damage = 1;
                    return;
                }
            }
            #endregion

            #region DragonSwing
            if (target.ContainFlag(MsgUpdate.Flags.DragonSwing))
            {
                byte additionalRate = 0;
                if (target.Owner.Rune.IsEquipped("SwingingTail") && DBSpell != null && (DBSpell.ID == (ushort)Role.Flags.SpellID.EagleEye || DBSpell.ID == (ushort)Role.Flags.SpellID.BlackbeardsRage || DBSpell.ID == (ushort)Role.Flags.SpellID.KineticSpark || DBSpell.ID == (ushort)Role.Flags.SpellID.DaggerStorm || DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave || DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound))//SwingingTail(Dragon Warrior Rune)
                    additionalRate += 30;
                if (Calculate.Base.Rate(target.DragonSwingChance + additionalRate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        target.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                    }
                    SpellObj.Damage = 0;
                    return;
                }
            }
            #endregion

            #region FreezingPelter
            if (player.ContainFlag(MsgUpdate.Flags.FreezingPelter) && Calculate.Base.Rate(30 + (int)(player.BattlePower - (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DeathSigh) && target.BattlePower > player.BattlePower ? player.BattlePower : target.BattlePower) * 5)))
            {
                target.AddFlag(MsgUpdate.Flags.xFreezingPelter, 10, true);
            }
            #endregion

            bool update = false;

            #region Dodged
            if (DBSpell == null)
            {
                if (Base.Dodged(player.Owner, target.Owner))
                {
                    SpellObj.Damage = 0;
                    return;
                }
            }
            #endregion

            #region HideGui
            unsafe
            {
                using (var rect = new ServerSockets.RecycledPacket())
                {
                    var stream = rect.GetStream();
                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = target.UID,
                        Type = ActionType.HideGui,
                    };
                    target.Owner.Send(stream.ActionCreate(action));
                }
            }
            #endregion

            #region Calculator(Attack)
            if (player.Owner.GodDmgMode)
            {
                SpellObj.Damage = 2000000000;
                player.HitInMele = true;
                return;
            }
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);
            #endregion



            #region AjustDefense
            var rawDefense = target.Owner.AjustDefense;
            if (target.ContainFlag(MsgUpdate.Flags.DefensiveStance) && target.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance))
            {
                Database.MagicType.Magic DefensiveStance = Pool.Magic[(ushort)Role.Flags.SpellID.DefensiveStance][target.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DefensiveStance].Level];
                rawDefense = (rawDefense * (uint)(DefensiveStance.GDamage % 1000)) / 150;
            }

            if (player.Owner.Rune.IsEquipped("Wild Cleave") || player.Owner.Rune.IsEquipped("Peakcarver"))
            {
                MsgGameItem Shield;
                if (target.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out Shield))
                {
                    if (Database.ItemType.IsShield(Shield.ITEM_ID))
                    {
                        int Extra = 0;
                        #region SoulExtra
                        if (Shield.Purification.InLife)
                        {
                            Database.ItemType.DBItem purificare;
                            if (Pool.ItemsBase.TryGetValue(Shield.Purification.PurificationItemID, out purificare))
                                Extra += purificare.PhysicalDefence;
                        }
                        #endregion
                        #region ItemExtra&&ItemBounce
                        var DBItem = Pool.ItemsBase[Shield.ITEM_ID];
                        byte AnimaPercent = 0;
                        if (Shield.AnimaItemID > 0)
                        {
                            Database.ItemType.DBItem animaItem;
                            if (Pool.ItemsBase.TryGetValue(Shield.AnimaItemID, out animaItem))
                                AnimaPercent = (byte)(animaItem.ItemHP / 100);
                        }
                        var extraitematributes = DBItem.Plus[Shield.Plus];
                        Extra += ((extraitematributes.PhysicalDefence + (extraitematributes.PhysicalDefence * AnimaPercent / 100)) + DBItem.PhysicalDefence);
                        #endregion
                        rawDefense -= (uint)Extra;
                    }
                }
            }
            if (Damage > rawDefense)
                Damage -= (int)rawDefense;
            else
                Damage = 1;
            #endregion

            #region BonePulse
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                {
                    if (!player.ContainFlag(MsgUpdate.Flags.BonePulse) && player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BonePulse][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BonePulse].Level];
                            if (player.Rate(Spell.Damage2))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(player.UID, player.UID, player.X, player.Y, Spell.ID, Spell.Level, 0);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(player.Owner);
                                    if (player.AddFlag(MsgUpdate.Flags.BonePulse, (int)Spell.Duration, true))
                                        player.SendUpdate(stream, MsgUpdate.Flags.BonePulse, (uint)Spell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                }
                            }
                        }
                    }
                }
            }
            #endregion




            bool Effect = false;

            int ReduceWarAegis = 0;

            #region War Aegis
            if (target.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarAegis))
            {
                var WarAgeis = Pool.Magic[(ushort)Role.Flags.SpellID.WarAegis][target.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WarAegis].Level];
                if (WarAgeis != null)
                {
                    ReduceWarAegis = (int)WarAgeis.Damage2 / 10;
                }
            }
            #endregion

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 80, 100);
                    update = true;
                }
            }

            #region Attack [ Monk ] By Mohy ElDin ...

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.RadiantPalm)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(DBSpell.GDamage % 1000), 100); // [CombatAudit 2026-10-14] RadiantPalm official 100-125% = power-30000 (was custom x1.1)
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.ClawStrike)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnMonster % 1000), 100); // [CombatAudit 2026-10-14] ClawStrike official Cn_Res STR_AWAKEN_MAGIC_DESC_25110: [data]-30000 vs players (was custom x1.3)
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.BellShield)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] BellShield official STR_AWAKEN_MAGIC_DESC_25130: [status_data0]-30000 vs players (was custom x1.3)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.QuellingRobe)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] QuellingRobe official STR_AWAKEN_MAGIC_DESC_25170: [status_data0]-30000 vs players (was custom x1.3)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.VajraRing)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] VajraRing official STR_AWAKEN_MAGIC_DESC_25250: [status_data0]-30000 vs players (was custom x1.3)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.ClapBomb)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] ClapBomb official STR_AWAKEN_MAGIC_DESC_25230: [status_data0]-30000 vs players (was custom x0.7)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttack)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 100, 100); // [CombatAudit 2026-10-14] TripleAttack official tooltip: launches 3 normal attacks (100% each) vs players (was custom x1.2)
                    update = true;
                }
            } 

            #endregion

            #region Attack [ Archer ] By Mohy ElDin ...

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK1)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 120, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK2)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 120, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK1)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK2)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK3)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK1)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK2)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK3)
            {
                if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                {
                    Damage = Base.MulDiv((int)Damage, 90, 100);
                    update = true;
                }
            }

            #endregion


            #region Attack [ Dune Wanderer ] By Mohy ElDin ...

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)((IncreaseAttack == 1 ? DBSpell.Damage2 : DBSpell.Damage3) % 1000), 100); // [CombatAudit 2026-10-14] official STR_AWAKEN_MAGIC_DESC_25690: 1st hit [status_data2]-30000, 2nd hit (after 1s) [status_data1]-30000 (was custom x3.5)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDDune.LonelyBattle)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] official STR_AWAKEN_MAGIC_DESC_25700: [status_data0]-30000 (was custom x3.5)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDDune.CliffCrusher)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)((IncreaseAttack == 1 ? DBSpell.Damage3 : DBSpell.DamageOnHuman) % 1000), 100); // [CombatAudit 2026-10-14] official STR_AWAKEN_MAGIC_DESC_25670: 1st hit [status_data0]-30000, far-end hit [status_data2]-30000 (was custom x3.5)
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SwallowDive)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, 350, 100);
                        update = true;
                    }
                }

            if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SwallowDive2)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) < 50)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpell.DamageOnHuman % 1000), 100); // [CombatAudit 2026-10-14] official STR_AWAKEN_MAGIC_DESC_25650: [25650|status_data0]-30000 (was custom x3.5)
                        update = true;
                    }
                }

                #endregion

                #region BonePluse
                using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MsgSpell Owner_spell = null;
                if (DBSpell != null)
                {
                    //if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                    //                || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                    //                || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                    //                || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                    //                || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                    //{
                    //    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                    //    {
                    //        if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                    //        {
                    //            if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.BonePulse, out Owner_spell))
                    //            {

                    //                Database.MagicType.Magic BonePulse = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                    //                int IncDg = BonePulse.Damage2 % 1000;
                    //                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                    //                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                    //                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive)
                    //                {
                    //                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000) - ReduceWarAegis, 100);
                    //                    IncDg -= DBSpell.Damage2 % 1000;
                    //                }


                    //                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                    //                    || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                    //                {
                    //                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000) - ReduceWarAegis, 100);
                    //                    IncDg -= DBSpell.DamageOnHuman % 1000;
                    //                }


                    //                Damage += (int)(Damage * (IncDg) / 100);

                    //                Effect = true;
                    //            }
                    //        }
                    //    }
                    //}
                    #region DeathSigh
                    if (player.ContainFlag(MsgUpdate.Flags.DeathSigh))
                    {
                        byte level = 0;
                        int cantidadnivel = 0;
                        if (target.Owner.Rune.IsEquipped("VioletShield", ref level))
                        {
                            switch (level)
                            {
                                case 1: cantidadnivel = 10; break;
                                case 2: cantidadnivel = 20; break;
                                case 3: cantidadnivel = 30; break;
                                case 4: cantidadnivel = 40; break;
                                case 5: cantidadnivel = 50; break;
                                case 6: cantidadnivel = 60; break;
                                case 7: cantidadnivel = 70; break;
                                case 8: cantidadnivel = 85; break;
                                case 9: cantidadnivel = 100; break;
                            }
                        }
                        if (!Role.Core.Rate(cantidadnivel))
                        {
                            if (DBSpell == null || DBSpell.Passive)
                                Damage = (int)((double)Damage * (double)(player.DeathSighPassive) / 100d);
                            else Damage += (int)((double)Damage * (double)(player.DeathSighActive) / 100d);
                        }
                    }
                    #endregion


                    

                    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.Commanding) && player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniCommand))
                    {
                        if (DBSpell != null)
                        {

                            if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonRising || DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack1 || DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack2 || DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack3)
                            {
                                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniCommand][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level];
                                uint DBSpellDG = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.DBSpell.GetDBSpellDG(player, DBSpell, IncreaseAttack);

                                Effect = true;
                                if (player.SuanniCommandCount == 5)
                                    Damage = Base.MulDiv((int)Damage, (int)(DBSpellDG + 30) - ReduceWarAegis, 100);
                                else
                                    Damage = Base.MulDiv((int)Damage, (int)(DBSpellDG + (player.SuanniCommandCount * 5)) - ReduceWarAegis, 100);
                            }
                        }

                    }
                }
            }
            #endregion

            #region Edit Damage(MT)
            if (DBSpell != null && !Effect)
            {

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackBloodlust
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackRedcurse)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage * 2 - ReduceWarAegis, 100);
                    update = true;
                }
                #region Edit Damage(Index)[10]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleBlasts)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage / 2 - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeTempest)
                {
                    MsgGameItem RightWeapon;
                    MsgGameItem LeftWeapon;
                    if (player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon)
                        && player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                    {
                        if (Database.ItemType.IsPirateEpicWeapon(RightWeapon.ITEM_ID) && Database.ItemType.IsPirateEpicWeapon(LeftWeapon.ITEM_ID))
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                            update = true;
                        }
                    }

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SwirlingStorm)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage / 4 - ReduceWarAegis, 100);
                    update = true;
                }
                #endregion                              
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ZenStaff)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FlowerTouch)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.VajraPalm)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000) - ReduceWarAegis, 100); // [CombatAudit 2026-10-14] VajraPalm official STR_AWAKEN_MAGIC_DESC_25190: [status_data0]-30000 (was % 100 -> 0..45%)
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.VioletBowl)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }                                
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.RageofWar)
                {
                    // [CombatAudit 2026-10-14] official tooltip "Deals 98%..135% of normal damage to enemies" = data(col 44) - 30000 (was DamageOnHuman % 100 = 3%)
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnMonster % 1000) - ReduceWarAegis, 100);
                    update = true;
                }

                #region All Dune Wanderer Skills Edit By [ Mohy ElDin - محي الدين ]

                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.MoonwardLeap)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.TempestStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SwallowDive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                // if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SwallowDive2)  // [CombatAudit 2026-10-14] handled above with the official field (this block re-applied DamageOnHuman % 100)
                // {
                    // Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    // update = true;
                // }
                // if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.CliffCrusher)  // [CombatAudit 2026-10-14] handled above with the official field (this block re-applied DamageOnHuman % 100)
                // {
                    // Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    // update = true;
                // }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.WandererNormalATK)
                {
                    Damage = Base.MulDiv((int)Damage, 100 - ReduceWarAegis, 100); // [CombatAudit 2026-10-14] Dune Wanderer normal attack = 100% (was DamageOnHuman % 100 = 0 -> 1 damage)
                    update = true;
                }
                // if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)  // [CombatAudit 2026-10-14] handled above with the official field (this block re-applied DamageOnHuman % 100)
                // {
                    // Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    // update = true;
                // }
                // if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.LonelyBattle)  // [CombatAudit 2026-10-14] handled above with the official field (this block re-applied DamageOnHuman % 100)
                // {
                    // Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    // update = true;
                // }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SheathParry)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FleetingShadow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.SkyStep)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.HeroicHeart)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.TideReversal)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }

                #endregion

                #region All Dune Wanderer Archive Skills Edit By [ Mohy ElDin - محي الدين ]

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DualStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.InnerSight)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.PulseLock)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.MightyBlaze)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.MirrorStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DreadSlash)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DreadSlash)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.RemoteHit)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Unruffled)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;
                }

                #endregion

                #region DamageOnHuman(Index)[35]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderStrike
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.CleanSweep
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SongofPhoenix
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.AxeShadow
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DeadlyStrike
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SacredBlessing
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DeathSigh
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Celestial
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Roamer
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWind
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachment
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.BloodyScythe
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WildDashAttack
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.ArrowBlades
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.RevengeAttack
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Megabolt
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.SkyFall
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderBlast
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows
                         || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Drukyle
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolCano
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Spitfire
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SandMist
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.CaptiveArrow
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SpitfirePassive
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive
                                         || DBSpell.ID == (ushort)Role.Flags.SpellID.DragonTransformationPassive

                         || DBSpell.ID == (ushort)Role.Flags.SpellID.SuanniRoar
                               || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.DrukylePassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSea
                           || DBSpell.ID == (ushort)Role.Flags.SpellID.DragonRising
                     )
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                    update = true;
                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarFlow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                    update = true;
                }



                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ElementalArrow)
                {
                    uint DamageInc = 0;
                    Database.MagicType.Magic ElementalArrow = Pool.Magic[(ushort)Role.Flags.SpellID.ElementalArrow][(ushort)player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ElementalArrow].Level];
                    if (player.ContainFlag(MsgUpdate.Flags.FireArrow)
                           || player.ContainFlag(MsgUpdate.Flags.IceArrow)
                           || player.ContainFlag(MsgUpdate.Flags.PoisonArrow)
                           || player.ContainFlag(MsgUpdate.Flags.ThunderArrow)
                           || player.ContainFlag(MsgUpdate.Flags.WindArrow))
                    {
                        if (player.ContainFlag(MsgUpdate.Flags.FireArrow))
                        {
                            player.RemoveFlag(MsgUpdate.Flags.FireArrow);
                            DamageInc += 20;
                        }
                        if (player.ContainFlag(MsgUpdate.Flags.IceArrow))
                        {
                            player.RemoveFlag(MsgUpdate.Flags.IceArrow);
                            DamageInc += 20;
                        }
                        if (player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                        {
                            player.RemoveFlag(MsgUpdate.Flags.PoisonArrow);
                            DamageInc += 20;
                        }
                        if (player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                        {
                            player.RemoveFlag(MsgUpdate.Flags.ThunderArrow);
                            DamageInc += 20;
                        }
                        if (player.ContainFlag(MsgUpdate.Flags.WindArrow))
                        {
                            player.RemoveFlag(MsgUpdate.Flags.WindArrow);
                            DamageInc += 20;
                        }


                    }
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + DamageInc) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireArrow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) - ReduceWarAegis, 100);
                    update = true;


                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackShot)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + DBSpell.Damage3 % 1000 - ReduceWarAegis, 100);
                    update = true;
                }
                if (IncreaseAttack == 1)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    // [CombatAudit 2026-10-14] FastBlade/ScentSword side targets: official tooltip "85%..105% on those on the two sides" = status_data0 (DamageOnHuman) - 30000
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.FastBlader || DBSpell.ID == (ushort)Role.Flags.SpellID.ScrenSword)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }

                    // if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)  // [CombatAudit 2026-10-14] handled in the Dune region with the official field
                    // {
                        // Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                        // update = true;
                    // }
                }
                if (IncreaseAttack == 0)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.DamageOnHuman % 1000) + DBSpell.Damage3) - ReduceWarAegis, 100);
                        update = true;
                    }
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 100 - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonCyclone)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 100 - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage)
                {
                    // [CombatAudit 2026-10-14] official tooltip "inflict 10% damage of normal attack (500% to monsters)" = status_data0 - 30000 (was hard-coded 50)
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
                {
                    // [CombatAudit 2026-10-14] official tooltip "Deal 10% of normal damage on nearby enemies (350% on monsters)" = status_data0 - 30000
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000) - ReduceWarAegis, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                    update = true;
                }
                #endregion

                #region Damage2(Index)[36]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireball
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlash
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrison
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.BonePulse
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.Ironbone
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.IronbonePassive
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeak
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAge
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGun
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGunPassive
        || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanction
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanctionPassive
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive
             || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAgePassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 - ReduceWarAegis, 100);
                    update = true;
                }
                if (IncreaseAttack == 1)
                {
                    //if (DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                    //{
                    //    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 - ReduceWarAegis, 100);
                    //    update = true;
                    //}
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpear)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + DBSpell.DamageOnHuman) - ReduceWarAegis, 100);
                    update = true;
                }
                #endregion

                #region Damage3(Index)[37]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterShockwave
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.HookMoon
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.HellVortex)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 - ReduceWarAegis, 100);
                    update = true;

                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TwilightDance)
                {
                    if (IncreaseAttack == 1)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 2)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 3)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage2 % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }


                }
                #endregion

                #region DamageOnMonster(Index)[44]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK1
                   || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK1
                  || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK1
                                || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK2
                                              || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit402NormalATK3
                   || DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK2
                   || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK2
                   || DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK3
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingShock
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.WindstormBattleaxe
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.CannonBarrage
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.Wildwind || DBSpell.ID == (ushort)Role.Flags.SpellID.HeavensWrath)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 - ReduceWarAegis, 100);
                    update = true;
                }
                if (IncreaseAttack == 1)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                }


                #endregion
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChaoticDanceAttack)
                {
                    #region SilentBlade

                    double RateSilentBlade = 0;
                    int reduceDMG = 0;
                    byte SilentBlade = 0;
                    if (target.Owner.Rune.IsEquipped("Silent Blade", ref SilentBlade))
                    {
                        switch (SilentBlade)
                        {
                            case 1: RateSilentBlade = 25; break;
                            case 2: RateSilentBlade = 27; break;
                            case 3: RateSilentBlade = 29; break;
                            case 4: RateSilentBlade = 31; break;
                            case 5: RateSilentBlade = 33; break;
                            case 6: RateSilentBlade = 36; break;
                            case 7: RateSilentBlade = 39; break;
                            case 8: RateSilentBlade = 42; break;
                            case 9: RateSilentBlade = 45; break;
                        }

                    }
                    #endregion
                    if (IncreaseAttack == 1)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 2)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 3)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 - ReduceWarAegis, 100);
                        update = true;
                    }
                    if (RateSilentBlade > 0)
                    {
                        reduceDMG = (int)Damage * (int)RateSilentBlade / 100;
                        Damage -= reduceDMG;

                    }


                }
            }

            #endregion

            #region Damage(Index)[10]

            if (DBSpell != null)
            {

            }
            if (!update && !Effect)
            {
                if (DBSpell != null)
                {

                }
                if (IncreaseAttack == 0)
                {
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null && DBSpell.Damage > 0) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage) - ReduceWarAegis, 100);
                }
            }
            #endregion

            bool onbreak = false;

            update = false;

            bool Strike = false;

            #region TailedBeast
            if (target.Owner.Beasts.Activated && Base.Rate(1))
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.TailedBeast;
            #endregion

            #region CriticalStrike
            if (player.Owner.AjustCriticalStrike() > 0 && !Effect)
            {
                uint WhirlSigilSage1 = 0;
                uint WhirlSigilSage2 = 0;
                if (DBSpell != null)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken)
                    {
                        Ninja.Item item;
                        if (player.Owner.MyNinja.TryGetValueEquip(Ninja.ItemType.WhirlSigilSage, out item))
                        {
                            WhirlSigilSage1 = (uint)item.DBItem.Power / 100;
                            WhirlSigilSage2 = (uint)item.DBItem.Damage;
                        }
                    }
                }
                if (Base.GetRefinery((player.Owner.AjustCriticalStrike() / 100) + WhirlSigilSage1, target.Owner.AjustImunity() / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    if (player.Owner.Rune.IsEquipped("StrikeBooster") || player.Owner.Rune.IsEquipped("Peakcarver"))
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(140 + WhirlSigilSage2), 100);
                    }
                    else
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(120 + WhirlSigilSage2), 100);
                    }


                    update = true;
                }
            }
            #endregion
            // #region Lucky Strike [X2]
            // if (!update && Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
            // {
            //     if (Role.Core.Rate(42))
            //     {
            //         if (Base.Rate(target.Owner.PerfectionStatus.StrikeLock))
            //         {
            //             using (var rec = new ServerSockets.RecycledPacket())
            //             {
            //                 var stream = rec.GetStream();
            //                 target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //                 {
            //                     Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
            //                     Id = player.UID
            //                 }), true);
            //             }
            //         }
            //         else
            //         {
            //             if (player.Owner.PerfectionStatus.LuckyStrike != target.Owner.PerfectionStatus.LuckyStrike)
            //             {
            //                 using (var rec = new ServerSockets.RecycledPacket())
            //                 {
            //                     var stream = rec.GetStream();
            //                     target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //                     {
            //                         Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //                         Id = player.UID
            //                     }), true);
            //                 }
            //                 SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //                 Damage = Base.MulDiv((int)Damage, 50, 100);
            //                 update = true;
            //             }
            //         }
            //     }
            // }
            //#endregion


            //#region Lucky Strike (X2)

            //if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
            //{
            //    if (Role.Core.Rate(42))
            //    {
            //        using (var rec = new ServerSockets.RecycledPacket())
            //        {
            //            var stream = rec.GetStream();
            //            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //            {
            //                Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //                Id = player.UID
            //            }), true);
            //        }
            //        SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 50, 100);

            //    }
            //}

            //#endregion

            #region Breakthrough
            if (!update && player.Owner.AjustBreakthrough() > 0)
            {
                #region Front Break
                byte FrontBreakL = 0;
                ushort INCB = 0;
                if (player.Owner.Rune.IsEquipped("Front Break", ref FrontBreakL) && player.Owner.Player.ContainFlag(MsgUpdate.Flags.FrontBreak))
                {
                    switch (FrontBreakL)
                    {
                        case 1: INCB = 10; break;
                        case 2: INCB = 20; break;
                        case 3: INCB = 30; break;
                        case 4: INCB = 40; break;
                        case 5: INCB = 50; break;
                        case 6: INCB = 60; break;
                        case 7: INCB = 70; break;
                        case 8: INCB = 80; break;
                        case 9: INCB = 100; break;
                    }
                }
                #endregion
                if (Base.GetRefinery((player.Owner.AjustBreakthrough() + INCB) / 11, target.Owner.AjustAntiBreack() / 10))
                {
                    onbreak = true;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Penetration;

                }
            }
            #endregion

            #region DashRate
            if (!Strike)
            {
                if (Base.GetRefinery((player.Owner.Status.DashRate / 100), target.Owner.Status.Resist / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 125, 100);
                }
            }
            #endregion

            #region EditDamage!Break
            if (onbreak == false)
            {
                var olddamage = Damage;
                Damage = Database.Disdain.UserAttackUser(player, target, Damage);
                if (Damage < olddamage)
                {
                    Damage += olddamage / 4;
                }
            }
            #endregion

            #region GetDefence
            if (target.Reborn > 0)
                Damage = (int)Base.BigMulDiv((int)Damage, 6000, Client.GameClient.DefaultDefense2);
            #endregion

            #region Blessed
            Damage -= (int)(Damage * target.Owner.Status.ItemBless / 100);
            #endregion

            #region TortoisePercent
            var TortoisePercent = target.Owner.GemValues(Role.Flags.Gem.NormalTortoiseGem);

            if (TortoisePercent > 0)
            {
                if (TortoisePercent > 48)
                    TortoisePercent = 48;

                #region TortoiseSmasher
                if (DBSpell != null)
                {
                    byte TortoiseSmasherL = 0;
                    int ignores = 0;

                    if (player.Owner.Rune.IsEquipped("TortoiseSmasher", ref TortoiseSmasherL) ||
                        player.Owner.Rune.IsEquipped("Tortoise Predator", ref TortoiseSmasherL))
                    {
                        switch (TortoiseSmasherL)
                        {
                            case 1: ignores = 3; break;
                            case 2: ignores = 4; break;
                            case 3: ignores = 5; break;
                            case 4: ignores = 6; break;
                            case 5: ignores = 7; break;
                            case 6: ignores = 8; break;
                            case 7: ignores = 9; break;
                            case 8: ignores = 10; break;
                            case 9: ignores = 12; break;
                        }

                        TortoisePercent -= (uint)Math.Min(ignores, TortoisePercent);
                    }
                }
                #endregion

                #region TortoiseBreaker
                if (DBSpell == null || DBSpell.Passive)
                {
                    byte TortoiseBreakerL = 0;
                    int decrease = 0;

                    if (player.Owner.Rune.IsEquipped("TortoiseBreaker", ref TortoiseBreakerL) ||
                        player.Owner.Rune.IsEquipped("Tortoise Predator", ref TortoiseBreakerL))
                    {
                        switch (TortoiseBreakerL)
                        {
                            case 1: decrease = 134000; break;
                            case 2: decrease = 133500; break;
                            case 3: decrease = 133000; break;
                            case 4: decrease = 132500; break;
                            case 5: decrease = 132000; break;
                            case 6: decrease = 131500; break;
                            case 7: decrease = 131000; break;
                            case 8: decrease = 130500; break;
                            case 9: decrease = 130000; break;
                        }
                    }

                    Damage += decrease / 3;
                }
                #endregion

                // ==================================================
                // عدّل الرقم ده فقط للتحكم في قوة الضربة
                // 100 = عادي
                // 150 = زيادة 50%
                // 200 = ضعف الدمج
                // 250 = 2.5 ضعف
                // 300 = 3 أضعاف
                // ==================================================
                int TortoiseDamageRate = 100; // [CombatAudit 2026-10-14] was custom 150 (+50% physical damage vs any target that has Tortoise gems); 100 = official (gem only reduces damage)

                Damage -= (int)(Damage * TortoisePercent) / 100;

                Damage = Damage * TortoiseDamageRate / 100;
            }
            #endregion

            Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);

            #region UniversalShield
            byte UniversalShieldL = 0;
            uint FinalPD = 0;
            if ((target.Owner.Rune.IsEquipped("UniversalShield", ref UniversalShieldL) || target.Owner.Rune.IsEquipped("Indestructible Balance", ref UniversalShieldL)) && target.Owner.Status.PhysicalDamageDecrease < player.Owner.Status.PhysicalDamageIncrease)
            {
                if (player.BattlePower < target.BattlePower && !player.ContainFlag(MsgUpdate.Flags.DeathSigh))
                {
                    switch (UniversalShieldL)
                    {
                        case 1: FinalPD = 5000; break;
                        case 2: FinalPD = 6000; break;
                        case 3: FinalPD = 6500; break;
                        case 4: FinalPD = 7000; break;
                        case 5: FinalPD = 7500; break;
                        case 6: FinalPD = 8000; break;
                        case 7: FinalPD = 8500; break;
                        case 8: FinalPD = 9000; break;
                        case 9: FinalPD = 10000; break;
                    }
                }
                else if (player.BattlePower > target.BattlePower)
                {
                    switch (UniversalShieldL)
                    {
                        case 1: FinalPD = 5000; break;
                        case 2: FinalPD = 5000; break;
                        case 3: FinalPD = 7000; break;
                        case 4: FinalPD = 8000; break;
                        case 5: FinalPD = 9000; break;
                        case 6: FinalPD = 10000; break;
                        case 7: FinalPD = 11000; break;
                        case 8: FinalPD = 12000; break;
                        case 9: FinalPD = 15000; break;
                    }
                }
                uint FinalPDM = (player.Owner.Status.PhysicalDamageIncrease - target.Owner.Status.PhysicalDamageDecrease);
                if (FinalPDM > FinalPD)
                    FinalPDM = FinalPD;
                Damage -= (int)FinalPDM;
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.UniversalShieldBarrier;

            }
            #endregion

            #region Slayer
            if (player.ContainFlag(MsgUpdate.Flags.Slayer) && player.Owner.Rune.IsEquipped("Slayer"))
            {
                if (DBSpell != null)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.Windstorm || DBSpell.ID == (ushort)Role.Flags.SpellID.BloomofDeath)
                    {

                        Damage += Base.MulDiv((int)Damage, 30, 100);
                    }
                    else
                    {
                        uint DBSpellDG = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.DBSpell.GetDBSpellDG(player, DBSpell, IncreaseAttack);
                        #region Slayer
                        if (player.ContainFlag(MsgUpdate.Flags.Slayer) && player.Owner.Rune.IsEquipped("Slayer"))
                        {
                            if (DBSpellDG < 110)
                            {
                                Damage += Base.MulDiv((int)Damage, 10, 100);
                            }
                        }
                        #endregion

                    }
                }
            }
            #endregion
            SpellObj.Damage = (uint)Math.Max(1, Damage);

            #region Judgment
            byte itemLevell = 0;
            byte chanceIgnoreAzure5 = 0;
            if (player.Owner.Rune.IsEquipped("Judgment", ref itemLevell))
            {
                chanceIgnoreAzure5 = (byte)(10 + (itemLevell * 5));
                if (itemLevell == 9) chanceIgnoreAzure5 = 60;
            }
            #endregion

            #region Crack
            bool Crack = false;
            if (player.Owner.Status.Crack > 0)
            {
                Database.MythSoulAttributes.Attribute MythInfo;
                if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Crack].TryGetValue(player.Owner.Status.Crack, out MythInfo))
                {
                    double IncRate = 0;
                    Database.MythSoulAttributes.Attribute SuperPower;
                    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Superpower].TryGetValue(player.Owner.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    Database.MythSoulAttributes.Attribute Oracle;
                    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Oracle].TryGetValue(player.Owner.Status.Oracle, out Oracle))
                        {
                            IncRatee = (double)Oracle.Damage / 100;
                        }
                    }
                    if (Calculate.Base.Rate((int)MythInfo.Rate + (int)IncRate - (int)IncRatee))
                    {
                        Crack = true;
                    }
                }
            }
            #endregion

            #region AzureShield
            bool Fusing = false;
            if (player.Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fusing) && player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Fusing))
            {
                var Fusings = Pool.Magic[(ushort)Role.Flags.SpellIDPirate.Fusing][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellIDPirate.Fusing].Level];
                if (Calculate.Base.Rate(Fusings.GDamage / 100))
                {
                    Fusing = true;
                }
            }
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FlameofDestruction)
                {
                    Fusing = true;
                }

            }

            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (!Fusing)
                {
                    if (!Crack)
                    {
                        if (!Calculate.Base.Rate(chanceIgnoreAzure5))
                        {

                            #region AzureShieldDefence

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
                            #endregion

                        }
                        else
                        {
                            SpellObj.Effect |= MsgAttackPacket.AttackEffect.BreakDown;
                        }
                    }
                }
                else
                {
                    SpellObj.Effect = MsgAttackPacket.AttackEffect.CrackMyth;
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

            if (!player.ContainFlag(MsgUpdate.Flags.DisableBlock))
            {
                if (Base.Rate((byte)Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;

                }
            }

            #region MyClones
            if (target.MyClones.Count != 0)
            {
                foreach (var clone in target.MyClones.GetValues())
                    clone.RemoveThat(target.Owner);

                target.MyClones.Clear();
            }
            #endregion

            #region Infinity
            if (player.ContainFlag(MsgUpdate.Flags.Infinity) && player.Owner.Rune.IsEquipped("Infinity"))
            {
                uint InfinityDamage = (uint)(SpellObj.Damage * player.InfinityDamage / 100);
                SpellObj.Damage = InfinityDamage;
            }
            #endregion

            #region Assassin
            if (player.Owner.Rune.IsEquipped("Assassin"))
            {
                if (DBSpell == null || DBSpell.Passive)
                {
                    if (target.Action == Role.Flags.ConquerAction.Sit || target.OnAutoHunt)
                    {
                        SpellObj.Damage += 30000;
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.Assasssin;
                    }

                }

            }
            #endregion

            #region WideSwipe/Sharpness(Rune Skill)
            byte runeExtraAttack = 0;
            var weapon1 = player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon);
            if (weapon1 != null && ((Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("WideSwipe", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Sharpness", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Cosmic Balance", ref runeExtraAttack))))
            {
                if (runeExtraAttack == 9) runeExtraAttack = 10;

                SpellObj.Damage += (uint)((double)SpellObj.Damage * (double)runeExtraAttack / 100d);
            }
            #endregion

            if (onbreak)
            {
                #region Barrier&&TacitStrike
                byte Level = 0;
                if (target.Owner.Rune.IsEquipped("Barrier", ref Level))
                {
                    int sub = (int)((2500 + Level) * 500);
                    if (Level == 5) sub = 6000;
                    else if (Level == 6) sub = 7000;
                    else if (Level == 7) sub = 9000;
                    else if (Level == 8) sub = 11000;
                    else if (Level == 9) sub = 20000;
                    Level = 0;
                    if (player.Owner.Rune.IsEquipped("TacitStrike", ref Level))
                    {
                        int add = (int)((2500 + Level) * 500);
                        if (Level == 5) add = 6000;
                        else if (Level == 6) add = 7000;
                        else if (Level == 7) add = 9000;
                        else if (Level == 8) add = 11000;
                        else if (Level == 9) add = 15000;
                        if (sub >= add)
                            sub -= add;
                        else sub = 0;
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.TacitStrike;
                    }
                    else
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.UniversalShieldBarrier;
                    }
                    if (SpellObj.Damage >= sub)
                        SpellObj.Damage -= (uint)sub;
                }
                #endregion
            }

            #region NoMercy(Rune Skill)
            byte itemLevel6 = 0;
            if (SpellObj.Damage < 22000 && player.Owner.Rune.IsEquipped("NoMercy", ref itemLevel6))
            {
                uint addDef = 0;
                addDef = (uint)(4500 + itemLevel6 * 500);
                if (itemLevel6 == 9) addDef = 10000;
                else if (itemLevel6 == 8) addDef = 9000;

                SpellObj.Damage += addDef;
                if (SpellObj.Damage > 35000)
                    SpellObj.Damage = 35000;
            }
            #endregion

            #region BloodFeast
            if (Pool.Constants.FreePkMap.Contains(player.Map))
            {
                byte BloodFeastL = 0;
                if (player.Owner.Rune.IsEquipped("BloodFeast", ref BloodFeastL))
                {
                    int points = (int)((BloodFeastL + 1) * 5000);
                    if (Calculate.Base.Rate(20))
                    {
                        player.HitPoints += points;
                        if (player.HitPoints > player.Owner.AjustMaxHitpoints())
                            player.HitPoints = (int)player.Owner.AjustMaxHitpoints();
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.SendString(stream, (MsgStringPacket.StringID)30, true, "hxdf_hf");
                        }
                    }
                }
            }
            #endregion

            #region Adamant
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound || DBSpell.ID == (ushort)Role.Flags.SpellID.RapidFire)
                {
                    byte AdamantL = 0;
                    byte Rate = 0;
                    if (target.Owner.Rune.IsEquipped("Adamant", ref AdamantL))
                    {
                        switch (AdamantL)
                        {
                            case 1: Rate = 50; break;
                            case 2: Rate = 55; break;
                            case 3: Rate = 60; break;
                            case 4: Rate = 65; break;
                            case 5: Rate = 70; break;
                            case 6: Rate = 75; break;
                            case 7: Rate = 80; break;
                            case 8: Rate = 90; break;
                            case 9: Rate = 100; break;
                        }
                        if (Base.Rate(Rate))
                        {
                            uint decreaseDamage = SpellObj.Damage * 30 / 100;
                            if (SpellObj.Damage > decreaseDamage)
                                SpellObj.Damage -= decreaseDamage;
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

            #region Reflect
            if (CheckAttack.BlockRefect.CanUseReflect(player.Owner))
            {
                if (!StackOver)
                {
                    MsgSpellAnimation.SpellObj InRedirect;
                    if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                        SpellObj = InRedirect;
                }
            }
            #endregion



            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.HookMoonCaster))
                SpellObj.Damage = (uint)(SpellObj.Damage * player.Owner.Player.HookMoonPower / 100);

            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.SuanNiHeart))
            {
                if (DBSpell == null || DBSpell.Passive || player.RandomSpell > 0)
                {

                    Database.MagicType.Magic SuanniHeart = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniHeart][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level];
                    SpellObj.Damage += (uint)(SpellObj.Damage * SuanniHeart.Damage2) / 100;
                }
                if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.HookMoonCaster))
                    SpellObj.Damage = (uint)(SpellObj.Damage * player.Owner.Player.HookMoonPower / 100);
            }

            #region Redcurse
            if (player.Owner.MyArchives.isOpen(Role.Instance.Archives.TypeID.Redcurse) && player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PowerDash))
            {
                if (DBSpell != null)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.Pounce)
                    {
                        Database.MagicType.Magic PowerDash = Pool.Magic[(ushort)Role.Flags.SpellID.PowerDash][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.PowerDash].Level];
                        SpellObj.Damage += (uint)((SpellObj.Damage * PowerDash.DamageOnHuman) / 100);

                        bool CanAttack = player.Owner.Equipment.SteedPlus > target.Owner.Equipment.SteedPlus;
                        if (player.Owner.Equipment.SteedPlus == target.Owner.Equipment.SteedPlus)
                            CanAttack = player.Owner.Equipment.SteedPlusPorgres > target.Owner.Equipment.SteedPlusPorgres;

                        if (CanAttack)
                        {
                            target.Owner.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        }
                    }
                }
            }
            #endregion
            MsgAttackPacket.HitInMele(player, DBSpell);

        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, int IncreaseAttack = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            bool update = false;

            bool Effect = false;

            bool BonePlus = false;

            bool DeathSigh = false;

            #region Calculator(Attack)
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            #endregion

            int IncDamagee = 0;


            if (player.ContainFlag(MsgUpdate.Flags.Superman))
                SpellObj.Damage *= 2;
            SpellObj.Damage = (uint)Math.Max(1, Damage);



            #region Breakdown
            byte BreakdownL = 0;
            if (player.Owner.Rune.IsEquipped("Breakdown", ref BreakdownL) || player.Owner.Rune.IsEquipped("Battle Reaper", ref BreakdownL))
            {
                IncDamagee += (int)(110 + BreakdownL * 10);
                if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage = (uint)(SpellObj.Damage * 10) / 100;
            }

            #endregion

            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
            {
                SpellObj.Damage = (uint)((double)SpellObj.Damage * 75 / 100d);

            }
            if (player.ContainFlag(MsgUpdate.Flags.Superman))
                SpellObj.Damage *= 11;
            SpellObj.Damage /= 2;

            //#region TenacityPillar
            //if (player.Map == 1038 && Game.MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead)
            //{
            //    if (player.GuildID == Game.MsgTournaments.MsgSchedules.GuildWar.NPC_INFO[Qahr.Mobile.MsgGuildWar.NPCID.TenacityPillar].Npc.GuildID)
            //    {
            //        IncDamagee += 200;
            //    }

            //}
            //#endregion

            #region DeathSigh
            if (player.ContainFlag(MsgUpdate.Flags.DeathSigh))
            {
                if (player.ContainFlag(MsgUpdate.Flags.DeathSigh))
                {
                    if (DBSpell == null || DBSpell.Passive)
                    {
                        uint DBSpellDG = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.DBSpell.GetDBSpellDG(player, DBSpell, IncreaseAttack);
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpellDG + player.DeathSighPassive), 100);
                        DeathSigh = true;
                    }
                    else
                    {
                        uint DBSpellDG = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.DBSpell.GetDBSpellDG(player, DBSpell, IncreaseAttack);
                        Damage = Base.MulDiv((int)Damage, (int)(DBSpellDG + player.DeathSighActive), 100);
                        DeathSigh = true;
                    }
                }
                #endregion

            }

            #region Edit Damage
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarChainFire)
                {
                    Damage = Base.MulDiv((int)Damage, 350, 100);
                    update = true;
                }

            }
            #endregion

            #region DBSpell(Edit)
            if (DBSpell != null && !DeathSigh)
            {
                //#region BonePluse
                //using (var rec = new ServerSockets.RecycledPacket())
                //{
                //    var stream = rec.GetStream();
                //    MsgSpell Owner_spell = null;
                //    if (DBSpell != null)
                //    {
                //        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                //                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                //                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                //                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                //                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                //        {
                //            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                //            {
                //                if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                //                {
                //                    if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.BonePulse, out Owner_spell))
                //                    {

                //                        Database.MagicType.Magic BonePulse = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                //                        int IncDg = BonePulse.Damage2 % 1000;
                //                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                //                            || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                //                            || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive)
                //                        {
                //                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000) , 100);
                //                            IncDg -= DBSpell.Damage2 % 1000;
                //                        }


                //                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                //                            || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                //                        {
                //                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000), 100);
                //                            IncDg -= DBSpell.DamageOnHuman % 1000;
                //                        }

                //                        Damage += (int)(Damage * (IncDg) / 100);

                //                        BonePlus = true;
                //                    }
                //                }
                //            }
                //        }
                //    }

                //}
                //#endregion
                if (!BonePlus)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackBloodlust
                       || DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackRedcurse)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage * 3 + IncDamagee, 100);
                        update = true;
                    }

                    #region Edit Damage(Index)[10]
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleBlasts)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage / 2 + IncDamagee, 100);
                        update = true;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeTempest)
                    {
                        MsgGameItem RightWeapon;
                        MsgGameItem LeftWeapon;
                        if (player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon)
                            && player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                        {
                            if (Database.ItemType.IsPirateEpicWeapon(RightWeapon.ITEM_ID) && Database.ItemType.IsPirateEpicWeapon(LeftWeapon.ITEM_ID))
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                                update = true;
                            }
                        }

                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.SwirlingStorm)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage / 4 + IncDamagee, 100);
                        update = true;
                    }
                    #endregion

                    #region DamageOnHuman(Index)[35]
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderStrike
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.CleanSweep
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SongofPhoenix
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.AxeShadow
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DeadlyStrike
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SacredBlessing
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DeathSigh
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Celestial
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Roamer
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWind
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachment
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.BloodyScythe
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WildDashAttack
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.ArrowBlades
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.RevengeAttack
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.Megabolt
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.SkyFall
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderBlast
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows
                         || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Drukyle
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolCano
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Spitfire
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SandMist
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.CaptiveArrow
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SpitfirePassive
                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive
                                         || DBSpell.ID == (ushort)Role.Flags.SpellID.DragonTransformationPassive

                         || DBSpell.ID == (ushort)Role.Flags.SpellID.SuanniRoar
                               || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.DrukylePassive
                                || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSea
                           || DBSpell.ID == (ushort)Role.Flags.SpellID.DragonRising
                     )
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                        update = true;
                    }

                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarFlow)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                        update = true;
                    }



                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.ElementalArrow)
                    {
                        uint DamageInc = 0;
                        Database.MagicType.Magic ElementalArrow = Pool.Magic[(ushort)Role.Flags.SpellID.ElementalArrow][(ushort)player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ElementalArrow].Level];
                        if (player.ContainFlag(MsgUpdate.Flags.FireArrow)
                               || player.ContainFlag(MsgUpdate.Flags.IceArrow)
                               || player.ContainFlag(MsgUpdate.Flags.PoisonArrow)
                               || player.ContainFlag(MsgUpdate.Flags.ThunderArrow)
                               || player.ContainFlag(MsgUpdate.Flags.WindArrow))
                        {
                            if (player.ContainFlag(MsgUpdate.Flags.FireArrow))
                            {
                                player.RemoveFlag(MsgUpdate.Flags.FireArrow);
                                DamageInc += 20;
                            }
                            if (player.ContainFlag(MsgUpdate.Flags.IceArrow))
                            {
                                player.RemoveFlag(MsgUpdate.Flags.IceArrow);
                                DamageInc += 20;
                            }
                            if (player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                            {
                                player.RemoveFlag(MsgUpdate.Flags.PoisonArrow);
                                DamageInc += 20;
                            }
                            if (player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                            {
                                player.RemoveFlag(MsgUpdate.Flags.ThunderArrow);
                                DamageInc += 20;
                            }
                            if (player.ContainFlag(MsgUpdate.Flags.WindArrow))
                            {
                                player.RemoveFlag(MsgUpdate.Flags.WindArrow);
                                DamageInc += 20;
                            }


                        }
                        Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + DamageInc) + IncDamagee, 100);
                        update = true;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireArrow)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 100) + IncDamagee, 100);
                        update = true;


                    }

                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackShot)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + DBSpell.Damage3 % 1000 + IncDamagee, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 1)
                    {
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                            update = true;
                        }
                        
                        
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                            update = true;
                        }
                    }
                    if (IncreaseAttack == 0)
                    {
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.DamageOnHuman % 1000) + DBSpell.Damage3) + IncDamagee, 100);
                            update = true;
                        }
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 100 + IncDamagee, 100);
                        update = true;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonCyclone)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 100 + IncDamagee, 100);
                        update = true;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)50 + IncDamagee, 100);
                        update = true;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                        update = true;
                    }
                    #endregion

                    #region Damage2(Index)[36]
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireball
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlash
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrison
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.BonePulse
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.Ironbone
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.IronbonePassive
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeak
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAge
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGun
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGunPassive
            || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanction
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanctionPassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive
                 || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAgePassive)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 + IncDamagee, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 1)
                    {
                        //if (DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                        //{
                        //    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 + IncDamagee, 100);
                        //    update = true;
                        //}
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpear)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + DBSpell.DamageOnHuman) + IncDamagee, 100);
                        update = true;
                    }
                    #endregion

                    #region Damage3(Index)[37]
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterShockwave
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.HookMoon
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.HellVortex)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 + IncDamagee, 100);
                        update = true;

                    }

                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.TwilightDance)
                    {
                        if (IncreaseAttack == 1)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 + IncDamagee, 100);
                            update = true;
                        }
                        if (IncreaseAttack == 2)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage % 1000 + IncDamagee, 100);
                            update = true;
                        }
                        if (IncreaseAttack == 3)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage2 % 1000 + IncDamagee, 100);
                            update = true;
                        }


                    }
                    #endregion

                    #region DamageOnMonster(Index)[44]
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK1
                       || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK1
                       || DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK2
                       || DBSpell.ID == (ushort)Role.Flags.SpellID.WarSuit401NormalATK2
                       || DBSpell.ID == (ushort)Role.Flags.SpellID.NormalATK3
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingShock
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WindstormBattleaxe
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage
                        || DBSpell.ID == (ushort)Role.Flags.SpellID.CannonBarrage
                         || DBSpell.ID == (ushort)Role.Flags.SpellID.Wildwind || DBSpell.ID == (ushort)Role.Flags.SpellID.HeavensWrath)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 + IncDamagee, 100);
                        update = true;
                    }
                    if (IncreaseAttack == 1)
                    {
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 + IncDamagee, 100);
                            update = true;
                        }
                    }


                    #endregion
                }
            }
            #endregion

            #region MTD
            if (!update && !BonePlus && !DeathSigh)
            {
                Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? (uint)DBSpell.Damage + IncDamagee : Program.ServerConfig.PhysicalDamage + IncDamagee), 100);
            }
            #endregion

            #region CriticalStrike
            if (Role.Core.Rate((int)(player.Owner.Status.CriticalStrike / 100) / 3))
            {
                if (player.Owner.AjustCriticalStrike() > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    Damage = Base.MulDiv((int)Damage, (int)(170), 100);
                    Effect = true;
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
            //    Damage = Base.MulDiv((int)Damage, 50, 100);
            //}
            //#endregion

            #region DashRate
            if (player.Owner.Status.DashRate > 0 && !Effect)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 200, 100);
                }

            }
            #endregion

            SpellObj.Damage = (uint)Math.Max(1, Damage);



            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);

            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.SuanNiHeart))
            {
                if (DBSpell == null || DBSpell.Passive || player.RandomSpell > 0)
                {

                    Database.MagicType.Magic SuanniHeart = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniHeart][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level];
                    SpellObj.Damage += (uint)(SpellObj.Damage * SuanniHeart.Damage2) / 100;
                }
                if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.HookMoonCaster))
                    SpellObj.Damage = (uint)(SpellObj.Damage * player.Owner.Player.HookMoonPower / 100);
            }

            MsgAttackPacket.HitInMele(player, DBSpell);

        }
    }
}

