using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet BroadcastlistCreate(this ServerSockets.Packet stream, uint dwParam, ushort Total, ushort Count)
        {
            stream.InitWriter();
            stream.Write(dwParam);//4
            stream.Write(Total);//8
            stream.Write(Count);//10
            stream.Write((uint)0);//12
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemBroadcastlist(this ServerSockets.Packet stream, MsgTournaments.MsgBroadcast.BroadcastStr str, uint index)
        {
            stream.Write(str.ID);//16
            stream.Write(index);//20
            stream.Write(str.EntityID);
            stream.Write(str.EntityName, 32);
            stream.Write(str.SpentCPs);
            stream.Write(str.Message, 80);
            stream.ZeroFill(20);
            stream.Write(str.UnionRank);
            return stream;
        }
        public static unsafe ServerSockets.Packet FinalizeBroadcastlist(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgPigeonQuery);
            return stream;
        }
    }
}
