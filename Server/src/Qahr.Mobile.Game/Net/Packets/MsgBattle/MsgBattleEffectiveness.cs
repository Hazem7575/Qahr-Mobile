using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgBattleEffectiveness
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public long Member1;//3570583
            [ProtoMember(2, IsRequired = true)]
            public long Member2;//216
            [ProtoMember(3, IsRequired = true)]
            public long Member3;//0
        }
        public static unsafe ServerSockets.Packet CreateBattleEffectiveness(this ServerSockets.Packet stream, ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBattleEffectiveness);
            return stream;

        }
        public static void GetBattleEffectiveness(this ServerSockets.Packet stream, out ProtoStructure pQuery)
        {
            pQuery = new ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<ProtoStructure>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgBattleEffectiveness)]
        public unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            ProtoStructure Info;
            stream.GetBattleEffectiveness(out Info);
            
        }
    }
}
