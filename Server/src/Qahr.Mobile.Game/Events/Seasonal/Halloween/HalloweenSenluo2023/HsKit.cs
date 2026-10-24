// ============================================================================
//  [HalloweenSenluo2023 2026-10-15] small play kit for the Halloween festivals of this agent group
//  (HalloweenSenluo2023, Halloween2024, PuppetTheater...): private map copies (DynamicID) with their own NPCs and
//  monsters, kill detection by polling (no hook in MonsterRole), one NPC click router and one item-use router,
//  safe item helpers (count / take / give with bag-space check), a 1 s timer that closes copies whose owner left.
//
//  Hooks (one line each, tagged [HalloweenSenluo2023 2026-10-15]):
//    Game\MsgNpc\Procesor.cs (before FestivalManager.OnNpc)      -> if (HalloweenSenluo2023.HsKit.OnNpc(...)) return;
//    MY System\MsgItem\MsgItemUsuagePacket.cs UseItem (top)      -> if (HalloweenSenluo2023.HsKit.OnUseItem(...)) return;
//  Nothing here runs unless the owning festival is running (every handler checks it).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.HalloweenSenluo2023
{
    /// <summary>One spawned monster of a copy.</summary>
    public sealed class HsMob
    {
        public Game.MsgMonster.MonsterRole Mob;
        public uint Type;
        public string Tag = "";
        public bool Dead;
    }

    /// <summary>One private copy of a map for one player.</summary>
    public sealed class HsCopy
    {
        public string Festival = "", Kind = "";
        public uint Owner, Map, Dyn, ExitMap = 1002;
        public ushort ExitX = 439, ExitY = 478;
        public DateTime Expires, LastInside;
        public readonly List<Game.MsgNpc.Npc> Npcs = new List<Game.MsgNpc.Npc>();
        public readonly List<HsMob> Mobs = new List<HsMob>();
        public readonly Dictionary<string, long> V = new Dictionary<string, long>(StringComparer.Ordinal);// run state
        public readonly object Sync = new object();
        public bool Closed;
        /// <summary>A monster of this copy died (called once per monster, from the kit timer or a harness).</summary>
        public Action<HsCopy, HsMob, Client.GameClient> OnKill;
        /// <summary>The copy is being closed (owner left / expired / festival over).</summary>
        public Action<HsCopy> OnClose;
        public long Get(string k) { long v; lock (V) return V.TryGetValue(k, out v) ? v : 0; }
        public void Set(string k, long v) { lock (V) V[k] = v; }
        public int Alive(string tag) { lock (Sync) return Mobs.Count(m => !m.Dead && (tag == null || m.Tag == tag)); }
    }

    public static class HsKit
    {
        // ---------------------------------------------------------------- seams (null on the server)
        public static Action<Client.GameClient, ushort, ushort, uint, uint> TeleportOverride;
        public static Func<DateTime> ClockOverride;
        public static DateTime Now { get { var c = ClockOverride; return c != null ? c() : DateTime.Now; } }

        static readonly ConcurrentDictionary<uint, HsCopy> Copies = new ConcurrentDictionary<uint, HsCopy>();
        static Timer _timer;
        static int _ticking;
        static readonly object _timerLock = new object();

        public static bool Running(string festival) { try { return Festivals.FestivalManager.IsRunning(festival); } catch { return false; } }

        // ---------------------------------------------------------------- routers (the two hook lines)
        /// <summary>NPC click router (Procesor.cs, the NPC is already checked to be in the player's view). True = answered.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null) return false;
            try
            {
                if (HsZones.OnNpc(c, stream, npcid, option, input)) return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return false;
        }

        /// <summary>Item-use router (MsgItemUsuagePacket.UseItem). True = this kit opened the item.</summary>
        public static bool OnUseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null) return false;
            try
            {
                if (HsBoxes.OnUse(c, item, stream)) return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return false;
        }

        // ---------------------------------------------------------------- messages
        public static void Msg(Client.GameClient c, string t)
        {
            try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        public static void Center(Client.GameClient c, string t)
        {
            try { c.SendSysMesage(t, MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow); } catch { }
            Msg(c, t);
        }
        public static void Lua(Client.GameClient c, string call, uint msg) { LuaUi.LuaPush.Send(c, call, msg); }

        /// <summary>A Lua single-quoted string literal.</summary>
        public static string LuaStr(string s)
        {
            s = s ?? "";
            return "'" + s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ") + "'";
        }

        // ---------------------------------------------------------------- items
        public static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>Removes exactly count of id (stack aware). False (nothing removed) when the bag has fewer.</summary>
        public static bool Take(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (count == 0) return true;
            if (count > 100000 || Count(c, id) < count) return false;
            uint left = count;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).OrderBy(i => i.StackSize).ToArray())
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

        /// <summary>Bag slots needed for these grants (stack aware, pessimistic).</summary>
        public static int SlotsFor(IEnumerable<HsGrant> grants)
        {
            int n = 0;
            foreach (var g in grants)
            {
                if (g == null || g.Id == 0 || g.Count <= 0) continue;
                Database.ItemType.DBItem db;
                if (g.Minutes > 0 || Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(g.Id, out db) || db.StackSize <= 1) n += g.Count;
                else n += (g.Count + db.StackSize - 1) / db.StackSize;
            }
            return n;
        }
        public static bool HasSpace(Client.GameClient c, int slots)
        {
            if (slots <= 0) return true;
            if (slots > 40) return false;
            try { return c.Inventory.HaveSpace((byte)slots); } catch { return false; }
        }

        /// <summary>Gives one grant (the caller checked the space). Returns a readable text or null on failure.</summary>
        public static string Give(Client.GameClient c, HsGrant g, ServerSockets.Packet stream)
        {
            if (g == null || g.Id == 0 || g.Count <= 0) return null;
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(g.Id, out db)) return null;
            bool ok = true;
            try
            {
                if (g.Minutes > 0)
                {
                    for (int i = 0; i < g.Count && ok; i++)
                        ok = c.Inventory.Add(stream, g.Id, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, g.Bound, Role.Flags.ItemEffect.None, false, "", (uint)g.Minutes);
                }
                else if (db.StackSize > 1)
                {
                    int left = g.Count;
                    while (left > 0 && ok)
                    {
                        int part = Math.Min(left, Math.Max(1, (int)db.StackSize));
                        ok = c.Inventory.AddItemWitchStack(g.Id, 0, (ushort)part, stream, g.Bound);
                        left -= part;
                    }
                }
                else
                {
                    for (int i = 0; i < g.Count && ok; i++)
                        ok = c.Inventory.Add(stream, g.Id, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, g.Bound);
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); ok = false; }
            if (!ok) return null;
            return Name(g.Id) + (g.Count > 1 ? " x" + g.Count : "");
        }

        public static string Name(uint id)
        {
            try { return QuestEngine.QuestGrammar.ItemName(id); } catch { }
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) ? db.Name : id.ToString();
        }

        // ---------------------------------------------------------------- moves
        public static void Move(Client.GameClient c, ushort x, ushort y, uint map, uint dyn)
        {
            var o = TeleportOverride;
            if (o != null) { o(c, x, y, map, dyn); return; }
            c.Teleport(x, y, map, dyn);
        }
        public static bool NearestValid(Role.GameMap gm, ref ushort x, ref ushort y)
        {
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

        // ---------------------------------------------------------------- copies
        public static HsCopy CopyOf(uint owner) { HsCopy r; return Copies.TryGetValue(owner, out r) && !r.Closed ? r : null; }
        public static bool Inside(Client.GameClient c, HsCopy r)
        {
            return c != null && c.Player != null && r != null && !r.Closed && c.Player.Map == r.Map && c.Player.DynamicID == r.Dyn;
        }
        /// <summary>The copy the player stands in right now (null when outside any of the kit's copies).</summary>
        public static HsCopy Here(Client.GameClient c)
        {
            var r = c != null && c.Player != null ? CopyOf(c.Player.UID) : null;
            return r != null && Inside(c, r) ? r : null;
        }
        public static IEnumerable<HsCopy> All() { return Copies.Values.Where(r => !r.Closed).ToArray(); }

        /// <summary>Opens a NEW private copy of map for the player (an older copy of the same player is closed first)
        /// and moves the player to (x,y). Null when the map can't be loaded or the cell is not walkable.</summary>
        public static HsCopy Open(Client.GameClient c, string festival, string kind, uint map, ushort x, ushort y, int minutes, uint exitMap, ushort exitX, ushort exitY)
        {
            Role.GameMap gm = null;
            try { gm = Pool.ServerMaps[map]; } catch { }
            if (gm == null || !NearestValid(gm, ref x, ref y)) return null;
            HsCopy old;
            if (Copies.TryRemove(c.Player.UID, out old)) Close(old, false);
            var r = new HsCopy
            {
                Festival = festival, Kind = kind, Owner = c.Player.UID, Map = map, Dyn = gm.GenerateDynamicID(),
                Expires = Now.AddMinutes(Math.Max(1, minutes)), LastInside = Now, ExitMap = exitMap, ExitX = exitX, ExitY = exitY
            };
            Copies[c.Player.UID] = r;
            StartTimer();
            Move(c, x, y, map, r.Dyn);
            return r;
        }

        /// <summary>Moves the owner inside the copy (another spot of the same copy).</summary>
        public static void MoveInside(Client.GameClient c, HsCopy r, ushort x, ushort y)
        {
            Role.GameMap gm = null;
            try { gm = Pool.ServerMaps[r.Map]; } catch { }
            if (gm != null) NearestValid(gm, ref x, ref y);
            Move(c, x, y, r.Map, r.Dyn);
        }

        public static Game.MsgNpc.Npc AddNpc(HsCopy r, uint uid, uint mesh, string name, ushort x, ushort y, byte type = 2)
        {
            if (r == null || r.Closed) return null;
            Role.GameMap gm = null;
            try { gm = Pool.ServerMaps[r.Map]; } catch { }
            if (gm == null) return null;
            if (x >= gm.bounds.Width || y >= gm.bounds.Height) return null;
            lock (r.Sync)
            {
                var old = r.Npcs.FirstOrDefault(n => n.UID == uid);
                if (old != null) return old;
                var np = Game.MsgNpc.Npc.Create();
                np.UID = uid;
                np.NpcType = (Role.Flags.NpcType)type;
                np.Mesh = mesh;
                np.Map = r.Map;
                np.X = x; np.Y = y;
                np.DynamicID = r.Dyn;
                np.AllowDynamic = false;
                np.Name = name ?? "";
                try
                {
                    gm.View.EnterMap<Role.IMapObj>(np);
                    using (var rec = new ServerSockets.RecycledPacket()) np.Send(rec.GetStream());
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                r.Npcs.Add(np);
                return np;
            }
        }
        public static bool HasNpc(HsCopy r, uint uid) { if (r == null) return false; lock (r.Sync) return r.Npcs.Any(n => n.UID == uid); }
        public static Game.MsgNpc.Npc NpcOf(HsCopy r, uint uid) { if (r == null) return null; lock (r.Sync) return r.Npcs.FirstOrDefault(n => n.UID == uid); }

        public static void RemoveNpc(HsCopy r, uint uid)
        {
            Game.MsgNpc.Npc np;
            lock (r.Sync)
            {
                np = r.Npcs.FirstOrDefault(n => n.UID == uid);
                if (np == null) return;
                r.Npcs.Remove(np);
            }
            DespawnNpc(r, np);
        }
        static void DespawnNpc(HsCopy r, Game.MsgNpc.Npc np)
        {
            try
            {
                Role.GameMap gm;
                if (Pool.ServerMaps == null || !Pool.ServerMaps.TryGetValue(r.Map, out gm) || gm == null) return;
                gm.View.LeaveMap<Role.IMapObj>(np);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var remove = rec.GetStream().ActionCreate(new ActionQuery { ObjId = np.UID, Type = ActionType.RemoveEntity });
                    foreach (var p in gm.View.Roles(Role.MapObjectType.Player, np.X, np.Y))
                        if (np.CanSee(p)) (p as Role.Player).Send(remove);
                }
            }
            catch { }
        }

        /// <summary>Spawns one monster of a registered type (festival monster types are registered while their festival
        /// runs) inside the copy. Never respawns; drops are the type's normal drops.</summary>
        public static HsMob Spawn(HsCopy r, uint typeId, ushort x, ushort y, ushort range, string tag)
        {
            if (r == null || r.Closed) return null;
            Game.MsgMonster.MonsterFamily fam;
            if (Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(typeId, out fam) || fam == null) return null;
            Role.GameMap map = null;
            try { map = Pool.ServerMaps[r.Map]; } catch { }
            if (map == null) return null;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;
            NearestValid(map, ref x, ref y);
            var copy = fam.Copy();
            copy.SpawnX = x; copy.SpawnY = y;
            copy.MaxSpawnX = (ushort)(x + Math.Max((ushort)1, range)); copy.MaxSpawnY = (ushort)(y + Math.Max((ushort)1, range));
            copy.MapID = map.ID; copy.SpawnCount = 1;
            Game.MsgMonster.MonsterRole mob = null;
            for (int tries = 0; tries < 4 && mob == null; tries++)
                mob = map.MonstersColletion.Add(copy, true, r.Dyn, true);
            if (mob == null) return null;
            mob.RemoveOnDead = true;
            var rec = new HsMob { Mob = mob, Type = typeId, Tag = tag ?? "" };
            lock (r.Sync) r.Mobs.Add(rec);
            try
            {
                using (var p = new ServerSockets.RecycledPacket())
                {
                    var stream = p.GetStream();
                    mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                    mob.Send(mob.GetArray(stream, false));
                }
            }
            catch { }
            return rec;
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

        /// <summary>Removes every NPC / living monster of the copy; kick = move the owner out when still inside.</summary>
        public static void Close(HsCopy r, bool kick)
        {
            if (r == null) return;
            lock (r.Sync)
            {
                if (r.Closed) return;
                r.Closed = true;
            }
            HsCopy cur;
            if (Copies.TryGetValue(r.Owner, out cur) && ReferenceEquals(cur, r)) Copies.TryRemove(r.Owner, out cur);
            try { var oc = r.OnClose; if (oc != null) oc(r); } catch (Exception e) { MyConsole.SaveException(e); }
            Game.MsgNpc.Npc[] npcs; HsMob[] mobs;
            lock (r.Sync) { npcs = r.Npcs.ToArray(); r.Npcs.Clear(); mobs = r.Mobs.ToArray(); }
            foreach (var np in npcs) DespawnNpc(r, np);
            foreach (var m in mobs) if (!m.Dead && m.Mob != null && m.Mob.Alive) Despawn(m.Mob);
            if (kick)
            {
                Client.GameClient c;
                if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(r.Owner, out c) && c != null && c.Player != null && c.Player.Map == r.Map && c.Player.DynamicID == r.Dyn)
                {
                    try { Move(c, r.ExitX, r.ExitY, r.ExitMap, 0); } catch { }
                }
            }
        }

        /// <summary>Leaves the player's copy: closes it and moves the player to its exit.</summary>
        public static void Leave(Client.GameClient c)
        {
            var r = CopyOf(c.Player.UID);
            if (r == null) return;
            bool inside = Inside(c, r);
            Close(r, false);
            if (inside) Move(c, r.ExitX, r.ExitY, r.ExitMap, 0);
        }

        // ---------------------------------------------------------------- timer
        static void StartTimer()
        {
            if (_timer != null) return;
            lock (_timerLock)
            {
                if (_timer == null) _timer = new Timer(_ => Tick(), null, 1000, 1000);
            }
        }

        /// <summary>Kill detection + copy expiry (1 s timer; the harness calls it directly).</summary>
        public static void Tick()
        {
            if (Interlocked.Exchange(ref _ticking, 1) == 1) return;
            try
            {
                var now = Now;
                foreach (var r in Copies.Values.ToArray())
                {
                    try
                    {
                        if (r.Closed) continue;
                        Client.GameClient c = null;
                        if (Pool.GamePoll != null) Pool.GamePoll.TryGetValue(r.Owner, out c);
                        bool inside = c != null && Inside(c, r);
                        if (inside) r.LastInside = now;
                        if (!Running(r.Festival)) { if (c != null) Msg(c, "The event is over."); Close(r, true); continue; }
                        if (now > r.Expires) { if (c != null && inside) Msg(c, "Time is up. You left the challenge."); Close(r, true); continue; }
                        if (!inside && (now - r.LastInside).TotalSeconds > 90) { Close(r, false); continue; }
                        // kills
                        List<HsMob> died = null;
                        lock (r.Sync)
                            foreach (var m in r.Mobs)
                                if (!m.Dead && (m.Mob == null || !m.Mob.Alive)) { m.Dead = true; (died ?? (died = new List<HsMob>())).Add(m); }
                        if (died != null && r.OnKill != null)
                            foreach (var m in died) r.OnKill(r, m, c);
                    }
                    catch (Exception e) { MyConsole.SaveException(e); }
                }
            }
            finally { Interlocked.Exchange(ref _ticking, 0); }
        }

        // ---------------------------------------------------------------- misc
        static readonly ConcurrentDictionary<string, long> _last = new ConcurrentDictionary<string, long>();
        /// <summary>Per player + key rate limit (true = too fast, ignore).</summary>
        public static bool Throttled(uint uid, string key, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            string k = uid + "|" + key;
            if (_last.TryGetValue(k, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[k] = now;
            return false;
        }
        public static void ClearThrottle() { _last.Clear(); }

        static readonly Random _rnd = new Random();
        public static int Rand(int maxExclusive) { lock (_rnd) return maxExclusive <= 1 ? 0 : _rnd.Next(maxExclusive); }
        public static int Rand(int min, int maxInclusive) { lock (_rnd) return maxInclusive <= min ? min : _rnd.Next(min, maxInclusive + 1); }
        /// <summary>Test seam: forces the next random picks (harness only).</summary>
        public static Func<int, int> RandOverride;
        public static int Roll(int maxExclusive) { var o = RandOverride; return o != null ? Math.Max(0, Math.Min(maxExclusive - 1, o(maxExclusive))) : Rand(maxExclusive); }

        public static bool Near(Client.GameClient c, ushort x, ushort y, int dist)
        {
            return Math.Max(Math.Abs(c.Player.X - x), Math.Abs(c.Player.Y - y)) <= dist;
        }
    }

    /// <summary>One item grant (count, bound, optional lifetime in minutes).</summary>
    public sealed class HsGrant
    {
        public uint Id;
        public int Count = 1;
        public bool Bound = true;
        public int Minutes;
        public HsGrant() { }
        public HsGrant(uint id, int count, bool bound = true, int minutes = 0) { Id = id; Count = count; Bound = bound; Minutes = minutes; }
    }
}
