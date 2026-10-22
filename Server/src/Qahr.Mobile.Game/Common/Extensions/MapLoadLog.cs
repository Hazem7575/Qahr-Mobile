// قياس زمن تحميل الخريطة (أول مرة لاعب يدخلها) لكل مرحلة، بيتكتب في mapload_log.txt (مش في الكونسول).
// [Diagnostics] MapLoadBench=1 في ServerConfig.ini: بعد الإقلاع بيحمّل خرايط Traps.txt كلها في الخلفية ويكتب ملخّص
// (للقياس قبل/بعد بس؛ رجّعه 0 بعدها).
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Qahr.Mobile
{
    /// <summary>زمن مراحل دخول اللاعب (توكن / تحميل الشخصية / إرسال الخريطة) -> login_log.txt. كله على نفس الثريد.</summary>
    public static class LoginTiming
    {
        [ThreadStatic] public static double TokenMs;
        private static readonly object fileLock = new object();
        public static void Write(string name, double loadMs, double mapMs)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + name + " token=" + TokenMs.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)
                    + "ms load=" + loadMs.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "ms map=" + mapMs.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)
                    + "ms total=" + (TokenMs + loadMs + mapMs).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "ms";
                lock (fileLock)
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "login_log.txt", line + Environment.NewLine);
            }
            catch { }
            TokenMs = 0;
        }
    }

    public sealed class MapLoadLog
    {
        private static readonly object fileLock = new object();
        public static long Maps, TotalMs;
        private readonly Stopwatch total = Stopwatch.StartNew();
        private readonly Stopwatch stage = Stopwatch.StartNew();
        private readonly StringBuilder parts = new StringBuilder();
        private readonly int mapId;

        public MapLoadLog(int mapId) { this.mapId = mapId; }

        public void Mark(string name)
        {
            parts.Append(' ').Append(name).Append('=').Append(stage.Elapsed.TotalMilliseconds.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));
            stage.Restart();
        }

        public void Done()
        {
            double ms = total.Elapsed.TotalMilliseconds;
            System.Threading.Interlocked.Increment(ref Maps);
            System.Threading.Interlocked.Add(ref TotalMs, (long)ms);
            Write("map " + mapId + " loaded in " + ms.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " ms |" + parts);
        }

        public static void Write(string line)
        {
            try
            {
                lock (fileLock)
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "mapload_log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine);
            }
            catch { }
        }

        /// <summary>[Diagnostics] MapLoadBench=1: يحمّل خرايط Traps.txt في الخلفية ويكتب الملخّص.</summary>
        public static void StartBenchIfEnabled()
        {
            if (SharedConfig.Int("Diagnostics", "MapLoadBench", 0) == 0) return;
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Threading.Thread.Sleep(2000);
                    var ids = new System.Collections.Generic.SortedSet<uint>();
                    foreach (var l in File.ReadAllLines(Program.ServerConfig.DbLocation + "Traps.txt"))
                    {
                        if (l.Length == 0) break;
                        uint id;
                        if (uint.TryParse(l.Split(',')[0], out id)) ids.Add(id);
                    }
                    long gc0 = GC.CollectionCount(2);
                    var sw = Stopwatch.StartNew();
                    int loaded = 0;
                    foreach (var id in ids)
                        if (!Pool.ServerMaps.Base.ContainsKey(id)) { Role.GameMap.EnterMap((int)id); loaded++; }
                    Write("BENCH: " + loaded + " maps in " + sw.ElapsedMilliseconds + " ms (avg " + (loaded == 0 ? 0 : sw.ElapsedMilliseconds / loaded) + " ms/map), gen2 GCs: " + (GC.CollectionCount(2) - gc0));
                }
                catch (Exception e) { Write("BENCH failed: " + e); }
            }) { IsBackground = true, Name = "MapLoadBench" }.Start();
        }
    }
}
