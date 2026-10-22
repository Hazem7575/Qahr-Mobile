using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Qahr.Mobile
{
    public class ThreadPlayer
    {
        public const int

            AI_Buffers = 500,

            AI_Guards = 700,

            User_SaveMele = 500,

            AI_Monsters = 1000,

            User_Stamina = 500,

            User_Buffers = 300,

            User_StampXPCount = 3000,

            User_AutoAttack = 500,

            User_CheckSecounds = 1000,

            User_FloorSpell = 100,

            User_ItemTIme = 1000;


        public System.ThreadGroup.ThreadItem Thread;
        public ThreadPlayer(int interval, string name)
        {
            Thread = new System.ThreadGroup.ThreadItem(interval, name, OnProcess);
        }
        public void Start()
        {
            Thread.Open();
        }

        public void OnProcess()
        {
            // قياس زمن الدورة فقط (PerfMetrics.TickDone) — منطق اللوب نفسه في OnProcessCore بدون أي تغيير.
            long tickStart = System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {
                OnProcessCore();
            }
            finally
            {
                long elapsed = System.Diagnostics.Stopwatch.GetTimestamp() - tickStart;
                PerfMetrics.TickDone(elapsed * 1000000L / System.Diagnostics.Stopwatch.Frequency);
            }
        }

        private void OnProcessCore()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            foreach (var user in Pool.GamePoll.Values)
            {
                try
                {
                    if (user == null || user.Player == null || user.Player.View == null)
                        continue;
                    if (!user.Fake)
                    {
                        user.Player.View.CheckUpMonsters(clock);
                        if (clock > user.BuffersStamp)
                        {
                            Client.PoolProcesses.BuffersCallback(user);
                            user.BuffersStamp.Value = clock.Value + User_Buffers;
                        }

                        if (clock > user.AttackStamp)
                        {
                            Client.PoolProcesses.AutoAttackCallback(user);
                            user.AttackStamp.Value = clock.Value + User_AutoAttack;
                        }
                        if (clock > user.CheckSecoundsStamp)
                        {
                            Client.PoolProcesses.CharactersCallback(user);
                            user.CheckSecoundsStamp.Value = clock.Value + User_CheckSecounds;
                        }
                        if (clock > user.FloorSpellStamp)
                        {
                            Client.PoolProcesses.FloorCallback(user);
                            user.FloorSpellStamp.Value = clock.Value + User_FloorSpell;
                        }
                        if (user.onSaveMele && DateTime.Now > user.SaveMeleStamp)
                        {
                            Client.PoolProcesses.SaveMeleAttack(user);
                            user.SaveMeleStamp = DateTime.Now.AddMilliseconds(User_SaveMele);
                        }
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("ThreadPlayer processing failed for player: " +
                        (user != null && user.Player != null ? (user.Player.Name + " (UID " + user.Player.UID + ")") : "<unknown>"));
                    MyConsole.WriteException(e);
                }
            }

        }
    }
}
