using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetUpdateItem(this ServerSockets.Packet stream, out MsgUpdateItem.ActionType Action, out uint ItemUID, out List<uint> items)
        {
            Action = (MsgUpdateItem.ActionType)stream.ReadUInt8();
            byte FullCount = stream.ReadUInt8();
            ushort padding = stream.ReadUInt16();
            ItemUID = stream.ReadUInt32();
            uint Count = stream.ReadUInt32();

            items = new List<uint>();
            if (Action != MsgUpdateItem.ActionType.UpdateLevel && Action != MsgUpdateItem.ActionType.UpdateQuality)
            {

                items.Add(Count);
                for (ushort x = 0; x < FullCount - 2; x++)
                {
                    items.Add(stream.ReadUInt32());
                }
            }
            else
            {
                // Count جاي من العميل: عدّاد ushort قدام قيمة كبيرة كان بيعمل loop لا نهائي
                uint safeCount = Math.Min(Count, 200u);
                for (uint x = 0; x < safeCount; x++)
                    items.Add(stream.ReadUInt32());
            }
        }
    }


    public struct MsgUpdateItem
    {
        public enum ActionType : byte
        {
            Plus = 0,
            CurrentSteed = 2,
            NewSteed = 3,
            ChanceUpgrade = 4,
            ChanceUpgradeSteed = 5,
            UpdateLevel = 6,
            UpdateQuality = 7,
            UpdateRate = 8,
            UpdateRateSteed = 9,
        }
        [PacketAttribute(GamePackets.MsgDataArray)]
        public unsafe static void Compose(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgUpdateItem.ActionType Action; uint ItemUID;
            List<uint> ItemsUIDS;

            uint dwParam1 = 2;

            stream.GetUpdateItem(out Action, out ItemUID, out ItemsUIDS);

            switch (Action)
            {
                case ActionType.UpdateLevel:
                    {
                        if (ItemsUIDS.Count == 0)
                            break;

                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            if (Position != (ushort)Role.Flags.ConquerItem.Head
                            && Position != (ushort)Role.Flags.ConquerItem.Necklace
                            && Position != (ushort)Role.Flags.ConquerItem.Armor
                            && Position != (ushort)Role.Flags.ConquerItem.RightWeapon
                            && Position != (ushort)Role.Flags.ConquerItem.LeftWeapon
                            && Position != (ushort)Role.Flags.ConquerItem.Ring
                            && Position != (ushort)Role.Flags.ConquerItem.Boots
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteHead
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteNecklace
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteArmor
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteRing
                            && Position != (ushort)Role.Flags.ConquerItem.AleternanteBoots)
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                return;
                            }
                            if (Database.ItemType.isRune(DataItem.ITEM_ID))
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To Bug Rune [Update Level] .", ConsoleColor.White);
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            if (DataItem.ITEM_ID >= 2100025 && DataItem.ITEM_ID <= 2100125 || DataItem.ITEM_ID == 2100165 || DataItem.ITEM_ID == 2100175 || DataItem.ITEM_ID == 2100185 || DataItem.ITEM_ID == 2100195 || DataItem.ITEM_ID == 2100205 || DataItem.ITEM_ID == 2100245 || DataItem.ITEM_ID == 2168735 || DataItem.ITEM_ID == 2168896 || DataItem.ITEM_ID == 2168725 || DataItem.ITEM_ID == 2168715 || DataItem.ITEM_ID == 2168705)
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To Bug Cup [Update Level] .", ConsoleColor.White); ;
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            //anti proxy --------------------
                            if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                            {
#if Arabic
                                   client.SendSysMesage("This item's level cannot be upgraded anymore.");
#else
                                client.SendSysMesage("This item's level cannot be upgraded anymore.");
#endif

                                return;
                            }
                            //------------------------
                            MsgGameItem itemuse;
                            if (client.Inventory.ClientItems.TryGetValue(ItemsUIDS[0], out itemuse))
                            {
                                if (itemuse.ITEM_ID == Database.ItemType.DragonBall)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                    {
                                        bool succesed = false;
                                        uint nextItemId = Pool.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);

                                        if ((DBItem.Level >= 140 && Database.ItemType.Equipable(nextItemId, client) == false)
                                              && (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeapon
                                              || Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon))
                                        {
                                            client.CreateBoxDialog("You can`t update this item.");
                                        }
                                        else
                                        {
                                            dwParam1 = 1;
                                            uint oldid = DataItem.ITEM_ID;
                                            DataItem.ITEM_ID = Pool.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(client, stream);//.Update(itemuse, Role.Instance.AddMode.REMOVE,stream);
                                            if (succesed && oldid != DataItem.ITEM_ID)
                                            {
                                                client.Inventory.Update(itemuse, Role.Instance.AddMode.REMOVE, stream);
                                            }
                                            else
                                            {
#if Arabic
                                                client.SendSysMesage("This item's level cannot be upgraded anymore.");
#else
                                                client.SendSysMesage("This item's level cannot be upgraded anymore.");
#endif
                                            }
                                        }
                                    }
                                }
                                else if (itemuse.ITEM_ID == Database.ItemType.Meteor || itemuse.ITEM_ID == Database.ItemType.MeteorScroll)
                                {
                                    if (client.Inventory.CheckMeteors((byte)ItemsUIDS.Count, false, stream))
                                    {
                                        Database.ItemType.DBItem DBItem;
                                        if (Pool.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                        {
                                            bool succesed = false;

                                            uint nextItemId = Pool.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);

                                            if ((DBItem.Level >= 140 && Database.ItemType.Equipable(nextItemId, client) == false)
                                             && (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeapon
                                             || Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon))
                                            {
                                                client.CreateBoxDialog("You can`t update this item.");
                                            }
                                            else
                                            {
                                                if (Database.ItemType.UpItemMeteors(DataItem.ITEM_ID, (uint)ItemsUIDS.Count))
                                                {
                                                    client.HeroRewards.AddGoal(205);

                                                    dwParam1 = 1;
                                                    DataItem.ITEM_ID = Pool.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);

                                                }
#if TEST
                                            Console.WriteLine("Update Item " + DBItem.Name + " id = " + Database.ItemType.GetLevel(DataItem.ITEM_ID));
#endif

                                                client.Inventory.CheckMeteors((byte)ItemsUIDS.Count, true, stream);
                                            }
                                        }
                                    }
                                }
                                if (DataItem.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpgradeMeteor, ItemUID, dwParam1, 0, 0, 0, 0));

                        }
                        break;
                    }
                case ActionType.UpdateQuality:
                    {


                        if (ItemsUIDS.Count == 0)
                            break;
                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            if (Position != (ushort)Role.Flags.ConquerItem.Head
                                  && Position != (ushort)Role.Flags.ConquerItem.Necklace
                                  && Position != (ushort)Role.Flags.ConquerItem.Armor
                                  && Position != (ushort)Role.Flags.ConquerItem.RightWeapon
                                  && Position != (ushort)Role.Flags.ConquerItem.LeftWeapon
                                  && Position != (ushort)Role.Flags.ConquerItem.Ring
                                  && Position != (ushort)Role.Flags.ConquerItem.Boots
                                  && Position != (ushort)Role.Flags.ConquerItem.Fan
                                 && Position != (ushort)Role.Flags.ConquerItem.Tower
                                 && Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                                 && Position != (ushort)Role.Flags.ConquerItem.Wing
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteHead
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteNecklace
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteArmor
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteRing
                                  && Position != (ushort)Role.Flags.ConquerItem.AleternanteBoots)
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                return;
                            }
                            if (Database.ItemType.isRune(DataItem.ITEM_ID))
                            {
                                MyConsole.WriteLine("Player " + client.Player.Name + " Banned To Bug Rune [Update Quality].", ConsoleColor.White);
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            if (DataItem.ITEM_ID >= 2100025 && DataItem.ITEM_ID <= 2100125 || DataItem.ITEM_ID == 2100165 || DataItem.ITEM_ID == 2100175 || DataItem.ITEM_ID == 2100185 || DataItem.ITEM_ID == 2100195 || DataItem.ITEM_ID == 2100205 || DataItem.ITEM_ID == 2100245 || DataItem.ITEM_ID == 2168735 || DataItem.ITEM_ID == 2168896 || DataItem.ITEM_ID == 2168725 || DataItem.ITEM_ID == 2168715 || DataItem.ITEM_ID == 2168705)
                            {
                                MyConsole.WriteLine("Player " + client.Player.Name + " Banned To Bug Cup [Update Quality].", ConsoleColor.White);
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            if (Position != (ushort)Role.Flags.ConquerItem.Fan
                                && Position != (ushort)Role.Flags.ConquerItem.Tower && Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                            {
                                if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                {

                                    client.SendSysMesage("This item's Quality cannot be upgraded anymore.");

                                    return;
                                }
                            }
                            //------------------------
                            Queue<MsgGameItem> UseItems = new Queue<MsgGameItem>();
                            bool EmbedUpdate = false;
                            // Dedup the client-supplied UID list and exclude the target item itself.
                            // Without this, the same material UID could be listed many times: each copy added
                            // progress/points while only the first REMOVE actually consumed the material.
                            HashSet<uint> SeenUIDs = new HashSet<uint>();
                            for (int x = 0; x < ItemsUIDS.Count; x++)
                            {
                                uint materialUID = ItemsUIDS[x];
                                // A duplicate UID, or the target item used as its own material, is a cheat packet.
                                if (materialUID == ItemUID || !SeenUIDs.Add(materialUID))
                                {
                                    EmbedUpdate = false;
                                    break;
                                }
                                MsgGameItem itemuse;
                                if (client.Inventory.ClientItems.TryGetValue(materialUID, out itemuse))
                                {
                                    UseItems.Enqueue(itemuse);
                                    EmbedUpdate = true;
                                }
                                else { EmbedUpdate = false; break; }
                            }
                            if (EmbedUpdate && UseItems.Count > 0)
                            {
                                var CheckItem = UseItems.Dequeue();
                                if (CheckItem.ITEM_ID == Database.ItemType.DragonBall)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                    {
                                        if (Database.ItemType.UpQualityDB(DataItem.ITEM_ID, (uint)(UseItems.Count + 1)))
                                        {
                                            dwParam1 = 1;
                                            if (DataItem.ITEM_ID % 10 < 5)
                                                DataItem.ITEM_ID += 5 - DataItem.ITEM_ID % 10;
                                            DataItem.ITEM_ID++;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(client, stream);
                                        }


                                        client.Inventory.Update(CheckItem, Role.Instance.AddMode.REMOVE, stream);
                                        while (UseItems.Count > 0)
                                            client.Inventory.Update(UseItems.Dequeue(), Role.Instance.AddMode.REMOVE, stream);
                                    }
                                }
                            }
                            if (DataItem.Position != 0)
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }

                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpgradeDragonball, ItemUID, dwParam1, 0, 0, 0, 0, 0));


                        break;
                    }
                default:
                    {

                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            //anti proxy --------------------
                            if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Steed
                                && Position != (ushort)Role.Flags.ConquerItem.Tower && Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                            {
                                if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                {
                                    client.SendSysMesage("This item's Plus cannot be upgraded anymore.");
                                    return;
                                }
                            }
                            if (Database.ItemType.isRune(DataItem.ITEM_ID))
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To Bug Rune Plus [Update Quality] .", ConsoleColor.White);
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            if (DataItem.ITEM_ID >= 2100025 && DataItem.ITEM_ID <= 2100125 || DataItem.ITEM_ID == 2100165 || DataItem.ITEM_ID == 2100175 || DataItem.ITEM_ID == 2100185 || DataItem.ITEM_ID == 2100195 || DataItem.ITEM_ID == 2100205 || DataItem.ITEM_ID == 2100245 || DataItem.ITEM_ID == 2168735 || DataItem.ITEM_ID == 2168896 || DataItem.ITEM_ID == 2168725 || DataItem.ITEM_ID == 2168715 || DataItem.ITEM_ID == 2168705)
                            {
                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To Bug Cup [Update Quality] .", ConsoleColor.White);
                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                                client.Socket.Disconnect();
                                break;
                            }
                            if (Action == ActionType.UpdateRate)
                            {
                                if (DataItem.Plus < 12 && DataItem.PlusProgress != 0)
                                {

                                    byte oldplus = DataItem.Plus;

                                    double percent = (double)Database.ItemType.ComposePlusPoints(DataItem.Plus) / (double)DataItem.PlusProgress;

                                    if (Role.Core.Rate(percent))
                                    {
                                        DataItem.PlusProgress++;
                                    }
                                    else
                                    {
                                        DataItem.PlusProgress -= DataItem.PlusProgress / 10;
                                    }

                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                    {
                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

                                    }
                                }
                                break;
                            }
                            if (Action == ActionType.ChanceUpgrade)
                            {
                                client.CreateBoxDialog("Sorry, This System Is Closed.");
                                break;
                                if (DataItem.Plus < 12 && DataItem.PlusProgress != 0)
                                {

                                    byte oldplus = DataItem.Plus;

                                    double percent = (double)DataItem.PlusProgress / (double)Database.ItemType.ComposePlusPoints(DataItem.Plus);

                                    if (Role.Core.Rate(percent))
                                    {
                                        DataItem.Plus++;
                                    }


                                    DataItem.PlusProgress = 0;
                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                    {
                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

                                    }
                                }
                                break;
                            }
                            if (Action == ActionType.ChanceUpgradeSteed)
                            {
                                client.CreateBoxDialog("Sorry, This System Is Closed.");
                                break;
                                if (DataItem.Plus < 12 && DataItem.PlusProgress != 0)
                                {

                                    byte oldplus = DataItem.Plus;

                                    double percent = (double)DataItem.PlusProgress / (double)Database.ItemType.ComposePlusPoints(DataItem.Plus);

                                    percent = percent * 100;
                                    if (Role.Core.Rate(percent))
                                    {
                                        DataItem.Plus++;
                                    }


                                    DataItem.PlusProgress = 0;
                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                    {
                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

                                    }
                                }
                                break;
                            }

                            if (Action == ActionType.UpdateRateSteed)
                            {
                                client.CreateBoxDialog("Sorry, This System Is Closed.");
                                break;
                                if (DataItem.Plus < 12 && DataItem.PlusProgress != 0)
                                {

                                    byte oldplus = DataItem.Plus;

                                    double percent = (double)DataItem.PlusProgress / (double)Database.ItemType.ComposePlusPoints(DataItem.Plus);

                                    percent = percent * 100;

                                    percent = percent * 2;
                                    if (Role.Core.Rate(percent))
                                    {
                                        DataItem.PlusProgress += (DataItem.PlusProgress * (uint)percent / 100);
                                    }


                                    DataItem.PlusProgress = 0;
                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                    {
                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

                                    }
                                }
                                break;
                            }
                            Queue<MsgGameItem> UseItems = new Queue<MsgGameItem>();
                            bool EmbedUpdate = false;
                            // Dedup the client-supplied UID list and exclude the target item itself.
                            // Without this, the same material UID could be listed many times: each copy added
                            // progress/points while only the first REMOVE actually consumed the material.
                            HashSet<uint> SeenUIDs = new HashSet<uint>();
                            for (int x = 0; x < ItemsUIDS.Count; x++)
                            {
                                uint materialUID = ItemsUIDS[x];
                                // A duplicate UID, or the target item used as its own material, is a cheat packet.
                                if (materialUID == ItemUID || !SeenUIDs.Add(materialUID))
                                {
                                    EmbedUpdate = false;
                                    break;
                                }
                                MsgGameItem itemuse;
                                if (client.Inventory.ClientItems.TryGetValue(materialUID, out itemuse))
                                {
                                    UseItems.Enqueue(itemuse);
                                    EmbedUpdate = true;
                                }
                                else { EmbedUpdate = false; break; }
                            }
                            if (EmbedUpdate && UseItems.Count > 0)
                            {


                                switch (Action)
                                {
                                    case ActionType.CurrentSteed:
                                        {

                                            if (DataItem.Plus < 12)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    byte oldplus = DataItem.Plus;

                                                    var Stone = UseItems.Dequeue();
                                                    if (Action == ActionType.CurrentSteed)
                                                    {
                                                        uint Pos = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                                        if (Position != Pos)
                                                            return;
                                                    }
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    DataItem.PlusProgress += Stone.PlusProgress;
                                                    while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                    {
                                                        DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                        DataItem.Plus++;
                                                        if (DataItem.Plus == 12)
                                                            DataItem.PlusProgress = 0;
                                                    }
                                                    client.HeroRewards.AddGoal(403);

                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                                    {
#if Arabic
                                                                 client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");
                                                
#else
                                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

#endif
                                                    }

                                                    if (client.Player.MyMentor != null)
                                                    {
                                                        client.Player.MyMentor.Mentor_Blessing += (uint)(Database.ItemType.StonePlusPoints(Stone.Plus) / 100);
                                                        Role.Instance.Associate.Member mee;
                                                        if (client.Player.MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                                                        {
                                                            if (client.Player.MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(client.Player.UID, out mee))
                                                            {
                                                                mee.Blessing += (uint)(Database.ItemType.ComposePlusPoints(Stone.Plus) / 100);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    var Stone = UseItems.Dequeue();

                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;

                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);

                                                }
                                            }
                                            else
                                            {

                                                var Stone = UseItems.Dequeue();
                                                DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                if (DataItem.PlusProgress >= 2000000)
                                                    DataItem.PlusProgress = 2000000;
                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                DataItem.Send(client, stream);
                                            }
                                            break;
                                        }
                                    case ActionType.Plus:
                                        {

                                            if (DataItem.Plus < 12)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    byte oldplus = DataItem.Plus;

                                                    var Stone = UseItems.Dequeue();
                                                    ushort positions = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                                    if (Position == positions
                                                    || Stone.ITEM_ID >= 730001 && Stone.ITEM_ID <= 730008


                                                    || Database.ItemType.IsTrojanEpicWeapon(Stone.ITEM_ID)
                                                    || Database.ItemType.IsTrojanEpicWeapon(DataItem.ITEM_ID)

                                                    || Database.ItemType.IsNinjaEpicWeapon(Stone.ITEM_ID)
                                                    || Database.ItemType.IsNinjaEpicWeapon(DataItem.ITEM_ID)

                                                    || Database.ItemType.IsMonkEpicWeapon(Stone.ITEM_ID)
                                                    || Database.ItemType.IsMonkEpicWeapon(DataItem.ITEM_ID)

                                                    || Database.ItemType.IsPirateEpicWeapon(Stone.ITEM_ID)
                                                    || Database.ItemType.IsPirateEpicWeapon(DataItem.ITEM_ID)
                                                    || Database.ItemType.IsArcherEpicWeapon(Stone.ITEM_ID)
                                                    || Database.ItemType.IsArcherEpicWeapon(DataItem.ITEM_ID))
                                                    {
                                                        #region Protect(EpicWeapon)
                                                        if (Database.ItemType.IsTwoHand(Stone.ITEM_ID) || Database.ItemType.IsTwoHand(DataItem.ITEM_ID))
                                                        {
                                                            if (Database.ItemType.IsTrojanEpicWeapon(Stone.ITEM_ID) || Database.ItemType.IsTrojanEpicWeapon(DataItem.ITEM_ID))
                                                            {
                                                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                                                return;
                                                            }
                                                            else if (Database.ItemType.IsNinjaEpicWeapon(Stone.ITEM_ID) || Database.ItemType.IsNinjaEpicWeapon(DataItem.ITEM_ID))
                                                            {
                                                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                                                return;
                                                            }
                                                            else if (Database.ItemType.IsMonkEpicWeapon(Stone.ITEM_ID) || Database.ItemType.IsMonkEpicWeapon(DataItem.ITEM_ID))
                                                            {
                                                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                                                return;
                                                            }
                                                            else if (Database.ItemType.IsPirateEpicWeapon(Stone.ITEM_ID) || Database.ItemType.IsPirateEpicWeapon(DataItem.ITEM_ID))
                                                            {
                                                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                                                return;
                                                            }
                                                            else if (Database.ItemType.IsArcherEpicWeapon(Stone.ITEM_ID) || Database.ItemType.IsArcherEpicWeapon(DataItem.ITEM_ID))
                                                            {
                                                                MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Cheater .", ConsoleColor.White);
                                                                return;
                                                            }
                                                        }
                                                        #endregion
                                                        DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                        DataItem.PlusProgress += Stone.PlusProgress;
                                                        while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                        {
                                                            DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                            DataItem.Plus++;
                                                            if (DataItem.Plus == 12)
                                                                DataItem.PlusProgress = 0;
                                                        }
                                                        client.HeroRewards.AddGoal(403);

                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                        if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                                        {
#if Arabic
                                                                 client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");
                                                
#else
                                                            client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Pool.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

#endif
                                                        }

                                                        if (client.Player.MyMentor != null)
                                                        {
                                                            client.Player.MyMentor.Mentor_Blessing += (uint)(Database.ItemType.StonePlusPoints(Stone.Plus) / 100);
                                                            Role.Instance.Associate.Member mee;
                                                            if (client.Player.MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                                                            {
                                                                if (client.Player.MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(client.Player.UID, out mee))
                                                                {
                                                                    mee.Blessing += (uint)(Database.ItemType.ComposePlusPoints(Stone.Plus) / 100);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    var Stone = UseItems.Dequeue();
                                                    ushort positions = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                                    if (Position == positions
                                                    || Stone.ITEM_ID >= 730001 && Stone.ITEM_ID <= 730008)
                                                    {
                                                        DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                        if (DataItem.PlusProgress >= 2000000)
                                                            DataItem.PlusProgress = 2000000;

                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                    }

                                                }
                                            }
                                            else
                                            {

                                                var Stone = UseItems.Dequeue();
                                                ushort positions = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                                if (Position == positions
                                                || Stone.ITEM_ID >= 730001 && Stone.ITEM_ID <= 730008)
                                                {
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                    DataItem.Send(client, stream);
                                                }
                                            }

                                            break;
                                        }
                                    case ActionType.NewSteed:
                                        {

                                            while (UseItems.Count > 0)
                                            {
                                                var Stone = UseItems.Dequeue();
                                                if (Action == ActionType.CurrentSteed)
                                                {
                                                    if (Position != Stone.Position)
                                                        return;
                                                }
                                                if (DataItem.Plus < 12)
                                                {
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                    {
                                                        DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                        DataItem.Plus++;
                                                        if (DataItem.Plus == 12)
                                                            DataItem.PlusProgress = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);
                                                }
                                                int color1 = (int)DataItem.SocketProgress;
                                                int color2 = (int)Stone.SocketProgress;

                                                int G1 = color1 & 0xFF;
                                                int G2 = color2 & 0xFF;
                                                int B1 = (color1 >> 8) & 0xFF;
                                                int B2 = (color2 >> 8) & 0xFF;
                                                int R1 = (color1 >> 16) & 0xFF;
                                                int R2 = (color2 >> 16) & 0xFF;
                                                byte ProgresGreen = (byte)((int)Math.Floor(0.9 * G1) + (int)Math.Floor(0.1 * G2) + 1);
                                                byte ProgresBlue = (byte)((int)Math.Floor(0.9 * B1) + (int)Math.Floor(0.1 * B2) + 1);
                                                byte ProgresRed = (byte)((int)Math.Floor(0.9 * R1) + (int)Math.Floor(0.1 * R2) + 1);

                                                DataItem.ProgresGreen = ProgresGreen;
                                                DataItem.Enchant = ProgresBlue;
                                                DataItem.Bless = ProgresRed;

                                                DataItem.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));

                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                            }
                                            break;
                                        }

                                }
                                if (DataItem.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }

                        break;
                    }

            }
        }
    }
}
