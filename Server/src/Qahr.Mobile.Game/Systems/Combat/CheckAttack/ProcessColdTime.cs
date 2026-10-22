using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
   public static class ProcessColdTime
    {
       public class Record
       {
           public Client.GameClient Attacker;
           public Client.GameClient Attacked;
       }
       public static ProcessAttackQueue ProcessAttack = new ProcessAttackQueue();



       public class ProcessAttackQueue : ConcurrentSmartThreadQueue<Record>
       {
           public ProcessAttackQueue()
               : base(2)
           {
               Start(5);
           }
           public void TryEnqueue(Record action)
           {
               Enqueue(action);
           }

           protected unsafe override void OnDequeue(Record record, int time)
           {
               var Attacker = record.Attacker;
               var Attacked = record.Attacked;
               if (Attacker != null && Attacked != null)
               {
                   if (!Attacked.Player.ContainFlag(MsgUpdate.Flags.xChillingSnow))
                   {
                       if (Role.Core.Rate(85 - 5 * ((Attacker.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DeathSigh) && Attacked.Player.BattlePower > Attacker.Player.BattlePower ? Attacker.Player.BattlePower : Attacked.Player.BattlePower) - Attacker.Player.BattlePower)))
                       {
                           int IncreaseColdTime = 0;
                           MsgSpell ClientSpell = null;
                           if (Attacker.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ChillingSnow, out ClientSpell))
                           {
                               var DBSpells = Pool.Magic[(ushort)Role.Flags.SpellID.ChillingSnow];
                               IncreaseColdTime = (int)DBSpells[(ushort)Math.Min(DBSpells.Count - 1, ClientSpell.Level)].Damage;
                           }
                           Attacked.Player.AddFlag(MsgUpdate.Flags.xChillingSnow, IncreaseColdTime, true);
                           List<MsgSpell> array = new List<MsgSpell>();

                           foreach (var spell in Attacked.MySpells.ClientSpells.Values)
                           {
                               if (spell.IsSpellWithColdTime)
                               {
                                   spell.ColdTime = spell.ColdTime.AddMilliseconds(IncreaseColdTime * 1000);
                                   if (spell.GetColdTime > 0)
                                       array.Add(spell);
                               }
                           }
                           if (array.Count > 0)
                           {
                               MsgMagicColdTime.MagicColdTime packet = new MsgMagicColdTime.MagicColdTime();
                               packet.WriteSpells(array);

                               using (var rec = new ServerSockets.RecycledPacket())
                               {
                                   var stream = rec.GetStream();
                                   Attacked.Send(stream.MagicColdTimeCreate(packet));
                               }
                           }
                       }
                   }
               }
           }
       }

    }
}
