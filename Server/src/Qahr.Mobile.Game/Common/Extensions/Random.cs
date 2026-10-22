using System;

namespace System
{
    public class RandomLite
    {
        private const uint Y = 842502087u;

        private const uint Z = 3579807591u;

        private const uint W = 273326509u;

        private volatile uint x;

        private volatile uint y;

        private volatile uint z;

        private volatile uint w;

        public RandomLite()
            : this(Environment.TickCount)
        {
        }

        public RandomLite(int seed)
        {
            Reseed(seed);
        }

        public void Reseed(int seed)
        {
            x = (uint)seed;
            y = 842502087u;
            z = 3579807591u;
            w = 273326509u;
        }

        public int Next()
        {
            uint num = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = (w ^ (w >> 19) ^ (num ^ (num >> 8)));
            uint num2 = w & 0x7FFFFFFF;
            if (num2 == 2147483647)
            {
                return Next();
            }
            return (int)num2;
        }

        public int Next(int upperBound)
        {
            if (upperBound < 0)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            }
            if (upperBound == 0)
            {
                return 0;
            }
            return Next() % (upperBound + 1);
        }

        public int Next(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
            }
            if (lowerBound == upperBound)
            {
                return lowerBound;
            }
            return lowerBound + Next() % (upperBound + 1 - lowerBound);
        }
    }
    /// <summary>
    /// Lock-free replacement for the old "lock(SyncRoot) + one shared System.Random".
    /// Every thread owns its own System.Random (same algorithm, same Next/Next(max)/Next(min,max)
    /// semantics: max exclusive, same exceptions), so the hot combat path no longer serialises
    /// on one global lock. Only the sequence differs; range and distribution are identical.
    /// </summary>
    public class SafeRandom
    {
        // Kept for API compatibility only (nothing locks on it any more).
        public object SyncRoot;

        [ThreadStatic]
        private static Random t_Rand;

        public SafeRandom(int seed = 0)
        {
            SyncRoot = new object();
        }

        private static Random CreateRandom()
        {
#if NET6_0_OR_GREATER
            // Unseeded Random on .NET 6+ is seeded from the OS RNG -> independent per thread.
            return new Random();
#else
            // .NET Framework: unseeded Random uses TickCount, so threads created in the same
            // tick would share a sequence. Use a unique random seed per thread instead.
            return new Random(Guid.NewGuid().GetHashCode());
#endif
        }

        private static Random Rand
        {
            get
            {
                Random r = t_Rand;
                if (r == null)
                    t_Rand = r = CreateRandom();
                return r;
            }
        }

        public int Next(int minval, int maxval)
        {
            return Rand.Next(minval, maxval);
        }

        public int Next(int maxval)
        {
            return Rand.Next(maxval);
        }

        public int Next()
        {
            return Rand.Next();
        }

        public void SetSeed(int seed)
        {
            // Old behaviour: replace the generator with a freshly (randomly) seeded one; the
            // argument was ignored. Now reseeds the calling thread's generator.
            t_Rand = CreateRandom();
        }
    }

}