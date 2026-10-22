using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class ChiInfo
    {
        public static unsafe ServerSockets.Packet InterServerChiCreate(this ServerSockets.Packet stream
            , uint ChiPoints, string Dragon, string Phoenix, string Turtle, string Tiger)
        {
            stream.InitWriter();
            stream.Write(ChiPoints);
            stream.Write(Dragon, Phoenix, Turtle, Tiger);

            stream.Finalize(PacketTypes.InterServer_Chi);
            return stream;
        }
        public static unsafe void GetInterServerChi(this ServerSockets.Packet stream, out uint ChitPoints
            , out string Dragon, out string Phoenix, out string Turtle, out string Tiger)
        {
            ChitPoints = stream.ReadUInt32();
            var strs = stream.ReadStringList();
            Dragon = strs[0];
            Phoenix = strs[1];
            Turtle = strs[2];
            Tiger = strs[3];
        }
    }
}
