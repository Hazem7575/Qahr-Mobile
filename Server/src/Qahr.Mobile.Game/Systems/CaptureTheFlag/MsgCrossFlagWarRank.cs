using Qahr.Mobile.Client;
using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static void GetCrossFlagWarRank(this ServerSockets.Packet stream, out MsgCrossFlagWarRank.MsgCrossFlagWarRankPB pQuery)
        {
            pQuery = new MsgCrossFlagWarRank.MsgCrossFlagWarRankPB();
            pQuery = stream.ProtoBufferDeserialize<MsgCrossFlagWarRank.MsgCrossFlagWarRankPB>(pQuery);
        }
        public static ServerSockets.Packet CreateCrossFlagWarRank(this ServerSockets.Packet stream, MsgCrossFlagWarRank.MsgCrossFlagWarRankPB obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCrossFlagWarRank);
            return stream;
        }
    }
    public class MsgCrossFlagWarRank
    {
        [ProtoContract]
        public class MsgCrossFlagWarRankPB
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action;
            [ProtoMember(2, IsRequired = true)]
            public uint IdAltar;
            [ProtoMember(3, IsRequired = true)]
            public uint Page;
            [ProtoMember(4, IsRequired = true)]
            public uint Total;
            [ProtoMember(5, IsRequired = true)]
            public MsgCrossFlagWarRankListPB[] info;

            [ProtoContract]
            public class MsgCrossFlagWarRankListPB
            {
                [ProtoMember(1, IsRequired = true)]
                public uint Rank;
                [ProtoMember(2, IsRequired = true)]
                public string ServerName;
                [ProtoMember(3, IsRequired = true)]
                public string SynName;
                [ProtoMember(4, IsRequired = true)]
                public uint Score;
                [ProtoMember(5, IsRequired = true)]
                public uint TarScore;
                [ProtoMember(6, IsRequired = true)]
                public uint MemberCount;
                [ProtoMember(7, IsRequired = true)]
                public uint IdSyn;
                [ProtoMember(8, IsRequired = true)]
                public uint LocalServer;
                [ProtoMember(9, IsRequired = true)]
                public ulong Money;
                [ProtoMember(10, IsRequired = true)]
                public uint EMoney;
            }
        }
        [PacketAttribute(GamePackets.MsgCrossFlagWarRank)]
        public unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            MsgCrossFlagWarRankPB Info;
            stream.GetCrossFlagWarRank(out Info);
           
        }
    }
}