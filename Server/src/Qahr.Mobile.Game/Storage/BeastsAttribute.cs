using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class BeastsAtrribute
    {
        public static Dictionary<byte, Attribute> Attributes = new Dictionary<byte, Attribute>();

        public class Attribute
        {
            public byte Level;
            public uint RequiredPoints;

            public uint HPAdd;
            public uint PAttack;
            public uint MAttack;
            public uint PDefense;
            public uint MDefense;
            public uint FinalPDmgDealt;
            public uint FinalMDmgDealt;
            public uint FinalPDmgReceived;
            public uint FinalMDmgReceived;

            public byte YellowRuneSlots;
        }

        public static void Load()
        {
            string path = Program.ServerConfig.DbLocation + "beasts_attr.txt";

            if (!File.Exists(path))
                return;

            string[] Lines = File.ReadAllLines(path);

            foreach (var line in Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.None);


                if (spilitline.Length < 17)
                {
                    Console.WriteLine("Invalid line (columns missing): " + line);
                    continue;
                }

                try
                {
                    Attribute attr = new Attribute();

                    byte tmpByte;
                    uint tmpUInt;

                    if (byte.TryParse(spilitline[0].Trim(), out tmpByte))
                        attr.Level = tmpByte;
                    else continue;

                    if (uint.TryParse(spilitline[1].Trim(), out tmpUInt))
                        attr.RequiredPoints = tmpUInt;

                    if (uint.TryParse(spilitline[2].Trim(), out tmpUInt))
                        attr.HPAdd = tmpUInt;

                    if (uint.TryParse(spilitline[3].Trim(), out tmpUInt))
                        attr.PAttack = tmpUInt;

                    if (uint.TryParse(spilitline[4].Trim(), out tmpUInt))
                        attr.MAttack = tmpUInt;

                    if (uint.TryParse(spilitline[5].Trim(), out tmpUInt))
                        attr.PDefense = tmpUInt;

                    if (uint.TryParse(spilitline[6].Trim(), out tmpUInt))
                        attr.MDefense = tmpUInt;

                    if (uint.TryParse(spilitline[7].Trim(), out tmpUInt))
                        attr.FinalPDmgDealt = tmpUInt;

                    if (uint.TryParse(spilitline[8].Trim(), out tmpUInt))
                        attr.FinalMDmgDealt = tmpUInt;

                    if (uint.TryParse(spilitline[9].Trim(), out tmpUInt))
                        attr.FinalPDmgReceived = tmpUInt;

                    if (uint.TryParse(spilitline[10].Trim(), out tmpUInt))
                        attr.FinalMDmgReceived = tmpUInt;

                    if (byte.TryParse(spilitline[16].Trim(), out tmpByte))
                        attr.YellowRuneSlots = tmpByte;


                    if (!Attributes.ContainsKey(attr.Level))
                        Attributes.Add(attr.Level, attr);
                    else
                        Console.WriteLine("Duplicate Level skipped: " + attr.Level);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing line: " + line);
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}