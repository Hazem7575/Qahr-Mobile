using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Qahr.Mobile.Database
{
    public class ExchangeShopGoods
    {
        public static Dictionary<uint, item> exchange_shop_goods;
        public class item
        {
            public uint id;
            public uint itemtypeid;
            public uint shopid; 
            public uint amount; 
            public uint monopoly;
            public uint material1;
            public uint material_amount1;
            public uint material2;
            public uint material_amount2;
            public uint material3;
            public uint material_amount3; 
            public uint material4;
            public uint material_amount4;
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\exchange_shop_goods.txt"))
                {
                    Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "\\exchange_shop_goods.txt");
                    return;
                }
                string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\exchange_shop_goods.txt");
                exchange_shop_goods = new Dictionary<uint, item>();
                item obj;
                for (int x = 0; x < text.Length; x++)
                {
                    string line = text[x];
                    string[] split = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    obj = new item();
                    try
                    {
                    obj.id = uint.Parse(split[0]);
                    obj.itemtypeid = uint.Parse(split[1]);
                    obj.shopid = uint.Parse(split[2]);
                    obj.amount = uint.Parse(split[3]);
                    obj.monopoly = uint.Parse(split[4]);
                    obj.material1 = uint.Parse(split[5]);
                    obj.material_amount1 = uint.Parse(split[6]);
                    obj.material2 = uint.Parse(split[7]);
                    obj.material_amount2 = uint.Parse(split[8]);
                    obj.material3 = uint.Parse(split[9]);
                    obj.material_amount3 = uint.Parse(split[10]);
                    obj.material4 = uint.Parse(split[11]);
                    obj.material_amount4 = uint.Parse(split[12]);
                   
                        exchange_shop_goods.Add(obj.id, obj);
                    }
                    catch
                    {
                        MyConsole.WriteLine(obj.id.ToString());
                        MyConsole.WriteLine(obj.shopid.ToString());
                    }
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
