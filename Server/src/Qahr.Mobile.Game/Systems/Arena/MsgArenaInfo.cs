using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace Qahr.Mobile.Game.MsgServer
{

    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaInfoCreate(this ServerSockets.Packet stream, MsgArenaInfo user)
        {
            stream.InitWriter();
            stream.Write(user.TodayRank);
            stream.Write((uint)0);
            stream.Write((uint)user.Status);
            stream.Write(user.TotalWin);
            stream.Write(user.TotalLose);
            stream.Write(user.TodayWin);
            stream.Write(user.TodayBattles);
            stream.Write(user.HistoryHonor);
            stream.Write(user.CurrentHonor);
            stream.Write(user.ArenaPoints);
            stream.ZeroFill(8);
            stream.Finalize(GamePackets.MsgQualifyingDetailInfo);
            return stream;
        }
    }

    public class MsgArenaInfo
    {
        public enum Action : uint
        {
            NotSignedUp = 0,
            WaitingForOpponent = 1,
            WaitingInactive = 2
        }
        public ushort Length;
        public ushort PacketID;
        public uint TodayRank;//4
        public Action Status;//12
        public uint TotalWin;//16
        public uint TotalLose;//20
        public uint TodayWin;//24
        public uint TodayBattles;//28   
        public uint HistoryHonor;//32
        public uint CurrentHonor;//36
        public uint ArenaPoints;//40

        [PacketAttribute(GamePackets.MsgQualifyingDetailInfo)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            user.Send(stream.ArenaInfoCreate(user.ArenaStatistic.Info));
        }

    }
}
