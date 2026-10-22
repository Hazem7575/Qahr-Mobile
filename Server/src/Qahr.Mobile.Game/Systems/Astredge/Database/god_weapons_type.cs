using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class god_weapons_type
    {
        public static Dictionary<uint, Dictionary<uint, ItemsAstredge>> Astredge;
        public class ItemsAstredge
        {
            public uint ID;
            public byte Level;
            public uint[] SkillFirst;
            public uint[] SkillMele;
            public uint Unk;
            public uint Status;
            public uint Unk2;
            public uint AstredgeAttack;
        
        }
        public static bool TryGetValue(uint id,byte level, out ItemsAstredge Items)
        {
            if (god_weapons_type.Astredge.ContainsKey(id) && god_weapons_type.Astredge[id].TryGetValue(level, out Items))
                return true;
            Items = (ItemsAstredge)null;
            return false;
        }
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "god_weapons_type.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "god_weapons_type.txt");
                }
                else
                {
                    god_weapons_type.Astredge = new Dictionary<uint, Dictionary<uint, ItemsAstredge>>();
                    foreach (string ReadAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "god_weapons_type.txt"))
                    {
                        string[] REntries = new string[2] { "@@", " " };
                        string[] Line = ReadAllLine.Split(REntries, StringSplitOptions.RemoveEmptyEntries);
                        ItemsAstredge ITEMAstredge = new ItemsAstredge();
                        ITEMAstredge.ID = Convert.ToUInt32(Line[0]);
                        ITEMAstredge.Level = Convert.ToByte(Line[1]);
                        ITEMAstredge.SkillFirst = new uint[2];
                        ITEMAstredge.SkillFirst[0] = Convert.ToUInt32(Line[2]);
                        ITEMAstredge.SkillFirst[1] = Convert.ToUInt32(Line[3]);
                        ITEMAstredge.SkillMele = new uint[2];
                        ITEMAstredge.SkillMele[0] = Convert.ToUInt32(Line[4]);
                        ITEMAstredge.SkillMele[1] = Convert.ToUInt32(Line[5]);
                        ITEMAstredge.Unk = Convert.ToUInt32(Line[6]);
                        ITEMAstredge.Status = Convert.ToUInt32(Line[7]);
                        ITEMAstredge.Unk2 = Convert.ToUInt32(Line[8]);
                        ITEMAstredge.AstredgeAttack = Convert.ToUInt32(Line[9]);
                        if (god_weapons_type.Astredge.ContainsKey(ITEMAstredge.ID))
                        {
                            if (!god_weapons_type.Astredge[ITEMAstredge.ID].ContainsKey(ITEMAstredge.Level))
                                god_weapons_type.Astredge[ITEMAstredge.ID].Add(ITEMAstredge.Level, ITEMAstredge);
                        }
                        else
                        {
                            god_weapons_type.Astredge.Add(ITEMAstredge.ID, new Dictionary<uint, god_weapons_type.ItemsAstredge>());
                            god_weapons_type.Astredge[ITEMAstredge.ID].Add(ITEMAstredge.Level, ITEMAstredge);
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