using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;
namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgFlagsWar
    {
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();

        public MsgFlagsWar()
        {
            Process = ProcesType.Dead;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                StartTimer = DateTime.Now;
                MsgSchedules.SendInvitation("Flags War", "", Places.TwinCity, 0, 60); // QahrMobile: PC spot 346,300 is water on the mobile Twin City; Twin City square
                MsgSchedules.SendSysMesage("Flags War has started now!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                Process = ProcesType.Alive;
            }
        }
        public void Finish()
        {

            if (Process == ProcesType.Alive)
            {
                MsgSchedules.SendSysMesage("Flags War has Finished.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

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
