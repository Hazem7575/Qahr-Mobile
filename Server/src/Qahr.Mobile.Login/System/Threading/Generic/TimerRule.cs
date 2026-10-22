// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

namespace System.Threading.Generic
{
    public class TimerRule<T>
    {
        internal Action<T, int> action_0;
        internal int int_0;
        internal bool bool_0;
        internal ThreadPriority threadPriority_0;
        public TimerRule(Action<T, int> action, int period, ThreadPriority priority = ThreadPriority.Normal)
        {
            this.action_0 = action;
            this.int_0 = period;
            this.bool_0 = true;
            this.threadPriority_0 = priority;
        }
        ~TimerRule()
        {
            this.action_0 = (Action<T, int>)null;
        }
    }
}