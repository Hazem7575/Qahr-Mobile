using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class FateExpTable
    {
        public static Dictionary<uint, Value> Values = new Dictionary<uint, Value>();

        public class Value
        {
            public uint Index;
            public Game.MsgServer.MsgChiInfo.ChiPowerType ChiType;
            public byte Level;
            public uint RequiredPoints;
            public Role.Instance.Chi.ChiAttribute[] Attributes;
            public uint MinScore;
            public uint MDefense;
            public uint MaxChiPoints;
            public uint StudyLuck;
            public uint DailyFreePoints;
            public uint CostReduction;
            public uint PrizeItemID;
        }

        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "fate_exp.txt"))
            {
                string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "fate_exp.txt");
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    Value attr = new Value();
                    attr.Index = uint.Parse(spilitline[0]);
                    attr.ChiType = (Game.MsgServer.MsgChiInfo.ChiPowerType)byte.Parse(spilitline[1]);
                    attr.Level = byte.Parse(spilitline[2]);
                    attr.RequiredPoints = uint.Parse(spilitline[3]);
                    attr.Attributes = new Role.Instance.Chi.ChiAttribute[4];
                    for (int i = 0; i < attr.Attributes.Length; i++)
                        attr.Attributes[i] = new Role.Instance.Chi.ChiAttribute((Game.MsgServer.MsgChiInfo.ChiAttribute)(uint.Parse(spilitline[4 + i]) / 10000), (ushort)(uint.Parse(spilitline[4 + i]) % 10000), false);
                    attr.MinScore = uint.Parse(spilitline[8]);
                    attr.MDefense = uint.Parse(spilitline[9]);
                    attr.CostReduction = uint.Parse(spilitline[10]) / 100;
                    attr.StudyLuck = uint.Parse(spilitline[11]);
                    attr.DailyFreePoints = uint.Parse(spilitline[12]);
                    attr.MaxChiPoints = uint.Parse(spilitline[13]);
                    attr.PrizeItemID = uint.Parse(spilitline[14]);
                    Values.Add(attr.Index, attr);
                }
            }
        }
    }
}