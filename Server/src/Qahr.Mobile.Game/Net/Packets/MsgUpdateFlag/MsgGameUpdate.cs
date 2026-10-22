using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GameUpdateCreate(this ServerSockets.Packet stream, uint UID
            , MsgGameUpdate.DataType ID, bool active, uint shownamount, uint time, uint amount = 0)
        {
            stream.InitWriter();


            stream.Write(UID);
            stream.Write((uint)1);//count
            stream.Write((uint)ID);
            stream.Write((uint)(active ? (uint)(1 << 8) : 1));
            stream.Write(shownamount);
            stream.Write(time);
          if(ID == MsgGameUpdate.DataType.Decelerated)
              stream.Write(uint.MaxValue - amount);
            else
            stream.Write(amount);



          stream.Finalize(GamePackets.MsgRaceTrackStatus);
            return stream;
        }
    }
    public unsafe class MsgGameUpdate
    {
        public enum DataType : uint
        {
            Accelerated = 52,
            Decelerated = 53,
            Flustered = 54,
            Sprint = 55,
            DivineShield = 57,
            Stun = 58,
            Freeze = 59,
            Dizzy = 60,
            AzureShield = 93,
            SoulShacle = 111
        }
    }
}
