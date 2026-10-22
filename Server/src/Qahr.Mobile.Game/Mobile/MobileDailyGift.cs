// ============================================================================
//  QahrMobile: Welfare > "Daily Packs" (client CDlgActivityDailyGift + CMyDailyGiftNode, state in CDailyGiftMgr).
//
//  Client -> server: CMsgMBAction 10046 (server DataMap 2099), protobuf field 12 Type, field 3 dwParam:
//    466 (0x1D2) query (dialog open, and once at login)
//    467 (0x1D3) dwParam 0 = the free chest, 1..3 = buy pack 1..3 (or claim it free while the 7-day set runs)
//    468 (0x1D4) dwParam = the milestone's times (3/9/15/18) = claim a "Total Times (Week)" reward
//    469 (0x1D5) "Buy All (1 Day)" (= the sum of the packs not bought today) / "Claim All" while the 7-day set runs
//    470 (0x1D6) "Buy All (7 Days)"
//  Server -> client: the same action back (any of 466..470); CMsgMBAction::Process copies it into
//  CDailyGiftMgr::DAILY_GIFT_INFO and refreshes the dialog (PostCmd 0xF59):
//    field 3 dwParam  = today's bits: bit0 free chest taken, bit1..3 = pack 1..3 bought/claimed
//    field 4 dwParam2 = purchases this week ("Total Times (Week)", progress bar = CumulativeGiftProCfg)
//    field 5 dwParam3 = claimed milestones, 64-bit mask, bit (times - 1)
//    field 1 ObjId    = start of the 7-day set (unix seconds, 0 = none); it runs [start, start + 7 days)
//    field 2 TargetUID= server time now (the client shows "Remaining: (start + 7d - this) / 86400 day(s)")
//  Prices and pack contents are drawn by the client from ini\luacfg\activity.dat; the server copy is
//  Data\mobile_dailygift.txt (re-read when saved). Tapping an item icon opens a local item tip (no packet).
//
//  State: EventStore (MySQL event_data, event "mdailygift"): t:day + c:bought (today's bits), t:week + c:count +
//  c:claimed (week = Monday's date), t:pass (7-day start). Written with flushNow BEFORE the CPs are taken and the
//  items given, so a DB failure grants and charges nothing. Daily reset = server's local midnight, weekly = Monday.
//
//  Routing (works without a restart): MsgDataPacket.Process (else branch: 467/468/469), MsgDataPacket.GiveOrder
//  (466 = ActionType.GiveOrder) and MsgDataPacket.ReturnReward (470 = ActionType.ReturnReward) call Handle() first.
//  Only mobile clients are taken; PC clients keep the old handlers.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Qahr.Mobile.EventData;

namespace Qahr.Mobile.Phone
{
    public static class MobileDailyGift
    {
        public const ushort ActQuery = 466, ActBuy = 467, ActMilestone = 468, ActBuyAll = 469, ActBuy7 = 470;
        const string Ev = "mdailygift";
        const long Week7 = 7 * 86400;

        sealed class Item { public uint Id; public ushort Count; public bool Bound; }
        sealed class Cfg
        {
            public Dictionary<int, int> Price = new Dictionary<int, int>();
            public Dictionary<int, List<Item>> Packs = new Dictionary<int, List<Item>>();
            public SortedDictionary<int, List<Item>> Milestones = new SortedDictionary<int, List<Item>>();
            public List<Item> Free = new List<Item>();
            public int Week7Price = 1999;
        }
        sealed class St { public long Bought, Count, Claimed, Pass; }

        static Cfg cfg;
        static DateTime cfgTime;
        static readonly object cfgLock = new object();

        static string CfgPath { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_dailygift.txt"); } }

        /// <summary>DataMap 2099 types 466..470 from a mobile client: true = handled here.</summary>
        public static bool Handle(Client.GameClient c, ServerSockets.Packet stream, Game.MsgServer.ActionQuery data)
        {
            if (data == null || data.Type < ActQuery || data.Type > ActBuy7) return false;
            if (c == null || c.Player == null || !MobileEvents.IsMobile(c)) return false;
            try
            {
                lock (c)
                {
                    switch (data.Type)
                    {
                        case ActQuery: break;
                        case ActBuy: if (data.dwParam == 0) Free(c, stream); else BuyPacks(c, stream, new[] { (int)data.dwParam }); break;
                        case ActMilestone: Milestone(c, stream, (int)data.dwParam); break;
                        case ActBuyAll: BuyPacks(c, stream, new[] { 1, 2, 3 }); break;
                        case ActBuy7: Buy7(c, stream); break;
                    }
                    SendState(c, stream, data.Type);
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[DailyGift] " + e); }
            return true;
        }

        // ------------------------------------------------------------------ actions
        static void BuyPacks(Client.GameClient c, ServerSockets.Packet stream, int[] wanted)
        {
            var cf = Load();
            uint uid = c.Player.UID;
            var st = Read(uid);
            long now = Now;
            bool pass = st.Pass > 0 && now >= st.Pass && now < st.Pass + Week7;
            var packs = wanted.Where(p => cf.Packs.ContainsKey(p) && (st.Bought & (1L << p)) == 0).ToList();
            if (packs.Count == 0) { if (wanted.Length == 1 && !cf.Packs.ContainsKey(wanted[0])) Msg(c, "This pack is not available."); return; }
            long cost = pass ? 0 : packs.Sum(p => (long)cf.Price[p]);
            var items = packs.SelectMany(p => cf.Packs[p]).ToList();
            if (!CanGive(c, items)) return;
            if (cost > c.Player.ConquerPoints) { Msg(c, "You don`t have enough CPs (" + cost + " needed)."); return; }
            long mask = packs.Aggregate(0L, (m, p) => m | (1L << p));
            bool ok;
            try
            {
                ok = EventStore.Edit(uid, Ev, s =>
                {
                    Normalize(s);
                    if ((s.Counter("bought") & mask) != 0) return Tuple.Create(false, false);// bought meanwhile
                    s.Counters["bought"] = s.Counter("bought") | mask;
                    s.Counters["count"] = s.Counter("count") + packs.Count;
                    return Tuple.Create(true, true);
                }, true);
            }
            catch (EventStore.EditFailed) { Msg(c, "The purchase could not be saved. Please try again later."); return; }
            if (!ok) return;
            if (cost > 0) c.Player.ConquerPoints -= (uint)cost;
            var given = Give(c, stream, items);
            Msg(c, (cost > 0 ? "You paid " + cost + " CPs. " : "") + "You received " + string.Join(", ", given) + ".");
            LogLine(c, (pass ? "CLAIM" : "BUY") + " packs " + string.Join("+", packs) + " cost " + cost + " gave [" + string.Join(", ", given) + "]");
        }

        static void Free(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cf = Load();
            uint uid = c.Player.UID;
            if ((Read(uid).Bought & 1) != 0) return;
            if (!CanGive(c, cf.Free)) return;
            bool ok;
            try
            {
                ok = EventStore.Edit(uid, Ev, s =>
                {
                    Normalize(s);
                    if ((s.Counter("bought") & 1) != 0) return Tuple.Create(false, false);
                    s.Counters["bought"] = s.Counter("bought") | 1;
                    return Tuple.Create(true, true);
                }, true);
            }
            catch (EventStore.EditFailed) { Msg(c, "The reward could not be saved. Please try again later."); return; }
            if (!ok) return;
            var given = Give(c, stream, cf.Free);
            if (given.Count > 0) Msg(c, "Free reward: you received " + string.Join(", ", given) + ".");
            LogLine(c, "FREE gave [" + string.Join(", ", given) + "]");
        }

        static void Milestone(Client.GameClient c, ServerSockets.Packet stream, int times)
        {
            var cf = Load();
            List<Item> items;
            if (times < 1 || times > 64 || !cf.Milestones.TryGetValue(times, out items)) return;
            uint uid = c.Player.UID;
            var st = Read(uid);
            long bit = 1L << (times - 1);
            if ((st.Claimed & bit) != 0) return;
            if (st.Count < times) { Msg(c, "Buy " + times + " packs this week to claim this reward (" + st.Count + " so far)."); return; }
            if (!CanGive(c, items)) return;
            bool ok;
            try
            {
                ok = EventStore.Edit(uid, Ev, s =>
                {
                    Normalize(s);
                    if ((s.Counter("claimed") & bit) != 0 || s.Counter("count") < times) return Tuple.Create(false, false);
                    s.Counters["claimed"] = s.Counter("claimed") | bit;
                    return Tuple.Create(true, true);
                }, true);
            }
            catch (EventStore.EditFailed) { Msg(c, "The reward could not be saved. Please try again later."); return; }
            if (!ok) return;
            var given = Give(c, stream, items);
            Msg(c, "Weekly reward (" + times + " times): you received " + string.Join(", ", given) + ".");
            LogLine(c, "MILESTONE " + times + " gave [" + string.Join(", ", given) + "]");
        }

        static void Buy7(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cf = Load();
            uint uid = c.Player.UID;
            var st = Read(uid);
            long now = Now;
            if (st.Pass > 0 && now < st.Pass + Week7) { Msg(c, "Your 7-day packs are still running."); return; }
            long cost = cf.Week7Price;
            if (cost > c.Player.ConquerPoints) { Msg(c, "You don`t have enough CPs (" + cost + " needed)."); return; }
            // a pack already bought today -> the set starts tomorrow (STR_DLG_ACTIVITY_BUY_7DAILY_GIFT_CONFIRM_BEGIN_TOMORROW)
            long start = (st.Bought & 0xE) != 0 ? new DateTimeOffset(DateTime.Today.AddDays(1)).ToUnixTimeSeconds() : now;
            bool ok;
            try
            {
                ok = EventStore.Edit(uid, Ev, s =>
                {
                    Normalize(s);
                    long p; s.Times.TryGetValue("pass", out p);
                    if (p > 0 && now < p + Week7) return Tuple.Create(false, false);
                    s.Times["pass"] = start;
                    return Tuple.Create(true, true);
                }, true);
            }
            catch (EventStore.EditFailed) { Msg(c, "The purchase could not be saved. Please try again later."); return; }
            if (!ok) return;
            c.Player.ConquerPoints -= (uint)cost;
            Msg(c, "You paid " + cost + " CPs for 7 days of daily packs" + (start > now ? " (from tomorrow)" : "") + ". Claim them in Welfare > Daily Packs every day.");
            LogLine(c, "BUY7 cost " + cost + " start " + start);
        }

        // ------------------------------------------------------------------ state
        static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
        static long Day(DateTime d) { return d.Year * 10000L + d.Month * 100 + d.Day; }
        static long WeekId { get { var t = DateTime.Today; return Day(t.AddDays(-(((int)t.DayOfWeek + 6) % 7))); } }

        /// <summary>Clears today's bits on a new day and the week's counter/claims on a new week.</summary>
        static void Normalize(EventSection s)
        {
            long v, today = Day(DateTime.Today), week = WeekId;
            if (!s.Times.TryGetValue("day", out v) || v != today) { s.Times["day"] = today; s.Counters.Remove("bought"); }
            if (!s.Times.TryGetValue("week", out v) || v != week) { s.Times["week"] = week; s.Counters.Remove("count"); s.Counters.Remove("claimed"); }
        }

        static St Read(uint uid)
        {
            var s = EventStore.Section(uid, Ev);// a copy
            Normalize(s);
            long p; s.Times.TryGetValue("pass", out p);
            return new St { Bought = s.Counter("bought"), Count = s.Counter("count"), Claimed = s.Counter("claimed"), Pass = p };
        }

        static void SendState(Client.GameClient c, ServerSockets.Packet stream, ushort type)
        {
            var st = Read(c.Player.UID);
            c.Send(Game.MsgServer.MsgDataPacket.ActionCreate(stream, new Game.MsgServer.ActionQuery
            {
                ObjId = (uint)Math.Max(0, st.Pass),
                TargetUID = (uint)Now,
                dwParam = (uint)st.Bought,
                dwParam2 = (int)st.Count,
                dwParam3 = (ulong)st.Claimed,
                Timestamp = 1,
                Type = type
            }));
        }

        // ------------------------------------------------------------------ items
        /// <summary>Every item type known and enough bag slots (counted as new stacks). Tells the player why not.</summary>
        static bool CanGive(Client.GameClient c, List<Item> items)
        {
            int slots = 0;
            foreach (var i in items)
            {
                Database.ItemType.DBItem db;
                if (!Pool.ItemsBase.TryGetValue(i.Id, out db))
                {
                    Msg(c, "This reward is not available yet (item " + i.Id + " is missing on the server). Nothing was charged.");
                    MyConsole.WriteLine("[DailyGift] item " + i.Id + " is not in itemtype.txt (Data\\mobile_dailygift.txt)");
                    return false;
                }
                slots += db.StackSize > 1 ? (i.Count + db.StackSize - 1) / db.StackSize : i.Count;
            }
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots)))
            {
                Msg(c, "Please make " + slots + " free space(s) in your inventory first. Nothing was charged.");
                return false;
            }
            return true;
        }

        static List<string> Give(Client.GameClient c, ServerSockets.Packet stream, List<Item> items)
        {
            var given = new List<string>();
            foreach (var i in items)
            {
                try
                {
                    if (c.Inventory.AddItemWitchStack(i.Id, 0, i.Count, stream, i.Bound))
                        given.Add(Pool.ItemsBase[i.Id].Name + (i.Count > 1 ? " x" + i.Count : ""));
                    else MyConsole.WriteLine("[DailyGift] could not give " + i.Id + " x" + i.Count + " to " + c.Player.Name);
                }
                catch (Exception e) { MyConsole.WriteLine("[DailyGift] give " + i.Id + ": " + e.Message); }
            }
            return given;
        }

        // ------------------------------------------------------------------ config
        static Cfg Load()
        {
            lock (cfgLock)
            {
                try
                {
                    var t = File.Exists(CfgPath) ? File.GetLastWriteTimeUtc(CfgPath) : DateTime.MinValue;
                    if (cfg != null && t == cfgTime) return cfg;
                    var n = Defaults();
                    if (t != DateTime.MinValue)
                    {
                        n = new Cfg();
                        foreach (var raw in File.ReadAllLines(CfgPath))
                        {
                            var line = raw.Split('#')[0].Trim();
                            if (line.Length == 0) continue;
                            var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            switch (p[0].ToLowerInvariant())
                            {
                                case "pack": if (p.Length >= 3) { int id = int.Parse(p[1]); n.Price[id] = int.Parse(p[2]); n.Packs[id] = Items(p, 3); } break;
                                case "week7": if (p.Length >= 2) n.Week7Price = int.Parse(p[1]); break;
                                case "milestone": if (p.Length >= 2) n.Milestones[int.Parse(p[1])] = Items(p, 2); break;
                                case "free": n.Free = Items(p, 1); break;
                            }
                        }
                    }
                    cfg = n; cfgTime = t;
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[DailyGift] " + CfgPath + ": " + e.Message);
                    if (cfg == null) cfg = Defaults();
                }
                return cfg;
            }
        }

        static List<Item> Items(string[] p, int from)
        {
            var l = new List<Item>();
            for (int i = from; i < p.Length; i++)
            {
                var f = p[i].Split(':');
                l.Add(new Item { Id = uint.Parse(f[0]), Count = f.Length > 1 ? ushort.Parse(f[1]) : (ushort)1, Bound = f.Length > 2 && f[2].StartsWith("b", StringComparison.OrdinalIgnoreCase) });
            }
            return l;
        }

        /// <summary>The official numbers (ini\luacfg\activity.dat) when the data file is missing.</summary>
        static Cfg Defaults()
        {
            var n = new Cfg();
            n.Price[1] = 19; n.Packs[1] = Items("x 730002:1:b 4060001:5".Split(' '), 1);
            n.Price[2] = 99; n.Packs[2] = Items("x 3339816:8 4060001:10".Split(' '), 1);
            n.Price[3] = 199; n.Packs[3] = Items("x 3339817:2 4060001:20".Split(' '), 1);
            n.Milestones[3] = Items("x 730002:1:b".Split(' '), 1);
            n.Milestones[9] = Items("x 4060001:10".Split(' '), 1);
            n.Milestones[15] = Items("x 3336123:1".Split(' '), 1);
            n.Milestones[18] = Items("x 3339817:2".Split(' '), 1);
            n.Free = Items("x 730001:1:b".Split(' '), 1);
            return n;
        }

        // ------------------------------------------------------------------ misc
        static void Msg(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void LogLine(Client.GameClient c, string text)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + c.Player.Name + " (" + c.Player.UID + ") " + text;
                MyConsole.WriteLine("[DailyGift] " + line);
                File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_dailygift_log.txt"), line + Environment.NewLine);
            }
            catch { }
        }
    }
}
