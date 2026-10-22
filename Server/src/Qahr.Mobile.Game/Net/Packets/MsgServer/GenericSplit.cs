using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public class GenericSplit
    {
        public Dictionary<uint, List<T>> Lists<T>(int max_fiels, int split_size, ushort page, T[] Array)
        {
            Dictionary<uint, List<T>> lists = new Dictionary<uint, List<T>>();

            int offset = page / max_fiels * max_fiels;
            int count = Math.Min(max_fiels, Math.Max(0, Array.Length - offset));

            uint Size = (uint)((max_fiels / split_size) + 1);

            for (int x = 0; x < count; x++)
            {
                if (Array.Length > offset + x)
                {
                    var element = Array[offset + x];
                    if (x % split_size == 0)
                        lists.Add(--Size, new List<T>());
                    lists[Size].Add(element);

                }
            }
            return lists;
        }
        public Dictionary<uint, List<T>> Lists<T>(int max_fiels, T[] Array)
        {
            Dictionary<uint, List<T>> lists = new Dictionary<uint, List<T>>();

            int count = Array.Length;

            uint Size = (uint)(max_fiels);

            for (int x = 0; x < Array.Length; x++)
            {
                var element = Array[x];
                if (x % max_fiels == 0)
                    lists.Add(++Size, new List<T>());
                lists[Size].Add(element);
            }
            return lists;
        }
    }
}
