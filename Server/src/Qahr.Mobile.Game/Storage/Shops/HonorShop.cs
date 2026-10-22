using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.Shops
{
    public static class HonorShop
    {
        public const uint UID = 6000;

        public static HonorShopBase Shop;
        public class HonorShopBase
        {
            public int Count { get { return Items.Count; } }
            public Dictionary<uint, uint> Items;
        }
        public static void Load()
        {
            string path = Program.ServerConfig.DbLocation + "\\shops\\HonorShop.ini";
            string[] text = File.Exists(path) ? File.ReadAllLines(path) : new string[0]; // missing file -> empty shop
            var Shop = new HonorShopBase(); // [HotReload] published at the end
            Shop.Items = new Dictionary<uint, uint>(); // never null, even for an empty file
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x];
                if (line.StartsWith("Item"))
                {
                    line = line.Remove(0, 11);
                    Shop.Items = new Dictionary<uint, uint>(int.Parse(line));
                }
                else if (line.StartsWith("Recommend"))
                {
                    break;
                }
                else
                {
                    if (line.Length > 3)
                    {
                        string[] Parts = line.Split(',');
                        uint itemID = uint.Parse(Parts[0]);
                        uint cost = uint.Parse(Parts[1]);
                        Shop.Items.Add(itemID, cost);
                    }
                }
            }
            HonorShop.Shop = Shop;
        }
    }
}
