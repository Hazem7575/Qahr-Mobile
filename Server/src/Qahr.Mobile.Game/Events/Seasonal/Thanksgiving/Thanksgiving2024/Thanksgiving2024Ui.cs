// ============================================================================
//  [Thanksgiving2024 2026-10-14] The official 2024 "Global Thanksgiving" entry (Mythic Domain / Lady Coco),
//  client msg 10486 tMsg_Info["Fui24Ge"] (ERP601000 2024全球感恩活动 11.19-12.2). Shown while festival Thanksgiving2024 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fairygui\window\config\fguibtnconfig FGUI_ActivityMenu["150184"] (img lua_2024qqgeru1),
//              clickfunc ThanksGiving2024View_MainView on msg 10486. Pushed by festival.json icon_show / icon_hide.
//    window    fairygui\window\fui24ganenmain (Fui24GanenMain):
//                Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_Fui24GanenMain_Open',Unlock={lmsy,hjbns,qyfr},Stage=n,
//                                 Point={cur,max},Red={lmsy,hjbns,qyfr,point}}}
//              Unlock 0 = grey / 1 = open. Stage -1 "The feast has ended", 0 "New storyline is now available",
//              n "New storyline will be available in %d days" (Cn_Res STR_FUI_24GANEN_*). Point -> pgb_point + "Points: %d".
//    buttons   (fui24ganenmaindata iInterface, all on msg 10486)
//              com_lmsy   ThanksGiving2024View_MianButton</N>1        Finale Feast (story)
//              com_hjbns  ThanksGiving2024View_MianButton</N>2        Turkey Refuge
//              com_qyfr   ThanksgivingKua2024_OpenView                 Wish of Fertility (cross-server)
//              btn_point  ThanksGiving2024View_RewardView</N>1</N>1    points rewards / ranking
//              (rule)     ThanksGiving2024View_Rule</S>10486-1 -> DlgPublic_OpenRule|{Code=200,Data={RuleId='10486-1'}}
//              item-get   ThanksGivingNpc2024_OpenEmoneyShop / _OpenExchangeShop (g_function Fui24GanenDraw_ItemGet)
//  Texts: luacfg Server_Key tThanksGiving2024View_DelText[105] / tThanksGivingNpc2024_DelText[33500] (official English).
//
//  NOT on this server (system message, nothing granted): the Finale Feast story, Turkey Refuge, Wish of Fertility (CS),
//  the rankings, the shops. Fertility Points are 0: nothing awards them. Stage = -1 (the story is not available here).
//  Points bar max 120000 = top cumulative tier of pointsrankdata CONST_POINTS_AWARD_DATA[10486] (derived, config).
//  Config (optional): Infinity[DB]\Thanksgiving2024\thanksgiving2024.json {festival, minLevel, pointsMax}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10486), tagged [Thanksgiving2024 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Thanksgiving2024
{
    public static class Thanksgiving2024Ui
    {
        public const uint LuaMsg = 10486;
        public const string RuleId = "10486-1";

        public sealed class Config
        {
            public string Festival = "Thanksgiving2024";
            public int MinLevel = 80;           // official "Requirement: Level 80" (tThanksGivingNpc2024_DelText[33500][Text115])
            public uint PointsMax = 120000;     // derived: last tier of CONST_POINTS_AWARD_DATA[10486]
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "Thanksgiving2024", "thanksgiving2024.json"); } }
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
                        var pm = FL.Val(d, "pointsMax"); if (pm != null) c.PointsMax = Math.Max(1u, Math.Min(100000000u, FL.U(pm)));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Thanksgiving2024] thanksgiving2024.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key)
        const string TxtOver = "The Thanksgiving story in Mythic Domain has come to an end, and more exciting stories will continue at Christmas. Stay tuned!"; // tThanksGiving2024View_DelText[105][1]
        // derived (polite, nothing invented)
        const string TxtLevel = "Thanksgiving Feast requirement: Level 80. Please level up first!";
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
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        static bool Ours(string func)
        {
            return func.StartsWith("ThanksGiving2024View_", StringComparison.Ordinal)
                || func.StartsWith("ThanksGivingNpc2024_", StringComparison.Ordinal)
                || func.StartsWith("ThanksgivingKua2024_", StringComparison.Ordinal);
        }

        /// <summary>LuaPush.HandleCallback (msg 10486). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Ours(func)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (func == "ThanksGiving2024View_Rule")
            {
                Lua(c, "DlgPublic_OpenRule|{['Code']=200,['Data']={['RuleId']='" + RuleId + "'}}");
                return true;
            }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "ThanksGiving2024View_MainView": Open(c); return true;
                case "ThanksGiving2024View_MianButton":
                    {
                        int i = 0;
                        if (args != null && args.Length > 0) int.TryParse(args[0], out i);
                        if (i == 1) { Msg(c, "The Finale Feast story" + TxtNotOpen); return true; }
                        if (i == 2) { Msg(c, "Turkey Refuge" + TxtNotOpen); return true; }
                        break;
                    }
                case "ThanksgivingKua2024_OpenView": Msg(c, "Wish of Fertility is a cross-server event and isn`t available on this server."); return true;
                case "ThanksGiving2024View_RewardView": Msg(c, "The Thanksgiving rankings and rewards" + TxtNotOpen); return true;
                case "ThanksGivingNpc2024_OpenEmoneyShop":
                case "ThanksGivingNpc2024_OpenExchangeShop": Msg(c, "The Thanksgiving store" + TxtNotOpen); return true;
            }
            Msg(c, "This part of Thanksgiving" + TxtNotOpen);
            return true;
        }

        /// <summary>Real data only: all three entries clickable (each answers with a message), Stage -1, Fertility Points 0, no red dots.</summary>
        public static string OpenCall(Config cfg)
        {
            return "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_Fui24GanenMain_Open',['Unlock']={1,1,1},['Stage']=-1,['Point']={0,"
                + cfg.PointsMax + "},['Red']={0,0,0,0}}}";
        }
        static void Open(Client.GameClient c) { Lua(c, OpenCall(Cfg)); }
    }
}
