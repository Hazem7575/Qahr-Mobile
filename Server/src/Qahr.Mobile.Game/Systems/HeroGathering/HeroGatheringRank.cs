using Qahr.Mobile.Database.DBActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Qahr.Mobile
{
    public class HeroGatheringRank
    {
        [Flags]
        public enum Type : uint
        {
            Gathering = 0,
            Trojan = 1,
            Warrior = 2,
            Archer = 4,
            Ninja = 5,
            Monk = 6,
            Pirate = 7,
            DragonWarrior = 8,
            Water = 13,
            Fire = 14,
            Windwalker = 16,
            Thunderstriker = 9,
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
            public uint AvatarFrame;
            public Entry ShallowCopy()
            {
                return (Entry)this.MemberwiseClone();
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
            Ranks.Add(Type.Gathering, new Rank(Type.Gathering) { MaxItems = 100, });
            Ranks.Add(Type.Trojan, new Rank(Type.Trojan) { MaxItems = 100, });
            Ranks.Add(Type.Warrior, new Rank(Type.Warrior) { MaxItems = 100, });
            Ranks.Add(Type.Archer, new Rank(Type.Archer) { MaxItems = 100, });
            Ranks.Add(Type.Ninja, new Rank(Type.Ninja) { MaxItems = 100, });
            Ranks.Add(Type.Monk, new Rank(Type.Monk) { MaxItems = 100, });
            Ranks.Add(Type.Pirate, new Rank(Type.Pirate) { MaxItems = 100, });
            Ranks.Add(Type.DragonWarrior, new Rank(Type.DragonWarrior) { MaxItems = 100, });
            Ranks.Add(Type.Water, new Rank(Type.Water) { MaxItems = 100, });
            Ranks.Add(Type.Fire, new Rank(Type.Fire) { MaxItems = 100, });
            Ranks.Add(Type.Windwalker, new Rank(Type.Windwalker) { MaxItems = 100, });
            Ranks.Add(Type.Thunderstriker, new Rank(Type.Thunderstriker) { MaxItems = 100, });
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
            Create();
            using (Read reader = new Read("HeroGatheringRank.txt"))
            {
                if (reader.Reader())
                {
                    int count = reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        ReadLine line = new ReadLine(reader.ReadString(""), '/');
                        Entry item = new Entry();
                        item.UID = line.Read((uint)0);
                        item.Name = line.Read("");
                        item.Type = (Type)line.Read((byte)0);
                        item.TotalPoints = line.Read((uint)0);
                        item.Class = line.Read((uint)0);
                        item.Level = line.Read((byte)0);
                        item.Mesh = line.Read((uint)0);
                        item.AvatarFrame = line.Read((uint)0);
                        Ranks[item.Type].UpdateItem(item);

                    }
                }
            }
        }
        public static void Save()
        {
            using (Write writer = new Write("HeroGatheringRank.txt"))
            {
                var mains = Ranks.GetValues().ToArray();
                foreach (var rank in mains)
                {
                    var ranks = rank.RankingList.Values.ToArray();
                    foreach (var obj in ranks)
                    {
                        WriteLine line = new WriteLine('/');
                        line.Add(obj.UID)
                            .Add(obj.Name)
                            .Add((byte)obj.Type)
                            .Add(obj.TotalPoints)
                            .Add(obj.Class)
                                .Add(obj.Level)
                                .Add(obj.Mesh).Add(obj.AvatarFrame);
                        writer.Add(line.Close());
                    }
                }
                writer.Execute(Mode.Open);
            }
        }
    }
}
