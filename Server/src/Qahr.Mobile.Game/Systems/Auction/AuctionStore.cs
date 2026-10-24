// [Auction 2026-10-14] Auction House data: config, listings (escrow), ledger and logs.
//
// Files (Data\Auction\):
//   Auction.ini        config. "official" = from the 7957 client (Cn_Res.ini / Info.ini defaults / client code),
//                      "derived" = server-side numbers the client does not carry (marked in the file).
//   listings.txt       every listing + its escrowed item (ClientItems.DBItem bytes, base64). Rewritten atomically
//                      (tmp + replace) after every change.
//   _ledger.log        append-only: one line per state change (LIST, CANCEL, EXPIRE, BUY, DELIVER, CREDIT, DELIST ...).
//   logs\auction_yyyy-MM-dd.log   human readable log.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.AuctionHouse
{
    public enum AhState : int { Active = 0, Returned = 1, Sold = 2, Closed = 3, Void = 4 }
    public enum AhPay : int { None = 0, Pending = 1, Crediting = 2, Credited = 3 }

    public sealed class AhListing
    {
        public uint Id;                 // = UID of the escrowed item
        public uint Seller;
        public string SellerName = "";
        public uint ItemId;
        public uint Amount;
        public uint UnitPrice;
        public uint Class;              // itemtype column 58 (2101..2106 = L1..L6 Mythsoul)
        public long Created, Expires, Changed;
        public AhState State;
        public AhState PrevState;       // state to go back to when an unconfirmed Sold/Closed is rolled back
        public bool EscrowConfirmed;    // the seller's file was saved WITHOUT the item
        public uint Buyer;              // Sold: buyer uid
        public uint DeliveredUid;       // Sold/Closed: item uid as it landed in the receiver's bag
        public bool DeliveryConfirmed;  // the receiver's file was saved WITH the item
        public ulong Fee, Total, Net;
        public AhPay Pay;
        // display copy of the item (browse rows / my list)
        public uint Bless, Dur, MaxDur, Exp, Mut, Ups;
        public uint[] Attr = new uint[5];
        public byte[] Blob;

        public uint Receiver { get { return State == AhState.Sold ? Buyer : Seller; } }
    }

    public sealed class AhConfig
    {
        public bool Enabled = true;
        public int ListingHours = 48;            // official: STR_AUCTION_SELL_TIME "Duration: 48 hours"
        public int MaxListings = 10;             // official: client default of ini/Info.ini AuctionMyList (10)
        public int PageSize = 7;                 // official: CDlgAuctionView / CDlgAuctionSell show 7 rows
        public bool RequireDelistFirst = true;   // official: STR_AUCTION_HAS_TIME_OUT_ITEM "Please delist them first."
        public uint MinUnitPrice = 1;            // derived
        public ulong MaxTotalPrice = 2000000000; // derived (fits the client's u32 price fields)
        public uint ListingFeePermille = 10;     // derived: 1% of the total price, paid when listing, not refunded
        public uint MinListingFee = 1;           // derived
        public uint SalesTaxPermille = 0;        // derived: cut taken from the seller's income (0 = none)
        public int ExpiryCheckSeconds = 30;      // derived
        public int ActionIntervalMs = 500;       // derived (rate limit of write actions)
        public int ListIntervalMs = 300;         // derived (rate limit of list requests)
        public int KeepClosedDays = 30;          // derived (finished listings are pruned from listings.txt after this)
    }

    public static class AuctionStore
    {
        public static readonly object Sync = new object();
        public static AhConfig Config = new AhConfig();
        public static readonly Dictionary<uint, AhListing> Listings = new Dictionary<uint, AhListing>();
        public static readonly Dictionary<uint, uint> ClassByItem = new Dictionary<uint, uint>();
        public static bool Loaded;
        public static string LoadError;

        /// <summary>Folder (test seam). Default: DbLocation\Auction.</summary>
        public static string Folder;
        static string Dir
        {
            get
            {
                if (Folder == null)
                    Folder = Path.Combine(Program.ServerConfig.DbLocation ?? "", "Auction");
                return Folder;
            }
        }
        public static string ListingsPath { get { return Path.Combine(Dir, "listings.txt"); } }
        public static string ConfigPath { get { return Path.Combine(Dir, "Auction.ini"); } }
        public static string LedgerPath { get { return Path.Combine(Dir, "_ledger.log"); } }
        /// <summary>itemtype.txt used for the class column (test seam).</summary>
        public static string ItemTypePath;

        public static long Unix(DateTime t) { return (long)(t.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }

        // ------------------------------------------------------------------ load
        /// <summary>Loads config + listings (no reconcile; AuctionEngine.EnsureLoaded does that).</summary>
        public static void LoadRaw()
        {
            Directory.CreateDirectory(Dir);
            Directory.CreateDirectory(Path.Combine(Dir, "logs"));
            LoadConfig();
            LoadClasses();
            Listings.Clear();
            if (File.Exists(ListingsPath))
            {
                foreach (var line in File.ReadAllLines(ListingsPath, Encoding.UTF8))
                {
                    if (line.Length == 0 || line[0] == '#') continue;
                    var l = ParseListing(line);
                    if (l != null) Listings[l.Id] = l;
                    else Log("LOAD bad line skipped: " + (line.Length > 80 ? line.Substring(0, 80) : line));
                }
            }
            Loaded = true;
        }

        static void LoadConfig()
        {
            var c = new AhConfig();
            if (!File.Exists(ConfigPath)) WriteDefaultConfig(c);
            foreach (var raw in File.ReadAllLines(ConfigPath, Encoding.UTF8))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#' || line[0] == '[') continue;
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                int sc = v.IndexOf(';'); if (sc >= 0) v = v.Substring(0, sc).Trim();
                long n; long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n);
                switch (k)
                {
                    case "Enabled": c.Enabled = n != 0; break;
                    case "ListingHours": c.ListingHours = (int)Clamp(n, 1, 24 * 30); break;
                    case "MaxListings": c.MaxListings = (int)Clamp(n, 1, 100); break;
                    case "PageSize": c.PageSize = (int)Clamp(n, 1, 7); break;
                    case "RequireDelistFirst": c.RequireDelistFirst = n != 0; break;
                    case "MinUnitPrice": c.MinUnitPrice = (uint)Clamp(n, 1, 2000000000); break;
                    case "MaxTotalPrice": c.MaxTotalPrice = (ulong)Clamp(n, 1, 2000000000); break;
                    case "ListingFeePermille": c.ListingFeePermille = (uint)Clamp(n, 0, 500); break;
                    case "MinListingFee": c.MinListingFee = (uint)Clamp(n, 0, 1000000); break;
                    case "SalesTaxPermille": c.SalesTaxPermille = (uint)Clamp(n, 0, 500); break;
                    case "ExpiryCheckSeconds": c.ExpiryCheckSeconds = (int)Clamp(n, 5, 3600); break;
                    case "ActionIntervalMs": c.ActionIntervalMs = (int)Clamp(n, 100, 10000); break;
                    case "ListIntervalMs": c.ListIntervalMs = (int)Clamp(n, 50, 10000); break;
                    case "KeepClosedDays": c.KeepClosedDays = (int)Clamp(n, 1, 3650); break;
                }
            }
            Config = c;
        }

        static long Clamp(long v, long lo, long hi) { return v < lo ? lo : v > hi ? hi : v; }

        static void WriteDefaultConfig(AhConfig c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("; [Auction 2026-10-14] Auction House (CDlgAuction #572). official = 7957 client, derived = server choice.");
            sb.AppendLine("[Auction]");
            sb.AppendLine("Enabled=1");
            sb.AppendLine("ListingHours=" + c.ListingHours + "            ; official: STR_AUCTION_SELL_TIME \"Duration: 48 hours\"");
            sb.AppendLine("MaxListings=" + c.MaxListings + "             ; official: client default of Info.ini AuctionMyList");
            sb.AppendLine("PageSize=" + c.PageSize + "                 ; official: 7 rows per page in the client lists");
            sb.AppendLine("RequireDelistFirst=1        ; official: STR_AUCTION_HAS_TIME_OUT_ITEM (delist expired items before listing)");
            sb.AppendLine("MinUnitPrice=" + c.MinUnitPrice + "              ; derived");
            sb.AppendLine("MaxTotalPrice=" + c.MaxTotalPrice + "    ; derived (client price fields are u32)");
            sb.AppendLine("ListingFeePermille=" + c.ListingFeePermille + "       ; derived: 10 = 1% of the total price, paid in CPs when listing, not refunded");
            sb.AppendLine("MinListingFee=" + c.MinListingFee + "             ; derived");
            sb.AppendLine("SalesTaxPermille=" + c.SalesTaxPermille + "          ; derived: cut taken from the seller's income (0 = none)");
            sb.AppendLine("ExpiryCheckSeconds=" + c.ExpiryCheckSeconds + "       ; derived");
            sb.AppendLine("ActionIntervalMs=" + c.ActionIntervalMs + "       ; derived (per player rate limit of write actions)");
            sb.AppendLine("ListIntervalMs=" + c.ListIntervalMs + "         ; derived (per player rate limit of list requests)");
            sb.AppendLine("KeepClosedDays=" + c.KeepClosedDays + "           ; derived");
            File.WriteAllText(ConfigPath, sb.ToString(), new UTF8Encoding(false));
        }

        /// <summary>Auction class of every itemtype = column 58 of itemtype.txt (same data as the client's itemtype.dat).</summary>
        static void LoadClasses()
        {
            ClassByItem.Clear();
            string path = ItemTypePath ?? Path.Combine(Program.ServerConfig.DbLocation ?? "", "itemtype.txt");
            if (!File.Exists(path)) { LoadError = "itemtype.txt not found: " + path; return; }
            foreach (var line in File.ReadLines(path, Encoding.UTF8))
            {
                int c = 0, start = 0; uint id = 0; bool ok = true;
                for (int i = 0; i <= line.Length - 2 && ok; i++)
                {
                    if (line[i] != '@' || line[i + 1] != '@') continue;
                    if (c == 0 && !uint.TryParse(line.Substring(0, i), out id)) ok = false;
                    c++; i++; start = i + 1;
                    if (c == 58)
                    {
                        int end = line.IndexOf("@@", start, StringComparison.Ordinal);
                        if (end < 0) end = line.Length;
                        uint cls;
                        if (uint.TryParse(line.Substring(start, end - start), out cls) && cls != 0) ClassByItem[id] = cls;
                        break;
                    }
                }
            }
        }

        public static uint ClassOf(uint itemId)
        {
            uint c;
            return ClassByItem.TryGetValue(itemId, out c) ? c : 0;
        }

        // ------------------------------------------------------------------ save
        static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            foreach (char ch in s) if (ch != '|' && ch != '\r' && ch != '\n') sb.Append(ch);
            return sb.ToString();
        }

        static string Line(AhListing l)
        {
            var inv = CultureInfo.InvariantCulture;
            return string.Join("|", new[]
            {
                "L", l.Id.ToString(inv), l.Seller.ToString(inv), Esc(l.SellerName), l.ItemId.ToString(inv), l.Amount.ToString(inv),
                l.UnitPrice.ToString(inv), l.Class.ToString(inv), l.Created.ToString(inv), l.Expires.ToString(inv), l.Changed.ToString(inv),
                ((int)l.State).ToString(inv), ((int)l.PrevState).ToString(inv), l.EscrowConfirmed ? "1" : "0", l.Buyer.ToString(inv),
                l.DeliveredUid.ToString(inv), l.DeliveryConfirmed ? "1" : "0", l.Fee.ToString(inv), l.Total.ToString(inv), l.Net.ToString(inv),
                ((int)l.Pay).ToString(inv), l.Bless.ToString(inv), l.Dur.ToString(inv), l.MaxDur.ToString(inv), l.Exp.ToString(inv),
                l.Mut.ToString(inv), l.Ups.ToString(inv),
                string.Join(",", Array.ConvertAll(l.Attr ?? new uint[5], a => a.ToString(inv))),
                l.Blob == null ? "" : Convert.ToBase64String(l.Blob)
            });
        }

        static AhListing ParseListing(string line)
        {
            var p = line.Split('|');
            if (p.Length != 29 || p[0] != "L") return null;
            try
            {
                var inv = CultureInfo.InvariantCulture;
                var l = new AhListing
                {
                    Id = uint.Parse(p[1], inv), Seller = uint.Parse(p[2], inv), SellerName = p[3], ItemId = uint.Parse(p[4], inv),
                    Amount = uint.Parse(p[5], inv), UnitPrice = uint.Parse(p[6], inv), Class = uint.Parse(p[7], inv),
                    Created = long.Parse(p[8], inv), Expires = long.Parse(p[9], inv), Changed = long.Parse(p[10], inv),
                    State = (AhState)int.Parse(p[11], inv), PrevState = (AhState)int.Parse(p[12], inv), EscrowConfirmed = p[13] == "1",
                    Buyer = uint.Parse(p[14], inv), DeliveredUid = uint.Parse(p[15], inv), DeliveryConfirmed = p[16] == "1",
                    Fee = ulong.Parse(p[17], inv), Total = ulong.Parse(p[18], inv), Net = ulong.Parse(p[19], inv), Pay = (AhPay)int.Parse(p[20], inv),
                    Bless = uint.Parse(p[21], inv), Dur = uint.Parse(p[22], inv), MaxDur = uint.Parse(p[23], inv), Exp = uint.Parse(p[24], inv),
                    Mut = uint.Parse(p[25], inv), Ups = uint.Parse(p[26], inv),
                };
                var a = p[27].Split(',');
                for (int i = 0; i < 5 && i < a.Length; i++) l.Attr[i] = uint.Parse(a[i], inv);
                l.Blob = p[28].Length == 0 ? null : Convert.FromBase64String(p[28]);
                if (l.Id == 0 || l.Seller == 0) return null;
                return l;
            }
            catch { return null; }
        }

        /// <summary>Atomic rewrite of listings.txt. Caller holds Sync.</summary>
        public static void Save()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# [Auction 2026-10-14] listings - do not edit while the server runs");
            foreach (var l in Listings.Values) sb.AppendLine(Line(l));
            string tmp = ListingsPath + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            if (File.Exists(ListingsPath))
            {
                try { File.Replace(tmp, ListingsPath, ListingsPath + ".bak", true); }
                catch { File.Copy(tmp, ListingsPath, true); File.Delete(tmp); }
            }
            else File.Move(tmp, ListingsPath);
        }

        // ------------------------------------------------------------------ logs
        static readonly object logSync = new object();
        static readonly Encoding Utf8 = new UTF8Encoding(false);
        public static Func<DateTime> Clock = () => DateTime.Now;

        public static void Ledger(string text)
        {
            lock (logSync)
            {
                try { File.AppendAllText(LedgerPath, Clock().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + text + Environment.NewLine, Utf8); }
                catch { }
            }
            Log(text);
        }

        public static void Log(string text)
        {
            lock (logSync)
            {
                try
                {
                    var now = Clock();
                    string path = Path.Combine(Dir, "logs", "auction_" + now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".log");
                    File.AppendAllText(path, now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " " + text + Environment.NewLine, Utf8);
                }
                catch { }
            }
        }
    }
}
