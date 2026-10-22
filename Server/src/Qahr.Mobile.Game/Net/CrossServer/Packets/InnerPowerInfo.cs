using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class InnerPowerInfo
    {
        public static unsafe ServerSockets.Packet InterServerInnerPowerCreate(this ServerSockets.Packet stream, string text)
        {
            stream.InitWriter();
            stream.Write(text.Length);
            stream.Write(text, text.Length);

            stream.Finalize(PacketTypes.InterServer_InnerPower);
            return stream;
        }
        public static unsafe void GetInterServerInnerPower(this ServerSockets.Packet stream, out string text)
        {
            int size = stream.ReadInt32();
            text = stream.ReadCString(size);
        }


    }
}
