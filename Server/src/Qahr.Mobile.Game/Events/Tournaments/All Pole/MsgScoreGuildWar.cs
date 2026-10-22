//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Qahr.Mobile.Game.MsgTournaments
//{
//    public class MsgTopWarScore
//    {
//        public System.SafeDictionary<uint, Winner> TopWarScore = new System.SafeDictionary<uint, Winner>();
//        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures;
//        public static ushort MapID = 10504;
//        public ProcesType Proces;
//        public static uint PoleUID = 100000;
//        public class Winner
//        {
//            public uint GuildID;
//            public long Score;
//            public string GuildName;
//        }
//        public MsgTopWarScore()
//        {
//            Proces = ProcesType.Dead;
//            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
//        }

//        public void Load()
//        {
//            if (!Pool.ServerMaps.ContainsKey(MapID))
//            {
//                MyConsole.WriteLine("Error: Map " + MapID + " not found for Score Guild War.", ConsoleColor.Red);
//                return;
//            }
//            var pole = Pool.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, PoleUID);
//            if (pole == null)
//            {
//                pole = new Role.SobNpc();
//                pole.ObjType = Role.MapObjectType.SobNpc;
//                pole.UID = PoleUID;
//                pole.Name = "Score Guild War";
//                pole.MaxHitPoints = 500000000;
//                pole.HitPoints = 500000000;
//                pole.Map = MapID;
//                pole.X = 166;
//                pole.Y = 175;
//                pole.Type = Role.Flags.NpcType.Pole;
//                pole.Mesh = Role.SobNpc.StaticMesh.Pole;
//                Pool.ServerMaps[MapID].View.EnterMap<Role.IMapObj>(pole);
//            }
//            if (!Furnitures.ContainsKey(Role.SobNpc.StaticMesh.Pole))
//            {
//                Furnitures.Add(Role.SobNpc.StaticMesh.Pole, pole);
//            }
//        }
//        public void CheckUP()
//        {
//            DateTime now = DateTime.Now;

//            if (Proces == ProcesType.Dead &&
//                now.DayOfWeek == DayOfWeek.Tuesday &&
//                now.Hour == 21 &&
//                now.Minute == 0)
//            {
//                this.Start();
//            }
//            if (Proces == ProcesType.Alive &&
//                now.DayOfWeek == DayOfWeek.Tuesday)
//            {
//                if (now.Hour == 21 && now.Minute == 55 && now.Second == 20)
//                {
//                    MsgSchedules.SendSysMesage(
//                        "[Score Guild War] Will End After 5 Minutes.",
//                        MsgServer.MsgMessage.ChatMode.Center);
//                }
//                if (now.Hour == 22 && now.Minute == 0)
//                {
//                    this.End();
//                }
//            }
//        }


//        public void Start()
//        {
//            Proces = ProcesType.Alive;
//            TopWarScore = new System.SafeDictionary<uint, Winner>();
//            MsgSchedules.SendInvitation("Score Guild War just started do you want to join now ? ", "", 410, 354, 1002, 0, 60);

//        }
//        public void Reset(ServerSockets.Packet stream)
//        {
//            TopWarScore = new System.SafeDictionary<uint, Winner>();

//            foreach (var npc in Furnitures.Values)
//                npc.HitPoints = npc.MaxHitPoints;

//            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
//            var users = Pool.GamePoll.Values.Where(u => u != null && u.Player.Map == MapID && Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
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
//        public void FinishRound(ServerSockets.Packet stream)
//        {
//            SortScores();
//            var RoundOwner = MsgSchedules.TopWarScore.TopWarScore.Values.OrderByDescending(i => i.Score).FirstOrDefault();
//            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.GuildName;
//            MsgSchedules.SendSysMesage("The Round Ownered By Guild " + RoundOwner.GuildName + ".", MsgServer.MsgMessage.ChatMode.Center);
//            Reset(stream);
//        }
//        public void End()
//        {
//            if (Proces == ProcesType.Alive)
//            {
//                Proces = ProcesType.Dead;
//                var RoundOwner = MsgSchedules.TopWarScore.TopWarScore.Values.OrderByDescending(i => i.Score).FirstOrDefault();
//                MsgSchedules.SendSysMesage(RoundOwner == null ? "Score Guild War has ended and there is no winner" : " Guild " + RoundOwner.GuildName + ", He is the winner of Score Guild War.", MsgServer.MsgMessage.ChatMode.Center);
//            }
//        }
//        public void UpdateScore(ServerSockets.Packet stream, long Score, Role.Instance.Guild Guild)
//        {
//            if (!TopWarScore.ContainsKey(Guild.Info.GuildID))
//                TopWarScore.Add(Guild.Info.GuildID, new Winner() { GuildName = Guild.GuildName, GuildID = Guild.Info.GuildID, Score = Score });
//            else
//                TopWarScore[Guild.Info.GuildID].Score += Score;

//            // MyConsole.WriteLine("[ScoreWar] Guild " + Guild.GuildName + " Score updated to: " + TopWarScore[Guild.Info.GuildID].Score, ConsoleColor.Cyan);

//            SortScores();
//            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 100000000)
//            {
//                Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints = Furnitures[Role.SobNpc.StaticMesh.Pole].MaxHitPoints;
//            }
//        }
//        private void SortScores()
//        {
//            if (Proces != ProcesType.Dead)
//            {
//                var DescendingList = TopWarScore.GetValues().OrderByDescending(p => p.Score).ToArray();
//                for (int x = 0; x < DescendingList.Length; x++)
//                {
//                    var element = DescendingList[x];
//                    using (var rec = new ServerSockets.RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.GuildName + " (" + element.Score + ")"
//                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//                        if (x == 0) msg.winning = 1; // Signal UI to appear
//                        SendMapPacket(msg.GetArray(stream, (uint)(x + 1)));
//                    }
//                    if (x == 4)
//                        break;
//                }
//            }
//        }
//        public void SendMapPacket(ServerSockets.Packet packet)
//        {
//            foreach (var client in Pool.GamePoll.Values)
//            {
//                if (client.Player.Map == MapID)
//                    client.Send(packet);
//            }
//        }
//    }
//}
