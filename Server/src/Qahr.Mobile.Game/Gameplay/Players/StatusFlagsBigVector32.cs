using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role
{
    public class StatusFlagsBigVector32 : System.BitVector32
    {
        public const int PermanentFlag = 60 * 60 * 24 * 30;
        public const int WeeklyFlag = 7 * 24 * 30;
        public const int DailyFlag = 24 * 30;
        public Flag FlagValue;
        public class Flag
        {

            public int Secounds = 0;
            public int InvokerSecouds = 0;
            public int Key;
            public DateTime TimerInvoker = new DateTime();
            public DateTime Timer = new DateTime();
            public bool RemoveOnDead = false;
            public Flag(int _flag, int _Secounds, bool _removeondead, int _InvokerSecouds)
            {
                Secounds = _Secounds;
                Key = _flag;
                Timer = DateTime.Now;
                RemoveOnDead = _removeondead;
                InvokerSecouds = _InvokerSecouds;
                if (InvokerSecouds != 0)
                    TimerInvoker = DateTime.Now;
            }

            public bool Expire(DateTime Now)
            {
                if (Now >= Timer.AddSeconds(Secounds))
                    return true;
                return false;
            }
            public bool CheckInvoke(DateTime Now)
            {
                if (InvokerSecouds == 0)
                    return true;
                else
                {
                    if (Now >= TimerInvoker.AddSeconds(InvokerSecouds))
                    {
                        TimerInvoker = DateTime.Now;
                        return true;
                    }
                    return false;
                }
            }
            public DateTime ElseStamp = new DateTime();
            public bool CheckElseInvocke(DateTime Now)
            {
                if (Now > ElseStamp)
                {
                    ElseStamp = DateTime.Now;
                    ElseStamp = DateTime.Now.AddSeconds(2);
                    return true;
                }
                return false;
            }
            public bool CheckElseInvockes(DateTime Now)
            {
                if (Now > ElseStamp)
                {
                    ElseStamp = DateTime.Now;
                    ElseStamp = DateTime.Now.AddSeconds(5);
                    return true;
                }
                return false;
            }
            
            
        }

        public System.Collections.Concurrent.ConcurrentDictionary<int, Flag> ArrayFlags;
        private Flag[] Array = new Flag[0];
        private bool Update = false;

        public StatusFlagsBigVector32(int Size)
            : base(Size)
        {
            ArrayFlags = new System.Collections.Concurrent.ConcurrentDictionary<int, Flag>();
        }

        public bool TryAdd(int flag, int Secounds, bool RemoveOnDead, int InvokerSecouds)
        {
            if (!ArrayFlags.ContainsKey(flag))
            {
                ArrayFlags.TryAdd(flag, new Flag(flag, Secounds, RemoveOnDead, InvokerSecouds));
                Add(flag);

                Update = true;

                return true;
            }
            return false;
        }
        public bool UpdateFlag(int flag, int Secounds, bool SetNewTimer, int MaxSecounds)
        {
            Flag FlagClass;
            if (ArrayFlags.TryGetValue(flag, out FlagClass))
            {
                if (SetNewTimer)
                {
                    FlagClass.Timer = DateTime.Now;
                    FlagClass.Secounds = Secounds;
                }
                else
                {
                    if (FlagClass.Timer.AddSeconds(FlagClass.Secounds) > DateTime.Now.AddSeconds(MaxSecounds))
                    {
                        FlagClass.Timer = DateTime.Now;
                        FlagClass.Secounds = MaxSecounds;
                    }
                    else
                        FlagClass.Secounds += Secounds;

                }
                return true;
            }
            return false;
        }
        public bool TryRemove(int flag)
        {
            Flag FlagClass;
            if (ArrayFlags.TryRemove(flag, out FlagClass))
            {
                Remove(flag);

                Update = true;

                return true;
            }
            return false;
        }
        public bool InLife(int flag, DateTime Now64)
        {
            Flag FlagClass;
            if (ArrayFlags.TryGetValue(flag, out FlagClass))
            {
                return FlagClass.Expire(Now64);
            }
            return false;
        }
        public bool CheckInvoke(int flag, DateTime Now64)
        {
            Flag FlagClass;
            if (ArrayFlags.TryGetValue(flag, out FlagClass))
            {
                return FlagClass.CheckInvoke(Now64);
            }
            return false;
        }
        public bool ContainFlag(int flag)
        {
            return ArrayFlags.ContainsKey(flag);
        }
        public void GetClear()
        {
            List<int> remove = new List<int>();

            foreach (var item in GetFlags())
            {
                if (item.RemoveOnDead)
                    remove.Add(item.Key);
            }
            foreach (var item in remove)
                TryRemove(item);
        }
        public Flag[] GetFlags()
        {
            try
            {
                if (Update)
                {
                    Array = ArrayFlags.Values.ToArray();
                    Update = false;
                }
                return Array;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return new Flag[0];
            }

        }
    }
}
