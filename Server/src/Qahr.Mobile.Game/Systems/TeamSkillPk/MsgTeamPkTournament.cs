using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgTournaments
{

    public class MsgSkillTeamPkTournament
    {
        public ProcesType Proces;
        public static MsgTeamEliteGroup[] EliteGroups;

        public MsgSkillTeamPkTournament()
        {
            Create();
        }
        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgTeamEliteGroup[(byte)MsgEliteTournament.GroupTyp.Count];

            for (MsgEliteTournament.GroupTyp x = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus; x < MsgEliteTournament.GroupTyp.Count; x++)
            {
                EliteGroups[(byte)x] = new MsgTeamEliteGroup(x, GamePackets.MsgTeamPopPKArenic);
            }
        }
        public void Start()
        {
        Agen:
            if (Proces == ProcesType.Dead)
            {
                foreach (var group in EliteGroups)
                    group.CreateWaitingMap();

                Proces = ProcesType.Idle;
                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                {
                    client.Player.MessageBox("The Skill Team PK Tournament will start at 20:00. Prepare yourself and sign up for it as a team!", new Action<Client.GameClient>(p => p.Teleport(448, 271, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.None);
                }
            }
            else if (Proces != ProcesType.Dead && EliteGroups.All(o => o.Proces == ProcesType.Dead))
            {
                Proces = ProcesType.Dead;
                goto Agen;
            }
        }
        public void Save()
        {
            Database.DBActions.Write writer = new Database.DBActions.Write("\\SkillTeamPK.ini");
            for (int x = 0; x < EliteGroups.Length; x++)
            {
                var Tournament = EliteGroups[x];

                for (int i = 0; i < Tournament.Top8.Length; i++)
                {
                    Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                    var element = Tournament.Top8[i];
                    writerline.Add(x).Add(i).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ClaimReward).Add(element.LeaderUID);
                    writer.Add(writerline.Close());
                }

            }
            writer.Execute(Database.DBActions.Mode.Open);
        }
        public void Load()
        {
            Database.DBActions.Read Reader = new Database.DBActions.Read("\\SkillTeamPK.ini");
            if (Reader.Reader())
            {
                int count = Reader.Count;
                for (int x = 0; x < count; x++)
                {
                    Database.DBActions.ReadLine Readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                    byte Tournament = Readline.Read((byte)0);
                    byte Rank = Readline.Read((byte)0);
                    // نفس ترتيب Save: UID, Name, Mesh, ClaimReward, LeaderUID (كان ClaimReward بياخد قيمة LeaderUID → الجايزة تتاخد تاني بعد الريستارت)
                    uint fUid = Readline.Read((uint)0);
                    string fName = Readline.Read("");
                    uint fMesh = Readline.Read((uint)0);
                    byte fClaim = Readline.Read((byte)0);
                    uint fLeader = Readline.Read((uint)0);
                    MsgTeamEliteGroup.FighterStats status = new MsgTeamEliteGroup.FighterStats(fUid, fName, fMesh, null, 0);
                    status.ClaimReward = fClaim;
                    status.LeaderUID = fLeader;
                    EliteGroups[Tournament].Top8[Rank] = status;
                }
            }
        }
        public bool SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (client.Team == null)
                    return false;
                client.HeroRewards.AddGoal(407);
                MsgEliteTournament.GroupTyp group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }
            return false;
        }
        public void Remove(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp group = GetGroup(client);
            if (client.Team != null)
            {
                if (EliteGroups[(byte)group].Teams.ContainsKey(client.Team.UID))
                {
                    EliteGroups[(byte)group].Teams.Remove(client.Team.UID);
                }
            }
            else
            {
                foreach (var team in EliteGroups[(byte)group].Teams.GetValues())
                {
                    if (team.IsTeamMember(client.Player.UID))
                    {
                        team.Remove(client, true);
                    }
                }
            }
        }
        public MsgEliteTournament.GroupTyp GetGroup(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;
            ushort level = client.Team.Leader.Player.Level;

            return tournament;
        }

        public bool GetReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            foreach (var tournament in EliteGroups)
            {
                byte Rank = 0;
                if (tournament.GetReward(client, out Rank))
                {
                    ReceiceTitle(tournament, Rank, client);
                    uint ItemID = GetItemID(tournament, Rank);
                    Database.ItemType.DBItem DBItem;
                    if (Pool.ItemsBase.TryGetValue(ItemID, out DBItem))
                    {
                        if (client.Inventory.HaveSpace(1))
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                        else
                            client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);
                    }
                    #region SkillTeamPk EPK_Lvl130Plus
                    if (Rank == 1) // ( All Rewards Rank 1 In Skill Team )
                    {
                        if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
                        {
                            Role.Statue.ElitePkStatue(client);
                        }
                        client.Player.ConquerPoints += 100000;
                        client.Inventory.AddItemWitchStack(3004121, 0, 1, stream, false); // Vip6Token[1day]

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 1 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 100.000 CPs + VIP6 Token (1 Day) ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 100.000 CPs - VIP 6 Token (1 Day)";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));



                    }
                    else if (Rank == 2) // ( All Rewards Rank 2 In Skill Team )
                    {

                        client.Player.ConquerPoints += 80000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 2 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 80.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 80.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                    }
                    else if (Rank == 3) // ( All Rewards Rank 3 In Skill Team )
                    {

                        client.Player.ConquerPoints += 70000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 3 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 70.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 70.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                    }
                    else if (Rank == 4) // ( All Rewards Rank 4 In Skill Team )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 4 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 5) // ( All Rewards Rank 5 In Skill Team )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 5 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 6) // ( All Rewards Rank 6 In Skill Team )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 6 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 7) // ( All Rewards Rank 7 In Skill Team )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 7 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 8) // ( All Rewards Rank 8 In Skill Team )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Skill Team Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 8 ] " +
    "Tournament : [ Skill Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    #endregion

                    return true;
                }
            }
            return false;
        }
        public void GetTitle(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!GetReward(client, stream))
            {
                foreach (var tournament in EliteGroups)
                {
                    byte Rank = 0;
                    if (!tournament.GetReward(client, out Rank) && Rank != 0)
                    {
                        ReceiceTitle(tournament, Rank, client);
                        break;
                    }
                }
            }
        }
        public uint GetItemID(MsgTeamEliteGroup tournament, byte Rank)
        {
            return (uint)(721300 + 1 * (byte)tournament.GroupTyp + Math.Min(3, (Rank - 1)) * 4);
        }
        public void ReceiceTitle(MsgTournaments.MsgTeamEliteGroup tournament, byte Rank, Client.GameClient client)
        {

            if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion_High_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_High_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place__High_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8_High_, true);
            }
            else
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion__Low_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_Low_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place_Low_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8__Low_, true);
            }
        }
    }
    public class MsgTeamPkTournament
    {
        public ProcesType Proces;
        public static MsgTeamEliteGroup[] EliteGroups;

        public MsgTeamPkTournament()
        {
            Create();
        }
        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgTeamEliteGroup[(byte)MsgEliteTournament.GroupTyp.Count];

            for (MsgEliteTournament.GroupTyp x = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus; x < MsgEliteTournament.GroupTyp.Count; x++)
            {
                EliteGroups[(byte)x] = new MsgTeamEliteGroup(x, GamePackets.MsgTeamPKArenic);
            }
        }
        public void Start()
        {
        Agen:
            if (Proces == ProcesType.Dead)
            {

                foreach (var group in EliteGroups)
                    group.CreateWaitingMap();

                Proces = ProcesType.Idle;
                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                {
                    client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(443, 271, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.TeamPkTournament);
                }
            }
            else if (Proces != ProcesType.Dead && EliteGroups.All(o => o.Proces == ProcesType.Dead))
            {
                Proces = ProcesType.Dead;
                goto Agen;
            }
        }
        public void Save()
        {
            Database.DBActions.Write writer = new Database.DBActions.Write("\\TeamElitePK.ini");
            for (int x = 0; x < EliteGroups.Length; x++)
            {
                var Tournament = EliteGroups[x];

                for (int i = 0; i < Tournament.Top8.Length; i++)
                {
                    Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                    var element = Tournament.Top8[i];
                    writerline.Add(x).Add(i).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ClaimReward).Add(element.LeaderUID);
                    writer.Add(writerline.Close());
                }

            }
            writer.Execute(Database.DBActions.Mode.Open);
        }
        public void Load()
        {
            Database.DBActions.Read Reader = new Database.DBActions.Read("\\TeamElitePK.ini");
            if (Reader.Reader())
            {
                int count = Reader.Count;
                for (int x = 0; x < count; x++)
                {
                    Database.DBActions.ReadLine Readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                    byte Tournament = Readline.Read((byte)0);
                    byte Rank = Readline.Read((byte)0);
                    // نفس ترتيب Save: UID, Name, Mesh, ClaimReward, LeaderUID (كان ClaimReward بياخد قيمة LeaderUID → الجايزة تتاخد تاني بعد الريستارت)
                    uint fUid = Readline.Read((uint)0);
                    string fName = Readline.Read("");
                    uint fMesh = Readline.Read((uint)0);
                    byte fClaim = Readline.Read((byte)0);
                    uint fLeader = Readline.Read((uint)0);
                    MsgTeamEliteGroup.FighterStats status = new MsgTeamEliteGroup.FighterStats(fUid, fName, fMesh, null, 0);
                    status.ClaimReward = fClaim;
                    status.LeaderUID = fLeader;
                    EliteGroups[Tournament].Top8[Rank] = status;
                }
            }
        }
        public void Remove(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp group = GetGroup(client);
            if (client.Team != null)
            {
                if (EliteGroups[(byte)group].Teams.ContainsKey(client.Team.UID))
                {
                    EliteGroups[(byte)group].Teams.Remove(client.Team.UID);
                }
            }
            else
            {
                foreach (var team in EliteGroups[(byte)group].Teams.GetValues())
                {
                    if (team.IsTeamMember(client.Player.UID))
                    {
                        team.Remove(client, true);
                    }
                }
            }
        }
        public bool SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (client.Team == null)
                    return false;
                client.HeroRewards.AddGoal(406);

                MsgEliteTournament.GroupTyp group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }

            return false;
        }
        public MsgEliteTournament.GroupTyp GetGroup(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;
            ushort level = client.Team.Leader.Player.Level;
            return tournament;
        }

        public bool GetReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            foreach (var tournament in EliteGroups)
            {
                byte Rank = 0;
                if (tournament.GetReward(client, out Rank))
                {
                    ReceiceTitle(tournament, Rank, client);
                    uint ItemID = GetItemID(tournament, Rank);
                    Database.ItemType.DBItem DBItem;
                    if (Pool.ItemsBase.TryGetValue(ItemID, out DBItem))
                    {
                        if (client.Inventory.HaveSpace(1))
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                        else
                            client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                    }
                    #region TeamPk EPK_Lvl130Plus
                    if (Rank == 1) // ( All Rewards Rank 1 In Team PK )
                    {
                        if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
                        {
                            Role.Statue.ElitePkStatue(client);
                        }
                        client.Player.ConquerPoints += 100000;
                        client.Inventory.AddItemWitchStack(3004121, 0, 1, stream, false); // Vip6Token[1day]

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 1 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 100.000 CPs + VIP6 Token (1 Day) ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 100.000 CPs - VIP 6 Token (1 Day)";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));



                    }
                    else if (Rank == 2) // ( All Rewards Rank 2 In Team PK )
                    {
                        client.Player.ConquerPoints += 80000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 2 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 80.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 80.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 3) // ( All Rewards Rank 3 In Team PK )
                    {
                        client.Player.ConquerPoints += 70000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 3 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 70.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 70.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 4) // ( All Rewards Rank 4 In Team PK )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 4 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 5) // ( All Rewards Rank 5 In Team PK )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 5 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 6) // ( All Rewards Rank 6 In Team PK )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 6 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 7) // ( All Rewards Rank 7 In Team PK )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 7 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 8) // ( All Rewards Rank 8 In Team PK )
                    {
                        client.Player.ConquerPoints += 50000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Team PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 8 ] " +
    "Tournament : [ Team PK Tournament ] " +
    "Reward : [ 50.000 CPs ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Team PK Tournament and claimed 50.000 CPs";
                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    #endregion
                    return true;
                }
            }
            return false;
        }
        public uint GetItemID(MsgTeamEliteGroup tournament, byte Rank)
        {
            return (uint)(720794 + 1 * (byte)tournament.GroupTyp + Math.Min(3, (Rank - 1)) * 4);
        }
        public void GetTitle(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!GetReward(client, stream))
            {
                foreach (var tournament in EliteGroups)
                {
                    byte Rank = 0;
                    if (!tournament.GetReward(client, out Rank) && Rank != 0)
                    {
                        ReceiceTitle(tournament, Rank, client);
                        break;
                    }
                }
            }
        }
        public void ReceiceTitle(MsgTournaments.MsgTeamEliteGroup tournament, byte Rank, Client.GameClient client)
        {
            // return;
            if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion_High_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_High_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place__High_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8_High_, true);
            }
            else
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion__Low_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_Low_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place_Low_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8__Low_, true);
            }
        }
    }
    public class MsgTeamEliteGroup
    {
        public const ushort WaitingAreaID = 2068;

        public class FighterStats
        {
            public enum StatusFlag : uint
            {
                None = 0,
                Fighting = 2,
                Lost = 4,
                Qualified = 3,
                Waiting = 1,
            }
            public System.SafeDictionary<uint, Client.GameClient> Wagers;
            public string Name;
            public uint UID;
            public uint Mesh;
            public uint Wager;
            public uint Cheers;
            public uint Points;
            public byte ClaimReward = 0;
            public Role.Instance.Team Team;
            public uint FrameID = 0;
            uint _leaderuid = 0;
            public uint LeaderUID
            {
                get
                {
                    if (Team != null) return Team.Leader.Player.UID;
                    else return _leaderuid;
                }
                set
                {
                    _leaderuid = value;
                }
            }

            public uint LeaderMesh
            {
                get
                {
                    if (Team != null) return Team.Leader.Player.Mesh;
                    return Mesh;
                }
            }

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(Name).Add(UID).Add(Mesh).Add(ClaimReward).Add(LeaderUID).Add(FrameID);
                return writer.Close();
            }

            StatusFlag _flg;
            public StatusFlag Flag
            {
                get { return _flg; }
                set
                {
                    _flg = value;
                    if (_flg == StatusFlag.Qualified)
                        Advanced = true;
                }
            }
            public bool Advanced = false;

            public FighterStats(uint id, string name, uint mesh, Role.Instance.Team _team, uint _Portrait)
            {
                Wagers = new System.SafeDictionary<uint, Client.GameClient>();
                UID = id;
                Team = _team;
                Name = name;
                Mesh = mesh;
                if (_team == null)
                    LeaderUID = id;
                else if (Team.Leader != null)
                    LeaderUID = Team.Leader.Player.UID;
                FrameID = _Portrait;
            }

            public void Reset(bool setflag = false)
            {
                Wagers.Clear();
                Wager = 0;
                Points = 0;
                Cheers = 0;
                Flag = StatusFlag.None;
                if (setflag)
                    Flag = StatusFlag.None;
            }

            public FighterStats Clone()
            {
                FighterStats stats = new FighterStats(UID, Name, Mesh, Team, FrameID);
                stats.Points = this.Points;
                stats.Flag = this.Flag;
                stats.Wager = this.Wager;
                return stats;
            }
        }
        public class Match
        {
            public enum StatusFlag : ushort
            {
                AcceptingWagers = 0,
                Watchable = 1,
                SwitchOut = 3,
                OK = 2,
            }

            public uint TimeLeft
            {
                get
                {
                    int val = (int)((ImportTime.AddSeconds(60 * 10).AllMilliseconds() - DateTime.Now.AllMilliseconds()) / 1000);
                    if (val < 0) val = 0;
                    return (uint)val;
                }
            }
            private DateTime ImportTime;
            public ushort Index;
            public uint ID;
            public StatusFlag Flag;
            public System.SafeDictionary<uint, Role.Instance.Team> Teams;
            public DateTime ExportTimer = new DateTime();
            public System.SafeDictionary<uint, FighterStats> MatchStats;

            public List<uint> Cheerers = new List<uint>();
            public System.SafeDictionary<uint, Client.GameClient> Watchers = new System.SafeDictionary<uint, Client.GameClient>();
            public Role.Instance.Team[] TeamsFighting
            {
                get
                {
                    return TeamsArray.Where(p => IsFightingTeam(p)).ToArray();
                }
            }


            public bool IsFightingTeam(Role.Instance.Team team)
            {
                foreach (var user in team.Members.Values)
                {
                    if (Predicat()(user.client))
                        return true;
                }
                return false;
            }
            private Func<Client.GameClient, bool> Predicat()
            {
                return new Func<Client.GameClient, bool>(p => p.Player.Map == 9855 && p.Player.DynamicID == DinamicID && !p.IsWatching());
            }
            public Role.Instance.Team TeamWaiting = null;

            public FighterStats[] GetMatchStats { get { return MatchStats.GetValues(); } }
            public Role.Instance.Team[] TeamsArray { get { return Teams.GetValues(); } }
            public int GroupID { get { return (int)ID / 100000 - 1; } }
            public int Count { get { return Teams.Count; } }

            private bool Done = false;
            private bool Exported = false;
            private bool Imported = false;

            private Role.GameMap Map;
            public uint DinamicID;

            public MsgTeamEliteGroup elitepkgroup;

            public bool IsFinishd() { return Exported; }
            public Match(MsgTeamEliteGroup elitegroup)
            {
                elitepkgroup = elitegroup;
                Teams = new System.SafeDictionary<uint, Role.Instance.Team>();

                Map = Pool.ServerMaps[9855];
                DinamicID = Map.GenerateDynamicID();

                Flag = StatusFlag.AcceptingWagers;

                MatchStats = new System.SafeDictionary<uint, FighterStats>();
            }
            public Match AddPlayer(Role.Instance.Team team, FighterStats.StatusFlag flag = FighterStats.StatusFlag.None)
            {
                if (team.PKStats != null)
                {
                    Teams.Add(team.UID, team);
                    team.PkMatch = this;
                    if (team.PKStats.Name != "None")
                        team.PKStats = new FighterStats(team.PKStats.UID, team.TeamName + "`s team", team.Leader.Player.Mesh, team, team.Leader.Player.FrameID);
                    else team.PKStats = new FighterStats(team.PKStats.UID, team.TeamName, team.Leader.Player.Mesh, team, team.Leader.Player.FrameID);
                    team.PKStats.Flag = flag;
                    MatchStats.Add(team.UID, team.PKStats);
                }
                return this;
            }
            public void AddWaiting()
            {
                if (Count == 3)
                {
                    TeamWaiting = TeamsArray[0];
                    TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Waiting;

                }
            }
            public void CheckFinish()
            {
                if (Count == 1)
                {
                    Done = Exported = true;
                    Flag = StatusFlag.OK;

                    foreach (var user in TeamsArray)
                        user.PKStats.Flag = FighterStats.StatusFlag.Qualified;

                    return;
                }
            }
            public bool CheckTeams()
            {

                if (TeamsArray.Length == 2)
                {
                    var team1 = TeamsArray[0];
                    var team2 = TeamsArray[1];

                    if (!team1.ReadyForTeamPK())
                    {
                        End(team1, true);
                        return false;
                    }
                    if (!team2.ReadyForTeamPK())
                    {
                        End(team2, true);
                        return false;
                    }

                }
                else if (TeamsArray.Length == 3)
                {
                    var team1 = TeamsArray[0];
                    var team2 = TeamsArray[1];
                    var team3 = TeamsArray[2];

                    if (!team1.ReadyForTeamPK())
                    {
                        End(team1, true);
                        return false;
                    }
                    if (!team2.ReadyForTeamPK())
                    {
                        End(team2, true);
                        return false;
                    }
                    if (!team3.ReadyForTeamPK())
                    {
                        End(team3, true);
                        return false;
                    }
                }
                return true;
            }
            public unsafe void Import(ServerSockets.Packet stream)
            {
                if (Count == 1)
                {
                    foreach (var user in TeamsArray)
                        user.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                    Flag = StatusFlag.OK;

                    Exported = Done = true;
                    return;
                }

                if (CheckTeams())
                {
                    if (Imported)
                        return;

                    Imported = true;
                    if (Done)
                        return;

                    Flag = StatusFlag.Watchable;

                    ImportTime = DateTime.Now;

                    if (TeamsArray.Length == 2)
                    {
                        Import(stream, TeamsArray[0], TeamsArray[1]);
                        Import(stream, TeamsArray[1], TeamsArray[0]);
                    }
                    else if (TeamsArray.Length == 3)
                    {
                        if (TeamWaiting.PKStats.Flag != FighterStats.StatusFlag.Lost)
                            TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Waiting;

                        if (elitepkgroup.pState == States.T_Finished)
                        {
                            Flag = StatusFlag.SwitchOut;

                            var Winner = TeamsArray.Where(p => p.PKStats.Flag == FighterStats.StatusFlag.Qualified).SingleOrDefault();

                            Import(stream, TeamWaiting, Winner);
                            Import(stream, Winner, TeamWaiting);
                        }
                        else
                        {
                            var array = TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Waiting && p.PKStats.Flag != FighterStats.StatusFlag.Lost).ToArray();

                            Import(stream, array[0], array[1]);
                            Import(stream, array[1], array[0]);
                        }
                    }
                    UpdateScore();
                }
            }
            public unsafe void Import(ServerSockets.Packet stream, Role.Instance.Team team, Role.Instance.Team OpponentTeam)
            {
                team.PKStats.Flag = FighterStats.StatusFlag.Fighting;

                team.UpdatePlayers(null,
                    new Action<Client.GameClient>(user =>
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map.GetRandCoord(ref x, ref y);
                        user.Teleport(50, 50, 9855, DinamicID);//Fix jump
                        user.Player.ProtectJumpAttack(10);
                        user.Player.Revive(stream);
                        if (user.Player.MyJiangHu != null)
                        {
                            if (user.Player.JiangHuActive != 0)
                            {
                                user.Player.PkMode = Role.Flags.PKMode.Capture;
                                user.Player.MyJiangHu.OnJiangMode = false;
                                user.Player.MyJiangHu.DisableJiang(user);
                            }
                        }
                        user.Player.InTeamPk = true;
                        user.Player.SetPkMode(Role.Flags.PKMode.PK);

                        user.Send(stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.BeginMatch, MsgTeamElitePKMatchUI.EffectTyp.Effect_Lose, OpponentTeam.Leader.Player.UID, OpponentTeam.TeamName, TimeLeft));


                    }));
            }

            public unsafe void UpdateScore()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.TeamElitePKMatchStatsCreate(elitepkgroup.PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgTeamPKArenicScore : GamePackets.MsgTeamPopPKArenicScore, this);

                    foreach (var team in TeamsArray)
                        team.SendFunc(Predicat(), stream);
                    foreach (var user in Watchers.GetValues())
                    {
                        if (user.Player.Map == 9855 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }

                }
            }
            public unsafe void End(Role.Instance.Team loser, bool Fource)
            {


                if (Count == 2)
                {
                    if (!Imported)
                        Exported = true;
                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    try
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            var Winner = GetOpponent(loser.UID);

                            Winner.UpdatePlayers(Predicat(), new Action<Client.GameClient>(user =>
                            {
                                if (user.Inventory.HaveSpace(1))
                                    user.Inventory.Add(stream, 723912, 1);
                                else
                                    user.Inventory.AddReturnedItem(stream, 723912, 1);

                            }));
                            loser.UpdatePlayers(Predicat(), new Action<Client.GameClient>(user =>
                            {
                                if (user.Inventory.HaveSpace(1))
                                    user.Inventory.Add(stream, 723912, 1);
                                else
                                    user.Inventory.AddReturnedItem(stream, 723912, 1);
                            }));

                            Flag = StatusFlag.OK;
                            Winner.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                            loser.PKStats.Flag = FighterStats.StatusFlag.Lost;

                            loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.Effect, MsgTeamElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                            Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.Effect, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                            loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.EndMatch, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                            Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.EndMatch, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));


                        }
                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                }
                else if (Count == 3)
                {
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        if (loser.UID == TeamWaiting.UID)
                        {
                            TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Lost;
                            return;
                        }
                    }

                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    if (!Imported)
                        Exported = true;

                    var Winner = GetOpponent(loser.UID);

                    if (Flag == StatusFlag.SwitchOut)
                        Flag = StatusFlag.OK;

                    Winner.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                    loser.PKStats.Flag = FighterStats.StatusFlag.Lost;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.Effect, MsgTeamElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                        Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.Effect, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.EndMatch, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                        Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgTeamElitePKMatchUI.State.EndMatch, MsgTeamElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        Console.WriteLine(Winner.TeamName + "= winner " + loser.TeamName + " == loser");
                    }
                }
            }
            public Role.Instance.Team GetOpponent(uint UID)
            {
                if (Count == 2)
                {
                    foreach (var team in TeamsArray)
                    {
                        if (team.UID != UID)
                            return team;
                    }
                }
                else if (Count == 3)
                {
                    if (TeamWaiting.PKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && TeamWaiting.PKStats.UID != team.PKStats.UID)
                                return team;
                        }
                    }
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && team.PKStats.Flag != FighterStats.StatusFlag.Waiting)
                                return team;
                        }
                    }
                    else if (Flag == StatusFlag.SwitchOut)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && team.PKStats.Flag != FighterStats.StatusFlag.Lost)
                                return team;
                        }
                    }
                }
                return null;
            }
            public void Export()
            {
                if (!Imported)
                {
                    return;
                }

                if (DateTime.Now > ExportTimer && Done)
                {
                    if (Exported)
                        return;

                    Exported = true;

                    foreach (var user in Watchers.GetValues())
                    {
                        DoLeaveWatching(user);
                    }

                    foreach (var team in TeamsFighting)
                    {
                        team.UpdatePlayers(Predicat(), new Action<Client.GameClient>(
                            user =>
                            {
                                ushort x = 0;
                                ushort y = 0;
                                elitepkgroup.Map.GetRandCoord(ref x, ref y);
                                user.Teleport(x, y, MsgEliteGroup.WaitingAreaID, elitepkgroup.DinamycID);
                                user.Player.RestorePkMode();

#if TEST
                                Console.WriteLine("name = " + user.Player.Name + " has teleported to X=" + x + " Y=" + y + "");
#endif
                            }));
                    }
                }


            }
            public void CheckMatch()
            {
                if (TimeLeft == 0)
                {
                    if (Count == 2 && !Done)
                    {
                        var team1 = TeamsArray[0];
                        var team2 = TeamsArray[1];

                        try
                        {
                            if (team1.PKStats.Points > team2.PKStats.Points)
                                End(team2, false);
                            else
                                End(team1, false);
                        }
                        catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                    }
                    else if (Count == 3 && !Done)
                    {
                        if (Flag != StatusFlag.SwitchOut)
                        {
                            var array = TeamsArray.Where(p => p.UID != TeamWaiting.UID).ToArray();
                            var team1 = array[0];
                            var team2 = array[1];

                            try
                            {
                                if (team1.PKStats.Points > team2.PKStats.Points)
                                    End(team2, false);
                                else
                                    End(team1, false);
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            var team1 = TeamWaiting;
                            var team2 = TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost && p.PKStats.UID != team1.PKStats.UID).SingleOrDefault();

                            try
                            {
                                if (team1.PKStats.Points > team2.PKStats.Points)
                                    End(team2, false);
                                else
                                    End(team1, false);
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }
                    }
                }
            }
            public void Switch()
            {
                if (Count == 3)
                {
                    if (TeamWaiting.PKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    if (TeamsArray.Where(p => p.UID != TeamWaiting.UID).Where(p => p.PKStats.Flag == FighterStats.StatusFlag.Lost).ToArray().Length == 2)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    Imported = false;
                    Done = false;
                    Exported = false;
                    Flag = StatusFlag.SwitchOut;
                }
            }

            public unsafe void BeginWatching(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (!client.Player.Alive)
                {
                    client.SendSysMesage("Please revive your character to watching our match.");

                    return;
                }
                if (/*client.InQualifier() ||*/ client.IsWatching())
                {
                    client.SendSysMesage("You're already in a match.");
                    return;
                }
                if (TeamsFighting.Length == 2)
                {
                    if (!Watchers.ContainsKey(client.Player.UID))
                    {
                        Watchers.Add(client.Player.UID, client);
                    }
                    try
                    {

                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.RequestView, ID, (uint)TeamsFighting.First().UID, 0, 0, 0);
                        client.Send(stream.ElitePKWatchFinalize());

                        client.TeamElitePkWatchingGroup = this;
                        client.Teleport((ushort)Pool.GetRandom.Next(35, 70), (ushort)Pool.GetRandom.Next(35, 70), 9855, DinamicID);
                        UpdateScore();
                        UpdateWatchers();

                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

                }
            }
            public unsafe void UpdateWatchers()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, ID, (uint)Watchers.Count, (TeamsFighting.Length > 0 ? TeamsFighting[0].PKStats.Cheers : 0), (TeamsFighting.Length > 1 ? TeamsFighting[1].PKStats.Cheers : 0));

                    foreach (var watch in Watchers.Values)
                        stream.AddItemElitePKWatch(watch.Player.Mesh, watch.Player.Name);

                    stream.ElitePKWatchFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var team in TeamsFighting)
                        team.SendFunc(Predicat(), stream);

                }
            }
            public unsafe void DoLeaveWatching(Client.GameClient client)
            {

                if (client.IsWatching() && Watchers.ContainsKey(client.Player.UID) && client.Player.Map == 9855 && client.Player.DynamicID == DinamicID)
                {
                    Watchers.Remove(client.Player.UID);
                    if (TeamsFighting.Length == 2)
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, ID, 0, 0, 0);
                            client.Send(stream.ElitePKWatchFinalize());
                        }


                    }
                    UpdateWatchers();
                    UpdateScore();
                    client.TeleportCallBack();

                }
                client.TeamElitePkWatchingGroup = null;
            }
            public unsafe void DoCheer(Client.GameClient client, uint uid)
            {
                if (client.IsWatching() && !Cheerers.Contains(client.Player.UID))
                {
                    Cheerers.Add(client.Player.UID);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Fighters, 0, ID, (uint)TeamsFighting.Length, TeamsFighting[0].PKStats.Cheers, TeamsFighting[1].PKStats.Cheers);



                        if (TeamsFighting[0].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[0].TeamName, 0);
                            TeamsFighting[0].PKStats.Cheers++;
                        }
                        else if (TeamsFighting.Length > 1 && TeamsFighting[1].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[1].TeamName, 0);
                            TeamsFighting[1].PKStats.Cheers++;
                        }
                        else if (TeamsFighting.Length > 2 && TeamsFighting[2].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[2].TeamName, 0);
                            TeamsFighting[2].PKStats.Cheers++;
                        }
                        stream.AddItemElitePKWatch(client.Player.Name, 0);
                        stream = stream.ElitePKWatchFinalize();

                        foreach (var team in TeamsArray)
                            team.SendFunc(Predicat(), stream);

                        foreach (var user in Watchers.GetValues())
                        {
                            if (user.Player.Map == 9855 && user.Player.DynamicID == DinamicID)
                                user.Send(stream);
                        }
                    }
                    UpdateWatchers();
                    UpdateScore();
                }
            }
        }
        public enum States : byte
        {
            T_Organize = 0,
            T_CreateMatches = 1,
            T_Import = 2,
            T_Fights = 3,
            T_Finished = 4,
            T_ReOrganize = 5
        }
        public bool GetReward(GameClient client, out byte Rank)
        {
            if (Top8.Length == 8)
            {
                for (int x = 0; x < Top8.Length; x++)
                {
                    if (Top8[x] != null)
                    {
                        if (Top8[x].LeaderUID == client.Player.UID || Top8[x].Name == client.Player.Name)
                        {
                            if (Top8[x].ClaimReward == 0)
                            {
                                Top8[x].ClaimReward = 1;
                                Rank = (byte)(x + 1);
                                return true;
                            }
                            else
                            {
                                Rank = (byte)(x + 1);
                                return false;
                            }
                        }
                    }
                }
            }
            Rank = 0;
            return false;
        }
        public Role.GameMap Map;
        public uint DinamycID;

        public MsgServer.MsgTeamElitePKBrackets.GuiTyp State;
        private States pState = States.T_Organize;
        public FighterStats[] Top8 = new FighterStats[0];

        public uint MatchIndex = 0;
        public DateTime StartTimer = new DateTime();
        public DateTime WaitForFinish = new DateTime();

        public System.SafeDictionary<uint, Role.Instance.Team> Teams;
        public Extensions.SafeDictionary<uint, Role.Instance.Team> HistoryTeams;
        public System.SafeDictionary<uint, Match> Matches;
        public System.SafeDictionary<uint, Match> Top4Matches;
        public System.SafeDictionary<uint, Match> ThreeQualiferMatch;
        public System.SafeDictionary<uint, Match> FinalMatch;

        private System.Counter MatchCounter;
        private DateTime pStamp;

        public ProcesType Proces;
        public MsgEliteTournament.GroupTyp GroupTyp;
        public ushort PKTournamentID;
        public MsgTeamEliteGroup(MsgEliteTournament.GroupTyp group, ushort _PKTournamentID)
        {
            PKTournamentID = _PKTournamentID;
            Proces = ProcesType.Dead;
            GroupTyp = group;
            Teams = new System.SafeDictionary<uint, Role.Instance.Team>();
            HistoryTeams = new Extensions.SafeDictionary<uint, Role.Instance.Team>();
            MatchCounter = new System.Counter((uint)((uint)group * 100000 + 100000));

            Top8 = new FighterStats[8];
            for (int x = 0; x < Top8.Length; x++)
                Top8[x] = new FighterStats(0, "None", 0, null, 0);
        }
        public void CreateWaitingMap()
        {

            Map = Pool.ServerMaps[WaitingAreaID];
            DinamycID = Map.GenerateDynamicID();

            if (GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            else if (GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl120To129)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            if (GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl100To119)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            else if (GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl100Minus)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }

            if (!Qahr.Mobile.Pool.Constants.BlockAttackMap.Contains(DinamycID))
                Qahr.Mobile.Pool.Constants.BlockAttackMap.Add(DinamycID);

            SubscribeTimer();

        }

        public void SubscribeTimer()
        {
            Proces = ProcesType.Idle;
        }

        public unsafe void SignUp(Client.GameClient client, bool skillteam = false)
        {
            ushort level = client.Team.Leader.Player.Level;
            if (Proces == ProcesType.Idle)
            {
                if (!Teams.ContainsKey(client.Team.UID))
                    Teams.Add(client.Team.UID, client.Team);
                client.Team.PKStats = new FighterStats(client.Team.UID, client.Team.TeamName, client.Player.Mesh, client.Team, client.Player.FrameID);
                client.Player.InTeamPk = true;
            retry:
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                if (!Map.ValidLocation(x, y))
                    goto retry;
                client.Teleport(x, y, WaitingAreaID, DinamycID);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.TeamElitePKMatchUICreate(PKTournamentID, MsgTeamElitePKMatchUI.State.Information, MsgTeamElitePKMatchUI.EffectTyp.Effect_Lose, client.Player.UID, client.Player.Name, 0);
                    client.Send(stream);

                    if (client.Team.TeamLider(client))
                    {
                        client.Send(stream.TeamEliteSetTeamName(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgDominateTeamName : GamePackets.MsgDominateTeamPopPkName, MsgTeamEliteSetTeamName.State.Apprend, client.Team.UID, client.Team.TeamName));
                    }
                }
            }
        }
        public void CheckTeams()
        {
            List<Role.Instance.Team> Remover = new List<Role.Instance.Team>();
            foreach (var team in Teams.GetValues())
            {
                if (!team.ReadyForTeamPK())
                    Remover.Add(team);
            }
            foreach (var team in Remover)
                Teams.Remove(team.UID);
        }
        public void CreateDoubleMatchs(System.SafeDictionary<uint, Match> Array)
        {
            foreach (var user in Teams.GetValues())
            {
                Match match = GetDoubleImcompleteMatch(Array);
                match.AddPlayer(user);
                if (!Array.ContainsKey(match.ID))
                {
                    Array.Add(match.ID, match);
                }
            }


        }
        public Match GetDoubleImcompleteMatch(System.SafeDictionary<uint, Match> Array)
        {
            foreach (var match in Array.GetValues())
            {
                if (match.Count < 2)
                    return match;
            }
            Match n_match = new Match(this);
            n_match.Index = (ushort)MatchIndex++;
            n_match.ID = MatchCounter.Next;
            return n_match;
        }
        public Match GetTripleImcompleteMatch(System.SafeDictionary<uint, Match> Array)
        {
            foreach (var match in Array.GetValues())
            {
                if (match.Count == 1)
                    return match;
            }
            foreach (var match in Array.GetValues())
            {
                if (match.Count <= 2)
                    return match;
            }
            Match n_match = new Match(this);
            n_match.Index = (ushort)MatchIndex++;
            n_match.ID = MatchCounter.Next;
            return n_match;
        }
        public void CreateTripleMatchs(System.SafeDictionary<uint, Match> Array)
        {
            foreach (var user in Teams.GetValues())
            {
                if (user != null)
                {
                    if (user.Leader != null)
                    {
                        if (Complete8Match())
                        {
                            Match match = GetDoubleImcompleteMatch(Array);
                            match.AddPlayer(user);
                            if (!Array.ContainsKey(match.ID))
                                Array.Add(match.ID, match);

                        }
                        else
                        {
                            GetTripleImcompleteMatch(Array).AddPlayer(user).AddWaiting();

                        }
                    }
                }
            }
        }
        public bool Complete8Match()
        {
            return Matches.Count < 8;
        }
        public ushort TimeLeft
        {
            get
            {
                int value = (int)((pStamp.AllMilliseconds() - DateTime.Now.AllMilliseconds()) / 1000);
                if (value < 0) return 0;
                return (ushort)value;
            }
        }
        public void Finish()
        {
            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

            Proces = ProcesType.Dead;
            if (Teams != null)
                Teams.Clear();
            if (Matches != null)
                Matches.Clear();
            if (Top4Matches != null)
                Top4Matches.Clear();
            if (ThreeQualiferMatch != null)
                ThreeQualiferMatch.Clear();
            if (FinalMatch != null)
                FinalMatch.Clear();
            if (HistoryTeams != null)
            {
                if (HistoryTeams.Count > 0)
                    foreach (var t_gc in HistoryTeams.Values)
                        foreach (var mem in t_gc.Members.Values)
                            if (mem.client.Player.Map == WaitingAreaID)
                                mem.client.Teleport(Places.TwinCity);
                HistoryTeams.Clear();
            }

        }
        public void timerCallback()
        {
            try
            {
                if (DateTime.Now > StartTimer && Proces == ProcesType.Idle)
                {
                    // no sign-ups: close the group and keep the last Top 8
                    if (Teams.Count == 0)
                    {
                        Finish();
                        return;
                    }
                    Top8 = new FighterStats[8];
                    for (int x = 0; x < Top8.Length; x++)
                        Top8[x] = new FighterStats(0, "None", 0, null, 0);
                    Proces = ProcesType.Alive;
                }
                if (Teams.Count == 0)
                {
                    return;
                }
                if (Teams.Count > HistoryTeams.Count)
                    foreach (var t in Teams.Values)
                        if (!HistoryTeams.ContainsKey(t.UID))
                            HistoryTeams.Add(t.UID, t);
                if (Proces == ProcesType.Alive)
                {
                    foreach (var user in Map.Values)
                        if (user != null && user.Team == null)
                        {
                            user.Player.InElitePk = false;
                            user.Player.InTeamPk = false;
                            user.TeleportCallBack();
                            user.SendSysMesage("You were not participating in a team and were automatically disqualified.");
                        }
                    if (State == MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking)
                    {

                        CheckTeams();
                        if (Teams.Count == 1)
                        {
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_ReconstructTop;
                            WaitForFinish = DateTime.Now.AddMinutes(1);
                            ActiveArena(false);
                            foreach (var player in Teams.GetValues())
                            {
                                if (player.Leader.Player.InTeamPk)
                                    player.Leader.Player.InTeamPk = false;
                                player.TeleportTeam(Places.TwinCity);
                            }

                        }
                        else if (Teams.Count == 2)
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top1;
                        else if (Teams.Count > 2 && Teams.Count <= 4)
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top2Qualifier;
                        else if (Teams.Count > 4 && Teams.Count <= 8)
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top4Qualifier;
                        else if (Teams.Count <= 24)
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier;
                        else
                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            foreach (var team in Teams.GetValues())
                            {
                                foreach (var user in team.Temates)
                                {

                                    user.client.Send(stream.TeamEliteSetTeamName(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgDominateTeamName : GamePackets.MsgDominateTeamPopPkName, MsgTeamEliteSetTeamName.State.SuccessfulName, team.UID, team.TeamName));
                                    if (team.TeamLider(user.client))
                                    {
                                        user.client.Send(stream.TeamEliteSetTeamName(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgDominateTeamName : GamePackets.MsgDominateTeamPopPkName, MsgTeamEliteSetTeamName.State.Remove, team.UID, team.TeamName));
                                    }

                                }
                            }
                        }
                        pState = States.T_Organize;
                    }
                    switch (State)
                    {
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_ReconstructTop:
                            {
                                if (DateTime.Now > WaitForFinish)
                                {
                                    Finish();
                                }
                                break;
                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            MatchIndex = 0;
                                            Matches = new System.SafeDictionary<uint, Match>();
                                            CreateDoubleMatchs(Matches);
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in Matches.GetValues())
                                                match.CheckFinish();

                                            SendBrackets(Matches.GetValues(), null, true);
                                            ActiveArena(true);

                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in Matches.GetValues())
                                                        match.Import(stream);
                                                }
                                                pState = States.T_Fights;
                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in Matches.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == Matches.Count)
                                                {
                                                    State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                    foreach (var team in Teams.GetValues())
                                                    {
                                                        if (team.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            removers.Add(team);
                                                    }
                                                    foreach (var team in removers)
                                                        Teams.Remove(team.UID);

                                                    Matches.Clear();
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            MatchIndex = 0;
                                            Matches = new System.SafeDictionary<uint, Match>();
                                            CreateTripleMatchs(Matches);

                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in Matches.GetValues())
                                                match.CheckFinish();

                                            SendBrackets(Matches.GetValues(), null, true, 0);
                                            ActiveArena(true);
                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.Import(stream);
                                                    }
                                                }
                                                pState = States.T_Fights;
                                                SendBrackets(Matches.GetValues(), null, true, 0);
                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in Matches.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == Matches.Count)
                                                {
                                                    pState = States.T_ReOrganize;
                                                }
                                            }
                                            break;
                                        }
                                    case States.T_ReOrganize:
                                        {
                                            foreach (var match in Matches.GetValues())
                                            {
                                                match.Switch();
                                            }
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            SendBrackets(Matches.GetValues(), null, true, 0);
                                            SendBrackets(Matches.GetValues(), null, true, 0);
                                            pState = States.T_Finished;
                                            break;
                                        }

                                    case States.T_Finished:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.Import(stream);

                                                    }
                                                }
                                                pState = States.T_CreateMatches;
                                                SendBrackets(Matches.GetValues(), null, true, 0);
                                            }
                                            break;
                                        }
                                    case States.T_CreateMatches:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in Matches.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == Matches.Count)
                                                {
                                                    State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                    foreach (var team in Teams.GetValues())
                                                    {
                                                        if (team.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            removers.Add(team);
                                                    }
                                                    foreach (var team in removers)
                                                        Teams.Remove(team.UID);

                                                    Matches.Clear();
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;

                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Top4Qualifier:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            MatchIndex = 0;
                                            Matches = new System.SafeDictionary<uint, Match>();
                                            CreateDoubleMatchs(Matches);
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in Matches.GetValues())
                                                match.CheckFinish();

                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                            ActiveArena(true);

                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in Matches.GetValues())
                                                        match.Import(stream);
                                                }
                                                pState = States.T_Fights;
                                                SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in Matches.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == Matches.Count)
                                                {
                                                    State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                    int i = 7;
                                                    foreach (var user in Teams.GetValues())
                                                    {
                                                        if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                            Top8[i--] = user.PKStats.Clone();

                                                        }
                                                    }
                                                    foreach (var team in removers)
                                                        Teams.Remove(team.UID);


                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Top2Qualifier:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            // MatchIndex = 0;
                                            if (Top4Matches == null)
                                                Top4Matches = new System.SafeDictionary<uint, Match>();
                                            if (Matches == null || Matches != null && Matches.Count == 0)
                                            {
                                                Matches = new System.SafeDictionary<uint, Match>();
                                                CreateDoubleMatchs(Top4Matches);
                                                if (Matches.Count == 0)
                                                {
                                                    foreach (var match in Top4Matches.GetValues())
                                                        Matches.Add(match.ID, match);
                                                }
                                            }
                                            else
                                            {
                                                Match n_match = new Match(this);
                                                n_match.Index = (ushort)MatchIndex++;
                                                n_match.ID = MatchCounter.Next;

                                                var arraymatchs = Matches.GetValues();
                                                n_match.AddPlayer(arraymatchs[0].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                n_match.AddPlayer(arraymatchs[2].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                Match m_match = new Match(this);
                                                m_match.Index = (ushort)MatchIndex++;
                                                m_match.ID = MatchCounter.Next;
                                                m_match.AddPlayer(arraymatchs[1].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                if (arraymatchs.Length > 3)
                                                    m_match.AddPlayer(arraymatchs[3].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                Top4Matches.Add(m_match.ID, m_match);
                                                Top4Matches.Add(n_match.ID, n_match);
                                            }


                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in Top4Matches.GetValues())
                                                match.CheckFinish();
                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                            SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);


                                            ActiveArena(true);

                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in Top4Matches.GetValues())
                                                        match.Import(stream);
                                                }
                                                pState = States.T_Fights;

                                                SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in Top4Matches.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == Top4Matches.Count)
                                                {


                                                    List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                    ThreeQualiferMatch = new System.SafeDictionary<uint, Match>();

                                                    foreach (var user in Teams.GetValues())
                                                    {
                                                        if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                        }
                                                    }
                                                    if (removers.Count == 1)//for 3 players.
                                                    {
                                                        foreach (var team in removers)
                                                        {
                                                            Teams.Remove(team.UID);
                                                            Top8[2] = team.PKStats;
                                                        }
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    }
                                                    else
                                                    {
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top3;
                                                        pState = States.T_Organize;

                                                        // MatchIndex = 0;
                                                        Match n_match = new Match(this);
                                                        n_match.Index = (ushort)MatchIndex++;
                                                        n_match.ID = MatchCounter.Next;
                                                        foreach (var team in removers)
                                                        {
                                                            Teams.Remove(team.UID);
                                                            n_match.AddPlayer(team, FighterStats.StatusFlag.Waiting);
                                                        }
                                                        ThreeQualiferMatch.Add(n_match.ID, n_match);
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Top3:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in ThreeQualiferMatch.GetValues())
                                                match.CheckFinish();



                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                            SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

                                            ActiveArena(true);


                                            FinalMatch = new System.SafeDictionary<uint, Match>();
                                            CreateDoubleMatchs(FinalMatch);
                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in ThreeQualiferMatch.GetValues())
                                                        match.Import(stream);
                                                }
                                                pState = States.T_Fights;

                                                SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                if (Top4Matches != null)
                                                    SendBrackets(Top4Matches.GetValues(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in ThreeQualiferMatch.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == ThreeQualiferMatch.Count)
                                                {
                                                    State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    foreach (var match in ThreeQualiferMatch.GetValues())
                                                    {
                                                        foreach (var team in match.TeamsArray)
                                                        {
                                                            if (team.PKStats.Flag != FighterStats.StatusFlag.Qualified)
                                                            {
                                                                Top8[3] = team.PKStats.Clone();
                                                            }
                                                            else
                                                                Top8[2] = team.PKStats.Clone();
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgTeamElitePKBrackets.GuiTyp.GUI_Top1:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            if (FinalMatch == null)
                                            {
                                                ThreeQualiferMatch = null;
                                                FinalMatch = new System.SafeDictionary<uint, Match>();

                                                //   MatchIndex = 0;
                                                CreateDoubleMatchs(FinalMatch);

                                            }
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in FinalMatch.GetValues())
                                                match.CheckFinish();

                                            if (Matches != null)
                                            {
                                                SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
                                            }
                                            else
                                                SendBrackets(ArrayMatchesTop3(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

                                            ActiveArena(true);

                                            break;
                                        }
                                    case States.T_Import:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    foreach (var match in FinalMatch.GetValues())
                                                        match.Import(stream);
                                                }
                                                pState = States.T_Fights;

                                                if (Matches != null)
                                                {
                                                    SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                    SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
                                                }
                                                else
                                                    SendBrackets(ArrayMatchesTop3(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

                                            }
                                            break;
                                        }
                                    case States.T_Fights:
                                        {
                                            if (TimeLeft == 0)
                                            {
                                                int FinishMatchs = 0;
                                                foreach (var match in FinalMatch.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == FinalMatch.Count)
                                                {

                                                    List<Role.Instance.Team> removers = new List<Role.Instance.Team>();

                                                    foreach (var user in Teams.GetValues())
                                                    {
                                                        if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                        }
                                                        else
                                                            Top8[0] = user.PKStats.Clone();
                                                    }

                                                    foreach (var team in removers)
                                                    {
                                                        Top8[1] = team.PKStats.Clone();
                                                        Teams.Remove(team.UID);
                                                    }

                                                    State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }

        public Match[] ArrayMatchesTop3()
        {
            Match[] array = new Match[(Top4Matches != null ? Top4Matches.Count : 0)
                + (ThreeQualiferMatch != null ? ThreeQualiferMatch.Count : 0) + (FinalMatch != null ? FinalMatch.Count : 0)];
            int position = -1;
            for (int x = 0; x < (Top4Matches != null ? Top4Matches.Count : 0); x++)
                array[++position] = Top4Matches.GetValues()[x];
            for (int x = 0; x < (ThreeQualiferMatch != null ? ThreeQualiferMatch.Count : 0); x++)
                array[++position] = ThreeQualiferMatch.GetValues()[x];
            for (int x = 0; x < (FinalMatch != null ? FinalMatch.Count : 0); x++)
                array[++position] = FinalMatch.GetValues()[x];
            return array;
        }

        public unsafe void ActiveArena(bool active)
        {

        }

        public unsafe void SendBrackets(Match[] matches, Client.GameClient user, bool sendtoall = false, ushort page = 0
         , MsgServer.MsgTeamElitePKBrackets.Action type = MsgServer.MsgTeamElitePKBrackets.Action.InitialList, bool sendmatch = false)
        {
            if (matches == null)
                return;
            switch (State)
            {
                default:
                    {
                        if (matches == null) break;
                        ListMatch(user, matches.ToList(), page, (ushort)(matches.Length * 2), sendtoall, type);
                        break;
                    }
                #region GUI_Knockout
                case MsgServer.MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout:
                    {
                        int count = Math.Min(7, matches.Length - page * 7);
                        List<Match> Matchs = new List<Match>();
                        for (int i = page * 7; i < page * 7 + count; i++)
                        {
                            Matchs.Add(matches[i]);
                        }
                        ListMatch(user, Matchs, page, (ushort)matches.Length, sendtoall);
                        break;
                    }
                #endregion
                #region GUI_Top8Qualifier
                case MsgServer.MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
                    {
                        ListMatch(user, matches.ToList(), page, (ushort)matches.Length, sendtoall);
                        break;
                    }
                #endregion
            }
        }
        public unsafe void ListMatch(Client.GameClient user, List<Match> Matchs, ushort page = 0, ushort Length = 0, bool sendtoall = false, MsgServer.MsgTeamElitePKBrackets.Action type = MsgServer.MsgTeamElitePKBrackets.Action.InitialList)
        {

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (Matchs.Count > 0)
                {
                    uint count = 1;
                    if (Matchs.Count > 1) count++;
                    if (Matchs.Count > 2) count++;
                    stream.MsgTeamEliteBracketsCreate(type, page, 0, (ushort)(Length), GroupTyp, State, TimeLeft, count, PKTournamentID == GamePackets.MsgTeamPopPKArenic, 1);
                    stream.AddItemTeamElitePKBrackets(Matchs[0], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    if (Matchs.Count > 1)
                        stream.AddItemTeamElitePKBrackets(Matchs[1], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    if (Matchs.Count > 2)
                        stream.AddItemTeamElitePKBrackets(Matchs[2], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    stream.TeamElitePKBracketsFinalize(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgTeamPKMatchInfo : GamePackets.MsgTeamPopPKMatchInfo);
                    Send(user, stream, sendtoall);
                }
                if (Matchs.Count > 3)
                {
                    uint count = 1;
                    if (Matchs.Count > 4) count++;
                    if (Matchs.Count > 5) count++;
                    stream.MsgTeamEliteBracketsCreate(MsgServer.MsgTeamElitePKBrackets.Action.InitialList, page, 0, (ushort)(Length), GroupTyp, State, TimeLeft, count, PKTournamentID == GamePackets.MsgTeamPopPKArenic, 0);
                    stream.AddItemTeamElitePKBrackets(Matchs[3], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    if (Matchs.Count > 4)
                        stream.AddItemTeamElitePKBrackets(Matchs[4], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    if (Matchs.Count > 5)
                        stream.AddItemTeamElitePKBrackets(Matchs[5], PKTournamentID == GamePackets.MsgTeamPopPKArenic);

                    stream.TeamElitePKBracketsFinalize(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgTeamPKMatchInfo : GamePackets.MsgTeamPopPKMatchInfo);
                    Send(user, stream, sendtoall);
                }
                if (Matchs.Count > 6)
                {
                    uint count = 1;
                    if (Matchs.Count > 7)
                        count++;
                    stream.MsgTeamEliteBracketsCreate(MsgServer.MsgTeamElitePKBrackets.Action.InitialList, page, 0, (ushort)(Length), GroupTyp, State, TimeLeft, count, PKTournamentID == GamePackets.MsgTeamPopPKArenic, 0);
                    stream.AddItemTeamElitePKBrackets(Matchs[6], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    if (Matchs.Count > 7)
                        stream.AddItemTeamElitePKBrackets(Matchs[7], PKTournamentID == GamePackets.MsgTeamPopPKArenic);
                    stream.TeamElitePKBracketsFinalize(PKTournamentID == GamePackets.MsgTeamPKArenic ? GamePackets.MsgTeamPKMatchInfo : GamePackets.MsgTeamPopPKMatchInfo);
                    Send(user, stream, sendtoall);
                }
            }
        }
        public void Send(Client.GameClient user, ServerSockets.Packet stream, bool sendtoall = false)
        {
            if (user != null)
                user.Send(stream);
            if (sendtoall)
                Server.SendGlobalPacket(stream);
        }
    }
}
