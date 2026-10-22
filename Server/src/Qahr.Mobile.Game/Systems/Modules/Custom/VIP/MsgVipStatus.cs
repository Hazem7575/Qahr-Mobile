using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet VipStatusCreate(this ServerSockets.Packet stream, MsgVipStatus.VipFlags Mode)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Finalize(GamePackets.MsgVip);

            return stream;
        }


    }
    public unsafe struct MsgVipStatus
    {
        public enum VipFlags
        {
            PortalTeleport = 1,
            Avatar = 2,
            MoreForVip = 4,
            FrozenGrot = 8,
            TeleportTeam = 16,
            CityTeleport = 32,
            CityTeleportTeam = 64,
            VipLevelOne = PortalTeleport | Avatar | MoreForVip | FrozenGrot | VipHaire | BonusLotery | CityTeleport,
            BlessTime = 128,
            OflineTG = 256,
            RefinaryAndArtefacts = 512,
            Friends = 1024,
            VipHaire = 2048,
            Labirint = 4096,
            DailyQuests = 8192,
            VipFurniture = 16384,
            BonusLotery = 32768,
            FullVip = PortalTeleport | Avatar | MoreForVip | FrozenGrot | TeleportTeam
                | CityTeleport | CityTeleportTeam | BlessTime | OflineTG | RefinaryAndArtefacts
                | Friends | VipHaire | Labirint | DailyQuests | VipFurniture | BonusLotery,
            None = 65536
        }

    }
}
