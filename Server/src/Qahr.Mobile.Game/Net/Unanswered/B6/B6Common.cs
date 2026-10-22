// [Unanswered B6 2026-10-14] shared helpers for the B6 (daily goals / quests) handlers.
using System;
using System.Runtime.CompilerServices;

namespace Qahr.Mobile.Unanswered.B6
{
    /// <summary>Per-client, per-packet throttle (the panels re-ask on every open; anything faster is spam).</summary>
    public static class B6Throttle
    {
        public const int MinIntervalMs = 400;

        // One slot per handled id; dies with the GameClient object (no leak on logout).
        sealed class Slots { public readonly int[] Last = new int[8]; public readonly bool[] Used = new bool[8]; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> table = new ConditionalWeakTable<Client.GameClient, Slots>();

        /// <summary>True = allowed now. slot: 0..7, one per packet.</summary>
        public static bool Allow(Client.GameClient client, int slot)
        {
            if (client == null || slot < 0 || slot >= 8) return false;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;// wrap-safe: unchecked difference (works on net48 and net10)
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < MinIntervalMs) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    public static class B6Util
    {
        /// <summary>Valid in-game player (uid always comes from client.Player.UID, never from a packet).</summary>
        public static bool Ready(Client.GameClient client)
        {
            return client != null && client.Player != null && client.FullLoading;
        }

        public static void Tell(Client.GameClient client, string text)
        {
            try { client.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }
    }
}
