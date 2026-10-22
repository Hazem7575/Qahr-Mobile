using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Qahr.Mobile.Login.Network.AuthPackets
{
    /// <summary>
    /// QahrMobile: hashed mobile login proof (F5, design in conquer-mobile\docs\LOGIN_HARDENING.md).
    /// The mobile client cannot see a server nonce (CMsgSDKLoginChallenge::Process ignores the 1798 body and the
    /// session string is fixed before 1797), so the 1799 session carries a time-based proof instead of the password:
    ///   "H2:" unixSeconds ":" cnonce(16..64 hex) ":" hex(HMAC-SHA256(V, "QahrMobileLogin2|" account "|" server "|" time "|" cnonce))
    ///   V = PBKDF2-HMAC-SHA1(password UTF-8, salt = UTF-8("QahrMobile|" + lower(account)), 20000, 32 bytes)
    /// V is kept per account in `mobile_verifiers` (with the accounts.Password value it was made from, so a password
    /// change makes it stale). It is written on every successful plaintext login while the mode is on.
    /// Config: ServerConfig.ini [Security] MobileLoginProof = 0 off (default, nothing changes),
    ///         1 = accept the proof or the plaintext password, 2 = the proof is required for mobile logins.
    /// </summary>
    public static class MobileLoginProof
    {
        public const string Prefix = "H2:";
        public const int Iterations = 20000;
        public const int MaxClockSkewSeconds = 600;
        public static int Mode;

        // (account|cnonce) -> expiry, so a captured proof cannot be replayed inside the time window
        private static readonly ConcurrentDictionary<string, DateTime> Used = new ConcurrentDictionary<string, DateTime>();
        private static DateTime nextSweep = DateTime.MinValue;

        public static void Load()
        {
            Mode = SharedConfig.Int("Security", "MobileLoginProof", 0);
            if (Mode < 0 || Mode > 2) Mode = 0;
            if (Mode == 0) return;
            try
            {
                using (var conn = Database.DataHolder.MySqlConnection)
                using (var cmd = new MySqlConnector.MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS `mobile_verifiers` (`Username` VARCHAR(64) NOT NULL PRIMARY KEY, " +
                    "`Verifier` VARCHAR(64) NOT NULL, `PwHash` VARCHAR(255) NOT NULL, `UpdatedAt` BIGINT NOT NULL)", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("[Security] mobile login proof mode " + Mode + (Mode == 2 ? " (required)" : " (optional)"));
            }
            catch (Exception e)
            {
                Console.WriteLine("[Security] mobile_verifiers table failed, mobile login proof off: " + e.Message);
                Mode = 0;
            }
        }

        public static bool IsProof(string session)
        {
            return session != null && session.StartsWith(Prefix, StringComparison.Ordinal);
        }

        /// <summary>The raw 1799 session string (up to 200 ASCII chars), or null.</summary>
        public static string ReadSession(byte[] packet)
        {
            if (packet == null || packet.Length < 5) return null;
            int end = 4, max = Math.Min(packet.Length, 4 + 200);
            while (end < max && packet[end] != 0) end++;
            string s = Encoding.ASCII.GetString(packet, 4, end - 4).Trim();
            return s.Length > 0 ? s : null;
        }

        public static byte[] Verifier(string account, string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes("QahrMobile|" + (account ?? "").Trim().ToLowerInvariant());
            using (var kdf = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password ?? ""), salt, Iterations, HashAlgorithmName.SHA1))
                return kdf.GetBytes(32);
        }

        /// <summary>Checks an "H2:" session for this account/server. storedHash = accounts.Password now.</summary>
        public static bool Verify(string account, string server, string session, string storedHash, out string reason)
        {
            reason = "bad format";
            string[] p = session.Substring(Prefix.Length).Split(':');
            if (p.Length != 3) return false;
            long time;
            if (!long.TryParse(p[0], out time)) return false;
            string cnonce = p[1].ToLowerInvariant();
            if (cnonce.Length < 16 || cnonce.Length > 64 || !IsHex(cnonce) || p[2].Length != 64 || !IsHex(p[2])) return false;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (Math.Abs(now - time) > MaxClockSkewSeconds) { reason = "proof time out of window (clock " + (time - now) + " s)"; return false; }

            string acc = (account ?? "").Trim().ToLowerInvariant();
            string row = LoadVerifier(acc, storedHash);
            if (row == null) { reason = "no current verifier (log in once with the password while MobileLoginProof>=1)"; return false; }

            byte[] v = Convert.FromHexString(row);
            byte[] msg = Encoding.UTF8.GetBytes("QahrMobileLogin2|" + acc + "|" + (server ?? "") + "|" + p[0] + "|" + cnonce);
            byte[] mac;
            using (var h = new HMACSHA256(v)) mac = h.ComputeHash(msg);
            if (!CryptographicOperations.FixedTimeEquals(mac, Convert.FromHexString(p[2]))) { reason = "wrong proof"; return false; }

            Sweep();
            if (!Used.TryAdd(acc + "|" + cnonce, DateTime.UtcNow.AddSeconds(2 * MaxClockSkewSeconds + 60))) { reason = "proof replayed"; return false; }
            reason = null;
            return true;
        }

        /// <summary>After a successful plaintext login: keep V up to date for this account (only when the mode is on).</summary>
        public static void Remember(string account, string password, string storedHash)
        {
            if (Mode == 0 || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash)) return;
            try
            {
                string acc = account.Trim().ToLowerInvariant();
                if (LoadVerifier(acc, storedHash) != null) return;// already current
                string v = Convert.ToHexString(Verifier(acc, password));
                using (var conn = Database.DataHolder.MySqlConnection)
                using (var cmd = new MySqlConnector.MySqlCommand(
                    "REPLACE INTO `mobile_verifiers` (`Username`, `Verifier`, `PwHash`, `UpdatedAt`) VALUES (@u, @v, @h, @t)", conn))
                {
                    cmd.Parameters.AddWithValue("@u", acc);
                    cmd.Parameters.AddWithValue("@v", v);
                    cmd.Parameters.AddWithValue("@h", storedHash);
                    cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.Ticks);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine("[Security] mobile verifier not saved: " + e.Message); }
        }

        /// <summary>The verifier hex of the account when it was made from the current accounts.Password, else null.</summary>
        private static string LoadVerifier(string acc, string storedHash)
        {
            try
            {
                using (var conn = Database.DataHolder.MySqlConnection)
                using (var cmd = new MySqlConnector.MySqlCommand("SELECT `Verifier`, `PwHash` FROM `mobile_verifiers` WHERE `Username` = @u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", acc);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;
                        string v = Convert.ToString(r["Verifier"]), h = Convert.ToString(r["PwHash"]);
                        if (h != (storedHash ?? "") || v.Length != 64 || !IsHex(v)) return null;
                        return v;
                    }
                }
            }
            catch { return null; }
        }

        private static void Sweep()
        {
            DateTime now = DateTime.UtcNow;
            if (now < nextSweep) return;
            nextSweep = now.AddMinutes(1);
            foreach (var kv in Used)
                if (kv.Value < now) { DateTime x; Used.TryRemove(kv.Key, out x); }
        }

        private static bool IsHex(string s)
        {
            foreach (char c in s)
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) return false;
            return true;
        }
    }
}
