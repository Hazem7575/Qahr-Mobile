// [Unanswered B3 2026-10-14] shared helpers for the B3 (guild & league lists) handlers.
using System;
using System.Runtime.CompilerServices;

namespace Qahr.Mobile.Unanswered.B3
{
    /// <summary>Per-client, per-packet throttle. The list panels re-ask on every open; anything faster is spam.</summary>
    public static class B3Throttle
    {
        public const int MinIntervalMs = 400;
        public const int SlotCount = 8;

        // One slot per handled id; the table entry dies with the GameClient object (no leak on logout).
        sealed class Slots { public readonly int[] Last = new int[SlotCount]; public readonly bool[] Used = new bool[SlotCount]; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> table = new ConditionalWeakTable<Client.GameClient, Slots>();

        /// <summary>True = allowed now. slot: 0..7, one per packet.</summary>
        public static bool Allow(Client.GameClient client, int slot)
        {
            if (client == null || slot < 0 || slot >= SlotCount) return false;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;// wrap-safe unchecked difference
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < MinIntervalMs) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    public static class B3Util
    {
        /// <summary>Valid in-game player. The uid always comes from client.Player.UID, never from a packet.</summary>
        public static bool Ready(Client.GameClient client)
        {
            return client != null && client.Player != null && client.FullLoading;
        }

        public static ServerSockets.Packet BuildProto(ServerSockets.Packet stream, object proto, ushort id)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(id);
            return stream;
        }

        public static void Log(Exception e)
        {
            MyConsole.SaveException(e);
        }
    }
}
