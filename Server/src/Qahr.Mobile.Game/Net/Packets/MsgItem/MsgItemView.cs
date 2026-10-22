using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ItemViewCreate(this ServerSockets.Packet stream, uint PlayerUID, ulong Cost, MsgGameItem DateItem, MsgItemView.ActionMode Mode)
        {
            stream.InitWriter();

            uint calculatedRemainingTime = 0;

            if (!Database.ItemType.isRune(DateItem.ITEM_ID))
            {
                if (DateItem.TimeStamp != DateTime.MinValue && DateItem.TimeStamp > DateTime.Now)
                {
                    TimeSpan Remain = DateItem.TimeStamp - DateTime.Now;
                    calculatedRemainingTime = (uint)Remain.TotalSeconds;
                }
                else
                {
                    calculatedRemainingTime = DateItem.RemainingTime > 0 ? DateItem.RemainingTime : DateItem.TimeLeftInMinutes;
                }
            }

            var itemInfo = new MsgItemView.ItemInfo()
            {
                UID = DateItem.UID,
                PlayerUID = PlayerUID,
                Cost = Cost,
                ITEM_ID = DateItem.ITEM_ID,
                Durability = DateItem.Durability,
                MaximDurability = DateItem.MaximDurability,
                Position = (ushort)DateItem.Position,
                SocketProgress = DateItem.SocketProgress,
                SocketOne = (byte)DateItem.SocketOne,
                SocketTwo = (byte)DateItem.SocketTwo,
                SpellID = (uint)DateItem.SpellID,
                Effect = DateItem.Effect,
                Plus = DateItem.Plus,
                Bless = DateItem.Bless,
                Bound = DateItem.Bound,
                Enchant = DateItem.Enchant,
                ProgresGreen = (uint)DateItem.ProgresGreen,
                Suspicious = DateItem.Suspicious == 1,
                Locked = DateItem.Locked == 1,
                PlusProgress = DateItem.PlusProgress,
                Activate = DateItem.Activate,
                RemainingTime = calculatedRemainingTime,
                StackSize = DateItem.StackSize,
                PurificationItemID1 = DateItem.Purification.PurificationItemID,
                PurificationItemID2 = DateItem.Purification.PurificationItemID,
                PerfectionLevel = DateItem.PerfectionLevel,
                PerfectionProgress = DateItem.PerfectionProgress,
                OwnerUID = DateItem.OwnerUID,
                OwnerName = DateItem.OwnerName,
                Signature = DateItem.Signature,
                RuneProgress = Database.ItemType.isRune(DateItem.ITEM_ID) ? DateItem.PlusProgress : 0,
                AnimaItemID = DateItem.AnimaItemID,
                MythsoulID = DateItem.MythSoulID,
                MythsoulEXP = DateItem.MythSoulProgress,
                MythsoulID2 = DateItem.Mutacion,
                MythsoulID3 = DateItem.UpsurgeID,
                RelicAttribs = new List<uint>(),
                EonspiritActived = DateItem.EonspiritActived,
                YuanshenTime = DateItem.EonspiritTime,
            };

            for (int x = 0; x < Math.Max(5, DateItem.RelicAttributes.Length); x++)
            {
                if (x < DateItem.RelicAttributes.Length)
                    itemInfo.RelicAttribs.Add(DateItem.RelicAttributes[x]);
                else
                    itemInfo.RelicAttribs.Add(0);
            }

            var Info = new MsgItemView.MsgItemProto()
            {
                Action = Mode,
                Items = new List<MsgItemView.ItemInfo> { itemInfo }
            };

            stream.ProtoBufferSerialize(Info);
            stream.Finalize(GamePackets.MsgItemInfoEx);

            return stream;
        }
        public static unsafe ServerSockets.Packet BoothItemViewCreate(this ServerSockets.Packet stream, uint boothUID, ulong cost, MsgGameItem dataItem, MsgItemView.ActionMode mode)
        {
            if (dataItem == null)
                throw new ArgumentNullException("dataItem");

            stream.InitWriter();

            uint calculatedRemainingTime = 0;
            if (!Database.ItemType.isRune(dataItem.ITEM_ID))
            {
                if (dataItem.TimeStamp != DateTime.MinValue && dataItem.TimeStamp > DateTime.Now)
                {
                    TimeSpan remain = dataItem.TimeStamp - DateTime.Now;
                    calculatedRemainingTime = (uint)Math.Max(0, remain.TotalSeconds);
                }
                else
                {
                    calculatedRemainingTime = dataItem.RemainingTime > 0
                        ? dataItem.RemainingTime
                        : dataItem.TimeLeftInMinutes;
                }
            }

            var packet = new MsgItemView.PlayerShopPacket
            {
                Action = (uint)mode,
                Item = new MsgItemView.PlayerShopItemData
                {
                    Unknown1 = dataItem.UID,
                    Unknown2 = boothUID,
                    Unknown3 = cost,
                    Unknown4 = dataItem.ITEM_ID,
                    Unknown5 = dataItem.Durability,
                    Unknown6 = dataItem.MaximDurability,
                    Unknown7 = 0,
                    Unknown8 = dataItem.Position,
                    Unknown9 = dataItem.SocketProgress,
                    Unknown10 = (uint)dataItem.SocketOne,
                    Unknown11 = (uint)dataItem.SocketTwo,
                    Unknown12 = dataItem.SpellID,
                    Unknown13 = (uint)dataItem.Effect,
                    Unknown14 = dataItem.Plus,
                    Unknown15 = dataItem.Bless,
                    Unknown16 = dataItem.Bound,
                    Unknown17 = dataItem.Enchant,
                    Unknown18 = dataItem.ProgresGreen,
                    Unknown19 = dataItem.Suspicious,
                    Unknown20 = dataItem.Locked,
                    Unknown21 = dataItem.PlusProgress,
                    Unknown22 = dataItem.Activate,
                    Unknown23 = calculatedRemainingTime,
                    Unknown24 = dataItem.StackSize == 0 ? 1u : dataItem.StackSize,
                    Unknown25 = dataItem.Purification.PurificationItemID,
                    Unknown26 = dataItem.Purification.PurificationItemID,
                    Unknown27 = dataItem.PerfectionLevel,
                    Unknown28 = dataItem.PerfectionProgress,
                    Unknown29 = dataItem.OwnerUID,
                    Unknown30 = string.Empty, // [ProtoFix 2026-10-14] client field 30 = string (OwnerName); empty string = same bytes as old empty message
                    Unknown31 = string.Empty, // [ProtoFix 2026-10-14] client field 31 = string (Signature)
                    Unknown32 = Database.ItemType.isRune(dataItem.ITEM_ID) ? dataItem.PlusProgress : 0,
                    Unknown34 = dataItem.AnimaItemID,
                    Unknown35 = dataItem.MythSoulENERGY,
                    Unknown36 = dataItem.MythSoulID,
                    Unknown37 = dataItem.MythSoulProgress,
                    Unknown38 = 0,
                    Unknown39 = dataItem.Mutacion,
                    Unknown40 = dataItem.UpsurgeID,
                    Unknown41 = dataItem.EonspiritActived,
                    Unknown42 = dataItem.EonspiritTime,
                    Unknown43 = 0,
                    Unknown44 = 0,
                    Unknown45 = 0
                }
            };

            stream.ProtoBufferSerialize(packet);
            stream.Finalize(GamePackets.MsgItemInfoEx);
            return stream;
        }
    }

    public class MsgItemView
    {
        [ProtoContract]
        public sealed class PlayerShopPacket
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action { get; set; }

            [ProtoMember(2, IsRequired = true)]
            public PlayerShopItemData Item { get; set; }
        }

        [ProtoContract]
        public sealed class PlayerShopItemData
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Unknown1 { get; set; }
            [ProtoMember(2, IsRequired = true)]
            public uint Unknown2 { get; set; }
            [ProtoMember(3, IsRequired = true)]
            public ulong Unknown3 { get; set; }
            [ProtoMember(4, IsRequired = true)]
            public uint Unknown4 { get; set; }
            [ProtoMember(5, IsRequired = true)]
            public uint Unknown5 { get; set; }
            [ProtoMember(6, IsRequired = true)]
            public uint Unknown6 { get; set; }
            [ProtoMember(7, IsRequired = true)]
            public uint Unknown7 { get; set; }
            [ProtoMember(8, IsRequired = true)]
            public uint Unknown8 { get; set; }
            [ProtoMember(9, IsRequired = true)]
            public uint Unknown9 { get; set; }
            [ProtoMember(10, IsRequired = true)]
            public uint Unknown10 { get; set; }
            [ProtoMember(11, IsRequired = true)]
            public uint Unknown11 { get; set; }
            [ProtoMember(12, IsRequired = true)]
            public uint Unknown12 { get; set; }
            [ProtoMember(13, IsRequired = true)]
            public uint Unknown13 { get; set; }
            [ProtoMember(14, IsRequired = true)]
            public uint Unknown14 { get; set; }
            [ProtoMember(15, IsRequired = true)]
            public uint Unknown15 { get; set; }
            [ProtoMember(16, IsRequired = true)]
            public uint Unknown16 { get; set; }
            [ProtoMember(17, IsRequired = true)]
            public uint Unknown17 { get; set; }
            [ProtoMember(18, IsRequired = true)]
            public uint Unknown18 { get; set; }
            [ProtoMember(19, IsRequired = true)]
            public uint Unknown19 { get; set; }
            [ProtoMember(20, IsRequired = true)]
            public uint Unknown20 { get; set; }
            [ProtoMember(21, IsRequired = true)]
            public uint Unknown21 { get; set; }
            [ProtoMember(22, IsRequired = true)]
            public uint Unknown22 { get; set; }
            [ProtoMember(23, IsRequired = true)]
            public uint Unknown23 { get; set; }
            [ProtoMember(24, IsRequired = true)]
            public uint Unknown24 { get; set; }
            [ProtoMember(25, IsRequired = true)]
            public uint Unknown25 { get; set; }
            [ProtoMember(26, IsRequired = true)]
            public uint Unknown26 { get; set; }
            [ProtoMember(27, IsRequired = true)]
            public uint Unknown27 { get; set; }
            [ProtoMember(28, IsRequired = true)]
            public uint Unknown28 { get; set; }
            [ProtoMember(29, IsRequired = true)]
            public uint Unknown29 { get; set; }
            [ProtoMember(30, IsRequired = true)]
            public string Unknown30 { get; set; } = string.Empty; // [ProtoFix 2026-10-14] was EmptyMessage; client CMsgItemInfoExList.Unknown30 is string
            [ProtoMember(31, IsRequired = true)]
            public string Unknown31 { get; set; } = string.Empty; // [ProtoFix 2026-10-14] was EmptyMessage; client CMsgItemInfoExList.Unknown31 is string
            [ProtoMember(32, IsRequired = true)]
            public uint Unknown32 { get; set; }
            // Field 33 is intentionally absent in the captured client packet.
            [ProtoMember(34, IsRequired = true)]
            public uint Unknown34 { get; set; }
            [ProtoMember(35, IsRequired = true)]
            public uint Unknown35 { get; set; }
            [ProtoMember(36, IsRequired = true)]
            public uint Unknown36 { get; set; }
            [ProtoMember(37, IsRequired = true)]
            public uint Unknown37 { get; set; }
            [ProtoMember(38, IsRequired = true)]
            public ulong Unknown38 { get; set; } // [ProtoFix 2026-10-14] client uint64 (was uint)
            [ProtoMember(39, IsRequired = true)]
            public uint Unknown39 { get; set; }
            [ProtoMember(40, IsRequired = true)]
            public uint Unknown40 { get; set; }
            [ProtoMember(41, IsRequired = true)]
            public uint Unknown41 { get; set; }
            [ProtoMember(42, IsRequired = true)]
            public uint Unknown42 { get; set; }
            [ProtoMember(43, IsRequired = true)]
            public ulong Unknown43 { get; set; } // [ProtoFix 2026-10-14] client uint64 (was uint)
            [ProtoMember(44, IsRequired = true)]
            public uint Unknown44 { get; set; }
            [ProtoMember(45, IsRequired = true)]
            public ulong Unknown45 { get; set; } // [ProtoFix 2026-10-14] client uint64 (was uint)
        }

        [ProtoContract]
        public sealed class EmptyMessage
        {
        }

        [ProtoContract]
        public class MsgItemProto
        {
            [ProtoMember(1, IsRequired = true)]
            public MsgItemView.ActionMode Action;

            [ProtoMember(2)]
            public List<ItemInfo> Items;
        }

        [ProtoContract]
        public class ItemInfo
        {
            [ProtoMember(1)]
            public uint UID;
            [ProtoMember(2)]
            public uint PlayerUID;
            [ProtoMember(3)]
            public ulong Cost;
            [ProtoMember(4)]
            public uint ITEM_ID;
            [ProtoMember(5)]
            public ushort Durability;
            [ProtoMember(6)]
            public ushort MaximDurability;
            [ProtoMember(7)]
            public uint Unknown7; // [ProtoFix 2026-10-14] client uint32 (was int)
            [ProtoMember(8)]
            public ushort Position;
            [ProtoMember(9)]
            public uint SocketProgress;
            [ProtoMember(10)]
            public byte SocketOne;
            [ProtoMember(11)]
            public byte SocketTwo;
            [ProtoMember(12)]
            public uint SpellID;
            [ProtoMember(13)]
            public Role.Flags.ItemEffect Effect;
            [ProtoMember(14)]
            public byte Plus;
            [ProtoMember(15)]
            public byte Bless;
            [ProtoMember(16)]
            public byte Bound;
            [ProtoMember(17)]
            public byte Enchant;
            [ProtoMember(18)]
            public uint ProgresGreen;
            [ProtoMember(19)]
            public bool Suspicious;
            [ProtoMember(20)]
            public bool Locked;
            [ProtoMember(21)]
            public uint PlusProgress;
            [ProtoMember(22)]
            public uint Activate;
            [ProtoMember(23)]
            public uint RemainingTime;
            [ProtoMember(24)]
            public ushort StackSize;
            [ProtoMember(25)]
            public uint PurificationItemID1;
            [ProtoMember(26)]
            public uint PurificationItemID2;
            [ProtoMember(27)]
            public uint PerfectionLevel;
            [ProtoMember(28)]
            public uint PerfectionProgress;
            [ProtoMember(29)]
            public uint OwnerUID;
            [ProtoMember(30)]
            public string OwnerName;
            [ProtoMember(31)]
            public string Signature;
            [ProtoMember(32)]
            public uint RuneProgress;
            [ProtoMember(33)]
            public List<uint> RelicAttribs;
            [ProtoMember(34)]
            public uint AnimaItemID;
            [ProtoMember(35)]
            public uint MythsoulEffect;
            [ProtoMember(36)]
            public uint MythsoulID;
            [ProtoMember(37)]
            public uint MythsoulEXP;
            [ProtoMember(38)]
            public ulong MythEnergy; // [ProtoFix 2026-10-14] client uint64 (was uint)
            [ProtoMember(39)]
            public uint MythsoulID2;
            [ProtoMember(40)]
            public uint MythsoulID3;
            [ProtoMember(41)]
            public uint EonspiritActived;
            [ProtoMember(42)]
            public uint YuanshenTime;
            [ProtoMember(43)]
            public ulong Unknown43; // [ProtoFix 2026-10-14] client uint64 (was uint)
            [ProtoMember(44)]
            public uint Unknown44;
            [ProtoMember(45)]
            public ulong Unknown45; // [ProtoFix 2026-10-14] client uint64 (was uint)
        }

        public enum ActionMode : ushort
        {
            Gold = 1,
            CPs = 3,
            ViewEquip = 4,
            Rune = 7,
            Yuanshen = 8,
            RelicFuseNew = 9,
        }
    }
}