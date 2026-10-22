using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.Database
{
    public class ProfessionTable
    {
        #region ProfLevelUP

        public static Dictionary<uint, ProfLevelUp> ProfLevelUPList = new Dictionary<uint, ProfLevelUp>();

        public class ProfLevelUp
        {
            public uint Class;
            public byte Level;
            public uint QuestID;
            public uint PrestigeScore;
            public uint EXP;
        }

        private static void LoadProfLevUP()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "prof_lev_up.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "prof_lev_up.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    ProfLevelUp prof = new ProfLevelUp();
                    prof.Class = Convert.ToUInt32(spilitline[1]);
                    prof.Level = Convert.ToByte(spilitline[2]);
                    prof.QuestID = Convert.ToUInt32(spilitline[3]);
                    prof.PrestigeScore = Convert.ToUInt32(spilitline[4]);
                    prof.EXP = Convert.ToUInt32(spilitline[5]);
                    ProfLevelUPList.Add(Convert.ToUInt32(spilitline[0]), prof);
                }
            }
        }
        #endregion

        #region Level Benefits

        public static Dictionary<uint, Benefit> Benefits = new Dictionary<uint, Benefit>();

        public class Benefit
        {
            public uint Class;
            public byte BattlePower;
            public AttributeValue[] AttributeValue;
            public uint PrestigeScore;
        }
        public class AttributeValue
        {
            public Game.MsgServer.MsgChiInfo.ChiAttribute Type;
            public uint Value;
        }
        private static void LoadProfBenefits()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "prof_lev_benefit.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "prof_lev_benefit.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    Benefit attr = new Benefit();
                    attr.Class = Convert.ToUInt32(spilitline[1]);
                    attr.BattlePower = Convert.ToByte(spilitline[2]);
                    attr.AttributeValue = new AttributeValue[4];
                    for (int i = 0; i < attr.AttributeValue.Length; i++)
                    {
                        attr.AttributeValue[i] = new AttributeValue();
                        attr.AttributeValue[i].Type = (Game.MsgServer.MsgChiInfo.ChiAttribute)(Convert.ToUInt32(spilitline[i + 3]) / 10000);
                        attr.AttributeValue[i].Value = Convert.ToUInt32(spilitline[i + 3]) % 10000;
                    }
                    attr.PrestigeScore = Convert.ToUInt32(spilitline[7]);
                    Benefits.Add(attr.Class, attr);
                }
            }
        }

        #endregion

        #region Title Benefits

        public static Dictionary<uint, TitleBenefit> TitleBenefits = new Dictionary<uint, TitleBenefit>();

        public class TitleBenefit
        {
            public byte Class;
            public byte Rank;
            public AttributeValue[] AttributeValue;
        }
        private static void LoadProfTitleBenefits()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "prof_title_benefit.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "prof_title_benefit.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    TitleBenefit attr = new TitleBenefit();
                    attr.Class = Convert.ToByte(spilitline[1]);
                    attr.Rank = Convert.ToByte(spilitline[2]);
                    attr.AttributeValue = new AttributeValue[4];
                    for (int i = 0; i < attr.AttributeValue.Length; i++)
                    {
                        attr.AttributeValue[i] = new AttributeValue();
                        attr.AttributeValue[i].Type = (Game.MsgServer.MsgChiInfo.ChiAttribute)(Convert.ToUInt32(spilitline[i + 3]) / 10000);
                        attr.AttributeValue[i].Value = Convert.ToUInt32(spilitline[i + 3]) % 10000;
                    }
                    TitleBenefits.Add(Convert.ToUInt32(spilitline[0]), attr);
                }
            }
        }

        #endregion

        public static void Load()
        {
            LoadProfBenefits();
            LoadProfLevUP();
            LoadProfTitleBenefits();
        }
    }
}