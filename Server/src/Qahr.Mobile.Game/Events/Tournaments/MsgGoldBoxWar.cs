using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;
namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgGoldBoxWar
    {
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();

        public MsgGoldBoxWar()
        {
            Process = ProcesType.Dead;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                StartTimer = DateTime.Now;
                MsgSchedules.SendInvitation("Lucky Golden Boxes", " ", Places.TwinCity, 0, 60); // QahrMobile: PC spot 356,298 is water on the mobile Twin City; Twin City square
                MsgSchedules.SendSysMesage("Lucky Golden Boxes has now begun!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                Process = ProcesType.Alive;
            }
        }
        public void Finish()
        {

            if (Process == ProcesType.Alive)
            {
                MsgSchedules.SendSysMesage("Lucky Golden Boxes has Ended.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                Process = ProcesType.Dead;
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
