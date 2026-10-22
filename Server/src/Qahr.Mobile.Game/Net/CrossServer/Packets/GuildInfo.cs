using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
   public static class GuildInfo
    {
       public static unsafe ServerSockets.Packet GuildInfoCreate(this ServerSockets.Packet stream, uint UID, Role.Flags.GuildMemberRank Rank
           , string GuildName, string LeaderName)
       {
           stream.InitWriter();

           stream.Write(UID);
           stream.Write((uint)Rank);
           stream.Write(GuildName, 32);
           stream.Write(LeaderName, 32);

           stream.Finalize(PacketTypes.InterServer_GuildInfo);
           return stream;
       }
       public static unsafe void GetGuildInfo(this ServerSockets.Packet stream, out uint UID, out Role.Flags.GuildMemberRank rank
           , out string GuildName, out string LeaderName)
       {
           UID = stream.ReadUInt32();
           rank = (Role.Flags.GuildMemberRank)stream.ReadUInt32();
           GuildName = stream.ReadCString(32);
           LeaderName = stream.ReadCString(32);

       }
    }
}
