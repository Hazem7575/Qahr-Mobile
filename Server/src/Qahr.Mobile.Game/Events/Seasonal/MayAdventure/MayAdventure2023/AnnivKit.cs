// ============================================================================
//  [MayAdventure2023 2026-10-15] AnnivKit: shared server pieces of the anniversary festival group
//  (MayAdventure2023, BleakValleyVillage, AnniversaryOtherworld2024, AnniversaryWindblownSand2025 / AnniversaryJubilee2025,
//  Anniversary2026). One place for:
//    - Prize          one reward line, official client item string "id 0 count monopoly" (monopoly 3 = bound)
//    - IBag           the bag operations a grant needs (count / take / space / give). ClientBag = the real inventory,
//                     the harness uses a fake bag, so every rule (deduct before grant, space first, limits) is tested
//                     against the real code path.
//    - Store          per-player key/value state: Infinity[DB]\<folder>\players.txt (uid TAB k=v;k=v), atomic save
//    - Throttle, crypto weighted pick (Qahr.Mobile.Lottery.LotteryCommon), festival log lines, config helpers.
//  Rule for every caller: hold the festival lock, check bag space FIRST, take the cost, Commit the state, then give.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.AnnivKit
{
    /// <summary>One reward line. Client string = "id 0 count monopoly" (Public_AnalysisItemAttr: id flag addamount monopoly).</summary>
    public sealed class Prize
    {
        public uint Item;
        public uint Count = 1;
        public bool Bound;
        public uint Weight = 1;
        public uint LifeMinutes;   // 0 = permanent; else a timed item (client field 5 "save_time" in minutes)
        public Prize() { }
        public Prize(uint item, uint count, bool bound, uint weight = 1) { Item = item; Count = Math.Max(1u, count); Bound = bound; Weight = weight; }
        /// <summary>"730004 0 1 3" -> +4 Stone x1 bound.</summary>
        public static Prize Parse(string s, uint weight = 1)
        {
            var p = (s ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            uint id, cnt = 1, mono = 0, life = 0;
            if (p.Length < 1 || !uint.TryParse(p[0], out id) || id == 0) return null;
            if (p.Length > 2) uint.TryParse(p[2], out cnt);
            if (p.Length > 3) uint.TryParse(p[3], out mono);
            if (p.Length > 4) uint.TryParse(p[4], out life);
            if (cnt == 0 || cnt > 10000 || life > 5256000) return null;
            return new Prize(id, cnt, mono == 3 || mono == 1, weight) { LifeMinutes = life };
        }
        public string ClientString() { return Item + " 0 " + Count + " " + (Bound ? 3 : 0) + (LifeMinutes > 0 ? " " + LifeMinutes + " 1" : ""); }
        public Prize Clone() { return new Prize(Item, Count, Bound, Weight) { LifeMinutes = LifeMinutes }; }
        public override string ToString() { return Item + "x" + Count + (Bound ? "(B)" : "") + (LifeMinutes > 0 ? "[" + LifeMinutes + "m]" : ""); }
    }

    /// <summary>The bag operations a festival grant uses.</summary>
    public interface IBag
    {
        long Count(uint item);
        /// <summary>Takes exactly n (false = nothing taken).</summary>
        bool Take(uint item, uint n);
        bool HasSpace(int slots);
        /// <summary>Adds the prize (space checked by the caller).</summary>
        bool Give(Prize p);
        long Cps { get; }
        bool TakeCps(uint n);
    }

    /// <summary>The real inventory of a player.</summary>
    public sealed class ClientBag : IBag
    {
        readonly Client.GameClient c;
        public ClientBag(Client.GameClient client) { c = client; }
        public long Count(uint item) { return Kit.Count(c, item); }
        public bool Take(uint item, uint n)
        {
            if (n == 0) return true;
            if (n > 1000000 || Kit.Count(c, item) < n) return false;
            return Lottery.LotteryCommon.TakeItem(c, item, n, true, true);
        }
        public bool HasSpace(int slots) { return slots <= 0 || (slots <= 40 && c.Inventory.HaveSpace((byte)slots)); }
        public bool Give(Prize p)
        {
            if (p == null || !Kit.Exists(p.Item)) return false;
            Database.ItemType.DBItem db; Pool.ItemsBase.TryGetValue(p.Item, out db);
            int stack = Math.Max(1, (int)db.StackSize);
            bool ok = true;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                uint left = p.Count;
                if (p.LifeMinutes > 0)
                    for (uint i = 0; i < p.Count; i++) ok &= c.Inventory.AddTime(s, p.Item, 0, 0, 0, p.Bound, p.LifeMinutes);
                else if (stack <= 1)
                    while (left > 0) { byte k = (byte)Math.Min(left, 40u); ok &= c.Inventory.Add(s, p.Item, k, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, p.Bound); left -= k; }
                else
                    while (left > 0) { uint k = Math.Min(left, (uint)stack); ok &= c.Inventory.AddItemWitchStack(p.Item, 0, (ushort)k, s, p.Bound); left -= k; }
            }
            return ok;
        }
        public long Cps { get { return c.Player.ConquerPoints; } }
        public bool TakeCps(uint n) { return n == 0 || Lottery.LotteryCommon.TakeCps(c, n); }
    }

    public static class Kit
    {
        // ------------------------------------------------------------------ messages / lua
        public static void Msg(Client.GameClient c, string t)
        {
            try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        public static void Lua(Client.GameClient c, string call, uint msg) { LuaUi.LuaPush.Send(c, call, msg); }
        public static string Q(string s) { return LuaUi.LuaPush.Q(s); }

        // ------------------------------------------------------------------ throttle (per uid + action key)
        static readonly ConcurrentDictionary<string, long> _last = new ConcurrentDictionary<string, long>();
        /// <summary>True = too fast (ignore the request).</summary>
        public static bool Throttled(string key, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            string k = key + "|" + uid;
            if (_last.TryGetValue(k, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[k] = now;
            return false;
        }
        public static void ResetThrottle() { _last.Clear(); }

        // ------------------------------------------------------------------ per-uid locks
        static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();
        public static object Lock(string festival, uint uid) { return _locks.GetOrAdd(festival + "|" + uid, k => new object()); }

        // ------------------------------------------------------------------ time
        public static Func<DateTime> Now = () => DateTime.Now;
        public static string Day { get { return Now().ToString("yyyyMMdd", CultureInfo.InvariantCulture); } }

        // ------------------------------------------------------------------ items
        public static long Count(Client.GameClient c, uint id)
        {
            long n = 0;
            try { foreach (var i in c.Inventory.ClientItems.Values.ToArray()) if (i.ITEM_ID == id) n += i.StackSize > 1 ? i.StackSize : 1; } catch { }
            return n;
        }
        public static Func<uint, string> NameHook;
        public static string ItemName(uint id)
        {
            var h = NameHook; if (h != null) return h(id);
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && !string.IsNullOrEmpty(db.Name) ? db.Name.Replace('~', ' ') : id.ToString();
        }
        public static Func<uint, bool> ExistsHook;
        public static bool Exists(uint id)
        {
            var h = ExistsHook; if (h != null) return h(id);
            Database.ItemType.DBItem db; return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db);
        }
        public static Func<uint, int> StackHook;
        public static int Stack(uint id)
        {
            var h = StackHook; if (h != null) return Math.Max(1, h(id));
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) ? Math.Max(1, (int)db.StackSize) : 1;
        }
        public static int Slots(IEnumerable<Prize> ps)
        {
            int n = 0;
            foreach (var p in ps)
            {
                if (p == null) continue;
                int st = Stack(p.Item);
                n += st <= 1 || p.LifeMinutes > 0 ? (int)p.Count : (int)((p.Count + st - 1) / st);
            }
            return n;
        }
        /// <summary>Worst-case slots of one pick from a pool.</summary>
        public static int WorstSlots(IEnumerable<Prize> pool)
        {
            int w = 0;
            foreach (var p in pool) w = Math.Max(w, Slots(new[] { p }));
            return Math.Max(1, w);
        }
        public static string Names(IEnumerable<Prize> ps)
        {
            return string.Join(", ", ps.Where(p => p != null).Select(p => (p.Count > 1 ? p.Count + " " : "") + ItemName(p.Item) + (p.Bound ? " (B)" : "")));
        }

        /// <summary>Weighted pick with the crypto RNG (weights = the derived odds of a config file).</summary>
        public static Func<IList<long>, int> PickHook;
        public static int Pick(IList<Prize> pool)
        {
            if (pool == null || pool.Count == 0) return -1;
            var w = pool.Select(g => (long)g.Weight).ToList();
            if (w.Sum() <= 0) return -1;
            var h = PickHook; if (h != null) return h(w);
            return Lottery.LotteryCommon.PickWeighted(w);
        }
        public static Prize PickOne(IList<Prize> pool)
        {
            int i = Pick(pool);
            return i < 0 ? null : pool[i].Clone();
        }

        public static void Log(string festival, string what, Client.GameClient c, string detail)
        {
            try { Festivals.FestivalManager.Log(what, festival, (c != null && c.Player != null ? c.Player.Name + "(" + c.Player.UID + ") " : "") + detail); } catch { }
        }

        public static Func<string, bool> RunningHook;
        public static bool Running(string festival)
        {
            var h = RunningHook; if (h != null) return h(festival);
            try { return Festivals.FestivalManager.IsRunning(festival); } catch { return false; }
        }
        /// <summary>The run number of a running festival (event-scoped state resets when it changes); 0 = not running.</summary>
        public static Func<string, int> RunHook;
        public static int Run(string festival)
        {
            var h = RunHook; if (h != null) return h(festival);
            try { var r = Festivals.FestivalManager.Running(festival); return r == null ? 0 : Math.Max(1, r.Run); } catch { return 0; }
        }

        // ------------------------------------------------------------------ config helpers
        public static List<Prize> ReadPool(object arr)
        {
            var list = new List<Prize>();
            var a = FL.Arr(arr);
            if (a == null) return list;
            foreach (var o in a)
            {
                var d = FL.Obj(o);
                Prize g = d != null ? Prize.Parse(FL.Str(d, "item"), FL.Val(d, "weight") == null ? 1u : FL.U(FL.Val(d, "weight"))) : Prize.Parse(o == null ? null : o.ToString());
                if (g != null) list.Add(g);
            }
            return list;
        }
        public static int Int(Dictionary<string, object> d, string k, int def)
        {
            var v = FL.Val(d, k); if (v == null) return def;
            long n; return long.TryParse(v.ToString().Split('.')[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, n)) : def;
        }
        /// <summary>Reads a JSON config file when it changed (size + mtime), else returns the cached object.</summary>
        public sealed class CfgCache<T> where T : class
        {
            readonly string _rel; readonly Func<Dictionary<string, object>, T> _read; readonly Func<T> _def;
            T _cfg; string _stamp;
            public CfgCache(string relPath, Func<Dictionary<string, object>, T> read, Func<T> def) { _rel = relPath; _read = read; _def = def; }
            public string File { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", _rel); } }
            public T Get()
            {
                try
                {
                    var fi = new FileInfo(File);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    T c = fi.Exists ? _read(FL.ReadJson(fi.FullName)) : _def();
                    _cfg = c ?? _def(); _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[AnnivKit] " + _rel + ": " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = _def();
                }
                return _cfg;
            }
        }
    }

    /// <summary>[EventData 2026-10-15] Per-player key/value state of one festival, kept in the unified EventStore (MySQL;
    /// section = folder[.file], values in its Strings). The old Infinity[DB]\&lt;folder&gt;\&lt;file&gt; is imported once and
    /// renamed *.migrated. Set marks the value dirty; Commit writes this store's changed players to MySQL (throws on failure).</summary>
    public sealed class Store
    {
        readonly string _folder, _file;
        readonly object _sync = new object();
        readonly EventData.LegacyBackend _be;
        readonly HashSet<uint> _touched = new HashSet<uint>();

        public Store(string folder, string file = "players.txt")
        {
            _folder = folder; _file = file;
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyFor(folder, file, "players.txt"), () => FilePath, EventData.LegacyFormat.KvTextSkip0);
        }
        public object Sync { get { return _sync; } }
        /// <summary>The OLD file (only read once by the migration).</summary>
        public string FilePath { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", _folder, _file); } }
        public string EventKey { get { return _be.Event; } }

        public string Get(uint uid, string k, string def = "") { _be.EnsureMigrated(); return EventData.EventStore.GetString(uid, _be.Event, k, def); }
        public long GetL(uint uid, string k, long def = 0) { long v; return long.TryParse(Get(uid, k, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def; }
        public int GetI(uint uid, string k, int def = 0) { return (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, GetL(uid, k, def))); }
        public void Set(uint uid, string k, string v)
        {
            v = (v ?? "").Replace(";", ",").Replace("=", ":").Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
            lock (_sync)
            {
                _be.EnsureMigrated();
                EventData.EventStore.SetString(uid, _be.Event, k, v);
                _touched.Add(uid);
            }
        }
        public void Set(uint uid, string k, long v) { Set(uid, k, v.ToString(CultureInfo.InvariantCulture)); }
        public long Add(uint uid, string k, long d) { lock (_sync) { long v = GetL(uid, k) + d; Set(uid, k, v); return v; } }
        /// <summary>A value that resets every day: returns 0 when the stored day is not today.</summary>
        public long GetDaily(uint uid, string k) { lock (_sync) return Get(uid, k + "@d") == Kit.Day ? GetL(uid, k) : 0; }
        public void SetDaily(uint uid, string k, long v) { lock (_sync) { Set(uid, k + "@d", Kit.Day); Set(uid, k, v); } }
        public IEnumerable<KeyValuePair<uint, long>> All(string k)
        {
            _be.EnsureMigrated();
            var r = new List<KeyValuePair<uint, long>>();
            foreach (var kv in EventData.EventStore.All(_be.Event)) { string s; long v; if (kv.Value.Strings.TryGetValue(k, out s) && long.TryParse(s, out v)) r.Add(new KeyValuePair<uint, long>(kv.Key, v)); }
            return r;
        }
        /// <summary>Writes the players changed through this store to MySQL now. Throws when that fails (grant nothing).</summary>
        public void Commit()
        {
            lock (_sync)
            {
                foreach (var uid in _touched.ToList())
                {
                    if (!EventData.EventStore.Flush(uid)) throw new IOException("EventData: MySQL write failed for uid " + uid + " (" + _be.Event + ")");
                    _touched.Remove(uid);
                }
            }
        }
        public void ResetForTests() { lock (_sync) { _touched.Clear(); _be.Recheck(); } }
    }
}
