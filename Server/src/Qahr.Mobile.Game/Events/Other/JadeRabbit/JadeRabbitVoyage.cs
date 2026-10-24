// ============================================================================
//  [JadeRabbit 2026-10-15] PHASE COMPLETE: the Jade Rabbit "Lunar Voyage" exploration of the official Mid-Autumn 2024 event
//  (client fairygui exploration / endresult / pointsrank, msg 10459 tMsg_Info["FuiZhongqiu2024"], ERP592820), all 6 areas.
//
//  Official client data:
//    open      Fgui_Exploration_Open|{Code=200,Data={[1]=back,[2]={3 item strings, [3] = Heart Grass count},[3]={pts,max},
//              [4]=area 1..6,[5]=rabbit position 1 Start/2 Motivation/3 Trial/4 Ordeal/5 Terminus,[6]=red,[7]=red}}
//    explore   MidAutumnAdventure_ClickChou (1 Heart Grass 3600735: "After eating the grass, Jade Rabbit can set out on
//              adventure") -> Fgui_Exploration_Result {[1]=0 fail/1 success,[2],[3],[4],[5],[6],[7],[8]={rewards}}
//              (texts STR_FGUI_150137_12_* success / 13_* failure are chosen by the client).
//    terminus  MidAutumnAdventure_ClickEndChou -> Fgui_Exploration_Ending {[1]=area,[2]=ending 1|2,[3]={rewards}}
//              (EndResultData.CONST_DATA.Ending: areas 1-3 one ending, 4-6 a hidden 2nd ending "Reaching the terminus ...
//              repeatedly grants a chance of triggering this hidden ending", STR_FUI_ZHONGQIU2024_QUYUTIP4-6).
//    points    Voyage Points ("Dispatch Jade Rabbit on a Lunar Voyage to earn Voyage Points"); stage rewards
//              Fgui_PointsRank_Open MsgID 10459 = PointsRankData.CONST_POINTS_AWARD_DATA[10459] (ID 1 every 1000 points
//              3349949; IDs 2-8 at 5000/10000/20000/40000/60000/80000/120000), claim ZhongQiu2024Kua_StageReward</N>id.
//    rewards   exploration preview RewardPreviewData[150137] (Plenilune/Crescent Gems, Dewy Beauty Fragment, ...).
//    rules     [Sys][1] "Please return to Twin City first." [Sys][2] lack of items, [Sys][4] area already completed.
//  Story quests: the first ending of areas 1/2/6 continues on the area map (quests 8066/8067/8073, JadeRabbitUi.Goto).
//  DERIVED (Data\JadeRabbit\voyage.json): success chance, points per step, reward amounts and weights, the hidden
//  ending chance, the ending rewards (the client receives them from the server).
//
//  Hook: Base\MidAutumn2024\LunarVoyage.cs (msg 10459 callbacks), no new hook line.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.MidAutumnReunion2022;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using Row = System.Collections.Generic.Dictionary<string, string>;

namespace Qahr.Mobile.JadeRabbit
{
    public static class JadeRabbitVoyage
    {
        public const uint LuaMsg = 10459;
        public const uint HeartGrass = 3600735, Crescent = 3349919, Plenilune = 3349920;
        public static readonly int[] Endings = { 1, 1, 1, 2, 2, 2 };// EndResultData.CONST_DATA.Ending

        public sealed class Config
        {
            public int MinLevel = 80;
            public uint[] FromMaps = { 1002 };// [Sys][1] "Please return to Twin City first."
            public int SuccessPercent = 60;
            public int PointsSuccess = 100, PointsFail = 50, PointsTerminus = 500;
            public string StepSuccess = "3349919 0 2", StepFail = "3349919 0 1", Terminus = "3349920 0 5";
            public int ExtraPercent = 30;
            public string[] ExtraPool = { "3349968 0 1", "3349970 0 1", "3347157 0 2", "3347158 0 2", "3347151 0 10", "3314253 0 2 3", "3009102 0 1", "3009101 0 1", "3346830 0 1", "3346829 0 1", "3314207 0 1 3" };
            public int[] ExtraWeights = { 4, 2, 6, 10, 18, 12, 10, 14, 8, 8, 8 };
            public int HiddenPercent = 30;
            public string[][] EndingRewards =
            {
                new[] { "3349968 0 3" }, new[] { "3349968 0 3" }, new[] { "3349968 0 3" },
                new[] { "3349968 0 3" }, new[] { "3349968 0 3" }, new[] { "3349968 0 3" },
            };
            public string[][] HiddenRewards = { new[] { "3349968 0 5" }, new[] { "3349968 0 5" }, new[] { "3349968 0 5" } };// areas 4-6
            public int StageEvery = 1000; public string StageEveryItem = "3349949 0 1"; public int StageEveryMax = 999;
            public int[] StagePoints = { 5000, 10000, 20000, 40000, 60000, 80000, 120000 };
            public string[][] StageRewards =
            {
                new[] { "3349968 0 1", "3346824 0 1" }, new[] { "3349968 0 2", "3346837 0 1" }, new[] { "3349968 0 3", "3346831 0 1" },
                new[] { "3349968 0 5", "4060001 0 200 3", "3347034 0 1" }, new[] { "3349968 0 8", "3349442 0 6" },
                new[] { "3349968 0 10", "3349442 0 10" }, new[] { "3349968 0 15", "3349442 0 20" },
            };
        }
        static Config _cfg; static string _stamp;
        public static Config CfgOverride;
        static string DataDir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "JadeRabbit"); } }
        static string CfgFile { get { return Path.Combine(DataDir, "voyage.json"); } }
        public static Config Cfg
        {
            get
            {
                if (CfgOverride != null) return CfgOverride;
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
                    try { MyConsole.WriteLine("[JadeRabbit] voyage.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static int Int(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(int.MaxValue, FL.U(v)); }
        static int[] Ints(Dictionary<string, object> d, string k, int[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => (int)Math.Min(int.MaxValue, FL.U(x))).ToArray(); }
        static string[] Strs(Dictionary<string, object> d, string k, string[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => x == null ? "" : x.ToString()).ToArray(); }
        static string[][] Nested(Dictionary<string, object> d, string k, string[][] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => (FL.Arr(x) ?? new List<object>()).Select(y => y == null ? "" : y.ToString()).ToArray()).ToArray(); }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.MinLevel = Int(d, "minLevel", c.MinLevel);
            c.FromMaps = Ints(d, "fromMaps", c.FromMaps.Select(x => (int)x).ToArray()).Select(x => (uint)x).ToArray();
            c.SuccessPercent = Math.Max(0, Math.Min(100, Int(d, "successPercent", c.SuccessPercent)));
            c.PointsSuccess = Int(d, "pointsSuccess", c.PointsSuccess); c.PointsFail = Int(d, "pointsFail", c.PointsFail); c.PointsTerminus = Int(d, "pointsTerminus", c.PointsTerminus);
            c.StepSuccess = FL.Str(d, "stepSuccess") ?? c.StepSuccess; c.StepFail = FL.Str(d, "stepFail") ?? c.StepFail; c.Terminus = FL.Str(d, "terminus") ?? c.Terminus;
            c.ExtraPercent = Math.Max(0, Math.Min(100, Int(d, "extraPercent", c.ExtraPercent)));
            c.ExtraPool = Strs(d, "extraPool", c.ExtraPool); c.ExtraWeights = Ints(d, "extraWeights", c.ExtraWeights);
            c.HiddenPercent = Math.Max(0, Math.Min(100, Int(d, "hiddenPercent", c.HiddenPercent)));
            c.EndingRewards = Nested(d, "endingRewards", c.EndingRewards); c.HiddenRewards = Nested(d, "hiddenRewards", c.HiddenRewards);
            c.StageEvery = Math.Max(1, Int(d, "stageEvery", c.StageEvery)); c.StageEveryItem = FL.Str(d, "stageEveryItem") ?? c.StageEveryItem; c.StageEveryMax = Int(d, "stageEveryMax", c.StageEveryMax);
            c.StagePoints = Ints(d, "stagePoints", c.StagePoints); c.StageRewards = Nested(d, "stageRewards", c.StageRewards);
        }

        // ------------------------------------------------------------------ state (per festival run of MidAutumnVillage = the 10459 event)
        static PlayerStore _store;
        public static PlayerStore StoreOverride;
        public static PlayerStore Store { get { if (StoreOverride != null) return StoreOverride; if (_store == null) _store = new PlayerStore(Path.Combine(DataDir, "players.json")); return _store; } }
        public static Func<int> RunOverride;
        static int RunNo { get { var o = RunOverride; if (o != null) return o(); try { var r = Festivals.FestivalManager.Running(MidAutumn2024.LunarVoyage.Cfg.Festival); return r == null ? 0 : r.Run; } catch { return 0; } } }
        static void Ensure(Row r)
        {
            int run = RunNo;
            if (PlayerStore.I(r, "run", -1) != run) { r.Clear(); r["run"] = run.ToString(CultureInfo.InvariantCulture); r["pts"] = "0"; r["area"] = "0"; r["pos"] = "1"; r["end"] = "000000"; r["hid"] = "000"; r["claims"] = ""; r["c1"] = "0"; }
        }
        static void Set(Row r, string k, int v) { r[k] = v.ToString(CultureInfo.InvariantCulture); }
        static int Pts(Row r) { return PlayerStore.I(r, "pts"); }
        static bool End1(Row r, int area) { var s = PlayerStore.S(r, "end"); return area >= 1 && area <= 6 && s.Length == 6 && s[area - 1] == '1'; }
        static bool End2(Row r, int area) { var s = PlayerStore.S(r, "hid"); return area >= 4 && area <= 6 && s.Length == 3 && s[area - 4] == '1'; }
        public static Row StateOf(uint uid) { return Store.Get(uid); }

        /// <summary>Main window: 1 = ending j (1|2) of the area is achieved.</summary>
        public static int EndingDone(uint uid, int area, int ending)
        {
            var r = Store.Get(uid);
            if (PlayerStore.I(r, "run", -1) != RunNo) return 0;
            return ending == 2 ? (End2(r, area) ? 1 : 0) : (End1(r, area) ? 1 : 0);
        }
        public static int Points(uint uid) { var r = Store.Get(uid); return PlayerStore.I(r, "run", -1) == RunNo ? Pts(r) : 0; }
        public static bool StageRed(uint uid) { var r = Store.Get(uid); return PlayerStore.I(r, "run", -1) == RunNo && Claimable(r, Cfg).Count > 0; }

        /// <summary>Can the rabbit (re)visit this area: earlier areas' first endings done, and this one not finished
        /// (areas 4-6 stay open until their hidden ending; area 6 is always open once reachable - client Mission()).</summary>
        static string AreaBlock(Row r, int area)
        {
            for (int i = 1; i < area; i++) if (!End1(r, i)) return "Complete the previous areas first.";
            if (area < 6 && End1(r, area) && (Endings[area - 1] == 1 || End2(r, area))) return "Jade Rabbit has already completed the exploration of this area. No need to go there again.";// [Sys][4]
            return null;
        }

        // ------------------------------------------------------------------ requests
        public delegate bool PlaceCheck(IFestIo io, out string why);
        public static PlaceCheck PlaceOverride;// harness: map / alive checks
        static bool Place(IFestIo io, out string why)
        {
            var o = PlaceOverride; if (o != null) return o(io, out why);
            why = null;
            var cio = io as ClientIo; if (cio == null) return true;
            var p = cio.Client.Player;
            if (!p.Alive || p.DynamicID != 0 || !Cfg.FromMaps.Contains(p.Map)) { why = "Please return to Twin City first."; return false; }// [Sys][1]
            return true;
        }
        static string Items(IFestIo io) { return "{'" + Crescent + " 0 " + io.Count(Crescent) + "','" + Plenilune + " 0 " + io.Count(Plenilune) + "','" + HeartGrass + " 0 " + io.Count(HeartGrass) + "'}"; }
        static string PtsLua(Row r) { var c = Cfg; int max = c.StagePoints.Length > 0 ? c.StagePoints[c.StagePoints.Length - 1] : 120000; return "{" + Pts(r) + "," + max + "}"; }
        static int Red(Row r) { return Claimable(r, Cfg).Count > 0 ? 1 : 0; }

        /// <summary>GoTo 1..6 from the Lunar Voyage window: the exploration of that area.</summary>
        public static void Open(IFestIo io, int area)
        {
            var cfg = Cfg;
            if (area < 1 || area > 6) return;
            if (io.Level < cfg.MinLevel) { io.Msg("You need to reach Level " + cfg.MinLevel + " to join the Mid-Autumn event."); return; }
            string why;
            if (!Place(io, out why)) { io.Msg(why); return; }
            string block = null;
            var r = Store.Update(io.Uid, row =>
            {
                Ensure(row);
                block = AreaBlock(row, area);
                if (block != null) return Tuple.Create(true, new Row(row));
                if (PlayerStore.I(row, "area") != area) { Set(row, "area", area); Set(row, "pos", 1); }// a new area starts at the Start
                return Tuple.Create(true, new Row(row));
            });
            if (block != null) { io.Msg(block); return; }
            io.Lua(LuaMsg, "Fgui_Exploration_Open|{['Code']=200,['Data']={0," + Items(io) + "," + PtsLua(r) + "," + area + "," + PlayerStore.I(r, "pos") + "," + Red(r) + ",0}}");
        }

        /// <summary>One exploration step: 1 Heart Grass, success (move on) or failure, rewards, Voyage Points.</summary>
        public static void Explore(IFestIo io)
        {
            var cfg = Cfg;
            var r0 = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            int area = PlayerStore.I(r0, "area"), pos = PlayerStore.I(r0, "pos");
            if (area < 1 || area > 6) return;
            if (pos >= 5) { io.Lua(LuaMsg, "Fgui_Exploration_StopExploration|"); return; }// at the Terminus: ClickEndChou
            if (io.Count(HeartGrass) < 1) { io.Lua(LuaMsg, "Fgui_Exploration_StopExploration|"); io.Msg("You lack the necessary items. Complete the Moon Palace dailies to get Heart Grass."); return; }// [Sys][2]
            bool ok = Kit.Next(100) < cfg.SuccessPercent;
            var gives = new List<ItemSpec> { ItemSpec.Parse(ok ? cfg.StepSuccess : cfg.StepFail) };
            if (ok && cfg.ExtraPool.Length > 0 && Kit.Next(100) < cfg.ExtraPercent)
            {
                var w = cfg.ExtraWeights.ToList(); while (w.Count < cfg.ExtraPool.Length) w.Add(0);
                gives.Add(ItemSpec.Parse(cfg.ExtraPool[Kit.Pick(w.Take(cfg.ExtraPool.Length).ToList())]));
            }
            gives = gives.Where(x => x.Id != 0).ToList();
            if (io.FreeSlots() < io.SlotsFor(gives)) { io.Lua(LuaMsg, "Fgui_Exploration_StopExploration|"); io.Msg("Please make some spaces in your item inventory first."); return; }
            if (!io.Take(HeartGrass, 1)) return;
            int add = ok ? cfg.PointsSuccess : cfg.PointsFail;
            var r = Store.Update(io.Uid, row =>
            {
                Ensure(row);
                Set(row, "pts", Pts(row) + add);
                if (ok && PlayerStore.I(row, "area") == area) Set(row, "pos", Math.Min(5, PlayerStore.I(row, "pos") + 1));
                return Tuple.Create(true, new Row(row));
            });
            var given = Kit.GiveAll(io, gives);
            io.Lua(LuaMsg, "Fgui_Exploration_Result|{['Code']=200,['Data']={" + (ok ? 1 : 0) + "," + Items(io) + "," + PtsLua(r) + "," + area + "," + PlayerStore.I(r, "pos") + "," + Red(r) + ",0,"
                + Kit.LuaItems(gives.Select(x => x.Id + " 0 " + x.Count)) + "}}");
            Kit.Log("JadeRabbit", "EXPLORE", io, "area " + area + " " + (ok ? "success" : "failure") + " pos " + PlayerStore.I(r, "pos") + " +" + add + " pts | " + string.Join(", ", given));
        }

        public static Action<IFestIo, int> StoryOverride;// harness
        /// <summary>At the Terminus: the ending (first, or the hidden one of areas 4-6), its rewards, back to the Start.</summary>
        public static void Terminus(IFestIo io)
        {
            var cfg = Cfg;
            var r0 = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            int area = PlayerStore.I(r0, "area");
            if (area < 1 || area > 6 || PlayerStore.I(r0, "pos") < 5) return;
            bool first = !End1(r0, area);
            int ending = 1;
            if (!first && Endings[area - 1] == 2 && !End2(r0, area) && Kit.Next(100) < cfg.HiddenPercent) ending = 2;
            var gives = new List<ItemSpec> { ItemSpec.Parse(cfg.Terminus) };
            if (first && area - 1 < cfg.EndingRewards.Length) gives.AddRange(cfg.EndingRewards[area - 1].Select(ItemSpec.Parse));
            if (ending == 2 && area - 4 < cfg.HiddenRewards.Length) gives.AddRange(cfg.HiddenRewards[area - 4].Select(ItemSpec.Parse));
            gives = gives.Where(x => x.Id != 0).ToList();
            if (io.FreeSlots() < io.SlotsFor(gives)) { io.Msg("Please make some spaces in your item inventory first."); return; }
            bool done = Store.Update(io.Uid, row =>
            {
                Ensure(row);
                if (PlayerStore.I(row, "area") != area || PlayerStore.I(row, "pos") < 5) return Tuple.Create(false, false);// raced / repeated
                Set(row, "pos", 1); Set(row, "pts", Pts(row) + cfg.PointsTerminus);
                var e = PlayerStore.S(row, "end").PadRight(6, '0').ToCharArray(); e[area - 1] = '1'; row["end"] = new string(e);
                if (ending == 2) { var h = PlayerStore.S(row, "hid").PadRight(3, '0').ToCharArray(); h[area - 4] = '1'; row["hid"] = new string(h); }
                return Tuple.Create(true, true);
            });
            if (!done) return;
            var given = Kit.GiveAll(io, gives);
            io.Lua(LuaMsg, "Fgui_Exploration_Ending|{['Code']=200,['Data']={" + area + "," + ending + "," + Kit.LuaItems(gives.Select(x => x.Id + " 0 " + x.Count)) + "}}");
            var r = Store.Get(io.Uid);
            io.Lua(LuaMsg, "Fgui_Exploration_Refresh|{['Code']=200,['Data']={" + Items(io) + "," + PtsLua(r) + "," + Red(r) + ",0,1}}");
            Kit.Log("JadeRabbit", "TERMINUS", io, "area " + area + " ending " + ending + (first ? " (first)" : "") + " | " + string.Join(", ", given));
            if (first)
            {
                var so = StoryOverride;
                if (so != null) so(io, area);
                else { var cio = io as ClientIo; var a = JadeRabbitUi.Find(area); if (cio != null && a != null) JadeRabbitUi.Goto(cio.Client, area, cfg.MinLevel, new HashSet<uint>(cfg.FromMaps)); }
            }
        }

        // ------------------------------------------------------------------ Voyage Point stage rewards (PointsRank 10459)
        static int EveryDue(Row r, Config c) { return Math.Min(c.StageEveryMax, Pts(r) / c.StageEvery); }
        static List<int> Claimable(Row r, Config c)
        {
            var l = new List<int>();
            if (EveryDue(r, c) > PlayerStore.I(r, "c1")) l.Add(1);
            var claimed = Kit.Ints(PlayerStore.S(r, "claims"));
            for (int i = 0; i < c.StagePoints.Length; i++) if (Pts(r) >= c.StagePoints[i] && !claimed.Contains(i + 2)) l.Add(i + 2);
            return l;
        }
        public static string StageData(IFestIo io)
        {
            var c = Cfg;
            var r = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            var claimed = Kit.Ints(PlayerStore.S(r, "claims")).Where(x => x >= 2 && x <= c.StagePoints.Length + 1).OrderBy(x => x).ToList();
            var can = Claimable(r, c);
            var no = Enumerable.Range(1, c.StagePoints.Length + 1).Where(i => !claimed.Contains(i) && !can.Contains(i)).ToList();
            return "{['Code']=200,['Data']={['MsgID']=10459,['Page']=1,['LastDialog']=0,['IsBack']=1,['CurScore']=" + Pts(r)
                + ",['Claimed']={" + Kit.Csv(claimed) + "},['NoClaim']={" + Kit.Csv(no) + "},['CanClaim']={" + Kit.Csv(can) + "}"
                + ",['Frequency']={{1," + PlayerStore.I(r, "c1") + "," + c.StageEveryMax + "}}}}";
        }
        public static void ClaimStage(IFestIo io, int id)
        {
            var c = Cfg;
            if (id < 1 || id > c.StagePoints.Length + 1) return;
            var items = (id == 1 ? new[] { c.StageEveryItem } : c.StageRewards[id - 2]).Select(ItemSpec.Parse).Where(x => x.Id != 0).ToList();
            if (io.FreeSlots() < io.SlotsFor(items)) { io.Msg("Please make some spaces in your item inventory first."); return; }
            bool ok = Store.Update(io.Uid, r =>
            {
                Ensure(r);
                if (!Claimable(r, c).Contains(id)) return Tuple.Create(false, false);
                if (id == 1) Set(r, "c1", PlayerStore.I(r, "c1") + 1);
                else { var l = Kit.Ints(PlayerStore.S(r, "claims")); l.Add(id); r["claims"] = Kit.Csv(l.Distinct().OrderBy(x => x)); }
                return Tuple.Create(true, true);
            });
            if (!ok) { io.Msg("You can`t claim this reward yet."); return; }
            var given = Kit.GiveAll(io, items);
            io.Msg("Congrats! You received " + string.Join(", ", given) + ".");
            io.Lua(LuaMsg, "Fgui_PointsRank_Open|" + StageData(io));
            Kit.Log("JadeRabbit", "STAGE", io, "stage " + id + " | " + string.Join(", ", given));
        }
        public static void Leaderboard(IFestIo io)
        {
            var rows = new List<Tuple<uint, int>>();
            try
            {
                var root = Store.ReadRoot(); // [EventData 2026-10-15] players.json -> EventStore (MySQL)
                var players = FL.Obj(FL.Val(root, "players")); int run = RunNo;
                if (players != null)
                    foreach (var kv in players)
                    {
                        var d = FL.Obj(kv.Value); uint uid; if (d == null || !uint.TryParse(kv.Key, out uid)) continue;
                        if ((int)FL.U(FL.Val(d, "run")) != run) continue;
                        rows.Add(Tuple.Create(uid, (int)FL.U(FL.Val(d, "pts"))));
                    }
            }
            catch { }
            var top = rows.Where(x => x.Item2 >= 5000).OrderByDescending(x => x.Item2).Take(10).ToList();// STR_FGUI_150143_3: local ranking needs 5000
            if (top.Count == 0) { io.Msg("The local ranking requires at least 5000 Voyage Points."); return; }
            io.Msg("Local Voyage Point Rankings:");
            for (int i = 0; i < top.Count; i++)
            {
                string name = "#" + top[i].Item1;
                try { Client.GameClient c; if (Pool.GamePoll.TryGetValue(top[i].Item1, out c) && c.Player != null) name = c.Player.Name; } catch { }
                io.Msg((i + 1) + ". " + name + " - " + top[i].Item2);
            }
        }
    }
}
