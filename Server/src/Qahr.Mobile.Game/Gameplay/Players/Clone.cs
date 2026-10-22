using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role
{
    public unsafe class Clone
    {
        public void RemoveThat(Client.GameClient _Owner)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = this.UID,
                    Type = ActionType.RemoveEntity
                };
                Owner.Player.View.SendView(stream.ActionCreate(action), true);
            }
        }
        public System.BitVector32 BitVector;
        public uint UID = 0;
        public Client.GameClient Owner;
        public static System.Counter CounterUID = new System.Counter(700100);
        public void AddFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (!BitVector.Contain((int)Flag))
            {
                BitVector.Add((int)Flag);
                UpdateSpawnPacket();
            }
        }
        public void RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.Contain((int)Flag))
            {
                BitVector.Remove((int)Flag);
                UpdateSpawnPacket();
            }
        }
        public void UpdateSpawnPacket()
        {
            SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Owner.Send(stream);
            }
        }
        public static void CreateShadowClone(Role.Player client, ServerSockets.Packet stream)
        {
            CreateShadowClone(client, 3, stream);
            CreateShadowClone(client, 10003, stream);
        }
        public static void CreateShadowClone(Role.Player client, int flag, ServerSockets.Packet stream)
        {
            client.MyClones.Add(new Clone(client, "ShadowClone", flag, stream));
        }
        public int cloneflag = 0;
        public string clonename = "";
        public Clone(Role.Player role, string CloneName, int flag, ServerSockets.Packet stream)
        {
            BitVector = new System.BitVector32(32 * 18);
            Owner = role.Owner;
            cloneflag = flag;
            clonename = CloneName;
            UID = CounterUID.Next;
            SendView(role.Owner, stream);
            AddFlag(Game.MsgServer.MsgUpdate.Flags.Invisibility);
        }
        public void SendView(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Player.View.SendView(GetArray(stream), true);
        }
        public void Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(GetArray(stream));
        }
        public ServerSockets.Packet GetArray(ServerSockets.Packet stream)
        {
            stream.InitWriter();
            var proto = new Role.Player.SpawnPacketProto()
            {
                UID = UID,
                Mesh = Owner.Player.Mesh,
                Head = Owner.Player.HeadId,
                Garment = Owner.Player.GarmentId,
                LeftWeapon = Owner.Player.LeftWeaponId,
                LeftWeaponSoul = Owner.Player.LeftWeapsonSoul,
                RightWeapon = Owner.Player.RightWeaponId,
                RightWeaponSoul = Owner.Player.RightWeapsonSoul,
                LeftWeaponAccessory = Owner.Player.LeftWeaponAccessoryId,
                RightWeaponAccessory = Owner.Player.RightWeaponAccessoryId,
                Steed = Owner.Player.SteedId,
                MountArmor = Owner.Player.MountArmorId,
                AppearanceType = (ushort)Owner.Player.AparenceType,
                GuildRank = (uint)Owner.Player.GuildRank,
                GuildID = Owner.Player.GuildID,
                Wing = Owner.Player.WingId,
                WingPlus = Owner.Player.WingPlus,
                WingProgress = Owner.Player.WingProgress,
                Bottle = Owner.Player.Bottle,
                Hitpoints = (uint)Owner.Player.HitPoints,
                X = Owner.Player.X,
                Y = Owner.Player.Y,
                HairStyle = Owner.Player.Hair,
                Facing = (byte)Owner.Player.Angle,
                Action = (ushort)Owner.Player.Action,
                Reborn = Owner.Player.Reborn,
                SecondRebornClass = Owner.Player.SecoundeClass,
                FirstRebornClass = Owner.Player.FirstClass,
                Level = Owner.Player.Level,
                ExtraBattlePower = Owner.Player.ExtraBattlePower,
                FlowerIcon = -1,
                NobilityRank = (byte)Owner.Player.NobilityRank,
                Armor = Owner.Player.ArmorId,
                ArmorSoul = Owner.Player.ArmorSoul,
                HeadSoul = Owner.Player.HeadSoul,
                SteedColor = Owner.Player.SteedColor,
                SteedPlus = Owner.Player.SteedPlus,
                ClanUID = Owner.Player.ClanUID,
                ClanRank = Owner.Player.ClanRank,
                Title = Owner.Player.MyTitle,
                ActiveSubClasses = (byte)Owner.Player.ActiveSublass,
                SubClass = Owner.Player.SubClassHasPoints,
                JiangHuActive = Owner.Player.JiangHuActive > 0,
                JiangHuTalent = Owner.Player.JiangHuTalent,
                MaxLife = Owner.Status.MaxHitpoints,
                OwnerUID = Owner.Player.UID,
                BattlePower = Owner.Player.BattlePower,
                Class = Owner.Player.Class,
                MainFlag = (byte)Owner.Player.MainFlag,
                OwnerPet = 2,
                OwnerPet1 = (ushort)cloneflag,
                Name = System.Text.Encoding.UTF8.GetBytes(Owner.Player.Name ?? ""),
                Name134 = System.Text.Encoding.UTF8.GetBytes("ShadowClone(" + Owner.Player.Name + ")")
            };
            proto.StatusFlags = new ulong[BitVector.bits.Length];
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(Game.GamePackets.MsgPlayer);
            return stream;
        }
    }
}