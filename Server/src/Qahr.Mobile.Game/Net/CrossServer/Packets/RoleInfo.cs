using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Packets
{
    public static class RoleInfo
    {
        public static unsafe ServerSockets.Packet InterServerRoleInfoCreate(this ServerSockets.Packet stream, Role.Player user)
        {
            stream.InitWriter();
            stream.Write((ulong)0);
            if (user.Nobility != null)
                stream.Write(user.Nobility.Donation);
            else
                stream.ZeroFill(8);
            stream.Write(user.ChampionPoints);
            stream.Write(user.RacePoints);
            stream.Write((uint)user.DonatePoints);
            stream.Write((uint)user.OnlineMinutes);
            stream.Write((uint)user.GodzillaPoints);

            stream.Write(user.SavePromote);
            stream.Write(user.SavePromote1);
            stream.Write(user.SavePromote2);
            stream.Write(user.SavePromote3);
            stream.Write(user.SavePromote4);
            stream.Write(user.SavePromote5);
            stream.Write(user.SavePromote6);
            stream.Write(user.SavePromote7);
            stream.Write(user.SavePromote8);
            stream.Write(user.SavePromote9);
            stream.Write(user.SavePromote10);
            stream.Write(user.SavePromote11);
            stream.Write(user.SavePromote12);

            stream.Write(user.Online);
            stream.Write(user.RewardPoints);
            stream.Write(user.ItemRewordChristmas);
            stream.Write(user.Owner.ArenaHonorPoints);
            stream.Write(user.FirstRebornLevel);
            stream.Write(user.SecoundeRebornLevel);
            stream.Write(user.SecoundeClass);
            stream.Write((byte)(user.Reincarnation ? 1 : 0));
            stream.Write(user.QuizPoints);
            stream.Write(user.WHMoney);
            stream.Write(user.TournamentKills);
            stream.Write(user.InventorySashCount);
            stream.Write(user.MyFootBallPoints);
            stream.Write(user.ExtraAtributes);

            stream.Write(user.VipLevel);
            stream.Write(user.ExpireVip.Ticks);

            stream.Finalize(PacketTypes.InterServer_RoleInfo);
            return stream;
        }
        public static unsafe void GetInterServerRoleInfo(this ServerSockets.Packet stream, Role.Player user)
        {
            stream.ReadUInt64();
            user.Nobility.Donation = stream.ReadUInt64();
            user.AddChampionPoints(stream.ReadUInt32(), false);
            user.RacePoints = stream.ReadUInt32();
            user.DonatePoints = stream.ReadUInt32();
            user.OnlineMinutes = stream.ReadUInt32();
            user.GodzillaPoints = stream.ReadUInt32();
            user.SavePromote = stream.ReadUInt32();
            user.SavePromote1 = stream.ReadUInt32();
            user.SavePromote2 = stream.ReadUInt32();
            user.SavePromote3 = stream.ReadUInt32();
            user.SavePromote4 = stream.ReadUInt32();
            user.SavePromote5 = stream.ReadUInt32();
            user.SavePromote6 = stream.ReadUInt32();
            user.SavePromote7 = stream.ReadUInt32();
            user.SavePromote8 = stream.ReadUInt32();
            user.SavePromote9 = stream.ReadUInt32();
            user.SavePromote10 = stream.ReadUInt32();
            user.SavePromote11 = stream.ReadUInt32();
            user.SavePromote12 = stream.ReadUInt32();


            user.Online = stream.ReadUInt32();
            user.RewardPoints = stream.ReadUInt32();
            user.ItemRewordChristmas = stream.ReadUInt32();
            user.Owner.ArenaHonorPoints = stream.ReadUInt32();
            user.FirstRebornLevel = stream.ReadUInt8();
            user.SecoundeRebornLevel = stream.ReadUInt8();
            user.SecoundeClass = stream.ReadUInt8();
            user.Reincarnation = stream.ReadUInt8() == 1 ? true : false;
            user.QuizPoints = stream.ReadUInt32();
            user.WHMoney = stream.ReadUInt64();
            user.TournamentKills = stream.ReadUInt32();
            user.InventorySashCount = stream.ReadUInt16();
            user.MyFootBallPoints = stream.ReadUInt32();
            user.ExtraAtributes = stream.ReadUInt16();

            user.VipLevel = stream.ReadUInt8();
            user.ExpireVip = DateTime.FromBinary(stream.ReadInt64());

        }
    }
}
