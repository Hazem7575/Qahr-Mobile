// ============================================================================
//  [Halloween2024 2026-10-14] The official 2024 "Global Halloween" entry (Temple of Hades / Duke Macabre),
//  client msg 10482 tMsg_Info["Fui24Halloween"] (ERP597236 全球万圣活动 10.22-11.4). Shown while festival Halloween2024 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fairygui\window\config\fguibtnconfig FGUI_ActivityMenu["150175"] (img lua_2024qqws_btn1Normal),
//              clickfunc HalloweenNpc2024_OpenMain on msg 10482. Pushed by festival.json icon_show / icon_hide.
//    window    fairygui\window\halloweenmain (HalloweenMain):
//                Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_HalloweenMain_Open'}}
//                Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_HalloweenMain_UpdateData',Points=n,PRed=0|1,Task={a,b},Reds={...}}}
//              (mediator UpdateData: Points -> com_point progress + "STR_FUI_PETDAILY_POINT"; >= 300000 unlocks the Grimloft
//              secret room look; Task[1]==1 and Task[2]==1 -> c_mmo 1; Reds = img_red_n to show).
//    buttons   (halloweenmaindata callFunc, all on msg 10482)
//              1 btn_home   HallowMonsterCultivation_OpeanMain</N>1</N>0</N>1   Witch House
//              2 btn_shop   HalloweenNpc2024_OpenExShop                          Grimloft
//              3 btn_mmo    SilentHill2024_ClickViewEnter                        Silent Hill
//              4 btn_kuafu  Halloween2024Kua_EnterKuaFu                          Devil Valley (CS)
//              5 com_point  HalloweenNpc2024_StageRankView                       Thrills Ranking
//              btn_fight    SkillCalculate_OpenChat                              Midnight Duel
//              btn_rule     HalloweenNpc2024_OpenRule</S>10482_1 -> DlgPublic_OpenRule|{Code=200,Data={RuleId='10482_1'}}
//                           (official text luaUiRes RuleText["10482_1"]).
//  Texts: luacfg Server_Key tHalloweenNpc2024_DelText[33387] / [33388] (official English).
//
//  NOT on this server (system message, nothing granted): Witch House beast cultivation, Grimloft exchange, Silent Hill,
//  Midnight Duel, Devil Valley (cross-server), Thrills Ranking, CP / Anima shops. Thrill Points are 0: nothing awards them.
//  Quests 5924-5933 (tHalloweenNightmare) are not part of this festival (no evidence they belong to 2024).
//  Config (optional, derived values): Infinity[DB]\Halloween2024\halloween2024.json {festival, minLevel}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10482), tagged [Halloween2024 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Halloween2024
{
    public static class Halloween2024Ui
    {
        public const uint LuaMsg = 10482;
        public const string RuleId = "10482_1";

        public sealed class Config
        {
            public string Festival = "Halloween2024";
            public int MinLevel = 80;   // official "Requirement: Level 80" (tHalloweenNpc2024_DelText[33387][Text114])
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "Halloween2024", "halloween2024.json"); } }
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
                    try { MyConsole.WriteLine("[Halloween2024] halloween2024.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tHalloweenNpc2024_DelText)
        const string TxtOver = "The Temple of Hades has returned to peace, thanks to your efforts. We shall meet again!";   // [33387][Text212]
        const string TxtLevel = "Please upgrade to level 80 first!";                                                        // [33388][Text211]
        // derived (polite, nothing invented)
        const string TxtNotOpen = " isn`t open on this server yet.";
        const string TxtKua = "Devil Valley (CS) is a cross-server event and isn`t available on this server.";

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
            return func.StartsWith("HalloweenNpc2024_", StringComparison.Ordinal)
                || func.StartsWith("Halloween2024Kua_", StringComparison.Ordinal)
                || func.StartsWith("SilentHill2024_", StringComparison.Ordinal)
                || func.StartsWith("HallowMonsterCultivation_", StringComparison.Ordinal)
                || func.StartsWith("SkillCalculate_", StringComparison.Ordinal);
        }

        /// <summary>LuaPush.HandleCallback (msg 10482). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Ours(func)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (func == "HalloweenNpc2024_GuideSet") return true;// tutorial step flag: nothing to store
            if (!Running) { Msg(c, TxtOver); return true; }
            if (func == "HalloweenNpc2024_OpenRule")
            {
                // only the official rule page of this window
                Lua(c, "DlgPublic_OpenRule|{['Code']=200,['Data']={['RuleId']='" + RuleId + "'}}");
                return true;
            }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "HalloweenNpc2024_OpenMain": Open(c); return true;
                case "HallowMonsterCultivation_OpeanMain": Msg(c, "The Witch House" + TxtNotOpen); return true;
                case "HalloweenNpc2024_OpenExShop": Msg(c, "Grimloft" + TxtNotOpen); return true;
                case "HalloweenNpc2024_OpenEmoneyShop": Msg(c, "The Halloween store" + TxtNotOpen); return true;
                case "SilentHill2024_ClickViewEnter": Msg(c, "Silent Hill" + TxtNotOpen); return true;
                case "SkillCalculate_OpenChat": Msg(c, "The Midnight Duel" + TxtNotOpen); return true;
                case "HalloweenNpc2024_StageRankView": Msg(c, "The Thrills Ranking" + TxtNotOpen); return true;
            }
            if (func.StartsWith("Halloween2024Kua_", StringComparison.Ordinal)) { Msg(c, TxtKua); return true; }
            Msg(c, "This part of Halloween" + TxtNotOpen);
            return true;
        }

        public static string OpenCall() { return "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_HalloweenMain_Open'}}"; }
        /// <summary>Real data only: Thrill Points 0 (nothing awards them on this server), no red dots, Silent Hill not cleared.</summary>
        public static string DataCall() { return "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_HalloweenMain_UpdateData',['Points']=0,['PRed']=0,['Task']={0,0},['Reds']={}}}"; }
        static void Open(Client.GameClient c)
        {
            Lua(c, OpenCall());
            Lua(c, DataCall());
        }
    }
}
