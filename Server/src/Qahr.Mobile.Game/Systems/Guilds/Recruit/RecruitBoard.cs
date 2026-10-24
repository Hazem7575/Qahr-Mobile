// [GuildRecruit 2026-10-14] Guild recruitment board (official 7957 structs, reverse-engineered from Conquer.exe):
//   2019 CMsgSynRecruitAdvertising (both, raw len 0x21c) - publish (CDlgIssueRecruit 543, builder @0x684269 / @0xe6c110):
//        +4 u32 guild id, +8 char[0x200] text, +0x208 u32 bid (silver), +0x20c u32 end date (s2c only, YYMMDDhhmm),
//        +0x210 u16 auto-join, +0x212 / +0x214 / +0x216 / +0x218 / +0x21a u16 join requirements (level, reborn, BP,
//        class mask, gender mask as typed in the dialog). Reply = echo, client shows the end date and PostCmd(3244).
//   2048 CMsgSynRecruitAdvertisingList (both, raw len 0x2a8) - board page (CDlgFactionRecruit 542, Process @0x117fe3a):
//        request +4 u32 page. Reply: +4 page, +8 count (one entry fits the 0x400 client buffer), +0xc page count,
//        +0x10 u8 clear list, entry at +0x18: +0 guild id, +4 char[0x200] text, +0x204 char[0x48] guild name,
//        +0x24c char[0x20] leader name, +0x26c u64 guild fund, +0x278 u32 level, +0x27c u32 members,
//        +0x280 u16 auto-join + 5 x u16 requirements (same order as 2019), +0x28c u32 bid. Client ends with PostCmd(3243).
// Cost (official text): at least 500,000 silver, paid from the Guild Fund ("Insufficient Guild Fund"), ad lasts 7 days,
// higher bids are listed first. Only the guild leader can publish. State: Data\GuildRecruit\ads.txt.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.GuildRecruit
{
    public sealed class RecruitAd
    {
        public uint GuildId, Bid, PostedBy;
        public string Text = "";
        public DateTime Posted, Ends;
        public ushort AutoJoin, R212, R214, R216, R218, R21A;

        public string ToLine()
        {
            return string.Join("\t", GuildId, Bid, PostedBy, Posted.Ticks, Ends.Ticks, AutoJoin, R212, R214, R216, R218, R21A,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Text ?? "")));
        }
        public static RecruitAd FromLine(string l)
        {
            var p = l.Split('\t');
            if (p.Length < 12) return null;
            var a = new RecruitAd();
            long t1, t2;
            if (!uint.TryParse(p[0], out a.GuildId) || !uint.TryParse(p[1], out a.Bid) || !uint.TryParse(p[2], out a.PostedBy)
                || !long.TryParse(p[3], out t1) || !long.TryParse(p[4], out t2)) return null;
            a.Posted = new DateTime(t1); a.Ends = new DateTime(t2);
            ushort.TryParse(p[5], out a.AutoJoin); ushort.TryParse(p[6], out a.R212); ushort.TryParse(p[7], out a.R214);
            ushort.TryParse(p[8], out a.R216); ushort.TryParse(p[9], out a.R218); ushort.TryParse(p[10], out a.R21A);
            try { a.Text = Encoding.UTF8.GetString(Convert.FromBase64String(p[11])); } catch { a.Text = ""; }
            return a;
        }
    }

    public static class RecruitBoard
    {
        public const ushort IdPublish = 2019, IdList = 2048;
        public const int PublishLen = 0x21c, ListLen = 0x2a8, EntryBase = 0x18;

        static readonly object Sync = new object();
        static List<RecruitAd> _ads;
        static string StoreFile { get { return RecruitIO.FilePath("ads.txt"); } }

        static List<RecruitAd> Ads
        {
            get
            {
                if (_ads == null)
                {
                    _ads = new List<RecruitAd>();
                    foreach (var l in RecruitIO.ReadLines(StoreFile))
                    {
                        var a = RecruitAd.FromLine(l);
                        if (a != null) _ads.Add(a);
                    }
                }
                return _ads;
            }
        }
        public static void ResetForTests() { lock (Sync) _ads = null; }

        static void Save()
        {
            try { RecruitIO.WriteAtomic(StoreFile, Ads.Select(a => a.ToLine()).ToArray()); }
            catch (Exception e) { RecruitIO.Error(e); }
        }

        static void Purge_NoLock()
        {
            var now = RecruitIO.Now;
            Ads.RemoveAll(a => a.Ends <= now || !Role.Instance.Guild.GuildPoll.ContainsKey(a.GuildId));
        }

        public static RecruitAd ActiveAd(uint guildId)
        {
            lock (Sync)
            {
                Purge_NoLock();
                return Ads.FirstOrDefault(a => a.GuildId == guildId);
            }
        }

        public static List<RecruitAd> ActiveSorted()
        {
            lock (Sync)
            {
                Purge_NoLock();
                return Ads.OrderByDescending(a => a.Bid).ThenBy(a => a.Posted).ToList();
            }
        }

        /// <summary>YYMMDDhhmm as the client decodes it (@0xed3e47: year = v/1e8 % 100 + 2000).</summary>
        public static uint EncodeDate(DateTime d)
        {
            return (uint)((d.Year % 100) * 100000000u + d.Month * 1000000u + d.Day * 10000u + d.Hour * 100u + d.Minute);
        }

        // ------------------------------------------------------------------ 2019 publish
        public enum PublishResult { Ok, NotLeader, BadRequest, BidTooLow, BidTooHigh, NoFund, EmptyText }

        /// <summary>Validates and charges the guild fund, stores the ad. raw = full client packet bytes.</summary>
        public static PublishResult Publish(Client.GameClient user, byte[] raw, out RecruitAd ad)
        {
            ad = null;
            if (raw == null || raw.Length < PublishLen) return PublishResult.BadRequest;
            var p = user.Player;
            if (p.MyGuild == null || p.MyGuildMember == null || p.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                return PublishResult.NotLeader;
            var guild = p.MyGuild;
            var cfg = RecruitIO.Config;

            uint bid = BitConverter.ToUInt32(raw, 0x208);
            if (bid < cfg.AdMinBid) return PublishResult.BidTooLow;
            if (bid > cfg.AdMaxBid) return PublishResult.BidTooHigh;

            int n = 0;
            while (n < 0x200 && raw[8 + n] != 0) n++;
            string text = RecruitIO.CleanText(Encoding.UTF8.GetString(raw, 8, n), cfg.AdTextMax);
            if (text.Length == 0) return PublishResult.EmptyText;

            var now = RecruitIO.Now;
            var nad = new RecruitAd
            {
                GuildId = guild.Info.GuildID,// never the client's +4
                Bid = bid,
                PostedBy = p.UID,
                Text = text,
                Posted = now,
                Ends = now.AddDays(cfg.AdDays),
                AutoJoin = (ushort)(BitConverter.ToUInt16(raw, 0x210) != 0 ? 1 : 0),
                R212 = BitConverter.ToUInt16(raw, 0x212),
                R214 = BitConverter.ToUInt16(raw, 0x214),
                R216 = BitConverter.ToUInt16(raw, 0x216),
                R218 = BitConverter.ToUInt16(raw, 0x218),
                R21A = BitConverter.ToUInt16(raw, 0x21a),
            };

            lock (Sync)
            {
                // deduct before granting the ad; the guild object is shared, so lock it while checking + charging
                lock (guild)
                {
                    if (guild.Info.SilverFund < bid) return PublishResult.NoFund;
                    guild.Info.SilverFund -= bid;
                }
                Purge_NoLock();
                Ads.RemoveAll(a => a.GuildId == nad.GuildId);// a new ad replaces the old one
                Ads.Add(nad);
                Save();
            }
            RecruitIO.Log("ad-publish guild=" + nad.GuildId + " leader=" + p.UID + " bid=" + bid + " fundLeft=" + guild.Info.SilverFund + " ends=" + nad.Ends.ToString("yyyy-MM-dd HH:mm"));
            ad = nad;
            return PublishResult.Ok;
        }

        public static unsafe ServerSockets.Packet BuildPublishEcho(ServerSockets.Packet stream, RecruitAd ad)
        {
            var body = new byte[PublishLen - 4];
            BitConverter.GetBytes(ad.GuildId).CopyTo(body, 0);                 // +4
            var t = Encoding.UTF8.GetBytes(ad.Text ?? "");
            Array.Copy(t, 0, body, 4, Math.Min(t.Length, 0x1ff));            // +8 text (NUL kept)
            BitConverter.GetBytes(ad.Bid).CopyTo(body, 0x208 - 4);
            BitConverter.GetBytes(EncodeDate(ad.Ends)).CopyTo(body, 0x20c - 4);
            BitConverter.GetBytes(ad.AutoJoin).CopyTo(body, 0x210 - 4);
            BitConverter.GetBytes(ad.R212).CopyTo(body, 0x212 - 4);
            BitConverter.GetBytes(ad.R214).CopyTo(body, 0x214 - 4);
            BitConverter.GetBytes(ad.R216).CopyTo(body, 0x216 - 4);
            BitConverter.GetBytes(ad.R218).CopyTo(body, 0x218 - 4);
            BitConverter.GetBytes(ad.R21A).CopyTo(body, 0x21a - 4);
            stream.InitWriter();
            fixed (byte* b = body) stream.WriteUnsafe(b, body.Length);
            stream.Finalize(IdPublish);
            return stream;
        }

        static string PublishText(PublishResult r)
        {
            var cfg = RecruitIO.Config;
            switch (r)
            {
                case PublishResult.NotLeader: return "Only the Guild Leader can release a recruitment advertisement.";
                case PublishResult.BidTooLow: return "You need to pay at least " + cfg.AdMinBid.ToString("N0") + " silver to release an advertisement.";
                case PublishResult.BidTooHigh: return "The bid is too high.";
                case PublishResult.NoFund: return "Insufficient Guild Fund";
                case PublishResult.EmptyText: return "Please enter the advertisement text.";
                default: return "Failed to release the advertisement.";
            }
        }

        public static unsafe byte[] RawOf(ServerSockets.Packet stream)
        {
            int size = stream.Size;
            if (size <= 0) return new byte[0];
            if (size > 0x400) size = 0x400;
            var b = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)stream.Memory, b, 0, size);
            return b;
        }

        [PacketAttribute(IdPublish)]
        private static void OnPublish(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                if (!RecruitUtil.Ready(user) || !RecruitThrottle.Allow(user, 2)) return;
                RecruitAd ad;
                var r = Publish(user, RawOf(stream), out ad);
                if (r != PublishResult.Ok) { RecruitUtil.Msg(user, PublishText(r)); return; }
                user.Send(BuildPublishEcho(stream, ad));
                var d = ad.Ends;
                RecruitUtil.Msg(user, "Your ad will be due at: " + d.Month + "/" + d.Day + "/" + d.Year + " " + d.Hour + ":" + d.Minute.ToString("00"));
            }
            catch (Exception e) { RecruitIO.Error(e); }
        }

        // ------------------------------------------------------------------ 2048 board page
        public const uint MaxPage = 1000;

        /// <summary>One 0x2a8 frame per ad of the page (the client buffer holds one entry), first frame clears the list.
        /// Empty page -> a single frame with count 0 and clear 1.</summary>
        public static List<byte[]> PageBodies(uint requestedPage)
        {
            var cfg = RecruitIO.Config;
            var ads = ActiveSorted();
            int pages = Math.Max(1, (ads.Count + cfg.AdPageSize - 1) / cfg.AdPageSize);
            uint page = requestedPage >= (uint)pages || requestedPage > MaxPage ? 0u : requestedPage;
            var slice = ads.Skip((int)page * cfg.AdPageSize).Take(cfg.AdPageSize).ToList();
            var res = new List<byte[]>();
            if (slice.Count == 0)
            {
                var b = new byte[ListLen - 4];
                BitConverter.GetBytes(page).CopyTo(b, 0);
                BitConverter.GetBytes((uint)pages).CopyTo(b, 8);
                b[12] = 1;
                res.Add(b);
                return res;
            }
            for (int i = 0; i < slice.Count; i++)
            {
                var ad = slice[i];
                var b = new byte[ListLen - 4];// offsets below are packet offsets - 4
                BitConverter.GetBytes(page).CopyTo(b, 0x04 - 4);
                BitConverter.GetBytes(1).CopyTo(b, 0x08 - 4);
                BitConverter.GetBytes((uint)pages).CopyTo(b, 0x0c - 4);
                b[0x10 - 4] = (byte)(i == 0 ? 1 : 0);
                int e = EntryBase - 4;
                Role.Instance.Guild g;
                Role.Instance.Guild.GuildPoll.TryGetValue(ad.GuildId, out g);
                BitConverter.GetBytes(ad.GuildId).CopyTo(b, e + 0x0);
                Put(b, e + 0x4, ad.Text, 0x200);
                Put(b, e + 0x204, g != null ? g.GuildName : "", 0x48);
                Put(b, e + 0x24c, g != null && g.Info != null ? g.Info.LeaderName : "", 0x20);
                BitConverter.GetBytes(g != null && g.Info != null ? (ulong)Math.Max(0L, g.Info.SilverFund) : 0UL).CopyTo(b, e + 0x26c);
                BitConverter.GetBytes(g != null && g.Info != null ? g.Info.Level : 0u).CopyTo(b, e + 0x278);
                BitConverter.GetBytes(g != null && g.Members != null ? (uint)g.Members.Count : 0u).CopyTo(b, e + 0x27c);
                BitConverter.GetBytes(ad.AutoJoin).CopyTo(b, e + 0x280);
                BitConverter.GetBytes(ad.R212).CopyTo(b, e + 0x282);
                BitConverter.GetBytes(ad.R214).CopyTo(b, e + 0x284);
                BitConverter.GetBytes(ad.R216).CopyTo(b, e + 0x286);
                BitConverter.GetBytes(ad.R218).CopyTo(b, e + 0x288);
                BitConverter.GetBytes(ad.R21A).CopyTo(b, e + 0x28a);
                BitConverter.GetBytes(ad.Bid).CopyTo(b, e + 0x28c);
                res.Add(b);
            }
            return res;
        }

        static void Put(byte[] b, int off, string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return;
            var t = Encoding.UTF8.GetBytes(s);
            Array.Copy(t, 0, b, off, Math.Min(t.Length, max - 1));
        }

        public static unsafe ServerSockets.Packet Frame(ServerSockets.Packet stream, byte[] body)
        {
            stream.InitWriter();
            fixed (byte* p = body) stream.WriteUnsafe(p, body.Length);
            stream.Finalize(IdList);
            return stream;
        }

        public static uint RequestedPage(ServerSockets.Packet stream)
        {
            if (stream.Size < 8) return 0;
            stream.Seek(0);
            ushort len = stream.ReadUInt16();
            if (len < 8) return 0;
            stream.Seek(4);
            return stream.ReadUInt32();
        }

        /// <summary>2048 handler body (called from the B3 delegate): sends every frame of the page.</summary>
        public static void SendPage(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint page = RequestedPage(stream);
            foreach (var body in PageBodies(page))
                user.Send(Frame(stream, body));
        }
    }
}
