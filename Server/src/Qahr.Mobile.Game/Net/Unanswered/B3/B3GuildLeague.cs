// [Unanswered B3 2026-10-14] Guild & League list panels:
//   2351 CMsgSyndicateApplyListSyn, 2384 CMsgLeagueApplyList, 2048 CMsgSynRecruitAdvertisingList,
//   2393 CMsgTokenUpdate, 2061 CMsgNewSynWar.
//
// Server state (read-only check, nothing in MsgNewGuild\ or Union\ was edited):
//   - Guilds store recruit SETTINGS (Recruitment_Flag / BP / RecruitmentON) but no join APPLICATIONS and no posted
//     recruit ADS (2019 has no handler, so no guild has ever posted an ad).        -> 2351 / 2048 = official empty.
//   - Unions join by direct pledge (MsgLeagueOpt Pledge/GuildPledge); no union application queue exists. -> 2384 empty.
//   - League order tokens: MsgLeagueOpt.Tokens sends the token list (2447); usage/cooldown is NOT tracked
//     (MsgLeagueToken 2392 handler is an empty stub).          -> 2393 = every requested token "unused, no cooldown".
//   - CMsgNewSynWar is the cross-server "New Guild War"; the server only has the classic pole Guild War
//     (MsgTournaments.MsgGuildWar), a different event with different UI.                 -> 2061 = empty info.
//
// Client behaviour (Conquer.exe 7957, Ghidra decompile of Process / request builders):
//   2351 Process @0xf9b917: clears the applicant list; per f1 item -> add row; PostCmd(0x1131) only if count > 0.
//        Request @0x113e973: empty message.
//   2384 Process @0x11be84e: f2 bit0 -> clear list; per f3 item -> add row; f2 bit1 -> PostCmd(0xd27,0xeee) opens
//        CDlgLeagueEnrolling (3822). Request @0x10ce01d is sent right after the client itself opened
//        CDlgLeagueSynList (702) -> reply f2 = 1 (clear) only, never bit1 (would pop an unrequested dialog).
//   2048 Process @0x117fe3a (raw struct, len 0x2a8): +4 u32 page, +8 i32 count, +0xc u32, +0x10 u8 clear,
//        entries of 0x290 bytes from +0x18; ends with PostCmd(0xcab=3243) refresh.
//        Reply: page echo, count 0, clear 1, zero entry area (keeps the official 0x2a8 size).
//   2393 Process @0x1130445: per f2 info {Token, Used, Seconds} -> cooldown = now + Seconds*1000; PostCmd(0xf62).
//   2061 Process @0x1031693: switch f1. Requests @0x10cf14d / @0x10cf1f2 send (f1=2, f2=0) / (f1=2, f2=2).
//        f1=2 always ends with PostCmd(0xd27,0x4c6) = open CDlgNewSynWarInfo (1222). Reply: f1=2, f2 echo (0|2),
//        f3=0, no flags, no f19 rows. Other f1 values are server-pushed events -> no reply.
using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B3
{
    // ------------------------------------------------------------------ contracts (match ProtoStudio schema.json)
    [ProtoContract]
    public class B3ApplicationInfoPB// CApplicationInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;   // uid
        [ProtoMember(2, IsRequired = true)] public string f2 = "";// name
        [ProtoMember(3, IsRequired = true)] public int f3;    // level
        [ProtoMember(4, IsRequired = true)] public int f4;
        [ProtoMember(5, IsRequired = true)] public int f5;
        [ProtoMember(6, IsRequired = true)] public int f6;
    }
    [ProtoContract]
    public class B3SyndicateApplyListPB// 2351 CMsgSyndicateApplyListSyncPB
    {
        [ProtoMember(1)] public List<B3ApplicationInfoPB> f1 = new List<B3ApplicationInfoPB>();
    }

    [ProtoContract]
    public class B3LeagueApplyListInfoPB// CMsgLeagueApplyListInfo
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public string f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public string f7;
    }
    [ProtoContract]
    public class B3LeagueApplyListPB// 2384 CMsgLeagueApplyListPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;// bit0 = clear list, bit1 = open CDlgLeagueEnrolling
        [ProtoMember(3)] public List<B3LeagueApplyListInfoPB> f3 = new List<B3LeagueApplyListInfoPB>();
    }

    [ProtoContract]
    public class B3TokenUpdateInfoPB// CMsgTokenUpdateInfoPB
    {
        [ProtoMember(1, IsRequired = true)] public uint Token;
        [ProtoMember(2, IsRequired = true)] public uint Used;
        [ProtoMember(3, IsRequired = true)] public uint Seconds;
    }
    [ProtoContract]
    public class B3TokenUpdatePB// 2393 CMsgTokenUpdatePB
    {
        [ProtoMember(1)] public List<uint> Ids = new List<uint>();
        [ProtoMember(2)] public List<B3TokenUpdateInfoPB> Infos = new List<B3TokenUpdateInfoPB>();
    }

    [ProtoContract]
    public class B3NewSynWarListPB// CMsgNewSynWarListPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2, IsRequired = true)] public ulong f2;
        [ProtoMember(3, IsRequired = true)] public string f3 = "";
    }
    [ProtoContract]
    public class B3NewSynWarPB// 2061 CMsgNewSynWarPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// action
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public ulong f3;
        [ProtoMember(4)] public string f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public uint f6;
        [ProtoMember(7)] public string f7;
        [ProtoMember(8)] public uint f8;
        [ProtoMember(9)] public string f9;
        [ProtoMember(10)] public uint f10;
        [ProtoMember(11)] public string f11;
        [ProtoMember(12)] public string f12;
        [ProtoMember(13)] public uint f13;
        [ProtoMember(14)] public string f14;
        [ProtoMember(15)] public uint f15;
        [ProtoMember(16)] public bool f16;
        [ProtoMember(17)] public bool f17;
        [ProtoMember(18)] public uint f18;
        [ProtoMember(19)] public List<B3NewSynWarListPB> f19 = new List<B3NewSynWarListPB>();
    }

    public static class B3GuildLeague
    {
        public const ushort IdSynApplyList = Game.GamePackets.MsgSyndicateApplyListSyn;     // 2351
        public const ushort IdLeagueApplyList = Game.GamePackets.MsgLeagueApplyList;        // 2384
        public const ushort IdRecruitAdList = Game.GamePackets.MsgSynRecruitAdvertisingList;// 2048
        public const ushort IdTokenUpdate = Game.GamePackets.MsgTokenUpdate;                // 2393
        public const ushort IdNewSynWar = Game.GamePackets.MsgNewSynWar;                    // 2061

        public const int RecruitAdStructLen = 0x2a8;// official CMsgSynRecruitAdvertisingList size (header included)
        public const uint MaxRecruitPage = 1000;
        public const int MaxTokenIds = 32;
        public const uint MaxTokenId = 100000;

        // ------------------------------------------------------------------ pure answers (test seam, no socket)
        /// <summary>2351: the server stores no guild join applications -> official empty list.</summary>
        public static B3SyndicateApplyListPB AnswerSynApplyList()
        {
            return new B3SyndicateApplyListPB();
        }

        /// <summary>2384: unions join by direct pledge, there is no application queue -> clear + empty list.
        /// Never sets bit1 (the client already opened its own dialog before asking).</summary>
        public static B3LeagueApplyListPB AnswerLeagueApplyList(B3LeagueApplyListPB req)
        {
            uint f1 = req == null ? 0u : req.f1;
            if (f1 > MaxRecruitPage) f1 = 0;
            return new B3LeagueApplyListPB { f1 = f1, f2 = 1 };
        }

        /// <summary>2393: league-order usage/cooldown is not tracked by the server -> each requested token is reported
        /// unused with no cooldown. Ids are de-duplicated, capped and range-checked. Not in a union -> no infos.</summary>
        public static B3TokenUpdatePB AnswerTokenUpdate(B3TokenUpdatePB req, bool inUnion)
        {
            var ans = new B3TokenUpdatePB();
            if (!inUnion || req == null || req.Ids == null) return ans;
            var seen = new HashSet<uint>();
            foreach (var id in req.Ids)
            {
                if (ans.Infos.Count >= MaxTokenIds) break;
                if (id == 0 || id > MaxTokenId || !seen.Add(id)) continue;
                ans.Infos.Add(new B3TokenUpdateInfoPB { Token = id, Used = 0, Seconds = 0 });
            }
            return ans;
        }

        /// <summary>2061: no New Guild War system -> empty info for the f1=2 query; anything else -> null (no reply).</summary>
        public static B3NewSynWarPB AnswerNewSynWar(B3NewSynWarPB req)
        {
            if (req == null || req.f1 != 2) return null;
            uint f2 = req.f2 == 2 ? 2u : 0u;
            return new B3NewSynWarPB { f1 = 2, f2 = f2, f3 = 0 };
        }

        /// <summary>2048: no guild has posted a recruit ad (2019 is unhandled) -> page echo, count 0, clear list.
        /// Returns the struct body after the 4-byte header (len 0x2a8 - 4).</summary>
        public static byte[] AnswerRecruitAdBody(uint requestedPage)
        {
            uint page = requestedPage > MaxRecruitPage ? 0u : requestedPage;
            var body = new byte[RecruitAdStructLen - 4];
            BitConverter.GetBytes(page).CopyTo(body, 0);// +4  page
            BitConverter.GetBytes(0u).CopyTo(body, 4);  // +8  count
            BitConverter.GetBytes(0u).CopyTo(body, 8);  // +0xc
            body[12] = 1;                               // +0x10 clear list
            return body;
        }

        // ------------------------------------------------------------------ reply builders (exact handler path, no socket)
        // Each one reads the client request from `stream` and writes the reply into the same packet (null = no reply).
        public static ServerSockets.Packet ReplySynApplyList(ServerSockets.Packet stream)
        {
            // Request is empty; nothing from the client is used.
            return B3Util.BuildProto(stream, AnswerSynApplyList(), IdSynApplyList);
        }

        public static ServerSockets.Packet ReplyLeagueApplyList(ServerSockets.Packet stream)
        {
            B3LeagueApplyListPB req;
            try { req = stream.ProtoBufferDeserialize(new B3LeagueApplyListPB()); } catch { return null; }// malformed: drop
            return B3Util.BuildProto(stream, AnswerLeagueApplyList(req), IdLeagueApplyList);
        }

        public static unsafe ServerSockets.Packet ReplyRecruitAdList(ServerSockets.Packet stream)
        {
            uint page = 0;
            stream.Seek(0);
            ushort len = stream.ReadUInt16();
            if (len >= 8)
            {
                stream.Seek(4);
                page = stream.ReadUInt32();
            }
            byte[] body = AnswerRecruitAdBody(page);
            stream.InitWriter();
            fixed (byte* p = body)
                stream.WriteUnsafe(p, body.Length);
            stream.Finalize(IdRecruitAdList);
            return stream;
        }

        public static ServerSockets.Packet ReplyTokenUpdate(ServerSockets.Packet stream, bool inUnion)
        {
            B3TokenUpdatePB req;
            try { req = stream.ProtoBufferDeserialize(new B3TokenUpdatePB()); } catch { return null; }
            return B3Util.BuildProto(stream, AnswerTokenUpdate(req, inUnion), IdTokenUpdate);
        }

        public static ServerSockets.Packet ReplyNewSynWar(ServerSockets.Packet stream)
        {
            B3NewSynWarPB req;
            try { req = stream.ProtoBufferDeserialize(new B3NewSynWarPB()); } catch { return null; }
            var ans = AnswerNewSynWar(req);
            return ans == null ? null : B3Util.BuildProto(stream, ans, IdNewSynWar);
        }

        // ------------------------------------------------------------------ handlers
        static void Run(Client.GameClient client, int slot, Func<ServerSockets.Packet> reply)
        {
            try
            {
                if (!B3Util.Ready(client) || !B3Throttle.Allow(client, slot)) return;
                var p = reply();
                if (p != null) client.Send(p);
            }
            catch (Exception e) { B3Util.Log(e); }
        }

        [PacketAttribute(IdSynApplyList)]
        private static void OnSynApplyList(Client.GameClient client, ServerSockets.Packet stream)
        {
            Run(client, 0, () => global::Qahr.Mobile.GuildRecruit.GuildApply.ReplyList(client, stream)); // [GuildRecruit 2026-10-14] real applicant list (was ReplySynApplyList)
        }

        [PacketAttribute(IdLeagueApplyList)]
        private static void OnLeagueApplyList(Client.GameClient client, ServerSockets.Packet stream)
        {
            Run(client, 1, () => global::Qahr.Mobile.GuildRecruit.LeagueApply.ReplyList(client, stream)); // [GuildRecruit 2026-10-14] real union application list (was ReplyLeagueApplyList)
        }

        [PacketAttribute(IdRecruitAdList)]
        private static void OnRecruitAdList(Client.GameClient client, ServerSockets.Packet stream)
        {
            Run(client, 2, () => { global::Qahr.Mobile.GuildRecruit.RecruitBoard.SendPage(client, stream); return null; }); // [GuildRecruit 2026-10-14] real recruitment board (was ReplyRecruitAdList)
        }

        [PacketAttribute(IdTokenUpdate)]
        private static void OnTokenUpdate(Client.GameClient client, ServerSockets.Packet stream)
        {
            Run(client, 3, () => global::Qahr.Mobile.GuildRecruit.LeagueOrders.Reply(client, stream)); // [GuildRecruit 2026-10-14] stored order usage / cooldown (was ReplyTokenUpdate)
        }

        [PacketAttribute(IdNewSynWar)]
        private static void OnNewSynWar(Client.GameClient client, ServerSockets.Packet stream)
        {
            Run(client, 4, () => global::Qahr.Mobile.NewGuildWar.NgwEngine.Reply(client, stream)); // [NewGuildWar 2026-10-14] live war info (was ReplyNewSynWar)
        }
    }
}
