using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: Data\mobile_quests\main_story.txt, read once for both users:
    ///   columns 1-13  the chain (MobileQuestGuide.Row: next step, finish NPC, place, goal count, dialog, Lua target ...)
    ///   columns 14-16 the reward of the step (MobileQuestStory.Reward: expPct, silver, items "id[xN][@class];...")
    /// After every (re)load MobileQuestStory writes main_story_check.txt.
    /// </summary>
    public static class MainStory
    {
        public sealed class Table
        {
            public readonly Dictionary<uint, MobileQuestGuide.Row> Rows = new Dictionary<uint, MobileQuestGuide.Row>();
            public readonly Dictionary<uint, MobileQuestStory.Reward> Rewards = new Dictionary<uint, MobileQuestStory.Reward>();
        }

        static readonly DataFile<Table> File = new DataFile<Table>(Path.Combine("mobile_quests", "main_story.txt"), "MainStory", Parse,
            _ => MobileQuestStory.WriteCheck());

        public static Table Current { get { return File.Value; } }

        static Table Parse(string[] lines)
        {
            var t = new Table();
            foreach (var line in lines)
            {
                if (line.Length == 0 || line[0] == '#') continue;
                var p = line.Split('\t');
                if (p.Length < 13) continue;
                uint id = U(p[0]);
                if (id == 0 || t.Rows.ContainsKey(id)) continue;
                t.Rows[id] = new MobileQuestGuide.Row
                {
                    Id = id, Next = U(p[1]), Npc = U(p[2]), Map = U(p[3]), X = U(p[4]), Y = U(p[5]),
                    Count = Math.Max(1, U(p[6])), Dialog = U(p[7]), Func = p[8], Target = p[9], NpcName = p[10],
                    LvMin = U(p[11]), Name = p[12]
                };
                if (p.Length >= 16)
                    t.Rewards[id] = ParseReward(p[13], p[14], p[15]);
            }
            MyConsole.WriteLine("[Mobile] main story: " + t.Rows.Count + " steps, " + t.Rewards.Count + " with rewards", ConsoleColor.Cyan);
            return t;
        }

        static MobileQuestStory.Reward ParseReward(string expPct, string silver, string items)
        {
            var r = new MobileQuestStory.Reward { ExpPct = U(expPct), Silver = U(silver) };
            foreach (var tok in (items ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string s = tok.Trim(), cls = "";
                if (s == "-" || s.Length == 0) continue;
                int at = s.IndexOf('@');
                if (at >= 0) { cls = s.Substring(at + 1).Trim().ToLowerInvariant(); s = s.Substring(0, at); }
                ushort n = 1;
                int x = s.IndexOf('x');
                if (x > 0) { n = (ushort)Math.Max(1, U(s.Substring(x + 1))); s = s.Substring(0, x); }
                uint item = U(s);
                if (item != 0) r.Items.Add(Tuple.Create(item, n, cls));
            }
            return r;
        }

        static uint U(string s)
        {
            uint v;
            return uint.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }
    }
}
