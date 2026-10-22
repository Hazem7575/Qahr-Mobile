using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class ExchangeShopGoodsEx
    {
        public static Dictionary<uint, item> exchange_shop_goods_ex;
        public class item
        {
            public uint id;
            public uint itemtypeid;
            public uint shopid;
            public uint amount;
            public uint monopoly;
            public uint cost_type;
            public uint cost_value;
            public uint weight;
            public uint server_type_flag;
            public ulong start_date;
            public ulong end_date;
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\exchange_shop_goods_ex.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "\\exchange_shop_goods_ex.txt");
                    return;
                }
                string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\exchange_shop_goods_ex.txt");
                exchange_shop_goods_ex = new Dictionary<uint, item>();
                item obj;
                for (int x = 0; x < text.Length; x++)
                {
                    string line = text[x];
                    string[] split = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    obj = new item();
                    obj.id = uint.Parse(split[0]);
                    obj.itemtypeid = uint.Parse(split[1]);
                    obj.shopid = uint.Parse(split[2]);
                    obj.amount = uint.Parse(split[3]);
                    obj.monopoly = uint.Parse(split[4]);
                    obj.cost_type = uint.Parse(split[5]);
                    obj.cost_value = uint.Parse(split[6]);
                    obj.weight = uint.Parse(split[7]);
                    obj.server_type_flag = uint.Parse(split[8]);
                  
                        obj.start_date = ulong.Parse(split[9]);
                   
                    obj.end_date = ulong.Parse(split[10]);
                    exchange_shop_goods_ex.Add(obj.id, obj);
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}
