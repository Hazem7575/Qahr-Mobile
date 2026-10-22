using ProtoBuf;
using Qahr.Mobile.Client;
using System.Collections.Generic;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateShopClan(this ServerSockets.Packet stream, MsgShopClan.ProtoStructure pQuery)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pQuery);
            stream.Finalize(GamePackets.MsgNewShopGoods);
            return stream;
        }
        public static void GetExchangeShopGoodsInfo(this ServerSockets.Packet stream, out MsgShopClan.ProtoStructure pQuery)
        {
            pQuery = new MsgShopClan.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgShopClan.ProtoStructure>(pQuery);
        }

    }
  
    public class MsgShopClan
    {
        [ProtoContract]
        public class ProtoStructure
        {
            public enum Action : byte
            {
                Show = 0,
                Buy = 1,
                Refersh = 4,
                BuyShop = 6,
            }
            [ProtoMember(1, IsRequired = true)]
            public Action ACTion;
            [ProtoMember(2, IsRequired = true)]
            public uint ID;
            [ProtoMember(3, IsRequired = true)]
            public uint Level;
            [ProtoMember(4, IsRequired = true)]
            public uint Unk1;
            [ProtoMember(5, IsRequired = true)]
            public ulong FreeTime;
            [ProtoMember(6, IsRequired = true)]
            public uint PayNum;
            [ProtoMember(7, IsRequired = true)]
            public uint Ret;
            [ProtoMember(8, IsRequired = true)]
            public List<ITemProto> Items;
            [ProtoMember(9, IsRequired = true)]
            public uint Ret9;
        }
        [ProtoContract]
        public class ITemProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint IDShop;
            [ProtoMember(2, IsRequired = true)]
            public uint Count;
            [ProtoMember(3, IsRequired = true)]
            public ulong Time;
        }
        // Row must belong to the requested shop, count must fit a stack add, and the purchase limit must allow it.
        private static bool CanBuy(GameClient user, uint shopId, Database.new_shop_goods.item obj, uint cnt)
        {
            if (cnt == 0 || cnt > ushort.MaxValue || obj.shopid != shopId)
                return false;
            // QahrMobile: never take the money for an item type the server has not loaded (new itemtype rows need a restart)
            if (Pool.ItemsBase == null || !Pool.ItemsBase.ContainsKey(obj.ItemID))
                return false;
            if (obj.amount == 0)
                return true;
            Role.Instance.ExchangeShop.Item rec;
            ulong bought = user.MyExchangeShop.Items.TryGetValue(obj.id, out rec) ? rec.times : 0UL;
            return bought + cnt <= obj.amount;
        }
        // QahrMobile: goods paid with items (phone Swap Shop, currency 1001): takes CostCount x count of CostItem from the
        // bag, bound or not; nothing is taken when the bag has too few or no room for the bought item.
        private static bool PayWithItems(GameClient user, Database.new_shop_goods.item obj, uint cnt, ServerSockets.Packet stream)
        {
            ulong need = (ulong)obj.CostCount * cnt;
            if (need == 0 || need > uint.MaxValue || !user.Inventory.HaveSpace(1))
                return false;
            if (!user.Inventory.Contain(obj.CostItem, (uint)need) && !user.Inventory.Contain(obj.CostItem, (uint)need, 1))
                return false;
            return user.Inventory.Remove(obj.CostItem, (uint)need, stream);
        }
        [PacketAttribute(GamePackets.MsgNewShopGoods)]
        private static void Process(GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade || user.IsVendor)
                return;
            MsgShopClan.ProtoStructure pQuery;
            stream.GetExchangeShopGoodsInfo(out pQuery);
            switch (pQuery.ACTion)
            {
                case (ProtoStructure.Action)9:
                    {
                        // QahrMobile: phone Swap Shop "pay with items" (action 9: shop 6, Items[0] = goods + count, field 11 =
                        // the ticket stack the client picked). The server takes the ticket of the goods row itself (PayWithItems).
                        Database.new_shop_goods.item obj;
                        if (pQuery.Items == null || pQuery.Items.Count == 0)
                            break;
                        uint cnt = pQuery.Items[0].Count;
                        if (Database.new_shop_goods.ShopClan.TryGetValue(pQuery.Items[0].IDShop, out obj)
                            && obj.CostItem != 0 && CanBuy(user, pQuery.ID, obj, cnt) && PayWithItems(user, obj, cnt, stream))
                        {
                            user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt);
                            user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                            pQuery.Ret = 1;
                            user.Send(stream.CreateShopClan(pQuery));
                        }
                        break;
                    }
                case ProtoStructure.Action.Show:
                    {
                        if (pQuery.ID == 27950
                            || pQuery.ID == 23561
                            || pQuery.ID == 22372
                            || pQuery.ID == 23127)
                        {
                            user.Send(stream.CreateShopClan(pQuery));
                        }
                        else
                        {
                            user.MyExchangeShop.OpenClan(stream, 1);
                        }
                        break;
                    }
                case ProtoStructure.Action.Buy:
                    {
                        Database.new_shop_goods.item obj;
                        if (pQuery.Items == null || pQuery.Items.Count == 0)
                            break;
                        uint cnt = pQuery.Items[0].Count;
                        if (Database.new_shop_goods.ShopClan.TryGetValue(pQuery.Items[0].IDShop, out obj)
                            && CanBuy(user, pQuery.ID, obj, cnt))
                        {
                            ulong cost = (ulong)obj.CountBuy * cnt; // price x count, no overflow
                            if (obj.CostItem != 0)
                            {
                                if (PayWithItems(user, obj, cnt, stream))
                                {
                                    user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt);
                                    user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                                    pQuery.Ret = 1;
                                    user.Send(stream.CreateShopClan(pQuery));
                                }
                            }
                            else if (obj.shopid == 1)
                            {
                                if (user.Player.CyanJadeRing >= cost && user.Inventory.HaveSpace(1))
                                {
                                    user.Player.CyanJadeRing -= (uint)cost;
                                    user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt); // limit only after payment
                                    user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                                    pQuery.Ret = 1;
                                    user.Send(stream.CreateShopClan(pQuery));
                                }
                            }
                            else if (obj.unk1 == 1)
                            {
                                // QahrMobile: the phone Mall (shop 21 "Regular" and the other CP tabs) buys with this
                                // action and CPs; before, only Cyan Jade and bound CPs were handled here.
                                if (user.Player.ConquerPoints >= cost && user.Inventory.HaveSpace(1))
                                {
                                    user.Player.ConquerPoints -= (uint)cost;
                                    user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt); // limit only after payment
                                    user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                                    pQuery.Ret = 1;
                                    user.Send(stream.CreateShopClan(pQuery));
                                }
                            }
                            else if (obj.unk1 == 2)
                            {
                                if (user.Player.BoundConquerPoints >= 0 && (ulong)user.Player.BoundConquerPoints >= cost && user.Inventory.HaveSpace(1))
                                {
                                    user.Player.BoundConquerPoints -= (int)cost;
                                    user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt);
                                    user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, true); // bought with bound CPs -> bound
                                    pQuery.Ret = 1;
                                    user.Send(stream.CreateShopClan(pQuery));
                                }
                            }
                        }
                        break;
                    }
                case ProtoStructure.Action.BuyShop:
                    {
                        Database.new_shop_goods.item obj;
                        if (pQuery.Items == null)
                            break;
                        for (int x = 0; x < pQuery.Items.Count; x++)
                        {
                            uint cnt = pQuery.Items[x].Count;
                            if (Database.new_shop_goods.ShopClan.TryGetValue(pQuery.Items[x].IDShop, out obj)
                                && CanBuy(user, pQuery.ID, obj, cnt))
                            {
                                ulong cost = (ulong)obj.CountBuy * cnt; // price x count, no overflow
                                if (obj.CostItem != 0)
                                {
                                    if (PayWithItems(user, obj, cnt, stream))
                                    {
                                        user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt);
                                        user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                                        pQuery.Ret = 1;
                                        user.Send(stream.CreateShopClan(pQuery));
                                    }
                                }
                                else if (obj.unk1 == 1)
                                {
                                    if (user.Player.ConquerPoints >= cost && user.Inventory.HaveSpace(1))
                                    {
                                        user.Player.ConquerPoints -= (uint)cost;
                                        user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt); // limit only after payment
                                        user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, (bool)(obj.Bound != 0 ? true : false));
                                        pQuery.Ret = 1;

                                        user.Send(stream.CreateShopClan(pQuery));
                                    }
                                }
                                else if (obj.unk1 == 2)
                                {
                                    if (user.Player.BoundConquerPoints >= 0 && (ulong)user.Player.BoundConquerPoints >= cost && user.Inventory.HaveSpace(1))
                                    {
                                        user.Player.BoundConquerPoints -= (int)cost;
                                        user.MyExchangeShop.AddItemClan(pQuery.ID, obj.id, cnt);
                                        user.Inventory.AddItemWitchStack(obj.ItemID, 0, (ushort)cnt, stream, true); // bought with bound CPs -> bound
                                        pQuery.Ret = 1;
                                        user.Send(stream.CreateShopClan(pQuery));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ProtoStructure.Action.Refersh:
                    {
                        if (user.Player.CyanJadeRing >= 50)
                        {
                            user.MyExchangeShop.GetRandomClan(1, 8);
                            user.MyExchangeShop.OpenClan(stream, 1);
                            user.Player.CyanJadeRing -= 50;
                        }
                        break;
                    }
                default: MyConsole.WriteLine("Unknown Action Type [" + (uint)pQuery.ACTion + "] MsgShopClan."); break;
            }
        }

    }
}
