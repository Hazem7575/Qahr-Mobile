// ============================================================================
//  [SwordSaga 2026-10-14] The official Qixi 2026 "Feathered Romance" entry (client msg 10950 tMsg_Info["Qixi26"]).
//  Shown while the festival SwordSaga (Questinfo TypeId 8 quests 9898-9922, maps 1005869-1005882) runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150469"] (img lua_2026qqqx70, EN "Feathered Romance"), clickfunc
//              Qixi2026Npc_OpenMainView on msg 10950. Pushed by festival.json icon_show / icon_hide.
//    window    g_function Fui_QiXi26Main_Open(tData) takes a TABLE -> sent as "StrToTable|Fui_QiXi26Main_Open({...})".
//              Event time shown by the window: Aug. 11 08:59 (US)/16:59 (EU) - Aug. 31.
//              tData = {Code=200,Data={StarInfo={3 x {id,red,msgid,sfunc,nowCount,maxCount}},
//              JiFen={cur,max},Award={red,msgid,sfunc},ChongZhu={red},FirstGuide={0|1 x3}}}
//              star id 1-7 = Sword Aura / Essence / Light / Path / Heart / Force / Spirit realm (qixi26mainmediator,
//              Cn_Res STR_FGUI_26QIXI_MAIN_STAR_n "%s/%s"). btn_sword -> QiXi2026ZJ_OpenZJView</N>0 (Sword Forge).
//    others    Qixi2026Npc_OpenMainGuide, QiXi2026ZJ_* (Sword Forge, Sword Archive, stage rewards).
//    highlight ActInfo[10950] (com_gg page) -> 10511 LUAViewSummary_ClientEnterButton</N>10950.
//  Rules (Server_Key tQixi2026Npc[37255][Text134]): "Three realms will open daily for training. Each realm can be completed
//  once per day. Realms and attempts refresh at midnight daily." Which three is server data -> derived rotation below.
//  nowCount = 1 when the realm's Iridescent Feather quest is Finished in the player's quest log (real data), else 0.
//
//  NOT on this server (answered with a system message, nothing invented): the seven Sword Realm instances (their NPC
//  types and wave/aura monsters exist in no client data with a look; the prize items 3356909 / 3356910 exist in no
//  itemtype), Qixi Points, the ranking, the Sword Forge and the Sword Bond Store.
//  Config (derived values): Data\SwordSaga\swordsaga.json (optional).
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10950 + 10511 enter button), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.SwordSaga
{
    public static class SwordSagaUi
    {
        public const uint LuaMsg = 10950;
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10950";

        public sealed class Config
        {
            public string Festival = "SwordSaga";
            public int MinLevel = 80;            // official: "Level: 80" (tQixi2026Npc[37255][Text114])
            public uint PointsMax = 0;           // JiFen max (the window shows only the current points)
            /// <summary>star id 1..7 -> the quest whose Finished state means "this realm's feather is collected".
            /// Derived from Questinfo: the last prize quest of each realm (Spirit: 9921, the last step before 9922).</summary>
            public uint[] FeatherQuest = { 9904, 9907, 9911, 9916, 9900, 9917, 9921 };
            /// <summary>Derived: realms open today = 3 consecutive star ids starting at (day number * 3) mod 7.</summary>
            public int RealmsPerDay = 3;
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "SwordSaga", "swordsaga.json"); } }
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
                        var fq = FL.Arr(FL.Val(d, "featherQuests"));
                        if (fq != null && fq.Count == 7) c.FeatherQuest = fq.Select(FL.U).ToArray();
                        var rp = FL.Val(d, "realmsPerDay"); if (rp != null) c.RealmsPerDay = (int)Math.Max(1, Math.Min(3, FL.U(rp)));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[SwordSaga] swordsaga.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (luacfg Server_Key tQixi2026Npc[37255])
        const string TxtOver = "The stars have turned, and the Qixi event has come to a close. Thank you for participating. May happiness follow you, and we shall meet again next year!";
        const string TxtLevel = "You need to reach Level 80 to join Feathered Romance.";
        const string TxtRealm = "The Sword Realms aren`t open on this server yet.";
        const string TxtNotOpen = "This part of Feathered Romance isn`t open on this server yet.";

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        /// <summary>LuaPush.HandleCallback (msg 10950, and 10511 for the Highlight Events "enter" button). True = answered.</summary>
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
            if (!func.StartsWith("Qixi2026", StringComparison.Ordinal) && !func.StartsWith("QiXi2026", StringComparison.Ordinal)) return false;
            switch (func)
            {
                case "Qixi2026Npc_OpenMainGuide":   // guide progress: nothing to store
                case "QiXi2026ZJ_AllViewClose":
                case "QiXi2026ZJ_Skip":
                case "QiXi2026ZJ_SkipEffect":
                case "QiXi2026ZJ_ZJSucClose":
                    return true;
            }
            if (Throttled(c.Player.UID)) return true;
            if (func == "Qixi2026Npc_OpenMainView") { Open(c, LuaMsg); return true; }
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            Msg(c, func == "Qixi2026Npc_EnterRealm" ? TxtRealm : TxtNotOpen);
            return true;
        }

        /// <summary>Star ids (1..7) open today (derived rotation, changes at local midnight).</summary>
        public static int[] RealmsToday(Config cfg, DateTime now)
        {
            int day = (int)((now.Date - new DateTime(2026, 1, 1)).TotalDays % 7 + 7) % 7;
            int n = Math.Max(1, Math.Min(3, cfg.RealmsPerDay));
            var r = new int[n];
            for (int i = 0; i < n; i++) r[i] = (day * 3 + i) % 7 + 1;
            return r;
        }
        static bool Finished(Client.GameClient c, uint q)
        {
            if (q == 0) return false;
            try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; }
        }

        public static string OpenData(Client.GameClient c, Config cfg, DateTime now)
        {
            var sb = new StringBuilder("StrToTable|Fui_QiXi26Main_Open({['Code']=200,['Data']={['StarInfo']={");
            var stars = RealmsToday(cfg, now);
            for (int i = 0; i < stars.Length; i++)
            {
                int id = stars[i];
                bool done = Finished(c, cfg.FeatherQuest[id - 1]);
                if (i > 0) sb.Append(',');
                sb.Append('{').Append(id).Append(",0,").Append(LuaMsg).Append(",'Qixi2026Npc_EnterRealm</N>").Append(id).Append("',")
                  .Append(done ? 1 : 0).Append(",1}");
            }
            sb.Append("},['JiFen']={0,").Append(cfg.PointsMax).Append("},['Award']={0,").Append(LuaMsg).Append(",'Qixi2026Npc_OpenRank'}")
              .Append(",['ChongZhu']={0},['FirstGuide']={0,0,0}}})");
            return sb.ToString();
        }
        static void Open(Client.GameClient c, uint replyMsg)
        {
            if (!Running) { Msg(c, TxtOver); return; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return; }
            LuaUi.LuaPush.Send(c, OpenData(c, cfg, DateTime.Now), replyMsg);
        }
    }
}
