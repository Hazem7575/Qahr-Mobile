// ============================================================================
//  [Anniversary2026 2026-10-14] The official Global Anniversary 2026 "Anniversary Sail" entry
//  (client msg 10883 tMsg_Info["ChouJiang26ZNQ"]). Shown while the festival Anniversary2026 runs.
//  Official dates 4.21-5.11 (Server_Key header "2026全球周年庆活动[4.21-5.11]", Cn_Res STR_FUI_ACTTIME_10883 /
//  STR_FUI_26ZNQCENTER_TIME "Apr. 21 - May 11").
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150428"] (img lua_2026qqznq_34), clickfunc AnswerQuestion_OpenMain on 10883.
//    window    h_fuifunction Fui26ZnqCenter_Open(tData) takes a TABLE with tData.Data -> "StrToTable|Fui26ZnqCenter_Open({...})".
//              fui26znqcentermediator: Data.Points {cur,max,red}, Data.RedLuo (Luoxing Pagoda red dot), Data.YinDao (hand
//              guide), Data.OpenWin, Data.Map[1..7] = {state 0 closed/1 open/2 done, progress %, {3 condition states},
//              {3 reward-show ids}, claim state 0/1/2, {condition items "id 0 1"}} for the 7 ports
//              (Quanzhou, Fuxu, Srivijaya, Semarang, Colombo, Hormuz, Alexandria).
//              Buttons -> 10883 Anniversary2026_Lottery_OpenRankInterface</N>1, AnswerQuestion_QuYu</N>1..4
//              (Revival Academy quiz, Residential Area, Naval Drill, Luoxing Pagoda), AnswerQuestion_HangReward</N>map,
//              AnswerQuestion_PageClick</N>map</N>idx. Lottery window ChouJiang26ZNQ (Stargazing, Star Raft).
//    highlight ActInfo[10883] (com_gg page) -> 10511 LUAViewSummary_ClientEnterButton</N>10883.
//  Real data sent: the route rewards and condition items are the client's own (fui26znqcenterdata: reward-show ids
//  4300x000..2 exist in luauirewardshow, items 3356092-3356101 exist in itemtype and match the Cn_Res condition names).
//  Every route is at its starting state (port 1 open, nothing done), points 0, because nothing on this server
//  awards progress. Points max 99999999 is the client data file's value (derived, config).
//
//  Quiz: the client has NO question bank for this quiz (tAnswerQuestion has no questions in Server_Key/Cn_Res; the
//  questions are server data), so the Revival Academy answers with a system message, like every other part:
//  Naval Drill, Residential Area, Luoxing Pagoda Stargazing, route recruiting/claiming and rankings. Nothing invented.
//  Config (optional): Infinity[DB]\Anniversary2026\anniversary2026.json {festival, minLevel, pointsMax}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10883 + 10511 enter button), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Anniversary2026
{
    public static class Anniversary2026Ui
    {
        public const uint LuaMsg = 10883;
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10883";

        public sealed class Config
        {
            public string Festival = "Anniversary2026";
            public int MinLevel = 80;              // official: "Requirement: Level 80" (tAnswerQuestion_DelText[35479][Text515])
            public uint PointsMax = 99999999;      // DERIVED: client fui26znqcenterdata sample Points {0, 99999999}
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "Anniversary2026", "anniversary2026.json"); } }
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
                        var pm = FL.Val(d, "pointsMax"); if (pm != null && FL.U(pm) > 0) c.PointsMax = FL.U(pm);
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Anniversary2026] anniversary2026.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tAnswerQuestion_DelText, Cn_Res STR_FUI_26ZNQCENTER_*)
        const string TxtOver = "The CO: Anniversary Sail event has concluded. Look forward to our next collaboration!";
        const string TxtLevel = "You need to reach Level 80 to join Anniversary Sail.";
        static readonly string[] AreaNames = { "Revival Academy", "Residential Area", "Naval Drill", "Luoxing Pagoda" };// STR_FUI_26ZNQCENTER_LANDMAP_1..4
        static readonly string[] PortNames = { "Quanzhou Port", "Fuxu Port", "Srivijaya Port", "Semarang Port", "Colombo Port", "Hormuz Port", "Alexandria Port" };

        // client fui26znqcenterdata: per port the 3 reward-show ids and the condition items (slot -> item id)
        static readonly uint[][] RouteRewards =
        {
            new uint[] { 430001000, 430001001, 430001002 }, new uint[] { 430002000, 430002001, 430002002 },
            new uint[] { 430003000, 430003001, 430003002 }, new uint[] { 430004000, 430004001, 430004002 },
            new uint[] { 430005000, 430005001, 430005002 }, new uint[] { 430006000, 430006001, 430006002 },
            new uint[] { 430007000, 430007001, 430007002 },
        };
        static readonly uint[][] RouteItems =// {slot1, slot2, slot3}, 0 = a hero to recruit (no item)
        {
            new uint[] { 0, 3356092, 0 }, new uint[] { 3356097, 0, 3356093 }, new uint[] { 0, 3356098, 0 },
            new uint[] { 3356094, 0, 3356099 }, new uint[] { 0, 3356100, 0 }, new uint[] { 3356101, 0, 3356095 },
            new uint[] { 0, 3356096, 0 },
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
        static int Arg(string[] args, int i) { int v; return args != null && args.Length > i && int.TryParse(args[i], out v) ? v : 0; }

        /// <summary>LuaPush.HandleCallback (msg 10883, and 10511 for the Highlight Events "enter" button). True = answered.</summary>
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
            if (!func.StartsWith("AnswerQuestion_", StringComparison.Ordinal) && !func.StartsWith("Anniversary2026", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (func == "AnswerQuestion_OpenMain") { Open(c, LuaMsg); return true; }
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            int a = Arg(args, 0);
            if (func == "AnswerQuestion_QuYu" && a >= 1 && a <= AreaNames.Length)
            {
                if (a == 1) Msg(c, "The Revival Academy quiz isn`t open on this server yet.");
                else Msg(c, AreaNames[a - 1] + " isn`t open on this server yet.");
                return true;
            }
            if ((func == "AnswerQuestion_PageClick" || func == "AnswerQuestion_HangReward") && a >= 1 && a <= PortNames.Length)
            {
                Msg(c, "The route to " + PortNames[a - 1] + " can`t be expanded on this server yet.");
                return true;
            }
            Msg(c, "This part of Anniversary Sail isn`t open on this server yet.");// rankings, Stargazing lottery
            return true;
        }

        /// <summary>The main window data (see the header for the field meaning).</summary>
        public static string OpenData()
        {
            var sb = new StringBuilder();
            sb.Append("StrToTable|Fui26ZnqCenter_Open({['Code']=200,['Data']={['Points']={0,").Append(Cfg.PointsMax).Append(",0},['RedLuo']=0,['YinDao']=0,['OpenWin']=0,['Map']={");
            for (int i = 0; i < RouteRewards.Length; i++)
            {
                if (i > 0) sb.Append(',');
                var r = RouteRewards[i];
                sb.Append('{').Append(i == 0 ? 1 : 0).Append(",0,{0,0,0},{").Append(r[0]).Append(',').Append(r[1]).Append(',').Append(r[2]).Append("},0,{");
                bool first = true;
                for (int s = 0; s < 3; s++)
                {
                    if (RouteItems[i][s] == 0) continue;
                    if (!first) sb.Append(',');
                    sb.Append('[').Append(s + 1).Append("]='").Append(RouteItems[i][s]).Append(" 0 1'");
                    first = false;
                }
                sb.Append("}}");
            }
            sb.Append("}}})");
            return sb.ToString();
        }
        static void Open(Client.GameClient c, uint replyMsg)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return; }
            LuaUi.LuaPush.Send(c, OpenData(), replyMsg);
        }
    }
}
