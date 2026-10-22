// ============================================================================
//  [SpringFestival2024 2026-10-14] The official 2024 Global Spring Festival (Year of the Dragon, ERP569531, client msg
//  10358 tMsg_Info["24SpringFestival"], luaui\dialog\dlgspringfestivals). Shown while the festival SpringFestival2024 runs.
//
//  Official client (luaui, decrypted):
//    button    DlgFestival2024BTN (luagui 1462 Dialog_Info["1462-1"], anisection 2024qqcj_btn11), shown with
//              Dlg_OpenUI|{Code=200,View='DlgFestival2024BTN',Data={RedPoint=0}} (festival.json icon_show / icon_hide).
//              Its click DlgFestival2024BTN_OpenUI / g_function DlgFestival2024BTN_NewOpenUI (ActBtnConfig[18], commented
//              out in this client, dates "2024-02-01 00:00 2024-02-21 07:59", NeedLevel 80) both send
//              LuckyPool2024_OpenLuckView on 10358.
//    window    DlgFestivalMain2024Task (Dialog_Info["1462-Task"]) via Dlg_OpenUI|{View=..,Head={Func={6 callbacks}},
//              Data={Call,Guide,Actime,Task={4 x "lock,cur/max,red,effect"},Pro,MoneyDown,MoneyDownRed}} (no Code: the
//              window reads Head and Data from g_player_info[View] = the whole table).
//              Tasks (luaUiRes["1462-main"], guide texts luaUiRes[1278][10358], seen live): 1 Exorcism, 2 Dragon Blessing
//              Painting (celebration tasks, "Once per Day"), 3 Dragon Trial = the Nine Sons challenge Dlg24SpringChallenge
//              (SpingYuan2023_*), 4 Year Beast (NianBeast2024_*). Pro = server Fortune Points (0..100000, reward boxes
//              1000..100000), MoneyDown = personal Auspicious Points (luaUiRes["1462-main"].Tip[2]).
//              A locked task (lock 0) is answered by the client itself (luaUiRes["1462-main"].Tip[4]).
//  Texts: luacfg Server_Key tSpringFestival2024Main[32466] (Zhang Sengyou) Text122 = event over.
//
//  NOT on this server (answered with a system message, nothing invented): the Dragon Blessing Painting daily quests,
//  Dragon Awakening / Dragon Wishes (DlgFestivalMain2024Wish: its 7 awakening + 10 wish rewards are server tables),
//  the Nine Sons Dragon Trial (its 9 prize items and odds are server-side), Exorcism, Year Beast, the Fortune Pool and
//  the ranking. There are no Questinfo quests and no maps.txt rows. So all 4 tasks are "Locked" and the points are 0.
//  Config (optional, derived values): Infinity[DB]\SpringFestival2024\springfestival2024.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10358), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.SpringFestival2024
{
    public static class SpringFestival2024Ui
    {
        public const uint LuaMsg = 10358;

        public sealed class Config
        {
            public string Festival = "SpringFestival2024";
            public int MinLevel = 80;      // official: ActBtnConfig[18] NeedLevel 80, "Requirement: Level 80" (tSpringFestival2024Main[32466][Text114])
            public string ActTime = "2 1~2 21";// official: "2024 2 1~2024 2 21" (month day; the year is the current one)
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "SpringFestival2024", "springfestival2024.json"); } }
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
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[SpringFestival2024] springfestival2024.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        const string TxtOver = "The celebration has come to an end. Wishing all heroes a healthy and prosperous New Year!";// tSpringFestival2024Main[32466][Text122]
        const string TxtLevel = "The Spring Festival celebration requires level 80.";// derived from "Requirement: Level 80"
        const string TxtDragon = "Dragon Awakening and Dragon Wishes aren`t open on this server yet.";
        const string TxtPool = "The Fortune Pool isn`t open on this server yet.";
        const string TxtTrial = "Dragon Trial isn`t open on this server yet.";
        const string TxtDaily = "The Dragon Blessing Painting tasks aren`t open on this server yet.";
        const string TxtBeast = "Exorcism and Year Beast aren`t open on this server yet.";

        // the window's 6 callbacks (Head.Func): tasks 1-4, the Fortune Pool box, the dragon. The names are the official
        // scripts' prefixes (client luaui Access lists); the server answers them all here.
        static readonly string[] Funcs =
        {
            "NianBeast2024_OpenSL",                  // 1 Exorcism (btn_task_1, lower left)
            "SpringFestivalDailyActivities_EnterMap",// 2 Dragon Blessing Painting, "Once per Day" (btn_task_2, upper left)
            "SpingYuan2023_OpenNineDragon",          // 3 Dragon Trial = Nine Sons (btn_task_3, upper right)
            "NianBeast2024_GotoBoss",                // 4 Year Beast (btn_task_4, lower right)
            "SpringFestival2024Main_GiftClick",      // 5 Fortune Pool box
            "SpringFestival2024Main_DragonClick",    // 6 the dragon (awaken / wishes)
        };

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        static bool Ours(string f)
        {
            return f.StartsWith("LuckyPool2024_", StringComparison.Ordinal)
                || f.StartsWith("SpringFestival2024Main_", StringComparison.Ordinal)
                || f.StartsWith("SpringFestivalDailyActivities_", StringComparison.Ordinal)
                || f.StartsWith("SpingYuan2023_", StringComparison.Ordinal)
                || f.StartsWith("NianBeast2024_", StringComparison.Ordinal);
        }

        /// <summary>LuaPush.HandleCallback (msg 10358). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Ours(func)) return false;
            if (Throttled(c.Player.UID)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "LuckyPool2024_OpenLuckView":// the button, and the Nine Sons window's close button
                    Lua(c, MainData(cfg));
                    return true;
                case "SpringFestival2024Main_DragonClick": Msg(c, TxtDragon); return true;
                case "SpringFestival2024Main_GiftClick": Msg(c, TxtPool); return true;
            }
            if (func.StartsWith("SpingYuan2023_", StringComparison.Ordinal)) { Msg(c, TxtTrial); return true; }
            if (func.StartsWith("SpringFestivalDailyActivities_", StringComparison.Ordinal)) { Msg(c, TxtDaily); return true; }
            if (func.StartsWith("NianBeast2024_", StringComparison.Ordinal)) { Msg(c, TxtBeast); return true; }
            Msg(c, "This part of the Spring Festival isn`t open on this server yet.");
            return true;
        }

        /// <summary>The main window (DlgFestivalMain2024Task): 4 locked tasks, 0 Fortune Points, 0 Auspicious Points,
        /// the dragon asleep (Call 0). Sent without Code on purpose (see the header).</summary>
        public static string MainData(Config cfg)
        {
            int y = DateTime.Now.Year;
            var t = (cfg.ActTime ?? "2 1~2 21").Split('~');
            string actime = y + " " + t[0].Trim() + "~" + y + " " + (t.Length > 1 ? t[1].Trim() : t[0].Trim());
            var sb = new StringBuilder("Dlg_OpenUI|{['View']='DlgFestivalMain2024Task',['Head']={['Func']={");
            for (int i = 0; i < Funcs.Length; i++) { if (i > 0) sb.Append(','); sb.Append(LuaUi.LuaPush.Q(Funcs[i])); }
            sb.Append("}},['Data']={['Call']=0,['Guide']=0,['Actime']=").Append(LuaUi.LuaPush.Q(actime)).Append(",['Task']={");
            for (int i = 0; i < 4; i++) { if (i > 0) sb.Append(','); sb.Append("'0,-1,-1,-1'"); }
            return sb.Append("},['Pro']=0,['MoneyDown']=0,['MoneyDownRed']=0}}").ToString();
        }
    }
}
