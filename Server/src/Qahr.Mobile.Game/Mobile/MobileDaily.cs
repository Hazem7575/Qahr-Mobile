// ============================================================================
//  QahrMobile: the mobile's daily activity screen (CDlgActivityTask / CDlgActivityOwer, CActivityTaskData).
//
//  The server's own activity tasks (Data\ActivityTask.ini ids 1..39, Role.Instance.Activeness) are unknown to the
//  mobile client, which only displays the ids of its activitytask.ini (103..202) and claims the chests of its
//  activity_reward_type.dat (ids 10..24). For mobile clients this class replaces the Activeness packets:
//
//  CMsgActivityTask 10316 (server 2250): mode u8 4 (0 reset = the client fills its own list, 1 del, 2 add/update,
//      3 update), count u8 5, units of 6 from 6: id u32, completed u8, progress u8 (CActivityTaskData::Add/Del/
//      UpdateActivityUserInfo). We send 1 (del: tasks we cannot count) then 2 (ours).
//  CMsgStatisticDaily 10174 (server 2198): count u8 4, units of 12 from 5: type u32, sub u32, value u32.
//      type 2 sub 1 = today's activity points (SetTodayActiveValue), type 2 sub 2 = claimed chests as a bitmask,
//      bit index-1 (SetUserRewardGrade -> UpdateUserPrize: tst 1 << (index-1)).
//  CMsgActivityTaskReward 10105 (server 2009): id u8 4. Client -> server: the chest id (10 + 5 * grade + index - 1,
//      grade = the hero's reborn count, CPlayer 0xb78). Server -> client: the same id = PrizeGetSuccess.
//
//  Data (re-read when saved): Data\mobile_daily\tasks.txt, rewards.txt. State: Data\mobile_daily\state.txt
//  (one line per player, rewritten on change). Reset: the first use on a new day and Activeness.ResetDay.
//  Hooks (one line each): Activeness (IncreaseTask / CheckTasks / UpdateActivityPoints / UpdateTasksList /
//  UpdateSingleTask / UpdateClaimRewards / ResetDay), MsgActivityClaims, MsgArena (loser), TeamArenaCore,
//  MsgInnerPower, MobileQuestBounty, Quests.FinishQuest, ReceiveAttack.Monster. PC clients are unchanged.
//  Docs: D:\conquer\conquer-mobile\docs\DAILY_TASKS.md
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Phone
{
    public static class MobileDaily
    {
        sealed class TaskDef { public uint Id; public byte Progress, Points; public ushort MinLevel; public byte MinReborn; public string Event = ""; public HashSet<uint> Maps; public uint Quest; }
        sealed class RewardDef { public uint Id; public byte Grade, Index; public uint Points; public List<Tuple<uint, ushort>> Items = new List<Tuple<uint, ushort>>(); public int Cps; }
        sealed class Prog { public byte Progress, Done; }
        sealed class State { public int Day; public uint Points; public byte Claimed; public Dictionary<uint, Prog> Tasks = new Dictionary<uint, Prog>(); }

        static List<TaskDef> tasks = new List<TaskDef>();
        static List<uint> clientIds = new List<uint>();
        static Dictionary<uint, RewardDef> rewards = new Dictionary<uint, RewardDef>();
        static DateTime tasksTime, rewardsTime, nextCheck;
        static readonly object loadLock = new object(), fileLock = new object();
        static readonly ConcurrentDictionary<uint, State> states = new ConcurrentDictionary<uint, State>();
        static volatile bool stateLoaded;

        static string Dir { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_daily"); } }
        static int Today { get { var n = DateTime.Now; return n.Year * 10000 + n.Month * 100 + n.Day; } }

        public static bool Is(Client.GameClient c) { return c != null && c.Player != null && MobileEvents.IsMobile(c); }

        // ------------------------------------------------------------------ hooks
        /// <summary>Activeness.IncreaseTask(serverTaskId): true = handled (mobile client).</summary>
        public static bool OnServerTask(Client.GameClient c, uint serverTask)
        {
            if (!Is(c)) return false;
            string ev = null;
            switch (serverTask)
            {
                case 1: ev = "login"; break;
                case 2: ev = "vip"; break;
                case 3: ev = "online"; break;
                case 4: ev = "lottery"; break;
                case 6: ev = "flowers"; break;
                case 7: ev = "arena"; break;// winner (the loser comes from MsgArena)
                case 11: ev = "enlight"; break;
                case 37: ev = "chi"; break;
                case 38: ev = "jianghu"; break;
            }
            if (ev != null) Event(c, ev);
            return true;// the other ids are the same events for other level bands: ignored
        }

        /// <summary>Activeness list / points / claim packets: true = a mobile client, the full mobile view was sent instead.</summary>
        public static bool OnSendAll(Client.GameClient c)
        {
            if (!Is(c)) return false;
            try { SendAll(c); } catch (Exception e) { Log(e); }
            return true;
        }

        /// <summary>Activeness packets that have no mobile equivalent: true = mobile, skip.</summary>
        public static bool Skip(Client.GameClient c) { return Is(c); }

        /// <summary>Activeness.ResetDay.</summary>
        public static void OnResetDay(Client.GameClient c)
        {
            if (c == null || c.Player == null) return;
            State st;
            if (states.TryGetValue(c.Player.UID, out st))
                lock (st) { st.Day = Today; st.Points = 0; st.Claimed = 0; st.Tasks.Clear(); }
        }

        public static void EventUid(uint uid, string ev)
        {
            Client.GameClient c;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c)) Event(c, ev);
        }

        public static void OnQuestFinished(Role.Player p, uint questId)
        {
            if (p == null) return;
            Event(p.Owner, "quest:" + questId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>ReceiveAttack.Monster, just before monster.Dead.</summary>
        public static void OnMonsterKill(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            try
            {
                if (monster == null || !monster.Alive) return;
                if (killer != null) Event(killer, "kill", monster.Map);
                if (monster.Boss == 1 && monster.OwnerUID == 0 && monster.Hunters != null)
                {
                    uint[] uids;
                    try { uids = monster.Hunters.Keys.ToArray(); } catch { uids = new uint[0]; }
                    foreach (var u in uids) EventUid(u, "worldboss");
                }
            }
            catch (Exception e) { Log(e); }
        }

        public static void Event(Client.GameClient c, string ev) { Event(c, ev, 0); }

        static void Event(Client.GameClient c, string ev, uint map)
        {
            try
            {
                if (!Is(c)) return;
                Load();
                var st = Get(c.Player.UID);
                var changed = new List<uint>();
                bool pts = false;
                lock (st)
                {
                    foreach (var t in tasks)
                    {
                        bool match = ev == "kill" ? (t.Maps != null && t.Maps.Contains(map)) : t.Event == ev;
                        if (!match || !Open(c, t)) continue;
                        Prog p;
                        if (!st.Tasks.TryGetValue(t.Id, out p)) st.Tasks[t.Id] = p = new Prog();
                        if (p.Done != 0) continue;
                        if (p.Progress < t.Progress) p.Progress++;
                        if (p.Progress >= t.Progress) { p.Done = 1; st.Points += t.Points; pts = true; }
                        changed.Add(t.Id);
                    }
                }
                if (changed.Count == 0) return;
                Save();
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    lock (st) c.Send(TaskPacket(stream, 2, changed.Select(id => Tuple.Create(id, st.Tasks[id])).ToList()));
                    if (pts) c.Send(PointsPacket(stream, st));
                }
            }
            catch (Exception e) { Log(e); }
        }

        /// <summary>MsgActivityClaims: true = handled (mobile client).</summary>
        public static bool OnClaim(Client.GameClient c, uint id, ServerSockets.Packet stream)
        {
            if (!Is(c)) return false;
            try
            {
                Load();
                RewardDef r;
                var st = Get(c.Player.UID);
                int grade = Math.Min(2, (int)c.Player.Reborn);
                if (id >= 1 && id <= 8)// the client sends the chest index 1..5 (seen live), the table row ids 10..24 are accepted too
                {
                    var byIndex = rewards.Values.FirstOrDefault(x => x.Grade == grade && x.Index == id);
                    if (byIndex != null) id = byIndex.Id;
                }
                if (!rewards.TryGetValue(id, out r) || r.Grade != grade || r.Index < 1 || r.Index > 8)
                { Msg(c, "This chest is not for your rebirth grade."); return true; }
                byte bit = (byte)(1 << (r.Index - 1));
                lock (st)
                {
                    if ((st.Claimed & bit) != 0) { c.Send(PointsPacket(stream, st)); return true; }
                    if (st.Points < r.Points) { Msg(c, "You need " + r.Points + " activity points for this chest (you have " + st.Points + ")."); return true; }
                    int slots = SlotsFor(r);
                    if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, slots))) { Msg(c, "Please make " + slots + " more space in your inventory."); return true; }
                    st.Claimed |= bit;// mark first: no second claim while paying
                }
                Save();
                var given = new List<string>();
                foreach (var i in r.Items)
                    try { if (Pool.ItemsBase.ContainsKey(i.Item1) && c.Inventory.AddItemWitchStack(i.Item1, 0, i.Item2, stream, true)) given.Add(Pool.ItemsBase[i.Item1].Name + (i.Item2 > 1 ? " x" + i.Item2 : "")); } catch (Exception e) { Log(e); }
                if (r.Cps > 0) { c.Player.BoundConquerPoints += r.Cps; given.Add(r.Cps + " CPs (B)"); }
                if (given.Count > 0) Msg(c, "Activity chest: you received " + string.Join(", ", given) + ".");
                c.Send(ClaimPacket(stream, id));
                c.Send(PointsPacket(stream, st));
                LogLine(c, "CLAIM " + id + " points " + st.Points + " gave [" + string.Join(", ", given) + "]");
            }
            catch (Exception e) { Log(e); }
            return true;
        }

        /// <summary>GM chat command (MsgMessage.ChatCommands): "@mdaily" (resend), "@mdaily event NAME [times]",
        /// "@mdaily points N", "@mdaily reset". Mobile GM characters only.</summary>
        public static bool ChatCommand(Client.GameClient c, string message)
        {
            if (string.IsNullOrEmpty(message) || !message.StartsWith("@mdaily", StringComparison.OrdinalIgnoreCase)) return false;
            if (!Is(c) || !(c.ProjectManager || c.IsStaffAccount)) return false;
            try
            {
                var p = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string sub = p.Length > 1 ? p[1].ToLowerInvariant() : "";
                var st = Get(c.Player.UID);
                if (sub == "event" && p.Length > 2)
                {
                    int n = p.Length > 3 ? (int)Math.Max(1u, U(p[3])) : 1;
                    for (int i = 0; i < n; i++)
                        if (p[2].StartsWith("kill:")) Event(c, "kill", U(p[2].Substring(5))); else Event(c, p[2].ToLowerInvariant());
                }
                else if (sub == "points" && p.Length > 2) { lock (st) st.Points = U(p[2]); Save(); }
                else if (sub == "reset") { OnResetDay(c); Save(); }
                SendAll(c);
                lock (st) Msg(c, "[mdaily] points " + st.Points + " chests 0x" + st.Claimed.ToString("x") + " tasks " + string.Join(" ", st.Tasks.Select(t => t.Key + ":" + t.Value.Progress + (t.Value.Done != 0 ? "*" : ""))));
            }
            catch (Exception e) { Log(e); }
            return true;
        }

        // ------------------------------------------------------------------ packets
        static void SendAll(Client.GameClient c)
        {
            Load();
            var st = Get(c.Player.UID);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                List<Tuple<uint, Prog>> list;
                List<uint> hide;
                lock (st)
                {
                    var open = tasks.Where(t => Open(c, t)).ToList();
                    foreach (var t in open) if (!st.Tasks.ContainsKey(t.Id)) st.Tasks[t.Id] = new Prog();
                    list = open.Select(t => Tuple.Create(t.Id, st.Tasks[t.Id])).ToList();
                    var served = new HashSet<uint>(open.Select(t => t.Id));
                    hide = clientIds.Where(i => !served.Contains(i)).ToList();
                }
                if (hide.Count > 0) c.Send(TaskPacket(stream, 1, hide.Select(i => Tuple.Create(i, new Prog())).ToList()));
                c.Send(TaskPacket(stream, 2, list));
                c.Send(PointsPacket(stream, st));
            }
        }

        static ServerSockets.Packet TaskPacket(ServerSockets.Packet stream, byte mode, List<Tuple<uint, Prog>> list)
        {
            stream.InitWriter();
            stream.Write(mode);
            int n = Math.Min(list.Count, 100);
            stream.Write((byte)n);
            for (int i = 0; i < n; i++)
            {
                stream.Write(list[i].Item1);
                stream.Write(list[i].Item2.Done);
                stream.Write(list[i].Item2.Progress);
            }
            stream.Finalize(Game.GamePackets.MsgActivityTask);
            return stream;
        }

        static ServerSockets.Packet PointsPacket(ServerSockets.Packet stream, State st)
        {
            stream.InitWriter();
            stream.Write((byte)2);
            stream.Write((uint)2); stream.Write((uint)1); stream.Write(st.Points);
            stream.Write((uint)2); stream.Write((uint)2); stream.Write((uint)st.Claimed);
            stream.Finalize(Game.GamePackets.MsgStatisticDaily);
            return stream;
        }

        static ServerSockets.Packet ClaimPacket(ServerSockets.Packet stream, uint id)
        {
            stream.InitWriter();
            stream.Write((byte)id);
            stream.Finalize(Game.GamePackets.MsgActivityTaskReward);
            return stream;
        }

        // ------------------------------------------------------------------ data
        static bool Open(Client.GameClient c, TaskDef t)
        {
            return c.Player.Reborn >= t.MinReborn && (c.Player.Level >= t.MinLevel || c.Player.Reborn > t.MinReborn);
        }

        static int SlotsFor(RewardDef r)
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

        static void Load()
        {
            if (DateTime.UtcNow < nextCheck && tasks.Count > 0) return;
            lock (loadLock)
            {
                nextCheck = DateTime.UtcNow.AddSeconds(5);
                try
                {
                    string f = Path.Combine(Dir, "tasks.txt");
                    DateTime t = File.Exists(f) ? File.GetLastWriteTimeUtc(f) : DateTime.MinValue;
                    if (t != tasksTime)
                    {
                        var l = new List<TaskDef>(); var ids = new List<uint>();
                        if (File.Exists(f))
                            foreach (var raw in File.ReadAllLines(f))
                            {
                                var line = Strip(raw); if (line.Length == 0) continue;
                                var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p[0] == "client") { ids.AddRange(p.Skip(1).Select(U)); continue; }
                                if (p.Length < 6) continue;
                                var d = new TaskDef { Id = U(p[0]), Progress = (byte)Math.Max(1u, U(p[1])), Points = (byte)U(p[2]), MinLevel = (ushort)U(p[3]), MinReborn = (byte)U(p[4]), Event = p[5].ToLowerInvariant() };
                                if (d.Event.StartsWith("kill:")) d.Maps = new HashSet<uint>(d.Event.Substring(5).Split(',').Select(U));
                                l.Add(d);
                            }
                        tasks = l; clientIds = ids; tasksTime = t;
                    }
                    f = Path.Combine(Dir, "rewards.txt");
                    t = File.Exists(f) ? File.GetLastWriteTimeUtc(f) : DateTime.MinValue;
                    if (t != rewardsTime)
                    {
                        var d = new Dictionary<uint, RewardDef>();
                        if (File.Exists(f))
                            foreach (var raw in File.ReadAllLines(f))
                            {
                                var line = Strip(raw); if (line.Length == 0) continue;
                                var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p.Length < 5) continue;
                                var r = new RewardDef { Id = U(p[0]), Grade = (byte)U(p[1]), Index = (byte)U(p[2]), Points = U(p[3]) };
                                foreach (var it in p[4].Split(';'))
                                {
                                    var s = it.Trim().ToLowerInvariant();
                                    if (s.Length == 0 || s == "-") continue;
                                    if (s.StartsWith("cps:")) { r.Cps += (int)U(s.Substring(4)); continue; }
                                    var q = s.Split('x');
                                    r.Items.Add(Tuple.Create(U(q[0]), (ushort)Math.Max(1u, q.Length > 1 ? U(q[1]) : 1)));
                                }
                                d[r.Id] = r;
                            }
                        rewards = d; rewardsTime = t;
                    }
                }
                catch (Exception e) { Log(e); }
            }
        }

        static string Strip(string s) { int i = s.IndexOf('#'); return (i >= 0 ? s.Substring(0, i) : s).Trim(); }
        static uint U(string s) { uint v; return uint.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        // ------------------------------------------------------------------ state
        static State Get(uint uid)
        {
            LoadState();
            var st = states.GetOrAdd(uid, _ => new State { Day = Today });
            lock (st)
                if (st.Day != Today) { st.Day = Today; st.Points = 0; st.Claimed = 0; st.Tasks.Clear(); }
            return st;
        }

        static void LoadState()
        {
            if (stateLoaded) return;
            lock (fileLock)
            {
                if (stateLoaded) return;
                try
                {
                    string f = Path.Combine(Dir, "state.txt");
                    if (File.Exists(f))
                        foreach (var line in File.ReadAllLines(f))
                        {
                            var p = line.Trim().TrimStart((char)0xFEFF).Split(' ');
                            if (p.Length < 4) continue;
                            var st = new State { Day = (int)U(p[1]), Points = U(p[2]), Claimed = (byte)U(p[3]) };
                            if (p.Length > 4)
                                foreach (var e in p[4].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var q = e.Split(':');
                                    if (q.Length == 3) st.Tasks[U(q[0])] = new Prog { Progress = (byte)U(q[1]), Done = (byte)U(q[2]) };
                                }
                            states[U(p[0])] = st;
                        }
                }
                catch (Exception e) { Log(e); }
                stateLoaded = true;
            }
        }

        static void Save()
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var kv in states)
                {
                    var st = kv.Value;
                    lock (st)
                    {
                        if (st.Day != Today) continue;
                        sb.Append(kv.Key).Append(' ').Append(st.Day).Append(' ').Append(st.Points).Append(' ').Append(st.Claimed).Append(' ')
                          .Append(string.Join(",", st.Tasks.Select(t => t.Key + ":" + t.Value.Progress + ":" + t.Value.Done))).Append('\n');
                    }
                }
                lock (fileLock)
                {
                    Directory.CreateDirectory(Dir);
                    File.WriteAllText(Path.Combine(Dir, "state.txt"), sb.ToString(), new UTF8Encoding(false));
                }
            }
            catch (Exception e) { Log(e); }
        }

        // ------------------------------------------------------------------ misc
        static void Msg(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void LogLine(Client.GameClient c, string text)
        {
            try
            {
                lock (fileLock)
                    File.AppendAllText(Path.Combine(Dir, "log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + c.Player.UID + " " + c.Player.Name + " " + text + "\n");
            }
            catch { }
        }

        static void Log(Exception e) { try { MyConsole.SaveException(e); } catch { } }
    }
}
