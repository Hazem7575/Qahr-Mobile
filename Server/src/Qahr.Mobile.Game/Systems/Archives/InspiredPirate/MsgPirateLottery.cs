using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgPirateLottery
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public Action Type;//1

            [ProtoMember(2, IsRequired = true)]
            public uint Switch;//1

            [ProtoMember(3, IsRequired = true)]
            public List<Item> Items;
        }

        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ItemID;//ID

            [ProtoMember(2, IsRequired = true)]
            public uint Change;//Change
        }
        public static unsafe ServerSockets.Packet CreatePirateLottery(this ServerSockets.Packet stream, ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgPrirateDictLottery);
            return stream;
        }
        public static unsafe void GetPirateLottery(this ServerSockets.Packet stream, out ProtoStructure pQuery)
        {
            pQuery = new ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<ProtoStructure>(pQuery);
        }
        public enum Action : uint
        {
            OneLottery = 0,
            Draw = 1,
        }
        [PacketAttribute(GamePackets.MsgPrirateDictLottery)]
        private unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade || client.IsVendor)
                return;
            ProtoStructure pQuery;
            stream.GetPirateLottery(out pQuery);
            switch (pQuery.Type)
            {
                case Action.OneLottery:
                    {
                        #region AirPower
                        if (pQuery.Switch == 1)
                        {

                            if (client.MyArchives.AirPower >= 405)
                            {
                                client.MyArchives.AirPower -= 405;
                                MsgPirateOpt.UpdatePoints(client);
                                var daoqi = dict_lottery.DaoqiItem.Values.ToArray();
                                dict_lottery.item selected = null;
                                pQuery.Items = new List<Item>();
                                if (Role.Core.Rate(4.3))
                                {
                                    var list = daoqi.Where(p => p.ID >= 26 && p.ID <= 28).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(23.7))
                                {
                                    var list = daoqi.Where(p => p.ID >= 1 && p.ID <= 11).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(31.1))
                                {
                                    var list = daoqi.Where(p => p.ID >= 12 && p.ID <= 25).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else
                                {
                                    var list = daoqi.Where(p => p.ID >= 29 && p.ID <= 31).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                uint Have = 0;

                                uint Amount = 0;
                                if (selected.Quality == 2)
                                {
                                    if (!client.MyArchives.AddRune(selected.IDItem))
                                    {
                                        Have = 1;
                                        Amount = 10;
                                    }
                                }
                                else
                                {
                                    Have = 1;
                                    if (selected.ID >= 26 && selected.ID <= 28)
                                        Amount = 50;
                                    if (selected.ID >= 29 && selected.ID <= 31)
                                        Amount = selected.IDItem;
                                }
                                pQuery.Items.Add(new Item() { ItemID = selected.ID, Change = Have });
                                
                                client.Send(stream.CreatePirateLottery(pQuery));
                                if (Amount > 0)
                                {
                                    client.MyArchives.UniversalFragment += Amount;
                                    MsgCombatGearTao.AddFragemnt(client);
                                }


                            }

                        }
                        #endregion
                        #region Evil Compass
                        else if (client.Inventory.Contain(3329550,1))
                        {
                            client.Inventory.Remove(3329550, 1, stream);
                            var daoqi = dict_lottery.DaoqiItem.Values.ToArray();
                            dict_lottery.item selected = null;
                            pQuery.Items = new List<Item>();
                            if (Role.Core.Rate(4.3))
                            {
                                var list = daoqi.Where(p => p.ID >= 26 && p.ID <= 28).ToArray();
                                selected = list[Program.GetRandom.Next(list.Length)];
                            }
                            else if (Role.Core.Rate(23.7))
                            {
                                var list = daoqi.Where(p => p.ID >= 1 && p.ID <= 11).ToArray();
                                selected = list[Program.GetRandom.Next(list.Length)];
                            }
                            else if (Role.Core.Rate(31.1))
                            {
                                var list = daoqi.Where(p => p.ID >= 12 && p.ID <= 25).ToArray();
                                selected = list[Program.GetRandom.Next(list.Length)];
                            }
                            else
                            {
                                var list = daoqi.Where(p => p.ID >= 29 && p.ID <= 31).ToArray();
                                selected = list[Program.GetRandom.Next(list.Length)];
                            }
                            uint Have = 0;

                            uint Amount = 0;
                            if (selected.Quality == 2)
                            {
                                if (!client.MyArchives.AddRune(selected.IDItem))
                                {
                                    Have = 1;
                                    Amount = 10;
                                }
                            }
                            else
                            {
                                Have = 1;
                                if (selected.ID >= 26 && selected.ID <= 28)
                                    Amount = 50;
                                if (selected.ID >= 29 && selected.ID <= 31)
                                    Amount = selected.IDItem;
                            }
                            pQuery.Switch = 1;
                            pQuery.Items.Add(new Item() { ItemID = selected.ID, Change = Have });
                            client.Send(stream.CreatePirateLottery(pQuery));
                            if (Amount > 0)
                            {
                                client.MyArchives.UniversalFragment += Amount;
                                MsgCombatGearTao.AddFragemnt(client);
                            }
                        }
                        #endregion
                        break;
                    }
                case Action.Draw:
                    {
                        #region AirPower
                        if (pQuery.Switch == 1)
                        {
                            if (client.MyArchives.AirPower >= 4050)
                            {
                                client.MyArchives.AirPower -= 4050;
                                MsgPirateOpt.UpdatePoints(client);
                                uint Amount = 0;
                                pQuery.Items = new List<Item>();

                                for (int i = 0; i < 10; i++)
                                {
                                    var daoqi = dict_lottery.DaoqiItem.Values.ToArray();
                                    dict_lottery.item selected = null;

                                    if (Role.Core.Rate(4.3))
                                    {
                                        var list = daoqi.Where(p => p.ID >= 26 && p.ID <= 28).ToArray();
                                        selected = list[Program.GetRandom.Next(list.Length)];
                                    }
                                    else if (Role.Core.Rate(23.7))
                                    {
                                        var list = daoqi.Where(p => p.ID >= 1 && p.ID <= 11).ToArray();
                                        selected = list[Program.GetRandom.Next(list.Length)];
                                    }
                                    else if (Role.Core.Rate(31.1))
                                    {
                                        var list = daoqi.Where(p => p.ID >= 12 && p.ID <= 25).ToArray();
                                        selected = list[Program.GetRandom.Next(list.Length)];
                                    }
                                    else
                                    {
                                        var list = daoqi.Where(p => p.ID >= 29 && p.ID <= 31).ToArray();
                                        selected = list[Program.GetRandom.Next(list.Length)];

                                    }
                                    uint Have = 0;
                                    if (selected.Quality == 2)
                                    {
                                        if (!client.MyArchives.AddRune(selected.IDItem))
                                        {
                                            Have = 1;
                                            Amount = 10;
                                        }
                                    }
                                    else
                                    {
                                        Have = 1;
                                        if (selected.ID >= 26 && selected.ID <= 28)
                                            Amount = 50;
                                        if (selected.ID >= 29 && selected.ID <= 31)
                                            Amount = selected.IDItem;
                                    }
                                    pQuery.Items.Add(new Item() { ItemID = selected.ID, Change = Have });
                                    if (Amount > 0)
                                    {
                                        client.MyArchives.UniversalFragment += Amount;
                                        MsgCombatGearTao.AddFragemnt(client);
                                    }


                                }
                                
                                client.Send(stream.CreatePirateLottery(pQuery));
                            }
                        }
                        #endregion
                        #region Evil Compass
                        else if (client.Inventory.Contain(3329550,10))
                        {
                            client.Inventory.RemoveStackItem(3329550, 10, stream);
                            uint Amount = 0;
                            pQuery.Items = new List<Item>();

                            for (int i = 0; i < 10; i++)
                            {
                                var daoqi = dict_lottery.DaoqiItem.Values.ToArray();
                                dict_lottery.item selected = null;

                                if (Role.Core.Rate(4.3))
                                {
                                    var list = daoqi.Where(p => p.ID >= 26 && p.ID <= 28).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(23.7))
                                {
                                    var list = daoqi.Where(p => p.ID >= 1 && p.ID <= 11).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(31.1))
                                {
                                    var list = daoqi.Where(p => p.ID >= 12 && p.ID <= 25).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else
                                {
                                    var list = daoqi.Where(p => p.ID >= 29 && p.ID <= 31).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];

                                }
                                uint Have = 0;
                                if (selected.Quality == 2)
                                {
                                    if (!client.MyArchives.AddRune(selected.IDItem))
                                    {
                                        Have = 1;
                                        Amount = 10;
                                    }
                                }
                                else
                                {
                                    Have = 1;
                                    if (selected.ID >= 26 && selected.ID <= 28)
                                        Amount = 50;
                                    if (selected.ID >= 29 && selected.ID <= 31)
                                        Amount = selected.IDItem;
                                }
                                pQuery.Switch = 1;
                                pQuery.Items.Add(new Item() { ItemID = selected.ID, Change = Have });
                                if (Amount > 0)
                                {
                                    client.MyArchives.UniversalFragment += Amount;
                                    MsgCombatGearTao.AddFragemnt(client);
                                }


                            }
                            
                            client.Send(stream.CreatePirateLottery(pQuery));
                        }
                        #endregion
                        break;
                    }
                default:
                    MyConsole.WriteLine("MsgPirateLottery Not Find: " + pQuery.Type);
                    break;
            }
        }
    }

}