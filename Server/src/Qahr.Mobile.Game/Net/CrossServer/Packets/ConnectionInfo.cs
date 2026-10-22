using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class ConnectionInfo
    {
        public static unsafe ServerSockets.Packet ConnectionInfoCreate(this ServerSockets.Packet stream, uint Type, uint ServerID)
        {
            stream.InitWriter();
            stream.Write(Type);
            stream.Write(ServerID);

            stream.Finalize(PacketTypes.InterServer_ConnectionInfo);
            return stream;
        }
        public static unsafe void GetConnectionInfo(this ServerSockets.Packet stream, out uint Type, out  uint ServerID)
        {
            Type = stream.ReadUInt32();
            ServerID = stream.ReadUInt32();

        }
    }
}
