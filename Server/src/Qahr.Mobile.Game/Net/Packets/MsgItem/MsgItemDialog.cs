using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgNpc;
using System.Xml.Linq;
using System.IO;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateDialogItem(this ServerSockets.Packet stream, ushort dwParam , ushort dwParam1 , ushort dwParam2 , ushort dwParam3 , ushort dwParam4 , string dwParam5)
        {
            stream.InitWriter();
            stream.Write(17825800);//4
            stream.Write(dwParam1);//6
            stream.Write(dwParam2);//8
            stream.Write(dwParam3);//10
            stream.Write((uint)dwParam4);//12
            stream.Write(dwParam5, 58);//74
            stream.Finalize(GamePackets.MsgItemDialog);

            return stream;
        }
    }
}
