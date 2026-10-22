// Decompiled with JetBrains decompiler
// Type: Qahr.Mobile.Login.Network.AuthPackets.PasswordCryptographySeed
// Assembly: AccServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FDFCA3C4-90EE-49AE-B6B0-6D3A101B9C51
// Assembly location: D:\شغل كونكر محمود حسن\شغلى مهم جدا\شغل سي شارب\سورسات السي شارب كامله\سورس اليكس\سورس اليكس 3 دى بورتو\AccServer.exe

using Qahr.Mobile.Login.Interfaces;
using System;

namespace Qahr.Mobile.Login.Network.AuthPackets
{
    public class PasswordCryptographySeed : IPacket
    {
        private byte[] Buffer;

        public PasswordCryptographySeed()
        {
            this.Buffer = new byte[8];
            Writer.WriteUInt16((ushort)8, 0, this.Buffer);
            Writer.WriteUInt16((ushort)1059, 2, this.Buffer);
        }

        public int Seed
        {
            get
            {
                return BitConverter.ToInt32(this.Buffer, 4);
            }
            set
            {
                Writer.WriteInt32(value, 4, this.Buffer);
            }
        }

        public void Deserialize(byte[] buffer)
        {
        }

        public byte[] ToArray()
        {
            return this.Buffer;
        }
    }
}
