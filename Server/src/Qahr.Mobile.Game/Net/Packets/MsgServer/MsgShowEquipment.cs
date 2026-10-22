using Qahr.Mobile.Game;
using Qahr.Mobile.ServerSockets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        [ProtoContract]
        public class ShowEquipmentAction
        {
            [ProtoMember(1, IsRequired = true)]
            public uint wParam;
            [ProtoMember(2)]
            public ulong up2; // [ProtoFix 2026-10-14] client CMsgItemPB #2 is uint64 (was uint)
            [ProtoMember(3)]
            public uint Alternante;//لبس
            [ProtoMember(4)]
            public uint up4;
            [ProtoMember(5)]
            public uint up5;
            [ProtoMember(6)]
            public uint UID;//d
            [ProtoMember(7)]
            public uint up7;
            [ProtoMember(8)]
            public uint up8;
            [ProtoMember(9)]
            public uint up9;
            [ProtoMember(10)]
            public uint up10;
            [ProtoMember(11)]
            public uint Head;//d
            [ProtoMember(12)]
            public uint Necklace;//d
            [ProtoMember(13)]
            public uint Armor;//d
            [ProtoMember(14)]
            public uint RightWeapon;//d
            [ProtoMember(15)]
            public uint LeftWeapon;//d
            [ProtoMember(16)]
            public uint Ring;//d
            [ProtoMember(17)]
            public uint Bottle;//d
            [ProtoMember(18)]
            public uint Boots;//d
            [ProtoMember(19)]
            public uint Garment;//d
            [ProtoMember(20)]
            public uint RightWeaponAccessory;//d
            [ProtoMember(21)]
            public uint LeftWeaponAccessory;//d
            [ProtoMember(22)]
            public uint SteedMount;//d
            [ProtoMember(23)]
            public uint RidingCrop;//d
            [ProtoMember(24)]
            public uint Wing;//d
            [ProtoMember(25)]
            public uint Relic;//d

        }
        public static unsafe ServerSockets.Packet ShowEquipmentCreate(this ServerSockets.Packet stream, MsgShowEquipment item)
        {
            stream.InitWriter();
            var data = new ShowEquipmentAction()
            {
                UID = item.UID,
                wParam = item.wParam,
                Alternante = item.Alternante,
                Head = item.Head,
                Necklace = item.Necklace,
                Armor = item.Armor,
                RightWeapon = item.RightWeapon,
                LeftWeapon = item.LeftWeapon,
                Ring = item.Ring,
                Bottle = item.Bottle,
                Boots = item.Boots,
                Garment = item.Garment,
                RightWeaponAccessory = item.RightWeaponAccessory,
                LeftWeaponAccessory = item.LeftWeaponAccessory,
                SteedMount = item.SteedMount,
                RidingCrop = item.RidingCrop,
                Wing = item.Wing,
                Relic = item.Relic,
            };
            stream.ProtoBufferSerialize(data);
            stream.Finalize(GamePackets.MsgItem);

            return stream;
        }
    }

    public class MsgShowEquipment
    {
        public const ushort AlternanteAllow = 44,
    Show = 46;

        public int Stamp;
        public uint wParam;
        public uint Alternante;
        public ushort UID;
        public uint Head;
        public uint Necklace;
        public uint Armor;
        public uint RightWeapon;
        public uint LeftWeapon;
        public uint Ring;
        public uint Bottle;
        public uint Boots;
        public uint Garment;
        public uint RightWeaponAccessory;
        public uint LeftWeaponAccessory;
        public uint SteedMount;
        public uint RidingCrop;
        public uint Wing;
        public uint Relic;

    }
}
