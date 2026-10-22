using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class SpiritTable
    {
        public static List<Spirit> SpiritRates = new List<Spirit>();

        public enum Type : byte
        {
            Failed,
            Upgraded,
            Awkaned
        }
        public class Spirit
        {
            public uint Index;
            public uint AnimaID;
            public Type Type;
            public bool IsItem;
            public uint NewAnimaID;
        }

        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "spirit_rate.txt"))
            {
                string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "spirit_rate.txt");
                foreach (var line in Lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    Spirit s = new Spirit();
                    s.Index = uint.Parse(spilitline[0]);
                    s.AnimaID = uint.Parse(spilitline[2]);
                    s.Type = (Type)byte.Parse(spilitline[3]);
                    s.IsItem = byte.Parse(spilitline[4]) == 1;
                    s.NewAnimaID = uint.Parse(spilitline[5]);
                    SpiritRates.Add(s);
                }
            }
        }
    }
}