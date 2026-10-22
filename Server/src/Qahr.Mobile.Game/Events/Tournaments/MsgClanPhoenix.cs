
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Concurrent;

//namespace Qahr.Mobile.Game.MsgTournaments
//{
//    public class MsgClanPhoenix
//    {
//        public static System.SafeDictionary<uint, Winner> ScoreList = new System.SafeDictionary<uint, Winner>();
//        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures;
//        public Winner RoundOwner;
//        public static ushort MapID = 11996;
//        public ProcesType Proces;
//        public class Winner
//        {
//            public uint ClanID;
//            public uint Score;
//            public string ClanName;
//        }
//        public MsgClanPhoenix()
//        {
//            Proces = ProcesType.Dead;
//            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
//        }

//        public void Load()
//        {
//            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Pool.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 16061));
//        }

//        public void CheckUP()
//        {
//            DateTime now = DateTime.Now;

//            if (Proces == ProcesType.Dead &&
//                now.DayOfWeek == DayOfWeek.Thursday &&
//                now.Hour == 21 &&
//                now.Minute == 0)
//            {
//                this.Start();
//            }

//            if (Proces == ProcesType.Alive &&
//                now.DayOfWeek == DayOfWeek.Thursday &&
//                now.Hour == 22 &&
//                now.Minute == 0)
//            {
//                this.End();
//            }
//        }

//        public void Start()
//        {
//            RoundOwner = null;
//            Proces = ProcesType.Alive;
//            ScoreList = new System.SafeDictionary<uint, Winner>();
//            MsgSchedules.SendInvitation("Clan War just started do you want to join now ? ", "", 209, 259, 1011, 0, 60);
//            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "ClanPhoenix";
//        }
//        public void Reset(ServerSockets.Packet stream)
//        {
//            ScoreList = new System.SafeDictionary<uint, Winner>();

//            foreach (var npc in Furnitures.Values)
//                npc.HitPoints = npc.MaxHitPoints;

//            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
//            var users = Pool.GamePoll.Values.Where(u => u != null && u.Player.Map == MapID && Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
//            if (users != null)
//            {
//                foreach (var user in users)
//                {
//                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 2);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
//                    stream = upd.GetArray(stream);
//                    user.Send(stream);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
//                    stream = upd.GetArray(stream);
//                    user.Send(stream);
//                    user.Send(Pole.GetArray(stream, false));

//                }
//            }
//        }
//        public void FinishRound(ServerSockets.Packet stream)
//        {
//            SortScores(true);
//            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.ClanName;
//            Reset(stream);
//        }
//        public void End()
//        {
//            if (Proces == ProcesType.Alive)
//            {
//                Proces = ProcesType.Dead;
//                ScoreList.Clear();
//                if (RoundOwner != null)
//                {
//                    var ClanWinner = Pool.GamePoll.Values.Where(p => p.Player.MyClan != null && p.Player.MyClan.ID == RoundOwner.ClanID && p.Player.Map == MapID).ToArray();
//                    if (ClanWinner != null)
//                    {
//                        foreach (var User in ClanWinner)
//                        {
//                            using (var rec = new ServerSockets.RecycledPacket())
//                            {
//                                var stream = rec.GetStream();
//                                if (User.Player.ClanRank == (ushort)Role.Instance.Clan.Ranks.Leader)
//                                {
//                                    User.Player.OnlineMinutes += 5000;
//                                    User.Inventory.AddItemWitchStack(3315881, 0, 2, stream, false); // 2x  Bag = 2b CPs
//                                    User.CreateBoxDialog("You Received x2 Bags 2.000.000.000 CPs - 5.000 Online Points ");

//                                }
//                                //if (User.Player.ClanRank == (ushort)Role.Instance.Clan.Ranks.Member)
//                                //{
//                                //    User.CreateBoxDialog("You Received 10.000.000 CPs - 1x P14Anima ");
//                                //    User.Player.ConquerPoints += 10000000;
//                                //    User.Inventory.AddItemWitchStack(4200014, 0, 1, stream, false); // 1x P14Anima
//                                //}
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        public void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Instance.Clan Clan)
//        {
//            if (!ScoreList.ContainsKey(Clan.ID))
//                ScoreList.Add(Clan.ID, new Winner() { ClanName = Clan.Name, ClanID = Clan.ID, Score = Score });
//            else
//                ScoreList[Clan.ID].Score += Score;

//            SortScores();

//            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
//            {
//                FinishRound(stream);
//                return;
//            }
//        }
//        private void SortScores(bool gePhoienxner = false)
//        {
//            if (Proces != ProcesType.Dead)
//            {
//                var Array = ScoreList.Values.ToArray();
//                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
//                for (int x = 0; x < DescendingList.Length; x++)
//                {
//                    var element = DescendingList[x];
//                    if (x == 0 && gePhoienxner)
//                    {
//                        RoundOwner = element;
//                    }
//                    using (var rec = new ServerSockets.RecycledPacket())
//                    {
//                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.ClanName + " (" + element.Score + ")"
//                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//                        SendMapPacket(msg.GetArray(rec.GetStream()));
//                    }
//                    if (x == 4)
//                        break;
//                }
//            }
//        }
//        public void SendMapPacket(ServerSockets.Packet packet)
//        {
//            foreach (var client in Pool.ServerMaps[MapID].Values)
//                client.Send(packet);
//        }
//    }
//}
