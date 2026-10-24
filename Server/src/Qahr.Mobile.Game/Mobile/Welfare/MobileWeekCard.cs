// ============================================================================
//  QahrMobile: Welfare > "Weekly Cards" (client CDlgWeekdayCard, state in CMonthAndFundMgr).
//
//  Card list/config: client ini\month_card_type.ini rows 10001..10006 + ini\luacfg\weekcard.dat; server copy
//  Data\mobile_weekcard.txt (re-read when saved).
//  - 10001 Primary  (month_card_type type 2): free, needs 240 active points today. Buy button -> CMsgMBAction 258
//    with the Lua string "</F>MonthCard_BuyFreeMonth" (routed from MsgDataPacket.AnimaCrest -> OnLua).
//  - 10002 Advanced / 10003 Supreme / 10004 bundle (type 0): the buy button goes through BuyEmoneyMgr ->
//    CSdkManager (= ChannelPayment, our QahrPay page, productId 9/10/11..13). The server never sees that buy; a card is
//    granted with Grant() or admin.py eventdata (see the data file).
//
//  Server -> client
//    MsgUserMonthCardInfo 2275 (mobile 10394): repeated CardItemPB #1 { #1 Type = card id, #2 TimeLastGet int64 =
//      the time the next daily claim opens (claimable when server time > it), #3 TimeOut int64 = card end,
//      #4 AllowBuyCount = purchases left }. Replaces the client's whole card map (RefreshMonthCardInfo).
//    MsgUserMonthCardAction 2251 (mobile 10393): #1 action, #2 repeated int64 n64data.
//      claim (1): {card, nextClaim}; buy (0): {card, 1, nextClaim, timeOut, allowBuy}. n64data[1] == 0 = failed
//      (STR_FUND_MONTH_TIPS4/2), else TIPS3 "Claimed successfully!" / TIPS1 "Purchased successfully!".
//  Client -> server: MsgUserMonthCardAction {#1 action (1 = claim), #2 [card id]} (CMonthAndFundMgr::GetCardAction),
//    routed from Program (packet loop) before the registry: the old PC handler is commented out.
//  All times are compared with CHero::CalcServerTimeStamp = MsgData (2227) type 0 field 9 (unix seconds), which
//  the server did not send before; ServerTimerCreate now sends it (TimeProto below).
//
//  Monthly Cards (client CDlgFundMonth, ini\luacfg\fund.dat fund_month_reward): cards 376 Basic (iap 782111) and
//  377 Deluxe (iap 782117), 30 days; same 2251/2275 packets (GetCardAction 0 = buy, 1 = claim). "onbuy" items are given
//  once per Grant (c:pend<card>, delivered online or at login); the login "claim all" popup (CFDlgMonthGet) sends a
//  claim per card. GrantProduct(uid, iap id) maps the real-money products ("product" lines) to cards.
//  Items missing from itemtype.txt are owed (c:owed<itemId>) and delivered at login once the item type exists.
//  State: EventStore event "mweekcard": t:to<id> card end, t:next<id> next claim, c:buys<id> purchases.
//  Written (flushNow) before items are given; nothing is given when the write fails.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ProtoBuf;
using Qahr.Mobile.EventData;

namespace Qahr.Mobile.Phone
{
    public static class MobileWeekCard
    {
        const string Ev = "mweekcard";
        public const ushort PacketAction = 2251, PacketInfo = 2275;
        public const string LuaBuyFree = "</F>MonthCard_BuyFreeMonth";

        [ProtoContract]
        public class CardItem
        {
            [ProtoMember(1, IsRequired = true)] public uint Type;
            [ProtoMember(2, IsRequired = true)] public long TimeLastGet;
            [ProtoMember(3, IsRequired = true)] public long TimeOut;
            [ProtoMember(4, IsRequired = true)] public uint AllowBuyCount;
        }
        [ProtoContract]
        public class CardInfo
        {
            [ProtoMember(1)] public List<CardItem> Items = new List<CardItem>();
        }
        [ProtoContract]
        public class CardAction
        {
            [ProtoMember(1)] public uint Action;
            [ProtoMember(2)] public List<long> Data = new List<long>();
        }
        /// <summary>MsgData 2227 type 0 (server time) with field 9 = unix seconds (CHero::SetServerTimeStamp).</summary>
        [ProtoContract]
        public class TimeProto
        {
            [ProtoMember(1, IsRequired = true)] public uint Type;
            [ProtoMember(2, IsRequired = true)] public uint Year;
            [ProtoMember(3, IsRequired = true)] public uint Month;
            [ProtoMember(4, IsRequired = true)] public uint DayOfYear;
            [ProtoMember(5, IsRequired = true)] public uint DayOfMonth;
            [ProtoMember(6, IsRequired = true)] public uint Hour;
            [ProtoMember(7, IsRequired = true)] public uint Minute;
            [ProtoMember(8, IsRequired = true)] public uint Second;
            [ProtoMember(9, IsRequired = true)] public ulong Stamp;
        }
        public static TimeProto Time()
        {
            DateTime now = DateTime.Now;
            return new TimeProto
            {
                Year = (uint)(now.Year - 1900), Month = (uint)(now.Month - 1), DayOfYear = (uint)now.DayOfYear, DayOfMonth = (uint)now.Day,
                Hour = (uint)now.Hour, Minute = (uint)now.Minute, Second = (uint)now.Second, Stamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };
        }
        sealed class Card { public uint Id; public int Days = 7, Limit = 99, Points; public bool Free; public List<RewardItem> Daily = new List<RewardItem>(), Random = new List<RewardItem>(); public List<uint> Bundle; }

        /// <summary>Data\mobile_weekcard.txt: the cards, the purchase rewards ("onbuy") and iap product -> card ("product").</summary>
        sealed class Config
        {
            public readonly Dictionary<uint, Card> Cards = new Dictionary<uint, Card>();
            public readonly Dictionary<uint, List<RewardItem>> OnBuy = new Dictionary<uint, List<RewardItem>>();
            public readonly Dictionary<uint, uint> Products = new Dictionary<uint, uint>();

            Card Get(uint id)
            {
                Card c;
                if (!Cards.TryGetValue(id, out c)) Cards[id] = c = new Card { Id = id };
                return c;
            }

            public static Config Parse(string[] lines)
            {
                var n = new Config();
                foreach (var line in DataFile.Clean(lines))
                {
                    var p = DataFile.Words(line);
                    if (p.Length < 2) continue;
                    switch (p[0].ToLowerInvariant())
                    {
                        case "onbuy": n.OnBuy[uint.Parse(p[1])] = Rewards.Parse(p, 2); break;
                        case "product": if (p.Length > 2) n.Products[uint.Parse(p[1])] = uint.Parse(p[2]); break;
                        case "card":
                            {
                                var card = n.Get(uint.Parse(p[1]));
                                if (p.Length > 2) card.Days = int.Parse(p[2]);
                                if (p.Length > 3) card.Limit = int.Parse(p[3]);
                                card.Free = p.Length > 4 && p[4].Equals("free", StringComparison.OrdinalIgnoreCase);
                                if (p.Length > 5) card.Points = int.Parse(p[5]);
                                break;
                            }
                        case "daily": n.Get(uint.Parse(p[1])).Daily = Rewards.Parse(p, 2); break;
                        case "random": n.Get(uint.Parse(p[1])).Random = Rewards.Parse(p, 2); break;
                        case "bundle": n.Get(uint.Parse(p[1])).Bundle = p.Skip(2).Select(uint.Parse).ToList(); break;
                    }
                }
                return n;
            }
        }

        static readonly DataFile<Config> Settings = new DataFile<Config>("mobile_weekcard.txt", "WeekCard", Config.Parse);
        static readonly Random rnd = new Random();
        static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
        static long Midnight(int addDays) { return new DateTimeOffset(DateTime.Today.AddDays(addDays)).ToUnixTimeSeconds(); }

        /// <summary>Delivers a confirmed real-money product (iap.ini id, e.g. 782111 Basic Monthly Card) as its card.
        /// The mapping is the "product" lines of Data\mobile_weekcard.txt. false = unknown product or save failed.</summary>
        public static bool GrantProduct(uint uid, uint productId, string why)
        {
            Load();
            uint card;
            if (!Settings.Value.Products.TryGetValue(productId, out card)) { LogUid(uid, "GRANT product " + productId + " has no card (" + why + ")"); return false; }
            return Grant(uid, card, why + " product " + productId);
        }

        /// <summary>Gives the items whose type exists (bag space checked by the caller). Items missing from itemtype.txt
        /// are kept as owed (c:owed&lt;id&gt;) and delivered by DeliverOwed once the item type is added.</summary>
        static List<string> GiveOrOwe(Client.GameClient c, ServerSockets.Packet stream, List<RewardItem> items)
        {
            var have = items.Where(i => Pool.ItemsBase.ContainsKey(i.Id)).ToList();
            var missing = items.Where(i => !Pool.ItemsBase.ContainsKey(i.Id)).ToList();
            if (missing.Count > 0)
            {
                try { EventStore.Edit(c.Player.UID, Ev, x => { foreach (var m in missing) x.Counters["owed" + m.Id] = x.Counter("owed" + m.Id) + m.Count; }, true); }
                catch (EventStore.EditFailed) { }
                PlayerMsg.Tell(c, "Not on this server yet (kept for you, delivered later): " + string.Join(", ", missing.Select(m => m.Id + " x" + m.Count)) + ".");
                MyConsole.WriteLine("[WeekCard] item(s) " + string.Join(",", missing.Select(m => m.Id)) + " not in itemtype.txt, owed to " + c.Player.Name);
            }
            return Rewards.Give(c, stream, have, "WeekCard");
        }

        /// <summary>Purchase rewards of granted cards (c:pend&lt;card&gt;) and owed items, when the bag has room.</summary>
        static void DeliverPending(Client.GameClient c, ServerSockets.Packet stream)
        {
            uint uid = c.Player.UID;
            var s = EventStore.Section(uid, Ev);
            foreach (var kv in s.Counters.Where(k => k.Key.StartsWith("pend", StringComparison.Ordinal) && k.Value > 0).ToList())
            {
                uint card; List<RewardItem> items;
                if (!uint.TryParse(kv.Key.Substring(4), out card) || !Settings.Value.OnBuy.TryGetValue(card, out items)) continue;
                int slots = Rewards.Slots(items);
                if (slots > 0 && !Rewards.HasRoom(c, slots)) { PlayerMsg.Tell(c, "Make " + slots + " free space(s) in your inventory to receive your card's purchase reward."); continue; }
                bool ok;
                try { ok = EventStore.Edit(uid, Ev, x => { long n = x.Counter(kv.Key); if (n <= 0) return Tuple.Create(false, false); if (n == 1) x.Counters.Remove(kv.Key); else x.Counters[kv.Key] = n - 1; return Tuple.Create(true, true); }, true); }
                catch (EventStore.EditFailed) { continue; }
                if (!ok) continue;
                var given = GiveOrOwe(c, stream, items);
                if (given.Count > 0) PlayerMsg.Tell(c, "Card purchase reward: you received " + string.Join(", ", given) + ".");
                Log(c, "ONBUY " + card + " gave [" + string.Join(", ", given) + "]");
            }
            foreach (var kv in s.Counters.Where(k => k.Key.StartsWith("owed", StringComparison.Ordinal) && k.Value > 0).ToList())
            {
                uint id;
                if (!uint.TryParse(kv.Key.Substring(4), out id) || !Pool.ItemsBase.ContainsKey(id)) continue;
                var items = new List<RewardItem> { new RewardItem { Id = id, Count = (ushort)Math.Min(ushort.MaxValue, kv.Value), Bound = true } };
                int slots = Rewards.Slots(items);
                if (slots > 0 && !Rewards.HasRoom(c, slots)) continue;
                try { EventStore.Edit(uid, Ev, x => x.Counters.Remove(kv.Key), true); }
                catch (EventStore.EditFailed) { continue; }
                var given = Rewards.Give(c, stream, items, "WeekCard");
                Log(c, "OWED gave [" + string.Join(", ", given) + "]");
            }
        }

        // ------------------------------------------------------------------ hooks
        /// <summary>Program packet loop: MsgUserMonthCardAction from a mobile client. true = handled.</summary>
        public static bool TryHandle(Client.GameClient c, ServerSockets.Packet stream, ushort packetId)
        {
            if (packetId != PacketAction || c == null || c.Player == null || !MobileClient.Is(c)) return false;
            try
            {
                var a = stream.ProtoBufferDeserialize<CardAction>(new CardAction());
                uint card = a.Data != null && a.Data.Count > 0 ? (uint)a.Data[0] : 0;
                lock (c)
                {
                    if (a.Action == 1) Claim(c, stream, card);
                    else { Log(c, "unknown action " + a.Action + " data [" + string.Join(",", a.Data ?? new List<long>()) + "]"); SendInfo(c, stream); }
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[WeekCard] " + e); }
            return true;
        }

        /// <summary>MsgDataPacket.AnimaCrest (258 Lua call). true = handled.</summary>
        public static bool OnLua(Client.GameClient c, ServerSockets.Packet stream, string[] strings)
        {
            if (strings == null || !strings.Any(s => s != null && s.StartsWith(LuaBuyFree, StringComparison.Ordinal))) return false;
            if (c == null || c.Player == null || !MobileClient.Is(c)) return false;
            try { lock (c) BuyFree(c, stream); }
            catch (Exception e) { MyConsole.WriteLine("[WeekCard] " + e); }
            return true;
        }

        /// <summary>Login (after the empty MsgUserMonthCardInfo): the player's cards.</summary>
        public static void OnLogin(Client.GameClient c, ServerSockets.Packet stream)
        {
            try { if (c != null && c.Player != null && MobileClient.Is(c)) { Load(); DeliverPending(c, stream); SendInfo(c, stream); } }
            catch (Exception e) { MyConsole.WriteLine("[WeekCard] " + e); }
        }

        /// <summary>Grants a card (or a bundle) for its days, e.g. after a real-money payment. Extends a running card.</summary>
        public static bool Grant(uint uid, uint cardId, string why)
        {
            var cf = Load();
            Card card;
            if (!cf.TryGetValue(cardId, out card)) return false;
            if (card.Bundle != null) { bool ok = true; foreach (var id in card.Bundle) ok &= Grant(uid, id, why + " (bundle " + cardId + ")"); return ok; }
            long now = Now;
            try
            {
                EventStore.Edit(uid, Ev, s =>
                {
                    long to; s.Times.TryGetValue("to" + cardId, out to);
                    long from = to > now ? to : Midnight(0);
                    s.Times["to" + cardId] = from + card.Days * 86400L;
                    s.Counters["buys" + cardId] = s.Counter("buys" + cardId) + 1;
                    if (Settings.Value.OnBuy.ContainsKey(cardId)) s.Counters["pend" + cardId] = s.Counter("pend" + cardId) + 1;// purchase reward, given online
                }, true);
            }
            catch (EventStore.EditFailed) { return false; }
            Client.GameClient c;
            if (Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && MobileClient.Is(c))
                using (var rec = new ServerSockets.RecycledPacket())
                    lock (c) { DeliverPending(c, rec.GetStream()); SendInfo(c, rec.GetStream()); }
            LogUid(uid, "GRANT " + cardId + " " + why);
            return true;
        }

        // ------------------------------------------------------------------ actions
        static void BuyFree(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cf = Load();
            var card = cf.Values.FirstOrDefault(x => x.Free);
            if (card == null) { PlayerMsg.Tell(c, "This card is not available."); return; }
            uint uid = c.Player.UID;
            var s = EventStore.Section(uid, Ev);
            long now = Now, to, next;
            s.Times.TryGetValue("to" + card.Id, out to);
            if (to > now) { PlayerMsg.Tell(c, "Your card is still running."); SendInfo(c, stream); return; }
            if (s.Counter("buys" + card.Id) >= card.Limit) { PlayerMsg.Tell(c, "Purchase limit reached!"); SendInfo(c, stream); return; }
            uint pts = MobileDaily.TodayPoints(uid);
            if (pts < card.Points) { PlayerMsg.Tell(c, "You need " + card.Points + " active points today (you have " + pts + ")."); SendAction(c, stream, 0, card.Id, 0); return; }
            try
            {
                EventStore.Edit(uid, Ev, x =>
                {
                    x.Times["to" + card.Id] = Midnight(card.Days);
                    x.Times.Remove("next" + card.Id);
                    x.Counters["buys" + card.Id] = x.Counter("buys" + card.Id) + 1;
                }, true);
            }
            catch (EventStore.EditFailed) { PlayerMsg.Tell(c, "The purchase could not be saved. Please try again later."); return; }
            s = EventStore.Section(uid, Ev);
            s.Times.TryGetValue("to" + card.Id, out to);
            s.Times.TryGetValue("next" + card.Id, out next);
            SendAction(c, stream, 0, card.Id, 1, next, to, Math.Max(0, card.Limit - s.Counter("buys" + card.Id)));
            SendInfo(c, stream);
            Log(c, "BUY free " + card.Id + " (points " + pts + ") until " + to);
        }

        static void Claim(Client.GameClient c, ServerSockets.Packet stream, uint cardId)
        {
            var cf = Load();
            Card card;
            uint uid = c.Player.UID;
            if (!cf.TryGetValue(cardId, out card) || card.Bundle != null) { SendAction(c, stream, 1, cardId, 0); return; }
            var s = EventStore.Section(uid, Ev);
            long now = Now, to, next;
            s.Times.TryGetValue("to" + cardId, out to);
            s.Times.TryGetValue("next" + cardId, out next);
            if (to <= now) { PlayerMsg.Tell(c, "Your card has expired."); SendAction(c, stream, 1, cardId, 0); SendInfo(c, stream); return; }
            if (next > now) { PlayerMsg.Tell(c, "You have already claimed today's reward."); SendInfo(c, stream); return; }
            var items = new List<RewardItem>(card.Daily);
            if (card.Random.Count > 0) items.Add(card.Random[rnd.Next(card.Random.Count)]);
            // QahrMobile: items missing from itemtype.txt no longer block the claim (a missed day is lost): they are owed
            int slots = Rewards.Slots(items);
            if (slots > 0 && !Rewards.HasRoom(c, slots)) { PlayerMsg.Tell(c, "Please keep at least " + slots + " empty space(s) in your inventory."); SendAction(c, stream, 1, cardId, 0); return; }
            long newNext = Midnight(1);
            try
            {
                bool ok = EventStore.Edit(uid, Ev, x =>
                {
                    long n; x.Times.TryGetValue("next" + cardId, out n);
                    if (n > now) return Tuple.Create(false, false);
                    x.Times["next" + cardId] = newNext;
                    return Tuple.Create(true, true);
                }, true);
                if (!ok) { SendInfo(c, stream); return; }
            }
            catch (EventStore.EditFailed) { PlayerMsg.Tell(c, "The reward could not be saved. Please try again later."); SendAction(c, stream, 1, cardId, 0); return; }
            var given = GiveOrOwe(c, stream, items);
            SendAction(c, stream, 1, cardId, newNext);
            SendInfo(c, stream);
            if (given.Count > 0) PlayerMsg.Tell(c, "Weekly card: you received " + string.Join(", ", given) + ".");
            Log(c, "CLAIM " + cardId + " gave [" + string.Join(", ", given) + "]");
        }

        // ------------------------------------------------------------------ packets
        public static void SendInfo(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cf = Load();
            var s = EventStore.Section(c.Player.UID, Ev);
            var info = new CardInfo();
            foreach (var card in cf.Values.Where(x => x.Bundle == null).OrderBy(x => x.Id))
            {
                long to, next;
                s.Times.TryGetValue("to" + card.Id, out to);
                s.Times.TryGetValue("next" + card.Id, out next);
                long buys = s.Counter("buys" + card.Id);
                if (to == 0 && buys == 0) continue;// never had it: the client shows the buy button
                info.Items.Add(new CardItem { Type = card.Id, TimeLastGet = next, TimeOut = to, AllowBuyCount = (uint)Math.Max(0, card.Limit - buys) });
            }
            // the time stamp the card times are compared with (sent at login too; repeated here for running sessions)
            stream.InitWriter();
            stream.ProtoBufferSerialize(Time());
            stream.Finalize(Game.GamePackets.MsgData);
            c.Send(stream);
            stream.InitWriter();
            stream.ProtoBufferSerialize(info);
            stream.Finalize(PacketInfo);
            c.Send(stream);
        }

        static void SendAction(Client.GameClient c, ServerSockets.Packet stream, uint action, uint cardId, params long[] data)
        {
            var a = new CardAction { Action = action };
            a.Data.Add(cardId);
            a.Data.AddRange(data);
            stream.InitWriter();
            stream.ProtoBufferSerialize(a);
            stream.Finalize(PacketAction);
            c.Send(stream);
        }

        // ------------------------------------------------------------------ config
        static Dictionary<uint, Card> Load() { return Settings.Value.Cards; }

        // ------------------------------------------------------------------ misc
        static void Log(Client.GameClient c, string text) { LogUid(c.Player.UID, c.Player.Name + " " + text); }
        static void LogUid(uint uid, string text)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " (" + uid + ") " + text;
                MyConsole.WriteLine("[WeekCard] " + line);
                File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_weekcard_log.txt"), line + Environment.NewLine);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }
    }
}
