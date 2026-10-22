// ============================================================================
//  QahrMobile: the mobile's daily Bounty Hall of Chinju (NPC 30893, Twin City 441,290), window DlgGangBounty
//  (luaui\dialog\dlggangbountyui). First version, backed by the quest engine (kill / collect counting, 2339 counters)
//  and the engine's daily reset (quests.json "repeat": "daily" on the TypeId 3 mobile quests).
//
//  Client -> server (MsgAction 258): "</F>ClientManager_Callback</N>10083</S>TaskAssemblyGfit_<Func>[</N>id</N>page]"
//      OpenDialogMain | AcceptTask id page | CompleteTask id page | DelTask id page (give up) | Refresh | Find id page (Go)
//      The quest tracker's goal link "[lua X,</F>TaskAssemblyGfit_Find]" sends the same Find without the callback.
//  Server -> client (dwParam 10083): DlgGangBounty_Open | DlgGangBountyAceeptTaskBack | DlgGangBountyCompleteTaskBack |
//      DlgGangBountyGiveUpTaskBack | DlgGangBountyReloadBack, each "{['Code']=..,['Data']={..}}" (gangbountyfunc.lua).
//      Task "Statue": 0 offered, 1 accepted, 2 done (claim), 3 claimed. Star 1/2/3/5 (4 has no art). Type 2 kill, 3 collect.
//
//  Rules (luaUiRes[1240] "rulecontenttext"): 3 offers at a time, the next 3 after the previous 3 are claimed, 6 per day,
//  one accepted at a time, 1 free refresh a day (then 10 CPs (B) each, at most 30), all reset at the first use of a new day.
//  Offers: the Chinju quests the engine can complete (Workable, only kill / collect objectives, a title in the client's
//  luaUiRes[1240].TaskInfo), level band = the highest Lv_min the player reaches, class / Lv_max checked.
//  Rewards per star: Data\mobile_quests\bounty_rewards.txt ("star expPct silver items"; items "id[xN];..." bound).
//  State per player: Data\mobile_quests\bounty_state.txt (one line per player, rewritten on change).
//  Mobile clients only (PC players keep the engine's NPC dialog). Notes: docs\quests\quest-story.md.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qahr.Mobile.QuestEngine;

namespace Qahr.Mobile.Phone
{
    public static class MobileQuestBounty
    {
        public const uint ChinjuNpc = 30893, MsgId = 10083;
        public const int MaxPerDay = 6, Offers = 3, FreeRefresh = 1, MaxReload = 30, RefreshCost = 10;
        const string Prefix = "TaskAssemblyGfit_";

        sealed class Slot { public uint Id; public byte Star = 1, Type = 2, State; }
        sealed class State
        {
            public int Day; public int Complete, Reload, Free = FreeRefresh;
            public Slot[] Slots = new Slot[0];
        }
        sealed class Cand { public uint Id; public int LvMin, LvMax; public byte Star, Type; public string Target = ""; }

        static readonly ConcurrentDictionary<uint, State> states = new ConcurrentDictionary<uint, State>();
        static volatile bool loaded;
        static readonly object fileLock = new object();

        // ------------------------------------------------------------------ entry points
        /// <summary>QuestEngineCore.OnNpc: Chinju opens the board for mobile players.</summary>
        public static bool OnNpc(Client.GameClient c, uint npcid)
        {
            if (c == null || c.Player == null || !MobileEvents.IsMobile(c)) return false;
            if (npcid != ChinjuNpc)// talk / rescue / pick steps of the bounty quests (MobileBountyGoals)
            {
                try { return MobileBountyGoals.OnNpc(c, npcid); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return false; }
            }
            try { Open(c); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        /// <summary>MobileQuestGuide.OnLuaCall: TaskAssemblyGfit_* calls (true = handled).</summary>
        public static bool OnLuaCall(Client.GameClient c, string[] strings)
        {
            foreach (var s0 in strings)
            {
                if (string.IsNullOrEmpty(s0)) continue;
                int at = s0.IndexOf(Prefix, StringComparison.Ordinal);
                if (at < 0) continue;
                string call = s0.Substring(at + Prefix.Length);
                var p = call.Split(new[] { "</N>" }, StringSplitOptions.None);
                string func = p[0].Trim();
                uint id = p.Length > 1 ? U(p[1]) : 0, page = p.Length > 2 ? U(p[2]) : 0;
                try
                {
                    switch (func)
                    {
                        case "OpenDialogMain": Open(c); break;
                        case "AcceptTask": Accept(c, id, (int)page); break;
                        case "CompleteTask": Complete(c, id, (int)page); break;
                        case "DelTask": GiveUp(c, id, (int)page); break;
                        case "Refresh": Refresh(c); break;
                        case "Find": Find(c, id); break;
                        default: return false;
                    }
                }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ actions
        static void Open(Client.GameClient c)
        {
            MobileBountyGoals.EnsurePlaced();// the pick / rescue objects and the extra talk NPCs (once)
            var st = Get(c);
            if (st.Slots.Length == 0) { Push(c, "DlgGangBounty_Open|{['Code']=201}"); return; }// nothing for this level
            Push(c, "DlgGangBounty_Open|{['Code']=200,['Data']=" + Data(c, st, true) + "}");
        }

        static void Accept(Client.GameClient c, uint id, int page)
        {
            var st = Get(c);
            var s = SlotAt(st, id, page);
            const string f = "DlgGangBountyAceeptTaskBack|";
            if (s == null || s.State != 0) { Push(c, f + "{['Code']=202}"); return; }
            if (st.Slots.Any(x => x.State == 1 || x.State == 2)) { Push(c, f + "{['Code']=202}"); return; }
            if (st.Complete >= MaxPerDay) { Push(c, f + "{['Code']=203}"); return; }
            Database.QuestInfo.DBQuest dbq;
            if (!Database.QuestInfo.AllQuests.TryGetValue(id, out dbq) || !Fits(c, dbq)) { Push(c, f + "{['Code']=201}"); return; }
            var gui = c.Player.QuestGUI;
            if (gui.src.ContainsKey(id))
            {
                if (!gui.IsActiveQuest(id)) { Push(c, f + "{['Code']=202}"); return; }// finished today already
            }
            else
            {
                if (!gui.AllowAccept()) { c.SendSysMesage("[Bounty] You already have 5 quests in progress.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); Push(c, f + "{['Code']=202}"); return; }
                gui.Accept(dbq, 0);// 2126 action 1 -> the tracker shows it; the engine counts its kills / items
            }
            var goal = MobileBountyGoals.GoalOf(id);
            if (goal != null) MobileBountyGoals.OnAccept(c, goal);
            s.State = 1;
            Save();
            Log(c, "ACCEPT", id);
            Push(c, f + "{['Code']=200,['Data']={['Page']=" + Page(st, s) + "}}");
        }

        static void Complete(Client.GameClient c, uint id, int page)
        {
            var st = Get(c);
            var s = SlotAt(st, id, page);
            const string f = "DlgGangBountyCompleteTaskBack|";
            if (s == null || (s.State != 1 && s.State != 2)) { Push(c, f + "{['Code']=202}"); return; }
            var t = QuestEngineCore.Current;
            EngineQuest q;
            var gui = c.Player.QuestGUI;
            var goal = MobileBountyGoals.GoalOf(id);
            if (t == null || !t.Quests.TryGetValue(id, out q) || !gui.IsActiveQuest(id) || !(goal != null ? MobileBountyGoals.IsDone(c, goal) : QuestEngineCore.IsComplete(c, q, true)))
            { Push(c, f + "{['Code']=202}"); return; }
            var rw = RewardFor(s.Star, c.Player.Level);
            int slots = SlotsFor(rw);
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots)))
            { Push(c, f + "{['Code']=203,['Data']={['Space']=" + slots + "}}"); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!gui.FinishQuest(id)) { Push(c, f + "{['Code']=202}"); return; }// Accepted -> Finished once
                if (goal != null) MobileBountyGoals.HandIn(c, goal, stream);// picked items
                else
                    foreach (var o in q.Objectives)// hand in the collected items
                        if (o.Kind == ObjectiveKind.Collect && o.ItemId != 0)
                            try { c.Inventory.Remove(o.ItemId, o.Count, stream); } catch { }
                string given = Pay(c, rw, stream);
                st.Complete++;
                MobileDaily.Event(c, "bounty");// QahrMobile: mobile daily Quests of Bounty Hall
                s.State = 3;
                Log(c, "FINISH", id, "star " + s.Star + " gave [" + given + "]");
            }
            if (st.Slots.All(x => x.State == 3) && st.Complete < MaxPerDay) Roll(c, st);
            Save();
            Push(c, f + "{['Code']=200,['Data']={['Page']=" + Page(st, s) + ",['CompleteNum']=" + st.Complete + ",['TaskList']=" + TaskList(c, st) + "}}");
        }

        static void GiveUp(Client.GameClient c, uint id, int page)
        {
            var st = Get(c);
            var s = SlotAt(st, id, page);
            const string f = "DlgGangBountyGiveUpTaskBack|";
            if (s == null || (s.State != 1 && s.State != 2)) { Push(c, f + "{['Code']=201}"); return; }
            var gui = c.Player.QuestGUI;
            if (gui.IsActiveQuest(id))
            {
                gui.RemoveQuest(id);
                gui.SendSinglePacket(new Game.MsgServer.MsgQuestList.QuestListItem { UID = id, Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Available, Time = 0 }, Game.MsgServer.MsgQuestList.QuestMode.QuitQuest);// 2126 action 2: off the tracker
            }
            s.State = 0;
            Save();
            Log(c, "GIVEUP", id);
            Push(c, f + "{['Code']=200,['Data']={['Page']=" + Page(st, s) + "}}");
        }

        static void Refresh(Client.GameClient c)
        {
            var st = Get(c);
            const string f = "DlgGangBountyReloadBack|";
            if (st.Slots.Any(x => x.State == 1 || x.State == 2)) { Push(c, f + "{['Code']=202}"); return; }
            if (st.Free > 0) st.Free--;
            else
            {
                if (st.Reload >= MaxReload) { Push(c, f + "{['Code']=204}"); return; }
                if (c.Player.BoundConquerPoints < RefreshCost) { Push(c, f + "{['Code']=203}"); return; }
                c.Player.BoundConquerPoints -= RefreshCost;
            }
            st.Reload++;
            Roll(c, st);
            Save();
            Log(c, "REFRESH", 0, "reload " + st.Reload);
            Push(c, f + "{['Code']=200,['Data']={['FreeNum']=" + st.Free + ",['ReloadNum']=" + st.Reload + ",['CostNum']=" + RefreshCost + ",['TaskList']=" + TaskList(c, st) + "}}");
        }

        /// <summary>"Go": auto-walk to the nearest spawn of the quest's monster.</summary>
        static void Find(Client.GameClient c, uint id)
        {
            var st = Get(c);
            if (id == 0) { var a = st.Slots.FirstOrDefault(x => x.State == 1 || x.State == 2); if (a != null) id = a.Id; }
            var t = QuestEngineCore.Current;
            EngineQuest q;
            if (id == 0 || t == null || !t.Quests.TryGetValue(id, out q)) return;
            var goal = MobileBountyGoals.GoalOf(id);
            if (goal != null)
            {
                if (!MobileBountyGoals.Find(c, goal, q.Name))
                    c.SendSysMesage("[Bounty] " + q.Name + ": " + (MobileBountyGoals.IsDone(c, goal) ? "done - go back to Chinju (Twin City 441,290)." : MobileBountyGoals.Left(c, goal) + "."), Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                return;
            }
            Cand cd;
            string target = Candidates().TryGetValue(id, out cd) ? cd.Target : "";
            if (target.Length == 0 || target == "*")
            {
                c.SendSysMesage("[Bounty] " + q.Name + ": defeat monsters of your level anywhere.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                return;
            }
            if (!MobileQuestStory.KillHint(c, q.Name, target, ""))
                c.SendSysMesage("[Bounty] " + q.Name + ": look for " + target + ".", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
        }

        // ------------------------------------------------------------------ state
        static int Today { get { var d = DateTime.Now; return d.Year * 1000 + d.DayOfYear; } }

        static State Get(Client.GameClient c)
        {
            Load();
            var st = states.GetOrAdd(c.Player.UID, _ => new State());
            lock (st)
            {
                if (st.Day != Today)
                {
                    st.Day = Today; st.Complete = 0; st.Reload = 0; st.Free = FreeRefresh;
                    Roll(c, st);
                    Save();
                }
                // an accepted bounty that the daily reset (engine) or a give-up removed is offered again
                var gui = c.Player.QuestGUI;
                foreach (var s in st.Slots)
                {
                    if ((s.State == 1 || s.State == 2) && !gui.IsActiveQuest(s.Id)) s.State = gui.CheckQuest(s.Id, Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished) ? (byte)3 : (byte)0;
                    if (s.State == 1)
                    {
                        var t = QuestEngineCore.Current; EngineQuest q;
                        var goal = MobileBountyGoals.GoalOf(s.Id);
                        if (goal != null ? MobileBountyGoals.IsDone(c, goal) : (t != null && t.Quests.TryGetValue(s.Id, out q) && QuestEngineCore.IsComplete(c, q, true))) s.State = 2;
                    }
                }
                if (st.Slots.Length == 0 || (st.Slots.All(x => x.State == 3) && st.Complete < MaxPerDay) || (st.Slots.Length < Offers && st.Slots.All(x => x.State == 0))) Roll(c, st);
            }
            return st;
        }

        static void Roll(Client.GameClient c, State st)
        {
            var gui = c.Player.QuestGUI;
            var all = Candidates().Values.Where(x =>
            {
                Database.QuestInfo.DBQuest dbq;
                return !gui.src.ContainsKey(x.Id) && Database.QuestInfo.AllQuests.TryGetValue(x.Id, out dbq) && Fits(c, dbq);
            }).ToList();
            var keep = st.Slots.Where(x => x.State == 1 || x.State == 2).ToList();
            var list = new List<Slot>(keep);
            // one of the offers is a talk / rescue / pick bounty (MobileBountyGoals) when one fits; the others are kill / collect
            var gls = all.Where(x => MobileBountyGoals.GoalOf(x.Id) != null).ToList();
            all = all.Where(x => MobileBountyGoals.GoalOf(x.Id) == null).ToList();
            var grnd = new Random(unchecked((int)(c.Player.UID * 17 + Environment.TickCount)));
            if (gls.Count > 0 && list.Count < Offers && !list.Any(k => MobileBountyGoals.GoalOf(k.Id) != null))
            {
                var gt = gls.Select(y => y.Target).Distinct().ToList();// each kind (target) equally likely, then one of its quests
                var gtt = gt[grnd.Next(gt.Count)];
                var gq = gls.Where(y => y.Target == gtt).ToList();
                var x = gq[grnd.Next(gq.Count)];
                list.Add(new Slot { Id = x.Id, Star = x.Star, Type = x.Type, State = 0 });
                gls.RemoveAll(y => y.Target == x.Target);
            }
            if (all.Count > 0)
            {
                int band = all.Max(x => x.LvMin);
                var pick = all.Where(x => x.LvMin == band && !list.Any(k => k.Id == x.Id)).ToList();
                foreach (var b in all.Select(x => x.LvMin).Distinct().OrderByDescending(x => x))
                {
                    if (pick.Select(x => x.Target).Distinct().Count() >= Offers + 1) break;// enough different monsters
                    if (b == band) continue;
                    pick.AddRange(all.Where(x => x.LvMin == b && !list.Any(k => k.Id == x.Id)));
                }
                var rnd = new Random(unchecked((int)(c.Player.UID * 31 + Environment.TickCount)));
                while (list.Count < Offers && pick.Count > 0)
                {
                    var x = pick[rnd.Next(pick.Count)];
                    pick.RemoveAll(y => y.Id == x.Id || y.Target == x.Target);// three different monsters
                    list.Add(new Slot { Id = x.Id, Star = x.Star, Type = x.Type, State = 0 });
                }
            }
            while (list.Count < Offers && gls.Count > 0)// not enough kill / collect bounties for this level
            {
                var gt = gls.Select(y => y.Target).Distinct().ToList();// each kind (target) equally likely, then one of its quests
                var gtt = gt[grnd.Next(gt.Count)];
                var gq = gls.Where(y => y.Target == gtt).ToList();
                var x = gq[grnd.Next(gq.Count)];
                list.Add(new Slot { Id = x.Id, Star = x.Star, Type = x.Type, State = 0 });
                gls.RemoveAll(y => y.Target == x.Target);
            }
            st.Slots = list.ToArray();
        }

        static bool Fits(Client.GameClient c, Database.QuestInfo.DBQuest dbq)
        {
            if (dbq.Lv_min > 0 && c.Player.Level < dbq.Lv_min) return false;
            if (dbq.Lv_max > 0 && c.Player.Level > dbq.Lv_max) return false;
            if (dbq.Profession != null && !dbq.CanAcceptQuest(c.Player.Class)) return false;
            return true;
        }
        static Slot SlotAt(State st, uint id, int page)
        {
            if (page >= 1 && page <= st.Slots.Length && st.Slots[page - 1].Id == id) return st.Slots[page - 1];
            return st.Slots.FirstOrDefault(x => x.Id == id);
        }
        static int Page(State st, Slot s) { return Array.IndexOf(st.Slots, s) + 1; }

        // ------------------------------------------------------------------ the offer pool
        static volatile Dictionary<uint, Cand> cands;
        static object candsOf;
        static HashSet<uint> titled;

        static Dictionary<uint, Cand> Candidates()
        {
            var t = QuestEngineCore.Current;
            var d = cands;
            int gver = MobileBountyGoals.Version;
            var ck = candsOf as Tuple<QuestTable, int>;
            if (d != null && ck != null && ReferenceEquals(ck.Item1, t) && ck.Item2 == gver) return d;
            d = new Dictionary<uint, Cand>();
            var report = new StringBuilder();
            report.AppendLine("# QahrMobile: Chinju bounty pool (MobileQuestBounty.cs), " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            if (t != null)
            {
                var titles = Titles();
                var raw = new List<Tuple<Cand, string, uint>>();
                foreach (var q in t.Quests.Values)
                {
                    if (q.BeginNpc != ChinjuNpc || q.TypeId != 3) continue;
                    Database.QuestInfo.DBQuest dbq;
                    if (!Database.QuestInfo.AllQuests.TryGetValue(q.MissionId, out dbq)) continue;
                    var goal = MobileBountyGoals.GoalOf(q.MissionId);// talk / rescue / pick (MobileBountyGoals)
                    if (goal != null)
                    {
                        if (titles.Count > 0 && !titles.Contains(q.MissionId)) { report.AppendLine("skip\t" + q.MissionId + "\t" + q.Name + "\tno title in the client"); continue; }
                        var gc = new Cand { Id = q.MissionId, LvMin = dbq.Lv_min, LvMax = dbq.Lv_max, Type = (byte)goal.Kind, Target = "goal:" + string.Join("+", goal.Steps.Select(x => x.Target).Distinct()) };
                        raw.Add(Tuple.Create(gc, q.Name + "|" + gc.Target + "|" + gc.LvMin, goal.Total));
                        continue;
                    }
                    string why = null;
                    if (!q.Owned) why = "not engine-owned";
                    else if (!q.Workable) why = "not workable: " + string.Join("; ", q.Problems.Take(2));
                    else if (q.Objectives.Count == 0 || q.Objectives.Any(o => o.Kind != ObjectiveKind.Kill && o.Kind != ObjectiveKind.Collect)) why = "objective kind";
                    else if (titles.Count > 0 && !titles.Contains(q.MissionId)) why = "no title in the client";
                    if (why != null) { report.AppendLine("skip\t" + q.MissionId + "\t" + q.Name + "\t" + why); continue; }
                    var o0 = q.Objectives[0];
                    string target = o0.Kind == ObjectiveKind.Kill ? (o0.AnyMonster ? "*" : (o0.Name.Length > 0 ? o0.Name : o0.Monster)) : FirstMonster(q);
                    var cd = new Cand { Id = q.MissionId, LvMin = dbq.Lv_min, LvMax = dbq.Lv_max, Type = (byte)(q.Objectives.Any(o => o.Kind == ObjectiveKind.Collect) ? 3 : 2), Target = target };
                    raw.Add(Tuple.Create(cd, q.Name + "|" + target + "|" + cd.LvMin, o0.Count));
                }
                // star: rank of the goal count among the same quest kind (name + target + level band): 1, 2, 3, 5
                foreach (var g in raw.GroupBy(x => x.Item2))
                {
                    var counts = g.Select(x => x.Item3).Distinct().OrderBy(x => x).ToList();
                    foreach (var x in g)
                    {
                        int rank = counts.IndexOf(x.Item3);
                        int n = counts.Count;
                        int star = n <= 1 ? 1 : 1 + (int)Math.Round(3.0 * rank / (n - 1));// 1..4
                        x.Item1.Star = (byte)(star >= 4 ? 5 : star);
                        d[x.Item1.Id] = x.Item1;
                        report.AppendLine("pool\t" + x.Item1.Id + "\tlv " + x.Item1.LvMin + "\tstar " + x.Item1.Star + "\ttype " + x.Item1.Type + "\t" + x.Item2 + "\tx" + x.Item3);
                    }
                }
            }
            report.Insert(0, "# " + d.Count + " offers\n");
            try { File.WriteAllText(Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests", "bounty_pool.txt"), report.ToString(), Encoding.UTF8); } catch { }
            cands = d; candsOf = Tuple.Create(t, gver);
            return d;
        }
        static string FirstMonster(EngineQuest q)
        {
            foreach (var o in q.Objectives)
                foreach (var r in o.Drops)
                    if (!string.IsNullOrEmpty(r.Monster)) return r.Monster;
            return "";
        }
        /// <summary>Quest ids that have a title in the client's luaUiRes[1240].TaskInfo (Data\mobile_quests\bounty_titles.txt).</summary>
        static HashSet<uint> Titles()
        {
            var h = titled;
            if (h != null) return h;
            h = new HashSet<uint>();
            try
            {
                string f = Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests", "bounty_titles.txt");
                if (File.Exists(f))
                    foreach (var line in File.ReadAllLines(f)) { if (line.StartsWith("#")) continue; uint v = U(line); if (v != 0) h.Add(v); }
            }
            catch { }
            titled = h;
            return h;
        }

        // ------------------------------------------------------------------ rewards
        sealed class Reward { public uint ExpPct, Silver; public List<Tuple<uint, ushort>> Items = new List<Tuple<uint, ushort>>(); }
        static Dictionary<int, Reward> rewards; static DateTime rewardsTime;
        /// <summary>The reward of a star for this level: the line with the highest "lv" band not above the level.</summary>
        static Reward RewardFor(byte star, int level)
        {
            RewardFor(star);// (re-)reads the file
            var d = rewards;
            Reward best = null; int bestLv = -1;
            if (d != null)
                foreach (var kv in d)
                {
                    int st = kv.Key % 10, lv = kv.Key / 10;
                    if (st == star && lv <= level && lv > bestLv) { best = kv.Value; bestLv = lv; }
                }
            return best ?? RewardFor(star);
        }
        static Reward RewardFor(byte star)
        {
            string f = Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests", "bounty_rewards.txt");
            try
            {
                DateTime t = File.Exists(f) ? File.GetLastWriteTimeUtc(f) : DateTime.MinValue;
                if (rewards == null || t != rewardsTime)
                {
                    var d = new Dictionary<int, Reward>();
                    if (File.Exists(f))
                        foreach (var line in File.ReadAllLines(f))
                        {
                            if (line.Length == 0 || line[0] == '#') continue;
                            var p = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (p.Length < 3) continue;
                            var r = new Reward { ExpPct = U(p[1]), Silver = U(p[2]) };
                            if (p.Length > 3)
                                foreach (var tok in p[3].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string s = tok; ushort n = 1; int x = s.IndexOf('x');
                                    if (x > 0) { n = (ushort)Math.Max(1, U(s.Substring(x + 1))); s = s.Substring(0, x); }
                                    if (U(s) != 0) r.Items.Add(Tuple.Create(U(s), n));
                                }
                            int minLv = 0;// optional 5th column "lv<N>": this line is for players of level N and up (bounty_rewards.txt)
                            if (p.Length > 4 && p[4].StartsWith("lv", StringComparison.OrdinalIgnoreCase)) minLv = (int)U(p[4]);
                            else if (p.Length > 3 && p[3].StartsWith("lv", StringComparison.OrdinalIgnoreCase)) { minLv = (int)U(p[3]); r.Items.Clear(); }
                            d[(int)U(p[0]) + minLv * 10] = r;// key = level band * 10 + star
                        }
                    rewards = d; rewardsTime = t;
                }
            }
            catch { if (rewards == null) rewards = new Dictionary<int, Reward>(); }
            Reward rw;
            return rewards.TryGetValue(star, out rw) ? rw : (rewards.TryGetValue(1, out rw) ? rw : new Reward());
        }
        static int SlotsFor(Reward r)
        {
            int n = 0;
            foreach (var i in r.Items)
            {
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i.Item1, out db)) continue;
                n += db.StackSize > 1 ? (i.Item2 + db.StackSize - 1) / db.StackSize : i.Item2;
            }
            return n;
        }
        static string Pay(Client.GameClient c, Reward r, ServerSockets.Packet stream)
        {
            var parts = new List<string>();
            try
            {
                Dictionary<byte, Database.DBLevExp> t; Database.DBLevExp lv;
                if (r.ExpPct > 0 && c.Player.Level < 140 && Pool.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out t) && t.TryGetValue((byte)c.Player.Level, out lv) && lv != null)
                {
                    double exp = Math.Max(1.0, (double)lv.Experience * r.ExpPct / 100.0);
                    c.IncreaseExperience(stream, exp, Role.Flags.ExperienceEffect.angelwing, true);
                    parts.Add(((ulong)exp).ToString(CultureInfo.InvariantCulture) + " EXP");
                }
            }
            catch { }
            try
            {
                if (r.Silver > 0)
                {
                    c.Player.Money += r.Silver;
                    c.Player.SendUpdate(stream, c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                    parts.Add(r.Silver + " silver");
                }
            }
            catch { }
            foreach (var i in r.Items)
                try { if (Pool.ItemsBase.ContainsKey(i.Item1) && c.Inventory.AddItemWitchStack(i.Item1, 0, i.Item2, stream, true)) parts.Add(Pool.ItemsBase[i.Item1].Name + (i.Item2 > 1 ? " x" + i.Item2 : "")); } catch { }
            if (parts.Count > 0) c.SendSysMesage("[Bounty] You received: " + string.Join(", ", parts) + ".", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            return string.Join(", ", parts);
        }

        // ------------------------------------------------------------------ wire text
        static string Data(Client.GameClient c, State st, bool open)
        {
            return "{['CompleteNum']=" + st.Complete + ",['MaxNum']=" + MaxPerDay + ",['FreeNum']=" + st.Free + ",['ReloadNum']=" + st.Reload +
                   ",['CostNum']=" + RefreshCost + ",['TaskList']=" + TaskList(c, st) + "}";
        }
        static string TaskList(Client.GameClient c, State st)
        {
            var sb = new StringBuilder("{");
            for (int i = 0; i < st.Slots.Length; i++)
            {
                var s = st.Slots[i];
                var rw = RewardFor(s.Star, c.Player.Level);
                if (i > 0) sb.Append(',');
                sb.Append("{['TaskId']=").Append(s.Id).Append(",['Star']=").Append(s.Star).Append(",['Type']=").Append(s.Type)
                  .Append(",['Statue']=").Append(s.State).Append(",['FlagList']={1},['Item']={");
                for (int k = 0; k < rw.Items.Count && k < 3; k++)
                    sb.Append(k > 0 ? "," : "").Append("\"").Append(rw.Items[k].Item1).Append(" 0 ").Append(rw.Items[k].Item2).Append(" 3 0\"");
                sb.Append("}}");
            }
            return sb.Append("}").ToString();
        }
        static void Push(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, MsgId); }

        // ------------------------------------------------------------------ persistence
        static string StateFile() { return Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests", "bounty_state.txt"); }
        static void Load()
        {
            if (loaded) return;
            lock (fileLock)
            {
                if (loaded) return;
                try
                {
                    string f = StateFile();
                    if (File.Exists(f))
                        foreach (var line in File.ReadAllLines(f))
                        {
                            // uid|day|complete|reload|free|id:star:type:state,...
                            var p = line.Split('|');
                            if (p.Length < 6) continue;
                            var st = new State { Day = (int)U(p[1]), Complete = (int)U(p[2]), Reload = (int)U(p[3]), Free = (int)U(p[4]) };
                            st.Slots = p[5].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(':')).Where(x => x.Length >= 4)
                                .Select(x => new Slot { Id = U(x[0]), Star = (byte)U(x[1]), Type = (byte)U(x[2]), State = (byte)U(x[3]) }).ToArray();
                            states[U(p[0])] = st;
                        }
                }
                catch (Exception e) { MyConsole.WriteLine("[Mobile] bounty state load: " + e.Message, ConsoleColor.Red); }
                loaded = true;
            }
        }
        static void Save()
        {
            lock (fileLock)
            {
                try
                {
                    var sb = new StringBuilder();
                    foreach (var kv in states)
                        sb.Append(kv.Key).Append('|').Append(kv.Value.Day).Append('|').Append(kv.Value.Complete).Append('|').Append(kv.Value.Reload).Append('|').Append(kv.Value.Free).Append('|')
                          .Append(string.Join(",", kv.Value.Slots.Select(s => s.Id + ":" + s.Star + ":" + s.Type + ":" + s.State))).Append('\n');
                    string f = StateFile(), tmp = f + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), Encoding.UTF8);
                    if (File.Exists(f)) File.Replace(tmp, f, null); else File.Move(tmp, f);
                }
                catch (Exception e) { MyConsole.WriteLine("[Mobile] bounty state save: " + e.Message, ConsoleColor.Red); }
            }
        }

        static uint U(string s) { var m = Regex.Match(s ?? "", @"\d+"); uint v; return m.Success && uint.TryParse(m.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static void Log(Client.GameClient c, string what, uint id, string detail = "")
        {
            try
            {
                File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | BOUNTY " + what + " | " + id + " | " + c.Player.UID + " " + c.Player.Name + " lv" + c.Player.Level + " | " + detail + Environment.NewLine);
            }
            catch { }
        }
    }
}
