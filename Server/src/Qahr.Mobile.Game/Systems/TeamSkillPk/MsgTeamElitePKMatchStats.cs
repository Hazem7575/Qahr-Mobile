using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {//MsgTeamPKArenicScore
        public static unsafe ServerSockets.Packet TeamElitePKMatchStatsCreate(this ServerSockets.Packet stream, ushort ID, MsgTournaments.MsgTeamEliteGroup.Match match)
        {
            stream.InitWriter();
            // [Unanswered B2 2026-10-14] 2111 layout per client CMsgTeamPKArenicScore::Process (0xf0e5cb):
            // +4 byte action, +5 byte mode (3 = other path), +6 u16 count (client drops the packet if < 2),
            // then 2 entries of 0x74 at +8: u32, leaderUID, teamUID, name[32] @+0xc, teamName[64] @+0x2c, points @+0x6c, u32 @+0x70.
            // The old code wrote a u32 0 here (count = 0), so the client ignored every 2111 score update.
            bool b2Score = ID == GamePackets.MsgTeamPKArenicScore;
            if (b2Score)
            {
                stream.Write((byte)1);//4 action
                stream.Write((byte)0);//5 mode
                stream.Write((ushort)2);//6 count
            }
            if (match.TeamsFighting.Length > 0)
            {
                var team = match.TeamsFighting[0];
                if (ID == GamePackets.MsgTeamPKArenicScore)
                    stream.Write(0);
                stream.Write(team.Leader.Player.UID);//4
                stream.Write(team.UID);//8
                stream.Write(team.Leader.Player.Name, 32);//12
                stream.Write(team.TeamName, 64);//44
                stream.Write(team.PKStats.Points);//108
                if (b2Score) stream.Write((uint)0);// [Unanswered B2 2026-10-14] entry +0x70
            }
            else
                stream.ZeroFill(b2Score ? 116 : 108);// [Unanswered B2 2026-10-14] 2111 entry = 0x74

            if (match.TeamsFighting.Length > 1)
            {
                var team = match.TeamsFighting[1];
                if (ID == GamePackets.MsgTeamPKArenicScore)
                    stream.Write(0);
                stream.Write(team.Leader.Player.UID);//112
                stream.Write(team.UID);//116
                stream.Write(team.Leader.Player.Name, 32);//120
                stream.Write(team.TeamName, 64);//152
                stream.Write(team.PKStats.Points);//216
                if (b2Score) stream.Write((uint)0);// [Unanswered B2 2026-10-14] entry +0x70
            }
            else
                stream.ZeroFill(b2Score ? 116 : 108);// [Unanswered B2 2026-10-14] 2111 entry = 0x74
            stream.Finalize(ID);
            return stream;
        }
    }
}
