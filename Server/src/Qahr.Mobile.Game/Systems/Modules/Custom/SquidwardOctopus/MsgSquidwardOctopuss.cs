using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgSquidwardOctopuss
    {

        public int GoldRemain = 0;
        public int SilverRemain = 0;
        public int CopperRemain = 0;
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();

        public MsgSquidwardOctopuss()
        {
            Process = ProcesType.Dead;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                MsgSchedules.SendInvitation("Squidward Octopus Event", " ", 463, 371, 1002, 0, 60); // QahrMobile: PC spot 322,298 is water on the mobile Twin City; walkable cell next to SquidwardOctopus (9534) at 463,367
                MsgSchedules.SendSysMesage("Squidward Octopus Event has now begun!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);                
                StartTimer = DateTime.Now;
                GoldRemain = 10000;
                SilverRemain = 10000;
                CopperRemain = 10000;

                Process = ProcesType.Alive;
            }
        }
        public void Finish()
        {
            if (Process == ProcesType.Alive)
            {
                Process = ProcesType.Dead;

                foreach (var user in Pool.GamePoll.Values)
                {
                    try
                    {
                        if (user.Player.Map == 3071)
                        {
                            user.Teleport(410, 354, 1002);
                        }
                    }
                    catch
                    {

                    }
                }

                MsgSchedules.SendSysMesage("Squidward Octopus Event has Ended.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;
                if (Now > StartTimer.AddMinutes(5))
                    Finish();

            }
        }
    }
}
