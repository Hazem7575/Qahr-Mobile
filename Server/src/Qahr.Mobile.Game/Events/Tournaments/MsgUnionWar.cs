//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections.Concurrent;

//namespace Qahr.Mobile.Game.MsgTournaments
//{
//    public class MsgUnionWar
//    {

//        public bool SendInvitation = false;
//        public class info
//        {

//            public uint UnionID;
//            public string Name;
//            public uint Score;

//            public int LeaderReward = 1;
//        }

//        public List<uint> RewardLeader = new List<uint>();
//        public List<uint> RewardDeputiLeader = new List<uint>();

//        public DateTime StampRound = new DateTime();
//        public DateTime StampShuffleScore = new DateTime();

//        public ProcesType Proces { get; set; }

//        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
//        public ConcurrentDictionary<uint, info> ScoreList;
//        public info Winner;
//        public Role.GameMap Map;
//        public MsgUnionWar()
//        {
//            Proces = ProcesType.Dead;
//            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
//            ScoreList = new ConcurrentDictionary<uint, info>();
//            Winner = new info() { Name = "UnionWar", Score = 100, UnionID = 0 };

//        }

//        public void Create()
//        {
//            Map = Pool.ServerMaps[13140]; AddNpc(148, 227);
//        }
//        public Role.SobNpc Pole;
//        public void AddNpc(ushort x, ushort y)
//        {
//            if (Map.View.Contain(13140, x, y))
//                return;
//            Pole = new Role.SobNpc();
//            Pole.X = x;
//            Pole.Map = Map.ID;
//            Pole.ObjType = Role.MapObjectType.SobNpc;
//            Pole.Y = y;
//            Pole.UID = 13140;
//            Pole.Type = Role.Flags.NpcType.Stake;
//            Pole.Mesh = (Role.SobNpc.StaticMesh)1137;
//            Pole.Name = Winner.Name;
//            Pole.HitPoints = 1200000000;
//            Pole.MaxHitPoints = 1200000000;
//            Pole.Sort = 10;
//            Map.View.EnterMap<Role.IMapObj>(Pole);
//            Map.SetFlagNpc(Pole.X, Pole.Y);
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                foreach (var user in Map.View.Roles(Role.MapObjectType.Player, Pole.X, Pole.Y))
//                {
//                    user.Send(Pole.GetArray(stream, false));
//                }
//            }
//            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Pole);
//        }

//        public void ResetPole(ServerSockets.Packet stream)
//        {

//            foreach (var npc in Furnitures.Values)
//                npc.HitPoints = npc.MaxHitPoints;

//            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
//            var users = Pool.GamePoll.Values.Where(u => u != null && u.Player.Map == 13140 && Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
//            if (users != null)
//            {
//                foreach (var user in users)
//                {
//                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 1);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (ulong)Pole.Mesh);
//                    stream = upd.GetArray(stream);
//                    user.Send(stream);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (ulong)Pole.HitPoints);
//                    stream = upd.GetArray(stream);
//                    user.Send(stream);
//                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
//                        user.Send(Pole.GetArray(stream, false));
//                }
//            }
//        }
//        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
//        {
//            ResetPole(stream);
//        }
//        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
//        {
//            foreach (var client in Pool.GamePoll.Values)
//            {
//                if (client.Player.Map == 13140)
//                {
//                    client.Send(packet);
//                }
//            }
//        }
//        internal unsafe void CompleteUnionwar()
//        {
//            SendInvitation = false;
//            ShuffleGuildScores();
//            Proces = ProcesType.Dead;
//            ScoreList.Clear();
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();

//                Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the UnionWar with a score of " + Winner.Score.ToString() + ""
//                  , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
//                Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the UnionWar with a score of " + Winner.Score.ToString() + ""
//                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
//            }

//            RewardLeader.Clear();
//            Winner.LeaderReward = 1;
//        }


//        public void CheckUP()
//        {
//            DateTime now = DateTime.Now;

//            if (Proces == ProcesType.Dead &&
//                now.DayOfWeek == DayOfWeek.Monday &&
//                now.Hour == 21 &&
//                now.Minute == 0 &&
//                now.Second < 2)
//            {
//                this.Start();
//            }

//            if (Proces == ProcesType.Alive &&
//                now.DayOfWeek == DayOfWeek.Monday)
//            {
//                if (now.Hour == 21 && now.Minute == 55 && now.Second < 2)
//                {
//                    MsgSchedules.SendSysMesage(
//                        "[Union War] Will End After 5 Minutes",
//                        MsgServer.MsgMessage.ChatMode.Center);
//                }

//                if (now.Hour == 22 && now.Minute == 0 && now.Second < 2)
//                {
//                    this.FinishRound();
//                }
//            }
//        }
//        internal unsafe void Start()
//        {
//            Proces = ProcesType.Alive;
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                ResetFurnitures(stream);
//                ScoreList.Clear();
//                MsgSchedules.SendInvitation("Union War has begun Would you like to join ! ", " ", 410, 354, 1002, 0, 90);
//                Server.SendGlobalPacket(new MsgServer.MsgMessage("Union War war has started"
//                   , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
//                Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "UnionWar";


//            }
//        }

//        internal unsafe void FinishRound()
//        {
//            ShuffleGuildScores(true);
//            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
//            //Proces = ProcesType.Idle;
//            ScoreList.Clear();
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();

//                Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ",they've won the UnionWar round with a score of  " + Winner.Score.ToString() + ""
//                 , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
//                Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the UnionWar round with a score of  " + Winner.Score.ToString() + ""
//                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
//                ResetFurnitures(stream);
//            }
//            StampRound = DateTime.Now.AddSeconds(3);

//        }

//        internal void UpdateScore(Role.Player client, uint Damage)
//        {
//            if (client.MyUnion == null)
//                return;
//            if (Proces == ProcesType.Alive)
//            {
//                if (!ScoreList.ContainsKey(client.MyUnion.UID))
//                {
//                    ScoreList.TryAdd(client.MyUnion.UID, new info() { UnionID = client.MyUnion.UID, Name = client.MyUnion.Name, Score = Damage });
//                }
//                else
//                {
//                    ScoreList[client.MyUnion.UID].Score += Damage;
//                }
//                ShuffleGuildScores();
//                using (var rec = new ServerSockets.RecycledPacket())
//                {
//                    var stream = rec.GetStream();
//                    foreach (var Bas in Furnitures.Values)
//                    {
//                        if ((Role.SobNpc.StaticMesh)Bas.Mesh == Role.SobNpc.StaticMesh.Pole)
//                        {
//                            foreach (var user in Pool.GamePoll.Values)
//                            {
//                                if (user.Player.Map == Map.ID)
//                                {
//                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, Bas.X, Bas.Y) <= Role.SobNpc.SeedDistrance)
//                                    {
//                                        var upd = new MsgServer.MsgUpdate(stream, Bas.UID, 2);
//                                        stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Bas.HitPoints);
//                                        stream = upd.GetArray(stream);
//                                        user.Send(stream);
//                                        stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.MaxHitpoints, Bas.MaxHitPoints);
//                                        stream = upd.GetArray(stream);
//                                        user.Send(stream);
//                                        user.Send(Bas.GetArray(stream, false));
//                                    }
//                                    else
//                                    {
//                                        continue;
//                                    }
//                                }
//                                else
//                                {
//                                    continue;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            continue;
//                        }
//                    }
//                }
//                if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
//                    FinishRound();
//            }
//        }

//        internal unsafe void ShuffleGuildScores(bool createWinned = false)
//        {
//            if (Proces != ProcesType.Dead)
//            {
//                StampShuffleScore = DateTime.Now.AddSeconds(3);
//                var Array = ScoreList.Values.ToArray();
//                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
//                for (int x = 0; x < DescendingList.Length; x++)
//                {
//                    var element = DescendingList[x];
//                    if (x == 0 && createWinned)
//                        Winner = element;
//                    using (var rec = new ServerSockets.RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score.ToString() + ")"
//                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//                        SendMapPacket(msg.GetArray(stream));

//                    }
//                    if (x == 4)
//                        break;
//                }
//            }
//        }
//    }
//}
