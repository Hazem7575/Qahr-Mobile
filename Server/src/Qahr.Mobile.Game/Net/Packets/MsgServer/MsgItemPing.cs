using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateItemPing(this ServerSockets.Packet stream, uint Type, uint Date)
        {
            stream.InitWriter();
            stream.Write((uint)Type);
            uint serverTime = (Date != 0) ? Date : (uint)System.DateTime.Now.Ticks;
            stream.Write(serverTime);
            stream.Finalize((ushort)2614);
            return stream;
        }

        public static unsafe void GetItemPing(this ServerSockets.Packet stream, out uint Type, out uint Date)
        {
            Type = stream.ReadUInt32();
            Date = stream.ReadUInt32();
        }
    }

    public class MsgPing
    {
        [Packet((ushort)2614)]
        private static void Handler(Client.GameClient client, ServerSockets.Packet stream)
        {
            uint Type;
            uint Date;
            stream.GetItemPing(out Type, out Date);

            var serverNow = DateTime.Now;

            if (client.LastClientPing == 0)
            {
                client.LastClientPing = Date;
                client.LastPingTime = serverNow;
            }
            else
            {
                uint diffClient = Date - client.LastClientPing;
                double diffServer = (serverNow - client.LastPingTime).TotalMilliseconds;

                if (diffClient > diffServer * 2 || diffClient < diffServer * 0.5)
                {
                    //client.Socket.Disconnect();
                    //return;
                }

                client.LastClientPing = Date;
                client.LastPingTime = serverNow;
            }
            client.Send(stream.CreateItemPing(Type, Date));
        }
    }
}