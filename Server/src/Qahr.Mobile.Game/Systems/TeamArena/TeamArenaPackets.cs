// [TeamArena 2026-10-14] Team Arena (Team Qualifier) - packet builders.
//
// Every layout below was read from the 7957 client (Conquer.exe md5 D1803D09, Ghidra) - the Process function of
// each CMsg* and the getters the dialogs use (dlgteamarena.cpp):
//   2138 CMsgTeamArenaRank::Process            0x10c6727  getters 0xfd0f4a/0xfd1006 (CDlgTeamArenaWinsRank)
//   2167 CMsgTeamArenaYTop10List::Process      0x10c6aa6  getters 0xfd5868/0xfd58f2 (STR_TEAM_ARENA_FACE_TIP)
//   2168 CMsgTeamArenaFightingMemberInfo       0x10c5c26  getters 0xfccfd0/0xfcd07e (fighting tip), 0xfcffb7 (opponent dlg)
//   2218 CMsgTeamArenaInteractive::Process     0x10c639f  client sends 0/1/3(p1 accept,p2 give up)/4/5(buy points)/11(continue)
//   2239 CMsgTeamArenaScore::Process           0x10c68bc  (CDlgTeamArenaFightScore reads damage + name)
//   2296 CMsgTeamArenaHeroData::Process        0x10c6199
//   2460 CMsgTeamArenaFightingTeamList         0x10c5faf  getters 0xfcd1a5 (leader id / member count)
// "Body type" below = (mesh / 1000) % 10, the value the client passes with the class id to its class-name lookup.
using System;
using System.Collections.Generic;
using System.Text;

namespace Qahr.Mobile.TeamQualifier
{
    public static class TeamArenaPackets
    {
        public const ushort IdRank = Game.GamePackets.MsgTeamArenaRank;                       // 2138
        public const ushort IdYTop10 = Game.GamePackets.MsgTeamArenaYTop10List;               // 2167
        public const ushort IdMembers = Game.GamePackets.MsgTeamArenaFightingMemberInfo;      // 2168
        public const ushort IdInteractive = Game.GamePackets.MsgTeamArenaInteractive;         // 2218
        public const ushort IdScore = Game.GamePackets.MsgTeamArenaScore;                     // 2239
        public const ushort IdHeroData = Game.GamePackets.MsgTeamArenaHeroData;               // 2296
        public const ushort IdFightingList = Game.GamePackets.MsgTeamArenaFightingTeamList;   // 2460

        // 2218 actions
        public const ushort ActSignUp = 0, ActQuit = 1, ActMatchFound = 2, ActAnswer = 3, ActRivalGaveUp = 4,
            ActBuyPoints = 5, ActStatus = 6, ActHeroOut = 7, ActStartFight = 8, ActResult = 9, ActContinueDlg = 10,
            ActContinue = 11, ActCloseDlg = 13;
        // 2218 action 6 / 2296 +8 status (client 0x10c6199: 0 not joined, 1 waiting, 2..4 waiting inactive, 5 in combat)
        public const uint StNone = 0, StWaiting = 1, StInactiveInvite = 2, StInactiveAway = 3, StInactiveMap = 4, StFighting = 5;

        public const int LenInteractive = 0x44, LenHeroData = 0x2c;

        static void Name(ServerSockets.Packet s, string name, int len)
        {
            var b = Encoding.UTF8.GetBytes(name ?? "");
            int n = Math.Min(b.Length, len - 1);   // always keep a terminating zero
            for (int i = 0; i < n; i++) s.Write(b[i]);
            s.ZeroFill(len - n);
        }

        static void PadTo(ServerSockets.Packet s, int len)
        {
            int left = len - s.Position;
            if (left > 0) s.ZeroFill(left);
        }

        /// <summary>2218: +4 u16 action, +8 u32 param (len 0x44).</summary>
        public static ServerSockets.Packet Interactive(ServerSockets.Packet s, ushort action, uint param)
        {
            s.InitWriter();
            s.Write(action);
            s.Write((ushort)0);
            s.Write(param);
            PadTo(s, LenInteractive);
            s.Finalize(IdInteractive);
            return s;
        }

        /// <summary>2296: +4 rank, +8 status, +0xc total win, +0x10 total lose, +0x14 today win, +0x18 today lose,
        /// +0x1c history honor, +0x20 current honor, +0x24 team arena points (len 0x2c).</summary>
        public static ServerSockets.Packet HeroData(ServerSockets.Packet s, uint rank, uint status, TeamArenaRecord r, uint historyHonor, uint currentHonor)
        {
            s.InitWriter();
            s.Write(rank);
            s.Write(status);
            s.Write(r == null ? 0u : r.TotalWin);
            s.Write(r == null ? 0u : r.TotalLose);
            s.Write(r == null ? 0u : r.TodayWin);
            s.Write(r == null ? 0u : r.TodayLose);
            s.Write(historyHonor);
            s.Write(currentHonor);
            s.Write(r == null ? 0u : r.Points);
            PadTo(s, LenHeroData);
            s.Finalize(IdHeroData);
            return s;
        }

        /// <summary>2138 page: +4 u16 page, +6 u16 total, +8 u32 count, +0xc byte clear, rows of 0x38 at +0x10:
        /// +0 u16 rank, +8 points, +0xc class, +0x10 level, +0x14 byte body type, +0x15 name.</summary>
        public static ServerSockets.Packet Rank(ServerSockets.Packet s, ushort page, List<TeamArenaRecord> ranking, int perPage)
        {
            int total = Math.Min(ranking.Count, 1000);
            int pages = total == 0 ? 1 : (total - 1) / perPage + 1;
            if (page < 1) page = 1;
            if (page > pages) page = (ushort)pages;
            int start = (page - 1) * perPage;
            int count = Math.Max(0, Math.Min(perPage, total - start));
            s.InitWriter();
            s.Write(page);
            s.Write((ushort)total);
            s.Write((uint)count);
            s.Write((byte)1);
            PadTo(s, 0x10);
            for (int i = 0; i < count; i++)
            {
                var r = ranking[start + i];
                int row = s.Position;
                s.Write((ushort)(start + i + 1));
                s.ZeroFill(6);
                s.Write(r.Points);
                s.Write(r.Class);
                s.Write((uint)r.Level);
                s.Write((byte)r.BodyType);
                Name(s, r.Name, 0x20);
                PadTo(s, row + 0x38);
            }
            PadTo(s, 0x48);
            s.Finalize(IdRank);
            return s;
        }

        /// <summary>2167: +4 count, +8 byte clear, rows of 0x3c at +0xc: name[32], +0x20 rank, +0x24 mesh, +0x28 class,
        /// +0x2c level, +0x30 points, +0x34 win, +0x38 lose (all yesterday).</summary>
        public static ServerSockets.Packet YTop10(ServerSockets.Packet s, List<TeamArenaRecord> top)
        {
            int count = Math.Min(10, top.Count);
            s.InitWriter();
            s.Write((uint)count);
            s.Write((byte)1);
            PadTo(s, 0xc);
            for (int i = 0; i < count; i++)
            {
                var r = top[i];
                Name(s, r.Name, 0x20);
                s.Write(r.YRank);
                s.Write(r.Mesh);
                s.Write(r.Class);
                s.Write((uint)r.Level);
                s.Write(r.YPoints);
                s.Write(r.YWin);
                s.Write(r.YLose);
            }
            PadTo(s, 0x48);
            s.Finalize(IdYTop10);
            return s;
        }

        public sealed class MemberRow { public uint Uid, Level, Class, BodyType, Rank, Points; public string Name; }

        /// <summary>2168: +4 u16 type (0 = watch-list query, 1 = opponent dialog), +8 leader id, +0xc count,
        /// +0x10 byte clear, rows of 0x38 at +0x14: uid, level, class, body type, rank, points, name[32].
        /// Type 0 with count 0 makes the client show STR_TEAM_ARENA_QUERY_FIGHTING_MEMBER_FAIL.</summary>
        public static ServerSockets.Packet Members(ServerSockets.Packet s, ushort type, uint leaderId, List<MemberRow> rows)
        {
            int count = rows == null ? 0 : Math.Min(rows.Count, 10);
            s.InitWriter();
            s.Write(type);
            s.Write((ushort)0);
            s.Write(leaderId);
            s.Write((uint)count);
            s.Write((byte)1);
            PadTo(s, 0x14);
            for (int i = 0; i < count; i++)
            {
                var m = rows[i];
                s.Write(m.Uid); s.Write(m.Level); s.Write(m.Class); s.Write(m.BodyType); s.Write(m.Rank); s.Write(m.Points);
                Name(s, m.Name, 0x20);
            }
            PadTo(s, 0x4c);
            s.Finalize(IdMembers);
            return s;
        }

        public sealed class FightRow { public uint LeaderA, CountA, LeaderB, CountB; public string NameA, NameB; }

        /// <summary>2460: +4 first index, +8 last index, +0xc total matches, +0x10 total teams, +0x14 count,
        /// +0x18 byte clear, rows of 0x50 at +0x1c: leaderA, nameA[32], members A, leaderB, nameB[32], members B.</summary>
        public static ServerSockets.Packet FightingList(ServerSockets.Packet s, uint first, List<FightRow> all, uint totalTeams, int perPage)
        {
            int total = all.Count;
            int pages = total == 0 ? 1 : (total - 1) / perPage + 1;
            int page = first < 1 ? 1 : (int)Math.Min((first - 1) / (uint)perPage + 1, (uint)pages);
            int start = (page - 1) * perPage;
            int count = Math.Max(0, Math.Min(perPage, total - start));
            s.InitWriter();
            s.Write((uint)(start + 1));
            s.Write((uint)(start + perPage));
            s.Write((uint)total);
            s.Write(totalTeams);
            s.Write((uint)count);
            s.Write((byte)1);
            PadTo(s, 0x1c);
            for (int i = 0; i < count; i++)
            {
                var f = all[start + i];
                s.Write(f.LeaderA); Name(s, f.NameA, 0x20); s.Write(f.CountA);
                s.Write(f.LeaderB); Name(s, f.NameB, 0x20); s.Write(f.CountB);
            }
            PadTo(s, 0x6c);
            s.Finalize(IdFightingList);
            return s;
        }

        /// <summary>2239: side A at +4 (leader id, leader id, name[32] @+0xc, damage @+0x2c),
        /// side B at +0x30 (leader id, leader id, name[32] @+0x38, damage @+0x58). Len 0x5c.</summary>
        public static ServerSockets.Packet Score(ServerSockets.Packet s, uint leaderA, string nameA, uint dmgA, uint leaderB, string nameB, uint dmgB)
        {
            s.InitWriter();
            s.Write(leaderA); s.Write(leaderA); Name(s, nameA, 0x20); s.Write(dmgA);
            s.Write(leaderB); s.Write(leaderB); Name(s, nameB, 0x20); s.Write(dmgB);
            PadTo(s, 0x5c);
            s.Finalize(IdScore);
            return s;
        }
    }
}
