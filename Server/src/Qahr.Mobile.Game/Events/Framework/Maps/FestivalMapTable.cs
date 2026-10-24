// [FestivalMaps 2026-10-14] official festival map ids (static ids missing from GameMap.dat + 7-digit instance ids)
// -> the map doc the client renders.
//
// Data: Data\FestivalMaps\maps.txt, one row per map id (tab separated):
//     id  wireDoc  officialDoc  file  confidence  evidence
//   id          the official map id used by Questinfo.ini (e.g. 1003981)
//   wireDoc     the doc id the server sends (MsgMapInfo.BaseID / teleport MapID). It is the official doc when our client's
//               GameMap.dat has it, otherwise another GameMap.dat id that points at the SAME map file.
//   officialDoc the official doc id (client - orignal\ini\GameMap.dat)
//   file        the map file (map/map/x.7z -> the server loads map/map/x.dmap)
//
// How it works: GameMap.ResolveBase(id) asks TryResolve first (one hook line). The client never sees the 7-digit id:
// the server already sends BaseID for remapped maps (GameClient.Teleport + MapStatusCreate). Maps load lazily
// (Pool.ServerMaps.ContainsKey -> EnterMap), so a row added to maps.txt works on the next visit of that id
// (the file is re-read when it changes); a map that is ALREADY loaded keeps its old doc until restart.
using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.FestivalMaps
{
    public static class FestivalMapTable
    {
        public sealed class Row
        {
            public uint Id, WireDoc, OfficialDoc;
            public string File, Confidence, Evidence;
        }

        private static readonly object Sync = new object();
        private static Dictionary<uint, Row> _rows = new Dictionary<uint, Row>();
        private static DateTime _stamp = DateTime.MinValue;
        private static DateTime _nextCheck = DateTime.MinValue;

        public static string FilePath { get { return Path.Combine(Program.ServerConfig.DbLocation, "FestivalMaps", "maps.txt"); } }

        /// <summary>The doc for an official festival map id, or false when the id is not in maps.txt.</summary>
        public static bool TryResolve(uint id, out uint doc)
        {
            doc = 0;
            if (id < 10000) return false; // all festival ids are >= 10820: keep the hot path for normal maps free
            Row r;
            try
            {
                Refresh();
                lock (Sync) { if (!_rows.TryGetValue(id, out r)) return false; }
                // the doc must be loadable: register its dmap if this server's GameMap.dat does not list it
                if (!Role.GameMap.MapContents.ContainsKey((int)r.WireDoc))
                {
                    string dmap = r.File.Replace(".7z", ".dmap");
                    if (!Role.MapFiles.Exists(dmap)) return false;
                    lock (Role.GameMap.MapContents) Role.GameMap.MapContents[(int)r.WireDoc] = dmap;
                }
                doc = r.WireDoc;
                return true;
            }
            catch (Exception e) { MyConsole.WriteLine("[FestivalMaps] " + e.Message); return false; }
        }

        /// <summary>All rows (copy), for tools / admin.</summary>
        public static List<Row> All()
        {
            Refresh();
            lock (Sync) return new List<Row>(_rows.Values);
        }

        private static void Refresh()
        {
            DateTime now = DateTime.UtcNow;
            if (now < _nextCheck) return;
            _nextCheck = now.AddSeconds(5);
            string p = FilePath;
            if (!System.IO.File.Exists(p)) { lock (Sync) _rows = new Dictionary<uint, Row>(); return; }
            DateTime st = System.IO.File.GetLastWriteTimeUtc(p);
            if (st == _stamp) return;
            var rows = new Dictionary<uint, Row>();
            foreach (string raw in System.IO.File.ReadAllLines(p))
            {
                string l = raw.Trim();
                if (l.Length == 0 || l[0] == '#') continue;
                string[] c = l.Split('\t');
                if (c.Length < 4) continue;
                uint id, wire, off;
                if (!uint.TryParse(c[0], out id) || !uint.TryParse(c[1], out wire) || wire == 0 || wire == id) continue;
                uint.TryParse(c[2], out off);
                string file = c[3].Trim().Replace('\\', '/');
                if (!file.StartsWith("map/map/", StringComparison.OrdinalIgnoreCase) || file.Contains("..")) continue;
                rows[id] = new Row { Id = id, WireDoc = wire, OfficialDoc = off, File = file, Confidence = c.Length > 4 ? c[4] : "", Evidence = c.Length > 5 ? c[5] : "" };
            }
            lock (Sync) { _rows = rows; _stamp = st; }
            MyConsole.WriteLine("[FestivalMaps] " + rows.Count + " official festival map ids loaded from maps.txt");
        }
    }
}
