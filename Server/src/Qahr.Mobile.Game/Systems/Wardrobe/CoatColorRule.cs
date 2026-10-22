using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Role;
using Qahr.Mobile.WindowsAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile
{
    public class Coat_Color_Rule
    {
        public class ItemColor
        {
            public uint ItemType;
            public uint Type;
            public uint RandomChange_CPs;
            public uint ColorFixing_CPs;
            public uint ItemType1;
            public uint Amount1;
            public uint ItemType2;
            public uint Amount2;
            public uint Max;
            public uint Color1;
            public uint Color2;
            public uint Color3;
            public uint Color4;
            public uint Color5;
            public uint Color6;
            public uint Color7;
            public uint Color8;
            public uint F1;
            public uint F2;
        }
        public static Dictionary<uint, ItemColor> Info = new Dictionary<uint, ItemColor>();
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "coat_color_rule.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "coat_color_rule.txt");
                    return;
                }
                if (File.Exists(Program.ServerConfig.DbLocation + "coat_color_rule.txt"))
                {
                    Info = new Dictionary<uint, ItemColor>();
                    var Lines = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "coat_color_rule.txt");
                    foreach (string Line in Lines)
                    {
                        string[] spilitline = Line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        ItemColor attr = new ItemColor
                        {
                            ItemType = uint.Parse(spilitline[0]),
                            Type = uint.Parse(spilitline[1]),
                            RandomChange_CPs = uint.Parse(spilitline[2]),
                            ColorFixing_CPs = uint.Parse(spilitline[3]),
                            ItemType1 = uint.Parse(spilitline[4]),
                            Amount1 = uint.Parse(spilitline[5]),
                            ItemType2 = uint.Parse(spilitline[6]),
                            Amount2 = uint.Parse(spilitline[7]),
                            Max = uint.Parse(spilitline[8]),
                            Color1 = uint.Parse(spilitline[9]),
                            Color2 = uint.Parse(spilitline[10]),
                            Color3 = uint.Parse(spilitline[11]),
                            Color4 = uint.Parse(spilitline[12]),
                            Color5 = uint.Parse(spilitline[13]),
                            Color6 = uint.Parse(spilitline[14]),
                            Color7 = uint.Parse(spilitline[15]),
                            Color8 = uint.Parse(spilitline[16]),
                            F1 = uint.Parse(spilitline[18]),
                            F2 = uint.Parse(spilitline[18])
                        };
                        Info.Add(attr.ItemType, attr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}
