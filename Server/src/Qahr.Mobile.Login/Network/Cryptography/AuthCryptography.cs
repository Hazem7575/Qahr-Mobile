// Decompiled with JetBrains decompiler
// Type: Qahr.Mobile.Login.Network.Cryptography.AuthCryptography
// Assembly: AccServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FDFCA3C4-90EE-49AE-B6B0-6D3A101B9C51
// Assembly location: D:\شغل كونكر محمود حسن\شغلى مهم جدا\شغل سي شارب\سورسات السي شارب كامله\سورس اليكس\سورس اليكس 3 دى بورتو\AccServer.exe

using System;

namespace Qahr.Mobile.Login.Network.Cryptography
{
    public class AuthCryptography
    {
        private static bool Decrypt2 = false;
        private AuthCryptography.CryptCounter _decryptCounter;
        private AuthCryptography.CryptCounter _encryptCounter;
        private static byte[] _cryptKey1;
        private static byte[] _cryptKey2;
        private static byte[] _cryptKey3;
        private static byte[] _cryptKey4;

        public static void PrepareAuthCryptography()
        {
            if (AuthCryptography._cryptKey1 != null && AuthCryptography._cryptKey1.Length != 0)
                return;
            AuthCryptography._cryptKey1 = new byte[256];
            AuthCryptography._cryptKey2 = new byte[256];
            byte num1 = 157;
            byte num2 = 98;
            for (int index = 0; index < 256; ++index)
            {
                AuthCryptography._cryptKey1[index] = num1;
                AuthCryptography._cryptKey2[index] = num2;
                num1 = (byte)((15 + (int)(byte)((uint)num1 * 250U)) * (int)num1 + 19);
                num2 = (byte)((121 - (int)(byte)((uint)num2 * 92U)) * (int)num2 + 109);
            }
        }

        public AuthCryptography()
        {
            this._encryptCounter = new AuthCryptography.CryptCounter();
            this._decryptCounter = new AuthCryptography.CryptCounter();
        }

        public void Encrypt(byte[] buffer)
        {
            for (int index = 0; index < buffer.Length; ++index)
            {
                buffer[index] ^= (byte)171;
                buffer[index] = (byte)((int)buffer[index] >> 4 | (int)buffer[index] << 4);
                buffer[index] ^= (byte)((uint)AuthCryptography._cryptKey1[(int)this._encryptCounter.Key1] ^ (uint)AuthCryptography._cryptKey2[(int)this._encryptCounter.Key2]);
                this._encryptCounter.Increment();
            }
        }

        public void Decrypt(byte[] buffer, int length)
        {
            if (!AuthCryptography.Decrypt2)
            {
                for (int index = 0; index < length; ++index)
                {
                    buffer[index] ^= (byte)171;
                    buffer[index] = (byte)((int)buffer[index] >> 4 | (int)buffer[index] << 4);
                    buffer[index] ^= (byte)((uint)AuthCryptography._cryptKey2[(int)this._decryptCounter.Key2] ^ (uint)AuthCryptography._cryptKey1[(int)this._decryptCounter.Key1]);
                    this._decryptCounter.Increment();
                }
            }
            else
            {
                for (int index = 0; index < length; ++index)
                {
                    buffer[index] ^= (byte)171;
                    buffer[index] = (byte)((int)buffer[index] >> 4 | (int)buffer[index] << 4);
                    buffer[index] ^= (byte)((uint)AuthCryptography._cryptKey4[(int)this._decryptCounter.Key2] ^ (uint)AuthCryptography._cryptKey3[(int)this._decryptCounter.Key1]);
                    this._decryptCounter.Increment();
                }
            }
        }

        public static void GenerateKeys(uint CryptoKey, uint AccountID)
        {
            uint num1 = (uint)((int)CryptoKey + (int)AccountID ^ 17185) ^ CryptoKey;
            uint num2 = num1 * num1;
            AuthCryptography._cryptKey3 = new byte[256];
            AuthCryptography._cryptKey4 = new byte[256];
            for (int index = 0; index < 256; ++index)
            {
                int num3 = (3 - index % 4) * 8;
                int num4 = index % 4 * 8 + num3;
                AuthCryptography._cryptKey3[index] = (byte)((uint)AuthCryptography._cryptKey1[index] ^ num1 << num3 >> num4);
                AuthCryptography._cryptKey4[index] = (byte)((uint)AuthCryptography._cryptKey2[index] ^ num2 << num3 >> num4);
            }
            AuthCryptography.Decrypt2 = true;
        }

        public static void GenerateKeys2(byte[] InKey1, byte[] InKey2)
        {
            byte[] numArray1 = new byte[4];
            byte[] numArray2 = new byte[4];
            byte[] numArray3 = new byte[4];
            byte[] numArray4 = new byte[4];
            AuthCryptography._cryptKey3 = new byte[256];
            AuthCryptography._cryptKey4 = new byte[256];
            for (int index = 0; index < 4; ++index)
            {
                numArray1[index] = InKey1[3 - index];
                numArray2[index] = InKey2[3 - index];
            }
            uint num1 = ((uint)((int)numArray1[3] << 24 | (int)numArray1[2] << 16 | (int)numArray1[1] << 8) | (uint)numArray1[0]) + ((uint)((int)numArray2[3] << 24 | (int)numArray2[2] << 16 | (int)numArray2[1] << 8) | (uint)numArray2[0]);
            numArray3[0] = (byte)(num1 & (uint)byte.MaxValue);
            numArray3[1] = (byte)(num1 >> 8 & (uint)byte.MaxValue);
            numArray3[2] = (byte)(num1 >> 16 & (uint)byte.MaxValue);
            numArray3[3] = (byte)(num1 >> 24 & (uint)byte.MaxValue);
            for (int index = 3; index >= 0; --index)
                numArray4[3 - index] = numArray3[index];
            numArray4[2] = (byte)((uint)numArray4[2] ^ 67U);
            numArray4[3] = (byte)((uint)numArray4[3] ^ 33U);
            for (int index = 0; index < 4; ++index)
                numArray4[index] = (byte)((uint)numArray4[index] ^ (uint)InKey1[index]);
            for (int index = 0; index < 256; ++index)
                AuthCryptography._cryptKey3[index] = (byte)((uint)numArray4[3 - index % 4] ^ (uint)AuthCryptography._cryptKey1[index]);
            for (int index = 0; index < 4; ++index)
                numArray3[index] = numArray4[3 - index];
            uint num2 = (uint)((int)numArray3[3] << 24 | (int)numArray3[2] << 16 | (int)numArray3[1] << 8) | (uint)numArray3[0];
            uint uint32 = Convert.ToUInt32((long)(num2 * num2) << 32 >> 32 & (long)uint.MaxValue);
            numArray3[0] = (byte)(uint32 & (uint)byte.MaxValue);
            numArray3[1] = (byte)(uint32 >> 8 & (uint)byte.MaxValue);
            numArray3[2] = (byte)(uint32 >> 16 & (uint)byte.MaxValue);
            numArray3[3] = (byte)(uint32 >> 24 & (uint)byte.MaxValue);
            for (int index = 3; index >= 0; --index)
                numArray4[3 - index] = numArray3[index];
            for (int index = 0; index < 256; ++index)
                AuthCryptography._cryptKey4[index] = Convert.ToByte((int)numArray4[3 - index % 4] ^ (int)AuthCryptography._cryptKey2[index]);
            AuthCryptography.Decrypt2 = true;
        }

        private class CryptCounter
        {
            private ushort m_Counter = 0;

            public CryptCounter()
            {
            }

            public CryptCounter(ushort with)
            {
                this.m_Counter = with;
            }

            public byte Key2
            {
                get
                {
                    return (byte)((uint)this.m_Counter >> 8);
                }
            }

            public byte Key1
            {
                get
                {
                    return (byte)((uint)this.m_Counter & (uint)byte.MaxValue);
                }
            }

            public void Increment()
            {
                ++this.m_Counter;
            }
        }
    }
}
