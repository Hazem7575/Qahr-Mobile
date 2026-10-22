// ============================================================================
//  [Christmas2022 2026-10-14] Global Christmas 2022 "Xmas Miracle Factory" (Dec. 20 - Jan. 4, luaUiRes_EN["1323"].acttime),
//  client tMsg_Info["DlgChristmas"] = 10153. Shown while the festival Christmas2022 runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui\dialog\dlgchristmas, luagui\1323.dat, luaUiRes_EN["1323"]):
//    button   DlgChristmasBtn (Dialog_Info "1323-btn", label "Xmas Miracle"), shown by the server with the client's own
//             DlgChristmas_OpenMainBtn|{Code=200,Data={Rewardflag=0|1}}; click -> 10153 ChrismasFactory_OpenMainButton.
//    main     DlgChristmas_OpenMainView|{Code=200,Data={CurFinish,BearState={3 x 0|1|2},DrawRed,RankRed,
//             ItemInfo='3600441 0 n',MakeState=cost,BearGrade,OutputItemList={}}}   (Dialog_Info "1323-1")
//  Real data: Xmas Wishing Star (3600441) bag count. Start state = the official opening (luaUiRes MainOpenTips "A sudden
//  accident happened on the production line"): the 3 production lines are broken (BearState 0), no gift produced,
//  bear level 0, first production free (MakeState 0, the guide's free first gift).
//  NOT on this server (system message, nothing taken or granted): repairing the lines (ChrismasFactory_DealFault, its
//  answers and rewards are server-side only), producing gifts, Bear Wish training, Miracle Delivery, ranking,
//  cross-server Snowman Kingdom, CP Mall shop guide.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10153), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Christmas2022
{
    public static class Christmas2022Ui
    {
        public const uint LuaMsg = 10153;
        public const string Festival = "Christmas2022";
        public const uint WishingStar = 3600441;// Xmas Wishing Star (itemtype), Dialog item_star
        public const string Ended = "The Global Christmas event has ended. See you next time!";

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        static bool Mine(string func)
        {
            return func.StartsWith("ChrismasFactory_", StringComparison.Ordinal) || func.StartsWith("Christmas2022BearTrain_", StringComparison.Ordinal)
                || func.StartsWith("ChristmasGiftsSend_", StringComparison.Ordinal) || func.StartsWith("ChristmasNPC2022_", StringComparison.Ordinal)
                || func.StartsWith("SnowmanKingdom_", StringComparison.Ordinal);
        }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10153).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Mine(func)) return false;
            if (Throttled(c.Player.UID, 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            switch (func)
            {
                case "ChrismasFactory_OpenMainButton":
                    Lua(c, "DlgChristmas_OpenMainView|" + MainData(c));
                    return true;
                case "ChrismasFactory_GetProductionGif":
                    return true;// end of the production animation: nothing was produced here
                case "ChrismasFactory_DealFault":
                    Msg(c, "The production line repairs aren`t open on this server yet.");
                    return true;
                case "ChrismasFactory_Production":
                    Msg(c, "The bear is in trouble. Go to help it!");// luaUiRes_EN["1323"].MsgBringOut1 (lines are still broken)
                    return true;
                case "ChrismasFactory_DailyMsg":
                    Msg(c, "Xmas Wishing Stars can`t be obtained from daily quests on this server yet.");
                    return true;
                case "SnowmanKingdom_EnterCross":
                    Msg(c, "The cross-server Christmas event isn`t open on this server yet.");
                    return true;
                case "ChristmasNPC2022_FindShop":
                    Msg(c, "Xmas Wishing Stars aren`t sold in the CP Mall on this server yet.");
                    return true;
                case "ChristmasNPC2022_OpenRankInterface":
                    Msg(c, "The Christmas ranking isn`t open on this server yet.");
                    return true;
            }
            if (func.StartsWith("Christmas2022BearTrain_", StringComparison.Ordinal)) { Msg(c, "Bear Wish isn`t open on this server yet."); return true; }
            if (func.StartsWith("ChristmasGiftsSend_", StringComparison.Ordinal)) { Msg(c, "Miracle Delivery isn`t open on this server yet."); return true; }
            Msg(c, "This part of the Xmas Miracle Factory isn`t open on this server yet.");
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The DlgChristmasMain data with this player's real data (official opening state).</summary>
        public static string MainData(Client.GameClient c)
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['CurFinish']=0,['BearState']={0,0,0},['DrawRed']=0,['RankRed']=0,");
            sb.Append("['ItemInfo']='").Append(WishingStar).Append(" 0 ").Append(Count(c, WishingStar)).Append("',");
            sb.Append("['MakeState']=0,['BearGrade']=0,['OutputItemList']={}");
            return sb.Append("}}").ToString();
        }
    }
}
