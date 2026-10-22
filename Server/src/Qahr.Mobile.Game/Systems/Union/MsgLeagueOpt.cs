using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgLeagueOpt
    {
        public enum ActionID : byte
        {
            Info = 0,
            SynList = 1,
            Members = 2,
            ViewOthers = 3,
            List = 4,
            MyUnion = 5,
            NewUnionName = 6,
            Create = 7,
            Pledge = 8,
            Quit = 9,
            ExpelMember = 10,
            GuildPledge = 11,
            GuildQuit = 12,
            ExpelGuild = 13,
            TransferLeader = 14,
            KingdomRank = 15,
            UnionRank = 16,
            KingdomTitleGui = 17,
            NewUnionTitle = 18,
            RecruitDeclaration = 20,
            NewNameDone = 22,
            ImperialGuards = 23,
            AppointImperialGuard = 24,
            RemovePalaceGuard = 25,
            ReSignPalaceGuard = 26,
            CoreOfficials = 27,
            AppointCoreOffical = 28,
            RemoveCoreOffical = 29,
            ReSignCoreOffical = 30,
            Officials = 34,
            Tokens = 37,
            SaveLeagueOrderStatu = 38,
            Stipend = 39,
            ImperialHarem = 40,
            AppointLeagueConcubines = 41,
            UppdateLeagueConcubines = 42,
            RemoveHarem = 44,
            Allowance = 46,
            AppointCore = 47,
            AppointGuard = 48,
            AppointHarem = 49,
            UpdateHarem = 50,
            ChangeUnionName = 51,
            Announce = 66,
            PlunderWar = 69



        }
        public static unsafe void GetLeagueOpt(this ServerSockets.Packet stream, out ActionID Action, out uint ID
              , out uint UID, out uint Index, out string[] texts)
        {
            Action = (ActionID)stream.ReadUInt16();
            ID = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Index = stream.ReadUInt32();
            texts = stream.ReadStringList();

        }

        public static unsafe ServerSockets.Packet LeagueOptCreate(this ServerSockets.Packet stream, ActionID Action, uint ID,
            uint UID, uint Index, params string[] texts)
        {
            stream.InitWriter();

            stream.Write((ushort)Action);
            stream.Write(ID);
            stream.Write(UID);
            stream.Write(Index);
            if (texts != null)
                stream.Write(texts);
            else
                stream.ZeroFill(2);

            stream.Finalize(GamePackets.MsgLeagueOpt);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgLeagueOpt)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            ActionID Action;
            uint ID;
            uint UID;
            uint Index;
            string[] texts;

            stream.GetLeagueOpt(out Action, out ID, out UID, out Index, out texts);
            if (global::Qahr.Mobile.GuildRecruit.LeagueApply.OnLeagueOpt(user, (ushort)Action, Index, stream)) return; // [GuildRecruit 2026-10-14] union applications 8/11/82/83/85/86/87

            switch (Action)
            {

                case ActionID.ChangeUnionName:
                    {
                        var name = texts[0];
                        if (Role.Instance.Union.CheckName(name))
                        {
                            if (user.Player.InUnion && user.Player.IsUnionEmperror)
                            {
                                user.Player.MyUnion.Name = name;
                                foreach (var player in Pool.GamePoll.Values)
                                    if (player.Player.InUnion && player.Player.MyUnion.UID == user.Player.MyUnion.UID)
                                        user.Player.MyUnion.SendMyInfo(stream, player);
                                user.Send(stream.LeagueOptCreate(ActionID.ChangeUnionName, 0, 0, user.Player.MyUnion.UID, ""));
                            }
                        }
                        else
                            user.CreateBoxDialog("Invalid union name.");
                        break;
                    }
                case ActionID.NewUnionTitle:
                    {
                        var name = texts[0];
                        if (Role.Instance.Union.CheckName(name))
                        {
                            if (user.Player.InUnion && user.Player.IsUnionEmperror)
                            {
                                user.Player.MyUnion.Title = name;
                                user.Send(stream.LeagueOptCreate(ActionID.NewUnionTitle, 0, 0, user.Player.MyUnion.UID, ""));
                            }
                        }
                        break;
                    }
                case ActionID.Quit:
                    {
                        if (user.Player.MyGuild != null && user.Player.InUnion)
                        {
                            if (user.Player.MyGuild.UnionID == user.Player.MyUnion.UID)
                                break;
                            if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                                break;
                        }
                        else if (user.Player.InUnion)
                        {

                            user.Send(stream.LeagueOptCreate(ActionID.MyUnion, 0, 0, user.Player.MyUnion.UID, ""));
                            user.Player.MyUnion.Quit(stream, user.Player.UnionMemeber);


                        }
                        break;
                    }
                case ActionID.GuildQuit:
                    {
                        if (user.Player.MyGuildMember == null || user.Player.MyGuild == null)
                            break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        Role.Instance.Guild guild;
                        if (user.Player.MyUnion.Guilds.TryGetValue(ID, out guild))
                        {
                            user.Send(stream.LeagueOptCreate(ActionID.MyUnion, 0, 0, user.Player.MyUnion.UID, ""));
                            user.Player.MyUnion.ExpelGuild(stream, guild);
                        }


                        break;
                    }
                case ActionID.ExpelGuild:
                    {
                        if (user.Player.IsUnionEmperror)
                        {

                            Role.Instance.Guild guild;
                            if (user.Player.MyUnion.Guilds.TryGetValue(ID, out guild))
                            {
                                user.Player.MyUnion.ExpelGuild(stream, guild);
                            }
                        }
                        break;
                    }
                case ActionID.ExpelMember:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member;
                            if (user.Player.MyUnion.Members.TryGetValue(Index, out Member))
                            {
                                user.Player.MyUnion.Quit(stream, Member);
                            }
                        }
                        break;
                    }
                case (ActionID)81:
                case (ActionID)84:
                    {
                        if (user.Player.InUnion == false)
                        {
                            if (user.Player.MyGuild == null)
                            {
                                Role.Instance.Union union;
                                if (Role.Instance.Union.UnionPoll.TryGetValue(Index, out union))
                                {
                                    union.AddOtherMember(stream, user);
                                }
                            }
                        }
                        break;
                    }
                case ActionID.GuildPledge:
                    {
                        if (user.Player.InUnion == false && user.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader
                            && user.Player.MyGuild != null)
                        {
                            Role.Instance.Union union;
                            if (Role.Instance.Union.UnionPoll.TryGetValue(Index, out union))
                            {
                                union.AddGuild(stream, user.Player.MyGuild);
                            }
                        }
                        break;
                    }
                case ActionID.List:
                    {
                        if (user.Player.InUnion == false)
                        {
                            const int max = 3;
                            var array = Role.Instance.Union.GetOrderyUnions;
                            int offset = (int)(Index) * max;
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeagueAllegianceListCreate((uint)array.Length, (ushort)Index, (ushort)count, 1);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var element = array[x + offset];
                                stream.AddItemLeagueAllegianceList(element.Treasury, element.UID, element.GoldBrick, element.Name, element.Emperor, element.RecruitDeclaration, element.IsKingdom);
                            }
                            user.Send(stream.LeagueAllegianceListFinalize());


                        }
                        break;
                    }

                case ActionID.ImperialGuards:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;
                            var array = user.Player.MyUnion.ImperialGuards.Values.ToArray();

                            int offset = (int)(Index - 1) * max;
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            // QahrMobile: offset 4 is the number of units in this packet (CMsgLeaguePalaceGuardsList::Process), not the total
                            stream.LeaguePalaceGuardsListCreate((byte)Index, (byte)count);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var element = array[x + offset];

                                if (element.IsOnline)
                                    stream.AddItemLeaguePalaceGuardsList((uint)element.Owner.Player.BattlePower, element.Mesh, element.Exploits, (uint)element.NobilityRank, element.UID, element.Level, element.Class, 1, element.Name);
                                else
                                    stream.AddItemLeaguePalaceGuardsList(0, element.Mesh, element.Exploits, (uint)element.NobilityRank, element.UID, element.Level, element.Class, 0, element.Name);
                            }


                            user.Send(stream.LeaguePalaceGuardsListFinalize());

                        }
                        break;
                    }

                case ActionID.RemovePalaceGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member;
                            if (user.Player.MyUnion.ImperialGuards.TryGetValue(UID, out Member))
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));
                            }
                        }
                        break;
                    }

                case ActionID.RemoveCoreOffical:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member = user.Player.MyUnion.GetOfficialMemberByUID(UID);
                            if (Member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));


                            }
                        }
                        break;
                    }
                case ActionID.RemoveHarem:
                    {

                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member = user.Player.MyUnion.GetImperialHaremMemberByUID(UID);
                            if (Member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));


                            }
                        }
                        break;
                    }
                case ActionID.ImperialHarem:
                    {

                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.GetImperialHarem;
                            MsgLeagueConcubines.LeagueImperialHarem harem = new MsgLeagueConcubines.LeagueImperialHarem();
                            harem.type = (byte)1;
                            harem.ImperialHarems = new MsgLeagueConcubines.ImperialHarem[array.Length];
                            int index_s = 0;
                            int index_c = 0;
                            for (int x = 0; x < array.Length; x++)
                            {
                                harem.ImperialHarems[x] = new MsgLeagueConcubines.ImperialHarem(array[x]);
                                if (harem.ImperialHarems[x].Rank == 401)
                                {
                                    harem.ImperialHarems[x].Rank = (uint)(4011 + index_s);
                                    index_s += 1;
                                }
                                if (harem.ImperialHarems[x].Rank == 402)
                                {
                                    harem.ImperialHarems[x].Rank = (uint)(4021 + index_c);
                                    index_c += 1;
                                }
                            }
                            user.Send(stream.CreateLeagueConcubines(harem));
                        }
                        break;
                    }

                case ActionID.AppointGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            if (user.Player.MyUnion.ImperialGuards.Count == 20)
                            {
                                user.CreateBoxDialog("Sorry the imperial guards list is full.");
                                break;
                            }
                            var member = user.Player.MyUnion.GetMemberByName(texts[0]);
                            if (member != null && member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                user.Send(stream.LeagueOptCreate(ActionID.AppointGuard, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.AppointImperialGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            if (user.Player.MyUnion.ImperialGuards.Count == 20)
                            {
                                user.CreateBoxDialog("Sorry the imperial guards list is full.");
                                break;
                            }
                            Index = (uint)Role.Instance.Union.Member.MilitaryRanks.Imprl_Guard;
                            var member = user.Player.MyUnion.GetMemberByName(texts[0]);
                            if (member != null && member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                user.Send(stream.LeagueOptCreate(ActionID.AppointImperialGuard, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.AppointLeagueConcubines:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var member = user.Player.MyUnion.GetMemberByName(texts[0]);
                            if (member != null && member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {

                                if (Index >= 4010 && Index <= 4020)
                                    Index = 401;
                                if (Index >= 4020 && Index <= 4030)
                                    Index = 402;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Empress)
                                    user.Player.MyUnion.CheckUpHarem(member, (Role.Instance.Union.Member.MilitaryRanks)Index);


                                bool CanAdd = true;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.SeniorConc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 2;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Conc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 7;
                                if (CanAdd)
                                {
                                    user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                    user.Send(stream.LeagueOptCreate(ActionID.UppdateLeagueConcubines, 0, member.UID, (uint)member.Rank, member.Name));

                                }
                                else
                                {
                                    user.CreateBoxDialog("Sorry the " + ((Role.Instance.Union.Member.MilitaryRanks)Index).ToString() + " list is full.");
                                }
                            }
                        }
                        break;
                    }
                case ActionID.AppointHarem:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var member = user.Player.MyUnion.GetMemberByName(texts[0]);
                            if (member != null && member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Empress)
                                    user.Player.MyUnion.CheckUpHarem(member, (Role.Instance.Union.Member.MilitaryRanks)Index);

                                bool CanAdd = true;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.SeniorConc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 2;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Conc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 7;
                                if (CanAdd)
                                {
                                    user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);

                                    user.Send(stream.LeagueOptCreate(ActionID.UpdateHarem, 0, member.UID, (uint)member.Rank, member.Name));
                                }
                                else
                                {
                                    user.CreateBoxDialog("Sorry the " + ((Role.Instance.Union.Member.MilitaryRanks)Index).ToString() + " list is full.");
                                }
                            }
                        }
                        break;
                    }
                case ActionID.AppointCoreOffical:
                case ActionID.AppointCore:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var member = user.Player.MyUnion.GetMemberByName(texts[0]);
                            if (member != null && member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);

                                user.Send(stream.LeagueOptCreate(Action, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.CoreOfficials:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.UnionOfficials;
                            stream.LeagueImperialCourtListCreate(MsgLeagueImperialCourtList.ActionID.CoreOfficials, (byte)array.Length);
                            foreach (var member in array)
                            {
                                if (member.IsOnline)
                                    stream.AddItemLeagueImperialCourtList((uint)member.Owner.Player.BattlePower, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                                else
                                    stream.AddItemLeagueImperialCourtList(0, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                            }

                            user.Send(stream.LeagueImperialCourtListFinalize());
                        }
                        break;
                    }
                case ActionID.Tokens:
                    {
                        if (user.Player.InUnion)
                        {
                            const int count = 5;
                            MsgLeagueTokens.GroupToken GroupTokens = new MsgLeagueTokens.GroupToken();
                            GroupTokens.Tokens = new MsgLeagueTokens.Token[count];
                            GroupTokens.Tokens[0] = new MsgLeagueTokens.Token() { param1 = 101 };
                            GroupTokens.Tokens[1] = new MsgLeagueTokens.Token() { param1 = 102 };
                            GroupTokens.Tokens[2] = new MsgLeagueTokens.Token() { param1 = 103 };
                            GroupTokens.Tokens[3] = new MsgLeagueTokens.Token() { param1 = 205, param2 = 467887 };
                            GroupTokens.Tokens[4] = new MsgLeagueTokens.Token() { param1 = 206 };
                            user.Send(stream.CreateLeagueTokens(GroupTokens));
                        }
                        break;
                    }
                case ActionID.Officials:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.UnionOfficials;
                            stream.LeagueImperialCourtListCreate(MsgLeagueImperialCourtList.ActionID.Officials, (byte)array.Length);
                            foreach (var member in array)
                            {
                                if (member.IsOnline)
                                {
                                    stream.AddItemLeagueImperialCourtList((uint)member.Owner.Player.BattlePower, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                                }
                                else
                                {
                                    stream.AddItemLeagueImperialCourtList(0, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                                }
                            }

                            user.Send(stream.LeagueImperialCourtListFinalize());
                        }
                        break;
                    }
                case ActionID.UnionRank:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = MsgInterServer.Instance.Union.InfoUnions.Values.ToArray();
                            if (array.Length == 0)
                            {
                                // no cross-server link: list the local unions
                                array = Role.Instance.Union.GetOrderyUnions.Select(p => new MsgInterServer.Instance.Union()
                                {
                                    ServerID = Database.GroupServerList.MyServerInfo.ID,
                                    GoldBricks = p.GoldBrick,
                                    Name = p.Name,
                                    LeaderName = p.Emperor
                                }).ToArray();
                            }
                            const int max = 10;

                            int offset = (int)Index * max;
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));
                            stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)array.Length, (ushort)Index, 1, (ushort)count);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueRank(entity.ServerID, entity.GoldBricks, entity.Name, entity.LeaderName);
                            }
                            user.Send(stream.LeagueRankFinalize());
                        }
                        break;
                    }
                case ActionID.KingdomRank:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = Role.Instance.Union.Top3Unions;
                            stream.LeagueRankCreate(0, 3, 1, (uint)array.Length, 0);
                            for (int x = 0; x < array.Length; x++)
                                stream.AddItemLeagueRank(array[x].GoldBrick, array[x].Name, array[x].Emperor, 0);
                            user.Send(stream.LeagueRankFinalize());
                        }
                        break;
                    }
                case ActionID.ViewOthers:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;
                            var array = user.Player.MyUnion.GetOnlineMembers;
                            int offset = (int)Index * max;
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeagueMemListCreate(0, 0, (uint)array.Length, (ushort)count, 1);

                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueMemList(entity.Exploits, entity.Class, entity.Level, entity.Mesh, entity.IsOnline, entity.Name, (uint)Role.Instance.Union.Member.GetRank(entity.Rank), (uint)entity.NobilityRank, entity.UID, (ushort)(entity.IsOnline ? entity.Owner.Player.BattlePower : 0));
                            }
                            user.Send(stream.LeagueMemListFinalize());
                        }
                        break;
                    }

                case ActionID.Members:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;

                            Role.Instance.Guild Guild;
                            if (Role.Instance.Guild.GuildPoll.TryGetValue(ID, out Guild))
                            {
                                var array = Guild.GetOnlineMembers;

                                int offset = (int)Index * max;
                                int count = Math.Min(max, Math.Max(0, array.Length - offset));
                                stream.LeagueMemListCreate(Guild.Info.GuildID, Guild.GetGuildLeader.UID, (uint)array.Length, (ushort)count, 1);
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset].UnionMem;
                                    stream.AddItemLeagueMemList(entity.Exploits, entity.Class, entity.Level, entity.Mesh, entity.IsOnline, entity.Name, (uint)Role.Instance.Union.Member.GetRank(entity.Rank), (uint)entity.NobilityRank, entity.UID, (ushort)(entity.IsOnline ? entity.Owner.Player.BattlePower : 0));

                                }
                                user.Send(stream.LeagueMemListFinalize());
                            }
                        }
                        break;
                    }
                case ActionID.SynList:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 15;

                            var array = user.Player.MyUnion.GetOrderGuilds;
                            int offset = (int)Index * max;
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));
                            stream.LeagueSynListCreate(array.Length, (ushort)(count), 1);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueSynList(entity.Info.GuildID, (ulong)entity.Info.SilverFund, (ushort)entity.Members.Count, (ushort)entity.Info.Level, entity.GuildName, entity.Info.LeaderName);
                            }
                            user.Send(stream.LeagueSynListFinalize());
                        }
                        break;
                    }
                case ActionID.Announce:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var str = texts[0];
                            if (BaseFunc.NameStrCheck(str, false))
                            {
                                user.Player.MyUnion.Bulletin = str;
                            }
                            else
                            {
                                user.SendSysMesage("Invalid Announce charasters.");
                            }
                        }
                        break;
                    }
                case ActionID.Info:
                    {
                        if (user.Player.InUnion)
                        {
                            user.Send(stream.LeagueInfoCreate(user.Player.MyUnion.Treasury, user.Player.MyUnion.GoldBrick
                                , user.Player.MyUnion.Stipend, user.Player.MyUnion.Name, user.Player.MyUnion.Emperor
                                , user.Player.MyUnion.Bulletin, user.Player.MyUnion.Title, user.Player.MyUnion.PlunderTarget,
                                user.Player.MyUnion.InvadingUnion));
                        }
                        break;
                    }
                case ActionID.Create:
                    {
                        var name = texts[0];
                        if (user.Player.InUnion == false)
                        {
                            if (user.Player.MyGuild != null)
                            {
                                if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader && user.Player.MyGuild.Info.Level >= 9)
                                {
                                    if (user.Player.MyGuild.Info.ConquerPointFund >= 3000)
                                    {

                                        if (Role.Instance.Union.CheckName(name))
                                        {
                                            user.Player.MyGuild.Info.ConquerPointFund -= 3000;
                                            user.Player.MyUnion = Role.Instance.Union.Create(stream, user, name);
                                            user.Player.MyUnion.AddGuild(stream, user.Player.MyGuild);
                                        }
                                        else
                                            user.CreateBoxDialog("There is a Union Already with this Name i'm sorry.");
                                    }
                                    else
                                        user.CreateBoxDialog("Sorry, you don`t have 3000 CPs in guild to create a Union.");
                                }
                                else
                                    user.CreateBoxDialog("Sorry, you aren't a guild leader or your guild isn't LVL 9");
                            }
                            else
                                user.CreateBoxDialog("You Aren't in a Guild Yet.");
                        }
                        else
                            user.CreateBoxDialog("You Are Already in a Union");
                        break;
                    }
            }
        }
    }
}
