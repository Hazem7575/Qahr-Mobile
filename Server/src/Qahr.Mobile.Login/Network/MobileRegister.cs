using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: "Register" button of the mobile login page (the NetDragon "Register 99 Account" dialog, rebuilt in
    /// QahrLogin.java). A very small HTTP endpoint, off unless configured:
    ///
    ///   ServerConfig.ini [Server] MobileRegisterPort = 9548     (0 or missing = off, the default)
    ///                    [Server] MobileRegisterPerIpPerDay = 5  (accounts one IP may create in 24 h)
    ///
    ///   POST /qahr/register   Content-Type: application/x-www-form-urlencoded   body: account=..&amp;email=..&amp;password=..
    ///   -> 200 {"code":N,"msg":"..."}   0 = created, 1 = bad account name, 2 = bad password, 3 = name taken,
    ///                                    4 = too many requests/accounts from this IP, 5 = server error, 6 = bad request,
    ///                                    11 = bad e-mail, 12 = too many accounts on this e-mail
    ///                    [Server] MobileAccountsPerEmail = 5   (accounts one e-mail may own)
    ///   POST /qahr/forgot, /qahr/reset: "Forgot password?" by e-mail, see MobileRecovery.cs
    ///
    /// The account is written like every other one: `accounts` row, PBKDF2 password (PasswordHash.Hash), State 2,
    /// EntityID 0 (given on the first login by Program.HandleLogin). Login itself is unchanged (1797/1799 on the auth port).
    /// </summary>
    public static class MobileRegister
    {
        static readonly Regex AccountRx = new Regex("^[A-Za-z0-9_]{4,16}$", RegexOptions.Compiled);
        static readonly Regex EmailRx = new Regex(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);
        const int MaxOpen = 32, MaxHeader = 4096, MaxBody = 1024;
        const int RequestsPerWindow = 20;
        static readonly TimeSpan RequestWindow = TimeSpan.FromMinutes(10);
        static int perIpPerDay = 5, perEmail = 5;
        static readonly SemaphoreSlim Slots = new SemaphoreSlim(MaxOpen);
        static readonly object CreateLock = new object();

        class IpState { public DateTime WindowStart; public int Requests; public DateTime DayStart; public int Created; }
        static readonly ConcurrentDictionary<string, IpState> Ips = new ConcurrentDictionary<string, IpState>();

        public static void Start()
        {
            int port = SharedConfig.Int("Server", "MobileRegisterPort", 0);
            if (port <= 0)
                return;
            perIpPerDay = Math.Max(1, SharedConfig.Int("Server", "MobileRegisterPerIpPerDay", 5));
            perEmail = Math.Max(1, SharedConfig.Int("Server", "MobileAccountsPerEmail", 5));
            try
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                new Thread(() => Loop(listener)) { IsBackground = true, Name = "MobileRegister" }.Start();
                Console.WriteLine("Mobile register (HTTP) on port [" + port + "].");
                MobileGoogleLogin.Load();// QahrMobile: POST /qahr/google on this listener ([Server] GoogleClientId, empty = off)
            }
            catch (Exception e)
            {
                Console.WriteLine("Mobile register port " + port + " failed: " + e.Message);
            }
        }

        static void Loop(TcpListener listener)
        {
            while (true)
            {
                TcpClient client;
                try { client = listener.AcceptTcpClient(); }
                catch { Thread.Sleep(100); continue; }
                if (!Slots.Wait(0))
                {
                    try { client.Close(); } catch { }
                    continue;
                }
                Task.Run(async () =>
                {
                    try { await Serve(client); }
                    catch { }
                    finally
                    {
                        try { client.Close(); } catch { }
                        Slots.Release();
                    }
                });
            }
        }

        static async Task Serve(TcpClient client)
        {
            string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            using (var cts = new CancellationTokenSource(5000))
            {
                var s = client.GetStream();
                byte[] buf = new byte[MaxHeader + MobileGoogleLogin.MaxBody];// QahrMobile: a Google ID token is ~1-2 KB
                int got = 0, headerEnd = -1;
                while (headerEnd < 0 && got < MaxHeader)
                {
                    int n = await s.ReadAsync(buf, got, MaxHeader - got, cts.Token);
                    if (n <= 0) return;
                    got += n;
                    headerEnd = IndexOf(buf, got, "\r\n\r\n");
                }
                if (headerEnd < 0) { await Reply(s, 6, "bad request", cts.Token); return; }
                string head = Encoding.ASCII.GetString(buf, 0, headerEnd);
                string[] lines = head.Split("\r\n");
                if (MobileShopPage.Matches(lines[0])) { await MobileShopPage.Handle(s, lines[0], cts.Token); return; }// QahrMobile: in-game recharge page (GET)
                if (MobileHubPage.Matches(lines[0])) { await MobileHubPage.Handle(s, lines[0], cts.Token); return; }// QahrMobile: WebHub page (GET)
                bool google = lines[0].StartsWith("POST /qahr/google ", StringComparison.Ordinal);// QahrMobile: MobileGoogleLogin.cs
                bool forgot = lines[0].StartsWith("POST /qahr/forgot ", StringComparison.Ordinal);// QahrMobile: MobileRecovery.cs
                bool reset = lines[0].StartsWith("POST /qahr/reset ", StringComparison.Ordinal);
                if (!google && !forgot && !reset && !lines[0].StartsWith("POST /qahr/register ", StringComparison.Ordinal)) { await Reply(s, 6, "bad request", cts.Token); return; }
                int length = -1;
                foreach (string l in lines)
                    if (l.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                        int.TryParse(l.Substring(15).Trim(), out length);
                if (length < 0 || length > (google ? MobileGoogleLogin.MaxBody : MaxBody)) { await Reply(s, 6, "bad request", cts.Token); return; }
                int bodyStart = headerEnd + 4;
                while (got - bodyStart < length)
                {
                    int n = await s.ReadAsync(buf, got, bodyStart + length - got, cts.Token);
                    if (n <= 0) return;
                    got += n;
                }
                string body = Encoding.UTF8.GetString(buf, bodyStart, length);

                if (!AllowRequest(ip)) { await Reply(s, 4, "Too many requests, try again later.", cts.Token); return; }
                if (google) { await ReplyJson(s, await MobileGoogleLogin.Handle(ip, body), cts.Token); return; }
                var form = new System.Collections.Generic.Dictionary<string, string>();
                foreach (string kv in body.Split('&'))
                {
                    int eq = kv.IndexOf('=');
                    if (eq <= 0) continue;
                    form[WebUtility.UrlDecode(kv.Substring(0, eq))] = WebUtility.UrlDecode(kv.Substring(eq + 1));
                }
                if (forgot || reset)
                {
                    // sending the mail can take longer than the 5 s read window
                    string json = await Task.Run(() => forgot ? MobileRecovery.Forgot(ip, form) : MobileRecovery.Reset(ip, form));
                    using (var write = new CancellationTokenSource(5000))
                        await ReplyJson(s, json, write.Token);
                    return;
                }
                string account, email, password;
                form.TryGetValue("account", out account);
                form.TryGetValue("email", out email);
                form.TryGetValue("password", out password);
                int code; string msg;
                Create(ip, account, email, password, out code, out msg);
                await Reply(s, code, msg, cts.Token);
            }
        }

        internal static bool ValidPassword(string password)
        {
            return password != null && password.Length >= 6 && password.Length <= 32 && IsPrintableAscii(password);
        }

        static void Create(string ip, string account, string email, string password, out int code, out string msg)
        {
            account = (account ?? "").Trim();
            email = (email ?? "").Trim();
            if (!AccountRx.IsMatch(account)) { code = 1; msg = "Account: 4-16 letters, digits or _."; return; }
            if (email.Length > 64 || !EmailRx.IsMatch(email)) { code = 11; msg = "Enter a valid e-mail address (used to recover the password)."; return; }
            if (!ValidPassword(password))
            { code = 2; msg = "Password: 6-32 characters (letters, digits, symbols)."; return; }
            IpState st = Ips.GetOrAdd(ip, x => new IpState { WindowStart = DateTime.UtcNow, DayStart = DateTime.UtcNow });
            lock (st)
            {
                if (DateTime.UtcNow - st.DayStart >= TimeSpan.FromDays(1)) { st.DayStart = DateTime.UtcNow; st.Created = 0; }
                if (st.Created >= perIpPerDay) { code = 4; msg = "Too many accounts from this network today."; Log(account, ip, "REGISTER FAIL: per-IP daily limit"); return; }
            }
            try
            {
                string hash = Database.PasswordHash.Hash(password);
                int rows;
                lock (CreateLock)
                {
                    using (var conn = Database.DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var count = new MySqlConnector.MySqlCommand("SELECT COUNT(*) FROM `accounts` WHERE `Email`=@e", conn))
                        {
                            count.Parameters.AddWithValue("@e", email);
                            if (Convert.ToInt32(count.ExecuteScalar()) >= perEmail)
                            { code = 12; msg = "This e-mail already has the most accounts allowed."; Log(account, ip, "REGISTER FAIL: per-e-mail limit"); return; }
                        }
                        using (var cmd = new MySqlConnector.MySqlCommand(
                            "INSERT IGNORE INTO `accounts` (`Username`, `Password`, `IP`, `EntityID`, `State`, `Email`) VALUES (@u, @p, @ip, 0, 2, @e)", conn))
                        {
                            cmd.Parameters.AddWithValue("@u", account);
                            cmd.Parameters.AddWithValue("@p", hash);
                            cmd.Parameters.AddWithValue("@ip", ip);
                            cmd.Parameters.AddWithValue("@e", email);
                            rows = cmd.ExecuteNonQuery();
                        }
                    }
                }
                if (rows == 0) { code = 3; msg = "This account name is already taken."; Log(account, ip, "REGISTER FAIL: name taken"); return; }
                lock (st) st.Created++;
                Network.AuthPackets.MobileLoginProof.Remember(account, password, hash);
                code = 0; msg = "OK";
                Log(account, ip, "REGISTER OK (mobile)");
            }
            catch (Exception e)
            {
                code = 5; msg = "Server error, try again later.";
                Log(account, ip, "REGISTER FAIL: " + e.Message);
            }
        }

        static bool AllowRequest(string ip)
        {
            IpState st = Ips.GetOrAdd(ip, x => new IpState { WindowStart = DateTime.UtcNow, DayStart = DateTime.UtcNow });
            lock (st)
            {
                if (DateTime.UtcNow - st.WindowStart >= RequestWindow) { st.WindowStart = DateTime.UtcNow; st.Requests = 0; }
                return ++st.Requests <= RequestsPerWindow;
            }
        }

        static bool IsPrintableAscii(string s)
        {
            foreach (char c in s)
                if (c < 0x21 || c > 0x7E) return false;
            return true;
        }

        static int IndexOf(byte[] b, int len, string pat)
        {
            for (int i = 0; i + pat.Length <= len; i++)
            {
                int j = 0;
                while (j < pat.Length && b[i + j] == pat[j]) j++;
                if (j == pat.Length) return i;
            }
            return -1;
        }

        static async Task Reply(NetworkStream s, int code, string msg, CancellationToken ct)
        {
            await ReplyJson(s, "{\"code\":" + code + ",\"msg\":\"" + msg.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}", ct);
        }

        static async Task ReplyJson(NetworkStream s, string json, CancellationToken ct)
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            byte[] head = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: "
                + body.Length + "\r\nConnection: close\r\n\r\n");
            await s.WriteAsync(head, 0, head.Length, ct);
            await s.WriteAsync(body, 0, body.Length, ct);
        }

        internal static void Log(string user, string ip, string result)
        {
            Program.AuthLog(user, ip, result);
        }
    }
}
