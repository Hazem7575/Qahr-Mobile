using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgBatchEquip
    {
        [ProtoContract]
        public class MsgRuneQueryProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint[] EquipItems;
            [ProtoMember(2)]
            public ItemList[] Item;
        }
        [ProtoContract]
        public class ItemList
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Postion;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemID;
        }
        public static unsafe ServerSockets.Packet CreateBatchEquip(this ServerSockets.Packet stream, MsgRuneQueryProto msg)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(msg);
            stream.Finalize(GamePackets.MsgBatchEquip);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgBatchEquip)]
        public static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgRuneQueryProto msg = new MsgRuneQueryProto();
            msg = stream.ProtoBufferDeserialize<MsgRuneQueryProto>(msg);
            if (msg.Item.Length > 0)
            {
                MsgGameItem Items;
                foreach (var Item in msg.Item)
                {
                    if (client.Rune.Free((byte)Item.Postion) || client.Rune.Unequip(client.Rune.TryGetItem((byte)Item.Postion), true))
                    {
                        if (client.Rune.TryGetItem(Item.ItemID, out Items))
                        {
                            if (client.Rune.Equip(Items, (byte)Item.Postion, true))
                            {
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                Database.RuneRank.Update(client);
                            }
                        }
                    }
                }
            }
            client.Send(stream.CreateBatchEquip(msg));
        }
    }
}
