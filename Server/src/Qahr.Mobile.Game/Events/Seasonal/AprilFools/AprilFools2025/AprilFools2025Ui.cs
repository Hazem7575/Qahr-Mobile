// ============================================================================
//  [AprilFools2025 2026-10-14] The official 2025 Global April Fools "Fool`s Paradise" entry
//  (client msg 10568 tMsg_Info["25FoolsDay"], 【英文征服】全球愚人节活动 4.1-4.14), shown while the festival AprilFools2025 runs.
//  (AliceRescue is April Fools 2024, a different event: not touched.)
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150260"] (img lua_2025qqyrj_btnrukou), clickfunc
//              AprilFools2025MainPage_OpenView on msg 10568. Pushed by festival.json icon_show / icon_hide.
//    window    h_fuifunction Fui_FoolsDayMain_OpenData(sData) (string, StrToTable) -> fairygui\window\foolsdaymain:
//              Data[1] = {sysid, count}  currency (Fate Coin 3600774, Fui_FoolsDayLottery_PropAccess)
//              Data[2] = {DetectivePoints}            (Cn_Res STR_FUI_25FOOLSDAY_POINTSNUM "Points: %s")
//              Data[3] = {8 x 0/1}                    story pictures unlocked
//              Data[4] = {state, a, b}                state 1 "Reward (%s/%s)", 2 "Quest (%s/%s)", 3/4 clue collection
//              Data[5] = {red x4}                     {Wonder Box, points, cross-server, Fool`s Paradise}
//    clicks    btn_1 AprilFools2025MainPage_EnterGameplay, btn_2 AprilFools2025MainPage_EnterCross,
//              btn_3 / currency "+" -> DrawClient_OpenView|GetWay</N>10568 on msg 10510 (tMsg_Info["Christmas24"]),
//              points -> AprilFoolsDay2025_OpenStage. DlgItemGet (ViewId 10568): AprilFools2025MainPage_EnterGameplay,
//              AprilFoolsKua2025_EnterKuaLand, AprilFoolsDay2025Other_ExChangeShop, AprilFoolsDay2025Other_OpenShop.
//  Texts: luacfg Server_Key tAprilFools2025MainPage_DelText[34118] / [Sys] (official English), Cn_Res STR_FUI_25FOOLSDAY_*.
//
//  REAL data only: the Fate Coin count from the bag. Detective Points 0 (nothing awards them), no story pictures.
//  Data[4] = {2,0,8} "Quest (0/8)" is DERIVED (8 = the window's 8 pictures; the official chapter count is server-side).
//  NOT on this server (system message, nothing granted): Fool`s Paradise story (tPlotEditor), Pleasure Vault (CS,
//  tAprilFoolsKua2025), Wonder Box wishes (reward pool is server-side only), ranking, Joyful Gift Shop.
//  Config (optional, derived values): Data\AprilFools2025\aprilfools2025.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10568 + 10510 DrawClient_* with act 10568 only), tagged.
//  It sits BEFORE the VillageRequests line, which otherwise takes every DrawClient_* on 10510.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.AprilFools2025
{
    public static class AprilFools2025Ui
    {
        public const uint LuaMsg = 10568;
        public const uint DrawMsg = 10510;           // Fgui_CommonDraw_Request sends on tMsg_Info["Christmas24"]
        public const string DrawAct = "10568";
        public const uint FateCoin = 3600774;        // official currency (h_fuifunction PropAccess, itemtype "Fate Coin")

        public sealed class Config
        {
            public string Festival = "AprilFools2025";
            public int MinLevel = 80;                                               // official "Requirement: Level 80"
            public HashSet<uint> OpenFrom = new HashSet<uint> { 1002, 1036 };       // official [Sys][NoCity] "Twin City or Market"
            public int StoryParts = 8;                                              // derived: the window's 8 pictures
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "AprilFools2025", "aprilfools2025.json"); } }
        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(CfgFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists)
                    {
                        var d = FL.ReadJson(fi.FullName);
                        var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
                        var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
                        var from = FL.Arr(FL.Val(d, "openFromMaps")); if (from != null) c.OpenFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[AprilFools2025] aprilfools2025.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (Server_Key tAprilFools2025MainPage_DelText, Cn_Res STR_FUI_25FOOLSDAY_MAINBTN_*)
        const string TxtOver = "All members of the Grinning Group have been apprehended, and this farce is finally over. I look forward to our next collaboration!";
        const string TxtLevel = "Please upgrade to level 80 first.";
        const string TxtNoCity = "You can only open this interface in Twin City or Market!";
        const string TxtNotOpen = " isn`t open on this server yet.";

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call, uint msg) { LuaUi.LuaPush.Send(c, call, msg); }

        /// <summary>LuaPush.HandleCallback (msg 10568, and 10510 DrawClient_* for act 10568 only). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == DrawMsg)
            {
                // only the Wonder Box of this event; every other DrawClient_* stays with its owner
                if (!func.StartsWith("DrawClient_", StringComparison.Ordinal) || args == null || args.Length == 0 || args[0] != DrawAct) return false;
            }
            else if (msgId != LuaMsg || !func.StartsWith("AprilFools", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "AprilFools2025MainPage_OpenView":
                    if (c.Player.DynamicID != 0 || !Cfg.OpenFrom.Contains(c.Player.Map)) { Msg(c, TxtNoCity); return true; }
                    Lua(c, OpenCall(c, Cfg), LuaMsg);
                    return true;
                case "DrawClient_GetWay": Lua(c, "Fui_FoolsDayLottery_PropAccess", DrawMsg); return true;// official "get Fate Coins" list
                case "AprilFools2025MainPage_EnterGameplay": Msg(c, "Fool`s Paradise" + TxtNotOpen); return true;
                case "AprilFools2025MainPage_EnterCross":
                case "AprilFoolsKua2025_EnterKuaLand": Msg(c, "Pleasure Vault (Cross-server)" + TxtNotOpen); return true;
                case "AprilFoolsDay2025_OpenStage": Msg(c, "The Detective Ranking" + TxtNotOpen); return true;
                case "AprilFoolsDay2025Other_ExChangeShop":
                case "AprilFoolsDay2025Other_OpenShop": Msg(c, "The Joyful Gift Shop" + TxtNotOpen); return true;
            }
            if (msgId == DrawMsg) { Msg(c, "The Wonder Box" + TxtNotOpen); return true; }
            Msg(c, "This part of Fool`s Paradise" + TxtNotOpen);
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The main window (Fui_FoolsDayMain_OpenData). Only real data: the bag's Fate Coins.</summary>
        public static string OpenCall(Client.GameClient c, Config cfg)
        {
            var sb = new StringBuilder("Fui_FoolsDayMain_OpenData|{['Code']=200,['Data']={");
            sb.Append('{').Append(FateCoin).Append(',').Append(c == null ? 0 : Count(c, FateCoin)).Append("},");
            sb.Append("{0},");                                                           // Detective Points
            sb.Append("{0,0,0,0,0,0,0,0},");                                             // no story picture unlocked
            sb.Append("{2,0,").Append(cfg.StoryParts).Append("},");                      // "Quest (0/8)" (derived)
            sb.Append("{0,0,0,0}");                                                      // no red points
            sb.Append("}}");
            return sb.ToString();
        }
    }
}
