using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.Shops
{
    public static class RacePointShop
    {
        public const uint UID = 6001;

        public static RacePointShopBase Shop;
        public class RacePointItem
        {
            public uint ID, Price;
        }
        public class RacePointShopBase
        {
            public int Count { get { return Items.Count; } }
            public Dictionary<uint, RacePointItem> Items;
        }
        public static void Load()
        {
            string path = Program.ServerConfig.DbLocation + "\\shops\\RacePointShop.ini";
            string[] text = File.Exists(path) ? File.ReadAllLines(path) : new string[0]; // missing file -> empty shop
            var Shop = new RacePointShopBase(); // [HotReload] published at the end
            Shop.Items = new Dictionary<uint, RacePointItem>();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].ToLower();

                if (line.StartsWith("[normal]"))
                {
                    continue;
                }
                else if (line.StartsWith("[recommend]"))
                {
                    break;
                }
                else
                {
                    if (line.Length > 3)
                    {
                        string[] Parts = line.Split(';')[0].Split(' ');
                        RacePointItem item = new RacePointItem();

                        item.ID = uint.Parse(Parts[0]);
                        item.Price = uint.Parse(Parts[1]);

                        Shop.Items.Add(item.ID, item);
                    }
                }
            }
            RacePointShop.Shop = Shop;
        }
    }
}
