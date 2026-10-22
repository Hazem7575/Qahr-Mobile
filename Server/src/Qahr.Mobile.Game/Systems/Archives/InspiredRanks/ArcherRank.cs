using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Client;
using System.Threading;

namespace Qahr.Mobile.Database
{
    public class ArcherRank
    {
        public enum Type : uint
        {
            Overall,
            StoneCracker,
            ColdMoon,
            ThronCutter,
            WeeklyArcher,
            Count,
        }
        public class Entry
        {
            public ArcherRank.Type Type;
            public uint UID;
            public string Name;
            public uint Class;
            public byte Level;
            public uint Mesh;
            public uint TotalPoints;
            public uint HairStyle;
            public uint Head;
            public uint Garment;
            public uint LeftWeapon;
            public uint LefttWeaponAccessory;
            public uint RightWeapon;
            public uint RightWeaponAccessory;
            public uint MountArmor;
            public uint Armor;
            public uint Wing;
            public uint WingPlus;
            public uint Title;
            public uint Flag;
            public string GuildName = "";
            public uint WeaponWar;
            public void AddInfo(GameClient user)
            {
                this.HairStyle = user.Player.Hair;
                this.Garment = user.Player.GarmentId;
                if (user.Equipment != null)
                {
                    this.Head = user.Equipment.HeadID;
                    this.LeftWeapon = user.Equipment.LeftWeapon;
                    this.RightWeapon = user.Equipment.RightWeapon;
                }
                this.RightWeaponAccessory = user.Player.RightWeaponAccessoryId;
                this.LefttWeaponAccessory = user.Player.LeftWeaponAccessoryId;
                this.MountArmor = user.Player.MountArmorId;
                this.Wing = user.Player.WingId;
                if (user.Player.SpecialWingID > 0U)
                    this.Wing = user.Player.SpecialWingID;
                this.WingPlus = (uint)user.Player.WingPlus;
                this.Title = user.Player.SpecialTitleID;
                this.Armor = user.Player.ArmorId;
                if (user.Player.MyGuild != null)
                    this.GuildName = user.Player.MyGuild.GuildName;
                if (user.MyArchives.isOpen() != null)
                    this.WeaponWar = (uint)user.MyArchives.isOpen().ItemID;
                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopTrojan))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopTrojan;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWarrior))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWarrior;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopArcher))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopArcher;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopNinja))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopNinja;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMonk))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMonk;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopPirate))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopPirate;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDragonLee))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDragonLee;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopFireTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopFireTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarFiveStars))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarFiveStars;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarThreeStars))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarThreeStars;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarOneStar))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarOneStar;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMrsConquer))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMrsConquer;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMrConquer))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMrConquer;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopGuildLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopGuildLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWindWalker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWindWalker;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopThunderstriker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopThunderstriker;
            }

            public static implicit operator Game.MsgServer.MsgRankMemberShow.MsgRankMemberShowProto.Entry(ArcherRank.Entry BestOf)
            {
                return new Game.MsgServer.MsgRankMemberShow.MsgRankMemberShowProto.Entry()
                {
                    EntityUID = BestOf.UID,
                    Flag = BestOf.Flag,
                    Garment = BestOf.Garment,
                    GuildName = BestOf.GuildName,
                    HairStyle = BestOf.HairStyle,
                    Head = BestOf.Head,
                    LefttWeaponAccessory = BestOf.LefttWeaponAccessory,
                    LeftWeapon = BestOf.LeftWeapon,
                    Mesh = BestOf.Mesh,
                    MountArmor = BestOf.MountArmor,
                    Name = BestOf.Name,
                    Rank = 1,
                    RightWeapon = BestOf.RightWeapon,
                    RightWeaponAccessory = BestOf.RightWeaponAccessory,
                    Title = BestOf.Title,
                    Type = 2,
                    Armor = BestOf.Armor,
                    Wing = BestOf.Wing,
                    WingPlus = BestOf.WingPlus,
                    WeaponWar = BestOf.WeaponWar,
                    Profession = BestOf.Class
                };
            }
            public Entry ShallowCopy()
            {
                return (Entry)this.MemberwiseClone();
            }

        }
        public class Rank
        {
            public ArcherRank.Type _Type;
            public int MaxItems;
            public Dictionary<uint, ArcherRank.Entry> RankingList;
            public object SynRoot;
            public Rank(Type typ)
            {
                _Type = typ;
                SynRoot = new object();
                RankingList = new Dictionary<uint, Entry>();
            }

            public void Remove(uint UID)
            {
                lock (SynRoot)
                {
                    if (RankingList.ContainsKey(UID))
                    {
                        RankingList.Remove(UID);
                        RankingList = RankingList.OrderByDescending(p => p.Value.TotalPoints).Take(MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                    }
                }
            }
            public void UpdateItem(ArcherRank.Entry item)
            {
                lock (SynRoot)
                {
                    if (!RankingList.ContainsKey(item.UID))
                        RankingList.Add(item.UID, item);
                    else if (RankingList.ContainsKey(item.UID))
                        RankingList[item.UID] = item;
                    RankingList = RankingList.OrderByDescending(p => p.Value.TotalPoints).Take(MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                }
            }
        }
      
        public static System.SafeDictionary<Type, Rank> Ranks = new System.SafeDictionary<Type, Rank>();
        public static void Create()
        {
            for (int x = 0; x < (byte)Type.Count; x++)
                Ranks.Add((Type)x, new Rank((Type)x) { MaxItems = 10 });
           
        }
        public static Entry GetInfo(Type typ, uint UID)
        {
            if (Ranks[typ].RankingList.Values.Count(i => i.UID == UID) > 0)
            {
                var item = Ranks[typ].RankingList.Values.FirstOrDefault(i => i.UID == UID);
                return item;
            }
            return null;
        }
        public static uint GetMyRank(Type typ, uint UID)
        {
            if (Ranks[typ].RankingList.Values.Count(i => i.UID == UID) > 0)
            {
                var item = Ranks[typ].RankingList.Values.FirstOrDefault(i => i.UID == UID);
                return (uint)(Ranks[typ].RankingList.Values.ToList().IndexOf(item) + 1);
            }
            return 0;
        }

        public static void Remove(uint UID)
        {
            foreach (var rank in Ranks.Values)
            {
                rank.Remove(UID);
            }
        }
        public static Entry BestOf(Type type)
        {
            return Ranks[type].RankingList.Values.FirstOrDefault();
        }
        public static void Load()
        {
            ArcherRank.Create();
            using (Read read = new Read("ArcherRank.txt"))
            {
                if (!read.Reader())
                    return;
                int count = read.Count;
                for (int index = 0; index < count; ++index)
                {
                    ReadLine readLine = new ReadLine(read.ReadString("/"), '/');
                    Entry entry = new Entry();
                    entry.UID = readLine.Read((uint)0);
                    entry.Name = readLine.Read("");
                    entry.Type = (ArcherRank.Type)readLine.Read((byte)0);
                    entry.TotalPoints = readLine.Read((uint)0);
                    entry.Class = readLine.Read((uint)0);
                    entry.Level = readLine.Read((byte)0);
                    entry.Mesh = readLine.Read((uint)0);
                    entry.HairStyle = readLine.Read((uint)0);
                    entry.Head = readLine.Read((uint)0);
                    entry.Garment = readLine.Read((uint)0);
                    entry.LeftWeapon = readLine.Read((uint)0);
                    entry.LefttWeaponAccessory = readLine.Read((uint)0);
                    entry.RightWeapon = readLine.Read((uint)0);
                    entry.RightWeaponAccessory = readLine.Read((uint)0);
                    entry.MountArmor = readLine.Read((uint)0);
                    entry.Armor = readLine.Read((uint)0);
                    entry.Wing = readLine.Read((uint)0);
                    entry.WingPlus = readLine.Read((uint)0);
                    entry.Title = readLine.Read((uint)0);
                    entry.Flag = readLine.Read((uint)0);
                    entry.GuildName = readLine.Read("");
                    entry.WeaponWar = readLine.Read((uint)0);
                    ArcherRank.Ranks[entry.Type].UpdateItem(entry);
                }
            }
        }

        public static void Save()
        {
            using (Write write = new Write("ArcherRank.txt"))
            {
                var mains = Ranks.GetValues().ToArray();
                foreach (var rank in mains)
                {
                    var ranks = rank.RankingList.Values.ToArray();
                    foreach (var obj in ranks)
                    {
                        if (obj.UID >= 3999900001)
                            continue;
                        WriteLine writeLine = new WriteLine('/');
                        writeLine.Add(obj.UID)
                            .Add(obj.Name)
                            .Add((byte)obj.Type)
                            .Add(obj.TotalPoints)
                            .Add(obj.Class)
                            .Add(obj.Level)
                            .Add(obj.Mesh)
                            .Add(obj.HairStyle)
                            .Add(obj.Head)
                            .Add(obj.Garment)
                            .Add(obj.LeftWeapon)
                            .Add(obj.LefttWeaponAccessory)
                            .Add(obj.RightWeapon)
                            .Add(obj.RightWeaponAccessory)
                            .Add(obj.MountArmor)
                            .Add(obj.Armor)
                            .Add(obj.Wing)
                            .Add(obj.WingPlus)
                            .Add(obj.Title)
                            .Add(obj.Flag)
                            .Add(obj.GuildName)
                            .Add(obj.WeaponWar);
                        write.Add(writeLine.Close());
                    }
                }
                write.Execute(Mode.Open);
            }
        }
        //public static void Load()
        //{
        //    ArcherRank.Create();
        //    using (var cmd = new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.SELECT).Select("ArcherRank"))
        //    using (var reader = new DBFunctionality.MySqlReader(cmd))
        //    {
        //        while (reader.Read())
        //        {
        //            Entry entry = new Entry();
        //            entry.UID = reader.ReadUInt32("UID");
        //            entry.Name = reader.ReadString("Name");
        //            entry.Type = (ArcherRank.Type)reader.ReadUInt32("Type");
        //            entry.TotalPoints = reader.ReadUInt32("TotalPoints");
        //            entry.Class = reader.ReadUInt32("Class");
        //            entry.Level = reader.ReadByte("Level");
        //            entry.Mesh = reader.ReadUInt32("Mesh");
        //            entry.HairStyle = reader.ReadUInt32("HairStyle");
        //            entry.Head = reader.ReadUInt32("Head");
        //            entry.Garment = reader.ReadUInt32("Garment");
        //            entry.LeftWeapon = reader.ReadUInt32("LeftWeapone");
        //            entry.LefttWeaponAccessory = reader.ReadUInt32("LefttWeaponeAccessory");
        //            entry.RightWeapon = reader.ReadUInt32("RightWeapone");
        //            entry.RightWeaponAccessory = reader.ReadUInt32("RighhtWeaponAccessory");
        //            entry.MountArmor = reader.ReadUInt32("MountArmor");
        //            entry.Armor = reader.ReadUInt32("Armor");
        //            entry.Wing = reader.ReadUInt32("Wing");
        //            entry.WingPlus = reader.ReadUInt32("WingPlus");
        //            entry.Title = reader.ReadUInt32("Title");
        //            entry.Flag = reader.ReadUInt32("Flag");
        //            entry.GuildName = reader.ReadString("GuildName");
        //            entry.WeaponWar = reader.ReadUInt32("Weaponewar");
        //            ArcherRank.Ranks[entry.Type].UpdateItem(entry);
        //        }
        //    }
        //}
        //public static void Save()
        //{
        //    var mains = Ranks.GetValues().ToArray();
        //    foreach (var rank in mains)
        //    {
        //        var ranks = rank.RankingList.Values.ToArray();
        //        foreach (var obj in ranks)
        //        {
        //            new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.INSERT).Insert("ArcherRank")
        //                .Insert("UID", obj.UID)
        //                .Insert("Name", obj.Name)
        //                .Insert("Type", (byte)obj.Type)
        //                .Insert("TotalPoints", obj.TotalPoints)
        //                .Insert("Class", obj.Class)
        //                .Insert("Level", obj.Level)
        //                .Insert("Mesh", obj.Mesh)
        //                .Insert("HairStyle", obj.HairStyle)
        //                .Insert("Head", obj.Head)
        //                .Insert("Garment", obj.Garment)
        //                .Insert("LeftWeapone", obj.LeftWeapon)
        //                .Insert("LefttWeaponeAccessory", obj.LefttWeaponAccessory)
        //                .Insert("RightWeapone", obj.RightWeapon)
        //                .Insert("RighhtWeaponAccessory", obj.RightWeaponAccessory)
        //                .Insert("MountArmor", obj.MountArmor)
        //                .Insert("Armor", obj.Armor)
        //                .Insert("Wing", obj.Wing)
        //                .Insert("WingPlus", obj.WingPlus)
        //                .Insert("Title", obj.Title)
        //                .Insert("Flag", obj.Flag)
        //                .Insert("GuildName", obj.GuildName)
        //                .Insert("Weaponewar", obj.WeaponWar).Execute();
        //        }
        //    }
        //}


    }
}
     

        

