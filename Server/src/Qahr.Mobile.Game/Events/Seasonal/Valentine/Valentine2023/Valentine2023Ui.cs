// ============================================================================
//  [Valentine2023 2026-10-14/15] Valentine 2023 "Starlight Recital" (Feb. 14 - Feb. 27, luaUiRes_EN[1334].ActTime),
//  client tMsg_Info["DlgValentinesDay"] = 10139 (shared msgId: CastleMystery owns the Lover2025Kua_* calls on it,
//  this class only the ValentinesDayConcert_* / ValentineDay2022Npc_* calls of the 2023 window).
//  Shown while the festival Valentine2023 runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui\dialog\dlgvalentinesday dlgloverday*, luaUiRes_EN[1334], Server_Key tValentinesDayConcert_DelText):
//    button   DlgLoverDayBtn, shown with DlgValentinesDay_OpenMainBtn|{Code=200,Data={Rewardflag}}; click -> OpenMainButton.
//    main     DlgLoverDayMain_OpenMain|{Code=100,Data={ViewTip,SunData={10 x {State}},BossData={4 x {Index 1-6,State}},
//             TimeEnergy,ActScore,StoveRed,RewardRed,ShineWarsRed,ArrowNum,Makings1,ViewGuideIndex}}; 101 over / 102 level.
//    calls    ValentinesDayConcert_KillMonster </N>spiritIndex (1-6)  -> the spirit appears and is fought (EnterIns texts)
//             ValentinesDayConcert_KillBird </N>rewardTip(1|2)</N>0  -> 1 Performance Inspiration lights 1 of the 10 notes
//             ValentinesDayConcert_TransDay                           -> Time Shift (50 Time Power, ShineWarsTip)
//             ValentineDay2022Npc_ScoreReward </N>1                    -> Ardor Points rewards (no table in this client)
//    replies  DlgLoverDayMain_UpdateAnySunData|{Data={SunIndex,SunState,ArrowNum,ActScore}}, DlgLoverDayMain_ResetShootSun,
//             DlgLoverDayMain_KillBossCallFunc|{Data={BossIndex,DropItemInfo,ArrowInfo,ScoreNum}} (KillBossTip).
//
//  Playable here (numbers: Infinity[DB]\Valentine2023\recital.json, "derived" where the client has none):
//    - 4 of the 6 instrument spirits per Recital day; a click spawns that spirit (monster types 996301-996306, Festivals\
//      Valentine2023\monsters, derived looks) next to the player in Twin City (official NoMap text). Kill credit = the spawn
//      owner with the top damage (Boss=1 keeps per-player damage). Reward: Performance Inspiration + Dream Note + points.
//    - the 10 notes: each costs 1 Performance Inspiration (taken first), lights a random unlit note, gives 1 Dream Note +
//      that note's official preview item (tRewardItemInfo: notes 2-4 Stone 730004, 5-7 RadiantStarStone 3009002, 8-9
//      Awakening Spirit x300 3328247, 10 PermanentStone 723694) + points; the "Extra" zone adds 100-500 Ardor Points (official).
//    - Time Power: official max 100 / Time Shift cost 50; regen per hour + start value derived; Time Berries (3600449) in the
//      bag are used automatically when power is short (daily limit derived, official Limit text).
//    - Time Shift: all 4 spirits must be defeated (official Trans[1]); it starts a new Recital day (new spirits, dark notes).
//      Golden Masquerade (the night half, Modern Plaza) is not on this server: the shift skips it (derived).
//  State: Infinity[DB]\Valentine2023\state.json (per character, per event year). Every grant checks bag space first and is
//  recorded before it is given; requests are throttled per player.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10139, after CastleMystery), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Valentine2023
{
    public static class Valentine2023Ui
    {
        public const uint LuaMsg = 10139;
        public const string Festival = "Valentine2023";
        public const int MinLevel = 80;// luaUiRes_EN[1334].Open_error[102]
        public const uint PerformanceInspiration = 3340630, LoveCandy = 3600448, DreamNote = 3340595, TimeBerry = 3600449;
        public const uint TwinCity = 1002;
        public static readonly string[] SpiritNames = { "Piano Spirit", "Violin Spirit", "Cello Spirit", "Flute Spirit", "Clarinet Spirit", "Horn Spirit" };
        public static readonly string[] SpiritTypes = { "PianoSpirit", "ViolinSpirit", "CelloSpirit", "FluteSpirit", "ClarinetSpirit", "HornSpirit" };
        public static readonly string[] EnterText = {// Server_Key tValentinesDayConcert_DelText[EnterIns][1-6]
            "Piano Spirit made a gorgeous appearance!", "Violin Spirit made a gorgeous appearance.", "Cello Spirit made a gorgeous appearance.",
            "Flute Spirit made a gorgeous appearance.", "Clarinet Spirit made a gorgeous appearance.", "Horn Spirit made a gorgeous appearance." };

        /// <summary>Test seams (null on the server).</summary>
        public static Func<DateTime> ClockOverride;
        public static Func<bool> RunningOverride;
        public static Func<Client.GameClient, int, Game.MsgMonster.MonsterRole> SpawnOverride;
        public static Func<int, int> RandomOverride;// (n) -> 0..n-1
        static DateTime Now { get { var f = ClockOverride; return f != null ? f() : DateTime.Now; } }
        public static bool Running { get { var o = RunningOverride; if (o != null) return o(); try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        // ------------------------------------------------------------------ config (recital.json)
        public sealed class Config
        {
            public int SpiritInspiration = 3, SpiritPoints = 100, SpiritDreamNotes = 1, SpiritMinutes = 10;
            public int NoteDreamNotes = 1, NotePoints = 100, ExtraMin = 100, ExtraMax = 500;
            public int PowerMax = 100, PowerStart = 50, PowerPerHour = 5, ShiftCost = 50, BerryDailyLimit = 20;
            public int ShootCooldownMs = 1500;
            // official preview tRewardItemInfo by note position (1 = "No special rewards"): id, count, bound
            public uint[][] NoteItems = {
                null,
                new uint[] { 730004, 1, 1 }, new uint[] { 730004, 1, 1 }, new uint[] { 730004, 1, 1 },
                new uint[] { 3009002, 1, 0 }, new uint[] { 3009002, 1, 0 }, new uint[] { 3009002, 1, 0 },
                new uint[] { 3328247, 300, 0 }, new uint[] { 3328247, 300, 0 },
                new uint[] { 723694, 1, 1 } };
        }
        static Config _cfg; static string _cfgStamp;
        static string DataDir { get { return Path.Combine(Program.ServerConfig.DbLocation, Festival); } }
        public static Config Cfg
        {
            get
            {
                string f = Path.Combine(DataDir, "recital.json");
                try
                {
                    var fi = new FileInfo(f);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _cfgStamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists)
                    {
                        var d = Festivals.FestivalLoader.ReadJson(f);
                        Func<string, int, int> I = (k, def) => { var v = Festivals.FestivalLoader.Val(d, k); return v == null ? def : (int)Math.Min(int.MaxValue, Festivals.FestivalLoader.U(v)); };
                        c.SpiritInspiration = Clamp(I("spiritInspiration", c.SpiritInspiration), 1, 10);
                        c.SpiritPoints = Clamp(I("spiritPoints", c.SpiritPoints), 0, 10000);
                        c.SpiritDreamNotes = Clamp(I("spiritDreamNotes", c.SpiritDreamNotes), 0, 10);
                        c.SpiritMinutes = Clamp(I("spiritMinutes", c.SpiritMinutes), 1, 60);
                        c.NoteDreamNotes = Clamp(I("noteDreamNotes", c.NoteDreamNotes), 0, 10);
                        c.NotePoints = Clamp(I("notePoints", c.NotePoints), 0, 10000);
                        c.ExtraMin = Clamp(I("extraMin", c.ExtraMin), 0, 10000);
                        c.ExtraMax = Clamp(I("extraMax", c.ExtraMax), c.ExtraMin, 10000);
                        c.PowerMax = Clamp(I("powerMax", c.PowerMax), 1, 1000);
                        c.PowerStart = Clamp(I("powerStart", c.PowerStart), 0, c.PowerMax);
                        c.PowerPerHour = Clamp(I("powerPerHour", c.PowerPerHour), 0, 1000);
                        c.ShiftCost = Clamp(I("shiftCost", c.ShiftCost), 0, c.PowerMax);
                        c.BerryDailyLimit = Clamp(I("berryDailyLimit", c.BerryDailyLimit), 0, 1000);
                        c.ShootCooldownMs = Clamp(I("shootCooldownMs", c.ShootCooldownMs), 300, 60000);
                    }
                    _cfg = c; _cfgStamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Valentine2023] recital.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }
        static int Clamp(int v, int lo, int hi) { return v < lo ? lo : v > hi ? hi : v; }

        // ------------------------------------------------------------------ per-character state (state.json)
        public sealed class PState
        {
            public uint Uid; public int Season;
            public int[] Spirits = new int[4];// spirit index 1-6 per slot
            public int DeadMask;               // bit slot
            public int NoteMask;               // bit (note-1)
            public int Power; public long PowerTicks;// last regen time
            public int Score;
            public int BerryDay, BerryUsed;
            public int Days;                   // Recital days played (Time Shifts + 1)
        }
        static readonly object _lock = new object();
        static Dictionary<uint, PState> _players;
        static bool _dirty;
        static string StateFile { get { return Path.Combine(DataDir, "state.json"); } }
        static readonly Random _rnd = new Random();
        static int Rnd(int n) { var o = RandomOverride; if (o != null) return Clamp(o(n), 0, n - 1); lock (_rnd) return _rnd.Next(n); }
        static int Season() { var r = Festivals.FestivalManager.Running(Festival); return (r != null ? r.Started : Now).Year; }

        static void NewDay(PState p)
        {
            var pool = new List<int> { 1, 2, 3, 4, 5, 6 };
            for (int i = 0; i < 4; i++) { int k = Rnd(pool.Count); p.Spirits[i] = pool[k]; pool.RemoveAt(k); }// derived: the official server picks them
            p.DeadMask = 0; p.NoteMask = 0; p.Days++;
        }
        public static PState State(Client.GameClient c)
        {
            lock (_lock)
            {
                if (_players == null) Load();
                int s = Season();
                PState p;
                if (!_players.TryGetValue(c.Player.UID, out p) || p.Season != s)
                {
                    p = new PState { Uid = c.Player.UID, Season = s, Power = Cfg.PowerStart, PowerTicks = Now.Ticks };
                    NewDay(p);
                    _players[p.Uid] = p; _dirty = true;
                }
                Regen(p);
                return p;
            }
        }
        static void Regen(PState p)
        {
            var cfg = Cfg;
            var now = Now;
            if (p.PowerTicks <= 0 || p.PowerTicks > now.Ticks) { p.PowerTicks = now.Ticks; _dirty = true; return; }
            long hours = (now.Ticks - p.PowerTicks) / TimeSpan.TicksPerHour;
            if (hours <= 0) return;
            p.PowerTicks += hours * TimeSpan.TicksPerHour;
            if (p.Power < cfg.PowerMax) p.Power = (int)Math.Min(cfg.PowerMax, p.Power + hours * cfg.PowerPerHour);
            _dirty = true;
        }
        static void Load()
        {
            _players = new Dictionary<uint, PState>();
            try
            {
                if (!File.Exists(StateFile)) return;
                var d = Festivals.FestivalLoader.ReadJson(StateFile);
                var list = Festivals.FestivalLoader.Val(d, "players") as List<object>;
                if (list == null) return;
                foreach (var o in list)
                {
                    var e = o as Dictionary<string, object>; if (e == null) continue;
                    Func<string, int> I = k => (int)Math.Min(int.MaxValue, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, k)));
                    var p = new PState { Uid = Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, "uid")), Season = I("season"), DeadMask = I("dead") & 15, NoteMask = I("notes") & 1023,
                        Power = I("power"), PowerTicks = (long)Festivals.FestivalLoader.UL(Festivals.FestivalLoader.Val(e, "powerTicks")), Score = I("score"), BerryDay = I("berryDay"), BerryUsed = I("berryUsed"), Days = I("days") };
                    var sp = Festivals.FestivalLoader.Val(e, "spirits") as List<object>;
                    bool ok = sp != null && sp.Count == 4;
                    for (int i = 0; ok && i < 4; i++) { p.Spirits[i] = (int)Festivals.FestivalLoader.U(sp[i]); if (p.Spirits[i] < 1 || p.Spirits[i] > 6) ok = false; }
                    if (!ok) NewDay(p);
                    if (p.Uid != 0) _players[p.Uid] = p;
                }
            }
            catch (Exception ex) { try { MyConsole.WriteLine("[Valentine2023] state.json: " + ex.Message, ConsoleColor.Yellow); } catch { } }
        }
        public static void Save()
        {
            lock (_lock)
            {
                if (_players == null) return;
                try
                {
                    var sb = new StringBuilder("{\"_readme\":\"Written by the GameServer [Valentine2023] Starlight Recital. Do not edit while the server runs.\",\"players\":[");
                    bool first = true;
                    foreach (var p in _players.Values)
                    {
                        if (!first) sb.Append(','); first = false;
                        sb.Append("{\"uid\":").Append(p.Uid).Append(",\"season\":").Append(p.Season).Append(",\"spirits\":[").Append(string.Join(",", p.Spirits)).Append("]")
                          .Append(",\"dead\":").Append(p.DeadMask).Append(",\"notes\":").Append(p.NoteMask).Append(",\"power\":").Append(p.Power).Append(",\"powerTicks\":").Append(p.PowerTicks)
                          .Append(",\"score\":").Append(p.Score).Append(",\"berryDay\":").Append(p.BerryDay).Append(",\"berryUsed\":").Append(p.BerryUsed).Append(",\"days\":").Append(p.Days).Append('}');
                    }
                    sb.Append("]}");
                    Directory.CreateDirectory(DataDir);
                    string f = StateFile, tmp = f + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
                    if (File.Exists(f)) File.Replace(tmp, f, null); else File.Move(tmp, f);
                    _dirty = false;
                }
                catch (Exception e) { try { MyConsole.WriteLine("[Valentine2023] save failed: " + e.Message, ConsoleColor.Yellow); } catch { } }
            }
        }

        // ------------------------------------------------------------------ helpers
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastShot = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Log(Client.GameClient c, string what) { try { Festivals.FestivalManager.Log("RECITAL", Festival, c.Player.UID + " " + c.Player.Name + " | " + what); } catch { } }
        static string ItemName(uint id) { try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); } }

        public static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        static bool Take(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (count == 0) return true;
            if (Count(c, id) < count) return false;
            uint left = count;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).ToArray())
            {
                if (left == 0) break;
                uint stack = item.StackSize > 1 ? item.StackSize : 1u;
                if (stack > left) { item.StackSize = (ushort)(stack - left); item.Mode = Role.Flags.ItemMode.Update; item.Send(c, stream); left = 0; }
                else { left -= stack; item.StackSize = 1; c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream); }
            }
            return true;
        }
        static int Slots(IEnumerable<uint[]> items)
        {
            int n = 0;
            foreach (var i in items)
            {
                if (i == null) continue;
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(i[0], out db)) { n++; continue; }
                n += db.StackSize > 1 ? (int)((i[1] + db.StackSize - 1) / db.StackSize) : (int)i[1];
            }
            return n;
        }
        static bool Space(Client.GameClient c, int slots) { try { return slots <= 0 || c.Inventory.HaveSpace((byte)Math.Min(40, slots)); } catch { return false; } }
        static string Give(Client.GameClient c, IEnumerable<uint[]> items, ServerSockets.Packet stream)
        {
            var parts = new List<string>();
            foreach (var g in items)
            {
                if (g == null || g[1] == 0) continue;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.ContainsKey(g[0])) { parts.Add("MISSING " + g[0]); continue; }
                try
                {
                    uint left = g[1];
                    while (left > 0)
                    {
                        ushort n = (ushort)Math.Min(left, 1000u);
                        if (!c.Inventory.AddItemWitchStack(g[0], 0, n, stream, g[2] != 0)) { parts.Add("FAILED " + g[0]); break; }
                        left -= n;
                    }
                    if (left == 0) parts.Add(ItemName(g[0]) + (g[1] > 1 ? " x" + g[1] : ""));
                }
                catch (Exception e) { parts.Add("FAILED " + g[0] + " (" + e.Message + ")"); }
            }
            return string.Join(", ", parts);
        }

        static bool Mine(string func)
        {
            return func.StartsWith("ValentinesDayConcert_", StringComparison.Ordinal) || func.StartsWith("ValentineDay2022Npc_", StringComparison.Ordinal)
                || func == "MoonFestivalPage_GuideOpen";
        }
        static int Arg(string[] a, int i) { int v; return a != null && a.Length > i && int.TryParse((a[i] ?? "").Trim(), out v) ? v : int.MinValue; }

        // ------------------------------------------------------------------ entry (LuaPush.HandleCallback, msgId 10139)
        /// <summary>True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Mine(func)) return false;
            if (func == "MoonFestivalPage_GuideOpen") return true;// end of the closing animation
            if (Throttled(_last, c.Player.UID, 400)) return true;
            try
            {
                if (func == "ValentinesDayConcert_OpenMainButton")
                {
                    if (!Running) Lua(c, "DlgLoverDayMain_OpenMain|{['Code']=101}");
                    else if (c.Player.Level < MinLevel) Lua(c, "DlgLoverDayMain_OpenMain|{['Code']=102}");
                    else Lua(c, "DlgLoverDayMain_OpenMain|" + MainData(c, 1));
                    return true;
                }
                if (!Running) { Msg(c, "The Valentine event is over."); return true; }
                if (c.Player.Level < MinLevel) { Msg(c, "Please upgrade to level 80 first."); return true; }
                switch (func)
                {
                    case "ValentinesDayConcert_KillMonster": Challenge(c, Arg(args, 0)); return true;
                    case "ValentinesDayConcert_KillBird": Shoot(c, Arg(args, 0)); return true;
                    case "ValentinesDayConcert_KillBirdRefh": Lua(c, "DlgLoverDayMain_ResetShootSun|"); return true;
                    case "ValentinesDayConcert_TransDay": TimeShift(c); return true;
                    case "ValentineDay2022Npc_ScoreReward":
                        {
                            var p = State(c);
                            Msg(c, "Ardor Points: " + p.Score + ". The Ardor Points rewards of the Romance Feast aren`t in this client, so there is nothing to claim here.");
                            return true;
                        }
                }
                Msg(c, "This part of the Starlight Recital isn`t open on this server.");
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        public static string MainData(Client.GameClient c, int viewTip)
        {
            var p = State(c);
            var sb = new StringBuilder("{['Code']=100,['Data']={['ViewTip']=").Append(viewTip).Append(",['SunData']={");
            lock (_lock)
            {
                for (int i = 0; i < 10; i++) sb.Append(i > 0 ? "," : "").Append("{['State']=").Append((p.NoteMask >> i) & 1).Append('}');
                sb.Append("},['BossData']={");
                for (int i = 0; i < 4; i++) sb.Append(i > 0 ? "," : "").Append("{['Index']=").Append(p.Spirits[i]).Append(",['State']=").Append((p.DeadMask >> i) & 1).Append('}');
                bool canShift = p.DeadMask == 15 && p.Power >= Cfg.ShiftCost;
                sb.Append("},['TimeEnergy']=").Append(p.Power).Append(",['ActScore']=").Append(p.Score).Append(",['StoveRed']=0,['RewardRed']=0,['ShineWarsRed']=").Append(canShift ? 1 : 0).Append(',');
            }
            sb.Append("['ArrowNum']=").Append(Count(c, PerformanceInspiration)).Append(",['Makings1']=").Append(Count(c, LoveCandy));
            return sb.Append("}}").ToString();
        }

        // ------------------------------------------------------------------ instrument spirits
        sealed class Fight { public uint Owner; public int Slot, Spirit; public Game.MsgMonster.MonsterRole Mob; public DateTime Expires; public uint Map; }
        static readonly ConcurrentDictionary<uint, Fight> _fights = new ConcurrentDictionary<uint, Fight>();
        static Timer _timer;

        static void Challenge(Client.GameClient c, int spirit)
        {
            var p = State(c);
            int slot;
            lock (_lock) slot = Array.IndexOf(p.Spirits, spirit);
            if (spirit < 1 || spirit > 6 || slot < 0) { Msg(c, "This instrument spirit isn`t on stage today."); return; }
            if (((p.DeadMask >> slot) & 1) != 0) { Msg(c, SpiritNames[spirit - 1] + " has been killed. You cannot challenge it again."); return; }// NoMonster
            Fight f;
            if (_fights.TryGetValue(c.Player.UID, out f) && f.Mob != null && f.Mob.Alive && Now < f.Expires)
            {
                Msg(c, SpiritNames[f.Spirit - 1] + " is waiting for you in Twin City. Defeat it first!");
                return;
            }
            var pl = c.Player;
            if (pl.Map != TwinCity || pl.DynamicID != 0 || !pl.Alive) { Msg(c, "Please go to Twin City to join in the Romance Feast."); return; }// NoMap
            var mob = Spawn(c, spirit);
            if (mob == null) { Msg(c, "The instrument spirit can`t appear right now. Please try again later."); return; }
            _fights[pl.UID] = new Fight { Owner = pl.UID, Slot = slot, Spirit = spirit, Mob = mob, Expires = Now.AddMinutes(Cfg.SpiritMinutes), Map = pl.Map };
            EnsureTimer();
            Msg(c, EnterText[spirit - 1]);
            Log(c, "spirit " + SpiritTypes[spirit - 1] + " spawned (slot " + slot + ")");
        }
        static Game.MsgMonster.MonsterRole Spawn(Client.GameClient c, int spirit)
        {
            var o = SpawnOverride;
            if (o != null) return o(c, spirit);
            string type = SpiritTypes[spirit - 1];
            var fam = Pool.MonsterFamilies == null ? null : Pool.MonsterFamilies.Values.FirstOrDefault(f => f != null && string.Equals(f.Name, type, StringComparison.OrdinalIgnoreCase));
            var map = c.Map;
            if (fam == null || map == null) return null;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;
            var copy = fam.Copy();
            copy.SpawnX = (ushort)Math.Max(1, c.Player.X - 3); copy.SpawnY = (ushort)Math.Max(1, c.Player.Y - 3);
            copy.MaxSpawnX = (ushort)(copy.SpawnX + 6); copy.MaxSpawnY = (ushort)(copy.SpawnY + 6);
            copy.MapID = map.ID; copy.SpawnCount = 1;
            Game.MsgMonster.MonsterRole mob = null;
            for (int i = 0; i < 4 && mob == null; i++) mob = map.MonstersColletion.Add(copy, true, c.Player.DynamicID, true);
            if (mob == null) return null;
            mob.RemoveOnDead = true;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                    mob.Send(mob.GetArray(stream, false));
                }
            }
            catch { }
            return mob;
        }
        static void EnsureTimer()
        {
            if (_timer != null || SpawnOverride != null) return;
            lock (_lock) if (_timer == null) _timer = new Timer(_ => Tick(), null, 1000, 1000);
        }
        static int _ticks;
        static void Tick()
        {
            try { Sweep(); if (_dirty && (++_ticks % 30) == 0) Save(); }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        /// <summary>Settles dead / expired spirits (the server timer every second; the harness calls it directly).</summary>
        public static void Sweep()
        {
            foreach (var kv in _fights.ToArray())
            {
                var f = kv.Value;
                bool dead = f.Mob == null || !f.Mob.Alive;
                Client.GameClient owner;
                bool online = Pool.GamePoll != null && Pool.GamePoll.TryGetValue(kv.Key, out owner) && owner != null && owner.Player != null;
                if (!dead && Now < f.Expires && online && Running) continue;
                Fight r;
                if (!_fights.TryRemove(kv.Key, out r) || !ReferenceEquals(r, f)) continue;
                if (dead)
                {
                    Client.GameClient c;
                    if (online && Pool.GamePoll.TryGetValue(kv.Key, out c) && OwnerKilled(c, f.Mob)) Credit(c, f);
                    else if (online && Pool.GamePoll.TryGetValue(kv.Key, out c)) Msg(c, SpiritNames[f.Spirit - 1] + " was defeated by someone else. Challenge it again from the Starlight Recital.");
                }
                else Despawn(f.Mob);
            }
        }
        /// <summary>Kill credit: the owner did the most damage (Boss=1 keeps per-player damage), is alive and near the spot.</summary>
        static bool OwnerKilled(Client.GameClient c, Game.MsgMonster.MonsterRole mob)
        {
            if (c == null || c.Player == null || mob == null) return false;
            try
            {
                uint top = 0, topScore = 0;
                lock (mob.scoreObj)
                    if (mob.Hunters != null)
                        foreach (var h in mob.Hunters.Values) if (h != null && h.HunterScore > topScore) { topScore = h.HunterScore; top = h.HunterUID; }
                if (top != 0 && top != c.Player.UID) return false;
                return c.Player.Map == mob.Map && c.Player.DynamicID == mob.DynamicID && Role.Core.GetDistance(c.Player.X, c.Player.Y, mob.X, mob.Y) <= 18;
            }
            catch { return false; }
        }
        static void Credit(Client.GameClient c, Fight f)
        {
            var cfg = Cfg;
            var p = State(c);
            var items = new List<uint[]> { new uint[] { PerformanceInspiration, (uint)cfg.SpiritInspiration, 1 } };
            if (cfg.SpiritDreamNotes > 0) items.Add(new uint[] { DreamNote, (uint)cfg.SpiritDreamNotes, 1 });
            lock (_lock)
            {
                if (((p.DeadMask >> f.Slot) & 1) != 0 || p.Spirits[f.Slot] != f.Spirit) return;// already credited / a new day
                if (!Space(c, Slots(items))) { Msg(c, "Please make at least " + Slots(items) + " space(s) in your item inventory first. Then challenge " + SpiritNames[f.Spirit - 1] + " again."); return; }
                p.DeadMask |= 1 << f.Slot;
                p.Score += cfg.SpiritPoints;
                _dirty = true;
            }
            Save();
            string got;
            using (var rec = new ServerSockets.RecycledPacket()) got = Give(c, items, rec.GetStream());
            Lua(c, "DlgLoverDayMain_KillBossCallFunc|{['Data']={['BossIndex']=" + f.Spirit + ",['DropItemInfo']='" + PerformanceInspiration + " 0 " + cfg.SpiritInspiration + "'"
                + (cfg.SpiritDreamNotes > 0 ? ",['ArrowInfo']='" + DreamNote + " 0 " + cfg.SpiritDreamNotes + "'" : "") + ",['ScoreNum']=" + cfg.SpiritPoints + "}}");
            Msg(c, "You successfully defeated " + SpiritNames[f.Spirit - 1] + " and got " + got + " and " + cfg.SpiritPoints + " points.");// KillMonsterNice
            Log(c, "spirit " + f.Spirit + " defeated: " + got + " +" + cfg.SpiritPoints + " pts");
        }
        static void Despawn(Game.MsgMonster.MonsterRole mob)
        {
            if (mob == null) return;
            try
            {
                mob.RemoveOnDead = true;
                bool was = mob.Alive;
                mob.HitPoints = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                    mob.Send(rec.GetStream().ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                var gm = mob.GMap;
                if (gm != null) { gm.View.LeaveMap<Role.IMapObj>(mob); if (was) gm.SetMonsterOnTile(mob.X, mob.Y, false); }
            }
            catch { }
        }

        // ------------------------------------------------------------------ the 10 notes (Magic Book)
        static void Shoot(Client.GameClient c, int rewardTip)
        {
            var cfg = Cfg;
            if (Throttled(_lastShot, c.Player.UID, cfg.ShootCooldownMs)) { Msg(c, "The performance is under way. Please try later."); Lua(c, "DlgLoverDayMain_ResetShootSun|"); return; }
            var p = State(c);
            int note; uint[] item; int extra;
            lock (_lock)
            {
                if (p.NoteMask == 1023) { Msg(c, "All notes have been lit up, so you cannot perform again!"); Lua(c, "DlgLoverDayMain_ResetShootSun|"); return; }// KillAll
                var dark = new List<int>();
                for (int i = 0; i < 10; i++) if (((p.NoteMask >> i) & 1) == 0) dark.Add(i + 1);
                note = dark[Rnd(dark.Count)];
                item = cfg.NoteItems.Length >= note ? cfg.NoteItems[note - 1] : null;
                extra = rewardTip == 2 ? cfg.ExtraMin + Rnd(cfg.ExtraMax - cfg.ExtraMin + 1) : 0;
            }
            var items = new List<uint[]>();
            if (cfg.NoteDreamNotes > 0) items.Add(new uint[] { DreamNote, (uint)cfg.NoteDreamNotes, 1 });
            if (item != null) items.Add(item);
            if (!Space(c, Slots(items))) { Msg(c, "Please make at least " + Slots(items) + " space(s) in your item inventory first."); Lua(c, "DlgLoverDayMain_ResetShootSun|"); return; }
            string got;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                lock (_lock)
                {
                    if (((p.NoteMask >> (note - 1)) & 1) != 0) { Lua(c, "DlgLoverDayMain_ResetShootSun|"); return; }
                    if (!Take(c, PerformanceInspiration, 1, stream))
                    {
                        Lua(c, "DlgLoverDayMain_ResetShootSun|");
                        Msg(c, "Failed to light up notes for lack of Performance Inspiration. Go to challenge instrument spirits for a chance to get Performance Inspiration.");// LackArrow
                        return;
                    }
                    p.NoteMask |= 1 << (note - 1);
                    p.Score += cfg.NotePoints + extra;
                    _dirty = true;
                }
                Save();
                got = Give(c, items, stream);
            }
            int left = 0;
            lock (_lock) for (int i = 0; i < 10; i++) if (((p.NoteMask >> i) & 1) == 0) left++;
            Lua(c, "DlgLoverDayMain_UpdateAnySunData|{['Data']={['SunIndex']=" + note + ",['SunState']=1,['ArrowNum']=" + Count(c, PerformanceInspiration) + ",['ActScore']=" + p.Score + "}}");
            Msg(c, "A note was lit up! Remaining: " + left + ".");// IsSunDrop
            Msg(c, "You received " + got + " and " + (cfg.NotePoints + extra) + " Ardor Points" + (extra > 0 ? " (Extra +" + extra + ")" : "") + ".");
            Log(c, "note " + note + " lit: " + got + " +" + (cfg.NotePoints + extra) + " pts");
        }

        // ------------------------------------------------------------------ Time Watch (Time Shift)
        static void TimeShift(Client.GameClient c)
        {
            var cfg = Cfg;
            var p = State(c);
            Fight f;
            if (_fights.TryGetValue(c.Player.UID, out f) && f.Mob != null && f.Mob.Alive) { Msg(c, "Please challenge those instrument spirits before using the Time Watch!"); return; }
            lock (_lock)
                if (p.DeadMask != 15) { Msg(c, "Please challenge those instrument spirits before using the Time Watch!"); return; }// Trans[1]
            int need;
            lock (_lock) need = cfg.ShiftCost - p.Power;
            if (need > 0)
            {
                // official NoValue3: "You can use Time Berries in your inventory instead."
                int today = Now.Year * 1000 + Now.DayOfYear;
                int use;
                lock (_lock)
                {
                    if (p.BerryDay != today) { p.BerryDay = today; p.BerryUsed = 0; _dirty = true; }
                    use = (int)Math.Min((uint)need, Math.Min(Count(c, TimeBerry), (uint)Math.Max(0, cfg.BerryDailyLimit - p.BerryUsed)));
                }
                if (use > 0)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        lock (_lock)
                        {
                            if (!Take(c, TimeBerry, (uint)use, rec.GetStream())) use = 0;
                            else { p.BerryUsed += use; p.Power = Math.Min(cfg.PowerMax, p.Power + use); _dirty = true; }
                        }
                    }
                    if (use > 0) { Msg(c, "Congrats! You received " + use + " point(s) of Time Power."); Log(c, "used " + use + " Time Berries"); }
                }
                lock (_lock) need = cfg.ShiftCost - p.Power;
                if (need > 0)
                {
                    lock (_lock)
                        if (p.BerryUsed >= cfg.BerryDailyLimit && Count(c, TimeBerry) > 0) Msg(c, "You cannot use more Time Berries today. Please try tomorrow.");
                    Msg(c, "Failed. You don`t have " + cfg.ShiftCost + " points of Time Power!");// LackTimeEnergy
                    return;
                }
            }
            lock (_lock)
            {
                if (p.DeadMask != 15 || p.Power < cfg.ShiftCost) return;
                p.Power -= cfg.ShiftCost;
                NewDay(p);
                _dirty = true;
            }
            Save();
            Msg(c, "Time Shift! A new Starlight Recital begins. The instrument spirits are waiting for you!");
            Lua(c, "DlgLoverDayMain_OpenMain|" + MainData(c, 0));
            Log(c, "time shift -> day " + p.Days + ", power " + p.Power);
        }
    }
}
