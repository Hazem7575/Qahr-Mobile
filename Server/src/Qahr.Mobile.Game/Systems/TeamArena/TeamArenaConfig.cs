// [TeamArena 2026-10-14] Team Arena (Team Qualifier) - editable settings.
//
// File: Infinity[DB]\TeamArena\TeamArenaConfig.ini  (key=value, '#' comments). Missing file/keys -> defaults below.
// Values marked OFFICIAL come from the 7957 client (Cn_Res.ini texts / Conquer.exe code / itemtype descriptions).
// Values marked DERIVED are not in the client; they are balanced choices and can be edited here.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.TeamQualifier
{
    public sealed class TeamArenaConfig
    {
        // ---- OFFICIAL (client)
        public List<int[]> OpenHours = new List<int[]>();    // STR_TEAM_ARENA_INTRO / ATTEND_DISABLE_TIME: 08:00-12:00 & 13:00-23:00
        public int MinLevel = 40;                              // STR_TEAM_ARENA_ATTEND_DISABLE_LEVEL
        public uint PointsCap = 1500;                          // Conquer.exe 0xfc0269: buy up to 0x5dc points
        public uint SilverPerPoint = 1000;                     // Conquer.exe 0xfc02a7: silver = missing points * 0x3e8
        public uint RewardItem = 3007308;                      // itemtype 3007308 TeamArenaPack "Team Qualifier's reward"
        public int RewardWinsUpTo = 4;                         // "win in the first 4 fights"
        public int[] RewardJoinCounts = { 1, 3, 5, 7 };        // "or join 1/3/5/7 fights"
        public int PageRank = 10, PageFighting = 6;            // client paging (0x10c6727 /10, 0x10c5faf /6)
        public int ArenaMap = 700;                             // the Qualifier arena map (same as the solo Qualifier)

        // ---- DERIVED (server-side numbers the client does not carry)
        public uint InitialPoints = 1500;      // new players and the daily reset (= the client's buy cap)
        public uint MinPointsToJoin = 1;       // below this: STR_TEAM_ARENA_ATTEND_DISABLE_MARK / HERO_OUT
        public int MinTeamSize = 2;            // a "team" (leader + at least one member)
        public int MaxTeamSize = 5;            // game team limit
        public int PointsBase = 20;            // points won/lost per match = Base + (loserAvg - winnerAvg) / Divisor
        public int PointsDivisor = 50;
        public int PointsMin = 5, PointsMax = 40;
        public uint RewardChampionPoints = 30; // with every TeamArenaPack
        public int MatchmakingSeconds = 10;
        public int AcceptSeconds = 60;         // same as the solo Qualifier accept box
        public int FightSeconds = 180;         // same as the solo Qualifier
        public int EndDelaySeconds = 4;        // same as the solo Qualifier (result effect before leaving)
        public int RequestCooldownMs = 400;    // per client, per packet
        public int InteractCooldownMs = 600;

        public bool IsOpen(DateTime now)
        {
            int m = now.Hour * 60 + now.Minute;
            foreach (var w in OpenHours)
                if (m >= w[0] && m < w[1]) return true;
            return false;
        }

        // ------------------------------------------------------------------ loading
        public static string DirOverride;   // tests
        public static string Dir
        {
            get
            {
                if (!string.IsNullOrEmpty(DirOverride)) return DirOverride;
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "TeamArena";
            }
        }
        public static string FilePath { get { return Path.Combine(Dir, "TeamArenaConfig.ini"); } }

        static TeamArenaConfig _cur;
        static DateTime _stamp = DateTime.MinValue;
        static DateTime _lastCheck = DateTime.MinValue;
        static readonly object _sync = new object();

        public static TeamArenaConfig Current
        {
            get
            {
                lock (_sync)
                {
                    if (_cur != null && (DateTime.UtcNow - _lastCheck).TotalSeconds < 10) return _cur;
                    _lastCheck = DateTime.UtcNow;
                    DateTime st = DateTime.MinValue;
                    try { if (File.Exists(FilePath)) st = File.GetLastWriteTimeUtc(FilePath); } catch { }
                    if (_cur == null || st != _stamp)
                    {
                        _cur = Load(FilePath);
                        _stamp = st;
                    }
                    return _cur;
                }
            }
        }

        public static void Reset() { lock (_sync) { _cur = null; _stamp = DateTime.MinValue; } }

        static TeamArenaConfig Load(string path)
        {
            var c = new TeamArenaConfig();
            var hours = "08:00-12:00,13:00-23:00";
            try
            {
                if (File.Exists(path))
                {
                    foreach (var raw in File.ReadAllLines(path, Encoding.UTF8))
                    {
                        string line = raw.Trim();
                        if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("[")) continue;
                        int eq = line.IndexOf('=');
                        if (eq <= 0) continue;
                        string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                        int hash = v.IndexOf('#'); if (hash >= 0) v = v.Substring(0, hash).Trim();
                        switch (k.ToLowerInvariant())
                        {
                            case "openhours": hours = v; break;
                            case "minlevel": c.MinLevel = I(v, c.MinLevel, 1, 255); break;
                            case "pointscap": c.PointsCap = U(v, c.PointsCap, 1, 100000); break;
                            case "silverperpoint": c.SilverPerPoint = U(v, c.SilverPerPoint, 0, 10000000); break;
                            case "rewarditem": c.RewardItem = U(v, c.RewardItem, 0, uint.MaxValue); break;
                            case "rewardwinsupto": c.RewardWinsUpTo = I(v, c.RewardWinsUpTo, 0, 100); break;
                            case "rewardjoincounts": c.RewardJoinCounts = Ints(v, c.RewardJoinCounts); break;
                            case "arenamap": c.ArenaMap = I(v, c.ArenaMap, 1, int.MaxValue); break;
                            case "initialpoints": c.InitialPoints = U(v, c.InitialPoints, 1, 100000); break;
                            case "minpointstojoin": c.MinPointsToJoin = U(v, c.MinPointsToJoin, 0, 100000); break;
                            case "minteamsize": c.MinTeamSize = I(v, c.MinTeamSize, 1, 5); break;
                            case "maxteamsize": c.MaxTeamSize = I(v, c.MaxTeamSize, 1, 5); break;
                            case "pointsbase": c.PointsBase = I(v, c.PointsBase, 0, 1000); break;
                            case "pointsdivisor": c.PointsDivisor = I(v, c.PointsDivisor, 1, 100000); break;
                            case "pointsmin": c.PointsMin = I(v, c.PointsMin, 0, 1000); break;
                            case "pointsmax": c.PointsMax = I(v, c.PointsMax, 0, 1000); break;
                            case "rewardchampionpoints": c.RewardChampionPoints = U(v, c.RewardChampionPoints, 0, 10000); break;
                            case "matchmakingseconds": c.MatchmakingSeconds = I(v, c.MatchmakingSeconds, 1, 3600); break;
                            case "acceptseconds": c.AcceptSeconds = I(v, c.AcceptSeconds, 5, 600); break;
                            case "fightseconds": c.FightSeconds = I(v, c.FightSeconds, 10, 3600); break;
                            case "enddelayseconds": c.EndDelaySeconds = I(v, c.EndDelaySeconds, 0, 60); break;
                            case "requestcooldownms": c.RequestCooldownMs = I(v, c.RequestCooldownMs, 0, 60000); break;
                            case "interactcooldownms": c.InteractCooldownMs = I(v, c.InteractCooldownMs, 0, 60000); break;
                        }
                    }
                }
            }
            catch (Exception e) { try { MyConsole.WriteLine("[TeamArena] config: " + e.Message, ConsoleColor.Yellow); } catch { } }
            if (c.MaxTeamSize < c.MinTeamSize) c.MaxTeamSize = c.MinTeamSize;
            if (c.PointsMax < c.PointsMin) c.PointsMax = c.PointsMin;
            if (c.InitialPoints > c.PointsCap) c.InitialPoints = c.PointsCap;
            c.OpenHours = Hours(hours);
            return c;
        }

        static int I(string v, int def, int min, int max)
        {
            int r; if (!int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out r)) return def;
            return r < min ? min : (r > max ? max : r);
        }
        static uint U(string v, uint def, uint min, uint max)
        {
            uint r; if (!uint.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out r)) return def;
            return r < min ? min : (r > max ? max : r);
        }
        static int[] Ints(string v, int[] def)
        {
            var l = new List<int>();
            foreach (var p in v.Split(new[] { ',', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries))
            { int x; if (int.TryParse(p, out x) && x > 0 && x < 1000) l.Add(x); }
            return l.Count > 0 ? l.ToArray() : def;
        }
        static List<int[]> Hours(string v)
        {
            var l = new List<int[]>();
            foreach (var part in v.Split(new[] { ',', ';', '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var se = part.Trim().Split('-');
                if (se.Length != 2) continue;
                int a = Min(se[0]), b = Min(se[1]);
                if (a >= 0 && b > a) l.Add(new[] { a, b });
            }
            if (l.Count == 0) { l.Add(new[] { 8 * 60, 12 * 60 }); l.Add(new[] { 13 * 60, 23 * 60 }); }
            return l;
        }
        static int Min(string hhmm)
        {
            var p = hhmm.Trim().Split(':');
            int h, m = 0;
            if (!int.TryParse(p[0], out h)) return -1;
            if (p.Length > 1 && !int.TryParse(p[1], out m)) return -1;
            if (h < 0 || h > 24 || m < 0 || m > 59) return -1;
            return h * 60 + m;
        }
    }
}
