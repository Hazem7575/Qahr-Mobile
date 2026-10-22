using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgRaceRecord
    {
       public enum RaceRecordTypes : int
       {
           BestTime = 0,
           EndTime = 1,
           AddRecord = 2,
           CloseRecords = 3,
       }
       public static unsafe ServerSockets.Packet RaceRecordCreate(this ServerSockets.Packet stream, RaceRecordTypes typ, int rank, string name, int dwparam, int prize)
       {
           stream.InitWriter();
           stream.Write((uint)typ);
           stream.Write(rank);
           stream.Write(name,32);
           stream.Write(dwparam);
           stream.Write(prize);

           stream.Finalize(GamePackets.RaceRecord);

           return stream;
       }
       public static unsafe ServerSockets.Packet RaceRecordCreate(this ServerSockets.Packet stream, RaceRecordTypes typ, int rank,int dwparam1, int dwparam2, int time, int prize)
       {
           stream.InitWriter();
           stream.Write((uint)typ);
           stream.Write(rank);
           stream.Write(dwparam1);
           stream.Write(dwparam2);
           stream.ZeroFill(8);
           stream.Write(time);
           stream.Write(prize);
           stream.Finalize(GamePackets.RaceRecord);

           return stream;
       }
    }
}
