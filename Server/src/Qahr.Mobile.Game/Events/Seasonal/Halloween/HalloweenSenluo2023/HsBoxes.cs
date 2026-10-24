// ============================================================================
//  [HalloweenSenluo2023 2026-10-15] open handlers for festival boxes that have no server script, driven by data:
//  Data\<FestivalKey>\boxes.json (re-read on change) for the festivals of this agent group.
//    { "boxes": [ { "id": 3345658, "name": "Xuanyuan Box", "festival": "HalloweenSenluo2023",
//                   "give": [ {"id":730003,"count":2,"bound":true,"minutes":7200} ],      fixed contents
//                   "coins": [10,50], "points": 99,                                       festival currency (callback)
//                   "picks": 3, "pool": [ {"id":...,"count":1,"w":10}, {"coins":[10,50],"w":5} ],  random contents
//                   "_src": "official item text ..." } ] }
//  Safety: the festival must be running, the box must be in the bag, bag space for the WHOLE content is checked before
//  the box is removed, the box is removed BEFORE anything is given, 300 ms rate limit, every open is logged.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.HalloweenSenluo2023
{
    public static class HsBoxes
    {
        public sealed class Entry { public HsGrant Grant; public int CoinMin, CoinMax, W = 1; }
        public sealed class Box
        {
            public uint Id; public string Name = "", Festival = "";
            public readonly List<HsGrant> Give = new List<HsGrant>();
            public int CoinMin, CoinMax, Points, Picks;
            public readonly List<Entry> Pool = new List<Entry>();
        }

        /// <summary>The festivals whose Data\&lt;key&gt;\boxes.json is read.</summary>
        public static readonly string[] Keys = { "HalloweenSenluo2023", "Halloween2024", "PuppetTheater", "FairyStories", "EonspiritsReturn", "InnerPowerSecretland", "GearMythforging" };

        /// <summary>Festival currency (coins / points) per festival key: (client, coins, points, stream).</summary>
        public static readonly Dictionary<string, Action<Client.GameClient, int, int, ServerSockets.Packet>> Currency =
            new Dictionary<string, Action<Client.GameClient, int, int, ServerSockets.Packet>>(StringComparer.OrdinalIgnoreCase)
            {
                { "HalloweenSenluo2023", (c, coins, points, s) =>
                    {
                        var p = HsState.Of(c);
                        if (coins > 0) HsZones.GrantCoins(c, p, coins, s);
                        if (points > 0) { lock (HsState.Lock) p.Points += points; HsKit.Msg(c, "Congrats on earning " + points + " Macabre Points. You have earned " + p.Points + " Macabre Points in total."); }
                        HsState.Save();
                    } },
            };

        static Dictionary<uint, Box> _boxes = new Dictionary<uint, Box>();
        static readonly Dictionary<string, DateTime> _stamps = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        static DateTime _next = DateTime.MinValue;
        static readonly object Sync = new object();

        public static Dictionary<uint, Box> Table()
        {
            lock (Sync)
            {
                var now = DateTime.UtcNow;
                if (now < _next) return _boxes;
                _next = now.AddSeconds(5);
                bool changed = false;
                foreach (var k in Keys)
                {
                    string f = Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, k, "boxes.json");
                    DateTime st = File.Exists(f) ? File.GetLastWriteTimeUtc(f) : DateTime.MinValue;
                    DateTime old;
                    if (!_stamps.TryGetValue(k, out old) || old != st) { _stamps[k] = st; changed = true; }
                }
                if (!changed) return _boxes;
                var all = new Dictionary<uint, Box>();
                foreach (var k in Keys)
                {
                    string f = Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, k, "boxes.json");
                    if (!File.Exists(f)) continue;
                    try
                    {
                        var root = Festivals.FestivalLoader.ReadJson(f);
                        foreach (var o in Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(root, "boxes")) ?? new List<object>())
                        {
                            var d = Festivals.FestivalLoader.Obj(o);
                            if (d == null) continue;
                            var b = new Box { Id = Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "id")), Name = Festivals.FestivalLoader.Str(d, "name") ?? "", Festival = Festivals.FestivalLoader.Str(d, "festival") ?? k };
                            if (b.Id == 0) continue;
                            foreach (var g in Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(d, "give")) ?? new List<object>()) { var hg = Grant(Festivals.FestivalLoader.Obj(g)); if (hg != null) b.Give.Add(hg); }
                            Range(Festivals.FestivalLoader.Val(d, "coins"), out b.CoinMin, out b.CoinMax);
                            b.Points = (int)Math.Min(1000000, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "points")));
                            b.Picks = (int)Math.Min(10, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "picks")));
                            foreach (var e in Festivals.FestivalLoader.Arr(Festivals.FestivalLoader.Val(d, "pool")) ?? new List<object>())
                            {
                                var ed = Festivals.FestivalLoader.Obj(e);
                                if (ed == null) continue;
                                var en = new Entry { Grant = Grant(ed), W = Math.Max(1, (int)Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(ed, "w"))) };
                                Range(Festivals.FestivalLoader.Val(ed, "coins"), out en.CoinMin, out en.CoinMax);
                                if (en.Grant != null || en.CoinMax > 0) b.Pool.Add(en);
                            }
                            if (b.Pool.Count == 0) b.Picks = 0;
                            all[b.Id] = b;
                        }
                    }
                    catch (Exception e) { MyConsole.WriteLine("[HsBoxes] " + f + " ignored: " + e.Message, ConsoleColor.Yellow); }
                }
                _boxes = all;
                return _boxes;
            }
        }
        static HsGrant Grant(Dictionary<string, object> d)
        {
            if (d == null) return null;
            uint id = Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "id"));
            if (id == 0) return null;
            int count = (int)Math.Max(1, Math.Min(10000, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "count"))));
            int minutes = (int)Math.Min(525600, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "minutes")));
            return new HsGrant(id, count, Festivals.FestivalLoader.Bool(d, "bound") != false, minutes);
        }
        static void Range(object v, out int lo, out int hi)
        {
            lo = hi = 0;
            var a = Festivals.FestivalLoader.Arr(v);
            if (a != null && a.Count == 2) { lo = (int)Math.Min(100000, Festivals.FestivalLoader.U(a[0])); hi = (int)Math.Min(100000, Festivals.FestivalLoader.U(a[1])); }
            else if (v != null) lo = hi = (int)Math.Min(100000, Festivals.FestivalLoader.U(v));
            if (hi < lo) hi = lo;
        }

        public static bool Handles(uint itemId) { return Table().ContainsKey(itemId); }

        /// <summary>Item-use hook. True = handled (opened or refused with a message).</summary>
        public static bool OnUse(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            Box b;
            if (!Table().TryGetValue(item.ITEM_ID, out b)) return false;
            if (HsKit.Throttled(c.Player.UID, "box", 300)) return true;
            if (!HsKit.Running(b.Festival)) { HsKit.Msg(c, "The event is over. This item can`t be opened now."); return true; }
            if (HsKit.Count(c, b.Id) < 1) return true;
            // decide the random part first, then check the space for everything
            var picks = new List<Entry>();
            for (int i = 0; i < b.Picks; i++)
            {
                int total = b.Pool.Sum(e => e.W), roll = HsKit.Roll(Math.Max(1, total));
                foreach (var e in b.Pool) { if (roll < e.W) { picks.Add(e); break; } roll -= e.W; }
            }
            var grants = new List<HsGrant>(b.Give);
            grants.AddRange(picks.Where(e => e.Grant != null).Select(e => e.Grant));
            int coins = b.CoinMax > 0 ? HsKit.Rand(b.CoinMin, b.CoinMax) : 0;
            foreach (var e in picks) if (e.CoinMax > 0) coins += HsKit.Rand(e.CoinMin, e.CoinMax);
            Action<Client.GameClient, int, int, ServerSockets.Packet> cur;
            bool hasCur = Currency.TryGetValue(b.Festival, out cur);
            int slots = HsKit.SlotsFor(grants) + (coins > 0 && hasCur ? 1 : 0);
            if (!HsKit.HasSpace(c, Math.Max(1, slots))) { HsKit.Msg(c, "Your inventory is full. Please prepare at least " + Math.Max(1, slots) + " space(s) in your inventory first."); return true; }
            if (!HsKit.Take(c, b.Id, 1, stream)) return true;// removed BEFORE the contents
            var got = new List<string>();
            foreach (var g in grants) { string s = HsKit.Give(c, g, stream); got.Add(s ?? ("FAILED " + g.Id)); }
            if (hasCur && (coins > 0 || b.Points > 0)) cur(c, coins, b.Points, stream);
            if (coins > 0) got.Add(coins + " coins");
            if (b.Points > 0) got.Add(b.Points + " points");
            try { Festivals.FestivalManager.Log("BOX", b.Festival, c.Player.UID + " " + c.Player.Name + " | opened " + b.Id + " " + b.Name + " -> " + string.Join(", ", got)); } catch { }
            var shown = got.Where(s => !s.StartsWith("FAILED") && !s.EndsWith(" coins") && !s.EndsWith(" points")).ToList();
            if (shown.Count > 0) HsKit.Msg(c, "You opened the " + (b.Name.Length > 0 ? b.Name : HsKit.Name(b.Id)) + " and received " + string.Join(", ", shown) + ".");
            return true;
        }

        public static void ResetForTests() { lock (Sync) { _stamps.Clear(); _next = DateTime.MinValue; } }
    }
}
