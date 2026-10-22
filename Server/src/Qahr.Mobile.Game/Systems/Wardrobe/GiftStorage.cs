using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class GiftStorage
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(3, IsRequired = true)]
            public uint dwparam3;
            [ProtoMember(4, IsRequired = true)]
            public uint dwparam4;
            [ProtoMember(5, IsRequired = true)]
            public uint dwparam5;
            [ProtoMember(6, IsRequired = true)]
            public uint dwparam6;
            [ProtoMember(7, IsRequired = true)]
            public uint dwparam7;
            // [ProtoFix 2026-10-14] client CMsgPlayerNpcInfoPB #8 is a string (was uint, always sent as varint 0 and skipped).
            // Never filled by the server; null = not sent.
            [ProtoMember(8)]
            public string Plus;
            [ProtoMember(9, IsRequired = true)]
            public uint Bless;
            [ProtoMember(10, IsRequired = true)]
            public uint Type1;//??
            [ProtoMember(11, IsRequired = true)]
            public uint dwparam11;
            [ProtoMember(12, IsRequired = true)]
            public uint dwparam12;
            [ProtoMember(13, IsRequired = true)]
            public uint dwparam13;
            [ProtoMember(14, IsRequired = true)]
            public uint dwparam14;
            [ProtoMember(15, IsRequired = true)]
            public uint Type2;
            [ProtoMember(16, IsRequired = true)]
            public uint dwparam16;
            [ProtoMember(17, IsRequired = true)]
            public uint dwparam17;
            [ProtoMember(18, IsRequired = true)]
            public uint dwparam18;
            [ProtoMember(19, IsRequired = true)]
            public uint TimeLeft;
            [ProtoMember(20, IsRequired = true)]
            public uint dwparam20;
            [ProtoMember(21, IsRequired = true)]
            public uint Stack;
            [ProtoMember(22, IsRequired = true)]
            public uint MinDurability;
            [ProtoMember(23, IsRequired = true)]
            public uint MaxDurability;
        }

        [Flags]
        public enum Action : uint
        {
            Equip = 1,
            Retrive = 2,
            Update = 3,
            AddToWardRobe = 4,
        }
        public static unsafe ServerSockets.Packet CreatePlayerNpcInfo(this ServerSockets.Packet stream, ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgPlayerNpcInfo);

            return stream;
        }
        public static unsafe void GetPlayerNpcInfo(this ServerSockets.Packet stream, out ProtoStructure pQuery)
        {
            pQuery = new ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<ProtoStructure>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgPlayerNpcInfo)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            ProtoStructure pQuery;
            stream.GetPlayerNpcInfo(out pQuery);
            switch ((Action)pQuery.Action)
            {

                default:
                    MyConsole.WriteLine("Unknown Action Type [" + (uint)pQuery.Action + "] GiftStorage."); 
                    client.Send(stream.CreatePlayerNpcInfo(pQuery)); 
                    break;
            }
        }

    }
}