// ============================================================================
//  [AprilFools2023 2026-10-15] FoolsKit: shared server pieces of the April Fools group
//  (AprilFools2023 Tricky Wit, AliceRescue Wonderland, AprilFools2025 Fool`s Paradise, AprilFools2026 Astral Dream).
//    - Store   : per-player state file Data\<Key>\players.txt (uid TAB key=value;key=value, atomic save)
//    - Items   : count / take / bag space / grant (AddItemWitchStack, logs), weighted picks
//    - Arena   : private DynamicID copies of a map + personal monsters + "all dead" callbacks (no loot: drop_* = 0 in the inis)
//    - Hooks   : OnTick (PoolProcesses), OnDialog (Procesor, virtual dialog ids), OnUse (MsgItemUsuagePacket.UseItem)
//  Every grant: the caller holds the player lock, checks bag space FIRST, deducts, records the state, then grants.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.FoolsKit
{
    public sealed class Reward
    {
        public uint Item, Count = 1;
        public bool Bound;
        public uint Weight = 1;
        public uint Minutes;       // > 0 = a timed item (the official "id 0 n 3 minutes 1" strings)
        public Reward() { }
        public Reward(uint item, uint count, bool bound = false, uint weight = 1) { Item = item; Count = Math.Max(1u, count); Bound = bound; Weight = Math.Max(1u, weight); }
        public static Reward Timed(uint item, uint count, bool bound, uint minutes) { return new Reward(item, count, bound) { Minutes = minutes }; }
        public override string ToString() { return Item + "x" + Count + (Bound ? "(B)" : "") + (Minutes > 0 ? "[" + Minutes + "m]" : ""); }
    }

    public static class Kit
    {
        // ------------------------------------------------------------------ messages / lua
        /// <summary>Test seam (null on the server): every system message of the group.</summary>
        public static Action<Client.GameClient, string> MsgOverride;
        public static void Msg(Client.GameClient c, string t)
        {
            var o = MsgOverride; if (o != null) o(c, t);
            try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        public static void Center(Client.GameClient c, string t)
        {
            try { c.SendSysMesage(t, MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow); } catch { }
            Msg(c, t);
        }
        /// <summary>Test seam: the last dialog texts / options opened per player (harness).</summary>
        public static readonly ConcurrentDictionary<uint, uint> LastDialog = new ConcurrentDictionary<uint, uint>();
        public static void World(string t)
        {
            try
            {
                foreach (var c in Pool.GamePoll.Values.ToArray())
                    if (c != null && c.Player != null) c.SendSysMesage(t, MsgMessage.ChatMode.TopLeftSystem, MsgMessage.MsgColor.yellow);
            }
            catch { }
        }
        public static void Lua(Client.GameClient c, string call, uint msgId) { try { LuaUi.LuaPush.Send(c, call, msgId); } catch { } }

        static readonly ConcurrentDictionary<string, long> _last = new ConcurrentDictionary<string, long>();
        /// <summary>true = too soon (the request is dropped).</summary>
        public static bool Throttled(uint uid, string what, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            string k = uid + ":" + what;
            if (_last.TryGetValue(k, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[k] = now;
            return false;
        }

        static readonly ConcurrentDictionary<uint, object> _locks = new ConcurrentDictionary<uint, object>();
        public static object Lock(uint uid) { return _locks.GetOrAdd(uid, _ => new object()); }

        // ------------------------------------------------------------------ random
        static readonly Random _rnd = new Random();
        public static int Next(int min, int maxInclusive)
        {
            if (maxInclusive <= min) return min;
            lock (_rnd) return _rnd.Next(min, maxInclusive + 1);
        }
        public static T Pick<T>(IList<T> list, Func<T, uint> weight)
        {
            if (list == null || list.Count == 0) return default(T);
            long total = 0;
            foreach (var x in list) total += Math.Max(0u, weight(x));
            if (total <= 0) return list[Next(0, list.Count - 1)];
            long r;
            lock (_rnd) r = (long)(_rnd.NextDouble() * total);
            foreach (var x in list) { r -= weight(x); if (r < 0) return x; }
            return list[list.Count - 1];
        }

        // ------------------------------------------------------------------ items
        public static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        public static bool Known(uint id) { return Pool.ItemsBase != null && Pool.ItemsBase.ContainsKey(id); }
        public static string Name(uint id)
        {
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && db != null && !string.IsNullOrEmpty(db.Name) ? db.Name.Replace('~', ' ') : id.ToString();
        }
        /// <summary>Bag slots a list of rewards needs (worst case: a new stack for every stackable item).</summary>
        public static int Slots(IEnumerable<Reward> list)
        {
            int n = 0;
            foreach (var r in list)
            {
                if (r == null || r.Count == 0) continue;
                Database.ItemType.DBItem db;
                if (!Pool.ItemsBase.TryGetValue(r.Item, out db)) continue;
                if (r.Minutes > 0) n += (int)r.Count;
                else if (db.StackSize > 1) n += (int)((r.Count + db.StackSize - 1) / db.StackSize);
                else n += (int)r.Count;
            }
            return n;
        }
        public static bool HasSpace(Client.GameClient c, int slots)
        {
            if (slots <= 0) return true;
            if (slots > 40) return false;
            try { return c.Inventory.HaveSpace((byte)slots); } catch { return false; }
        }
        /// <summary>Takes n of an item (false = not enough, nothing taken).</summary>
        public static bool Take(Client.GameClient c, uint id, uint n, ServerSockets.Packet stream)
        {
            if (n == 0) return true;
            if (n > 100000 || Count(c, id) < n) return false;
            if (c.Inventory.Remove(id, n, stream)) return true;
            // [2026-10-15 e2e] Inventory.Remove checks bound and unbound stacks separately (Contain), so a mix
            // (3 bound + 27 unbound Octave Bells) failed "use 30 bells". The total was checked above: take unit by unit.
            for (uint k = 0; k < n; k++)
            {
                MsgGameItem it = null;
                foreach (var x in c.Inventory.ClientItems.Values) if (x.ITEM_ID == id) { it = x; break; }
                if (it == null) return false;
                try { c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, stream); } catch (Exception e) { MyConsole.SaveException(e); return false; }
            }
            return true;
        }
        /// <summary>Grants every known item (the caller checked the space). Returns the readable list.</summary>
        public static string Give(Client.GameClient c, IEnumerable<Reward> list, ServerSockets.Packet stream, string festival, string why)
        {
            var parts = new List<string>();
            foreach (var r in list)
            {
                if (r == null || r.Count == 0 || !Known(r.Item)) continue;
                uint left = Math.Min(r.Count, 60000u);
                bool ok = true;
                if (r.Minutes > 0)
                    for (uint i = 0; i < Math.Min(left, 40u); i++) c.Inventory.AddTime(stream, r.Item, 0, 0, 0, r.Bound, r.Minutes * 60);
                else ok = c.Inventory.AddItemWitchStack(r.Item, 0, (ushort)left, stream, r.Bound);
                parts.Add(r.Count + " " + Name(r.Item));
                Log(festival, c, "GRANT", why + " " + r + (ok ? "" : " (FAILED)"));
            }
            return string.Join(", ", parts);
        }

        // ------------------------------------------------------------------ log
        static readonly object _logSync = new object();
        public static void Log(string festival, Client.GameClient c, string what, string detail)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\t" + festival + "\t" + what + "\t"
                    + (c != null && c.Player != null ? c.Player.UID + " " + c.Player.Name : "-") + "\t" + detail;
                string dir = Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, festival);
                lock (_logSync)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "log.txt"), line + Environment.NewLine);
                }
            }
            catch { }
        }

        // ------------------------------------------------------------------ hooks (one line each in the shared files)
        /// <summary>Threading\PoolProcesses.cs CharactersCallback: the per-player tick (copies, encounters).</summary>
        public static void OnTick(Client.GameClient c)
        {
            if (c == null || c.Player == null) return;
            try { AprilFools2023.AprilFools2023Ui.Tick(c); Arena.Tick(c); PuppetTheater.PT.Tick(c); } catch (Exception e) { MyConsole.SaveException(e); }// [PuppetTheater 2026-10-15] re-entry hints / stuck players
        }

        public delegate bool DialogHandler(Client.GameClient c, ServerSockets.Packet stream, byte option, string input);
        static readonly ConcurrentDictionary<uint, DialogHandler> _dialogs = new ConcurrentDictionary<uint, DialogHandler>();
        /// <summary>Opens a server dialog that answers on a virtual NPC id (no NPC object: the client only echoes ActiveNpc).</summary>
        public static Game.MsgNpc.Dialog Dialog(Client.GameClient c, ServerSockets.Packet stream, uint virtualId, DialogHandler handler)
        {
            _dialogs[virtualId] = handler;
            c.ActiveNpc = virtualId;
            LastDialog[c.Player.UID] = virtualId;
            return new Game.MsgNpc.Dialog(c, stream);
        }
        /// <summary>Game\MsgNpc\Procesor.cs: the answer of a dialog opened with Dialog(). True = answered.</summary>
        public static bool OnDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null) return false;
            if (PuppetTheater.PT.OnDialog(c, stream, npcid, option, input)) return true;// [PuppetTheater 2026-10-15] Strings of Terror NPCs 8893001-8893004 + dialogs 8893090-8893099
            if (npcid == AprilFools2023.AprilFools2023Ui.LyonNpc && option == 0) return NpcInView(c, npcid) && AprilFools2023.AprilFools2023Ui.OnLyon(c, stream);
            if (npcid < VirtualFirst || npcid > VirtualLast) return false;
            DialogHandler h; uint last;
            // only the dialog this player opened last answers (no blind replies to another virtual id)
            if (!LastDialog.TryGetValue(c.Player.UID, out last) || last != npcid) return true;
            if (!_dialogs.TryGetValue(npcid, out h) || h == null) return true;
            try { h(c, stream, option, input); }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }
        public const uint VirtualFirst = 8892090, VirtualLast = 8892099;
        /// <summary>The festival NPC is a running festival`s NPC and stands in the player`s screen.</summary>
        public static bool NpcInView(Client.GameClient c, uint npcid)
        {
            try
            {
                if (!Festivals.FestivalManager.IsFestivalNpc(npcid)) return false;
                Game.MsgNpc.Npc obj;
                return c.Map != null && c.Map.SearchNpcInScreen(npcid, c.Player.X, c.Player.Y, c.Player.DynamicID, out obj);
            }
            catch { return false; }
        }

        /// <summary>MY System\MsgItem\MsgItemUsuagePacket.cs UseItem: right-click of an event item. True = handled.</summary>
        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null) return false;
            try
            {
                if (AprilFools2023.AprilFools2023Items.OnUse(c, item, stream)) return true;
                if (PuppetTheater.PT.OnUse(c, item, stream)) return true;// [PuppetTheater 2026-10-15] Strings of Terror boxes
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return false;
        }
    }

    // ====================================================================== per-player state
    /// <summary>[EventData 2026-10-15] Per-player state of one festival, kept in the unified EventStore (MySQL, section
    /// = festival key, values in its Strings). The old Data\&lt;Key&gt;\players.txt is imported once and renamed
    /// *.migrated. Set is written to MySQL before it returns (a player's write that fails throws => the caller grants nothing).</summary>
    public sealed class Store
    {
        readonly string _path;
        readonly object _sync = new object();
        readonly EventData.LegacyBackend _be;

        public Store(string festival)
        {
            _path = Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, festival, "players.txt");
            _be = new EventData.LegacyBackend(festival, () => _path, EventData.LegacyFormat.KvText);
        }
        /// <summary>The OLD file (only read once by the migration).</summary>
        public string FilePath { get { return _path; } }
        public string EventKey { get { return _be.Event; } }

        static string Clean(string v) { return (v ?? "").Replace(";", ",").Replace("=", ":").Replace("\t", " ").Replace("\r", "").Replace("\n", ""); }

        public string Get(uint uid, string key, string def = "")
        {
            _be.EnsureMigrated();
            return EventData.EventStore.GetString(uid, _be.Event, key, def);
        }
        public long GetLong(uint uid, string key, long def = 0)
        {
            long v;
            return long.TryParse(Get(uid, key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def;
        }
        public List<int> GetList(uint uid, string key)
        {
            var l = new List<int>();
            foreach (var p in Get(uid, key, "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            { int x; if (int.TryParse(p, out x)) l.Add(x); }
            return l;
        }
        /// <summary>Writes several keys at once to MySQL (null value = remove).</summary>
        public void Set(uint uid, params KeyValuePair<string, string>[] values)
        {
            lock (_sync)
            {
                _be.EnsureMigrated();
                try
                {
                    EventData.EventStore.Edit(uid, _be.Event, s =>
                    {
                        foreach (var kv in values)
                        {
                            if (kv.Value == null) s.Strings.Remove(kv.Key);
                            else s.Strings[kv.Key] = Clean(kv.Value);
                        }
                    }, true);
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[FoolsKit] save " + _be.Event + " uid " + uid + ": " + e.Message); } catch { }
                    if (uid != 0) throw;   // a player's state that is not stored must not lead to a grant
                }
            }
        }
        public void Set(uint uid, string key, string value) { Set(uid, new KeyValuePair<string, string>(key, value)); }
        public void Set(uint uid, string key, long value) { Set(uid, key, value.ToString(CultureInfo.InvariantCulture)); }
        public static KeyValuePair<string, string> KV(string k, object v)
        {
            if (v == null) return new KeyValuePair<string, string>(k, null);
            var e = v as System.Collections.IEnumerable;
            if (e != null && !(v is string)) return new KeyValuePair<string, string>(k, string.Join(",", e.Cast<object>()));
            return new KeyValuePair<string, string>(k, Convert.ToString(v, CultureInfo.InvariantCulture));
        }
        /// <summary>Every player (not uid 0) that has this key, with its value (rankings).</summary>
        public List<KeyValuePair<uint, string>> All(string key)
        {
            _be.EnsureMigrated();
            var r = new List<KeyValuePair<uint, string>>();
            foreach (var kv in EventData.EventStore.All(_be.Event)) { string v; if (kv.Value.Strings.TryGetValue(key, out v)) r.Add(new KeyValuePair<uint, string>(kv.Key, v)); }
            return r;
        }
        /// <summary>Test seam: look for the old players.txt again.</summary>
        public void Reset() { _be.Recheck(); }
    }

    // ====================================================================== private copies + personal monsters
    public sealed class Copy
    {
        public uint Owner, Map, Dyn;
        public string Tag;
        public ushort ExitX, ExitY; public uint ExitMap;
        public DateTime Expires, LastInside;
        public readonly List<Game.MsgMonster.MonsterRole> Mobs = new List<Game.MsgMonster.MonsterRole>();
        /// <summary>Fired once (on the owner's tick) when every monster of the current wave is dead and the owner is inside.</summary>
        public Action<Client.GameClient, Copy> OnCleared;
        public bool Armed;
        public object Data;
    }

    public static class Arena
    {
        static readonly ConcurrentDictionary<uint, Copy> _copies = new ConcurrentDictionary<uint, Copy>();
        /// <summary>Test seam (null on the server): replaces GameClient.Teleport.</summary>
        public static Action<Client.GameClient, ushort, ushort, uint, uint> TeleportOverride;
        /// <summary>Test seam: spawns are recorded, not placed (harness without loaded maps).</summary>
        public static Func<Copy, uint, ushort, ushort, Game.MsgMonster.MonsterRole> SpawnOverride;

        public static Copy Of(uint owner) { Copy c; return _copies.TryGetValue(owner, out c) ? c : null; }
        public static bool Inside(Client.GameClient c, Copy cp)
        {
            return c != null && c.Player != null && cp != null && c.Player.Map == cp.Map && c.Player.DynamicID == cp.Dyn;
        }

        public static void Move(Client.GameClient c, ushort x, ushort y, uint map, uint dyn)
        {
            var o = TeleportOverride;
            if (o != null) { o(c, x, y, map, dyn); return; }
            c.Teleport(x, y, map, dyn);
        }

        public static bool NearestValid(Role.GameMap gm, ref ushort x, ref ushort y)
        {
            if (gm == null) return false;
            if (gm.ValidLocation(x, y)) return true;
            for (int r = 1; r <= 8; r++)
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r) continue;
                        int nx = x + dx, ny = y + dy;
                        if (nx < 0 || ny < 0 || nx > ushort.MaxValue || ny > ushort.MaxValue) continue;
                        if (gm.ValidLocation((ushort)nx, (ushort)ny)) { x = (ushort)nx; y = (ushort)ny; return true; }
                    }
            return false;
        }

        /// <summary>The owner's copy of <paramref name="map"/> with this tag (a new one when there is none), and moves the
        /// owner to x,y inside it. Null when the map can't be loaded.</summary>
        public static Copy Enter(Client.GameClient c, uint map, ushort x, ushort y, string tag, int minutes, uint exitMap, ushort exitX, ushort exitY)
        {
            var cur = Of(c.Player.UID);
            if (cur != null && (cur.Map != map || cur.Tag != tag)) { Close(cur, false); cur = null; }
            Role.GameMap gm = null;
            if (TeleportOverride == null)
            {
                gm = Pool.ServerMaps[map];
                if (gm == null || !NearestValid(gm, ref x, ref y)) return null;
            }
            if (cur == null)
            {
                uint dyn = gm != null ? gm.GenerateDynamicID() : (uint)(900000 + c.Player.UID % 100000);
                cur = new Copy { Owner = c.Player.UID, Map = map, Dyn = dyn, Tag = tag, ExitMap = exitMap, ExitX = exitX, ExitY = exitY };
                _copies[c.Player.UID] = cur;
            }
            cur.Expires = DateTime.Now.AddMinutes(Math.Max(5, minutes));
            cur.LastInside = DateTime.Now;
            Move(c, x, y, map, cur.Dyn);
            return cur;
        }

        /// <summary>Moves the owner to x,y inside its existing copy.</summary>
        public static bool Goto(Client.GameClient c, Copy cp, ushort x, ushort y)
        {
            if (cp == null) return false;
            if (TeleportOverride == null)
            {
                var gm = Pool.ServerMaps[cp.Map];
                if (gm == null || !NearestValid(gm, ref x, ref y)) return false;
            }
            cp.LastInside = DateTime.Now;
            Move(c, x, y, cp.Map, cp.Dyn);
            return true;
        }

        /// <summary>One personal monster of a registered family (festival types are registered while the festival runs).</summary>
        public static Game.MsgMonster.MonsterRole Spawn(Copy cp, uint family, ushort x, ushort y, ushort range, ServerSockets.Packet stream)
        {
            if (cp == null) return null;
            var so = SpawnOverride;
            if (so != null)
            {
                var m = so(cp, family, x, y);
                if (m != null) lock (cp.Mobs) cp.Mobs.Add(m);
                return m;
            }
            Game.MsgMonster.MonsterFamily fam;
            if (Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(family, out fam) || fam == null) return null;
            var map = Pool.ServerMaps[cp.Map];
            if (map == null) return null;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;
            var copy = fam.Copy();
            ushort r = Math.Max((ushort)1, range);
            copy.SpawnX = (ushort)Math.Max(1, x - r); copy.SpawnY = (ushort)Math.Max(1, y - r);
            copy.MaxSpawnX = (ushort)(x + r); copy.MaxSpawnY = (ushort)(y + r);
            copy.MapID = map.ID; copy.SpawnCount = 1;
            Game.MsgMonster.MonsterRole mob = null;
            for (int tries = 0; tries < 6 && mob == null; tries++)
                mob = map.MonstersColletion.Add(copy, true, cp.Dyn, true);
            if (mob == null) return null;
            mob.RemoveOnDead = true;
            lock (cp.Mobs) cp.Mobs.Add(mob);
            try
            {
                mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                mob.Send(mob.GetArray(stream, false));
            }
            catch { }
            return mob;
        }

        public static int Alive(Copy cp)
        {
            if (cp == null) return 0;
            int n = 0;
            lock (cp.Mobs) foreach (var m in cp.Mobs) if (m != null && m.Alive) n++;
            return n;
        }

        /// <summary>Starts a wave: OnCleared fires once all its monsters are dead.</summary>
        public static void Arm(Copy cp, Action<Client.GameClient, Copy> onCleared)
        {
            if (cp == null) return;
            cp.OnCleared = onCleared;
            cp.Armed = onCleared != null;
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
                {
                    var stream = rec.GetStream();
                    mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                }
                var gm = mob.GMap;
                if (gm != null)
                {
                    gm.View.LeaveMap<Role.IMapObj>(mob);
                    if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false);
                }
            }
            catch { }
        }
        public static void ClearMobs(Copy cp)
        {
            if (cp == null) return;
            Game.MsgMonster.MonsterRole[] list;
            lock (cp.Mobs) { list = cp.Mobs.ToArray(); cp.Mobs.Clear(); }
            if (SpawnOverride != null) return;
            foreach (var m in list) if (m != null && m.Alive) Despawn(m);
        }

        /// <summary>Removes the copy (monsters despawned); kick = the owner is sent to the exit when still inside.</summary>
        public static void Close(Copy cp, bool kick)
        {
            if (cp == null) return;
            Copy cur;
            if (_copies.TryGetValue(cp.Owner, out cur) && cur == cp) _copies.TryRemove(cp.Owner, out cur);
            cp.Armed = false; cp.OnCleared = null;
            ClearMobs(cp);
            if (!kick) return;
            Client.GameClient c;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(cp.Owner, out c) && Inside(c, cp) && cp.ExitMap != 0)
                try { Move(c, cp.ExitX, cp.ExitY, cp.ExitMap, 0); } catch { }
        }
        public static void Leave(Client.GameClient c)
        {
            var cp = Of(c.Player.UID);
            if (cp != null) Close(cp, true);
        }

        /// <summary>The per-player tick: expiry, "left the copy for 3 minutes", the armed wave.</summary>
        public static void Tick(Client.GameClient c)
        {
            Copy cp;
            if (!_copies.TryGetValue(c.Player.UID, out cp) || cp == null) return;
            var now = DateTime.Now;
            bool inside = Inside(c, cp);
            if (inside) cp.LastInside = now;
            if (now > cp.Expires) { Kit.Msg(c, "Your time in the maze is over."); Close(cp, true); return; }
            if (!inside && (now - cp.LastInside).TotalMinutes >= 3) { Close(cp, false); return; }
            if (cp.Armed && cp.OnCleared != null && inside && Alive(cp) == 0)
            {
                var h = cp.OnCleared;
                cp.Armed = false; cp.OnCleared = null;
                lock (cp.Mobs) cp.Mobs.Clear();
                if (h != null) h(c, cp);
            }
        }
        /// <summary>Test seam / festival end: every copy with this tag is closed.</summary>
        public static int CloseAll(string tag, bool kick)
        {
            int n = 0;
            foreach (var kv in _copies.ToArray()) if (kv.Value != null && kv.Value.Tag == tag) { Close(kv.Value, kick); n++; }
            return n;
        }
    }
}
