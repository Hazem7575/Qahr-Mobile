using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePKMatchStatsCreate(this ServerSockets.Packet stream, MsgTournaments.MsgEliteGroup.Match match)
        {
            stream.InitWriter();

            if (match.PlayersFighting.Length > 0)
            {

                stream.Write(match.PlayersFighting[0].Player.UID);
                stream.Write(match.PlayersFighting[0].Player.Name, 32);
                stream.Write((uint)match.PlayersFighting[0].Player.ServerID);
                stream.Write(match.PlayersFighting[0].ElitePKStats.Points);

            }
            else
                stream.ZeroFill(44);

            if (match.PlayersFighting.Length > 1)
            {
                stream.Write(match.PlayersFighting[1].Player.UID);
                stream.Write(match.PlayersFighting[1].Player.Name, 32);
                stream.Write((uint)match.PlayersFighting[1].Player.ServerID);
                stream.Write(match.PlayersFighting[1].ElitePKStats.Points);
            }
            else
                stream.ZeroFill(44);


            stream.Write((uint)0);//unknow

            stream.Finalize(GamePackets.MsgElitePKScore);

            return stream;
        }

    }
}
