using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile
{
    public class dict_collection
    {
        public static Dictionary<uint, RuneItem> Runecollection = new Dictionary<uint, RuneItem>();
        public static RuneItem[] item;
        public class RuneItem
        {

            public uint ID;
            public uint Type;
            public uint Min;
            public uint Mix;
            public RuneAttribute Attribute;
            public uint Value1;
            public uint Value2;
        }
        public enum RuneAttribute
        {
            None = 0,
            MaxHP = 1,
            PAttack = 2,
            FinalPAttack = 6,
            FinalPDamage = 7,

        }
        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "dict_collection.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "dict_collection.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    RuneItem Obj = new RuneItem();
                    Obj.ID = Convert.ToUInt32(spilitline[0]);
                    Obj.Type = Convert.ToUInt32(spilitline[1]);
                    Obj.Min = Convert.ToUInt32(spilitline[2]);
                    Obj.Mix = Convert.ToUInt32(spilitline[3]);
                    Obj.Attribute = (RuneAttribute)Convert.ToUInt32(spilitline[4]);
                    Obj.Value1 = Convert.ToUInt32(spilitline[5]);
                    Obj.Value2 = Convert.ToUInt32(spilitline[6]);
                    if (!Runecollection.ContainsKey(Obj.ID))
                        Runecollection.Add(Obj.ID, Obj);
                }
            }
        }



    }
}
