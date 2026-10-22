using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;
namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgCpsCastle
    {
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();

        public MsgCpsCastle()
        {
            Process = ProcesType.Dead;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                StartTimer = DateTime.Now;
                MsgSchedules.SendInvitation("Shadow Castle Event", " ", 410, 354, 1002, 0, 60); // QahrMobile: PC spot 368,298 is water on the mobile Twin City; Twin City square
                MsgSchedules.SendSysMesage("Shadow Castle Event has now begun!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                Process = ProcesType.Alive;
            }
        }
        public void Finish()
        {

            if (Process == ProcesType.Alive)
            {
                MsgSchedules.SendSysMesage("Shadow Castle Event has Ended.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

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