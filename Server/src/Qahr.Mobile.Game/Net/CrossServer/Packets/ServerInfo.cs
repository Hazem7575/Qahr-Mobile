using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class ServerInfo
    {
        public static unsafe ServerSockets.Packet ServerInfoCreate(this ServerSockets.Packet stream, uint type,
            uint ServerID, string ServerName, uint MapID, uint X, uint Y, uint Group)
        {
            stream.InitWriter();

            stream.Write(type);
            stream.Write(ServerID);
            stream.Write(ServerName, 32);
            stream.Write(MapID);
            stream.Write(X);
            stream.Write(Y);
            stream.Write(Group);


            stream.Finalize(PacketTypes.InterServer_ServerInfo);
            return stream;
        }
        public static unsafe void GetServerInfo(this ServerSockets.Packet stream, out uint type,
          out  uint ServerID, out string ServerName, out uint MapID, out uint X, out uint Y, out uint Group)
        {
            type = stream.ReadUInt32();
            ServerID = stream.ReadUInt32();
            ServerName = stream.ReadCString(32);
            MapID = stream.ReadUInt32();
            X = stream.ReadUInt32();
            Y = stream.ReadUInt32();
            Group = stream.ReadUInt32();

        }

    }
}
