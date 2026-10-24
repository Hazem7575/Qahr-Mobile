using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: gradual opening of the mobile main-screen functions (official behaviour), instead of "everything open".
    ///
    /// Client side (libCQ2ClientEn.so, CNewFunctionOpenMgr + ini\luacfg\newfuncopen.dat):
    ///   - every function id (1 InPower, 5 Guild, 6 Forge, 16 Arena ...) has a "server_func" number; its server bit is
    ///     (server_func - 1) of the 64-bit mask in MsgAction 471 (CNewFunctionOpenMgr::SetFunctionFlag). For ids 1..12 the
    ///     server_func is NOT the id (Guild 5 -> bit 2, Forge 6 -> bit 5, InPower 1 -> bit 6 ...); from 13 on it is the id.
    ///   - NewFuncOpenCfg_TestEnableCondition: the function opens when its server bit is set, OR when the client's own
    ///     check passes (quest enable_task finished in the client's mission data AND reborn*130+level >= enable_mete*130+enable_lev),
    ///     and in both cases the class must be in enable_pro (when the list is not empty).
    ///   - the client re-checks on level change (CMsgUserAttrib) and on quest finish, and on every MsgAction 471. A function
    ///     that goes from locked to open after the first check shows the "new function" window (CFDlgNewFunctionOpen,
    ///     effects xgnkfts / xgnkaiqi_tbzd) unless it has disable_show=1.
    /// Server side (this file): Data\mobile_functions.txt gives the rule of each function; the mask of a player is the bits
    /// of the functions whose rule passes, minus Data\mobile_hud.txt hide_functions. A timer re-checks the online mobile
    /// players every 2 s and sends a new MsgAction 471 when the mask changes (level up, reborn, quest, VIP ...), so the
    /// icon appears with the client's own unlock animation.
    /// Hooks: MobileFunctionUnlock.MaskFor(client) (used by MobileProtocol.SendFunctionFlags).
    /// </summary>
    public static class MobileFunctionUnlock
    {
        public sealed class Rule
        {
            public int Id;
            public int Bit;          // 0..62 (server_func - 1)
            public string Name;
            public int Level;        // with Reborn: (reborn, level) >= (Reborn, Level)
            public int Reborn;
            public int Vip;
            public List<uint> Quests = new List<uint>();
            public bool Never, Always;
            public string Text;
        }

        /// <summary>Data\mobile_functions.txt: the rules plus "staff_all_open" and "log".</summary>
        sealed class Config
        {
            public readonly List<Rule> Rules = new List<Rule>();
            public bool StaffAllOpen;
            public bool LogChanges = true;

            public static Config Parse(string[] lines)
            {
                var cfg = new Config();
                int n = 0;
                foreach (var raw in lines)
                {
                    n++;
                    int hash = raw.IndexOf('#');
                    string line = (hash >= 0 ? raw.Substring(0, hash) : raw).Trim();
                    if (line.Length == 0)
                        continue;
                    var tok = DataFile.Words(line);
                    int id, bit;
                    if (tok.Length >= 3 && int.TryParse(tok[0], out id) && int.TryParse(tok[1], out bit))
                    {
                        if (bit < 1 || bit > 63)
                        {
                            MyConsole.WriteLine("[Mobile] mobile_functions.txt line " + n + ": server_func must be 1..63", ConsoleColor.Red);
                            continue;
                        }
                        cfg.Rules.Add(ParseRule(id, bit, tok, n));
                        continue;
                    }
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string key = line.Substring(0, eq).Trim().ToLowerInvariant(), value = line.Substring(eq + 1).Trim();
                    bool on = value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    if (key == "staff_all_open") cfg.StaffAllOpen = on;
                    else if (key == "log") cfg.LogChanges = on;
                }
                MyConsole.WriteLine("[Mobile] mobile_functions.txt: " + cfg.Rules.Count + " functions", ConsoleColor.Cyan);
                return cfg;
            }

            /// <summary>"id server_func Name level=N reborn=N vip=N quest=a,b never always"</summary>
            static Rule ParseRule(int id, int bit, string[] tok, int lineNo)
            {
                var r = new Rule { Id = id, Bit = bit - 1, Name = tok[2] };
                var text = new StringBuilder();
                for (int i = 3; i < tok.Length; i++)
                {
                    string k = tok[i].ToLowerInvariant(), v = "";
                    int e = k.IndexOf('=');
                    if (e > 0) { v = k.Substring(e + 1); k = k.Substring(0, e); }
                    int iv;
                    int.TryParse(v, out iv);
                    switch (k)
                    {
                        case "level": r.Level = iv; break;
                        case "reborn": r.Reborn = iv; break;
                        case "vip": r.Vip = iv; break;
                        case "quest":
                            foreach (var q in v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                uint qv;
                                if (uint.TryParse(q, out qv)) r.Quests.Add(qv);
                            }
                            break;
                        case "never": r.Never = true; break;
                        case "always": r.Always = true; break;
                        default:
                            MyConsole.WriteLine("[Mobile] mobile_functions.txt line " + lineNo + ": unknown condition " + tok[i], ConsoleColor.Red);
                            break;
                    }
                    text.Append(tok[i]).Append(' ');
                }
                r.Text = text.ToString().Trim();
                return r;
            }
        }

        static readonly DataFile<Config> Settings = new DataFile<Config>("mobile_functions.txt", "FunctionUnlock", Config.Parse);
        static readonly object sync = new object();
        static readonly ConcurrentDictionary<uint, long> lastSent = new ConcurrentDictionary<uint, long>();
        static System.Threading.Timer timer;
        static int busy;

        public static List<Rule> Rules
        {
            get { return new List<Rule>(Settings.Value.Rules); }
        }

        /// <summary>Does the rule pass for these values? (level is compared together with the reborn count: a reborn
        /// player has passed every level of the lower reborns).</summary>
        public static bool Passes(Rule r, int level, int reborn, int vip, Func<uint, bool> questDone)
        {
            if (r.Never) return false;
            if (r.Always) return true;
            if (reborn < r.Reborn) return false;
            if (reborn == r.Reborn && level < r.Level) return false;
            if (vip < r.Vip) return false;
            foreach (var q in r.Quests)
                if (questDone == null || !questDone(q)) return false;
            return true;
        }

        /// <summary>The MsgAction 471 mask of a player (and remembers it as sent).</summary>
        public static long MaskFor(Client.GameClient c)
        {
            long m = Compute(c);
            if (c != null && c.Player != null)
                lastSent[c.Player.UID] = m;
            StartTimer();
            return m;
        }

        static long Compute(Client.GameClient c)
        {
            long hud = MobileHud.HiddenMask();// owner's hide_functions: bit (id-1) cleared there
            var cfg = Settings.Value;
            List<Rule> list = cfg.Rules;
            bool staff = cfg.StaffAllOpen;
            if (c == null || c.Player == null || list.Count == 0)
                return hud;// no table: the old behaviour (everything open minus hide_functions)
            if (staff && AdminApi.AdminApiServer.CanUseGmCommands(c))
                return Hide(long.MaxValue, hud, list);
            int level = c.Player.Level, reborn = c.Player.Reborn, vip = 0;
            try { vip = c.Player.VipLevel; } catch (Exception err) { MyConsole.SaveException(err); }
            Func<uint, bool> done = q =>
            {
                try { return c.Player.QuestGUI != null && c.Player.QuestGUI.CheckQuest(q, Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished); }
                catch (Exception err) { MyConsole.SaveException(err); return false; }
            };
            long m = 0;
            foreach (var r in list)
                if (Passes(r, level, reborn, vip, done))
                    m |= 1L << r.Bit;
            return Hide(m, hud, list);
        }

        /// <summary>hide_functions of mobile_hud.txt are function ids: clear the server bit of each hidden id.</summary>
        static long Hide(long m, long hud, List<Rule> list)
        {
            if (hud == long.MaxValue)
                return m;
            for (int id = 1; id <= 63; id++)
            {
                if ((hud & (1L << (id - 1))) != 0)
                    continue;
                bool found = false;
                foreach (var r in list)
                    if (r.Id == id) { m &= ~(1L << r.Bit); found = true; }
                if (!found)
                    m &= ~(1L << (id - 1));
            }
            return m;
        }

        static void StartTimer()
        {
            if (timer != null)
                return;
            lock (sync)
            {
                if (timer == null)
                    timer = new System.Threading.Timer(_ => Tick(), null, 2000, 2000);
            }
        }

        /// <summary>Every 2 s: a new mask for the online mobile players whose functions changed.</summary>
        static void Tick()
        {
            if (System.Threading.Interlocked.Exchange(ref busy, 1) == 1)
                return;
            try
            {
                if (Pool.GamePoll == null)
                    return;
                foreach (var c in Pool.GamePoll.Values)
                {
                    try
                    {
                        if (c == null || c.Player == null || !c.FullLoading || !MobileClient.Is(c))
                            continue;
                        long old;
                        if (!lastSent.TryGetValue(c.Player.UID, out old))
                            old = MobileHud.HiddenMask();// logged in before this code: had the old mask
                        long now = Compute(c);
                        if (now == old)
                            continue;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MobileProtocol.SendFunctionFlags(c, stream);// calls MaskFor -> lastSent
                        }
                        if (Settings.Value.LogChanges)
                            MyConsole.WriteLine("[Mobile] functions " + c.Player.Name + " (lv " + c.Player.Level + " rb " + c.Player.Reborn + "): " + Diff(old, now), ConsoleColor.Cyan);
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine("[Mobile] function unlock: " + e.Message, ConsoleColor.Red);
                    }
                }
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref busy, 0);
            }
        }

        /// <summary>"+Forge -Arena" between two masks (names from the table).</summary>
        public static string Diff(long old, long now)
        {
            var sb = new StringBuilder();
            List<Rule> list = Settings.Value.Rules;
            for (int b = 0; b < 63; b++)
            {
                long bit = 1L << b;
                if ((old & bit) == (now & bit))
                    continue;
                string name = null;
                foreach (var r in list)
                    if (r.Bit == b) { name = r.Name; break; }
                if (name == null)
                    continue;// a bit no function uses
                sb.Append((now & bit) != 0 ? " +" : " -").Append(name);
            }
            return sb.Length == 0 ? "(same)" : sb.ToString().Trim();
        }
    }
}
