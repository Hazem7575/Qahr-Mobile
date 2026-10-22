using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgCountryFlag
    {
        public static unsafe void GetCountryFlag(this ServerSockets.Packet stream, out uint UID, out ushort CountryID)
        {
            UID = stream.ReadUInt32();
            CountryID = stream.ReadUInt16();
        }

        public static unsafe ServerSockets.Packet CountryFlagCreate(this ServerSockets.Packet stream, uint UID, ushort CountryID)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(CountryID);

            stream.Finalize(GamePackets.MsgNationality);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgNationality)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint UID;
            ushort CountryID;
            stream.GetCountryFlag(out UID, out CountryID);

            user.Player.CountryID = CountryID;
            user.Player.View.SendView(stream.CountryFlagCreate(user.Player.UID, user.Player.CountryID), true);
        }
    }
}
