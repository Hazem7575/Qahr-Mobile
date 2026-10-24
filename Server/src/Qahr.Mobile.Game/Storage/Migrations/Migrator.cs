using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
// [net10] MySql.Data 6.7 is .NET Framework only; the .NET 10 build uses MySqlConnector (same as DataHolder).
using MYSQLCONNECTION = MySqlConnector.MySqlConnection;

namespace Qahr.Mobile.Database.Migrations
{
    // ============================================================================
    //  Laravel-style SQL migrations for the MySQL backend.
    //
    //  Goal: you never hand-build the schema again. If the whole database is gone,
    //  the first boot (as long as the MySQL CONNECTION works) will:
    //     1) CREATE DATABASE `<name>`           (EnsureDatabase)
    //     2) CREATE the core tables             (the existing *Storage.EnsureSchema, runs before us)
    //     3) run every *.sql migration in order (Run) and remember which ran
    //  Any later change = drop a new numbered file in Persistence\Database\Migrations\
    //  (e.g. 0002_add_column.sql). It runs once, on the next boot, and is recorded in
    //  `schema_migrations`. Nothing manual, nothing re-gathered.
    //
    //  Migration files must be idempotent-friendly DDL (use IF NOT EXISTS where you can);
    //  a failed migration is NOT recorded, so it retries on the next boot.
    // ============================================================================
    public static class Migrator
    {
        private static bool IsMySql
        {
            get { return Qahr.Mobile.Database.Storage.PlayerStorage.Backend != Qahr.Mobile.Database.Storage.PlayerStorage.Mode.File; }
        }

        /// <summary>Step 1 - create the database itself if it is missing (connection must be OK). Runs before EnsureSchema.</summary>
        public static void EnsureDatabase()
        {
            if (!IsMySql) return;
            try
            {
                string cs = Qahr.Mobile.DBFunctionality.DataHolder.ConnectionString ?? "";
                string db = ConnValue(cs, "Database");
                if (string.IsNullOrEmpty(db)) { Warn("no Database in the connection string - skipping CREATE DATABASE."); return; }

                string serverCs = ConnRemove(cs, "Database");          // connect at server level (no default schema)
                using (var conn = new MYSQLCONNECTION())
                {
                    conn.ConnectionString = serverCs;
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `" + db.Replace("`", "") + "` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                        cmd.ExecuteNonQuery();
                    }
                }
                Info("database `" + db + "` is ready.");
            }
            catch (Exception e)
            {
                // Connection genuinely down: the normal backend will fall back to files (existing behaviour).
                Warn("could not ensure database (" + e.Message + ") - continuing; backend may fall back to files.");
            }
        }

        /// <summary>Step 3 - apply every migration file that has not run yet, in filename order.</summary>
        public static void Run()
        {
            if (!IsMySql) return;
            try
            {
                string dir = FindMigrationsDir();
                using (var conn = new MYSQLCONNECTION())
                {
                    conn.ConnectionString = Qahr.Mobile.DBFunctionality.DataHolder.ConnectionString;
                    conn.Open();
                    EnsureTrackingTable(conn);
                    HashSet<string> applied = LoadApplied(conn);

                    string[] files = (dir == null) ? new string[0] : Directory.GetFiles(dir, "*.sql");
                    Array.Sort(files, StringComparer.OrdinalIgnoreCase);

                    int ran = 0;
                    foreach (string f in files)
                    {
                        string name = Path.GetFileNameWithoutExtension(f);
                        if (applied.Contains(name)) continue;
                        ApplyOne(conn, name, File.ReadAllText(f));
                        ran++;
                        Info("applied " + name);
                    }

                    if (dir == null) Warn("no Migrations folder found next to the exe or in the source tree - only the tracking table was ensured.");
                    Info((applied.Count) + " already applied, " + ran + " new, " + files.Length + " file(s) total.");
                }
            }
            catch (Exception e)
            {
                Warn("FAILED: " + e.Message);
                MyConsole.SaveException(e);
            }
        }

        // ------------------------------------------------------------------ internals
        private static void EnsureTrackingTable(MYSQLCONNECTION conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS `schema_migrations` (" +
                    "`name` VARCHAR(190) NOT NULL, " +
                    "`applied_at` DATETIME NOT NULL, " +
                    "PRIMARY KEY (`name`)" +
                    ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";
                cmd.ExecuteNonQuery();
            }
        }

        private static HashSet<string> LoadApplied(MYSQLCONNECTION conn)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT `name` FROM `schema_migrations`;";
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) set.Add(r.GetString(0));
            }
            return set;
        }

        private static void ApplyOne(MYSQLCONNECTION conn, string name, string sql)
        {
            foreach (string stmt in SplitStatements(sql))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = stmt;
                    cmd.ExecuteNonQuery();
                }
            }
            // Record only after every statement succeeded; a failure above throws and the row is never written,
            // so the migration simply retries on the next boot.
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO `schema_migrations` (`name`,`applied_at`) VALUES (@n, NOW());";
                cmd.Parameters.AddWithValue("@n", name);
                cmd.ExecuteNonQuery();
            }
        }

        // Splits a .sql file into statements on ';', dropping -- and # line comments. Good enough for DDL/DML
        // migrations (no stored-procedure bodies / no ';' inside string literals).
        private static IEnumerable<string> SplitStatements(string sql)
        {
            var clean = new StringBuilder();
            foreach (string raw in (sql ?? "").Replace("\r", "").Split('\n'))
            {
                string t = raw.TrimStart();
                if (t.StartsWith("--") || t.StartsWith("#")) continue;
                clean.Append(raw).Append('\n');
            }
            foreach (string part in clean.ToString().Split(';'))
            {
                string s = part.Trim();
                if (s.Length > 0) yield return s;
            }
        }

        private static string FindMigrationsDir()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 12 && !string.IsNullOrEmpty(dir); i++)
                {
                    string[] candidates =
                    {
                        Path.Combine(dir, "Migrations"),
                        Path.Combine(dir, "Database\\Migrations"),
                        Path.Combine(dir, "Persistence\\Database\\Migrations"),
                    };
                    foreach (string c in candidates)
                        if (Directory.Exists(c)) return c;
                    DirectoryInfo parent = Directory.GetParent(dir);
                    dir = parent != null ? parent.FullName : null;
                }
            }
            catch { }
            return null;
        }

        // ------------------------------------------------------------------ connection-string helpers
        private static string ConnValue(string cs, string key)
        {
            foreach (string part in (cs ?? "").Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq <= 0) continue;
                if (part.Substring(0, eq).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    return part.Substring(eq + 1).Trim();
            }
            return null;
        }

        private static string ConnRemove(string cs, string key)
        {
            var sb = new StringBuilder();
            foreach (string part in (cs ?? "").Split(';'))
            {
                if (part.Trim().Length == 0) continue;
                int eq = part.IndexOf('=');
                if (eq > 0 && part.Substring(0, eq).Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) continue;
                sb.Append(part.Trim()).Append(';');
            }
            return sb.ToString();
        }

        private static void Info(string m) { try { MyConsole.WriteLine("[Migrate] " + m, ConsoleColor.Cyan); } catch { } }
        private static void Warn(string m) { try { MyConsole.WriteLine("[Migrate] " + m, ConsoleColor.Yellow); } catch { } }
    }
}
