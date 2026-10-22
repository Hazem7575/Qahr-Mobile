// QahrMobile: Guild / World auction ("vendue") - tables, config and saved state.
//
// Folder Data\GuildAuction\ (Program.ServerConfig.DbLocation + "GuildAuction"):
//   GuildAuction.ini      config (written with defaults when missing).
//   cq_vendue_item.txt    decrypted copy of the mobile ini\cq_vendue_item.dat (tools\tqdat.py). The server MUST use the
//                         same rows as the client: a lot is sent as a row id and the client takes item / prices from it.
//   cq_vendue_type.txt    decrypted copy of the mobile ini\cq_vendue_type.dat (server-only table, the client never reads it).
//   state.json            lots, pending deliveries (items / refunds / earnings) and sale records. Rewritten atomically
//                         (tmp + replace) after every change.
//   logs\vendue_yyyy-MM-dd.log
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Qahr.Mobile.GuildAuction
{
    /// <summary>One row of cq_vendue_item (11 "@@" columns). The client reads columns 1,3..9 (CAuctionMgr::CreateVendueItemInfo).</summary>
    public sealed class VendueItemRow
    {
        public uint Id;          // c1  key (CVendueItemInfoPB.IdVendue)
        public uint Group;       // c2  item group (cq_vendue_type c6); skipped by the client
        public uint ItemType;    // c3
        public uint Amount;      // c4  stack shown on the icon
        public uint Monopoly;    // c5  1 = bound item (CItem::SetMonopolyPre)
        public uint MoneyType;   // c6  1 CPs (Emoney), 2 bound CPs (PresentEmoney), 1002 phase points
        public uint StartPrice;  // c7  price shown while there is no bid
        public uint Increment;   // c8  minimum raise
        public uint FixedPrice;  // c9  buy-out ("Fixed Price" button); a bid >= this ends the lot
        public uint MaxLots;     // c10 server-only, read as: how many lots of this row one session may hold
        public uint Weight;      // c11 server-only, read as: draw weight
    }

    /// <summary>One row of cq_vendue_type (13 columns, server-only). Meaning of the columns is inferred (see the doc).</summary>
    public sealed class VendueTypeRow
    {
        public uint Id;          // c1
        public uint Activity;    // c2  1 Guild Defense (rows 4-66), 3 World Auction (rows 1-3) = STR_VENDUE_ACTIVITY_TYPE_n
        public uint C3, C4;      // c3 0/1/2, c4 0/1 (unknown)
        public uint Level;       // c5  1..9 for the guild rows (read as guild level), 0 for world
        public uint Group;       // c6  item group of cq_vendue_item c2 (1..7 = Mon..Sun for guild rows; 8/9/10 world)
        public uint Count;       // c7  lots per session (10/8/6, world 50)
        public uint Duration;    // c8  1200 s bidding time
        public uint C9, C10;
        public uint Wait;        // c11 300 s preview before bidding opens
        public uint C12, C13;
    }

    public sealed class GaConfig
    {
        public bool Enabled = true;
        public string GuildSchedule = "Tue 19:30,Sun 19:30";  // official Guild Defense ends Tue/Sun 19:30 (STR_FACTION_ACTIVITY_TIME_6)
        public string WorldSchedule = "";                      // e.g. "Sat 22:00" (off by default)
        public int GuildMinMembers = 1;
        public int SharePercent = 100;          // part of a guild lot's price shared among the guild members of the session
        public int WaitSeconds = -1;            // -1 = cq_vendue_type c11
        public int DurationSeconds = -1;        // -1 = cq_vendue_type c8
        public int KeepEndedHours = 24;         // finished lots stay in the list ("Ended") this long
        public int KeepRecords = 300;
        public int BidIntervalMs = 400;
        public int TickSeconds = 5;
    }

    public enum LotState : int { Active = 0, Sold = 1, Unsold = 2 }

    public sealed class Lot
    {
        public ulong Id { get; set; }
        public uint JoinType { get; set; }      // 0 world, 1 guild
        public uint GuildId { get; set; }
        public uint Activity { get; set; }
        public uint IdVendue { get; set; }
        public long Begin { get; set; }
        public long End { get; set; }
        public uint Price { get; set; }
        public uint TopUid { get; set; }
        public string TopName { get; set; } = "";
        public LotState State { get; set; }
        public List<uint> Bidders { get; set; } = new List<uint>();
        public List<uint> Share { get; set; } = new List<uint>();
        public long Settled { get; set; }
    }

    /// <summary>Something owed to a player, delivered when he is online. Kind: "cps", "bcps", "item".</summary>
    public sealed class Pending
    {
        public ulong Id { get; set; }
        public uint Uid { get; set; }
        public string Kind { get; set; } = "";
        public uint Value { get; set; }
        public uint ItemType { get; set; }
        public uint Amount { get; set; }
        public bool Bound { get; set; }
        public string Text { get; set; } = "";
    }

    public sealed class Record
    {
        public long Unix { get; set; }
        public ulong Time { get; set; }         // MMddHHmmss (server local time)
        public uint JoinType { get; set; }
        public uint GuildId { get; set; }
        public uint IdVendue { get; set; }
        public uint Uid { get; set; }
        public string Name { get; set; } = "";
        public uint MoneyType { get; set; }
        public uint Money { get; set; }
    }

    public sealed class GaState
    {
        public ulong NextId { get; set; } = 1;
        public List<Lot> Lots { get; set; } = new List<Lot>();
        public List<Pending> Pending { get; set; } = new List<Pending>();
        public List<Record> Records { get; set; } = new List<Record>();
        public List<string> ScheduleDone { get; set; } = new List<string>();
    }

    public static class GuildAuctionStore
    {
        public static readonly object Sync = new object();
        public static GaConfig Config = new GaConfig();
        public static Dictionary<uint, VendueItemRow> Items = new Dictionary<uint, VendueItemRow>();
        public static Dictionary<uint, VendueTypeRow> Types = new Dictionary<uint, VendueTypeRow>();
        public static GaState State = new GaState();
        public static bool Loaded;

        public static string Dir { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "GuildAuction"); } }
        static string StatePath { get { return Path.Combine(Dir, "state.json"); } }
        static string ConfigPath { get { return Path.Combine(Dir, "GuildAuction.ini"); } }

        public static long Now { get { return DateTimeOffset.UtcNow.ToUnixTimeSeconds(); } }

        public static void Load()
        {
            lock (Sync)
            {
                Directory.CreateDirectory(Dir);
                Directory.CreateDirectory(Path.Combine(Dir, "logs"));
                LoadConfig();
                Items = LoadTable(Path.Combine(Dir, "cq_vendue_item.txt"), 11).ToDictionary(c => c[0], c => new VendueItemRow
                {
                    Id = c[0], Group = c[1], ItemType = c[2], Amount = c[3], Monopoly = c[4], MoneyType = c[5],
                    StartPrice = c[6], Increment = c[7], FixedPrice = c[8], MaxLots = c[9], Weight = c[10]
                });
                Types = LoadTable(Path.Combine(Dir, "cq_vendue_type.txt"), 13).ToDictionary(c => c[0], c => new VendueTypeRow
                {
                    Id = c[0], Activity = c[1], C3 = c[2], C4 = c[3], Level = c[4], Group = c[5], Count = c[6], Duration = c[7],
                    C9 = c[8], C10 = c[9], Wait = c[10], C12 = c[11], C13 = c[12]
                });
                if (File.Exists(StatePath))
                {
                    try { State = JsonSerializer.Deserialize<GaState>(File.ReadAllText(StatePath, Encoding.UTF8)) ?? new GaState(); }
                    catch (Exception e)
                    {
                        Log("LOAD state.json unreadable (" + e.Message + "), kept as state.bad.json");
                        try { File.Copy(StatePath, StatePath + ".bad", true); } catch { }
                        State = new GaState();
                    }
                }
                Loaded = true;
                Log("LOAD " + Items.Count + " items, " + Types.Count + " types, " + State.Lots.Count + " lots, " + State.Pending.Count + " pending");
            }
        }

        static List<uint[]> LoadTable(string path, int cols)
        {
            var rows = new List<uint[]>();
            if (!File.Exists(path)) { Log("LOAD missing " + path); return rows; }
            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == '#' || line.StartsWith("seed")) continue;
                var p = line.Split(new[] { "@@" }, StringSplitOptions.None);
                if (p.Length < cols) continue;
                var r = new uint[cols];
                bool ok = true;
                for (int i = 0; i < cols && ok; i++) ok = uint.TryParse(p[i].Trim(), out r[i]);
                if (ok) rows.Add(r);
            }
            return rows;
        }

        static void LoadConfig()
        {
            var c = new GaConfig();
            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, string.Join("\r\n", new[]
                {
                    "; Guild / World auction (mobile CFDlgFactionAuction). Saved changes apply with @gauction reload.",
                    "Enabled=1",
                    "; sessions: \"Day HH:mm\" list (server local time). Guild: one session per guild with >= GuildMinMembers members.",
                    "GuildSchedule=" + c.GuildSchedule,
                    "WorldSchedule=" + c.WorldSchedule,
                    "GuildMinMembers=" + c.GuildMinMembers,
                    "; percent of a sold guild lot's price shared equally among the guild members of the session",
                    "SharePercent=" + c.SharePercent,
                    "; -1 = from cq_vendue_type (c11 wait 300 s, c8 bidding 1200 s)",
                    "WaitSeconds=" + c.WaitSeconds,
                    "DurationSeconds=" + c.DurationSeconds,
                    "KeepEndedHours=" + c.KeepEndedHours,
                    "KeepRecords=" + c.KeepRecords,
                    "BidIntervalMs=" + c.BidIntervalMs,
                    "TickSeconds=" + c.TickSeconds,
                    ""
                }), Encoding.UTF8);
            }
            foreach (var raw in File.ReadAllLines(ConfigPath, Encoding.UTF8))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                int n; int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n);
                switch (k)
                {
                    case "Enabled": c.Enabled = n != 0; break;
                    case "GuildSchedule": c.GuildSchedule = v; break;
                    case "WorldSchedule": c.WorldSchedule = v; break;
                    case "GuildMinMembers": c.GuildMinMembers = n; break;
                    case "SharePercent": c.SharePercent = Math.Max(0, Math.Min(100, n)); break;
                    case "WaitSeconds": c.WaitSeconds = n; break;
                    case "DurationSeconds": c.DurationSeconds = n; break;
                    case "KeepEndedHours": c.KeepEndedHours = n; break;
                    case "KeepRecords": c.KeepRecords = n; break;
                    case "BidIntervalMs": c.BidIntervalMs = n; break;
                    case "TickSeconds": c.TickSeconds = Math.Max(1, n); break;
                }
            }
            Config = c;
        }

        /// <summary>Call inside lock(Sync).</summary>
        public static void Save()
        {
            try
            {
                string tmp = StatePath + ".tmp";
                File.WriteAllText(tmp, JsonSerializer.Serialize(State, new JsonSerializerOptions { WriteIndented = true }), Encoding.UTF8);
                if (File.Exists(StatePath)) File.Replace(tmp, StatePath, null);
                else File.Move(tmp, StatePath);
            }
            catch (Exception e) { Log("SAVE failed: " + e.Message); }
        }

        public static void Log(string text)
        {
            try
            {
                var now = DateTime.Now;
                File.AppendAllText(Path.Combine(Dir, "logs", "vendue_" + now.ToString("yyyy-MM-dd") + ".log"),
                    now.ToString("HH:mm:ss ") + text + "\r\n", Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>Decimal MMddHHmmss, the record time format of CAuctionRecordItem::SetInfo.</summary>
        public static ulong RecordTime(DateTime t)
        {
            return (ulong)t.Month * 100000000UL + (ulong)t.Day * 1000000UL + (ulong)t.Hour * 10000UL + (ulong)t.Minute * 100UL + (ulong)t.Second;
        }
    }
}
