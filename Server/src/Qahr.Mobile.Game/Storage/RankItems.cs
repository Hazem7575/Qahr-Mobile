using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using ProtoBuf;

namespace Qahr.Mobile.Database
{
    public class RankItems
    {
        public static Dictionary<uint, Rank> RankPoll = new Dictionary<uint, Rank>();

        public class Rank
        {
            public const int MaxItems = 50;
            public System.SafeDictionary<uint, MsgGameItem> Items = new System.SafeDictionary<uint, MsgGameItem>();
            public object SynRoot = new object();
            private MsgGameItem[] Top50 = new MsgGameItem[0];

            public uint GetItemRank(uint UID)
            {
                MsgGameItem item;
                if (Items.TryGetValue(UID, out item))
                    return item.PerfectionRank;
                return 100;
            }

            public MsgGameItem[] GetRank50Items()
            {
                lock (Top50)
                    return Top50;
            }


            public void AddItem(MsgGameItem item)
            {
                if (Items.Count < MaxItems)
                {
                    if (!Items.ContainsKey(item.UID))
                    {
                        Items.Add(item.UID, item);
                        lock (SynRoot)
                        {
                            Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                            for (int x = 0; x < Top50.Length; x++)
                            {
                                Top50[x].PerfectionRank = (ushort)(x + 1);

                            }
                        }
                    }
                    else if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].Position;
                        ushort rank = Items[item.UID].PerfectionRank;
                        Items[item.UID] = item;
                        item.PerfectionRank = rank;
                        if (points != item.ItemPoints)
                        {
                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].PerfectionRank = (ushort)(x + 1);
                            }
                        }
                    }
                }
                else
                {
                    if (Items.ContainsKey(item.UID))
                    {
                        ushort rank = Items[item.UID].PerfectionRank;
                        Items[item.UID] = item;
                        item.PerfectionRank = rank;
                    }
                    else
                    {
                        var last = Top50[Top50.Length - 1];
                        if (item.ItemPoints > last.ItemPoints)
                        {
                            Items.Remove(last.UID);
                            Items.Add(item.UID, item);

                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].PerfectionRank = (ushort)(x + 1);
                            }
                        }
                    }
                }
            }
        }

        public static void CreateRanks()
        {
            for (int x = 0; x < 11; x++)
                RankPoll.Add((uint)(x + 1), new Rank());
        }


        public static unsafe void SaveRanks()
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin", FileMode.Create))
            {
                ClientItems.DBItem DBItem = new ClientItems.DBItem();
                int RanksCount = 11;
                binary.Write(&RanksCount, sizeof(int));
                foreach (var rank in RankPoll.Values)
                {
                    int ItemsCounts = rank.Items.Count;
                    binary.Write(&ItemsCounts, sizeof(int));
                    foreach (var item in rank.Items.GetValues())
                    {
                        DBItem.GetDBItem(item);
                        if (!binary.Write(&DBItem, sizeof(ClientItems.DBItem)))
                            Console.WriteLine("test");
                        var info = DBItem.GetPerfectionInfo(item);
                        if (!binary.Write(&info, sizeof(ClientItems.Perfection)))
                            Console.WriteLine("test");
                    }
                }
                binary.Close();
            }
        }
        public unsafe static void LoadAllItems()
        {
            CreateRanks();
            if (File.Exists(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin"))
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin", FileMode.Open))
                {
                    ClientItems.DBItem Item;
                    int RanksCount;
                    binary.Read(&RanksCount, sizeof(int));
                    for (int x = 0; x < RanksCount; x++)
                    {
                        int ItemsCount;
                        binary.Read(&ItemsCount, sizeof(int));
                        for (int i = 0; i < ItemsCount; i++)
                        {
                            binary.Read(&Item, sizeof(ClientItems.DBItem));
                            Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                            ClientItems.Perfection info = new ClientItems.Perfection();
                            binary.Read(&info, sizeof(ClientItems.Perfection));
                            ClienItem.PerfectionLevel = info.Level;
                            ClienItem.OwnerUID = info.OwnerUID;
                            ClienItem.OwnerName = info.OwnerName;
                            ClienItem.PerfectionProgress = info.Progres;
                            ClienItem.Signature = info.SpecialText;

                            RankPoll[(uint)(x + 1)].AddItem(ClienItem);
                        }
                    }
                    binary.Close();
                }
            }

        }
    }
}
