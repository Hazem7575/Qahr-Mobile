// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class FastRandom
    {
        private uint uint_0;
        private uint uint_1;
        private uint uint_2;
        private uint uint_3;
        private object object_0;

        public FastRandom()
            : this(Time32.Now.TotalMilliseconds)
        {
        }

        public FastRandom(int seed)
        {
            this.object_0 = new object();
            this.Reinitialise(seed);
        }

        public void Reinitialise(int seed)
        {
            lock (this.object_0)
            {
                this.uint_0 = (uint)seed;
                this.uint_1 = 842502087U;
                this.uint_2 = 3579807591U;
                this.uint_3 = 273326509U;
            }
        }

        public int Next()
        {
            lock (this.object_0)
            {
                uint local_0 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_0 ^ (int)(local_0 >> 8)));
                uint local_1 = this.uint_3 & (uint)int.MaxValue;
                if ((int)local_1 == int.MaxValue)
                    return this.Next();
                else
                    return (int)local_1;
            }
        }

        public int Next(int upperBound)
        {
            lock (this.object_0)
            {
                if (upperBound < 0)
                    upperBound = 0;
                uint local_0 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return (int)(4.65661287307739E-10 * (double)(int.MaxValue & (int)(this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_0 ^ (int)(local_0 >> 8))))) * (double)upperBound);
            }
        }

        public int Sign()
        {
            return this.Next(0, 2) == 0 ? -1 : 1;
        }

        public int Next(int lowerBound, int upperBound)
        {
            lock (this.object_0)
            {
                if (lowerBound > upperBound)
                {
                    int local_0 = lowerBound;
                    lowerBound = upperBound;
                    upperBound = local_0;
                }
                uint local_1 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                int local_2 = upperBound - lowerBound;
                if (local_2 < 0)
                    return lowerBound + (int)(0.0 * Math.PI * (double)(this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_1 ^ (int)(local_1 >> 8)))) * (double)((long)upperBound - (long)lowerBound));
                else
                    return lowerBound + (int)(4.65661287307739E-10 * (double)(int.MaxValue & (int)(this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_1 ^ (int)(local_1 >> 8))))) * (double)local_2);
            }
        }

        public double NextDouble()
        {
            lock (this.object_0)
            {
                uint local_0 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return 4.65661287307739E-10 * (double)(int.MaxValue & (int)(this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_0 ^ (int)(local_0 >> 8)))));
            }
        }

        public unsafe void NextBytes(byte[] buffer)
        {
            lock (this.object_0)
            {
                if (buffer.Length % 8 != 0)
                {
                    new Random().NextBytes(buffer);
                }
                else
                {
                    uint local_0 = this.uint_0;
                    uint local_1 = this.uint_1;
                    uint local_2 = this.uint_2;
                    uint local_3 = this.uint_3;
                    fixed (byte* fixed_0 = buffer)
                    {
                        int local_6 = 0;
                        int local_7 = buffer.Length >> 2;
                        while (local_6 < local_7)
                        {
                            uint local_8 = local_0 ^ local_0 << 11;
                            uint local_0_1 = local_1;
                            uint local_1_1 = local_2;
                            uint local_2_1 = local_3;
                            uint local_3_1;
                            ((uint*)fixed_0)[local_6] = local_3_1 = (uint)((int)local_3 ^ (int)(local_3 >> 19) ^ ((int)local_8 ^ (int)(local_8 >> 8)));
                            uint local_8_1 = local_0_1 ^ local_0_1 << 11;
                            local_0 = local_1_1;
                            local_1 = local_2_1;
                            local_2 = local_3_1;
                            ((uint*)fixed_0)[local_6 + 1] = local_3 = (uint)((int)local_3_1 ^ (int)(local_3_1 >> 19) ^ ((int)local_8_1 ^ (int)(local_8_1 >> 8)));
                            local_6 += 2;
                        }
                    }
                }
            }
        }

        public uint NextUInt()
        {
            lock (this.object_0)
            {
                uint local_0 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_0 ^ (int)(local_0 >> 8)));
            }
        }

        public int NextInt()
        {
            lock (this.object_0)
            {
                uint local_0 = this.uint_0 ^ this.uint_0 << 11;
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return int.MaxValue & (int)(this.uint_3 = (uint)((int)this.uint_3 ^ (int)(this.uint_3 >> 19) ^ ((int)local_0 ^ (int)(local_0 >> 8))));
            }
        }
    }
}