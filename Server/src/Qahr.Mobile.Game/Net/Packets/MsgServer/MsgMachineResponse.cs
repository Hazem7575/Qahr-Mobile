using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet MachineResponseCreate(this ServerSockets.Packet stream, MsgMachine.SlotMachineSubType Mode, byte WheelOne
            , byte WheelTwo, byte WheelThree,uint NpcID)
        {
            stream.InitWriter();
            stream.Write((byte)Mode);//4
            stream.Write(WheelOne);//8
            stream.Write(WheelTwo);
            stream.Write(WheelThree);
            stream.Write((ulong)0);//if you have more money and add in warehause
            stream.Write(NpcID);
            stream.Write((uint)0);


            stream.Finalize(GamePackets.MsgSlotResult);
          
            return stream;
        }
    }
}
