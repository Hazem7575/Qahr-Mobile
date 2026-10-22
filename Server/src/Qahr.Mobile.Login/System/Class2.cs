// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

internal abstract class Class2 : IDisposable
{
    internal static volatile int int_0 = int.MinValue;
    internal bool bool_0;
    internal bool bool_1;
    internal Time32 time32_0;
    protected int int_1;

    static Class2()
    {
    }

    public Class2()
    {
        ++Class2.int_0;
        this.int_1 = Class2.int_0;
        this.bool_0 = true;
        this.bool_1 = false;
        this.method_1(0);
    }

    ~Class2()
    {
        this.Dispose();
    }

    internal abstract void vmethod_0();

    internal bool method_0()
    {
        return Time32.Now > this.time32_0;
    }

    internal void method_1(int int_2)
    {
        this.time32_0 = Time32.Now.AddMilliseconds(int_2);
    }

    public void Dispose()
    {
        this.bool_0 = false;
        this.vmethod_1();
    }

    internal abstract void vmethod_1();

    internal abstract MethodInfo vmethod_2();

    internal abstract ThreadPriority vmethod_3();

    public override int GetHashCode()
    {
        return this.int_1;
    }
}