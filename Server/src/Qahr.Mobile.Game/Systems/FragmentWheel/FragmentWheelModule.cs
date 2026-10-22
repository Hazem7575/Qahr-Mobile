using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Qahr.Mobile.Game.MsgNpc;

namespace Qahr.Mobile.FragmentWheel
{
    /// <summary>
    /// [FragmentWheel 2026-10-14] Official Fragment Wheel of Twin City: Daisy 27814 (NpcType 1951) explains it,
    /// FragmentWheel 27815 (NpcType 1952) spins it. Texts = client ini\Server_Key.lua tRecycleFragmentAndUpLev[27814/27815].
    ///
    /// Official (client): 12 wheel levels, EXP for upgrade 10..120, fragment cost 40000..200000 of
    /// [Souvenir Medal Fragment] (itemtype 3327170), 1 EXP per spin, first spin of every level is free,
    /// prizes = Super Gem / Dragon Ball (B) / +4 Stone (B).
    /// Derived (not in the client): prize weights/counts per level, EXP only from the current level's wheel,
    /// min player level, cooldowns -> Infinity[DB]\FragmentWheel\config.json (re-read when the file changes).
    ///
    /// State per character (wheel level, EXP, free spin used) -> Infinity[DB]\FragmentWheel\state.txt
    /// (append-only journal "uid|level|exp|freeUsed|seq", the last line of a uid wins, compacted at load).
    ///
    /// Security: uid from client.Player.UID only; the option byte is checked against the menu the server itself
    /// sent (per-uid pending state, consumed once); everything (level, cost, balance, bag space) is recomputed on
    /// confirm under a per-uid lock; fragments are deducted (or the free spin is marked used and persisted) BEFORE
    /// the prize is granted through Inventory.AddItemWitchStack; spins are rate limited.
    /// Routing: [NpcAttribute] below (the attribute scan covers the whole assembly) + one hook in Procesor.cs that
    /// removes 27814/27815 from Procesor.MaintinaceNpc.
    /// </summary>
    public static class FragmentWheelModule
    {
        public const uint DaisyId = 27814, WheelId = 27815;

        // option bytes (byte, 0 and 255 are never routed by Procesor)
        const byte OptInfo = 1;         // Daisy: "FragmentWheel" / Wheel: "Spin the wheel."
        const byte OptPickBase = 10;    // 11..22 = "Spin Lx wheel."
        const byte OptConfirmBase = 30; // 31..42 = "Yes." for level x

        /// <summary>Test hook (harness only): receives every dialog line the module sends (kind, option id, text).</summary>
        public static Action<uint, string, byte, string> Trace;

        // ------------------------------------------------------------------ hook
        /// <summary>One-line hook in Procesor.cs: the official wheel NPCs are no longer "Under maintenance".</summary>
        public static bool ReleaseMaintenance(List<uint> maintenance)
        {
            try { if (maintenance != null) { maintenance.Remove(DaisyId); maintenance.Remove(WheelId); } } catch { }
            return true;
        }

        // ------------------------------------------------------------------ NPC scripts
        [NpcAttribute((NpcID)DaisyId)]
        public static void Daisy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try { OnDaisy(client, stream, Option); }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [NpcAttribute((NpcID)WheelId)]
        public static void Wheel(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try { OnWheel(client, stream, Option); }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        const string Line = "—————————————————————————————\n";

        static void OnDaisy(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (c == null || c.Player == null) return;
            var cfg = Cfg;
            var d = new Dlg(c, stream, DaisyId);
            if (option == 0)
            {
                d.Text("Daisy:\n ")
                 .Text("New Fragment Wheel is launched! The higher the level of Fragment Wheel, the more bountiful the rewards! No one can refuse such great rewards!\n")
                 .Text("Reward: Super Gem, Dragon Ball (B), +4 Stone (B)\n")
                 .Option("FragmentWheel", OptInfo)
                 .Option("Leave.", 255).Finish();
                return;
            }
            if (option != OptInfo) return;
            var st = GetState(c.Player.UID);
            d.Text(Line).Text("[Wheel Level]       [EXP for Upgrade]       [Fragment Consumption]\n");
            // official column layout of Text213..Text2114
            string[] rows =
            {
                "       1                                  10                                   40000\n",
                "       2                                  20                                   50000\n",
                "       3                                  30                                   60000\n",
                "       4                                  40                                   70000\n",
                "       5                                  50                                   80000\n",
                "       6                                  60                                   90000\n",
                "       7                                  70                                   100000\n",
                "       8                                  80                                   120000\n",
                "       9                                  90                                   140000\n",
                "       10                               100                                  160000\n",
                "       11                               120                                  180000\n",
                "       12                                0                                     200000\n",
            };
            bool official = cfg.MatchesOfficialTable();
            for (int i = 0; i < 12; i++)
            {
                if (official) d.Text(rows[i]);
                else d.Text("       " + (i + 1) + "                                  " + cfg.Levels[i].Exp + "                                   " + cfg.Levels[i].Cost + "\n");
            }
            d.Text(Line)
             .Text(string.Format("Current level: {0}\n", st.Level))
             .Text("Spin the wheel once to get 1 EXP. You can spin it for free for the first time.\n")
             .Text("The rewards will upgrade with the level of the Wheel. Each time it upgrades a level, you can get a chance to spin it for free.\n")
             .Option("I~see.", 255).Finish();
        }

        static void OnWheel(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (c == null || c.Player == null) return;
            uint uid = c.Player.UID;
            var cfg = Cfg;
            var d = new Dlg(c, stream, WheelId);
            if (option == 0)
            {
                Pending old; _pending.TryRemove(uid, out old);
                d.Text("Fragment Wheel:\n")
                 .Text("New Fragment Wheel is launched! The higher the level of Fragment Wheel, the more bountiful the rewards!\n")
                 .Text("Use [Souvenir Medal Fragment] to spin the wheel and win grand rewards!\n")
                 .Text("Reward: Super Gem, Dragon Ball (B), +4 Stone (B)\n")
                 .Option("Spin~the~wheel.\n", OptInfo)
                 .Option("Leave.\n", 255).Finish();
                Arm(uid, Stage.Main, 0);
                return;
            }
            Pending p;
            if (!_pending.TryGetValue(uid, out p) || p.Expires < DateTime.UtcNow) { Log(c, "option " + option + " without an open menu -> ignored"); return; }

            if (option == OptInfo && p.Stage == Stage.Main)
            {
                if (!cfg.Enabled) { c.CreateBoxDialog("The Fragment Wheel is closed now. Please come back later."); return; }
                if (c.Player.Level < cfg.MinPlayerLevel) { c.CreateBoxDialog("You must be at least level " + cfg.MinPlayerLevel + " to spin the Fragment Wheel."); return; }
                ShowStatus(c, d, cfg);
                return;
            }
            if (option > OptPickBase && option <= OptPickBase + 12 && p.Stage == Stage.Status)
            {
                int lv = option - OptPickBase;
                var st = GetState(uid);
                if (lv > st.Level) { Log(c, "picked L" + lv + " above wheel level " + st.Level + " -> refused"); return; }
                bool free = lv == st.Level && !st.FreeUsed;
                if (free) d.Text(string.Format("This is the first time you spin the L{0} Fragment Wheel. It`s free of charge.\n", lv));
                else d.Text(string.Format("Consume {0} [Souvenir Medal Fragment(s)] to spin the wheel?\n", cfg.Levels[lv - 1].Cost));
                d.Option("Yes.\n", (byte)(OptConfirmBase + lv)).Option("Leave.\n", 255).Finish();
                Arm(uid, Stage.Confirm, lv);
                return;
            }
            if (option > OptConfirmBase && option <= OptConfirmBase + 12 && p.Stage == Stage.Confirm && p.Level == option - OptConfirmBase)
            {
                Pending gone;
                if (!_pending.TryRemove(uid, out gone) || !ReferenceEquals(gone, p)) return;// consumed exactly once
                string err = Spin(c, stream, p.Level);
                if (err != null) { c.CreateBoxDialog(err); return; }
                ShowStatus(c, new Dlg(c, stream, WheelId), Cfg);// back to the wheel for the next spin
                return;
            }
            Log(c, "option " + option + " does not match the open menu (" + p.Stage + ") -> ignored");
        }

        static void ShowStatus(Client.GameClient c, Dlg d, Config cfg)
        {
            uint uid = c.Player.UID;
            var st = GetState(uid);
            int need = cfg.Levels[st.Level - 1].Exp;
            d.Text(Line)
             .Text(string.Format("Fragment Wheel Level: {0}\n", st.Level))
             .Text(string.Format("Fragment Wheel Upgrade Progress: {0}/{1}\n", st.Exp, need))
             .Text(string.Format("You need to use {0} fragment(s) to spin the wheel now.\n", cfg.Levels[st.Level - 1].Cost))
             .Text(Line);
            if (!st.FreeUsed) d.Text(string.Format("This is the first time you spin the L{0} Fragment Wheel. It`s free of charge.\n", st.Level));
            for (int lv = st.Level; lv >= 1; lv--)// official option order: Option2112 (L12) first ... Option211 (L1)
                d.Option("Spin~L" + lv + "~wheel.\n", (byte)(OptPickBase + lv));
            d.Option("Leave.\n", 255).Finish();
            Arm(uid, Stage.Status, 0);
        }

        // ------------------------------------------------------------------ spin (the only place that pays)
        static readonly ConcurrentDictionary<uint, object> _locks = new ConcurrentDictionary<uint, object>();
        static readonly ConcurrentDictionary<uint, DateTime> _lastSpin = new ConcurrentDictionary<uint, DateTime>();

        /// <summary>Returns null on success, else the message for the player. Nothing is taken when it fails.</summary>
        internal static string Spin(Client.GameClient c, ServerSockets.Packet stream, int lv)
        {
            var cfg = Cfg;
            if (!cfg.Enabled) return "The Fragment Wheel is closed now. Please come back later.";
            if (c == null || c.Player == null || c.Inventory == null) return "Error.";
            uint uid = c.Player.UID;
            if (uid == 0) return "Error.";
            if (c.Player.Level < cfg.MinPlayerLevel) return "You must be at least level " + cfg.MinPlayerLevel + " to spin the Fragment Wheel.";
            if (lv < 1 || lv > 12) return "Error.";
            lock (_locks.GetOrAdd(uid, k => new object()))
            {
                DateTime now = DateTime.UtcNow, last;
                if (_lastSpin.TryGetValue(uid, out last) && (now - last).TotalMilliseconds < cfg.SpinCooldownMs)
                    return "Please wait a moment before spinning again.";
                var st = GetState(uid);
                if (lv > st.Level) return "Error.";
                bool free = lv == st.Level && !st.FreeUsed;
                uint cost = free ? 0u : (uint)cfg.Levels[lv - 1].Cost;

                var table = cfg.Rewards[lv - 1];
                if (table == null || table.Count == 0) return "The Fragment Wheel is closed now. Please come back later.";
                int slots = 0;
                foreach (var r in table) slots = Math.Max(slots, SlotsFor(r));
                if (slots > 40 || !c.Inventory.HaveSpace((byte)slots))
                    return "Your inventory is full. Please leave at least " + slots + " free slot(s) and try again.";
                if (cost > 0 && CountItem(c, cfg.FragmentItem) < cost)
                    return string.Format("You don`t have {0} [Souvenir Medal Fragment(s)].", cost);

                Reward prize = Roll(table);
                if (prize == null) return "Error.";
                uint prizeItem = prize.Items[RandInt(prize.Items.Length)];

                // 1) pay: deduct the fragments / burn the free spin, and persist the new wheel state
                if (cost > 0 && !Deduct(c, stream, cfg.FragmentItem, cost))
                {
                    Log(c, "deduct of " + cost + " fragments FAILED -> no spin");
                    return string.Format("You don`t have {0} [Souvenir Medal Fragment(s)].", cost);
                }
                _lastSpin[uid] = now;
                int oldLevel = st.Level, oldExp = st.Exp;
                var ns = new State { Level = st.Level, Exp = st.Exp, FreeUsed = st.FreeUsed };
                if (free) ns.FreeUsed = true;
                bool gainsExp = lv == st.Level || !cfg.ExpOnlyFromCurrentLevel;
                bool levelUp = false;
                if (gainsExp && ns.Level < 12)
                {
                    ns.Exp++;
                    int need = cfg.Levels[ns.Level - 1].Exp;
                    if (need > 0 && ns.Exp >= need) { ns.Level++; ns.Exp = 0; ns.FreeUsed = false; levelUp = true; }
                }
                SetState(uid, ns);

                // 2) grant through the normal item path (stack/bound handled there)
                bool ok = false;
                try { ok = c.Inventory.AddItemWitchStack(prizeItem, 0, (ushort)prize.Count, stream, prize.Bound); }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                string itemName = ItemName(prizeItem);
                Log(c, string.Format("spin L{0} {1} cost={2} wheel {3}:{4}->{5}:{6}{7} prize={8}x{9} ({10}{11}) grant={12}",
                    lv, free ? "FREE" : "paid", cost, oldLevel, oldExp, ns.Level, ns.Exp, levelUp ? " LEVEL-UP" : "",
                    prizeItem, prize.Count, itemName, prize.Bound ? ",bound" : "", ok ? "ok" : "FAILED"));
                if (!ok) return "The reward could not be delivered. Please contact a GM.";

                try
                {
                    c.SendSysMesage(string.Format("You spun the L{0} Fragment Wheel and received {1} {2}!", lv, prize.Count, prize.Name),
                        Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                    if (levelUp)
                        c.SendSysMesage(string.Format("Your Fragment Wheel reached level {0}! Your first L{0} spin is free.", ns.Level),
                            Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                }
                catch { }
                return null;
            }
        }

        static int SlotsFor(Reward r)
        {
            int worst = 0;
            foreach (var id in r.Items)
            {
                Database.ItemType.DBItem db;
                int stack = 0;
                if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db)) stack = db.StackSize;
                int need = stack > 1 ? (r.Count + stack - 1) / stack : r.Count;
                worst = Math.Max(worst, need);
            }
            return Math.Max(1, worst);
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) ? db.Name : id.ToString();
        }

        internal static long CountItem(Client.GameClient c, uint id)
        {
            long n = 0;
            foreach (var it in c.Inventory.ClientItems.Values)
                if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1;
            return n;
        }

        /// <summary>Stack-aware removal of exactly <paramref name="count"/> pieces (Inventory.Remove counts whole items).</summary>
        static bool Deduct(Client.GameClient c, ServerSockets.Packet stream, uint id, uint count)
        {
            var list = new List<Game.MsgServer.MsgGameItem>();
            foreach (var it in c.Inventory.ClientItems.Values) if (it.ITEM_ID == id) list.Add(it);
            long have = 0;
            foreach (var it in list) have += it.StackSize > 1 ? it.StackSize : 1;
            if (have < count) return false;
            list.Sort((a, b) => (a.StackSize > 1 ? a.StackSize : 1).CompareTo(b.StackSize > 1 ? b.StackSize : 1));// smallest stacks first
            uint left = count;
            foreach (var it in list)
            {
                if (left == 0) break;
                uint size = it.StackSize > 1 ? it.StackSize : 1u;
                if (size > left)
                {
                    it.StackSize = (ushort)(size - left);
                    it.Mode = Role.Flags.ItemMode.Update;
                    it.Send(c, stream);
                    left = 0;
                }
                else
                {
                    left -= size;
                    c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, stream, true);
                }
            }
            return left == 0;
        }

        // ------------------------------------------------------------------ dialog state (anti forged options)
        enum Stage { Main, Status, Confirm }
        sealed class Pending { public Stage Stage; public int Level; public DateTime Expires; }
        static readonly ConcurrentDictionary<uint, Pending> _pending = new ConcurrentDictionary<uint, Pending>();
        static void Arm(uint uid, Stage s, int lv)
        {
            _pending[uid] = new Pending { Stage = s, Level = lv, Expires = DateTime.UtcNow.AddSeconds(Math.Max(5, Cfg.ConfirmTimeoutSec)) };
        }

        sealed class Dlg
        {
            readonly Dialog d; readonly uint npc;
            public Dlg(Client.GameClient c, ServerSockets.Packet s, uint npc) { d = new Dialog(c, s); this.npc = npc; }
            public Dlg Text(string t) { var tr = Trace; if (tr != null) tr(npc, "text", 0, t); d.AddText(t); return this; }
            public Dlg Option(string t, byte id) { var tr = Trace; if (tr != null) tr(npc, "option", id, t); d.AddOption(t, id); return this; }
            public void Finish() { var tr = Trace; if (tr != null) tr(npc, "finish", 0, ""); d.FinalizeDialog(); }
        }

        // ------------------------------------------------------------------ per-character state
        public sealed class State { public int Level = 1; public int Exp; public bool FreeUsed; }
        static Dictionary<uint, State> _state;
        static readonly object _stateLock = new object();
        static long _seq;

        static string Dir
        {
            get
            {
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "FragmentWheel";
            }
        }
        static string StatePath { get { return Path.Combine(Dir, "state.txt"); } }

        public static State GetState(uint uid)
        {
            lock (_stateLock)
            {
                EnsureState();
                State s;
                if (!_state.TryGetValue(uid, out s)) return new State();
                return new State { Level = s.Level, Exp = s.Exp, FreeUsed = s.FreeUsed };
            }
        }

        static void SetState(uint uid, State s)
        {
            lock (_stateLock)
            {
                EnsureState();
                _state[uid] = new State { Level = s.Level, Exp = s.Exp, FreeUsed = s.FreeUsed };
                Directory.CreateDirectory(Dir);
                File.AppendAllText(StatePath, Row(uid, s, ++_seq) + "\n", Encoding.ASCII);
            }
        }

        static string Row(uint uid, State s, long seq)
        {
            return uid + "|" + s.Level + "|" + s.Exp + "|" + (s.FreeUsed ? 1 : 0) + "|" + seq;
        }

        static void EnsureState()
        {
            if (_state != null) return;
            var m = new Dictionary<uint, State>();
            string path = StatePath;
            if (File.Exists(path))
            {
                foreach (var raw in File.ReadAllLines(path))
                {
                    var p = raw.Trim().Split('|');
                    uint uid; int lv, exp, fu; long seq;
                    if (p.Length < 4 || !uint.TryParse(p[0], out uid) || !int.TryParse(p[1], out lv) || !int.TryParse(p[2], out exp) || !int.TryParse(p[3], out fu)) continue;
                    if (lv < 1 || lv > 12 || exp < 0 || uid == 0) continue;
                    m[uid] = new State { Level = lv, Exp = exp, FreeUsed = fu != 0 };
                    if (p.Length > 4 && long.TryParse(p[4], out seq) && seq > _seq) _seq = seq;
                }
                // compact: one line per character (write a temp file, then swap)
                try
                {
                    var sb = new StringBuilder();
                    long n = 0;
                    foreach (var kv in m) sb.Append(Row(kv.Key, kv.Value, ++n)).Append('\n');
                    _seq = Math.Max(_seq, n);
                    string tmp = path + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), Encoding.ASCII);
                    File.Copy(path, path + ".bak", true);
                    File.Copy(tmp, path, true);
                    File.Delete(tmp);
                }
                catch (Exception e) { Log("state compact failed: " + e.Message); }
            }
            _state = m;
        }

        // ------------------------------------------------------------------ config
        public sealed class LevelRow { public int Exp; public int Cost; }
        public sealed class Reward { public string Key; public string Name; public uint[] Items; public bool Bound; public int Count; public int Weight; }
        public sealed class Config
        {
            public bool Enabled = true;
            public uint FragmentItem = 3327170;
            public LevelRow[] Levels = new LevelRow[12];
            public List<Reward>[] Rewards = new List<Reward>[12];
            public bool ExpOnlyFromCurrentLevel = true;
            public int MinPlayerLevel = 70;
            public int SpinCooldownMs = 1500;
            public int ConfirmTimeoutSec = 60;
            static readonly int[] OffExp = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 0 };
            static readonly int[] OffCost = { 40000, 50000, 60000, 70000, 80000, 90000, 100000, 120000, 140000, 160000, 180000, 200000 };
            public bool MatchesOfficialTable()
            {
                for (int i = 0; i < 12; i++) if (Levels[i] == null || Levels[i].Exp != OffExp[i] || Levels[i].Cost != OffCost[i]) return false;
                return true;
            }
            public static Config Official()
            {
                var c = new Config();
                for (int i = 0; i < 12; i++) { c.Levels[i] = new LevelRow { Exp = OffExp[i], Cost = OffCost[i] }; c.Rewards[i] = new List<Reward>(); }
                return c;
            }
        }

        static volatile Config _cfg;
        static DateTime _cfgStamp = DateTime.MinValue;
        static readonly object _cfgLock = new object();

        public static Config Cfg
        {
            get
            {
                try
                {
                    string path = Path.Combine(Dir, "config.json");
                    DateTime t = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                    if (_cfg == null || t != _cfgStamp)
                        lock (_cfgLock)
                            if (_cfg == null || t != _cfgStamp) { _cfg = Load(path); _cfgStamp = t; }
                }
                catch (Exception e)
                {
                    Log("config error (wheel closed until fixed): " + e.Message);
                    var off = Config.Official(); off.Enabled = false; _cfg = off;
                }
                return _cfg;
            }
        }

        static Config Load(string path)
        {
            var c = Config.Official();
            if (!File.Exists(path)) { Log("config.json missing -> wheel closed"); c.Enabled = false; return c; }
            var root = AdminApi.AdminJson.Parse(File.ReadAllText(path, Encoding.UTF8)) as Dictionary<string, object>;
            if (root == null) throw new Exception("config.json is not an object");
            c.Enabled = B(root, "enabled", true);
            c.FragmentItem = (uint)D(root, "fragmentItemId", c.FragmentItem);
            c.ExpOnlyFromCurrentLevel = B(root, "expOnlyFromCurrentLevel", true);
            c.MinPlayerLevel = (int)D(root, "minPlayerLevel", c.MinPlayerLevel);
            c.SpinCooldownMs = Math.Max(200, (int)D(root, "spinCooldownMs", c.SpinCooldownMs));
            c.ConfirmTimeoutSec = (int)D(root, "confirmTimeoutSec", c.ConfirmTimeoutSec);
            var levels = Arr(root, "levels");
            if (levels != null)
                foreach (var o in levels)
                {
                    var m = o as Dictionary<string, object>;
                    if (m == null) continue;
                    int lv = (int)D(m, "level", 0);
                    if (lv < 1 || lv > 12) throw new Exception("bad level " + lv);
                    int exp = (int)D(m, "expToNext", 0), cost = (int)D(m, "cost", 0);
                    if (exp < 0 || cost <= 0 || cost > 10000000) throw new Exception("bad exp/cost at level " + lv);
                    c.Levels[lv - 1] = new LevelRow { Exp = exp, Cost = cost };
                }
            var prizes = Obj(root, "prizes");
            if (prizes == null) throw new Exception("prizes missing");
            var rewards = Arr(root, "rewards");
            if (rewards == null) throw new Exception("rewards missing");
            foreach (var o in rewards)
            {
                var m = o as Dictionary<string, object>;
                if (m == null) continue;
                int lv = (int)D(m, "level", 0);
                if (lv < 1 || lv > 12) throw new Exception("bad reward level " + lv);
                var list = new List<Reward>();
                var tab = Arr(m, "table");
                if (tab != null)
                    foreach (var t in tab)
                    {
                        var e = t as Dictionary<string, object>;
                        if (e == null) continue;
                        string key = S(e, "prize", "");
                        var pz = Obj(prizes, key);
                        if (pz == null) throw new Exception("unknown prize '" + key + "' at level " + lv);
                        var ids = Arr(pz, "items");
                        if (ids == null || ids.Count == 0) throw new Exception("prize '" + key + "' has no items");
                        var arr = new uint[ids.Count];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = (uint)Num(ids[i]);
                            Database.ItemType.DBItem db;
                            if (Pool.ItemsBase != null && Pool.ItemsBase.Count > 0 && !Pool.ItemsBase.TryGetValue(arr[i], out db))
                                throw new Exception("prize '" + key + "' item " + arr[i] + " is not in itemtype");
                        }
                        int count = (int)D(e, "count", 1), weight = (int)D(e, "weight", 0);
                        if (count < 1 || count > 20) throw new Exception("bad count at level " + lv);
                        if (weight <= 0) continue;
                        list.Add(new Reward { Key = key, Name = S(pz, "name", key), Items = arr, Bound = B(pz, "bound", false), Count = count, Weight = weight });
                    }
                c.Rewards[lv - 1] = list;
            }
            for (int i = 0; i < 12; i++)
            {
                if (c.Levels[i] == null) throw new Exception("level " + (i + 1) + " missing");
                if (c.Rewards[i] == null || c.Rewards[i].Count == 0) throw new Exception("no rewards for level " + (i + 1));
            }
            Log("config loaded: enabled=" + c.Enabled + " fragment=" + c.FragmentItem + " minLevel=" + c.MinPlayerLevel + " officialTable=" + c.MatchesOfficialTable());
            return c;
        }

        static Reward Roll(List<Reward> table)
        {
            long sum = 0;
            foreach (var r in table) sum += r.Weight;
            if (sum <= 0) return null;
            long x = RandInt((int)Math.Min(int.MaxValue, sum));
            foreach (var r in table) { if (x < r.Weight) return r; x -= r.Weight; }
            return table[table.Count - 1];
        }

        static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        static int RandInt(int max)
        {
            if (max <= 1) return 0;
            var b = new byte[4];
            uint limit = uint.MaxValue - (uint.MaxValue % (uint)max), v;
            do { lock (_rng) _rng.GetBytes(b); v = BitConverter.ToUInt32(b, 0); } while (v >= limit);
            return (int)(v % (uint)max);
        }

        static Dictionary<string, object> Obj(Dictionary<string, object> d, string k) { object o; return d.TryGetValue(k, out o) ? o as Dictionary<string, object> : null; }
        static List<object> Arr(Dictionary<string, object> d, string k) { object o; return d.TryGetValue(k, out o) ? o as List<object> : null; }
        static double Num(object o)
        {
            if (o == null) return 0;
            if (o is bool) return (bool)o ? 1 : 0;
            double v; return double.TryParse(o.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : 0;
        }
        static double D(Dictionary<string, object> d, string k, double def) { object o; return d.TryGetValue(k, out o) && o != null ? Num(o) : def; }
        static bool B(Dictionary<string, object> d, string k, bool def) { object o; return d.TryGetValue(k, out o) && o is bool ? (bool)o : def; }
        static string S(Dictionary<string, object> d, string k, string def) { object o; return d.TryGetValue(k, out o) && o is string ? (string)o : def; }

        // ------------------------------------------------------------------ log
        static readonly object _logLock = new object();
        static void Log(Client.GameClient c, string text)
        {
            string who = c != null && c.Player != null ? c.Player.Name + "(" + c.Player.UID + ")" : "?";
            Log(who + " " + text);
        }
        static void Log(string text)
        {
            try
            {
                string dir = Path.Combine(Dir, "logs");
                lock (_logLock)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "wheel_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
