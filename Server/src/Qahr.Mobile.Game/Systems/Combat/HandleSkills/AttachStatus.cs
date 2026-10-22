using Qahr.Mobile.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class AttachStatus
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.BeastMastery:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.BeastMastery))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.BeastMastery, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.BeastMastery, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }

                    case (ushort)Role.Flags.SpellID.WaveBreak:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.WaveBreak, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.KunpengHeart:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            user.MyArchives.SetValue("KunpengHeart", DBSpell.GDamage);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.KunpengHeart, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.KunpengHeart, (uint)DBSpell.Duration, (uint)DBSpell.GDamage, 0, MsgUpdate.DataType.ArchiveSkill, true);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }

                    case (ushort)Role.Flags.SpellID.KunpengTrek:
                        {
                            if (Attack.X == user.Player.X || Attack.X == 0)
                                break;
                            if (!user.Map.ValidLocation(Attack.X, Attack.Y))
                                break;
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Hit = 1 });
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Shift(Attack.X, Attack.Y, stream, false);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.KunpengRocket:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            if (user.Player.PengchengMilesCount < 3)
                            {
                                if (user.Player.PengchengMilesCount >= 0 && user.Player.PengchengMilesCount <= 2)
                                {
                                    user.Player.PengchengMilesCount++;
                                    user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.Kunpeng_Soaring, 0, user.Player.PengchengMilesCount, 0, (Game.MsgServer.MsgUpdate.DataType)143);
                                }
                            }
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Hitthewaterthreethousand:
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Spreadyourwings))
                            {
                                user.Send(stream.InteractionCreate(Attack));
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                                 , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Spreadyourwings, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Spreadyourwings, DBSpell.Duration, 0, (uint)DBSpell.DamageOnMonster, MsgUpdate.DataType.ArchiveSkill, true);

                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuanniHeart:
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.SuanNiHeart))
                            {
                                user.Send(stream.InteractionCreate(Attack));
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                                 , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.SuanNiHeart, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.SuanNiHeart, DBSpell.Duration, (uint)DBSpell.GDamage, (uint)DBSpell.Level, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonTransformation:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                           , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.DragonShift, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.DragonShift, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            user.Player.DragonShiftCount = 3;
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #region WaterTaoSkills
                    case (ushort)Role.Flags.SpellID.Substitution:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            user.Player.AddSpellFlag(MsgUpdate.Flags.Substitution, (int)Role.StatusFlagsBigVector32.PermanentFlag, true);
                            user.Player.SubstitutionDefence = (int)DBSpell.Damage;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WaterAegis:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AzureShieldLevel = (byte)ClientSpell.Level;
                            user.Player.AzureShieldDefence = (ushort)DBSpell.DamageOnHuman;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.AzureShield, (int)DBSpell.Damage2, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.AzureShield, (uint)DBSpell.Damage2, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.Level, MsgUpdate.DataType.AzureShield);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WaterAegisRebirth:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AzureShieldLevel = (byte)ClientSpell.Level;
                            user.Player.AzureShieldDefence = (ushort)DBSpell.DamageOnHuman;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.AzureShield, (int)DBSpell.Damage2, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.AzureShield, (uint)DBSpell.Damage2, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.Level, MsgUpdate.DataType.AzureShield);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlowKnack:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.FlowKnack, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.FlowKnack, DBSpell.Duration, (uint)DBSpell.Damage, (uint)DBSpell.DamageOnMonster, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FantasyKnack:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DivineEmptiness:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.DivineEmptiness, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.DivineEmptiness, DBSpell.Duration, (uint)DBSpell.DamageOnHuman, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlameShield:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.FlameShield)) break;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.FlameShield, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.FlameShield, DBSpell.Duration, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.Damage2, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SolidBulwark:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.SolidBulwark, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.SolidBulwark, DBSpell.Duration, (uint)DBSpell.Damage, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }


                    case (ushort)Role.Flags.SpellID.HolyProtection:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.HolyProtection, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.HolyProtection, DBSpell.Duration, (uint)DBSpell.DamageOnHuman, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;

                        }
                    case (ushort)Role.Flags.SpellID.PhoenixBlessing:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.PhoenixBlessing, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.PhoenixBlessing, DBSpell.Duration, (uint)DBSpell.DamageOnHuman, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;

                        }
                    case (ushort)Role.Flags.SpellID.WildPhoenix:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.WildPhoenixDamage = 3;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.WildPhoenix, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.WildPhoenix, DBSpell.Duration, (uint)((DBSpell.DamageOnHuman * 1000) + DBSpell.Damage2), user.Player.WildPhoenixDamage, MsgUpdate.DataType.ArchiveSkill, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;

                        }
                    #endregion
                    case (ushort)Role.Flags.SpellID.Immersion:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.Immersion, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.Immersion, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;

                        }


                    case (ushort)Role.Flags.SpellID.Insouciance:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.Insouciance, (int)DBSpell.Level, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.Insouciance, DBSpell.Level, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;

                        }
                    case (ushort)Role.Flags.SpellID.MudWall:
                    case (ushort)Role.Flags.SpellID.MudWallPassive:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.MudWall, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.MudWall, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;

                        }
                    case (ushort)Role.Flags.SpellID.ArmorofImmunity:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.ArmorOfImmunity, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.ArmorOfImmunity, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;

                        }

                    case (ushort)Role.Flags.SpellID.PaperDance:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.PaperDance, (int)DBSpell.Duration, true);

                            user.Player.SendUpdate(stream, MsgUpdate.Flags.PaperDance, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HawksAmbition:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.ContainFlag(MsgUpdate.Flags.HawksAmbition))
                            {
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (targer.UID == Attack.OpponentUID)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 150 / 100; // Mohe Done (Attack)
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }

                                }
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Player.RemoveFlag(MsgUpdate.Flags.HawksAmbition);
                                break;
                            }
                            user.Player.AddFlag(MsgUpdate.Flags.HawksAmbition, (int)DBSpell.Duration, true);

                            user.Player.HawksAmbitionPower = (uint)DBSpell.Damage;
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.HawksAmbition, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.RevengeTail:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.RevengeTail))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.RevengeTail, 10, true, 20);


                                user.Player.RevengeTailChange = 5;
                                if (user.Rune.IsEquipped("RevengeGale"))
                                    user.Player.RevengeTailChange = int.MaxValue;

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Infinity:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.Infinity, (int)DBSpell.Duration, true);
                            user.Player.InfinitySDamage = (uint)DBSpell.DamageOnHuman;
                            user.Player.InfinityDamage = (uint)DBSpell.DamageOnMonster;
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.Infinity, DBSpell.Duration
       , (uint)0, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.Infinity, true);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.CrackStar:
                        {
                            if (!Database.AtributesStatus.IsWindWalker(user.Player.Class) || (user.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack) break;

                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.CrackStar, (int)DBSpell.Duration, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Sacrifice:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            if (user.Player.ContainFlag(MsgUpdate.Flags.FineRain1) || user.Player.ContainFlag(MsgUpdate.Flags.FineRain2))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.FineRain1);
                                user.Player.RemoveFlag(MsgUpdate.Flags.FineRain2);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante);

                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Sacrifice))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.Sacrifice);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                break;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.Sacrifice, Role.StatusFlagsBigVector32.PermanentFlag, false);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.RiseofTaoism:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.RiseofTaoism, (int)DBSpell.Duration, true);
                            user.Player.RiseofTaoismExtraMDamage = (uint)DBSpell.Damage;
                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.RiseofTaoism, DBSpell.Duration
     , (uint)30000, 0, Game.MsgServer.MsgUpdate.DataType.FineRain, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BloodTide:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            user.Player.BloodTideCaptured = 0;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.BloodTide, (int)35, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.BloodTide, 35, user.Player.BloodTideHP, user.Player.BloodTideHP, MsgUpdate.DataType.FineRain);
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.XPList) && user.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                            {
                                user.Player.XPCount = 0;
                                user.Player.SendUpdate(stream, 1, MsgUpdate.DataType.XPList);
                                user.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            user.Player.BloodTideHP = (uint)DBSpell.Damage;

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            user.Player.HitPoints = (int)user.Status.MaxHitpoints;
                            user.Player.BloodTideStamp = Time32.Now;
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FuryStrike:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.IronGuard, (int)DBSpell.Duration, true);
                            user.Player.FuryStrikeHP = (uint)DBSpell.DamageOnHuman;
                            user.Player.FuryStrikeAttack = (byte)DBSpell.Damage;
                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.IronGuard, DBSpell.Duration
     , (uint)0, 0, Game.MsgServer.MsgUpdate.DataType.SkillCountdown, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.IronShield:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.IronShieldLevel = (byte)ClientSpell.Level;
                            user.Player.IronShieldDamage = (uint)DBSpell.DamageOnMonster;
                            user.Player.IronShieldTime = (int)DBSpell.Duration;
                            user.Player.IronShieldStamp = DateTime.Now;
                            user.Player.AddSpellFlag(MsgUpdate.Flags.IronShield, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.IronShield, DBSpell.Duration
     , (uint)DBSpell.DamageOnMonster, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FineRain:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            if (!user.Player.Alive)
                                return;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.FineRainPower = (uint)DBSpell.Damage;
                            user.Player.defFineRainPower = (uint)DBSpell.Damage;
                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.FineRain1, DBSpell.Duration
     , user.Player.FineRainPower, user.Player.FineRainPower, Game.MsgServer.MsgUpdate.DataType.FineRain, true);
                            user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.FineRain2, DBSpell.Duration
   , user.Player.FineRainPower, user.Player.FineRainPower, Game.MsgServer.MsgUpdate.DataType.FineRain, true);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.FineRain1, (int)DBSpell.Duration, true, 1);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.FineRain2, (int)DBSpell.Duration, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            if (user.Team != null)
                                foreach (var target in user.Team.GetMembers())
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18 && target.Player.UID != user.Player.UID)
                                    {
                                        if (target.Player.Alive && !target.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                                        {
                                            target.Player.FineRainPower = (uint)DBSpell.Damage;
                                            target.Player.defFineRainPower = (uint)DBSpell.Damage;
                                            target.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.FineRain1, DBSpell.Duration
                     , target.Player.FineRainPower, target.Player.FineRainPower, Game.MsgServer.MsgUpdate.DataType.FineRain, true);
                                            target.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.FineRain2, DBSpell.Duration
                   , target.Player.FineRainPower, target.Player.FineRainPower, Game.MsgServer.MsgUpdate.DataType.FineRain, true);

                                            target.Player.AddSpellFlag(MsgUpdate.Flags.FineRain1, (int)DBSpell.Duration, true, 1);
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.FineRain2, (int)DBSpell.Duration, true);
                                            target.Equipment.QueryEquipment(target.Equipment.Alternante);
                                            target.Player.HitPoints = (int)target.Status.MaxHitpoints;
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                        }
                                    }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            user.Player.HitPoints = (int)user.Status.MaxHitpoints;
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ChillingSnow:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.ChillingSnow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);
                                break;
                            }
                            user.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.ChillingSnow, Role.StatusFlagsBigVector32.PermanentFlag, false, 5);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HealingSnow:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                                break;
                            }
                            user.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.HealingSnow, Role.StatusFlagsBigVector32.PermanentFlag, false, 5);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FreezingPelter:
                        {

                            user.Send(stream.InteractionCreate(Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                                break;
                            }
                            user.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.FreezingPelter, Role.StatusFlagsBigVector32.PermanentFlag, false, 5);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.ResistWood));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpiritFocus:
                    case (ushort)Role.Flags.SpellID.Intensify:
                        {
                            Attack.SpellID = 0;
                            Attack.KilledMonster = ClientSpell.ID;
                            Attack.SpellLevel = ClientSpell.Level;
                            user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                            user.Player.IntensifyStamp = DateTime.Now;
                            user.Player.InUseIntensify = true;
                            user.Player.IntensifyDamage = (int)DBSpell.Damage;
                            user.Player.FocusClientSpell = ClientSpell;
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WeepStorm:
                        {
                            user.Player.WeepStronCantidad = 0;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul, 1);
                            user.Player.AddSpellFlag(Game.MsgServer.MsgUpdate.Flags.WeepStorm, (int)8, true);
                            Attack.SpellID = 0;
                            Attack.KilledMonster = ClientSpell.ID;
                            Attack.SpellLevel = ClientSpell.Level;
                            user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                            user.Player.FocusClientSpell = ClientSpell;
                            user.Player.InUseIntensify = true;
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Backfire:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);



                            user.Player.AddFlag(MsgUpdate.Flags.Backfire, 10, false);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    //case (ushort)Role.Flags.SpellID.AutoBackfire:
                    //    {
                    //        if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                    //        {
                    //            user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                    //            break;
                    //        }


                    //        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                    //            , 0, Attack.X, Attack.Y, ClientSpell.ID
                    //            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                    //        if (DateTime.Now > user.Player.AutoBackfire.AddSeconds(15))
                    //        {
                    //            user.Player.AddFlag(MsgUpdate.Flags.Backfire, 10, false);
                    //        }

                    //        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                    //        MsgSpell.SetStream(stream);
                    //        MsgSpell.Send(user);

                    //        break;
                    //    }
                    case (ushort)Role.Flags.SpellID.PathOfShadow:
                        {
                            if (!user.MyArchives.Items.ContainsKey(Role.Instance.Archives.TypeID.StoneCracker)
                               && !user.MyArchives.Items.ContainsKey(Role.Instance.Archives.TypeID.ColdMoon)
                               && !user.MyArchives.Items.ContainsKey(Role.Instance.Archives.TypeID.ThornCutter))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);


                                if (!user.Player.RemoveFlag(MsgUpdate.Flags.PathOfShadow))
                                    user.Player.AddFlag(MsgUpdate.Flags.PathOfShadow, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                else
                                {
                                    user.Player.RemoveFlag(MsgUpdate.Flags.KineticSpark);
                                }
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.DefensiveStance:
                        {
                            if (user.MyArchives.isOpen(Role.Instance.Archives.TypeID.Redcurse))
                            {
                                if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                    user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);


                                if (!user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance))
                                {
                                    user.Player.AddFlag(MsgUpdate.Flags.DefensiveStance, (int)DBSpell.Duration, false);
                                    user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DefensiveStance, (uint)DBSpell.Duration
                                      , (uint)DBSpell.Damage, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DefensiveStance, true);
                                }
                                else
                                {
                                    user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance);
                                }


                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.LightningShield:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (!user.Player.ContainFlag(MsgUpdate.Flags.LightningShield))
                            {
                                user.Player.AddFlag(MsgUpdate.Flags.LightningShield, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.LightningShield);
                                user.Player.RemoveFlag(MsgUpdate.Flags.LightningShieldActivated);
                                using (var recycledPacket = new ServerSockets.RecycledPacket())
                                {
                                    var streamm = recycledPacket.GetStream();
                                    user.Player.SendUpdate(streamm, MsgUpdate.Flags.LightningShieldActivated, 0, 0, 0, MsgUpdate.DataType.AzureShield);
                                }
                            }

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SparkShield:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (!user.Player.ContainFlag(MsgUpdate.Flags.SparkShield))
                            {
                                user.Player.AddFlag(MsgUpdate.Flags.SparkShield, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.SparkShield);

                            }


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ImmortalForce:
                        {

                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.ImmortalForce, (int)DBSpell.Duration, true);


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.PoisonStar:
                        {
                            user.CreateBoxDialog("Sorry, This Spell Is Closed .");
                            //MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            //, 0, Attack.X, Attack.Y, ClientSpell.ID
                            //, ClientSpell.Level, ClientSpell.UseSpellSoul);


                            //Role.IMapObj target;
                            //if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            //{
                            //    Role.Player attacked = target as Role.Player;
                            //    byte itemLevel = 0;
                            //    if (attacked.Owner.Rune.IsEquipped("Sturdiness", ref itemLevel))
                            //    {
                            //        itemLevel = (byte)(5 + (itemLevel * 5));
                            //        if (Role.Core.Rate(itemLevel))
                            //        {
                            //            MsgSpell.SetStream(stream);
                            //            MsgSpell.Send(user);
                            //            break;
                            //        }
                            //    }
                            //    if (Calculate.Base.Rate(80))
                            //    {
                            //        attacked.AddSpellFlag(MsgUpdate.Flags.PoisonStar, (int)DBSpell.Duration, true);
                            //        attacked.Owner.SendSysMesage("Poison star activated. You will not be able to use drugs for " + (int)DBSpell.Duration + " seconds.");
                            //        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 1, MsgAttackPacket.AttackEffect.None));
                            //    }
                            //    else
                            //    {

                            //        var clientobj = new MsgSpellAnimation.SpellObj(attacked.UID, MsgSpell.SpellID, MsgAttackPacket.AttackEffect.None);
                            //        clientobj.Hit = 0;
                            //        MsgSpell.Targets.Enqueue(clientobj);
                            //    }
                            //}
                            //MsgSpell.SetStream(stream);
                            //MsgSpell.Send(user);

                            //Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 250, DBSpells);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonSwing:
                        {
                            user.Send(stream.InteractionCreate(Attack));


                            if (user.Player.ContainFlag(MsgUpdate.Flags.DragonSwing))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.DragonSwing);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DragonSwing, DBSpell.Duration
       , (uint)33, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DragonSwing, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                user.Player.DragonSwingChance = (byte)(DBSpell.Damage / 100);
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);



                            MsgSpell.Send(user);




                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonFlow:
                        {
                            user.Send(stream.InteractionCreate(Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.DragonFlow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.DragonFlow);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.DragonFlow, Role.StatusFlagsBigVector32.PermanentFlag, false, 8);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);



                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Stigma:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                user.SetStigmaPower((int)(DBSpell.GDamage % 1000) - 100, (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official Stigma +10..30% = power-30100
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                    attacked.Owner.SetStigmaPower((int)(DBSpell.GDamage % 1000) - 100, (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official Stigma +10..30% = power-30100
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.MagicShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                if (!user.Player.ContainFlag(MsgUpdate.Flags.Shield))
                                {
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                    user.SetShieldPower((int)(DBSpell.GDamage % 1000), (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official MagicShield/SuperShield defense +10..50% = power-30000
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        attacked.Owner.SetShieldPower((int)(DBSpell.GDamage % 1000), (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official MagicShield/SuperShield defense +10..50% = power-30000
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuperShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                if (!user.Player.ContainFlag(MsgUpdate.Flags.Shield))
                                {
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                    user.SetShieldPower((int)(DBSpell.GDamage % 1000), (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official MagicShield/SuperShield defense +10..50% = power-30000
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        attacked.Owner.SetShieldPower((int)(DBSpell.GDamage % 1000), (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official MagicShield/SuperShield defense +10..50% = power-30000
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SoulShackle:
                        {
                            if (!Database.AtributesStatus.IsMonk(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Monk Character Can Use This Spell !");
                                break;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                user.Player.UID,
                                0,
                                Attack.X,
                                Attack.Y,
                                ClientSpell.ID,
                                ClientSpell.Level,
                                ClientSpell.UseSpellSoul);

                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;

                                if (attacked == null || attacked.Owner == null)
                                    return;

                                if (user.Team != null && attacked.Owner.Team != null)
                                {
                                    if (user.Team == attacked.Owner.Team)
                                    {
                                        user.SendSysMesage("You cannot use this skill on your team member.");
                                        return;
                                    }
                                }

                                if (user.Player.MyGuild != null &&
                                    user.Player.MyGuild.Ally != null &&
                                    attacked.MyGuild != null)
                                {
                                    if (attacked.MyGuild.Info.GuildID == user.Player.MyGuild.Info.GuildID
                                        || user.Player.MyGuild.Ally.ContainsKey(attacked.MyGuild.Info.GuildID))
                                    {
                                        user.SendSysMesage("You cannot use this skill on your guild or ally member.");
                                        return;
                                    }
                                }

                                if (attacked.Alive)
                                    return;

                                if (!attacked.ContainFlag(MsgUpdate.Flags.SoulShackle) &&
                                    !attacked.ContainFlag(MsgUpdate.Flags.SacredBlessing))
                                {
                                    if ((attacked.BattlePower - user.Player.BattlePower) >= 20)
                                    {
                                        user.SendSysMesage(
                                            "Your BattlePower is lower than your opponent get a better BattlePower to be able to use this skill on him.",
                                            MsgMessage.ChatMode.TopLeftSystem,
                                            MsgMessage.MsgColor.red);

                                        break;
                                    }

                                    byte itemLevel = 0;

                                    if (attacked.Owner.Rune.IsEquipped("Sturdiness", ref itemLevel))
                                    {
                                        itemLevel = (byte)(5 + (itemLevel * 5));
                                        if (Role.Core.Rate(itemLevel))
                                        {
                                            MsgSpell.SetStream(stream);
                                            MsgSpell.Send(user);
                                            break;
                                        }
                                    }

                                    if (attacked.Owner.PerfectionStatus.FreeSoul > 0)
                                    {
                                        if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.FreeSoul))
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                attacked.View.SendView(
                                                    stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                                    {
                                                        Effect = MsgRefineEffect.RefineEffects.FreeSoul,
                                                        Id = attacked.UID,
                                                        dwParam = attacked.UID
                                                    }), true);
                                            }

                                            MsgSpell.SetStream(stream);
                                            MsgSpell.Send(user);
                                            break;
                                        }
                                    }

                                    uint seconds = (uint)DBSpell.Duration;
                                    itemLevel = 0;

                                    if (attacked.Owner.Rune.IsEquipped("Evocation", ref itemLevel))
                                    {
                                        seconds -= (byte)(20 + (itemLevel * 5));
                                        if (seconds < 0) seconds = 0;
                                    }

                                    attacked.SendUpdate(
                                        stream,
                                        MsgUpdate.Flags.SoulShackle,
                                        seconds,
                                        0,
                                        ClientSpell.Level,
                                        MsgUpdate.DataType.SoulShackle,
                                        false);

                                    attacked.AddSpellFlag(MsgUpdate.Flags.SoulShackle, (int)seconds, true);

                                    MsgSpell.Targets.Enqueue(
                                        new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.StarofAccuracy:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Invisibility:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WildDash:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID
                            , 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.WildDash, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.WildDash, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.WildDashAttack;
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Revenge:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Revenge)) break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            if (user.Player.AddFlag(MsgUpdate.Flags.Revenge, (int)DBSpell.Duration, true))
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Revenge, DBSpell.Duration, 10000, 0, MsgUpdate.DataType.ArchiveSkill);



                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.AzureShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            int Time = 15 * ClientSpell.Level;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AzureShieldLevel = (byte)ClientSpell.Level;
                                user.Player.AzureShieldDefence = 50000;
                                user.Player.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AzureShieldLevel = (byte)ClientSpell.Level;
                                    attacked.AzureShieldDefence = 50000;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.Shield:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.AddFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                            user.SetShieldPower((int)(DBSpell.GDamage % 1000), (int)DBSpell.Duration); // [CombatAudit 2026-10-14] official XP Shield "Triple the caster`s Defense" = power 30300

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Accuracy:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.AddFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.XpFly:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                                user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                            else
                                user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Fly:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.OnTransform || user.Player.ContainFlag(MsgUpdate.Flags.Ride) || user.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;
                            }

                            if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                                user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                            else
                                user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);
                            #region HoverFeather
                            MsgSpell Owner_spell = null;
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HoverFeather))
                            {
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HoverFeather, out Owner_spell))
                                {
                                    MsgSpell user_spell = null;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ElementalArrowData, out user_spell))
                                    {
                                        Database.MagicType.Magic Data = Pool.Magic[user_spell.ID][user_spell.Level];
                                        Database.MagicType.Magic Data2 = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                                        List<byte> CanUse = new List<byte>();
                                        user.Player.HoverFatherDamage = (uint)Data2.GDamage;
                                        user.Player.AddFlag(MsgUpdate.Flags.HoverFather, (int)Data2.Duration, true);
                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.HoverFather, Data2.Duration, (uint)Data2.GDamage, 0, MsgUpdate.DataType.HoverData);
                                        for (int x = 0; x < Data2.Damage3 - 2; x++)
                                        {
                                            switch (x)
                                            {
                                                case 0:
                                                    {
                                                        user.Player.AddSpellFlag(MsgUpdate.Flags.FireArrow, (int)Data.Duration, true);
                                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.FireArrow, (uint)Data.Duration, (uint)100, (uint)0, MsgUpdate.DataType.ArchiveSkill);
                                                        break;
                                                    }
                                                case 1:
                                                    {


                                                        user.Player.AddSpellFlag(MsgUpdate.Flags.IceArrow, (int)Data.Duration, true);
                                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.IceArrow, (uint)Data.Duration, (uint)Data.DamageOnHuman, (uint)Data.Damage2, MsgUpdate.DataType.ArchiveSkill);
                                                        break;
                                                    }
                                                case 2:
                                                    {


                                                        user.Player.AddSpellFlag(MsgUpdate.Flags.PoisonArrow, (int)Data.Duration, true);
                                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.PoisonArrow, (uint)Data.Duration, (uint)Data.Damage3, (uint)Data.Damage3, MsgUpdate.DataType.ArchiveSkill);
                                                        break;
                                                    }
                                                case 3:
                                                    {

                                                        user.Player.AddSpellFlag(MsgUpdate.Flags.ThunderArrow, (int)Data.Duration, true);
                                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.ThunderArrow, (uint)Data.Duration, (uint)200, 0, MsgUpdate.DataType.ArchiveSkill);
                                                        break;
                                                    }
                                                case 4:
                                                    {

                                                        user.Player.AddSpellFlag(MsgUpdate.Flags.WindArrow, (int)Data.Duration, true);
                                                        user.Player.SendUpdate(stream, MsgUpdate.Flags.WindArrow, (uint)Data.Duration, (uint)Data.DamageOnMonster, 0, MsgUpdate.DataType.ArchiveSkill);
                                                        break;
                                                    }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.Bless:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            user.Player.AddFlag(MsgUpdate.Flags.CastPray, Role.StatusFlagsBigVector32.PermanentFlag, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FatalStrike:
                        {

                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.FatalStrike, 60);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuperCyclone:
                        {

                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            byte MaxStamina = (byte)(user.Player.HeavenBlessing > 0 ? 150 : 100);
                            if (user.Player.Stamina < MaxStamina)
                            {
                                user.Player.Stamina = MaxStamina;
                                using (var rect = new ServerSockets.RecycledPacket())
                                {
                                    var steam = rect.GetStream();
                                    user.Player.SendUpdate(steam, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);//الاستمينا
                                }
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.SuperCyclone, 40);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Cyclone:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                             , 0, Attack.X, Attack.Y, ClientSpell.ID
                             , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.Cyclone, 20);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Superman:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.Superman, 20);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
