using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{

    public static class MsgCoatStorage
    {
        [ProtoContract]
        public class CoatStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ActionID;

            [ProtoMember(2, IsRequired = false)]
            public uint ID;

            [ProtoMember(3, IsRequired = true)]
            public uint dwpram2;

            [ProtoMember(4, IsRequired = true)]
            public uint dwpram3;

            [ProtoMember(5, IsRequired = false)]
            public ItemStorage[] Item;

            [ProtoMember(6, IsRequired = true)]
            public long Time;

            [ProtoMember(7, IsRequired = true)]
            public List<ItemColor> MountColor = new List<ItemColor>();

            [ProtoMember(8, IsRequired = true)]
            public int Remove = 0;

        }
        [ProtoContract]
        public class ItemStorage
        {
            public ItemStorage(Game.MsgServer.MsgGameItem item)
            {
                ItemUID = item.UID;
                ItemID = item.ITEM_ID;
                MaxDurability = MinDurability = item.MaximDurability;
                Stack = 1;
                Plus = item.Plus;
                Bless = item.Bless;

                Type1 = item.Bound;
                Type2 = item.Bound; Type1 = item.Bound;

            }
            [ProtoMember(1, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(3, IsRequired = true)]
            public uint dwparam3;
            [ProtoMember(4, IsRequired = true)]
            public uint dwparam4;
            [ProtoMember(5, IsRequired = true)]
            public uint dwparam5;
            [ProtoMember(6, IsRequired = true)]
            public uint dwparam6;
            [ProtoMember(7, IsRequired = true)]
            public uint dwparam7;
            [ProtoMember(8, IsRequired = true)]
            public uint Plus;
            [ProtoMember(9, IsRequired = true)]
            public uint Bless;
            [ProtoMember(10, IsRequired = true)]
            public uint Type1;//??
            [ProtoMember(11, IsRequired = true)]
            public uint dwparam11;
            [ProtoMember(12, IsRequired = true)]
            public uint dwparam12;
            [ProtoMember(13, IsRequired = true)]
            public uint dwparam13;
            [ProtoMember(14, IsRequired = true)]
            public uint dwparam14;
            [ProtoMember(15, IsRequired = true)]
            public uint Type2;
            [ProtoMember(16, IsRequired = true)]
            public uint dwparam16;
            [ProtoMember(17, IsRequired = true)]
            public uint dwparam17;

            [ProtoMember(18, IsRequired = true)]
            public uint TimeLeft;

            [ProtoMember(19, IsRequired = true)]
            public uint Stack;
            [ProtoMember(20, IsRequired = true)]
            public uint MinDurability;
            [ProtoMember(21, IsRequired = true)]
            public uint MaxDurability;
            [ProtoMember(22, IsRequired = true)]
            public uint Color;

        }
        [ProtoContract]
        public class ItemColor
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ITEMID;
            [ProtoMember(2, IsRequired = true)]
            public ulong TIMEITEM; // [ProtoFix 2026-10-14] client CMsgCoatColorInfoPB.TIMEITEM is uint64 (was uint)
            [ProtoMember(3, IsRequired = true)]
            public uint ColorID;
            [ProtoMember(4, IsRequired = true)]
            public uint RGB;
            [ProtoMember(5, IsRequired = true)]
            public ulong ExpiryTIME; // [ProtoFix 2026-10-14] client CMsgCoatColorInfoPB.ExpiryTIME is uint64 (was uint)
        }
        [Flags]
        public enum Types : uint
        {
            Equip = 1,
            Retrive = 2,
            Update = 3,
            AddToWardRobe = 4,
            TakeOff = 5,
            Expire = 6,
            LoadAnthoerWardrobe = 8,
            //COLOR
            LoadColor = 10,
            RandomChange = 11,

            ColorFix = 12,
            STORAGE_COLOR_FIXING2 = 13,
            STORAGE_EQUIP_COLOR = 14,

        }
        public static unsafe ServerSockets.Packet CreateCoatStorage(this ServerSockets.Packet stream, CoatStorage obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCoatStorage);

            return stream;
        }
        public static unsafe void GetCoatStorage(this ServerSockets.Packet stream, out CoatStorage pQuery)
        {
            pQuery = new CoatStorage();
            pQuery = stream.ProtoBufferDeserialize<CoatStorage>(pQuery);
        }
        public static bool CheckGender(Client.GameClient user, uint itemid)
        {
            Database.ItemType.DBItem dbitem;
            if (Pool.ItemsBase.TryGetValue(itemid, out dbitem))
            {
                if (dbitem.Gender == 0)
                    return true;
                if (dbitem.Gender == 1)//boy
                {
                    if (Role.Core.IsBoy(user.Player.Body))
                        return true;
                }
                if (dbitem.Gender == 2)//female
                {
                    if (Role.Core.IsGirl(user.Player.Body))
                        return true;
                }
            }
            return false;
        }
        [PacketAttribute(GamePackets.MsgCoatStorage)]
        private unsafe static void Process(Client.GameClient Client, ServerSockets.Packet stream)
        {
            CoatStorage pMsg;
            stream.GetCoatStorage(out pMsg);
            // QahrMobile: the phone numbers its wardrobe actions differently (5 = wear, 6 = take off) and also stores
            // weapon accessories; Mobile\MobileWardrobe.cs handles 1/2/5/6 for mobile clients
            if (global::Qahr.Mobile.Phone.MobileWardrobe.Handle(Client, stream, pMsg))
            {
                Database.TitleStorage.CheckUpUser(Client, stream);
                return;
            }
            switch ((Types)pMsg.ActionID)
            {
                case Types.Equip:
                    {
                        if (Client.IsVendor)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (Client.InTrade)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }

                        Game.MsgServer.MsgGameItem item;
                        if (Client.Inventory.TryGetItem(pMsg.ID, out item))
                        {
                            if (Client.MyWardrobe.Contain(item.ITEM_ID))
                                return;
                            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment)
                            {
                                if (Client.Player.SpecialGarment != 0)
                                {

                                    Client.SendSysMesage("Item can't be unequiped during the event.");


                                    return;
                                }
                            }
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment || Position == (ushort)Role.Flags.ConquerItem.SteedMount)
                            {
                                if (item.Locked > 0)
                                {
                                    Client.SendSysMesage("This item is locked and can't be stored.");
                                    return;
                                }

                                // Deposit must take the item OUT of the bag first; otherwise the same
                                // object stays in both the bag and the wardrobe (permanent dupe).
                                // Removefull=true so a stacked item leaves the bag entirely.
                                Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                if (Client.Inventory.ClientItems.ContainsKey(item.UID))
                                    return; // removal failed / another thread grabbed it

                                Client.MyWardrobe.AddItem(item);

                                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                store.ActionID = (uint)MsgCoatStorage.Types.Equip;
                                store.ID = item.UID;
                                Client.Send(stream.CreateCoatStorage(store));

                            }

                        }
                        break;
                    }

                case Types.Update:
                    {
                        MsgGameItem item, ExistedItem;
                        if (Client.Inventory.TryGetItem(pMsg.dwpram2, out item))
                        {
                            if (Client.MyWardrobe.TryGetItem(pMsg.ID, out ExistedItem))
                            {
                                // The feeder item must be the exact same ITEM_ID as the wardrobe item it renews;
                                // otherwise any item could be fed to make an event item permanent or unbind it.
                                if (item != null && ExistedItem != null && item.ITEM_ID == ExistedItem.ITEM_ID)
                                {
                                    // Remove the fed item fully (Removefull) so a stacked feeder can't leave a copy.
                                    if (ExistedItem.TimeLeftInMinutes > 0 && item.TimeLeftInMinutes == 0)
                                    {
                                        if (!Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true))
                                            break;
                                        if (Client.Inventory.ClientItems.ContainsKey(item.UID))
                                            break;
                                        ExistedItem.TimeLeftInMinutes = item.TimeLeftInMinutes;
                                        ExistedItem.TimeStamp = DateTime.Now.AddSeconds((item.TimeLeftInMinutes));
                                        var store = new CoatStorage();
                                        store.ActionID = (uint)Types.Expire;
                                        store.ID = ExistedItem.UID;
                                        store.dwpram2 = ExistedItem.ITEM_ID;
                                        store.Item = new MsgCoatStorage.ItemStorage[1];
                                        store.Item[0] = new ItemStorage(ExistedItem);
                                        store.Item[0].dwparam17 = item.TimeLeftInMinutes;
                                        store.Item[0].TimeLeft = item.TimeLeftInMinutes / 60;
                                        Client.Send(stream.CreateCoatStorage(store));
                                    }
                                    else if (ExistedItem.TimeLeftInMinutes > 0 && item.TimeLeftInMinutes > 0 && ExistedItem.Bound == item.Bound)
                                    {
                                        if (!Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true))
                                            break;
                                        if (Client.Inventory.ClientItems.ContainsKey(item.UID))
                                            break;
                                        uint Date = ExistedItem.TimeLeftInMinutes + item.TimeLeftInMinutes;
                                        ExistedItem.TimeStamp = DateTime.Now.AddSeconds((Date));
                                        ExistedItem.TimeLeftInMinutes = Date;
                                        var store = new CoatStorage();
                                        store.ActionID = (uint)Types.Expire;
                                        store.ID = ExistedItem.UID;
                                        store.dwpram2 = ExistedItem.ITEM_ID;
                                        store.Item = new MsgCoatStorage.ItemStorage[1];
                                        store.Item[0] = new ItemStorage(ExistedItem);
                                        store.Item[0].dwparam17 = Date;
                                        store.Item[0].TimeLeft = item.TimeLeftInMinutes / 60;
                                        Client.Send(stream.CreateCoatStorage(store));
                                    }
                                    else if (ExistedItem.Bound == 1 && item.Bound == 0)
                                    {
                                        if (!Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true))
                                            break;
                                        if (Client.Inventory.ClientItems.ContainsKey(item.UID))
                                            break;
                                        ExistedItem.Bound = item.Bound;
                                        var store = new CoatStorage();
                                        store.ActionID = (uint)Types.Expire;
                                        store.ID = ExistedItem.UID;
                                        store.dwpram2 = ExistedItem.ITEM_ID;
                                        store.Item = new MsgCoatStorage.ItemStorage[1];
                                        store.Item[0] = new ItemStorage(ExistedItem);
                                        store.Item[0].dwparam17 = item.TimeLeftInMinutes;
                                        store.Item[0].TimeLeft = item.TimeLeftInMinutes / 60;
                                        Client.Send(stream.CreateCoatStorage(store));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Types.AddToWardRobe:
                    {
                        if (Client.IsVendor)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (Client.InTrade)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        Game.MsgServer.MsgGameItem item;

                        if (Client.MyWardrobe.TryGetItem(pMsg.ID, out item))
                        {
                            if (!CheckGender(Client, item.ITEM_ID))
                                break;
                            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment || Position == (ushort)Role.Flags.ConquerItem.SteedMount)
                            {
                                if (Client.Equipment.FreeEquip((Role.Flags.ConquerItem)Position))
                                {
                                    if (Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    {
                                        item.Position = (ushort)Position;
                                        Client.Equipment.Add(item, stream);
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(Client, stream);
                                    }
                                }
                                else
                                {
                                    if (Client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    {
                                        if (Client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream))
                                        {
                                            item.Position = (ushort)Position;
                                            Client.Equipment.Add(item, stream);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(Client, stream);
                                        }
                                    }
                                }
                            }
                            Client.Equipment.QueryEquipment(Client.Equipment.Alternante);
                            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            store.ActionID = (uint)MsgCoatStorage.Types.AddToWardRobe;
                            store.ID = item.UID;
                            store.dwpram2 = item.ITEM_ID;
                            Client.Send(stream.CreateCoatStorage(store));
                        }
                        break;
                    }
                case Types.TakeOff:
                    {
                        Game.MsgServer.MsgGameItem item;
                        if (Client.Equipment.TryGetEquip((Role.Flags.ConquerItem)pMsg.dwpram2, out item))
                        {
                            if (item.Position == (ushort)Role.Flags.ConquerItem.Garment)
                            {
                                if (Client.Player.SpecialGarment != 0)
                                {

                                    Client.SendSysMesage("Item can't be unequiped during the event.");


                                    return;
                                }
                            }
                            if (Client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, stream))
                            {
                                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                store.ActionID = (uint)MsgCoatStorage.Types.TakeOff;
                                store.ID = item.UID;
                                store.dwpram2 = item.ITEM_ID;
                                Client.Send(stream.CreateCoatStorage(store));

                                Client.Equipment.QueryEquipment(Client.Equipment.Alternante);
                            }
                        }
                        break;
                    }
                case Types.Retrive:
                    {
                        if (Client.IsVendor)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (Client.InTrade)
                        {
                            Client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (!Client.Inventory.HaveSpace(1))
                        {
                            Client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        Game.MsgServer.MsgGameItem item2;
                        if (Client.Equipment.TryGetValue(pMsg.ID, out item2))
                        {
                            if (item2.UID == pMsg.ID)
                            {
                                if (Client.Equipment.Remove((Role.Flags.ConquerItem)item2.Position, stream))
                                {
                                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                    store.ActionID = (uint)MsgCoatStorage.Types.TakeOff;
                                    store.ID = item2.UID;
                                    store.dwpram2 = item2.ITEM_ID;
                                    Client.Send(stream.CreateCoatStorage(store));
                                    Client.Equipment.QueryEquipment(Client.Equipment.Alternante);
                                }
                            }
                        }

                        Game.MsgServer.MsgGameItem item;
                        if (Client.MyWardrobe.RemoveItem(pMsg.ID, out item))
                        {
                            Client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                            Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            astore.ActionID = (uint)MsgCoatStorage.Types.Retrive;
                            astore.ID = pMsg.ID;
                            astore.dwpram2 = item.ITEM_ID;
                            Client.Send(stream.CreateCoatStorage(astore));
                            Client.Equipment.QueryEquipment(Client.Equipment.Alternante);
                        }
                        break;
                    }
                case MsgCoatStorage.Types.LoadAnthoerWardrobe:
                    {
                        Client.GameClient Target;
                        if (Pool.GamePoll.TryGetValue(pMsg.ID, out Target))
                        {
                            Target.MyWardrobe.LoadAnthoerWardrobe(stream, Client);
                        }
                        break;
                    }
                case MsgCoatStorage.Types.LoadColor:
                    {
                        Client.CoatColorRule.LoadColors(pMsg.ID);
                        break;
                    }
                case MsgCoatStorage.Types.RandomChange:
                    {
                        Client.CoatColorRule.RandomChange(pMsg.ID);
                        break;
                    }
                case MsgCoatStorage.Types.ColorFix:
                    {
                        Client.CoatColorRule.ColorFixing(pMsg.ID);
                        break;
                    }
                case MsgCoatStorage.Types.STORAGE_COLOR_FIXING2:
                    {
                        Client.CoatColorRule.ColorFixing2(pMsg.ID, pMsg.Time);
                        break;
                    }
                case MsgCoatStorage.Types.STORAGE_EQUIP_COLOR:
                    {
                        Client.CoatColorRule.EquipColor(pMsg.ID, pMsg.Time);
                        break;
                    }



                default: MyConsole.WriteLine("Unknown Action Type [" + (uint)pMsg.ActionID + "] MsgCoatStorage."); break;
            }
            Database.TitleStorage.CheckUpUser(Client, stream);
        }

    }
}
