using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database.DBActions
{
   public class ReadLine : IDisposable
    {
       public int GetCount()
       {
           if (data == null)
               return 0;
           return data.Length;
       }
       public bool Valid = false;
       private string[] data;
       private int Count = 0;
       public ReadLine(string line, char Split)
       {
           if (line == null)
               return;
           if (!line.Contains(Split))
               return;
           data = line.Split(Split);
           Valid = true;
       }

       public int Read(int def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               int val = 0;
               if (!int.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public uint Read(uint def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               uint val = 0;
               if (!uint.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public byte Read(byte def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               byte val = 0;
               if (!byte.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public sbyte Read(sbyte def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               sbyte val = 0;
               if (!sbyte.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public ushort Read(ushort def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               ushort val = 0;
               if (!ushort.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public ulong Read(ulong def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               ulong val = 0;
               if (!ulong.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public long Read(long def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
               long val = 0;
               if (!long.TryParse(data[Count], out val))
               {
                   Count++;
                   return def_val;
               }
               else
               {
                   Count++;
                   return val;
               }
           }
           Count++;
           return def_val;
       }
       public string Read(string def_val)
       {
           if (!Valid)
           {
               Count++;
               return def_val;
           }
           if (data.Length > Count)
           {
             
               string ddata = data[Count];
                 Count++;
                 return ddata;
           }
           Count++;
           return def_val;
       }
       public void Dispose()
       {
           data = null;
       }
    }
}
