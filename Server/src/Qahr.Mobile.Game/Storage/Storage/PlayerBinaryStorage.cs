// ============================================================================
//  PlayerBinaryStorage - المرحلة 2 من هجرة بيانات اللاعب لـ MySQL: مرآة الجداول
//  الثنائية (.bin) الخاصة باللاعب في جداول MySQL، في وضع Dual.
//
//  المبدأ (أأمن تصميم بصفر خطر):
//    * الملفات (.bin) تفضل مصدر الحقيقة؛ الـ DB مرآة بس (للقراءة/التقارير لاحقًا).
//    * صفر تغيير في منطق الحفظ/التحميل الأصلي. كل دالة مرآة هنا بتتنادى بسطر واحد
//      في آخر دالة Save* المقابلة، وبتقرأ نفس ملف الـ .bin اللي الحفظ لسه كاتبه
//      بنفس أسلوب دالة Load* (نفس BinaryFile + نفس الـ struct) فالقيم تطابق المكتوب.
//    * أي فشل DB لا يكسر الحفظ أبدًا: كله جوّه try/catch صامت (تحذير بس).
//    * أي شيء يعمل فقط لو PlayerStorage.Backend != File (و schema جاهز).
//
//  الجداول السبعة (CREATE TABLE IF NOT EXISTS في EnsureSchema):
//    player_items            PK(PlayerUID, UID)        - many per player
//    player_item_perfection  PK(PlayerUID, ItemUID)    - many per player
//    player_mail             PK(PlayerUID, id)         - many per player
//    player_profs            PK(PlayerUID, ID)         - many per player
//    player_spells           PK(PlayerUID, ID)         - many per player
//    player_hairface_storage PK(PlayerUID, ID, Type)   - many per player
//    player_title_equips     PK(PlayerUID)             - one per player
//
//  كل جدول فيه عمود PlayerUID (مفتاح الربط باللاعب) ضمن الـ PRIMARY KEY المركّب.
//  أعمدة النص VARCHAR(len-1) عشان الـ null terminator في الـ fixed buffers.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySqlConnector; // [net10] .NET 10 build (Qahr.Mobile.Net10.csproj)
using Qahr.Mobile.DBFunctionality;

namespace Qahr.Mobile.Database.Storage
{
    public static class PlayerBinaryStorage
    {
        private static bool _ready;

        // -------- أسماء أعمدة الجداول (نفس ترتيب الـ struct؛ أول عمود/أعمدة = PK) --------
        private static readonly string[] ItemCols =
        {
            "PlayerUID", "UID",                                   // PK (pkCount = 2)
            "ITEM_ID", "Durability", "MaximDurability", "Position", "SocketProgress",
            "SocketOne", "SocketTwo", "Effect", "Plus", "Bless", "Bound", "Enchant",
            "Suspicious", "Locked", "PlusProgress", "Inscribed", "Activate",
            "TimeLeftInMinutes", "StackSize", "WH_ID", "Color", "UnLockTimer", "ItemPoints",
            "RemainingTime", "UpsurgeID", "test4", "PurificationItemID", "PurificationLevel",
            "PurificationDuration", "PurificationAddedOn", "EffectID", "EffectLevel",
            "EffectPercent", "EffectPercent2", "EffectDuration", "EffectAddedOn",
            "DepositeCount", "AnimaItemID", "MythSoulID", "MythSoulProgress", "Mutacion",
            "RuneEXP", "Attribute1", "Attribute2", "Attribute3", "Attribute4", "Attribute5",
            "YuanshenIndex", "YuanshenTime", "Timer", "RelicResonance", "Mutacion2"
        };

        private static readonly string[] PerfectionCols =
        {
            "PlayerUID", "ItemUID",                               // PK (pkCount = 2)
            "Level", "Progres", "OwnerUID", "OwnerName", "SpecialText"
        };

        private static readonly string[] MailCols =
        {
            "PlayerUID", "id",                                    // PK (pkCount = 2)
            "Money", "EMoney", "itemid", "Emoney_Record_Type", "Flag", "Timer",
            "title", "Sender_name", "content"
        };

        private static readonly string[] ProfCols =
        {
            "PlayerUID", "ID",                                    // PK (pkCount = 2)
            "Level", "PreviouseLevel", "Experience", "NeededExperience"
        };

        private static readonly string[] SpellCols =
        {
            "PlayerUID", "ID",                                    // PK (pkCount = 2)
            "Level", "Experience", "PreviousLevel", "SoulLevel", "UseJiangSpell"
        };

        private static readonly string[] HairfaceCols =
        {
            "PlayerUID", "ID", "Type",                            // PK (pkCount = 3)
            "EquippedColor", "Equipped", "Colors"
        };

        private static readonly string[] TitleCols =
        {
            "PlayerUID",                                          // PK (pkCount = 1)
            "SpecialTitleID", "SpecialWingID", "SpecialHaloID", "SpecialFootprintID", "SpecialHaloAction"
        };

        // =====================================================================
        //  Raw blob mirror (byte-exact .bin content) - the DB-only reload source [2026-10-17]
        //  The normalized tables above stay as a read-only mirror for SQL queries; the authoritative
        //  reload comes from these exact bytes (deposit chains and every struct field round-trip perfectly).
        //  `store` = the source folder name (PlayersItems, PlayersMail, HairfaceStorage, ...).
        // =====================================================================
        public static void OnBlobSaved(string finalPath)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return;
            uint uid; string store;
            if (!ParseBlobPath(finalPath, out uid, out store) || uid == 0) return;
            MirrorQueue.Submit("blob:" + store + ":" + uid, "save", false, () =>
            {
                byte[] data = null;
                try { data = File.ReadAllBytes(finalPath); } catch { }
                return (Action)(() => { if (data != null) SaveBlob(uid, store, data); });
            }, "OnBlobSaved(" + store + "/" + uid + ") failed (file is still saved)");
        }

        private static bool ParseBlobPath(string path, out uint uid, out string store)
        {
            uid = 0; store = null;
            try
            {
                store = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));
                return !string.IsNullOrEmpty(store) && uint.TryParse(System.IO.Path.GetFileNameWithoutExtension(path), out uid);
            }
            catch { return false; }
        }

        public static void SaveBlob(uint uid, string store, byte[] data)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || data == null) return;
            using (var conn = Qahr.Mobile.DBFunctionality.DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO `player_blobs` (`uid`,`store`,`data`,`updated_at`) VALUES (@u,@s,@d,NOW()) " +
                                      "ON DUPLICATE KEY UPDATE `data`=VALUES(`data`),`updated_at`=VALUES(`updated_at`);";
                    cmd.Parameters.AddWithValue("@u", uid);
                    cmd.Parameters.AddWithValue("@s", store);
                    cmd.Parameters.AddWithValue("@d", data);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>One-time: copy every existing &lt;uid&gt;.bin under the DB folder into player_blobs (store = folder name).
        /// Lets players that were created before the blob mirror still load from the DB. Returns files written.</summary>
        public static int BackfillBlobsFromFiles()
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return 0;
            int n = 0;
            string db = Program.ServerConfig.DbLocation ?? "";
            if (!Directory.Exists(db)) return 0;
            foreach (string dir in Directory.GetDirectories(db))
            {
                string store = System.IO.Path.GetFileName(dir);
                foreach (string f in Directory.EnumerateFiles(dir, "*.bin"))
                {
                    uint uid;
                    if (!uint.TryParse(System.IO.Path.GetFileNameWithoutExtension(f), out uid) || uid == 0) continue;
                    try { SaveBlob(uid, store, File.ReadAllBytes(f)); n++; } catch { }
                }
            }
            return n;
        }

        /// <summary>Diagnostic: compare a player's on-disk .bin with its DB blob (byte-exact check before trusting DB-only).</summary>
        public static Dictionary<string, object> VerifyBlob(uint uid, string store)
        {
            var d = new Dictionary<string, object>();
            d["uid"] = uid; d["store"] = store;
            string path = (Program.ServerConfig.DbLocation ?? "") + "\\" + store + "\\" + uid + ".bin";
            byte[] file = null; try { if (File.Exists(path)) file = File.ReadAllBytes(path); } catch { }
            byte[] blob = LoadBlob(uid, store);
            d["fileLen"] = file == null ? -1 : file.Length;
            d["blobLen"] = blob == null ? -1 : blob.Length;
            bool eq = file != null && blob != null && file.Length == blob.Length;
            if (eq) for (int i = 0; i < file.Length; i++) if (file[i] != blob[i]) { eq = false; break; }
            d["equal"] = eq;
            return d;
        }

        /// <summary>Resolver for BinaryFile.Open: map a .bin file path to its DB blob bytes (or null -> use the file).</summary>
        public static byte[] LoadBlobByPath(string path)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return null;
            uint uid; string store;
            if (!ParseBlobPath(path, out uid, out store) || uid == 0) return null;
            return LoadBlob(uid, store);
        }

        /// <summary>Exact .bin bytes for a player's store, or null if none (then the caller falls back to the file).</summary>
        public static byte[] LoadBlob(uint uid, string store)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return null;
            try
            {
                using (var conn = Qahr.Mobile.DBFunctionality.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT `data` FROM `player_blobs` WHERE `uid`=@u AND `store`=@s;";
                        cmd.Parameters.AddWithValue("@u", uid);
                        cmd.Parameters.AddWithValue("@s", store);
                        return cmd.ExecuteScalar() as byte[];
                    }
                }
            }
            catch { return null; }
        }

        // [DB-only 2026-10-17] fileless binary save: store the exact bytes in player_blobs (the authoritative
        // reload source). No .bin file, and no normalized mirror tables (those were dropped - migration 0003).
        public static void OnBinarySaved(uint uid, string store, byte[] bytes)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || uid == 0 || bytes == null) return;
            MirrorQueue.Submit("blob:" + store + ":" + uid, "save", false,
                () => (Action)(() => SaveBlob(uid, store, bytes)),
                "OnBinarySaved(" + store + "/" + uid + ") failed");
        }

        // =====================================================================
        //  Schema
        // =====================================================================
        public static void EnsureSchema()
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File) return;
            try
            {
                using (var conn = DataHolder.MySqlConnection) { conn.Open(); } // connectivity check; player_blobs is created by migration 0002
                _ready = true;
                try { MyConsole.WriteLine("[Storage] Player binary storage ready (blob mode; normalized mirror tables removed).", ConsoleColor.Cyan); } catch { }
            }
            catch (Exception e)
            {
                _ready = false;
                try { MyConsole.WriteLine("[Storage] PlayerBinaryStorage.EnsureSchema failed (binary DB disabled, files unaffected): " + e.Message, ConsoleColor.Yellow); } catch { }
            }
        }

        private static string[] CreateStatements()
        {
            return new string[]
            {
                // ---- player_items ----
                "CREATE TABLE IF NOT EXISTS `player_items` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `UID` INT UNSIGNED NOT NULL,\n" +
                "  `ITEM_ID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Durability` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `MaximDurability` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Position` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SocketProgress` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SocketOne` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SocketTwo` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Effect` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Plus` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Bless` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Bound` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Enchant` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Suspicious` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Locked` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PlusProgress` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Inscribed` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Activate` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `TimeLeftInMinutes` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `StackSize` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `WH_ID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Color` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `UnLockTimer` INT NOT NULL DEFAULT 0,\n" +
                "  `ItemPoints` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `RemainingTime` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `UpsurgeID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `test4` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PurificationItemID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PurificationLevel` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PurificationDuration` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PurificationAddedOn` BIGINT NOT NULL DEFAULT 0,\n" +
                "  `EffectID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EffectLevel` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EffectPercent` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EffectPercent2` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EffectDuration` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EffectAddedOn` BIGINT NOT NULL DEFAULT 0,\n" +
                "  `DepositeCount` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `AnimaItemID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `MythSoulID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `MythSoulProgress` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Mutacion` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `RuneEXP` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Attribute1` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Attribute2` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Attribute3` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Attribute4` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Attribute5` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `YuanshenIndex` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `YuanshenTime` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Timer` BIGINT NOT NULL DEFAULT 0,\n" +
                "  `RelicResonance` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Mutacion2` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  PRIMARY KEY (`PlayerUID`, `UID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_item_perfection ----
                "CREATE TABLE IF NOT EXISTS `player_item_perfection` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `ItemUID` INT UNSIGNED NOT NULL,\n" +
                "  `Level` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Progres` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `OwnerUID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `OwnerName` VARCHAR(31) NOT NULL DEFAULT '',\n" +
                "  `SpecialText` VARCHAR(31) NOT NULL DEFAULT '',\n" +
                "  PRIMARY KEY (`PlayerUID`, `ItemUID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_mail ----
                "CREATE TABLE IF NOT EXISTS `player_mail` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `id` INT UNSIGNED NOT NULL,\n" +
                "  `Money` BIGINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `EMoney` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `itemid` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Emoney_Record_Type` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Flag` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Timer` BIGINT NOT NULL DEFAULT 0,\n" +
                "  `title` VARCHAR(63) NOT NULL DEFAULT '',\n" +
                "  `Sender_name` VARCHAR(127) NOT NULL DEFAULT '',\n" +
                "  `content` VARCHAR(511) NOT NULL DEFAULT '',\n" +
                "  PRIMARY KEY (`PlayerUID`, `id`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_profs ----
                "CREATE TABLE IF NOT EXISTS `player_profs` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `ID` INT UNSIGNED NOT NULL,\n" +
                "  `Level` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `PreviouseLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Experience` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `NeededExperience` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  PRIMARY KEY (`PlayerUID`, `ID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_spells ----
                "CREATE TABLE IF NOT EXISTS `player_spells` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `ID` SMALLINT UNSIGNED NOT NULL,\n" +
                "  `Level` SMALLINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Experience` INT NOT NULL DEFAULT 0,\n" +
                "  `PreviousLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SoulLevel` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `UseJiangSpell` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  PRIMARY KEY (`PlayerUID`, `ID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_hairface_storage ----
                "CREATE TABLE IF NOT EXISTS `player_hairface_storage` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `ID` INT UNSIGNED NOT NULL,\n" +
                "  `Type` TINYINT UNSIGNED NOT NULL,\n" +
                "  `EquippedColor` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Equipped` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `Colors` VARBINARY(1024) NULL,\n" +
                "  PRIMARY KEY (`PlayerUID`, `ID`, `Type`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

                // ---- player_title_equips ----
                "CREATE TABLE IF NOT EXISTS `player_title_equips` (\n" +
                "  `PlayerUID` INT UNSIGNED NOT NULL,\n" +
                "  `SpecialTitleID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SpecialWingID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SpecialHaloID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SpecialFootprintID` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  `SpecialHaloAction` INT UNSIGNED NOT NULL DEFAULT 0,\n" +
                "  PRIMARY KEY (`PlayerUID`)\n" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
            };
        }

        // =====================================================================
        //  UPSERT helper (DELETE صفوف اللاعب القديمة ثم INSERT ... ON DUPLICATE KEY UPDATE)
        //  cols[0..pkCount) = أعمدة الـ PRIMARY KEY؛ باقي الأعمدة بتتحدّث في ODKU.
        // =====================================================================
        private static void UpsertRows(string table, string[] cols, int pkCount, List<object[]> rows, uint uid, bool deleteFirst)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    if (deleteFirst)
                    {
                        using (var del = conn.CreateCommand())
                        {
                            del.Transaction = tx;
                            del.CommandText = "DELETE FROM `" + table + "` WHERE `PlayerUID`=@uid;";
                            del.Parameters.AddWithValue("@uid", uid);
                            del.ExecuteNonQuery();
                        }
                    }

                    if (rows.Count > 0)
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
                        string sql = "INSERT INTO `" + table + "` (" + insCols + ") VALUES (" + insVals + ")";
                        if (upd.Length > 0) sql += " ON DUPLICATE KEY UPDATE " + upd;
                        sql += ";";

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = sql;
                            foreach (var row in rows)
                            {
                                cmd.Parameters.Clear();
                                for (int i = 0; i < cols.Length; i++)
                                    cmd.Parameters.AddWithValue("@p" + i, row[i] ?? (object)DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
            }
        }

        // =====================================================================
        //  Items + Perfection
        // =====================================================================
        public static void OnItemsSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            // قراءة الملف هنا (ثريد المنادي)، والـ UPSERT في الخلفية لو [Storage] AsyncMirror=1
            MirrorQueue.Submit("items:" + uid, "save", false, () =>
            {
                var items = new List<object[]>();
                var perfs = new List<object[]>();
                ReadItemsFile(uid, items, perfs);
                return (Action)(() =>
                {
                    UpsertRows("player_items", ItemCols, 2, items, uid, true);
                    UpsertRows("player_item_perfection", PerfectionCols, 2, perfs, uid, true);
                });
            }, "OnItemsSaved(" + uid + ") mirror failed (file is still saved)");
        }

        // [DB-only 2026-10-17] fileless save: the exact bytes come from memory. Store the blob (the authoritative
        // reload source) and refresh the normalized mirror, both from the same bytes - no .bin file involved.
        public static void OnItemsSaved(Client.GameClient client, byte[] bytes)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null || bytes == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("items:" + uid, "save", false, () =>
            {
                var items = new List<object[]>();
                var perfs = new List<object[]>();
                ReadItemsBytes(bytes, uid, items, perfs);
                return (Action)(() =>
                {
                    SaveBlob(uid, "PlayersItems", bytes);
                    UpsertRows("player_items", ItemCols, 2, items, uid, true);
                    UpsertRows("player_item_perfection", PerfectionCols, 2, perfs, uid, true);
                });
            }, "OnItemsSaved(" + uid + ") mirror failed");
        }

        // يقرأ ملف PlayersItems\<uid>.bin بنفس أسلوب LoadClientItems (نفس الـ structs)
        private static unsafe void ReadItemsFile(uint uid, List<object[]> items, List<object[]> perfs)
        {
            string path = Program.ServerConfig.DbLocation + "\\PlayersItems\\" + uid + ".bin";
            if (!File.Exists(path)) return;
            var binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open)) return;
            try { ParseItems(binary, uid, items, perfs); }
            finally { binary.Close(); }
        }

        // Same parse from in-memory bytes (fileless save path).
        private static unsafe void ReadItemsBytes(byte[] data, uint uid, List<object[]> items, List<object[]> perfs)
        {
            if (data == null || data.Length == 0) return;
            var binary = new WindowsAPI.BinaryFile();
            if (!binary.OpenBytes(data)) return;
            try { ParseItems(binary, uid, items, perfs); }
            finally { binary.Close(); }
        }

        private static unsafe void ParseItems(WindowsAPI.BinaryFile binary, uint uid, List<object[]> items, List<object[]> perfs)
        {
            ClientItems.DBItem it;
            int count = 0;
            binary.Read(&count, sizeof(int));
            for (int x = 0; x < count; x++)
            {
                binary.Read(&it, sizeof(ClientItems.DBItem));
                items.Add(ItemRow(uid, it));
                if (it.DepositeCount != 0)
                {
                    uint dep = it.DepositeCount;
                    for (int i = 0; i < dep; i++)
                    {
                        binary.Read(&it, sizeof(ClientItems.DBItem));
                        items.Add(ItemRow(uid, it));
                    }
                }
            }

            int perfCount = 0;
            binary.Read(&perfCount, sizeof(int));
            for (int x = 0; x < perfCount; x++)
            {
                var info = new ClientItems.Perfection();
                binary.Read(&info, sizeof(ClientItems.Perfection));
                perfs.Add(new object[]
                {
                    uid, info.ItemUID, info.Level, info.Progres, info.OwnerUID,
                    info.OwnerName ?? "", info.SpecialText ?? ""
                });
            }
        }

        private static object[] ItemRow(uint uid, ClientItems.DBItem it)
        {
            return new object[]
            {
                uid, it.UID,
                it.ITEM_ID, it.Durability, it.MaximDurability, it.Position, it.SocketProgress,
                (byte)it.SocketOne, (byte)it.SocketTwo, (uint)it.Effect, it.Plus, it.Bless, it.Bound, it.Enchant,
                it.Suspicious, it.Locked, it.PlusProgress, it.Inscribed, it.Activate,
                it.TimeLeftInMinutes, it.StackSize, it.WH_ID, (uint)it.Color, it.UnLockTimer, it.ItemPoints,
                it.RemainingTime, it.UpsurgeID, it.test4, it.PurificationItemID, it.PurificationLevel,
                it.PurificationDuration, it.PurificationAddedOn, it.EffectID, it.EffectLevel,
                it.EffectPercent, it.EffectPercent2, it.EffectDuration, it.EffectAddedOn,
                it.DepositeCount, it.AnimaItemID, it.MythSoulID, it.MythSoulProgress, it.Mutacion,
                it.RuneEXP, it.Attribute1, it.Attribute2, it.Attribute3, it.Attribute4, it.Attribute5,
                it.YuanshenIndex, it.YuanshenTime, it.Timer, it.RelicResonance, it.Mutacion2
            };
        }

        // =====================================================================
        //  Mail
        // =====================================================================
        public static void OnMailsSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("mail:" + uid, "save", false, () =>
            {
                var rows = ReadMailFile(uid);
                return (Action)(() => UpsertRows("player_mail", MailCols, 2, rows, uid, true));
            }, "OnMailsSaved(" + uid + ") mirror failed (file is still saved)");
        }

        private static unsafe List<object[]> ReadMailFile(uint uid)
        {
            var rows = new List<object[]>();
            string path = Program.ServerConfig.DbLocation + "\\PlayersMail\\" + uid + ".bin";
            if (!File.Exists(path)) return rows;

            var binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open)) return rows;
            try
            {
                Game.MsgServer.ClientMail.DBPrize p;
                int count = 0;
                binary.Read(&count, sizeof(int));
                for (int x = 0; x < count; x++)
                {
                    binary.Read(&p, sizeof(Game.MsgServer.ClientMail.DBPrize));
                    rows.Add(new object[]
                    {
                        uid, p.id, p.Money, p.EMoney, p.itemid, p.Emoney_Record_Type, p.Flag, p.Timer,
                        p.title ?? "", p.Sender_name ?? "", p.content ?? ""
                    });
                }
            }
            finally { binary.Close(); }
            return rows;
        }

        // =====================================================================
        //  Proficiencies
        // =====================================================================
        public static void OnProfsSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("profs:" + uid, "save", false, () =>
            {
                var rows = ReadProfsFile(uid);
                return (Action)(() => UpsertRows("player_profs", ProfCols, 2, rows, uid, true));
            }, "OnProfsSaved(" + uid + ") mirror failed (file is still saved)");
        }

        private static unsafe List<object[]> ReadProfsFile(uint uid)
        {
            var rows = new List<object[]>();
            string path = Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + uid + ".bin";
            if (!File.Exists(path)) return rows;

            var binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open)) return rows;
            try
            {
                ClientProficiency.DBProf d;
                int count = 0;
                binary.Read(&count, sizeof(int));
                for (int x = 0; x < count; x++)
                {
                    binary.Read(&d, sizeof(ClientProficiency.DBProf));
                    rows.Add(new object[] { uid, d.ID, d.Level, d.PreviouseLevel, d.Experience, d.NeededExperience });
                }
            }
            finally { binary.Close(); }
            return rows;
        }

        // =====================================================================
        //  Spells
        // =====================================================================
        public static void OnSpellsSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("spells:" + uid, "save", false, () =>
            {
                var rows = ReadSpellsFile(uid);
                return (Action)(() => UpsertRows("player_spells", SpellCols, 2, rows, uid, true));
            }, "OnSpellsSaved(" + uid + ") mirror failed (file is still saved)");
        }

        private static unsafe List<object[]> ReadSpellsFile(uint uid)
        {
            var rows = new List<object[]>();
            string path = Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + uid + ".bin";
            if (!File.Exists(path)) return rows;

            var binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open)) return rows;
            try
            {
                ClientSpells.DBSpell s;
                int count = 0;
                binary.Read(&count, sizeof(int));
                for (int x = 0; x < count; x++)
                {
                    binary.Read(&s, sizeof(ClientSpells.DBSpell));
                    rows.Add(new object[] { uid, s.ID, s.Level, s.Experience, s.PreviousLevel, s.SoulLevel, s.UseJiangSpell });
                }
            }
            finally { binary.Close(); }
            return rows;
        }

        // =====================================================================
        //  Hairface storage
        // =====================================================================
        public static void OnHairfaceSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("hairface:" + uid, "save", false, () =>
            {
                var rows = ReadHairfaceFile(uid);
                return (Action)(() => UpsertRows("player_hairface_storage", HairfaceCols, 3, rows, uid, true));
            }, "OnHairfaceSaved(" + uid + ") mirror failed (file is still saved)");
        }

        private static unsafe List<object[]> ReadHairfaceFile(uint uid)
        {
            var rows = new List<object[]>();
            string path = Program.ServerConfig.DbLocation + "\\HairfaceStorage\\" + uid + ".bin";
            if (!File.Exists(path)) return rows;

            var binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open)) return rows;
            try
            {
                int count = 0;
                binary.Read(&count, sizeof(int));
                for (int i = 0; i < count; i++)
                {
                    uint id = 0; byte type = 0; byte color = 0; byte equipped = 0; int len = 0;
                    binary.Read(&id, sizeof(uint));
                    binary.Read(&type, sizeof(byte));
                    binary.Read(&color, sizeof(byte));
                    binary.Read(&equipped, sizeof(byte));
                    binary.Read(&len, sizeof(int));

                    byte[] colors = len > 0 ? new byte[len] : new byte[0];
                    if (len > 0)
                        fixed (byte* ptr = colors) { binary.Read(ptr, sizeof(byte) * len); }

                    rows.Add(new object[] { uid, id, type, color, equipped, colors });
                }
            }
            finally { binary.Close(); }
            return rows;
        }

        // =====================================================================
        //  Title equips (one row per player -> UPSERT بدون حذف)
        // =====================================================================
        public static void OnTitleEquipsSaved(Client.GameClient client)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            MirrorQueue.Submit("title:" + uid, "save", false, () =>
            {
                string path = Program.ServerConfig.DbLocation + "\\TitleStorage\\" + uid + ".bin";
                var row = ReadTitleFile(uid, path);
                var rows = new List<object[]> { row };
                return (Action)(() => UpsertRows("player_title_equips", TitleCols, 1, rows, uid, false));
            }, "OnTitleEquipsSaved(" + uid + ") mirror failed (file is still saved)");
        }

        private static unsafe object[] ReadTitleFile(uint uid, string path)
        {
            uint title = 0, wing = 0, halo = 0, foot = 0, haloAction = 0;
            if (File.Exists(path))
            {
                var binary = new WindowsAPI.BinaryFile();
                if (binary.Open(path, FileMode.Open))
                {
                    try
                    {
                        binary.Read(&title, sizeof(uint));
                        binary.Read(&wing, sizeof(uint));
                        binary.Read(&halo, sizeof(uint));
                        binary.Read(&foot, sizeof(uint));
                        binary.Read(&haloAction, sizeof(uint));
                    }
                    finally { binary.Close(); }
                }
            }
            return new object[] { uid, title, wing, halo, foot, haloAction };
        }

        // =====================================================================
        //  [Storage backfill 2026-10-16] مرآة شاملة لمرة واحدة: تقرأ ملفات .bin لكل لاعب
        //  (مش المتصلين بس) وتكتبها في الجداول السبعة مباشرةً (synchronous, من غير client/queue).
        //  لازمها قبل التبديل لـ DB-only عشان لاعبين مادخلوش من بعد ما المرآة اتعملت. idempotent.
        //  بترجع عدد اللاعبين اللي اتعملهم mirror.
        // =====================================================================
        public static int MirrorAllFromFiles()
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return 0;
            var uids = new List<uint>();
            try
            {
                string usersDir = Program.ServerConfig.DbLocation + "\\Users";
                foreach (var f in Directory.GetFiles(usersDir, "*.ini"))
                {
                    uint u; if (uint.TryParse(Path.GetFileNameWithoutExtension(f), out u)) uids.Add(u);
                }
            }
            catch { return 0; }
            int done = 0;
            foreach (uint uid in uids)
            {
                try
                {
                    var items = new List<object[]>(); var perfs = new List<object[]>();
                    ReadItemsFile(uid, items, perfs);
                    UpsertRows("player_items", ItemCols, 2, items, uid, true);
                    UpsertRows("player_item_perfection", PerfectionCols, 2, perfs, uid, true);
                    UpsertRows("player_mail", MailCols, 2, ReadMailFile(uid), uid, true);
                    UpsertRows("player_profs", ProfCols, 2, ReadProfsFile(uid), uid, true);
                    UpsertRows("player_spells", SpellCols, 2, ReadSpellsFile(uid), uid, true);
                    UpsertRows("player_hairface_storage", HairfaceCols, 3, ReadHairfaceFile(uid), uid, true);
                    var titleRow = ReadTitleFile(uid, Program.ServerConfig.DbLocation + "\\TitleStorage\\" + uid + ".bin");
                    UpsertRows("player_title_equips", TitleCols, 1, new List<object[]> { titleRow }, uid, false);
                    done++;
                }
                catch (Exception e) { try { MyConsole.WriteLine("[Storage] backfill uid " + uid + " failed: " + e.Message, ConsoleColor.Yellow); } catch { } }
            }
            return done;
        }

        // =====================================================================
        //  حذف الشخصية: امسح صفوف اللاعب من كل الجداول السبعة
        // =====================================================================
        public static void OnDeleted(uint uid)
        {
            if (PlayerStorage.Backend == PlayerStorage.Mode.File || !_ready) return;
            if (MirrorQueue.Async)
            {
                // حذف لكل مفتاح لوحده (supersede): أي حفظ منتظر لنفس الجدول/اللاعب بيتلغي -> النتيجة النهائية حذف
                DeleteAsync(uid, "items", "player_items", "player_item_perfection");
                DeleteAsync(uid, "mail", "player_mail");
                DeleteAsync(uid, "profs", "player_profs");
                DeleteAsync(uid, "spells", "player_spells");
                DeleteAsync(uid, "hairface", "player_hairface_storage");
                DeleteAsync(uid, "title", "player_title_equips");
                return;
            }
            // AsyncMirror=0: السلوك القديم بالظبط
            try
            {
                string[] tables =
                {
                    "player_items", "player_item_perfection", "player_mail", "player_profs",
                    "player_spells", "player_hairface_storage", "player_title_equips"
                };
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    foreach (string t in tables)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM `" + t + "` WHERE `PlayerUID`=@uid;";
                            cmd.Parameters.AddWithValue("@uid", uid);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try { MyConsole.WriteLine("[Storage] PlayerBinaryStorage.OnDeleted(" + uid + ") failed: " + e.Message, ConsoleColor.Yellow); } catch { }
            }
        }

        private static void DeleteAsync(uint uid, string key, params string[] tables)
        {
            MirrorQueue.Submit(key + ":" + uid, "delete", true, () => () =>
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    foreach (string t in tables)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM `" + t + "` WHERE `PlayerUID`=@uid;";
                            cmd.Parameters.AddWithValue("@uid", uid);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }, "PlayerBinaryStorage.OnDeleted(" + uid + ") failed");
        }
    }
}
