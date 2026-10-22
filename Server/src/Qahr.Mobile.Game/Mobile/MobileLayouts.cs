using System;
using System.Collections.Generic;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: converts packets whose format differs between this server (7957) and the Conquer Online Mobile
    /// client (1.1.2.3). Each packet is described by a layout in Data\mobile_layouts\*.txt (MobileLayouts.Loader.cs),
    /// in one of these kinds:
    ///
    ///   [2427 MsgItem 95]                  protobuf on the server, a C struct on the mobile  (MobileLayouts.ProtoStruct.cs)
    ///   [2361 MsgTalk proto]               protobuf on both, other field numbers             (MobileLayouts.ProtoRenumber.cs)
    ///   [2205 MsgRegister same]            the same struct on both, only the id differs
    ///   [2126 MsgTaskStatus shift 24 12]   struct on both, the tail moved to another offset  (MobileLayouts.StructStruct.cs)
    ///   [2254 MsgFriend struct 124 76]     struct on both, fields at other offsets           (MobileLayouts.StructStruct.cs)
    ///   [2261 MsgSyncAction structpb 24]   a C struct on the server (24 bytes with the header), protobuf on the mobile
    ///                                      (the reverse of the first kind)                    (MobileLayouts.ProtoStruct.cs)
    ///
    /// A proto layout can renumber inside a sub-message: "2.3 1" = in server field 2, field 3 -> mobile field 1.
    /// A struct layout can copy a counted run of units: "8 8 units:32 count 6 6 u16".
    ///
    /// Incoming (mobile -> server) runs before the server handlers, outgoing (server -> mobile) before encryption.
    /// </summary>
    public static unsafe partial class MobileLayouts
    {
        enum Kind { ProtoStruct, ProtoRenumber, Same, Shift, Struct }

        class Field
        {
            /// <summary>ProtoStruct: the protobuf field; ProtoRenumber: the server field; Struct: the server offset.</summary>
            public int Proto;
            /// <summary>The offset in the mobile struct (ProtoRenumber: the mobile field number).</summary>
            public int Offset;
            public int Size, CountOffset = -1;
            /// <summary>ProtoStruct "units": a repeated sub-message written as fixed-size structs (Stride bytes each);
            /// Units are its fields ("> subField subOffset type" lines, offsets inside one unit).</summary>
            public int Stride;
            public List<Field> Units;
            public string Type;
            public List<int> Pack = new List<int>();// strpack: the proto string fields, in order
            public bool In = true, Out = true;
            /// <summary>Shift: server value -> mobile value of a header field (and back).</summary>
            public Dictionary<ulong, ulong> ToMobile = new Dictionary<ulong, ulong>(), ToServer = new Dictionary<ulong, ulong>();
        }

        class Layout
        {
            public ushort ServerId;
            public string Name;
            public Kind Kind;
            /// <summary>ProtoStruct: the struct length (4-byte header included) without lists / strings.</summary>
            public int BaseSize;
            /// <summary>Shift: where the tail starts; Struct: the whole struct length, on each side.</summary>
            public int ServerSize, MobileSize;
            public List<Field> Fields = new List<Field>();
        }

        const string ServerSeal = "TQServer";

        static Dictionary<ushort, Layout> layouts = new Dictionary<ushort, Layout>();

        /// <summary>True when the packet has a layout of any kind.</summary>
        public static bool Has(ushort serverId)
        {
            return layouts.ContainsKey(serverId);
        }

        /// <summary>Mobile packet -> the server format, rewritten in place under the server id.</summary>
        public static void Incoming(ServerSockets.Packet stream, ushort serverId, ushort mobileId)
        {
            Layout l;
            if (mobileId == serverId || !layouts.TryGetValue(serverId, out l) || l.Kind == Kind.Same)
                return;// only packets the client sent under its own id are in the mobile format
            int len = Math.Min((int)*(ushort*)stream.Memory, stream.Size);
            switch (l.Kind)
            {
                case Kind.ProtoStruct:
                    if (LayoutExt.IsServerStruct(l))
                        Replace(stream, serverId, LayoutExt.MobileToStruct(stream.Memory + 4, len - 4, l), false);
                    else
                        Replace(stream, serverId, StructToProto(stream.Memory, len, l), true);
                    break;
                case Kind.ProtoRenumber:
                    Replace(stream, serverId, Renumber(stream.Memory + 4, len - 4, l, false), true);
                    break;
                case Kind.Shift:
                    Replace(stream, serverId, ShiftToServer(stream.Memory, len, l), false);
                    break;
                case Kind.Struct:
                    Replace(stream, serverId, Restruct(stream.Memory, len, l, false), false);
                    break;
            }
        }

        /// <summary>Server packet -> the mobile packet (mobile id, server seal), or null when it needs no conversion.</summary>
        public static byte[] Outgoing(ServerSockets.Packet msg, ushort mobileId)
        {
            if (msg == null || msg.Size < 4)
                return null;
            Layout l;
            if (!layouts.TryGetValue(*(ushort*)(msg.Memory + 2), out l) || l.Kind == Kind.Same)
                return null;
            int len = Math.Min((int)*(ushort*)msg.Memory, msg.Size);
            switch (l.Kind)
            {
                case Kind.ProtoStruct:
                    if (LayoutExt.IsServerStruct(l))
                        return Seal(LayoutExt.StructToMobile(msg.Memory, len, l), mobileId, true);
                    return ProtoToStruct(msg.Memory, len, l, mobileId);
                case Kind.ProtoRenumber:
                    return Seal(Renumber(msg.Memory + 4, len - 4, l, true), mobileId, true);
                case Kind.Shift:
                    return Seal(ShiftToMobile(msg.Memory, len, l), mobileId, false);
                case Kind.Struct:
                    return Seal(Restruct(msg.Memory, len, l, true), mobileId, false);
            }
            return null;
        }

        /// <summary>A finished mobile packet: header + data + seal. withHeader: data is only the body (4-byte header
        /// added here); otherwise data is a whole struct whose length at 0 is already set.</summary>
        static byte[] Seal(byte[] data, ushort mobileId, bool withHeader)
        {
            int head = withHeader ? 4 : 0;
            // QahrMobile: the result is copied into a Packet buffer of Packet.MAX_SIZE (SecuritySocket.Send) and its
            // length is a u16: a bigger packet would overflow that native buffer, so it is refused (Send disconnects)
            if (head + data.Length + ServerSeal.Length > ServerSockets.Packet.MAX_SIZE)
                throw new InvalidOperationException("[Mobile] converted packet too big for mobile id " + mobileId + ": " + (head + data.Length));
            byte[] pkt = new byte[head + data.Length + ServerSeal.Length];
            Buffer.BlockCopy(data, 0, pkt, head, data.Length);
            if (withHeader)
                MobileWire.WriteU16(pkt, 0, (ushort)(4 + data.Length));
            MobileWire.WriteU16(pkt, 2, mobileId);
            Encoding.ASCII.GetBytes(ServerSeal, 0, ServerSeal.Length, pkt, head + data.Length);
            return pkt;
        }

        /// <summary>Replace the packet with converted data under the server id (+ the seal). isBody: data is the body
        /// after the 4-byte header; otherwise a whole struct.</summary>
        static void Replace(ServerSockets.Packet stream, ushort serverId, byte[] data, bool isBody)
        {
            int head = isBody ? 4 : 0;
            int total = head + data.Length;
            // QahrMobile: a converted packet can be longer than the one received (varints, string lists); it must
            // still fit the receive Packet (Packet.MAX_SIZE, native memory) with its seal, or it is refused
            if (total + ServerSockets.Packet.SealSize > ServerSockets.Packet.MAX_SIZE)
                throw new InvalidOperationException("[Mobile] converted packet too big for server id " + serverId + ": " + total);
            fixed (byte* b = data)
                stream.memcpy(stream.Memory + head, b, data.Length);
            *(ushort*)stream.Memory = (ushort)total;
            *(ushort*)(stream.Memory + 2) = serverId;
            stream.Seek(total);
            stream.WriteSeal();
            stream.Size = stream.Position;
            stream.Seek(4);
        }
    }
}
