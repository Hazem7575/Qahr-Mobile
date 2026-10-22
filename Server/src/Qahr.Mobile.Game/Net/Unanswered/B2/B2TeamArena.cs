// [Unanswered B2 2026-10-14] Team Arena / Team PK score / Honor shop handlers.
// [TeamArena 2026-10-14] 2138/2167/2460/2296/2218 now delegate to Qahr.Mobile.TeamQualifier.TeamArenaCore (Base\TeamArena);
// the empty builders below are kept unused for reference.
//
// The Team Arena (Team Qualifier) system does not exist on this server, so 2138/2167/2460/2296 get the
// client's official "empty" answer and 2218 (sign-up / accept / give up ...) is refused with the
// "not signed up" state. 2111 resends the live Team PK scoreboard. 2283 only REPORTS the honor points.
// No handler here writes any player state, and none grants or spends anything.
//
// Struct layouts come from the client's Process functions (Conquer.exe 7957):
//   2138 CMsgTeamArenaRank::Process            0x10c6727  (+6 u16 total == 0 -> clear list, page 1, PostCmd 0xcfc)
//   2167 CMsgTeamArenaYTop10List::Process      0x10c6aa6  (+4 u32 count, +8 byte clear flag, rows of 0x3c at +0xc)
//   2460 CMsgTeamArenaFightingTeamList::Process 0x10c5faf (+0xc total == 0 -> clear list, page 1, +0x10 value)
//   2296 CMsgTeamArenaHeroData::Process        0x10c6199  (u32 stats +4..+0x24, +8 status 0 = not signed up)
//   2218 CMsgTeamArenaInteractive::Process     0x10c639f  (+4 u16 action, +8 u32 param; action 6 = status)
//   2111 CMsgTeamPKArenicScore::Process        0xf0e5cb   (builder: MsgTeamElitePKMatchStats.cs)
//   2283 CMsgAthleteShopPB {1 uint32 points}   (protobuf)
using System;
using System.Runtime.CompilerServices;
using ProtoBuf;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Unanswered.B2
{
    /// <summary>Per-client, per-packet throttle. The panels re-ask on open/page; anything faster is spam.</summary>
    public static class B2Throttle
    {
        public const int MinIntervalMs = 400;
        public const int Slots = 8;

        sealed class State { public readonly int[] Last = new int[Slots]; public readonly bool[] Used = new bool[Slots]; }
        static readonly ConditionalWeakTable<Client.GameClient, State> table = new ConditionalWeakTable<Client.GameClient, State>();

        /// <summary>True = allowed now. slot 0..7, one per packet id.</summary>
        public static bool Allow(Client.GameClient client, int slot, int minMs = MinIntervalMs)
        {
            if (client == null || slot < 0 || slot >= Slots) return false;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < minMs) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    [ProtoContract]
    public class AthleteShopPB// 2283 CMsgAthleteShopPB
    {
        [ProtoMember(1, IsRequired = true)] public uint Points;
    }

    public static class B2TeamArena
    {
        public const ushort IdRank = Game.GamePackets.MsgTeamArenaRank;                     // 2138
        public const ushort IdYTop10 = Game.GamePackets.MsgTeamArenaYTop10List;             // 2167
        public const ushort IdFightingList = Game.GamePackets.MsgTeamArenaFightingTeamList; // 2460
        public const ushort IdHeroData = Game.GamePackets.MsgTeamArenaHeroData;             // 2296
        public const ushort IdInteractive = Game.GamePackets.MsgTeamArenaInteractive;       // 2218
        public const ushort IdTeamPkScore = Game.GamePackets.MsgTeamPKArenicScore;          // 2111
        public const ushort IdAthleteShop = Game.GamePackets.MsgAthleteShop;                // 2283

        // Client struct sizes (what the client itself sends); replies are padded to them, rest is zero.
        public const int LenRank = 0x48, LenYTop10 = 0x48, LenFightingList = 0x6c, LenHeroData = 0x2c, LenInteractive = 0x44;

        public const ushort InteractiveStatus = 6;  // 2218 action 6: param = arena status (0 = not signed up)
        public const string NotAvailableText = "Team Arena is not available on this server.";

        static bool Ready(Client.GameClient client)
        {
            return client != null && client.Player != null && client.FullLoading;
        }

        static void PadTo(ServerSockets.Packet stream, int len)
        {
            int left = len - stream.Position;
            if (left > 0) stream.ZeroFill(left);
        }

        // ------------------------------------------------------------------ builders (test seam, no socket)

        /// <summary>2138 empty rank list: page, total 0, count 0, clear flag.</summary>
        public static ServerSockets.Packet BuildRankEmpty(ServerSockets.Packet stream, ushort page)
        {
            if (page == 0 || page > 1000) page = 1;
            stream.InitWriter();
            stream.Write(page);          // +4 page
            stream.Write((ushort)0);     // +6 total (0 -> client clears list and shows page 1)
            stream.Write((uint)0);       // +8 count
            stream.Write((byte)1);       // +0xc clear flag
            PadTo(stream, LenRank);
            stream.Finalize(IdRank);
            return stream;
        }

        /// <summary>2167 empty yesterday top 10: count 0, clear flag.</summary>
        public static ServerSockets.Packet BuildYTop10Empty(ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write((uint)0);       // +4 count
            stream.Write((byte)1);       // +8 clear flag
            PadTo(stream, LenYTop10);
            stream.Finalize(IdYTop10);
            return stream;
        }

        /// <summary>2460 empty fighting-team list: total 0, count 0.</summary>
        public static ServerSockets.Packet BuildFightingListEmpty(ServerSockets.Packet stream, uint page)
        {
            if (page == 0 || page > 1000) page = 1;
            stream.InitWriter();
            stream.Write(page);          // +4 page
            stream.Write((uint)0);       // +8
            stream.Write((uint)0);       // +0xc total (0 -> client clears list)
            stream.Write((uint)0);       // +0x10
            stream.Write((uint)0);       // +0x14 count
            stream.Write((byte)1);       // +0x18 clear flag
            PadTo(stream, LenFightingList);
            stream.Finalize(IdFightingList);
            return stream;
        }

        /// <summary>2296 hero data: no team arena record -> all zero (rank 0, status 0 = not signed up).</summary>
        public static ServerSockets.Packet BuildHeroDataEmpty(ServerSockets.Packet stream)
        {
            stream.InitWriter();
            PadTo(stream, LenHeroData);
            stream.Finalize(IdHeroData);
            return stream;
        }

        /// <summary>2218 refusal: action 6 (status) with param 0 = not signed up.</summary>
        public static ServerSockets.Packet BuildInteractiveRefuse(ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write(InteractiveStatus); // +4 action
            stream.Write((ushort)0);         // +6
            stream.Write((uint)0);           // +8 param: status 0
            PadTo(stream, LenInteractive);
            stream.Finalize(IdInteractive);
            return stream;
        }

        /// <summary>2283: the player's honor points as stored on the server. Read only.</summary>
        public static ServerSockets.Packet BuildAthleteShop(ServerSockets.Packet stream, uint honorPoints)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new AthleteShopPB { Points = honorPoints });
            stream.Finalize(IdAthleteShop);
            return stream;
        }

        /// <summary>The Team PK match this client may see: fighting in it, or a registered watcher. Null otherwise.</summary>
        public static Game.MsgTournaments.MsgTeamEliteGroup.Match FindTeamPkMatch(Client.GameClient client)
        {
            if (client == null || client.Player == null) return null;
            if (client.Player.Map != 9855) return null;

            var match = client.TeamElitePkWatchingGroup;
            if (match != null)
            {
                if (!match.Watchers.ContainsKey(client.Player.UID)) return null;
            }
            else
            {
                var team = client.Team;
                if (team == null) return null;
                match = team.PkMatch;
                if (match == null) return null;
            }
            if (match.DinamicID != client.Player.DynamicID) return null;
            if (match.elitepkgroup == null || match.elitepkgroup.PKTournamentID != Game.GamePackets.MsgTeamPKArenic) return null;// Skill Team PK uses 2238
            foreach (var t in match.TeamsFighting)
                if (t == null || t.Leader == null || t.Leader.Player == null) return null;
            return match;
        }

        // ------------------------------------------------------------------ handlers

        [PacketAttribute(IdRank)]
        private static void OnRank(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                global::Qahr.Mobile.TeamQualifier.TeamArenaCore.OnRank(client, stream); // [TeamArena 2026-10-14] real Team Arena (Base\TeamArena)
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdYTop10)]
        private static void OnYTop10(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                global::Qahr.Mobile.TeamQualifier.TeamArenaCore.OnYTop10(client, stream); // [TeamArena 2026-10-14]
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdFightingList)]
        private static void OnFightingList(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                global::Qahr.Mobile.TeamQualifier.TeamArenaCore.OnFightingList(client, stream); // [TeamArena 2026-10-14]
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdHeroData)]
        private static void OnHeroData(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                global::Qahr.Mobile.TeamQualifier.TeamArenaCore.OnHeroData(client, stream); // [TeamArena 2026-10-14]
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdInteractive)]
        private static void OnInteractive(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                // Client actions seen in Conquer.exe: 0, 1, 3 (param 1/2), 4, 5, 11 (sign-up / cancel / accept /
                // give up / buy points / continue) - handled by the Team Arena system in Base\TeamArena.
                global::Qahr.Mobile.TeamQualifier.TeamArenaCore.OnInteractive(client, stream); // [TeamArena 2026-10-14] sign-up / accept / quit / buy points
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdTeamPkScore)]
        private static void OnTeamPkScore(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client) || !B2Throttle.Allow(client, 5, 1000)) return;
                var match = FindTeamPkMatch(client);
                if (match == null) return;// not in / not watching a Team PK match: nothing to show (official ignores it)
                client.Send(stream.TeamElitePKMatchStatsCreate(IdTeamPkScore, match));
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        [PacketAttribute(IdAthleteShop)]
        private static void OnAthleteShop(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                // Read-only. The request's f1 is ignored (never trusted, never applied): honor points are only
                // changed by the existing arena reward and the existing honor shop purchase code.
                if (!Ready(client) || !B2Throttle.Allow(client, 6)) return;
                client.Send(BuildAthleteShop(stream, client.ArenaHonorPoints));
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
