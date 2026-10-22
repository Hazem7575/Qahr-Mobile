// ============================================================================
//  [PuppetTheater 2026-10-14 / COMPLETE 2026-10-15] The official Global Halloween 2025 "Strings of Terror" windows
//  (client msg 10768 tMsg_Info["25Halloween"]). Shown while the festival PuppetTheater runs.
//
//  Official client (luaui, decrypted):
//    top bar   FGUI_ActivityMenu["150348"] "Strings of Terror" -> Halloween2025Kua_OpenMain</N>0 (msg 10768).
//    main      InterfaceID 15034801 Halloween25Main_OpenData: [1] {{SP},{Soulthread}}, [2] view, [3] day / [4] night
//              stages {unlock, exploration %, boss rating, look}, Points {cur,max}, OtherReds.
//              Halloween2025Kua_ClickRegion</N>map</N>idx: day 1-5 areas, 6 Underground Cave (Puppet Finale), 7 Puppet Home;
//              night 1 Underground Cave, 2-6 dark areas, 7 Puppet Home.
//    board     InterfaceID 10771003 Fui_Fgui_25Wsj_Zgz_Open: {grid[64]{id,state,event,eventState,show}, {pts,max,red},
//              {SP}, {Soulthread}, SP per step, level}; Halloween2025Exploration_ClickGrid</N>level</N>index.
//    draw      InterfaceID 15034901 Halloween25Draw_OpenData / Halloween25Draw_UpdateData / _ResultReturn /
//              _OpenSeletReward; Halloween2025Lottery_ClientLottery / ClientChoose / ClientChooseConfirm / ClientSkipLottery.
//    rewards   StageSybTemplate_OpenView</N>10768001..10768004 (Fui_EnsemblePointRank: points / ranking / level / explore)
//              answered with a server dialog (points milestones, local ranking).  150411001 belongs to JellyfishSkyCity.
//    guides    Halloween2025Npc_ShoppingGuide</N>1|2 -> the Puppet Store (Elise, official CP prices).
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10768), tagged.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;

namespace Qahr.Mobile.PuppetTheater
{
    public static class PuppetTheaterUi
    {
        public const uint LuaMsg = PT.LuaMsg;
        public const uint InterfaceMain = 15034801, InterfaceBoard = 10771003, InterfaceDraw = 15034901;
        public const string Festival = PT.Festival;
        public const string StageRewardsId = "10768001";
        public static bool Running { get { return PT.Running; } }

        /// <summary>LuaPush.HandleCallback (msg 10768). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || msgId != LuaMsg) return false;
            if (func.StartsWith("StageSybTemplate_", StringComparison.Ordinal))
            {
                if (args.Length == 0 || !args[0].Trim().StartsWith("10768", StringComparison.Ordinal)) return false;// e.g. 150411001 = White Day 2026
            }
            else if (!func.StartsWith("Halloween2025", StringComparison.Ordinal) && !func.StartsWith("Halloween25", StringComparison.Ordinal)) return false;
            if (Kit.Throttled(c.Player.UID, "ptlua", 300)) return true;
            if (!Running) { PT.Msg(c, PT.T("Npc[34646][Text121]")); return true; }
            if (c.Player.Level < PT.Cfg.MinLevel) { PT.Msg(c, PT.T("Exploration[Sys][LevelLimit]", "Please reach level 80 to proceed.")); return true; }
            int a0 = 0, a1 = 0;
            // [2026-10-15 live fix] the client sends Lua numbers as "1.0" (ClientChooseConfirm</N>2</N>1.0): parse as a number
            if (args.Length > 0) a0 = Num(args[0]);
            if (args.Length > 1) a1 = Num(args[1]);
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    lock (Kit.Lock(c.Player.UID))
                    {
                        switch (func)
                        {
                            case "Halloween2025Kua_OpenMain": PT.Daily(c, s); SendMain(c); return true;
                            case "Halloween2025Kua_ClickRegion": Region(c, s, a0, a1); return true;
                            case "Halloween2025Exploration_ClickGrid": PuppetMaze.Click(c, s, a0, a1); return true;
                            case "Halloween2025Npc_ShoppingGuide": PuppetNpcs.StoreMenu(c, s, false, a0); return true;
                            case "StageSybTemplate_OpenView": PuppetRewards.Open(c, a0); return true;
                            case "StageSybTemplate_StageReward": PuppetRewards.Claim(c, s, a0); return true;
                            case "StageSybTemplate_ViewRank": PuppetNpcs.Stage(c, s, PuppetRewards.TabRank); return true;
                            case "StageSybTemplate_BackClick": SendMain(c); return true;
                            case "Halloween2025Lottery_ClientLottery": PuppetHome.Train(c, s); return true;
                            case "Halloween2025Lottery_ClientChoose": PuppetHome.Choose(c, a0); return true;
                            case "Halloween2025Lottery_ClientChooseConfirm": PuppetHome.Confirm(c, a0, a1); return true;
                            case "Halloween2025Lottery_ClientSkipLottery": PT.St.Set(c.Player.UID, "skip", PT.Get(c.Player.UID, "skip") == 1 ? 0 : 1); return true;
                            case "Halloween2025Lottery_ClientCongratulation": PuppetHome.Congratulation(c); return true;
                            case "Halloween2025Lottery_StartGuide": return true;
                            case "Halloween2025Kua_ClientGuide": return true;
                            case "Halloween2025Npc_GoPuppetHome": PuppetHome.Open(c); return true;   // loss box Confirm ("Go to enhance your puppet?")
                            case "Halloween2025Exploration_Event": PuppetEvents.Event(c, a0); return true;            // event box Confirm / Cancel
                            case "Halloween2025Exploration_StoryNext": PuppetEvents.StoryNext(c, a0); return true;    // story guide closed (ExitFunc)
                            case "Halloween2025Exploration_StoryGo": ShowSub(c, "story"); PuppetMaze.Story(c, a0, true); return true;   // the event windows close before the story map (e2e 16:49)      // story box Confirm
                            case "Halloween2025Exploration_Again": PuppetMaze.AgainPick(c, s, a0, a1); return true;    // cleared box: 1 explore again / 2 challenge boss
                            case "Halloween2025Npc_LinkNpc": PuppetFinale.Info(c, s); return true;     // guide 150348 step 3 (Puppet Finale, "3-1")                    // guide Unlock step closed (official ClientFunc)
                        }
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            PT.Msg(c, "This part of Strings of Terror isn`t open on this server.");
            return true;
        }

        public static int Num(string v)
        {
            int i; if (int.TryParse((v ?? "").Trim(), out i)) return i;
            double d; return double.TryParse((v ?? "").Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d) && d > int.MinValue && d < int.MaxValue ? (int)Math.Round(d) : 0;
        }
        static void Region(Client.GameClient c, ServerSockets.Packet s, int map, int idx)
        {
            bool night = map == 4 || map == 2;
            if (idx == 7) { PuppetHome.Open(c); return; }
            if ((!night && idx == 6) || (night && idx == 1)) { PuppetFinale.Info(c, s); return; }
            int L = night ? idx + 4 : idx;
            if (L < 1 || L > 10 || (night && L < 6) || (!night && L > 5)) return;
            if (!PuppetMaze.Unlocked(c.Player.UID, L)) { PT.Msg(c, PT.Fmt("Clear %s first to unlock this area.", PT.Cfg.Levels[L - 1] != null ? PT.Cfg.Levels[L - 1].Name : "the previous area")); return; }
            PuppetMaze.Send(c, L);
            // [2026-10-15 e2e] no box on open: official shows the cleared box only when an explored cell is clicked
        }

        // ------------------------------------------------------------------ main window
        // [2026-10-15] official flow = ONE view at a time: opening the board / Puppet Home closes the main window
        // (interfacecfg 15034803 Cmd_Halloween25Main_Close), the board's Return asks Halloween2025Kua_OpenMain and the
        // server closes the board (g_function Fui_Fgui_25Wsj_Zgz_Close); the draw closes itself on Return.
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, string> View = new System.Collections.Concurrent.ConcurrentDictionary<uint, string>();
        public const string CloseMain = "InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=15034803,['Data']={}}";
        public const string CloseBoard = "Fui_Fgui_25Wsj_Zgz_Close|";
        public static void ShowSub(Client.GameClient c, string view)
        {
            string cur;
            if (View.TryGetValue(c.Player.UID, out cur) && cur == "main") Kit.Lua(c, CloseMain, LuaMsg);
            if (cur == "board" && view != "board") Kit.Lua(c, CloseBoard, LuaMsg);
            View[c.Player.UID] = view;
        }
        public static void SendMain(Client.GameClient c)
        {
            string cur;
            if (View.TryGetValue(c.Player.UID, out cur) && cur == "board") Kit.Lua(c, CloseBoard, LuaMsg);
            View[c.Player.UID] = "main";
            Kit.Lua(c, MainData(c), LuaMsg);
            PT.MainGuide(c);
        }
        /// <summary>[2026-10-15 fix] After SP / Soulthreads change (item use, store, daily): updates the open windows in place.</summary>
        public static void Refresh(Client.GameClient c)
        {
            try
            {
                string view; View.TryGetValue(c.Player.UID, out view);
                if (view == "main")
                {
                    string md = MainData(c);
                    int i = md.IndexOf("['Data']=", StringComparison.Ordinal);
                    if (i > 0) Kit.Lua(c, "Halloween25Main_UpdateData|{['Code']=200," + md.Substring(i), LuaMsg);
                }
                int L;
                if (view == "board" && PuppetMaze.LastBoard.TryGetValue(c.Player.UID, out L) && PuppetMaze.Lvl(L) != null)
                    Kit.Lua(c, PuppetMaze.Board(c, L, false).Replace("['InterfaceID']=" + InterfaceBoard, "['InterfaceID']=" + (InterfaceBoard + 1)), LuaMsg);
                if (view == "draw") PuppetHome.Update(c);
            }
            catch { }
        }
        // [2026-10-15 owner screenshots + halloween25mainmediator UpdateMapState] stage = {unlocked, explore %, boss rating, boss model look}.
        // data[4] is the region's boss model (initMonsterViewByLua, scale 0.3; 490 = 0.6), shown on locked areas too.
        // Looks = the Puppet Home soul models (halloween25drawmediator t_look {246,493,104,487,490}): Gilt Fox 104 on
        // Puppet Garden (L2), Abyss Nixie 487 on Midnight Stage (L4), Stone Lion 490 on Puppet Garden (Dark) (L7), Blaze Phoenix
        // 493 on Midnight Stage (Dark) (L9); Papet 246 on Puppet Home. The model leaves once the area's soul is restored (story done).
        public static readonly int[] RegionLook = { 0, 0, 104, 0, 487, 0, 0, 490, 0, 493, 0 };
        public const int HomeLook = 246;
        static string Stage(Client.GameClient c, int L)
        {
            uint uid = c.Player.UID;
            int r = (int)PT.Get(uid, "r" + L);
            int look = L >= 1 && L <= 10 ? RegionLook[L] : 0;
            if (look != 0) { var l = PuppetMaze.Lvl(L); if (l != null && l.Story.Length > 0 && FestivalStory.StoryEntry.NextOf(c, l.Story) == 0) look = 0; }
            return "{" + (PuppetMaze.Unlocked(uid, L) ? 1 : 0) + "," + PuppetMaze.Pct(uid, L) + "," + (r == 3 ? "'S'" : r == 2 ? "'A'" : r == 1 ? "'B'" : "nil") + "," + (look != 0 ? look.ToString() : "nil") + "}";
        }
        /// <summary>The current area (highest unlocked, not cleared): ['Light']={{map 3 day / 4 night, button index}, red}.</summary>
        static string Light(uint uid)
        {
            int cur = 1;
            for (int L = 1; L <= 10; L++) if (PuppetMaze.Unlocked(uid, L)) cur = L;
            return "['Light']={{" + (cur <= 5 ? 3 : 4) + "," + (cur <= 5 ? cur : cur - 4) + "},0},";
        }
        /// <summary>interfacecfg 15034804 Cmd_Halloween25Main_UnlockNewLv: the unlocked area flashes (lua2024qqyrj7), then the
        /// client itself opens guide 150348 "Unlock" (Papet bust: "Congratulations! You've unlocked a new area...").</summary>
        public static void UnlockFlash(Client.GameClient c, int L)
        {
            Kit.Lua(c, "InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=15034804,['Data']={['Unlock']={" + (L <= 5 ? 3 : 4) + "," + (L <= 5 ? L : L - 4) + "}}}", LuaMsg);
        }
        public static string MainData(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            string fin = "{" + (PuppetHome.TotalLevel(uid) >= PT.Cfg.FinaleMinPuppet ? 1 : 0) + ",0,nil,nil}";
            string home = "{1,0,nil," + HomeLook + "}";
            long pts = PT.Get(uid, "pts");
            bool red = PuppetRewards.AnyClaim(uid);
            var sb = new StringBuilder("InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=").Append(InterfaceMain).Append(",['Data']={");
            sb.Append("{{").Append(PT.SybSP).Append(',').Append(PT.Count(c, PT.SP)).Append("},{").Append(PT.SybSoul).Append(',').Append(PT.Count(c, PT.Soulthread)).Append("}},");
            sb.Append(PuppetMaze.Cleared(uid, 5) ? 1 : 0).Append(',');
            sb.Append('{'); for (int L = 1; L <= 5; L++) sb.Append(Stage(c, L)).Append(','); sb.Append(fin).Append(',').Append(home).Append("},");
            sb.Append('{').Append(fin).Append(','); for (int L = 6; L <= 10; L++) sb.Append(Stage(c, L)).Append(','); sb.Append(home).Append("},");
            sb.Append(Light(uid));
            sb.Append("['Points']={").Append(pts).Append(',').Append(PT.NextMilestone(pts)).Append("},['OtherReds']={").Append(red ? 1 : 0).Append(",0}");
            return sb.Append("}}").ToString();
        }

        /// <summary>Kept for older tests: the empty starting window.</summary>
        public static string OpenData(int sp, int soulthread)
        {
            var sb = new StringBuilder("InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=").Append(InterfaceMain).Append(",['Data']={");
            sb.Append("{{").Append(PT.SybSP).Append(',').Append(Math.Max(0, sp)).Append("},{").Append(PT.SybSoul).Append(',').Append(Math.Max(0, soulthread)).Append("}},0,");
            sb.Append("{{1,0},{0,0},{0,0},{0,0},{0,0},{0,0},{1,0}},{{0,0},{0,0},{0,0},{0,0},{0,0},{0,0},{1,0}},['Points']={0,1},['OtherReds']={0,0}");
            return sb.Append("}}").ToString();
        }
    }

    // ====================================================================== the exploration board
    public static class PuppetMaze
    {
        public static PtLevel Lvl(int L) { return L >= 1 && L <= 10 ? PT.Cfg.Levels[L] : null; }
        /// <summary>The unfinished challenge of area L (null = none): "boss" = its boss copy is still open, "story" = a story quest is running.</summary>
        public static string Pending(Client.GameClient c, int L)
        {
            var l = Lvl(L); if (l == null || !Cleared(c.Player.UID, L)) return null;
            var cp = Arena.Of(c.Player.UID);
            if (cp != null && cp.Tag == "PTBoss" + L && cp.Armed) return "boss";
            foreach (var q in l.Story) if (FestivalStory.StoryEntry.Active(c, q)) return "story";
            if (PT.Get(c.Player.UID, "r" + L) == 0) return "boss";       // cleared but the boss was never beaten (also after a restart)
            return null;
        }
        public static bool Cleared(uint uid, int L) { return PT.Get(uid, "c" + L) == 1; }
        public static bool Unlocked(uint uid, int L) { return L == 1 || (L >= 2 && L <= 10 && Cleared(uid, L - 1)); }
        public static int Pct(uint uid, int L) { return (int)Math.Min(100, PT.Get(uid, "p" + L)); }
        static int StartCell(PtLevel l) { for (int i = 0; i < 64; i++) if (l.Cells[i] == 'S') return i; return 0; }
        public static ulong Mask(uint uid, PtLevel l)
        {
            ulong m; if (!ulong.TryParse(PT.St.Get(uid, "m" + l.No), out m) || m == 0) m = 1UL << StartCell(l);
            return m;
        }
        static ulong Done(uint uid, int L) { ulong m; ulong.TryParse(PT.St.Get(uid, "d" + L), out m); return m; }
        static bool Has(ulong m, int i) { return (m & (1UL << i)) != 0; }

        /// <summary>Neighbours on the client's hex board (Fgui_25Wsj_ZgzMediator:SetGridNext).</summary>
        public static IEnumerable<int> Near(int i)
        {
            int x = i % 8 + 1, y = i / 8 + 1;
            var l = new List<int[]> { new[] { x - 1, y }, new[] { x + 1, y } };
            if (y % 2 == 1) { l.Add(new[] { x, y - 1 }); l.Add(new[] { x + 1, y - 1 }); l.Add(new[] { x, y + 1 }); l.Add(new[] { x + 1, y + 1 }); }
            else { l.Add(new[] { x - 1, y - 1 }); l.Add(new[] { x, y - 1 }); l.Add(new[] { x - 1, y + 1 }); l.Add(new[] { x, y + 1 }); }
            foreach (var p in l) if (p[0] >= 1 && p[0] <= 8 && p[1] >= 1 && p[1] <= 8) yield return (p[1] - 1) * 8 + p[0] - 1;
        }
        static bool Special(char k) { return k != '.' && k != 'o' && k != 'S'; }
        static int CountOpen(PtLevel l, ulong m, char k) { int n = 0; for (int i = 0; i < 64; i++) if (l.Cells[i] == k && Has(m, i)) n++; return n; }
        static int CountAll(PtLevel l, char k) { return l.Cells.Count(x => x == k); }
        /// <summary>null = the gate is open; otherwise the official tip of the lock.</summary>
        static string GateLock(PtLevel l, ulong m, char k)
        {
            switch (k)
            {
                case 'y': return CountOpen(l, m, 'Y') >= CountAll(l, 'Y') ? null : "Exploration[UpfloatTips][102]";
                case 'g': return CountOpen(l, m, 'G') >= CountAll(l, 'G') ? null : "Exploration[UpfloatTips][102]";
                case 'b': return CountOpen(l, m, 'B') >= CountAll(l, 'B') ? null : "Exploration[UpfloatTips][104]";
                case 'r': return CountOpen(l, m, 'R') >= CountAll(l, 'R') ? null : "Exploration[UpfloatTips][104]";
            }
            return null;
        }

        public static string Board(Client.GameClient c, int L, bool open)
        {
            var l = Lvl(L); uint uid = c.Player.UID; ulong m = Mask(uid, l);
            var sb = new StringBuilder("InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=").Append(PuppetTheaterUi.InterfaceBoard).Append(",['Data']={{");
            for (int i = 0; i < 64; i++)
            {
                if (i > 0) sb.Append(',');
                char k = l.Cells[i];
                if (k == '.') sb.Append('{').Append(i + 1).Append(",2,0,0,0}");
                else if (Has(m, i)) sb.Append('{').Append(i + 1).Append(",1,").Append(l.Event[i]).Append(",1,0}");
                // official (video 01:12 / 04:30, checked with an inject): a reachable event cell is REVEALED (state 1, eventState 0)
                // and shows its portrait / chest + name; an unrevealed cell is a grey "?" (state 0) and shows nothing
                else if (k == 'E') sb.Append('{').Append(i + 1).Append(",1,").Append(l.Event[i] != 0 ? l.Event[i] : 16).Append(",0,0}");   // official: the End is shown from the start (video 00:58)
                else if (l.Event[i] != 0 && Near(i).Any(n => Has(m, n))) sb.Append('{').Append(i + 1).Append(",1,").Append(l.Event[i]).Append(",0,0}");
                else sb.Append('{').Append(i + 1).Append(",0,0,0,0}");
            }
            long pts = PT.Get(uid, "pts");
            sb.Append("},{").Append(pts).Append(',').Append(PT.NextMilestone(pts)).Append(",0},{").Append(PT.SybSP).Append(',').Append(PT.Count(c, PT.SP)).Append("},{")
              .Append(PT.SybSoul).Append(',').Append(PT.Count(c, PT.Soulthread)).Append("},").Append(PT.Cfg.SpPerStep).Append(',').Append(L).Append("}}");
            return sb.ToString();
        }
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, int> LastBoard = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();
        public static void Send(Client.GameClient c, int L)
        {
            if (Lvl(L) == null) return; Board2(c, L);
            PT.Guide(c, PT.LuaMsg, "ZgzUi_Guide_1");   // official first-open board guide (FguiGuideInfo[10768], video 00:58)
        }
        static void Board2(Client.GameClient c, int L) { LastBoard[c.Player.UID] = L; PuppetTheaterUi.ShowSub(c, "board"); Kit.Lua(c, Board(c, L, true), PT.LuaMsg); }

        /// <summary>One step on the board (all checks server-side).</summary>
        public static string Click(Client.GameClient c, ServerSockets.Packet s, int L, int index)
        {
            var cfg = PT.Cfg; var l = Lvl(L); uint uid = c.Player.UID;
            if (l == null || index < 1 || index > 64) return "bad";
            if (!Unlocked(uid, L)) return "locked";
            int i = index - 1; char k = l.Cells[i];
            ulong m = Mask(uid, l);
            if (k == '.') { PtUi.Tip(c, PT.T("Exploration[UpfloatTips][101]")); return "void"; }
            if (Has(m, i))
            {
                if (k == 'N') { PuppetEvents.StoryCell(c, L, l.Extra[i]); return "story"; }      // the story can be read again
                // an explored cell of a cleared area: the official "cleared" choice (explore again / challenge boss)
                if (k == 'E' || Cleared(uid, L)) { Again(c, s, L); return "again"; }
                return "open";
            }
            if (!Near(i).Any(n => Has(m, n))) { PtUi.Tip(c, PT.T("Exploration[UpfloatTips][101]")); Board2(c, L); return "far"; }
            string lk = GateLock(l, m, k);
            if (lk != null) { PtUi.Tip(c, PT.T(lk)); Board2(c, L); return "gate"; }
            if (PT.Count(c, PT.SP) < cfg.SpPerStep)
            {
                PtUi.MsgBox(c, PT.T("Exploration[ClientTips][NoPower]"), "", "Halloween2025Npc_ShoppingGuide</N>1", "", "");
                return "nosp";
            }
            if (!Kit.HasSpace(c, 2)) { PtUi.Tip(c, "Please make 2 free slots in your inventory first."); return "space"; }
            if (!Kit.Take(c, PT.SP, (uint)cfg.SpPerStep, s)) return "nosp";
            m |= 1UL << i;
            ulong done = Done(uid, L); bool first = !Has(done, i);
            PT.St.Set(uid, Store.KV("m" + L, m), Store.KV("d" + L, done | (1UL << i)));
            string res = "step";
            switch (k)
            {
                case 'o':
                    if (Kit.Next(1, 100) <= cfg.PlainSoulChance)
                    {
                        PT.Give(c, s, "maze L" + L + " cell " + index, new Reward(PT.Soulthread, 1));
                        PtUi.Tip(c, PT.T("Exploration[UpfloatTips][" + Kit.Next(1, 7) + "]"));
                        res = "soul";
                    }
                    break;
                case 'C': case 'X': case 'A':
                    // official (video 04:22): the "Notice" box; its Confirm pays the reward with the "You received" popup
                    PuppetEvents.Ask(c, L, index, k, l.Extra[i], first);
                    res = "event";
                    break;
                case 'N': PuppetEvents.StoryCell(c, L, l.Extra[i]); res = "story"; break;
                case 'Y': case 'G': PtUi.Tip(c, PT.T(CountOpen(l, m, k) >= CountAll(l, k) ? "Exploration[UpfloatTips][103]" : "Exploration[UpfloatTips][102]")); break;
                case 'B': case 'R': PtUi.Tip(c, PT.T("Exploration[UpfloatTips][105]")); break;
                case 'E':
                    {
                        bool was = Cleared(uid, L);
                        PT.St.Set(uid, "c" + L, 1);
                        PT.Log(c, "CLEAR", "L" + L);
                        if (L >= 2 && L <= 5 && !was) PT.Msg(c, PT.T("Lottery[MsgBox][Reset][" + L + "]") + "a new puppet soul in Puppet Home.");
                        res = "end";
                        break;
                    }
            }
            // exploration: every event cell opened (chests, encounters, artifacts, gates, the end)
            int all = 0, open = 0;
            for (int j = 0; j < 64; j++) if (Special(l.Cells[j])) { all++; if (Has(m, j)) open++; }
            int pct = all == 0 ? 100 : open * 100 / all;
            if (pct > Pct(uid, L)) PT.St.Set(uid, "p" + L, pct);
            Board2(c, L);
            // official (video 01:50): reaching the End starts the boss battle at once; later visits get the cleared box
            if (res == "end") { if (PT.Get(uid, "r" + L) == 0) Boss(c, s, L); else Again(c, s, L); }
            return res;
        }

        static string Pay(Client.GameClient c, ServerSockets.Packet s, int L, int index, bool first, int souls)
        {
            if (!first || souls <= 0) return "nothing";
            var r = new Reward(PT.Soulthread, (uint)souls);
            PT.Give(c, s, "maze L" + L + " event " + index, r);
            return souls + " Soulthread" + (souls > 1 ? "s" : "");
        }
        static string PayItems(Client.GameClient c, ServerSockets.Packet s, int L, int index, bool first, List<uint[]> items)
        {
            if (!first || items == null || items.Count == 0) return "nothing";
            var r = items.Select(x => new Reward(x[0], x[1], true)).ToArray();
            if (!PT.Space(c, r)) return "nothing (bag full)";
            return PT.Give(c, s, "maze L" + L + " encounter " + index, r);
        }
        static void Story(Client.GameClient c, ServerSockets.Packet s, int L, string a, string b)
        {
            var d = PT.Dialog(c, s, PT.DlgMaze, (cc, ss, o) => { });
            PT.Say(d, a + "\n"); PT.Say(d, b);
            d.AddOption("OK.", 255).FinalizeDialog();
        }
        static void Gbjq(Client.GameClient c, ServerSockets.Packet s, int L, int n)
        {
            string name = PT.T("CN:FUI_GBJQ_TXT_25QQWS_ZGZ_" + n + "_1", PT.T("CN:FUI_GBJQ_TXT_25QQWS_ZGZ_" + n + "_2", ""));
            string sn = StoryName(c, L, true);
            var d = PT.Dialog(c, s, PT.DlgMaze, (cc, ss, o) => { if (o == 1) Story(cc, L, true); });
            var sb = new StringBuilder(name.Length > 0 ? name + ":\n" : "");
            for (int j = 1; j <= 8; j++) { string t = PT.T("CN:FUI_GBJQ_TXT_DESC_25QQWS_ZGZ_" + n + "_" + j); if (t.Length > 0) sb.Append((j % 2 == 0 ? "You: " : "") + t + "\n"); }
            PT.Say(d, sb.ToString());
            // official (video 04:46): the story cell starts the area's story quest on the area map
            if (sn != null) d.AddOption("Follow the clue. (" + sn + ")", 1);
            d.AddOption("...", 255).FinalizeDialog();
        }

        /// <summary>Official "Again" box after the area is cleared (Exploration[ClientTips][Again]): Confirm = explore again, Cancel = challenge boss.</summary>
        public static void Again(Client.GameClient c, ServerSockets.Packet s, int L)
        {
            if (Lvl(L) == null) return;
            PtUi.MsgBox(c, PT.T("Exploration[ClientTips][Again][Content]", "You have cleared this area. Choose to explore again or challenge the boss directly."),
                PT.T("Exploration[ClientTips][Again][Option1]", "Explore again"), "Halloween2025Exploration_Again</N>" + L + "</N>1",
                PT.T("Exploration[ClientTips][Again][Option2]", "Challenge boss"), "Halloween2025Exploration_Again</N>" + L + "</N>2");
        }
        public static string AgainPick(Client.GameClient c, ServerSockets.Packet s, int L, int o)
        {
            if (Lvl(L) == null || !Cleared(c.Player.UID, L)) return "locked";
            if (o == 1) { PT.St.Set(c.Player.UID, "m" + L, (long)0); Board2(c, L); return "reset"; }
            if (o == 2) return Boss(c, s, L);
            return "none";
        }

        static bool NpcCellOpen(uint uid, int L)
        {
            var l = Lvl(L); if (l == null) return false; ulong m = Mask(uid, l);
            for (int i = 0; i < 64; i++) if (l.Cells[i] == 'N' && Has(m, i)) return true;
            return Cleared(uid, L);
        }
        /// <summary>The next official story quest of area L (null = none / all done).</summary>
        public static string StoryName(Client.GameClient c, int L, bool fromCell = false)
        {
            var l = Lvl(L); if (l == null || l.Story.Length == 0) return null;
            if (!fromCell && !Cleared(c.Player.UID, L)) return null;
            uint q = FestivalStory.StoryEntry.NextOf(c, l.Story); if (q == 0) return null;
            QuestEngine.EngineQuest eq; var t = QuestEngine.QuestEngineCore.Current;
            return t != null && t.Quests.TryGetValue(q, out eq) && !string.IsNullOrEmpty(eq.Name) ? eq.Name : "quest " + q;
        }
        /// <summary>The area's official story quests (9186-9201): accept / resume in the private copy of the area map.</summary>
        public static string Story(Client.GameClient c, int L, bool fromCell = false)
        {
            var l = Lvl(L); if (l == null || l.Story.Length == 0) return "nostory";
            if (!fromCell && !Cleared(c.Player.UID, L)) return "nostory";
            if (fromCell && !NpcCellOpen(c.Player.UID, L)) return "nostory";
            return FestivalStory.StoryEntry.Enter(c, l.Story, PT.Cfg.MinLevel, "Please go to Twin City to continue the story.", "You have finished this story.");
        }
        /// <summary>Challenge boss: the area's story quests first (official 9186-9201), then a private boss copy.</summary>
        public static string Boss(Client.GameClient c, ServerSockets.Packet s, int L)
        {
            return PuppetBattle.Boss(c, L);   // [2026-10-15 video] the official in-window puppet battle
        }
        static string BossCopyOld(Client.GameClient c, ServerSockets.Packet s, int L)
        {
            var cfg = PT.Cfg; var l = Lvl(L); uint uid = c.Player.UID;
            if (l == null || !Cleared(uid, L)) return "locked";
            if (!PT.InTown(c)) { PT.Msg(c, "Please go to Twin City or the Market to challenge the boss."); return "town"; }
            // [2026-10-15] Challenge boss = ALWAYS the area's boss copy; the story quests have their own option (Again dialog)
            if (l.BossFamily == 0 || l.BossMap == 0) return "noboss";
            string tag = "PTBoss" + L;
            var old = Arena.Of(uid);
            if (old != null && old.Tag == tag && old.Armed && Arena.Alive(old) > 0)
            {
                // [re-entry] the same copy, the same boss (HP and timer kept): no new boss, no second reward
                if (!Arena.Goto(c, old, l.BossX, (ushort)Math.Max(1, l.BossY - 5))) { PT.Msg(c, "This place isn`t open yet."); return "nomap"; }
                PT.Msg(c, PT.Fmt("You are back in the boss fight of %s.", l.Name));
                return "reenter";
            }
            var cp = Arena.Enter(c, l.BossMap, l.BossX, (ushort)Math.Max(1, l.BossY - 5), tag, cfg.BossMinutes, PT.TwinCity, PT.ExitX, PT.ExitY);
            if (cp == null) { PT.Msg(c, "This place isn`t open yet."); return "nomap"; }
            Arena.ClearMobs(cp);
            var mob = Arena.Spawn(cp, l.BossFamily, l.BossX, l.BossY, 2, s);
            if (mob == null) { Arena.Close(cp, true); PT.Msg(c, "The boss isn`t ready yet. Please try again later."); return "nomob"; }
            cp.Data = DateTime.Now;
            Arena.Arm(cp, (cc, cpp) => BossCleared(cc, L, cpp));
            PT.Msg(c, PT.Fmt("Defeat the boss of %s! (S-Rating within %s seconds)", l.Name, cfg.RateS));
            PT.Log(c, "BOSS", "L" + L + " start");
            return "boss";
        }
        static void BossCleared(Client.GameClient c, int L, Copy cp)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            double secs = cp.Data is DateTime ? (DateTime.Now - (DateTime)cp.Data).TotalSeconds : 9999;
            int rate = secs <= cfg.RateS ? 3 : secs <= cfg.RateA ? 2 : 1;
            lock (Kit.Lock(uid))
            {
                if (rate > PT.Get(uid, "r" + L)) PT.St.Set(uid, "r" + L, rate);
                string extra = "";
                if (PT.Get(uid, "bf" + L) == 0 && cfg.BossFirst.Count > 0)
                {
                    var r = cfg.BossFirst.Select(x => new Reward(x[0], x[1], true)).ToArray();
                    if (PT.Space(c, r))
                    {
                        PT.St.Set(uid, "bf" + L, 1);
                        using (var rec = new ServerSockets.RecycledPacket()) extra = " You obtained " + PT.Give(c, rec.GetStream(), "boss first L" + L, r) + ".";
                    }
                    else extra = " Make room in your inventory and defeat it again for the first-clear reward.";
                }
                PT.Log(c, "BOSS", "L" + L + " cleared " + (int)secs + "s rating " + rate);
                PT.Msg(c, PT.Fmt("Boss defeated in %s seconds. Boss Rating: %s.", (int)secs, rate == 3 ? "S" : rate == 2 ? "A" : "B") + extra);
            }
            Arena.Close(cp, true);
        }
    }
}
