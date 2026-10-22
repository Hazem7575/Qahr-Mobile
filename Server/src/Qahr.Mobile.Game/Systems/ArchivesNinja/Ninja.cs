using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Ninja
    {
        public enum ItemType : ulong
        {
            WildSigil = 100,//x
            WildSigilBurning = 101,//1
            WildSigilRapid = 102,//x

            FlameSigil = 103,//x
            FlameSigilRapid = 104,//x
            FlameSigilScorch = 105,//x

            PrisonSigil = 200,//x
            PrisonSigilFatigue = 201,//x
            PrisonSigilFutility = 202,//2

            DragonSigil = 203,//x
            DragonSigilBillow = 204,//x
            DragonSigilFading = 205,//3

            MudSigil = 300,//x
            MudSigilImpeccable = 301,//x
            MudSigilSolidity = 302,//x

            DustSigil = 303,//x
            DustSigilExtinction = 304,//x
            DustSigilStunn = 305,//4

            SickleSigil = 400,//x
            SickleSigilFlurry = 401,//5
            SickleSigilGallop = 402,//x

            WhirlSigil = 403,//x
            WhirlSigilSage = 404,//x
            WhirlSigilAura = 405,//6

            KylinSigil = 500,//x
            KylinSigilBurial = 501,//7
            KylinSigilSeal = 502,//8

            SlashSeal = 503,//x
            SlashSealFlash = 504,//9
            SlashSealProwess = 505,//10

            PaperDance = 600,//x
            HellVortex = 601,//x
            InfiniteMist = 602,//x
            HeavenSWonder = 603,//x
            BonePulse = 604,//x
        }
        public enum Flags : ulong
        {
            Gate_of_Dawn = 1UL << 0,
            Gate_of_Rest = 1UL << 1,
            Gate_of_Life = 1UL << 2,
            Gate_of_Pain = 1UL << 3,
            Gate_of_Limlt = 1UL << 4,
            Gate_of_View = 1UL << 5,
            Gate_of_Shock = 1UL << 6,
            Gate_of_Death = 1UL << 7,
            Gate_of_Dawn_2 = 1UL << 8,
            Gate_of_Rest_2 = 1UL << 9,
            Gate_of_Life_2 = 1UL << 10,
            Gate_of_Pain_2 = 1UL << 11,
            Gate_of_Limlt_2 = 1UL << 12,
            Gate_of_View_2 = 1UL << 13,
            Gate_of_Shock_2 = 1UL << 14,
            Gate_of_Death_2 = 1UL << 15,
            Full =
                Gate_of_Dawn | Gate_of_Rest | Gate_of_Life | Gate_of_Pain | Gate_of_Limlt | Gate_of_View | Gate_of_Shock | Gate_of_Death
                | Gate_of_Dawn_2 | Gate_of_Rest_2 | Gate_of_Life_2 | Gate_of_Pain_2 | Gate_of_Limlt_2 | Gate_of_View_2 | Gate_of_Shock_2 | Gate_of_Death_2,
        }
        public enum UnlockMasterys : ulong
        {
            FireMastery = 1UL << 0,
            WaterMastery = 1UL << 1,
            EarthMastery = 1UL << 2,
            WindMastery = 1UL << 3,
            LightningMastery = 1UL << 4,
        }
        public ConcurrentDictionary<uint, Item> Items;
        public Client.GameClient user;
        public Ninja(Client.GameClient _user)
        {
            user = _user;
            Items = new ConcurrentDictionary<uint, Item>();
        }
        public bool TryGetValueEquip(ItemType id, out Item obj)
        {
            obj = null;
            if (!Unlocked || !Valid()) return false;
              
            if (user.Equipment.Alternante)
            {
                for (int i = 9; i < 17; i++)
                {
                    var Item = Items.Values.Where(p => p.Position == i && p.ItemID == (uint)id).FirstOrDefault();
                    if (Item != null)
                        obj = Item;
                    else
                    {
                        Item = Items.Values.Where(p => p.Position == i - 8 && p.ItemID == (uint)id).FirstOrDefault();
                        if (Item != null)
                            obj = Item;
                    }
                }
            }
            else
            {
                for (int i = 1; i < 9; i++)
                {
                    var Item = Items.Values.Where(p => p.Position == i && p.ItemID == (uint)id).FirstOrDefault();
                    if (Item != null)
                        obj = Item;
                }
            }
            if (obj != null)
                return true;
            return false;
        }
        public bool TryGetValue(uint id, out Item obj)
        {
            obj = null;
            if (!Unlocked || !Valid()) return false;
            obj = Items.Values.Where(p => p.ItemID == id).FirstOrDefault();
            if (obj != null) return true;
            return false;
        }
        public void UnEquipAll()
        {
            if (Items.Values.Count(p => p.Position > 0) > 0)
            {
                foreach (var item in Items.Values.Where(p => p.Position > 0))
                    item.Position = 0;
                GetLevel();
                Alternate();
                user.Equipment.QueryEquipment(user.Equipment.Alternante);
            }
        }
        public void AddFlag(ulong flag)
        {
            if (!ContainsFlag(flag))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Flag |= flag;
                    MsgGouYuAptitude.MsgGouYuAptitudeProto obj = new MsgGouYuAptitude.MsgGouYuAptitudeProto();
                    obj.Type = (uint)MsgGouYuAptitude.MsgGouYuAptitudeProto.TypeID.Flag;
                    obj.Flag = Flag;
                    user.Send(stream.CreateNinjaUser(obj));
                }
            }
        }
        public bool ContainsFlag(ulong flag)
        {
            ulong aux = Flag;
            aux &= ~flag;
            return !(aux == Flag);
        }
        public void RemoveFlag(ulong flag)
        {
            if (ContainsFlag(flag))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Flag &= ~flag;
                    MsgGouYuAptitude.MsgGouYuAptitudeProto obj = new MsgGouYuAptitude.MsgGouYuAptitudeProto();
                    obj.Type = (uint)MsgGouYuAptitude.MsgGouYuAptitudeProto.TypeID.Flag;
                    obj.Flag = Flag;
                    user.Send(stream.CreateNinjaUser(obj));
                }
            }
        }
        public void AddFlagMastery(UnlockMasterys flag)
        {
            if (!ContainsFlagMastery(flag))
            {
                UnlockMastery |= (uint)flag;
            }
        }
        public bool ContainsFlagMastery(UnlockMasterys flag)
        {
            ulong aux = UnlockMastery;
            aux &= ~(uint)flag;
            return !(aux == UnlockMastery);
        }
        public void RemoveFlagMastery(UnlockMasterys flag)
        {
            if (ContainsFlagMastery(flag))
            {
                UnlockMastery &= ~(uint)flag;
            }
        }
        public void UpdateRank()
        {
            if (Unlocked && Valid())
            {
                
                if (Score >= 1000)
                {
                    var entry = new Database.NinjaRank.Entry()
                    {
                        Type = Database.NinjaRank.Type.Overall,
                        TotalPoints = Score,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry.AddInfo(user);
                    Database.NinjaRank.Ranks[Database.NinjaRank.Type.Overall].UpdateItem(entry);

                    if (Fire_Score >= 1000)
                    {
                        var entry2 = entry.ShallowCopy();
                        entry2.TotalPoints = Fire_Score;
                        entry2.Type = Database.NinjaRank.Type.Fire;
                        Database.NinjaRank.Ranks[Database.NinjaRank.Type.Fire].UpdateItem(entry2);
                    }
                    if (Water_Score >= 1000)
                    {
                        var entry3 = entry.ShallowCopy();
                        entry3.TotalPoints = Water_Score;
                        entry3.Type = Database.NinjaRank.Type.Water;
                        Database.NinjaRank.Ranks[Database.NinjaRank.Type.Water].UpdateItem(entry3);
                    }
                    if (Earth_Score >= 1000)
                    {
                        var entry4 = entry.ShallowCopy();
                        entry4.TotalPoints = Earth_Score;
                        entry4.Type = Database.NinjaRank.Type.Earth;
                        Database.NinjaRank.Ranks[Database.NinjaRank.Type.Earth].UpdateItem(entry4);
                    }
                    if (Wind_Score > 1000)
                    {
                        var entry5 = entry.ShallowCopy();
                        entry5.TotalPoints = Wind_Score;
                        entry5.Type = Database.NinjaRank.Type.Wind;
                        Database.NinjaRank.Ranks[Database.NinjaRank.Type.Wind].UpdateItem(entry5);
                    }
                    if (Lightning_Score > 1000)
                    {
                        var entry6 = entry.ShallowCopy();
                        entry6.TotalPoints = Lightning_Score;
                        entry6.Type = Database.NinjaRank.Type.Lightning;
                        Database.NinjaRank.Ranks[Database.NinjaRank.Type.Lightning].UpdateItem(entry6);
                    }
                }
            }
            else Database.NinjaRank.Remove(user.Player.UID);
        }
        public void AddPoints(uint _Points)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Exp += _Points;
                MsgGouYuAptitude.MsgGouYuAptitudeProto obj = new  MsgGouYuAptitude.MsgGouYuAptitudeProto();
                obj.Type = (uint)MsgGouYuAptitude.MsgGouYuAptitudeProto.TypeID.Points;
                obj.Points = Exp;
                user.Send(stream.CreateNinjaUser(obj));
            }
        }
        public void AddItem(uint ID, uint Level, uint Position, uint Points, uint UK, bool Updat = true)
        {
            if (Level == 0) Level = 1;
            if (Level > 9) Level = 9;
            Item item = new Item();
            item.ItemID = (uint)ID;
            item.Level = Level;
            item.Position = Position;
            item.Exp = Points;
            item.UK = UK;

            if (!Items.ContainsKey(ID))
            {
                Items.Add(ID, item);
                if (Updat)
                    MsgGouYuInfo.MsgGouYuInfoProto.Create(user, item, (MsgGouYuInfo.MsgGouYuInfoProto.TypeID)2);
            }
            else
            {
                Items[ID] = item;
                if (Updat)
                    MsgGouYuInfo.MsgGouYuInfoProto.Create(user, item, (MsgGouYuInfo.MsgGouYuInfoProto.TypeID)2);
            }
            UpdateRank();
        }
        public bool Unlocked;
        public bool Valid()
        {
            return Database.AtributesStatus.IsNinja(user.Player.Class);
        }
        public ulong Flag;
        public static byte[] CoolDownChange = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10 };
        public uint Exp;
        public uint Fire;
        public uint Water;
        public uint Earth;
        public uint Wind;
        public uint Lightning;
        public uint tmp_Fire;
        public uint tmp_Water;
        public uint tmp_Earth;
        public uint tmp_Wind;
        public uint tmp_Lightning;
        public uint Score
        {
            get
            {

                return Fire_Score
                    + Water_Score
                    + Earth_Score
                    + Wind_Score
                    + Lightning_Score
                    + a_Score;
            }
        }
        public uint FireMastery = 0;
        public uint WaterMastery = 0;
        public uint EarthMastery = 0;
        public uint WindMastery = 0;
        public uint LightningMastery = 0;
        public uint UnlockMastery = 0;
        public uint SusanooLevel = 0;
        public uint SusanooExp;
        public uint Fire_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 100 && p.ItemID <= 105))
                {
                    score += item.DBItem.Score + (Fire * 4);
                }
                return score + FireMastery;
            }
        }
        public uint Water_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 200 && p.ItemID <= 205))
                {
                    score += item.DBItem.Score + (Water * 4);
                }
                return score + WaterMastery;
            }
        }
        public uint Earth_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 300 && p.ItemID <= 305))
                {
                    score += item.DBItem.Score + (Earth * 4);
                }
                return score + EarthMastery;
            }
        }
        public uint Wind_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 400 && p.ItemID <= 405))
                {
                    score += item.DBItem.Score + (Wind * 4);
                }
                return score + WindMastery;
            }
        }
        public uint Lightning_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 500 && p.ItemID <= 505))
                {
                    score += item.DBItem.Score + (Lightning * 4);
                }
                return score + LightningMastery;
            }
        }
        public uint a_Score
        {
            get
            {
                uint score = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 600 && p.ItemID <= 605))
                {
                    score += item.DBItem.Score;
                }
                return score;
            }
        }
        public uint Levels
        {
            get
            {
                uint level = 0;
                if (user.Equipment.Alternante)
                {
                    for (int i = 9; i < 17; i++)
                    {
                        var Item = Items.Values.Where(p => p.Position == i).FirstOrDefault();
                        if (Item != null)
                            level += Item.Level;
                        else
                        {
                            Item = Items.Values.Where(p => p.Position == i - 8).FirstOrDefault();
                            if (Item != null)
                                level += Item.Level;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 9; i++)
                    {
                        var Item = Items.Values.Where(p => p.Position == i).FirstOrDefault();
                        if (Item != null)
                            level += Item.Level;
                    }
                }
                return level;
            }
        }
        public class Item
        {
            public uint ItemID;
            public uint Level;
            public uint Position;
            public uint Exp;
            public uint UK;
           
            public NinjaFile.NinjaSprintItem DBItem
            {
                get
                {
                    var item = NinjaFile.gouyu_type.Values.Where(p => p.ItemID == ItemID && p.Level == Level).FirstOrDefault();
                    if (item != null)
                        return item;
                    return new NinjaFile.NinjaSprintItem();
                }
            }
        }
        public override string ToString()
        {
            var file = new Database.DBActions.WriteLine('/').Add(Unlocked ? (byte)1 : (byte)0);
            if (Unlocked)
            {
                file.Add(Flag);
                file.Add(Exp);
                file.Add(Fire);
                file.Add(Water);
                file.Add(Earth);
                file.Add(Wind);
                file.Add(Lightning);
                file.Add(tmp_Fire);
                file.Add(tmp_Water);
                file.Add(tmp_Earth);
                file.Add(tmp_Wind);
                file.Add(tmp_Lightning);
                file.Add(Score);
                file.Add(FireMastery);
                file.Add(WaterMastery);
                file.Add(EarthMastery);
                file.Add(WindMastery);
                file.Add(LightningMastery);
                file.Add(UnlockMastery);
                file.Add(Items.Count);
                foreach (var att in Items.Values)
                {

                    file.Add(att.ItemID);
                    file.Add(att.Level);
                    file.Add(att.Position);
                    file.Add(att.Exp);
                    file.Add(att.UK);
                }
                file.Add(SusanooLevel);
                file.Add(SusanooExp);
            }
            return file.Close();
        }
        public void Load(string line)
        {
            ReadLine reader = new ReadLine(line, '/');
             Unlocked = reader.Read((byte)0) == 1;
             if (Unlocked)
             {
                 Flag = reader.Read((ulong)0);
                 Exp = reader.Read((uint)0);
                 Fire = reader.Read((uint)0);
                 Water = reader.Read((uint)0);
                 Earth = reader.Read((uint)0);
                 Wind = reader.Read((uint)0);
                 Lightning = reader.Read((uint)0);
                 tmp_Fire = reader.Read((uint)0);
                 tmp_Water = reader.Read((uint)0);
                 tmp_Earth = reader.Read((uint)0);
                 tmp_Wind = reader.Read((uint)0);
                 tmp_Lightning = reader.Read((uint)0);
                 //Score
                 reader.Read((uint)0);
                 FireMastery = reader.Read((uint)0);
                 WaterMastery = reader.Read((uint)0);
                 EarthMastery = reader.Read((uint)0);
                 WindMastery = reader.Read((uint)0);
                 LightningMastery = reader.Read((uint)0);
                 UnlockMastery = reader.Read((uint)0);
                 int count = reader.Read((int)0);
                 for (int i = 0; i < count; i++)
                 {
                     Item item = new Item();

                     item.ItemID = reader.Read((uint)0);
                     item.Level = reader.Read((uint)0);
                     item.Position = reader.Read((uint)0);
                     item.Exp = reader.Read((uint)0);
                     item.UK = reader.Read((uint)0);
                     if (!Items.ContainsKey(item.ItemID))
                         Items.Add(item.ItemID, item);
                 }
                 SusanooLevel = reader.Read((uint)0);
                 SusanooExp = reader.Read((uint)0);
             }
             GetLevel();
             Alternate();
        }
        public ushort GetSkill()
        {
            List<ushort> CanUse = new List<ushort>();
            
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterPrisonPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.WaterPrisonPassive);
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlShurikenPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.WhirlShurikenPassive);
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DustDetachmentPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.DustDetachmentPassive);
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LightningSlashPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.LightningSlashPassive);
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SickleWindPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.SickleWindPassive);
            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildFireballPassive))
                CanUse.Add((ushort)Role.Flags.SpellID.WildFireballPassive);

            
           
            if (CanUse.Count != 0)
            {
                return (ushort)CanUse[Pool.GetRandom.Next(0, CanUse.Count)];
            }
            return 0;
        }
        public uint PerfectionScore
        {
            get
            {
                if(Perfection_Score > 82000)
                    return (uint)(82000);
                return (uint)(Perfection_Score);
            }
        }
        public ulong Perfection_Score
        {
            get
            {
                ulong WindSigil = 0, EarthSigil = 0, LightningSigil = 0, WaterSigil = 0, FireSigil = 0, BloodlineSigil = 0;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 400 && p.ItemID <= 405))
                {
                    WindSigil += item.DBItem.Score;
                }
                WindSigil = WindSigil * (500 + Wind) / 500;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 300 && p.ItemID <= 305))
                {
                    EarthSigil += item.DBItem.Score;
                }
                EarthSigil = EarthSigil * (500 + Earth) / 500;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 500 && p.ItemID <= 505))
                {
                    LightningSigil += item.DBItem.Score;
                }
                LightningSigil = LightningSigil * (500 + Lightning) / 500;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 100 && p.ItemID <= 105))
                {
                    FireSigil += item.DBItem.Score;
                }
                FireSigil = FireSigil * (500 + Fire) / 500;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 200 && p.ItemID <= 205))
                {
                    WaterSigil += item.DBItem.Score;
                }
                WaterSigil = WaterSigil * (500 + Water) / 500;
                foreach (var item in Items.Values.Where(p => p.ItemID >= 600 && p.ItemID <= 605))
                {
                    BloodlineSigil += item.DBItem.Score;
                }
                return FireSigil + WaterSigil + LightningSigil + EarthSigil + WindSigil + BloodlineSigil;
            }
        }
        public void Alternate()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                List<ushort> Skills = new List<ushort> 
            {
                16310, 
                16320, 
                16330,
                16340,
                16350, 
                16360, 
                16370,
                16380, 
                16390, 
                16400, 
                16410, 
                16420, 
                16430,
                16440,
                16450,
                16460,
                16470,
                16480, 
                16490, 
                16500, 
                16510, 
                16520, 
                16530, 
                16540, 
                16550 
            };
                if (user.Equipment.Alternante)
                {
                    for (int i = 9; i < 17; i++)
                    {
                        var Item = Items.Values.Where(p => p.Position == i).FirstOrDefault();
                        if (Item != null)
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic1))
                                user.MySpells.Add(stream, (ushort)Item.DBItem.magic1, (byte)Item.Level);
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic2))
                                user.MySpells.Add(stream, (ushort)Item.DBItem.magic2, (byte)Item.Level);
                            if (Skills.Contains((ushort)Item.DBItem.magic1))
                                Skills.Remove((ushort)Item.DBItem.magic1);
                            if (Skills.Contains((ushort)Item.DBItem.magic2))
                                Skills.Remove((ushort)Item.DBItem.magic2);
                        }
                        else
                        {
                            Item = Items.Values.Where(p => p.Position == i - 8).FirstOrDefault();
                            if (Item != null)
                            {
                                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic1))
                                    user.MySpells.Add(stream, (ushort)Item.DBItem.magic1, (byte)Item.Level);
                                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic2))
                                    user.MySpells.Add(stream, (ushort)Item.DBItem.magic2, (byte)Item.Level);
                                if (Skills.Contains((ushort)Item.DBItem.magic1))
                                    Skills.Remove((ushort)Item.DBItem.magic1);
                                if (Skills.Contains((ushort)Item.DBItem.magic2))
                                    Skills.Remove((ushort)Item.DBItem.magic2);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 9; i++)
                    {
                        var Item = Items.Values.Where(p => p.Position == i).FirstOrDefault();
                        if (Item != null)
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic1))
                                user.MySpells.Add(stream, (ushort)Item.DBItem.magic1, (byte)Item.Level);
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Item.DBItem.magic2))
                                user.MySpells.Add(stream, (ushort)Item.DBItem.magic2, (byte)Item.Level);
                            if (Skills.Contains((ushort)Item.DBItem.magic1))
                                Skills.Remove((ushort)Item.DBItem.magic1);
                            if (Skills.Contains((ushort)Item.DBItem.magic2))
                                Skills.Remove((ushort)Item.DBItem.magic2);
                        }
                    }
                }
                foreach (var r in Skills)
                {
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)r))
                        user.MySpells.Remove((ushort)r, stream);
                }
            }
        }
        public void GetLevel()
        {
            byte p = 0;
            if (user.MyNinja.Levels >= 72)
                p = 9;
            else if (user.MyNinja.Levels >= 64)
                p = 8;
            else if (user.MyNinja.Levels >= 56)
                p = 7;
            else if (user.MyNinja.Levels >= 48)
                p = 6;
            else if (user.MyNinja.Levels >= 40)
                p = 5;
            else if (user.MyNinja.Levels >= 32)
                p = 4;
            else if (user.MyNinja.Levels >= 24)
                p = 3;
            else if (user.MyNinja.Levels >= 15)
                p = 2;
            else if (user.MyNinja.Levels >= 9)
                p = 1;
            if (user.Player.NiniaP0 != p)
                user.Player.NiniaP0 = p;
        }
        public void SageMode(uint Timer)
        {
            user.Player.AddFlag(MsgUpdate.Flags.SageMode, (int)Timer, true);
            using (var recycledPacket = new ServerSockets.RecycledPacket())
            {
                var stream = recycledPacket.GetStream();
                {
                    user.Player.SendUpdate(stream, MsgUpdate.Flags.SageMode, (uint)Timer, 0, 0, MsgUpdate.DataType.ArchiveSkill, true);
                }
            }
        }
        public Item isOpen()
        {
            var Item = Items.Values.Where(p => p.ItemID != 0).FirstOrDefault(); return Item;
        }
         public void Loading()
        {
            if (Valid() && Unlocked)
            {
                UpdateRank();
                Alternate();
                GetLevel();
                MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(user, this, user.Player.UID);
                MsgGouYuInfo.MsgGouYuInfoProto.Create(user, this, MsgGouYuInfo.MsgGouYuInfoProto.TypeID.Load, user.Player.UID);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var packet = new MsgUpdate(stream, user.Player.UID, 1);
                    stream = packet.Append(stream, MsgUpdate.DataType.SageModeLevel, user.Player.NiniaP0);
                    stream = packet.GetArray(stream);
                    user.Send(stream);
                }
            }
        }
    }
}
