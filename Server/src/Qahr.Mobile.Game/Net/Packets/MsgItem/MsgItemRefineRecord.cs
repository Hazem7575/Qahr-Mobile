using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
  public static class MsgItemRefineRecord
    {

      [ProtoContract]
      public class ItemRefineRecord
      {
          [ProtoMember(1, IsRequired=true)]
          public uint Type;
          [ProtoMember(2)]
          public uint Points;
          [ProtoMember(3, IsRequired=true)]
          public uint UnKnow;
          [ProtoMember(4, IsRequired = false)]
          public uint Unlocked;
          [ProtoMember(5, IsRequired = false)]
          public RefineRecord[] msgs;
      }
       [ProtoContract]
      public class RefineRecord
      {
           [ProtoMember(1, IsRequired = true)]
           public uint Index;
           [ProtoMember(2, IsRequired = true)]
           public uint ItemUID;
           [ProtoMember(3, IsRequired = true)]
           public uint ItemID;
           [ProtoMember(4, IsRequired = true)]
           public uint PurificationID;
           [ProtoMember(5, IsRequired = true)]
           public uint OpenStars;
           [ProtoMember(6, IsRequired = true)]
           public uint RequestStars;
           [ProtoMember(7, IsRequired = true)]
           public string str;
      }

      public static unsafe ServerSockets.Packet ItemRefineRecordCreate(this ServerSockets.Packet stream, ItemRefineRecord obj)
      {
          stream.InitWriter();

          stream.ProtoBufferSerialize(obj);

          stream.Finalize(GamePackets.MsgItemRefineRecord);
          return stream;
      }

      public static unsafe void GetItemRefineRecord(this ServerSockets.Packet stream, out ItemRefineRecord pQuery)
      {
          pQuery = new ItemRefineRecord();
          pQuery = stream.ProtoBufferDeserialize<ItemRefineRecord>(pQuery);
      }
      [PacketAttribute(GamePackets.MsgItemRefineRecord)]
      private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
      {
          ItemRefineRecord pQuery;
          pQuery = new ItemRefineRecord();
          pQuery.Type = 0;
          pQuery.Points = 54;
          client.Send(stream.ItemRefineRecordCreate(pQuery));
      }
    }
}
