using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
   public static class CheckTransfer
    {

        public static unsafe ServerSockets.Packet InterServerCheckTransferCreate(this ServerSockets.Packet stream,uint Type, uint UID)
        {
            stream.InitWriter();
            stream.Write(Type);
            stream.Write(UID);

            stream.Finalize(PacketTypes.InterServer_CheckTransfer);
            return stream;
        }
        public static unsafe void GetInterServerCheckTransfer(this ServerSockets.Packet stream, out uint Type,out uint UID)
        {
            Type = stream.ReadUInt32();
            UID = stream.ReadUInt32();
        }

    }
}
