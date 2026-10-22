using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.Shops
{
    public static class EShopV2File
    {
        public static void Load()
        {
            Merge(EShopFile.Shops);
        }
        /// <summary>[HotReload] Adds emoneyShopV2.txt into the given emoneyshop table (Load uses the live one).</summary>
        public static void Merge(Dictionary<uint, ShopFile.Shop> Shops)
        {

            string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\shops\\emoneyShopV2.txt");
            var shop = new ShopFile.Shop();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].Trim().Replace("_", " ");
                if (line == "")
                    continue;
                bool bound = false;
                if (line.Contains(" present") || line.Contains(" present|new"))
                    bound = true;
                string[] split = line.Split(new string[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "ID")
                {
                    uint id = uint.Parse(split[1]);
                    if (Shops.ContainsKey(id))
                        shop = Shops[uint.Parse(split[1])];
                    else
                    {
                        shop = new ShopFile.Shop();
                        shop.Items = new List<uint>();
                        shop.UID = id;
                        Shops.Add(id, shop);
                    }
                }
                else if (split[0] == "MoneyType")
                {
                    shop.MoneyType = (ShopFile.MoneyType)byte.Parse(split[1]);
                }
                else if (split[0].Contains("Item") || split[0].Contains("item"))
                {
                    if (split[0].StartsWith("Item") || split[0].StartsWith("item"))
                    {
                        uint ID = uint.Parse(split[1].Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                    else
                    {
                        uint ID = uint.Parse(split[0].Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                }
                else if (split[0].Length != 0 && split[0] != "[recommend]" && split[0] != "Amount")
                {
                    if (split[0] == "//")
                        continue;
                    uint ID = uint.Parse(split[0].Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (bound)
                    {
                        if (!shop.BoundItems.Contains(ID))
                            shop.BoundItems.Add(ID);
                    }
                    else
                    {
                        if (!shop.Items.Contains(ID))
                            shop.Items.Add(ID);
                    }

                }
            }

        }
    }
}
