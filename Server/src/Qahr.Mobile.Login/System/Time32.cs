// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System
{
    public struct Time32
    {
        public static readonly Time32 NULL = new Time32(0);
        private int int_0;
        private static uint uint_0;

        public static Time32 Now
        {
            get
            {
                return Time32.timeGetTime();
            }
        }

        public int TotalMilliseconds
        {
            get
            {
                return this.int_0;
            }
        }

        public int Value
        {
            get
            {
                return this.int_0;
            }
        }

        static Time32()
        {
        }

        public Time32(int Value)
        {
            this.int_0 = Value;
        }

        public Time32(uint Value)
        {
            this.int_0 = (int)Value;
        }

        public Time32(long Value)
        {
            this.int_0 = (int)Value;
        }

        public static bool operator ==(Time32 t1, Time32 t2)
        {
            return t1.int_0 == t2.int_0;
        }

        public static bool operator !=(Time32 t1, Time32 t2)
        {
            return t1.int_0 != t2.int_0;
        }

        public static bool operator >(Time32 t1, Time32 t2)
        {
            return t1.int_0 > t2.int_0;
        }

        public static bool operator <(Time32 t1, Time32 t2)
        {
            return t1.int_0 < t2.int_0;
        }

        public static bool operator >=(Time32 t1, Time32 t2)
        {
            return t1.int_0 >= t2.int_0;
        }

        public static bool operator <=(Time32 t1, Time32 t2)
        {
            return t1.int_0 <= t2.int_0;
        }

        public static Time32 operator -(Time32 t1, Time32 t2)
        {
            return new Time32(t1.int_0 - t2.int_0);
        }

        public Time32 AddMilliseconds(int Amount)
        {
            return new Time32(this.int_0 + Amount);
        }

        public int AllMilliseconds()
        {
            return this.GetHashCode();
        }

        public Time32 AddSeconds(int Amount)
        {
            return this.AddMilliseconds(Amount * 1000);
        }

        public int AllSeconds()
        {
            return this.AllMilliseconds() / 1000;
        }

        public Time32 AddMinutes(int Amount)
        {
            return this.AddSeconds(Amount * 60);
        }

        public int AllMinutes()
        {
            return this.AllSeconds() / 60;
        }

        public Time32 AddHours(int Amount)
        {
            return this.AddMinutes(Amount * 60);
        }

        public int AllHours()
        {
            return this.AllMinutes() / 60;
        }

        public Time32 AddDays(int Amount)
        {
            return this.AddHours(Amount * 24);
        }

        public int AllDays()
        {
            return this.AllHours() / 24;
        }

        public bool Next(int due = 0, int time = 0)
        {
            if (time == 0)
                time = Time32.timeGetTime().int_0;
            return this.int_0 + due <= time;
        }

        public void Set(int due, int time = 0)
        {
            if (time == 0)
                time = Time32.timeGetTime().int_0;
            this.int_0 = time + due;
        }

        public void SetSeconds(int due, int time = 0)
        {
            this.Set(due * 1000, time);
        }

        public override bool Equals(object obj)
        {
            if (obj is Time32)
                return (Time32)obj == this;
            else
                return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.int_0.ToString();
        }

        public override int GetHashCode()
        {
            return this.int_0;
        }

        [DllImport("winmm.dll")]
        public static extern Time32 timeGetTime();
    }
}
