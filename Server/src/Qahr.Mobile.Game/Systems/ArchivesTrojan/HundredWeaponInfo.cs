using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class HundredWeaponInfo
    {
        public Database.MagicType.WeaponsType WeaponSubtype;
        public byte Level;
        public uint Progress;
        public Dictionary<Database.HundredWeapons.AttributeType, int> Attributes;
        public BitVector32 BitVector;
        public byte AppearancePosition;
        public uint TampHitpoints;
        public uint TampPhysicalAttack;
        public uint TampPhysicalDefense;
        public uint Score
        {
            get
            {
                return (uint)(Level * 1000 + (Attributes.Values.Sum(i => i) + DBInfo.Attributes.Values.Sum(i => i)) * 5);
            }
        }
        public Database.HundredWeapons.HundredWeapon DBInfo
        {
            get
            {
                return Database.HundredWeapons.HundredWeaponsList.Values.Where(x => x.WeaponSubtype == WeaponSubtype && x.Level == Level).FirstOrDefault();
            }
        }
        public Database.MagicType.Magic DBSpell
        {
            get
            {
                return Pool.Magic[DBInfo.MagicType][(ushort)(Level + 1)];
            }
        }
    }
}
