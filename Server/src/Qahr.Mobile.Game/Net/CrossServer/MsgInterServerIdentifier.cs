using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {


        public static void GetInterServerIdentifier(this ServerSockets.Packet stream, out uint mode, out uint dwparam1, out uint dwparam2)
        {
            mode = stream.ReadUInt32();
            dwparam1 = stream.ReadUInt32();
            dwparam2 = stream.ReadUInt32();
        }


        public static unsafe ServerSockets.Packet MsgInterServerIdentifier(this ServerSockets.Packet stream, uint mode, uint dwparam1, uint dwparam2)
        {

            stream.InitWriter();
            stream.Write(mode);//4
            stream.Write(dwparam1);//8
            stream.Write(dwparam2);//12
            stream.Write(26);//16
            stream.Write(1);//20
            stream.ZeroFill(12);//24
         
            stream.Finalize(GamePackets.MsgCrossSwitch);

            return stream;
        }
    }
}
