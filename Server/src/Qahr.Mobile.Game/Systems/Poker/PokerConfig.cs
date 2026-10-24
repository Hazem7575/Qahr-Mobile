// [Poker 2026-10-14] One editable config: Data\Poker\config.json (re-read when the file changes).
// Values copied from the client (official) and values the client does not carry (derived) are marked in the file.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.Poker
{
    public sealed class PokerLevel
    {
        public uint Id;            // client NewTexasTableList id
        public ulong MinBuyIn, MaxBuyIn, BetUnit;
        public uint Type;          // client "type": 0 Texas normal, 1 Texas crazy, 2 Omaha, 3 ...
        public bool Enabled = true;
    }

    public sealed class PokerSettings
    {
        public bool Enabled = true;
        public double RakePercent = 5;      // derived
        public ulong RakeCapBigBlinds = 3;  // derived
        public bool NoFlopNoDrop = true;    // derived
        public int ActionSeconds = 15;
        public int NextHandSeconds = 6;
        public int Seats = 9;
        public int MaxTablesPerLevel = 50;
        public int RequestCooldownMs = 300;
        public int SitOutHandsBeforeKick = 3;
        public Dictionary<uint, PokerLevel> Levels = new Dictionary<uint, PokerLevel>();
        public Dictionary<string, object> Raw = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    public static class PokerConfig
    {
        static volatile PokerSettings cur;
        static DateTime stamp = DateTime.MinValue;
        static readonly object sync = new object();
        public static string FolderOverride;

        public static string Folder
        {
            get
            {
                if (!string.IsNullOrEmpty(FolderOverride)) return FolderOverride;
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "Poker";
            }
        }

        public static PokerSettings Current
        {
            get
            {
                try
                {
                    string path = Path.Combine(Folder, "config.json");
                    DateTime t = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                    if (cur == null || t != stamp)
                        lock (sync)
                            if (cur == null || t != stamp) { cur = Load(path); stamp = t; }
                }
                catch (Exception e)
                {
                    PokerLog.Write("config error (poker disabled until fixed): " + e.Message);
                    cur = new PokerSettings { Enabled = false };
                }
                return cur;
            }
        }

        static PokerSettings Load(string path)
        {
            var s = new PokerSettings();
            if (!File.Exists(path)) { PokerLog.Write("config.json missing -> poker disabled"); s.Enabled = false; return s; }
            var root = AdminApi.AdminJson.Parse(File.ReadAllText(path, Encoding.UTF8)) as Dictionary<string, object>;
            if (root == null) throw new Exception("config.json is not an object");
            s.Raw = root;
            s.Enabled = B(root, "enabled", true);
            var cash = Obj(root, "cash");
            if (cash != null)
            {
                s.RakePercent = D(cash, "rakePercent", s.RakePercent);
                s.RakeCapBigBlinds = (ulong)D(cash, "rakeCapBigBlinds", s.RakeCapBigBlinds);
                s.NoFlopNoDrop = B(cash, "noFlopNoDrop", s.NoFlopNoDrop);
                s.ActionSeconds = (int)D(cash, "actionSeconds", s.ActionSeconds);
                s.NextHandSeconds = (int)D(cash, "nextHandSeconds", s.NextHandSeconds);
                s.Seats = (int)D(cash, "seats", s.Seats);
                s.MaxTablesPerLevel = (int)D(cash, "maxTablesPerLevel", s.MaxTablesPerLevel);
                s.RequestCooldownMs = (int)D(cash, "requestCooldownMs", s.RequestCooldownMs);
                s.SitOutHandsBeforeKick = (int)D(cash, "sitOutHandsBeforeKick", s.SitOutHandsBeforeKick);
                var lv = Arr(cash, "levels");
                if (lv != null)
                    foreach (var o in lv)
                    {
                        var m = o as Dictionary<string, object>;
                        if (m == null) continue;
                        var l = new PokerLevel
                        {
                            Id = (uint)D(m, "id", 0),
                            MinBuyIn = U(m, "min_enter", 0),
                            MaxBuyIn = U(m, "max_enter", 0),
                            BetUnit = U(m, "bet_unit", 0),
                            Type = (uint)D(m, "type", 0),
                            Enabled = B(m, "enabled", true)
                        };
                        if (l.Id != 0 && l.BetUnit > 1 && l.MinBuyIn > 0 && l.MaxBuyIn >= l.MinBuyIn) s.Levels[l.Id] = l;
                    }
            }
            if (s.Seats < 2 || s.Seats > 9) s.Seats = 9;
            if (s.ActionSeconds < 5) s.ActionSeconds = 5;
            if (s.NextHandSeconds < 2) s.NextHandSeconds = 2;
            if (s.RakePercent < 0 || s.RakePercent > 20) s.RakePercent = 5;
            PokerLog.Write("config loaded: " + s.Levels.Count + " cash levels, rake " + s.RakePercent + "% cap " + s.RakeCapBigBlinds + " BB");
            return s;
        }

        public static Dictionary<string, object> Obj(Dictionary<string, object> d, string k) { object o; return d != null && d.TryGetValue(k, out o) ? o as Dictionary<string, object> : null; }
        public static List<object> Arr(Dictionary<string, object> d, string k) { object o; return d != null && d.TryGetValue(k, out o) ? o as List<object> : null; }
        public static double Num(object o)
        {
            if (o == null) return 0;
            if (o is bool) return (bool)o ? 1 : 0;
            double d; return double.TryParse(o.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0;
        }
        public static double D(Dictionary<string, object> d, string k, double def) { object o; return d != null && d.TryGetValue(k, out o) && o != null ? Num(o) : def; }
        public static ulong U(Dictionary<string, object> d, string k, ulong def)
        {
            object o; ulong v;
            return d != null && d.TryGetValue(k, out o) && o != null && ulong.TryParse(o.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def;
        }
        public static bool B(Dictionary<string, object> d, string k, bool def) { object o; return d != null && d.TryGetValue(k, out o) && o is bool ? (bool)o : def; }
        public static string S(Dictionary<string, object> d, string k, string def) { object o; return d != null && d.TryGetValue(k, out o) && o is string ? (string)o : def; }
    }
}
