// ============================================================================
//  [WorldCup2026 2026-10-14] The official Global World Cup 2026 "Pitch Glory" entry (client msg 10900 tMsg_Info["Foot26"]).
//  Two festivals share this window:
//    WorldCup2026          = Pitch Glory (Cloudsurge), second half, official 6.30-7.20 (Server_Key header
//                            "2026全球世界杯决赛世界杯后半程活动[6.30-7.20]", Cn_Res STR_FUI_ACTTIME_10900 / STR_LUAUI_26FOOT_TIME1).
//                            Top-bar Fgui button 150448, clickfunc WorldCup2026TaskSecond_OpenMainView.
//    WorldCup2026Windrise  = Pitch Glory (Windrise), first half ("2026全球世界杯开幕前半程"). Top-bar Fgui button 150444,
//                            clickfunc WorldCupMainPage_OpenMain. The client has no exact dates for it (manual festival).
//
//  Official client (luaui, decrypted):
//    window  g_function Fui_26foot_main_Open(tData) takes a TABLE -> "StrToTable|Fui_26foot_main_Open({...})".
//            fgui_26foot_mainmediator UpdateData(Data): Stage (0 first half, >0 second half; NOTE the proxy turns a
//            missing Stage into 1, so it is always sent), Time "yyyy-MM-dd HH:mm yyyy-MM-dd HH:mm", JiFen {cur,max},
//            DuoQi {cur,max,left}, IsFinish, IsFinish2, RaceDay (0 match today, n = opens in n days, -1 ended),
//            Guide {{item,n},..} (top-right currencies), BtnRed {8 red dots}.
//            Buttons call back on 10900: WorldCupMainPage_Go / WorldCupLottery2026_* / WorldCup2026Foot_ClientOpenMain /
//            WorldCup2026Task_StartTask / WorldCupMainPage_ClientGuide (first half) and
//            WorldCup2026TaskSecond_OnMainViewClick</N>1..8 / _OpenItemGuide (second half).
//    highlight ActInfo[10900] ("Pitch Glory", com_gg page) -> 10511 LUAViewSummary_ClientEnterButton</N>10900.
//  Real data sent: Stage, the official Time, RaceDay computed from the official Cuju Tournament schedule
//  (tWorldCupSecond_DelText[35660][Text615]: Jun 30, Jul 1, 2, 4, 5, 8, 10), points / attempts 0 and no red dots
//  (nothing on this server awards them). Guide is NOT sent: Field Whistle / Champion Horn / Golden Ball Coin are not in
//  the client itemtype, so there is no item id to show.
//
//  [2026-10-15] Vault Palace Visit teleports to Vault Palace 12818 (the official Windrise statue spot). The rest answers with the
//  exact reason (Reason()): the 2026 World Cup items (Field Whistle, Pitch Point Box, Cheer Coin, prizes 3356639+/3600869) are newer
//  than this client's itemtype, the pitch map 13319 (2026worldcup_new) is not in client_test GameMap.dat, Lucky Cheer is real-match betting.
//  Config (optional): Data\WorldCup2026\worldcup2026.json {firstHalf, secondHalf, minLevel, firstHalfTime, secondHalfTime}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10900 + 10511 enter button), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.WorldCup2026
{
    public static class WorldCup2026Ui
    {
        public const uint LuaMsg = 10900;
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10900";

        public sealed class Config
        {
            public string FirstHalf = "WorldCup2026Windrise";
            public string SecondHalf = "WorldCup2026";
            public int MinLevel = 80;                // official: "Requirement: Level 80" (tWorldCupMainPage_DelText[35642][Text515])
            public string FirstHalfTime = "06-09 00:00 06-29 23:59";   // DERIVED: Lucky Cheer Round of 32 opens Jun 9, second half starts Jun 30
            public string SecondHalfTime = "06-30 00:00 07-20 23:59";  // official 6.30-7.20
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "WorldCup2026", "worldcup2026.json"); } }
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
                        var s = FL.Str(d, "firstHalf"); if (!string.IsNullOrWhiteSpace(s)) c.FirstHalf = s.Trim();
                        s = FL.Str(d, "secondHalf"); if (!string.IsNullOrWhiteSpace(s)) c.SecondHalf = s.Trim();
                        var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
                        s = FL.Str(d, "firstHalfTime"); if (!string.IsNullOrWhiteSpace(s)) c.FirstHalfTime = s.Trim();
                        s = FL.Str(d, "secondHalfTime"); if (!string.IsNullOrWhiteSpace(s)) c.SecondHalfTime = s.Trim();
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[WorldCup2026] worldcup2026.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        static bool IsRunning(string key) { try { return !string.IsNullOrEmpty(key) && Festivals.FestivalManager.IsRunning(key); } catch { return false; } }
        public static bool FirstRunning { get { return IsRunning(Cfg.FirstHalf); } }
        public static bool SecondRunning { get { return IsRunning(Cfg.SecondHalf); } }

        // official texts (luacfg Server_Key tWorldCupMainPage_DelText / tWorldCupSecond_DelText / tWorldCup2026TaskSecond_DelText)
        const string TxtOverFirst = "The Pitch Glory (Windrise) event has ended. We look forward to working with you again next time!";
        const string TxtOverSecond = "The Pitch Glory (Cloudsurge) event has ended. We look forward to working with you again next time!";
        const string TxtLevel = "You need to reach Level 80 to join Pitch Glory.";

        // official Cuju Tournament days (tWorldCupSecond_DelText[35660][Text615])
        static readonly int[][] RaceDays = { new[] { 6, 30 }, new[] { 7, 1 }, new[] { 7, 2 }, new[] { 7, 4 }, new[] { 7, 5 }, new[] { 7, 8 }, new[] { 7, 10 } };

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        /// <summary>Official names of the window's parts (tWorldCupMainPage_DelText[Msg] / tWorldCup2026TaskSecond_DelText[Msg][ItemGuideN] / Cn_Res STR_LUAUI_26FOOT_MAIN_*).</summary>
        static string PartName(string func, string[] args)
        {
            string a0 = args != null && args.Length > 0 ? args[0] : "";
            switch (func)
            {
                case "WorldCupMainPage_Go": return a0 == "1" ? "Lucky Cheer" : a0 == "2" ? "Vault Palace Visit" : "Points & Ranking Rewards";
                case "WorldCupLottery2026_OpenLottery": return "Golden Boot";
                case "WorldCupLottery2026_OpenShop": return "Pitch Store";
                case "WorldCup2026Foot_ClientOpenMain": return "Pitch Flag Capture";
                case "WorldCup2026Task_StartTask": return "Cuju Dream";
                case "WorldCup2026TaskSecond_OnMainViewClick":
                    switch (a0)
                    {
                        case "1": return "Points & Ranking Rewards";
                        case "2": return "Lucky Cheer";
                        case "3": return "Penalty Kick-Off";
                        case "4": return "Cuju Training";
                        case "5": return "Cuju Tournament";
                        case "6": return "Pitch Adventure";
                        case "7": return "Vault Palace Visit";
                        case "8": return "Pitch Store";
                    }
                    break;
            }
            return null;
        }

        /// <summary>LuaPush.HandleCallback (msg 10900, and 10511 for the Highlight Events "enter" button). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == HighlightMsg)
            {
                if (func != "LUAViewSummary_ClientEnterButton" || args == null || args.Length == 0 || args[0] != HighlightAct) return false;
                if (Throttled(c.Player.UID)) return true;
                Open(c, SecondRunning || !FirstRunning, HighlightMsg);
                return true;
            }
            if (msgId != LuaMsg) return false;
            if (!func.StartsWith("WorldCup", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (func == "WorldCupMainPage_OpenMain") { Open(c, !FirstRunning && SecondRunning, LuaMsg); return true; }
            if (func == "WorldCup2026TaskSecond_OpenMainView") { Open(c, SecondRunning || !FirstRunning, LuaMsg); return true; }
            bool second = func.StartsWith("WorldCup2026TaskSecond_", StringComparison.Ordinal);
            if (second ? !SecondRunning : !FirstRunning) { Msg(c, second ? TxtOverSecond : TxtOverFirst); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            var name = PartName(func, args);
            if (name == "Vault Palace Visit") { VaultVisit(c); return true; }// [WorldCup2026 2026-10-15] real map
            Msg(c, Reason(name));
            return true;
        }

        // ------------------------------------------------------------------ [WorldCup2026 2026-10-15] COMPLETE phase
        // Vault Palace = official map id 12818 (FestivalMaps maps.txt -> doc 12019). The official minimap of doc 12019 marks the
        // Pitch Glory (Windrise) statue at (310,242) (luacfg minimap.lua, Cn_Res STR_INI_LUA_MINIMAP_Target_12019_NAME_6); that cell
        // is the statue itself, so the arrival is the nearest walkable cell (304,243) (DERIVED).
        public const uint VaultMap = 12818; public const ushort VaultX = 304, VaultY = 243;
        static readonly System.Collections.Generic.HashSet<uint> VaultFrom = new System.Collections.Generic.HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036 };
        static void VaultVisit(Client.GameClient c)
        {
            if (c.Player.Map == VaultMap) { Msg(c, "You are already in Vault Palace."); return; }
            if (!VaultFrom.Contains(c.Player.Map) || c.Player.DynamicID != 0 || !c.Player.Alive) { Msg(c, "Please go back to Twin City first."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, VaultX, VaultY, VaultMap, 0); else c.Teleport(VaultX, VaultY, VaultMap, 0);
        }
        /// <summary>Why a part can't run here (the 2026 World Cup items are newer than this client's itemtype).</summary>
        public static string Reason(string name)
        {
            switch (name)
            {
                case "Golden Boot":
                case "Pitch Store":
                    return name + " needs Field Whistles and its 2026 prizes, which are not in this client`s item list, so it can`t run on this server.";
                case "Points & Ranking Rewards":
                    return "Pitch Points come from Field Whistles and Pitch Point Boxes, which are not in this client`s item list, so there are no points or ranking rewards on this server.";
                case "Lucky Cheer":
                    return "Lucky Cheer is betting on the real 2026 World Cup matches (cross-server, with Cheer Coins that are not in this client), so it can`t run on this server.";
                case "Pitch Flag Capture":
                case "Penalty Kick-Off":
                case "Cuju Training":
                case "Cuju Tournament":
                    return name + " is played on the Cuju Tournament pitch (official map 13319 = 2026worldcup_new), which this client`s map list does not have, so it can`t run on this server.";
                case "Cuju Dream":
                case "Pitch Adventure":
                    return name + " is a server story (Albert`s Dream) whose steps and prizes are not in the client, so it can`t run on this server.";
            }
            return (name ?? "This part of Pitch Glory") + " isn`t available on this server.";
        }

        static string Stamp(string mmdd, int year, bool end)
        {
            DateTime d;
            if (DateTime.TryParseExact(year + "-" + mmdd.Trim(), new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd" },
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                return d.ToString(end && mmdd.Trim().Length <= 5 ? "yyyy-MM-dd 23:59" : "yyyy-MM-dd HH:mm");
            return null;
        }
        /// <summary>"MM-dd HH:mm MM-dd HH:mm" (config) -> the client's "yyyy-MM-dd HH:mm yyyy-MM-dd HH:mm" for this year.</summary>
        public static string TimeText(string range, int year)
        {
            var p = (range ?? "").Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 4)
            {
                string a = Stamp(p[0] + " " + p[1], year, false), b = Stamp(p[2] + " " + p[3], year, true);
                if (a != null && b != null) return a + " " + b;
            }
            return "";
        }
        /// <summary>RaceDay for the window: 0 = a Cuju Tournament match today, n = next match in n days, -1 = all played.</summary>
        public static int RaceDay(DateTime now)
        {
            var today = now.Date;
            foreach (var md in RaceDays)
            {
                var d = new DateTime(today.Year, md[0], md[1]);
                if (d == today) return 0;
                if (d > today) return (int)(d - today).TotalDays;
            }
            return -1;
        }

        /// <summary>The main window call (see the header for the field meaning).</summary>
        public static string OpenData(bool second, DateTime now)
        {
            var cfg = Cfg;
            string time = TimeText(second ? cfg.SecondHalfTime : cfg.FirstHalfTime, now.Year);
            return "StrToTable|Fui_26foot_main_Open({['Code']=200,['Data']={['Stage']=" + (second ? 1 : 0)
                + ",['Time']=" + LuaUi.LuaPush.Q(time)
                + ",['JiFen']={0,0},['DuoQi']={0,0,0},['IsFinish']=0,['IsFinish2']=0,['RaceDay']=" + (second ? RaceDay(now) : 0)
                + ",['BtnRed']={0,0,0,0,0,0,0,0}}})";
        }
        static void Open(Client.GameClient c, bool second, uint replyMsg)
        {
            bool running = second ? SecondRunning : FirstRunning;
            if (!running) { Msg(c, second ? TxtOverSecond : TxtOverFirst); return; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return; }
            LuaUi.LuaPush.Send(c, OpenData(second, DateTime.Now), replyMsg);
        }
    }
}
