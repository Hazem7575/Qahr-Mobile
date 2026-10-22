using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.Shops
{
    public static class ChampionShop
    {
        public const uint UID = 6003;

        public static ChampionShopBase Shop;
        public class ChampionItem
        {
            public uint ID, Price;
            public bool Bound;
        }
        public class ChampionShopBase
        {
            public int Count { get { return Items.Count; } }
            public Dictionary<uint, ChampionItem> Items;
        }
        public static void Load()
        {
            string path = Program.ServerConfig.DbLocation + "\\shops\\GoldenLeagueShop.ini";
            string[] text = File.Exists(path) ? File.ReadAllLines(path) : new string[0]; // missing file -> empty shop
            var Shop = new ChampionShopBase(); // [HotReload] published at the end
            Shop.Items = new Dictionary<uint, ChampionItem>();
            for (int x = 0; x < text.Length; x++)
            {
                String line = text[x];
                if (String.IsNullOrWhiteSpace(line) || (line.Contains("[") && line.Contains("]")) || line.StartsWith("//")) continue;
                String[] split = line.Split(' ');
                try
                {
                    Shop.Items.Add(uint.Parse(split[0]),

                        new ChampionItem()
                        {
                            ID = uint.Parse(split[0]),
                            Price = uint.Parse(split[1]),
                            Bound = split[2].Contains("Bound") || split[2].Contains("(B)")
                        });
                }
                catch { continue; }
            }
            ChampionShop.Shop = Shop;
        }
    }
}
