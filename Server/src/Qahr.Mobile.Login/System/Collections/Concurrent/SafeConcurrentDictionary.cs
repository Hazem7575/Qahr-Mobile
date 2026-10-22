// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Concurrent
{
    public class SafeConcurrentDictionary<T, T2> : ConcurrentDictionary<T, T2>
    {
        public new T2 this[T key]
        {
            get
            {
                if (this.ContainsKey(key))
                    return base[key];
                else
                    return default(T2);
            }
            set
            {
                base[key] = value;
            }
        }
    }
}