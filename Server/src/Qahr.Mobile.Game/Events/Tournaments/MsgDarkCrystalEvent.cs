using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;
namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgDarkCrystalEvent
    {
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();

        public MsgDarkCrystalEvent()
        {
            Process = ProcesType.Dead;
        }
        //public void Start()
        //{
        //    if (Process == ProcesType.Dead)
        //    {
        //        StartTimer = DateTime.Now;

        //        MsgSchedules.SendInvitation("Dark Crystal", " ", 348, 391, 1002, 0, 60);
        //        MsgSchedules.SendSysMesage("Dark Crystal has now begun!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

        //        Process = ProcesType.Alive;
        //    }
        //}
        //public void Finish()
        //{

        //    if (Process == ProcesType.Alive)
        //    {
        //        //Game.MsgTournaments.MsgSchedules.SendSysMesage(
        //        //           "Dark Crystal Event Has Now Ended!",
        //        //           Game.MsgServer.MsgMessage.ChatMode.SlideCrosTheServer,
        //        //           Game.MsgServer.MsgMessage.MsgColor.white);


        //        MsgSchedules.SendSysMesage("Dark Crystal Ended!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

        //        Process = ProcesType.Dead;
        //    }
        //}
        //public void CheckUp()
        //{
        //    if (Process == ProcesType.Alive)
        //    {
        //        DateTime Now = DateTime.Now;
        //        if (Now > StartTimer.AddMinutes(5))
        //            Finish();

        //    }
        //}
    }
}
