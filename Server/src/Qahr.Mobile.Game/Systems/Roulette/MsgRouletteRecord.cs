using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteRecordCreate(this ServerSockets.Packet stream, Database.Roulettes.RouletteTable.Member[] members, byte WinnerNumber)
        {
            stream.InitWriter();

            stream.Write(WinnerNumber);
            if (members != null)
            {
                stream.Write((byte)members.Length);

                foreach (var member in members)
                {
                    stream.Write(member.Betting);
                    stream.Write(member.Winning);
                    stream.Write(member.Owner.Player.Name, 32);
                }
            }
            else
                stream.Write((byte)0);

            stream.Finalize(GamePackets.MsgRouletteLatestProfitLossList);
            return stream;
        }
    }
}
