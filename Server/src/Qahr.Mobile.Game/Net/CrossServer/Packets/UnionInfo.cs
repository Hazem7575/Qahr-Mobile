using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class UnionInfo
    {
        public static unsafe ServerSockets.Packet UnionInfoCreate(this ServerSockets.Packet stream, uint UID, Role.Instance.Union.Member.MilitaryRanks Rank
              , string Name, string LeaderName, byte IsKingDom)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write((uint)Rank);
            stream.Write(Name, 32);
            stream.Write(LeaderName, 32);
            stream.Write(IsKingDom);

            stream.Finalize(PacketTypes.InterServer_UnionInfo);
            return stream;
        }
        public static unsafe void GetUnionInfo(this ServerSockets.Packet stream, out uint UID, out Role.Instance.Union.Member.MilitaryRanks rank
              , out string GuildName, out string LeaderName, out byte IsKingDom)
        {
            UID = stream.ReadUInt32();
            rank = (Role.Instance.Union.Member.MilitaryRanks)stream.ReadUInt32();
            GuildName = stream.ReadCString(32);
            LeaderName = stream.ReadCString(32);
            IsKingDom = stream.ReadUInt8();
        }
    }
}
