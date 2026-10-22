using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Database
{
    public class HairColorType
    {
        public static System.Collections.Generic.List<HairColor> HairColors = new System.Collections.Generic.List<HairColor>();

        public class HairColor
        {
            public uint ID;
            public uint Cost;
            public byte Color;
        }

        public static void Load()
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "hair_color_type.txt"))
            {
                string[] Lines = System.IO.File.ReadAllLines((Program.ServerConfig.DbLocation + "hair_color_type.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, System.StringSplitOptions.None);
                    var Hc = new HairColor();
                    Hc.ID = System.Convert.ToUInt32(spilitline[0]);
                    Hc.Color = System.Convert.ToByte(spilitline[1]);
                    Hc.Cost = System.Convert.ToUInt32(spilitline[4]);
                    HairColors.Add(Hc);
                }
            }
        }
    }
}
