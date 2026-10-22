using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteNoWinnerCreate(this ServerSockets.Packet stream, byte Number)
        {
            stream.InitWriter();

            stream.Write(Number);

            stream.Finalize(GamePackets.MsgRouletteWinningNumber);
            return stream;
        }
    }
}
