using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.MsgInterServer.Packets;

namespace Qahr.Mobile.MsgInterServer
{
    public class StaticConnexion
    {
        public class Entity
        {
            public ServerSockets.SecuritySocket SecuritySocket;

            public Entity(ServerSockets.SecuritySocket _socket)
            {
                _socket.Client = this;
                SecuritySocket = _socket;
            }
            public void Send(ServerSockets.Packet msg)
            {
                SecuritySocket.Send(msg);
            }
            public void Disconnect(ServerSockets.SecuritySocket obj)
            {
                SecuritySocket.Disconnect();
            }

        }
        public static ServerSockets.ServerSocket ServerConnecxion;
        public static System.ThreadGroup.ThreadItem Thread = null;
        public static ServerSockets.SecuritySocket ClientConnecxion;
        public static DateTime SendInfo = new DateTime();



        public static void Create()
        {
            if (Program.ServerConfig.IsInterServer == false)
            {
                //ServerConnecxion = new ServerSockets.ServerSocket(ProcessConnect, (p, data) => { ProcesReceive(p, data); }, (p, data) => { (p.Client as Entity).Disconnect(p); });
                //Connect();
                //if (Thread == null)
                //{
                //    Thread = new System.ThreadGroup.ThreadItem(10000, "InterServer", CheckConnection);
                //    Thread.Open();
                //}
            }
        }
        public static void Send(ServerSockets.Packet stream)
        {
            if (ClientConnecxion == null)
                return;
            if (ClientConnecxion.Alive == false)
                return;
            ClientConnecxion.Send(stream);
        }
        public static void Connect()
        {
            if (Program.ServerConfig.IsInterServer == false)
            {
                //ServerConnecxion.Connect(Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port, "InterServer");
            }
        }
        public static void ProcessConnect(ServerSockets.SecuritySocket Socket)
        {
            if (Program.ServerConfig.IsInterServer == false)
            {
                var obj = new Entity(Socket);
                Socket.OnInterServer = true;
                ClientConnecxion = Socket;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    var DBServer = Database.GroupServerList.MyServerInfo;
                    System.Threading.Thread.Sleep(1000);
                    obj.Send(stream.ServerInfoCreate(1, DBServer.ID, DBServer.Name, DBServer.MapID, DBServer.X, DBServer.Y, DBServer.Group));
                }
                Socket.ConnectFull = true;
            }
        }
        public static void ProcesReceive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            var Game = (obj.Client as Entity);
            ushort PacketID = stream.ReadUInt16();
            try
            {
                switch (PacketID)
                {
                    case PacketTypes.InterServer_EliteRank:
                        {
                            MsgElitePkRanking.RankType rank;
                            uint Group;
                            MsgElitePKBrackets.GuiTyp GroupStatus;
                            uint Count;
                            uint UID;

                            List<Game.MsgTournaments.MsgEliteGroup.FighterStats> players = new List<Game.MsgTournaments.MsgEliteGroup.FighterStats>();
                            stream.GetElitePkRanking(out rank, out Group, out GroupStatus, out Count, out UID);
                            for (int x = 0; x < Count; x++)
                            {
                                Game.MsgTournaments.MsgEliteGroup.FighterStats status;
                                stream.GetItemElitePkRanking(out status);
                                players.Add(status);
                            }
                            Instance.CrossElitePKTournament.AddRanks(Group, players);
                            break;
                        }
                    case Qahr.Mobile.Game.GamePackets.MsgTalk:
                        {

                            var mes = new MsgMessage();
                            mes.Deserialize(stream);

                            if (mes.ChatType == MsgMessage.ChatMode.BroadcastMessage)
                            {
                                if (mes.__Message == "[Cross Elite PK Tournament] begins at 20:00. Get yourself prepared for it!")
                                {
                                    Core.IsCrossPkOpen = true;
                                    Core.JoinCrossEliteStamp = DateTime.Now.AddMinutes(60 * 5);
                                    Qahr.Mobile.Game.MsgTournaments.MsgSchedules.SendInvitation("Cross Elite PK Tournament", "Special Accessory/Boots, CPs, etc", Places.TwinCity, 0, 60);
                                    break;
                                }
                            }
                            stream.Seek(stream.Size);
                            foreach (var user in Pool.GamePoll.Values)
                            {
                                user.Send(stream);
                            }

                            break;
                        }
                    case PacketTypes.InterServer_UnionRanks:
                        {
                            Game.MsgServer.MsgLeagueRank.ActionID type; ushort count;
                            ushort Page; byte dwparam; ushort PageCount;
                            stream.GetLeagueRank(out type, out count, out Page, out dwparam, out PageCount);
                            if (count == 1)
                            {
                                uint ServerID; uint GoldBricks; string Name; string LeaderName;
                                stream.GetItemLeagueRank(out ServerID, out GoldBricks, out Name, out LeaderName);
                                Instance.Union.AddUnion(ServerID, GoldBricks, Name, LeaderName);

                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(stream);
            }
        }
        public static void CheckConnection()
        {
            if (ClientConnecxion == null)
                return;

            if (!ClientConnecxion.Alive || ClientConnecxion.Connection.Connected == false)
            {
                ClientConnecxion = null;
                Create();
                return;
            }
            if (DateTime.Now >= SendInfo.AddSeconds(10))
            {


                //send union info.

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    var array = Role.Instance.Union.UnionPoll.Values.Where(p => p.IsKingdom == 1 && p.GoldBrick > 0).ToArray();

                    foreach (var obj in array)
                    {

                        stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)1, (ushort)0, (byte)0, (ushort)0);
                        stream.AddItemLeagueRank(Database.GroupServerList.MyServerInfo.ID, obj.GoldBrick, obj.Name, obj.Emperor);
                        ClientConnecxion.Send(stream.InterServerLeagueRankFinalize());
                    }
                }

                SendInfo = DateTime.Now;
            }


        }

    }
}
