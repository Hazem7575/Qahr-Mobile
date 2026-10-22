using System;
using System.Text;
using Qahr.Mobile.Login.Auth;

namespace Qahr.Mobile.Login.Network.AuthPackets
{
    public unsafe class Forward : Interfaces.IPacket
    {
        public enum ForwardType : byte
        {
            Ready = 2,
            InvalidInfo = 1,
            WrongAccount = 57,
            ServersNotConfigured = 59,
            InvalidAuthenticationProtocol = 73,
            Banned = 25,
            Banned_12 = 12
        }
        private byte[] Buffer;
        public Forward()
        {
            Buffer = new byte[52];
            Network.Writer.WriteUInt16(52, 0, Buffer);
            Network.Writer.WriteUInt16(1637, 2, Buffer);
        }
        public uint Identifier
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Network.Writer.WriteUInt32(value, 4, Buffer); }
        }
        public uint State
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { Network.Writer.WriteUInt32(value, 12, Buffer); }
        }
        public ForwardType Type
        {
            get { return (ForwardType)(byte)BitConverter.ToUInt32(Buffer, 12); }
            set { Network.Writer.WriteUInt32((byte)value, 12, Buffer); }
        }
        public ushort Port
        {
            get { return BitConverter.ToUInt16(Buffer, 16); }
            set { Network.Writer.WriteUInt16(value, 16, Buffer); }
        }
        public string IP
        {
            get { return global::Qahr.Mobile.Login.AnsiText.Value.GetString(Buffer, 24, 16); }
            set { Network.Writer.WriteString(value, 24, Buffer); }
        }
        public void Deserialize(byte[] buffer)
        {
        }
        public byte[] ToArray()
        {
            TQCipher.HandleBuffer(ref Buffer, false);
            return Buffer;
        }
    }
}