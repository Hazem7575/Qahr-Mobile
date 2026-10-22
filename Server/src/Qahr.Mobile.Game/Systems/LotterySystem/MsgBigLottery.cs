using ProtoBuf;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgBigLottery
    {
        [ProtoContract]
        public class MsgBigLotteryProto
        {
            [ProtoMember(1, IsRequired = true)]
            public byte Type;
            [ProtoMember(2, IsRequired = true)]
            public ItemsProto[] Items;
        }
        [ProtoContract]
        public class ItemsProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint GateID; // max 5
            [ProtoMember(2, IsRequired = true)]
            public uint RankItems;
            [ProtoMember(3, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(4, IsRequired = true)]
            public uint UK;//0
            [ProtoMember(5, IsRequired = true)]
            public string Name;
        }
        public static unsafe void GetBigLottery(this ServerSockets.Packet stream, out MsgBigLotteryProto pQuery)
        {
            pQuery = new MsgBigLotteryProto();
            pQuery = stream.ProtoBufferDeserialize(pQuery);
        }
        public static unsafe ServerSockets.Packet CreateBigLottery(this ServerSockets.Packet stream, MsgBigLotteryProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBigLottery);
            return stream;
        }
        // Winners board: newest entries first, capped; Pool.LotteryList is shared by all client threads.
        public const int BoardSize = 10;
        private static readonly object BoardLock = new object();
        public static void AddWinner(Pool.LvLottery winner)
        {
            lock (BoardLock)
            {
                Pool.LotteryList.Add(winner);
                if (Pool.LotteryList.Count > BoardSize)
                    Pool.LotteryList.RemoveRange(0, Pool.LotteryList.Count - BoardSize);
            }
        }
        public static ItemsProto[] Board()
        {
            Pool.LvLottery[] list;
            lock (BoardLock)
                list = Pool.LotteryList.ToArray();
            var items = new ItemsProto[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var w = list[list.Length - 1 - i];
                items[i] = new ItemsProto() { Name = w.Name, GateID = w.GateID, ItemID = w.ItemID, RankItems = w.RanksItem };
            }
            return items;
        }
        [Packet(GamePackets.MsgBigLottery)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgBigLotteryProto Info;
            stream.GetBigLottery(out Info);
            switch (Info.Type)
            {
                case 0:
                    {
                        Info.Items = Board();
                        client.Send(stream.CreateBigLottery(Info));
                        break;
                    }
                default:
                    {
                        MyConsole.WriteLine("Type not found ->" + Info.Type, System.ConsoleColor.Yellow);
                        break;
                    }
            }
        }
    }
}