// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    public class ThreadSafeDictionary<T1, T2>
    {
        private Dictionary<T1, T2> SafeDictionaryBase;
        public T2[] Values = new T2[0];
        public ThreadSafeDictionary(int capacity)
        {
            SafeDictionaryBase = new Dictionary<T1, T2>(capacity);
        }
        public int Count
        {
            get
            {
                return SafeDictionaryBase.Count;
            }
        }
        public Dictionary<T1, T2> Base
        {
            get
            {
                return SafeDictionaryBase;
            }
        }
        public void Add(T1 key, T2 value)
        {
            if (SafeDictionaryBase.ContainsKey(key) == false)
            {
                SafeDictionaryBase.Add(key, value);
                safeUpdate();
            }
        }
        public void Remove(T1 key)
        {
            SafeDictionaryBase.Remove(key);
            safeUpdate();
        }
        public T2 this[T1 key]
        {
            get
            {
                if (ContainsKey(key))
                    return SafeDictionaryBase[key];
                else return default(T2);
            }
        }
        public bool TryGetValue(T1 key, out T2 value)
        {
            return SafeDictionaryBase.TryGetValue(key, out value);
        }
        public bool ContainsKey(T1 key)
        {
            return SafeDictionaryBase.ContainsKey(key);
        }
        public bool ContainsValue(T2 value)
        {
            return SafeDictionaryBase.ContainsValue(value);
        }
        private void safeUpdate()
        {
            lock (Values)
            {
                Values = SafeDictionaryBase.Values.ToArray();
            }
        }
    }
}