using System;
using System.Collections.Generic;
namespace System
{
    public class MapDictionary<T1, T2>
    {
        private System.Collections.Generic.Dictionary<T1, T2> DictBase;
        public System.Collections.Generic.Dictionary<T1, T2> Base
        {
            get
            {
                return this.DictBase;
            }
        }
        public int Count
        {
            get
            {
                return this.DictBase.Count;
            }
        }
        public T2 this[T1 key]
        {
            get
            {
                T2 result;
                if (this.ContainsKey(key))
                {
                    result = this.DictBase[key];
                }
                else
                {
                    result = default(T2);
                }
                return result;
            }
        }
        public System.Collections.Generic.Dictionary<T1, T2>.ValueCollection Values
        {
            get
            {
                if (this.DictBase == null)
                {
                    this.DictBase = new System.Collections.Generic.Dictionary<T1, T2>();
                }
                return this.DictBase.Values;
            }
        }
        public MapDictionary(int capacity)
        {
            this.DictBase = new System.Collections.Generic.Dictionary<T1, T2>(capacity);
        }
        public MapDictionary()
        {
            this.DictBase = new System.Collections.Generic.Dictionary<T1, T2>();
        }
        public bool Add(T1 key, T2 value)
        {
            bool result;
            if (!this.DictBase.ContainsKey(key))
            {
                this.DictBase.Add(key, value);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public void Remove(T1 key)
        {
            this.DictBase.Remove(key);
        }
        public bool ContainsKey(T1 key)
        {
            if (!this.DictBase.ContainsKey(key))
            {
                if (Convert.ToUInt32(key) != 0)
                {
                    Qahr.Mobile.Role.GameMap.EnterMap(Convert.ToInt32(key));
                }
            }
            return this.DictBase.ContainsKey(key);
        }
        public bool ContainsValue(T2 value)
        {
            return this.DictBase.ContainsValue(value);
        }
        public void Clear()
        {
            this.DictBase.Clear();
        }
        public bool TryGetValue(T1 key, out T2 value)
        {
            return this.DictBase.TryGetValue(key, out value);
        }
    }
}
