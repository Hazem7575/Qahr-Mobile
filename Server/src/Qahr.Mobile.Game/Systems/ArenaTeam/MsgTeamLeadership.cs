using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{/* Int: 138739732 UInt: 138739732  Offset = 0
 Int: 67653 UInt: 67653  Offset = 2
 Int: 1 UInt: 1  Offset = 4
 Int: 8828009 UInt: 8828009  Offset = 8
 Int: -1268187002 UInt: 3026780294  Offset = 10
 Int: 8828009 UInt: 8828009  Offset = 12
 Int: 65670 UInt: 65670  Offset = 14
 Int: 1 UInt: 1  Offset = 16
*/
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TeamLeadership
    {
        public MsgTeamLeadership.Mode Typ;//4
        public uint UID;//8
        public uint LeaderUID;//12
        public int Count;//16
    }

    public unsafe static class MsgTeamLeadership
    {
        public enum Mode : uint
        {
            Leader = 1,
            Teammate = 2
        }
        public static unsafe void GetTeamLeadership(this ServerSockets.Packet stream, TeamLeadership* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(TeamLeadership));
        }

        public static unsafe ServerSockets.Packet TeamLeadershipCreate(this ServerSockets.Packet stream, TeamLeadership* pQuery)
        {
            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(TeamLeadership));

            stream.Finalize(GamePackets.MsgAutoGroup);

            return stream;
        }

        [PacketAttribute(GamePackets.MsgAutoGroup)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            TeamLeadership action;

            stream.GetTeamLeadership(&action);

            user.Send(stream.TeamLeadershipCreate(&action));

            if (user.Team != null)
                user.Team.AutoInvite = action.UID == 1;
        }
    }
}
