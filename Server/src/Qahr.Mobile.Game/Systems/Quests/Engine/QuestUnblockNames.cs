// ============================================================================
//  QahrMobile: NPC name -> id fallback for QuestUnblock (talk goals named only by "[lua Name,...]").
//  Reads Data\mobile_quests\npc_names.txt ("id<TAB>name", the client's STR_NPC_ID_<id> table), re-read when the
//  file changes. A placed NPC (Npcs.txt / SobNpcs.txt) wins; else the lowest id carrying the name.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.QuestEngine
{
    public static class QuestUnblockNames
    {
        static Dictionary<string, List<uint>> _byName;
        static DateTime _stamp;

        static Dictionary<string, List<uint>> Table()
        {
            try
            {
                string f = Path.Combine(Path.Combine(Program.ServerConfig.DbLocation, "mobile_quests"), "npc_names.txt");
                if (!File.Exists(f)) return new Dictionary<string, List<uint>>();
                var st = File.GetLastWriteTimeUtc(f);
                if (_byName != null && st == _stamp) return _byName;
                var d = new Dictionary<string, List<uint>>();
                foreach (var l in File.ReadLines(f))
                {
                    if (l.Length == 0 || !char.IsDigit(l[0])) continue;
                    int tab = l.IndexOf('\t');
                    uint id;
                    if (tab <= 0 || !uint.TryParse(l.Substring(0, tab), out id) || id == 0) continue;
                    string n = QuestGrammar.Norm(l.Substring(tab + 1));
                    if (n.Length < 2) continue;
                    List<uint> ids;
                    if (!d.TryGetValue(n, out ids)) d[n] = ids = new List<uint>();
                    ids.Add(id);
                }
                _byName = d; _stamp = st;
                return d;
            }
            catch { return new Dictionary<string, List<uint>>(); }
        }

        /// <summary>NPC id for a name (0 = unknown). Placed NPCs first.</summary>
        public static uint Find(string name, HashSet<uint> placed)
        {
            List<uint> ids;
            if (!Table().TryGetValue(QuestGrammar.Norm(name), out ids) || ids.Count == 0) return 0;
            if (placed != null) foreach (uint id in ids) if (placed.Contains(id)) return id;
            return ids[0];
        }
    }
}
