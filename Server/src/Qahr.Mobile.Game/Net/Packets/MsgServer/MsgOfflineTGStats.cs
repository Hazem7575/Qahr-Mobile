using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet OfflineTGStatsCreate(this ServerSockets.Packet stream, ushort TrainedMinutes, ushort TotalTrainingMinutesLeft
            , int Character_AcquiredLevel, long Character_NewExp)
        {
            stream.InitWriter();

            stream.Write(TrainedMinutes);//4
            stream.Write(TotalTrainingMinutesLeft);//6
            stream.Write((int)Character_AcquiredLevel);
            stream.Write(Character_NewExp);

            stream.Finalize(GamePackets.MsgTrainingInfo);
            return stream;
        }
    }
}
