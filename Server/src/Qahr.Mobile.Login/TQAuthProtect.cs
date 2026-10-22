using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Qahr.Mobile.Login.Auth
{
    internal class TQCipher
    {
        public static uint PublicSeed = 0;
        public TQCipher(bool _IsGameServer = false)
        {
            IsGameServer = _IsGameServer;
            PublicSeed = (uint)new System.Random().Next();
        }
  

        public static int HandleBuffer(ref byte[] data, bool isDecrypt)
        {
            if (data == null || data.Length <= 4)
                return 0;

            if (!isDecrypt)
                PadToMultipleOf(ref data);

            int length = System.BitConverter.ToUInt16(data, 0);

            if (data.Length - 8 != length && data.Length != length)
                throw new Exception("Invalid buffer length");

            if ((length - PADDING_SIZE) % 16 != 0)
                throw new Exception("Invalid buffer Padding");
            // Separate the length bytes and the data bytes
            byte[] lengthBytes = new byte[4];
            byte[] dataBytes = new byte[length - 4];

            Buffer.BlockCopy(data, 0, lengthBytes, 0, 4);
            Buffer.BlockCopy(data, 4, dataBytes, 0, length - 4);

            using (AesManaged aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;

                var srand = new srand(TQCipher.PublicSeed);
                byte[] Key1 = new byte[16];
                byte[] Key2 = new byte[16];

                //key 1
                for (int i = 0; i < 16; i++)
                    Key1[i] = (byte)srand.rand();

                //key 2
                for (int i = 0; i < 16; i++)
                    Key2[i] = (byte)srand.rand();

                aes.Key = Key1;
                aes.BlockSize = 128;
                aes.IV = Key2;
                aes.Padding = PaddingMode.PKCS7;

                if (dataBytes.Length % 16 == 0)
                    aes.Padding = PaddingMode.None;

                byte[] result = isDecrypt
                    ? aes.CreateDecryptor().TransformFinalBlock(dataBytes, 0, dataBytes.Length)
                    : aes.CreateEncryptor().TransformFinalBlock(dataBytes, 0, dataBytes.Length);


                byte[] newData = new byte[4 + result.Length];
                Buffer.BlockCopy(lengthBytes, 0, newData, 0, 4);
                Buffer.BlockCopy(result, 0, newData, 4, result.Length);
                data = newData;

            }
            return data.Length;
        }

        private static void PadToMultipleOf(ref byte[] src, int pad = PADDING_SIZE * 4)
        {
            int OldLength = src.Length - PADDING_SIZE - PADDING_REDUCE;
            if (OldLength % pad == 0)
                return;
            int len = (src.Length + pad - 1) / pad * pad;
            Array.Resize(ref src, len + PADDING_REDUCE + PADDING_SIZE);
            unsafe
            {
                fixed (byte* Buffer = src)
                    *((ushort*)(Buffer + 0)) = (ushort)(src.Length - PADDING_REDUCE);
            }
        }

        private static bool IsGameServer = false;
        private const byte PADDING_SIZE = 4;
        private static readonly int TQSeal_SIZE = 8;
        private static int PADDING_REDUCE
        {
            get { return IsGameServer ? TQSeal_SIZE : 0; }
        }
    }
    public class srand
    {
        private uint _seed = 0;
        public short rand()
        {
            _seed *= 0x343fd;
            _seed += 0x269ec3;
            return (short)((_seed >> 0x10) & 0x7fff);
        }
        public srand(uint seed)
        {
            _seed = seed;
        }
    }
    public static class TQAuthProtect
    {
        internal static void Load(bool _IsGameServer)
        {
            new TQCipher(_IsGameServer);
        }

        public enum PacketIDs : ushort
        {
            _MSG_NONE = 0,
            _MSG_GENERAL = 0x2328,
            _MSG_ACCOUNT,
            _MSG_CONNECT,
            _MSG_DATA,
            _MSG_MACHINE,
            _MSG_PROCESS,
            _MSG_STRING,
        }
      
        public class Authentication
        {
            public string Username, Password , HWID, MachineProductID, Server, Token, Language, MacAddress, MachineName;
            public bool VMware, VPSMachine;
            public uint CryptoKey;

            public static void DecryptObfuscate(byte[] data, uint SeedKey)
            {
                ushort MsgLenght = BitConverter.ToUInt16(data, 0);
                ushort MsgType = BitConverter.ToUInt16(data, 2);

                byte[] keyTable1 = new byte[256];
                byte[] keyTable2 = new byte[256];
                byte[] keyTable3 = new byte[256];

                for (int i = 0; i < 256; ++i)
                {
                    keyTable1[i] = (byte)(((MsgLenght >> (i % 24)) + i * 37) ^ (byte)(MsgType & 0xFF));
                    keyTable2[i] = (byte)(((MsgType << (i % 16)) ^ i * 19) + (SeedKey >> (i % 8)));
                    keyTable3[i] = (byte)(((SeedKey ^ i * 91) + (MsgLenght << (i % 7))) & 0xFF);
                }

                for (int i = 4; i < data.Length; ++i)
                {
                    byte b = data[i];

                    b ^= keyTable3[i % 256];
                    b = (byte)(b - (byte)(i * 11));
                    b ^= (byte)((MsgLenght >> (i % 16)) & 0xFF);
                    b ^= keyTable2[i % 256];
                    b = (byte)(((b << 2) | (b >> (8 - 2))) & 0xFF);              
                    b ^= (byte)(i * 73);
                    b = (byte)(((b >> 3) | (b << (8 - 3))) & 0xFF);            
                    b = (byte)~b;
                    b ^= keyTable1[i % 256];

                    data[i] = b;
                }
            }
            /// <summary>Mobile client login (Network\AuthPackets\MobileLogin.cs): only account, password and server.</summary>
            public Authentication(string server, string username, string password)
            {
                Server = server;
                Username = username;
                Password = password;
                Token = Language = MacAddress = MachineName = HWID = MachineProductID = "";
            }
            public Authentication(byte[] Buffer)
            {
                const ushort length = 320;
                if (Buffer != null && Buffer.Length >= length)
                {
                    ushort type = BitConverter.ToUInt16(Buffer, 2);
                    if (type == (ushort)PacketIDs._MSG_ACCOUNT)
                    {
                        TQCipher.HandleBuffer(ref Buffer, true);
                        DecryptObfuscate(Buffer, TQCipher.PublicSeed);
                        using (BinaryReader BR = new BinaryReader(new MemoryStream(Buffer)))
                        {
                            BR.BaseStream.Seek(4, SeekOrigin.Current);
                            var GetTickCount = BR.ReadUInt32();
                            CryptoKey = BR.ReadUInt32();
                            if (CryptoKey != 0x6574154)
                                throw new Exception("Invalid CryptoKey");
                            Username = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            Password = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            Server = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            Token = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            Language = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            MacAddress = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            MachineName = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            HWID = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            MachineProductID = global::Qahr.Mobile.Login.AnsiText.Value.GetString(BR.ReadBytes(BR.ReadByte())).Replace("\0", "");
                            VMware = BR.ReadBoolean();
                            VPSMachine = BR.ReadBoolean();
                        }
                    }
                }
            }
        }
    }
}
