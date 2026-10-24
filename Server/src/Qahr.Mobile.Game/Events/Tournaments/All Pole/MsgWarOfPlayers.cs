

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgWarOfPlayers
    {
        public static System.SafeDictionary<uint, Info> WarOfPlayers = new System.SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();

        public static ushort MapID = 11222;

        public class Info
        {
            public uint UID;
            public uint Score;
            public string Name;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Pool.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 99639));
        }

        public static void Start()
        {
            RoundOwner = null;
            WarOfPlayers = new System.SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;
            MsgSchedules.SendInvitation("Dark Flag Base", " ", Places.TwinCity, 0, 60); // QahrMobile: PC spot 360,298 is water on the mobile Twin City; Twin City square
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "DarkFlagBase";

            //MsgSchedules.SendSysMesage("WarOfPlayers has been begun.", MsgServer.MsgMessage.ChatMode.BroadcastMessage);

        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                MsgSchedules.SendSysMesage(RoundOwner == null ? "Dark Flag Base has ended and there is no winner " : " [ " + RoundOwner.Name + " ] , He is the winner of Dark Flag Base", MsgServer.MsgMessage.ChatMode.TopLeft);
                //MsgSchedules.SendSysMesage(RoundOwner == null ? "WarOfPlayers has ended and there is no winner" : "" + RoundOwner.Name + ", He is the winner of WarOfPlayers .", MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                if (RoundOwner != null)
                {
                    //Program.DiscordAPI.Enqueue("``Player " + RoundOwner.Name + " Win WarOfPlayer ``");
                }
                WarOfPlayers.Clear();
            }
        }

        static bool started = false;
        static bool warned = false;
        static bool ended = false;

        public static void CheckUP()
        {
            DateTime now = DateTime.Now;


            if (!started && Proces == ProcesType.Dead &&
                now.Minute == 10 && now.Second >= 10 && now.Second <= 20)
            {
                Start();
                started = true;
            }


            if (Proces == ProcesType.Alive)
            {

                if (!warned &&
                    now.Minute == 14 && now.Second >= 0 && now.Second <= 59)
                {
                    MsgSchedules.SendSysMesage(
                        "[Dark Flag Base] Will End After 1 Minute",
                        MsgServer.MsgMessage.ChatMode.Center);

                    warned = true;
                }


                if (!ended &&
                    now.Minute == 15 && now.Second >= 0 && now.Second <= 10)
                {
                    End();
                    ended = true;
                }
            }

            // re-arm any time before the start minute, so a missed tick does not skip the hour
            if (now.Minute < 10 && (started || warned || ended))
            {
                started = warned = ended = false;
            }
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            WarOfPlayers = new System.SafeDictionary<uint, Info>();

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            var users = Pool.GamePoll.Values.Where(u => u != null && u.Player.Map == MapID && Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 1);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (ulong)Pole.Mesh);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (ulong)Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
                        user.Send(Pole.GetArray(stream, false));
                }
            }
        }
        public static void FinishRound(ServerSockets.Packet stream)
        {
            SortScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.Name;
            MsgSchedules.SendSysMesage("The Round Ownered by [ " + RoundOwner.Name + " ] ", MsgServer.MsgMessage.ChatMode.TopLeft);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Player Player)
        {
            if (!WarOfPlayers.ContainsKey(Player.UID))
                WarOfPlayers.Add(Player.UID, new Info() { Name = Player.Name, UID = Player.UID, Score = Score });
            else
                WarOfPlayers[Player.UID].Score += Score;

            SortScores();
            foreach (var Bas in Furnitures.Values)
            {
                if ((Role.SobNpc.StaticMesh)Bas.Mesh == Role.SobNpc.StaticMesh.Pole)
                {
                    foreach (var user in Pool.GamePoll.Values)
                    {
                        if (user.Player.Map == MapID)
                        {
                            if (Role.Core.GetDistance(user.Player.X, user.Player.Y, Bas.X, Bas.Y) <= Role.SobNpc.SeedDistrance)
                            {
                                var upd = new MsgServer.MsgUpdate(stream, Bas.UID, 2);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Bas.HitPoints);
                                stream = upd.GetArray(stream);
                                user.Send(stream);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.MaxHitpoints, Bas.MaxHitPoints);
                                stream = upd.GetArray(stream);
                                user.Send(stream);
                                user.Send(Bas.GetArray(stream, false));
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }
        private static void SortScores(bool getwinner = false)
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = WarOfPlayers.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && getwinner)
                    {
                        RoundOwner = element;
                    }
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                        SendMapPacket(msg.GetArray(rec.GetStream()));
                    }
                    if (x == 4)
                        break;
                }
            }
        }
        public static void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Pool.ServerMaps[MapID].Values)
                client.Send(packet);
        }
    }
}
