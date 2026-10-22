// تقرير أداء دوري: سطر واحد في الكونسول كل N ثانية من PerfMetrics + معلومات البروسيس.
// الإعداد: ServerConfig.ini -> [Diagnostics] PerfReportSeconds=60  (0 = مقفول).
// ثريد خلفية؛ أي exception بيتسجّل والتقرير يكمّل — عمره ما يوقع السيرفر.
using System;
using System.Diagnostics;
using System.Threading;

namespace Qahr.Mobile
{
    public static class PerfReporter
    {
        private static Thread _thread;
        private static readonly object _sync = new object();

        public static void Start()
        {
            try
            {
                int seconds = SharedConfig.Int("Diagnostics", "PerfReportSeconds", 60);
                if (seconds <= 0)
                    return;
                lock (_sync)
                {
                    if (_thread != null)
                        return;
                    _thread = new Thread(() => Run(seconds));
                    _thread.IsBackground = true;
                    _thread.Name = "PerfReporter";
                    _thread.Priority = ThreadPriority.BelowNormal;
                    _thread.Start();
                }
            }
            catch (Exception e)
            {
                try { MyConsole.WriteLine("PerfReporter failed to start: " + e.Message, ConsoleColor.DarkYellow); } catch { }
            }
        }

        private struct Snapshot
        {
            public long PacketsIn, PacketsOut, BytesIn, BytesOut;
            public long Saves, SaveMsTotal;
            public long Ticks, TickUsTotal;
            public long DbFailed;
            public int Gc0, Gc1, Gc2;
            public TimeSpan Cpu;
            public long WallTicks;

            public static Snapshot Take(Process proc)
            {
                Snapshot s = new Snapshot();
                s.PacketsIn = Interlocked.Read(ref PerfMetrics.PacketsIn);
                s.PacketsOut = Interlocked.Read(ref PerfMetrics.PacketsOut);
                s.BytesIn = Interlocked.Read(ref PerfMetrics.BytesIn);
                s.BytesOut = Interlocked.Read(ref PerfMetrics.BytesOut);
                s.Saves = Interlocked.Read(ref PerfMetrics.Saves);
                s.SaveMsTotal = Interlocked.Read(ref PerfMetrics.SaveMsTotal);
                s.Ticks = Interlocked.Read(ref PerfMetrics.Ticks);
                s.TickUsTotal = Interlocked.Read(ref PerfMetrics.TickUsTotal);
                s.DbFailed = Interlocked.Read(ref PerfMetrics.DbJobsFailed);
                s.Gc0 = GC.CollectionCount(0);
                s.Gc1 = GC.CollectionCount(1);
                s.Gc2 = GC.CollectionCount(2);
                try { proc.Refresh(); s.Cpu = proc.TotalProcessorTime; } catch { s.Cpu = TimeSpan.Zero; }
                s.WallTicks = Stopwatch.GetTimestamp();
                return s;
            }
        }

        private static void Run(int seconds)
        {
            Process proc = null;
            Snapshot prev = new Snapshot();
            bool havePrev = false;
            while (true)
            {
                try
                {
                    if (proc == null)
                        proc = Process.GetCurrentProcess();
                    if (!havePrev)
                    {
                        prev = Snapshot.Take(proc);
                        PerfMetrics.TakeSaveMaxWindow();
                        PerfMetrics.TakeTickMaxWindow();
                        havePrev = true;
                    }
                }
                catch (Exception e)
                {
                    LogError(e);
                }

                Thread.Sleep(seconds * 1000);

                try
                {
                    if (proc == null)
                        continue;
                    Snapshot cur = Snapshot.Take(proc);
                    long saveMax = PerfMetrics.TakeSaveMaxWindow();
                    long tickMaxUs = PerfMetrics.TakeTickMaxWindow();
                    Report(proc, prev, cur, saveMax, tickMaxUs);
                    prev = cur;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
        }

        private static void Report(Process proc, Snapshot a, Snapshot b, long saveMaxMs, long tickMaxUs)
        {
            double wallSec = (b.WallTicks - a.WallTicks) / (double)Stopwatch.Frequency;
            if (wallSec <= 0) wallSec = 1;

            int online = 0;
            try { if (Pool.GamePoll != null) online = Pool.GamePoll.Count; } catch { }

            double pinS = (b.PacketsIn - a.PacketsIn) / wallSec;
            double poutS = (b.PacketsOut - a.PacketsOut) / wallSec;
            double kbinS = (b.BytesIn - a.BytesIn) / 1024.0 / wallSec;
            double kboutS = (b.BytesOut - a.BytesOut) / 1024.0 / wallSec;

            long saves = b.Saves - a.Saves;
            double saveAvg = saves > 0 ? (b.SaveMsTotal - a.SaveMsTotal) / (double)saves : 0;

            long ticks = b.Ticks - a.Ticks;
            double tickAvgMs = ticks > 0 ? (b.TickUsTotal - a.TickUsTotal) / (double)ticks / 1000.0 : 0;
            double tickMaxMs = tickMaxUs / 1000.0;

            long dbPending = Interlocked.Read(ref PerfMetrics.DbJobsQueued) - Interlocked.Read(ref PerfMetrics.DbJobsDone) - b.DbFailed;
            if (dbPending < 0) dbPending = 0;
            long dbFailWindow = b.DbFailed - a.DbFailed;

            long wsMb = 0;
            int threads = 0;
            try { wsMb = proc.WorkingSet64 / (1024 * 1024); } catch { }
            try { threads = proc.Threads.Count; } catch { }
            long heapMb = GC.GetTotalMemory(false) / (1024 * 1024);

            double cpu = 0;
            if (a.Cpu != TimeSpan.Zero || b.Cpu != TimeSpan.Zero)
                cpu = (b.Cpu - a.Cpu).TotalSeconds / (wallSec * Environment.ProcessorCount) * 100.0;
            if (cpu < 0) cpu = 0;

            string line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "[Perf {0}s] online={1} | in {2:0}pk/s {3:0.0}KB/s | out {4:0}pk/s {5:0.0}KB/s | save n={6} avg={7:0.0}ms max={8}ms | db pend={9} fail={10}(+{11}) | tick n={12} avg={13:0.00}ms max={14:0.0}ms | gc {15}/{16}/{17} | mem ws={18}MB heap={19}MB | thr={20} | cpu={21:0.0}%",
                (int)Math.Round(wallSec), online,
                pinS, kbinS, poutS, kboutS,
                saves, saveAvg, saveMaxMs,
                dbPending, b.DbFailed, dbFailWindow,
                ticks, tickAvgMs, tickMaxMs,
                b.Gc0 - a.Gc0, b.Gc1 - a.Gc1, b.Gc2 - a.Gc2,
                wsMb, heapMb, threads, cpu);
            // السطر الكامل (كل الأرقام) بيتحفظ في perf_log.txt للتاريخ والمقارنة؛ الكونسول بيعرض نسخة مختصرة ملوّنة
            try { System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "perf_log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine); } catch { }

            var inv = System.Globalization.CultureInfo.InvariantCulture;
            bool utf8 = false;
            try { utf8 = Console.OutputEncoding.CodePage == 65001; } catch { }
            string sep = utf8 ? " │ " : " | ";
            const ConsoleColor L = ConsoleColor.DarkGray, V = ConsoleColor.White, S = ConsoleColor.DarkGray;

            ConsoleColor cpuC = cpu >= 70 ? ConsoleColor.Red : cpu >= 35 ? ConsoleColor.Yellow : ConsoleColor.Green;
            ConsoleColor saveC = saveMaxMs >= 2000 ? ConsoleColor.Red : saveMaxMs >= 500 ? ConsoleColor.Yellow : V;
            ConsoleColor tickC = tickMaxMs >= 100 ? ConsoleColor.Red : tickMaxMs >= 20 ? ConsoleColor.Yellow : V;
            string db; ConsoleColor dbC;
            if (dbFailWindow > 0) { db = "FAIL " + dbFailWindow; dbC = ConsoleColor.Red; }
            else if (dbPending > 100) { db = "queue " + dbPending; dbC = ConsoleColor.Yellow; }
            else { db = dbPending > 0 ? "queue " + dbPending : "ok"; dbC = ConsoleColor.Green; }

            // مختصر عشان يدخل في سطر واحد (~85 حرف). الأرقام الكاملة في perf_log.txt
            string save = saves == 0 ? "0" : saves + " · " + saveMaxMs + "ms";
            long gc2 = b.Gc2 - a.Gc2;
            var parts = new System.Collections.Generic.List<object>
            {
                "ONLINE ", L, online.ToString(), ConsoleColor.Cyan, sep, S,
                "CPU ", L, cpu.ToString("0.0", inv) + "%", cpuC, sep, S,
                "RAM ", L, wsMb + "MB", V, sep, S,
                "NET ", L, pinS.ToString("0", inv) + "/" + poutS.ToString("0", inv), V, sep, S,
                "SAVE ", L, save, saveC, sep, S,
                "DB ", L, db, dbC, sep, S,
                "TICK ", L, tickMaxMs.ToString("0.0", inv) + "ms", tickC
            };
            if (gc2 > 0) { parts.Add(sep); parts.Add(S); parts.Add("GC2 "); parts.Add(L); parts.Add(gc2.ToString()); parts.Add(ConsoleColor.Yellow); }
            MyConsole.WriteParts(parts.ToArray());
        }

        private static void LogError(Exception e)
        {
            try { MyConsole.WriteLine("PerfReporter error: " + e.Message, ConsoleColor.DarkYellow); } catch { }
        }
    }
}
