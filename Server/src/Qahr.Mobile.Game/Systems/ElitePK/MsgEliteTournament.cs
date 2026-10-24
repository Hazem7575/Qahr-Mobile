using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgEliteTournament
    {
        public enum GroupTyp : ushort
        {
            EPK_Lvl100Minus = 0,
            EPK_Lvl100To119 = 1,
            EPK_Lvl120To129 = 2,
            EPK_Lvl130Plus = 3,
            Count = 4
        }

        public enum BPGroupTyp : ushort
        {
            EPK_BP300Minus = 0,
            EPK_BP301To375 = 1,
            EPK_BP376To450 = 2,
            EPK_BP451Plus = 3,
            Count = 4
        }

        public class top_typ
        {
            public const byte Elite_PK_Champion__Low_ = 12,
            Elite_PK_2nd_Place_Low_ = 13,
            Elite_PK_3rd_Place_Low_ = 14,
            Elite_PK_Top_8__Low_ = 15,
             Vip = 11,
            GM = 24,
            Elite_PK_Champion_High_ = 16,
            Elite_PK_2nd_Place_High_ = 17,
            Elite_PK_3rd_Place__High_ = 18,
            Elite_PK_Top_8_High_ = 19;
        }

        public ProcesType Proces;
        public static MsgEliteGroup[] EliteGroups;

        public MsgEliteTournament()
        {
            Create();
        }
        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgEliteGroup[(byte)BPGroupTyp.Count];
            for (BPGroupTyp x = BPGroupTyp.EPK_BP300Minus; x < BPGroupTyp.Count; x++)
            {
                EliteGroups[(byte)x] = new MsgEliteGroup(x);
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
                if (Program.ServerConfig.IsInterServer)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        MsgInterServer.PipeServer.Send(new MsgMessage("[Cross Elite PK Tournament] begins at 20:00. Get yourself prepared for it!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                    }
                }
                else
                {
                    foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                    {
                        client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(438, 271, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.ElitePKTournament);
                    }
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
            if (!Program.ServerConfig.IsInterServer)
            {
                Database.DBActions.Write writer = new Database.DBActions.Write("\\ElitePk.ini");
                for (int x = 0; x < EliteGroups.Length; x++)
                {
                    var Tournament = EliteGroups[x];

                    for (int i = 0; i < Tournament.Top8.Length; i++)
                    {
                        Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                        var element = Tournament.Top8[i];
                        writerline.Add(x).Add(i).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ClaimReward).Add(element.ServerID).Add(element.FrameID);
                        writer.Add(writerline.Close());
                    }

                }
                writer.Execute(Database.DBActions.Mode.Open);
            }
        }
        public void Load()
        {
            if (!Program.ServerConfig.IsInterServer)
            {
                Database.DBActions.Read Reader = new Database.DBActions.Read("\\ElitePk.ini");
                if (Reader.Reader())
                {
                    int count = Reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine Readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                        byte Tournament = Readline.Read((byte)0);
                        byte Rank = Readline.Read((byte)0);
                        // نقرأ بنفس ترتيب Save بالظبط: UID, Name, Mesh, ClaimReward, ServerID, FrameID.
                        // (كان بيدّي ClaimReward للـ Portrait و ServerID لـ ClaimReward → الجايزة بتتصفّر وتتاخد تاني بعد كل ريستارت)
                        uint fUid = Readline.Read((uint)0);
                        string fName = Readline.Read("");
                        uint fMesh = Readline.Read((uint)0);
                        byte fClaim = Readline.Read((byte)0);
                        byte fServer = Readline.Read((byte)0);
                        uint fFrame = Readline.Read((uint)0);
                        MsgEliteGroup.FighterStats status = new MsgEliteGroup.FighterStats(fUid, fName, fMesh, 0, 0, fFrame);
                        status.ClaimReward = fClaim;
                        status.ServerID = fServer;
                        EliteGroups[Tournament].Top8[Rank] = status;
                    }
                }
            }
        }
        public bool SignUp(Client.GameClient client)
        {
            if (client.Player.BattlePower < 300)
            {
                client.SendSysMesage("Your BP is less than 300. Please improve your BP before trying again! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return false;
            }
            if (Proces == ProcesType.Idle)
            {
                BPGroupTyp group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }
            return false;
        }
        public BPGroupTyp GetGroup(Client.GameClient client)
        {
            BPGroupTyp tournament = BPGroupTyp.EPK_BP451Plus;


            return tournament;
        }

        public bool GetReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            foreach (var tournament in EliteGroups)
            {
                byte Rank = 0;
                if (tournament.GetReward(client, out Rank))
                {
                    if (tournament.BPGroupTyp == BPGroupTyp.EPK_BP451Plus && Rank == 1)
                    {
                        client.Player.AddSpecialWings(MsgTitleStorage.WingsType.WingsofInfernal, stream);
                    }
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

                    #region ElitePk 130
                    if (Rank == 1) // ( All Rewards Rank 1 In Elite PK )
                    {
                        if (tournament.BPGroupTyp == BPGroupTyp.EPK_BP451Plus)
                        {
                            Role.Statue.ElitePkStatue(client);
                        }
                        Role.Statue.ElitePkStatue(client);
                        client.Player.ConquerPoints += 50000;
                        client.Player.Money += 1000000000;
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Inventory.AddItemWitchStack(3004121, 0, 1, stream, false); // Vip6Token[1day]

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 1 ] " +
    "Reward : [ 50.000 CPs + VIP6 Token (1 Day) + x1 SacredRefineryPack - 1b money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 50.000 CPs - VIP 6 Token (1 Day) - x1 SacredRefineryPack - 1.000.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                    }
                    else if (Rank == 2)  // ( All Rewards Rank 2 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 40000;
                        client.Player.Money += 500000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 2 ] " +
    "Reward : [ 40.000 CPs + x1 SacredRefineryPack - 500m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 40.000 CPs - x1 SacredRefineryPack - 500.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 3) // ( All Rewards Rank 3 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 30000;
                        client.Player.Money += 300000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 3 ] " +
    "Reward : [ 30.000 CPs + x1 SacredRefineryPack - 300m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 30.000 CPs - x1 SacredRefineryPack - 300.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 4) // ( All Rewards Rank 4 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 25000;
                        client.Player.Money += 200000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 4 ] " +
    "Reward : [ 25.000 CPs + x1 SacredRefineryPack - 200m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 25.000 CPs - x1 SacredRefineryPack - 200.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 5) // ( All Rewards Rank 5 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 25000;
                        client.Player.Money += 200000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 5 ] " +
    "Reward : [ 25.000 CPs + x1 SacredRefineryPack - 200m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 25.000 CPs - x1 SacredRefineryPack - 200.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 6) // ( All Rewards Rank 6 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 25000;
                        client.Player.Money += 200000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 6 ] " +
    "Reward : [ 25.000 CPs + x1 SacredRefineryPack - 200m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 25.000 CPs - x1 SacredRefineryPack - 200.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 7) // ( All Rewards Rank 7 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 25000;
                        client.Player.Money += 200000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 7 ] " +
    "Reward : [ 25.000 CPs + x1 SacredRefineryPack - 200m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 25.000 CPs - x1 SacredRefineryPack - 200.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }


                    else if (Rank == 8) // ( All Rewards Rank 8 In Elite PK )
                    {
                        client.Inventory.AddItemWitchStack(3004249, 0, 1, stream, false); // SacredRefineryPack
                        client.Player.ConquerPoints += 25000;
                        client.Player.Money += 200000000;

                        Database.ServerDatabase.LoginQueue.Enqueue(
    "[Elite PK Reward] " +
    "Player Name : [ " + client.Player.Name + " ] " +
    "Player UID : [ " + client.Player.UID + " ] " +
    "Rank : [ 8 ] " +
    "Reward : [ 25.000 CPs + x1 SacredRefineryPack - 200m Money ] " +
    "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
);


                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed 25.000 CPs - x1 SacredRefineryPack - 200.000.000 Money";
                        Server.SendGlobalPacket(new MsgMessage(MSG, Game.MsgServer.MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }

                    #endregion

                    if (Rank <= 8)
                        client.HeroRewards.AddGoal(801);
                    if (Rank <= 3)
                        client.HeroRewards.AddGoal(901);
                    if (Rank == 1)
                    {
                        client.HeroRewards.AddGoal(405);
                        if (tournament.BPGroupTyp == BPGroupTyp.EPK_BP451Plus)
                            client.HeroRewards.AddGoal(1002);
                    }

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
        public void ReceiceTitle(MsgTournaments.MsgEliteGroup tournament, byte Rank, Client.GameClient client)
        {
            if (tournament.BPGroupTyp == MsgEliteTournament.BPGroupTyp.EPK_BP451Plus)
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
        public uint GetItemID(MsgEliteGroup tournament, byte Rank)
        {
            return (uint)(720714 + 1 * (byte)tournament.BPGroupTyp + Math.Min(3, (Rank - 1)) * 4);
        }
    }
    public class MsgEliteGroup
    {
        public const ushort WaitingAreaID = 2068;

        public class FighterStats
        {
            public enum StatusFlag : uint
            {
                None = 0,
                Fighting = 1,
                Lost = 2,
                Qualified = 3,
                Waiting = 4,
                Bye = 5,
                Inactive = 7,
                WonMatch = 8
            }
            public uint CrossEliteRank = 0;
            public System.SafeDictionary<uint, Client.GameClient> Wagers;
            public string Name;
            public uint UID;
            public uint RealUID;
            public uint Mesh;
            public uint Wager;
            public uint Cheers;
            public uint Points;
            public byte ClaimReward = 0;
            public uint FrameID = 0;
            public uint ServerID = 0;

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(Name).Add(UID).Add(Mesh).Add(ClaimReward).Add(FrameID);
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
            public FighterStats()
            {

            }
            public FighterStats(uint id, string name, uint mesh, uint _ServerID, uint _RealUID, uint _Portail)
            {
                Wagers = new System.SafeDictionary<uint, Client.GameClient>();
                UID = id;
                Name = name;
                ServerID = _ServerID;
                Mesh = mesh;
                RealUID = _RealUID;
                FrameID = _Portail;
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
                FighterStats stats = new FighterStats(UID, Name, Mesh, ServerID, RealUID, FrameID);
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
                SwitchOut = 2,
                InFight = 3,
                OK = 4
            }

            public uint TimeLeft
            {
                get
                {
                    int val = (int)((ImportTime.AddSeconds(60 * 7).AllMilliseconds() - DateTime.Now.AllMilliseconds()) / 1000);
                    if (val < 0) val = 0;
                    return (uint)val;
                }
            }
            private DateTime ImportTime;

            public ushort Index;
            public uint ID;
            public StatusFlag Flag;
            public System.SafeDictionary<uint, Client.GameClient> Players;
            public DateTime ExportTimer = new DateTime();
            public System.SafeDictionary<uint, FighterStats> MatchStats;

            public List<uint> Cheerers = new List<uint>();
            public System.SafeDictionary<uint, Client.GameClient> Watchers = new System.SafeDictionary<uint, Client.GameClient>();
            public Client.GameClient[] PlayersFighting
            {
                get
                {
                    return PlayersArray.Where(p => p.Player.Map == 9855 && p.Player.DynamicID == DinamicID && !p.IsWatching()).ToArray();
                }
            }

            public Client.GameClient PlayerWaiting = null;

            public FighterStats[] GetMatchStats
            {
                get
                {
                    if (elitepkgroup.State == MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier)
                    {
                        FighterStats[] stats = new FighterStats[PlayersArray.Length];
                        for (int x = 0; x < stats.Length; x++)
                            stats[x] = PlayersArray[x].ElitePKStats;
                        return stats;
                    }
                    else
                        return MatchStats.GetValues();
                }
            }
            public Client.GameClient[] PlayersArray { get { return Players.GetValues(); } }
            public int GroupID { get { return (int)ID / 100000 - 1; } }
            public int Count { get { return Players.Count; } }

            private bool Done = false;
            private bool Exported = false;
            private bool Imported = false;

            private Role.GameMap Map;
            public uint DinamicID;

            public MsgEliteGroup elitepkgroup;

            public bool IsFinishd() { return Exported; }
            public Match(MsgEliteGroup elitegroup)
            {
                elitepkgroup = elitegroup;
                Players = new System.SafeDictionary<uint, Client.GameClient>();

                Map = Pool.ServerMaps[9855];
                DinamicID = Map.GenerateDynamicID();

                Flag = StatusFlag.AcceptingWagers;

                MatchStats = new System.SafeDictionary<uint, FighterStats>();
            }
            public Match AddPlayer(Client.GameClient user, FighterStats.StatusFlag flag = FighterStats.StatusFlag.None)
            {
                Players.Add(user.Player.UID, user);

                user.ElitePkMatch = this;
                user.ElitePKStats = new FighterStats(user.Player.UID, user.Player.Name, user.Player.Mesh, user.Player.ServerID, user.Player.RealUID, user.Player.FrameID);

                user.ElitePKStats.Flag = flag;

                MatchStats.Add(user.Player.UID, user.ElitePKStats);

                return this;
            }
            public void AddWaiting()
            {
                if (Count == 3)
                {
                    PlayerWaiting = PlayersArray[0];
                    PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Waiting;

                }
            }
            public void CheckFinish()
            {
                if (Count == 1)
                {
                    Done = Exported = true;
                    Flag = StatusFlag.OK;

                    foreach (var user in PlayersArray)
                        user.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;

                    return;
                }
            }
            public bool CheckPlayers()
            {

                if (PlayersArray.Length == 2)
                {
                    var user1 = PlayersArray[0];
                    var user2 = PlayersArray[1];

                    if (user1.Socket != null && (!user1.Socket.Alive || !user1.Player.InElitePk))
                    {
                        End(user1, true);
                        return false;
                    }
                    if (user2.Socket != null && (!user2.Socket.Alive || !user2.Player.InElitePk))
                    {
                        End(user2, true);
                        return false;
                    }
                }
                else if (PlayersArray.Length == 3)
                {
                    var user1 = PlayerWaiting;

                    var array = PlayersArray.Where(a => a.Player.UID != PlayerWaiting.Player.UID).ToArray();
                    var user2 = array[0];
                    var user3 = array[1];

                    if (user1.Socket != null && (!user1.Socket.Alive || !user1.Player.InElitePk))
                    {
                        End(user1, true);
                        return false;
                    }
                    if (user2.Socket != null && (!user2.Socket.Alive || !user2.Player.InElitePk))
                    {
                        End(user2, true);
                        return false;
                    }
                    if (user3.Socket != null && (!user3.Socket.Alive || !user3.Player.InElitePk))
                    {
                        End(user3, true);
                        return false;
                    }
                }
                return true;
            }
            public unsafe void Import(ServerSockets.Packet stream)
            {
                if (Count == 1)
                {
                    foreach (var user in PlayersArray)
                        user.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                    Flag = StatusFlag.OK;

                    Exported = Done = true;
                    return;
                }

                if (CheckPlayers())
                {
                    if (Imported)
                        return;

                    Imported = true;
                    if (Done)
                        return;

                    Flag = StatusFlag.Watchable;

                    ImportTime = DateTime.Now;

                    if (PlayersArray.Length == 2)
                    {
                        Import(stream, PlayersArray[0], PlayersArray[1]);
                        Import(stream, PlayersArray[1], PlayersArray[0]);
                    }
                    else if (PlayersArray.Length == 3)
                    {
                        if (PlayerWaiting.ElitePKStats.Flag != FighterStats.StatusFlag.Lost)
                            PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Waiting;

                        if (elitepkgroup.pState == States.T_Finished)
                        {
                            Flag = StatusFlag.SwitchOut;

                            var Winner = PlayersArray.Where(p => p.ElitePKStats.Flag == FighterStats.StatusFlag.Qualified).SingleOrDefault();

                            Import(stream, PlayerWaiting, Winner);
                            Import(stream, Winner, PlayerWaiting);
                        }
                        else
                        {
                            var array = PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Waiting && p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).ToArray();

                            Import(stream, array[0], array[1]);
                            Import(stream, array[1], array[0]);
                        }
                    }
                    UpdateScore();
                }
            }
            public unsafe void Import(ServerSockets.Packet stream, Client.GameClient user, Client.GameClient Opponent)
            {
                user.ElitePKStats.Flag = FighterStats.StatusFlag.Fighting;
                user.ElitePkMatch = this;
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, 9855, DinamicID);
                user.Player.ProtectJumpAttack(10);
                user.CantAttack = DateTime.Now.AddSeconds(11);

                user.ElitePkMap = true;

                if (user.Player.MyJiangHu != null)
                {
                    if (user.Player.JiangHuActive != 0)
                    {
                        user.Player.PkMode = Role.Flags.PKMode.Capture;
                        user.Player.MyJiangHu.OnJiangMode = false;
                        user.Player.MyJiangHu.DisableJiang(user);
                    }
                }

                user.Player.SetPkMode(Role.Flags.PKMode.PK);

                stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.BeginMatch, MsgElitePKMatchUI.EffectTyp.Effect_Lose, Opponent.Player.UID, Opponent.Player.Name, TimeLeft);

                user.Send(stream);

            }

            public unsafe void UpdateScore()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKMatchStatsCreate(this);

                    foreach (var user in PlayersArray)
                    {
                        if (user.Player.Map == 9855 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }
                    foreach (var user in Watchers.GetValues())
                    {
                        if (user.Player.Map == 9855 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }

                }
            }
            public unsafe void End(Client.GameClient loser, bool Fource)
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

                            var Winner = GetOpponent(loser.Player.UID);


                            Winner.ArenaPoints += 1000;   // the loser no longer gets +1000 (arena honor farming)

                            if (Winner.Inventory.HaveSpace(1))
                                Winner.Inventory.Add(stream, 723912, 1);
                            else
                                Winner.Inventory.AddReturnedItem(stream, 723912, 1);


                            if (loser.Inventory.HaveSpace(1))
                                loser.Inventory.Add(stream, 723912, 1);
                            else
                                loser.Inventory.AddReturnedItem(stream, 723912, 1);



                            Flag = StatusFlag.OK;
                            Winner.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                            loser.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;


                            loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                            Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                            loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                            Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                            loser.ElitePkMap = false;

#if TEST
                            Console.WriteLine(Winner.Player.Name +"= winner " + loser.Player.Name + " == loser");
#endif
                        }
                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                }
                else if (Count == 3)
                {
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        if (loser != null)
                            if (loser.Player.UID != 0)
                                if (PlayerWaiting.Player.UID != 0)
                                    if (loser.Player.UID == PlayerWaiting.Player.UID)
                                    {
                                        PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;
                                        return;
                                    }
                    }

                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    if (!Imported)
                        Exported = true;

                    var Winner = GetOpponent(loser.Player.UID);

                    if (Flag == StatusFlag.SwitchOut)
                        Flag = StatusFlag.OK;

                    Winner.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                    loser.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                        Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                        Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        loser.ElitePkMap = false;

#if TEST
                        Console.WriteLine(Winner.Player.Name + "= winner " + loser.Player.Name + " == loser");
#endif
                    }
                }
            }
            public Client.GameClient GetOpponent(uint UID)
            {
                if (Count == 2)
                {
                    foreach (var user in PlayersArray)
                    {
                        if (user.Player.UID != UID)
                            return user;
                    }
                }
                else if (Count == 3)
                {
                    if (PlayerWaiting.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && PlayerWaiting.ElitePKStats.UID != user.ElitePKStats.UID)
                                return user;
                        }
                    }
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && user.ElitePKStats.Flag != FighterStats.StatusFlag.Waiting)
                                return user;
                        }
                    }
                    else if (Flag == StatusFlag.SwitchOut)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && user.ElitePKStats.Flag != FighterStats.StatusFlag.Lost)
                                return user;
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

                    foreach (var user in PlayersFighting)
                    {

                        ushort x = 0;
                        ushort y = 0;
                        elitepkgroup.Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, MsgEliteGroup.WaitingAreaID, elitepkgroup.DinamycID);
                        user.Player.RestorePkMode();
                        user.ElitePkMatch = null;
                        user.ElitePkMap = false;

                    }
                }
            }
            public void CheckMatch()
            {
                if (TimeLeft == 0)
                {
                    if (Count == 2 && !Done)
                    {
                        var user1 = PlayersArray[0];
                        var user2 = PlayersArray[1];

                        try
                        {
                            if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                End(user2, false);
                            else
                                End(user1, false);
                        }
                        catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                    }
                    else if (Count == 3 && !Done)
                    {
                        if (Flag != StatusFlag.SwitchOut)
                        {
                            var array = PlayersArray.Where(p => p.Player.UID != PlayerWaiting.Player.UID).ToArray();
                            var user1 = array[0];
                            var user2 = array[1];

                            try
                            {
                                if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                    End(user2, false);
                                else
                                    End(user1, false);
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            var user1 = PlayerWaiting;


                            var user2 = PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost && p.ElitePKStats.UID != user1.ElitePKStats.UID).SingleOrDefault();

                            try
                            {
                                if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                    End(user2, false);
                                else
                                    End(user1, false);
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
                    if (PlayerWaiting.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    if (PlayersArray.Where(p => p.Player.UID != PlayerWaiting.Player.UID).Where(p => p.ElitePKStats.Flag == FighterStats.StatusFlag.Lost).ToArray().Length == 2)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    Imported = false;
                    Done = false;
                    Exported = false;
                    Flag = StatusFlag.SwitchOut;
                    PlayerWaiting.ElitePkMatch = this;
                    PlayerWaiting.ElitePKStats = new FighterStats(PlayerWaiting.Player.UID, PlayerWaiting.Player.Name, PlayerWaiting.Player.Mesh, PlayerWaiting.Player.ServerID, PlayerWaiting.Player.RealUID, PlayerWaiting.Player.FrameID);
                }
            }

            public unsafe void BeginWatching(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (!client.Player.Alive)
                {
                    client.SendSysMesage("Please revive your character to watching that match");

                    return;
                }
                if (/*client.InQualifier() || */client.IsWatching())
                {
                    client.SendSysMesage("You're already in a match.");

                    return;
                }
                if (client.InQualifier() && client.Player.Map != 2068)
                {
                    return;
                }
                if (PlayersFighting.Length == 2)
                {
                    if (!Watchers.ContainsKey(client.Player.UID))
                    {
                        Watchers.Add(client.Player.UID, client);

                        try
                        {

                            stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, ID, (uint)PlayersFighting.Length, PlayersFighting[0].ElitePKStats.Cheers, PlayersFighting[1].ElitePKStats.Cheers);

                            foreach (var Fighter in PlayersFighting)
                                stream.AddItemElitePKWatch(Fighter.Player.UID, Fighter.Player.Name);

                            client.Send(stream.ElitePKWatchFinalize());

                            client.ElitePkWatchingGroup = this;
                            client.Teleport((ushort)Pool.GetRandom.Next(35, 70), (ushort)Pool.GetRandom.Next(35, 70), 9855, DinamicID);
                            UpdateScore();
                            UpdateWatchers();

                        }
                        catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

                    }
                }
            }
            public unsafe void UpdateWatchers()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, ID, (uint)Watchers.Count, PlayersFighting.Length > 0 ? PlayersFighting[0].ElitePKStats.Cheers : 0, PlayersFighting.Length > 1 ? PlayersFighting[1].ElitePKStats.Cheers : 0);

                    foreach (var watch in Watchers.Values)
                        stream.AddItemElitePKWatch(watch.Player.Mesh, watch.Player.Name);

                    stream.ElitePKWatchFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var user in PlayersFighting)
                        user.Send(stream);

                }
            }
            public unsafe void DoLeaveWatching(Client.GameClient client)
            {

                if (client.IsWatching() && Watchers.ContainsKey(client.Player.UID) && client.Player.Map == 9855 && client.Player.DynamicID == DinamicID)
                {
                    Watchers.Remove(client.Player.UID);
                    if (PlayersFighting.Length == 2)
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
                    client.ElitePkWatchingGroup = null;
                    client.TeleportCallBack();

                }
                client.ElitePkWatchingGroup = null;
            }
            public unsafe void DoCheer(Client.GameClient client, uint uid)
            {
                if (client.IsWatching() && !Cheerers.Contains(client.Player.UID))
                {
                    Cheerers.Add(client.Player.UID);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Fighters, 0, ID, (uint)PlayersFighting.Length, PlayersFighting[0].ElitePKStats.Cheers, PlayersFighting[1].ElitePKStats.Cheers);



                        if (PlayersFighting[0].Player.UID == uid)
                        {
                            stream.AddItemElitePKWatch(PlayersFighting[0].Player.Name, 0);
                            PlayersFighting[0].ElitePKStats.Cheers++;
                        }
                        else if (PlayersFighting[1].ElitePKStats.UID == uid)
                        {
                            stream.AddItemElitePKWatch(PlayersFighting[1].Player.Name, 0);
                            PlayersFighting[1].ElitePKStats.Cheers++;
                        }
                        stream.AddItemElitePKWatch(client.Player.Name, 0);
                        stream = stream.ElitePKWatchFinalize();

                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.Map == 9855 && user.Player.DynamicID == DinamicID)
                                user.Send(stream);
                        }
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
        public bool GetReward(Client.GameClient client, out byte Rank)
        {
            if (Top8.Length == 8)
            {
                for (int x = 0; x < Top8.Length; x++)
                {
                    if (Top8[x] != null)
                    {
                        if (Top8[x].UID == client.Player.UID)
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
        // private IDisposable Subscriber;

        public MsgServer.MsgElitePKBrackets.GuiTyp State;
        private States pState = States.T_Organize;
        public FighterStats[] Top8 = new FighterStats[0];

        public uint MatchIndex = 0;
        public DateTime StartTimer = new DateTime();
        public DateTime WaitForFinish = new DateTime();
        public Extensions.SafeDictionary<uint, Client.GameClient> HistoryPlayers;
        public System.SafeDictionary<uint, Client.GameClient> Players;

        public System.SafeDictionary<uint, Match> Matches;
        public System.SafeDictionary<uint, Match> Top4Matches;
        public System.SafeDictionary<uint, Match> ThreeQualiferMatch;
        public System.SafeDictionary<uint, Match> FinalMatch;

        private System.Counter MatchCounter;
        private DateTime pStamp;

        public ProcesType Proces;
        public MsgEliteTournament.BPGroupTyp BPGroupTyp;
        public MsgEliteGroup(MsgEliteTournament.BPGroupTyp group)
        {
            Proces = ProcesType.Dead;
            BPGroupTyp = group;
            Players = new System.SafeDictionary<uint, Client.GameClient>();
            HistoryPlayers = new Extensions.SafeDictionary<uint, Client.GameClient>();
            MatchCounter = new System.Counter((uint)((uint)group * 100000 + 100000));

            Top8 = new FighterStats[8];
            for (int x = 0; x < Top8.Length; x++)
                Top8[x] = new FighterStats(0, "None", 0, 0, 0, 0);
        }
        public void CreateWaitingMap()
        {
            Map = Pool.ServerMaps[WaitingAreaID];
            DinamycID = Map.GenerateDynamicID();
            if (BPGroupTyp == MsgEliteTournament.BPGroupTyp.EPK_BP451Plus)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            else if (BPGroupTyp == MsgEliteTournament.BPGroupTyp.EPK_BP376To450)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            else if (BPGroupTyp == MsgEliteTournament.BPGroupTyp.EPK_BP301To375)
            {
                StartTimer = DateTime.Now.AddMinutes(5);
            }
            else if (BPGroupTyp == MsgEliteTournament.BPGroupTyp.EPK_BP300Minus)
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

        public unsafe void SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (!Players.ContainsKey(client.Player.UID))
                    Players.Add(client.Player.UID, client);



                client.ElitePKStats = new FighterStats(client.Player.UID, client.Player.Name, client.Player.Mesh, client.Player.ServerID, client.Player.RealUID, client.Player.FrameID);
                client.Player.InElitePk = true;

                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, WaitingAreaID, DinamycID);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Information, MsgElitePKMatchUI.EffectTyp.Effect_Lose, client.Player.UID, client.Player.Name, 0);
                    client.Send(stream);
                }
            }
        }
        public void CheckPlayers()
        {
            List<Client.GameClient> Remover = new List<Client.GameClient>();
            foreach (var user in Players.GetValues())
            {
                if ((user.Player.Map != WaitingAreaID || user.Player.DynamicID != DinamycID) && user.IsWatching() == false)
                {
                    if (user.Player.Map != 9855)
                        Remover.Add(user);
                }
                if (user.Fake == false)
                {
                    if (user.Socket.Alive == false)
                        Remover.Add(user);
                }
            }
            foreach (var user in Remover)
                Players.Remove(user.Player.UID);
        }
        public void CreateDoubleMatchs(System.SafeDictionary<uint, Match> Array)
        {

            foreach (var user in Players.GetValues())
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

        public void CreateTripleMatchs(System.SafeDictionary<uint, Match> Array)
        {
            var array = Players.Values.ToArray();

            if (array.Length <= 16)
            {
                ushort counter = 0;
                int t1Group = array.Length - 8;
                for (int i = 0; i < t1Group; i++)
                {
                    try
                    {
                        Match match = new Match(this);
                        match.Index = (ushort)MatchIndex++;
                        match.ID = MatchCounter.Next;
                        match.AddPlayer(array[counter++]);
                        match.AddPlayer(array[counter++]);
                        Array.Add(match.ID, match);
                    }
                    catch { counter++; }
                }
                for (int i = 0; i < 8 - t1Group; i++)
                {
                    try
                    {
                        Match match = new Match(this);
                        match.Index = (ushort)MatchIndex++;
                        match.ID = MatchCounter.Next;
                        match.AddPlayer(array[counter++], FighterStats.StatusFlag.Qualified);
                        Array.Add(match.ID, match);
                    }
                    catch { counter++; }
                }
            }
            else
            {
                ushort counter = 0;
                int t3GroupCount = array.Length - 16;
                for (int i = 0; i < t3GroupCount; i++)
                {
                    int r = counter++;
                    int t = counter++;
                    int y = counter++;
                    try
                    {
                        Match match = new Match(this);
                        match.Index = (ushort)MatchIndex++;
                        match.ID = MatchCounter.Next;
                        match.AddPlayer(array[r]);
                        match.AddPlayer(array[t]);
                        match.AddPlayer(array[y]);
                        match.AddWaiting();
                        Array.Add(match.ID, match);
                    }
                    catch { }
                }
                int t2GroupCount = array.Length - counter;
                for (int i = 0; i < t2GroupCount / 2; i++)
                {
                    int r = counter++;
                    int t = counter++;
                    try
                    {
                        Match match = new Match(this);
                        match.Index = (ushort)MatchIndex++;
                        match.ID = MatchCounter.Next;
                        match.AddPlayer(array[r]);
                        match.AddPlayer(array[t]);
                        Array.Add(match.ID, match);
                    }
                    catch { }
                }
            }


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
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Program.ServerConfig.IsInterServer)
                {

                    stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, (uint)this.BPGroupTyp, State, (uint)Top8.Length, 0);
                    for (int x = 0; x < Top8.Length; x++)
                        stream.AddItemElitePkRanking(Top8[x], (byte)(x + 1));
                    stream.InterServerElitePkRankingFinalize();
                    MsgInterServer.PipeServer.Send(stream);


                }
                foreach (var clients in Pool.GamePoll.Values)
                    MsgSchedules.ElitePkTournament.GetReward(clients, stream);
                State = MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;
                Proces = ProcesType.Dead;
                if (Players != null)
                    Players.Clear();
                if (Matches != null)
                    Matches.Clear();
                if (Top4Matches != null)
                    Top4Matches.Clear();
                if (ThreeQualiferMatch != null)
                    ThreeQualiferMatch.Clear();
                if (FinalMatch != null)
                    FinalMatch.Clear();
                if (HistoryPlayers != null)
                {
                    if (HistoryPlayers.Count > 0)
                        foreach (var hero in HistoryPlayers.Values)
                            if (hero.Player.Map == WaitingAreaID)
                                hero.Teleport(Places.TwinCity);
                    HistoryPlayers.Clear();
                }
            }
        }
        public DateTime ConstructTop8 = DateTime.Now;

        public void timerCallback()
        {
            try
            {
                if (DateTime.Now > StartTimer && Proces == ProcesType.Idle)
                {
                    // no sign-ups: keep the last Top 8 so offline winners can still claim
                    if (Players.Count == 0)
                    {
                        Finish();
                        return;
                    }
                    Top8 = new FighterStats[8];
                    for (int x = 0; x < Top8.Length; x++)
                        Top8[x] = new FighterStats(0, "None", 0, 0, 0, 0);
                    Proces = ProcesType.Alive;
                }
                if (Players.Count == 0)
                    return;
                if (Proces == ProcesType.Alive)
                {
                    if (State == MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking)
                    {

                        CheckPlayers();
                        if (Players.Count == 1)
                        {
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_ReconstructTop;
                            WaitForFinish = DateTime.Now.AddMinutes(1);
                            ActiveArena(false);
                            foreach (var Client in this.Players.Values)
                            {
                                if (Client.Player.InTeamPk)
                                    Client.Player.InTeamPk = false;
                                Client.Teleport(Places.TwinCity);
                            }
                            Top8[0] = this.Players.Values.FirstOrDefault().ElitePKStats;
                        }
                        else if (Players.Count == 2)
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top1;
                        else if (Players.Count > 2 && Players.Count <= 4)
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top2Qualifier;
                        else if (Players.Count > 4 && Players.Count <= 8)
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top4Qualifier;
                        else if (Players.Count <= 24)
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier;
                        else
                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Knockout;

                        pState = States.T_Organize;
                    }
                    switch (State)
                    {
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_ReconstructTop:
                            {
                                if (DateTime.Now > WaitForFinish)
                                {
                                    Finish();//finish
                                }
                                break;
                            }
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Knockout:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            MatchIndex = 0;
                                            Matches = new System.SafeDictionary<uint, MsgEliteGroup.Match>();
                                            CreateDoubleMatchs(Matches);
                                            pStamp = DateTime.Now.AddSeconds(60.0);
                                            pState = MsgEliteGroup.States.T_Import;
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
                                                    State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    List<Client.GameClient> removers = new List<Client.GameClient>();
                                                    foreach (var user in Players.GetValues())
                                                    {
                                                        if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            removers.Add(user);
                                                    }
                                                    foreach (var player in removers)
                                                    {
                                                        Players.Remove(player.Player.UID);
                                                        player.Teleport(Places.TwinCity);
                                                    }

                                                    Matches.Clear();
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
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
                                                    State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    List<Client.GameClient> removers = new List<Client.GameClient>();
                                                    foreach (var user in Players.GetValues())
                                                    {
                                                        if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            removers.Add(user);
                                                    }
                                                    foreach (var player in removers)
                                                    {
                                                        Players.Remove(player.Player.UID);
                                                        player.Teleport(Places.TwinCity);
                                                    }

                                                    Matches.Clear();
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;

                            }
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top4Qualifier:
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

                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
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
                                                    int i = 4;
                                                    foreach (var match in Matches.Values)
                                                    {
                                                        foreach (var user in match.Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats != null && user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                Top8[Math.Min(7, i++)] = user.ElitePKStats.Clone();
                                                            }
                                                        }

                                                    }

                                                    List<Client.GameClient> removers = new List<Client.GameClient>();
                                                    foreach (var user in Players.GetValues())
                                                    {
                                                        if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                        }
                                                    }
                                                    foreach (var player in removers)
                                                    {
                                                        Players.Remove(player.Player.UID);
                                                        player.Teleport(Places.TwinCity);
                                                    }

                                                    State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top2Qualifier:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            MatchIndex = 0;
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
                                                n_match.AddPlayer(arraymatchs[0].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                n_match.AddPlayer(arraymatchs[2].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                Match m_match = new Match(this);
                                                m_match.Index = (ushort)MatchIndex++;
                                                m_match.ID = MatchCounter.Next;
                                                m_match.AddPlayer(arraymatchs[1].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                if (arraymatchs.Length > 3)
                                                    m_match.AddPlayer(arraymatchs[3].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                Top4Matches.Add(m_match.ID, m_match);
                                                Top4Matches.Add(n_match.ID, n_match);
                                            }


                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in Top4Matches.GetValues())
                                                match.CheckFinish();

                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                            if (Top4Matches != null)
                                                SendBrackets(Top4Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);

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


                                                    List<Client.GameClient> removers = new List<Client.GameClient>();
                                                    ThreeQualiferMatch = new System.SafeDictionary<uint, Match>();

                                                    foreach (var user in Players.GetValues())
                                                    {
                                                        if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                        }
                                                    }
                                                    if (removers.Count == 1)//for 3 players.
                                                    {
                                                        foreach (var player in removers)
                                                        {
                                                            Players.Remove(player.Player.UID);
                                                            Top8[2] = player.ElitePKStats;
                                                        }
                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    }
                                                    else
                                                    {
                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top3;
                                                        pState = States.T_Organize;

                                                        MatchIndex = 0;
                                                        Match n_match = new Match(this);
                                                        n_match.Index = (ushort)MatchIndex++;
                                                        n_match.ID = MatchCounter.Next;
                                                        foreach (var player in removers)
                                                        {
                                                            Players.Remove(player.Player.UID);
                                                            n_match.AddPlayer(player);
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
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top3:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in ThreeQualiferMatch.GetValues())
                                                match.CheckFinish();

                                            SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                            if (ThreeQualiferMatch != null)
                                                SendBrackets(ThreeQualiferMatch.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
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

                                                    foreach (var match in ThreeQualiferMatch.GetValues())
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
                                                foreach (var match in ThreeQualiferMatch.GetValues())
                                                {
                                                    match.CheckMatch();
                                                    match.Export();
                                                    if (match.IsFinishd())
                                                        FinishMatchs++;
                                                }
                                                if (FinishMatchs == ThreeQualiferMatch.Count)
                                                {
                                                    State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    foreach (var match in ThreeQualiferMatch.GetValues())
                                                    {
                                                        foreach (var user in match.PlayersArray)
                                                        {
                                                            if (user.ElitePKStats.Flag != FighterStats.StatusFlag.Qualified)
                                                            {
                                                                Top8[3] = user.ElitePKStats.Clone();
                                                            }
                                                            else
                                                                Top8[2] = user.ElitePKStats.Clone();
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top1:
                            {
                                switch (pState)
                                {
                                    case States.T_Organize:
                                        {
                                            ThreeQualiferMatch = null;
                                            FinalMatch = new System.SafeDictionary<uint, Match>();

                                            MatchIndex = 0;
                                            CreateDoubleMatchs(FinalMatch);

                                            pStamp = DateTime.Now.AddSeconds(60);
                                            pState = States.T_Import;

                                            foreach (var match in FinalMatch.GetValues())
                                                match.CheckFinish();

                                            if (Matches != null)
                                                SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                            if (Top4Matches != null)
                                                SendBrackets(Top4Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
                                            if (FinalMatch != null)
                                                SendBrackets(FinalMatch.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);

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

                                                    List<Client.GameClient> removers = new List<Client.GameClient>();

                                                    foreach (var user in Players.GetValues())
                                                    {
                                                        if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                        {
                                                            removers.Add(user);
                                                        }
                                                        else
                                                            Top8[0] = user.ElitePKStats.Clone();
                                                    }

                                                    foreach (var player in removers)
                                                    {
                                                        Top8[1] = player.ElitePKStats.Clone();
                                                        Players.Remove(player.Player.UID);
                                                    }

                                                    State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

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
        public unsafe void ActiveArena(bool active)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.ElitePKBracketsCreate(MsgElitePKBrackets.Action.EPK_State, 0, 0, BPGroupTyp, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, (uint)(active ? 1 : 0));
                stream.ElitePKBracketsFinalize();
                Server.SendGlobalPacket(stream);
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
        public unsafe void ListMatch(Client.GameClient user, List<Match> Matchs, ushort page = 0, ushort Length = 0, bool sendtoall = false, MsgServer.MsgElitePKBrackets.Action type = MsgServer.MsgElitePKBrackets.Action.InitialList, ushort PacketNo = 0)
        {
            if (Matchs.Count > 0)
            {
                uint count = 1;
                if (Matchs.Count > 1) count++;
                if (Matchs.Count > 2) count++;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.ElitePKBracketsCreate(type, page, (ushort)Length, BPGroupTyp, State, TimeLeft, (ushort)count, PacketNo, 1);
                    stream.AddItemElitePKBrackets(Matchs[0]);
                    if (Matchs.Count > 1) stream.AddItemElitePKBrackets(Matchs[1]);
                    if (Matchs.Count > 2) stream.AddItemElitePKBrackets(Matchs[2]);
                    stream.ElitePKBracketsFinalize();
                    if (user != null) user.Send(stream);
                    if (sendtoall) Server.SendGlobalPacket(stream);
                }
            }
            if (Matchs.Count > 3)
            {
                uint count = 1;
                if (Matchs.Count > 4) count++;
                if (Matchs.Count > 5) count++;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.ElitePKBracketsCreate(type, page, (ushort)Length, BPGroupTyp, State, TimeLeft, (ushort)count, PacketNo, 0);
                    stream.AddItemElitePKBrackets(Matchs[3]);
                    if (Matchs.Count > 4) stream.AddItemElitePKBrackets(Matchs[4]);
                    if (Matchs.Count > 5) stream.AddItemElitePKBrackets(Matchs[5]);
                    stream.ElitePKBracketsFinalize();
                    if (user != null) user.Send(stream);
                    if (sendtoall) Server.SendGlobalPacket(stream);

                }
            }
            if (Matchs.Count > 6)
            {
                uint count = 1;
                if (Matchs.Count > 7) count++;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.ElitePKBracketsCreate(type, page, (ushort)Length, BPGroupTyp, State, TimeLeft, (ushort)count, PacketNo, 0);
                    stream.AddItemElitePKBrackets(Matchs[6]);
                    if (Matchs.Count > 7) stream.AddItemElitePKBrackets(Matchs[7]);
                    stream.ElitePKBracketsFinalize();
                    if (user != null) user.Send(stream);
                    if (sendtoall) Server.SendGlobalPacket(stream);

                }
            }
        }
        public unsafe void SendBrackets(Match[] matches, Client.GameClient user, bool sendtoall = false, ushort page = 0
            , MsgServer.MsgElitePKBrackets.Action type = MsgServer.MsgElitePKBrackets.Action.InitialList, bool sendmatch = false, ushort PacketNo = 0)
        {
            if (matches == null)
                return;
            switch (State)
            {
                default:
                    {
                        if (matches == null) break;
                        ListMatch(user, matches.ToList(), page, (ushort)(matches.Length), sendtoall, type);
                        break;
                    }
                #region GUI_Knockout
                case MsgElitePKBrackets.GuiTyp.GUI_Knockout:
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
                case MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
                    {
                        ListMatch(user, matches.ToList(), page, (ushort)matches.Length, sendtoall);
                        break;
                    }
                #endregion
            }
        }
    }
}

