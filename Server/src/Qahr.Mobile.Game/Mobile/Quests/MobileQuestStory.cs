// ============================================================================
//  QahrMobile: rewards, start rule, monster hints and the login quest list of the mobile main story
//  (MobileQuestGuide.cs drives the chain; this class only adds what the chain needs around it).
//  Notes: D:\conquer\conquer-mobile\docs\quests\quest-story.md
//
//  Rewards = columns 14-16 of Data\mobile_quests\main_story.txt (the mobile's own Prize= lines are empty):
//      expPct   per cent of the EXP the player needs for its CURRENT level (so the reward scales with the level)
//      silver   silver
//      items    "id[xN][@class];..." bound items; @trojan / @warrior / @archer / @taoist / @ninja / @monk / @pirate /
//               @lee / @windwalker limit an item to that class line ("-" or empty = none)
//  Re-read when the file changes. Paid once per finish (MobileQuestGuide.TryComplete, after FinishQuest succeeded).
//
//  Start rule (task 13507 at login): a character with no main-story quest at all that is level 70+ or reborn is past
//  the beginner stage and does not get "World of Conquer" pushed into its tracker. It still can take it by itself
//  (quest panel near Eloise, or the quest's goal link), and from then on the chain continues normally.
//
//  Kill steps: the quest engine counts the kills (Kill objective parsed from "[lua Pheasant,...][chk_quest id,1,1,3]")
//  and pushes the counters (2339). A goal click with kills missing auto-walks to the nearest spawn of that monster
//  (MonsterSpawns point files / spawn folders), not to the quest NPC.
//
//  Check report: Data\mobile_quests\main_story_check.txt (written when main_story.txt changes): every step whose
//  target is a monster, the engine objectives / problems, and the nearest spawn to the quest NPC.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    public static class MobileQuestStory
    {
        public const byte StartMaxLevel = 70;// level >= this (or reborn) with no main-story quest = past the beginner stage

        public sealed class Reward
        {
            public uint ExpPct, Silver;
            public List<Tuple<uint, ushort, string>> Items = new List<Tuple<uint, ushort, string>>();// id, count, class ("" = all)
        }

        /// <summary>The reward columns of main_story.txt (MainStory.cs).</summary>
        public static Dictionary<uint, Reward> Rewards
        {
            get { return MainStory.Current.Rewards; }
        }
        static uint U(string s) { uint v; return uint.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        static bool ClassFits(Client.GameClient c, string cls)
        {
            if (string.IsNullOrEmpty(cls)) return true;
            uint j = c.Player.Class;
            switch (cls)
            {
                case "trojan": return Database.AtributesStatus.IsTrojan(j);
                case "warrior": return Database.AtributesStatus.IsWarrior(j);
                case "archer": return Database.AtributesStatus.IsArcher(j);
                case "taoist": return Database.AtributesStatus.IsTaoist(j);
                case "ninja": return Database.AtributesStatus.IsNinja(j);
                case "monk": return Database.AtributesStatus.IsMonk(j);
                case "pirate": return Database.AtributesStatus.IsPirate(j);
                case "lee": return Database.AtributesStatus.IsLee(j);
                case "windwalker": return Database.AtributesStatus.IsWindWalker(j);
                default: return false;
            }
        }
        static List<Tuple<uint, ushort, string>> ItemsFor(Client.GameClient c, Reward r)
        {
            return r.Items.Where(i => ClassFits(c, i.Item3) && Pool.ItemsBase != null && Pool.ItemsBase.ContainsKey(i.Item1)).ToList();
        }
        static int SlotsFor(List<Tuple<uint, ushort, string>> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                n += Phone.Rewards.Slots(i.Item1, i.Item2);
            }
            return n;
        }

        /// <summary>Before the finish: bag room for the step's items (false = the player was told).</summary>
        public static bool HasSpace(Client.GameClient c, uint id)
        {
            try
            {
                Reward r;
                if (!Rewards.TryGetValue(id, out r)) return true;
                int slots = SlotsFor(ItemsFor(c, r));
                if (slots == 0 || Phone.Rewards.HasRoom(c, slots)) return true;
                c.SendSysMesage("[Quest] Please make " + slots + " free space(s) in your inventory first.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                return false;
            }
            catch (Exception err) { MyConsole.SaveException(err); return true; }
        }

        /// <summary>After FinishQuest succeeded: EXP (per cent of the current level), silver, bound items.</summary>
        public static string Pay(Client.GameClient c, uint id, ServerSockets.Packet stream)
        {
            var parts = new List<string>();
            try
            {
                Reward r;
                if (!Rewards.TryGetValue(id, out r)) return "";
                if (r.ExpPct > 0 && c.Player.Level < 140)
                {
                    try
                    {
                        Dictionary<byte, Database.DBLevExp> t;
                        Database.DBLevExp lv;
                        if (Pool.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out t) && t.TryGetValue((byte)c.Player.Level, out lv) && lv != null)
                        {
                            double exp = Math.Max(1.0, (double)lv.Experience * r.ExpPct / 100.0);
                            c.IncreaseExperience(stream, exp, Role.Flags.ExperienceEffect.angelwing, true);
                            parts.Add(((ulong)exp).ToString(CultureInfo.InvariantCulture) + " EXP");
                        }
                    }
                    catch (Exception e) { Log(c, id, "ERROR exp: " + e.Message); }
                }
                if (r.Silver > 0)
                {
                    try
                    {
                        c.Player.Money += r.Silver;
                        c.Player.SendUpdate(stream, c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                        parts.Add(r.Silver + " silver");
                    }
                    catch (Exception e) { Log(c, id, "ERROR silver: " + e.Message); }
                }
                foreach (var i in ItemsFor(c, r))
                {
                    try
                    {
                        if (c.Inventory.AddItemWitchStack(i.Item1, 0, i.Item2, stream, true))
                        {
                            Database.ItemType.DBItem db;
                            string name = Pool.ItemsBase.TryGetValue(i.Item1, out db) && db != null && !string.IsNullOrEmpty(db.Name) ? db.Name : i.Item1.ToString();
                            parts.Add(name + (i.Item2 > 1 ? " x" + i.Item2 : ""));
                        }
                    }
                    catch (Exception e) { Log(c, id, "ERROR item " + i.Item1 + ": " + e.Message); }
                }
                if (parts.Count > 0)
                    c.SendSysMesage("[Quest] You received: " + string.Join(", ", parts) + ".", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            string given = string.Join(", ", parts);
            Log(c, id, "reward [" + given + "]");
            return given;
        }

        // ------------------------------------------------------------------ start rule
        /// <summary>True = do not push 13507 at login: no main-story quest yet and level 70+ or reborn.</summary>
        public static bool SkipAutoStart(Client.GameClient c)
        {
            try
            {
                if (c.Player.Level < StartMaxLevel && c.Player.Reborn == 0) return false;
                var gui = c.Player.QuestGUI;
                foreach (uint id in MobileQuestGuide.Table.Keys)
                    if (gui.src.ContainsKey(id)) return false;// already in the story: keep it going
                return true;
            }
            catch (Exception err) { MyConsole.SaveException(err); return false; }
        }

        // ------------------------------------------------------------------ login quest list
        /// <summary>Mobile: finished quests as ONE list packet (2126 action 3, status 1) instead of accept + finish per
        /// quest (action 4 re-runs CNewGuideMgr::ProcesNewGuideTask on the mobile).</summary>
        public static void SendFinishedList(Client.GameClient c, uint[] ids)
        {
            if (c == null || c.Player == null || ids == null || ids.Length == 0) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.QuestListCreate(Game.MsgServer.MsgQuestList.QuestMode.Review, (ushort)ids.Length);
                foreach (uint id in ids)
                    stream.AddItemQuestList(new Game.MsgServer.MsgQuestList.QuestListItem { UID = id, Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished, Time = 0 });
                c.Send(stream.QuestListFinalize());
            }
        }

        // ------------------------------------------------------------------ monster spots
        sealed class Spot { public uint Map; public ushort X, Y; }
        static volatile Dictionary<string, List<Spot>> spots;
        static readonly object slock = new object();

        static Dictionary<string, List<Spot>> Spots
        {
            get
            {
                if (spots != null) return spots;
                lock (slock)
                {
                    if (spots != null) return spots;
                    var d = new Dictionary<string, List<Spot>>();
                    string root = Path.Combine(Program.ServerConfig.DbLocation ?? "", "MonsterSpawns");
                    try
                    {
                        // point files: "uid,look,name,x,y,map"
                        foreach (var kv in QuestEngine.QuestGrammar.PointFileMaps)
                        {
                            string f = Path.Combine(root, kv.Value);
                            if (!File.Exists(f)) continue;
                            foreach (var line in File.ReadLines(f))
                            {
                                var p = line.Split(',');
                                if (p.Length < 6) continue;
                                uint map = U(p[5]); if (map == 0) map = kv.Key;
                                Add(d, p[2], map, U(p[3]), U(p[4]));
                            }
                        }
                        // spawn folders (maps without a point file): MonsterSpawns\<map>\<name>\*.ini npctype + bound
                        if (Directory.Exists(root))
                            foreach (var mapDir in Directory.GetDirectories(root))
                            {
                                uint map = U(Path.GetFileName(mapDir));
                                if (map == 0 || QuestEngine.QuestGrammar.PointFileMaps.ContainsKey(map)) continue;
                                foreach (var f in Directory.GetFiles(mapDir, "*.ini", SearchOption.AllDirectories))
                                {
                                    uint type = 0, bx = 0, by = 0, cx = 0, cy = 0;
                                    foreach (var line in File.ReadLines(f))
                                    {
                                        int eq = line.IndexOf('='); if (eq < 0) continue;
                                        string k = line.Substring(0, eq).Trim(); uint v = U(line.Substring(eq + 1));
                                        if (k == "npctype") type = v; else if (k == "bound_x") bx = v; else if (k == "bound_y") by = v;
                                        else if (k == "bound_cx") cx = v; else if (k == "bound_cy") cy = v;
                                    }
                                    Game.MsgMonster.MonsterFamily fam;
                                    if (type == 0 || Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(type, out fam) || fam == null) continue;
                                    Add(d, fam.Name, map, bx + cx / 2, by + cy / 2);
                                }
                            }
                        // myMonsters.txt (all maps, loaded by Server.cs): "id map x y xPlus yPlus diameter respawn amount type"
                        string my = Path.Combine(root, "myMonsters.txt");
                        if (File.Exists(my) && Pool.MonsterFamilies != null)
                            foreach (var line in File.ReadLines(my))
                            {
                                var a = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                if (a.Length < 10) continue;
                                Game.MsgMonster.MonsterFamily fam;
                                if (!Pool.MonsterFamilies.TryGetValue(U(a[9]), out fam) || fam == null) continue;
                                Add(d, fam.Name, U(a[1]), U(a[2]) + U(a[4]) / 2, U(a[3]) + U(a[5]) / 2);
                            }
                    }
                    catch (Exception e) { MyConsole.WriteLine("[Mobile] monster spots: " + e.Message, ConsoleColor.Red); }
                    spots = d;
                    return d;
                }
            }
        }
        static void Add(Dictionary<string, List<Spot>> d, string name, uint map, uint x, uint y)
        {
            string n = QuestEngine.QuestGrammar.Norm(name);
            if (n.Length == 0 || map == 0) return;
            List<Spot> l;
            if (!d.TryGetValue(n, out l)) d[n] = l = new List<Spot>();
            l.Add(new Spot { Map = map, X = (ushort)x, Y = (ushort)y });
        }

        /// <summary>Nearest spawn of a monster: on the given map first (closest to x,y), else the first map that has it.</summary>
        public static bool NearestSpot(string monster, uint map, int x, int y, out uint sm, out ushort sx, out ushort sy, out int dist)
        {
            sm = 0; sx = sy = 0; dist = int.MaxValue;
            List<Spot> l;
            string n = QuestEngine.QuestGrammar.Norm(monster);
            if (!Spots.TryGetValue(n, out l) && !Spots.TryGetValue(QuestEngine.QuestGrammar.Singular(n), out l)) return false;
            Spot best = null;
            foreach (var s in l)
            {
                if (s.Map != map) continue;
                int d = Math.Max(Math.Abs(s.X - x), Math.Abs(s.Y - y));
                if (d < dist) { dist = d; best = s; }
            }
            if (best == null) { best = l[0]; dist = int.MaxValue; }
            sm = best.Map; sx = best.X; sy = best.Y;
            return true;
        }

        /// <summary>Goal click with kills missing: hint + auto-walk to the nearest spawn. False = no spawn known.</summary>
        public static bool KillHint(Client.GameClient c, string questName, string monster, string left)
        {
            try
            {
                uint m; ushort x, y; int d;
                if (!NearestSpot(monster, c.Player.Map, c.Player.X, c.Player.Y, out m, out x, out y, out d)) return false;
                string where = (m != c.Player.Map ? "map " + m + " " : "") + x + "," + y;
                c.SendSysMesage("[Quest] " + questName + ": " + (string.IsNullOrEmpty(left) ? "" : left + " ") + monster + " can be found at " + where + ".", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                if (d > 6) c.Player.QuestGUI.SendAutoPatcher(m, x, y, 0);
                return true;
            }
            catch (Exception err) { MyConsole.SaveException(err); return false; }
        }

        // ------------------------------------------------------------------ report
        /// <summary>Data\mobile_quests\main_story_check.txt; MainStory calls it after every (re)load.</summary>
        internal static void WriteCheck()
        {
            var t = MobileQuestGuide.Table;
            var eng = QuestEngine.QuestEngineCore.Current;
            var sb = new StringBuilder();
            sb.AppendLine("# QahrMobile: main-story steps with a monster goal (written " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " by MobileQuestStory.cs)");
            sb.AppendLine("# id\tname\ttarget\tcount\tnpc map,x,y\tengine objectives\tproblems\tnearest spawn (map x,y distance)");
            foreach (var r in t.Values)
            {
                QuestEngine.EngineQuest q = null;
                if (eng != null) eng.Quests.TryGetValue(r.Id, out q);
                bool kill = q != null && q.Objectives.Any(o => o.Kind == QuestEngine.ObjectiveKind.Kill);
                bool isMon = Pool.MonsterFamilies != null && Pool.MonsterFamilies.Values.Any(f => f != null && QuestEngine.QuestGrammar.Norm(f.Name) == QuestEngine.QuestGrammar.Norm(r.Target));
                if (!kill && !isMon && !(q != null && q.IntentionText != null && System.Text.RegularExpressions.Regex.IsMatch(q.IntentionText, @"\b(Kill|Defeat|Hunt|Eliminate|Slay|Destroy|Clear)\b"))) continue;
                string spot = "-";
                uint m; ushort x, y; int d;
                if (NearestSpot(r.Target, r.Map, (int)r.X, (int)r.Y, out m, out x, out y, out d))
                    spot = m + " " + x + "," + y + (d == int.MaxValue ? " (other map)" : " d=" + d);
                sb.Append(r.Id).Append('\t').Append(r.Name).Append('\t').Append(r.Target).Append('\t').Append(r.Count).Append('\t')
                  .Append(r.Map + " " + r.X + "," + r.Y).Append('\t')
                  .Append(q == null ? "not in engine" : string.Join(" | ", q.Objectives.Select(o => o.Describe()))).Append('\t')
                  .Append(q == null ? "" : string.Join(" | ", q.Problems)).Append('\t').Append(spot).AppendLine();
            }
            File.WriteAllText(Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests", "main_story_check.txt"), sb.ToString(), Encoding.UTF8);
        }

        static void Log(Client.GameClient c, uint id, string what)
        {
            try
            {
                File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | MAINSTORY | " + c.Player.UID + " " + c.Player.Name + " " + id + " " + what + Environment.NewLine);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }
    }
}
