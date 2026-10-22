using Qahr.Mobile.Client;
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
    public static class MsgSynEventList
    {
        [ProtoContract]
        public class MsgGuildAffairs
        {
            [ProtoMember(1, IsRequired = true)]
            public MsgSynEventList.MsgGuildAffairs.Message[] Items;

            [ProtoContract]
            public class Message
            {
                [ProtoMember(1, IsRequired = true)]
                public ulong Time; // [ProtoFix 2026-10-14] client CSynEventInfo.Time is uint64 (was uint)
                [ProtoMember(2, IsRequired = true)]
                public string message;
            }

        }
        public static unsafe ServerSockets.Packet CreateGuildAffairs(this ServerSockets.Packet stream, MsgSynEventList.MsgGuildAffairs obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSynEventList);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgSynEventList)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.MyGuild == null)
                return;
            List<Role.Instance.Guild.GuildMessage> message = user.Player.MyGuild.Message;
            int count = message.Count;
            for (int index1 = 0; index1 < message.Count / 10 + 1; ++index1)
            {
                MsgSynEventList.MsgGuildAffairs msgGuildAffairs = new MsgSynEventList.MsgGuildAffairs();
                msgGuildAffairs.Items = new MsgSynEventList.MsgGuildAffairs.Message[count > 10 ? 10 : count];
                for (int index2 = 0; index2 < msgGuildAffairs.Items.Length && count > 0; ++index2)
                {
                    msgGuildAffairs.Items[index2] = new MsgSynEventList.MsgGuildAffairs.Message();
                    msgGuildAffairs.Items[index2].Time = message[count - 1].Time;
                    msgGuildAffairs.Items[index2].message = message[count - 1].message;
                    --count;
                }
                user.Send(stream.CreateGuildAffairs(msgGuildAffairs));
            }
        }



    }
}


