using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>QahrMobile: protobuf wire format and little-endian struct helpers used by MobileLayouts.</summary>
    static unsafe class MobileWire
    {
        // ------------------------------------------------------------------------------------- protobuf
        /// <summary>All fields of a protobuf body: field number -> values (ulong for varint / fixed, byte[] for bytes).</summary>
        public static Dictionary<int, List<object>> ParseProto(byte* p, int len)
        {
            var r = new Dictionary<int, List<object>>();
            int i = 0;
            while (i < len)
            {
                ulong key = ReadVarint(p, len, ref i);
                int field = (int)(key >> 3), wire = (int)(key & 7);
                object v;
                if (wire == 0) v = ReadVarint(p, len, ref i);
                else if (wire == 1) { if (len - i < 8) break; v = *(ulong*)(p + i); i += 8; }// QahrMobile: no read past the end
                else if (wire == 5) { if (len - i < 4) break; v = (ulong)*(uint*)(p + i); i += 4; }
                else if (wire == 2)
                {
                    ulong ln = ReadVarint(p, len, ref i);
                    if (ln > (ulong)(len - i)) break;// QahrMobile: no int overflow / huge allocation from a bad length
                    int n = (int)ln;
                    var b = new byte[n];
                    for (int k = 0; k < n; k++) b[k] = p[i + k];
                    i += n;
                    v = b;
                }
                else break;
                List<object> list;
                if (!r.TryGetValue(field, out list)) r[field] = list = new List<object>();
                if (wire == 2 && field == 0) break;
                list.Add(v);
            }
            return r;
        }

        public static ulong AsULong(object v)
        {
            if (v is ulong) return (ulong)v;
            var b = v as byte[];// a packed repeated field is unpacked by the list code, a scalar never is length-delimited
            if (b != null)
            {
                int i = 0;
                fixed (byte* p = b) return ReadVarint(p, b.Length, ref i);
            }
            return 0;
        }

        public static ulong ReadVarint(byte* p, int len, ref int i)
        {
            ulong v = 0;
            for (int shift = 0; i < len && shift < 64; shift += 7)
            {
                byte b = p[i++];
                v |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0) break;
            }
            return v;
        }

        public static void WriteVarint(Stream s, ulong v)
        {
            while (v >= 0x80) { s.WriteByte((byte)(v | 0x80)); v >>= 7; }
            s.WriteByte((byte)v);
        }

        public static void WriteVarintField(Stream s, int field, ulong v)
        {
            WriteVarint(s, (ulong)(field << 3));
            WriteVarint(s, v);
        }

        public static void WriteBytesField(Stream s, int field, byte[] b, int off, int n)
        {
            WriteVarint(s, (ulong)((field << 3) | 2));
            WriteVarint(s, (ulong)n);
            s.Write(b, off, n);
        }

        // ------------------------------------------------------------------------------------- struct
        public static ulong ReadScalar(byte[] b, int o, string type)
        {
            switch (type)
            {
                case "u8": return b[o];
                case "u16": return BitConverter.ToUInt16(b, o);
                case "u32": return BitConverter.ToUInt32(b, o);
                case "i32": return (ulong)(long)BitConverter.ToInt32(b, o);
                case "u64": return BitConverter.ToUInt64(b, o);
            }
            return 0;
        }

        public static void WriteScalar(byte[] b, int o, string type, ulong v)
        {
            switch (type)
            {
                case "u8": b[o] = (byte)v; break;
                case "u16": WriteU16(b, o, (ushort)v); break;
                case "u32": case "i32": WriteU32(b, o, (uint)v); break;
                case "u64": for (int i = 0; i < 8; i++) b[o + i] = (byte)(v >> (8 * i)); break;
            }
        }

        public static void WriteU16(byte[] b, int o, ushort v) { b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); }
        public static void WriteU32(byte[] b, int o, uint v) { for (int i = 0; i < 4; i++) b[o + i] = (byte)(v >> (8 * i)); }
    }
}
