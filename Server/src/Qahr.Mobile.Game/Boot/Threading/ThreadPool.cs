using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Linq;
using System.Threading;
using Qahr.Mobile.Game.MsgTournaments;
// The Class Edit By Mohy ElDin ...
namespace Qahr.Mobile
{
    public class ThreadFunctions
    {
        public const int
            TournamentsStamp = 1000,
            Arena_CreateMatches = 1100,
            Arena_VerifyMatches = 1200,
            Arena_CheckGroups = 1150,
            TeamPkStamp = 1000,
            ElitePkStamp = 1000,
            BroadCastStamp = 1000,
            ResetDayStamp = 6000;

        private static DateTime savestamp = DateTime.Now;
        public static DateTime ArenaStamp = DateTime.Now;
        private static DateTime TitleStamp = DateTime.Now;
        private static DateTime RandomStamp = DateTime.Now;

        public System.ThreadGroup.ThreadItem Thread;
        public DateTime UpdateServerStatus = DateTime.Now;

        public ThreadFunctions(int interval, string name)
        {
            if (interval < 100)
                interval = 100;

            Thread = new System.ThreadGroup.ThreadItem(interval, name, OnProcess);
        }

        public void Start()
        {
            Thread.Open();
        }

        int _last = 0;

        public int Online
        {
            get
            {
                int current = Pool.GamePoll.Values.Count(p => p.Fake == false);

                if (current > _last)
                    _last = current;

                return current;
            }
        }

        public int MaxOnline
        {
            get { return _last; }
        }

        public void OnProcess()
        {
            try
            {
                DateTime DateNow = DateTime.Now;

                if (DateNow > UpdateServerStatus.AddSeconds(1))
                {
                    if (Program.ServerConfig.IsInterServer)
                        MyConsole.Title = "[" + Database.GroupServerList.MyServerInfo.Name + "] Queue Packets: " + ServerSockets.PacketRecycle.Count + " Online " + Pool.GamePoll.Count + " Time: " + DateTime.Now.Hour + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Second + "";
                    else
                        MyConsole.Title = "" + Program.ServerConfig.ServerName + " - Online [ " + Online + " ] - Max Online [ " + MaxOnline + " ]";
                    UpdateServerStatus = DateNow.AddSeconds(5);
                }

                #region Save Database

                if (DateNow > Pool.ServerSave)
                {
                    // [perf] SavingObj كان متمسك طول حفظ كل اللاعبين (+ Sleep(1) لكل لاعب) فـ handler الـ Relic
                    // (MsgXuanBaoOpt) كان بيجمّد ثريد استقبال لثواني. دلوقتي: الحفظ العام تحت القفل مرة واحدة،
                    // وبعدين القفل بيتمسك لكل لاعب لوحده (SaveClient واحد) والـ Sleep برّه القفل.
                    // نفس الضمان: عمر ما SaveClient بتاع الأوتوسيف يشتغل في نفس وقت عملية Relic أو حفظ الإغلاق.
                    lock (Database.ServerDatabase.SavingObj)
                    {
                        Server.SaveDBPayers();
                        global::Qahr.Mobile.Modules.ModuleManager.Save();
                    }

                    foreach (var client in Pool.GamePoll.Values)
                    {
                        bool saved = false;
                        lock (Database.ServerDatabase.SavingObj)
                        {
                            // الإغلاق (Program.ProcessConsoleEvent) بيمسك القفل ويعمل ExitRequested=true وبيحفظ الكل؛
                            // بعد ما يسيبه البروسيس بيقفل، فممنوع نبدأ نكتب ملفات لاعب (ممكن تتقطع في النص).
                            if (Program.ExitRequested)
                                break;
                            try
                            {
                                if (client.OnInterServer ||
                                    client.Socket == null ||
                                    !client.Socket.Alive ||
                                    !client.FullLoading)
                                    continue;

                                Database.ServerDatabase.SaveClient(client);
                                saved = true;
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteLine("SaveClient failed for player: " +
                                    (client != null && client.Player != null ? (client.Player.Name + " (UID " + client.Player.UID + ")") : "<unknown>"));
                                MyConsole.WriteException(e);
                            }
                        }
                        if (saved)
                            System.Threading.Thread.Sleep(1);
                    }

                    Pool.ServerSave = DateNow.AddMinutes(10);
                }

                #endregion

                #region Arena

                if (DateNow > ArenaStamp)
                {
                    ArenaFunctions();
                    ArenaStamp = DateNow.AddSeconds(3);
                }

                #endregion

                #region Broadcast

                Game.MsgTournaments.MsgBroadcast.Work();

                #endregion

                #region Tournaments

                Game.MsgTournaments.MsgSchedules.CheckUp();

                #endregion

                #region Team PK

                foreach (var teamGroup in Game.MsgTournaments.MsgTeamPkTournament.EliteGroups)
                {
                    try
                    {
                        teamGroup.timerCallback();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine("Team PK timerCallback failed.");
                        MyConsole.WriteException(e);
                    }
                }

                #endregion

                #region Skill Team

                foreach (var sTeamGroup in Game.MsgTournaments.MsgSkillTeamPkTournament.EliteGroups)
                {
                    try
                    {
                        sTeamGroup.timerCallback();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine("Skill Team PK timerCallback failed.");
                        MyConsole.WriteException(e);
                    }
                }

                #endregion

                #region Elite PK

                foreach (var elitegroup in Game.MsgTournaments.MsgEliteTournament.EliteGroups)
                {
                    try
                    {
                        elitegroup.timerCallback();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine("Elite PK timerCallback failed.");
                        MyConsole.WriteException(e);
                    }
                }

                #endregion

                #region Random Seed

                if (DateNow > RandomStamp)
                {
                    Pool.GetRandom.SetSeed(Environment.TickCount);
                    RandomStamp = DateNow.AddMinutes(30);
                }

                #endregion

                #region Reset Server

                Server.Reset();

                #endregion

                #region Boss Spawns

                HandleBossSpawns(DateNow);

                #endregion

                System.Threading.Thread.Sleep(10);
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);

                System.Threading.Thread.Sleep(100);
            }
        }

        private static void ArenaFunctions()
        {
            try
            {
                Game.MsgTournaments.MsgSchedules.Arena.CheckGroups();
                Game.MsgTournaments.MsgSchedules.Arena.CreateMatches();
                Game.MsgTournaments.MsgSchedules.Arena.VerifyMatches();
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("ArenaFunctions failed.");
                MyConsole.WriteException(e);
            }
        }

        private static void HandleBossSpawns(DateTime DateNow)
        {
            try
            {
                // Nemesis Tyrant
                if (DateNow.Minute == 15 && DateNow.Second == 15)
                    SpawnBoss(10137, 4220, 568, 372,
                        "Nemesis Tyrant Has Now Appeared In Dragon Island Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Snow Banshee
                if (DateNow.Minute == 27 && DateNow.Second == 15)
                    SpawnBoss(10137, 4171, 658, 718,
                        "Snow Banshee Has Now Appeared In Dragon Island Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Thrilling Spook
                if (DateNow.Minute == 0 && DateNow.Second == 15)
                    SpawnBoss(10137, 4212, 349, 635,
                        "Thrilling Spook Has Now Appeared In Dragon Island Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Nether Tyrant
                if (DateNow.Minute == 45 && DateNow.Second == 15)
                    SpawnBoss(10250, 3978, 872, 1135,
                        "Nether Tyrant Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Bloody Banshee
                if (DateNow.Minute == 57 && DateNow.Second == 15)
                    SpawnBoss(10250, 3976, 484, 176,
                        "Bloody Banshee Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Chilling Spook
                if (DateNow.Minute == 30 && DateNow.Second == 15)
                    SpawnBoss(10250, 3977, 1020, 698,
                        "Chilling Spook Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Dragon Wraith
                if (DateNow.Minute == 5 && DateNow.Second == 15)
                    SpawnBoss(10250, 3971, 163, 415,
                        "Dragon Wraith Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");               

                // Golden Octopus
                if (DateNow.Minute == 50 && DateNow.Second == 30)
                    SpawnBoss(3071, 2700, 141, 176,
                        "Golden Octopus Monster appeared at \n (X:141 Y:176)");


                // Queen Of Evil
                if (DateNow.Hour == 19 && DateNow.Minute == 10 && DateNow.Second == 15)
                    SpawnBoss(10250, 3970, 640, 600,
                        "Queen Of Evil Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");

                // Queen Of Evil
                if (DateNow.Hour == 20 && DateNow.Minute == 10 && DateNow.Second == 15)
                    SpawnBoss(10250, 3970, 640, 600,
                        "Queen Of Evil Has Now Appeared In Deity Land Hurry Up and Kill Him To Receive Many Rare Rewards.");
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("HandleBossSpawns failed.");
                MyConsole.WriteException(e);
            }
        }

        private static void SpawnBoss(
            uint mapid,
            uint monsterid,
            ushort x,
            ushort y,
            string message)
        {
            try
            {
                var map = Pool.ServerMaps[mapid];

                if (map.ContainMobID(monsterid))
                    return;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Server.AddMapMonster(
                        stream,
                        map,
                        monsterid,
                        x,
                        y,
                        18,
                        18,
                        1);

                    Server.SendGlobalPacket(
                        new MsgMessage(
                            message,
                            MsgMessage.MsgColor.red,
                            MsgMessage.ChatMode.TopLeft)
                        .GetArray(stream));

                    foreach (var user in Pool.GamePoll.Values)
                    {
                        try
                        {
                            global::Qahr.Mobile.Phone.MobileEvents.Invite(user,// QahrMobile: was user.Player.MessageBox (mobile: chat line + @join)
                                message,
                                p => { p.Teleport(x, y, mapid); },
                                60);
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteLine("Boss spawn MessageBox failed for player: " +
                                (user != null && user.Player != null ? (user.Player.Name + " (UID " + user.Player.UID + ")") : "<unknown>"));
                            MyConsole.WriteException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("SpawnBoss failed (monsterid " + monsterid + ", mapid " + mapid + ").");
                MyConsole.WriteException(e);
            }
        }
    }
}