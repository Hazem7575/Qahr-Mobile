using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Qahr.Mobile.ServerSockets
{
    public unsafe class Packet : IDisposable
    {

        public const int MAX_SIZE = 16384;

        private const int TQ_SEALSIZE = 8;

        private static string seal;

        // C: void* memset(void* dst, int val, size_t count). size_t = UIntPtr (same 4 bytes as the old int on
        // x86, 8 bytes on x64); the int overload keeps every caller unchanged.
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void* memset_native(void* dst, int val, UIntPtr count);

        private static void* memset(void* dst, int val, int count)
        {
            return memset_native(dst, val, (UIntPtr)unchecked((uint)count));
        }

        public static void WriteString(string arg, int offset, byte[] buffer)
        {
            if (buffer != null && offset <= buffer.Length - 1 && buffer.Length >= offset + arg.Length)
            {
                for (ushort num = 0; num < arg.Length; num = (ushort)(num + 1))
                {
                    buffer[(ushort)(num + offset)] = (byte)arg[num];
                }
            }
        }

        public static void WriteUInt16(ushort arg, int offset, byte[] buffer)
        {
            if (buffer != null && offset <= buffer.Length - 1 && buffer.Length >= offset + 2)
            {
                buffer[offset] = (byte)arg;
                buffer[offset + 1] = (byte)(arg >> 8);
            }
        }

        public static string SealString
        {
            get { return seal; }
            set
            {

                if (value.Length != TQ_SEALSIZE)
                    throw new ArgumentOutOfRangeException("value", "Packet seal must be " + TQ_SEALSIZE + " chars long.");

                seal = value;
            }
        }

        public static int SealSize
        {
            get { return seal != null ? TQ_SEALSIZE : 0; }
        }

        public int Size { get; set; }
        public byte* Memory { get; private set; }
        private bool IsDisposed = false;
        // [Security flaw 12] Guards against the same packet being returned to the recycle bin twice
        // (which would hand one buffer to two concurrent users and corrupt traffic).
        public bool Pooled = false;
        public byte* stream;

        public int Position { get { return (int)(stream - Memory); } }
        public void Print()
        {
            int oldPos = Position;
            this.Seek(0);
            byte[] Data = ReadBytes(Size);
            ushort PacketLength = (ushort)Data.Length;
            if (ASCIIEncoding.ASCII.GetString(Data).Contains("TQServer") || ASCIIEncoding.ASCII.GetString(Data).Contains("TQClient"))
                PacketLength += 8;
            if (PacketLength > Data.Length) PacketLength = (ushort)Data.Length;

            string DataStr = "";
            DataStr += "Packet Length : " + PacketLength + ", PacketType: " + BitConverter.ToInt16(Data, 2) + Environment.NewLine;


            for (int i = 0; i < Math.Ceiling((double)PacketLength / 16); i++)
            {
                int t = 16;
                if (((i + 1) * 16) > PacketLength)
                    t = PacketLength - (i * 16);
                for (int a = 0; a < t; a++)
                {
                    DataStr += Data[i * 16 + a].ToString("X2") + " ";

                }
                if (t < 16)
                    for (int a = t; a < 16; a++)
                        DataStr += "   ";
                DataStr += "     ;";

                for (int a = 0; a < t; a++)
                {
                    DataStr += Convert.ToChar(Data[i * 16 + a]);
                }
                DataStr += Environment.NewLine;
            }
            DataStr.Replace(Convert.ToChar(0), '.');
            DataStr += Environment.NewLine;
            MyConsole.WriteLine(DataStr, ConsoleColor.DarkRed);
            this.Seek(oldPos);
        }
        public Packet(byte[] buffer)
        {
            // [Security flaw 19] Allocate a full MAX_SIZE buffer and never copy more than fits, so an
            // oversized input can't overflow the native allocation (was a fixed 1052-byte alloc).
            Memory = (byte*)Marshal.AllocHGlobal(MAX_SIZE);
            AddPressure(MAX_SIZE);
            stream = Memory;
            int copy = buffer.Length;
            if (copy > MAX_SIZE) copy = MAX_SIZE;
            Marshal.Copy(buffer, 0, (IntPtr)this.stream, copy);
            this.Size = copy;
            // [Perf flaw 5] Removed per-packet `new StackTrace()` (ran on every allocation).
        }
        public Packet(int size)
        {
            Memory = (byte*)Marshal.AllocHGlobal(size);
            AddPressure(size);
            stream = Memory;
            // [Perf flaw 5] Removed per-packet `new StackTrace()` (ran on every allocation / Take()).
        }

        // [perf N3] Every Packet owns a native (AllocHGlobal) block that the GC can't see, and most packets
        // are only freed by their finalizer. Telling the GC about the block makes it collect (and run the
        // finalizers) in time instead of letting native memory pile up. Switch: ServerConfig.ini
        // [Network] PacketMemoryPressure=0 turns it off (no build needed).
        public static readonly bool MemoryPressureEnabled = ReadMemoryPressureSwitch();
        private long PressureBytes;
        private static bool ReadMemoryPressureSwitch()
        {
            try { return Qahr.Mobile.SharedConfig.Int("Network", "PacketMemoryPressure", 1) != 0; }
            catch { return true; }
        }
        private void AddPressure(int bytes)
        {
            if (MemoryPressureEnabled && bytes > 0 && (IntPtr)Memory != IntPtr.Zero)
            {
                GC.AddMemoryPressure(bytes);
                PressureBytes = bytes;
            }
        }
        // [Security flaw 12] Zero the whole buffer so a recycled packet can never leak the previous
        // (possibly another client's) packet contents through reads past Size.
        public void ClearMemory()
        {
            if ((IntPtr)Memory != IntPtr.Zero)
                memset(Memory, 0, MAX_SIZE);
        }

        ~Packet()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            
            {
                if (this.IsDisposed) return;
                IsDisposed = true;
                if ((IntPtr)this.Memory == IntPtr.Zero)
                    return;
                Marshal.FreeHGlobal((IntPtr)this.Memory);
                this.Memory = null;
                if (PressureBytes > 0)
                {
                    GC.RemoveMemoryPressure(PressureBytes);
                    PressureBytes = 0;
                }
                GC.SuppressFinalize(this);
            }
        }

        public void InitWriter()
        {
            Seek(4);
        }
        public void Seek(int offset)
        {
            stream = &Memory[offset];
        }
        public void SeekForward(int amount)
        {
            Seek(Position + amount);
        }
        public void SeekBackwards(int amount)
        {
            Seek(Position - amount);
        }
        public void Write(bool value)
        {
            Write((byte)(value ? 1 : 0));
        }
        public void Write(byte value)
        {
            if (Position + 1 >= MAX_SIZE)
                return;
            *stream = value;
            stream++;
        }
        public void Write(ushort value)
        {
            if (Position + 2 >= MAX_SIZE)
                return;
            *((ushort*)stream) = value;
            stream += sizeof(ushort);
        }
        public void Write(uint value)
        {
            if (Position + 4 >= MAX_SIZE)
                return;
            *((uint*)stream) = value;
            stream += sizeof(uint);
        }
        public void Write(ulong value)
        {
            if (Position + 8 >= MAX_SIZE)
                return;
            *((ulong*)stream) = value;
            stream += sizeof(ulong);
        }
        public void Write(sbyte value) { Write((byte)value); }
        public void Write(short value) { Write((ushort)value); }
        public void Write(int value) { Write((uint)value); }
        public void Write(long value) { Write((ulong)value); }
        public void Write(string value, int length)
        {
            int min = Math.Min(value.Length, length);
            var buf = Encoding.UTF8.GetBytes(value);
            for (int i = 0; i < min; i++)
                Write(buf[i]);
            ZeroFill(length - min);
        }
        public void Write(params string[] value)
        {
            Write((byte)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                var str = value[i];
                if (string.IsNullOrEmpty(str))
                {
                    Write((ushort)0);
                    continue;
                }

                var buffer = Encoding.UTF8.GetBytes(str);
                Write((ushort)buffer.Length);
                for (int x = 0; x < buffer.Length; x++)
                    Write(buffer[x]);
            }
        }
        public void WriteUnsafe(void* buf, int length)
        {
            if (Position + length >= MAX_SIZE)
                return;
            memcpy(stream, buf, length);
            stream += length;
        }
        public void ZeroFill(int amount)
        {
            if (Position + amount >= MAX_SIZE)
                return;
            if (amount > 0)
                memset(stream, 0, amount);
            stream += amount;
        }
        public string[] ReadStringList()
        {
            var result = new string[ReadUInt8()];
            for (int i = 0; i < result.Length; i++)
                result[i] = ReadCString(ReadUInt16());
            return result;
        }
        public void ReadUnsafe(void* buf, int length)
        {
            if (Position + length >= MAX_SIZE)
                return;
            memcpy(buf, stream, length);
            stream += length;
        }
        public byte[] ReadBytes(int size)
        {
            byte[] res = new byte[size];
            for (int i = 0; i < res.Length; i++)
                res[i] = ReadUInt8();
            return res;
        }
        public string ReadCString(int size)
        {
            if (Position + size >= MAX_SIZE)
                return "";
            string result = Encoding.UTF8.GetString(ReadBytes(size));
            int idx = result.IndexOf('\0');
            return (idx > -1) ? result.Substring(0, idx) : result;
        }
        public byte ReadUInt8()
        {
            if (Position + 1 >= MAX_SIZE)
                return 0;
            var result = *stream;
            stream++;
            return result;
        }
        public ushort ReadUInt16()
        {
            if (Position + 2 >= MAX_SIZE)
                return 0;
            var result = *((ushort*)stream);
            stream += sizeof(ushort);
            return result;
        }
        public uint ReadUInt32()
        {
            if (Position + 4 >= MAX_SIZE)
                return 0;
            var result = *((uint*)stream);
            stream += sizeof(uint);
            return result;
        }
        public ulong ReadUInt64()
        {
            if (Position + 8 >= MAX_SIZE)
                return 0;
            var result = *((ulong*)stream);
            stream += sizeof(ulong);
            return result;
        }
        public sbyte ReadInt8() { return (sbyte)ReadUInt8(); }
        public short ReadInt16() { return (short)ReadUInt16(); }
        public int ReadInt32() { return (int)ReadUInt32(); }
        public long ReadInt64() { return (long)ReadUInt64(); }

        public void Finalize(ushort type)
        {
            if (SealSize > 0)
            {
                WriteSeal();
            }

            this.Size = this.Position;
            Seek(0);
            Write((ushort)(this.Size - SealSize));
            Write((ushort)type);
        }
        public void ProtoBufferSerialize(object obj)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                byte[] array = ms.ToArray();

                fixed (byte* proto_ptr = array)
                    memcpy(stream, proto_ptr, array.Length);

                stream += array.Length;
            }
        }
        public T ProtoBufferDeserialize<T>(T obj)
        {
            Seek(0);
            ushort packet_length = ReadUInt16();
            ReadUInt16();
            byte[] array = new byte[packet_length - 4];
            array = ReadBytes(packet_length - 4);

            using (var ms = new System.IO.MemoryStream(array))
            {
                obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            return obj;
        }
        public T ProtoBufferDeserialize<T>(T obj, byte[] packet)
        {

            using (var ms = new System.IO.MemoryStream(packet))
            {
                obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            return obj;
        }
        public void CreateProtocolBuffer(params uint[] values)
        {
            Write((byte)8);
            for (int x = 0; x < values.Length; x++)
            {
                uint value = values[x];
                while (value > 0x7F)
                {
                    Write((byte)((value & 0x7F) | 0x80));
                    value >>= 7;
                }
                Write((byte)(value & 0x7F));
                Write((byte)(8 * (x + 2)));
                if (x + 1 == values.Length)
                    break;
            }
        }
        public void WriteSeal()
        {
            Write(SealString, TQ_SEALSIZE); 
        }
        private static string CreatePacketStringWithNumbers(byte* b, int len)
        {
           

            int msgSize = (len) * 4 
                + ((len / 16) + 1) * 9;
            StringBuilder hex = new StringBuilder(msgSize);
            for (int i = 0; i < len; i += 16)
            {
                hex.AppendFormat("[{0:000}] ", i);
                for (int z = i; z < i + 16; z++)
                {
                    if (z >= len) hex.Append("   ");
                    else
                        hex.AppendFormat("{0:x2} ", b[z]);
                }
                hex.Append('\t');
                for (int z = i; z < i + 16; z++)
                {
                    if (z >= len) hex.Append(" ");
                    else
                    {
                        if (b[z] > 32 && b[z] < 127)
                        {
                            hex.AppendFormat("{0}", (char)b[z]);
                        }
                        else
                        {
                            hex.Append('.');
                        }
                    }
                }
                hex.Append("\r\n");
            }
            return hex.ToString();
        }
        public static string Dump(byte[] b)
        {
            fixed (byte* ptr = b)
                return CreatePacketStringWithNumbers(ptr, b.Length);
        }
        public string Dump(string header)
        {
            int offset = Position; Seek(2);
            int type = ReadUInt16(); Seek(offset);

            string str = "Packet (" + header + ") - Type: " + type + " Size - " + Size + "\r\n";
            str += CreatePacketStringWithNumbers(this.Memory, this.Size);
            return str;
        }
        public unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                try
                {
                    *(((long*)dest) + i) = *(((long*)src) + i);
                }
                catch { break; }

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                try
                {
                    *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
                }
                catch { break; }
        }
        public unsafe bool EncryptPipeServer()
        {
            try
            {
                Write((ulong)19562354);
                return true;
            }
            catch
            {
                MyConsole.WriteLine("Error Encrypting Web Server Packet.", ConsoleColor.DarkRed);
                return false;
            }
        }
        public unsafe bool DecryptPipeServer()
        {
            try
            {
                Seek(Size - 8);
                if (ReadUInt64() == 19562354)
                {
                    Seek(0);
                    return true;
                }
                Seek(0);
                return false;
            }
            catch
            {
                MyConsole.WriteLine("Error Decrypting Web Server Packet.", ConsoleColor.DarkRed);
            }
            return false;
        }
    }
}
