// ============================================================================
//  QahrMobile: quest unblock pass of the quest engine.
//
//  Runs inside QuestGrammar.Build (two hooks): Apply() right after quests.json / festival overlays (before the
//  NPC aliases, slots and workability checks), Report() at the end of the build.
//
//  Apply turns "scripted" goal lines of the mobile questinfo.ini into goals the server can check:
//    - "Defeat/Kill/Hunt/Eliminate/Slay [lua Name,</F>Func...][chk_quest id,1,slot,n]"  -> Kill n x Name
//      (generic "monsters" / "Get N PiecesofEthersuit dropped from monsters" -> Kill n x any monster)
//    - "View/Find/Talk to/Meet/Visit/Ask/Deliver ... [lua Name,...]" or plain "Find David" -> Talk to the NPC
//      carrying that name in the quest data (BeginNpcId / FinishNpcId / [npc] links of all quests; a placed NPC on
//      the quest's map is preferred)
//  and gives a small default prize (part of an EXP ball + silver) to mobile quests whose Prize= is empty.
//  The main-story steps of Data\mobile_quests\main_story.txt are left alone (MobileQuestGuide runs them), and
//  quests with explicit quests.json objectives are never touched.
//  Report writes Data\mobile_quests\unblock_report.csv: every quest with its owner state and the problems that keep
//  it from being engine-workable (used to count the blockers by reason).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.QuestEngine
{
    public static class QuestUnblock
    {
        public static int KillsMade, TalksMade, AnyKillsMade, PrizesFilled;

        sealed class NpcRef { public uint Id; public uint Map; public ushort X, Y; }

        static readonly Regex LuaTok = new Regex(@"\[lua\s+([^,\]]*),\s*</F>([A-Za-z0-9_]+)([^\]]*)\]", RegexOptions.Compiled);
        static readonly Regex AnyTok = new Regex(@"\[\s*([A-Za-z_]+)[ ,]([^\[\]]*)\]", RegexOptions.Compiled);
        static readonly Regex NpcTok = new Regex(@"\[npc\s*([^,\]]*),\s*(-?\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex KillVerb = new Regex(@"^(defeat|kill|hunt|eliminate|slay|exterminate|destroy|subdue|beat|challenge|wipe out|clear)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex TalkVerb = new Regex(@"^(view|find|talk to|talk with|speak to|speak with|meet|meet with|visit|welcome|ask|ask about|report to|deliver to|deliver|look for|go to|tank to|consult|greet|see|search for|send a message to|send|inquire about|return to)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex Generic = new Regex(@"^(monsters?|allmonsters|enemies|kill\d*monsters?|\d*monsters?)$", RegexOptions.Compiled);
        static readonly Regex DropPieces = new Regex(@"^get\b.*dropped from monsters", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex PlainMonsters = new Regex(@"^(defeat|kill|eliminate|slay|clear)\s+(\d+\s+)?monsters\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static HashSet<uint> MainStoryIds()
        {
            var set = new HashSet<uint>();
            try
            {
                string f = Path.Combine(Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests"), "main_story.txt");
                if (File.Exists(f))
                    foreach (var l in File.ReadLines(f))
                    {
                        if (l.Length == 0 || !char.IsDigit(l[0])) continue;
                        int tab = l.IndexOf('\t'); uint id;
                        if (tab > 0 && uint.TryParse(l.Substring(0, tab), out id)) set.Add(id);
                    }
            }
            catch { }
            return set;
        }

        static uint U(string s) { uint v; return uint.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        /// <summary>name (normalized) -> NPC spots, from every BeginNpcId / FinishNpcId / [npc] link of Questinfo.ini.</summary>
        static Dictionary<string, List<NpcRef>> NpcNames(List<Dictionary<string, string>> sections, Dictionary<uint, uint> beginMap)
        {
            var d = new Dictionary<string, List<NpcRef>>();
            Action<string, uint, uint, ushort, ushort> add = (name, id, map, x, y) =>
            {
                string n = QuestGrammar.Norm(name);
                if (n.Length < 2 || id == 0 || id >= int.MaxValue) return;
                List<NpcRef> l;
                if (!d.TryGetValue(n, out l)) d[n] = l = new List<NpcRef>();
                if (!l.Any(r => r.Id == id && r.Map == map)) l.Add(new NpcRef { Id = id, Map = map, X = x, Y = y });
            };
            foreach (var s in sections)
            {
                string v;
                uint mid = s.TryGetValue("MissionId", out v) ? U(v) : 0;
                foreach (string key in new[] { "BeginNpcId", "FinishNpcId" })
                {
                    if (!s.TryGetValue(key, out v)) continue;
                    var f = v.Split(',');
                    if (f.Length >= 5) add(f[4], U(f[0]), U(f[1]), (ushort)U(f[2]), (ushort)U(f[3]));
                    if (key == "BeginNpcId" && mid != 0 && f.Length >= 2) beginMap[mid] = U(f[1]);
                }
                foreach (var kv in s)
                {
                    if (kv.Value.IndexOf("[npc", StringComparison.OrdinalIgnoreCase) < 0) continue;
                    foreach (Match m in NpcTok.Matches(kv.Value))
                    {
                        int id; int.TryParse(m.Groups[2].Value, out id);
                        if (id > 0) add(m.Groups[1].Value, (uint)id, U(m.Groups[3].Value), (ushort)U(m.Groups[4].Value), (ushort)U(m.Groups[5].Value));
                    }
                }
            }
            return d;
        }

        static uint PickNpc(Dictionary<string, List<NpcRef>> names, string name, uint map, HashSet<uint> placed)
        {
            List<NpcRef> l;
            string n = QuestGrammar.Norm(name);
            if (n.Length < 2 || !names.TryGetValue(n, out l) || l.Count == 0) return QuestUnblockNames.Find(name, placed);// client STR_NPC_ID table
            NpcRef best = null; int bestScore = -1;
            foreach (var r in l)
            {
                int sc = 0;
                if (placed != null && placed.Contains(r.Id)) sc += 4;
                if (map != 0 && r.Map == map) sc += 2;
                if (sc > bestScore) { best = r; bestScore = sc; }
            }
            return best != null ? best.Id : 0;
        }

        static bool IsMonster(HashSet<string> monsters, string norm)
        {
            if (monsters.Contains(norm)) return true;
            return monsters.Contains(QuestGrammar.Singular(norm));
        }

        /// <summary>Post-parse fixes of the objectives (before aliases / workability are computed).</summary>
        public static void Apply(QuestTable table, HashSet<string> monsters, HashSet<string> spawned, HashSet<uint> placed)
        {
            KillsMade = TalksMade = AnyKillsMade = PrizesFilled = 0;
            try
            {
                var main = MainStoryIds();
                var sections = QuestGrammar.ReadSections(Path.Combine(Program.ServerConfig.DbLocation, "Questinfo.ini"));
                var beginMap = new Dictionary<uint, uint>();
                var names = NpcNames(sections, beginMap);
                foreach (var q in table.Quests.Values)
                {
                    if (main.Contains(q.MissionId) || q.ObjectivesExplicit) continue;
                    uint qmap; beginMap.TryGetValue(q.MissionId, out qmap);
                    foreach (var o in q.Objectives)
                    {
                        if (o.Kind != ObjectiveKind.Unknown || o.Slot == 0 || string.IsNullOrEmpty(o.Text)) continue;
                        Convert(q, o, monsters, names, qmap, placed);
                    }
                }
                FillPrizes(table, main);
            }
            catch (Exception e) { MyConsole.WriteLine("[QuestUnblock] apply: " + e.Message); }
        }

        static void Convert(EngineQuest q, Objective o, HashSet<string> monsters, Dictionary<string, List<NpcRef>> names, uint qmap, HashSet<uint> placed)
        {
            string text = o.Text.Replace("\\n", " ");
            var lua = LuaTok.Match(text);
            int cut = text.IndexOf('[');
            string verb = (cut >= 0 ? text.Substring(0, cut) : text).Trim().TrimStart('·', '-', ' ').Trim();
            string plain = AnyTok.Replace(text, " ").Trim().TrimStart('·', '-', ' ').Trim();
            if (lua.Success)
            {
                string name = lua.Groups[1].Value.Trim();
                string norm = QuestGrammar.Norm(name);
                if (KillVerb.IsMatch(verb) && norm.Length > 0)
                {
                    if (Generic.IsMatch(norm)) { MakeAnyKill(o); return; }
                    if (verb.StartsWith("clear", StringComparison.OrdinalIgnoreCase) && !IsMonster(monsters, norm)) return;// "Clear pebbles"
                    string mon = monsters.Contains(norm) ? norm : (monsters.Contains(QuestGrammar.Singular(norm)) ? QuestGrammar.Singular(norm) : norm);
                    o.Kind = ObjectiveKind.Kill; o.Monster = mon; o.Name = name; KillsMade++;
                    return;
                }
                if (DropPieces.IsMatch(plain)) { MakeAnyKill(o); return; }
                if (TalkVerb.IsMatch(verb) || verb.Length == 0 || Regex.IsMatch(verb, @"^(rescue|free|deliver)\b", RegexOptions.IgnoreCase))
                {
                    uint id = PickNpc(names, name, qmap, placed);
                    if (id != 0) { o.Kind = ObjectiveKind.Talk; o.NpcId = id; o.Name = name; TalksMade++; }
                    return;
                }
                return;
            }
            // no link: "Eliminate monsters." (count > 1), "Find David", "Send a message to Jack"
            if (PlainMonsters.IsMatch(plain) && o.Count > 1) { MakeAnyKill(o); return; }
            var tv = TalkVerb.Match(plain);
            if (tv.Success)
            {
                string rest = plain.Substring(tv.Length).Trim().TrimEnd('.', ' ');
                if (rest.StartsWith("the ", StringComparison.OrdinalIgnoreCase)) rest = rest.Substring(4);
                if (rest.Length == 0 || rest.Split(' ').Length > 3) return;
                uint id = PickNpc(names, rest, qmap, placed);
                if (id != 0) { o.Kind = ObjectiveKind.Talk; o.NpcId = id; o.Name = rest; TalksMade++; }
            }
        }

        static void MakeAnyKill(Objective o)
        {
            o.Kind = ObjectiveKind.Kill; o.AnyMonster = true; o.Name = "monster"; o.MinLevelGap = 20; AnyKillsMade++;
        }

        /// <summary>Mobile quests with an empty Prize= get a small reward: part of an EXP ball (scales with the level)
        /// and some silver. Not for main-story steps (MobileQuestGuide pays), festivals, Chinju bounties, quests.json prizes.</summary>
        static void FillPrizes(QuestTable table, HashSet<uint> main)
        {
            var all = Database.QuestInfo.AllQuests;
            foreach (var q in table.Quests.Values)
            {
                if (main.Contains(q.MissionId) || q.TypeId >= 8 || q.Prize.FromJson || !q.Prize.IsEmpty || q.Extra != null) continue;
                if (!string.IsNullOrWhiteSpace(q.OfficialPrizeText) || q.BeginNpc == 30893 || q.IsBounty || q.HandCoded) continue;
                if (q.Objectives.Count == 0 && q.IntentionLines == 0) continue;// pure flag / tracker entries
                Database.QuestInfo.DBQuest dbq = null;
                if (all != null) all.TryGetValue(q.MissionId, out dbq);
                uint lv = dbq != null ? Math.Min((uint)140, (uint)dbq.Lv_min) : 1;
                bool daily = q.TypeId == 3 || q.Repeat != 0;
                q.Prize = new Prize { Text = "QahrMobile default", ExpBallUnits = daily ? 120u : 300u, Silver = (ulong)(daily ? 200 + 20 * lv : 500 + 50 * lv) };
                PrizesFilled++;
            }
        }

        static string Csv(string s)
        {
            s = (s ?? "").Replace("\r", " ").Replace("\n", " ");
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>Writes the per-quest state table (after the build decided ownership / workability).</summary>
        public static void Report(QuestTable table)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("id,name,type,lvmin,lvmax,owned,workable,disabled,handcoded,json,begin,finish,problems,objectives,prize");
                var all = Database.QuestInfo.AllQuests;
                foreach (var q in table.Quests.Values.OrderBy(x => x.MissionId))
                {
                    Database.QuestInfo.DBQuest dbq = null;
                    if (all != null) all.TryGetValue(q.MissionId, out dbq);
                    sb.Append(q.MissionId).Append(',').Append(Csv(q.Name)).Append(',').Append(q.TypeId).Append(',')
                      .Append(dbq != null ? dbq.Lv_min : 0).Append(',').Append(dbq != null ? dbq.Lv_max : 0).Append(',')
                      .Append(q.Owned ? 1 : 0).Append(',').Append(q.Workable ? 1 : 0).Append(',').Append(q.Disabled ? 1 : 0).Append(',')
                      .Append(q.HandCoded ? 1 : 0).Append(',').Append(q.FromJson ? 1 : 0).Append(',')
                      .Append(q.BeginNpc).Append(',').Append(q.FinishNpc).Append(',')
                      .Append(Csv(string.Join(" | ", q.Problems))).Append(',')
                      .Append(Csv(string.Join(" | ", q.Objectives.Select(o => o.Kind + ": " + (o.Kind == ObjectiveKind.Unknown ? o.Text : o.Describe()))))).Append(',')
                      .Append(Csv(q.Prize.Describe(q.ChainEnd)))
                      .AppendLine();
                }
                string dir = Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests");
                if (Directory.Exists(dir))
                    File.WriteAllText(Path.Combine(dir, "unblock_report.csv"), sb.ToString(), new UTF8Encoding(false));
                MyConsole.WriteLine("[QuestUnblock] lua/text goals -> kill " + KillsMade + ", any-monster kill " + AnyKillsMade + ", talk " + TalksMade + "; default prizes " + PrizesFilled);
            }
            catch (Exception e) { MyConsole.WriteLine("[QuestUnblock] report: " + e.Message); }
        }
    }
}
