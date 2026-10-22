using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgServerConfig
    {
       public static unsafe ServerSockets.Packet ServerConfig(this ServerSockets.Packet stream)
       {
           stream.InitWriter();

           stream.Write(0);
           stream.Write(0);
           stream.Write(15);
           stream.Write(9999);
           stream.Write(3);
           stream.Write((byte)1);
           stream.Finalize(GamePackets.MsgPCServerConfig);
           return stream;
       }
       public static unsafe ServerSockets.Packet CreateBenefitsConfig(this ServerSockets.Packet stream)
       {
           stream.InitWriter();

           stream.Write(4);

           stream.Write(1);
           stream.Write(20191203);
           stream.Write(20200106);

           stream.Write(3);
           stream.Write(20190917);
           stream.Write(20201220);

           stream.Write(12);
           stream.Write(20200509);
           stream.Write(20200601);


           stream.Finalize(GamePackets.MsgBenefitsConfig);
           return stream;
       }
    }
}
