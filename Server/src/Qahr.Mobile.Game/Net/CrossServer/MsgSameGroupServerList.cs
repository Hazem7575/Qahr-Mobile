using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSameGroupServerList
    {
        [ProtoContract]
        public class GroupServer
        {
            [ProtoMember(1, IsRequired = true)]
            public Server[] Servers;
            [ProtoMember(2, IsRequired = true)]
            public uint ServerID;
        }
        [ProtoContract]
        public class Server
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ServerID;
            [ProtoMember(2, IsRequired = true)]
            public uint Type;
            [ProtoMember(3, IsRequired = true)]
            public string Name;
            [ProtoMember(4)]
            public uint MapID;
            [ProtoMember(5)]
            public uint X;
            [ProtoMember(6)]
            public uint Y;
            [ProtoMember(7)]
            public uint groupid;
            [ProtoMember(8)]
            public uint unknown;
        }
        public static unsafe ServerSockets.Packet CreateGroupServerList(this ServerSockets.Packet stream, GroupServer obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgServerList);
            return stream;
        }

        public static unsafe void GetGroupServerList(this ServerSockets.Packet stream, out GroupServer pQuery)
        {
            pQuery = new GroupServer();
            pQuery = stream.ProtoBufferDeserialize<GroupServer>(pQuery);
        }
    }
}
