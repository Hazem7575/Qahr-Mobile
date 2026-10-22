using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: "Forgot password?" of the mobile login page (QahrLogin.java), on the MobileRegister HTTP port.
    ///
    ///   POST /qahr/forgot   who=<account or e-mail>
    ///     -> {"code":0,"msg":"A code was sent to m***@gmail.com.","account":"name"}   (6 digits, valid 15 min)
    ///        an e-mail with several accounts gets their names by mail instead -> code 9
    ///   POST /qahr/reset    account=..&amp;code=..&amp;password=..
    ///     -> {"code":0,"msg":"OK"}   new PBKDF2 password; the client then logs in with it
    ///
    /// Codes: 0 ok, 2 bad password, 4 too many requests, 5 server error, 6 bad request, 7 mail not set up,
    /// 8 no such account / no e-mail on it, 9 names sent by mail, 10 wrong or expired code.
    ///
    /// The mail goes out through SMTP, set in ServerConfig.ini (empty Host = recovery off):
    ///   [Mail] Host=smtp.gmail.com  Port=587  User=you@gmail.com  Password=(Gmail app password)  From=  FromName=Qahr Online
    /// Codes live in memory only (an AccServer restart drops them; the player asks for a new one).
    /// </summary>
    public static class MobileRecovery
    {
        const int CodeMinutes = 15, MaxTries = 5, MaxCodesPerDay = 5;
        static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(60);

        class Pending { public string Code; public DateTime Expires; public int Tries; public DateTime LastSent; public DateTime DayStart; public int SentToday; }
        static readonly ConcurrentDictionary<string, Pending> Codes = new ConcurrentDictionary<string, Pending>(StringComparer.OrdinalIgnoreCase);

        public static bool MailReady => !string.IsNullOrEmpty(SharedConfig.String("Mail", "Host", ""));

        public static string Forgot(string ip, Dictionary<string, string> form)
        {
            string who = (Get(form, "who") ?? "").Trim();
            if (who.Length == 0 || who.Length > 128) return Json(6, "Enter your account name or e-mail.");
            if (!MailReady) return Json(7, "Password recovery is not set up on this server yet.");
            try
            {
                var found = Find(who);
                if (found.Count == 0)
                {
                    MobileRegister.Log(who, ip, "FORGOT FAIL: no account");
                    return Json(8, who.Contains("@") ? "No account uses this e-mail." : "This account does not exist.");
                }
                if (found.Count > 1)
                {
                    var names = new StringBuilder();
                    foreach (var f in found) names.Append("  - ").Append(f.Key).Append("\r\n");
                    Send(found[0].Value, "Qahr Online - your accounts",
                        "Your e-mail is linked to these accounts:\r\n" + names +
                        "\r\nIn the game tap \"Forgot password?\" and enter the account name to reset its password.\r\n\r\n" +
                        "بريدك مربوط بالحسابات دي:\r\n" + names +
                        "\r\nفي اللعبة دوس \"Forgot password?\" واكتب اسم الحساب علشان تغيّر كلمة السر بتاعته.\r\n");
                    MobileRegister.Log(who, ip, "FORGOT: account names mailed (" + found.Count + ")");
                    return Json(9, "Your account names were sent to your e-mail. Enter the account name to reset its password.");
                }
                string account = found[0].Key, email = found[0].Value;
                if (string.IsNullOrEmpty(email))
                {
                    MobileRegister.Log(account, ip, "FORGOT FAIL: no e-mail on the account");
                    return Json(8, "This account has no e-mail. Contact the server staff.");
                }
                var now = DateTime.UtcNow;
                var p = Codes.GetOrAdd(account, x => new Pending { DayStart = now });
                string code;
                lock (p)
                {
                    if (now - p.DayStart >= TimeSpan.FromDays(1)) { p.DayStart = now; p.SentToday = 0; }
                    if (now - p.LastSent < Cooldown) return Json(4, "Wait a minute before asking for a new code.");
                    if (p.SentToday >= MaxCodesPerDay) return Json(4, "Too many codes today, try again tomorrow.");
                    code = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
                    p.Code = code; p.Expires = now.AddMinutes(CodeMinutes); p.Tries = 0; p.LastSent = now; p.SentToday++;
                }
                Send(email, "Qahr Online - password reset code " + code,
                    "Your code to reset the password of \"" + account + "\": " + code + "\r\n" +
                    "It works for " + CodeMinutes + " minutes. If you did not ask for it, ignore this e-mail.\r\n\r\n" +
                    "كود تغيير كلمة السر للحساب \"" + account + "\": " + code + "\r\n" +
                    "الكود شغال " + CodeMinutes + " دقيقة. لو انت مطلبتهوش، تجاهل الرسالة دي.\r\n");
                MobileRegister.Log(account, ip, "FORGOT: code mailed");
                return "{\"code\":0,\"msg\":" + Str("A code was sent to " + Mask(email) + ".") + ",\"account\":" + Str(account) + "}";
            }
            catch (Exception e)
            {
                MobileRegister.Log(who, ip, "FORGOT FAIL: " + e.Message);
                return Json(5, "Could not send the e-mail, try again later.");
            }
        }

        public static string Reset(string ip, Dictionary<string, string> form)
        {
            string account = (Get(form, "account") ?? "").Trim(), code = (Get(form, "code") ?? "").Trim(), password = Get(form, "password");
            if (account.Length == 0 || code.Length == 0) return Json(6, "Enter the code from the e-mail.");
            if (!MobileRegister.ValidPassword(password)) return Json(2, "Password: 6-32 characters (letters, digits, symbols).");
            Pending p;
            if (!Codes.TryGetValue(account, out p)) return Json(10, "Wrong or expired code, ask for a new one.");
            lock (p)
            {
                if (p.Code == null || DateTime.UtcNow > p.Expires || p.Tries >= MaxTries)
                {
                    p.Code = null;
                    return Json(10, "Wrong or expired code, ask for a new one.");
                }
                if (!CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(code.PadRight(6)), Encoding.ASCII.GetBytes(p.Code)))
                {
                    p.Tries++;
                    MobileRegister.Log(account, ip, "RESET FAIL: wrong code");
                    return Json(10, "Wrong code (" + (MaxTries - p.Tries) + " tries left).");
                }
                p.Code = null;// one use
            }
            try
            {
                string hash = Database.PasswordHash.Hash(password);
                using (var conn = Database.DataHolder.MySqlConnection)
                using (var cmd = new MySqlConnector.MySqlCommand("UPDATE `accounts` SET `Password`=@p WHERE `Username`=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@p", hash);
                    cmd.Parameters.AddWithValue("@u", account);
                    conn.Open();
                    if (cmd.ExecuteNonQuery() == 0) return Json(8, "This account does not exist.");
                }
                Network.AuthPackets.MobileLoginProof.Remember(account, password, hash);
                MobileRegister.Log(account, ip, "RESET OK (mobile)");
                return Json(0, "OK");
            }
            catch (Exception e)
            {
                MobileRegister.Log(account, ip, "RESET FAIL: " + e.Message);
                return Json(5, "Server error, try again later.");
            }
        }

        /// <summary>account name -> its e-mail; or e-mail -> every account that uses it</summary>
        static List<KeyValuePair<string, string>> Find(string who)
        {
            var list = new List<KeyValuePair<string, string>>();
            bool email = who.Contains("@");
            using (var conn = Database.DataHolder.MySqlConnection)
            using (var cmd = new MySqlConnector.MySqlCommand(email
                ? "SELECT `Username`, `Email` FROM `accounts` WHERE `Email`=@w LIMIT 20"
                : "SELECT `Username`, `Email` FROM `accounts` WHERE `Username`=@w LIMIT 1", conn))
            {
                cmd.Parameters.AddWithValue("@w", who);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new KeyValuePair<string, string>(r.GetString(0), r.IsDBNull(1) ? "" : r.GetString(1)));
            }
            return list;
        }

        static void Send(string to, string subject, string body)
        {
            string host = SharedConfig.String("Mail", "Host", ""), user = SharedConfig.String("Mail", "User", "");
            string from = SharedConfig.String("Mail", "From", user), name = SharedConfig.String("Mail", "FromName", "Qahr Online");
            using (var msg = new MailMessage(new MailAddress(from, name), new MailAddress(to)))
            using (var smtp = new SmtpClient(host, SharedConfig.Int("Mail", "Port", 587)))
            {
                msg.Subject = subject;
                msg.Body = body;
                msg.SubjectEncoding = msg.BodyEncoding = Encoding.UTF8;
                smtp.EnableSsl = SharedConfig.Int("Mail", "Ssl", 1) != 0;
                smtp.Timeout = 15000;
                if (user.Length > 0) smtp.Credentials = new NetworkCredential(user, SharedConfig.String("Mail", "Password", ""));
                smtp.Send(msg);
            }
        }

        /// <summary>mohamed@gmail.com -> m*****d@gmail.com</summary>
        static string Mask(string email)
        {
            int at = email.IndexOf('@');
            if (at <= 1) return "***" + email.Substring(Math.Max(0, at));
            return email[0] + new string('*', Math.Min(5, at - 2)) + email[at - 1] + email.Substring(at);
        }

        static string Get(Dictionary<string, string> form, string key)
        {
            string v;
            return form.TryGetValue(key, out v) ? v : null;
        }

        static string Json(int code, string msg) => "{\"code\":" + code + ",\"msg\":" + Str(msg) + "}";

        static string Str(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }
}
