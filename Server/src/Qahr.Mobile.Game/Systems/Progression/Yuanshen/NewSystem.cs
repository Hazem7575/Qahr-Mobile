using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile
{
    public class MsgItemUsuagePacket2
    {
        public static void Process(Client.GameClient client, MsgItemUsuagePacket.ItemUsuageID action, uint id, ulong dwParam, uint timestamp, uint dwParam2, uint PokerPot, uint dwparam4, uint dwparam5, List<uint> args)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (action == MsgItemUsuagePacket.ItemUsuageID.AddOrRemoveEonspirit)
                {
                    switch (dwParam)
                    {
                        case 0://Equip => Add
                            {
                                MsgGameItem Item;
                                if (client.Inventory.TryGetItem(id, out Item))
                                {
                                    Item.EonspiritActived = 0;
                                    Item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritInventory;
                                    client.Inventory.ClientItems.Remove(Item.UID);
                                    client.EonspiritSystem.Add(Item.UID, Item);
                                    Item.Mode = Role.Flags.ItemMode.Update;
                                    Item.Send(client, stream);
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, PokerPot, dwparam4, 0, args));
                                }
                                break;
                            }
                        case 1://Withdraw => Remove
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    MsgGameItem item = null;
                                    if (client.EonspiritSystem.TryGetValue(id, out item))
                                    {
                                        if (Database.ItemType.EonspiritItem.Contains(item.ITEM_ID))
                                        {
                                            item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.Inventory;
                                            client.Inventory.ClientItems.Add(item.UID, item);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            client.EonspiritSystem.Remove(item.UID);
                                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, PokerPot, dwparam4, 0, args));
                                        }
                                    }
                                }
                                else
                                {
                                    client.CreateBoxDialog("You Don't Have Much Space in You Invetory Try Again When you Have one Space");
                                }
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Action In Eonspirit System Equip Not Found Action Missed is => " + dwParam);
                                break;
                            }
                    }
                }
                if (action == MsgItemUsuagePacket.ItemUsuageID.EquipEonspirit)//89
                {
                    MsgGameItem Item;
                    if (client.EonspiritSystem.TryGetValue(id, out Item))
                    {
                        switch (dwParam)
                        {
                            case (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive:
                                {
                                    var item = client.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive;
                                        item.EonspiritActived = 0;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UnEquipEonspirit, item.UID, 255, 0, 0, 0, 0, 0,null));
                                        item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritInventory;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                    Item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive;
                                    Item.EonspiritActived = PokerPot;
                                    Item.Mode = Role.Flags.ItemMode.Update;
                                    Item.Send(client, stream);
                                    client.Player.EonspiritLevel = 0;
                                    client.Player.EonspiritCurrentEnergy = 0;
                                    MsgYuanshen.UpdateEnergy(client, client.Player.EonspiritLevel);
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, PokerPot, dwparam4, 0, args));
                                    client.Player.EonspiritLevel = Item.ITEM_ID % 100;
                                    MsgYuanshen.UpdateSpells(client);
                                    break;
                                }
                            default:
                                {
                                    uint Exp = 0;
                                    var item = client.EonspiritSystem.Values.Where(p => (p.ITEM_ID / 1000) == (Item.ITEM_ID / 1000) && p.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritInventory;
                                        item.EonspiritActived = 0;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UnEquipEonspirit, item.UID, 255, 0, 0, 0, 0, 0,null));
                                    }
                                    Item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive;
                                    Item.EonspiritActived = PokerPot;
                                    Item.Mode = Role.Flags.ItemMode.Update;
                                    Item.Send(client, stream);
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, PokerPot, dwparam4, 0, args));
                                    foreach (var msg_item in client.EonspiritSystem.Values)
                                    {
                                        var Info = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == msg_item.ITEM_ID % 100 && i.TypeLevel == msg_item.EonspiritPercentage);
                                        if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive || msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                                        {
                                            Exp += Info.Rating;
                                            client.Player.EonspiritPrestrige = Info.Rating;
                                        }
                                    }
                                    if (Exp > 0)
                                    {
                                        var entry = new Database.YuanshenRank.Entry()
                                        {
                                            Type = Database.YuanshenRank.Type.Overall_EonSpirit,
                                            TotalPoints = Exp,
                                            UID = client.Player.UID,
                                            Name = client.Player.Name,
                                            Level = (byte)client.Player.Level,
                                            Class = client.Player.Class,
                                            Mesh = client.Player.Mesh,
                                        };
                                        entry.AddInfo(client);
                                        Database.YuanshenRank.Ranks[Database.YuanshenRank.Type.Overall_EonSpirit].UpdateItem(entry);
                                    }
                                    else
                                    {
                                        Database.YuanshenRank.Remove(client.Player.UID);
                                    }
                                    break;
                                }
                        }
                    }
                }
                if (action == MsgItemUsuagePacket.ItemUsuageID.ActivateEonspirit)
                {
                    MsgGameItem Item;
                    if (client.EonspiritSystem.TryGetValue(id, out Item))
                    {
                        var item = client.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault();
                        if (item != null)
                        {
                            item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive;
                            item.EonspiritActived = (uint)PokerPot;
                            item.Mode = Role.Flags.ItemMode.Update;
                            item.Send(client, stream);
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UnEquipEonspirit, Item.UID, 255, 0, 0, 0, 0,0, null));
                        }
                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.EquipEonspirit, Item.UID, 255, 0, 0, 0, 0, 0,null));
                        client.Player.EonspiritLevel = 0;
                        client.Player.EonspiritCurrentEnergy = 0;
                        MsgYuanshen.UpdateEnergy(client, client.Player.EonspiritLevel);
                        Item.Position = (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive;
                        Item.EonspiritActived = (uint)dwParam;
                        Item.Mode = Role.Flags.ItemMode.Update;
                        Item.Send(client, stream);
                        client.Player.EonspiritLevel = Item.ITEM_ID % 100;
                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.EquipEonspirit, Item.UID, (uint)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive, 0, 0, (uint)dwParam, 0,0, null));
                        if (item != null)
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.EquipEonspirit, item.UID, (uint)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive, 0, 0, PokerPot, 0, 0,null));
                        MsgYuanshen.UpdateSpells(client);
                    }
                }
            }
        }
    }
}