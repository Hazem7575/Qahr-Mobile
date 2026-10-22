using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
   public static class SpecialTitles
    {
       public static unsafe ServerSockets.Packet InterServerSpecialTitlesCreate(this ServerSockets.Packet stream, string text)
        {
            stream.InitWriter();
            stream.Write(text.Length);
            stream.Write(text, text.Length);

            stream.Finalize(PacketTypes.InterServer_SpecialTitles);
            return stream;
        }
       public static unsafe void GetInterServerSpecialTitles(this ServerSockets.Packet stream, out string text)
        {
            int size = stream.ReadInt32();
            text = stream.ReadCString(size);
        }
    }
}
