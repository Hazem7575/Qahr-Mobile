// [Unanswered B1 2026-10-14] Read-only ranking / record panels whose system does not exist on this server.
// Each one gets the official "empty" answer so the panel renders (no rows) instead of waiting.
// Nothing here reads or writes player state except client.Player.UID (via the Run helper); nothing is granted.
//
// Client behaviour (Conquer.exe 7957, Ghidra decompile of Process / request builders):
//   2265 CMsgTaskRewardRank      req @0x116a167 (f1 = type).  Process @0x1180170: rows (max 7) -> PostCmd(0x10d8). Reply f1 echo, no rows.
//        Server: MY System\TaskReward has no ranking data.
//   2069 CMsgCardsLotteryRankList req @0x11e6ccb (f1=1, f2, f4). Process @0x11ff932: f1=1 -> rows + stores f3/f4,
//        PostCmd(0xd27, 837 CDlgTurnoverLottery). f1=0 = server broadcast. Reply f1=1, f2/f4 echo, f3=0, no rows.
//        Server: no turnover (flop) lottery.
//   2391 CMsgGlobalLotteryRankList req @0xf36db4 (Page, TotalPages=10). Process @0xf56d48: RegistedCount + Pickers,
//        PostCmd(0xde5). Reply Page echo, TotalPages=1, RegistedCount=0, no Pickers. Server: 2005 Global Lottery not wired.
//   2484 CMsgTexasGlobalRankYesterday / 2510 CMsgGlobalRouletteRankYesterday: req @0xe4775b (f1 = type-100000, f2 = page).
//        Process @0x11c1659 / @0xfe6ac6: stores f3/f5 under key f1+100000, rows f4, PostCmd(0x117d)+(0x120a|0x1210).
//        Reply f1/f2 echo, f3=0, f5=0, no rows. Server: no Texas raffle / global roulette ranking.
//   9009 CMsgCrossTaskRank       req @0x11a5d16 (f1 byte, f2, f3, f4). Process @0x11be093: f5 = clear, rows f6.
//        Reply f1..f4 echo, f5=true, no rows. Server: no cross-server task ranking (MY System\MsgTaskRank is 9018).
//   2449 CMsgNsdsHisData         Process @0xf0d9a1: rows {id,value} -> PostCmd(0xd27, 1029 CDlgSmeltRecord).
//        Reply f1 echo, no rows. Server: Pool.SmeltingSuccesses is never filled (only read by an NPC dialog) and holds
//        {name, prize text}, not {id, value}.
//   2184 CMsgDragonSkinRecord    Process @0x107f0f7: rows {id,name,text} -> PostCmd(0xff7). Reply f1 echo, no rows.
//        Server: DragonSkin keeps no draw/obtain log.
using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B1
{
    // ------------------------------------------------------------------ contracts (match ProtoStudio schema.json)
    [ProtoContract]
    public class B1TaskRewardRankInfoPB// CMsgTaskRewardRankInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public string f2;
        [ProtoMember(3)] public uint f3;
    }
    [ProtoContract]
    public class B1TaskRewardRankPB// 2265 CMsgTaskRewardRankPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public List<B1TaskRewardRankInfoPB> f2 = new List<B1TaskRewardRankInfoPB>();
    }

    [ProtoContract]
    public class B1RankInfoPB// CRankInfoPB
    {
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public string f3;
    }
    [ProtoContract]
    public class B1CardsLotteryRankListPB// 2069 CMsgCardsLotteryRankListPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public List<B1RankInfoPB> f5 = new List<B1RankInfoPB>();
    }

    [ProtoContract]
    public class B1PrizeInfoPB// CPrizeInfoPB
    {
        [ProtoMember(1)] public uint PickItem;
        [ProtoMember(2)] public string Name;
    }
    [ProtoContract]
    public class B1GlobalLotteryRankListPB// 2391 CMsgGlobalLotteryRankListPB
    {
        [ProtoMember(1)] public uint Page;
        [ProtoMember(2)] public uint TotalPages;
        [ProtoMember(3)] public uint RegistedCount;
        [ProtoMember(4)] public List<B1PrizeInfoPB> Pickers = new List<B1PrizeInfoPB>();
    }

    [ProtoContract]
    public class B1RankYesterdayItemPB// CMsgTexasGlobalRankYesterdayItemPB == CMsgGlobalActiveRouletteRankItemPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public string f2;
        [ProtoMember(3)] public ulong f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public uint f7;
    }
    [ProtoContract]
    public class B1RankYesterdayPB// 2484 CMsgTexasGlobalRankYesterdayPB / 2510 CMsgGlobalActiveRouletteRankPB (same layout)
    {
        [ProtoMember(1)] public int f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public List<B1RankYesterdayItemPB> f4 = new List<B1RankYesterdayItemPB>();
        [ProtoMember(5)] public uint f5;
    }

    [ProtoContract]
    public class B1CrossTaskRankInfoPB// CMsgCrossTaskRankInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public int f4;
        [ProtoMember(5)] public string f5;
        [ProtoMember(6)] public ulong f6;
        [ProtoMember(7)] public uint f7;
    }
    [ProtoContract]
    public class B1CrossTaskRankPB// 9009 CMsgCrossTaskRankPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public bool f5;
        [ProtoMember(6)] public List<B1CrossTaskRankInfoPB> f6 = new List<B1CrossTaskRankInfoPB>();
    }

    [ProtoContract]
    public class B1NsdsHisDataInfoPB// CMsgNSDSHisDataInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
    }
    [ProtoContract]
    public class B1NsdsHisDataPB// 2449 CMsgNSDSHisDataPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public List<B1NsdsHisDataInfoPB> f2 = new List<B1NsdsHisDataInfoPB>();
    }

    [ProtoContract]
    public class B1DragonSkinRecordInfoPB// CDragonSkinRecordInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public string f2;
        [ProtoMember(3)] public string f3;
    }
    [ProtoContract]
    public class B1DragonSkinRecordPB// 2184 CMsgDragonSkinRecordPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(5)] public List<B1DragonSkinRecordInfoPB> f5 = new List<B1DragonSkinRecordInfoPB>();
    }

    public static class B1Rankings
    {
        public const uint MaxType = 100000;   // types / rank ids above this are garbage
        public const uint MaxPage = 1000;

        static uint Small(uint v, uint max) { return v > max ? 0 : v; }

        // ------------------------------------------------------------------ pure answers (unit tested)
        public static B1TaskRewardRankPB AnswerTaskRewardRank(B1TaskRewardRankPB req)
        {
            if (req == null) return null;
            return new B1TaskRewardRankPB { f1 = Small(req.f1, MaxType) };
        }

        public static B1CardsLotteryRankListPB AnswerCardsLotteryRank(B1CardsLotteryRankListPB req)
        {
            if (req == null || req.f1 != 1) return null;// f1=1 is the only client query
            return new B1CardsLotteryRankListPB { f1 = 1, f2 = Small(req.f2, MaxPage), f3 = 0, f4 = Small(req.f4, MaxType) };
        }

        public static B1GlobalLotteryRankListPB AnswerGlobalLotteryRank(B1GlobalLotteryRankListPB req)
        {
            if (req == null) return null;
            return new B1GlobalLotteryRankListPB { Page = Small(req.Page, MaxPage), TotalPages = 1, RegistedCount = 0 };
        }

        public static B1RankYesterdayPB AnswerRankYesterday(B1RankYesterdayPB req)
        {
            if (req == null) return null;
            int type = req.f1 < 0 || req.f1 >= (int)MaxType ? 0 : req.f1;// client keys the list by type + 100000
            return new B1RankYesterdayPB { f1 = type, f2 = Small(req.f2, MaxPage), f3 = 0, f5 = 0 };
        }

        public static B1CrossTaskRankPB AnswerCrossTaskRank(B1CrossTaskRankPB req)
        {
            if (req == null) return null;
            return new B1CrossTaskRankPB
            {
                f1 = req.f1 > 255 ? 0 : req.f1,  // client sends a byte
                f2 = Small(req.f2, uint.MaxValue - 1),
                f3 = Small(req.f3, MaxPage),
                f4 = Small(req.f4, MaxPage),
                f5 = true                       // clear the list
            };
        }

        public static B1NsdsHisDataPB AnswerNsdsHisData(B1NsdsHisDataPB req)
        {
            if (req == null) return null;
            return new B1NsdsHisDataPB { f1 = Small(req.f1, MaxType) };
        }

        public static B1DragonSkinRecordPB AnswerDragonSkinRecord(B1DragonSkinRecordPB req)
        {
            if (req == null) return null;
            return new B1DragonSkinRecordPB { f1 = Small(req.f1, MaxType) };
        }

        // ------------------------------------------------------------------ stream -> reply
        static ServerSockets.Packet Build<T>(ServerSockets.Packet stream, T empty, Func<T, object> answer, ushort id) where T : class
        {
            var req = B1Util.Read(stream, empty);
            if (req == null) return null;
            var ans = answer(req);
            return ans == null ? null : B1Util.BuildProto(stream, ans, id);
        }

        public static ServerSockets.Packet ReplyTaskRewardRank(ServerSockets.Packet s) { return Build(s, new B1TaskRewardRankPB(), AnswerTaskRewardRank, B1Ids.TaskRewardRank); }
        public static ServerSockets.Packet ReplyCardsLotteryRank(ServerSockets.Packet s) { return Build(s, new B1CardsLotteryRankListPB(), AnswerCardsLotteryRank, B1Ids.CardsLotteryRankList); }
        public static ServerSockets.Packet ReplyGlobalLotteryRank(ServerSockets.Packet s) { return Build(s, new B1GlobalLotteryRankListPB(), AnswerGlobalLotteryRank, B1Ids.GlobalLotteryRankList); }
        public static ServerSockets.Packet ReplyTexasRankYesterday(ServerSockets.Packet s) { return Build(s, new B1RankYesterdayPB(), AnswerRankYesterday, B1Ids.TexasGlobalRankYesterday); }
        public static ServerSockets.Packet ReplyRouletteRankYesterday(ServerSockets.Packet s) { return Build(s, new B1RankYesterdayPB(), AnswerRankYesterday, B1Ids.GlobalRouletteRankYesterday); }
        public static ServerSockets.Packet ReplyCrossTaskRank(ServerSockets.Packet s) { return Build(s, new B1CrossTaskRankPB(), AnswerCrossTaskRank, B1Ids.CrossTaskRank); }
        public static ServerSockets.Packet ReplyNsdsHisData(ServerSockets.Packet s) { return Build(s, new B1NsdsHisDataPB(), AnswerNsdsHisData, B1Ids.NsdsHisData); }
        public static ServerSockets.Packet ReplyDragonSkinRecord(ServerSockets.Packet s) { return Build(s, new B1DragonSkinRecordPB(), AnswerDragonSkinRecord, B1Ids.DragonSkinRecord); }

        // ------------------------------------------------------------------ handlers (slot 0 = 2382, 9/10 = casino)
        [PacketAttribute(B1Ids.TaskRewardRank)]
        private static void OnTaskRewardRank(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 1, () => ReplyTaskRewardRank(s)); }

        [PacketAttribute(B1Ids.CardsLotteryRankList)]
        private static void OnCardsLotteryRank(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 2, () => global::Qahr.Mobile.Lottery.TurnoverLotteryModule.Reply2069(s)); }// [Lottery 2026-10-14] real flop winners (Base\Lottery)

        [PacketAttribute(B1Ids.GlobalLotteryRankList)]
        private static void OnGlobalLotteryRank(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 3, () => global::Qahr.Mobile.Lottery.GlobalLotteryModule.Reply2391(s)); }// [Lottery 2026-10-14] real Golden Tree winners (Base\Lottery)

        [PacketAttribute(B1Ids.TexasGlobalRankYesterday)]
        private static void OnTexasRankYesterday(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 4, () => ReplyTexasRankYesterday(s)); }

        [PacketAttribute(B1Ids.GlobalRouletteRankYesterday)]
        private static void OnRouletteRankYesterday(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 5, () => ReplyRouletteRankYesterday(s)); }

        [PacketAttribute(B1Ids.CrossTaskRank)]
        private static void OnCrossTaskRank(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 6, () => ReplyCrossTaskRank(s)); }

        [PacketAttribute(B1Ids.NsdsHisData)]
        private static void OnNsdsHisData(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 7, () => ReplyNsdsHisData(s)); }

        [PacketAttribute(B1Ids.DragonSkinRecord)]
        private static void OnDragonSkinRecord(Client.GameClient c, ServerSockets.Packet s) { B1Util.Run(c, 8, () => ReplyDragonSkinRecord(s)); }
    }
}
