// [Poker 2026-10-14] New Texas Hold'em cash tables: lobby (9025), seating (2471/2472), betting (2469/2468),
// dealing (2470), pot/chips (2477/2466), showdown/result (2474/2473), table options (2463), auto-play (2526).
// Protocol: scratchpad poker\spec_lobby.txt + spec_hand.txt (RE of Conquer.exe 7957). serverId is always 0 on this
// server (the client's hero server id is 0 because we never send 2091).
//
// Money: silver -> chips only through PokerLedger (journal first, then Money/marker, then SaveClient); chips -> silver
// the same way back. The table engine audits every chip after every change.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Qahr.Mobile.Poker
{
    public static partial class PokerModule
    {
        public const ushort IdNewTexasAct = 2463, IdTableChip = 2466, IdGameRecord = 2467, IdActivePlayer = 2468, IdCallAction = 2469,
            IdDealtCard = 2470, IdEnter = 2471, IdExit = 2472, IdGameResult = 2473, IdLayCard = 2474, IdOnlineStatus = 2476,
            IdData = 2477, IdFinishShow = 2478, IdTrusteeship = 2526, IdGameListAct = 9025;
        public const uint ServerId = 0;

        // ------------------------------------------------------------------ state
        internal sealed class TableCtx
        {
            public PokerTable T;
            public PokerLevel Level;
            public uint RaceType;
            public readonly ConcurrentDictionary<uint, Client.GameClient> Watchers = new ConcurrentDictionary<uint, Client.GameClient>();
            public readonly Dictionary<int, uint> AnnouncedStatus = new Dictionary<int, uint>();
            public readonly Dictionary<uint, int> AddTimeUsed = new Dictionary<uint, int>();
            public ulong AddTimeHand;
            /// <summary>Tournament driver (Sit&amp;Go / MTT); null on cash tables.</summary>
            public ITourney Tour;
            public uint SurfaceF9;          // 2471 f9: 0 cash, 0x10 SNG, 0xF MTT
        }

        /// <summary>Tournament tables plug into the same event pipeline.</summary>
        internal interface ITourney
        {
            Client.GameClient ClientOf(uint uid);
            void AfterResult(TableCtx ctx, PokerEvent e, long now);
            void Housekeeping(TableCtx ctx, long now);
        }

        internal sealed class Session
        {
            public uint Uid;
            public ulong Id;
            public TableCtx Ctx;
            public Client.GameClient Client;
            public bool AutoRebuy, AutoTopOff;
            public ulong BuyAmount;
            public ulong PendingTopUp;     // silver already moved into the journal? no: requested, applied at hand end
            public long BustedAt;
            public int SitOutHands;
        }

        internal static readonly ConcurrentDictionary<uint, Session> sessions = new ConcurrentDictionary<uint, Session>();
        internal static readonly ConcurrentDictionary<uint, TableCtx> tables = new ConcurrentDictionary<uint, TableCtx>();
        static readonly ConcurrentDictionary<uint, long> lastReq = new ConcurrentDictionary<uint, long>();
        static readonly object joinGate = new object();
        internal static int nextTableId = 100;
        static Timer ticker;
        static readonly object tickGate = new object();
        public static Func<long> Clock = () => PokerClock.Now();

        internal static TableCtx TableOf(uint uid)
        {
            Session s;
            if (sessions.TryGetValue(uid, out s)) return s.Ctx;
            return PokerSng.TableOf(uid) ?? PokerMtt.TableOf(uid);
        }

        public static bool IsSeated(uint uid) { return sessions.ContainsKey(uid) || PokerSng.IsIn(uid) || PokerMtt.IsIn(uid); }

        public static Action<Client.GameClient, string> NotifyHook;
        public static bool TestMode;
        public static readonly HashSet<uint> TestOffline = new HashSet<uint>();
        public static void Notify(Client.GameClient client, string text)
        {
            if (NotifyHook != null) { NotifyHook(client, text); return; }
            try { if (client != null) client.SendSysMesage(text); } catch { }
        }

        internal static void EnsureTicker()
        {
            if (ticker != null) return;
            lock (tickGate)
            {
                if (ticker == null) ticker = new Timer(_ => TickAll(), null, 250, 250);
            }
        }

        /// <summary>Test hook: run one tick synchronously.</summary>
        public static void TickAll()
        {
            if (!Monitor.TryEnter(tickGate)) return;
            try
            {
                long now = Clock();
                foreach (var ctx in tables.Values.ToList())
                {
                    try
                    {
                        lock (ctx.T.Gate)
                        {
                            ctx.T.Tick(now);
                            Flush(ctx, now);
                            Housekeeping(ctx, now);
                        }
                    }
                    catch (Exception e) { PokerLog.Write("tick error table " + ctx.T.Def.Id + ": " + e); }
                }
            }
            finally { Monitor.Exit(tickGate); }
        }

        internal static bool Throttle(Client.GameClient c, int ms)
        {
            long now = Clock();
            uint uid = c.Player.UID;
            long last;
            if (lastReq.TryGetValue(uid, out last) && now - last < ms) return false;
            lastReq[uid] = now;
            return true;
        }

        internal static bool Ready(Client.GameClient c) { return c != null && c.Player != null && c.FullLoading; }

        // ------------------------------------------------------------------ sending
        internal static void Send(Client.GameClient c, PbW w, ushort id)
        {
            try { if (!TestMode && c != null && c.Player != null) { var p = w.Packet(id); if (p.Length <= 1032) c.Send(p); else PokerLog.Write(c, "packet " + id + " too big " + p.Length); } }
            catch (Exception e) { PokerLog.Write("send error: " + e.Message); }
            if (SentHook != null) SentHook(c, id, w.ToArray());
        }
        /// <summary>Test hook: every packet the module sends (client, id, protobuf body).</summary>
        public static Action<Client.GameClient, ushort, byte[]> SentHook;

        static IEnumerable<Client.GameClient> Audience(TableCtx ctx)
        {
            foreach (var s in ctx.T.Seats)
            {
                if (s.Empty) continue;
                if (ctx.Tour != null) { var tc = ctx.Tour.ClientOf(s.Uid); if (tc != null) yield return tc; continue; }
                Session ss;
                if (sessions.TryGetValue(s.Uid, out ss) && ss.Client != null) yield return ss.Client;
            }
            foreach (var w in ctx.Watchers.Values) yield return w;
        }

        internal static void Broadcast(TableCtx ctx, PbW w, ushort id, uint exceptUid = 0)
        {
            foreach (var c in Audience(ctx).ToList())
                if (c.Player != null && c.Player.UID != exceptUid) Send(c, w, id);
        }

        static PbW Card(int c) { return new PbW().U(1, (ulong)(PokerCard.Rank(c) - 2)).U(2, (ulong)PokerCard.Suit(c)); }

        // ---- builders
        static PbW GameList(uint action, uint type, uint level = 0, bool autoRebuy = false, bool autoTop = false, ulong buy = 0, uint err = 0, uint tableId = 0)
        {
            var w = new PbW().U(1, action).U(2, type);
            if (action == 1) { w.U(3, level).B(4, autoRebuy).B(5, autoTop).U(6, buy).B(7, false); }
            if (tableId != 0) w.U(8, tableId);
            if (action == 4 || action == 5) w.U(11, err);
            return w;
        }

        internal static PbW EnterFor(TableCtx ctx, PokerSeat s, bool hero, uint status)
        {
            // f1 > 0 makes the client draw the seat only when it already has an info record for (uid, server);
            // non-hero seats therefore use f1 = 0 (seat drawn with the client's own name placeholder) unless the
            // config says otherwise (cash.otherSeatOnlineFlag).
            ulong online = hero ? 1UL : (ulong)PokerConfig.D(PokerConfig.Obj(PokerConfig.Current.Raw, "cash"), "otherSeatOnlineFlag", 0);
            var w = new PbW().U(1, online).U(2, status).U(3, (ulong)s.Index).U(4, ctx.T.Def.Id).U(5, ServerId).U(6, s.Uid);
            if (hero)
            {
                w.U(7, s.Stack).U(8, ctx.T.Def.Id).U(9, ctx.SurfaceF9).U(12, ctx.RaceType).U(13, ctx.T.Def.BigBlind).U(14, 0).U(15, (ulong)ctx.T.Seats.Length).U(16, 0);
            }
            return w;
        }

        internal static PbW Chips(IEnumerable<PokerSeat> seats)
        {
            var w = new PbW();
            foreach (var s in seats) if (!s.Empty) w.Msg(1, new PbW().U(1, ServerId).U(2, s.Uid).U(3, 0).U(4, s.Stack));
            return w;
        }

        static PbW ChipsOne(uint uid, ulong stack) { return new PbW().Msg(1, new PbW().U(1, ServerId).U(2, uid).U(3, 0).U(4, stack)); }

        static PbW Pot(ulong pot) { return new PbW().U(1, 1).U(3, pot); }

        internal static uint StatusOf(TableCtx ctx, PokerSeat s)
        {
            if (s.InHand && ctx.T.Stage != PokerStage.Idle) return 1;
            return 3;
        }

        // ------------------------------------------------------------------ packet handlers
        [PacketAttribute(IdGameListAct)]
        static void OnGameListAct(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                uint action = r.U32(1);
                if (action == 0 || action == 2) return;                    // hall tab (list is client-side) / auto echo of our a1
                if (!Throttle(client, PokerConfig.Current.RequestCooldownMs)) return;
                EnsureTicker();
                PokerLedger.ApplyPendingRefund(client);
                uint type = (uint)(int)r.U(2);
                switch (action)
                {
                    case 1: BuyIn(client, type, r.U32(3), r.Bool(4), r.Bool(5), r.U(6)); break;
                    case 3: LeaveTable(client, true); break;
                    case 4: ChangeTable(client); break;
                    case 5: QuickPlay(client, type); break;
                    case 6: ChangeTable(client); break;
                }
            }
            catch (Exception e) { PokerLog.Write(client, "9025 error: " + e.Message); }
        }

        [PacketAttribute(IdCallAction)]
        static void OnCallAction(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                uint act = r.U32(2);
                ulong add = r.U(3);
                uint uid = client.Player.UID;
                var ctx = TableOf(uid);
                if (ctx == null) return;
                Session s; sessions.TryGetValue(uid, out s);
                lock (ctx.T.Gate)
                {
                    long now = Clock();
                    var seat = ctx.T.SeatOf(uid);
                    if (seat == null || ctx.T.ToAct != seat.Index) return;
                    PokerAct a;
                    ulong to = 0;
                    switch (act)
                    {
                        case 0x1: case 0x10:
                            {
                                ulong basis = seat.Bet + ctx.T.ToCall(seat);
                                if (add == 0 || add > ulong.MaxValue / 2 || basis + add < basis) return;
                                to = basis + add; a = ctx.T.CurrentBet == 0 ? PokerAct.Bet : PokerAct.Raise; break;
                            }
                        case 0x2: a = PokerAct.Call; break;
                        case 0x4: a = PokerAct.Fold; break;
                        case 0x8: a = PokerAct.Check; break;
                        case 0x20: a = PokerAct.AllIn; break;
                        default: return;
                    }
                    seat.SitOut = false; seat.Timeouts = 0;
                    if (!ctx.T.Act(uid, a, to, now)) { PokerLog.Write(client, "refused action " + act + " add " + add); ResendTurn(ctx, client); return; }
                    if (s != null) s.SitOutHands = 0;
                    Flush(ctx, now);
                }
            }
            catch (Exception e) { PokerLog.Write(client, "2469 error: " + e.Message); }
        }

        [PacketAttribute(IdEnter)]
        static void OnEnter(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                if (r.U32(2) != 2) return;                                  // only "stand up / spectate"
                if (!Throttle(client, PokerConfig.Current.RequestCooldownMs)) return;
                StandUp(client);
            }
            catch (Exception e) { PokerLog.Write(client, "2471 error: " + e.Message); }
        }

        [PacketAttribute(IdNewTexasAct)]
        static void OnNewTexasAct(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                uint action = r.U32(1);
                if (!Throttle(client, PokerConfig.Current.RequestCooldownMs)) return;
                var f2 = r.List(2); var f3 = r.List(3);
                switch (action)
                {
                    case 7: SetOptions(client, f2.Count > 0 && f2[0] != 0, f2.Count > 1 && f2[1] != 0); break;
                    case 8: AddTime(client); break;
                    case 16: TopUp(client, f3.Count > 0 ? f3[0] : 0, f2.Count > 0 && f2[0] != 0, f2.Count > 1 && f2[1] != 0); break;
                }
            }
            catch (Exception e) { PokerLog.Write(client, "2463 error: " + e.Message); }
        }

        [PacketAttribute(IdExit)]
        static void OnExit(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                if (r.U32(1) != 1) return;
                PokerMtt.OnExit(client);                                    // cash tables leave with 9025 f1=3
            }
            catch (Exception e) { PokerLog.Write(client, "2472 error: " + e.Message); }
        }

        [PacketAttribute(IdTrusteeship)]
        static void OnTrusteeship(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                var r = PbR.FromPacket(stream);
                bool on = r.Bool(3);
                var tctx = TableOf(client.Player.UID);
                if (tctx == null) return;
                Session s; sessions.TryGetValue(client.Player.UID, out s);
                lock (tctx.T.Gate) { tctx.T.SetSitOut(client.Player.UID, on); if (!on && s != null) s.SitOutHands = 0; }
                Send(client, new PbW().U(1, ServerId).U(2, client.Player.UID).B(3, on), IdTrusteeship);
            }
            catch (Exception e) { PokerLog.Write(client, "2526 error: " + e.Message); }
        }

        // ------------------------------------------------------------------ lobby actions
        static uint RaceTypeFor(uint type) { return type == 2 ? 5u : type == 1 ? 4u : 3u; }

        static bool TypeEnabled(uint type)
        {
            var cash = PokerConfig.Obj(PokerConfig.Current.Raw, "cash");
            var arr = PokerConfig.Arr(cash, "enabledTypes");
            if (arr == null) return type == 0 || type == 2;
            foreach (var o in arr) if ((uint)PokerConfig.Num(o) == type) return true;
            return false;
        }

        static void BuyIn(Client.GameClient client, uint type, uint levelId, bool autoRebuy, bool autoTop, ulong buy)
        {
            var cfg = PokerConfig.Current;
            PokerLevel lv;
            if (!cfg.Enabled || !cfg.Levels.TryGetValue(levelId, out lv) || !lv.Enabled || lv.Type != type || !TypeEnabled(type))
            { Send(client, GameList(5, type, err: 5), IdGameListAct); PokerLog.Write(client, "buy-in refused: level " + levelId + " type " + type); return; }
            if (buy < lv.MinBuyIn || buy > lv.MaxBuyIn) { Send(client, GameList(5, type, err: 1), IdGameListAct); PokerLog.Write(client, "buy-in refused: amount " + buy); return; }
            if (sessions.ContainsKey(client.Player.UID)) { Send(client, GameList(5, type, err: 4), IdGameListAct); PokerLog.Write(client, "buy-in refused: already seated"); return; }
            if (!TestMode && client.InTrade) { Notify(client, "You cannot join a poker table while trading."); return; }
            if (PokerMoney.Balance(client) < buy) { Notify(client, "Buy-in failed: insufficient silver."); return; }
            Join(client, lv, buy, autoRebuy, autoTop, null);
        }

        static TableCtx FindTable(PokerLevel lv, TableCtx avoid)
        {
            var cfg = PokerConfig.Current;
            TableCtx best = null;
            int count = 0;
            foreach (var t in tables.Values)
            {
                if (t.Level.Id != lv.Id || t == avoid || t.T.Frozen) continue;
                count++;
                int occ = t.T.Occupied;
                if (occ >= t.T.Seats.Length) continue;
                if (best == null || occ > best.T.Occupied) best = t;
            }
            if (best != null) return best;
            if (count >= cfg.MaxTablesPerLevel) return null;
            var def = new PokerTableDef
            {
                Id = (uint)Interlocked.Increment(ref nextTableId),
                LevelId = lv.Id,
                GameType = lv.Type,
                Variant = lv.Type == 2 ? PokerVariant.Omaha : PokerVariant.Texas,
                Seats = cfg.Seats,
                SmallBlind = lv.BetUnit / 2,
                BigBlind = lv.BetUnit,
                MinBuyIn = lv.MinBuyIn,
                MaxBuyIn = lv.MaxBuyIn,
                ActionSeconds = cfg.ActionSeconds,
                NextHandSeconds = cfg.NextHandSeconds,
                RakePercent = cfg.RakePercent,
                RakeCap = cfg.RakeCapBigBlinds * lv.BetUnit,
                NoFlopNoDrop = cfg.NoFlopNoDrop
            };
            var ctx = new TableCtx { T = new PokerTable(def), Level = lv, RaceType = RaceTypeFor(lv.Type) };
            tables[def.Id] = ctx;
            PokerLog.Write("table " + def.Id + " opened: level " + lv.Id + " blinds " + def.SmallBlind + "/" + def.BigBlind);
            return ctx;
        }

        /// <summary>Moves silver into a new seat. Order: journal -> Money/marker -> SaveClient -> seat.</summary>
        static bool Join(Client.GameClient client, PokerLevel lv, ulong buy, bool autoRebuy, bool autoTop, TableCtx avoid)
        {
            uint uid = client.Player.UID;
            lock (joinGate)
            {
                if (sessions.ContainsKey(uid)) return false;
                var ctx = FindTable(lv, avoid);
                if (ctx == null) { Send(client, GameList(5, lv.Type, err: 5), IdGameListAct); return false; }
                lock (ctx.T.Gate)
                {
                    int seat = ctx.T.FirstFreeSeat();
                    if (seat < 0) { Send(client, GameList(5, lv.Type, err: 5), IdGameListAct); return false; }
                    if (PokerMoney.Balance(client) < buy) { Notify(client, "Buy-in failed: insufficient silver."); return false; }
                    ulong sid = PokerRng.NextId();
                    PokerLedger.SetChips(sid, uid, ctx.T.Def.Id, buy, true);              // 1) journal
                    if (!PokerMoney.Debit(client, buy, "buy-in table " + ctx.T.Def.Id))   // 2) silver
                    {
                        PokerLedger.SetChips(sid, uid, ctx.T.Def.Id, 0, false);
                        Notify(client, "Buy-in failed: insufficient silver.");
                        return false;
                    }
                    PokerLedger.SetMarker(uid, sid);
                    PokerMoney.Save(client);                                               // 3) persist Money + marker together
                    var s = new Session { Uid = uid, Id = sid, Ctx = ctx, Client = client, AutoRebuy = autoRebuy, AutoTopOff = autoTop, BuyAmount = buy };
                    sessions[uid] = s;
                    ctx.Watchers.TryRemove(uid, out _);
                    long now = Clock();
                    var ps = ctx.T.Sit(seat, uid, client.Player.Name, buy, now);
                    PokerLog.Write(client, "SIT table " + ctx.T.Def.Id + " seat " + seat + " chips " + buy + " session " + sid);
                    ctx.T.Drain();                                                         // the Seat event is announced below
                    // a) join ack, b) hero enter (opens the board), c) the other seats, d) chips
                    Send(client, GameList(1, lv.Type, lv.Id, autoRebuy, autoTop, buy), IdGameListAct);
                    uint heroStatus = StatusOf(ctx, ps);
                    ctx.AnnouncedStatus[ps.Index] = heroStatus;
                    Send(client, EnterFor(ctx, ps, true, heroStatus), IdEnter);
                    foreach (var o in ctx.T.Seats)
                        if (!o.Empty && o.Uid != uid) Send(client, EnterFor(ctx, o, false, StatusOf(ctx, o)), IdEnter);
                    Broadcast(ctx, EnterFor(ctx, ps, false, heroStatus), IdEnter, uid);
                    Send(client, Chips(ctx.T.Seats), IdTableChip);
                    Broadcast(ctx, ChipsOne(uid, ps.Stack), IdTableChip, uid);
                    Send(client, new PbW().U(1, 7).Rep(2, new uint[] { autoTop ? 1u : 0u, autoRebuy ? 1u : 0u }), IdNewTexasAct);
                    if (ctx.T.Stage != PokerStage.Idle) Send(client, Pot(ctx.T.PotTotal), IdData);
                    Flush(ctx, now);
                    return true;
                }
            }
        }

        /// <summary>Cash out the seat (or stop watching). sendAck: reply 9025 f1=3 so the client closes the board.</summary>
        public static void LeaveTable(Client.GameClient client, bool sendAck)
        {
            uint uid = client.Player.UID;
            Session s;
            if (sessions.TryGetValue(uid, out s))
            {
                var ctx = s.Ctx;
                uint type = ctx.Level.Type;
                lock (ctx.T.Gate) { CashOut(ctx, s, Clock(), "leave"); }
                if (sendAck) Send(client, GameList(3, type), IdGameListAct);
                return;
            }
            foreach (var t in tables.Values)
                if (t.Watchers.TryRemove(uid, out _)) { if (sendAck) Send(client, GameList(3, t.Level.Type), IdGameListAct); return; }
            if (sendAck) Send(client, GameList(3, 0), IdGameListAct);
        }

        /// <summary>Caller holds ctx.T.Gate. Removes the seat and pays the stack back as silver.</summary>
        static ulong CashOut(TableCtx ctx, Session s, long now, string why)
        {
            ulong cash = ctx.T.Leave(s.Uid, now);
            sessions.TryRemove(s.Uid, out _);
            PokerLedger.SetChips(s.Id, s.Uid, ctx.T.Def.Id, cash, false);           // journal: closed with the paid amount
            if (s.Client != null && s.Client.Player != null)
            {
                if (cash > 0) PokerMoney.Credit(s.Client, cash, why + " table " + ctx.T.Def.Id);
                PokerLedger.SetMarker(s.Uid, 0);
                PokerMoney.Save(s.Client);
            }
            PokerLog.Write(s.Client, "LEAVE (" + why + ") table " + ctx.T.Def.Id + " cash " + cash + " session " + s.Id);
            Broadcast(ctx, new PbW().U(1, 1).U(2, ctx.T.Def.Id).U(3, s.Uid).U(4, ServerId), IdExit, s.Uid);
            Flush(ctx, now);
            if (ctx.T.Occupied == 0 && ctx.Watchers.IsEmpty) { tables.TryRemove(ctx.T.Def.Id, out _); PokerLog.Write("table " + ctx.T.Def.Id + " closed (rake total " + ctx.T.TotalRake + ")"); }
            return cash;
        }

        static void ChangeTable(Client.GameClient client)
        {
            Session s;
            if (!sessions.TryGetValue(client.Player.UID, out s)) { Send(client, GameList(4, 0, err: 2), IdGameListAct); return; }
            var old = s.Ctx; var lv = old.Level;
            ulong cash;
            lock (old.T.Gate) cash = CashOut(old, s, Clock(), "change table");
            Send(client, GameList(4, lv.Type, err: 0), IdGameListAct);
            ulong buy = Math.Max(lv.MinBuyIn, Math.Min(lv.MaxBuyIn, cash));
            if (PokerMoney.Balance(client) < buy) { Send(client, GameList(3, lv.Type), IdGameListAct); Notify(client, "Buy-in failed: insufficient silver."); return; }
            if (!Join(client, lv, buy, s.AutoRebuy, s.AutoTopOff, old)) Send(client, GameList(3, lv.Type), IdGameListAct);
        }

        static void QuickPlay(Client.GameClient client, uint type)
        {
            var cfg = PokerConfig.Current;
            if (sessions.ContainsKey(client.Player.UID)) { Send(client, GameList(5, type, err: 4), IdGameListAct); return; }
            if (!cfg.Enabled || !TypeEnabled(type)) { Send(client, GameList(5, type, err: 5), IdGameListAct); return; }
            // DERIVED: the highest blind level whose minimum buy-in the player can pay; buy-in = that minimum.
            PokerLevel pick = null;
            foreach (var lv in cfg.Levels.Values)
                if (lv.Enabled && lv.Type == type && lv.MinBuyIn <= PokerMoney.Balance(client) && (pick == null || lv.MinBuyIn > pick.MinBuyIn)) pick = lv;
            if (pick == null) { Send(client, GameList(5, type, err: 5), IdGameListAct); return; }
            Join(client, pick, pick.MinBuyIn, false, false, null);
        }

        static void StandUp(Client.GameClient client)
        {
            Session s;
            if (!sessions.TryGetValue(client.Player.UID, out s)) return;
            var ctx = s.Ctx;
            lock (ctx.T.Gate)
            {
                var seat = ctx.T.SeatOf(s.Uid);
                int idx = seat != null ? seat.Index : 0;
                CashOut(ctx, s, Clock(), "stand up");
                ctx.Watchers[client.Player.UID] = client;
                tables[ctx.T.Def.Id] = ctx;
                // hero becomes a spectator: status 2
                var w = new PbW().U(1, 1).U(2, 2).U(3, (ulong)idx).U(4, ctx.T.Def.Id).U(5, ServerId).U(6, client.Player.UID)
                    .U(7, 0).U(8, ctx.T.Def.Id).U(9, 0).U(12, ctx.RaceType).U(13, ctx.T.Def.BigBlind).U(14, 0).U(15, (ulong)ctx.T.Seats.Length).U(16, 0);
                Send(client, w, IdEnter);
            }
        }

        static void SetOptions(Client.GameClient client, bool autoTop, bool autoRebuy)
        {
            Session s;
            if (!sessions.TryGetValue(client.Player.UID, out s)) return;
            s.AutoTopOff = autoTop; s.AutoRebuy = autoRebuy;
            Send(client, new PbW().U(1, 7).Rep(2, new uint[] { autoTop ? 1u : 0u, autoRebuy ? 1u : 0u }), IdNewTexasAct);
        }

        /// <summary>2463 a16: top-up / re-buy while seated (or sit down again as a spectator). Applied between hands only.</summary>
        static void TopUp(Client.GameClient client, ulong amount, bool autoRebuy, bool autoTop)
        {
            uint uid = client.Player.UID;
            Session s;
            if (!sessions.TryGetValue(uid, out s))
            {
                // spectator pressing buy-in again -> normal join at the watched table's level
                foreach (var t in tables.Values)
                    if (t.Watchers.ContainsKey(uid))
                    {
                        if (amount < t.Level.MinBuyIn || amount > t.Level.MaxBuyIn || PokerMoney.Balance(client) < amount) { Notify(client, "Buy-in failed."); return; }
                        t.Watchers.TryRemove(uid, out _);
                        Join(client, t.Level, amount, autoRebuy, autoTop, null);
                        return;
                    }
                return;
            }
            s.AutoRebuy = autoRebuy; s.AutoTopOff = autoTop;
            var ctx = s.Ctx;
            lock (ctx.T.Gate)
            {
                var seat = ctx.T.SeatOf(uid);
                if (seat == null || amount == 0) return;
                ulong room = ctx.T.Def.MaxBuyIn > seat.Stack ? ctx.T.Def.MaxBuyIn - seat.Stack : 0;
                if (amount > room) amount = room;
                if (amount == 0) { Notify(client, "You already have the maximum chips for this table."); return; }
                if (seat.InHand && ctx.T.Stage != PokerStage.Idle) { s.PendingTopUp = amount; Notify(client, "Your chips will be added after this game."); return; }
                ApplyTopUp(ctx, s, seat, amount, "top-up");
                Flush(ctx, Clock());
            }
        }

        /// <summary>Caller holds the table gate; between hands only.</summary>
        static bool ApplyTopUp(TableCtx ctx, Session s, PokerSeat seat, ulong amount, string why)
        {
            var c = s.Client;
            if (c == null || c.Player == null || amount == 0 || PokerMoney.Balance(c) < amount) return false;
            ulong after = seat.Stack + amount;
            if (after > ctx.T.Def.MaxBuyIn) return false;
            PokerLedger.SetChips(s.Id, s.Uid, ctx.T.Def.Id, after, true);
            if (!PokerMoney.Debit(c, amount, why + " table " + ctx.T.Def.Id)) { PokerLedger.SetChips(s.Id, s.Uid, ctx.T.Def.Id, seat.Stack, true); return false; }
            PokerMoney.Save(c);
            ulong added = ctx.T.AddChips(s.Uid, amount);
            if (added != amount)
            {
                // engine refused part of it (should not happen: checked above) -> give the rest back at once
                ulong back = amount - added;
                PokerMoney.Credit(c, back, "top-up refund");
                PokerLedger.SetChips(s.Id, s.Uid, ctx.T.Def.Id, seat.Stack, true);
                PokerMoney.Save(c);
            }
            s.BustedAt = 0;
            PokerLog.Write(c, why.ToUpperInvariant() + " table " + ctx.T.Def.Id + " +" + added + " -> " + seat.Stack);
            return added > 0;
        }

        static void AddTime(Client.GameClient client)
        {
            var ctx = TableOf(client.Player.UID);
            if (ctx == null) return;
            uint suid = client.Player.UID;
            lock (ctx.T.Gate)
            {
                var seat = ctx.T.SeatOf(suid);
                if (seat == null || ctx.T.ToAct != seat.Index) return;
                if (ctx.AddTimeHand != ctx.T.HandNo) { ctx.AddTimeHand = ctx.T.HandNo; ctx.AddTimeUsed.Clear(); }
                int used; ctx.AddTimeUsed.TryGetValue(suid, out used);
                int max = (int)PokerConfig.D(PokerConfig.Obj(PokerConfig.Current.Raw, "cash"), "addTimeMaxPerHand", 2);
                if (used >= max) return;
                ulong cost = ctx.T.Def.BigBlind * (ulong)PokerConfig.D(PokerConfig.Obj(PokerConfig.Current.Raw, "cash"), "addTimeCostBigBlinds", 1);
                if (cost > 0 && !PokerMoney.Debit(client, cost, "add time table " + ctx.T.Def.Id)) { Notify(client, "You don`t have enough money to reset the duration."); return; }
                int secs = ctx.T.Def.ActionSeconds;
                ctx.T.ExtendTurn(Clock(), secs);
                used++; ctx.AddTimeUsed[suid] = used;
                Broadcast(ctx, new PbW().U(1, 8).Rep(2, new uint[] { 0, (uint)secs, (uint)used }), IdNewTexasAct);
            }
        }

        static void ResendTurn(TableCtx ctx, Client.GameClient c)
        {
            if (ctx.T.ToAct < 0) return;
            var seat = ctx.T.Seats[ctx.T.ToAct];
            var e = ctx.T.TurnSnapshot(seat.Index);
            Send(c, TurnPacket(e, Math.Max(1, (int)((ctx.T.ActionDeadline - Clock()) / 1000))), IdActivePlayer);
        }

        static PbW TurnPacket(PokerEvent e, int secs)
        {
            return new PbW().U(1, (ulong)secs).U(2, e.Mask).U(3, e.MinAdd).U(4, e.MaxAdd).U(5, ServerId).U(6, e.Uid).U(7, e.CallAmt).B(8, false);
        }

        // ------------------------------------------------------------------ events -> packets
        /// <summary>Caller holds ctx.T.Gate.</summary>
        internal static void Flush(TableCtx ctx, long now)
        {
            var t = ctx.T;
            foreach (var e in t.Drain())
            {
                switch (e.Kind)
                {
                    case PokerEventKind.Seat:
                        Broadcast(ctx, ChipsOne(e.Uid, e.Stack), IdTableChip);
                        break;
                    case PokerEventKind.HandStart: SendHandStart(ctx, e); break;
                    case PokerEventKind.Turn:
                        Broadcast(ctx, TurnPacket(e, e.Seconds), IdActivePlayer);
                        break;
                    case PokerEventKind.Action:
                        {
                            ulong f3 = e.Act == PokerAct.AllIn ? e.Amount : e.Bet;
                            Broadcast(ctx, new PbW().U(2, (uint)e.Act).U(3, f3).U(4, e.Bet).U(5, ServerId).U(6, e.Uid), IdCallAction);
                            if (e.Amount > 0) Broadcast(ctx, ChipsOne(e.Uid, e.Stack), IdTableChip);
                            Broadcast(ctx, Pot(e.Pot), IdData);
                            break;
                        }
                    case PokerEventKind.Board:
                        {
                            Broadcast(ctx, Pot(e.Pot), IdData);
                            var w = new PbW().U(1, 0).U(2, (ulong)(e.Stage == PokerStage.Flop ? 1 : e.Stage == PokerStage.Turn ? 2 : 3));
                            foreach (int c in e.Cards) w.Msg(9, Card(c));
                            Broadcast(ctx, w, IdDealtCard);
                            break;
                        }
                    case PokerEventKind.Result: SendResult(ctx, e, now); break;
                    case PokerEventKind.Leave: break;        // announced by CashOut (2472)
                    case PokerEventKind.Error:
                        PokerLog.Write("TABLE " + t.Def.Id + " FROZEN: " + e.Text);
                        break;
                }
            }
            if (t.Frozen) CloseFrozen(ctx, now);
        }

        static void SendHandStart(TableCtx ctx, PokerEvent e)
        {
            var t = ctx.T;
            // players that became active get their status refreshed
            foreach (var kv in e.Holes)
            {
                uint st; ctx.AnnouncedStatus.TryGetValue(kv.Key, out st);
                if (st != 1)
                {
                    ctx.AnnouncedStatus[kv.Key] = 1;
                    var seat = t.Seats[kv.Key];
                    foreach (var c in Audience(ctx).ToList())
                        Send(c, EnterFor(ctx, seat, c.Player.UID == seat.Uid, 1), IdEnter);
                }
            }
            foreach (var s in t.Seats) if (!s.Empty && !e.Holes.ContainsKey(s.Index)) ctx.AnnouncedStatus[s.Index] = 3;
            Broadcast(ctx, new PbW().U(1, 0).Rep(2, new uint[] { 1 }), IdData);
            foreach (var c in Audience(ctx).ToList())
            {
                uint me = c.Player.UID;
                var w = new PbW().U(1, 0).U(2, 0)
                    .U(3, ServerId).U(4, e.Uids[e.Dealer])
                    .U(5, ServerId).U(6, e.Uids[e.Sb])
                    .U(7, ServerId).U(8, e.Uids[e.Bb])
                    .U(11, ServerId).U(12, 0);
                foreach (var kv in e.Holes)
                    if (e.Uids[kv.Key] == me) foreach (int card in kv.Value) w.Msg(9, Card(card));
                foreach (var kv in e.Holes)
                {
                    if (e.Uids[kv.Key] == me) continue;
                    var info = new PbW().U(1, ServerId).U(2, e.Uids[kv.Key]);
                    for (int i = 0; i < kv.Value.Count; i++) info.Msg(3, new PbW().U(1, 0).U(2, 0));   // face down placeholders
                    w.Msg(10, info);
                }
                Send(c, w, IdDealtCard);
            }
            // exact blind piles and stacks
            Broadcast(ctx, new PbW().U(2, (uint)PokerAct.SmallBlind).U(3, e.Bets[e.Sb]).U(4, e.Bets[e.Sb]).U(5, ServerId).U(6, e.Uids[e.Sb]), IdCallAction);
            Broadcast(ctx, new PbW().U(2, (uint)PokerAct.BigBlind).U(3, e.Bets[e.Bb]).U(4, e.Bets[e.Bb]).U(5, ServerId).U(6, e.Uids[e.Bb]), IdCallAction);
            var chips = new PbW();
            foreach (var kv in e.Stacks) chips.Msg(1, new PbW().U(1, ServerId).U(2, e.Uids[kv.Key]).U(3, 0).U(4, kv.Value));
            Broadcast(ctx, chips, IdTableChip);
            Broadcast(ctx, Pot(e.Pot), IdData);
        }

        /// <summary>Silver removed by the rake since start (house), for audits.</summary>
        public static long RakeTotal;

        static void SendResult(TableCtx ctx, PokerEvent e, long now)
        {
            var t = ctx.T;
            var r = e.Result;
            Interlocked.Add(ref RakeTotal, (long)r.Rake);
            if (r.Showdown && r.Hands.Count > 0)
            {
                var lay = new PbW().U(1, 0);
                foreach (var kv in r.Hands)
                {
                    var hole = t.Seats[kv.Key].Hole;
                    var info = new PbW().U(1, ServerId).U(2, e.Uids[kv.Key]);
                    foreach (int c in hole) info.Msg(3, Card(c));
                    lay.Msg(2, info);
                }
                Broadcast(ctx, lay, IdLayCard);
            }
            var w = new PbW().U(1, (ulong)t.Def.NextHandSeconds);
            foreach (var kv in r.Won)
                w.Msg(2, new PbW().U(1, ServerId).U(2, e.Uids[kv.Key]).U(3, kv.Value).U(6, kv.Value));
            int idx = 0;
            foreach (var p in r.Pots)
            {
                if (p.Amount == 0) continue;
                var pw = new PbW().U(1, p.Amount).U(2, (ulong)idx++);
                foreach (int s in p.Winners) pw.Msg(3, new PbW().U(1, ServerId).U(2, e.Uids[s]));
                w.Msg(3, pw);
            }
            Broadcast(ctx, w, IdGameResult);
            var chips = new PbW();
            foreach (var kv in e.Stacks) chips.Msg(1, new PbW().U(1, ServerId).U(2, e.Uids[kv.Key]).U(3, 0).U(4, kv.Value));
            Broadcast(ctx, chips, IdTableChip);
            Broadcast(ctx, Pot(0), IdData);
            foreach (var kv in r.Committed)
            {
                uint puid; if (!r.Uids.TryGetValue(kv.Key, out puid)) continue;
                var pc = ctx.Tour != null ? ctx.Tour.ClientOf(puid) : (sessions.TryGetValue(puid, out var ps0) ? ps0.Client : null);
                if (pc != null) PokerBoYi.OnHandPlayed(pc, r.Won.ContainsKey(kv.Key));
            }
            if (ctx.Tour != null) { ctx.Tour.AfterResult(ctx, e, now); return; }

            // journal: everyone's chips at this hand boundary (one atomic write)
            var j = new List<KeyValuePair<ulong, ulong>>();
            foreach (var s in t.Seats)
            {
                if (s.Empty) continue;
                Session ss;
                if (sessions.TryGetValue(s.Uid, out ss)) j.Add(new KeyValuePair<ulong, ulong>(ss.Id, s.Stack));
            }
            PokerLedger.SetMany(j);
            var sb = new System.Text.StringBuilder("HAND table " + t.Def.Id + " #" + r.HandNo + " board " + string.Join(" ", r.Board.Select(PokerCard.Name)) + " rake " + r.Rake);
            foreach (var kv in r.Net) sb.Append(" | ").Append(e.Uids.ContainsKey(kv.Key) ? e.Uids[kv.Key] : 0).Append(kv.Value >= 0 ? " +" : " ").Append(kv.Value)
                .Append(r.Hands.ContainsKey(kv.Key) ? " " + r.Hands[kv.Key] : "");
            PokerLog.Write(sb.ToString());

            // between hands: pending top-ups, auto top off / re-buy, busted players, sit-out kicks
            foreach (var s in t.Seats.Where(x => !x.Empty).ToList())
            {
                Session ss;
                if (!sessions.TryGetValue(s.Uid, out ss)) continue;
                if (ss.PendingTopUp > 0) { ulong a = Math.Min(ss.PendingTopUp, t.Def.MaxBuyIn > s.Stack ? t.Def.MaxBuyIn - s.Stack : 0); ss.PendingTopUp = 0; if (a > 0) ApplyTopUp(ctx, ss, s, a, "top-up"); }
                if (ss.AutoTopOff && s.Stack > 0 && s.Stack < ss.BuyAmount) ApplyTopUp(ctx, ss, s, ss.BuyAmount - s.Stack, "auto top off");
                if (s.Stack == 0)
                {
                    if (ss.AutoRebuy && ApplyTopUp(ctx, ss, s, ss.BuyAmount, "auto re-buy")) continue;
                    if (ss.BustedAt == 0)
                    {
                        ss.BustedAt = now;
                        if (ss.Client != null) Send(ss.Client, new PbW().U(1, 12).Rep(3, new ulong[] { t.Def.MinBuyIn, t.Def.MaxBuyIn }), IdNewTexasAct);
                    }
                }
                if (s.SitOut) ss.SitOutHands++;
            }
        }

        static void Housekeeping(TableCtx ctx, long now)
        {
            if (ctx.Tour != null) { ctx.Tour.Housekeeping(ctx, now); return; }
            var cfg = PokerConfig.Current;
            foreach (var s in ctx.T.Seats.Where(x => !x.Empty).ToList())
            {
                Session ss;
                if (!sessions.TryGetValue(s.Uid, out ss)) continue;
                bool offline = ss.Client == null || (!TestMode && (ss.Client.Socket == null || !ss.Client.Socket.Alive));
                bool busted = ss.BustedAt != 0 && s.Stack == 0 && now - ss.BustedAt > 30000;   // DERIVED: 30 s to re-buy
                bool idle = ss.SitOutHands >= cfg.SitOutHandsBeforeKick && ctx.T.Stage == PokerStage.Idle;
                if (offline || busted || idle)
                {
                    CashOut(ctx, ss, now, offline ? "offline" : busted ? "busted" : "sit out");
                    if (!offline && ss.Client != null) Send(ss.Client, new PbW().U(1, 6), IdNewTexasAct);
                }
            }
            foreach (var w in ctx.Watchers.ToList())
                if (w.Value == null || (!TestMode && (w.Value.Socket == null || !w.Value.Socket.Alive))) ctx.Watchers.TryRemove(w.Key, out _);
            if (ctx.T.Occupied == 0 && ctx.Watchers.IsEmpty) tables.TryRemove(ctx.T.Def.Id, out _);
        }

        static void CloseFrozen(TableCtx ctx, long now)
        {
            // integrity failure: nobody plays on; every seat is paid what the table still shows (never more)
            foreach (var s in ctx.T.Seats.Where(x => !x.Empty).ToList())
            {
                Session ss;
                if (!sessions.TryGetValue(s.Uid, out ss)) continue;
                ulong cash = s.Stack;
                sessions.TryRemove(s.Uid, out _);
                PokerLedger.SetChips(ss.Id, ss.Uid, ctx.T.Def.Id, cash, false);
                if (ss.Client != null && ss.Client.Player != null)
                {
                    PokerMoney.Credit(ss.Client, cash, "frozen table refund " + ctx.T.Def.Id);
                    PokerLedger.SetMarker(ss.Uid, 0);
                    PokerMoney.Save(ss.Client);
                    Send(ss.Client, new PbW().U(1, 6), IdNewTexasAct);
                }
            }
            tables.TryRemove(ctx.T.Def.Id, out _);
        }

        // ------------------------------------------------------------------ hooks (one-liners in existing files)
        /// <summary>MsgLoginHandler: pay back a session left open by a crash/restart.</summary>
        public static void OnLogin(Client.GameClient client)
        {
            try { PokerLedger.ApplyPendingRefund(client); } catch (Exception e) { PokerLog.Write(client, "login refund error: " + e.Message); }
        }

        /// <summary>Program disconnect path (before the save is queued): fold + cash out.</summary>
        public static void OnDisconnect(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null) return;
                Session s;
                if (sessions.TryGetValue(client.Player.UID, out s))
                {
                    var ctx = s.Ctx;
                    lock (ctx.T.Gate) CashOut(ctx, s, Clock(), "disconnect");
                }
                foreach (var t in tables.Values) t.Watchers.TryRemove(client.Player.UID, out _);
                PokerSng.OnDisconnect(client);
                PokerMtt.OnDisconnect(client);
            }
            catch (Exception e) { PokerLog.Write(client, "disconnect error: " + e.Message); }
        }
    }

    public static class PokerClock
    {
        public static long Now()
        {
            return System.Diagnostics.Stopwatch.GetTimestamp() / (System.Diagnostics.Stopwatch.Frequency / 1000);
        }
    }
}
