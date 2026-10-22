// [TeamArena 2026-10-14] Team Arena (Team Qualifier) - per-player records, ranking, daily settlement, persistence.
//
// Data: Infinity[DB]\TeamArena\TeamArena.dat   (one line per player, written atomically: .tmp -> replace, .bak kept)
//       Infinity[DB]\TeamArena\TeamArenaHonorRewards.ini  (copy of the client's ini\TeamArenaHonorRewards.ini, OFFICIAL)
//       Infinity[DB]\TeamArena\logs\teamarena_yyyy-MM-dd.log  (every grant / spend / settlement)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.TeamQualifier
{
    public sealed class TeamArenaRecord
    {
        public uint Uid;
        public string Name = "";
        public ushort Level;
        public uint Class;
        public uint Mesh;          // Face * 10000 + Body (the client derives face and body type from it)
        public uint Points;
        public uint TodayWin, TodayLose, TotalWin, TotalLose;
        public uint TodayFights;   // matches that really started today (reward counter)
        public uint TodayPacks;    // TeamArenaPacks granted today
        public uint YRank, YPoints, YWin, YLose; // yesterday (top 10 / face tips)
        public uint PendingHonor;  // settled honor not yet credited (player had no arena record loaded)
        public uint HonorEarned;   // all honor this system ever credited (audit)

        public uint BodyType { get { return (Mesh / 1000) % 10; } }

        internal string Serialize()
        {
            return string.Join("|", new string[] {
                "P", Uid.ToString(), Clean(Name), Level.ToString(), Class.ToString(), Mesh.ToString(), Points.ToString(),
                TodayWin.ToString(), TodayLose.ToString(), TotalWin.ToString(), TotalLose.ToString(), TodayFights.ToString(), TodayPacks.ToString(),
                YRank.ToString(), YPoints.ToString(), YWin.ToString(), YLose.ToString(), PendingHonor.ToString(), HonorEarned.ToString() });
        }

        internal static TeamArenaRecord Parse(string line)
        {
            var p = line.Split('|');
            if (p.Length < 19 || p[0] != "P") return null;
            var r = new TeamArenaRecord();
            int i = 1;
            r.Uid = U(p[i++]); r.Name = p[i++]; r.Level = (ushort)Math.Min(ushort.MaxValue, U(p[i++])); r.Class = U(p[i++]); r.Mesh = U(p[i++]);
            r.Points = U(p[i++]); r.TodayWin = U(p[i++]); r.TodayLose = U(p[i++]); r.TotalWin = U(p[i++]); r.TotalLose = U(p[i++]);
            r.TodayFights = U(p[i++]); r.TodayPacks = U(p[i++]); r.YRank = U(p[i++]); r.YPoints = U(p[i++]); r.YWin = U(p[i++]); r.YLose = U(p[i++]);
            r.PendingHonor = U(p[i++]); r.HonorEarned = U(p[i++]);
            return r.Uid == 0 ? null : r;
        }

        static uint U(string s) { uint v; return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static string Clean(string s) { return (s ?? "").Replace("|", "").Replace("\r", "").Replace("\n", ""); }
    }

    public static class TeamArenaStore
    {
        public static readonly object Sync = new object();
        static readonly Dictionary<uint, TeamArenaRecord> records = new Dictionary<uint, TeamArenaRecord>();
        static bool loaded;
        static bool dirty;
        public static DateTime SettledDate = DateTime.MinValue;   // the day whose stats are "today"
        public static uint MatchCounter;

        static string DataFile { get { return Path.Combine(TeamArenaConfig.Dir, "TeamArena.dat"); } }

        // ------------------------------------------------------------------ records
        public static void EnsureLoaded()
        {
            if (loaded) return;
            lock (Sync)
            {
                if (loaded) return;
                loaded = true;
                try
                {
                    string f = DataFile;
                    if (!File.Exists(f) && File.Exists(f + ".bak")) f = f + ".bak";
                    if (File.Exists(f))
                    {
                        foreach (var raw in File.ReadAllLines(f, Encoding.UTF8))
                        {
                            if (raw.StartsWith("S|"))
                            {
                                var p = raw.Split('|');
                                DateTime d;
                                if (p.Length > 1 && DateTime.TryParseExact(p[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) SettledDate = d;
                                uint mc; if (p.Length > 2 && uint.TryParse(p[2], out mc)) MatchCounter = mc;
                                continue;
                            }
                            var r = TeamArenaRecord.Parse(raw);
                            if (r != null) records[r.Uid] = r;
                        }
                    }
                }
                catch (Exception e) { Log("LOAD FAILED " + e.Message); }
                if (SettledDate == DateTime.MinValue) { SettledDate = DateTime.Now.Date; dirty = true; }
            }
        }

        /// <summary>Get or create the record of an online player, refreshing name/level/class/look.</summary>
        public static TeamArenaRecord Get(Client.GameClient client)
        {
            EnsureLoaded();
            if (client == null || client.Player == null) return null;
            lock (Sync)
            {
                TeamArenaRecord r;
                uint uid = client.Player.UID;
                if (!records.TryGetValue(uid, out r))
                {
                    r = new TeamArenaRecord { Uid = uid, Points = TeamArenaConfig.Current.InitialPoints };
                    records[uid] = r;
                    dirty = true;
                }
                try
                {
                    string n = client.Player.Name ?? "";
                    if (r.Name != n) { r.Name = n; dirty = true; }
                    ushort lv = client.Player.Level; if (r.Level != lv) { r.Level = lv; dirty = true; }
                    uint cl = client.Player.Class; if (r.Class != cl) { r.Class = cl; dirty = true; }
                    uint mesh = (uint)(client.Player.Face * 10000 + client.Player.Body); if (r.Mesh != mesh) { r.Mesh = mesh; dirty = true; }
                }
                catch { }
                return r;
            }
        }

        public static TeamArenaRecord Find(uint uid)
        {
            EnsureLoaded();
            lock (Sync) { TeamArenaRecord r; return records.TryGetValue(uid, out r) ? r : null; }
        }

        public static void MarkDirty() { lock (Sync) dirty = true; }

        // ------------------------------------------------------------------ ranking
        /// <summary>Today's ranking: players who fought today, by points, then wins. Max 1000.</summary>
        public static List<TeamArenaRecord> TodayRanking()
        {
            EnsureLoaded();
            lock (Sync)
            {
                return records.Values.Where(r => r.TodayWin + r.TodayLose > 0)
                    .OrderByDescending(r => r.Points).ThenByDescending(r => r.TodayWin).ThenBy(r => r.Uid)
                    .Take(1000).ToList();
            }
        }

        public static uint TodayRank(uint uid)
        {
            var l = TodayRanking();
            for (int i = 0; i < l.Count; i++) if (l[i].Uid == uid) return (uint)(i + 1);
            return 0;
        }

        public static List<TeamArenaRecord> YesterdayTop10()
        {
            EnsureLoaded();
            lock (Sync)
            {
                return records.Values.Where(r => r.YRank >= 1 && r.YRank <= 10).OrderBy(r => r.YRank).ThenBy(r => r.Uid).Take(10).ToList();
            }
        }

        // ------------------------------------------------------------------ honor table (OFFICIAL client data)
        static uint[] honorTable;
        public static uint HonorForRank(int rank)
        {
            if (honorTable == null)
            {
                var l = new List<uint>();
                try
                {
                    string f = Path.Combine(TeamArenaConfig.Dir, "TeamArenaHonorRewards.ini");
                    if (File.Exists(f))
                    {
                        foreach (var raw in File.ReadAllLines(f))
                        {
                            string s = raw.Trim();
                            if (s.Length == 0 || s.StartsWith("RankAmount", StringComparison.OrdinalIgnoreCase)) continue;
                            uint v; if (uint.TryParse(s, out v)) l.Add(v);
                        }
                    }
                    else Log("TeamArenaHonorRewards.ini missing - no daily team honor");
                }
                catch (Exception e) { Log("honor table: " + e.Message); }
                honorTable = l.ToArray();
            }
            return rank >= 1 && rank <= honorTable.Length ? honorTable[rank - 1] : 0;
        }
        public static void ResetHonorTable() { honorTable = null; }

        // ------------------------------------------------------------------ daily settlement
        /// <summary>Runs once per day change: honor by today's rank, yesterday snapshot, today stats and points reset.
        /// The new date and the pending honor are saved BEFORE anything is credited, so a crash can lose a
        /// payment but never repeat one.</summary>
        public static bool SettleIfNewDay(DateTime now)
        {
            EnsureLoaded();
            lock (Sync)
            {
                if (now.Date <= SettledDate) return false;
                var cfg = TeamArenaConfig.Current;
                var ranking = TodayRanking();
                var rankOf = new Dictionary<uint, int>();
                for (int i = 0; i < ranking.Count; i++) rankOf[ranking[i].Uid] = i + 1;
                int paid = 0; ulong total = 0;
                foreach (var r in records.Values)
                {
                    int rank;
                    if (rankOf.TryGetValue(r.Uid, out rank))
                    {
                        r.YRank = (uint)rank; r.YPoints = r.Points; r.YWin = r.TodayWin; r.YLose = r.TodayLose;
                        uint h = HonorForRank(rank);
                        if (h > 0) { r.PendingHonor = SatAdd(r.PendingHonor, h); paid++; total += h; }
                    }
                    else { r.YRank = 0; r.YPoints = 0; r.YWin = 0; r.YLose = 0; }
                    r.TodayWin = r.TodayLose = r.TodayFights = r.TodayPacks = 0;
                    r.Points = cfg.InitialPoints;
                }
                Log("SETTLE day " + SettledDate.ToString("yyyy-MM-dd") + " -> " + now.Date.ToString("yyyy-MM-dd") + ": ranked " + ranking.Count + ", honor rows " + paid + ", honor total " + total);
                SettledDate = now.Date;
                dirty = true;
                Save(true);
                return true;
            }
        }

        /// <summary>Credit pending honor to players whose arena record is loaded (ArenaPoll). Saved right after.</summary>
        public static void PayPendingHonor()
        {
            EnsureLoaded();
            lock (Sync)
            {
                bool any = false;
                foreach (var r in records.Values)
                {
                    if (r.PendingHonor == 0) continue;
                    Game.MsgTournaments.MsgArena.User user;
                    if (!Game.MsgTournaments.MsgArena.ArenaPoll.TryGetValue(r.Uid, out user) || user == null || user.Info == null) continue;
                    uint h = r.PendingHonor;
                    r.PendingHonor = 0;
                    r.HonorEarned = SatAdd(r.HonorEarned, h);
                    user.Info.CurrentHonor = SatAdd(user.Info.CurrentHonor, h);
                    user.Info.HistoryHonor = SatAdd(user.Info.HistoryHonor, h);
                    Log("HONOR uid " + r.Uid + " " + r.Name + " +" + h + " (rank " + r.YRank + ") current " + user.Info.CurrentHonor);
                    any = true;
                }
                if (any) { dirty = true; Save(true); }
            }
        }

        public static uint SatAdd(uint a, uint b) { ulong s = (ulong)a + b; return s > uint.MaxValue ? uint.MaxValue : (uint)s; }

        // ------------------------------------------------------------------ persistence
        public static void Save(bool force = false)
        {
            lock (Sync)
            {
                if (!loaded || (!dirty && !force)) return;
                try
                {
                    Directory.CreateDirectory(TeamArenaConfig.Dir);
                    var sb = new StringBuilder();
                    sb.Append("# TeamArena v1 - Team Qualifier records (see Base\\TeamArena\\TeamArenaStore.cs)\r\n");
                    sb.Append("S|").Append(SettledDate.ToString("yyyyMMdd")).Append('|').Append(MatchCounter).Append("\r\n");
                    foreach (var r in records.Values.OrderBy(x => x.Uid)) sb.Append(r.Serialize()).Append("\r\n");
                    string f = DataFile, tmp = f + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), Encoding.UTF8);
                    if (File.Exists(f)) File.Replace(tmp, f, f + ".bak", true);
                    else File.Move(tmp, f);
                    dirty = false;
                }
                catch (Exception e) { Log("SAVE FAILED " + e.Message); }
            }
        }

        /// <summary>Tests only.</summary>
        public static void ResetForTests()
        {
            lock (Sync) { records.Clear(); loaded = false; dirty = false; SettledDate = DateTime.MinValue; MatchCounter = 0; honorTable = null; }
        }

        // ------------------------------------------------------------------ log
        static readonly object logSync = new object();
        public static void Log(string text)
        {
            try
            {
                string dir = Path.Combine(TeamArenaConfig.Dir, "logs");
                lock (logSync)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "teamarena_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
