//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Qahr.Mobile.Game.MsgTournaments
//{
//    public class MsgPkWar
//    {
//        public const int FinishMinutes = 15;

//        private ProcesType Mode;
//        private DateTime FinishTimer = new DateTime();

//        public uint WinnerUID = 0;
//        public MsgPkWar()
//        {
//            Mode = ProcesType.Dead;
//        }


//        public void Open()
//        {
//            if (Mode == ProcesType.Dead)
//            {
//                Mode = ProcesType.Idle;

//#if Arabic
//                   MsgSchedules.SendInvitation("PkWar", "CPs,PowerExpBall", 324, 194, 1002, 0, 60);
//#else
//                MsgSchedules.SendInvitation("PkWar", "CPs,RandomRareRuneBag", 423, 280, 1002, 0, 60);
//#endif
//#if Arabic
//                     MsgSchedules.SendSysMesage("PkWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
//#else
//                MsgSchedules.SendSysMesage("PkWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
//#endif
             
//                FinishTimer= DateTime.Now.AddMinutes(FinishMinutes);
//            }
//        }
//        public bool AllowJoin()
//        {
//            return Mode == ProcesType.Idle;
//        }

//        public void CheckUp()
//        {
//            if (Mode == ProcesType.Idle)
//            {
//                if (DateTime.Now > FinishTimer)
//                {
//                    Mode = ProcesType.Dead;
//                }
//            }
//        }
//        public bool IsFinished() { return Mode == ProcesType.Dead; }
//        public bool TheLastPlayer()
//        {
//            return Pool.GamePoll.Values.Where(p => p.Player.Map == 1508 && p.Player.Alive).Count() == 1;
//        }
//        public void GiveReward(Client.GameClient client, ServerSockets.Packet stream)
//        {
//            WinnerUID = client.Player.UID;

//            client.SendSysMesage("You received " + NewSystem.PrizeAllTops.PkWar.ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
//            MsgSchedules.SendSysMesage("" + client.Player.Name + " Won  PK War , he received " + NewSystem.PrizeAllTops.PkWar.ToString() + " ConquerPoints!", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);

//            client.Player.ConquerPoints += (uint)NewSystem.PrizeAllTops.PkWar;
     
//            AddTop(client);
//            client.Teleport(410, 354, 1002, 0);
//        }
//        public void AddTop(Client.GameClient client)
//        {
//            if(WinnerUID == client.Player.UID)
//                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.WeeklyPKChampion, Role.StatusFlagsBigVector32.PermanentFlag, false);
//        }
//    }
//}
