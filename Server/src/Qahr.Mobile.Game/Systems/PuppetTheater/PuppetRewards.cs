// ============================================================================
//  [PuppetTheater 2026-10-15] The official rewards window (video 07:59-08:38): EnsemblePointRank (window 150355) with the
//  4 tabs of tMsg 10768 (ensemblepointrankdata CONST_DATA[10768]): 10768001 Points, 10768002 Ranking, 10768003 Level,
//  10768004 Exploration.  Opened with InfiniteReceiverData InterfaceID=<tab> Data={Flag,PageData={ActID,PointData|RankData},Red}.
//  Rows (cmp_point): {index, {reward sybs}, state -1 claimed / 0 not yet / 1 claimable, {claimed, 999} repeat, {numbers}, {}}.
//  Claim button: StageSybTemplate_StageReward</N><tab> (claims every claimable row of the tab). Ranking "View rankings":
//  GoCallFunc -> the local list.  Reward sybs / items: luauirewardshow 279001-298001 (official), config.json rewardSyb.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.PuppetTheater
{
    public static class PuppetRewards
    {
        public const int TabPoints = 10768001, TabRank = 10768002, TabLevel = 10768003, TabExplore = 10768004;
        static Dictionary<string, object> Raw { get { return PT.Cfg.Raw; } }
        static List<object> Arr(string k) { return FL.Arr(FL.Val(Raw, k)) ?? new List<object>(); }
        static uint U(object o) { return FL.U(o); }

        /// <summary>syb -> {item, count, minutes, bound}</summary>
        public static uint[] Syb(uint syb)
        {
            var m = FL.Obj(FL.Val(Raw, "rewardSyb")); var a = m == null ? null : FL.Arr(FL.Val(m, syb.ToString()));
            return a == null ? null : a.Select(FL.U).ToArray();
        }
        static Reward[] Rewards(IEnumerable<uint> sybs)
        {
            var list = new List<Reward>();
            foreach (var s in sybs) { var r = Syb(s); if (r == null || r.Length < 2) continue; list.Add(r.Length > 2 && r[2] > 0 ? Reward.Timed(r[0], r[1], true, r[2]) : new Reward(r[0], r[1], r.Length < 4 || r[3] == 1)); }
            return list.ToArray();
        }

        // ------------------------------------------------------------------ rows: {sybs, state, repeat{a,b}, numbers}
        sealed class Row { public uint[] Sybs; public int State; public string Repeat = "{}"; public string Nums = "{}"; public string Key; public int Times; }
        static List<Row> Rows(uint uid, int tab)
        {
            var rows = new List<Row>();
            var cfg = PT.Cfg; long pts = PT.Get(uid, "pts");
            if (tab == TabPoints)
            {
                uint every = Math.Max(1u, cfg.MilestoneEvery[0]);
                long ready = pts / every - PT.Get(uid, "tbx");
                rows.Add(new Row { Sybs = new uint[] { 298001000 }, State = ready > 0 ? 1 : 0, Repeat = "{" + PT.Get(uid, "tbx") + ",999}", Nums = "{" + every + "}", Key = "every", Times = (int)Math.Max(0, Math.Min(40, ready)) });
                var done = PT.St.GetList(uid, "mc");
                int i = 0;
                foreach (var o in Arr("pointRows"))
                {
                    var r = FL.Arr(o); if (r == null || r.Count < 2) { i++; continue; }
                    uint need = U(r[0]); var sy = (FL.Arr(r[1]) ?? new List<object>()).Select(U).ToArray();
                    rows.Add(new Row { Sybs = sy, State = done.Contains(i) ? -1 : pts >= need ? 1 : 0, Nums = "{" + need + "}", Key = "mc:" + i });
                    i++;
                }
            }
            else if (tab == TabLevel)
            {
                var done = PT.St.GetList(uid, "lr");
                int i = 0;
                foreach (var o in Arr("levelRows"))
                {
                    var r = FL.Arr(o); if (r == null || r.Count < 3) { i++; continue; }
                    int k = (int)U(r[0]); uint need = U(r[1]);
                    rows.Add(new Row { Sybs = new[] { U(r[2]) }, State = done.Contains(1000 + i) ? -1 : PuppetHome.Level(uid, k) >= need ? 1 : 0, Nums = "{" + need + "}", Key = "lr:" + i });
                    i++;
                }
                var rep = FL.Arr(FL.Val(Raw, "levelRepeat"));
                if (rep != null && rep.Count >= 3)
                {
                    uint from = U(rep[0]), step = Math.Max(1u, U(rep[1]));
                    long can = 0; for (int k = 1; k <= 5; k++) { int lvl = PuppetHome.Level(uid, k); if (lvl > from) can += (lvl - from) / step; }
                    long got = PT.Get(uid, "lrx");
                    rows.Add(new Row { Sybs = new[] { U(rep[2]) }, State = can > got ? 1 : 0, Repeat = "{" + got + ",999}", Nums = "{" + from + "," + step + "}", Key = "lrx", Times = (int)Math.Max(0, Math.Min(40, can - got)) });
                }
            }
            else if (tab == TabExplore)
            {
                var done = PT.St.GetList(uid, "xr");
                int clear = 0, full = 0, s = 0, art = Artifacts(uid);
                for (int L = 1; L <= 10; L++) { if (PuppetMaze.Cleared(uid, L)) clear++; if (PuppetMaze.Pct(uid, L) >= 100) full++; if (PT.Get(uid, "r" + L) == 3) s++; }
                int i = 0;
                foreach (var o in Arr("exploreRows"))
                {
                    var r = FL.Arr(o); if (r == null || r.Count < 3) { i++; continue; }
                    string kind = (r[0] ?? "").ToString(); uint need = U(r[1]);
                    int have = kind == "clear" ? clear : kind == "full" ? full : kind == "srank" ? s : art;
                    rows.Add(new Row { Sybs = new[] { U(r[2]) }, State = done.Contains(i) ? -1 : have >= need ? 1 : 0, Nums = "{" + need + "}", Key = "xr:" + i });
                    i++;
                }
            }
            return rows;
        }
        /// <summary>Soul artifacts collected (artifact cells ever opened).</summary>
        public static int Artifacts(uint uid)
        {
            int n = 0;
            for (int L = 1; L <= 10; L++)
            {
                var l = PuppetMaze.Lvl(L); if (l == null) continue;
                ulong d; ulong.TryParse(PT.St.Get(uid, "d" + L), out d);
                for (int i = 0; i < 64; i++) if (l.Cells[i] == 'A' && (d & (1UL << i)) != 0) n++;
            }
            return n;
        }
        public static bool AnyClaim(uint uid)
        {
            return Rows(uid, TabPoints).Any(r => r.State == 1) || Rows(uid, TabLevel).Any(r => r.State == 1) || Rows(uid, TabExplore).Any(r => r.State == 1);
        }

        public static void Open(Client.GameClient c, int tab)
        {
            uint uid = c.Player.UID;
            if (tab != TabPoints && tab != TabRank && tab != TabLevel && tab != TabExplore) tab = TabPoints;
            var sb = new StringBuilder("InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=").Append(tab).Append(",['Data']={['Flag']=0,['PageData']={['ActID']=").Append(tab).Append(',');
            if (tab == TabRank)
            {
                sb.Append("['RankData']={");
                var show = FL.Obj(FL.Val(Raw, "rankShow"));
                for (int k = 1; k <= 3; k++)
                {
                    sb.Append('{');
                    var tiers = show == null ? null : FL.Arr(FL.Val(show, k.ToString()));
                    int t = 0;
                    foreach (var o in tiers ?? new List<object>())
                    {
                        var sy = (FL.Arr(o) ?? new List<object>()).Select(U).ToList();
                        sb.Append("{{").Append(k).Append(',').Append(k == 3 ? 1 : 0).Append('}');
                        foreach (var y in sy) sb.Append(",{").Append(y).Append(",0}");
                        sb.Append('}'); if (++t < tiers.Count) sb.Append(',');
                    }
                    sb.Append('}'); if (k < 3) sb.Append(',');
                }
                sb.Append("},['GoCallFunc']={10768,'StageSybTemplate_ViewRank'}");
            }
            else
            {
                sb.Append("['PointData']={");
                var rows = Rows(uid, tab);
                for (int i = 0; i < rows.Count; i++)
                {
                    var r = rows[i];
                    sb.Append('{').Append(i + 1).Append(",{").Append(string.Join(",", r.Sybs)).Append("},").Append(r.State).Append(',').Append(r.Repeat).Append(',').Append(r.Nums).Append(",{}}");
                    if (i < rows.Count - 1) sb.Append(',');
                }
                sb.Append('}');
            }
            sb.Append("},['Red']={").Append(Rows(uid, TabPoints).Any(x => x.State == 1) ? 1 : 0).Append(",0,").Append(Rows(uid, TabLevel).Any(x => x.State == 1) ? 1 : 0).Append(',')
              .Append(Rows(uid, TabExplore).Any(x => x.State == 1) ? 1 : 0).Append("}}}");
            PuppetTheaterUi.ShowSub(c, "rewards");
            Kit.Lua(c, sb.ToString(), PT.LuaMsg);
        }

        /// <summary>StageSybTemplate_StageReward</N>tab: claims every claimable row of the tab (bag space first, recorded before giving).</summary>
        public static string Claim(Client.GameClient c, ServerSockets.Packet s, int tab)
        {
            uint uid = c.Player.UID;
            if (tab == TabRank) return ClaimRank(c, s);
            var rows = Rows(uid, tab).Where(r => r.State == 1).ToList();
            if (rows.Count == 0) { PT.Msg(c, "There is no reward to claim yet."); return "none"; }
            var all = new List<Reward>();
            foreach (var r in rows)
            {
                var rw = Rewards(r.Sybs);
                int times = Math.Max(1, r.Times);
                foreach (var x in rw) all.Add(x.Minutes > 0 ? Reward.Timed(x.Item, x.Count * (uint)times, x.Bound, x.Minutes) : new Reward(x.Item, x.Count * (uint)times, x.Bound));
            }
            if (!Kit.HasSpace(c, Kit.Slots(all))) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            foreach (var r in rows)
            {
                if (r.Key == "every") PT.St.Set(uid, "tbx", PT.Get(uid, "tbx") + Math.Max(1, r.Times));
                else if (r.Key == "lrx") PT.St.Set(uid, "lrx", PT.Get(uid, "lrx") + Math.Max(1, r.Times));
                else
                {
                    string k = r.Key.Split(':')[0]; int idx = int.Parse(r.Key.Split(':')[1]);
                    var l = PT.St.GetList(uid, k); int v = k == "lr" ? 1000 + idx : idx;
                    if (l.Contains(v)) return "claimed";
                    l.Add(v); PT.St.Set(uid, k, string.Join(",", l));
                }
            }
            PT.Msg(c, "You obtained " + PT.Give(c, s, "rewards tab " + tab, all.ToArray()) + ".");
            Open(c, tab);
            PuppetTheaterUi.Refresh(c);
            return "claimed " + rows.Count;
        }

        /// <summary>Local ranking reward: from the last event day (rankClaimFrom), No.1-10 with >= rankMin points, once.</summary>
        public static string ClaimRank(Client.GameClient c, ServerSockets.Packet s)
        {
            uint uid = c.Player.UID;
            string from = (FL.Val(Raw, "rankClaimFrom") ?? "11-17").ToString();
            if (string.CompareOrdinal(DateTime.Now.ToString("MM-dd"), from) < 0) { PT.Msg(c, "Ranking rewards can be claimed when the event ends (" + from + ")."); return "early"; }
            long pts = PT.Get(uid, "pts");
            if (pts < (long)FL.U(FL.Val(Raw, "rankMin"))) { PT.Msg(c, "Local Ranking Entry: " + FL.U(FL.Val(Raw, "rankMin")) + " pts."); return "min"; }
            if (PT.Get(uid, "rankPaid") == 1) { PT.Msg(c, "You have already claimed this reward."); return "claimed"; }
            int pos = PT.Ranking().FindIndex(r => r.Item1 == uid) + 1;
            uint syb = 0;
            foreach (var o in Arr("rankTiers")) { var t = FL.Arr(o); if (t != null && t.Count >= 3 && pos >= U(t[0]) && pos <= U(t[1])) syb = U(t[2]); }
            if (pos <= 0 || syb == 0) { PT.Msg(c, "You are not in the top 10 of the local ranking."); return "rank"; }
            var rw = Rewards(new[] { syb });
            if (!PT.Space(c, rw)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            PT.St.Set(uid, "rankPaid", 1);
            PuppetSeer.RankPaid(c);   // Ziwei Halloween Crystal (local equivalent of the Global ranking)
            PT.Msg(c, PT.Fmt(PT.T("Kua[Mail][Content]", "You ranked No.%s on the local ranking. Please claim your rewards."), pos) + " " + PT.Give(c, s, "rank " + pos, rw));
            return "rank " + pos;
        }

        // ------------------------------------------------------------------ the boxes / medals of these rewards
        public static bool UseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet s)
        {
            uint id = item.ITEM_ID;
            var box = FL.Obj(FL.Val(Raw, "puppeteerBox"));
            var rows = box == null ? null : FL.Arr(FL.Val(box, id.ToString()));
            if (rows != null) return Open(c, s, id, rows.Select(o => (FL.Arr(o) ?? new List<object>()).Select(U).ToArray()).Where(x => x.Length >= 2)
                .Select(x => x[0] == 3322813 ? Reward.Timed(x[0], x[1], true, 10080) : new Reward(x[0], x[1], true)).ToArray());
            long donation = id == 3322762 || id == 3315629 || id == 3350414 ? 10000000L : id == 3322813 ? 100000000L : id == 3322760 ? 3000000L : id == 3322761 ? 6000000L : 0;
            if (donation > 0) return Medal(c, s, item, donation);
            switch (id)
            {
                case 3346822: return Open(c, s, id, new[] { Reward.Timed(3322760, 1, true, 10080) });   // gift: 1 7-day 3KK Contribution Medal (B)
                case 3346823: return Open(c, s, id, new[] { Reward.Timed(3322761, 1, true, 10080) });   // 6KK
                case 3346824: return Open(c, s, id, new[] { Reward.Timed(3322762, 1, true, 10080) });   // 10KK
            }
            return false;
        }
        static bool Open(Client.GameClient c, ServerSockets.Packet s, uint id, Reward[] r)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return true; }
                if (!c.Inventory.Remove(id, 1, s)) return true;
                PT.Msg(c, "You obtained " + PT.Give(c, s, "open " + id, r) + ".");
            }
            return true;
        }
        /// <summary>Contribution Medal (official tip): "Use it to increase your Nobility Contribution by N points."</summary>
        static bool Medal(Client.GameClient c, ServerSockets.Packet s, Game.MsgServer.MsgGameItem item, long amount)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (c.Player.Nobility == null) { PT.Msg(c, "Nobility isn`t ready yet. Please try again."); return true; }
                if (!c.Inventory.Remove(item.ITEM_ID, 1, s)) return true;
                c.Player.Nobility.Donation += (ulong)amount;
                try { c.Send(s.NobilityIconCreate(c.Player.Nobility)); Pool.NobilityRanking.UpdateRank(c.Player.Nobility); } catch { }
                PT.Log(c, "GRANT", "use " + item.ITEM_ID + " -> " + amount + " Nobility Contribution");
                PT.Msg(c, "Your Nobility Contribution increased by " + amount.ToString("N0") + ".");
            }
            return true;
        }
    }
}
