using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class ExchangeShop
    {
        public class Item
        {
            public uint shopid;
            public uint goods_rec_id;
            public uint times;
        }
        public Dictionary<uint, Item> Items = new Dictionary<uint, Item>();
        private Client.GameClient user;
        public ExchangeShop(Client.GameClient _user)
        {
            user = _user;
        }

        public uint GetTime()
        {
            DateTime now64 = DateTime.Now;
            int hours = now64.Hour;
            int minutes = now64.Minute;
            int secounds = now64.Second;
            int lefthours = 23 - hours;
            int leftminutes = 60 - minutes;
            int leftsecounds = 60 - secounds;
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan nextday = new TimeSpan(DateTime.Now.AddHours(lefthours).AddMinutes(leftminutes).AddSeconds(leftsecounds).Ticks);
            return (uint)(nextday.TotalSeconds - now.TotalSeconds);
        }
        public void Open(ServerSockets.Packet stream, uint NpcID, uint ShopID)
        {
            CMsgExchangeShopGoods obj = new CMsgExchangeShopGoods();
            obj.NpcID = NpcID;
            obj.ShopID = ShopID;
            obj.UID = user.Player.UID;
            obj.TimeInSeconds = GetTime();
            var Array = Items.Values.Where(p => p.shopid == ShopID).ToArray();
            obj.Items = new CMsgExchangeShopGoods.Item[Array.Length];
            int i = 0; foreach (var Item in Array)
            {
                obj.Items[i] = new CMsgExchangeShopGoods.Item();
                obj.Items[i].Index = Item.goods_rec_id;
                obj.Items[i].Count = Item.times;
                obj.Items[i].uk = 0;
                i++;
            }
            user.Send(stream.CreateExchangeShopGoods(obj));
        }
        public void OpenClan(ServerSockets.Packet stream, uint NpcID)
        {
            MsgShopClan.ProtoStructure Clan = new MsgShopClan.ProtoStructure();
            Clan.ID = NpcID;
            Clan.FreeTime = this.GetTime();
            var Array = Items.Values.Where(p => p.shopid == 1).ToArray();
            Clan.Items = new List<MsgShopClan.ITemProto>();
            int index = 0;
            foreach (var obj in Array)
            {
                Clan.Items.Add(new MsgShopClan.ITemProto() { IDShop = obj.goods_rec_id });
                ++index;
            }

            user.Send(stream.CreateShopClan(Clan));
        }
        public void OpenReb(ServerSockets.Packet stream, uint NpcID, uint ShopID)
        {
            CMsgExchangeShopGoods obj = new CMsgExchangeShopGoods();
            obj.NpcID = NpcID;
            obj.ShopID = ShopID;
            obj.UID = user.Player.UID;
            obj.TimeInSeconds = GetTime();
            var Array = Items.Values.Where(p => p.shopid == 137).ToArray();
            obj.Items = new CMsgExchangeShopGoods.Item[Array.Length];
            int i = 0; foreach (var Item in Array)
            {
                obj.Items[i] = new CMsgExchangeShopGoods.Item();
                obj.Items[i].Index = Item.goods_rec_id;
                obj.Items[i].Count = Item.times;
                obj.Items[i].uk = 0;
                i++;
            }
            user.Send(stream.CreateExchangeShopGoods(obj));
        }
        // Peek (no increment) with ulong math, mirroring CanAddItem.
        public bool CanAddItemEx(uint shopid, uint goods_rec_id, uint times)
        {
            ExchangeShopGoodsEx.item obj2;
            if (!ExchangeShopGoodsEx.exchange_shop_goods_ex.TryGetValue(goods_rec_id, out obj2))
                return false;
            if (obj2.amount == 0)
                return true; // unlimited
            Item obj;
            ulong current = Items.TryGetValue(goods_rec_id, out obj) ? obj.times : 0UL;
            return (current + times) <= obj2.amount;
        }
        public bool AddItemEx(uint shopid, uint goods_rec_id, uint times)
        {
            Item obj;
            ExchangeShopGoodsEx.item obj2;
            if (Items.TryGetValue(goods_rec_id, out obj))
            {
                if (ExchangeShopGoodsEx.exchange_shop_goods_ex.TryGetValue(goods_rec_id, out obj2))
                {
                    if (((ulong)obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times += times;
                        return true;
                    }
                }
            }
            else
            {
                obj = new Item();
                obj.goods_rec_id = goods_rec_id;
                obj.shopid = shopid;
                obj.times = 0;
                if (ExchangeShopGoodsEx.exchange_shop_goods_ex.TryGetValue(goods_rec_id, out obj2))
                {
                    if (((ulong)obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times = times;
                        Items.Add(obj.goods_rec_id, obj);
                        return true;
                    }
                }
            }
            return false;
        }
        public bool AddItemClan(uint shopid, uint goods_rec_id, uint times)
        {
            ExchangeShop.Item obj1;
            if (this.Items.TryGetValue(goods_rec_id, out obj1))
            {
                new_shop_goods.item obj2;
                if (new_shop_goods.ShopClan.TryGetValue(goods_rec_id, out obj2))
                {
                    if ((obj1.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj1.times += times;
                        return true;
                    }
                }
            }
            else
            {
                ExchangeShop.Item obj3 = new ExchangeShop.Item();
                obj3.goods_rec_id = goods_rec_id;
                obj3.shopid = shopid;
                obj3.times = 0;
                new_shop_goods.item obj4;
                if (new_shop_goods.ShopClan.TryGetValue(goods_rec_id, out obj4))
                {
                    if ((obj3.times + times) <= obj4.amount || obj4.amount == 0)
                    {
                        obj3.times = times;
                        this.Items.Add(obj3.goods_rec_id, obj3);
                        return true;
                    }
                }
            }
            return false;
        }
        public bool AddItemReb(uint shopid, uint goods_rec_id, uint times)
        {
            Item obj;
            ExchangeShopGoods.item obj2;
            if (Items.TryGetValue(goods_rec_id, out obj))
            {
                if (ExchangeShopGoods.exchange_shop_goods.TryGetValue(goods_rec_id, out obj2))
                {
                    if ((obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times += times;
                        return true;
                    }
                }
            }
            else
            {
                obj = new Item();
                obj.goods_rec_id = goods_rec_id;
                obj.shopid = shopid;
                obj.times = 0;
                if (ExchangeShopGoods.exchange_shop_goods.TryGetValue(goods_rec_id, out obj2))
                {
                    if ((obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times = times;
                        Items.Add(obj.goods_rec_id, obj);
                        return true;
                    }
                }
            }
            return false;
        }
        
        public void GetRandomEx(uint shopid, uint Count)
        {
            List<uint> CanUse = new List<uint>();
            foreach (var get in ExchangeShopGoodsEx.exchange_shop_goods_ex.Values.Where(p => p.shopid == shopid))
            {
                if (Items.ContainsKey(get.id))
                {
                    Items.Remove(get.id);
                }
                CanUse.Add(get.id);
            }
            // [hang-fix] guard: asking for more items than exist used to loop forever (every pick already added)
            for (int x = 0, _g = CanUse.Count * 20 + 100; x < Count && _g-- > 0; )
            {
                uint id = CanUse[Pool.GetRandom.Next(0, CanUse.Count)];
                if (!Items.ContainsKey(id))
                {
                    if (AddItemEx(shopid, id, 0))
                        x++;
                }
            }
        }
        public void GetRandomReb(uint shopid, uint Count)
        {
            List<uint> CanUse = new List<uint>();
            foreach (var get in ExchangeShopGoods.exchange_shop_goods.Values.Where(p => p.shopid == shopid))
            {
                if (Items.ContainsKey(get.id))
                {
                    Items.Remove(get.id);
                }
                CanUse.Add(get.id);
            }
            // [hang-fix] guard: asking for more items than exist used to loop forever (every pick already added)
            for (int x = 0, _g = CanUse.Count * 20 + 100; x < Count && _g-- > 0; )
            {
                uint id = CanUse[Pool.GetRandom.Next(0, CanUse.Count)];
                if (!Items.ContainsKey(id))
                {
                    if (AddItemReb(shopid, id, 0))
                        x++;
                }
            }
        }
        public void GetRandomClan(uint shopid, uint Count)
        {
            List<uint> CanUse = new List<uint>();
            foreach (var get in new_shop_goods.ShopClan.Values.Where(p => p.shopid == shopid))
            {
                if (Items.ContainsKey(get.id))
                {
                    Items.Remove(get.id);
                }
                CanUse.Add(get.id);
            }
            // [hang-fix] guard: asking for more items than exist used to loop forever (every pick already added)
            for (int x = 0, _g = CanUse.Count * 20 + 100; x < Count && _g-- > 0; )
            {
                uint id = CanUse[Pool.GetRandom.Next(0, CanUse.Count)];
                if (!Items.ContainsKey(id))
                {
                    if (AddItemEx(shopid, id, 0))
                        x++;
                }
            }
        }
        public void Reset()
        {
            Items.Clear();
            GetRandomEx(168, 8);
        }
        public void Loading()
        {
            if (Items.Values.Count(p => p.shopid == 168) == 0)
                GetRandomEx(168, 8);
            if (Items.Values.Count(p => p.shopid == 1) == 0)
                GetRandomClan(1, 8);
            if (Items.Values.Count(p => p.shopid == 137) == 0)
                GetRandomReb(137, 9);
        }
        // Peek whether 'times' more purchases are allowed, WITHOUT incrementing the counter.
        // ulong math so a huge client 'times' can't wrap the uint sum past the limit.
        public bool CanAddItem(uint shopid, uint goods_rec_id, uint times)
        {
            ExchangeShopGoods.item obj2;
            if (!ExchangeShopGoods.exchange_shop_goods.TryGetValue(goods_rec_id, out obj2))
                return false;
            if (obj2.amount == 0)
                return true; // unlimited
            Item obj;
            ulong current = Items.TryGetValue(goods_rec_id, out obj) ? obj.times : 0UL;
            return (current + times) <= obj2.amount;
        }
        public bool AddItem(uint shopid, uint goods_rec_id, uint times)
        {
            Item obj;
            ExchangeShopGoods.item obj2;
            if (Items.TryGetValue(goods_rec_id, out obj))
            {
                if (ExchangeShopGoods.exchange_shop_goods.TryGetValue(goods_rec_id, out obj2))
                {
                    if (((ulong)obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times += times;
                        return true;
                    }
                }
            }
            else
            {
                obj = new Item();
                obj.goods_rec_id = goods_rec_id;
                obj.shopid = shopid;
                obj.times = 0;
                if (ExchangeShopGoods.exchange_shop_goods.TryGetValue(goods_rec_id, out obj2))
                {
                    if (((ulong)obj.times + times) <= obj2.amount || obj2.amount == 0)
                    {
                        obj.times = times;
                        Items.Add(obj.goods_rec_id, obj);
                        return true;
                    }
                }
            }
            return false;
        }
        public override string ToString()
        {
            WriteLine Writer = new WriteLine('/');
            Writer.Add(Items.Values.Count);
            foreach (var s in Items.Values)
            {
                Writer.Add(s.shopid);
                Writer.Add(s.goods_rec_id);
                Writer.Add(s.times);
            }
            return Writer.Close();
        }
        public void Load(string Line)
        {
            try
            {
                ReadLine dbline = new ReadLine(Line, '/');
                int Count = dbline.Read((int)0);
                for (int i = 0; i < Count; i++)
                {
                    Item obj = new Item();
                    obj.shopid = dbline.Read((uint)0);
                    obj.goods_rec_id = dbline.Read((uint)0);
                    obj.times = dbline.Read((uint)0);
                    Items.Add(obj.goods_rec_id, obj);
                }
            }
            catch { }
        }
    }
}
