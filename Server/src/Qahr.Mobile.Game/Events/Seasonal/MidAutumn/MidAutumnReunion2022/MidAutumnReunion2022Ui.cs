// ============================================================================
//  [MidAutumnReunion2022 2026-10-15] The official 2022 Global Mid-Autumn "Reunion Under Moon" (Hou Yi shoots the Sun
//  Ravens), client msg 10052 tMsg_Info["MidAutumnFestival"] + 10049 tMsg_Info["MidAutumn_Common"] (elixir / stage
//  rewards), lua dialogs luaui\dialog\dlgmidautumnfestival + dlgmidautumcommon (ActivityId 10049). Runs while the festival
//  MidAutumnReunion2022 (Data\Festivals\MidAutumnReunion2022) runs.  PHASE COMPLETE: the whole day loop is playable.
//
//  Official client data used (luaui, decrypted; luaUiRes[1272] / [1273][10049]; Server_Key tMidAutumnArchery2022_DelText):
//    main     DlgShootingSunUi_OpenMain|{Code=100,Data={ViewTip,SunData={10 x {State}},BossData={<=4 x {Index 1..6,State}},
//             TimeEnergy,ActScore,StoveRed,RewardRed,ShineWarsRed,ArrowNum,Makings1..3,ViewGuideIndex}}; 101 over / 102 level.
//    beasts   MidAutumnArchery2022_KillMonster</N>boss -> private Illusion Realm fight; DlgShootingSunUi_KillBossCallFunc
//             {BossIndex,DropItemInfo,ArrowInfo,ScoreNum} (KillBossTip). [NoMonster] when already killed.
//    shoot    _KillBirdRefh -> 1 Sun-chasing Arrow 3336643 -> DlgShootingSunUi_UpdateAnySunData {SunIndex,SunState=1,ArrowNum,
//             ActScore}; the client animates and calls _KillBird</N>tip</N>sun (tip 2 = arrow stopped in the bonus band).
//             Sun 10 = "the first sun" (cannot be shot). Group items = the window's reward preview tRewardItemInfo
//             (group 1..5 = btn_sun4/6/3/2/5): 730004 / 730004 / 3009002 / 3328247 x300 / 723694; + Sun Raven Essences
//             (3336644, RewardTitle); bonus band = "100 - 500 Reunion Points" (ExtraTitle).
//    compass  _TransDay: all beasts expelled first (Trans[1]), 50 Moon Points (ShineWarsTip/LackTimeEnergy) -> next day:
//             new beasts, 10 suns again (AllBossDie "They will respawn after the alternation of day and night").
//    elixir   ZhongQiu2022Npc_OpenAlchemyInterface -> DlgMidAutumnElixir_BindData {ActivityId=10049,Item,Reward,Point,...};
//             10049 ZhongQiu2022Npc_Refine</N>1|10 ("at least 2 materials"; 3-Material / 2-Material pools =
//             MidAutumn_Config[10049].PreviewReward[1]/[2]) -> DlgMidAutumnElixir_RefineData.
//    stages   ZhongQiu2022Cross_OpenRankInterface -> DlgMidAutumnRank_BindData; 10049 ZhongQiu2022Cross_ClimbStageReward
//             </N>10049</N>i, Rank.Total[1..8] at 1000/3000/5000/10000/30000/50000/100000/every 2000 more (luaUiRes Total).
//  DERIVED (all in Data\MidAutumnReunion2022\midautumnreunion2022.json, marked there): which 4 beasts a day holds,
//  beast map/stats/looks, arrows/points per beast, Sun Raven Essences per shot, bonus distribution, Moon Point regen,
//  rounds per day, the night materials (the night window of this client was re-used by Valentine), elixir odds/points.
//  NOT here: global/local ranking mail rewards (cross-server), CP purchases of Moon Points / materials, Moon Palace.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10052 and 10049), tagged [MidAutumnReunion2022 ...].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using Row = System.Collections.Generic.Dictionary<string, string>;

namespace Qahr.Mobile.MidAutumnReunion2022
{
    public static class MidAutumnReunion2022Ui
    {
        public const uint LuaMsg = 10052;   // tMsg_Info["MidAutumnFestival"]
        public const uint CommonMsg = 10049;// tMsg_Info["MidAutumn_Common"] (elixir / stage rewards, ActivityId 10049)
        public const uint Arrow = 3336643, Making1 = 3336640, Making2 = 3336641, Making3 = 3336642, RavenEssence = 3336644;
        public static readonly string[] BeastNames = { "", "Yayu", "Zaochi", "Jiuying", "Dafeng", "Fengxi", "Xiushe" };

        // ------------------------------------------------------------------ config
        public sealed class Config
        {
            public string Festival = "MidAutumnReunion2022";
            public int MinLevel = 80;
            public uint BeastMap = 12176; public ushort BeastX = 0, BeastY = 0; public int BeastMinutes = 10;
            public uint[] BeastTypes = { 996601, 996602, 996603, 996604, 996605, 996606 };
            public int BeastsPerDay = 4;
            public int[] BeastArrows = { 2, 2, 2, 3 };
            public int BeastPoints = 100;
            public string BeastDrop = "3336640 0 1 3";
            public string[] SunGroupItems = { "730004 0 1 3", "730004 0 1 3", "3009002 0 1 0", "3328247 0 300 0", "723694 0 1 3" };
            public string SunEssence = "3336644 0 2 3";
            public int[] BonusPoints = { 100, 200, 300, 400, 500 };
            public int[] BonusWeights = { 40, 25, 18, 10, 7 };
            public int MoonStart = 50, MoonPerHour = 10, MoonMax = 100, CompassCost = 50, DaysPerDay = 3;
            public string[] NightItems = { "3336641 0 2 3", "3336642 0 2 3" };
            public int RefinePoints3 = 30, RefinePoints2 = 20;
            public string[] Pool3 = { "3336639 0 1", "730002 0 1 3 7200 1", "3009100 0 3", "4060001 0 10 3", "3009001 0 1 0 2880 1", "730003 0 1 3 7200 1", "3009102 0 1", "730005 0 1 3 7200 1", "3009002 0 1 0 2880 1", "3314219 0 1 3", "723695 0 1 3", "200725 0 1 3 0 0 0 0 1" };
            public int[] Weights3 = { 2, 140, 150, 150, 120, 80, 120, 12, 40, 5, 10, 1 };
            public string[] Pool2 = { "3336639 0 1", "730002 0 1 3 7200 1", "3009100 0 3", "4060001 0 10 3", "3009001 0 1 0 2880 1", "730003 0 1 3 7200 1", "3009102 0 1", "730005 0 1 3 7200 1", "3009002 0 1 0 2880 1", "3314219 0 1 3", "723694 0 1 3" };
            public int[] Weights2 = { 1, 170, 170, 160, 110, 60, 120, 6, 25, 3, 20 };
            public uint RareItem = 3336639;
            public int[] StagePoints = { 1000, 3000, 5000, 10000, 30000, 50000, 100000 };
            public int StageEvery = 2000;
            public string[][] StageRewards =
            {
                new[] { "730004 0 1 3 7200 1", "3336644 0 1" }, new[] { "4060001 0 100 3", "3336644 0 1" },
                new[] { "3009002 0 1 0 2880 1", "3336644 0 1" }, new[] { "3009104 0 1", "3336644 0 2" },
                new[] { "3328247 0 500", "3336644 0 3" }, new[] { "198045 0 1 3 0 0 0 0 1", "3336644 0 5" },
                new[] { "198055 0 1 3 0 0 0 0 1", "3336644 0 10" }, new[] { "3325756 0 1 0 0 0 0 0 1", "3336644 0 1" },
            };
        }
        static Config _cfg; static string _stamp;
        public static string DataDir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "MidAutumnReunion2022"); } }
        static string CfgFile { get { return Path.Combine(DataDir, "midautumnreunion2022.json"); } }
        public static Config CfgOverride;// harness
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
                    try { MyConsole.WriteLine("[MidAutumnReunion2022] midautumnreunion2022.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }
                }
                return _cfg;
            }
        }
        static int Int(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(int.MaxValue, FL.U(v)); }
        static int[] Ints(Dictionary<string, object> d, string k, int[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => (int)Math.Min(int.MaxValue, FL.U(x))).ToArray(); }
        static string[] Strs(Dictionary<string, object> d, string k, string[] def) { var a = FL.Arr(FL.Val(d, k)); return a == null || a.Count == 0 ? def : a.Select(x => x == null ? "" : x.ToString()).ToArray(); }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            c.MinLevel = Int(d, "minLevel", c.MinLevel);
            c.BeastMap = (uint)Int(d, "beastMap", (int)c.BeastMap); c.BeastX = (ushort)Int(d, "beastX", c.BeastX); c.BeastY = (ushort)Int(d, "beastY", c.BeastY);
            c.BeastMinutes = Int(d, "beastMinutes", c.BeastMinutes);
            c.BeastTypes = Ints(d, "beastTypes", c.BeastTypes.Select(x => (int)x).ToArray()).Select(x => (uint)x).ToArray();
            c.BeastsPerDay = Math.Max(1, Math.Min(4, Int(d, "beastsPerDay", c.BeastsPerDay)));
            c.BeastArrows = Ints(d, "beastArrows", c.BeastArrows); c.BeastPoints = Int(d, "beastPoints", c.BeastPoints);
            c.BeastDrop = FL.Str(d, "beastDrop") ?? c.BeastDrop;
            c.SunGroupItems = Strs(d, "sunGroupItems", c.SunGroupItems); c.SunEssence = FL.Str(d, "sunEssence") ?? c.SunEssence;
            c.BonusPoints = Ints(d, "bonusPoints", c.BonusPoints); c.BonusWeights = Ints(d, "bonusWeights", c.BonusWeights);
            c.MoonStart = Int(d, "moonStart", c.MoonStart); c.MoonPerHour = Int(d, "moonPerHour", c.MoonPerHour); c.MoonMax = Int(d, "moonMax", c.MoonMax);
            c.CompassCost = Int(d, "compassCost", c.CompassCost); c.DaysPerDay = Math.Max(1, Int(d, "daysPerDay", c.DaysPerDay));
            c.NightItems = Strs(d, "nightItems", c.NightItems);
            c.RefinePoints3 = Int(d, "refinePoints3", c.RefinePoints3); c.RefinePoints2 = Int(d, "refinePoints2", c.RefinePoints2);
            c.Pool3 = Strs(d, "pool3", c.Pool3); c.Weights3 = Ints(d, "weights3", c.Weights3);
            c.Pool2 = Strs(d, "pool2", c.Pool2); c.Weights2 = Ints(d, "weights2", c.Weights2);
            c.StagePoints = Ints(d, "stagePoints", c.StagePoints); c.StageEvery = Math.Max(1, Int(d, "stageEvery", c.StageEvery));
            var sr = FL.Arr(FL.Val(d, "stageRewards"));
            if (sr != null && sr.Count > 0) c.StageRewards = sr.Select(x => (FL.Arr(x) ?? new List<object>()).Select(y => y == null ? "" : y.ToString()).ToArray()).ToArray();
        }

        // ------------------------------------------------------------------ state
        static PlayerStore _store;
        public static PlayerStore StoreOverride;// harness
        public static PlayerStore Store { get { if (StoreOverride != null) return StoreOverride; if (_store == null) _store = new PlayerStore(Path.Combine(DataDir, "players.json")); return _store; } }
        public static Func<DateTime> Clock = () => DateTime.Now;
        public static Func<int> RunOverride;// harness: festival run number
        public static Func<bool> RunningOverride;
        public static bool Running { get { var o = RunningOverride; if (o != null) return o(); try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }
        static int RunNo { get { var o = RunOverride; if (o != null) return o(); try { var r = Festivals.FestivalManager.Running(Cfg.Festival); return r == null ? 0 : r.Run; } catch { return 0; } } }

        /// <summary>Day state (beasts / suns / moon points) of the current festival run; a new run starts fresh.</summary>
        static void Ensure(Row r, Config cfg, DateTime now)
        {
            int run = RunNo;
            if (PlayerStore.I(r, "run", -1) != run)
            {
                r.Clear();
                r["run"] = run.ToString(CultureInfo.InvariantCulture);
                r["score"] = "0"; r["claims"] = ""; r["c8"] = "0";
                r["mp"] = cfg.MoonStart.ToString(CultureInfo.InvariantCulture);
                r["mpAt"] = Kit.NowMinutes(now).ToString(CultureInfo.InvariantCulture);
                NewDay(r, cfg);
                r["date"] = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture); r["days"] = "0";
            }
            // Moon Points: + MoonPerHour for every full hour (official "increase a bit every 60 minutes"), up to MoonMax
            long nowM = Kit.NowMinutes(now), at = PlayerStore.L(r, "mpAt", nowM);
            if (nowM - at >= 60)
            {
                long hours = (nowM - at) / 60;
                int mp = PlayerStore.I(r, "mp");
                if (mp < cfg.MoonMax) mp = (int)Math.Min(cfg.MoonMax, mp + hours * cfg.MoonPerHour);
                r["mp"] = mp.ToString(CultureInfo.InvariantCulture);
                r["mpAt"] = (at + hours * 60).ToString(CultureInfo.InvariantCulture);
            }
            string today = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            if (PlayerStore.S(r, "date") != today) { r["date"] = today; r["days"] = "0"; }
        }
        static void NewDay(Row r, Config cfg)
        {
            var all = new List<int> { 1, 2, 3, 4, 5, 6 };
            var pick = new List<int>();
            for (int i = 0; i < cfg.BeastsPerDay && all.Count > 0; i++) { int k = Kit.Next(all.Count); pick.Add(all[k]); all.RemoveAt(k); }
            r["beasts"] = Kit.Csv(pick);
            r["bdead"] = Kit.Csv(pick.Select(x => 0));
            r["suns"] = "0000000000";
            r["pend"] = "0";
            r["killed"] = "0";
        }
        static List<int> Beasts(Row r) { return Kit.Ints(PlayerStore.S(r, "beasts")); }
        static List<int> BDead(Row r) { return Kit.Ints(PlayerStore.S(r, "bdead")); }
        static string Suns(Row r) { var s = PlayerStore.S(r, "suns"); return s.Length == 10 ? s : "0000000000"; }
        static bool AllBeastsDead(Row r) { var d = BDead(r); return d.Count > 0 && d.All(x => x == 1); }
        static int AliveShootable(Row r) { var s = Suns(r); int n = 0; for (int i = 0; i < 9; i++) if (s[i] == '0') n++; return n; }
        public static Row StateOf(uint uid) { return Store.Get(uid); }// harness / admin

        // ------------------------------------------------------------------ entry
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        public static bool NoThrottle;// harness
        static bool Throttled(uint uid)
        {
            if (NoThrottle) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 350) return true;
            _last[uid] = now;
            return false;
        }

        static readonly HashSet<string> Funcs10052 = new HashSet<string>(StringComparer.Ordinal)
        {
            "MidAutumnArchery2022_OpenMainButton", "MidAutumnArchery2022_KillMonster", "MidAutumnArchery2022_TransDay",
            "MidAutumnArchery2022_KillBirdRefh", "MidAutumnArchery2022_KillBird", "ZhongQiu2022Npc_OpenAlchemyInterface",
            "ZhongQiu2022Cross_OpenRankInterface", "MoonFestivalPage_GuideOpen",
        };
        static readonly HashSet<string> Funcs10049 = new HashSet<string>(StringComparer.Ordinal)
        {
            "ZhongQiu2022Npc_Refine", "ZhongQiu2022Npc_ClickPlusMaterial", "ZhongQiu2022Npc_Click", "ZhongQiu2022Npc_LookReward",
            "ZhongQiu2022Cross_OpenRankInterface", "ZhongQiu2022Cross_ClimbStageReward", "ZhongQiu2022Cross_BackLast",
            "ZhongQiu2022Cross_LinkLeaderboard", "MidAutumnArchery2022_OpenMainButton",
        };

        /// <summary>LuaPush.HandleCallback (msg 10052 / 10049). False whenever the festival is off (10052 then goes to
        /// AprilFools2026, which re-uses the same functions in 2026).</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == LuaMsg ? !Funcs10052.Contains(func) : msgId == CommonMsg ? !Funcs10049.Contains(func) : true) return false;
            if (!Running) return false;
            return Handle(new ClientIo(c), msgId, func, args ?? new string[0]);
        }

        public delegate bool FightStarter(IFestIo io, int boss, uint monsterType, Action onKill, out string why);
        public static FightStarter FightOverride;// harness

        public static bool Handle(IFestIo io, uint msgId, string func, string[] args)
        {
            if (func == "MoonFestivalPage_GuideOpen")
            {
                if (Store.Update(io.Uid, r => { bool re = PlayerStore.I(r, "reopen") == 1; if (re) r["reopen"] = "0"; return Tuple.Create(re, re); }))
                    io.Lua(LuaMsg, "DlgShootingSunUi_OpenMain|" + MainData(io));
                return true;
            }
            bool shoot = func == "MidAutumnArchery2022_KillBird" || func == "MidAutumnArchery2022_KillBirdRefh";
            if (func != "MidAutumnArchery2022_KillBird" && Throttled(io.Uid))
            {
                if (shoot) io.Lua(LuaMsg, "DlgShootingSunUi_ResetShootSun|");
                return true;
            }
            var cfg = Cfg;
            if (io.Level < cfg.MinLevel)
            {
                io.Msg("Please upgrade to level " + cfg.MinLevel + " first.");// luaUiRes[1272].Open_error[102]
                return true;
            }
            switch (func)
            {
                case "MidAutumnArchery2022_OpenMainButton": io.Lua(LuaMsg, "DlgShootingSunUi_OpenMain|" + MainData(io)); return true;
                case "MidAutumnArchery2022_KillMonster": KillMonster(io, Arg(args, 0)); return true;
                case "MidAutumnArchery2022_TransDay": TransDay(io); return true;
                case "MidAutumnArchery2022_KillBirdRefh": ShootRefresh(io); return true;
                case "MidAutumnArchery2022_KillBird": ShootClaim(io, Arg(args, 0), Arg(args, 1)); return true;
                case "ZhongQiu2022Npc_OpenAlchemyInterface": io.Lua(CommonMsg, "DlgMidAutumnElixir_BindData|" + ElixirData(io)); return true;
                case "ZhongQiu2022Npc_Refine": Refine(io, Arg(args, 0)); return true;
                case "ZhongQiu2022Npc_Click":
                    Store.Update(io.Uid, r => { r["skip"] = Arg(args, 0) == 1 ? "1" : "0"; return Tuple.Create(true, 0); });
                    return true;
                case "ZhongQiu2022Npc_ClickPlusMaterial":
                    io.Msg("You don`t have enough materials. Please go to Illusion Realm to collect materials via gameplay like Chopping Down Laurel, Grinding the Medicine, and Killing Beasts.");// tZhongQiu2022Npc[Msg][NoMoney]
                    return true;
                case "ZhongQiu2022Npc_LookReward": return true;// the client builds the preview itself
                case "ZhongQiu2022Cross_OpenRankInterface":
                    io.Lua(CommonMsg, "DlgMidAutumnRank_BindData|" + RankData(io, msgId == CommonMsg ? 2 : 1));
                    io.Lua(CommonMsg, "DlgMidAutumnRank_ChangeLabel|1");// live test: this client's BindData stops before its own ChangeLabel("1")
                    return true;
                case "ZhongQiu2022Cross_ClimbStageReward": ClaimStage(io, Arg(args, 1)); return true;
                case "ZhongQiu2022Cross_BackLast":
                    if (Arg(args, 0) == 2) io.Lua(CommonMsg, "DlgMidAutumnElixir_BindData|" + ElixirData(io));
                    else io.Lua(LuaMsg, "DlgShootingSunUi_OpenMain|" + MainData(io));
                    return true;
                case "ZhongQiu2022Cross_LinkLeaderboard": Leaderboard(io); return true;
            }
            return true;
        }
        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse((a[i] ?? "").Trim(), out v) ? v : 0; }

        // ------------------------------------------------------------------ main window
        static int Score(Row r) { return PlayerStore.I(r, "score"); }
        public static string MainData(IFestIo io)
        {
            var cfg = Cfg; var now = Clock();
            var r = Store.Update(io.Uid, row => { Ensure(row, cfg, now); SettlePending(io, row, cfg, false); return Tuple.Create(true, new Row(row)); });
            var sb = new StringBuilder("{['Code']=100,['Data']={['ViewTip']=1,['SunData']={");
            var suns = Suns(r);
            for (int i = 0; i < 10; i++) sb.Append(i > 0 ? "," : "").Append("{['State']=").Append(suns[i] == '1' ? 1 : 0).Append("}");
            sb.Append("},['BossData']={");
            var b = Beasts(r); var d = BDead(r);
            for (int i = 0; i < b.Count; i++) sb.Append(i > 0 ? "," : "").Append("{['Index']=").Append(b[i]).Append(",['State']=").Append(i < d.Count ? d[i] : 0).Append("}");
            int mp = PlayerStore.I(r, "mp");
            sb.Append("},['TimeEnergy']=").Append(mp).Append(",['ActScore']=").Append(Score(r));
            sb.Append(",['StoveRed']=").Append(CanRefine(io) ? 1 : 0);
            sb.Append(",['RewardRed']=").Append(Claimable(r, cfg).Count > 0 ? 1 : 0);
            sb.Append(",['ShineWarsRed']=").Append(AllBeastsDead(r) && mp >= cfg.CompassCost ? 1 : 0);
            sb.Append(",['ArrowNum']=").Append(io.Count(Arrow));
            sb.Append(",['Makings1']=").Append(io.Count(Making1)).Append(",['Makings2']=").Append(io.Count(Making2)).Append(",['Makings3']=").Append(io.Count(Making3));
            return sb.Append(",['ViewGuideIndex']=0}}").ToString();
        }

        // ------------------------------------------------------------------ beasts
        static void KillMonster(IFestIo io, int boss)
        {
            var cfg = Cfg; var now = Clock();
            if (boss < 1 || boss > 6) return;
            var row = Store.Update(io.Uid, r => { Ensure(r, cfg, now); return Tuple.Create(true, new Row(r)); });
            int slot = Beasts(row).IndexOf(boss);
            if (slot < 0) return;// not one of today's beasts: a forged request
            var dead = BDead(row);
            if (slot < dead.Count && dead[slot] == 1) { io.Msg(BeastNames[boss] + " has been killed. Don`t repeat the challenge."); return; }// [NoMonster]
            if (cfg.BeastTypes.Length < boss) { io.Msg("This beast isn`t available."); return; }
            uint type = cfg.BeastTypes[boss - 1];
            int run = PlayerStore.I(row, "run");
            Action onKill = () => BeastKilled(io, boss, run);
            string why;
            var fo = FightOverride;
            bool ok = fo != null ? fo(io, boss, type, onKill, out why) : StartFight(io, type, onKill, out why);
            if (!ok)
            {
                io.Msg(why == "busy" ? "You are already fighting a beast." : "The beasts can`t be challenged right now.");
                if (why != "busy") Kit.Log(cfg.Festival, "BEAST-FAIL", io, BeastNames[boss] + ": " + why);
                return;
            }
            io.Msg(BeastNames[boss] + " is ferocious! Go to kill it!");// [EnterIns][i]
        }
        static bool StartFight(IFestIo io, uint type, Action onKill, out string why)
        {
            var cio = io as ClientIo; why = "no client";
            if (cio == null) return false;
            var cfg = Cfg;
            string fest = cfg.Festival;
            ushort x = cfg.BeastX, y = cfg.BeastY;
            var gm = Pool.ServerMaps[cfg.BeastMap];
            if (gm != null && (x == 0 || y == 0)) { x = (ushort)(gm.bounds.Width / 2); y = (ushort)(gm.bounds.Height / 2); }
            return PrivateFight.Start(cio.Client, cfg.BeastMap, x, y, type, cfg.BeastMinutes, fest, cl => onKill(),
                () => Festivals.FestivalManager.IsRunning(fest), out why);
        }
        /// <summary>The fight's monster died with the player inside: arrows, Beast Core, points; once per beast per day.</summary>
        public static void BeastKilled(IFestIo io, int boss, int run)
        {
            var cfg = Cfg; var now = Clock();
            int res = Store.Update(io.Uid, r =>
            {
                Ensure(r, cfg, now);
                if (PlayerStore.I(r, "run", -1) != run) return Tuple.Create(false, -1);
                var b = Beasts(r); var d = BDead(r);
                int slot = b.IndexOf(boss);
                if (slot < 0 || slot >= d.Count || d[slot] == 1) return Tuple.Create(false, -1);
                d[slot] = 1; r["bdead"] = Kit.Csv(d);
                int k = PlayerStore.I(r, "killed"); r["killed"] = (k + 1).ToString(CultureInfo.InvariantCulture);
                r["score"] = (Score(r) + cfg.BeastPoints).ToString(CultureInfo.InvariantCulture);
                return Tuple.Create(true, k);
            });
            if (res < 0) return;
            int arrows = cfg.BeastArrows.Length == 0 ? 2 : cfg.BeastArrows[Math.Min(res, cfg.BeastArrows.Length - 1)];
            var drop = ItemSpec.Parse(cfg.BeastDrop);
            var arrow = ItemSpec.Of(Arrow, (uint)Math.Max(0, arrows));
            var given = GiveOrKeep(io, new List<ItemSpec> { drop, arrow });
            io.Lua(LuaMsg, "DlgShootingSunUi_KillBossCallFunc|{['Data']={['BossIndex']=" + boss + ",['DropItemInfo']='" + drop.Id + " 0 " + drop.Count + "',['ArrowInfo']='" + Arrow + " 0 " + arrow.Count + "',['ScoreNum']=" + cfg.BeastPoints + "}}");
            Kit.Log(cfg.Festival, "BEAST", io, BeastNames[boss] + " +" + cfg.BeastPoints + " pts, " + string.Join(", ", given));
        }

        /// <summary>Gives what fits now; the rest is stored and paid the next time the window opens (never lost, never doubled).</summary>
        static List<string> GiveOrKeep(IFestIo io, List<ItemSpec> items)
        {
            var given = new List<string>(); var keep = new List<ItemSpec>();
            foreach (var it in items)
            {
                if (it.Id == 0 || it.Count == 0) continue;
                if (io.FreeSlots() >= io.SlotsFor(new[] { it }) && io.Give(it)) given.Add(it.ToString()); else keep.Add(it);
            }
            if (keep.Count > 0)
            {
                Store.Update(io.Uid, r => { var s = PlayerStore.S(r, "owed"); r["owed"] = (s.Length > 0 ? s + ";" : "") + string.Join(";", keep.Select(k => k.Lua())); return Tuple.Create(true, 0); });
                io.Msg("Your bag is full: the rest of the reward is kept until you open the window with free space.");
            }
            return given;
        }
        static void PayOwed(IFestIo io, Row r)
        {
            var s = PlayerStore.S(r, "owed");
            if (s.Length == 0) return;
            var left = new List<string>();
            foreach (var p in s.Split(';'))
            {
                var it = ItemSpec.Parse(p);
                if (it.Id == 0) continue;
                if (io.FreeSlots() >= io.SlotsFor(new[] { it }) && io.Give(it)) io.Msg("You received " + it + ".");
                else left.Add(p);
            }
            r["owed"] = string.Join(";", left);
        }

        // ------------------------------------------------------------------ shooting
        static void ShootRefresh(IFestIo io)
        {
            var cfg = Cfg; var now = Clock();
            // settle an unclaimed shot first (window closed during the animation)
            var row = Store.Update(io.Uid, r => { Ensure(r, cfg, now); SettlePending(io, r, cfg, false); return Tuple.Create(true, new Row(r)); });
            if (AliveShootable(row) == 0) { io.Lua(LuaMsg, "DlgShootingSunUi_ResetShootSun|"); io.Msg("All Sun Ravens have been shot down."); return; }// [KillAll]
            if (io.Count(Arrow) < 1) { io.Lua(LuaMsg, "DlgShootingSunUi_ResetShootSun|"); io.Msg("Failed to shoot down the sun foam lack of Sun-chasing Arrows."); return; }// [NoArrow]
            if (!io.Take(Arrow, 1)) { io.Lua(LuaMsg, "DlgShootingSunUi_ResetShootSun|"); return; }// deduct before anything is granted
            int sun = Store.Update(io.Uid, r =>
            {
                var s = Suns(r).ToCharArray();
                var alive = new List<int>(); for (int i = 0; i < 9; i++) if (s[i] == '0') alive.Add(i + 1);
                if (alive.Count == 0) return Tuple.Create(false, 0);
                int pick = alive[Kit.Next(alive.Count)];
                s[pick - 1] = '1'; r["suns"] = new string(s); r["pend"] = pick.ToString(CultureInfo.InvariantCulture);
                return Tuple.Create(true, pick);
            });
            if (sun == 0) { io.Give(ItemSpec.Of(Arrow, 1)); io.Lua(LuaMsg, "DlgShootingSunUi_ResetShootSun|"); return; }
            var after = Store.Get(io.Uid);
            io.Lua(LuaMsg, "DlgShootingSunUi_UpdateAnySunData|{['Data']={['SunIndex']=" + sun + ",['SunState']=1,['ArrowNum']=" + io.Count(Arrow) + ",['ActScore']=" + Score(after) + "}}");
            Kit.Log(cfg.Festival, "SHOOT", io, "sun " + sun + " (arrow taken)");
        }
        static void ShootClaim(IFestIo io, int tip, int sun)
        {
            var cfg = Cfg; var now = Clock();
            bool paid = Store.Update(io.Uid, r =>
            {
                Ensure(r, cfg, now);
                int pend = PlayerStore.I(r, "pend");
                if (pend == 0 || pend != sun) return Tuple.Create(false, false);// nothing pending / not the shot sun: forged or repeated
                SettlePending(io, r, cfg, tip == 2);
                return Tuple.Create(true, true);
            });
            if (!paid) return;
            var after = Store.Get(io.Uid);
            io.Lua(LuaMsg, "DlgShootingSunUi_ShootingSunAfter|{['Data']={['ActScore']=" + Score(after) + ",['ArrowNum']=" + io.Count(Arrow) + "}}");
        }
        public static int Group(int sun) { return sun >= 9 ? 5 : (sun + 1) / 2; }// suns 1-2 group 1 ... 7-8 group 4, 9 group 5
        /// <summary>Pays the pending shot (called under the store lock). bonus = arrow stopped in the bonus band.</summary>
        static void SettlePending(IFestIo io, Row r, Config cfg, bool bonus)
        {
            PayOwed(io, r);
            int pend = PlayerStore.I(r, "pend");
            if (pend <= 0) return;
            r["pend"] = "0";
            int g = Group(pend);
            var items = new List<ItemSpec>();
            if (g - 1 < cfg.SunGroupItems.Length) items.Add(ItemSpec.Parse(cfg.SunGroupItems[g - 1]));
            items.Add(ItemSpec.Parse(cfg.SunEssence));
            int extra = 0;
            if (bonus && cfg.BonusPoints.Length > 0) extra = cfg.BonusPoints[Math.Min(cfg.BonusPoints.Length - 1, Kit.Pick(cfg.BonusWeights))];
            if (extra > 0) r["score"] = (Score(r) + extra).ToString(CultureInfo.InvariantCulture);
            var given = new List<string>(); var keep = new List<string>();
            foreach (var it in items)
            {
                if (it.Id == 0 || it.Count == 0) continue;
                if (io.FreeSlots() >= io.SlotsFor(new[] { it }) && io.Give(it)) given.Add(it.ToString()); else keep.Add(it.Lua());
            }
            if (keep.Count > 0)
            {
                var s = PlayerStore.S(r, "owed"); r["owed"] = (s.Length > 0 ? s + ";" : "") + string.Join(";", keep);
                io.Msg("Your bag is full: the rest of the reward is kept until you open the window with free space.");
            }
            if (given.Count > 0) io.Msg("Congrats! You received " + string.Join(", ", given) + (extra > 0 ? " and " + extra + " Reunion Points" : "") + ".");
            else if (extra > 0) io.Msg("Congrats! You received " + extra + " Reunion Points.");
            Kit.Log(cfg.Festival, "SUN", io, "sun " + pend + " group " + g + (extra > 0 ? " bonus " + extra : "") + " | " + string.Join(", ", given) + (keep.Count > 0 ? " | kept " + string.Join(";", keep) : ""));
        }

        // ------------------------------------------------------------------ Illusion Compass
        static void TransDay(IFestIo io)
        {
            var cfg = Cfg; var now = Clock();
            string err = Store.Update(io.Uid, r =>
            {
                Ensure(r, cfg, now);
                SettlePending(io, r, cfg, false);
                if (!AllBeastsDead(r)) return Tuple.Create(true, "Please expel all beasts before using the Illusion Compass.");// [Trans][1]
                if (PlayerStore.I(r, "mp") < cfg.CompassCost) return Tuple.Create(true, "Your Moon Points are lower than " + cfg.CompassCost + ". The Moon Points will increase a bit every 60 minutes.");// [NoValue2] (no CP option)
                if (PlayerStore.I(r, "days") >= cfg.DaysPerDay) return Tuple.Create(true, "You have reached the limit today. Please try tomorrow.");// [ChgLimit]
                r["mp"] = (PlayerStore.I(r, "mp") - cfg.CompassCost).ToString(CultureInfo.InvariantCulture);
                r["days"] = (PlayerStore.I(r, "days") + 1).ToString(CultureInfo.InvariantCulture);
                NewDay(r, cfg);
                r["reopen"] = "1";
                // the night (Moon Palace: laurel / medicine) passes: its materials (derived, the night window isn't in this client)
                var night = cfg.NightItems.Select(ItemSpec.Parse).Where(x => x.Id != 0).ToList();
                var keep = new List<string>();
                foreach (var it in night) if (!(io.FreeSlots() >= io.SlotsFor(new[] { it }) && io.Give(it))) keep.Add(it.Lua());
                if (keep.Count > 0) { var s = PlayerStore.S(r, "owed"); r["owed"] = (s.Length > 0 ? s + ";" : "") + string.Join(";", keep); }
                Kit.Log(cfg.Festival, "COMPASS", io, "-" + cfg.CompassCost + " Moon Points, night: " + string.Join(", ", night) + (keep.Count > 0 ? " (kept)" : ""));
                if (night.Count > 0) io.Msg("The night passed in Illusion Realm. You received " + string.Join(", ", night) + ".");
                return Tuple.Create(true, (string)null);
            });
            if (err != null) { io.Msg(err); return; }
            io.Lua(LuaMsg, "DlgShootingSunUi_ChangeNight|");// closing animation, then the client calls MoonFestivalPage_GuideOpen -> new day
        }

        // ------------------------------------------------------------------ elixir
        static List<uint> Mats(IFestIo io, uint n) { var l = new List<uint>(); foreach (var m in new[] { Making1, Making2, Making3 }) if (io.Count(m) >= n) l.Add(m); return l; }
        static bool CanRefine(IFestIo io) { return Mats(io, 1).Count >= 2; }
        public static string ElixirData(IFestIo io)
        {
            var cfg = Cfg; var r = Store.Get(io.Uid);
            var sb = new StringBuilder("{['Code']=200,['Data']={['ActivityId']=10049,['Item']={");
            var mats = new[] { Making1, Making2, Making3 };
            for (int i = 0; i < 3; i++) sb.Append(i > 0 ? "," : "").Append("{['Id']='").Append(mats[i]).Append(" 0 1',['Num']=").Append(io.Count(mats[i])).Append("}");
            sb.Append("},['Reward']=").Append(Kit.LuaItems(cfg.Pool3));
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 100000;
            sb.Append(",['Point']={").Append(Score(r)).Append(",").Append(top).Append("}");
            sb.Append(",['IsBack']=1,['IsMust']=0,['IsTime']=0,['IsRule']=1,['IsSkip']=").Append(PlayerStore.I(r, "skip"));
            sb.Append(",['BtnOn']={").Append(Mats(io, 1).Count >= 2 ? 1 : 0).Append(",").Append(Mats(io, 10).Count >= 2 ? 1 : 0).Append("}");
            sb.Append(",['Red']=").Append(Claimable(r, cfg).Count > 0 ? 1 : 0).Append(",['IsFirst']=0}}");
            return sb.ToString();
        }
        static void Refine(IFestIo io, int times)
        {
            var cfg = Cfg;
            if (times != 1 && times != 10) return;
            var mats = Mats(io, (uint)times);
            if (mats.Count < 2) { io.Msg("Please put in at least 2 materials."); return; }// luaUiRes[1273][10049].Dialog
            bool three = mats.Count == 3;
            var pool = three ? cfg.Pool3 : cfg.Pool2; var w = (three ? cfg.Weights3 : cfg.Weights2).ToList();
            while (w.Count < pool.Length) w.Add(0);
            var draws = new List<ItemSpec>();
            for (int i = 0; i < times; i++) draws.Add(ItemSpec.Parse(pool[Kit.Pick(w.Take(pool.Length).ToList())]));
            if (io.FreeSlots() < io.SlotsFor(draws)) { io.Msg("Please make some spaces in your item inventory first."); return; }// [Msg][NoSpace]
            var taken = new List<uint>();
            foreach (var m in mats)
            {
                if (io.Take(m, (uint)times)) { taken.Add(m); continue; }
                foreach (var t in taken) io.Give(ItemSpec.Of(t, (uint)times));// a lost race: give back what was taken, nothing paid
                io.Msg("You don`t have enough materials."); return;
            }
            int pts = (three ? cfg.RefinePoints3 : cfg.RefinePoints2) * times;
            int skip = 0;
            Store.Update(io.Uid, r => { Ensure(r, cfg, Clock()); r["score"] = (Score(r) + pts).ToString(CultureInfo.InvariantCulture); skip = PlayerStore.I(r, "skip"); return Tuple.Create(true, 0); });
            var given = Kit.GiveAll(io, draws);
            foreach (var d in draws) if (d.Id == cfg.RareItem) BroadcastRare(io);
            io.Lua(CommonMsg, "DlgMidAutumnElixir_RefineData|{['Code']=200,['Data']={['ActivityId']=10049,['Type']=" + (times == 10 ? 4 : 3) + ",['IsSkip']=" + skip + ",['IsExtra']=0,['Reward']=" + Kit.LuaItems(draws.Select(x => x.Id + " 0 " + x.Count)) + "}}");
            io.Lua(CommonMsg, "DlgMidAutumnElixir_BindData|" + ElixirData(io));
            Kit.Log(cfg.Festival, "REFINE", io, (three ? "3" : "2") + "-material x" + times + " -" + string.Join("/", mats) + " +" + pts + " pts | " + string.Join(", ", given));
        }
        public static Action<string> BroadcastOverride;
        static void BroadcastRare(IFestIo io)
        {
            string t = "Hero " + io.Name + " is super lucky to draw the rare Immortality Pill!";// tZhongQiu2022Npc[Msg][Broad]
            var o = BroadcastOverride; if (o != null) { o(t); return; }
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    Pool.SendGlobalPackets.Enqueue(new MsgMessage(t, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(rec.GetStream()));
            }
            catch { }
        }

        // ------------------------------------------------------------------ stage rewards (Total points reach ...)
        static int RepeatsDue(int score, Config cfg)
        {
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 100000;
            return score <= top ? 0 : (score - top) / cfg.StageEvery;
        }
        static List<int> Claimable(Row r, Config cfg)
        {
            var l = new List<int>(); var claimed = Kit.Ints(PlayerStore.S(r, "claims")); int sc = Score(r);
            for (int i = 0; i < cfg.StagePoints.Length; i++) if (sc >= cfg.StagePoints[i] && !claimed.Contains(i + 1)) l.Add(i + 1);
            int rep = cfg.StagePoints.Length + 1;
            if (cfg.StageRewards.Length >= rep && RepeatsDue(sc, cfg) > PlayerStore.I(r, "c8")) l.Add(rep);
            return l;
        }
        public static string RankData(IFestIo io, int last)
        {
            var cfg = Cfg; var now = Clock();
            var r = Store.Update(io.Uid, row => { Ensure(row, cfg, now); return Tuple.Create(true, new Row(row)); });
            var claimed = Kit.Ints(PlayerStore.S(r, "claims")).Where(x => x >= 1 && x <= cfg.StagePoints.Length).OrderBy(x => x).ToList();
            int n = Math.Min(cfg.StageRewards.Length, cfg.StagePoints.Length + 1);
            var no = Enumerable.Range(1, n).Where(i => !claimed.Contains(i)).ToList();
            var can = Claimable(r, cfg);
            int top = cfg.StagePoints.Length > 0 ? cfg.StagePoints[cfg.StagePoints.Length - 1] : 100000;
            return "{['Code']=200,['Data']={['ActivityId']=10049,['LastDialog']=" + last + ",['IsOpen']=1,['IsBack']=1,['IsTime']=0,['IsRule']=0,['RankNum']=2"
                + ",['Claimed']={" + Kit.Csv(claimed) + "},['NoClaim']={" + Kit.Csv(no) + "},['CanClaim']={" + Kit.Csv(can) + "}"
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
                if (!Claimable(r, cfg).Contains(idx)) return Tuple.Create(false, false);
                if (idx == cfg.StagePoints.Length + 1) r["c8"] = (PlayerStore.I(r, "c8") + 1).ToString(CultureInfo.InvariantCulture);
                else { var c = Kit.Ints(PlayerStore.S(r, "claims")); c.Add(idx); r["claims"] = Kit.Csv(c.Distinct().OrderBy(x => x)); }
                return Tuple.Create(true, true);// recorded BEFORE the reward
            });
            if (!ok) { io.Msg("You can`t claim this reward yet."); return; }
            var given = Kit.GiveAll(io, items);
            io.Msg("Congrats! You received " + string.Join(", ", given) + ".");
            Kit.Log(cfg.Festival, "STAGE", io, "stage " + idx + " | " + string.Join(", ", given));
            io.Lua(CommonMsg, "DlgMidAutumnRank_UpdateReward|" + RankData(io, 1));
            io.Lua(CommonMsg, "DlgMidAutumnRank_ChangeLabel|1");
        }
        static void Leaderboard(IFestIo io)
        {
            // local Reunion Point rankings from the real per-player state of this run (top 10)
            var rows = new List<Tuple<uint, int>>();
            try
            {
                var root = Store.ReadRoot(); // [EventData 2026-10-15] players.json -> EventStore (MySQL)
                var players = FL.Obj(FL.Val(root, "players"));
                int run = RunNo;
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
            if (top.Count == 0) { io.Msg("Local Reunion Point Rankings: nobody has points yet."); return; }
            io.Msg("Local Reunion Point Rankings:");
            for (int i = 0; i < top.Count; i++) io.Msg((i + 1) + ". " + NameOf(top[i].Item1) + " - " + top[i].Item2);
        }
        static string NameOf(uint uid) { try { Client.GameClient c; if (Pool.GamePoll.TryGetValue(uid, out c) && c.Player != null) return c.Player.Name; } catch { } return "#" + uid; }
    }
}
