using Qahr.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Database
{
    public class dict_lottery
    {
        public enum Quality : uint
        {
            Refined = 1,
            Super = 2,
            Sacred = 3
        }
        public class item
        {
            public uint ID;
            public uint Quality;
            public uint IDItem;
            public uint Unk1;
            public uint Unk2;
            public uint Unk3;
        }
        public static System.SafeDictionary<uint, item> DaoqiItem = new System.SafeDictionary<uint, item>();
        public static void Load()
        {
         
            if (File.Exists(Program.ServerConfig.DbLocation + "dict_lottery.txt"))
            {
                string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "dict_lottery.txt");
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    item attr = new item();
                    attr.ID = uint.Parse(spilitline[0]);
                    attr.Quality = uint.Parse(spilitline[1]);
                    attr.Unk1 = uint.Parse(spilitline[2]);
                    attr.IDItem = uint.Parse(spilitline[3]);
                    attr.Unk2 = uint.Parse(spilitline[4]);
                    attr.Unk3 = uint.Parse(spilitline[5]);
                    DaoqiItem.Add(attr.ID, attr);
                }
               
            }
        }
    }
}
