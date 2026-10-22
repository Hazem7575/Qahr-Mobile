// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Generic;

internal sealed class Class3<T> : Class2
{
    private TimerRule<T> timerRule_0;
    private T gparam_0;

    public Class3(TimerRule<T> timerRule_1, T gparam_1)
    {
        this.timerRule_0 = timerRule_1;
        this.gparam_0 = gparam_1;
    }

    internal override void vmethod_0()
    {
        if (this.timerRule_0 == null)
            return;
        this.timerRule_0.action_0(this.gparam_0, Time32.Now.Value);
        if (this.timerRule_0 == null)
            return;
        if (!this.timerRule_0.bool_0)
            this.Dispose();
        else
            this.method_1(this.timerRule_0.int_0);
    }

    internal override void vmethod_1()
    {
        this.timerRule_0 = (TimerRule<T>)null;
        this.gparam_0 = default(T);
    }

    internal override MethodInfo vmethod_2()
    {
        return this.timerRule_0.action_0.Method;
    }

    internal override ThreadPriority vmethod_3()
    {
        return this.timerRule_0.threadPriority_0;
    }
}