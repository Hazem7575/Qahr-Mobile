using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// Reads the layouts from Data\mobile_layouts\*.txt (one file per group of packets, in name order). A file is
    /// re-read about a second after it is saved (MobileProtocol.WatchFiles).
    ///
    ///   [serverId name kind args]   a layout header; the lines below it until the next header belong to it
    ///   # ...                       a comment (also after a line)
    ///
    /// Lines per kind:
    ///   ProtoStruct ([id name size]):         protoField structOffset type [countOffset] [in|out]
    ///       types: u8 u16 u32 u64 i32 str:N list32 (countOffset follows) strpack:f1,f2,.. (out only) strlist (u8 count, u16 length + bytes each)
    ///       units: "protoField offset units stride countOffset" + "> subField offsetInUnit type" lines (out only)
    ///   ProtoRenumber ([id name proto]):      serverField mobileField [mobileField...]
    ///       nested: "2.3 1" = inside the sub-message server field 2 (repeated or not), server field 3 -> mobile 1
    ///       (both ways; field 2 itself still needs its own "2 x" line; up to 8 levels: "2.5.1 4")
    ///   StructPb ([id name structpb sSize]):  protoField serverOffset type [countOffset] [in|out]
    ///       server C struct <-> mobile protobuf (the reverse of [id name size]); sSize = the server struct incl. the
    ///       4-byte header; types u8 u16 u32 i32 u64 str:N list32 (countOffset follows); out writes zero values too
    ///   Shift ([id name shift from to]):      headerOffset type map s=m s>m s<m ... (= both ways, > out only, < in only)
    ///   Struct ([id name struct sSize mSize]): serverOffset mobileOffset type [in|out] [map s=m ...]   (type "strings" = a string list)
    ///       units: "serverOffset mobileOffset units:stride[:bytes] count sCountOffset mCountOffset u8|u16|u32 [in|out]"
    ///       = count units of stride bytes (only the first "bytes" copied); the count line must come first
    /// </summary>
    public static unsafe partial class MobileLayouts
    {
        public static string Folder
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_layouts"); }
        }

        public static void Load()
        {
            var map = new Dictionary<ushort, Layout>();
            string[] files = Directory.Exists(Folder) ? Directory.GetFiles(Folder, "*.txt") : new string[0];
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
                LoadFile(file, map);
            MyConsole.WriteLine("[Mobile] " + map.Count + " packet layouts from " + files.Length + " files in " + Folder, ConsoleColor.Cyan);
            layouts = map;
        }

        static void LoadFile(string file, Dictionary<ushort, Layout> map)
        {
            Layout cur = null;
            int lineNo = 0;
            try
            {
                foreach (var raw in File.ReadAllLines(file))
                {
                    lineNo++;
                    string line = raw;
                    int c = line.IndexOf('#');
                    if (c >= 0) line = line.Substring(0, c);
                    line = line.Trim();
                    if (line.Length == 0)
                        continue;
                    if (line[0] == '[')
                    {
                        cur = ParseHeader(line);
                        if (map.ContainsKey(cur.ServerId))
                            MyConsole.WriteLine("[Mobile] " + Path.GetFileName(file) + " line " + lineNo + ": " + cur.ServerId + " already has a layout, replaced", ConsoleColor.Yellow);
                        map[cur.ServerId] = cur;
                        continue;
                    }
                    if (cur == null)
                        continue;
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts[0] == ">")
                    {
                        // a field of the last "units" line: "> subField offsetInUnit type"
                        var owner = cur.Fields.Count > 0 ? cur.Fields[cur.Fields.Count - 1] : null;
                        if (owner == null || owner.Units == null)
                            throw new Exception("'>' line without a units line above it");
                        var u = new Field { Proto = int.Parse(parts[1]), Offset = int.Parse(parts[2]), Type = parts[3].ToLowerInvariant() };
                        u.Size = SizeOf(u.Type);
                        owner.Units.Add(u);
                        continue;
                    }
                    ParseLine(cur, parts);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] " + Path.GetFileName(file) + " line " + lineNo + ": " + e.Message, ConsoleColor.Red);
            }
        }

        static Layout ParseHeader(string line)
        {
            var h = line.Trim('[', ']').Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var l = new Layout { ServerId = ushort.Parse(h[0]), Name = h.Length > 1 ? h[1] : "", Kind = Kind.ProtoStruct };
            string kind = h.Length > 2 ? h[2] : "";
            switch (kind)
            {
                case "proto": l.Kind = Kind.ProtoRenumber; break;
                case "structpb":
                    // server C struct, mobile protobuf: the ProtoStruct kind flagged as reversed (MobileLayouts.ProtoStruct.cs)
                    l.BaseSize = LayoutExt.ParseStructSize(h);
                    LayoutExt.For(l).ServerStruct = true;
                    break;
                case "same": l.Kind = Kind.Same; break;
                case "shift":
                    l.Kind = Kind.Shift;
                    l.ServerSize = int.Parse(h[3]);
                    l.MobileSize = int.Parse(h[4]);
                    break;
                case "struct":
                    l.Kind = Kind.Struct;
                    l.ServerSize = int.Parse(h[3]);
                    l.MobileSize = int.Parse(h[4]);
                    break;
                default:
                    l.BaseSize = int.Parse(kind);
                    break;
            }
            return l;
        }

        static void ParseLine(Layout l, string[] p)
        {
            switch (l.Kind)
            {
                case Kind.ProtoRenumber:
                    if (p[0].IndexOf('.') >= 0)
                    {
                        LayoutExt.ParseNestedRenumber(l, p);// "2.3 1": a field inside sub-message 2
                        return;
                    }
                    // "serverField mobileField [mobileField...]": a server field may feed several mobile fields;
                    // back to the server only the first one is used
                    for (int k = 1; k < p.Length; k++)
                        l.Fields.Add(new Field { Proto = int.Parse(p[0]), Offset = int.Parse(p[k]), Type = "proto", In = k == 1 });
                    return;
                case Kind.Shift:
                    {
                        var f = new Field { Offset = int.Parse(p[0]), Type = p[1].ToLowerInvariant() };
                        f.Size = SizeOf(f.Type);
                        ParseValueMap(f, p, 3);// p[2] = "map"
                        l.Fields.Add(f);
                        return;
                    }
                case Kind.Struct:
                    {
                        // "serverOffset mobileOffset type [in|out] [map s=m ...]"
                        if (p.Length > 2 && p[2].StartsWith("units:"))
                        {
                            LayoutExt.ParseStructUnits(l, p);
                            return;
                        }
                        var f = new Field { Proto = int.Parse(p[0]), Offset = int.Parse(p[1]), Type = p[2].ToLowerInvariant() };
                        f.Size = SizeOf(f.Type);
                        int m = Array.IndexOf(p, "map");
                        ParseDirection(f, m < 0 ? p : p.Take(m).ToArray(), 3);
                        if (m >= 0)
                            ParseValueMap(f, p, m + 1);
                        l.Fields.Add(f);
                        return;
                    }
                case Kind.ProtoStruct:
                    {
                        if (LayoutExt.IsServerStruct(l))
                        {
                            LayoutExt.ParseStructPbLine(l, p);
                            return;
                        }
                        var f = new Field { Proto = int.Parse(p[0]), Offset = int.Parse(p[1]), Type = p[2].ToLowerInvariant() };
                        if (f.Type.StartsWith("strpack:"))
                        {
                            // CNetStringPacker: u8 count, then u16 length + bytes per string; "0" = an empty string
                            foreach (var s in f.Type.Substring(8).Split(','))
                                f.Pack.Add(int.Parse(s));
                            f.Size = 1;
                            f.In = false;
                            l.Fields.Add(f);
                            return;
                        }
                        if (f.Type == "units")
                        {
                            // "protoField offset units stride countOffset" + "> ..." lines (server -> mobile only)
                            f.Stride = int.Parse(p[3]);
                            f.CountOffset = int.Parse(p[4]);
                            f.Units = new List<Field>();
                            f.In = false;
                            l.Fields.Add(f);
                            return;
                        }
                        int next = 3;
                        if (f.Type == "list32")
                            f.CountOffset = int.Parse(p[next++]);
                        f.Size = SizeOf(f.Type);
                        ParseDirection(f, p, next);
                        l.Fields.Add(f);
                        return;
                    }
            }
        }

        /// <summary>Value pairs from p[from]: s=m both ways, s>m server -> mobile only, s&lt;m mobile -> server only.</summary>
        static void ParseValueMap(Field f, string[] p, int from)
        {
            for (int k = from; k < p.Length; k++)
            {
                char op = p[k].IndexOf('>') >= 0 ? '>' : p[k].IndexOf('<') >= 0 ? '<' : '=';
                var kv = p[k].Split(op);
                ulong s = ulong.Parse(kv[0]), m = ulong.Parse(kv[1]);
                if (op != '<') f.ToMobile[s] = m;
                if (op != '>') f.ToServer[m] = s;
            }
        }

        /// <summary>"in" = mobile -> server only, "out" = server -> mobile only (default both).</summary>
        static void ParseDirection(Field f, string[] p, int from)
        {
            for (int k = from; k < p.Length; k++)
            {
                if (p[k] == "in") f.Out = false;
                else if (p[k] == "out") f.In = false;
            }
        }

        static int SizeOf(string type)
        {
            switch (type)
            {
                case "u8": return 1;
                case "u16": return 2;
                case "u32": case "i32": case "list32": return 4;
                case "u64": return 8;
                case "strings": return 0;// variable (Restruct)
                case "strlist": return 0;// variable, out only (ProtoToStruct)
            }
            if (type.StartsWith("str:"))
                return int.Parse(type.Substring(4));
            throw new Exception("unknown type " + type);
        }
    }
}
