// بديل في السورس لـ System.Threading.StaticPool اللي في SystemExtensions.dll (نفس السلوك بالظبط).
//
// المشكلة اللي اتقاست (2026-10-13، لاعب واحد أونلاين): StaticPool القديم بيعمل 96 ثريد (32 + 64)، كل واحد
// بيعمل Sleep(1) ويصحى يبص في الطابور حتى لو فاضي => ~72,000 context switch/ثانية و ~91% من الـ CPU في الكيرنل
// (حوالي كور كامل على الفاضي).
//
// هنا: نفس التصميم (subscriptions + ثريد propagation واحد كل 1ms بيحط اللي جه وقته في طابور + N worker)
// بس الـ workers بتستنى على SemaphoreSlim لحد ما يجي شغل فعلًا، بدل Sleep(1) polling.
// نفس الدلالات: اللي اتعمل Subscribe بيتنفّذ فورًا أول مرة (LazyDelegate بعد المدة)، Repeat بيتعاد بعد
// period من نهاية التنفيذ، الـ subscription مايدخلش الطابور تاني وهو لسه شغال (Enqueued)، Dispose بيوقفه،
// الأولوية بتتغير أثناء التنفيذ، والـ int اللي بيتبعت للـ callback = Time32.Now.Value.
//
// ServerConfig.ini [Performance] WakePool=1 (الافتراضي). WakePool=0 = الـ workers ترجع لـ Sleep(1) polling زي القديم.
using System;
using System.Collections.Generic;
using System.Threading;

namespace Qahr.Mobile.Threading4K
{
    public class PoolRule
    {
        internal Action<int> Action;
        internal int Period;
        internal bool Repeat;
        internal ThreadPriority Priority;
        public PoolRule(Action<int> action, int period, ThreadPriority priority = ThreadPriority.Normal)
        {
            Action = action; Period = period; Repeat = true; Priority = priority;
        }
    }

    public class PoolRule<T>
    {
        internal Action<T, int> Action;
        internal int Period;
        internal bool Repeat;
        internal ThreadPriority Priority;
        public PoolRule(Action<T, int> action, int period, ThreadPriority priority = ThreadPriority.Normal)
        {
            Action = action; Period = period; Repeat = true; Priority = priority;
        }
    }

    public class PoolLazy : PoolRule
    {
        public PoolLazy(Action<int> action, int dueTime, ThreadPriority priority = ThreadPriority.Normal)
            : base(action, dueTime, priority) { Repeat = false; }
    }

    public class PoolLazy<T> : PoolRule<T>
    {
        public PoolLazy(Action<T, int> action, int dueTime, ThreadPriority priority = ThreadPriority.Normal)
            : base(action, dueTime, priority) { Repeat = false; }
    }

    internal abstract class PoolSubscription : IDisposable
    {
        private static int counter = int.MinValue;
        internal volatile bool Viable = true;
        internal volatile bool Enqueued;
        internal Time32 NextInvocation;
        private readonly int hash;

        protected PoolSubscription()
        {
            hash = Interlocked.Increment(ref counter);
            Set(0);
        }
        internal bool Due { get { return Time32.Now > NextInvocation; } }
        internal void Set(int dueTime) { NextInvocation = Time32.Now.AddMilliseconds(dueTime); }
        internal abstract void Invoke();
        internal abstract ThreadPriority GetPriority();
        protected abstract void CleanUp();
        public void Dispose() { Viable = false; CleanUp(); }
        public override int GetHashCode() { return hash; }
    }

    internal sealed class PoolSubscription<T> : PoolSubscription
    {
        private PoolRule<T> rule;
        private T param;
        private readonly ThreadPriority priority;
        public PoolSubscription(PoolRule<T> rule, T param) { this.rule = rule; this.param = param; priority = rule.Priority; }
        internal override void Invoke()
        {
            var r = rule;
            if (r == null) return;
            // finally: لو الـ callback رمى exception، القديم كان بيسيبه due فيتعاد كل 1ms للأبد (loop أخطاء).
            // هنا بيتعامل زي النجاح: Lazy يتقفل، والمتكرر يستنى الـ period بتاعه. الـ exception لسه بيطلع للـ log.
            try
            {
                r.Action(param, Time32.Now.Value);
            }
            finally
            {
                if (rule != null)
                {
                    if (!r.Repeat) Dispose();
                    else Set(r.Period);
                }
            }
        }
        internal override ThreadPriority GetPriority() { return priority; }
        protected override void CleanUp() { rule = null; param = default(T); }
    }

    internal sealed class PoolSubscriptionPlain : PoolSubscription
    {
        private PoolRule rule;
        private readonly ThreadPriority priority;
        public PoolSubscriptionPlain(PoolRule rule) { this.rule = rule; priority = rule.Priority; }
        internal override void Invoke()
        {
            var r = rule;
            if (r == null) return;
            // finally: لو الـ callback رمى exception، القديم كان بيسيبه due فيتعاد كل 1ms للأبد (loop أخطاء).
            // هنا بيتعامل زي النجاح: Lazy يتقفل، والمتكرر يستنى الـ period بتاعه. الـ exception لسه بيطلع للـ log.
            try
            {
                r.Action(Time32.Now.Value);
            }
            finally
            {
                if (rule != null)
                {
                    if (!r.Repeat) Dispose();
                    else Set(r.Period);
                }
            }
        }
        internal override ThreadPriority GetPriority() { return priority; }
        protected override void CleanUp() { rule = null; }
    }

    public class WakePool
    {
        /// <summary>true = الـ workers بتستنى على semaphore (الجديد). false = Sleep(1) polling زي StaticPool القديم.</summary>
        public static bool UseWake = true;
        /// <summary>true = kernel Semaphore (بينام على طول من غير spin). false = SemaphoreSlim (بيعمل spin شوية قبل ما ينام).</summary>
        public static bool KernelWait = true;
        /// <summary>true = ثريد التايمر ينام لحد أقرب تايمر (ويتصحّى لو اتضاف اشتراك أو تايمر قصير خلص) بدل ما يصحى كل 1ms.</summary>
        public static bool SmartTimer = true;

        private readonly object subsRoot = new object();
        private readonly object queueRoot = new object();
        private readonly Dictionary<int, PoolSubscription> subscribers = new Dictionary<int, PoolSubscription>();
        private readonly Queue<PoolSubscription> queue = new Queue<PoolSubscription>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0, int.MaxValue);
        private readonly Semaphore kernelSignal = new Semaphore(0, int.MaxValue);
        private readonly int threadCount;
        private readonly string name;
        private volatile bool running;
        private readonly AutoResetEvent timerWake = new AutoResetEvent(false);
        private int sleepUntil; // Time32.Value اللي ثريد التايمر هيصحى عنده
        private int idle, inUse;

        public WakePool(int threads = 32, string name = "WakePool")
        {
            threadCount = Math.Max(1, threads);
            this.name = name;
        }

        public int Threads { get { return threadCount; } }
        public int IdleThreads { get { return idle; } }
        public int InUseThreads { get { return inUse; } }
        public int Treshold { get { lock (queueRoot) return queue.Count; } }

        public WakePool Run()
        {
            running = true;
            for (int i = 0; i < threadCount; i++)
            {
                Interlocked.Increment(ref idle);
                var t = new Thread(Work, 1048576) { Name = name + "#" + i };
                t.Start();
            }
            new Thread(Propagate) { Name = name + "-timer" }.Start();
            return this;
        }

        public IDisposable Subscribe(PoolRule rule)
        {
            var sub = new PoolSubscriptionPlain(rule);
            if (rule is PoolLazy) sub.Set(rule.Period);
            lock (subsRoot) subscribers[sub.GetHashCode()] = sub;
            if (SmartTimer) timerWake.Set();
            return sub;
        }

        public IDisposable Subscribe<T>(PoolRule<T> rule, T param)
        {
            var sub = new PoolSubscription<T>(rule, param);
            if (rule is PoolLazy<T>) sub.Set(rule.Period);
            lock (subsRoot) subscribers[sub.GetHashCode()] = sub;
            if (SmartTimer) timerWake.Set();
            return sub;
        }

        private void Propagate()
        {
            var due = new List<PoolSubscription>();
            var dead = new List<int>();
            while (running)
            {
                int now = Time32.Now.Value;
                int minRemain = 1000;
                // وإحنا بنلف: أي تايمر يخلص دلوقتي لازم يضرب الـ event (مايضيعش بين اللفة والـ Wait)
                Volatile.Write(ref sleepUntil, now + 1000000);
                lock (subsRoot)
                {
                    foreach (var s in subscribers.Values)
                    {
                        if (s.Viable)
                        {
                            if (!s.Enqueued)
                            {
                                if (s.Due)
                                {
                                    s.Enqueued = true;
                                    due.Add(s);
                                }
                                else
                                {
                                    // Due = Now > Next، يعني أول لحظة يبقى due فيها = Next + 1
                                    int remain = s.NextInvocation.Value - now + 1;
                                    if (remain < minRemain) minRemain = remain;
                                }
                            }
                        }
                        else dead.Add(s.GetHashCode());
                    }
                    for (int i = 0; i < dead.Count; i++)
                        subscribers.Remove(dead[i]);
                }
                dead.Clear();
                if (due.Count != 0)
                {
                    lock (queueRoot)
                        for (int i = 0; i < due.Count; i++)
                            queue.Enqueue(due[i]);
                    if (UseWake)
                    {
                        if (KernelWait) kernelSignal.Release(due.Count);
                        else signal.Release(due.Count);
                    }
                    minRemain = 1;
                    due.Clear();
                }
                if (SmartTimer)
                {
                    int wait = Math.Max(1, minRemain);
                    Volatile.Write(ref sleepUntil, Time32.Now.Value + wait);
                    timerWake.WaitOne(wait);
                }
                else
                    Thread.Sleep(1);
            }
        }

        private bool TryDequeue(out PoolSubscription sub)
        {
            lock (queueRoot)
            {
                if (queue.Count != 0) { sub = queue.Dequeue(); return true; }
            }
            sub = null;
            return false;
        }

        private void Work()
        {
            Thread current = Thread.CurrentThread;
            while (running)
            {
                if (UseWake)
                {
                    // مهلة 1s بس كشبكة أمان؛ في العادي بيصحى أول ما يتحط شغل
                    if (KernelWait) kernelSignal.WaitOne(1000);
                    else signal.Wait(100);
                }
                else
                    Thread.Sleep(1);

                PoolSubscription sub;
                if (!TryDequeue(out sub))
                    continue;
                if (!sub.Viable)
                {
                    lock (subsRoot) subscribers.Remove(sub.GetHashCode());
                    continue;
                }
                Interlocked.Decrement(ref idle);
                Interlocked.Increment(ref inUse);
                current.Priority = sub.GetPriority();
                try
                {
                    sub.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    sub.Enqueued = false;
                    // تايمر متكرر خلص وميعاده الجاي قبل ما ثريد التايمر يصحى => صحّيه عشان مايتأخرش
                    if (SmartTimer && sub.Viable && sub.NextInvocation.Value - Volatile.Read(ref sleepUntil) < 0)
                        timerWake.Set();
                    current.Priority = ThreadPriority.Normal;
                    Interlocked.Decrement(ref inUse);
                    Interlocked.Increment(ref idle);
                }
            }
        }

        public void Clear()
        {
            lock (queueRoot) queue.Clear();
        }

        public override string ToString()
        {
            int subs; lock (subsRoot) subs = subscribers.Count;
            return string.Format("{0} waiting exec, {1} subscriptions, {2} threads: {3} in use, {4} idle", Treshold, subs, threadCount, inUse, idle);
        }
    }
}
