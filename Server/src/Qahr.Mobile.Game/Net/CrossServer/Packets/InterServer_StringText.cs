using Qahr.Mobile.MsgInterServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Qahr.Mobile.MsgInterServer.Packets
{
    public unsafe static class InterServer_StringText
    {
        public static unsafe ServerSockets.Packet InterServerStringTextCreate(this ServerSockets.Packet stream, Qahr.Mobile.Client.GameClient user)
        {
            stream.InitWriter();

            if (user.HundredWeapons != null)
                stream.Write(user.HundredWeapons.ToString());

            if (user.MyNinja != null)
                stream.Write(user.MyNinja.ToString());

            if (user.MyArchives != null)
                stream.Write(user.MyArchives.ToString());
           
            stream.Finalize(PacketTypes.InterServer_StringText);
            return stream;
        }
        public static unsafe void GetInterServerStringText(this ServerSockets.Packet stream, out string HundredWeapons, out string MyNinja, out string MyArchives)
        {
            HundredWeapons = stream.ReadStringList()[0];
            MyNinja = stream.ReadStringList()[0];
            MyArchives = stream.ReadStringList()[0];

        }
    }
}
