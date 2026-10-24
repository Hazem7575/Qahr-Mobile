// ============================================================================
//  [ChildrensDay2025 2026-10-14 / COMPLETE 2026-10-15] The official 2025 Global Children's Day "Dream Park"
//  (client msg 10664 tMsg_Info["ChildrenDay"], 全球儿童节活动 5.27-6.9), shown while the festival ChildrensDay2025 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150288"], clickfunc Children2025Npc_OpenMainView on msg 10664 (festival.json icon_*).
//    window    Fui_ChildrenDay_Open(Data) -> fairygui\window\childrenday:
//              Data[1] {points, max, red}; Data[2] {{sysid,count},{sysid,count}} (luauirewardshow ids!);
//              Data[3] {{q,events,red} x3 (Dessert Shop, Ferris Wheel, Carousel), {red,cur,total} Dream Quest, {red} Vault Palace, {storeType,red}};
//              Data[4] {state 0 wait/1 claim/2 closed, nextTs, {{sysid,num},..}} operation reward.
//              Fui_ChildrenDay_GetBuildingInfo {{id,q,exp,max},{cost x1},{cost x10},{revenue},{next revenue},{renovation reward},nextQ,upFx}
//              Fui_ChildrenDay_UpBuilding {{id,oldQ,newQ},{before},{after}}; Fui_ChildrenDay_SpecialEvent {{b,evt},{1,{cost},{rw,min,max}},{2,{rw,n}}}
//    rank      fairygui\window\childermas25rank: Fgui_Childermas25Rank_PointData / _AchieveData / _RankRewardData (same msg 10664).
//  Rules (official Cn_Res STR_FUI_CHILD_TXT_33): 1 Starwish Stone per renovation or decision = 50 Dream Points; 1 operation per day;
//  better quality = better operation reward; VIP Dream Store at 15000 points; ranking from 20000 points.
//  Numbers that are not in the client: DreamParkConfig (derived, Data\ChildrensDay2025\childrensday2025.json).
//
//  Security: uid from client.Player.UID only; building/decision/count values are whitelisted; every action re-checks the
//  festival, the level, the bag (space and Starwish Stones), takes the stones BEFORE granting, saves the state under one lock;
//  daily/once claims are recorded before the reward; 400 ms per-uid throttle.
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10664), tagged [ChildrensDay2025 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Cfg_ = Qahr.Mobile.ChildrensDay2025.DreamParkConfig;

namespace Qahr.Mobile.ChildrensDay2025
{
    public static class ChildrensDay2025Ui
    {
        public const uint LuaMsg = 10664;
        public const uint StarwishStone = Cfg_.StarwishStone;

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg_.Cfg.Festival); } catch { return false; } } }
        public static int Run { get { try { var r = Festivals.FestivalManager.Running(Cfg_.Cfg.Festival); return r == null ? 0 : r.Run; } catch { return 0; } } }

        // official texts (Server_Key tChildren2025Npc / tChildrenDay2025Part5 / tChildrenDayNpc2025Park, Cn_Res STR_FUI_CHILD_*)
        public const string TxtOver = "Dream Park has finally concluded perfectly. Looking forward to our next collaboration!";
        public const string TxtLevel = "Please upgrade to level 80 first.";
        const string TxtNoItem = "You don`t have enough Starwish Stones. View acquisition methods?";

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        public static bool NoThrottle;// harness
        static bool Throttled(uint uid)
        {
            if (NoThrottle) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static string Call(string func, string data) { return func + "|{['Code']=200,['Data']=" + data + "}"; }

        static int Arg(string[] args, int i)
        {
            int v;
            return args != null && args.Length > i && int.TryParse(args[i], out v) ? v : -1;
        }

        static DreamPlayer State(Client.GameClient c) { return DreamParkState.Get(c.Player.UID, c.Player.Name, Run); }

        /// <summary>LuaPush.HandleCallback (msg 10664). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool ours = func.StartsWith("Children2025Npc_", StringComparison.Ordinal)
                     || func.StartsWith("BuildIncome_", StringComparison.Ordinal)
                     || func.StartsWith("ChildrenDay2025Part5_", StringComparison.Ordinal);
            if (!ours) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            var cfg = Cfg_.Cfg;
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    lock (DreamParkState.Sync)
                    {
                        switch (func)
                        {
                            case "Children2025Npc_OpenMainView": Lua(c, OpenCall(c, cfg)); return true;
                            case "Children2025Npc_OpenStarWay": Lua(c, "Fui_ChildrenDay_GuidBuy"); return true;
                            case "Children2025Npc_BuildingClick": BuildingClick(c, cfg, Arg(args, 0)); return true;
                            case "Children2025Npc_BuildingAddExp": Renovate(c, cfg, stream, Arg(args, 0), Arg(args, 1)); return true;
                            case "Children2025Npc_StarWay": StarWay(c, cfg, Arg(args, 0)); return true;
                            case "BuildIncome_GetOperateReward": Operate(c, cfg, stream); return true;
                            case "BuildIncome_SuddenEventView": EventView(c, cfg, Arg(args, 0)); return true;
                            case "BuildIncome_SuddenEvent": EventDecide(c, cfg, stream, Arg(args, 0), Arg(args, 1), Arg(args, 2)); return true;
                            case "ChildrenDay2025Part5_StageViewUpdate": Lua(c, PointCall(c, cfg)); return true;
                            case "ChildrenDay2025Part5_StageReward": ClaimStages(c, cfg, stream); return true;
                            case "ChildrenDay2025Part5_OpenStagePage": Lua(c, AchieveCall(c, cfg)); return true;
                            case "ChildrenDay2025Part5_ReceiveStageReward": ClaimAchievements(c, cfg, stream); return true;
                            case "ChildrenDay2025Part5_RankData": RankRewards(c, Arg(args, 0)); return true;
                            case "ChildrenDay2025Part5_OpenRankView": RankList(c, cfg, Arg(args, 0)); return true;
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            Msg(c, "This part of Dream Park isn`t open on this server yet.");
            return true;
        }

        // ------------------------------------------------------------------ main window
        static long NextMidnightUnix()
        {
            var t = DreamParkState.Now.Date.AddDays(1);
            return new DateTimeOffset(t).ToUnixTimeSeconds();
        }
        static uint RevenueMg(Cfg_ cfg, DreamPlayer p)
        {
            uint mg = 0; for (int b = 1; b <= 3; b++) mg += cfg.Revenue(cfg.QualityOf(p.BPts[b]));
            return mg;
        }
        static string Revenue(Cfg_ cfg, DreamPlayer p)
        {
            var sb = new StringBuilder("{");
            sb.Append('{').Append(Cfg_.SysMoonglass).Append(',').Append(RevenueMg(cfg, p)).Append('}');
            if (cfg.OperationStarwish > 0) sb.Append(",{").Append(Cfg_.SysStarwish).Append(',').Append(cfg.OperationStarwish).Append('}');
            return sb.Append('}').ToString();
        }

        /// <summary>The main window (Fui_ChildrenDay_Open) with this player's real state.</summary>
        public static string OpenCall(Client.GameClient c, Cfg_ cfg)
        {
            DreamPlayer p;
            lock (DreamParkState.Sync) p = State(c);
            uint stones = DreamParkItems.Count(c, Cfg_.StarwishStone), mg = DreamParkItems.Count(c, Cfg_.Moonglass);
            var sb = new StringBuilder("{");
            sb.Append('{').Append(p.Points).Append(',').Append(cfg.VipPoints).Append(",0},");
            sb.Append("{{").Append(Cfg_.SysStarwish).Append(',').Append(stones).Append("},{").Append(Cfg_.SysMoonglass).Append(',').Append(mg).Append("}},");
            sb.Append('{');
            for (int b = 1; b <= 3; b++)
                sb.Append('{').Append(cfg.QualityOf(p.BPts[b])).Append(',').Append(p.Events[b].Count).Append(',').Append(stones > 0 && p.BPts[b] < cfg.MaxStructurePts ? 1 : 0).Append("},");
            sb.Append("{0,0,3},{").Append(p.BlessDone ? 0 : 1).Append("},{").Append(p.Points >= cfg.VipPoints ? 1 : 0).Append(",0}},");
            bool canOp = p.OpDay != DreamParkState.Today;
            sb.Append('{').Append(canOp ? 1 : 0).Append(',').Append(canOp ? 0 : NextMidnightUnix()).Append(',').Append(Revenue(cfg, p)).Append('}');
            sb.Append('}');
            return Call("Fui_ChildrenDay_Open", sb.ToString());
        }

        public static string BuildingCall(Client.GameClient c, Cfg_ cfg, int b, bool upFx)
        {
            var p = State(c);
            uint pts = p.BPts[b], q = cfg.QualityOf(pts), nq = Math.Min(7u, q + 1);
            uint max = cfg.NextQualityPts(q), shown = Math.Min(pts, max);
            var sb = new StringBuilder("{");
            sb.Append('{').Append(b).Append(',').Append(q).Append(',').Append(shown).Append(',').Append(max).Append("},");
            sb.Append('{').Append(Cfg_.SysStarwish).Append(",1},{").Append(Cfg_.SysStarwish).Append(",10},");
            sb.Append("{{").Append(Cfg_.SysMoonglass).Append(',').Append(cfg.Revenue(q)).Append("}},");
            sb.Append("{{").Append(Cfg_.SysMoonglass).Append(',').Append(cfg.Revenue(nq)).Append("}},");
            sb.Append('{');
            if (cfg.RenovatePack) sb.Append('{').Append(Cfg_.SysPack[b]).Append(",1},");
            sb.Append('{').Append(Cfg_.SysMoonglass).Append(',').Append(cfg.RenovateMoonglass).Append("}},");
            sb.Append(nq).Append(',').Append(upFx ? 1 : 0).Append('}');
            return Call("Fui_ChildrenDay_GetBuildingInfo", sb.ToString());
        }

        static readonly HashSet<uint> VaultFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036 };
        static void BuildingClick(Client.GameClient c, Cfg_ cfg, int b)
        {
            if (b >= 1 && b <= 3) { Lua(c, BuildingCall(c, cfg, b, false)); return; }
            if (b == 4) { Msg(c, "Dream Quests take place inside Dream Park, and the Dream Park map isn`t in this client, so they can`t be entered on this server."); return; }
            if (b == 5) { GoVault(c, cfg); return; }
            if (b == 6)
            {
                var p = State(c);
                Msg(c, (p.Points >= cfg.VipPoints ? "VIP Dream Store" : "Dream Store") + ": talk to Suyi in Twin City (" + cfg.SuyiX + "," + cfg.SuyiY + "). Your Dream Points: " + p.Points + "/" + cfg.VipPoints + ".");
            }
        }

        public static void GoVault(Client.GameClient c, Cfg_ cfg)
        {
            if (!VaultFrom.Contains(c.Player.Map) || c.Player.DynamicID != 0 || !c.Player.Alive) { Msg(c, c.Player.Map == cfg.VaultMap ? "You are already in Vault Palace." : "Please go to Twin City first."); return; }
            var p = State(c);
            if (p.BlessDone) { Msg(c, "You`ve reached today`s limit for Vault Palace and Starlight Blessing."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, cfg.VaultX, cfg.VaultY, cfg.VaultMap, 0); else c.Teleport(cfg.VaultX, cfg.VaultY, cfg.VaultMap, 0);
            DreamParkState.Log(c, "VAULT enter");
        }

        static void StarWay(Client.GameClient c, Cfg_ cfg, int w)
        {
            switch (w)
            {
                case 1: Msg(c, "Dream Quests take place inside Dream Park, which isn`t in this client, so they can`t be done on this server."); break;
                case 2: GoVault(c, cfg); break;
                case 3: Msg(c, "Prince Puff in Twin City sells Starwish Packs (3-100 Starwish Stones) for 100 CPs."); break;
                case 4: Msg(c, "Anima Exchange: Suyi`s Dream Store in Twin City trades 1 P9/P10/P11 Anima for a Small/Medium/Grand Starwish Box (42/85/175 Starwish Stones)."); break;
            }
        }

        // ------------------------------------------------------------------ renovation
        static void Renovate(Client.GameClient c, Cfg_ cfg, ServerSockets.Packet stream, int b, int times)
        {
            if (b < 1 || b > 3 || (times != 1 && times != 10)) return;
            uint n = (uint)times;
            if (DreamParkItems.Count(c, Cfg_.StarwishStone) < n) { Msg(c, TxtNoItem); Lua(c, "Fui_ChildrenDay_GuidBuy"); return; }
            var gives = new List<DreamParkItems.Grant>();
            gives.Add(new DreamParkItems.Grant(Cfg_.Moonglass, cfg.RenovateMoonglass * n));
            if (cfg.RenovatePack) gives.Add(new DreamParkItems.Grant(Cfg_.PackItem[b], n));
            int slots = DreamParkItems.Slots(gives);
            if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first!"); return; }
            var p = State(c);
            uint oldQ = cfg.QualityOf(p.BPts[b]);
            if (!DreamParkItems.Take(c, Cfg_.StarwishStone, n, stream)) return;
            p.Points += cfg.PointsPerStone * n;
            p.BPts[b] += cfg.PointsPerStone * n;
            for (int i = 0; i < times; i++)
                if (p.Events[b].Count < cfg.MaxEventsPerBuilding && DreamParkItems.Rand(100) < cfg.EventChancePct) p.Events[b].Add(1 + DreamParkItems.Rand(3));
            p.Updated = DateTime.UtcNow.Ticks;
            DreamParkState.Save();
            string got = DreamParkItems.GiveAll(c, gives, stream);
            uint newQ = cfg.QualityOf(p.BPts[b]);
            DreamParkState.Log(c, "RENOVATE b" + b + " x" + n + " pts=" + p.Points + " bpts=" + p.BPts[b] + " q" + oldQ + "->" + newQ + " events=" + p.Events[b].Count + " got " + got);
            if (newQ != oldQ)
            {
                string before = "{{" + Cfg_.SysMoonglass + "," + cfg.Revenue(oldQ) + "}}", after = "{{" + Cfg_.SysMoonglass + "," + cfg.Revenue(newQ) + "}}";
                Lua(c, Call("Fui_ChildrenDay_UpBuilding", "{{" + b + "," + oldQ + "," + newQ + "}," + before + "," + after + "}"));
            }
            Lua(c, BuildingCall(c, cfg, b, newQ != oldQ));
            Lua(c, OpenCall(c, cfg));
        }

        // ------------------------------------------------------------------ daily operation
        static void Operate(Client.GameClient c, Cfg_ cfg, ServerSockets.Packet stream)
        {
            var p = State(c);
            if (p.OpDay == DreamParkState.Today) { Msg(c, "You`ve already operated the park today. Come back tomorrow!"); Lua(c, OpenCall(c, cfg)); return; }
            var gives = new List<DreamParkItems.Grant> { new DreamParkItems.Grant(Cfg_.Moonglass, RevenueMg(cfg, p)), new DreamParkItems.Grant(Cfg_.StarwishStone, cfg.OperationStarwish) };
            int slots = DreamParkItems.Slots(gives);
            if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first!"); return; }
            p.OpDay = DreamParkState.Today;// claim recorded before the reward
            DreamParkState.Save();
            string got = DreamParkItems.GiveAll(c, gives, stream);
            DreamParkState.Log(c, "OPERATE " + got);
            Msg(c, "Park operation complete! You received " + got + ".");
            Lua(c, OpenCall(c, cfg));
        }

        // ------------------------------------------------------------------ sudden events
        static void EventView(Client.GameClient c, Cfg_ cfg, int b)
        {
            if (b < 1 || b > 3) return;
            var p = State(c);
            if (p.Events[b].Count == 0) { Lua(c, "Fui_ChildrenDay_CloseEvtUI"); return; }
            int e = p.Events[b][0];
            Lua(c, Call("Fui_ChildrenDay_SpecialEvent", "{{" + b + "," + e + "},{1,{" + Cfg_.SysStarwish + "," + cfg.EventCost + "},{" + Cfg_.SysMoonglass + "," + cfg.EventMinMg + "," + cfg.EventMaxMg + "}},{2,{" + Cfg_.SysMoonglass + "," + cfg.EventSafeMg + "}}}"));
        }

        static void EventDecide(Client.GameClient c, Cfg_ cfg, ServerSockets.Packet stream, int b, int costType, int evt)
        {
            if (b < 1 || b > 3 || (costType != 1 && costType != 2) || evt < 1 || evt > 3) return;
            var p = State(c);
            int idx = p.Events[b].IndexOf(evt);
            if (idx < 0) { Lua(c, "Fui_ChildrenDay_CloseEvtUI"); Lua(c, OpenCall(c, cfg)); return; }
            uint mg = costType == 1 ? cfg.EventMinMg + (uint)DreamParkItems.Rand((int)(cfg.EventMaxMg - cfg.EventMinMg + 1)) : cfg.EventSafeMg;
            var gives = new List<DreamParkItems.Grant> { new DreamParkItems.Grant(Cfg_.Moonglass, mg) };
            int slots = DreamParkItems.Slots(gives);
            if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first!"); return; }
            if (costType == 1)
            {
                if (DreamParkItems.Count(c, Cfg_.StarwishStone) < cfg.EventCost) { Msg(c, TxtNoItem); Lua(c, "Fui_ChildrenDay_GuidBuy"); return; }
                if (!DreamParkItems.Take(c, Cfg_.StarwishStone, cfg.EventCost, stream)) return;
                p.Points += cfg.PointsPerStone * cfg.EventCost;// official: a decision with a Starwish Stone = 50 Dream Points
            }
            p.Events[b].RemoveAt(idx);
            p.Updated = DateTime.UtcNow.Ticks;
            DreamParkState.Save();
            string got = DreamParkItems.GiveAll(c, gives, stream);
            DreamParkState.Log(c, "EVENT b" + b + " e" + evt + " d" + costType + " -> " + got + " pts=" + p.Points);
            Msg(c, "Congratulations! You obtained " + got + "!");
            Lua(c, "Fui_ChildrenDay_CloseEvtUI");
            Lua(c, OpenCall(c, cfg));
        }

        // ------------------------------------------------------------------ points / structure rewards (rank window)
        static string It(uint id, uint n) { return "'" + id + " 0 " + n + "'"; }
        /// <summary>Derived stage rewards (event items only). Index 0 = the repeating 1000-point stage.</summary>
        public static DreamParkItems.Grant[] StageReward(int stage)
        {
            switch (stage)
            {
                case 0: return new[] { new DreamParkItems.Grant(Cfg_.Moonglass, 10) };
                case 1: return new[] { new DreamParkItems.Grant(Cfg_.DessertShopPack, 1), new DreamParkItems.Grant(Cfg_.Moonglass, 50) };
                case 2: return new[] { new DreamParkItems.Grant(Cfg_.FerrisWheelPack, 1), new DreamParkItems.Grant(Cfg_.Moonglass, 100) };
                case 3: return new[] { new DreamParkItems.Grant(Cfg_.CarouselPack, 1), new DreamParkItems.Grant(Cfg_.Moonglass, 200) };
                case 4: return new[] { new DreamParkItems.Grant(Cfg_.DreamMaterialPack, 2), new DreamParkItems.Grant(Cfg_.Moonglass, 300) };
                case 5: return new[] { new DreamParkItems.Grant(Cfg_.ConcertoFragment, 5) };
                case 6: return new[] { new DreamParkItems.Grant(Cfg_.SweetFragment, 10) };
                case 7: return new[] { new DreamParkItems.Grant(Cfg_.SweetDreamBoxB, 1) };
            }
            return new DreamParkItems.Grant[0];
        }
        static string List(DreamParkItems.Grant[] g) { return "{" + string.Join(",", g.Select(x => It(x.Id, x.Count))) + "}"; }
        static uint RepeatDue(Cfg_ cfg, DreamPlayer p) { return Math.Min(cfg.StageRepeatMax, p.Points / cfg.StageRepeatPts); }

        public static string PointCall(Client.GameClient c, Cfg_ cfg)
        {
            var p = State(c);
            var rows = new List<string>();
            uint due = RepeatDue(cfg, p);
            rows.Add("{['Stage']=0," + List(StageReward(0)) + "," + (due > p.RepeatClaimed ? 1 : 0) + "," + p.RepeatClaimed + "}");
            for (int s = 1; s <= cfg.StagePts.Length; s++)
            {
                int st = (p.StageMask & (1u << s)) != 0 ? 2 : p.Points >= cfg.StagePts[s - 1] ? 1 : 0;
                rows.Add("{['Stage']=" + s + "," + List(StageReward(s)) + "," + st + "}");
            }
            return Call("Fgui_Childermas25Rank_PointData", "{" + string.Join(",", rows) + "}");
        }

        static void ClaimStages(Client.GameClient c, Cfg_ cfg, ServerSockets.Packet stream)
        {
            var p = State(c);
            var gives = new List<DreamParkItems.Grant>(); uint newMask = p.StageMask; uint due = RepeatDue(cfg, p), rep = 0;
            if (due > p.RepeatClaimed) { rep = due - p.RepeatClaimed; foreach (var g in StageReward(0)) gives.Add(new DreamParkItems.Grant(g.Id, g.Count * rep, g.Bound)); }
            for (int s = 1; s <= cfg.StagePts.Length; s++)
                if ((p.StageMask & (1u << s)) == 0 && p.Points >= cfg.StagePts[s - 1]) { newMask |= 1u << s; gives.AddRange(StageReward(s)); }
            if (gives.Count == 0) { Msg(c, "No point reward is ready yet."); return; }
            int slots = DreamParkItems.Slots(gives);
            if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first!"); return; }
            p.StageMask = newMask; p.RepeatClaimed += rep;
            DreamParkState.Save();
            string got = DreamParkItems.GiveAll(c, gives, stream);
            DreamParkState.Log(c, "STAGES mask=" + newMask + " rep+" + rep + " -> " + got);
            Msg(c, "Congratulations! You obtained " + got + "!");
            Lua(c, PointCall(c, cfg));
        }

        // structure achievements (official texts STR_LUAUI_105290_ACHIEVE_1..4; derived rewards)
        public static DreamParkItems.Grant[] AchReward(int a)
        {
            if (a == 1) return new[] { new DreamParkItems.Grant(Cfg_.SweetFragment, 10) };
            return new[] { new DreamParkItems.Grant(Cfg_.PackItem[a - 1], 3) };
        }
        static bool AchDone(Cfg_ cfg, DreamPlayer p, int a)
        {
            if (a == 1) return p.BPts[1] >= cfg.MaxStructurePts && p.BPts[2] >= cfg.MaxStructurePts && p.BPts[3] >= cfg.MaxStructurePts;
            return p.BPts[a - 1] >= cfg.MaxStructurePts;// a 2..4 = Dessert Shop, Ferris Wheel, Carousel
        }
        public static string AchieveCall(Client.GameClient c, Cfg_ cfg)
        {
            var p = State(c);
            var rows = new List<string>();
            for (int a = 1; a <= 4; a++)
            {
                int st = (p.AchMask & (1u << a)) != 0 ? 2 : AchDone(cfg, p, a) ? 1 : 0;
                rows.Add("{{" + a + "," + cfg.MaxStructurePts + "}," + st + "," + List(AchReward(a)) + "}");
            }
            return Call("Fgui_Childermas25Rank_AchieveData", "{" + string.Join(",", rows) + "}");
        }
        static void ClaimAchievements(Client.GameClient c, Cfg_ cfg, ServerSockets.Packet stream)
        {
            var p = State(c);
            var gives = new List<DreamParkItems.Grant>(); uint mask = p.AchMask;
            for (int a = 1; a <= 4; a++) if ((mask & (1u << a)) == 0 && AchDone(cfg, p, a)) { mask |= 1u << a; gives.AddRange(AchReward(a)); }
            if (gives.Count == 0) { Msg(c, "No structure reward is ready yet."); return; }
            int slots = DreamParkItems.Slots(gives);
            if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) first!"); return; }
            p.AchMask = mask;
            DreamParkState.Save();
            string got = DreamParkItems.GiveAll(c, gives, stream);
            DreamParkState.Log(c, "ACHIEVE mask=" + mask + " -> " + got);
            Msg(c, "Congratulations! You obtained " + got + "!");
            Lua(c, AchieveCall(c, cfg));
        }

        // ranking: type 1 = Global (STR_LUAUI_105290_RANKTITLE_0, cross-server), type 2 = Local
        static void RankRewards(Client.GameClient c, int type)
        {
            if (type != 1 && type != 2) return;
            uint b = type == 1 ? Cfg_.GlobalBox1 : Cfg_.LocalBox1;
            string data = "{" + type + ",{{" + It(b, 1) + "},{" + It(b + 1, 1) + "},{" + It(b + 2, 1) + "},{" + It(b + 3, 1) + "}}}";
            Lua(c, Call("Fgui_Childermas25Rank_RankRewardData", data));
        }
        static void RankList(Client.GameClient c, Cfg_ cfg, int type)
        {
            if (type == 1) { Msg(c, "The global Dream Ranking is cross-server and isn`t open on this server. Check the local ranking."); return; }
            var top = DreamParkState.Ranking(Run, cfg.RankPoints, 10);
            var p = State(c);
            Msg(c, "Local Dream Ranking (at least " + cfg.RankPoints + " Dream Points). Your points: " + p.Points + ".");
            if (top.Count == 0) Msg(c, "Nobody has reached " + cfg.RankPoints + " Dream Points yet.");
            for (int i = 0; i < top.Count; i++) Msg(c, "No." + (i + 1) + "  " + top[i].Name + "  " + top[i].Points);
        }
    }
}
