using Qahr.Mobile.Client;
using System;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgMailOperation
    {
        public enum Type : byte
        {
            CMsgMailContent = 1,
            Delete = 2,
            ClaimMoney = 3,
            ClaimItem = 4,
            ClaimRecordType = 5,
            DeleteClaim = 6,
            ClaimAll = 7,
        }
        public static void GetMailboxMode(this ServerSockets.Packet stream, out Game.MsgServer.MsgMailOperation.Type Type, out uint ID, out uint count, out ulong[] items)
        {
            Type = (MsgMailOperation.Type)stream.ReadUInt16();//4
            ID = stream.ReadUInt32();//6
            count = stream.ReadUInt32();//10
            items = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = stream.ReadUInt64();//14
            }
        }
        public static ServerSockets.Packet CreateMailboxMode(this ServerSockets.Packet stream, Game.MsgServer.MsgMailOperation.Type Type, uint ID, uint count, ulong[] items)
        {
            stream.InitWriter();
            stream.Write((ushort)Type);
            stream.Write(ID);
            stream.Write(count);
            for (int i = 0; i < count; i++)
            {
                stream.Write(items[i]);
            }
            stream.Finalize(GamePackets.MsgMailOperation);
            return stream;
   
        }
        public static bool Item(uint id, GameClient user)
        {
            // Rewards must not be lost in a full bag: the mail is only cleared when this returns true.
            if (!user.Inventory.HaveSpace((byte)(id == 30 ? 4 : 2)))
            {
                user.SendSysMesage("STR_BAG_FULL@@");
                return false;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                switch (id)
                {
                    case 581477:
                        {

                            user.Inventory.AddItemWitchStack(3314674, 0, 1, stream);
                            return true;

                        }
                    case 581478:
                        {
                            user.Inventory.AddItemWitchStack(3314675, 0, 1, stream);
                            return true;
                        }
                    case 1:
                        {
                            user.Inventory.AddItemWitchStack(3004181, 0, 3, stream);
                            return true;
                        }

                    case 2:
                        {
                            user.Inventory.AddItemWitchStack(3004181, 0, 2, stream);
                            return true;
                        }
                    case 3:
                        {
                            user.Inventory.AddItemWitchStack(3004181, 0, 1, stream);
                            return true;
                        }
                    case 30:
                        {
                            var runes = Pool.ItemsBase.Values.Where(i => Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.YellowRune && i.ID % 10 == 1).ToArray();
                            user.Inventory.Add(stream, runes.ToArray()[Pool.GetRandom.Next(0, runes.Count())].ID, 1, 0);
                            var runess = Pool.ItemsBase.Values.Where(i => Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.BlueRune && i.ID % 10 == 1).ToArray();
                            user.Inventory.Add(stream, runess[Pool.GetRandom.Next(0, runess.Length)].ID, 1, 0);
                            user.Inventory.AddItemWitchStack(4060001, 0, 1000, stream);
                            return true;
                        }
                    case 31:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 900, stream);
                            return true;
                        }
                    case 32:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 800, stream);
                            return true;
                        }
                    case 33:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 34:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 35:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 36:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 37:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 38:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 39:
                        {
                            user.Inventory.AddItemWitchStack(4060001, 0, 500, stream);
                            return true;
                        }
                    case 723911:
                        {
                            user.Inventory.AddItemWitchStack(723911, 0, 1, stream);
                            return true;
                        }


                }
            }
            return false;
        }
        [PacketAttribute(GamePackets.MsgMailOperation)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            Type Type;
            uint ID;
            uint count;
            ulong[] Items;
            stream.GetMailboxMode(out Type, out ID, out count, out Items);
            switch (Type)
            {
                case MsgMailOperation.Type.CMsgMailContent:
                    {
                        PrizeInfo Mail;
                        if (user.PrizeInfo.TryGetValue(ID, out Mail))
                        {
                            CMsgMailContent.Create(user, Mail);
                            user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                            user.Warehouse.SendInboxItem(stream, Mail.itemid, 1000);
                        }
                        break;
                    }
                case MsgMailOperation.Type.Delete:
                    {
                        PrizeInfo Mail;
                        if (user.PrizeInfo.TryGetValue(ID, out Mail))
                        {
                            if (Mail.Money == 0 && Mail.EMoney == 0 && Mail.itemid == 0 && Mail.Flag == 0)
                            {
                                user.PrizeInfo.Remove(Mail.id);
                                user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                            }
                        }
                        break;
                    }
                case MsgMailOperation.Type.ClaimMoney:
                    {
                        PrizeInfo Mail;
                        if (user.PrizeInfo.TryGetValue(ID, out Mail))
                        {
                            if (Mail.Money > 0)
                            {
                                user.Player.Money += (ulong)Mail.Money;
                                user.SendSysMesage("STR_PICK_MONEY@@" + Mail.Money + "@@");
                                Mail.Money = 0;
                            }
                            if (Mail.EMoney > 0)
                            {
                                user.Player.ConquerPoints += (uint)Mail.EMoney;
                                user.SendSysMesage("You received " + Mail.EMoney + " ConquerPoints.");
                                Mail.EMoney = 0;
                            }
                            user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                        }
                        break;
                    }
                case MsgMailOperation.Type.ClaimItem:
                    {
                        PrizeInfo Mail;
                        if (user.PrizeInfo.TryGetValue(ID, out Mail))
                        {
                            if (Mail.itemid > 0)
                            {
                                if (!user.Inventory.HaveSpace(1))
                                {
                                    user.SendSysMesage("STR_BAG_FULL@@");
                                }
                                else
                                {
                                    if (user.Warehouse != null)
                                    {
                                        user.Warehouse.RemoveItem(Mail.itemid, 1000, stream);
                                        Mail.itemid = 0;
                                    }
                                }
                            }
                            user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                        }
                        break;
                    }
                case MsgMailOperation.Type.ClaimRecordType:
                    {
                        PrizeInfo Mail;
                        if (user.PrizeInfo.TryGetValue(ID, out Mail))
                        {
                            if (Mail.Flag > 0)
                            {
                                if (Item(Mail.Flag, user))
                                {
                                    Mail.Flag = 0;
                                    user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                                }
                            }
                        }
                        break;

                    }
                case MsgMailOperation.Type.DeleteClaim:
                    {
                        foreach (var id in Items)
                        {
                            PrizeInfo Mail;
                            if (user.PrizeInfo.TryGetValue((uint)id, out Mail))
                            {
                                if (Mail.Money == 0 && Mail.EMoney == 0 && Mail.itemid == 0 && Mail.Flag == 0)
                                {
                                    user.PrizeInfo.Remove(Mail.id);
                                    user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                                }
                            }
                            else
                            {
                                user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                            }
                        }
                        break;

                    }
                case MsgMailOperation.Type.ClaimAll:
                    {
                        foreach (var id in Items)
                        {
                            PrizeInfo Mail;
                            if (user.PrizeInfo.TryGetValue((uint)id, out Mail))
                            {
                                if (Mail.Money > 0)
                                {
                                    user.Player.Money += (ulong)Mail.Money;
                                    user.SendSysMesage("STR_PICK_MONEY@@" + Mail.Money + "@@");
                                    Mail.Money = 0;
                                }
                                if (Mail.EMoney > 0)
                                {
                                    user.Player.ConquerPoints += (uint)Mail.EMoney;
                                    user.SendSysMesage("You received " + Mail.EMoney + " ConquerPoints.");
                                    Mail.EMoney = 0;
                                }
                                
                                if (Mail.Flag > 0)
                                {
                                    if (Item(Mail.Flag, user))
                                    {
                                        Mail.Flag = 0;
                                        Mail.Emoney_Record_Type = 1;
                                    }
                                }
                                if (Mail.itemid > 0)
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("STR_BAG_FULL@@");
                                    }
                                    else
                                    {
                                        if (user.Warehouse != null)
                                        {
                                            user.Warehouse.RemoveItem(Mail.itemid, 1000, stream);
                                            Mail.itemid = 0;
                                        }
                                    }
                                }
                                MsgMailList.CMsgMailListProto _Mail = new MsgMailList.CMsgMailListProto();
                                _Mail.Type = 1;
                                _Mail.Count = 1;
                                _Mail.MailList = new MsgMailList.CMsgMailListProto.Mail[1];
                                _Mail.MailList[0] = new MsgMailList.CMsgMailListProto.Mail(Mail);
                                user.Send(stream.CreateMailList(_Mail));
                            }
                            #region MTEdit
                            // Delete only when every attachment was delivered (bag full keeps the mail).
                            if (user.PrizeInfo != null && Mail != null
                                && Mail.Money == 0 && Mail.EMoney == 0 && Mail.itemid == 0 && Mail.Flag == 0)
                            {
                                user.PrizeInfo.Remove(Mail.id);
                                user.Send(stream.CreateMailboxMode(MsgMailOperation.Type.Delete, Mail.id, count, Items));
                                MsgMailNotify.Loading(user);
                            }
                            #endregion
                        }
                        user.Send(stream.CreateMailboxMode(Type, ID, count, Items));
                        break;
                    }
                default:
                    Console.WriteLine("[Mailbox] Unknown Type:" + Type.ToString());
                    break;

            }
        }
    }
}