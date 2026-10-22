// [GuildRecruit 2026-10-14] shared helpers for the guild / union recruitment systems:
//   guild join applications (2351 + MsgSyndicate 55..59 + 2342), recruitment board (2019 / 2048),
//   union join applications (2384 + MsgLeagueOpt 8/11/82/83/85/86/87) and league order status (2393).
// Data + config live in Infinity[DB]\GuildRecruit\ (config.ini, *.txt stores, log.txt).
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qahr.Mobile.GuildRecruit
{
    /// <summary>Editable values (Infinity[DB]\GuildRecruit\config.ini). "official" = taken from the client text,
    /// "derived" = picked because the client does not carry the server value.</summary>
    public sealed class RecruitConfig
    {
        public int ApplyExpireHours = 72;           // derived: a guild / union application expires after 3 days
        public int MaxApplicationsPerGuild = 50;    // derived: queue size per guild / union
        public int MaxPendingPerPlayer = 5;         // derived: open applications per player
        public long AdMinBid = 500000;              // official: STR_ISSUE_RECRUIT_FEE_TIP2 (at least 500,000 silver)
        public long AdMaxBid = 2000000000;          // derived: hard cap (u32 on the wire)
        public int AdDays = 7;                      // official: STR_ISSUE_RECRUIT_CONFIRM (7-day advertisement)
        public int AdPageSize = 5;                  // derived: ads per board page
        public int AdTextMax = 250;                 // derived: characters kept from the ad text (client field is 512 bytes)
        public int GuildPledgeNeedsApproval = 1;    // derived: a guild pledge (MsgLeagueOpt 11) waits for the union leader
        public int RateLimitMs = 400;               // derived: min gap between two requests of one kind

        public static RecruitConfig Load(string file)
        {
            var c = new RecruitConfig();
            try
            {
                if (!File.Exists(file)) return c;
                foreach (var raw in File.ReadAllLines(file))
                {
                    var l = raw.Split('#')[0].Trim();
                    int eq = l.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = l.Substring(0, eq).Trim(), v = l.Substring(eq + 1).Trim();
                    long n;
                    if (!long.TryParse(v, out n)) continue;
                    switch (k.ToLowerInvariant())
                    {
                        case "applyexpirehours": c.ApplyExpireHours = (int)Clamp(n, 1, 24 * 30); break;
                        case "maxapplicationsperguild": c.MaxApplicationsPerGuild = (int)Clamp(n, 1, 500); break;
                        case "maxpendingperplayer": c.MaxPendingPerPlayer = (int)Clamp(n, 1, 50); break;
                        case "adminbid": c.AdMinBid = Clamp(n, 1, uint.MaxValue); break;
                        case "admaxbid": c.AdMaxBid = Clamp(n, 1, uint.MaxValue); break;
                        case "addays": c.AdDays = (int)Clamp(n, 1, 60); break;
                        case "adpagesize": c.AdPageSize = (int)Clamp(n, 1, 20); break;
                        case "adtextmax": c.AdTextMax = (int)Clamp(n, 1, 500); break;
                        case "guildpledgeneedsapproval": c.GuildPledgeNeedsApproval = n != 0 ? 1 : 0; break;
                        case "ratelimitms": c.RateLimitMs = (int)Clamp(n, 0, 60000); break;
                    }
                }
                if (c.AdMaxBid < c.AdMinBid) c.AdMaxBid = c.AdMinBid;
            }
            catch (Exception e) { RecruitIO.Error(e); }
            return c;
        }
        static long Clamp(long v, long lo, long hi) { return v < lo ? lo : (v > hi ? hi : v); }
    }

    public static class RecruitIO
    {
        /// <summary>Test seam: the harness points this at a COPY of the data.</summary>
        public static string RootOverride;
        /// <summary>Test seam: the clock.</summary>
        public static Func<DateTime> ClockOverride;
        public static DateTime Now { get { var c = ClockOverride; return c != null ? c() : DateTime.Now; } }

        static RecruitConfig _cfg;
        static readonly object _cfgLock = new object();

        public static string Root
        {
            get
            {
                if (!string.IsNullOrEmpty(RootOverride)) return RootOverride;
                string db = Program.ServerConfig.DbLocation;
                if (string.IsNullOrEmpty(db)) db = Path.Combine(Environment.CurrentDirectory, "Infinity[DB]");
                return Path.Combine(db, "GuildRecruit");
            }
        }
        public static string FilePath(string name) { return Path.Combine(Root, name); }

        public static RecruitConfig Config
        {
            get
            {
                lock (_cfgLock)
                {
                    if (_cfg == null) _cfg = RecruitConfig.Load(FilePath("config.ini"));
                    return _cfg;
                }
            }
        }
        public static void ReloadConfig() { lock (_cfgLock) _cfg = null; }

        /// <summary>Atomic write: tmp file, then replace (keeps a .bak of the previous version).</summary>
        public static void WriteAtomic(string file, IEnumerable<string> lines)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            string tmp = file + ".tmp";
            File.WriteAllLines(tmp, lines, new UTF8Encoding(false));
            if (File.Exists(file)) File.Replace(tmp, file, file + ".bak", true);
            else File.Move(tmp, file);
        }

        public static string[] ReadLines(string file)
        {
            try { return File.Exists(file) ? File.ReadAllLines(file, Encoding.UTF8) : new string[0]; }
            catch (Exception e) { Error(e); return new string[0]; }
        }

        static readonly object _logLock = new object();
        public static void Log(string line)
        {
            try
            {
                lock (_logLock)
                {
                    Directory.CreateDirectory(Root);
                    File.AppendAllText(FilePath("log.txt"), Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine, new UTF8Encoding(false));
                }
            }
            catch { }
        }

        public static void Error(Exception e)
        {
            try { MyConsole.SaveException(e); } catch { }
        }

        /// <summary>Tab separated field without tabs / newlines.</summary>
        public static string Field(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
        }

        /// <summary>Keeps printable text only (no control chars), trimmed and capped.</summary>
        public static string CleanText(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            foreach (char ch in s)
            {
                if (ch < 0x20 || ch == 0x7f) continue;
                if (char.IsSurrogate(ch)) continue;
                sb.Append(ch);
                if (sb.Length >= max) break;
            }
            return sb.ToString().Trim();
        }
    }

    /// <summary>Per-client, per-slot rate limit (entry dies with the GameClient object).</summary>
    public static class RecruitThrottle
    {
        public const int Slots = 16;
        sealed class S { public readonly int[] Last = new int[Slots]; public readonly bool[] Used = new bool[Slots]; }
        static readonly ConditionalWeakTable<object, S> table = new ConditionalWeakTable<object, S>();
        /// <summary>Test seam: disable the throttle in the harness.</summary>
        public static bool Disabled;

        public static bool Allow(object client, int slot)
        {
            if (client == null || slot < 0 || slot >= Slots) return false;
            if (Disabled) return true;
            int gap = RecruitIO.Config.RateLimitMs;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < gap) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    public static class RecruitUtil
    {
        public static bool Ready(Client.GameClient c)
        {
            return c != null && c.Player != null && c.FullLoading;
        }

        public static void Msg(Client.GameClient c, string text)
        {
            try { if (c != null && c.Player != null) c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        public static Client.GameClient Online(uint uid)
        {
            Client.GameClient c;
            return Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null ? c : null;
        }

        /// <summary>Guild ranks the client lets open the applicant list (rank 1..3 = leader, deputy, manager).</summary>
        public static bool IsGuildOfficer(Client.GameClient c)
        {
            if (c == null || c.Player == null || c.Player.MyGuild == null || c.Player.MyGuildMember == null) return false;
            var r = c.Player.MyGuildMember.Rank;
            return r == Role.Flags.GuildMemberRank.GuildLeader || r == Role.Flags.GuildMemberRank.DeputyLeader || r == Role.Flags.GuildMemberRank.Manager;
        }
    }
}
