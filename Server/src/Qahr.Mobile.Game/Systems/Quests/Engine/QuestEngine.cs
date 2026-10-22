// ============================================================================
//  [QuestEngine] runtime: generic, data-driven quests (see QuestEngineData.cs for the data side).
//
//  Hooks (each one a single call, wrapped so an engine error never breaks the caller):
//      MonsterRole.Dead            -> OnMonsterKilled   kill counters, Gather counters, quest-item drops, kill points
//                                  -> ConsumeQuestSpawn engine-spawned story monsters drop no loot
//      Procesor (NPC in screen)    -> OnNpc             generic accept / progress / finish dialog, Talk objectives,
//                                                       npcTeleports, "summon again" (also for maintenance NPCs that
//                                                       have engine quests: HasEngineQuests)
//      MsgQuestList AcceptQuest    -> AllowPanelAccept / AfterPanelAccept (engine rules + onAccept actions)
//      MsgQuestList FinishQuest    -> OnClientFinish    quest-panel finish (fully re-checked on the server)
//      MsgQuestList FinishBountyHall -> BeforeBountyFinish / AfterBountyFinish (bag check, prize, "bountyDone")
//      Quests.RollBountyHallOffers -> IsBountyWorkable  only bounties the engine can complete are offered
//      daily reset (Server / ServerDatabase) -> ResetRepeatables   daily / weekly engine quests
//      PoolProcesses.CharactersCallback -> OnTick       Visit objectives, personal spawn / instance cleanup
//      any system                  -> OnEvent(client, key)          "event" objectives
//      @questinfo <id>, @questreload / console + AdminApi "reloadquests"
//
//  Rules:
//  - Hand-written quests keep priority: a MissionId referenced by hand-written code (HandCodedQuests) never gets
//    engine dialogs / finish / rewards unless quests.json says "engine": true. The engine only raises the kill
//    counters of hand-coded quests whose code never counts them (they gate on CheckObjectives but nothing counted).
//  - The client is never trusted: progress comes only from server events (kills, NPC clicks in view, positions,
//    server systems), the finish re-checks every objective and the bag space, and FinishQuest (Accepted ->
//    Finished, once) is done BEFORE any reward is given, so a reward can never be paid twice.
//  - Progress lives in the normal QuestGUI Intentions (saved by RoleQuests -> kept over relog) and is sent with
//    the same MsgQuestData packet as the hand-written code (SetQuestObjectives).
//  - Engine-spawned monsters (personal story bosses, instance monsters) belong to one player: only the owner (or
//    the owner's team) gets quest credit, they drop no loot, they despawn after their time, a player has at most
//    the quest's count alive and a re-summon has a cooldown.
//  - Engine finishes are logged to Infinity[DB]\questengine_log.txt.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.QuestEngine
{
    public static class QuestEngineCore
    {
        /// <summary>The published table (replaced as a whole by Load / reload).</summary>
        public static volatile QuestTable Current;
        private static readonly object _logLock = new object();
        private static readonly object _loadLock = new object();

        // ------------------------------------------------------------------ load / reload
        public static void Load()
        {
            lock (_loadLock)
            {
                try
                {
                    var t = QuestGrammar.Build();
                    ApplyDbOverrides(t);
                    Current = t;
                    MyConsole.WriteLine("[QuestEngine] " + t.Summary(), ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[QuestEngine] load failed (engine disabled until a reload works): " + e.Message, ConsoleColor.Yellow);
                    try { MyConsole.SaveException(e); } catch { }
                }
            }
        }

        /// <summary>HotReload op "reloadquests": Questinfo.ini (QuestInfo tables) + the engine table + quests.json.</summary>
        public static void Reload(HotReload.ReloadResult result)
        {
            HotReload.HotReloadManager.ReadStable(HotReload.HotReloadManager.DbFile("Questinfo.ini"));// refuses a file still being written
            int before = Database.QuestInfo.AllQuests.Count;
            QuestTable t;
            lock (_loadLock)
            {
                // Init reads ~25k keys through the profile API: without the startup ini cache that takes minutes.
                // The cache re-validates every file by size + write time and writers invalidate it, so it is
                // safe to switch on for the moment of this reload (reloads are serialized by HotReload).
                bool ownCache = !WindowsAPI.IniCache.Active;
                if (ownCache) WindowsAPI.IniCache.Begin();
                try { Database.QuestInfo.Init(); }// builds aside and publishes by reference swap
                finally { if (ownCache) WindowsAPI.IniCache.End(); }
                t = QuestGrammar.Build();
                ApplyDbOverrides(t);
                Current = t;
            }
            result.Counts["quests"] = Database.QuestInfo.AllQuests.Count;
            result.Counts["engineOwned"] = t.Owned;
            result.Counts["engineWorkable"] = t.Workable;
            result.Counts["handCoded"] = t.HandCoded;
            result.Counts["jsonQuests"] = t.JsonQuests;
            result.Message = "Applied live: Questinfo.ini " + before + " -> " + Database.QuestInfo.AllQuests.Count + " quests; engine: " + t.Summary();
            if (t.JsonError != null)
                result.Note("quests.json was NOT applied: " + t.JsonError);
            result.Note("Players see new/removed quests in the quest panel after their next login (the panel list is sent at login).");
        }

        /// <summary>[Festivals] rebuilds only the engine table (Questinfo.ini sections + quests.json + the festival
        /// overlays) and publishes it; QuestInfo tables are NOT re-read. Used when a festival starts / ends.</summary>
        public static string Rebuild()
        {
            lock (_loadLock)
            {
                var t = QuestGrammar.Build();
                ApplyDbOverrides(t);
                Current = t;
                int closed = CloseOrphanInstances(t);// [InstanceNpc] a stopped festival's private copies (and their NPCs) go too
                return t.Summary() + (closed > 0 ? ", " + closed + " instance copy(ies) of removed instances closed" : "");
            }
        }

        /// <summary>quests.json "prequest" / "beginNpc" (and begin NPC aliases) are written into the QuestInfo copy too,
        /// so the quest panel (MsgQuestList.CanClientAcceptQuest) and the engine agree. Re-applied after every Init.</summary>
        static void ApplyDbOverrides(QuestTable t)
        {
            var all = Database.QuestInfo.AllQuests;
            if (all == null) return;
            foreach (var q in t.Quests.Values)
            {
                Database.QuestInfo.DBQuest dbq;
                if (!all.TryGetValue(q.MissionId, out dbq) || dbq == null) continue;
                try
                {
                    if (q.PrequestOverride != null)
                    {
                        bool any;
                        dbq.Prequest = Database.QuestInfo.ParseIdList(q.PrequestOverride, out any);
                        dbq.PrequestAny = any;
                    }
                    if (dbq.BeginNpcId != null && q.Owned && q.BeginNpc != 0 && dbq.BeginNpcId.ID != q.BeginNpc)
                    {
                        var ni = new Database.QuestInfo.DBQuest.NpcInfo("") { ID = q.BeginNpc, Map = dbq.BeginNpcId.Map, X = dbq.BeginNpcId.X, Y = dbq.BeginNpcId.Y, Name = dbq.BeginNpcId.Name, MapName = dbq.BeginNpcId.MapName };
                        dbq.BeginNpcId = ni;
                    }
                }
                catch (Exception e) { MyConsole.WriteLine("[QuestEngine] override " + q.MissionId + ": " + e.Message); }
            }
        }

        // ------------------------------------------------------------------ helpers
        static readonly ConcurrentDictionary<string, string> NormCache = new ConcurrentDictionary<string, string>();
        static string NormName(string name)
        {
            if (name == null) return "";
            string n;
            if (!NormCache.TryGetValue(name, out n)) { n = QuestGrammar.Norm(name); NormCache.TryAdd(name, n); }
            return n;
        }
        static bool Roll(int chancePer10000)
        {
            if (chancePer10000 >= 10000) return true;
            if (chancePer10000 <= 0) return false;
            return Program.GetRandom.Next(10000) < chancePer10000;
        }
        static uint[] Snapshot(Role.Instance.Quests gui)
        {
            for (int i = 0; i < 3; i++)
            {
                try { return gui.AcceptedQuests.Keys.ToArray(); }
                catch (InvalidOperationException) { }
                catch (ArgumentException) { }
            }
            return new uint[0];
        }
        static uint[] AllKeys(Role.Instance.Quests gui)
        {
            for (int i = 0; i < 3; i++)
            {
                try { return gui.src.Keys.ToArray(); }
                catch (InvalidOperationException) { }
                catch (ArgumentException) { }
            }
            return new uint[0];
        }
        static uint[] Progress(Client.GameClient c, uint id)
        {
            uint[] cur;
            c.Player.QuestGUI.GetQuestObjectives(id, out cur);
            return cur ?? new uint[0];
        }
        static uint At(uint[] cur, byte slot) { return slot >= 1 && slot <= cur.Length ? cur[slot - 1] : 0; }
        static void SetSlot(Client.GameClient c, uint id, byte slot, uint value, ServerSockets.Packet stream)
        {
            if (slot < 1) return;
            var cur = Progress(c, id);
            var arr = new uint[Math.Max(cur.Length, slot)];
            Array.Copy(cur, arr, cur.Length);
            arr[slot - 1] = value;
            c.Player.QuestGUI.SetQuestObjectives(stream, id, arr);// active quests only; sends MsgQuestData like the hand-written code
        }
        static void Msg(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
        static bool Usable(EngineQuest q)
        {
            return q != null && !Database.QuestInfo.IsKingDomMission(q.MissionId);
        }

        // ------------------------------------------------------------------ state objectives (ER-2 / R3)
        public static bool StateKnown(Objective o)
        {
            switch ((o.Stat ?? "").ToLowerInvariant())
            {
                case "classstage": case "reborn": case "level": case "hundredweaponsunlocked": return true;
                case "hundredweapon": { Database.MagicType.WeaponsType w; return Enum.TryParse(o.Weapon, true, out w); }
                default: return false;
            }
        }
        static bool StateDone(Client.GameClient c, Objective o)
        {
            try
            {
                uint cls = c.Player.Class;
                switch ((o.Stat ?? "").ToLowerInvariant())
                {
                    case "classstage":
                        // a class id (awakening data, e.g. 5003 Dark Ninja) = same class line and at least that id;
                        // a small number (progression data) = the promotion stage Class % 100
                        if (o.Min >= 1000) return cls / 1000 == o.Min / 1000 && cls >= o.Min;
                        return cls % 100 >= o.Min;
                    case "reborn": return c.Player.Reborn >= o.Min;
                    case "level": return c.Player.Level >= o.Min;
                    case "hundredweaponsunlocked": return c.HundredWeapons != null && c.HundredWeapons.Unlocked;
                    case "hundredweapon":
                        {
                            Database.MagicType.WeaponsType w;
                            return c.HundredWeapons != null && Enum.TryParse(o.Weapon, true, out w) && c.HundredWeapons.ContainsWeapon(w);
                        }
                }
            }
            catch { }
            return false;
        }
        /// <summary>Writes satisfied state objectives into their slot (so the client tracker shows 1/1).</summary>
        static void SyncStates(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream)
        {
            if (!c.Player.QuestGUI.IsActiveQuest(q.MissionId)) return;
            foreach (var o in q.Objectives)
            {
                if (o.Kind != ObjectiveKind.State || o.Slot == 0) continue;
                uint target = Math.Max(1u, o.Count);// [QuestEngineFix] "Reach level 30" ([chk_quest ..,30]) shows 30/30
                if (StateDone(c, o) && At(Progress(c, q.MissionId), o.Slot) < target) SetSlot(c, q.MissionId, o.Slot, target, stream);
            }
        }

        /// <summary>Is one objective satisfied? atFinishNpc = the player is at the quest's finish NPC right now.</summary>
        static bool Done(Client.GameClient c, EngineQuest q, Objective o, uint[] cur, bool atFinishNpc, out uint have)
        {
            have = 0;
            switch (o.Kind)
            {
                case ObjectiveKind.Talk:
                    if (o.NpcId == q.FinishNpc && atFinishNpc) { have = o.Count; return true; }
                    have = At(cur, o.Slot);
                    return o.Slot > 0 && have >= o.Count;
                case ObjectiveKind.Kill:
                case ObjectiveKind.Gather:
                case ObjectiveKind.Visit:
                case ObjectiveKind.Event:
                case ObjectiveKind.Counter:
                case ObjectiveKind.KillPoints:
                    have = At(cur, o.Slot);
                    return o.Slot > 0 && have >= o.Count;
                case ObjectiveKind.Collect:
                    have = (uint)Math.Max(0, c.Inventory.GetCountItem(o.ItemId));
                    return have >= o.Count;
                case ObjectiveKind.State:
                    bool ok = StateDone(c, o);
                    have = ok ? 1u : 0u;
                    return ok;
                default:
                    return false;
            }
        }
        public static bool IsComplete(Client.GameClient c, EngineQuest q, bool atFinishNpc)
        {
            if (q.Objectives.Count == 0) return q.FinishNpc != 0 && atFinishNpc;
            var cur = Progress(c, q.MissionId);
            uint have;
            foreach (var o in q.Objectives)
                if (!Done(c, q, o, cur, atFinishNpc, out have)) return false;
            return true;
        }
        static List<string> ProgressLines(Client.GameClient c, EngineQuest q, bool atFinishNpc)
        {
            var lines = new List<string>();
            var cur = Progress(c, q.MissionId);
            foreach (var o in q.Objectives)
            {
                uint have;
                bool ok = Done(c, q, o, cur, atFinishNpc, out have);
                string what;
                switch (o.Kind)
                {
                    case ObjectiveKind.Talk: what = "Talk to " + o.Name; break;
                    case ObjectiveKind.Kill: what = "Defeat " + (o.AnyMonster ? "monsters" : (o.Name.Length > 0 ? o.Name : o.Monster)) + " " + Math.Min(have, o.Count) + "/" + o.Count + (o.TimeLimit > 0 ? " within " + o.TimeLimit + "s" : ""); break;
                    case ObjectiveKind.Collect: what = QuestGrammar.ItemName(o.ItemId) + " " + Math.Min(have, o.Count) + "/" + o.Count; break;
                    case ObjectiveKind.Gather: what = (o.Name.Length > 0 ? o.Name : "Gather") + " " + Math.Min(have, o.Count) + "/" + o.Count; break;
                    case ObjectiveKind.Visit: what = "Go to " + (o.Name.Length > 0 ? o.Name : "the place") + " (" + o.X + "," + o.Y + ")"; break;
                    case ObjectiveKind.State: what = o.Name.Length > 0 ? QuestGrammar.Clean(o.Name) : ("Reach " + o.Stat + " " + o.Min); break;
                    case ObjectiveKind.Event:
                    case ObjectiveKind.Counter:
                    case ObjectiveKind.KillPoints: what = (o.Name.Length > 0 ? QuestGrammar.Clean(o.Name) : "Progress") + " " + Math.Min(have, o.Count) + "/" + o.Count; break;
                    default: what = QuestGrammar.Clean(o.Text); break;
                }
                lines.Add((ok ? "[done] " : "") + what);
            }
            return lines;
        }

        // ------------------------------------------------------------------ requirements
        /// <summary>Same rules as the quest panel (MsgQuestList.CanClientAcceptQuest) minus the begin-NPC check (the caller is at the NPC).</summary>
        public static bool CanAccept(Client.GameClient c, Database.QuestInfo.DBQuest dbq, out string why)
        {
            why = null;
            var gui = c.Player.QuestGUI;
            if (dbq == null) { why = "unknown quest"; return false; }
            if (gui.src.ContainsKey(dbq.MissionId)) { why = "already taken"; return false; }
            if (!gui.AllowAccept()) { why = "You already have 5 quests in progress."; return false; }
            if (dbq.Profession != null && !dbq.CanAcceptQuest(c.Player.Class)) { why = "Your class can't take this quest."; return false; }
            if (dbq.Lv_min > 0 && c.Player.Level < dbq.Lv_min) { why = "You need level " + dbq.Lv_min + "."; return false; }
            if (dbq.Lv_max > 0 && c.Player.Level > dbq.Lv_max) { why = "This quest is for level " + dbq.Lv_max + " or lower."; return false; }
            if (dbq.Prequest != null)
            {
                bool declared = false, any = false;
                foreach (uint pre in dbq.Prequest)
                {
                    if (pre == 0) continue;
                    declared = true;
                    bool fin = gui.CheckQuest(pre, MsgQuestList.QuestListItem.QuestStatus.Finished);
                    if (fin) any = true;
                    else if (!dbq.PrequestAny) { why = "Finish the previous quest first."; return false; }
                }
                if (dbq.PrequestAny && declared && !any) { why = "Finish the previous quest first."; return false; }
            }
            return true;
        }
        /// <summary>Engine-only rules on top of CanAccept: clan, today's pool pick, bag space for onAccept items.</summary>
        static bool CanAcceptEngine(Client.GameClient c, QuestTable t, EngineQuest q, out string why)
        {
            why = null;
            if (q.RequireClan && c.Player.MyClan == null) { why = "Only clan members can take this task."; return false; }
            if (q.Pool != null && !IsPoolPick(c, t, q)) { why = "That task is not offered to you today."; return false; }
            if (q.OnAccept != null && q.OnAccept.Give.Count > 0)
            {
                int slots = SlotsFor(GiveNeeded(c, q.OnAccept));
                if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots))) { why = "Please make " + slots + " free space(s) in your inventory first."; return false; }
            }
            return true;
        }
        /// <summary>Pool quests: one workable quest of the pool per player per day (deterministic, nothing stored).</summary>
        static bool IsPoolPick(Client.GameClient c, QuestTable t, EngineQuest q)
        {
            List<EngineQuest> pool;
            if (q.Pool == null || !t.Pools.TryGetValue(q.Pool, out pool)) return true;
            var cand = new List<EngineQuest>();
            foreach (var p in pool)
            {
                Database.QuestInfo.DBQuest dbq;
                if (!p.Workable || !Database.QuestInfo.AllQuests.TryGetValue(p.MissionId, out dbq)) continue;
                if (dbq.Profession != null && !dbq.CanAcceptQuest(c.Player.Class)) continue;
                cand.Add(p);
            }
            if (cand.Count == 0) return false;
            var now = DateTime.Now;
            unchecked
            {
                uint h = 2166136261;
                foreach (char ch in q.Pool) h = (h ^ ch) * 16777619;
                h = (h ^ c.Player.UID) * 16777619;
                h = (h ^ (uint)(now.Year * 400 + now.DayOfYear)) * 16777619;
                h ^= h >> 13; h *= 0x5bd1e995; h ^= h >> 15;
                return cand[(int)(h % (uint)cand.Count)] == q;
            }
        }

        // ------------------------------------------------------------------ kills / drops
        public static void OnMonsterKilled(Game.MsgMonster.MonsterRole mob, Client.GameClient killer, ServerSockets.Packet stream)
        {
            try
            {
                var t = Current;
                if (t == null || mob == null || killer == null || killer.Player == null) return;
                string norm = NormName(mob.Name);
                SpawnRec own;
                if (Spawns.TryGetValue(mob.UID, out own))
                {
                    // an engine-spawned monster: quest credit only for its owner (the killer, or the killer's team mate nearby)
                    own.Killed = true;
                    if (killer.Player.UID == own.Owner) { Credit(killer, mob, norm, t, stream, true); return; }
                    var tm = killer.Team;
                    if (tm != null)
                        foreach (var member in tm.GetMembers().ToArray())
                            if (member != null && member.Player != null && member.Player.UID == own.Owner && member.Player.Alive && member.Player.Map == mob.Map && member.Player.DynamicID == mob.DynamicID && Role.Core.GetDistance(member.Player.X, member.Player.Y, mob.X, mob.Y) <= 18)
                                Credit(member, mob, norm, t, stream, true);
                    return;
                }
                Credit(killer, mob, norm, t, stream, true);
                var team = killer.Team;
                if (team != null)
                {
                    foreach (var member in team.GetMembers().ToArray())
                    {
                        if (member == null || member == killer || member.Player == null || !member.Player.Alive) continue;
                        if (member.Player.Map != mob.Map || member.Player.DynamicID != mob.DynamicID) continue;
                        if (Role.Core.GetDistance(member.Player.X, member.Player.Y, mob.X, mob.Y) > 18) continue;
                        Credit(member, mob, norm, t, stream, false);
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static readonly ConcurrentDictionary<string, DateTime> KillRuns = new ConcurrentDictionary<string, DateTime>();

        static void Credit(Client.GameClient c, Game.MsgMonster.MonsterRole mob, string norm, QuestTable t, ServerSockets.Packet stream, bool isKiller)
        {
            var gui = c.Player.QuestGUI;
            if (gui == null || gui.AcceptedQuests.Count == 0) return;
            foreach (uint id in Snapshot(gui))
            {
                EngineQuest q;
                if (!t.Quests.TryGetValue(id, out q) || !Usable(q) || q.Disabled) continue;
                if (!isKiller && !q.Team) continue;
                if (!gui.IsActiveQuest(id)) continue;
                bool changed = false;
                var cur = Progress(c, id);
                foreach (var o in q.Objectives)
                {
                    switch (o.Kind)
                    {
                        case ObjectiveKind.Kill:
                            {
                                if (!q.EngineCounts || o.Slot == 0 || !o.MatchesMonster(norm)) break;
                                if (o.AnyMonster)
                                {
                                    if (o.Maps != null ? !o.Maps.Contains(mob.Map) : (o.Map != 0 && o.Map != mob.Map)) break;
                                    if (mob.Level + o.MinLevelGap < c.Player.Level) break;// no farming of low monsters
                                }
                                uint have = At(cur, o.Slot);
                                if (have >= o.Count) break;
                                if (o.TimeLimit > 0)
                                {
                                    string key = c.Player.UID + ":" + id + ":" + o.Slot;
                                    DateTime start, now = DateTime.Now;
                                    if (have == 0 || !KillRuns.TryGetValue(key, out start) || (now - start).TotalSeconds > o.TimeLimit)
                                    {
                                        if (have > 0) Msg(c, "[Quest] " + q.Name + ": too slow, the count starts again.");
                                        have = 0;
                                        KillRuns[key] = now;
                                    }
                                }
                                SetSlot(c, id, o.Slot, have + 1, stream);
                                cur = Progress(c, id);
                                changed = true;
                                Msg(c, "[Quest] " + q.Name + ": " + (o.AnyMonster ? "monsters" : (o.Name.Length > 0 ? o.Name : mob.Name)) + " " + (have + 1) + "/" + o.Count);
                                break;
                            }
                        case ObjectiveKind.KillPoints:
                            {
                                if (!q.EngineCounts || o.Slot == 0) break;
                                if (o.Map != 0 && o.Map != mob.Map) break;
                                if (mob.Level + o.MinLevelGap < c.Player.Level) break;
                                uint have = At(cur, o.Slot);
                                if (have >= o.Count) break;
                                uint add = Math.Max(1u, (uint)mob.Level);
                                SetSlot(c, id, o.Slot, Math.Min(o.Count, have + add), stream);
                                cur = Progress(c, id);
                                changed = true;
                                break;
                            }
                        case ObjectiveKind.Gather:
                            {
                                if (!q.EngineCounts || o.Slot == 0 || o.Map != mob.Map) break;
                                if (Math.Max(Math.Abs(mob.X - o.X), Math.Abs(mob.Y - o.Y)) > o.Range) break;
                                uint have = At(cur, o.Slot);
                                if (have >= o.Count || !Roll(o.Chance)) break;
                                SetSlot(c, id, o.Slot, have + 1, stream);
                                cur = Progress(c, id);
                                changed = true;
                                Msg(c, "[Quest] " + q.Name + ": " + (o.Name.Length > 0 ? o.Name : "collected") + " " + (have + 1) + "/" + o.Count);
                                break;
                            }
                        case ObjectiveKind.Collect:
                            {
                                // quest items drop only for a player who has the quest and still needs the item
                                if (o.ItemId == 0 || o.Drops.Count == 0) break;
                                int have = c.Inventory.GetCountItem(o.ItemId);
                                if (have >= o.Count) break;
                                DropRule rule = null;
                                foreach (var r in o.Drops)
                                    if (r.Matches(norm, mob.Map, mob.X, mob.Y)) { rule = r; break; }
                                if (rule == null || !Roll(rule.Chance)) break;
                                if (!Pool.ItemsBase.ContainsKey(o.ItemId)) break;
                                if (c.Inventory.HaveSpace(1))
                                {
                                    c.Inventory.AddItemWitchStack(o.ItemId, 0, 1, stream, true);
                                    Msg(c, "[Quest] " + q.Name + ": " + QuestGrammar.ItemName(o.ItemId) + " " + (have + 1) + "/" + o.Count);
                                }
                                else if (isKiller && c.Map != null)
                                {
                                    mob.DropItemID(c, o.ItemId, stream);// owned by the player
                                    Msg(c, "[Quest] Your bag is full: " + QuestGrammar.ItemName(o.ItemId) + " dropped on the ground.");
                                }
                                changed = true;
                                break;
                            }
                    }
                }
                if (changed)
                    AfterProgress(c, q, stream);
            }
        }

        // ------------------------------------------------------------------ events (ER-3 / ER-6 / R6 / R7)
        /// <summary>
        /// A server system reports something the player did ("bountyDone", "useItem:&lt;itemId&gt;", ...). Raises every
        /// active engine-counted quest with an event objective of that key. Never throws. A new key must also be added
        /// to QuestGrammar.KnownEvents (otherwise quests waiting for it stay "not workable" and are never offered).
        /// </summary>
        public static void OnEvent(Client.GameClient c, string key, uint amount = 1)
        {
            try
            {
                var t = Current;
                if (t == null || c == null || c.Player == null || c.Player.QuestGUI == null || string.IsNullOrEmpty(key) || amount == 0) return;
                var gui = c.Player.QuestGUI;
                if (gui.AcceptedQuests.Count == 0) return;
                foreach (uint id in Snapshot(gui))
                {
                    EngineQuest q;
                    if (!t.Quests.TryGetValue(id, out q) || !Usable(q) || !q.EngineCounts || !gui.IsActiveQuest(id)) continue;
                    bool changed = false;
                    foreach (var o in q.Objectives)
                    {
                        if (o.Kind != ObjectiveKind.Event || o.Slot == 0 || !string.Equals(o.Event, key, StringComparison.OrdinalIgnoreCase)) continue;
                        uint have = At(Progress(c, id), o.Slot);
                        if (have >= o.Count) continue;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            SetSlot(c, id, o.Slot, Math.Min(o.Count, have + amount), rec.GetStream());
                            changed = true;
                            Msg(c, "[Quest] " + q.Name + ": " + (o.Name.Length > 0 ? QuestGrammar.Clean(o.Name) : "progress") + " " + Math.Min(o.Count, have + amount) + "/" + o.Count);
                            if (changed) AfterProgress(c, q, rec.GetStream());
                        }
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>After any progress: bounty bookkeeping, onComplete actions (once) and the "objectives complete" notice.</summary>
        static readonly ConcurrentDictionary<string, byte> CompleteFired = new ConcurrentDictionary<string, byte>();
        static void AfterProgress(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream)
        {
            if (!IsComplete(c, q, true)) return;
            if (q.IsBounty)
            {
                c.Player.QuestGUI.MarkBountyObjectivesDone(q.MissionId);
                Msg(c, "[Quest] " + q.Name + ": complete! Claim it at the Bounty Hall.");
                return;
            }
            if (q.Owned && q.OnComplete != null && CompleteFired.TryAdd(c.Player.UID + ":" + q.MissionId, 0))
                RunActions(c, q, q.OnComplete, stream, "complete");
            if (q.Owned)
                Msg(c, "[Quest] " + q.Name + ": objectives complete. Report back" + (q.FinishNpc != 0 && q.FinishMap != 0 ? " (" + q.FinishMap + ": " + q.FinishX + "," + q.FinishY + ")" : "") + ".");
        }

        // ------------------------------------------------------------------ visits (called ~1/s per player)
        static readonly ConcurrentDictionary<uint, HashSet<string>> Visited = new ConcurrentDictionary<uint, HashSet<string>>();
        public static void OnTick(Client.GameClient c)
        {
            try
            {
                Sweep();
                var t = Current;
                if (t == null || c == null || c.Player == null || c.Player.QuestGUI == null || c.Player.QuestGUI.AcceptedQuests.Count == 0) return;
                foreach (uint id in Snapshot(c.Player.QuestGUI))
                {
                    EngineQuest q;
                    // ER-1 (dailies): hand-coded quests whose code never counts get their Visit steps counted too
                    if (!t.Quests.TryGetValue(id, out q) || !q.EngineCounts || !Usable(q)) continue;
                    for (int i = 0; i < q.Objectives.Count; i++)
                    {
                        var o = q.Objectives[i];
                        if (o.Kind != ObjectiveKind.Visit || o.Map != c.Player.Map || o.Slot == 0) continue;
                        if (Math.Max(Math.Abs(c.Player.X - o.X), Math.Abs(c.Player.Y - o.Y)) > o.Range) continue;
                        var cur = Progress(c, id);
                        uint have = At(cur, o.Slot);
                        if (have >= o.Count) continue;
                        int sharing = q.Objectives.Count(x => x.Kind == ObjectiveKind.Visit && x.Slot == o.Slot);
                        uint value;
                        byte memo = 0; uint memoMask = 0;
                        if (sharing <= 1) value = o.Count;// one place for the slot: reaching it completes the step
                        else
                        {
                            // several places share one counter: each place counts once.
                            // [QuestEngineFix 2026-10-14] the places already counted are kept as a bit mask in the slot after
                            // the quest's last objective slot, i.e. in the saved quest progress, so a relog can't count a
                            // place again (the in-memory set is only the fallback when no free slot / bit exists).
                            byte maxSlot = q.MaxSlot;
                            if (maxSlot < 32 && i < 32)
                            {
                                memo = (byte)(maxSlot + 1);
                                uint mask = At(cur, memo), bit = 1u << i;
                                if ((mask & bit) != 0) continue;
                                memoMask = mask | bit;
                            }
                            else
                            {
                                var set = Visited.GetOrAdd(c.Player.UID, k => new HashSet<string>());
                                string key = id + ":" + i;
                                lock (set) { if (!set.Add(key)) continue; }
                            }
                            value = have + 1;
                        }
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            if (memo != 0)
                            {
                                var arr = new uint[Math.Max(cur.Length, (int)Math.Max(memo, o.Slot))];
                                Array.Copy(cur, arr, cur.Length);
                                arr[o.Slot - 1] = value;
                                arr[memo - 1] = memoMask;
                                c.Player.QuestGUI.SetQuestObjectives(rec.GetStream(), id, arr);
                            }
                            else
                                SetSlot(c, id, o.Slot, value, rec.GetStream());
                            Msg(c, "[Quest] " + q.Name + ": " + (o.Name.Length > 0 ? o.Name : "place") + " reached.");
                            AfterProgress(c, q, rec.GetStream());
                        }
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ NPC dialogs
        enum Act : byte { Finish, Progress, Offer, Accept, Other, Teleport, Respawn }
        sealed class DialogState
        {
            public uint Npc;
            public readonly Dictionary<byte, KeyValuePair<Act, uint>> Options = new Dictionary<byte, KeyValuePair<Act, uint>>();
        }
        static readonly ConcurrentDictionary<uint, DialogState> States = new ConcurrentDictionary<uint, DialogState>();
        const byte OptOther = 250, OptClose = 255;

        static bool HasHandler(uint npcid)
        {
            Game.MsgNpc.ProcessAction act;
            try { var fx = ExtraNpcHandlerExists; if (fx != null && fx(npcid)) return true; } catch { }// [Festivals]
            if (Game.MsgNpc.Procesor.MaintinaceNpc.Contains(npcid)) return false;// "Under maintenance" scripts are never opened
            return Game.MsgNpc.Procesor.invoker.TryGetInvoker((Game.MsgNpc.NpcID)npcid, out act);
        }
        /// <summary>[Festivals] NPCs whose "Something else." goes to another module (festival NPCs: exchanges/greeting).</summary>
        public static Func<uint, bool> ExtraNpcHandlerExists;
        public static Action<Client.GameClient, ServerSockets.Packet, uint> ExtraNpcHandler;
        /// <summary>[Festivals] called after an engine finish of a festival quest (festival key, quest id, what was given).</summary>
        public static Action<Client.GameClient, string, uint, string> FestivalQuestFinished;

        /// <summary>The NPC begins / finishes engine quests or has engine teleports (Procesor lets such a maintenance NPC through).</summary>
        public static bool HasEngineQuests(uint npcid)
        {
            var t = Current;
            return t != null && (t.ByBeginNpc.ContainsKey(npcid) || t.ByFinishNpc.ContainsKey(npcid) || t.NpcTeleports.ContainsKey(npcid));
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

        /// <summary>
        /// Called for every dialog request to an NPC that is in the player's view. Returns true when the engine
        /// answered (the hand-written handler must not run for this request).
        /// </summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            try
            {
                var t = Current;
                if (t == null || c == null || c.Player == null) return false;
                if (option == 0 && global::Qahr.Mobile.Phone.MobileQuestBounty.OnNpc(c, npcid)) return true;// QahrMobile: Chinju 30893 = the mobile's daily Bounty Hall window
                if (option == 0 && global::Qahr.Mobile.Phone.MobileQuestStoryNpc.OnNpc(c, npcid)) return true;// QahrMobile: main-story finish NPC -> story dialog + reward + next step
                if (option == 0)
                {
                    DialogState old; States.TryRemove(c.Player.UID, out old);
                    bool talked = ApplyTalk(c, t, npcid, stream);
                    bool handled = ShowMenu(c, t, npcid, stream);
                    if (!handled && talked && !HasHandler(npcid))
                    {
                        var d = new Game.MsgNpc.Dialog(c, stream);
                        d.AddText("Thank you for coming. Your quest log has been updated.").AddOption("Goodbye.", OptClose).AddAvatar(10).FinalizeDialog();
                        return true;
                    }
                    return handled;
                }
                DialogState st;
                if (!States.TryGetValue(c.Player.UID, out st) || st.Npc != npcid) return false;
                KeyValuePair<Act, uint> choice;
                if (!st.Options.TryGetValue(option, out choice)) { States.TryRemove(c.Player.UID, out st); return false; }
                States.TryRemove(c.Player.UID, out st);
                EngineQuest q;
                t.Quests.TryGetValue(choice.Value, out q);
                switch (choice.Key)
                {
                    case Act.Other:
                        {
                            var fx = ExtraNpcHandlerExists; var fh = ExtraNpcHandler;// [Festivals]
                            if (fx != null && fh != null && fx(npcid)) { fh(c, stream, npcid); return true; }
                            Game.MsgNpc.ProcessAction act;
                            if (HasHandler(npcid) && Game.MsgNpc.Procesor.invoker.TryGetInvoker((Game.MsgNpc.NpcID)npcid, out act))
                                act(c, stream, 0, "", npcid);
                            return true;
                        }
                    case Act.Finish:
                        {
                            string result = "That quest is gone.";
                            bool ok = q != null && TryFinish(c, q, stream, npcid, out result);
                            var d = new Game.MsgNpc.Dialog(c, stream);
                            Text(d, ok ? (q.FinishText ?? "Well done! Here is your reward.") + " " + result : result);
                            d.AddOption(ok ? "Thank you!" : "I see.", OptClose).AddAvatar(10).FinalizeDialog();
                            if (ok) FollowUp(c, t, q, stream);
                            return true;
                        }
                    case Act.Progress:
                        {
                            var d = new Game.MsgNpc.Dialog(c, stream);
                            Text(d, (q != null ? (q.ProgressText ?? (q.Name + ": you are not done yet.")) + " " + string.Join(". ", ProgressLines(c, q, npcid == q.FinishNpc)) + "." : "That quest is gone."));
                            if (q != null && CanRespawn(c, q))
                            {
                                var ns = new DialogState { Npc = npcid };
                                ns.Options[1] = new KeyValuePair<Act, uint>(Act.Respawn, q.MissionId);
                                States[c.Player.UID] = ns;
                                d.AddOption("Call it out again.", 1);
                            }
                            d.AddOption("I'll be back.", OptClose).AddAvatar(10).FinalizeDialog();
                            return true;
                        }
                    case Act.Respawn:
                        {
                            var d = new Game.MsgNpc.Dialog(c, stream);
                            if (q != null && CanRespawn(c, q) && SpawnAll(c, q, stream) > 0) Text(d, "It has appeared again. Hurry!");
                            else Text(d, "Not now. Try again a little later.");
                            d.AddOption("OK.", OptClose).AddAvatar(10).FinalizeDialog();
                            return true;
                        }
                    case Act.Teleport:
                        {
                            List<NpcTeleport> tps;
                            if (t.NpcTeleports.TryGetValue(npcid, out tps) && choice.Value < tps.Count && TeleportAllowed(c, tps[(int)choice.Value]))
                            {
                                var tp = tps[(int)choice.Value];
                                InstanceDef inst;
                                if (tp.Instance != null && t.Instances.TryGetValue(tp.Instance, out inst)) EnterInstance(c, inst, tp.RequireQuest, stream);
                                else DoTeleport(c, tp.Map, tp.X, tp.Y, KeepDyn(c, tp.Map));// [InstanceNpc] inside the own copy: stay in it
                            }
                            return true;
                        }
                    case Act.Offer:
                        {
                            if (q == null) return true;
                            var d = new Game.MsgNpc.Dialog(c, stream);
                            Text(d, q.BeginText ?? (q.Content.Length > 0 ? q.Content : q.Desc));
                            if (!q.Prize.IsEmpty || q.Extra != null) Text(d, "Reward: " + q.Prize.Describe(q.ChainEnd) + (q.Extra != null ? " " + q.Extra : "") + ".");
                            var ns = new DialogState { Npc = npcid };
                            ns.Options[1] = new KeyValuePair<Act, uint>(Act.Accept, q.MissionId);
                            States[c.Player.UID] = ns;
                            d.AddOption("I accept.", 1).AddOption("Not now.", OptClose).AddAvatar(10).FinalizeDialog();
                            return true;
                        }
                    case Act.Accept:
                        {
                            if (q == null) return true;
                            string why;
                            var d = new Game.MsgNpc.Dialog(c, stream);
                            if (AcceptQuest(c, q, out why, stream))
                                Text(d, "Quest accepted: " + q.Name + ". " + string.Join(". ", ProgressLines(c, q, false)) + ".");
                            else
                                Text(d, why ?? "You can't take this quest now.");
                            d.AddOption("OK.", OptClose).AddAvatar(10).FinalizeDialog();
                            return true;
                        }
                }
                return true;
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                return false;
            }
        }

        /// <summary>Talk objectives of engine quests: clicking the NPC (in view) completes "talk to X" for an accepted quest.</summary>
        static bool ApplyTalk(Client.GameClient c, QuestTable t, uint npcid, ServerSockets.Packet stream)
        {
            var gui = c.Player.QuestGUI;
            if (gui.AcceptedQuests.Count == 0) return false;
            bool any = false;
            foreach (uint id in Snapshot(gui))
            {
                EngineQuest q;
                if (!t.Quests.TryGetValue(id, out q) || !q.Owned || !Usable(q) || !gui.IsActiveQuest(id)) continue;
                foreach (var o in q.Objectives)
                {
                    if (o.Kind != ObjectiveKind.Talk || o.NpcId != npcid || o.Slot == 0) continue;
                    var cur = Progress(c, id);
                    if (At(cur, o.Slot) >= o.Count) continue;
                    if (o.After > 0)
                    {
                        var prev = q.Objectives.FirstOrDefault(x => x.Slot == o.After);
                        uint h;
                        if (prev != null && !Done(c, q, prev, cur, false, out h)) continue;
                    }
                    // [QuestEngineFix 2026-10-14] "pick 9 flowers" / "search 3 times" ([chk_quest ..,9] on an NPC): each click
                    // counts one (it completed on the first click before)
                    uint now = o.Count > 1 ? At(cur, o.Slot) + 1 : o.Count;
                    SetSlot(c, id, o.Slot, now, stream);
                    if (npcid != q.FinishNpc)
                    {
                        Msg(c, "[Quest] " + q.Name + ": " + (o.Count > 1 ? QuestGrammar.Clean(o.Name) + " " + now + "/" + o.Count : "talked to " + o.Name) + ".");
                        any = true;
                    }
                    AfterProgress(c, q, stream);
                }
            }
            return any;
        }

        static bool TeleportAllowed(Client.GameClient c, NpcTeleport tp)
        {
            if (tp.MinLevel > 0 && c.Player.Level < tp.MinLevel) return false;
            if (tp.RequireQuest != 0 && !c.Player.QuestGUI.src.ContainsKey(tp.RequireQuest)) return false;
            if (tp.Instance != null && tp.RequireQuest != 0 && !c.Player.QuestGUI.IsActiveQuest(tp.RequireQuest)) return false;
            return true;
        }

        static bool ShowMenu(Client.GameClient c, QuestTable t, uint npcid, ServerSockets.Packet stream)
        {
            var gui = c.Player.QuestGUI;
            var st = new DialogState { Npc = npcid };
            var d = new Game.MsgNpc.Dialog(c, stream);
            var lines = new List<KeyValuePair<string, byte>>();
            byte next = 100;
            string intro = null;
            List<EngineQuest> list;
            if (t.ByFinishNpc.TryGetValue(npcid, out list))
                foreach (var q in list)
                {
                    if (!Usable(q) || q.IsBounty || q.OfferOnly || !q.Workable || !gui.IsActiveQuest(q.MissionId) || lines.Count >= 6) continue;
                    SyncStates(c, q, stream);
                    bool done = IsComplete(c, q, true);
                    st.Options[next] = new KeyValuePair<Act, uint>(done ? Act.Finish : Act.Progress, q.MissionId);
                    lines.Add(new KeyValuePair<string, byte>(Opt((done ? "Complete: " : "Progress: ") + q.Name), next++));
                    if (done && intro == null) intro = q.Name + " - you have done everything I asked.";
                }
            if (t.ByBeginNpc.TryGetValue(npcid, out list))
                foreach (var q in list)
                {
                    if (!Usable(q) || q.IsBounty || !q.Workable || lines.Count >= 6) continue;
                    if (gui.IsActiveQuest(q.MissionId) && q.FinishNpc != npcid && CanRespawn(c, q))
                    {
                        // the begin NPC of a story boss quest offers "progress" (with "call it out again")
                        st.Options[next] = new KeyValuePair<Act, uint>(Act.Progress, q.MissionId);
                        lines.Add(new KeyValuePair<string, byte>(Opt("Progress: " + q.Name), next++));
                        continue;
                    }
                    Database.QuestInfo.DBQuest dbq;
                    if (!Database.QuestInfo.AllQuests.TryGetValue(q.MissionId, out dbq)) continue;
                    string why;
                    if (!CanAccept(c, dbq, out why) || !CanAcceptEngine(c, t, q, out why)) continue;
                    st.Options[next] = new KeyValuePair<Act, uint>(Act.Offer, q.MissionId);
                    lines.Add(new KeyValuePair<string, byte>(Opt("Quest: " + q.Name), next++));
                }
            List<NpcTeleport> tps;
            if (t.NpcTeleports.TryGetValue(npcid, out tps))
                for (int i = 0; i < tps.Count && lines.Count < 8; i++)
                {
                    if (!TeleportAllowed(c, tps[i])) continue;
                    st.Options[next] = new KeyValuePair<Act, uint>(Act.Teleport, (uint)i);
                    lines.Add(new KeyValuePair<string, byte>(Opt(tps[i].Text), next++));
                }
            if (lines.Count == 0) return false;
            bool handler = HasHandler(npcid);
            Text(d, intro ?? Festivals.FestivalManager.NpcGreeting(npcid) ?? "I may need your help, traveler.");// [DreamTheater 2026-10-15] the festival NPC greeting when there is one
            foreach (var l in lines) d.AddOption(l.Key, l.Value);
            if (handler)
            {
                st.Options[OptOther] = new KeyValuePair<Act, uint>(Act.Other, 0);
                d.AddOption("Something else.", OptOther);
            }
            d.AddOption("Goodbye.", OptClose).AddAvatar(10).FinalizeDialog();
            States[c.Player.UID] = st;
            return true;
        }

        static bool AcceptQuest(Client.GameClient c, EngineQuest q, out string why, ServerSockets.Packet stream)
        {
            why = null;
            var t = Current;
            Database.QuestInfo.DBQuest dbq;
            if (t == null || !q.Owned || !q.Workable || q.IsBounty || !Database.QuestInfo.AllQuests.TryGetValue(q.MissionId, out dbq)) { why = "This quest is not available."; return false; }
            if (!CanAccept(c, dbq, out why)) return false;
            if (!CanAcceptEngine(c, t, q, out why)) return false;
            c.Player.QuestGUI.Accept(dbq, 0);
            if (!c.Player.QuestGUI.IsActiveQuest(q.MissionId)) { why = "This quest is not available."; return false; }
            Log(c, q, "ACCEPT", "");
            OnAccepted(c, q, stream);
            return true;
        }
        static void OnAccepted(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream)
        {
            CompleteFired.TryRemove(c.Player.UID + ":" + q.MissionId, out _);
            SyncStates(c, q, stream);
            if (q.OnAccept != null) RunActions(c, q, q.OnAccept, stream, "accept");
        }

        /// <summary>Quest-panel accept (MsgQuestList AcceptQuest), before QuestGUI.Accept: the engine-only rules of
        /// engine-owned quests (clan, today's pool pick, bag space for onAccept items). Other quests: always true.</summary>
        public static bool AllowPanelAccept(Client.GameClient c, uint id)
        {
            try
            {
                var t = Current;
                EngineQuest q;
                if (t != null && c != null && t.Quests.TryGetValue(id, out q) && q.FestivalOff) { Msg(c, "[Quest] This festival quest is available only while the festival is running."); return false; }// [Festivals]
                // [QuestEngineFix 2026-10-14] the panel took ANY quest of the table: a disabled or unworkable quest (that no
                // hand script runs) then sat in one of the 5 slots and could never be finished
                if (t != null && c != null && t.Quests.TryGetValue(id, out q) && Usable(q) && ((q.Owned && !q.Workable) || (!q.HandCoded && (q.Disabled || !q.Workable))))
                { Msg(c, "[Quest] This quest is not available yet."); return false; }
                if (t == null || c == null || !t.Quests.TryGetValue(id, out q) || !q.Owned) return true;
                string why;
                if (!CanAcceptEngine(c, t, q, out why)) { Msg(c, "[Quest] " + why); return false; }
                return true;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return true; }
        }
        /// <summary>Quest-panel accept, after a successful QuestGUI.Accept: onAccept actions of engine-owned quests.</summary>
        public static void AfterPanelAccept(Client.GameClient c, uint id)
        {
            try
            {
                var t = Current;
                EngineQuest q;
                if (t == null || c == null || !t.Quests.TryGetValue(id, out q) || !q.Owned || !q.Workable || !c.Player.QuestGUI.IsActiveQuest(id)) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    Log(c, q, "ACCEPT", "panel");
                    OnAccepted(c, q, rec.GetStream());
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>[LuaUi] the begin NPC's "I accept." from a client Lua window (Dream Theater: Agnes in DlgWhiteDayMain):
        /// only a quest that NPC begins, with every accept rule of the NPC menu (CanAccept + CanAcceptEngine).</summary>
        public static bool AcceptAtNpc(Client.GameClient c, uint npcid, uint id, out string name, out string why)
        {
            name = null; why = null;
            try
            {
                var t = Current;
                EngineQuest q; List<EngineQuest> list;
                if (t == null || c == null || c.Player == null || !t.Quests.TryGetValue(id, out q) || !Usable(q) || q.Disabled || q.FestivalOff
                    || !t.ByBeginNpc.TryGetValue(npcid, out list) || !list.Contains(q)) { why = "This quest is not available."; return false; }
                name = q.Name;
                using (var rec = new ServerSockets.RecycledPacket())
                    return AcceptQuest(c, q, out why, rec.GetStream());
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } why = "This quest is not available."; return false; }
        }

        static void FollowUp(Client.GameClient c, QuestTable t, EngineQuest q, ServerSockets.Packet stream)
        {
            EngineQuest next;
            bool ok = false;
            if (q.NextQuest != 0 && q.AutoAccept && t.Quests.TryGetValue(q.NextQuest, out next))
            {
                string why;
                ok = AcceptQuest(c, next, out why, stream);
                if (ok) Msg(c, "[Quest] New quest: " + next.Name + ".");
            }
            // [InstanceNpc] the copy was kept for the next quest, but it was not taken over: leave it now
            InstanceRec ir;
            if (Instances.TryGetValue(c.Player.UID, out ir) && ir.HandOver != 0 && ir.HandOver == q.NextQuest && ir.Quest == q.MissionId)
            {
                ir.HandOver = 0;
                LeaveInstance(c, true);
            }
        }

        // ------------------------------------------------------------------ actions (R2 / R5 / ER-9 / R8)
        static List<PrizeItem> GiveNeeded(Client.GameClient c, QuestActions a)
        {
            // an accept/abandon loop must not farm the handed-over items: only what the bag lacks is given
            var l = new List<PrizeItem>();
            foreach (var g in a.Give)
            {
                if (!Pool.ItemsBase.ContainsKey(g.Id)) continue;
                int have = c.Inventory.GetCountItem(g.Id);
                if (have < g.Count) l.Add(new PrizeItem { Id = g.Id, Count = (ushort)(g.Count - have), Bound = true });
            }
            return l;
        }
        static void RunActions(Client.GameClient c, EngineQuest q, QuestActions a, ServerSockets.Packet stream, string when)
        {
            if (a == null) return;
            var done = new List<string>();
            Try(c, "give", () =>
            {
                foreach (var g in GiveNeeded(c, a))
                    if (c.Inventory.HaveSpace(1) && c.Inventory.AddItemWitchStack(g.Id, 0, g.Count, stream, true))
                        done.Add("gave " + g.Id + "x" + g.Count);
            });
            bool moved = false;
            if (a.Instance != null)
            {
                var t = Current; InstanceDef inst;
                Try(c, "instance", () => { if (t != null && t.Instances.TryGetValue(a.Instance, out inst) && EnterInstance(c, inst, q.MissionId, stream)) { moved = true; done.Add("instance " + a.Instance); } });
            }
            else if (a.LeaveInstance)
                Try(c, "leave", () => { if (LeaveInstance(c, true)) { moved = true; done.Add("left instance"); } });
            if (!moved && a.TeleportMap != 0)
                Try(c, "teleport", () => { if (DoTeleport(c, a.TeleportMap, a.TeleportX, a.TeleportY, KeepDyn(c, a.TeleportMap))) done.Add("teleport " + a.TeleportMap); });
            if (a.Spawn.Count > 0)
                Try(c, "spawn", () => { int n = SpawnSpecs(c, q, a.Spawn, stream); if (n > 0) done.Add("spawned " + n); });
            if (done.Count > 0) Log(c, q, "ACTION", when + ": " + string.Join(", ", done));
        }
        static bool DoTeleport(Client.GameClient c, uint map, ushort x, ushort y, uint dyn)
        {
            var gm = map != 0 ? Pool.ServerMaps[map] : null;// lazily loads a known (or remapped) map, null when unknown
            if (gm == null || !NearestValid(gm, ref x, ref y))
            {
                Msg(c, "[Quest] This place is not open yet.");
                return false;
            }
            Move(c, x, y, map, dyn);
            return true;
        }
        /// <summary>Test seam: the in-process harness replaces the real GameClient.Teleport (null on the server).</summary>
        public static Action<Client.GameClient, ushort, ushort, uint, uint> TeleportOverride;
        static void Move(Client.GameClient c, ushort x, ushort y, uint map, uint dyn = 0)
        {
            var o = TeleportOverride;
            if (o != null) o(c, x, y, map, dyn);
            else c.Teleport(x, y, map, dyn);
        }
        /// <summary>The cell itself or the nearest walkable cell within 8 (quest points often sit in an NPC's footprint).</summary>
        static bool NearestValid(Role.GameMap gm, ref ushort x, ref ushort y)
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

        // ---- personal spawns
        sealed class SpawnRec
        {
            public uint Owner, Quest;
            public Game.MsgMonster.MonsterRole Mob;
            public DateTime Expires;
            public bool Killed;
            public string Instance;
        }
        static readonly ConcurrentDictionary<uint, SpawnRec> Spawns = new ConcurrentDictionary<uint, SpawnRec>();
        static readonly ConcurrentDictionary<string, DateTime> SpawnCooldown = new ConcurrentDictionary<string, DateTime>();
        const int RespawnCooldownSeconds = 30;
        static volatile Dictionary<string, Game.MsgMonster.MonsterFamily> FamilyByName;
        static int FamilyCount = -1;

        static Game.MsgMonster.MonsterFamily Family(string norm)
        {
            var map = FamilyByName;
            if (map == null || Pool.MonsterFamilies == null || Pool.MonsterFamilies.Count != FamilyCount)
            {
                map = new Dictionary<string, Game.MsgMonster.MonsterFamily>();
                if (Pool.MonsterFamilies != null)
                    foreach (var f in Pool.MonsterFamilies.Values.ToArray().OrderBy(f => f.ID))
                        if (f != null && !string.IsNullOrEmpty(f.Name)) { string n = QuestGrammar.Norm(f.Name); if (!map.ContainsKey(n)) map[n] = f; }
                FamilyByName = map; FamilyCount = Pool.MonsterFamilies != null ? Pool.MonsterFamilies.Count : 0;
            }
            Game.MsgMonster.MonsterFamily fam;
            return map.TryGetValue(norm, out fam) ? fam : null;
        }
        static int LiveSpawns(uint owner, uint quest)
        {
            int n = 0;
            foreach (var r in Spawns.Values) if (r.Owner == owner && r.Quest == quest && !r.Killed && r.Mob != null && r.Mob.Alive) n++;
            return n;
        }
        /// <summary>[NinjaChain 2026-10-14] live spawns of ONE monster of the quest: a quest that spawns several different
        /// monsters (4361 seven Shichinin, 4366 Fumataro + Fumataro(Mad)) gets each of them, not only the first spec.</summary>
        static int LiveSpawns(uint owner, uint quest, string norm)
        {
            int n = 0;
            foreach (var r in Spawns.Values) if (r.Owner == owner && r.Quest == quest && !r.Killed && r.Mob != null && r.Mob.Alive && QuestGrammar.Norm(r.Mob.Name) == norm) n++;
            return n;
        }
        static bool HasSpawnActions(EngineQuest q) { return q.AllActions.Any(a => a.Spawn.Count > 0); }
        /// <summary>The player may call the quest's story boss out again (it died elsewhere / despawned / was stolen).</summary>
        static bool CanRespawn(Client.GameClient c, EngineQuest q)
        {
            if (!q.Owned || !HasSpawnActions(q) || !c.Player.QuestGUI.IsActiveQuest(q.MissionId)) return false;
            if (IsComplete(c, q, true)) return false;
            if (LiveSpawns(c.Player.UID, q.MissionId) > 0) return false;
            DateTime until;
            return !SpawnCooldown.TryGetValue(c.Player.UID + ":" + q.MissionId, out until) || DateTime.Now >= until;
        }
        static int SpawnAll(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream)
        {
            var specs = new List<SpawnSpec>();
            if (q.OnAccept != null) specs.AddRange(q.OnAccept.Spawn);
            if (q.OnComplete != null) specs.AddRange(q.OnComplete.Spawn);
            return SpawnSpecs(c, q, specs, stream);
        }
        static int SpawnSpecs(Client.GameClient c, EngineQuest q, List<SpawnSpec> specs, ServerSockets.Packet stream)
        {
            string key = c.Player.UID + ":" + q.MissionId;
            int made = 0;
            foreach (var sp in specs)
            {
                uint map = sp.Map != 0 ? sp.Map : c.Player.Map;
                uint dyn = c.Player.Map == map ? c.Player.DynamicID : 0;// inside a private copy: spawn in it
                int allowed = sp.Count - LiveSpawns(c.Player.UID, q.MissionId, sp.Norm);// [NinjaChain 2026-10-14] per monster
                for (int i = 0; i < allowed; i++)
                    if (SpawnOne(c.Player.UID, q.MissionId, sp, map, dyn, null, stream) != null) made++;
            }
            SpawnCooldown[key] = DateTime.Now.AddSeconds(RespawnCooldownSeconds);
            if (made > 0) Msg(c, "[Quest] " + q.Name + ": " + string.Join(", ", specs.Select(s => s.Name).Distinct()) + " appeared" + (specs.Count > 0 && specs[0].Map != 0 && specs[0].Map != c.Player.Map ? " at map " + specs[0].Map + " (" + specs[0].X + "," + specs[0].Y + ")" : "") + ". It leaves in " + (specs.Count > 0 ? specs[0].Minutes : 10) + " minutes.");
            return made;
        }
        static Game.MsgMonster.MonsterRole SpawnOne(uint owner, uint quest, SpawnSpec sp, uint mapId, uint dyn, string instance, ServerSockets.Packet stream)
        {
            var fam = Family(sp.Norm);
            Game.MsgMonster.MonsterFamily exact;// [NinjaChain 2026-10-14] "monsterId": one of several types with the same name
            if (sp.MonsterId != 0 && Pool.MonsterFamilies != null && Pool.MonsterFamilies.TryGetValue(sp.MonsterId, out exact) && exact != null && QuestGrammar.Norm(exact.Name) == sp.Norm)
                fam = exact;
            if (fam == null) return null;
            var map = Pool.ServerMaps[mapId];
            if (map == null) return null;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;
            var copy = fam.Copy();
            copy.SpawnX = sp.X; copy.SpawnY = sp.Y;
            copy.MaxSpawnX = (ushort)(sp.X + Math.Max((ushort)1, sp.Range)); copy.MaxSpawnY = (ushort)(sp.Y + Math.Max((ushort)1, sp.Range));
            copy.MapID = map.ID; copy.SpawnCount = 1;
            Game.MsgMonster.MonsterRole mob = null;
            for (int tries = 0; tries < 4 && mob == null; tries++)
                mob = map.MonstersColletion.Add(copy, true, dyn, true);// RemoveOnDead: never respawns
            if (mob == null) return null;
            mob.RemoveOnDead = true;
            Spawns[mob.UID] = new SpawnRec { Owner = owner, Quest = quest, Mob = mob, Expires = DateTime.Now.AddMinutes(sp.Minutes), Instance = instance };
            try
            {
                var action = new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y };
                mob.Send(stream.ActionCreate(action));
                mob.Send(mob.GetArray(stream, false));
            }
            catch { }
            return mob;
        }
        /// <summary>MonsterRole.Dead: true (and forgotten) for an engine-spawned monster = no loot / no event drops.</summary>
        public static bool ConsumeQuestSpawn(Game.MsgMonster.MonsterRole mob)
        {
            SpawnRec r;
            return mob != null && Spawns.TryRemove(mob.UID, out r);
        }
        public static bool IsQuestSpawn(Game.MsgMonster.MonsterRole mob) { return mob != null && Spawns.ContainsKey(mob.UID); }
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
        static void DespawnQuest(uint owner, uint quest)
        {
            foreach (var kv in Spawns.ToArray())
                if (kv.Value.Owner == owner && kv.Value.Quest == quest)
                {
                    SpawnRec r;
                    if (Spawns.TryRemove(kv.Key, out r) && r.Mob != null && r.Mob.Alive) Despawn(r.Mob);
                }
        }

        // ---- instances (R8): a private DynamicID copy of an existing map
        sealed class InstanceRec
        {
            public uint Owner, Quest, Map, Dyn;
            public InstanceDef Def;
            public DateTime Expires, LastInside;
            public readonly List<Game.MsgNpc.Npc> Npcs = new List<Game.MsgNpc.Npc>();// [InstanceNpc] this copy's NPCs
            public uint HandOver;   // [InstanceNpc] the auto-accepted next quest that continues in this same copy
        }
        static readonly ConcurrentDictionary<uint, InstanceRec> Instances = new ConcurrentDictionary<uint, InstanceRec>();
        static bool Inside(Client.GameClient c, InstanceRec r) { return c != null && c.Player != null && c.Player.Map == r.Map && c.Player.DynamicID == r.Dyn; }

        /// <summary>[DreamTheater 2026-10-15] Re-enters the private copy of an ACTIVE quest whose onAccept opens one (the player left it
        /// by a scroll / admin move / timeout). False = not active, no copy, or the map is closed.</summary>
        public static bool ReenterInstance(Client.GameClient c, uint quest)
        {
            var t = Current; EngineQuest q; InstanceDef inst;
            if (t == null || c == null || c.Player == null || !c.Player.QuestGUI.IsActiveQuest(quest)) return false;
            if (!t.Quests.TryGetValue(quest, out q) || q.OnAccept == null || q.OnAccept.Instance == null || !t.Instances.TryGetValue(q.OnAccept.Instance, out inst)) return false;
            using (var rec = new ServerSockets.RecycledPacket()) return EnterInstance(c, inst, quest, rec.GetStream());
        }
        static bool EnterInstance(Client.GameClient c, InstanceDef def, uint quest, ServerSockets.Packet stream)
        {
            var gm = Pool.ServerMaps[def.Map];
            ushort ex = def.X, ey = def.Y;
            if (gm == null || !NearestValid(gm, ref ex, ref ey)) { Msg(c, "[Quest] This place is not open yet."); return false; }
            InstanceRec old;
            if (Instances.TryGetValue(c.Player.UID, out old))
            {
                if (old.Def.Name == def.Name && old.HandOver != 0 && old.HandOver == quest && DateTime.Now < old.Expires && Inside(c, old))
                {
                    // [InstanceNpc] a chapter of several quests in one copy: the next quest takes the copy over
                    old.Quest = quest; old.HandOver = 0; old.LastInside = DateTime.Now;
                    return true;
                }
                if (old.Def.Name == def.Name && old.Quest == quest && DateTime.Now < old.Expires)
                {
                    // re-enter the same copy (relog / death)
                    old.LastInside = DateTime.Now;
                    Move(c, ex, ey, def.Map, old.Dyn);
                    return true;
                }
                CloseInstance(old, false);
            }
            uint dyn = gm.GenerateDynamicID();
            var r = new InstanceRec { Owner = c.Player.UID, Quest = quest, Map = def.Map, Dyn = dyn, Def = def, Expires = DateTime.Now.AddMinutes(def.Minutes), LastInside = DateTime.Now };
            Instances[c.Player.UID] = r;
            PlaceInstanceNpcs(gm, r);// before the move: the arrival view scan shows them
            Move(c, ex, ey, def.Map, dyn);
            foreach (var sp in def.Monsters)
                for (int i = 0; i < sp.Count; i++)
                    SpawnOne(c.Player.UID, quest, sp, def.Map, dyn, def.Name, stream);
            Msg(c, "[Quest] You entered " + def.Name + " (private, " + def.Minutes + " minutes).");
            return true;
        }
        static bool LeaveInstance(Client.GameClient c, bool teleport)
        {
            InstanceRec r;
            if (!Instances.TryRemove(c.Player.UID, out r)) return false;
            bool inside = Inside(c, r);
            CloseInstance(r, false);
            if (teleport && inside) DoTeleport(c, r.Def.ExitMap, r.Def.ExitX, r.Def.ExitY, 0);
            return true;
        }
        /// <summary>[InstanceNpc] one Npc object per definition, on the copy's DynamicID (only players inside the copy see
        /// it: RoleView.CanSee / Npc.CanSee). The DMap cells of the shared base map are NOT changed.</summary>
        static void PlaceInstanceNpcs(Role.GameMap gm, InstanceRec r)
        {
            if (r.Def.Npcs.Count == 0) return;
            lock (r.Npcs)
                foreach (var d in r.Def.Npcs)
                {
                    try
                    {
                        ushort x = d.X, y = d.Y;
                        if (x >= gm.bounds.Width || y >= gm.bounds.Height) continue;
                        var np = Game.MsgNpc.Npc.Create();
                        np.UID = d.Uid;
                        np.NpcType = (Role.Flags.NpcType)d.Type;
                        np.Mesh = d.Mesh;
                        np.Map = r.Map;
                        np.X = x; np.Y = y;
                        np.DynamicID = r.Dyn;
                        np.AllowDynamic = false;
                        np.Name = d.Name;
                        gm.View.EnterMap<Role.IMapObj>(np);
                        r.Npcs.Add(np);
                        using (var rec = new ServerSockets.RecycledPacket())
                            np.Send(rec.GetStream());// a player already inside (re-placed) sees it at once
                    }
                    catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                }
        }
        static void RemoveInstanceNpcs(InstanceRec r)
        {
            Game.MsgNpc.Npc[] list;
            lock (r.Npcs) { list = r.Npcs.ToArray(); r.Npcs.Clear(); }
            if (list.Length == 0) return;
            Role.GameMap gm;
            if (Pool.ServerMaps == null || !Pool.ServerMaps.TryGetValue(r.Map, out gm) || gm == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var np in list)
                {
                    try
                    {
                        gm.View.LeaveMap<Role.IMapObj>(np);
                        var remove = stream.ActionCreate(new ActionQuery { ObjId = np.UID, Type = ActionType.RemoveEntity });
                        foreach (var p in gm.View.Roles(Role.MapObjectType.Player, np.X, np.Y))
                            if (np.CanSee(p)) (p as Role.Player).Send(remove);
                    }
                    catch { }
                }
            }
        }
        /// <summary>[InstanceNpc] closes (players sent to the exit) every copy whose instance is no longer in the table.</summary>
        static int CloseOrphanInstances(QuestTable t)
        {
            int n = 0;
            foreach (var kv in Instances.ToArray())
            {
                if (t != null && t.Instances.ContainsKey(kv.Value.Def.Name)) continue;
                try { CloseInstance(kv.Value, true); n++; } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            }
            return n;
        }
        /// <summary>[InstanceNpc] live NPC objects of a player's current instance copy (harness / diagnostics).</summary>
        public static List<Game.MsgNpc.Npc> InstanceNpcsOf(uint owner)
        {
            InstanceRec r;
            if (!Instances.TryGetValue(owner, out r)) return new List<Game.MsgNpc.Npc>();
            lock (r.Npcs) return r.Npcs.ToList();
        }
        /// <summary>The DynamicID to keep for a teleport to <paramref name="map"/>: the player's own copy when they are
        /// inside it and the target is the same map (an NPC / action teleport inside the instance), else 0.</summary>
        static uint KeepDyn(Client.GameClient c, uint map)
        {
            InstanceRec r;
            return c != null && c.Player != null && Instances.TryGetValue(c.Player.UID, out r) && Inside(c, r) && r.Map == map ? r.Dyn : 0;
        }

        static void CloseInstance(InstanceRec r, bool kick)
        {
            InstanceRec cur;
            if (Instances.TryGetValue(r.Owner, out cur) && cur == r) Instances.TryRemove(r.Owner, out cur);
            RemoveInstanceNpcs(r);
            foreach (var kv in Spawns.ToArray())
                if (kv.Value.Owner == r.Owner && kv.Value.Mob != null && kv.Value.Mob.Map == r.Map && kv.Value.Mob.DynamicID == r.Dyn)
                {
                    SpawnRec s;
                    if (Spawns.TryRemove(kv.Key, out s) && s.Mob.Alive) Despawn(s.Mob);
                }
            if (kick)
            {
                Client.GameClient c;
                if (Pool.GamePoll.TryGetValue(r.Owner, out c) && Inside(c, r))
                {
                    Msg(c, "[Quest] Your time in " + r.Def.Name + " is over.");
                    try { DoTeleport(c, r.Def.ExitMap, r.Def.ExitX, r.Def.ExitY, 0); } catch { }
                }
            }
        }

        // ---- cleanup (from OnTick, at most every 5 seconds)
        static long _lastSweep;
        static void Sweep()
        {
            long now = DateTime.UtcNow.Ticks, last = System.Threading.Interlocked.Read(ref _lastSweep);
            if (now - last < TimeSpan.TicksPerSecond * 5) return;
            if (System.Threading.Interlocked.CompareExchange(ref _lastSweep, now, last) != last) return;
            var time = DateTime.Now;
            foreach (var kv in Spawns.ToArray())
            {
                var r = kv.Value;
                if (r.Mob == null || !r.Mob.Alive || r.Killed) { if (r.Mob == null || !r.Mob.Alive) Spawns.TryRemove(kv.Key, out r); continue; }
                bool expired = time > r.Expires;
                if (!expired)
                {
                    // the owner logged out or dropped the quest: the boss leaves
                    Client.GameClient o;
                    expired = !Pool.GamePoll.TryGetValue(r.Owner, out o) || o.Player == null || !o.Player.QuestGUI.IsActiveQuest(r.Quest);
                }
                if (expired && Spawns.TryRemove(kv.Key, out r)) Despawn(r.Mob);
            }
            foreach (var kv in Instances.ToArray())
            {
                var r = kv.Value;
                Client.GameClient o;
                bool online = Pool.GamePoll.TryGetValue(r.Owner, out o) && o.Player != null;
                if (online && Inside(o, r)) r.LastInside = time;
                if (time > r.Expires) CloseInstance(r, true);
                else if ((time - r.LastInside).TotalMinutes >= 3) CloseInstance(r, false);
            }
        }

        // ------------------------------------------------------------------ finishing
        /// <summary>Bag slots the prize needs (stackables share stacks; the old stack is not counted on purpose).</summary>
        static int SlotsFor(List<PrizeItem> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                Database.ItemType.DBItem db;
                if (!Pool.ItemsBase.TryGetValue(i.Id, out db)) continue;
                if (db.StackSize > 1) n += (i.Count + db.StackSize - 1) / db.StackSize;
                else n += i.Count;
            }
            return n;
        }
        static List<PrizeItem> PrizeItems(EngineQuest q, Client.GameClient c)
        {
            var items = new List<PrizeItem>();
            // [QuestEngineFix 2026-10-14] unknown item ids are dropped BEFORE the random pick (the pick could land on one
            // and pay nothing), and "one of the following weapons" picks a weapon of the player's class
            var avail = q.Prize.Items.Where(i => Pool.ItemsBase.ContainsKey(i.Id)).ToList();
            if (avail.Count > 0)
            {
                if (q.Prize.RandomItem) items.Add(PickRandom(q, c, avail));
                else items.AddRange(avail);
            }
            if (q.ChainEnd) items.AddRange(q.Prize.FinalItems.Where(i => Pool.ItemsBase.ContainsKey(i.Id)));
            return items;
        }

        static readonly System.Text.RegularExpressions.Regex WeaponWord = new System.Text.RegularExpressions.Regex(@"\bweapons?\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);
        /// <summary>[QuestEngineFix] one random item; a weapon list ("One of the following weapons", 611) gives a weapon
        /// of the player's class (class-restricted items it can use, else the unrestricted weapon types of its class,
        /// else any weapon it can use, else any of them).</summary>
        static PrizeItem PickRandom(EngineQuest q, Client.GameClient c, List<PrizeItem> avail)
        {
            var pool = avail;
            try
            {
                if (c != null && c.Player != null && avail.Count > 1 && WeaponWord.IsMatch(q.Prize.Text ?? "") && avail.All(i => IsWeaponId(i.Id)))
                {
                    var fit = avail.Where(i => FitsClass(c, i.Id, true)).ToList();
                    if (fit.Count == 0) fit = avail.Where(i => FitsClass(c, i.Id, false)).ToList();
                    if (fit.Count > 0) pool = fit;
                }
            }
            catch { pool = avail; }
            return pool[Program.GetRandom.Next(pool.Count)];
        }
        static bool IsWeaponId(uint id) { uint t = id / 100000; return t == 4 || t == 5 || t == 6; }
        /// <summary>Unrestricted (itemtype class 0) weapon types (id / 1000) that a class family really uses.</summary>
        static uint[] ClassWeaponTypes(uint cls)
        {
            if (Database.AtributesStatus.IsTrojan(cls)) return new uint[] { 410, 420, 480 };
            if (Database.AtributesStatus.IsWarrior(cls)) return new uint[] { 410, 420, 480, 510, 530, 560, 561, 580 };
            if (Database.AtributesStatus.IsTaoist(cls)) return new uint[] { 421 };
            if (Database.AtributesStatus.IsArcher(cls)) return new uint[] { 500 };
            if (Database.AtributesStatus.IsNinja(cls)) return new uint[] { 601, 616 };
            if (Database.AtributesStatus.IsMonk(cls)) return new uint[] { 610, 622 };
            if (Database.AtributesStatus.IsPirate(cls)) return new uint[] { 611, 612 };
            if (Database.AtributesStatus.IsLee(cls)) return new uint[] { 617 };
            if (Database.AtributesStatus.IsWindWalker(cls)) return new uint[] { 626 };
            return new uint[0];
        }
        static bool FitsClass(Client.GameClient c, uint id, bool strict)
        {
            Database.ItemType.DBItem db;
            if (!Pool.ItemsBase.TryGetValue(id, out db) || db == null) return false;
            if (db.Class != 0) return Database.ItemType.EquipPassJobReq(db, c);
            return !strict || ClassWeaponTypes(c.Player.Class).Contains(id / 1000);
        }

        /// <summary>
        /// [QuestEngineFix 2026-10-14] Quests.SendSinglePacket(FinishQuest) added a full EXP ball (600) to EVERY quest finish
        /// under level 130 - official Conquer does not. Engine-owned quests (their prize is the official Prize text) and
        /// Bounty Hall missions get no such ball any more (a daily with no reward took a test player from level 9 to 25,
        /// the ~60 ninja talk steps paid ~60 balls). It stays only for the old hand-written quests (not engine-owned)
        /// whose official prize gives EXP: their scripts were written around that ball and many of them pay no EXP of
        /// their own, so removing it would leave them below the official reward. A script that pays its own EXP
        /// (HandCodedQuests.ScriptPaysExp) gets no ball either: official gives only that scripted reward.
        /// </summary>
        static readonly HashSet<uint> ScriptPaysExp = new HashSet<uint>(HandCodedQuests.ScriptPaysExp);
        public static bool PaysFinishExpBall(uint id)
        {
            try
            {
                if (Role.Instance.Quests.IsBountyHallMission(id)) return false;
                if (ScriptPaysExp.Contains(id)) return false;
                var t = Current;
                if (t == null) return true;// engine not loaded: the old behaviour
                EngineQuest q;
                if (!t.Quests.TryGetValue(id, out q)) return false;
                if (q.Owned || !q.HandCoded) return false;
                return ExpWord.IsMatch(q.OfficialPrizeText ?? "");
            }
            catch { return false; }
        }
        static readonly System.Text.RegularExpressions.Regex ExpWord = new System.Text.RegularExpressions.Regex(@"\bEXP\b|experience", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Server-side finish of an engine quest at (or with) its finish NPC: every objective, the bag space, then
        /// FinishQuest (once), then the collected items are taken and the prize is given.
        /// </summary>
        public static bool TryFinish(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream, uint atNpc, out string message)
        {
            message = "";
            var gui = c.Player.QuestGUI;
            if (!q.Owned || !Usable(q) || q.IsBounty || q.OfferOnly) { message = "I can't help you with that."; return false; }
            if (!gui.IsActiveQuest(q.MissionId)) { message = "You don't have this quest."; return false; }
            if (q.FinishNpc != 0 && atNpc != q.FinishNpc) { message = "Report to the right person to finish this quest."; return false; }
            SyncStates(c, q, stream);
            if (!IsComplete(c, q, atNpc == q.FinishNpc && q.FinishNpc != 0))
            {
                message = "You are not done yet: " + string.Join(", ", ProgressLines(c, q, true).Where(l => !l.StartsWith("[done]"))) + ".";
                return false;
            }
            var items = PrizeItems(q, c);
            int slots = SlotsFor(items);
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots)))
            {
                message = "Please make " + slots + " free space(s) in your inventory first.";
                return false;
            }
            bool finished;
            try { finished = gui.FinishQuest(q.MissionId); }// Accepted -> Finished exactly once (the reward gate)
            catch (Exception e)
            {
                // FinishQuest's own 600 exp-ball can throw after the status already changed: then it IS finished
                finished = gui.CheckQuest(q.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && !gui.IsActiveQuest(q.MissionId);
                Log(c, q, "WARN", "FinishQuest threw " + e.GetType().Name + ": " + e.Message);
            }
            if (!finished)
            {
                message = "You don't have this quest.";
                return false;
            }
            string taken = TakeItems(c, q, stream);
            string given = Grant(c, q.Prize, items, stream);
            string extra = q.Extra != null ? GrantExtra(c, q.Extra) : "";
            if (extra.Length > 0) given = given.Length > 0 ? given + ", " + extra : extra;
            message = given.Length > 0 ? "You received: " + given + "." : "";
            Log(c, q, "FINISH", "npc=" + atNpc + " took=[" + taken + "] gave=[" + given + "]");
            global::Qahr.Mobile.Phone.MobileQuestStoryNpc.OnEngineFinished(c, q.MissionId, stream);// QahrMobile: main-story reward + next step
            if (q.Festival != null) { try { var ff = FestivalQuestFinished; if (ff != null) ff(c, q.Festival, q.MissionId, given); } catch { } }// [Festivals]
            Visited.TryRemove(c.Player.UID, out _);
            CompleteFired.TryRemove(c.Player.UID + ":" + q.MissionId, out _);
            DespawnQuest(c.Player.UID, q.MissionId);
            InstanceRec ir;
            bool ownInstance = Instances.TryGetValue(c.Player.UID, out ir) && ir.Quest == q.MissionId;
            if (q.OnFinish != null) RunActions(c, q, q.OnFinish, stream, "finish");
            if (ownInstance && Instances.TryGetValue(c.Player.UID, out ir) && ir.Quest == q.MissionId)
            {
                // [InstanceNpc] the auto-accepted next quest enters the same instance: the player stays in this copy
                var t = Current; EngineQuest nx;
                if (q.NextQuest != 0 && q.AutoAccept && Inside(c, ir) && t != null && t.Quests.TryGetValue(q.NextQuest, out nx) && nx.OnAccept != null
                    && nx.OnAccept.Instance != null && string.Equals(nx.OnAccept.Instance, ir.Def.Name, StringComparison.OrdinalIgnoreCase))
                    ir.HandOver = q.NextQuest;
                else LeaveInstance(c, true);
            }
            return true;
        }

        static string TakeItems(Client.GameClient c, EngineQuest q, ServerSockets.Packet stream)
        {
            var parts = new List<string>();
            foreach (var o in q.Objectives)
            {
                if (o.Kind != ObjectiveKind.Collect || !o.TakeItem || o.ItemId == 0) continue;
                uint left = o.Count;
                foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == o.ItemId).ToArray())
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
                parts.Add(o.ItemId + "x" + (o.Count - left));
            }
            return string.Join(",", parts);
        }

        static string Grant(Client.GameClient c, Prize p, List<PrizeItem> items, ServerSockets.Packet stream)
        {
            // each part on its own: a failure in one (logged) never cancels the others
            var parts = new List<string>();
            if (p.Exp > 0)
                Try(c, "exp", () => { c.IncreaseExperience(stream, p.Exp, Role.Flags.ExperienceEffect.angelwing, true); parts.Add(p.Exp + " EXP"); });// [QuestEngineFix] questReward: the official amount (no server rate x2 / EXP potion / rainbow gem)
            if (p.ExpBallUnits > 0)
                Try(c, "expball", () => { c.GainExpBall(p.ExpBallUnits, true); parts.Add((p.ExpBallUnits / 600.0).ToString("0.##", CultureInfo.InvariantCulture) + " EXP ball(s)"); });
            if (p.Silver > 0)
                Try(c, "silver", () => { c.Player.Money += p.Silver; parts.Add(p.Silver + " silver"); c.Player.SendUpdate(stream, c.Player.Money, MsgUpdate.DataType.Money); });
            if (p.Cps > 0 && p.FromJson)// ER-9: CPs only from an explicit quests.json prize (parsed text CPs are zeroed at load anyway)
                Try(c, "cps", () => { c.Player.ConquerPoints += p.Cps; parts.Add(p.Cps + " CPs"); });
            foreach (var i in items)
            {
                var it = i;
                Try(c, "item " + it.Id, () =>
                {
                    if (c.Inventory.AddItemWitchStack(it.Id, 0, it.Count, stream, it.Bound))
                        parts.Add(QuestGrammar.ItemName(it.Id) + (it.Count > 1 ? " x" + it.Count : ""));
                });
            }
            return string.Join(", ", parts);
        }
        /// <summary>R4: Ninja Eight Gates / unlock and Archives war suits (each once; skipped when already owned).</summary>
        static string GrantExtra(Client.GameClient c, PrizeExtra ex)
        {
            var parts = new List<string>();
            Try(c, "ninja", () =>
            {
                if ((ex.NinjaUnlock || ex.NinjaGates.Count > 0) && c.MyNinja != null && Database.AtributesStatus.IsNinja(c.Player.Class))
                {
                    if (ex.NinjaUnlock && !c.MyNinja.Unlocked) { c.MyNinja.Unlocked = true; parts.Add("Ninja Archives unlocked"); }
                    foreach (var g in ex.NinjaGates)
                    {
                        Role.Instance.Ninja.Flags fl;
                        if (!Enum.TryParse(g, out fl)) continue;
                        if (!c.MyNinja.ContainsFlag((ulong)fl)) { c.MyNinja.AddFlag((ulong)fl); parts.Add(g.Replace('_', ' ')); }
                    }
                }
            });
            Try(c, "archives", () =>
            {
                if (c.MyArchives == null) return;
                foreach (var a in ex.Archives)
                {
                    Role.Instance.Archives.TypeID ty;
                    if (!Enum.TryParse(a, out ty) || c.MyArchives.Items.ContainsKey(ty)) continue;
                    c.MyArchives.AddItem(ty, 1, 0, 1, 0);
                    parts.Add(a + " archive");
                }
            });
            return string.Join(", ", parts);
        }
        static void Try(Client.GameClient c, string what, Action a)
        {
            try { a(); }
            catch (Exception e)
            {
                try { File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | ERROR | " + what + " for " + c.Player.UID + ": " + e.Message + Environment.NewLine, Encoding.UTF8); } catch { }
            }
        }

        /// <summary>Quest panel "finish" (MsgTaskStatus FinishQuest): accepted only for engine quests, with the finish NPC in view.</summary>
        public static void OnClientFinish(Client.GameClient c, uint id)
        {
            try
            {
                var t = Current;
                EngineQuest q;
                if (t == null || c == null || !t.Quests.TryGetValue(id, out q) || !q.Owned || !q.Workable || q.IsBounty || q.OfferOnly) return;
                uint npc = 0;
                if (q.FinishNpc != 0)
                {
                    Game.MsgNpc.Npc obj;
                    if (c.Map == null || !c.Map.SearchNpcInScreen(q.FinishNpc, c.Player.X, c.Player.Y, c.Player.DynamicID, out obj))
                    {
                        Msg(c, "[Quest] Report to the quest NPC to finish " + q.Name + ".");
                        return;
                    }
                    npc = q.FinishNpc;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    string msg;
                    if (TryFinish(c, q, stream, npc, out msg)) { Msg(c, "[Quest] " + q.Name + " complete. " + msg); FollowUp(c, t, q, stream); }
                    else Msg(c, "[Quest] " + msg);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ Bounty Hall
        /// <summary>ER-7: Quests.RollBountyHallOffers offers only bounties the engine can complete.</summary>
        public static bool IsBountyWorkable(uint id)
        {
            var t = Current;
            EngineQuest q;
            if (t == null) return true;// engine not loaded: old behaviour
            return t.Quests.TryGetValue(id, out q) && !q.Disabled && q.Workable;
        }
        /// <summary>Before FinishBountyHall: re-checks an engine bounty (marks it done when it is) and the bag space. false = refuse.</summary>
        public static bool BeforeBountyFinish(Client.GameClient c, uint id)
        {
            try
            {
                var t = Current;
                EngineQuest q;
                if (t == null || c == null || !t.Quests.TryGetValue(id, out q) || q.Disabled || !q.IsBounty) return true;
                if (!c.Player.QuestGUI.IsActiveQuest(id)) return true;
                if (q.Objectives.Count > 0 && IsComplete(c, q, false))
                    c.Player.QuestGUI.MarkBountyObjectivesDone(id);
                int slots = SlotsFor(PrizeItemsPreview(q));
                if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots)))
                {
                    Msg(c, "[Quest] Please make " + slots + " free space(s) in your inventory to claim " + q.Name + ".");
                    return false;
                }
                return true;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return true; }
        }
        static List<PrizeItem> PrizeItemsPreview(EngineQuest q)
        {
            var items = new List<PrizeItem>();
            var avail = q.Prize.Items.Where(i => Pool.ItemsBase.ContainsKey(i.Id)).ToList();// [QuestEngineFix] existing ids first
            if (avail.Count > 0) items.AddRange(q.Prize.RandomItem ? avail.OrderByDescending(i => i.Count).Take(1) : avail);
            if (q.ChainEnd) items.AddRange(q.Prize.FinalItems);
            return items.Where(i => Pool.ItemsBase.ContainsKey(i.Id)).ToList();
        }
        /// <summary>After a successful FinishBountyHall (it is Finished now, so this runs once): take the items, give the prize.</summary>
        public static void AfterBountyFinish(Client.GameClient c, uint id)
        {
            try
            {
                var t = Current;
                EngineQuest q;
                if (t != null && c != null && t.Quests.TryGetValue(id, out q) && !q.Disabled && q.IsBounty)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string taken = TakeItems(c, q, stream);
                        string given = Grant(c, q.Prize, PrizeItems(q, c), stream);
                        if (given.Length > 0) Msg(c, "[Quest] " + q.Name + ": you received " + given + ".");
                        Log(c, q, "BOUNTY", "took=[" + taken + "] gave=[" + given + "] today=" + c.Player.BountyHallToday);
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            OnEvent(c, "bountyDone");// R7: quests that ask for a finished bounty
        }

        // ------------------------------------------------------------------ daily / weekly reset (ER-8 / ER-7)
        /// <summary>
        /// Daily reset (both paths: players online at midnight and the login catch-up): removes the finished or active
        /// copy of every repeatable engine quest ("repeat" in quests.json, or a [Daily]/[Weekly] name of an engine-owned
        /// quest), so it can be taken again. Weekly ones only when a Monday passed. lastDayOfYear = Player.Day before the
        /// reset, or -1 when exactly one day passed. Hand-coded quests are reset only when quests.json asks for it.
        /// </summary>
        public static void ResetRepeatables(Client.GameClient c, int lastDayOfYear)
        {
            try
            {
                var t = Current;
                if (t == null || c == null || c.Player == null || c.Player.QuestGUI == null) return;
                var today = DateTime.Now.Date;
                int days = 1;
                if (lastDayOfYear > 0)
                {
                    days = today.DayOfYear - lastDayOfYear;
                    if (days <= 0) days += 365;
                }
                bool weekly = days >= 7;
                for (int i = 0; i < days && !weekly; i++) if (today.AddDays(-i).DayOfWeek == DayOfWeek.Monday) weekly = true;
                var removed = new List<uint>();
                foreach (uint id in AllKeys(c.Player.QuestGUI))
                {
                    EngineQuest q;
                    if (!t.Quests.TryGetValue(id, out q) || q.Repeat == 0 || q.IsBounty || (q.Disabled && !q.FestivalOff)) continue;// [Festivals] a stopped festival's repeat quests still reset
                    if (!q.RepeatExplicit && (!q.Owned || q.HandCoded)) continue;
                    if (q.Repeat == 2 && !weekly) continue;
                    c.Player.QuestGUI.RemoveQuest(id);
                    CompleteFired.TryRemove(c.Player.UID + ":" + id, out _);
                    removed.Add(id);
                }
                if (removed.Count > 0) Visited.TryRemove(c.Player.UID, out _);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ commands / log
        public static List<string> Describe(uint id)
        {
            var lines = new List<string>();
            var t = Current;
            EngineQuest q;
            if (t == null) { lines.Add("[QuestEngine] not loaded."); return lines; }
            if (!t.Quests.TryGetValue(id, out q)) { lines.Add("[QuestEngine] quest " + id + " is not in Questinfo.ini."); return lines; }
            lines.Add("[Quest " + id + "] " + q.Name + " type " + q.TypeId + " begin npc " + q.BeginNpc + " finish npc " + q.FinishNpc + (q.Team ? " team-kills" : "") + (q.IsBounty ? " BOUNTY" : "") + (q.Repeat == 1 ? " daily" : q.Repeat == 2 ? " weekly" : "") + (q.OfferOnly ? " offer-only" : "") + (q.Pool != null ? " pool " + q.Pool : "") + (q.RequireClan ? " clan" : ""));
            lines.Add("  status: " + (q.Disabled ? "disabled by quests.json" : q.Owned ? (q.Workable ? "ENGINE (workable)" : "ENGINE (not workable)") : "hand-coded" + (q.EngineCounts ? " + engine counting" : "")) + (q.FromJson ? " [quests.json]" : "") + (q.SelfCounted ? " [code counts itself]" : "") + (q.ForceCount ? " [forced counting]" : ""));
            for (int i = 0; i < q.Objectives.Count; i++) lines.Add("  " + (i + 1) + ". " + q.Objectives[i].Describe());
            if (q.Objectives.Count == 0) lines.Add("  (no objectives: finish at the finish NPC)");
            lines.Add("  prize: " + q.Prize.Describe(q.ChainEnd) + (q.ChainEnd ? "" : " (final items only at the chain end)") + (q.Extra != null ? " + " + q.Extra : ""));
            if (q.OnAccept != null) lines.Add("  on accept: " + q.OnAccept);
            if (q.OnComplete != null) lines.Add("  on complete: " + q.OnComplete);
            if (q.OnFinish != null) lines.Add("  on finish: " + q.OnFinish);
            if (q.PrequestOverride != null) lines.Add("  prequest (quests.json): '" + q.PrequestOverride + "'");
            foreach (var p in q.Problems.Take(5)) lines.Add("  problem: " + p);
            if (q.NextQuest != 0) lines.Add("  next: " + q.NextQuest + (q.AutoAccept ? " (auto-accept)" : ""));
            return lines;
        }
        public static void CommandQuestInfo(Client.GameClient c, string arg)
        {
            uint id;
            if (!uint.TryParse((arg ?? "").Trim(), out id)) { c.SendSysMesage("usage: @questinfo <MissionId>"); return; }
            foreach (var l in Describe(id))
                c.SendSysMesage(l.Length > 240 ? l.Substring(0, 237) + "..." : l, MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
            if (c.Player.QuestGUI.src.ContainsKey(id))
            {
                var item = c.Player.QuestGUI.src[id];
                c.SendSysMesage("  you: " + item.Status + " progress [" + string.Join(",", item.Intentions ?? new uint[0]) + "]", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
            }
        }

        static void Log(Client.GameClient c, EngineQuest q, string what, string detail)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | " + what + " | " + q.MissionId + " " + q.Name + " | " +
                              c.Player.UID + " " + c.Player.Name + " lv" + c.Player.Level + " | " + detail + Environment.NewLine;
                lock (_logLock)
                    File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"), line, Encoding.UTF8);
            }
            catch { }
        }
    }
}
