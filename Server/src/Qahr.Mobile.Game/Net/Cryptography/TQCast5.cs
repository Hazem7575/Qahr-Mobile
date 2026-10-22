using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Cryptography
{
    public unsafe class TQCast5
    {
        private byte[] EncIvec;
        private byte[] DecIvec;
        private int[] EncNum;
        private int[] DecNum;
        private TQCast5Impl Impl;

        public TQCast5()
        {
            Impl = new TQCast5Impl();
            Reset();
        }
        public void GenerateKey(byte[] kb)
        {
            var key = new byte[16];
            for (int i = 0; i < 16; i++)
                key[i] = kb[i];

            // QahrMobile: the Conquer Online Mobile client keys CAST5 with the plain 16 bytes (OpenSSL CAST_set_key),
            // not with the TQCipher.GenerateCast5Key derivation of the patched PC client.
            Impl.SetKey(key);
        }
        public void Reset()
        {
            EncIvec = new byte[16];
            DecIvec = new byte[16];
            DecNum = new int[8];
            EncNum = new int[8];
        }
        public void Encrypt(byte* buffer_in, int in_off, byte[] buffer_out, int out_off, int length)
        {
            byte c;
            for (int l = length, n = EncNum[0], inc = in_off, outc = 0; l > 0; l--)
            {
                if (n == 0)
                {
                    Impl.EncryptBlock(EncIvec, 0, EncIvec, 0);
                }
                c = (byte)((buffer_in[inc++] ^ EncIvec[n]) & 0xff);
                buffer_out[outc++] = c;
                EncIvec[n] = c;
                n = (n + 1) & 0x07;
                EncNum[0] = n;
            }

        }
        public void Decrypt(byte[] buffer_in, int in_off, byte* buffer_out, int out_off, int length)
        {
            byte c, cc;
            for (int l = length, n = DecNum[0], inc = in_off, outc = 0; l > 0; l--)
            {
                if (n == 0)
                {
                    Impl.EncryptBlock(DecIvec, 0, DecIvec, 0);
                }
                cc = buffer_in[inc++];
                c = DecIvec[n];
                DecIvec[n] = cc;
                buffer_out[out_off + outc] = (byte)((c ^ cc) & 0xff);
                outc++;
                n = (n + 1) & 0x07;
                DecNum[0] = n;
            }
        }
    }
}
