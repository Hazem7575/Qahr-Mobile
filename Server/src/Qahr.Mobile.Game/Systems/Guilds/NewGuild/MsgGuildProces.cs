using Qahr.Mobile.Database;
using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class MsgGuildProces
    {
        public enum GuildAction : uint
        {
            Accept = 1,
            Invite = 2,
            Quit = 3,
            InfoName = 6,
            Allied = 7,
            RemoveAlly = 8,
            Enemy = 9,
            RemoveEnemy = 10,
            SilverDonate = 11,
            Show = 12,
            Disband = 19,
            CpDonate = 20,
            RequestAllied = 23,
            Requirements = 24,
            Bulletin = 27,
            Promote = 28,
            ConfirmPromote = 29,
            Discharge = 30,
            Resign = 32,
            RequestPromote = 37,
            UpdatePromote = 38,
            Create = 51,
            Appoint = 53,
            Retire = 54,
            Join = 55,
        }

        public static void GuildRequest(this ServerSockets.Packet stream, out GuildAction requesttype, out uint UID, out int[] args, out string[] strlist)
        {
            requesttype = (GuildAction)stream.ReadInt32();
            UID = stream.ReadUInt32();
            args = new int[4];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = stream.ReadInt32();
            }
            strlist = stream.ReadStringList();
            stream.ReadBytes(3);
        }

        public static unsafe ServerSockets.Packet GuildRequestCreate(this ServerSockets.Packet stream, GuildAction requesttype, uint UID, int[] args, params string[] strlist)
        {
            stream.InitWriter();
            stream.Write((uint)requesttype);
            stream.Write(UID);
            stream.Write(args[0]);
            stream.Write(args[1]);
            stream.Write(args[2]);
            stream.Write(args[3]);
            stream.Write(strlist);
            stream.Finalize(GamePackets.MsgSyndicate);
            return stream;
        }

        [PacketAttribute(GamePackets.MsgSyndicate)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            GuildAction Action;
            uint UID;
            int[] args;
            string[] strlist;
            stream.GuildRequest(out Action, out UID, out args, out strlist);
            if (global::Qahr.Mobile.GuildRecruit.GuildApply.OnSyndicate(user, (uint)Action, UID, stream)) return; // [GuildRecruit 2026-10-14] join applications 55..59

            switch (Action)
            {
                case MsgGuildProces.GuildAction.Accept:
                    {
                        IMapObj mapObj;
                        if (user.Player.MyGuild != null || !user.Player.View.TryGetValue(UID, out mapObj, MapObjectType.Player))
                            break;
                        Client.GameClient client = (mapObj as Player).Owner;
                        if (client == null || client.Player.MyGuild == null || client.Player.MyGuildMember == null || (int)client.Player.TargetGuild != (int)user.Player.UID)
                            break;
                        client.Player.MyGuild.AddPlayer(user.Player, stream);

                        break;
                    }
                case MsgGuildProces.GuildAction.Invite:
                    {
                        IMapObj mapObj;
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader && user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.DeputyLeader || !user.Player.View.TryGetValue(UID, out mapObj, MapObjectType.Player))
                            break;
                        Client.GameClient client = (mapObj as Player).Owner;
                        if (client == null || client == null || client.Player.MyGuild != null || client.Player.MyGuildMember != null)
                            break;
                        user.Player.TargetGuild = client.Player.UID;
                        client.Send(stream.RelationCreate(user.Player, client.Player));
                        client.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Invite, user.Player.UID, args, strlist));
                        break;
                    }
                case MsgGuildProces.GuildAction.Quit:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        user.Player.MyGuild.Quit(user.Player.Name, false, stream);
                        break;
                    }
                case MsgGuildProces.GuildAction.InfoName:
                    {
                        Role.Instance.Guild guild1;
                        if (user.OnInterServer || !Role.Instance.Guild.GuildPoll.TryGetValue(UID, out guild1))
                            break;
                        user.Player.SendString(stream, MsgStringPacket.StringID.GuildName, guild1.Info.GuildID, true, guild1.GuildName + " " + guild1.Info.LeaderName + " " + guild1.Info.Level.ToString() + " " + guild1.Members.Count.ToString());
                        break;
                    }
                case MsgGuildProces.GuildAction.Allied:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        string str = strlist[0];
                        if (str == user.Player.MyGuild.GuildName)
                            break;
                        if (!user.Player.MyGuild.IsEnemy(str))
                        {
                            Role.Instance.Guild.Member leaderGuild = Role.Instance.Guild.GetLeaderGuild(str);
                            Client.GameClient Client;
                            if (leaderGuild != null && leaderGuild.IsOnline && Pool.GamePoll.TryGetValue(leaderGuild.UID, out Client))
                            {
                                Client.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RequestAllied, 0, new int[4], user.Player.MyGuild.GuildName));
                                user.Player.MyGuild.AddAlly(stream, str);
                            }
                        }
                        else
                            user.SendSysMesage("Sorry, this guild is in ennemy`s list");
                        break;
                    }
                case MsgGuildProces.GuildAction.RemoveAlly:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        user.Player.MyGuild.RemoveAlly(strlist[0], stream);
                        break;

                    }
                case MsgGuildProces.GuildAction.Enemy:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        string Name1 = strlist[0];
                        if (Name1 == user.Player.MyGuild.GuildName)
                            break;
                        if (user.Player.MyGuild.AllowAddAlly(Name1))
                        {
                            user.Player.MyGuild.AddEnemy(stream, Name1);
                            user.Player.MyGuild.SendMessajGuild("Guild Leader " + user.Player.Name + " has added Guild " + Name1 + " to the enemy list!");
                        }
                        else
                            user.SendSysMesage("Sorry, this guild is in Ally`s list");
                        break;
                    }
                case MsgGuildProces.GuildAction.RemoveEnemy:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        user.Player.MyGuild.RemoveEnemy(strlist[0], stream);
                        break;

                    }
                case MsgGuildProces.GuildAction.SilverDonate:
                    {
                        if (UID < 10000 || user.InTrade || user.Player.Money < (ulong)UID || user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        user.Player.Money -= (ulong)(uint)UID;
                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                        user.Player.MyGuildMember.MoneyDonate += (long)UID;
                        user.Player.MyGuild.Info.SilverFund += (long)UID;
                        user.Player.MyGuild.SendThat(user.Player);
                        user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_DONATE_MONEY@@" + user.Player.Name + "@@" + UID.ToString() + "@@");
                        if (global::Qahr.Mobile.Phone.MobileEvents.IsMobile(user)) user.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.SilverDonate, UID, new int[4]));// QahrMobile: the echo makes CMsgSyndicate::Process show the tip, count the daily attempt (CHero::SetSynDonateNum) and refresh the donation window
                        break;
                    }
                case MsgGuildProces.GuildAction.Show:
                    {
                        if (user.Player.MyGuild == null)
                            break;
                        user.Player.MyGuild.SendThat(user.Player);
                        break;
                    }
                case MsgGuildProces.GuildAction.CpDonate:
                    {
                        if (UID < 1 || user.InTrade || user.Player.ConquerPoints < UID || user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        user.Player.ConquerPoints -= (uint)UID;
                        user.Player.MyGuildMember.CpsDonate += UID;
                        user.Player.MyGuild.Info.ConquerPointFund += UID;
                        user.Player.MyGuild.SendThat(user.Player);
                        user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_DONATE_EMONEY@@" + user.Player.Name + "@@" + UID.ToString() + "@@");
                        if (global::Qahr.Mobile.Phone.MobileEvents.IsMobile(user)) user.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.CpDonate, UID, new int[4]));// QahrMobile: echo, as for the silver donation
                        break;
                    }
                case MsgGuildProces.GuildAction.RequestAllied:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        string Name2 = strlist[0];
                        if (Name2 == user.Player.MyGuild.GuildName)
                            break;
                        if (!user.Player.MyGuild.IsEnemy(Name2))
                            user.Player.MyGuild.AddAlly(stream, Name2);
                        else
                            user.SendSysMesage("Sorry, this guild is in ennemy`s list");
                        break;
                    }
                case MsgGuildProces.GuildAction.Requirements:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        byte[] numArray = new byte[stream.Size];
                        fixed (byte* numPtr = numArray)
                            stream.memcpy((void*)numPtr, (void*)stream.Memory, stream.Size);
                        user.Player.MyGuild.Info.Recruitment_Battle_Power = BitConverter.ToUInt32(numArray, 12);
                        user.Player.MyGuild.Info.Recruitment_Flag = (Role.Instance.Guild.ClassFlag)BitConverter.ToUInt32(numArray, 16);
                        user.Player.MyGuild.Info.RecruitmentOFF = numArray[20] == (byte)1;
                        user.Player.MyGuild.Info.RecruitmentON = numArray[21] == (byte)1;
                        break;
                    }
                case MsgGuildProces.GuildAction.Bulletin:
                    {
                        if (user.Player.MyGuild == null || user.Player.Name != user.Player.MyGuild.Info.LeaderName || strlist.Length == 0 || strlist[0] == null)
                            break;
                        if (BaseFunc.NameStrCheck(strlist[0], false))
                        {
                            user.Player.MyGuild.CreateBuletinTime();
                            user.Player.MyGuild.Bulletin = strlist[0];
                            user.Player.MyGuild.SendThat(user.Player);
                        }
                        else
                            user.SendSysMesage("Invalid Charasters in Bulletin.");
                        break;
                    }
                case MsgGuildProces.GuildAction.Promote:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        user.Player.MyGuild.Promote(UID, user.Player, strlist[0], stream);

                        break;
                    }
                case MsgGuildProces.GuildAction.Discharge:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        if (user.Player.InUnion)
                        {
                            user.CreateBoxDialog("Please quit union first time.");
                            break;
                        }
                        user.Player.MyGuild.Dismis(user, stream);
                        break;
                    }
                case MsgGuildProces.GuildAction.Resign:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        user.Player.MyGuild.Promote(6U, user.Player, user.Player.Name, stream);
                        break;
                    }
                case MsgGuildProces.GuildAction.RequestPromote:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;

                        var guild = user.Player.MyGuild;
                        uint targetUID = UID;
                        var targetMember = guild.Members.Values.FirstOrDefault(m => m.UID == targetUID);
                        if (targetMember == null)
                            break;
                        // target must be online and still in this guild
                        Client.GameClient targetClient;
                        if (targetMember.UID == user.Player.UID || !Pool.GamePoll.TryGetValue(targetMember.UID, out targetClient)
                            || targetClient.Player.MyGuild != guild)
                        {
                            SendPromote(stream, user, Action);
                            break;
                        }
                        var oldLeader = guild.Members.Values.FirstOrDefault(m => m.Rank == Role.Flags.GuildMemberRank.GuildLeader);

                        if (oldLeader != null && oldLeader.UID != targetMember.UID)
                        {
                            oldLeader.Rank = Role.Flags.GuildMemberRank.Member;
                            Client.GameClient oldClient;
                            if (Pool.GamePoll.TryGetValue(oldLeader.UID, out oldClient))
                            {
                                oldClient.Player.GuildRank = oldLeader.Rank;
                                oldClient.Player.View.SendView(oldClient.Player.GetArray(stream, false), false);
                                guild.SendThat(oldClient.Player);
                            }
                        }
                        targetMember.Rank = Role.Flags.GuildMemberRank.GuildLeader;
                        // keep the guild's leader fields in sync with the ranks
                        guild.Info.Leaderid = targetMember.UID;
                        guild.Info.LeaderName = targetMember.Name;
                        targetClient.Player.GuildRank = targetMember.Rank;
                        targetClient.Player.View.SendView(targetClient.Player.GetArray(stream, false), false);
                        guild.SendThat(targetClient.Player);
                        SendPromote(stream, user, Action);
                        break;
                    }
                case MsgGuildProces.GuildAction.Create:
                    {
                        if (user.Player.NobilityRank < Nobility.NobilityRank.Earl || user.Player.Money < 10000000 || !Role.Instance.Guild.AllowToCreate(strlist[0]))
                            break;
                        user.Player.Money -= 10000000;
                        Role.Instance.Guild Guild = new Role.Instance.Guild(user, strlist[0], stream);
                        break;
                    }
                case MsgGuildProces.GuildAction.Appoint:
                    {
                        Role.Instance.Guild.Member member1;
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || !user.Player.MyGuild.Members.TryGetValue(UID, out member1))
                            break;
                        Role.Instance.Guild.Member member2;
                        if (user.Player.MyGuild.CommandID > 0U && user.Player.MyGuild.Members.TryGetValue(user.Player.MyGuild.CommandID, out member2))
                            member2.Command = 0U;
                        user.Player.MyGuild.CommandID = member1.UID;
                        member1.Command = 1U;
                        user.Send(stream.GuildRequestCreate(Action, 0, new int[4], ((IEnumerable<string>)strlist).ToArray<string>()));
                        break;
                    }
                case MsgGuildProces.GuildAction.Retire:
                    {
                        Role.Instance.Guild.Member member3;
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader || !user.Player.MyGuild.Members.TryGetValue(UID, out member3))
                            break;
                        user.Player.MyGuild.CommandID = 0U;
                        member3.Command = 0U;
                        user.Send(stream.GuildRequestCreate(Action, 0, new int[4], ((IEnumerable<string>)strlist).ToArray<string>()));
                        break;
                    }
                case MsgGuildProces.GuildAction.Join:
                    {
                        Role.Instance.Guild Guild;
                        if (user.Player.MyGuild != null || user.Player.MyGuildMember != null || !Role.Instance.Guild.GuildPoll.TryGetValue(UID, out Guild))
                            break;
                        Guild.AddPlayer(user.Player, stream);
                        break;
                    }
            }
        }
        private static string CreatePromotion(StringBuilder builder, Role.Flags.GuildMemberRank rank, int occupants, uint maxOccupants, uint extraBattlePower, uint conquerPoints)
        {
            builder.Remove(0, builder.Length);
            builder.Append((int)rank);
            builder.Append(" ");
            builder.Append(occupants);
            builder.Append(" ");
            builder.Append(maxOccupants);
            builder.Append(" ");
            builder.Append(extraBattlePower);
            builder.Append(" ");
            builder.Append(conquerPoints);
            builder.Append(" ");
            return builder.ToString();
        }

        public static unsafe void SendPromote(ServerSockets.Packet stream, Client.GameClient client, GuildAction typ)
        {
            if (client.Player.MyGuild == null || client.Player.MyGuildMember == null)
                return;
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();

            Role.Instance.Guild guild = client.Player.MyGuild;
            uint arsenalBp = guild.Info.ArsenalBP;
            uint level1 = guild.Info.Level;
            syndicate_level.Level level2;
            if (syndicate_level._syndicate_level.TryGetValue(level1, out level2))
            {
                if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.GuildLeader, 1, 1, arsenalBp, 0));
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.DeputyLeader, guild.DeputyLeader_Count, level2.Max_DeputyLeader, arsenalBp, 0));
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.Manager, guild.Manager_Count, level2.Max_Manager, arsenalBp, 0));
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.Steward, guild.Steward_Count, level2.Max_Steward, arsenalBp, 0));
                }
                if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.DeputyLeader)
                {
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.Manager, guild.Manager_Count, level2.Max_Manager, arsenalBp, 0));
                    list.Add(MsgGuildProces.CreatePromotion(builder, Role.Flags.GuildMemberRank.Steward, guild.Steward_Count, level2.Max_Steward, arsenalBp, 0));
                }
            }
            int num = 0;
            foreach (string str in list)
                num += str.Length + 1;
            client.Send(stream.GuildRequestCreate(typ, 0, new int[4], list.ToArray()));
        }
    }
}
