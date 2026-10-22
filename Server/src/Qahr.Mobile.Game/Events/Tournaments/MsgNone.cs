using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
   public class MsgNone : ITournament
    {
       public ProcesType Process { get; set; }
       public TournamentType Type { get; set; }
        public MsgNone(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public void Open()
        {

        }
     public   bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            return false;
        }
     public void CheckUp()
        {

        }
     public bool InTournament(Client.GameClient user)
        {
            return false;
        }
    }
}
