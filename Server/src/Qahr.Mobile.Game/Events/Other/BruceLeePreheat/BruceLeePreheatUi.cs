// ============================================================================
//  [BruceLeePreheat 2026-10-14] The official Dragon Warrior awakening preheat "Beast Scroll" (Dragon / Kunpeng / Suanni,
//  Peach Village), client msg 10127 tMsg_Info["AwakenBruceLee"], lua dialog luaui\dialog\dlgawakenbrucelee
//  (DlgAwakenBruceLeeBtn layout 1300-btn, DlgAwakenBruceLeeEntrance Dialog_Info 1300-2). Script header 2022-10-08,
//  Peach Village opened "at 8:00 of Feb. 14" (luaUiRes[1300].notopentyc). Shown while the festival BruceLeePreheat runs.
//
//  Official client (luaui, decrypted):
//    button  DlgAwakenBruceLee_OpenMainBtn|{Code=200,Data={[nTime=countdown s]}}; hide LuaUIMgr_HideDialog("DlgAwakenBruceLeeBtn").
//    click   10127 XiaoLongPreheat_MainView -> DlgAwakenBruceLee_OpenEntranceView|{Code=100,Data={nTime,tAnimalState={3},
//            tDayState={7},nBubbleIndex,tAnimalGuide}}  (Beast Scroll: 3 beasts, 7-Day Gift, Celestial Pool).
//            tAnimalState 0 = locked ("will be unlocked tomorrow" / "the day after tomorrow"), >=1 = Beast Trial open.
//            tDayState 0 locked (masked), 1 claimable, 2/3 claimed.
//    window  XiaoLongPreheat_BeastView</N>i (trials window), XiaoLongPreheat_WishView / _MakeWish (Celestial Pool),
//            LongPreheating_7DaysReward</N>d, LongPreheating_takeTask / _FindTask / _GodBeastReward</N>beast</N>task,
//            XiaoLongPreheat_ShowBeast, XiaoLongPreheat_GuideGoto, DragonNewAreaTask_ClickEnd.
//  Texts: luaUiRes[1300], Server_Key tXiaoLongPreheat_DelText / tLongPreheating_DelText.
//
//  REAL data only: beast i is open from event day i (official texts "unlocked tomorrow" / "the day after tomorrow";
//  day 1 = the day the festival started on this server, see README), the 7-Day Gift row left at its default (tDayState omitted, nothing claimable here),
//  no countdown (Peach Village is not on this server).
//  NOT on this server (system message, nothing granted or taken): Beast Trials (Wind Plain / Sea of Sinking Ship /
//  Zhenwu Temple instances), 7-Day Gift claiming, Celestial Pool wishes (Dragon Scales), Peach Village.
//  Config (optional): Data\BruceLeePreheat\bruceleepreheat.json {festival, minLevel}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10127), tagged [BruceLeePreheat 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.BruceLeePreheat
{
    public static class BruceLeePreheatUi
    {
        public const uint LuaMsg = 10127;// tMsg_Info["AwakenBruceLee"]

        public sealed class Config
        {
            public string Festival = "BruceLeePreheat";
            public int MinLevel = 80;// tXiaoLongPreheat_DelText[105][6]
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "BruceLeePreheat", "bruceleepreheat.json"); } }
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
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[BruceLeePreheat] bruceleepreheat.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        /// <summary>Event day 1.. (1 = the day the festival started on this server).</summary>
        public static int EventDay()
        {
            try
            {
                var r = Festivals.FestivalManager.Running(Cfg.Festival);
                if (r == null) return 0;
                return Math.Max(1, (int)(DateTime.Now.Date - r.Started.Date).TotalDays + 1);
            }
            catch { return 1; }
        }

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

        static readonly HashSet<string> Funcs = new HashSet<string>(StringComparer.Ordinal)
        {
            "XiaoLongPreheat_MainView", "XiaoLongPreheat_BeastView", "XiaoLongPreheat_WishView", "XiaoLongPreheat_MakeWish",
            "XiaoLongPreheat_ShowBeast", "XiaoLongPreheat_GuideGoto", "LongPreheating_7DaysReward", "LongPreheating_takeTask",
            "LongPreheating_FindTask", "LongPreheating_GodBeastReward", "DragonNewAreaTask_ClickEnd",
        };

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10127).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Funcs.Contains(func)) return false;
            if (Throttled(c.Player.UID)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, "It is not the event time, so you cannot join in it now."); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return true; }
            switch (func)
            {
                case "XiaoLongPreheat_MainView":
                case "XiaoLongPreheat_GuideGoto":
                    Lua(c, "DlgAwakenBruceLee_OpenEntranceView|" + EntranceData(EventDay()));
                    return true;
                case "XiaoLongPreheat_BeastView":
                case "XiaoLongPreheat_ShowBeast":
                case "LongPreheating_takeTask":
                case "LongPreheating_FindTask":
                case "LongPreheating_GodBeastReward":
                    Msg(c, "The Beast Trials aren`t available on this server yet.");
                    return true;
                case "XiaoLongPreheat_WishView":
                case "XiaoLongPreheat_MakeWish":
                    Msg(c, "The Celestial Pool isn`t available on this server yet.");
                    return true;
                case "LongPreheating_7DaysReward":
                    Msg(c, "The 7-Day Gift isn`t available on this server yet.");
                    return true;
                case "DragonNewAreaTask_ClickEnd":
                    Msg(c, "Peach Village isn`t open on this server yet.");
                    return true;
            }
            return true;
        }

        /// <summary>Beast Scroll data: beast i open from event day i, 7-Day Gift untouched, no countdown.</summary>
        public static string EntranceData(int day)
        {
            var sb = new StringBuilder("{['Code']=100,['Data']={['tAnimalState']={");
            for (int i = 1; i <= 3; i++) sb.Append(i > 1 ? "," : "").Append(day >= i ? 1 : 0);
            sb.Append("}}}");// no tDayState: the 7-Day Gift keeps its default (nothing claimable); an all-0 list hits a nil progress index in BindItemData
            return sb.ToString();
        }
    }
}
