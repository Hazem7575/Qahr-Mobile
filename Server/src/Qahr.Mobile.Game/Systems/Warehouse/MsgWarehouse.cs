using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetWarehouse(this ServerSockets.Packet stream, out uint NpcID, out MsgWarehouse.DepositActionID Action, out uint ItemUID)
        {
            NpcID = stream.ReadUInt32();
            uint unknow = stream.ReadUInt32();
            Action = (MsgWarehouse.DepositActionID)stream.ReadUInt16();
            uint file_size = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();


        }

        public static unsafe ServerSockets.Packet WarehouseCreate(this ServerSockets.Packet stream, uint NpcID, MsgWarehouse.DepositActionID Action, uint ItemUID, int File_Size, int count)
        {
            stream.InitWriter();
            stream.Write(NpcID);
            stream.Write((uint)0);
            stream.Write((uint)Action);
            stream.Write((ushort)File_Size);
            stream.Write(ItemUID);
            stream.Write((ushort)count);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemWarehouse(this ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
            stream.Write(item.UID);//24
            stream.Write(item.ITEM_ID);//28
            stream.Write((byte)0);//32
            stream.Write((byte)item.SocketOne);//33
            stream.Write((byte)item.SocketTwo);//34
            stream.Write((uint)item.SpellID);//35
            stream.Write((ushort)item.Effect);//39
            stream.Write(item.Plus);//41
            stream.Write(item.Bless);//42
            stream.Write(item.Bound);//43
            stream.Write(item.Enchant);//44
            stream.Write((byte)item.ProgresGreen);//45
            stream.Write((uint)item.Effect);//46
            stream.Write((byte)item.Locked);//50
            stream.Write((byte)0);//51
            stream.Write(item.SocketProgress);//52
            stream.Write(item.PlusProgress);//56
            stream.Write((uint)item.TimeLeftInMinutes);//60
            stream.Write((uint)item.RemainingTime);//64
            stream.Write(item.StackSize);//68
            stream.Write(item.Purification.PurificationItemID);//70
            stream.Write((ushort)0);//74
            stream.Write(item.PerfectionLevel);//76
            stream.Write(item.PerfectionProgress);//80
            stream.Write(item.OwnerUID);//84
            stream.Write(item.OwnerName, 32);//88
            stream.Write(item.Signature, 60);//120
            stream.Write(item.RuneEXP);//180
            for (int i = 0; i < item.RelicAttributes.Length; i++)
                stream.Write(item.RelicAttributes[i]);//188 //192 //196 //200 
            stream.Write(item.AnimaItemID);//204
            stream.ZeroFill(4);///208          
            stream.Write(item.MythSoulID);//212            
            stream.Write(item.MythSoulProgress);//216 
            stream.Write((uint)0);//220
            stream.Write((uint)0);//224
            stream.Write((uint)0);//228
            stream.Write((uint)item.Mutacion);//232
            stream.Write(item.UpsurgeID);//236
            stream.Write(0);//240
            stream.Write(0);//244
            stream.Write(item.YuanshenTime);// 
            stream.Write((uint)0);//220
            stream.Write((uint)0);//224
            stream.Write((uint)0);//228
            return stream;
        }
        public static unsafe ServerSockets.Packet FinalizeWarehouse(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgPackage);
            return stream;
        }
    }
    public class MsgWarehouse
    {
        public enum DepositActionID : ushort
        {
            Show = 2560,
            DepositItem = 2561,
            WithdrawItem = 2562,
            DepositItem2 = 2565,
            Show_WH_House = 5120,
            DepositItem_WH_House = 5121,
            WithdrawItem_WH_House = 5122,

            ShashShow = 7680,
            ShashDepositItem = 7681,
            ShashWithdrawItem = 7682,

            ShowInventorySash = 10240,
            InventorySashDepositItem = 10241,
            InventorySashWithdrawItem = 10242,
        }



        [PacketAttribute(GamePackets.MsgPackage)]
        public unsafe static void HandlerWarehause(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.IsCheckedPass)
                return;//cheater
            if (client.Player.HitPoints == 0) { client.SendSysMesage("You are dead.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true); return; };
            uint NpcID;
            MsgWarehouse.DepositActionID Action;
            uint ItemUID;

            
            stream.GetWarehouse(out NpcID, out Action, out ItemUID);
            NpcID = global::Qahr.Mobile.Phone.MobileVipMenu.MapWarehouse(client, NpcID);// QahrMobile: VIP remote warehouse (MobileVipMenu.cs)

            switch (Action)
            {
                case DepositActionID.InventorySashDepositItem:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (item.ITEM_ID == 711624)
                                {
                                    client.CreateBoxDialog("You cannot deposit Arrest Tokens.");
                                    break;
                                }
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.DepositItem_WH_House:
                case DepositActionID.DepositItem:
                    {

                        // Block system banks (e.g. RelicFuse 215, JailCPAdmin 1000): the house path accepts any
                        // NpcID, which let players pull items out of internal banks they should never reach.
                        if (NpcID == (uint)MsgNpc.NpcID.RelicFuse || NpcID == (uint)MsgNpc.NpcID.JailCPAdmin)
                            break;
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID || client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (item.ITEM_ID == 711624)
                                {
                                    client.CreateBoxDialog("You cannot deposit Arrest Tokens.");
                                    break;
                                }
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.ShashDepositItem:
                    {
                        // Can't deposit the sash into itself (L5): it would vanish from the bag.
                        if (ItemUID == NpcID)
                            break;
                        MsgGameItem Shash;
                        if (client.Inventory.TryGetItem(NpcID, out Shash))
                        {
                            if (Database.ItemType.IsSash(Shash.ITEM_ID))
                            {
                                MsgGameItem item;
                                if (client.Inventory.TryGetItem(ItemUID, out item))
                                {
                                    if (item.Locked > 0)
                                    {
                                        client.SendSysMesage("This item is locked and can't be stored.");
                                        break;
                                    }
                                    byte ShashCount = Database.ItemType.GetSashCounts(Shash.ITEM_ID);
                                    if (Shash.Deposite.Count < ShashCount)
                                    {
                                        // Take the item OUT of the bag FIRST (Removefull so a stacked item leaves
                                        // entirely); otherwise a stack of N left N-1 copies in the bag -> dupe.
                                        if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true))
                                            break;
                                        if (client.Inventory.ClientItems.ContainsKey(item.UID))
                                            break;

                                        if (Shash.Deposite.TryAdd(item.UID, item))
                                        {
                                            stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                            stream.AddItemWarehouse(item);

                                            client.Send(stream.FinalizeWarehouse());

                                            item.SendItemExtra(client, stream);
                                            item.SendItemLocked(client, stream);
                                        }
                                        else
                                        {
                                            // Couldn't store it: give it back so it isn't lost.
                                            client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                case DepositActionID.ShashShow:
                    {
                        MsgGameItem Shash;
                        if (client.Inventory.TryGetItem(NpcID, out Shash))
                        {

                            if (Database.ItemType.IsSash(Shash.ITEM_ID))
                            {


                                stream.WarehouseCreate(NpcID, Action, 0, Database.ItemType.GetSashCounts(Shash.ITEM_ID), Shash.Deposite.Count);

                                foreach (var item in Shash.Deposite.Values)
                                    stream.AddItemWarehouse(item);

                                client.Send(stream.FinalizeWarehouse());



                                Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();

                                var Array = Shash.Deposite.Values.ToArray();
                                for (byte x = 0; x < Array.Length; x++)
                                {
                                    var item = Array[x];
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



                                if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                                    client.Send(itemExtra.CreateArray(stream));
                            }
                        }

                        break;
                    }
                case DepositActionID.ShowInventorySash:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            client.Warehouse.Show(NpcID, Action, stream);
                        }
                        break;
                    }
                case DepositActionID.Show_WH_House:
                case DepositActionID.Show:
                    {
                        if (NpcID == (uint)MsgNpc.NpcID.RelicFuse || NpcID == (uint)MsgNpc.NpcID.JailCPAdmin)
                            break;
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            client.Warehouse.Show(NpcID, Action, stream);
                        }
                        break;
                    }
                case DepositActionID.ShashWithdrawItem:
                    {
                        MsgGameItem Shash;
                        if (client.Inventory.TryGetItem(NpcID, out Shash))
                        {
                            if (Database.ItemType.IsSash(Shash.ITEM_ID))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    MsgGameItem item;
                                    if (Shash.Deposite.TryRemove(ItemUID, out item))
                                    {
                                        item.WH_ID = 0;
                                        item.Position = 0;
                                        client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);

                                        stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 1);

                                        client.Send(stream.FinalizeWarehouse());
                                    }
                                }
                                else
                                {
#if Arabic
                                         client.SendSysMesage("Your Inventory Is Full!");
#else
                                    client.SendSysMesage("Your Inventory Is Full!");
#endif

                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.InventorySashWithdrawItem:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID, stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
                case DepositActionID.WithdrawItem_WH_House:
                case DepositActionID.WithdrawItem:
                    {
                        // Block system banks (RelicFuse 215 / JailCPAdmin 1000) from the generic withdraw path;
                        // this is the core of the Relic-Fuse dupe (pulling the 3 relics back out).
                        if (NpcID == (uint)MsgNpc.NpcID.RelicFuse || NpcID == (uint)MsgNpc.NpcID.JailCPAdmin)
                            break;
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID, stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
            }
        }


    }
}
