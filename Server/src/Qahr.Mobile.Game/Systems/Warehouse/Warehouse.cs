using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Warehouse
    {
        public const byte Max_Count = 60;

        public static bool IsWarehouse(Game.MsgNpc.NpcID ID)
        {
            return (ID == Game.MsgNpc.NpcID.WHTwin || ID == Game.MsgNpc.NpcID.wHPheonix
                              || ID == Game.MsgNpc.NpcID.WHMarket || ID == Game.MsgNpc.NpcID.WHBird
                              || ID == Game.MsgNpc.NpcID.WHDesert || ID == Game.MsgNpc.NpcID.WHApe
                              || ID == Game.MsgNpc.NpcID.WHPoker || ID == Game.MsgNpc.NpcID.WHPokerTwin || ID == Game.MsgNpc.NpcID.WHStone
                              || ID == (Game.MsgNpc.NpcID)ushort.MaxValue);
        }


        public byte WHMaxSpace()
        {

            return Max_Count;
        }

        public void RemoveInscribedItems()
        {
            foreach (var wh in ClientItems.Values)
            {
                foreach (var item in wh.Values)
                {
                    if (item.Inscribed == 1)
                        item.Inscribed = 0;
                }
            }
        }

        public bool HaveItemsInBanks()
        {

            foreach (var bank in ClientItems.Values)
            {
                if (bank.Count > 0)
                    return true;
            }
            return false;
        }
        public bool TryGetItem(uint UID, out MsgGameItem item)
        {
            foreach (var wh in ClientItems.Values)
            {
                foreach (var _item in wh.Values)
                {
                    if (_item.UID == UID)
                    {
                        item = _item;
                        return true;
                    }
                }
            }
            item = null;
            return false;
        }
        public bool TryGetItemID(uint ITEMID, out MsgGameItem item)
        {
            foreach (var wh in ClientItems.Values)
            {
                foreach (var _item in wh.Values)
                {
                    if (_item.ITEM_ID == ITEMID)
                    {
                        item = _item;
                        return true;
                    }
                }
            }
            item = null;
            return false;
        }
        public bool TryGetItem(uint WH_ID, uint UID, out MsgGameItem item)
        {
            ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> items;
            if (ClientItems.TryGetValue((uint)WH_ID, out items))
            {
                return items.TryGetValue(UID, out item);
            }
            item = null;
            return false;
        }
        public ConcurrentDictionary<uint, ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>> ClientItems;

        public List<uint> IsShow = new List<uint>();
        public Client.GameClient User;
        public Warehouse(Client.GameClient client)
        {
            ClientItems = new ConcurrentDictionary<uint, ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>>();
            User = client;
        }
        public void SendInboxItems(ServerSockets.Packet stream)
        {
            ConcurrentDictionary<uint, MsgGameItem> concurrentDictionary;
            if (!this.ClientItems.TryGetValue(1000U, out concurrentDictionary))
                return;
            foreach (MsgGameItem msgItemInfo in (IEnumerable<MsgGameItem>)concurrentDictionary.Values)
            {
                msgItemInfo.Mode = Role.Flags.ItemMode.Inbox;
                msgItemInfo.Send(this.User, stream);
            }
        }

        public void SendInboxItem(ServerSockets.Packet stream, uint UID, uint NpcID = 1000)
        {
            MsgGameItem msgItemInfo;
            if (!this.ClientItems.ContainsKey(NpcID) || !this.ClientItems[NpcID].TryGetValue(UID, out msgItemInfo))
                return;
            msgItemInfo.Mode = Role.Flags.ItemMode.Inbox;
            msgItemInfo.Send(this.User, stream);
        }
        public void SendReturnedItems(ServerSockets.Packet stream)
        {
            ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> wh_items;
            if (ClientItems.TryGetValue(ushort.MaxValue, out wh_items))
            {
                foreach (var item in wh_items.Values)
                {
                    item.Mode = Flags.ItemMode.AddItemReturned;
                    item.Send(User, stream);
                }
            }
        }
        public bool AddItem(Game.MsgServer.MsgGameItem DataItem, uint NpcID)
        {
            if (!ClientItems.ContainsKey(NpcID))
                ClientItems.TryAdd(NpcID, new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());


            if (ClientItems[NpcID].TryAdd(DataItem.UID, DataItem))
            {
                DataItem.WH_ID = NpcID;
                return true;
            }
            return false;
        }

        //public bool AddItem(Game.MsgServer.MsgGameItem DataItem, uint NpcID)
        //{
        //    using (var rec = new ServerSockets.RecycledPacket())
        //    {
        //        var stream = rec.GetStream();
        //        if (!ClientItems.ContainsKey(NpcID))
        //            ClientItems.TryAdd(NpcID, new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());
                
               
        //        Database.ItemType.DBItem DBItem;
        //        if (Pool.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
        //        {
        //            #region AddStackSize
        //            if (DBItem.StackSize > 0 && DataItem.Locked == 0)
        //            {
        //                byte bound = 0;
        //                if (DataItem.Bound == 1)
        //                    bound = 1;
        //                ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> Items;
        //                if (ClientItems.TryGetValue(NpcID, out Items))
        //                {//MT  
        //                    #region (New)MT
        //                    foreach (var Item in Items.Values)
        //                    {
        //                        if (Item.ITEM_ID == DataItem.ITEM_ID && Item.Bound == bound)
        //                        {
        //                            if (Item.StackSize + DataItem.StackSize <= DBItem.StackSize)
        //                            {
        //                                Game.MsgServer.MsgGameItem item;
        //                                if (ClientItems[NpcID].TryRemove(Item.UID, out item))
        //                                {
        //                                    item.Position = 0;
        //                                    item.WH_ID = 0;
        //                                    if (NpcID != User.Player.UID)
        //                                    {
        //                                        stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.WithdrawItem, Item.UID, 0, 0);
        //                                        User.Send(stream.FinalizeWarehouse());
        //                                    }
        //                                    else
        //                                        stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.InventorySashWithdrawItem, Item.UID, 0, 0);
        //                                    User.Send(stream.FinalizeWarehouse());
        //                                }
        //                                if (ClientItems[NpcID].TryAdd(DataItem.UID, DataItem))
        //                                {
        //                                    DataItem.StackSize += Item.StackSize;
        //                                    DataItem.WH_ID = NpcID;
        //                                    return true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    var HaveItems = Items.Values.Where(p => p.ITEM_ID == DataItem.ITEM_ID).OrderBy(p => p.StackSize).FirstOrDefault();
        //                    if (HaveItems != null && DataItem.StackSize != DBItem.StackSize)
        //                    {
        //                        int Amount = HaveItems.StackSize + DataItem.StackSize;
        //                        if (Amount > DBItem.StackSize)
        //                        {
        //                            User.Inventory.Update(DataItem, Role.Instance.AddMode.REMOVE, stream, true);
        //                            while (Amount >= DBItem.StackSize)
        //                            {
        //                                //MT  
        //                                #region RemoveFirst(MT)
        //                                Game.MsgServer.MsgGameItem item;
        //                                if (ClientItems[NpcID].TryRemove(HaveItems.UID, out item))
        //                                {
        //                                    item.Position = 0;
        //                                    item.WH_ID = 0;
        //                                    if (NpcID != User.Player.UID)
        //                                    {
        //                                        stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.WithdrawItem, HaveItems.UID, 0, 0);
        //                                        User.Send(stream.FinalizeWarehouse());
        //                                    }
        //                                    else
        //                                        stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.InventorySashWithdrawItem, HaveItems.UID, 0, 0);
        //                                    User.Send(stream.FinalizeWarehouse());
        //                                }
        //                                #endregion
        //                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
        //                                ItemDat.UID = Pool.ITEM_Counter.Next;
        //                                ItemDat.ITEM_ID = DataItem.ITEM_ID;
        //                                ItemDat.Durability = ItemDat.MaximDurability = DataItem.Durability;
        //                                ItemDat.Plus = DataItem.Plus;
        //                                ItemDat.RemainingTime = (DBItem.StackSize > 1) ? 0 : uint.MaxValue;
        //                                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
        //                                if (bound == 1)
        //                                    ItemDat.Bound = 1;
        //                                if (ClientItems[NpcID].TryAdd(ItemDat.UID, ItemDat))
        //                                {
        //                                    ItemDat.StackSize += DBItem.StackSize;
        //                                    ItemDat.WH_ID = NpcID;
        //                                }
        //                                #region DepositItem
        //                                if (NpcID != User.Player.UID)
        //                                {//MT  
        //                                    stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.DepositItem, 0, 0, 1);
        //                                }
        //                                else
        //                                    stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.InventorySashDepositItem, 0, 0, 1);

        //                                stream.AddItemWarehouse(ItemDat);

        //                                User.Send(stream.FinalizeWarehouse());
        //                                #endregion
        //                                Amount -= DBItem.StackSize;
        //                            }
        //                            if (Amount > 0 && Amount < DBItem.StackSize)
        //                            {
        //                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
        //                                ItemDat.UID = Pool.ITEM_Counter.Next;
        //                                ItemDat.ITEM_ID = DataItem.ITEM_ID;
        //                                ItemDat.Durability = ItemDat.MaximDurability = DataItem.Durability;
        //                                ItemDat.Plus = DataItem.Plus;
        //                                ItemDat.RemainingTime = (DBItem.StackSize > 1) ? 0 : uint.MaxValue;
        //                                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
        //                                if (bound == 1)
        //                                    ItemDat.Bound = 1;
        //                                if (ClientItems[NpcID].TryAdd(ItemDat.UID, ItemDat))
        //                                {
        //                                    ItemDat.StackSize += (ushort)Amount;
        //                                    ItemDat.WH_ID = NpcID;
        //                                }
        //                                #region DepositItem
        //                                if (NpcID != User.Player.UID)
        //                                {//MT  
        //                                    stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.DepositItem, 0, 0, 1);
        //                                }
        //                                else
        //                                    stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.InventorySashDepositItem, 0, 0, 1);

        //                                stream.AddItemWarehouse(ItemDat);

        //                                User.Send(stream.FinalizeWarehouse());
        //                                #endregion
        //                            }
        //                            return false;

        //                        }
        //                        else
        //                        {
        //                            if (ClientItems[NpcID].TryAdd(DataItem.UID, DataItem))
        //                            {
        //                                DataItem.WH_ID = NpcID;
        //                                return true;
        //                            }
        //                        }
        //                    }
        //                    #endregion
        //                }
        //            }
        //            #endregion
        //            if (ClientItems[NpcID].TryAdd(DataItem.UID, DataItem))
        //            {
        //                DataItem.WH_ID = NpcID;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        public unsafe bool RemoveItem(uint UID, uint NpcID, ServerSockets.Packet stream)
        {
            if (ClientItems.ContainsKey(NpcID))
            {
                if (User.Inventory.HaveSpace(1))
                {
                    Game.MsgServer.MsgGameItem item;
                    if (ClientItems[NpcID].TryRemove(UID, out item))
                    {
                        item.Position = 0;
                        item.WH_ID = 0;
                        User.Inventory.Update(item, AddMode.ADD, stream);
                        return true;
                    }
                }
                else
                {
#if Arabic
                     User.SendSysMesage("Your Inventory Is Full!");
#else
                    User.SendSysMesage("Your Inventory Is Full!");
#endif

                }
            }
            return false;
        }
        public unsafe void Show(uint NpcID, Game.MsgServer.MsgWarehouse.DepositActionID Action, ServerSockets.Packet stream)
        {
            if (ClientItems.ContainsKey(NpcID) && !IsShow.Contains(NpcID))
            {
                IsShow.Add(NpcID);

                Dictionary<int, List<Game.MsgServer.MsgGameItem>> Queues = new Dictionary<int, List<Game.MsgServer.MsgGameItem>>();
                Queues.Add(0, new List<Game.MsgServer.MsgGameItem>());

                int count = 0;
                var Array = ClientItems[NpcID].Values.ToArray();
                for (uint x = 0; x < Array.Length; x++)
                {
                    if (x % 1 == 0 && x > 0)
                    {
                        count++;
                        Queues.Add(count, new List<Game.MsgServer.MsgGameItem>());
                    }
                    Queues[count].Add(Array[x]);
                }

                foreach (var aray in Queues.Values)
                {
                    Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();

                    stream.WarehouseCreate(NpcID, Action, 0, NpcID == User.Player.UID ? User.Player.InventorySashCount : WHMaxSpace(), aray.Count);

                    foreach (var item in aray)
                    {
                        stream.AddItemWarehouse(item);

                        if (item.Refinary.InLife)
                        {
                            item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.RefinaryAdding;
                            if (item.Refinary.EffectDuration == 0)
                                item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.PermanentRefinery;
                            itemExtra.Refinerys.Add(item.Refinary);
                        }
                        if (item.Purification.InLife)
                        {
                            item.Purification.Typ = Game.MsgServer.MsgItemExtra.Typing.PurificationAdding;
                            itemExtra.Purifications.Add(item.Purification);
                        }
                    }
                    User.Send(stream.FinalizeWarehouse());

                    foreach (var item in aray)
                        item.SendItemLocked(User, stream);

                    if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                        User.Send(itemExtra.CreateArray(stream));
                }
            }
        }

        public IEnumerable<MsgGameItem> FuseRelics
        {
            get
            {
                foreach (var bank in ClientItems)
                {
                    if (bank.Key == (uint)Game.MsgNpc.NpcID.RelicFuse)
                    {
                        foreach (var items in bank.Value)
                            yield return items.Value;
                    }

                }

            }
        }
        public void Clear(uint NpcID)
        {
            if (ClientItems.ContainsKey(NpcID))
                ClientItems[NpcID].Clear();
        }
    }
}
