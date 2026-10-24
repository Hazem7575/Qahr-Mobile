using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>ProtoRenumber layouts: protobuf on both sides, only the field numbers differ (e.g. MsgTalk, MsgPlayer).</summary>
    public static unsafe partial class MobileLayouts
    {
        /// <summary>Data of a layout that is not in Layout itself (kept apart so Hot Reload never needs new fields on
        /// Layout/Field): the nested renumberings and the "structpb" flag.</summary>
        sealed partial class LayoutExt
        {
            /// <summary>ProtoRenumber: server field of a sub-message -> how its own fields are renumbered.</summary>
            public Dictionary<int, Layout> Nested;
            /// <summary>"structpb" layouts: a C struct on the server, protobuf on the mobile (MobileLayouts.ProtoStruct.cs).</summary>
            public bool ServerStruct;

            static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Layout, LayoutExt> table =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Layout, LayoutExt>();

            public static LayoutExt Get(Layout l)
            {
                LayoutExt e;
                return l != null && table.TryGetValue(l, out e) ? e : null;
            }

            public static LayoutExt For(Layout l)
            {
                return table.GetOrCreateValue(l);
            }

            const int MaxNestDepth = 8, MaxProtoField = 536870911;

            /// <summary>"2.3 1" (or "2.5.1 4 ..."): inside the server's sub-message field 2, server field 3 -> mobile field 1.
            /// The outer field itself still needs its own line ("2 2"); every occurrence of it (repeated or not) is renumbered.</summary>
            public static void ParseNestedRenumber(Layout l, string[] p)
            {
                var path = p[0].Split('.');
                if (path.Length < 2 || path.Length > MaxNestDepth + 1)
                    throw new Exception("bad nested field " + p[0]);
                Layout cur = l;
                for (int k = 0; k < path.Length - 1; k++)
                {
                    int outer = ParseFieldNumber(path[k]);
                    var ext = LayoutExt.For(cur);
                    if (ext.Nested == null)
                        ext.Nested = new Dictionary<int, Layout>();
                    Layout sub;
                    if (!ext.Nested.TryGetValue(outer, out sub))
                        ext.Nested[outer] = sub = new Layout { ServerId = cur.ServerId, Name = cur.Name + "." + outer, Kind = Kind.ProtoRenumber };
                    cur = sub;
                }
                int inner = ParseFieldNumber(path[path.Length - 1]);
                for (int k = 1; k < p.Length; k++)
                    cur.Fields.Add(new Field { Proto = inner, Offset = ParseFieldNumber(p[k]), Type = "proto", In = k == 1 });
            }

            public static int ParseFieldNumber(string s)
            {
                int n = int.Parse(s);
                if (n < 1 || n > MaxProtoField)
                    throw new Exception("bad field number " + s);
                return n;
            }
        }

        /// <summary>Copy a protobuf body with its field numbers changed (server -> mobile, or back); unmapped fields
        /// are dropped, the values (varint / bytes / fixed) are copied unchanged, except a sub-message with a nested
        /// renumbering ("2.3 1" lines), which is renumbered the same way (depth limited by the layout, at most 8).</summary>
        static byte[] Renumber(byte* p, int len, Layout l, bool toMobile)
        {
            var map = new Dictionary<int, List<int>>();
            foreach (var f in l.Fields)
            {
                int from = toMobile ? f.Proto : f.Offset, to = toMobile ? f.Offset : f.Proto;
                if (!toMobile && (!f.In || map.ContainsKey(from)))
                    continue;// back to the server: only the first mobile field of a line
                List<int> list;
                if (!map.TryGetValue(from, out list)) map[from] = list = new List<int>();
                list.Add(to);
            }
            var ext = LayoutExt.Get(l);
            var nested = ext != null ? ext.Nested : null;
            var o = new MemoryStream();
            int i = 0;
            while (i < len)
            {
                ulong key = MobileWire.ReadVarint(p, len, ref i);
                int field = (int)(key >> 3), wire = (int)(key & 7);
                int valStart = i;
                if (wire == 0) MobileWire.ReadVarint(p, len, ref i);
                else if (wire == 1) i += 8;
                else if (wire == 5) i += 4;
                else if (wire == 2)
                {
                    // QahrMobile: a hostile length (negative after an int cast, or past the end) used to move i
                    // backwards = an endless loop on the receive thread
                    ulong n = MobileWire.ReadVarint(p, len, ref i);
                    if (n > (ulong)(len - i)) break;
                    i += (int)n;
                }
                else break;
                if (i > len) break;
                List<int> targets;
                if (!map.TryGetValue(field, out targets))
                    continue;
                foreach (int to in targets)
                {
                    Layout sub = null;
                    if (wire == 2 && nested != null)
                        nested.TryGetValue(toMobile ? field : to, out sub);// keyed by the server field
                    if (sub != null)
                    {
                        int j = valStart;
                        MobileWire.ReadVarint(p, len, ref j);// the length, checked above: j..i is the sub-message
                        byte[] inner = Renumber(p + j, i - j, sub, toMobile);
                        MobileWire.WriteVarint(o, (ulong)((to << 3) | 2));
                        MobileWire.WriteVarint(o, (ulong)inner.Length);
                        o.Write(inner, 0, inner.Length);
                        continue;
                    }
                    MobileWire.WriteVarint(o, (ulong)((to << 3) | wire));
                    for (int k = valStart; k < i; k++) o.WriteByte(p[k]);
                }
            }
            return o.ToArray();
        }
    }
}
