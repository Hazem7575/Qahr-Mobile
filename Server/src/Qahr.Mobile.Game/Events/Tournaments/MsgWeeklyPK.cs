using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Game.MsgTournaments
{
    class MsgWeeklyPK
    {
        public const int EndSignTime = 6;
        private DateTime EndSignTimer = new DateTime();
        public void Open()
        {

            if (Mode == ProcesType.Dead)
            {
                Mode = ProcesType.Idle;
                EndSignTimer = DateTime.Now.AddMinutes(EndSignTime);
            }
        }
        public bool Started()
        {
            return Mode == ProcesType.Idle;
        }
        public bool AllowJoin()
        {
            return Mode == ProcesType.Idle && EndSignTimer > DateTime.Now;
        }
        private ProcesType Mode;
        public bool IsFinished() { return Mode == ProcesType.Dead; }
    }
}