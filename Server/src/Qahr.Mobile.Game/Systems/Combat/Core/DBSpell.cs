using Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate
{
    public class DBSpell
    {
        public static uint GetDBSpellDG(Role.Player player, Database.MagicType.Magic DBSpell, int IncreaseAttack = 0)
        {

            #region Edit Damage(MT)
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackBloodlust
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackRedcurse)
                {
                    return (uint)DBSpell.Damage * 3;
                }
                #region Edit Damage(Index)[10]

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeTempest)
                {
                    MsgGameItem RightWeapon;
                    MsgGameItem LeftWeapon;
                    if (player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon)
                        && player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                    {
                        if (Database.ItemType.IsPirateEpicWeapon(RightWeapon.ITEM_ID) && Database.ItemType.IsPirateEpicWeapon(LeftWeapon.ITEM_ID))
                        {
                            return (uint)DBSpell.DamageOnHuman % 1000;
                        }
                    }

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SwirlingStorm)
                {
                    return (uint)DBSpell.Damage / 4;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleBlasts)
                {
                    return (uint)DBSpell.Damage / 2;

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
                    return (uint)DBSpell.DamageOnHuman % 1000;
                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarFlow)
                {
                    return (uint)DBSpell.DamageOnHuman % 1000;
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
                    return (uint)(DBSpell.Damage2 % 1000 + DamageInc);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireArrow)
                {
                    return (uint)(uint)(DBSpell.DamageOnHuman % 100);


                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackShot)
                {
                    return (uint)(DBSpell.DamageOnHuman % 1000 + DBSpell.Damage3 % 1000);
                }
                if (IncreaseAttack == 1)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                    {
                        return (uint)DBSpell.DamageOnHuman % 1000;
                    }
                   
                    if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                    {
                        return (uint)DBSpell.DamageOnHuman % 1000;
                    }
                }
                if (IncreaseAttack == 0)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                    {
                        return (uint)((DBSpell.DamageOnHuman % 1000) + DBSpell.Damage3);
                    }
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike)
                {
                    return (uint)DBSpell.DamageOnHuman % 100;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonCyclone)
                {
                    return (uint)DBSpell.DamageOnHuman % 100;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage)
                {
                    return (uint)50;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                     || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                {
                    return (uint)DBSpell.DamageOnHuman % 1000;
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
                    return (uint)DBSpell.Damage2 % 1000;
                }
                if (IncreaseAttack == 1)
                {
                    //if (DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                    //{
                    //    return (uint)DBSpell.Damage2 % 1000;
                    //}
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpear)
                {
                    return (uint)(DBSpell.Damage2 % 1000 + DBSpell.DamageOnHuman);
                }
                #endregion

                #region Damage3(Index)[37]
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterShockwave
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.HookMoon
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.HellVortex)
                {
                    return (uint)DBSpell.Damage3 % 1000;

                }

                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TwilightDance)
                {
                    if (IncreaseAttack == 1)
                    {
                        return (uint)DBSpell.Damage3 % 1000;
                    }
                    if (IncreaseAttack == 2)
                    {
                        return (uint)DBSpell.Damage3 % 1000;
                    }
                    if (IncreaseAttack == 3)
                    {
                        return (uint)DBSpell.NewDamage2 % 1000;
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
                    || DBSpell.ID == (ushort)Role.Flags.SpellID.CannonBarrage || DBSpell.ID == (ushort)Role.Flags.SpellID.HeavensWrath)
                {
                    return (uint)DBSpell.DamageOnMonster % 1000;

                }
                if (IncreaseAttack == 1)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                    {
                        return (uint)DBSpell.DamageOnMonster % 1000;
                    }
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.RageofWar)
                    {
                        return (uint)DBSpell.DamageOnMonster % 1000;
                    }
                }


                #endregion
            }
            #region Damage(Index)[10]
            if (IncreaseAttack == 0)
            {
                return (uint)(int)((DBSpell != null && DBSpell.Damage > 0) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage);
            }
            #endregion
            #endregion

            return 0;
        }
    }
}
