using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        [ProtoContract]
        public class MsgAuraProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;

            [ProtoMember(2, IsRequired = true)]
            public uint UID;

            [ProtoMember(3, IsRequired = true)]
            public MsgFlagIcon.ShowIcon Icon;

            [ProtoMember(4, IsRequired = true)]
            public uint Level;

            [ProtoMember(5, IsRequired = true)]
            public uint Mode;

            [ProtoMember(6, IsRequired = true)]
            public uint Damage;
        }

        public static unsafe void GetFlagIcon(this ServerSockets.Packet stream, out MsgAuraProto proto)
        {
            proto = new MsgAuraProto();
            proto = stream.ProtoBufferDeserialize<MsgAuraProto>(proto);
        }

        public static unsafe ServerSockets.Packet FlagIconCreate(this ServerSockets.Packet stream, uint UID, MsgFlagIcon.ShowIcon Icon
            , uint Level, uint Damage)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(new MsgAuraProto() { Type = 3, UID = UID, Icon = Icon, Level = Level, Mode = 30, Damage = Damage });
            stream.Finalize(GamePackets.MsgAura);

            return stream;
        }
    }

    public struct MsgFlagIcon
    {
        public enum ShowIcon : uint
        {
            TyrantAura = 1,
            FendAura = 2,
            MetalAura = 3,
            WoodAura = 4,
            WaterAura = 5,
            FireAura = 6,
            EarthAura = 7,
            MagicDefender = 8,
            NobleSpirit = 9,
            WackeSpirit = 10,
        }

        [PacketAttribute(GamePackets.MsgAura)]
        private static unsafe void MsgFlagIconHandler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgBuilder.MsgAuraProto proto;
            stream.GetFlagIcon(out proto);
            user.Send(stream.FlagIconCreate(proto.UID, proto.Icon, proto.Level, proto.Damage));
        }
    }
}