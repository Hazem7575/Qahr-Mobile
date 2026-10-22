using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgLeagueSynList
    {
       public static unsafe ServerSockets.Packet LeagueSynListCreate(this ServerSockets.Packet stream, int Count, ushort Page, byte DwParam)
       {
           stream.InitWriter();
           stream.Write(Count);
           stream.Write(Page);
           stream.Write(DwParam);
           return stream;
       }
       public static unsafe ServerSockets.Packet AddItemLeagueSynList(this ServerSockets.Packet stream, uint UID,
           ulong SilverFound, ushort Members, ushort Level, string Name, string LeaderName)
       {
           stream.Write(UID);
           stream.Write(SilverFound);
           stream.Write(Members);
           stream.Write(Level);
           stream.Write(Name, 72);
           stream.Write(LeaderName, 32);
           return stream;
       }
       public static unsafe ServerSockets.Packet LeagueSynListFinalize(this ServerSockets.Packet stream)
       {
           stream.Finalize(GamePackets.MsgLeagueSynList);
           return stream;
       }
    }
}
