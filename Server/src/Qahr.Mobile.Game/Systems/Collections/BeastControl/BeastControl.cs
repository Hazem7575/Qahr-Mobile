using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class BeastControl
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                if (user.Player.ContainFlag(MsgUpdate.Flags.Fly) || user.Player.OnTransform || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(user.Player.Map) || user.Player.ContainFlag(MsgUpdate.Flags.DefensiveStance))
                {

                    user.SendSysMesage("You can`t use this skill right now !");


                    return;
                }
                if (user.Player.Map == 3825 || user.Player.Map == 6011 || user.Player.Map == 3030 || user.Player.Map == 5342 || user.Player.Map == 3825 || user.Player.Map == 6521 || user.Player.Map == 9988 || user.Player.Map == 6891 || user.Player.Map == 5599 || user.Player.Map == 8521 || user.Player.Map == 1005 || user.Player.Map == 9855 || user.Player.Map == 3053 || user.Player.Map == 10230 || user.Player.Map == 1860 || user.Player.Map == 10741 || user.Player.Map == 22330 || user.Player.Map == 22331 || user.Player.Map == 22332 || user.Player.Map == 22333 || user.Player.Map == 22334 || user.Player.Map == 1860 || user.Player.Map == 1858 || user.Player.Map == 8881 || user.Player.Map == 8880 || user.Player.Map == 700 || MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
                {

                    user.SendSysMesage("You can't use this skill on this map.");


                    return;
                }

                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , 0, Attack.X, Attack.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);

                if (!user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                {
                    user.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true, 1);

                    user.Vigor = user.Status.MaxVigor;

                    user.Send(stream.ServerInfoCreate(user.Vigor));

                }
                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

            }
        }
    }
}
