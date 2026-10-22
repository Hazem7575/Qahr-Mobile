using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgMonster
{
    public class MobRateWatcher
    {
        private int tick;
        private int count;
        /// <summary>[FileDrops] the "every N" of this counter (read by FileDrops for [SpecialDrop]).</summary>
        public int Tick { get { return tick; } }
        public static implicit operator bool(MobRateWatcher q)
        {
            // [FileDrops] atomic: kills run on several threads; the old count++ / == could skip past tick and never fire again
            if (q == null || q.tick <= 0) return false;
            int c = System.Threading.Interlocked.Increment(ref q.count) & int.MaxValue;
            return c % q.tick == 0;
        }
        public MobRateWatcher(int Tick)
        {
            tick = Tick;
            count = 0;
        }
    }

    public struct SpecialItemWatcher
    {
        public uint ID;
        public MobRateWatcher Rate;
        public SpecialItemWatcher(uint ID, int Tick)
        {
            this.ID = ID;
            Rate = new MobRateWatcher(Tick);
        }
    }
    public class MobItemGenerator
    {
        private static ushort[] NecklaceType = new ushort[] { 120, 121 };
        private static ushort[] RingType = new ushort[] { 150, 151 };
        private static ushort[] ArmetType = new ushort[] { 111, 112, 113, 114, 117, 118 };
        private static ushort[] ArmorType = new ushort[] { 130, 131, 132, 133, 134 };
        private static ushort[] OneHanderType = new ushort[] { 410, 420, 421, 430, 440, 450, 460, 480, 481, 490, 500, 601 };
        private static ushort[] TwoHanderType = new ushort[] { 510, 530, 560, 561, 580, 900, };
        private static uint[] SeaPotions = new uint[] { 3004230, 3004231, 3004232, 3004233, 3004234, 3004235, 3004236, 3004237, 3004238 };
        private MonsterFamily Family;
        private MobRateWatcher Refined;
        private MobRateWatcher Unique;
        private MobRateWatcher Elite;
        private MobRateWatcher Super;
        private MobRateWatcher PlusOne;
        private MobRateWatcher PlusTwo;
        
        private MobRateWatcher DropHp;
        private MobRateWatcher DropMp;
        private MobRateWatcher Chi100;
        private MobRateWatcher Study20;
        private MobRateWatcher Chi300;
        private MobRateWatcher Bomb;
        private MobRateWatcher LuckyAmulet;
        private MobRateWatcher MoonBox;


        private MobRateWatcher SoulAroma;
        private MobRateWatcher Moss;
        private MobRateWatcher DreamGras;
        private MobRateWatcher BlueEgg;//710062
        private MobRateWatcher PinkEgg;//710063
        private MobRateWatcher DropSpecialPotions;
        private MobRateWatcher CuteCPPack;
        private MobRateWatcher BloodyJadesFragments;
        private MobRateWatcher IvoryJadesFragments;
        private MobRateWatcher Letter14;//3303131
        private MobRateWatcher LetterC;//3303124
        private MobRateWatcher LetterO;//3303125
        private MobRateWatcher LetterN;//3303126
        private MobRateWatcher LetterQ;//3303127
        private MobRateWatcher LetterU;//3303128
        private MobRateWatcher LetterE;//3303129
        private MobRateWatcher LetterR;//3303130
        private MobRateWatcher NormalCPPack;
        private MobRateWatcher SmallCPPack;
        private MobRateWatcher SweetyCPPack;
        private MobRateWatcher BigCPPack;
        private MobRateWatcher HugeCPPack;
        private MobRateWatcher TitanCPsBag;

        public MobItemGenerator(MonsterFamily family)
        {
            Family = family;
            Refined = new MobRateWatcher(10);
            Unique = new MobRateWatcher(20);
            Elite = new MobRateWatcher(25);
            Super = new MobRateWatcher(30);
            Letter14 = new MobRateWatcher(150);
            LetterC = new MobRateWatcher(19);
            LetterO = new MobRateWatcher(18);
            LetterN = new MobRateWatcher(17);
            LetterQ = new MobRateWatcher(16);
            LetterU = new MobRateWatcher(15);
            LetterE = new MobRateWatcher(14);
            LetterR = new MobRateWatcher(20);
            PlusOne = new MobRateWatcher(12);
            PlusTwo = new MobRateWatcher(11);
            
            DropHp = new MobRateWatcher(50);
            DropMp = new MobRateWatcher(50);
            DropSpecialPotions = new MobRateWatcher(50);
            LuckyAmulet = new MobRateWatcher(30);
            BloodyJadesFragments = new MobRateWatcher(30);
            IvoryJadesFragments = new MobRateWatcher(30);
            Chi100 = new MobRateWatcher(50);
            Chi300 = new MobRateWatcher(90);
            MoonBox = new MobRateWatcher(50);
            Study20 = new MobRateWatcher(40);
            Bomb = new MobRateWatcher(70);
            CuteCPPack = new MobRateWatcher(20);
            Moss = new MobRateWatcher(20);
            SoulAroma = new MobRateWatcher(20);
            DreamGras = new MobRateWatcher(20);
            BlueEgg = new MobRateWatcher(30);
            PinkEgg = new MobRateWatcher(30);
            SmallCPPack = new MobRateWatcher(100);
            NormalCPPack = new MobRateWatcher(100);
            SweetyCPPack = new MobRateWatcher(100);
            BigCPPack = new MobRateWatcher(100);
            HugeCPPack = new MobRateWatcher(100);
            TitanCPsBag = new MobRateWatcher(100);
        }
        public uint GeneratePotionExtra(bool Special = false)
        {
            if (Special)
            {
                return SeaPotions[Program.GetRandom.Next(0, SeaPotions.Length)];
            }

            if (DropSpecialPotions)
            {
                return SeaPotions[Program.GetRandom.Next(0, SeaPotions.Length)];
            }
            return 0;
        }
        public List<uint> GenerateSoulsItems(ushort level)
        {
            List<uint> items = new List<uint>();
            ushort rand = (ushort)(Program.GetRandom.Next() % 1000);
            byte count = (byte)(rand % 3);
            if (Database.ItemType.PurificationItems.ContainsKey(level))
            {
                var array = Database.ItemType.PurificationItems[level].Values.ToArray();
                for (int x = 0; x < (int)(count == 0 ? 1 : count); x++)
                {
                    int position = Program.GetRandom.Next(0, array.Length);
                    items.Add(array[position].ID);
                }
            }
            //genereate accessory
            var Accessorys = Database.ItemType.Accessorys.Values.ToArray();
            rand = (ushort)(Program.GetRandom.Next() % 1000);
            count = (byte)(rand % 3);
            for (int x = 0; x < count; x++)
            {
                int position = Program.GetRandom.Next(0, Accessorys.Length);
                items.Add(Accessorys[position].ID);
            }
            //--------------------------

            if (level <= 3)
                items.Add(723341);//20 study points
            else if (level > 3)
                items.Add(723342);//500 study points

            return items;
        }
        public List<uint> GenerateBossFamily()
        {
            List<uint> Items = new List<uint>();
            byte rand = (byte)Program.GetRandom.Next(1, 7);
            for (int x = 0; x < 1; x++)
            {
                byte dwItemQuality = 9;
                uint dwItemSort = 0;
                uint dwItemLev = 0;
                switch (rand)
                {
                    case 1:
                        {
                            dwItemSort = NecklaceType[Program.GetRandom.Next(0, NecklaceType.Length)];
                            dwItemLev = Family.DropNecklace;
                            break;
                        }
                    case 2:
                        {
                            dwItemSort = RingType[Program.GetRandom.Next(0, RingType.Length)];
                            dwItemLev = Family.DropRing;
                            break;
                        }
                    case 3:
                        {
                            dwItemSort = ArmorType[Program.GetRandom.Next(0, ArmorType.Length)];
                            dwItemLev = Family.DropArmor;
                            break;
                        }
                    case 4:
                        {
                            dwItemSort = TwoHanderType[Program.GetRandom.Next(0, TwoHanderType.Length)];
                            dwItemLev = ((dwItemSort == 900) ? Family.DropShield : Family.DropWeapon);
                            break;
                        }
                    default:
                        {
                            dwItemSort = OneHanderType[Program.GetRandom.Next(0, OneHanderType.Length)];
                            dwItemLev = Family.DropWeapon;
                            break;
                        }
                }
                dwItemLev = AlterItemLevel(dwItemLev, dwItemSort);
                uint idItemType = (dwItemSort * 1000) + (dwItemLev * 10) + dwItemQuality;
                if (Pool.ItemsBase.ContainsKey(idItemType))
                    Items.Add(idItemType);
            }
            return Items;
        }
        
        #region QuestDrop
        public uint GenerateItemId(uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            foreach (SpecialItemWatcher sp in Family.DropSpecials)
            {
                if (sp.Rate)
                {
                    Special = true;
                    dwItemQuality = (byte)(sp.ID % 10);
                    if (Pool.ItemsBase.TryGetValue(sp.ID, out DbItem))
                        return sp.ID;
                }
            }
            
            
            if (Chi100)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(729476, out DbItem))
                    return 729476;
            }
            if (Study20)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(723341, out DbItem))
                    return 723341;
            }           
            if (map == 1001 || Role.GameMap.IsFrozengrotoMaps(map))
            {
                if (Chi300)
                {
                    dwItemQuality = 0;
                    Special = true;
                    if (Pool.ItemsBase.TryGetValue(729478, out DbItem))
                        return 729478;
                }
               
               

            }
            dwItemQuality = GenerateQuality();
            uint dwItemSort = 0;
            uint dwItemLev = 0;

            int nRand = BaseFunc.RandGet(1200, false);
            if (nRand >= 0 && nRand < 20) // 0.17%
            {
                
            }
            else if (nRand >= 20 && nRand < 50) // 0.25%
            {
               
            }
            else if (nRand >= 50 && nRand < 100) // 4.17%
            {
                
            }
            else if (nRand >= 100 && nRand < 400) // 25%
            {
                
            }
            else if (nRand >= 400 && nRand < 700) // 25%
            {
               
            }
           
            if (dwItemLev != 99)
            {
               
            }
            DbItem = null;
            return 0;
        }
        public uint GenerateItem2(uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            foreach (SpecialItemWatcher sp in Family.DropSpecials)
            {
                if (sp.Rate)
                {
                    Special = true;
                    dwItemQuality = (byte)(sp.ID % 10);
                    if (Pool.ItemsBase.TryGetValue(sp.ID, out DbItem))
                        return sp.ID;
                }
            }
            if (Moss)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(722723, out DbItem))
                    return 722723;
            }
            if (SoulAroma)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(722724, out DbItem))
                    return 722724;
            }
            if (DreamGras)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(722725, out DbItem))
                    return 722725;
            }
            dwItemQuality = GenerateQuality();
            DbItem = null;
            return 0;
        }
        public uint GenerateItem1(uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            foreach (SpecialItemWatcher sp in Family.DropSpecials)
            {
                
            }
            if (PinkEgg)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(711726, out DbItem))
                    return 711726;
            }
            
            dwItemQuality = GenerateQuality();
            DbItem = null;
            return 0;
        }
        public uint GenerateItem3(uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            foreach (SpecialItemWatcher sp in Family.DropSpecials)
            {
                if (sp.Rate)
                {
                    Special = true;
                    dwItemQuality = (byte)(sp.ID % 10);
                    if (Pool.ItemsBase.TryGetValue(sp.ID, out DbItem))
                        return sp.ID;
                }
            }
            if (BlueEgg)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(710062, out DbItem))
                    return 710062;
            }
            dwItemQuality = GenerateQuality();
            DbItem = null;
            return 0;
        }
        #endregion
        public uint GenerateItem(uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            foreach (SpecialItemWatcher sp in Family.DropSpecials)
            {
                if (sp.Rate)
                {
                    Special = true;
                    dwItemQuality = (byte)(sp.ID % 10);
                    if (Pool.ItemsBase.TryGetValue(sp.ID, out DbItem))
                        return sp.ID;
                }
            }

            if (DropHp)
            {
                dwItemQuality = 0;
                Special = true;
                if (Pool.ItemsBase.TryGetValue(Family.DropHPItem, out DbItem))
                    return Family.DropHPItem;
            }
            if (DropMp)
            {
                dwItemQuality = 0;
                Special = true; if (Pool.ItemsBase.TryGetValue(Family.DropMPItem, out DbItem))
                    return Family.DropMPItem;
            }
            dwItemQuality = GenerateQuality();
            uint dwItemSort = 0;
            uint dwItemLev = 0;

            int nRand = BaseFunc.RandGet(1200, false);
            if (nRand >= 0 && nRand < 20) 
            {
                dwItemSort = 160;
                dwItemLev = Family.DropBoots;
            }
            else if (nRand >= 20 && nRand < 50) 
            {
                dwItemSort = NecklaceType[BaseFunc.RandGet(NecklaceType.Length, false)];
                dwItemLev = Family.DropNecklace;
            }
            else if (nRand >= 50 && nRand < 100) 
            {
                dwItemSort = RingType[BaseFunc.RandGet(RingType.Length, false)];
                dwItemLev = Family.DropRing;
            }
            else if (nRand >= 100 && nRand < 400) 
            {
                dwItemSort = ArmetType[BaseFunc.RandGet(ArmetType.Length, false)];
                dwItemLev = Family.DropArmet;
            }
            else if (nRand >= 400 && nRand < 700)
            {
                dwItemSort = ArmorType[BaseFunc.RandGet(ArmorType.Length, false)];
                dwItemLev = Family.DropArmor;
            }
            else 
            {
                int nRate = BaseFunc.RandGet(100, false);
                if (nRate >= 0 && nRate < 20) 
                {
                    dwItemSort = 421;
                }
                else if (nRate >= 40 && nRate < 80)	
                {
                    dwItemSort = OneHanderType[BaseFunc.RandGet(OneHanderType.Length, false)];
                    dwItemLev = Family.DropWeapon;
                }
                else if (nRand >= 80 && nRand < 100)
                {
                    dwItemSort = TwoHanderType[BaseFunc.RandGet(TwoHanderType.Length, false)];
                    dwItemLev = ((dwItemSort == 900) ? Family.DropShield : Family.DropWeapon);
                }
            }
       

            if (dwItemLev != 99)
            {
                dwItemLev = AlterItemLevel(dwItemLev, dwItemSort);

                uint idItemType = (dwItemSort * 1000) + (dwItemLev * 10) + dwItemQuality;
              
                if (Pool.ItemsBase.TryGetValue(idItemType, out DbItem))
                {
                    ushort position = Database.ItemType.ItemPosition(idItemType);
                    byte level = Database.ItemType.ItemMaxLevel((Role.Flags.ConquerItem)position);
                    if (DbItem.Level > level)
                        return 0;
                    return idItemType;
                }
            }
            DbItem = null;
            return 0;
        }
       

        /// <summary>
        /// [FileDrops] the file drop table without [SpecialDrop] (FileDrops runs those itself, per kill):
        /// 30% the drop_hp/drop_mp potion (when set), else equipment from the drop_* levels (99 = none) with the
        /// original quality spread (refined ~10%, unique ~4.5%, elite ~3.4%, super ~2.8%, else normal).
        /// Fixes of GenerateItem: thread-safe random (BaseFunc.RandGet re-seeds a shared rand by time), the
        /// 2-hander branch tested nRand instead of nRate, 20-40 gave nothing. Returns 0 when the id is not in
        /// itemtype or above the max level of its slot. Never throws for a valid family.
        /// </summary>
        public uint GenerateFileItem(out byte quality, out Database.ItemType.DBItem dbItem)
        {
            quality = 0;
            dbItem = null;
            var rnd = Program.GetRandom;
            MonsterFamily f = Family;
            if (f == null) return 0;

            if (rnd.Next(0, 100) < 30)
            {
                uint potion = rnd.Next(0, 2) == 0 ? f.DropHPItem : f.DropMPItem;
                if (potion == 0) potion = f.DropHPItem != 0 ? f.DropHPItem : f.DropMPItem;
                if (potion != 0 && Pool.ItemsBase.TryGetValue(potion, out dbItem) && dbItem != null && Database.ItemType.ItemPosition(potion) == 0)
                    return potion;
                dbItem = null;
            }

            int q = rnd.Next(0, 1000);
            quality = (byte)(q < 100 ? 6 : q < 145 ? 7 : q < 179 ? 8 : q < 207 ? 9 : 3);

            uint sort, lev;
            int r = rnd.Next(0, 1200);
            if (r < 20) { sort = 160; lev = f.DropBoots; }
            else if (r < 50) { sort = NecklaceType[rnd.Next(0, NecklaceType.Length)]; lev = f.DropNecklace; }
            else if (r < 100) { sort = RingType[rnd.Next(0, RingType.Length)]; lev = f.DropRing; }
            else if (r < 400) { sort = ArmetType[rnd.Next(0, ArmetType.Length)]; lev = f.DropArmet; }
            else if (r < 700) { sort = ArmorType[rnd.Next(0, ArmorType.Length)]; lev = f.DropArmor; }
            else
            {
                int w = rnd.Next(0, 100);
                if (w < 20) { sort = 421; lev = f.DropWeapon; }
                else if (w < 80) { sort = OneHanderType[rnd.Next(0, OneHanderType.Length)]; lev = f.DropWeapon; }
                else { sort = TwoHanderType[rnd.Next(0, TwoHanderType.Length)]; lev = (sort == 900) ? f.DropShield : f.DropWeapon; }
            }
            if (lev == 99 || lev > 23) return 0; // 99 = this slot never drops
            int a = rnd.Next(0, 100);
            if (a < 50)
            {
                int span = (int)(lev / 2 + lev / 3);
                lev = span > 0 ? (uint)rnd.Next(0, span) : 0;
                if (lev > 1) lev--;
            }
            else if (a > 80)
            {
                bool smallRange = (sort >= 110 && sort <= 114) || (sort >= 130 && sort <= 134) || (sort >= 900 && sort <= 999);
                lev = Math.Min(lev + 1, smallRange ? 9u : 23u);
            }
            uint id = sort * 1000 + lev * 10 + quality;
            if (!Pool.ItemsBase.TryGetValue(id, out dbItem) || dbItem == null) { dbItem = null; return 0; }
            ushort position = Database.ItemType.ItemPosition(id);
            byte maxLevel = Database.ItemType.ItemMaxLevel((Role.Flags.ConquerItem)position);
            if (dbItem.Level > maxLevel) { dbItem = null; return 0; }
            return id;
        }

        public uint GenerateItemCPID(uint map, out Database.ItemType.DBItem DbItem, out uint Value)
        {

            if (SmallCPPack)
            {
                if (Pool.ItemsBase.TryGetValue(720661, out DbItem))
                {
                    Value = 25;
                    return 720661;
                }
            }
            if (NormalCPPack)
            {
                if (Pool.ItemsBase.TryGetValue(720662, out DbItem))
                {
                    Value = 50;
                    return 720662;
                }
            }
            if (SweetyCPPack)
            {
                if (Pool.ItemsBase.TryGetValue(720663, out DbItem))
                {
                    Value = 200;
                    return 720663;
                }
            }
            Value = 0;
            DbItem = null;
            return 0;
        }
        public byte GeneratePurity()
        {
            if (PlusOne)
                return 1;
            if (PlusTwo)
                return 2;
            return 0;
        }
        public byte GenerateBless()
        {
            if (Pool.GetRandom.Next(0, 1000) < 250) // 25%
            {
                int selector = Pool.GetRandom.Next(0, 100);
                if (selector < 1)
                    return 5;
                else if (selector < 6)
                    return 3;
            }
            return 0;
        }
        public byte GenerateSocketCount(uint ItemID)
        {
            if (ItemID >= 410000 && ItemID <= 601999)
            {
                int nRate = Pool.GetRandom.Next(0, 1000) % 100;
                if (nRate < 5) // 5%
                    return 2;
                else if (nRate < 20) // 15%
                    return 1;
            }
            return 0;
        }
        private byte GenerateQuality()
        {
            if (Refined)
                return 6;
            else if (Unique)
                return 7;
            else if (Elite)
                return 8;
            else if (Super)
                return 9;
            return 3;
        }
        public uint GenerateGold(out uint ItemID, bool normal = false, bool twin = false)
        {
            uint amount = (uint)Pool.GetRandom.Next(200, 1000);
            ItemID = Database.ItemType.MoneyItemID((uint)amount);
            return amount;
        }
        private uint AlterItemLevel(uint dwItemLev, uint dwItemSort)
        {
            int nRand = BaseFunc.RandGet(100, true);
            if (nRand < 50)
            {
                uint dwLev = dwItemLev;
                dwItemLev = (uint)(BaseFunc.RandGet((int)(dwLev / 2 + dwLev / 3), false));
                if (dwItemLev > 1)
                    dwItemLev--;
            }
            else if (nRand > 80)
            {
                if ((dwItemSort >= 110 && dwItemSort <= 114) ||
                    (dwItemSort >= 130 && dwItemSort <= 134) ||
                    (dwItemSort >= 900 && dwItemSort <= 999))
                {
                    dwItemLev = Math.Min(dwItemLev + 1, 9);
                }
                else
                {
                    dwItemLev = Math.Min(dwItemLev + 1, 23);
                }
            }
            return dwItemLev;
        }

        
    }
}