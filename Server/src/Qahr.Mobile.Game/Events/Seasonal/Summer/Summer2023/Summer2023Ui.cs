// ============================================================================
//  [Summer2023 2026-10-14] Global Summer 2023 "Dream Cruise" (ERP539531, 2023-07-04 01:00 - 2023-07-17 08:59),
//  client tMsg_Info["Summer23"] = 10250. Shown while the festival Summer2023 runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui\dialog\dlgsummeractivities, luagui\1386.dat, luaUiRes_EN[1386], Fgui window 150438):
//    button   Dlg23SummerBtn (Dialog_Info "1386-btn", anisection 2023qqsq_btn7); shown with
//             StrToTable|LuaUIMgr_ShowDialog("Dlg23SummerBtn"); click Dlg23SummerBtn_Enter ->
//             10250 SummerActive2023Page_OpenMainInterface.
//    main     Dlg23SummerMain_BindData|{Code=200,Data={Point,IslanIndex,Fog{6},Boat{lev,fx,pos},ItemInfo{2},Event,Boss,EventFinish}}
//             -> the client forwards it to Fui_YangFanYuanHang_Open (Fgui 150438).
//    islet    SummerActive2023Page_SailInformation</N>i</N>j -> Dlg23SummerMain_UpdateBlock|{Code=200,Data={...}}
//    access   SummerActive2023Page_ClientGuide</N>1|2 -> Dlg23SummerAccess_BindData|{Code=200,Data={ItemIndex=n}}
//             (client-side "How to get" page of Dialog_Info["1386-accessconfig"]).
//  Real data: Heart of Ocean(B) 3342540 / Dream Conch 3342541 bag counts. Dream Points, ark level, islet progress:
//  nothing on this server earns them (the islet instances tSummer2023Instance have no known map), so every player
//  is at the official start state. Every action that needs the missing parts answers with a polite system message;
//  nothing is granted, nothing is taken.
//
//  [2026-10-15] Cruise, daily sign-in and the Anniversary Wish draw are played for real: Summer2023Cruise.cs.
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10250), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Summer2023
{
    public static class Summer2023Ui
    {
        public const uint LuaMsg = 10250;// tMsg_Info["Summer23"]
        public const string Festival = "Summer2023";
        public const int MinLevel = 80;// ActBtnConfig[5] NeedLevel
        public const uint HeartOfOcean = 3342540, DreamConch = 3342541;// Dialog_Info["1386-accessconfig"]
        public const string Ended = "The Global Summer event has ended. See you next summer!";
        const string NotOpen = "The Dream Cruise isn`t open on this server yet.";

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

        static bool Known(string f)
        {
            return f.StartsWith("SummerActive2023Page_", StringComparison.Ordinal) || f.StartsWith("SummerActive2023Npc_", StringComparison.Ordinal)
                || f.StartsWith("SummerActiveInstance_", StringComparison.Ordinal) || f.StartsWith("Summer2023Instance_", StringComparison.Ordinal)
                || f.StartsWith("SummerActiveCrossFu_", StringComparison.Ordinal);
        }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10250).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Known(func)) return false;
            if (Throttled(c.Player.UID, 300)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, "Your level is less than " + MinLevel + ". Please level up first!"); return true; }
            if (Summer2023Cruise.OnFunc(c, func, args)) return true;// [Summer2023 2026-10-15] cruise / sign-in / Anniversary Wish (Summer2023Cruise.cs)
            switch (func)
            {
                case "SummerActive2023Page_OpenMainInterface":
                    Lua(c, "Dlg23SummerMain_BindData|" + MainData(c));
                    return true;
                case "SummerActive2023Page_SailInformation":
                    {
                        int i, j;
                        if (args.Length < 2 || !int.TryParse(args[0], out i) || !int.TryParse(args[1], out j) || !ValidIslet(i, j)) return true;
                        Lua(c, "Dlg23SummerMain_UpdateBlock|" + BlockData(i, j));
                        if (i > 1) Msg(c, "Upgrade ships to unlock more areas.");// luaUiRes_EN[1386].Locked
                        return true;
                    }
                case "SummerActive2023Page_ClientGuide":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > 2) return true;
                        Lua(c, "Dlg23SummerAccess_BindData|{['Code']=200,['Data']={['ItemIndex']=" + i + "}}");
                        return true;
                    }
                case "SummerActive2023Page_ClickAreaFog":
                    Msg(c, "Upgrade ships to unlock more areas.");// luaUiRes_EN[1386].Locked
                    return true;
                case "SummerActive2023Page_ClickCruise":
                case "SummerActive2023Page_ToggleCruiseType":
                case "SummerActiveInstance_ClickSweep":
                case "SummerActiveInstance_EnterIsland":
                case "SummerActiveInstance_Sweep":
                    Msg(c, NotOpen);
                    return true;
                case "SummerActive2023Page_OpenDrawInterface":
                    Msg(c, "Ultramarine Wishes aren`t open on this server yet.");
                    return true;
                case "SummerActive2023Page_OpenStageInterface":
                    Msg(c, "Dream Point rewards aren`t open on this server yet.");
                    return true;
                case "SummerActive2023Npc_OpenExchangeStore":
                case "SummerActive2023Npc_OpenEmoneyStore":
                    Msg(c, "The Summer store isn`t open on this server yet.");
                    return true;
                case "SummerActive2023Page_OpenWelfareInterface":
                    Msg(c, "Daily sign-in packs aren`t available on this server yet.");
                    return true;
                case "Summer2023Instance_IntoBoss":
                    Msg(c, "Marine Monarch hasn`t awakened on this server yet.");
                    return true;
                case "SummerActiveCrossFu_EnterOS":
                case "SummerActiveCrossFu_EnterKeLaoOS":
                    Msg(c, "The cross-server Summer event isn`t open on this server yet.");
                    return true;
            }
            return true;// guide / dialogue notifications (GuideInterface, TreasureHunt, PhotoReward, ShowMainButton): nothing to do
        }

        static bool ValidIslet(int i, int j)
        {
            if (i == 0 && j == 0) return true;
            if (i == 1) return j >= 1 && j <= 6;
            return i >= 2 && i <= 6 && j >= 1 && j <= 5;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The Dream Cruise main data with this player's real data (official start state otherwise).</summary>
        public static string MainData(Client.GameClient c)
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['Point']=0,");// Dream Points: no source on this server
            sb.Append("['IslanIndex']='00',");// Departure Spot
            sb.Append("['Fog']={3,1,1,1,1,1},");// Moon Area open, the other 5 areas still fogged
            sb.Append("['Boat']={0,0,'00'},");// ark level 1 (BoatAni[1] 2023qqsq_ship0), no upgrade effect, at the Departure Spot
            sb.Append("['ItemInfo']={'").Append(HeartOfOcean).Append(" 0 ").Append(Count(c, HeartOfOcean)).Append("','")
              .Append(DreamConch).Append(" 0 ").Append(Count(c, DreamConch)).Append("'},");
            sb.Append("['Event']={},['Boss']={0,0},['EventFinish']='00'");
            return sb.Append("}}").ToString();
        }

        /// <summary>Islet panel: official islet name (client side), Cruise disabled (State 2), no cost / reward preview (server-only data).</summary>
        public static string BlockData(int i, int j)
        {
            return "{['Code']=200,['Data']={['SailType']=1,['State']=2,['IsFinished']=0,['Type']=1,['PlayEffect']=0,['IslanIndex']='"
                + i + j + "',['MoveBoat']=0}}";
        }
    }
}
