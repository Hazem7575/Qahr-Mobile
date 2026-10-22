using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class JiangHuInfo
    {
        public static unsafe ServerSockets.Packet InterServerJiangHuCreate(this ServerSockets.Packet stream, Client.GameClient user)
        {
            stream.InitWriter();
            stream.Write(user.Player.MyJiangHu.ToString());
            stream.Finalize(PacketTypes.InterServer_JiangHu);
            return stream;
        }
        public static unsafe void GetInterServerJiangHu(this ServerSockets.Packet stream, out string text)
        {
            text = stream.ReadStringList()[0];
        }
    }
}
