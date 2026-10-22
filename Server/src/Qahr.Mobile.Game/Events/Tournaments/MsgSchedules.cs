using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgSchedules
    {
        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(ThreadFunctions.TournamentsStamp);
        public static Dictionary<TournamentType, ITournament> Tournaments = new Dictionary<TournamentType, ITournament>();
        private static bool InfinityWarCheck = false;
        private static bool MortalKnightsWarCheck = false;
        private static int LastCpCastleTeleportHour = -1;
        private static int LastLuckyBoxesTeleportHour = -1;
        private static int LastFlagsWarTeleportHour = -1;
        private static int LastSquidwardOctopusTeleportHour = -1;

        public static ITournament CurrentTournament;
        internal static MsgGuildWar GuildWar;
        internal static MsgSuperGuildWar SuperGuildWar;
        internal static MsgArena Arena;
        internal static MsgClassPKWar ClassPkWar;
        internal static MsgEliteTournament ElitePkTournament;
        internal static MsgTeamPkTournament TeamPkTournament;
        internal static MsgSkillTeamPkTournament SkillTeamPkTournament;
        internal static MsgCaptureTheFlag CaptureTheFlag;
        internal static MsgTowerOfMystery TowerOfMystery;
        internal static MsgSteedRace SteedRace;
        internal static MsgDisCity DisCity;
        internal static MsgFlagsWar FlagsWar;
        internal static MsgDarkCrystalEvent DarkCrystalEvent;
        internal static MsgCpsCastle CpsCastle;
        internal static MsgTreasureThief TreasureThief;
        internal static MsgGoldBoxWar GoldBoxWar;
        internal static MsgWarOfPlayers WarOfPlayers;
        internal static MsgEliteGuildWar EliteGuildWar;
        //internal static MsgTopWarScore TopWarScore;
        //internal static MsgEmperorWar EmperorWar;
        //internal static MsgUnionWar UnionWar;
        //internal static MsgNewKingsWar KingsWar;
        //internal static MsgClanPhoenix ClanPhoenix;
        internal static MsgMonthlyPK MonthlyPK;
        internal static MsgWeeklyPK WeeklyPK;
        internal static MsgSquidwardOctopuss SquidwardOctopus;
        internal static void Create()
        {
            Tournaments.Add(TournamentType.None, new MsgNone(TournamentType.None));
            GuildWar = new MsgGuildWar();
            CurrentTournament = Tournaments[TournamentType.None];
            SquidwardOctopus = new MsgSquidwardOctopuss();
            CpsCastle = new MsgCpsCastle();
            SuperGuildWar = new MsgSuperGuildWar();
            Arena = new MsgArena();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            ElitePkTournament = new MsgEliteTournament();
            CaptureTheFlag = new MsgCaptureTheFlag();
            SteedRace = new MsgSteedRace();
            WeeklyPK = new MsgWeeklyPK();
            MonthlyPK = new MsgMonthlyPK();
            DisCity = new MsgDisCity();
            TowerOfMystery = new MsgTowerOfMystery();
            EliteGuildWar = new MsgEliteGuildWar();

            TreasureThief = new MsgTreasureThief(TournamentType.TreasureThief);

            Tournaments.Add(
                TournamentType.TreasureThief,
                TreasureThief
            );
           
            GoldBoxWar = new MsgGoldBoxWar();
            FlagsWar = new MsgFlagsWar();
            DarkCrystalEvent = new MsgDarkCrystalEvent();
            //TopWarScore = new MsgTopWarScore();
            //UnionWar = new MsgUnionWar();
            //EmperorWar = new MsgEmperorWar();
            WarOfPlayers = new MsgWarOfPlayers();
            //KingsWar = new MsgNewKingsWar();
            TeamPkTournament = new MsgTeamPkTournament();
            SkillTeamPkTournament = new MsgSkillTeamPkTournament();
            //ClanPhoenix = new MsgClanPhoenix();
        }
        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Secounds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            string Message = "" + Name + (" Just Started Do You Want To Join Now !");

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var packet = new Game.MsgServer.MsgMessage(Message, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                {

                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        continue;

                    if (client.Player.Map == 1860 || client.Player.Map == 1858 || client.Player.Map == 1038)
                        continue;

                    if (client.Player.Map == 6004)
                        continue;

                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                    {
                        client.SendSysMesage("You are dead.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        continue;
                    }

                    if (client.IsVendor)
                        continue;

                    if (client.InTrade)
                        continue;

                    client.Send(packet);
                    global::Qahr.Mobile.Phone.MobileEvents.Invite(client,// QahrMobile: was client.Player.MessageBox (mobile: chat line + @join)
    Message,
    new Action<Client.GameClient>(user =>
    {
        if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        {
            client.SendSysMesage("You are dead.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
            return;
        }
        
        if (client.IsVendor)
            return;

        if (client.InTrade)
            return;

        user.Teleport(X, Y, map, DinamicID);
    }),
    Secounds,
    messaj);
                }
            }
        }
        internal unsafe static void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
                {
                    var client = kv.Value;
                    if (client.Fake != false)
                        continue;
                    var packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                    client.Send(packet);

                }
            }
        }
        internal static void CheckUp()
        {
            DateTime Now64 = DateTime.Now;
            {
                #region Arena
                if (Arena.Proces == ProcesType.Dead)
                {
                    Arena.Proces = ProcesType.Alive;
                }
                #endregion
                try
                {
                    #region Check Ups
                    CpsCastle.CheckUp();
                    FlagsWar.CheckUp();
                    DisCity.CheckUp();
                    //DarkCrystalEvent.CheckUp();
                    SquidwardOctopus.CheckUp();
                    MsgWarOfPlayers.CheckUP();
                    GoldBoxWar.CheckUp();
                    //TreasureThief.CheckUp();
                    //EmperorWar.CheckUP();
                    //MsgNewKingsWar.CheckUP();
                    //ClanPhoenix.CheckUP();
                    EliteGuildWar.CheckUP();
                    //TopWarScore.CheckUP();
                    CurrentTournament.CheckUp();
                    if (CaptureTheFlag.Proces == ProcesType.Alive)
                    {
                        CaptureTheFlag.UpdateMapScore();
                        CaptureTheFlag.CheckUpX2();
                        CaptureTheFlag.SpawnFlags();
                    }
                    #endregion   

                    #region CP Castle Out In Twin City

                    if (Now64.Minute == 5 && LastCpCastleTeleportHour != Now64.Hour)
                    {
                        LastCpCastleTeleportHour = Now64.Hour;

                        MsgSchedules.SendSysMesage(
                            "Shadow Castle Event has Ended.",
                            MsgServer.MsgMessage.ChatMode.Center,
                            MsgServer.MsgMessage.MsgColor.red);

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            if (user.Map.ID == 10133)
                            {
                                user.Teleport(410, 354, 1002);
                            }
                        }
                    }

                    #endregion


                    #region Lucky Golden Boxes Out In Twin City

                    if (Now64.Minute == 25 && LastLuckyBoxesTeleportHour != Now64.Hour)
                    {
                        LastLuckyBoxesTeleportHour = Now64.Hour;

                        MsgSchedules.SendSysMesage(
                            "Lucky Golden Boxes has Ended.",
                            MsgServer.MsgMessage.ChatMode.Center,
                            MsgServer.MsgMessage.MsgColor.red);

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            if (user.Map.ID == 10741)
                            {
                                user.Teleport(410, 354, 1002);
                            }
                        }
                    }

                    #endregion


                    #region Flags War Out In Twin City

                    if (Now64.Minute == 35 && LastFlagsWarTeleportHour != Now64.Hour)
                    {
                        LastFlagsWarTeleportHour = Now64.Hour;

                        MsgSchedules.SendSysMesage(
                            "Flags War has Finished.",
                            MsgServer.MsgMessage.ChatMode.Center,
                            MsgServer.MsgMessage.MsgColor.red);

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            if (user.Map.ID == 2071)
                            {
                                user.Teleport(410, 354, 1002);
                            }
                        }
                    }

                    #endregion



                    #region Squidward Octopus Out In Twin City

                    if (Now64.Minute == 55 && LastSquidwardOctopusTeleportHour != Now64.Hour)
                    {
                        LastSquidwardOctopusTeleportHour = Now64.Hour;

                        MsgSchedules.SendSysMesage(
                            "Squidward Octopus Event has Ended.",
                            MsgServer.MsgMessage.ChatMode.Center,
                            MsgServer.MsgMessage.MsgColor.red);

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            if (user.Map.ID == 3071)
                            {
                                user.Teleport(410, 354, 1002);
                            }
                        }
                    }

                    #endregion







                    #region CP Castle

                    if (Now64.Minute == 00 && CpsCastle.Process == ProcesType.Dead)
                    {
                        CpsCastle.Start();
                    }

                    #endregion 

                    #region Flags War

                    if (Now64.Minute == 30 && FlagsWar.Process == ProcesType.Dead)
                    {
                        FlagsWar.Start();
                    }

                    #endregion 


                    #region Gold Box War

                    if (Now64.Minute == 20 && GoldBoxWar.Process == ProcesType.Dead)
                    {
                        GoldBoxWar.Start();
                    }

                    #endregion


                    #region Infinity War

                    if (Now64.Minute == 06 && Now64.Second == 4 && !InfinityWarCheck)
                    {
                        InfinityWarCheck = true;

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            global::Qahr.Mobile.Phone.MobileEvents.Invite(user,// QahrMobile: was user.Player.MessageBox
                                "Infinity War Just Started Do You Want To Join Now ! ",
                                new Action<Client.GameClient>(p =>
                                {
                                    p.Teleport(364, 298, 1002);
                                }),
                                60);                            
                        }
                    }

                    if (Now64.Minute != 06)
                    {
                        InfinityWarCheck = false;
                    }

                    #endregion                             



                    //#region Titan War

                    //if (Now64.Minute == 36 && Now64.Second == 4 && !TitanWarCheck)
                    //{
                    //    TitanWarCheck = true;

                    //    foreach (var user in Pool.GamePoll.Values)
                    //    {
                    //        user.Player.MessageBox(
                    //            "Titan War Just Started Do You Want To Join Now ! ",
                    //            new Action<Client.GameClient>(p =>
                    //            {
                    //                p.Teleport(334, 298, 1002);
                    //            }),
                    //            null,
                    //            60);                            
                    //    }
                    //}

                    //if (Now64.Minute != 36)
                    //{
                    //    TitanWarCheck = false;
                    //}

                    //#endregion




                    #region Mortal Knights War

                    if (Now64.Minute == 41 && Now64.Second == 4 && !MortalKnightsWarCheck)
                    {
                        MortalKnightsWarCheck = true;

                        foreach (var user in Pool.GamePoll.Values)
                        {
                            global::Qahr.Mobile.Phone.MobileEvents.Invite(user,// QahrMobile: was user.Player.MessageBox
                                "Mortal Knights War Just Started Do You Want To Join Now ! ",
                                new Action<Client.GameClient>(p =>
                                {
                                    p.Teleport(328, 298, 1002);
                                }),
                                60);                            
                        }
                    }

                    if (Now64.Minute != 41)
                    {
                        MortalKnightsWarCheck = false;
                    }

                    #endregion


                    #region SquidwardOctopus

                    if (Now64.Minute == 50 && SquidwardOctopus.Process == ProcesType.Dead)
                    {
                        SquidwardOctopus.Start();
                    }

                    #endregion                    


                    #region Team PK Tournament

                    if (Now64.Hour == 18 && Now64.Minute == 55)
                    {
                        TeamPkTournament.Start();
                    }

                    #endregion

                    #region Skill Team PK Tournament

                    if (Now64.Hour == 19 && Now64.Minute == 55)
                    {
                        SkillTeamPkTournament.Start();
                    }

                    #endregion

                    #region Elite PK Tournament

                    if (Now64.Hour != 19 && Now64.Hour != 20 && Now64.Hour != 21 && Now64.Hour != 22)
                    {
                        if (ElitePkTournament.Proces != ProcesType.Dead)
                        {
                            ElitePkTournament.Proces = ProcesType.Dead;
                        }
                    }
                    if (Now64.Hour == 20 && Now64.Minute == 55)
                    {
                        ElitePkTournament.Start();
                    }

                    #endregion

                    #region Maintenance
                    if (Now64.Hour == 23 && Now64.Minute == 55 && Now64.Second == 4) // Time Restart Server
                    {

                        new System.Threading.Thread(new System.Threading.ThreadStart(Program.Maintenance)).Start();
                    }

                    #endregion

                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }

                switch (Now64.DayOfWeek)
                {
                    //case DayOfWeek.Tuesday:
                    //    {
                    //        #region Score Guild War
                    //        if (Now64.Hour == 21 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.TopWarScore.Start();
                    //        }
                    //        if (Now64.Hour == 22 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.TopWarScore.End();
                    //        }
                    //        #endregion
                    //        break;
                    //    }
                    //case DayOfWeek.Sunday:
                    //    {
                    //        #region Team PK Tournament
                    //        if (Now64.Hour == 18 && Now64.Minute == 55) // 6.55 PM
                    //        {
                    //            TeamPkTournament.Start();
                    //        }
                    //        #endregion     

                    //        #region Elite Guild War
                    //        if (Now64.Hour == 21 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.EliteGuildWar.Start();
                    //        }
                    //        if (Now64.Hour == 22 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.EliteGuildWar.End();
                    //        }
                    //        #endregion
                    //        break;
                    //    }
                    //case DayOfWeek.Thursday:
                    //    {
                    //        #region Clan War
                    //        if (Now64.Hour == 21 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.ClanPhoenix.Start();
                    //        }
                    //        if (Now64.Hour == 22 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.ClanPhoenix.End();
                    //        }
                    //        #endregion
                    //        break;
                    //    }
                    case DayOfWeek.Saturday:
                        {
                            #region Capture The Flag
                            if (Now64.Hour == 21 && Now64.Minute == 00)
                            {
                                CaptureTheFlag.Start();
                            }
                            if (Now64.Hour == 22 && Now64.Minute == 00)
                            {
                                CaptureTheFlag.CheckFinish();
                            }
                            #endregion
                            break;
                        }
                    case DayOfWeek.Friday:
                        {
                            #region GuildWar
                            if (Now64.Hour == 21) // 21.00 = 9 PM > معاد بداية الجيلد وار <
                            {
                                if (GuildWar.Proces == ProcesType.Dead)
                                    GuildWar.Start();
                                if (GuildWar.Proces == ProcesType.Idle)
                                {
                                    if (Now64 > GuildWar.StampRound)
                                        GuildWar.Began();
                                }
                                if (GuildWar.Proces != ProcesType.Dead)
                                {
                                    if (DateTime.Now > GuildWar.StampShuffleScore)
                                    {
                                        GuildWar.ShuffleGuildScores();
                                    }
                                }

                            }
                            else if (Now64.Hour == 22) // 22.00 = 10 PM > معاد نهاية الجيلد وار <
                            {
                                if (GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle)
                                    GuildWar.CompleteEndGuildWar();
                            }
                            #endregion

                            if (Now64.Hour != 19 && Now64.Hour != 20 && Now64.Hour != 21)
                            {
                                if (ElitePkTournament.Proces != ProcesType.Dead)
                                {
                                    ElitePkTournament.Proces = ProcesType.Dead;
                                }
                            }                           
                            break;
                        }
                    //case DayOfWeek.Monday:
                    //    {
                    //        #region ClassPkWar
                    //        if (Now64.Hour == 19 && Now64.Minute == 00)
                    //        {
                    //            ClassPkWar.Start();
                    //        }
                    //        #endregion

                    //        #region Union War
                    //        if (Now64.Hour == 21 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.UnionWar.Start();
                    //        }
                    //        if (Now64.Hour == 22 && Now64.Minute == 00)
                    //        {
                    //            Game.MsgTournaments.MsgSchedules.UnionWar.FinishRound();
                    //        }
                    //        #endregion
                    //        break;
                    //    }
                    //case DayOfWeek.Wednesday:
                    //    {
                    //        #region Skill Team Tournament
                    //        if (Now64.Hour == 19 && Now64.Minute == 55) // 7.55 PM
                    //        {
                    //            SkillTeamPkTournament.Start();
                    //        }
                    //        #endregion
                    //        if (Now64.Hour != 21 && Now64.Hour != 22)
                    //        {
                    //            if (Now64.Minute % 21 == 0 && Now64.Second < 2)
                    //            {
                    //                if (CurrentTournament.Process == ProcesType.Dead)
                    //                {
                    //                    var type = (TournamentType)Pool.GetRandom.Next(1, (byte)TournamentType.Count);
                    //                    while (!Tournaments.ContainsKey(type))
                    //                        type = (TournamentType)Pool.GetRandom.Next(1, (byte)TournamentType.Count);
                    //                    CurrentTournament = Tournaments[type];
                    //                    CurrentTournament.Open();
                    //                }
                    //            }
                    //        }

                    //        if (Now64.Hour < 22)
                    //        {

                    //            if (SuperGuildWar.Proces == ProcesType.Dead)
                    //                SuperGuildWar.Start();
                    //            if (SuperGuildWar.Proces == ProcesType.Idle)
                    //            {
                    //                if (Now64 > SuperGuildWar.StampRound)
                    //                    SuperGuildWar.Began();
                    //            }
                    //            if (SuperGuildWar.Proces != ProcesType.Dead)
                    //            {
                    //                if (DateTime.Now > SuperGuildWar.StampShuffleScore)
                    //                {
                    //                    SuperGuildWar.ShuffleGuildScores();
                    //                }
                    //                SuperGuildWar.CheckFlags();
                    //            }
                    //        }
                    //        if (Now64.Hour == 22)
                    //        {
                    //            if (SuperGuildWar.Proces == ProcesType.Alive)
                    //                SuperGuildWar.CompleteEndGuildWar();
                    //        }
                    //        break;
                    //    }
                }
            }
        }
    }
}