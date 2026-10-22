// ============================================================================
//  SocialStorage - مرآة MySQL (وضع Dual) للجيلد + اليونيون + الكلان.
//
//  المبدأ (نفس PlayerStorage / PlayerBinaryStorage):
//    * الملفات (Guilds\<id>.txt, Unions\<id>.ini, Clans\<id>.txt) تفضل مصدر الحقيقة.
//    * صفر تغيير في منطق اللعبة: كل مرآة بتتنادى بسطر واحد بعد كتابة الملف مباشرة،
//      وبتعيد قراءة نفس الملف اللي لسه اتكتب وتحلّله بنفس طريقة دالة Load المقابلة
//      (نفس ReadLine('/') / نفس IniFile)، فالقيم في القاعدة = اللي اللعبة هتحمّله بالظبط.
//      (وكمان مفيش أي لمس للقوائم في الذاكرة -> مفيش خطر List غير thread-safe.)
//    * أي فشل DB لا يكسر الحفظ أبدًا: كله جوّه try/catch (تحذير أصفر بس).
//    * كل شيء يعمل فقط لو PlayerStorage.Backend != File و schema جاهز (_ready).
//
//  الجداول (CREATE TABLE IF NOT EXISTS في EnsureSchema، بدون FOREIGN KEYs لأن الملفات
//  ممكن تحتوي على مراجع يتيمة، وبدون AUTO_INCREMENT لأن اللعبة هي اللي بتوزّع الـ IDs):
//    guilds            PK(GuildID)
//    guild_allies      PK(GuildID, AllyGuildID)
//    guild_enemies     PK(GuildID, EnemyGuildID)
//    guild_messages    PK(GuildID, Seq)
//    guild_constructs  PK(GuildID, ConstructType, BeastIndex)
//    unions            PK(UnionID)
//    clans             PK(ClanID)
//    clan_relations    PK(ClanID, RelationType, OtherClanID)   RelationType: 1=Ally, 2=Enemy
//  مفيش جداول أعضاء: الأعضاء من جدول players (GuildID/GuildRank/ClanID/ClanRank/UnionUID/...)،
//  ومعاهم VIEWs للتقارير بس.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySqlConnector; // [net10] .NET 10 build (Qahr.Mobile.Net10.csproj)
using Qahr.Mobile.DBFunctionality;
using Qahr.Mobile.Database.DBActions;

namespace Qahr.Mobile.Database.Storage
{
    public static class SocialStorage
    {
        private static bool _ready;

        private const int MaxRowsPerInsert = 200;

        private static bool Enabled
        {
            get { return PlayerStorage.Backend != PlayerStorage.Mode.File && _ready; }
        }

        // =====================================================================
        //  Schema
        // =====================================================================
        public static void EnsureSchema()
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File) return;
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    foreach (string sql in CreateStatements())
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                _ready = true;
                try { MyConsole.WriteLine("[Storage] Social mirror tables ready (guilds/unions/clans, 8 tables).", ConsoleColor.Cyan); } catch { }
            }
            catch (Exception e)
            {
                _ready = false;
                try { MyConsole.WriteLine("[Storage] SocialStorage.EnsureSchema failed (mirror disabled, files unaffected): " + e.Message, ConsoleColor.Yellow); } catch { }
                return;
            }

            // VIEWs اختيارية للتقارير: فشلها (صلاحيات مثلًا) ما يعطّلش المرآة.
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    foreach (string sql in ViewStatements())
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try { MyConsole.WriteLine("[Storage] SocialStorage views skipped (tables are fine): " + e.Message, ConsoleColor.Yellow); } catch { }
            }
        }

        private static string[] CreateStatements()
        {
            return new string[]
            {
                // ---- guilds (Guilds\<id>.txt: السطر 1 + السطور 4-8) ----
                "CREATE TABLE IF NOT EXISTS `guilds` (\n" +
                "  `GuildID` INT UNSIGNED NOT NULL,\n" +
                "  `GuildName` VARCHAR(64) NOT NULL DEFAULT '',\n" +
                "  `GuildLevel` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `LeaderName` VARCHAR(64) NOT NULL DEFAULT 'None',\n" +
                "  `SilverFund` BIGINT NOT NULL DEFAULT 0,\n" +
                "  `ConquerPointsFund` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CreatedDate` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'yyyymmdd',\n" +
                "  `Bulletin` VARCHAR(1024) NOT NULL DEFAULT 'None',\n" +
                "  `BulletinDate` INT NOT NULL DEFAULT 0 COMMENT 'yyyymmdd',\n" +
                "  `UnionID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `LeaderUID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Material` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Prestige` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `RecruitClassMask` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Guild.ClassFlag',\n" +
                "  `RecruitMinBattlePower` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `RecruitmentOn` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `RecruitmentOff` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `ArsenalBattlePower` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CommanderUID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CtfExploits` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CtfNextConquerPoints` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CtfNextMoney` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CtfRank` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `CtfRewardClaimed` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `IsDisbanded` TINYINT(1) NOT NULL DEFAULT 0,\n" +
                "  `DisbandedAt` DATETIME NULL DEFAULT NULL,\n" +
                "  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,\n" +
                "  PRIMARY KEY (`GuildID`),\n" +
                "  KEY `idx_name` (`GuildName`),\n" +
                "  KEY `idx_leader_uid` (`LeaderUID`),\n" +
                "  KEY `idx_leader_name` (`LeaderName`),\n" +
                "  KEY `idx_union` (`UnionID`),\n" +
                "  KEY `idx_disbanded` (`IsDisbanded`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- guild_allies (السطر 2) ----
                "CREATE TABLE IF NOT EXISTS `guild_allies` (\n" +
                "  `GuildID` INT UNSIGNED NOT NULL,\n" +
                "  `AllyGuildID` INT UNSIGNED NOT NULL,\n" +
                "  PRIMARY KEY (`GuildID`, `AllyGuildID`),\n" +
                "  KEY `idx_ally` (`AllyGuildID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- guild_enemies (السطر 3) ----
                "CREATE TABLE IF NOT EXISTS `guild_enemies` (\n" +
                "  `GuildID` INT UNSIGNED NOT NULL,\n" +
                "  `EnemyGuildID` INT UNSIGNED NOT NULL,\n" +
                "  PRIMARY KEY (`GuildID`, `EnemyGuildID`),\n" +
                "  KEY `idx_enemy` (`EnemyGuildID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- guild_messages (السطر 9) ----
                "CREATE TABLE IF NOT EXISTS `guild_messages` (\n" +
                "  `GuildID` INT UNSIGNED NOT NULL,\n" +
                "  `Seq` INT UNSIGNED NOT NULL COMMENT 'list order 0..n-1',\n" +
                "  `MessageText` VARCHAR(1024) NOT NULL DEFAULT '',\n" +
                "  `CreatedUnix` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'unix seconds UTC',\n" +
                "  PRIMARY KEY (`GuildID`, `Seq`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- guild_constructs (السطر 10) ----
                "CREATE TABLE IF NOT EXISTS `guild_constructs` (\n" +
                "  `GuildID` INT UNSIGNED NOT NULL,\n" +
                "  `ConstructType` INT UNSIGNED NOT NULL COMMENT 'Construct.ID',\n" +
                "  `BeastIndex` BIGINT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Construct.Beast; dict key = ID*100+Beast',\n" +
                "  `ConstructLevel` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Experience` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `ExtraValue2` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `ExtraValue3` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `ExtraValue4` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  PRIMARY KEY (`GuildID`, `ConstructType`, `BeastIndex`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- unions (Unions\<id>.ini [Info]) ----
                "CREATE TABLE IF NOT EXISTS `unions` (\n" +
                "  `UnionID` INT UNSIGNED NOT NULL COMMENT 'ini UID (= GuildID of founder guild)',\n" +
                "  `UnionName` VARCHAR(64) NOT NULL DEFAULT 'None',\n" +
                "  `EmperorName` VARCHAR(64) NOT NULL DEFAULT 'None',\n" +
                "  `EmperorUID` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'ini EmperrorUID',\n" +
                "  `GoldBricks` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'ini GoldBrick',\n" +
                "  `Treasury` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Stipend` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `IsKingdom` TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '1 = kingdom',\n" +
                "  `PlunderTargetUnionID` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'ini PlunderID',\n" +
                "  `InvadingUnionID` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'ini InvadingID',\n" +
                "  `Bulletin` VARCHAR(1024) NOT NULL DEFAULT 'None',\n" +
                "  `UnionTitle` VARCHAR(64) NOT NULL DEFAULT 'None' COMMENT 'ini Title',\n" +
                "  `PlunderTargetName` VARCHAR(64) NOT NULL DEFAULT 'None' COMMENT 'ini PlunderTarget',\n" +
                "  `InvadingUnionName` VARCHAR(64) NOT NULL DEFAULT 'None' COMMENT 'ini InvadingUnion',\n" +
                "  `RecruitDeclaration` VARCHAR(1024) NOT NULL DEFAULT 'None',\n" +
                "  `IsDisbanded` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'reserved: no union disband exists in code today',\n" +
                "  `DisbandedAt` DATETIME NULL DEFAULT NULL,\n" +
                "  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,\n" +
                "  PRIMARY KEY (`UnionID`),\n" +
                "  KEY `idx_name` (`UnionName`),\n" +
                "  KEY `idx_emperor_uid` (`EmperorUID`),\n" +
                "  KEY `idx_emperor_name` (`EmperorName`),\n" +
                "  KEY `idx_kingdom` (`IsKingdom`),\n" +
                "  KEY `idx_disbanded` (`IsDisbanded`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- clans (Clans\<id>.txt السطر 1) ----
                "CREATE TABLE IF NOT EXISTS `clans` (\n" +
                "  `ClanID` INT UNSIGNED NOT NULL,\n" +
                "  `ClanName` VARCHAR(64) NOT NULL DEFAULT 'None',\n" +
                "  `LeaderName` VARCHAR(64) NOT NULL DEFAULT 'None',\n" +
                "  `ClanLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Fund` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Clan.Donation',\n" +
                "  `Bulletin` VARCHAR(1024) NOT NULL DEFAULT 'None',\n" +
                "  `BattlePowerLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Clan.BP',\n" +
                "  `StoreLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Clan.LevelStore',\n" +
                "  `Reserved1` INT NOT NULL DEFAULT 0,\n" +
                "  `Reserved2` INT NOT NULL DEFAULT 0,\n" +
                "  `Reserved3` INT NOT NULL DEFAULT 0,\n" +
                "  `IsDisbanded` TINYINT(1) NOT NULL DEFAULT 0,\n" +
                "  `DisbandedAt` DATETIME NULL DEFAULT NULL,\n" +
                "  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,\n" +
                "  PRIMARY KEY (`ClanID`),\n" +
                "  KEY `idx_name` (`ClanName`),\n" +
                "  KEY `idx_leader_name` (`LeaderName`),\n" +
                "  KEY `idx_disbanded` (`IsDisbanded`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- clan_relations (السطر 2 = Ally، السطر 3 = Enemy) ----
                "CREATE TABLE IF NOT EXISTS `clan_relations` (\n" +
                "  `ClanID` INT UNSIGNED NOT NULL,\n" +
                "  `RelationType` TINYINT UNSIGNED NOT NULL COMMENT '1=Ally, 2=Enemy',\n" +
                "  `OtherClanID` INT UNSIGNED NOT NULL,\n" +
                "  PRIMARY KEY (`ClanID`, `RelationType`, `OtherClanID`),\n" +
                "  KEY `idx_other` (`OtherClanID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
            };
        }

        // VIEWs للتقارير فوق players (مفيش جداول أعضاء)
        private static string[] ViewStatements()
        {
            return new string[]
            {
                "CREATE OR REPLACE VIEW `guild_members_v` AS\n" +
                "SELECT p.`GuildID`, p.`UID`, p.`Name`, p.`GuildRank`, p.`Level`, p.`Class`\n" +
                "FROM `players` p WHERE p.`GuildID` <> 0;",

                "CREATE OR REPLACE VIEW `union_guilds_v` AS\n" +
                "SELECT g.`UnionID`, g.`GuildID`, g.`GuildName`, g.`LeaderUID`\n" +
                "FROM `guilds` g WHERE g.`UnionID` <> 0 AND g.`IsDisbanded` = 0;",

                "CREATE OR REPLACE VIEW `union_members_v` AS\n" +
                "SELECT g.`UnionID` AS `UnionID`, p.`UID`, p.`Name`, p.`GuildID`, 'guild' AS `Via`, p.`UnionRank`\n" +
                "FROM `players` p JOIN `guilds` g ON g.`GuildID` = p.`GuildID`\n" +
                "WHERE p.`GuildID` <> 0 AND g.`UnionID` <> 0\n" +
                "UNION ALL\n" +
                "SELECT p.`UnionUID`, p.`UID`, p.`Name`, 0, 'direct', p.`UnionRank`\n" +
                "FROM `players` p WHERE p.`GuildID` = 0 AND p.`UnionUID` <> 0;",

                "CREATE OR REPLACE VIEW `clan_members_v` AS\n" +
                "SELECT p.`ClanID`, p.`UID`, p.`Name`, p.`ClanRank`, p.`ClanDonation`, p.`Level`, p.`Class`\n" +
                "FROM `players` p WHERE p.`ClanID` <> 0;"
            };
        }

        // =====================================================================
        //  Helpers
        // =====================================================================
        private static string DbRoot { get { return Program.ServerConfig.DbLocation; } }

        // نفس سلوك DBActions.Read: أول سطر Count=N ثم N سطر (السطر الناقص = null)
        private static string[] ReadCountFile(string path)
        {
            if (!File.Exists(path)) return null;
            string[] all = File.ReadAllLines(path);
            if (all.Length == 0) return null;
            int count;
            string first = all[0];
            int eq = first.IndexOf('=');
            if (eq < 0 || !int.TryParse(first.Substring(eq + 1).Trim(), out count) || count <= 0) return null;
            string[] lines = new string[count];
            for (int i = 0; i < count; i++)
                lines[i] = (i + 1 < all.Length) ? all[i + 1] : null;
            return lines;
        }

        private static string Line(string[] lines, int idx)
        {
            return (lines != null && idx < lines.Length) ? lines[idx] : null;
        }

        private static uint LineUInt(string[] lines, int idx)
        {
            uint v;
            string s = Line(lines, idx);
            return (s != null && uint.TryParse(s.Trim(), out v)) ? v : 0;
        }

        private static void Exec(MySqlConnection conn, MySqlTransaction tx, string sql, params object[] nameValue)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                for (int i = 0; i + 1 < nameValue.Length; i += 2)
                    cmd.Parameters.AddWithValue((string)nameValue[i], nameValue[i + 1] ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        // INSERT ... ON DUPLICATE KEY UPDATE لصف واحد. cols[0..pkCount) = PK.
        private static void UpsertRow(MySqlConnection conn, MySqlTransaction tx, string table, string[] cols, int pkCount, object[] vals, string extraUpdate)
        {
            var insCols = new StringBuilder();
            var insVals = new StringBuilder();
            var upd = new StringBuilder();
            for (int i = 0; i < cols.Length; i++)
            {
                if (i > 0) { insCols.Append(','); insVals.Append(','); }
                insCols.Append('`').Append(cols[i]).Append('`');
                insVals.Append("@p").Append(i);
                if (i >= pkCount)
                {
                    if (upd.Length > 0) upd.Append(',');
                    upd.Append('`').Append(cols[i]).Append("`=VALUES(`").Append(cols[i]).Append("`)");
                }
            }
            if (!string.IsNullOrEmpty(extraUpdate))
            {
                if (upd.Length > 0) upd.Append(',');
                upd.Append(extraUpdate);
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO `" + table + "` (" + insCols + ") VALUES (" + insVals + ") ON DUPLICATE KEY UPDATE " + upd + ";";
                for (int i = 0; i < cols.Length; i++)
                    cmd.Parameters.AddWithValue("@p" + i, vals[i] ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        // DELETE WHERE <parentCol>=@id ثم INSERT multi-row (دفعات) — جوّه نفس الـ transaction.
        // cols[0] = عمود الأب، cols[0..pkCount) = PK. لو مفيش أعمدة غير الـ PK -> INSERT IGNORE.
        private static void ReplaceChildRows(MySqlConnection conn, MySqlTransaction tx, string table, string[] cols, int pkCount, uint parentId, List<object[]> rows)
        {
            Exec(conn, tx, "DELETE FROM `" + table + "` WHERE `" + cols[0] + "`=@id;", "@id", parentId);
            if (rows == null || rows.Count == 0) return;

            var colSql = new StringBuilder();
            var upd = new StringBuilder();
            for (int i = 0; i < cols.Length; i++)
            {
                if (i > 0) colSql.Append(',');
                colSql.Append('`').Append(cols[i]).Append('`');
                if (i >= pkCount)
                {
                    if (upd.Length > 0) upd.Append(',');
                    upd.Append('`').Append(cols[i]).Append("`=VALUES(`").Append(cols[i]).Append("`)");
                }
            }
            string head = (upd.Length == 0 ? "INSERT IGNORE INTO `" : "INSERT INTO `") + table + "` (" + colSql + ") VALUES ";
            string tail = upd.Length == 0 ? ";" : " ON DUPLICATE KEY UPDATE " + upd + ";";

            for (int start = 0; start < rows.Count; start += MaxRowsPerInsert)
            {
                int end = Math.Min(rows.Count, start + MaxRowsPerInsert);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    var sb = new StringBuilder(head);
                    int p = 0;
                    for (int r = start; r < end; r++)
                    {
                        if (r > start) sb.Append(',');
                        sb.Append('(');
                        object[] row = rows[r];
                        for (int c = 0; c < cols.Length; c++)
                        {
                            if (c > 0) sb.Append(',');
                            string name = "@p" + p++;
                            sb.Append(name);
                            cmd.Parameters.AddWithValue(name, (c < row.Length ? row[c] : null) ?? (object)DBNull.Value);
                        }
                        sb.Append(')');
                    }
                    sb.Append(tail);
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // تعليم التفكيك من غير حذف الصف (بيتنفّذ خارج ثريد الباكت عشان ما يعطّلش اللعب)
        private static void MarkDisbandedAsync(string table, string idCol, uint id, string what)
        {
            if (!Enabled) return;
            if (MirrorQueue.Async)
            {
                // نفس مفتاح الحفظ (guild:/clan:) -> بيتنفّذ بالترتيب بعد أي حفظ منتظر لنفس الجيلد/الكلان
                // (tag مختلف = مش بيتدمج مع الحفظ ولا بيلغيه).
                string key = (table == "guilds" ? "guild:" : "clan:") + id;
                MirrorQueue.Submit(key, "disband", false, () => () =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        Exec(conn, null, "UPDATE `" + table + "` SET `IsDisbanded`=1, `DisbandedAt`=NOW() WHERE `" + idCol + "`=@id;", "@id", id);
                    }
                }, what + "(" + id + ") mirror failed (game unaffected)");
                return;
            }
            // AsyncMirror=0: السلوك القديم بالظبط (ThreadPool)
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        using (var conn = DataHolder.MySqlConnection)
                        {
                            conn.Open();
                            Exec(conn, null, "UPDATE `" + table + "` SET `IsDisbanded`=1, `DisbandedAt`=NOW() WHERE `" + idCol + "`=@id;", "@id", id);
                        }
                    }
                    catch (Exception e)
                    {
                        try { MyConsole.WriteLine("[Storage] " + what + "(" + id + ") mirror failed (game unaffected): " + e.Message, ConsoleColor.Yellow); } catch { }
                    }
                });
            }
            catch { }
        }

        // =====================================================================
        //  Guild  (Guilds\<id>.txt — نفس تحليل GuildTable.Load)
        // =====================================================================
        private static readonly string[] GuildCols =
        {
            "GuildID",
            "GuildName", "GuildLevel", "LeaderName", "SilverFund", "ConquerPointsFund", "CreatedDate",
            "Bulletin", "BulletinDate", "UnionID", "LeaderUID", "Material", "Prestige",
            "RecruitClassMask", "RecruitMinBattlePower", "RecruitmentOn", "RecruitmentOff",
            "ArsenalBattlePower", "CommanderUID",
            "CtfExploits", "CtfNextConquerPoints", "CtfNextMoney", "CtfRank", "CtfRewardClaimed"
        };
        private static readonly string[] GuildAllyCols = { "GuildID", "AllyGuildID" };
        private static readonly string[] GuildEnemyCols = { "GuildID", "EnemyGuildID" };
        private static readonly string[] GuildMessageCols = { "GuildID", "Seq", "MessageText", "CreatedUnix" };
        private static readonly string[] GuildConstructCols = { "GuildID", "ConstructType", "BeastIndex", "ConstructLevel", "Experience", "ExtraValue2", "ExtraValue3", "ExtraValue4" };

        /// <summary>بعد write.Execute في GuildTable.Save: يعيد قراءة Guilds\&lt;key&gt;.txt ويعكسه.</summary>
        public static void OnGuildSaved(uint fileKey)
        {
            if (!Enabled) return;
            // قراءة الملف هنا (ثريد المنادي)، والـ UPSERT في الخلفية لو [Storage] AsyncMirror=1
            MirrorQueue.Submit("guild:" + fileKey, "save", false, () =>
            {
                string[] lines = ReadCountFile(DbRoot + "Guilds\\" + fileKey + ".txt");
                if (lines == null) return null;

                // ---- السطر 1: الهيدر (نفس ترتيب GuildTable.Load) ----
                ReadLine h = new ReadLine(Line(lines, 0) ?? "0/", '/');
                uint id = h.Read((uint)0);
                if (id == 0) id = fileKey;
                object[] g = new object[GuildCols.Length];
                g[0] = id;
                g[1] = h.Read("None");          // GuildName
                g[2] = h.Read((uint)0);         // Level
                g[3] = h.Read("None");          // LeaderName
                g[4] = h.Read((long)0);         // SilverFund
                g[5] = h.Read((uint)0);         // ConquerPointFund
                g[6] = h.Read((uint)0);         // CreateTime
                g[7] = h.Read("None");          // Bulletin
                g[8] = h.Read((int)0);          // BuletinEnrole
                g[9] = h.Read((uint)0);         // UnionID
                g[10] = h.Read((uint)0);        // Leaderid
                g[11] = h.Read((ulong)0);       // Material
                g[12] = h.Read((ulong)0);       // Prestige
                g[13] = h.Read((uint)0);        // Recruitment_Flag
                g[14] = h.Read((uint)0);        // Recruitment_Battle_Power
                g[15] = h.Read((byte)0) == 1 ? (byte)1 : (byte)0;   // RecruitmentON
                g[16] = h.Read((byte)0) == 1 ? (byte)1 : (byte)0;   // RecruitmentOFF
                g[17] = h.Read((uint)0);        // ArsenalBP
                g[18] = h.Read((uint)0);        // CommandID
                // ---- السطور 4-8: CTF ----
                g[19] = LineUInt(lines, 3);
                g[20] = LineUInt(lines, 4);
                g[21] = LineUInt(lines, 5);
                g[22] = LineUInt(lines, 6);
                g[23] = LineUInt(lines, 7);

                // ---- السطر 2/3: الحلفاء والأعداء ----
                var allies = new List<object[]>();
                var enemies = new List<object[]>();
                ReadIdList(Line(lines, 1), id, allies);
                ReadIdList(Line(lines, 2), id, enemies);

                // ---- السطر 9: الرسائل (Count/msg/time/...) ----
                var msgs = new List<object[]>();
                ReadLine m = new ReadLine(Line(lines, 8) ?? "0/", '/');
                int mc = m.Read(0);
                for (int i = 0; i < mc; i++)
                {
                    string text = m.Read("");
                    uint time = m.Read((uint)0);
                    msgs.Add(new object[] { id, (uint)i, text ?? "", time });
                }

                // ---- السطر 10: المنشآت (Count/ID/Level/Exp/Beast/v2/v3/v4/...) ----
                var cons = new List<object[]>();
                ReadLine c = new ReadLine(Line(lines, 9) ?? "0/", '/');
                int cc = c.Read(0);
                for (int i = 0; i < cc; i++)
                {
                    uint cid = c.Read((uint)0);
                    uint level = c.Read((uint)0);
                    ulong exp = c.Read((ulong)0);
                    ulong beast = c.Read((ulong)0);
                    ulong v2 = c.Read((ulong)0);
                    ulong v3 = c.Read((ulong)0);
                    ulong v4 = c.Read((ulong)0);
                    cons.Add(new object[] { id, cid, beast, level, exp, v2, v3, v4 });
                }

                return (Action)(() =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var tx = conn.BeginTransaction())
                        {
                            UpsertRow(conn, tx, "guilds", GuildCols, 1, g, "`IsDisbanded`=0,`DisbandedAt`=NULL");
                            ReplaceChildRows(conn, tx, "guild_allies", GuildAllyCols, 2, id, allies);
                            ReplaceChildRows(conn, tx, "guild_enemies", GuildEnemyCols, 2, id, enemies);
                            ReplaceChildRows(conn, tx, "guild_messages", GuildMessageCols, 2, id, msgs);
                            ReplaceChildRows(conn, tx, "guild_constructs", GuildConstructCols, 3, id, cons);
                            tx.Commit();
                        }
                    }
                });
            }, "OnGuildSaved(" + fileKey + ") mirror failed (file is still saved)");
        }

        // "Count/id1/id2/" -> صفوف (parentId, id) — نفس LoadGuildAlly/LoadclanAlly
        private static void ReadIdList(string line, uint parentId, List<object[]> rows)
        {
            ReadLine r = new ReadLine(line, '/');
            int n = r.Read(0);
            for (int i = 0; i < n; i++)
                rows.Add(new object[] { parentId, r.Read((uint)0) });
        }

        /// <summary>Guild.Dismis: يعلّم الجيلد متفكّك (IsDisbanded=1) من غير حذف الصف.</summary>
        public static void OnGuildDisbanded(uint guildId)
        {
            MarkDisbandedAsync("guilds", "GuildID", guildId, "OnGuildDisbanded");
        }

        // =====================================================================
        //  Union  (Unions\<id>.ini — نفس تحليل LeagueTable.Load)
        // =====================================================================
        private static readonly string[] UnionCols =
        {
            "UnionID",
            "UnionName", "EmperorName", "GoldBricks", "Treasury", "Stipend", "EmperorUID", "IsKingdom",
            "PlunderTargetUnionID", "InvadingUnionID", "Bulletin", "UnionTitle", "PlunderTargetName",
            "InvadingUnionName", "RecruitDeclaration"
        };

        /// <summary>بعد كتابة Unions\&lt;uid&gt;.ini في LeagueTable.Save: يعيد قراءة الملف ويعكسه.</summary>
        public static void OnUnionSaved(uint unionUid)
        {
            if (!Enabled) return;
            MirrorQueue.Submit("union:" + unionUid, "save", false, () =>
            {
                string path = DbRoot + "\\Unions\\" + unionUid + ".ini";
                if (!File.Exists(path)) return null;
                WindowsAPI.IniFile reader = new WindowsAPI.IniFile(path, true);

                object[] u = new object[UnionCols.Length];
                uint id = reader.ReadUInt32("Info", "UID", 0);
                u[0] = id;
                u[1] = reader.ReadString("Info", "Name", "None");
                u[2] = reader.ReadString("Info", "Emperor", "None");
                u[3] = reader.ReadUInt32("Info", "GoldBrick", 0);
                u[4] = reader.ReadUInt64("Info", "Treasury", 0);
                u[5] = reader.ReadUInt32("Info", "Stipend", 0);
                u[6] = reader.ReadUInt32("Info", "EmperrorUID", 0);
                u[7] = reader.ReadByte("Info", "IsKingdom", 0);
                u[8] = reader.ReadUInt32("Info", "PlunderID", 0);
                u[9] = reader.ReadUInt32("Info", "InvadingID", 0);
                u[10] = reader.ReadString("Info", "Bulletin", "None");
                u[11] = reader.ReadString("Info", "Title", "None");
                u[12] = reader.ReadString("Info", "PlunderTarget", "None");
                u[13] = reader.ReadString("Info", "InvadingUnion", "None");
                u[14] = reader.ReadString("Info", "RecruitDeclaration", "None");

                return (Action)(() =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        UpsertRow(conn, null, "unions", UnionCols, 1, u, "`IsDisbanded`=0,`DisbandedAt`=NULL");
                    }
                });
            }, "OnUnionSaved(" + unionUid + ") mirror failed (file is still saved)");
        }

        // =====================================================================
        //  Clan  (Clans\<id>.txt — نفس تحليل ClanTable.Load / Clan.Load)
        // =====================================================================
        private static readonly string[] ClanCols =
        {
            "ClanID",
            "ClanName", "LeaderName", "ClanLevel", "Fund", "Bulletin", "BattlePowerLevel", "StoreLevel",
            "Reserved1", "Reserved2", "Reserved3"
        };
        private static readonly string[] ClanRelationCols = { "ClanID", "RelationType", "OtherClanID" };

        /// <summary>بعد Execute في ClanTable.Save: يعيد قراءة Clans\&lt;key&gt;.txt ويعكسه.</summary>
        public static void OnClanSaved(uint fileKey)
        {
            if (!Enabled) return;
            MirrorQueue.Submit("clan:" + fileKey, "save", false, () =>
            {
                string[] lines = ReadCountFile(DbRoot + "Clans\\" + fileKey + ".txt");
                if (lines == null) return null;

                string l1 = Line(lines, 0);
                if (string.IsNullOrEmpty(l1)) return null;          // Clan.Load بيتجاهل السطر الفاضي
                ReadLine r = new ReadLine(l1, '/');
                object[] c = new object[ClanCols.Length];
                uint id = r.Read((uint)0);
                if (id == 0) id = fileKey;
                c[0] = id;
                c[1] = r.Read("None");          // Name
                c[2] = r.Read("None");          // LeaderName
                c[3] = r.Read((byte)0);         // Level
                c[4] = r.Read((uint)0);         // Donation
                c[5] = r.Read("None");          // ClanBuletin
                c[6] = r.Read((byte)0);         // BP
                c[7] = r.Read((byte)0);         // LevelStore
                c[8] = r.Read((int)0);          // reserved
                c[9] = r.Read((int)0);
                c[10] = r.Read((int)0);

                var rel = new List<object[]>();
                var tmp = new List<object[]>();
                ReadIdList(Line(lines, 1), id, tmp);
                foreach (var t in tmp) rel.Add(new object[] { id, (byte)1, t[1] });
                tmp.Clear();
                ReadIdList(Line(lines, 2), id, tmp);
                foreach (var t in tmp) rel.Add(new object[] { id, (byte)2, t[1] });

                return (Action)(() =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var tx = conn.BeginTransaction())
                        {
                            UpsertRow(conn, tx, "clans", ClanCols, 1, c, "`IsDisbanded`=0,`DisbandedAt`=NULL");
                            ReplaceChildRows(conn, tx, "clan_relations", ClanRelationCols, 3, id, rel);
                            tx.Commit();
                        }
                    }
                });
            }, "OnClanSaved(" + fileKey + ") mirror failed (file is still saved)");
        }

        /// <summary>Clan.DisbandClan: يعلّم الكلان متفكّك (IsDisbanded=1) من غير حذف الصف.</summary>
        public static void OnClanDisbanded(uint clanId)
        {
            MarkDisbandedAsync("clans", "ClanID", clanId, "OnClanDisbanded");
        }
    }
}
