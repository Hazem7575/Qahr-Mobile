using Qahr.Mobile.MsgInterServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Qahr.Mobile.MsgInterServer.Packets
{
    public unsafe static class InterServerLoader
    {
        public static unsafe ServerSockets.Packet InterServerLoaderCreate(this ServerSockets.Packet stream, uint EncryptTokenSpell)
        {
            stream.InitWriter();
            stream.Write(EncryptTokenSpell);
            stream.Finalize(PacketTypes.InterServer_Loader);
            return stream;
        }
        public static unsafe void GetInterLoader(this ServerSockets.Packet stream, out uint EncryptTokenSpell)
        {
            EncryptTokenSpell = stream.ReadUInt32();
        }
    }
}
