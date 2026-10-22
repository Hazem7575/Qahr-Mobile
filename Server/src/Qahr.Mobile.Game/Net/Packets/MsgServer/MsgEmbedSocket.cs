using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetEmbedSocket(this ServerSockets.Packet stream, out uint ItemUID, out uint GemUID
            , out MsgEmbedSocket.ActionSlot Slot, out MsgEmbedSocket.ActionMode Mode)
        {
            uint unknow2 = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
            GemUID = stream.ReadUInt32();
            Slot = (MsgEmbedSocket.ActionSlot)stream.ReadUInt16();
            Mode = (MsgEmbedSocket.ActionMode)stream.ReadUInt16();
        }
    }
    public struct MsgEmbedSocket
    {
        public enum ActionSlot : ushort
        {
            SlotOne = 1,
            SlotTwo = 2
        }
        public enum ActionMode : ushort
        {
            Add = 0,
            Remove = 1,
        }

        [PacketAttribute(GamePackets.MsgGemEmbed)]
        public unsafe static void EmbedSocket(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.IsVendor)
            {
                user.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (user.InTrade)
            {
                user.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }
            uint ItemUID;
            uint GemUID;
            ActionSlot Slot;
            ActionMode Mode;

            stream.GetEmbedSocket(out ItemUID, out GemUID, out Slot, out Mode);

            MsgGameItem DataItem;
            if (user.TryGetItem(ItemUID, out DataItem))
            {
                ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                //anti-proxy-----------------------
                if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Tower
                    && Position != (ushort)Role.Flags.ConquerItem.Wing && Position != (ushort)Role.Flags.ConquerItem.Relics & Position != (ushort)Role.Flags.ConquerItem.Bottle)
                {
                    if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                    {
#if Arabic
                         user.SendSysMesage("Sorry can't make socket in this item.");
#else
                        user.SendSysMesage("Sorry can't make socket in this item.");
#endif

                        return;
                    }
                }
                //------------------------
                switch (Mode)
                {
                    case ActionMode.Add:
                        {
                            MsgGameItem Gem;
                            // Look in the BAG only (user.TryGetItem would also match an equipped item, letting the
                            // "gem" be a worn piece that the REMOVE below could never consume -> free socketing).
                            if (user.Inventory.TryGetItem(GemUID, out Gem) && GemUID != DataItem.UID)
                            {
                                // Must be a real gem (700xxx). The old check only looked at ITEM_ID % 1000, so any
                                // item whose last 3 digits matched a gem enum value counted as a gem.
                                if (Gem.ITEM_ID / 1000 == 700
                                    && Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(Gem.ITEM_ID % 1000)))
                                {
                                    Role.Flags.Gem GemType = (Role.Flags.Gem)(Gem.ITEM_ID % 1000);

                                    //anti proxy------------------
                                    if (Position == (ushort)Role.Flags.ConquerItem.Fan)
                                    {
                                        if (!Database.ItemType.CheckAddGemFan(GemType))
                                            return;
                                    }
                                    if (Position == (ushort)Role.Flags.ConquerItem.Tower)
                                    {
                                        if (!Database.ItemType.CheckAddGemTower(GemType))
                                            return;
                                    }
                                    if (Position == (ushort)Role.Flags.ConquerItem.Wing)
                                    {
                                        if (!Database.ItemType.CheckAddGemWing(GemType, (byte)Slot))
                                            return;
                                    }
                                    if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Tower
                                        && Position != (ushort)Role.Flags.ConquerItem.Wing)
                                    {
                                        if (Database.ItemType.CheckAddGemFan(GemType) || Database.ItemType.CheckAddGemTower(GemType))
                                            return;
                                    }
                                    if (Database.ItemType.IsHossu(DataItem.ITEM_ID))
                                    {
                                        if (GemType != Role.Flags.Gem.NormalInfinityGem && GemType != Role.Flags.Gem.RefinedInfinityGem && GemType != Role.Flags.Gem.SuperInfinityGem)
                                            return;
                                    }
                                    //------------------------

                                    switch (Slot)
                                    {
                                        case ActionSlot.SlotOne:
                                            {
                                                if (DataItem.SocketOne == Role.Flags.Gem.EmptySocket)
                                                {
                                                    DataItem.SocketOne = GemType;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(user, stream).Update(Gem, Role.Instance.AddMode.REMOVE, stream);
                                                    if (DataItem.Position != 0)
                                                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                                }
                                                break;
                                            }
                                        case ActionSlot.SlotTwo:
                                            {

                                                if ((byte)DataItem.SocketOne > 0 && (byte)DataItem.SocketOne < 255 || Position == (ushort)Role.Flags.ConquerItem.Wing)
                                                {
                                                    if (DataItem.SocketTwo == Role.Flags.Gem.EmptySocket)
                                                    {
                                                        DataItem.SocketTwo = GemType;
                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(user, stream).Update(Gem, Role.Instance.AddMode.REMOVE, stream);
                                                        if (DataItem.Position != 0)
                                                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                                    }
                                                }
                                                else
                                                {
                                                    goto case ActionSlot.SlotOne;
                                                }
                                                break;
                                            }

                                    }

                                }
                            }

                            break;
                        }
                    case ActionMode.Remove:
                        {
                            switch (Slot)
                            {
                                case ActionSlot.SlotOne:
                                    {
                                        if (DataItem.SocketOne != Role.Flags.Gem.NoSocket)
                                        {
                                            DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                            if (DataItem.SocketTwo != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.EmptySocket
                                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                DataItem.SocketOne = DataItem.SocketTwo;
                                                DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                            }
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        break;
                                    }
                                case ActionSlot.SlotTwo:
                                    {
                                        if (DataItem.SocketTwo != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.EmptySocket)
                                        {
                                            DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        else if (DataItem.SocketOne != Role.Flags.Gem.NoSocket)
                                        {
                                            DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }

                }
            }
        }
    }
}
