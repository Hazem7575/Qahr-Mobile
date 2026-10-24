using System;
using System.Collections.Generic;

namespace Qahr.Mobile
{
    /// <summary>A map point: map id and cell.</summary>
    public struct Place
    {
        public uint Map;
        public ushort X, Y;

        public Place(uint map, ushort x, ushort y)
        {
            Map = map;
            X = x;
            Y = y;
        }

        public override string ToString() { return Map + " (" + X + "," + Y + ")"; }
    }

    /// <summary>
    /// The named places the server code sends players to (Data\places.txt, "name map x y", re-read when saved).
    /// Code asks by name (Places.TwinCity, Places.TryGet("market", out p)), so moving a point is a data edit, not a code edit.
    /// </summary>
    public static class Places
    {
        /// <summary>Used only when places.txt has no "twin" line, so a player is never sent to map 0.</summary>
        static readonly Place DefaultTwinCity = new Place(1002, 410, 354);

        static readonly Phone.DataFile<Dictionary<string, Place>> File =
            new Phone.DataFile<Dictionary<string, Place>>("places.txt", "Places", Parse);

        static Dictionary<string, Place> Parse(string[] lines)
        {
            var d = new Dictionary<string, Place>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in Phone.DataFile.Clean(lines))
            {
                var p = Phone.DataFile.Words(line);
                uint map; ushort x, y;
                if (p.Length >= 4 && uint.TryParse(p[1], out map) && ushort.TryParse(p[2], out x) && ushort.TryParse(p[3], out y))
                    d[p[0]] = new Place(map, x, y);
            }
            return d;
        }

        public static bool TryGet(string name, out Place place)
        {
            return File.Value.TryGetValue(name ?? "", out place);
        }

        public static IEnumerable<string> Names { get { return File.Value.Keys; } }

        /// <summary>Twin City square: the "back to town" point (revive, end of an event, leaving a special map ...).</summary>
        public static Place TwinCity
        {
            get
            {
                Place p;
                return TryGet("twin", out p) ? p : DefaultTwinCity;
            }
        }

        /// <summary>The Market (places.txt "market"); Twin City when the line is missing.</summary>
        public static Place Market
        {
            get
            {
                Place p;
                return TryGet("market", out p) ? p : TwinCity;
            }
        }
    }
}
