using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Riding
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                if (user.Player.ContainFlag(MsgUpdate.Flags.Fly) || user.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || user.Player.OnTransform || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(user.Player.Map)||user.Player.ContainFlag(MsgUpdate.Flags.DefensiveStance))
                {

                    user.SendSysMesage("You can`t use this skill right now !");


                    return;
                }
                if (Pool.Constants.RemoveRide.Contains(user.Player.Map))
                {

                    user.SendSysMesage("You can't use this skill on this map.");


                    return;
                }

                if (user.Player.Map == 1004)
                {
                    user.SendSysMesage("Sorry, You Can`t Use This Skill In This Map.");
                    return;
                }

                if (user.Player.Map == 7 || user.Player.Map == 8 || user.Player.Map == 12 || user.Player.Map == 14 || user.Player.Map == 15 || user.Player.Map == 11 || user.Player.Map == 5 || user.Player.Map == 3071 || user.Player.Map == 20 || user.Player.Map == 10134 || user.Player.Map == 12099 || user.Player.Map == 10133 || user.Player.Map == 22348 || user.Player.Map == 12344 || user.Player.Map == 10070 || user.Player.Map == 10504 || user.Player.Map == 12344 || user.Player.Map == 6521 || user.Player.Map == 95152)
                {
                    user.SendSysMesage("Sorry, You Can`t Use This Skill In This Map.");
                    return;
                }

                if (user.Player.Map == 12667 || user.Player.Map == 3030 || user.Player.Map == 12275 || user.Player.Map == 12602 || user.Player.Map == 10741 || user.Player.Map == 2071 || user.Player.Map == 3030 || user.Player.Map == 3071 || user.Player.Map == 10133)
                {
                    user.SendSysMesage("Sorry, You Can`t Use This Skill In This Map.");
                    return;
                }

                if (user.Player.Map == 12546 || user.Player.Map == 13249 || user.Player.Map == 12602 || user.Player.Map == 12275 || user.Player.Map == 12602 || user.Player.Map == 13140 || user.Player.Map == 12546 || user.Player.Map == 12800 || user.Player.Map == 11331 || user.Player.Map == 12799 || user.Player.Map == 11996 || user.Player.Map == 12665 || user.Player.Map == 12601 || user.Player.Map == 12816 || user.Player.Map == 12099 || user.Player.Map == 66666 || user.Player.Map == 33333 || user.Player.Map == 22222 || user.Player.Map == 88888)
                {
                    user.SendSysMesage("Sorry, You Can`t Use This Skill In This Map.");
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
                else
                { 
                    user.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                }
                   
                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

            }
        }
    }
}
