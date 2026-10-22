using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class InfoHeroReward : System.SafeDictionary<uint, InfoHeroReward.StageGoal>
    {
        public class StageGoal
        {
            public uint UID = 0;
            public uint OpenLev = 0;
            public uint CountGoals = 0;
            public string Prize = "";

            public System.SafeDictionary<uint, Item> ArrayItem = new System.SafeDictionary<uint, Item>();

            public bool CheckOpen(ushort level, byte reborn)
            {
                uint needlevel = OpenLev % 1000;
                uint needreborn = (OpenLev - needlevel) / 1000;

                return level >= needlevel && reborn >= needreborn;
            }
            public uint GetPrize()
            {
                return uint.Parse(Prize.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            public byte GetCount()
            {
                return byte.Parse(Prize.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[1]);
            }

            public class Item
            {
                public uint UID;
                public string Prize = "";
                public byte Progres;

                public uint GetPrize()
                {
                    return uint.Parse(Prize.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                }
                public byte GetCount()
                {
                    return byte.Parse(Prize.Split(new string[] { "@@"}, StringSplitOptions.RemoveEmptyEntries)[1]);
                }
            }
        }

        internal void LoadInformations()
        {
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("StageGoal.txt");

            int StageCount = Reader.ReadByte("StageGoal", "StageAmount", 0);

            const ushort FileSize = 100;

            for (uint x = 1; x <= StageCount; x++)
            {
                var Stage = new StageGoal()
                {
                    UID = x,
                    CountGoals = Reader.ReadByte(x.ToString(), "GoalNum", 0),
                    Prize = Reader.ReadString(x.ToString(), "Prize1","")
                };

                this.Add(x, Stage);

                for (uint i = 1; i <= Stage.CountGoals; i++)
                {
                    uint Key = x * FileSize + i;
                    string section = x.ToString() + "-" + i.ToString();
                    Stage.ArrayItem.Add(Key, new StageGoal.Item()
                     {
                         UID = Key,
                         Progres = Reader.ReadByte(section, "Progress", 0),
                         Prize = Reader.ReadString(section, "Prize1", "")
                     });
                }
            }
        }
    }
}
