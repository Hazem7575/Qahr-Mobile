using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildMembersListCreate(this ServerSockets.Packet stream, MsgGuildMembers.Action Mode, int Page, Role.Instance.Guild.Member[] Members, Client.GameClient client)
        {
            if (global::Qahr.Mobile.Phone.MobileGuildMembers.Send(stream, Mode, Page, Members, client)) return stream;// QahrMobile: chunks of 6 (MobileGuildMembers.cs)
            stream.InitWriter();
            DateTime now = DateTime.Now;
            // QahrMobile: the mobile client asks with the first index of a page of 12 (0, 12, 24, ...) and shows the
            // online count from offset 16 (CMsgSynMemberList::Process)
            int offest = Page / 12 * 12;
            int Count = Math.Min(12, Math.Max(0, Members.Length - offest));
            stream.Write((uint)Mode);
            stream.Write(Page);
            stream.Write(Count);
            stream.Write(Members.Count(m => m.IsOnline));
            stream.Write(0);
            for (int index = 0; index < Count; ++index)
            {
                if (Members.Length > offest + index)
                {
                    Role.Instance.Guild.Member member = Members[offest + index];
                    stream.Write(member.UID);
                    stream.Write(member.Name, 32);
                    stream.Write(member.IsOnline ? 1 : 0);
                    stream.Write(member.NobilityRank);
                    stream.Write((uint)member.Graden);
                    stream.Write(member.Level);
                    stream.Write((uint)member.Rank);
                    stream.Write(member.TotalDonation);
                    stream.Write(0);
                    stream.Write((uint)0);
                    stream.Write(member.PrestigePoints);
                    stream.Write(member.BattlePower);

                    stream.Write(member.Class);
                    stream.Write((uint)((ulong)(now.Ticks - member.LastLogin) / 10000000));
                    stream.Write(member.Mesh);
                    stream.Write(member.Command);
                    stream.Write(0);
                }
            }
            stream.Finalize(GamePackets.MsgSynMemberList);

            client.Send(stream);
            return stream;
        }

     
    }

    public struct MsgGuildMembers
    {
        public enum Action : uint
        {
            MembersList = 0,
            ListRanks = 1
        }


        [PacketAttribute(GamePackets.MsgSynMemberList)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Program.ServerConfig.IsInterServer)
                return;
            if (user.Player.MyGuild == null)
                return;
            Role.Instance.Guild.Member[] array = user.Player.MyGuild.Members.Values.ToArray<Role.Instance.Guild.Member>();
            Array.Sort<Role.Instance.Guild.Member>(array, (Comparison<Role.Instance.Guild.Member>)((f1, f2) => f2.IsOnline.CompareTo(f1.IsOnline)));
            if (array != null)
            {
                MsgGuildMembers.Action Mode = (MsgGuildMembers.Action)stream.ReadUInt32();
                uint num = stream.ReadUInt32();
                stream.GuildMembersListCreate(Mode, (int)num, array, user);
            }
        }
    }
}
