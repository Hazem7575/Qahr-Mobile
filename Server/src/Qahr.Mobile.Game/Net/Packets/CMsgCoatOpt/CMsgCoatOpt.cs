using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Collections.Concurrent;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreatCoatOpt(this ServerSockets.Packet stream, CMsgCoatOpt.ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.CMsgCoatOpt);
            return stream;

        }

        public static void GetCoatOpt(this ServerSockets.Packet stream, out CMsgCoatOpt.ProtoStructure pQuery)
        {
            pQuery = new CMsgCoatOpt.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<CMsgCoatOpt.ProtoStructure>(pQuery);
        }
    }
    public static class CMsgCoatOpt
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public Action Action;

            [ProtoMember(2, IsRequired = true)]
            public uint dwpram2 = 0;

            [ProtoMember(3, IsRequired = true)]
            public uint dwpram3 = 0;

            [ProtoMember(4, IsRequired = true)]
            public uint dwpram4 = 0;

            [ProtoMember(5, IsRequired = true)]
            public uint dwpram5 = 0;

            [ProtoMember(6, IsRequired = true)]
            public List<ItemPrize> Items = new List<ItemPrize>();

        }
        [ProtoContract]
        public class ItemPrize
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ITEMID = 0;

            [ProtoMember(2, IsRequired = true)]
            public uint COUNT = 0;

            [ProtoMember(3, IsRequired = true)]
            public uint TYPE = 0;
        }
        [Flags]
        public enum Action : uint
        {
            OneDraw = 2,
            ThreeDraw = 3,
        }
        [PacketAttribute(GamePackets.CMsgCoatOpt)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade || client.IsVendor)
                return;
            ProtoStructure pQuery;
            stream.GetCoatOpt(out pQuery);

            switch ((Action)pQuery.Action)
            {
                case Action.OneDraw:
                    {
                        switch (pQuery.dwpram5)
                        {
                            case 1:
                                {
                                    if (client.Inventory.Contain(3008221, 1) || client.Inventory.Contain(3008221, 1, 1))
                                    {
                                        client.Inventory.Remove(3008221, 1, stream);
                                        uint id = Database.CoatOptTable.OliveJadePrize[Pool.GetRandom.Next(0, Database.CoatOptTable.OliveJadePrize.Count)] / 10;
                                        pQuery.Items = new List<ItemPrize>();
                                        pQuery.Items.Add(new ItemPrize() { ITEMID = id , COUNT = 1, TYPE = 3 });
                                        client.Inventory.Add(stream, id , 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Send(stream.CreatCoatOpt(pQuery));
                                        
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (client.Inventory.Contain(3008223, 1) || client.Inventory.Contain(3008223, 1, 1))
                                    {
                                        client.Inventory.Remove(3008223, 1, stream);
                                        for (uint index = 0; index < 1; index++)
                                        {
                                            uint id = Database.CoatOptTable.IvoryJadePrize[Pool.GetRandom.Next(0, Database.CoatOptTable.IvoryJadePrize.Count)] / 10;
                                            pQuery.Items = new List<ItemPrize>();
                                            pQuery.Items.Add(new ItemPrize() { ITEMID = id , COUNT = 1 , TYPE = 3});
                                            client.Inventory.Add(stream, id, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }

                                          client.Send(stream.CreatCoatOpt(pQuery));
                                    }
                                    break;
                                }
                        }

                        break;
                    }
                case Action.ThreeDraw:
                    {
                        switch (pQuery.dwpram5)
                        {
                            case 1:
                                {
                                    if (client.Inventory.Contain(3008221, 3) || client.Inventory.Contain(3008221, 3, 1))
                                    {
                                        client.Inventory.RemoveStackItem(3008221, 3, stream);
                                        pQuery.Items = new List<ItemPrize>();
                                        for (uint i = 0; i < 3; i++)
                                        {
                                            uint id = Database.CoatOptTable.OliveJadePrize[Pool.GetRandom.Next(0, Database.CoatOptTable.OliveJadePrize.Count)] / 10;
                                            pQuery.Items.Add(new ItemPrize() { ITEMID = id, COUNT = 1, TYPE = 3 });
                                            client.Inventory.Add(stream, id, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        client.Send(stream.CreatCoatOpt(pQuery));
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (client.Inventory.Contain(3008223, 3) || client.Inventory.Contain(3008223, 3, 1))
                                    {
                                        client.Inventory.RemoveStackItem(3008223, 3, stream);
                                        pQuery.Items = new List<ItemPrize>();
                                        for (uint i = 0; i < 3; i++)
                                        {
                                            uint id = Database.CoatOptTable.IvoryJadePrize[Pool.GetRandom.Next(0, Database.CoatOptTable.IvoryJadePrize.Count)] / 10;
                                            pQuery.Items.Add(new ItemPrize() { ITEMID = id, COUNT = 1, TYPE = 3 });
                                            client.Inventory.Add(stream, id, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        client.Send(stream.CreatCoatOpt(pQuery));
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    Console.WriteLine(" CMsgCoatOpt Info.Type: " + pQuery.Action);
                    break;
            }
        }

    }
}
