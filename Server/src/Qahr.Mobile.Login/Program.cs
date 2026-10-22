using System;
using Qahr.Mobile.Login.Network;
using Qahr.Mobile.Shared.Cryptography;
using Qahr.Mobile.Login.Database;
using System.Windows.Forms;
using Qahr.Mobile.Login.Network.Sockets;
using Qahr.Mobile.Login.Network.AuthPackets;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;
using Qahr.Mobile.Login.Auth;
namespace Qahr.Mobile.Login
{
    public unsafe class Program
    {
        public static Counter EntityUID;
        public static FastRandom Random = new FastRandom();
        public static ServerSocket AuthServer;
        public static World World;
        public static ushort Port = 16000;
        private static object SyncLogin;
        private static System.Collections.Concurrent.ConcurrentDictionary<uint, int> LoginProtection;
        private const int TimeLimit = 10000;

        // [Security flaw 1/2] CSPRNG used to mint one-time login session tokens.
        private static readonly RNGCryptoServiceProvider SecureRng = new RNGCryptoServiceProvider();

        // [Security flaw 4] Online brute-force / account-enumeration protection (per source IP).
        private sealed class LoginStrike
        {
            public int Count;
            public DateTime WindowStart;
            public DateTime BlockedUntil;
        }
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, LoginStrike> LoginFailures
            = new System.Collections.Concurrent.ConcurrentDictionary<string, LoginStrike>();
        // Configurable in Shell.ini [Security]; defaults are conservative.
        public static int MaxLoginFailures = 6;
        public static int LoginBlockMinutes = 5;
        public static int LoginFailureWindowMinutes = 5;
        private static void WorkConsole()
        {
            while (true)
            {
                try
                {
                    CommandsAI(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        // [net10] app.config <legacyUnhandledExceptionPolicy> is ignored on .NET 5+: an exception escaping any
        // thread terminates the process. These handlers only LOG (they cannot keep the process alive), so the
        // cause is visible; the real protection is the try/catch around each long-running thread loop.
        private static void LogUnhandled(string source, object error)
        {
            string text = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + source + ": " + error;
            try { Console.WriteLine(text); } catch { }
            try { File.AppendAllText("AccServer_Unhandled.log", text + Environment.NewLine + Environment.NewLine); } catch { }
        }
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogUnhandled("UnhandledException (terminating=" + e.IsTerminating + ")", e.ExceptionObject);
        }
        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogUnhandled("UnobservedTaskException", e.Exception);
            e.SetObserved(); // same as the .NET 4.5+ default (unobserved task exceptions never crash the process)
        }
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint uMilliseconds);

        // Windows 11 power saving: when the server's console window is minimized or covered, Windows ignores
        // timeBeginPeriod(1) and waits fall back to 15.6 ms (LoadBot: walk ack p50 1.2 ms visible vs 15.2 ms minimized).
        // Opt this process out of timer and execution-speed throttling (documented SetProcessInformation flags).
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct PROCESS_POWER_THROTTLING_STATE { public uint Version, ControlMask, StateMask; }
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessInformation(IntPtr hProcess, int infoClass, ref PROCESS_POWER_THROTTLING_STATE info, uint size);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();
        private static void DisablePowerThrottling()
        {
            try
            {
                // ProcessPowerThrottling = 4; EXECUTION_SPEED = 0x1, IGNORE_TIMER_RESOLUTION = 0x4; StateMask 0 = off
                var s = new PROCESS_POWER_THROTTLING_STATE { Version = 1, ControlMask = 0x1 | 0x4, StateMask = 0 };
                SetProcessInformation(GetCurrentProcess(), 4, ref s, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PROCESS_POWER_THROTTLING_STATE)));
            }
            catch { }
        }

        static void Main(string[] args)
        {
            // 1 ms timer resolution (the AccServer never asked for it: each login waited 3 x 15.6 ms = ~47 ms)
            try { timeBeginPeriod(1); } catch { }
            DisablePowerThrottling();
            // [net10] Legacy ANSI code pages (1252, 1256, ...) only exist on .NET 5+ after this. MUST run before anything
            // touches Qahr.Mobile.Login.AnsiText.Value, otherwise it falls back to UTF-8 and login names/passwords decode differently.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // [net10] see LogUnhandled above.
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            Console.Title = "Account Server";
            World = new World();
            World.Init(); 
            EntityUID = new Counter(0);
            LoginProtection = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();
            SyncLogin = new object();
            WindowsAPI.IniFile file = new WindowsAPI.IniFile("Shell.ini");
            // [Central config 2026-10-17] login listen port from central ServerConfig.ini [Server] LoginPort; Shell.ini fallback.
            Port = (ushort)SharedConfig.Int("Server", "LoginPort", file.ReadUInt16("DataBase", "Port"));
            // [Security flaw 15] Do NOT silently fall back to root with an empty password. Default to a
            // least-privilege account name and read the real credentials from Shell.ini.
            // DB connection: read from the ONE central ServerConfig.ini [Database]; fall back to this
            // server's Shell.ini [DataBase] only if the central file is missing (so nothing breaks).
            DataHolder.CreateConnection(
                SharedConfig.String("Database", "Name", file.ReadString("DataBase", "DBName", "mohe2121")),
                SharedConfig.String("Database", "Password", file.ReadString("DataBase", "Pass", "")),
                SharedConfig.String("Database", "User", file.ReadString("DataBase", "User", "acc_server")),
                SharedConfig.String("Database", "Host", file.ReadString("DataBase", "Host", "localhost")),
                SharedConfig.Int("Database", "Port", 3306));
            // [Security flaw 4/5] Brute-force + connection-cap tuning (safe defaults when absent).
            // [Central config 2026-10-17] central ServerConfig.ini [Security] first; Shell.ini fallback.
            MaxLoginFailures = SharedConfig.Int("Security", "MaxLoginFailures", file.ReadInt32("Security", "MaxLoginFailures", 6));
            LoginBlockMinutes = SharedConfig.Int("Security", "LoginBlockMinutes", file.ReadInt32("Security", "LoginBlockMinutes", 5));
            LoginFailureWindowMinutes = SharedConfig.Int("Security", "LoginFailureWindowMinutes", file.ReadInt32("Security", "LoginFailureWindowMinutes", 5));
            ServerSocket.MaxConnectionsPerIp = SharedConfig.Int("Security", "MaxConnectionsPerIp", file.ReadInt32("Security", "MaxConnectionsPerIp", 32));
            Auth.TQAuthProtect.Load(false);

            Database.Server.Load();
            // [Security flaw 4] One-time bulk migration of any remaining plaintext passwords to PBKDF2.
            // Idempotent (hashed rows skipped), so it is safe on every boot; logs the count.
            try
            {
                int migrated = AccountTable.MigrateAllPlaintext();
                if (migrated > 0)
                    Console.WriteLine("[Security] migrated " + migrated + " plaintext password(s) to PBKDF2.");
            }
            catch (Exception e) { Console.WriteLine("[Security] password bulk migration failed: " + e.Message); }
            Network.Cryptography.AuthCryptography.PrepareAuthCryptography();
            AuthServer = new ServerSocket();
            AuthServer.OnClientConnect += AuthServer_OnClientConnect;
            AuthServer.OnClientReceive += AuthServer_OnClientReceive;
            AuthServer.OnClientDisconnect += AuthServer_OnClientDisconnect;
            AuthServer.Enable(Port, "0.0.0.0");
            MobileLoginProof.Load();// QahrMobile: [Security] MobileLoginProof (default 0 = off)
            MobileAutoPatch.Start();// QahrMobile: the mobile client's update check (port 9547) gets "READY"
            MobileRegister.Start();// QahrMobile: register button of the mobile login page ([Server] MobileRegisterPort, default 0 = off)
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Listening On Port : [" + Port + "] .");
            Console.WriteLine("----------------------------------------\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            WorkConsole();
            CommandsAI(Console.ReadLine());
        }
        public static void CommandsAI(string command)
        {
            if (command == null) return;
            string[] data = command.Split(' ');
            switch (data[0].ToLower())
            {
                case "@memo":
                    {
                        var proc = System.Diagnostics.Process.GetCurrentProcess();
                        Console.WriteLine("Thread count: " + proc.Threads.Count);
                        Console.WriteLine("Memory set(MB): " + ((double)((double)proc.WorkingSet64 / 1024)) / 1024);
                        proc.Close();
                        break;
                    }
                case "@clear":
                    {
                        Console.Clear();
                        break;
                    }
                case "banned":
                    {
                        AccountTable.Banned12(data[1]);

                        break;
                    }
                case "unbanned":
                    {
                        AccountTable.Banned12(data[1], true);

                        break;
                    }
                case "@restart":
                    {
                        AuthServer.Disable();
                        Application.Restart();
                        Environment.Exit(0);
                        break;
                    }
            }
        }

        private static TransferCipher LoginTicketCipher;

        private static void AuthServer_OnClientReceive(byte[] buffer, int length, ClientWrapper arg3)
        {
            var player = arg3.Connector as Client.AuthClient;
            player.Cryptographer.Decrypt(buffer, length);
            player.Queue.Enqueue(buffer, length);
            // [Security flaw 17] Drop clients that send malformed/oversized frames.
            if (player.Queue.Corrupted)
            {
                AuthLog("", player.IP, "DISCONNECT: malformed frame (" + length + " bytes)");
                arg3.Disconnect();
                return;
            }
            while (player.Queue.CanDequeue())
            {
                byte[] packet = player.Queue.Dequeue();
                ushort len = BitConverter.ToUInt16(packet, 0);
                ushort PacketID = BitConverter.ToUInt16(packet, 2);
                if (PacketID == MobileLogin.SDKLoginEx)
                {
                    // Mobile client, step 1: account + server name; the password comes in the proof (1799)
                    string account, server;
                    if (!MobileLogin.ReadLoginEx(packet, out account, out server))
                    {
                        AuthLog("", player.IP, "DISCONNECT: malformed mobile login (1797)");
                        arg3.Disconnect();
                        return;
                    }
                    player.MobileAccount = account;
                    player.MobileServer = server;
                    player.Send(MobileLogin.Challenge());
                }
                else if (PacketID == MobileLogin.SDKLoginProof && player.MobileAccount != null)
                {
                    // QahrMobile: "Sign in with Google" -> the session is a one-time token from MobileGoogleLogin (POST /qahr/google)
                    string googleSession = MobileLoginProof.ReadSession(packet);
                    if (MobileGoogleLogin.IsToken(googleSession))
                    {
                        string googleFail;
                        player.IsMobile = true;
                        player.MobileProof = null;
                        player.GoogleVerified = MobileGoogleLogin.Redeem(googleSession, player.MobileAccount, out googleFail);
                        if (!player.GoogleVerified)
                        {
                            AuthLog(player.MobileAccount, player.IP, "FAIL: " + googleFail);
                            RegisterLoginFailure(player.IP);
                            player.MobileAccount = null;
                            SendForward(player, new Forward() { Type = Forward.ForwardType.InvalidInfo });
                            continue;
                        }
                        player.Info = new Auth.TQAuthProtect.Authentication(player.MobileServer, player.MobileAccount, "");
                        player.MobileAccount = null;
                        HandleLogin(player, arg3);
                        continue;
                    }
                    // Mobile client, step 2: the session string is the password typed on the login screen
                    string password = MobileLogin.ReadProof(packet);
                    // QahrMobile: hashed proof instead of the password (MobileLoginProof, off unless configured)
                    string session = MobileLoginProof.Mode > 0 ? MobileLoginProof.ReadSession(packet) : null;
                    player.MobileProof = MobileLoginProof.IsProof(session) ? session : null;
                    if (player.MobileProof != null)
                        password = "";
                    else if (MobileLoginProof.Mode == 2 && password != null)
                    {
                        AuthLog(player.MobileAccount, player.IP, "FAIL: plaintext mobile password refused (MobileLoginProof=2)");
                        RegisterLoginFailure(player.IP);
                        player.IsMobile = true;
                        player.MobileAccount = null;
                        SendForward(player, new Forward() { Type = Forward.ForwardType.InvalidInfo });
                        continue;
                    }
                    if (password == null)
                    {
                        AuthLog(player.MobileAccount, player.IP, "DISCONNECT: malformed mobile proof (1799)");
                        arg3.Disconnect();
                        return;
                    }
                    player.IsMobile = true;
                    player.Info = new Auth.TQAuthProtect.Authentication(player.MobileServer, player.MobileAccount, password);
                    player.MobileAccount = null;
                    HandleLogin(player, arg3);
                }
                else if (player.IsMobile && (PacketID == MobileLogin.ConnectLegalitySpec || PacketID == MobileLogin.AfterLoginInfo))
                {
                    // Mobile client: legality/device info after the 1055 reply; nothing to answer
                }
                else
                {
                    AuthLog("", player.IP, "DISCONNECT: unexpected packet " + PacketID + " len " + len + " (expected " + (ushort)Auth.TQAuthProtect.PacketIDs._MSG_ACCOUNT + ")");
                    arg3.Disconnect();
                }
            }
        }

        /// <summary>Checks player.Info and hands the account off to the game server (PC _MSG_ACCOUNT and mobile 1799).</summary>
        private static void HandleLogin(Client.AuthClient player, ClientWrapper arg3)
        {
            var loginSw = System.Diagnostics.Stopwatch.StartNew(); // [perf] زمن معالجة الدخول (بيظهر في سطر اللوج)
            bool google = player.GoogleVerified;// QahrMobile: a redeemed Google token counts for this one login only
            player.GoogleVerified = false;
            player.Account = new AccountTable(player.Info.Username);
            Database.ServerInfo Server = null;
            Forward Fw = new Forward();
            if (Database.Server.Servers.TryGetValue(player.Info.Server, out Server))
            {
                // [Security flaw 4] Throttle brute-force / enumeration before touching the DB.
                if (IsLoginBlocked(player.IP))
                {
                    AuthLog(player.Info.Username, player.IP, "BLOCKED: too many wrong logins from this IP (disconnected)");
                    arg3.Disconnect();
                    return;
                }

                if (!player.Account.exists)
                {
                    // [Security flaw 4] Generic failure — must NOT reveal whether the account exists
                    // (previously WrongAccount vs InvalidInfo enabled account enumeration).
                    Fw.Type = Forward.ForwardType.InvalidInfo;
                    RegisterLoginFailure(player.IP);
                    AuthLog(player.Info.Username, player.IP, "FAIL: account does not exist");
                }
                else
                {
                    // [Security flaw 4] Hash-aware verification (PBKDF2) with legacy plaintext fallback.
                    // QahrMobile: a mobile "H2:" proof is checked against its verifier instead of the password
                    string proofFail = null;
                    bool passwordOk = google ? true// QahrMobile: one-time Google token already checked (MobileGoogleLogin.Redeem)
                        : player.MobileProof != null
                        ? MobileLoginProof.Verify(player.Info.Username, player.Info.Server, player.MobileProof, player.Account.Password, out proofFail)
                        : PasswordHash.Verify(player.Info.Password, player.Account.Password);
                    if (passwordOk)
                    {
                        if (!player.Account.Banned)
                            Fw.Type = Forward.ForwardType.Ready;
                        else
                        {
                            Fw.Type = Forward.ForwardType.Banned;
                            AuthLog(player.Info.Username, player.IP, "FAIL: account is banned");
                        }
                    }
                    else
                    {
                        Fw.Type = Forward.ForwardType.InvalidInfo;
                        RegisterLoginFailure(player.IP);
                        AuthLog(player.Info.Username, player.IP, "FAIL: wrong password" + (proofFail != null ? " (proof: " + proofFail + ")" : ""));
                    }
                }

                lock (SyncLogin)
                {
                    if (Fw.Type == Forward.ForwardType.Ready)
                    {
                        //AccountTable.UpdateEarth(player.Info.Username, player.Info.MacAddress);
                        AccountTable.UpdateIP(player.Info.Username, player.IP);
                        // [Security flaw 11] EntityID allocation moved inside SyncLogin so two
                        // concurrent first logins cannot be handed the same id.
                        if (player.Account.EntityID == 0)
                        {
                            using (MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT))
                            {
                                cmd.Select("configuration");
                                using (MySqlReader r = new MySqlReader(cmd))
                                {
                                    if (r.Read())
                                    {
                                        EntityUID = new Counter(r.ReadUInt32("ID"));
                                        player.Account.EntityID = EntityUID.Next;
                                        using (MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration")
                                        .Set("ID", player.Account.EntityID)) cmd2.Execute();
                                        player.Account.Save();
                                    }
                                }
                            }
                        }

                        // [Security flaw 4] Auto-migrate: on a successful login whose stored password is
                        // still plaintext, re-hash it to PBKDF2 and persist. The client keeps using the
                        // same password; only the stored value is upgraded. (Owner decision 2026-10-16:
                        // migrate every account including test ones.)
                        try
                        {
                            if (!google && !PasswordHash.IsHashed(player.Account.Password))// QahrMobile: no password on a Google login
                                AccountTable.UpdatePassword(player.Info.Username, PasswordHash.Hash(player.Info.Password));
                        }
                        catch { }
                        // QahrMobile: keep the mobile proof verifier current (only when MobileLoginProof >= 1)
                        // (a plaintext row migrated just above gets its verifier on the next login)
                        if (player.MobileProof == null && PasswordHash.IsHashed(player.Account.Password))
                            MobileLoginProof.Remember(player.Info.Username, player.Info.Password, player.Account.Password);

                        // [Security flaw 1/2/7] Mint a one-time, random, IP-bound, expiring session
                        // token; store it server-side and send only its two encrypted halves. The
                        // client can no longer forge an EntityID or replay an old ticket.
                        uint lo, hi;
                        if (GenerateLoginToken(player.Account.EntityID, (uint)player.Account.State, player.IP, out lo, out hi))
                        {
                            // Same cipher as the original working ticket (GameServer sets the matching
                            // static Key/Salt to these exact values, then uses TransferCipher("127.0.0.1")).
                            // The 3-arg ctor also initialises the static Key/Salt, so this must stay 3-arg.
                            // [perf] المفتاح ثابت، فالـ cipher بيتبني مرة واحدة بس (كان بيتبني مع كل دخول، ~0.1ms).
                            // الاستخدام جوه lock (SyncLogin) فمفيش استخدام متوازي.
                            if (LoginTicketCipher == null)
                            {
                                // [Security 2026-10-16] Key/Salt read from ServerConfig.ini [Transfer] (must match GameServer), not hardcoded.
                                string _tcKey = SharedConfig.String("Transfer", "Key", "");
                                string _tcSalt = SharedConfig.String("Transfer", "Salt", "");
                                if (string.IsNullOrEmpty(_tcKey) || string.IsNullOrEmpty(_tcSalt))
                                    throw new Exception("ServerConfig.ini [Transfer] Key/Salt are missing (must match GameServer).");
                                LoginTicketCipher = new TransferCipher(_tcKey, _tcSalt, "127.0.0.1");
                            }
                            uint[] encrypted = LoginTicketCipher.Encrypt(new uint[] { lo, hi });
                            Fw.Identifier = encrypted[0];
                            Fw.State = encrypted[1];
                            Fw.IP = Server.IP;
                            Fw.Port = Server.Port;
                            ResetLoginFailures(player.IP);
                            AuthLog(player.Info.Username, player.IP, "OK -> " + Server.Name + " " + Server.IP + ":" + Server.Port);
                            Console.WriteLine("{0} is Logging to to [{1}:{2}:{3}]. ({4:0.0} ms)", player.Info.Username, Server.Name, Server.IP, Server.Port, loginSw.Elapsed.TotalMilliseconds);
                        }
                        else
                        {
                            // Fail closed if the token could not be persisted.
                            Fw.Type = Forward.ForwardType.InvalidInfo;
                            AuthLog(player.Info.Username, player.IP, "FAIL: could not save the login token (DB error)");
                        }
                    }
                    SendForward(player, Fw);
                }
            }
            else
            {
                Console.WriteLine("Unknown Server Name [" + player.Info.Server + "]");
                AuthLog(player.Info.Username, player.IP, "FAIL: unknown server name [" + player.Info.Server + "]");
                Fw.Type = Forward.ForwardType.ServersNotConfigured;
                SendForward(player, Fw);
            }
        }

        /// <summary>PC clients get the Forward packet (1637, TQCipher), the mobile client its plain 1055 form.</summary>
        private static void SendForward(Client.AuthClient player, Forward Fw)
        {
            if (player.IsMobile)
                player.Send(MobileLogin.Reply(Fw));
            else
                player.Send(Fw);
        }
        private static void AuthServer_OnClientDisconnect(ClientWrapper obj)
        {
            obj.Disconnect();
        }
        private static void AuthServer_OnClientConnect(ClientWrapper obj)
        {
            Client.AuthClient authState;
            obj.Connector = (authState = new Client.AuthClient(obj));
            AuthLog("", authState.IP, "connected");
            authState.Cryptographer = new Network.Cryptography.AuthCryptography();
            Network.AuthPackets.PasswordCryptographySeed pcs = new PasswordCryptographySeed();
            pcs.Seed = (int)TQCipher.PublicSeed;
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
        }

        // [Security flaw 1/2/7] Mint and persist a one-time login session token.
        private static bool GenerateLoginToken(uint entityId, uint state, string ip, out uint lo, out uint hi)
        {
            lo = 0; hi = 0;
            try
            {
                ulong token = 0;
                byte[] buf = new byte[8];
                int tries = 0;
                do
                {
                    SecureRng.GetBytes(buf);
                    token = BitConverter.ToUInt64(buf, 0);
                    tries++;
                } while (token == 0 && tries < 5);
                if (token == 0) return false;

                using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("login_tokens")
                    .Insert("Token", token)
                    .Insert("EntityID", (ulong)entityId)
                    .Insert("State", (ulong)state)
                    .Insert("IP", ip ?? "")
                    .Insert("CreatedAt", DateTime.UtcNow.Ticks))
                    cmd.Execute();

                lo = (uint)(token & 0xFFFFFFFFUL);
                hi = (uint)(token >> 32);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        // [Security flaw 4] Online brute-force / enumeration throttle helpers.
        private static bool IsLoginBlocked(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            LoginStrike s;
            if (LoginFailures.TryGetValue(ip, out s))
            {
                lock (s)
                    return DateTime.UtcNow < s.BlockedUntil;
            }
            return false;
        }
        private static void RegisterLoginFailure(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return;
            LoginStrike s = LoginFailures.GetOrAdd(ip, x => new LoginStrike { Count = 0, WindowStart = DateTime.UtcNow });
            lock (s)
            {
                if ((DateTime.UtcNow - s.WindowStart).TotalMinutes >= LoginFailureWindowMinutes)
                {
                    s.Count = 0;
                    s.WindowStart = DateTime.UtcNow;
                }
                s.Count++;
                if (s.Count >= MaxLoginFailures)
                {
                    s.BlockedUntil = DateTime.UtcNow.AddMinutes(LoginBlockMinutes);
                    s.Count = 0;
                    s.WindowStart = DateTime.UtcNow;
                }
            }
        }
        private static void ResetLoginFailures(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return;
            LoginStrike s;
            LoginFailures.TryRemove(ip, out s);
        }

        // [AuthLog] one line per login result in auth_log.txt (next to the exe), so a client error like
        // "too crowded" can be traced to its cause (blocked IP, wrong password, banned, unknown server...).
        private static readonly object AuthLogLock = new object();
        internal static void AuthLog(string user, string ip, string result)// QahrMobile: internal (MobileRegister logs here too)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + (ip ?? "?") + " [" + (user ?? "") + "] " + result;
                Console.WriteLine("[Auth] " + line);
                lock (AuthLogLock)
                    System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth_log.txt"), line + Environment.NewLine);
            }
            catch { }
        }
    }
}