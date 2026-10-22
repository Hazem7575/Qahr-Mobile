using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgMagicColdTime
    {
       [ProtoContract]
       public class MagicColdTime
       {
           [ProtoMember(1, IsRequired = true)]
           public Spell[] Spells;

     
           public void WriteSpells(List<MsgSpell> array)
           {
               Spells = new Spell[array.Count];
               for (int x = 0; x < array.Count; x++)
               {
                   Spells[x] = new Spell();
                   Spells[x].SpellID = array[x].ID;
                   Spells[x].Time = (uint)System.Math.Max(0, array[x].GetColdTime); // [ProtoFix 2026-10-14] an expired cooldown (negative ms) was sent as a 10-byte varint = huge uint32 on the client
               }
           }
           public void ZeroEagleEye()
           {
               Spells = new Spell[1];
               Spells[0] = new Spell();
               Spells[0].SpellID = (ushort)Role.Flags.SpellID.EagleEye;
               Spells[0].Time = 0;
           }

       }
       [ProtoContract]
       public class Spell
       {
           [ProtoMember(1, IsRequired = true)]
           public uint SpellID;
           [ProtoMember(2, IsRequired = true)]
           public uint Time; // [ProtoFix 2026-10-14] client CMagicColdTimeInfoPB #2 is uint32 (was int)

  
       }

       public static unsafe ServerSockets.Packet MagicColdTimeCreate(this ServerSockets.Packet stream, MagicColdTime obj)
       {
           stream.InitWriter();
           // QahrMobile: mobile magic types (Data\mobile_magic.txt); safe to apply twice (the id sets do not overlap)
           if (obj != null && obj.Spells != null)
               foreach (var s in obj.Spells)
                   if (s != null)
                       s.SpellID = global::Qahr.Mobile.Phone.MobileMagic.ToMobile(s.SpellID);
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgMagicColdTime);

           return stream;
       }
       public static unsafe void GetMagicColdTime(this ServerSockets.Packet stream, out MagicColdTime pQuery)
       {
           pQuery = new MagicColdTime();
           pQuery = stream.ProtoBufferDeserialize<MagicColdTime>(pQuery);
       } 
   }
}
