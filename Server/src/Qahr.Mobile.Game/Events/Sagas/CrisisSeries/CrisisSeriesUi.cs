// ============================================================================
//  [CrisisSeries 2026-10-14] White Day 2025 "Castle Startle" (Green / Smith / Miller castles), client msg 10553
//  tMsg_Info["WhiteDay25"]. Shown while the festival CrisisSeries runs (festival.json icon_show/icon_hide = Fgui 150249).
//
//  Official client (luaui, decrypted):
//    top bar   FGUI_ActivityMenu["150249"] (label "Castle Startle"); click -> WhiteDayEvent2025_OpenMainDialog
//    main      Fgui_25Bsqrj_Gbjq_OpenView|{Code=200,Data={Score,ActList={[0..5]={State,Cur,Red}},ItemInfo={showId,n},Hand}}
//              castles = ActList[1..3] (State 0 new / 1 liberating (Cur 0-100) / >=2 liberated); btn_click_i ->
//              WhiteDayEvent2025_ClickCastle</N>i
//    board     Fgui_25Bsqrj_Jxmx_OpenView|{Code=200,Data={Map,Page,CurIndex,ItemInfo={showId,n,need},Hand,Skip,BossRed,BoxRed}}
//              3 pages x 20 cells per castle, cell 20 = the stage boss (BossId look); "Move forward 1 step" ->
//              WhiteDayEvent2025_BegainChallenge</N>map</N>page</N>index ; buttons -> ChallengeMonster / OpenBox ;
//              Back -> OpenMainDialog ; ClickWeb (rewards) ; ClickSkip
//    rules     STR_LUAUI_25BSQRJ_MAIN_RULE_1: "Each castle will be liberated after you complete 3 stages. Defeating the
//              final boss of each stage increases liberation progress." RULE_3: "Only after full liberation can you
//              liberate other castles and challenge the Castle Spirit."
//  On this server: the 9 stage bosses = Questinfo quests 8740/8746/8751 (Green), 8757/8762/8767 (Smith),
//  8773/8778/8783 (Miller), each fought in a private copy of its official instance map (quests.json instances).
//  The board opens at the boss cell (the event cells before it have no official data); boxes, board monsters,
//  rewards page and the Castle Spirit answer with a system message. Questinfo has no prize for these quests.
//  Config (derived values): Data\CrisisSeries\crisis.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10553), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.CrisisSeries
{
    public static class CrisisSeriesUi
    {
        public const uint LuaMsg = 10553;

        /// <summary>Stage boss quest per castle (1 Green, 2 Smith, 3 Miller) and stage (1-3) = official board order.</summary>
        public static readonly uint[][] Stages =
        {
            new uint[] { 8740, 8746, 8751 },
            new uint[] { 8757, 8762, 8767 },
            new uint[] { 8773, 8778, 8783 },
        };
        static readonly string[] Castle = { "Green", "Smith", "Miller" };

        public sealed class Config
        {
            public string Festival = "CrisisSeries";
            public int MinLevel = 80;
            public uint BunShowId = 136001000, BunItem = 3352187;
            public HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036 };
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "CrisisSeries", "crisis.json"); } }
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
                        if (d != null)
                        {
                            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
                            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
                            var si = FL.Val(d, "bunShowId"); if (si != null) c.BunShowId = FL.U(si);
                            var bi = FL.Val(d, "bunItem"); if (bi != null) c.BunItem = FL.U(bi);
                            var from = FL.Arr(FL.Val(d, "teleportFromMaps"));
                            if (from != null) c.TeleportFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
                        }
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[CrisisSeries] crisis.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

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

        // ------------------------------------------------------------------ progress (from the quest log only)
        static bool Done(Client.GameClient c, uint q) { return c.Player.QuestGUI.CheckQuest(q, MsgQuestList.QuestListItem.QuestStatus.Finished); }
        static bool Active(Client.GameClient c, uint q) { return c.Player.QuestGUI.IsActiveQuest(q); }
        /// <summary>Stages finished in castle i (0-based), 0-3.</summary>
        public static int StagesDone(Client.GameClient c, int i) { int n = 0; foreach (var q in Stages[i]) if (Done(c, q)) n++; else break; return n; }
        /// <summary>0 not started, 1 liberating, 2 liberated (client c_state).</summary>
        public static int State(Client.GameClient c, int i)
        {
            int d = StagesDone(c, i);
            if (d >= 3) return 2;
            return d > 0 || Stages[i].Any(q => Active(c, q)) ? 1 : 0;
        }

        /// <summary>Engine finish of a stage boss whose objectives are done (official FinishNpcId 0 = finished on the spot).</summary>
        static void FinishDone(Client.GameClient c)
        {
            var t = QuestEngine.QuestEngineCore.Current;
            if (t == null) return;
            foreach (var row in Stages)
                foreach (var id in row)
                {
                    QuestEngine.EngineQuest q;
                    if (!Active(c, id) || !t.Quests.TryGetValue(id, out q) || q.FinishNpc != 0) continue;
                    if (!QuestEngine.QuestEngineCore.IsComplete(c, q, false)) continue;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        string msg;
                        if (QuestEngine.QuestEngineCore.TryFinish(c, q, rec.GetStream(), 0, out msg))
                            Msg(c, "[Castle Startle] " + q.Name + " complete." + (msg.Length > 0 ? " " + msg : ""));
                    }
                }
        }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10553).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !func.StartsWith("WhiteDayEvent2025_", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, "The Castle Startle event has ended. See you next White Day!"); return true; }
            var cfg = Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "You need to reach Level " + cfg.MinLevel + " to join Castle Startle."); return true; }
            switch (func)
            {
                case "WhiteDayEvent2025_OpenMainDialog":
                    FinishDone(c);
                    OpenMain(c, cfg);
                    return true;
                case "WhiteDayEvent2025_ClickCastle":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 0 || i > 5) return true;
                        FinishDone(c);
                        if (i >= 1 && i <= 3) { OpenCastle(c, cfg, i - 1); return true; }
                        // 0 / 4 / 5: Auter Castle and the Castle Spirit (no official server data)
                        bool all = Enumerable.Range(0, 3).All(k => State(c, k) >= 2);
                        Msg(c, all ? "The Castle Spirit challenge isn`t open on this server yet." : "Liberate the Green, Smith and Miller castles to challenge the Castle Spirit.");
                        return true;
                    }
                case "WhiteDayEvent2025_BegainChallenge":
                    {
                        int map, page, idx;
                        if (args.Length < 3 || !int.TryParse(args[0], out map) || !int.TryParse(args[1], out page) || !int.TryParse(args[2], out idx)) return true;
                        if (map < 1 || map > 3 || page < 1 || page > 3) return true;
                        FinishDone(c);
                        Challenge(c, cfg, map - 1, page - 1);
                        return true;
                    }
                case "WhiteDayEvent2025_ChallengeMonster":
                    Msg(c, "The monsters on the board aren`t available on this server yet. Move forward to challenge the stage boss.");
                    return true;
                case "WhiteDayEvent2025_OpenBox":
                    Msg(c, "The treasure chests on the board aren`t available on this server yet.");
                    return true;
                case "WhiteDayEvent2025_ClickWeb":
                    Msg(c, "Castle Startle gives no event rewards on this server yet.");
                    return true;
                case "WhiteDayEvent2025_ClickSkip":
                    return true;
            }
            Msg(c, "This part of Castle Startle isn`t open on this server yet.");
            return true;
        }

        static int Buns(Client.GameClient c, Config cfg) { try { return Math.Max(0, c.Inventory.GetCountItem(cfg.BunItem)); } catch { return 0; } }

        public static string MainData(Client.GameClient c, Config cfg)
        {
            var sb = new StringBuilder("Fgui_25Bsqrj_Gbjq_OpenView|{['Code']=200,['Data']={['Score']=0,['Hand']=0,['ItemInfo']={");
            sb.Append(cfg.BunShowId).Append(',').Append(Buns(c, cfg)).Append("},['ActList']={");
            for (int i = 0; i <= 5; i++)
            {
                if (i > 0) sb.Append(',');
                int st = 0, cur = 0;
                if (i >= 1 && i <= 3) { st = State(c, i - 1); cur = StagesDone(c, i - 1) * 100 / 3; if (st >= 2) cur = 100; }
                sb.Append('[').Append(i).Append("]={['State']=").Append(st).Append(",['Cur']=").Append(cur).Append(",['Red']=0}");
            }
            return sb.Append("}}}").ToString();
        }
        static void OpenMain(Client.GameClient c, Config cfg) { Lua(c, MainData(c, cfg)); }

        /// <summary>The castle another castle's liberation blocks (RULE_3), or -1.</summary>
        static int Blocking(Client.GameClient c, int i)
        {
            for (int k = 0; k < 3; k++) if (k != i && State(c, k) == 1) return k;
            return -1;
        }

        public static string BoardData(Client.GameClient c, Config cfg, int i, int page, int cur)
        {
            return "Fgui_25Bsqrj_Jxmx_OpenView|{['Code']=200,['Data']={['Map']=" + (i + 1) + ",['Page']=" + (page + 1) + ",['CurIndex']=" + cur
                + ",['ItemInfo']={" + cfg.BunShowId + "," + Buns(c, cfg) + ",0},['Hand']=0,['Skip']=0,['BossRed']=0,['BoxRed']={0,0}}}";
        }
        static void OpenCastle(Client.GameClient c, Config cfg, int i)
        {
            int b = Blocking(c, i);
            if (b >= 0 && State(c, i) == 0) { Msg(c, "Fully liberate " + Castle[b] + " Castle first, then you can liberate other castles."); return; }
            int done = StagesDone(c, i);
            if (done >= 3) { Lua(c, BoardData(c, cfg, i, 2, 19)); Msg(c, Castle[i] + " Castle is already liberated."); return; }
            Lua(c, BoardData(c, cfg, i, done, 18));// at the step before this stage's boss (cell 20)
        }

        static void Challenge(Client.GameClient c, Config cfg, int i, int page)
        {
            var p = c.Player;
            int done = StagesDone(c, i);
            if (done >= 3) { Msg(c, Castle[i] + " Castle is already liberated."); return; }
            if (page != done) { Lua(c, BoardData(c, cfg, i, done, 18)); return; }// stale board: show the current stage
            int b = Blocking(c, i);
            if (b >= 0 && State(c, i) == 0) { Msg(c, "Fully liberate " + Castle[b] + " Castle first, then you can liberate other castles."); return; }
            if (!p.Alive) { Msg(c, "You can`t start a challenge now."); return; }
            if (p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map)) { Msg(c, "Go back to a city to start the castle challenge."); return; }
            uint qid = Stages[i][page];
            var t = QuestEngine.QuestEngineCore.Current;
            QuestEngine.EngineQuest q;
            if (t == null || !t.Quests.TryGetValue(qid, out q) || q.FestivalOff || !q.Owned || !q.Workable) { Msg(c, "This castle challenge isn`t available yet."); return; }
            if (Active(c, qid))
            {
                // already accepted (died / left): back into the own copy (a new copy if the old one expired)
                QuestEngine.QuestEngineCore.AfterPanelAccept(c, qid);
                return;
            }
            Database.QuestInfo.DBQuest dbq; string why;
            if (!Database.QuestInfo.AllQuests.TryGetValue(qid, out dbq)) { Msg(c, "This castle challenge isn`t available yet."); return; }
            if (!QuestEngine.QuestEngineCore.CanAccept(c, dbq, out why)) { Msg(c, "[Castle Startle] " + why); return; }
            if (!QuestEngine.QuestEngineCore.AllowPanelAccept(c, qid)) return;
            c.Player.QuestGUI.Accept(dbq, 0);
            if (!Active(c, qid)) { Msg(c, "This castle challenge isn`t available yet."); return; }
            QuestEngine.QuestEngineCore.AfterPanelAccept(c, qid);// onAccept: private copy of the official map + the boss
            Msg(c, "[Castle Startle] " + q.Name + ": defeat the boss, then open the Castle Startle window to report.");
        }
    }
}
