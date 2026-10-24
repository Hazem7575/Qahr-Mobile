// [GiftCenter 2026-10-14] Gift Center data: config, catalog (client hairface_storage_type) and the persistent record store.
//
// Files (Data\GiftCenter\):
//   GiftCenter.ini   config (official values from the client's Face.lua / hairface_storage_type; derived values marked)
//   records.txt      every record + per-player settings (atomic rewrite on each change)
//   _claims.log      append-only ledger of claimed record ids (a claim survives a rollback of records.txt)
//   logs\gifts_yyyy-MM-dd.log   human-readable action log
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.GiftCenter
{
    /// <summary>List the record lives in (the client's list type in 2050 f1).</summary>
    public enum GcList : int { Sent = 0, Asks = 1, Received = 2 }

    public sealed class GcRecord
    {
        public uint Id;
        public uint Owner;          // whose list holds it
        public GcList List;
        public uint OtherUid;       // receiver (sent log) / sender (received, asks)
        public string OtherName = "";
        public uint OtherMesh;
        public uint Kind;           // 1 avatar, 2 frame
        public uint Item;           // hairface id
        public uint Text;           // STR_GIFT_TO_FRIEND_TEXT%d
        public long Created;        // unix seconds
        public bool Claimed;
        public bool New = true;
    }

    public sealed class GcPlayer
    {
        public uint Uid;
        public bool RejectAsks;
        public int AskDay;          // yyyymmdd of AskCount
        public uint AskCount;
    }

    public sealed class GcItem
    {
        public uint Kind;           // 1 avatar, 2 frame
        public uint Id;
        public string Name;
        public bool PayCps;         // money type 1 = CPs, otherwise Silver
        public ulong Price;
    }

    public sealed class GcConfig
    {
        public bool Enabled = true;
        public uint AskLimitPerDay = 10;           // official: Face.lua Face_GetAskTimesLimitOneDay
        public int RecordLifetimeDays = 30;        // official: Face.lua Face_GetLogSaveTime (2592000 s)
        public ulong MinAvatarPrice = 10000000;    // official: Face.lua Face_GetFaceLimitMoney
        public uint DuplicateAvatarItem = 3300136; // official: Face.lua (already owned -> item)
        public uint DuplicateFrameItem = 3312060;  // official: Face.lua
        public int MaxReceived = 100;              // derived
        public int MaxAsks = 100;                  // derived
        public int MaxSentLog = 100;               // derived
        public bool RequireFriend = true;          // derived (the client only offers friends)
        public bool RequireTargetOnline = true;    // derived (STR_GIFT_TO_FRIEND_ERROR_4 "The player is offline.")
        public int ActionIntervalMs = 800;         // derived
        public int ListIntervalMs = 300;           // derived
    }

    public static class GiftCenterStore
    {
        public static readonly object Sync = new object();
        public static GcConfig Config = new GcConfig();
        public static readonly Dictionary<uint, GcItem> Catalog = new Dictionary<uint, GcItem>();// key = kind*1000000 + id
        public static readonly Dictionary<uint, GcRecord> Records = new Dictionary<uint, GcRecord>();
        public static readonly Dictionary<uint, GcPlayer> Players = new Dictionary<uint, GcPlayer>();
        static readonly HashSet<uint> claimedLedger = new HashSet<uint>();
        public static uint NextId = 1;
        public static bool Loaded;
        public static string LoadError;

        public static string Dir
        {
            get
            {
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "GiftCenter";
            }
        }
        static string RecordsFile { get { return Path.Combine(Dir, "records.txt"); } }
        static string LedgerFile { get { return Path.Combine(Dir, "_claims.log"); } }

        public static uint Key(uint kind, uint id) { return kind * 1000000u + id; }

        // ------------------------------------------------------------------ load
        public static void EnsureLoaded()
        {
            if (Loaded) return;
            lock (Sync)
            {
                if (Loaded) return;
                try
                {
                    Directory.CreateDirectory(Dir);
                    LoadConfig();
                    LoadCatalog();
                    LoadLedger();
                    LoadRecords();
                    LoadError = null;
                }
                catch (Exception e)
                {
                    LoadError = e.Message;
                    Config.Enabled = false;
                    Log("LOAD FAILED (gift center disabled): " + e.Message);
                }
                Loaded = true;
            }
        }

        /// <summary>Test / reload helper: forget everything and read the files again.</summary>
        public static void Reload()
        {
            lock (Sync)
            {
                Loaded = false;
                Config = new GcConfig();
                Catalog.Clear(); Records.Clear(); Players.Clear(); claimedLedger.Clear(); NextId = 1;
            }
            EnsureLoaded();
        }

        static void LoadConfig()
        {
            string path = Path.Combine(Dir, "GiftCenter.ini");
            var c = new GcConfig();
            if (File.Exists(path))
            {
                foreach (string raw in File.ReadAllLines(path, Encoding.UTF8))
                {
                    string line = raw;
                    int sc = line.IndexOf(';'); if (sc >= 0) line = line.Substring(0, sc);
                    line = line.Trim();
                    if (line.Length == 0 || line.StartsWith("[")) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                    long n; long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n);
                    switch (k)
                    {
                        case "enabled": c.Enabled = n != 0; break;
                        case "asklimitperday": c.AskLimitPerDay = (uint)Clamp(n, 0, 1000); break;
                        case "recordlifetimedays": c.RecordLifetimeDays = (int)Clamp(n, 1, 3650); break;
                        case "minavatarprice": c.MinAvatarPrice = (ulong)Clamp(n, 0, long.MaxValue); break;
                        case "duplicateavataritem": c.DuplicateAvatarItem = (uint)Clamp(n, 0, uint.MaxValue); break;
                        case "duplicateframeitem": c.DuplicateFrameItem = (uint)Clamp(n, 0, uint.MaxValue); break;
                        case "maxreceived": c.MaxReceived = (int)Clamp(n, 1, 10000); break;
                        case "maxasks": c.MaxAsks = (int)Clamp(n, 1, 10000); break;
                        case "maxsentlog": c.MaxSentLog = (int)Clamp(n, 1, 10000); break;
                        case "requirefriend": c.RequireFriend = n != 0; break;
                        case "requiretargetonline": c.RequireTargetOnline = n != 0; break;
                        case "actionintervalms": c.ActionIntervalMs = (int)Clamp(n, 0, 60000); break;
                        case "listintervalms": c.ListIntervalMs = (int)Clamp(n, 0, 60000); break;
                    }
                }
            }
            Config = c;
        }

        static long Clamp(long v, long min, long max) { return v < min ? min : v > max ? max : v; }

        /// <summary>The giftable avatars / frames, with the client's own rule (FUN_008aad2c):
        /// get-way column != 6; avatars must also be Silver-priced and cost at least MinAvatarPrice.
        /// Price and money type are the client's (column 7 / column 6).</summary>
        static void LoadCatalog()
        {
            string db = Program.ServerConfig.DbLocation ?? "";
            if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
            string path = db + "hairface_storage_type.txt";
            if (!File.Exists(path)) throw new Exception("hairface_storage_type.txt missing");
            foreach (string line in File.ReadAllLines(path))
            {
                var c = line.Split(new[] { "@@" }, StringSplitOptions.None);
                if (c.Length < 8) continue;
                uint type, id, getWay, money; ulong price;
                if (!uint.TryParse(c[0].Trim(), out type) || !uint.TryParse(c[1].Trim(), out id)) continue;
                uint.TryParse(c[4].Trim(), out getWay);
                uint.TryParse(c[6].Trim(), out money);
                ulong.TryParse(c[7].Trim(), out price);
                uint kind = type == 1 ? 1u : type == 9 ? 2u : 0u;
                if (kind == 0 || getWay == 6 || price == 0) continue;
                if (kind == 1 && (money != 0 || price < Config.MinAvatarPrice)) continue;
                Catalog[Key(kind, id)] = new GcItem { Kind = kind, Id = id, Name = c[3].Trim(), PayCps = money == 1, Price = price };
            }
        }

        static void LoadLedger()
        {
            if (!File.Exists(LedgerFile)) return;
            foreach (string l in File.ReadAllLines(LedgerFile))
            {
                var p = l.Split('|');
                uint id;
                if (p.Length >= 2 && uint.TryParse(p[1], out id)) claimedLedger.Add(id);
            }
        }

        static void LoadRecords()
        {
            if (!File.Exists(RecordsFile)) return;
            foreach (string l in File.ReadAllLines(RecordsFile, Encoding.UTF8))
            {
                var p = l.Split('\t');
                if (p.Length == 0) continue;
                if (p[0] == "N" && p.Length >= 2) { uint n; if (uint.TryParse(p[1], out n) && n > NextId) NextId = n; }
                else if (p[0] == "P" && p.Length >= 5)
                {
                    var pl = new GcPlayer { Uid = uint.Parse(p[1]), RejectAsks = p[2] == "1", AskDay = int.Parse(p[3]), AskCount = uint.Parse(p[4]) };
                    Players[pl.Uid] = pl;
                }
                else if (p[0] == "R" && p.Length >= 14)
                {
                    var r = new GcRecord
                    {
                        Id = uint.Parse(p[1]), Owner = uint.Parse(p[2]), List = (GcList)int.Parse(p[3]),
                        OtherUid = uint.Parse(p[4]), OtherName = p[5], OtherMesh = uint.Parse(p[6]), Kind = uint.Parse(p[7]),
                        Item = uint.Parse(p[8]), Text = uint.Parse(p[9]), Created = long.Parse(p[10]), Claimed = p[11] == "1", New = p[12] == "1"
                    };
                    if (claimedLedger.Contains(r.Id)) r.Claimed = true;// a rolled-back file can never re-open a claim
                    Records[r.Id] = r;
                    if (r.Id >= NextId) NextId = r.Id + 1;
                }
            }
            foreach (uint id in claimedLedger) if (id >= NextId) NextId = id + 1;
        }

        // ------------------------------------------------------------------ save (caller holds Sync)
        public static void Save()
        {
            var sb = new StringBuilder();
            sb.Append("N\t").Append(NextId).Append('\n');
            foreach (var pl in Players.Values)
                sb.Append("P\t").Append(pl.Uid).Append('\t').Append(pl.RejectAsks ? 1 : 0).Append('\t').Append(pl.AskDay).Append('\t').Append(pl.AskCount).Append('\n');
            foreach (var r in Records.Values)
                sb.Append("R\t").Append(r.Id).Append('\t').Append(r.Owner).Append('\t').Append((int)r.List).Append('\t').Append(r.OtherUid).Append('\t')
                  .Append(Clean(r.OtherName)).Append('\t').Append(r.OtherMesh).Append('\t').Append(r.Kind).Append('\t').Append(r.Item).Append('\t')
                  .Append(r.Text).Append('\t').Append(r.Created).Append('\t').Append(r.Claimed ? 1 : 0).Append('\t').Append(r.New ? 1 : 0).Append("\t.\n");
            Directory.CreateDirectory(Dir);
            string tmp = RecordsFile + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            if (File.Exists(RecordsFile)) File.Replace(tmp, RecordsFile, RecordsFile + ".bak");
            else File.Move(tmp, RecordsFile);
        }

        static string Clean(string s) { return (s ?? "").Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '); }

        public static bool IsClaimedInLedger(uint id) { return claimedLedger.Contains(id); }

        /// <summary>Writes the claim to the append-only ledger BEFORE anything is granted (caller holds Sync).</summary>
        public static void LedgerClaim(GcRecord r)
        {
            Directory.CreateDirectory(Dir);
            File.AppendAllText(LedgerFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "|" + r.Id + "|" + r.Owner + "|" + r.Kind + "|" + r.Item + Environment.NewLine, Encoding.UTF8);
            claimedLedger.Add(r.Id);
        }

        // ------------------------------------------------------------------ log
        static readonly object logLock = new object();
        public static void Log(string text)
        {
            try
            {
                string dir = Path.Combine(Dir, "logs");
                lock (logLock)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "gifts_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
