// [Unanswered B6 2026-10-14] Daily goals / quests: 2328 CMsgProcessGoalSchedule, 2385 CMsgProcessGoalQuery,
// 2546 CMsgProcessGoalTaskNumReward.
//
// Server state: there is NO server-side tracking for the open-server / 7-day goals (2328) and the Hero Biography
// goal system (Modules\Custom\HeroReward, 2378/2381) is deliberately closed ("Hero Biography System is now closed").
// So every reply here is the official EMPTY / inactive state and every claim is refused. Nothing here grants anything.
//
// Client behaviour (Conquer.exe 7957, Ghidra decomp of the Process functions):
//   2328 Process @0xf9934d, switch on f1 (type):
//        0: per detail -> goal UI updates (may open dialog 0x770); then PostCmd(0x102b). Empty f2 => only the refresh.
//        1: reads detail ids 0x1e/0x6c..0x72 (values) -> setter(all 0 when empty). 3: per detail + refresh. 4: per detail.
//        => empty f2 never opens a window and never shows progress.
//   2385 Process @0xf992af: per f3 item {goalId, done, claimed} -> update + PostCmd(0x1141); f1=true -> refresh all.
//        => empty f3 + f1=false is a no-op on the client.
//   2546 Process @0xf0dd8e: f1=0 -> marks each f3 id "claimed"; f1=1: f2 ABSENT -> ANIMATE_TIP popup,
//        f2 present && true && f3 -> marks f3[0] claimed. => refusal = f1=1 with f2 PRESENT and false (silent) + a
//        system message. f2 is therefore IsRequired on the server side so "false" is written on the wire.
using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B6
{
    // ------------------------------------------------------------------ contracts (match ProtoStudio schema.json)
    [ProtoContract]
    public class GoalScheduleDetailPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// goal id
        [ProtoMember(2)] public ulong f2;               // value
        [ProtoMember(3)] public uint f3;                // progress
        [ProtoMember(4)] public uint f4;                // state
    }
    [ProtoContract]
    public class GoalSchedulePB// 2328
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// type
        [ProtoMember(2)] public List<GoalScheduleDetailPB> f2 = new List<GoalScheduleDetailPB>();
    }

    [ProtoContract]
    public class GoalQueryItemPB
    {
        [ProtoMember(1)] public uint f1;// goal id
        [ProtoMember(2)] public bool f2;// done
        [ProtoMember(3)] public bool f3;// claimed
    }
    [ProtoContract]
    public class GoalQueryPB// 2385
    {
        [ProtoMember(1)] public bool f1;// refresh all
        [ProtoMember(2)] public List<uint> f2 = new List<uint>();
        [ProtoMember(3)] public List<GoalQueryItemPB> f3 = new List<GoalQueryItemPB>();
    }

    [ProtoContract]
    public class GoalTaskNumRewardPB// 2546
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// 0 query, 1 claim
        [ProtoMember(2, IsRequired = true)] public bool f2;// success (always written: absent = client reward popup)
        [ProtoMember(3)] public List<uint> f3 = new List<uint>();
    }

    public static class B6Goals
    {
        public const ushort IdSchedule = Game.GamePackets.MsgProcessGoalSchedule;// 2328
        public const ushort IdQuery = Game.GamePackets.MsgProcessGoalQuery;      // 2385
        public const ushort IdNumReward = 2546;// not in GamePackets yet: B8 should add MsgProcessGoalTaskNumReward = 2546

        public const string ClosedText = "This event is not available right now.";

        // ------------------------------------------------------------------ pure answers (test seam, no socket)
        /// <summary>2328: the official empty state. Echo the request type when it is one the client handles (0,1,3,4),
        /// otherwise type 0 (plain refresh). Never any goal detail: the server does not track these goals.</summary>
        public static GoalSchedulePB AnswerSchedule(GoalSchedulePB req)
        {
            uint t = req == null ? 0u : req.f1;
            if (t != 0 && t != 1 && t != 3 && t != 4) t = 0;
            return new GoalSchedulePB { f1 = t };
        }

        /// <summary>2385: no goal is tracked (Hero Biography closed) -> no items, no refresh.</summary>
        public static GoalQueryPB AnswerQuery(GoalQueryPB req)
        {
            return new GoalQueryPB { f1 = false };
        }

        /// <summary>2546: query -> nothing claimed; claim (or anything else) -> refused (success=false, no ids).</summary>
        public static GoalTaskNumRewardPB AnswerNumReward(GoalTaskNumRewardPB req, out bool refusedClaim)
        {
            refusedClaim = req == null || req.f1 != 0;
            return new GoalTaskNumRewardPB { f1 = refusedClaim ? 1u : 0u, f2 = false };
        }

        static ServerSockets.Packet Build(ServerSockets.Packet stream, object proto, ushort id)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(id);
            return stream;
        }

        // ------------------------------------------------------------------ handlers
        [PacketAttribute(IdSchedule)]
        private static void OnSchedule(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B6Util.Ready(client) || !B6Throttle.Allow(client, 0)) return;
                GoalSchedulePB req;
                try { req = stream.ProtoBufferDeserialize(new GoalSchedulePB()); } catch { return; }// malformed: drop
                client.Send(Build(stream, AnswerSchedule(req), IdSchedule));
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        [PacketAttribute(IdQuery)]
        private static void OnQuery(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B6Util.Ready(client) || !B6Throttle.Allow(client, 1)) return;
                GoalQueryPB req;
                try { req = stream.ProtoBufferDeserialize(new GoalQueryPB()); } catch { return; }
                client.Send(Build(stream, AnswerQuery(req), IdQuery));
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        [PacketAttribute(IdNumReward)]
        private static void OnNumReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B6Util.Ready(client) || !B6Throttle.Allow(client, 2)) return;
                GoalTaskNumRewardPB req;
                try { req = stream.ProtoBufferDeserialize(new GoalTaskNumRewardPB()); } catch { return; }
                bool refused;
                var ans = AnswerNumReward(req, out refused);
                client.Send(Build(stream, ans, IdNumReward));
                if (refused)
                {
                    // DANGEROUS packet: claims grant rewards. No server system backs it -> always refused, never paid.
                    B6Util.Tell(client, ClosedText);
                    try { MyConsole.WriteLine("[B6] refused 2546 claim uid=" + client.Player.UID); } catch { }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
    }
}
