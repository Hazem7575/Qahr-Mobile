using System;
using System.IO;
using System.Text;
using Qahr.Mobile.Login.Network.Cryptography;

namespace Qahr.Mobile.Login.Network.AuthPackets
{
    public class Authentication : Interfaces.IPacket
    {
        public string Username;
        public string Password;
        public string Server;
        public string Mac;

        public Authentication()
        {
        }
        public void Deserialize(byte[] buffer)
        {
            ushort length = BitConverter.ToUInt16(buffer, 0);
            ushort type = BitConverter.ToUInt16(buffer, 2);
            if (type == 1999)
            {
                MemoryStream MS = new MemoryStream(buffer);
                BinaryReader BR = new BinaryReader(MS);
                MS.Seek(4, SeekOrigin.Current);
                Username = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(16)).Replace("\0", "");
                Password = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(32)).Replace("\0", "");
                Server = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(32)).Replace("\0", "");
                Mac = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(16)).Replace("\0", "");
                BR.Close();
                MS.Close();
            }
        }

        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
    }
}
