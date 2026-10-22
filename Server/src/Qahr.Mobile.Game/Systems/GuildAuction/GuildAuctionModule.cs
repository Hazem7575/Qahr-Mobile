// QahrMobile: Guild / World auction ("vendue") of the Conquer Online Mobile client (FairyGUI ui_bangpaijingpai:
// CFDlgFactionAuction / CFactionAuctionItem / CFComAuctionBuy / CFDlgAuctionRecord, manager CAuctionMgr).
// The PC 7957 server has no such system; this is a server-side implementation built from the client (see
// conquer-mobile\docs\GUILD_AUCTION.md).
//
// Packets (both protobuf, no layout needed):
//   2084 MsgItemVendue    (mobile 10417 CMsgItemVendue, Data\mobile_ids.txt)
//   10418 MsgItemVendueList (mobile 10418, no PC id; the server uses the same number, free in GamePackets.cs)
// Dispatch: the [PacketAttribute] registry is built once at startup, so the handlers are reached through
// TryHandle(), called from Program.Game_Receive before the registry lookup (hook marked "QahrMobile: guild auction").
//
// Flow: a session puts N lots (rows of cq_vendue_item) up for one guild (or the world). Lots show for Wait seconds,
// then accept bids for Duration seconds. A bid takes the money at once (CPs or bound CPs = the row's money type)
// and refunds the previous top bidder. A bid >= the fixed price wins at once. At the end the winner gets the item and
// the price is shared among the guild members of the session (SharePercent). Everything owed to a player is a
// "pending" entry, delivered when he is online (CPs / bound CPs credited, item put in the bag).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Qahr.Mobile.GuildAuction
{
    public static class GuildAuctionModule
    {
        public const ushort IdVendue = 2084, IdVendueList = 10418, IdData = 2227;
        public const uint MoneyCps = 1, MoneyBoundCps = 2;

        static int wired;
        static Timer timer;
        static readonly Random rnd = new Random();
        static readonly Dictionary<uint, int> lastBid = new Dictionary<uint, int>();

        // ================================================================== entry points (hooks)
        /// <summary>Called from Program.Game_Receive for every client packet (server id). True = handled here.</summary>
        public static bool TryHandle(Client.GameClient client, ServerSockets.Packet stream, ushort packetId)
        {
            if (packetId != IdVendue && packetId != IdVendueList) return false;
            try
            {
                if (client == null || client.Player == null || client.Player.UID == 0) return true;
                Wire();
                if (packetId == IdVendue) OnVendue(client, stream);
                else OnVendueList(client, stream);
            }
            catch (Exception e) { GuildAuctionStore.Log("packet " + packetId + " error: " + e); }
            return true;
        }

        /// <summary>GM chat command "@gauction ..." (called from MsgMessage.ChatCommands). True = handled here.</summary>
        public static bool ChatCommand(Client.GameClient client, string text)
        {
            if (text == null || !text.StartsWith("@gauction", StringComparison.OrdinalIgnoreCase)) return false;
            try
            {
                if (client == null || client.Player == null || !(client.IsStaffAccount || client.Player.Name.Contains("[GM]"))) return false;
                Wire();
                Tell(client, Command(client, text.Substring(9).Trim()));
            }
            catch (Exception e) { Tell(client, "gauction error: " + e.Message); }
            return true;
        }

        static void Wire()
        {
            if (Interlocked.Exchange(ref wired, 1) == 1) return;
            GuildAuctionStore.Load();
            timer = new Timer(_ => Tick(), null, 3000, GuildAuctionStore.Config.TickSeconds * 1000);
        }

        // ================================================================== 2084 CMsgItemVendue
        static void OnVendue(Client.GameClient c, ServerSockets.Packet stream)
        {
            ItemVenduePB req;
            try { req = stream.ProtoBufferDeserialize(new ItemVenduePB()); } catch { return; }
            if (req == null) return;
            if (req.Action == 0) SendList(c, req.JoinType);
            else if (req.Action == 1 && req.Info != null && req.Info.Count > 0 && c.FullLoading) Bid(c, req.JoinType, req.Info[0]);
        }

        static uint GuildOf(Client.GameClient c)
        {
            try { return c.Player.MyGuild != null && c.Player.MyGuild.Info != null ? c.Player.MyGuild.Info.GuildID : 0; } catch { return 0; }
        }

        static bool Visible(Lot l, uint joinType, uint guildId)
        {
            if (l.JoinType != joinType) return false;
            return joinType == 0 || (guildId != 0 && l.GuildId == guildId);
        }

        static VendueItemInfoPB Row(Lot l, uint viewer)
        {
            return new VendueItemInfoPB
            {
                AutoId = l.Id, VendueType = l.Activity, IdVendue = l.IdVendue, Flag = l.Bidders.Contains(viewer) ? 1u : 0u,
                IdUser = l.TopUid, Price = l.Price, BeginStamp = (ulong)l.Begin, EndStamp = (ulong)l.End, LastName = l.TopName ?? ""
            };
        }

        public static void SendList(Client.GameClient c, uint joinType)
        {
            if (joinType > 1) return;
            SendTime(c);
            var msg = new ItemVenduePB { Action = 0, JoinType = joinType };
            uint g = GuildOf(c);
            lock (GuildAuctionStore.Sync)
                foreach (var l in GuildAuctionStore.State.Lots)
                    if (Visible(l, joinType, g)) msg.Info.Add(Row(l, c.Player.UID));
            Send(c, msg, IdVendue);
            DeliverPending(c);
        }

        /// <summary>CMsgData action 0 with U64TimeStamp: gives the client the unix clock its countdowns compare with.</summary>
        public static void SendTime(Client.GameClient c)
        {
            var now = DateTime.Now;
            Send(c, new TimeSyncPB
            {
                Type = 0, Year = (uint)(now.Year - 1900), Month = (uint)(now.Month - 1), DayOfYear = (uint)now.DayOfYear, DayOfMonth = (uint)now.Day,
                Hour = (uint)now.Hour, Minute = (uint)now.Minute, Second = (uint)now.Second, TimeStamp = (ulong)GuildAuctionStore.Now
            }, IdData);
        }

        static void Bid(Client.GameClient c, uint joinType, VendueItemInfoPB r)
        {
            uint uid = c.Player.UID;
            int tick = Environment.TickCount;
            lock (lastBid)
            {
                int last;
                if (lastBid.TryGetValue(uid, out last) && unchecked(tick - last) < GuildAuctionStore.Config.BidIntervalMs) return;
                lastBid[uid] = tick;
            }
            uint g = GuildOf(c);
            Lot lot; VendueItemRow row; string fail = null; uint prevUid = 0, prevPrice = 0; bool won = false;
            lock (GuildAuctionStore.Sync)
            {
                lot = GuildAuctionStore.State.Lots.FirstOrDefault(x => x.Id == r.AutoId);
                GuildAuctionStore.Items.TryGetValue(lot != null ? lot.IdVendue : 0, out row);
                long now = GuildAuctionStore.Now;
                uint basePrice = lot == null || row == null ? 0 : (lot.TopUid != 0 ? lot.Price : row.StartPrice);
                uint min = row == null ? 0 : basePrice + row.Increment;
                if (row != null && row.FixedPrice > 0 && min >= row.FixedPrice) min = row.FixedPrice;
                uint price = r.Price;
                if (row != null && row.FixedPrice > 0 && price > row.FixedPrice) price = row.FixedPrice;

                if (!GuildAuctionStore.Config.Enabled) fail = "The auction is closed.";
                else if (lot == null || row == null || !Visible(lot, joinType, g)) fail = "This lot is no longer available.";
                else if (lot.State != LotState.Active || now >= lot.End) fail = "This auction has ended.";
                else if (now < lot.Begin) fail = "Bidding has not started yet.";
                else if (r.IdVendue != lot.IdVendue || price < min) fail = "";// -> "The current bid has changed. Please bid again."
                else if (row.MoneyType != MoneyCps && row.MoneyType != MoneyBoundCps) fail = "This currency is not supported.";
                else if (!TrySpend(c, row.MoneyType, price, lot.Id)) fail = "Please make sure you have enough money first.";
                if (fail == null)
                {
                    prevUid = lot.TopUid; prevPrice = lot.Price;
                    if (prevUid != 0 && prevPrice > 0)
                        AddPending(prevUid, Kind(row.MoneyType), prevPrice, 0, 0, false,
                            prevUid == uid ? "Your previous bid of " + prevPrice + " " + MoneyName(row.MoneyType) + " was returned."
                                : "You were outbid on " + ItemName(row.ItemType) + ". " + prevPrice + " " + MoneyName(row.MoneyType) + " returned.");
                    lot.TopUid = uid; lot.TopName = c.Player.Name; lot.Price = price;
                    if (!lot.Bidders.Contains(uid)) lot.Bidders.Add(uid);
                    if (row.FixedPrice > 0 && price >= row.FixedPrice) { lot.End = now; won = true; }
                    GuildAuctionStore.Log("BID lot " + lot.Id + " row " + lot.IdVendue + " " + c.Player.Name + "(" + uid + ") " + price
                        + " money " + row.MoneyType + (prevUid != 0 ? " prev " + prevUid + ":" + prevPrice : "") + (won ? " FIXED" : ""));
                    GuildAuctionStore.Save();
                }
            }
            if (fail != null)
            {
                if (fail.Length > 0) Tell(c, fail);
                else Tell(c, "The current bid has changed. Please bid again.");
                if (lot != null && Visible(lot, joinType, g))
                {
                    var m = new ItemVenduePB { Action = 1, JoinType = lot.JoinType, Ret = 0 };
                    m.Info.Add(Row(lot, uid));
                    Send(c, m, IdVendue);
                }
                return;
            }
            var ok = new ItemVenduePB { Action = 1, JoinType = lot.JoinType, Ret = 1 };
            ok.Info.Add(Row(lot, uid));
            Send(c, ok, IdVendue);
            Broadcast(lot, uid);
            if (prevUid != 0) DeliverTo(prevUid);
            if (won) Settle();
        }

        static void Broadcast(Lot lot, uint except)
        {
            if (Pool.GamePoll == null) return;
            foreach (var o in Pool.GamePoll.Values)
            {
                try
                {
                    if (o == null || o.Player == null || !o.FullLoading || o.Player.UID == except) continue;
                    if (!Visible(lot, lot.JoinType, lot.JoinType == 0 ? 0 : GuildOf(o))) continue;
                    var m = new ItemVenduePB { Action = 1, JoinType = lot.JoinType, Ret = 0 };
                    m.Info.Add(Row(lot, o.Player.UID));
                    Send(o, m, IdVendue);
                }
                catch { }
            }
        }

        /// <summary>Action 2: the client re-requests the list of that type (new session / lots removed).</summary>
        static void NotifyChanged(uint joinType, uint guildId)
        {
            if (Pool.GamePoll == null) return;
            foreach (var o in Pool.GamePoll.Values)
            {
                try
                {
                    if (o == null || o.Player == null || !o.FullLoading) continue;
                    if (joinType == 1 && GuildOf(o) != guildId) continue;
                    Send(o, new ItemVenduePB { Action = 2, JoinType = joinType }, IdVendue);
                }
                catch { }
            }
        }

        // ================================================================== 10418 CMsgItemVendueList (records)
        static void OnVendueList(Client.GameClient c, ServerSockets.Packet stream)
        {
            VendueRecordListPB req;
            try { req = stream.ProtoBufferDeserialize(new VendueRecordListPB()); } catch { return; }
            if (req == null || req.JoinType > 1) return;
            uint per = req.PerPage == 0 || req.PerPage > 50 ? 10 : req.PerPage;
            uint page = req.PageNo == 0 ? 1 : req.PageNo;
            uint g = GuildOf(c);
            var reply = new VendueRecordListPB { JoinType = req.JoinType, PageNo = page, PerPage = per };
            lock (GuildAuctionStore.Sync)
            {
                var rows = GuildAuctionStore.State.Records
                    .Where(x => x.JoinType == req.JoinType && (req.JoinType == 0 || (g != 0 && x.GuildId == g)))
                    .OrderByDescending(x => x.Unix).ToList();
                reply.TotalAmount = (uint)rows.Count;
                foreach (var x in rows.Skip((int)((page - 1) * per)).Take((int)per))
                    reply.Info.Add(new VendueRecordPB { IdVendue = x.IdVendue, IdUser = x.Uid, MoneyType = x.MoneyType, MoneyValue = x.Money, Time = x.Time, LastName = x.Name ?? "" });
            }
            Send(c, reply, IdVendueList);
        }

        // ================================================================== sessions
        /// <summary>Puts a session of lots up. joinType 1 = guild (guildId), 0 = world. typeRow 0 = automatic.</summary>
        public static string StartSession(uint joinType, uint guildId, uint typeRow, int waitSec, int durSec)
        {
            var cfg = GuildAuctionStore.Config;
            Role.Instance.Guild guild = null;
            if (joinType == 1 && (!Role.Instance.Guild.GuildPoll.TryGetValue(guildId, out guild) || guild == null)) return "guild " + guildId + " not found";
            VendueTypeRow type;
            List<Lot> made = new List<Lot>();
            lock (GuildAuctionStore.Sync)
            {
                type = typeRow != 0 && GuildAuctionStore.Types.ContainsKey(typeRow) ? GuildAuctionStore.Types[typeRow] : PickType(joinType, guild);
                if (type == null) return "no cq_vendue_type row";
                var pool = GuildAuctionStore.Items.Values.Where(x => x.Group == type.Group && ItemKnown(x.ItemType)).ToList();// mobile-only itemtypes are skipped
                if (pool.Count == 0) return "item group " + type.Group + " is empty";
                long now = GuildAuctionStore.Now;
                long begin = now + (waitSec >= 0 ? waitSec : cfg.WaitSeconds >= 0 ? cfg.WaitSeconds : (int)type.Wait);
                long end = begin + Math.Max(30, durSec > 0 ? durSec : cfg.DurationSeconds > 0 ? cfg.DurationSeconds : (int)type.Duration);
                var share = joinType == 1 ? guild.Members.Keys.ToList() : new List<uint>();
                var used = new Dictionary<uint, uint>();
                for (int i = 0; i < Math.Max(1, (int)type.Count); i++)
                {
                    var left = pool.Where(x => !used.ContainsKey(x.Id) || used[x.Id] < Math.Max(1, x.MaxLots)).ToList();
                    if (left.Count == 0) break;
                    long total = left.Sum(x => (long)Math.Max(1, x.Weight));
                    long pick = (long)(rnd.NextDouble() * total);
                    var item = left[left.Count - 1];
                    foreach (var x in left) { pick -= Math.Max(1, x.Weight); if (pick < 0) { item = x; break; } }
                    uint u; used.TryGetValue(item.Id, out u); used[item.Id] = u + 1;
                    var lot = new Lot
                    {
                        Id = GuildAuctionStore.State.NextId++, JoinType = joinType, GuildId = joinType == 1 ? guildId : 0, Activity = type.Activity,
                        IdVendue = item.Id, Begin = begin, End = end, State = LotState.Active, Share = share
                    };
                    GuildAuctionStore.State.Lots.Add(lot);
                    made.Add(lot);
                }
                GuildAuctionStore.Log("SESSION " + (joinType == 1 ? "guild " + guildId : "world") + " type " + type.Id + " group " + type.Group
                    + " lots " + string.Join(",", made.Select(x => x.Id + ":" + x.IdVendue)) + " begin " + begin + " end " + end);
                GuildAuctionStore.Save();
            }
            NotifyChanged(joinType, guildId);
            return made.Count + " lots for " + (joinType == 1 ? "guild " + guild.GuildName : "the world") + " (type " + type.Id + ", group " + type.Group
                + "), bidding opens in " + (made[0].Begin - GuildAuctionStore.Now) + " s for " + (made[0].End - made[0].Begin) + " s";
        }

        /// <summary>Guild: the row of the guild's level (c5, 1..9) for today's weekday (c6 1..7 = Mon..Sun). World: a random c2=3 row.</summary>
        static VendueTypeRow PickType(uint joinType, Role.Instance.Guild guild)
        {
            var types = GuildAuctionStore.Types.Values;
            if (joinType == 0)
            {
                var w = types.Where(x => x.Activity == 3).ToList();
                return w.Count == 0 ? null : w[rnd.Next(w.Count)];
            }
            uint level = 1;
            try { uint lv = guild.Info != null ? guild.Info.Level : 1; level = lv < 1 ? 1 : lv > 9 ? 9 : lv; } catch { }
            uint day = (uint)(((int)DateTime.Now.DayOfWeek + 6) % 7 + 1);
            return types.FirstOrDefault(x => x.Activity == 1 && x.Level == level && x.Group == day)
                ?? types.FirstOrDefault(x => x.Activity == 1 && x.Level == level)
                ?? types.FirstOrDefault(x => x.Activity == 1);
        }

        // ================================================================== timer: schedule, settle, deliver
        static void Tick()
        {
            try { Schedule(); } catch (Exception e) { GuildAuctionStore.Log("schedule error: " + e); }
            try { Settle(); } catch (Exception e) { GuildAuctionStore.Log("settle error: " + e); }
            try
            {
                if (Pool.GamePoll != null)
                    foreach (var o in Pool.GamePoll.Values)
                        if (o != null && o.Player != null && o.FullLoading) DeliverPending(o);
            }
            catch (Exception e) { GuildAuctionStore.Log("deliver error: " + e); }
        }

        static void Schedule()
        {
            var cfg = GuildAuctionStore.Config;
            if (!cfg.Enabled) return;
            var now = DateTime.Now;
            foreach (var pair in new[] { new { J = 1u, S = cfg.GuildSchedule }, new { J = 0u, S = cfg.WorldSchedule } })
            {
                foreach (var entry in (pair.S ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var p = entry.Trim().Split(' ');
                    DayOfWeek day; TimeSpan at;
                    if (p.Length != 2 || !TryDay(p[0], out day) || !TimeSpan.TryParse(p[1], CultureInfo.InvariantCulture, out at)) continue;
                    if (now.DayOfWeek != day || now.TimeOfDay < at || now.TimeOfDay > at.Add(TimeSpan.FromMinutes(10))) continue;
                    string key = pair.J + " " + now.ToString("yyyy-MM-dd") + " " + p[1];
                    lock (GuildAuctionStore.Sync)
                    {
                        if (GuildAuctionStore.State.ScheduleDone.Contains(key)) continue;
                        GuildAuctionStore.State.ScheduleDone.Add(key);
                        while (GuildAuctionStore.State.ScheduleDone.Count > 50) GuildAuctionStore.State.ScheduleDone.RemoveAt(0);
                        GuildAuctionStore.Save();
                    }
                    if (pair.J == 0) GuildAuctionStore.Log("AUTO " + StartSession(0, 0, 0, -1, -1));
                    else
                        foreach (var g in Role.Instance.Guild.GuildPoll.Values.ToList())
                            if (g != null && g.Info != null && g.Members != null && g.Members.Count >= cfg.GuildMinMembers)
                                GuildAuctionStore.Log("AUTO " + StartSession(1, g.Info != null ? g.Info.GuildID : 0, 0, -1, -1));
                }
            }
        }

        static bool TryDay(string s, out DayOfWeek day)
        {
            foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                if (d.ToString().StartsWith(s, StringComparison.OrdinalIgnoreCase) && s.Length >= 2) { day = d; return true; }
            day = DayOfWeek.Sunday; return false;
        }

        /// <summary>Ends the lots whose time is over: item to the winner, price shared to the session's guild members.</summary>
        static void Settle()
        {
            var changed = new HashSet<string>();
            var notify = new List<uint>();
            lock (GuildAuctionStore.Sync)
            {
                var st = GuildAuctionStore.State;
                long now = GuildAuctionStore.Now;
                bool dirty = false;
                foreach (var l in st.Lots.Where(x => x.State == LotState.Active && x.End <= now).ToList())
                {
                    VendueItemRow row;
                    GuildAuctionStore.Items.TryGetValue(l.IdVendue, out row);
                    l.Settled = now; dirty = true;
                    if (l.TopUid == 0 || row == null) { l.State = LotState.Unsold; GuildAuctionStore.Log("END lot " + l.Id + " unsold"); continue; }
                    l.State = LotState.Sold;
                    AddPending(l.TopUid, "item", 0, row.ItemType, Math.Max(1, row.Amount), row.Monopoly != 0,
                        "You won " + ItemName(row.ItemType) + " in the " + (l.JoinType == 1 ? "Guild" : "World") + " Auction for " + l.Price + " " + MoneyName(row.MoneyType) + ".");
                    notify.Add(l.TopUid);
                    st.Records.Add(new Record
                    {
                        Unix = now, Time = GuildAuctionStore.RecordTime(DateTime.Now), JoinType = l.JoinType, GuildId = l.GuildId, IdVendue = l.IdVendue,
                        Uid = l.TopUid, Name = l.TopName, MoneyType = row.MoneyType, Money = l.Price
                    });
                    uint pot = (uint)((ulong)l.Price * (ulong)GuildAuctionStore.Config.SharePercent / 100UL);
                    var share = l.Share ?? new List<uint>();
                    uint each = share.Count == 0 ? 0 : pot / (uint)share.Count;
                    if (each > 0)
                        foreach (var m in share)
                        {
                            AddPending(m, Kind(row.MoneyType), each, 0, 0, false,
                                "Guild Auction earnings: " + ItemName(row.ItemType) + " sold for " + l.Price + ". Your share: " + each + " " + MoneyName(row.MoneyType) + ".");
                            notify.Add(m);
                        }
                    GuildAuctionStore.Log("END lot " + l.Id + " row " + l.IdVendue + " sold to " + l.TopName + "(" + l.TopUid + ") for " + l.Price
                        + " money " + row.MoneyType + ", share " + each + " x " + share.Count);
                }
                long keep = now - (long)GuildAuctionStore.Config.KeepEndedHours * 3600;
                foreach (var l in st.Lots.Where(x => x.State != LotState.Active && x.Settled < keep).ToList())
                {
                    st.Lots.Remove(l); dirty = true;
                    changed.Add(l.JoinType + ":" + l.GuildId);
                }
                if (st.Records.Count > GuildAuctionStore.Config.KeepRecords)
                {
                    st.Records = st.Records.OrderByDescending(x => x.Unix).Take(GuildAuctionStore.Config.KeepRecords).ToList();
                    dirty = true;
                }
                if (dirty) GuildAuctionStore.Save();
            }
            foreach (var k in changed)
            {
                var p = k.Split(':');
                NotifyChanged(uint.Parse(p[0]), uint.Parse(p[1]));
            }
            foreach (var u in notify.Distinct()) DeliverTo(u);
        }

        // ================================================================== money / pending
        static string Kind(uint moneyType) { return moneyType == MoneyBoundCps ? "bcps" : "cps"; }
        static string MoneyName(uint moneyType) { return moneyType == MoneyBoundCps ? "bound CPs" : moneyType == 1002 ? "points" : "CPs"; }

        static string ItemName(uint id)
        {
            try
            {
                Database.ItemType.DBItem d;
                if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out d) && d != null) return d.Name.Replace('~', ' ');
            }
            catch { }
            return "item " + id;
        }

        static bool TrySpend(Client.GameClient c, uint moneyType, uint amount, ulong lotId)
        {
            if (moneyType == MoneyCps)
            {
                uint cur = c.Player.ConquerPoints;
                if (amount > cur) return false;
                c.Player.ConquerPoints = cur - amount;
                GuildAuctionStore.Log(c.Player.Name + "(" + c.Player.UID + ") CPs -" + amount + " bid lot " + lotId + " " + cur + " -> " + c.Player.ConquerPoints);
                return true;
            }
            if (moneyType == MoneyBoundCps)
            {
                int cur = c.Player.BoundConquerPoints;
                if (cur < 0 || amount > (uint)cur) return false;
                c.Player.BoundConquerPoints = cur - (int)amount;
                GuildAuctionStore.Log(c.Player.Name + "(" + c.Player.UID + ") bound CPs -" + amount + " bid lot " + lotId + " " + cur + " -> " + c.Player.BoundConquerPoints);
                return true;
            }
            return false;
        }

        /// <summary>Call inside lock(Sync).</summary>
        static void AddPending(uint uid, string kind, uint value, uint itemType, uint amount, bool bound, string text)
        {
            var st = GuildAuctionStore.State;
            var p = new Pending { Id = st.NextId++, Uid = uid, Kind = kind, Value = value, ItemType = itemType, Amount = amount, Bound = bound, Text = text };
            st.Pending.Add(p);
            GuildAuctionStore.Log("PENDING " + p.Id + " uid " + uid + " " + kind + " " + value + " " + itemType + "x" + amount);
        }

        static void DeliverTo(uint uid)
        {
            Client.GameClient c;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading) DeliverPending(c);
        }

        /// <summary>Each entry is removed and saved BEFORE it is given, so a second call or a relog finds nothing.</summary>
        static void DeliverPending(Client.GameClient c)
        {
            if (c == null || c.Player == null || !c.FullLoading || c.OnInterServer) return;
            uint uid = c.Player.UID;
            while (true)
            {
                Pending p;
                lock (GuildAuctionStore.Sync)
                {
                    p = GuildAuctionStore.State.Pending.FirstOrDefault(x => x.Uid == uid && (x.Kind != "item" || (HasRoom(c) && ItemKnown(x.ItemType))));
                    if (p == null) return;
                    GuildAuctionStore.State.Pending.Remove(p);
                    GuildAuctionStore.Save();
                }
                try
                {
                    if (p.Kind == "cps")
                    {
                        uint cur = c.Player.ConquerPoints;
                        ulong next = (ulong)cur + p.Value;
                        c.Player.ConquerPoints = next > uint.MaxValue ? uint.MaxValue : (uint)next;
                    }
                    else if (p.Kind == "bcps")
                    {
                        long next = (long)c.Player.BoundConquerPoints + p.Value;
                        c.Player.BoundConquerPoints = next > int.MaxValue ? int.MaxValue : (int)next;
                    }
                    else if (p.Kind == "item")
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (!c.Inventory.Add(stream, p.ItemType, (byte)Math.Max(1, Math.Min(255, p.Amount)), 0, 0, 0,
                                Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, p.Bound))
                                throw new Exception("Inventory.Add refused " + p.ItemType);
                        }
                    }
                    GuildAuctionStore.Log("DELIVER " + p.Id + " to " + c.Player.Name + "(" + uid + ") " + p.Kind + " " + p.Value + " " + p.ItemType + "x" + p.Amount);
                    if (!string.IsNullOrEmpty(p.Text)) Tell(c, p.Text);
                }
                catch (Exception e)
                {
                    GuildAuctionStore.Log("DELIVER " + p.Id + " failed, kept: " + e.Message);
                    lock (GuildAuctionStore.Sync) { GuildAuctionStore.State.Pending.Add(p); GuildAuctionStore.Save(); }
                    return;
                }
            }
        }

        /// <summary>The itemtype exists on the server (many cq_vendue_item rows are mobile-only items).</summary>
        static bool ItemKnown(uint id)
        {
            try { return Pool.ItemsBase != null && Pool.ItemsBase.ContainsKey(id); } catch { return false; }
        }

        static bool HasRoom(Client.GameClient c)
        {
            try { return c.Inventory != null && c.Inventory.HaveSpace(1); } catch { return false; }
        }

        // ================================================================== GM command
        static string Command(Client.GameClient c, string args)
        {
            var a = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string op = a.Length > 0 ? a[0].ToLowerInvariant() : "help";
            Func<int, int, int> num = (i, d) => { int v; return a.Length > i && int.TryParse(a[i], out v) ? v : d; };
            switch (op)
            {
                case "start":   // @gauction start [typeRow|0] [waitSec] [durSec] [guildId]
                    {
                        uint g = (uint)num(4, 0);
                        if (g == 0) g = GuildOf(c);
                        if (g == 0) return "you are not in a guild (or give a guild id as 4th argument)";
                        return StartSession(1, g, (uint)num(1, 0), num(2, -1), num(3, -1));
                    }
                case "world":   // @gauction world [typeRow|0] [waitSec] [durSec]
                    return StartSession(0, 0, (uint)num(1, 0), num(2, -1), num(3, -1));
                case "end":     // @gauction end <lotId|all>: bidding time over now
                    {
                        int n = 0;
                        lock (GuildAuctionStore.Sync)
                        {
                            long now = GuildAuctionStore.Now;
                            foreach (var l in GuildAuctionStore.State.Lots.Where(x => x.State == LotState.Active))
                                if (a.Length > 1 && (a[1] == "all" || a[1] == l.Id.ToString())) { l.End = now; if (l.Begin > now) l.Begin = now; n++; }
                            GuildAuctionStore.Save();
                        }
                        Settle();
                        return n + " lot(s) ended";
                    }
                case "open":    // @gauction open <lotId|all>: bidding opens now
                    {
                        int n = 0; var groups = new HashSet<string>();
                        lock (GuildAuctionStore.Sync)
                        {
                            long now = GuildAuctionStore.Now;
                            foreach (var l in GuildAuctionStore.State.Lots.Where(x => x.State == LotState.Active && x.Begin > now))
                                if (a.Length > 1 && (a[1] == "all" || a[1] == l.Id.ToString())) { l.End = now + (l.End - l.Begin); l.Begin = now; n++; groups.Add(l.JoinType + ":" + l.GuildId); }
                            GuildAuctionStore.Save();
                        }
                        foreach (var k in groups) { var p = k.Split(':'); NotifyChanged(uint.Parse(p[0]), uint.Parse(p[1])); }
                        return n + " lot(s) open";
                    }
                case "clear":   // @gauction clear: removes every finished lot now
                    {
                        lock (GuildAuctionStore.Sync)
                        {
                            GuildAuctionStore.State.Lots.RemoveAll(x => x.State != LotState.Active);
                            GuildAuctionStore.Save();
                        }
                        return "finished lots removed (players see them until relog)";
                    }
                case "list":
                    lock (GuildAuctionStore.Sync)
                    {
                        long now = GuildAuctionStore.Now;
                        var s = GuildAuctionStore.State;
                        return s.Lots.Count + " lots, " + s.Pending.Count + " pending, " + s.Records.Count + " records. "
                            + string.Join("; ", s.Lots.Take(12).Select(l => "#" + l.Id + " row" + l.IdVendue + " " + l.State
                                + (l.JoinType == 1 ? " g" + l.GuildId : " world") + " " + l.Price + (l.TopUid != 0 ? " by " + l.TopName : "")
                                + (l.State == LotState.Active ? (now < l.Begin ? " opens " + (l.Begin - now) + "s" : " ends " + (l.End - now) + "s") : "")));
                    }
                case "reload":
                    GuildAuctionStore.Load();
                    return "reloaded: " + GuildAuctionStore.Items.Count + " items, " + GuildAuctionStore.Types.Count + " types";
                case "show":    // @gauction show [0|1]: resend the list to me
                    SendList(c, (uint)num(1, 1));
                    return "list sent";
                default:
                    return "@gauction start [typeRow] [waitSec] [durSec] [guildId] | world [typeRow] [waitSec] [durSec] | open <id|all> | end <id|all> | list | clear | show [0|1] | reload";
            }
        }

        // ================================================================== send helpers
        static void Tell(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void Send(Client.GameClient c, object proto, ushort id)
        {
            if (c == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                s.InitWriter();
                s.ProtoBufferSerialize(proto);
                s.Finalize(id);
                c.Send(s);
            }
        }
    }
}
