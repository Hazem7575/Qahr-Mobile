// ============================================================================
//  [WenwenDetective 2026-10-15] Goosie Adventure (Children's Day 2024) - Titan Vault golden eggs, Goosie Points, stage rewards,
//  fragment combining. Window = official DlgChildrenDayDraw (DlgChildrenDayDraw_MainOpen / _ShowDrawBack / _UpdateView) and the
//  common stage window DlgMidAutumnRank (MidAutumn_Config[10406]).
//  OFFICIAL (client): Bubble Hammer 3600709 smashes the golden eggs (itemtype text); smash 1 or 5 (btn_draw_1 "1", btn_draw_10 "5");
//    of the 5 eggs one is golden: index ((2 - LotteryCount%5)+5)%5+1 (dlgchildrendaydraw BindEggInfo); egg rewards =
//    MidAutumn_Config[10406]["PreviewReward"][1] (eggs) and [2] (golden egg); stage rewards = ["Total"][1..9]; nMaxScore 120000;
//    Bubble Machine Fragment 3348595: 30 -> permanent unbound Bubble Machine; Spring Goosie Fragment 3348596: 30 -> 3-star
//    Spring Goosie (B), 50 -> 5-star Spring Goosie (Cute) (itemtype texts); texts tChildrenDayBigKu_DelText[Sys].
//  DERIVED (Data\WenwenDetective\goosie.json): reward weights, Goosie Points per egg, stage thresholds, 1 hammer per egg.
//  State: Data\WenwenDetective\state.json (per character per festival run). Security: uid from the session, whitelisted
//  arguments, hammers taken before the eggs are rolled, stage claims recorded before the reward, bag space first, 400 ms throttle.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.ChildrensDay2025;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using G = Qahr.Mobile.ChildrensDay2025.DreamParkItems.Grant;

namespace Qahr.Mobile.WenwenDetective
{
    public sealed class GoosiePlayer { public uint Uid; public string Name = ""; public int Run; public uint Score, Lottery, StageMask, RepeatClaimed; public long Updated; }

    public static class GoosieVault
    {
        public const uint BubbleHammer = 3600709, BubbleFragment = 3348595, GoosieFragment = 3348596;
        public static readonly object Sync = new object();

        // official preview tables (MidAutumn_Config[10406]); weights DERIVED
        public static readonly G[] Eggs =
        {
            new G(3348595, 1, false, 6), new G(3348596, 1, false, 4), new G(3346828, 1, false, 20), new G(3346831, 1, false, 3),
            new G(3346836, 1, false, 20), new G(3346837, 1, false, 6), new G(3321077, 1, true, 15), new G(3314207, 1, true, 5),
            new G(3346821, 1, false, 15), new G(3009100, 1, false, 6),
        };
        public static readonly G[] GoldenEgg =
        {
            new G(3348596, 5, false, 3), new G(3348596, 1, false, 12), new G(3328247, 30, false, 20), new G(3328247, 100, false, 8),
            new G(4060001, 20, true, 15), new G(4060001, 50, true, 6), new G(3346822, 1, false, 12), new G(3346824, 1, false, 6),
            new G(3009100, 10, false, 8), new G(3009102, 1, false, 6), new G(723694, 1, true, 3), new G(723695, 1, true, 1),
        };
        // official "Total" rows 1..9 (row 9 repeats)
        public static readonly G[][] Total =
        {
            new[] { new G(3346830, 1, true), new G(3348595, 1) },
            new[] { new G(3346824, 2, true), new G(3348595, 2) },
            new[] { new G(3346837, 1), new G(3348595, 3) },
            new[] { new G(3009103, 1), new G(3348596, 1) },
            new[] { new G(3348594, 1), new G(3328247, 500), new G(3348596, 3) },
            new[] { new G(3337910, 6), new G(3348596, 5) },
            new[] { new G(3337910, 10), new G(3348596, 5) },
            new[] { new G(3337910, 20), new G(3348596, 10) },
            new[] { new G(3009103, 1), new G(3348596, 1) },
        };

        public sealed class Config
        {
            public uint[] Stage = { 100, 300, 600, 1000, 2000, 4000, 6000, 10000 };  // rows 1..8 (DERIVED)
            public uint RepeatEvery = 2000;                                          // row 9 every N points after row 8 (DERIVED)
            public uint EggPoints = 10, GoldenPoints = 20;                           // Goosie Points (DERIVED)
            public uint MaxScore = 120000;                                           // official nMaxScore
            public uint BubbleFragments = 30, GoosieFragments3 = 30, GoosieFragments5 = 50;// official
        }
        static Config _cfg; static string _stamp;
        static string Dir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "WenwenDetective"); } }
        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(Path.Combine(Dir, "goosie.json"));
                    string st = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && st == _stamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists)
                    {
                        var d = FL.ReadJson(fi.FullName);
                        var a = FL.Arr(FL.Val(d, "stagePoints"));
                        if (a != null && a.Count == 8) { var v = a.Select(x => FL.U(x)).ToArray(); bool asc = true; for (int i = 1; i < 8; i++) if (v[i] <= v[i - 1]) asc = false; if (asc && v[0] > 0) c.Stage = v; }
                        var r = FL.Val(d, "repeatEvery"); if (r != null) c.RepeatEvery = Math.Max(1u, FL.U(r));
                        var e = FL.Val(d, "eggPoints"); if (e != null) c.EggPoints = Math.Min(100000u, FL.U(e));
                        var g = FL.Val(d, "goldenPoints"); if (g != null) c.GoldenPoints = Math.Min(100000u, FL.U(g));
                    }
                    _cfg = c; _stamp = st;
                }
                catch (Exception ex) { try { MyConsole.WriteLine("[WenwenDetective] goosie.json: " + ex.Message, ConsoleColor.Yellow); } catch { } if (_cfg == null) _cfg = new Config(); }
                return _cfg;
            }
        }

        // ------------------------------------------------------------------ state
        static Dictionary<uint, GoosiePlayer> _all;
        static string StateFile { get { return Path.Combine(Dir, "state.json"); } }
        static int Run { get { try { var r = Festivals.FestivalManager.Running(WenwenDetectiveUi.Festival); return r == null ? 0 : r.Run; } catch { return 0; } } }
        public static GoosiePlayer Get(Client.GameClient c)
        {
            if (_all == null)
            {
                _all = new Dictionary<uint, GoosiePlayer>();
                try
                {
                    if (File.Exists(StateFile))
                        foreach (var o in FL.Arr(FL.Val(FL.ReadJson(StateFile), "players")) ?? new List<object>())
                        {
                            var d = FL.Obj(o); if (d == null) continue;
                            var p = new GoosiePlayer { Uid = FL.U(FL.Val(d, "uid")), Name = FL.Str(d, "name") ?? "", Run = (int)FL.U(FL.Val(d, "run")), Score = FL.U(FL.Val(d, "score")), Lottery = FL.U(FL.Val(d, "lottery")), StageMask = FL.U(FL.Val(d, "stages")), RepeatClaimed = FL.U(FL.Val(d, "repeat")), Updated = (long)FL.UL(FL.Val(d, "updated")) };
                            if (p.Uid != 0) _all[p.Uid] = p;
                        }
                }
                catch (Exception ex) { try { MyConsole.WriteLine("[WenwenDetective] state.json ignored: " + ex.Message, ConsoleColor.Yellow); } catch { } }
            }
            GoosiePlayer gp; int run = Run;
            if (!_all.TryGetValue(c.Player.UID, out gp) || gp.Run != run) { gp = new GoosiePlayer { Uid = c.Player.UID, Run = run }; _all[gp.Uid] = gp; }
            gp.Name = c.Player.Name ?? "";
            return gp;
        }
        static void Save()
        {
            try
            {
                var list = _all.Values.OrderBy(p => p.Uid).Select(p => (object)new Dictionary<string, object> { { "uid", p.Uid }, { "name", p.Name }, { "run", p.Run }, { "score", p.Score }, { "lottery", p.Lottery }, { "stages", p.StageMask }, { "repeat", p.RepeatClaimed }, { "updated", p.Updated } }).ToList();
                var root = new Dictionary<string, object> { { "_readme", "[WenwenDetective] Goosie Points / Titan Vault per character (written by the GameServer)." }, { "players", list } };
                Directory.CreateDirectory(Dir);
                string tmp = StateFile + ".tmp";
                File.WriteAllText(tmp, AdminApi.AdminJson.Write(root), new UTF8Encoding(false));
                if (File.Exists(StateFile)) File.Replace(tmp, StateFile, null); else File.Move(tmp, StateFile);
            }
            catch (Exception ex) { try { MyConsole.WriteLine("[WenwenDetective] could not save state.json: " + ex.Message, ConsoleColor.Yellow); } catch { } }
        }
        public static void ResetCache() { lock (Sync) _all = null; }
        static void Log(Client.GameClient c, string what)
        {
            try { Directory.CreateDirectory(Dir); File.AppendAllText(Path.Combine(Dir, "log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + c.Player.UID + " " + c.Player.Name + " | " + what + Environment.NewLine, Encoding.UTF8); } catch { }
        }

        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, WenwenDetectiveUi.LuaMsg); }
        static string Str(G g) { return g.Id + " 0 " + g.Count + (g.Bound ? " 3" : ""); }
        static string ItemInfo(Client.GameClient c) { return BubbleHammer + " 0 " + DreamParkItems.Count(c, BubbleHammer); }
        public static int GoldenIndex(uint lottery) { return (int)(((2 - (int)(lottery % 5)) + 5) % 5) + 1; }

        // ------------------------------------------------------------------ draw window
        public static string DrawCall(Client.GameClient c, string func)
        {
            GoosiePlayer p; lock (Sync) p = Get(c);
            return func + "|{['Code']=200,['Data']={['Score']=" + p.Score + ",['ItemInfo']='" + ItemInfo(c) + "',['LotteryCount']=" + p.Lottery + ",['ScoreTip']=" + (CanClaim(p).Count > 0 ? 1 : 0) + "}}";
        }

        /// <summary>ChildrenDayBigKu_Click n (n = 1 or 5 eggs).</summary>
        public static void Smash(Client.GameClient c, int n)
        {
            if (n != 1 && n != 5) return;
            var cfg = Cfg;
            lock (Sync)
            {
                if (DreamParkItems.Count(c, BubbleHammer) < (uint)n) { Msg(c, "Insufficient Bubble Hammers."); Lua(c, "DlgChildrenDayMain_Pay"); return; }
                var p = Get(c);
                if (p.Score >= cfg.MaxScore) { Msg(c, "You have already smashed too many golden eggs and cannot continue for now."); return; }
                if (!DreamParkItems.HaveSpace(c, n * 2)) { Msg(c, "Please make " + (n * 2) + " space(s) in your inventory first."); return; }
                int golden = GoldenIndex(p.Lottery);
                var got = new List<G>(); uint pts = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (!DreamParkItems.Take(c, BubbleHammer, (uint)n, stream)) return;
                    for (int i = 1; i <= n; i++)
                    {
                        bool gold = i == golden;
                        got.Add(DreamParkItems.Pick(gold ? GoldenEgg : Eggs));
                        pts += gold ? cfg.GoldenPoints : cfg.EggPoints;
                    }
                    p.Lottery += (uint)n; p.Score = Math.Min(cfg.MaxScore, p.Score + pts); p.Updated = DateTime.UtcNow.Ticks;
                    Save();
                    string s = DreamParkItems.GiveAll(c, got, stream);
                    Log(c, "SMASH x" + n + " golden@" + golden + " +" + pts + " pts=" + p.Score + " -> " + s);
                    if (got.Any(g => g.Id == GoosieFragment)) Msg(c, string.Format("How lucky! {0} actually received a Spring Goosie Fragment in Titan Vault!", c.Player.Name));
                }
                var sb = new StringBuilder("DlgChildrenDayDraw_ShowDrawBack|{['Code']=200,['Data']={");
                foreach (var g in got) sb.Append('\'').Append(Str(g)).Append("',");
                sb.Append("['Score']=").Append(p.Score).Append(",['ItemInfo']='").Append(ItemInfo(c)).Append("'}}");
                Lua(c, sb.ToString());
            }
        }

        // ------------------------------------------------------------------ Goosie Points stages (DlgMidAutumnRank, MidAutumn_Config[10406])
        static uint Repeats(Config cfg, GoosiePlayer p) { uint top = cfg.Stage[7]; return p.Score <= top ? 0 : (p.Score - top) / cfg.RepeatEvery; }
        static List<int> CanClaim(GoosiePlayer p)
        {
            var cfg = Cfg; var l = new List<int>();
            for (int i = 0; i < 8; i++) if ((p.StageMask & (1u << (i + 1))) == 0 && p.Score >= cfg.Stage[i]) l.Add(i + 1);
            if (Repeats(cfg, p) > p.RepeatClaimed) l.Add(9);
            return l;
        }
        static string L(IEnumerable<int> l) { return "{" + string.Join(",", l) + "}"; }
        public static string StageData(Client.GameClient c, string last)
        {
            var cfg = Cfg; GoosiePlayer p; lock (Sync) p = Get(c);
            var claimed = new List<int>(); var no = new List<int>();
            for (int i = 0; i < 8; i++) if ((p.StageMask & (1u << (i + 1))) != 0) claimed.Add(i + 1); else if (p.Score < cfg.Stage[i]) no.Add(i + 1);
            var can = CanClaim(p);
            if (!can.Contains(9)) no.Add(9);
            uint next = cfg.Stage.FirstOrDefault(s => s > p.Score);
            if (next == 0) next = cfg.Stage[7] + cfg.RepeatEvery * (Repeats(cfg, p) + 1);
            return "{['Code']=200,['Data']={['ActivityId']=" + WenwenDetectiveUi.LuaMsg + ",['LastDialog']='" + last + "',['IsOpen']=1,['IsBack']=1,['IsTime']=1,['IsRule']=1"
                + ",['Claimed']=" + L(claimed) + ",['NoClaim']=" + L(no) + ",['CanClaim']=" + L(can) + ",['RankNum']=2"
                + ",['IsPoint']=1,['PointProgress']={" + p.Score + "," + next + "}}}";
        }
        public static void ClaimStage(Client.GameClient c, int row)
        {
            if (row < 1 || row > 9) return;
            var cfg = Cfg;
            lock (Sync)
            {
                var p = Get(c);
                if (!CanClaim(p).Contains(row)) { Msg(c, "You don`t have any rewards to claim."); return; }
                var gives = Total[row - 1];
                int slots = DreamParkItems.Slots(gives);
                if (!DreamParkItems.HaveSpace(c, slots)) { Msg(c, "Please make " + slots + " space(s) in your inventory first."); return; }
                if (row == 9) p.RepeatClaimed++; else p.StageMask |= 1u << row;
                Save();
                string s;
                using (var rec = new ServerSockets.RecycledPacket()) s = DreamParkItems.GiveAll(c, gives, rec.GetStream());
                Log(c, "STAGE " + row + " pts=" + p.Score + " -> " + s);
                Msg(c, "Congratulations! You obtained " + s + "!");
            }
            Lua(c, "DlgMidAutumnRank_UpdateReward|" + StageData(c, "DlgChildrenDayMain"));
        }
        public static void Ranking(Client.GameClient c)
        {
            List<GoosiePlayer> top; GoosiePlayer me; int run = Run;
            lock (Sync) { me = Get(c); top = _all.Values.Where(p => p.Run == run && p.Score > 0).OrderByDescending(p => p.Score).ThenBy(p => p.Updated).Take(10).ToList(); }
            Msg(c, "Children`s Day Local Ranking (Goosie Points). Yours: " + me.Score + ".");
            if (top.Count == 0) Msg(c, "No one has achieved a ranking.");
            for (int i = 0; i < top.Count; i++) Msg(c, "No." + (i + 1) + "  " + top[i].Name + "  " + top[i].Score);
        }

        // ------------------------------------------------------------------ fragments (MsgItemUsuagePacket.UseItem hook)
        public static bool OnUse(MsgGameItem item, Client.GameClient c, ServerSockets.Packet stream)
        {
            if (item == null || c == null || c.Player == null) return false;
            uint iid = item.ITEM_ID;
            if (iid != BubbleFragment && iid != GoosieFragment && iid != 3348650 && iid != 3348651 && iid != 3348803) return false;
            try
            {
                var cfg = Cfg;
                lock (Sync)
                {
                    if (iid == 3348650 || iid == 3348651 || iid == 3348803)
                    {
                        // official 'Use it to get': 3348650 -> 3-star Mount Armor Spring Goosie 200921, 3348651 -> 5-star Spring Goosie (Cute) 200922,
                        // 3348803 -> permanent Bubble Machine (official: choose 1H/2H; no choice dialog here -> 1-handed 360446, DERIVED)
                        MsgGameItem live;
                        if (!c.Inventory.TryGetItem(item.UID, out live) || live.ITEM_ID != iid) return true;
                        var g2 = iid == 3348650 ? new G(200921, 1, item.Bound != 0) : iid == 3348651 ? new G(200922, 1, item.Bound != 0) : new G(360446, 1, item.Bound != 0);
                        if (!DreamParkItems.Exists(g2.Id)) return true;
                        if (!DreamParkItems.HaveSpace(c, 1)) { Msg(c, "Please make 1 space(s) in your inventory first."); return true; }
                        if (!DreamParkItems.Take(c, iid, 1, stream)) return true;
                        string s2 = DreamParkItems.Give(c, g2, stream);
                        Log(c, "OPEN " + iid + " -> " + s2);
                        Msg(c, "Congratulations! You obtained " + s2 + "!");
                        return true;
                    }
                    uint id = item.ITEM_ID, have = DreamParkItems.Count(c, id), need; G give;
                    if (id == BubbleFragment) { need = cfg.BubbleFragments; give = new G(3348803, 1); }        // Bubble Machine (choose 1H/2H) - permanent, unbound
                    else if (have >= cfg.GoosieFragments5) { need = cfg.GoosieFragments5; give = new G(3348651, 1); }// Spring Goosie (Cute) 5-star
                    else { need = cfg.GoosieFragments3; give = new G(3348650, 1, true); }                     // Spring Goosie 3-star (B)
                    if (have < need) { Msg(c, "Combine " + need + " " + QuestEngine.QuestGrammar.ItemName(id) + "s (you have " + have + ")."); return true; }
                    if (!DreamParkItems.Exists(give.Id)) return true;
                    if (!DreamParkItems.HaveSpace(c, 1)) { Msg(c, "Please make 1 space(s) in your inventory first."); return true; }
                    if (!DreamParkItems.Take(c, id, need, stream)) return true;
                    string s = DreamParkItems.Give(c, give, stream);
                    Log(c, "COMBINE " + need + "x" + id + " -> " + s);
                    Msg(c, "Congratulations! You obtained " + s + "!");
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }
    }
}
