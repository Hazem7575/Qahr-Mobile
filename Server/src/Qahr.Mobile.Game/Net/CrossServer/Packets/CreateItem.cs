using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
   public static class CreateItem
    {
       public static unsafe ServerSockets.Packet InterCreateItem(this ServerSockets.Packet stream, uint ID)
        {
            stream.InitWriter();
            stream.Write(ID);

            stream.Finalize(PacketTypes.InterServer_CreateItem);
            return stream;
        }
       public static unsafe void GetInterCreateItem(this ServerSockets.Packet stream, out uint ID)
        {
            ID = stream.ReadUInt32();

        }
    }
}
