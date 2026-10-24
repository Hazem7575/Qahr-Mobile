// ============================================================================
//  QahrMobile: the Chinju bounty quests the quest engine cannot judge (Message Delivery, Rescue / Free Hostages /
//  Supply Delivery, Flower / Herb / Water / Gem / Supply / Fish / Spirit picking). Used by MobileQuestBounty.
//
//  Goals come from the quest's own Intention lines (the mobile questinfo.ini syntax):
//      <verb> [lua X,</F>TaskAssemblyGfit_Find][chk_quest id,1,slot,N]  talk to NPC X (N = 1) / rescue N different X
//      <verb> [lua X,</F>TaskAssemblyGfit_Find][chk_item item,N]        pick N items from X objects
//  X is mapped by Data\mobile_quests\bounty_goals.txt to an NPC uid or to a group of "objects".
//  The objects are the mobile's own scene-object NPC types (npc.ini NpcType4436-4446: TheWounded, Hostage,
//  DesertRefugee, Flower, Herb, WaterSpot, DiggingSpot, SupplySpot, FishingSpot, FearlessSpirit), placed at run time
//  on walkable cells (no Npcs.txt change needed; bounty_npcs.txt has the same NPCs as Npcs.txt lines).
//  Talk / rescue progress is the quest's 2339 counter (QuestGUI.SetQuestObjectives); picks are real bag items
//  (the mobile checks chk_item on the bag). The missing quest items (Data\mobile_quests\bounty_items.txt) are added
//  to the live item table (copy + swap of Pool.ItemsBase, so readers never see a half-changed dictionary).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.Phone
{
    public static class MobileBountyGoals
    {
        public enum Kind : byte { Talk = 1, Rescue = 4, Pick = 3 }

        public sealed class Step
        {
            public string Target = "";  // lua name
            public byte Slot;           // chk_quest slot (talk / rescue)
            public uint Count = 1;
            public uint Item;           // pick
        }
        public sealed class Goal
        {
            public uint Id;
            public Kind Kind;
            public List<Step> Steps = new List<Step>();
            public uint Total { get { uint n = 0; foreach (var s in Steps) n += s.Count; return n; } }
        }
        sealed class Target { public string Name = ""; public uint Npc; public string Object; }
        sealed class ObjGroup
        {
            public string Name = ""; public uint Look, Map, FirstUid, Item; public ushort X, Y, Radius; public int Count;
            public List<Game.MsgNpc.Npc> Placed = new List<Game.MsgNpc.Npc>();
        }
        sealed class NpcDef { public uint Uid, Mesh, Map; public ushort X, Y; public string Name = ""; }

        sealed class Data
        {
            public Dictionary<string, Target> Targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, ObjGroup> Objects = new Dictionary<string, ObjGroup>(StringComparer.OrdinalIgnoreCase);
            public List<NpcDef> Npcs = new List<NpcDef>();
            public Dictionary<uint, ObjGroup> ByUid = new Dictionary<uint, ObjGroup>();
        }
        static volatile Data data; static string dataStamp = "";
        static int version; static long dataChecked;
        /// <summary>Changes when bounty_goals.txt was re-read (MobileQuestBounty rebuilds its offer pool then).</summary>
        public static int Version { get { var d = D; return version; } }
        static readonly object placeLock = new object();
        static Dictionary<uint, Goal> goals; static object goalsOf;
        static readonly ConcurrentDictionary<ulong, HashSet<uint>> used = new ConcurrentDictionary<ulong, HashSet<uint>>();// (player, quest) -> object uids already used
        static readonly ConcurrentDictionary<uint, long> lastClick = new ConcurrentDictionary<uint, long>();

        static string Dir { get { return Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests"); } }

        // ------------------------------------------------------------------ data file
        static Data D
        {
            get
            {
                var d = data;
                long now = Environment.TickCount64;
                if (d != null && now - dataChecked < 2000) return d;// the file is looked at every 2 s at most
                dataChecked = now;
                string f = Path.Combine(Dir, "bounty_goals.txt");
                string stamp = "none";
                try { var fi = new FileInfo(f); if (fi.Exists) stamp = fi.Length + ":" + fi.LastWriteTimeUtc.Ticks; } catch (Exception err) { MyConsole.SaveException(err); }
                if (d != null && stamp == dataStamp) return d;
                d = new Data();
                try
                {
                    if (File.Exists(f))
                        foreach (var raw in File.ReadAllLines(f))
                        {
                            string line = raw; int c = line.IndexOf('#'); if (c >= 0) line = line.Substring(0, c);
                            var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (p.Length == 0) continue;
                            switch (p[0].ToLowerInvariant())
                            {
                                case "target":
                                    if (p.Length >= 4) d.Targets[p[1]] = new Target { Name = p[1], Npc = p[2] == "npc" ? U(p[3]) : 0, Object = p[2] == "object" ? p[3] : null };
                                    break;
                                case "npc":
                                    if (p.Length >= 7) d.Npcs.Add(new NpcDef { Uid = U(p[1]), Mesh = U(p[2]), Map = U(p[3]), X = (ushort)U(p[4]), Y = (ushort)U(p[5]), Name = p[6] });
                                    break;
                                case "object":
                                    if (p.Length >= 9)
                                    {
                                        var g = new ObjGroup { Name = p[1], Look = U(p[2]), Map = U(p[3]), X = (ushort)U(p[4]), Y = (ushort)U(p[5]), Radius = (ushort)U(p[6]), Count = (int)U(p[7]), FirstUid = U(p[8]), Item = p.Length > 9 ? U(p[9]) : 0 };
                                        d.Objects[g.Name] = g;
                                    }
                                    break;
                            }
                        }
                }
                catch (Exception e) { MyConsole.WriteLine("[Mobile] bounty_goals.txt: " + e.Message, ConsoleColor.Red); }
                // keep the objects already on the map when the file is re-read (same group name + first uid)
                var old = data;
                if (old != null)
                    foreach (var g in d.Objects.Values)
                    {
                        ObjGroup o;
                        if (old.Objects.TryGetValue(g.Name, out o) && o.FirstUid == g.FirstUid && o.Map == g.Map && o.X == g.X && o.Y == g.Y && o.Count == g.Count && o.Look == g.Look) g.Placed = o.Placed;
                        else if (o != null) Remove(o);
                    }
                foreach (var g in d.Objects.Values) for (int i = 0; i < g.Count; i++) d.ByUid[g.FirstUid + (uint)i] = g;
                data = d; dataStamp = stamp; goals = null; version++;
                return d;
            }
        }

        // ------------------------------------------------------------------ goals of the quests
        static readonly Regex StepRx = new Regex(@"\[lua ([^,\]]+),</F>TaskAssemblyGfit_Find\]\s*\[chk_(quest|item) (\d+),(\d+)(?:,(\d+),(\d+))?\]", RegexOptions.Compiled);

        /// <summary>The goal of a Chinju quest when every one of its steps is a talk / rescue / pick this class handles (else null).</summary>
        public static Goal GoalOf(uint id)
        {
            var t = QuestEngine.QuestEngineCore.Current;
            var d = D;
            var g = goals;
            if (g == null || !ReferenceEquals(goalsOf, t))
            {
                g = new Dictionary<uint, Goal>();
                if (t != null)
                    foreach (var q in t.Quests.Values)
                    {
                        if (q.BeginNpc != MobileQuestBounty.ChinjuNpc || q.TypeId != 3) continue;
                        var gl = Parse(q, d);
                        if (gl != null) g[q.MissionId] = gl;
                    }
                goals = g; goalsOf = t;
            }
            Goal r;
            return g.TryGetValue(id, out r) ? r : null;
        }

        static Goal Parse(QuestEngine.EngineQuest q, Data d)
        {
            string text = q.IntentionText ?? "";
            var ms = StepRx.Matches(text);
            if (ms.Count == 0) return null;
            // every "[lua X,</F>TaskAssemblyGfit_Find]" of the quest must be one of the matched steps
            if (Regex.Matches(text, @"TaskAssemblyGfit_Find").Count != ms.Count) return null;
            var gl = new Goal { Id = q.MissionId };
            foreach (Match m in ms)
            {
                Target tg;
                if (!d.Targets.TryGetValue(m.Groups[1].Value.Trim(), out tg)) return null;
                var st = new Step { Target = tg.Name };
                Kind k;
                if (m.Groups[2].Value == "item")
                {
                    if (tg.Object == null) return null;
                    ObjGroup og;
                    if (!d.Objects.TryGetValue(tg.Object, out og) || og.Item == 0 || og.Item != U(m.Groups[3].Value)) return null;
                    st.Item = og.Item; st.Count = Math.Max(1, U(m.Groups[4].Value)); k = Kind.Pick;
                    if (st.Count > og.Count) return null;// not enough objects to pick from
                }
                else
                {
                    if (U(m.Groups[3].Value) != q.MissionId || m.Groups[6].Value.Length == 0) return null;
                    st.Slot = (byte)U(m.Groups[5].Value); st.Count = Math.Max(1, U(m.Groups[6].Value));
                    if (st.Slot == 0) return null;
                    if (tg.Npc != 0) { if (st.Count != 1) return null; k = Kind.Talk; }
                    else
                    {
                        ObjGroup og;
                        if (tg.Object == null || !d.Objects.TryGetValue(tg.Object, out og) || st.Count > og.Count) return null;
                        k = Kind.Rescue;
                    }
                }
                if (gl.Steps.Count > 0 && gl.Kind != k) return null;// mixed kinds: not handled
                gl.Kind = k;
                gl.Steps.Add(st);
            }
            return gl;
        }

        // ------------------------------------------------------------------ progress
        static uint[] Progress(Client.GameClient c, uint id) { uint[] cur; c.Player.QuestGUI.GetQuestObjectives(id, out cur); return cur ?? new uint[0]; }
        static uint At(uint[] cur, byte slot) { return slot >= 1 && slot <= cur.Length ? cur[slot - 1] : 0; }
        static uint Have(Client.GameClient c, Goal g, Step s)
        {
            if (g.Kind == Kind.Pick) { try { return (uint)Math.Max(0, c.Inventory.GetCountItem(s.Item)); } catch (Exception err) { MyConsole.SaveException(err); return 0; } }
            return At(Progress(c, g.Id), s.Slot);
        }
        /// <summary>A placed bounty object (Hostage, Flower, FishingSpot ...) is only for players who have a running
        /// bounty quest with a step on that object group; everyone else does not get it (MobileQuestMarkers.Hide).</summary>
        public static bool IsHiddenObject(Client.GameClient c, uint npcUid)
        {
            var d = D;
            ObjGroup group;
            if (d == null || !d.ByUid.TryGetValue(npcUid, out group))
                return false;// not a bounty object
            GoalOf(0);// builds the goal table
            var all = goals;
            if (all == null || c == null || c.Player == null || c.Player.QuestGUI == null)
                return true;
            foreach (var kv in all)
            {
                if (!c.Player.QuestGUI.IsActiveQuest(kv.Key))
                    continue;
                foreach (var s in kv.Value.Steps)
                {
                    Target t;
                    if (d.Targets.TryGetValue(s.Target, out t) && string.Equals(t.Object, group.Name, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }
            return true;
        }

        public static bool IsDone(Client.GameClient c, Goal g)
        {
            if (g == null || c == null || c.Player == null) return false;
            foreach (var s in g.Steps) if (Have(c, g, s) < s.Count) return false;
            return true;
        }
        public static string Left(Client.GameClient c, Goal g)
        {
            return string.Join(", ", g.Steps.Select(s => s.Target + " " + Math.Min(Have(c, g, s), s.Count) + "/" + s.Count));
        }

        /// <summary>Accept: a fresh run (counters 0, no used objects).</summary>
        public static void OnAccept(Client.GameClient c, Goal g)
        {
            HashSet<uint> h; used.TryRemove(Key(c, g.Id), out h);
            EnsurePlaced();
        }
        /// <summary>Complete: the picked items are handed in.</summary>
        public static void HandIn(Client.GameClient c, Goal g, ServerSockets.Packet stream)
        {
            HashSet<uint> h; used.TryRemove(Key(c, g.Id), out h);
            if (g.Kind != Kind.Pick) return;
            foreach (var s in g.Steps) try { c.Inventory.Remove(s.Item, s.Count, stream); } catch (Exception err) { MyConsole.SaveException(err); }
        }
        static ulong Key(Client.GameClient c, uint id) { return ((ulong)c.Player.UID << 32) | id; }

        // ------------------------------------------------------------------ NPC / object clicks
        /// <summary>QuestEngineCore.OnNpc (through MobileQuestBounty.OnNpc). True = answered.</summary>
        public static bool OnNpc(Client.GameClient c, uint npcid)
        {
            if (c == null || c.Player == null) return false;
            EnsurePlaced();// re-places the objects after bounty_goals.txt changed (cheap when nothing changed)
            var d = D;
            ObjGroup og; d.ByUid.TryGetValue(npcid, out og);
            bool isTalkNpc = og == null && d.Targets.Values.Any(t => t.Npc == npcid);
            if (og == null && !isTalkNpc) return false;
            long now = Environment.TickCount64, last;
            if (og != null && lastClick.TryGetValue(c.Player.UID, out last) && now - last < 1000) return true;// a double tap
            lastClick[c.Player.UID] = now;

            var gui = c.Player.QuestGUI;
            foreach (var id in ActiveGoals(c))
            {
                var g = GoalOf(id);
                if (g == null) continue;
                foreach (var s in g.Steps)
                {
                    Target tg; if (!d.Targets.TryGetValue(s.Target, out tg)) continue;
                    if (og == null ? tg.Npc != npcid : !string.Equals(tg.Object, og.Name, StringComparison.OrdinalIgnoreCase)) continue;
                    uint have = Have(c, g, s);
                    if (have >= s.Count) continue;
                    var set = used.GetOrAdd(Key(c, id), _ => new HashSet<uint>());
                    string text;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        lock (set)
                        {
                            if (og != null && set.Contains(npcid)) { Dialog(c, Already(g.Kind)); return true; }
                            if (g.Kind == Kind.Pick)
                            {
                                if (!c.Inventory.HaveSpace(1)) { Dialog(c, "Please make 1 free space in your inventory first."); return true; }
                                if (!c.Inventory.AddItemWitchStack(s.Item, 0, 1, stream, true)) { Dialog(c, "Please make 1 free space in your inventory first."); return true; }
                                text = Verb(og) + " " + ItemName(s.Item) + " (" + (have + 1) + "/" + s.Count + ").";
                            }
                            else
                            {
                                var cur = Progress(c, id);
                                var arr = new uint[Math.Max(cur.Length, (int)g.Steps.Max(x => x.Slot))];
                                Array.Copy(cur, arr, cur.Length);
                                arr[s.Slot - 1] = Math.Min(s.Count, have + 1);
                                gui.SetQuestObjectives(stream, id, arr);// 2339 -> the tracker shows n/N
                                text = g.Kind == Kind.Talk ? "You delivered Chinju's message to " + Spaced(s.Target) + "."
                                                           : "You helped " + Spaced(s.Target) + " (" + (have + 1) + "/" + s.Count + ").";
                            }
                            if (og != null) set.Add(npcid);
                        }
                    }
                    if (IsDone(c, g)) text += " Go back to Chinju in Twin City to claim the bounty.";
                    Log(c, id, "STEP " + s.Target + " npc " + npcid + " -> " + Left(c, g));
                    Dialog(c, text);
                    return true;
                }
            }
            if (og == null) return false;// a talk NPC with nothing to deliver: its normal dialog
            Dialog(c, og.Item != 0 ? "A " + Spaced(og.Name) + ". Accept a bounty at Chinju (Twin City 441,290) to gather it." : Spaced(og.Name) + ": \"Please help us...\" (a Chinju bounty quest)");
            return true;
        }
        static string Already(Kind k) { return k == Kind.Pick ? "There is nothing left here. Try another spot." : "You have already helped this one. Look for another."; }
        static string Verb(ObjGroup og)
        {
            switch (og.Name.ToLowerInvariant())
            {
                case "waterspot": return "You fetched";
                case "diggingspot": return "You dug out";
                case "fishingspot": return "You caught";
                case "supplyspot": case "fearlessspirit": return "You collected";
                default: return "You picked";
            }
        }
        static IEnumerable<uint> ActiveGoals(Client.GameClient c)
        {
            var gui = c.Player.QuestGUI;
            var g = goals ?? new Dictionary<uint, Goal>();
            if (goals == null) GoalOf(0);
            g = goals ?? g;
            var list = new List<uint>();
            foreach (var id in g.Keys) if (gui.IsActiveQuest(id)) list.Add(id);
            return list;
        }

        // ------------------------------------------------------------------ "Go"
        /// <summary>Auto-walk to the nearest thing the quest still needs.</summary>
        public static bool Find(Client.GameClient c, Goal g, string questName)
        {
            var d = D;
            EnsurePlaced();
            uint bestMap = 0, bestNpc = 0; ushort bx = 0, by = 0; int best = int.MaxValue; string what = "";
            HashSet<uint> set; used.TryGetValue(Key(c, g.Id), out set);
            foreach (var s in g.Steps)
            {
                if (Have(c, g, s) >= s.Count) continue;
                Target tg; if (!d.Targets.TryGetValue(s.Target, out tg)) continue;
                var spots = new List<Tuple<uint, uint, ushort, ushort>>();// uid map x y
                if (tg.Npc != 0)
                {
                    uint m; ushort x, y;
                    if (NpcPlace(tg.Npc, out m, out x, out y)) spots.Add(Tuple.Create(tg.Npc, m, x, y));
                }
                else
                {
                    ObjGroup og;
                    if (tg.Object != null && d.Objects.TryGetValue(tg.Object, out og))
                        foreach (var n in og.Placed.ToArray())
                            if (set == null || !set.Contains(n.UID)) spots.Add(Tuple.Create(n.UID, n.Map, n.X, n.Y));
                }
                foreach (var sp in spots)
                {
                    int dist = sp.Item2 == c.Player.Map ? Math.Max(Math.Abs(sp.Item3 - c.Player.X), Math.Abs(sp.Item4 - c.Player.Y)) : 100000;
                    if (dist < best) { best = dist; bestMap = sp.Item2; bx = sp.Item3; by = sp.Item4; bestNpc = sp.Item1; what = Spaced(s.Target); }
                }
            }
            if (bestMap == 0) return false;
            c.SendSysMesage("[Bounty] " + questName + ": " + what + " is at " + (bestMap != c.Player.Map ? "map " + bestMap + " " : "") + bx + "," + by + ".", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            if (best > 2) c.Player.QuestGUI.SendAutoPatcher(bestMap, bx, by, bestNpc);
            return true;
        }
        static bool NpcPlace(uint uid, out uint map, out ushort x, out ushort y)
        {
            map = 0; x = y = 0;
            var d = D;
            var nd = d.Npcs.FirstOrDefault(n => n.Uid == uid);
            if (nd != null) { map = nd.Map; x = nd.X; y = nd.Y; }
            var p = placedNpcs.FirstOrDefault(n => n.UID == uid);
            if (p != null) { map = p.Map; x = p.X; y = p.Y; return true; }
            if (nd != null) return true;
            Tuple<uint, ushort, ushort> at;
            if (NpcsTxt().TryGetValue(uid, out at)) { map = at.Item1; x = at.Item2; y = at.Item3; return true; }
            return false;
        }
        static Dictionary<uint, Tuple<uint, ushort, ushort>> npcsTxt; static string npcsStamp = "";
        /// <summary>uid -> map, x, y from Data\Npcs.txt (re-read when it changes).</summary>
        static Dictionary<uint, Tuple<uint, ushort, ushort>> NpcsTxt()
        {
            string f = Path.Combine(Program.ServerConfig.DbLocation, "Npcs.txt");
            string stamp = "none";
            try { var fi = new FileInfo(f); if (fi.Exists) stamp = fi.Length + ":" + fi.LastWriteTimeUtc.Ticks; } catch (Exception err) { MyConsole.SaveException(err); }
            var d = npcsTxt;
            if (d != null && stamp == npcsStamp) return d;
            d = new Dictionary<uint, Tuple<uint, ushort, ushort>>();
            try
            {
                foreach (var line in File.ReadAllLines(f))
                {
                    var p = line.Split(',');
                    if (p.Length < 6) continue;
                    uint uid = U(p[0]);
                    if (uid != 0 && !d.ContainsKey(uid)) d[uid] = Tuple.Create(U(p[3]), (ushort)U(p[4]), (ushort)U(p[5]));
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            npcsTxt = d; npcsStamp = stamp;
            return d;
        }

        // ------------------------------------------------------------------ placing the objects / NPCs
        static readonly List<Game.MsgNpc.Npc> placedNpcs = new List<Game.MsgNpc.Npc>();
        static volatile bool itemsDone;

        /// <summary>Places the objects and the extra talk NPCs (once; again after bounty_goals.txt changes) and adds the items.</summary>
        public static void EnsurePlaced()
        {
            try
            {
                EnsureItems();
                var d = D;
                bool changed = false;
                lock (placeLock)
                {
                    foreach (var nd in d.Npcs)
                    {
                        var was = placedNpcs.FirstOrDefault(n => n.UID == nd.Uid);
                        if (was != null)
                        {
                            // keep it unless bounty_goals.txt moved it (or it was put on a bad cell)
                            if (was.Map == nd.Map && was.Mesh == nd.Mesh && was.X != 0 && Math.Max(Math.Abs(was.X - nd.X), Math.Abs(was.Y - nd.Y)) <= 8) continue;
                            var wm = GetMap(was.Map);
                            using (var rec = new ServerSockets.RecycledPacket()) { if (wm != null) wm.RemoveNpc(was, rec.GetStream()); }
                            placedNpcs.Remove(was);
                        }
                        var map = GetMap(nd.Map);
                        if (map == null) continue;
                        if (map.View.MapContain(Role.MapObjectType.Npc, nd.Uid, p => p.DynamicID == 0)) { MyConsole.WriteLine("[Mobile] bounty npc " + nd.Uid + " is already on map " + nd.Map, ConsoleColor.DarkGray); continue; }
                        ushort x = nd.X, y = nd.Y;
                        if ((!map.ValidLocation(x, y) || NearOtherNpc(map, x, y)) && !FreeCell(map, nd.X, nd.Y, 20, new List<Tuple<ushort, ushort>>(), out x, out y))
                        { MyConsole.WriteLine("[Mobile] bounty npc " + nd.Uid + ": no walkable cell near " + nd.Map + " " + nd.X + "," + nd.Y, ConsoleColor.Yellow); continue; }
                        var np = Make(nd.Uid, nd.Mesh, nd.Map, x, y);
                        map.AddNpc(np);
                        placedNpcs.Add(np); changed = true;
                    }
                    foreach (var g in d.Objects.Values)
                    {
                        if (g.Placed.Count >= g.Count) continue;
                        var map = GetMap(g.Map);
                        if (map == null) continue;
                        var taken = g.Placed.Select(n => Tuple.Create(n.X, n.Y)).ToList();
                        for (int i = g.Placed.Count; i < g.Count; i++)
                        {
                            uint uid = g.FirstUid + (uint)i;
                            ushort x, y;
                            if (map.View.MapContain(Role.MapObjectType.Npc, uid, p => p.DynamicID == 0))
                            {
                                // already there (merged into Npcs.txt): remember where, for "Go"
                                Tuple<uint, ushort, ushort> at;
                                if (NpcsTxt().TryGetValue(uid, out at)) { g.Placed.Add(Make(uid, g.Look * 10, at.Item1, at.Item2, at.Item3)); taken.Add(Tuple.Create(at.Item2, at.Item3)); }
                                continue;
                            }
                            if (!Spot(map, g, i, taken, out x, out y)) break;
                            taken.Add(Tuple.Create(x, y));
                            var np = Make(uid, g.Look * 10, g.Map, x, y);
                            map.AddNpc(np);
                            g.Placed.Add(np); changed = true;
                        }
                    }
                }
                if (changed || !File.Exists(Path.Combine(Dir, "bounty_npcs.txt"))) WritePlaced(d);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        static Game.MsgNpc.Npc Make(uint uid, uint mesh, uint map, ushort x, ushort y)
        {
            var np = Game.MsgNpc.Npc.Create();
            np.UID = uid; np.NpcType = (Role.Flags.NpcType)2; np.Mesh = mesh; np.Map = map; np.X = x; np.Y = y;
            return np;
        }
        static void Remove(ObjGroup g)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var n in g.Placed.ToArray()) { var m = GetMap(n.Map); if (m != null) m.RemoveNpc(n, stream); }
                }
                g.Placed.Clear();
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }
        /// <summary>Object i of a group: a walkable cell on a ring around (x,y), at least 3 cells from the others.</summary>
        static bool Spot(Role.GameMap map, ObjGroup g, int i, List<Tuple<ushort, ushort>> taken, out ushort x, out ushort y)
        {
            double a = 2 * Math.PI * i / Math.Max(1, g.Count) + 0.4;
            int r = Math.Max(3, g.Radius * (i % 2 == 0 ? 2 : 3) / 3);
            int cx = g.X + (int)Math.Round(Math.Cos(a) * r), cy = g.Y + (int)Math.Round(Math.Sin(a) * r);
            return FreeCell(map, (ushort)Math.Max(0, cx), (ushort)Math.Max(0, cy), g.Radius, taken, out x, out y) || FreeCell(map, g.X, g.Y, g.Radius * 3, taken, out x, out y);
        }
        static bool FreeCell(Role.GameMap map, ushort cx, ushort cy, int max, List<Tuple<ushort, ushort>> taken, out ushort x, out ushort y)
        {
            for (int r = 0; r <= max; r++)
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r) continue;
                        int tx = cx + dx, ty = cy + dy;
                        if (tx < 1 || ty < 1 || tx > 65000 || ty > 65000) continue;
                        if (!map.ValidLocation((ushort)tx, (ushort)ty)) continue;
                        // keep the 8 neighbours walkable too (not inside a wall / on a cliff edge)
                        bool open = true;
                        for (int ox = -1; ox <= 1 && open; ox++) for (int oy = -1; oy <= 1; oy++) if (!map.ValidLocation((ushort)(tx + ox), (ushort)(ty + oy))) { open = false; break; }
                        if (!open) continue;
                        if (taken.Any(t => Math.Max(Math.Abs(t.Item1 - tx), Math.Abs(t.Item2 - ty)) < 3)) continue;
                        if (NearOtherNpc(map, tx, ty)) continue;// the mobile picks the other NPC when two are on top of each other
                        x = (ushort)tx; y = (ushort)ty; return true;
                    }
            x = y = 0; return false;
        }
        /// <summary>Another (non-bounty) NPC within 5 cells: a tap there would hit that NPC instead of the object.</summary>
        static bool NearOtherNpc(Role.GameMap map, int x, int y)
        {
            try
            {
                foreach (var o in map.View.Roles(Role.MapObjectType.Npc, x, y))
                {
                    if (o == null || (o.UID >= 9720001 && o.UID <= 9720999)) continue;
                    if (Math.Max(Math.Abs(o.X - x), Math.Abs(o.Y - y)) <= 5) return true;
                }
                Tuple<uint, ushort, ushort> at;
                foreach (var kv in NpcsTxt())
                {
                    at = kv.Value;
                    if (at.Item1 == map.ID && kv.Key < 9720001 && Math.Max(Math.Abs(at.Item2 - x), Math.Abs(at.Item3 - y)) <= 5) return true;
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            return false;
        }
        static Role.GameMap GetMap(uint id)
        {
            Role.GameMap m;
            if (Pool.ServerMaps == null) return null;
            if (Pool.ServerMaps.TryGetValue(id, out m)) return m;
            try { return Pool.ServerMaps[id]; } catch (Exception err) { MyConsole.SaveException(err); return null; }
        }
        static void WritePlaced(Data d)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("# QahrMobile: bounty objects / talk NPCs as placed by MobileBountyGoals.cs (Npcs.txt format: uid,type,mesh,map,x,y), ").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append("\r\n");
                sb.Append("# They are placed at run time; merging them into Npcs.txt is optional (MobileBountyGoals then finds them on the map and skips them).\r\n");
                foreach (var n in placedNpcs) sb.Append(n.UID).Append(",2,").Append(n.Mesh).Append(',').Append(n.Map).Append(',').Append(n.X).Append(',').Append(n.Y).Append("\r\n");
                foreach (var g in d.Objects.Values)
                {
                    sb.Append("# ").Append(g.Name).Append(" (mobile NpcType").Append(g.Look).Append(g.Item != 0 ? ", item " + g.Item : "").Append(")\r\n");
                    foreach (var n in g.Placed) sb.Append(n.UID).Append(",2,").Append(n.Mesh).Append(',').Append(n.Map).Append(',').Append(n.X).Append(',').Append(n.Y).Append("\r\n");
                }
                File.WriteAllText(Path.Combine(Dir, "bounty_npcs.txt"), sb.ToString(), new UTF8Encoding(false));
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }

        // ------------------------------------------------------------------ items
        /// <summary>Adds the bounty items the server does not know (bounty_items.txt) to Pool.ItemsBase (copy + swap).</summary>
        public static void EnsureItems()
        {
            if (itemsDone) return;
            lock (placeLock)
            {
                if (itemsDone) return;
                try
                {
                    var cur = Pool.ItemsBase;
                    string f = Path.Combine(Dir, "bounty_items.txt");
                    if (cur == null || cur.Count == 0 || !File.Exists(f)) return;
                    var add = new List<Database.ItemType.DBItem>();
                    bool prev = Database.ItemType.DBItem.SkipStaticTables;
                    Database.ItemType.DBItem.SkipStaticTables = true;
                    try
                    {
                        foreach (var line in File.ReadAllLines(f))
                        {
                            string s = line.Trim();
                            if (s.Length <= 11 || s.StartsWith("#") || s.StartsWith("//")) continue;
                            var it = new Database.ItemType.DBItem();
                            try { it.Parse(s); } catch { continue; }
                            if (!it.ParsedOk || cur.ContainsKey(it.ID)) continue;
                            add.Add(it);
                        }
                    }
                    finally { Database.ItemType.DBItem.SkipStaticTables = prev; }
                    if (add.Count > 0)
                    {
                        var copy = new Database.ItemType();
                        foreach (var kv in cur) copy[kv.Key] = kv.Value;
                        foreach (var it in add) copy[it.ID] = it;
                        Pool.ItemsBase = copy;
                        MyConsole.WriteLine("[Mobile] bounty items added to the item table: " + string.Join(", ", add.Select(x => x.ID + " " + x.Name)), ConsoleColor.Green);
                    }
                    itemsDone = true;
                }
                catch (Exception e) { MyConsole.WriteLine("[Mobile] bounty items: " + e.Message, ConsoleColor.Red); }
            }
        }
        static string ItemName(uint id) { Database.ItemType.DBItem db; return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && db != null ? db.Name : "item " + id; }

        // ------------------------------------------------------------------ helpers
        static void Dialog(Client.GameClient c, string text)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var d = new Game.MsgNpc.Dialog(c, rec.GetStream());
                    d.AddText(text).AddOption("OK.", 255).AddAvatar(10).FinalizeDialog();
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }
        static string Spaced(string s) { return Regex.Replace(s ?? "", @"(?<=[a-z])(?=[A-Z])", " "); }
        static uint U(string s) { var m = Regex.Match(s ?? "", @"\d+"); uint v; return m.Success && uint.TryParse(m.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static void Log(Client.GameClient c, uint id, string what)
        {
            try
            {
                File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | BOUNTY GOAL | " + id + " | " + c.Player.UID + " " + c.Player.Name + " | " + what + Environment.NewLine);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }
    }
}
