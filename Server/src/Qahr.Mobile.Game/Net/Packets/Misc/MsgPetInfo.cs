using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgPetInfo
    {
        public static unsafe ServerSockets.Packet CreatePetInfo(this ServerSockets.Packet stream, uint UID, uint PetID, uint PetType, uint Mesh, ushort Range, ushort X, ushort Y, string Name)
        {
            stream.InitWriter();
            stream.Write(UID);
            stream.Write(PetID);
            stream.Write(PetType);
            stream.Write((ulong)Mesh);
            stream.Write(Range);
            stream.Write(X);
            stream.Write(Y);
            stream.Write(Name, 32);

            stream.Finalize(GamePackets.MsgPetInfo);
            return stream;
        }
      
    }
}
