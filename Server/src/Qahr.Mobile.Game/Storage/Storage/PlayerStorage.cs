// ============================================================================
//  PlayerStorage - هجرة بيانات الشخصية من ملفات Users\<uid>.ini إلى MySQL،
//  بعمود لكل حقل (جدول players) بأسماء أعمدة واضحة.
//
//  مصدر الحقيقة الوحيد = players_columns.csv (جنب الـ exe، وإلا بنبحث لأعلى حتى
//  مجلد السورس). كل سطر: OldKey|CleanColumn|Type
//    OldKey      = مفتاح الـ ini القديم (= اسم العمود القديم) — ده اللي السورس بيكتبه.
//    CleanColumn = اسم العمود الواضح في جدول players.
//    Type        = نوع MySQL.
//  من الملف ده بنبني: (1) جملة CREATE TABLE بأسماء واضحة، (2) خريطة ربط
//  OldKey->CleanColumn، (3) نوع كل عمود (نص/صحيح/عشري) لتحويل القيم. فمستحيل
//  يحصل اختلاف بين الـ schema وخريطة الربط لأنهم من نفس المصدر.
//
//  الفكرة (أأمن تصميم بصفر خطر): SaveClient بيكتب ملف الـ ini كامل زي ما هو،
//  وبعدها بنقرأ الملف، نطابق كل مفتاح بعموده الواضح عبر خريطة الربط، ونعمل UPSERT.
//  فبنعيد استخدام نفس القيم اللي السورس حسبها بالظبط.
//
//  الوضع (ServerConfig.ini [Storage] PlayerBackend، وإلا shell.ini [Storage]):
//    File  (افتراضي) - سلوك قديم، DB مش بتتلمس.
//    Dual  - نكتب الملف + الأعمدة؛ القراءة لسه من الملف (الملفات مصدر الحقيقة).
//    MySql - (لاحقًا بعد التحقق) القراءة من الأعمدة.
//
//  موصول من ServerDatabase.SaveClient (النهاية) + LoadCharacter (البداية)،
//  وInit() من Server.Initialize.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MySqlConnector; // [net10] .NET 10 build (Qahr.Mobile.Net10.csproj)
using Qahr.Mobile.DBFunctionality;

namespace Qahr.Mobile.Database.Storage
{
    public static class PlayerStorage
    {
        public enum Mode { File, Dual, MySql }
        public static Mode Backend = Mode.File;

        // [Storage] PlayerReadSource: منين LoadCharacter بتقرا بيانات الشخصية الأساسية (Users\<uid>.ini <-> players)
        //   File   = الملف (القديم).
        //   Verify = الملف، + كل قيمة بتتقارن بالقاعدة بعد التحويل للنوع، والاختلافات -> storage_verify.txt.
        //   Db     = القاعدة (أي مفتاح مالوش عمود بيتقري من الملف). لو اللاعب عنده حفظ لسه في طابور القاعدة
        //            أو آخر حفظ فشل، أو الصف مش موجود، أو القاعدة وقعت -> الملف (عمره ما يقرا داتا قديمة).
        public enum ReadMode { File, Verify, Db }
        public static ReadMode ReadSource = ReadMode.File;
        private static bool _ready;

        private const int KIND_TEXT = 0, KIND_INT = 1, KIND_REAL = 2;

        private static Dictionary<string, string> _rename;   // OldKey -> CleanColumn
        private static Dictionary<string, int> _cols;         // CleanColumn -> kind
        private static string _createSql;

        // أعمدة لها قيمة افتراضية خاصة (غير 0/''): الموقع والأسماء الابتدائية.
        private static readonly Dictionary<string, string> _defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "BodyType", "1002" }, { "MapID", "1002" }, { "PrevMapID", "1002" },
            { "PosX", "410" }, { "PrevMapX", "410" }, { "PosY", "354" }, { "PrevMapY", "354" },
            { "Name", "'None'" }, { "SpouseName", "'None'" }, { "Description", "'None'" },
        };

        public static void Init()
        {
            // [DB-only 2026-10-17] MySql is the only supported backend (File/Dual were removed as options).
            // If the DB connection fails, EnsureSchema below leaves _ready=false and the loader still falls
            // back to files at runtime - phase 2 removes file storage entirely.
            try
            {
                string s = (Qahr.Mobile.SharedConfig.String("Storage", "PlayerBackend", "MySql") ?? "MySql").Trim().ToLowerInvariant();
                if (s == "file" || s == "dual")
                    try { MyConsole.WriteLine("[Storage] PlayerBackend='" + s + "' is no longer supported - using MySql.", ConsoleColor.Yellow); } catch { }
            }
            catch { }
            Backend = Mode.MySql;

            if (Backend != Mode.File)
            {
                Qahr.Mobile.Database.Migrations.Migrator.EnsureDatabase(); // [Migrations] create the DB if missing (connection OK)
                EnsureSchema();
                MirrorQueue.Init();   // [Storage] AsyncMirror=1 -> UPSERTs في الخلفية بدل ثريد اللعب
                Qahr.Mobile.Database.Migrations.Migrator.Run();            // [Migrations] apply *.sql deltas once, in order
                string rs = (Qahr.Mobile.SharedConfig.String("Storage", "PlayerReadSource", "File") ?? "File").Trim().ToLowerInvariant();
                ReadSource = !_ready ? ReadMode.File : rs == "db" ? ReadMode.Db : rs == "verify" ? ReadMode.Verify : ReadMode.File;
                if (ReadSource != ReadMode.File)
                    try { MyConsole.WriteLine("[Storage] Player read source: " + ReadSource, ConsoleColor.Cyan); } catch { }
                StartOnlineSync();                                        // [Dashboard] keep players.Online accurate (~15s)
                // [DB-only 2026-10-17] binary loads (items/mail/profs/spells/quests/houses/...) now read the
                // exact bytes from player_blobs; the file is only a fallback when no blob exists yet.
                global::Qahr.Mobile.WindowsAPI.BinaryFile.BlobResolver = PlayerBinaryStorage.LoadBlobByPath;
            }
            try { MyConsole.WriteLine("[Storage] Player backend: " + Backend + (Backend != Mode.File ? (_ready ? " (players table ready, " + (_cols != null ? _cols.Count : 0) + " cols)" : " (table NOT ready - using files)") : ""), ConsoleColor.Cyan); }
            catch { }
        }

        // يلاقي players_columns.csv جنب الـ exe أو بالبحث لأعلى (زي SharedConfig مع ServerConfig.ini)
        private static string FindMapFile()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 12 && !string.IsNullOrEmpty(dir); i++)
                {
                    string p1 = Path.Combine(dir, "players_columns.csv");
                    if (File.Exists(p1)) return p1;
                    string p2 = Path.Combine(dir, "Database\\Storage\\players_columns.csv");
                    if (File.Exists(p2)) return p2;
                    DirectoryInfo parent = Directory.GetParent(dir);
                    dir = parent != null ? parent.FullName : null;
                }
            }
            catch { }
            return null;
        }

        // يقرأ الـ CSV ويبني خريطة الربط + أنواع الأعمدة + جملة CREATE TABLE
        private static bool LoadMap()
        {
            string file = FindMapFile();
            if (file == null) { try { MyConsole.WriteLine("[Storage] players_columns.csv not found - using files.", ConsoleColor.Yellow); } catch { } return false; }

            var rename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE IF NOT EXISTS `players` (\n");

            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                string[] p = line.Split('|');
                if (p.Length != 3) continue;
                string oldKey = p[0].Trim(), clean = p[1].Trim(), type = p[2].Trim();
                if (oldKey.Length == 0 || clean.Length == 0 || type.Length == 0) continue;

                rename[oldKey] = clean;

                string up = type.ToUpperInvariant();
                int kind = (up.Contains("DOUBLE") || up.Contains("FLOAT") || up.Contains("DECIMAL")) ? KIND_REAL
                         : (up.Contains("INT") || up.Contains("BIT")) ? KIND_INT
                         : KIND_TEXT;
                cols[clean] = kind;

                // سطر تعريف العمود
                sb.Append("  `").Append(clean).Append("` ").Append(type);
                if (clean.Equals("UID", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" NOT NULL");
                }
                else if (up.Contains("TEXT"))
                {
                    sb.Append(" NULL");
                }
                else
                {
                    string def;
                    if (!_defaults.TryGetValue(clean, out def))
                        def = (kind == KIND_TEXT) ? "''" : "0";
                    sb.Append(" NOT NULL DEFAULT ").Append(def);
                }
                sb.Append(",\n");
            }

            // المفتاح الأساسي + الفهارس (أسماء الأعمدة دي لم تتغيّر)
            sb.Append("  PRIMARY KEY (`UID`),\n");
            sb.Append("  KEY `idx_name` (`Name`),\n");
            sb.Append("  KEY `idx_guild` (`GuildID`),\n");
            sb.Append("  KEY `idx_clan` (`ClanID`),\n");
            sb.Append("  KEY `idx_union` (`UnionUID`),\n");
            sb.Append("  KEY `idx_spouse` (`SpouseUID`),\n");
            sb.Append("  KEY `idx_level` (`Level`),\n");
            sb.Append("  KEY `idx_online` (`Online`)\n");
            sb.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");

            _rename = rename;
            _cols = cols;
            _createSql = sb.ToString();
            return _rename.Count > 0 && _cols.Count > 0;
        }

        private static void EnsureSchema()
        {
            try
            {
                if (!LoadMap()) { _ready = false; return; }

                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = _createSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                _ready = true;

                // المرحلة 2: مرآة الجداول الثنائية (items/mail/profs/spells/hairface/title) في وضع Dual.
                PlayerBinaryStorage.EnsureSchema();

                // المرحلة 3: مرآة الجيلد + اليونيون + الكلان (guilds/unions/clans + الجداول الفرعية).
                SocialStorage.EnsureSchema();
            }
            catch (Exception e)
            {
                _ready = false;
                try { MyConsole.WriteLine("[Storage] EnsureSchema(players) failed (falling back to files): " + e.Message, ConsoleColor.Yellow); } catch { }
            }
        }

        private static string PlayerFile(uint uid)
        {
            return Program.ServerConfig.DbLocation + "\\Users\\" + uid + ".ini";
        }

        // يحوّل قيمة INI نصية لقيمة صالحة للعمود حسب نوعه
        private static string Coerce(string val, int kind)
        {
            if (val == null) val = "";
            val = val.Trim();
            if (kind == KIND_TEXT) return val;

            // bool نصي
            if (val.Equals("True", StringComparison.OrdinalIgnoreCase)) return "1";
            if (val.Equals("False", StringComparison.OrdinalIgnoreCase)) return "0";
            if (val.Length == 0) return "0";

            if (kind == KIND_REAL)
            {
                // لو الثقافة كتبت فاصلة عشرية "," بدّلها بنقطة عشان MySQL
                if (val.IndexOf(',') >= 0 && val.IndexOf('.') < 0) val = val.Replace(',', '.');
                double d;
                return double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? val : "0";
            }

            // KIND_INT: بنسيب الرقم كنص (يحافظ على دقة BIGINT UNSIGNED الكاملة)، والفاضي/غير الصالح -> 0
            double chk;
            if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out chk)) return val;
            return "0";
        }

        // يقرأ ملف الـ ini ويطابق كل مفتاح بعموده الواضح عبر خريطة الربط
        private static Dictionary<string, string> ParseIniToColumns(string path)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string section = "";
            foreach (string raw in System.IO.File.ReadAllLines(path))
            {
                string t = raw.Trim();
                if (t.Length == 0 || t[0] == ';' || t[0] == '#') continue;
                if (t[0] == '[' && t.EndsWith("]")) { section = t.Substring(1, t.Length - 2).Trim(); continue; }
                int eq = t.IndexOf('=');
                if (eq <= 0) continue;
                string key = t.Substring(0, eq).Trim();     // يعالج المسافة البادئة زي " TableBetDice"
                string val = t.Substring(eq + 1);
                if (key.Length == 0) continue;

                // اسم المفتاح القديم: للأقسام غير Character بادئة section_key أولًا، ثم المفتاح نفسه
                string oldKey = null;
                bool isChar = section.Equals("Character", StringComparison.OrdinalIgnoreCase) || section.Length == 0;
                if (!isChar && _rename.ContainsKey(section + "_" + key)) oldKey = section + "_" + key;
                else if (_rename.ContainsKey(key)) oldKey = key;
                if (oldKey == null) continue;

                string clean = _rename[oldKey];
                int kind;
                if (!_cols.TryGetValue(clean, out kind)) continue;
                result[clean] = Coerce(val, kind);          // آخر قيمة تكسب لو اتكرر المفتاح
            }
            return result;
        }

        /// <summary>آخر SaveClient: الملف اتكتب بالكامل. نملأ صف players بعمود واضح لكل حقل.</summary>
        public static void OnSaved(Client.GameClient client)
        {
            if (Backend == Mode.File || !_ready || client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            string name = client.Player.Name ?? "None";
            // قراءة الملف هنا (ثريد المنادي)، والـ UPSERT في الخلفية لو [Storage] AsyncMirror=1
            MirrorQueue.Submit("players:" + uid, "save", false,
                () => PrepareUpsertFromIni(uid, name, false),
                "OnSaved(" + uid + ") players UPSERT failed (file is still saved)");
        }

        /// <summary>
        /// بعد أي كتابة مباشرة على Users\&lt;uid&gt;.ini من غير SaveClient (لاعب أوفلاين:
        /// UpdateGuildMember/UpdateUnionMember/UpdateClanMember/DestroySpouse/UpdateSpouse/ChatBanned...).
        /// يعيد قراءة الملف ويعمل نفس UPSERT بتاع OnSaved بالظبط.
        /// لو الملف مش موجود أو ناقص فيه Name (WritePrivateProfileString بيخلق ملف جديد لـ uid محذوف)
        /// بنتجاهله عشان ما نعملش صف يتيم.
        /// </summary>
        public static void OnIniChanged(uint uid)
        {
            if (Backend == Mode.File || !_ready || uid == 0) return;
            MirrorQueue.Submit("players:" + uid, "save", false,
                () => PrepareUpsertFromIni(uid, null, true),
                "OnIniChanged(" + uid + ") players UPSERT failed (file is still saved)");
        }

        // قراءة الملف + تجهيز الـ UPSERT (مشتركة بين OnSaved و OnIniChanged). القراءة بتحصل دلوقتي،
        // والـ Action اللي راجعة فيها شغل القاعدة بس (null = مفيش حاجة تتعمل). بترمي الاستثناء للمنادي.
        private static Action PrepareUpsertFromIni(uint uid, string fallbackName, bool requireName)
        {
            string path = PlayerFile(uid);
            if (!System.IO.File.Exists(path)) return null;

            var vals = ParseIniToColumns(path);
            if (requireName && !vals.ContainsKey("Name")) return null;
            vals["UID"] = uid.ToString();                       // المفتاح الأساسي لازم موجود
            if (!vals.ContainsKey("Name")) vals["Name"] = fallbackName ?? "None";
            if (vals.Count == 0) return null;

            var colList = new List<string>(vals.Keys);
            var insCols = new StringBuilder();
            var insVals = new StringBuilder();
            var updSet = new StringBuilder();
            for (int i = 0; i < colList.Count; i++)
            {
                if (i > 0) { insCols.Append(','); insVals.Append(','); }
                insCols.Append('`').Append(colList[i]).Append('`');
                insVals.Append("@p").Append(i);
                if (!colList[i].Equals("UID", StringComparison.OrdinalIgnoreCase))
                {
                    if (updSet.Length > 0) updSet.Append(',');
                    updSet.Append('`').Append(colList[i]).Append("`=VALUES(`").Append(colList[i]).Append("`)");
                }
            }
            string sql = "INSERT INTO players (" + insCols + ") VALUES (" + insVals + ") ON DUPLICATE KEY UPDATE " + updSet + ";";

            return () =>
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        for (int i = 0; i < colList.Count; i++)
                            cmd.Parameters.AddWithValue("@p" + i, vals[colList[i]]);
                        cmd.ExecuteNonQuery();
                    }
                }
            };
        }

        /// <summary>يُنادى عند حذف الشخصية ليحذف صفها من players (الملف بيتمسح، فالصف ميبقاش يتيم).</summary>
        public static void OnDeleted(uint uid)
        {
            if (Backend == Mode.File || !_ready) return;
            // supersede: أي حفظ منتظر لنفس اللاعب قبل الحذف بيتلغي -> النتيجة النهائية حذف
            MirrorQueue.Submit("players:" + uid, "delete", true,
                () => () =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM players WHERE UID=@uid;";
                            cmd.Parameters.AddWithValue("@uid", uid);
                            cmd.ExecuteNonQuery();
                        }
                    }
                },
                "OnDeleted(" + uid + ") failed");
        }

        // ================================================================== dashboard helpers [2026-10-17]
        // Keep players.Online = 1/0 reconciled with the live GamePoll so the flag is queryable straight from
        // MySQL (SELECT COUNT(*) FROM players WHERE Online=1) and feeds the dashboard player list. Runs on
        // its own background thread; it never touches the login/logout code paths.
        private static bool _onlineSyncStarted;

        public static void StartOnlineSync()
        {
            if (_onlineSyncStarted || Backend == Mode.File) return;
            _onlineSyncStarted = true;
            var t = new System.Threading.Thread(OnlineSyncLoop) { IsBackground = true, Name = "OnlineSync" };
            t.Start();
        }

        private static void OnlineSyncLoop()
        {
            while (true)
            {
                try
                {
                    var ids = new List<uint>();
                    foreach (var c in Pool.GamePoll.Values)
                        if (c != null && !c.Fake && c.Player != null && c.FullLoading) ids.Add(c.Player.UID);
                    SyncOnline(ids);
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                System.Threading.Thread.Sleep(15000);
            }
        }

        /// <summary>Reconcile players.Online with the set of currently-connected UIDs.</summary>
        public static void SyncOnline(ICollection<uint> onlineUids)
        {
            if (Backend == Mode.File || !_ready) return;
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    if (onlineUids == null || onlineUids.Count == 0)
                    {
                        cmd.CommandText = "UPDATE `players` SET `Online`=0 WHERE `Online`<>0;";
                        cmd.ExecuteNonQuery();
                        return;
                    }
                    var sb = new StringBuilder();
                    foreach (uint u in onlineUids) { if (sb.Length > 0) sb.Append(','); sb.Append(u.ToString(CultureInfo.InvariantCulture)); }
                    string ids = sb.ToString();
                    cmd.CommandText = "UPDATE `players` SET `Online`=1 WHERE `Online`<>1 AND `UID` IN (" + ids + ");";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE `players` SET `Online`=0 WHERE `Online`<>0 AND `UID` NOT IN (" + ids + ");";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Total rows in players (the whole roster).</summary>
        public static int CountPlayers()
        {
            if (Backend == Mode.File || !_ready) return 0;
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM `players`;";
                        object o = cmd.ExecuteScalar();
                        return o == null ? 0 : Convert.ToInt32(o, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch { return 0; }
        }

        /// <summary>Dashboard player list straight from MySQL, online-first then by level, with the Online flag.</summary>
        public static List<Dictionary<string, object>> ListPlayers(string search, int limit)
        {
            var list = new List<Dictionary<string, object>>();
            if (Backend == Mode.File || !_ready) return list;
            if (limit <= 0) limit = 100;
            if (limit > 500) limit = 500;
            bool hasSearch = !string.IsNullOrEmpty(search);
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT `UID`,`Name`,`Level`,`Class`,`RebornCount`,`VipLevel`,`ConquerPoints`,`BoundConquerPoints`,`Money`,`Online` FROM `players` " +
                        (hasSearch ? "WHERE `Name` LIKE @s " : "") +
                        "ORDER BY `Online` DESC, `Level` DESC LIMIT " + limit + ";";
                    if (hasSearch) cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var d = new Dictionary<string, object>();
                            d["uid"] = (uint)ToL(r, 0);
                            d["name"] = r.IsDBNull(1) ? "" : r.GetString(1);
                            d["level"] = (int)ToL(r, 2);
                            d["cls"] = (int)ToL(r, 3);
                            d["reborn"] = (int)ToL(r, 4);
                            d["vip"] = (int)ToL(r, 5);
                            d["cps"] = ToL(r, 6);
                            d["boundCps"] = ToL(r, 7);
                            d["money"] = ToL(r, 8);
                            d["online"] = ToL(r, 9) != 0;
                            list.Add(d);
                        }
                    }
                }
            }
            return list;
        }

        private static long ToL(System.Data.IDataRecord r, int i)
        {
            try { return r.IsDBNull(i) ? 0 : Convert.ToInt64(r.GetValue(i), CultureInfo.InvariantCulture); }
            catch { return 0; }
        }

        public static long DbLoads, FileFallbacks, VerifyLogins, VerifyMismatches;

        /// <summary>بداية LoadCharacter: في وضع Verify/Db بيسجّل صف اللاعب كـ overlay لملف الـ ini بتاعه.</summary>
        public static void BeforeLoad(uint uid)
        {
            if (ReadSource == ReadMode.File || !_ready || uid == 0) return;
            string why = null;
            try
            {
                if (MirrorQueue.IsDirty("players:" + uid)) why = "DB copy not up to date yet (pending/failed save)";
                else
                {
                    var row = LoadRow(uid);
                    if (row == null) why = "no players row";
                    else
                    {
                        WindowsAPI.IniFile.SetOverlay(PlayerFile(uid), new RowOverlay(uid, row), ReadSource == ReadMode.Verify);
                        if (ReadSource == ReadMode.Db) System.Threading.Interlocked.Increment(ref DbLoads);
                        else System.Threading.Interlocked.Increment(ref VerifyLogins);
                        return;
                    }
                }
            }
            catch (Exception e) { why = "DB error: " + e.Message; }
            System.Threading.Interlocked.Increment(ref FileFallbacks);
            VerifyLog("uid " + uid + ": reading from FILE (" + why + ")");
        }

        /// <summary>نهاية LoadCharacter (finally): يشيل الـ overlay ويكتب ملخّص التحقق.</summary>
        public static void AfterLoad(uint uid)
        {
            if (ReadSource == ReadMode.File || uid == 0) return;
            WindowsAPI.IniFile.ClearOverlay(PlayerFile(uid));
        }

        private static Dictionary<string, string> LoadRow(uint uid)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM players WHERE UID=@uid;";
                    cmd.Parameters.AddWithValue("@uid", uid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;
                        var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            if (r.IsDBNull(i)) continue;          // NULL = مفيش قيمة -> المفتاح يتقري من الملف
                            object v = r.GetValue(i);
                            row[r.GetName(i)] = v is IFormattable ? ((IFormattable)v).ToString(null, CultureInfo.InvariantCulture) : Convert.ToString(v);
                        }
                        return row;
                    }
                }
            }
        }

        // نفس منطق ParseIniToColumns بالظبط بس بالعكس: (section, key) -> اسم العمود
        private static string ColumnFor(string section, string key)
        {
            if (section == null || key == null) return null;
            key = key.Trim();
            bool isChar = section.Equals("Character", StringComparison.OrdinalIgnoreCase) || section.Length == 0;
            string oldKey = null;
            if (!isChar && _rename.ContainsKey(section + "_" + key)) oldKey = section + "_" + key;
            else if (_rename.ContainsKey(key)) oldKey = key;
            if (oldKey == null) return null;
            string clean = _rename[oldKey];
            return _cols.ContainsKey(clean) ? clean : null;
        }

        private sealed class RowOverlay : WindowsAPI.IniFile.IReadOverlay
        {
            private readonly uint uid;
            private readonly Dictionary<string, string> row;
            private readonly HashSet<string> written = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public RowOverlay(uint uid, Dictionary<string, string> row) { this.uid = uid; this.row = row; }
            // LoadCharacter بتكتب في الملف أثناء التحميل (مثلًا Online=1) -> المفتاح ده يتقري من الملف بعد كده
            public void OnWrite(string section, string key)
            {
                lock (written) written.Add((section ?? "") + "|" + (key ?? ""));
            }
            public bool TryGet(string section, string key, out string value)
            {
                value = null;
                lock (written) if (written.Count != 0 && written.Contains((section ?? "") + "|" + (key ?? ""))) return false;
                string col = ColumnFor(section, key);
                if (col == null || col.Equals("UID", StringComparison.OrdinalIgnoreCase)) return false;
                return row.TryGetValue(col, out value);
            }
            public void OnMismatch(string section, string key, string fileValue, string overlayValue)
            {
                System.Threading.Interlocked.Increment(ref VerifyMismatches);
                VerifyLog("uid " + uid + " MISMATCH [" + section + "] " + key + " (col " + ColumnFor(section, key) + "): file='" + fileValue + "' db='" + overlayValue + "'");
            }
        }

        private static readonly object verifyLock = new object();
        private static void VerifyLog(string line)
        {
            try
            {
                lock (verifyLock)
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "storage_verify.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine);
            }
            catch { }
        }
    }
}
