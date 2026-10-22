// [Unanswered B1 2026-10-14] 2382 CMsgBattlePassRank - Martial Token (BattlePass) hero ranking.
//
// Client (Conquer.exe 7957, Ghidra):
//   requests  @0xe8a73e  f1=2, f2=0          sent when the BattlePass window opens  -> "my rank"
//             @0xe8a992  f1=1, f2=page-1     sent by the ranking dialog (955) for page N (0-based)
//   Process   @0xdb3239
//     f1=1: if f4 -> clear the list; add every f5 row; page = f2+1; stores f3; PostCmd(0xd27, 955 = CDlgAnnualBenefitsRanking)
//     f1=2: stores f3 (my rank); PostCmd(0xd27, 953)
//   CDlgAnnualBenefitsRanking (@0xc0c430) pages the stored rows itself, 5 per page, total pages = rows/5, and looks a row up
//   by its rank. Row (CMsgBattlePassRankItemPB):
//     f1 rank, f2 uid (own row highlighted), f3 name, f4 level (col "Level"), f5 hero points (col "Hero Pts"),
//     f6 class + f7 lookface (client: class name from f6 and (f7/1000)%10 = body), f8 privilege 0/1/2 (token icon).
//   The rank award icons (BattlePass.lua RankAward) cover ranks 1..10.
//
// Server data (read-only): BattlePass state of every character, current season only (BattlePassConfig.SeasonId):
//   - in-memory states of BattlePassManager (online / recently online players, authoritative),
//   - [BattlePass] State= of Users\<uid>.ini for everybody else (same format as BattlePassManager.Serialize v1).
//   Ranking = season TotalPoints (all hero points earned this season, never decreased by store spending), then level, then uid.
//   Only players with points > 0 in the current season are ranked. Board cached for 60 s (files re-read at most every 10 min).
//   Nothing is written, nothing is granted (end-of-season rank rewards are NOT implemented server-side: needs system).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B1
{
    [ProtoContract]
    public class B1BpRankItemPB// CMsgBattlePassRankItemPB
    {
        [ProtoMember(1)] public uint f1;// rank
        [ProtoMember(2)] public uint f2;// uid
        [ProtoMember(3)] public string f3;// name
        [ProtoMember(4)] public uint f4;// battle pass level
        [ProtoMember(5)] public uint f5;// hero points (season total)
        [ProtoMember(6)] public uint f6;// class
        [ProtoMember(7)] public uint f7;// lookface (face*10000 + body)
        [ProtoMember(8)] public uint f8;// privilege (token) 0/1/2
    }

    [ProtoContract]
    public class B1BpRankPB// 2382 CMsgBattlePassRankPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// action 1 list / 2 my rank
        [ProtoMember(2)] public uint f2;// page (0-based)
        [ProtoMember(3)] public uint f3;// my rank (0 = not ranked)
        [ProtoMember(4)] public bool f4;// clear the list first
        [ProtoMember(5)] public List<B1BpRankItemPB> f5 = new List<B1BpRankItemPB>();
    }

    public static class B1BattlePassRank
    {
        public const int PageSize = 5;          // client: 5 rows per page
        public const int MaxRows = 20;          // 4 pages; keeps the reply far under the 1032-byte send limit
        public const int MaxPageRequest = 1000; // anything above is garbage

        public class Entry
        {
            public uint Uid; public string Name = ""; public uint Class; public uint Mesh;
            public uint Level; public ulong Points; public uint Privilege;
        }

        // ------------------------------------------------------------------ pure logic (unit tested)
        /// <summary>Merge file + live entries (live wins per uid), drop empty, sort by points desc, level desc, uid asc.</summary>
        public static List<Entry> Rank(IEnumerable<Entry> fromFiles, IEnumerable<Entry> live)
        {
            var map = new Dictionary<uint, Entry>();
            if (fromFiles != null) foreach (var e in fromFiles) if (e != null && e.Uid != 0) map[e.Uid] = e;
            if (live != null) foreach (var e in live) if (e != null && e.Uid != 0) map[e.Uid] = e;
            return map.Values.Where(e => e.Points > 0)
                .OrderByDescending(e => e.Points).ThenByDescending(e => e.Level).ThenBy(e => e.Uid).ToList();
        }

        public static uint RankOf(List<Entry> board, uint uid)
        {
            if (board == null) return 0;
            int i = board.FindIndex(e => e.Uid == uid);
            return i < 0 ? 0u : (uint)(i + 1);
        }

        public static B1BpRankPB Answer(B1BpRankPB req, List<Entry> board, uint myUid)
        {
            if (req == null) return null;
            board = board ?? new List<Entry>();
            uint mine = RankOf(board, myUid);
            if (req.f1 == 2)
                return new B1BpRankPB { f1 = 2, f3 = mine };
            if (req.f1 != 1) return null;// other actions are not client queries

            var rows = board.Take(MaxRows).ToList();
            int pages = Math.Max(1, (rows.Count + PageSize - 1) / PageSize);
            uint page = req.f2 > MaxPageRequest ? 0 : req.f2;
            if (page >= pages) page = (uint)(pages - 1);
            var ans = new B1BpRankPB { f1 = 1, f2 = page, f3 = mine, f4 = true };
            for (int i = 0; i < rows.Count; i++)
            {
                var e = rows[i];
                ans.f5.Add(new B1BpRankItemPB
                {
                    f1 = (uint)(i + 1), f2 = e.Uid, f3 = Clip(e.Name), f4 = e.Level,
                    f5 = e.Points > uint.MaxValue ? uint.MaxValue : (uint)e.Points,
                    f6 = e.Class, f7 = e.Mesh, f8 = e.Privilege > 2 ? 0 : e.Privilege
                });
            }
            return ans;
        }

        static string Clip(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length > 16 ? s.Substring(0, 16) : s;
        }

        /// <summary>Parse the v1 BattlePass state string (BattlePassManager.Serialize). Null = not this season / unreadable.</summary>
        public static Entry ParseState(string raw, uint season)
        {
            if (string.IsNullOrEmpty(raw) || season == 0) return null;
            string[] p = raw.Split('|');
            if (p.Length < 14 || p[0] != "1") return null;
            uint s; int level; ulong total; int priv;
            if (!uint.TryParse(p[1], out s) || s != season) return null;
            if (!int.TryParse(p[2], out level) || !ulong.TryParse(p[5], out total) || !int.TryParse(p[6], out priv)) return null;
            if (level < 1) level = 1;
            if (priv < 0 || priv > 2) return null;
            return new Entry { Level = (uint)level, Points = total, Privilege = (uint)priv };
        }

        // ------------------------------------------------------------------ server state (read-only)
        static readonly object sync = new object();
        static List<Entry> board = new List<Entry>();
        static List<Entry> fileCache = new List<Entry>();
        static DateTime boardAt = DateTime.MinValue, filesAt = DateTime.MinValue;
        static uint boardSeason;
        static readonly FieldInfo statesField =
            typeof(BattlePass.BattlePassManager).GetField("states", BindingFlags.NonPublic | BindingFlags.Static);

        public static List<Entry> Board()
        {
            BattlePass.BattlePassConfig.EnsureLoaded();
            uint season = BattlePass.BattlePassConfig.Enabled ? BattlePass.BattlePassConfig.SeasonId : 0;
            if (season == 0) return new List<Entry>();
            lock (sync)
            {
                DateTime now = DateTime.Now;
                if (boardSeason == season && (now - boardAt).TotalSeconds < 60) return board;
                if (boardSeason != season || (now - filesAt).TotalMinutes >= 10)
                {
                    fileCache = ScanFiles(season);
                    filesAt = now;
                }
                board = Rank(fileCache, Live(season));
                boardAt = now;
                boardSeason = season;
                return board;
            }
        }

        static List<Entry> Live(uint season)
        {
            var list = new List<Entry>();
            var states = statesField == null ? null : statesField.GetValue(null) as ConcurrentDictionary<uint, BattlePass.BpState>;
            if (states == null) return list;
            foreach (var kv in states)
            {
                var s = kv.Value;
                if (s == null) continue;
                Entry e;
                lock (s)
                {
                    if (s.Corrupt != null || s.Season != season) continue;
                    e = new Entry { Uid = kv.Key, Level = (uint)Math.Max(1, s.Level), Points = s.TotalPoints, Privilege = (uint)s.Privilege };
                }
                Client.GameClient c;
                var poll = Pool.GamePoll;
                if (poll != null && poll.TryGetValue(kv.Key, out c) && c != null && c.Player != null)
                {
                    e.Name = c.Player.Name; e.Class = c.Player.Class; e.Mesh = (uint)(c.Player.Face * 10000 + c.Player.Body);
                }
                else if (!FillFromFile(e)) continue;
                list.Add(e);
            }
            return list;
        }

        static string UsersDir { get { return Path.Combine(Program.ServerConfig.DbLocation.TrimEnd('\\', '/'), "Users"); } }

        static bool FillFromFile(Entry e)
        {
            try
            {
                var ini = new WindowsAPI.IniFile("\\Users\\" + e.Uid + ".ini");
                e.Name = ini.ReadString("Character", "Name", "");
                if (string.IsNullOrEmpty(e.Name) || e.Name == "None") return false;
                e.Class = ini.ReadUInt32("Character", "Class", 0);
                e.Mesh = (uint)(ini.ReadUInt16("Character", "Face", 0) * 10000 + ini.ReadUInt16("Character", "Body", 1003));
                return true;
            }
            catch { return false; }
        }

        static List<Entry> ScanFiles(uint season)
        {
            var list = new List<Entry>();
            try
            {
                string dir = UsersDir;
                if (!Directory.Exists(dir)) return list;
                foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                {
                    uint uid;
                    if (!uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid) || uid == 0) continue;
                    try
                    {
                        var ini = new WindowsAPI.IniFile("\\Users\\" + uid + ".ini");
                        Entry e = ParseState(ini.ReadBigString("BattlePass", "State", ""), season);
                        if (e == null || e.Points == 0) continue;
                        e.Uid = uid;
                        if (!FillFromFile(e)) continue;
                        list.Add(e);
                    }
                    catch { }
                }
            }
            catch (Exception ex) { B1Util.Log(ex); }
            return list;
        }

        // ------------------------------------------------------------------ handler
        public static ServerSockets.Packet Reply(ServerSockets.Packet stream, uint myUid, List<Entry> currentBoard)
        {
            var req = B1Util.Read(stream, new B1BpRankPB());
            var ans = Answer(req, currentBoard, myUid);
            return ans == null ? null : B1Util.BuildProto(stream, ans, B1Ids.BattlePassRank);
        }

        [PacketAttribute(B1Ids.BattlePassRank)]
        private static void OnBattlePassRank(Client.GameClient client, ServerSockets.Packet stream)
        {
            B1Util.Run(client, 0, () => Reply(stream, client.Player.UID, Board()));
        }
    }
}
