using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgEquipRefineRank
    {
        [ProtoContract]
        public class EquipRefineRank
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action;
            [ProtoMember(2)]
            public uint RegisteredCount;
            [ProtoMember(3)]
            public uint Page;
            [ProtoMember(4, IsRequired = true)]
            public uint Index;
            [ProtoMember(5, IsRequired = true)]
            public Equip[] items;
        }
        [ProtoContract]
        public class Equip
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Rank;
            [ProtoMember(2, IsRequired = true)]
            public uint UnKnow2;
            [ProtoMember(3, IsRequired = true)]
            public uint Position;
            [ProtoMember(4, IsRequired = true)]
            public uint RankScore;
            [ProtoMember(5, IsRequired = true)]
            public uint UID;
            [ProtoMember(6, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(7, IsRequired = true)]
            public uint PurificationID;
            [ProtoMember(8, IsRequired = true)]
            public uint Plus;
            [ProtoMember(9, IsRequired = true)]
            public uint PerfectionLevel;
            [ProtoMember(10, IsRequired = true)]
            public string Name = "";
        }
        public static ServerSockets.Packet MsgEquipRankCreate(this ServerSockets.Packet stream, EquipRefineRank obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgEquipRefineRank);
            return stream;

        }
        [Flags]
        public enum ActionID
        {
            MainRank = 0,
            RankItems = 1,
            UserItemRanking = 2,
            MyRanks = 3,
            View = 4,

        }

        public static void GetMsgEquipRefineRank(this ServerSockets.Packet stream, out EquipRefineRank obj)
        {
            obj = new EquipRefineRank();
            obj = stream.ProtoBufferDeserialize<EquipRefineRank>(obj);
        }
        [PacketAttribute(GamePackets.MsgEquipRefineRank)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            EquipRefineRank msg;
            stream.GetMsgEquipRefineRank(out msg);


            var perfectionitems = user.Equipment.CurentEquip.Where(p => p.OwnerUID != 0).ToArray();
            if (perfectionitems.Length > 0)
            {
                var itemBig = perfectionitems.OrderBy(p => p.PerfectionLevel).Last();

                var pQuery = new MsgItemRefineRecord.ItemRefineRecord();
                pQuery.Type = 0;//update stars
                pQuery.Points = 54;

                user.Send(stream.ItemRefineRecordCreate(pQuery));

                pQuery = new MsgItemRefineRecord.ItemRefineRecord();
                pQuery.Type = 1;
                pQuery.Points = Math.Max(1, itemBig.PerfectionLevel / 9);
                pQuery.msgs = new MsgItemRefineRecord.RefineRecord[perfectionitems.Length];
                uint index = 0;
                foreach (var item in user.Equipment.CurentEquip)
                {
                    if (item.OwnerUID != 0)
                    {
                        if (item.PerfectionStage == itemBig.PerfectionStage)
                        {
                            pQuery.Unlocked += item.PerfectionStageStars;
                        }
                        pQuery.msgs[index] = new MsgItemRefineRecord.RefineRecord();
                        pQuery.msgs[index].Index = (uint)(index + 1);
                        pQuery.msgs[index].ItemID = item.ITEM_ID;
                        pQuery.msgs[index].ItemUID = item.UID;
                        pQuery.msgs[index].OpenStars = item.PerfectionLevel;
                        pQuery.msgs[index].RequestStars = (uint)Math.Min(54, (item.PerfectionLevel / 9 + 1) * 9);
                        pQuery.msgs[index].str = item.OwnerName;
                        index++;
                    }
                }
                pQuery.Unlocked = Math.Min(pQuery.Unlocked, 20);
                user.Send(stream.ItemRefineRecordCreate(pQuery));
            }
            switch ((ActionID)msg.Action)
            {
                case ActionID.View:
                    {
                        foreach (var Rank in Database.RankItems.RankPoll.Values)
                        {
                            MsgGameItem item;
                            if (Rank.Items.TryGetValue(msg.Index, out item))
                            {
                                item.Mode = Role.Flags.ItemMode.ChatItem;
                                item.Send(user, stream);
                                item.Mode = Role.Flags.ItemMode.AddItem;
                                break;
                            }

                        }
                        break;
                    }
                case ActionID.UserItemRanking:
                    {
                        MsgGameItem item = user.Equipment.CurentEquip.Where(p => p.OwnerUID != 0 && p.GetPerfectionPosition == msg.Index).FirstOrDefault();
                        if (item != null)
                        {
                            msg = new EquipRefineRank();
                            msg.Action = (uint)ActionID.UserItemRanking;
                            msg.items = new Equip[1];
                            msg.items[0] = new Equip();
                            var element = msg.items[0];
                            element.ItemID = item.ITEM_ID;
                            element.Name = item.OwnerName;
                            element.PerfectionLevel = item.PerfectionLevel;
                            element.Plus = item.Plus;
                            element.Position = (uint)item.GetPerfectionPosition;
                            element.PurificationID = item.Purification.PurificationItemID;
                            element.Rank = item.PerfectionRank;
                            element.RankScore = item.GetPrestigeScore;
                            element.UID = item.UID;
                            user.Send(stream.MsgEquipRankCreate(msg));
                        }
                        break;
                    }
                case ActionID.RankItems:
                    {
                        uint position = msg.Index;
                        uint page = msg.Page;
                        msg = new EquipRefineRank();
                        var RankTable = Database.RankItems.RankPoll[Math.Max(1, Math.Min(position, 11))];
                        if (RankTable.Items.Count == 0)
                            break;
                        var rank = RankTable.GetRank50Items();

                        const int max = 10;
                        int offset = (int)page * max;
                        int count = Math.Min(max, rank.Length);

                        msg.RegisteredCount = (uint)Math.Min(rank.Length, 50);
                        msg.Action = (uint)ActionID.RankItems;
                        msg.Page = page;


                        msg.items = new Equip[count];
                        for (int x = 0; x < count; x++)
                        {

                            if (x + offset >= rank.Length)
                                break;
                            var item = rank[x + offset];
                            var element = msg.items[x] = new Equip();
                            if (item != null)
                            {
                                element.ItemID = item.ITEM_ID;
                                element.Name = item.OwnerName;
                                element.PerfectionLevel = item.PerfectionLevel;
                                element.Plus = item.Plus;
                                element.UnKnow2 = 1;
                                element.Position = (uint)item.GetPerfectionPosition;
                                element.PurificationID = item.Purification.PurificationItemID;
                                element.Rank = (ushort)(offset + x + 1);
                                element.RankScore = item.GetPrestigeScore;
                                element.UID = item.UID;
                            }
                        }
                        user.Send(stream.MsgEquipRankCreate(msg));

                        break;
                    }
                case ActionID.MyRanks:
                    {
                        msg.items = new Equip[11];

                        for (int x = 0; x < 11; x++)
                            msg.items[x] = new Equip();
                        ushort count = 0;
                        foreach (var item in user.Equipment.CurentEquip)
                        {

                            if (item.GetPerfectionPosition != -1 && (item.PerfectionProgress > 0 || item.PerfectionLevel > 0))
                            {
                                count = (ushort)item.GetPerfectionPosition;
                                var element = msg.items[count - 1];
                                element.ItemID = item.ITEM_ID;
                                element.Name = item.OwnerName;
                                element.PerfectionLevel = item.PerfectionLevel;
                                element.Plus = item.Plus;
                                element.Position = (uint)item.GetPerfectionPosition;
                                element.PurificationID = item.Purification.PurificationItemID;
                                element.Rank = item.PerfectionRank;
                                element.RankScore = item.GetPrestigeScore;
                                element.UID = item.UID;
                            }
                        }
                        user.Send(stream.MsgEquipRankCreate(msg));
                        break;
                    }
                case ActionID.MainRank:
                    {
                        msg = new EquipRefineRank();
                        msg.items = new Equip[11];
                        for (int x = 0; x < 11; x++)
                        {
                            var element = msg.items[x] = new Equip();
                            var RankTable = Database.RankItems.RankPoll[(uint)(x + 1)];
                            if (RankTable.Items.Count == 0)
                                continue;
                            var item = RankTable.GetRank50Items()[0];
                            if (item != null)
                            {
                                element.ItemID = item.ITEM_ID;
                                element.Name = item.OwnerName;
                                element.PerfectionLevel = item.PerfectionLevel;
                                element.Plus = item.Plus;

                                element.Position = (uint)item.GetPerfectionPosition;
                                element.PurificationID = item.Purification.PurificationItemID;
                                element.Rank = (ushort)(x + 1);
                                element.RankScore = item.GetPrestigeScore;
                                element.UID = item.UID;
                            }
                        }
                        user.Send(stream.MsgEquipRankCreate(msg));
                        break;
                    }
            }

        }
    }
}
