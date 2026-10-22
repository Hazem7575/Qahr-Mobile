// ============================================================================
//  [MidAutumnReunion2022 2026-10-15] HolidayKit: small shared helpers for the Mid-Autumn holiday group
//  (MidAutumnReunion2022, DemonInvasionMadPet, MidAutumnVillage/JadeRabbit, DragonSaga, BruceLeePreheat).
//  No hooks: everything here is called from the festivals' own LuaPush handlers.
//
//    IFestIo        what a festival game needs from a player (bag counts, take, give, lua push, message). The game
//                   logic only talks to this, so a harness can run every rule against a fake player.
//    ClientIo       the real implementation over Client.GameClient (stack-safe take, stack/timed give, space check).
//    ItemSpec       the client's item string "id addamount num monopoly savetime active" (luacfg / luaui tables).
//    PlayerStore    per-player key/value state in ONE json file per festival (Infinity[DB]\<Festival>\players.json),
//                   written atomically on every change (tmp + replace). Survives relogs and restarts.
//    PrivateFight   one personal monster in a private copy (own DynamicID) of a map: teleport in, spawn, poll the kill,
//                   callback once, send the player back. Used for the beast / boss challenges of the event windows.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.MidAutumnReunion2022
{
    /// <summary>The client's item string: "id addamount num monopoly savetime active" (only id and num are required).</summary>
    public struct ItemSpec
    {
        public uint Id; public uint Count; public bool Bound; public uint Minutes;
        public static ItemSpec Parse(string s)
        {
            var it = new ItemSpec();
            if (string.IsNullOrWhiteSpace(s)) return it;
            var p = s.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            uint v;
            if (p.Length > 0 && uint.TryParse(p[0], out v)) it.Id = v;
            it.Count = p.Length > 2 && uint.TryParse(p[2], out v) && v > 0 ? v : 1;
            it.Bound = p.Length > 3 && uint.TryParse(p[3], out v) && v != 0;
            it.Minutes = p.Length > 4 && uint.TryParse(p[4], out v) ? v : 0;
            return it;
        }
        public static ItemSpec Of(uint id, uint count, bool bound = true, uint minutes = 0) { return new ItemSpec { Id = id, Count = count, Bound = bound, Minutes = minutes }; }
        /// <summary>"id 0 n" (+ " monopoly" + " savetime 1" when they matter) for the client windows.</summary>
        public string Lua() { return Id + " 0 " + Count + (Bound || Minutes > 0 ? " " + (Bound ? 3 : 0) : "") + (Minutes > 0 ? " " + Minutes + " 1" : ""); }
        public override string ToString() { return Kit.ItemName(Id) + (Count > 1 ? " x" + Count : ""); }
    }

    public interface IFestIo
    {
        uint Uid { get; }
        string Name { get; }
        int Level { get; }
        uint Count(uint itemId);
        int FreeSlots();
        /// <summary>Slots the items need (stack-aware; timed items 1 each).</summary>
        int SlotsFor(IEnumerable<ItemSpec> items);
        /// <summary>Takes exactly n (false and nothing taken when the bag has fewer).</summary>
        bool Take(uint itemId, uint n);
        bool Give(ItemSpec it);
        void Lua(uint msgId, string call);
        void Msg(string text);
    }

    public sealed class ClientIo : IFestIo
    {
        readonly Client.GameClient c;
        public ClientIo(Client.GameClient client) { c = client; }
        public Client.GameClient Client { get { return c; } }
        public uint Uid { get { return c.Player.UID; } }
        public string Name { get { return c.Player.Name; } }
        public int Level { get { return c.Player.Level; } }
        public uint Count(uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        public int FreeSlots() { try { return Math.Max(0, 40 - c.Inventory.ClientItems.Count); } catch { return 0; } }// Inventory.File_Size = 40 (private)
        public int SlotsFor(IEnumerable<ItemSpec> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                if (i.Id == 0 || i.Count == 0) continue;
                Database.ItemType.DBItem db;
                if (i.Minutes > 0 || Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i.Id, out db) || db.StackSize <= 1) n += (int)i.Count;
                else n += (int)((i.Count + db.StackSize - 1) / db.StackSize);
            }
            return n;
        }
        public bool Take(uint id, uint count)
        {
            if (count == 0) return true;
            if (Count(id) < count) return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                uint left = count;
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
        public bool Give(ItemSpec it)
        {
            if (it.Id == 0 || it.Count == 0) return false;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (it.Minutes > 0)
                    {
                        bool ok = true;
                        for (uint i = 0; i < it.Count; i++) ok &= c.Inventory.AddTime(stream, it.Id, 0, 0, 0, it.Bound, it.Minutes);
                        return ok;
                    }
                    uint left = it.Count;
                    while (left > 0)
                    {
                        ushort part = (ushort)Math.Min(left, 60000u);
                        if (!c.Inventory.AddItemWitchStack(it.Id, 0, part, stream, it.Bound)) return false;
                        left -= part;
                    }
                    return true;
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return false; }
        }
        public void Lua(uint msgId, string call) { LuaUi.LuaPush.Send(c, call, msgId); }
        public void Msg(string text) { try { c.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
    }

    public static class Kit
    {
        static readonly Random _rng = new Random();
        public static Func<int, int> RngOverride;// harness: deterministic draws
        /// <summary>0 .. max-1.</summary>
        public static int Next(int max)
        {
            if (max <= 1) return 0;
            var o = RngOverride; if (o != null) return Math.Max(0, Math.Min(max - 1, o(max)));
            lock (_rng) return _rng.Next(max);
        }
        public static int Pick(IList<int> weights)
        {
            int total = 0; foreach (var w in weights) total += Math.Max(0, w);
            if (total <= 0) return 0;
            int r = Next(total);
            for (int i = 0; i < weights.Count; i++) { r -= Math.Max(0, weights[i]); if (r < 0) return i; }
            return weights.Count - 1;
        }
        public static string ItemName(uint id)
        {
            try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); }
        }
        /// <summary>Gives all (space checked by the caller); returns the list of what was given.</summary>
        public static List<string> GiveAll(IFestIo io, IEnumerable<ItemSpec> items)
        {
            var given = new List<string>();
            foreach (var it in items) { if (it.Id == 0 || it.Count == 0) continue; given.Add((io.Give(it) ? "" : "FAILED ") + it); }
            return given;
        }
        public static long NowMinutes(DateTime now) { return (long)(now - new DateTime(2020, 1, 1)).TotalMinutes; }
        public static string Csv(IEnumerable<int> v) { return string.Join(",", v.Select(x => x.ToString(CultureInfo.InvariantCulture))); }
        public static List<int> Ints(string s)
        {
            var l = new List<int>();
            if (string.IsNullOrEmpty(s)) return l;
            foreach (var p in s.Split(',')) { int v; if (int.TryParse(p.Trim(), out v)) l.Add(v); }
            return l;
        }
        public static void Log(string festival, string what, IFestIo io, string detail)
        {
            try { Festivals.FestivalManager.Log(what, festival, io.Uid + " " + io.Name + " lv" + io.Level + " | " + detail); } catch { }
        }
        /// <summary>A lua table of item strings: {'a','b'}.</summary>
        public static string LuaItems(IEnumerable<string> items) { return "{" + string.Join(",", items.Select(LuaUi.LuaPush.Q)) + "}"; }
    }

    /// <summary>[EventData 2026-10-15] Per-player festival state (string row), kept in the unified EventStore (MySQL;
    /// section = the folder of the old file). The old players.json is imported once and renamed *.migrated.
    /// Put / Update write to MySQL before they return (throws EventStore.EditFailed when it can't => grant nothing).</summary>
    public sealed class PlayerStore
    {
        readonly string _file;
        readonly EventData.LegacyBackend _be;
        public PlayerStore(string file)
        {
            _file = file;
            _be = new EventData.LegacyBackend(EventData.LegacyBackend.KeyForPath(file, "players.json"), () => _file, EventData.LegacyFormat.JsonPlayersStrings);
        }
        /// <summary>The OLD file (only read once by the migration).</summary>
        public string File_ { get { return _file; } }
        public string EventKey { get { return _be.Event; } }
        /// <summary>A COPY of the player's row (empty when new). Change it and Put it back.</summary>
        public Dictionary<string, string> Get(uint uid)
        {
            _be.EnsureMigrated();
            return new Dictionary<string, string>(EventData.EventStore.Section(uid, _be.Event).Strings);
        }
        public void Put(uint uid, Dictionary<string, string> row)
        {
            _be.EnsureMigrated();
            EventData.EventStore.Edit(uid, _be.Event, s => { s.Strings.Clear(); foreach (var kv in row) s.Strings[kv.Key] = kv.Value ?? ""; }, true);
        }
        /// <summary>Runs fn on the row under the player's lock and saves when it returns true (one player can't race himself).</summary>
        public T Update<T>(uint uid, Func<Dictionary<string, string>, Tuple<bool, T>> fn)
        {
            _be.EnsureMigrated();
            return EventData.EventStore.Edit(uid, _be.Event, s =>
            {
                var work = new Dictionary<string, string>(s.Strings);
                var res = fn(work);
                if (res.Item1) { s.Strings.Clear(); foreach (var kv in work) s.Strings[kv.Key] = kv.Value ?? ""; }
                return res;
            }, true);
        }
        /// <summary>Same shape as the old players.json ({"players":{uid:{k:v}}}) for the leaderboards.</summary>
        public Dictionary<string, object> ReadRoot()
        {
            _be.EnsureMigrated();
            var players = new Dictionary<string, object>();
            foreach (var kv in EventData.EventStore.All(_be.Event))
                players[kv.Key.ToString(CultureInfo.InvariantCulture)] = kv.Value.Strings.ToDictionary(x => x.Key, x => (object)x.Value);
            var root = new Dictionary<string, object>();
            root["players"] = players;
            return root;
        }
        public static int I(Dictionary<string, string> r, string k, int def = 0) { string s; int v; return r.TryGetValue(k, out s) && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def; }
        public static long L(Dictionary<string, string> r, string k, long def = 0) { string s; long v; return r.TryGetValue(k, out s) && long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def; }
        public static string S(Dictionary<string, string> r, string k, string def = "") { string s; return r.TryGetValue(k, out s) ? s : def; }
    }

    /// <summary>One personal monster in a private copy of a map (the official "enter the instance and kill X").</summary>
    public static class PrivateFight
    {
        sealed class Fight
        {
            public uint Owner, Map, Dyn, BackMap, BackDyn; public ushort BackX, BackY;
            public Game.MsgMonster.MonsterRole Mob;
            public DateTime Expires, LeaveAt, OutsideSince = DateTime.MinValue;
            public bool Done;
            public Action<Client.GameClient> OnKill;
            public Func<bool> Valid;
            public string Tag;
        }
        static readonly ConcurrentDictionary<uint, Fight> _fights = new ConcurrentDictionary<uint, Fight>();
        static Timer _timer;
        static readonly object _tsync = new object();
        public static int Active { get { return _fights.Count; } }
        public static bool InFight(uint uid) { return _fights.ContainsKey(uid); }
        public static string TagOf(uint uid) { Fight f; return _fights.TryGetValue(uid, out f) ? f.Tag : null; }

        static void EnsureTimer()
        {
            if (_timer != null) return;
            lock (_tsync) if (_timer == null) _timer = new Timer(_ => Tick(), null, 500, 500);
        }

        public static bool FindWalkable(Role.GameMap gm, ref ushort x, ref ushort y, int radius)
        {
            if (gm == null) return false;
            if (x > 0 && y > 0 && gm.ValidLocation(x, y)) return true;
            for (int r = 1; r <= radius; r++)
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r) continue;
                        int nx = x + dx, ny = y + dy;
                        if (nx <= 0 || ny <= 0 || nx >= gm.bounds.Width || ny >= gm.bounds.Height) continue;
                        if (gm.ValidLocation((ushort)nx, (ushort)ny)) { x = (ushort)nx; y = (ushort)ny; return true; }
                    }
            return false;
        }

        /// <summary>Teleports the player into a new private copy of map (near x,y), spawns monster type there and calls onKill
        /// once when it dies while the player is inside and valid() still holds. False (nothing done) when anything is missing.</summary>
        public static bool Start(Client.GameClient c, uint map, ushort x, ushort y, uint monsterType, int minutes, string tag,
                                 Action<Client.GameClient> onKill, Func<bool> valid, out string why)
        {
            why = null;
            if (c == null || c.Player == null) { why = "no player"; return false; }
            if (_fights.ContainsKey(c.Player.UID)) { why = "busy"; return false; }
            Game.MsgMonster.MonsterFamily fam;
            if (Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(monsterType, out fam) || fam == null) { why = "monster type " + monsterType + " is not registered"; return false; }
            var gm = Pool.ServerMaps[map];
            if (gm == null) { why = "map " + map + " can't be loaded"; return false; }
            if (!FindWalkable(gm, ref x, ref y, 40)) { why = "no walkable cell on map " + map; return false; }
            if (gm.MonstersColletion == null) gm.MonstersColletion = new Game.MsgMonster.MobCollection(gm.ID);
            if (!gm.MonstersColletion.ReadMap()) { why = "map " + map + " has no monster layer"; return false; }
            uint dyn = gm.GenerateDynamicID();
            var f = new Fight
            {
                Owner = c.Player.UID, Map = map, Dyn = dyn, BackMap = c.Player.Map, BackDyn = c.Player.DynamicID, BackX = c.Player.X, BackY = c.Player.Y,
                Expires = DateTime.Now.AddMinutes(Math.Max(1, minutes)), OnKill = onKill, Valid = valid, Tag = tag
            };
            if (!_fights.TryAdd(c.Player.UID, f)) { why = "busy"; return false; }
            try
            {
                c.Teleport(x, y, map, dyn);
                ushort mx = (ushort)(x + 4), my = (ushort)(y + 4);
                if (!FindWalkable(gm, ref mx, ref my, 8)) { mx = x; my = y; }
                var copy = fam.Copy();
                copy.SpawnX = mx; copy.SpawnY = my; copy.MaxSpawnX = (ushort)(mx + 1); copy.MaxSpawnY = (ushort)(my + 1);
                copy.MapID = gm.ID; copy.SpawnCount = 1;
                Game.MsgMonster.MonsterRole mob = null;
                for (int i = 0; i < 4 && mob == null; i++) mob = gm.MonstersColletion.Add(copy, true, dyn, true);
                if (mob == null) throw new Exception("spawn failed");
                mob.RemoveOnDead = true;
                f.Mob = mob;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    try { mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y })); } catch { }
                    try { mob.Send(mob.GetArray(stream, false)); } catch { }
                }
            }
            catch (Exception e)
            {
                Fight dummy; _fights.TryRemove(c.Player.UID, out dummy);
                try { if (c.Player.Map == map && c.Player.DynamicID == dyn) c.Teleport(f.BackX, f.BackY, f.BackMap, f.BackDyn); } catch { }
                why = e.Message; return false;
            }
            EnsureTimer();
            return true;
        }

        static void Despawn(Game.MsgMonster.MonsterRole mob)
        {
            if (mob == null) return;
            try
            {
                bool was = mob.Alive;
                mob.RemoveOnDead = true; mob.HitPoints = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                }
                var gm = mob.GMap;
                if (gm != null) { gm.View.LeaveMap<Role.IMapObj>(mob); if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false); }
            }
            catch { }
        }

        static bool Inside(Client.GameClient c, Fight f) { return c != null && c.Player != null && c.Player.Map == f.Map && c.Player.DynamicID == f.Dyn; }

        static void SendBack(Client.GameClient c, Fight f)
        {
            try { if (Inside(c, f)) c.Teleport(f.BackX, f.BackY, f.BackMap, f.BackDyn); } catch { }
        }

        /// <summary>Ends a player's fight now (festival over / logout): monster removed, player sent back, no reward.</summary>
        public static void Cancel(uint uid)
        {
            Fight f;
            if (!_fights.TryRemove(uid, out f)) return;
            if (f.Mob != null && f.Mob.Alive) Despawn(f.Mob);
            Client.GameClient c;
            if (Pool.GamePoll.TryGetValue(uid, out c)) SendBack(c, f);
        }

        static int _busy;
        static void Tick()
        {
            if (Interlocked.Exchange(ref _busy, 1) == 1) return;
            try
            {
                var now = DateTime.Now;
                foreach (var kv in _fights.ToArray())
                {
                    var f = kv.Value;
                    try
                    {
                        Client.GameClient c;
                        bool online = Pool.GamePoll.TryGetValue(f.Owner, out c) && c != null && c.Player != null;
                        bool valid = true; try { valid = f.Valid == null || f.Valid(); } catch { valid = false; }
                        if (!online || !valid) { Cancel(f.Owner); continue; }
                        if (f.Done)
                        {
                            if (now >= f.LeaveAt) { Fight x; _fights.TryRemove(f.Owner, out x); SendBack(c, f); }
                            continue;
                        }
                        bool inside = Inside(c, f);
                        if (!inside) { if (f.OutsideSince == DateTime.MinValue) f.OutsideSince = now; }
                        else f.OutsideSince = DateTime.MinValue;
                        if (f.Mob != null && !f.Mob.Alive)
                        {
                            f.Done = true; f.LeaveAt = now.AddSeconds(3);
                            if (inside) { try { if (f.OnKill != null) f.OnKill(c); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } } }
                            continue;
                        }
                        if (now >= f.Expires || (f.OutsideSince != DateTime.MinValue && (now - f.OutsideSince).TotalSeconds > 20))
                        {
                            if (inside) { try { c.SendSysMesage("Time is up. You left the challenge.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
                            Cancel(f.Owner);
                        }
                    }
                    catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                }
            }
            finally { Interlocked.Exchange(ref _busy, 0); }
        }
    }
}
