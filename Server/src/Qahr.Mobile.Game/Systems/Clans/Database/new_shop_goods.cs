using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{

    public class new_shop_goods
    {
        public class item
        {
            public uint id;
            public uint unk1;
            public uint shopid;
            public uint ItemID;
            public uint CountBuy;
            public byte Bound;
            public uint amount;
            // QahrMobile: goods paid with items instead of money (phone Swap Shop: tickets 3342874..3342878);
            // columns 20/21 of new_shop_goods.txt, 0 = paid with the money of unk1
            public uint CostItem;
            public uint CostCount;

        }
        public static Dictionary<uint, new_shop_goods.item> ShopClan;

        public static void Load()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "\\new_shop_goods.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "\\new_shop_goods.txt");
                }
                else
                {
                    string[] strArray1 = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\new_shop_goods.txt");
                    // QahrMobile: build the whole table, then publish it, so a reload (reloadshops) never leaves a half
                    // table; a bad or repeated row is skipped instead of aborting the load.
                    var table = new Dictionary<uint, new_shop_goods.item>();
                    for (int index = 0; index < strArray1.Length; ++index)
                    {
                        string[] strArray2 = strArray1[index].Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        new_shop_goods.item obj = new new_shop_goods.item();
                        if (strArray2.Length < 8
                            || !uint.TryParse(strArray2[0], out obj.id)
                            || !uint.TryParse(strArray2[1], out obj.shopid)
                            || !uint.TryParse(strArray2[2], out obj.unk1)
                            || !byte.TryParse(strArray2[3], out obj.Bound)
                            || !uint.TryParse(strArray2[4], out obj.ItemID)
                            || !uint.TryParse(strArray2[5], out obj.CountBuy)
                            || !uint.TryParse(strArray2[7], out obj.amount))
                            continue;
                        if (strArray2.Length > 21)
                        {
                            uint.TryParse(strArray2[20], out obj.CostItem);
                            uint.TryParse(strArray2[21], out obj.CostCount);
                        }
                        table[obj.id] = obj;
                    }
                    new_shop_goods.ShopClan = table;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine((object)ex);
            }
        }


    }
}