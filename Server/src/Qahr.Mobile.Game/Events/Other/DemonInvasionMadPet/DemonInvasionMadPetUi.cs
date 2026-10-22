// ============================================================================
//  [DemonInvasionMadPet 2026-10-15] the official Mid-Autumn 2023 "Hewing Laurel" (Moon Palace cultivation) - PHASE COMPLETE.
//  Client: luaui\dialog\dlgmidautumnfestival\dlgfelllaurel* + dlg23midautumntravel* (Mystic Travel), Dialog_Info 1425 / 1410,
//  button DlgFellLaurelBtn, tMsg_Info["MidAutumnFestival"] = 10052; Cultivation stage rewards = DlgMidAutumnRank ActivityId 10295
//  (CallServer 10295). Texts luaUiRes[1425] / [1410] / [1273][10295]. Runs while the festival DemonInvasionMadPet runs.
//
//  Playable now (all rules / items from the client; numbers it lacks are DERIVED in Infinity[DB]\DemonInvasionMadPet\config.json):
//    hew      MidTree2023_ChouType</N>1|10: 1 Lunar Spirit Pill 3600628 per hew (rule 2) -> 1 treasure of "Normal Hewing Rewards"
//             (DlgFellLaurelPreReward tReward1) + 15 Cultivation; Divine Axe (rule 3/4): an attempt instead of a pill -> "Divine Axe
//             Hewing Rewards" (tReward2) + 45 Cultivation. Reply DlgFellLaurelUi_PlayFellTree {FellTreeList={'item,tier'}}; the client
//             calls MidTree2023_AwardItemShow after the animation -> DlgFellLaurelUi_UpdateMain.
//    axe      MidTree2023_GreatYaoOpen -> DlgFellLaurelUse_OpenMain (Giant Pill 3345019, "x%d to boost hewing state %d time(s)");
//             the confirm calls MidTree2023_GreatYaoUse</N>n (server-chosen callback) -> attempts.
//    phases   tForce (44 phases, client) / lvtitle; MidTree2023_NextYurang -> DlgFellLaurelUi_OpenPreRewardBack {ItemList};
//             MidTree2023_RankAwardget claims the reached phase (once each).
//    stages   MidAutumnKua_OpenJiFen -> DlgMidAutumnRank_BindData ActivityId 10295 (Total[1..10] official items, thresholds
//             500..80000 then every 2000 up to 500 times); msg 10295 MidAutumnKua_Phase</N>10295</N>i / _OpenJiFenReturn.
//    travel   ZQExplorePage_OpenMainView -> Dlg23MidautumnTravel (2 strength per travel, rule 2; Mooncake 3345018 restores strength,
//             rule 3; rewards = Dialog_Info["1410-preview-config"][10295]); ZQExplorePage_Draw</N>1|2, _MooncakeView/_MooncakeUse.
//    pills    the "Daily Sign-in Pack" (luaUiRes[1425].itemget1[1]) = Lunar Spirit Pills once a day on the first window open.
//  Not here (polite message, nothing taken): the dream bubbles (quests 5580/5581: instances 1002422/24/25 unresolved), Moon
//  Devourer (CS), Vault Palace, the Moon exchange shop and CP Mall (no item table in the client), celestial-being travel events.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10052 + 10295), tagged [DemonInvasionMadPet ...].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.MidAutumnReunion2022;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using Row = System.Collections.Generic.Dictionary<string, string>;

namespace Qahr.Mobile.DemonInvasionMadPet
{
    public static class DemonInvasionMadPetUi
    {
        public const uint LuaMsg = 10052;// tMsg_Info["MidAutumnFestival"]
        public const uint RankMsg = 10295;// MidAutumn_Config[10295].Config.CallServer
        public const string Festival = "DemonInvasionMadPet";
        public const uint LunarSpiritPill = 3600628, GiantPill = 3345019, Mooncake = 3345018;
        /// <summary>tForce: Cultivation needed for each phase (client dlgfelllaurelui).</summary>
        public static readonly int[] Force = { 15, 100, 180, 230, 280, 330, 380, 430, 480, 530, 580, 630, 680, 730, 780, 830, 880, 930, 980, 1030, 1160,
            1300, 1450, 1600, 1750, 1900, 2050, 2200, 2350, 2500, 2650, 2800, 2950, 3100, 3250, 3400, 3550, 3700, 3850, 4000, 4150, 4300, 4450, 4600 };

        public sealed class Config
        {
            public int MinLevel = 1;
            public int HewPoints = 15, DivinePoints = 45;// luaUiRes[1425].rule (official)
            public string[] NormalPool = { "3344998 0 1", "3345055 0 1", "3342771 0 1", "3009102 0 1", "3342773 0 1", "3342777 0 1", "3009101 0 1", "3314211 0 4 3", "3344996 0 2", "3342770 0 1", "3345021 0 10", "3344997 0 5", "3345018 0 1" };
            public int[] NormalWeights = { 60, 3, 50, 25, 60, 30, 70, 90, 70, 110, 120, 80, 232 };
            public int[] NormalTiers = { 2, 6, 3, 4, 3, 4, 2, 1, 2, 1, 1, 2, 1 };
            public string[] DivinePool = { "3342774 0 1", "3345055 0 1", "3344999 0 1", "3340960 0 1", "4060001 0 16 3", "3345022 0 9", "3328247 0 45", "4060001 0 13 3", "3342771 0 1", "3009102 0 1", "3345018 0 1" };
            public int[] DivineWeights = { 90, 15, 60, 50, 110, 140, 120, 130, 120, 70, 95 };
            public int[] DivineTiers = { 3, 6, 5, 5, 3, 4, 3, 2, 2, 3, 1 };
            public int AttemptsPerGiantPill = 1;
            public int DailyPills = 10;
            public string[][] PhaseRewards = null;// null = built from the pools (see BuildPhaseRewards)
            public int[] StagePoints = { 500, 1000, 2500, 4500, 6000, 10000, 30000, 50000, 80000 };
            public int StageEvery = 2000, StageRepeatMax = 500;
            public string[][] StageRewards =
            {
                new[] { "3321077 0 2 3", "3009101 0 1", "730003 0 1 3 7200 1" },
                new[] { "3002926 0 2", "3009101 0 1", "3009001 0 2 0 2880 1", "3345055 0 1" },
                new[] { "3303373 0 2", "3009101 0 1", "3009001 0 2 0 2880 1", "3345055 0 1" },
                new[] { "3322761 0 1 3 7200 1", "3009101 0 2", "730004 0 1 3 2880 1", "3345055 0 1" },
                new[] { "3322762 0 1 3 7200 1", "3009102 0 1", "4060001 0 40 3", "3345055 0 1" },
                new[] { "730005 0 2 3 7200 1", "3345055 0 1" },
                new[] { "3342255 0 5", "3345055 0 2" },
                new[] { "3342255 0 6", "3009103 0 1", "3345055 0 3" },
                new[] { "3342255 0 8", "3009104 0 1", "3345055 0 5" },
                new[] { "3009103 0 1", "3345055 0 1" },
            };
            public int TravelCost = 2, DailyStrength = 20, StrengthMax = 200, StrengthPerMooncake = 2, TravelPoints = 10;
            public string[] TravelPool = { "3342776 0 1", "3314250 0 1 3", "3342775 0 1", "3345048 0 1", "3314248 0 1 3", "3314207 0 1 3", "3009103 0 1", "3345055 0 1", "3345021 0 65", "3009102 0 2", "3345021 0 50", "3342773 0 2", "3342771 0 1", "3321077 0 3 3", "3345021 0 35" };
            public int[] TravelWeights = { 4, 6, 5, 3, 6, 8, 6, 3, 40, 60, 80, 130, 150, 199, 300 };
        }
        static Config _cfg; static string _stamp;
        public static Config CfgOverride;
        static string DataDir { get { return Path.Combine(Program.ServerConfig.DbLocation, "DemonInvasionMadPet"); } }
        static string CfgFile { get { return Path.Combine(DataDir, "config.json"); } }
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
                    try { MyConsole.WriteLine("[DemonInvasionMadPet] config.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static int Int(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(int.MaxValue, FL.U(v)); }
        static int[] Ints(Dictionary<string, object> d, string k, int[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => (int)Math.Min(int.MaxValue, FL.U(x))).ToArray(); }
        static string[] Strs(Dictionary<string, object> d, string k, string[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => x == null ? "" : x.ToString()).ToArray(); }
        static string[][] Nested(Dictionary<string, object> d, string k, string[][] def)
        {
            var a = FL.Arr(FL.Val(d, k)); if (a == null || a.Count == 0) return def;
            return a.Select(x => (FL.Arr(x) ?? new List<object>()).Select(y => y == null ? "" : y.ToString()).ToArray()).ToArray();
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            c.MinLevel = Int(d, "minLevel", c.MinLevel);
            c.HewPoints = Int(d, "hewPoints", c.HewPoints); c.DivinePoints = Int(d, "divinePoints", c.DivinePoints);
            c.NormalPool = Strs(d, "normalPool", c.NormalPool); c.NormalWeights = Ints(d, "normalWeights", c.NormalWeights); c.NormalTiers = Ints(d, "normalTiers", c.NormalTiers);
            c.DivinePool = Strs(d, "divinePool", c.DivinePool); c.DivineWeights = Ints(d, "divineWeights", c.DivineWeights); c.DivineTiers = Ints(d, "divineTiers", c.DivineTiers);
            c.AttemptsPerGiantPill = Math.Max(1, Int(d, "attemptsPerGiantPill", c.AttemptsPerGiantPill));
            c.DailyPills = Int(d, "dailyPills", c.DailyPills);
            c.PhaseRewards = Nested(d, "phaseRewards", c.PhaseRewards);
            c.StagePoints = Ints(d, "stagePoints", c.StagePoints); c.StageEvery = Math.Max(1, Int(d, "stageEvery", c.StageEvery)); c.StageRepeatMax = Int(d, "stageRepeatMax", c.StageRepeatMax);
            c.StageRewards = Nested(d, "stageRewards", c.StageRewards);
            c.TravelCost = Math.Max(1, Int(d, "travelCost", c.TravelCost)); c.DailyStrength = Int(d, "dailyStrength", c.DailyStrength);
            c.StrengthMax = Int(d, "strengthMax", c.StrengthMax); c.StrengthPerMooncake = Math.Max(1, Int(d, "strengthPerMooncake", c.StrengthPerMooncake));
            c.TravelPoints = Int(d, "travelPoints", c.TravelPoints);
            c.TravelPool = Strs(d, "travelPool", c.TravelPool); c.TravelWeights = Ints(d, "travelWeights", c.TravelWeights);
        }
        /// <summary>Phase i (1..44) reward when the config has none: items of the two official hewing pools, growing with the
        /// phase (DERIVED - the client sends the list from the server).</summary>
        public static string[] PhaseReward(Config cfg, int phase)
        {
            if (cfg.PhaseRewards != null && phase >= 1 && phase <= cfg.PhaseRewards.Length) return cfg.PhaseRewards[phase - 1];
            if (phase <= 6) return new[] { "3345018 0 2", "3342770 0 1" };
            if (phase <= 12) return new[] { "3345018 0 3", "3342771 0 1", "3345021 0 10" };
            if (phase <= 18) return new[] { "3342773 0 1", "3009101 0 1", "3345021 0 20" };
            if (phase <= 26) return new[] { "3342774 0 1", "3009102 0 1", "3345022 0 9" };
            if (phase <= 34) return new[] { "3344999 0 1", "3328247 0 45", "3345022 0 9" };
            return new[] { "3340960 0 1", "4060001 0 16 3", "3345055 0 1" };
        }

        // ------------------------------------------------------------------ state
        static PlayerStore _store;
        public static PlayerStore StoreOverride;
        public static PlayerStore Store { get { if (StoreOverride != null) return StoreOverride; if (_store == null) _store = new PlayerStore(Path.Combine(DataDir, "players.json")); return _store; } }
        public static Func<DateTime> Clock = () => DateTime.Now;
        public static Func<bool> RunningOverride; public static Func<int> RunOverride;
        public static bool Running { get { var o = RunningOverride; if (o != null) return o(); try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }
        static int RunNo { get { var o = RunOverride; if (o != null) return o(); try { var r = Festivals.FestivalManager.Running(Festival); return r == null ? 0 : r.Run; } catch { return 0; } } }
        static string Today { get { return Clock().ToString("yyyyMMdd", CultureInfo.InvariantCulture); } }
        static void Ensure(Row r)
        {
            int run = RunNo;
            if (PlayerStore.I(r, "run", -1) != run) { r.Clear(); r["run"] = run.ToString(CultureInfo.InvariantCulture); r["score"] = "0"; r["lv"] = "0"; r["great"] = "0"; r["claims"] = ""; r["c10"] = "0"; r["str"] = "0"; }
        }
        static void Set(Row r, string k, int v) { r[k] = v.ToString(CultureInfo.InvariantCulture); }
        static int Score(Row r) { return PlayerStore.I(r, "score"); }
        public static Row StateOf(uint uid) { return Store.Get(uid); }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        public static bool NoThrottle;
        static bool Throttled(uint uid, int ms)
        {
            if (NoThrottle) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }

        /// <summary>LuaPush.HandleCallback (msg 10052 / 10295). On 10052 only this window's functions: the other Mid-Autumn years
        /// share the msg.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool own = msgId == RankMsg ? func.StartsWith("MidAutumnKua_", StringComparison.Ordinal)
                : func.StartsWith("MidTree2023_", StringComparison.Ordinal) || func == "MidAutumnDreamlandAct_Enter";
            bool shared = msgId == LuaMsg && (func.StartsWith("MidAutumnKua_", StringComparison.Ordinal) || func.StartsWith("ZQExplorePage_", StringComparison.Ordinal));
            if (!own && !shared) return false;
            if (!Running)
            {
                if (!own) return false;
                if (!Throttled(c.Player.UID, 400)) { try { c.SendSysMesage("Hewing Laurel has ended. See you next Mid-Autumn!", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { } }
                return true;
            }
            return Handle(new ClientIo(c), msgId, func, args ?? new string[0]);
        }

        public static bool Handle(IFestIo io, uint msgId, string func, string[] args)
        {
            if (func == "MidTree2023_AwardItemShow") { io.Lua(LuaMsg, "DlgFellLaurelUi_UpdateMain|" + MainData(io)); return true; }
            if (Throttled(io.Uid, 300)) return true;
            var cfg = Cfg;
            if (io.Level < cfg.MinLevel) { io.Msg("Please upgrade to level " + cfg.MinLevel + " first."); return true; }
            switch (func)
            {
                case "MidTree2023_OpenCutTree": DailyPack(io); io.Lua(LuaMsg, "DlgFellLaurelUi_OpenMain|" + MainData(io)); return true;
                case "MidTree2023_ChouType": Hew(io, Arg(args, 0)); return true;
                case "MidTree2023_GreatYaoOpen": AxeView(io); return true;
                case "MidTree2023_GreatYaoUse": UseGiantPills(io, Arg(args, 0)); return true;
                case "MidTree2023_NextYurang": NextPhase(io); return true;
                case "MidTree2023_RankAwardget": ClaimPhase(io); return true;
                case "MidAutumnDreamlandAct_Enter": io.Msg("The dream bubbles aren`t open on this server yet."); return true;
                case "MidAutumnKua_OpenJiFen":
                    io.Lua(RankMsg, "DlgMidAutumnRank_BindData|" + RankData(io, Arg(args, 0) == 2 ? 2 : 1));
                    io.Lua(RankMsg, "DlgMidAutumnRank_ChangeLabel|1");// this client's BindData stops before its own ChangeLabel("1")
                    return true;
                case "MidAutumnKua_Phase": ClaimStage(io, Arg(args, 1)); return true;
                case "MidAutumnKua_OpenJiFenReturn":
                    if (Arg(args, 0) == 2) OpenTravel(io); else io.Lua(LuaMsg, "DlgFellLaurelUi_OpenMain|" + MainData(io));
                    return true;
                case "MidAutumnKua_LinkRank": Leaderboard(io); return true;
                case "MidAutumnKua_OpenDuiHuan": case "MidAutumnKua_TianShiShop": io.Msg("This shop isn`t open on this server yet."); return true;
                case "MidAutumnKua_KillYueGong": io.Msg("Moon Devourer (CS) isn`t open on this server yet."); return true;
                case "MidAutumnKua_XunLi": io.Msg("Vault Palace isn`t open on this server yet."); return true;
                case "MidAutumnKua_QianDaoLiBao": DailyPack(io); return true;
                case "ZQExplorePage_OpenMainView": OpenTravel(io); return true;
                case "ZQExplorePage_Draw": Travel(io, Arg(args, 0)); return true;
                case "ZQExplorePage_MooncakeView": MooncakeView(io); return true;
                case "ZQExplorePage_MooncakeUse": UseMooncakes(io, Arg(args, 0)); return true;
                case "ZQExplorePage_EnterThing": case "ZQExplorePage_ClientEndThing": io.Msg("The celestial beings are traveling elsewhere now."); return true;
            }
            if (func.StartsWith("ZQExplorePage_", StringComparison.Ordinal) || func.StartsWith("MidAutumnKua_", StringComparison.Ordinal)) { io.Msg("This isn`t open on this server yet."); return true; }
            return true;
        }
        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse((a[i] ?? "").Trim(), out v) ? v : 0; }

        // ------------------------------------------------------------------ main window
        static int ForceBefore(int lv) { int n = 0; for (int i = 0; i < lv && i < Force.Length; i++) n += Force[i]; return n; }
        static bool PhaseReady(Row r) { int lv = PlayerStore.I(r, "lv"); return lv < Force.Length && Score(r) >= ForceBefore(lv + 1); }
        public static string MainData(IFestIo io)
        {
            var cfg = Cfg;
            var r = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            int lv = PlayerStore.I(r, "lv");
            int role = Math.Min(5, 1 + lv / 9);// DERIVED: the hewer changes as the phases grow (Chang`e .. Hou Yi, luaUiRes[1278][10301])
            return "{['Code']=200,['Data']={['Score']=" + Score(r) + ",['Lv']=" + lv + ",['ShowPopList']={0,0,0,0,0},['LeftItem']='"
                + LunarSpiritPill + " 0 " + io.Count(LunarSpiritPill) + "',['RighttItem']='" + GiantPill + " 0 " + io.Count(GiantPill)
                + "',['GreatNum']=" + PlayerStore.I(r, "great") + ",['RedList']={" + (StageClaimable(r, cfg).Count > 0 ? 1 : 0) + "," + (Strength(r, cfg) >= cfg.TravelCost ? 1 : 0) + "},['CurRole']=" + role + "}}";
        }
        static void DailyPack(IFestIo io)
        {
            var cfg = Cfg;
            if (cfg.DailyPills <= 0) return;
            var pack = ItemSpec.Of(LunarSpiritPill, (uint)cfg.DailyPills, true);
            if (io.FreeSlots() < io.SlotsFor(new[] { pack })) return;// tried again on the next open
            bool give = Store.Update(io.Uid, r => { Ensure(r); if (PlayerStore.S(r, "pack") == Today) return Tuple.Create(false, false); r["pack"] = Today; return Tuple.Create(true, true); });
            if (!give) return;
            if (io.Give(pack)) io.Msg("Daily Sign-in Pack: you received " + pack + ".");
            Kit.Log(Festival, "SIGNIN", io, pack.ToString());
        }

        // ------------------------------------------------------------------ hewing
        static void Hew(IFestIo io, int n)
        {
            var cfg = Cfg;
            if (n != 1 && n != 10) return;
            var r0 = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            int great = Math.Min(n, PlayerStore.I(r0, "great"));
            int pills = n - great;
            if (io.Count(LunarSpiritPill) < (uint)pills) { io.Msg("You need " + pills + " Lunar Spirit Pill(s) to hew the laurel tree."); return; }
            // draw first (nothing taken yet), check space, then take, record, give
            var list = new List<Tuple<ItemSpec, int>>();
            for (int i = 0; i < n; i++)
            {
                bool divine = i < great;
                var pool = divine ? cfg.DivinePool : cfg.NormalPool; var w = (divine ? cfg.DivineWeights : cfg.NormalWeights).ToList(); var t = divine ? cfg.DivineTiers : cfg.NormalTiers;
                while (w.Count < pool.Length) w.Add(0);
                int k = Kit.Pick(w.Take(pool.Length).ToList());
                list.Add(Tuple.Create(ItemSpec.Parse(pool[k]), Math.Max(1, Math.Min(6, k < t.Length ? t[k] : 1))));
            }
            if (io.FreeSlots() < io.SlotsFor(list.Select(x => x.Item1))) { io.Msg("Please make some spaces in your item inventory first."); return; }
            if (pills > 0 && !io.Take(LunarSpiritPill, (uint)pills)) { io.Msg("You need " + pills + " Lunar Spirit Pill(s) to hew the laurel tree."); return; }
            int pts = great * cfg.DivinePoints + pills * cfg.HewPoints;
            bool ok = Store.Update(io.Uid, r =>
            {
                Ensure(r);
                if (PlayerStore.I(r, "great") < great) return Tuple.Create(false, false);// raced: attempts used meanwhile
                Set(r, "great", PlayerStore.I(r, "great") - great); Set(r, "score", Score(r) + pts);
                return Tuple.Create(true, true);
            });
            if (!ok) { if (pills > 0) io.Give(ItemSpec.Of(LunarSpiritPill, (uint)pills, true)); return; }
            var given = Kit.GiveAll(io, list.Select(x => x.Item1));
            io.Lua(LuaMsg, "DlgFellLaurelUi_PlayFellTree|{['Data']={['FellTreeList']=" + Kit.LuaItems(list.Select(x => x.Item1.Id + " 0 " + x.Item1.Count + "," + x.Item2)) + "}}");
            Kit.Log(Festival, "HEW", io, "x" + n + " (divine " + great + ", pills " + pills + ") +" + pts + " Cultivation | " + string.Join(", ", given));
        }
        static void AxeView(IFestIo io)
        {
            var cfg = Cfg;
            uint have = io.Count(GiantPill);
            if (have == 0) { io.Msg("You don`t have any Giant Pills. Giant Pills can be obtained through the CP Mall and Anima exchange."); return; }// luaUiRes[1278][10301][13]
            io.Lua(LuaMsg, "DlgFellLaurelUse_OpenMain|{['Data']={['ItemInfo']='" + GiantPill + " 0 " + have + "," + cfg.AttemptsPerGiantPill
                + "',['tSureFunc']={['ViewId']=" + LuaMsg + ",['FuncName']='MidTree2023_GreatYaoUse'}}}");
        }
        static void UseGiantPills(IFestIo io, int n)
        {
            var cfg = Cfg;
            if (n < 1 || n > 9999) return;
            if (io.Count(GiantPill) < (uint)n) { io.Msg("You don`t have enough Giant Pills."); return; }
            if (!io.Take(GiantPill, (uint)n)) return;
            int add = n * cfg.AttemptsPerGiantPill;
            Store.Update(io.Uid, r => { Ensure(r); Set(r, "great", PlayerStore.I(r, "great") + add); return Tuple.Create(true, 0); });
            io.Msg("The Divine Axe state is active: " + add + " attempt(s).");
            io.Lua(LuaMsg, "DlgFellLaurelUi_UpdateMain|" + MainData(io));
            Kit.Log(Festival, "AXE", io, "-" + n + " Giant Pill, +" + add + " attempts");
        }

        // ------------------------------------------------------------------ cultivation phases
        static void NextPhase(IFestIo io)
        {
            var cfg = Cfg; var r = Store.Get(io.Uid); int lv = PlayerStore.I(r, "lv");
            if (lv >= Force.Length) return;
            io.Lua(LuaMsg, "DlgFellLaurelUi_OpenPreRewardBack|{['Data']={['ItemList']=" + Kit.LuaItems(PhaseReward(cfg, lv + 1)) + "}}");
        }
        static void ClaimPhase(IFestIo io)
        {
            var cfg = Cfg;
            var r0 = Store.Get(io.Uid); int lv0 = PlayerStore.I(r0, "lv");
            if (lv0 >= Force.Length) return;
            var items = PhaseReward(cfg, lv0 + 1).Select(ItemSpec.Parse).Where(x => x.Id != 0).ToList();
            if (io.FreeSlots() < io.SlotsFor(items)) { io.Msg("Please make some spaces in your item inventory first."); return; }
            int got = Store.Update(io.Uid, r =>
            {
                Ensure(r); int lv = PlayerStore.I(r, "lv");
                if (lv != lv0 || !PhaseReady(r)) return Tuple.Create(false, 0);
                Set(r, "lv", lv + 1); return Tuple.Create(true, lv + 1);// recorded before the reward
            });
            if (got == 0) { io.Msg("Your Cultivation hasn`t reached the next phase yet."); return; }
            var given = Kit.GiveAll(io, items);
            io.Msg("Congrats! You reached a new phase and received " + string.Join(", ", given) + ".");
            io.Lua(LuaMsg, "DlgFellLaurelUi_UpdateMain|" + MainData(io));
            Kit.Log(Festival, "PHASE", io, "phase " + got + " | " + string.Join(", ", given));
        }

        // ------------------------------------------------------------------ Cultivation stage rewards (ActivityId 10295)
        static int RepeatsDue(int score, Config cfg)
        {
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 80000;
            return score <= top ? 0 : Math.Min(cfg.StageRepeatMax, (score - top) / cfg.StageEvery);
        }
        static List<int> StageClaimable(Row r, Config cfg)
        {
            var l = new List<int>(); var claimed = Kit.Ints(PlayerStore.S(r, "claims")); int sc = Score(r);
            for (int i = 0; i < cfg.StagePoints.Length; i++) if (sc >= cfg.StagePoints[i] && !claimed.Contains(i + 1)) l.Add(i + 1);
            int rep = cfg.StagePoints.Length + 1;
            if (cfg.StageRewards.Length >= rep && RepeatsDue(sc, cfg) > PlayerStore.I(r, "c10")) l.Add(rep);
            return l;
        }
        public static string RankData(IFestIo io, int last)
        {
            var cfg = Cfg;
            var r = Store.Update(io.Uid, row => { Ensure(row); return Tuple.Create(true, new Row(row)); });
            var claimed = Kit.Ints(PlayerStore.S(r, "claims")).Where(x => x >= 1 && x <= cfg.StagePoints.Length).OrderBy(x => x).ToList();
            int n = Math.Min(cfg.StageRewards.Length, cfg.StagePoints.Length + 1);
            var no = Enumerable.Range(1, n).Where(i => !claimed.Contains(i)).ToList();
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 80000;
            return "{['Code']=200,['Data']={['ActivityId']=10295,['LastDialog']=" + last + ",['IsOpen']=1,['IsBack']=1,['IsTime']=0,['IsRule']=0,['RankNum']=2"
                + ",['Claimed']={" + Kit.Csv(claimed) + "},['NoClaim']={" + Kit.Csv(no) + "},['CanClaim']={" + Kit.Csv(StageClaimable(r, cfg)) + "}"
                + ",['IsPoint']=1,['PointProgress']={" + Score(r) + "," + top + "}}}";
        }
        static void ClaimStage(IFestIo io, int idx)
        {
            var cfg = Cfg;
            if (idx < 1 || idx > cfg.StageRewards.Length || idx > cfg.StagePoints.Length + 1) return;
            var items = cfg.StageRewards[idx - 1].Select(ItemSpec.Parse).Where(x => x.Id != 0).ToList();
            if (io.FreeSlots() < io.SlotsFor(items)) { io.Msg("Please make some spaces in your item inventory first."); return; }
            bool ok = Store.Update(io.Uid, r =>
            {
                Ensure(r);
                if (!StageClaimable(r, cfg).Contains(idx)) return Tuple.Create(false, false);
                if (idx == cfg.StagePoints.Length + 1) Set(r, "c10", PlayerStore.I(r, "c10") + 1);
                else { var c = Kit.Ints(PlayerStore.S(r, "claims")); c.Add(idx); r["claims"] = Kit.Csv(c.Distinct().OrderBy(x => x)); }
                return Tuple.Create(true, true);
            });
            if (!ok) { io.Msg("You can`t claim this reward yet."); return; }
            var given = Kit.GiveAll(io, items);
            io.Msg("Congrats! You received " + string.Join(", ", given) + ".");
            io.Lua(RankMsg, "DlgMidAutumnRank_UpdateReward|" + RankData(io, 1));
            io.Lua(RankMsg, "DlgMidAutumnRank_ChangeLabel|1");
            Kit.Log(Festival, "STAGE", io, "stage " + idx + " | " + string.Join(", ", given));
        }
        static void Leaderboard(IFestIo io)
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
                        rows.Add(Tuple.Create(uid, (int)FL.U(FL.Val(d, "score"))));
                    }
            }
            catch { }
            var top = rows.Where(x => x.Item2 > 0).OrderByDescending(x => x.Item2).Take(10).ToList();
            if (top.Count == 0) { io.Msg("Cultivation Rankings: nobody has Cultivation Points yet."); return; }
            io.Msg("Cultivation Rankings:");
            for (int i = 0; i < top.Count; i++) io.Msg((i + 1) + ". " + NameOf(top[i].Item1) + " - " + top[i].Item2);
        }
        static string NameOf(uint uid) { try { Client.GameClient c; if (Pool.GamePoll.TryGetValue(uid, out c) && c.Player != null) return c.Player.Name; } catch { } return "#" + uid; }

        // ------------------------------------------------------------------ Mystic Travel
        static int Strength(Row r, Config cfg)
        {
            int s = PlayerStore.I(r, "str");
            if (PlayerStore.S(r, "strDay") != Today) s = Math.Min(cfg.StrengthMax, s + cfg.DailyStrength);// the daily strength (DERIVED)
            return s;
        }
        static void RollStrength(Row r, Config cfg) { if (PlayerStore.S(r, "strDay") != Today) { Set(r, "str", Strength(r, cfg)); r["strDay"] = Today; } }
        static string TravelData(IFestIo io)
        {
            var cfg = Cfg;
            var r = Store.Update(io.Uid, row => { Ensure(row); RollStrength(row, cfg); return Tuple.Create(true, new Row(row)); });
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 80000;
            return "{['Code']=200,['Data']={['Point']={" + Score(r) + "," + top + "},['Num']={" + PlayerStore.I(r, "str") + "," + io.Count(Mooncake) + "},['Cost']={"
                + cfg.TravelCost + "," + cfg.TravelCost * 10 + "},['Red']=" + (StageClaimable(r, cfg).Count > 0 ? 1 : 0) + ",['IconIndex']={},['IconFight']={}}}";
        }
        static void OpenTravel(IFestIo io)
        {
            io.Lua(LuaMsg, "Dlg23MidautumnTravel_BindData|");
            io.Lua(LuaMsg, "Dlg23MidautumnTravel_UpdateData|" + TravelData(io));
        }
        static void Travel(IFestIo io, int mode)
        {
            var cfg = Cfg;
            int times = mode == 2 ? 10 : mode == 1 ? 1 : 0;
            if (times == 0) return;
            int cost = cfg.TravelCost * times;
            var w = cfg.TravelWeights.ToList(); while (w.Count < cfg.TravelPool.Length) w.Add(0);
            var draws = new List<ItemSpec>();
            for (int i = 0; i < times; i++) draws.Add(ItemSpec.Parse(cfg.TravelPool[Kit.Pick(w.Take(cfg.TravelPool.Length).ToList())]));
            if (io.FreeSlots() < io.SlotsFor(draws)) { io.Msg("Please make some spaces in your item inventory first."); return; }
            int pts = cfg.TravelPoints * times;
            bool ok = Store.Update(io.Uid, r =>
            {
                Ensure(r); RollStrength(r, cfg);
                int s = PlayerStore.I(r, "str");
                if (s < cost) return Tuple.Create(false, false);
                Set(r, "str", s - cost); Set(r, "score", Score(r) + pts);
                return Tuple.Create(true, true);
            });
            if (!ok) { io.Msg("You don`t have enough strength. Eating Mooncakes can replenish your strength."); return; }// luaUiRes[1410].Rule 3
            var given = Kit.GiveAll(io, draws);
            io.Lua(LuaMsg, "Dlg23MidautumnTravel_UpdateReward|{['Code']=200,['Data']={['Reward']=" + Kit.LuaItems(draws.Select(x => x.Id + " 0 " + x.Count)) + "}}");
            io.Lua(LuaMsg, "Dlg23MidautumnTravel_UpdateData|" + TravelData(io));
            Kit.Log(Festival, "TRAVEL", io, "x" + times + " -" + cost + " strength +" + pts + " Cultivation | " + string.Join(", ", given));
        }
        static void MooncakeView(IFestIo io)
        {
            uint have = io.Count(Mooncake);
            if (have == 0) { io.Msg("You don`t have any Mooncakes. Hewing the laurel tree yields Mooncakes."); return; }
            io.Lua(LuaMsg, "Dlg23MidautumnUse_BindData|{['Code']=200,['Data']={['Item']='" + Mooncake + " 0 1',['MaxNum']=" + have + ",['LimitNum']=-1}}");
        }
        static void UseMooncakes(IFestIo io, int n)
        {
            var cfg = Cfg;
            if (n < 1 || n > 9999) return;
            if (io.Count(Mooncake) < (uint)n) { io.Msg("You don`t have enough Mooncakes."); return; }
            int room = Store.Update(io.Uid, r => { Ensure(r); RollStrength(r, cfg); return Tuple.Create(true, cfg.StrengthMax - PlayerStore.I(r, "str")); });
            int maxUse = room / cfg.StrengthPerMooncake;
            if (maxUse <= 0) { io.Msg("Your strength is full."); return; }
            n = Math.Min(n, maxUse);
            if (!io.Take(Mooncake, (uint)n)) return;
            int add = n * cfg.StrengthPerMooncake;
            Store.Update(io.Uid, r => { Set(r, "str", Math.Min(cfg.StrengthMax, PlayerStore.I(r, "str") + add)); return Tuple.Create(true, 0); });
            io.Msg("You ate " + n + " Mooncake(s) and recovered " + add + " strength.");
            io.Lua(LuaMsg, "Dlg23MidautumnTravel_UpdateData|" + TravelData(io));
            Kit.Log(Festival, "MOONCAKE", io, "-" + n + " Mooncake, +" + add + " strength");
        }
    }
}
