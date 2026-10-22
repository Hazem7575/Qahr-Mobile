// [Auction 2026-10-14] Auction House rules + escrow. Pure logic over IAhPlayer (GameClient adapter in AuctionModule,
// fake players in the test harness). Every public method takes AuctionStore.Sync.
//
// Anti-duplication design (see also the "Reconcile" notes):
//   * The item leaves the bag BEFORE the listing exists; the buyer pays BEFORE the item is given; the seller's income
//     is credited only after the buyer's save was confirmed, and is marked "Crediting" BEFORE the CPs are added.
//   * Every listing is a state machine (Active -> Sold | Returned -> Closed). A request is applied only if the listing
//     is in the expected state, owned by client.Player.UID, and confirmed. A duplicate or replayed packet finds the
//     listing in the next state and is refused.
//   * Crash safety: each move is written to listings.txt as "unconfirmed" first, then the player is saved, then it
//     is confirmed. An unconfirmed move is settled by looking at the player's real items (in memory if online at
//     login, else PlayersItems\<uid>.bin): item still with the seller -> the listing is dropped; item not with the
//     buyer -> the sale is rolled back. Settling happens at load and at the player's login (before he can act).
//     A "Crediting" income is never paid twice (it is treated as paid).
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.AuctionHouse
{
    public enum AhResult
    {
        Ok, Disabled, BadRequest, NotFound, NotOwner, NotTradable, NoClass, BadPrice, BadAmount, NoMoney, ListingLimit,
        HasExpired, BagFull, PriceChanged, OwnListing, NotReady, Failed
    }

    /// <summary>Item as seen by the auction (built by the adapter from MsgGameItem).</summary>
    public sealed class AhItem
    {
        public uint Uid, ItemId, Amount;
        public bool Bound, Locked, Suspicious, Inscribed, HasExtras, Timed, Perfection, Deposits, NoTrade;
        public uint Bless, Dur, MaxDur, Exp, Mut, Ups;
        public uint[] Attr = new uint[5];
        public byte[] Blob;
    }

    public interface IAhPlayer
    {
        uint Uid { get; }
        string Name { get; }
        ulong Cps { get; }
        bool TrySpendCps(ulong amount, string why);
        void AddCps(ulong amount, string why);
        bool PeekItem(uint itemUid, out AhItem item);      // bag (inventory) only
        bool RemoveItem(uint itemUid);                     // removes the whole stack from the bag
        bool HasSpace(int slots);
        uint GiveItem(byte[] blob);                        // returns the uid the item got in the bag, 0 = failed
        bool OwnsItem(uint itemUid);                       // any container (bag, equipment, warehouses ...)
        bool Save();                                       // persist the player now; false = could not
    }

    public static class AuctionEngine
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        /// <summary>uid -> online player (null = offline). Set by the module / harness.</summary>
        public static Func<uint, IAhPlayer> Online = uid => null;
        /// <summary>(owner uid, item uid) -> true/false from the owner's saved item file; null = unknown (keep waiting).</summary>
        public static Func<uint, uint, bool?> FileHasItem = (owner, item) => null;
        /// <summary>itemtype -> name for the search filter.</summary>
        public static Func<uint, string> ItemName = id => null;

        static AhConfig Cfg { get { return AuctionStore.Config; } }
        static long NowUnix { get { return AuctionStore.Unix(Now()); } }

        public static void EnsureLoaded()
        {
            if (AuctionStore.Loaded) return;
            lock (AuctionStore.Sync)
            {
                if (AuctionStore.Loaded) return;
                try
                {
                    AuctionStore.LoadRaw();
                    ReconcileAll();
                }
                catch (Exception e)
                {
                    AuctionStore.LoadError = e.Message;
                    AuctionStore.Loaded = true;
                    Cfg.Enabled = false;// never trade on top of a store that did not load
                    AuctionStore.Log("LOAD FAILED, auction disabled: " + e);
                }
            }
        }

        public static uint SubClassOf(uint cls) { return cls >= 1000 ? cls : 0; }
        public static bool ClassMatches(uint wanted, uint itemCls)
        {
            if (wanted == 0) return true;
            if (wanted == itemCls) return true;
            return wanted < 1000 && itemCls / 100 == wanted;// top class (21) contains its sub classes (2101..2106)
        }

        static string Who(IAhPlayer p) { return p == null ? "?" : p.Name + "(" + p.Uid + ")"; }

        // ================================================================== sell
        public static AhResult Sell(IAhPlayer p, uint itemUid, uint unitPrice, uint count, out AhListing listing)
        {
            listing = null;
            EnsureLoaded();
            if (p == null || p.Uid == 0) return AhResult.NotReady;
            if (!Cfg.Enabled) return AhResult.Disabled;
            AhListing l;
            lock (AuctionStore.Sync)
            {
                AhItem it;
                if (itemUid == 0 || !p.PeekItem(itemUid, out it) || it == null || it.Uid != itemUid) return AhResult.NotFound;
                if (it.Bound || it.Locked || it.Suspicious || it.Inscribed || it.HasExtras || it.Timed || it.Perfection || it.Deposits || it.NoTrade || it.Blob == null)
                    return AhResult.NotTradable;
                uint cls = AuctionStore.ClassOf(it.ItemId);
                if (cls == 0) return AhResult.NoClass;
                uint amount = it.Amount == 0 ? 1 : it.Amount;
                if (count != amount) return AhResult.BadAmount;// a listing is always the whole stack
                if (unitPrice < Cfg.MinUnitPrice) return AhResult.BadPrice;
                ulong total = (ulong)unitPrice * amount;
                if (total == 0 || total > Cfg.MaxTotalPrice) return AhResult.BadPrice;
                if (AuctionStore.Listings.ContainsKey(itemUid)) return AhResult.Failed;// same uid already escrowed (never)
                int active = 0; bool expired = false;
                foreach (var x in AuctionStore.Listings.Values)
                {
                    if (x.Seller != p.Uid) continue;
                    if (x.State == AhState.Active) active++;
                    if (x.State == AhState.Returned) expired = true;
                }
                if (active >= Cfg.MaxListings) return AhResult.ListingLimit;
                if (expired && Cfg.RequireDelistFirst) return AhResult.HasExpired;
                ulong fee = total * Cfg.ListingFeePermille / 1000;
                if (fee < Cfg.MinListingFee) fee = Cfg.MinListingFee;
                if (Cfg.ListingFeePermille == 0 && Cfg.MinListingFee == 0) fee = 0;
                if (p.Cps < fee) return AhResult.NoMoney;

                // 1) fee and item leave the player (memory) - deduct before anything is created
                if (fee > 0 && !p.TrySpendCps(fee, "auction listing fee " + itemUid)) return AhResult.NoMoney;
                if (!p.RemoveItem(itemUid))
                {
                    if (fee > 0) p.AddCps(fee, "auction listing fee refund " + itemUid);
                    return AhResult.Failed;
                }
                long now = NowUnix;
                l = new AhListing
                {
                    Id = itemUid, Seller = p.Uid, SellerName = p.Name ?? "", ItemId = it.ItemId, Amount = amount, UnitPrice = unitPrice,
                    Class = cls, Created = now, Expires = now + (long)Cfg.ListingHours * 3600, Changed = now, State = AhState.Active,
                    PrevState = AhState.Active, EscrowConfirmed = false, Fee = fee, Total = total, Bless = it.Bless, Dur = it.Dur,
                    MaxDur = it.MaxDur, Exp = it.Exp, Mut = it.Mut, Ups = it.Ups, Attr = (uint[])it.Attr.Clone(), Blob = it.Blob
                };
                // 2) the listing is written (unconfirmed) BEFORE the player is saved
                AuctionStore.Listings[l.Id] = l;
                AuctionStore.Save();
                AuctionStore.Ledger("LIST id=" + l.Id + " seller=" + Who(p) + " item=" + l.ItemId + " x" + amount + " unit=" + unitPrice + " total=" + total + " fee=" + fee);
            }
            // 3) the player is saved without the item, then the escrow is confirmed
            // SaveClient logs and swallows its own errors: the saved file itself must show the item is gone
            bool saved = SafeSave(p) && FileHasItem(p.Uid, l.Id) != true;
            lock (AuctionStore.Sync)
            {
                if (saved && l.State == AhState.Active) { l.EscrowConfirmed = true; AuctionStore.Save(); }
                else AuctionStore.Log("LIST id=" + l.Id + " escrow NOT confirmed yet (player save not verified)");
            }
            listing = l;
            return AhResult.Ok;
        }

        // ================================================================== cancel (seller) / delist (expired)
        /// <summary>Seller cancels an active listing. Item back to the bag, or to the expired list if the bag is full.</summary>
        public static AhResult Cancel(IAhPlayer p, uint id, out bool toBag)
        {
            toBag = false;
            EnsureLoaded();
            if (p == null || p.Uid == 0) return AhResult.NotReady;
            if (!Cfg.Enabled) return AhResult.Disabled;
            AhListing l;
            lock (AuctionStore.Sync)
            {
                if (!AuctionStore.Listings.TryGetValue(id, out l) || l.State != AhState.Active || !l.EscrowConfirmed) return AhResult.NotFound;
                if (l.Seller != p.Uid) return AhResult.NotOwner;
                if (!p.HasSpace(1))
                {
                    l.State = AhState.Returned; l.Changed = NowUnix;
                    AuctionStore.Save();
                    AuctionStore.Ledger("CANCEL id=" + id + " seller=" + Who(p) + " -> returned list (bag full)");
                    return AhResult.Ok;
                }
                if (!ReturnToOwner(p, l, AhState.Active, "CANCEL")) return AhResult.Failed;
                toBag = true;
            }
            ConfirmDelivery(p, l);
            return AhResult.Ok;
        }

        /// <summary>Owner takes an expired / returned item back to the bag (2028 action 8).</summary>
        public static AhResult Delist(IAhPlayer p, uint id)
        {
            EnsureLoaded();
            if (p == null || p.Uid == 0) return AhResult.NotReady;
            AhListing l;
            lock (AuctionStore.Sync)
            {
                if (!AuctionStore.Listings.TryGetValue(id, out l) || l.State != AhState.Returned || !l.EscrowConfirmed) return AhResult.NotFound;
                if (l.Seller != p.Uid) return AhResult.NotOwner;
                if (!p.HasSpace(1)) return AhResult.BagFull;
                if (!ReturnToOwner(p, l, AhState.Returned, "DELIST")) return AhResult.Failed;
            }
            ConfirmDelivery(p, l);
            return AhResult.Ok;
        }

        /// <summary>Caller holds Sync. Item goes back to its owner; listing -> Closed (unconfirmed).</summary>
        static bool ReturnToOwner(IAhPlayer p, AhListing l, AhState from, string what)
        {
            uint got;
            if (p.OwnsItem(l.Id)) got = l.Id;// already with the owner (replay after a rollback) - never give twice
            else got = p.GiveItem(l.Blob);
            if (got == 0) return false;
            l.PrevState = from; l.State = AhState.Closed; l.DeliveredUid = got; l.DeliveryConfirmed = false; l.Changed = NowUnix;
            AuctionStore.Save();
            AuctionStore.Ledger(what + " id=" + l.Id + " owner=" + Who(p) + " item=" + l.ItemId + " x" + l.Amount + " -> bag uid=" + got);
            return true;
        }

        static void ConfirmDelivery(IAhPlayer p, AhListing l)
        {
            bool saved = SafeSave(p) && FileHasItem(p.Uid, l.DeliveredUid) != false;// the saved file must hold the item
            lock (AuctionStore.Sync)
            {
                if (saved && !l.DeliveryConfirmed && (l.State == AhState.Closed || l.State == AhState.Sold))
                {
                    l.DeliveryConfirmed = true;
                    AuctionStore.Save();
                    AuctionStore.Ledger("CONFIRM id=" + l.Id + " delivered to " + Who(p));
                }
            }
        }

        static bool SafeSave(IAhPlayer p)
        {
            try { return p.Save(); }
            catch (Exception e) { AuctionStore.Log("SAVE failed for " + Who(p) + ": " + e.Message); return false; }
        }

        // ================================================================== buy
        public static AhResult Buy(IAhPlayer b, uint id, uint itemId, uint count, uint unitPrice, out AhListing listing)
        {
            listing = null;
            EnsureLoaded();
            if (b == null || b.Uid == 0) return AhResult.NotReady;
            if (!Cfg.Enabled) return AhResult.Disabled;
            AhListing l;
            lock (AuctionStore.Sync)
            {
                if (!AuctionStore.Listings.TryGetValue(id, out l) || l.State != AhState.Active || !l.EscrowConfirmed) return AhResult.NotFound;
                if (l.Expires <= NowUnix) return AhResult.NotFound;
                if (l.Seller == b.Uid) return AhResult.OwnListing;
                if (itemId != l.ItemId || unitPrice != l.UnitPrice) return AhResult.PriceChanged;
                if (count != l.Amount) return AhResult.BadAmount;
                ulong total = (ulong)l.UnitPrice * l.Amount;
                if (total != l.Total || total == 0) return AhResult.Failed;
                if (b.Cps < total) return AhResult.NoMoney;
                if (!b.HasSpace(1)) return AhResult.BagFull;
                if (b.OwnsItem(l.Id)) return AhResult.Failed;// would collide with an item the buyer already has

                // 1) pay first
                if (!b.TrySpendCps(total, "auction buy " + l.Id)) return AhResult.NoMoney;
                // 2) the item is given
                uint got = b.GiveItem(l.Blob);
                if (got == 0)
                {
                    b.AddCps(total, "auction buy refund " + l.Id);
                    return AhResult.BagFull;
                }
                ulong tax = total * Cfg.SalesTaxPermille / 1000;
                l.PrevState = AhState.Active; l.State = AhState.Sold; l.Buyer = b.Uid; l.DeliveredUid = got; l.DeliveryConfirmed = false;
                l.Net = total - tax; l.Pay = AhPay.Pending; l.Changed = NowUnix;
                AuctionStore.Save();
                AuctionStore.Ledger("BUY id=" + l.Id + " buyer=" + Who(b) + " seller=" + l.SellerName + "(" + l.Seller + ") item=" + l.ItemId + " x" + l.Amount
                    + " paid=" + total + " tax=" + tax + " sellerNet=" + l.Net + " -> bag uid=" + got);
            }
            ConfirmDelivery(b, l);
            listing = l;
            return AhResult.Ok;
        }

        // ================================================================== seller income
        /// <summary>Credits every confirmed, unpaid sale of an online seller. Returns the CPs credited.</summary>
        public static ulong CreditIncome(IAhPlayer s)
        {
            EnsureLoaded();
            if (s == null || s.Uid == 0) return 0;
            var paid = new List<AhListing>();
            ulong sum = 0;
            lock (AuctionStore.Sync)
            {
                foreach (var l in AuctionStore.Listings.Values)
                {
                    if (l.Seller != s.Uid || l.State != AhState.Sold || !l.DeliveryConfirmed || l.Pay != AhPay.Pending) continue;
                    l.Pay = AhPay.Crediting;// marked BEFORE the CPs exist: a crash can lose, never double, an income
                    paid.Add(l);
                }
                if (paid.Count == 0) return 0;
                AuctionStore.Save();
                foreach (var l in paid)
                {
                    s.AddCps(l.Net, "auction sale " + l.Id);
                    sum += l.Net;
                    AuctionStore.Ledger("CREDIT id=" + l.Id + " seller=" + Who(s) + " +" + l.Net + " CPs");
                }
            }
            bool saved = SafeSave(s);
            lock (AuctionStore.Sync)
            {
                foreach (var l in paid) if (saved) l.Pay = AhPay.Credited;
                if (saved) AuctionStore.Save();
            }
            return sum;
        }

        // ================================================================== expiry
        /// <summary>Active listings past their end -> Returned. Returns them (to notify online sellers).</summary>
        public static List<AhListing> ExpireDue()
        {
            EnsureLoaded();
            var list = new List<AhListing>();
            lock (AuctionStore.Sync)
            {
                long now = NowUnix;
                foreach (var l in AuctionStore.Listings.Values)
                    if (l.State == AhState.Active && l.EscrowConfirmed && l.Expires <= now)
                    {
                        l.State = AhState.Returned; l.Changed = now;
                        list.Add(l);
                        AuctionStore.Ledger("EXPIRE id=" + l.Id + " seller=" + l.SellerName + "(" + l.Seller + ") item=" + l.ItemId + " x" + l.Amount);
                    }
                long keep = now - (long)Cfg.KeepClosedDays * 86400;
                var drop = AuctionStore.Listings.Values.Where(l => l.Changed < keep &&
                    ((l.State == AhState.Closed && l.DeliveryConfirmed) || l.State == AhState.Void ||
                     (l.State == AhState.Sold && l.DeliveryConfirmed && l.Pay == AhPay.Credited))).Select(l => l.Id).ToList();
                foreach (var id in drop) AuctionStore.Listings.Remove(id);
                if (list.Count > 0 || drop.Count > 0) AuctionStore.Save();
            }
            return list;
        }

        // ================================================================== queries
        public static List<AhListing> Browse(uint cls, string name, uint page, out uint total)
        {
            EnsureLoaded();
            int size = Cfg.PageSize;
            string q = Norm(name);
            lock (AuctionStore.Sync)
            {
                long now = NowUnix;
                var all = AuctionStore.Listings.Values.Where(l => l.State == AhState.Active && l.EscrowConfirmed && l.Expires > now && ClassMatches(cls, l.Class)
                        && (q.Length == 0 || Norm(ItemName(l.ItemId)).Contains(q)))
                    .OrderBy(l => l.UnitPrice).ThenBy(l => l.Created).ThenBy(l => l.Id).ToList();
                total = (uint)all.Count;
                if (page > 100000) page = 100000;
                return all.Skip((int)page * size).Take(size).ToList();
            }
        }

        public static string Norm(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Length > 32) s = s.Substring(0, 32);
            var sb = new System.Text.StringBuilder();
            foreach (char c in s) if (char.IsLetterOrDigit(c)) sb.Append(char.ToLowerInvariant(c));
            return sb.ToString();
        }

        /// <summary>DirList rows: per sub class of <paramref name="cls"/> the number of active listings and the lowest unit price.</summary>
        public static List<AhDirListItemPB> Directory(uint cls, string name)
        {
            EnsureLoaded();
            string q = Norm(name);
            lock (AuctionStore.Sync)
            {
                long now = NowUnix;
                return AuctionStore.Listings.Values.Where(l => l.State == AhState.Active && l.EscrowConfirmed && l.Expires > now && ClassMatches(cls, l.Class)
                        && (q.Length == 0 || Norm(ItemName(l.ItemId)).Contains(q)))
                    .GroupBy(l => l.Class).OrderBy(g => g.Key)
                    .Select(g => new AhDirListItemPB { f1 = g.Key, f2 = (uint)g.Count(), f3 = g.Min(x => x.UnitPrice) }).ToList();
            }
        }

        public static List<AhListing> Mine(uint uid)
        {
            EnsureLoaded();
            lock (AuctionStore.Sync)
                return AuctionStore.Listings.Values.Where(l => l.Seller == uid && l.State == AhState.Active && l.EscrowConfirmed)
                    .OrderBy(l => l.Created).ToList();
        }

        public static List<AhListing> Expired(uint uid)
        {
            EnsureLoaded();
            lock (AuctionStore.Sync)
                return AuctionStore.Listings.Values.Where(l => l.Seller == uid && l.State == AhState.Returned && l.EscrowConfirmed)
                    .OrderBy(l => l.Changed).ThenBy(l => l.Id).ToList();
        }

        public static uint RemainingSeconds(AhListing l)
        {
            long r = l.Expires - NowUnix;
            return r <= 0 ? 0 : r > uint.MaxValue ? uint.MaxValue : (uint)r;
        }

        // ================================================================== reconcile (crash safety)
        /// <summary>Settles every unconfirmed move that involves <paramref name="uid"/> (all players when 0). Caller holds Sync.</summary>
        static void Reconcile(uint uid)
        {
            bool changed = false;
            foreach (var l in AuctionStore.Listings.Values.ToList())
            {
                // escrow: the item must have left the seller's saved items
                if (!l.EscrowConfirmed && (l.State == AhState.Active || l.State == AhState.Returned) && (uid == 0 || uid == l.Seller))
                {
                    bool? has = Has(l.Seller, l.Id);
                    if (has == true)
                    {
                        l.State = AhState.Void; l.Changed = NowUnix;// the seller still owns it: the listing never happened
                        AuctionStore.Ledger("RECONCILE id=" + l.Id + " seller " + l.Seller + " still has the item -> listing void");
                    }
                    else if (has == false)
                    {
                        l.EscrowConfirmed = true;
                        AuctionStore.Ledger("RECONCILE id=" + l.Id + " escrow confirmed");
                    }
                    changed |= has != null;
                }
                // delivery: the item must be in the receiver's saved items
                if (!l.DeliveryConfirmed && (l.State == AhState.Sold || l.State == AhState.Closed) && (uid == 0 || uid == l.Receiver))
                {
                    bool? has = Has(l.Receiver, l.DeliveredUid);
                    if (has == true)
                    {
                        l.DeliveryConfirmed = true;
                        AuctionStore.Ledger("RECONCILE id=" + l.Id + " delivery to " + l.Receiver + " confirmed");
                    }
                    else if (has == false)
                    {
                        // the receiver's save never happened: the move is undone (a buyer's CPs were not saved either)
                        var back = l.State == AhState.Sold ? AhState.Active : AhState.Returned;
                        if (back == AhState.Active && l.Expires <= NowUnix) back = AhState.Returned;
                        AuctionStore.Ledger("RECONCILE id=" + l.Id + " " + l.State + " to " + l.Receiver + " not saved -> back to " + back);
                        l.State = back; l.Buyer = 0; l.DeliveredUid = 0; l.Net = 0; l.Pay = AhPay.None; l.Changed = NowUnix;
                    }
                    changed |= has != null;
                }
                if (l.Pay == AhPay.Crediting && (uid == 0 || uid == l.Seller))
                {
                    l.Pay = AhPay.Credited;// never pay twice
                    AuctionStore.Ledger("RECONCILE id=" + l.Id + " income of " + l.Seller + " was being credited -> treated as paid (check logs)");
                    changed = true;
                }
            }
            if (changed) AuctionStore.Save();
        }

        static bool? Has(uint owner, uint itemUid)
        {
            if (owner == 0 || itemUid == 0) return null;
            var p = Online(owner);
            if (p != null) return p.OwnsItem(itemUid);
            return FileHasItem(owner, itemUid);
        }

        static void ReconcileAll() { Reconcile(0); }

        /// <summary>Login: settle this player's unconfirmed moves before he can act.</summary>
        public static void OnLogin(uint uid)
        {
            EnsureLoaded();
            lock (AuctionStore.Sync) Reconcile(uid);
        }

        public static bool HasReturned(uint uid)
        {
            EnsureLoaded();
            lock (AuctionStore.Sync)
                return AuctionStore.Listings.Values.Any(l => l.Seller == uid && l.State == AhState.Returned && l.EscrowConfirmed);
        }

        /// <summary>Test / admin: drop the in-memory state (next call reloads from disk).</summary>
        public static void Unload()
        {
            lock (AuctionStore.Sync) { AuctionStore.Loaded = false; AuctionStore.Listings.Clear(); }
        }
    }
}
