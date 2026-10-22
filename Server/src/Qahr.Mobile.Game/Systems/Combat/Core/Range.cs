using Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate
{
    public class Range
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0, int IncreaseAttack = 0)
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
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);
            #endregion

            #region Defense(Monster)
            var rawDefense = monster.Family.Defense;

            Damage = Math.Max(0, Damage - rawDefense);

            #endregion

            #region MTD
            bool Effect = false;
            if (MultipleDamage != 0)
            {
                Damage = Damage * MultipleDamage;
            }
            else
            {
                int IncDamagee = 0;
                if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
                {
                    IncDamagee += 250;
                }
                #region BonePluse
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgSpell Owner_spell = null;
                    if (DBSpell != null)
                    {
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                                        || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive
                                        || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                                        || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                        {
                            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                            {
                                if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                                {
                                    if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.BonePulse, out Owner_spell))
                                    {

                                        Database.MagicType.Magic BonePulse = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                                        int IncDg = BonePulse.GDamage % 1000;
                                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive
                                            || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive
                                            || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive)
                                        {
                                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000, 100);
                                            IncDg -= DBSpell.GDamage % 1000;
                                        }


                                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive
                                            || DBSpell.ID == (ushort)Role.Flags.SpellID.SickleWindPassive)
                                        {
                                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000, 100);
                                            IncDg -= DBSpell.GDamage % 1000;
                                        }


                                        Damage += (int)(Damage * (IncDg) / 100);
                                        Effect = true;
                                    }
                                }
                            }
                        }
                    }
                    if (player.ContainFlag(MsgUpdate.Flags.DeathSigh))
                    {


                        Damage += (int)(Damage * player.DeathSighActive / 100);
                        Effect = true;

                    }
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
                if (DBSpell != null)
                {
                    #region DBSpell(Edit)
                    if (DBSpell != null)
                    {
                        #region ThunderStrike
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingShock)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderBlast)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Megabolt)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WindstormBattleaxe)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.HeavensWrath)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        #endregion

                        #region WindWalker
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleBlasts)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ShadowofChaser)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SwirlingStorm)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BurntFrost)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.AngerofStomper)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.HorrorofStomper)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.PeaceofStomper)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        #endregion

                        #region Monk
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.RadiantPalm)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlwindKick)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.InfernalEcho)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WrathoftheEmperor)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                            Effect = true;
                        }

                        #endregion

                        #region Archer
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ArrowBlades)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScatterFire)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.RevengeAttack)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackShot)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ElementalArrow)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        #endregion

                        #region Trojan
                        if (IncreaseAttack == 1)
                        {
                            

                            
                            if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee + DBSpell.Damage2 % 1000), 100);
                                Effect = true;
                            }
                        }
                        if (IncreaseAttack == 0)
                        {
                            
                            
                            if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.Damage2 % 1000), 100);
                                Effect = true;
                            }
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.FastBlader)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScrenSword)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Hercules)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Celestial)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Roamer)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Penetration)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Phoenix)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Rage)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.AxeShadow)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.CleanSweep)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SongofPhoenix)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.HookMoon)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.Damage3 % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderStrike)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DeadlyStrike)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SacredBlessing)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DeathSigh)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }                       
                        #endregion

                        #region Warrior
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaveofBlood)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Pounce)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChargingVortex)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.Ironbone)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.IronbonePassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackDragonhowl)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackBloodlust)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildDashAttack)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackRedcurse)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                            Effect = true;
                        }
                        #endregion

                        #region Pirate
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeTempest)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.EagleEye)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.GaleBomb)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        //if (IncreaseAttack == 0 && DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                        //{
                        //    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        //    Effect = true;
                        //}
                        //if (IncreaseAttack == 1 && DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                        //{
                        //    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + IncDamagee), 100);
                        //    Effect = true;
                        //}
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BloomofDeath)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Drukyle || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.DrukylePassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SandMist)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.CaptiveArrow)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanction || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanctionPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpear || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeak || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGun || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGunPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAge || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAgePassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSea || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolCano || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Spitfire || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SpitfirePassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.CannonBarrage)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BlackbeardsRage)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.NeedLevel % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        #endregion

                        #region Ninja
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.TwilightDance)
                        {
                            if (IncreaseAttack == 1)
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage3 % 1000 + IncDamagee, 100);
                                Effect = true;
                            }
                            if (IncreaseAttack == 2)
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage % 1000 + IncDamagee, 100);
                                Effect = true;
                            }
                            if (IncreaseAttack == 3)
                            {
                                Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.NewDamage2 % 1000 + IncDamagee, 100);
                                Effect = true;
                            }
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.RageofWar)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.BloodyScythe)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;

                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.FatalSpin)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireball || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrison || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachment || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlash || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.ShurikenVortex)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        #endregion

                        #region DW DragonWarrior
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingSwipe)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SplittingSwipe)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.AirKick)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SpeedKick)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonTransformationPassive)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuanniRoar)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonRising)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack1 ||
                            DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack2 ||
                            DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack3)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonCyclone)
                        {
                            Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Rate % 1000 + IncDamagee, 100);
                            Effect = true;
                        }
                        #endregion
                    }
                    #endregion
                }
                #region MonsterHunter
                if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MonsterHunter))
                {
                    var MonsterHunter = Pool.Magic[(ushort)Role.Flags.SpellID.MonsterHunter][player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.MonsterHunter].Level];
                    IncDamagee += MonsterHunter.GDamage % 1000;
                }
                #endregion
                if (!Effect)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(Base.MonsterPower(DBSpell) + IncDamagee), 100); // [CombatAudit 2026-10-14] uncapped official power vs monsters
                }

            }
            #endregion

            #region Damage(INC)
            Damage += player.getFan(false);

            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);

            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);

            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, Client.GameClient.DefaultDefense2);

            if (monster.Family.Defense2 > 0)
                Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);
            #endregion

            SpellObj.Damage = (uint)Math.Max(1, Damage);

            bool update = false;

            #region CriticalStrike
            if (Role.Core.Rate((int)(player.Owner.Status.CriticalStrike / 100) / 3))
            {
                if (player.Owner.AjustCriticalStrike() / 100 > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = (uint)(SpellObj.Damage * 1.5);
                    update = true;
                }
            }
            #endregion

            //#region Lucky Strike [X2]
            //if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
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
            //    SpellObj.Effect = MsgAttackPacket.AttackEffect.LuckyStrike;
            //    SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);

            //}
            //#endregion

            #region DashRate
            if (player.Owner.Status.DashRate > 0 && !update)
            {
                if (Base.Rate((int)player.Owner.Status.DashRate / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 125, 100);
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

            #region Destroy

            if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit) || player.Owner.MyArchives.isOpen(Archives.TypeID.Dragonhowl) && player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit) && (SpellObj.Effect & MsgAttackPacket.AttackEffect.CriticalStrike) == MsgAttackPacket.AttackEffect.CriticalStrike)
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.DestroyWarrior;
            }
            #endregion

            #region Monster(Guard)
            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            #endregion

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
                    SpellObj.Damage *= 10;
            }
            if (monster.Boss == 1)
            {
                if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                    SpellObj.Damage *= 3;
            }
            #endregion

            #region SuanNiHeart
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
            #endregion

            #region NewUpdate(Reducation) && DetiyLand
            if (monster.Name == "QueenofEvil" || monster.Name == "NetherTyrant"
                || monster.Name == "BloodyBanshee"
                || monster.Name == "ChillingSpook"
                || monster.Name == "DragonWraith")
                //SpellObj.Damage -= (SpellObj.Damage * 80) / 100;
            #endregion

                #region Monster(Damage)//1
                if (monster.Family.Defense2 == 0)
                    SpellObj.Damage = 1;
            if (monster.Family.ID == 5385)
                SpellObj.Damage = 0;
            if (monster.Family.ID == 7562)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 2699)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 2700)
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


            if (monster.Family.ID == 3636)
                SpellObj.Damage = 1;
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
            MsgAttackPacket.HitInMele(player, DBSpell);
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, int increasedmg = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            #region ShurikenVortex
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                SpellObj.Damage = 1;
                return;
            }
            #endregion

            #region DragonSwing
            if (target.ContainFlag(MsgUpdate.Flags.DragonSwing))
            {
                byte additionalRate = 0;
                if (target.Owner.Rune.IsEquipped("SwingingTail") && DBSpell != null && (DBSpell.ID == (ushort)Role.Flags.SpellID.EagleEye || DBSpell.ID == (ushort)Role.Flags.SpellID.BlackbeardsRage || DBSpell.ID == (ushort)Role.Flags.SpellID.KineticSpark || DBSpell.ID == (ushort)Role.Flags.SpellID.DaggerStorm || DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave || DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound))//SwingingTail(Dragon Warrior Rune)
                    additionalRate += 30;
                if (Role.Core.Rate(target.DragonSwingChance + additionalRate))
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
            if (player.ContainFlag(MsgUpdate.Flags.FreezingPelter) && Role.Core.Rate(30 + (int)(player.BattlePower - (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DeathSigh) && target.BattlePower > player.BattlePower ? player.BattlePower : target.BattlePower) * 5)))
            {
                target.AddFlag(MsgUpdate.Flags.xFreezingPelter, 10, true);
            }
            #endregion

            bool update = false;
            MsgYuanshen.SwordBody(target.Owner, SpellObj);
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

            #region MaxAttack
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);


            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);
            #endregion

            #region AjustDefense
            var rawDefense = target.Owner.AjustDefense;
            if (Damage > rawDefense)
                Damage -= (int)rawDefense;
            else
                Damage = 1;
            #endregion

            #region Edit Damage(MT)
            if (DBSpell != null)
            {
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FireArrow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 100, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ElementalArrow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeFlurry)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 100, 100);
                    update = true;
                }
              
            }

            #endregion

            #region DBSpell(Damage)
            if (!update)
            {
                if (DBSpell != null)
                {
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);
                }
            }
            #endregion

            #region Tortoise Breaker - Tortoise Smasher - Tortoise Predator
            if (target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem) > 0)
            {
                int reduction = Base.MulDiv((int)target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem), 50, 100);
                byte torReduction = 0;
                if (player.Owner.Rune.IsEquipped("TortoiseBreaker", ref torReduction) || player.Owner.Rune.IsEquipped("Tortoise Predator", ref torReduction))
                    reduction = (int)((double)reduction * (double)torReduction / 20d);
                if (player.Owner.Rune.IsEquipped("TortoiseSmasher", ref torReduction) || player.Owner.Rune.IsEquipped("Tortoise Predator", ref torReduction))
                    reduction = (int)((double)reduction * (double)torReduction / 40d);
                Damage = Base.MulDiv((int)Damage, (int)(100 - Math.Min(67, reduction)), 100);
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
            if (player.Owner.AjustCriticalStrike() > 0)
            {
                uint WhirlSigilSage1 = 0;
                uint WhirlSigilSage2 = 0;
                if (DBSpell != null)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken)
                    {
                        Role.Instance.Ninja.Item item;
                        if (player.Owner.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.WhirlSigilSage, out item))
                        {
                            WhirlSigilSage1 = (uint)item.DBItem.Power;
                            WhirlSigilSage2 = (uint)item.DBItem.Damage;
                        }
                    }
                }
                if (Base.GetRefinery(player.Owner.AjustCriticalStrike() / 100 + WhirlSigilSage1, target.Owner.AjustImunity() / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    Damage = Base.MulDiv((int)Damage, (int)(150 + WhirlSigilSage2), 100);

                    update = true;
                    Strike = true;
                }
            }
            #endregion


            //#region Lucky Strike [X2]
            //if (!update && Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
            //{
            //    if (Base.Rate(target.Owner.PerfectionStatus.StrikeLock))
            //    {
            //        using (var rec = new ServerSockets.RecycledPacket())
            //        {
            //            var stream = rec.GetStream();
            //            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //            {
            //                Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
            //                Id = player.UID
            //            }), true);
            //        }
            //    }
            //    else
            //    {
            //        if (player.Owner.PerfectionStatus.LuckyStrike != target.Owner.PerfectionStatus.LuckyStrike)
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
            //                {
            //                    Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
            //                    Id = player.UID
            //                }), true);
            //            }
            //            SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
            //            Damage = Base.MulDiv((int)Damage, 200, 100);
            //            update = true;
            //        }
            //    }
            //}
            //#endregion

            #region Breakthrough
            if (!update && player.Owner.AjustBreakthrough() > 0)
            {
                //  if (Base.Rate((int)((uint)player.Owner.AjustBreakthrough() / 10 - target.Owner.AjustAntiBreack() / 10)))
                {
                    onbreak = true;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;

                }
            }
            #endregion

            #region DashRate
            if (!Strike)
            {
                if (Base.Rate((int)(player.Owner.Status.DashRate / 100)))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.DashRate;
                    Damage = Base.MulDiv((int)Damage, 180, 100);
                }
            }
            #endregion



            #region olddamage
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
                Damage = (int)Base.BigMulDiv((int)Damage, 6500, Client.GameClient.DefaultDefense2);
            #endregion

            #region Blessed
            Damage -= (int)(Damage * target.Owner.Status.ItemBless / 100);
            #endregion

            Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);

            SpellObj.Damage = (uint)Damage;

            #region Judgment(RuneSkill)
            byte itemLevell = 0;
            byte chanceIgnoreAzure5 = 0;
            if (player.Owner.Rune.IsEquipped("Judgment", ref itemLevell))
            {
                chanceIgnoreAzure5 = (byte)(10 + (itemLevell * 5));
                if (itemLevell == 9) chanceIgnoreAzure5 = 60;
            }
            #endregion

            #region Crack
            int RateCrack = 0;
            bool Crack = true;
            if (player.Owner.Status.Crack > 0)
            {
                Database.MythSoulAttributes.Attribute MythInfo;
                if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Crack].TryGetValue(player.Owner.Status.Crack, out MythInfo))
                {
                    double IncRate = 0;
                    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Superpower].TryGetValue(player.Owner.Status.Superpower, out MythInfo))
                        {
                            IncRate = (double)MythInfo.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    if (player.Owner.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.Oracle].TryGetValue(player.Owner.Status.Oracle, out MythInfo))
                        {
                            IncRatee = (double)MythInfo.Damage / 100;
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
            if (player.Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fusing))
            {
                MsgSpell ClientSpell;
                if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellIDPirate.Fusing, out ClientSpell))
                {
                    Database.MagicType.Magic Magic = Pool.Magic[ClientSpell.ID][ClientSpell.Level];
                    if (Role.Core.Rate(Magic.Rate / 100))
                    {
                        Fusing = true;
                    }
                }
            }
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (!Fusing)
                {
                    if (!Crack)
                    {
                        if (!Role.Core.Rate(chanceIgnoreAzure5))
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
                }
                else
                {
                    SpellObj.Effect = MsgAttackPacket.AttackEffect.CrackMyth;
                }
            }

            #endregion
            if (target.Owner.Equipment.ShieldID != 0)
            {
                int Block = (int)(target.Owner.Status.Block / 100);
                uint Change = (uint)Math.Min(70, Block / 2);

                if (target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                {
                    if (Change > 20)
                        Change -= 20;
                    else
                        Change = 0;
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
                byte itemLevel = 0;
                if (target.Owner.Rune.IsEquipped("Wonder", ref itemLevel) || target.Owner.Rune.IsEquipped("Supreme Armor", ref itemLevel))
                {
                    switch (itemLevel)
                    {
                        case 1: itemLevel = 10; break;
                        case 2: itemLevel = 13; break;
                        case 3: itemLevel = 15; break;
                        case 4: itemLevel = 16; break;
                        case 5: itemLevel = 18; break;
                        case 6: itemLevel = 20; break;
                        case 7: itemLevel = 22; break;
                        case 8: itemLevel = 25; break;
                        case 9: itemLevel = 30; break;
                    }
                    if (itemLevel > Change)
                        Change = itemLevel;
                }
                if (Base.Rate((byte)Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;
                }
            }



            //#region WhettedBlade
            //byte rate = 0;
            //byte WhettedBlade = 0;
            //if (player.Owner.Rune.IsEquipped("Whetted Blade", ref WhettedBlade))
            //{
            //    switch (WhettedBlade)
            //    {
            //        case 1: rate = 1; break;
            //        case 2: rate = 2; break;
            //        case 3: rate = 3; break;
            //        case 4: rate = 4; break;
            //        case 5: rate = 5; break;
            //        case 6: rate = 6; break;
            //        case 7: rate = 7; break;
            //        case 8: rate = 8; break;
            //        case 9: rate = 10; break;
            //    }
            //}
            //if (!Role.Core.Rate((byte)rate))
            //{
            //    if (Base.Rate((byte)Change))
            //    {
            //        SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
            //        SpellObj.Damage /= 2;

            //    }
            //}
            //#endregion

            #region DefensiveStance
            if (target.ContainFlag(MsgUpdate.Flags.DefensiveStance))
            {
                SpellObj.Damage = Calculate.Base.CalculateBless(SpellObj.Damage, 40);
                SpellObj.Effect = MsgAttackPacket.AttackEffect.Block;
                return;
            }
            #endregion

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

            #region Slayer
            if (player.ContainFlag(MsgUpdate.Flags.Slayer) && player.Owner.Rune.IsEquipped("Slayer"))
            {
                uint SlayerDamage = (uint)(SpellObj.Damage * (DBSpell == null ? player.SlayerNormalPercent : player.SlayerSkillPercent) / 100);
                SpellObj.Damage += SlayerDamage;

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
                    }
                    if (SpellObj.Damage >= sub)
                        SpellObj.Damage -= (uint)sub;
                }
                #endregion
            }
            #region UniversalShield
            byte UniversalShieldL = 0;
            uint FinalPD = 0;
            if (target.Owner.Rune.IsEquipped("UniversalShield", ref UniversalShieldL) || target.Owner.Rune.IsEquipped("Indestructible Balance", ref UniversalShieldL))
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
                if (SpellObj.Damage > FinalPD && SpellObj.Damage > player.Owner.Status.PhysicalDamageIncrease)
                {
                    if (player.Owner.Status.PhysicalDamageIncrease >= FinalPD)
                        SpellObj.Damage -= FinalPD;
                    else
                        SpellObj.Damage -= player.Owner.Status.PhysicalDamageIncrease;
                }
            }
            #endregion

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


            #region WideSwipe/Sharpness(Rune Skill)
            byte runeExtraAttack = 0;
            var weapon1 = player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon);
            if (weapon1 != null && ((Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("WideSwipe", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Sharpness", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Cosmic Balance", ref runeExtraAttack))))
            {
                if (runeExtraAttack == 9) runeExtraAttack = 10;

                SpellObj.Damage += (uint)((double)SpellObj.Damage * (double)runeExtraAttack / 100d);
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
            MsgAttackPacket.HitInMele(player, DBSpell);
        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, int IncreaseAttack = 0)
        {

            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            bool update = false;

            bool Breakdown = false;

            bool Strike = false;

            #region Calculator(Attack)
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
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
                #region ThunderStrike
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingShock)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderBlast)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Megabolt)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WindstormBattleaxe)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HeavensWrath)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderRampage)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                #endregion

                #region WindWalker
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleBlasts)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnMonster % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ShadowofChaser)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SwirlingStorm)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BurntFrost)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.AngerofStomper)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HorrorofStomper)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.PeaceofStomper)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage2 % 1000 + IncDamagee, 100);
                    update = true;
                }
                #endregion

                #region Monk
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.RadiantPalm)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlwindKick)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.InfernalEcho)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WrathoftheEmperor)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                    update = true;
                }

                #endregion

                #region Archer
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ArrowBlades)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BlisteringWave)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScatterFire)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.MortalWound)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.RevengeAttack)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackShot)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ElementalArrow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                #endregion

                #region Trojan
                if (IncreaseAttack == 1)
                {
                    

                    
                    if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee + DBSpell.Damage2 % 1000), 100);
                        update = true;
                    }
                }
                if (IncreaseAttack == 0)
                {
                    
                    if (DBSpell.ID == (ushort)Role.Flags.SpellIDDune.FinalStand)
                    {
                        Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.Damage2 % 1000), 100);
                        update = true;
                    }
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Hercules)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScrenSword)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FastBlader)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Celestial)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Roamer)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Penetration)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Phoenix)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Rage)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.AxeShadow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CleanSweep)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SongofPhoenix)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HookMoon)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.Damage3 % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ThunderStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DeadlyStrike)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SacredBlessing)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee + DBSpell.DamageOnHuman % 1000), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DeathSigh)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }

                             
                #endregion

                #region Warrior
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaveofBlood)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Pounce)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ChargingVortex)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.Ironbone)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.IronbonePassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackDragonhowl)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackBloodlust)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildDashAttack)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.TripleAttackRedcurse)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)((DBSpell.GDamage % 1000) * 3 + IncDamagee), 100);
                    update = true;
                }
                #endregion

                #region Pirate
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BladeTempest)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.EagleEye)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.GaleBomb)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                //if (IncreaseAttack == 0 && DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                //{
                //    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                //    update = true;
                //}
                //if (IncreaseAttack == 1 && DBSpell.ID == (ushort)Role.Flags.SpellID.SeaBurial)
                //{
                //    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.Damage2 % 1000 + IncDamagee), 100);
                //    update = true;
                //}
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BloomofDeath)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.DamageOnHuman % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Drukyle || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.DrukylePassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SandMist)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.CaptiveArrow)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanction || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.HolySanctionPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpear || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeak || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGun || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.GiantGunPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAge || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.IceAgePassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSea || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolCano || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.Spitfire || DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.SpitfirePassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CannonBarrage)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BlackbeardsRage)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.NeedLevel % 1000 + IncDamagee), 100);
                    update = true;
                }
                #endregion

                #region Ninja
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
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
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.RageofWar)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)(DBSpell.GDamage % 1000 + IncDamagee), 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.BloodyScythe)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;

                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.FatalSpin)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.DamageOnHuman % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireball || DBSpell.ID == (ushort)Role.Flags.SpellID.WildFireballPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrison || DBSpell.ID == (ushort)Role.Flags.SpellID.WaterPrisonPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachment || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShuriken || DBSpell.ID == (ushort)Role.Flags.SpellID.WhirlShurikenPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlash || DBSpell.ID == (ushort)Role.Flags.SpellID.LightningSlashPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ShurikenVortex)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                #endregion

                #region DW DragonWarrior
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonSlash)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.CrackingSwipe)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SplittingSwipe)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.AirKick)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SpeedKick)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonTransformationPassive)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.SuanniRoar)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonRising)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.GDamage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack1 ||
                    DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack2 ||
                    DBSpell.ID == (ushort)Role.Flags.SpellID.HittheWaterThreethouSandAttack3)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Damage % 1000 + IncDamagee, 100);
                    update = true;
                }
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.DragonCyclone)
                {
                    Damage = Base.MulDiv((int)Damage, (int)(uint)DBSpell.Rate % 1000 + IncDamagee, 100);
                    update = true;
                }
                #endregion
            }
            #endregion

            #region MTD
            if (!update)
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
                    Damage = Base.MulDiv((int)Damage, (int)(150), 100);
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


            #region WideSwipe/Sharpness(Rune Skill)
            byte runeExtraAttack = 0;
            var weapon1 = player.Owner.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon);
            if (weapon1 != null && ((Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("WideSwipe", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Sharpness", ref runeExtraAttack)) || (!Database.ItemType.IsTwoHand(weapon1.ITEM_ID) && player.Owner.Rune.IsEquipped("Cosmic Balance", ref runeExtraAttack))))
            {
                if (runeExtraAttack == 9) runeExtraAttack = 10;

                SpellObj.Damage += (uint)((double)SpellObj.Damage * (double)runeExtraAttack / 100d);
            }
            #endregion

            SpellObj.Damage = (uint)Math.Max(1, Damage);



            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);

            if (Breakdown)
                SpellObj.Effect = MsgAttackPacket.AttackEffect.BreakDown;

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