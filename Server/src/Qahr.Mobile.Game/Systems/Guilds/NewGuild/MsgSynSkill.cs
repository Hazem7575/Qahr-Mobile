using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSynSkill
    {
        public enum _Type : byte
        {
            Upgrade,
            Show,
        }

        [ProtoContract]
        public class MsgGuildSkill
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;

            [ProtoMember(2, IsRequired = true)]
            public Skill[] Skills;

            [ProtoContract]
            public class Skill
            {
                [ProtoMember(1, IsRequired = true)]
                public uint ID;

                [ProtoMember(2, IsRequired = true)]
                public uint Level;
            }
        }

        public static ServerSockets.Packet CreateGuildSkill(this ServerSockets.Packet stream, MsgGuildSkill obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSynSkill);
            return stream;
        }

        public static void GetGuildSkill(this ServerSockets.Packet stream, out MsgGuildSkill pQuery)
        {
            pQuery = stream.ProtoBufferDeserialize<MsgGuildSkill>(new MsgGuildSkill());
        }

        [PacketAttribute(GamePackets.MsgSynSkill)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgGuildSkill pQuery;
            stream.GetGuildSkill(out pQuery);

            user.CreateBoxDialog("Sorry, This System Is Closed");
            return;
        }
    }
}