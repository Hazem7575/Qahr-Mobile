using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: MsgCoatStorage (2233 / mobile 10220 CMsgCoatStoragePB) with the mobile wardrobe numbering.
    /// The phone keeps garments, weapon accessories and mount armor in its wardrobe (CWrapPackageMgr) and only wears
    /// them from there; its action numbers are not the PC ones (MsgCoatStorage.Types):
    ///   mobile -> server (CWrapPackageMgr::Submit*):
    ///     1 {ID=uid}              check in from the bag (SubmitCheckInItem; the bag "Equip" button of a garment/accessory)
    ///     2 {ID=uid}              take back to the bag (CheckOutItem)
    ///     5 {ID=uid, dwpram2=pos} wear (SubmitEquipWrap; pos 9 garment, 15/16 weapon accessory, 17 mount armor)
    ///     6 {ID=uid, dwpram2=pos} take off (SubmitUnEquipWrap)
    ///   server -> mobile (CMsgCoatStorage::Process):
    ///     0 item list, 1 PutItemToWrapPackage(ID) (the client moves the item out of its own bag, so no RemoveInventory
    ///     may come first), 2 DelWrapItem(dwpram2=type, ID) + tip, 5 EquipWrap(ID, -, dwpram3=pos), 6 UnEquipWrap(ID),
    ///     7 update items, 8 DelWrapItem (expired), 9 other player's wardrobe, 10 exchange tip.
    /// The PC handler read 5 as TakeOff and had no 6, and only stored garments/mount armor, so wearing never happened.
    /// </summary>
    public static class MobileWardrobe
    {
        public const uint CheckIn = 1, CheckOut = 2, Wear = 5, TakeOff = 6;

        /// <summary>True when the packet was a mobile wardrobe action and has been handled here.</summary>
        public static bool Handle(Client.GameClient client, ServerSockets.Packet stream, MsgCoatStorage.CoatStorage msg)
        {
            if (client == null || client.Player == null || !MobileClient.Is(client))
                return false;
            switch (msg.ActionID)
            {
                case CheckIn: DoCheckIn(client, stream, msg.ID); return true;
                case CheckOut: DoCheckOut(client, stream, msg.ID); return true;
                case Wear: DoWear(client, stream, msg.ID, msg.dwpram2); return true;
                case TakeOff: DoTakeOff(client, stream, msg.ID); return true;
            }
            return false;
        }

        public static bool IsWardrobePosition(ushort pos)
        {
            return pos == (ushort)Role.Flags.ConquerItem.Garment || pos == (ushort)Role.Flags.ConquerItem.SteedMount
                || pos == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory;
        }

        private static bool IsAccessoryPosition(ushort pos)
        {
            return pos == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory;
        }

        private static bool Busy(Client.GameClient client)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return true;
            }
            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return true;
            }
            return false;
        }

        private static void Reply(Client.GameClient client, ServerSockets.Packet stream, uint action, uint uid, uint param2, uint param3)
        {
            var store = new MsgCoatStorage.CoatStorage();
            store.ActionID = action;
            store.ID = uid;
            store.dwpram2 = param2;
            store.dwpram3 = param3;
            client.Send(stream.CreateCoatStorage(store));
        }

        private static void DoCheckIn(Client.GameClient client, ServerSockets.Packet stream, uint uid)
        {
            if (Busy(client))
                return;
            MsgGameItem item;
            if (!client.Inventory.TryGetItem(uid, out item))
                return;
            ushort pos = Database.ItemType.ItemPosition(item.ITEM_ID);
            if (!IsWardrobePosition(pos))
            {
                client.SendSysMesage("This item can't be stored in the wardrobe.");
                return;
            }
            if (item.Locked > 0)
            {
                client.SendSysMesage("This item is locked and can't be stored.");
                return;
            }
            if (client.MyWardrobe.Contain(item.ITEM_ID))
            {
                client.SendSysMesage("You already have this item in your wardrobe.");
                return;
            }
            // Take it out of the bag first (no dupe). No RemoveInventory to the phone: CHero::PutItemToWrapPackage
            // looks the item up in its own bag and moves it, so removing it first would lose it until relogin.
            MsgGameItem removed;
            if (!client.Inventory.ClientItems.TryRemove(item.UID, out removed))
                return;
            item.Position = 0;
            if (!client.MyWardrobe.AddItem(item))
            {
                client.Inventory.ClientItems.TryAdd(item.UID, item);
                return;
            }
            Reply(client, stream, CheckIn, item.UID, 0, 0);
        }

        private static void DoCheckOut(Client.GameClient client, ServerSockets.Packet stream, uint uid)
        {
            if (Busy(client))
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                client.CreateBoxDialog("Please make 1 more space in your inventory.");
                return;
            }
            MsgGameItem item;
            if (!client.MyWardrobe.TryGetItem(uid, out item))
                return;
            if (client.Equipment.Contains(uid))
            {
                if (!TakeOffWorn(client, stream, item))
                    return;
            }
            if (!client.MyWardrobe.RemoveItem(uid, out item))
                return;
            client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
            Reply(client, stream, CheckOut, uid, item.ITEM_ID, 0);
        }

        private static void DoWear(Client.GameClient client, ServerSockets.Packet stream, uint uid, uint wanted)
        {
            if (!client.Player.Alive || Busy(client))
                return;
            MsgGameItem item;
            if (!client.MyWardrobe.TryGetItem(uid, out item))
                return;
            if (!MsgCoatStorage.CheckGender(client, item.ITEM_ID))
            {
                client.SendSysMesage("This item doesn't fit your gender.");
                return;
            }
            ushort type = Database.ItemType.ItemPosition(item.ITEM_ID);
            ushort pos;
            if (IsAccessoryPosition(type))
            {
                // ItemPosition says 15 for every accessory; the phone sends the hand it chose (15 right / 16 left)
                pos = IsAccessoryPosition((ushort)wanted) ? (ushort)wanted : type;
                if (!Database.ItemType.Equipable(item, client))
                {
                    client.SendSysMesage("You can't wear this accessory.");
                    return;
                }
            }
            else if (type == (ushort)Role.Flags.ConquerItem.Garment || type == (ushort)Role.Flags.ConquerItem.SteedMount)
                pos = type;
            else
                return;
            if (pos == (ushort)Role.Flags.ConquerItem.Garment && client.Player.SpecialGarment != 0)
            {
                client.SendSysMesage("Item can't be unequiped during the event.");
                return;
            }
            if (item.Position == pos && client.Equipment.Contains(uid))
            {
                Reply(client, stream, Wear, uid, item.ITEM_ID, pos);
                return;
            }
            // the same item worn in the other hand moves over
            if (item.Position != 0 && client.Equipment.Contains(uid))
            {
                if (!TakeOffWorn(client, stream, item))
                    return;
            }
            MsgGameItem old;
            if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)pos, out old))
            {
                if (client.MyWardrobe.ContainUID(old.UID))
                {
                    if (!TakeOffWorn(client, stream, old))
                        return;
                }
                else if (!client.Equipment.Remove((Role.Flags.ConquerItem)pos, stream))
                    return;// bag full (Remove told the player)
            }
            item.Position = pos;
            client.Equipment.Add(item, stream);
            item.Mode = Role.Flags.ItemMode.Update;
            item.Send(client, stream);
            Reply(client, stream, Wear, uid, item.ITEM_ID, pos);
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
        }

        private static void DoTakeOff(Client.GameClient client, ServerSockets.Packet stream, uint uid)
        {
            if (Busy(client))
                return;
            MsgGameItem item;
            if (!client.Equipment.TryGetValue(uid, out item))
                return;
            if (!client.MyWardrobe.ContainUID(uid))
            {
                // worn before it was ever in the wardrobe: the normal path puts it in the bag
                ushort p = item.Position;
                if (client.Equipment.Remove((Role.Flags.ConquerItem)p, stream))
                {
                    Reply(client, stream, TakeOff, uid, item.ITEM_ID, p);
                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                }
                return;
            }
            if (TakeOffWorn(client, stream, item))
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
        }

        /// <summary>Takes a worn wardrobe item off (it stays in the wardrobe) and tells the phone (6 UnEquipWrap).</summary>
        private static bool TakeOffWorn(Client.GameClient client, ServerSockets.Packet stream, MsgGameItem item)
        {
            if (item.Position == (ushort)Role.Flags.ConquerItem.Garment && client.Player.SpecialGarment != 0)
            {
                client.SendSysMesage("Item can't be unequiped during the event.");
                return false;
            }
            ushort p = item.Position;
            MsgGameItem removed;
            if (!client.Equipment.ClientItems.TryRemove(item.UID, out removed))
                return false;
            item.Position = 0;
            Reply(client, stream, TakeOff, item.UID, item.ITEM_ID, p);
            return true;
        }
    }
}
