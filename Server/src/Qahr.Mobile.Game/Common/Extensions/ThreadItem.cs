using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Qahr.Mobile;

namespace System.ThreadGroup
{
    public class ThreadItem
    {
        private int Max_ProcessInterval;
        private Action Event;
        private string Name;
        public Action<int> EventSleep;
        private Thread _thread;
        // [net10] volatile + wake event replace Thread.Abort (not supported on .NET Core/.NET 5+).
        private volatile bool Alive = false;
        private readonly ManualResetEvent _stopSignal = new ManualResetEvent(false);
        private const int StopJoinTimeoutMs = 5000;

        public ThreadItem(int interval, string MutexName, Action _Event, Action<int> Sleep = null)
        {
            _thread = new Thread(new ThreadStart(ThreadProc));
            Max_ProcessInterval = interval;
            Name = MutexName;
            // [perf M3] removed an unused named kernel Mutex that was created per ThreadItem but never acquired.
            Event = _Event;
            EventSleep = Sleep;
        }

        public void Open()
        {
            if (Alive == false)
            {
                Alive = true;
                _thread.Start();
            }
        }

        public void ThreadProc()
        {

            while (Alive)
            {
                try
                {
                    if (!OnProcess())
                    {
                        Close();
                        break;
                    }
                }
                catch { }
            }
        }

        public void Close()
        {
            if (Alive)
            {
                Alive = false;
                // [net10] cooperative stop instead of _thread.Abort(): the loop checks Alive, and the
                // event wakes the thread if it is waiting out its interval. Join only from another thread.
                _stopSignal.Set();
                if (_thread != Thread.CurrentThread && _thread.IsAlive)
                    _thread.Join(StopJoinTimeoutMs);
                Qahr.Mobile.MyConsole.WriteLine("[WARNING] Thread " + Name + " has been aported .", ConsoleColor.DarkYellow);
            }
        }
        protected bool OnProcess()
        {

            try
            {
                DateTime clockStart = DateTime.Now;
                try
                {
                    if (Event != null)
                        Event.Invoke();
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
                finally
                {
                    DateTime clocknow = DateTime.Now;
                    int next_stamp = Max_ProcessInterval - (int)(clocknow - clockStart).TotalMilliseconds;

                    if (next_stamp > 0 && next_stamp <= Max_ProcessInterval)
                    {

                        if (EventSleep != null)
                        {
                            EventSleep.Invoke(next_stamp);
                        }
                        else
                        {
                            if (next_stamp > 10000)
                                Qahr.Mobile.MyConsole.WriteLine("[WARNING] Thread " + Name + " is gonna sleep for " + (next_stamp / 1000) + " secoonds .", ConsoleColor.DarkYellow);

                            // [net10] was Thread.Sleep(next_stamp); waits the same time but wakes at once on Close().
                            _stopSignal.WaitOne(next_stamp);
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            return true;
        }
    }
}
