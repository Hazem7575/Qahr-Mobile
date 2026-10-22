using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePKWatchCreate(this ServerSockets.Packet stream, MsgArenaWatchers.WatcherTyp typ, uint dwParam
            , uint ID, uint WatcherCount, uint dwCheers1, uint dwCheers2)
        {
            stream.InitWriter();

            stream.Write((ushort)typ);//4
            stream.Write(dwParam);//6
            stream.Write(ID);//10
            stream.Write(WatcherCount);//14
            stream.Write(dwCheers1);//18
            stream.Write(dwCheers2);//22
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, uint mesh, string name)
        {
            stream.Write(mesh);

            stream.Write(name, 32);
            stream.ZeroFill(16);
            stream.ZeroFill(16);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, uint mesh, uint ServerID, string name)
        {
            stream.Write(mesh);

            stream.Write(name, 32);
            stream.ZeroFill(16);
            stream.ZeroFill(16);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, string name, uint ServerID)
        {
            stream.Write((uint)0);
            stream.Write(name, 32);
            stream.ZeroFill(16);
            stream.ZeroFill(16);
            return stream;
        }
        public static unsafe ServerSockets.Packet ElitePKWatchFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgArenicWitness);
            return stream;
        }
    }
}
