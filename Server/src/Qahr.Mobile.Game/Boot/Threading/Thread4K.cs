using Qahr.Mobile.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Generic;
using System.Threading.Tasks;
using Qahr.Mobile.Threading4K;

namespace Qahr.Mobile.ServerSockets
{
    class Thread4K
    {
        public PoolRule<ServerSockets.SecuritySocket> ConnectionReceive;
        public WakePool ReceivePool;
        public static WakePool GenericThreadPool;
        public PoolRule<GameClient> MainTimer;

        public Thread4K()
        {
            // [perf] WakePool بدل StaticPool (SystemExtensions.dll): نفس السلوك بس الثريدات مش بتصحى كل 1ms على الفاضي.
            // [Performance] WakePool=0 يرجّع الـ Sleep(1) polling القديم من غير build.
            WakePool.UseWake = Qahr.Mobile.SharedConfig.Int("Performance", "WakePool", 1) != 0;
            WakePool.KernelWait = Qahr.Mobile.SharedConfig.Int("Performance", "KernelWait", 1) != 0; // 0 = SemaphoreSlim (بيعمل spin)
            WakePool.SmartTimer = WakePool.UseWake && Qahr.Mobile.SharedConfig.Int("Performance", "SmartTimer", 1) != 0; // 0 = ثريد التايمر يصحى كل 1ms
            GenericThreadPool = new WakePool(32, "GenericPool").Run();
            ReceivePool = new WakePool(64, "ReceivePool").Run();
            MyConsole.WriteLine("[Threads] WakePool=" + (WakePool.UseWake ? "1 (event-driven" + (WakePool.KernelWait ? ", kernel wait" : ", slim wait") + (WakePool.SmartTimer ? ", smart timer)" : ")") : "0 (Sleep(1) polling)"), ConsoleColor.White);
            ConnectionReceive = new PoolRule<ServerSockets.SecuritySocket>(connectionReceive, 1, ThreadPriority.Highest);
            MainTimer = new PoolRule<GameClient>(MainCallBack, 250);
        }
        #region Thread
        public static void MainCallBack(GameClient client, int time)
        {
            try
            {

            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        public bool Register(GameClient client)
        {
            if (client.TimerSubscriptions == null)
            {
                client.TimerSubscriptions = new IDisposable[]
                {
                   Subscribe(MainTimer, client)
                };
                return true;
            }
            return false;
        }
        public void Unregister(GameClient client)
        {
            lock (client.TimerSyncRoot)
            {
                if (client.TimerSubscriptions != null)
                {
                    foreach (var Now in client.TimerSubscriptions)
                        Now.Dispose();
                    client.TimerSubscriptions = null;
                }
            }
        }
        public static void connectionReceive(ServerSockets.SecuritySocket wrapper, int time)
        {
            try
            {
                if (wrapper.ReceiveBuffer())
                {
                    wrapper.HandlerBuffer();
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        #endregion
        #region Funcs
        public static void Execute(Action<int> action, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe(new PoolLazy(action, timeOut, priority));
        }
        public static void Execute<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe<T>(new PoolLazy<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe(Action<int> action, int period = 1, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe(new PoolRule(action, period, priority));
        }
        public static IDisposable Subscribe<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe<T>(new PoolRule<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe<T>(PoolRule<T> rule, T param, WakePool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(PoolRule<T> rule, T param)
        {
            return GenericThreadPool.Subscribe<T>(rule, param);
        }
        #endregion
    }
}
