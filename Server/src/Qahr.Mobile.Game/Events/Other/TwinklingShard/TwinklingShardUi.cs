// ============================================================================
//  [TwinklingShard 2026-10-14] The official Christmas 2025 "Twinkling Fest" window (client msg 10831
//  tMsg_Info["Christmas25"], Fgui ChristmasMainUI, top-bar button FGUI_ActivityMenu["150381"], Highlight page ActInfo 10831).
//  Shown while the festival TwinklingShard (Questinfo 9500-9514, Data\Festivals\TwinklingShard) runs.
//
//  Official client (luaui\fairygui\window\christmasmainui, decrypted):
//    click   10831 Christmas2025Interface_ClientOpenMain -> Fui_ChristmasMainUI_Open|{Code=200,Data={
//              [1] shop guide {{sysid,n},{sysid,n}} (338001000 Aurora Dew, 339001000 Aurora Crystal), [2] points {{cur,max}},
//              [3] time {begin,end} (unix), [4] searches today (0-5), [5] shard reward {shards,{sysid,n},state},
//              [6] 6 buildings {state 0 open / 1 aura / 2 grey, efftype 0-5 light pillar, isfirst}, [7] find result 0/1,
//              [8] red dots {points,find,cannon}, [9] 5 shards {collected,isfirst} (Wisdom,Valor,Hope,Will,Joy)}}
//            Building order (mediator + Server_Key [Area]): 1 Dawn Star (Joy 1005569), 2 Stellar Court (special encounters),
//              3 Stellar Spire (Hope 1005565), 4 Star Nest (Wisdom 1005568), 5 Stellar Bay (Will 1005566), 6 Stellar Tower (Valor 1005567).
//    window  Christmas2025Interface_ClientFind (Find Twinkling Shards, 5 a day), _ClientTriggerEvent</N>1..6 (enter an area),
//            _ClientCollectReward (5 shards -> Wuqu Snow Box 3355342 = luauirewardshow 341006000), _Congratulation,
//            _ClientGuide</N>1|2 (CP shop guide), ChristmasLottery_OpenView (Aurora Fireworks).
//  Entering an area starts its first quest (official BeginNpcId=0) and teleports there; the quests are engine quests
//  (repeat daily) in the festival quests.json. A shard counts as collected when the area's last quest was finished during
//  the current festival run (read from Data\festivals_log.txt "QUEST" lines, written by the festival module on
//  every festival quest finish, plus the live quest log).
//  NOT on this server (answered with a system message, nothing invented): Stellar Court encounters, Aurora Fireworks
//  (cannons, Twinkling Points, ranking), the CP shop guide. Points are 0.
//  Config (derived values): Data\TwinklingShard\twinklingshard.json. Claims: Data\TwinklingShard\claims.txt.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10831), tagged.
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
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.TwinklingShard
{
    public static class TwinklingShardUi
    {
        public const uint LuaMsg = 10831;
        public const uint HighlightMsg = 10511;// Highlight Events "enter" button: LUAViewSummary_ClientEnterButton</N>10831

        public sealed class Area
        {
            public string Name;
            public uint Map, First, Last;
            public ushort X, Y;
            public int Shard;      // position in the client's shard list [9] (1 Wisdom, 2 Valor, 3 Hope, 4 Will, 5 Joy)
            public int Effect;     // light pillar colour 1-5
            public Area(string name, uint map, ushort x, ushort y, uint first, uint last, int shard, int eff)
            { Name = name; Map = map; X = x; Y = y; First = first; Last = last; Shard = shard; Effect = eff; }
        }
        public sealed class Config
        {
            public string Festival = "TwinklingShard";
            public int MinLevel = 80;// official "Requirement: Level 80"
            public uint RewardItem = 3355342, RewardSysId = 341006000;// Wuqu Snow Box (official text + luauirewardshow)
            public uint AuroraDew = 3600827, AuroraDewSys = 338001000, AuroraCrystal = 3600826, AuroraCrystalSys = 339001000;
            public int EndMonth = 1, EndDay = 14, EndHour = 15, EndMinute = 59;// official end (EU time of the item texts)
            public HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1005565, 1005566, 1005567, 1005568, 1005569 };
            // index = building 1..6 (null = Stellar Court)
            public Area[] Areas =
            {
                new Area("Dawn Star", 1005569, 86, 89, 9512, 9514, 5, 5),
                null,
                new Area("Stellar Spire", 1005565, 102, 123, 9500, 9502, 3, 4),
                new Area("Star Nest", 1005568, 60, 62, 9509, 9511, 1, 2),
                new Area("Stellar Bay", 1005566, 115, 142, 9503, 9505, 4, 3),
                new Area("Stellar Tower", 1005567, 196, 142, 9506, 9508, 2, 1),
            };
        }

        static Config _cfg; static string _stamp;
        static string Dir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "TwinklingShard"); } }
        static string CfgFile { get { return Path.Combine(Dir, "twinklingshard.json"); } }
        static string ClaimFile { get { return Path.Combine(Dir, "claims.txt"); } }
        static string FestLog { get { return Path.Combine(Program.ServerConfig.DbLocation, "festivals_log.txt"); } }

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
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[TwinklingShard] twinklingshard.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                    try { var fe = new FileInfo(CfgFile); _stamp = fe.Exists ? fe.Length + ":" + fe.LastWriteTimeUtc.Ticks : "none"; } catch { }// report a bad file once, not on every call
                }
                return _cfg;
            }
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
            var ri = FL.Val(d, "rewardItem"); if (ri != null) c.RewardItem = FL.U(ri);
            var rs = FL.Val(d, "rewardSysId"); if (rs != null) c.RewardSysId = FL.U(rs);
            var end = FL.Str(d, "officialEnd");// "MM-dd HH:mm"
            if (!string.IsNullOrWhiteSpace(end))
            {
                DateTime e;
                if (DateTime.TryParseExact("2000-" + end.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out e))
                { c.EndMonth = e.Month; c.EndDay = e.Day; c.EndHour = e.Hour; c.EndMinute = e.Minute; }
            }
            var from = FL.Arr(FL.Val(d, "teleportFromMaps"));
            if (from != null) c.TeleportFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
        }

        static Festivals.FestivalManager.RunState Run { get { try { return Festivals.FestivalManager.Running(Cfg.Festival); } catch { return null; } } }
        public static bool Running { get { return Run != null; } }

        // ------------------------------------------------------------------ callbacks (msg 10831)
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastEnter = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10831, and 10511 for the Highlight page 10831).</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == HighlightMsg)
            {
                if (func != "LUAViewSummary_ClientEnterButton" || args.Length == 0 || args[0].Trim() != LuaMsg.ToString()) return false;
                func = "Christmas2025Interface_ClientOpenMain";
            }
            else if (msgId != LuaMsg) return false;
            if (func == "Christmas2025Interface_Congratulation") return true;// the client's own "congratulations" pop-up
            bool known = func.StartsWith("Christmas2025Interface_", StringComparison.Ordinal) || func == "ChristmasLottery_OpenView";
            if (!known) return false;
            if (Throttled(_last, c.Player.UID, 400)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, "The Christmas event has come to a close. Thank you for joining us. May joy follow you wherever you go!"); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Requirement: Level " + cfg.MinLevel + "."); return true; }
            switch (func)
            {
                case "Christmas2025Interface_ClientOpenMain":
                    Lua(c, "Fui_ChristmasMainUI_Open|" + MainData(c, cfg, false));
                    return true;
                case "Christmas2025Interface_ClientFind":
                    Find(c, cfg);
                    return true;
                case "Christmas2025Interface_ClientTriggerEvent":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > 6) return true;
                        if (Throttled(_lastEnter, c.Player.UID, 3000)) return true;
                        Enter(c, cfg, i);
                        return true;
                    }
                case "Christmas2025Interface_ClientCollectReward":
                    Claim(c, cfg);
                    return true;
                case "Christmas2025Interface_ClientGuide":
                    Msg(c, "The Aurora Crystal / Aurora Dew shop isn`t available on this server yet.");
                    return true;
                case "ChristmasLottery_OpenView":
                    Msg(c, "Aurora Fireworks isn`t available on this server yet.");
                    return true;
            }
            return true;
        }

        // ------------------------------------------------------------------ state
        static bool Finished(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; } }
        static bool Taken(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; } }
        static uint Count(Client.GameClient c, uint id) { try { return (uint)Math.Max(0, c.Inventory.GetCountItem(id)); } catch { return 0; } }

        /// <summary>Areas explored today (the last quest of the area is finished; the engine clears it at the daily reset).</summary>
        static int SearchesToday(Client.GameClient c, Config cfg) { return cfg.Areas.Count(a => a != null && Finished(c, a.Last)); }

        // shards collected this run, from the festival log ("... | QUEST | TwinklingShard | uid name finished quest 9502 gave=[]")
        static readonly object LogSync = new object();
        static DateTime _logRunStart = DateTime.MinValue;
        static long _logPos;
        static readonly Dictionary<uint, HashSet<uint>> _logDone = new Dictionary<uint, HashSet<uint>>();
        static HashSet<uint> LoggedFinishes(uint uid, Festivals.FestivalManager.RunState run, Config cfg)
        {
            lock (LogSync)
            {
                try
                {
                    if (run.Started != _logRunStart) { _logRunStart = run.Started; _logPos = 0; _logDone.Clear(); }
                    var fi = new FileInfo(FestLog);
                    if (fi.Exists)
                    {
                        if (fi.Length < _logPos) { _logPos = 0; _logDone.Clear(); }
                        if (fi.Length > _logPos)
                            using (var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                fs.Seek(_logPos, SeekOrigin.Begin);
                                var buf = new byte[fs.Length - _logPos];
                                int n = 0, r;
                                while (n < buf.Length && (r = fs.Read(buf, n, buf.Length - n)) > 0) n += r;
                                int lastNl = Array.LastIndexOf(buf, (byte)'\n', Math.Max(0, n - 1));
                                if (lastNl >= 0)
                                {
                                    string text = Encoding.UTF8.GetString(buf, 0, lastNl + 1);
                                    _logPos += lastNl + 1;
                                    ParseLog(text, cfg);
                                }
                            }
                    }
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                HashSet<uint> s;
                return _logDone.TryGetValue(uid, out s) ? new HashSet<uint>(s) : new HashSet<uint>();
            }
        }
        static void ParseLog(string text, Config cfg)
        {
            var lasts = new HashSet<uint>(cfg.Areas.Where(a => a != null).Select(a => a.Last));
            foreach (var raw in text.Split('\n'))
            {
                var parts = raw.Split(new[] { " | " }, StringSplitOptions.None);
                if (parts.Length < 4 || parts[1] != "QUEST" || !string.Equals(parts[2].Trim(), cfg.Festival, StringComparison.OrdinalIgnoreCase)) continue;
                DateTime when;
                if (!DateTime.TryParseExact(parts[0].Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out when) || when < _logRunStart.AddSeconds(-60)) continue;
                string d = parts[3];
                int sp = d.IndexOf(' '), fq = d.LastIndexOf(" finished quest ", StringComparison.Ordinal);
                if (sp <= 0 || fq < 0) continue;
                uint uid, q;
                string qs = d.Substring(fq + 16).Trim();
                int e = qs.IndexOf(' ');
                if (e > 0) qs = qs.Substring(0, e);
                if (!uint.TryParse(d.Substring(0, sp), out uid) || !uint.TryParse(qs, out q) || !lasts.Contains(q)) continue;
                HashSet<uint> set;
                if (!_logDone.TryGetValue(uid, out set)) _logDone[uid] = set = new HashSet<uint>();
                set.Add(q);
            }
        }
        /// <summary>Last quests finished this run (log + live quest log).</summary>
        static HashSet<uint> Shards(Client.GameClient c, Config cfg, Festivals.FestivalManager.RunState run)
        {
            var s = run != null ? LoggedFinishes(c.Player.UID, run, cfg) : new HashSet<uint>();
            foreach (var a in cfg.Areas) if (a != null && Finished(c, a.Last)) s.Add(a.Last);
            return s;
        }

        // claims (once per character per festival run)
        static readonly object ClaimSync = new object();
        static HashSet<string> _claims;
        static HashSet<string> Claims()
        {
            if (_claims != null) return _claims;
            var s = new HashSet<string>();
            try
            {
                if (File.Exists(ClaimFile))
                    foreach (var l in File.ReadAllLines(ClaimFile))
                    {
                        var p = l.Split('|');
                        if (p.Length >= 3 && p[0] == "CLAIM") s.Add(p[1] + "|" + p[2]);
                        else if (p.Length >= 3 && p[0] == "UNDO") s.Remove(p[1] + "|" + p[2]);
                    }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return _claims = s;
        }
        static string RunKey(Festivals.FestivalManager.RunState run) { return run.Run + "@" + run.Started.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture); }
        static bool Claimed(uint uid, Festivals.FestivalManager.RunState run) { lock (ClaimSync) return run != null && Claims().Contains(RunKey(run) + "|" + uid); }
        static void AppendClaim(string kind, string run, uint uid, string name)
        {
            Directory.CreateDirectory(Dir);
            File.AppendAllText(ClaimFile, kind + "|" + run + "|" + uid + "|" + name + "|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + Environment.NewLine, Encoding.UTF8);
        }

        // aura of "Find Twinkling Shards" (per player, today)
        static readonly ConcurrentDictionary<uint, KeyValuePair<DateTime, int>> _aura = new ConcurrentDictionary<uint, KeyValuePair<DateTime, int>>();
        static readonly Random Rng = new Random();
        static int Aura(Client.GameClient c, Config cfg)
        {
            KeyValuePair<DateTime, int> a;
            if (!_aura.TryGetValue(c.Player.UID, out a) || a.Key != DateTime.Today) return 0;
            var area = a.Value >= 1 && a.Value <= 6 ? cfg.Areas[a.Value - 1] : null;
            return area != null && !Finished(c, area.Last) ? a.Value : 0;
        }

        // ------------------------------------------------------------------ window data
        static long Unix(DateTime local) { return new DateTimeOffset(local).ToUnixTimeSeconds(); }
        public static string MainData(Client.GameClient c, Config cfg, bool found)
        {
            var run = Run;
            var shards = Shards(c, cfg, run);
            int aura = Aura(c, cfg);
            DateTime begin = run != null ? run.Started : DateTime.Now;
            var end = new DateTime(begin.Year, cfg.EndMonth, cfg.EndDay, cfg.EndHour, cfg.EndMinute, 0);
            if (end <= begin) end = end.AddYears(1);
            int got = shards.Count;
            int state = Claimed(c.Player.UID, run) ? 2 : got >= 5 ? 1 : 0;

            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("{{").Append(cfg.AuroraDewSys).Append(',').Append(Count(c, cfg.AuroraDew)).Append("},{")
              .Append(cfg.AuroraCrystalSys).Append(',').Append(Count(c, cfg.AuroraCrystal)).Append("}},");
            sb.Append("{},");
            sb.Append('{').Append(Unix(begin)).Append(',').Append(Unix(end)).Append("},");
            sb.Append(SearchesToday(c, cfg)).Append(',');
            sb.Append('{').Append(Math.Min(5, got)).Append(",{").Append(cfg.RewardSysId).Append(",1},").Append(state).Append("},");
            sb.Append('{');
            for (int i = 0; i < 6; i++)
            {
                if (i > 0) sb.Append(',');
                var a = cfg.Areas[i];
                int st = 0, eff = 0;
                if (a != null && Finished(c, a.Last)) st = 2;
                else if (a != null && aura == i + 1) { st = 1; eff = a.Effect; }
                sb.Append('{').Append(st).Append(',').Append(eff).Append(",0}");
            }
            sb.Append("},").Append(found ? 1 : 0).Append(",{0,").Append(aura == 0 && SearchesToday(c, cfg) < 5 ? 1 : 0).Append(",0},{");
            for (int s = 1; s <= 5; s++)
            {
                if (s > 1) sb.Append(',');
                var a = cfg.Areas.FirstOrDefault(x => x != null && x.Shard == s);
                sb.Append('{').Append(a != null && shards.Contains(a.Last) ? 1 : 0).Append(",0}");
            }
            return sb.Append("}}}").ToString();
        }

        // ------------------------------------------------------------------ actions
        static void Find(Client.GameClient c, Config cfg)
        {
            if (SearchesToday(c, cfg) >= 5) { Msg(c, "Your Twinkling Shard Search attempts have reached the daily limit. Please come back another day."); return; }
            if (Aura(c, cfg) != 0) { Msg(c, "The aura of Twinkling Shards has appeared. Hurry and explore it!"); Lua(c, "Fui_ChristmasMainUI_Update|" + MainData(c, cfg, false)); return; }
            var open = Enumerable.Range(1, 6).Where(i => cfg.Areas[i - 1] != null && !Finished(c, cfg.Areas[i - 1].Last)).ToList();
            if (open.Count == 0) { Msg(c, "Your Twinkling Shard Search attempts have reached the daily limit. Please come back another day."); return; }
            int pick;
            lock (Rng) pick = open[Rng.Next(open.Count)];
            _aura[c.Player.UID] = new KeyValuePair<DateTime, int>(DateTime.Today, pick);
            Lua(c, "Fui_ChristmasMainUI_Update|" + MainData(c, cfg, true));
        }

        static void Enter(Client.GameClient c, Config cfg, int building)
        {
            var p = c.Player;
            var a = cfg.Areas[building - 1];
            if (a == null) { Msg(c, "The Stellar Court encounters aren`t available on this server yet."); return; }
            if (Finished(c, a.Last)) { Msg(c, "You have already found the Twinkling Shard in " + a.Name + " today. Please come back another day."); return; }
            if (!p.Alive || p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map)) { Msg(c, "You can`t go to " + a.Name + " from here. Please go back to a city first."); return; }
            if (Pool.ServerMaps[a.Map] == null) { Msg(c, a.Name + " isn`t open yet."); return; }
            var t = QuestEngine.QuestEngineCore.Current;
            if (t == null) { Msg(c, a.Name + " isn`t open yet."); return; }
            bool inChain = false;
            for (uint q = a.First; q <= a.Last; q++) if (Taken(c, q) && !Finished(c, q)) inChain = true;
            if (!inChain)
            {
                QuestEngine.EngineQuest first;
                if (!t.Quests.TryGetValue(a.First, out first)) { Msg(c, a.Name + " isn`t open yet."); return; }
                string name, why;
                if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, first.BeginNpc, a.First, out name, out why))
                {
                    Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                    return;
                }
                Msg(c, "[Quest] New quest: " + name + ".");
            }
            if (p.Map == a.Map) return;
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, a.X, a.Y, a.Map, 0); else c.Teleport(a.X, a.Y, a.Map, 0);
        }

        static void Claim(Client.GameClient c, Config cfg)
        {
            var run = Run;
            if (run == null) return;
            if (Shards(c, cfg, run).Count < 5) { Msg(c, "Collect all 5 Twinkling Shards first."); return; }
            if (!Pool.ItemsBase.ContainsKey(cfg.RewardItem)) { Msg(c, "This reward isn`t available on this server yet."); return; }
            string key = RunKey(run) + "|" + c.Player.UID;
            lock (ClaimSync)
            {
                if (Claims().Contains(key)) { Msg(c, "You have already claimed this reward."); return; }
                if (!c.Inventory.HaveSpace(1)) { Msg(c, "Your inventory is full. Please make 1 slot and try again."); return; }
                try { AppendClaim("CLAIM", RunKey(run), c.Player.UID, c.Player.Name); }
                catch (Exception e) { MyConsole.SaveException(e); Msg(c, "Please try again later."); return; }
                Claims().Add(key);
                bool ok = false;
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                        ok = c.Inventory.AddItemWitchStack(cfg.RewardItem, 0, 1, rec.GetStream(), true);
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                if (!ok)
                {
                    Claims().Remove(key);
                    try { AppendClaim("UNDO", RunKey(run), c.Player.UID, c.Player.Name); } catch { }
                    Msg(c, "Please try again later.");
                    return;
                }
            }
            try { Festivals.FestivalManager.Log("REWARD", cfg.Festival, c.Player.UID + " " + c.Player.Name + " claimed the 5-shard reward " + cfg.RewardItem); } catch { }
            Msg(c, "You collected all 5 Twinkling Shards and received a Wuqu Snow Box!");
            Lua(c, "Fui_ChristmasMainUI_Update|" + MainData(c, cfg, false));
        }
    }
}
