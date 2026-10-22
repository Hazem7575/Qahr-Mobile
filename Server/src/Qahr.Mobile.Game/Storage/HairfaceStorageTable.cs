using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class HairfaceStorageTable
    {
        public class Hairface
        {
            public Game.MsgServer.MsgHairfaceStorage.Type Type;
            public uint ID;
            public string Name;
            public byte RareLevel;
            public byte RequiredVIPLevel;
            public uint Cost;
            public List<byte> Classes;
            public byte Sex;
            public byte EquippedColor;
            public byte[] Colors;
            public bool Equiped;
        }

        public class HairColor
        {
            public uint ID;
            public uint Cost;
            public byte Color;
        }

        public static List<Hairface> Hairfaces = new List<Hairface>();
        public static List<HairColor> HairColors = new List<HairColor>();

        public static void LoadFaces()
        {
            string path = Program.ServerConfig.DbLocation + "hairface_storage_type.txt";

            if (!File.Exists(path))
                return;

            string[] Lines = File.ReadAllLines(path);

            foreach (var line in Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.None);

                if (spilitline.Length < 10)
                    continue;

                try
                {
                    Hairface Hf = new Hairface();

                    byte tmpByte;
                    uint tmpUInt;


                    if (byte.TryParse(spilitline[0].Trim(), out tmpByte))
                        Hf.Type = (Game.MsgServer.MsgHairfaceStorage.Type)tmpByte;


                    if (uint.TryParse(spilitline[1].Trim(), out tmpUInt))
                        Hf.ID = tmpUInt;


                    Hf.Name = spilitline[3].Trim();


                    if (byte.TryParse(spilitline[4].Trim(), out tmpByte))
                        Hf.RareLevel = tmpByte;


                    if (byte.TryParse(spilitline[5].Trim(), out tmpByte))
                        Hf.RequiredVIPLevel = tmpByte;


                    if (uint.TryParse(spilitline[7].Trim(), out tmpUInt))
                        Hf.Cost = tmpUInt;


                    Hf.Classes = new List<byte>();
                    var spilit = spilitline[9].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in spilit)
                    {
                        if (byte.TryParse(item.Trim(), out tmpByte))
                            Hf.Classes.Add(tmpByte);
                    }


                    if (spilitline.Length > 10 && byte.TryParse(spilitline[10].Trim(), out tmpByte))
                        Hf.Sex = tmpByte;
                    else
                        Hf.Sex = 0;


                    Hf.Colors = new byte[7] { 1, 0, 0, 0, 0, 0, 0 };
                    Hf.Equiped = false;
                    Hf.EquippedColor = 0;

                    Hairfaces.Add(Hf);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing line: " + line);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void LoadColors()
        {
            string path = Program.ServerConfig.DbLocation + "hair_color_type.txt";

            if (!File.Exists(path))
                return;

            string[] Lines = File.ReadAllLines(path);

            foreach (var line in Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.None);

                if (spilitline.Length < 5)
                    continue;

                try
                {
                    HairColor Hc = new HairColor();

                    byte tmpByte;
                    uint tmpUInt;

                    if (uint.TryParse(spilitline[0].Trim(), out tmpUInt))
                        Hc.ID = tmpUInt;

                    if (byte.TryParse(spilitline[1].Trim(), out tmpByte))
                        Hc.Color = tmpByte;

                    if (uint.TryParse(spilitline[4].Trim(), out tmpUInt))
                        Hc.Cost = tmpUInt;

                    HairColors.Add(Hc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing color line: " + line);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void Load()
        {
            LoadFaces();
            LoadColors();
        }
    }
}