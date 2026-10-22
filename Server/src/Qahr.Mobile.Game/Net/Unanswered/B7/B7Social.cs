// [Unanswered B7 2026-10-14] Social & market lists: 2612 CMsgTeamRecruit, 2297 CMsgSceneChat, 2399 CMsgAuctionItem,
// 2441 CMsgAuctionQuery, 2012 CMsgAuctionDirList, 2050 CMsgPresentList, 2090 CMsgPresentUserData.
//
// Server state: there is NO auction house, NO gift center and NO team-recruit board on this server. So every reply here
// is the official EMPTY state, and every action that would move items / gold / CPs or change a team is refused.
// Nothing here grants anything. The uid always comes from client.Player.UID, never from the packet.
//
// Client behaviour (Conquer.exe 7957, Ghidra decomp of the Process / send functions):
//   2612 Process @0xdfeb69, switch f1: 3 = recruit list (f2,f3 -> Lua "%d %d", f5 items) + PostCmd(0xdb4);
//        4 = applicant list (f2,f3, f6 items) + PostCmd(0xdb4); 2 = "new applicant" flag (server push only).
//        Client sends: f1=1 publish (f5[0].f3 activity + text), f1=2 apply (f6[0]), f1=3 list (f3 page, f4, f5[0] filter),
//        f1=4 applicant list (f3), f1=5 accept / f1=7 reject applicant (f6[0].f1), f1=6 revoke.
//        => list requests get an empty page, every write action is refused (needs a real recruit system).
//   2297 Process @0x117f87c: shows each f2 string as a screen (barrage) message ONLY when f1 == the value the client
//        itself puts in f1 when sending (sender @0x1169fd5: f1 = singleton(0x5b9233)+0xc, one string in f2).
//        => relayed to the players of the sender's map; f1 is re-written per receiver (see SceneChatKey).
//   2399 Process @0x117c5ce: f7 items -> list, f3+1 = page shown, f2 -> total, PostCmd(0xcc3). Client (Auction.lua
//        browse @0x11c597b) sends f1 nosuch-flag, f2 category, f3 page, f5, f6 name. => empty f7, f2=0, f3=page.
//   2441 (struct, 0x1c bytes) Process @0xfe4c1a: u16 count @+6, 0x14-byte entries @+8 -> "My Trades" + PostCmd(0xcc3).
//        Client (@0x11c49ab) sends a zeroed 0x1c struct. => zeroed 0x1c reply (count 0), same as the login push.
//   2012 Process @0xe843dd: f1/f2 -> category header, f3 items, PostCmd(0xcc3). Client (@0x11c60c4) sends f1,f2.
//        => echo f1/f2, empty f3.
//   2050 Process @0x11bf878: f1 0/1/2 = list type (dialogs 3667/3665/3664), f2 = clear first, f3 = last page ->
//        refresh PostCmd(0xf6d). Client sends f1 0/1/2 when a gift dialog opens; f1 3/4 = "mark read" (no reply).
//        => f1 echo, f2 = f3 = true, empty f4.
//   2090 Process @0x10c4d57: f1=1 -> stores f2 (a 0/1 gift-receive setting the client toggles @0x736de7).
//        f1 0/2/3 are server pushes (counts / "gift sent" / notify). Client sends f1=1 + f2 toggle.
//        => echo f1=1 + f2 (0/1). Not persisted (no gift system).
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B7
{
    // ------------------------------------------------------------------ contracts (match ProtoStudio schema.json)
    [ProtoContract]
    public class TeamRecruitInfoPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public ulong f6;
        [ProtoMember(7)] public string f7;
        [ProtoMember(8)] public string f8;
    }
    [ProtoContract]
    public class TeamApplyRecruitPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public string f5;
    }
    [ProtoContract]
    public class TeamRecruitPB// 2612
    {
        [ProtoMember(1)] public uint f1;// action
        [ProtoMember(2)] public uint f2;// total
        [ProtoMember(3)] public uint f3;// page
        [ProtoMember(4)] public uint f4;
        [ProtoMember(5)] public List<TeamRecruitInfoPB> f5 = new List<TeamRecruitInfoPB>();
        [ProtoMember(6)] public List<TeamApplyRecruitPB> f6 = new List<TeamApplyRecruitPB>();
    }

    [ProtoContract]
    public class SceneChatPB// 2297
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public List<byte[]> f2 = new List<byte[]>();// raw bytes: relayed exactly as the client encodes them
    }

    [ProtoContract]
    public class AuctionItemInfoPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public ulong f4;
        [ProtoMember(5)] public uint f5;
    }
    [ProtoContract]
    public class AuctionItemPB// 2399
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;// required on the client
        [ProtoMember(2)] public uint f2;// total
        [ProtoMember(3)] public uint f3;// page
        [ProtoMember(4)] public ulong f4;
        [ProtoMember(5)] public uint f5;
        [ProtoMember(6)] public string f6;
        [ProtoMember(7)] public List<AuctionItemInfoPB> f7 = new List<AuctionItemInfoPB>();
    }

    [ProtoContract]
    public class AuctionDirListItemPB
    {
        [ProtoMember(1, IsRequired = true)] public ulong f1;
        [ProtoMember(2, IsRequired = true)] public uint f2;
        [ProtoMember(3)] public uint f3;
    }
    [ProtoContract]
    public class AuctionDirListPB// 2012
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2, IsRequired = true)] public ulong f2;
        [ProtoMember(3)] public List<AuctionDirListItemPB> f3 = new List<AuctionDirListItemPB>();
    }

    [ProtoContract]
    public class PresentRecordPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public string f3;
        [ProtoMember(4)] public uint f4;
    }
    [ProtoContract]
    public class PresentListPB// 2050
    {
        [ProtoMember(1)] public int f1;// list type
        [ProtoMember(2)] public bool f2;// clear first
        [ProtoMember(3)] public bool f3;// last page -> refresh
        [ProtoMember(4)] public List<PresentRecordPB> f4 = new List<PresentRecordPB>();
    }

    [ProtoContract]
    public class PresentUserPB// 2090
    {
        [ProtoMember(1, IsRequired = true)] public int f1;// action (1 written explicitly)
        [ProtoMember(2, IsRequired = true)] public uint f2;// setting 0/1 (0 must be on the wire)
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public uint f4;
    }

    // ------------------------------------------------------------------ throttle
    /// <summary>Per-client, per-slot throttle (dies with the GameClient object).</summary>
    public static class B7Throttle
    {
        sealed class Slots { public readonly int[] Last = new int[8]; public readonly bool[] Used = new bool[8]; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> table = new ConditionalWeakTable<Client.GameClient, Slots>();

        public static bool Allow(Client.GameClient client, int slot, int minIntervalMs)
        {
            if (client == null || slot < 0 || slot >= 8) return false;
            var s = table.GetOrCreateValue(client);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < minIntervalMs) return false;
                s.Used[slot] = true;
                s.Last[slot] = now;
                return true;
            }
        }
    }

    public static class B7Social
    {
        public const ushort IdTeamRecruit = 2612, IdSceneChat = 2297, IdAuctionItem = 2399, IdAuctionQuery = 2441,
            IdAuctionDirList = 2012, IdPresentList = 2050, IdPresentUser = 2090;

        const int ListIntervalMs = 400;
        public const int SceneChatIntervalMs = 5000;
        public const int SceneChatMaxBytes = 180;
        const uint MaxPage = 10000;

        static bool Ready(Client.GameClient client) { return client != null && client.Player != null && client.FullLoading; }

        static void Tell(Client.GameClient client, string text)
        {
            try { client.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static ServerSockets.Packet Build(ServerSockets.Packet stream, object proto, ushort id)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(id);
            return stream;
        }

        // ================================================================== pure reply builders (harness-tested)
        /// <summary>2612: list requests (3, 4) get an empty page; every other action returns null (refused).</summary>
        public static TeamRecruitPB AnswerTeamRecruit(TeamRecruitPB req)
        {
            if (req == null) return null;
            uint page = Math.Min(req.f3, MaxPage);
            if (req.f1 == 3 || req.f1 == 4)
                return new TeamRecruitPB { f1 = req.f1, f2 = 0, f3 = page };
            return null;
        }

        public static AuctionItemPB AnswerAuctionItem(AuctionItemPB req)
        {
            if (req == null) return null;
            return new AuctionItemPB { f1 = req.f1 != 0 ? 1u : 0u, f2 = 0, f3 = Math.Min(req.f3, MaxPage) };
        }

        public static AuctionDirListPB AnswerAuctionDirList(AuctionDirListPB req)
        {
            if (req == null) return null;
            return new AuctionDirListPB { f1 = req.f1, f2 = req.f2 };
        }

        public static PresentListPB AnswerPresentList(PresentListPB req)
        {
            if (req == null || req.f1 < 0 || req.f1 > 2) return null;// 3/4 = mark read: nothing to answer
            return new PresentListPB { f1 = req.f1, f2 = true, f3 = true };
        }

        public static PresentUserPB AnswerPresentUser(PresentUserPB req)
        {
            if (req == null || req.f1 != 1) return null;
            return new PresentUserPB { f1 = 1, f2 = req.f2 != 0 ? 1u : 0u };
        }

        /// <summary>2441 reply body: 24 zero bytes after the header (0x1c packet, entry count 0).</summary>
        public const int AuctionQueryBodySize = 24;

        static readonly string[] stripTokens = { "#70", "#71", "#72", "#73", "#74", "#75", "&245&", "&246&", "&247&", "&248&", "&249&", "&250&" };

        /// <summary>2297 text check: exactly one UTF-8 string, 1..SceneChatMaxBytes bytes, no control chars.
        /// Returns the cleaned UTF-8 bytes or null.</summary>
        public static byte[] CleanSceneText(SceneChatPB req, Func<string, string> filter = null)
        {
            if (req == null || req.f2 == null || req.f2.Count != 1) return null;
            var raw = req.f2[0];
            if (raw == null || raw.Length == 0 || raw.Length > SceneChatMaxBytes) return null;
            string text;
            try { text = new UTF8Encoding(false, true).GetString(raw); } catch { return null; }
            foreach (char c in text) if (char.IsControl(c)) return null;
            foreach (var t in stripTokens) text = text.Replace(t, "");
            if (filter != null) { try { text = filter(text); } catch { } }
            text = text.Trim();
            if (text.Length == 0) return null;
            var outBytes = Encoding.UTF8.GetBytes(text);
            return outBytes.Length <= SceneChatMaxBytes ? outBytes : null;
        }

        /// <summary>How the client fills f1 of 2297: 1 = its own uid, 2 = its map id, 3 = its dynamic map id, 0 = unknown.
        /// Each receiver must get the value ITS client would compare against.</summary>
        public static int SceneChatKey(uint f1, uint uid, uint map, uint dynamicId)
        {
            if (f1 == uid) return 1;
            if (f1 == map) return 2;
            if (dynamicId != 0 && f1 == dynamicId) return 3;
            return 0;
        }

        public static uint SceneChatTarget(int key, uint uid, uint map, uint dynamicId)
        {
            switch (key) { case 1: return uid; case 2: return map; case 3: return dynamicId; default: return 0; }
        }

        // ================================================================== handlers
        [PacketAttribute(IdTeamRecruit)]
        private static void OnTeamRecruit(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                TeamRecruitPB req;
                try { req = stream.ProtoBufferDeserialize(new TeamRecruitPB()); } catch { return; }
                if (req == null) return;
                var ans = AnswerTeamRecruit(req);
                if (ans != null)
                {
                    if (!B7Throttle.Allow(client, 0, ListIntervalMs)) return;
                    client.Send(Build(stream, ans, IdTeamRecruit));
                }
                else if ((req.f1 == 1 || req.f1 == 2) && B7Throttle.Allow(client, 1, 3000))
                {
                    // publish / apply: the client already printed its own "success" tip; tell the truth.
                    Tell(client, "Team recruitment is not available on this server yet.");
                }
                // 5 accept / 6 revoke / 7 reject: nothing exists to change -> ignored.
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdAuctionItem)]
        private static void OnAuctionItem(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client != null) { global::Qahr.Mobile.AuctionHouse.AuctionModule.OnAuctionItem(client, stream); return; }// [Auction 2026-10-14] real auction browse (Base\Auction)
            try
            {
                if (!Ready(client) || !B7Throttle.Allow(client, 2, ListIntervalMs)) return;
                AuctionItemPB req;
                try { req = stream.ProtoBufferDeserialize(new AuctionItemPB()); } catch { return; }
                var ans = AnswerAuctionItem(req);
                if (ans != null) client.Send(Build(stream, ans, IdAuctionItem));
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdAuctionQuery)]
        private static void OnAuctionQuery(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client != null) { global::Qahr.Mobile.AuctionHouse.AuctionModule.OnAuctionQuery(client, stream); return; }// [Auction 2026-10-14] (Base\Auction)
            try
            {
                if (!Ready(client) || !B7Throttle.Allow(client, 3, ListIntervalMs)) return;
                // Read-only "My Trades" list: the request carries no action. Nothing is ever claimed here.
                stream.InitWriter();
                stream.ZeroFill(AuctionQueryBodySize);
                stream.Finalize(IdAuctionQuery);
                client.Send(stream);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdAuctionDirList)]
        private static void OnAuctionDirList(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client != null) { global::Qahr.Mobile.AuctionHouse.AuctionModule.OnDirList(client, stream); return; }// [Auction 2026-10-14] (Base\Auction)
            try
            {
                if (!Ready(client) || !B7Throttle.Allow(client, 4, ListIntervalMs)) return;
                AuctionDirListPB req;
                try { req = stream.ProtoBufferDeserialize(new AuctionDirListPB()); } catch { return; }
                var ans = AnswerAuctionDirList(req);
                if (ans != null) client.Send(Build(stream, ans, IdAuctionDirList));
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdPresentList)]
        private static void OnPresentList(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client != null) { global::Qahr.Mobile.GiftCenter.GiftCenterModule.OnPresentList(client, stream); return; }// [GiftCenter 2026-10-14] real gift lists (Base\GiftCenter)
            try
            {
                if (!Ready(client)) return;
                PresentListPB req;
                try { req = stream.ProtoBufferDeserialize(new PresentListPB()); } catch { return; }
                var ans = AnswerPresentList(req);
                if (ans == null || !B7Throttle.Allow(client, 5, ListIntervalMs)) return;
                client.Send(Build(stream, ans, IdPresentList));
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdPresentUser)]
        private static void OnPresentUser(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client != null) { global::Qahr.Mobile.GiftCenter.GiftCenterModule.OnPresentUser(client, stream); return; }// [GiftCenter 2026-10-14] persisted gift setting (Base\GiftCenter)
            try
            {
                if (!Ready(client)) return;
                PresentUserPB req;
                try { req = stream.ProtoBufferDeserialize(new PresentUserPB()); } catch { return; }
                var ans = AnswerPresentUser(req);
                if (ans == null || !B7Throttle.Allow(client, 6, ListIntervalMs)) return;
                client.Send(Build(stream, ans, IdPresentUser));
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [PacketAttribute(IdSceneChat)]
        private static void OnSceneChat(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                SceneChatPB req;
                try { req = stream.ProtoBufferDeserialize(new SceneChatPB()); } catch { return; }
                if (req == null) return;
                var player = client.Player;
                if (player.IsStillBanned) return;
                if (!B7Throttle.Allow(client, 7, SceneChatIntervalMs)) return;

                int key = SceneChatKey(req.f1, player.UID, player.Map, player.DynamicID);
                if (key == 0) return;// f1 is not something this client could legitimately send

                byte[] text = CleanSceneText(req, s => { Pool.Insults.CheckInsults(ref s); return s; });
                if (text == null) return;

                try { Database.ServerDatabase.LoginQueue.Enqueue("SceneChat [" + player.Name + "] map " + player.Map + ": " + Encoding.UTF8.GetString(text)); } catch { }

                uint map = player.Map, dyn = player.DynamicID;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var p = rec.GetStream();
                    foreach (var other in Pool.GamePoll.Values)
                    {
                        if (other == null || other.Player == null || !other.FullLoading) continue;
                        if (other.Player.Map != map || other.Player.DynamicID != dyn) continue;
                        var msg = new SceneChatPB { f1 = SceneChatTarget(key, other.Player.UID, other.Player.Map, other.Player.DynamicID) };
                        msg.f2.Add(text);
                        other.Send(Build(p, msg, IdSceneChat));
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
    }
}
