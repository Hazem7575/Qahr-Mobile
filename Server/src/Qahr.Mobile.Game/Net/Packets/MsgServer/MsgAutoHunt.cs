using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgAutoHunt
    {
        [Flags]
        public enum Mode : byte
        {
            Start = 1,
            EndAuto = 2,
            EXPGained = 3,

        }
        public static unsafe ServerSockets.Packet AutoHuntCreate(this ServerSockets.Packet stream, ushort type, ulong Icon, ulong Exp = 0, string KillerName = null)
        {
            stream.InitWriter();

            stream.Write(type);
            stream.Write(Icon);
            stream.Write(Exp);
            stream.Write(KillerName);
            stream.Finalize(GamePackets.MsgHangUp);
            return stream;
        }
        public static unsafe void GetAutoHunt(this ServerSockets.Packet stream, out ushort Act)
        {
            Act = stream.ReadUInt16();
        }
        [PacketAttribute(GamePackets.MsgHangUp)]
        private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Map == 10133 || user.Player.Map == 3030 || user.Player.Map == 3071 || user.Player.Map == 10741 || user.Player.Map == 10134 || user.Player.Map == 10250 || user.Player.Map == 11447 || user.Player.Map == 1700 || user.Player.Map == 10134 || user.Player.Map == 10133 || user.Player.Map == 700 || user.Inventory.Contain(750000, 1))
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }

            if (Pool.Constants.SkillsNotAvailableHere.Contains(user.Player.Map))
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }

            if (user.Player.Map == 5061 || user.Player.Map == 5062 || user.Player.Map == 5063 || user.Player.Map == 5064 || user.Player.Map == 5065 || user.Player.Map == 5066 || user.Player.Map == 5051 || user.Player.Map == 5052 || user.Player.Map == 5053 || user.Player.Map == 5054 || user.Player.Map == 5055 || user.Player.Map == 5056 || user.Player.Map == 5057 || user.Player.Map == 5058)
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }

            if (user.Player.Map == 11 || user.Player.Map == 7 || user.Player.Map == 5 || user.Player.Map == 15 || user.Player.Map == 14 || user.Player.Map == 12 || user.Player.Map == 8 || user.Player.Map == 20 || user.Player.Map == 1730)
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }

            if (user.Player.Map == 10134 || user.Player.Map == 3030 || user.Player.Map == 3071 || user.Player.Map == 10741 || user.Player.Map == 12099 || user.Player.Map == 10133 || user.Player.Map == 13140 || user.Player.Map == 12344 || user.Player.Map == 12816 || user.Player.Map == 10070 || user.Player.Map == 10504 || user.Player.Map == 12344 || user.Player.Map == 6521 || user.Player.Map == 95152)
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }


            if (user.Player.Map == 12667 || user.Player.Map == 12546 || user.Player.Map == 12275 || user.Player.Map == 12602 || user.Player.Map == 12546 || user.Player.Map == 12605 || user.Player.Map == 12602 || user.Player.Map == 3030 || user.Player.Map == 10741 || user.Player.Map == 10741 || user.Player.Map == 2071 || user.Player.Map == 3071 || user.Player.Map == 3030 || user.Player.Map == 10133 || user.Player.Map == 66666 || user.Player.Map == 33333 || user.Player.Map == 22222 || user.Player.Map == 88888)
            {
                user.SendSysMesage("You cannot use Auto Hunt in here.");
                return;
            }

            ushort Action;
            stream.GetAutoHunt(out Action);
            switch ((Mode)Action)
            {
                case Mode.Start:
                    {
                        if (user.Player.OnAutoHunt == false)
                        {
                            if (user.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                                user.Player.RemoveFlag(user.Player.OnXPSkill());
                            user.Send(stream.AutoHuntCreate(0, 341));
                            user.Send(stream.AutoHuntCreate(1, 341));
                            user.Player.OnAutoHunt = true;
                        }

                        break;
                    }
                case Mode.EndAuto:
                    {

                        if (user.Player.AutoHuntExp > 0)
                        {

                            user.IncreaseAutoExperience(stream, user.Player.AutoHuntExp);
                        }
                        user.Send(stream.AutoHuntCreate(3, 0));
                        user.Player.OnAutoHunt = false;
                        user.Player.AutoHuntExp = 0;
                        break;
                    }
                case Mode.EXPGained:
                    {
                        if (user.Player.AutoHuntExp > 0)
                        {
                            user.IncreaseAutoExperience(stream, user.Player.AutoHuntExp);
                        }
                        user.Send(stream.AutoHuntCreate(3, 0, user.Player.AutoHuntExp));
                        user.Player.AutoHuntExp = 0;
                        break;
                    }
                default:
                    {
                        MyConsole.WriteLine("[AutoHunt] Unknown Action: " + Action + "");
                        break;
                    }
            }


        }
    }
}