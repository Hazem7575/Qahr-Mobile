using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile
{
    public class daoqi_dict_type
    {
        public static Dictionary<uint, Item> Items;
        public static Dictionary<uint, Item> PirateRune;
        public class Item
        {
            public uint ID;
            public Type Type;
            public uint attr_type1;
            public uint attr_value1;
            public uint attr_type2;
            public uint attr_value2;
            public List<attr> Attributes = new List<attr>();
            public uint score;
            public uint points;
            public uint rate;
            public uint TypeRune;
            public uint Level;
            public uint Exp;
            public uint u12;
            public uint u13;
            public uint TypeCombatGear;
        }
        public class attr
        {
            public uint Type;
            public uint Value;
        }
        [Flags]
        public enum Type : uint
        {
            Refined,
            Elite,
            Super,
            Sacred
        }
        [Flags]
        public enum AttrType : uint
        {
            None = 0,
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
            Parry = 15,
            LuckyStrike = 16,
            CoreStrike = 17,
            InvisibleArrow = 18,
            DrainingTouch = 19,
            BloodSpawn = 20,
            KillingFlash = 21,
            MirrorOfSin = 22,
            MaxMP = 23,
            Spell = 24,
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "daoqi_dict_type.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "daoqi_dict_type.txt");
                    return;
                }
                Items = new Dictionary<uint, Item>();
                PirateRune = new Dictionary<uint, Item>();
                var str = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "daoqi_dict_type.txt");
                foreach (string Line in str)
                {
                    string[] Data = Line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    Item obj = new Item
                    {
                        ID = Convert.ToUInt32(Data[0]),
                        Type = (Type)Convert.ToUInt32(Data[1]),
                        attr_type1 = Convert.ToUInt32(Data[2]),
                        attr_value1 = Convert.ToUInt32(Data[3]),
                        attr_type2 = Convert.ToUInt32(Data[4]),
                        attr_value2 = Convert.ToUInt32(Data[5]),
                        score = Convert.ToUInt32(Data[6]),
                        points = Convert.ToUInt32(Data[7]),
                        rate = Convert.ToUInt32(Data[8]),
                        TypeRune = Convert.ToUInt32(Data[9]),
                        Level = Convert.ToUInt32(Data[10]),
                        Exp = Convert.ToUInt32(Data[11]),
                        u12 = Convert.ToUInt32(Data[12]),
                        u13 = Convert.ToUInt32(Data[13]),
                        TypeCombatGear = Convert.ToUInt32(Data[14])
                    };
                    obj.Attributes.Add(new attr() { Type = obj.attr_type1, Value = obj.attr_value1 });
                    obj.Attributes.Add(new attr() { Type = obj.attr_type2, Value = obj.attr_value2 });
                    Items.Add((uint)obj.ID, obj);
                    if (obj.ID == 1007
                               || obj.ID == 2007
                               || obj.ID == 3007
                               || obj.ID == 5007
                               || obj.ID == 6007
                               || obj.ID == 7007
                               || obj.ID == 8007
                               || obj.ID == 9007
                               || obj.ID == 10007
                               || obj.ID == 11007
                               || obj.ID == 12007
                               || obj.ID == 13007
                               || obj.ID == 14007
                               || obj.ID == 15007
                               || obj.ID == 16007
                               || obj.ID == 17007
                               || obj.ID == 18007
                               || obj.ID == 19007
                               || obj.ID == 20007
                               || obj.ID == 21007
                               || obj.ID == 22007
                               || obj.ID == 23007
                               || obj.ID == 24007
                               || obj.ID == 25007
                               || obj.ID == 26007)
                    {
                        PirateRune.Add((uint)obj.ID, obj);
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
