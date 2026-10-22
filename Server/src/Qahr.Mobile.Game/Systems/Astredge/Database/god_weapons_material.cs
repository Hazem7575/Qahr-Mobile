using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class god_weapons_material
    {
        [Flags]
        public enum TypeExp : uint
        {
            Quality = 1,
            ItemPlus = 2,
            SocketEquipment = 3,
            SocketWeapon = 4,
            Material = 5,
            EquipmentOneHandBouns = 6,
            TwoHandWeaponBouns = 7,
        }
        public static Dictionary<TypeExp, Dictionary<uint, MaterialAstredge>> Material;
        public class MaterialAstredge
        {
            public uint ID;
            public TypeExp TYPE;
            public uint TYPEEXP;
            public uint EXP;
        }
        public static bool TryGetValue(TypeExp id, uint ITEMID, out MaterialAstredge Items)
        {
            if (god_weapons_material.Material.ContainsKey(id) && god_weapons_material.Material[id].TryGetValue(ITEMID, out Items))
                return true;
            Items = (MaterialAstredge)null;
            return false;
        }
        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "god_weapons_material.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "god_weapons_material.txt");
                }
                else
                {
                    god_weapons_material.Material = new Dictionary<TypeExp, Dictionary<uint, MaterialAstredge>>();
                    foreach (string ReadAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "god_weapons_material.txt"))
                    {
                        string[] REntries = new string[2] { "@@", " " };
                        string[] Line = ReadAllLine.Split(REntries, StringSplitOptions.RemoveEmptyEntries);
                        MaterialAstredge Material = new MaterialAstredge();
                        Material.ID = Convert.ToUInt32(Line[0]);
                        Material.TYPE = (TypeExp)Convert.ToUInt32(Line[1]);
                        Material.TYPEEXP = Convert.ToUInt32(Line[2]);
                        Material.EXP = Convert.ToUInt32(Line[3]);
                        if (god_weapons_material.Material.ContainsKey(Material.TYPE))
                        {
                            if (!god_weapons_material.Material[Material.TYPE].ContainsKey(Material.TYPEEXP))
                                god_weapons_material.Material[Material.TYPE].Add(Material.TYPEEXP, Material);
                        }
                        else
                        {
                            god_weapons_material.Material.Add(Material.TYPE, new Dictionary<uint, god_weapons_material.MaterialAstredge>());
                            god_weapons_material.Material[Material.TYPE].Add(Material.TYPEEXP, Material);
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