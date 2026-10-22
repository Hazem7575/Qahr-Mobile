// ============================================================================
//  [Thanksgiving2022 2026-10-15] HolidayKit - shared helpers for the holiday festival group owned by one agent:
//  Thanksgiving2022, ThanksgivingTribe2023, Thanksgiving2024, ThanksgivingFashion2025, Christmas2022, Christmas2023,
//  VillageRequests, TwinklingShard. Nothing here is used by other festivals.
//
//    HKit        bag counts, take / give (bag space checked first), CP spend, Lua item strings ("id plus count [3=bound]"),
//                weighted draws, throttles, two-click confirmation, festival log lines.
//    PlayerStore per-festival persistent state (Infinity[DB]\<Folder>\state.json): uid -> { key: long }. Written
//                atomically (tmp + replace) on every change, under one lock, BEFORE a reward is handed out.
//    Battles     private DynamicID copies of a real map: waves of festival monsters, a time limit, success / failure
//                callbacks. One battle per player; kills are polled (no shared kill hook is needed).
//    Boxes       open handlers for festival boxes; hook: MY System\MsgItem\MsgItemUsuagePacket.cs UseItem (one line).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Thanksgiving2022
{
    public struct HItem
    {
        public uint Id; public int Count; public bool Bound;
        public HItem(uint id, int count, bool bound = true) { Id = id; Count = count; Bound = bound; }
        /// <summary>Client Lua item string "id plus count [monopoly ...]" (monopoly 3 = bound).</summary>
        public static HItem Parse(string s)
        {
            var p = (s ?? "").Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            uint id = 0; int n = 1, mono = 0;
            if (p.Length > 0) uint.TryParse(p[0], out id);
            if (p.Length > 2) int.TryParse(p[2], out n);
            if (p.Length > 3) int.TryParse(p[3], out mono);
            return new HItem(id, Math.Max(1, n), mono == 3);
        }
        public string Lua() { return Id + " 0 " + Count + (Bound ? " 3" : ""); }
        public override string ToString() { return Id + "x" + Count; }
    }

    public static class HKit
    {
        // ------------------------------------------------------------------ test seams
        /// <summary>Harness seam: replaces the bag (count / take / give) and CP access.</summary>
        public interface IBag
        {
            uint Count(Client.GameClient c, uint id);
            bool Take(Client.GameClient c, uint id, uint n);
            bool Give(Client.GameClient c, uint id, int n, bool bound);
            int FreeSlots(Client.GameClient c);
            uint Cps(Client.GameClient c);
            bool TakeCps(Client.GameClient c, uint n);
        }
        public static IBag BagOverride;
        public static Func<DateTime> ClockOverride;
        public static DateTime Now { get { var o = ClockOverride; return o != null ? o() : DateTime.Now; } }
        public static Action<Client.GameClient, string> MsgOverride;
        public static readonly Random Rng = new Random();
        static readonly object _rngLock = new object();
        public static int Roll(int max) { lock (_rngLock) return Rng.Next(max); }

        public static void Msg(Client.GameClient c, string t)
        {
            var o = MsgOverride; if (o != null) { o(c, t); return; }
            try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        public static void Center(Client.GameClient c, string t)
        {
            var o = MsgOverride; if (o != null) { o(c, t); return; }
            try { c.SendSysMesage(t, MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow); } catch { }
            try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        public static void Log(string what, string festival, Client.GameClient c, string detail)
        {
            string who = c != null && c.Player != null ? c.Player.UID + " " + c.Player.Name + " lv" + c.Player.Level : "-";
            try { Festivals.FestivalManager.Log(what, festival, who + " | " + detail); } catch { }
        }

        // ------------------------------------------------------------------ throttles / confirm
        static readonly ConcurrentDictionary<string, long> _last = new ConcurrentDictionary<string, long>();
        public static bool Throttled(string scope, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            string k = scope + ":" + uid;
            if (_last.TryGetValue(k, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[k] = now;
            return false;
        }
        static readonly ConcurrentDictionary<uint, Tuple<string, DateTime>> _confirm = new ConcurrentDictionary<uint, Tuple<string, DateTime>>();
        /// <summary>First call arms (false), a second identical call within 15 s confirms (true).</summary>
        public static bool Confirmed(uint uid, string token)
        {
            Tuple<string, DateTime> t;
            var now = DateTime.UtcNow;
            if (_confirm.TryGetValue(uid, out t) && t.Item1 == token && now - t.Item2 < TimeSpan.FromSeconds(15))
            {
                _confirm.TryRemove(uid, out t);
                return true;
            }
            _confirm[uid] = Tuple.Create(token, now);
            return false;
        }

        // ------------------------------------------------------------------ bag
        public static uint Count(Client.GameClient c, uint id)
        {
            var o = BagOverride; if (o != null) return o.Count(c, id);
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        static int FreeSlots(Client.GameClient c)
        {
            var o = BagOverride; if (o != null) return o.FreeSlots(c);
            for (int k = 40; k > 0; k--) if (c.Inventory.HaveSpace((byte)k)) return k;
            return 0;
        }
        public static int SlotsFor(IEnumerable<HItem> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                if (i.Id == 0 || i.Count <= 0) continue;
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(i.Id, out db) && db.StackSize > 1) n += (i.Count + db.StackSize - 1) / db.StackSize;
                else n += i.Count;
            }
            return n;
        }
        public static bool HasRoom(Client.GameClient c, IEnumerable<HItem> items) { int need = SlotsFor(items); return need <= 0 || FreeSlots(c) >= need; }
        public static bool ItemExists(uint id)
        {
            if (BagOverride != null) return true;
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db);
        }

        /// <summary>Removes n items of one type (stacks first). Call only after Count() said there are enough.</summary>
        public static bool Take(Client.GameClient c, uint id, uint n)
        {
            if (n == 0) return true;
            var o = BagOverride; if (o != null) return o.Take(c, id, n);
            if (Count(c, id) < n) return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                uint left = n;
                foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).ToArray())
                {
                    if (left == 0) break;
                    uint stack = item.StackSize > 1 ? item.StackSize : 1u;
                    if (stack > left)
                    {
                        item.StackSize = (ushort)(stack - left);
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(c, stream);
                        left = 0;
                    }
                    else
                    {
                        left -= stack;
                        item.StackSize = 1;
                        c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    }
                }
                return left == 0;
            }
        }
        /// <summary>Gives every item (bag space must have been checked). Returns a readable list.</summary>
        public static string Give(Client.GameClient c, IEnumerable<HItem> items)
        {
            var given = new List<string>();
            var o = BagOverride;
            using (var rec = o == null ? new ServerSockets.RecycledPacket() : null)
            {
                var stream = rec == null ? null : rec.GetStream();
                foreach (var it in items)
                {
                    if (it.Id == 0 || it.Count <= 0) continue;
                    int left = it.Count;
                    bool ok = true;
                    while (left > 0 && ok)
                    {
                        ushort part = (ushort)Math.Min(left, 60000);
                        try { ok = o != null ? o.Give(c, it.Id, part, it.Bound) : c.Inventory.AddItemWitchStack(it.Id, 0, part, stream, it.Bound); }
                        catch { ok = false; }
                        left -= part;
                    }
                    given.Add((ok ? "" : "FAILED ") + Name(it.Id) + (it.Count > 1 ? " x" + it.Count : ""));
                }
            }
            return string.Join(", ", given);
        }
        public static string Name(uint id)
        {
            try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); }
        }
        /// <summary>Harness seam for Chi Points.</summary>
        public static Action<Client.GameClient, int> ChiOverride;
        public static string GiveChi(Client.GameClient c, int n)
        {
            if (n <= 0) return "";
            var o = ChiOverride; if (o != null) { o(c, n); return n + " Chi Points"; }
            try
            {
                c.Player.MyChi.ChiPoints = c.Player.MyChi.ChiPoints + n;
                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(c, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                return n + " Chi Points";
            }
            catch { return "FAILED " + n + " Chi Points"; }
        }
        public static uint Cps(Client.GameClient c) { var o = BagOverride; return o != null ? o.Cps(c) : c.Player.ConquerPoints; }
        public static bool TakeCps(Client.GameClient c, uint n)
        {
            if (n == 0) return true;
            var o = BagOverride; if (o != null) return o.TakeCps(c, n);
            if (c.Player.ConquerPoints < n) return false;
            c.Player.ConquerPoints -= n;
            return true;
        }
        public static string LuaItems(IEnumerable<HItem> items) { return "{" + string.Join(",", items.Select(i => "'" + i.Lua() + "'")) + "}"; }

        // ------------------------------------------------------------------ draws
        public sealed class Prize { public HItem Item; public int Weight; public bool Rare; }
        public static Prize Draw(IList<Prize> pool)
        {
            int total = pool.Sum(p => Math.Max(0, p.Weight));
            if (total <= 0) return null;
            int r = Roll(total);
            foreach (var p in pool) { int w = Math.Max(0, p.Weight); if (r < w) return p; r -= w; }
            return pool[pool.Count - 1];
        }

        // ------------------------------------------------------------------ config helpers
        public static string DbDir(string folder) { return Path.Combine(Program.ServerConfig.DbLocation, folder); }
        public static List<HItem> Items(object arr)
        {
            var l = new List<HItem>();
            var a = Festivals.FestivalLoader.Arr(arr);
            if (a == null) return l;
            foreach (var o in a)
            {
                if (o == null) continue;
                var d = o as Dictionary<string, object>;
                if (d != null)
                {
                    var b = Festivals.FestivalLoader.Bool(d, "bound");
                    l.Add(new HItem(Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "id")), (int)Math.Max(1, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(d, "count"))), b ?? true));
                }
                else l.Add(HItem.Parse(o.ToString()));
            }
            return l.Where(i => i.Id != 0).ToList();
        }
        public static int Int(Dictionary<string, object> d, string k, int def)
        {
            var v = Festivals.FestivalLoader.Val(d, k);
            if (v == null) return def;
            int r; return int.TryParse(v.ToString().Split('.')[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? r : def;
        }
        public static string Today { get { return Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture); } }
    }

    // ====================================================================== per-player persistent state
    /// <summary>[EventData 2026-10-15] Per-player festival progress (counters), kept in the unified EventStore (MySQL;
    /// section = folder[.file], values in its Counters). The old state.json is imported once and renamed *.migrated.
    /// Every change is in MySQL before the call returns (throws EventStore.EditFailed when it can't => grant nothing).</summary>
    public sealed class PlayerStore
    {
        readonly string _file;
        readonly EventData.LegacyBackend _be;
        readonly object _memLock = new object();
        readonly Dictionary<uint, Dictionary<string, long>> _mem = new Dictionary<uint, Dictionary<string, long>>();
        public PlayerStore(string folder, string file = "state.json")
        {
            _file = Path.Combine(HKit.DbDir(folder), file);
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyFor(folder, file, "state.json"), () => _file, EventData.LegacyFormat.JsonPlayersCounters);
        }
        /// <summary>Harness seam: keep state in memory only.</summary>
        public bool MemoryOnly;
        /// <summary>The OLD file (only read once by the migration).</summary>
        public string FilePath { get { return _file; } }
        public string EventKey { get { return _be.Event; } }

        public long Get(uint uid, string key)
        {
            if (MemoryOnly) lock (_memLock) { Dictionary<string, long> m; long v; return _mem.TryGetValue(uid, out m) && m.TryGetValue(key, out v) ? v : 0; }
            _be.EnsureMigrated();
            return EventData.EventStore.GetLong(uid, _be.Event, key);
        }
        /// <summary>Atomic read-modify-write; stored before this returns. Returns the new value.</summary>
        public long Update(uint uid, string key, Func<long, long> f)
        {
            return Tx(uid, m => { long v; m.TryGetValue(key, out v); long n = f(v); m[key] = n; return n; });
        }
        public long Set(uint uid, string key, long v) { return Update(uid, key, _ => v); }
        public long Add(uint uid, string key, long d) { return Update(uid, key, v => v + d); }
        /// <summary>Several changes in one save, all under the player's lock: f gets the live map (keys missing = 0).</summary>
        public T Tx<T>(uint uid, Func<Dictionary<string, long>, T> f)
        {
            if (MemoryOnly)
                lock (_memLock)
                {
                    Dictionary<string, long> m;
                    if (!_mem.TryGetValue(uid, out m)) _mem[uid] = m = new Dictionary<string, long>();
                    var r = f(m);
                    foreach (var k in m.Where(e => e.Value == 0).Select(e => e.Key).ToList()) m.Remove(k);
                    return r;
                }
            _be.EnsureMigrated();
            return EventData.EventStore.Edit(uid, _be.Event, s =>
            {
                var m = new Dictionary<string, long>(s.Counters);
                var r = f(m);
                s.Counters.Clear();
                foreach (var e in m) if (e.Value != 0) s.Counters[e.Key] = e.Value;
                return Tuple.Create(true, r);
            }, true);
        }
        /// <summary>Claim once: true the first time (saved), false afterwards.</summary>
        public bool Claim(uint uid, string key) { return Tx(uid, m => { long v; if (m.TryGetValue(key, out v) && v != 0) return false; m[key] = 1; return true; }); }
        public Dictionary<string, long> Snapshot(uint uid)
        {
            if (MemoryOnly) lock (_memLock) { Dictionary<string, long> m; return _mem.TryGetValue(uid, out m) ? new Dictionary<string, long>(m) : new Dictionary<string, long>(); }
            _be.EnsureMigrated();
            return new Dictionary<string, long>(EventData.EventStore.Section(uid, _be.Event).Counters);
        }
        public List<KeyValuePair<uint, long>> Top(string key, int n)
        {
            if (MemoryOnly) lock (_memLock) return _mem.Where(p => p.Value.ContainsKey(key)).Select(p => new KeyValuePair<uint, long>(p.Key, p.Value[key])).OrderByDescending(p => p.Value).Take(n).ToList();
            _be.EnsureMigrated();
            return EventData.EventStore.All(_be.Event).Where(p => p.Value.Counters.ContainsKey(key)).Select(p => new KeyValuePair<uint, long>(p.Key, p.Value.Counters[key])).OrderByDescending(p => p.Value).Take(n).ToList();
        }
    }
}
