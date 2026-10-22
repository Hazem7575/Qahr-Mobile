using Qahr.Mobile.Database;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;

namespace Qahr.Mobile
{
    public static class Gouyuxuzuotype
    {
        public class Items
        {
            public int Level;
            public int RequiredExp;
            public int Unknow2;
            public int Unknow3;
            public readonly List<Tuple<ushort, ushort>> Skills = new List<Tuple<ushort, ushort>>();
        }
        public static System.Collections.Generic.SafeDictionary<int, Items> gouyu_xuzuo_type = new System.Collections.Generic.SafeDictionary<int, Items>();
        public static void Load()
        {
            string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "gouyu_xuzuo_type.txt");
            foreach (var bas_line in baseText)
            {
                string[] line = bas_line.Split(new[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                Items obj = new Items();
                obj.Level = int.Parse(line[0]);
                obj.RequiredExp = int.Parse(line[1]);
                obj.Unknow2 = int.Parse(line[2]);
                obj.Unknow3 = int.Parse(line[3]);
                if (uint.Parse(line[4]) > 0)
                    obj.Skills.Add(new Tuple<ushort, ushort>((ushort)(uint.Parse(line[4]) / 1000), (ushort)(uint.Parse(line[4]) % 100)));
                if (uint.Parse(line[5]) > 0)
                    obj.Skills.Add(new Tuple<ushort, ushort>((ushort)(uint.Parse(line[5]) / 1000), (ushort)(uint.Parse(line[5]) % 100)));
                gouyu_xuzuo_type.Add(obj.Level, obj);
            }
        }
    }
}