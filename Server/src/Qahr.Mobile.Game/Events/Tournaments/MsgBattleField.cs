//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Qahr.Mobile.Game.MsgServer;
//namespace Qahr.Mobile.Game.MsgTournaments
//{
//    public class Participant
//    {
//        public string Name = "";
//        public uint UID = 0;
//        public uint BattlePoints;

//        public Participant()
//        {

//        }

//        public Participant(Client.GameClient user)
//        {
//            Name = user.Player.Name;
//            UID = user.Player.UID;
//            BattlePoints = user.Player.BattleFieldPoints;
//        }
//        public override string ToString()
//        {
//            Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');
//            line.Add(UID).Add(BattlePoints).Add(Name);
//            return line.Close();
//        }
//    }
//    public class MsgBattleField : ITournament
//    {
//        public ProcesType Process { get; set; }
//        private DateTime StartTimer = new DateTime();

//        public List<Participant> Rank3 = new List<Participant>();
//        public TournamentType Type { get; set; }
//        public MsgBattleField(TournamentType _type)
//        {
//            Type = _type;
//            Process = ProcesType.Dead;
//            StartTimer= DateTime.Now;
//        }

//        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
//        {
//            if (Process == ProcesType.Idle)
//            {
//                user.Player.BattleFieldPoints = 0;
//                user.Teleport(196, 214, 1081);
//                return true;
//            }
//            return false;
//        }
//        public void Open()
//        {
//            if (Process == ProcesType.Dead)
//            {
//                StartTimer= DateTime.Now;
//                Process = ProcesType.Idle;
//                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
//                {
//                    client.Player.MessageBox(
//#if Arabic
//                        "BattleField is about to begin! Will you join it?"
//#else
//"Battlefield is about to begin! Will you join it?"
//#endif

//                        , new Action<Client.GameClient>(p =>
//                    {
//                        p.Teleport(192, 223, 1036);
//                    }), null, 60);
//                }
//            }
          
//        }
//        public bool InTournament(Client.GameClient user)
//        {
//            return
//                user.Player.Map == 1081
//                || user.Player.Map == 2060
//                || user.Player.Map == 1080;
//        }
//        private List<Client.GameClient> Participants()
//        {
//            List<Client.GameClient> Participants = new List<Client.GameClient>();

//            foreach (var user in Pool.GamePoll.Values)
//            {
//                if (user.Player.Map == 2060 || user.Player.Map == 1080 || user.Player.Map == 1081)
//                {
//                    if (user.Player.DynamicID == 0)
//                        Participants.Add(user);
//                }
//            }
//            return Participants;
//        }
//        public void GetOut(Client.GameClient client)
//        {
//            if (client.Player.BattleFieldPoints > 0)
//            {
//#if Arabic
//                 client.CreateBoxDialog("You've received " + (client.Player.BattleFieldPoints / 10).ToString() + " ConquerPoints.");
//#else
//                client.CreateBoxDialog("You've received " + (client.Player.BattleFieldPoints / 10).ToString() + " ConquerPoints.");
//#endif

//                client.Player.ConquerPoints += (uint)(client.Player.BattleFieldPoints / 10);
//                client.Player.BattleFieldPoints = 0;
//            }
//            client.Teleport(357,303, 1002);
//        }

//        public void CheckUp()
//        {
//            if (Process == ProcesType.Idle)
//            {
//                if (StartTimer.AddSeconds(60) < DateTime.Now)
//                {
//                    Process = ProcesType.Alive;

//                    var map = Pool.ServerMaps[2060];
//                    if (!map.ContainMobID(4220))
//                    {
//                        using (var rec = new ServerSockets.RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            Server.AddMapMonster(stream, map, 4220, 123, 129, 18, 18, 1);
//#if Arabic
//                                  Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("The NemesisTyrant have spawned in the BattleField Map 3 on (123, 129) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                      
//#else
//                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("The NemesisTyrant have spawned in the BattleField Map 3 on (123, 129) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                      
//#endif
//                        }
//                    }
//                }
//            }
//            if (Process == ProcesType.Alive)
//            {
//                if (StartTimer.AddMinutes(15) < DateTime.Now)
//                {
//                    Process = ProcesType.Dead;

//                    List<Client.GameClient> Users = Participants();
//                    CreateRanks(Users);
//                    foreach (var user in Users)
//                        GetOut(user);

//                }
//            }
//        }
//        public void CreateRanks(List<Client.GameClient> Users)
//        {
//            Rank3.Clear();

//            List<Participant> Rank = new List<Participant>();
//            foreach (var user in Users)
//                Rank.Add(new Participant(user));

            

//            var array = Rank.OrderByDescending(p => p.BattlePoints);

//            int count = 0;
//            foreach (var user in array)
//            {
//                if (count == 3)
//                    break;
//                Rank3.Add(user);
//                count++;
//            }

//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                for (int x = 0; x < Rank3.Count; x++)
//                {
//                    var element = Rank3[x];
//#if Arabic
//                         Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulation! Rank " + (x + 1).ToString() + " " + element.Name + " with " + element.BattlePoints.ToString() + " BattleFieldPoints. .", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
               
//#else
//                    Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulation! Rank " + (x + 1).ToString() + " " + element.Name + " with " + element.BattlePoints.ToString() + " BattleFieldPoints. .", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
               
//#endif
//                }
//            }
//        }
//        public void Save()
//        {
//            using (Database.DBActions.Write write = new Database.DBActions.Write("BattleField.txt"))
//            {
//                foreach (var user in Rank3)
//                    write.Add(user.ToString());
//                write.Execute(Database.DBActions.Mode.Open);
//            }
//        }
//        public void Load()
//        {
//            using (Database.DBActions.Read reader = new Database.DBActions.Read("BattleField.txt"))
//            {
//                if (reader.Reader())
//                {
//                    for (int x = 0; x < reader.Count; x++)
//                    {
//                        Participant part = new Participant();
//                        Database.DBActions.ReadLine readline = new Database.DBActions.ReadLine(reader.ReadString(""), '/');
//                        part.UID = readline.Read((uint)0);
//                        part.BattlePoints = readline.Read((uint)0);
//                        part.Name = readline.Read("None");
//                        Rank3.Add(part);
//                    }
//                }
//            }
//        }
//    }
//}
