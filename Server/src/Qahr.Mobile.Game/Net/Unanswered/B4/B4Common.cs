// [Unanswered B4 2026-10-14] shared helpers for the B4 (character / info panels) handlers.
using System;
using System.Runtime.CompilerServices;

namespace Qahr.Mobile.Unanswered.B4
{
    /// <summary>Per-client, per-packet throttle. Info panels re-ask on open/refresh; anything faster is spam.</summary>
    public static class B4Throttle
    {
        public const int MinIntervalMs = 400;
        public const int SlotCount = 8;

        public const int Slot2398 = 0, Slot2001 = 1, Slot2274 = 2, Slot2024 = 3, Slot2089 = 4, Slot2007 = 5, Slot2161 = 6;

        sealed class Slots { public readonly int[] Last = new int[SlotCount]; public readonly bool[] Used = new bool[SlotCount]; }
        // Entry dies with the GameClient object (no leak on logout).
        static readonly ConditionalWeakTable<Client.GameClient, Slots> table = new ConditionalWeakTable<Client.GameClient, Slots>();

        /// <summary>True = allowed now.</summary>
        public static bool Allow(Client.GameClient client, int slot)
        {
            if (client == null || slot < 0 || slot >= SlotCount) return false;
            if (B4Util.DisableThrottle) return true;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < MinIntervalMs) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    public static class B4Util
    {
        /// <summary>Test seam only (offline harness). Null in production: replies go to client.Send.</summary>
        public static Action<Client.GameClient, byte[]> TestSink;
        /// <summary>Test seam only. Never set in production.</summary>
        public static bool DisableThrottle;
        /// <summary>Test seam only: harness skips the FullLoading check on a fake client.</summary>
        public static bool TestSkipLoadingCheck;

        /// <summary>Valid in-game player. The uid always comes from client.Player.UID, never from a packet.</summary>
        public static bool Ready(Client.GameClient client)
        {
            return client != null && client.Player != null && (client.FullLoading || TestSkipLoadingCheck);
        }

        public static unsafe void Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            var sink = TestSink;
            if (sink != null)
            {
                int len = stream.Size;
                var b = new byte[len];
                for (int i = 0; i < len; i++) b[i] = stream.Memory[i];
                sink(client, b);
                return;
            }
            client.Send(stream);
        }

        public static ServerSockets.Packet BuildProto(ServerSockets.Packet stream, object proto, ushort id)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(id);
            return stream;
        }

        /// <summary>Protobuf request body; null on malformed input (never throws).</summary>
        public static T ReadProto<T>(ServerSockets.Packet stream) where T : class, new()
        {
            try
            {
                return stream.ProtoBufferDeserialize<T>(new T());
            }
            catch { return null; }
        }

        public static void Log(Exception e)
        {
            MyConsole.SaveException(e);
        }
    }
}
