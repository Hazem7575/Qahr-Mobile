// [Poker 2026-10-14] Minimal exact protobuf-lite wire helpers (same format the client's *.pb.cc code reads).
using System;
using System.Collections.Generic;
using System.Text;

namespace Qahr.Mobile.Poker
{
    /// <summary>Protobuf message writer. Field order = call order (the client parser accepts any order).</summary>
    public sealed class PbW
    {
        readonly List<byte> b = new List<byte>(64);
        public int Length { get { return b.Count; } }

        void Var(ulong v) { while (v > 0x7F) { b.Add((byte)(v | 0x80)); v >>= 7; } b.Add((byte)v); }
        public PbW U(int num, ulong v) { Var((ulong)(num << 3)); Var(v); return this; }
        public PbW U(int num, long v) { Var((ulong)(num << 3)); Var((ulong)v); return this; }          // int32/int64: two's complement, 10 bytes if negative
        public PbW I32(int num, int v) { Var((ulong)(num << 3)); Var((ulong)(long)v); return this; }
        public PbW B(int num, bool v) { Var((ulong)(num << 3)); b.Add(v ? (byte)1 : (byte)0); return this; }
        public PbW Opt(int num, ulong v) { if (v != 0) U(num, v); return this; }
        public PbW Bytes(int num, byte[] v) { Var((ulong)((num << 3) | 2)); Var((ulong)v.Length); b.AddRange(v); return this; }
        public PbW Str(int num, string s) { return Bytes(num, Encoding.UTF8.GetBytes(s ?? "")); }
        public PbW Msg(int num, PbW m) { return Bytes(num, m.ToArray()); }
        public PbW Rep(int num, IEnumerable<uint> vs) { foreach (var v in vs) U(num, v); return this; }      // unpacked repeated varints
        public PbW Rep(int num, IEnumerable<ulong> vs) { foreach (var v in vs) U(num, v); return this; }
        public byte[] ToArray() { return b.ToArray(); }

        /// <summary>Full game packet: [u16 len][u16 id][body][8-byte seal space]. GameClient.Send(byte[]) writes the seal.</summary>
        public byte[] Packet(ushort id)
        {
            var body = b.ToArray();
            var p = new byte[4 + body.Length + 8];
            int len = 4 + body.Length;
            p[0] = (byte)len; p[1] = (byte)(len >> 8);
            p[2] = (byte)id; p[3] = (byte)(id >> 8);
            Buffer.BlockCopy(body, 0, p, 4, body.Length);
            var seal = Encoding.ASCII.GetBytes("TQServer");
            Buffer.BlockCopy(seal, 0, p, 4 + body.Length, 8);
            return p;
        }
    }

    /// <summary>Protobuf message reader into field lists. Hostile input -> exception (callers drop the packet).</summary>
    public sealed class PbR
    {
        public readonly Dictionary<int, List<object>> F = new Dictionary<int, List<object>>();

        public PbR(byte[] b) : this(b, 0, b.Length) { }
        public PbR(byte[] b, int o, int end)
        {
            int guard = 0;
            while (o < end)
            {
                if (++guard > 512) throw new FormatException("too many fields");
                ulong tag = ReadVar(b, ref o, end);
                int num = (int)(tag >> 3), wt = (int)(tag & 7);
                if (num <= 0) throw new FormatException("bad field");
                object v;
                if (wt == 0) v = ReadVar(b, ref o, end);
                else if (wt == 2)
                {
                    ulong l = ReadVar(b, ref o, end);
                    if (l > (ulong)(end - o)) throw new FormatException("bad length");
                    var x = new byte[(int)l]; Buffer.BlockCopy(b, o, x, 0, (int)l); o += (int)l; v = x;
                }
                else if (wt == 1) { if (o + 8 > end) throw new FormatException("short"); v = BitConverter.ToUInt64(b, o); o += 8; }
                else if (wt == 5) { if (o + 4 > end) throw new FormatException("short"); v = (ulong)BitConverter.ToUInt32(b, o); o += 4; }
                else throw new FormatException("wire type " + wt);
                List<object> l2;
                if (!F.TryGetValue(num, out l2)) F[num] = l2 = new List<object>();
                if (l2.Count < 64) l2.Add(v);
            }
        }

        static ulong ReadVar(byte[] b, ref int o, int end)
        {
            ulong r = 0; int shift = 0;
            while (true)
            {
                if (o >= end || shift > 63) throw new FormatException("bad varint");
                byte x = b[o++];
                r |= (ulong)(x & 0x7F) << shift;
                if ((x & 0x80) == 0) return r;
                shift += 7;
            }
        }

        public bool Has(int n) { return F.ContainsKey(n); }
        public ulong U(int n) { List<object> l; return F.TryGetValue(n, out l) && l.Count > 0 && l[0] is ulong ? (ulong)l[0] : 0UL; }
        public uint U32(int n) { ulong v = U(n); return v > uint.MaxValue ? uint.MaxValue : (uint)v; }
        public bool Bool(int n) { return U(n) != 0; }
        public List<ulong> List(int n)
        {
            var r = new List<ulong>();
            List<object> l;
            if (!F.TryGetValue(n, out l)) return r;
            foreach (var o in l)
            {
                if (o is ulong) r.Add((ulong)o);
                else if (o is byte[]) { var b = (byte[])o; int p = 0; while (p < b.Length && r.Count < 64) r.Add(ReadVar(b, ref p, b.Length)); }  // packed
            }
            return r;
        }
        public string Str(int n) { List<object> l; return F.TryGetValue(n, out l) && l.Count > 0 && l[0] is byte[] ? Encoding.UTF8.GetString((byte[])l[0]) : ""; }

        /// <summary>Reads a client game packet body (skips the 4-byte header; ignores the 8-byte TQClient seal).</summary>
        public static PbR FromPacket(ServerSockets.Packet stream)
        {
            stream.Seek(0);
            ushort len = stream.ReadUInt16();
            stream.ReadUInt16();
            if (len < 4 || len > 1024) throw new FormatException("bad packet length");
            byte[] body = stream.ReadBytes(len - 4);
            return new PbR(body);
        }
    }
}
