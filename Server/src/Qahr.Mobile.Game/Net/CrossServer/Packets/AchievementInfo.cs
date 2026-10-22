using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class AchievementInfo
    {
        public static unsafe ServerSockets.Packet InterServerAchievementCreate(this ServerSockets.Packet stream, string text)
        {
            stream.InitWriter();
            stream.Write(text);

            stream.Finalize(PacketTypes.InterServer_Achievement);
            return stream;
        }
        public static unsafe void GetInterServerAchievement(this ServerSockets.Packet stream, out string text)
        {
            text = stream.ReadStringList()[0];
        }
    }
}
