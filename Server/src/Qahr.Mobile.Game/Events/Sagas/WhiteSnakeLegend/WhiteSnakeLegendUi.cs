// ============================================================================
//  [WhiteSnakeLegend 2026-10-14] The official Spring Festival 2025 "Bless All" window (client msg 10527
//  tMsg_Info["NewYear25"], Fgui Fgui_25ChunJie_Main, top-bar button FGUI_ActivityMenu["150235"]).
//  Shown while the festival WhiteSnakeLegend (Questinfo 8350-8358, Data\Festivals\WhiteSnakeLegend) runs.
//
//  Official client (luaui\fairygui\window\fgui_25chunjie_main, decrypted):
//    click   10527 CJMainTask2024_MainView -> Fgui_25ChunJie_Main_OpenView|{Code=200,Data={ActList={4 x {State 0 not open /
//              1 open / 2 done, Times, Tar, Red}}, MainItem={sysid,n}, Score, ScoreRed, RewardScore, IsWakeUp}}
//            ActList 1 White Snake Mark (Cn_Res "Story Rewards (x/y)"), 2 Green Snake Adventure, 3 Bless All, 4 Nian carnival.
//            MainItem 111001000 = Fortune Knack 3351698 (luauirewardshow).
//    window  CJMainTask2024_ButtonClick</N>1..4 (entries), </N>5 (draw), CJMainTask2024_RewardView (points),
//            LuckyPool2024_LuckClientData (Blessing Pool).
//  Entry 1 starts 8350 (official BeginNpcId=0) and teleports to the White Snake on the official map 1003905 (doc 12734);
//  the next quests follow on their own (quests.json next + autoAccept).
//  NOT on this server (answered with a system message, nothing invented): entries 2-4, the draw, points, Blessing Pool,
//  the White Snake awakening (Radiant Pearls). Score / RewardScore are 0, IsWakeUp 0.
//  Config (derived values): Data\WhiteSnakeLegend\whitesnakelegend.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10527), tagged.
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

namespace Qahr.Mobile.WhiteSnakeLegend
{
    public static class WhiteSnakeLegendUi
    {
        public const uint LuaMsg = 10527;

        public sealed class Config
        {
            public string Festival = "WhiteSnakeLegend";
            public int MinLevel = 80;// Questinfo 8350 Lv_min
            public uint First = 8350, Last = 8358;
            public uint Map = 1003905; public ushort X = 542, Y = 402;// next to the White Snake (539,398)
            public uint FortuneKnack = 3351698, FortuneKnackSys = 111001000;
            public HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1003905 };
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "WhiteSnakeLegend", "whitesnakelegend.json"); } }
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
                    try { MyConsole.WriteLine("[WhiteSnakeLegend] whitesnakelegend.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }// report a bad file once, not on every call
                }
                return _cfg;
            }
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
            var e = FL.Val(d, "entry") as Dictionary<string, object>;
            if (e != null)
            {
                uint m = FL.U(FL.Val(e, "map")), x = FL.U(FL.Val(e, "x")), y = FL.U(FL.Val(e, "y"));
                if (m != 0 && x > 0 && y > 0 && x < 65535 && y < 65535) { c.Map = m; c.X = (ushort)x; c.Y = (ushort)y; }
            }
            var from = FL.Arr(FL.Val(d, "teleportFromMaps"));
            if (from != null) c.TeleportFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastEnter = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10527).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (!func.StartsWith("CJMainTask2024_", StringComparison.Ordinal) && func != "LuckyPool2024_LuckClientData") return false;
            if (Throttled(_last, c.Player.UID, 400)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, "The Spring Festival has ended. See you next year!"); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return true; }
            switch (func)
            {
                case "CJMainTask2024_MainView":
                    Lua(c, "Fgui_25ChunJie_Main_OpenView|" + MainData(c, cfg));
                    return true;
                case "CJMainTask2024_ButtonClick":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i)) return true;
                        if (i == 1)
                        {
                            if (Throttled(_lastEnter, c.Player.UID, 3000)) return true;
                            Enter(c, cfg);
                        }
                        else Msg(c, "This event isn`t available on this server yet.");// 2-4 Bless All / Green Snake / Year Beast, 5 draw
                        return true;
                    }
                case "CJMainTask2024_RewardView":
                    Msg(c, "Spring Festival points aren`t available on this server yet.");
                    return true;
                case "LuckyPool2024_LuckClientData":
                    Msg(c, "The Blessing Pool isn`t available on this server yet.");
                    return true;
            }
            return true;
        }

        static bool Finished(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; } }
        static bool Taken(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; } }
        static uint Count(Client.GameClient c, uint id) { try { return (uint)Math.Max(0, c.Inventory.GetCountItem(id)); } catch { return 0; } }

        public static string MainData(Client.GameClient c, Config cfg)
        {
            int total = (int)(cfg.Last - cfg.First + 1), done = 0;
            for (uint q = cfg.First; q <= cfg.Last; q++) if (Finished(c, q)) done++;
            bool any = false;
            for (uint q = cfg.First; q <= cfg.Last; q++) if (Taken(c, q) && !Finished(c, q)) any = true;
            int state = done >= total ? 2 : 1;
            int red = state == 1 && !any ? 1 : 0;// nothing in progress yet: the entry waits for you
            var sb = new StringBuilder("{['Code']=200,['Data']={['ActList']={");
            sb.Append("{['State']=").Append(state).Append(",['Times']=").Append(done).Append(",['Tar']=").Append(total).Append(",['Red']=").Append(red).Append('}');
            for (int i = 2; i <= 4; i++) sb.Append(",{['State']=0,['Times']=0,['Tar']=0,['Red']=0}");
            sb.Append("},['MainItem']={").Append(cfg.FortuneKnackSys).Append(',').Append(Count(c, cfg.FortuneKnack)).Append('}');
            return sb.Append(",['Score']=0,['ScoreRed']=0,['RewardScore']=0,['IsWakeUp']=0}}").ToString();
        }

        static void Enter(Client.GameClient c, Config cfg)
        {
            var p = c.Player;
            if (Finished(c, cfg.Last)) { Msg(c, "You have completed this event. Please participate in other events."); return; }
            if (!p.Alive || p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map)) { Msg(c, "You can`t go to the White Snake from here. Please go back to a city first."); return; }
            if (Pool.ServerMaps[cfg.Map] == null) { Msg(c, "This story isn`t open yet."); return; }
            var t = QuestEngine.QuestEngineCore.Current;
            if (t == null) { Msg(c, "This story isn`t open yet."); return; }
            bool inChain = false;
            for (uint q = cfg.First; q <= cfg.Last; q++) if (Taken(c, q)) inChain = true;
            if (!inChain)
            {
                QuestEngine.EngineQuest first;
                if (!t.Quests.TryGetValue(cfg.First, out first)) { Msg(c, "This story isn`t open yet."); return; }
                string name, why;
                if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, first.BeginNpc, cfg.First, out name, out why))
                {
                    Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                    return;
                }
                Msg(c, "[Quest] New quest: " + name + ".");
            }
            if (p.Map == cfg.Map) return;
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, cfg.X, cfg.Y, cfg.Map, 0); else c.Teleport(cfg.X, cfg.Y, cfg.Map, 0);
        }
    }
}
