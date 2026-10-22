using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgWeather
    {
       public enum WeatherType : uint
       {
           Nothing = 1,
           Rain = 2,
           Snow = 3,
           RainWind = 4,
           AutumnLeaves = 5,
           CherryBlossomPetals = 7,
           CherryBlossomPetalsWind = 8,
           BlowingCotten = 9,
           Atoms = 10
       }
       public static unsafe ServerSockets.Packet WeatherCreate(this ServerSockets.Packet stream, WeatherType Type, uint nIntensity, uint nDir, uint nColor, uint nSpeedSecs)
       {
           stream.InitWriter();

           stream.Write((uint)Type);
           stream.Write((uint)nIntensity);
           stream.Write((uint)nDir);
           stream.Write((uint)nColor);
           stream.Write((uint)nSpeedSecs);

           stream.Finalize(GamePackets.MsgWeather);
           return stream;
       }


    }
}
