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
    public static class MsgFactionList
    {
        [ProtoContract]
        public class MsgGuildList
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public MsgFactionList.MsgGuildList.Guild[] Items;

            [ProtoContract]
            public class Guild
            {
                [ProtoMember(1, IsRequired = true)]
                public uint id;
                [ProtoMember(2, IsRequired = true)]
                public uint Rank;
                [ProtoMember(3, IsRequired = true)]
                public string Name;
                [ProtoMember(4, IsRequired = true)]
                public string LeaderName;
                [ProtoMember(5, IsRequired = true)]
                public uint Level;
                [ProtoMember(6, IsRequired = true)]
                public uint MembersCount;
                [ProtoMember(7, IsRequired = true)]
                public uint MembersCountMix;
                [ProtoMember(8, IsRequired = true)]
                public ulong Prestige;
                [ProtoMember(9, IsRequired = true)]
                public bool Join;

                public Guild(Role.Instance.Guild obj, uint rank)
                {
                     id = obj.Info.GuildID;
                     Rank = rank;
                     Name = obj.GuildName;
                     LeaderName = obj.Info.LeaderName;
                     Level = obj.Info.Level;
                     MembersCount = (uint)obj.Members.Count;
                     MembersCountMix = 999;
                     Prestige = obj.Info.Prestige;
                     Join = obj.Info.RecruitmentON;
                     if (Join)
                        return;
                     Join = obj.Info.RecruitmentOFF;
                }
            }
           
            
        }
        public static unsafe ServerSockets.Packet CreateGuildList(this ServerSockets.Packet stream, MsgFactionList.MsgGuildList obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgFactionList);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgFactionList)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            Role.Instance.Guild[] array = Role.Instance.Guild.GuildPoll.Values.Where<Role.Instance.Guild>((Func<Role.Instance.Guild, bool>)(p => p.Info.Prestige > 0)).OrderByDescending<Role.Instance.Guild, ulong>((Func<Role.Instance.Guild, ulong>)(p => p.Info.Prestige)).ToArray<Role.Instance.Guild>();
            MsgFactionList.MsgGuildList msgGuildList = new MsgFactionList.MsgGuildList();
            msgGuildList.Items = new MsgFactionList.MsgGuildList.Guild[array.Length > 10 ? 10 : array.Length];
            for (int index = 0; index < msgGuildList.Items.Length; ++index)
                msgGuildList.Items[index] = new MsgFactionList.MsgGuildList.Guild(array[index], (uint)(index + 1));
            user.Send(stream.CreateGuildList(msgGuildList));
        }
    }
}


