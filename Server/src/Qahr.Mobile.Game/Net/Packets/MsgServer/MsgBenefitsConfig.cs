using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgJiangMessage
    {
       public static unsafe ServerSockets.Packet CreateJiangMessage(this ServerSockets.Packet stream, uint SpellID)
       {
           stream.InitWriter();

           stream.Write((ushort)SpellID);


           stream.Finalize(GamePackets.MsgMagicCoat);
           return stream;
       }
       
    }
}
