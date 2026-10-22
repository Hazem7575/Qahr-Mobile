using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{

    public class CoatStorage
    {
        public class StorageItem
        {
            public uint ID;
            public ushort Points;
            public byte Stars;
            public uint Type;
        }
        public class StorageAttribute
        {
            public uint ID;
            public uint Type;
            public ushort CountReq;
            public Dictionary<byte, uint> Attributes;
        }
        public static List<StorageItem> AllItem = new List<StorageItem>();
        public static Dictionary<uint, StorageAttribute> StorageAttributes = new Dictionary<uint, StorageAttribute>();
        public static Dictionary<uint, StorageItem> Garments = new Dictionary<uint, StorageItem>();
        public static Dictionary<uint, StorageItem> Mounts = new Dictionary<uint, StorageItem>();
        public static Dictionary<uint, StorageItem> StorageItems = new Dictionary<uint, StorageItem>();
        public static Dictionary<int, StorageItem> GarmentsBig = new Dictionary<int, StorageItem>();
        public static Dictionary<int, StorageItem> MountsBig = new Dictionary<int, StorageItem>();
        public static Dictionary<uint, StorageItem> GarmentsBig5Star = new Dictionary<uint, StorageItem>();
        public static Dictionary<uint, StorageItem> MountsBig5Star = new Dictionary<uint, StorageItem>();
        public static List<StorageItem> Frac3Estrellas = new List<StorageItem>();
        public static void Load()
        {
            string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "coat_storage_type.txt");
            foreach (var bas_line in baseText)
            {
                var line = bas_line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                var item = new StorageItem();
                item.ID = Convert.ToUInt32(line[0]);
                item.Points = Convert.ToUInt16(line[1]);
                item.Stars = Convert.ToByte(line[3]);
                item.Type = Convert.ToUInt32(line[2]) / 1000000000;
                if (item.Type == 1)
                {
                    if (!Garments.ContainsKey(item.ID))
                        Garments.Add(item.ID, item);
                    ItemType.DBItem db;
                    if (Pool.ItemsBase.TryGetValue(item.ID, out db))
                    {
                        if (!ItemType.Garments.ContainsKey(item.ID))
                            ItemType.Garments.Add(item.ID, db);
                    }
                    if (item.Stars >= 4)
                    {
                        if (!GarmentsBig.ContainsKey((int)item.ID))
                            GarmentsBig.Add((int)item.ID, item);
                    }
                    if (item.Stars >= 5)
                    {
                        if (!GarmentsBig5Star.ContainsKey((uint)item.ID))
                            GarmentsBig5Star.Add((uint)item.ID, item);
                    }
                    if (item.Stars <= 3)
                        Frac3Estrellas.Add(item);
                }
                else
                {
                    if (!Mounts.ContainsKey(item.ID))
                        Mounts.Add(item.ID, item);
                    ItemType.DBItem db;
                    if (Pool.ItemsBase.TryGetValue(item.ID, out db))
                    {
                        if (!ItemType.SteedMounts.ContainsKey(item.ID))
                            ItemType.SteedMounts.Add(item.ID, db);
                    }
                    if (item.Stars >= 4)
                    {
                        if (!MountsBig.ContainsKey((int)item.ID))
                            MountsBig.Add((int)item.ID, item);
                    }
                    if (item.Stars >= 5)
                    {
                        if (!MountsBig5Star.ContainsKey((uint)item.ID))
                            MountsBig5Star.Add((uint)item.ID, item);
                    }
                }
                if (!StorageItems.ContainsKey(item.ID))
                    StorageItems.Add(item.ID, item);
                AllItem.Add(item);
            }
        }
        public static uint AmountStarGarments5tar(Client.GameClient client, byte Star)
        {
            uint Count = 0;
            foreach (var garment in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Garment].Values)
            {
                StorageItem item;
                if (GarmentsBig5Star.TryGetValue(garment.ITEM_ID, out item))
                {
                    if (item.Stars >= Star)
                        Count++;
                }
            }
            return Count;
        }
        public static uint AmountStarMount5tar(Client.GameClient client, byte Star)
        {
            uint Count = 0;
            foreach (var Mount in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values)
            {
                StorageItem item;
                if (MountsBig5Star.TryGetValue(Mount.ITEM_ID, out item))
                {
                    if (item.Stars >= Star)
                        Count++;
                }
            }
            return Count;
        }
        public static uint AmountStarGarments(Client.GameClient client, byte Star)
        {
            uint Count = 0;
            foreach (var garment in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Garment].Values)
            {
                StorageItem item;
                if (Garments.TryGetValue(garment.ITEM_ID, out item))
                {
                    if (item.Stars >= Star)
                        Count++;
                }
            }
            return Count;
        }
        public static uint AmountMonkeyMounts(Client.GameClient client, byte Star)
        {
            uint Count = 0;
            foreach (var garment in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values)
            {
                StorageItem item;
                if (Garments.TryGetValue(garment.ITEM_ID, out item))
                {
                    if (item.ID >= 200553 && item.ID <= 200560)
                    {
                        if (item.Stars >= Star)
                            Count++;
                    }
                }
            }
            return Count;
        }
        public static uint AmountStarMount(Client.GameClient client, byte Star)
        {
            uint Count = 0;
            foreach (var mount in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values)
            {
                StorageItem item;
                if (Mounts.TryGetValue(mount.ITEM_ID, out item))
                {
                    if (item.Stars >= Star)
                        Count++;
                }
            }
            return Count;
        }
    }
}
