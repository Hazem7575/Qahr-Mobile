using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Database;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgExchange
    {
        public static unsafe ServerSockets.Packet CreateExchangeShopBuy(this ServerSockets.Packet stream, CMsgExchangeShopBuy obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgExchangeShopBuy);
            return stream;
        }
        
        public static unsafe void GetExchangeShopBuy(this ServerSockets.Packet stream, out CMsgExchangeShopBuy pQuery)
        {
            pQuery = new CMsgExchangeShopBuy();
            pQuery = stream.ProtoBufferDeserialize(pQuery);
        }
       
        
    }
    [ProtoContract]
    public class CMsgExchangeShopBuy
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Type;
        [ProtoMember(2, IsRequired = true)]
        public uint NpcID;
        [ProtoMember(3, IsRequired = true)]
        public uint ShopID;
        [ProtoMember(4, IsRequired = true)]
        public uint Count;
        [ProtoMember(5, IsRequired = true)]
        public uint Index;
        [ProtoMember(6, IsRequired = true)]
        public Item[] Items;
        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ID;
            [ProtoMember(2, IsRequired = true)]
            public uint Count;
        }
        [Packet(GamePackets.MsgExchangeShopBuy)]
        private unsafe static void Process1(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade || client.IsVendor)
                return;
            CMsgExchangeShopBuy Info = null;
            stream.GetExchangeShopBuy(out Info);
            switch (Info.Type)
            {
                case 0:
                    {
                        ExchangeShopGoods.item obj;
                        if (ExchangeShopGoods.exchange_shop_goods.TryGetValue(Info.Index, out obj))
                        {
                            // Count is client-supplied; bound it BEFORE anything scales by it so it can't
                            // be 0 (free buy) or huge (16-bit wrap on the price).
                            uint Count = Info.Count;
                            if (Count < 1 || Count > ushort.MaxValue)
                                break;
                            if (!client.Inventory.HaveSpace(1))
                                break;
                            // Daily/purchase limit check WITHOUT incrementing (increment only after payment).
                            if (!client.MyExchangeShop.CanAddItem(Info.ShopID, Info.Index, Count))
                                break;

                            foreach (var ITEM in Info.Items)
                            {
                                Game.MsgServer.MsgGameItem invItem;
                                if (client.Inventory.TryGetItem((uint)ITEM.ID, out invItem))
                                {
                                    if (invItem.ITEM_ID != obj.material1 && invItem.ITEM_ID != obj.material2 && invItem.ITEM_ID != obj.material3 && invItem.ITEM_ID != obj.material4)
                                    {

                                        return;
                                    }
                                }
                            }

                            // Compute every required material total server-side with ulong (no 16-bit cut),
                            // and require each total to fit a ushort for Contain/RemoveStackItem.
                            ulong need1 = (ulong)obj.material_amount1 * Count;
                            ulong need2 = (ulong)obj.material_amount2 * Count;
                            ulong need3 = (ulong)obj.material_amount3 * Count;
                            ulong need4 = (ulong)obj.material_amount4 * Count;

                            bool paid = false;

                            #region FourItem
                            if (obj.material1 != 0 && obj.material2 != 0 && obj.material3 != 0
                                && obj.material4 != 0)
                            {
                                if (need1 <= ushort.MaxValue && need2 <= ushort.MaxValue && need3 <= ushort.MaxValue && need4 <= ushort.MaxValue
                                && client.Inventory.Contain(obj.material1, (uint)need1)
                                && client.Inventory.Contain(obj.material2, (uint)need2)
                                && client.Inventory.Contain(obj.material3, (uint)need3)
                                && client.Inventory.Contain(obj.material4, (uint)need4))
                                {
                                    client.Inventory.RemoveStackItem(obj.material1, (ushort)need1, stream);
                                    client.Inventory.RemoveStackItem(obj.material2, (ushort)need2, stream);
                                    client.Inventory.RemoveStackItem(obj.material3, (ushort)need3, stream);
                                    client.Inventory.RemoveStackItem(obj.material4, (ushort)need4, stream);
                                    paid = true;
                                }
                            }
                            #endregion

                            #region ThreeItem
                            else if (obj.material1 != 0 && obj.material2 != 0 && obj.material3 != 0)
                            {
                                if (need1 <= ushort.MaxValue && need2 <= ushort.MaxValue && need3 <= ushort.MaxValue
                                && client.Inventory.Contain(obj.material1, (uint)need1)
                                && client.Inventory.Contain(obj.material2, (uint)need2)
                                && client.Inventory.Contain(obj.material3, (uint)need3))
                                {
                                    client.Inventory.RemoveStackItem(obj.material1, (ushort)need1, stream);
                                    client.Inventory.RemoveStackItem(obj.material2, (ushort)need2, stream);
                                    client.Inventory.RemoveStackItem(obj.material3, (ushort)need3, stream);
                                    paid = true;
                                }
                            }
                            #endregion

                            #region TwoItem
                            else if (obj.material1 != 0 && obj.material2 != 0)
                            {
                                if (need1 <= ushort.MaxValue && need2 <= ushort.MaxValue
                                && client.Inventory.Contain(obj.material1, (uint)need1)
                                && client.Inventory.Contain(obj.material2, (uint)need2))
                                {
                                    client.Inventory.RemoveStackItem(obj.material1, (ushort)need1, stream);
                                    client.Inventory.RemoveStackItem(obj.material2, (ushort)need2, stream);
                                    paid = true;
                                }
                            }
                            #endregion

                            #region OneItem
                            else if (obj.material1 != 0)
                            {
                                // Charge the server-side material and amount, NOT the client's Info.Items counts.
                                if (need1 <= ushort.MaxValue && client.Inventory.Contain(obj.material1, (uint)need1))
                                {
                                    client.Inventory.RemoveStackItem(obj.material1, (ushort)need1, stream);
                                    paid = true;
                                }
                            }
                            #endregion

                            if (paid)
                            {
                                client.Inventory.AddItemWitchStack(obj.itemtypeid, 0, (ushort)Count, stream, (bool)(obj.monopoly != 0 ? true : false));
                                // Record the purchase AFTER payment succeeded.
                                client.MyExchangeShop.AddItem(Info.ShopID, Info.Index, Count);
                                client.Send(stream.CreateExchangeShopBuy(Info));
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        ExchangeShopGoodsEx.item obj;
                        if (ExchangeShopGoodsEx.exchange_shop_goods_ex.TryGetValue(Info.Index, out obj))
                        {
                            uint Count = Info.Count;
                            if (Count < 1 || Count > ushort.MaxValue)
                                break;
                            if (obj.cost_type == 4 && client.Inventory.HaveSpace(1))
                            {
                                if (!client.MyExchangeShop.CanAddItemEx(Info.ShopID, Info.Index, Count))
                                    break;
                                // Donation cost must scale with the quantity bought.
                                ulong totalCost = (ulong)obj.cost_value * Count;
                                if (client.Player.MyDontion >= totalCost)
                                {
                                    client.Player.MyDontion -= (uint)totalCost;
                                    client.Inventory.AddItemWitchStack(obj.itemtypeid, 0, (ushort)Count, stream, (bool)(obj.monopoly != 0 ? true : false));
                                    client.MyExchangeShop.AddItemEx(Info.ShopID, Info.Index, Count);
                                    client.Send(stream.CreateExchangeShopBuy(Info));
                                }
                            }
                            else if (obj.cost_type == 2)// [BattlePass] Martial Token Point Store (shop 108, paid with Hero Points)
                            {
                                if (global::Qahr.Mobile.BattlePass.BattlePassManager.ShopBuy(client, Info.ShopID, Info.Index, Count, obj, stream))
                                    client.Send(stream.CreateExchangeShopBuy(Info));
                            }
                        }
                        break;
                    }
            }
        }
    }
}
