// [Auction 2026-10-14] Auction House (CDlgAuction #572, CDlgAuctionView #573, CDlgAuctionSell #574, CDlgAuctionSellConfirm #988).
//
// What the 7957 client really has (Ghidra, Conquer.exe md5 D1803D09, auctionmgr.cpp / dlgauction*.cpp):
//   * The only categories are the client's ClassDesc.ini / class_auction.ini: class 21 "Mythsoul" with sub classes
//     2101..2106 (L1..L6). An item's class is itemtype column 58 (1020 Mythsoul itemtypes 407xxxxL). Auction.lua marks
//     these classes "cross-server" (Auction_IsNosuchItem), so every sell / buy / cancel goes through CMsgNosuch 2186.
//     Prices are CPs only; a listing is fixed price ("Buy"), 48 hours, max 10 per player. There is NO bidding in this
//     auction house (the bid strings belong to the separate instance/world auctions, CMsgItemVendue 2084).
//   * 2186 CMsgNosuch C->S (f1 action, f2 u32 data):
//       0  sell     [item uid, unit price, count]                (FUN_00fc64be, from the sell-confirm dialog)
//       1  cancel   [listing id]                                 (FUN_00fc5931, msgbox "cancel the auction?")
//       3  buy      [listing id, itemtype, count, unit price]    (FUN_00fc605f, msgbox buy confirm)
//       0x13 query my listings (no data)                         (FUN_011c467f)
//       4/6/7/9/10/0xb/0xc/0xd/0x12 = buy requests / auto-trade / min price: they need the "cross server ready" flag
//       (2186 S->C 0x10) which this server never sends, so the client itself answers "The server is not ready".
//     2186 S->C (process @0xfe77ba): 0 = sell result / my-list row (18 u32: id, price, auto, itemtype, ok, seconds,
//       amount, bless, dur, maxdur, attr1..5, exp, mutation, upsurge); 1 = cancel result [id, ok]; 2 = remove [id];
//       3 = buy result [id, itemtype, count, price, ok] (ok 0 -> STR_AUCTION_VIEW_NO_ITEM).
//   * 2399 CMsgAuctionItem browse (f1 cross flag, f3 page 0 based, f4 class, f6 name) -> f2 total, f3 page, f7 rows.
//   * 2012 CMsgAuctionDirList (f1 cross flag, f2 class) -> per sub class rows (derived meaning, see AuctionEngine.Directory).
//   * 2415 CMsgMyNosuchQuery: my listings. 2564 CMsgAuctionTimeOutList + MsgItemInfo (2350) mode 15: expired list.
//   * 2028 CMsgAuction (struct 0x18: +4 u16 action, +8 id, +0xc, +0x10 ok, +0x14):
//       C->S 7 = expired page (+0x14 page from 1, +0xc page size 7), 8 = take expired item to bag (+8 id),
//            0/1/2 = sell/cancel/buy of "local" categories (none exist in the client data -> refused).
//       S->C 5 = "You have expired listings" tip, 8 = item back in bag (+8 id) -> STR_AUCTION_GET_TO_BAG_SUCESS.
//   * 2441 CMsgAuctionQuery = "local" my-trades list -> always empty (no local categories).
//   * 2084 CMsgItemVendue (instance / world / family auctions) and 2571 CMsgCopTrade (card platform trade) are other
//     systems and are NOT handled here.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Qahr.Mobile.AuctionHouse
{
    public static class AuctionModule
    {
        public const ushort IdAuction = 2028, IdNosuch = 2186, IdAuctionItem = 2399, IdAuctionQuery = 2441, IdDirList = 2012,
            IdMyNosuch = 2415, IdTimeOut = 2564;

        // test seam
        public static Action<Client.GameClient, ServerSockets.Packet> SendHook = (c, p) => c.Send(p);

        static AhConfig Cfg { get { AuctionEngine.EnsureLoaded(); return AuctionStore.Config; } }

        // ------------------------------------------------------------------ wiring
        static int wired;
        static Timer timer;

        static void Wire()
        {
            if (Interlocked.Exchange(ref wired, 1) == 1) return;
            AuctionEngine.Online = uid =>
            {
                Client.GameClient c;
                if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c) && Usable(c)) return new ClientPlayer(c);
                return null;
            };
            AuctionEngine.FileHasItem = FileHasItem;
            AuctionEngine.ItemName = id =>
            {
                Database.ItemType.DBItem d;
                return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out d) && d != null ? d.Name : null;
            };
            AuctionEngine.EnsureLoaded();
            int sec = AuctionStore.Config.ExpiryCheckSeconds;
            timer = new Timer(_ => Tick(), null, 5000, sec * 1000);
        }

        static bool Usable(Client.GameClient c)
        {
            return c != null && c.Player != null && c.Player.UID != 0 && c.FullLoading && !c.OnInterServer;
        }

        static bool Ready(Client.GameClient c)
        {
            if (!Usable(c)) return false;
            Wire();
            return true;
        }

        // ------------------------------------------------------------------ throttle
        sealed class Slots { public readonly int[] Last = new int[3]; public readonly bool[] Used = new bool[3]; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> slots = new ConditionalWeakTable<Client.GameClient, Slots>();
        static bool Allow(Client.GameClient c, int slot, int ms)
        {
            var s = slots.GetOrCreateValue(c);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < ms) return false;
                s.Used[slot] = true; s.Last[slot] = now; return true;
            }
        }

        // ------------------------------------------------------------------ send helpers
        static void Tell(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void SendProto(Client.GameClient c, object proto, ushort id)
        {
            if (c == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                SendHook(c, BuildProto(s, proto, id));
            }
        }

        public static ServerSockets.Packet BuildProto(ServerSockets.Packet s, object proto, ushort id)
        {
            s.InitWriter();
            s.ProtoBufferSerialize(proto);
            s.Finalize(id);
            return s;
        }

        /// <summary>2028 struct, 0x18 bytes: +4 u16 action, +8 id, +0xc, +0x10 ok, +0x14.</summary>
        public static ServerSockets.Packet BuildAuction(ServerSockets.Packet s, ushort action, uint id, uint v0c, uint ok, uint v14)
        {
            s.InitWriter();
            s.Write(action);
            s.Write((ushort)0);
            s.Write(id);
            s.Write(v0c);
            s.Write(ok);
            s.Write(v14);
            s.Finalize(IdAuction);
            return s;
        }

        static void SendAuction(Client.GameClient c, ushort action, uint id, uint v0c, uint ok, uint v14)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                SendHook(c, BuildAuction(s, action, id, v0c, ok, v14));
            }
        }

        public static AhNosuchPB Nosuch(uint action, params uint[] data)
        {
            var m = new AhNosuchPB { f1 = action };
            m.f2.AddRange(data);
            return m;
        }

        static uint ServerId()
        {
            try
            {
                var s = Database.GroupServerList.MyServerInfo;
                if (s != null && s.ID != 0) return (uint)s.ID;
            }
            catch { }
            return 1;// the client treats 0 as a "local" row
        }

        // ------------------------------------------------------------------ row builders
        public static uint[] SellRow(AhListing l, bool ok)
        {
            if (!ok || l == null) return new uint[18];
            return new uint[]
            {
                l.Id, l.UnitPrice, 0, l.ItemId, 1, AuctionEngine.RemainingSeconds(l), l.Amount,
                l.Bless, l.Dur, l.MaxDur, l.Attr[0], l.Attr[1], l.Attr[2], l.Attr[3], l.Attr[4], l.Exp, l.Mut, l.Ups
            };
        }

        public static AhItemInfoPB BrowseRow(AhListing l, uint serverId)
        {
            return new AhItemInfoPB
            {
                f1 = 1, f3 = l.Id, f5 = serverId, f6 = l.Seller, f8 = l.UnitPrice, f9 = AuctionEngine.RemainingSeconds(l), f10 = l.ItemId,
                f11 = (int)l.Amount, f12 = l.Bless, f13 = l.Dur, f14 = l.MaxDur, f15 = l.Attr[0], f16 = l.Attr[1], f17 = l.Attr[2],
                f18 = l.Attr[3], f19 = (int)l.Attr[4], f20 = (int)l.Exp, f21 = (int)l.Mut, f22 = (int)l.Ups
            };
        }

        public static AhMyItemPB MyRow(AhListing l, uint serverId)
        {
            return new AhMyItemPB
            {
                f1 = 0, f2 = l.Id, f4 = serverId, f5 = l.Seller, f7 = l.UnitPrice, f8 = AuctionEngine.RemainingSeconds(l), f9 = l.ItemId,
                f10 = (int)l.Amount, f11 = l.Bless, f12 = l.Dur, f13 = l.MaxDur, f14 = l.Attr[0], f15 = l.Attr[1], f16 = l.Attr[2],
                f17 = l.Attr[3], f18 = (int)l.Attr[4], f19 = (int)l.Exp, f20 = (int)l.Mut, f21 = (int)l.Ups
            };
        }

        public static AhAuctionItemPB AnswerBrowse(AhAuctionItemPB req, uint serverId)
        {
            var ans = new AhAuctionItemPB { f1 = req.f1, f3 = req.f3 };
            if (req.f1 == 0) return ans;// "local" categories do not exist in the client data
            uint total;
            var rows = AuctionEngine.Browse((uint)Math.Min(req.f4, uint.MaxValue), req.f6, req.f3, out total);
            ans.f2 = total;
            foreach (var l in rows) ans.f7.Add(BrowseRow(l, serverId));
            return ans;
        }

        public static AhDirListPB AnswerDirList(AhDirListPB req)
        {
            var ans = new AhDirListPB { f1 = req.f1, f2 = req.f2 };
            if (req.f1 == 0) return ans;
            ans.f3.AddRange(AuctionEngine.Directory((uint)Math.Min(req.f2, uint.MaxValue), null));
            return ans;
        }

        public static AhMyQueryPB AnswerMine(uint uid, uint serverId)
        {
            var ans = new AhMyQueryPB();
            foreach (var l in AuctionEngine.Mine(uid)) ans.f1.Add(MyRow(l, serverId));
            return ans;
        }

        static string ItemText(AhListing l)
        {
            string n = AuctionEngine.ItemName(l.ItemId) ?? l.ItemId.ToString();
            return "[" + n + "] x" + l.Amount;
        }

        static string Reason(AhResult r)
        {
            switch (r)
            {
                case AhResult.Disabled: return "The Auction House is closed.";
                case AhResult.NotTradable: return "This item cannot be traded.";
                case AhResult.NoClass: return "This item does not belong to any Auction House category and cannot be listed.";
                case AhResult.BadPrice: return "Invalid price.";
                case AhResult.BadAmount: return "Only the whole stack can be listed or bought.";
                case AhResult.ListingLimit: return "Cannot list more than " + AuctionStore.Config.MaxListings + " items.";
                case AhResult.HasExpired: return "You have expired listings. Please delist them first.";
                case AhResult.BagFull: return "Insufficient inventory space. Please leave at least one empty slot.";
                case AhResult.PriceChanged: return "This listing has changed. Please refresh.";
                case AhResult.OwnListing: return "You cannot buy your own item.";
                case AhResult.NotFound: return "This item is no longer available.";
                case AhResult.NotOwner: return "This item is no longer available.";
                default: return "The Auction House could not complete the request.";
            }
        }

        // ================================================================== 2186 CMsgNosuch
        [PacketAttribute(IdNosuch)]
        public static void OnNosuch(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                AhNosuchPB req;
                try { req = stream.ProtoBufferDeserialize(new AhNosuchPB()); } catch { return; }
                if (req == null || req.f2 == null || req.f2.Count > 32) return;
                var d = req.f2;
                switch (req.f1)
                {
                    case 0: if (d.Count >= 3 && Allow(client, 0, Cfg.ActionIntervalMs)) DoSell(client, d[0], d[1], d[2]); break;
                    case 1: if (d.Count >= 1 && Allow(client, 0, Cfg.ActionIntervalMs)) DoCancel(client, d[0]); break;
                    case 3: if (d.Count >= 4 && Allow(client, 0, Cfg.ActionIntervalMs)) DoBuy(client, d[0], d[1], d[2], d[3]); break;
                    case 0x13: if (Allow(client, 1, Cfg.ListIntervalMs)) SendProto(client, AnswerMine(client.Player.UID, ServerId()), IdMyNosuch); break;
                    default: break;// buy requests / auto trade: the client blocks them itself ("server is not ready")
                }
            }
            catch (Exception e) { AuctionStore.Log("2186 error: " + e); }
        }

        static bool Busy(Client.GameClient c)
        {
            try { return c.InTrade || c.IsVendor; } catch { return true; }
        }

        static void DoSell(Client.GameClient c, uint itemUid, uint unitPrice, uint count)
        {
            AhListing l = null;
            AhResult r;
            if (Busy(c)) r = AhResult.Failed;
            else r = AuctionEngine.Sell(new ClientPlayer(c), itemUid, unitPrice, count, out l);
            if (r == AhResult.Ok)
            {
                SendProto(c, Nosuch(0, SellRow(l, true)), IdNosuch);
                Tell(c, ItemText(l) + " is now listed for " + l.UnitPrice + " CPs each" + (l.Fee > 0 ? " (listing fee " + l.Fee + " CPs)." : "."));
            }
            else
            {
                Game.MsgServer.MsgGameItem it;
                uint itemType = c.Inventory.TryGetItem(itemUid, out it) && it != null ? it.ITEM_ID : 0;
                SendProto(c, Nosuch(0, itemUid, unitPrice, 0, itemType, 0, 0, count, 0), IdNosuch);
                Tell(c, r == AhResult.NoMoney ? "Not enough CPs for the listing fee." : Reason(r));
            }
        }

        static void DoCancel(Client.GameClient c, uint id)
        {
            bool toBag = false;
            var r = Busy(c) ? AhResult.Failed : AuctionEngine.Cancel(new ClientPlayer(c), id, out toBag);
            SendProto(c, Nosuch(1, id, r == AhResult.Ok ? 1u : 0u), IdNosuch);
            if (r == AhResult.Ok && !toBag)
            {
                Tell(c, "Your inventory is full. The item was moved to your expired listings.");
                SendAuction(c, 5, 0, 0, 0, 0);
            }
            else if (r != AhResult.Ok && r != AhResult.NotFound && r != AhResult.NotOwner) Tell(c, Reason(r));
        }

        static void DoBuy(Client.GameClient c, uint id, uint itemType, uint count, uint unitPrice)
        {
            AhListing l = null;
            var r = Busy(c) ? AhResult.Failed : AuctionEngine.Buy(new ClientPlayer(c), id, itemType, count, unitPrice, out l);
            SendProto(c, Nosuch(3, id, itemType, count, unitPrice, r == AhResult.Ok ? 1u : 0u), IdNosuch);
            if (r != AhResult.Ok)
            {
                if (r == AhResult.NoMoney) Tell(c, "Insufficient CPs to purchase.");
                else if (r != AhResult.NotFound) Tell(c, Reason(r));
                return;
            }
            Tell(c, "You bought " + ItemText(l) + " for " + l.Total + " CPs.");
            NotifySold(l);
        }

        static void NotifySold(AhListing l)
        {
            Client.GameClient s;
            if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(l.Seller, out s) || !Usable(s)) return;
            SendProto(s, Nosuch(2, l.Id), IdNosuch);
            Tell(s, "Your " + ItemText(l) + " was sold on the Auction House.");
            PayIncome(s);
        }

        static void PayIncome(Client.GameClient s)
        {
            ulong got = AuctionEngine.CreditIncome(new ClientPlayer(s));
            if (got > 0) Tell(s, "Auction House income: " + got + " CPs.");
        }

        // ================================================================== 2028 CMsgAuction (struct)
        [PacketAttribute(IdAuction)]
        public static void OnAuction(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client) || stream.Size < 0x18) return;
                stream.Seek(4);
                ushort action = stream.ReadUInt16();
                stream.ReadUInt16();
                uint id = stream.ReadUInt32();
                uint v0c = stream.ReadUInt32();
                stream.ReadUInt32();
                uint v14 = stream.ReadUInt32();
                switch (action)
                {
                    case 7:// expired list page
                        if (Allow(client, 2, Cfg.ListIntervalMs)) SendExpiredPage(client, v14, v0c);
                        break;
                    case 8:// expired item -> bag
                        if (!Allow(client, 0, Cfg.ActionIntervalMs)) return;
                        {
                            var r = Busy(client) ? AhResult.Failed : AuctionEngine.Delist(new ClientPlayer(client), id);
                            if (r == AhResult.Ok) SendAuction(client, 8, id, 0, 1, 0);
                            else Tell(client, Reason(r));
                        }
                        break;
                    case 1:
                    case 2:// "local" cancel / buy: no local listing can exist
                        if (Allow(client, 0, Cfg.ActionIntervalMs)) SendAuction(client, action, id, 0, 0, 0);
                        break;
                    case 0:// "local" sell: the client data has no local category
                        if (Allow(client, 0, Cfg.ActionIntervalMs)) Tell(client, Reason(AhResult.NoClass));
                        break;
                }
            }
            catch (Exception e) { AuctionStore.Log("2028 error: " + e); }
        }

        public static AhTimeOutListPB ExpiredPage(uint uid, uint page, uint size, out List<AhListing> rows)
        {
            if (size == 0 || size > 7) size = 7;
            if (page == 0) page = 1;
            if (page > 100000) page = 100000;
            var all = AuctionEngine.Expired(uid);
            rows = all.Skip((int)((page - 1) * size)).Take((int)size).ToList();
            var ans = new AhTimeOutListPB { f1 = page, f2 = (uint)all.Count };
            foreach (var l in rows) ans.f3.Add(l.Id);
            return ans;
        }

        static void SendExpiredPage(Client.GameClient c, uint page, uint size)
        {
            List<AhListing> rows;
            var ans = ExpiredPage(c.Player.UID, page, size, out rows);
            SendProto(c, ans, IdTimeOut);// ids first, the client waits for their MsgItemInfo (mode 15) before refreshing
            foreach (var l in rows)
            {
                var item = ItemCodec.Decode(l.Blob);
                if (item == null) continue;
                item.Mode = (Role.Flags.ItemMode)15;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    SendHook(c, item.ItemCreate(s, item));
                }
            }
        }

        // ================================================================== 2399 / 2441 / 2012 (delegated from B7Social)
        public static void OnAuctionItem(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client) || !Allow(client, 1, Cfg.ListIntervalMs)) return;
                AhAuctionItemPB req;
                try { req = stream.ProtoBufferDeserialize(new AhAuctionItemPB()); } catch { return; }
                if (req == null) return;
                SendProto(client, AnswerBrowse(req, ServerId()), IdAuctionItem);
            }
            catch (Exception e) { AuctionStore.Log("2399 error: " + e); }
        }

        public static void OnAuctionQuery(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client) || !Allow(client, 2, Cfg.ListIntervalMs)) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    s.InitWriter();
                    s.ZeroFill(24);// 0x1c bytes, u16 count @+6 = 0: no "local" listings
                    s.Finalize(IdAuctionQuery);
                    SendHook(client, s);
                }
            }
            catch (Exception e) { AuctionStore.Log("2441 error: " + e); }
        }

        public static void OnDirList(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client) || !Allow(client, 1, Cfg.ListIntervalMs)) return;
                AhDirListPB req;
                try { req = stream.ProtoBufferDeserialize(new AhDirListPB()); } catch { return; }
                if (req == null) return;
                SendProto(client, AnswerDirList(req), IdDirList);
            }
            catch (Exception e) { AuctionStore.Log("2012 error: " + e); }
        }

        // ================================================================== login / tick
        /// <summary>Login hook: load the store, settle this player's unconfirmed moves, tell about expired listings.</summary>
        public static void OnLogin(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null || client.Player.UID == 0) return;
                Wire();
                AuctionEngine.OnLogin(client.Player.UID);
                if (AuctionEngine.HasReturned(client.Player.UID))
                    Tell(client, "You have expired Auction House listings. Delist them from the Auction House (Show Expired).");
            }
            catch (Exception e) { AuctionStore.Log("login error: " + e); }
        }

        static int ticking;
        static void Tick()
        {
            if (Interlocked.Exchange(ref ticking, 1) == 1) return;
            try
            {
                foreach (var l in AuctionEngine.ExpireDue())
                {
                    Client.GameClient s;
                    if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(l.Seller, out s) || !Usable(s)) continue;
                    SendProto(s, Nosuch(2, l.Id), IdNosuch);
                    SendAuction(s, 5, 0, 0, 0, 0);
                }
                // income of sellers that are online now (sold while offline, or not credited yet)
                HashSet<uint> sellers;
                lock (AuctionStore.Sync)
                    sellers = new HashSet<uint>(AuctionStore.Listings.Values
                        .Where(l => l.State == AhState.Sold && l.DeliveryConfirmed && l.Pay == AhPay.Pending).Select(l => l.Seller));
                foreach (var uid in sellers)
                {
                    Client.GameClient s;
                    if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out s) && Usable(s)) PayIncome(s);
                }
                RetryConfirm();
            }
            catch (Exception e) { AuctionStore.Log("tick error: " + e); }
            finally { Interlocked.Exchange(ref ticking, 0); }
        }

        /// <summary>A move whose player save failed is confirmed once that player is saved (online players only).</summary>
        static void RetryConfirm()
        {
            List<AhListing> todo;
            lock (AuctionStore.Sync)
                todo = AuctionStore.Listings.Values.Where(l =>
                    (!l.EscrowConfirmed && l.State == AhState.Active) ||
                    (!l.DeliveryConfirmed && (l.State == AhState.Sold || l.State == AhState.Closed))).ToList();
            foreach (var l in todo)
            {
                uint who = l.EscrowConfirmed ? l.Receiver : l.Seller;
                Client.GameClient c;
                if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(who, out c) || !Usable(c)) continue;
                var p = new ClientPlayer(c);
                bool saved;
                try { saved = p.Save(); } catch { saved = false; }
                if (!saved) continue;
                lock (AuctionStore.Sync)
                {
                    if (!l.EscrowConfirmed && l.State == AhState.Active && !p.OwnsItem(l.Id) && FileHasItem(who, l.Id) == false) l.EscrowConfirmed = true;
                    else if (l.EscrowConfirmed && !l.DeliveryConfirmed && p.OwnsItem(l.DeliveredUid) && FileHasItem(who, l.DeliveredUid) == true) l.DeliveryConfirmed = true;
                    else continue;
                    AuctionStore.Save();
                    AuctionStore.Ledger("CONFIRM (retry) id=" + l.Id + " player " + who);
                }
            }
        }

        // ================================================================== saved item file check
        static unsafe bool? FileHasItem(uint owner, uint itemUid)
        {
            try
            {
                string path = System.IO.Path.Combine(Program.ServerConfig.DbLocation ?? "", "PlayersItems", owner + ".bin");
                if (!System.IO.File.Exists(path)) return false;
                byte[] data = System.IO.File.ReadAllBytes(path);
                int size = sizeof(Database.ClientItems.DBItem);
                if (data.Length < 4) return false;
                int count = BitConverter.ToInt32(data, 0), pos = 4;
                fixed (byte* b = data)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (pos + size > data.Length) return null;// truncated file: unknown, keep waiting
                        var it = *(Database.ClientItems.DBItem*)(b + pos);
                        pos += size;
                        if (it.UID == itemUid) return true;
                        for (uint k = 0; k < it.DepositeCount; k++)
                        {
                            if (pos + size > data.Length) return null;
                            var dep = *(Database.ClientItems.DBItem*)(b + pos);
                            pos += size;
                            if (dep.UID == itemUid) return true;
                        }
                    }
                }
                return false;
            }
            catch { return null; }
        }

        // ================================================================== GameClient adapter
        public sealed class ClientPlayer : IAhPlayer
        {
            readonly Client.GameClient c;
            public ClientPlayer(Client.GameClient client) { c = client; }
            public uint Uid { get { return c.Player.UID; } }
            public string Name { get { return c.Player.Name; } }
            public ulong Cps { get { return c.Player.ConquerPoints; } }

            public bool TrySpendCps(ulong amount, string why)
            {
                if (amount == 0) return true;
                uint cur = c.Player.ConquerPoints;
                if (amount > cur) return false;
                c.Player.ConquerPoints = cur - (uint)amount;
                AuctionStore.Log(c.Player.Name + "(" + c.Player.UID + ") CPs -" + amount + " (" + why + ") " + cur + " -> " + c.Player.ConquerPoints);
                return true;
            }

            public void AddCps(ulong amount, string why)
            {
                if (amount == 0) return;
                uint cur = c.Player.ConquerPoints;
                ulong next = (ulong)cur + amount;
                c.Player.ConquerPoints = next > uint.MaxValue ? uint.MaxValue : (uint)next;// the setter moves > 2,147,000,000 to the CPs bank
                AuctionStore.Log(c.Player.Name + "(" + c.Player.UID + ") CPs +" + amount + " (" + why + ") " + cur + " -> " + c.Player.ConquerPoints);
            }

            public bool PeekItem(uint itemUid, out AhItem item)
            {
                item = null;
                Game.MsgServer.MsgGameItem it;
                if (!c.Inventory.TryGetItem(itemUid, out it) || it == null) return false;
                bool anima = it.AnimaItemID > 0 || (it.ITEM_ID >= 4200001 && it.ITEM_ID <= 4200019);
                bool pinBlock = false;
                try { pinBlock = anima && !c.Player.TREPIN2 && c.Player.CheckPin(); } catch { pinBlock = anima; }
                item = new AhItem
                {
                    Uid = it.UID, ItemId = it.ITEM_ID, Amount = it.StackSize == 0 ? 1u : it.StackSize,
                    Bound = it.Bound >= 1, Locked = it.Locked != 0, Suspicious = it.Suspicious != 0, Inscribed = it.Inscribed != 0,
                    HasExtras = it.Purification.ItemUID != 0 || it.Refinary.ItemUID != 0,
                    Timed = it.Activate != 0 || it.TimeStamp.Ticks > 0,
                    Perfection = it.PerfectionLevel > 0 || it.PerfectionProgress > 0,
                    Deposits = it.Deposite != null && it.Deposite.Count > 0,
                    NoTrade = pinBlock || Database.ItemType.unabletradeitem.Contains(it.ITEM_ID) || (it.Agate_map != null && it.Agate_map.Count > 0),
                    Bless = it.Bless, Dur = it.Durability, MaxDur = it.MaximDurability, Exp = it.MythSoulProgress, Mut = it.Mutacion, Ups = it.UpsurgeID,
                    Blob = ItemCodec.Encode(it)
                };
                if (it.RelicAttributes != null)
                    for (int i = 0; i < 5 && i < it.RelicAttributes.Length; i++) item.Attr[i] = (uint)it.RelicAttributes[i];
                return true;
            }

            public bool RemoveItem(uint itemUid)
            {
                Game.MsgServer.MsgGameItem it;
                if (!c.Inventory.TryGetItem(itemUid, out it) || it == null) return false;
                using (var rec = new ServerSockets.RecycledPacket())
                    c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, rec.GetStream(), true);
                return !c.Inventory.ClientItems.ContainsKey(itemUid);
            }

            public bool HasSpace(int slots) { return c.Inventory.HaveSpace((byte)slots); }

            public uint GiveItem(byte[] blob)
            {
                var it = ItemCodec.Decode(blob);
                if (it == null || !c.Inventory.HaveSpace(1)) return 0;
                it.Position = 0;
                bool ok;
                using (var rec = new ServerSockets.RecycledPacket())
                    ok = c.Inventory.Update(it, Role.Instance.AddMode.ADD, rec.GetStream());
                return ok && c.Inventory.ClientItems.ContainsKey(it.UID) ? it.UID : 0;
            }

            public bool OwnsItem(uint itemUid)
            {
                foreach (var it in c.AllMyItems()) if (it != null && it.UID == itemUid) return true;
                return false;
            }

            public bool Save()
            {
                lock (Database.ServerDatabase.SavingObj)
                {
                    if (Program.ExitRequested || !c.FullLoading || c.OnInterServer || c.Player == null) return false;
                    Database.ServerDatabase.SaveClient(c);
                    return true;
                }
            }
        }
    }

    /// <summary>Escrowed item &lt;-&gt; bytes (the same Database.ClientItems.DBItem struct the server saves PlayersItems with).</summary>
    public static class ItemCodec
    {
        public static unsafe byte[] Encode(Game.MsgServer.MsgGameItem item)
        {
            var d = new Database.ClientItems.DBItem().GetDBItem(item);
            var buf = new byte[sizeof(Database.ClientItems.DBItem)];
            fixed (byte* b = buf) *(Database.ClientItems.DBItem*)b = d;
            return buf;
        }

        public static unsafe Game.MsgServer.MsgGameItem Decode(byte[] blob)
        {
            if (blob == null || blob.Length != sizeof(Database.ClientItems.DBItem)) return null;
            Database.ClientItems.DBItem d;
            fixed (byte* b = blob) d = *(Database.ClientItems.DBItem*)b;
            return d.GetDataItem();
        }
    }
}
