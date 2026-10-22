using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSynPrestige
    {
        [ProtoContract]
        public class MsgGuildPrestige
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public ulong Prestige;
        }
        public static void GetGuildPrestige(this ServerSockets.Packet stream, out MsgSynPrestige.MsgGuildPrestige pQuery)
        {
            pQuery = new MsgSynPrestige.MsgGuildPrestige();
            pQuery = stream.ProtoBufferDeserialize<MsgSynPrestige.MsgGuildPrestige>(pQuery);
        }
        public static unsafe ServerSockets.Packet CreateGuildPrestige(this ServerSockets.Packet stream, MsgSynPrestige.MsgGuildPrestige obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSynPrestige);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgSynPrestige)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgSynPrestige.MsgGuildPrestige pQuery = (MsgSynPrestige.MsgGuildPrestige)null;
            stream.GetGuildPrestige(out pQuery);
            if (pQuery.Type != 0 || user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                return;
            int Amount = (int)pQuery.Prestige;
            if (Amount < 0)
                return;
            if (user.Player.Money >= (ulong)pQuery.Prestige)
            {
                user.Player.Money -= (ulong)pQuery.Prestige;
                user.Player.MyGuild.Info.Prestige += pQuery.Prestige;
                user.Player.MyGuildMember.MoneyDonate += (long)pQuery.Prestige;
                if (user.Player.MyGuild.Info.Prestige > 200000000000)
                    user.Player.MyGuild.Info.Prestige = 200000000000;
                user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_ADD_PRESTIGE@@" + user.Player.Name + "@@" + pQuery.Prestige.ToString() + "@@");
            }
            user.Send(stream.CreateGuildPrestige(pQuery));


        }


    }
}


