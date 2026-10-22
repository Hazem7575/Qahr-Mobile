using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Qahr.Mobile.Role.Instance;
namespace Qahr.Mobile.Database
{
    public class RefinaryBoxes : Dictionary<uint, RefinaryBoxes.Boxe>
    {
        public class Boxe
        {
            public UInt32 Identifier, Position;
            public Boolean Untradable;
            public Rifinery.RefineryType Type;
        }

        public RefinaryBoxes()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("databaserefineryboxes.txt"))
            {
                if (reader.Reader())
                {
                    for (int x = 0; x < reader.Count; x++)
                    {
                        DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("0,0"), ',');
                        Boxe box = new Boxe();
                        box.Identifier = line.Read((uint)0);
                        box.Position = line.Read((uint)0);
                        box.Type = (Rifinery.RefineryType)line.Read((byte)0);
                        box.Untradable = line.Read((byte)0) == 1;

                        Add(box.Identifier, box);
                    }
                }

            }

        }

        public uint GainRefineryItem(uint ID)
        {
            Boxe RefineryB = null;
            if (TryGetValue(ID, out RefineryB))
            {
                List<Rifinery.Item> Possible = new List<Rifinery.Item>();
                foreach (Rifinery.Item RefineryI in Pool.RifineryItems.Values)
                {
                    if (RefineryI.Type == RefineryB.Type)
                    {
                        if (RefineryI.ForItemPosition == RefineryB.Position)
                        {
                            if (RefineryB.Identifier >= 3004197 && RefineryB.Identifier <= 3004226
                                || RefineryB.Identifier >= 3004266 && RefineryB.Identifier <= 3004280)
                            {
                                if (RefineryI.Level > 3)
                                    Possible.Add(RefineryI);
                            }
                            else
                                if (RefineryI.Level < 6)
                                    Possible.Add(RefineryI);
                        }
                    }
                }
                if (Possible.Count > 0)
                {
                    Random Rand = new Random();
                    Int32 x = Rand.Next(1, Possible.Count);
                    Rifinery.Item Refinery = Possible[x];

                    if (Refinery != null)
                    {
                        return Refinery.ItemID;
                    }
                }
            }
            return 0;

        }
    }
    public class Rifinery : Dictionary<uint, Rifinery.Item>
    {
        public enum RefineryType
        {
            None = 0,
            MDefence = 1,
            CriticalStrike = 2,
            SkillCriticalStrike = 3,
            Immunity = 4,
            Break = 5,
            Counteraction = 6,
            Detoxication = 7,
            Block = 8,
            Penetration = 9,
            Intensification = 10,

            FinalMDamage = 11,
            FinalMAttack = 12
        }
        public Rifinery()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "Rifinery.txt");
            foreach (string aline in baseText)
            {
                string[] line = aline.Split(' ');
                Item ite = new Item();
                ite.ItemID = uint.Parse(line[0]);
                ite.Level = CalculateLevel(line[1]);
                ite.Procent = uint.Parse(line[2]);
                ite.Type = (RefineryType)byte.Parse(line[3]);


                ite.Type2 = (RefineryType)byte.Parse(line[4]);
                ite.Procent2 = uint.Parse(line[5]);

                string UseItemName = line[53];
                if (ite.ItemID >= 3006165 && ite.ItemID <= 3006170)
                {
                    UseItemName = UseItemName.Replace("(", "");
                    UseItemName = UseItemName.Split(')')[0];
                }
                else if (ite.ItemID >= 3004136)
                {
                    UseItemName = UseItemName.Replace("[", "");
                    UseItemName = UseItemName.Split(']')[0];
                }

                ite.Name = UseItemName;
                ite.ForItemPosition = ForItemPosition(UseItemName);

                if (!ContainsKey(ite.ItemID))
                    Add(ite.ItemID, ite);

                if (!ItemType.Refinary.ContainsKey(ite.Level))
                    ItemType.Refinary.Add(ite.Level, new Dictionary<uint, Item>());
                if (!ItemType.Refinary[ite.Level].ContainsKey(ite.ItemID))
                    ItemType.Refinary[ite.Level].Add(ite.ItemID, ite);

            }

            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }

        public uint ForItemPosition(string name)
        {
            uint pos = 0;
            if (name == "Bow" || name == "2-Handed" || name == "1-Handed" || name == "Backsword" || name == "2-handed" || name == "1-handed")
                pos = (ushort)Role.Flags.ConquerItem.RightWeapon;
            if (name == "Shield" || name == "Hossu")
                pos = (ushort)Role.Flags.ConquerItem.LeftWeapon;
            if (name == "Ring" || name == "Bracelet")
                pos = (ushort)Role.Flags.ConquerItem.Ring;
            if (name == "Armor")
                pos = (ushort)Role.Flags.ConquerItem.Armor;
            if (name == "Boots")
                pos = (ushort)Role.Flags.ConquerItem.Boots;
            if (name == "Headgear")
                pos = (ushort)Role.Flags.ConquerItem.Head;
            if (name == "Necklace" || name == "Bag")
                pos = (ushort)Role.Flags.ConquerItem.Necklace;
            return pos;
        }

        public static uint CalculateLevel(string name)
        {
            byte level = 0;
            if (name.Contains("Normal")) level = 1;
            if (name.Contains("Refined")) level = 2;
            if (name.Contains("Unique")) level = 3;
            if (name.Contains("Elite")) level = 4;
            if (name.Contains("Super")) level = 5;
            if (name.Contains("Sacred")) level = 6;
            return level;
        }
        public class Item
        {
            public string Name = "";
            public uint ItemID = 0;
            public uint Level = 0;
            public uint ForItemPosition = 0;
            public uint Procent = 0;
            public uint Procent2 = 0;
            public RefineryType Type = 0;
            public RefineryType Type2 = 0;

        }
    }
    public class ItemType : Dictionary<uint, ItemType.DBItem>
    {
        public static List<uint> Relic = new List<uint> { 4100001, 4100002, 4100003, 4100004, 4100005 };
        public static List<uint> EonspiritItem = new List<uint>();

        public static Dictionary<ushort, Dictionary<uint, ItemType.DBItem>> PurificationItems = new Dictionary<ushort, Dictionary<uint, DBItem>>();
        public static Dictionary<uint, ItemType.DBItem> Accessorys = new Dictionary<uint, DBItem>();
        public static List<uint> TopAccesorys = new List<uint>()
        {
            360071 ,
360072  ,
360073 ,
360074  ,
360150  ,
360151  ,
380028 ,
200501 ,
360102 ,
360085  ,
360084  ,
360091  ,
360092 ,
350081 ,
350082 ,
360152 ,
360153  ,
360154 ,
360101  ,
360144 ,
350054 ,
360149  ,
360150  ,
360159 ,
360186  ,
380046  ,
360176 ,
360185 ,

350103  ,
350104  ,
350105 ,
360201 ,
360202 ,
360203
        };

        public static Dictionary<uint, Dictionary<uint, Rifinery.Item>> Refinary = new Dictionary<uint, Dictionary<uint, Rifinery.Item>>();
        public static Dictionary<uint, ItemType.DBItem> Garments = new Dictionary<uint, ItemType.DBItem>();
        public static Dictionary<uint, ItemType.DBItem> SteedMounts = new Dictionary<uint, ItemType.DBItem>();
        public static List<uint> unabletradeitem = new List<uint>()
        {
            750000,
            722700,
            3306885,//reliccrystal
            3307142,//relicspirit
            3600025,
            3200000,
            729549,
            3008994,
            3200005,
            3600029,
            3007108,
            3007109,
            3007110,
            3200004,
            3600031,
            3600027,
            3600024


        };
        public const uint
            MaxPlus = 12,
            MaxEnchant = 255,
            MaxDurability = 7099,
            ExpBall = 723700,
            MeteorTearPacket = 723711,
            MeteorTear = 1088002,
            OneStonePacket = 723712,
            ArenaExperience = 723912,
            OneStone = 730001,
            FiveStone = 730005,
            DBScroll = 720028,
            SuperDBScroll = 3302770,
            SuperMeteorScroll = 3302769,
            DragonBall = 1088000,
            MeteorScroll = 720027,
            Meteor = 1088001,
            MoonBox = 721080,
            ExperiencePotion = 723017,
            NinjaAmulet = 723583,
            PowerExpBall = 722057,
            DoubleExp = 723017,
            SkillTeamPKPack = 720981,
            ToughDrill = 1200005,
            StarDrill = 1200006,
            SuperToroiseGem = 700073,
            ExpBall2 = 723834;


        public static bool IsMoneyBag(uint ID)
        {
            return ID >= 723713 && ID <= 723723 || ID == 3005945 || ID == 3008452;
        }

        public static bool CheckAddGemFan(Role.Flags.Gem gem)
        {
            return gem == Role.Flags.Gem.NormalThunderGem || gem == Role.Flags.Gem.RefinedThunderGem || gem == Role.Flags.Gem.SuperThunderGem;
        }
        public static bool CheckAddGemTower(Role.Flags.Gem gem)
        {
            return gem == Role.Flags.Gem.NormalGloryGem || gem == Role.Flags.Gem.RefinedGloryGem || gem == Role.Flags.Gem.SuperGloryGem;
        }
        public static bool CheckAddGemWing(Role.Flags.Gem gem, byte slot)
        {
            if (slot == 1)
                return CheckAddGemFan(gem);
            else if (slot == 2)
                return CheckAddGemTower(gem);
            return false;
        }

        public static uint GetItemPoints(DBItem _dbitem, Game.MsgServer.MsgGameItem item)
        {

            uint Points = (uint)(item.GetPerfectionPosition == 9 ? 0 : 50);
            if (_dbitem.Level <= 120)
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 30);
            else if (_dbitem.Level > 120 && _dbitem.Level <= 130)
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 50);
            else
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 60);

            if (Database.ItemType.ItemPosition(item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.Garment && Database.ItemType.ItemPosition(item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.SteedMount
                && Database.ItemType.ItemPosition(item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                && Database.ItemType.ItemPosition(item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                && Database.ItemType.ItemPosition(item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.Relics)
            {
                switch (_dbitem.ID % 10)
                {
                    case 6: Points += 50; break;
                    case 7: Points += 150; break;
                    case 8: Points += 250; break;
                    case 9: Points += 450; break;

                }
            }
            else Points = 0;
            switch (item.Bless)
            {
                case 1: Points += 100; break;
                case 3: Points += 300; break;
                case 5: Points += 500; break;
                case 7: Points += 700; break;
            }
            if (item.SocketOne != Role.Flags.Gem.NoSocket)
            {
                Points += 1000;
                switch ((ushort)item.SocketOne % 10)
                {
                    case 1: Points += 200; break;
                    case 2: Points += 500; break;
                    case 3: Points += 800; break;
                }
            }
            if (item.SocketTwo != Role.Flags.Gem.NoSocket)
            {
                Points += 2500;
                switch ((ushort)item.SocketTwo % 10)
                {
                    case 1: Points += 200; break;
                    case 2: Points += 500; break;
                    case 3: Points += 800; break;
                }
            }
            switch (item.Plus)
            {
                case 1: Points += 200; break;
                case 2: Points += 600; break;
                case 3: Points += 1200; break;
                case 4: Points += 1800; break;
                case 5: Points += 2600; break;
                case 6: Points += 3500; break;
                case 7: Points += 4800; break;
                case 8: Points += 5800; break;
                case 9: Points += 6800; break;
                case 10: Points += 7800; break;
                case 11: Points += 8800; break;
                case 12: Points += 10000; break;

            }
            switch (item.Purification.PurificationLevel)
            {
                case 7: Points += 2000; break;
                case 6: Points += 1600; break;
                case 5: Points += 1200; break;
                case 4:
                    {
                        if (ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Ring)
                            Points += 2000;
                        else
                            Points += 800;
                        break;
                    }
                case 3:
                    {
                        Points += 500; break;

                    }
            }
            switch (item.Refinary.EffectLevel)
            {

                case 6: Points += 2000; break;
                case 5: Points += 1600; break;
                case 4: Points += 1200; break;
                case 3: Points += 800; break;
                case 2: Points += 200; break;
                case 1: Points += 100; break;
            }
            if (item.GetPerfectionPosition != 9)//steed
                Points += (uint)(item.Enchant * 2);
            if (item.PerfectionLevel >= 1) Points += 180;
            if (item.PerfectionLevel >= 2) Points += 180;
            if (item.PerfectionLevel >= 3) Points += 180;
            if (item.PerfectionLevel >= 4) Points += 180;
            if (item.PerfectionLevel >= 5) Points += 180;
            if (item.PerfectionLevel >= 6) Points += 180;
            if (item.PerfectionLevel >= 7) Points += 180;
            if (item.PerfectionLevel >= 8) Points += 180;
            if (item.PerfectionLevel >= 9) Points += 180;
            if (item.PerfectionLevel >= 10) Points += 2380;
            if (item.PerfectionLevel >= 11) Points += 400;
            if (item.PerfectionLevel >= 12) Points += 400;
            if (item.PerfectionLevel >= 13) Points += 400;
            if (item.PerfectionLevel >= 14) Points += 400;
            if (item.PerfectionLevel >= 15) Points += 400;
            if (item.PerfectionLevel >= 16) Points += 400;
            if (item.PerfectionLevel >= 17) Points += 400;
            if (item.PerfectionLevel >= 18) Points += 400;
            if (item.PerfectionLevel >= 19) Points += 5150;
            if (item.PerfectionLevel >= 20) Points += 650;
            if (item.PerfectionLevel >= 21) Points += 650;
            if (item.PerfectionLevel >= 22) Points += 650;
            if (item.PerfectionLevel >= 23) Points += 650;
            if (item.PerfectionLevel >= 24) Points += 650;
            if (item.PerfectionLevel >= 25) Points += 650;
            if (item.PerfectionLevel >= 26) Points += 650;
            if (item.PerfectionLevel >= 27) Points += 650;
            if (item.PerfectionLevel >= 28) Points += 100;
            if (item.PerfectionLevel >= 29) Points += 100;
            if (item.PerfectionLevel >= 30) Points += 100;
            if (item.PerfectionLevel >= 31) Points += 100;
            if (item.PerfectionLevel >= 32) Points += 100;
            if (item.PerfectionLevel >= 33) Points += 100;
            if (item.PerfectionLevel >= 34) Points += 100;
            if (item.PerfectionLevel >= 35) Points += 100;
            if (item.PerfectionLevel >= 36) Points += 100;
            if (item.PerfectionLevel >= 37) Points += 100;
            if (item.PerfectionLevel >= 38) Points += 100;
            if (item.PerfectionLevel >= 39) Points += 100;
            if (item.PerfectionLevel >= 40) Points += 100;
            if (item.PerfectionLevel >= 41) Points += 100;
            if (item.PerfectionLevel >= 42) Points += 100;
            if (item.PerfectionLevel >= 43) Points += 100;
            if (item.PerfectionLevel >= 44) Points += 100;
            if (item.PerfectionLevel >= 45) Points += 100;
            if (item.PerfectionLevel >= 46) Points += 100;
            if (item.PerfectionLevel >= 47) Points += 100;
            if (item.PerfectionLevel >= 48) Points += 100;
            if (item.PerfectionLevel >= 49) Points += 100;
            if (item.PerfectionLevel >= 50) Points += 100;
            if (item.PerfectionLevel >= 51) Points += 100;
            if (item.PerfectionLevel >= 52) Points += 100;
            if (item.PerfectionLevel >= 53) Points += 100;
            if (item.PerfectionLevel >= 54) Points += 100;
            if (ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Relics)
            {
                foreach (var attr in item.RelicAttributes.Where(i => i > 0))
                {
                    switch (attr.Type)
                    {
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike: Points += (uint)(5 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike: Points += (uint)(5 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity: Points += (uint)(5 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough: Points += (uint)(5 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction: Points += (uint)(5 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd: Points += (uint)(2 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack: Points += (uint)(4 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack: Points += (uint)(2 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense: Points += (uint)(6 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease: Points += (uint)(2 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease: Points += (uint)(2 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease: Points += (uint)(2 * attr.Value); break;
                        case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease: Points += (uint)(2 * attr.Value); break;
                    }
                }
                for (int x = 0; x < item.RelicAttributes.Length; x++)
                {
                    List<uint> AttrTypes = new List<uint>();
                    int Count = item.RelicAttributes.Count(p => p.Type == item.RelicAttributes[x].Type);
                    var xuanbaoadditionattr = Database.RuneTable.xuanbaoadditionattr.FirstOrDefault(i => i.Type == (byte)item.RelicAttributes[x].Type && i.Num == Count);
                    if (xuanbaoadditionattr != null && !AttrTypes.Contains(xuanbaoadditionattr.Type))
                    {
                        Points += (ushort)(xuanbaoadditionattr.Score);
                        AttrTypes.Add(xuanbaoadditionattr.Type);
                    }
                }
            }
            if (item.AnimaItemID > 0)
                Points += (item.AnimaItemID % 100) * 500;
            return Points;
        }
        public static uint GetGemID(Role.Flags.Gem Gem)
        {
            return (uint)(700000 + (byte)Gem);
        }

        public static uint[] TalismanExtra = new uint[13] { 0, 6, 30, 70, 240, 740, 2240, 6670, 20000, 60000, 62000, 67000, 73000 };

        public static ushort PurifyStabilizationPoints(byte plevel)
        {
            return purifyStabilizationPoints[Math.Min(plevel - 1, (byte)5)];
        }

        static ushort[] purifyStabilizationPoints = new ushort[6] { 10, 30, 60, 100, 150, 200 };

        public static ushort RefineryStabilizationPoints(byte elevel)
        {
            return refineryStabilizationPoints[Math.Min(elevel - 1, (byte)4)];
        }
        static ushort[] refineryStabilizationPoints = new ushort[5] { 10, 30, 70, 150, 270 };

        public string GetItemName(uint ID)
        {
            DBItem item;
            if (Pool.ItemsBase.TryGetValue(ID, out item))
            {
                return item.Name;
            }
            return "";
        }

        public static uint ComposePlusPoints(byte plus)
        {
            return ComposePoints[Math.Min(plus, (byte)12)];
        }
        public static uint StonePlusPoints(byte plus)
        {
            return StonePoints[Math.Min((int)plus, 8)];
        }

        public static bool IsWarriorEpicWeapons(uint ID)
        {
            return ID / 1000 == 624;
        }
        static ushort[] StonePoints = new ushort[9] { 1, 10, 40, 120, 360, 1080, 3240, 9720, 29160 };
        static ushort[] ComposePoints = new ushort[13] { 20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700, 5500, 9000, 0 };

        public static Role.Flags.SoulTyp GetSoulPosition(uint ID)
        {
            if (ID >= 820001 && ID <= 820076)
                return Role.Flags.SoulTyp.Headgear;
            if (ID >= 821002 && ID <= 821034)
                return Role.Flags.SoulTyp.Necklace;
            if (ID >= 824002 && ID <= 824020)
                return Role.Flags.SoulTyp.Boots;
            if (ID >= 823000 && ID <= 823062 || ID == 823098)
                return Role.Flags.SoulTyp.Ring;
            if (ID >= 800000 && ID <= 800142 || ID >= 800701 && ID <= 800917 || ID >= 801000 && ID <= 801104 || ID >= 801200 && ID <= 801308 || ID >= 827000 && ID <= 827013 || ID >= 800621 && ID <= 800633
                || ID >= 801512 && ID <= 801513)
                return Role.Flags.SoulTyp.OneHandWeapon;
            if (ID >= 800200 && ID <= 800618 || ID == 801103)
                return Role.Flags.SoulTyp.TwoHandWeapon;
            if (ID >= 822001 && ID <= 822072)
                return Role.Flags.SoulTyp.Armor;

            return Role.Flags.SoulTyp.None;
        }
        public static bool CompareSoul(uint ITEMID, uint SoulID)
        {
            Role.Flags.SoulTyp soul = GetSoulPosition(SoulID);
            var positionit = GetItemSoulTYPE(ITEMID);
            if (positionit == soul)
                return true;
            return false;
        }
        public static Role.Flags.SoulTyp GetItemSoulTYPE(UInt32 itemid)
        {
            UInt32 iType = itemid / 1000;

            if (iType >= 111 && iType <= 118 || iType == 123 || iType >= 141 && iType <= 148 || itemid >= 170000 && itemid <= 170309 || itemid >= 171000 && itemid <= 171309)
                return Role.Flags.SoulTyp.Headgear;

            else if (iType >= 120 && iType <= 121)
                return Role.Flags.SoulTyp.Necklace;

            else if (iType >= 130 && iType <= 139 || itemid >= 101000 && itemid <= 102309 || itemid >= 103000 && itemid <= 103309)
                return Role.Flags.SoulTyp.Armor;

            else if (iType >= 150 && iType <= 152)
                return Role.Flags.SoulTyp.Ring;

            else if (iType == 160)
                return Role.Flags.SoulTyp.Boots;

            else if (IsTwoHand(itemid) || itemid >= 421003 && itemid <= 421439)
                return Role.Flags.SoulTyp.TwoHandWeapon;

            else if ((iType >= 410 && iType <= 490) || (iType >= 500 && iType <= 580) || (iType >= 601 && iType <= 613) || iType == 616 || iType == 614 || iType == 617 || iType == 622
            || iType == 624 || iType == 619 || iType == 606 || IsWindWalkerWeapon(itemid) || IsPirateEpicWeapon(itemid) || (iType >= 680 && iType <= 681))
                return Role.Flags.SoulTyp.OneHandWeapon;


            else
                return Role.Flags.SoulTyp.TwoHandWeapon;
        }
        public static Game.MsgServer.MsgChiInfo.ChiAttribute RelicAttribute(uint ID)
        {
        again:
            Game.MsgServer.MsgChiInfo.ChiAttribute attribute = Game.MsgServer.MsgChiInfo.ChiAttribute.None;
            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ID).ToArray();
            attribute = Attrs[Pool.GetRandom.Next(0, Attrs.Count())].Attribute;
            switch (ID)
            {
                case 4100001://Featured (M-Attack)
                    {

                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                            goto again;
                        break;
                    }
                case 4100002://Featured (Max-HP)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                            goto again;
                        break;
                    }
                case 4100003://Featured (M-Strike)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                            goto again;
                        break;
                    }
                case 4100004://Featured (P-Attack)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike)
                            goto again;
                        break;
                    }
                case 4100005://Featured (P-Strike)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike)
                            goto again;
                        break;
                    }
            }
            return attribute;
        }
        public static uint MoneyItemID(uint value)
        {
            if (value < 100)
                return 1090000;
            else if (value < 399)
                return 1090010;
            else if (value < 5099)
                return 1090020;
            else if (value < 8099)
                return 1091020;
            else if (value < 12099)
                return 1091020;
            else
                return 1091020;
        }


        public void Loading()
        {
            Dictionary<uint, ITPlus[]> itemsplus = new Dictionary<uint, ITPlus[]>();

            string[] baseplusText = File.ReadAllLines(Program.ServerConfig.DbLocation + "ItemAdd.ini");
            foreach (string line in baseplusText)
            {
                string _item_ = line.Trim();
                ITPlus pls = new ITPlus();
                pls.Parse(_item_);
                if (itemsplus.ContainsKey(pls.ID))
                {
                    itemsplus[pls.ID][pls.Plus] = pls;
                }
                else
                {
                    ITPlus[] a_pls = new ITPlus[13];
                    a_pls[pls.Plus] = pls;
                    itemsplus.Add(pls.ID, a_pls);
                }
            }
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "itemtype.txt");
            foreach (string line in baseText)
            {
                string _item_ = line.Trim();
                if (_item_.Length > 11)
                {
                    if (_item_.IndexOf("//", 0, 2) != 0)
                    {
                        DBItem item = new DBItem();
                        item.Parse(line);
                        // if (item.ID == PowerExpBall)
                        //     continue;
                        if (itemsplus.ContainsKey(GetBaseID(item.ID)) || itemsplus.ContainsKey(GetBaseID(item.ID) + 10) || itemsplus.ContainsKey(GetBaseID(item.ID) + 20))
                        {
                            item.AllowUpgradePlus = true;
                            if (!itemsplus.TryGetValue(GetBaseID(item.ID), out item.Plus))
                            {
                                if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 10, out item.Plus))
                                {
                                    if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 20, out item.Plus))
                                    {
                                        int pos = ItemPosition(item.ID);
                                        if (pos < 6)
                                            Console.WriteLine("eroror item " + item.ID + " " + item.Name);
                                    }
                                }
                            }
                        }
                        if (!ContainsKey(item.ID))
                            Add(item.ID, item);
                    }
                }
            }
            baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "ItemtypeSub.txt");
            foreach (string line in baseText)
            {
                string _item_ = line.Trim();
                if (_item_.Length > 11)
                {
                    if (_item_.IndexOf("//", 0, 2) != 0)
                    {
                        DBItem item = new DBItem();
                        item.Parse(line);
                        // if (item.ID == PowerExpBall)
                        //     continue;
                        if (itemsplus.ContainsKey(GetBaseID(item.ID)) || itemsplus.ContainsKey(GetBaseID(item.ID) + 10) || itemsplus.ContainsKey(GetBaseID(item.ID) + 20))
                        {
                            item.AllowUpgradePlus = true;
                            if (!itemsplus.TryGetValue(GetBaseID(item.ID), out item.Plus))
                            {
                                if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 10, out item.Plus))
                                {
                                    if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 20, out item.Plus))
                                    {
                                        int pos = ItemPosition(item.ID);
                                        if (pos < 6)
                                            Console.WriteLine("eroror item " + item.ID + " " + item.Name);
                                    }
                                }
                            }
                        }
                        if (!ContainsKey(item.ID))
                            Add(item.ID, item);
                    }
                }
            }
            itemsplus = null;
            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
        public uint DowngradeItem(uint ID)
        {
            try
            {
                ushort Tryng = 0;
                ushort firstposition = ItemPosition(ID);
                uint rebornid = ID;
                while (true)
                {
                    if (Tryng > 1000)
                        break;
                    Tryng++;
                    //shield !
                    if (ID >= 900000 && ID <= 900309)
                    {
                        if (this[rebornid].Level <= 40)
                            break;
                    }
                    if (this[rebornid].Level <= ItemMinLevel((Role.Flags.ConquerItem)ItemPosition(ID)))
                    {
                        break;
                    }
                    if (this.ContainsKey(rebornid - 10))
                    {
                        rebornid -= 10;
                    }
                    else if (this.ContainsKey(rebornid - 20))
                    {
                        rebornid -= 20;
                    }
                    else if (this.ContainsKey(rebornid - 30))
                    {
                        rebornid -= 30;
                    }
                    else if (this.ContainsKey(rebornid - 40))
                    {
                        rebornid -= 40;
                    }
                }
                if (firstposition == ItemPosition(rebornid) && this.ContainsKey(rebornid))
                    return rebornid;
                else
                    return ID;

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); return ID; }
        }
        public uint UpdateItemLevelLast(uint id, out bool succesed)
        {
            ushort firstposition = ItemPosition(id);
            uint nextId = 0;
            if (this[id].Level < ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id)))
            {
                var Item = this.Values.Where(p => p.ID / 1000 == id / 1000 && p.Level == ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id)) && p.ID % 10 == id % 10).FirstOrDefault();
                if (Item != null)
                    nextId = Item.ID;
            }
            if (firstposition == ItemPosition(nextId) && this.ContainsKey(nextId))
            {
                succesed = true;
                return nextId;
            }
            else
            {
                succesed = false;
                return id;
            }
        }
        public uint UpdateItem(uint id, out bool succesed)
        {
            ushort firstposition = ItemPosition(id);
            uint nextId = 0;
            if (this[id].Level < ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id)))
            {
                if (this.ContainsKey(id + 10))
                {
                    nextId = id + 10;
                }
                else if (this.ContainsKey(id + 20))
                {
                    nextId = id + 20;
                }
                else if (this.ContainsKey(id + 30))
                {
                    nextId = id + 30;
                }
                else if (this.ContainsKey(id + 40))
                {
                    nextId = id + 40;
                }
            }
            if (firstposition == ItemPosition(nextId) && this.ContainsKey(nextId))
            {
                succesed = true;
                return nextId;
            }
            else
            {
                succesed = false;
                return id;
            }
        }
        public uint UpdateItem(uint id, Client.GameClient client)
        {
            uint nextId = id;
            if ((this[id].Level < ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id))) && this[id].Level < client.Player.Level)
            {
                if (this.ContainsKey(id + 10))
                {
                    nextId = id + 10;
                }
            }
            return nextId;
        }
        public static byte ItemMinLevel(Role.Flags.ConquerItem postion)
        {
            switch (postion)
            {
                case 0: return 0;
                case Role.Flags.ConquerItem.Head: return 15;
                case Role.Flags.ConquerItem.Necklace: return 7;
                case Role.Flags.ConquerItem.Armor: return 15;
                case Role.Flags.ConquerItem.LeftWeapon: return 15;
                case Role.Flags.ConquerItem.RightWeapon: return 15;
                case Role.Flags.ConquerItem.Boots: return 10;
                case Role.Flags.ConquerItem.Ring: return 10;
                case Role.Flags.ConquerItem.Tower: return 0;
                case Role.Flags.ConquerItem.Fan: return 0;
                case Role.Flags.ConquerItem.Steed: return 0;
                case Role.Flags.ConquerItem.Garment: return 0;
                case Role.Flags.ConquerItem.RidingCrop: return 0;
            }
            return 0;
        }
        public static byte GetSashCounts(uint ID)
        {
            if (ID == 1100009) return 12;
            if (ID == 1100006) return 6;
            if (ID == 1100003) return 3;
            return 0;
        }
        public static bool IsSash(uint ID)
        {
            if (ID == 1100009 || ID == 1100006 || ID == 1100003) return true;
            return false;
        }

        public static bool IsPistol(uint ID)
        {
            return ID >= 612000 && ID <= 612439;
        }
        public static bool IsPickAxe(uint ID)
        {
            return ID == 562000;
        }
        public static bool IsRapier(uint ID)
        {
            return ID >= 611000 && ID <= 611439;
        }
        public static bool IsKnife(uint ID)
        {
            return ID >= 613000 && ID <= 613429;
        }
        public static bool IsSolusBlade(uint ID)
        {
            return ID >= 608000 && ID <= 608439;
        }
        public static bool IsClub(uint ID)
        {
            return ID >= 480003 && ID <= 490439;
        }
        public static bool IsBlade(uint ID)
        {
            return ID >= 410003 && ID <= 410439;
        }
        public static bool IsSword(uint ID)
        {
            return ID >= 420003 && ID <= 420439;
        }
        public static bool IsDagger(uint ID)
        {
            return ID >= 490003 && ID <= 490149;
        }
        public static bool IsScepter(uint ID)
        {
            return ID >= 481003 && ID <= 481439;
        }
        public static bool IsShield(uint ID)
        {
            return ID >= 900000 && ID <= 900309;
        }
        public static bool IsTrojanEpicWeapon(uint ID)
        {
            return ID >= 614000 && ID <= 614439;
        }
        public static bool IsNinjaEpicWeapon(uint ID)
        {
            return ID >= 616000 && ID <= 616439;
        }
        public static bool IsPrayedBead(uint ID)
        {
            return ID >= 610000 && ID <= 610439;
        }
        public static bool IsMonkEpicWeapon(uint ID)
        {
            return ID >= 622000 && ID <= 622439;

        }
        public static bool IsDragonWarriorWeapon(uint ID)
        {
            return ((ID >= 617000) && (ID <= 617439));
        }
        public static bool IsRightWeaponThunder(uint ID)
        {
            return ((ID >= 681000) && (ID <= 681439));
        }
        public static bool IsLeftWeaponThunder(uint ID)
        {
            return ((ID >= 680000) && (ID <= 680439));
        }
        public static bool IsPirateEpicWeapon(uint ID)
        {
            return ID / 1000 == 670 || ID / 1000 == 671;
        }
        public static bool IsArcherEpicWeapon(uint ID)
        {
            return ID >= 606000 && ID <= 606429;
        }
        public static bool IsTalisman(uint ID)
        {
            return ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Tower || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Fan
                || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Wing;
        }
        public static bool Animaaaa(UInt32 itemid)
        {
            if (itemid >= 4200001 && itemid <= 4200019)
                return true;
            return false;
        }
        public static bool isRune(UInt32 itemid)
        {
            if (itemid / 10000 >= 401 && itemid / 10000 <= 403 || itemid >= 4070001 && itemid <= 4070009 || itemid >= 4070501 && itemid <= 4070509 || itemid == 4070609 || itemid >= 4038601 && itemid <= 4038609 || itemid >= 4070701 && itemid <= 4070709 || itemid >= 4071001 && itemid <= 4071009 || itemid >= 4071201 && itemid <= 4071209 || itemid >= 4071401 && itemid <= 4071409 || itemid == 4039009 || itemid == 4070309
               || itemid >= 4071501 && itemid <= 4071509 || itemid >= 4072001 && itemid <= 4072009 || itemid == 4072609 || itemid == 4072809 || itemid >= 4071601 && itemid <= 4071609 || itemid >= 4071701 && itemid <= 4071709 || itemid >= 4071801 && itemid <= 4071809 || itemid >= 4071901 && itemid <= 4071909 || itemid >= 4072901 && itemid <= 4072909 || itemid >= 4073001 && itemid <= 4073009 || itemid == 4072109 || itemid == 4072709 || itemid >= 4028701 && itemid <= 4028727 || itemid == 4073309 || itemid == 4073409 || itemid >= 4073101 && itemid <= 4073109)
                return true;
            return false;
        }
        public static bool isRedRune(UInt32 itemid)
        {
            if (itemid / 10000 == 401)
                return true;
            return false;
        }
        public static bool isBlueRune(UInt32 itemid)
        {
            if (itemid / 10000 == 402)
                return true;
            return false;
        }
        public static bool isYellowRune(UInt32 itemid)
        {
            if (itemid / 10000 == 403 || itemid > 4070001 && itemid <= 4070009 || itemid > 4070501 && itemid <= 4070509 || itemid >= 4038601 && itemid <= 4038609 || itemid >= 4070701 && itemid <= 4070709 || itemid >= 4071001 && itemid <= 4071009 || itemid >= 4071201 && itemid <= 4071209 || itemid >= 4071401 && itemid <= 4071409 || itemid == 4039009 || itemid == 4070309
                || itemid >= 4071501 && itemid <= 4071509 || itemid >= 4072001 && itemid <= 4072009 || itemid == 4072609 || itemid >= 4071601 && itemid <= 4071609 || itemid >= 4071701 && itemid <= 4071709 || itemid >= 4071801 && itemid <= 4071809 || itemid >= 4071901 && itemid <= 4071909 || itemid >= 4072901 && itemid <= 4072909 || itemid >= 4073001 && itemid <= 4073009 || itemid == 4072109 || itemid == 4072709 || itemid >= 4028701 && itemid <= 4028727 || itemid == 4073309 || itemid == 4073409 || itemid >= 4073101 && itemid <= 4073109)
                return true;
            return false;
        }
        public static byte MaxRuneLevel(uint id)
        {
            byte Max = (byte)((id / 10000 == 403 || id / 10000 == 407) ? 9 : 27);
            if (id / 10000 == 401 || id / 10000 == 401) Max = 1;
            return Max;
        }

        public static bool IsComprehensionRune(Game.MsgServer.MsgGameItem item)
        {
            return item != null && item.ITEM_ID == 4010001;
        }
        public static uint FlashShieldRunePoints(uint itemID)
        {
            uint addition = 5000;
            switch (itemID % 100)
            {
                case 2: addition = 6000; break;
                case 3: addition = 7000; break;
                case 4: addition = 7500; break;
                case 5: addition = 8000; break;
                case 6: addition = 8500; break;
                case 7: addition = 9000; break;
                case 8: addition = 9500; break;
                case 9: addition = 10000; break;
                case 10: addition = 11000; break;
                case 11: addition = 11500; break;
                case 12: addition = 12000; break;
                case 13: addition = 12500; break;
                case 14: addition = 13000; break;
                case 15: addition = 13500; break;
                case 16: addition = 14000; break;
                case 17: addition = 14500; break;
                case 18: addition = 15000; break;
                case 19: addition = 15500; break;
                case 20: addition = 16000; break;
                case 21: addition = 16500; break;
                case 22: addition = 17000; break;
                case 23: addition = 17500; break;
                case 24: addition = 18000; break;
                case 25: addition = 18500; break;
                case 26: addition = 19000; break;
                case 27: addition = 20000; break;
            }
            return addition;
        }
        public static bool AllowToUpdate(Role.Flags.ConquerItem Position)
        {
            if (Position == Role.Flags.ConquerItem.RidingCrop
                 || Position == Role.Flags.ConquerItem.Fan
                  || Position == Role.Flags.ConquerItem.Tower
                  || Position == Role.Flags.ConquerItem.Wing
                || Position == Role.Flags.ConquerItem.Garment
                || Position == Role.Flags.ConquerItem.AleternanteGarment
                || Position == Role.Flags.ConquerItem.AleternanteBottle
                || Position == Role.Flags.ConquerItem.Bottle
                || Position == Role.Flags.ConquerItem.Relics
                || Position == Role.Flags.ConquerItem.LeftWeaponAccessory
                || Position == Role.Flags.ConquerItem.RightWeaponAccessory
                || Position == Role.Flags.ConquerItem.SteedMount
                || Position == Role.Flags.ConquerItem.Steed)
                return false;
            return true;
        }
        public static bool AllowAnima(Role.Flags.ConquerItem Position)
        {
            if (Position == Role.Flags.ConquerItem.Head
                 || Position == Role.Flags.ConquerItem.LeftWeapon
                  || Position == Role.Flags.ConquerItem.RightWeapon
                      || Position == Role.Flags.ConquerItem.Ring
                      || Position == Role.Flags.ConquerItem.Necklace
                      || Position == Role.Flags.ConquerItem.Boots
                  || Position == Role.Flags.ConquerItem.Armor)
                return true;
            return false;
        }
        public static int TypeofItem(uint ID)
        {
            int ret = -1;
            uint Type1 = ID % 1000000;
            uint Type2 = Type1 / 1000;
            switch (Type2)
            {
                case 123:
                    return 1;
                case 203:
                    return 21;
                case 204:
                    return 22;
                case 300:
                    if (ID % 10 != 0)
                        return 14;
                    break;
                case 601:
                    return 4;
                case 201:
                case 202:
                    return 12;
                case 200:
                    return 20;
                case 350:
                    return 16;
                case 360:
                    return 15;
                case 370:
                    return 17;
                case 380:
                    return 18;
            }
            uint Type3 = ID % 10000000 / 100000;
            uint Type4 = Type1 / 10000;
            switch (Type3)
            {
                case 1:
                    switch (Type4)
                    {
                        case 11:
                        case 14:
                        case 17:
                            return 1;
                        case 18:
                        case 19:
                            return 11;
                        case 12:
                            return 2;
                        case 13:
                            return 3;
                        case 15:
                            return 7;
                        case 16:
                            return 8;
                    }
                    break;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 7:
                    return 9;
                case 9:
                    return 6;
                case 10:
                    return 0;
                default:
                    if (Type3 <= 29 && Type3 > 7)
                        return 9;
                    if (Type3 != 6)
                        return ret;
                    break;
            }
            return ret;
        }
        public static bool IsWindWalkerWeapon(uint ID)
        {
            return ID >= 626003 && ID <= 626439;
        }
        public static bool IsDragonWorroir(uint ID)
        {
            return ID >= 617393 && ID <= 617439;
        }

        public static ushort ItemPosition(uint ID)
        {
            if (ID >= 203003 && ID <= 203009)

                return (ushort)Role.Flags.ConquerItem.RidingCrop;

            if (ID >= 201003 && ID <= 201009)

                return (ushort)Role.Flags.ConquerItem.Fan;

            else if (ID >= 202003 && ID <= 202009)

                return (ushort)Role.Flags.ConquerItem.Tower;

            else if (ID >= 204003 && ID <= 204009)

                return (ushort)Role.Flags.ConquerItem.Wing;

            CoatStorage.StorageItem DB;
            if (CoatStorage.StorageItems.TryGetValue(ID, out DB))
            {
                if (DB.Type == 1)
                    return (ushort)Role.Flags.ConquerItem.Garment;
                else
                    return (ushort)Role.Flags.ConquerItem.SteedMount;
            }

            if (ID == 390001)

                return (ushort)Role.Flags.ConquerItem.RightWeaponAccessory;

            else if (ID >= 350001 && ID <= 380060)

                return (ushort)Role.Flags.ConquerItem.RightWeaponAccessory;
            else if (ID >= 370001 && ID <= 380060)

                return (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory;

            if ((ID >= 111003 && ID <= 118309) || (ID >= 123000 && ID <= 123309) || (ID >= 141003 && ID <= 145309) || (ID >= 146000 && ID <= 148309)
               || ID >= 170000 && ID <= 170309 || ID >= 171000 && ID <= 171309)
                return (ushort)Role.Flags.ConquerItem.Head;

            else if (ID >= 120001 && ID <= 121269)
                return (ushort)Role.Flags.ConquerItem.Necklace;

            else if (ID >= 130003 && ID <= 139309 || ID >= 101000 && ID <= 102309 || ID >= 103000 && ID <= 103309)
                return (ushort)Role.Flags.ConquerItem.Armor;

            else if (ID >= 150000 && ID <= 152279)
                return (ushort)Role.Flags.ConquerItem.Ring;

            else if (ID >= 160013 && ID <= 160249)
                return (ushort)Role.Flags.ConquerItem.Boots;
            else if (ID == 300000)
                return (ushort)Role.Flags.ConquerItem.Steed;

            else if (ID >= 410003 && ID <= 613429 || IsTrojanEpicWeapon(ID) || IsMonkEpicWeapon(ID) || IsNinjaEpicWeapon(ID) || (ID >= 617000 && ID <= 617439) || IsTaoistEpicWeapon(ID)
               || IsWarriorEpicWeapons(ID) || IsWindWalkerWeapon(ID) || IsPirateEpicWeapon(ID) || IsSolusBlade(ID) || IsMonkEpicWeapon(ID) || (ID / 1000 == 681))
                return (ushort)Role.Flags.ConquerItem.RightWeapon;

            else if ((ID >= 900000 && ID <= 900309) || (ID >= 1050000 && ID <= 1051000) || (ID >= 612000 && ID <= 612439)
                || (ID >= 612000 && ID <= 613429) || IsWarriorEpicWeapons(ID) || IsWindWalkerWeapon(ID) || IsSolusBlade(ID)
                || (ID >= 611000 && ID <= 611439) || IsTrojanEpicWeapon(ID) || IsNinjaEpicWeapon(ID) || IsMonkEpicWeapon(ID) || (ID >= 617000 && ID <= 617439) || IsHossu(ID) || (ID / 1000 == 680))
                return (ushort)Role.Flags.ConquerItem.LeftWeapon;

            else if (ID >= 2100025 && ID <= 2100125 || ID == 2169345 || ID == 2169645 || ID == 2100165 || ID == 2100175 ||
              ID == 2169165 || ID == 2169645 || ID == 2100075 || ID == 2100095 || ID == 2100185 || ID == 2100195 || ID == 2100205 || ID == 2100245 || ID == 2100253 || ID == 2168735 || ID == 2168896 || ID == 2168725 || ID == 2168715 || ID == 2168705)
                return (ushort)Role.Flags.ConquerItem.Bottle;

            #region P1 Jade Kylin - P15 Jade Kylin
            else if (ID == 2169805 || ID == 2169815 || ID == 2169825 || ID == 2169835 || ID == 2169845 || ID == 2169855 || ID == 2169865 || ID == 2169875 || ID == 2169885 || ID == 2169895 || ID == 2169905 || ID == 2169915 || ID == 2169925 || ID == 2169935 || ID == 2169945)
                return (ushort)Role.Flags.ConquerItem.Bottle;
            #endregion

            #region New Cups 
            else if (ID == 2102384 || ID == 2170155 || ID == 2169335 || ID == 2169385 || ID == 2169255 || ID == 2169025 || ID == 2169035)
                return (ushort)Role.Flags.ConquerItem.Bottle;
            #endregion


            #region Rune
            else if (ID / 10000 == 401)
                return (ushort)Role.Flags.ConquerItem.RedRune;
            else if (ID / 10000 == 402)
                return (ushort)Role.Flags.ConquerItem.BlueRune;
            else if (ID / 10000 == 403)
                return (ushort)Role.Flags.ConquerItem.YellowRune;
            else if (ID / 10000 == 407)
                return (ushort)Role.Flags.ConquerItem.YellowRune;
            else if (ID / 10000 == 410)
                return (ushort)Role.Flags.ConquerItem.Relics;
            else if (ID / 10000 == 4070)
                return (ushort)Role.Flags.ConquerItem.MythSoul;
            return 0;
            #endregion
        }
        public static bool IsAnima(uint ID)
        {
            return ID >= 4200001 && ID <= 4200019;
        }
        public static bool IsHossu(uint ID)
        {
            return ID >= 619000 && ID <= 619439;
        }

        public static bool IsTaoistEpicWeapon(uint ID)
        {
            return ID >= 620003 && ID <= 620439;
        }
        public static bool IsArrow(uint ID)
        {
            if (ID >= 1050000 && ID <= 1051000)
                return true;
            return false;
        }
        public static bool IsTwoHand(uint ID)
        {
            return ((ID.ToString()[0] == '5' || ID >= 421003 && ID <= 421439) ? true : false);
        }
        public static bool IsBow(uint ID)
        {
            return ID >= 500003 && ID <= 500429;
        }
        public static bool IsAccessory(uint ID)
        {
            return ID >= 350001 && ID <= 380046;
        }
        public static byte ItemMaxLevel(Role.Flags.ConquerItem Position)
        {
            switch (Position)
            {
                case 0: return 0;
                case Role.Flags.ConquerItem.Head: return 140;
                case Role.Flags.ConquerItem.Necklace: return 139;
                case Role.Flags.ConquerItem.Armor: return 140;
                case Role.Flags.ConquerItem.LeftWeapon: return 140;
                case Role.Flags.ConquerItem.RightWeapon: return 140;
                case Role.Flags.ConquerItem.Boots: return 129;
                case Role.Flags.ConquerItem.Ring: return 136;
                case Role.Flags.ConquerItem.Tower: return 100;
                case Role.Flags.ConquerItem.Fan: return 100;
                case Role.Flags.ConquerItem.Steed: return 0;
                case Role.Flags.ConquerItem.SteedMount: return 30;
                case Role.Flags.ConquerItem.Wing: return 100;
            }
            return 0;
        }
        public static bool IsBacksword(uint ID)
        {
            return ID >= 421003 && ID <= 421439;
        }
        public static bool IsKatana(uint ID)
        {
            return ID >= 601000 && ID <= 601439;
        }
        public static bool IsScythe(uint ID)
        {
            if (ID == 511) return true;
            if (ID >= 511000 && ID <= 511439) return true;
            return false;
        }
        public static bool IsMonkWeapon(uint ID)
        {
            return ID >= 610000 && ID <= 610439;
        }

        public static bool EquipPassStatsReq(DBItem baseInformation, Client.GameClient client)
        {
            if (client.Player.Strength >= baseInformation.Strength && client.Player.Agility >= baseInformation.Agility)
                return true;
            else
                return false;
        }
        public static bool EquipPassJobReq(DBItem baseInformation, Client.GameClient client)
        {
            switch (baseInformation.Class)
            {
                #region Trojan
                case 1000: if (client.Player.Class <= 1057 && client.Player.Class >= 1000) return true; break;
                case 1001: if (client.Player.Class <= 1057 && client.Player.Class >= 1001) return true; break;
                case 1002: if (client.Player.Class <= 1057 && client.Player.Class >= 1002) return true; break;
                case 1003: if (client.Player.Class <= 1057 && client.Player.Class >= 1003) return true; break;
                case 1004: if (client.Player.Class <= 1057 && client.Player.Class >= 1004) return true; break;
                case 1005: if (client.Player.Class <= 1057 && client.Player.Class >= 1005) return true; break;
                #endregion
                #region Warrior
                case 2000: if (client.Player.Class <= 2057 && client.Player.Class >= 2000) return true; break;
                case 2001: if (client.Player.Class <= 2057 && client.Player.Class >= 2001) return true; break;
                case 2002: if (client.Player.Class <= 2057 && client.Player.Class >= 2002) return true; break;
                case 2003: if (client.Player.Class <= 2057 && client.Player.Class >= 2003) return true; break;
                case 2004: if (client.Player.Class <= 2057 && client.Player.Class >= 2004) return true; break;
                case 2005: if (client.Player.Class <= 2057 && client.Player.Class >= 2005) return true; break;
                #endregion
                #region DuneWanderer
                case 3000: if (client.Player.Class <= 3057 && client.Player.Class >= 3000) { return true; } break;
                case 3001: if (client.Player.Class <= 3057 && client.Player.Class >= 3001) { return true; } break;
                case 3002: if (client.Player.Class <= 3057 && client.Player.Class >= 3002) { return true; } break;
                case 3003: if (client.Player.Class <= 3057 && client.Player.Class >= 3003) { return true; } break;
                case 3004: if (client.Player.Class <= 3057 && client.Player.Class >= 3004) { return true; } break;
                case 3005: if (client.Player.Class <= 3057 && client.Player.Class >= 3005) { return true; } break;
                #endregion
                #region Archer
                case 4000: if (client.Player.Class <= 4057 && client.Player.Class >= 4000) return true; break;
                case 4001: if (client.Player.Class <= 4057 && client.Player.Class >= 4001) return true; break;
                case 4002: if (client.Player.Class <= 4057 && client.Player.Class >= 4002) return true; break;
                case 4003: if (client.Player.Class <= 4057 && client.Player.Class >= 4003) return true; break;
                case 4004: if (client.Player.Class <= 4057 && client.Player.Class >= 4004) return true; break;
                case 4005: if (client.Player.Class <= 4057 && client.Player.Class >= 4005) return true; break;
                #endregion
                #region Ninja
                case 5000: if (client.Player.Class <= 5057 && client.Player.Class >= 5000) return true; break;
                case 5001: if (client.Player.Class <= 5057 && client.Player.Class >= 5001) return true; break;
                case 5002: if (client.Player.Class <= 5057 && client.Player.Class >= 5002) return true; break;
                case 5003: if (client.Player.Class <= 5057 && client.Player.Class >= 5003) return true; break;
                case 5004: if (client.Player.Class <= 5057 && client.Player.Class >= 5004) return true; break;
                case 5005: if (client.Player.Class <= 5057 && client.Player.Class >= 5005) return true; break;
                #endregion
                #region Monk
                case 6000: if (client.Player.Class <= 6057 && client.Player.Class >= 6000) return true; break;
                case 6001: if (client.Player.Class <= 6057 && client.Player.Class >= 6001) return true; break;
                case 6002: if (client.Player.Class <= 6057 && client.Player.Class >= 6002) return true; break;
                case 6003: if (client.Player.Class <= 6057 && client.Player.Class >= 6003) return true; break;
                case 6004: if (client.Player.Class <= 6057 && client.Player.Class >= 6004) return true; break;
                case 6005: if (client.Player.Class <= 6057 && client.Player.Class >= 6005) return true; break;
                #endregion
                #region Pirate
                case 7000: if (client.Player.Class <= 7057 && client.Player.Class >= 7000) return true; break;
                case 7001: if (client.Player.Class <= 7057 && client.Player.Class >= 7001) return true; break;
                case 7002: if (client.Player.Class <= 7057 && client.Player.Class >= 7002) return true; break;
                case 7003: if (client.Player.Class <= 7057 && client.Player.Class >= 7003) return true; break;
                case 7004: if (client.Player.Class <= 7057 && client.Player.Class >= 7004) return true; break;
                case 7005: if (client.Player.Class <= 7057 && client.Player.Class >= 7005) return true; break;
                #endregion
                #region LongLee
                case 8000: if (client.Player.Class <= 8057 && client.Player.Class >= 8000) return true; break;
                case 8001: if (client.Player.Class <= 8057 && client.Player.Class >= 8001) return true; break;
                case 8002: if (client.Player.Class <= 8057 && client.Player.Class >= 8002) return true; break;
                case 8003: if (client.Player.Class <= 8057 && client.Player.Class >= 8003) return true; break;
                case 8004: if (client.Player.Class <= 8057 && client.Player.Class >= 8004) return true; break;
                case 8005: if (client.Player.Class <= 8057 && client.Player.Class >= 8005) return true; break;
                #endregion
                #region ThunderStriker
                case 9000: if (client.Player.Class <= 9057 && client.Player.Class >= 9000) return true; break;
                case 9001: if (client.Player.Class <= 9057 && client.Player.Class >= 9001) return true; break;
                case 9002: if (client.Player.Class <= 9057 && client.Player.Class >= 9002) return true; break;
                case 9003: if (client.Player.Class <= 9057 && client.Player.Class >= 9003) return true; break;
                case 9004: if (client.Player.Class <= 9057 && client.Player.Class >= 9004) return true; break;
                case 9005: if (client.Player.Class <= 9057 && client.Player.Class >= 9005) return true; break;
                #endregion
                #region Water
                case 13000: if (client.Player.Class <= 13057 && client.Player.Class >= 13000) return true; break;
                case 13001: if (client.Player.Class <= 13057 && client.Player.Class >= 13001) return true; break;
                case 13002: if (client.Player.Class <= 13057 && client.Player.Class >= 13002) return true; break;
                case 13003: if (client.Player.Class <= 13057 && client.Player.Class >= 13003) return true; break;
                case 13004: if (client.Player.Class <= 13057 && client.Player.Class >= 13004) return true; break;
                case 13005: if (client.Player.Class <= 13057 && client.Player.Class >= 13005) return true; break;
                #endregion
                #region Fire
                case 14000: if (client.Player.Class <= 14057 && client.Player.Class >= 14000) return true; break;
                case 14001: if (client.Player.Class <= 14057 && client.Player.Class >= 14001) return true; break;
                case 14002: if (client.Player.Class <= 14057 && client.Player.Class >= 14002) return true; break;
                case 14003: if (client.Player.Class <= 14057 && client.Player.Class >= 14003) return true; break;
                case 14004: if (client.Player.Class <= 14057 && client.Player.Class >= 14004) return true; break;
                case 14005: if (client.Player.Class <= 14057 && client.Player.Class >= 14005) return true; break;
                #endregion
                #region Taoist
                case 10000:
                case 19000: if (client.Player.Class >= 10000) return true; break;
                #endregion
                #region WindWalker
                case 16000: if (client.Player.Class <= 16057 && client.Player.Class >= 16000) return true; break;
                case 16001: if (client.Player.Class <= 16057 && client.Player.Class >= 16001) return true; break;
                case 16002: if (client.Player.Class <= 16057 && client.Player.Class >= 16002) return true; break;
                case 16003: if (client.Player.Class <= 16057 && client.Player.Class >= 16003) return true; break;
                case 16004: if (client.Player.Class <= 16057 && client.Player.Class >= 16004) return true; break;
                case 16005: if (client.Player.Class <= 16057 && client.Player.Class >= 16005) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static byte GetNextRefineryItem()
        {
            if (BaseFunc.RandGet(100, false) < 30)
                return 2;
            if (BaseFunc.RandGet(100, false) < 30)
                return 1;
            return 0;
        }

        public static byte GetLevel(uint ID)
        {
            if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Armor || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Head || IsShield(ID))
                return (byte)((ID % 100) / 10);
            else
                return (byte)((ID % 1000) / 10);
        }

        public static int GetUpEpLevelInfo(uint ID)
        {
            int cost = 0;
            int nLev = GetLevel(ID);

            if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Armor || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Head || IsShield(ID))
            {
                switch (nLev)
                {
                    case 5: cost = 50; break;
                    case 6: cost = 40; break;
                    case 7: cost = 30; break;
                    case 8:
                    case 9: cost = 20; break;
                    default: cost = 500; break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = cost * 10 / 100; break;
                    default:
                        break;
                }
            }
            else
            {
                switch (nLev)
                {
                    case 11: cost = 95; break;
                    case 12: cost = 90; break;
                    case 13: cost = 85; break;
                    case 14: cost = 80; break;
                    case 15: cost = 75; break;
                    case 16: cost = 70; break;
                    case 17: cost = 65; break;
                    case 18: cost = 60; break;
                    case 19: cost = 55; break;
                    case 20: cost = 50; break;
                    case 21: cost = 45; break;
                    case 22: cost = 40; break;
                    default:
                        cost = 500;
                        break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = cost * 10 / 100; break;
                    default:
                        break;
                }
            }
            return (100 / cost + 1) * 12 / 10;
        }
        public static int GetUpEpQualityInfo(uint ID)
        {
            var item = Pool.ItemsBase[ID];
            int change = 100;
            switch (ID % 10)
            {
                case 6: change = 50; break;
                case 7: change = 33; break;
                case 8: change = 20; break;
                default: change = 100; break;
            }
            int level = item.Level;
            if (level > 70)
                change = (int)(change * (100 - (level - 70) * 1.0) / 100);

            return Math.Max(1, 100 / change);
        }
        public static bool UpQualityDB(uint ID, uint DBs)
        {
            int cost = GetUpEpQualityInfo(ID);
            if (DBs >= cost)
                return true;
            else
            {
                double percent = 100 / cost;
                double MyCost = DBs * percent;
                return BaseFunc.RandGet(100, true) < MyCost;
            }
        }
        public static bool UpItemMeteors(uint ID, uint Meteors)
        {
            int CompleteCost = GetUpEpLevelInfo(ID);
            if (Meteors >= CompleteCost)
                return true;
            else
            {
                double percent = 100 / CompleteCost;
                double MyCost = Meteors * percent;
                return BaseFunc.RandGet(100, true) < MyCost;
            }
        }
        public static int RateSuccForQuality(uint wQuality)
        {
            if (wQuality <= 6) return 30;
            else if (wQuality == 7) return 12;
            else if (wQuality == 8) return 6;//6
            else if (wQuality == 9) return 5;//4

            return 0;
        }

        public static bool EquipPassSexReq(DBItem baseInformation, Client.GameClient client)
        {
            int ClientGender = client.Player.Body / 1000;
            if (baseInformation.Gender == ClientGender || baseInformation.Gender == 0)
                return true;
            return false;
        }
        public static bool EquipPassRbReq(DBItem baseInformation, Client.GameClient client)
        {
            if (baseInformation.Level < 71 && client.Player.Reborn > 0 && client.Player.Level >= 70)
                return true;
            else
                return false;
        }
        public static bool EquipPassLvlReq(DBItem baseInformation, Client.GameClient client)
        {
            if (client.Player.Level < baseInformation.Level)
                return false;
            else
                return true;
        }
        public static bool Equipable(Game.MsgServer.MsgGameItem item, Client.GameClient client)
        {
            DBItem BaseInformation = null;
            if (Pool.ItemsBase.TryGetValue((uint)item.ITEM_ID, out BaseInformation))
            {
                bool pass = false;
                if (!EquipPassSexReq(BaseInformation, client))
                    return false;
                if (EquipPassRbReq(BaseInformation, client))
                    pass = true;
                else
                    if (EquipPassJobReq(BaseInformation, client) && EquipPassStatsReq(BaseInformation, client) && EquipPassLvlReq(BaseInformation, client))
                        pass = true;
                if (!pass)
                    return false;

                if (client.Player.Reborn > 0)
                {
                    if (client.Player.Level >= 70 && BaseInformation.Level <= 70)
                        return pass;
                }
                return pass;
            }
            return false;
        }
        public static bool Equipable(uint ItemID, Client.GameClient client)
        {
            DBItem BaseInformation = null;
            if (Pool.ItemsBase.TryGetValue(ItemID, out BaseInformation))
            {
                bool pass = false;

                if (EquipPassRbReq(BaseInformation, client))
                    pass = true;
                else
                    if (EquipPassJobReq(BaseInformation, client) && EquipPassLvlReq(BaseInformation, client))
                        pass = true;
                if (!pass)
                    return false;

                if (client.Player.Reborn > 0)
                {
                    if (client.Player.Level >= 70 && BaseInformation.Level <= 70)
                        return pass;
                }
                return pass;
            }
            return false;
        }
        public static uint GetBaseID(uint ID)
        {
            int itemtype = (int)(ID / 1000);
            if (ID == 300000)//steed
                return ID;
            switch ((byte)(ID / 10000))
            {
                case 20://tower/ Fan / RidingCrop
                case 14://hood`s. cap`s . band`s
                case 11://helment / hat / cap...
                case 90://shields
                case 13://armors
                case 12://necklace / bag / hood
                case 15://ring
                case 16://boots
                case 42:/*BackswordID*/

                    if (itemtype == 420)//Normal Sword
                        goto default;
                    ID = (uint)(ID - (ID % 10));
                    break;
                case 51:/*Sytch*/
                    {
                        ID = (uint)(ID - (ID % 10));
                        break;
                    }

                case 60:/*NinjaSwordID*/
                case 61:/*AssasinKnifeID*/
                case 62://epic tao backsword
                case 67:
                case 68:
                    ID = (uint)(ID - (ID % 10));
                    break;
                default://someting weapon`s / someting coronet`s earing`s / bow`s
                    {
                        byte def_val = (byte)(ID / 100000);
                        ID = (uint)(((def_val * 100000) + (def_val * 10000) + (def_val * 1000)) + ((ID % 1000) - (ID % 10)));
                        break;
                    }
            }
            return ID;
        }
        public class ITPlus
        {
            public uint ID;
            public byte Plus;
            public ushort ItemHP;
            public uint MinAttack;
            public uint MaxAttack;
            public ushort PhysicalDefence;
            public ushort MagicAttack;
            public ushort MagicDefence;
            public ushort Agility;
            public ushort Vigor { get { return Agility; } }
            public byte Dodge;
            public ushort SpeedPlus { get { return Dodge; } }
            public void Parse(string Line)
            {
                string[] Info = Line.Split(' ');
                ID = uint.Parse(Info[0]);
                Plus = byte.Parse(Info[1]);
                ItemHP = ushort.Parse(Info[2]);
                MinAttack = uint.Parse(Info[3]);
                MaxAttack = uint.Parse(Info[4]);
                PhysicalDefence = ushort.Parse(Info[5]);
                MagicAttack = ushort.Parse(Info[6]);
                MagicDefence = ushort.Parse(Info[7]);
                Agility = ushort.Parse(Info[8]);
                Dodge = byte.Parse(Info[9]);

            }
        }
        public class DBItem
        {
            public uint ID;
            public ITPlus[] Plus;
            public bool AllowUpgradePlus = false;
            public string Name;
            public uint Class;
            public byte Proficiency;
            public byte Level;
            public byte Gender;
            public ushort Strength;
            public ushort Agility;
            public uint GoldWorth;
            public ushort MinAttack;
            public ushort MaxAttack;
            public ushort PhysicalDefence, PhysicalDamageIncrease, PhysicalDamageDecrease;
            public ushort MagicDefence;
            public ushort MagicAttack, MagicDamageIncrease, MagicDamageDecrease;
            public uint RuneSpellID, Sort;
            public byte BonusAttribute;
            public byte Dodge;
            public ushort Frequency;
            public uint ConquerPointsWorth;
            public ushort Durability;
            public ushort StackSize;
            public ushort ItemHP;
            public ushort ItemMP;
            public ushort AttackRange;
            public ItemType Type;
            public string Description;
            public int GradeKey;

            public uint Dash;
            public uint RDash;

            public uint Crytical;
            public uint SCrytical;
            public uint Imunity;
            public uint Penetration;
            public uint Block;
            public uint BreackTrough;
            public uint ConterAction;
            public uint Detoxication;

            public uint MetalResistance = 0;
            public byte RuneView;

            public uint WoodResistance = 0;
            public uint WaterResistance = 0;
            public uint FireResistance = 0;
            public uint EarthResistance = 0;
            public uint Save_Time;
            public ushort PurificationLevel;
            public ushort PurificationMeteorNeed;
            public uint Time;
            /// <summary>[HotReload] true when Parse read the whole line without an error.</summary>
            public bool ParsedOk;
            /// <summary>[HotReload] set by a live itemtype reload: parse values only, don't register the item in the
            /// shared static tables (Garments/Accessorys/SteedMounts/PurificationItems/EonspiritItem) that game threads read.</summary>
            [ThreadStatic]
            public static bool SkipStaticTables;
            public void Parse(string Line)
            {
                Plus = new ITPlus[13];
                string[] data = Line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (data[2] == "")
                {
                    for (int x = 2; x < data.Length - 1; x++)
                        data[x] = data[x + 1];
                }
                try
                {
                    if (data.Length > 52 && data[0] != "\0")
                    {
                        ID = Convert.ToUInt32(data[0]);

                        Name = data[1].Trim();
                        if (String.IsNullOrEmpty(Name) || Name == "0")
                            return;
                        Class = Convert.ToUInt32(data[2]);
                        Proficiency = Convert.ToByte(data[3]);
                        Level = Convert.ToByte(data[4]);
                        Gender = Convert.ToByte(data[5]);
                        Strength = Convert.ToUInt16(data[6]);
                        Agility = Convert.ToUInt16(data[7]);
                        Type = Convert.ToUInt32(data[10]) == 0 ? ItemType.Dropable : ItemType.Others;
                        GoldWorth = Convert.ToUInt32(data[12 - 1]);
                        MaxAttack = Convert.ToUInt16(data[14 - 1]);
                        MinAttack = Convert.ToUInt16(data[15 - 1]);
                        PhysicalDefence = Convert.ToUInt16(data[16 - 1]);
                        Frequency = Convert.ToUInt16(data[17 - 1]);
                        Dodge = Convert.ToByte(data[18 - 1]);
                        ItemHP = Convert.ToUInt16(data[19 - 1]);
                        ItemMP = Convert.ToUInt16(data[20 - 1]);
                        Durability = Convert.ToUInt16(data[22 - 1]);
                        MagicAttack = Convert.ToUInt16(data[30 - 1]);
                        MagicDefence = Convert.ToUInt16(data[31 - 1]);
                        AttackRange = Convert.ToUInt16(data[32 - 1]);
                        ConquerPointsWorth = Convert.ToUInt32(data[37 - 1]);

                        Crytical = Convert.ToUInt32(data[40 - 1]);
                        SCrytical = Convert.ToUInt32(data[41 - 1]);
                        Imunity = Convert.ToUInt32(data[42 - 1]);
                        Penetration = Convert.ToUInt32(data[43 - 1]);
                        Block = Convert.ToUInt32(data[44 - 1]);
                        BreackTrough = Convert.ToUInt32(data[45 - 1]);
                        ConterAction = Convert.ToUInt32(data[46 - 1]);

                        Save_Time = Convert.ToUInt32(data[38]);
                        MetalResistance = Convert.ToUInt16(data[48 - 1]);
                        WoodResistance = Convert.ToUInt16(data[49 - 1]);
                        WaterResistance = Convert.ToUInt16(data[50 - 1]);
                        FireResistance = Convert.ToUInt16(data[51 - 1]);
                        EarthResistance = Convert.ToUInt32(data[52 - 1]);
                        RuneView = Convert.ToByte(data[26]);

                        ushort.TryParse(data[47 - 1].ToString(), out StackSize);
                        if (ID == (uint)4033301)
                            StackSize = (ushort)GoldWorth;
                        //if (ID == (uint)DragonBall)
                           // StackSize = (ushort)GoldWorth;
                        Description = data[53 - 1].Replace("`s", "");
                        if (Description == "NinjaKatana")
                            Description = "NinjaWeapon";
                        if (Description == "Earrings")
                            Description = "Earring";
                        if (Description == "Bow")
                            Description = "ArcherBow";
                        if (Description == "Backsword")
                            Description = "TaoistBackSword";
                        Description = Description.ToLower();
                        if (ID == 2169165 || ID == 2169645 || ID == 2100075 || ID == 2100095)
                        {
                            Dash = Convert.ToUInt32(data[65]);
                            RDash = Convert.ToUInt32(data[66]);
                        }
                        if (ID >= 730001 && ID <= 730009)
                        {
                            Name += "(+" + (ID % 10).ToString() + ")";
                        }

                        if (data.Length >= 56)
                        {
                            ushort.TryParse(data[56 - 1].ToString(), out PurificationLevel);
                            ushort.TryParse(data[57 - 1].ToString(), out PurificationMeteorNeed);

                            if (PurificationLevel != 0 && ID != 729305 && ID != 727465 && !SkipStaticTables)
                            {
                                if (!PurificationItems.ContainsKey(PurificationLevel))
                                    PurificationItems.Add(PurificationLevel, new Dictionary<uint, DBItem>());

                                if (!PurificationItems[PurificationLevel].ContainsKey(ID))
                                    PurificationItems[PurificationLevel].Add(ID, this);
                            }
                        }
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                        {
                            if (!SkipStaticTables && !Accessorys.ContainsKey(ID))
                                Accessorys.Add(ID, this);
                            StackSize = 1;
                        }
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                            if (!SkipStaticTables && !SteedMounts.ContainsKey(ID))
                                SteedMounts.Add(ID, this);
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Garment)
                            if (!SkipStaticTables && !Garments.ContainsKey(ID))
                                Garments.Add(ID, this);
                        if (ID == 754099)
                            ConquerPointsWorth = 299;
                        if (ID == 754999 || ID == 753999 || ID == 751999)
                            ConquerPointsWorth = 1699;

                        if (ID == 619028)
                            ConquerPointsWorth = 489;

                        if (ID == 723723)
                            ConquerPointsWorth = 7100;
                        if (ID == 3005945)
                            ConquerPointsWorth = 3100;

                        RuneSpellID = Convert.ToUInt32(data[25]);
                        BonusAttribute = data.Length > 54 ? Convert.ToByte(data[54]) : (byte)0;
                        Sort = Convert.ToUInt32(data[58]);
                        PhysicalDamageIncrease = data.Length > 60 ? Convert.ToUInt16(data[60]) : (ushort)0;
                        MagicDamageIncrease = data.Length > 61 ? Convert.ToUInt16(data[61]) : (ushort)0;
                        PhysicalDamageDecrease = data.Length > 62 ? Convert.ToUInt16(data[62]) : (ushort)0;
                        MagicDamageDecrease = data.Length > 63 ? Convert.ToUInt16(data[63]) : (ushort)0;
                        if (!SkipStaticTables && ((ID >= 10020101 && ID <= 10020110) || (ID >= 10040101 && ID <= 10040110) || (ID >= 10100101 && ID <= 10100110) || (ID >= 10060101 && ID <= 10060110)))
                        {
                            EonspiritItem.Add(ID);
                        }
                        ParsedOk = true;
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
            public enum ItemType : byte
            {
                Dropable = 0,
                Others
            }
        }


    }
}