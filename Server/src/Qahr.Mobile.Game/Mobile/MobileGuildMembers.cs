using System;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// Guild member list (MsgSynMemberList 2278 -> mobile CMsgSynMemberList 10259) for mobile clients.
    /// CNetMsg::Create copies a received packet into a 1024-byte buffer at +4 of the message object, and the
    /// pointer CMsgSynMemberList::Process reads (+0x404) sits right after it: a packet longer than 1024 bytes
    /// overwrites that pointer and the message is ignored (no rows, "Online Members: 0"). A full page of 12
    /// SYN_MEMBER_INFO units (24 + 12 * 88 = 1080 bytes) is too long, so the page is sent in chunks of
    /// <see cref="Chunk"/> members; u8 21 = 1 on every chunk but the last tells Process not to refresh yet
    /// (the layout in Data\mobile_layouts\guild2.txt is sized for one chunk).
    /// </summary>
    public static class MobileGuildMembers
    {
        public const int Chunk = 6;

        /// <summary>Sends the page for a mobile client; false for other clients (the caller sends its own packet).</summary>
        public static bool Send(ServerSockets.Packet stream, Game.MsgServer.MsgGuildMembers.Action mode, int page,
            Role.Instance.Guild.Member[] members, Client.GameClient client)
        {
            if (!MobileEvents.IsMobile(client))
                return false;
            DateTime now = DateTime.Now;
            int first = page / 12 * 12;
            int total = Math.Min(12, Math.Max(0, members.Length - first));
            int online = 0;
            foreach (var m in members)
                if (m.IsOnline)
                    online++;
            int sent = 0;
            do
            {
                int count = Math.Min(Chunk, total - sent);
                bool more = sent + count < total;
                stream.InitWriter();
                stream.Write((uint)mode);
                stream.Write(page);
                stream.Write(count);
                stream.Write(online);
                stream.Write(more ? 0x100 : 0);// u8 21: 1 = more chunks follow
                for (int i = 0; i < count; i++)
                {
                    var member = members[first + sent + i];
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
                stream.Finalize(Game.GamePackets.MsgSynMemberList);
                client.Send(stream);
                sent += count;
            }
            while (sent < total);
            return true;
        }
    }
}
