// [Lottery 2026-10-14] Start-up + timer for the lotteries (one hook line in MY System\Program.cs: LotteryModule.Start()).
// Every 2 s: Golden Tree state changes are pushed to everyone, new logins get the current state, pending prizes are
// delivered when the bag has room, stale flop sessions are dropped.
using System;
using System.Threading;

namespace Qahr.Mobile.Lottery
{
    public static class LotteryModule
    {
        static Timer timer;
        static int busy;

        public static void Start()
        {
            try
            {
                GlobalLotteryModule.EnsureLoaded();
                TurnoverLotteryModule.EnsureLoaded();
                timer = new Timer(_ => Tick(), null, 5000, 2000);
                int types = 0; foreach (var t in TurnoverLotteryModule.Types.Values) if (t.Disabled == null) types++;
                MyConsole.WriteLine("[Lottery] Golden Tree " + (GlobalLotteryModule.Enabled ? GlobalLotteryModule.Blooms.Count + " bloom(s)/day, " + GlobalLotteryModule.Conditions.Count + " client conditions" : "OFF " + GlobalLotteryModule.LoadError)
                    + " | Turnover " + (TurnoverLotteryModule.Enabled ? types + " usable type(s)" : "OFF " + TurnoverLotteryModule.LoadError), ConsoleColor.Cyan);
            }
            catch (Exception e) { try { MyConsole.WriteLine("[Lottery] start failed: " + e.Message, ConsoleColor.Yellow); } catch { } }
        }

        public static void Tick()
        {
            if (Interlocked.Exchange(ref busy, 1) == 1) return;
            try
            {
                GlobalLotteryModule.Tick();
                TurnoverLotteryModule.Tick();
                foreach (var c in LotteryCommon.Online())
                {
                    if (LotteryCommon.FirstSeen(c)) GlobalLotteryModule.LoginPush(c);
                    if (LotteryCommon.PendingCount(c.Player.UID) > 0) LotteryCommon.Deliver(c);
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            finally { Interlocked.Exchange(ref busy, 0); }
        }
    }
}
