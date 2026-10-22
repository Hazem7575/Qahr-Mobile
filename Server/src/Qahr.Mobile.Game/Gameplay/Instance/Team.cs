using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Team
    {
        public enum TournamentProces : byte
        {
            None = 0,
            Winner = 1,
            Loser = 2
        }
        public enum StateType : byte
        {
            None = 0,
            FindMatch = 1,
            WaitForBox = 2,
            WaitForOther = 3,
            Fight = 4
        }
        public static System.Counter TeamCounter = new System.Counter(1);

        public unsafe class MemberInfo
        {
            public uint Index = 0;

            public bool Lider = false;
            public Client.GameClient client;
            public Game.MsgServer.TeamMemberInfo Info;
            public MemberInfo(Client.GameClient _client, Team _team)
            {
                _client.Team = _team;
                client = _client;
                Info = new Game.MsgServer.TeamMemberInfo();

                Info.Name = client.Player.Name;
                Info.MaxHitpoints = (ushort)Math.Min(ushort.MaxValue, client.Status.MaxHitpoints);
                Info.Mesh = client.Player.Mesh;
                Info.UID = client.Player.UID;
                Info.MinMHitpoints = (ushort)Math.Min(ushort.MaxValue, client.Player.HitPoints);
                Info.Frame = (ushort)client.Player.FrameID;
            }
        }
        public MemberInfo GetMember(uint UID)
        {
            MemberInfo member = null;
            Members.TryGetValue(UID, out member);
            return member;
        }

        public unsafe void SendTeamInfo(MemberInfo Member)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, new MemberInfo[] { Member });
                foreach (var TemmateMember in Temates)
                {
                    if (TemmateMember.client != null)
                    {
                        TemmateMember.client.Send(stream);
                    }
                }
            }
        }
        public bool ForbidJoin = false;
        public bool PickupMoney = true;
        public bool PickupItems = false;
        public bool AutoInvite = false;



        public Game.MsgTournaments.MsgTeamEliteGroup.FighterStats PKStats;
        public Game.MsgTournaments.MsgTeamEliteGroup.Match PkMatch;
        public System.Counter CounterMembers = new System.Counter(10);

        public uint Damage = 0;
        public TournamentProces Status;
        public uint Cheers = 0;
        public StateType ArenaState = StateType.None;
        public DateTime AcceptBoxShow = new DateTime();
        public bool AcceptBox = false;

        public void ResetTeamArena()
        {
            ArenaState = StateType.None;
            AcceptBox = false;
            Cheers = 0;
        }

        public DateTime InviteTimer = new DateTime();
        public DateTime UpdateLeaderLocationStamp = new DateTime();
        public List<uint> SendInvitation = new List<uint>();

        public Client.GameClient Leader;
        public uint UID;
        public ConcurrentDictionary<uint, MemberInfo> Members;
        public MemberInfo[] Temates { get { return Members.Values.ToArray(); } }

        public MemberInfo[] GetOrdonateMembers()
        {
            var array = Members.Values.OrderBy(p => p.Index).ToArray();
            return array;
        }


        public string TeamName;
        public IEnumerable<Client.GameClient> GetMembers()
        {
            foreach (var member in Temates)
            {
                yield return member.client;
            }
        }
      
        public bool IsDead(ushort Map)
        {
            int inMatchCount = Temates.Where(t => t.client.Player.InTeamPk).Count();
            int DeadCount = 0;

            foreach (var member in Temates)
            {
                if (!member.client.Player.Alive)
                    DeadCount++;
            }

            return inMatchCount != 0 && (inMatchCount == DeadCount);
        }
        public bool ReadyForTeamPK()
        {
            bool activ = Members.Count > 0 && Leader != null; //&& Leader.Socket.Alive;
            if (!activ)
                return false;
            foreach (var member in Temates)
            {
                if (member.client.Player.InTeamPk && member.client.Team != null)
                {
                    activ = true;
                    break;
                }
            }
            return activ;
        }
      
        public uint ProcentClanBp(uint Bp)
        {
            if (Bp == 1)
                return 40;
            if (Bp == 2)
                return 50;
            if (Bp == 3)
                return 60;
            if (Bp == 4)
                return 70;
            return 30;
        }

        public void ShareExperience(ServerSockets.Packet stream, Client.GameClient Killer, Game.MsgMonster.MonsterRole Target)
        {
            int Experience = Target.Family.MaxHealth / 50;

            if (IsTeamWithNewbie(Target))
                Experience *= 2;

            AwardMembersExp(stream, Killer, Experience);
        }


        private void AwardMembersExp(ServerSockets.Packet stream, Client.GameClient Killer, int nExp)
        {
            foreach (var user in Temates)
            {
                if (user.client.Player.UID == Killer.Player.UID)
                    continue;

                if (!user.client.Player.Alive)
                    continue;
                if (user.client.Player.Map != Killer.Player.Map)
                    continue;

                if (Core.GetDistance(user.client.Player.X, user.client.Player.Y, Killer.Player.X, Killer.Player.Y) > RoleView.ViewThreshold)
                    continue;


                if (user.client.Player.UID == Killer.Player.SpouseUID)
                    user.client.IncreaseExperience(stream, (double)(nExp * 2));
                else
                    user.client.IncreaseExperience(stream, (double)(nExp));


            }
        }

        public bool IsTeamWithNewbie(Game.MsgMonster.MonsterRole Target)
        {
            foreach (var user in Temates)
            {
                if (!user.client.Player.Alive)
                    continue;
                if (user.client.Player.Map != Target.Map)
                    continue;

                if (Core.GetDistance(user.client.Player.X, user.client.Player.Y, Target.X, Target.Y) > RoleView.ViewThreshold)
                    continue;

                if (user.client.Player.Level + 20 < Target.Level)
                    return true;
            }
            return false;
        }

        public Team(Client.GameClient owner)
        {

            Status = TournamentProces.None;
            Members = new ConcurrentDictionary<uint, MemberInfo>();
            UID = TeamCounter.Next;
            Leader = owner;
            TeamName = Leader.Player.Name;
            Members.TryAdd(owner.Player.UID, new MemberInfo(owner, this));
            AddLider();
        }
        public unsafe void AddLider()
        {

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();


                if (Temates.Length >= 1)
                {
                    var member = GetOrdonateMembers()[0];
                    if (member.client != null)
                    {
                        Leader = member.client;
                        TeamName = Leader.Player.Name;
                        member.Lider = true;
                        member.client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.TeamLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);


                        TeamLeadership action = new TeamLeadership()
                        {
                            UID = member.client.Player.UID,
                            LeaderUID = member.client.Player.UID,
                            Count = Members.Count,
                            Typ = MsgTeamLeadership.Mode.Leader
                        };
                        var pMembers = GetOrdonateMembers();
                        foreach (var TemmateMember in pMembers)
                        {
                            if (TemmateMember.client != null)
                            {
                                TemmateMember.client.Send(stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, pMembers));
                                TemmateMember.client.Send(stream.TeamLeadershipCreate(&action));
                            }
                        }

                        //Members.Values.ToArray();
                        foreach (var remover_members in pMembers)
                        {
                            if (remover_members.client.Player.UID != member.client.Player.UID)
                                Remove(remover_members.client, true);
                        }

                        foreach (var add_members in pMembers)
                            Add(stream, add_members.client);

                    }
                }
            }
        }
        public unsafe void Add(ServerSockets.Packet stream, Client.GameClient client)
        {
            
            if (CkeckToAdd())
            {
                MemberInfo member = new MemberInfo(client, this);
                member.Index = CounterMembers.Next;
                Members.TryAdd(client.Player.UID, member);

                TeamLeadership action = new TeamLeadership()
                {
                    Typ = MsgTeamLeadership.Mode.Teammate
                };
                client.Send(stream.TeamLeadershipCreate(&action));

                action = new TeamLeadership()
                {
                    Typ = MsgTeamLeadership.Mode.Leader,
                    UID = client.Player.UID,
                    LeaderUID = Leader.Player.UID,
                    Count = Members.Count
                };
                client.Send(stream.TeamLeadershipCreate(&action));

                action.UID = Leader.Player.UID;
                client.Send(stream.TeamLeadershipCreate(&action));
                Leader.Send(stream.TeamCreate(Game.MsgServer.MsgTeam.TeamTypes.Send2, Leader.Player.UID));
               
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, client.Player.UID, 1);

                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Team, UID);
                stream = upd.GetArray(stream);
                client.Send(stream);
                var pMembers = GetOrdonateMembers();
                stream = stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, pMembers);
                foreach (var TeamMember in pMembers)
                    TeamMember.client.Send(stream);
            }
            else
            {

                client.SendSysMesage("Sorry, the team is full!");

            }
        }
        public unsafe void Remove(Client.GameClient client, bool mode)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MemberInfo member = null;
                if (Members.TryGetValue(client.Player.UID, out member))
                {
                    var pMembers = GetOrdonateMembers();
                    foreach (var noob in pMembers)
                    {
                        noob.client.Send(stream.TeamCreate(mode ? Game.MsgServer.MsgTeam.TeamTypes.ExitTeam
                            : Game.MsgServer.MsgTeam.TeamTypes.Kick, member.client.Player.UID));
                    }
                    Members.TryRemove(client.Player.UID, out member);

                    TeamLeadership action = new TeamLeadership()
                    {
                        UID = member.client.Player.UID,
                        Count = Members.Count,
                        Typ = MsgTeamLeadership.Mode.Leader
                    };
                    member.client.Send(stream.TeamLeadershipCreate(&action));



                    Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, client.Player.UID, 1);
                    stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Team, 0);
                    stream = upd.GetArray(stream);
                    client.Send(stream);

                    if (member.Lider)
                    {
                        member.client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.TeamLeader);
                        AddLider();
                    }

                    member.client.Team = null;

                }
            }
        }

        public unsafe void SendFunc(Func<Client.GameClient, bool> pr, ServerSockets.Packet stream)
        {
            foreach (var user in Members.Values)
            {
                if (pr(user.client))
                    user.client.Send(stream);
            }
        }
        public bool TryGetMember(uint UID, out Client.GameClient client)
        {
            MemberInfo member = null;
            if (!Members.TryGetValue(UID, out member))
            {
                client = null;
                return false;
            }
            client = member.client;
            return true;
        }
        public void TeleportTeam(ushort map, ushort x, ushort y, uint dinamic = 0, Func<Client.GameClient, bool> pr = null)
        {
            foreach (var member in Temates)
            {
                if (pr != null)
                {
                    if (pr(member.client))
                    {
                        member.client.Teleport(x, y, map, dinamic);
                    }
                }
                else
                    member.client.Teleport(x, y, map, dinamic);
            }
        }
        public bool IsTeamMember(uint UID)
        {
            return Members.ContainsKey(UID);
        }
        public bool TeamLider(Client.GameClient client)
        {
            return client.Player.UID == Leader.Player.UID;
        }

        public unsafe void SendTeam(ServerSockets.Packet packet, uint UID)
        {
            foreach (var member in Temates)
            {
                if (member.client.Player.UID == UID)
                    continue;
                member.client.Send(packet);
            }
        }
        public void UpdatePlayers(Func<Client.GameClient, bool> pr, Action<Client.GameClient> handler)
        {
            foreach (var user in Members.Values)
            {
                if (pr == null)
                    handler.Invoke(user.client);
                else if (pr(user.client))
                {
                    handler.Invoke(user.client);
                }
            }

        }
        public unsafe void SendTeam(ServerSockets.Packet packet, uint UID, uint Map)
        {
            foreach (var member in Temates)
            {
                if (member.client.Player.UID == UID)
                    continue;
                if (member.client.Player.Map == Map)
                    member.client.Send(packet);
            }
        }
        public bool CkeckToAdd()
        {
            return Members.Count < 5;
        }

    }
}
