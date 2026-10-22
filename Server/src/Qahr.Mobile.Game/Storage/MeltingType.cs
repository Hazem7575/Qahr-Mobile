using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class MeltingTypeTable
    {
        public static List<Melt> MeltingTypes = new List<Melt>();

        public enum Type : byte
        {
            MeltingItem = 1,
            PrizePool
        }
        public class Melt
        {
            public Type Type;
            public uint ItemID;
            public byte Gift;
            public uint GiftID;
            public byte PrizeType;

            public List<uint> Prizes;
        }

        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "MeltingType.ini"))
            {
                string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "MeltingType.ini");
                foreach (var line in Lines)
                {
                    if (line.StartsWith(";")) continue;
                    var spilitline = line.Split(' ');
                    if (spilitline[0]=="")
                        continue;
                    Melt melt = new Melt();
                    try
                    {

                        melt.Type = (Type)byte.Parse(spilitline[0]);

                        if (melt.Type == Type.PrizePool)
                        {
                            melt.PrizeType = byte.Parse(spilitline[1]);
                            melt.Prizes = new List<uint>();
                            for (int i = 0; i < spilitline.Length - 2; i++)
                                melt.Prizes.Add(uint.Parse(spilitline[i + 2]));
                        }
                        else
                        {
                            melt.ItemID = uint.Parse(spilitline[1]);
                            melt.Gift = byte.Parse(spilitline[2]);
                            melt.GiftID = uint.Parse(spilitline[3]);
                            melt.PrizeType = byte.Parse(spilitline[4]);
                        }
                        MeltingTypes.Add(melt);
                    }
                    catch
                    {
                        MyConsole.WriteLine(byte.Parse(spilitline[0]).ToString());

                    }
                }
            }
        }
    }
}