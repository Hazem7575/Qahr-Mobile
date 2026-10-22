// [Unanswered B4 2026-10-14] 2024 / 2089 / 2007 / 2161: panels whose server system is closed or missing.
// Each gets the official "empty / not available" reply so the client UI does not wait. Nothing is ever granted.
using System;
using System.Collections.Generic;
using ProtoBuf;
using Qahr.Mobile.Game;

namespace Qahr.Mobile.Unanswered.B4
{
    public static class B4InfoPanels
    {
        // ---------------- 2024 CMsgUserMelter (CMsgUserMelterPB: 1,2,3 uint32) ----------------
        // Client Process 0xfea035: hero melter data = (f1,f2,f3), PostCmd 0x111f. Melting is closed on this server
        // (MsgMelterOpt 2037 answers "closed"), so the panel gets zeros.
        [ProtoContract]
        public class UserMelter
        {
            [ProtoMember(1, IsRequired = true)] public uint Action;
            [ProtoMember(2, IsRequired = true)] public uint Value1;
            [ProtoMember(3, IsRequired = true)] public uint Value2;
        }

        [PacketAttribute(GamePackets.MsgUserMelter)]
        public static void HandleUserMelter(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client)) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2024)) return;
                var req = B4Util.ReadProto<UserMelter>(stream);
                if (req == null || req.Action != 0) return; // only the query; the system is closed
                B4Util.Send(client, B4Util.BuildProto(stream, new UserMelter(), GamePackets.MsgUserMelter));
            }
            catch (Exception e) { B4Util.Log(e); }
        }

        // ---------------- 2089 CMsgExpPool (CMsgExpPoolPB: 1 uint32, 2..4 uint64) ----------------
        // DANGER (plan risk "Claim grants experience"): there is no exp-pool system on this server.
        // Every request (query or claim) gets f1=0 with zero amounts. Client values are never read or trusted,
        // and nothing is granted. Client Process 0x112b053: f1==0 -> show amounts, PostCmd(0xd27, 850).
        [ProtoContract]
        public class ExpPool
        {
            [ProtoMember(1, IsRequired = true)] public uint Action;
            [ProtoMember(2, IsRequired = true)] public ulong Exp;
            [ProtoMember(3, IsRequired = true)] public ulong Value3;
            [ProtoMember(4, IsRequired = true)] public ulong Value4;
        }

        [PacketAttribute(GamePackets.MsgExpPool)]
        public static void HandleExpPool(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client)) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2089)) return;
                // Claims are refused by design: no grant path exists here (needs a real exp-pool system).
                B4Util.Send(client, B4Util.BuildProto(stream, new ExpPool(), GamePackets.MsgExpPool));
            }
            catch (Exception e) { B4Util.Log(e); }
        }

        // ---------------- 2007 CMsgInstanceInfo (CMsgInstanceInfoPB) ----------------
        // 1 action (required), 2 instanceId, 3 names[], 4 userInfo[], 5 completed[], 6 killBossTime[].
        // No instance-progress system: echo f1/f2 with empty lists (= no progress).
        [ProtoContract]
        public class InstanceInfo
        {
            [ProtoMember(1, IsRequired = true)] public uint Action;
            [ProtoMember(2, IsRequired = true)] public uint InstanceId;
        }

        public const uint InstanceMaxAction = 2;

        [PacketAttribute(GamePackets.MsgInstanceInfo)]
        public static void HandleInstanceInfo(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client)) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2007)) return;
                var req = B4Util.ReadProto<InstanceInfo>(stream);
                if (req == null || req.Action > InstanceMaxAction) return; // client Process knows 0,1,2 only
                var reply = new InstanceInfo { Action = req.Action, InstanceId = req.InstanceId };
                B4Util.Send(client, B4Util.BuildProto(stream, reply, GamePackets.MsgInstanceInfo));
            }
            catch (Exception e) { B4Util.Log(e); }
        }

        // ---------------- 2161 CMsgMonsterLive (CMsgMonsterLiveInfoPB: 1 uint32 req, 2 repeated uint32, 3 bool) ----------------
        // Client Process 0x11be9da: marks each id in f2 with (f1, f3), then PostCmd(0xfa2, 3).
        // Monster ids per query group are unknown -> echo f1 with an empty list.
        [ProtoContract]
        public class MonsterLive
        {
            [ProtoMember(1, IsRequired = true)] public uint QueryId;
            [ProtoMember(2)] public List<uint> MonsterIds = new List<uint>();
            [ProtoMember(3)] public bool Alive;
        }

        [PacketAttribute(GamePackets.MsgMonsterLive)]
        public static void HandleMonsterLive(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client)) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2161)) return;
                var req = B4Util.ReadProto<MonsterLive>(stream);
                if (req == null) return;
                var reply = new MonsterLive { QueryId = req.QueryId };
                B4Util.Send(client, B4Util.BuildProto(stream, reply, GamePackets.MsgMonsterLive));
            }
            catch (Exception e) { B4Util.Log(e); }
        }
    }
}
