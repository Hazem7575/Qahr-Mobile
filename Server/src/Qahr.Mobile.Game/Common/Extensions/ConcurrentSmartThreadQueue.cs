using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;

namespace Qahr.Mobile
{
    public abstract class ConcurrentSmartThreadQueue<T>
    {
        public ConcurrentQueue<T> Queues;
        public System.ThreadGroup.ThreadItem thread;

        // Number of items currently being processed inside OnDequeue (never negative).
        private int _processing;

        // [perf] idle parking: the worker used to wake every `period` ms (5-50ms) even with an empty
        // queue. Now, when the queue is empty at a tick, it parks on _signal until Enqueue() wakes it,
        // then sleeps up to the next point of the SAME tick grid (lastTick + n*period), so every item
        // is still processed exactly when the old polling loop would have picked it up.
        private readonly System.Threading.AutoResetEvent _signal = new System.Threading.AutoResetEvent(false);
        private int _parked;
        private int _period;

        public ConcurrentSmartThreadQueue(int Processors)
        {
            Queues = new ConcurrentQueue<T>();
        }
        protected abstract void OnDequeue(T Value, int time);

        public bool Finish()
        {
            while (Queues.Count > 0)
            {
                System.Threading.Thread.Sleep(1);
            }
            System.Threading.Thread.Sleep(500);
            return true;
        }

        /// <summary>
        /// Waits until the queue is empty AND no item is currently being processed, or until the
        /// timeout elapses. Returns true if fully drained, false if it timed out. A negative
        /// timeout waits indefinitely. Safe to call during shutdown before the final save.
        /// </summary>
        public bool WaitUntilEmpty(int timeoutMs)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (Queues.Count > 0 || System.Threading.Interlocked.CompareExchange(ref _processing, 0, 0) > 0)
            {
                if (timeoutMs >= 0 && sw.ElapsedMilliseconds >= timeoutMs)
                    return false;
                System.Threading.Thread.Sleep(5);
            }
            return true;
        }
        public void Start(int period)
        {
            _period = period;
            thread = new System.ThreadGroup.ThreadItem(period, "ConquerThreadQueue", Work, IdleSleep);
            thread.Open();
        }

        public void Work()
        {
            try
            {
                DateTime timer = DateTime.Now;
                T obj;
                while (Queues.TryDequeue(out obj))
                {
                    System.Threading.Interlocked.Increment(ref _processing);
                    try
                    {
                        OnDequeue(obj, timer.AllMilliseconds());
                    }
                    finally
                    {
                        System.Threading.Interlocked.Decrement(ref _processing);
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        // Called by ThreadItem instead of its own sleep (only when there is time left in this period).
        private void IdleSleep(int next_stamp)
        {
            // Same wait as before: the rest of the current period -> we are now on the next tick.
            System.Threading.Thread.Sleep(next_stamp);
            if (_period <= 0 || !Queues.IsEmpty)
                return;

            long tick = System.Diagnostics.Stopwatch.GetTimestamp();
            System.Threading.Interlocked.Exchange(ref _parked, 1);
            try
            {
                // Re-check after publishing _parked so an Enqueue racing with us is never missed.
                while (Queues.IsEmpty)
                    _signal.WaitOne();
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref _parked, 0);
            }
            long elapsedMs = (System.Diagnostics.Stopwatch.GetTimestamp() - tick) * 1000 / System.Diagnostics.Stopwatch.Frequency;
            int wait = (int)((_period - (elapsedMs % _period)) % _period);
            if (wait > 0)
                System.Threading.Thread.Sleep(wait);
        }

        public virtual void Enqueue(T obj)
        {
            Queues.Enqueue(obj);
            if (System.Threading.Interlocked.CompareExchange(ref _parked, 0, 0) == 1)
                _signal.Set();
        }
        public int Count { get { return Queues.Count; } }
    }
}
