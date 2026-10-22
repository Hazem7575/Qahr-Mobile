using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class Counter
    {
        // Backed by a long so Interlocked can be used; exposed as uint (unchecked).
        private long val;
        public Counter(uint start)
        {
            val = start;
        }
        // Thread-safe atomic increment. Called from dozens of threads for UID allocation.
        public uint Next { get { return unchecked((uint)System.Threading.Interlocked.Increment(ref val)); } }
        public uint Count { get { return unchecked((uint)System.Threading.Interlocked.Read(ref val)); } }

        public void Set(uint start)
        {
            System.Threading.Interlocked.Exchange(ref val, start);
        }
    }
}
