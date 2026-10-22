using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgBeastsInfo
    {
        [ProtoContract]
        public class MsgBeastsInfoProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2)]
            public byte Level;
            [ProtoMember(3)]
            public uint Points;
            [ProtoMember(4)]
            public uint FixedLevel;
            [ProtoMember(5)]
            public uint Unknown2;
            [ProtoMember(6)]
            public bool Activated;
            [ProtoMember(7)]
            public uint Flag;
        }

        public static unsafe ServerSockets.Packet CreateBeastsInfo(this ServerSockets.Packet stream, MsgBeastsInfoProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBeastsInfo);
            return stream;
        }

        public static unsafe void GetBeastsInfo(this ServerSockets.Packet stream, out MsgBeastsInfoProto pQuery)
        {
            pQuery = new MsgBeastsInfoProto();
            pQuery = stream.ProtoBufferDeserialize<MsgBeastsInfoProto>(pQuery);
        }

        [PacketAttribute(GamePackets.MsgBeastsInfo)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgBeastsInfoProto Info;
            stream.GetBeastsInfo(out Info);

            Client.GameClient player;
            if (Pool.GamePoll.TryGetValue(Info.UID, out player))
            {
                client.Send(stream.CreateBeastsInfo(new MsgBeastsInfoProto()
                {
                    UID = player.Player.UID,
                    Level = player.Beasts.Level,
                    Activated = player.Beasts.Activated,
                    FixedLevel = player.Beasts.FixedLevel,
                    Points = player.Beasts.Points,
                    Unknown2 = 1,
                    Flag = player.Beasts.Flag
                }));
            }
        }
    }

    public static class MsgBeastsOpt
    {
        [ProtoContract]
        public class MsgBeastsOptProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;

            [ProtoMember(4, IsRequired = true)]
            public Item[] Items;
        }

        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2, IsRequired = true)]
            public ushort Stack;
        }

        public static unsafe ServerSockets.Packet CreateBeastsOpt(this ServerSockets.Packet stream, MsgBeastsOptProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBeastsOpt);
            return stream;
        }

        public static unsafe void GetBeastsOpt(this ServerSockets.Packet stream, out MsgBeastsOptProto pQuery)
        {
            pQuery = new MsgBeastsOptProto();
            pQuery = stream.ProtoBufferDeserialize<MsgBeastsOptProto>(pQuery);
        }

        public static uint GetPoints(uint itemID)
        {
            if (itemID == 3009100) return 100;
            if (itemID == 3009101) return 300;
            if (itemID == 3009102) return 1000;
            if (itemID == 3009103) return 3000;
            if (itemID == 3009104) return 10000;
            return 0;
        }

        [PacketAttribute(GamePackets.MsgBeastsOpt)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgBeastsOptProto Info;
            stream.GetBeastsOpt(out Info);

            if (Info.Type == 0)
            {
                if (!client.Beasts.Activated && client.Rune.EquippedCount >= 5)
                {
                    client.Beasts.Activated = true;
                    client.Beasts.Level = 1;
                }
            }
            else if (Info.Type == 1)
            {
                if (Info.Items != null)
                {
                    for (int i = 0; i < Info.Items.Length; i++)
                    {
                        MsgGameItem item;
                        if (client.Inventory.TryGetItem(Info.Items[i].UID, out item))
                        {
                            if (item.StackSize < Info.Items[i].Stack)
                            {
                                Info.Items[i].Stack = item.StackSize;
                                item.Send(client, stream);
                            }

                            client.Inventory.RemoveStackItem(item.UID, Info.Items[i].Stack, stream);

                            client.Beasts.Points += GetPoints(item.ITEM_ID) * Info.Items[i].Stack;

                            while (Database.BeastsAtrribute.Attributes.ContainsKey(client.Beasts.Level) &&
                                   client.Beasts.Points >= Database.BeastsAtrribute.Attributes[client.Beasts.Level].RequiredPoints)
                            {
                                client.Beasts.Points -= Database.BeastsAtrribute.Attributes[client.Beasts.Level].RequiredPoints;
                                client.Beasts.Level++;
                            }
                        }
                    }
                }

                client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }

            client.Send(stream.CreateBeastsInfo(new MsgBeastsInfo.MsgBeastsInfoProto()
            {
                UID = client.Player.UID,
                Level = client.Beasts.Level,
                Activated = client.Beasts.Activated,
                FixedLevel = client.Beasts.FixedLevel,
                Points = client.Beasts.Points,
                Unknown2 = 1,
                Flag = client.Beasts.Flag
            }));

            client.Send(stream.CreateBeastsOpt(Info));
        }
    }
}