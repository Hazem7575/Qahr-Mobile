using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>ProtoStruct layouts: the server writes protobuf, the mobile client keeps a C struct (e.g. CMsgItem).</summary>
    public static unsafe partial class MobileLayouts
    {
        /// <summary>Mobile struct -> protobuf body (without the 4-byte header).</summary>
        static byte[] StructToProto(byte* p, int len, Layout l)
        {
            byte[] src = new byte[len];
            for (int i = 0; i < len; i++)
                src[i] = p[i];

            var proto = new MemoryStream();
            foreach (var f in l.Fields)
            {
                if (!f.In || f.Offset + f.Size > len)
                    continue;
                if (f.Type == "list32")
                {
                    if (f.CountOffset + 4 > len)
                        continue;
                    int count = (int)BitConverter.ToUInt32(src, f.CountOffset);
                    for (int i = 0; i < count && f.Offset + 4 * i + 4 <= len; i++)
                        MobileWire.WriteVarintField(proto, f.Proto, BitConverter.ToUInt32(src, f.Offset + 4 * i));
                }
                else if (f.Type.StartsWith("str:"))
                {
                    int end = f.Offset;
                    while (end < f.Offset + f.Size && src[end] != 0) end++;
                    MobileWire.WriteBytesField(proto, f.Proto, src, f.Offset, end - f.Offset);
                }
                else if (f.Type == "strlist")
                {
                    // CNetStringPacker from the mobile (e.g. CMsgMBName::Create): u8 count, then u16 length (little
                    // endian) + bytes per string; each one becomes a value of the repeated field, cut at the packet end
                    if (f.Offset >= len)
                        continue;
                    int count = src[f.Offset], o = f.Offset + 1;
                    for (int k = 0; k < count && o + 2 <= len; k++)
                    {
                        int n = src[o] | src[o + 1] << 8;
                        o += 2;
                        if (n > len - o)
                            break;
                        MobileWire.WriteBytesField(proto, f.Proto, src, o, n);
                        o += n;
                    }
                }
                else
                {
                    ulong v = MobileWire.ReadScalar(src, f.Offset, f.Type);
                    if (v != 0)
                        MobileWire.WriteVarintField(proto, f.Proto, v);
                }
            }
            return proto.ToArray();
        }

        /// <summary>Server protobuf packet -> the whole mobile packet (struct + seal).</summary>
        static byte[] ProtoToStruct(byte* p, int len, Layout l, ushort mobileId)
        {
            var fields = MobileWire.ParseProto(p + 4, len - 4);

            int size = l.BaseSize;
            foreach (var f in l.Fields)
            {
                if (!f.Out) continue;
                if (f.Pack.Count > 0)
                {
                    int n = 1;
                    foreach (int pf in f.Pack)
                        n += 2 + Math.Min(1000, StringField(fields, pf).Length);
                    size = Math.Max(size, f.Offset + n);
                }
                else if (f.Type == "strlist")
                {
                    int n = 1;
                    foreach (var s in StringValues(fields, f.Proto))
                        n += 2 + s.Length;
                    size = Math.Max(size, f.Offset + n);
                }
                else if (f.Type == "list32" || f.Units != null)
                {
                    List<object> vals;
                    int n = fields.TryGetValue(f.Proto, out vals) ? vals.Count : 0;
                    size = Math.Max(size, f.Offset + (f.Units != null ? f.Stride : 4) * n);
                }
                else
                    size = Math.Max(size, f.Offset + f.Size);
            }
            byte[] dst = new byte[size];
            MobileWire.WriteU16(dst, 0, (ushort)size);
            foreach (var f in l.Fields)
            {
                if (!f.Out) continue;
                if (f.Pack.Count > 0)
                {
                    WriteStringPack(dst, f, fields);
                    continue;
                }
                if (f.Type == "strlist")
                {
                    // every value of a repeated string field: u8 count, then u16 length + bytes each
                    var all = StringValues(fields, f.Proto);
                    int o = f.Offset;
                    dst[o++] = (byte)all.Count;
                    foreach (var s in all)
                    {
                        dst[o++] = (byte)s.Length; dst[o++] = (byte)(s.Length >> 8);
                        Buffer.BlockCopy(s, 0, dst, o, s.Length);
                        o += s.Length;
                    }
                    continue;
                }
                List<object> vals;
                if (!fields.TryGetValue(f.Proto, out vals) || vals.Count == 0)
                    continue;
                if (f.Units != null)
                {
                    for (int i = 0; i < vals.Count; i++)
                        WriteUnit(dst, f, f.Offset + i * f.Stride, vals[i] as byte[]);
                    MobileWire.WriteU32(dst, f.CountOffset, (uint)vals.Count);
                }
                else if (f.Type == "list32")
                {
                    for (int i = 0; i < vals.Count; i++)
                        MobileWire.WriteU32(dst, f.Offset + 4 * i, (uint)MobileWire.AsULong(vals[i]));
                    MobileWire.WriteU32(dst, f.CountOffset, (uint)vals.Count);
                }
                else if (f.Type.StartsWith("str:"))
                {
                    var b = vals[vals.Count - 1] as byte[];
                    if (b != null)
                        Buffer.BlockCopy(b, 0, dst, f.Offset, Math.Min(b.Length, f.Size - 1));
                }
                else
                {
                    // zero is skipped: the struct starts zeroed, and several fields may share one offset (MsgTrade)
                    ulong v = MobileWire.AsULong(vals[vals.Count - 1]);
                    if (v != 0)
                        MobileWire.WriteScalar(dst, f.Offset, f.Type, v);
                }
            }
            return Seal(dst, mobileId, false);
        }

        /// <summary>One element of a "units" field: a protobuf sub-message written as a struct at "at".</summary>
        static void WriteUnit(byte[] dst, Field f, int at, byte[] message)
        {
            if (message == null)
                return;
            Dictionary<int, List<object>> sub;
            fixed (byte* p = message)
                sub = MobileWire.ParseProto(p, message.Length);
            foreach (var u in f.Units)
            {
                List<object> vals;
                if (!sub.TryGetValue(u.Proto, out vals) || vals.Count == 0)
                    continue;
                if (u.Type.StartsWith("str:"))
                {
                    var b = vals[vals.Count - 1] as byte[];
                    if (b != null)
                        Buffer.BlockCopy(b, 0, dst, at + u.Offset, Math.Min(b.Length, u.Size - 1));
                }
                else
                    MobileWire.WriteScalar(dst, at + u.Offset, u.Type, MobileWire.AsULong(vals[vals.Count - 1]));
            }
        }

        /// <summary>CNetStringPacker: u8 count, then u16 length (little endian) + bytes per string.</summary>
        static void WriteStringPack(byte[] dst, Field f, Dictionary<int, List<object>> fields)
        {
            int o = f.Offset;
            dst[o++] = (byte)f.Pack.Count;
            foreach (int pf in f.Pack)
            {
                byte[] s = StringField(fields, pf);
                int n = Math.Min(1000, s.Length);
                dst[o++] = (byte)n; dst[o++] = (byte)(n >> 8);
                Buffer.BlockCopy(s, 0, dst, o, n);
                o += n;
            }
        }

        /// <summary>All values of a repeated string field (at most 255, each cut at 1000 bytes).</summary>
        static List<byte[]> StringValues(Dictionary<int, List<object>> fields, int proto)
        {
            var r = new List<byte[]>();
            List<object> vals;
            if (fields.TryGetValue(proto, out vals))
                foreach (var v in vals)
                {
                    var b = v as byte[];
                    if (b == null || r.Count == 255) continue;
                    if (b.Length > 1000) Array.Resize(ref b, 1000);
                    r.Add(b);
                }
            return r;
        }

        static byte[] StringField(Dictionary<int, List<object>> fields, int proto)
        {
            List<object> vals;
            if (proto == 0 || !fields.TryGetValue(proto, out vals) || vals.Count == 0)
                return new byte[0];
            return vals[vals.Count - 1] as byte[] ?? new byte[0];
        }

        /// <summary>"structpb" layouts ([id name structpb serverSize]): the server writes / reads a C struct, the mobile
        /// protobuf (the reverse of the [id name size] kind). Lines: "protoField serverOffset type [countOffset] [in|out]",
        /// types u8 u16 u32 i32 u64 str:N list32 (count u32 at countOffset). Server -> mobile writes every mapped field,
        /// zero values included (the mobile's protobuf-lite checks has-bits, e.g. CMsgItemDialog).</summary>
        sealed partial class LayoutExt
        {
            const int MaxStructSize = 4096, MaxListCount = 1024;

            public static bool IsServerStruct(Layout l)
            {
                var e = Get(l);
                return e != null && e.ServerStruct;
            }

            /// <summary>"[id name structpb serverSize]": serverSize = the server struct with its 4-byte header.</summary>
            public static int ParseStructSize(string[] h)
            {
                if (h.Length < 4)
                    throw new Exception("structpb needs the server struct size");
                int n = int.Parse(h[3]);
                if (n < 4 || n > MaxStructSize)
                    throw new Exception("bad structpb size " + h[3]);
                return n;
            }

            public static void ParseStructPbLine(Layout l, string[] p)
            {
                if (p.Length < 3)
                    throw new Exception("structpb line needs: protoField serverOffset type");
                var f = new Field { Proto = ParseFieldNumber(p[0]), Offset = int.Parse(p[1]), Type = p[2].ToLowerInvariant() };
                if (f.Type != "u8" && f.Type != "u16" && f.Type != "u32" && f.Type != "i32" && f.Type != "u64" && f.Type != "list32" && !f.Type.StartsWith("str:"))
                    throw new Exception("type " + f.Type + " is not supported by structpb");
                f.Size = SizeOf(f.Type);
                int next = 3;
                if (f.Type == "list32")
                {
                    if (p.Length < 4)
                        throw new Exception("list32 needs a count offset");
                    f.CountOffset = int.Parse(p[next++]);
                    if (f.CountOffset < 4 || f.CountOffset + 4 > MaxStructSize)
                        throw new Exception("bad count offset " + f.CountOffset);
                }
                if (f.Offset < 4 || f.Size < 1 || f.Offset + f.Size > MaxStructSize)
                    throw new Exception("bad offset/size " + f.Offset + "/" + f.Size);
                ParseDirection(f, p, next);
                l.Fields.Add(f);
            }

            /// <summary>Server struct (whole packet, header included) -> protobuf body for the mobile.</summary>
            public static byte[] StructToMobile(byte* p, int len, Layout l)
            {
                if (len < 0) len = 0;
                byte[] src = new byte[len];
                for (int i = 0; i < len; i++)
                    src[i] = p[i];
                var proto = new MemoryStream();
                foreach (var f in l.Fields)
                {
                    if (!f.Out)
                        continue;
                    if (f.Type == "list32")
                    {
                        if (f.CountOffset < 0 || f.CountOffset + 4 > len)
                            continue;
                        uint count = BitConverter.ToUInt32(src, f.CountOffset);
                        for (uint k = 0; k < count && k < MaxListCount; k++)
                        {
                            long at = f.Offset + 4L * k;
                            if (at + 4 > len)
                                break;
                            MobileWire.WriteVarintField(proto, f.Proto, BitConverter.ToUInt32(src, (int)at));
                        }
                    }
                    else if (f.Type.StartsWith("str:"))
                    {
                        if (f.Offset >= len)
                            continue;
                        int max = Math.Min(f.Size, len - f.Offset), n = 0;
                        while (n < max && src[f.Offset + n] != 0) n++;
                        MobileWire.WriteBytesField(proto, f.Proto, src, f.Offset, n);
                    }
                    else if (f.Offset + f.Size <= len)
                        MobileWire.WriteVarintField(proto, f.Proto, MobileWire.ReadScalar(src, f.Offset, f.Type));
                }
                return proto.ToArray();
            }

            /// <summary>Mobile protobuf body -> the server struct (whole, the length/id at 0/2 are set by Replace).</summary>
            public static byte[] MobileToStruct(byte* p, int len, Layout l)
            {
                var fields = MobileWire.ParseProto(p, Math.Max(0, len));
                int size = Math.Max(4, l.BaseSize);
                foreach (var f in l.Fields)
                {
                    if (!f.In)
                        continue;
                    if (f.Type == "list32")
                    {
                        List<object> vals;
                        int n = fields.TryGetValue(f.Proto, out vals) ? Math.Min(vals.Count, MaxListCount) : 0;
                        size = Math.Max(size, Math.Max(f.Offset + 4 * n, f.CountOffset + 4));
                    }
                    else
                        size = Math.Max(size, f.Offset + f.Size);
                }
                byte[] dst = new byte[size];
                foreach (var f in l.Fields)
                {
                    if (!f.In)
                        continue;
                    List<object> vals;
                    if (!fields.TryGetValue(f.Proto, out vals) || vals.Count == 0)
                        continue;
                    if (f.Type == "list32")
                    {
                        int n = Math.Min(vals.Count, MaxListCount);
                        for (int i = 0; i < n; i++)
                            MobileWire.WriteU32(dst, f.Offset + 4 * i, (uint)MobileWire.AsULong(vals[i]));
                        MobileWire.WriteU32(dst, f.CountOffset, (uint)n);
                    }
                    else if (f.Type.StartsWith("str:"))
                    {
                        var b = vals[vals.Count - 1] as byte[];
                        if (b != null)
                            Buffer.BlockCopy(b, 0, dst, f.Offset, Math.Min(b.Length, f.Size - 1));
                    }
                    else
                        MobileWire.WriteScalar(dst, f.Offset, f.Type, MobileWire.AsULong(vals[vals.Count - 1]));
                }
                MobileWire.WriteU16(dst, 0, (ushort)size);
                return dst;
            }
        }
    }
}
