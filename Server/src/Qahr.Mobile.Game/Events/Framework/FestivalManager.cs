// ============================================================================
//  [Festivals] runtime: schedules, live start / end, festival NPC dialogs, exchanges, commands, state.
//
//  START (live, no restart):
//    1. its monster types are registered in Pool.MonsterFamilies (new dictionary published by reference swap,
//       the same way HotReload's reloadmonsters adds types),
//    2. its NPCs are put on their maps with GameMap.AddNpc (the same runtime path as HotReload's NpcHotReload:
//       players in view get MsgNpcInfo at once, players walking in see it through the normal view scan),
//    3. its spawn boxes get their monsters through MobCollection.Add (respawning normally while it runs),
//    4. its quests.json is published to the QuestEngine as an overlay (QuestGrammar.OverlayProvider + Rebuild),
//    5. announcement (center + system chat, Arabic and English) and a line in Infinity[DB]\festivals_log.txt.
//  END: the quests are switched off first (so nothing can be finished while the rest goes away), then every
//    monster of its spawns (and any live monster of its own types on a loaded map) is removed (RemoveEntity to the
//    players in view), its NPCs are removed (GameMap.RemoveNpc + the DMap cells restored like NpcHotReload), its
//    types are unregistered, announcement + log.
//  START and END also push the festival quests' state to every online player (MsgTaskStatus Review list + the
//  progress of the ones in their log), so the quest panel / tracker follow without a relog.
//  [FestivalTrack] npcs.txt may use the OFFICIAL id of an NPC the festival's quests name in Questinfo.ini (at its
//  official map/x,y): the client's quest tracker paths to that spot and looks for that id. NPCs without an official
//  id keep the 8800000-8899999 range. [InstanceNpc] quests.json "instances" may list "npcs" for the private copies.
//  A player who accepted a festival quest keeps it in the quest log: while the festival is stopped the quest is
//  Disabled (QuestEngine: no offer, no counting, no finish, no prize; the quest panel refuses to accept it) and it
//  continues with its saved progress the next time the festival runs. Daily/weekly festival quests still reset.
//
//  Schedule: checked every minute (server local time). Manual on/off overrides are kept in
//  Infinity[DB]\Festivals\_state.json ("on" = running whatever the schedule says, "off" = stopped, "auto" clears).
//  Rewards: festival quest prizes are paid only through the engine finish (once per character unless repeat);
//  exchanges (rewards.json) only while running, next to the NPC, cost first, once per character unless repeat,
//  each claim recorded in Infinity[DB]\Festivals\_claims.txt BEFORE the reward is given.
//
//  Commands: owner @festival list | info <key> | on <key> | off <key> | auto <key> | reload
//            console festival ... (same) ; AdminApi festivals / festival {key, action} / reloadfestivals
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

namespace Qahr.Mobile.Festivals
{
    public static class FestivalManager
    {
        /// <summary>One running festival and everything it put on the server.</summary>
        public sealed class RunState
        {
            public FestivalDef Def;
            public int Run;
            public DateTime Started;
            public bool Forced;
            public readonly List<Game.MsgNpc.Npc> Npcs = new List<Game.MsgNpc.Npc>();
            public readonly List<Game.MsgMonster.MonsterRole> Mobs = new List<Game.MsgMonster.MonsterRole>();
            public readonly List<uint> Types = new List<uint>();
            public readonly List<string> Notes = new List<string>();
        }

        public static readonly object Sync = new object();
        static volatile Dictionary<string, FestivalDef> _defs = new Dictionary<string, FestivalDef>(StringComparer.OrdinalIgnoreCase);
        static readonly ConcurrentDictionary<string, RunState> _running = new ConcurrentDictionary<string, RunState>(StringComparer.OrdinalIgnoreCase);
        static volatile Dictionary<uint, string> _npcOwner = new Dictionary<uint, string>();
        static readonly Dictionary<string, string> _overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, int> _runs = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        static readonly HashSet<string> _wasActive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        static readonly HashSet<string> _claims = new HashSet<string>(StringComparer.Ordinal);
        static readonly object _claimLock = new object(), _logLock = new object();
        static Timer _timer;
        static int _ticking;
        static bool _loaded;

        /// <summary>Test seams (null on the server): announcements and the clock.</summary>
        public static Action<string, string> AnnounceOverride;
        public static Func<DateTime> ClockOverride;
        static DateTime Now { get { var c = ClockOverride; return c != null ? c() : DateTime.Now; } }

        static string StateFile { get { return Path.Combine(FestivalLoader.Root, "_state.json"); } }
        static string ClaimsFile { get { return Path.Combine(FestivalLoader.Root, "_claims.txt"); } }
        static string LogFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "festivals_log.txt"); } }

        public static IEnumerable<FestivalDef> Defs { get { return _defs.Values; } }
        public static bool IsRunning(string key) { return key != null && _running.ContainsKey(key); }
        public static RunState Running(string key) { RunState r; return key != null && _running.TryGetValue(key, out r) ? r : null; }

        // ------------------------------------------------------------------ start / load
        /// <summary>Called once at server start (after HotReload). Loads the festivals and starts the minute timer.</summary>
        public static void Start()
        {
            try
            {
                Init();
                _timer = new Timer(_ => Tick(), null, 1000, 60000); // [FestivalFastStart 2026-10-15] first check 1 s after start (was 15 s): players logging in right after a restart got the icons late
                int valid = _defs.Values.Count(d => d.Valid);
                MyConsole.WriteLine("[Festivals] " + _defs.Count + " festival(s), " + valid + " valid (Infinity[DB]\\Festivals). Schedules checked every minute. Commands: festival list|info|on|off|auto|reload.", ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Festivals] start failed (module off): " + e.Message, ConsoleColor.Yellow);
                try { MyConsole.SaveException(e); } catch { }
            }
        }

        /// <summary>Loads defs + state and hooks the QuestEngine (no timer; the harness calls this).</summary>
        public static void Init()
        {
            lock (Sync)
            {
                if (!_loaded)
                {
                    QuestEngine.QuestGrammar.OverlayProvider = Overlays;
                    QuestEngine.QuestEngineCore.ExtraNpcHandlerExists = id => _npcOwner.ContainsKey(id);
                    QuestEngine.QuestEngineCore.ExtraNpcHandler = (c, s, id) => ShowMenu(c, s, id);
                    QuestEngine.QuestEngineCore.FestivalQuestFinished = (c, key, qid, given) => Log("QUEST", key, c.Player.UID + " " + c.Player.Name + " finished quest " + qid + " gave=[" + given + "]");
                    LoadState();
                    LoadClaims();
                    _loaded = true;
                }
                LoadDefs();
            }
            // stopped festivals switch their quests off right away (before any schedule tick)
            RebuildQuests();
        }

        static void LoadDefs()
        {
            var types = new HashSet<uint>(_running.Values.SelectMany(r => r.Types));
            var list = FestivalLoader.LoadAll(types);
            var map = new Dictionary<string, FestivalDef>(StringComparer.OrdinalIgnoreCase);
            foreach (var d in list)
            {
                if (!map.ContainsKey(d.Key)) map[d.Key] = d;
                if (!d.Valid)
                {
                    MyConsole.WriteLine("[Festivals] " + d.Key + " is DISABLED: " + string.Join(" | ", d.Errors.Take(5)) + (d.Errors.Count > 5 ? " (+" + (d.Errors.Count - 5) + " more, see festival info)" : ""), ConsoleColor.Yellow);
                    Log("INVALID", d.Key, string.Join(" | ", d.Errors));
                }
            }
            _defs = map;
        }

        /// <summary>Re-reads every folder. A running festival whose folder changed (or became invalid / was deleted)
        /// is ended and, when still due, started again with the new content.</summary>
        public static List<string> Reload()
        {
            var lines = new List<string>();
            lock (Sync)
            {
                var old = _defs;
                LoadDefs();
                foreach (var r in _running.Values.ToArray())
                {
                    FestivalDef nd;
                    bool same = _defs.TryGetValue(r.Def.Key, out nd) && nd.Valid && nd.Hash == r.Def.Hash;
                    if (same) { _defs[r.Def.Key] = r.Def; continue; }// keep the running copy (same content)
                    lines.Add(r.Def.Key + ": content changed -> restarting");
                    StopLocked(r.Def.Key, false, "reload");
                    _wasActive.Add(r.Def.Key);// started again quietly (no second announcement)
                }
                lines.Add("Festivals reloaded: " + _defs.Count + " folder(s), " + _defs.Values.Count(d => d.Valid) + " valid, " + _defs.Values.Count(d => !d.Valid) + " disabled by errors.");
                foreach (var d in _defs.Values.Where(x => !x.Valid)) lines.Add("  " + d.Key + ": " + string.Join(" | ", d.Errors.Take(3)));
            }
            RebuildQuests();
            lines.AddRange(Evaluate(true));
            return lines;
        }

        // ------------------------------------------------------------------ schedule
        /// <summary>Should the festival run now (override, then enabled + schedule).</summary>
        public static bool Want(FestivalDef d, DateTime now)
        {
            if (d == null || !d.Valid) return false;
            string ov;
            lock (_overrides) _overrides.TryGetValue(d.Key, out ov);
            if (ov == "on") return true;
            if (ov == "off") return false;
            return d.Enabled && d.Schedule.IsDue(now);
        }

        static void Tick()
        {
            if (Interlocked.Exchange(ref _ticking, 1) == 1) return;
            try { Evaluate(true); }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            finally { Interlocked.Exchange(ref _ticking, 0); }
        }

        /// <summary>Starts / stops every festival whose wanted state differs from its real state. fromTimer: the minute
        /// tick or a reload (festivals that were running before a restart / reload start again without an announcement).</summary>
        public static List<string> Evaluate(bool fromTimer)
        {
            var lines = new List<string>();
            bool changed = false;
            lock (Sync)
            {
                DateTime now = Now;
                foreach (var d in _defs.Values.ToArray())
                {
                    bool want = Want(d, now), run = _running.ContainsKey(d.Key);
                    if (want && !run)
                    {
                        bool quiet = fromTimer && _wasActive.Contains(d.Key);// the server restarted while it was running
                        lines.Add(StartLocked(d, !quiet, quiet ? "resume (server restart / reload)" : (fromTimer ? "schedule" : "command")));
                        changed = true;
                    }
                    else if (!want && run)
                    {
                        lines.Add(StopLocked(d.Key, true, fromTimer ? "schedule" : "command"));
                        changed = true;
                    }
                }
                // a running festival whose folder was deleted / made invalid by reload
                foreach (var r in _running.Values.ToArray())
                    if (!_defs.ContainsKey(r.Def.Key)) { lines.Add(StopLocked(r.Def.Key, true, "folder removed")); changed = true; }
                if (fromTimer) _wasActive.Clear();
                if (changed) SaveState();
            }
            return lines;
        }

        // ------------------------------------------------------------------ START
        static string StartLocked(FestivalDef d, bool announce, string why)
        {
            var r = new RunState { Def = d, Started = Now };
            try
            {
                lock (HotReload.HotReloadManager.Sync)// never at the same time as reloadmonsters / reloadnpcs
                {
                    RegisterTypes(r);
                    PlaceNpcs(r);
                    PlaceSpawns(r);
                }
            }
            catch (Exception e)
            {
                r.Notes.Add("start error: " + e.Message);
                try { MyConsole.SaveException(e); } catch { }
            }
            int run;
            lock (_runs) { _runs.TryGetValue(d.Key, out run); run++; _runs[d.Key] = run; }
            r.Run = run;
            _running[d.Key] = r;
            RebuildNpcOwner();
            string q = RebuildQuests();
            int pushed = PushQuestPanel(d);// online players get the festival quests' state at once (no relog)
            if (pushed > 0) r.Notes.Add("quest panel refreshed for " + pushed + " player(s)");
            if (announce) Announce(d.StartAr, d.StartEn);
            PushIcon(d, true);// [LuaUi] festival icon for the online players
            string msg = "[Festivals] " + d.Key + " ON (run " + run + ")  ·  " + r.Npcs.Count + " npc · " + r.Mobs.Count + " mob · " + d.QuestIds.Count + " quest";
            MyConsole.WriteLine(msg, ConsoleColor.Green);
            Log("START", d.Key, why + " run=" + run + " npcs=" + r.Npcs.Count + " types=" + r.Types.Count + " mobs=" + r.Mobs.Count + " quests=" + d.QuestIds.Count + (q != null ? " engine " + (q.StartsWith("rebuild failed") ? q : "rebuilt") : "") + (r.Notes.Count > 0 ? " notes: " + string.Join("; ", r.Notes) : ""));
            return msg;
        }

        static void RegisterTypes(RunState r)
        {
            if (r.Def.Monsters.Count == 0) return;
            var cur = Pool.MonsterFamilies ?? new Dictionary<uint, Game.MsgMonster.MonsterFamily>();
            var next = new Dictionary<uint, Game.MsgMonster.MonsterFamily>(cur);
            foreach (var fam in r.Def.Monsters)
            {
                if (next.ContainsKey(fam.ID)) { r.Notes.Add("monster type " + fam.ID + " already exists - not registered"); continue; }
                next[fam.ID] = fam;
                r.Types.Add(fam.ID);
            }
            Pool.MonsterFamilies = next;
        }

        static Role.GameMap GetMap(uint id)
        {
            Role.GameMap m;
            if (Pool.ServerMaps == null) return null;
            if (Pool.ServerMaps.TryGetValue(id, out m)) return m;
            return Pool.ServerMaps[id];// loads the map (lazy load, same as a player entering it)
        }

        static void PlaceNpcs(RunState r)
        {
            foreach (var n in r.Def.Npcs)
            {
                try
                {
                    var map = GetMap(n.Map);
                    if (map == null) { r.Notes.Add("npc " + n.Uid + ": map " + n.Map + " could not be loaded"); continue; }
                    if (map.View.MapContain(Role.MapObjectType.Npc, n.Uid, p => p.DynamicID == 0)) { r.Notes.Add("npc " + n.Uid + " is already on map " + n.Map + " - skipped"); continue; }
                    var np = Game.MsgNpc.Npc.Create();
                    np.UID = n.Uid;
                    np.NpcType = (Role.Flags.NpcType)n.Type;
                    np.Mesh = n.Mesh;
                    np.Map = n.Map;
                    np.X = n.X;
                    np.Y = n.Y;
                    np.AllowDynamic = n.Dynamic;
                    map.AddNpc(np);// view + cells + MsgNpcInfo to the players who see it
                    r.Npcs.Add(np);
                }
                catch (Exception e) { r.Notes.Add("npc " + n.Uid + ": " + e.Message); }
            }
        }

        static void PlaceSpawns(RunState r)
        {
            var types = new HashSet<uint>(_running.Values.SelectMany(x => x.Types).Where(x => !r.Types.Contains(x)));
            foreach (var sp in r.Def.Spawns)
            {
                try
                {
                    var fam = FestivalLoader.FindType(r.Def, sp.Monster, types);
                    if (fam == null) { r.Notes.Add("spawn " + sp.Monster + ": unknown monster"); continue; }
                    var map = GetMap(sp.Map);
                    if (map == null) { r.Notes.Add("spawn " + sp.Monster + ": map " + sp.Map + " could not be loaded"); continue; }
                    if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                    if (!map.MonstersColletion.ReadMap()) { r.Notes.Add("spawn " + sp.Monster + ": map " + sp.Map + " has no monster collection"); continue; }
                    var copy = fam.Copy();
                    copy.SpawnX = sp.X; copy.SpawnY = sp.Y;
                    copy.MaxSpawnX = (ushort)Math.Min(ushort.MaxValue, sp.X + Math.Max(1, (int)sp.W));
                    copy.MaxSpawnY = (ushort)Math.Min(ushort.MaxValue, sp.Y + Math.Max(1, (int)sp.H));
                    copy.MapID = map.ID; copy.SpawnCount = 1;
                    int made = 0;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        for (int i = 0; i < sp.Count; i++)
                        {
                            Game.MsgMonster.MonsterRole mob = null;
                            for (int tries = 0; tries < 4 && mob == null; tries++)
                                mob = map.MonstersColletion.Add(copy, false, 0, true);
                            if (mob == null) continue;
                            mob.RemoveOnDead = false;
                            if (mob.Family != null) mob.Family.RespawnTime = Math.Max(0, sp.Respawn - 8);// MonsterRole adds 8 s
                            r.Mobs.Add(mob);
                            made++;
                            try
                            {
                                mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                                mob.Send(mob.GetArray(stream, false));
                            }
                            catch { }
                        }
                    }
                    if (made < sp.Count) r.Notes.Add("spawn " + sp.Monster + " @" + sp.Map + ": " + made + "/" + sp.Count + " placed (no free walkable cell in the box?)");
                }
                catch (Exception e) { r.Notes.Add("spawn " + sp.Monster + ": " + e.Message); }
            }
        }

        // ------------------------------------------------------------------ END
        static string StopLocked(string key, bool announce, string why)
        {
            RunState r;
            if (!_running.TryRemove(key, out r)) return "[Festivals] " + key + " is not running.";
            RebuildNpcOwner();
            string q = RebuildQuests();// quests off first
            int mobs = 0, npcs = 0;
            var notes = new List<string>();
            int pushed = PushQuestPanel(r.Def);
            if (pushed > 0) notes.Add("quest panel refreshed for " + pushed + " player(s)");
            lock (HotReload.HotReloadManager.Sync)
            {
                try { mobs = RemoveMonsters(r); } catch (Exception e) { notes.Add("monsters: " + e.Message); }
                try { npcs = RemoveNpcs(r); } catch (Exception e) { notes.Add("npcs: " + e.Message); }
                try { UnregisterTypes(r); } catch (Exception e) { notes.Add("types: " + e.Message); }
            }
            if (announce) Announce(r.Def.StopAr, r.Def.StopEn);
            PushIcon(r.Def, false);// [LuaUi] hide the festival icon
            string msg = "[Festivals] " + key + " ENDED (" + why + "): " + npcs + " NPC(s) and " + mobs + " monster(s) removed, " + r.Types.Count + " type(s) unregistered, quests off" + (notes.Count > 0 ? "; " + string.Join("; ", notes) : "");
            MyConsole.WriteLine(msg, ConsoleColor.Green);
            Log("END", key, why + " run=" + r.Run + " npcs=" + npcs + " mobs=" + mobs + (q != null ? " engine " + (q.StartsWith("rebuild failed") ? q : "rebuilt") : "") + (notes.Count > 0 ? " notes: " + string.Join("; ", notes) : ""));
            return msg;
        }

        // ------------------------------------------------------------------ [LuaUi] festival icons
        static void PushIcon(FestivalDef d, bool show)
        {
            try
            {
                // [FestivalIcons 2026-10-14] tracked per player + shared buttons ref-counted (Base\Festivals\FestivalIcons.cs)
                if (d == null || string.IsNullOrWhiteSpace(d.IconShow)) return;
                int n = show ? FestivalIcons.Show(d.IconShow) : FestivalIcons.Hide(d.IconShow, d.IconHide, RunningIconShows());
                if (n != 0) Log(show ? "ICON" : (n < 0 ? "ICONKEEP" : "ICONOFF"), d.Key, n < 0 ? "shared button still used by a running festival" : n + " player(s)");
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        /// <summary>[FestivalIcons 2026-10-14] the icon_show calls of the running festivals.</summary>
        static List<string> RunningIconShows()
        {
            return _running.Values.ToArray().Where(r => r != null && r.Def != null && !string.IsNullOrWhiteSpace(r.Def.IconShow)).Select(r => r.Def.IconShow).ToList();
        }
        /// <summary>Login: each distinct icon of the running festivals once. [FestivalIcons 2026-10-14]</summary>
        public static void SendIcons(Client.GameClient c)
        {
            if (c == null || c.Player == null) return;
            try { FestivalIcons.Login(c, RunningIconShows()); }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        /// <summary>The Highlight Events pages (ActInfo ids) of the running festivals, in festival order, no duplicates.</summary>
        public static List<string> RunningHighlightActs()
        {
            var l = new List<string>();
            foreach (var r in _running.Values.ToArray().OrderBy(x => x.Def != null ? x.Def.Key : "", StringComparer.OrdinalIgnoreCase))
                if (r != null && r.Def != null) foreach (var a in r.Def.HighlightActs) if (!l.Contains(a)) l.Add(a);
            return l;
        }
        /// <summary>The running festival whose icon_click is this callback ("{msgId}</S>{func}"), or null.</summary>
        public static FestivalDef RunningByIconClick(uint msgId, string func)
        {
            string key = msgId + "</S>" + func;
            foreach (var r in _running.Values.ToArray())
                if (r != null && r.Def != null && string.Equals((r.Def.IconClick ?? "").Trim(), key, StringComparison.Ordinal)) return r.Def;
            return null;
        }

        static int RemoveMonsters(RunState r)
        {
            int n = 0;
            var seen = new HashSet<uint>();
            foreach (var mob in r.Mobs)
                if (mob != null && seen.Add(mob.UID)) { Despawn(mob); n++; }
            r.Mobs.Clear();
            // any other live monster of its own types (e.g. a quest onAccept spawn) on a loaded map
            if (r.Types.Count > 0)
            {
                var ids = new HashSet<uint>(r.Types);
                foreach (uint mapId in HotReload.HotReloadManager.LoadedMapIds())
                {
                    var map = HotReload.HotReloadManager.LoadedMap(mapId);
                    if (map == null || map.View == null) continue;
                    foreach (var obj in map.View.GetAllMapRoles(Role.MapObjectType.Monster).ToArray())
                    {
                        var mob = obj as Game.MsgMonster.MonsterRole;
                        if (mob == null || mob.Family == null || !ids.Contains(mob.Family.ID) || !seen.Add(mob.UID)) continue;
                        Despawn(mob); n++;
                    }
                }
            }
            return n;
        }

        static void Despawn(Game.MsgMonster.MonsterRole mob)
        {
            try
            {
                mob.RemoveOnDead = true;
                mob.RespawnStamp = DateTime.Now.AddDays(9999);
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

        static int RemoveNpcs(RunState r)
        {
            int n = 0;
            var cells = new HotReload.MapCells();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var mine = new HashSet<Game.MsgNpc.Npc>(r.Npcs);
                foreach (var def in r.Def.Npcs)
                {
                    var map = HotReload.HotReloadManager.LoadedMap(def.Map);
                    if (map == null || map.View == null) continue;
                    // the objects it placed (instance copies of the same id, DynamicID != 0, are not touched)
                    foreach (var o in map.View.GetAllMapRoles(Role.MapObjectType.Npc, p => p.UID == def.Uid && (mine.Contains(p as Game.MsgNpc.Npc) || p.DynamicID == 0)).ToArray())
                    {
                        var np = o as Game.MsgNpc.Npc;
                        if (np == null) continue;
                        ushort x = np.X, y = np.Y;
                        map.RemoveNpc(np, stream);// leaves the view + RemoveEntity to the players who see it
                        try { cells.RestoreAround(map, x, y); } catch { }
                        n++;
                    }
                }
            }
            r.Npcs.Clear();
            return n;
        }

        static void UnregisterTypes(RunState r)
        {
            if (r.Types.Count == 0) return;
            var cur = Pool.MonsterFamilies;
            if (cur == null) return;
            var next = new Dictionary<uint, Game.MsgMonster.MonsterFamily>(cur);
            foreach (uint id in r.Types)
            {
                Game.MsgMonster.MonsterFamily f;
                if (next.TryGetValue(id, out f) && r.Def.Monsters.Contains(f)) next.Remove(id);
            }
            Pool.MonsterFamilies = next;
        }

        // ------------------------------------------------------------------ QuestEngine overlay
        static List<QuestEngine.QuestGrammar.QuestOverlay> Overlays()
        {
            var list = new List<QuestEngine.QuestGrammar.QuestOverlay>();
            foreach (var d in _defs.Values.ToArray())
            {
                RunState r;
                bool active = _running.TryGetValue(d.Key, out r);
                var def = active ? r.Def : d;
                if (!def.Valid && !active) continue;
                var ov = new QuestEngine.QuestGrammar.QuestOverlay { Key = def.Key, Active = active, Root = def.QuestsRoot };
                if (active)
                {
                    ov.MonsterNames.AddRange(def.Monsters.Select(m => m.Name));
                    ov.NpcIds.AddRange(def.Npcs.Select(n => n.Uid));
                    ov.NpcIds.AddRange(def.InstanceNpcs);
                }
                list.Add(ov);
            }
            // running copies whose folder vanished from _defs
            foreach (var r in _running.Values.ToArray())
                if (!list.Any(o => o.Key.Equals(r.Def.Key, StringComparison.OrdinalIgnoreCase)))
                    list.Add(new QuestEngine.QuestGrammar.QuestOverlay { Key = r.Def.Key, Active = true, Root = r.Def.QuestsRoot, MonsterNames = r.Def.Monsters.Select(m => m.Name).ToList(), NpcIds = r.Def.Npcs.Select(n => n.Uid).ToList() });
            return list;
        }

        /// <summary>Republishes the QuestEngine table with the current overlays (null when the engine is not loaded).</summary>
        static string RebuildQuests()
        {
            try
            {
                if (QuestEngine.QuestEngineCore.Current == null) return null;// engine never loaded (it reads the overlays when it does)
                if (!_defs.Values.Any(d => d.QuestsRoot != null) && !_running.Values.Any(r => r.Def.QuestsRoot != null) && !_hadOverlays) return null;
                _hadOverlays = _defs.Values.Any(d => d.QuestsRoot != null) || _running.Values.Any(r => r.Def.QuestsRoot != null);
                return QuestEngine.QuestEngineCore.Rebuild();
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Festivals] quest engine rebuild failed: " + e.Message, ConsoleColor.Yellow);
                try { MyConsole.SaveException(e); } catch { }
                return "rebuild failed: " + e.Message;
            }
        }
        static bool _hadOverlays;

        static void RebuildNpcOwner()
        {
            var d = new Dictionary<uint, string>();
            foreach (var r in _running.Values.ToArray())
            {
                foreach (var n in r.Def.Npcs) d[n.Uid] = r.Def.Key;
                foreach (uint id in r.Def.InstanceNpcs) d[id] = r.Def.Key;// [InstanceNpc] NPCs inside its private copies
            }
            _npcOwner = d;
        }

        // ------------------------------------------------------------------ quest panel refresh (start / end)
        /// <summary>Test seam (null on the server): called with every player the refresh goes to.</summary>
        public static Action<Client.GameClient, List<uint>> QuestPanelPushOverride;

        /// <summary>Start / end: every online player gets the current state of the festival's quests (the same
        /// MsgTaskStatus "Review" list the client gets at login, only these ids) and, for the ones in their log, the
        /// progress (MsgQuestData) again - so the quest panel and the tracker follow the festival without a relog.</summary>
        static int PushQuestPanel(FestivalDef d)
        {
            if (d == null || d.QuestIds.Count == 0 || Pool.GamePoll == null) return 0;
            var all = Database.QuestInfo.AllQuests;
            var ids = d.QuestIds.Where(id => all != null && all.ContainsKey(id)).Distinct().ToList();
            if (ids.Count == 0) return 0;
            int n = 0;
            foreach (var c in Pool.GamePoll.Values.ToArray())
            {
                if (c == null || c.Player == null || c.Player.QuestGUI == null) continue;
                try { SendQuestStates(c, ids); n++; }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            }
            return n;
        }

        public static void SendQuestStates(Client.GameClient c, List<uint> ids)
        {
            var o = QuestPanelPushOverride;
            if (o != null) o(c, ids);
            var gui = c.Player.QuestGUI;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                for (int i = 0; i < ids.Count; i += 80)
                {
                    var part = ids.Skip(i).Take(80).ToList();
                    stream.QuestListCreate(MsgQuestList.QuestMode.Review, (ushort)part.Count);
                    foreach (uint id in part)
                    {
                        MsgQuestList.QuestListItem item = null;
                        try { gui.src.TryGetValue(id, out item); } catch { item = null; }
                        stream.AddItemQuestList(item ?? new MsgQuestList.QuestListItem { UID = id, Status = MsgQuestList.QuestListItem.QuestStatus.Available, Time = 0 });
                    }
                    c.Send(stream.QuestListFinalize());
                }
                foreach (uint id in ids)
                {
                    if (!gui.IsActiveQuest(id)) continue;
                    uint[] cur;
                    gui.GetQuestObjectives(id, out cur);
                    if (cur != null && cur.Length > 0) c.Send(stream.MsgQuestDataCreate(0, id, cur));
                }
            }
        }

        // ------------------------------------------------------------------ NPC dialogs + exchanges
        sealed class DlgState { public uint Npc; public string Key; public readonly Dictionary<byte, int> Pick = new Dictionary<byte, int>(); public readonly Dictionary<byte, int> Confirm = new Dictionary<byte, int>(); }
        static readonly ConcurrentDictionary<uint, DlgState> _dlg = new ConcurrentDictionary<uint, DlgState>();
        const byte OptClose = 255;

        public static bool IsFestivalNpc(uint npcid) { return _npcOwner.ContainsKey(npcid); }

        /// <summary>Procesor hook (NPC already checked to be in the player's view). True = answered.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            string key;
            if (c == null || c.Player == null || !_npcOwner.TryGetValue(npcid, out key)) return false;
            try
            {
                if (option == 0)
                {
                    DlgState old; _dlg.TryRemove(c.Player.UID, out old);
                    if (QuestEngine.QuestEngineCore.OnNpc(c, stream, npcid, option, input)) return true;
                    ShowMenu(c, stream, npcid);
                    return true;
                }
                DlgState st;
                if (_dlg.TryGetValue(c.Player.UID, out st) && st.Npc == npcid)
                {
                    int idx;
                    if (st.Pick.TryGetValue(option, out idx)) { _dlg.TryRemove(c.Player.UID, out st); ShowConfirm(c, stream, npcid, st.Key, idx); return true; }
                    if (st.Confirm.TryGetValue(option, out idx)) { _dlg.TryRemove(c.Player.UID, out st); DoExchange(c, stream, npcid, st.Key, idx); return true; }
                    _dlg.TryRemove(c.Player.UID, out st);
                    if (option == OptClose) return true;
                }
                QuestEngine.QuestEngineCore.OnNpc(c, stream, npcid, option, input);
                return true;
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                return true;
            }
        }

        static void Text(Game.MsgNpc.Dialog d, string text)
        {
            text = (text ?? "").Trim();
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 200);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n - 1, n); if (sp > 100) n = sp + 1; }
                d.AddText(text.Substring(0, n));
                text = text.Substring(n);
            }
        }
        static string Opt(string s) { s = s ?? ""; return s.Length > 60 ? s.Substring(0, 57) + "..." : s; }

        /// <summary>[DreamTheater 2026-10-15] The festival greeting (rewards.json npcText, English) of a running festival NPC, null = none.
        /// Used by the QuestEngine menu instead of its generic "I may need your help, traveler.".</summary>
        public static string NpcGreeting(uint npcid)
        {
            string key; string[] txt;
            if (!_npcOwner.TryGetValue(npcid, out key)) return null;
            var r = Running(key);
            return r != null && r.Def.NpcText.TryGetValue(npcid, out txt) && txt[1].Trim().Length > 0 ? txt[1] : null;
        }

        /// <summary>Greeting + this NPC's exchanges (also the QuestEngine "Something else." of a festival NPC).</summary>
        public static void ShowMenu(Client.GameClient c, ServerSockets.Packet stream, uint npcid)
        {
            string key;
            if (!_npcOwner.TryGetValue(npcid, out key)) return;
            var r = Running(key);
            if (r == null) return;
            var st = new DlgState { Npc = npcid, Key = key };
            var d = new Game.MsgNpc.Dialog(c, stream);
            string[] txt;
            string greet = r.Def.NpcText.TryGetValue(npcid, out txt) && txt[1].Trim().Length > 0 ? txt[1] : "Welcome to the " + r.Def.NameEn + "!";
            Text(d, greet);
            byte opt = 1;
            for (int i = 0; i < r.Def.Exchanges.Count && opt <= 40; i++)
            {
                var ex = r.Def.Exchanges[i];
                if (ex.Npc != npcid) continue;
                st.Pick[opt] = i;
                d.AddOption(Opt(ex.TextEn), opt++);
            }
            d.AddOption("Goodbye.", OptClose).AddAvatar(10).FinalizeDialog();
            if (st.Pick.Count > 0) _dlg[c.Player.UID] = st;
        }

        static void ShowConfirm(Client.GameClient c, ServerSockets.Packet stream, uint npcid, string key, int idx)
        {
            var r = Running(key);
            if (r == null || idx < 0 || idx >= r.Def.Exchanges.Count) return;
            var ex = r.Def.Exchanges[idx];
            var d = new Game.MsgNpc.Dialog(c, stream);
            string rep = ex.Repeat == "once" ? "once per character" : ex.Repeat == "daily" ? "once a day" : ex.Repeat == "festival" ? "once per festival" : "as often as you like";
            Text(d, ex.TextEn + ": you give " + ex.DescribeCost() + " and receive " + ex.DescribeGive() + " (" + rep + ").");
            var st = new DlgState { Npc = npcid, Key = key };
            st.Confirm[101] = idx;
            _dlg[c.Player.UID] = st;
            d.AddOption("Deal.", 101).AddOption("No, thanks.", OptClose).AddAvatar(10).FinalizeDialog();
        }

        static string ClaimId(FestivalDef def, FestivalExchange ex, uint uid, int run)
        {
            string period;
            switch (ex.Repeat)
            {
                case "daily": period = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); break;
                case "festival": period = "run" + run; break;
                case "unlimited": return null;
                default: period = "once"; break;
            }
            return def.Key.ToLowerInvariant() + "/" + ex.Id.ToLowerInvariant() + "|" + uid + "|" + period;
        }

        static void Box(Client.GameClient c, ServerSockets.Packet stream, string text)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            Text(d, text);
            d.AddOption("OK.", OptClose).AddAvatar(10).FinalizeDialog();
        }

        static uint CountItem(Client.GameClient c, uint id)
        {
            uint n = 0;
            foreach (var it in c.Inventory.ClientItems.Values.ToArray())
                if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u;
            return n;
        }
        static void TakeItem(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
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
        }
        static int SlotsFor(List<FestivalItem> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i.Id, out db)) { n += i.Count; continue; }
                if (db.StackSize > 1) n += (i.Count + db.StackSize - 1) / db.StackSize;
                else n += i.Count;
            }
            return n;
        }

        static readonly object _exchangeLock = new object();

        /// <summary>One exchange: festival running, player next to the NPC, level, claim, bag space, cost (taken first),
        /// claim recorded, then the reward. Everything is re-checked here; nothing comes from the client but the choice.</summary>
        public static string DoExchange(Client.GameClient c, ServerSockets.Packet stream, uint npcid, string key, int idx)
        {
            string result;
            lock (_exchangeLock)
                result = ExchangeCore(c, stream, npcid, key, idx);
            if (stream != null) { try { Box(c, stream, result); } catch { } }
            return result;
        }

        static string ExchangeCore(Client.GameClient c, ServerSockets.Packet stream, uint npcid, string key, int idx)
        {
            var r = Running(key);
            if (r == null) return "The festival is over.";
            if (idx < 0 || idx >= r.Def.Exchanges.Count) return "That offer is gone.";
            var ex = r.Def.Exchanges[idx];
            if (ex.Npc != npcid) return "That offer is gone.";
            var npc = r.Def.Npcs.FirstOrDefault(n => n.Uid == npcid);
            if (npc == null || c.Player.Map != npc.Map || Math.Max(Math.Abs(c.Player.X - npc.X), Math.Abs(c.Player.Y - npc.Y)) > 18) return "Come closer.";
            if (c.Player.Level < ex.MinLevel) return "You need level " + ex.MinLevel + " for this.";
            string claim = ClaimId(r.Def, ex, c.Player.UID, r.Run);
            if (claim != null) lock (_claimLock) if (_claims.Contains(claim)) return ex.Repeat == "daily" ? "You already did this today." : ex.Repeat == "festival" ? "You already did this during this festival." : "You already received this.";
            int slots = SlotsFor(ex.GiveItems);
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return "Please make " + slots + " free space(s) in your inventory first.";
            foreach (var it in ex.CostItems)
                if (CountItem(c, it.Id) < it.Count) return "You need " + QuestEngine.QuestGrammar.ItemName(it.Id) + " x" + it.Count + ".";
            if (ex.CostSilver > 0 && c.Player.Money < ex.CostSilver) return "You need " + ex.CostSilver + " silver.";
            if (ex.CostCps > 0 && c.Player.ConquerPoints < ex.CostCps) return "You need " + ex.CostCps + " CPs.";
            // [DreamTheater 2026-10-16] Flori (NPC 8830002) gives at most 500 Wish Twinkle Coins (3347478) per day
            // from flowers/gifts (official cap). Certificate exchanges (*_cert) are the uncapped "if you want more"
            // path and are NOT limited. Counter is persisted + daily-reset in EventStore (no new field, hot-reloadable).
            long twinkleGive = 0;
            if (npcid == 8830002 && ex.Id != null && ex.Id.StartsWith("gift_", StringComparison.Ordinal))
                foreach (var gi in ex.GiveItems) if (gi.Id == 3347478) twinkleGive += gi.Count;
            if (twinkleGive > 0)
            {
                long gotToday = 0;
                try { gotToday = global::Qahr.Mobile.EventData.EventStore.GetDaily(c.Player.UID, "DreamTheater", "flori_twinkle"); } catch { }
                if (gotToday + twinkleGive > 500)
                    return "Flori gives at most 500 Wish Twinkle Coins a day from flowers and gifts (you already have " + gotToday + " today). Use Flower Certificates for more.";
            }
            // take the cost
            using (var rec = stream == null ? new ServerSockets.RecycledPacket() : null)
            {
                var s = stream ?? rec.GetStream();
                foreach (var it in ex.CostItems) TakeItem(c, it.Id, it.Count, s);
                if (ex.CostSilver > 0) { c.Player.Money -= ex.CostSilver; try { c.Player.SendUpdate(s, c.Player.Money, MsgUpdate.DataType.Money); } catch { } }
                if (ex.CostCps > 0) c.Player.ConquerPoints -= ex.CostCps;
                // record the claim BEFORE the reward
                if (claim != null) AddClaim(claim);
                var given = new List<string>();
                foreach (var it in ex.GiveItems)
                {
                    try { if (c.Inventory.AddItemWitchStack(it.Id, 0, it.Count, s, it.Bound)) given.Add(QuestEngine.QuestGrammar.ItemName(it.Id) + (it.Count > 1 ? " x" + it.Count : "")); else given.Add("FAILED " + it.Id); }
                    catch (Exception e) { given.Add("FAILED " + it.Id + " (" + e.Message + ")"); }
                }
                // [DreamTheater 2026-10-16] count the Twinkle just granted toward Flori's 500/day cap
                if (twinkleGive > 0) { try { global::Qahr.Mobile.EventData.EventStore.AddDaily(c.Player.UID, "DreamTheater", "flori_twinkle", twinkleGive, 500); } catch { } }
                if (ex.GiveExp > 0) { try { c.IncreaseExperience(s, ex.GiveExp, Role.Flags.ExperienceEffect.angelwing); given.Add(ex.GiveExp + " EXP"); } catch { } }
                if (ex.GiveSilver > 0) { try { c.Player.Money += ex.GiveSilver; c.Player.SendUpdate(s, c.Player.Money, MsgUpdate.DataType.Money); given.Add(ex.GiveSilver + " silver"); } catch { } }
                if (ex.GiveCps > 0) { try { c.Player.ConquerPoints += ex.GiveCps; given.Add(ex.GiveCps + " CPs"); } catch { } }
                Log("EXCHANGE", key, c.Player.UID + " " + c.Player.Name + " lv" + c.Player.Level + " | " + ex.Id + " | paid [" + ex.DescribeCost() + "] got [" + string.Join(", ", given) + "] | " + (claim ?? "unlimited"));
                return "Done! You received: " + string.Join(", ", given) + ".";
            }
        }

        // ------------------------------------------------------------------ claims / state / log
        static void LoadClaims()
        {
            try
            {
                if (!File.Exists(ClaimsFile)) return;
                lock (_claimLock)
                    foreach (string l in File.ReadAllLines(ClaimsFile, Encoding.UTF8))
                        if (l.Trim().Length > 0) _claims.Add(l.Trim());
            }
            catch (Exception e) { MyConsole.WriteLine("[Festivals] _claims.txt: " + e.Message, ConsoleColor.Yellow); }
        }
        static void AddClaim(string claim)
        {
            lock (_claimLock)
            {
                _claims.Add(claim);
                try { Directory.CreateDirectory(FestivalLoader.Root); File.AppendAllText(ClaimsFile, claim + Environment.NewLine, Encoding.UTF8); }
                catch (Exception e) { MyConsole.WriteLine("[Festivals] could not save a claim: " + e.Message, ConsoleColor.Yellow); }
            }
        }

        static void LoadState()
        {
            try
            {
                if (!File.Exists(StateFile)) return;
                var root = FestivalLoader.ReadJson(StateFile);
                var ov = FestivalLoader.Obj(FestivalLoader.Val(root, "overrides"));
                if (ov != null) lock (_overrides) foreach (var kv in ov) { string v = (kv.Value ?? "").ToString().ToLowerInvariant(); if (v == "on" || v == "off") _overrides[kv.Key] = v; }
                foreach (var a in FestivalLoader.Arr(FestivalLoader.Val(root, "active")) ?? new List<object>()) if (a != null) _wasActive.Add(a.ToString());
                var runs = FestivalLoader.Obj(FestivalLoader.Val(root, "runs"));
                if (runs != null) lock (_runs) foreach (var kv in runs) { var d = FestivalLoader.Obj(kv.Value); _runs[kv.Key] = (int)FestivalLoader.U(d != null ? FestivalLoader.Val(d, "run") : kv.Value); }
            }
            catch (Exception e) { MyConsole.WriteLine("[Festivals] _state.json ignored: " + e.Message, ConsoleColor.Yellow); }
        }

        static void SaveState()
        {
            try
            {
                var root = new Dictionary<string, object>();
                root["_readme"] = "Written by the GameServer [Festivals]. overrides: on/off set by @festival on/off (cleared by @festival auto). active: running when written. runs: run counter per festival.";
                var ov = new Dictionary<string, object>();
                lock (_overrides) foreach (var kv in _overrides) ov[kv.Key] = kv.Value;
                root["overrides"] = ov;
                root["active"] = _running.Keys.OrderBy(k => k).Cast<object>().ToList();
                var runs = new Dictionary<string, object>();
                lock (_runs)
                    foreach (var kv in _runs)
                    {
                        var d = new Dictionary<string, object>();
                        d["run"] = kv.Value;
                        RunState r;
                        if (_running.TryGetValue(kv.Key, out r)) d["started"] = r.Started.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        runs[kv.Key] = d;
                    }
                root["runs"] = runs;
                Directory.CreateDirectory(FestivalLoader.Root);
                string tmp = StateFile + ".tmp";
                File.WriteAllText(tmp, AdminApi.AdminJson.Write(root), new UTF8Encoding(false));
                if (File.Exists(StateFile)) File.Replace(tmp, StateFile, null);
                else File.Move(tmp, StateFile);
            }
            catch (Exception e) { MyConsole.WriteLine("[Festivals] could not save _state.json: " + e.Message, ConsoleColor.Yellow); }
        }

        public static void Log(string what, string key, string detail)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | " + what + " | " + key + " | " + (detail ?? "").Replace("\r", " ").Replace("\n", " ") + Environment.NewLine;
                lock (_logLock) File.AppendAllText(LogFile, line, Encoding.UTF8);
            }
            catch { }
        }

        static void Announce(string ar, string en)
        {
            try
            {
                var o = AnnounceOverride;
                if (o != null) { o(ar, en); return; }
                if (Pool.GamePoll == null) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (string text in new[] { ar, en })
                    {
                        if (string.IsNullOrWhiteSpace(text)) continue;
                        Server.SendGlobalPacket(new MsgMessage(text, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                        Server.SendGlobalPacket(new MsgMessage(text, "ALLUSERS", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.System).GetArray(stream));
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ commands
        /// <summary>Owner @festival: runs off the socket thread and replies with system messages.</summary>
        public static void CommandForPlayer(Client.GameClient client, string[] args)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var lines = Command(args, client.Player.Name);
                try
                {
                    for (int i = 0; i < lines.Count && i < 12; i++) client.SendSysMesage(lines[i]);
                    if (lines.Count > 12) client.SendSysMesage("... (" + (lines.Count - 12) + " more lines in the console / AdminApi)");
                }
                catch { }
            });
        }

        /// <summary>festival list | info &lt;key&gt; | on &lt;key&gt; | off &lt;key&gt; | auto &lt;key&gt; | reload</summary>
        public static List<string> Command(string[] args, string who)
        {
            var lines = new List<string>();
            try
            {
                if (!_loaded) Init();
                string verb = args != null && args.Length > 0 ? args[0].ToLowerInvariant() : "list";
                string key = args != null && args.Length > 1 ? args[1] : null;
                switch (verb)
                {
                    case "list":
                        if (_defs.Count == 0) { lines.Add("[Festivals] no festival folders in Infinity[DB]\\Festivals."); break; }
                        foreach (var d in _defs.Values.OrderBy(x => x.Key)) lines.Add(Summary(d));
                        break;
                    case "info":
                        {
                            var d = Find(key);
                            if (d == null) { lines.Add("[Festivals] unknown festival '" + key + "'. Use: festival list"); break; }
                            lines.AddRange(Info(d));
                            break;
                        }
                    case "on":
                    case "off":
                    case "auto":
                        {
                            var d = Find(key);
                            if (d == null) { lines.Add("[Festivals] unknown festival '" + key + "'. Use: festival list"); break; }
                            if (verb == "on" && !d.Valid) { lines.Add("[Festivals] " + d.Key + " has errors and can't start: " + string.Join(" | ", d.Errors.Take(3))); break; }
                            lock (_overrides) { if (verb == "auto") _overrides.Remove(d.Key); else _overrides[d.Key] = verb; }
                            Log("OVERRIDE", d.Key, verb + " by " + who);
                            var res = Evaluate(false);
                            lock (Sync) SaveState();
                            lines.AddRange(res);
                            lines.Add("[Festivals] " + d.Key + ": override " + (verb == "auto" ? "cleared (schedule decides)" : verb) + ", now " + (IsRunning(d.Key) ? "RUNNING" : "stopped") + ".");
                            break;
                        }
                    case "reload":
                        lines.AddRange(Reload());
                        Log("RELOAD", "-", "by " + who);
                        break;
                    default:
                        lines.Add("[Festivals] usage: festival list | info <key> | on <key> | off <key> | auto <key> | reload");
                        break;
                }
            }
            catch (Exception e)
            {
                lines.Add("[Festivals] error: " + e.Message);
                try { MyConsole.SaveException(e); } catch { }
            }
            return lines;
        }

        public static FestivalDef Find(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            FestivalDef d;
            if (_defs.TryGetValue(key.Trim(), out d)) return d;
            var r = Running(key.Trim());
            return r != null ? r.Def : null;
        }

        public static string Override(string key) { string v; lock (_overrides) return _overrides.TryGetValue(key, out v) ? v : null; }

        static string Summary(FestivalDef d)
        {
            bool run = IsRunning(d.Key);
            string ov = Override(d.Key);
            bool due = false; var next = d.Valid && d.Enabled ? d.Schedule.NextChange(Now, out due) : null;// [FestivalAudit] disabled: no "next start"
            return d.Key + " [" + (!d.Valid ? "INVALID" : run ? "RUNNING" : "stopped") + "]" + (ov != null ? " override=" + ov : "") + (!d.Enabled ? " (disabled)" : "") +
                   " " + d.Schedule + (next != null ? " | next " + (due ? "start " : "end ") + next.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : "");
        }

        static List<string> Info(FestivalDef d)
        {
            var l = new List<string>();
            l.Add(Summary(d));
            l.Add("  " + d.NameEn + " / " + d.NameAr + " - folder " + d.Folder);
            l.Add("  npcs " + d.Npcs.Count + ", monster types " + d.Monsters.Count + ", spawns " + d.Spawns.Count + " (" + d.Spawns.Sum(s => s.Count) + " monsters), quests " + d.QuestIds.Count + ", exchanges " + d.Exchanges.Count);
            var r = Running(d.Key);
            if (r != null)
                l.Add("  running since " + r.Started.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + " (run " + r.Run + "): " + r.Npcs.Count + " NPC(s), " + r.Mobs.Count(m => m != null && m.Alive) + "/" + r.Mobs.Count + " monster(s) alive" + (r.Notes.Count > 0 ? "; notes: " + string.Join("; ", r.Notes.Take(4)) : ""));
            foreach (var e in d.Errors) l.Add("  ERROR " + e);
            foreach (var w in d.Warnings) l.Add("  warning " + w);
            if (d.Notes.Length > 0) l.Add("  notes: " + d.Notes);
            return l;
        }

        /// <summary>AdminApi "festivals": every festival with its state and the next start / end.</summary>
        public static List<object> ListJson()
        {
            if (!_loaded) Init();
            var list = new List<object>();
            DateTime now = Now;
            foreach (var d in _defs.Values.OrderBy(x => x.Key))
            {
                var o = new Dictionary<string, object>();
                o["key"] = d.Key; o["name_ar"] = d.NameAr; o["name_en"] = d.NameEn;
                o["valid"] = d.Valid; o["enabled"] = d.Enabled;
                var r = Running(d.Key);
                o["running"] = r != null;
                o["override"] = Override(d.Key) ?? "auto";
                o["schedule"] = d.Schedule.ToString();
                o["due"] = d.Valid && d.Enabled && d.Schedule.IsDue(now);
                bool becomesDue = false;
                var next = d.Valid && d.Enabled ? d.Schedule.NextChange(now, out becomesDue) : null;// [FestivalAudit] a disabled festival never starts by schedule
                o["nextStart"] = next != null && becomesDue ? (object)next.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : null;
                o["nextEnd"] = next != null && !becomesDue ? (object)next.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : null;
                if (next != null && becomesDue)
                {
                    DateTime from = next.Value; bool b2;
                    var end = d.Schedule.NextChange(from, out b2);
                    if (end != null && !b2) o["nextEnd"] = end.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                o["npcs"] = d.Npcs.Count; o["monsterTypes"] = d.Monsters.Count; o["spawns"] = d.Spawns.Count; o["quests"] = d.QuestIds.Count; o["exchanges"] = d.Exchanges.Count;
                if (r != null) { o["started"] = r.Started.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); o["run"] = r.Run; o["liveNpcs"] = r.Npcs.Count; o["liveMonsters"] = r.Mobs.Count(m => m != null && m.Alive); }
                o["errors"] = d.Errors.Cast<object>().ToList();
                o["warnings"] = d.Warnings.Cast<object>().ToList();
                list.Add(o);
            }
            return list;
        }
    }
}
