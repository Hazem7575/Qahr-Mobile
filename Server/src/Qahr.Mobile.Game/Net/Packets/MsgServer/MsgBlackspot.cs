using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {


        public static unsafe ServerSockets.Packet BlackspotCreate(this ServerSockets.Packet stream, bool Active, uint UID)
        {
            stream.InitWriter();

          
            stream.Write((uint)(Active ? 0 : 1));
            stream.Write(UID);

            stream.Finalize(GamePackets.MsgDeadMark);
            return stream;
        }
    }

    public unsafe class MsgBlackspot
    {
        public uint dwParam;
        public uint UID;

        public bool Active { get { return dwParam == 0; } set { dwParam = (byte)(value ? 0 : 1); } }

    }
}
