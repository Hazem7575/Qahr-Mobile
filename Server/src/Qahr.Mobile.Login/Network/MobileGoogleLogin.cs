using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: "Sign in with Google" of the mobile login page (QahrLogin.java, docs conquer-mobile\docs\GOOGLE_LOGIN.md).
    /// Off unless configured:
    ///
    ///   ServerConfig.ini [Server] GoogleClientId = 1234-abc.apps.googleusercontent.com   (the WEB client id; empty = off)
    ///                    [Server] MobileRegisterPort = 9548                                (the HTTP listener it shares)
    ///                    [Server] GoogleNewAccountsPerIpPerDay = 5                         (optional, new accounts per IP per 24 h)
    ///
    ///   POST /qahr/google   Content-Type: application/x-www-form-urlencoded   body: id_token=...
    ///   -> 200 {"code":N,"msg":"...","account":"...","token":"..."}
    ///      0 = ok (account + token set), 1 = invalid Google token, 2 = Google e-mail not verified,
    ///      4 = too many requests / new accounts from this IP, 5 = server error, 6 = bad request, 7 = Google login is off
    ///
    /// The ID token is checked here: RS256 signature with Google's keys (https://www.googleapis.com/oauth2/v3/certs,
    /// cached), iss, aud = GoogleClientId, exp/iat, email_verified. Its "sub" is looked up in `mobile_google_links`
    /// (Sub, Username, Email, Created); a new Google user gets a new `accounts` row (name from the e-mail, random
    /// password stored as PBKDF2 and never sent anywhere, State 2, EntityID 0) and a link row.
    /// The answer carries a one-time login token "QG1:" + 64 hex (kept in memory only, 10 minutes, single use, tied to
    /// the account). The client hands it to the game as the session string; the 1799 handler (Program.cs) redeems it
    /// with <see cref="Redeem"/> instead of checking a password. Tokens are lost when the AccServer restarts.
    /// </summary>
    public static class MobileGoogleLogin
    {
        public const string TokenPrefix = "QG1:";
        public const int MaxBody = 8192;
        static readonly TimeSpan TokenLife = TimeSpan.FromMinutes(10);
        static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(5);
        const string CertsUrl = "https://www.googleapis.com/oauth2/v3/certs";
        static readonly Regex TokenRx = new Regex("^QG1:[0-9a-f]{64}$", RegexOptions.Compiled);

        static string clientId = "";
        static int perIpPerDay = 5;
        static readonly object CreateLock = new object();
        static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(4) };

        public static bool Enabled { get { return clientId.Length > 0; } }

        class Pending { public string Account; public DateTime Expires; }
        static readonly ConcurrentDictionary<string, Pending> Tokens = new ConcurrentDictionary<string, Pending>();

        class IpDay { public DateTime Start; public int Created; }
        static readonly ConcurrentDictionary<string, IpDay> NewPerIp = new ConcurrentDictionary<string, IpDay>();

        // Google's signing keys: kid -> RSA parameters
        static volatile Dictionary<string, RSAParameters> keys = new Dictionary<string, RSAParameters>();
        static DateTime keysUntil = DateTime.MinValue, lastFetch = DateTime.MinValue;
        static readonly SemaphoreSlim KeysLock = new SemaphoreSlim(1, 1);

        /// <summary>Called by MobileRegister.Start once its listener runs.</summary>
        public static void Load()
        {
            clientId = (SharedConfig.String("Server", "GoogleClientId", "") ?? "").Trim();
            if (clientId.Length == 0)
                return;
            perIpPerDay = Math.Max(1, SharedConfig.Int("Server", "GoogleNewAccountsPerIpPerDay", 5));
            try
            {
                using (var conn = Database.DataHolder.MySqlConnection)
                using (var cmd = new MySqlConnector.MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS `mobile_google_links` (`Sub` VARCHAR(64) NOT NULL PRIMARY KEY, " +
                    "`Username` VARCHAR(64) NOT NULL, `Email` VARCHAR(255) NOT NULL DEFAULT '', `Created` BIGINT NOT NULL, " +
                    "KEY `Username` (`Username`))", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Mobile Google login on (POST /qahr/google).");
                Task.Run(() => GetKeys(null));// warm the key cache
            }
            catch (Exception e)
            {
                Console.WriteLine("Mobile Google login off, mobile_google_links failed: " + e.Message);
                clientId = "";
            }
        }

        // ------------------------------------------------------------------------------------------- HTTP request
        /// <summary>Handles the body of POST /qahr/google; returns the JSON answer.</summary>
        public static async Task<string> Handle(string ip, string body)
        {
            if (!Enabled) return Json(7, "Google login is not available.");
            string idToken = null;
            foreach (string kv in (body ?? "").Split('&'))
            {
                int eq = kv.IndexOf('=');
                if (eq > 0 && WebUtility.UrlDecode(kv.Substring(0, eq)) == "id_token")
                    idToken = WebUtility.UrlDecode(kv.Substring(eq + 1));
            }
            if (string.IsNullOrEmpty(idToken) || idToken.Length > MaxBody) return Json(6, "bad request");

            JsonElement claims;
            string why;
            try
            {
                var r = await VerifyIdToken(idToken);
                claims = r.Item1; why = r.Item2;
            }
            catch (Exception e)
            {
                Log("", ip, "GOOGLE FAIL: " + e.Message);
                return Json(5, "Server error, try again later.");
            }
            if (why != null) { Log("", ip, "GOOGLE FAIL: " + why); return Json(1, "Google sign-in failed, try again."); }

            string sub = Str(claims, "sub"), email = Str(claims, "email") ?? "";
            if (string.IsNullOrEmpty(sub) || sub.Length > 64) { Log("", ip, "GOOGLE FAIL: no sub"); return Json(1, "Google sign-in failed, try again."); }
            if (!IsTrue(claims, "email_verified")) { Log(email, ip, "GOOGLE FAIL: e-mail not verified"); return Json(2, "Your Google e-mail is not verified."); }

            string account;
            int code; string msg;
            FindOrCreate(ip, sub, email, out account, out code, out msg);
            if (code != 0) return Json(code, msg);

            string token = NewToken(account);
            Log(account, ip, "GOOGLE OK (mobile, token issued)");
            return "{\"code\":0,\"msg\":\"OK\",\"account\":\"" + Esc(account) + "\",\"token\":\"" + token + "\"}";
        }

        // ------------------------------------------------------------------------------------------ ID token check
        /// <summary>(claims, null) when the token is good, else (default, reason).</summary>
        static async Task<Tuple<JsonElement, string>> VerifyIdToken(string jwt)
        {
            var fail = new Func<string, Tuple<JsonElement, string>>(r => Tuple.Create(default(JsonElement), r));
            string[] p = jwt.Split('.');
            if (p.Length != 3) return fail("not a JWT");
            JsonElement head, claims;
            byte[] sig;
            try
            {
                head = JsonDocument.Parse(B64Url(p[0])).RootElement;
                claims = JsonDocument.Parse(B64Url(p[1])).RootElement;
                sig = B64Url(p[2]);
            }
            catch { return fail("malformed JWT"); }
            if (Str(head, "alg") != "RS256") return fail("alg " + Str(head, "alg"));
            string kid = Str(head, "kid");
            if (string.IsNullOrEmpty(kid)) return fail("no kid");

            RSAParameters key;
            if (!(await GetKeys(kid)).TryGetValue(kid, out key)) return fail("unknown key " + kid);
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(key);
                byte[] signed = Encoding.ASCII.GetBytes(p[0] + "." + p[1]);
                if (!rsa.VerifyData(signed, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)) return fail("bad signature");
            }

            string iss = Str(claims, "iss");
            if (iss != "accounts.google.com" && iss != "https://accounts.google.com") return fail("iss " + iss);
            if (!AudOk(claims)) return fail("aud is not our client id");
            DateTime now = DateTime.UtcNow;
            long exp = Num(claims, "exp"), iat = Num(claims, "iat");
            if (exp <= 0 || DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime < now - ClockSkew) return fail("expired");
            if (iat > 0 && DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime > now + ClockSkew) return fail("issued in the future");
            return Tuple.Create(claims, (string)null);
        }

        static bool AudOk(JsonElement claims)
        {
            JsonElement aud;
            if (!claims.TryGetProperty("aud", out aud)) return false;
            if (aud.ValueKind == JsonValueKind.String) return aud.GetString() == clientId;
            if (aud.ValueKind == JsonValueKind.Array)
                foreach (var a in aud.EnumerateArray())
                    if (a.ValueKind == JsonValueKind.String && a.GetString() == clientId) return true;
            return false;
        }

        /// <summary>The cached keys; fetched again when expired, or when <paramref name="kid"/> is unknown (at most once a minute).</summary>
        static async Task<Dictionary<string, RSAParameters>> GetKeys(string kid)
        {
            var k = keys;
            bool stale = DateTime.UtcNow >= keysUntil;
            if (!stale && (kid == null || k.ContainsKey(kid))) return k;
            if (!stale && DateTime.UtcNow - lastFetch < TimeSpan.FromMinutes(1)) return k;
            await KeysLock.WaitAsync();
            try
            {
                if (DateTime.UtcNow - lastFetch < TimeSpan.FromSeconds(10) && keys.Count > 0) return keys;
                lastFetch = DateTime.UtcNow;
                using (var resp = await Http.GetAsync(CertsUrl))
                {
                    resp.EnsureSuccessStatusCode();
                    var fresh = new Dictionary<string, RSAParameters>();
                    using (var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()))
                        foreach (var j in doc.RootElement.GetProperty("keys").EnumerateArray())
                        {
                            if (Str(j, "kty") != "RSA" || Str(j, "kid") == null) continue;
                            fresh[Str(j, "kid")] = new RSAParameters { Modulus = B64Url(Str(j, "n")), Exponent = B64Url(Str(j, "e")) };
                        }
                    TimeSpan life = TimeSpan.FromHours(1);
                    var cc = resp.Headers.CacheControl;
                    if (cc != null && cc.MaxAge.HasValue && cc.MaxAge.Value > TimeSpan.FromMinutes(5)) life = cc.MaxAge.Value;
                    if (fresh.Count > 0) { keys = fresh; keysUntil = DateTime.UtcNow + life; }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Google] key download failed: " + e.Message);
                if (keys.Count == 0) throw;
            }
            finally { KeysLock.Release(); }
            return keys;
        }

        // ------------------------------------------------------------------------------------------- accounts
        static void FindOrCreate(string ip, string sub, string email, out string account, out int code, out string msg)
        {
            account = null;
            try
            {
                lock (CreateLock)
                {
                    using (var conn = Database.DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var cmd = new MySqlConnector.MySqlCommand("SELECT `Username` FROM `mobile_google_links` WHERE `Sub` = @s", conn))
                        {
                            cmd.Parameters.AddWithValue("@s", sub);
                            account = cmd.ExecuteScalar() as string;
                        }
                        if (account != null)
                        {
                            using (var cmd = new MySqlConnector.MySqlCommand("SELECT COUNT(*) FROM `accounts` WHERE `Username` = @u", conn))
                            {
                                cmd.Parameters.AddWithValue("@u", account);
                                if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                                {
                                    Log(account, ip, "GOOGLE FAIL: linked account no longer exists");
                                    account = null; code = 5; msg = "Your account was not found, contact support.";
                                    return;
                                }
                            }
                            code = 0; msg = "OK";
                            return;
                        }

                        // new Google user
                        IpDay day = NewPerIp.GetOrAdd(ip, x => new IpDay { Start = DateTime.UtcNow });
                        lock (day)
                        {
                            if (DateTime.UtcNow - day.Start >= TimeSpan.FromDays(1)) { day.Start = DateTime.UtcNow; day.Created = 0; }
                            if (day.Created >= perIpPerDay)
                            {
                                Log(email, ip, "GOOGLE FAIL: per-IP daily new account limit");
                                code = 4; msg = "Too many new accounts from this network today.";
                                return;
                            }
                        }
                        string hash = Database.PasswordHash.Hash(RandomSecret());// nobody knows this password
                        foreach (string name in Candidates(email))
                        {
                            using (var cmd = new MySqlConnector.MySqlCommand(
                                "INSERT IGNORE INTO `accounts` (`Username`, `Password`, `IP`, `EntityID`, `State`) VALUES (@u, @p, @ip, 0, 2)", conn))
                            {
                                cmd.Parameters.AddWithValue("@u", name);
                                cmd.Parameters.AddWithValue("@p", hash);
                                cmd.Parameters.AddWithValue("@ip", ip);
                                if (cmd.ExecuteNonQuery() == 0) continue;// name taken
                            }
                            account = name;
                            break;
                        }
                        if (account == null) { Log(email, ip, "GOOGLE FAIL: no free account name"); code = 5; msg = "Server error, try again later."; return; }
                        using (var cmd = new MySqlConnector.MySqlCommand(
                            "INSERT INTO `mobile_google_links` (`Sub`, `Username`, `Email`, `Created`) VALUES (@s, @u, @e, @t)", conn))
                        {
                            cmd.Parameters.AddWithValue("@s", sub);
                            cmd.Parameters.AddWithValue("@u", account);
                            cmd.Parameters.AddWithValue("@e", email.Length > 255 ? email.Substring(0, 255) : email);
                            cmd.Parameters.AddWithValue("@t", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                            cmd.ExecuteNonQuery();
                        }
                        lock (day) day.Created++;
                        Log(account, ip, "REGISTER OK (mobile, Google)");
                        code = 0; msg = "OK";
                    }
                }
            }
            catch (Exception e)
            {
                account = null; code = 5; msg = "Server error, try again later.";
                Log(email, ip, "GOOGLE FAIL: " + e.Message);
            }
        }

        /// <summary>Account names for an e-mail: the local part (A-Z a-z 0-9 _ only, 4..16), then with random digits.</summary>
        static IEnumerable<string> Candidates(string email)
        {
            string local = email.IndexOf('@') > 0 ? email.Substring(0, email.IndexOf('@')) : email;
            var sb = new StringBuilder();
            foreach (char c in local)
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_') sb.Append(c);
            string b = sb.ToString();
            if (b.Length == 0) b = "player";
            if (b.Length > 16) b = b.Substring(0, 16);
            if (b.Length >= 4) yield return b;
            string stem = b.Length > 10 ? b.Substring(0, 10) : b;
            for (int i = 0; i < 12; i++)
                yield return stem + "_" + RandomNumberGenerator.GetInt32(i < 6 ? 1000 : 100000, i < 6 ? 10000 : 1000000);
        }

        static string RandomSecret()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        }

        // ------------------------------------------------------------------------------------- one-time token
        static string NewToken(string account)
        {
            Sweep();
            string t = TokenPrefix + Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            Tokens[t] = new Pending { Account = account, Expires = DateTime.UtcNow + TokenLife };
            return t;
        }

        /// <summary>True when the 1799 session string has the Google token form (it is then never a password).</summary>
        public static bool IsToken(string session)
        {
            return session != null && session.StartsWith(TokenPrefix, StringComparison.Ordinal);
        }

        /// <summary>Uses up a token from 1799. account = the name the client sent in 1797. Removed even when it does not match.</summary>
        public static bool Redeem(string session, string account, out string reason)
        {
            reason = null;
            if (!Enabled) { reason = "Google login is off"; return false; }
            if (!TokenRx.IsMatch(session ?? "")) { reason = "malformed Google token"; return false; }
            Pending p;
            if (!Tokens.TryRemove(session, out p)) { reason = "unknown or used Google token"; return false; }
            if (DateTime.UtcNow > p.Expires) { reason = "Google token expired"; return false; }
            if (!string.Equals(p.Account, (account ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) { reason = "Google token is for another account"; return false; }
            return true;
        }

        static void Sweep()
        {
            DateTime now = DateTime.UtcNow;
            foreach (var kv in Tokens)
                if (kv.Value.Expires < now) Tokens.TryRemove(kv.Key, out _);
        }

        // ------------------------------------------------------------------------------------------- helpers
        static byte[] B64Url(string s)
        {
            s = (s ?? "").Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
            return Convert.FromBase64String(s);
        }

        static string Str(JsonElement e, string name)
        {
            JsonElement v;
            return e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        }

        static long Num(JsonElement e, string name)
        {
            JsonElement v;
            long n;
            if (!e.TryGetProperty(name, out v)) return 0;
            if (v.ValueKind == JsonValueKind.Number && v.TryGetInt64(out n)) return n;
            if (v.ValueKind == JsonValueKind.String && long.TryParse(v.GetString(), out n)) return n;
            return 0;
        }

        static bool IsTrue(JsonElement e, string name)
        {
            JsonElement v;
            if (!e.TryGetProperty(name, out v)) return false;
            return v.ValueKind == JsonValueKind.True || (v.ValueKind == JsonValueKind.String && v.GetString() == "true");
        }

        static string Esc(string s)
        {
            return (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        static string Json(int code, string msg)
        {
            return "{\"code\":" + code + ",\"msg\":\"" + Esc(msg) + "\"}";
        }

        static void Log(string user, string ip, string result)
        {
            Program.AuthLog(user, ip, result);
        }
    }
}
