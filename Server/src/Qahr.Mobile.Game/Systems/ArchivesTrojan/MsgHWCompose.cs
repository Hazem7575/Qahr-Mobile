using ProtoBuf;
using System;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgHWCompose
    {
        public const uint ImproveItemID = 3321098, ImproveEXP = 10;
        public const uint SpiritStone_ID = 3321107, ChaosJade_ID = SpiritStone_ID + 1;
        [ProtoContract]
        public class MsgHWComposeProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2)]
            public uint SubType;
            [ProtoMember(3)]
            public uint dwParam;
            [ProtoMember(4)]
            public uint AppearancePosition;
            [ProtoMember(5)]
            public Item[] Items;
        }
        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(2, IsRequired = true)]
            public uint dwParam; // [ProtoFix 2026-10-14] client CHWComposeInfoPB #2 is uint32 (was int). Also closes a negative-count exploit (-1 skipped the StackSize clamp and added huge Progress)
        }
        [Flags]
        public enum ActionID
        {
            Improve,//0
            Baptize,//1
            ConfirmBaptize,//2
            Guardian//3
        }
        public static unsafe ServerSockets.Packet CreateHWCompose(this ServerSockets.Packet stream, MsgHWComposeProto msg)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(msg);
            stream.Finalize(GamePackets.MsgHWCompose);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgHWCompose)]
        public static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgHWComposeProto msg = new MsgHWComposeProto();
            msg = stream.ProtoBufferDeserialize<MsgHWComposeProto>(msg);
            if ((msg.Items == null && (byte)msg.Type < (byte)ActionID.Baptize) || !client.HundredWeapons.Unlocked || !client.HundredWeapons.Valid() || !client.HundredWeapons.ContainsWeapon((Database.MagicType.WeaponsType)msg.SubType)) return;
            var myWeapon = client.HundredWeapons.Objects[(Database.MagicType.WeaponsType)msg.SubType];
            switch (msg.Type)
            {
                case ActionID.Improve:
                    {
                        MsgGameItem ItemPlus = null;
                        for (int i = 0; i < msg.Items.Length; i++)
                        {
                            if (!client.Inventory.TryGetItem(msg.Items[i].ItemUID, out ItemPlus))
                                break;
                            if (ItemPlus.ITEM_ID != ImproveItemID)
                                break;
                            if (myWeapon.Level >= 9)
                                break;
                            if (ItemPlus.StackSize < msg.Items[i].dwParam)
                            {
                                msg.Items[i].dwParam = ItemPlus.StackSize;
                                ItemPlus.Send(client, stream);
                            }
                            myWeapon.Progress += (uint)(ImproveEXP * msg.Items[i].dwParam);
                         
                            while (myWeapon.Progress >= myWeapon.DBInfo.Progress)
                            {
                                myWeapon.Progress -= myWeapon.DBInfo.Progress;
                                myWeapon.Level++;
                                if (myWeapon.Level >= 9)
                                {
                                    myWeapon.Progress = 0;
                                    break;
                                }
                            }
                            client.Inventory.RemoveStackItem(ItemPlus.UID, (ushort)msg.Items[i].dwParam, stream);
                        }
                        msg.Items = null;
                        client.Send(stream.CreateHundredWeaponsInfo(myWeapon, client, MsgHundredWeaponsInfo.ActionID.Update));
                        client.Send(stream.CreateHWCompose(msg));
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case ActionID.Baptize:
                    {
                        if (msg.Items.Length < 1 || myWeapon.Level < 3)
                            break;
                        if (!client.Inventory.Remove(msg.Items[0].ItemUID, myWeapon.DBInfo.ReqItemCount, stream))
                            break;
                        if (msg.Items[0].ItemUID == SpiritStone_ID)
                        {
                            int value1 = 0;
                            int value2 = 0;
                            int value3 = 0;
                            value1 += 5;
                            value2 += 5;
                            value3 += 5;
                            myWeapon.TampHitpoints = (uint)Math.Min(500, myWeapon.TampHitpoints + (uint)value1);
                            myWeapon.TampPhysicalAttack = (uint)Math.Min(300, myWeapon.TampHitpoints + (uint)value2);
                            myWeapon.TampPhysicalDefense = (uint)Math.Min(200, myWeapon.TampHitpoints + (uint)value3);
                        }
                        else if (msg.Items[0].ItemUID == ChaosJade_ID)
                        {
                            int value1 = 0;
                            int value2 = 0;
                            int value3 = 0;
                            value1 += 10;
                            value2 += 10;
                            value3 += 10;
                            myWeapon.TampHitpoints = (uint)Math.Min(500, myWeapon.TampHitpoints + (uint)value1);
                            myWeapon.TampPhysicalAttack = (uint)Math.Min(300, myWeapon.TampHitpoints + (uint)value2);
                            myWeapon.TampPhysicalDefense = (uint)Math.Min(200, myWeapon.TampHitpoints + (uint)value3);
                           
                        }

                        msg.Items = null;
                        client.Send(stream.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.Update));
                        client.Send(stream.CreateHWCompose(msg));
                        break;
                    }
                case ActionID.ConfirmBaptize:
                    {
                        if (msg.Items.Length < 1 || myWeapon.Level < 3)
                            break;
                        if (msg.dwParam == 1)
                        {
                            myWeapon.Attributes[Database.HundredWeapons.AttributeType.Hitpoints] = (int)myWeapon.TampHitpoints;
                            myWeapon.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] = (int)myWeapon.TampPhysicalAttack;
                            myWeapon.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense] = (int)myWeapon.TampPhysicalDefense;
                        }

                        msg.Items = null;
                        client.Send(stream.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.Update));
                        client.Send(stream.CreateHWCompose(msg));
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case ActionID.Guardian:
                    {
                        if (msg.AppearancePosition > 0)
                        {
                            if (client.HundredWeapons.FreeAppearance((byte)msg.AppearancePosition) || client.HundredWeapons.Hide((byte)msg.AppearancePosition))
                            {
                                byte duplicatedPosition;
                                if (!client.HundredWeapons.checkDuplicatedAppearance((Database.MagicType.WeaponsType)msg.SubType, (byte)msg.AppearancePosition, out duplicatedPosition) || client.HundredWeapons.Hide(duplicatedPosition))
                                {
                                    client.HundredWeapons.Show((Database.MagicType.WeaponsType)msg.SubType, (byte)msg.AppearancePosition);
                                    msg.dwParam = 1;
                                }
                            }
                        }
                        else
                        {
                            if (myWeapon.AppearancePosition > 0)
                                client.HundredWeapons.Hide((Database.MagicType.WeaponsType)msg.SubType);
                        }
                        client.Send(stream.CreateHWCompose(msg));
                        break;
                    }
            }
            client.HundredWeapons.UpdateRank();
        }
    }
}