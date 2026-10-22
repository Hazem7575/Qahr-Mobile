using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Role;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgGuildWar
    {
        public bool SendInvitation = false;
        internal ushort[][] StatueCoords = 
        {
          new ushort[] {140 ,134 }
         ,new ushort[] {144 ,124 }
         ,new ushort[] {130 ,138 }
         ,new ushort[] {153 ,124 }
         ,new ushort[] {161 ,124 }
         ,new ushort[] {130 ,147 }
         ,new ushort[] {130 ,155 }
        };

        public class GuildConductor
        {
            public static List<uint> BlockMaps = new List<uint>()
            {
                1038
            };
            public Game.MsgNpc.Npc Npc;
            public uint ToMap;
            public ushort ToX, ToY;

            public override string ToString()
            {
                Database.DBActions.WriteLine Line = new Database.DBActions.WriteLine(',');
                Line.Add(Npc.UID).Add(Npc.X).Add(Npc.Y).Add(Npc.Map).Add(Npc.Mesh).Add((ushort)Npc.NpcType)
                    .Add(ToX).Add(ToY).Add(ToMap);
                return Line.Close();
            }
            internal void Load(string Line, MsgNpc.NpcID UID)
            {
                Npc = Game.MsgNpc.Npc.Create();
                if (Line == "")
                {
                    Npc.UID = (uint)UID;
                    return;
                }
                Database.DBActions.ReadLine Reader = new Database.DBActions.ReadLine(Line, ',');
                Npc.UID = Reader.Read((uint)0);
                Npc.X = Reader.Read((ushort)0);
                Npc.Y = Reader.Read((ushort)0);
                Npc.Map = Reader.Read((ushort)0);
                Npc.Mesh = Reader.Read((ushort)0);
                Npc.NpcType = (Role.Flags.NpcType)Reader.Read((ushort)0);

                ToX = Reader.Read((ushort)0);
                ToY = Reader.Read((ushort)0);
                ToMap = Reader.Read((ushort)0);
            }
            internal static bool ChangeNpcLocation(Role.GameMap map, ref ushort X, ref ushort Y, ref uint Map)
            {


                return false;
            }

            internal void GetCoords(out ushort x, out ushort y, out uint map)
            {
                if (ToMap != 0 && ToX != 0 && ToY != 0)
                {
                    x = ToX;
                    y = ToY;
                    map = ToMap;
                    return;
                }
                x = 410;
                y = 354;
                map = 1002;
            }
        }

        public class DataFlameQuest
        {
            public List<uint> Registred;
            public bool ActiveFlame10;

            public DataFlameQuest()
            {
                Registred = new List<uint>();
                ActiveFlame10 = false;
            }
        }
        public class GuildWarScrore
        {

            public uint GuildID;
            public string Name;
            public uint Score;

            //for reward
            public int LeaderReward = 1;
            public int DeputiLeaderReward = 20;
        }

        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        public Role.GameMap GuildWarMap;

        public ProcesType Proces { get; set; }

        public DataFlameQuest FlamesQuest;
        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;
        public GuildWarScrore Winner;
        public Dictionary<MsgNpc.NpcID, GuildConductor> GuildConductors;

        public bool LeftGateOpen { get { return Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.OpenLeftGate; } }
        public bool RightGateOpen { get { return Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.OpenRightGate; } }
        public MsgGuildWar()
        {
            FlamesQuest = new DataFlameQuest();
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            GuildConductors = new Dictionary<MsgNpc.NpcID, GuildConductor>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore() { Name = "None", Score = 100, GuildID = 0 };
        }

        public unsafe void CreateFurnitures()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.LeftGate, Pool.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516074));
            Furnitures.Add(Role.SobNpc.StaticMesh.RightGate, Pool.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516075));
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Pool.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 810));
        }
        public unsafe bool Bomb(ServerSockets.Packet stream, Client.GameClient client, Role.SobNpc.StaticMesh gate)
        {
            if (Furnitures[gate].HitPoints > 3000000)
            {
                Furnitures[gate].HitPoints -= 2000000;

                MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Furnitures[gate].UID, 1);
                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Furnitures[gate].HitPoints);
                Furnitures[gate].SendScrennPacket(upd.GetArray(stream));

                client.Player.Dead(null, client.Player.X, client.Player.Y, client.Player.UID);
                Furnitures[gate].SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, "firemagic");
                Furnitures[gate].SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, "bombarrow");

                Server.SendGlobalPacket(new MsgServer.MsgMessage("" + client.Player.Name + " from " + (client.Player.MyGuild != null ? client.Player.MyGuild.GuildName.ToString() : "None".ToString()).ToString() + " detonated the Bomb and killed herself/himself. But the " + gate.ToString() + " was blown up!"
                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                return true;
            }
            return false;
        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {
            Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;
            Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Pool.GamePoll.Values)
            {
                if (client.Player.Map == 1038)
                {
                    foreach (var npc in Furnitures.Values)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
                        {
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, npc.UID, 2);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                            stream = upd.GetArray(stream);
                            client.Send(stream);
                            if ((Role.SobNpc.StaticMesh)npc.Mesh == Role.SobNpc.StaticMesh.Pole)
                                client.Send(npc.GetArray(stream, false));
                        }
                    }
                }
            }
        }
        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Pool.GamePoll.Values)
            {
                if (client.Player.Map == 1038 || client.Player.Map == 6001)
                {
                    client.Send(packet);
                }
            }
        }
        internal unsafe void CompleteEndGuildWar()
        {
            SendInvitation = false;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
#if Arabic
              Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations, to " + Winner.Name + " they`ve won the Guild War With a Score of " + Winner.Score.ToString() + "!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
#else

#if Encore
    Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations, to " + Winner.Name + " they`ve won the Guild War With a Score of " + Winner.Score.ToString() + "!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#else
                //   Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the guildwar with a score of " + Winner.Score.ToString() + ""
                // , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                // Server.SendGlobalPacket(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", He won the Guild War with a score of " + Winner.Score.ToString() + ""
                // , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));

                Server.SendGlobalPacket(new MsgServer.MsgMessage(" Congratulations, to [ " + Winner.Name + " ] they`ve won the Guild War With a Score of " + Winner.Score.ToString() + " !", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
#endif
#endif


            }

            RewardDeputiLeader.Clear();
            RewardLeader.Clear();
            Winner.DeputiLeaderReward = 20;
            Winner.LeaderReward = 1;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Role.Instance.Union.CheckGuildWar(stream);
            }
        }

        internal unsafe void Start()
        {
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "None";

            FlamesQuest = new DataFlameQuest();
            Proces = ProcesType.Alive;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ResetFurnitures(stream);
                ScoreList.Clear();
#if Arabic
                    Server.SendGlobalPacket(new MsgServer.MsgMessage("The Guild War has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            
#else
                Server.SendGlobalPacket(new MsgServer.MsgMessage("The Guild War has started !", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Server.SendGlobalPacket(new MsgServer.MsgMessage(" The Guild War has started !", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#endif
            }
            // client invite box (10515), same call as the GM command
            MsgSchedules.SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
        }

        internal unsafe void FinishRound()
        {


            ShuffleGuildScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            // Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
#if Arabic
               Server.SendGlobalPacket(new MsgServer.MsgMessage("" + Winner.Name + " wins!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
#else
                Server.SendGlobalPacket(new MsgServer.MsgMessage("" + Winner.Name + " wins!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
#endif


                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3);
        }
        internal unsafe void Began()
        {
            if (Proces == ProcesType.Idle)
            {
                Proces = ProcesType.Alive;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
#if Arabic
                         Server.SendGlobalPacket(new MsgServer.MsgMessage("Guild war has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
               
#else
                    Server.SendGlobalPacket(new MsgServer.MsgMessage("Guild war has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

#endif
                }
            }
        }
        internal void UpdateScore(Role.Player client, uint Damage)
        {
            if (client.MyGuild == null)
                return;

            if (Proces != ProcesType.Alive)
                return;

            if (!ScoreList.ContainsKey(client.GuildID))
            {
                ScoreList.TryAdd(client.GuildID,
                    new GuildWarScrore()
                    {
                        GuildID = client.MyGuild.Info.GuildID,
                        Name = client.MyGuild.GuildName,
                        Score = Damage
                    });
            }
            else
            {
                ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
            }
            if (DateTime.Now > StampShuffleScore)
            {
                ShuffleGuildScores();
                StampShuffleScore = DateTime.Now.AddSeconds(2);
            }

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                FinishRound();
        }

        internal unsafe void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Dead)
            {
                StampShuffleScore = DateTime.Now.AddSeconds(8);
                var Array = ScoreList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && createWinned)
                        Winner = element;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
#if Arabic
                         Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score.ToString() + ")"
                            , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
#else
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score.ToString() + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
#endif

                        SendMapPacket(msg.GetArray(stream));

                    }
                    if (x == 4)
                        break;
                }
            }
        }
        internal bool ValidJump(int Current, out int New, ushort X, ushort Y)
        {
            if (Role.Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = Current;
            int new_FloorType = GuildWarMap.FloorType[X, Y];
            if (Current == 3)
            {
                if (new_FloorType == 0 || new_FloorType == 9 || new_FloorType == 13)
                {
                    if (Role.Core.GetDistance(X, Y, 164, 211) <= 10)
                    {
                        if (LeftGateOpen)
                        {
                            New = new_FloorType;
                            return true;
                        }
                    }
                    if (Role.Core.GetDistance(X, Y, 164, 211) <= 6)
                    {

                    }
                    if (Role.Core.GetDistance(X, Y, 223, 183) <= 10)
                    {
                        if (RightGateOpen)
                        {
                            New = new_FloorType;
                            return true;
                        }
                    }
                    return false;
                }

            }
            New = new_FloorType;
            return true;
        }
        internal bool ValidWalk(int Current, out int New, ushort X, ushort Y)
        {
            if (Role.Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = Current;
            int new_mask = GuildWarMap.FloorType[X, Y];
            if (Current == 3)
            {
                if (new_mask == 0 || new_mask == 9 || new_mask == 13)
                {
                    if (Y == 209 || Y == 208)
                    {
                        if (Role.Core.GetDistance(X, Y, 164, 209) <= 3)
                        {
                            if (LeftGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }
                    else if (X == 216)
                    {
                        if (Role.Core.GetDistance(X, Y, 216, 177) <= 4)
                        {
                            if (RightGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }
            New = new_mask;
            return true;
        }
        internal void Save()
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\GuildWarInfo.ini");
            if (Proces == ProcesType.Dead)
            {
                write.Write<uint>("Info", "ID", Winner.GuildID);
                write.WriteString("Info", "Name", Winner.Name);
                write.Write<int>("Info", "LeaderReward", Winner.LeaderReward);
                write.Write<int>("Info", "DeputiLeaderReward", Winner.DeputiLeaderReward);

                // snapshot بدون الأصفار، عشان اللي استلم بعد التحميل يتحفظ في خانات يقراها Load (0..19)
                // والمفاتيح القديمة الزايدة تتصفّر بدل ما تفضل كأنها "استلمت"
                uint[] leaders = RewardLeader.ToArray().Where(p => p != 0).Distinct().ToArray();
                write.Write<uint>("Info", "LeaderTop0", leaders.Length > 0 ? leaders[0] : 0);
                for (int x = 1; x < leaders.Length; x++)
                    write.Write<uint>("Info", "LeaderTop" + x.ToString() + "", leaders[x]);
                uint[] deputies = RewardDeputiLeader.ToArray().Where(p => p != 0).Distinct().ToArray();
                for (int x = 0; x < 20; x++)
                    write.Write<uint>("Info", "DeputiTop" + x.ToString() + "", x < deputies.Length ? deputies[x] : 0);
                write.WriteString("Pole", "Name", Winner.Name);
                write.Write<int>("Pole", "HitPoints", Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints);
            }

            // write.WriteString("Condutors", "GuildConductor2", GuildConductors[MsgNpc.NpcID.TeleGuild2].ToString());
            // write.WriteString("Condutors", "GuildConductor3", GuildConductors[MsgNpc.NpcID.TeleGuild3].ToString());
            //  write.WriteString("Condutors", "GuildConductor4", GuildConductors[MsgNpc.NpcID.TeleGuild4].ToString());
        }
        internal void Load()
        {
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\GuildWarInfo.ini");
            Winner.GuildID = reader.ReadUInt32("Info", "ID", 0);
            Winner.Name = reader.ReadString("Info", "Name", "None");
            Winner.LeaderReward = reader.ReadInt32("Info", "LeaderReward", 0);
            Winner.DeputiLeaderReward = reader.ReadInt32("Info", "DeputiLeaderReward", 0);

            RewardLeader.Add(reader.ReadUInt32("Info", "LeaderTop0", 0));
            for (int x = 0; x < 20; x++)
            {
                RewardDeputiLeader.Add(reader.ReadUInt32("Info", "DeputiTop" + x.ToString() + "", 0));
            }

            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = reader.ReadString("Pole", "Name", "None");
            Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints = reader.ReadInt32("Pole", "HitPoints", 0);

            for (int x = 0; x < 4; x++)
            {
                GuildConductor conductor = new GuildConductor();
                conductor.Load(reader.ReadString("Condutors", "GuildConductor" + (x + 1).ToString() + "", ""), (MsgNpc.NpcID)(101614 + x * 2));
                GuildConductors.Add((MsgNpc.NpcID)(101614 + x * 2), conductor);
                if (conductor.Npc.Map != 0)
                {
                    if (Pool.ServerMaps.ContainsKey(conductor.Npc.Map))
                        Pool.ServerMaps[conductor.Npc.Map].AddNpc(conductor.Npc);
                }
            }
            GuildWarMap = Pool.ServerMaps[1038];

        }

    }
}
