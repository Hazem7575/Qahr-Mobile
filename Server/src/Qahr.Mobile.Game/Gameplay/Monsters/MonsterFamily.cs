using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgMonster
{
    public class MonsterFamily
    {
        public static Random Random = new Random();

        public int rest_secs = 0;

        public string Name;
        public uint UidMonster;
        public int MaxAttack, MinAttack;
        public int MaxHealth;
        public ushort Defense;
        public ushort Mesh;
        public ushort Level;
        public byte ViewRange;
        public sbyte AttackRange;
        public byte Dodge;
        public uint ID;
        public byte Boss;
        public int AttackSpeed;
        public int MoveSpeed;
        public uint SpellId;
        public int RespawnTime;
        public int Defense2;

        public byte DropBoots;
        public byte DropArmor;
        public byte DropShield;
        public byte DropWeapon;
        public byte DropArmet;
        public byte DropRing;
        public byte DropNecklace;
        public ushort DropMoney;
        public uint DropHPItem;
        public uint DropMPItem;

        public ushort SpawnX;
        public ushort SpawnY;
        public ushort MaxSpawnX;
        public ushort MaxSpawnY;
        public uint MapID;
        public byte SpawnCount;
        public int maxnpc;
        public int extra_battlelev;
        public int extra_exp;
        public int extra_damage;


        public uint ExtraCritical;
        public uint ExtraBreack;

        public SpecialItemWatcher[] DropSpecials;

        public MonsterSettings Settings;
        public MobItemGenerator ItemGenerator;
        private static Dictionary<uint, MobItemGenerator> ItemGeneratorLinker;

        static MonsterFamily()
        {
            ItemGeneratorLinker = new Dictionary<uint, MobItemGenerator>();
        }

        /// <summary>
        /// Creates the settings for this family based off the name
        /// </summary>
        public void CreateMonsterSettings()
        {

            Settings = MonsterSettings.Standard;
            if (Name.Contains("Guard") && !Name.Contains("Chaos") || Name.Contains("CitySoldier"))
                Settings = MonsterSettings.Guard;
            else if (Name.Contains("Reviver"))
                Settings = MonsterSettings.Reviver;
            else if (Name.Contains("King"))
                Settings = MonsterSettings.King;
            else if (Name.Contains("Messenger"))
                Settings = MonsterSettings.Messenger;


        }
        /// <summary>
        /// Creates, or obtains a item-generator instance by the ID variable of this generation.
        /// </summary>
        public void CreateItemGenerator()
        {
            if (!ItemGeneratorLinker.TryGetValue(ID, out ItemGenerator))
            {
                ItemGenerator = new MobItemGenerator(this);
                ItemGeneratorLinker.Add(ID, ItemGenerator);
            }
        }

        public MonsterFamily Copy()
        {
            MonsterFamily Mob = new MonsterFamily();

            Mob.Name = Name;
            Mob.UidMonster = UidMonster;
            Mob.MaxAttack = MaxAttack;
            Mob.MinAttack = MinAttack;
            Mob.MaxHealth = MaxHealth;
            Mob.Defense = Defense;
            Mob.Mesh = Mesh;
            Mob.Level = Level;
            Mob.ViewRange = ViewRange;
            Mob.AttackRange = AttackRange;
            Mob.Dodge = Dodge;
            Mob.ID = ID;
            Mob.DropBoots = DropBoots;
            Mob.DropArmor = DropArmor;
            Mob.DropShield = DropShield;
            Mob.DropWeapon = DropWeapon;
            Mob.DropRing = DropRing;
            Mob.DropNecklace = DropNecklace;
            Mob.DropMoney = DropMoney;
            Mob.DropHPItem = DropHPItem;
            Mob.DropMPItem = DropMPItem;
            Mob.Boss = Boss;
            Mob.Defense2 = Defense2;
            Mob.AttackSpeed = AttackSpeed;
            Mob.MoveSpeed = MoveSpeed;
            Mob.SpellId = SpellId;
            Mob.RespawnTime = Random.Next(10, 20);
            Mob.ExtraCritical = ExtraCritical;
            Mob.ExtraBreack = ExtraBreack;
            Mob.MapID = MapID;
            Mob.extra_battlelev = extra_battlelev;
            Mob.extra_damage = extra_damage;
            Mob.extra_exp = extra_exp;

            if (Mob.MaxHealth > 100000 && Mob.MaxHealth < 7000000 && Mob.Boss > 0)
            {
                Mob.AttackRange = 12;
                Mob.RespawnTime = 60 * 60;
            }
            if (Mob.ID == 20211)
            {
                Mob.RespawnTime = Random.Next(100, 200);
            }
            if (Mob.ID == 2700 || Mob.ID == 2699 || Mob.ID == 7022 || Mob.ID == 2164 || Mob.ID == 2165)
            {
                Mob.RespawnTime = rest_secs;
            }
            if (Mob.ID == 20055 || Mob.ID == 4171 || Mob.ID == 32070)
                Mob.RespawnTime = 60 * 60;

            Mob.DropSpecials = new SpecialItemWatcher[DropSpecials.Length];
            for (int x = 0; x < DropSpecials.Length; x++)
                Mob.DropSpecials[x] = DropSpecials[x];
            Mob.CreateItemGenerator();
            Mob.CreateMonsterSettings();
            return Mob;
        }
    }
}
