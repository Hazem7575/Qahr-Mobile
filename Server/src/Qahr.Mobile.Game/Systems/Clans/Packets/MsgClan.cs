using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static unsafe ServerSockets.Packet ClanMembersCreate(this ServerSockets.Packet stream, Role.Instance.Clan.Member[] Elements)
        {
            stream.InitWriter();
            stream.Write((uint)MsgClan.Info.Members);//4
            stream.Write((uint)0);//8
            stream.Write((uint)1);//12
            stream.Write((uint)Elements.Length);//16
            foreach (var member in Elements)
            {
                stream.Write(member.Name, 32);//20
                stream.Write((uint)member.Level);//52
                stream.Write((ushort)member.Rank);//56
                stream.Write((ushort)(member.Online ? 1 : 0));//58
                stream.Write(member.Class);//60
                // QahrMobile: the mobile FAM_MEMBER_INFO has a u32 at 64 and the donation as a u32 at 68
                stream.Write((uint)0);//64
                stream.Write((uint)member.Donation);//68
            }
            stream.Finalize(GamePackets.MsgFamily);
            return stream;
        }

        public static unsafe ServerSockets.Packet ClanBulletinCreate(this ServerSockets.Packet stream, Client.GameClient client, Role.Instance.Clan clan)
        {
            stream.InitWriter();

            stream.Write((uint)MsgClan.Info.SetAnnouncement);
            stream.Write(client.Player.ClanUID);
            stream.Write((uint)0);
            stream.Write(clan.ClanBuletin);

            stream.Finalize(GamePackets.MsgFamily);
            return stream;
        }

        public static unsafe ServerSockets.Packet ClanRelationCreate(this ServerSockets.Packet stream, uint ClanID, string ClanName, string ClanLeader, MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);
            stream.Write(ClanID);
            stream.Write((uint)0);
            stream.Write(ClanName, ClanLeader);
            stream.Finalize(GamePackets.MsgFamily);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanAppendSingleClientCreate(this ServerSockets.Packet stream, uint ClanID, string Name, MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);
            stream.Write(ClanID);
            stream.Write((uint)0);
            stream.Write(Name);
            stream.Finalize(GamePackets.MsgFamily);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanCreate(this ServerSockets.Packet stream, Client.GameClient client, Role.Instance.Clan clan)
        {

            stream.InitWriter();

            stream.Write((uint)MsgClan.Info.Info);//4
            stream.Write(client.Player.ClanUID);//8
            stream.Write((uint)1);//12
            stream.Write(new string[] 
            {
           
                string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
            client.Player.MyClan.ID,//0
            client.Player.MyClan.Members.Count,//1
            0,client.Player.MyClan.Donation,//2
            client.Player.MyClan.Level,//3
            (byte)client.Player.MyClan.Members[client.Player.UID].Rank,//4
            0,//5
            client.Player.MyClan.BP,//6
            0,//7
            0,//8
            0,//9
            client.Player.MyClan.LevelStore,
            client.Player.MyClan.Members[client.Player.UID].Donation), clan.Name,clan.LeaderName,
            string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",0,0,0,0,0,0,0,0,0,0),
                string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",0,0,0,0,0,0,0,0),
            });
            stream.Finalize(GamePackets.MsgFamily);

            return stream;
        }
        public static unsafe void GetClan(this ServerSockets.Packet stream, out MsgClan.Info Mode, out uint UID, out uint dwparam, out string[] list)
        {
            Mode = (MsgClan.Info)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            dwparam = stream.ReadUInt32();

            list = stream.ReadStringList();
        }
        public static unsafe ServerSockets.Packet ClanCallBackCreate(this ServerSockets.Packet stream, MsgClan.Info Mode, uint UID, uint dwparam, string[] list)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(UID);
            stream.Write(dwparam);

            if (list != null)
            {
                stream.Write(list);
            }

            stream.Finalize(GamePackets.MsgFamily);
            return stream;
        }
    }
    public unsafe struct MsgClan
    {
        public enum Info : uint
        {
            Info = 1,
            Members = 4,
            Recruit = 9,
            AcceptRecruit = 10,
            Join = 11,
            AcceptJoinRequest = 12,
            AddEnemy = 14,
            DeleteEnemy = 15,
            AddAlly = 19,
            RequestAlly = 17,
            AcceptAlly = 18,
            DeleteAlly = 20,
            TransferLeader = 21,
            KickMember = 22,
            Quit = 23,
            Announce = 24,
            SetAnnouncement = 25,
            Dedicate = 26,
            MyClan = 29,
            UpgradeLevel = 30,
            CreateClan = 33

        }

        [PacketAttribute(GamePackets.MsgFamily)]
        private static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.OnMyOwnServer)
                return;
            if (client.InTrade)
                return;

            if (client.Player.ObjInteraction != null)
                return;
            if (client.InTrade || client.IsVendor)
                return;
            MsgClan.Info Mode;
            uint UID;
            uint dwparam;
            string[] list;


            stream.GetClan(out Mode, out UID, out dwparam, out list);

            switch (Mode)
            {
                case Info.Info:
                    {
                        client.Send(stream.ClanCreate(client, client.Player.MyClan));
                        break;
                    }
                case Info.CreateClan:
                    {
                        if (client.Player.MyClan == null)
                        {
                            if (client.Player.Money >= 500000)
                            {
                                if (client.Player.Level >= 50)
                                {
                                    string Name = list[0];
                                    if (Role.Instance.Clan.AllowCreateClan(Name))
                                    {
                                        client.Player.Money -= 500000;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                        new Role.Instance.Clan().Create(client, Name, stream);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("The~name~has~been~used~or~you~haven`t~quit~a~Clan~for~at~least~a~day.");
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Info.UpgradeLevel:
                    {
                        if (UID == 1)
                        {
                            if (client.Player.MyClan == null)
                                break;
                            if (client.Player.MyClanMember == null) break;
                            uint Money = Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level];
                            if (client.Player.MyClan.Donation > Money)
                            {

                                client.Player.MyClan.Level++;
                                client.Player.MyClan.Donation -= Money;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);


                                client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));

                                client.Player.MyClan.SendThat(stream, client);
                            }
                        }
                        else if (UID == 2)
                        {
                            if (client.Player.MyClan == null)
                                break;
                            if (client.Player.MyClanMember == null) break;
                            uint Money = Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.BP++;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                                client.Player.MyClan.SendThat(stream, client);

                            }
                        }
                        else if (UID == 3)
                        {
                            if (client.Player.MyClanMember == null) break;
                            uint Money = Role.Instance.Clan.UpdateScoreLevel[client.Player.MyClan.LevelStore];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.LevelStore++;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                                client.Player.MyClan.SendThat(stream, client);

                            }
                        }
                        else if (UID == 4)
                        {

                        }
                        break;
                    }
                case Info.AcceptAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (UID == 1)
                        {
                            if (client.Player.MyClan.Ally.Count >= 5)
                            {
                                client.SendSysMesage("The amount of Allies have exceeded the maximum amount.");
                                break;
                            }
                            Role.Instance.Clan clan;
                            if (Role.Instance.Clan.Clans.TryGetValue(client.Player.MyClan.RequestAlly, out clan))
                            {
                                if (client.Player.MyClan.Ally.ContainsKey(clan.ID))
                                {
                                    client.SendSysMesage("This clan already is it on your Ally list .");
                                    break;
                                }
                                if (client.Player.MyClan.Enemy.ContainsKey(clan.ID))
                                {
                                    client.SendSysMesage("This clan already is on your Enemy list.");
                                    break;
                                }

                                if (client.Player.MyClan.Ally.TryAdd(clan.ID, clan))
                                {
                                    if (clan.Ally.TryAdd(client.Player.MyClan.ID, client.Player.MyClan))
                                    {
                                        client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.AddAlly));
                                        clan.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.AddAlly));
                                    }
                                }
                            }
                        }
                        else
                            client.Player.MyClan.RequestAlly = 0;
                        break;
                    }
                case Info.RequestAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (client.Player.MyClan.Ally.Count >= 5)
                        {
                            client.SendSysMesage("The amount of Allys have exceeded the maximum amount.");
                            break;
                        }

                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;
                            if (player.MyClan == null)
                                break;
                            if (player.MyClanMember == null) break;
                            if (player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                            if (client.Player.MyClan.Ally.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is it on your Ally list .");
                                break;
                            }
                            if (client.Player.MyClan.Enemy.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is on your Enemy list.");
                                break;
                            }

                            player.MyClan.RequestAlly = client.Player.MyClan.ID;


                            player.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.RequestAlly));

                        }
                        break;
                    }
                case Info.DeleteAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (client.Player.MyClan.IsAlly(Name))
                            {
                                Role.Instance.Clan clan;
                                if (client.Player.MyClan.Ally.TryRemove(client.Player.MyClan.GetClanAlly(Name), out clan))
                                {
                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, "", Info.DeleteAlly));

                                    Role.Instance.Clan ally_clan;
                                    if (clan.Ally.TryRemove(client.Player.MyClan.ID, out ally_clan))
                                    {
                                        clan.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, "", Info.DeleteAlly));
                                    }
                                }
                            }
                            else
                            {
                                client.SendSysMesage("This clan is not on your allies list .");
                            }
                        }
                        break;
                    }
                case Info.DeleteEnemy:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (client.Player.MyClan.IsEnemy(Name))
                            {
                                Role.Instance.Clan clan;
                                if (client.Player.MyClan.Enemy.TryRemove(client.Player.MyClan.GetClanEnemy(Name), out clan))
                                {
                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.DeleteEnemy));
                                }
                            }
                            else
                            {
                                client.SendSysMesage("This clan is not on your enemy list.");
                            }
                        }
                        break;
                    }
                case Info.AddEnemy:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (Name == client.Player.MyClan.Name)
                            {
                                client.SendSysMesage("You can't use your own clan name at enemy list.");
                                break;
                            }
                            if (client.Player.MyClan.Enemy.Count >= 5)
                            {
                                client.SendSysMesage("You can't have more than 5 Enemy's .");
                                break;
                            }
                            if (client.Player.MyClan.IsAlly(Name))
                            {
                                client.SendSysMesage("This clan belongs already to Ally's you can't add them to Enemys.");
                                break;
                            }
                            if (client.Player.MyClan.IsEnemy(Name))
                            {
                                client.SendSysMesage("This clan is already on your Enemy list.");
                                break;
                            }
                            Role.Instance.Clan clan;
                            if (client.Player.MyClan.TryGetClan(Name, out clan))
                            {
                                if (client.Player.MyClan.Enemy.TryAdd(clan.ID, clan))
                                {

                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.AddEnemy));
                                }
                            }
                        }
                        break;
                    }
                case Info.KickMember:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            client.Player.MyClan.RemoveMember(Name, stream);
                            goto case Info.Members;
                        }
                        break;
                    }
                case Info.AcceptRecruit:
                    {
                        if (list.Length > 0)
                        {
                            if (client.Player.MyClan != null) break;
                            if (client.Player.MyClanMember != null) break;

                            Role.IMapObj obj;
                            if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                if (obj == null) break;
                                Role.Player Leader = obj as Role.Player;
                                if (Leader == null) break;
                                if (Leader.MyClan == null) break;
                                if (Leader.MyClanMember == null) break;
                                if (Leader.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                                if (Leader.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(Leader.MyClan.Level))
                                {
                                    Leader.Owner.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(Leader.MyClan.Level) + ".");
                                    break;
                                }

                                Leader.MyClan.AddMember(client, Role.Instance.Clan.Ranks.Member, stream);
                            }
                        }
                        break;
                    }
                case Info.Recruit:
                    {
                        if (client.Player.MyClan == null) break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (client.Player.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                        {
                            client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                            break;
                        }
                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            if (obj == null) break;
                            Role.Player recuiter = obj as Role.Player;
                            if (recuiter == null) break;
                            if (recuiter.MyClan != null) break;
                            if (recuiter.MyClanMember != null) break;

                            recuiter.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.Name, Info.Recruit));
                        }
                        break;
                    }
                case Info.AcceptJoinRequest:
                    {
                        if (list.Length > 0)
                        {
                            if (client.Player.MyClan == null) break;
                            if (client.Player.MyClanMember == null) break;
                            if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                            if (client.Player.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                            {
                                client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                                break;
                            }

                            Role.IMapObj obj;
                            if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                if (obj == null) break;
                                Role.Player member = obj as Role.Player;
                                if (member == null) break;
                                if (member.MyClan != null) break;
                                if (member.MyClanMember != null) break;

                                client.Player.MyClan.AddMember(member.Owner, Role.Instance.Clan.Ranks.Member, stream);

                            }
                        }
                        else
                        {

                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        }
                        break;
                    }
                case Info.Join:
                    {
                        if (client.Player.MyClan != null) break;
                        if (client.Player.MyClanMember != null) break;
                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            if (obj == null) break;
                            Role.Player Leader = obj as Role.Player;
                            if (Leader == null) break;
                            if (Leader.MyClan == null) break;
                            if (Leader.MyClanMember == null) break;
                            if (Leader.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                            Leader.Owner.Send(stream.ClanAppendSingleClientCreate(client.Player.UID, client.Player.Name, Info.Join));
                        }

                        break;
                    }
                case Info.TransferLeader:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (Name == client.Player.Name)
                                break;
                            Role.Instance.Clan.Member member;
                            if (client.Player.MyClan.TryGetMember(Name, out member))
                            {
                                Client.GameClient pClient;
                                if (Pool.GamePoll.TryGetValue(member.UID, out pClient))
                                {
                                    if (pClient.Player.MyClan == null) break;
                                    if (pClient.Player.MyClanMember == null) break;
                                    if (pClient.Player.ClanUID != client.Player.ClanUID) break;
                                    pClient.Player.MyClan.LeaderName = pClient.Player.Name;
                                    pClient.Player.ClanRank = (ushort)Role.Instance.Clan.Ranks.Leader;
                                    pClient.Player.MyClanMember.Rank = Role.Instance.Clan.Ranks.Leader;
                                    pClient.Player.MyClan.SendThat(stream, pClient);

                                    client.Player.MyClanMember.Rank = Role.Instance.Clan.Ranks.Member;
                                    client.Player.ClanRank = (ushort)Role.Instance.Clan.Ranks.Member;
                                    client.Player.MyClan.SendThat(stream, client);

                                    client.Player.View.ReSendView(stream);
                                    pClient.Player.View.ReSendView(stream);
                                }
                            }
                        }
                        break;
                    }
                case Info.Quit:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        client.Player.MyClan.RemoveMember(client);

                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        break;
                    }
                case Info.Dedicate:
                    {
                        if (client.InTrade)
                            return;

                        if (client.Player.Map == 3852)
                            return;
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        ulong Amount = (uint)UID;
                        if (client.Player.Money >= Amount)
                        {
                            client.Player.Money -= Amount;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.MyClan.Donation += (uint)Amount;
                            client.Player.MyClanMember.Donation += (uint)Amount;

                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));

                            client.Player.MyClan.SendThat(stream, client);
                        }
                        break;
                    }
                case Info.SetAnnouncement:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (list.Length > 0)
                        {
                            string Name = list[0];

                            if (BaseFunc.NameStrCheck(Name))
                            {
                                client.Player.MyClan.ClanBuletin = Name;
                                client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                            }
                            else
                            {
                                client.SendSysMesage("Your buletin requires alpha-numeric characters (a-z/0-9).");
                            }
                        }
                        break;
                    }
                case Info.Members:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        Role.Instance.Clan.Member[] members = client.Player.MyClan.Members.Values.ToArray();
                        Array.Sort(members, (c1, c2) =>
                        {
                            int val = c2.Online.CompareTo(c1.Online);
                            if (c1.Online == c2.Online)
                            {
                                val = c1.Rank.CompareTo(c2.Rank);
                                if (c1.Rank == c2.Rank)
                                    val = c1.Level.CompareTo(c2.Level);
                            }
                            return val;
                        });
                        client.Send(stream.ClanMembersCreate(members));
                        break;
                    }
                case Info.Announce:
                    {
                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        break;
                    }
                case Info.MyClan:
                    {
                        if (client.Player.MyClan != null)
                        {
                            try
                            {
                                client.Send(stream.ClanCreate(client, client.Player.MyClan));
                                string ExtraInfo = "0 0 0 0 0 0 0";
                                //var war = Game.MsgTournaments.MsgSchedules.ClanWar.GetWinnerWar(client.Player.ClanUID);
                               // if (war != null && war.Proces == MsgTournaments.ProcesType.Dead)
                               // {
                               //     ExtraInfo = "1 " + war.Winner.OccupationDays + " "
                               //+ war.Winner.Reward + " " + war.Winner.NextReward + " 0 0 0";
                               // }
                                client.Send(stream.ClanAppendSingleClientCreate(client.Player.MyClan.ID, ExtraInfo/*for clan war*/, Info.MyClan));
                                client.Player.MyClan.SendBuletin(stream, client);
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        }
                        break;
                    }
            }
        }

    }
}
