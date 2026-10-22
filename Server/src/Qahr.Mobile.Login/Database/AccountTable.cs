// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Qahr.Mobile.Login.Database
{
    /// <summary>
    /// [Security flaw 4] PBKDF2 password hashing with backward-compatible plaintext fallback.
    /// Stored format: "$P1$&lt;iterations&gt;$&lt;saltBase64&gt;$&lt;hashBase64&gt;".
    /// Legacy rows still contain plaintext; Verify() accepts them and the login path re-hashes
    /// them transparently on the next successful login, so existing players are never locked out.
    /// TODO: once all live accounts have been migrated (no plaintext rows remain), drop the
    /// plaintext fallback in Verify() so a DB leak cannot expose any usable credential.
    /// </summary>
    public static class PasswordHash
    {
        private const string Prefix = "$P1$";
        private const int Iterations = 20000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static bool IsHashed(string stored)
        {
            return stored != null && stored.StartsWith(Prefix);
        }

        public static string Hash(string password)
        {
            if (password == null) password = "";
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                hash = pbkdf2.GetBytes(HashSize);
            return Prefix + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string stored)
        {
            if (stored == null) return false;
            if (!IsHashed(stored))
            {
                // Legacy plaintext comparison (constant-time-ish). Kept for migration only.
                return FixedTimeEquals(Encoding.UTF8.GetBytes(password ?? ""), Encoding.UTF8.GetBytes(stored));
            }
            try
            {
                string[] parts = stored.Split('$'); // ["", "P1", iter, salt, hash]
                if (parts.Length != 5) return false;
                int iterations = int.Parse(parts[2]);
                byte[] salt = Convert.FromBase64String(parts[3]);
                byte[] expected = Convert.FromBase64String(parts[4]);
                byte[] actual;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password ?? "", salt, iterations))
                    actual = pbkdf2.GetBytes(expected.Length);
                return FixedTimeEquals(expected, actual);
            }
            catch
            {
                return false;
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }

    public unsafe class AccountTable
    {
        public enum AccountState : byte
        {
            NotActivated = 100,
            ProjectManager = 255,
            GameHelper = 5,
            GameMaster = 3,
            Player = 2,
            Banned = 1,
            DoesntExist = 0
        }
        public string Username;
        public string Password;
        public string IP;
        public AccountState State;
        public uint EntityID;
        public bool exists = false;
        public bool Banned;

        // [sqli] All `accounts` access below is parameterized (was the string-building Qahr.Mobile.Login.Database.MySqlCommand
        // builder: Where() escaped by regex, Set(string) did not escape at all). `column` is always a constant here.
        private static MySqlConnector.MySqlCommand NewCmd(string sql, MySqlConnector.MySqlConnection conn) { return new MySqlConnector.MySqlCommand(sql, conn); }
        // Same semantics as MySqlReader.ReadString: NULL -> "", missing column -> "" + console note.
        private static string Col(System.Data.IDataRecord r, string column)
        {
            try { return Convert.ToString(r[column]); }
            catch { Console.WriteLine("[MySql-Reader] No such field named " + column + " in the row.", ConsoleColor.DarkRed); return ""; }
        }
        private static bool AccountExists(string username)
        {
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = NewCmd("SELECT * FROM `accounts` WHERE `Username` = @username", conn))
            {
                cmd.Parameters.AddWithValue("@username", username ?? "");
                conn.Open();
                using (var r = cmd.ExecuteReader())
                    return r.Read();
            }
        }
        private static int UpdateByUsername(string column, object value, string username)
        {
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = NewCmd("UPDATE `accounts` SET `" + column + "` = @val WHERE `Username` = @username", conn))
            {
                cmd.Parameters.AddWithValue("@val", value ?? "");
                cmd.Parameters.AddWithValue("@username", username ?? "");
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public AccountTable(string username)
        {
            if (username == null) return;
            Username = username;
            Password = "";
            IP = "";
            State = AccountState.DoesntExist;
            EntityID = 0;
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = NewCmd("SELECT * FROM `accounts` WHERE `Username` = @username", conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exists = true;
                        Password = Col(reader, "Password");
                        IP = Col(reader, "IP");
                        uint eid; uint.TryParse(Col(reader, "EntityID"), out eid);
                        EntityID = eid;
                        int st; int.TryParse(Col(reader, "State"), out st);
                        State = (AccountState)st;
                        if (State == AccountState.Banned)
                        {
                            Banned = true;
                        }
                    }
                }
            }
        }
        public static void UpdateEarth(string Account, string str)
        {
            if (AccountExists(Account))
                UpdateByUsername("EarthID", str, Account);
        }
        public static void UpdateIP(string Account, string str)
        {
            if (AccountExists(Account))
                UpdateByUsername("IP", str, Account);
        }
        public static void Banned12(string Account,bool unbanned = false)
        {
            if (AccountExists(Account))
            {
                if (unbanned)
                {
                    // [Security flaw 16] Unban must restore the account to Player (2),
                    // not DoesntExist (0).
                    UpdateByUsername("State", 2L, Account);
                }
                else
                {
                    UpdateByUsername("State", 1L, Account);
                }
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("This Account DoesntExist");
            }
        }
        public void Save()
        {
            UpdateByUsername("EntityID", EntityID, Username);
        }
        // [Security flaw 4] Persist a migrated (hashed) password.
        public static void UpdatePassword(string Account, string hashedPassword)
        {
            UpdateByUsername("Password", hashedPassword ?? "", Account);
        }

        // [Security flaw 4] One-time bulk migration: hash every account whose stored password is still
        // plaintext (not "$P1$..."). Idempotent — hashed rows are skipped, so it is safe to run every boot.
        // Returns how many accounts were migrated. Uses the exact same PBKDF2 format as login/registration.
        public static int MigrateAllPlaintext()
        {
            var toMigrate = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>();
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = NewCmd("SELECT `Username`, `Password` FROM `accounts`", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        string u = Col(reader, "Username");
                        string p = Col(reader, "Password");
                        if (!string.IsNullOrEmpty(u) && !PasswordHash.IsHashed(p))
                            toMigrate.Add(new System.Collections.Generic.KeyValuePair<string, string>(u, p));
                    }
            }
            foreach (var kv in toMigrate)
            {
                try { UpdatePassword(kv.Key, PasswordHash.Hash(kv.Value)); }
                catch (Exception e) { Console.WriteLine("[Security] migrate " + kv.Key + " failed: " + e.Message); }
            }
            return toMigrate.Count;
        }
    }
}