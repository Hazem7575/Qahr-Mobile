// ============================================================================
//  [JellyfishSkyCity 2026-10-14] The official Global White Day 2026 "Tidal Serenade" entry (Mood Jellyfish, Stellan,
//  Spirit Ark; client msg 10871 tMsg_Info["26WhiteDay"]). Shown while the festival JellyfishSkyCity runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150410"] (EN label "Tidal Serenade"), clickfunc
//              WhiteDay2026Main_ClientOpen on msg 10871. Pushed by festival.json icon_show / icon_hide.
//    highlight ActInfo[10871] (com_gg page); its enter button sends LUAViewSummary_ClientEnterButton</N>10871 on 10511.
//    window    the chapter overview Fgui_26Bsqrj_Cxlg = interfacecfg 1087189 -> Fui_Fgui_26Bsqrj_Cxlg_Open, sent through
//              InfiniteReceiverData|{Code=200,Step=1,InterfaceID=1087189,Data={...}} (the voyage map Fgui_26Bsqrj_Hhmx
//              1087199 goes back to it with WhiteDay2026Main_ClientOpen). fgui_26bsqrj_cxlgmediator UpdateMainUI:
//                [1] duration text  [2] points  [3] {red: Heart Promise, points, ranking}  [4] {sybid, n} Stellar Fuel
//                (384018000 -> item 3355898)  [5] 6 chapters {state, progress %, red}  [6] current chapter (tip text
//                STR_LUAUI_26BSQRJ_CXLG_TIP_n)  [7] chapter to play the unlock effect on (0 = none).
//    clicks    (msg 10871) WhiteDay2026Main_ClientInterfaceClick</N>1..6 (chapter -> voyage map), _ClientGuide</N>1|2
//              (Stellar Fuel), _ClientIslandClick, WhiteValentine2026_OpenChou</N>0 (Heart Promise, Tear Pearls 3600844);
//              (msg 10768) StageSybTemplate_OpenView</N>150411001</N>0 (points rewards / ranking).
//  Real data sent: Stellar Fuel bag count, 0 points, chapter I open with 0% (the official starting state), no red dots.
//  Duration text: Cn_Res STR_LUAUI_26BSQRJ_RANK_TIME (the only English duration string of this event).
//
//  NOT on this server (system message, nothing invented): the Star Sea voyage and its chapter quests (maps
//  1005622-1005637 are unresolved), Heart Promise draw, points rewards and rankings.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10871 + 10511 page 10871 + 10768 StageSybTemplate 150411001), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.JellyfishSkyCity
{
    public static class JellyfishSkyCityUi
    {
        public const uint LuaMsg = 10871;              // tMsg_Info["26WhiteDay"]
        public const uint HighlightMsg = 10511;        // Highlight Events enter button
        public const uint StageMsg = 10768;            // tMsg_Info["25Halloween"] (shared StageSybTemplate)
        public const uint InterfaceMain = 1087189;     // Fui_Fgui_26Bsqrj_Cxlg_Open
        public const string Festival = "JellyfishSkyCity";
        public const int MinLevel = 80;                // tWhiteValentine2026 [35360][Text515] "Level: Lv. 80+"
        public const uint StellarFuel = 3355898, TearPearl = 3600844;
        public const uint SybStellarFuel = 384018000;
        public const string StageRewardsId = "150411001";

        // official texts (luacfg Server_Key tWhiteValentine2026 / tWhiteDay2026Main, Cn_Res)
        const string TxtOver = "Tidal Serenade has ended. We look forward to our next adventure with you!";
        const string TxtLevel = "You need to reach Level 80 to join Tidal Serenade.";// derived from "Level: Lv. 80+"
        const string TxtNoFuel = "Insufficient Stellar Fuel. Click Confirm to see how to obtain more, or click Cancel to go use Tear Pearls.";
        const string TxtTime = "Duration: [color=#6bf14d]Mar. 10 - 08:59 (US)/16:59 (EU), Mar. 30[/color]";
        static readonly string[] Chapters = { "", "Chapter I: Lone Isle Meet", "Chapter II: Raging Whirlpool", "Chapter III: Shellfolk Realm", "Chapter IV: Eternal Vigil", "Chapter V: Ship Sanctum", "Chapter VI: Stellar Redemption" };

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static int Count(Client.GameClient c, uint id) { try { return Math.Max(0, c.Inventory.GetCountItem(id)); } catch { return 0; } }

        /// <summary>LuaPush.HandleCallback (msg 10871, 10511 page 10871, 10768 StageSybTemplate 150411001). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == HighlightMsg)
            {
                if (func != "LUAViewSummary_ClientEnterButton" || args.Length == 0 || args[0].Trim() != LuaMsg.ToString()) return false;
                func = "WhiteDay2026Main_ClientOpen";
            }
            else if (msgId == StageMsg)
            {
                if (func != "StageSybTemplate_OpenView" || args.Length == 0 || args[0].Trim() != StageRewardsId) return false;
            }
            else if (msgId != LuaMsg) return false;
            else if (!func.StartsWith("WhiteDay2026", StringComparison.Ordinal) && !func.StartsWith("WhiteValentine2026", StringComparison.Ordinal)) return false;

            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "WhiteDay2026Main_ClientOpen":
                    LuaUi.LuaPush.Send(c, OpenData(Count(c, StellarFuel)), LuaMsg);
                    return true;
                case "WhiteDay2026Main_ClientInterfaceClick":
                    {
                        int n = 0;
                        if (args.Length > 0) int.TryParse(args[0], out n);
                        string name = n >= 1 && n <= 6 ? Chapters[n] : "The Star Sea voyage";
                        Msg(c, name + " isn`t open on this server yet.");
                        return true;
                    }
                case "WhiteDay2026Main_ClientIslandClick":
                    Msg(c, Count(c, StellarFuel) < 100 ? TxtNoFuel : "The Star Sea voyage isn`t open on this server yet.");
                    return true;
                case "WhiteDay2026Main_ClientGuide":
                    Msg(c, "Stellar Fuel isn`t available on this server yet.");
                    return true;
                case "WhiteValentine2026_OpenChou":
                    Msg(c, Count(c, TearPearl) <= 0 ? "You don`t have any Tear Pearls. Complete Star Sea adventures to obtain them." : "Heart Promise isn`t open on this server yet.");
                    return true;
                case "StageSybTemplate_OpenView":
                    Msg(c, "The Tidal Serenade points rewards and rankings aren`t open on this server yet.");
                    return true;
            }
            Msg(c, "This part of Tidal Serenade isn`t open on this server yet.");
            return true;
        }

        /// <summary>Chapter overview (InterfaceID 1087189). Real data: Stellar Fuel bag count, 0 points, chapter I open at 0%.</summary>
        public static string OpenData(int fuel)
        {
            return "InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=" + InterfaceMain + ",['Data']={"
                + LuaUi.LuaPush.Q(TxtTime) + ",0,{0,0,0},{" + SybStellarFuel + "," + Math.Max(0, fuel) + "},"
                + "{{1,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0}},1,0}}";
        }
    }
}
