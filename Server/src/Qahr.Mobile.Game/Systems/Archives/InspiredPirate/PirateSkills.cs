using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class PirateSkills
    {

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellIDPirate.LordThreat:
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);
                            #region Lord Impact
                            byte LordImpactL = 0;
                            byte Rate = 0;
                            if (user.Rune.IsEquipped("Lord Impact", ref LordImpactL))
                            {
                                switch (LordImpactL)
                                {
                                    case 1: Rate = 1; break;
                                    case 2: Rate = 2; break;
                                    case 3: Rate = 3; break;
                                    case 4: Rate = 5; break;
                                    case 5: Rate = 7; break;
                                    case 6: Rate = 9; break;
                                    case 7: Rate = 11; break;
                                    case 8: Rate = 13; break;
                                    case 9: Rate = 15; break;
                                }
                            }
                            #endregion
                            if (Calculate.Base.Rate((DBSpell.DamageOnHuman / 100) + Rate))
                            {
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 10)
                                    {
                                        if (!attacked.ContainFlag(MsgUpdate.Flags.Ride))
                                            continue;



                                        attacked.Stamina = 0;
                                        attacked.SendUpdate(stream, attacked.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                        attacked.RemoveFlag(MsgUpdate.Flags.Ride);
                                        attacked.AddSpellFlag((MsgUpdate.Flags)418, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = attacked.UID, Hit = 1 });
                                        Attack.OpponentUID = attacked.UID;
                                        Attack.X = attacked.X;
                                        Attack.Y = attacked.Y;
                                        user.Player.View.SendView(stream.InteractionCreate(Attack), true);

                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.LordThreatPassive:
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);


                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 10)
                                {
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Ride))
                                        continue;



                                    attacked.Stamina = 0;
                                    attacked.SendUpdate(stream, attacked.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                    attacked.RemoveFlag(MsgUpdate.Flags.Ride);
                                    attacked.AddSpellFlag((MsgUpdate.Flags)418, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = attacked.UID, Hit = 1 });
                                    Attack.OpponentUID = attacked.UID;
                                    Attack.X = attacked.X;
                                    Attack.Y = attacked.Y;
                                    user.Player.View.SendView(stream.InteractionCreate(Attack), true);

                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.LavaNut:
                    case (ushort)Role.Flags.SpellIDPirate.ThunderNut:
                    case (ushort)Role.Flags.SpellIDPirate.FrozenNut:
                        {
                            if (!user.Player.Alive)
                                return;
                            PirateSkills.ExecuteSkills(user, (ushort)Attack.SpellPirate, Attack.UID, Attack.OpponentUID, Attack.X, Attack.Y);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, 0, 0);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID });
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #region FlagsHit
                    case (ushort)Role.Flags.SpellIDPirate.Diabolize:
                        {
                            if (!user.Player.Alive)
                                return;


                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Diabolize))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Diabolize, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Diabolize, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Armed:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Armed))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Armed, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Armed, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Vast:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Vast))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Vast, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Vast, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Barrier:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Barrier))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Barrier, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Barrier, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Shell:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Shell))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Shell, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Shell, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Storm:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.Damage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Storm))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Storm, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Storm, (uint)DBSpell.Duration, (uint)Power, (uint)Power, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Thrash:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var atacked = target as Role.Player;
                                if (!atacked.ContainFlag(MsgUpdate.Flags.Thrash))
                                {
                                    atacked.Owner.MyArchives.SetValue(DBSpell.Name, Power);
                                    atacked.AddSpellFlag(MsgUpdate.Flags.Thrash, (int)DBSpell.Duration, true);
                                    atacked.SendUpdate(stream, MsgUpdate.Flags.Thrash, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    atacked.Owner.Equipment.QueryEquipment(atacked.Owner.Equipment.Alternante);
                                }
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.ThunderPirate:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.ThunderPirate))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.ThunderPirate, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.ThunderPirate, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Torrent:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Torrent))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Torrent, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Torrent, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Tide:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Tide))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Tide, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Tide, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Splash:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Splash))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Splash, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Splash, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Sailing:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Sailing))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Sailing, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Sailing, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Thunderlord:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (user.Player.RemoveFlag(MsgUpdate.Flags.XPList))
                            {
                                user.Player.OpenXpSkill(MsgUpdate.Flags.ThunderLord, (int)DBSpell.Duration, 0);

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;

                        }
                    case (ushort)Role.Flags.SpellIDPirate.Revelator:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (DBSpell.Duration == 0 && DBSpell.Status != 0)
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                if (user.Player.ContainFlag(MsgUpdate.Flags.Revelator))
                                {
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Revelator, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.Revelator, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                }
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                                break;
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Fusing:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.GDamage;
                            if (DBSpell.Duration == 0 && DBSpell.Status != 0)
                            {
                                if (user.Player.ContainFlag(MsgUpdate.Flags.Fusing))
                                {
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Fusing, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.Fusing, (uint)DBSpell.Duration, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                }
                                else
                                    user.Player.RemoveFlag((MsgUpdate.Flags)DBSpell.Status);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                break;
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.ColdBloodline:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            uint Power = (uint)DBSpell.DamageOnHuman;
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.ColdBloodline))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, Power);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.ColdBloodline, (int)10, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.ColdBloodline, (uint)10, (uint)Power, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }

                    case (ushort)Role.Flags.SpellIDPirate.Sense:
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Sense))
                            {
                                user.MyArchives.SetValue(DBSpell.Name, DBSpell.DamageOnHuman, (uint)DBSpell.Damage2);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Sense, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Sense, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.Damage2, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                            }
                            break;
                        }
                    #endregion
                    #region AttackHit
                    case (ushort)Role.Flags.SpellIDPirate.Dark:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Role.IMapObj target;

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var atacked = target as Role.Player;

                                #region Pure Serenity
                                double RatePureSerenity = 0;
                                byte PureSerenity = 0;
                                bool Active = true;
                                if (atacked.Owner.Rune.IsEquipped("Pure Serenity", ref PureSerenity))
                                {
                                    switch (PureSerenity)
                                    {
                                        case 1: RatePureSerenity = 0.5; break;
                                        case 2: RatePureSerenity = 1; break;
                                        case 3: RatePureSerenity = 1.5; break;
                                        case 4: RatePureSerenity = 2; break;
                                        case 5: RatePureSerenity = 2.5; break;
                                        case 6: RatePureSerenity = 3; break;
                                        case 7: RatePureSerenity = 3.5; break;
                                        case 8: RatePureSerenity = 5; break;
                                        case 9: RatePureSerenity = 7; break;
                                    }
                                    if (atacked.Owner.Player.BattlePower >= user.Player.BattlePower)
                                    {
                                        int battlerpower = (atacked.Owner.Player.BattlePower - user.Player.BattlePower) * 3;
                                        RatePureSerenity += battlerpower;
                                    }

                                }
                                #endregion
                                if (!Role.MyMath.Success(RatePureSerenity))
                                    Active = false;
                                if (Active)
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = atacked.UID, Hit = 1, Damage = 0 });
                                    if (atacked.ContainFlag(MsgUpdate.Flags.Stigma))
                                        atacked.RemoveFlag(MsgUpdate.Flags.Stigma);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.Shield))
                                        atacked.RemoveFlag(MsgUpdate.Flags.Shield);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.AzureShield))
                                        atacked.RemoveFlag(MsgUpdate.Flags.AzureShield);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.DragonFlow))
                                        atacked.RemoveFlag(MsgUpdate.Flags.DragonFlow);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.Slayer))
                                        atacked.RemoveFlag(MsgUpdate.Flags.Slayer);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.ImmortalForce))
                                        atacked.RemoveFlag(MsgUpdate.Flags.ImmortalForce);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.LightningShieldActivated))
                                        atacked.RemoveFlag(MsgUpdate.Flags.LightningShieldActivated);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.SparkShield))
                                        atacked.RemoveFlag(MsgUpdate.Flags.SparkShield);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.FineRain1))
                                        atacked.RemoveFlag(MsgUpdate.Flags.FineRain1);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.FineRain2))
                                        atacked.RemoveFlag(MsgUpdate.Flags.FineRain2);
                                    if (atacked.ContainFlag(MsgUpdate.Flags.Rampage))
                                        atacked.RemoveFlag(MsgUpdate.Flags.Rampage);
                                    atacked.Owner.Equipment.QueryEquipment(atacked.Owner.Equipment.Alternante);
                                }
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.Drukyle:
                    case (ushort)Role.Flags.SpellIDPirate.DrukylePassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)DBSpell.Duration))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        int Rate = DBSpell.Damage2 / 100;
                                        if (attacked.BattlePower > user.Player.BattlePower)
                                        {
                                            int Bp = attacked.BattlePower - user.Player.BattlePower;
                                            Rate = Math.Min(0, Bp);
                                        }
                                        else if (user.Player.BattlePower > attacked.BattlePower)
                                        {
                                            int Bp = user.Player.BattlePower - attacked.BattlePower;
                                            Rate = Math.Min(100, Bp);
                                        }
                                        if (Role.Core.Rate(Rate))
                                        {
                                            attacked.AddFlag(MsgUpdate.Flags.Numb, (int)DBSpell.Duration, true);
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.Numb, (uint)DBSpell.Duration, (uint)0, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);

                                        }
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.CaptiveArrow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)DBSpell.Duration))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            user.Player.AddFlag(MsgUpdate.Flags.CaptiveArrow, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.CaptiveArrow, (uint)DBSpell.Duration, (uint)DBSpell.Damage3, 0, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.SetStream(stream);
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.GiantGun:
                    case (ushort)Role.Flags.SpellIDPirate.GiantGunPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)DBSpell.MaxTargets))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.TwospiendSpear:
                    case (ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)DBSpell.MaxTargets))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack:
                        {
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(1000);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                    case (ushort)Role.Flags.SpellIDPirate.Overlord:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official Overlord tooltip 50-75% to human targets = power): AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                    case (ushort)Role.Flags.SpellIDPirate.HolySanction:
                    case (ushort)Role.Flags.SpellIDPirate.HolySanctionPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.MaxTargets / 2)
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
                    case (ushort)Role.Flags.SpellIDPirate.Spitfire:
                    case (ushort)Role.Flags.SpellIDPirate.SpitfirePassive:
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
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
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
                    #region FloorItem
                    case (ushort)Role.Flags.SpellIDPirate.SandMist:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.SandMist, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);

                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 14;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.StarVolCano:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.StarVolcano, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 4000, 900);

                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.StarVolcanoPassive, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 4000, 900);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.LavaSea:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.LavaSea, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.LavaSeaPassive, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.IceAge:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.IceAge, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 23;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
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
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        #region IceAge(Stun)
                                        if (attacked.Alive)
                                        {
                                            int Rate = 5;
                                            if (attacked.BattlePower <= user.Player.BattlePower)
                                            {
                                                int Inc = (user.Player.BattlePower - attacked.BattlePower) * 2;
                                                Rate += Inc;
                                                if (Role.Core.Rate(Rate))
                                                {
                                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Freeze))
                                                        attacked.AddFlag(MsgUpdate.Flags.Freeze, (int)1, true);

                                                }
                                            }
                                        }
                                        #endregion
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
                    case (ushort)Role.Flags.SpellIDPirate.IceAgePassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.IceAgePassive, (ushort)Attack.X, (ushort)Attack.Y, 29, DBSpell, 3000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 29;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
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
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
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
                    case (ushort)Role.Flags.SpellIDPirate.PheasantBeak:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.PheasantBeak, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)DBSpell.Duration))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)DBSpell.Duration))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.PheasantBeakPassive, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, (byte)2))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
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
                                if (Line.InLine(targer.X, targer.Y, (byte)2))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, (byte)2))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion

                }
            }
        }
        public unsafe static bool ExecuteSkills(Client.GameClient user, ushort SpellID, uint UID, uint OpponentUID, ushort X, ushort Y, bool AR = false)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgSpell ClientSpell;
                    if (SpellID != 21690
                        && SpellID != 21670
                        && SpellID != 21680
                        && SpellID != 21700
                        && SpellID != 22070
                        && SpellID != 22080
                        && SpellID != 22090
                        && SpellID != 22100
                        && SpellID != 22110 && SpellID != 21930 && SpellID != 21930
                        && SpellID != 21930
                        && SpellID != 21930 && !AR)
                    {
                        if (!user.MyArchives.TryGetValueEquip(SpellID))
                            return false;
                    }

                    if (user.MySpells.ClientSpells.TryGetValue(SpellID, out ClientSpell))
                    {
                        MagicType.Magic Magic = Pool.Magic[ClientSpell.ID][ClientSpell.Level];
                        bool T = false;
                        bool F = true;
                        switch ((Role.Flags.SpellIDPirate)SpellID)
                        {
                            #region RateMeleSkills
                            #region HolySanctionPassive
                            case Role.Flags.SpellIDPirate.HolySanctionPassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region TwospiendSpearpassive
                            case Role.Flags.SpellIDPirate.TwospiendSpearpassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region GiantGunPassive
                            case Role.Flags.SpellIDPirate.GiantGunPassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region StarVolcanoPassive
                            case Role.Flags.SpellIDPirate.StarVolcanoPassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region LavaSeaPassive
                            case Role.Flags.SpellIDPirate.LavaSeaPassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region IceAgePassive
                            case Role.Flags.SpellIDPirate.IceAgePassive:
                                {
                                    if (Role.Core.Rate(Magic.Rate % 1000))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region SpitfirePassive
                            case Role.Flags.SpellIDPirate.SpitfirePassive:
                                {
                                    if (Role.Core.Rate((Magic.Rate % 1000), 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region PheasantBeakPassive
                            case Role.Flags.SpellIDPirate.PheasantBeakPassive:
                                {
                                    if (Role.Core.Rate((Magic.Rate % 1000), 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region DrukylePassive
                            case Role.Flags.SpellIDPirate.DrukylePassive:
                                {
                                    if (Role.Core.Rate((Magic.Rate % 1000), 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #endregion
                            #region RateRune
                            #region Vast
                            case Role.Flags.SpellIDPirate.Vast:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Barrier
                            case Role.Flags.SpellIDPirate.Barrier:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Shell
                            case Role.Flags.SpellIDPirate.Shell:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Storm
                            case Role.Flags.SpellIDPirate.Storm:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Thrash
                            case Role.Flags.SpellIDPirate.Thrash:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region ThunderPirate
                            case Role.Flags.SpellIDPirate.ThunderPirate:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Torrent
                            case Role.Flags.SpellIDPirate.Torrent:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Tide
                            case Role.Flags.SpellIDPirate.Tide:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Splash
                            case Role.Flags.SpellIDPirate.Splash:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Fusing
                            case Role.Flags.SpellIDPirate.Fusing:
                                {
                                    T = true;
                                    break;
                                }
                            #endregion
                            #region Sailing
                            case Role.Flags.SpellIDPirate.Sailing:
                                {
                                    if (Role.Core.Rate(Magic.Rate))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #endregion
                            #region RateAirPowersSkills
                            #region Sense
                            case Role.Flags.SpellIDPirate.Sense:
                                {
                                    if (Role.Core.Rate(Magic.Rate / 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #region Armed
                            case Role.Flags.SpellIDPirate.Armed:
                                {
                                    if (Role.Core.Rate(Magic.Rate / 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #endregion
                            #region RateSkillRank1
                            #region Dark
                            case Role.Flags.SpellIDPirate.Dark:
                                {
                                    if (Role.Core.Rate(Magic.Rate / 100))
                                        T = true;
                                    else
                                        F = false;
                                    break;
                                }
                            #endregion
                            #endregion
                        }
                        if (T || F)
                        {
                            user.Player.RandomSpell = ClientSpell.ID;
                            InteractQuery InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = ClientSpell.ID;
                            InteractQuery.SpellLevel = ClientSpell.Level;
                            InteractQuery.X = X;
                            InteractQuery.Y = Y;
                            InteractQuery.UID = UID;
                            InteractQuery.OpponentUID = OpponentUID;
                            MsgAttackPacket.ProcescMagic(user, stream, InteractQuery, true);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

    }
}