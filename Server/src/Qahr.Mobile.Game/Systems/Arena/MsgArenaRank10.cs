using System;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaRank10Create(this ServerSockets.Packet stream, uint count)
        {
            stream.InitWriter();
            stream.Write(count);// +4 entry count (client loop count)
            stream.Write(1);
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemArenaRank10(this ServerSockets.Packet stream, Game.MsgTournaments.MsgArena.User user)
        {
            stream.Write(user.UID);
            stream.Write(user.Name, 32);
            stream.Write(user.Mesh);
            stream.Write((uint)user.Level);
            stream.Write((uint)user.Class);
            stream.Write(user.LastSeasonRank);
            stream.Write(user.LastSeasonRank);
            stream.Write(user.LastSeasonArenaPoints);
            stream.Write(user.LastSeasonWin);
            stream.Write(user.LastSeasonLose);
            stream.Write((uint)0);
            return stream;
        }

        public static unsafe ServerSockets.Packet ArenaRank10Finalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgQualifyingSeasonRankList);
            return stream;
        }
    }

    public unsafe struct MsgArenaRank10
    {
        [PacketAttribute(GamePackets.MsgQualifyingSeasonRankList)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                var info = Game.MsgTournaments.MsgArena.Top10.Where(p => p != null).ToArray();
                int count = info.Length;
                stream.ArenaRank10Create((uint)count);
                for (int x = 0; x < count; x++)
                {
                    var element = info[x];
                    stream.AddItemArenaRank10(element);
                }
                user.Send(stream.ArenaRank10Finalize());
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}