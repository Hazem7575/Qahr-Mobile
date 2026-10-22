// ============================================================================
//  [EventData 2026-10-15] EventStore: ONE store for every event / festival player state, kept in MySQL.
//
//  Tables (created at first use, InnoDB utf8mb4):
//    event_data   (uid, event, `key`, value BIGINT, text VARCHAR(1024) NULL, updated)   PK(uid,event,key)
//                 key = "c:<name>" counter | "f:<name>" flag (value 1) | "t:<name>" time/day | "s:<name>" string (text)
//    event_claims (uid, event, claim_key, claimed_at)                                     PK(uid,event,claim_key)
//  uid 0 = server-wide sections (rankings, global pools, the "_eventdata" migration flags).
//
//  Cache  : one record per uid in memory (loaded at login / first use, dropped when idle and clean).
//  Writes : counters/flags/strings/times are batched (dirty keys) and flushed on SaveClient, logout and every
//           FlushMs; Edit(..., flushNow:true) writes before it returns. Claims are WRITE-THROUGH:
//           TryClaimOnce = INSERT IGNORE on the primary key -> atomic and crash-safe in the DB itself.
//  DB down: loud log; the record does not load, edits throw EditFailed, TryClaimOnce returns false
//           => nothing is ever granted without a claim recorded in MySQL. Parameterized queries only.
//  Hooks  : ServerDatabase.LoadCharacterCore -> OnLogin, ServerDatabase.SaveClient -> OnSave, Program.Game_Disconnect -> OnLogout.
//  Legacy : MigrateKvText / MigrateJsonPlayers import an old per-festival file once, then rename it to *.migrated.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MySqlConnector;

namespace Qahr.Mobile.EventData
{
    /// <summary>One event's data for one player. Only touch it inside EventStore.Edit (or on a copy from Section()).</summary>
    public sealed class EventSection
    {
        public readonly Dictionary<string, long> Counters = new Dictionary<string, long>(StringComparer.Ordinal);
        public readonly HashSet<string> Flags = new HashSet<string>(StringComparer.Ordinal);
        public readonly Dictionary<string, long> Claims = new Dictionary<string, long>(StringComparer.Ordinal);
        public readonly Dictionary<string, long> Times = new Dictionary<string, long>(StringComparer.Ordinal);
        public readonly Dictionary<string, string> Strings = new Dictionary<string, string>(StringComparer.Ordinal);

        public bool IsEmpty { get { return Counters.Count == 0 && Flags.Count == 0 && Claims.Count == 0 && Times.Count == 0 && Strings.Count == 0; } }
        public long Counter(string k) { long v; return Counters.TryGetValue(k, out v) ? v : 0; }
        public string Str(string k, string def = "") { string v; return Strings.TryGetValue(k, out v) ? v : def; }

        public EventSection Clone()
        {
            var s = new EventSection();
            foreach (var kv in Counters) s.Counters[kv.Key] = kv.Value;
            foreach (var f in Flags) s.Flags.Add(f);
            foreach (var kv in Claims) s.Claims[kv.Key] = kv.Value;
            foreach (var kv in Times) s.Times[kv.Key] = kv.Value;
            foreach (var kv in Strings) s.Strings[kv.Key] = kv.Value;
            return s;
        }

        /// <summary>Row view (key -> (value,text)) of everything except claims.</summary>
        internal Dictionary<string, Tuple<long, string>> Rows()
        {
            var d = new Dictionary<string, Tuple<long, string>>(StringComparer.Ordinal);
            foreach (var kv in Counters) d["c:" + kv.Key] = Tuple.Create(kv.Value, (string)null);
            foreach (var f in Flags) d["f:" + f] = Tuple.Create(1L, (string)null);
            foreach (var kv in Times) d["t:" + kv.Key] = Tuple.Create(kv.Value, (string)null);
            foreach (var kv in Strings) d["s:" + kv.Key] = Tuple.Create(0L, kv.Value ?? "");
            return d;
        }
        internal void ApplyRow(string key, long value, string text)
        {
            if (key == null || key.Length < 3 || key[1] != ':') return;
            string n = key.Substring(2);
            switch (key[0])
            {
                case 'c': Counters[n] = value; break;
                case 'f': if (value != 0) Flags.Add(n); break;
                case 't': Times[n] = value; break;
                case 's': Strings[n] = text ?? ""; break;
            }
        }

        public Dictionary<string, object> ToJson()
        {
            var d = new Dictionary<string, object>();
            d["c"] = Counters.OrderBy(k => k.Key, StringComparer.Ordinal).ToDictionary(k => k.Key, k => (object)k.Value);
            d["f"] = Flags.OrderBy(k => k, StringComparer.Ordinal).Cast<object>().ToList();
            d["cl"] = Claims.OrderBy(k => k.Key, StringComparer.Ordinal).ToDictionary(k => k.Key, k => (object)k.Value);
            d["t"] = Times.OrderBy(k => k.Key, StringComparer.Ordinal).ToDictionary(k => k.Key, k => (object)k.Value);
            d["s"] = Strings.OrderBy(k => k.Key, StringComparer.Ordinal).ToDictionary(k => k.Key, k => (object)k.Value);
            return d;
        }
    }

    public static class EventStore
    {
        // ------------------------------------------------------------------ config / seams
        /// <summary>Test seam: table name prefix (the harness uses a throw-away pair of tables).</summary>
        public static string TablePrefix = "";
        /// <summary>Test seam: connection string (null = the server's DataHolder.ConnectionString).</summary>
        public static string ConnectionOverride;
        /// <summary>Background flush period (ms). A crash loses at most this much of non-claim progress.</summary>
        public static int FlushMs = 3000;
        /// <summary>Test seam: false = no background timer (the harness flushes by hand).</summary>
        public static bool AutoFlush = true;
        /// <summary>Test seam: called before every DB command (throw to simulate the DB going down).</summary>
        public static Action<string> BeforeDb;

        public const int MaxEvent = 48, MaxKey = 64, MaxText = 1024;
        const string MetaEvent = "_eventdata";
        static string TData { get { return "`" + TablePrefix + "event_data`"; } }
        static string TClaims { get { return "`" + TablePrefix + "event_claims`"; } }

        sealed class Record
        {
            public readonly uint Uid;
            public readonly object Sync = new object();
            public readonly Dictionary<string, EventSection> Events = new Dictionary<string, EventSection>(StringComparer.Ordinal);
            /// <summary>event -> row keys changed since the last flush (c:/f:/t:/s:/cl:).</summary>
            public readonly Dictionary<string, HashSet<string>> Dirty = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            public bool Loaded;
            public long LastUse, LastLoadFail;
            public Record(uint uid) { Uid = uid; }
            public bool IsDirty { get { return Dirty.Count > 0; } }
        }

        static readonly ConcurrentDictionary<uint, Record> _recs = new ConcurrentDictionary<uint, Record>();
        static Timer _timer;
        static int _flushing;
        static volatile bool _schemaOk;
        static readonly object _schemaSync = new object();
        static long _lastLoudLog;

        static long Tick { get { return Environment.TickCount & 0x7FFFFFFF; } }

        static void Log(string s, bool loud = false)
        {
            try
            {
                if (loud)
                {
                    long now = Tick;
                    if (now - Interlocked.Read(ref _lastLoudLog) > 10000 || now < _lastLoudLog) { Interlocked.Exchange(ref _lastLoudLog, now); MyConsole.WriteLine("[EventData] !!! " + s, ConsoleColor.Red); }
                }
                else MyConsole.WriteLine("[EventData] " + s, ConsoleColor.Yellow);
            }
            catch { }
            try
            {
                string dir = Program.ServerConfig.DbLocation;
                if (!string.IsNullOrEmpty(dir))
                    File.AppendAllText(Path.Combine(dir, "eventdata_log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + s + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        internal static bool TryLong(string s, out long v)
        {
            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) return true;
            double d;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d) && !double.IsNaN(d) && !double.IsInfinity(d))
            { v = (long)Math.Max(long.MinValue, Math.Min(long.MaxValue, d)); return true; }
            return false;
        }

        // ------------------------------------------------------------------ DB plumbing
        static MySqlConnection Open()
        {
            var bd = BeforeDb; if (bd != null) bd("open");
            string cs = ConnectionOverride ?? DBFunctionality.DataHolder.ConnectionString;
            if (string.IsNullOrEmpty(cs)) throw new InvalidOperationException("no MySQL connection configured");
            var c = new MySqlConnection(cs);
            c.Open();
            return c;
        }
        static MySqlCommand Cmd(MySqlConnection c, MySqlTransaction tx, string sql)
        {
            var bd = BeforeDb; if (bd != null) bd(sql);
            var cmd = c.CreateCommand();
            cmd.CommandText = sql;
            if (tx != null) cmd.Transaction = tx;
            return cmd;
        }

        /// <summary>CREATE TABLE IF NOT EXISTS (once per process). Throws when the DB is unreachable.</summary>
        public static void EnsureSchema()
        {
            if (_schemaOk) return;
            lock (_schemaSync)
            {
                if (_schemaOk) return;
                using (var c = Open())
                {
                    using (var cmd = Cmd(c, null,
                        "CREATE TABLE IF NOT EXISTS " + TData + " (\n" +
                        "  `uid` INT UNSIGNED NOT NULL,\n" +
                        "  `event` VARCHAR(48) NOT NULL,\n" +
                        "  `key` VARCHAR(64) NOT NULL,\n" +
                        "  `value` BIGINT NOT NULL DEFAULT 0,\n" +
                        "  `text` VARCHAR(1024) NULL,\n" +
                        "  `updated` DATETIME NOT NULL,\n" +
                        "  PRIMARY KEY (`uid`,`event`,`key`),\n" +
                        "  KEY `ix_event_key` (`event`,`key`)\n" +
                        ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;"))
                        cmd.ExecuteNonQuery();
                    using (var cmd = Cmd(c, null,
                        "CREATE TABLE IF NOT EXISTS " + TClaims + " (\n" +
                        "  `uid` INT UNSIGNED NOT NULL,\n" +
                        "  `event` VARCHAR(48) NOT NULL,\n" +
                        "  `claim_key` VARCHAR(64) NOT NULL,\n" +
                        "  `claimed_at` DATETIME NOT NULL,\n" +
                        "  PRIMARY KEY (`uid`,`event`,`claim_key`)\n" +
                        ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;"))
                        cmd.ExecuteNonQuery();
                }
                _schemaOk = true;
            }
        }
        /// <summary>Test seam: forget that the schema exists (after the harness changes TablePrefix).</summary>
        public static void ResetSchemaForTests() { _schemaOk = false; }

        static void EnsureTimer()
        {
            if (_timer != null || !AutoFlush) return;
            lock (_schemaSync)
            {
                if (_timer != null) return;
                _timer = new Timer(_ => { try { FlushAll(); EvictIdle(); } catch { } }, null, FlushMs, FlushMs);
            }
        }

        static DateTime FromUnix(long s) { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Math.Max(0, s)).ToLocalTime(); }
        static long ToUnix(DateTime t) { return (long)(t.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }

        // ------------------------------------------------------------------ load
        static Record Rec(uint uid)
        {
            EnsureTimer();
            var r = _recs.GetOrAdd(uid, u => new Record(u));
            r.LastUse = Tick;
            if (!r.Loaded) lock (r.Sync) LoadLocked(r);
            return r;
        }

        /// <summary>Loads from MySQL (caller holds r.Sync). On failure Loaded stays false (retried after 2 s).</summary>
        static void LoadLocked(Record r)
        {
            if (r.Loaded) return;
            long now = Tick;
            if (r.LastLoadFail != 0 && now - r.LastLoadFail < 2000 && now >= r.LastLoadFail) return;
            try
            {
                EnsureSchema();
                var evs = new Dictionary<string, EventSection>(StringComparer.Ordinal);
                using (var c = Open())
                {
                    using (var cmd = Cmd(c, null, "SELECT `event`,`key`,`value`,`text` FROM " + TData + " WHERE `uid`=@u"))
                    {
                        cmd.Parameters.AddWithValue("@u", r.Uid);
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                            {
                                string ev = rd.GetString(0);
                                EventSection s; if (!evs.TryGetValue(ev, out s)) evs[ev] = s = new EventSection();
                                s.ApplyRow(rd.GetString(1), rd.GetInt64(2), rd.IsDBNull(3) ? null : rd.GetString(3));
                            }
                    }
                    using (var cmd = Cmd(c, null, "SELECT `event`,`claim_key`,`claimed_at` FROM " + TClaims + " WHERE `uid`=@u"))
                    {
                        cmd.Parameters.AddWithValue("@u", r.Uid);
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                            {
                                string ev = rd.GetString(0);
                                EventSection s; if (!evs.TryGetValue(ev, out s)) evs[ev] = s = new EventSection();
                                s.Claims[rd.GetString(1)] = ToUnix(rd.GetDateTime(2));
                            }
                    }
                }
                r.Events.Clear();
                foreach (var kv in evs) r.Events[kv.Key] = kv.Value;
                r.Dirty.Clear();
                r.Loaded = true;
                r.LastLoadFail = 0;
            }
            catch (Exception e)
            {
                r.LastLoadFail = now == 0 ? 1 : now;
                Log("MySQL unavailable - cannot load event data of uid " + r.Uid + ": " + e.Message + " (claims refused until it is back)", true);
            }
        }

        // ------------------------------------------------------------------ flush
        /// <summary>Writes the dirty keys of a record in one transaction (caller holds r.Sync). Throws on failure (still dirty).</summary>
        static void WriteLocked(Record r)
        {
            if (!r.IsDirty) return;
            EnsureSchema();
            using (var c = Open())
            using (var tx = c.BeginTransaction())
            {
                foreach (var de in r.Dirty)
                {
                    EventSection s; r.Events.TryGetValue(de.Key, out s);
                    var rows = s != null ? s.Rows() : new Dictionary<string, Tuple<long, string>>();
                    foreach (var key in de.Value)
                    {
                        if (key.StartsWith("cl:", StringComparison.Ordinal))
                        {
                            string ck = key.Substring(3); long at;
                            if (s != null && s.Claims.TryGetValue(ck, out at))
                                using (var cmd = Cmd(c, tx, "INSERT INTO " + TClaims + " (`uid`,`event`,`claim_key`,`claimed_at`) VALUES (@u,@e,@k,@t) ON DUPLICATE KEY UPDATE `claimed_at`=VALUES(`claimed_at`)"))
                                {
                                    cmd.Parameters.AddWithValue("@u", r.Uid); cmd.Parameters.AddWithValue("@e", de.Key);
                                    cmd.Parameters.AddWithValue("@k", ck); cmd.Parameters.AddWithValue("@t", FromUnix(at));
                                    cmd.ExecuteNonQuery();
                                }
                            else
                                using (var cmd = Cmd(c, tx, "DELETE FROM " + TClaims + " WHERE `uid`=@u AND `event`=@e AND `claim_key`=@k"))
                                {
                                    cmd.Parameters.AddWithValue("@u", r.Uid); cmd.Parameters.AddWithValue("@e", de.Key); cmd.Parameters.AddWithValue("@k", ck);
                                    cmd.ExecuteNonQuery();
                                }
                            continue;
                        }
                        Tuple<long, string> row;
                        if (rows.TryGetValue(key, out row))
                            using (var cmd = Cmd(c, tx, "INSERT INTO " + TData + " (`uid`,`event`,`key`,`value`,`text`,`updated`) VALUES (@u,@e,@k,@v,@x,NOW()) " +
                                                        "ON DUPLICATE KEY UPDATE `value`=VALUES(`value`),`text`=VALUES(`text`),`updated`=NOW()"))
                            {
                                cmd.Parameters.AddWithValue("@u", r.Uid); cmd.Parameters.AddWithValue("@e", de.Key); cmd.Parameters.AddWithValue("@k", key);
                                cmd.Parameters.AddWithValue("@v", row.Item1); cmd.Parameters.AddWithValue("@x", (object)row.Item2 ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        else
                            using (var cmd = Cmd(c, tx, "DELETE FROM " + TData + " WHERE `uid`=@u AND `event`=@e AND `key`=@k"))
                            {
                                cmd.Parameters.AddWithValue("@u", r.Uid); cmd.Parameters.AddWithValue("@e", de.Key); cmd.Parameters.AddWithValue("@k", key);
                                cmd.ExecuteNonQuery();
                            }
                    }
                }
                tx.Commit();
            }
            r.Dirty.Clear();
        }

        static bool TryWriteLocked(Record r)
        {
            if (!r.IsDirty) return true;
            try { WriteLocked(r); return true; }
            catch (Exception e) { Log("MySQL write of uid " + r.Uid + " failed (kept in memory, retried): " + e.Message, true); return false; }
        }

        // ------------------------------------------------------------------ core transaction
        public sealed class EditFailed : Exception { public EditFailed(string m) : base(m) { } }

        static void CheckEvent(string ev)
        {
            if (string.IsNullOrEmpty(ev) || ev.Length > MaxEvent) throw new ArgumentException("event key must be 1-" + MaxEvent + " chars");
        }

        static void Diff(EventSection a, EventSection b, HashSet<string> into)
        {
            var ra = a.Rows(); var rb = b.Rows();
            foreach (var kv in rb) { Tuple<long, string> o; if (!ra.TryGetValue(kv.Key, out o) || o.Item1 != kv.Value.Item1 || o.Item2 != kv.Value.Item2) into.Add(kv.Key); }
            foreach (var k in ra.Keys) if (!rb.ContainsKey(k)) into.Add(k);
            foreach (var kv in b.Claims) { long o; if (!a.Claims.TryGetValue(kv.Key, out o) || o != kv.Value) into.Add("cl:" + kv.Key); }
            foreach (var k in a.Claims.Keys) if (!b.Claims.ContainsKey(k)) into.Add("cl:" + k);
        }
        static void Validate(EventSection s, HashSet<string> keys)
        {
            foreach (var k in keys)
            {
                int len = k.StartsWith("cl:", StringComparison.Ordinal) ? k.Length - 3 : k.Length;
                if (len > MaxKey || len <= (k.StartsWith("cl:", StringComparison.Ordinal) ? 0 : 2)) throw new ArgumentException("event-data key too long/empty: " + k);
                if (k.StartsWith("s:", StringComparison.Ordinal)) { string v; if (s.Strings.TryGetValue(k.Substring(2), out v) && v != null && v.Length > MaxText) throw new ArgumentException("event-data text over " + MaxText + " chars: " + k); }
            }
        }

        /// <summary>Runs fn on the live section under the player's lock. fn returns (changed, result).
        /// flushNow = written to MySQL before Edit returns; if that fails the change is rolled back and EditFailed
        /// is thrown (the caller grants nothing). Throws EditFailed when the DB could not load the record.</summary>
        public static T Edit<T>(uint uid, string ev, Func<EventSection, Tuple<bool, T>> fn, bool flushNow = false)
        {
            CheckEvent(ev);
            var r = Rec(uid);
            lock (r.Sync)
            {
                LoadLocked(r);
                if (!r.Loaded) throw new EditFailed("event data of uid " + uid + " is not loaded (MySQL unavailable)");
                EventSection live;
                bool existed = r.Events.TryGetValue(ev, out live);
                var work = existed ? live.Clone() : new EventSection();
                var res = fn(work);
                if (!res.Item1) return res.Item2;
                var changed = new HashSet<string>(StringComparer.Ordinal);
                Diff(existed ? live : new EventSection(), work, changed);
                if (changed.Count == 0) return res.Item2;
                Validate(work, changed);
                // snapshot for rollback
                HashSet<string> oldDirty; r.Dirty.TryGetValue(ev, out oldDirty);
                var oldDirtyCopy = oldDirty != null ? new HashSet<string>(oldDirty, StringComparer.Ordinal) : null;
                r.Events[ev] = work;
                HashSet<string> d; if (!r.Dirty.TryGetValue(ev, out d)) r.Dirty[ev] = d = new HashSet<string>(StringComparer.Ordinal);
                d.UnionWith(changed);
                if (flushNow)
                {
                    try { WriteLocked(r); }
                    catch (Exception e)
                    {
                        if (existed) r.Events[ev] = live; else r.Events.Remove(ev);
                        if (oldDirtyCopy != null) r.Dirty[ev] = oldDirtyCopy; else r.Dirty.Remove(ev);
                        Log("MySQL sync write uid " + uid + " '" + ev + "' failed, rolled back: " + e.Message, true);
                        throw new EditFailed(e.Message);
                    }
                }
                return res.Item2;
            }
        }
        public static void Edit(uint uid, string ev, Action<EventSection> fn, bool flushNow = false)
        {
            Edit(uid, ev, s => { fn(s); return Tuple.Create(true, 0); }, flushNow);
        }

        static TR Read<TR>(uint uid, string ev, Func<EventSection, TR> fn, TR def)
        {
            var r = Rec(uid);
            lock (r.Sync)
            {
                LoadLocked(r);
                EventSection s;
                return r.Loaded && r.Events.TryGetValue(ev, out s) ? fn(s) : def;
            }
        }
        /// <summary>false = MySQL could not load this player's data (reads return defaults, edits/claims are refused).</summary>
        public static bool IsAvailable(uint uid) { var r = Rec(uid); lock (r.Sync) { LoadLocked(r); return r.Loaded; } }

        // ------------------------------------------------------------------ public API
        /// <summary>A COPY of the section (empty when the player has none).</summary>
        public static EventSection Section(uint uid, string ev) { return Read(uid, ev, s => s.Clone(), new EventSection()); }
        public static List<string> Events(uint uid)
        {
            var r = Rec(uid);
            lock (r.Sync) { LoadLocked(r); return r.Events.Where(k => !k.Value.IsEmpty).Select(k => k.Key).OrderBy(k => k, StringComparer.Ordinal).ToList(); }
        }

        public static long GetLong(uint uid, string ev, string key) { return Read(uid, ev, s => s.Counter(key), 0L); }
        public static int GetInt(uint uid, string ev, string key) { return (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, GetLong(uid, ev, key))); }
        public static void SetLong(uint uid, string ev, string key, long v, long min = long.MinValue, long max = long.MaxValue, bool flushNow = false)
        {
            v = Math.Max(min, Math.Min(max, v));
            Edit(uid, ev, s => { if (v == 0) s.Counters.Remove(key); else s.Counters[key] = v; }, flushNow);
        }
        public static void SetInt(uint uid, string ev, string key, int v, int min = int.MinValue, int max = int.MaxValue, bool flushNow = false) { SetLong(uid, ev, key, v, min, max, flushNow); }
        /// <summary>Adds delta and clamps to [min,max] (overflow-safe). Returns the new value.</summary>
        public static long AddLong(uint uid, string ev, string key, long delta, long min = long.MinValue, long max = long.MaxValue, bool flushNow = false)
        {
            return Edit(uid, ev, s =>
            {
                long cur = s.Counter(key), n;
                try { n = checked(cur + delta); } catch (OverflowException) { n = delta > 0 ? long.MaxValue : long.MinValue; }
                n = Math.Max(min, Math.Min(max, n));
                if (n == 0) s.Counters.Remove(key); else s.Counters[key] = n;
                return Tuple.Create(n != cur, n);
            }, flushNow);
        }
        public static int AddInt(uint uid, string ev, string key, int delta, int min = int.MinValue, int max = int.MaxValue, bool flushNow = false)
        {
            return (int)AddLong(uid, ev, key, delta, min, max, flushNow);
        }

        public static bool Flag(uint uid, string ev, string key) { return Read(uid, ev, s => s.Flags.Contains(key), false); }
        public static void Flag(uint uid, string ev, string key, bool on, bool flushNow = false)
        {
            Edit(uid, ev, s => { bool ch = on ? s.Flags.Add(key) : s.Flags.Remove(key); return Tuple.Create(ch, 0); }, flushNow);
        }

        public static string GetString(uint uid, string ev, string key, string def = "") { return Read(uid, ev, s => s.Str(key, def), def); }
        /// <summary>null = remove.</summary>
        public static void SetString(uint uid, string ev, string key, string v, bool flushNow = false)
        {
            Edit(uid, ev, s => { if (v == null) s.Strings.Remove(key); else s.Strings[key] = v; }, flushNow);
        }

        public static long GetTime(uint uid, string ev, string key) { return Read(uid, ev, s => { long v; return s.Times.TryGetValue(key, out v) ? v : 0; }, 0L); }
        public static void SetTime(uint uid, string ev, string key, long unixSec, bool flushNow = false) { Edit(uid, ev, s => { if (unixSec == 0) s.Times.Remove(key); else s.Times[key] = unixSec; }, flushNow); }
        public static long Now { get { return ToUnix(DateTime.UtcNow); } }
        public static long Today { get { return long.Parse(DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); } }

        // ------------------------------------------------------------------ claims (write-through, atomic in MySQL)
        public static bool IsClaimed(uint uid, string ev, string claimKey) { return Read(uid, ev, s => s.Claims.ContainsKey(claimKey), false); }
        /// <summary>INSERT IGNORE on the claim's primary key; true only for the call that inserted the row.
        /// Pattern: check bag space -> TryClaimOnce -> grant. A crash after the claim loses the reward but never
        /// duplicates it. false = already claimed, MySQL unavailable, or a bad key (nothing recorded => grant nothing).</summary>
        public static bool TryClaimOnce(uint uid, string ev, string claimKey)
        {
            if (string.IsNullOrEmpty(ev) || ev.Length > MaxEvent || string.IsNullOrEmpty(claimKey) || claimKey.Length > MaxKey) { Log("TryClaimOnce: bad key '" + ev + "'/'" + claimKey + "'"); return false; }
            var r = Rec(uid);
            lock (r.Sync)
            {
                LoadLocked(r);
                if (!r.Loaded) { Log("claim REFUSED (MySQL unavailable): uid " + uid + " " + ev + "/" + claimKey, true); return false; }
                EventSection s;
                if (r.Events.TryGetValue(ev, out s) && s.Claims.ContainsKey(claimKey)) return false;
                int n;
                long now = Now;
                try
                {
                    EnsureSchema();
                    using (var c = Open())
                    using (var cmd = Cmd(c, null, "INSERT IGNORE INTO " + TClaims + " (`uid`,`event`,`claim_key`,`claimed_at`) VALUES (@u,@e,@k,@t)"))
                    {
                        cmd.Parameters.AddWithValue("@u", uid); cmd.Parameters.AddWithValue("@e", ev);
                        cmd.Parameters.AddWithValue("@k", claimKey); cmd.Parameters.AddWithValue("@t", FromUnix(now));
                        n = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e) { Log("claim REFUSED (MySQL error): uid " + uid + " " + ev + "/" + claimKey + ": " + e.Message, true); return false; }
                if (s == null) r.Events[ev] = s = new EventSection();
                s.Claims[claimKey] = now;   // either we inserted it or it was already there (another process / stale cache)
                return n == 1;
            }
        }
        /// <summary>Undo a claim when the grant was refused BEFORE anything was given (e.g. the bag filled up).</summary>
        public static bool Unclaim(uint uid, string ev, string claimKey)
        {
            try { return Edit(uid, ev, s => { bool had = s.Claims.Remove(claimKey); return Tuple.Create(had, had); }, true); }
            catch (EditFailed) { return false; }
        }

        // ------------------------------------------------------------------ daily helpers
        /// <summary>A counter that is 0 again every new day (its day is kept in Times[key+"@day"]).</summary>
        public static long GetDaily(uint uid, string ev, string key)
        {
            long today = Today;
            return Read(uid, ev, s => { long d; return s.Times.TryGetValue(key + "@day", out d) && d == today ? s.Counter(key) : 0; }, 0L);
        }
        /// <summary>Adds to a daily counter (reset first when the day changed), clamped to [0,max]. Returns the new value.</summary>
        public static long AddDaily(uint uid, string ev, string key, long delta, long max = long.MaxValue, bool flushNow = false)
        {
            long today = Today;
            return Edit(uid, ev, s =>
            {
                long d, cur = 0;
                if (s.Times.TryGetValue(key + "@day", out d) && d == today) cur = s.Counter(key);
                long n = Math.Max(0, Math.Min(max, cur + delta));
                s.Times[key + "@day"] = today;
                if (n == 0) s.Counters.Remove(key); else s.Counters[key] = n;
                return Tuple.Create(true, n);
            }, flushNow);
        }
        /// <summary>Once per day: removes the counters/flags/claims whose key starts with one of the prefixes
        /// (no prefixes = all of them). Returns true when a reset happened.</summary>
        public static bool DailyReset(uint uid, string ev, params string[] prefixes)
        {
            long today = Today;
            return Edit(uid, ev, s =>
            {
                long d;
                if (s.Times.TryGetValue("@day", out d) && d == today) return Tuple.Create(false, false);
                Func<string, bool> hit = k => prefixes == null || prefixes.Length == 0 || prefixes.Any(p => k.StartsWith(p, StringComparison.Ordinal));
                foreach (var k in s.Counters.Keys.Where(hit).ToList()) s.Counters.Remove(k);
                foreach (var k in s.Claims.Keys.Where(hit).ToList()) s.Claims.Remove(k);
                s.Flags.RemoveWhere(k => hit(k));
                s.Times["@day"] = today;
                return Tuple.Create(true, true);
            });
        }

        // ------------------------------------------------------------------ enumeration (rankings)
        /// <summary>Every player (uid != 0) with data in this event, as copies. Flushes first, reads MySQL in two
        /// queries, then overlays the cached records (fresher if a flush is pending). Empty when MySQL is down.</summary>
        public static Dictionary<uint, EventSection> All(string ev)
        {
            CheckEvent(ev);
            FlushAll();
            var res = new Dictionary<uint, EventSection>();
            try
            {
                EnsureSchema();
                using (var c = Open())
                {
                    using (var cmd = Cmd(c, null, "SELECT `uid`,`key`,`value`,`text` FROM " + TData + " WHERE `event`=@e AND `uid`<>0"))
                    {
                        cmd.Parameters.AddWithValue("@e", ev);
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                            {
                                uint uid = Convert.ToUInt32(rd.GetValue(0));
                                EventSection s; if (!res.TryGetValue(uid, out s)) res[uid] = s = new EventSection();
                                s.ApplyRow(rd.GetString(1), rd.GetInt64(2), rd.IsDBNull(3) ? null : rd.GetString(3));
                            }
                    }
                    using (var cmd = Cmd(c, null, "SELECT `uid`,`claim_key`,`claimed_at` FROM " + TClaims + " WHERE `event`=@e AND `uid`<>0"))
                    {
                        cmd.Parameters.AddWithValue("@e", ev);
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                            {
                                uint uid = Convert.ToUInt32(rd.GetValue(0));
                                EventSection s; if (!res.TryGetValue(uid, out s)) res[uid] = s = new EventSection();
                                s.Claims[rd.GetString(1)] = ToUnix(rd.GetDateTime(2));
                            }
                    }
                }
            }
            catch (Exception e) { Log("MySQL unavailable - All('" + ev + "'): " + e.Message, true); }
            foreach (var r in _recs.Values.ToArray())
            {
                if (r.Uid == 0) continue;
                lock (r.Sync)
                {
                    if (!r.Loaded) continue;
                    EventSection s;
                    if (r.Events.TryGetValue(ev, out s) && !s.IsEmpty) res[r.Uid] = s.Clone();
                    else res.Remove(r.Uid);
                }
            }
            return res;
        }
        public static List<uint> Uids(string ev) { var l = All(ev).Keys.ToList(); l.Sort(); return l; }

        // ------------------------------------------------------------------ flush / lifecycle hooks
        /// <summary>Writes this uid if dirty. false = the write failed (it stays dirty; the timer retries).</summary>
        public static bool Flush(uint uid)
        {
            Record r;
            if (!_recs.TryGetValue(uid, out r)) return true;
            lock (r.Sync) return TryWriteLocked(r);
        }
        /// <summary>Writes every dirty record. Returns the number that failed.</summary>
        public static int FlushAll()
        {
            if (Interlocked.Exchange(ref _flushing, 1) == 1) return 0;
            int bad = 0;
            try
            {
                foreach (var r in _recs.Values.ToArray())
                    if (r.IsDirty) lock (r.Sync) if (!TryWriteLocked(r)) bad++;
            }
            finally { Interlocked.Exchange(ref _flushing, 0); }
            return bad;
        }
        /// <summary>Drops clean records idle for 10 minutes (the cache only keeps players who are around).</summary>
        static void EvictIdle()
        {
            long now = Tick;
            foreach (var r in _recs.Values.ToArray())
            {
                if (r.Uid == 0 || r.IsDirty) continue;
                if (now - r.LastUse < 600000 && now >= r.LastUse) continue;
                bool online = false;
                try { online = Pool.GamePoll.ContainsKey(r.Uid); } catch { }
                if (online) continue;
                lock (r.Sync) { if (!r.IsDirty) { Record x; _recs.TryRemove(r.Uid, out x); } }
            }
        }
        /// <summary>[hook] LoadCharacterCore: pre-loads the player's record.</summary>
        public static void OnLogin(uint uid) { try { if (uid != 0) Rec(uid); } catch (Exception e) { Log("login " + uid + ": " + e.Message); } try { if (uid != 0) global::Qahr.Mobile.Compensation.CompEngine.OnLogin(uid); } catch { } }// [Compensation 2026-10-15] pending compensation mail -> delivered once the login finished
        /// <summary>[hook] SaveClient: writes the player's record (and the server record when dirty).</summary>
        public static void OnSave(uint uid) { try { Flush(uid); Flush(0); } catch (Exception e) { Log("save " + uid + ": " + e.Message); } }
        /// <summary>[hook] Game_Disconnect: write now, then drop the cached record when it is clean.</summary>
        public static void OnLogout(uint uid)
        {
            OnSave(uid);
            try { Record r; if (uid != 0 && _recs.TryGetValue(uid, out r)) lock (r.Sync) if (!r.IsDirty) _recs.TryRemove(uid, out r); } catch { }
        }

        /// <summary>Test seam: drop every cached record so the next read comes from MySQL (unflushed changes are lost, like a crash).</summary>
        public static void ResetCacheForTests() { _recs.Clear(); }

        // ------------------------------------------------------------------ admin (AdminApi / FestivalStudio)
        public static Dictionary<string, object> AdminDump(uint uid, string ev)
        {
            var r = Rec(uid);
            var d = new Dictionary<string, object>();
            lock (r.Sync)
            {
                LoadLocked(r);
                d["uid"] = (long)uid;
                d["available"] = r.Loaded;
                var evs = new Dictionary<string, object>();
                foreach (var kv in r.Events.OrderBy(k => k.Key, StringComparer.Ordinal))
                    if (!kv.Value.IsEmpty && (string.IsNullOrEmpty(ev) || kv.Key == ev)) evs[kv.Key] = kv.Value.ToJson();
                d["events"] = evs;
            }
            return d;
        }
        /// <summary>key = "c.name" | "f.name" | "cl.name" | "t.name" | "s.name"; value null/"" = delete
        /// (for a flag any value other than ""/0/false = set). Written to MySQL before it returns. Returns the old value.</summary>
        public static string AdminSet(uint uid, string ev, string key, string value)
        {
            CheckEvent(ev);
            int dot = key == null ? -1 : key.IndexOf('.');
            if (dot <= 0 || dot == key.Length - 1) throw new ArgumentException("key must be c.|f.|cl.|t.|s.<name>");
            string kind = key.Substring(0, dot), name = key.Substring(dot + 1);
            if (kind != "c" && kind != "f" && kind != "cl" && kind != "t" && kind != "s") throw new ArgumentException("unknown kind '" + kind + "'");
            bool del = string.IsNullOrEmpty(value);
            long n = 0;
            if (!del && kind != "s" && kind != "f" && !TryLong(value, out n)) throw new ArgumentException("value must be a number");
            if (kind == "cl" && !del && n <= 0) n = Now;
            string old = "";
            Edit(uid, ev, s =>
            {
                long o; string so;
                switch (kind)
                {
                    case "c": old = s.Counters.TryGetValue(name, out o) ? o.ToString(CultureInfo.InvariantCulture) : ""; if (del || n == 0) s.Counters.Remove(name); else s.Counters[name] = n; break;
                    case "cl": old = s.Claims.TryGetValue(name, out o) ? o.ToString(CultureInfo.InvariantCulture) : ""; if (del) s.Claims.Remove(name); else s.Claims[name] = n; break;
                    case "t": old = s.Times.TryGetValue(name, out o) ? o.ToString(CultureInfo.InvariantCulture) : ""; if (del) s.Times.Remove(name); else s.Times[name] = n; break;
                    case "s": old = s.Strings.TryGetValue(name, out so) ? so : ""; if (del) s.Strings.Remove(name); else s.Strings[name] = value; break;
                    default:
                        old = s.Flags.Contains(name) ? "1" : "";
                        bool on = !del && value != "0" && !value.Equals("false", StringComparison.OrdinalIgnoreCase);
                        if (on) s.Flags.Add(name); else s.Flags.Remove(name);
                        break;
                }
                return Tuple.Create(true, 0);
            }, true);
            return old;
        }

        // ------------------------------------------------------------------ legacy migration
        static string MigFlag(string ev, string path) { string f = "m:" + ev + ":" + Path.GetFileName(path); return f.Length > MaxKey - 2 ? f.Substring(0, MaxKey - 2) : f; }

        /// <summary>Old "uid TAB key=value;key=value" files (FoolsKit / AnnivKit / QixiKit) -> Strings of the section.
        /// Runs once: imports, flushes to MySQL, records a uid-0 flag, then renames the file to *.migrated.
        /// Returns rows imported (0 = nothing to do, -1 = failed; the old file is kept and it retries later).</summary>
        public static int MigrateKvText(string ev, string path, bool skipUid0 = false)
        {
            return Migrate(ev, path, () =>
            {
                var rows = new List<Tuple<uint, Dictionary<string, string>>>();
                foreach (var raw in File.ReadAllLines(path, Encoding.UTF8))
                {
                    var l = raw.Trim();
                    if (l.Length == 0 || l[0] == '#') continue;
                    int tab = l.IndexOf('\t'); uint uid;
                    if (tab <= 0 || !uint.TryParse(l.Substring(0, tab), out uid)) continue;
                    if (uid == 0 && skipUid0) continue;
                    var d = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var kv in l.Substring(tab + 1).Split(';'))
                    {
                        int eq = kv.IndexOf('=');
                        if (eq > 0) d[kv.Substring(0, eq)] = kv.Substring(eq + 1);
                    }
                    rows.Add(Tuple.Create(uid, d));
                }
                return rows;
            }, (s, d) => { foreach (var kv in d) s.Strings[kv.Key] = kv.Value; });
        }

        /// <summary>Old JSON stores: {"players":{uid:{k:v}}} (playersKey="players") or {uid:{k:v}} (playersKey=null).
        /// asCounters = numbers go to Counters, else every value goes to Strings.</summary>
        public static int MigrateJsonPlayers(string ev, string path, string playersKey, bool asCounters)
        {
            return Migrate(ev, path, () =>
            {
                var rows = new List<Tuple<uint, Dictionary<string, string>>>();
                var root = AdminApi.AdminJson.Parse(File.ReadAllText(path, Encoding.UTF8).TrimStart('\uFEFF')) as Dictionary<string, object>;
                object pl = root;
                if (root != null && playersKey != null) root.TryGetValue(playersKey, out pl);
                var players = pl as Dictionary<string, object>;
                if (players == null) return rows;
                foreach (var kv in players)
                {
                    uint uid; var o = kv.Value as Dictionary<string, object>;
                    if (o == null || !uint.TryParse(kv.Key, out uid)) continue;
                    var d = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var f in o) d[f.Key] = f.Value == null ? "" : f.Value.ToString();
                    rows.Add(Tuple.Create(uid, d));
                }
                return rows;
            }, asCounters
                ? (Action<EventSection, Dictionary<string, string>>)((s, d) => { foreach (var kv in d) { long v; if (TryLong(kv.Value.Split('.')[0], out v) && v != 0) s.Counters[kv.Key] = v; } })
                : (s, d) => { foreach (var kv in d) s.Strings[kv.Key] = kv.Value; });
        }

        static readonly object _migSync = new object();
        static int Migrate(string ev, string path, Func<List<Tuple<uint, Dictionary<string, string>>>> read, Action<EventSection, Dictionary<string, string>> apply)
        {
            lock (_migSync)
            {
                try
                {
                    if (string.IsNullOrEmpty(path) || !File.Exists(path)) return 0;
                    CheckEvent(ev);
                    string flag = MigFlag(ev, path);
                    if (!IsAvailable(0)) { Log("migrate " + path + ": MySQL unavailable, old file kept (retried later)", true); return -1; }
                    int n = 0;
                    if (!Flag(0, MetaEvent, flag))
                    {
                        var rows = read();
                        foreach (var row in rows)
                        {
                            var d = row.Item2;
                            Edit(row.Item1, ev, s => apply(s, d), true);   // each row is in MySQL before we go on
                            n++;
                        }
                        Flag(0, MetaEvent, flag, true, true);
                        Log("migrated " + n + " rows of " + path + " -> MySQL event '" + ev + "'");
                    }
                    string dst = path + ".migrated";
                    if (File.Exists(dst)) dst = path + "." + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".migrated";
                    File.Move(path, dst);
                    return n;
                }
                catch (Exception e) { Log("migrate " + path + ": " + e.Message + " (old file kept, retried later)", true); return -1; }
            }
        }
    }
}
