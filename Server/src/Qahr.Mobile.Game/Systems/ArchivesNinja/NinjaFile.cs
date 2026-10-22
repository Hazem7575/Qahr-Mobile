using Qahr.Mobile.Database;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;

namespace Qahr.Mobile
{
    public class NinjaFile
    {
        
      
        public static Dictionary<uint, NinjaSprintItem> gouyu_type;
        public static Dictionary<uint, Item> gouyu_immortals;
    
        public class NinjaSprintItem
        {
            public uint ID;
            public uint ItemID;
            public uint Level;
            public uint Exp;
            public uint UK1;
            public uint UK2;
            public uint magic1;
            public uint magic2;
            public int Power;
            public uint Damage;
            public uint Score;
            public uint UK5;
        }

        public class Item
        {
            public uint Level;
            public uint U1;
            public uint MaxHP;
            public uint PAttack;
            public uint PDefense;
            public uint MDefense;
            public uint Chance;
            public uint Seconds;
            public uint Rate;//8 9 10 11
            public uint MovementSpeed;
            public uint U13;
            public uint U14;
            public uint P;
        }

        public static void Load()
        {
            gouyu_type = new Dictionary<uint, NinjaSprintItem>();
            if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "gouyu_type.txt"))
            {
                Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "gouyu_type.txt");
                return;
            }
            var str = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "gouyu_type.txt");
            foreach (string Line in str)
            {
                string[] Data = Line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                NinjaSprintItem Item = new NinjaSprintItem();
                Item.ID = Convert.ToUInt32(Data[0]);
                Item.ItemID = Convert.ToUInt32(Data[1]);
                Item.Level = Convert.ToUInt32(Data[2]);
                Item.Exp = Convert.ToUInt32(Data[3]);
                Item.UK1 = Convert.ToUInt32(Data[4]);
                Item.UK2 = Convert.ToUInt32(Data[5]);
                Item.magic1 = Convert.ToUInt32(Data[6]);
                Item.magic2 = Convert.ToUInt32(Data[7]);
                Item.Power = Convert.ToInt32(Data[8]);
                Item.Damage = Convert.ToUInt32(Data[9]);
                Item.Score = Convert.ToUInt32(Data[11]);
                Item.UK5 = Convert.ToUInt32(Data[11]);
                gouyu_type.Add(Item.ID, Item);
            }
            gouyu_immortals = new Dictionary<uint, Item>();
            if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "gouyu_immortal.txt"))
            {
                Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "gouyu_immortal.txt");
                return;
            }
            str = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "gouyu_immortal.txt");
            foreach (string Line in str)
            {
                string[] Data = Line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                Item Item = new Item();
                Item.Level = Convert.ToUInt32(Data[0]);
                Item.U1 = Convert.ToUInt32(Data[1]);
                Item.MaxHP = Convert.ToUInt32(Data[2]);
                Item.PAttack = Convert.ToUInt32(Data[3]);
                Item.PDefense = Convert.ToUInt32(Data[4]);
                Item.MDefense = Convert.ToUInt32(Data[5]);
                Item.Chance = Convert.ToUInt32(Data[6]);
                Item.Seconds = Convert.ToUInt32(Data[7]);
                Item.Rate = Convert.ToUInt32(Data[8]);
                Item.MovementSpeed = Convert.ToUInt32(Data[12]);
                Item.U13 = Convert.ToUInt32(Data[13]);
                Item.U14 = Convert.ToUInt32(Data[14]);
                Item.P = Convert.ToUInt32(Data[15]);
                gouyu_immortals.Add(Item.Level, Item);
            }
        }
    }
}