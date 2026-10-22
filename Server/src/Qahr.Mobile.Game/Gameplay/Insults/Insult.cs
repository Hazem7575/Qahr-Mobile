using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Insults
{
    public struct Insult
    {
        //insult max length must be 32 offest :D
        public unsafe fixed sbyte _text[32];
        public unsafe string Text
        {
            get { fixed (sbyte* txt = _text) { return new string(txt); } }
            set
            {
                string txt = null;
                if (value.Length > 32)
                {
                    int s = (int)(value.Length > 32 ? 32 : value.Length);
                    for (int p = 0; p < s; p++)
                    {
                        txt += value[p];
                    }
                }
                else
                    txt = value;
                fixed (sbyte* txs = _text)
                {
                    for (int x = 0; x < txt.Length; x++)
                        txs[x] = (sbyte)txt[x];
                }
            }
        }
        public byte Votedtrue;
        public byte Votedfalse;
    }
}
