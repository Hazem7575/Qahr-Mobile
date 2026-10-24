// [Unanswered B1 2026-10-14] shared helpers for the B1 (ranking panels + casino-hall acks) handlers.
using System;
using System.Runtime.CompilerServices;

namespace Qahr.Mobile.Unanswered.B1
{
    /// <summary>Packet ids used by B1. The ones missing from GamePackets.cs are literals here (B8 adds them).</summary>
    public static class B1Ids
    {
        public const ushort CardsLotteryRankList = Game.GamePackets.MsgCardsLotteryRankList; // 2069
        public const ushort DragonSkinRecord = Game.GamePackets.MsgDragonSkinRecord;         // 2184
        public const ushort TaskRewardRank = Game.GamePackets.MsgTaskRewardRank;             // 2265
        public const ushort BattlePassRank = Game.GamePackets.MsgBattlePassRank;             // 2382
        public const ushort GlobalLotteryRankList = Game.GamePackets.MsgGlobalLotteryRankList; // 2391
        public const ushort NsdsHisData = Game.GamePackets.MsgNsdsHisData;                   // 2449
        public const ushort TexasGlobalRankYesterday = 2484;   // not in GamePackets (B8: MsgTexasGlobalRankYesterday)
        public const ushort GlobalRouletteRankYesterday = 2510;// not in GamePackets (B8: MsgGlobalRouletteRankYesterday)
        public const ushort NTexasMTTAct = 2528;               // not in GamePackets (B8: MsgNTexasMTTAct)
        public const ushort GlobalPointRank = 2553;            // not in GamePackets (B8: MsgGlobalPointRank)
        public const ushort CrossTaskRank = 9009;              // not in GamePackets (B8: MsgCrossTaskRank)
    }

    /// <summary>Per-client, per-packet throttle. Panels re-ask on every open/page; anything faster is spam.</summary>
    public static class B1Throttle
    {
        public const int MinIntervalMs = 400;
        public const int SlotCount = 12;

        // One slot per handled id; the entry dies with the GameClient object (no leak on logout).
        sealed class Slots { public readonly int[] Last = new int[SlotCount]; public readonly bool[] Used = new bool[SlotCount]; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> table = new ConditionalWeakTable<Client.GameClient, Slots>();

        /// <summary>True = allowed now. slot: 0..SlotCount-1, one per packet id.</summary>
        public static bool Allow(Client.GameClient client, int slot)
        {
            if (client == null || slot < 0 || slot >= SlotCount) return false;
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

    public static class B1Util
    {
        /// <summary>Send limit of GameClient.Send (bigger packets are dropped there).</summary>
        public const int MaxPacket = 1032;

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

        /// <summary>Malformed protobuf = null (dropped), never an exception into the dispatcher.</summary>
        public static T Read<T>(ServerSockets.Packet stream, T empty) where T : class
        {
            try { return stream.ProtoBufferDeserialize(empty); } catch { return null; }
        }

        public static void Log(Exception e)
        {
            MyConsole.SaveException(e);
        }

        /// <summary>Common handler body: ready check, throttle, build, send. Nothing else happens.</summary>
        public static void Run(Client.GameClient client, int slot, Func<ServerSockets.Packet> reply)
        {
            try
            {
                if (!Ready(client) || !B1Throttle.Allow(client, slot)) return;
                var p = reply();
                if (p != null && p.Size <= MaxPacket) client.Send(p);
            }
            catch (Exception e) { Log(e); }
        }
    }
}
