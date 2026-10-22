using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgRuneStorage
    {
        [ProtoContract]
        public class MsgRuneStorageProto
        {
            [ProtoMember(1, IsRequired = true)]
            public byte Type;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(3)]
            public uint HPAdd;
        }
        public static unsafe ServerSockets.Packet CreateRuneStorage(this ServerSockets.Packet stream, MsgRuneStorageProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgRuneStorage);

            return stream;
        }
        public static unsafe void GetRuneStorage(this ServerSockets.Packet stream, out MsgRuneStorageProto pQuery)
        {
            pQuery = new MsgRuneStorageProto();
            pQuery = stream.ProtoBufferDeserialize<MsgRuneStorageProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgRuneStorage)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgRuneStorageProto Info;
            stream.GetRuneStorage(out Info);
            MsgGameItem item;
            if (client.Player.Reborn < 2)
            {
                if (Info.Type != 2)
                    client.SendSysMesage("You need to be a second reborn hero to interact with rune system!");
                return;
            }
            switch (Info.Type)
            {
                case 0://Move To Collection
                    {
                        if (client.Rune.TryGetItem(Info.ItemUID, out item))
                        {
                            var count = client.Rune.Objects.Count(i => i.Position == (ushort)Role.Flags.ConquerItem.RunesCollection && i.ITEM_ID / 100 == item.ITEM_ID / 100);
                            if (count != 0)
                                return;
                            item.Position = (ushort)Role.Flags.ConquerItem.RunesCollection;
                        }
                        break;
                    }
                case 1://Retrieve (Move To Wardrobe 'Bag')
                    {
                        if (client.Rune.TryGetItem(Info.ItemUID, out item))
                        {
                            item.Position = (ushort)Role.Flags.ConquerItem.RuneBag;
                        }
                        break;
                    }
                case 2://Info
                    {
                        if (Database.RuneRank.RankingList.ContainsKey(client.Player.UID))
                        {
                            for (int i = 0; i < Database.RuneRank.RankingList.Count; i++)
                                if (Database.RuneRank.RankingList.ToArray()[i].Key == client.Player.UID)
                                    client.SendSysMesage("My Rank: " + (i + 1) + "; Current Rune Bag Score: " + client.Rune.Score);
                        }
                        else client.SendSysMesage("My Rank: not ranked; Current Rune Bag Score: " + client.Rune.Score);
                        break;
                    }
                case 3://From inventory to collection
                    {
                        if (client.Inventory.TryGetItem(Info.ItemUID, out item) && client.Rune.Objects.Count(i => i.Position == (ushort)Role.Flags.ConquerItem.RunesCollection && i.ITEM_ID / 100 == item.ITEM_ID / 100) == 0)
                        {
                            if (client.Rune.Add(item))
                            {
                                item.Position = (ushort)Role.Flags.ConquerItem.RunesCollection;
                                client.Inventory.ClientItems.Remove(Info.ItemUID);
                            }
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        break;
                    }
            }
            Info.HPAdd = client.Rune.HPClient;
            client.Send(stream.CreateRuneStorage(Info));
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
            Database.RuneRank.Update(client);
        }
    }
}
