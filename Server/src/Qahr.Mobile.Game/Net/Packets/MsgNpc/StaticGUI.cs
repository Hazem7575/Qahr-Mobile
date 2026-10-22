using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe static partial class MsgBuilder
    {
        public enum StaticGUIType : byte
        {
            Header = 0,
            Body = 1,
            Footer = 2
        }

        public static unsafe ServerSockets.Packet StaticGUI(this ServerSockets.Packet stream, StaticGUIType Mode, string Text)
        {
            stream.InitWriter();

          //  stream.Write(DateTime.Now.Value());
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write((byte)Mode);
            stream.Write((byte)112);
            stream.Write(Text);

            stream.Finalize(GamePackets.MsgTaskDialog);
            return stream;
        }
    }
}
