using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.ServerSockets;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateWarriorSkin(this ServerSockets.Packet stream, MsgCombatGearSkin.Skin obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCombatGearSkin);
            return stream;
        }

        public static void GetWarriorSkin(this ServerSockets.Packet stream, out MsgCombatGearSkin.Skin pQuery)
        {
            pQuery = new MsgCombatGearSkin.Skin();
            pQuery = stream.ProtoBufferDeserialize<MsgCombatGearSkin.Skin>(pQuery);
        }
    }
    public static class MsgCombatGearSkin
    {
        [ProtoContract]
        public class Skin
        {
            [Flags]
            public enum TypeID
            {
                Gloryfate = 1,
                HolyOmen = 2,
            }
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public MsgCombatGearSkin.Skin.WeaponInfo[] Weapon;
            [ProtoContract]
            public class WeaponInfo
            {
                [ProtoMember(1, IsRequired = true)]
                public uint ID;
                [ProtoMember(2, IsRequired = true)]
                public MsgCombatGearSkin.Skin.TypeID Type;
                [ProtoMember(3, IsRequired = true)]
                public uint P1;
                [ProtoMember(4, IsRequired = true)]
                public uint P2;
                [ProtoMember(5, IsRequired = true)]
                public uint P3;
                [ProtoMember(6, IsRequired = true)]
                public uint P4;
                [ProtoMember(7, IsRequired = true)]
                public uint P5;
                [ProtoMember(8, IsRequired = true)]
                public uint P6;
                [ProtoMember(9, IsRequired = true)]
                public uint P7;
            }
            public static void Create(GameClient user, Archives.Item Item)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    ServerSockets.Packet stream = recycledPacket.GetStream();
                    MsgCombatGearSkin.Skin obj = new MsgCombatGearSkin.Skin();
                    obj.Type = 1;
                    obj.Weapon = new MsgCombatGearSkin.Skin.WeaponInfo[2];
                    obj.Weapon[0] = new MsgCombatGearSkin.Skin.WeaponInfo();
                    obj.Weapon[0].ID = (uint)Item.ItemID;
                    obj.Weapon[0].Type = MsgCombatGearSkin.Skin.TypeID.Gloryfate;
                    obj.Weapon[0].P1 = Item.Animas[0].AnimaID[0];
                    obj.Weapon[0].P2 = Item.Animas[0].AnimaID[1];
                    obj.Weapon[0].P3 = Item.Animas[0].AnimaID[2];
                    obj.Weapon[0].P4 = Item.Animas[0].AnimaID[3];
                    obj.Weapon[0].P5 = Item.Animas[0].AnimaID[4];
                    obj.Weapon[0].P6 = Item.Animas[0].AnimaID[5];
                    obj.Weapon[0].P7 = Item.Animas[0].AnimaID[6];
                    obj.Weapon[1] = new MsgCombatGearSkin.Skin.WeaponInfo();
                    obj.Weapon[1].ID = (uint)Item.ItemID;
                    obj.Weapon[1].Type = MsgCombatGearSkin.Skin.TypeID.HolyOmen;
                    obj.Weapon[1].P1 = Item.Animas[1].AnimaID[0];
                    obj.Weapon[1].P2 = Item.Animas[1].AnimaID[1];
                    obj.Weapon[1].P3 = Item.Animas[1].AnimaID[2];
                    obj.Weapon[1].P4 = Item.Animas[1].AnimaID[3];
                    obj.Weapon[1].P5 = Item.Animas[1].AnimaID[4];
                    obj.Weapon[1].P6 = Item.Animas[1].AnimaID[5];
                    obj.Weapon[1].P7 = Item.Animas[1].AnimaID[6];
                    user.Send(stream.CreateWarriorSkin(obj));
                }
            }

            public static void Create(GameClient user)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    ServerSockets.Packet stream = recycledPacket.GetStream();
                    MsgCombatGearSkin.Skin Skin = new MsgCombatGearSkin.Skin();
                    Skin.Type = 0;
                    int index1 = 0;
                    Skin.Weapon = new MsgCombatGearSkin.Skin.WeaponInfo[user.MyArchives.Items.Count * 2];
                    foreach (Archives.Item obj in (IEnumerable<Archives.Item>)user.MyArchives.Items.Values)
                    {
                        Skin.Weapon[index1] = new MsgCombatGearSkin.Skin.WeaponInfo();
                        Skin.Weapon[index1].ID = (uint)obj.ItemID;
                        Skin.Weapon[index1].Type = MsgCombatGearSkin.Skin.TypeID.Gloryfate;
                        Skin.Weapon[index1].P1 = obj.Animas[0].AnimaID[0];
                        Skin.Weapon[index1].P2 = obj.Animas[0].AnimaID[1];
                        Skin.Weapon[index1].P3 = obj.Animas[0].AnimaID[2];
                        Skin.Weapon[index1].P4 = obj.Animas[0].AnimaID[3];
                        Skin.Weapon[index1].P5 = obj.Animas[0].AnimaID[4];
                        Skin.Weapon[index1].P6 = obj.Animas[0].AnimaID[5];
                        Skin.Weapon[index1].P7 = obj.Animas[0].AnimaID[6];
                        int index2 = index1 + 1;
                        Skin.Weapon[index2] = new MsgCombatGearSkin.Skin.WeaponInfo();
                        Skin.Weapon[index2].ID = (uint)obj.ItemID;
                        Skin.Weapon[index2].Type = MsgCombatGearSkin.Skin.TypeID.HolyOmen;
                        Skin.Weapon[index2].P1 = obj.Animas[1].AnimaID[0];
                        Skin.Weapon[index2].P2 = obj.Animas[1].AnimaID[1];
                        Skin.Weapon[index2].P3 = obj.Animas[1].AnimaID[2];
                        Skin.Weapon[index2].P4 = obj.Animas[1].AnimaID[3];
                        Skin.Weapon[index2].P5 = obj.Animas[1].AnimaID[4];
                        Skin.Weapon[index2].P6 = obj.Animas[1].AnimaID[5];
                        Skin.Weapon[index2].P7 = obj.Animas[1].AnimaID[6];
                        index1 = index2 + 1;
                    }
                    user.Send(stream.CreateWarriorSkin(Skin));
                }
            }
        }

    }
}