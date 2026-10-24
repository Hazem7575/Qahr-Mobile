// [Poker 2026-10-14] New Texas MTT (multi-table tournaments).
// Official data (read-only): Data\newtexas_mtt_race.txt (Gold 31 / Platinum 32: entry cost, start stack,
// blind level seconds, daily HHMM schedule, return rate, guarantee, first blind row), newtexas_mtt_type.txt (min/max
// players), newtexas_mtt_stage.txt (big blind ladder), newtexas_mtt_prize.txt (prize table: type 4 = share of the pool in
// 1/10000, by "up to rank").
// Protocol (spec_tourney.txt sec. 5): hall 2528 f1=0 -> 2529 list (+2533 sign-up map); register 2528 f1=5 [inst, race]
// -> case 6 [inst, race, code, hhmm, yyyymmdd]; detail f1=0x17 -> case 0x17; enter f1=3 -> case 3 + case 8 + board
// (2471 f9=0xF); 2531 blinds/players; elimination -> case 0xf [inst, rank, hasPrize] + f3 [prize]; cancel for too few
// players -> case 7 + refund. Leaving the board (2472 f1=1) keeps the seat (auto-fold) until the player enters again.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Poker
{
    public static class PokerMtt
    {
        public const ushort IdMttAct = 2528, IdMttList = 2529, IdMttTableInfo = 2531, IdMttNotify = 2533;

        public sealed class Race
        {
            public uint Id; public string Name = ""; public uint MatchType; public ulong Cost; public ulong StartStack; public int LevelSeconds;
            public uint PrizeTable; public List<int> Times = new List<int>(); public ulong Guarantee; public uint ReturnRate; public int FirstStage;
            public int MinPlayers = 50, MaxPlayers = 1000; public bool Enabled;
            public List<KeyValuePair<uint, uint>> PrizeShares = new List<KeyValuePair<uint, uint>>();   // (up to rank, per 10000)
        }

        internal sealed class Entry
        {
            public uint Uid; public Client.GameClient Client; public ulong Session; public Instance Inst;
            public bool Out; public uint Rank; public bool Away; public PokerModule.TableCtx Table;
        }

        internal sealed class Instance : PokerModule.ITourney
        {
            public uint Id; public Race Race; public int Date; public int Hhmm; public DateTime StartUtcLocal;
            public readonly Dictionary<uint, Entry> Players = new Dictionary<uint, Entry>();
            public bool Started, Finished, Cancelled; public ulong Pool;
            public int Level; public long NextLevelAt;
            public readonly List<PokerModule.TableCtx> Tables = new List<PokerModule.TableCtx>();
            public readonly object Gate = new object();

            public Client.GameClient ClientOf(uint uid)
            {
                Entry e;
                return Players.TryGetValue(uid, out e) && !e.Out && !e.Away ? e.Client : null;
            }
            public void AfterResult(PokerModule.TableCtx ctx, PokerEvent ev, long now) { }   // eliminations run in TickAll (lock order inst -> table)
            public void Housekeeping(PokerModule.TableCtx ctx, long now) { }
        }

        static readonly ConcurrentDictionary<uint, Entry> entries = new ConcurrentDictionary<uint, Entry>();
        static readonly Dictionary<uint, Instance> instances = new Dictionary<uint, Instance>();
        static readonly object gate = new object();
        static Dictionary<uint, Race> races;
        static List<ulong> stages;
        static DateTime stamp;
        static System.Threading.Timer timer;
        /// <summary>Test hook: local wall clock.</summary>
        public static Func<DateTime> Now = () => DateTime.Now;

        public static bool IsIn(uint uid) { return entries.ContainsKey(uid); }
        internal static PokerModule.TableCtx TableOf(uint uid)
        {
            Entry e;
            return entries.TryGetValue(uid, out e) && !e.Out ? e.Table : null;
        }

        // ------------------------------------------------------------------ data
        static string DbRoot { get { return Path.GetDirectoryName(PokerConfig.Folder.TrimEnd('\\', '/')); } }
        static IEnumerable<string[]> Table(string name)
        {
            string p = Path.Combine(DbRoot, name);
            if (!File.Exists(p)) yield break;
            foreach (var l in File.ReadAllLines(p, Encoding.UTF8)) if (l.Trim().Length > 0) yield return l.Split(new[] { "@@" }, StringSplitOptions.None);
        }
        static uint UI(string s) { uint v; return uint.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static ulong UL(string s) { ulong v; return ulong.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        static Dictionary<string, object> Cfg { get { return PokerConfig.Obj(PokerConfig.Current.Raw, "mtt"); } }
        static bool Enabled { get { return PokerConfig.Current.Enabled && PokerConfig.B(Cfg, "enabled", false); } }

        static void EnsureData()
        {
            string cp = Path.Combine(PokerConfig.Folder, "config.json");
            var t = File.Exists(cp) ? File.GetLastWriteTimeUtc(cp) : DateTime.MinValue;
            if (races != null && t == stamp) return;
            lock (gate)
            {
                var enabled = new HashSet<uint>();
                var arr = PokerConfig.Arr(Cfg, "enabledRaces");
                if (arr != null) foreach (var o in arr) enabled.Add((uint)PokerConfig.Num(o));
                int minOverride = (int)PokerConfig.D(Cfg, "minPlayersOverride", 0);
                var types = new Dictionary<uint, int[]>();
                foreach (var l in Table("newtexas_mtt_type.txt")) if (l.Length >= 6) types[UI(l[0])] = new[] { (int)UI(l[3]), (int)UI(l[4]) };
                var prizes = new Dictionary<uint, List<KeyValuePair<uint, uint>>>();
                foreach (var l in Table("newtexas_mtt_prize.txt"))
                {
                    if (l.Length < 5 || UI(l[3]) != 4) continue;                     // type 4 = share of the pool (1/10000)
                    List<KeyValuePair<uint, uint>> list;
                    if (!prizes.TryGetValue(UI(l[1]), out list)) prizes[UI(l[1])] = list = new List<KeyValuePair<uint, uint>>();
                    list.Add(new KeyValuePair<uint, uint>(UI(l[2]), UI(l[4])));
                }
                var r = new Dictionary<uint, Race>();
                foreach (var l in Table("newtexas_mtt_race.txt"))
                {
                    if (l.Length < 21) continue;
                    var x = new Race
                    {
                        Id = UI(l[0]), Name = l[1], MatchType = UI(l[2]), Cost = UL(l[3]), StartStack = UL(l[6]), LevelSeconds = (int)UI(l[8]),
                        PrizeTable = UI(l[10]), Guarantee = UL(l[18]), ReturnRate = UI(l[19]), FirstStage = (int)UI(l[20])
                    };
                    // only daily "HHMM HHMM" schedules (dated one-off events of 2024 are skipped)
                    foreach (var part in l[13].Split(' ')) { if (part.Length == 4) x.Times.Add((int)UI(part)); }
                    if (x.Times.Count == 0 || x.Cost == 0) continue;
                    int[] mm; if (types.TryGetValue(x.MatchType, out mm)) { x.MinPlayers = mm[0]; x.MaxPlayers = mm[1]; }
                    if (minOverride > 0) x.MinPlayers = minOverride;
                    if (x.LevelSeconds < 30) x.LevelSeconds = 120;
                    if (x.ReturnRate == 0 || x.ReturnRate > 10000) x.ReturnRate = 9000;
                    List<KeyValuePair<uint, uint>> pl; if (prizes.TryGetValue(x.PrizeTable, out pl)) x.PrizeShares = pl.OrderBy(k => k.Key).ToList();
                    x.Enabled = enabled.Contains(x.Id);
                    r[x.Id] = x;
                }
                var st = new List<ulong>();
                foreach (var l in Table("newtexas_mtt_stage.txt")) if (l.Length >= 2) st.Add(UL(l[1]));   // index = level row (row 0 unused)
                if (st.Count < 2) st = new List<ulong> { 0, 20 };
                stages = st; races = r; stamp = t;
            }
        }

        // ------------------------------------------------------------------ schedule
        static int Ymd(DateTime d) { return d.Year * 10000 + d.Month * 100 + d.Day; }
        static uint InstanceId(uint race, int ymd, int hhmm) { return (uint)((ymd % 1000000) * 10000 + hhmm) ^ (race << 28); }

        /// <summary>Caller holds gate. Today's instances of every enabled race (created on demand).</summary>
        static List<Instance> Today()
        {
            EnsureData();
            var now = Now();
            int ymd = Ymd(now);
            var list = new List<Instance>();
            foreach (var race in races.Values.Where(x => x.Enabled))
                foreach (int hm in race.Times)
                {
                    uint id = InstanceId(race.Id, ymd, hm);
                    Instance inst;
                    if (!instances.TryGetValue(id, out inst))
                    {
                        inst = new Instance { Id = id, Race = race, Date = ymd, Hhmm = hm, StartUtcLocal = new DateTime(now.Year, now.Month, now.Day, hm / 100, hm % 100, 0) };
                        instances[id] = inst;
                    }
                    list.Add(inst);
                }
            return list;
        }

        static uint State(Instance i)
        {
            if (i.Started && !i.Finished) return 3;           // ongoing -> "Enter"
            if (i.Finished || i.Cancelled) return 5;
            return 1;                                         // registering (M)
        }

        static void EnsureTimer()
        {
            if (timer != null) return;
            lock (gate) if (timer == null) timer = new System.Threading.Timer(_ => { try { TickAll(); } catch (Exception e) { PokerLog.Write("mtt tick error: " + e); } }, null, 1000, 1000);
        }

        /// <summary>Starts / cancels instances whose time has come, raises blinds, balances tables.</summary>
        public static void TickAll()
        {
            if (races == null) return;
            List<Instance> due;
            lock (gate) due = instances.Values.Where(i => !i.Finished && !i.Cancelled).ToList();
            var now = Now();
            long ms = PokerModule.Clock();
            foreach (var inst in due)
            {
                lock (inst.Gate)
                {
                    if (!inst.Started && now >= inst.StartUtcLocal) Start(inst, ms);
                    else if (inst.Started && !inst.Finished) Run(inst, ms);
                }
            }
        }

        // ------------------------------------------------------------------ packets
        static void Act(Client.GameClient c, uint action, uint[] f2, ulong[] f3 = null, string f4 = null)
        {
            var w = new PbW().U(1, action).Rep(2, f2);
            if (f3 != null) w.Rep(3, f3);
            if (f4 != null) w.Str(4, f4);
            PokerModule.Send(c, w, IdMttAct);
        }

        static void SendList(Client.GameClient c)
        {
            List<Instance> today;
            lock (gate) today = Today();
            var now = Now();
            ulong stampNow = (ulong)(Ymd(now)) * 1000000UL + (ulong)(now.Hour * 10000 + now.Minute * 100 + now.Second);
            var w = new PbW().U(1, 0).U(2, 0).U(3, (ulong)today.Count);
            foreach (var i in today)
            {
                bool signed = i.Players.ContainsKey(c.Player.UID);
                w.Msg(4, new PbW().U(1, i.Id).U(2, i.Race.Id).U(3, (ulong)i.Players.Count).B(4, signed).U(5, (ulong)i.Hhmm).U(7, State(i)).U(8, (ulong)i.Date).U(9, PoolOf(i)));
            }
            w.U(5, stampNow);
            PokerModule.Send(c, w, IdMttList);
            var n = new PbW().U(1, 0);
            foreach (var i in today) n.Msg(2, new PbW().U(1, i.Race.Id).U(2, (ulong)i.Date).U(3, (ulong)i.Hhmm).B(4, i.Players.ContainsKey(c.Player.UID)).U(5, 0));
            n.U(3, stampNow);
            PokerModule.Send(c, n, IdMttNotify);
        }

        static ulong PoolOf(Instance i)
        {
            ulong pool = i.Race.Cost * (ulong)i.Players.Count * i.Race.ReturnRate / 10000;
            if (PokerConfig.B(Cfg, "honorGuarantee", true) && pool < i.Race.Guarantee) pool = i.Race.Guarantee;
            return pool;
        }

        /// <summary>Called by the existing 2528 handler (Unanswered B1). True = handled here.</summary>
        public static bool Handle(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Enabled) return false;
            try
            {
                if (!PokerModule.Ready(client)) return true;
                var r = PbR.FromPacket(stream);
                uint action = r.U32(1);
                var f2 = r.List(2);
                if (!PokerModule.Throttle(client, PokerConfig.Current.RequestCooldownMs)) return true;
                EnsureData(); EnsureTimer(); PokerModule.EnsureTicker();
                PokerLedger.ApplyPendingRefund(client);
                uint inst = f2.Count > 0 ? (uint)f2[0] : 0, race = f2.Count > 1 ? (uint)f2[1] : 0;
                switch (action)
                {
                    case 0: SendList(client); break;
                    case 5: Register(client, inst, race); break;
                    case 3: Enter(client, inst, race); break;
                    case 0x17: Detail(client, inst, race); break;
                }
            }
            catch (Exception e) { PokerLog.Write(client, "2528 error: " + e.Message); }
            return true;
        }

        static Instance Find(uint inst, uint race)
        {
            lock (gate) { Today(); Instance i; return instances.TryGetValue(inst, out i) && i.Race.Id == race ? i : null; }
        }

        static void Detail(Client.GameClient c, uint inst, uint race)
        {
            var i = Find(inst, race);
            if (i == null) return;
            Act(c, 0x17, new[] { inst, race, (uint)i.Players.Count });
        }

        static void Register(Client.GameClient c, uint inst, uint race)
        {
            uint uid = c.Player.UID;
            var i = Find(inst, race);
            Func<uint, uint[]> res = code => new[] { inst, race, code, i != null ? (uint)i.Hhmm : 0u, i != null ? (uint)i.Date : 0u };
            if (i == null) { Act(c, 6, new[] { inst, race, 2u, 0u, 0u }); return; }
            lock (i.Gate)
            {
                if (i.Started || i.Cancelled || i.Finished || Now() >= i.StartUtcLocal) { Act(c, 6, res(3)); return; }
                if (i.Players.ContainsKey(uid)) { Act(c, 6, res(7)); return; }
                if (PokerModule.IsSeated(uid)) { Act(c, 6, res(4)); return; }
                if (i.Players.Count >= i.Race.MaxPlayers) { Act(c, 6, res(1)); return; }
                if (PokerMoney.Balance(c) < i.Race.Cost) { Act(c, 6, res(8)); return; }
                ulong sid = PokerRng.NextId();
                PokerLedger.SetChips(sid, uid, i.Race.Id, i.Race.Cost, true);
                if (!PokerMoney.Debit(c, i.Race.Cost, "MTT entry " + i.Race.Name + " " + i.Hhmm)) { PokerLedger.SetChips(sid, uid, i.Race.Id, 0, false); Act(c, 6, res(8)); return; }
                PokerLedger.SetMarker(uid, sid);
                PokerMoney.Save(c);
                var e = new Entry { Uid = uid, Client = c, Session = sid, Inst = i };
                i.Players[uid] = e; entries[uid] = e;
                PokerLog.Write(c, "MTT REGISTER " + i.Race.Name + " " + i.Date + " " + i.Hhmm + " cost " + i.Race.Cost + " session " + sid + " (" + i.Players.Count + ")");
                Act(c, 6, res(0));
            }
        }

        static void Enter(Client.GameClient c, uint inst, uint race)
        {
            var i = Find(inst, race);
            Entry e;
            if (i == null || !i.Players.TryGetValue(c.Player.UID, out e) || e.Out || !i.Started) { Act(c, 6, new[] { inst, race, 11u, 0u, 0u }); return; }
            lock (i.Gate)
            {
                e.Client = c; e.Away = false;
                Act(c, 3, new[] { inst, race, 1u, (uint)i.Players.Values.Count(x => !x.Out) });
                if (e.Table != null) ShowBoard(i, e);
            }
        }

        // ------------------------------------------------------------------ tournament flow (caller holds inst.Gate)
        static void Start(Instance i, long ms)
        {
            i.Started = true;
            var list = i.Players.Values.ToList();
            if (list.Count < Math.Max(2, i.Race.MinPlayers))
            {
                i.Cancelled = true; i.Started = false;
                foreach (var e in list)
                {
                    entries.TryRemove(e.Uid, out _);
                    bool on = PokerLedger.Online(e.Client);
                    PokerLedger.Payout(e.Client, e.Uid, e.Session, i.Race.Id, i.Race.Cost, "MTT cancelled (" + list.Count + "/" + i.Race.MinPlayers + ") refund");
                    if (on) Act(e.Client, 7, new[] { i.Id, i.Race.Id, 0u, (uint)i.Hhmm }, null, "0_" + i.Race.Name);
                }
                PokerLog.Write("MTT CANCEL " + i.Race.Name + " " + i.Date + " " + i.Hhmm + ": " + list.Count + " < " + i.Race.MinPlayers + " players, all refunded");
                return;
            }
            i.Pool = PoolOf(i);
            i.Level = Math.Max(1, Math.Min(stages.Count - 1, i.Race.FirstStage));
            i.NextLevelAt = ms + 1000L * i.Race.LevelSeconds;
            // shuffle players onto tables of 9
            var order = list.OrderBy(x => PokerRng.NextU64()).ToList();
            int tables = (order.Count + 8) / 9;
            for (int t = 0; t < tables; t++) i.Tables.Add(NewTable(i));
            for (int k = 0; k < order.Count; k++)
            {
                var ctx = i.Tables[k % tables];
                lock (ctx.T.Gate)
                {
                    int seat = ctx.T.FirstFreeSeat();
                    ctx.T.Sit(seat, order[k].Uid, order[k].Client != null && order[k].Client.Player != null ? order[k].Client.Player.Name : "", i.Race.StartStack, ms);
                    ctx.T.Drain();
                    ctx.T.HoldUntil(ms + 10000);
                }
                order[k].Table = ctx;
                if (!PokerLedger.Online(order[k].Client)) { order[k].Away = true; lock (ctx.T.Gate) ctx.T.SetSitOut(order[k].Uid, true); }
            }
            PokerLog.Write("MTT START " + i.Race.Name + " " + i.Date + " " + i.Hhmm + " players " + order.Count + " tables " + tables + " pool " + i.Pool);
            foreach (var e in order) if (!e.Away) { Act(e.Client, 3, new[] { i.Id, i.Race.Id, 1u, (uint)order.Count }); ShowBoard(i, e); }
        }

        static PokerModule.TableCtx NewTable(Instance i)
        {
            var cfg = PokerConfig.Current;
            ulong bb = stages[Math.Max(1, Math.Min(stages.Count - 1, i.Race.FirstStage))];
            var def = new PokerTableDef
            {
                Id = (uint)System.Threading.Interlocked.Increment(ref PokerModule.nextTableId), LevelId = i.Race.Id, GameType = 0, Variant = PokerVariant.Texas,
                Seats = 9, BigBlind = bb, SmallBlind = bb / 2, MinBuyIn = 0, MaxBuyIn = ulong.MaxValue,
                ActionSeconds = cfg.ActionSeconds, NextHandSeconds = cfg.NextHandSeconds, Tournament = true
            };
            var ctx = new PokerModule.TableCtx { T = new PokerTable(def), Level = new PokerLevel { Id = i.Race.Id, BetUnit = bb, MaxBuyIn = ulong.MaxValue }, RaceType = 3, Tour = i, SurfaceF9 = 0xF };
            PokerModule.tables[def.Id] = ctx;
            return ctx;
        }

        static void ShowBoard(Instance i, Entry e)
        {
            var ctx = e.Table;
            if (ctx == null || !PokerLedger.Online(e.Client)) return;
            lock (ctx.T.Gate)
            {
                var me = ctx.T.SeatOf(e.Uid);
                if (me == null) return;
                ctx.T.SetSitOut(e.Uid, false);
                Act(e.Client, 8, new[] { i.Id, i.Race.Id, ctx.T.Def.Id, (uint)me.Index });
                PokerModule.Send(e.Client, PokerModule.EnterFor(ctx, me, true, PokerModule.StatusOf(ctx, me)), PokerModule.IdEnter);
                foreach (var o in ctx.T.Seats)
                    if (!o.Empty && o.Uid != e.Uid) PokerModule.Send(e.Client, PokerModule.EnterFor(ctx, o, false, PokerModule.StatusOf(ctx, o)), PokerModule.IdEnter);
                PokerModule.Send(e.Client, PokerModule.Chips(ctx.T.Seats), PokerModule.IdTableChip);
                PokerModule.Send(e.Client, TableInfo(i, ctx), IdMttTableInfo);
            }
        }

        static PbW TableInfo(Instance i, PokerModule.TableCtx ctx)
        {
            var w = new PbW().U(1, ctx.T.Def.BigBlind).U(2, 0).U(3, (ulong)i.Players.Values.Count(x => !x.Out));
            foreach (var s in ctx.T.Seats) if (!s.Empty) w.Msg(4, new PbW().U(1, PokerModule.ServerId).U(2, s.Uid).U(3, 0));
            return w;
        }

        static void Run(Instance i, long ms)
        {
            if (ms >= i.NextLevelAt && i.Level < stages.Count - 1)
            {
                i.Level++;
                i.NextLevelAt = ms + 1000L * i.Race.LevelSeconds;
                foreach (var ctx in i.Tables.ToList())
                    lock (ctx.T.Gate)
                    {
                        ctx.T.Def.BigBlind = stages[i.Level]; ctx.T.Def.SmallBlind = stages[i.Level] / 2;
                        PokerModule.Broadcast(ctx, TableInfo(i, ctx), IdMttTableInfo);
                    }
            }
            Eliminations(i, ms);
            if (!i.Finished) Balance(i, ms);
        }

        static void Eliminations(Instance i, long ms)
        {
            foreach (var ctx in i.Tables.ToList())
            {
                lock (ctx.T.Gate)
                {
                    if (ctx.T.Stage != PokerStage.Idle) continue;
                    var busted = i.Players.Values.Where(p => !p.Out && p.Table == ctx && ctx.T.SeatOf(p.Uid) != null && ctx.T.SeatOf(p.Uid).Stack == 0).ToList();
                    foreach (var p in busted) Eliminate(i, p, ms);
                    PokerModule.Flush(ctx, ms);
                }
                if (i.Finished) return;
            }
        }

        static void Eliminate(Instance i, Entry p, long ms)
        {
            int alive = i.Players.Values.Count(x => !x.Out);
            p.Rank = (uint)alive; p.Out = true;
            var ctx = p.Table;
            if (ctx != null)
            {
                ctx.T.Leave(p.Uid, ms);
                PokerModule.Broadcast(ctx, new PbW().U(1, 1).U(2, ctx.T.Def.Id).U(3, p.Uid).U(4, PokerModule.ServerId), PokerModule.IdExit, p.Uid);
            }
            Pay(i, p);
            var left = i.Players.Values.Where(x => !x.Out).ToList();
            if (left.Count == 1)
            {
                var w = left[0];
                w.Rank = 1; w.Out = true;
                if (w.Table != null) lock (w.Table.T.Gate) w.Table.T.Leave(w.Uid, ms);
                Pay(i, w);
                i.Finished = true;
                foreach (var t in i.Tables) PokerModule.tables.TryRemove(t.T.Def.Id, out _);
                PokerLog.Write("MTT END " + i.Race.Name + " " + i.Date + " " + i.Hhmm + " winner " + w.Uid);
            }
        }

        static ulong PrizeFor(Instance i, uint rank, out bool has)
        {
            has = false;
            // shares are "up to rank"; a row covering several ranks is split evenly between them (DERIVED split rule)
            uint prev = 0;
            foreach (var kv in i.Race.PrizeShares)
            {
                uint upTo = Math.Min(kv.Key, (uint)i.Players.Count);
                if (rank > prev && rank <= upTo)
                {
                    has = true;
                    uint n = upTo - prev;
                    return i.Pool * kv.Value / 10000 / Math.Max(1u, n);
                }
                prev = kv.Key;
                if (prev >= i.Players.Count) break;
            }
            return 0;
        }

        static void Pay(Instance i, Entry p)
        {
            bool has;
            ulong prize = PrizeFor(i, p.Rank, out has);
            entries.TryRemove(p.Uid, out _);
            bool on = PokerLedger.Online(p.Client);
            PokerLedger.Payout(p.Client, p.Uid, p.Session, i.Race.Id, prize, "MTT " + i.Race.Name + " rank " + p.Rank);
            if (on) Act(p.Client, 0xf, new[] { i.Id, p.Rank, prize > 0 ? 1u : 0u }, new[] { prize });
            PokerLog.Write(p.Client, "MTT FINISH " + i.Race.Name + " rank " + p.Rank + " prize " + prize);
        }

        /// <summary>Moves players from the smallest idle table when the field fits on fewer tables (DERIVED rule).</summary>
        static void Balance(Instance i, long ms)
        {
            i.Tables.RemoveAll(t => t.T.Occupied == 0);
            int alive = i.Players.Values.Count(x => !x.Out);
            int need = Math.Max(1, (alive + 8) / 9);
            if (i.Tables.Count <= need) return;
            var src = i.Tables.Where(t => t.T.Stage == PokerStage.Idle).OrderBy(t => t.T.Occupied).FirstOrDefault();
            if (src == null) return;
            lock (src.T.Gate)
            {
                foreach (var s in src.T.Seats.Where(x => !x.Empty).ToList())
                {
                    var dst = i.Tables.Where(t => t != src && t.T.Stage == PokerStage.Idle && t.T.FirstFreeSeat() >= 0).OrderByDescending(t => t.T.Occupied).FirstOrDefault();
                    if (dst == null) return;
                    Entry e; if (!i.Players.TryGetValue(s.Uid, out e)) continue;
                    ulong chips = src.T.Leave(s.Uid, ms);
                    src.T.Drain();
                    lock (dst.T.Gate)
                    {
                        dst.T.Sit(dst.T.FirstFreeSeat(), s.Uid, s.Name, chips, ms);
                        dst.T.Drain();
                    }
                    e.Table = dst;
                    if (PokerLedger.Online(e.Client) && !e.Away)
                    {
                        PokerModule.Send(e.Client, new PbW().U(1, 1).U(2, src.T.Def.Id).U(3, e.Uid).U(4, PokerModule.ServerId), PokerModule.IdExit);
                        ShowBoard(i, e);
                    }
                    var me = dst.T.SeatOf(e.Uid);
                    PokerModule.Broadcast(dst, PokerModule.EnterFor(dst, me, false, 3), PokerModule.IdEnter, e.Uid);
                }
            }
            if (src.T.Occupied == 0) { i.Tables.Remove(src); PokerModule.tables.TryRemove(src.T.Def.Id, out _); }
        }

        /// <summary>2472 f1=1 from an MTT board: the player stops watching; his seat auto-folds until he enters again.</summary>
        public static bool OnExit(Client.GameClient c)
        {
            Entry e;
            if (!entries.TryGetValue(c.Player.UID, out e) || e.Out) return false;
            lock (e.Inst.Gate)
            {
                e.Away = true;
                if (e.Table != null) lock (e.Table.T.Gate) e.Table.T.SetSitOut(e.Uid, true);
                PokerModule.Send(c, new PbW().U(1, 1).U(2, e.Table != null ? e.Table.T.Def.Id : 0).U(3, e.Uid).U(4, PokerModule.ServerId), PokerModule.IdExit);
            }
            return true;
        }

        public static void OnDisconnect(Client.GameClient c)
        {
            Entry e;
            if (c == null || c.Player == null || !entries.TryGetValue(c.Player.UID, out e)) return;
            lock (e.Inst.Gate)
            {
                e.Away = true;
                if (e.Table != null) lock (e.Table.T.Gate) e.Table.T.SetSitOut(e.Uid, true);
            }
        }
    }
}
