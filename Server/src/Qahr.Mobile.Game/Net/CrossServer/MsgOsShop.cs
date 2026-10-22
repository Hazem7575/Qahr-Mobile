using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgOsShop
    {
        public static unsafe void GetOsShop(this ServerSockets.Packet stream, out byte type,
            out ushort DwParam1, out ushort DwParam2, out ushort DwParam3, out byte Count, out ushort DwParam4)
        {
            type = stream.ReadUInt8();
            DwParam1 = stream.ReadUInt16();
            DwParam2 = stream.ReadUInt16();
            DwParam3 = stream.ReadUInt16();
            Count = stream.ReadUInt8();
            DwParam4 = stream.ReadUInt16();

        }
        public static unsafe ServerSockets.Packet OsShopCreate(this ServerSockets.Packet stream, byte type,
             ushort DwParam1,  ushort DwParam2,  ushort DwParam3,  byte Count,  ushort DwParam4)
        {
            stream.InitWriter();

            stream.Write(type);
            stream.Write(DwParam1);
            stream.Write(DwParam2);
            stream.Write(DwParam3);
            stream.Write(Count);
            stream.Write(DwParam4);

            stream.Finalize(GamePackets.MsgOsShop);
            return stream;
        }

        [PacketAttribute(GamePackets.MsgOsShop)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
           

            byte type;
            ushort DwParam1;  
            ushort DwParam2;  
            ushort DwParam3; 
            byte Count;
            ushort DwParam4;
            stream.GetOsShop(out type, out DwParam1, out DwParam2, out DwParam3, out Count, out DwParam4);

            if (type == 8 && Count != 0)
            {
                byte Spaces = Count;
                int Cost = (int)(Count);
                if (user.Inventory.HaveSpace(Spaces))
                {
                    if (DwParam2 == 40066 || DwParam2 == 41962)
                    {
                        if (user.Player.BoundConquerPoints >= Cost)
                        {
                            user.Player.BoundConquerPoints -= Cost;
                            List<uint> Items = new List<uint>();
                            foreach (var item in user.Inventory.ClientItems.Values)
                                Items.Add(item.UID);
                            if (DwParam2 == 40066)
                            {
                                user.Inventory.AddItemWitchStack(725065, 0, (ushort)(Count * 5), stream);
                            }
                            else
                            {
                                user.Inventory.AddItemWitchStack(725066, 0, (ushort)(Count * 5), stream);
                            }
                            if (user.IsConnectedInterServer())
                            {
                                foreach (var item in user.Inventory.ClientItems.Values)
                                    if (!Items.Contains(item.UID))
                                        item.Send(user.PipeClient, stream);
                            }
                        }
                        else if (user.Player.ConquerPoints >= Cost)
                        {
                            if (user.InTrade) return;
                            user.Player.ConquerPoints -= (uint)Cost;
                            user.Player.SendUpdate(stream, user.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints); // CP branch: send CP, not BoundCP
                            List<uint> Items = new List<uint>();
                            foreach (var item in user.Inventory.ClientItems.Values)
                                Items.Add(item.UID);
                            if (DwParam2 == 40066)
                            {
                                user.Inventory.AddItemWitchStack(725065, 0, (ushort)(Count * 5), stream);
                            }
                            else
                            {
                                user.Inventory.AddItemWitchStack(725066, 0, (ushort)(Count * 5), stream);
                            }
                            
                            if (user.IsConnectedInterServer())
                            {
                                foreach (var item in user.Inventory.ClientItems.Values)
                                    if (!Items.Contains(item.UID))
                                        item.Send(user.PipeClient, stream);
                            }
                        }
                    }
                }
            }

        }

    }
}
