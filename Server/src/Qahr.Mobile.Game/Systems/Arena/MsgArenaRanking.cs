using System;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaRankingCreate(
            this ServerSockets.Packet stream,
            MsgArenaRanking.RankTyp rank,
            ushort page,
            uint count,
            uint entries)
        {
            stream.InitWriter();

            stream.Write((ushort)rank);          // 4
            stream.Write(page);                  // 6
            stream.Write(count);                 // 8 total
            stream.Write(entries);               // 12 entries in this packet (client loop count)
            stream.Write((uint)1);               // 16

            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemArenaRanking(
            this ServerSockets.Packet stream,
            ushort rank,
            string name,
            ushort unknown,
            uint points,
            uint playerClass,
            uint level,
            uint unknown2,
            uint mesh)
        {
            stream.Write(rank);                  // 20
            stream.Write(name, 32);              // 22
            stream.Write(unknown);               // 54
            stream.Write(points);                // 56
            stream.Write(playerClass);           // 60
            stream.Write(level);                // 64
            stream.Write(unknown2);               // 68
            stream.Write(mesh);                  // 72

            return stream;
        }

        public static unsafe ServerSockets.Packet ArenaRankingFinalize(
            this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgQualifyingRank);
            return stream;
        }

        public static unsafe void GetArenaRanking(
            this ServerSockets.Packet stream,
            out MsgArenaRanking.RankTyp rank,
            out ushort pageNumber)
        {
            rank = (MsgArenaRanking.RankTyp)stream.ReadUInt16();
            pageNumber = stream.ReadUInt16();
        }
    }

    public unsafe struct MsgArenaRanking
    {
        public enum RankTyp : ushort
        {
            Today = 0,
            History = 1
        }

        [PacketAttribute(GamePackets.MsgQualifyingRank)]
        private static void Handler(
            Client.GameClient user,
            ServerSockets.Packet stream)
        {
            RankTyp subType;
            ushort pageNumber;

            stream.GetArenaRanking(
                out subType,
                out pageNumber);

            const int maxPerPage = 10;
            if (pageNumber == 0)
                pageNumber = 1;

            int offset = (pageNumber - 1) * maxPerPage;

            switch (subType)
            {
                case RankTyp.Today:
                    {
                        var info = MsgTournaments.MsgArena.Top1000Today
                            .Where(p => p != null && p.Info.TodayRank != 0)
                            .ToArray();

                        int count = Math.Min(
                            maxPerPage,
                            Math.Max(0, info.Length - offset));

                        stream.ArenaRankingCreate(
                            RankTyp.Today,
                            pageNumber,
                            (uint)info.Length,
                            (uint)count);

                        for (int x = 0; x < count; x++)
                        {
                            int index = offset + x;

                            if (index < 0 || index >= info.Length)
                                continue;

                            var element = info[index];

                            if (element == null)
                                continue;
                            uint mesh = element.Mesh;

                            stream.AddItemArenaRanking(
                                (ushort)(index + 1),
                                element.Name ?? "None",
                                0,
                                element.Info.ArenaPoints,
                                element.Class,
                                element.Level,
                                1,
                                mesh);
                        }

                        user.Send(
                            stream.ArenaRankingFinalize());

                        break;
                    }

                case RankTyp.History:
                    {
                        var info = MsgTournaments.MsgArena.Top1000
                            .Where(p => p != null)
                            .ToArray();

                        int count = Math.Min(
                            maxPerPage,
                            Math.Max(0, info.Length - offset));

                        stream.ArenaRankingCreate(
                            RankTyp.History,
                            pageNumber,
                            (uint)info.Length,
                            (uint)count);

                        for (int x = 0; x < count; x++)
                        {
                            int index = offset + x;

                            if (index < 0 || index >= info.Length)
                                continue;

                            var element = info[index];

                            if (element == null)
                                continue;
                            uint mesh = element.Mesh;

                            stream.AddItemArenaRanking(
                                (ushort)(index + 1),
                                element.Name ?? "None",
                                6004,
                                element.Info.HistoryHonor,
                                element.Class,
                                element.Level,
                                33942209,
                                mesh);
                        }

                        user.Send(
                            stream.ArenaRankingFinalize());

                        break;
                    }
            }
        }
    }
}
