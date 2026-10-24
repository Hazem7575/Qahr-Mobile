// [Lottery 2026-10-14] Shared pieces of the Golden Tree (Global Lottery) and the turnover (flop) lottery:
// config file, crypto RNG, per-client throttle, the prize ledger + pending-delivery store, and the log.
//
// Prizes are never given straight from a draw. Every win is first written to the append-only ledger
// (Data\Lottery\_wins.log) and to pending.txt, then delivered. Delivery marks the prize delivered and saves
// BEFORE the item is added, so a crash / duplicate packet / relog can lose nothing and repeat nothing. A full bag keeps
// the prize pending and it is delivered automatically when there is room (official Server_Cn_Res
// STR_GLOBALLOTTERY_BACKPACK_FULL / STR_CARDSLOTTERY_BACKPACK_FULL: "...login again to receive a reward").
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Qahr.Mobile.Lottery
{
    public sealed class LtPrize
    {
        public uint Id;
        public uint Uid;
        public uint Item;
        public bool Bound;
        public uint LifeMinutes;
        public ushort Count = 1;
        public string Source = "";
        public bool Delivered;
    }

    public static class LotteryCommon
    {
        public static readonly object Sync = new object();

        // test seams
        public static Action<Client.GameClient, ServerSockets.Packet> SendHook = (c, p) => c.Send(p);
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<IEnumerable<Client.GameClient>> Online = () =>
            Pool.GamePoll == null ? Enumerable.Empty<Client.GameClient>() : Pool.GamePoll.Values.Where(c => c != null && c.Player != null && c.FullLoading);

        public static string Dir
        {
            get
            {
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db + "Lottery";
            }
        }
        public static string DbRoot
        {
            get
            {
                string db = Program.ServerConfig.DbLocation ?? "";
                if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                return db;
            }
        }

        public static bool Ready(Client.GameClient c) { return c != null && c.Player != null && c.FullLoading && c.Player.UID != 0; }

        // ------------------------------------------------------------------ config (Lottery.ini, [Section] key=value ; comment)
        public static Dictionary<string, Dictionary<string, string>> ReadIni(string path)
        {
            var d = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            d[""] = cur;
            if (!File.Exists(path)) return d;
            foreach (string raw in File.ReadAllLines(path, Encoding.GetEncoding(28591)))
            {
                string line = raw;
                int sc = line.IndexOf(';'); if (sc >= 0) line = line.Substring(0, sc);
                int cm = line.IndexOf("//"); if (cm >= 0) line = line.Substring(0, cm);
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string name = line.Substring(1, line.Length - 2).Trim();
                    if (!d.TryGetValue(name, out cur)) d[name] = cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                cur[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            return d;
        }
        public static string Get(Dictionary<string, Dictionary<string, string>> ini, string sec, string key, string def = null)
        {
            Dictionary<string, string> s; string v;
            return ini.TryGetValue(sec, out s) && s.TryGetValue(key, out v) ? v : def;
        }
        public static long GetLong(Dictionary<string, Dictionary<string, string>> ini, string sec, string key, long def)
        {
            long v;
            return long.TryParse(Get(ini, sec, key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : def;
        }

        // ------------------------------------------------------------------ crypto RNG
        static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public static ulong RandU64() { byte[] b = new byte[8]; lock (rng) rng.GetBytes(b); return BitConverter.ToUInt64(b, 0); }
        /// <summary>Uniform 0..total-1 (no modulo bias).</summary>
        public static ulong Below(ulong total)
        {
            if (total <= 1) return 0;
            ulong limit = ulong.MaxValue - (ulong.MaxValue % total), v;
            do v = RandU64(); while (v >= limit);
            return v % total;
        }
        /// <summary>Index picked with probability weight[i] / sum (weights &lt;= 0 never picked); -1 if nothing can be picked.</summary>
        public static int PickWeighted(IList<long> weights)
        {
            ulong sum = 0;
            foreach (var w in weights) if (w > 0) sum += (ulong)w;
            if (sum == 0) return -1;
            ulong r = Below(sum);
            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] <= 0) continue;
                if (r < (ulong)weights[i]) return i;
                r -= (ulong)weights[i];
            }
            return -1;
        }

        // ------------------------------------------------------------------ throttle
        sealed class Slots { public readonly int[] Last = new int[8]; public readonly bool[] Used = new bool[8]; public bool Pushed; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> slots = new ConditionalWeakTable<Client.GameClient, Slots>();
        public static bool Allow(Client.GameClient c, int slot, int ms)
        {
            var s = slots.GetOrCreateValue(c);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < ms) return false;
                s.Used[slot] = true; s.Last[slot] = now; return true;
            }
        }
        public static void ResetThrottle(Client.GameClient c) { slots.Remove(c); }
        /// <summary>True once per client object (login push).</summary>
        public static bool FirstSeen(Client.GameClient c)
        {
            var s = slots.GetOrCreateValue(c);
            lock (s) { if (s.Pushed) return false; s.Pushed = true; return true; }
        }

        // ------------------------------------------------------------------ send
        public static void Send(Client.GameClient c, object proto, ushort id)
        {
            if (c == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                s.InitWriter();
                s.ProtoBufferSerialize(proto);
                s.Finalize(id);
                if (s.Size > 1032) { Log("packet " + id + " too big (" + s.Size + "), not sent"); return; }
                SendHook(c, s);
            }
        }
        public static void Tell(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        // ------------------------------------------------------------------ currencies (deduct first; the setters log)
        /// <summary>Takes `count` units of an item (bound and/or unbound). Checks the whole amount first.</summary>
        public static bool TakeItem(Client.GameClient c, uint item, uint count, bool allowUnbound, bool allowBound)
        {
            if (count == 0 || count > 1000000) return false;
            var list = c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == item && (i.Bound != 0 ? allowBound : allowUnbound))
                .OrderByDescending(i => i.Bound).ToList();// bound first
            ulong have = 0;
            foreach (var i in list) have += i.StackSize > 1 ? i.StackSize : 1u;
            if (have < count) return false;
            uint left = count;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                foreach (var i in list)
                {
                    if (left == 0) break;
                    uint n = i.StackSize > 1 ? i.StackSize : 1u;
                    uint take = Math.Min(n, left);
                    c.Inventory.RemoveStackItem(i.UID, (ushort)take, s);
                    left -= take;
                }
            }
            return left == 0;
        }
        public static ulong CountItem(Client.GameClient c, uint item, bool allowUnbound, bool allowBound)
        {
            ulong have = 0;
            foreach (var i in c.Inventory.ClientItems.Values)
                if (i.ITEM_ID == item && (i.Bound != 0 ? allowBound : allowUnbound)) have += i.StackSize > 1 ? i.StackSize : 1u;
            return have;
        }
        public static bool TakeCps(Client.GameClient c, ulong amount)
        {
            if (amount > int.MaxValue || c.Player.ConquerPoints < amount) return false;
            c.Player.ConquerPoints -= (uint)amount;
            return true;
        }
        public static bool TakeSilver(Client.GameClient c, ulong amount)
        {
            if (amount > 100000000000UL || c.Player.Money < amount) return false;
            c.Player.Money -= amount;
            using (var rec = new ServerSockets.RecycledPacket())
                try { c.Player.SendUpdate(rec.GetStream(), c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money); } catch { }
            return true;
        }
        /// <summary>CPs (B) first, then CPs (STR_TURNOVER_LOTTERY_COST_HINT).</summary>
        public static bool TakeBoundThenCps(Client.GameClient c, ulong amount)
        {
            if (amount > int.MaxValue) return false;
            long bound = Math.Max(0, c.Player.BoundConquerPoints);
            if ((ulong)bound + c.Player.ConquerPoints < amount) return false;
            long fromBound = Math.Min(bound, (long)amount);
            ulong rest = amount - (ulong)fromBound;
            if (fromBound > 0) c.Player.BoundConquerPoints -= (int)fromBound;
            if (rest > 0) c.Player.ConquerPoints -= (uint)rest;
            return true;
        }

        // ------------------------------------------------------------------ prize store
        static readonly Dictionary<uint, LtPrize> prizes = new Dictionary<uint, LtPrize>();
        static uint nextPrize = 1;
        static bool prizesLoaded;
        static string PendingFile { get { return Path.Combine(Dir, "pending.txt"); } }
        static string LedgerFile { get { return Path.Combine(Dir, "_wins.log"); } }

        static void LoadPrizes()
        {
            if (prizesLoaded) return;
            prizesLoaded = true;
            Directory.CreateDirectory(Dir);
            var delivered = new HashSet<uint>();
            if (File.Exists(LedgerFile))
                foreach (var l in File.ReadAllLines(LedgerFile))
                {
                    var p = l.Split('|');
                    uint id;
                    if (p.Length >= 3 && p[1] == "DELIVERED" && uint.TryParse(p[2], out id)) delivered.Add(id);
                    if (p.Length >= 3 && p[1] == "WIN" && uint.TryParse(p[2], out id) && id >= nextPrize) nextPrize = id + 1;
                }
            if (File.Exists(PendingFile))
                foreach (var l in File.ReadAllLines(PendingFile))
                {
                    var p = l.Split('\t');
                    if (p.Length < 8) continue;
                    var z = new LtPrize
                    {
                        Id = uint.Parse(p[0]), Uid = uint.Parse(p[1]), Item = uint.Parse(p[2]), Bound = p[3] == "1",
                        LifeMinutes = uint.Parse(p[4]), Count = ushort.Parse(p[5]), Source = p[6], Delivered = p[7] == "1"
                    };
                    if (delivered.Contains(z.Id)) z.Delivered = true;// ledger wins over a rolled-back pending file
                    if (!z.Delivered) prizes[z.Id] = z;
                    if (z.Id >= nextPrize) nextPrize = z.Id + 1;
                }
        }

        static void SavePrizes()
        {
            var sb = new StringBuilder();
            foreach (var z in prizes.Values.Where(x => !x.Delivered))
                sb.Append(z.Id).Append('\t').Append(z.Uid).Append('\t').Append(z.Item).Append('\t').Append(z.Bound ? 1 : 0).Append('\t')
                  .Append(z.LifeMinutes).Append('\t').Append(z.Count).Append('\t').Append(z.Source.Replace('\t', ' ')).Append("\t0\n");
            Directory.CreateDirectory(Dir);
            string tmp = PendingFile + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            if (File.Exists(PendingFile)) File.Replace(tmp, PendingFile, PendingFile + ".bak"); else File.Move(tmp, PendingFile);
        }

        static void Ledger(string line)
        {
            Directory.CreateDirectory(Dir);
            File.AppendAllText(LedgerFile, Now().ToString("yyyy-MM-dd HH:mm:ss") + "|" + line + Environment.NewLine, Encoding.UTF8);
        }

        /// <summary>Records a won prize (ledger + pending) and tries to deliver it now. Caller holds Sync.</summary>
        public static LtPrize AddPrize(Client.GameClient c, uint item, bool bound, uint lifeMinutes, ushort count, string source)
        {
            LoadPrizes();
            var z = new LtPrize { Id = nextPrize++, Uid = c.Player.UID, Item = item, Bound = bound, LifeMinutes = lifeMinutes, Count = Math.Max((ushort)1, count), Source = source };
            Ledger("WIN|" + z.Id + "|" + z.Uid + "|" + c.Player.Name + "|" + item + "|" + z.Count + "|" + (bound ? "bound" : "free") + "|" + source);
            prizes[z.Id] = z;
            SavePrizes();
            Deliver(c);
            return z;
        }

        /// <summary>Delivers this player's pending prizes that fit in the bag. Returns how many are still pending.</summary>
        public static int Deliver(Client.GameClient c)
        {
            if (!Ready(c)) return 0;
            lock (Sync)
            {
                LoadPrizes();
                var mine = prizes.Values.Where(z => z.Uid == c.Player.UID && !z.Delivered).OrderBy(z => z.Id).ToList();
                int left = 0;
                foreach (var z in mine)
                {
                    Database.ItemType.DBItem dbi;
                    if (!Pool.ItemsBase.TryGetValue(z.Item, out dbi)) { left++; continue; }
                    int stack = Math.Max(1, (int)dbi.StackSize);
                    int slots = z.LifeMinutes > 0 ? z.Count : (z.Count + stack - 1) / stack;
                    if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) { left++; continue; }
                    // mark first, then give
                    z.Delivered = true;
                    Ledger("DELIVERED|" + z.Id + "|" + z.Uid + "|" + z.Item + "|" + z.Count);
                    prizes.Remove(z.Id);
                    SavePrizes();
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var s = rec.GetStream();
                        if (z.LifeMinutes > 0)
                            for (int i = 0; i < z.Count; i++) c.Inventory.AddTime(s, z.Item, 0, 0, 0, z.Bound, z.LifeMinutes);
                        else
                        {
                            int n = z.Count;
                            while (n > 0) { int k = Math.Min(n, stack); c.Inventory.AddItemWitchStack(z.Item, 0, (ushort)k, s, z.Bound); n -= k; }
                        }
                    }
                    Log(c.Player.Name + "(" + c.Player.UID + ") received " + z.Item + " x" + z.Count + (z.Bound ? " (B)" : "") + " [" + z.Source + "] prize " + z.Id);
                }
                return left;
            }
        }

        public static int PendingCount(uint uid) { lock (Sync) { LoadPrizes(); return prizes.Values.Count(z => z.Uid == uid && !z.Delivered); } }

        public static void ResetForTests() { lock (Sync) { prizes.Clear(); nextPrize = 1; prizesLoaded = false; } }

        // ------------------------------------------------------------------ log
        static readonly object logLock = new object();
        public static void Log(string text)
        {
            try
            {
                string dir = Path.Combine(Dir, "logs");
                lock (logLock)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "lottery_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
