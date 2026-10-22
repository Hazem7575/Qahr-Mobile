// [Lottery 2026-10-14] Global Lottery "Golden Tree" (CDlgGlobalLottery#851): packets 2005 + 2391.
//
// Client protocol (Conquer.exe 7957, Process 0x10c1fb5 / builder 0x10adf77 / rank 0xf56d48):
//   C->S 2005 Type=1 pick a chest (Time=1: also pay CPs to reset the pick cooldown), Type=3 remaining-prize list,
//        Type=5 my cooldown.   C->S 2391 Page (0-based), TotalPages=10 (= page size).
//   S->C 2005 Type 0 state {PrizesType = condition id, GUI = 0 none/1 pre/2 blooming/3 ended, Time = seconds to start
//        (pre) / to end (blooming), TotalCount[0] = {remaining, total}}
//        Type 1 pick result to every online player {PrizesType = winner uid, GUI = state, Time = pool row id,
//        Remain = winner name, TotalCount[0] = {remaining, total}} (the winner's client shows its own result,
//        the others update the bar and show the notice level of the pool row)
//        Type 3 / 4 {TotalCount[] = {pool row id, remaining}} list / end broadcast; Type 5 {Time = my cooldown}.
//   S->C 2391 {RegistedCount = winners, Pickers[] = {pool row id, name}} (10 per page).
//
// Data (official, client files): Lottery\globallotterycondition.ini = the client's own ini (Id, Type, CostType,
// CostValue, Monopoly, CD, CDCost); global_lottery_pool.txt = the client's global_lottery_pool.dat (col 1 type,
// 4 itemtype, 5 binding (3 = bound), 6/7 lifetime flag / minutes, 10 quantity per bloom). The draw is uniform over the
// prizes still in the tree (weight = remaining quantity), so the odds are exactly the client's quantities.
// Derived (Lottery.ini [GlobalLottery]): bloom times, condition per bloom, pre-phase and bloom length.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Lottery
{
    public static class GlobalLotteryModule
    {
        public const ushort IdLottery = 2005, IdRank = 2391;

        public sealed class Condition { public uint Id, Type, CostType, CostValue, Monopoly, CD, CDCost; }
        public sealed class PoolRow { public uint Id, Type, Rank, Item, Binding, LifeFlag, LifeMinutes, Notice, Quantity, Amount; public string Name; }
        public sealed class Bloom { public int StartMinute; public uint ConditionId; }
        public sealed class Winner { public uint Uid; public string Name; public uint Row; }

        // config
        public static bool Enabled;
        public static int PreMinutes = 5, BloomMinutes = 179, PickIntervalMs = 300, PageSize = 10;
        public static readonly List<Bloom> Blooms = new List<Bloom>();
        public static readonly Dictionary<uint, Condition> Conditions = new Dictionary<uint, Condition>();
        public static readonly List<PoolRow> Pool_ = new List<PoolRow>();
        public static string LoadError;

        // live state (persisted in golden_tree.txt)
        static string curKey = "";                  // yyyyMMddHHmm of the bloom the state belongs to
        static uint curCondition;
        static readonly Dictionary<uint, uint> remaining = new Dictionary<uint, uint>();
        static readonly List<Winner> winners = new List<Winner>();
        static readonly Dictionary<uint, DateTime> lastPick = new Dictionary<uint, DateTime>();
        static int lastState = -1; static string lastStateKey = "";
        static bool loaded;

        static string StateFile { get { return Path.Combine(LotteryCommon.Dir, "golden_tree.txt"); } }

        // ------------------------------------------------------------------ load
        public static void EnsureLoaded()
        {
            if (loaded) return;
            lock (LotteryCommon.Sync)
            {
                if (loaded) return;
                loaded = true;
                try { Load(); LoadError = null; }
                catch (Exception e) { Enabled = false; LoadError = e.Message; LotteryCommon.Log("Golden Tree load failed (disabled): " + e.Message); }
            }
        }
        public static void Reload() { lock (LotteryCommon.Sync) { loaded = false; Blooms.Clear(); Conditions.Clear(); Pool_.Clear(); remaining.Clear(); winners.Clear(); lastPick.Clear(); curKey = ""; curCondition = 0; lastState = -1; lastStateKey = ""; } EnsureLoaded(); }

        static void Load()
        {
            var cfg = LotteryCommon.ReadIni(Path.Combine(LotteryCommon.Dir, "Lottery.ini"));
            Enabled = LotteryCommon.GetLong(cfg, "GlobalLottery", "Enabled", 1) != 0;
            PreMinutes = (int)Math.Max(0, Math.Min(120, LotteryCommon.GetLong(cfg, "GlobalLottery", "PreMinutes", 5)));
            BloomMinutes = (int)Math.Max(1, Math.Min(1439, LotteryCommon.GetLong(cfg, "GlobalLottery", "BloomMinutes", 179)));
            PickIntervalMs = (int)Math.Max(0, LotteryCommon.GetLong(cfg, "GlobalLottery", "PickIntervalMs", 300));

            // client condition ini
            var ci = LotteryCommon.ReadIni(Path.Combine(LotteryCommon.Dir, "globallotterycondition.ini"));
            long amount = LotteryCommon.GetLong(ci, "ConditionAmount", "Amount", 0);
            for (int i = 0; i < amount; i++)
            {
                string s = i.ToString(CultureInfo.InvariantCulture);
                var c = new Condition
                {
                    Id = (uint)LotteryCommon.GetLong(ci, s, "id", 0), Type = (uint)LotteryCommon.GetLong(ci, s, "Type", 0),
                    CostType = (uint)LotteryCommon.GetLong(ci, s, "CostType", 0), CostValue = (uint)LotteryCommon.GetLong(ci, s, "CostValue", 0),
                    Monopoly = (uint)LotteryCommon.GetLong(ci, s, "Monopoly", 0), CD = (uint)LotteryCommon.GetLong(ci, s, "CD", 0),
                    CDCost = (uint)LotteryCommon.GetLong(ci, s, "CDCost", 0)
                };
                if (c.Id != 0) Conditions[c.Id] = c;
            }
            if (Conditions.Count == 0) throw new Exception("Lottery\\globallotterycondition.ini (client copy) has no conditions");

            // client pool
            string poolPath = LotteryCommon.DbRoot + "global_lottery_pool.txt";
            if (!File.Exists(poolPath)) throw new Exception("global_lottery_pool.txt missing");
            foreach (var line in File.ReadAllLines(poolPath))
            {
                var p = line.Split(new[] { "@@" }, StringSplitOptions.None);
                if (p.Length < 12) continue;
                uint[] n = new uint[12];
                bool ok = true;
                for (int k = 0; k < 12; k++) { if (k == 3) continue; if (!uint.TryParse(p[k].Trim(), out n[k])) { ok = false; break; } }
                if (!ok) continue;
                Pool_.Add(new PoolRow { Id = n[0], Type = n[1], Rank = n[2], Name = p[3], Item = n[4], Binding = n[5], LifeFlag = n[6], LifeMinutes = n[7], Notice = n[9], Quantity = n[10], Amount = Math.Max(1, n[11]) });
            }

            // schedule (derived)
            string blooms = LotteryCommon.Get(cfg, "GlobalLottery", "Blooms", "08:00@247,17:00@247,21:00@247");
            foreach (var part in blooms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var hp = part.Trim().Split('@');
                var hm = hp[0].Split(':');
                int h, m; uint cid;
                if (hp.Length != 2 || hm.Length != 2 || !int.TryParse(hm[0], out h) || !int.TryParse(hm[1], out m) || !uint.TryParse(hp[1], out cid)) throw new Exception("bad bloom '" + part + "'");
                if (h < 0 || h > 23 || m < 0 || m > 59) throw new Exception("bad bloom time '" + part + "'");
                Condition c;
                if (!Conditions.TryGetValue(cid, out c)) throw new Exception("bloom '" + part + "': condition id " + cid + " is not in the client ini");
                if (!Pool_.Any(r => r.Type == c.Type && r.Quantity > 0)) throw new Exception("bloom '" + part + "': no pool rows of type " + c.Type);
                if (c.CostType != 1 && c.CostType != 2 && c.CostType != 5) throw new Exception("bloom '" + part + "': cost type " + c.CostType + " is not supported");
                Blooms.Add(new Bloom { StartMinute = h * 60 + m, ConditionId = cid });
            }
            Blooms.Sort((a, b) => a.StartMinute.CompareTo(b.StartMinute));
            LoadState();
        }

        static void LoadState()
        {
            if (!File.Exists(StateFile)) return;
            foreach (var l in File.ReadAllLines(StateFile, Encoding.UTF8))
            {
                var p = l.Split('\t');
                if (p[0] == "K" && p.Length >= 3) { curKey = p[1]; curCondition = uint.Parse(p[2]); }
                else if (p[0] == "R" && p.Length >= 3) remaining[uint.Parse(p[1])] = uint.Parse(p[2]);
                else if (p[0] == "W" && p.Length >= 4) winners.Add(new Winner { Uid = uint.Parse(p[1]), Row = uint.Parse(p[2]), Name = p[3] });
            }
        }

        static void SaveState()
        {
            var sb = new StringBuilder();
            sb.Append("K\t").Append(curKey).Append('\t').Append(curCondition).Append('\n');
            foreach (var kv in remaining) sb.Append("R\t").Append(kv.Key).Append('\t').Append(kv.Value).Append('\n');
            foreach (var w in winners) sb.Append("W\t").Append(w.Uid).Append('\t').Append(w.Row).Append('\t').Append((w.Name ?? "").Replace('\t', ' ')).Append('\n');
            Directory.CreateDirectory(LotteryCommon.Dir);
            string tmp = StateFile + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            if (File.Exists(StateFile)) File.Replace(tmp, StateFile, StateFile + ".bak"); else File.Move(tmp, StateFile);
        }

        // ------------------------------------------------------------------ schedule
        public struct Phase { public int State; public DateTime Start, End; public uint ConditionId; public string Key; }

        /// <summary>State at `now`: 1 pre, 2 blooming, 3 ended (after a bloom), 0 nothing scheduled.</summary>
        public static Phase PhaseAt(DateTime now)
        {
            var ph = new Phase { State = 0 };
            if (Blooms.Count == 0) return ph;
            Phase? last = null;
            for (int d = -1; d <= 1; d++)
            {
                foreach (var b in Blooms)
                {
                    var start = now.Date.AddDays(d).AddMinutes(b.StartMinute);
                    var end = start.AddMinutes(BloomMinutes);
                    var key = start.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
                    if (now >= start.AddMinutes(-PreMinutes) && now < start) return new Phase { State = 1, Start = start, End = end, ConditionId = b.ConditionId, Key = key };
                    if (now >= start && now < end) return new Phase { State = 2, Start = start, End = end, ConditionId = b.ConditionId, Key = key };
                    if (now >= end && (last == null || end > last.Value.End)) last = new Phase { State = 3, Start = start, End = end, ConditionId = b.ConditionId, Key = key };
                }
            }
            return last ?? ph;
        }

        static List<PoolRow> RowsOf(uint conditionId)
        {
            Condition c;
            return Conditions.TryGetValue(conditionId, out c) ? Pool_.Where(r => r.Type == c.Type && r.Quantity > 0).ToList() : new List<PoolRow>();
        }

        /// <summary>Makes the live pool belong to the bloom of `ph` (fills it from the client quantities once per bloom).</summary>
        static void Sync(Phase ph)
        {
            if (ph.State != 2 || ph.Key == curKey) return;
            curKey = ph.Key; curCondition = ph.ConditionId;
            remaining.Clear(); winners.Clear(); lastPick.Clear();
            foreach (var r in RowsOf(curCondition)) remaining[r.Id] = r.Quantity;
            SaveState();
            LotteryCommon.Log("Golden Tree bloom " + curKey + " condition " + curCondition + ": " + remaining.Values.Sum(x => (long)x) + " prizes");
        }

        static uint Total() { return (uint)RowsOf(curCondition).Sum(r => (long)r.Quantity); }
        static uint Left() { return (uint)remaining.Values.Sum(x => (long)x); }

        /// <summary>The state the client must see: blooming with an empty tree = ended.</summary>
        static int VisibleState(Phase ph)
        {
            if (ph.State == 2 && ph.Key == curKey && Left() == 0) return 3;
            return ph.State;
        }

        static LtGlobalLotteryPB StatePacket(DateTime now)
        {
            var ph = PhaseAt(now);
            Sync(ph);
            int st = VisibleState(ph);
            var pb = new LtGlobalLotteryPB { Type = 0, GUI = (uint)st, PrizesType = ph.ConditionId };
            if (st == 1) pb.Time = (uint)Math.Max(0, (ph.Start - now).TotalSeconds);
            else if (st == 2) pb.Time = (uint)Math.Max(0, (ph.End - now).TotalSeconds);
            bool current = ph.Key == curKey;
            uint total = current ? Total() : (uint)RowsOf(ph.ConditionId).Sum(r => (long)r.Quantity);
            uint left = current ? Left() : (st == 1 ? total : 0);
            if (st != 0) pb.TotalCount.Add(new LtPairPB { f1 = left, f2 = total });
            return pb;
        }

        static uint CooldownLeft(uint uid, Condition c, DateTime now)
        {
            DateTime t;
            if (c == null || c.CD == 0 || !lastPick.TryGetValue(uid, out t)) return 0;
            double left = c.CD - (now - t).TotalSeconds;
            return left <= 0 ? 0 : (uint)Math.Ceiling(left);
        }

        public enum PickResult { Ok, NotReady, NotBlooming, Empty, Cooldown, NoCps, NoTicket, RateLimited, BadRequest }

        // ------------------------------------------------------------------ requests
        public static PickResult Handle2005(Client.GameClient c, LtGlobalLotteryPB req)
        {
            EnsureLoaded();
            if (!LotteryCommon.Ready(c) || req == null) return PickResult.NotReady;
            if (!Enabled) return PickResult.NotReady;
            var now = LotteryCommon.Now();
            switch (req.Type)
            {
                case 1:
                    if (!LotteryCommon.Allow(c, 0, PickIntervalMs)) return PickResult.RateLimited;
                    return Pick(c, req.Time == 1, now);
                case 3:
                    if (!LotteryCommon.Allow(c, 1, 300)) return PickResult.RateLimited;
                    lock (LotteryCommon.Sync)
                    {
                        Sync(PhaseAt(now));
                        LotteryCommon.Send(c, RemainingList(3), IdLottery);
                        LotteryCommon.Send(c, StatePacket(now), IdLottery);
                    }
                    return PickResult.Ok;
                case 5:
                    if (!LotteryCommon.Allow(c, 2, 300)) return PickResult.RateLimited;
                    lock (LotteryCommon.Sync)
                    {
                        var ph = PhaseAt(now); Sync(ph);
                        Condition cond; Conditions.TryGetValue(curCondition, out cond);
                        LotteryCommon.Send(c, new LtGlobalLotteryPB { Type = 5, Time = CooldownLeft(c.Player.UID, cond, now) }, IdLottery);
                    }
                    return PickResult.Ok;
                default:
                    return PickResult.BadRequest;
            }
        }

        static LtGlobalLotteryPB RemainingList(uint type)
        {
            var pb = new LtGlobalLotteryPB { Type = type };
            foreach (var r in RowsOf(curCondition))
            {
                uint left; remaining.TryGetValue(r.Id, out left);
                pb.TotalCount.Add(new LtPairPB { f1 = r.Id, f2 = left });
            }
            return pb;
        }

        static PickResult Pick(Client.GameClient c, bool payCd, DateTime now)
        {
            lock (LotteryCommon.Sync)
            {
                var ph = PhaseAt(now);
                Sync(ph);
                if (ph.State != 2 || ph.Key != curKey) { LotteryCommon.Tell(c, "The Golden Tree hasn`t bloomed. Please be patient."); return PickResult.NotBlooming; }// STR_GLOBAL_LOTTERY_NO_START
                if (Left() == 0) { LotteryCommon.Tell(c, "Bloom has ended."); return PickResult.Empty; }// STR_GLOBAL_LOTTERY_END
                Condition cond = Conditions[curCondition];

                // 1) check every cost first
                uint cdLeft = CooldownLeft(c.Player.UID, cond, now);
                ulong cdCost = 0;
                if (cdLeft > 0)
                {
                    if (!payCd) { LotteryCommon.Send(c, new LtGlobalLotteryPB { Type = 5, Time = cdLeft }, IdLottery); return PickResult.Cooldown; }
                    cdCost = (ulong)Math.Ceiling((double)cond.CDCost * cdLeft / Math.Max(1u, cond.CD));// client formula (fn 0x11a8f02)
                    if (c.Player.ConquerPoints < cdCost) { LotteryCommon.Tell(c, "Make sure you have enough CPs to reset picking cooldown."); return PickResult.NoCps; }
                }
                bool anyBinding = cond.Monopoly == 2;
                if (cond.CostType == 5)
                {
                    if (LotteryCommon.CountItem(c, cond.CostValue, anyBinding, true) < 1) { LotteryCommon.Tell(c, "Unable to pick chests due to insufficient " + ItemName(cond.CostValue) + "."); return PickResult.NoTicket; }
                }
                else if (cond.CostType == 1)
                {
                    if (c.Player.ConquerPoints < cdCost + cond.CostValue) { LotteryCommon.Tell(c, "Unable to pick chests due to insufficient CP."); return PickResult.NoTicket; }
                }
                else if (cond.CostType == 2)
                {
                    if (c.Player.Money < cond.CostValue) { LotteryCommon.Tell(c, "Unable to pick chests due to insufficient Silver."); return PickResult.NoTicket; }
                }
                else return PickResult.NoTicket;

                // 2) deduct
                if (cdCost > 0 && !LotteryCommon.TakeCps(c, cdCost)) return PickResult.NoCps;
                bool paid = cond.CostType == 5 ? LotteryCommon.TakeItem(c, cond.CostValue, 1, anyBinding, true)
                          : cond.CostType == 1 ? LotteryCommon.TakeCps(c, cond.CostValue)
                          : LotteryCommon.TakeSilver(c, cond.CostValue);
                if (!paid) { LotteryCommon.Log(c.Player.Name + " pick: payment failed after check (cd cost " + cdCost + " already taken)"); return PickResult.NoTicket; }

                // 3) draw: uniform over the prizes left in the tree
                var rows = RowsOf(curCondition);
                var weights = rows.Select(r => { uint l; remaining.TryGetValue(r.Id, out l); return (long)l; }).ToList();
                int idx = LotteryCommon.PickWeighted(weights);
                var row = rows[idx];
                remaining[row.Id] = remaining[row.Id] - 1;
                lastPick[c.Player.UID] = now;
                winners.Insert(0, new Winner { Uid = c.Player.UID, Name = c.Player.Name, Row = row.Id });
                if (winners.Count > 5000) winners.RemoveAt(winners.Count - 1);
                SaveState();

                // 4) prize (ledger + pending, delivered now if the bag has room)
                LotteryCommon.AddPrize(c, row.Item, row.Binding == 3, row.LifeFlag == 1 ? row.LifeMinutes : 0, (ushort)Math.Min(row.Amount, 1000), "GoldenTree " + curKey + " row " + row.Id);
                if (LotteryCommon.PendingCount(c.Player.UID) > 0)
                    LotteryCommon.Tell(c, "Your inventory is full. Please make some room, and login again to receive a reward.");// STR_GLOBALLOTTERY_BACKPACK_FULL
                LotteryCommon.Log(c.Player.Name + "(" + c.Player.UID + ") GOLDEN TREE pick row " + row.Id + " " + row.Name + " (" + row.Item + ") cd cost " + cdCost + " left " + Left() + "/" + Total());

                // 5) result to everyone online (the winner's client shows its own result)
                uint left = Left(), total = Total();
                int st = left == 0 ? 3 : 2;
                var res = new LtGlobalLotteryPB { Type = 1, PrizesType = c.Player.UID, GUI = (uint)st, Time = row.Id, Remain = c.Player.Name };
                res.TotalCount.Add(new LtPairPB { f1 = left, f2 = total });
                var sent = new HashSet<uint>();
                foreach (var o in LotteryCommon.Online()) if (sent.Add(o.Player.UID)) LotteryCommon.Send(o, res, IdLottery);
                if (sent.Add(c.Player.UID)) LotteryCommon.Send(c, res, IdLottery);
                if (left == 0) EndBroadcast(now);
                return PickResult.Ok;
            }
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem d;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out d) ? d.Name : "items";
        }

        static void EndBroadcast(DateTime now)
        {
            var end = RemainingList(4);
            var st = StatePacket(now);
            foreach (var o in LotteryCommon.Online()) { LotteryCommon.Send(o, end, IdLottery); LotteryCommon.Send(o, st, IdLottery); }
            lastState = 3; lastStateKey = curKey;
            LotteryCommon.Log("Golden Tree bloom " + curKey + " ended, " + Left() + " prize(s) left");
        }

        // ------------------------------------------------------------------ 2391 winners list
        public static LtGlobalLotteryRankPB Rank(LtGlobalLotteryRankPB req)
        {
            EnsureLoaded();
            uint page = req == null ? 0 : Math.Min(req.Page, 100000u);
            lock (LotteryCommon.Sync)
            {
                Sync(PhaseAt(LotteryCommon.Now()));
                var pb = new LtGlobalLotteryRankPB { Page = page, TotalPages = (uint)PageSize, RegistedCount = (uint)winners.Count };
                foreach (var w in winners.Skip((int)Math.Min(int.MaxValue, (long)page * PageSize)).Take(PageSize))
                    pb.Pickers.Add(new LtPickerPB { PickItem = w.Row, Name = w.Name });
                return pb;
            }
        }

        // ------------------------------------------------------------------ timer: state pushes, login pushes, deliveries
        public static void Tick()
        {
            EnsureLoaded();
            if (!Enabled) return;
            var now = LotteryCommon.Now();
            lock (LotteryCommon.Sync)
            {
                var ph = PhaseAt(now);
                Sync(ph);
                int st = VisibleState(ph);
                if (st != lastState || ph.Key != lastStateKey)
                {
                    bool ended = st == 3 && lastState == 2 && ph.Key == lastStateKey;
                    lastState = st; lastStateKey = ph.Key;
                    if (ended) EndBroadcast(now);
                    else { var pb = StatePacket(now); foreach (var o in LotteryCommon.Online()) LotteryCommon.Send(o, pb, IdLottery); }
                    if (st == 1) foreach (var o in LotteryCommon.Online()) LotteryCommon.Tell(o, "Golden Tree is about to bloom! Get ready!");// STATUS_TIPS_PRE
                }
            }
        }

        public static void LoginPush(Client.GameClient c)
        {
            EnsureLoaded();
            if (!Enabled || !LotteryCommon.Ready(c)) return;
            lock (LotteryCommon.Sync) LotteryCommon.Send(c, StatePacket(LotteryCommon.Now()), IdLottery);
        }

        // ------------------------------------------------------------------ packet entry points
        [PacketAttribute(IdLottery)]
        private static void On2005(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!LotteryCommon.Ready(client)) return;
                LtGlobalLotteryPB req;
                try { req = stream.ProtoBufferDeserialize(new LtGlobalLotteryPB()); } catch { return; }
                Handle2005(client, req);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>Called from the 2391 handler in MY System\Unanswered\B1\B1Rankings.cs.</summary>
        public static ServerSockets.Packet Reply2391(ServerSockets.Packet stream)
        {
            LtGlobalLotteryRankPB req;
            try { req = stream.ProtoBufferDeserialize(new LtGlobalLotteryRankPB()); } catch { return null; }
            var ans = Rank(req);
            stream.InitWriter();
            stream.ProtoBufferSerialize(ans);
            stream.Finalize(IdRank);
            return stream;
        }
    }
}
