using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgTaskRank
    {
        // client CMsgTaskRank id (GamePackets.MsgTaskRank 2035 is unknown to the client)
        public const ushort PacketID = 9018;
        [ProtoContract]
        public class TaskRankProto
        {
            [ProtoMember(1, IsRequired = true)]
            public long Action;//1

            [ProtoMember(2, IsRequired = true)]
            public long Member2;//50025


            [ProtoMember(3, IsRequired = true)]
            public long Member3;//295

            [ProtoMember(4, IsRequired = true)]
            public long Member4;//24

            [ProtoMember(5, IsRequired = true)]
            public long Member5;//1
            [ProtoMember(6, IsRequired = true)]
            public List<ProtoStructureInfo> Items = new List<ProtoStructureInfo>();

            [ProtoMember(7, IsRequired = true)]
            public long Member7;//6

            [ProtoMember(8, IsRequired = true)]
            public long Member8;//1

        }
        [ProtoContract]
        public class ProtoStructureInfo
        {
            [ProtoMember(1, IsRequired = true)]
            public long Member1;//1

            [ProtoMember(2, IsRequired = true)]
            public long Member2;//1004930

            [ProtoMember(3, IsRequired = true)]
            public long Member3;//90

            [ProtoMember(4, IsRequired = true)]
            public long Member4;//3632007

            [ProtoMember(5, IsRequired = true)]
            public string Member5;//>FreeBunny

            [ProtoMember(6, IsRequired = true)]
            public string Member6;// client field 6 is a string

            [ProtoMember(7, IsRequired = true)]
            public string Member7;//Bitcoin

            [ProtoMember(8, IsRequired = true)]
            public long Member8;//26

        }
        public static unsafe ServerSockets.Packet CreateTaskRank(this ServerSockets.Packet stream, TaskRankProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(PacketID);
            return stream;

        }
        public static void GetTaskRank(this ServerSockets.Packet stream, out TaskRankProto pQuery)
        {
            pQuery = new TaskRankProto();
            pQuery = stream.ProtoBufferDeserialize<TaskRankProto>(pQuery);
        }
        [PacketAttribute(PacketID)]
        public unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            TaskRankProto Rank;
            stream.GetTaskRank(out Rank);
            switch (Rank.Action)
            {
                case 0:
                case 1:
                    {
                        // reply with the list of the requested type; empty list for types without data
                        var Info = new TaskRankProto();
                        Info.Member2 = Rank.Member2;
                        Info.Member3 = Rank.Member3;
                        Info.Member4 = Rank.Member4;
                        Info.Member7 = Rank.Member7;
                        Info.Member8 = Rank.Member8;
                        Info.Member5 = 1;
                        Info.Items = new List<ProtoStructureInfo>();
                        Dictionary<uint, Role.Instance.Flowers.Flower> list = null;
                        if (Rank.Member2 == 50025)
                            list = Pool.GirlsFlowersRanking.Peonies;
                        else if (Rank.Member2 == 50026)
                            list = Pool.BoysFlowersRanking.Peonies;
                        if (list != null)
                        {
                            var top = list.Values.ToArray().Where(p => p.Rank >= 1 && p.Rank <= 10 && p.Amount > 0).OrderBy(p => p.Rank);
                            foreach (var Peonies in top)
                                Info.Items.Add(new ProtoStructureInfo { Member1 = Peonies.Rank, Member2 = Peonies.UID, Member3 = Peonies.Amount, Member4 = Peonies.Mesh, Member5 = Peonies.Name, Member7 = Peonies.GuildName, Member8 = 26 });
                        }
                        user.Send(stream.CreateTaskRank(Info));
                        break;
                    }
            }
        }
    }
}
