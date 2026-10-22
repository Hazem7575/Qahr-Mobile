// ============================================================================
//  [SummerWhales 2026-10-15] Small per-character counter store for the festival mini-games of this agent's group
//  (SummerWhales / Summer2023 / BandsTrouble / Summer2025 / SportsMeet2024 / ArenaSports).
//  One JSON file per festival: { "<uid>": { "<counter>": <long>, ... }, ... } under Infinity[DB]\<Festival>\players.json.
//  Every change is written at once (temp file + replace) BEFORE the caller grants anything, so a crash, a relog or a
//  duplicate packet can never repeat a grant. All access is under one lock per store.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.SummerWhales
{
    /// <summary>[EventData 2026-10-15] Per-player counters of one festival, kept in the unified EventStore (MySQL;
    /// section = folder[.file]). The old players.json is imported once and renamed *.migrated.
    /// Set/Update are in MySQL before they return; they throw when they can't (the caller then grants nothing).</summary>
    public sealed class FestPlayerStore
    {
        readonly string _file;
        readonly object _sync = new object();
        readonly EventData.LegacyBackend _be;
        /// <summary>Tests may point every store at a copy of the data (only where the OLD file is looked for).</summary>
        public static string RootOverride;
        public FestPlayerStore(string folder, string fileName = "players.json")
        {
            _file = Path.Combine(RootOverride ?? Program.ServerConfig.DbLocation, folder, fileName);
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyFor(folder, fileName, "players.json"), () => _file, EventData.LegacyFormat.JsonRootCounters);
        }
        /// <summary>The OLD file (only read once by the migration).</summary>
        public string File_ { get { return _file; } }
        public object Sync { get { return _sync; } }
        public string EventKey { get { return _be.Event; } }
        public long Get(uint uid, string key) { _be.EnsureMigrated(); return EventData.EventStore.GetLong(uid, _be.Event, key); }
        /// <summary>Sets several counters of one character and stores them at once. Throws if MySQL can't be written
        /// (the caller then grants nothing).</summary>
        public void Set(uint uid, IDictionary<string, long> values)
        {
            _be.EnsureMigrated();
            lock (_sync)
                EventData.EventStore.Edit(uid, _be.Event, s => { foreach (var kv in values) { if (kv.Value == 0) s.Counters.Remove(kv.Key); else s.Counters[kv.Key] = kv.Value; } }, true);
        }
        public void Set(uint uid, string key, long value) { Set(uid, new Dictionary<string, long> { { key, value } }); }
        /// <summary>Atomically checks and updates: fn gets a getter and returns the new values (null = refuse).</summary>
        public bool Update(uint uid, Func<Func<string, long>, Dictionary<string, long>> fn)
        {
            _be.EnsureMigrated();
            lock (_sync)
                return EventData.EventStore.Edit(uid, _be.Event, s =>
                {
                    var next = fn(k => s.Counter(k));
                    if (next == null) return Tuple.Create(false, false);
                    foreach (var kv in next) { if (kv.Value == 0) s.Counters.Remove(kv.Key); else s.Counters[kv.Key] = kv.Value; }
                    return Tuple.Create(true, true);
                }, true);
        }
        public void ResetForTests() { _be.Recheck(); }
    }

    /// <summary>Item grants shared by the group's mini-games: bag space first, then the existing inventory methods.</summary>
    public static class FestGive
    {
        public sealed class GItem
        {
            public uint Id; public int Count = 1; public bool Bound; public uint LifeMinutes; public byte Bless;
            public override string ToString() { return Id + " 0 " + Count + " " + (Bound ? 3 : 0) + (LifeMinutes > 0 ? " " + LifeMinutes + " 1" : ""); }
        }

        /// <summary>Parses the client's item string "id flag count monopoly save_time active online data reduce_dmg ..."
        /// (luaui Public_AnalysisItemAttr). Returns null for a malformed one.</summary>
        public static GItem Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var p = s.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            uint id; if (p.Length < 1 || !uint.TryParse(p[0], out id) || id == 0) return null;
            Func<int, uint> at = i => { uint v; return p.Length > i && uint.TryParse(p[i], out v) ? v : 0; };
            return new GItem { Id = id, Count = (int)Math.Max(1, Math.Min(10000, at(2))), Bound = at(3) > 0, LifeMinutes = at(4), Bless = (byte)Math.Min(7, at(8)) };
        }

        public static bool Exists(uint id) { Database.ItemType.DBItem db; return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db); }

        public static int Slots(IEnumerable<GItem> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i.Id, out db)) { n += i.Count; continue; }
                int stack = Math.Max(1, (int)db.StackSize);
                if (i.LifeMinutes > 0 || i.Bless > 0 || stack <= 1) n += i.Count;
                else n += (i.Count + stack - 1) / stack;
            }
            return n;
        }

        public static long Count(Client.GameClient c, uint id)
        {
            long n = 0;
            foreach (var it in c.Inventory.ClientItems.Values.ToArray())
                if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1;
            return n;
        }

        /// <summary>Takes `count` units (bound first). Checks the whole amount first; false = nothing taken.</summary>
        public static bool Take(Client.GameClient c, uint id, long count, ServerSockets.Packet stream)
        {
            if (count <= 0 || count > 1000000) return false;
            var list = c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).OrderByDescending(i => i.Bound).ToList();
            long have = 0;
            foreach (var i in list) have += i.StackSize > 1 ? i.StackSize : 1;
            if (have < count) return false;
            long left = count;
            foreach (var i in list)
            {
                if (left == 0) break;
                long n = i.StackSize > 1 ? i.StackSize : 1;
                long take = Math.Min(n, left);
                c.Inventory.RemoveStackItem(i.UID, (ushort)take, stream);
                left -= take;
            }
            return left == 0;
        }

        /// <summary>Gives the items (the caller checked the space). Unknown ids are skipped and logged.</summary>
        public static void Give(Client.GameClient c, IEnumerable<GItem> items, ServerSockets.Packet stream, string why)
        {
            foreach (var z in items)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(z.Id, out db))
                {
                    try { MyConsole.WriteLine("[" + why + "] unknown item " + z.Id + " skipped", ConsoleColor.Yellow); } catch { }
                    continue;
                }
                int stack = Math.Max(1, (int)db.StackSize);
                if (z.LifeMinutes > 0)
                    for (int i = 0; i < z.Count; i++) c.Inventory.AddTime(stream, z.Id, 0, z.Bless, 0, z.Bound, z.LifeMinutes);
                else if (z.Bless > 0 || stack <= 1)
                    for (int i = 0; i < z.Count; i++) c.Inventory.Add(stream, z.Id, 1, 0, z.Bless, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, z.Bound);
                else
                {
                    int n = z.Count;
                    while (n > 0) { int k = Math.Min(n, stack); c.Inventory.AddItemWitchStack(z.Id, 0, (ushort)k, stream, z.Bound); n -= k; }
                }
            }
            try { Festivals.FestivalManager.Log("grant", why, c.Player.Name + "(" + c.Player.UID + ") " + string.Join(", ", items.Select(i => i.Id + "x" + i.Count + (i.Bound ? "(B)" : "")))); } catch { }
        }

        public static string ItemName(uint id) { try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); } }
    }
}
