using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgTournaments;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Npc
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.SobNpc attacked)
        {
            client.Player.AttackHit = true;
            MsgYuanshen.Magics(client, attacked, 0);
            client.MyArchives.ReceiveAttack(attacked);

            #region SupremeLeadership
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SupremeLeadership][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SupremeLeadership;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            #endregion
            #region DragonHeart
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonHeart))
            {
                Database.MagicType.Magic DragonHeart = Pool.Magic[(ushort)Role.Flags.SpellID.DragonHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level];
                if (Role.Core.Rate(DragonHeart.Rate))
                {


                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.DragonHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengHeart))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level];
                if (Role.Core.Rate(KunpengHeart.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengRocket))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengRocket][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level];
                int IncRate = 0;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Spreadyourwings))
                {
                    Database.MagicType.Magic Hitthewaterthreethousand = Pool.Magic[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand].Level];
                    IncRate = Hitthewaterthreethousand.DamageOnHuman;
                }
                if (Role.Core.Rate(KunpengHeart.Rate + IncRate))
                {

                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengRocket;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniHeart))
            {
                Database.MagicType.Magic SuanniHeart = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level];
                if (Role.Core.Rate(SuanniHeart.Damage2))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniCommand))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniCommand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniCommand;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = attacked.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            #endregion

            #region StarFlow
            if (client.MyArchives.isOpen(Archives.TypeID.StoneCracker))
            {
                #region StarFlow
                if (client.Player.StarFlow >= 0 && client.Player.StarFlow <= 2)
                {
                    client.Player.StarFlow++;
                    if (client.Player.StarFlow == 3)
                    {
                        client.Player.StarFlow = 4;
                        client.Player.RemoveFlag(MsgUpdate.Flags.StarFlow);
                        client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlowEx, 10, true);
                        InteractQuery AttackPaket = new InteractQuery();
                        AttackPaket.OpponentUID = attacked.UID;
                        AttackPaket.UID = client.Player.UID;
                        AttackPaket.X = client.Player.X;
                        AttackPaket.Y = client.Player.Y;
                        AttackPaket.SpellID = (ushort)Role.Flags.SpellID.StarFlow;
                        MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                    }
                    else
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.StarFlow))
                        {
                            client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlow, Role.StatusFlagsBigVector32.PermanentFlag, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.StarFlow, 0, (uint)client.Player.StarFlow, 0, MsgUpdate.DataType.StarFlow);
                        }
                    }
                }
                if (client.Player.StarFlow == 4)
                    client.Player.StarFlow = 0;
                #endregion

            }

            #endregion

            #region ThundercloudSight
            client.Player.ThundercloudSight = attacked.UID;
            #endregion

            #region DragonPierce
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPierce))
            {
                Database.MagicType.Magic DragonPierces = Pool.Magic[(ushort)Role.Flags.SpellID.DragonPierce][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonPierce].Level];

                List<byte> CanUse = new List<byte>();
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit))
                    CanUse.Add(1);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceTortoise))
                    CanUse.Add(2);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceBreak))
                    CanUse.Add(3);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceStamina))
                    CanUse.Add(4);
                if (CanUse.Count != 0)
                {
                    switch (CanUse[Pool.GetRandom.Next(0, CanUse.Count)])
                    {
                        case 1:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceCrit, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceCrit, (uint)DragonPierces.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 2:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceTortoise, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceTortoise, (uint)DragonPierces.Duration, (uint)DragonPierces.DamageOnHuman, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 3:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceBreak, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceBreak, (uint)DragonPierces.Duration, (uint)DragonPierces.Damage2, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 4:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceStamina, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceStamina, (uint)DragonPierces.Duration, 0, (uint)DragonPierces.Damage3, MsgUpdate.DataType.ArchiveSkill);
                            break;
                    }

                }
            }

            #endregion

            #region Trojan

            if (client.Player.ContainFlag(MsgUpdate.Flags.CleanSweep))
                obj.Damage += (uint)(obj.Damage * client.Player.CleanSweepPower / 100);
            if (client.Player.ContainFlag(MsgUpdate.Flags.AxeShadow))
                obj.Damage += (uint)(obj.Damage * client.Player.AxeShadowPower / 100);
            #endregion

            #region SageMode
            if (Database.AtributesStatus.IsNinja(client.Player.Class))
            {
                foreach (var Ninja in NinjaFile.gouyu_immortals.Values)
                {
                    if ((client.MyNinja.Levels > 17 ? 17 : client.MyNinja.Levels) == Ninja.Level)
                    {
                        if (Role.Core.Rate(Ninja.Rate / 100))
                        {
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.SageMode))
                            {
                                client.MyNinja.SageMode(Ninja.Seconds);
                            }
                        }
                    }
                }
            }
            #endregion

            #region DrainingTouch
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
                    client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                        Id = client.Player.UID,
                        dwParam = client.Player.UID
                    }), true);

                    bool update = false;
                    if (client.Player.HitPoints < client.Player.Owner.Status.MaxHitpoints)
                    {
                        update = true;
                        client.Player.HitPoints = (int)client.Player.Owner.Status.MaxHitpoints;
                    }
                    if (client.Player.Mana < client.Player.Owner.Status.MaxMana)
                    {
                        update = true;
                        client.Player.Mana = (ushort)client.Player.Owner.Status.MaxMana;
                    }
                    if (update)
                    {
                        client.Player.SendUpdateHP();
                        client.Player.SendUpdate(stream, client.Player.Mana, MsgUpdate.DataType.Mana, false);
                    }
                }
            }
            #endregion

            #region Bloodlust
            if (client.MyArchives.isOpen(Archives.TypeID.Bloodlust))
            {
                #region Immersion
                if (!client.Player.ContainFlag(MsgUpdate.Flags.Immersion))
                {
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Immersion))
                    {
                        Database.MagicType.Magic Immersion = Pool.Magic[(ushort)Role.Flags.SpellID.Immersion][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Immersion].Level];
                        if (Role.Core.Rate(Immersion.Damage2))
                        {
                            InteractQuery InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = Immersion.ID;
                            InteractQuery.SpellLevel = Immersion.Level;
                            InteractQuery.X = attacked.X;
                            InteractQuery.Y = attacked.Y;
                            InteractQuery.OpponentUID = client.Player.UID;
                            MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                        }
                    }
                }
                #endregion
                #region Insouciance
                if (!client.Player.ContainFlag(MsgUpdate.Flags.Insouciance))
                {
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Insouciance))
                    {
                        Database.MagicType.Magic Insouciance = Pool.Magic[(ushort)Role.Flags.SpellID.Insouciance][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Insouciance].Level];
                        if (Role.Core.Rate(Insouciance.Damage2))
                        {
                            InteractQuery InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = Insouciance.ID;
                            InteractQuery.SpellLevel = Insouciance.Level;
                            InteractQuery.X = attacked.X;
                            InteractQuery.Y = attacked.Y;
                            ;
                            InteractQuery.OpponentUID = client.Player.UID;
                            MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                        }
                    }
                }
                #endregion
            }
            #endregion

            if (client.Player.ContainFlag(MsgUpdate.Flags.Superman))
            {
                if (Database.ItemType.IsWarriorEpicWeapons(client.Equipment.RightWeapon)
                            && Database.ItemType.IsWarriorEpicWeapons(client.Equipment.LeftWeapon))
                {
                    obj.Damage *= 5;
                }
            }
            //#region Demolition
            //if (client.Status.Demolition > 0)
            //{
            //    MythSoulAttributes.Attribute MythInfo;
            //    if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Demolition].TryGetValue(client.Status.Demolition, out MythInfo))
            //    {
            //        obj.Damage += (obj.Damage * (uint)MythInfo.Rate) / 100;
            //    }
            //}
            //#endregion
            if (client.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                obj.Effect = MsgAttackPacket.AttackEffect.Destroy;
            global::Qahr.Mobile.NewGuildWar.NgwEngine.OnSobDamage(client, attacked, (uint)obj.Damage); // [NewGuildWar 2026-10-14] score pillar / gate / Honor Pillar damage
            #region SobNpc
            if (obj.Damage >= attacked.HitPoints)
            {
                uint exp = (uint)attacked.HitPoints;
                attacked.Die(stream, client);
                if (attacked.Map == 1039)
                    return exp / 10;
            }
            else
            {
                attacked.HitPoints -= (int)obj.Damage;

                if (attacked.UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);

                else if (attacked.UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.UpdateScore(client.Player, obj.Damage);


                else if (MsgTournaments.MsgWarOfPlayers.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    MsgTournaments.MsgWarOfPlayers.UpdateScore(stream, obj.Damage, client.Player);

                else if (MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    MsgSchedules.EliteGuildWar.UpdateScore(stream, obj.Damage, client.Player.MyGuild);

                //else if (MsgTournaments.MsgNewKingsWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgTournaments.MsgNewKingsWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgTournaments.MsgNewKingsWar.UpdateScore(stream, obj.Damage, client.Player);
                //else if (MsgSchedules.Guild6PoleWar6.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.Guild6PoleWar6.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.Guild6PoleWar6.UpdateScore(stream, obj.Damage, client.Player.MyGuild);


                //else if (MsgSchedules.EmperorWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.EmperorWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.EmperorWar.UpdateScore(stream, obj.Damage, client.Player.MyGuild);


                //else if (MsgSchedules.TopWarScore.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.TopWarScore.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.TopWarScore.UpdateScore(stream, obj.Damage, client.Player.MyGuild);

                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.UnionWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.UnionWar.UpdateScore(client.Player, obj.Damage);


                



                //else if (MsgSchedules.ClanTwin.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.ClanTwin.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.ClanTwin.UpdateScore(stream, obj.Damage, client.Player.MyClan);

                //else if (MsgSchedules.ClanPhoenix.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.ClanPhoenix.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.ClanPhoenix.UpdateScore(stream, obj.Damage, client.Player.MyClan);

                //else if (MsgSchedules.ClanApe.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.ClanApe.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.ClanApe.UpdateScore(stream, obj.Damage, client.Player.MyClan);

                //else if (MsgSchedules.ClanDesert.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.ClanDesert.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.ClanDesert.UpdateScore(stream, obj.Damage, client.Player.MyClan);

                //else if (MsgSchedules.ClanBird.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == MsgSchedules.ClanBird.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.ClanBird.UpdateScore(stream, obj.Damage, client.Player.MyClan);






                if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(attacked.UID))
                {
                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(client.Player, attacked, obj.Damage, stream);
                }
                //else if (Game.MsgTournaments.MsgSchedules.ClanWar.Process == MsgTournaments.ProcesType.Alive)
                //{
                //    if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar != null && Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.InWar(client))
                //    {
                //        if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.Proces == MsgTournaments.ProcesType.Alive)
                //        {
                //            Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.UpdateScore(client.Player, obj.Damage);
                //        }
                //    }
                //}
                if (attacked.Map == 1039 || attacked.Map == 1038)
                    return obj.Damage / 1000;
            }
            #endregion

            return 0;
        }
    }
}