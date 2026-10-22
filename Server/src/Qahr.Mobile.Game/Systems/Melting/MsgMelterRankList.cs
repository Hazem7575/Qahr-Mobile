using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgMelterRankList
    {
        [ProtoContract]
        public class MsgMelterRankListProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint CountInPage;
            [ProtoMember(2, IsRequired = true)]
            public uint Page;
            [ProtoMember(3)]
            public uint Count;
            [ProtoMember(4, IsRequired = true)]
            public Instance[] Instances;
        }
        [ProtoContract]
        public class Instance
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public ulong ItemID; // [ProtoFix 2026-10-14] client CMsgMelterRankInfoPB.ItemID is uint64 (was uint)
            [ProtoMember(3, IsRequired = true)]
            public bool Bound;
            [ProtoMember(4, IsRequired = true)]
            public string Name;
            [ProtoMember(5, IsRequired = true)]
            public uint Unknown3;
        }
        public static unsafe ServerSockets.Packet CreateMelterRankList(this ServerSockets.Packet stream, MsgMelterRankListProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgMelterRankList);

            return stream;
        }
        public static unsafe void GetMelterRankList(this ServerSockets.Packet stream, out MsgMelterRankListProto pQuery)
        {
            pQuery = new MsgMelterRankListProto();
            pQuery = stream.ProtoBufferDeserialize<MsgMelterRankListProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgMelterRankList)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgMelterRankListProto Info;
            stream.GetMelterRankList(out Info);
            var Info2 = new MsgMelterRankListProto()
            {
                Page = Info.Page,
                Count = Math.Min((byte)100, (byte)Pool.MelterRankList.Count),
            };
            if (Info2.Count > 0)
            {
                uint sss = (ushort)Math.Min(Info2.Count - ((Info.Page - 1) * 10), 7);
                int rank = (int)(Info.Page - 1) * 7;
                Info2.CountInPage = sss;
                Info2.Instances = new Instance[sss];

                for (int i = rank; i < rank + sss; i++)
                {
                    try
                    {
                        var instance = Pool.MelterRankList.ToArray()[i];
                        Info2.Instances[i - rank] = new Instance();
                        Info2.Instances[i - rank].Name = instance.Name;
                        Info2.Instances[i - rank].Bound = instance.Bound;
                        Info2.Instances[i - rank].ItemID = instance.ItemID;
                        Info2.Instances[i - rank].Type = instance.Type;
                        Info2.Instances[i - rank].Unknown3 = instance.Unknown3;
                    }
                    catch { break; }
                }
            }
            else Info2.CountInPage = 0;
            client.Send(stream.CreateMelterRankList(Info2));
        }
    }
}
