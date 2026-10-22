using Qahr.Mobile.Client;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Qahr.Mobile
{
    [ProtoContract]
    public class MsgEnemyInvadeArenic
    {
        public enum TypeID : byte
        {
            Status = 0,
            SendScore = 1,
            Info = 2,
            AddPoint = 4,
            Export = 5,
            Quit = 6,
        }
        [ProtoMember(1, IsRequired = true)]
        public uint Type;
        [ProtoMember(2, IsRequired = true)]
        public uint Type2;
        [ProtoMember(3, IsRequired = true)]
        public uint Point;
        [ProtoMember(4, IsRequired = true)]
        public Player[] Players;
        [ProtoContract]
        public class Player
        {
            [ProtoMember(1)]
            public uint UID;
            [ProtoMember(2)]
            public uint ServerID;
            [ProtoMember(3)]
            public uint Class;
            [ProtoMember(4)]
            public uint Level;
            [ProtoMember(5)]
            public uint u5;
            [ProtoMember(6)]
            public uint u6;
            [ProtoMember(7)]
            public uint Damage;
            [ProtoMember(8)]
            public string Name;
        }
        public static implicit operator MsgEnemyInvadeArenic(Qahr.Mobile.ServerSockets.Packet stream)
        {
            MsgEnemyInvadeArenic pQuery = new MsgEnemyInvadeArenic();
            pQuery = stream.ProtoBufferDeserialize(pQuery);
            return pQuery;
        }
        public static implicit operator Qahr.Mobile.ServerSockets.Packet(MsgEnemyInvadeArenic obj)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(obj);
                stream.Finalize(Qahr.Mobile.Game.GamePackets.MsgEnemyInvadeArenic);
                return stream;
            }
        }
        [PacketAttribute(Qahr.Mobile.Game.GamePackets.MsgEnemyInvadeArenic)]
        public static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgEnemyInvadeArenic Info = new MsgEnemyInvadeArenic();
            Info = stream.ProtoBufferDeserialize<MsgEnemyInvadeArenic>(Info);
            switch ((MsgEnemyInvadeArenic.TypeID)Info.Type)
            {
                default:
                   // Console.WriteLine(" EnemyInvade Info.Type: " + Info.Type);
                    break;
                //case MsgEnemyInvadeArenic.TypeID.Quit:
                //    {
                //        if (user.EnemyInvade.GET_Match != null)
                //            user.EnemyInvade.GET_Match.End(user);
                //        break;
                //    }
            }
        }
    }
}
