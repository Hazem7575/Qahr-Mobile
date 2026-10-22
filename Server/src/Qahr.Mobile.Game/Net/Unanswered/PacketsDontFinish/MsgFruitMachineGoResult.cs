
using Qahr.Mobile.Client;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static ServerSockets.Packet CreateFruitMachineGoResult(this ServerSockets.Packet stream, MsgFruitMachineGoResult obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize((object)obj);
            stream.Finalize(GamePackets.MsgFruitMachineGoResult);
            return stream;
        }

        public static void GetFruitMachineGoResult(this ServerSockets.Packet stream, out MsgFruitMachineGoResult pQuery)
        {
            pQuery = new MsgFruitMachineGoResult();
            pQuery = stream.ProtoBufferDeserialize<MsgFruitMachineGoResult>(pQuery);
        }

    }
    public class MsgFruitMachineGoResult
    {
        public enum FruitKind : byte
        {
        }
        [ProtoMember(1)]
        public MsgFruitMachineGoResult.FruitKind Type;
        [ProtoMember(2)]
        public uint Unknown2;
        [ProtoMember(3)]
        public uint Unknown3;
        [ProtoMember(4)]
        public uint Unknown4;

        [PacketAttribute(GamePackets.MsgFruitMachineGoResult)]
        public static void FruitMachineGoResult(GameClient client, Qahr.Mobile.ServerSockets.Packet stream)
        {
            MsgFruitMachineGoResult pQuery;
            stream.GetFruitMachineGoResult(out pQuery);
            MsgFruitMachineGoResult.FruitKind type = pQuery.Type;
            client.Send(stream.CreateFruitMachineGoResult(pQuery));
        }

       
    }
}
