using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgEliteGuildWar
    {
        public static System.SafeDictionary<uint, Winner> ScoreList = new System.SafeDictionary<uint, Winner>();
        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures;
        public Winner RoundOwner;
        public static ushort MapID = 12816;
        public ProcesType Proces;

        // Prevent Start / End from being called multiple times
        // during the same hour.
        private bool StartedThisHour = false;
        private bool EndedThisHour = false;

        public class Winner
        {
            public uint GuildID;
            public uint Score;
            public string GuildName;
        }

        public MsgEliteGuildWar()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        }

        public void Load()
        {
            Furnitures.Add(
                Role.SobNpc.StaticMesh.Pole,
                Pool.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(
                    Role.MapObjectType.SobNpc,
                    6539
                )
            );
        }

        public void CheckUP()
        {
            DateTime now = DateTime.Now;

            // =====================================================
            // Start every hour at XX:35:15
            // =====================================================
            if (!StartedThisHour &&
                Proces == ProcesType.Dead &&
                now.Minute == 35 &&
                now.Second >= 15 &&
                now.Second < 20)
            {
                this.Start();

                StartedThisHour = true;
                EndedThisHour = false;
            }

            // =====================================================
            // Warning at XX:39:00
            // Event will end after 1 minute
            // =====================================================
            if (Proces == ProcesType.Alive &&
                now.Minute == 39 &&
                now.Second < 2)
            {
                MsgSchedules.SendSysMesage(
                    "[Elite Guild War] Will End After 1 Minute",
                    MsgServer.MsgMessage.ChatMode.Center
                );
            }

            // =====================================================
            // End every hour at XX:40:00
            // =====================================================
            if (!EndedThisHour &&
                Proces == ProcesType.Alive &&
                now.Minute == 40 &&
                now.Second < 5)
            {
                this.End();

                EndedThisHour = true;
            }

            // =====================================================
            // Reset flags when a new hour begins
            // =====================================================
            // re-arm any time before the start minute, so a missed tick does not skip the hour
            if (now.Minute < 35 && (StartedThisHour || EndedThisHour))
            {
                StartedThisHour = false;
                EndedThisHour = false;
            }
        }

        public void Start()
        {
            RoundOwner = null;

            Proces = ProcesType.Alive;

            ScoreList = new System.SafeDictionary<uint, Winner>();

            MsgSchedules.SendInvitation(
                "Elite Guild War",
                " ",
                410, // QahrMobile: PC spot 334,298 is water on the mobile Twin City; Twin City square
                354,
                1002,
                0,
                60
            );

            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "EliteGuildWar";
        }

        public void Reset(ServerSockets.Packet stream)
        {
            ScoreList = new System.SafeDictionary<uint, Winner>();

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];

            var users = Pool.GamePoll.Values
                .Where(u =>
                    u != null &&
                    u.Player.Map == MapID &&
                    Role.Core.GetDistance(
                        u.Player.X,
                        u.Player.Y,
                        Pole.X,
                        Pole.Y
                    ) <= Role.SobNpc.SeedDistrance
                )
                .ToArray();

            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd =
                        new MsgServer.MsgUpdate(stream, Pole.UID, 2);

                    stream = upd.Append(
                        stream,
                        MsgServer.MsgUpdate.DataType.Mesh,
                        (long)Pole.Mesh
                    );

                    stream = upd.GetArray(stream);

                    user.Send(stream);

                    stream = upd.Append(
                        stream,
                        MsgServer.MsgUpdate.DataType.Hitpoints,
                        Pole.HitPoints
                    );

                    stream = upd.GetArray(stream);

                    user.Send(stream);

                    user.Send(Pole.GetArray(stream, false));
                }
            }
        }

        public void FinishRound(ServerSockets.Packet stream)
        {
            SortScores(true);

            if (RoundOwner != null)
            {
                Furnitures[Role.SobNpc.StaticMesh.Pole].Name =
                    RoundOwner.GuildName;

                MsgSchedules.SendSysMesage(
                    "The Round is owned by guild " +
                    RoundOwner.GuildName + ".",
                    MsgServer.MsgMessage.ChatMode.Center
                );
            }
            else
            {
                MsgSchedules.SendSysMesage(
                    "The Round has ended with no winner.",
                    MsgServer.MsgMessage.ChatMode.Center
                );
            }

            Reset(stream);
        }

        public void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;

                MsgSchedules.SendSysMesage(
                    RoundOwner == null
                        ? "Elite Guild War has ended and there is no winner"
                        : "Guild " + RoundOwner.GuildName +
                          ", He is the winner of Elite Guild War.",
                    MsgServer.MsgMessage.ChatMode.Center
                );

                MsgSchedules.SendSysMesage(
                    RoundOwner == null
                        ? "Elite Guild War has ended and there is no winner"
                        : "Guild " + RoundOwner.GuildName +
                          ", He is the winner of Elite Guild War.",
                    MsgServer.MsgMessage.ChatMode.BroadcastMessage
                );

                ScoreList.Clear();
            }
        }

        public void UpdateScore(
            ServerSockets.Packet stream,
            uint Score,
            Role.Instance.Guild Guild)
        {
            if (!ScoreList.ContainsKey(Guild.Info.GuildID))
            {
                ScoreList.Add(
                    Guild.Info.GuildID,
                    new Winner()
                    {
                        GuildName = Guild.GuildName,
                        GuildID = Guild.Info.GuildID,
                        Score = Score
                    }
                );
            }
            else
            {
                ScoreList[Guild.Info.GuildID].Score += Score;
            }

            SortScores();

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }

        private void SortScores(bool getwinner = false)
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = ScoreList.Values.ToArray();

                var DescendingList =
                    Array.OrderByDescending(p => p.Score).ToArray();

                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];

                    if (x == 0 && getwinner)
                    {
                        RoundOwner = element;
                    }

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Game.MsgServer.MsgMessage msg =
                            new Game.MsgServer.MsgMessage(
                                "No " +
                                (x + 1).ToString() +
                                ". " +
                                element.GuildName +
                                " (" +
                                element.Score +
                                ")",
                                MsgServer.MsgMessage.MsgColor.yellow,
                                x == 0
                                    ? MsgServer.MsgMessage.ChatMode.FirstRightCorner
                                    : MsgServer.MsgMessage.ChatMode.ContinueRightCorner
                            );

                        SendMapPacket(
                            msg.GetArray(rec.GetStream())
                        );
                    }

                    if (x == 4)
                        break;
                }
            }
        }

        public void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Pool.ServerMaps[MapID].Values)
                client.Send(packet);
        }
    }
}
