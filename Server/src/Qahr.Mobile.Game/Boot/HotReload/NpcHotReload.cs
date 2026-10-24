// ============================================================================
//  NpcHotReload - live reload of Data\Npcs.txt.
//
//  How NPCs live in memory:
//  - Each loaded map (Role.GameMap, maps load lazily on first visit) keeps its NPCs in map.View (MapView =
//    grid of 18x18-cell blocks, one list per MapObjectType). GameMap.AddNpc puts an Npc into the view,
//    marks the 5x5 cells around it as "Npc" (not walkable) and sends MsgNpcInfo to the players that see it.
//    GameMap.RemoveNpc takes it out of the view, frees the cells and sends MsgAction RemoveEntity to the
//    players that see it. These two are what events/GM commands already use at runtime from any thread.
//  - A player's screen is NOT a cached list: RoleView.Role() re-scans the map view on every move and spawns
//    whatever is in range (CanAdd). So once the view is right, players walking in see the new state, and
//    players already in range get the despawn/spawn packets from RemoveNpc/AddNpc right away.
//  - Dialog scripts are chosen by NPC id ([NpcAttribute(id)]), so moving/re-skinning an NPC keeps its script.
//
//  What this does:
//  - NpcServer.LoadNpcs (startup / first visit of a map) records what Npcs.txt put on each map (RecordLoaded).
//  - Reload(): re-reads Npcs.txt with the same parser (NpcServer.ParseNpcLine, same blank-line stop and the
//    same "first line wins" for a repeated id on one map), diffs it against that record for every LOADED map,
//    and applies removed / added / moved / changed. Same-map moves and look changes update the SAME Npc
//    object (so code that holds a reference to it keeps working). Moves between maps = remove + add.
//  - Maps that are not loaded yet are not touched: they read the new Npcs.txt when they load.
//  - NPCs that were not created from Npcs.txt (events, statues, Magnolia, guild-war furniture...) are never
//    touched: only NPCs recorded from the file are diffed.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.HotReload
{
    /// <summary>One NPC as Npcs.txt describes it.</summary>
    public sealed class NpcEntry
    {
        public uint Uid;
        public uint Map;
        public ushort X, Y;
        public uint Mesh;
        public byte Type;
        public bool AllowDynamic;
        public int Line;

        public static NpcEntry From(Game.MsgNpc.Npc np, int line)
        {
            return new NpcEntry
            {
                Uid = np.UID,
                Map = np.Map,
                X = np.X,
                Y = np.Y,
                Mesh = np.Mesh,
                Type = (byte)np.NpcType,
                AllowDynamic = np.AllowDynamic,
                Line = line
            };
        }

        public bool SameLook(NpcEntry o) { return Mesh == o.Mesh && Type == o.Type && AllowDynamic == o.AllowDynamic; }
        public bool SamePlace(NpcEntry o) { return Map == o.Map && X == o.X && Y == o.Y; }

        public Game.MsgNpc.Npc CreateNpc()
        {
            Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
            Apply(np);
            np.Map = Map;
            return np;
        }

        /// <summary>Copies position + look onto an Npc that is OFF the map view (same map only).</summary>
        public void Apply(Game.MsgNpc.Npc np)
        {
            np.UID = Uid;
            np.X = X;
            np.Y = Y;
            np.Mesh = Mesh;
            np.NpcType = (Role.Flags.NpcType)Type;
            np.AllowDynamic = AllowDynamic;
        }

        public override string ToString()
        {
            return Uid + " @" + Map + " (" + X + "," + Y + ") mesh " + Mesh + " type " + Type;
        }
    }

    /// <summary>One difference between the recorded NPCs and the file.</summary>
    public sealed class NpcChange
    {
        public const string Added = "added", Removed = "removed", Moved = "moved", Changed = "changed";
        public string Kind;
        public NpcEntry Old;   // null for added
        public NpcEntry New;   // null for removed
        public override string ToString()
        {
            switch (Kind)
            {
                case Added: return "added   " + New + " (line " + New.Line + ")";
                case Removed: return "removed " + Old;
                case Moved: return "moved   " + Old.Uid + " " + Old.Map + "(" + Old.X + "," + Old.Y + ") -> " + New.Map + "(" + New.X + "," + New.Y + ")" + (Old.SameLook(New) ? "" : " + look changed");
                default: return "changed " + Old.Uid + " @" + Old.Map + ": mesh " + Old.Mesh + "->" + New.Mesh + ", type " + Old.Type + "->" + New.Type + (Old.AllowDynamic != New.AllowDynamic ? ", dynamic " + Old.AllowDynamic + "->" + New.AllowDynamic : "");
            }
        }
    }

    /// <summary>Npcs.txt parsed the way NpcServer.LoadNpcs reads it.</summary>
    public sealed class NpcFile
    {
        public readonly Dictionary<uint, Dictionary<uint, NpcEntry>> ByMap = new Dictionary<uint, Dictionary<uint, NpcEntry>>();
        public int Lines, Npcs, Skipped, Duplicates, IgnoredAfterBlank, BlankLine;
        /// <summary>map id -> note, for lines whose id is repeated on the same map.</summary>
        public readonly List<KeyValuePair<uint, string>> DuplicateNotes = new List<KeyValuePair<uint, string>>();

        public Dictionary<uint, NpcEntry> For(uint map)
        {
            Dictionary<uint, NpcEntry> d;
            return ByMap.TryGetValue(map, out d) ? d : new Dictionary<uint, NpcEntry>();
        }
    }

    public static class NpcHotReload
    {
        /// <summary>map id -> (npc id -> entry): what Npcs.txt put on that loaded map (first line wins).</summary>
        private static readonly ConcurrentDictionary<uint, Dictionary<uint, NpcEntry>> _loaded =
            new ConcurrentDictionary<uint, Dictionary<uint, NpcEntry>>();

        /// <summary>Above this share of removed NPCs (on loaded maps) the reload refuses unless forced.</summary>
        public const double MaxRemoveShare = 0.5;

        // ------------------------------------------------------------------ record (called by LoadNpcs)
        /// <summary>Called by NpcServer.LoadNpcs(id) with the NPCs it put on the map(s).</summary>
        public static void RecordLoaded(uint id, List<Game.MsgNpc.Npc> loaded)
        {
            try
            {
                var byMap = new Dictionary<uint, Dictionary<uint, NpcEntry>>();
                if (id != 0)
                    byMap[id] = new Dictionary<uint, NpcEntry>();
                else
                    foreach (uint m in HotReloadManager.LoadedMapIds())
                        byMap[m] = new Dictionary<uint, NpcEntry>();
                foreach (var np in loaded)
                {
                    Dictionary<uint, NpcEntry> d;
                    if (!byMap.TryGetValue(np.Map, out d))
                        byMap[np.Map] = d = new Dictionary<uint, NpcEntry>();
                    if (!d.ContainsKey(np.UID)) // GameMap.AddNpc keeps the first one of a repeated id
                        d[np.UID] = NpcEntry.From(np, 0);
                }
                foreach (var kv in byMap)
                    _loaded[kv.Key] = kv.Value;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        /// <summary>Test/diagnostic access to the record of one map (null = map never loaded its NPCs).</summary>
        public static Dictionary<uint, NpcEntry> Recorded(uint map)
        {
            Dictionary<uint, NpcEntry> d;
            return _loaded.TryGetValue(map, out d) ? d : null;
        }

        // ------------------------------------------------------------------ parse
        /// <summary>Parses Npcs.txt text exactly like LoadNpcs (stops at the first blank line). Throws HotReloadError on a bad line.</summary>
        public static NpcFile Parse(TextReader reader)
        {
            var file = new NpcFile();
            int lineNo = 0;
            while (true)
            {
                string aline = reader.ReadLine();
                if (aline == null)
                    break;
                lineNo++;
                if (aline == "")
                {
                    // LoadNpcs stops at the first empty line; count what a restart would ignore too.
                    file.BlankLine = lineNo;
                    string rest;
                    while ((rest = reader.ReadLine()) != null)
                        if (rest.Trim().Length > 0) file.IgnoredAfterBlank++;
                    break;
                }
                file.Lines++;
                Game.MsgNpc.Npc np;
                try
                {
                    np = Database.NpcServer.ParseNpcLine(aline.Split(','));
                }
                catch (Exception e)
                {
                    string shown = aline.Length > 80 ? aline.Substring(0, 80) + "..." : aline;
                    throw new HotReloadError("Npcs.txt line " + lineNo + " is invalid ('" + shown + "': " + e.Message +
                                             "). Nothing was changed - fix the line (a restart would also fail to load that map's NPCs).");
                }
                if (np == null)
                {
                    file.Skipped++;
                    continue;
                }
                Dictionary<uint, NpcEntry> d;
                if (!file.ByMap.TryGetValue(np.Map, out d))
                    file.ByMap[np.Map] = d = new Dictionary<uint, NpcEntry>();
                if (d.ContainsKey(np.UID))
                {
                    file.Duplicates++;
                    file.DuplicateNotes.Add(new KeyValuePair<uint, string>(np.Map, "line " + lineNo + ": id " + np.UID + " is already on map " + np.Map + " (line " + d[np.UID].Line + ") - the server keeps the first one"));
                    continue;
                }
                d[np.UID] = NpcEntry.From(np, lineNo);
                file.Npcs++;
            }
            return file;
        }

        // ------------------------------------------------------------------ diff
        /// <summary>Differences between the recorded NPCs and the file, for the given maps only.</summary>
        public static List<NpcChange> Diff(Func<uint, Dictionary<uint, NpcEntry>> recorded, NpcFile file, IEnumerable<uint> maps)
        {
            var removed = new List<NpcChange>();
            var added = new List<NpcChange>();
            var result = new List<NpcChange>();
            foreach (uint map in maps)
            {
                var before = recorded(map);
                if (before == null) continue;
                var after = file.For(map);
                foreach (var old in before.Values)
                {
                    NpcEntry neu;
                    if (!after.TryGetValue(old.Uid, out neu))
                        removed.Add(new NpcChange { Kind = NpcChange.Removed, Old = old });
                    else if (old.X != neu.X || old.Y != neu.Y)
                        result.Add(new NpcChange { Kind = NpcChange.Moved, Old = old, New = neu });
                    else if (!old.SameLook(neu))
                        result.Add(new NpcChange { Kind = NpcChange.Changed, Old = old, New = neu });
                }
                foreach (var neu in after.Values)
                    if (!before.ContainsKey(neu.Uid))
                        added.Add(new NpcChange { Kind = NpcChange.Added, New = neu });
            }
            // An id that left exactly one map and arrived on exactly one other map = moved between maps.
            var remById = removed.GroupBy(c => c.Old.Uid).Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => g.First());
            var addById = added.GroupBy(c => c.New.Uid).Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => g.First());
            foreach (uint uid in remById.Keys.ToList())
            {
                NpcChange a;
                if (!addById.TryGetValue(uid, out a)) continue;
                NpcChange r = remById[uid];
                removed.Remove(r);
                added.Remove(a);
                result.Add(new NpcChange { Kind = NpcChange.Moved, Old = r.Old, New = a.New });
            }
            // removals first (frees cells/ids), then moves/changes, then additions
            var ordered = new List<NpcChange>();
            ordered.AddRange(removed);
            ordered.AddRange(result);
            ordered.AddRange(added);
            return ordered;
        }

        // ------------------------------------------------------------------ reload
        /// <summary>Re-reads Npcs.txt and applies the differences to every loaded map. Call under HotReloadManager.Sync.</summary>
        public static void Reload(ReloadResult result, bool force)
        {
            string path = HotReloadManager.DbFile("npcs.txt");
            byte[] data = HotReloadManager.ReadStable(path);
            NpcFile file;
            using (var reader = HotReloadManager.TextOf(data))
                file = Parse(reader);
            if (file.Npcs == 0)
                throw new HotReloadError("Npcs.txt has no NPC lines (empty or starts with a blank line) - not applied.");
            if (file.IgnoredAfterBlank > 0)
                result.Note("WARNING: blank line at line " + file.BlankLine + " - the server ignores the " + file.IgnoredAfterBlank + " line(s) after it (same as at startup). Remove the blank line.");

            // Which maps: loaded ones that recorded their NPCs. (A map that is loading right now records
            // only when it finishes; it reads the new file itself.)
            var maps = new List<uint>();
            int skippedMaps = 0;
            foreach (uint id in HotReloadManager.LoadedMapIds())
            {
                if (_loaded.ContainsKey(id)) maps.Add(id);
                else skippedMaps++;
            }

            if (file.Duplicates > 0)
            {
                result.Note(file.Duplicates + " line(s) repeat an id on the same map (the server keeps the first one of each); on loaded maps:");
                foreach (var kv in file.DuplicateNotes)
                    if (maps.Contains(kv.Key)) result.Note("  " + kv.Value);
            }

            List<NpcChange> changes = Diff(m => Recorded(m), file, maps);
            int recordedTotal = maps.Sum(m => { var d = Recorded(m); return d == null ? 0 : d.Count; });
            int removing = changes.Count(c => c.Kind == NpcChange.Removed);
            if (!force && recordedTotal >= 20 && removing > recordedTotal * MaxRemoveShare)
                throw new HotReloadError("the new Npcs.txt would remove " + removing + " of the " + recordedTotal +
                                         " NPCs on the loaded maps - refused as a safety check (truncated file?). Use 'reloadnpcs force' if that is really intended.");

            var cells = new MapCells();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (NpcChange c in changes)
                {
                    try
                    {
                        Apply(c, stream, cells, result);
                    }
                    catch (Exception e)
                    {
                        result.Add("errors");
                        result.Note("error on " + c + ": " + e.Message);
                        MyConsole.SaveException(e);
                    }
                }
            }

            // The record now matches the file for every map we processed.
            foreach (uint m in maps)
                _loaded[m] = new Dictionary<uint, NpcEntry>(file.For(m));

            foreach (NpcChange c in changes)
                result.Note(c.ToString());

            int added = result.Get(NpcChange.Added), removed = result.Get(NpcChange.Removed), moved = result.Get(NpcChange.Moved), changed = result.Get(NpcChange.Changed);
            result.Counts["loadedMaps"] = maps.Count;
            result.Counts["fileNpcs"] = file.Npcs;
            if (added + removed + moved + changed == 0 && result.Get("errors") == 0)
                result.Message = "No NPC changes on the " + maps.Count + " loaded map(s) (" + file.Npcs + " NPCs in Npcs.txt).";
            else
            {
                var parts = new List<string>();
                if (moved > 0) parts.Add(moved + " moved");
                if (added > 0) parts.Add(added + " added");
                if (removed > 0) parts.Add(removed + " removed");
                if (changed > 0) parts.Add(changed + " changed");
                if (result.Get("conflicts") > 0) parts.Add(result.Get("conflicts") + " skipped (id already on the map)");
                if (result.Get("errors") > 0) parts.Add(result.Get("errors") + " errors");
                result.Message = "Applied live: " + (parts.Count == 0 ? "nothing" : string.Join(", ", parts)) + ".";
            }
            if (skippedMaps > 0)
                result.Note(skippedMaps + " loaded map(s) have no NPC record yet (still loading) - they read Npcs.txt themselves.");
            result.Note("Maps that are not loaded yet will use the new Npcs.txt when a player first enters them.");
        }

        private static void Apply(NpcChange c, ServerSockets.Packet stream, MapCells cells, ReloadResult result)
        {
            switch (c.Kind)
            {
                case NpcChange.Removed:
                    {
                        var map = HotReloadManager.LoadedMap(c.Old.Map);
                        var npc = map == null ? null : Find(map, c.Old);
                        if (npc != null)
                            Remove(map, npc, stream, cells);
                        else
                            result.Note("removed " + c.Old.Uid + " @" + c.Old.Map + ": it was not on the map anymore (an event may have removed it).");
                        result.Add(NpcChange.Removed);
                        break;
                    }
                case NpcChange.Added:
                    {
                        if (Add(c.New, stream, result))
                            result.Add(NpcChange.Added);
                        break;
                    }
                default: // moved / changed
                    {
                        var oldMap = HotReloadManager.LoadedMap(c.Old.Map);
                        var npc = oldMap == null ? null : Find(oldMap, c.Old);
                        if (c.Old.Map == c.New.Map && npc != null && oldMap != null)
                        {
                            // same map: take the SAME object off, update it, put it back (references stay valid)
                            Remove(oldMap, npc, stream, cells);
                            c.New.Apply(npc);
                            oldMap.AddNpc(npc);
                            result.Add(c.Kind);
                        }
                        else
                        {
                            if (npc != null)
                                Remove(oldMap, npc, stream, cells);
                            if (Add(c.New, stream, result))
                                result.Add(c.Kind);
                        }
                        break;
                    }
            }
        }

        private static bool Add(NpcEntry e, ServerSockets.Packet stream, ReloadResult result)
        {
            var map = HotReloadManager.LoadedMap(e.Map);
            if (map == null)
                return false; // not loaded: it reads the file when it loads
            if (map.View.MapContain(Role.MapObjectType.Npc, e.Uid, p => p.DynamicID == 0))// [InstanceNpc] copies in private instances don't count
            {
                result.Add("conflicts");
                result.Note("id " + e.Uid + " is already on map " + e.Map + " (created at runtime, not from Npcs.txt) - not added.");
                return false;
            }
            map.AddNpc(e.CreateNpc());
            return true;
        }

        private static void Remove(Role.GameMap map, Game.MsgNpc.Npc npc, ServerSockets.Packet stream, MapCells cells)
        {
            ushort x = npc.X, y = npc.Y;
            map.RemoveNpc(npc, stream); // leaves the view + RemoveEntity to the players who see it
            cells.RestoreAround(map, x, y);
        }

        /// <summary>The live Npc for a recorded entry: near its recorded spot first, then anywhere on the map.</summary>
        private static Game.MsgNpc.Npc Find(Role.GameMap map, NpcEntry e)
        {
            Role.IMapObj obj;
            if (map.View.TryGetObject<Role.IMapObj>(e.Uid, Role.MapObjectType.Npc, e.X, e.Y, out obj) && obj is Game.MsgNpc.Npc && obj.DynamicID == 0)
                return (Game.MsgNpc.Npc)obj;
            foreach (var o in map.View.GetAllMapRoles(Role.MapObjectType.Npc, p => p.UID == e.Uid && p.DynamicID == 0))// [InstanceNpc] never a private copy
                if (o is Game.MsgNpc.Npc)
                    return (Game.MsgNpc.Npc)o;
            return null;
        }
    }
}
