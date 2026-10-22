using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateExchangeShopGoods(this ServerSockets.Packet stream, CMsgExchangeShopGoods obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgExchangeShopGoods);

            return stream;
        }
        public static unsafe void GetExchangeShopGoods(this ServerSockets.Packet stream, out CMsgExchangeShopGoods pQuery)
        {
            pQuery = new CMsgExchangeShopGoods();
            pQuery = stream.ProtoBufferDeserialize<CMsgExchangeShopGoods>(pQuery);
        }
    }
    [ProtoContract]
    public class CMsgExchangeShopGoods
    {
        [ProtoMember(1, IsRequired = true)]
        public uint NpcID;
        [ProtoMember(2, IsRequired = true)]
        public uint ShopID;
        [ProtoMember(3, IsRequired = true)]
        public uint UID;
        [ProtoMember(4, IsRequired = true)]
        public uint TimeInSeconds;
        [ProtoMember(5, IsRequired = true)]
        public uint uk5;
        [ProtoMember(6, IsRequired = true)]
        public Item[] Items;
        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Index;
            [ProtoMember(2, IsRequired = true)]
            public uint Count;
            [ProtoMember(3, IsRequired = true)]
            public ulong uk; // [ProtoFix 2026-10-14] client CGoodsInfoPB.uk is uint64 (was uint)
        }
    }
}
