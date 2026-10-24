// ============================================================================
//  [DreamTheater][LuaUi] the official client window of the event: client luaui\dialog\dlgwhiteday (Dialog 1466),
//  callback msgId 10369 = tMsg_Info["DlgWhiteDay"]. Replies: MsgAction 2099 Type 258, dwParam 10369 (Base\LuaUi\LuaPush).
//  [2026-10-15 video pass] rebuilt on the official flow of the videos qiOKUEANFQw / 9ZTyLoh6Pmk / tTO1t9_Tzts / UQaPhJISUQ0
//  (tools\ProtoStudio\out\dreamtheater_video_diff.md):
//
//  Top icon  DlgWhiteDayBtn: festival.json icon_show / icon_hide; click -> WhiteDayLua_OpenMain.
//  Main      DlgWhiteDayMain_OpenView / _UpdateView|{['Code']=200,['Data']={['Type']=1|2 (day/night),
//            ['RedPoint']={stage,upgrade,wish,enter},['TaskIndex']=0..9,['Dance']=0|1|2,['Score']=Theater Points,['Lv']=1..5,
//            ['ItemNum']={Wish Gold Coins,Wish Silver Coins}}}
//            First open: the official guide DlgCommonActivityGuideConfig[10369][1..8] (g_function PublicDlgOPenGuide);
//            its last step calls WhiteDayLua_CompleteFirst (recorded, never shown again).
//  Contract  Agnes offers ONE contract a day (TaskIndex 1-7 = the 7 recruitments in order, 8 cleaning, 9 troupe troubles).
//            ExecuteContracts</N>i -> DlgWhiteDayMain_OpenNpcCallFunc|i (the TaskCenter talk), SolveContracts</N>i accepts it.
//  Dance     0 = no member yet, 1 = waiting, 2 = the day's contract is done and the performance is ready.
//  Enter     MasterAndTheatre2024_EnterTheaterMap: ready -> DlgWhiteDayActFinish (Settlement) -> Settle ->
//            MasterAndTheatre2024_SettleAnAccount -> the performance -> DlgWhiteDayActFinishTwo (silver income + play count);
//            not ready -> the private copy of the theater (12430) "Welcome to Dream Theater! You can freely explore the theater!".
//  Wish      WhiteDay2024Event_Makewish</N>1|2 (Silver = 1 wish, Gold = 10 wishes): the coin is taken and the wish settled at
//            once, DlgWhiteDayMain_PlayDrawEff|i; the client ends the effect with WhiteDay2024Event_CongratulationReward ->
//            the official "You received" popup (DlgPublicReward) with the items of that wish.
//  Rewards   WhiteDayLua_OpenRankAndStage -> DlgMidAutumnRank_BindData (MidAutumn_Config[10369] rows 1-9);
//            WhiteDayLua_GetStageReward</N>10369</N>{row} claims one row; WhiteDayLua_GoRank = the local ranking list.
//  Every reward goes through the module's own checks (WishCore / UpgradeCore / ClaimMilestoneCore / PerformCore).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.LuaUi;

namespace Qahr.Mobile.DreamTheater
{
    public static partial class DreamTheaterModule
    {
        public const uint LuaMsg = 10369;

        static bool Lua(Client.GameClient c, string call) { return LuaPush.Send(c, call, LuaMsg); }
        static string L(IEnumerable<int> l) { return "{" + string.Join(",", l) + "}"; }
        static string Q(string s) { return "'" + (s ?? "").Replace("\\", "\\\\").Replace("'", "\\'") + "'"; }

        /// <summary>Per-player throttle of the window callbacks (double clicks / spam).</summary>
        static readonly ConcurrentDictionary<string, long> _last = new ConcurrentDictionary<string, long>();
        static bool Throttled(Client.GameClient c, string what, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            string k = c.Player.UID + ":" + what;
            if (_last.TryGetValue(k, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[k] = now;
            return false;
        }

        /// <summary>10369 callbacks of the window (true = answered).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (!func.StartsWith("WhiteDay", StringComparison.Ordinal) && !func.StartsWith("MasterAndTheatre2024_", StringComparison.Ordinal)
                && !func.StartsWith("SwanLake_", StringComparison.Ordinal)) return false;
            args = args ?? new string[0];
            try
            {
                if (!Running)
                {
                    Lua(c, "DlgWhiteDayMain_CloseView");
                    Lua(c, "LuaUIMgr_HideDialog|DlgWhiteDayBtn");
                    Msg(c, "Dream Theater is not open now.");
                    return true;
                }
                if (c.Player.Level < Cfg.MinLevel) { Msg(c, "Please upgrade to level " + Cfg.MinLevel + " first."); return true; }
                int a1 = ArgInt(args, 0);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    switch (func)
                    {
                        case "WhiteDayLua_OpenMain":
                            SendMain(c, true);
                            if (args.Length > 0 && args[0] == "Wish") Lua(c, "DlgWhiteDayDrawTip_OpenView");
                            else ShowFirstGuide(c);
                            return true;
                        case "WhiteDayLua_CompleteFirst":
                            { var p = State(c); lock (_lock) { if (!p.GuideDone) { p.GuideDone = true; _dirty = true; } } }
                            return true;
                        case "WhiteDay2024Event_EnterInstance":// guide 9 (Swan Lake miracle): Swan Lake is not on this server
                            Msg(c, "Only when Princess Odette obtains true love will a miracle occur, opening the path to Swan Lake.");
                            return true;
                        case "MasterAndTheatre2024_ExecuteContracts":
                            if (a1 > 0 && a1 == TaskIndex(c)) Lua(c, "DlgWhiteDayMain_OpenNpcCallFunc|" + a1);
                            else SendMain(c, false);
                            return true;
                        case "MasterAndTheatre2024_SolveContracts":
                            if (Throttled(c, "solve", 800)) return true;
                            SolveContract(c, a1); return true;
                        case "MasterAndTheatre2024_OpenGradeInterface": OpenUpgrade(c); return true;
                        case "MasterAndTheatre2024_Renovate":
                            if (Throttled(c, "renovate", 800)) return true;
                            LuaUpgrade(c, stream); return true;
                        case "MasterAndTheatre2024_EnterTheaterMap":
                            if (Throttled(c, "enter", 800)) return true;
                            LuaEnter(c); return true;
                        case "MasterAndTheatre2024_SettleAnAccount":
                            if (Throttled(c, "settle", 1500)) return true;
                            LuaSettle(c, stream); return true;
                        case "WhiteDay2024Event_OpenWishMain": Lua(c, "DlgWhiteDayDrawTip_OpenView"); return true;
                        case "WhiteDay2024Event_Makewish":
                            if (Throttled(c, "wish", 1300)) return true;// the client plays a 1.1 s effect per wish
                            LuaWish(c, stream, a1); return true;
                        case "WhiteDay2024Event_CongratulationReward": ShowWishReward(c); return true;
                        case "WhiteDayLua_OpenRankAndStage": Lua(c, "DlgMidAutumnRank_BindData|" + StageData(c)); return true;
                        case "WhiteDayLua_GetStageReward":
                            if (Throttled(c, "stage", 600)) return true;
                            LuaClaimStage(c, stream, ArgInt(args, 1)); return true;
                        case "WhiteDayLua_GoRank": LuaRank(c, stream); return true;
                        case "WhiteDay2024Event_EnterFlowerTask":
                        case "SwanLake_EnterTemple":
                        case "WhiteDay2024NPC_GooseLake":
                            Msg(c, "Only when Princess Odette obtains true love will a miracle occur, opening the path to Swan Lake.");
                            return true;
                        case "WhiteDay2024NPC_OpenEmoneyShop":
                            Msg(c, "The romantic opera `Swan Lake Romance` is currently in hot performance. You can purchase the latest theater merchandise from Fluffy [TheaterShop] in Twin City (367,444)!");
                            return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return true;
            }
        }
        /// <summary>Client Lua numbers can arrive as floats ("1.0").</summary>
        static int ArgInt(string[] a, int i)
        {
            if (a == null || i >= a.Length) return 0;
            double v;
            return double.TryParse((a[i] ?? "").Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v) ? (int)v : 0;
        }

        /// <summary>10369 WhiteDayLua_OpenMain (the top icon / Agnes "Go to Dream Theater" / rank "back").</summary>
        public static void LuaOpenMain(Client.GameClient c)
        {
            OnLua(c, "WhiteDayLua_OpenMain", null);
        }
        /// <summary>The official first-use guide of the window (DlgCommonActivityGuideConfig[10369] 1 -> 8, video qiOK 1:24-1:34).</summary>
        static void ShowFirstGuide(Client.GameClient c)
        {
            var p = State(c);
            bool show; lock (_lock) show = !p.GuideDone;
            if (show) Lua(c, "PublicDlgOPenGuide|{['Code']=200,['Data']={['activity']=" + LuaMsg + ",['guide_id']=1}}");
        }

        // ------------------------------------------------------------------ main window data
        static int Dance(Client.GameClient c)
        {
            if (PerformanceReady(c)) return 2;
            return Finished(c, Cfg.UnlockQuest) ? 1 : 0;
        }
        static List<int> CanClaimRows(PState p)
        {
            var cfg = Cfg; var l = new List<int>();
            for (int i = 0; i < cfg.Milestones.Count && i < 8; i++)
                if (p.Points >= cfg.Milestones[i].Points && !p.Claimed.Contains(cfg.Milestones[i].Points)) l.Add(i + 1);
            if (ExtraDue(p) > 0) l.Add(9);
            return l;
        }
        /// <summary>Red dot of the Dream Theater button in the activity-menu container: a Theater Points reward can be claimed.</summary>
        public static bool HasClaimableReward(Client.GameClient c)
        {
            if (c == null || c.Player == null) return false;
            try { var p = State(c); lock (_lock) return CanClaimRows(p).Count > 0; }
            catch { return false; }
        }
        static string MainData(Client.GameClient c)
        {
            var cfg = Cfg;
            var p = State(c);
            int ti = TaskIndex(c), dance = Dance(c);
            int lv; ulong pts, funds; bool stage;
            lock (_lock) { lv = Math.Max(1, Math.Min(5, p.Level)); pts = p.Points; funds = p.Funds; stage = CanClaimRows(p).Count > 0; }
            uint gold = CountItem(c, cfg.GoldCoin), silver = CountItem(c, cfg.SilverCoin);
            bool up = lv < cfg.MaxLevel && funds >= cfg.LevelOf(lv).UpgradeFunds;
            int h = Now.Hour;
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['Type']=").Append(h >= 6 && h < 18 ? 1 : 2);
            sb.Append(",['RedPoint']=").Append(L(new[] { stage ? 1 : 0, up ? 1 : 0, gold + silver > 0 ? 1 : 0, dance == 2 ? 1 : 0 }));
            sb.Append(",['TaskIndex']=").Append(ti);
            sb.Append(",['Dance']=").Append(dance);
            sb.Append(",['Score']=").Append(pts);
            sb.Append(",['Lv']=").Append(lv);
            sb.Append(",['ItemNum']={").Append(gold).Append(',').Append(silver).Append('}');
            return sb.Append("}}").ToString();
        }
        static void SendMain(Client.GameClient c, bool open)
        {
            Lua(c, (open ? "DlgWhiteDayMain_OpenView|" : "DlgWhiteDayMain_UpdateView|") + MainData(c));
            var p = State(c); bool red; lock (_lock) red = CanClaimRows(p).Count > 0;
            Lua(c, "DlgWhiteDay_UpdateRedPoint|{['Code']=200,['Data']={['Rewardflag']=" + (red ? 1 : 0) + "}}");
        }
        /// <summary>Refreshes the window if the player has it open (quest progress / coins changed elsewhere).</summary>
        public static void RefreshWindow(Client.GameClient c)
        {
            try { if (c != null && c.Player != null && Running) SendMain(c, false); } catch { }
        }

        // ------------------------------------------------------------------ contracts (Agnes in the window)
        static void SolveContract(Client.GameClient c, int idx)
        {
            var cfg = Cfg;
            if (idx <= 0 || idx != TaskIndex(c)) { SendMain(c, false); return; }
            IEnumerable<uint> ids;
            if (idx >= 1 && idx <= 7) { var s = cfg.Segments.FirstOrDefault(x => x.Index == idx); ids = s == null ? new uint[0] : new[] { s.Start }; }
            else if (idx == 8) ids = cfg.CleanQuests;
            else ids = new uint[] { 5753, 5756, 5757, 5758, 5759, 5760, 5761 };
            string why = null, name = null; bool ok = false;
            foreach (uint id in ids)
            {
                string w;
                if (QuestEngine.QuestEngineCore.AcceptAtNpc(c, Agnes, id, out name, out w)) { ok = true; break; }
                if (why == null) why = w;
            }
            if (ok)
            {
                var p = State(c);
                lock (_lock) { p.ContractIdx = idx; p.ContractDone = false; p.Intro = true; _dirty = true; }
                SaveState();
                Msg(c, "[Quest] Quest accepted: " + name + ".");
                Log(c, "window contract " + idx + " -> " + name);
            }
            else Msg(c, "[Quest] " + (why ?? "You can't take this quest now."));
            SendMain(c, false);
        }

        // ------------------------------------------------------------------ upgrade
        static void OpenUpgrade(Client.GameClient c)
        {
            var cfg = Cfg; var p = State(c);
            int lv; ulong funds; lock (_lock) { lv = p.Level; funds = p.Funds; }
            if (lv >= cfg.MaxLevel) { Msg(c, "Dream Theater has reached its highest level."); return; }
            Lua(c, "DlgWhiteDayUpStart_OpenView|{['Code']=200,['Data']={['Lv']=" + Math.Max(1, Math.Min(4, lv)) + ",['Money']=" + funds + "}}");
        }
        static void LuaUpgrade(Client.GameClient c, ServerSockets.Packet stream)
        {
            var p = State(c);
            int before; lock (_lock) before = p.Level;
            string result = UpgradeCore(c, stream, false);
            int after; lock (_lock) after = p.Level;
            if (after > before)
            {
                Lua(c, "LuaUIMgr_HideDialog|DlgWhiteDayUpStart");
                Lua(c, "DlgWhiteDayUpFinish_OpenView|{['Code']=200,['Data']={['Lv']=" + Math.Min(4, before) + "}}");// the client shows Lv -> Lv+1 (video qiOK 4:49 'Lv1 -> Lv2')
                SendMain(c, false);
            }
            else Msg(c, result);
        }

        // ------------------------------------------------------------------ Enter Theater / performance
        static void LuaEnter(Client.GameClient c)
        {
            var cfg = Cfg; var p = State(c);
            if (!Finished(c, cfg.UnlockQuest) && !PerformanceReady(c)) { Msg(c, "The performance has not started today. You can go and talk to Agnes first!"); return; }
            if (PerformanceReady(c))
            {
                ulong pts; int lv; ulong funds;
                lock (_lock) { pts = PerfPoints(c, p); lv = Math.Min(5, p.Level); funds = cfg.LevelOf(p.Level).Funds; }
                ulong a = pts / 2, b = pts * 3 / 10, m = pts - a - b;// the official 50 / 30 / 20 split (all videos)
                Lua(c, "DlgWhiteDayActFinish_OpenView|{['Code']=200,['Data']={['Lv']=" + lv + ",['Rate']=" + a + ",['Fame']=" + b + ",['Develop']=" + m
                       + ",['Score']=" + pts + ",['Money']=" + funds + "}}");
                return;
            }
            Lua(c, "DlgWhiteDayMain_CloseAllView");
            Lua(c, "DlgWhiteDayMain_CloseView");
            EnterTheater(c);
        }
        static void LuaSettle(Client.GameClient c, ServerSockets.Packet stream)
        {
            string result = PerformCore(c, stream, false);// all the checks (contract done, once a day, bag space)
            Lua(c, "LuaUIMgr_HideDialog|DlgWhiteDayActFinish");
            int[] s;
            if (_lastSettle.TryRemove(c.Player.UID, out s))
                Lua(c, "DlgWhiteDayActFinishTwo_OpenView|{['Code']=200,['Data']={['NumMoney']=" + s[0] + ",['LvMoney']=" + s[1] + ",['PlayNum']=" + s[2] + "}}");
            else Msg(c, result);
            SendMain(c, false);
        }

        // ------------------------------------------------------------------ Wish Fountain
        static readonly ConcurrentDictionary<uint, Tuple<int, List<RItem>>> _pendingWish = new ConcurrentDictionary<uint, Tuple<int, List<RItem>>>();
        static void LuaWish(Client.GameClient c, ServerSockets.Packet stream, int idx)
        {
            if (idx != 1 && idx != 2) return;
            var p = State(c);
            ulong before; lock (_lock) before = p.Points;
            List<RItem> won;
            string result = WishCore(c, stream, idx == 2, false, out won);
            ulong after; lock (_lock) after = p.Points;
            if (after > before)
            {
                _pendingWish[c.Player.UID] = Tuple.Create((int)(after - before), won);
                Lua(c, "LuaUIMgr_HideDialog|DlgWhiteDayDrawTip");
                Lua(c, "DlgWhiteDayMain_PlayDrawEff|" + idx);
                SendMain(c, false);
            }
            else Msg(c, result);
        }
        /// <summary>End of the fountain effect: the official "You received" popup of that wish (videos qiOK 9:32, 10:48; tTO1 5:20).</summary>
        static void ShowWishReward(Client.GameClient c)
        {
            Tuple<int, List<RItem>> w;
            if (!_pendingWish.TryRemove(c.Player.UID, out w)) return;
            Lua(c, "DlgPublicReward_OpenView|{['Code']=200,['Data']={['ShowTip']=" + Q("Increased " + w.Item1 + " Theater Points for the Dream Theater.") + "}}");
            Lua(c, "DlgPublicReward_UpdateView|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", w.Item2.Select(x => Q(x.Id + " 0 " + x.Count))) + "},['IsFinish']=1}}");
        }

        // ------------------------------------------------------------------ Theater Points milestones (DlgMidAutumnRank)
        static string StageData(Client.GameClient c)
        {
            var cfg = Cfg; var p = State(c);
            var claimed = new List<int>(); var no = new List<int>(); List<int> can; ulong pts, target;
            lock (_lock)
            {
                for (int i = 0; i < cfg.Milestones.Count && i < 8; i++) (p.Claimed.Contains(cfg.Milestones[i].Points) ? claimed : no).Add(i + 1);
                if (cfg.ExtraItems.Count > 0 && cfg.Milestones.Count > 0) no.Add(9);
                can = CanClaimRows(p);
                pts = p.Points;
                var next = cfg.Milestones.FirstOrDefault(m => m.Points > pts);
                ulong top = cfg.Milestones.Count > 0 ? cfg.Milestones[cfg.Milestones.Count - 1].Points : 0;
                target = next != null ? next.Points : top + cfg.ExtraEvery * ((pts - top) / Math.Max(1UL, cfg.ExtraEvery) + 1);
            }
            return "{['Code']=200,['Data']={['ActivityId']=" + LuaMsg + ",['LastDialog']='DlgWhiteDayMain',['IsOpen']=1,['IsBack']=1,['IsTime']=1,['IsRule']=1"
                 + ",['Claimed']=" + L(claimed) + ",['NoClaim']=" + L(no) + ",['CanClaim']=" + L(can) + ",['RankNum']=3"
                 + ",['IsPoint']=1,['PointProgress']={" + pts + "," + Math.Max(1UL, target) + "}}}";
        }
        static void LuaClaimStage(Client.GameClient c, ServerSockets.Packet stream, int row)
        {
            if (row < 1 || row > 9) return;
            var cfg = Cfg;
            List<RItem> items = row == 9 ? cfg.ExtraItems : row - 1 < cfg.Milestones.Count ? cfg.Milestones[row - 1].Items : new List<RItem>();
            string result = ClaimMilestoneCore(c, stream, (byte)(row == 9 ? 198 : 190 + row - 1), false);// all the checks
            if (result.StartsWith("You received", StringComparison.Ordinal))
            {
                Lua(c, "DlgPublicReward_OpenView|{['Code']=200,['Data']={}}");
                Lua(c, "DlgPublicReward_UpdateView|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", items.Select(x => Q(x.Id + " 0 " + x.Count))) + "},['IsFinish']=1}}");
            }
            else Msg(c, result.StartsWith("Failed to claim", StringComparison.Ordinal) ? "Failed to claim rewards for lack of Theater Points. You can earn more Theater Points through [Upgrading the Theater] and [Daily Performances]!" : result);
            Lua(c, "DlgMidAutumnRank_UpdateReward|" + StageData(c));
            SendMain(c, false);
        }

        // ------------------------------------------------------------------ local ranking
        static void LuaRank(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cfg = Cfg; var p = State(c);
            var rank = Ranking();
            int mine = rank.FindIndex(x => x.Uid == p.Uid) + 1;
            var sb = new StringBuilder("Local Dream Ranking: ");
            if (rank.Count == 0) sb.Append("No one has achieved a ranking. ");
            for (int i = 0; i < rank.Count; i++) sb.Append("No." + (i + 1) + " " + rank[i].Name + " " + rank[i].Points + (i + 1 < rank.Count ? ", " : ". "));
            sb.Append("My Rank: " + (mine > 0 ? "No." + mine : "Unranked") + " (" + p.Points + "). You need at least " + cfg.RankMinPoints + " Theater Points to get listed on the local ranking. The ranking will be settled at 8:00 (US)/16:00 (EU), Mar. 25, and the rewards will be sent by mail.");
            Box(c, stream, sb.ToString());
        }
    }
}
