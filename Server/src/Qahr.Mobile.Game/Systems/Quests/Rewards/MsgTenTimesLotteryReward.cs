using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgTenTimesLotteryReward
    {
        [ProtoContract]
        public class TenTimesLotteryReward
        {
            [ProtoMember(1, IsRequired = true)]
            public uint[] ID { get; set; }
        }
        public static unsafe ServerSockets.Packet CreateTenTimesLotteryReward(this ServerSockets.Packet stream, TenTimesLotteryReward obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgTenTimesLotteryReward);
            return stream;
        }
        public static unsafe void GetTenTimesLotteryReward(this ServerSockets.Packet stream, out TenTimesLotteryReward pQuery)
        {
            pQuery = new TenTimesLotteryReward();
            pQuery = stream.ProtoBufferDeserialize(pQuery);
        }
    }
}