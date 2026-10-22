using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Clan
    {

        public static int MaxPlayersInClan(byte level)
        {
            return 11;//
        }
        public static uint[] UpdateLevelAmount = new uint[9] { 1000000, 2000000, 3500000, 7500000, 10000000, 20000000, 50000000, 100000000, 200000000 };
        public static uint[] UpdateBPAmount = new uint[4] { 2500000, 12000000, 35000000, 120000000 };
        public static uint[] UpdateScoreLevel = new uint[5] { 0, 1000000, 5000000, 20000000, 100000000};
        public enum Ranks : ushort
        {
            Leader = 100,
            Spouse = 11,
            Member = 10
        }
        public class Member
        {
            public Ranks Rank = Ranks.Member;
            public uint UID;
            public string Name = "";
            public uint Donation;
            public byte Level;
            public uint Class;
            public bool Online = false;

        }
        public static ConcurrentDictionary<uint, string> ChangeNameRecords = new ConcurrentDictionary<uint, string>();
        public static ConcurrentDictionary<uint, string> ChangeNameRecords2 = new ConcurrentDictionary<uint, string>();
        public static System.Counter CounterClansID = new System.Counter(10);
        public static ConcurrentDictionary<uint, Clan> Clans = new ConcurrentDictionary<uint, Clan>();
        public ConcurrentDictionary<uint, Clan> Ally = new ConcurrentDictionary<uint, Clan>();
        public ConcurrentDictionary<uint, Clan> Enemy = new ConcurrentDictionary<uint, Clan>();
        public ConcurrentDictionary<uint, Member> Members = new ConcurrentDictionary<uint, Member>();

        public static void RegisterChangeName2(uint guildid, string name)
        {
            if (!ChangeNameRecords2.ContainsKey(guildid))
                ChangeNameRecords2.TryAdd(guildid, name);
            else
                ChangeNameRecords2[guildid] = name;

        }
        public static void RegisterChangeName(uint clanid, string name)
        {
            if (!ChangeNameRecords.ContainsKey(clanid))
                ChangeNameRecords.TryAdd(clanid, name);
            else
                ChangeNameRecords[clanid] = name;

        }
        public static void ProcessChangeNames()
        {
            foreach (var record in ChangeNameRecords)
            {
                Clan clan;
                if (Clans.TryGetValue(record.Key, out clan))
                {
                    clan.Name = record.Value;
                }
            }
        }


        public bool TryGetClan(string Name, out Clan cln)
        {
            var dicionary = Clans.Values.ToArray();
            foreach (var cl in dicionary)
            {
                if (cl.Name == Name)
                {
                    cln = cl;
                    return true;
                }
            }
            cln = null;
            return false;
        }

        public uint RequestAlly = 0;

        public string Name = "";
        public string LeaderName = "";
        public uint ID = 0;
        public uint GuildID = 0;
        public byte Level = 1;
        public uint Donation = 0;
        public string ClanBuletin = "None";
        public byte BP = 0;
        public byte LevelStore = 0;

        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(ID).Add(Name).Add(LeaderName).Add(Level).Add(Donation).Add(ClanBuletin).Add(BP).Add(LevelStore).Add(0).Add(0).Add(0);
            return writer.Close();
        }
        public void Load(string line)
        {
            if (line == "") return;
            if (line == null) return;

            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            ID = reader.Read((uint)0);
            Name = reader.Read("None");
            LeaderName = reader.Read("None");
            Level = reader.Read((byte)0);
            Donation = reader.Read((uint)0);
            ClanBuletin = reader.Read("None");
            BP = reader.Read((byte)0);
            LevelStore = reader.Read((byte)0);
        }
        public string SaveAlly()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(Ally.Count);
            foreach (var aly in Ally.Values)
                writer.Add(aly.ID);
            return writer.Close();
        }
        public string SaveEnemy()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(Enemy.Count);
            foreach (var enemy in Enemy.Values)
                writer.Add(enemy.ID);
            return writer.Close();
        }

        public static bool AllowCreateClan(string Name)
        {
            if (!BaseFunc.NameStrCheck(Name))
                return false;
            foreach (var nname in ChangeNameRecords.Values)
                if (nname == Name)
                    return false;
            foreach (var clan in Clans.Values)
                if (clan.Name == Name)
                    return false;
            return true;
        }
        public unsafe void Create(Client.GameClient client, string ClanName, ServerSockets.Packet stream)
        {
            Level = 0;
            BP = 0;
            LevelStore = 0;
            Name = ClanName;
            ID = CounterClansID.Next;
            LeaderName = client.Player.Name;
            Donation = 500000;
            Clans.TryAdd(ID, this);
            if (AddMember(client, Ranks.Leader, stream))
                Members[client.Player.UID].Donation = 500000;

            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has created new clan " + ClanName + " .", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

        }
        public unsafe void ShareBattlePower(uint leaderUID, uint BpShare, Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, client.Player.UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ClanShareBp, leaderUID);
                client.Send(upd.GetArray(stream));
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ClanShareBp, BpShare);
                client.Send(upd.GetArray(stream));
            }
        }
       
        public unsafe bool AddMember(Client.GameClient client, Ranks rnk, ServerSockets.Packet stream)
        {
            if (!Members.ContainsKey(client.Player.UID))
            {
                Member memb = new Member();
                memb.Class = client.Player.Class;
                memb.UID = client.Player.UID;
                memb.Level = (byte)client.Player.Level;
                memb.Rank = rnk;
                memb.Online = true;
                memb.Name = client.Player.Name;
                Members.TryAdd(memb.UID, memb);

                client.Player.MyClanMember = memb;
                client.Player.MyClan = this;
                client.Player.ClanName = Name;
                client.Player.ClanRank = (ushort)rnk;
                client.Player.ClanUID = ID;

                SendThat(stream, client);
                SendBuletin(stream, client);
                client.Player.View.SendView(client.Player.GetArray(stream, false), false);

                Send(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has join in clan !", Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));


                return true;
            }
            return false;
        }
        public unsafe void SendThat(ServerSockets.Packet stream, Client.GameClient client)
        {
            client.Send(stream.ClanCreate(client, this));
        }

        public unsafe void Send(ServerSockets.Packet data)
        {
            var dicionary = Pool.GamePoll.Values.ToArray();
            foreach (var client in dicionary)
                if (client.Player.ClanUID == ID)
                    client.Send(data);

        }

        public void RemoveMember(string name, ServerSockets.Packet stream)
        {
            Role.Instance.Clan.Member member;
            if (TryGetMember(name, out member))
            {
                Client.GameClient pClient;
                if (Pool.GamePoll.TryGetValue(member.UID, out pClient))
                {
                    if (pClient.Player.MyClan == null) return;
                    if (pClient.Player.MyClanMember == null) return;

                    RemoveMember(pClient);
                    pClient.Player.View.ReSendView(stream);

                }
                else
                {
                    if (Members.TryRemove(member.UID, out member))
                    {
                        Database.ServerDatabase.LoginQueue.Enqueue(member);
                    }
                }

            }

        }

        public unsafe bool RemoveMember(Client.GameClient client)
        {
            Member memb;
            if (Members.TryRemove(client.Player.UID, out memb))
            {
                client.Player.MyClanMember = null;
                client.Player.MyClan = null;
                client.Player.ClanName = "";
                client.Player.ClanRank = 0;
                client.Player.ClanUID = 0;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Send(stream.ClanCallBackCreate(MsgClan.Info.Quit, 0, 0, null));
                }
                ShareBattlePower(0, 0, client);
                //client.Player.ClanBp = 0;
                return true;
            }
            return false;
        }
        public unsafe void DisbandClan(ServerSockets.Packet stream)
        {
            Clan cln;
            if (Clans.TryRemove(ID, out cln))
            {
                global::Qahr.Mobile.Database.Storage.SocialStorage.OnClanDisbanded(ID);
                // نقل ملف الكلان لـ Clans\Disbanded عشان ميرجعش بعد الريستارت
                Database.GuildTable.MoveToDisbanded("Clans", ID);

                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("" + LeaderName + " has disbanded the clan " + Name + " !", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                foreach (var memeber in Members.Values)
                {
                    Client.GameClient client;
                    if (Pool.GamePoll.TryGetValue(memeber.UID, out client))
                    {
                        RemoveMember(client);
                        client.Player.View.ReSendView(stream);
                    }
                    else
                    {
                        Database.ServerDatabase.LoginQueue.TryEnqueue(memeber);
                    }
                }
            }
        }
        public bool TryGetMember(string MemberName, out Member membe)
        {
            foreach (Member obj in Members.Values)
            {
                if (obj.Name == MemberName)
                {
                    membe = obj;
                    return true;
                }
            }
            membe = null;
            return false;
        }
        public unsafe void SendBuletin(ServerSockets.Packet stream, Client.GameClient client)
        {
            if (ClanBuletin != "None")
            {
                client.Send(stream.ClanBulletinCreate(client, this));
            }
        }
        public bool IsEnemy(string name)
        {
            foreach (var cln in Enemy.Values)
                if (cln.Name == name)
                    return true;
            return false;
        }
        public bool IsAlly(string name)
        {
            foreach (var cln in Ally.Values)
                if (cln.Name == name)
                    return true;
            return false;
        }
        public uint GetClanAlly(string a_name)
        {
            foreach (var cln in Ally.Values)
                if (a_name == cln.Name)
                    return cln.ID;
            return 0;
        }
        public uint GetClanEnemy(string a_name)
        {
            foreach (var cln in Enemy.Values)
                if (a_name == cln.Name)
                    return cln.ID;
            return 0;
        }
    }
}
