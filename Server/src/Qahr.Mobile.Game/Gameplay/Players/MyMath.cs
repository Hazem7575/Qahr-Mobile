using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Role
{
    public partial class MyMath
    {
        private static SafeRandom Rand = new SafeRandom(Environment.TickCount);

        public const Int32 NORMAL_RANGE = 17;
        public const Int32 BIG_RANGE = 34;
        public const Int32 USERDROP_RANGE = 9;
        /// <summary>
        /// Generate a number in a specified range. (Number ∈ [Min, Max])
        /// </summary>
        public static Int32 Generate(Int32 Min, Int32 Max)
        {
            if (Max != Int32.MaxValue)
                Max++;

            Int32 Value = 0;
            /*lock (Rand) { */
            Value = Rand.Next(Min, Max); /*}*/
            return Value;
        }

        public static Boolean Success(Double Chance) 
        { 
            return ((Double)Generate(1, 1000000)) / 10000 >= 100 - Chance;
        }
    }
}
