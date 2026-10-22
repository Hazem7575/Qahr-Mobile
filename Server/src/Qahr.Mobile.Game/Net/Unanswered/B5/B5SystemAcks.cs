// [Unanswered B5 2026-10-14] Login / client-system / anti-cheat channel.
//
// Real replies : 2294 CMsgLoginNotice (MOTD), 2227 CMsgData type 4, 2151 CMsgStatisticActive (query), 2430 CMsgInnerWebPage.
// No-ops       : 2036 CMsgPing, 2364 CMsgSdkLoginSign, 2370 CMsgTQP, 2562 CMsgInstanceDownCountEncrypt,
//                2316 CMsgSafeHeat, 2049 CMsgPokerFriendAction, 2318 CMsgWeather.
//
// Nothing here grants items, currency, stats or teleports. Claims (2151 f1=2) are refused. The uid is never read from a packet.
//
// Client behaviour (Conquer.exe 7957, Ghidra decomp):
//   2294 Process @0x117e721: reads struct u32 @+4 (id), char[256] @+8 (title), char[256] @+0x108 (content) and
//        hands them to the notice board. Send @0x1169cda: len 0x208, u32 id @+4 (client sends 0 + empty strings on login).
//        => reply the same 0x208 struct with the server MOTD from ServerConfig.ini [LoginNotice].
//   2227 Process @0xf5642a: type 4 -> FUN_0117c24f(f2 key, f3 value): fires a "value changed" event (0xf) when the
//        client's cached value for the key differs. Client asks with f1=4 f2=key (periodic, @0x117c49c).
//        => reply f1=4, f2=key, f3=0 (no server value). Type 3 is the client's echo of a server push: ignored.
//   2151 Process @0xe880e7: if Statisticactive_GetIsSaveRebate(f2) is false, each f5 event updates the progress bars, then
//        PostCmd(0x102d); else PostCmd(0xf6d,0xe27). => query (f1=0): reply f1=0, f2=activeId, empty f5 (no progress).
//        f1=2 (claim from a message box) is refused: no statistic-activity system exists.
//   2430 Process @0x11be69d: f1 0/2 -> needs f4 count >= 1 and f3 == 1 to open a URL, else logs "Open InnerWeb Fail";
//        f1 1/3 toggle a web-page state with f2. => f1 0/2: reply f1/f2 echo, f3=0 (no URL configured, nothing opens).
//        f1 1/3 is not echoed (would toggle client state the server never set). Client strings are never echoed.
using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Unanswered.B5
{
    // ------------------------------------------------------------------ contracts (match ProtoStudio schema.json)
    [ProtoContract]
    public class B5EventDataPB // CEventDataPB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2, IsRequired = true)] public uint f2;
        [ProtoMember(3, IsRequired = true)] public uint f3;
    }

    [ProtoContract]
    public class B5StatisticActivePB // 2151 CMsgStatisticActivePB
    {
        [ProtoMember(1, IsRequired = true)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public uint f3;
        [ProtoMember(4)] public ulong f4;
        [ProtoMember(5)] public List<B5EventDataPB> f5;
    }

    [ProtoContract]
    public class B5InlinePagePB // 2430 CMsgInlinePagePB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint f2;
        [ProtoMember(3)] public int f3;
        [ProtoMember(4)] public List<string> f4;
    }

    [ProtoContract]
    public class B5DataInPB // 2227 CMsgDataPB (request side, only the fields we read)
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2)] public uint Year;
        [ProtoMember(3)] public uint Month;
        [ProtoMember(4)] public uint DayOfYear;
        [ProtoMember(5)] public uint DayOfMonth;
        [ProtoMember(6)] public uint Hour;
        [ProtoMember(7)] public uint Minute;
        [ProtoMember(8)] public uint Second;
        [ProtoMember(9)] public ulong f9;
        [ProtoMember(10)] public uint f10;
    }

    public static class B5SystemAcks
    {
        public const ushort MsgPing = 2036;
        public const ushort MsgPokerFriendAction = 2049;
        public const ushort MsgStatisticActive = 2151;
        public const ushort MsgData = 2227;
        public const ushort MsgLoginNotice = 2294;
        public const ushort MsgSafeHeat = 2316;
        public const ushort MsgWeather = 2318;
        public const ushort MsgSdkLoginSign = 2364;
        public const ushort MsgTQP = 2370;
        public const ushort MsgInnerWebPage = 2430;
        public const ushort MsgInstanceDownCountEncrypt = 2562;

        public const int LoginNoticeSize = 0x208; // 4 header + u32 id + char[256] title + char[256] content
        private const int NoticeText = 256;

        // ------------------------------------------------------------------ rate limit (per uid + packet id)
        private static readonly Dictionary<Tuple<uint, ushort, uint>, long> lastReply = new Dictionary<Tuple<uint, ushort, uint>, long>();
        private static readonly object rlLock = new object();

        /// <summary>True when this uid may get another reply for (packet id, sub key) now.</summary>
        public static bool Allow(uint uid, ushort id, uint sub, int minMs)
        {
            var k = Tuple.Create(uid, id, sub);
            long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond; // works on the legacy .NET Framework csproj too
            lock (rlLock)
            {
                long last;
                if (lastReply.TryGetValue(k, out last) && now - last < minMs)
                    return false;
                lastReply[k] = now;
                if (lastReply.Count > 50000) lastReply.Clear(); // hard cap; worst case one extra reply
                return true;
            }
        }

        private static bool Ready(Client.GameClient client)
        {
            return client != null && client.Player != null && client.Player.UID != 0 && client.Socket != null; // uid 0 = not logged in yet
        }

        private static void SendProto(Client.GameClient client, ushort id, object proto)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(proto);
                stream.Finalize(id);
                client.Send(stream);
            }
        }

        // ------------------------------------------------------------------ 2294 login notice (MOTD)
        public static string NoticeTitle()
        {
            string def = "Welcome";
            try
            {
                if (!string.IsNullOrEmpty(Program.ServerConfig.ServerName))
                    def = "Welcome to " + Program.ServerConfig.ServerName;
            }
            catch { }
            return SharedConfig.String("LoginNotice", "Title", def);
        }

        public static string NoticeContent()
        {
            return SharedConfig.String("LoginNotice", "Content", "Have fun and play fair.");
        }

        /// <summary>Writes 255 UTF-8 bytes max (cut on a char boundary) and zero-fills to 256 so the C string ends.</summary>
        public static void WriteFixed(ServerSockets.Packet stream, string text, int size)
        {
            byte[] raw = Encoding.UTF8.GetBytes(text ?? "");
            int n = Math.Min(raw.Length, size - 1);
            while (n > 0 && n < raw.Length && (raw[n] & 0xC0) == 0x80) n--; // do not split a UTF-8 sequence
            for (int i = 0; i < n; i++) stream.Write(raw[i]);
            stream.ZeroFill(size - n);
        }

        public static ServerSockets.Packet LoginNoticeCreate(ServerSockets.Packet stream, uint id, string title, string content)
        {
            stream.InitWriter();
            stream.Write(id);
            WriteFixed(stream, title, NoticeText);
            WriteFixed(stream, content, NoticeText);
            stream.Finalize(MsgLoginNotice);
            return stream;
        }

        [PacketAttribute(MsgLoginNotice)]
        private static void OnLoginNotice(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                if (SharedConfig.Int("LoginNotice", "Enabled", 1) == 0) return;
                if (!Allow(client.Player.UID, MsgLoginNotice, 0, 10000)) return; // the client asks twice on login
                string content = NoticeContent();
                if (string.IsNullOrEmpty(content)) return;
                uint id = (uint)Math.Max(0, SharedConfig.Int("LoginNotice", "Id", 1));
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var outp = rec.GetStream();
                    LoginNoticeCreate(outp, id, NoticeTitle(), content);
                    client.Send(outp);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ 2227 CMsgData
        public static B5DataInPB HandleData(uint uid, B5DataInPB req)
        {
            if (req == null || req.Type != 4) return null; // type 3 = echo of a server push; others are server-only
            if (!Allow(uid, MsgData, req.Year, 1000) || !Allow(uid, MsgData, uint.MaxValue, 200)) return null; // per key + max 5/s overall
            return req;
        }

        [PacketAttribute(MsgData)]
        private static void OnData(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var req = HandleData(client.Player.UID, stream.ProtoBufferDeserialize(new B5DataInPB()));
                if (req == null) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var outp = rec.GetStream();
                    Game.MsgServer.MsgBuilder.ServerDataValueCreate(outp, 4, req.Year, 0);
                    client.Send(outp);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ 2151 CMsgStatisticActive
        public static B5StatisticActivePB HandleStatisticActive(uint uid, B5StatisticActivePB req)
        {
            if (req == null || req.f1 != 0) return null;           // f1=2 = claim: refused, no system
            if (req.f2 > 100000) return null;                      // activity ids are small (271)
            if (!Allow(uid, MsgStatisticActive, req.f2, 2000) || !Allow(uid, MsgStatisticActive, uint.MaxValue, 200)) return null;
            return new B5StatisticActivePB { f1 = 0, f2 = req.f2 }; // empty f5 = no progress
        }

        [PacketAttribute(MsgStatisticActive)]
        private static void OnStatisticActive(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var reply = HandleStatisticActive(client.Player.UID, stream.ProtoBufferDeserialize(new B5StatisticActivePB()));
                if (reply != null) SendProto(client, MsgStatisticActive, reply);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ 2430 CMsgInnerWebPage
        public static B5InlinePagePB HandleInnerWebPage(uint uid, B5InlinePagePB req)
        {
            if (req == null || (req.f1 != 0 && req.f1 != 2)) return null; // 1/3 = toggle, not answered
            if (!Allow(uid, MsgInnerWebPage, 0, 1000)) return null;
            // QahrMobile: the phone's WebHub button sends f1=0 f2=2 (mobile 10412). Data\mobile_webpages.txt "page url" gives
            // the link ({uid} is filled in); f3=1 + f4=[url] opens it, f3=0 = "Open InnerWeb Fail".
            string url = WebPageUrl(req.f2, uid);
            if (url == null) return new B5InlinePagePB { f1 = req.f1, f2 = req.f2, f3 = 0 };
            return new B5InlinePagePB { f1 = req.f1, f2 = req.f2, f3 = 1, f4 = new List<string> { url } };
        }

        static string WebPageUrl(uint page, uint uid)
        {
            try
            {
                string file = System.IO.Path.Combine(Program.ServerConfig.DbLocation, "mobile_webpages.txt");
                if (!System.IO.File.Exists(file)) return null;
                foreach (string line in System.IO.File.ReadAllLines(file))
                {
                    string l = line.Trim();
                    if (l.Length == 0 || l[0] == '#') continue;
                    int sp = l.IndexOfAny(new[] { ' ', '\t' });
                    uint id;
                    if (sp <= 0 || !uint.TryParse(l.Substring(0, sp), out id) || id != page) continue;
                    return l.Substring(sp + 1).Trim().Replace("{uid}", uid.ToString());
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return null;
        }

        [PacketAttribute(MsgInnerWebPage)]
        private static void OnInnerWebPage(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var reply = HandleInnerWebPage(client.Player.UID, stream.ProtoBufferDeserialize(new B5InlinePagePB()));
                if (reply != null) SendProto(client, MsgInnerWebPage, reply);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ no-ops (client expects no reply)
        // 2036 ping: Process is trivial, no reply.            2364 SDK login sign: third-party SDK, not used here.
        // 2370 TQP: TQProtect blob, our TQProtect uses 0x2328+. 2562: TQNDP first-message token (handled pre-DH in
        //      SecuritySocket); a no-op here only stops the log. 2316: anti-cheat heartbeat, the client only answers a
        //      server 2316 (we never send one). 2049 f1=7: auto status, no client branch waits on it (no poker system).
        // 2318 weather: server -> client only.
        [PacketAttribute(MsgPing)]
        private static void OnPing(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgSdkLoginSign)]
        private static void OnSdkLoginSign(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgTQP)]
        private static void OnTQP(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgInstanceDownCountEncrypt)]
        private static void OnInstanceDownCountEncrypt(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgSafeHeat)]
        private static void OnSafeHeat(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgPokerFriendAction)]
        private static void OnPokerFriendAction(Client.GameClient client, ServerSockets.Packet stream) { }

        [PacketAttribute(MsgWeather)]
        private static void OnWeather(Client.GameClient client, ServerSockets.Packet stream) { }
    }
}
