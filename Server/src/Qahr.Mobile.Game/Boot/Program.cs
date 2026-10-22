using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Qahr.Mobile.DBFunctionality;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Cryptography;
using Qahr.Mobile.Shared.Cryptography;
using System.Net.Http;
using Qahr.Mobile.Database;
using System.Windows.Forms;
using Qahr.Mobile.ServerSockets;
using Qahr.Mobile.Game;
using Qahr.Mobile.MsgInterServer;
using System.Net;
using System.Collections.Specialized;
using Qahr.Mobile;
using Qahr.Mobile.Game.MsgTournaments;

namespace Qahr.Mobile
{
    class Program
    {
        public static ServerSockets.Thread4K CallBack;
        public static byte svrmaintance = 0;
        public static List<uint> norevivehere = new List<uint>() { 1038, 4447, 4448, 4449, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 3868, 6525, 6729, 6521, 6241, 3842, 6546, 6784, 6666, 3872, 2057, }; // حط ايدي الماب الى انت عايز تخليه ميصحاش فيها هنا
        public static List<uint> IDFixBug = new List<uint>() { 3071, 3030, 10741, 3071, 9955, 4447, 4448, 1518, 4449, 1508, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 10133, 9855, 3954, 9931, 4057, 5053, 5054, 5055, 5056, 5057, 5058, 1730, 22348 };
        public static readonly List<uint> NoDropItems = new List<uint>() { 3071, 2071, 31254, 31255, 31256, 12288, 9991, 9992, 9993, 9990, 66666, 33333, 22222, 88888, 1036, 10364, 12288, 9991, 9992, 9993, 12602, 12275, 12546, 13249, 12602, 13140, 12546, 3030, 12800, 11331, 12799, 11996, 12665, 12601, 12816, 12099, 12667, 12605, 12602, 2071, 3030, 11, 7, 5, 15, 14, 12, 8, 20, 1730, 10134, 12099, 10133, 22348, 12344, 10070, 10504, 12344, 6521, 95152, 1770, 10137, 10250, 9855, 3071, 6546, 11222, 9955, 3852, 15757, 15758, 15759, 15760, 15761, 15762, 15763, 15764, 15765, 15766, 10133, 10445, 10446, 10447, 10448, 10449, 10450, 10451, 10452, 10453, 10454, 6333, 1511, 1770, 4000, 1508, 8832, 8833, 8834, 8835, 8836, 8837, 8838, 8839, 3954, 9931, 1005, 4000, 11041, 11040, 11516, 10764, 11547, 11515, 11127, 10864, 10027, 1038, 1700, 3954, 3820, };
        public static List<uint> NoHP = new List<uint>() { 5051, 5053, 5054, 5055, 5056, 5057, 5058 };
        public static readonly List<uint> MapCounterHits = new List<uint>() { 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 1005, 6000 };
        public static List<uint> FreePkMap = new List<uint>() { 31254, 31255, 31256, 31257, 2071, 10133, 11222, 12288, 9991, 9992, 9993, 9990, 3030, 10741, 66666, 33333, 22222, 88888, 10133, 3071, 1036, 10364, 12288, 9991, 9992, 9993, 12602, 12275, 12546, 13249, 12602, 13140, 12546, 12800, 11331, 12799, 11996, 12665, 12601, 12816, 12099, 12667, 12605, 12602, 10741, 2071, 3030, 10133, 11, 7, 5, 15, 14, 12, 8, 20, 1730, 10134, 12099, 10133, 22348, 12344, 10070, 10504, 12344, 6521, 95152, 2353, 22330, 22331, 22332, 22333, 22334, 22335, 22336, 22337, 22338, 26391, 26392, 26393, 2515, 3954, 12099, 6521, 10550, 26394, 26395, 26396, 26397, 26398, 26399, 10137, 11447, 3998, 3071, 5051, 3691, 3692, 3693, 3694, 3695, 3696, 2581, 2582, 2583, 2584, 2585, 6000, 6001, 1505, 1005, 1038, 9855, 1508/*PkWar*/, 1357, Game.MsgTournaments.MsgCaptureTheFlag.MapID };
        public static List<uint> TreadeOrShop = new List<uint>() { 3053, 1858, 1860, 3852, 5050, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070 };
        public static List<uint> FBMap = new List<uint>() { 5051, 5053, 5054, 5055, 5056, 5057, 5058 };
        public static List<uint> NoAgateMap = new List<uint>() { 3071, 3030, 10741, 66666, 33333, 22222, 88888, 12288, 9991, 9992, 9993, 12602, 12275, 12546, 13249, 12602, 13140, 12546, 12800, 11331, 12799, 11996, 12665, 12601, 12816, 12099, 12667, 12605, 12602, 10741, 2071, 3030, 10133, 11, 7, 5, 15, 14, 12, 8, 20, 1730, 10134, 12099, 10133, 22348, 12344, 10070, 10504, 12344, 6521, 95152, 3301, 3302, 3303, 3304, 3305, 3306, 3307, 3308, 3309, 3310, 3311, 3312, 3313, 9855, 1038, 1764, 2057, 1081, 2060, 1080, 6521, 1138, 3954, 3820, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        public static SafeRandom GetRandom = new SafeRandom();
        //public static Packets packetsForm;
        public static bool ExitRequested = false;
        public static ServerSockets.ServerSocket GameServer;
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleHandlerDelegate handler, bool add);
        private delegate bool ConsoleHandlerDelegate(int type);
        private static ConsoleHandlerDelegate handlerKeepAlive;
        public static bool ProcessConsoleEvent(int type)
        {
            lock (Database.ServerDatabase.SavingObj)
            {
                try
                {
                    ExitRequested = true;
                    if (ServerConfig.IsInterServer)
                    {
                        foreach (var client in Pool.GamePoll.Values)
                        {
                            try
                            {
                                if (client.Socket != null)
                                    client.Socket.Disconnect();
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteLine(e.ToString());
                            }
                        }
                        return true;
                    }
                    Program.ConsoleCMD("save");
                    MyConsole.WriteLine("Saving Online Clients Information .", ConsoleColor.White);

                    new MySqlCommand(MySqlCommandType.UPDATE).Select("playersonline").Set("Online", 0).Execute();
                    foreach (var client in Pool.GamePoll.Values)
                    {
                        try
                        {
                            if (client.Socket != null)
                                client.Socket.Disconnect();
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteLine(e.ToString());
                        }
                    }

                    MyConsole.WriteLine("Saving Global Information .", ConsoleColor.White);
                    if (!Program.ServerConfig.IsInterServer)
                        Server.SaveDatabase();
                    Server.SaveDBPayers();
                    // حفظ المديولات (Votes/ShareVIP/RechargeShop) عند الإغلاق كمان، مش بس في الحفظ الدوري
                    try { global::Qahr.Mobile.Modules.ModuleManager.Save(); }
                    catch (Exception exMod) { MyConsole.WriteLine(exMod.ToString()); }
                    MyConsole.WriteLine("Database has been saved successfully .", ConsoleColor.White);
                    MyConsole.WriteLine("Waiting for online players information to be saved .", ConsoleColor.White);
                    if (Database.ServerDatabase.LoginQueue.Finish())
                    {
                        MyConsole.WriteLine("Online players information saved successfully .", ConsoleColor.White);
                        System.Threading.Thread.Sleep(1000);
                    }
                    global::Qahr.Mobile.Database.Storage.MirrorQueue.Flush(); // [Storage] تفريغ طابور مرآة القاعدة (مهلة AsyncMirrorFlushSec)
                    if (GameServer != null)
                        GameServer.Close();

                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
        }


        public static unsafe void PrintPacket(ServerSockets.Packet stream)
        {
            byte[] b = new byte[stream.Size];
            fixed (byte* ptr = b)
            {
                stream.memcpy(ptr, stream.Memory, stream.Size);
            }
            foreach (byte D in b)
            {
                System.Console.Write((Convert.ToString(D, 16)).PadLeft(2, '0').ToUpper() + " ");
            }
            System.Console.Write("\n\n");

        }
        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name == null)
                return false;
            if (name == "")
                return false;
            string ValidChars = "[^A-Za-z0-9ء-ي*~.&.$]$";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(ValidChars);
            if (r.IsMatch(name))
                return false;
            if (name.ToLower().Contains("gm"))
                return false;
            if (name.ToLower().Contains("pm"))
                return false;
            if (name.ToLower().Contains("p~m"))
                return false;
            if (name.ToLower().Contains("p!m"))
                return false;
            if (name.ToLower().Contains("g~m"))
                return false;
            if (name.ToLower().Contains("g!m"))
                return false;
            if (name.ToLower().Contains("help"))
                return false;
            if (name.ToLower().Contains("desk"))
                return false;
            if (name.ToLower().Contains("abdallah"))
                return false;
            if (name.Contains('/'))
                return false;
            if (name.Contains(@"\"))
                return false;
            if (name.Contains(@"'"))
                return false;
            //    if (name.Contains('#'))
            //      return false;
            if (name.Contains("GM") ||
                name.Contains("PM") ||
                name.Contains("None") ||
                name.Contains("Yousef") ||
                name.Contains("SYSTEM") ||
                name.Contains("{") || name.Contains("}") || name.Contains("[") || name.Contains("]"))
                return false;
            if (name.Length > 16 && ExceptedSize)
                return false;
            for (int x = 0; x < name.Length; x++)
                if (name[x] == 25)
                    return false;
            return true;
        }
        public static class ServerConfig
        {
            public static string LogginKey = "C238xs65pjy7HU9Q";// QahrMobile: the mobile client's initial CAST5 key (libCQ2ClientEn, next to "TQClient")

            public static string CO2Folder = "";

            public static string XtremeTopLink = "";
            public static uint ServerID = 0;

            public static uint ConquerorWinner = 0;
            public static string IPAddres = "";
            public static ushort AuthPort = 0;
            public static ushort GamePort = 0;
            public static string ServerName = "";
            public static string Program = "";
            //InternetPort
            public static ushort Port_BackLog;
            public static ushort Port_ReceiveSize = 16384;
            public static ushort Port_SendSize = 16384;

            //Database
            public static string DbLocation = "";


            public static uint ExpRateSpell = 3;
            public static uint ExpRateProf = 3;
            public static uint UserExpRate = 2;
            public static uint PhysicalDamage = 100;// + 150%

            public static bool IsInterServer = false;

            // [Security flaw 1/2] One-time login session token settings (read from shell.ini [Security]).
            public static int LoginTokenTtlSeconds = 30;
            public static bool LoginTokenBindIp = true;

            // [LiveRates 2026-10-17] experience rates, tunable LIVE from ServerConfig.ini [Rates] via the
            // "reloadrates" hot-reload command (no restart). Each default = the previous hardcoded value, so
            // a missing [Rates] section keeps the old behaviour exactly.
            public static void LoadRates()
            {
                UserExpRate  = (uint)System.Math.Max(0, SharedConfig.Int("Rates", "UserExpRate",  (int)UserExpRate));
                ExpRateSpell = (uint)System.Math.Max(0, SharedConfig.Int("Rates", "ExpRateSpell", (int)ExpRateSpell));
                ExpRateProf  = (uint)System.Math.Max(0, SharedConfig.Int("Rates", "ExpRateProf",  (int)ExpRateProf));
            }
        }
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string text = ex != null ? ex.ToString() : (e.ExceptionObject != null ? e.ExceptionObject.ToString() : "Unknown unhandled exception");

            Console.WriteLine(text);

            try
            {
                string crashLog = System.IO.Path.Combine(Application.StartupPath, "crash.log");
                System.IO.File.AppendAllText(crashLog,
                    "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] UnhandledException (IsTerminating=" + e.IsTerminating + "):" + Environment.NewLine +
                    text + Environment.NewLine + Environment.NewLine);
            }
            catch { }
        }
        public static void Maintenance()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (5 Minutes)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (5 Minutes)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {

                MyConsole.WriteLine("The server will be shut down for maintenance in (4 Minutes & 30 Seconds)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (4 Minutes & 30 Seconds)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (4 Minutes)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (4 Minutes)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (3 Minutes & 30 Seconds)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (3 Minutes & 30 Seconds)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (3 Minutes)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (3 Minutes)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (2 Minutes & 30 Seconds)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (2 Minutes & 30 Seconds)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (2 Minutes)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (2 Minutes)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (1 Minutes & 30 Seconds)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (1 Minutes & 30 Seconds)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (1 Minutes)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (1 Minutes)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                MyConsole.WriteLine("The server will be shut down for maintenance in (30 Seconds)");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance in (30 Seconds)", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
            }
            Thread.Sleep(1000 * 20);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                Program.svrmaintance = 1;
                MyConsole.WriteLine("The server will be shut down for maintenance now");
                MsgMessage msg = new MsgMessage(" The server will be shut down for maintenance now", "ALLUSERS", "[System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                Server.SendGlobalPacket(msg.GetArray(rec.GetStream()));
                Server.SaveDatabase();
                if (Server.FullLoading && !Program.ServerConfig.IsInterServer)
                {
                    foreach (var user in Pool.GamePoll.Values)
                    {
                        if (user.OnInterServer)
                            continue;

                        if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            user.ClientFlag |= Client.ServerFlag.QueuesSave;
                            Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                        }
                    }

                    MyConsole.WriteLine("All online clients have been saved successfully .", ConsoleColor.White);
                }
                if (Database.ServerDatabase.LoginQueue.Finish())
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            Thread.Sleep(1000 * 10);
            ProcessConsoleEvent(0);
            new MySqlCommand(MySqlCommandType.UPDATE).Update("online").Set("OnlineCount", 0).Execute();
            Application.Restart();
            Environment.Exit(0);
        }
        [STAThread]
        // دقة تايمر ويندوز الافتراضية 15.6ms، فكل Sleep(1) في ثريدات الاستقبال كان بياخد ~15.6ms
        // وده كان سقف لعدد الباكتات في الثانية (لاج مع عدد لاعبين كبير). 1ms = Sleep(1) بيبقى ~1ms فعلًا.
        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint uMilliseconds);

        // Windows 11 power saving: when the server's console window is minimized or covered, Windows ignores
        // timeBeginPeriod(1) and waits fall back to 15.6 ms (LoadBot: walk ack p50 1.2 ms visible vs 15.2 ms minimized).
        // Opt this process out of timer and execution-speed throttling (documented SetProcessInformation flags).
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_POWER_THROTTLING_STATE { public uint Version, ControlMask, StateMask; }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessInformation(IntPtr hProcess, int infoClass, ref PROCESS_POWER_THROTTLING_STATE info, uint size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();
        private static void DisablePowerThrottling()
        {
            try
            {
                // ProcessPowerThrottling = 4; EXECUTION_SPEED = 0x1, IGNORE_TIMER_RESOLUTION = 0x4; StateMask 0 = off
                var s = new PROCESS_POWER_THROTTLING_STATE { Version = 1, ControlMask = 0x1 | 0x4, StateMask = 0 };
                SetProcessInformation(GetCurrentProcess(), 4, ref s, (uint)Marshal.SizeOf(typeof(PROCESS_POWER_THROTTLING_STATE)));
            }
            catch { }
        }

        /// <summary>
        /// Every int/uint/double/float.Parse and number ToString() that passes no culture (ini files via
        /// IniFile, DB text, MySQL rows, GM commands) must read/write '.' as the decimal point and '-' as the
        /// sign whatever the Windows locale is (an Arabic/European locale - or ICU data on .NET 10 - would
        /// otherwise turn "1.5" into 15 or a FormatException). Only the NUMBER format becomes invariant:
        /// dates, string compare/casing and the UI culture stay the machine's, because some texts show a
        /// DateTime with its default format to players/GMs (MsgMessage "banned from chatting till ...",
        /// MsgBroadcast, GM chat panel, DoS console log) and must look exactly as before.
        /// On an en-US machine the numbers were already formatted this way, so nothing changes there.
        /// </summary>
        private static void SetNumberCultureInvariant()
        {
            try
            {
                var culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.InvariantInfo.Clone();
                culture = System.Globalization.CultureInfo.ReadOnly(culture);
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch { }
        }

        public static unsafe void Main(string[] args)
        {
            // [net10] legacy ANSI code pages (1252/1256/936...) used by AnsiText and IniFile.FileEncoding
            // only exist on .NET 5+ after this registration. Must run before anything touches them.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Phone.ConsoleTee.Start();// QahrMobile: everything written to the console also goes to console.log next to the exe
            try
            {
                SetNumberCultureInvariant();
                try { timeBeginPeriod(1); } catch { }
                DisablePowerThrottling();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                ServerSockets.Packet.SealString = "TQServer";
                handlerKeepAlive = ProcessConsoleEvent;
                SetConsoleCtrlHandler(handlerKeepAlive, true);

                // [Perf 2026-10-16] Raise the thread-pool floor so a burst (login storm / crowd event)
                // doesn't starve: the default pool grows only ~1 thread/sec, causing latency spikes, and
                // NetEngine V2 runs packet handlers on the pool. Default = max(cores*4, 100); 0 = leave as-is.
                // Tunable: ServerConfig.ini [Network] MinThreads.
                try
                {
                    int min = SharedConfig.Int("Network", "MinThreads", Math.Max(Environment.ProcessorCount * 4, 100));
                    if (min > 0)
                    {
                        int curW, curC; System.Threading.ThreadPool.GetMinThreads(out curW, out curC);
                        System.Threading.ThreadPool.SetMinThreads(Math.Max(curW, min), Math.Max(curC, min));
                        int newW, newC; System.Threading.ThreadPool.GetMinThreads(out newW, out newC);
                        MyConsole.WriteLine("[Perf] ThreadPool min threads: worker=" + newW + " iocp=" + newC + " (cores=" + Environment.ProcessorCount + ")", ConsoleColor.DarkGray);
                    }
                }
                catch { }

                Server.Initialize();
                // [Security flaw 1/2] Load one-time login token settings from shell.ini [Security].
                try
                {
                    string secBaseDir = AppDomain.CurrentDomain.BaseDirectory;
                    WindowsAPI.IniFile secIni = new WindowsAPI.IniFile(secBaseDir + "shell.ini", true);
                    // [Central config 2026-10-17] central ServerConfig.ini [Security] first; shell.ini is the fallback.
                    ServerConfig.LoginTokenTtlSeconds = SharedConfig.Int("Security", "LoginTokenTtlSeconds", secIni.ReadInt32("Security", "LoginTokenTtlSeconds", 30));
                    ServerConfig.LoginTokenBindIp = SharedConfig.Int("Security", "LoginTokenBindIp", secIni.ReadBool("Security", "LoginTokenBindIp", true) ? 1 : 0) != 0;
                }
                catch { }
                MyConsole.WriteLine("-------- The Sourse Version [ 7957 ]       --------", ConsoleColor.White);
                MyConsole.WriteLine("-------- The Sourse Edit By [ Mohy ElDin ] --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Trojan ]              --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Ninja ]               --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Warrior ]             --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Archer ]              --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Taoist ]              --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Pirate ]              --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Dragon Warrior ]      --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Monk ]                --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Archive Dune Wanderer ]       --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ New Character Dune Wanderer ] --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Eonspirit System ]            --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ Emoji System ]                --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ LoveForEver System ]          --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ HeartLock System ]            --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ New Relics System ]           --------", ConsoleColor.White);
                MyConsole.WriteLine("--------   [ New Runes System ]            --------", ConsoleColor.White);
                BootScreen.StepStart("TQProtect");
                BootScreen.StepDone();
                MyConsole.WriteLine("[ The Source has Completed Downloading ]", ConsoleColor.White);
                PerfReporter.Start(); // [Diagnostics] PerfReportSeconds in ServerConfig.ini (0 = off)
                Phone.MobileProtocol.Load();// QahrMobile: Data\mobile_ids.txt (mobile <-> server packet ids)
                Phone.MobileLayouts.Load();// QahrMobile: Data\mobile_layouts\*.txt (server <-> mobile packet formats)
                Phone.MobileNpcLooks.Load();// QahrMobile: Data\mobile_npclooks.txt (NPC bodies and names for the mobile)
                Phone.MobileProtocol.WatchFiles();// QahrMobile: the three mobile_*.txt reload by themselves when saved
                Cryptography.DHKeyExchange.KeyExchange.CreateKeys();
                Cryptography.AuthCryptography.PrepareAuthCryptography();
                #region Open Sockets
                BootScreen.StepStart("Network");
                CallBack = new ServerSockets.Thread4K();
                // [Security 2026-10-16] TransferCipher Key/Salt moved out of source into ServerConfig.ini [Transfer]
                // (shared by GameServer + AccServer; never shipped in any client patch/release).
                string _tcKey = SharedConfig.String("Transfer", "Key", "");
                string _tcSalt = SharedConfig.String("Transfer", "Salt", "");
                if (string.IsNullOrEmpty(_tcKey) || string.IsNullOrEmpty(_tcSalt))
                    throw new Exception("ServerConfig.ini [Transfer] Key/Salt are missing. Set them (the same values on AccServer).");
                TransferCipher.Key = Encoding.ASCII.GetBytes(_tcKey);
                TransferCipher.Salt = Encoding.ASCII.GetBytes(_tcSalt);
                Pool.transferCipher = new TransferCipher("127.0.0.1"); if (Program.ServerConfig.IsInterServer == false)
                    if (Program.ServerConfig.IsInterServer == false)
                    {
                        GameServer = new ServerSockets.ServerSocket(
                           new Action<ServerSockets.SecuritySocket>(p => new Client.GameClient(p))
                           , Game_Receive, Game_Disconnect);
                        GameServer.Initilize(ServerConfig.Port_SendSize, ServerConfig.Port_ReceiveSize, 1, 3);
                        GameServer.Open(Program.ServerConfig.IPAddres, Program.ServerConfig.GamePort, Program.ServerConfig.Port_BackLog);
                    }
                BootScreen.StepDone();
                // [boot] كان Database.NpcServer.LoadServerTraps() هنا (كل الخرايط). اتشال لأن GameMap.EnterMap بيحمّل
                // traps كل خريطة أول ما تتحمّل (LoadServerTraps(id))، فالنداء ده كان: (1) بيجبر 45 خريطة تتحمّل وقت
                // الإقلاع (~5.2 ثانية) و (2) بيكرّر كل trap مرتين. نفس الـ traps لسه بتظهر في كل خريطة.
                BootScreen.StepStart("InterServer");
                MsgInterServer.PipeServer.Initialize();
                BootScreen.StepDone();
                //Thread4K.Create();
                // [perf M4] SocketThread.Process() was an empty  while(true){ Sleep(1); }  loop that woke
                // ~1000x/sec and did nothing (the real receive runs on Thread4K). Not started anymore.
                //if (GameServer != null)
                //    new ServerSockets.SocketThread(GameServer).Start();
                #endregion

                Server.FullLoading = true;
                // [AdminApi] local player-admin endpoint for tools\PlayerStudio (off unless Infinity[DB]\adminapi.txt exists, 127.0.0.1 only)
                BootScreen.StepStart("AdminApi");
                // [FastBoot 2026-10-15] the parsed-ini cache (the same one Server.Initialize uses: every read still checks the
                // file's size + write time, writers invalidate it) also covers the module starts below - the festivals parse
                // ~60 monster .ini files key by key, each key was a GetPrivateProfileString file open (~125 ms).
                bool bootIniCache = !WindowsAPI.IniCache.Active;
                if (bootIniCache) WindowsAPI.IniCache.Begin();
                try {
                global::Qahr.Mobile.AdminApi.AdminApiServer.Start();
                // [HotReload] optional file watchers (off unless Infinity[DB]\hotreload.txt turns them on)
                global::Qahr.Mobile.HotReload.HotReloadManager.Start();
                // [Festivals] Infinity[DB]\Festivals\<key>\ - loads, then checks the schedules every minute (live on/off)
                global::Qahr.Mobile.DreamTheater.DreamTheaterModule.Init();// [DreamTheater] quest event keys before the festival quests are built
                global::Qahr.Mobile.QuellDemons.QuellDemonsModule.Init();// [QuellDemons 2026-10-14] Infernix Ruins bosses (schedule, ranking, mail rewards)
                global::Qahr.Mobile.Festivals.FestivalManager.Start();
                global::Qahr.Mobile.NewGuildWar.NgwEngine.Start();// [NewGuildWar 2026-10-14] New Guild War schedule (Infinity[DB]\NewGuildWar\config.ini)
                global::Qahr.Mobile.GiftCenter.GiftCenterModule.Start();// [GiftCenter 2026-10-14] gift records + login push (Infinity[DB]\GiftCenter)
                global::Qahr.Mobile.Lottery.LotteryModule.Start();// [Lottery 2026-10-14] Golden Tree schedule + flop lottery (Infinity[DB]\Lottery)
                }
                finally
                {
                    if (bootIniCache) WindowsAPI.IniCache.End();// [FastBoot 2026-10-15]
                    global::Qahr.Mobile.QuestEngine.QuestGrammar.BootSpawnMemo = false;// [FastBoot 2026-10-15] boot's 2 quest builds done; reloads re-read the spawn files
                }
                BootScreen.StepDone();
                BootScreen.Finish("Server is ONLINE  ·  port " + ServerConfig.GamePort);
                MapLoadLog.StartBenchIfEnabled();
                new ThreadPlayer(100, "ThreadPlayer").Start();
                new ThreadFunctions(500, "ThreadFunction").Start();
                for (; ; )
                    ConsoleCMD(Console.ReadLine());
            }
            catch (Exception e) { BootScreen.Abort(); MyConsole.WriteException(e); }

        }
        public unsafe static void ConsoleCMD(string cmd)
        {
            try
            {
                // Console.ReadLine() returns null when the console input is closed or interrupted
                if (cmd == null)
                {
                    System.Threading.Thread.Sleep(200);
                    return;
                }
                cmd = cmd.Trim();
                if (cmd.Length == 0)
                    return;
                if (cmd.StartsWith("@"))
                    cmd = cmd.Substring(1); // in-game style "@tool" works in the console too
                string[] line = cmd.Split(' ');

                switch (line[0])
                {
                    case "loadconfig":
                        {
                            global::Qahr.Mobile.Base.Security.DosProtection.Reload();
                            MyConsole.WriteLine("Configuration reloaded successfully.", ConsoleColor.Green);
                            break;
                        }
                    case "tool":
                    case "tools":
                        {
                            global::Qahr.Mobile.ServerTools.ToolLauncher.Console(line);
                            break;
                        }
                    //case "pac":
                    //    {
                    //        Program.packetsForm = new Packets();
                    //        Program.packetsForm.ShowDialog();
                    //        break;
                    //    }
                    #region Load
                    case "loadsobnpc":
                        {
                            NpcServer.LoadSobNpcs();
                            MyConsole.WriteLine("Loaded SobNpc Done");
                            break;
                        }

                    case "loadnpc": // [HotReload] the old full re-add could not move/remove NPCs; use the live diff
                    case "reloadnpcs":
                    case "reloadportals":
                    case "reloadshops":
                    case "reloaditems":
                    case "reloadmonsters":
                    case "reloadmapcells":
                    case "reloadquests":// [QuestEngine]
                    case "reloaddrops":// [FileDrops]
                    case "reloadatkspeed":// [AttackSpeed]
                    case "atkspeed":
                    case "reloadall":
                        {
                            // e.g.  reloadnpcs   reloadnpcs force   reloadmapcells 1002   reloadall   atkspeed
                            string rop = line[0] == "loadnpc" ? "reloadnpcs" : (line[0] == "atkspeed" ? "reloadatkspeed" : line[0]);
                            string rarg = line.Length > 1 ? line[1] : null;
                            global::Qahr.Mobile.HotReload.HotReloadManager.Run(rop, rarg, rarg == "force");
                            break;
                        }
                    case "festival":// [Festivals] festival list | info <key> | on <key> | off <key> | auto <key> | reload
                    case "festivals":
                        {
                            foreach (string l in global::Qahr.Mobile.Festivals.FestivalManager.Command(line.Skip(1).ToArray(), "console"))
                                MyConsole.WriteLine(l);
                            break;
                        }
                    case "questinfo":// [QuestEngine] questinfo <MissionId>
                        {
                            uint qid;
                            if (line.Length > 1 && uint.TryParse(line[1], out qid))
                                foreach (string l in global::Qahr.Mobile.QuestEngine.QuestEngineCore.Describe(qid))
                                    MyConsole.WriteLine(l);
                            break;
                        }
                    #endregion
                    #region item
                    case "rss":
                        {
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.MyNinja = new Role.Instance.Ninja(client);
                                Database.ServerDatabase.LoadClientItems(client);
                                var item = client.MyNinja.Items.Values.Where(p => p.ItemID == 600).FirstOrDefault();
                                if (item != null)
                                {

                                }
                            }
                            break;

                        }
                    case "return":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                ini.FileName = fname;
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.Player.Name = ini.ReadString("Character", "Name", "None");
                                Database.ServerDatabase.LoadClientItems(client);
                                foreach (var a in client.Warehouse.ClientItems)
                                {
                                    foreach (var b in a.Value.Values)
                                    {
                                        if (b.WH_ID == 65535)
                                        {
                                            if (b.StackSize > 0)
                                                b.StackSize = 0;
                                            client.Warehouse.ClientItems.Remove(b.UID);
                                            MyConsole.WriteLine("Done Restart Items");
                                        }

                                    }
                                }
                                Database.ServerDatabase.SaveClientItems(client);
                            }
                            break;
                        }
                    
                    case "rperf":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                ini.FileName = fname;
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.Player.Name = ini.ReadString("Character", "Name", "None");
                                Database.ServerDatabase.LoadClientItems(client);
                                var list = client.Equipment.ClientItems.Values.ToList();
                                foreach (var item in list)
                                {
                                    if (item.PerfectionLevel >= 27)
                                        item.PerfectionLevel = 0;
                                }
                                foreach (var a in client.Warehouse.ClientItems)
                                {
                                    foreach (var b in a.Value.Values)
                                    {
                                        if (b.PerfectionLevel >= 27)
                                            b.PerfectionLevel = 0;
                                    }
                                }
                                var listr = client.Inventory.ClientItems.Values.ToList();
                                foreach (var item in listr)
                                {
                                    if (item.PerfectionLevel >= 27)
                                        item.PerfectionLevel = 0;

                                }
                                Database.ServerDatabase.SaveClientItems(client);
                                MyConsole.WriteLine("Done Restart Items");
                            }
                            break;
                        }
                    case "rPlus":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                ini.FileName = fname;
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.Player.Name = ini.ReadString("Character", "Name", "None");
                                Database.ServerDatabase.LoadClientItems(client);
                                foreach (var a in client.Warehouse.ClientItems)
                                {
                                    foreach (var b in a.Value.Values)
                                    {
                                        if (b.Bound > 0)
                                        {
                                            if (b.Plus == 8)
                                                b.Plus = 0;
                                        }

                                    }
                                }
                                var listr = client.Inventory.ClientItems.Values.ToList();
                                foreach (var item in listr)
                                {
                                    if (item.Bound > 0)
                                    {
                                        if (item.Plus == 8)
                                            item.Plus = 0;
                                    }

                                }
                                Database.ServerDatabase.SaveClientItems(client);
                                MyConsole.WriteLine("Done Restart Items");
                            }
                            break;
                        }
                    case "rPokerCard":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                ini.FileName = fname;
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.Player.Name = ini.ReadString("Character", "Name", "None");
                                Database.ServerDatabase.LoadClientItems(client);
                                foreach (var a in client.Warehouse.ClientItems)
                                {
                                    foreach (var b in a.Value.Values)
                                    {
                                        if (b.ITEM_ID >= 3390072 && b.ITEM_ID <= 3390084)
                                            client.Warehouse.ClientItems.Remove(b.UID);
                                    }
                                }
                                var listr = client.Inventory.ClientItems.Values.ToList();
                                foreach (var item in listr)
                                {
                                    if (item.ITEM_ID >= 3390072 && item.ITEM_ID <= 3390084)
                                        client.Inventory.ClientItems.Remove(item.UID);
                                }
                                Database.ServerDatabase.SaveClientItems(client);
                                MyConsole.WriteLine("Done Restart Items");
                            }
                            break;
                        }
                    case "removeAnima":
                        {
                            string Name = "";
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                Name = ini.ReadString("Character", "Name", "None");
                            }

                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                client.Player.Name = Name;
                                Database.ServerDatabase.LoadClientItems(client);
                                var list = client.Equipment.ClientItems.Values.ToList();
                                foreach (var item in list)
                                {
                                    if (item.AnimaItemID >= 4200012)
                                    {
                                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                        ItemDat.UID = Pool.ITEM_Counter.Next;
                                        item.AnimaItemID = 0;
                                        Game.MsgServer.MsgGameItem ItemDatS = new Game.MsgServer.MsgGameItem();
                                        ItemDatS.UID = Pool.ITEM_Counter.Next;
                                        ItemDatS.ITEM_ID = item.AnimaItemID;
                                        ItemDatS.Mode = Role.Flags.ItemMode.AddItemReturned;
                                        ItemDatS.WH_ID = ushort.MaxValue;
                                        MyConsole.WriteLine("Done Restart Items " + clientUID + "");

                                    }
                                }
                                //foreach (var a in client.Warehouse.ClientItems)
                                //{
                                //    foreach (var b in a.Value.Values)
                                //    {
                                //        if (b.ITEM_ID >= 4200001 && b.ITEM_ID <= 4200018)
                                //        {
                                //            b.Position = 255;
                                //            b.WH_ID = 0;
                                //            client.Warehouse.ClientItems.Remove(b.UID);
                                //        }
                                //        if (b.AnimaItemID >= 4200001 && b.AnimaItemID <= 4200018)
                                //        {
                                //            b.AnimaItemID = 0;
                                //        }
                                //        MyConsole.WriteLine("Done Restart Items" + clientUID + "");

                                //    }
                                //}
                                //var listr = client.Inventory.ClientItems.Values.ToList();
                                //foreach (var item in listr)
                                //{
                                //    if (item.ITEM_ID >= 4200001 && item.ITEM_ID <= 4200018)
                                //    {
                                //        client.Inventory.ClientItems.Remove(item.UID);
                                //    }
                                //    if (item.AnimaItemID >= 4200001 && item.AnimaItemID <= 4200018)
                                //        item.AnimaItemID = 0;
                                //    MyConsole.WriteLine("Done Restart Items" + clientUID + "");

                                //}
                                Database.ServerDatabase.SaveClientItems(client);
                            }
                            break;
                        }
                    
                    case "ra":
                        {
                            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                            {
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".bin", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                client.Inventory = new Role.Instance.Inventory(client);
                                client.Equipment = new Role.Instance.Equip(client);
                                client.Warehouse = new Role.Instance.Warehouse(client);
                                client.MyWardrobe = new Role.Instance.Wardrobe(client);
                                client.Rune = new Role.Instance.Rune(client);
                                Database.ServerDatabase.LoadClientItems(client);

                                foreach (var a in client.Warehouse.ClientItems)
                                {
                                    foreach (var b in a.Value.Values)
                                    {
                                        if (b.PerfectionLevel > 0)
                                            b.PerfectionLevel = 0;

                                    }
                                }
                                var list = client.Inventory.ClientItems.Values.ToList();
                                foreach (var item in list)
                                {
                                    if (item.PerfectionLevel > 0)
                                        item.PerfectionLevel = 0;
                                    if (item.OwnerName != string.Empty)
                                        client.Inventory.ClientItems.Remove(item.UID);
                                }
                                Database.ServerDatabase.SaveClientItems(client);
                                MyConsole.WriteLine("Done Restart Items");
                            }
                            break;

                        }
                    #endregion

                    #region commandsConsle
                    case "exit":
                        {
                            ProcessConsoleEvent(0);
                            Environment.Exit(0);
                            break;
                        }
                    case "down":
                        {
                            new Thread(new ThreadStart(Maintenance)).Start();
                            break;
                        }
                    case "save":
                        {
                            Server.SaveDatabase();
                            if (Server.FullLoading && !Program.ServerConfig.IsInterServer)
                            {
                                foreach (var user in Pool.GamePoll.Values)
                                {
                                    if (user.Fake || user.OnInterServer || user.Socket == null || !user.Socket.Alive)
                                        continue;
                                    if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                    {
                                        user.ClientFlag |= Client.ServerFlag.QueuesSave;
                                        Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                                    }
                                }
                                Server.SaveDatabase();
                                MyConsole.WriteLine("Database Has Been Saved Successfully .", ConsoleColor.White);
                            }
                            if (Database.ServerDatabase.LoginQueue.Finish())
                            {
                                System.Threading.Thread.Sleep(1000);
                                MyConsole.WriteLine("Database Saved Successfully .", ConsoleColor.White);
                            }
                            break;
                        }
                    case "kick":
                        {

                            foreach (var user in Pool.GamePoll.Values)
                            {
                                if (user.Player.Name.Contains(line[1]))
                                {
                                    user.EndQualifier();
                                }
                            }
                            break;
                        }
                    #endregion
                    #region Panels
                    //case "packets":
                    //    {
                    //        Program.packetsForm = new Packets();
                    //        Program.packetsForm.ShowDialog();
                    //        break;
                    //    }

                    case "clear":
                        {
                            Console.Clear();
                            break;
                        }

                    #endregion
                    #region Events
                   

                    //case "ChampionsOfWar":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ChampionsOfWarr.Start();
                    //        break;
                    //    }
                    //case "ChampionsOfWarrEnd":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ChampionsOfWarr.End();
                    //        break;
                    //    }

                    //case "UnionWar":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.UnionWar.Start();
                    //        break;
                    //    }
                    //case "UnionWarEnd":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.UnionWar.FinishRound();
                    //        break;
                    //    }

                    /////
                    //case "ScoreGuildWar":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.TopWarScore.Start();
                    //        break;
                    //    }
                    //case "ScoreGuildWarEnd":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.TopWarScore.End();
                    //        break;
                    //    }
                   

                    case "WarOfPlayers":
                        {
                            Game.MsgTournaments.MsgWarOfPlayers.Start();

                            break;
                        }
                    case "WarOfPlayersEnd":
                        {
                            Game.MsgTournaments.MsgWarOfPlayers.End();

                            break;
                        }

                    //case "eliteEnd":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.EliteGuildWar.End();
                    //        break;
                    //    }
                    case "StartSuper":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                            break;
                        }
                    case "finishsSuper":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                            break;
                        }

                    case "5555":
                        {
                            Game.MsgTournaments.MsgSchedules.CpsCastle.Start();
                            break;
                        }
                    case "55555":
                        {
                            Game.MsgTournaments.MsgSchedules.CpsCastle.Finish();
                            break;
                        }





                    case "cpscastle":
                        {
                            Game.MsgTournaments.MsgSchedules.CpsCastle.Start();
                            break;
                        }                

                    case "ctfon":
                        {
                            Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Start();
                            break;
                        }
                    case "ctfonCheckFinish":
                        {
                            Game.MsgTournaments.MsgSchedules.CaptureTheFlag.CheckFinish();
                            break;
                        }
                    case "pk":
                        {
                            Game.MsgTournaments.MsgSchedules.ElitePkTournament.Start();

                            break;
                        }
                    case "teampk":
                        {
                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Start();



                            break;
                        }
                    case "teampk2":
                        {
                            Game.MsgTournaments.MsgSchedules.TeamPkTournament.Start();



                            break;

                        }
                    case "Startgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                            break;
                        }
                    case "finishgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                            break;
                        }
                    case "sgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                            break;
                        }
                    case "offsgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                            break;
                        }

                    //case "elite":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.EliteGuildWar.Start();
                    //        break;
                    //    }
                    case "Strike":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

                                foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\Test\\"))
                                {
                                    foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                                    {
                                        foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                                        {
                                            ini.FileName = ffile;
                                            uint Price = 1;
                                            var runes = Pool.ItemsBase.Values.Where((p => Database.ItemType.ItemPosition(p.ID) == (ushort)Role.Flags.ConquerItem.Garment || Database.ItemType.ItemPosition(p.ID) == (ushort)Role.Flags.ConquerItem.SteedMount)).ToArray();
                                            for (var i = 0; i < runes.Length; i++)
                                            {
                                                if (runes[i].Imunity > 0 || runes[i].Crytical > 0 || runes[i].SCrytical > 0 || runes[i].ItemHP > 0)
                                                    ini.WriteString("cq_generator", "Item" + i + "", "" + runes[i].ID.ToString() + "@@" + Price + "@@0@@0@0@0@0@@1");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Garments":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

                                foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\Test\\"))
                                {
                                    foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                                    {
                                        foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                                        {
                                            ini.FileName = ffile;
                                            uint Price = 1;
                                            var runes = Database.CoatStorage.GarmentsBig5Star.Values.ToArray();
                                            for (var i = 0; i < runes.Length; i++)
                                            {
                                                ini.WriteString("cq_generator", "Item" + i + "", "" + runes[i].ID.ToString() + "@@" + Price + "@@0@@0@0@0@0@@1");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Mounts":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

                                foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\Test\\"))
                                {
                                    foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                                    {
                                        foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                                        {
                                            ini.FileName = ffile;
                                            uint Price = 1;
                                            var runes = Database.CoatStorage.MountsBig5Star.Values.ToArray();
                                            for (var i = 0; i < runes.Length; i++)
                                            {
                                                ini.WriteString("cq_generator", "Item" + i + "", "" + runes[i].ID.ToString() + "@@" + Price + "@@0@@0@0@0@0@@1");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "RuneY":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

                                foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\Test\\"))
                                {
                                    foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                                    {
                                        foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                                        {
                                            ini.FileName = ffile;
                                            uint Price = 1;
                                            var runes = Pool.ItemsBase.Values.Where((p => p.ID >= 4035009 && p.ID <= 4070009)).ToArray();
                                            for (var i = 0; i < runes.Length; i++)
                                            {
                                                if (runes[i].ID % 10 == 9)
                                                    ini.WriteString("cq_generator", "Item" + i + "", "" + runes[i].ID.ToString() + "@@" + Price + "@@0@@0@0@0@0@@1");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    //case "Clan1":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanTwin.Start();
                    //        break;
                    //    }
                    //case "Clan2":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanPhoenix.Start();
                    //        break;
                    //    }

                    //case "Clan3":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanApe.Start();
                    //        break;
                    //    }
                    //case "Clan4":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanDesert.Start();
                    //        break;
                    //    }
                    //case "Clan5":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanBird.Start();
                    //        break;
                    //    }
                    //case "Clans1":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanTwin.End();
                    //        break;
                    //    }
                    //case "Clans2":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanPhoenix.End();
                    //        break;
                    //    }
                    //case "Clans3":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanApe.End();
                    //        break;
                    //    }
                    //case "Clans4":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanDesert.End();
                    //        break;
                    //    }
                    //case "Clans5":
                    //    {
                    //        Game.MsgTournaments.MsgSchedules.ClanBird.End();
                    //        break;
                    //    }
                    #endregion
                    #region Commands
                    case "search":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                string Name = ini.ReadString("Character", "Name", "None");
                                if (Name.ToLower() == line[1].ToLower() || Name.Contains(line[1]))
                                {
                                    Console.WriteLine(ini.ReadUInt32("Character", "UID", 0));
                                    break;
                                }

                            }
                            break;
                        }
                    case "resetavatar":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                string Name = ini.ReadString("Character", "Name", "None");
                                ini.WriteString("Character", "HairfaceStorage", "0");
                            }

                            break;
                        }
                    case "resetnobility":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "Level", 0);
                                nobility = 0;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                                MyConsole.WriteLine("Done Restart DonationNobility");
                            }

                            break;
                        }
                    case "ResstLevel":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ushort Level = ini.ReadUInt16("Character", "Level", 0);
                                ushort Level1 = ini.ReadUInt16("Character", "FRL", 0);
                                ushort Level2 = ini.ReadUInt16("Character", "SRL", 0);
                                if (Level > 129 || Level1 > 129 || Level2 > 129)
                                {
                                    ini.Write<ulong>("Character", "Level", 129);
                                    ini.Write<ulong>("Character", "FRL", 129);
                                    ini.Write<ulong>("Character", "SRL", 129);
                                }
                                MyConsole.WriteLine("Done Restart Level");
                            }

                            break;
                        }
                    case "RestFlower":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                string Flowers = ini.ReadBigString("Character", "Flowers", "");
                                Flowers = "";
                                ini.WriteString("Character", "Flowers", Flowers);
                                MyConsole.WriteLine("Done Restart Flowers");
                            }

                            break;
                        }
                    case "restmoney":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong Money = ini.ReadUInt64("Character", "Money", 0);
                                Money = 0;
                                ini.Write<ulong>("Character", "Money", Money);
                                ini.Write<ulong>("Character", "WHMoney", Money);

                            }
                            MyConsole.WriteLine("Done Restart Money");
                            break;
                        }
                    case "restcps":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong ConquerPoints = ini.ReadUInt64("Character", "ConquerPoints", 0);
                                ConquerPoints = 0;
                                ini.Write<ulong>("Character", "ConquerPoints", ConquerPoints);

                            }
                            MyConsole.WriteLine("Done Restart CPS");
                            break;
                        }
                    case "Any":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.WriteString("Character", "Flowers", "");

                            }
                            MyConsole.WriteLine("Done Flowers");
                            break;
                        }
                    case "rp":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                uint clientUID = uint.Parse(System.IO.Path.GetFileName(fname).Replace(".ini", ""));
                                Client.GameClient client = new Client.GameClient(null);
                                client.Player.UID = clientUID;
                                Database.ServerDatabase.LoadCharacter(client, (uint)clientUID);
                                if (client.MyArchives.UniversalFragment > 0)
                                {
                                    client.MyArchives.UniversalFragment = 0;
                                    Database.ServerDatabase.SaveClient(client);
                                    Console.WriteLine("Done Pirate Rest !");
                                }
                            }
                            break;
                        }
                    case "rop":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong OnlineMinutes = ini.ReadUInt64("Character", "OnlineMinutes", 0);
                                OnlineMinutes = 0;
                                ini.Write<ulong>("Character", "OnlineMinutes", OnlineMinutes);
                                MyConsole.WriteLine("Done Restart CPS");
                            }

                            break;
                        }
                    case "bancount":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong BanCount = ini.ReadUInt64("Character", "BanCount", 0);
                                BanCount = 0;
                                ini.Write<ulong>("Character", "BanCount", BanCount);
                                MyConsole.WriteLine("Done !");
                            }

                            break;
                        }

                    case "restvip":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<ulong>("Character", "VipLevel", 1);
                                ini.Write<long>("Character", "VipTime", 0);
                                MyConsole.WriteLine("Done Restart Money");
                            }

                            break;
                        }
                    case "wh":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;


                                ini.Write<int>("Character", "CpsBank", 0);
                                MyConsole.WriteLine("Done Restart Money");
                            }

                            break;
                        }
                    case "restartchi":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong Dragon = ini.ReadUInt64("Character", "Dragon", 0);
                                ulong Pheonix = ini.ReadUInt64("Character", "Pheonix", 0);
                                ulong Turtle = ini.ReadUInt64("Character", "Turtle", 0);
                                ulong Tiger = ini.ReadUInt64("Character", "Tiger", 0);
                                Dragon = 0;
                                Pheonix = 0;
                                Turtle = 0;
                                Tiger = 0;
                                ini.Write<ulong>("Character", "Dragon", Dragon);
                                ini.Write<ulong>("Character", "Dragon", Dragon);
                                ini.Write<ulong>("Character", "Dragon", Dragon);
                                ini.Write<ulong>("Character", "Dragon", Dragon);
                                MyConsole.WriteLine("Done Restart chi");
                            }

                            break;
                        }
                    case "twin":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong Map = ini.ReadUInt64("Character", "Map", 0);
                                Map = 1002;
                                ini.Write<ulong>("Character", "Map", Map);
                                ini.Write<ulong>("Character", "X", 410);
                                ini.Write<ulong>("Character", "Y", 354);
                                MyConsole.WriteLine("Done Restart Map");
                            }
                            break;
                        }
                    case "restartnobility":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                ulong LastNobilityDonation = ini.ReadUInt64("Character", "LastNobilityDonation", 0);
                                nobility = nobility * 0 / 100;
                                LastNobilityDonation = 0;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);

                                ini.Write<ulong>("Character", "LastNobilityDonation", LastNobilityDonation);
                                MyConsole.WriteLine("Done Restart Donation");
                            }

                            break;
                        }
                    case "sendvip":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                byte LevelVIP = ini.ReadByte("Character", "VipLevel", 0);
                                if (LevelVIP < 4 && LevelVIP != 6 && LevelVIP != 7)
                                {
                                    ini.Write<byte>("Character", "VipLevel", 4);
                                    System.DateTime Timeeee = DateTime.FromBinary(ini.ReadInt64("Character", "VipTime", 0));
                                    Timeeee = Timeeee.AddDays(1);
                                    ini.Write<long>("Character", "VipTime", Timeeee.Ticks);
                                    MyConsole.WriteLine("Done Give Vip" + fname + "");
                                }


                            }
                            break;
                        }
                    case "restartnobility2":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong LastNobilityDonation = ini.ReadUInt64("Character", "LastNobilityDonation", 0);
                                LastNobilityDonation = LastNobilityDonation * 0 / 100;

                                ini.Write<ulong>("Character", "LastNobilityDonation", LastNobilityDonation);
                                MyConsole.WriteLine("Done Restart Donation");
                            }

                            break;
                        }

                    case "DeleteAllBagsCPs":
                        {
                            uint ItemID = 3315881;

                            foreach (var user in Pool.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    while (user.Inventory.Contain(ItemID, 1))
                                    {
                                        user.Inventory.Remove(ItemID, 1, stream);
                                    }
                                    foreach (var bank in user.Warehouse.ClientItems.Values)
                                    {
                                        foreach (var item in bank.Values.ToArray())
                                        {
                                            if (item.ITEM_ID == ItemID)
                                            {
                                                Game.MsgServer.MsgGameItem removed;
                                                bank.TryRemove(item.UID, out removed);
                                            }
                                        }
                                    }
                                }
                            }
                            Program.ConsoleCMD("save");
                            ProcessConsoleEvent(0);
                            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
                            hproces.StartInfo.FileName = "Qahr.Mobile.Game.exe";
                            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            hproces.Start();
                            Environment.Exit(0);

                            break;
                        }

                    case "restart":
                        {
                            Program.ConsoleCMD("save");
                            ProcessConsoleEvent(0);

                            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
                            hproces.StartInfo.FileName = "Qahr.Mobile.Game.exe";
                            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            hproces.Start();

                            Environment.Exit(0);

                            break;
                        }

                    #endregion
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public unsafe static void Game_Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            if (!obj.SetDHKey)
                CreateDHKey(obj, stream);
            else
            {
                try
                {
                    if (obj.Game == null)
                        return;
                    ushort PacketID = stream.ReadUInt16();
                    // [PacketLogger] client->server, already decrypted. Off unless Infinity[DB]\packetlog.txt exists.
                    if (PacketLogger.Enabled)
                        PacketLogger.Log(false, obj.Game, stream.Memory, stream.Size);
                    // QahrMobile: the mobile client's login (10124), then mobile id -> server id for every other packet
                    if (PacketID == Phone.MobileProtocol.MsgConnect)
                    {
                        Phone.MobileProtocol.Login(obj.Game, stream);
                        return;
                    }
                    ushort mobilePacketId = PacketID;
                    PacketID = Phone.MobileProtocol.ToServer(stream, PacketID);
                    if (PacketID != mobilePacketId && Phone.MobileProtocol.IsStruct(mobilePacketId) && !Phone.MobileLayouts.Has(PacketID))
                    {
                        Phone.MobileProtocol.ReportDrop(PacketID, mobilePacketId, "in");// struct from the mobile, no layout yet
                        return;
                    }
                    Phone.MobileLayouts.Incoming(stream, PacketID, mobilePacketId);// mobile C struct -> the server's protobuf
                    if (obj.Game.Player.CheckTransfer)
                        goto jmp;
                    if (obj.Game.PipeClient != null && PacketID != Game.GamePackets.MsgAchievement)
                    {
                        if (PacketID == (ushort)Game.GamePackets.MsgOsShop
             || PacketID == (ushort)Game.GamePackets.Msg2ndPsw
                  || PacketID == (ushort)Game.GamePackets.MsgLeagueOpt
                  || PacketID == (ushort)Game.GamePackets.MsgLeagueConcubines
                  || PacketID == (ushort)Game.GamePackets.MsgLeagueAllegianceList
                      || PacketID == (ushort)Game.GamePackets.MsgTokenUpdate
                          || PacketID == (ushort)Game.GamePackets.MsgLeagueImperialCourtList
                              || PacketID == (ushort)Game.GamePackets.MsgLeagueInfo
                                  || PacketID == (ushort)Game.GamePackets.MsgOverheadLeagueInfo
                                      || PacketID == (ushort)Game.GamePackets.MsgLeagueMemList
                                          || PacketID == (ushort)Game.GamePackets.MsgLeaguePalaceGuardsList
                                             || PacketID == (ushort)Game.GamePackets.MsgLeagueRank
                                                || PacketID == (ushort)Game.GamePackets.MsgLeagueSynList
                                                   || PacketID == (ushort)Game.GamePackets.MsgLeagueToken
                      || PacketID == (ushort)Game.GamePackets.MsgLeagueRobOpt)
                            goto jmp;

                        stream.Seek(stream.Size);
                        obj.Game.PipeClient.Send(stream);

                        if (PacketID != Game.GamePackets.MsgItem)
                        {

                            return;
                        }
                        stream.Seek(4);
                    }

#if TEST
                    MyConsole.WriteLine("Receive -> PacketID: " + PacketID);
#endif

                // Database.ServerDatabase.LoginQueue.Enqueue("[CallStack]" + MyConsole.log1(obj.Game.Player.Name, stream.Memory, stream.Size));
                jmp:

                    // [Security flaw 6] Login-state gate: before the client has completed login
                    // (AcceptLogin), only a strict whitelist of login/handshake packets may be
                    // dispatched. Everything else is refused so no handler can run on a half
                    // initialized, unauthenticated Player object.
                    // [Security flaw 6] Pre-login packet gate DISABLED: this client's login handshake sends
                    // several packets (e.g. 2553, 2099) before the AcceptLogin flag is set, so a strict
                    // whitelist drop here stalls login. Left off for stability; revisit with the full,
                    // verified pre-AcceptLogin packet list before re-enabling PreLoginAllowedPackets.

                    // QahrMobile: mobile-only messages (no server packet id), Networking\Mobile\MobileOnly.cs
                    if (PacketID == mobilePacketId && Phone.MobileOnly.Handle(obj.Game, stream, PacketID))
                        return;
                    // QahrMobile: guild auction 2084 / 10418 (Features\GuildAuction; no [PacketAttribute]: the registry is built at startup)
                    if (global::Qahr.Mobile.GuildAuction.GuildAuctionModule.TryHandle(obj.Game, stream, PacketID))
                        return;
                    // QahrMobile: Welfare > Weekly Cards claim, MsgUserMonthCardAction 2251 (Mobile\MobileWeekCard.cs; the PC handler is commented out)
                    if (global::Qahr.Mobile.Phone.MobileWeekCard.TryHandle(obj.Game, stream, PacketID))
                        return;
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (Pool.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {

                        if (obj == null || obj.Game == null)
                        {
                            throw new ArgumentNullException("obj or obj.Game cannot be null.");
                        }

                        if (stream == null)
                        {

                        }


                        hinvoker(obj.Game, stream);
                    }
                    else if (Game.MsgServer.UnknownPacketLog.Enabled)
                        Game.MsgServer.UnknownPacketLog.Log("packet/" + PacketID, obj.Game.Player != null ? obj.Game.Player.Name : null);
                }
                catch (Exception e) { MyConsole.WriteException(e); }
                finally
                {
                    ServerSockets.PacketRecycle.Reuse(stream);
                }
            }

        }

        public static List<ushort> DHBlockPacket = new List<ushort>();
        public static bool isblockeddhkey = false;

        // [Security flaw 6] Packets a socket is allowed to send BEFORE it finishes logging in
        // (i.e. before Client.ServerFlag.AcceptLogin is set). This is only the login handshake,
        // the TQProtect anti-cheat exchange and character creation. After AcceptLogin the full
        // handler map is available as before.
        private static readonly HashSet<ushort> PreLoginAllowedPackets = new HashSet<ushort>
        {
            GamePackets.MsgConnect,                                              // 2429 - creates g_TQProtect
            GamePackets.MsgRegister,                                             // 2205 - create character (NEW_ROLE)
        };
        public unsafe static void CreateDHKey(ServerSockets.SecuritySocket obj, ServerSockets.Packet Stream)
        {

            var Array = BitConverter.ToUInt16(obj.DHKeyBuffer.buffer, 4);
            if (Array == 0)
                return;

            int length = 60;//128
            try
            {
                byte[] buffer = new byte[length];
                bool extra = false;
                string text = System.Text.ASCIIEncoding.ASCII.GetString(obj.DHKeyBuffer.buffer, 0, obj.DHKeyBuffer.Length());
                if (!text.EndsWith("TQClient"))
                {
                    System.Buffer.BlockCopy(obj.EncryptedDHKeyBuffer.buffer, obj.EncryptedDHKeyBuffer.Length() - length, buffer, 0, length);
                    extra = true;
                }
                string key;
                if (Stream.GetHandshakeReplyKey(out key))
                {
                    obj.SetDHKey = true;
                    obj.Game.DHKey.HandleResponse(key);
                    var compute_key = obj.Game.DHKeyExchance.PostProcessDHKey(obj.Game.DHKey.ToBytes());
                    obj.Game.Crypto.GenerateKey(compute_key);
                    obj.Game.Crypto.Reset();
                }
                else
                {
                    MyConsole.WriteLine("[WARNING] GetHandshakeReplyKey not found | IP: " + obj.RemoteIp, ConsoleColor.White);
                    obj.Disconnect();
                    return;
                }
                if (extra)
                {//take 
                    Stream.Seek(0);
                    obj.Game.Crypto.Decrypt(buffer, 0, Stream.Memory, 0, length);
                    Stream.Size = buffer.Length;
                    Stream.Seek(2);
                    ushort PacketID = Stream.ReadUInt16();
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (Pool.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {
                        hinvoker(obj.Game, Stream);
                    }
                    else
                    {
                        obj.Disconnect();
                        MyConsole.WriteLine("[WARNING] DHKey not found | IP: " + obj.RemoteIp, ConsoleColor.White);
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(diSource, diTarget);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public unsafe static void Game_Disconnect(ServerSockets.SecuritySocket obj)
        {
            if (obj.Game != null && obj.Game.Player != null)
            {
                try
                {
                    Client.GameClient client;
                    if (Pool.GamePoll.TryGetValue(obj.Game.Player.UID, out client))
                    {
                        Pool.DisconnectPool.TryAdd(client.Player.UID, client);

                        if (client.OnInterServer)
                            return;
                        if ((client.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            if (obj.Game.PipeClient != null)
                                obj.Game.PipeClient.Disconnect();
                            obj.Game.Player.MapGuildWar = false;
                            //MsgSchedules.GuildWar.RemoveUSER(obj.Game);
                            MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Out Game .", ConsoleColor.White);
                            global::Qahr.Mobile.Modules.ModuleManager.Logout(client);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                try
                                {
                                    if (client.Player.InUnion)
                                    {
                                        client.Player.UnionMemeber.Owner = null;
                                    }
                                    client.Player.ClearFloor();
                                    client.CheckRouletteDisconnect();
                                    client.EndQualifier();
                                    if (client.OnInterServer)
                                        return;
                                    if (client.Team != null)
                                        client.Team.Remove(client, true);
                                    if (client.Player.MyClanMember != null)
                                        client.Player.MyClanMember.Online = false;
                                    global::Qahr.Mobile.Poker.PokerModule.OnDisconnect(client);// [Poker 2026-10-14] fold + cash out poker chips before the save is queued
                                    if (client.IsVendor)
                                        client.MyVendor.StopVending(stream);
                                    if (client.InTrade)
                                        client.MyTrade.CloseTrade(new MsgTrade.Trade() { Action = 5 });
                                    if (client.Player.MyGuildMember != null)
                                        client.Player.MyGuildMember.IsOnline = false;

                                    if (client.Player.ObjInteraction != null)
                                    {
                                        client.Player.InteractionEffect.AtkType = (ushort)Game.MsgServer.MsgAttackPacket.AttackID.InteractionStopEffect;

                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);

                                        client.Send(stream.InteractionCreate(action));

                                        client.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                        client.Player.ObjInteraction.Player.ObjInteraction = null;
                                    }


                                    client.Player.View.Clear(stream);


                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                    client.Player.View.Clear(stream);
                                }
                                finally
                                {
                                    client.ClientFlag &= ~Client.ServerFlag.LoginFull;
                                    client.ClientFlag |= Client.ServerFlag.Disconnect;
                                    client.ClientFlag |= Client.ServerFlag.QueuesSave;
                                    global::Qahr.Mobile.EventData.EventStore.OnLogout(client.Player.UID); // [EventData 2026-10-15] flush event/festival data on logout
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }

                                try
                                {
                                    client.Player.Associate.OnDisconnect(stream, client);


                                    if (client.Player.MyMentor != null)
                                    {
                                        Client.GameClient me;
                                        client.Player.MyMentor.OnlineApprentice.TryRemove(client.Player.UID, out me);
                                        client.Player.MyMentor = null;
                                    }
                                    client.Player.Associate.Online = false;
                                    lock (client.Player.Associate.MyClient)
                                        client.Player.Associate.MyClient = null;
                                    foreach (var clien in client.Player.Associate.OnlineApprentice.Values)
                                        clien.Player.SetMentorBattlePowers(0, 0);
                                    client.Player.Associate.OnlineApprentice.Clear();
                                    //Program.CallBack.Unregister(client);

                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
            else if (obj.Game != null)
            {
                if (obj.Game.ConnectionUID != 0)
                {
                    Client.GameClient client;
                    Pool.GamePoll.TryRemove(obj.Game.ConnectionUID, out client);
                }
            }
        }
    }
}