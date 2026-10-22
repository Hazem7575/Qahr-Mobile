using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Cryptography
{
    public unsafe interface ICrypto
    {
        void Encrypt(Byte* pBup,byte[] pOut, Int32 Length);
        void Decrypt(byte[] In, int start, byte* buff_out, int Size);
        void GenerateKey(byte[] BufKey);
        void SetIVs(Byte[] EncryptIV, Byte[] DecryptIV);
    }
}
