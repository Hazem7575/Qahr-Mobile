using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class MsgTeam
    {
        public enum TeamTypes : uint
        {
            Create = 0,
            JoinRequest = 1,
            ExitTeam = 2,
            AcceptInvitation = 3,
            InviteRequest = 4,
            AcceptJoinRequest = 5,
            Dismiss = 6,
            Kick = 7,
            ForbidJoining = 8,
            UnforbidJoining = 9,
            LootMoneyOff = 10,
            LootMoneyOn = 11,
            LootItemsOff = 12,
            LootItemsOn = 13,
            RejectInvitation = 14,
            Send = 15,
            Send2 = 17,
        }

        public static unsafe void GetTeamPacket(this ServerSockets.Packet stream, out TeamTypes Typ, out uint UID)
        {
            Typ = (TeamTypes)stream.ReadUInt32();
            UID = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet TeamCreate(this ServerSockets.Packet stream, TeamTypes Typ, uint UID)
        {
            stream.InitWriter();
            stream.Write((uint)Typ);
            stream.Write(UID);

            stream.Finalize(GamePackets.MsgTeam);
            return stream;
        }

        [PacketAttribute(GamePackets.MsgTeam)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InQualifier())
            {
                return;
            }
            if (!user.Player.OnMyOwnServer)
                return;
            TeamTypes Typ;
            uint UID;
            stream.GetTeamPacket(out Typ, out UID);

            if (user.Player.Map == 2068 || user.Player.Map == 20081)
            {
                return;
            }
            if (user.Player.Map == 9855 || user.Player.InTeamPk)
            {
                return;
            }

            

            switch (Typ)
            {
                case TeamTypes.Send:
                    {
                        break;
                    }
                case TeamTypes.Create:
                    {
                        // leave the old team first so it isn't orphaned
                        if (user.Team != null)
                            user.Team.Remove(user, true);
                        user.Team = new Role.Instance.Team(user);
                        Typ = TeamTypes.Send;
                        user.Send(stream.TeamCreate(Typ, UID));
                        Typ = TeamTypes.Create;
                        user.Send(stream.TeamCreate(Typ, UID));
                        Typ = TeamTypes.Send2;
                        user.Send(stream.TeamCreate(Typ, UID));
                        user.HeroRewards.AddGoal(203);

                        break;
                    }
                case TeamTypes.Dismiss:
                    {
                        if (user.Team != null)
                        {
                            user.Team.Remove(user, false);
                        }
                        else
                        {
                            user.SendSysMesage("Sorry, you are not in team!");


                        }
                        break;
                    }
                case TeamTypes.Kick:
                    {
                        if (user.Team == null)
                        {

                            user.SendSysMesage("Sorry, you are not in team!");
                            break;
                        }
   
                        Client.GameClient Kicker;
                        if (user.Team.TryGetMember(UID, out Kicker))
                            user.Team.Remove(Kicker, false);
                        break;
                    }
                case TeamTypes.ExitTeam:
                    {
                        if (user.Team != null)
                        {
                            user.Team.Remove(user, true);
                        }
                        break;
                    }
                case TeamTypes.InviteRequest:
                    {
                        if (user.Team != null)
                        {
                            if (user.Team.CkeckToAdd() && user.Team.TeamLider(user))
                            {
                                Role.IMapObj Invitee;
                                if (user.Player.View.TryGetValue(UID, out Invitee, Role.MapObjectType.Player))
                                {
                                    Role.Player Inv = Invitee as Role.Player;
                                    if (Inv.Owner.Team == null)
                                    {
                                        Inv.Send(stream.RelationCreate(user.Player, Inv));

                                        UID = user.Player.UID;
                                        Phone.MobileTeam.SendTeam(Inv.Owner, stream, Typ, user);// QahrMobile: = TeamCreate(Typ, UID), plus the inviter name for a mobile receiver
                                        Phone.MobileOnly.SendTeamInvite(Inv.Owner, user, false);// QahrMobile: the mobile lists invitations from CMsgTeamInvite 10432
                                    }
                                    else
                                    {

                                        user.SendSysMesage(Inv.Name + " is already in a team.");


                                    }
                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.AcceptJoinRequest:
                    {
                        if (user.Team != null && user.Player.Alive)
                        {
                            if (user.Team.CkeckToAdd() && user.Team.TeamLider(user) && !user.Team.ForbidJoin)
                            {
                                Role.IMapObj obj;
                                if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                                {
                                    Client.GameClient NewTeammate = (obj as Role.Player).Owner;
                                    if (NewTeammate.Team != null)
                                    {

                                        NewTeammate.SendSysMesage("You are already in a team.");


                                        return;
                                    }
                                    NewTeammate.Team = user.Team;

                                    user.Team.Add(stream, NewTeammate);

                                }
                            }
                        }

                        break;
                    }
                case TeamTypes.AcceptInvitation:
                    {
                        if (user.Team == null && user.Player.Alive)
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Client.GameClient Leader = (obj as Role.Player).Owner;
                                if (Leader.Team != null)
                                {
                                    if (!Leader.Team.CkeckToAdd() || Leader.Team.ForbidJoin)
                                        return;

                                    user.Team = Leader.Team;
                                    Leader.Team.Add(stream, user);
                                }
                                else
                                {

                                    user.SendSysMesage(Leader.Player.Name + "'s doesn't have a team.");


                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.RejectInvitation:
                    {
                        // tell the inviter; UID is the inviter's UID
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Inviter = (obj as Role.Player).Owner;
                            if (Inviter != null)
                                Inviter.SendSysMesage(user.Player.Name + " rejected your team invitation.");
                        }
                        break;
                    }
                case TeamTypes.JoinRequest:
                    {
                        if (user.Team == null && user.Player.Alive)
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Client.GameClient Leader = (obj as Role.Player).Owner;
                                if (Leader.Team != null)
                                {
                                    if (Leader.Team.TeamLider(Leader) && Leader.Team.CkeckToAdd())
                                    {
                                        Leader.Send(stream.RelationCreate(user.Player, Leader.Player));

                                        UID = user.Player.UID;
                                        Phone.MobileTeam.SendTeam(Leader, stream, Typ, user);// QahrMobile: = TeamCreate(Typ, UID), plus the requester name for a mobile receiver
                                        Phone.MobileOnly.SendTeamInvite(Leader, user, true);// QahrMobile: the mobile lists join requests from CMsgTeamInvite 10432
                                    }
                                    else
                                    {

                                        user.SendSysMesage(Leader.Player.Name + "'s team is already full.");
    

                                    }
                                }
                                else if (Phone.MobileClient.Is(user))
                                {
                                    // QahrMobile: the mobile "Team Up" button sends JoinRequest also when the target has no
                                    // team (official server: make a team and invite). Create the team, then invite.
                                    Phone.MobileTeam.CreateAndInvite(user, Leader, stream);
                                }
                                else
                                {

                                    user.SendSysMesage(Leader.Player.Name + "'s doesn't have a team.");


                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.ForbidJoining:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.ForbidJoin = true;
                        }
                        break;
                    }
                case TeamTypes.UnforbidJoining:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.ForbidJoin = false;
                        }
                        break;
                    }
                case TeamTypes.LootMoneyOn:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupMoney = true;
                        }
                        break;
                    }
                case TeamTypes.LootMoneyOff:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupMoney = false;
                        }
                        break;
                    }
                case TeamTypes.LootItemsOn:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupItems = true;
                        }
                        break;
                    }
                case TeamTypes.LootItemsOff:
                    {
                        if (user.Team != null)
                        {
                            user.Team.SendTeam(stream.TeamCreate((TeamTypes)17, UID), 0);
                            user.Team.PickupItems = false;
                        }
                        break;
                    }
            }
        }
    }

}
