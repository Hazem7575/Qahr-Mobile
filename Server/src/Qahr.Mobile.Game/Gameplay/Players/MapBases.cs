using System.Collections.Generic;

namespace Qahr.Mobile.Role
{
    /// <summary>
    /// Map ids that are loaded from another map (instances, event copies, quest phases): Data\map_bases.txt,
    /// "id base" or "first-last base", the first matching line wins. Used by GameMap.ResolveBase.
    /// </summary>
    public static class MapBases
    {
        struct Row
        {
            public uint First, Last, Base;
        }

        static readonly Phone.DataFile<List<Row>> File = new Phone.DataFile<List<Row>>("map_bases.txt", "MapBases", Parse);

        static List<Row> Parse(string[] lines)
        {
            var rows = new List<Row>();
            foreach (var line in Phone.DataFile.Clean(lines))
            {
                var p = Phone.DataFile.Words(line);
                if (p.Length < 2) continue;
                uint first, last, b;
                int dash = p[0].IndexOf('-');
                bool ok = dash > 0
                    ? uint.TryParse(p[0].Substring(0, dash), out first) & uint.TryParse(p[0].Substring(dash + 1), out last)
                    : (uint.TryParse(p[0], out first) & (last = first) == first);
                if (ok && uint.TryParse(p[1], out b))
                    rows.Add(new Row { First = first, Last = last, Base = b });
            }
            return rows;
        }

        /// <summary>The map id whose files this map uses (the id itself when it is not in the table).</summary>
        public static uint Resolve(uint id)
        {
            foreach (var r in File.Value)
                if (id >= r.First && id <= r.Last)
                    return r.Base;
            return id;
        }
    }
}
