// ============================================================================
//  [QixiInterplane2024 2026-10-14] The official 2024 Global Qixi "Interplane" entry (ERP589085, 8.6-8.26,
//  client msg 10448 tMsg_Info["Fui24Qixi"]). Shown while the festival QixiInterplane2024 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fairygui\window\config\fguibtnconfig FGUI_ActivityMenu["150111"] (img luahdshouna_btn4), clickfunc
//              InterplanePlay_OpenMain on msg 10448. Pushed by festival.json icon_show / icon_hide.
//    window    Fui24QixiTravel (fairygui\window\fui24qixitravel): Fgui_CommandKey|{Code=200,Data={CommandKey=
//              'Cmd_Fui24QixiTravel_Open',Star={3 x {State,Text}},Effect=1..3,Hand}} (proxy InitView).
//              Star State: 1 "Roaming %d/%d", 2 "Exploring %d%", 3 "Not open yet." (grey), 4/5 (no EN text).
//              On enter the window asks InterplanePlay_Point -> Cmd_Fui24QixiTravel_UpdatePoint {Point,Red}
//              (Cn_Res STR_FUI_24QIXI_MAIN_POINT "Points: %d").
//    clicks    InterplanePlay_Rules</N>5 -> Fui24QixiTravel_Rule|{Code=200,Data={Id=n}} (luaUiRes[10448].Rule[1..6])
//              InterplanePlay_IntoStar</N>1..3 (Romance / Soulmate / Eternity Starfield), Qixi2024Part5_ForgingOpen
//              (Excalibur forging, QixiForging), Qixi2024Part5_OpenScoreGet</N>n (Starry Points / rankings),
//              QixiMainNpc2024_GuideStepFinish</N>n (guide progress, acknowledged).
//  Texts: luacfg Server_Key tQixiMainNpc2024[33197] (Sword Deity) Text122 (event over), tQixi2024Part5[Msg][Nolevel].
//
//  NOT on this server (answered with a system message, nothing invented): Starfield roaming (duo/solo tours, romantic
//  events, compass treasure hunts), Excalibur forging, Starry Points and the rankings. The server scripts
//  (tInterplanePlay / tQixi2024Part5 / tQixiMainNpc2024) and their reward tables are not in the client, the Sword
//  Deity NPC 33197 and the Vault Palace map are not on this server. So every starfield shows "Not open yet." and the
//  points are 0. Config (optional, derived values): Data\QixiInterplane2024\qixiinterplane2024.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10448), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.QixiInterplane2024
{
    public static class QixiInterplane2024Ui
    {
        public const uint LuaMsg = 10448;

        public sealed class Config
        {
            public string Festival = "QixiInterplane2024";
            public int MinLevel = 80;   // official: "Requirement: Level 80+" (tQixiMainNpc2024[33197][Text115])
            public int SwordEffect = 1; // derived: Excalibur light of the first domain (tSwordEf[1]); nobody forges here
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "QixiInterplane2024", "qixiinterplane2024.json"); } }
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
                        var ef = FL.Val(d, "swordEffect"); if (ef != null) { uint e = FL.U(ef); if (e >= 1 && e <= 3) c.SwordEffect = (int)e; }
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[QixiInterplane2024] qixiinterplane2024.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key)
        const string TxtOver = "The fantastical and romantic stellar journey is about to come to an end. See you again next year.";// tQixiMainNpc2024[33197][Text122]
        const string TxtLevel = "Please come back after reaching level 80.";// tQixi2024Part5[Msg][Nolevel]
        static readonly string[] StarNames = { "Romance Starfield", "Soulmate Starfield", "Eternity Starfield" };// Cn_Res STR_FUI_24QIXI_SUB_STARNAME1..3
        const string TxtForging = "Excalibur forging isn`t open on this server yet.";
        const string TxtPoints = "Starry Points and the Qixi rankings aren`t open on this server yet.";

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

        /// <summary>LuaPush.HandleCallback (msg 10448). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool ours = func.StartsWith("InterplanePlay_", StringComparison.Ordinal)
                || func.StartsWith("Qixi2024Part5_", StringComparison.Ordinal)
                || func.StartsWith("QixiMainNpc2024_", StringComparison.Ordinal)
                || func.StartsWith("StarfieldExploration_", StringComparison.Ordinal);
            if (!ours) return false;
            if (func == "QixiMainNpc2024_GuideStepFinish") return true;// guide step flag: nothing to store
            if (Throttled(c.Player.UID)) return true;
            var cfg = Cfg;
            switch (func)
            {
                case "InterplanePlay_OpenMain":
                    if (!Running) { Msg(c, TxtOver); return true; }
                    if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
                    Lua(c, OpenData(cfg));
                    return true;
                case "InterplanePlay_Point":// asked by the window itself on enter
                    Lua(c, PointData());
                    return true;
                case "InterplanePlay_Rules":
                    {
                        int id;
                        if (args != null && args.Length > 0 && int.TryParse(args[0], out id) && id >= 1 && id <= 6)
                            Lua(c, "Fui24QixiTravel_Rule|{['Code']=200,['Data']={['Id']=" + id + "}}");
                        return true;
                    }
            }
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            if (func == "InterplanePlay_IntoStar")
            {
                int i;
                if (args != null && args.Length > 0 && int.TryParse(args[0], out i) && i >= 1 && i <= StarNames.Length)
                    Msg(c, StarNames[i - 1] + " isn`t open on this server yet.");
                return true;
            }
            if (func.StartsWith("Qixi2024Part5_Forging", StringComparison.Ordinal)) { Msg(c, TxtForging); return true; }
            if (func == "Qixi2024Part5_OpenScoreGet") { Msg(c, TxtPoints); return true; }
            Msg(c, "This part of the Qixi Festival isn`t open on this server yet.");
            return true;
        }

        /// <summary>The main window: the three starfields are "Not open yet." (State 3) because roaming isn't on this server.</summary>
        public static string OpenData(Config cfg)
        {
            var sb = new StringBuilder("Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_Fui24QixiTravel_Open',['Star']={");
            for (int i = 0; i < 3; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['State']=3,['Text']={0,0}}");
            }
            return sb.Append("},['Effect']=").Append(cfg.SwordEffect).Append("}}").ToString();
        }
        /// <summary>Starry Points: 0, nothing on this server awards them.</summary>
        public static string PointData()
        {
            return "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_Fui24QixiTravel_UpdatePoint',['Point']=0,['Red']=0}}";
        }
    }
}
