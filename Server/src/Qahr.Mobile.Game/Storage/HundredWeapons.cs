using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class HundredWeapons
    {
        public enum AttributeType : byte
        {
            None,
            Hitpoints,
            PhysicalAttack,
            PhysicalDefense,
            MagicAttack,
            MagicDefense
        }
        public static Dictionary<uint, HundredWeapon> HundredWeaponsList = new Dictionary<uint, HundredWeapon>();

        public class HundredWeapon
        {
            public Database.MagicType.WeaponsType WeaponSubtype;
            public byte Level;
            public byte WeaponProf;
            public uint Progress;
            public Dictionary<AttributeType, int> Attributes;
            public ushort MagicType;
            public byte ReqItemCount;
            public bool CanPolish;
        }

        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "hundred_weapon.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "hundred_weapon.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    HundredWeapon irc = new HundredWeapon();
                    irc.WeaponSubtype = (Database.MagicType.WeaponsType)Convert.ToUInt16(spilitline[1]);
                    irc.Level = Convert.ToByte(spilitline[2]);
                    irc.WeaponProf = Convert.ToByte(spilitline[3]);
                    irc.Progress = Convert.ToUInt32(spilitline[4]);

                    irc.Attributes = new Dictionary<AttributeType, int>();
                    for (int i = 0; i < 3; i++)
                        irc.Attributes.Add((AttributeType)(Convert.ToInt32(spilitline[5 + i]) / 10000), Convert.ToInt32(spilitline[5 + i]) % 10000);
                    irc.Attributes.Add(AttributeType.MagicAttack, 0);
                    irc.Attributes.Add(AttributeType.MagicDefense, 0);

                    irc.MagicType = Convert.ToUInt16(spilitline[8]);
                    irc.ReqItemCount = Convert.ToByte(spilitline[9]);
                    irc.CanPolish = Convert.ToByte(spilitline[10]) == 1;
                    HundredWeaponsList.Add(Convert.ToUInt32(spilitline[0]), irc);
                }
            }
        }
    }
}