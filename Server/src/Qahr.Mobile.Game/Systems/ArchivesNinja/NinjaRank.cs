using Qahr.Mobile.Database.DBActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Database
{
    public class NinjaRank
    {
        public enum Type : uint
        {
            Overall,
            Fire,
            Water,
            Earth,
            Wind,
            Lightning,
            Count,
        }

        public class Entry
        {
            public Type Type;
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
            public uint Flag;//??
            public string GuildName = "";
            public uint WeaponID;//21
            public uint WeaponLevel;//22

            public void AddInfo(Client.GameClient user)
            {
                Name = user.Player.Name;
                HairStyle = user.Player.Hair;
                Garment = user.Player.GarmentId;
                if (user.Equipment != null)
                {
                    Head = user.Equipment.HeadID;
                    LeftWeapon = user.Equipment.LeftWeapon;
                    RightWeapon = user.Equipment.RightWeapon;
                }
                RightWeaponAccessory = user.Player.RightWeaponAccessoryId;
                LefttWeaponAccessory = user.Player.LeftWeaponAccessoryId;
                MountArmor = user.Player.MountArmorId;
                Wing = user.Player.WingId;
                if (user.Player.SpecialWingID != 0)
                    Wing = user.Player.SpecialWingID;
                WingPlus = user.Player.WingPlus;
                Title = user.Player.SpecialTitleID;
                Armor = user.Player.ArmorId;
                Class = user.Player.Class;
                Level = (byte)user.Player.Level;
                if (user.Player.MyGuild != null) GuildName = user.Player.MyGuild.GuildName;
                if (user.MyNinja.isOpen() != null)
                {
                    WeaponID = user.MyNinja.isOpen().ItemID;
                    WeaponLevel = user.MyNinja.isOpen().Level;
                }
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
                    Type = 2,
                    Armor = BestOf.Armor,
                    Wing = BestOf.Wing,
                    WingPlus = BestOf.WingPlus,
                    Profession = BestOf.Class,
                    WeaponWar = BestOf.WeaponID,
                    WeaponLevel = BestOf.WeaponLevel
                };
            }

            public Entry ShallowCopy()
            {
                return (Entry)MemberwiseClone();
            }
        }

        public class Rank
        {
            public Type _Type;

            public Rank(Type typ)
            {
                _Type = typ;
                SynRoot = new object();
                RankingList = new Dictionary<uint, Entry>();
            }

            public int MaxItems;
            public Dictionary<uint, Entry> RankingList;
            public object SynRoot;

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

            public void UpdateItem(Entry item)
            {
                lock (SynRoot)
                {
                    if (!RankingList.ContainsKey(item.UID))
                    {
                        RankingList.Add(item.UID, item);
                    }
                    else if (RankingList.ContainsKey(item.UID))
                    {
                        RankingList[item.UID] = item;
                    }

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
                rank.Remove(UID);
        }

        public static Entry BestOf(Type type)
        {
            return Ranks[type].RankingList.Values.FirstOrDefault();
        }

        public static void Load()
        {
            Create();
            using (Read reader = new Read("NinjaRank.txt", false))
            {
                if (reader.Reader())
                {
                    int count = reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        ReadLine line = new ReadLine(reader.ReadString("/"), '/');
                        Entry item = new Entry();
                        item.UID = line.Read((uint)0);
                        item.Name = line.Read("");
                        item.Type = (Type)line.Read((byte)0);
                        item.TotalPoints = line.Read((uint)0);
                        item.Class = line.Read((uint)0);
                        item.Level = line.Read((byte)0);
                        item.Mesh = line.Read((uint)0);
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
                        item.WeaponID = line.Read((uint)0);
                        item.WeaponLevel = line.Read((uint)0);
                        Ranks[item.Type].UpdateItem(item);
                    }
                }
            }

        }

        public static void Save()
        {
            using (Write writer = new Write("NinjaRank.txt"))
            {
                var mains = Ranks.GetValues().ToArray();
                foreach (var rank in mains)
                {
                    var ranks = rank.RankingList.Values.ToArray();
                    foreach (var obj in ranks)
                    {
                        if (obj.UID >= 3999900001)
                            continue;
                        WriteLine line = new WriteLine('/');
                        line.Add(obj.UID).Add(obj.Name).Add((byte)obj.Type).Add(obj.TotalPoints).Add(obj.Class)
                                .Add(obj.Level).Add(obj.Mesh)
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
                            .Add(obj.GuildName).Add(obj.WeaponID).Add(obj.WeaponLevel);
                        writer.Add(line.Close());
                    }
                }
                writer.Execute(Mode.Open);
            }

        }
    }
}