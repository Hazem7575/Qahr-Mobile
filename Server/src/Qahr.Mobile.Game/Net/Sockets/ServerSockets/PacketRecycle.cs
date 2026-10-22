using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.ServerSockets
{
    public class RecycledPacket : IDisposable
    {
        private Packet stream;
        public Packet GetStream() { return stream; }
        public RecycledPacket()
        {
            this.stream = PacketRecycle.Take();
        }
        ~RecycledPacket()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            if (stream == null)
                return;
            PacketRecycle.Reuse(stream);
            stream = null;
            GC.SuppressFinalize(this);
        }
    }
    public static class PacketRecycle
    {
        public static int Count
        {
            get { return bin.Count; }
        }
        private static ConcurrentQueue<Packet> bin = new ConcurrentQueue<Packet>();
        public static Packet Take()
        {
            Packet from;
            if (!bin.TryDequeue(out from))
                from = new Packet(Packet.MAX_SIZE);
            from.Seek(4);
            return from;
        }
        public static void Reuse(Packet old)
        {
            old.Seek(0);
            // bin.Enqueue(old);
        }
    }
}
