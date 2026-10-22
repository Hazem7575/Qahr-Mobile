// [Unanswered B6 2026-10-14] Lua side of the 7-day goals window (client luaui\dialog\dlgsevenday, msg 10366 "DlgSevenday").
// The client calls, through MsgAction 258 "</F>ClientManager_Callback</N>10366</S>{func}...":
//   NewServerSevengoal_OpenMainScreen</N>10   (top button 1461 / sevendaytargetfunc)
//   NewServerSevengoal_UpdateGoal</N>{day}    NewServerSevengoal_Phase</N>{n}    NewServerSevengoal_Goto</N>{day}</N>{n}
//   NewServerSevengoal_RewardItem</N>{day}</N>{n}   <- reward claim
// The server tracks none of these goals, so: no window data is pushed (the window stays closed = inactive), the player
// gets the "not available" line, and every RewardItem claim is refused. Nothing is granted.
//
// NOT wired yet: Base\LuaUi\LuaPush.HandleCallback is outside this batch. B8 adds one line there:
//     if (msgId == Qahr.Mobile.Unanswered.B6.B6SevenGoalLua.LuaMsg && Qahr.Mobile.Unanswered.B6.B6SevenGoalLua.OnLua(client, func, args)) return true;
using System;

namespace Qahr.Mobile.Unanswered.B6
{
    public static class B6SevenGoalLua
    {
        public const uint LuaMsg = 10366;
        public const string Prefix = "NewServerSevengoal_";

        /// <summary>True = answered (the callback was a 7-day goals one).</summary>
        public static bool OnLua(Client.GameClient client, string func, string[] args)
        {
            if (!B6Util.Ready(client) || string.IsNullOrEmpty(func) || !func.StartsWith(Prefix, StringComparison.Ordinal)) return false;
            if (!B6Throttle.Allow(client, 4)) return true;// swallowed spam
            switch (func)
            {
                case "NewServerSevengoal_RewardItem":
                    B6Util.Tell(client, B6Goals.ClosedText);
                    try { MyConsole.WriteLine("[B6] refused 7-day goal reward uid=" + client.Player.UID); } catch { }
                    return true;
                case "NewServerSevengoal_OpenMainScreen":
                    B6Util.Tell(client, B6Goals.ClosedText);
                    return true;
                case "NewServerSevengoal_UpdateGoal":
                case "NewServerSevengoal_Phase":
                case "NewServerSevengoal_Goto":
                    return true;// window never opened by the server; nothing to update
            }
            return false;
        }
    }
}
