// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Threading.Generic;
using System.Collections.Generic;

namespace System.Threading
{
    public class StandalonePool : IDisposable
    {
        public const int SleepTime = 1;
        internal object object_0;
        internal object object_1;
        internal Dictionary<int, Class2> dictionary_0;
        internal Queue<Class2> queue_0;
        internal List<Thread> list_0;
        protected internal Thread propagationThread;
        internal volatile bool bool_0;
        internal volatile bool bool_1;
        internal int int_0;
        internal int int_1;
        internal int int_2;
        internal int int_3;
        internal int int_4;

        public int Threads
        {
            get
            {
                return this.int_0;
            }
        }

        public int IdleThreads
        {
            get
            {
                return this.int_1;
            }
        }

        public int InUseThreads
        {
            get
            {
                return this.int_2;
            }
        }

        public int Treshold
        {
            get
            {
                return this.queue_0.Count;
            }
        }

        public StandalonePool(int minimumPoolSize = 6, int maximumPoolSize = 32)
        {
            this.bool_1 = false;
            this.object_1 = new object();
            this.object_0 = new object();
            this.dictionary_0 = new Dictionary<int, Class2>();
            this.queue_0 = new Queue<Class2>();
            this.list_0 = new List<Thread>();
            this.int_3 = minimumPoolSize;
            this.int_4 = maximumPoolSize;
        }

        ~StandalonePool()
        {
            this.method_2(false);
        }

        void IDisposable.Dispose()
        {
            this.method_2(true);
        }

        internal void method_0()
        {
            if (this.bool_1)
                return;
            Interlocked.Increment(ref this.int_0);
            Interlocked.Increment(ref this.int_1);
            Thread thread = new Thread(new ThreadStart(this.method_3));
            this.list_0.Add(thread);
            thread.Priority = ThreadPriority.Normal;
            thread.IsBackground = false;
            thread.Start();
        }

        internal void method_1()
        {
            if (this.bool_1)
                return;
            foreach (Thread thread in this.list_0)
            {
                if (!thread.IsBackground)
                {
                    thread.IsBackground = true;
                    Interlocked.Decrement(ref this.int_0);
                    this.list_0.Remove(thread);
                    break;
                }
            }
        }

        internal void method_2(bool bool_2)
        {
            if (this.bool_1)
                return;
            this.bool_1 = true;
            this.bool_0 = false;
            if (bool_2)
            {
                // [net10] was thread.Abort() on every worker (not supported on .NET 5+). bool_0 = false above
                // already makes method_3/method_7 leave their loops (they only Sleep(1)), so wait for them here
                // before the collections below are nulled. Never join the calling thread itself.
                Thread current = Thread.CurrentThread;
                foreach (Thread thread in this.list_0)
                {
                    if (thread != current && thread.IsAlive)
                        thread.Join(5000);
                }
                if (this.propagationThread != null && this.propagationThread != current && this.propagationThread.IsAlive)
                    this.propagationThread.Join(5000);
            }
            this.dictionary_0.Clear();
            this.dictionary_0 = (Dictionary<int, Class2>)null;
            this.queue_0 = (Queue<Class2>)null;
            this.list_0 = (List<Thread>)null;
        }

        internal void method_3()
        {
            Thread currentThread = Thread.CurrentThread;
            while (this.bool_0 && !currentThread.IsBackground)
            {
                Thread.Sleep(1);
                // [net10] per-iteration guard: an exception escaping a thread proc kills the whole process on
                // .NET 5+ (legacyUnhandledExceptionPolicy is gone). Log and keep the worker alive instead.
                try
                {
                Class2 class2_0;
                if (this.method_4(out class2_0))
                {
                    if (class2_0.bool_0)
                    {
                        Interlocked.Decrement(ref this.int_1);
                        Interlocked.Increment(ref this.int_2);
                        try
                        {
                            // [net10] moved inside the try: vmethod_3() dereferences timerRule_0, which a
                            // concurrent Dispose() nulls -> NullReferenceException used to kill this worker.
                            currentThread.Priority = class2_0.vmethod_3();
                            class2_0.vmethod_0();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine((object)ex);
                        }
                        finally
                        {
                            class2_0.bool_1 = false;
                        }
                        currentThread.Priority = ThreadPriority.Normal;
                        Interlocked.Decrement(ref this.int_2);
                        Interlocked.Increment(ref this.int_1);
                    }
                    else
                        this.method_5(class2_0.GetHashCode());
                }
                }
                catch (Exception ex)
                {
                    if (this.bool_0)
                        Console.WriteLine((object)ex);
                }
            }
            Interlocked.Decrement(ref this.int_1);
        }

        internal bool method_4(out Class2 class2_0)
        {
            class2_0 = (Class2)null;
            lock (this.object_0)
            {
                if (this.queue_0.Count != 0)
                {
                    Class2 local_0 = this.queue_0.Dequeue();
                    class2_0 = local_0;
                }
            }
            return class2_0 != null;
        }

        internal void method_5(int int_5)
        {
            lock (this.object_1)
                this.dictionary_0.Remove(int_5);
        }

        public IDisposable Subscribe(TimerRule instruction)
        {
            Class2 class2 = (Class2)null;
            lock (this.object_1)
            {
                class2 = (Class2)new Class4(instruction);
                if (instruction is LazyDelegate)
                    class2.method_1(instruction.int_0);
                this.dictionary_0[class2.GetHashCode()] = class2;
            }
            return (IDisposable)class2;
        }

        public IDisposable Subscribe<T>(TimerRule<T> instruction, T param)
        {
            Class2 class2 = (Class2)null;
            lock (this.object_1)
            {
                class2 = (Class2)new Class3<T>(instruction, param);
                if (instruction is LazyDelegate<T>)
                    class2.method_1(instruction.int_0);
                this.dictionary_0[class2.GetHashCode()] = class2;
            }
            return (IDisposable)class2;
        }

        public StandalonePool Run()
        {
            this.bool_0 = true;
            for (int index = 0; index < this.int_3; ++index)
                this.method_0();
            this.propagationThread = new Thread(new ThreadStart(this.method_7));
            this.propagationThread.Start();
            return this;
        }

        internal void method_6()
        {
            int num1 = this.int_2;
            int num2 = this.int_0;
            if ((num1 == num2 || this.queue_0.Count / 10 >= num2) && num2 < this.int_4)
                this.method_0();
            if (num1 > num2 / 4 || num2 <= this.int_3)
                return;
            this.method_1();
        }

        private void method_7()
        {
            while (this.bool_0)
            {
                // [net10] per-iteration guard (see method_3): log and keep propagating instead of dying.
                try
                {
                Queue<Class2> queue1 = new Queue<Class2>();
                Queue<int> queue2 = new Queue<int>();
                lock (this.object_1)
                {
                    foreach (Class2 item_0 in this.dictionary_0.Values)
                    {
                        if (item_0.bool_0)
                        {
                            if (!item_0.bool_1 && item_0.method_0())
                            {
                                item_0.bool_1 = true;
                                queue1.Enqueue(item_0);
                            }
                        }
                        else
                            queue2.Enqueue(item_0.GetHashCode());
                    }
                    while (queue2.Count != 0)
                        this.dictionary_0.Remove(queue2.Dequeue());
                }
                if (queue1.Count != 0)
                {
                    lock (this.object_0)
                    {
                        while (queue1.Count != 0)
                            this.queue_0.Enqueue(queue1.Dequeue());
                    }
                }
                this.method_6();
                }
                catch (Exception ex)
                {
                    if (this.bool_0)
                        Console.WriteLine((object)ex);
                }
                Thread.Sleep(1);
            }
        }

        public void Clear()
        {
            lock (this.object_0)
                this.queue_0.Clear();
        }

        public override string ToString()
        {
            return string.Format("{0} waiting exec, {1} subscriptions, {2} threads: {3} in use, {4} idle", (object)this.queue_0.Count, (object)this.dictionary_0.Count, (object)this.int_0, (object)this.int_2, (object)this.int_1);
        }
    }
}