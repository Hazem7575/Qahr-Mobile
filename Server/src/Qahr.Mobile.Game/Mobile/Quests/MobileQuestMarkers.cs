using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

namespace Qahr.Mobile.Phone
{
    // QahrMobile: invisible quest markers. Questinfo.ini names some quest NPCs "~" (look NpcType4913 and friends: no
    // body, no name), e.g. 32378 at the Twin City gate for "Welcome Eloise" / "Returning". The official server spawns
    // them as personal NPCs for the one player on that step. Sent to everybody they are invisible but still take
    // clicks, so tapping the ground next to a real NPC opened the marker's dialog.
    // Here a marker is only sent to a player who has one of its quests in progress (finish / personal NPC) or who can
    // take the quest it starts (begin NPC). The bounty scene objects (MobileBountyGoals: Hostage, Flower, FishingSpot ...)
    // follow the same rule. This class is the one place that decides who sees a personal quest NPC; it is called from
    // RoleView when an NPC enters the player's screen.
    public static class MobileQuestMarkers
    {
        private sealed class Marker
        {
            public readonly List<uint> Finish = new List<uint>();  // quest in progress -> show
            public readonly List<uint> Begin = new List<uint>();   // quest can be taken -> show
        }

        static readonly DataFile<Dictionary<uint, Marker>> Markers =
            new DataFile<Dictionary<uint, Marker>>("Questinfo.ini", "QuestMarkers", Parse);

        private static readonly Regex NpcRef = new Regex(@"^(BeginNpcId|FinishNpcId|PersonalNpcId\d*)=(\d+)(?:,\d+,\d+,\d+,([^,]*))?", RegexOptions.Compiled);

        /// <summary>Personal quest NPCs: every PersonalNpcId of Questinfo.ini, plus the "~" markers (no body, no name).
        /// The official server spawns them only for the player on that quest step.</summary>
        private static Dictionary<uint, Marker> Parse(string[] lines)
        {
            var result = new Dictionary<uint, Marker>();
            var personal = new HashSet<uint>();
            var refs = new List<KeyValuePair<uint, KeyValuePair<bool, uint>>>(); // npc -> (isBegin, quest)
            uint mission = 0;
            var section = new List<KeyValuePair<string, uint>>();
            Action flush = () =>
            {
                if (mission != 0)
                    foreach (var r in section)
                        refs.Add(new KeyValuePair<uint, KeyValuePair<bool, uint>>(r.Value, new KeyValuePair<bool, uint>(r.Key == "BeginNpcId", mission)));
                section.Clear();
                mission = 0;
            };
            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.StartsWith("[")) { flush(); continue; }
                if (line.StartsWith("MissionId=")) { uint.TryParse(line.Substring(10), out mission); continue; }
                var m = NpcRef.Match(line);
                if (!m.Success) continue;
                uint npc = uint.Parse(m.Groups[2].Value);
                bool marker = m.Groups[3].Success && m.Groups[3].Value.Trim() == "~";
                if (marker || m.Groups[1].Value.StartsWith("PersonalNpcId", StringComparison.Ordinal)) personal.Add(npc);
                section.Add(new KeyValuePair<string, uint>(m.Groups[1].Value, npc));
            }
            flush();
            foreach (var r in refs)
            {
                if (!personal.Contains(r.Key)) continue;
                Marker mk;
                if (!result.TryGetValue(r.Key, out mk)) result[r.Key] = mk = new Marker();
                (r.Value.Key ? mk.Begin : mk.Finish).Add(r.Value.Value);
            }
            MyConsole.WriteLine("[Mobile] quest markers: " + result.Count + " personal quest NPCs are shown only to players on their step");
            return result;
        }

        /// <summary>True = do not send this NPC to this player.</summary>
        public static bool Hide(Client.GameClient c, uint npcUid)
        {
            if (MobileBountyGoals.IsHiddenObject(c, npcUid))
                return true;// bounty scene objects: only for players on that bounty
            Marker mk;
            if (!Markers.Value.TryGetValue(npcUid, out mk)) return false;
            try
            {
                var gui = c.Player.QuestGUI;
                foreach (uint q in mk.Finish)
                    if (gui.IsActiveQuest(q)) return false;
                foreach (uint q in mk.Begin)
                {
                    string why;
                    if (QuestEngine.QuestEngineCore.CanAccept(c, Database.QuestInfo.GetQuest(q), out why)) return false;
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); return false; }
            return true;
        }
    }
}
