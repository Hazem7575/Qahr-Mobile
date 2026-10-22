// ============================================================================
//  MirrorQueue - طابور خلفية لمرآة MySQL (PlayerStorage / PlayerBinaryStorage /
//  SocialStorage) عشان الـ UPSERT ما يتعملش على ثريد اللعب.
//
//  ServerConfig.ini [Storage]:
//    AsyncMirror=1          1 = الطابور ده | 0 (أو مش موجود) = السلوك المتزامن القديم بالظبط
//    AsyncMirrorWorkers=2   عدد ثريدات التنفيذ (كل مفتاح ثابت على ثريد واحد -> ترتيبه محفوظ)
//    AsyncMirrorMaxQueue=20000  أقصى عدد عمليات منتظرة (بعده: حفظ لمفتاح جديد بيتساب مع تحذير)
//    AsyncMirrorFlushSec=8  مهلة تفريغ الطابور عند الإغلاق
//
//  التصميم:
//    * المنادي (ثريد اللعب) بيعمل Submit(key, tag, supersede, snapshot, what):
//      - snapshot() بيتنفّذ على ثريد المنادي (قراءة الملف اللي لسه اتكتب + تحليله، نفس
//        الوصول للملفات القديم بالظبط -> مفيش أي خطر sharing/قراءة نص ملف على ملفات
//        الحقيقة) وبيرجّع Action فيها شغل القاعدة بس، اللي بيتنفّذ في الخلفية.
//      - snapshot + الإضافة للطابور جوّه قفل مخطّط (stripe) حسب المفتاح، فترتيب الصور في
//        الطابور = ترتيب قراءتها من الملف (أحدث قراءة = أحدث ملف) حتى لو ثريدين حفظوا نفس اللاعب.
//    * coalescing لكل مفتاح (players:UID, items:UID, guild:ID ...): لو آخر عملية منتظرة لنفس
//      المفتاح ليها نفس الـ tag (مثلًا "save") بتتبدّل بالأحدث في مكانها -> تتنفّذ مرة واحدة.
//    * الترتيب لنفس المفتاح محفوظ: العمليات المختلفة بتتحط ورا بعض في قائمة المفتاح وبتتنفّذ
//      بالترتيب على نفس الثريد. supersede=true (الحذف) بيلغي أي حاجة منتظرة قبله لنفس المفتاح
//      (Save ثم Delete = حذف؛ Delete ثم Save = الاتنين بالترتيب -> آخر طلب يكسب).
//      المفتاح اللي بيتنفّذ حاليًا بيتشال من الخريطة قبل التنفيذ، فأي طلب جديد بيتحط من جديد
//      وراه في نفس الثريد.
//    * الفشل: PerfMetrics.DbDone(false) + تحذير أصفر بنفس رسالة الكود القديم، ومفيش crash.
//    * Flush(ms) من مسار الإغلاق (Program.ProcessConsoleEvent).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Threading;

namespace Qahr.Mobile.Database.Storage
{
    public static class MirrorQueue
    {
        public static bool Async { get; private set; }
        private static int _maxPending = 20000;
        private static int _flushSec = 8;

        private sealed class Op
        {
            public string Tag;
            public Action Work;
            public string What;
        }

        private sealed class Shard
        {
            public readonly object Sync = new object();
            public readonly Queue<string> Order = new Queue<string>();
            public readonly Dictionary<string, List<Op>> Pending = new Dictionary<string, List<Op>>(StringComparer.Ordinal);
            public bool Busy;
            public string RunningKey;   // المفتاح اللي الـ worker بينفّذه دلوقتي (للـ IsDirty)
        }

        // مفاتيح آخر نسخة ليها في القاعدة مش مضمونة (حفظ فشل / اتشال من الطابور / الـ snapshot وقع).
        // بتتشال أول ما حفظ لنفس المفتاح ينجح. القراءة من القاعدة لازم ترجع للملف لو المفتاح هنا.
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, byte> _stale =
            new System.Collections.Concurrent.ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        private static void MarkStale(string key) { if (key != null) _stale[key] = 1; }
        private static void MarkFresh(string key) { byte b; if (key != null) _stale.TryRemove(key, out b); }

        /// <summary>
        /// true = القاعدة ممكن تكون متأخرة عن الملف للمفتاح ده (فيه حفظ لسه في الطابور أو شغال دلوقتي، أو آخر حفظ
        /// فشل). اللي بيقرا من القاعدة لازم يقرا من الملف في الحالة دي.
        /// </summary>
        public static bool IsDirty(string key)
        {
            if (_stale.ContainsKey(key)) return true;
            Shard[] shards = _shards;
            if (!Async || shards == null) return false;
            Shard sh = shards[Hash(key) % shards.Length];
            lock (sh.Sync)
                return sh.Pending.ContainsKey(key) || string.Equals(sh.RunningKey, key, StringComparison.Ordinal);
        }

        private static Shard[] _shards;
        private static int _pendingOps;
        private static long _dropped;
        private static long _lastWarnTicks;
        private static readonly object[] _stripes = CreateStripes(64);
        private static readonly object _initLock = new object();

        private static object[] CreateStripes(int n)
        {
            var a = new object[n];
            for (int i = 0; i < n; i++) a[i] = new object();
            return a;
        }

        /// <summary>بيقرا [Storage] AsyncMirror* ويشغّل الثريدات. بيتنادى من PlayerStorage.Init.</summary>
        public static void Init()
        {
            lock (_initLock)
            {
                if (_shards != null) return;
                bool on = false;
                int workers = 2;
                try
                {
                    on = Qahr.Mobile.SharedConfig.Int("Storage", "AsyncMirror", 0) == 1;
                    workers = Qahr.Mobile.SharedConfig.Int("Storage", "AsyncMirrorWorkers", 2);
                    _maxPending = Qahr.Mobile.SharedConfig.Int("Storage", "AsyncMirrorMaxQueue", 20000);
                    _flushSec = Qahr.Mobile.SharedConfig.Int("Storage", "AsyncMirrorFlushSec", 8);
                }
                catch { on = false; }
                if (!on) { Async = false; return; }

                if (workers < 1) workers = 1;
                if (workers > 8) workers = 8;
                if (_maxPending < 100) _maxPending = 100;
                if (_flushSec < 0) _flushSec = 0;

                var shards = new Shard[workers];
                for (int i = 0; i < workers; i++)
                {
                    var sh = new Shard();
                    shards[i] = sh;
                    var t = new Thread(() => WorkerLoop(sh));
                    t.IsBackground = true;
                    t.Name = "StorageMirror-" + i;
                    t.Start();
                }
                _shards = shards;
                Async = true;
                try { MyConsole.WriteLine("[Storage] Async DB mirror ON (" + workers + " workers, max queue " + _maxPending + ")", ConsoleColor.Cyan); } catch { }
            }
        }

        public static int PendingCount { get { return Volatile.Read(ref _pendingOps); } }

        private static int Hash(string key)
        {
            // hash ثابت (مش string.GetHashCode) - مش ضروري بس أوضح
            unchecked
            {
                int h = 17;
                for (int i = 0; i < key.Length; i++) h = h * 31 + key[i];
                return h & 0x7fffffff;
            }
        }

        /// <summary>
        /// snapshot بيتنفّذ دلوقتي على ثريد المنادي (قراءة الملف) ويرجّع شغل القاعدة (أو null = مفيش حاجة).
        /// AsyncMirror=0: snapshot + الشغل بيتنفّذوا متزامن زي الكود القديم بالظبط (try/catch واحد ونفس الرسالة).
        /// </summary>
        public static void Submit(string key, string tag, bool supersede, Func<Action> snapshot, string what)
        {
            Shard[] shards = _shards;
            if (!Async || shards == null)
            {
                try
                {
                    Action w = snapshot();
                    if (w != null) { w(); MarkFresh(key); }
                }
                catch (Exception e)
                {
                    MarkStale(key);
                    try { MyConsole.WriteLine("[Storage] " + what + ": " + e.Message, ConsoleColor.Yellow); } catch { }
                }
                return;
            }

            int h = Hash(key);
            lock (_stripes[h % _stripes.Length])
            {
                Action work;
                try { work = snapshot(); }
                catch (Exception e)
                {
                    MarkStale(key);
                    PerfMetrics.DbDone(false);
                    try { MyConsole.WriteLine("[Storage] " + what + ": " + e.Message, ConsoleColor.Yellow); } catch { }
                    return;
                }
                if (work == null) return;
                Enqueue(shards[h % shards.Length], key, new Op { Tag = tag, Work = work, What = what }, supersede);
            }
        }

        private static void Enqueue(Shard sh, string key, Op op, bool supersede)
        {
            bool dropped = false;
            lock (sh.Sync)
            {
                List<Op> list;
                if (sh.Pending.TryGetValue(key, out list))
                {
                    if (supersede)
                    {
                        Interlocked.Add(ref _pendingOps, 1 - list.Count);
                        list.Clear();
                        list.Add(op);
                    }
                    else if (list.Count > 0 && list[list.Count - 1].Tag == op.Tag)
                    {
                        list[list.Count - 1] = op;            // coalesce: الأحدث مكان القديم
                    }
                    else
                    {
                        list.Add(op);
                        Interlocked.Increment(ref _pendingOps);
                    }
                }
                else if (!supersede && Volatile.Read(ref _pendingOps) >= _maxPending)
                {
                    dropped = true;                           // الحذف عمره ما بيتساب
                }
                else
                {
                    list = new List<Op>(2);
                    list.Add(op);
                    sh.Pending[key] = list;
                    sh.Order.Enqueue(key);
                    Interlocked.Increment(ref _pendingOps);
                    Monitor.PulseAll(sh.Sync);
                }
            }

            PerfMetrics.DbQueued();
            if (dropped)
            {
                MarkStale(key);
                PerfMetrics.DbDone(false);
                long n = Interlocked.Increment(ref _dropped);
                long now = DateTime.UtcNow.Ticks;
                long last = Interlocked.Read(ref _lastWarnTicks);
                if (now - last > TimeSpan.TicksPerSecond * 30 && Interlocked.CompareExchange(ref _lastWarnTicks, now, last) == last)
                {
                    try { MyConsole.WriteLine("[Storage] DB mirror queue full (" + _maxPending + "), dropped " + n + " mirror update(s) so far - files are saved, the DB catches up on the next save. Is MySQL slow/down?", ConsoleColor.Yellow); } catch { }
                }
            }
        }

        private static void WorkerLoop(Shard sh)
        {
            while (true)
            {
                List<Op> ops;
                string key;
                lock (sh.Sync)
                {
                    while (sh.Order.Count == 0)
                        Monitor.Wait(sh.Sync);
                    key = sh.Order.Dequeue();
                    if (!sh.Pending.TryGetValue(key, out ops)) continue;
                    sh.Pending.Remove(key);                   // أي طلب جديد لنفس المفتاح يتحط من جديد وراه
                    sh.Busy = true;
                    sh.RunningKey = key;
                }

                try
                {
                    for (int i = 0; i < ops.Count; i++)
                    {
                        Op op = ops[i];
                        try
                        {
                            op.Work();
                            MarkFresh(key);
                            PerfMetrics.DbDone(true);
                        }
                        catch (Exception e)
                        {
                            MarkStale(key);
                            PerfMetrics.DbDone(false);
                            try { MyConsole.WriteLine("[Storage] " + op.What + ": " + e.Message, ConsoleColor.Yellow); } catch { }
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _pendingOps);
                        }
                    }
                }
                catch { }
                finally
                {
                    lock (sh.Sync)
                    {
                        sh.Busy = false;
                        sh.RunningKey = null;
                        Monitor.PulseAll(sh.Sync);
                    }
                }
            }
        }

        /// <summary>عند الإغلاق: يستنى لحد ما الطابور يفضى (أو المهلة [Storage] AsyncMirrorFlushSec تخلص).</summary>
        public static void Flush()
        {
            Flush(_flushSec * 1000);
        }

        public static bool Flush(int timeoutMs)
        {
            Shard[] shards = _shards;
            if (!Async || shards == null) return true;
            int start = Environment.TickCount;
            bool ok = true;
            try
            {
                foreach (var sh in shards)
                {
                    lock (sh.Sync)
                    {
                        while (sh.Order.Count > 0 || sh.Busy)
                        {
                            int left = timeoutMs - unchecked(Environment.TickCount - start);
                            if (left <= 0) { ok = false; break; }
                            Monitor.Wait(sh.Sync, left);
                        }
                    }
                    if (!ok) break;
                }
                if (ok) MyConsole.WriteLine("[Storage] DB mirror queue flushed.", ConsoleColor.White);
                else MyConsole.WriteLine("[Storage] DB mirror flush timed out, " + PendingCount + " update(s) not mirrored (files are saved).", ConsoleColor.Yellow);
            }
            catch { }
            return ok;
        }
    }
}
