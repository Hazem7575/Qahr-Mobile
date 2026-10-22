using ProtoBuf;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgHundredWeaponsInfo
    {

        [ProtoContract]
        public class MsgHundredWeaponsInfoProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2, IsRequired = true)]
            public uint OwnerUID;
            [ProtoMember(3, IsRequired = true)]
            public WeaponInfo[] Weapons;
        }
        [ProtoContract]
        public class WeaponInfo
        {
            [ProtoMember(1, IsRequired = true)]
            public Database.MagicType.WeaponsType SubType;
            [ProtoMember(2, IsRequired = true)]
            public uint AppearancePosition;
            [ProtoMember(3, IsRequired = true)]
            public uint PositionFlags;
            [ProtoMember(4, IsRequired = true)]
            public byte Level;
            [ProtoMember(5, IsRequired = true)]
            public uint Progress;
            [ProtoMember(6, IsRequired = true)]
            public ulong OldWeaponHP = 0;
            [ProtoMember(7, IsRequired = true)]
            public ulong OldWeaponPAttack = 0;
            [ProtoMember(8, IsRequired = true)]
            public ulong OldWeaponPDefanse = 0;
            [ProtoMember(9, IsRequired = true)]
            public ulong NewWeaponHP = 0;
            [ProtoMember(10, IsRequired = true)]
            public ulong NewWeaponPAttack = 0;
            [ProtoMember(11, IsRequired = true)]
            public ulong NewWeaponPDefanse = 0;
            [ProtoMember(12, IsRequired = true)]
            public uint Score;
        }
        public enum ActionID : int
        {
            Add,
            Update,
            RequestInfo,
            QueryOne, // [Unanswered B4 2026-10-14] client type 3: one weapon's detail (Weapons[0] must exist), posts 0xf80
        }
        public static unsafe ServerSockets.Packet CreateHundredWeaponsInfoNPC(this ServerSockets.Packet stream, Client.GameClient client, Database.MagicType.WeaponsType WeaponSubtype, ActionID type)
        {
            MsgHundredWeaponsInfoProto msg = new MsgHundredWeaponsInfoProto() { Type = type };
            if (client != null)
            {
                msg.OwnerUID = client.Player.UID;
                msg.Weapons = new WeaponInfo[1];
                var obj = client.HundredWeapons.Objects.Values.Where(w => w.WeaponSubtype == WeaponSubtype).ToArray();
                if (obj.Length > 0)
                {
                    msg.Weapons[0] = new WeaponInfo()
                    {
                        SubType = obj.FirstOrDefault().WeaponSubtype,
                        Progress = obj.FirstOrDefault().Progress,
                        Level = obj.FirstOrDefault().Level,
                        PositionFlags = obj.FirstOrDefault().BitVector.bits[0],
                        AppearancePosition = obj.FirstOrDefault().AppearancePosition,
                        Score = obj.FirstOrDefault().Score,
                    };
                }
            }
            stream.InitWriter();
            stream.ProtoBufferSerialize(msg);
            stream.Finalize(GamePackets.MsgHundredWeaponsInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateHundredWeaponsInfo(this ServerSockets.Packet stream, Role.Instance.HundredWeaponInfo obj, Client.GameClient client, ActionID type)
        {
            MsgHundredWeaponsInfoProto msg = new MsgHundredWeaponsInfoProto();
            msg.OwnerUID = client.Player.UID;
            msg.Weapons = new WeaponInfo[1];
            msg.Type = type;
            msg.Weapons[0] = new WeaponInfo();
            msg.Weapons[0].SubType = obj.WeaponSubtype;
            msg.Weapons[0].Progress = obj.Progress;
            msg.Weapons[0].Level = obj.Level;

            msg.Weapons[0].OldWeaponHP = (ulong)((1 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.Hitpoints]);
            msg.Weapons[0].OldWeaponPAttack = (ulong)((2 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack]);
            msg.Weapons[0].OldWeaponPDefanse = (ulong)((3 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense]);

            if (obj.TampPhysicalDefense != 0)
                msg.Weapons[0].NewWeaponHP = (ulong)((1 * 10000) + obj.TampHitpoints);
            if (obj.TampPhysicalDefense != 0)
                msg.Weapons[0].NewWeaponPAttack = (ulong)((2 * 10000) + obj.TampPhysicalAttack);
            if (obj.TampPhysicalDefense != 0)
                msg.Weapons[0].NewWeaponPDefanse = (ulong)((3 * 10000) + obj.TampPhysicalDefense);

            msg.Weapons[0].PositionFlags = obj.BitVector.bits[0];
            msg.Weapons[0].AppearancePosition = obj.AppearancePosition;
            msg.Weapons[0].Score = obj.Score;
            stream.InitWriter();
            stream.ProtoBufferSerialize(msg);
            stream.Finalize(GamePackets.MsgHundredWeaponsInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateHundredWeaponsInfo(this ServerSockets.Packet stream, Client.GameClient client, ActionID type)
        {
            MsgHundredWeaponsInfoProto msg = new MsgHundredWeaponsInfoProto() { Type = type };
            if (client != null)
            {
                msg.OwnerUID = client.Player.UID;
                msg.Weapons = new WeaponInfo[client.HundredWeapons.Objects.Count];
                for (int i = 0; i < msg.Weapons.Length; i++)
                {
                    var obj = client.HundredWeapons.Objects.Values.OrderBy(w => (ushort)w.WeaponSubtype).ToArray()[i];
                    msg.Weapons[i] = new WeaponInfo();
                    msg.Weapons[i].SubType = obj.WeaponSubtype;
                    msg.Weapons[i].Progress = obj.Progress;
                    msg.Weapons[i].Level = obj.Level;
                    if (obj.Attributes[Database.HundredWeapons.AttributeType.Hitpoints] != 0)
                        msg.Weapons[i].OldWeaponHP = (ulong)((1 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.Hitpoints]);
                    if (obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] != 0)
                        msg.Weapons[i].OldWeaponPAttack = (ulong)((2 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack]);
                    if (obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense] != 0)
                        msg.Weapons[i].OldWeaponPDefanse = (ulong)((3 * 10000) + obj.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense]);


                    if (obj.TampPhysicalDefense != 0)
                        msg.Weapons[i].NewWeaponHP = (ulong)((1 * 10000) + obj.TampHitpoints);
                    if (obj.TampPhysicalDefense != 0)
                        msg.Weapons[i].NewWeaponPAttack = (ulong)((2 * 10000) + obj.TampPhysicalAttack);
                    if (obj.TampPhysicalDefense != 0)
                        msg.Weapons[i].NewWeaponPDefanse = (ulong)((3 * 10000) + obj.TampPhysicalDefense);
                    msg.Weapons[i].PositionFlags = obj.BitVector.bits[0];
                    msg.Weapons[i].AppearancePosition = obj.AppearancePosition;
                    msg.Weapons[i].Score = obj.Score;

                }
            }
            stream.InitWriter();
            stream.ProtoBufferSerialize(msg);
            stream.Finalize(GamePackets.MsgHundredWeaponsInfo);
            return stream;
        }
    }
}
