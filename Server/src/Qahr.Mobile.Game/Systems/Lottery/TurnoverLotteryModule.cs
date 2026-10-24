// [Lottery 2026-10-14] Turnover (flop / "cards") lottery (CDlgTurnoverLottery#837): packets 2102 + 2069.
//
// Client protocol (Conquer.exe 7957, Process 0x12060e2 / builder 0x11e7d44 / rank 0x11ff932):
//   S->C 2102 f1=0 {f2 LotteryType}           opens the dialog (PostCmd 4021) - the server opens it, the client has no trigger
//   C->S 2102 f1=1 {f2 type, f3 card}          flip a card (client already checked TurnoverPrice[flips done])
//   S->C 2102 f1=1 {f3 card, f4[0] = {cards_lottery_pool id, award index}}   flip result (PostCmd 4022)
//   S->C 2102 f1=2 {f4[] = {pool id, award index}}                           reveal the remaining awards
//   C->S 2102 f1=2 {f2 type} give up, f1=3 {f2 type} close
//   C->S 2069 f1=1 {f2 type, f4 page}  ->  S->C 2069 f1=1 {f2 type, f3 total pages, f4 page, f5[] = {f2 pool id, f3 name}}
//   S->C 2069 f1=0 {f5[] = {pool id, name}}  "%s was so lucky to flop %s!" broadcast
//
// Data (official, client files): Lottery\TurnoverLottery.ini (= client ini: MoneyType, ItemTypeID, CurrentAwards,
// ItemTypeID0..7 = cards_lottery_pool ids best first, LotteryLimitTimes, TurnoverPrice0..5) and cards_lottery_pool.txt
// (= client cards_lottery_pool.dat: id, name, itemtype, ...). The client has NO odds column, so the award weights
// (by award index) are derived: Lottery.ini [Turnover] Weights. Each card hides a different award (no repeats).
// MoneyType 1 CPs, 2 CPs (B) then CPs, 3 Silver, 7/8/9 the ItemTypeID item; 4/5/6 (horse / champion / chi points)
// do not exist on this server, so those lottery types stay disabled.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Lottery
{
    public static class TurnoverLotteryModule
    {
        public const ushort IdOpt = 2102, IdRank = 2069;

        public sealed class CardPrize { public uint Id, Item, BoundMode, Binding, LifeMinutes; public string Name; }
        public sealed class LType
        {
            public uint Type, MoneyType, CostItem, Limit;
            public List<uint> Awards = new List<uint>();
            public ulong[] Prices = new ulong[6];
            public string Disabled;
        }
        sealed class Session
        {
            public uint Type; public int Flips; public DateTime Opened;
            public readonly HashSet<uint> Cards = new HashSet<uint>();
            public readonly List<int> Left = new List<int>();// award indexes not won yet
        }
        public sealed class RankRow { public uint Pool; public string Name; }

        public static bool Enabled;
        public static long[] Weights = { 1, 3, 6, 10, 20, 20, 20, 20 };
        public static int TopIndex = 3, FlipIntervalMs = 500, SessionMinutes = 10, RankKeep = 50, RankPageSize = 10, MaxCards = 9;
        public static readonly Dictionary<uint, CardPrize> Prizes = new Dictionary<uint, CardPrize>();
        public static readonly Dictionary<uint, LType> Types = new Dictionary<uint, LType>();
        static readonly Dictionary<uint, Session> sessions = new Dictionary<uint, Session>();
        static readonly Dictionary<uint, List<RankRow>> ranks = new Dictionary<uint, List<RankRow>>();
        static bool loaded;
        public static string LoadError;

        static string RankFile { get { return Path.Combine(LotteryCommon.Dir, "turnover_rank.txt"); } }

        public static void EnsureLoaded()
        {
            if (loaded) return;
            lock (LotteryCommon.Sync)
            {
                if (loaded) return;
                loaded = true;
                try { Load(); LoadError = null; }
                catch (Exception e) { Enabled = false; LoadError = e.Message; LotteryCommon.Log("Turnover load failed (disabled): " + e.Message); }
            }
        }
        public static void Reload() { lock (LotteryCommon.Sync) { loaded = false; Prizes.Clear(); Types.Clear(); sessions.Clear(); ranks.Clear(); } EnsureLoaded(); }

        static void Load()
        {
            var cfg = LotteryCommon.ReadIni(Path.Combine(LotteryCommon.Dir, "Lottery.ini"));
            Enabled = LotteryCommon.GetLong(cfg, "Turnover", "Enabled", 1) != 0;
            TopIndex = (int)LotteryCommon.GetLong(cfg, "Turnover", "TopIndex", 3);
            FlipIntervalMs = (int)Math.Max(0, LotteryCommon.GetLong(cfg, "Turnover", "FlipIntervalMs", 500));
            SessionMinutes = (int)Math.Max(1, LotteryCommon.GetLong(cfg, "Turnover", "SessionMinutes", 10));
            RankKeep = (int)Math.Max(1, LotteryCommon.GetLong(cfg, "Turnover", "RankKeep", 50));
            RankPageSize = (int)Math.Max(1, Math.Min(30, LotteryCommon.GetLong(cfg, "Turnover", "RankPageSize", 10)));
            var w = LotteryCommon.Get(cfg, "Turnover", "Weights", null);
            if (!string.IsNullOrEmpty(w))
            {
                var parts = w.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
                var arr = new long[8];
                for (int i = 0; i < 8; i++) arr[i] = i < parts.Count ? Math.Max(0, long.Parse(parts[i])) : 1;
                Weights = arr;
            }

            string poolPath = LotteryCommon.DbRoot + "cards_lottery_pool.txt";
            if (!File.Exists(poolPath)) throw new Exception("cards_lottery_pool.txt missing");
            foreach (var line in File.ReadAllLines(poolPath))
            {
                var p = line.Split(new[] { "@@" }, StringSplitOptions.None);
                if (p.Length < 8) continue;
                uint id, item, bm, bind, life;
                if (!uint.TryParse(p[0].Trim(), out id) || !uint.TryParse(p[2].Trim(), out item)) continue;
                uint.TryParse(p[3].Trim(), out bm); uint.TryParse(p[5].Trim(), out bind); uint.TryParse(p[6].Trim(), out life);
                Prizes[id] = new CardPrize { Id = id, Name = p[1], Item = item, BoundMode = bm, Binding = bind, LifeMinutes = life };
            }

            var ti = LotteryCommon.ReadIni(Path.Combine(LotteryCommon.Dir, "TurnoverLottery.ini"));
            long amount = LotteryCommon.GetLong(ti, "LotteryTypeAmount", "Amount", 0);
            for (int i = 0; i < amount; i++)
            {
                string s = "TurnoverLottery" + i;
                var t = new LType
                {
                    Type = (uint)LotteryCommon.GetLong(ti, s, "LotteryType", 0), MoneyType = (uint)LotteryCommon.GetLong(ti, s, "MoneyType", 0),
                    CostItem = (uint)LotteryCommon.GetLong(ti, s, "ItemTypeID", 0), Limit = (uint)Math.Min(6, Math.Max(0, LotteryCommon.GetLong(ti, s, "LotteryLimitTimes", 0)))
                };
                long awards = Math.Min(8, LotteryCommon.GetLong(ti, s, "CurrentAwards", 0));
                for (int k = 0; k < awards; k++) t.Awards.Add((uint)LotteryCommon.GetLong(ti, s, "ItemTypeID" + k, 0));
                for (int k = 0; k < 6; k++) t.Prices[k] = (ulong)Math.Max(0, LotteryCommon.GetLong(ti, s, "TurnoverPrice" + k, 0));
                if (t.Type == 0) continue;
                if (t.Awards.Count == 0 || t.Limit == 0) t.Disabled = "no awards / flips";
                else if (t.Awards.Any(a => !Prizes.ContainsKey(a))) t.Disabled = "award id not in cards_lottery_pool";
                else if (t.MoneyType == 4 || t.MoneyType == 5 || t.MoneyType == 6) t.Disabled = "currency " + t.MoneyType + " does not exist on this server";
                else if (t.MoneyType >= 7 && t.MoneyType <= 9 && t.CostItem == 0) t.Disabled = "no cost item";
                else if (t.MoneyType < 1 || t.MoneyType > 9) t.Disabled = "unknown money type";
                Types[t.Type] = t;
            }
            var only = LotteryCommon.Get(cfg, "Turnover", "EnabledTypes", "");
            if (!string.IsNullOrWhiteSpace(only))
            {
                var keep = new HashSet<uint>(only.Split(',').Select(x => uint.Parse(x.Trim())));
                foreach (var t in Types.Values) if (t.Disabled == null && !keep.Contains(t.Type)) t.Disabled = "not in EnabledTypes";
            }
            LoadRanks();
        }

        static void LoadRanks()
        {
            if (!File.Exists(RankFile)) return;
            foreach (var l in File.ReadAllLines(RankFile, Encoding.UTF8))
            {
                var p = l.Split('\t');
                if (p.Length < 3) continue;
                uint t = uint.Parse(p[0]);
                List<RankRow> list;
                if (!ranks.TryGetValue(t, out list)) ranks[t] = list = new List<RankRow>();
                list.Add(new RankRow { Pool = uint.Parse(p[1]), Name = p[2] });
            }
        }
        static void SaveRanks()
        {
            var sb = new StringBuilder();
            foreach (var kv in ranks) foreach (var r in kv.Value) sb.Append(kv.Key).Append('\t').Append(r.Pool).Append('\t').Append((r.Name ?? "").Replace('\t', ' ')).Append('\n');
            Directory.CreateDirectory(LotteryCommon.Dir);
            string tmp = RankFile + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
            if (File.Exists(RankFile)) File.Replace(tmp, RankFile, RankFile + ".bak"); else File.Move(tmp, RankFile);
        }

        public enum FlipResult { Ok, NotReady, NoSession, WrongType, BadCard, CardUsed, NoFlipsLeft, NoMoney, RateLimited, BadRequest, Disabled }

        /// <summary>Opens the flop lottery of `type` for this player (server-side trigger: event / item / NPC script).</summary>
        public static bool Open(Client.GameClient c, uint type)
        {
            EnsureLoaded();
            LType t;
            if (!Enabled || !LotteryCommon.Ready(c) || !Types.TryGetValue(type, out t) || t.Disabled != null) return false;
            lock (LotteryCommon.Sync)
            {
                var s = new Session { Type = type, Opened = LotteryCommon.Now() };
                for (int i = 0; i < t.Awards.Count; i++) s.Left.Add(i);
                sessions[c.Player.UID] = s;
            }
            LotteryCommon.Send(c, new LtCardsOptPB { f1 = 0, f2 = type }, IdOpt);
            LotteryCommon.Log(c.Player.Name + "(" + c.Player.UID + ") turnover type " + type + " opened");
            return true;
        }

        public static FlipResult Handle2102(Client.GameClient c, LtCardsOptPB req)
        {
            EnsureLoaded();
            if (!LotteryCommon.Ready(c) || req == null) return FlipResult.NotReady;
            if (!Enabled) return FlipResult.Disabled;
            switch (req.f1)
            {
                case 1: return Flip(c, req.f2, req.f3);
                case 2:
                case 3:
                    lock (LotteryCommon.Sync)
                    {
                        Session s;
                        if (sessions.TryGetValue(c.Player.UID, out s) && s.Type == req.f2)
                        {
                            sessions.Remove(c.Player.UID);
                            LotteryCommon.Log(c.Player.Name + "(" + c.Player.UID + ") turnover type " + s.Type + (req.f1 == 2 ? " given up" : " closed") + " after " + s.Flips + " flip(s)");
                        }
                    }
                    return FlipResult.Ok;
                default: return FlipResult.BadRequest;
            }
        }

        static FlipResult Flip(Client.GameClient c, uint type, uint card)
        {
            if (!LotteryCommon.Allow(c, 3, FlipIntervalMs)) return FlipResult.RateLimited;
            lock (LotteryCommon.Sync)
            {
                Session s;
                if (!sessions.TryGetValue(c.Player.UID, out s)) return FlipResult.NoSession;
                var now = LotteryCommon.Now();
                if ((now - s.Opened).TotalMinutes > SessionMinutes) { sessions.Remove(c.Player.UID); return FlipResult.NoSession; }
                if (s.Type != type) return FlipResult.WrongType;
                LType t = Types[s.Type];
                if (card >= MaxCards) return FlipResult.BadCard;
                if (s.Cards.Contains(card)) return FlipResult.CardUsed;
                if (s.Flips >= t.Limit || s.Left.Count == 0) return FlipResult.NoFlipsLeft;

                // 1) pay the client price of this flip
                ulong price = t.Prices[Math.Min(5, s.Flips)];
                if (price > 0 && !Pay(c, t, price))
                {
                    LotteryCommon.Tell(c, "You don`t have enough " + CostName(t) + " to flop.");// STR_TURNOVER_LOTTERY_BALANCE_INSUFFICIENT_HINT
                    return FlipResult.NoMoney;
                }
                s.Flips++;
                s.Cards.Add(card);

                // 2) draw one of the awards still hidden (derived weights by award index)
                var w = s.Left.Select(i => i < Weights.Length ? Weights[i] : 1).ToList();
                int pick = LotteryCommon.PickWeighted(w);
                if (pick < 0) pick = 0;
                int award = s.Left[pick];
                s.Left.RemoveAt(pick);
                var prize = Prizes[t.Awards[award]];

                // 3) prize (ledger + pending, delivered now if possible)
                LotteryCommon.AddPrize(c, prize.Item, prize.BoundMode != 0 || prize.Binding != 0, prize.LifeMinutes, 1, "Turnover type " + t.Type + " award " + award + " pool " + prize.Id);
                if (LotteryCommon.PendingCount(c.Player.UID) > 0)
                    LotteryCommon.Tell(c, "Your inventory is full. Please make some room, and then login again to receive the prize.");// STR_CARDSLOTTERY_BACKPACK_FULL
                LotteryCommon.Log(c.Player.Name + "(" + c.Player.UID + ") TURNOVER type " + t.Type + " flip " + s.Flips + "/" + t.Limit + " card " + card + " paid " + price + " " + CostName(t) + " -> award " + award + " " + prize.Name + " (" + prize.Item + ")");

                var res = new LtCardsOptPB { f1 = 1, f2 = t.Type, f3 = card };
                res.f4.Add(new LtPairPB { f1 = prize.Id, f2 = (uint)award });
                LotteryCommon.Send(c, res, IdOpt);

                if (award <= TopIndex)
                {
                    List<RankRow> list;
                    if (!ranks.TryGetValue(t.Type, out list)) ranks[t.Type] = list = new List<RankRow>();
                    list.Insert(0, new RankRow { Pool = prize.Id, Name = c.Player.Name });
                    if (list.Count > RankKeep) list.RemoveRange(RankKeep, list.Count - RankKeep);
                    SaveRanks();
                    var bc = new LtCardsRankPB { f1 = 0, f2 = t.Type };
                    bc.f5.Add(new LtRankInfoPB { f2 = prize.Id, f3 = c.Player.Name });
                    foreach (var o in LotteryCommon.Online()) LotteryCommon.Send(o, bc, IdRank);
                }

                if (s.Flips >= t.Limit || s.Left.Count == 0)
                {
                    var rev = new LtCardsOptPB { f1 = 2, f2 = t.Type };
                    foreach (int i in s.Left) rev.f4.Add(new LtPairPB { f1 = t.Awards[i], f2 = (uint)i });
                    LotteryCommon.Send(c, rev, IdOpt);
                    sessions.Remove(c.Player.UID);
                }
                return FlipResult.Ok;
            }
        }

        static string CostName(LType t)
        {
            switch (t.MoneyType)
            {
                case 1: return "CPs";
                case 2: return "CPs (B)";
                case 3: return "Silver";
                default:
                    Database.ItemType.DBItem d;
                    return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(t.CostItem, out d) ? d.Name : "items";
            }
        }

        static bool Pay(Client.GameClient c, LType t, ulong price)
        {
            switch (t.MoneyType)
            {
                case 1: return LotteryCommon.TakeCps(c, price);
                case 2: return LotteryCommon.TakeBoundThenCps(c, price);
                case 3: return LotteryCommon.TakeSilver(c, price);
                case 7: case 8: case 9: return price <= 1000000 && LotteryCommon.TakeItem(c, t.CostItem, (uint)price, true, true);
                default: return false;
            }
        }

        // ------------------------------------------------------------------ 2069
        public static LtCardsRankPB Rank(LtCardsRankPB req)
        {
            EnsureLoaded();
            if (req == null || req.f1 != 1) return null;
            uint type = req.f2 > 100000 ? 0 : req.f2;
            lock (LotteryCommon.Sync)
            {
                List<RankRow> list;
                if (!ranks.TryGetValue(type, out list)) list = new List<RankRow>();
                uint pages = (uint)Math.Max(1, (list.Count + RankPageSize - 1) / RankPageSize);
                uint page = Math.Min(req.f4, pages - 1);
                var pb = new LtCardsRankPB { f1 = 1, f2 = type, f3 = pages, f4 = page };
                foreach (var r in list.Skip((int)page * RankPageSize).Take(RankPageSize)) pb.f5.Add(new LtRankInfoPB { f2 = r.Pool, f3 = r.Name });
                return pb;
            }
        }

        public static void Tick()
        {
            lock (LotteryCommon.Sync)
            {
                var now = LotteryCommon.Now();
                foreach (var k in sessions.Where(kv => (now - kv.Value.Opened).TotalMinutes > SessionMinutes).Select(kv => kv.Key).ToList()) sessions.Remove(k);
            }
        }

        // ------------------------------------------------------------------ packet entry points
        [PacketAttribute(IdOpt)]
        private static void On2102(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!LotteryCommon.Ready(client)) return;
                LtCardsOptPB req;
                try { req = stream.ProtoBufferDeserialize(new LtCardsOptPB()); } catch { return; }
                Handle2102(client, req);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        /// <summary>Called from the 2069 handler in MY System\Unanswered\B1\B1Rankings.cs.</summary>
        public static ServerSockets.Packet Reply2069(ServerSockets.Packet stream)
        {
            LtCardsRankPB req;
            try { req = stream.ProtoBufferDeserialize(new LtCardsRankPB()); } catch { return null; }
            var ans = Rank(req);
            if (ans == null) return null;
            stream.InitWriter();
            stream.ProtoBufferSerialize(ans);
            stream.Finalize(IdRank);
            return stream;
        }
    }
}
