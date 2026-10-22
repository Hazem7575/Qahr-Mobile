// ============================================================================
//  [QixiRomance2022 2026-10-15] QixiKit: shared server pieces of the five Qixi festivals of this group
//  (QixiRomance2022, QixiQingqiu2023, QixiInterplane2024, Qixi2025, SwordSaga). One place for:
//    - the per-player state file   Infinity[DB]\<Key>\players.txt   (uid TAB key=value;key=value, atomic save)
//    - item helpers (count / take / bag space / grant, timed + bound + blessed items, official "id 0 n mono mins act" strings)
//    - crypto RNG weighted picks (Qahr.Mobile.Lottery.LotteryCommon), per-player throttle, festival log lines.
//  Every grant: caller holds the festival lock, checks bag space FIRST, deducts, records the state, then grants.
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

namespace Qahr.Mobile.QixiKit
{
    /// <summary>One item of a reward: the official client item string is "id flag count monopoly saveMinutes active".</summary>
    public sealed class Grant
    {
        public uint Item;
        public uint Count = 1;
        public bool Bound;
        public uint LifeMinutes;   // 0 = permanent
        public byte Bless;
        public uint Weight = 1;    // for weighted pools (derived)
        public string Raw;         // the official string as written in the client data (kept for string-equality UIs)

        public Grant() { }
        public Grant(uint item, uint count, bool bound = false, uint life = 0, byte bless = 0) { Item = item; Count = Math.Max(1u, count); Bound = bound; LifeMinutes = life; Bless = bless; }

        /// <summary>"730003 0 1 3 7200 1" -> 730003 x1 bound, 7200 min. Monopoly 3 = bound ("(赠)" in the client comments).</summary>
        public static Grant Parse(string s, uint weight = 1)
        {
            var p = (s ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length < 1) return null;
            uint id, cnt = 1, mono = 0, life = 0;
            if (!uint.TryParse(p[0], out id) || id == 0) return null;
            if (p.Length > 2) uint.TryParse(p[2], out cnt);
            if (p.Length > 3) uint.TryParse(p[3], out mono);
            if (p.Length > 4) uint.TryParse(p[4], out life);
            return new Grant { Item = id, Count = Math.Max(1u, cnt), Bound = mono == 3 || mono == 1, LifeMinutes = life, Weight = weight, Raw = string.Join(" ", p) };
        }
        /// <summary>The client item string (DlgRewardTen / Public_AnalysisItemAttr).</summary>
        public string ClientString()
        {
            if (!string.IsNullOrEmpty(Raw)) return Raw;
            return Item + " 0 " + Count + " " + (Bound ? 3 : 0) + (LifeMinutes > 0 ? " " + LifeMinutes + " 1" : "");
        }
        public override string ToString() { return Item + "x" + Count + (Bound ? "(B)" : "") + (LifeMinutes > 0 ? "[" + LifeMinutes + "m]" : ""); }
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

        // ------------------------------------------------------------------ throttle (per uid + action slot)
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

        // ------------------------------------------------------------------ time
        public static Func<DateTime> Now = () => DateTime.Now;
        public static string Day { get { return Now().ToString("yyyyMMdd", CultureInfo.InvariantCulture); } }

        // ------------------------------------------------------------------ items
        public static uint Count(Client.GameClient c, uint id)
        {
            ulong n = 0;
            foreach (var i in c.Inventory.ClientItems.Values.ToArray())
                if (i.ITEM_ID == id) n += i.StackSize > 1 ? i.StackSize : 1u;
            return (uint)Math.Min(n, uint.MaxValue);
        }
        /// <summary>Takes `count` (bound first). Checks the whole amount first; false = nothing taken.</summary>
        public static bool Take(Client.GameClient c, uint id, uint count)
        {
            if (count == 0) return true;
            if (count > 1000000 || Count(c, id) < count) return false;
            return Lottery.LotteryCommon.TakeItem(c, id, count, true, true);
        }
        public static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && !string.IsNullOrEmpty(db.Name) ? db.Name.Replace('~', ' ') : id.ToString();
        }
        public static bool Exists(uint id) { Database.ItemType.DBItem db; return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db); }

        public static int Slots(IEnumerable<Grant> gs)
        {
            int n = 0;
            foreach (var g in gs)
            {
                if (g == null) continue;
                Database.ItemType.DBItem db;
                int stack = Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(g.Item, out db) ? Math.Max(1, (int)db.StackSize) : 1;
                if (g.LifeMinutes > 0 || g.Bless > 0 || stack <= 1) n += (int)g.Count;
                else n += (int)((g.Count + stack - 1) / stack);
            }
            return n;
        }
        public static bool HasSpace(Client.GameClient c, IEnumerable<Grant> gs)
        {
            int n = Slots(gs);
            return n <= 0 || (n <= 40 && c.Inventory.HaveSpace((byte)n));
        }

        /// <summary>Adds the items (space must be checked by the caller). Logs every line.</summary>
        public static void Give(Client.GameClient c, IEnumerable<Grant> gs, string festival, string source)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                foreach (var g in gs)
                {
                    if (g == null || !Exists(g.Item)) continue;
                    Database.ItemType.DBItem db; Pool.ItemsBase.TryGetValue(g.Item, out db);
                    int stack = Math.Max(1, (int)db.StackSize);
                    if (g.LifeMinutes > 0)
                        for (int i = 0; i < g.Count; i++) c.Inventory.AddTime(s, g.Item, 0, g.Bless, 0, g.Bound, g.LifeMinutes);
                    else if (g.Bless > 0 || stack <= 1)
                    {
                        uint left = g.Count;
                        while (left > 0) { byte k = (byte)Math.Min(left, 40u); c.Inventory.Add(s, g.Item, k, 0, g.Bless, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, g.Bound); left -= k; }
                    }
                    else
                    {
                        uint left = g.Count;
                        while (left > 0) { uint k = Math.Min(left, (uint)stack); c.Inventory.AddItemWitchStack(g.Item, 0, (ushort)k, s, g.Bound); left -= k; }
                    }
                }
            }
            Log(festival, "grant", c, source + " -> " + string.Join(", ", gs.Where(g => g != null).Select(g => g.ToString())));
        }

        /// <summary>Weighted pick with the crypto RNG (weights are the derived odds of a config file).</summary>
        public static int Pick(IList<Grant> pool)
        {
            if (pool == null || pool.Count == 0) return -1;
            return Lottery.LotteryCommon.PickWeighted(pool.Select(g => (long)g.Weight).ToList());
        }
        public static int PickW(IList<long> w) { return Lottery.LotteryCommon.PickWeighted(w); }
        public static ulong Below(ulong n) { return Lottery.LotteryCommon.Below(n); }

        public static void Log(string festival, string what, Client.GameClient c, string detail)
        {
            try { Festivals.FestivalManager.Log(what, festival, (c != null && c.Player != null ? c.Player.Name + "(" + c.Player.UID + ") " : "") + detail); } catch { }
        }

        // ------------------------------------------------------------------ config helpers
        public static List<Grant> ReadPool(object arr)
        {
            var list = new List<Grant>();
            var a = FL.Arr(arr);
            if (a == null) return list;
            foreach (var o in a)
            {
                var d = FL.Obj(o);
                if (d != null)
                {
                    var g = Grant.Parse(FL.Str(d, "item"), Math.Max(0u, FL.U(FL.Val(d, "weight") ?? 1)));
                    if (g != null) { var bl = FL.Val(d, "bless"); if (bl != null) g.Bless = (byte)Math.Min(7, FL.U(bl)); list.Add(g); }
                }
                else { var g = Grant.Parse(o == null ? null : o.ToString()); if (g != null) list.Add(g); }
            }
            return list;
        }
        public static int Int(Dictionary<string, object> d, string k, int def)
        {
            var v = FL.Val(d, k); if (v == null) return def;
            long n; return long.TryParse(v.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, n)) : def;
        }
        public static List<int> Ints(Dictionary<string, object> d, string k, List<int> def)
        {
            var a = FL.Arr(FL.Val(d, k)); if (a == null) return def;
            var r = new List<int>();
            foreach (var o in a) { int n; if (o != null && int.TryParse(o.ToString(), out n)) r.Add(n); }
            return r.Count > 0 ? r : def;
        }
    }

    /// <summary>[EventData 2026-10-15] Per-player key/value state of one festival, kept in the unified EventStore (MySQL;
    /// section = folder[.file], values in its Strings). The old Infinity[DB]\&lt;folder&gt;\&lt;file&gt; is imported once and
    /// renamed *.migrated. Set marks the value dirty; Commit writes this store's changed players to MySQL (throws on failure).</summary>
    public sealed class PlayerStore
    {
        readonly string _folder, _file;
        readonly object _sync = new object();
        readonly EventData.LegacyBackend _be;
        readonly HashSet<uint> _touched = new HashSet<uint>();

        public PlayerStore(string folder, string file = "players.txt")
        {
            _folder = folder; _file = file;
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyFor(folder, file, "players.txt"), () => FilePath, EventData.LegacyFormat.KvText);
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
