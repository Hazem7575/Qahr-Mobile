// ============================================================================
//  [SpringFestival2026 2026-10-14] The official Global Spring Festival 2026 "Steed Legend" entry
//  (client msg 10850 tMsg_Info["Chunjie26"]). Shown while the festival SpringFestival2026 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150401"] (img lua_2026qqcj_6), clickfunc SpringFestivalX2025_OpenView</N>0
//              on msg 10850. Pushed by festival.json icon_show / icon_hide.
//    window    g_function Fui_Fgui_26ChunJie_Qrcs_Open(tData) takes a TABLE -> "StrToTable|Fui_Fgui_26ChunJie_Qrcs_Open({...})".
//              fgui_26chunjie_qrcsmediator UpdateMainUI(Data):
//                [1] any table  -> shows STR_LUAUI_SPRINGFESTIVALDRAW_TIME
//                [2] {item,n}   -> currency widget com_huobi_1 AND the Steed's Gift button state (n = controller index)
//                [3] Steed Points            [4] {server Banishing Progress, personal progress}
//                [5] red dots {map1..4, points}   [6] Celestial Steed revived 0/1   [7] 6 box reward item ids
//              map buttons 1..4 -> SpringFestivalX2025_ViewClick</N>i (Cn_Res STR_LUAUI_26CHUNJIE_MAP_i:
//              Cherry Inkstone Pool, Petal Vale, Decay Beast Lair, Gale Fortress); btn_smkz -> SpringFestival2026Part4_Get;
//              currency "+" -> SpringFestival2026Part4_Item.
//    highlight ActInfo[10850] ("Steed Legend", com_gg page) -> 10511 LUAViewSummary_ClientEnterButton</N>10850.
//  Real data sent: Steed Points 0, Banishing Progress 0/0 and no red dots, because nothing on this server awards them.
//  [2] is NOT sent: the client data does not say which item it is (server data), and its count also drives a controller index.
//  [7] is NOT sent: the Banishing Reward items are server data. The Decay Beast Lair click checks the real
//  Purge Pearl (3355569) count and answers with the official "no Purge Pearl" text.
//
//  NOT on this server (answered with a system message, nothing invented): Petal Vale / Gale Fortress dailies
//  (Bloom/Ebony Steed instances), Decay Beast Lair, Steed Feeding, Steed's Gift, ranking and Steed Store.
//  Config (optional): Data\SpringFestival2026\springfestival2026.json {festival, minLevel, purgePearl}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10850 + 10511 enter button), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.SpringFestival2026
{
    public static class SpringFestival2026Ui
    {
        public const uint LuaMsg = 10850;
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10850";

        public sealed class Config
        {
            public string Festival = "SpringFestival2026";
            public int MinLevel = 80;            // official: "Requirement: Level 80" (tSpringFestivalX2025[35242][Text114])
            public uint PurgePearl = 3355569;    // itemtype "Purge Pearl" (Decay Beast Lair ticket)
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "SpringFestival2026", "springfestival2026.json"); } }
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
                        var pp = FL.Val(d, "purgePearl"); if (pp != null) c.PurgePearl = FL.U(pp);
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[SpringFestival2026] springfestival2026.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tSpringFestivalX2025 / tSpringFestival2026Nest, Cn_Res STR_LUAUI_26CHUNJIE_MAP_n)
        const string TxtOver = "The Spring Festival event has concluded. Thank you for your participation. We`ll meet again next year!";
        const string TxtNoPearl = "You do not possess a Purge Pearl and cannot enter the Decay Beast Lair.";
        const string TxtLevel = "You need to reach Level 80 to join Steed Legend.";
        const string TxtNotOpen = "This part of Steed Legend isn`t open on this server yet.";
        static readonly string[] MapNames = { "Cherry Inkstone Pool", "Petal Vale", "Decay Beast Lair", "Gale Fortress" };

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

        /// <summary>LuaPush.HandleCallback (msg 10850, and 10511 for the Highlight Events "enter" button). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == HighlightMsg)
            {
                if (func != "LUAViewSummary_ClientEnterButton" || args == null || args.Length == 0 || args[0] != HighlightAct) return false;
                if (Throttled(c.Player.UID)) return true;
                Open(c, HighlightMsg);
                return true;
            }
            if (msgId != LuaMsg) return false;
            if (!func.StartsWith("SpringFestival", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (func == "SpringFestivalX2025_OpenView") { Open(c, LuaMsg); return true; }
            if (!Running) { Msg(c, TxtOver); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            if (func == "SpringFestivalX2025_ViewClick")
            {
                int i;
                if (args != null && args.Length > 0 && int.TryParse(args[0], out i) && i >= 1 && i <= MapNames.Length)
                {
                    if (i == 3 && Count(c, cfg.PurgePearl) <= 0) { Msg(c, TxtNoPearl); return true; }
                    Msg(c, MapNames[i - 1] + " isn`t open on this server yet.");
                    return true;
                }
            }
            Msg(c, TxtNotOpen);// Steed's Gift, currency "+", lair / feeding sub-windows
            return true;
        }

        /// <summary>The main window data (see the header for the field meaning).</summary>
        public static string OpenData()
        {
            return "StrToTable|Fui_Fgui_26ChunJie_Qrcs_Open({['Code']=200,['Data']={[1]={1},[3]=0,[4]={0,0},[5]={0,0,0,0,0},[6]=0}})";
        }
        static void Open(Client.GameClient c, uint replyMsg)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return; }
            LuaUi.LuaPush.Send(c, OpenData(), replyMsg);
        }
    }
}
