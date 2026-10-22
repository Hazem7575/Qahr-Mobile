using ProtoBuf;
using Qahr.Mobile.Client;
using System.Collections.Generic;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateBetLevel(this ServerSockets.Packet stream, MsgBetLevel pQuery)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pQuery);
            stream.Finalize(GamePackets.MsgBetLevel);
            return stream;
        }
        public static void GetBetLevel(this ServerSockets.Packet stream, out MsgBetLevel pQuery)
        {
            pQuery = new MsgBetLevel();
            pQuery = stream.ProtoBufferDeserialize<MsgBetLevel>(pQuery);
        }

    }
    [ProtoContract]
    public class MsgBetLevel
    {
        public enum _Type : byte
        {
            Show = 0,
        }
        [ProtoMember(1, IsRequired = true)]
        public byte Type;//0

        [ProtoMember(2, IsRequired = true)]
        public uint Member2;//0

        [ProtoMember(3, IsRequired = true)]
        public uint UID;//5546776

        [ProtoMember(4, IsRequired = true)]
        public uint Member4;//0

        [ProtoMember(5, IsRequired = true)]
        public ulong Member5;//0 // [ProtoFix 2026-10-14] client CMsgBetLevelPB #5 is uint64 (was uint)

        // [ProtoFix 2026-10-14] client CMsgBetLevelPB has only #1-#5 (no nested list message in msgbetlevel.pb.cc):
        // #6 and #7 were dropped by the client. Not serialized any more; fields kept for server code.
        public uint Member6;//0 // was [ProtoMember(6, IsRequired = true)] // [ProtoFix 2026-10-14]

        public List<PlayerInfo> Items = new List<PlayerInfo>(); // was [ProtoMember(7, IsRequired = true)] // [ProtoFix 2026-10-14]

        [ProtoContract]
        public class PlayerInfo
        {
            [ProtoMember(1, IsRequired = true)]
            public byte Action;

            [ProtoMember(2, IsRequired = true)]
            public uint MoneyCps; //0 // 1 MoneyCps

            [ProtoMember(3, IsRequired = true)]
            public uint Level;

            [ProtoMember(4, IsRequired = true)]
            public uint UnKnow3;//0

            [ProtoMember(5, IsRequired = true)]
            public uint Win;//Win

            [ProtoMember(6, IsRequired = true)]
            public uint BestWin;//BestWin
        }
        [PacketAttribute(GamePackets.MsgBetLevel)]
        private static void Process(GameClient user, ServerSockets.Packet stream)
        {
            if (global::Qahr.Mobile.Poker.PokerBoYi.Handle(user, stream)) return;// [Poker 2026-10-14] real BoYi level/EXP (was a fixed level 9)
            MsgBetLevel pQuery;
            stream.GetBetLevel(out pQuery);
            switch (pQuery.Type)
            {
                case (byte)_Type.Show:
                    {
                        pQuery.Member4 = 9;

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)0, MoneyCps = 0, Level = 1, UnKnow3 = 1, Win = 0, BestWin = 0 });

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)1, MoneyCps = 0, Level = 1, UnKnow3 = 0, Win = 0, BestWin = 0 });

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)1, MoneyCps = 1, Level = 1, UnKnow3 = 0, Win = 0, BestWin = 0 });

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)2, MoneyCps = 0, Level = 1, UnKnow3 = 0, Win = 0, BestWin = 0 });

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)2, MoneyCps = 1, Level = 1, UnKnow3 = 0, Win = 0, BestWin = 0 });

                        pQuery.Items.Add(new PlayerInfo { Action = (byte)3, MoneyCps = 0, Level = 1, UnKnow3 = 0, Win = 0, BestWin = 0 });

                        user.Send(stream.CreateBetLevel(pQuery));
                        break;
                    }
            }
        }

    }
}
