
using Qahr.Mobile.Client;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static void GetFruitMachineGo(this ServerSockets.Packet stream, out MsgFruitMachineGo pQuery)
        {
            pQuery = new MsgFruitMachineGo();
            pQuery = stream.ProtoBufferDeserialize<MsgFruitMachineGo>(pQuery);
        }

        public static ServerSockets.Packet CreateFruitMachineGo(this ServerSockets.Packet stream, MsgFruitMachineGo obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize((object)obj);
            stream.Finalize(GamePackets.MsgFruitMachineGo);
            return stream;
        }


    }
    public class MsgFruitMachineGo
    {
        public enum TypeID : byte
        {
            Play,
            Exit,
        }
        [ProtoMember(1)]
        public MsgFruitMachineGo.TypeID Type;
        [ProtoMember(2)]
        public MsgFruitMachineGo.Item[] Unknown2;
        public class Item
        {
            [ProtoMember(1)]
            public uint Type;
            [ProtoMember(2)]
            public uint Unknown2;
        }
         [PacketAttribute(GamePackets.MsgFruitMachineGo)]
        public static void FruitMachineGo(GameClient client, Qahr.Mobile.ServerSockets.Packet stream)
        {
            // [FruitMachine 2026-10-14] was an echo stub; real logic (validate, deduct, draw, pay, 2214 GoResult):
            Qahr.Mobile.FruitMachine.FruitMachineModule.OnGo(client, stream);
        }




    }
}
