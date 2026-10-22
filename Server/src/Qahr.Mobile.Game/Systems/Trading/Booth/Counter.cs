using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NightMares
{
    public class Counter
    {
        uint Start = 0;
        uint finish = uint.MaxValue;

        public uint Finish
        {
            get
            {
                return finish;
            }
            set
            {
                finish = value;
            }
        }

        public uint Now
        {
            get;
            set;
        }

        public uint Next
        {
            get
            {
                Now++;
                if (Now == Finish)
                    Now = Start;
                return Now;
            }
        }
        public Counter()
        {
            Now = Start;
        }
        public Counter(uint startFrom)
        {
            Start = startFrom;
            Now = startFrom;
        }
    }
}
