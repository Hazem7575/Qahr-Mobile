using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class god_weapons_exp
    {
        public static Dictionary<uint, Dictionary<uint, EXPAstredge>> EXP;
        public class EXPAstredge
        {
            public uint ID;
            public uint TYPE;
            public byte Level;
            public uint EXP;
            public uint Prestige;
            public uint Unk5;
            public uint Unk6;
            public uint Unk7;
          
        }
        public static bool TryGetValue(uint id, byte level, out EXPAstredge Items)
        {
            if (god_weapons_exp.EXP.ContainsKey(id) && god_weapons_exp.EXP[id].TryGetValue(level, out Items))
                return true;
            Items = (EXPAstredge)null;
            return false;
        }
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "god_weapons_exp.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "god_weapons_exp.txt");
                }
                else
                {
                    god_weapons_exp.EXP = new Dictionary<uint, Dictionary<uint, EXPAstredge>>();
                    foreach (string ReadAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "god_weapons_exp.txt"))
                    {
                        string[] REntries = new string[2] { "@@", " " };
                        string[] Line = ReadAllLine.Split(REntries, StringSplitOptions.RemoveEmptyEntries);
                        EXPAstredge EXP = new EXPAstredge();
                        EXP.ID = Convert.ToUInt32(Line[0]);
                        EXP.TYPE = Convert.ToUInt32(Line[1]);
                        EXP.Level = Convert.ToByte(Line[2]);
                        EXP.EXP = Convert.ToUInt32(Line[3]);
                        EXP.Prestige = Convert.ToUInt32(Line[4]);
                        EXP.Unk5 = Convert.ToUInt32(Line[5]);
                        EXP.Unk6 = Convert.ToUInt32(Line[6]);
                        EXP.Unk7 = Convert.ToUInt32(Line[7]);
                        if (god_weapons_exp.EXP.ContainsKey(EXP.TYPE))
                        {
                            if (!god_weapons_exp.EXP[EXP.TYPE].ContainsKey(EXP.Level))
                                god_weapons_exp.EXP[EXP.TYPE].Add(EXP.Level, EXP);
                        }
                        else
                        {
                            god_weapons_exp.EXP.Add(EXP.TYPE, new Dictionary<uint, god_weapons_exp.EXPAstredge>());
                            god_weapons_exp.EXP[EXP.TYPE].Add(EXP.Level, EXP);
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine((object)ex);
            }
        }

    }
}