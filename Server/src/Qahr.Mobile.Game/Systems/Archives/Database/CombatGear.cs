using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class CombatGear
    {
        public static Dictionary<uint, Dictionary<uint, Items>> combat;
        public class Items
        {
            public uint ID;
            public uint Type;
            public uint Level;
            public uint Exp;
            public uint MaxHP;
            public uint PAttack;
            public uint PDefense;
            public uint MDefense;
            public uint[] Skills;
            public uint BP;
            public uint Score;
        }
        public static bool TryGetValue(uint id, uint level, out CombatGear.Items type)
        {
            if (CombatGear.combat.ContainsKey(id) && CombatGear.combat[id].TryGetValue(level, out type))
                return true;
            type = (CombatGear.Items)null;
            return false;
        }
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "combat_gear.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "combat_gear.txt");
                }
                else
                {
                    CombatGear.combat = new Dictionary<uint, Dictionary<uint, CombatGear.Items>>();
                    foreach (string readAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "combat_gear.txt"))
                    {
                        string[] separator = new string[2] { "@@", " " };
                        string[] strArray = readAllLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        CombatGear.Items items = new CombatGear.Items();
                        items.ID = Convert.ToUInt32(strArray[0]);
                        items.Type = Convert.ToUInt32(strArray[1]);
                        items.Level = Convert.ToUInt32(strArray[2]);
                        items.Exp = Convert.ToUInt32(strArray[3]);
                        items.MaxHP = Convert.ToUInt32(strArray[4]);
                        items.PAttack = Convert.ToUInt32(strArray[5]);
                        items.PDefense = Convert.ToUInt32(strArray[6]);
                        items.MDefense = Convert.ToUInt32(strArray[7]);
                        items.Skills = new uint[9];
                        items.Skills[0] = Convert.ToUInt32(strArray[8]);
                        items.Skills[1] = Convert.ToUInt32(strArray[9]);
                        items.Skills[2] = Convert.ToUInt32(strArray[10]);
                        items.Skills[3] = Convert.ToUInt32(strArray[11]);
                        items.Skills[4] = Convert.ToUInt32(strArray[12]);
                        items.Skills[5] = Convert.ToUInt32(strArray[13]);
                        items.Skills[6] = Convert.ToUInt32(strArray[14]);
                        items.Skills[7] = Convert.ToUInt32(strArray[15]);
                        items.Skills[8] = Convert.ToUInt32(strArray[16]);
                        items.BP = Convert.ToUInt32(strArray[17]);
                        items.Score = Convert.ToUInt32(strArray[18]);
                        if (CombatGear.combat.ContainsKey(items.Type))
                        {
                            if (!CombatGear.combat[items.Type].ContainsKey(items.Level))
                                CombatGear.combat[items.Type].Add(items.Level, items);
                        }
                        else
                        {
                            CombatGear.combat.Add(items.Type, new Dictionary<uint, CombatGear.Items>());
                            CombatGear.combat[items.Type].Add(items.Level, items);
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
