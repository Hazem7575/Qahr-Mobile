using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database.DBActions
{
   public class WriteLine
    {
       private char Spliter { get;  set; }
       private StringBuilder builder{ get;  set; }

       public WriteLine(char split)
       {
           builder = new StringBuilder();
           Spliter = split;
       }
       public WriteLine Add(int[] data)
       {
           for (int x = 0; x < data.Length; x++)
               Add(data[x]);
           return this;
       }
       public WriteLine Add(byte data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(sbyte data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(short data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(ushort data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(uint data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(int data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(long data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(ulong data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public WriteLine Add(string data)
       {
           builder.Append(data.ToString() + Spliter);
           return this;
       }
       public string Close()
       {
           return builder.ToString();
       }
    }
}
