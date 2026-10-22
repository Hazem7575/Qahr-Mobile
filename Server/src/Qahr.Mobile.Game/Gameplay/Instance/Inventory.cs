using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using System.Diagnostics;
using System.IO;
using Qahr.Mobile.DBFunctionality;
using System.Drawing.Drawing2D;
using Qahr.Mobile.Game;

namespace Qahr.Mobile.Role.Instance
{

    public class Inventory
    {
        private const byte File_Size = 40;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();




        private Client.GameClient Owner;
        public Inventory(Client.GameClient _own)
        {
            Owner = _own;
        }
        public int GetCountItem(uint ItemID)
        {
            int count = 0;
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ItemID)
                {
                    count += DataItem.StackSize > 1 ? DataItem.StackSize : 1;
                }
            }
            return count;
        }
        public bool VerifiedUpdateItem(List<uint> ItemsUIDS, uint ID, byte count, out Queue<Game.MsgServer.MsgGameItem> Items)
        {
            Queue<Game.MsgServer.MsgGameItem> ExistItems = new Queue<Game.MsgServer.MsgGameItem>();
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ID)
                {
                    if (ItemsUIDS.Contains(DataItem.UID))
                    {
                        count--;
                        ItemsUIDS.Remove(DataItem.UID);
                        ExistItems.Enqueue(DataItem);
                    }
                }
            }
            Items = ExistItems;
            return ItemsUIDS.Count == 0 && count == 0;
        }

        public void AddDBItem(Game.MsgServer.MsgGameItem item)
        {
            ClientItems.TryAdd(item.UID, item);
        }

        public bool HaveSpace(byte count)
        {
            return (ClientItems.Count + count) <= File_Size;
        }
        public bool HaveItemsInSash()
        {
            foreach (var item in ClientItems.Values)
            {
                if (Database.ItemType.IsSash(item.ITEM_ID))
                {
                    if (item.Deposite.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            return ClientItems.TryGetValue(UID, out item);
        }

        public bool SearchItemByID(uint ID, out Game.MsgServer.MsgGameItem item)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    item = msg_item;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool SearchItemByID(uint ID, byte count, out List<Game.MsgServer.MsgGameItem> Items)
        {
            byte increase = 0;
            Items = new List<Game.MsgServer.MsgGameItem>();
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    Items.Add(msg_item);
                    increase++;
                    if (increase == count)
                    {
                        return true;
                    }
                }
            }
            Items = null;
            return false;
        }

        public bool Contain(uint ID, uint Amount, byte bound = 0)
        {
            if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == Database.ItemType.Meteor
                        || item.ITEM_ID == Database.ItemType.MeteorTear)
                    {
                        if ((bound > 0 && item.Bound > 0) || bound == 0)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            else if (ID == Database.ItemType.MoonBox || ID == 723087)//execept for bound
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID && ((bound > 0 && item.Bound > 0) || bound == 0))
                    {
                        count += item.StackSize;
                        if (count >= Amount)
                            return true;
                    }
                }
            }

            else
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        if (item.Bound == bound || bound > 0 && item.Bound > 0)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool ContainRune(uint ID, uint Amount)
        {

            uint count = 0;
            foreach (var item in ClientItems.Values)
            {
                if (item.ITEM_ID / 10000 == ID)
                {
                    count += item.StackSize;
                    if (count >= Amount)
                        return true;
                }
            }
            return false;
        }
        public bool AddPerfection(ServerSockets.Packet stream, uint ID, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool withPerfection = false)
        {
            if (HaveSpace(1))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.ProgresGreen = 0;
                        ItemDat.Enchant = 150;
                        ItemDat.Bless = 255;
                        ItemDat.StackSize = 1;
                        ItemDat.SocketProgress = (uint)(0 | (150 << 8) | (255 << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = 12;
                        ItemDat.PerfectionLevel = withPerfection ? 54u : 0u;
                        ItemDat.OwnerUID = Owner.Player.UID;
                        ItemDat.OwnerName = Owner.Player.Name;
                        ItemDat.Signature = Owner.Player.Name;
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                    else
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = 12;

                        if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Tower ||
                            Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Fan ||
                            Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.RidingCrop)
                        {

                            ItemDat.Bless = 1;
                        }
                        else if (Database.ItemType.ItemPosition(ID) != (ushort)Role.Flags.ConquerItem.Wing)
                        {
                            ItemDat.Bless = 7;

                        }

                        if (Database.ItemType.ItemPosition(ID) != (ushort)Role.Flags.ConquerItem.Tower &&
                           Database.ItemType.ItemPosition(ID) != (ushort)Role.Flags.ConquerItem.Fan &&
                           Database.ItemType.ItemPosition(ID) != (ushort)Role.Flags.ConquerItem.RidingCrop &&
                           Database.ItemType.ItemPosition(ID) != (ushort)Role.Flags.ConquerItem.Wing)
                        {
                            ItemDat.Enchant = 255;
                        }

                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Plus = 12;
                        ItemDat.PerfectionLevel = withPerfection ? 54u : 0u;
                        ItemDat.OwnerUID = Owner.Player.UID;
                        ItemDat.OwnerName = Owner.Player.Name;
                        ItemDat.Signature = Owner.Player.Name;

                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                    }
                }
                return true;

            }
            return false;
        }

        /// <summary>Creates the max-stat equipment sold by Stuff(B), without perfection.</summary>
        public bool AddStuff(ServerSockets.Packet stream, uint ID, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket)
        {
            return AddPerfection(stream, ID, sockone, socktwo, false);
        }

        public bool Add(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "", uint TimeLeftInMinutes = 0)
        {
            if (count == 0)
                count = 1;

            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.Effect = Effect;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;

                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                        if (TimeLeftInMinutes > 0)
                        {
                            ItemDat.TimeStamp = DateTime.Now.AddSeconds((TimeLeftInMinutes));
                            ItemDat.TimeLeftInMinutes = TimeLeftInMinutes;
                        }
                        if (SendMessage) Owner.SendSysMesage("You~received~" + (count == 1 ? "a" : count.ToString()) + "~" + DbItem.Name + (count == 1 ? "" : "(s)") + (!bound ? "" : "(B)") + another_text + "!");
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;

            }
            else
            {
                if (Owner.Warehouse.ClientItems.ContainsKey(ushort.MaxValue))
                {
                    if (!Owner.Player.OnAutoHunt && Owner.Warehouse.ClientItems[ushort.MaxValue].Count < 100)
                    {
                        AddReturnedItem(stream, ID, 1, plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, (ushort)1);
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (!Owner.Player.OnAutoHunt)
                    {
                        AddReturnedItem(stream, ID, 1, plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, (ushort)1);
                        return true;
                    }
                    else return false;
                }
            }
            return false;
        }

        public bool AddMythSoul(ServerSockets.Packet stream, uint ID, uint MythSoulProgress, uint MutacionID, uint MutacionID2, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
          , Role.Flags.Gem sockone = Flags.Gem.NoSocket
           , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
          , string another_text = "")
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.Effect = Effect;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.MythSoulProgress = MythSoulProgress;
                        ItemDat.Mutacion = MutacionID;
                        ItemDat.Mutacion2 = MutacionID2;
                        ItemDat.UpsurgeID = MutacionID2;
                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);


                        if (SendMessage) Owner.SendSysMesage("You~received~" + (count == 1 ? "a" : count.ToString()) + "~" + DbItem.Name + (count == 1 ? "" : "(s)") + (!bound ? "" : "(B)") + another_text + "!");
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }
            return false;
        }

        public bool AddRandomMythSoul(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "")
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.Effect = Effect;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;

                        if (SendMessage) Owner.SendSysMesage("You~received~" + (count == 1 ? "a" : count.ToString()) + "~" + DbItem.Name + (count == 1 ? "" : "(s)") + (!bound ? "" : "(B)") + another_text + "!");
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }
            return false;
        }

        public bool Add(uint ID, byte Plus, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool bound = false)
        {
            if (ITEMDB.StackSize > 0)
            {
                byte _bound = 0;
                if (bound)
                    _bound = 1;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID && item.Bound == _bound && item.Locked != 2)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            item.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                            if (bound)
                                item.Bound = 1;
                            item.Send(Owner, stream);

                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Pool.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = Plus;
                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;

                if (bound)
                    ItemDat.Bound = 1;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            else
            {
                if (Owner.Warehouse.ClientItems.ContainsKey(ushort.MaxValue))
                {
                    if (!Owner.Player.OnAutoHunt && Owner.Warehouse.ClientItems[ushort.MaxValue].Count < 100)
                    {
                        AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, (ushort)1);
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (!Owner.Player.OnAutoHunt)
                    {
                        AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, (ushort)1);
                        return true;
                    }
                    else return false;
                }
            }

        }
        public bool Add(Game.MsgServer.MsgGameItem ItemDat, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool LotteryPlay = false)
        {
            if (ITEMDB.StackSize > 0)
            {
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ItemDat.ITEM_ID && item.Locked != 2)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                            item.Send(Owner, stream);
                            return true;
                        }
                    }
                }
            }
            if (Owner.Player.OnAutoHunt == true && !LotteryPlay)
            {
                if (Owner.Warehouse.ClientItems.ContainsKey(Owner.Player.UID))
                {
                    if (Owner.Warehouse.ClientItems[Owner.Player.UID].Count < Owner.Player.InventorySashCount)
                    {
                        MsgGameItem DataItem = new MsgGameItem()
                        {
                            UID = Pool.ITEM_Counter.Next,
                            Effect = ItemDat.Effect,
                            ITEM_ID = ItemDat.ITEM_ID,
                            StackSize = 1
                        };
                        if (Owner.Warehouse.AddItem(DataItem, Owner.Player.UID))
                        {
                            Update(DataItem, AddMode.REMOVE, stream);
                            stream.WarehouseCreate(Owner.Player.UID, MsgWarehouse.DepositActionID.InventorySashDepositItem, 0, 0, 1);
                            stream.AddItemWarehouse(ItemDat);
                            Owner.Send(stream.FinalizeWarehouse());
                            DataItem.SendItemExtra(Owner, stream);
                            DataItem.SendItemLocked(Owner, stream);
                            return true;
                        }
                    }
                }

            }
            else
            {
                if (HaveSpace(1))
                {
                    ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                    Update(ItemDat, AddMode.ADD, stream);
                    return true;
                }
                else
                {
                    if (Owner.Warehouse.ClientItems.ContainsKey(ushort.MaxValue))
                    {
                        if (!Owner.Player.OnAutoHunt && Owner.Warehouse.ClientItems[ushort.MaxValue].Count < 100)
                        {
                            AddReturnedItem(stream, ItemDat.ITEM_ID, 1, ItemDat.Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, ItemDat.Bound >= 1 ? true : false, Flags.ItemEffect.None, (ushort)1);
                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        if (!Owner.Player.OnAutoHunt)
                        {
                            AddReturnedItem(stream, ItemDat.ITEM_ID, 1, ItemDat.Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, ItemDat.Bound >= 1 ? true : false, Flags.ItemEffect.None, (ushort)1);
                            return true;
                        }
                        else return false;
                    }
                }
            }
            return false;

        }

        public bool Remove(uint ID, uint count, ServerSockets.Packet stream)
        {
            if (Contain(ID, count) || Contain(ID, count, 1))
            {
                if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
                {
                    uint removed = 0;
                    for (uint x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == Database.ItemType.Meteor
                         || item.ITEM_ID == Database.ItemType.MeteorTear)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                else
                {
                    uint removed = 0;
                    for (uint x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == ID)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddItemWitchStack(uint ID, byte Plus, ushort amount, ServerSockets.Packet stream, bool bound = false)
        {
            Database.ItemType.DBItem DbItem;

            if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
            {

                if (DbItem.StackSize > 0)
                {
                    byte _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (var item in ClientItems.Values)
                    {
                        //  if (item.ITEM_ID == 4050001 || item.ITEM_ID == 4060001 || item.ITEM_ID == 4040001)
                        // return true;
                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + amount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += amount;
                                if (bound)
                                    item.Bound = 1;
                                item.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }

                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((amount / DbItem.StackSize) + (byte)(Owner.OnInterServer ? 1 : 0))))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Pool.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += DbItem.StackSize;
                                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                                ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                amount -= DbItem.StackSize;

                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Pool.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += amount;
                                ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            while (amount >= DbItem.StackSize)
                            {

                                amount -= DbItem.StackSize;
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {

                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (HaveSpace(1))
                        {
                            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                            ItemDat.UID = Pool.ITEM_Counter.Next;
                            ItemDat.ITEM_ID = ID;
                            ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                            ItemDat.Plus = Plus;
                            ItemDat.StackSize = amount;
                            ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                            ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                            if (bound)
                                ItemDat.Bound = 1;
                            try
                            {
                                Update(ItemDat, AddMode.ADD, stream);
                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                            }
                            return true;
                        }
                    }
                }
                for (int count = 0; count < amount; count++)
                    Add(ID, Plus, DbItem, stream, bound);
                return true;
            }
            return false;
        }

        public bool ContainItemWithStack(uint UID, ushort Count)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                return ItemDat.StackSize >= Count || Count == 1 && ItemDat.StackSize == 0;
            }
            return false;
        }

        public bool RemoveStackItem(uint UID, ushort Count, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                if (ItemDat.StackSize > Count)
                {
                    ItemDat.StackSize -= Count;
                    ItemDat.Mode = Flags.ItemMode.Update;
                    ItemDat.Send(Owner, stream);
                }
                else
                {
                    ItemDat.StackSize = 1;
                    Update(ItemDat, AddMode.REMOVE, stream);
                    return true;
                }
            }
            else
            {
                foreach (var item in ClientItems.Values)
                {
                    if (0 == Count)
                        break;
                    if (item.ITEM_ID == UID)
                    {
                        if (item.StackSize > Count)
                        {
                            item.StackSize -= Count;
                            item.Mode = Flags.ItemMode.Update;
                            item.Send(Owner, stream);
                            Count = 0;
                        }
                        else
                        {
                            Count -= item.StackSize;
                            item.StackSize = 1;
                            Update(item, AddMode.REMOVE, stream);
                        }
                    }
                }
            }
            return false;
        }
        public bool AddSoul(uint ID, uint SoulID, uint soullevel, uint souldays, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool bound)
        {
            Database.ItemType.DBItem Soul = null;
            Database.ItemType.DBItem ITEMDB = null;
            if (SoulID != 0)
            {
                if (!Pool.ItemsBase.TryGetValue((uint)SoulID, out Soul))
                    return false;

            }
            if (!Pool.ItemsBase.TryGetValue((uint)ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Pool.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(200, 240));
                }
                ItemDat.PerfectionLevel = 0;
                ItemDat.OwnerName = Owner.Player.Name;
                ItemDat.OwnerUID = Owner.Player.UID;
                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;

                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }
        public bool Add23(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
      , Role.Flags.Gem sockone = Flags.Gem.NoSocket
       , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
      , string another_text = "")
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.Effect = Effect;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.PerfectionLevel = 54;
                        ItemDat.OwnerName = Owner.Player.Name;
                        ItemDat.OwnerUID = Owner.Player.UID;
                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        //ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;

                        if (SendMessage) Owner.SendSysMesage("You~received~" + (count == 1 ? "a" : count.ToString()) + "~" + DbItem.Name + (count == 1 ? "" : "(s)") + (!bound ? "" : "(B)") + another_text + "!");
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }
            return false;
        }
        public bool AddSoul(ServerSockets.Packet stream, Client.GameClient client, uint ID, uint SoulID, uint purfylevel, byte plus = 0, byte count = 1, Qahr.Mobile.Role.Flags.Gem sockone = 0, Qahr.Mobile.Role.Flags.Gem socktwo = 0, bool bound = false, Qahr.Mobile.Role.Flags.ItemEffect Effect = 0, bool SendMessage = false, string another_text = "")
        {
            if (count == 0)
            {
                count = 1;
            }
            if (this.HaveSpace(count))
            {
                byte num = 0;
                while (num < count)
                {
                    Qahr.Mobile.Database.ItemType.DBItem item;
                    num = (byte)(num + 1);
                    if (Qahr.Mobile.Pool.ItemsBase.TryGetValue(ID, out item))
                    {
                        MsgGameItem item2;
                        item2 = new MsgGameItem
                        {
                            UID = Qahr.Mobile.Pool.ITEM_Counter.Next,
                            ITEM_ID = ID,
                            Effect = Effect,
                            Durability = item.Durability,
                            Plus = plus,
                            Bless = 7,
                            Enchant = 0xff,
                            SocketOne = sockone,
                            SocketTwo = socktwo,
                            PerfectionLevel = 0,
                            OwnerName = Owner.Player.Name,
                            OwnerUID = Owner.Player.UID,
                            Color = (Qahr.Mobile.Role.Flags.Color)Pool.GetRandom.Next(3, 9),
                            Bound = bound ? ((byte)1) : ((byte)0),
                            RemainingTime2 = (item.StackSize > 1) ? 0 : uint.MaxValue,
                            Purification = new MsgItemExtra.Purification()
                        };
                        item2.Purification.AddedOn = DateTime.Now;
                        item2.Purification.ItemUID = item2.UID;
                        item2.Purification.PurificationLevel = purfylevel;
                        item2.Purification.PurificationDuration = 0;
                        item2.Purification.PurificationItemID = SoulID;
                        item2.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                        MsgItemExtra extra = new MsgItemExtra
                        {
                            Purifications = { item2.Purification }
                        };
                        client.Send(extra.CreateArray(stream, false));
                        item2.Mode = Qahr.Mobile.Role.Flags.ItemMode.AddItem | Qahr.Mobile.Role.Flags.ItemMode.Trade;
                        item2.Send(client, stream);
                        if (SendMessage)
                        {
                            this.Owner.CreateBoxDialog("You~received~a~" + item.Name + another_text);
                        }
                        try
                        {
                            if (!this.Update(item2, AddMode.ADD, stream, false))
                            {
                                return false;
                            }
                        }
                        catch (Exception exception)
                        {
                            MyConsole.SaveException(exception);
                        }
                    }
                }
                if (num >= count)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Removeb(uint ID, uint count, ServerSockets.Packet stream, byte bound)
        {
            if (Contain(ID, count) || Contain(ID, count, 1))
            {
                if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
                {
                    uint removed = 0;
                    for (uint x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == Database.ItemType.Meteor
                         || item.ITEM_ID == Database.ItemType.MeteorTear && item.Bound == bound)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                else
                {
                    uint removed = 0;
                    for (uint x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == ID && item.Bound == bound)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                return true;
            }
            return false;
        }

        #region Relic
        public MsgGameItem AddRandomRelic(ServerSockets.Packet stream, bool bound = false)
        {

            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
            ItemDat.UID = Pool.ITEM_Counter.Next;
            ItemDat.StackSize = 1;
            ItemDat.ITEM_ID = BaseFunc.RandFromGivingNums(new uint[5] { 4100001, 4100002, 4100003, 4100004, 4100005 });
            ItemDat.Durability = ItemDat.MaximDurability = (ushort)((Pool.GetRandom.Next(10, 99) * 100) + 99);
            if (bound)
            {
                ItemDat.Bound = 1;
            }

            ItemDat.RelicAttributes = new RelicAttribute[5];
            int count = Math.Min(Pool.GetRandom.Next(1, 6), 4);
            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ItemDat.ITEM_ID).ToArray();
            for (int q = 0; q < count; q++)
            {
            again:
                MsgChiInfo.ChiAttribute type = Database.ItemType.RelicAttribute(ItemDat.ITEM_ID);
                ItemDat.RelicAttributes[q].Type = type;
            reroll:
                ItemDat.RelicAttributes[q].Value = (ushort)Pool.GetRandom.Next((int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Min, (int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max + 1);
                while (ItemDat.RelicAttributes[q].Value / (double)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max * 100d < 60 && Role.Core.Rate(55))
                {
                    goto reroll;
                }

                if (ItemDat.RelicAttributes[q].Value >= Attrs.Where(i => i.Attribute == type).LastOrDefault().Max)
                {
                    ItemDat.RelicAttributes[q].Value = (ushort)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max;
                }

                ItemDat.RelicAttributes[q].Epic = Attrs.Where(i => i.Attribute == type).LastOrDefault().dwParam;
                if ((ItemDat.RelicAttributes.Count(i => i.Type == type) > 1)
                    || (type == MsgChiInfo.ChiAttribute.None))
                {
                    goto again;
                }
            }
            try
            {
                Update(ItemDat, AddMode.ADD, stream);
                return ItemDat;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return null;
        }

        public MsgGameItem AddRandomRelicL5(ServerSockets.Packet stream, uint _ITEM_ID = 0, bool bound = false)
        {
            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
            ItemDat.UID = Pool.ITEM_Counter.Next;
            ItemDat.StackSize = 1;
            if (_ITEM_ID > 0)
            {
                ItemDat.ITEM_ID = _ITEM_ID;
            }
            else
            {
                ItemDat.ITEM_ID = BaseFunc.RandFromGivingNums(new uint[5] { 4100001, 4100002, 4100003, 4100004, 4100005 });
            }

            ItemDat.Durability = ItemDat.MaximDurability = (ushort)((Pool.GetRandom.Next(10, 99) * 100) + 99);
            if (bound)
            {
                ItemDat.Bound = 1;
            }

            ItemDat.RelicAttributes = new RelicAttribute[5];
            int count = ItemDat.RelicAttributes.Length;
            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ItemDat.ITEM_ID).ToArray();
            for (int q = 0; q < count; q++)
            {
            again:
                MsgChiInfo.ChiAttribute type = Database.ItemType.RelicAttribute(ItemDat.ITEM_ID);
                ItemDat.RelicAttributes[q].Type = type;
            reroll:
                ItemDat.RelicAttributes[q].Value = (ushort)Pool.GetRandom.Next((int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Min, (int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max + 1);
                while (ItemDat.RelicAttributes[q].Value / (double)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max * 100d < 60 && Role.Core.Rate(55))
                {
                    goto reroll;
                }

                if (ItemDat.RelicAttributes[q].Value >= Attrs.Where(i => i.Attribute == type).LastOrDefault().Max)
                {
                    ItemDat.RelicAttributes[q].Value = (ushort)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max;
                }

                ItemDat.RelicAttributes[q].Epic = Attrs.Where(i => i.Attribute == type).LastOrDefault().dwParam;
                if ((ItemDat.RelicAttributes.Count(i => i.Type == type) > 1)
                    || (type == MsgChiInfo.ChiAttribute.None))
                {
                    goto again;
                }
            }
            try
            {
                MsgXuanBaoOpt.xuanbaoadditionattr(ItemDat);
                Update(ItemDat, AddMode.ADD, stream);
                return ItemDat;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return null;
        }
        public MsgGameItem AddRandomRelicL1(ServerSockets.Packet stream, bool bound = false)
        {
            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
            ItemDat.UID = Pool.ITEM_Counter.Next;
            ItemDat.StackSize = 1;
            ItemDat.ITEM_ID = BaseFunc.RandFromGivingNums(new uint[5] { 4100001, 4100002, 4100003, 4100004, 4100005 });
            ItemDat.Durability = ItemDat.MaximDurability = (ushort)((Pool.GetRandom.Next(10, 99) * 100) + 99);
            if (bound)
                ItemDat.Bound = 1;
            ItemDat.RelicAttributes = new RelicAttribute[5];
            int count = Math.Min(Pool.GetRandom.Next(1, 6), 4);
            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ItemDat.ITEM_ID).ToArray();
            for (int q = 0; q < count; q++)
            {
            again:
                MsgChiInfo.ChiAttribute type = Database.ItemType.RelicAttribute(ItemDat.ITEM_ID);
                ItemDat.RelicAttributes[q].Type = type;
            reroll:
                ItemDat.RelicAttributes[q].Value = (ushort)Pool.GetRandom.Next((int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Min, (int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max + 1);
                while ((double)ItemDat.RelicAttributes[q].Value / (double)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max * 100d < 60 && Role.Core.Rate(55))
                    goto reroll;
                if (ItemDat.RelicAttributes[q].Value >= Attrs.Where(i => i.Attribute == type).LastOrDefault().Max)
                    ItemDat.RelicAttributes[q].Value = (ushort)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max;
                ItemDat.RelicAttributes[q].Epic = Attrs.Where(i => i.Attribute == type).LastOrDefault().dwParam;
                if ((ItemDat.RelicAttributes.Count(i => i.Type == type) > 1)
                    || (type == MsgChiInfo.ChiAttribute.None))
                    goto again;
            }
            try
            {
                Update(ItemDat, AddMode.ADD, stream);
                return ItemDat;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return null;
        }
        #endregion

        #region NpcHand
        public bool AddSteed1(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.StackSize = 1;
                        ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.PerfectionLevel = 54;
                        ItemDat.OwnerName = Owner.Player.Name;
                        ItemDat.OwnerUID = Owner.Player.UID;
                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool AddSteedFull(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;

                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.StackSize = 1;
                        ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;

                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = 1;
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool AddSteed(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Pool.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;

                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.StackSize = 1;
                        ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool AddBotBooth(ServerSockets.Packet stream, uint ID, ushort stackcount = 1, byte plus = 0, byte bless = 0, byte Enchant = 0, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false, string another_text = "")
        {
            if (stackcount == 0)
                stackcount = 1;
            Database.ItemType.DBItem DbItem;
            if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
            {
                byte _bound = (byte)(bound ? 1 : 0);

                if (DbItem.StackSize > 1)
                {
                    foreach (var item in ClientItems.Values)
                    {
                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + stackcount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += stackcount;
                                item.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }
                }

                if (HaveSpace(1))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                    ItemDat.Effect = Effect;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.StackSize = stackcount;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = _bound;

                    try
                    {
                        if (Update(ItemDat, AddMode.ADD, stream))
                            return true;
                    }
                    catch (Exception e)
                    {
                        MyConsole.SaveException(e);
                        return false;
                    }
                }
            }
            return false;
        }

        public bool AddTime(ServerSockets.Packet stream, uint ID, byte plus = 0, byte bless = 0, byte Enchant = 0, bool bound = false, uint TimeLeftInMinutes1 = 0)
        {
            if (HaveSpace(1))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.StackSize = 1;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    ItemDat.TimeStamp = DateTime.Now.AddSeconds((TimeLeftInMinutes1));
                    ItemDat.Activate = 1;
                    TimeSpan timeSpan = ItemDat.TimeStamp - DateTime.Now;
                    ItemDat.TimeLeftInMinutes = (uint)timeSpan.TotalSeconds;
                    try
                    {
                        if (!Update(ItemDat, AddMode.ADD, stream))
                            return false;
                    }
                    catch (Exception e)
                    {
                        MyConsole.SaveException(e);
                    }

                }
            }
            return false;
        }


        #endregion

        public MsgGameItem Relice(ServerSockets.Packet stream, bool bound = false, bool Max = false, uint un = 0, uint deux = 0, uint trois = 0, uint quatre = 0, uint cinq = 0)
        {
            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
            ItemDat.UID = Pool.ITEM_Counter.Next;
            ItemDat.StackSize = 1;
            ItemDat.ITEM_ID = 4100001;
            if (un == 7)
            {
                ItemDat.ITEM_ID = 4100004;
            }

            if (un == 6)
            {
                ItemDat.ITEM_ID = 4100002;
            }

            if (un == 1)
            {
                ItemDat.ITEM_ID = 4100005;
            }

            if (un == 4)
            {
                ItemDat.ITEM_ID = 4100005;
            }

            if (un == 3)
            {
                ItemDat.ITEM_ID = 4100005;
            }

            ItemDat.Durability = ItemDat.MaximDurability = 10099;
            if (bound)
            {
                ItemDat.Bound = 1;
            }

            ItemDat.RelicAttributes = new RelicAttribute[5];
            int count = Max ? ItemDat.RelicAttributes.Length : Math.Min(Pool.GetRandom.Next(5, 7), ItemDat.RelicAttributes.Length);
            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ItemDat.ITEM_ID).ToArray();
            for (int q = 0; q < 5; q++)
            {
            again:
                Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute type = Database.ItemType.RelicAttribute(ItemDat.ITEM_ID);
                ItemDat.RelicAttributes[q].Type = type;
                if (q == 0) { ItemDat.RelicAttributes[q].Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)un; }
                if (q == 1) { ItemDat.RelicAttributes[q].Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)deux; }
                if (q == 2) { ItemDat.RelicAttributes[q].Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)trois; }
                if (q == 3) { ItemDat.RelicAttributes[q].Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)quatre; }
                if (q == 4) { ItemDat.RelicAttributes[q].Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)cinq; }
                var ComposeLimitDB = Database.RuneTable.composelimit.Where(i => i.Attribute == ItemDat.RelicAttributes[q].Type).ToArray().FirstOrDefault();
                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                if (ItemDat.RelicAttributes[q].Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease)
                {
                    ItemDat.RelicAttributes[q].Value = ComposeLimitDB.Max;
                }

                ItemDat.RelicAttributes[q].Epic = Attrs.Where(i => i.Attribute == type).LastOrDefault().dwParam;
                if ((ItemDat.RelicAttributes.Count(i => i.Type == type) > 1)
                    || (type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.None)
                    || !ItemDat.RelicAttributes[q].Epic)
                {
                    goto again;
                }
            }


            try
            {
                MsgXuanBaoOpt.xuanbaoadditionattr(ItemDat);
                Update(ItemDat, AddMode.ADD, stream);
                return ItemDat;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return null;
        }
        public void AddReturnedItem(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, ushort StackSize = 0)
        {

            byte x = 0;
            for (; x < count; )
            {
                x++;
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                    ItemDat.Effect = Effect;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.StackSize = StackSize;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    ItemDat.Mode = Flags.ItemMode.AddItemReturned;
                    ItemDat.WH_ID = ushort.MaxValue;

                    Owner.Warehouse.AddItem(ItemDat, ushort.MaxValue);

                    ItemDat.Send(Owner, stream);
                }
            }
        }

        public uint AddInbox(ServerSockets.Packet stream, uint ID, byte plus = 0, byte bless = 0, byte Enchant = 0, Role.Flags.Gem sockone = Role.Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Role.Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Role.Flags.ItemEffect.None, ushort StackSize = 0)
        {
            Database.ItemType.DBItem dbItem;
            if (!Pool.ItemsBase.TryGetValue(ID, out dbItem))
                return 0;
            MsgGameItem DataItem = new MsgGameItem()
            {
                UID = Pool.ITEM_Counter.Next,
                Effect = Effect,
                ITEM_ID = ID,
                StackSize = StackSize
            };
            DataItem.Durability = DataItem.MaximDurability = dbItem.Durability;
            DataItem.Plus = plus;
            DataItem.Bless = bless;
            DataItem.Enchant = Enchant;
            DataItem.SocketOne = sockone;
            DataItem.SocketTwo = socktwo;
            DataItem.Bound = bound ? (byte)1 : (byte)0;
            DataItem.Mode = Role.Flags.ItemMode.Inbox;
            DataItem.WH_ID = 1000;
            this.Owner.Warehouse.AddItem(DataItem, 1000);
            DataItem.Send(this.Owner, stream);
            return DataItem.UID;
        }

        public unsafe bool Update(Game.MsgServer.MsgGameItem ItemDat, AddMode mode, ServerSockets.Packet stream, bool Removefull = false)
        {
            if (ItemDat.ITEM_ID >= 4200001 && ItemDat.ITEM_ID <= 4200019)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Anima] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 3004123 || ItemDat.ITEM_ID == 780000 || ItemDat.ITEM_ID == 780001)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Tokens VIP] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 3315888 || ItemDat.ITEM_ID == 3315894 || ItemDat.ITEM_ID == 3315895 || ItemDat.ITEM_ID == 3315897 || ItemDat.ITEM_ID == 3315898 || ItemDat.ITEM_ID == 3315899 || ItemDat.ITEM_ID == 3315871
                || ItemDat.ITEM_ID == 3315872 || ItemDat.ITEM_ID == 3315873 || ItemDat.ITEM_ID == 3315874 || ItemDat.ITEM_ID == 3315875 || ItemDat.ITEM_ID == 3315876 || ItemDat.ITEM_ID == 3315877 || ItemDat.ITEM_ID == 3315878
                || ItemDat.ITEM_ID == 3315879 || ItemDat.ITEM_ID == 3315880 || ItemDat.ITEM_ID == 3315881 || ItemDat.ITEM_ID == 3315882 || ItemDat.ITEM_ID == 3315883)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check Charging Bags] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 730001 || ItemDat.ITEM_ID == 730002 || ItemDat.ITEM_ID == 730003 || ItemDat.ITEM_ID == 730004 || ItemDat.ITEM_ID == 730005 || ItemDat.ITEM_ID == 730006 || ItemDat.ITEM_ID == 730007 || ItemDat.ITEM_ID == 730008)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Stones] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 3314214 || ItemDat.ITEM_ID == 3314215 || ItemDat.ITEM_ID == 3314216 || ItemDat.ITEM_ID == 3314217 || ItemDat.ITEM_ID == 3314218 || ItemDat.ITEM_ID == 3314219 || ItemDat.ITEM_ID == 3314220 || ItemDat.ITEM_ID == 3314221)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Steeds] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 3009000 || ItemDat.ITEM_ID == 3009001 || ItemDat.ITEM_ID == 3009002 || ItemDat.ITEM_ID == 3009003)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Stars Stones] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);

            if (ItemDat.ITEM_ID == 2169805 || ItemDat.ITEM_ID == 2169815 || ItemDat.ITEM_ID == 2169825 || ItemDat.ITEM_ID == 2169835 || ItemDat.ITEM_ID == 2169845 || ItemDat.ITEM_ID == 2169855 || ItemDat.ITEM_ID == 2169865 || ItemDat.ITEM_ID == 2169875 || ItemDat.ITEM_ID == 2169885 || ItemDat.ITEM_ID == 2169895 || ItemDat.ITEM_ID == 2169905 || ItemDat.ITEM_ID == 2169915 || ItemDat.ITEM_ID == 2169925 || ItemDat.ITEM_ID == 2169935 || ItemDat.ITEM_ID == 2169945)
                Database.ServerDatabase.LoginQueue.Enqueue("[Check All Jade Kylin] Player Name: " + Owner.Player.Name + " Mode " + mode.ToString() + " |" + ItemDat.ToLog() + "| Stake [ " + ItemDat.StackSize + " ]" + Environment.StackTrace);



            if (HaveSpace(1) || mode == AddMode.REMOVE)
            {
                switch (mode)
                {
                    case AddMode.ADD:
                        {
                            CheakUp(ItemDat);
                            if (ItemDat.StackSize == 0)
                                ItemDat.StackSize++;
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            if (Owner.IsConnectedInterServer())
                            {
                                ItemDat.Send(Owner.PipeClient, stream);
                            }

                            break;
                        }
                    case AddMode.MOVE:
                        {

                            CheakUp(ItemDat);
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            break;
                        }
                    case AddMode.REMOVE:
                        {
                            if (ItemDat.StackSize > 1 && ItemDat.Position < 40 && !Removefull)
                            {
                                ItemDat.StackSize -= 1;
                                ItemDat.Mode = Flags.ItemMode.Update;
                                ItemDat.Send(Owner, stream);
                                break;
                            }
                            Game.MsgServer.MsgGameItem item;
                            if (ClientItems.TryRemove(ItemDat.UID, out item))
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, item.UID, 0, 0, 0, 0, 0, 0));
                            }
                            break;
                        }
                }

                if (ItemDat.ITEM_ID == 750000)
                {
                    Owner.DemonExterminator.ItemUID = ItemDat.UID;
                    if (mode == AddMode.REMOVE)
                        Owner.DemonExterminator.ItemUID = 0;
                }


                return true;

            }
            return false;
        }

        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Pool.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }

        public bool CheckMeteors(ushort count, bool Removethat, ServerSockets.Packet stream)
        {

            if (Contain(1088001, count))
            {
                if (Removethat)
                    Remove(1088001, count, stream);
                return true;
            }
            else
            {
                ushort Counter = 0;
                var RemoveThis = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                var MyMetscrolls = GetMyMetscrolls();
                var MyMeteors = GetMyMeteors();
                foreach (var GameItem in MyMetscrolls.Values)
                {
                    Counter += 10;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    byte needSpace = (byte)(Counter - count);
                    if (HaveSpace(needSpace))
                    {
                        if (Removethat)
                        {
                            //  Add(stream, 1088001, 0, (byte)needSpace);
                        }
                    }
                    else
                    {
                        Counter -= 10;
                        RemoveThis.Remove(RemoveThis.Values.First().UID);
                        ushort needmetsss = (ushort)(count - Counter);
                        if (needmetsss <= MyMeteors.Count)
                        {
                            foreach (var GameItem in MyMeteors.Values)
                            {
                                Counter += 1;
                                RemoveThis.Add(GameItem.UID, GameItem);
                                if (Counter >= count)
                                    break;
                            }
                            if (Removethat)
                            {
                                foreach (var GameItem in RemoveThis.Values)
                                    Update(GameItem, AddMode.REMOVE, stream);
                            }
                        }
                        else
                            return false;
                    }
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
                foreach (var GameItem in MyMeteors.Values)
                {
                    Counter += 1;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
            }

            return false;
        }

        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMetscrolls()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == 720027)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMeteors()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == Database.ItemType.Meteor || GameItem.ITEM_ID == Database.ItemType.MeteorTear)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }


        public void ShowALL(ServerSockets.Packet stream)
        {
            uint Exp = 0;
            foreach (var msg_item in ClientItems.Values)
            {
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }

            foreach (var msg_item in Owner.EonspiritSystem.Values)
            {
                var Info = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == msg_item.ITEM_ID % 100 && i.TypeLevel == msg_item.EonspiritPercentage);
                if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive)
                {
                    Owner.Player.EonspiritLevel = msg_item.ITEM_ID % 100;
                    Owner.Player.EonspiritPrestrige = Info.Rating;
                }
                if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive || msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                {
                    Exp += Info.Rating;
                }
                Owner.Player.EonspiritCurrentEnergy = 0;
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }
            if (Exp > 0)
            {
                var entry = new Database.YuanshenRank.Entry()
                {
                    Type = Database.YuanshenRank.Type.Overall_EonSpirit,
                    TotalPoints = Exp,
                    UID = Owner.Player.UID,
                    Name = Owner.Player.Name,
                    Level = (byte)Owner.Player.Level,
                    Class = Owner.Player.Class,
                    Mesh = Owner.Player.Mesh,
                };
                entry.AddInfo(Owner);
                Database.YuanshenRank.Ranks[Database.YuanshenRank.Type.Overall_EonSpirit].UpdateItem(entry);
            }
            else
            {
                Database.YuanshenRank.Remove(Owner.Player.UID);
            }
            foreach (var msg_item in Owner.MythSoulBag.Values)
            {
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }
        }


        public bool AddAnimaLock(ServerSockets.Packet stream, uint ID, int UnLockTimer = 0, byte Locked = 0)
        {
            Database.ItemType.DBItem DbItem;
            if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
            {
                if (Locked == 0)
                    return AddItemWitchStack(ID, 0, 1, stream);
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Pool.ITEM_Counter.Next;
                ItemDat.Effect = Flags.ItemEffect.None;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                ItemDat.Plus = 0;
                ItemDat.Bless = 0;
                ItemDat.Enchant = 0;
                ItemDat.SocketOne = Flags.Gem.NoSocket;
                ItemDat.SocketTwo = Flags.Gem.NoSocket;
                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                ItemDat.Bound = 0;
                ItemDat.Locked = Locked;
                ItemDat.UnLockTimer = UnLockTimer;
                try
                {
                    if (!Update(ItemDat, AddMode.ADD, stream))
                        return false;
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }

        public bool Contains(uint ID, uint MaxStack, uint Amount, out MsgGameItem Item, byte Bound)
        {
            Item = null;
            uint StackSize = 0;
            foreach (MsgGameItem item in ClientItems.Values)
            {
                StackSize = item.StackSize;
                if (item != null && item.ITEM_ID == ID && item.Bound == Bound && (StackSize + Amount) <= MaxStack)
                {
                    Item = item;
                    return true;
                }
            }
            return false;
        }

        public void Clear(ServerSockets.Packet stream)
        {
            var dictionary = ClientItems.Values.ToArray();
            foreach (var msg_item in dictionary)
                Update(msg_item, AddMode.REMOVE, stream, true);
        }


        //internal unsafe void AddRandomRelic(ServerSockets.Packet packet, bool p1, bool p2)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
