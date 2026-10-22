using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
  public static  class MsgOverheadLeagueInfo
    {

      [ProtoContract]
      public class OverheadLeagueInfo
      {
          [ProtoMember(1,IsRequired= true)]
          public uint Type;
          [ProtoMember(2,IsRequired=true)]
          public uint UID;
          [ProtoMember(3, IsRequired = true)]
          public uint DwParam1;
          [ProtoMember(4, IsRequired = true)]
          public uint DwParam2;
          [ProtoMember(5, IsRequired = true)]
          public uint DwParam3;
          [ProtoMember(6, IsRequired = true)]
          public uint ID;
          [ProtoMember(7, IsRequired = true)]
          public string Text;
      }

      public static unsafe ServerSockets.Packet CreateOverheadLeagueInfo(this ServerSockets.Packet stream, OverheadLeagueInfo obj)
      {
          stream.InitWriter();
          stream.ProtoBufferSerialize(obj);
          stream.Finalize(GamePackets.MsgOverheadLeagueInfo);
          return stream;
      }
    }
}
