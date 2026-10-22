
using Qahr.Mobile.Client;
using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateFruitMachine(this ServerSockets.Packet stream, MsgFruitMachine.ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgFruitMachine);
            return stream;
        }

        public static void GetFruitMachine(this ServerSockets.Packet stream, out MsgFruitMachine.ProtoStructure pQuery)
        {
            pQuery = new MsgFruitMachine.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgFruitMachine.ProtoStructure>(pQuery);
        }

    }
    public class MsgFruitMachine
    {
        [Flags]
        public enum Types : uint
        {
            Join = 0,
            Exit = 1,
        }
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public Types Type;//0
            [ProtoMember(2, IsRequired = true)]
            public long Member2;//26694
            [ProtoMember(3, IsRequired = true)]
            public long Member3;//1516200000
            [ProtoMember(4, IsRequired = true)]
            public long Member4;//78124928
            [ProtoMember(5, IsRequired = true)]
            public long Member5;//1
        }
        [PacketAttribute(GamePackets.MsgFruitMachine)]
        public unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            // [FruitMachine 2026-10-14] was an echo stub (it made the client close the window); real logic:
            Qahr.Mobile.FruitMachine.FruitMachineModule.OnMachine(client, stream);
        }
    }
}
