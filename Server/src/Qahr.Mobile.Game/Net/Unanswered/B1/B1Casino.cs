// [Unanswered B1 2026-10-14] Casino-hall acks: 2553 CMsgGlobalPointRank (BoYi points/level + ranking) and
// 2528 CMsgNTexasMTTAct (New Texas Hold'em MTT tournament hall).
// The BoYi points system and the MTT poker tournaments do NOT exist on this server, so both get the official
// "nothing here" answer. No silver / CPs / chips / items are ever touched; buy-in, sign-up and leave are refused (no reply).
//
// Client behaviour (Conquer.exe 7957, Ghidra):
//   2553 Process @0x11be65a switch f1:
//        0 -> @0x11c3afc: my BoYi info from f12 (absent = all zero)
//        1 -> my points/level (only parts selected by the f2 mask), rank lists f13/f14
//        3 -> @0x11c2947: my info from f12 (absent = default), PostCmd(0xdb3,10)
//        4 -> @0x11c2eda: rank page rows f13/f14, f9/f10 paging, PostCmd(0x121e)/(0xf6d,0x243)
//        2, 5 -> other server pushes (not answered).
//      Requests (live): f1=1 after every Fruit Machine spin, f1=4 f9=1 f10=4 from the minimap panel, f1=0/3 on open.
//      Reply: same f1, f2 mask echo, f9/f10 echo (action 4), no f12 (client treats as level 0 / 0 points), no rows.
//      Zero is exactly what the client already assumes today (no reply ever arrived), so no game gets newly locked.
//   2528 Process @0x11bea4f switch f1 (cases 2..0x17, no case 0): f1=0 is the hall query the client sends on open
//      (f2 = [0,1,4]); an f1=0 answer with no lists is a no-op ack = "no tournaments". Every other action
//      (sign up / buy-in / leave / ...) moves money in the official game -> refused: no reply (needs system).
using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B1
{
    [ProtoContract]
    public class B1GlobalPointUserInfoPB// CMsgGlobalPointUserInfoPB
    {
        [ProtoMember(1)] public ulong f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public ulong f6;
        [ProtoMember(7)] public uint f7;
        [ProtoMember(8)] public uint f8;
        [ProtoMember(9)] public uint f9;
        [ProtoMember(10)] public uint f10;
        [ProtoMember(11)] public uint f11;
        [ProtoMember(12)] public uint f12;
    }
    [ProtoContract]
    public class B1GlobalPointRankInfoPB// CMsgGlobalPointRankInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public string f5;
        [ProtoMember(6)] public ulong f6;
        [ProtoMember(7)] public uint f7;
    }
    [ProtoContract]
    public class B1GlobalPointRankPB// 2553 CMsgGlobalPointRankPB
    {
        [ProtoMember(1)] public uint f1;// action
        [ProtoMember(2)] public uint f2;// mask
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public uint f7;
        [ProtoMember(8)] public ulong f8;
        [ProtoMember(9)] public uint f9;// page
        [ProtoMember(10)] public uint f10;// count
        [ProtoMember(11)] public uint f11;
        [ProtoMember(12)] public B1GlobalPointUserInfoPB f12;
        [ProtoMember(13)] public List<B1GlobalPointRankInfoPB> f13 = new List<B1GlobalPointRankInfoPB>();
        [ProtoMember(14)] public List<B1GlobalPointRankInfoPB> f14 = new List<B1GlobalPointRankInfoPB>();
        [ProtoMember(15)] public bool f15;
    }

    [ProtoContract]
    public class B1NTexasMTTActPB// 2528 CMsgNTexasMTTActPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public List<uint> f2 = new List<uint>();
        [ProtoMember(3)] public List<ulong> f3 = new List<ulong>();
        [ProtoMember(4)] public string f4;
    }

    public static class B1Casino
    {
        public const uint MaxRankPage = 1000, MaxRankCount = 50;

        // ------------------------------------------------------------------ pure answers (unit tested)
        public static B1GlobalPointRankPB AnswerGlobalPointRank(B1GlobalPointRankPB req)
        {
            if (req == null) return null;
            switch (req.f1)
            {
                case 0:
                case 1:
                case 3:
                    return new B1GlobalPointRankPB { f1 = req.f1, f2 = req.f2 };
                case 4:
                    return new B1GlobalPointRankPB
                    {
                        f1 = 4, f2 = req.f2,
                        f9 = req.f9 > MaxRankPage ? 1 : req.f9,
                        f10 = req.f10 > MaxRankCount ? MaxRankCount : req.f10
                    };
                default:
                    return null;// 2 / 5 / unknown: server pushes, not queries
            }
        }

        public static B1NTexasMTTActPB AnswerNTexasMTTAct(B1NTexasMTTActPB req)
        {
            if (req == null || req.f1 != 0) return null;// only the hall query; money actions refused
            return new B1NTexasMTTActPB { f1 = 0 };
        }

        public static ServerSockets.Packet ReplyGlobalPointRank(ServerSockets.Packet stream)
        {
            var req = B1Util.Read(stream, new B1GlobalPointRankPB());
            var ans = AnswerGlobalPointRank(req);
            return ans == null ? null : B1Util.BuildProto(stream, ans, B1Ids.GlobalPointRank);
        }

        public static ServerSockets.Packet ReplyNTexasMTTAct(ServerSockets.Packet stream)
        {
            var req = B1Util.Read(stream, new B1NTexasMTTActPB());
            var ans = AnswerNTexasMTTAct(req);
            return ans == null ? null : B1Util.BuildProto(stream, ans, B1Ids.NTexasMTTAct);
        }

        // 2553 can arrive before login completes (see Program.cs note) -> B1Util.Ready (FullLoading) drops it then.
        [PacketAttribute(B1Ids.GlobalPointRank)]
        private static void OnGlobalPointRank(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 9, () => ReplyGlobalPointRank(s)); }

        [PacketAttribute(B1Ids.NTexasMTTAct)]
        private static void OnNTexasMTTAct(Client.GameClient c, ServerSockets.Packet s) { if (global::Qahr.Mobile.Poker.PokerMtt.Handle(c, s)) return;/* [Poker 2026-10-14] MTT tournaments when enabled in Poker\config.json */ B1Util.Run(c, 10, () => ReplyNTexasMTTAct(s)); }
    }
}
