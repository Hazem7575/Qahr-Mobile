// عدّادات الأداء المشتركة (thread-safe وخفيفة جدًا: Interlocked بس).
// أي جزء في السيرفر يزوّد العدّاد بتاعه، وفيه reporter بيعرضهم (ثواني/دقايق) عشان نقيس قبل وبعد أي تحسين.
using System.Threading;

namespace Qahr.Mobile
{
    public static class PerfMetrics
    {
        // الشبكة
        public static long PacketsIn, PacketsOut, BytesIn, BytesOut;
        // الحفظ (SaveMsMax = أقصى من أول التشغيل؛ SaveMsMaxWindow = أقصى من آخر تقرير، الـ reporter بيصفّره)
        public static long Saves, SaveMsTotal, SaveMsMax, SaveMsMaxWindow;
        // مرآة القاعدة (طابور الخلفية)
        public static long DbJobsQueued, DbJobsDone, DbJobsFailed;
        // دورة ThreadPlayer (بالميكروثانية) — TickUsMaxWindow بيتصفّر مع كل تقرير
        public static long Ticks, TickUsTotal, TickUsMax, TickUsMaxWindow;

        public static void PacketIn(int bytes) { Interlocked.Increment(ref PacketsIn); Interlocked.Add(ref BytesIn, bytes); }
        public static void PacketOut(int bytes) { Interlocked.Increment(ref PacketsOut); Interlocked.Add(ref BytesOut, bytes); }

        public static void SaveDone(long ms)
        {
            Interlocked.Increment(ref Saves);
            Interlocked.Add(ref SaveMsTotal, ms);
            RaiseMax(ref SaveMsMax, ms);
            RaiseMax(ref SaveMsMaxWindow, ms);
        }

        public static void TickDone(long microseconds)
        {
            Interlocked.Increment(ref Ticks);
            Interlocked.Add(ref TickUsTotal, microseconds);
            RaiseMax(ref TickUsMax, microseconds);
            RaiseMax(ref TickUsMaxWindow, microseconds);
        }

        /// <summary>يرجّع أقصى قيمة من آخر نداء ويصفّرها (للـ reporter).</summary>
        public static long TakeSaveMaxWindow() { return Interlocked.Exchange(ref SaveMsMaxWindow, 0); }
        public static long TakeTickMaxWindow() { return Interlocked.Exchange(ref TickUsMaxWindow, 0); }

        public static void DbQueued() { Interlocked.Increment(ref DbJobsQueued); }
        public static void DbDone(bool ok) { if (ok) Interlocked.Increment(ref DbJobsDone); else Interlocked.Increment(ref DbJobsFailed); }

        private static void RaiseMax(ref long field, long value)
        {
            long cur;
            while (value > (cur = Interlocked.Read(ref field)))
                if (Interlocked.CompareExchange(ref field, value, cur) == cur) break;
        }
    }
}
