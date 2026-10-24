// ============================================================================
//  [Summer2023 2026-10-15] The playable part of the Dream Cruise window (Fgui 150438, msg 10250 "Summer23"):
//
//   Cruise    SummerActive2023Page_SailInformation</N>i</N>j  -> Dlg23SummerMain_UpdateBlock (State / cost / reward preview)
//             SummerActive2023Page_ClickCruise</N>i</N>j</N>1 -> takes the Heart of Ocean, moves the ark (PlayEffect, MoveBoat,
//             SailType 2); the client ends the sail effect with SummerActiveInstance_Sweep</N>2</N>i</N>j -> the islet is
//             finished and its reward is given (only once per islet, only after a paid cruise to it).
//             OFFICIAL: the 31 islets / 6 areas (Dialog_Info 1386-mainconfig), the fog per area, "Upgrade Dream Ark to level %s"
//             (tSummerActive2023Page_DelText[105Tips][Locked]), "30 Dream Points from each cruise ... and Anniversary Wish"
//             (tSummerActive2023Npc[31769][Text315]), "Use Hearts of Ocean to start Dream Cruise", Dream Conch from cruises.
//             DERIVED (config): 1 Heart of Ocean per cruise and the islet reward list (both = the client's own sample block in
//             yangfanyuanhangproxy test2), ark level = 1 + areas fully cruised, the islet events are skipped (SailType 2: the
//             islet instances tSummer2023Instance 2391-2396 have no map on this server).
//   Sign-in   SummerActive2023Page_OpenWelfareInterface</N>6 ("Daily Sign-in %s/5", Text1312): DERIVED 5 Heart of Ocean(B) a day.
//   Wishes    SummerActive2023Page_OpenDrawInterface -> DlgValentinesDayMainUI with the client's default tables
//             (Dialog_Info["1220-other"]: cost 1 / 10 Dream Conch, 20 wheel items). SummerActive2023Page_DrawReward</N>1|10.
//             +30 Dream Points per wish (official). Wheel weights DERIVED (slot colour). No stage list (the client's default
//             stage table belongs to another event): RewardList = {}.
//  State per character: Data\Summer2023\players.json. Config: Data\Summer2023\summer2023.json.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using G = Qahr.Mobile.SummerWhales.FestGive;

namespace Qahr.Mobile.Summer2023
{
    public static class Summer2023Cruise
    {
        public const uint HeartOfOcean = 3342540, DreamConch = 3342541;

        // client Dialog_Info["1220-other"] DrawItemlist / DrawItemBg (luacj_item1 white .. 5 yellow)
        public static readonly string[] Wheel =
        {
            "3342770 0 1", "3342777 0 1", "3321077 0 2 3", "3342542 0 2", "3328247 0 20", "4060001 0 8 3", "3314212 0 1 3", "3009102 0 1",
            "3342542 0 5", "4060001 0 20 3", "3342779 0 1", "3328247 0 20", "3314553 0 1 3", "3342772 0 1", "3342774 0 1", "3342775 0 1",
            "3314250 0 1 3", "3342776 0 1", "3342621 0 1", "3342620 0 1",
        };
        static readonly int[] Colour = { 5, 4, 1, 4, 1, 2, 1, 2, 3, 1, 5, 4, 1, 4, 2, 5, 4, 5, 1, 1 };
        // client sample islet block (yangfanyuanhangproxy test2): cost + reward
        public static readonly string[] IsletReward = { "3342541 0 1", "3009101 0 3", "3303373 0 1", "730002 0 2 3 7200 1", "3009001 0 1 0 2880 1", "3321077 0 3 3" };

        public sealed class Config
        {
            public int CruiseCost = 1;                   // derived (client sample block)
            public string[] Reward = IsletReward;        // derived (client sample block)
            public int PointsPerCruise = 30, PointsPerWish = 30;// official Text315
            public int SignInPerDay = 5;                 // derived ("Daily Sign-in %s/5")
            public int WishCostOne = 1, WishCostTen = 10;// official 1220-other DrawCostItem
            public long[] Weights;                       // derived
            public int CooldownMs = 1200;                // derived
            public Config() { Weights = Colour.Select(c => c == 5 ? 2L : c == 4 ? 8L : c == 3 ? 20L : c == 2 ? 35L : 50L).ToArray(); }
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(SummerWhales.FestPlayerStore.RootOverride ?? global::Qahr.Mobile.Festivals.FestivalLoader.Root, "Summer2023", "summer2023.json"); } }
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
                    var d = fi.Exists ? FL.ReadJson(fi.FullName) : null;
                    if (d != null)
                    {
                        var v = FL.Val(d, "cruiseCost"); if (v != null) c.CruiseCost = (int)Math.Max(1, Math.Min(100, FL.U(v)));
                        v = FL.Val(d, "signInPerDay"); if (v != null) c.SignInPerDay = (int)Math.Min(100, FL.U(v));
                        var r = FL.Arr(FL.Val(d, "isletReward")); if (r != null && r.Count > 0) c.Reward = r.Select(x => x.ToString()).Where(s => G.Parse(s) != null).ToArray();
                        var w = FL.Arr(FL.Val(d, "wishWeights")); if (w != null && w.Count == Wheel.Length) c.Weights = w.Select(x => (long)Math.Min(1000000, FL.U(x))).ToArray();
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Summer2023] summer2023.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        static SummerWhales.FestPlayerStore _store;
        public static SummerWhales.FestPlayerStore Store { get { return _store ?? (_store = new SummerWhales.FestPlayerStore("Summer2023")); } }
        public static void ResetForTests() { _store = null; _pending.Clear(); _last.Clear(); _cfg = null; }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }
        static readonly ConcurrentDictionary<uint, Tuple<long, string[]>> _pending = new ConcurrentDictionary<uint, Tuple<long, string[]>>();
        static readonly Random _rng = new Random();
        static int Pick(long[] w)
        {
            long total = w.Sum(); if (total <= 0) return -1;
            long r; lock (_rng) r = (long)(_rng.NextDouble() * total);
            for (int i = 0; i < w.Length; i++) { if (r < w[i]) return i; r -= w[i]; }
            return w.Length - 1;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, Summer2023Ui.LuaMsg); }
        static string Q(string s) { return "'" + s.Replace("\\", "").Replace("'", "") + "'"; }

        // ------------------------------------------------------------------ islets
        /// <summary>Islet number 0..30 (Dialog_Info 1386-mainconfig StrToIndex minus 1), -1 = invalid. "00" = Departure Spot.</summary>
        public static int IsletNo(int i, int j)
        {
            if (i == 1 && j == 6) return 0;
            if (i == 1 && j >= 1 && j <= 5) return j;
            if (i >= 2 && i <= 6 && j >= 1 && j <= 5) return 6 + (i - 2) * 5 + (j - 1);
            return -1;
        }
        static int AreaOf(int no) { return no <= 5 ? 1 : 2 + (no - 6) / 5; }
        static int AreaSize(int area) { return area == 1 ? 6 : 5; }
        static IEnumerable<int> IsletsOf(int area) { return Enumerable.Range(0, 31).Where(n => AreaOf(n) == area); }
        static string Code(int no)
        {
            if (no == 0) return "16";
            if (no <= 5) return "1" + no;
            int a = 2 + (no - 6) / 5, j = 1 + (no - 6) % 5; return a.ToString() + j;
        }

        public static long Done(uint uid) { return Store.Get(uid, "done"); }
        static bool IsDone(long mask, int no) { return (mask & (1L << no)) != 0; }
        static bool AreaDone(long mask, int area) { return IsletsOf(area).All(n => IsDone(mask, n)); }
        /// <summary>Ark level 1..7: 1 + number of areas fully cruised in order (derived).</summary>
        public static int ArkLevel(long mask) { int lv = 1; for (int a = 1; a <= 6 && AreaDone(mask, a); a++) lv++; return lv; }
        static bool Unlocked(long mask, int area) { return area <= ArkLevel(mask); }

        public static string MainData(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            long mask = Done(uid);
            int lv = ArkLevel(mask);
            long pos = Store.Get(uid, "pos");
            string posCode = pos > 0 ? pos.ToString() : "00";
            var fog = Enumerable.Range(1, 6).Select(a => Unlocked(mask, a) ? 3 : 1);
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['Point']=").Append(Store.Get(uid, "points")).Append(',');
            sb.Append("['IslanIndex']='").Append(posCode).Append("',");
            sb.Append("['Fog']={").Append(string.Join(",", fog)).Append("},");
            sb.Append("['Boat']={").Append(Math.Min(6, lv - 1)).Append(",0,'").Append(posCode).Append("'},");
            sb.Append("['ItemInfo']={'").Append(HeartOfOcean).Append(" 0 ").Append(G.Count(c, HeartOfOcean)).Append("','")
              .Append(DreamConch).Append(" 0 ").Append(G.Count(c, DreamConch)).Append("'},");
            sb.Append("['Event']={},['Boss']={0,0},['EventFinish']='00'");
            return sb.Append("}}").ToString();
        }

        public static string BlockData(Client.GameClient c, int i, int j, int playEffect, int moveBoat)
        {
            var cfg = Cfg;
            if (i == 0 && j == 0)
            {
                long m = Done(c.Player.UID); int lv = ArkLevel(m);
                int area = Math.Min(6, lv), have = IsletsOf(area).Count(n => IsDone(m, n));
                return "{['Code']=200,['Data']={['SailType']=2,['State']=2,['IsFinished']=0,['Type']=2,['PlayEffect']=0,['IslanIndex']='00',['MoveBoat']=" + moveBoat
                    + ",['Progress']={" + lv + "," + have + "," + AreaSize(area) + "},['ItemReward']={}}}";
            }
            int no = IsletNo(i, j);
            long mask = Done(c.Player.UID);
            bool done = IsDone(mask, no), open = Unlocked(mask, AreaOf(no));
            int state = !done && open ? 1 : 2;
            return "{['Code']=200,['Data']={['SailType']=2,['State']=" + state + ",['IsFinished']=" + (done ? 1 : 0) + ",['Type']=1,['PlayEffect']=" + playEffect
                + ",['IslanIndex']='" + i + j + "',['MoveBoat']=" + moveBoat + ",['ItemCost']={'" + HeartOfOcean + " 0 " + cfg.CruiseCost + "'},['ItemGet']={"
                + string.Join(",", cfg.Reward.Select(Q)) + "}}}";
        }

        /// <summary>Paid cruise to islet (i,j). Returns the error text or null.</summary>
        public static string Cruise(Client.GameClient c, ServerSockets.Packet stream, int i, int j)
        {
            var cfg = Cfg;
            int no = IsletNo(i, j);
            if (no < 0) return "?";
            uint uid = c.Player.UID;
            long mask = Done(uid);
            if (IsDone(mask, no)) return "You have already cruised to this islet.";
            if (!Unlocked(mask, AreaOf(no))) return "This area was shrouded in mist. Upgrade Dream Ark to level " + AreaOf(no) + " to go there.";
            if (Store.Get(uid, "sailing") == 1)
            {
                // a paid sail that hasn't landed yet (effect interrupted, relog): it is re-aimed at this islet, no second charge
                Store.Set(uid, "sail", no);
                Lua(c, "Dlg23SummerMain_UpdateBlock|" + BlockData(c, i, j, 1, 1));
                return null;
            }
            var items = cfg.Reward.Select(G.Parse).Where(x => x != null && G.Exists(x.Id)).ToList();
            int slots = G.Slots(items);
            if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return "Please make " + slots + " space(s) in your inventory first.";
            if (G.Count(c, HeartOfOcean) < cfg.CruiseCost) return "You don`t have enough Hearts of Ocean. Click the `+` to view obtaining methods.";
            if (!G.Take(c, HeartOfOcean, cfg.CruiseCost, stream)) return "You don`t have enough Hearts of Ocean.";
            Store.Set(uid, new Dictionary<string, long> { { "sail", no }, { "sailing", 1 } });
            Lua(c, "Dlg23SummerMain_UpdateBlock|" + BlockData(c, i, j, 1, 1));
            return null;
        }

        /// <summary>The landing (client: end of the sail effect). Gives the islet reward once.</summary>
        public static string Land(Client.GameClient c, ServerSockets.Packet stream, int i, int j)
        {
            var cfg = Cfg;
            int no = IsletNo(i, j);
            if (no < 0) return "?";
            uint uid = c.Player.UID;
            var items = cfg.Reward.Select(G.Parse).Where(x => x != null && G.Exists(x.Id)).ToList();
            int slots = G.Slots(items);
            if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return "Please make " + slots + " space(s) in your inventory first.";
            bool ok = Store.Update(uid, get =>
            {
                if (get("sailing") != 1 || get("sail") != no) return null;
                long mask = get("done");
                if (IsDone(mask, no)) return null;
                return new Dictionary<string, long> { { "done", mask | (1L << no) }, { "sailing", 0 }, { "sail", -1 }, { "pos", i * 10 + j }, { "points", get("points") + cfg.PointsPerCruise }, { "cruises", get("cruises") + 1 } };
            });
            if (!ok) return "?";
            G.Give(c, items, stream, "Summer2023/Cruise" + i + j);
            Msg(c, "The Dream Ark reached " + i + j + ". You received " + cfg.PointsPerCruise + " Dream Points.");
            Lua(c, "Dlg23SummerMain_BindData|" + MainData(c));
            Lua(c, "Dlg23SummerMain_UpdateBlock|" + BlockData(c, i, j, 0, 0));
            return null;
        }

        public static string SignIn(Client.GameClient c, ServerSockets.Packet stream)
        {
            var cfg = Cfg;
            if (cfg.SignInPerDay <= 0) return "Daily sign-in packs aren`t available on this server.";
            long today = long.Parse(DateTime.Now.ToString("yyyyMMdd"));
            if (!c.Inventory.HaveSpace(1)) return "Please make 1 space(s) in your inventory first.";
            bool ok = Store.Update(c.Player.UID, get => get("sign") == today ? null : new Dictionary<string, long> { { "sign", today } });
            if (!ok) return "You`ve already signed in today. Come back tomorrow!";
            G.Give(c, new[] { new G.GItem { Id = HeartOfOcean, Count = cfg.SignInPerDay, Bound = true } }, stream, "Summer2023/SignIn");
            Msg(c, "Daily Sign-in: you received " + cfg.SignInPerDay + " Heart of Ocean(B).");
            Lua(c, "Dlg23SummerMain_BindData|" + MainData(c));
            return null;
        }

        // ------------------------------------------------------------------ Anniversary Wish (default DlgValentinesDayMainUI)
        public static string WishOpen(Client.GameClient c)
        {
            return "DlgValentinesDay_MainOpen|{['Code']=100,['Data']={['ModelType']=0,['CostItem']=" + G.Count(c, DreamConch) + ",['ChangeItem']=" + G.Count(c, DreamConch)
                + ",['DrawTimes']=" + Store.Get(c.Player.UID, "wishes") + ",['Rewardflag']=0,['RewardList']={},['DrawList']={}}}";
        }

        public static string Wish(Client.GameClient c, ServerSockets.Packet stream, int n)
        {
            var cfg = Cfg;
            if (n != 1 && n != 10) return "?";
            int cost = n == 1 ? cfg.WishCostOne : cfg.WishCostTen;
            if (G.Count(c, DreamConch) < cost) return "You don`t have enough Dream Conches. Click the `+` to view obtaining methods.";
            var w = new long[Wheel.Length];
            for (int k = 0; k < w.Length; k++) { var g = G.Parse(Wheel[k]); w[k] = g != null && G.Exists(g.Id) ? cfg.Weights[k] : 0; }
            var picks = new List<int>();
            for (int k = 0; k < n; k++) { int p = Pick(w); if (p < 0) return "Anniversary Wish isn`t available right now."; picks.Add(p); }
            var items = picks.Select(p => G.Parse(Wheel[p])).ToList();
            int slots = G.Slots(items);
            if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return "Please make " + slots + " space(s) in your inventory first.";
            if (!G.Take(c, DreamConch, cost, stream)) return "You don`t have enough Dream Conches.";
            long wishes = 0;
            Store.Update(c.Player.UID, get => { wishes = get("wishes") + n; return new Dictionary<string, long> { { "wishes", wishes }, { "points", get("points") + (long)cfg.PointsPerWish * n } }; });
            G.Give(c, items, stream, "Summer2023/AnniversaryWish");
            _pending[c.Player.UID] = Tuple.Create((long)(Environment.TickCount & 0x7FFFFFFF), items.Select(x => x.ToString()).ToArray());
            Lua(c, "DlgValentinesDay_DrawBack|{['Code']=200,['Data']={['CostItem']=" + G.Count(c, DreamConch) + ",['DrawTimes']=" + wishes + ",['DrawList']={"
                + string.Join(",", picks.Select(p => (p + 1).ToString())) + "},['ChangeItem']=" + G.Count(c, DreamConch) + ",['Rewardflag']=0}}");
            return null;
        }

        /// <summary>10139 Lover2025Kua_DrawCongratulation after one of our wishes (routed by SummerWhalesUi.OnRaw).</summary>
        public static bool OnCongratulation(Client.GameClient c)
        {
            Tuple<long, string[]> p;
            if (c == null || c.Player == null || !_pending.TryRemove(c.Player.UID, out p)) return false;
            long now = Environment.TickCount & 0x7FFFFFFF;
            if (now - p.Item1 < 0 || now - p.Item1 > 120000) return false;
            LuaUi.LuaPush.Send(c, "DlgValentinesDay_RewardServer|{['Code']=200,['Data']={" + string.Join(",", p.Item2.Select(Q)) + "}}");
            return true;
        }

        /// <summary>Summer2023Ui.OnLua delegates the playable calls here. True = handled.</summary>
        public static bool OnFunc(Client.GameClient c, string func, string[] args)
        {
            int a, b, n;
            Func<int, int> arg = k => args.Length > k && int.TryParse(args[k], out n) ? n : -1;
            string err = null;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                switch (func)
                {
                    case "SummerActive2023Page_OpenMainInterface":
                        Lua(c, "Dlg23SummerMain_BindData|" + MainData(c)); return true;
                    case "SummerActive2023Page_SailInformation":
                        a = arg(0); b = arg(1);
                        if (!(a == 0 && b == 0) && IsletNo(a, b) < 0) return true;
                        Lua(c, "Dlg23SummerMain_UpdateBlock|" + BlockData(c, a, b, 0, 0));
                        if (a > 0 && !Unlocked(Done(c.Player.UID), AreaOf(IsletNo(a, b)))) Msg(c, "This area was shrouded in mist. Upgrade Dream Ark to level " + AreaOf(IsletNo(a, b)) + " to go there.");
                        return true;
                    case "SummerActive2023Page_ClickCruise":
                        if (Throttled(c.Player.UID, Cfg.CooldownMs)) return true;
                        err = Cruise(c, s, arg(0), arg(1)); break;
                    case "SummerActiveInstance_Sweep":
                        if (arg(0) != 2) return true;
                        err = Land(c, s, arg(1), arg(2)); break;
                    case "SummerActiveInstance_EnterIsland":
                        err = "The islet events aren`t open on this server yet."; break;
                    case "SummerActive2023Page_OpenWelfareInterface":
                        if (Throttled(c.Player.UID, Cfg.CooldownMs)) return true;
                        err = SignIn(c, s); break;
                    case "SummerActive2023Page_OpenDrawInterface":
                        LuaUi.LuaPush.Send(c, WishOpen(c)); return true;
                    case "SummerActive2023Page_DrawReward":
                        if (Throttled(c.Player.UID, Cfg.CooldownMs)) return true;
                        err = Wish(c, s, arg(0)); break;
                    default: return false;
                }
            }
            if (err != null && err != "?") Msg(c, err);
            return true;
        }
    }
}
