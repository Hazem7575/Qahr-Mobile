using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackPlayer
    {
        public static bool Verified(Client.GameClient client, Role.Player attacked
      , Database.MagicType.Magic DBSpell, bool flashingname = true, InteractQuery Attack = null)
        {
            if (Attack != null)
            {
                if (Attack.OpponentUID > 0 && Attack.SpellID != 11170 && Attack.SpellID != (ushort)Role.Flags.SpellID.WhirlwindKick && Attack.SpellID != (ushort)Role.Flags.SpellID.InfernalEcho && Attack.SpellID != (ushort)Role.Flags.SpellID.HeavensWonder)
                {
                    if (attacked.UID != Attack.OpponentUID)
                        return false;
                }
            }
            if (client.Player.UID == attacked.UID)
                return false;

            if (attacked.ContainFlag(MsgUpdate.Flags.Dead) || attacked.HitPoints <= 0)
            {
                if (attacked.HitPoints != 0)
                    attacked.Dead(client.Player, attacked.X, attacked.Y, 0);
               // client.CreateBoxDialog("You cannot attack a dead player.");
                return false;
            }

            #region ImmortalForce
            if (DBSpell != null && attacked.ContainFlag(MsgUpdate.Flags.ImmortalForce))
            {
                if (!client.Player.ContainFlag(MsgUpdate.Flags.RiseofTaoism) &&
                    !(DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike))
                {
                    byte itemLevel = 0;
                    byte chanceIgnoreImmortal = 0;

                    if (client.Rune.IsEquipped("Overwhelm", ref itemLevel) ||
                        client.Rune.IsEquipped("Shadow Flow", ref itemLevel))
                    {
                        // 70% chance
                        chanceIgnoreImmortal = 70;

                        // أول مرة لا تتفعل
                        if (!Role.Core.Rate(chanceIgnoreImmortal))
                        {
                            using (var recFp = new ServerSockets.RecycledPacket())
                                client.Player.SendUpdate(
                                    recFp.GetStream(),
                                1,
                                MsgUpdate.DataType.ForbiddenPirate);

                            return false;
                        }
                    }
                    else
                    {
                        using (var recFp = new ServerSockets.RecycledPacket())
                            client.Player.SendUpdate(
                                recFp.GetStream(),
                            1,
                            MsgUpdate.DataType.ForbiddenPirate);

                        return false;
                    }
                }
            }
            #endregion

            #region HellVortex

            if (Database.AtributesStatus.IsNinja(attacked.Owner.Player.Class))
            {
                MsgSpell Spell;
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HellVortex))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HellVortex, out Spell))
                    {
                        Database.MagicType.Magic HellVortex = Pool.Magic[Spell.ID][Spell.Level];
                        if (Role.Core.Rate(HellVortex.Rate / 100))
                        {
                            InteractQuery Attacks = new InteractQuery();
                            Attacks.SpellID = Spell.ID;
                            Attacks.SpellLevel = Spell.Level;
                            Attacks.OpponentUID = client.Player.UID;
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attacks, true);
                            }
                            return false;
                        }

                    }
                }
            }
            #endregion




            //           #region Infinity
            //           if (attacked.ContainFlag(MsgUpdate.Flags.Infinity))
            //           {
            //               if (DBSpell != null)
            //               {
            //                   if ((DBSpell.ID == (ushort)Role.Flags.SpellID.Lightning
            //                   || DBSpell.ID == (ushort)Role.Flags.SpellID.Vulcano
            //                   ||
            //DBSpell.ID == (ushort)Role.Flags.SpellID.FireCircle || DBSpell.ID == (ushort)Role.Flags.SpellID.FireofHell ||
            //DBSpell.ID == (ushort)Role.Flags.SpellID.FlyingMoon || DBSpell.ID == (ushort)Role.Flags.SpellID.Thunder ||
            //DBSpell.ID == (ushort)Role.Flags.SpellID.Fire || DBSpell.ID == (ushort)Role.Flags.SpellID.Tornado))
            //                       return false;
            //               }
            //           }
            //           #endregion

            #region BonePulse

            if (Database.AtributesStatus.IsNinja(attacked.Owner.Player.Class))
            {
                MsgSpell Spell;
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BonePulse))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.BonePulse, out Spell))
                    {
                        Database.MagicType.Magic BonePulse = Pool.Magic[Spell.ID][Spell.Level];
                        if (Role.Core.Rate(BonePulse.Rate / 100))
                        {
                            InteractQuery Attacks = new InteractQuery();
                            Attacks.SpellID = Spell.ID;
                            Attacks.SpellLevel = Spell.Level;
                            Attacks.OpponentUID = client.Player.UID;
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attacks, true);
                            }
                            return false;
                        }

                    }
                }
            }
            #endregion

            if (client.Map.ID == 10137)
            {
                if (attacked.X < 145)
                {
                    client.SendSysMesage("You can't PK in the safe zone.", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                    return false;
                }
            }
            if (client.Map.ID == 10250)
            {
                if (attacked.X + attacked.Y >= 2267 && attacked.X < 1100)
                {
                    client.SendSysMesage("You can't PK in the safe zone.", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                    return false;
                }
            }

            if (attacked.Map == 9855 && attacked.DynamicID == 0)
                return false;

            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
            {
                return false;
            }


            #region ImmortalForce
            if (DBSpell != null && attacked.ContainFlag(MsgUpdate.Flags.ImmortalForce))
            {
                if (!client.Player.ContainFlag(MsgUpdate.Flags.RiseofTaoism) &&
                !(DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.DevouringStrike))
                {
                    byte itemLevel = 0;
                    byte chanceIgnoreImmortal = 0;

                    if (client.Rune.IsEquipped("Overwhelm", ref itemLevel) ||
                        client.Rune.IsEquipped("Shadow Flow", ref itemLevel))
                    {
                        // 70% chance
                        chanceIgnoreImmortal = 70;
                    }

                    if (!Role.Core.Rate(chanceIgnoreImmortal))
                    {
                        using (var recFp = new ServerSockets.RecycledPacket())
                            client.Player.SendUpdate(
                                recFp.GetStream(),
                            1,
                            MsgUpdate.DataType.ForbiddenPirate);

                        return false;
                    }
                }

            }
#endregion


            //#region Cosmic Flash

            //if (DBSpell != null &&
            //    attacked != null &&
            //    attacked.Owner != null &&
            //    attacked.Owner.Rune != null)
            //{
            //    if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScatterFire
            //     || DBSpell.ID == (ushort)Role.Flags.SpellID.NebulousHunt
            //     || DBSpell.ID == (ushort)Role.Flags.SpellID.StarburstArrows)
            //    {
            //        byte itemLevel = 0;

            //        if (attacked.Owner.Rune.IsEquipped("Cosmic Flash", ref itemLevel))
            //        {
            //            int Mohe1444 = 0;

            //            switch (itemLevel)
            //            {
            //                case 1: Mohe1444 = 10; break; // Level 1 
            //                case 2: Mohe1444 = 20; break; // Level 2
            //                case 3: Mohe1444 = 30; break; // Level 3 
            //                case 4: Mohe1444 = 40; break; // Level 4 
            //                case 5: Mohe1444 = 45; break; // Level 5 
            //                case 6: Mohe1444 = 50; break; // Level 6 
            //                case 7: Mohe1444 = 55; break; // Level 7 
            //                case 8: Mohe1444 = 65; break; // Level 8 
            //                case 9: Mohe1444 = 80; break; // Level 9 
            //            }

            //            if (Role.Core.Rate(Mohe1444))
            //            {
            //                using (var recycledPacket = new ServerSockets.RecycledPacket())
            //                {
            //                    var stream = recycledPacket.GetStream();

            //                    attacked.SendString(
            //                        stream,
            //                        MsgStringPacket.StringID.Effect,
            //                        true,
            //                        new string[1] { "poisonmiss" }
            //                    );
            //                }

            //                return false;
            //            }
            //        }
            //    }
            //}

            //#endregion

            #region DodgeRate
            if (attacked.Owner.Status.DodgeRate > 0)
            {
                byte itemLevel = 0;
                int chanceIgnoreImmortal = 0;
                if (client.Rune.IsEquipped("Slow Pace", ref itemLevel))
                {
                    switch (itemLevel)
                    {
                        case 1: chanceIgnoreImmortal = 5; break;
                        case 2: chanceIgnoreImmortal = 10; break;
                        case 3: chanceIgnoreImmortal = 15; break;
                        case 4: chanceIgnoreImmortal = 20; break;
                        case 5: chanceIgnoreImmortal = 25; break;
                        case 6: chanceIgnoreImmortal = 30; break;
                        case 7: chanceIgnoreImmortal = 35; break;
                        case 8: chanceIgnoreImmortal = 40; break;
                        case 9: chanceIgnoreImmortal = 50; break;
                    }
                }
                if (!Role.Core.Rate(chanceIgnoreImmortal + client.Status.HawkeyeLevel))
                {
                    #region SkySoarer
                    ushort RateHawkeye = 0;
                    byte SkySoarer = 0;
                    if (client.Rune.IsEquipped("Sky Soarer", ref SkySoarer)
                        && client.Player.ContainFlag(MsgUpdate.Flags.SkySoarer))
                    {
                        switch (SkySoarer)
                        {
                            case 1: RateHawkeye = 100; break;
                            case 2: RateHawkeye = 200; break;
                            case 3: RateHawkeye = 300; break;
                            case 4: RateHawkeye = 400; break;
                            case 5: RateHawkeye = 500; break;
                            case 6: RateHawkeye = 800; break;
                            case 7: RateHawkeye = 1000; break;
                            case 8: RateHawkeye = 1200; break;
                            case 9: RateHawkeye = 1500; break;
                        }
                    }
                    #endregion

                    if (Calculate.Base.GetRefinery((attacked.Owner.Status.DodgeRate / 100), (client.Status.HitRate + RateHawkeye) / 100))
                    {
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            attacked.SendString(streamm, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                            return false;
                        }
                    }
                }

            }
            if (attacked.Owner.Player.HitRateFantasy > 0 && attacked.ContainFlag(MsgUpdate.Flags.ShieldofTruth))
            {
                byte itemLevel = 0;
                int chanceIgnoreImmortal = 0;
                if (client.Rune.IsEquipped("Slow Pace", ref itemLevel))
                {
                    switch (itemLevel)
                    {
                        case 1: chanceIgnoreImmortal = 5; break;
                        case 2: chanceIgnoreImmortal = 10; break;
                        case 3: chanceIgnoreImmortal = 15; break;
                        case 4: chanceIgnoreImmortal = 20; break;
                        case 5: chanceIgnoreImmortal = 25; break;
                        case 6: chanceIgnoreImmortal = 30; break;
                        case 7: chanceIgnoreImmortal = 35; break;
                        case 8: chanceIgnoreImmortal = 40; break;
                        case 9: chanceIgnoreImmortal = 50; break;
                    }
                }
                if (!Role.Core.Rate(chanceIgnoreImmortal + client.Status.HawkeyeLevel))
                {

                    if (Calculate.Base.Rate(attacked.Owner.Player.HitRateFantasy))
                    {
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            attacked.SendString(streamm, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                            return false;
                        }
                    }
                }
            }
            #endregion
            #region DeadlyStrike
            if (client.Player.ContainFlag(MsgUpdate.Flags.DeadlyStrike))
            {
                if (Calculate.Base.Rate(20))
                {
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        attacked.SendString(streamm, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                        return false;
                    }
                }
            }

            #endregion
            #region Revenge
            if (attacked.ContainFlag(MsgUpdate.Flags.Revenge))
            {
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    attacked.SendString(streamm, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                    return false;
                }
            }
            #endregion
            #region Hunter
            if (attacked.ContainFlag(MsgUpdate.Flags.Hunter))
            {
                MsgSpell Owner_spells = null;
                if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Hunter, out Owner_spells))
                {
                    Database.MagicType.Magic Hunter = Pool.Magic[Owner_spells.ID][Owner_spells.Level];
                    if (Role.Core.Rate(Hunter.Damage / 100))
                    {
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            attacked.SendString(streamm, MsgStringPacket.StringID.Effect, true, new string[1] { "poisonmiss" });
                            return false;
                        }
                    }
                }
            }
            #endregion

            #region NeptuneCurse
            if (DBSpell != null && attacked.ContainFlag(MsgUpdate.Flags.NeptuneCurse))
            {
                return false;
            }
            #endregion

            if (!attacked.Alive)
                return false;
            if (attacked.ContainFlag(MsgUpdate.Flags.Dead) || attacked.HitPoints <= 0)
            {
                //client.CreateBoxDialog("You cannot attack a dead player.");
                return false;
            }
            if (attacked.Invisible)
                return false;
            if (client.Player.PkMode == Role.Flags.PKMode.Peace)
                return false;

            if (client.Player.Map == 3935 && client.Player.Map == 10137)
            {
                foreach (var server in Database.GroupServerList.GroupServers.Values)
                {
                    if (Role.Core.GetDistance((ushort)server.X, (ushort)server.Y, attacked.X, attacked.Y) <= 8)
                        return false;
                }
            }



            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.ExtremePk)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }





            if (DBSpell != null)
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.Fly) && DBSpell.ID != (ushort)Role.Flags.SpellID.SickleWind)
                {
                    if (DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachment || DBSpell.ID == (ushort)Role.Flags.SpellID.DustDetachmentPassive ||
                        DBSpell.ID == (ushort)Role.Flags.SpellID.SuperTwofoldBlade)
                        return false;
                    if (DBSpell == null || !DBSpell.AttackInFly || DBSpell.ID == client.Player.RandomSpell)
                        return false;
                }
            }


            if (!MsgYuanshen.AirBlocker(client))
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.Fly))
                {
                    if (DBSpell == null || !DBSpell.AttackInFly || DBSpell.ID == client.Player.RandomSpell)
                        return false;
                }

            }
            if (attacked.Owner.IsWatching())
            {
                return false;
            }
            if (client.IsWatching())
            {
                return false;
            }
            if (!attacked.AllowAttack())
                return false;
            { var ta = global::Qahr.Mobile.TeamQualifier.TeamArenaCore.CanAttack(client, attacked); if (ta.HasValue) return ta.Value; } // [TeamArena 2026-10-14]
            if (client.Player.Map == MsgTournaments.MsgCaptureTheFlag.MapID)
            {
                if (!MsgTournaments.MsgSchedules.CaptureTheFlag.Attackable(attacked))
                    return false;
            }


            if (client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009
                || client.Player.Map == 4020)
                return false;

            if (client.Player.PkMode == Role.Flags.PKMode.Team)
            {
                if (client.Team != null)
                {
                    if (client.Team.Members.ContainsKey(attacked.UID))
                        return false;
                }

                if (client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID))
                    return false;

                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;

                    if (attacked.MyGuild != null)
                    {
                        if (client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                            return false;
                    }
                }
                if (client.Player.MyClan != null && client.Player.ClanUID > 0)
                {
                    if (client.Player.ClanUID == attacked.ClanUID)
                        return false;

                    if (attacked.MyClan != null)
                    {
                        if (client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                            return false;
                    }
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Guild)
            {
                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;
                    if (attacked.MyGuild != null)
                    {
                        if (client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                            return false;
                    }

                }
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.HeroOfGame)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BestFight)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.CS)
            {
                if (client.Player.ServerID == attacked.ServerID)
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Union)
            {
                if (client.Player.InUnion && attacked.InUnion)
                {
                    if (client.Player.MyUnion.UID == attacked.MyUnion.UID)
                        return false;
                }
            }
            // [Audit H6] Enforce BlockAttackMap (market/arenas/event instances) BEFORE the
            // Jiang branch, otherwise Jiang mode returns true above and bypasses it entirely.
            // (Jiang's own feature maps - 1002/1000/1015/1020/1011 - are not in BlockAttackMap,
            // so legitimate JiangHu PvP there is unaffected.)
            if (Pool.Constants.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                return false;
            else if (client.Player.DynamicID != 0)
            {
                if (Pool.Constants.BlockAttackMap.Contains(client.Player.DynamicID))
                    return false;
            }

            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
            {
                if (attacked.JiangHuActive != 0)
                {
                    if (attacked.X < 390 && attacked.Y > 384)
                    {
                        client.SendSysMesage("You can't PK in the safe zone.", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                        return false;
                    }
                    {
                        if (client.Player.JiangPkFlag != Role.Instance.JiangHu.AttackFlag.None)
                        {
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitFriends) == Role.Instance.JiangHu.AttackFlag.NotHitFriends)
                            {
                                return !client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan) == Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan)
                            {
                                if (client.Player.MyClan != null)
                                    return !client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild) == Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild)
                            {
                                if (client.Player.MyGuild != null)
                                    return !client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitClanMembers) == Role.Instance.JiangHu.AttackFlag.NotHitClanMembers)
                            {
                                return !(client.Player.ClanUID == attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers) == Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers)
                            {
                                return !(client.Player.GuildID == attacked.GuildID);
                            }
                        }
                        return true;
                    }
                }
                else
                    return false;
            }
            if (client.Player.Map == 1002 || client.Player.Map == 1004 || client.Player.Map == 3053)
            {
                return false;
            }
            if (client.InTeamQualifier())
            {
                if (client.Team != null && client.Player.Map == 9855)
                {
                    if (client.Team.Members.ContainsKey(attacked.UID))
                        return false;
                }
            }
            if (client.Player.Map == 1002)
            {
                if (!client.Player.OnMyOwnServer) return false;
            }

            if (client.Player.ContainFlag(MsgUpdate.Flags.ChillingSnow))
            {
                MsgServer.AttackHandler.CheckAttack.ProcessColdTime.ProcessAttack.Enqueue(new MsgServer.AttackHandler.CheckAttack.ProcessColdTime.Record()
                {
                    Attacker = client,
                    Attacked = attacked.Owner
                });
            }

            if (client.Player.PkMode == Role.Flags.PKMode.Capture)
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.FlashingName)
                    || attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    return true;
                else
                    return false;
            }

            // [Audit H6] The duplicate Jiang block that previously stood here was dead code:
            // the first Jiang branch above always returns for Jiang mode, so execution never
            // reached it. It has been removed. BlockAttackMap is now enforced above the PK
            // branches (see the [Audit H6] block before the Jiang branch).
            if (Pool.Constants.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                return false;
            else if (client.Player.DynamicID != 0)
            {
                if (Pool.Constants.BlockAttackMap.Contains(client.Player.DynamicID))
                    return false;
            }
            if (Program.ServerConfig.IsInterServer == false && flashingname)
            {
                if (client.Player.PkMode != Role.Flags.PKMode.Capture && client.Player.PkMode != Role.Flags.PKMode.Peace)
                {
                    if (!attacked.ContainFlag(MsgUpdate.Flags.FlashingName) && !attacked.ContainFlag(MsgUpdate.Flags.RedName) && !attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    {
                        if (!Pool.Constants.FreePkMap.Contains(attacked.Map) && attacked.DynamicID == 0)
                        {
                            if (!Pool.Constants.SkillsNotAvailableHere.Contains(attacked.Map) && attacked.DynamicID == 0)
                            {
                                if (!(attacked.Map == 1015 && DateTime.Now.Minute >= 28 && DateTime.Now.Minute <= 35))//Pole Bird
                                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);
                            }
                        }

                    }
                }
            }


            return true;
        }

        public static bool CanAttack(Client.GameClient client, InteractQuery Attack)
        {
            Role.IMapObj target_;
            if (client.Player.View.TryGetValue(Attack.OpponentUID, out target_, Role.MapObjectType.Player))
            {
                Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                MsgSpell ClientSpell;
                Database.MagicType.Magic DBSpell;
                if (Pool.Magic.TryGetValue(Attack.SpellID, out DBSpells))
                {
                    if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                    {
                        if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                        {
                            Role.Player Target = target_ as Role.Player;
                            if (!CanAttackPlayer.Verified(client, Target, DBSpell, false, Attack))
                            {
                                client.OnAutoAttack = false;
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}