// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

namespace Qahr.Mobile.Login
{
    public unsafe class Counter
    {
        private uint Start = 0;
        private uint finish = uint.MaxValue;
        public uint Finish
        {
            get { return finish; }
            set { finish = value; }
        }
        public uint Now
        {
            get; set; 
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