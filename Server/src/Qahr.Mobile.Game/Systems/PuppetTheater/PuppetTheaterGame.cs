// ============================================================================
//  [PuppetTheater 2026-10-15] Strings of Terror (Global Halloween 2025) - the playable side.
//    Maze      : the official 8x8 hex exploration board (Fgui_25Wsj_Zgz, InterfaceID 10771003, ClickGrid on msg 10768)
//                for the 10 official areas (STR_LUAUI_QQWS_ZGZ_LEVEL_1..10). Official event ids / names / texts
//                (Cn_Res STR_LUAUI_QQWS_ZGZ_NAME_*, tHalloween2025Exploration, FUI_GBJQ_TXT_25QQWS_ZGZ_*); the layouts,
//                SP per step and Soulthread amounts are DERIVED (Infinity[DB]\PuppetTheater\config.json).
//    Boss      : "Challenge boss" -> the official story quests 9186-9201 (FestivalStory.StoryEntry, quests.json) and
//                afterwards a private boss copy of the area map (FoolsKit.Arena) with an S/A/B rating.
//    Home      : Puppet Home training (Halloween25Draw 15034901): 5 puppets, official reward lists 301001-301003xxx.
//    Finale    : Puppet Finale (Evil Marionette, once a day, local server instead of cross-server).
//    NPCs      : Suyi / Elise (Puppet Store, official CP prices) / Strange Spectator in Twin City, Broken Puppet
//                (Puppet Adventure) in the Vault Palace.  Items: the event boxes (official contents from the item tips).
//  Every grant: player lock -> festival running -> validation -> bag space -> take the cost / record state -> give (logged
//  in Infinity[DB]\PuppetTheater\log.txt).  State: Infinity[DB]\PuppetTheater\players.txt (FoolsKit.Store; uid 0 = server).
//  Hooks (one line each, in the shared April Fools kit): FoolsKit.Kit.OnDialog -> OnDialog, FoolsKit.Kit.OnUse -> OnUse.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.PuppetTheater
{
    public sealed class PtLevel
    {
        public int No; public string Name = "";
        public char[] Cells = new char[64];
        public int[] Event = new int[64];
        public int[] Extra = new int[64];          // encounter: Special n / npc: GBJQ story n / artifact: Artifact text n
        public uint BossFamily, BossMap; public ushort BossX, BossY;
        public uint[] Story = new uint[0];
    }

    public sealed class PtConfig
    {
        public int MinLevel = 80, SpDaily = 150, SpPerStep = 10, PlainSoulChance = 25, ChestSoul = 2, EncounterSoul = 1, ArtifactSoul = 2, BossFirstSoul = 3;
        public int RateS = 60, RateA = 180, BossMinutes = 15;
        public int TrainCost = 10, TrainPoints = 100, SkillMax = 30, ExtraMax = 8;
        public int[] AttrPerLevel = { 200, 20, 20, 15, 15 };
        public Dictionary<int, List<uint[]>> Tiers = new Dictionary<int, List<uint[]>>();   // tier -> {syb,item,count}
        public List<uint[]> Milestones = new List<uint[]>();                                   // {points,item,count}
        public List<uint[]> Store = new List<uint[]>();                                        // {item,cps}
        public uint FinalePool = 30000000, FinaleBossHp = 3000000; public int FinaleMinutes = 10, FinaleFirst = 2, FinaleCleanup = 1, FinaleTopSoul = 2, FinaleMinPuppet = 5; public uint FinaleTopBox = 3354673;
        public int AdvOneChance = 50, AdvOneSoul = 1, AdvTwoSoul = 2, VaultSp = 30; public List<uint[]> AdvThree = new List<uint[]>();
        public uint AdvMap = 12818; public ushort AdvX = 230, AdvY = 236; public List<ushort[]> AdvSpots = new List<ushort[]>();
        public int SpConvertChance = 50;
        // [2026-10-15 video rework]
        public List<uint[]> EncounterReward = new List<uint[]>();      // {item,count}
        public int[] PuppetList = { 1, 3, 2, 1, 3 };
        public int PerUpgDivisor = 5, MaxUpgPerClick = 200, BellDouble = 30;
        public uint OctaveBell = 3354663;
        public List<uint[]> BossFirst = new List<uint[]>();            // {item,count}
        public uint[] MilestoneEvery = { 1000, 3354679, 1 };          // {points,item,count} repeatable
        public List<uint[]> LevelRewards = new List<uint[]>();         // {level,item,count} per puppet
        public List<uint[]> RandomSoulPack = new List<uint[]>();
        public PtLevel[] Levels = new PtLevel[11];
        public Dictionary<string, object> Raw = new Dictionary<string, object>();   // the whole config.json (rewards window, battles)
    }

    public static class PT
    {
        public const string Festival = "PuppetTheater";
        public const uint LuaMsg = 10768;
        public const uint SP = 3354702, Soulthread = 3600813, SybSP = 299001000, SybSoul = 299001001;
        public const uint Suyi = 8893001, Elise = 8893002, Spectator = 8893003, BrokenPuppet = 8893004; public const uint SuyiMarket = 8893007, EliseMarket = 8893008, VaultPortal = 8893009;   // official 2nd spots in the Market (co.99 news)
        public static bool InView(Client.GameClient c, uint npc) { return Kit.NpcInView(c, npc) || (npc == Suyi && Kit.NpcInView(c, SuyiMarket)) || (npc == Elise && Kit.NpcInView(c, EliseMarket)); }
        public const uint DlgMain = 8893090, DlgStore = 8893091, DlgMaze = 8893092, DlgAdv = 8893093, DlgStage = 8893094, DlgFinale = 8893095, DlgInfo = 8893096;
        public const uint TwinCity = 1002, Market = 1036; public const ushort ExitX = 348, ExitY = 446;
        public const uint FamFinale = 993314, FamNightmare = 993315;
        public static readonly uint[] FamSpirit = { 993316, 993317, 993318 };
        public static readonly Store St = new Store(Festival);
        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }
        public static string Today { get { return DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture); } }

        // ------------------------------------------------------------------ texts (official, Infinity[DB]\PuppetTheater\texts.json)
        static Dictionary<string, string> _tx; static string _txStamp;
        public static string T(string key, string def = "")
        {
            try
            {
                var fi = new FileInfo(Path.Combine(Program.ServerConfig.DbLocation, Festival, "texts.json"));
                string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                if (_tx == null || stamp != _txStamp)
                {
                    var d = new Dictionary<string, string>();
                    if (fi.Exists) foreach (var kv in FL.ReadJson(fi.FullName)) d[kv.Key] = kv.Value == null ? "" : kv.Value.ToString();
                    _tx = d; _txStamp = stamp;
                }
                string v; return _tx.TryGetValue(key, out v) && v != null ? v : def;
            }
            catch { return def; }
        }
        public static string Plain(string s) { return System.Text.RegularExpressions.Regex.Replace(s ?? "", @"<[^>]*>|\[/?color[^\]]*\]|\[/?url[^\]]*\]", ""); }

        // ------------------------------------------------------------------ config
        static PtConfig _cfg; static string _cfgStamp;
        public static PtConfig Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(Path.Combine(Program.ServerConfig.DbLocation, Festival, "config.json"));
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _cfgStamp) return _cfg;
                    var c = new PtConfig();
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _cfgStamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[PuppetTheater] config.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new PtConfig();
                }
                return _cfg;
            }
        }
        static int I(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(100000000, FL.U(v)); }
        static List<uint[]> Rows(object o)
        {
            var r = new List<uint[]>();
            var a = FL.Arr(o); if (a == null) return r;
            foreach (var x in a) { var l = FL.Arr(x); if (l != null) r.Add(l.Select(FL.U).ToArray()); }
            return r;
        }
        static void Read(PtConfig c, Dictionary<string, object> d)
        {
            c.Raw = d ?? new Dictionary<string, object>();
            c.MinLevel = I(d, "minLevel", 80); c.SpDaily = I(d, "spDaily", 150); c.SpPerStep = Math.Max(1, I(d, "spPerStep", 10));
            c.PlainSoulChance = I(d, "plainSoulChance", 25); c.ChestSoul = I(d, "chestSoul", 2); c.EncounterSoul = I(d, "encounterSoul", 1);
            c.ArtifactSoul = I(d, "artifactSoul", 2); c.BossFirstSoul = I(d, "bossFirstSoul", 3); c.BossMinutes = Math.Max(5, I(d, "bossMinutes", 15));
            var rt = FL.Obj(FL.Val(d, "rating")); c.RateS = I(rt, "S", 60); c.RateA = I(rt, "A", 180);
            c.TrainCost = Math.Max(1, I(d, "trainCost", 10)); c.TrainPoints = I(d, "trainPoints", 100); c.SkillMax = Math.Max(1, I(d, "skillMax", 30)); c.ExtraMax = I(d, "extraMax", 8);
            var ap = FL.Arr(FL.Val(d, "attrPerLevel")); if (ap != null && ap.Count == 5) c.AttrPerLevel = ap.Select(x => (int)FL.U(x)).ToArray();
            var tiers = FL.Obj(FL.Val(d, "tiers"));
            if (tiers != null) foreach (var kv in tiers)
                {
                    int t; if (!int.TryParse(kv.Key, out t)) continue;
                    var list = new List<uint[]>();
                    foreach (var o in FL.Arr(kv.Value) ?? new List<object>()) { var e = FL.Obj(o); if (e != null) list.Add(new[] { FL.U(FL.Val(e, "syb")), FL.U(FL.Val(e, "item")), FL.U(FL.Val(e, "count")) }); }
                    c.Tiers[t] = list;
                }
            c.Milestones = Rows(FL.Val(d, "milestones")).Where(x => x.Length >= 3).ToList();
            c.Store = Rows(FL.Val(d, "store")).Where(x => x.Length >= 2 && x[1] > 0).ToList();
            var f = FL.Obj(FL.Val(d, "finale"));
            if (f != null)
            {
                c.FinalePool = (uint)I(f, "poolHp", 30000000); c.FinaleBossHp = (uint)Math.Max(1, I(f, "bossHp", 3000000)); c.FinaleMinutes = Math.Max(3, I(f, "minutes", 10));
                c.FinaleFirst = I(f, "firstSoul", 2); c.FinaleCleanup = I(f, "cleanupSoul", 1); c.FinaleTopSoul = I(f, "topSoul", 2); c.FinaleTopBox = (uint)I(f, "topBox", 3354673); c.FinaleMinPuppet = I(f, "minPuppetLevel", 5);
            }
            var a = FL.Obj(FL.Val(d, "adventure"));
            if (a != null)
            {
                var one = FL.Arr(FL.Val(a, "one")); if (one != null && one.Count == 2) { c.AdvOneChance = (int)FL.U(one[0]); c.AdvOneSoul = (int)FL.U(one[1]); }
                c.AdvTwoSoul = I(a, "two", 2); c.AdvThree = Rows(FL.Val(a, "three")); c.VaultSp = I(a, "vaultSp", 30);
                c.AdvMap = (uint)I(a, "map", 12818); c.AdvX = (ushort)I(a, "x", 230); c.AdvY = (ushort)I(a, "y", 236);
                c.AdvSpots = Rows(FL.Val(a, "spots")).Where(x => x.Length >= 2).Select(x => new[] { (ushort)x[0], (ushort)x[1] }).ToList();
            }
            c.SpConvertChance = I(d, "spConvertChance", 50);
            c.EncounterReward = Rows(FL.Val(d, "encounterReward")).Where(x => x.Length >= 2).ToList();
            var pl = FL.Arr(FL.Val(d, "puppetList")); if (pl != null && pl.Count == 5) c.PuppetList = pl.Select(x => (int)FL.U(x)).ToArray();
            c.PerUpgDivisor = Math.Max(1, I(d, "perUpgDivisor", 5)); c.MaxUpgPerClick = Math.Max(1, I(d, "maxUpgPerClick", 200)); c.BellDouble = I(d, "bellDouble", 30);
            c.OctaveBell = (uint)I(d, "octaveBell", 3354663);
            c.BossFirst = Rows(FL.Val(d, "bossFirst")).Where(x => x.Length >= 2).ToList();
            var me = FL.Arr(FL.Val(d, "milestoneEvery")); if (me != null && me.Count == 3) c.MilestoneEvery = me.Select(FL.U).ToArray();
            c.LevelRewards = Rows(FL.Val(d, "levelRewards")).Where(x => x.Length >= 3).ToList();
            c.RandomSoulPack = Rows(FL.Val(d, "randomSoulPack"));
            foreach (var o in FL.Arr(FL.Val(d, "levels")) ?? new List<object>())
            {
                var e = FL.Obj(o); if (e == null) continue;
                int no = I(e, "level", 0); if (no < 1 || no > 10) continue;
                var L = new PtLevel { No = no, Name = FL.Str(e, "name") ?? "" };
                var rows = FL.Arr(FL.Val(e, "layout"));
                for (int i = 0; i < 64; i++) L.Cells[i] = '.';
                if (rows != null) for (int y = 0; y < Math.Min(8, rows.Count); y++) { string r = (rows[y] ?? "").ToString(); for (int x = 0; x < Math.Min(8, r.Length); x++) L.Cells[y * 8 + x] = r[x]; }
                var npcs = (FL.Arr(FL.Val(e, "npc")) ?? new List<object>()).Select(FL.Obj).Where(x => x != null).ToList();
                var enc = (FL.Arr(FL.Val(e, "encounters")) ?? new List<object>()).Select(FL.Obj).Where(x => x != null).ToList();
                var art = FL.Obj(FL.Val(e, "artifact"));
                int ni = 0, xi = 0;
                for (int i = 0; i < 64; i++)
                {
                    switch (L.Cells[i])
                    {
                        case 'S': L.Event[i] = 1; break;
                        case 'E': L.Event[i] = 16; break;
                        case 'C': L.Event[i] = I(e, "chest", 10); break;
                        case 'y': L.Event[i] = 2; break;
                        case 'g': L.Event[i] = 3; break;
                        case 'b': L.Event[i] = 4; break;
                        case 'r': L.Event[i] = 5; break;
                        case 'B': L.Event[i] = 6; break;
                        case 'R': L.Event[i] = 7; break;
                        case 'Y': L.Event[i] = 8; break;
                        case 'G': L.Event[i] = 9; break;
                        case 'A': if (art != null) { L.Event[i] = I(art, "event", 11); L.Extra[i] = I(art, "text", 6); } else L.Cells[i] = 'o'; break;
                        case 'N': if (ni < npcs.Count) { L.Event[i] = I(npcs[ni], "event", 0); L.Extra[i] = I(npcs[ni], "story", 1); ni++; } else L.Cells[i] = 'o'; break;
                        case 'X': if (xi < enc.Count) { L.Event[i] = I(enc[xi], "event", 0); L.Extra[i] = I(enc[xi], "special", 1); xi++; } else L.Cells[i] = 'o'; break;
                    }
                }
                var b = FL.Obj(FL.Val(e, "boss"));
                if (b != null) { L.BossFamily = (uint)I(b, "family", 0); L.BossMap = (uint)I(b, "map", 0); L.BossX = (ushort)I(b, "x", 0); L.BossY = (ushort)I(b, "y", 0); }
                L.Story = (FL.Arr(FL.Val(e, "story")) ?? new List<object>()).Select(FL.U).Where(x => x != 0).ToArray();
                c.Levels[no] = L;
            }
        }

        // ------------------------------------------------------------------ helpers
        public static void Msg(Client.GameClient c, string t) { Kit.Msg(c, Plain(t)); }
        public static uint Count(Client.GameClient c, uint id) { return Kit.Count(c, id); }
        public static long Get(uint uid, string k) { return St.GetLong(uid, k, 0); }
        public static bool InTown(Client.GameClient c) { var p = c.Player; return p.Alive && p.DynamicID == 0 && (p.Map == TwinCity || p.Map == Market); }
        static string Lv(string s) { return s.Replace("\\", "\\\\").Replace("'", "\\'"); }
        public static string Fmt(string t, params object[] a)
        {
            var sb = new StringBuilder(); int ai = 0;
            for (int i = 0; i < (t ?? "").Length; i++)
            {
                if (t[i] == '%' && i + 1 < t.Length && t[i + 1] == 's') { sb.Append(ai < a.Length ? a[ai++] : ""); i++; }
                else if (t[i] == '%' && i + 1 < t.Length && t[i + 1] == '%') { sb.Append('%'); i++; }
                else sb.Append(t[i]);
            }
            return sb.ToString();
        }
        /// <summary>NpcReply texts are short: long official texts go in pieces.</summary>
        public static Game.MsgNpc.Dialog Say(Game.MsgNpc.Dialog d, string text)
        {
            text = Plain(text ?? "");
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 220);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n - 1); if (sp > 120) n = sp + 1; }
                d.AddText(text.Substring(0, n)); text = text.Substring(n);
            }
            return d;
        }
        /// <summary>Gives an item list after the caller's checks. Returns the readable list.</summary>
        public static string Give(Client.GameClient c, ServerSockets.Packet s, string why, params Reward[] list) { return Kit.Give(c, list, s, Festival, why); }
        public static bool Space(Client.GameClient c, params Reward[] list) { return Kit.HasSpace(c, Kit.Slots(list) + 0); }
        public static void Log(Client.GameClient c, string what, string detail) { Kit.Log(Festival, c, what, detail); }

        // virtual dialogs: the per-player handler of the dialog it opened last
        public delegate void Handler(Client.GameClient c, ServerSockets.Packet s, byte opt);
        static readonly ConcurrentDictionary<uint, KeyValuePair<uint, Handler>> _dlg = new ConcurrentDictionary<uint, KeyValuePair<uint, Handler>>();
        public static Game.MsgNpc.Dialog Dialog(Client.GameClient c, ServerSockets.Packet s, uint vid, Handler h)
        {
            _dlg[c.Player.UID] = new KeyValuePair<uint, Handler>(vid, h);
            c.ActiveNpc = vid;
            Kit.LastDialog[c.Player.UID] = vid;
            return new Game.MsgNpc.Dialog(c, s);
        }

        // ------------------------------------------------------------------ the shared hooks (FoolsKit.Kit.OnDialog / OnUse)
        public static bool OnDialog(Client.GameClient c, ServerSockets.Packet s, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null) return false;
            if (npcid >= DlgMain && npcid <= 8893099)
            {
                KeyValuePair<uint, Handler> h;
                if (option == 255 || !_dlg.TryGetValue(c.Player.UID, out h) || h.Key != npcid || h.Value == null) return true;
                if (Kit.Throttled(c.Player.UID, "ptdlg", 250)) return true;
                if (!Running && npcid != DlgInfo) { Msg(c, T("Npc[34646][Text121]")); return true; }// DlgInfo = item choices (boxes keep working after the event)
                try { lock (Kit.Lock(c.Player.UID)) h.Value(c, s, option); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                return true;
            }
            if ((npcid < Suyi || npcid > VaultPortal) && PuppetVault.ThemeOf(npcid) == 0 && npcid != PuppetSeer.Seer || option != 0) return false;
            if (!Kit.NpcInView(c, npcid)) return false;
            try { PuppetNpcs.Open(c, s, npcid == SuyiMarket ? Suyi : npcid == EliseMarket ? Elise : npcid); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s)
        {
            if (item == null) return false;
            return PuppetEvents.UseItem(c, item, s) || PuppetNpcs.UseItem(c, item, s) || PuppetRewards.UseItem(c, item, s);
        }

        // ------------------------------------------------------------------ [2026-10-15] official first-use guides (Festivals.FguiGuide)
        //  guideinfo.dat FguiGuideInfo[150348] (main: 1_1 first open -> Jester`s Maze, Unlock new area, 2/2_2 after area 1,
        //  3 after area 2 -> Puppet Finale, 4/4_2 after the Archon) and [150349] (Puppet Home: 1 first open, Show_2..5 souls).
        //  Shown steps persist per player in the festival store (key "guide").
        public static bool GuideSeen(uint uid, string k) { return ("," + St.Get(uid, "guide") + ",").Contains("," + k + ","); }
        public static void GuideMark(uint uid, string k) { string g = St.Get(uid, "guide"); St.Set(uid, "guide", g.Length == 0 ? k : g + "," + k); }
        public static bool Guide(Client.GameClient c, uint id, string key)
        {
            uint uid = c.Player.UID;
            return Festivals.FguiGuide.ShowOnce(c, id, key, LuaMsg, k => GuideSeen(uid, k), k => GuideMark(uid, k));
        }
        /// <summary>The next main-window guide step for this player (one per open).</summary>
        public static void MainGuide(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            if (Guide(c, 150348, "1_1")) return;
            if (PuppetMaze.Cleared(uid, 1) && Guide(c, 150348, "2")) return;
            if (PuppetMaze.Cleared(uid, 2) && Guide(c, 150348, "3")) return;
            if (Get(uid, "r10") > 0 && Guide(c, 150348, "4")) return;
            for (int L = 2; L <= 10; L++)
                if (PuppetMaze.Unlocked(uid, L) && !GuideSeen(uid, "150348:Unlock" + L)) { GuideMark(uid, "150348:Unlock" + L); PuppetTheaterUi.UnlockFlash(c, L); return; }   // official: flash, then the client opens guide 150348 Unlock
        }
        /// <summary>Soul k -> the area whose boss restores it (video: Gilt = Fox L2, Abyss = Nixie L4, Stone = Lion L7, Blaze = Phoenix L9).</summary>
        static readonly int[] SoulBoss = { 0, 1, 9, 2, 4, 7 };
        public static void HomeGuide(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            if (Guide(c, 150349, "1")) return;
            for (int k = 2; k <= 5; k++) if (PuppetHome.Awake(uid, k) && Get(uid, "r" + SoulBoss[k]) > 0 && Guide(c, 150349, "Show_" + k)) return;   // official: after the boss is subdued
        }

        // ------------------------------------------------------------------ [2026-10-15] stuck players (FoolsKit.Kit.OnTick)
        public static readonly uint[] InstanceMaps = { 1004398, 1004399, 1004400, 1004401, 1004402, 1004403 };
        static readonly ConcurrentDictionary<uint, string> _told = new ConcurrentDictionary<uint, string>();
        /// <summary>Per-player tick: tells a player who fell / left an unfinished challenge how to get back in,
        /// and sends a player standing on a festival instance map OUTSIDE any copy (login after logout inside) to Twin City.</summary>
        public static void Tick(Client.GameClient c)
        {
            var p = c.Player;
            if (p == null || Kit.Throttled(p.UID, "pttick", 4000)) return;
            try { PuppetEvents.Expire(c); } catch { }
            PuppetVault.TickHero(c);
            try { PuppetSeer.Check(c); } catch { }
            if (!p.Alive) return;
            if (p.DynamicID == 0 && Array.IndexOf(InstanceMaps, p.Map) >= 0)
            {
                Arena.Move(c, ExitX, ExitY, TwinCity, 0);
                Msg(c, "The Strings of Terror area closed behind you. Open Strings of Terror (top bar) -> the area -> Challenge boss to go back in.");
                return;
            }
            string what = null;
            var cp = Arena.Of(p.UID);
            if (cp != null && cp.Armed && !Arena.Inside(c, cp) && cp.Tag != null && cp.Tag.StartsWith("PT", StringComparison.Ordinal))
            {
                if (cp.Tag.StartsWith("PTBoss", StringComparison.Ordinal)) { int L; int.TryParse(cp.Tag.Substring(6), out L); var l = Cfg.Levels[L >= 1 && L <= 10 ? L : 1]; what = "boss" + L + "|You left the boss fight of " + (l != null ? l.Name : "the area") + ". Open Strings of Terror (top bar) -> " + (l != null ? l.Name : "the area") + " -> Challenge boss to go back in (the fight is kept 3 minutes, then a fresh one starts)."; }
                else if (cp.Tag == "PTFinale") what = "fin|You left the Puppet Finale. Open Strings of Terror -> Underground Cave -> Challenge to go back in (kept 3 minutes; today`s challenge is used only by a win).";
                else if (cp.Tag == "PTAdv") what = "adv|You left the spirit hunt. Talk to the Broken Puppet in the Vault Palace -> Search for fragments to continue.";
            }
            // [2026-10-15] the "story is waiting" reminder is gone: official has none (the story continues from the board's story cell)
            if (what == null) { string x; _told.TryRemove(p.UID, out x); return; }
            int bar = what.IndexOf('|');
            string key = what.Substring(0, bar);
            string prev;
            if (_told.TryGetValue(p.UID, out prev) && prev == key) return;
            _told[p.UID] = key;
            Kit.Center(c, what.Substring(bar + 1));
        }

        // ------------------------------------------------------------------ points + local ranking (uid 0 row)
        public static void AddPoints(Client.GameClient c, int n)
        {
            if (n <= 0) return;
            uint uid = c.Player.UID;
            long pts = Get(uid, "pts") + n;
            St.Set(uid, "pts", pts);
            lock (St)
            {
                var rows = Ranking();
                string nm = new string((c.Player.Name ?? "").Where(ch => ch != ';' && ch != '=' && ch != ',' && ch != ':' && ch != '\t').ToArray());
                rows.RemoveAll(r => r.Item1 == uid);
                rows.Add(Tuple.Create(uid, pts, nm));
                rows = rows.OrderByDescending(r => r.Item2).Take(20).ToList();
                St.Set(0, "rank", string.Join(",", rows.Select(r => r.Item1 + ":" + r.Item2 + ":" + r.Item3)));
            }
        }
        public static List<Tuple<uint, long, string>> Ranking()
        {
            var r = new List<Tuple<uint, long, string>>();
            foreach (var part in St.Get(0, "rank").Split(','))
            {
                var p = part.Split(':'); uint u; long v;
                if (p.Length >= 3 && uint.TryParse(p[0], out u) && long.TryParse(p[1], out v)) r.Add(Tuple.Create(u, v, p[2]));
            }
            return r;
        }
        public static uint NextMilestone(long pts)
        {
            uint every = Math.Max(1u, Cfg.MilestoneEvery[0]);
            return (uint)((pts / every + 1) * every);
        }

        // ------------------------------------------------------------------ daily things
        /// <summary>150 SP restored daily (official). Top-up to SpDaily once per day; the top-damage payout of yesterday.</summary>
        public static void Daily(Client.GameClient c, ServerSockets.Packet s)
        {
            var cfg = Cfg; uint uid = c.Player.UID;
            lock (Kit.Lock(uid))
            {
                if (St.Get(uid, "spDay") != Today && c.Player.Level >= cfg.MinLevel)
                {
                    uint have = Count(c, SP);
                    uint add = have >= cfg.SpDaily ? 0 : (uint)cfg.SpDaily - have;
                    var r = new Reward(SP, Math.Max(1, add));
                    if (add == 0 || Space(c, r))
                    {
                        St.Set(uid, "spDay", Today);
                        if (add > 0) { Give(c, s, "daily SP", r); PuppetTheaterUi.Refresh(c); Msg(c, "+" + add + " SP. " + T("Npc[Sys][StoreGuidTiLi]")); }
                    }
                }
                PuppetFinale.PayTop(c, s);
            }
        }
    }
}
