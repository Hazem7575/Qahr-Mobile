using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.Database
{
    public class RuneTable
    {
        #region RuneLevelEXP

        public static Dictionary<uint, RuneLevelEXP> RuneLevelEXPList = new Dictionary<uint, RuneLevelEXP>();

        public class RuneLevelEXP
        {
            public uint ItemType;
            public byte Level;
            public uint RequiredProgress;
        }

        public static void LoadRuneEXP()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "rune_levexp.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "rune_levexp.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, System.StringSplitOptions.RemoveEmptyEntries);
                    var irc = new RuneLevelEXP()
                    {
                        ItemType = System.Convert.ToUInt32(spilitline[1]),
                        Level = System.Convert.ToByte(spilitline[2]),
                        RequiredProgress = System.Convert.ToUInt32(spilitline[3]),
                    };
                    RuneLevelEXPList.Add(System.Convert.ToUInt32(spilitline[0]), irc);
                }
            }
        }



        public static uint GetRunePlusPoints(uint value)
        {
            var Type = value / 1000;
            Type = Convert.ToUInt32(Type.ToString().Substring(0, 3));
            if (RuneLevelEXPList.Values.Where(i => i.ItemType == Type).Count() > 0) return RuneLevelEXPList.Values.Where(i => i.ItemType == Type).FirstOrDefault().RequiredProgress;
            if (value >= 730001 && value <= 730008) return Database.ItemType.StonePlusPoints((byte)(value % 10));
            return 0;
        }
        public static uint GetRunePlusPoints(Game.MsgServer.MsgGameItem item)
        {
            var value = item.ITEM_ID;
            var Type = value / 1000;
            Type = Convert.ToUInt32(Type.ToString().Substring(0, 3));
            if (item.StackSize == 0) item.StackSize = 1;

            if (RuneLevelEXPList.Values.Where(i => i.ItemType == Type && i.Level == (item.ITEM_ID % 100)).Count() > 0)
                return (uint)(RuneLevelEXPList.Values.Where(i => i.ItemType == Type && i.Level == (item.ITEM_ID % 100)).FirstOrDefault().RequiredProgress * item.StackSize) + item.RuneEXP;

            return 0;
        }
        #endregion RuneLevelEXP

        #region RuneAttributes

        public static System.Collections.Generic.Dictionary<uint, Attribute> Attributes = new System.Collections.Generic.Dictionary<uint, Attribute>();


        public class Attribute
        {
            public uint ID;
            public uint Score;
            public RuneAttribute Type;
            public uint Value;
        }

        public enum RuneAttribute
        {
            None,
            HPAdd,
            AddAttack,
            AddMagicAttack,
            CriticalStrike = 10,
            SkillCriticalStrike,
            Immunity,
            Breakthrough,
            Counteraction,
        }
        public static void LoadRuneAttributes()
        {

            if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "rune_storage_attr.txt"))
            {
                System.Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "rune_storage_attr.txt");
                return;
            }
            Attributes = new System.Collections.Generic.Dictionary<uint, Attribute>();
            var str = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "rune_storage_attr.txt");
            foreach (string Line in str)
            {
                string[] Data = Line.Split(new string[] { "@@", " " }, System.StringSplitOptions.RemoveEmptyEntries);
                var obj = new Attribute();
                obj.ID = System.Convert.ToUInt32(Data[0]);
                obj.Score = System.Convert.ToUInt32(Data[1]);
                obj.Type = (RuneAttribute)System.Convert.ToUInt32(Data[2]);
                obj.Value = System.Convert.ToUInt32(Data[3]);
                Attributes.Add(obj.ID, obj);
            }
        }

        #endregion RuneLevelEXP

        #region xuanbao_compose_attr_limit
        public static List<compose_limit> composelimit = new List<compose_limit>();
        public class compose_limit
        {
            public Game.MsgServer.MsgChiInfo.ChiAttribute Attribute;
            public uint Min, Max;
        }
        public static void Loadcomposelimit()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "xuanbao_compose_attr_limit.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "xuanbao_compose_attr_limit.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    compose_limit irc = new compose_limit();
                    irc.Attribute = (Game.MsgServer.MsgChiInfo.ChiAttribute)Convert.ToByte(spilitline[0]);
                    irc.Min = Convert.ToUInt32(spilitline[2]);
                    irc.Max = Convert.ToUInt32(spilitline[3]);
                    composelimit.Add(irc);
                }
            }
        }
        #endregion
        #region xuanbao_addition_attr
        public static List<xuanbao_addition_attr> xuanbaoadditionattr = new List<xuanbao_addition_attr>();
        public class xuanbao_addition_attr
        {
            public uint Id;
            public byte Type;
            public byte Num;
            public int Value;
            public int Score;
        }
        public static void Loadxuanbaoadditionattr()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "xuanbao_addition_attr.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "xuanbao_addition_attr.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    xuanbao_addition_attr irc = new xuanbao_addition_attr();
                    irc.Id = Convert.ToUInt32(spilitline[0]);
                    irc.Type = Convert.ToByte(spilitline[1]);
                    irc.Num = Convert.ToByte(spilitline[2]);
                    irc.Value = Convert.ToInt32(spilitline[3]);
                    irc.Score = Convert.ToInt32(spilitline[4]);
                    xuanbaoadditionattr.Add(irc);
                }
            }
        }
        #endregion
        #region xuanbao_rand_attr

        public static List<RandomAttribute> RandomAttributes = new List<RandomAttribute>();

        public class RandomAttribute
        {
            public uint ItemID;
            public Game.MsgServer.MsgChiInfo.ChiAttribute Attribute;
            public uint Min, Max;
            public bool dwParam;
        }

        public static void LoadRandoms()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "xuanbao_rand_attr.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "xuanbao_rand_attr.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    RandomAttribute irc = new RandomAttribute();
                    irc.ItemID = Convert.ToUInt32(spilitline[0]);
                    irc.Attribute = (Game.MsgServer.MsgChiInfo.ChiAttribute)Convert.ToByte(spilitline[1]);
                    irc.Min = Convert.ToUInt32(spilitline[2]);
                    irc.Max = Convert.ToUInt32(spilitline[3]);
                    if (irc.Max % 10 == 1)
                    {
                        irc.Max--;
                        irc.dwParam = true;
                    }
                    RandomAttributes.Add(irc);
                }
            }
        }

        #endregion RuneLevelEXP


        public static void Load()
        {
            LoadRuneAttributes();
            LoadRuneEXP();
            Loadxuanbaoadditionattr();
            Loadcomposelimit();
            LoadRandoms();
        }
    }
}