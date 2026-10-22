using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class cq_syn_skill_type
    {
        public enum Type : uint
        {
            MaxHP = 1,
            PAttack = 2,
            PDefense = 3,
            MAttack = 4,
            MDefense = 5,
            FinalPAttack = 6,
            FinalPDamage = 7,
            FinalMAttack = 8,
            FinalMDamage = 9,
            PStrike = 10,
            MStrike = 11, 
            Immunity = 12, 
            Break = 13, 
            AntiBreak = 14, 
        }
        public class skill_type
        {
            public uint ID;
            public uint Type;
            public uint Level;
            public uint data1;
            public uint data2;
            public uint Silver;
            public uint DonationPoints;
        }
        public static Dictionary<uint, Dictionary<uint, cq_syn_skill_type.skill_type>> _syn_skill_type;
        public static bool TryGetValue(uint id, uint level, out cq_syn_skill_type.skill_type skill_type)
        {
            if (cq_syn_skill_type._syn_skill_type.ContainsKey(id) && cq_syn_skill_type._syn_skill_type[id].TryGetValue(level, out skill_type))
                return true;
            skill_type = (cq_syn_skill_type.skill_type)null;
            return false;
        }
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "syn_skill_type.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "syn_skill_type.txt");
                }
                else
                {
                    cq_syn_skill_type._syn_skill_type = new Dictionary<uint, Dictionary<uint, cq_syn_skill_type.skill_type>>();
                    foreach (string readAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "syn_skill_type.txt"))
                    {
                        string[] separator = new string[2] { "@@", " " };
                        string[] strArray = readAllLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        cq_syn_skill_type.skill_type skillType = new cq_syn_skill_type.skill_type();
                        skillType.ID = Convert.ToUInt32(strArray[0]);
                        skillType.Type = Convert.ToUInt32(strArray[1]);
                        skillType.Level = Convert.ToUInt32(strArray[2]);
                        skillType.data1 = Convert.ToUInt32(strArray[3]);
                        skillType.data2 = Convert.ToUInt32(strArray[4]);
                        skillType.Silver = Convert.ToUInt32(strArray[5]);
                        skillType.DonationPoints = Convert.ToUInt32(strArray[6]);
                        if (cq_syn_skill_type._syn_skill_type.ContainsKey(skillType.Type))
                        {
                            if (!cq_syn_skill_type._syn_skill_type[skillType.Type].ContainsKey(skillType.Level))
                                cq_syn_skill_type._syn_skill_type[skillType.Type].Add(skillType.Level, skillType);
                        }
                        else
                        {
                            cq_syn_skill_type._syn_skill_type.Add(skillType.Type, new Dictionary<uint, cq_syn_skill_type.skill_type>());
                            cq_syn_skill_type._syn_skill_type[skillType.Type].Add(skillType.Level, skillType);
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
