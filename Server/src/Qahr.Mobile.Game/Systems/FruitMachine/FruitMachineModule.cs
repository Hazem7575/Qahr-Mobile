using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Qahr.Mobile.FruitMachine
{
    /// <summary>
    /// [FruitMachine] Server side of the official single-player Fruit Machine (client CDlgFruitMachine,
    /// ini\FruitMachine.lua). NPCs 26694/26695/26696 (Silver) and 26698/26700/26701 (CPs) in Twin City.
    ///
    /// Wire protocol (reverse-engineered from the 7957 Conquer.exe, protobuf-lite, field numbers by hasbit order):
    ///   2212 MsgFruitMachine   {1 uint32 type, 2 u64, 3 u64, 4 u64, 5 u64, 6 u64, 7 string}
    ///        C->S type 0 Join   {2 npcId}                       (sent by the client itself when the NPC is clicked)
    ///        C->S type 1 Exit
    ///        C->S type 2 Compare (Big/Small) {2 choice^1, 3 current round win}
    ///        S->C type 0 Join   {2 npcId (must match), 3 balance>=0, 4 !=0 (fallback balance), 5 session !=0}
    ///        S->C type 2 Compare {2 drawn number, 3 new round win (0 = lost), 4 stake, 5 new balance}
    ///   2437 MsgFruitMachineGo C->S {1 uint32 multi(0/1), 2 repeated BetIcon{1 uint32 button, 2 u64 amount}, 3 string}
    ///   2214 MsgFruitMachineGoResult S->C {1 uint32 stop+1, 2 u64 total bet, 3 u64 new balance, 4 bool,
    ///        5 repeated uint32 extra stops+1 (Luck game: 3/5/7/22), 6 repeated Reward{1 uint32 type, 2 u64 amount, 3 uint32}}
    ///
    /// Everything the client defines (ring, payouts, Rate, MaxBet, Luck counts) is used as-is. The hidden server odds are
    /// DERIVED (not official) from a target RTP in Infinity[DB]\FruitMachine\config.json (re-read automatically when the
    /// file changes; no restart needed to tune odds).
    /// </summary>
    public static class FruitMachineModule
    {
        public const ushort PacketMachine = 2212, PacketGo = 2437, PacketGoResult = 2214;

        // ------------------------------------------------------------------ config
        public sealed class Machine { public uint Npc; public int Type; public ulong Rate; public ulong MaxBet; public string Name; }
        public sealed class Stop { public int Fruit = -1; public uint Mult; public bool Luck; }

        sealed class Config
        {
            public bool Enabled = true;
            public double Rtp = 0.95;
            public double LuckChance = 0.01;
            public int[] LuckCounts = { 3, 5, 7, 22 };
            public double[] LuckWeights = { 0.60, 0.28, 0.10, 0.02 };
            public double CompareWinChance = 0.475;
            public int CompareMin = 1, CompareMax = 14;   // small = CompareMin..mid, big = mid+1..CompareMax
            public uint CompareBigChoice = 1;              // value of field 2 the client sends for "big"
            public int MaxCompareRounds = 10;
            public ulong CompareMaxStake = 9999900000000UL; // client MoneyLimit
            public int SpinCooldownMs = 1000, CompareCooldownMs = 600;
            public string JoinField4 = "balance";          // "balance" or "one"
            public bool LossSendsZeroBalance = false;
            public bool ResultField4 = false;
            public Dictionary<uint, Machine> Machines = new Dictionary<uint, Machine>();
            public Stop[] Ring = new Stop[0];
            public double[] Weights = new double[0];       // per stop, derived (or weightsOverride)
            public double[] FruitRtp = new double[8];
        }

        static volatile Config _cfg;
        static DateTime _cfgStamp = DateTime.MinValue;
        static readonly object _cfgLock = new object();

        static string Dir
        {
            get
            {
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "FruitMachine";
            }
        }

        static Config Cfg
        {
            get
            {
                try
                {
                    string path = Path.Combine(Dir, "config.json");
                    DateTime t = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                    if (_cfg == null || t != _cfgStamp)
                    {
                        lock (_cfgLock)
                        {
                            if (_cfg == null || t != _cfgStamp)
                            {
                                _cfg = Load(path);
                                _cfgStamp = t;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("config error (machines disabled until fixed): " + e.Message);
                    var off = new Config(); off.Enabled = false; _cfg = off;
                }
                return _cfg;
            }
        }

        static Config Load(string path)
        {
            var c = new Config();
            if (!File.Exists(path)) { Log("config.json missing -> Fruit Machine disabled"); c.Enabled = false; return c; }
            var root = AdminApi.AdminJson.Parse(File.ReadAllText(path, Encoding.UTF8)) as Dictionary<string, object>;
            if (root == null) throw new Exception("config.json is not an object");
            c.Enabled = B(root, "enabled", true);
            c.Rtp = D(root, "rtp", 0.95);
            var luck = Obj(root, "luck");
            if (luck != null)
            {
                c.LuckChance = D(luck, "chance", c.LuckChance);
                var counts = Arr(luck, "counts");
                var weights = Arr(luck, "weights");
                if (counts != null && weights != null && counts.Count == weights.Count && counts.Count > 0)
                {
                    c.LuckCounts = new int[counts.Count]; c.LuckWeights = new double[counts.Count];
                    for (int i = 0; i < counts.Count; i++) { c.LuckCounts[i] = (int)Num(counts[i]); c.LuckWeights[i] = Num(weights[i]); }
                }
            }
            var cmp = Obj(root, "compare");
            if (cmp != null)
            {
                c.CompareWinChance = D(cmp, "winChance", c.CompareWinChance);
                c.CompareMin = (int)D(cmp, "numberMin", c.CompareMin);
                c.CompareMax = (int)D(cmp, "numberMax", c.CompareMax);
                c.CompareBigChoice = (uint)D(cmp, "bigChoiceValue", c.CompareBigChoice);
                c.MaxCompareRounds = (int)D(cmp, "maxRounds", c.MaxCompareRounds);
                c.CompareMaxStake = (ulong)D(cmp, "maxStake", c.CompareMaxStake);
            }
            var lim = Obj(root, "limits");
            if (lim != null)
            {
                c.SpinCooldownMs = (int)D(lim, "spinCooldownMs", c.SpinCooldownMs);
                c.CompareCooldownMs = (int)D(lim, "compareCooldownMs", c.CompareCooldownMs);
            }
            var proto = Obj(root, "protocol");
            if (proto != null)
            {
                c.JoinField4 = S(proto, "joinField4", c.JoinField4);
                c.LossSendsZeroBalance = B(proto, "lossSendsZeroBalance", c.LossSendsZeroBalance);
                c.ResultField4 = B(proto, "resultField4", c.ResultField4);
            }
            var ms = Arr(root, "machines");
            if (ms != null)
                foreach (var o in ms)
                {
                    var m = o as Dictionary<string, object>;
                    if (m == null) continue;
                    var mc = new Machine
                    {
                        Npc = (uint)D(m, "npc", 0),
                        Type = (int)D(m, "type", 1),
                        Rate = ULong(m, "rate", 1),
                        MaxBet = ULong(m, "maxBet", 0),
                        Name = S(m, "name", "")
                    };
                    if (mc.Npc != 0 && mc.Rate > 0 && (mc.Type == 1 || mc.Type == 2)) c.Machines[mc.Npc] = mc;
                }
            var ring = Arr(root, "ring");
            if (ring == null || ring.Count != 24) throw new Exception("ring must have 24 stops");
            c.Ring = new Stop[24];
            for (int i = 0; i < 24; i++)
            {
                var s = ring[i] as Dictionary<string, object>;
                var st = new Stop();
                if (s != null && B(s, "luck", false)) st.Luck = true;
                else if (s != null) { st.Fruit = (int)D(s, "fruit", -1); st.Mult = (uint)D(s, "mult", 0); }
                if (!st.Luck && (st.Fruit < 0 || st.Fruit > 7 || st.Mult == 0)) throw new Exception("bad ring stop " + i);
                c.Ring[i] = st;
            }
            var ow = Arr(root, "weightsOverride");
            if (ow != null && ow.Count == 24)
            {
                c.Weights = new double[24];
                double sum = 0;
                for (int i = 0; i < 24; i++) { c.Weights[i] = Math.Max(0, Num(ow[i])); sum += c.Weights[i]; }
                if (sum <= 0) throw new Exception("weightsOverride sums to 0");
                for (int i = 0; i < 24; i++) c.Weights[i] /= sum;
                ComputeRtp(c);
            }
            else Derive(c);
            var sb = new StringBuilder("config loaded: " + c.Machines.Count + " machines, rtp target " + F(c.Rtp) + ", real per-fruit RTP:");
            for (int k = 0; k < 8; k++) sb.Append(' ').Append(F(c.FruitRtp[k]));
            sb.Append(" | stop weights:");
            for (int i = 0; i < 24; i++) sb.Append(' ').Append(F(c.Weights[i]));
            Log(sb.ToString());
            return c;
        }

        /// <summary>
        /// Derives stop weights so EVERY fruit (bet button) returns exactly Rtp, Luck game included:
        ///   Luck lands with probability LuckChance and lights N distinct non-luck stops chosen uniformly (N from luck.counts),
        ///   so it adds LuckChance*E[N]/22*sum(mult of the fruit) to that fruit's RTP. The rest of the fruit's RTP is split
        ///   between its x3 stop and its big stops with one shared factor alpha, chosen so all probabilities sum to 1.
        /// </summary>
        static void Derive(Config c)
        {
            int luckStops = 0, plain = 0;
            foreach (var s in c.Ring) if (s.Luck) luckStops++; else plain++;
            double luck = luckStops > 0 ? c.LuckChance : 0;
            double L = plain > 0 ? luck * ExpectedLuck(c, plain) / plain : 0;
            var sumMult = new double[8]; var rp = new double[8]; var lowCnt = new int[8]; var highCnt = new int[8]; var highInv = new double[8];
            foreach (var s in c.Ring)
                if (!s.Luck) { sumMult[s.Fruit] += s.Mult; if (s.Mult <= 3) lowCnt[s.Fruit]++; else { highCnt[s.Fruit]++; highInv[s.Fruit] += 1.0 / s.Mult; } }
            double A = 0, Bc = 0;
            for (int k = 0; k < 8; k++)
            {
                if (lowCnt[k] + highCnt[k] == 0) continue;
                rp[k] = c.Rtp - L * sumMult[k];
                if (rp[k] <= 0) throw new Exception("luck.chance too high for fruit " + k + " (luck alone exceeds the RTP)");
                if (highCnt[k] == 0) { A += rp[k] / 3; continue; }
                double hp = rp[k] / highCnt[k] * highInv[k];
                A += hp;
                if (lowCnt[k] > 0) Bc += rp[k] / 3 - hp;
            }
            double alpha = Bc != 0 ? (1 - luck - A) / Bc : 0;
            if (alpha < 0 || alpha > 1) { Log("WARNING: rtp " + F(c.Rtp) + " is not reachable with this ring (alpha " + F(alpha) + "), clamped; check the real RTP line"); alpha = Math.Max(0, Math.Min(1, alpha)); }
            var w = new double[24];
            for (int i = 0; i < 24; i++)
            {
                var s = c.Ring[i];
                if (s.Luck) { w[i] = luck / luckStops; continue; }
                int k = s.Fruit;
                if (s.Mult <= 3) w[i] = (highCnt[k] == 0 ? 1 : alpha) * rp[k] / 3 / lowCnt[k];
                else w[i] = (lowCnt[k] > 0 ? 1 - alpha : 1) * rp[k] / highCnt[k] / s.Mult;
            }
            double sum = 0; foreach (var x in w) sum += x;
            for (int i = 0; i < 24; i++) w[i] /= sum;
            c.Weights = w;
            ComputeRtp(c);
        }

        static double ExpectedLuck(Config c, int plain)
        {
            double lw = 0, en = 0;
            for (int i = 0; i < c.LuckCounts.Length; i++) lw += c.LuckWeights[i];
            if (lw <= 0) return 0;
            for (int i = 0; i < c.LuckCounts.Length; i++) en += Math.Min(plain, c.LuckCounts[i]) * (c.LuckWeights[i] / lw);
            return en;
        }

        static void ComputeRtp(Config c)
        {
            int plain = 0; foreach (var s in c.Ring) if (!s.Luck) plain++;
            double pl = 0;
            for (int i = 0; i < 24; i++) if (c.Ring[i].Luck) pl += c.Weights[i];
            double en = ExpectedLuck(c, plain);
            for (int k = 0; k < 8; k++)
            {
                double r = 0, sm = 0;
                for (int i = 0; i < 24; i++) if (!c.Ring[i].Luck && c.Ring[i].Fruit == k) { r += c.Weights[i] * c.Ring[i].Mult; sm += c.Ring[i].Mult; }
                c.FruitRtp[k] = r + (plain > 0 ? pl * en / plain * sm : 0);
            }
        }

        // ------------------------------------------------------------------ sessions
        sealed class Session
        {
            public readonly object Gate = new object();
            public uint Npc;
            public ulong Id;
            public long LastSpin = long.MinValue / 2, LastCompare = long.MinValue / 2;
            public ulong CompareStake;   // win of the last round that may still be gambled (already credited)
            public int CompareRounds;
        }
        static readonly ConcurrentDictionary<uint, Session> _sessions = new ConcurrentDictionary<uint, Session>();
        static long _nextSession = (DateTime.UtcNow.Ticks >> 20) & 0x7FFFFFFF;

        static long Now { get { return System.Diagnostics.Stopwatch.GetTimestamp() / (System.Diagnostics.Stopwatch.Frequency / 1000); } }

        // ------------------------------------------------------------------ packet entry points
        public static void OnMachine(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                if (user == null || user.Player == null) return;
                var f = ReadFields(stream);
                uint type = (uint)First(f, 1);
                switch (type)
                {
                    case 0: Join(user, stream, (uint)First(f, 2)); break;
                    case 1: Exit(user); break;
                    case 2: Compare(user, stream, First(f, 2), First(f, 3)); break;
                    default: Log(user, "unknown 2212 type " + type); break;
                }
            }
            catch (Exception e) { Log(user, "2212 error: " + e.Message); }
        }

        public static void OnGo(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                if (user == null || user.Player == null) return;
                var f = ReadFields(stream);
                var bets = new List<KeyValuePair<uint, ulong>>();
                List<object> raw;
                if (f.TryGetValue(2, out raw))
                    foreach (var o in raw)
                    {
                        var b = o as byte[];
                        if (b == null) continue;
                        var bf = Parse(b, 0, b.Length);
                        bets.Add(new KeyValuePair<uint, ulong>((uint)First(bf, 1), First(bf, 2)));
                    }
                Spin(user, stream, (uint)First(f, 1), bets);
            }
            catch (Exception e) { Log(user, "2437 error: " + e.Message); }
        }

        // ------------------------------------------------------------------ handlers
        static bool NearMachine(Client.GameClient user, uint npc)
        {
            Game.MsgNpc.Npc obj;
            return user.Map != null && user.Map.SearchNpcInScreen(npc, user.Player.X, user.Player.Y, out obj);
        }

        static ulong Balance(Client.GameClient user, Machine m)
        {
            return m.Type == 1 ? user.Player.Money : user.Player.ConquerPoints;
        }

        static void Join(Client.GameClient user, ServerSockets.Packet stream, uint npc)
        {
            var c = Cfg;
            Machine m;
            if (!c.Enabled || !c.Machines.TryGetValue(npc, out m)) { Log(user, "join refused: npc " + npc + (c.Enabled ? " is not a machine" : " (disabled)")); return; }
            if (!NearMachine(user, npc)) { Log(user, "join refused: not near npc " + npc); return; }
            var s = _sessions.GetOrAdd(user.Player.UID, k => new Session());
            lock (s.Gate)
            {
                s.Npc = npc;
                s.Id = (ulong)System.Threading.Interlocked.Increment(ref _nextSession);
                s.CompareStake = 0; s.CompareRounds = 0;
                ulong bal = Balance(user, m);
                ulong f4 = c.JoinField4 == "one" ? 1UL : Math.Max(1UL, bal);
                var w = new List<byte>();
                VarField(w, 1, 0); VarField(w, 2, npc); VarField(w, 3, bal); VarField(w, 4, f4); VarField(w, 5, s.Id);
                user.Send(Build(stream, PacketMachine, w));
                Log(user, "join npc " + npc + " (" + m.Name + ") balance " + bal + " session " + s.Id);
            }
        }

        static void Exit(Client.GameClient user)
        {
            Session s;
            if (_sessions.TryRemove(user.Player.UID, out s)) Log(user, "exit npc " + s.Npc);
        }

        static void Spin(Client.GameClient user, ServerSockets.Packet stream, uint multi, List<KeyValuePair<uint, ulong>> bets)
        {
            var c = Cfg;
            Session s;
            Machine m;
            if (!c.Enabled || !_sessions.TryGetValue(user.Player.UID, out s) || !c.Machines.TryGetValue(s.Npc, out m)) { Log(user, "spin refused: no session"); return; }
            lock (s.Gate)
            {
                if (multi != 0) { Log(user, "spin refused: multiplayer flag"); return; }
                if (Now - s.LastSpin < c.SpinCooldownMs) { Log(user, "spin refused: rate limit"); return; }
                if (!NearMachine(user, s.Npc)) { Log(user, "spin refused: left the machine"); return; }
                if (user.InTrade) { Log(user, "spin refused: in trade"); return; }
                var perFruit = new ulong[8];
                ulong total = 0;
                foreach (var b in bets)
                {
                    if (b.Key > 7 || b.Value == 0 || b.Value % m.Rate != 0 || (m.MaxBet > 0 && b.Value > m.MaxBet)) { Log(user, "spin refused: bad bet " + b.Key + "=" + b.Value); Refuse(user, m); return; }
                    perFruit[b.Key] += b.Value;
                    if (m.MaxBet > 0 && perFruit[b.Key] > m.MaxBet) { Log(user, "spin refused: max bet"); Refuse(user, m); return; }
                    total += b.Value;
                }
                if (total == 0) { Log(user, "spin refused: empty bet"); return; }
                ulong before = Balance(user, m);
                if (before < total) { Log(user, "spin refused: balance " + before + " < " + total); Refuse(user, m); return; }
                s.LastSpin = Now;

                // 1) deduct first
                if (!Take(user, stream, m, total)) { Refuse(user, m); return; }

                // 2) draw (crypto RNG)
                int stop = Pick(c.Weights);
                var extras = c.Ring[stop].Luck ? LuckStops(c) : new List<int>();
                ulong win = 0;
                if (c.Ring[stop].Luck) { foreach (int e in extras) win += Pay(c.Ring[e], perFruit); }
                else win = Pay(c.Ring[stop], perFruit);

                // 3) pay
                if (win > 0) Give(user, stream, m, win);
                s.CompareStake = win; s.CompareRounds = 0;
                ulong after = Balance(user, m);

                var w = new List<byte>();
                VarField(w, 1, (ulong)(stop + 1));
                VarField(w, 2, total);
                VarField(w, 3, (win == 0 && c.LossSendsZeroBalance) ? 0UL : after);
                VarField(w, 4, c.ResultField4 && win > 0 ? 1UL : 0UL);
                foreach (int e in extras) VarField(w, 5, (ulong)(e + 1));
                if (win > 0)
                {
                    var r = new List<byte>();
                    VarField(r, 1, (ulong)m.Type); VarField(r, 2, win); VarField(r, 3, 0);
                    BytesField(w, 6, r.ToArray());
                }
                user.Send(Build(stream, PacketGoResult, w));

                var sb = new StringBuilder();
                for (int k = 0; k < 8; k++) if (perFruit[k] > 0) sb.Append(k).Append('=').Append(perFruit[k]).Append(' ');
                Log(user, "SPIN npc " + s.Npc + " " + (m.Type == 1 ? "silver" : "cps") + " bets[" + sb.ToString().Trim() + "] total " + total
                    + " stop " + stop + (extras.Count > 0 ? " luck+" + extras.Count + "[" + string.Join(",", extras) + "]" : "")
                    + " win " + win + " balance " + before + " -> " + after);
            }
        }

        static void Compare(Client.GameClient user, ServerSockets.Packet stream, ulong choiceField, ulong clientWin)
        {
            var c = Cfg;
            Session s;
            Machine m;
            if (!c.Enabled || !_sessions.TryGetValue(user.Player.UID, out s) || !c.Machines.TryGetValue(s.Npc, out m)) { Log(user, "compare refused: no session"); return; }
            lock (s.Gate)
            {
                if (Now - s.LastCompare < c.CompareCooldownMs) { Log(user, "compare refused: rate limit"); return; }
                if (!NearMachine(user, s.Npc) || user.InTrade) { Log(user, "compare refused: away/trade"); return; }
                ulong stake = s.CompareStake;
                if (stake == 0 || s.CompareRounds >= c.MaxCompareRounds || stake > c.CompareMaxStake) { Log(user, "compare refused: stake " + stake + " rounds " + s.CompareRounds); return; }
                if (clientWin != stake) Log(user, "compare note: client win " + clientWin + " != server " + stake + " (server value used)");
                ulong before = Balance(user, m);
                if (before < stake) { Log(user, "compare refused: balance"); return; }
                s.LastCompare = Now;
                if (!Take(user, stream, m, stake)) return;
                bool big = choiceField == c.CompareBigChoice;
                bool won = Rand01() < c.CompareWinChance;
                int mid = (c.CompareMin + c.CompareMax) / 2;
                bool numberBig = won ? big : !big;
                int number = numberBig ? RandInt(mid + 1, c.CompareMax) : RandInt(c.CompareMin, mid);
                ulong newWin = won ? stake * 2 : 0;
                if (newWin > 0) Give(user, stream, m, newWin);
                s.CompareStake = newWin; s.CompareRounds++;
                ulong after = Balance(user, m);
                var w = new List<byte>();
                VarField(w, 1, 2); VarField(w, 2, (ulong)number); VarField(w, 3, newWin); VarField(w, 4, stake); VarField(w, 5, after);
                user.Send(Build(stream, PacketMachine, w));
                Log(user, "COMPARE npc " + s.Npc + " field2 " + choiceField + " (" + (big ? "big" : "small") + ") number " + number + " stake " + stake + " -> " + newWin + " balance " + before + " -> " + after);
            }
        }

        static void Refuse(Client.GameClient user, Machine m)
        {
            user.SendSysMesage(m.Type == 1 ? "Insufficient Silver." : "Insufficient CPs.");
        }

        static bool Take(Client.GameClient user, ServerSockets.Packet stream, Machine m, ulong amount)
        {
            if (m.Type == 1)
            {
                if (user.Player.Money < amount) return false;
                user.Player.Money -= amount;
                user.Player.SendUpdate(stream, user.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
            }
            else
            {
                if (amount > uint.MaxValue || user.Player.ConquerPoints < amount) return false;
                user.Player.ConquerPoints -= (uint)amount;
            }
            return true;
        }

        static void Give(Client.GameClient user, ServerSockets.Packet stream, Machine m, ulong amount)
        {
            if (m.Type == 1)
            {
                user.Player.Money += amount;
                user.Player.SendUpdate(stream, user.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
            }
            else
            {
                ulong v = (ulong)user.Player.ConquerPoints + amount;
                user.Player.ConquerPoints = v > uint.MaxValue ? uint.MaxValue : (uint)v;
            }
        }

        static ulong Pay(Stop st, ulong[] perFruit)
        {
            if (st.Luck || st.Fruit < 0) return 0;
            return perFruit[st.Fruit] * st.Mult;
        }

        static List<int> LuckStops(Config c)
        {
            var plain = new List<int>();
            for (int i = 0; i < 24; i++) if (!c.Ring[i].Luck) plain.Add(i);
            int n = Math.Min(plain.Count, c.LuckCounts[Pick(c.LuckWeights)]);
            for (int i = plain.Count - 1; i > 0; i--) { int j = RandInt(0, i); int t = plain[i]; plain[i] = plain[j]; plain[j] = t; }
            var r = plain.GetRange(0, n);
            r.Sort();
            return r;
        }

        // ------------------------------------------------------------------ crypto RNG
        static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        static ulong RandU64()
        {
            byte[] b = new byte[8];
            lock (_rng) _rng.GetBytes(b);
            return BitConverter.ToUInt64(b, 0);
        }
        static double Rand01() { return (RandU64() >> 11) * (1.0 / (1UL << 53)); }
        static int RandInt(int min, int maxInclusive)
        {
            ulong range = (ulong)((long)maxInclusive - min + 1);
            if (range <= 1) return min;
            ulong limit = ulong.MaxValue - (ulong.MaxValue % range);
            ulong v;
            do v = RandU64(); while (v >= limit);
            return (int)((long)min + (long)(v % range));
        }
        static int Pick(double[] w)
        {
            double sum = 0; foreach (var x in w) sum += x;
            double r = Rand01() * sum;
            for (int i = 0; i < w.Length; i++) { r -= w[i]; if (r < 0) return i; }
            return w.Length - 1;
        }

        // ------------------------------------------------------------------ protobuf (hand-written, exact wire format)
        static Dictionary<int, List<object>> ReadFields(ServerSockets.Packet stream)
        {
            stream.Seek(0);
            ushort len = stream.ReadUInt16();
            stream.ReadUInt16();
            if (len < 4 || len > 1024) return new Dictionary<int, List<object>>();
            byte[] body = stream.ReadBytes(len - 4);
            return Parse(body, 0, body.Length);
        }

        static Dictionary<int, List<object>> Parse(byte[] b, int o, int end)
        {
            var d = new Dictionary<int, List<object>>();
            while (o < end)
            {
                ulong tag = ReadVar(b, ref o, end);
                int num = (int)(tag >> 3), wt = (int)(tag & 7);
                object v;
                if (wt == 0) v = ReadVar(b, ref o, end);
                else if (wt == 2)
                {
                    ulong l = ReadVar(b, ref o, end);
                    if (l > (ulong)(end - o)) throw new Exception("bad length");
                    var x = new byte[(int)l]; Buffer.BlockCopy(b, o, x, 0, (int)l); o += (int)l; v = x;
                }
                else if (wt == 1) { if (o + 8 > end) throw new Exception("short"); v = BitConverter.ToUInt64(b, o); o += 8; }
                else if (wt == 5) { if (o + 4 > end) throw new Exception("short"); v = (ulong)BitConverter.ToUInt32(b, o); o += 4; }
                else throw new Exception("wire type " + wt);
                List<object> l2;
                if (!d.TryGetValue(num, out l2)) d[num] = l2 = new List<object>();
                if (l2.Count < 64) l2.Add(v);
            }
            return d;
        }

        static ulong ReadVar(byte[] b, ref int o, int end)
        {
            ulong r = 0; int shift = 0;
            while (true)
            {
                if (o >= end || shift > 63) throw new Exception("bad varint");
                byte x = b[o++];
                r |= (ulong)(x & 0x7F) << shift;
                if ((x & 0x80) == 0) return r;
                shift += 7;
            }
        }

        static ulong First(Dictionary<int, List<object>> f, int n)
        {
            List<object> l;
            return f.TryGetValue(n, out l) && l.Count > 0 && l[0] is ulong ? (ulong)l[0] : 0UL;
        }

        static void Var(List<byte> w, ulong v) { while (v > 0x7F) { w.Add((byte)(v | 0x80)); v >>= 7; } w.Add((byte)v); }
        static void VarField(List<byte> w, int num, ulong v) { Var(w, (ulong)(num << 3)); Var(w, v); }
        static void BytesField(List<byte> w, int num, byte[] v) { Var(w, (ulong)((num << 3) | 2)); Var(w, (ulong)v.Length); w.AddRange(v); }

        static ServerSockets.Packet Build(ServerSockets.Packet stream, ushort id, List<byte> body)
        {
            stream.InitWriter();
            foreach (byte x in body) stream.Write(x);
            stream.Finalize(id);
            return stream;
        }

        // ------------------------------------------------------------------ json helpers
        static Dictionary<string, object> Obj(Dictionary<string, object> d, string k) { object o; return d.TryGetValue(k, out o) ? o as Dictionary<string, object> : null; }
        static List<object> Arr(Dictionary<string, object> d, string k) { object o; return d.TryGetValue(k, out o) ? o as List<object> : null; }
        static double Num(object o)
        {
            if (o == null) return 0;
            if (o is bool) return (bool)o ? 1 : 0;
            double d; return double.TryParse(o.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0;
        }
        static ulong ULong(Dictionary<string, object> d, string k, ulong def)
        {
            object o; ulong v;
            return d.TryGetValue(k, out o) && o != null && ulong.TryParse(o.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def;
        }
        static double D(Dictionary<string, object> d, string k, double def) { object o; return d.TryGetValue(k, out o) && o != null ? Num(o) : def; }
        static bool B(Dictionary<string, object> d, string k, bool def) { object o; return d.TryGetValue(k, out o) && o is bool ? (bool)o : def; }
        static string S(Dictionary<string, object> d, string k, string def) { object o; return d.TryGetValue(k, out o) && o is string ? (string)o : def; }
        static string F(double v) { return v.ToString("0.#####", CultureInfo.InvariantCulture); }

        // ------------------------------------------------------------------ daily log
        static readonly object _logLock = new object();
        static void Log(Client.GameClient user, string text)
        {
            string who = user != null && user.Player != null ? user.Player.Name + "(" + user.Player.UID + ")" : "?";
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
                    File.AppendAllText(Path.Combine(dir, "spins_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
