// ============================================================================
//  [ChildrensDay2025 2026-10-15] Per-character Dream Park progress, one record per uid, reset when a NEW festival run starts.
//  File: Data\ChildrensDay2025\state.json (rewritten atomically after every change; tmp + replace).
//  Every caller takes DreamParkState.Sync for the whole read-check-deduct-grant-save sequence.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.ChildrensDay2025
{
    public sealed class DreamPlayer
    {
        public uint Uid; public string Name = ""; public int Run;
        public uint Points;                                  // Dream Points (50 per Starwish Stone used)
        public uint[] BPts = new uint[4];                    // structure Dream Pts, index 1..3
        public List<int>[] Events = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };// pending event ids 1..3 per structure
        public string OpDay = "";                            // last daily park operation (yyyy-MM-dd)
        public string Day = ""; public int BlessMask; public bool BlessDone;// Vault Palace Starlight Blessing of Day
        public uint StageMask; public uint RepeatClaimed;    // point stages 1..7 claimed, repeat stage claims
        public uint AchMask;                                 // structure achievements claimed
        public Dictionary<uint, uint> Bought = new Dictionary<uint, uint>();// store GoodsId -> bought this run
        public long Updated;
    }

    public static class DreamParkState
    {
        public static readonly object Sync = new object();
        static Dictionary<uint, DreamPlayer> _all;
        public static Func<DateTime> Clock;                  // harness
        public static DateTime Now { get { var c = Clock; return c != null ? c() : DateTime.Now; } }
        public static string Today { get { return Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); } }
        static string FilePath { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "ChildrensDay2025", "state.json"); } }

        static void Load()
        {
            if (_all != null) return;
            _all = new Dictionary<uint, DreamPlayer>();
            try
            {
                if (!File.Exists(FilePath)) return;
                var root = FL.ReadJson(FilePath);
                foreach (var o in FL.Arr(FL.Val(root, "players")) ?? new List<object>())
                {
                    var d = FL.Obj(o); if (d == null) continue;
                    var p = new DreamPlayer { Uid = FL.U(FL.Val(d, "uid")), Name = FL.Str(d, "name") ?? "", Run = (int)FL.U(FL.Val(d, "run")), Points = FL.U(FL.Val(d, "points")) };
                    if (p.Uid == 0) continue;
                    var b = FL.Arr(FL.Val(d, "bpts")); if (b != null) for (int i = 0; i < 3 && i < b.Count; i++) p.BPts[i + 1] = FL.U(b[i]);
                    var e = FL.Arr(FL.Val(d, "events"));
                    if (e != null) for (int i = 0; i < 3 && i < e.Count; i++) foreach (var x in FL.Arr(e[i]) ?? new List<object>()) { int v = (int)FL.U(x); if (v >= 1 && v <= 3) p.Events[i + 1].Add(v); }
                    p.OpDay = FL.Str(d, "opDay") ?? ""; p.Day = FL.Str(d, "day") ?? ""; p.BlessMask = (int)FL.U(FL.Val(d, "bless")); p.BlessDone = FL.Bool(d, "blessDone") == true;
                    p.StageMask = FL.U(FL.Val(d, "stages")); p.RepeatClaimed = FL.U(FL.Val(d, "repeat")); p.AchMask = FL.U(FL.Val(d, "ach"));
                    var bo = FL.Obj(FL.Val(d, "bought"));
                    if (bo != null) foreach (var kv in bo) { uint g; if (uint.TryParse(kv.Key, out g)) p.Bought[g] = FL.U(kv.Value); }
                    p.Updated = (long)FL.UL(FL.Val(d, "updated"));
                    _all[p.Uid] = p;
                }
            }
            catch (Exception ex) { try { MyConsole.WriteLine("[ChildrensDay2025] state.json ignored: " + ex.Message, ConsoleColor.Yellow); } catch { } }
        }

        /// <summary>The record of uid for the CURRENT run (a new run starts from zero). Call under Sync.</summary>
        public static DreamPlayer Get(uint uid, string name, int run)
        {
            Load();
            DreamPlayer p;
            if (!_all.TryGetValue(uid, out p) || p.Run != run)
            {
                p = new DreamPlayer { Uid = uid, Run = run };
                _all[uid] = p;
            }
            if (!string.IsNullOrEmpty(name)) p.Name = name;
            if (p.Day != Today) { p.Day = Today; p.BlessMask = 0; p.BlessDone = false; }
            return p;
        }

        public static List<DreamPlayer> Ranking(int run, uint minPoints, int top)
        {
            Load();
            return _all.Values.Where(p => p.Run == run && p.Points >= minPoints).OrderByDescending(p => p.Points).ThenBy(p => p.Updated).Take(top).ToList();
        }

        /// <summary>Persist everything. Call under Sync.</summary>
        public static void Save()
        {
            Load();
            try
            {
                var list = new List<object>();
                foreach (var p in _all.Values.OrderBy(x => x.Uid))
                {
                    var d = new Dictionary<string, object>();
                    d["uid"] = p.Uid; d["name"] = p.Name; d["run"] = p.Run; d["points"] = p.Points;
                    d["bpts"] = new List<object> { p.BPts[1], p.BPts[2], p.BPts[3] };
                    d["events"] = new List<object> { p.Events[1].Cast<object>().ToList(), p.Events[2].Cast<object>().ToList(), p.Events[3].Cast<object>().ToList() };
                    d["opDay"] = p.OpDay; d["day"] = p.Day; d["bless"] = p.BlessMask; d["blessDone"] = p.BlessDone;
                    d["stages"] = p.StageMask; d["repeat"] = p.RepeatClaimed; d["ach"] = p.AchMask;
                    var bo = new Dictionary<string, object>(); foreach (var kv in p.Bought) bo[kv.Key.ToString()] = kv.Value; d["bought"] = bo;
                    d["updated"] = p.Updated;
                    list.Add(d);
                }
                var root = new Dictionary<string, object>();
                root["_readme"] = "[ChildrensDay2025] Dream Park progress per character (written by the GameServer; do not edit while it runs).";
                root["players"] = list;
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                string tmp = FilePath + ".tmp";
                File.WriteAllText(tmp, AdminApi.AdminJson.Write(root), new UTF8Encoding(false));
                if (File.Exists(FilePath)) File.Replace(tmp, FilePath, null); else File.Move(tmp, FilePath);
            }
            catch (Exception ex) { try { MyConsole.WriteLine("[ChildrensDay2025] could not save state.json: " + ex.Message, ConsoleColor.Yellow); } catch { } }
        }

        /// <summary>Harness only.</summary>
        public static void ResetCache() { lock (Sync) _all = null; }

        public static void Log(Client.GameClient c, string what)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + (c != null && c.Player != null ? c.Player.UID + " " + c.Player.Name : "-") + " | " + what;
                File.AppendAllText(Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "ChildrensDay2025", "log.txt"), line + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }
    }
}
