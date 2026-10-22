using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaMatchScoreCreate(this ServerSockets.Packet stream, uint UidOne, string nameone, uint damageone
            , uint UidTwo, string nametwo, uint damagetwo)
        {
            stream.InitWriter();
            stream.Write(UidOne);
            stream.Write((uint)Database.GroupServerList.MyServerInfo.ID);
            stream.Write(nameone, 32);
            stream.Write(damageone);
            stream.Write(UidTwo);
            stream.Write((uint)Database.GroupServerList.MyServerInfo.ID);
            stream.Write(nametwo, 32);
            stream.Write(damagetwo);
            stream.Write(uint.MaxValue);
            stream.Finalize(GamePackets.MsgArenicScore);
            return stream;
        }

    }


}
