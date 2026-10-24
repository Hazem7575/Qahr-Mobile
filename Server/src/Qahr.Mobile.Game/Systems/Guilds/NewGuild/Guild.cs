using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgTournaments;

namespace Qahr.Mobile.Role.Instance
{
    public class Guild
    {
        public class Construct
        {
            public uint ID;

            public uint Level;

            public ulong Exp;

            public ulong Beast;

            public ulong v2;

            public ulong v3;

            public ulong v4;

            public syn_formtype.formtype Data
            {
                get
                {
                    syn_formtype.formtype form;
                    return syn_formtype.TryGetValue((uint)(ID * 100 + Beast), Level, out form) ? form : new syn_formtype.formtype();
                }
            }

            public Construct(uint id, uint level, ulong beast, ulong _Exp = 0uL, ulong _v2 = 0uL, ulong _v3 = 0uL, ulong _v4 = 0uL)
            {
                ID = id;
                Level = level;
                Exp = _Exp;
                Beast = beast;
                v2 = _v2;
                v3 = _v3;
                v4 = _v4;
            }
        }

        public enum ClassFlag : uint
        {
            None = 0u,
            Trojan = 2u,
            Warrior = 4u,
            DuneWanderer = 8u,
            Archer = 16u,
            Ninja = 32u,
            Monk = 64u,
            Pirate = 128u,
            DragonWarrior = 256u,
            Thunderstriker = 512u,
            Water = 8192u,
            Fire = 16384u,
            Windwalker = 65536u,
            Full = 91134u,
        }
        public uint CommandID;
        public List<Guild.GuildMessage> Message = new List<Guild.GuildMessage>();
        public ConcurrentDictionary<uint, Guild.Construct> Constructs = new ConcurrentDictionary<uint, Guild.Construct>();
        public class UpdateDB
        {

            public uint UID;
            public uint GuildID;
            public Flags.GuildMemberRank Rank;
        }
        public class Member
        {

            public Union.Member UnionMem;
            public uint UID;
            public string Name;
            public Flags.GuildMemberRank Rank;
            public uint Level;
            public uint Class;
            public uint NobilityRank;
            public long MoneyDonate;
            public uint CpsDonate;
            public uint VirtutePointes;
            public uint Mesh;
            public uint PrestigePoints = 0;
            public uint BattlePower = 0;
            public uint Command;
            public uint ArsenalDonation;
            public uint PkDonation;
            public bool IsOnline = false;
            public long LastLogin = 0;
            public uint CTF_Exploits = 0;
            public uint RewardConquerPoints = 0;
            public uint RewardMoney = 0;
            public byte CTF_Claimed = 0;
            public uint GuildWarPoints;
            public byte Graden
            {
                get
                {
                    byte num = 2;
                    if (Mesh % 10 == 8)
                        num = (byte)1;
                    return num;
                }
            }

            public uint TotalDonation
            {
                get
                {

                    return (uint)(VirtutePointes + (uint)MoneyDonate + ArsenalDonation + PkDonation);
                }
            }


        }
        public class GuildMessage
        {
            public uint Time;
            public string message;
        }
        public void AddMessage(string Messaj)
        {
            Guild.GuildMessage guildMessage = new Guild.GuildMessage();
            DateTime dateTime = new DateTime(1970, 1, 1);
            guildMessage.Time = (uint)(DateTime.UtcNow - dateTime).TotalSeconds;
            guildMessage.message = Messaj;
            Message.Add(guildMessage);
        }
        public Member GetGuildLeader
        {

            get
            {
                return Members.Values.Where(p => p.Rank == Flags.GuildMemberRank.GuildLeader).FirstOrDefault();
            }
        }
        // public uint GuildWarPoints
        //{
        // get
        //{
        //  uint point = 0;
        // if (MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.RightGate].Npc.GuildID == Info.GuildID)
        //  point += 500;
        // if (MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.LeftGate].Npc.GuildID == Info.GuildID)
        //  point += 500;
        // if (MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.TenacityPillar].Npc.GuildID == Info.GuildID)
        // point += 500;
        // if (MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.ExorcimPillar].Npc.GuildID == Info.GuildID)
        // point += 500;

        //if (MsgSchedules.NEWGUILDWAR.NPC_INFO.ContainsKey(MsgGuildWar.NPCID.GuildWithHonorPillar))
        // {
        // if (MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.GuildWithHonorPillar].Npc.GuildID == Info.GuildID)
        //         point += 3000;
        // }
        // if (MsgSchedules.NEWGUILDWAR.Winner.GuildID == Info.GuildID)
        // point += (uint)((double)((MsgSchedules.NEWGUILDWAR.Winner.Score * 100) / (uint)MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.GuildPillar].Npc.MaxHitPoints)) * 2000 / 100;
        // var Array = MsgSchedules.NEWGUILDWAR.SCORE_LIST.Values.ToArray();
        // foreach (var obj in Array)
        //{
        // if (obj.GuildID == Info.GuildID)
        //  point += (uint)((double)((obj.Score * 100) / (uint)MsgSchedules.NEWGUILDWAR.NPC_INFO[MsgGuildWar.NPCID.GuildPillar].Npc.MaxHitPoints)) * 2000 / 100;
        //  }
        // foreach (var user in Members.Values)
        //  point += user.GuildWarPoints;
        // return point;
        //  }
        //}
        public static System.Counter Counter = new System.Counter(1000);
        public static ConcurrentDictionary<uint, Guild> GuildPoll = new ConcurrentDictionary<uint, Guild>();
        public Game.MsgServer.MsgGuildInformation Info;
        public ConcurrentDictionary<uint, Guild.Member> Members;
        public ConcurrentDictionary<uint, Guild> Ally;
        public ConcurrentDictionary<uint, Guild> Enemy;
        public uint ClaimCtfReward = 0;
        public uint CTF_Exploits = 0;
        public uint CTF_Next_ConquerPoints = 0;
        public uint CTF_Next_Money = 0;
        public uint CTF_Rank = 0;
        public uint[] GetLeaderReward()
        {
            // [OfficialFix 2026-10-14] the CTF NPC no longer pays leaders free CPs (500k/400k/300k was not official): the panel shows none
            uint[] wReward = new uint[2];
            return wReward;
        }
        public uint GuildID = 0;
        public uint UnionID;
        public Union GetUnion
        {
            get
            {
                Union union;
                return Union.UnionPoll.TryGetValue(UnionID, out union) ? union : (Union)null;
            }
        }
        public Member[] GetOnlineMembers
        {
            get { return Members.Values.OrderByDescending(p => p.IsOnline).ToArray(); }
        }
        public int DeputyLeader_Count
        {
            get
            {
                return Members.Values.Where<Guild.Member>((Func<Guild.Member, bool>)(p => p.Rank == Role.Flags.GuildMemberRank.DeputyLeader)).ToList<Guild.Member>().Count;
            }

        }
        public int Manager_Count
        {
            get
            {
                return Members.Values.Where<Guild.Member>((Func<Guild.Member, bool>)(p => p.Rank == Role.Flags.GuildMemberRank.Manager)).ToList<Guild.Member>().Count;
            }
        }
        public int Steward_Count
        {
            get
            {
                return Members.Values.Where<Guild.Member>((Func<Guild.Member, bool>)(p => p.Rank == Role.Flags.GuildMemberRank.Steward)).ToList<Guild.Member>().Count;
            }
        }
        public string Bulletin = "None";
        public string GuildName = "";
        public ushort[] RanksCounts = new ushort[(ushort)Flags.GuildMemberRank.GuildLeader + 1];
        public bool CanSave = true;

        public Guild(Client.GameClient client, string Name, ServerSockets.Packet stream)
        {
            Info = MsgGuildInformation.Create();
            Members = new ConcurrentDictionary<uint, Guild.Member>();
            Ally = new ConcurrentDictionary<uint, Guild>();
            Enemy = new ConcurrentDictionary<uint, Guild>();
            GuildName = Name;
            Constructs = new ConcurrentDictionary<uint, Guild.Construct>();
            Guild.Construct construct1 = new Guild.Construct(1, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct1.ID * 100) + construct1.Beast), construct1);
            Guild.Construct construct2 = new Guild.Construct(2, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct2.ID * 100) + construct2.Beast), construct2);
            Guild.Construct construct3 = new Guild.Construct(2, 1, 1);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct3.ID * 100) + construct3.Beast), construct3);
            Guild.Construct construct4 = new Guild.Construct(2, 1, 2);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct4.ID * 100) + construct4.Beast), construct4);
            Guild.Construct construct5 = new Guild.Construct(3, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct5.ID * 100) + construct5.Beast), construct5);
            Guild.Construct construct6 = new Guild.Construct(3, 1, 1);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct6.ID * 100) + construct6.Beast), construct6);
            Guild.Construct construct7 = new Guild.Construct(3, 1, 2);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct7.ID * 100) + construct7.Beast), construct7);
            Guild.Construct construct8 = new Guild.Construct(3, 1, 3);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct8.ID * 100) + construct8.Beast), construct8);
            Guild.Construct construct9 = new Guild.Construct(3, 1, 4);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct9.ID * 100) + construct9.Beast), construct9);
            Guild.Construct construct10 = new Guild.Construct(3, 1, 5);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct10.ID * 100) + construct10.Beast), construct10);
            Guild.Construct construct11 = new Guild.Construct(3, 1, 6);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct11.ID * 100) + construct11.Beast), construct11);
            Guild.Construct construct12 = new Guild.Construct(4, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct12.ID * 100) + construct12.Beast), construct12);
            Guild.Construct construct13 = new Guild.Construct(5, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct13.ID * 100) + construct13.Beast), construct13);
            Guild.Construct construct14 = new Guild.Construct(6, 1, 0);
            Constructs.Add<uint, Guild.Construct>((uint)((ulong)(construct14.ID * 100) + construct14.Beast), construct14);
            Info.Recruitment_Flag = Guild.ClassFlag.Full;
            if (client == null)
                return;
            DateTime now = DateTime.Now;
            Info.LeaderName = client.Player.Name;
            Info.Leaderid = client.Player.UID;
            Info.GuildID = Guild.Counter.Next;
            Info.CreateTime = (uint)GetTime(now.Year, now.Month, now.Day);
            Info.Level = (uint)1;
            Info.ArsenalBP = (uint)1;
            AddPlayer(client.Player, stream);
            Guild.GuildPoll.TryAdd(Info.GuildID, this);
        }

        public unsafe void AddPlayer(Role.Player player, ServerSockets.Packet stream)
        {
            if ((long)player.BattlePower < (long)Info.Recruitment_Battle_Power || !Profession(player.Class) || player.MyGuild != null)
                return;
            if (player.MyGuild == null)
            {
                Member memb = new Member();
                if (player.MyUnion != null)
                {
                    if (player.MyUnion.UID == UnionID)
                    {
                        Role.Instance.Union.Member member;
                        player.MyUnion.Members.TryRemove(player.UID, out member);
                    }
                    else
                    {
                        player.MyUnion.Quit(stream, player.UnionMemeber);
                        if (GetUnion != null)
                        {
                            memb.UnionMem = Role.Instance.Union.Member.CreateMember(player.Owner, Union.Member.MilitaryRanks.Member);
                            memb.UnionMem.Owner = player.Owner;
                            player.MyUnion = GetUnion;
                            player.UnionMemeber = memb.UnionMem;
                        }
                    }
                }
                else
                {
                    if (GetUnion != null)
                    {
                        memb.UnionMem = Role.Instance.Union.Member.CreateMember(player.Owner, Union.Member.MilitaryRanks.Member);
                        memb.UnionMem.Owner = player.Owner;
                        player.MyUnion = GetUnion;
                        player.UnionMemeber = memb.UnionMem;
                    }
                }

                memb.IsOnline = true;
                memb.Name = player.Name;
                memb.Rank = Members.Count != 0 ? Role.Flags.GuildMemberRank.Member : Role.Flags.GuildMemberRank.GuildLeader;
                memb.UID = player.UID;
                memb.Level = (uint)player.Level;
                memb.NobilityRank = (uint)player.NobilityRank;
                if (Members.Count == 0)
                    Info.SilverFund = memb.MoneyDonate = 5000000;
                memb.Class = player.Class;
                memb.Mesh = player.Mesh;
                Members.TryAdd(memb.UID, memb);
                player.GuildID = Info.GuildID;
                player.GuildRank = memb.Rank;
                player.MyGuild = this;
                player.MyGuildMember = memb;
                Info.MyRank = (uint)memb.Rank;
                if (!Program.ServerConfig.IsInterServer && !player.Owner.OnInterServer)
                {
                    SendThat(player);
                    player.View.SendView(player.GetArray(stream, false), false);

                    player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildName, Info.GuildID, true, new string[1] { GuildName + " " + Info.LeaderName + " " + Info.Level + " " + Members.Count });
                }
                player.GuildBattlePower = ShareMemberPotency(memb.Rank);
                if (GetUnion != null)
                {
                    memb.UnionMem = Union.Member.CreateMember(memb);
                    GetUnion.SendMyInfo(stream, player.Owner);
                }
                AddMessage("STR_SYNDICATE_EVENT_MEMBER_JOIN@@" + memb.Name + "@@");



            }
        }
        public bool ContainsFlag(Guild.ClassFlag flag)
        {
            return (long)((ulong)Info.Recruitment_Flag & (ulong)~(uint)flag) != (long)flag;
        }
        public bool Profession(uint profession)
        {
            if (profession >= 1000 && profession <= 1057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Trojan))
                    return false;
            }
            else if (profession >= 2000 && profession <= 2057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Warrior))
                    return false;
            }
            else if (profession >= 3000 && profession <= 3057)
            {
                if (!ContainsFlag(Guild.ClassFlag.DuneWanderer))
                    return false;
            }
            else if (profession >= 4000 && profession <= 4057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Archer))
                    return false;
            }
            else if (profession >= 5000 && profession <= 5057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Ninja))
                    return false;
            }
            else if (profession >= 6000 && profession <= 6057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Monk))
                    return false;
            }
            else if (profession >= 7000 && profession <= 7057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Pirate))
                    return false;
            }
            else if (profession >= 8000 && profession <= 8057)
            {
                if (!ContainsFlag(Guild.ClassFlag.DragonWarrior))
                    return false;
            }
            else if (profession >= 9000 && profession <= 9057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Thunderstriker))
                    return false;
            }
            else if (profession >= 13002 && profession <= 13057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Water))
                    return false;
            }
            else if (profession >= 14002 && profession <= 14057)
            {
                if (!ContainsFlag(Guild.ClassFlag.Fire))
                    return false;
            }
            else if (profession < 16000 || profession > 16057 || !ContainsFlag(Guild.ClassFlag.Fire))
                return false;
            return true;
        }
        public uint ShareMemberPotency(Role.Flags.GuildMemberRank RankMember)
        {
            if (Info.ArsenalBP > 15)
                Info.ArsenalBP = 15;
            return Info.ArsenalBP;
        }
        public void Promote(uint rank, Role.Player owner, string Name, ServerSockets.Packet stream)
        {
            Guild.Member member1 = this.GetMember(Name);
            syndicate_level.Level level;
            if (member1 == null || !syndicate_level._syndicate_level.TryGetValue(this.Info.Level, out level) || rank == 2 && (long)level.Max_DeputyLeader == (long)this.DeputyLeader_Count || rank == 4 && (long)level.Max_Steward == (long)this.Steward_Count || rank == 3 && (long)level.Max_Manager == (long)this.Manager_Count)
                return;
            Client.GameClient gameClient1;
            if (Pool.GamePoll.TryGetValue(member1.UID, out gameClient1))
            {
                byte gc2 = 0;
                Guild.Member member2;
                Client.GameClient gameClient2;
                if (rank == 1 && this.Members.TryGetValue(this.Info.Leaderid, out member2) && Pool.GamePoll.TryGetValue(member2.UID, out gameClient2))
                {
                    if (gameClient1 == null || gameClient2 == null) { gc2 = 1; return; }
                    member2.Rank = Role.Flags.GuildMemberRank.DeputyLeader;
                    this.Info.Leaderid = member1.UID;
                    this.Info.LeaderName = member1.Name;
                    gameClient2.Player.GuildRank = member2.Rank;
                    gameClient2.Player.View.SendView(gameClient2.Player.GetArray(stream, false), false);
                    this.SendThat(gameClient2.Player);
                }
                if (gameClient1 == null || gc2 == 1) return;
                member1.Rank = (Role.Flags.GuildMemberRank)rank;
                gameClient1.Player.GuildRank = member1.Rank;
                gameClient1.Player.View.SendView(gameClient1.Player.GetArray(stream, false), false);
                this.SendThat(gameClient1.Player);
            }
            else
            {
                Guild.Member member2;
                Client.GameClient gameClient2;
                if (rank == 1 && this.Members.TryGetValue(this.Info.Leaderid, out member2) && Pool.GamePoll.TryGetValue(member2.UID, out gameClient2))
                {

                    member2.Rank = Role.Flags.GuildMemberRank.DeputyLeader;
                    this.Info.Leaderid = member1.UID;
                    this.Info.LeaderName = member1.Name;
                    gameClient2.Player.GuildRank = member2.Rank;
                    gameClient2.Player.View.SendView(gameClient2.Player.GetArray(stream, false), false);
                    this.SendThat(gameClient2.Player);
                }
                member1.Rank = (Role.Flags.GuildMemberRank)rank;
                ServerDatabase.LoginQueue.TryEnqueue((object)member1);
            }

        }
        public bool AllowAddAlly(string Name)
        {
            if (IsEnemy(Name))
            {
                return false;
            }
            foreach (Guild all in Ally.Values)
                if (all.GuildName == Name) return false;
            return true;
        }
        public bool AllowAddEnemy(string name)
        {
            foreach (Guild all in Enemy.Values)
                if (all.GuildName == name) return false;
            return true;
        }
        public bool IsEnemy(string Name)
        {
            foreach (Guild gui in Enemy.Values)
                if (gui.GuildName == Name)
                    return true;
            return false;
        }
        public static Member GetLeaderGuild(string guildname)
        {
            foreach (var obj in GuildPoll.Values)
            {
                if (obj.GuildName == guildname)
                {
                    return obj.GetGuildLeader;
                }
            }
            return null;
        }
        public static bool AllowCreateGuild(string Name)
        {
            if (!BaseFunc.NameStrCheck(Name))
                return false;
            foreach (var nname in ChangeNameRecords.Values)
                if (nname == Name)
                    return false;
            foreach (var guild in GuildPoll.Values)
                if (guild.GuildName == Name)
                    return false;
            return true;
        }
        public uint AllowAlly = 0;
        public bool AddAlly(ServerSockets.Packet stream, string Name)
        {
            if (IsEnemy(Name))
            {
                return false;
            }
            if (Ally.Count >= 15)
                return false;
            if (!AllowAddAlly(Name))
                return false;
            Guild GuildAlly = null;
            foreach (Guild gui in GuildPoll.Values)
                if (gui.GuildName == Name)
                {
                    GuildAlly = gui;
                    break;
                }
            if (GuildAlly != null)
            {
                AllowAlly = GuildAlly.Info.GuildID;
                if (GuildAlly.AllowAlly == Info.GuildID)
                {
                    GuildAlly.Ally.TryAdd(Info.GuildID, this);
                    Ally.TryAdd(GuildAlly.Info.GuildID, GuildAlly);
                    GuildAlly.SendGuildAlly(stream, false, null);
                    SendGuildAlly(stream, false, null);
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        public void AddEnemy(ServerSockets.Packet stream, string Name)
        {
            if (Enemy.Count >= 15) return;
            if (AllowAddEnemy(Name))
            {
                Guild GuildEnnemy = null;
                foreach (Guild gui in GuildPoll.Values)
                    if (gui.GuildName == Name)
                    {
                        GuildEnnemy = gui;
                        break;
                    }
                if (GuildEnnemy != null)
                {
                    Enemy.TryAdd(GuildEnnemy.Info.GuildID, GuildEnnemy);
                    SendGuilEnnemy(stream, false, null);
                }
            }
        }
        public unsafe void RemoveAlly(string Name, ServerSockets.Packet stream)
        {
            if (AllowAddAlly(Name))
                return;
            Guild GuildAlly = null;
            foreach (Guild gui in Ally.Values)
                if (gui.GuildName == Name)
                {
                    GuildAlly = gui;
                    break;
                }
            if (GuildAlly != null)
            {
                foreach (Client.GameClient aclient in Pool.GamePoll.Values)
                {
                    if (aclient.Player.GuildID == Info.GuildID)
                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveAlly, GuildAlly.Info.GuildID, new int[4], ""));
                    if (aclient.Player.GuildID == GuildAlly.Info.GuildID)
                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveAlly, Info.GuildID, new int[4], ""));
                }
                Guild rem;
                GuildAlly.Ally.TryRemove(Info.GuildID, out rem);
                Ally.TryRemove(GuildAlly.Info.GuildID, out rem);
            }
        }
        public void SendGuilEnnemy(ServerSockets.Packet stream, bool JustMe, Client.GameClient client)
        {
            if (JustMe)
            {
                foreach (Guild GuildEnemie in Enemy.Values)
                {
                    client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildEnemies, GuildEnemie.Info.GuildID, false
                       , new string[1] { GuildEnemie.GuildName + " " + GuildEnemie.Info.LeaderName + " " + GuildEnemie.Info.Level + " " + GuildEnemie.Members.Count + "" });
                }
            }
            else
            {
                foreach (Client.GameClient GuildMember in Pool.GamePoll.Values)
                {
                    if (GuildMember.Player.GuildID == Info.GuildID)
                    {
                        foreach (Guild GuildEnemie in Enemy.Values)
                        {
                            GuildMember.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildEnemies, GuildEnemie.Info.GuildID, false
                                , new string[1] { GuildEnemie.GuildName + " " + GuildEnemie.Info.LeaderName + " " + GuildEnemie.Info.Level + " " + GuildEnemie.Members.Count });

                        }
                    }
                }
            }
        }
        public void SendGuildAlly(ServerSockets.Packet stream, bool JustMe, Client.GameClient client)
        {
            if (JustMe)
            {
                foreach (Guild AllyGuild in Ally.Values)
                {
                    client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildAllies, AllyGuild.Info.GuildID, false
                        , new string[1] { AllyGuild.GuildName + " " + AllyGuild.Info.LeaderName + " " + AllyGuild.Info.Level + " " + AllyGuild.Members.Count });
                }
            }
            else
            {
                foreach (Client.GameClient GuildMember in Pool.GamePoll.Values)
                {
                    if (GuildMember.Player.GuildID == Info.GuildID)
                    {
                        foreach (Guild AllyGuild in Ally.Values)
                        {
                            GuildMember.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildAllies, AllyGuild.Info.GuildID, false
                                , new string[1] { AllyGuild.GuildName + " " + AllyGuild.Info.LeaderName + " " + AllyGuild.Info.Level + " " + AllyGuild.Members.Count });
                        }
                    }
                }
            }
        }
        public unsafe void RemoveEnemy(string Name, ServerSockets.Packet stream)
        {
            if (AllowAddEnemy(Name))
                return;
            Guild GuildEnemy = null;
            foreach (Guild gui in Enemy.Values)
                if (gui.GuildName == Name)
                {
                    GuildEnemy = gui;
                    break;
                }
            if (GuildEnemy != null)
            {
                foreach (Client.GameClient aclient in Pool.GamePoll.Values)
                {
                    if (aclient.Player.GuildID == Info.GuildID)
                    {

                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveEnemy, GuildEnemy.Info.GuildID, new int[4], ""));
                    }
                }

                Guild rem;
                Enemy.TryRemove(GuildEnemy.Info.GuildID, out rem);
            }
        }
        public unsafe void Dismis(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (Members.Count == 1)
                {
                    Guild dismising;
                    if (GuildPoll.TryRemove(Info.GuildID, out dismising))
                    {
                        global::Qahr.Mobile.Database.Storage.SocialStorage.OnGuildDisbanded(Info.GuildID);
                        // نقل ملف الجيلد لـ Guilds\Disbanded عشان ميرجعش بعد الريستارت
                        GuildTable.MoveToDisbanded("Guilds", Info.GuildID);
                        if (Ally.Count > 0)
                        {
                            foreach (var GuildAlly in Ally.Values)
                                GuildAlly.RemoveAlly(GuildName, stream);
                        }
                        foreach (var Guilds in GuildPoll.Values)
                        {
                            if (Guilds.Info.GuildID != Info.GuildID)
                            {
                                if (Guilds.Enemy.ContainsKey(Info.GuildID))
                                    Guilds.RemoveEnemy(GuildName, stream);
                            }
                        }
                        client.Player.GuildID = 0;
                        client.Player.GuildRank = Role.Flags.GuildMemberRank.None;
                        client.Player.MyGuild = (Guild)null;
                        client.Player.MyGuildMember = (Guild.Member)null;
                        client.Player.View.SendView(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Disband, Info.GuildID, new int[4], ""), true);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);


                    }
                }
                else
                {
                    SendMessajGuild("Please kick all members, and next dismising");
                }

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public int BuletinEnrole = 0;
        public void CreateBuletinTime(int Time = 0)
        {
            if (Time == 0)
            {
                var timers = DateTime.Now;
                Time = GetTime(timers.Year, timers.Month, timers.Day);
            }
            BuletinEnrole = Time;
        }
        public int GetTime(int year, int month, int day)
        {
            int Timer = year * 10000 + month * 100 + day;
            return Timer;
        }
        public unsafe void SendMessajGuild(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.Guild, Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.yellow)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.GuildID == Info.GuildID)
                        user.Send(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream));
                }
            }
        }
        public unsafe void SendPacket(ServerSockets.Packet packet)
        {
            foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
            {
                var user = kv.Value;
                if (user.Player.GuildID == Info.GuildID)
                    user.Send(packet);
            }
        }
        public unsafe void SendThat(Role.Player player)
        {
            if (player.MyGuildMember == null)
                return;
            if (Bulletin != null && Bulletin != "" && Bulletin != "None")
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    player.Owner.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Bulletin, (uint)BuletinEnrole, new int[4], Bulletin));
                }
            }
            Info.MyRank = (uint)player.MyGuildMember.Rank;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                player.Owner.Send(stream.GuildInformationCreate(Info, Members.Count));

            }

        }
        public Member GetMember(string name)
        {
            foreach (Member memb in Members.Values)
                if (memb.Name == name)
                    return memb;
            return null;
        }
        public void Quit(string name, bool ReceiveKick, ServerSockets.Packet stream)
        {

            Guild.Member member = GetMember(name);
            if (member == null)
                return;
            if (ReceiveKick)
            {
                SendMessajGuild(member.Name + " have been expelled from our guild.");
                AddMessage("STR_SYNDICATE_EVENT_EXPEL_MEMBER@@" + member.Name + "@@" + member.Name + "@@");
            }
            else
            {
                SendMessajGuild(member.Name + " has quit our guild.");
                AddMessage("STR_SYNDICATE_EVENT_MEMBER_LEAVE@@" + member.Name + "@@");
            }
            Client.GameClient user;
            if (Pool.GamePoll.TryGetValue(member.UID, out user))
            {
                user.Player.GuildID = 0U;
                user.Player.GuildRank = Role.Flags.GuildMemberRank.None;
                user.Player.MyGuild = (Guild)null;
                user.Player.MyGuildMember = (Guild.Member)null;
                user.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Disband, Info.GuildID, new int[4], ""));
                user.Player.View.Clear(stream);
                user.Player.View.Role();
                user.Player.GuildBattlePower = 0U;
                if (member.UnionMem != null)
                {
                    Union getUnion = GetUnion;
                    if (getUnion != null)
                        getUnion.AddOtherMember(stream, user);
                    else if (user.Player.InUnion)
                        user.Player.MyUnion.AddOtherMember(stream, user);
                }
            }
            else
                ServerDatabase.LoginQueue.TryEnqueue((object)new Guild.UpdateDB()
                {
                    GuildID = 0U,
                    Rank = Role.Flags.GuildMemberRank.None,
                    UID = member.UID
                });
            Members.TryRemove(member.UID, out member);
            if (member.UnionMem == null)
                return;


        }
        public bool UseAdvertise = false;
        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            return writer.Add(Info.GuildID).Add(GuildName).Add(Info.Level).Add(Info.LeaderName).Add(Info.SilverFund).Add(Info.ConquerPointFund)
                  .Add(Info.CreateTime).Add(Bulletin).Add(BuletinEnrole).Add(UnionID).Add(this.Info.Leaderid).Add(this.Info.Material).Add(Info.Prestige).Add((uint)Info.Recruitment_Flag).Add(Info.Recruitment_Battle_Power).Add(Info.RecruitmentON ? (byte)1 : (byte)0).Add(Info.RecruitmentOFF ? (byte)1 : (byte)0).Add(Info.ArsenalBP).Add(CommandID).Close();
        }
        public static ConcurrentDictionary<uint, string> ChangeNameRecords = new ConcurrentDictionary<uint, string>();
        public static void RegisterChangeName(uint guildID, string guildname)
        {
            if (!ChangeNameRecords.ContainsKey(guildID))
                ChangeNameRecords.TryAdd(guildID, guildname);
            else
                ChangeNameRecords[guildID] = guildname;

        }
        public static void ProcessChangeNames()
        {
            foreach (var record in ChangeNameRecords)
            {
                Guild clan;
                if (GuildPoll.TryGetValue(record.Key, out clan))
                {
                    clan.GuildName = record.Value;
                }
            }
        }
        public static bool AllowToCreate(string Name)
        {
            if (!BaseFunc.NameStrCheck(Name))
                return false;

            foreach (var guil in ChangeNameRecords)
                if (guil.Value == Name)
                    return false;

            foreach (Guild guil in GuildPoll.Values)
                if (guil.GuildName == Name)
                    return false;

            return true;
        }
    }
}
