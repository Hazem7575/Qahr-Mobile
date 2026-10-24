// ============================================================================
//  [Qixi2025 2026-10-14] The official Global Qixi 2025 "Love Cycles" entry (client msg 10726 tMsg_Info["Qixi25"]).
//  Shown while the festival Qixi2025 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150313"] (img lua_2025qqqrj_pic9 "Love Cycles"), clickfunc
//              QixiMainNpc2025_OpenView on msg 10726. Pushed by festival.json icon_show / icon_hide.
//    window    g_function Fui_Fgui_25Qixi_Xjyy_Open(sParam) (Public_SendNotification -> Data) = Fgui_25Qixi_Xjyy (150313).
//              fgui_25qixi_xjyymediator UpdateMainUI(Data) = {Worldly Points (bar max 120000), {red score, red 1..3},
//              current beast's Mortal World 0..7 (Cn_Res STR_FGUI_25QIXI_XJYY_NAME_CSLL_n, 0 = "Worldly Test")}.
//              Entries (proxy DJ): 1 Qixi2025BloodMission_JudgeUserStory (Worldly Test), 2 QiXiDraw2025_OpenView</N>0
//              (Rebuild Bridge), 3 QiXiPart3_FeedView (Fate Beast); score button QiXiDraw2025_OtherWay</N>1.
//              Duration text (STR_FGUI_25QIXI_SSYC_TIME): Aug. 26 - 08:59 (US)/16:59 (EU), Sept. 15.
//  Real data sent: Worldly Points 0, no red dots, Mortal World 0 - nothing on this server awards points or beasts.
//  Rebuild Bridge checks the real Starway Sand (3353900) count and answers with the official "Insufficient Starway
//  Sands" text when there is none.
//
//  NOT on this server (answered with a system message, nothing invented): Worldly Dream instances, Magpie Bridge
//  draw, beast nurturing / reincarnation / handbook, stage rewards, ranking and Worldly Store.
//  Config (optional): Data\Qixi2025\qixi2025.json {festival, minLevel, starwaySand}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10726), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Qixi2025
{
    public static class Qixi2025Ui
    {
        public const uint LuaMsg = 10726;

        public sealed class Config
        {
            public string Festival = "Qixi2025";
            public int MinLevel = 80;            // official: "Requirement: Level 80" (tQixiMainNpc2025[34490][Text114])
            public uint StarwaySand = 3353900;   // itemtype "Starway Sand" (repairs the Magpie Bridge)
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "Qixi2025", "qixi2025.json"); } }
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
                        var ss = FL.Val(d, "starwaySand"); if (ss != null) c.StarwaySand = FL.U(ss);
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Qixi2025] qixi2025.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tQixiMainNpc2025[34490], tQiXiDraw2025, Cn_Res STR_FGUI_25QIXI_XJYY_NAME_n)
        const string TxtOver = "As stars complete their turn, Qixi festivities conclude. Thank you for joining us! May happiness follow you.";
        const string TxtNoSand = "Insufficient Starway Sands. Nurture beasts to obtain this item.";
        const string TxtLevel = "You need to reach Level 80 to join Love Cycles.";
        const string TxtNotOpen = "This part of Love Cycles isn`t open on this server yet.";

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static int Count(Client.GameClient c, uint id) { try { return id == 0 ? 0 : Math.Max(0, c.Inventory.GetCountItem(id)); } catch { return 0; } }

        /// <summary>LuaPush.HandleCallback (msg 10726). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || msgId != LuaMsg) return false;
            if (!func.StartsWith("Qixi", StringComparison.Ordinal) && !func.StartsWith("QiXi", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (func == "QixiMainNpc2025_OpenView") { Open(c); return true; }
            if (!Running) { Msg(c, TxtOver); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "Qixi2025BloodMission_JudgeUserStory": Msg(c, "Worldly Test isn`t open on this server yet."); return true;
                case "QiXiDraw2025_OpenView":
                case "QiXiDraw2025_Draw":
                    Msg(c, Count(c, cfg.StarwaySand) <= 0 ? TxtNoSand : "Rebuild Bridge isn`t open on this server yet.");
                    return true;
                case "QiXiPart3_FeedView": Msg(c, "Fate Beast isn`t open on this server yet."); return true;
            }
            Msg(c, TxtNotOpen);// stage rewards, guide steps, ranking, store
            return true;
        }

        /// <summary>The main window data: {Worldly Points, {red score, red 1..3}, Mortal World 0..7}.</summary>
        public static string OpenData()
        {
            return "Fui_Fgui_25Qixi_Xjyy_Open|{['Code']=200,['Data']={0,{0,0,0,0},0}}";
        }
        static void Open(Client.GameClient c)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return; }
            LuaUi.LuaPush.Send(c, OpenData(), LuaMsg);
        }
    }
}
