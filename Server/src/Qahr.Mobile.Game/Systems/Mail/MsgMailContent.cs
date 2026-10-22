using Qahr.Mobile.Client;
using Qahr.Mobile.ServerSockets;

namespace Qahr.Mobile.Game.MsgServer
{
    public class CMsgMailContent
    {
        public static void Create(GameClient client, PrizeInfo Mail)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.Write((uint)Mail.id);
                stream.Write(Mail.content, 512);
                stream.Finalize(GamePackets.MsgMailContent);
                client.Send(stream);
            }
        }
    }
}