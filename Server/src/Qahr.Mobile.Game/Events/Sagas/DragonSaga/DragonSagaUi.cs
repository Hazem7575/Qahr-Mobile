// ============================================================================
//  [DragonSaga 2026-10-14] The official Mid-Autumn 2026 "Dragonfest" entry (client msg 10978 tMsg_Info["Qqss26"]).
//  Shown while the festival DragonSaga (Questinfo TypeId 8 quests 9942-9960, maps 1006058-1006065) runs.
//
//  Official client (luaui, decrypted):
//    top bar   fairygui\window\config\fguibtnconfig FGUI_ActivityMenu["150475"] (img lua_2026qqss_41, EN "Dragonfest"),
//              clickfunc ZhongQiu2026Npc_OpenMainView on msg 10978. Pushed by festival.json icon_show / icon_hide.
//    window    g_function Fui_Fgui_26Qqss_Main_Open(tData) with tData = {Code=200,Data={CityInfo={4 x {red,msgid,sfunc}},
//              JiFen={red,msgid,sfunc,cur,max}}}. It takes a TABLE (TableToStr), so it is sent as
//              "StrToTable|Fui_Fgui_26Qqss_Main_Open({...})" (a plain "Func|{...}" string makes TableToStr fail; seen live).
//              Cities (Cn_Res STR_FGUI_26QQSS_MAIN_BTN_1..4): 1 Diamond March, 2 Dragon Vault, 3 Dragon Arena,
//              4 Primeval Battlefield. Event time (STR_FGUI_26QQSS_TIME): Sept. 15 08:59 (US)/16:59 (EU) - Oct. 5.  (fgui_26qqss_mainmediator: a city click sends {msgid,sfunc} back and
//              closes the window; the points button sends JiFen {msgid,sfunc}). The sfuncs are chosen by the server.
//    others    ZhongQiu2026Dragon_ClientOpenView / ClientOpenMain / ClientOpenCollect / ClientCharge / ClientWays /
//              SkipFunc (Dragon Arena views Fgui_26Qqss_Ssyc / Sstj), ZhongQiu2026Npc_OpenStore</N>n (Dragon Vault).
//    highlight ActInfo[10978] (com_gg page, "enter" button) -> 10511 LUAViewSummary_ClientEnterButton</N>10978.
//  Texts: luacfg Server_Key tZhongQiu2026Npc[37341] (Suyi) - the level and "event over" messages below are official.
//
//  NOT on this server (answered with a system message, nothing invented): Dragon March (CS), Diamond March (Ancient
//  Arena), Dragon Arena / Bestiary, the Dragon's Recognition story instances (their NPC types 10738-10747 and monsters
//  24309-24314 exist only in the newer official client, without lookface; the prize items 3600877 / 3357159 exist in no
//  itemtype), Dragonfest Points, the ranking and the Dragon Vault store. Points are 0 because nothing awards them.
//  Config (derived values): Data\DragonSaga\dragonsaga.json (optional).
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10978 + 10511 enter button), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.DragonSaga
{
    public static class DragonSagaUi
    {
        public const uint LuaMsg = 10978;
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10978";

        public sealed class Config
        {
            public string Festival = "DragonSaga";
            public int MinLevel = 80;            // official: "Level: 80+" (tZhongQiu2026Npc[37341][Text116])
            public uint PointsMax = 50000;       // official: Dragon Vault "Unlocks at 50,000 pts" (tExShop[10978][UnLock][2])
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "DragonSaga", "dragonsaga.json"); } }
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
                        var pm = FL.Val(d, "pointsMax"); if (pm != null) c.PointsMax = Math.Min(100000000u, FL.U(pm));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[DragonSaga] dragonsaga.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tZhongQiu2026Npc[37341])
        const string TxtOver = "The Dragonfest has come to a close. Thank you all for participating. We shall meet again next year!";
        const string TxtLevel = "Dragonfest demands great strength to survive. Please return after reaching level 80!";
        static readonly string[] CityNames = { "Diamond March", "Dragon Vault", "Dragon Arena", "Primeval Battlefield" };// Cn_Res STR_FGUI_26QQSS_MAIN_BTN_1..4
        const string TxtNotOpen = "This part of the Dragonfest isn`t open on this server yet.";

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

        /// <summary>LuaPush.HandleCallback (msg 10978, and 10511 for the Highlight Events "enter" button). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == HighlightMsg)
            {
                // only our page; anything else stays with its owner
                if (func != "LUAViewSummary_ClientEnterButton" || args == null || args.Length == 0 || args[0] != HighlightAct) return false;
                if (Throttled(c.Player.UID)) return true;
                Open(c, HighlightMsg);
                return true;
            }
            if (msgId != LuaMsg) return false;
            bool ours = func.StartsWith("ZhongQiu2026", StringComparison.Ordinal);
            if (!ours) return false;
            if (Throttled(c.Player.UID)) return true;
            switch (func)
            {
                case "ZhongQiu2026Npc_OpenMainView":
                    Open(c, LuaMsg);
                    return true;
                case "ZhongQiu2026Dragon_SkipFunc":// animation skip flag: nothing to store
                    return true;
            }
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            if (func == "ZhongQiu2026Npc_City")
            {
                int i;
                if (args != null && args.Length > 0 && int.TryParse(args[0], out i) && i >= 1 && i <= CityNames.Length) { Msg(c, CityNames[i - 1] + " isn`t open on this server yet."); return true; }
            }
            Msg(c, TxtNotOpen);// points, Dragon Arena views, Dragon Vault store
            return true;
        }

        /// <summary>The main window data. The four cities and the points button call back here (msg 10978).</summary>
        public static string OpenData(Config cfg)
        {
            var sb = new StringBuilder("StrToTable|Fui_Fgui_26Qqss_Main_Open({['Code']=200,['Data']={['CityInfo']={");
            for (int i = 1; i <= 4; i++)
            {
                if (i > 1) sb.Append(',');
                sb.Append("{0,").Append(LuaMsg).Append(",'ZhongQiu2026Npc_City</N>").Append(i).Append("'}");
            }
            sb.Append("},['JiFen']={0,").Append(LuaMsg).Append(",'ZhongQiu2026Npc_OpenRank',0,").Append(cfg.PointsMax).Append("}}})");
            return sb.ToString();
        }
        static void Open(Client.GameClient c, uint replyMsg)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            Lua(c, OpenData(cfg), replyMsg);
        }
    }
}
