// ============================================================================
//  [NewYear2023 2026-10-15] Shared helpers of the Spring Festival group (LunarNewYear2023, SpringFestival2024,
//  WhiteSnakeLegend, SpringFestival2026):
//    SpringStore  per-character counters, one JSON file per festival (Data\<Folder>\players.json). Every change is
//                 written at once (temp file + replace) BEFORE the caller grants anything: a crash, a relog or a duplicate
//                 packet can never repeat a grant. All access is under one lock per store.
//    SpringGive   item grants: bag space first, then the existing inventory methods (logged to festivals_log.txt).
//    SpringWorld  dynamic NPCs (official shop ids that the festival npcs.txt can't hold) and personal monsters.
//  Modelled on Base\SummerWhales\FestPlayerStore.cs (same format), own copy so the folders stay independent.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.NewYear2023
{
    /// <summary>[EventData 2026-10-15] Per-player counters of one festival, kept in the unified EventStore (MySQL;
    /// section = folder[.file]). The old players.json is imported once and renamed *.migrated.
    /// Update/Set/Roll are in MySQL before they return; they throw when they can't (the caller then grants nothing).</summary>
    public sealed class SpringStore
    {
        readonly string _folder, _fileName;
        readonly object _sync = new object();
        readonly EventData.LegacyBackend _be;

        /// <summary>Tests point every store at a copy of the data (only where the OLD file is looked for).</summary>
        public static string RootOverride;

        public SpringStore(string folder, string fileName = "players.json")
        {
            _folder = folder; _fileName = fileName;
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyFor(folder, fileName, "players.json"), () => FilePath, EventData.LegacyFormat.JsonRootCounters);
        }
        string FilePath { get { return Path.Combine(RootOverride ?? global::Qahr.Mobile.Festivals.FestivalLoader.Root, _folder, _fileName); } }
        public object Sync { get { return _sync; } }
        public string EventKey { get { return _be.Event; } }

        public long Get(uint uid, string key) { _be.EnsureMigrated(); return EventData.EventStore.GetLong(uid, _be.Event, key); }

        /// <summary>Atomically checks and updates: fn gets a getter and returns the new values (null = refuse).
        /// Stored before it returns true. Throws if MySQL can't be written (the caller then grants nothing).</summary>
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
        public void Set(uint uid, string key, long value) { Update(uid, g => new Dictionary<string, long> { { key, value } }); }

        /// <summary>Season / day housekeeping: removes every key of this character that starts with one of the prefixes
        /// when the stored marker differs (a new festival season or a new day). Returns true when something was reset.</summary>
        public bool Roll(uint uid, string marker, long value, params string[] prefixes)
        {
            _be.EnsureMigrated();
            lock (_sync)
                return EventData.EventStore.Edit(uid, _be.Event, s =>
                {
                    long cur;
                    if (s.Counters.TryGetValue(marker, out cur) && cur == value) return Tuple.Create(false, false);
                    foreach (var k in s.Counters.Keys.ToList())
                        if (k != marker && prefixes.Any(p => k.StartsWith(p, StringComparison.Ordinal))) s.Counters.Remove(k);
                    if (value == 0) s.Counters.Remove(marker); else s.Counters[marker] = value;
                    return Tuple.Create(true, true);
                }, true);
        }

        public void ResetForTests() { _be.Recheck(); }
    }

    /// <summary>Item grants: bag space first, then the existing inventory methods.</summary>
    public static class SpringGive
    {
        public sealed class GItem
        {
            public uint Id; public int Count = 1; public bool Bound; public uint LifeMinutes; public byte Bless;
            /// <summary>The client's item string (Public_AnalysisItemAttr) for the result windows.</summary>
            public override string ToString() { return Id + " 0 " + Count + " " + (Bound ? 3 : 0) + (LifeMinutes > 0 ? " " + LifeMinutes + " 1" : "") + (Bless > 0 ? (LifeMinutes > 0 ? "" : " 0 0") + " 0 0 " + Bless : ""); }
        }

        /// <summary>Parses the client's item string "id flag count monopoly save_time active online data bless ...".</summary>
        public static GItem Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var p = s.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            uint id; if (p.Length < 1 || !uint.TryParse(p[0], out id) || id == 0) return null;
            Func<int, uint> at = i => { uint v; return p.Length > i && uint.TryParse(p[i], out v) ? v : 0; };
            return new GItem { Id = id, Count = (int)Math.Max(1, Math.Min(10000, at(2))), Bound = at(3) > 0, LifeMinutes = at(4), Bless = (byte)Math.Min(7, at(8)) };
        }
        public static GItem I(uint id, int count = 1, bool bound = false, byte bless = 0) { return new GItem { Id = id, Count = count, Bound = bound, Bless = bless }; }

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
        public static bool HaveSpace(Client.GameClient c, int slots) { return slots <= 0 || (slots <= 40 && c.Inventory.HaveSpace((byte)slots)); }

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

        /// <summary>Takes exactly this item object (one unit of its stack).</summary>
        public static bool TakeOne(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            MsgGameItem cur;
            if (item == null || !c.Inventory.TryGetItem(item.UID, out cur) || cur.ITEM_ID != item.ITEM_ID) return false;
            c.Inventory.RemoveStackItem(cur.UID, 1, stream);
            return true;
        }

        /// <summary>Gives the items (the caller checked the space). Unknown ids are skipped and logged.</summary>
        public static void Give(Client.GameClient c, IEnumerable<GItem> items, ServerSockets.Packet stream, string why)
        {
            var list = items.ToList();
            foreach (var z in list)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(z.Id, out db))
                {
                    try { MyConsole.WriteLine("[" + why + "] unknown item " + z.Id + " skipped", ConsoleColor.Yellow); } catch { }
                    continue;
                }
                int stack = Math.Max(1, (int)db.StackSize);
                if (z.LifeMinutes > 0)
                    for (int i = 0; i < z.Count; i++) c.Inventory.AddTime(stream, z.Id, 0, z.Bless, 0, z.Bound, z.LifeMinutes * 60);// AddTime takes SECONDS (Inventory.AddTime: TimeStamp = Now.AddSeconds)
                else if (z.Bless > 0 || stack <= 1)
                    for (int i = 0; i < z.Count; i++) c.Inventory.Add(stream, z.Id, 1, 0, z.Bless, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, z.Bound);
                else
                {
                    int n = z.Count;
                    while (n > 0) { int k = Math.Min(n, stack); c.Inventory.AddItemWitchStack(z.Id, 0, (ushort)k, stream, z.Bound); n -= k; }
                }
            }
            Log(why, c.Player.Name + "(" + c.Player.UID + ") " + string.Join(", ", list.Select(i => i.Id + "x" + i.Count + (i.Bound ? "(B)" : ""))));
        }

        public static void BoundCps(Client.GameClient c, int amount, string why)
        {
            if (amount <= 0) return;
            c.Player.BoundConquerPoints += amount;
            Log(why, c.Player.Name + "(" + c.Player.UID + ") " + amount + " CPs (B)");
        }

        public static void Log(string why, string text) { try { Festivals.FestivalManager.Log("grant", why, text); } catch { } }
        public static string ItemName(uint id) { try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); } }
    }

    /// <summary>Random numbers, throttles, messages.</summary>
    public static class SpringKit
    {
        static readonly Random _rng = new Random();
        static readonly object _rngLock = new object();
        /// <summary>Tests replace the dice (returns a value in [0,1)).</summary>
        public static Func<double> DiceOverride;
        public static double Dice() { var o = DiceOverride; if (o != null) return o(); lock (_rngLock) return _rng.NextDouble(); }
        public static int Rand(int lo, int hi) { if (hi <= lo) return lo; return lo + (int)Math.Min(hi - lo, Math.Floor(Dice() * (hi - lo + 1))); }
        public static int Pick(IList<long> w)
        {
            long total = 0; foreach (var x in w) total += Math.Max(0, x);
            if (total <= 0) return -1;
            double r = Dice() * total;
            for (int i = 0; i < w.Count; i++) { long x = Math.Max(0, w[i]); if (r < x) return i; r -= x; }
            return w.Count - 1;
        }
        public static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        public static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        public static void Box(Client.GameClient c, ServerSockets.Packet stream, string text)
        {
            try
            {
                var d = new Game.MsgNpc.Dialog(c, stream);
                d.AddText(text).AddOption("OK.", 255).AddAvatar(0).FinalizeDialog();
            }
            catch { Msg(c, text); }
        }
        public static string Q(string s) { return "'" + (s ?? "").Replace("\\", "").Replace("'", "") + "'"; }
        public static int Today { get { var n = Now; return n.Year * 10000 + n.Month * 100 + n.Day; } }
        public static Func<DateTime> ClockOverride;
        public static DateTime Now { get { var o = ClockOverride; return o != null ? o() : DateTime.Now; } }
    }

    /// <summary>Dynamic NPCs and personal monsters of the group's festivals.</summary>
    public static class SpringWorld
    {
        // ------------------------------------------------------------------ dynamic NPCs
        static readonly ConcurrentDictionary<string, Game.MsgNpc.Npc> _npcs = new ConcurrentDictionary<string, Game.MsgNpc.Npc>();
        static string NpcKey(uint uid, uint map) { return uid + "@" + map; }

        /// <summary>Places the NPC once (the same id on the same map is never placed twice). Never throws.</summary>
        public static bool PlaceNpc(uint uid, Role.Flags.NpcType type, uint mesh, uint mapId, ushort x, ushort y)
        {
            try
            {
                string k = NpcKey(uid, mapId);
                if (_npcs.ContainsKey(k)) return true;
                var map = Pool.ServerMaps != null ? Pool.ServerMaps[mapId] : null;
                if (map == null) return false;
                if (map.View.MapContain(Role.MapObjectType.Npc, uid, p => p.DynamicID == 0)) return false;// someone else's NPC
                var np = Game.MsgNpc.Npc.Create();
                np.UID = uid; np.NpcType = type; np.Mesh = mesh; np.Map = mapId; np.X = x; np.Y = y;
                map.AddNpc(np);
                _npcs[k] = np;
                return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); return false; }
        }
        public static bool HasNpc(uint uid, uint mapId) { return _npcs.ContainsKey(NpcKey(uid, mapId)); }
        public static void RemoveNpc(uint uid, uint mapId)
        {
            Game.MsgNpc.Npc np;
            if (!_npcs.TryRemove(NpcKey(uid, mapId), out np) || np == null) return;
            try
            {
                var map = HotReload.HotReloadManager.LoadedMap(mapId);
                if (map == null) return;
                ushort x = np.X, y = np.Y;
                using (var rec = new ServerSockets.RecycledPacket())
                    map.RemoveNpc(np, rec.GetStream());
                try { new HotReload.MapCells().RestoreAround(map, x, y); } catch { }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        // ------------------------------------------------------------------ personal monsters
        public sealed class MobRec { public uint Owner; public string Tag; public Game.MsgMonster.MonsterRole Mob; public DateTime Expires; }
        static readonly ConcurrentDictionary<uint, MobRec> _mobs = new ConcurrentDictionary<uint, MobRec>();

        /// <summary>Spawns one monster of the type (registered, e.g. a running festival's monsters\*.ini) in the box.
        /// RemoveOnDead: it never respawns. Returns null when the type / map / box can't take it.</summary>
        public static Game.MsgMonster.MonsterRole Spawn(uint owner, string tag, uint typeId, uint mapId, ushort x, ushort y, ushort range, int minutes)
        {
            try
            {
                Game.MsgMonster.MonsterFamily fam;
                if (Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(typeId, out fam) || fam == null) return null;
                var map = Pool.ServerMaps != null ? Pool.ServerMaps[mapId] : null;
                if (map == null) return null;
                if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                if (!map.MonstersColletion.ReadMap()) return null;
                var copy = fam.Copy();
                copy.SpawnX = x; copy.SpawnY = y;
                copy.MaxSpawnX = (ushort)(x + Math.Max((ushort)1, range)); copy.MaxSpawnY = (ushort)(y + Math.Max((ushort)1, range));
                copy.MapID = map.ID; copy.SpawnCount = 1;
                Game.MsgMonster.MonsterRole mob = null;
                for (int tries = 0; tries < 4 && mob == null; tries++)
                    mob = map.MonstersColletion.Add(copy, true, 0, true);
                if (mob == null) return null;
                mob.RemoveOnDead = true;
                _mobs[mob.UID] = new MobRec { Owner = owner, Tag = tag, Mob = mob, Expires = SpringKit.Now.AddMinutes(Math.Max(1, minutes)) };
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                        mob.Send(mob.GetArray(stream, false));
                    }
                }
                catch { }
                return mob;
            }
            catch (Exception e) { MyConsole.SaveException(e); return null; }
        }

        /// <summary>MonsterRole.Dead: our personal monster (forgotten now), or null.</summary>
        public static MobRec Consume(Game.MsgMonster.MonsterRole mob)
        {
            MobRec r;
            return mob != null && _mobs.TryRemove(mob.UID, out r) ? r : null;
        }
        public static int Live(uint owner, string tag)
        {
            int n = 0;
            foreach (var r in _mobs.Values) if (r.Owner == owner && r.Tag == tag && r.Mob != null && r.Mob.Alive) n++;
            return n;
        }
        public static void DespawnAll(Func<MobRec, bool> which)
        {
            foreach (var kv in _mobs.ToArray())
            {
                if (!which(kv.Value)) continue;
                MobRec r;
                if (_mobs.TryRemove(kv.Key, out r)) Despawn(r.Mob);
            }
        }
        /// <summary>Removes the expired ones (timer).</summary>
        public static void Sweep()
        {
            var now = SpringKit.Now;
            DespawnAll(r => r.Expires < now || r.Mob == null || (!r.Mob.Alive && r.Mob.RemoveOnDead));
        }
        static void Despawn(Game.MsgMonster.MonsterRole mob)
        {
            if (mob == null) return;
            try
            {
                mob.RemoveOnDead = true;
                bool was = mob.Alive;
                mob.HitPoints = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                    mob.Send(rec.GetStream().ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                var gm = mob.GMap;
                if (gm != null)
                {
                    gm.View.LeaveMap<Role.IMapObj>(mob);
                    if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false);
                }
            }
            catch { }
        }
    }
}
