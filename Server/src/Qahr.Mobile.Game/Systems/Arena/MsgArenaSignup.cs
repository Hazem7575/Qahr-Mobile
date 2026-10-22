using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaSignupCreate(this ServerSockets.Packet stream, MsgArenaSignup.DialogType DialogID
            , MsgArenaSignup.DialogButton OptionID, Client.GameClient user)
        {
            stream.InitWriter();
            stream.Write((uint)DialogID);
            stream.Write((uint)OptionID);
            stream.Write(user.Player.UID);
            stream.Write(Database.GroupServerList.MyServerInfo.ID);//server id
            stream.Write(user.Player.Name, 32);
            stream.Write(user.ArenaStatistic.Info.TodayRank);
            stream.Write((uint)user.Player.Class);
            stream.Write((uint)0);
            stream.Write(user.ArenaStatistic.Info.ArenaPoints);
            stream.Write((uint)user.Player.Level);
            stream.Finalize(GamePackets.MsgQualifyingInteractive);
            return stream;
        }
        public static unsafe void GetArenaSignup(this ServerSockets.Packet stream, out MsgArenaSignup.DialogType DialogID, out MsgArenaSignup.DialogButton OptionID)
        {
            DialogID = (MsgArenaSignup.DialogType)stream.ReadUInt32();
            OptionID = (MsgArenaSignup.DialogButton)stream.ReadUInt32();
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public unsafe struct MsgArenaSignup
    {
        public enum DialogType : uint
        {
            ArenaIconOn = 0,
            ArenaIconOff = 1,
            ArenaGui = 3,
            StartCountDown = 2,
            OpponentGaveUp = 4,

            Match = 5,
            YouAreKicked = 6,
            StartTheFight = 7,
            Dialog = 8,
            Dialog2 = 9,

            Continue = 10

        }
        public enum DialogButton : uint
        {
            Lose = 3,
            Win = 1,
            DoGiveUp = 2,
            Accept = 1,
            MatchOff = 3,
            MatchOn = 5,
            SignUp = 0
        }
        [PacketAttribute(GamePackets.MsgQualifyingInteractive)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {

            DialogType DialogID;
            DialogButton OptionID;

            stream.GetArenaSignup(out DialogID, out OptionID);

            switch (DialogID)
            {
                case DialogType.OpponentGaveUp:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.SignUp:
                                {
                                    Game.MsgTournaments.MsgSchedules.Arena.DoQuit(stream, user);
                                    break;
                                }
                        }
                        break;
                    }
                case DialogType.ArenaIconOn:
                    {
                        if (Program.IDFixBug.Contains(user.Player.Map))
                            return;
                        if (user.Player.Map == 1700)
                            break;
                        Game.MsgTournaments.MsgSchedules.Arena.DoSignup(stream, user);
                        // echo the waiting UI only if DoSignup actually queued the player
                        if (user.ArenaStatistic.Info.Status == MsgArenaInfo.Action.WaitingForOpponent)
                            user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));

                        break;
                    }
                case DialogType.ArenaIconOff:
                    {
                        user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));

                        Game.MsgTournaments.MsgSchedules.Arena.DoQuit(stream, user);
                        break;
                    }

                case DialogType.ArenaGui:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.DoGiveUp: Game.MsgTournaments.MsgSchedules.Arena.DoGiveUp(stream, user); break;
                            case DialogButton.Accept:
                                {
                                    if (user.ArenaStatistic.ArenaState == MsgTournaments.MsgArena.User.StateType.WaitForBox)
                                    {
                                        user.ArenaStatistic.AcceptBox = true;
                                        user.ArenaStatistic.ArenaState = MsgTournaments.MsgArena.User.StateType.WaitForOther;

                                        //var info = MsgTournaments.MsgSchedules.Arena.MatchesRegistered.Values.ToArray();
                                        //for (int x = 0; x < info.Length; x++)
                                        //{
                                        //    if (info[x].Players[x].Fake)
                                        //    {
                                        //        if (info[x].Players[x].ArenaStatistic.ArenaState == MsgTournaments.MsgArena.User.StateType.WaitForBox)
                                        //        {
                                        //            info[x].Players[x].ArenaStatistic.AcceptBox = true;
                                        //            info[x].Players[x].ArenaStatistic.ArenaState = MsgTournaments.MsgArena.User.StateType.WaitForOther;
                                        //            break;
                                        //        }

                                        //    }
                                        //}
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DialogType.Continue:
                    {
                        if (Program.IDFixBug.Contains(user.Player.Map))
                            return;
                        switch (OptionID)
                        {
                            case DialogButton.SignUp:
                                {
                                    Game.MsgTournaments.MsgSchedules.Arena.DoSignup(stream, user);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
