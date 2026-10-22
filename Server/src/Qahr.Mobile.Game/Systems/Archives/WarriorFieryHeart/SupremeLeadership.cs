using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Role.Instance;
namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class SupremeLeadership
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.SupremeLeadership:
                        {
                            if (user.Player.SupremeLeadershipCount > 5)
                                break;
                            user.Player.SupremeLeadershipCount += 1;
                            user.Send(stream.InteractionCreate(Attack));

                            uint Power;
                            int Sec;
                            GetUpdateAmount(user.Player.SupremeLeadershipCount, out Power, out Sec);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.SupremeLeadership, (int)Sec, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.SupremeLeadership, (uint)Sec, Power, (uint)user.Player.SupremeLeadershipCount, MsgUpdate.DataType.ArchiveSkill, true);
                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {

                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        target.Player.AddSpellFlag(MsgUpdate.Flags.SupremeLeadership, (int)Sec, true);
                                        target.Player.SendUpdate(stream, MsgUpdate.Flags.SupremeLeadership, (uint)Sec, Power, (uint)user.Player.SupremeLeadershipCount, MsgUpdate.DataType.ArchiveSkill, true);
                                    }
                                }
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }

        }
       public static void GetUpdateAmount(int stacks, out uint Power, out int stepsecs)
        {
            Power = 0;
            stepsecs = 0;
            switch (stacks)
            {
                case 1: { stepsecs = 15; Power = 100100; break; }
                case 2: { stepsecs = 15; Power = 150150; break; }
                case 3: { stepsecs = 10; Power = 200200; break; }
                case 4: { stepsecs = 8; Power = 250250; break; }
                case 5: { stepsecs = 5; Power = 350300; break; }
            }
        }
    }
}