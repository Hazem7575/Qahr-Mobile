using System;
using System.Collections.Generic;

namespace Qahr.Mobile.Phone
{
    /// <summary>Shift and Struct layouts: a C struct on both sides, with another layout.</summary>
    public static unsafe partial class MobileLayouts
    {
        static byte[] ShiftToMobile(byte* p, int len, Layout l)
        {
            byte[] r = Shift(p, len, l.ServerSize, l.MobileSize);
            MapValues(r, l, true);
            return r;
        }

        static byte[] ShiftToServer(byte* p, int len, Layout l)
        {
            byte[] r = Shift(p, len, l.MobileSize, l.ServerSize);
            MapValues(r, l, false);
            return r;
        }

        /// <summary>A struct with its tail moved from offset "from" to offset "to" (the header before both is copied,
        /// a gap is zero filled); the length at 0 is updated, the id at 2 is left to the caller.</summary>
        static byte[] Shift(byte* p, int len, int from, int to)
        {
            int tail = Math.Max(0, len - from);
            byte[] r = new byte[to + tail];
            int head = Math.Min(Math.Min(from, to), len);
            for (int i = 0; i < head; i++) r[i] = p[i];
            for (int i = 0; i < tail; i++) r[to + i] = p[from + i];
            MobileWire.WriteU16(r, 0, (ushort)r.Length);
            return r;
        }

        /// <summary>Shift layouts: header values that differ between the two sides (e.g. the action numbers).</summary>
        static void MapValues(byte[] b, Layout l, bool toMobile)
        {
            foreach (var f in l.Fields)
            {
                if (f.Offset + f.Size > b.Length)
                    continue;
                ulong v = MobileWire.ReadScalar(b, f.Offset, f.Type), to;
                if ((toMobile ? f.ToMobile : f.ToServer).TryGetValue(v, out to))
                    MobileWire.WriteScalar(b, f.Offset, f.Type, to);
            }
        }

        /// <summary>Struct layouts: every listed field copied from its offset on one side to its offset on the other
        /// (str:N: the N bytes; "strings": a string list, u8 count then u16 length + bytes per string, the same on
        /// both sides, which makes the struct longer); the length at 0 is the result's size, the id at 2 is left to
        /// the caller.</summary>
        static byte[] Restruct(byte* p, int len, Layout l, bool toMobile)
        {
            byte[] r = new byte[toMobile ? l.MobileSize : l.ServerSize];
            foreach (var f in l.Fields)
            {
                if (toMobile ? !f.Out : !f.In)
                    continue;
                if (f.Type == "units" && f.Units != null && f.Units.Count == 1)
                {
                    LayoutExt.CopyUnits(p, len, f, ref r, toMobile);// "units:stride[:bytes] count ..." lines
                    continue;
                }
                int from = toMobile ? f.Proto : f.Offset, to = toMobile ? f.Offset : f.Proto;
                if (f.Type == "strings")
                {
                    int n = StringListLength(p, len, from);
                    if (to + n > r.Length)
                        Array.Resize(ref r, to + n);
                    for (int i = 0; i < n; i++)
                        r[to + i] = p[from + i];
                    continue;
                }
                for (int i = 0; i < f.Size && from + i < len && to + i < r.Length; i++)
                    r[to + i] = p[from + i];
                var map = toMobile ? f.ToMobile : f.ToServer;
                ulong v, mapped;
                if (map.Count != 0 && to + f.Size <= r.Length && map.TryGetValue(v = MobileWire.ReadScalar(r, to, f.Type), out mapped))
                    MobileWire.WriteScalar(r, to, f.Type, mapped);
            }
            MobileWire.WriteU16(r, 0, (ushort)r.Length);
            return r;
        }

        /// <summary>Bytes taken by a string list at "at" (u8 count, then u16 length + bytes each), cut at the packet end.</summary>
        static int StringListLength(byte* p, int len, int at)
        {
            if (at >= len)
                return 0;
            int count = p[at], o = at + 1;
            for (int i = 0; i < count && o + 2 <= len; i++)
                o += 2 + (p[o] | p[o + 1] << 8);
            return Math.Min(o, len) - at;
        }

        /// <summary>Struct layouts, type "units:stride[:bytes]": a run of fixed-size units whose count is another field
        /// of the struct. Line: "serverOffset mobileOffset units:stride[:bytes] count sCountOffset mCountOffset countType
        /// [in|out]". The count is read from the result after the lines above it (so a "map" on the count line applies:
        /// put the count line first), cut to the units the source really holds (at least the first one when any byte of
        /// it is there), written back, and that many units are copied (only the first "bytes" of each, the rest zero).</summary>
        sealed partial class LayoutExt
        {
            const int MaxUnits = 255;

            public static void ParseStructUnits(Layout l, string[] p)
            {
                // p: sOff mOff units:stride[:bytes] count sCount mCount type [in|out]
                var t = p[2].Split(':');
                if (t.Length < 2 || t.Length > 3 || p.Length < 7 || p[3] != "count")
                    throw new Exception("units line: serverOffset mobileOffset units:stride[:bytes] count sCountOffset mCountOffset type [in|out]");
                var f = new Field { Proto = int.Parse(p[0]), Offset = int.Parse(p[1]), Type = "units", Stride = int.Parse(t[1]) };
                f.Size = t.Length == 3 ? int.Parse(t[2]) : f.Stride;
                var c = new Field { Proto = int.Parse(p[4]), Offset = int.Parse(p[5]), Type = p[6].ToLowerInvariant() };
                if (c.Type != "u8" && c.Type != "u16" && c.Type != "u32")
                    throw new Exception("units count type must be u8, u16 or u32");
                c.Size = SizeOf(c.Type);
                if (f.Stride < 1 || f.Stride > 1024 || f.Size < 1 || f.Size > f.Stride || f.Proto < 4 || f.Offset < 4
                    || f.Proto > 4096 || f.Offset > 4096 || c.Proto < 4 || c.Offset < 4 || c.Proto > 4096 || c.Offset > 4096)
                    throw new Exception("bad units offsets/sizes");
                f.Units = new List<Field> { c };
                ParseDirection(f, p, 7);
                l.Fields.Add(f);
            }

            public static void CopyUnits(byte* p, int len, Field f, ref byte[] r, bool toMobile)
            {
                var c = f.Units[0];
                int from = toMobile ? f.Proto : f.Offset, to = toMobile ? f.Offset : f.Proto;
                int cntAt = toMobile ? c.Offset : c.Proto;
                if (cntAt + c.Size > r.Length)
                    return;
                ulong count = MobileWire.ReadScalar(r, cntAt, c.Type);
                int have = len > from ? Math.Max(1, (len - from) / f.Stride) : 0;
                int n = (int)Math.Min(count, (ulong)Math.Min(have, MaxUnits));
                if (to + n * f.Stride > r.Length)
                    Array.Resize(ref r, to + n * f.Stride);
                for (int k = 0; k < n; k++)
                {
                    int s = from + k * f.Stride, d = to + k * f.Stride;
                    for (int b = 0; b < f.Stride; b++)
                        r[d + b] = b < f.Size && s + b < len ? p[s + b] : (byte)0;
                }
                MobileWire.WriteScalar(r, cntAt, c.Type, (ulong)n);
            }
        }
    }
}
