using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class PrestigeRanking
    {
        public enum Type : uint
        {
            World,
            Trojan,
            Warrior,
            Archer,
            Ninja,
            Monk,
            Pirate,
            DragonWarrior,
            WaterTao,
            FireTao,
            WindWalker,
            Thunderstriker,
            DuneWanderer,
            Count
        }

        public static Type GetIndex(uint Class)
        {
            if (Database.AtributesStatus.IsTrojan(Class))
                return Type.Trojan;
            if (Database.AtributesStatus.IsWarrior(Class))
                return Type.Warrior;
            if (Database.AtributesStatus.IsArcher(Class))
                return Type.Archer;
            if (Database.AtributesStatus.IsNinja(Class))
                return Type.Ninja;
            if (Database.AtributesStatus.IsMonk(Class))
                return Type.Monk;
            if (Database.AtributesStatus.IsPirate(Class))
                return Type.Pirate;
            if (Database.AtributesStatus.IsLee(Class))
                return Type.DragonWarrior;
            if (Database.AtributesStatus.IsWater(Class))
                return Type.WaterTao;
            if (Database.AtributesStatus.IsFire(Class))
                return Type.FireTao;
            if (Database.AtributesStatus.IsWindWalker(Class))
                return Type.WindWalker;
            if (Database.AtributesStatus.IsThunderStriker(Class))
                return Type.Thunderstriker;
            if (Database.AtributesStatus.IsDune(Class))
                return Type.DuneWanderer;
            return Type.WaterTao;
        }
        public class Entry
        {
            public uint UID;
            public uint[] Points;
            public uint TotalPoints = 0;
            public uint Class;
            public byte Level;
            public Type type;
            public uint Mesh;
            public string Name = "";

            public uint HairStyle;
            public uint Head;
            public uint Garment;
            public uint LeftWeapon;
            public uint LefttWeaponAccessory;
            public uint RightWeapon;
            public uint RightWeaponAccessory;
            public uint MountArmor;
            public uint Armor;//??
            public uint Wing;
            public uint WingPlus;
            public uint Title;
            public uint Flag;//??
            public string GuildName = "";
            public uint FrameID;
            public void AddInfo(Client.GameClient user)
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
                this.FrameID = user.Player.FrameID;
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
            public void UpdateInfo(Client.GameClient user)
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
                this.FrameID = user.Player.FrameID;
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
            public static implicit operator Game.MsgServer.MsgRankMemberShow.MsgRankMemberShowProto.Entry(Entry BestOf)
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
                    Type = 1,
                    Armor = BestOf.Armor,
                    Wing = BestOf.Wing,
                    WingPlus = BestOf.WingPlus,

                };
            }
        }
        public static void Remove(uint UID)
        {
            foreach (var rank in PrestigeRanking.Ranks.Values)
            {
                rank.Remove(UID);
            }
        }
        public static Entry BestOfTheWorld
        {
            get
            {
                try
                {
                    return Ranks[Type.World].BestOfClass;
                }
                catch
                {
                    MyConsole.SaveException(new Exception("BestOfWorld Error Loading."));
                }
                return null;
            }
        }
        public static System.SafeDictionary<Type, Rank> Ranks = new System.SafeDictionary<Type, Rank>();
        public static object SynRoot = new object();


        public static void Create()
        {
            for (int x = 0; x < (byte)Type.Count; x++)
            {
                Ranks.Add((Type)x, new Rank((Type)x));
                if (x == (int)Type.World)
                {
                    Ranks[(Type)x].MaxItems = 100;
                }
                else Ranks[(Type)x].MaxItems = 30;
            }
        }

        public class Rank
        {
            public Entry BestOfClass;
            public Dictionary<uint, Entry> Items = new Dictionary<uint, Entry>();

            public Type _Type;
            public Rank(Type typ)
            {
                _Type = typ;
            }
            public int MaxItems = 0;
            public void Remove(uint UID)
            {
                lock (SynRoot)
                {
                    if (Items.ContainsKey(UID))
                    {
                        Items.Remove(UID);
                        Items = Items.OrderByDescending(p => p.Value.TotalPoints).Take(MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                    }
                }
            }
            public void AddItem(Entry item)
            {
                if (item.TotalPoints < 20000) return;
                lock (SynRoot)
                {
                    try
                    {
                        if (!Items.ContainsKey(item.UID))
                            Items.Add(item.UID, item);
                        else if (Items.ContainsKey(item.UID))
                            Items[item.UID] = item;
                        Items = Items.OrderByDescending(p => p.Value.TotalPoints).Take(MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                    }
                    finally
                    {
                        try
                        {
                            BestOfClass = Items.Values.First();
                        }
                        catch
                        {
                            MyConsole.SaveException(new Exception("Prestige Rank Type " + _Type + "is empty."));
                        }
                    }
                }
            }
        }
        private static System.SafeDictionary<uint, Entry> TopAll = new System.SafeDictionary<uint, Entry>();
        public static void UpdateRank(Client.GameClient user)
        {
            if (TopAll.ContainsKey(user.Player.UID))
            {
                TopAll[user.Player.UID].TotalPoints = user.MyPrestigePoints;
            }
            else
            {
                var entry = new Entry()
                {
                    type = Type.World,
                    TotalPoints = user.MyPrestigePoints,
                    UID = user.Player.UID,
                    Name = user.Player.Name,
                    Level = (byte)user.Player.Level,
                    Class = user.Player.Class,
                    Mesh = user.Player.Mesh
                };
                entry.AddInfo(user);
                TopAll.Add(user.Player.UID, entry);
            }
        }
        public static void CheckReborn(Client.GameClient user)
        {
            lock (SynRoot)
            {
                if (user.Player.Reborn == 2)
                {
                    if (Ranks[GetIndex(user.Player.SecoundeClass)].Items.ContainsKey(user.Player.UID))
                    {
                        Ranks[GetIndex(user.Player.SecoundeClass)].Items.Remove(user.Player.UID);
                        Ranks[GetIndex(user.Player.SecoundeClass)].Items = Ranks[GetIndex(user.Player.SecoundeClass)].Items.OrderByDescending(p => p.Value.TotalPoints).Take(Ranks[GetIndex(user.Player.SecoundeClass)].MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                    }
                }
                else if (user.Player.Reborn == 1)
                {
                    if (Ranks[GetIndex(user.Player.FirstClass)].Items.ContainsKey(user.Player.UID))
                    {
                        Ranks[GetIndex(user.Player.FirstClass)].Items.Remove(user.Player.UID);
                        Ranks[GetIndex(user.Player.FirstClass)].Items = Ranks[GetIndex(user.Player.FirstClass)].Items.OrderByDescending(p => p.Value.TotalPoints).Take(Ranks[GetIndex(user.Player.FirstClass)].MaxItems).ToDictionary<KeyValuePair<uint, Entry>, uint, Entry>(pair => pair.Key, pair => pair.Value);
                    }
                }
            }
        }

        public static uint GetMyRank(Type typ, uint UID)
        {
            if (Ranks.Count == 0)
                return 0;
            if (Ranks[typ].Items.Values.Count(i => i.UID == UID) > 0)
            {
                var item = Ranks[typ].Items.Values.FirstOrDefault(i => i.UID == UID);
                return (uint)(Ranks[typ].Items.Values.ToList().IndexOf(item) + 1);
            }
            return 0;
        }

        public static Entry GetInfo(uint UID)
        {
            lock (SynRoot)
            {
                foreach (var rank in Ranks.Values)
                {
                    foreach (var item in rank.Items.Values)
                        if (item.UID == UID)
                            return item;
                }
            }
            return null;
        }



        public static void Load()
        {
            lock (SynRoot)
            {
                Create();
                using (DBActions.Read reader = new DBActions.Read("PrestigeRanking.txt", false))
                {
                    if (reader.Reader())
                    {
                        int count = reader.Count;
                        for (int x = 0; x < count; x++)
                        {
                            DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("/"), '/');
                            Entry item = new Entry();
                            item.UID = line.Read((uint)0);
                            item.Name = line.Read("");
                            item.type = (Type)line.Read((byte)0);

                            item.Class = line.Read((uint)0);
                            item.Level = line.Read((byte)0);
                            item.Mesh = line.Read((uint)0);
                            item.TotalPoints = line.Read((uint)0);
                            item.HairStyle = line.Read((uint)0);
                            item.Head = line.Read((uint)0);
                            item.Garment = line.Read((uint)0);
                            item.LeftWeapon = line.Read((uint)0);
                            item.LefttWeaponAccessory = line.Read((uint)0);
                            item.RightWeapon = line.Read((uint)0);
                            item.RightWeaponAccessory = line.Read((uint)0);
                            item.MountArmor = line.Read((uint)0);
                            item.Armor = line.Read((uint)0);
                            item.Wing = line.Read((uint)0);
                            item.WingPlus = line.Read((uint)0);
                            item.Title = line.Read((uint)0);
                            item.Flag = line.Read((uint)0);
                            item.GuildName = line.Read("");
                            item.FrameID = line.Read((uint)0);
                            int pointscount = line.Read((int)0);
                            item.Points = new uint[pointscount];
                            for (int i = 0; i < item.Points.Length; i++)
                            {
                                item.Points[i] = line.Read((uint)0);

                            }
                            if (!item.Name.Contains("[PM]") && !item.Name.Contains("[GM]"))
                            {
                                Ranks[item.type].AddItem(item);
                                Ranks[Type.World].AddItem(item);
                            }
                        }
                    }
                }
            }
        }
        public static void Save()
        {
            lock (SynRoot)
            {
                using (DBActions.Write writer = new DBActions.Write("PrestigeRanking.txt"))
                {
                    foreach (var rank in Ranks.GetValues())
                    {

                        if (rank._Type == Type.World)
                            continue;
                        foreach (var obj in rank.Items.Values)
                        {
                            if (obj.UID >= 3999900001)
                                continue;
                            Database.DBActions.WriteLine line = new DBActions.WriteLine('/');
                            line.Add(obj.UID).Add(obj.Name).Add((byte)obj.type).Add(obj.Class)
                                .Add(obj.Level).Add(obj.Mesh).Add(obj.TotalPoints)
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
                             .Add(obj.FrameID);
                            if (obj.Points == null)
                                line.Add(0);
                            else
                            {
                                line.Add(obj.Points.Length);
                                for (int x = 0; x < obj.Points.Length; x++)
                                    line.Add(obj.Points[x]);
                            }
                            writer.Add(line.Close());
                        }
                    }
                    writer.Execute(DBActions.Mode.Open);
                }
            }
        }
    }
}
