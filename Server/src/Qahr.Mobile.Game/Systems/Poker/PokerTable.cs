// [Poker 2026-10-14] No-limit Texas Hold'em / Omaha table engine. Pure logic: no sockets, no silver.
// The module (PokerModule) turns the queued events into client packets and moves silver in/out of the table.
//
// Money integrity: every chip at the table is in exactly one place - a seat stack, a seat's commitment in the
// current hand, or already raked. Audit() proves  sum(stacks) + sum(committed) == TotalIn - TotalOut - TotalRake
// after every state change; a broken table is frozen (no more hands) and reported.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Poker
{
    [Flags]
    public enum PokerAct : uint
    {
        None = 0, Bet = 0x1, Call = 0x2, Fold = 0x4, Check = 0x8, Raise = 0x10, AllIn = 0x20, SmallBlind = 0x40, BigBlind = 0x80, Ante = 0x100
    }

    public enum PokerStage { Idle = 0, Preflop = 1, Flop = 2, Turn = 3, River = 4, Showdown = 5 }

    public enum PokerVariant { Texas = 0, Omaha = 1 }

    public sealed class PokerTableDef
    {
        public uint Id;              // table instance id (unique)
        public uint LevelId;         // client NewTexasTableList id (blind level)
        public uint GameType;        // client game/race type
        public PokerVariant Variant = PokerVariant.Texas;
        public int Seats = 9;
        public ulong SmallBlind, BigBlind;
        public ulong MinBuyIn, MaxBuyIn;
        public int ActionSeconds = 15;
        public int NextHandSeconds = 5;
        public double RakePercent;   // derived (config)
        public ulong RakeCap;        // derived (config), 0 = no cap
        public bool NoFlopNoDrop = true;
        public bool Tournament;      // SNG/MTT: chips are tournament chips, no rake, no cash-out
    }

    public sealed class PokerSeat
    {
        public int Index;
        public uint Uid;
        public string Name = "";
        public ulong Stack;
        public bool InHand;          // dealt into the current hand
        public bool Folded, AllIn;
        public ulong Bet;            // chips put in during the current betting round
        public ulong Committed;      // chips put in during the whole hand (includes Bet)
        public bool Acted;           // acted since the betting level was last fully raised
        public ulong ActedAtLevel;   // full-bet level when this seat last acted
        public PokerAct LastAction;
        public List<int> Hole = new List<int>();
        public bool LeaveAfterHand;
        public bool SitOut;          // trusteeship / timed out: auto check-fold
        public int Timeouts;
        public bool Empty { get { return Uid == 0; } }
        public bool Live { get { return InHand && !Folded; } }
        public bool CanAct { get { return InHand && !Folded && !AllIn; } }
    }

    public sealed class PokerPot
    {
        public ulong Amount;
        public List<int> Eligible = new List<int>();
        public List<int> Winners = new List<int>();
        public ulong Rake;
    }

    public sealed class PokerHandResult
    {
        public ulong HandNo;
        public bool Showdown;
        public List<PokerPot> Pots = new List<PokerPot>();
        public Dictionary<int, ulong> Won = new Dictionary<int, ulong>();        // seat -> chips won (gross, after rake)
        public Dictionary<int, long> Net = new Dictionary<int, long>();          // seat -> won - committed
        public Dictionary<int, PokerHandValue> Hands = new Dictionary<int, PokerHandValue>();
        public Dictionary<int, ulong> Refund = new Dictionary<int, ulong>();     // uncalled bets returned
        public ulong Rake;
        public List<int> Board = new List<int>();
        public Dictionary<int, ulong> Committed = new Dictionary<int, ulong>();
        public Dictionary<int, uint> Uids = new Dictionary<int, uint>();
    }

    public enum PokerEventKind { Seat, Leave, HandStart, Turn, Action, Board, Result, Error }

    public sealed class PokerEvent
    {
        public PokerEventKind Kind;
        public int Seat = -1;
        public uint Uid;
        public PokerAct Act;
        public ulong Amount;         // Action: chips put in by this action; Leave: chips cashed out
        public PokerStage Stage;
        public PokerHandResult Result;
        public string Text;
        // snapshots taken when the event was raised (the table may have moved on when the event is sent)
        public ulong Bet, Stack, Pot, HandNo;
        public List<int> Cards;                                  // Board: cards dealt by this street
        public int Dealer = -1, Sb = -1, Bb = -1;                // HandStart
        public Dictionary<int, uint> Uids;                       // HandStart / Result: seat -> uid
        public Dictionary<int, List<int>> Holes;                 // HandStart: seat -> hole cards (server only, never broadcast)
        public Dictionary<int, ulong> Stacks, Bets;              // HandStart / Result
        public uint Mask; public ulong CallAmt, MinAdd, MaxAdd; public int Seconds; // Turn
        public override string ToString() { return Kind + " seat " + Seat + " " + Act + " " + Amount + " " + Stage + (Text != null ? " " + Text : ""); }
    }

    public sealed class PokerTable
    {
        public readonly PokerTableDef Def;
        public readonly PokerSeat[] Seats;
        public readonly object Gate = new object();
        public readonly List<PokerEvent> Events = new List<PokerEvent>();

        public PokerStage Stage { get; private set; }
        public ulong HandNo { get; private set; }
        public int Button { get; private set; } = -1;
        public int SmallBlindSeat { get; private set; } = -1;
        public int BigBlindSeat { get; private set; } = -1;
        public int ToAct { get; private set; } = -1;
        public ulong CurrentBet { get; private set; }   // highest Bet this round
        public ulong MinRaise { get; private set; }     // minimum raise increment
        public ulong FullLevel { get; private set; }    // bet level created by the last full bet/raise
        public long ActionDeadline { get; private set; }
        public long NextHandAt { get; private set; }
        public readonly List<int> Board = new List<int>();
        public bool Frozen { get; private set; }
        public string FrozenWhy { get; private set; }

        // accounting
        public ulong TotalIn { get; private set; }
        public ulong TotalOut { get; private set; }
        public ulong TotalRake { get; private set; }

        PokerDeck deck;
        bool sawFlop;
        int lastAggressor = -1;
        public Func<PokerDeck> DeckFactory = () => new PokerDeck();
        public PokerHandResult LastResult;

        public PokerTable(PokerTableDef def)
        {
            Def = def;
            Seats = new PokerSeat[def.Seats];
            for (int i = 0; i < def.Seats; i++) Seats[i] = new PokerSeat { Index = i };
            Stage = PokerStage.Idle;
        }

        // ------------------------------------------------------------------ queries
        public int Occupied { get { int n = 0; foreach (var s in Seats) if (!s.Empty) n++; return n; } }
        public PokerSeat SeatOf(uint uid) { foreach (var s in Seats) if (!s.Empty && s.Uid == uid) return s; return null; }
        public bool InHand { get { return Stage != PokerStage.Idle; } }
        public ulong PotTotal { get { ulong t = 0; foreach (var s in Seats) t += s.Committed; return t; } }
        public ulong PotWithoutRoundBets { get { ulong t = 0; foreach (var s in Seats) t += s.Committed - s.Bet; return t; } }

        public ulong ToCall(PokerSeat s) { return CurrentBet > s.Bet ? Math.Min(CurrentBet - s.Bet, s.Stack) : 0; }
        public bool CanCheck(PokerSeat s) { return s.Bet >= CurrentBet; }
        public bool CanRaise(PokerSeat s)
        {
            if (!s.CanAct || s.Stack <= ToCall(s)) return false;          // calling already puts him all-in
            if (s.Acted && s.ActedAtLevel >= FullLevel) return false;      // short all-in did not reopen the betting
            int others = 0; foreach (var o in Seats) if (o != s && o.CanAct) others++;
            return others > 0;                                             // nobody left to raise against
        }
        /// <summary>Smallest legal "raise to" total (for this round), capped at the all-in total.</summary>
        public ulong MinRaiseTo(PokerSeat s)
        {
            ulong to = CurrentBet == 0 ? Math.Max(Def.BigBlind, MinRaise) : CurrentBet + MinRaise;
            return Math.Min(to, s.Bet + s.Stack);
        }
        public ulong MaxRaiseTo(PokerSeat s) { return s.Bet + s.Stack; }

        // ------------------------------------------------------------------ seating (chips come from the module's ledger)
        public PokerSeat Sit(int seat, uint uid, string name, ulong stack, long now)
        {
            if (Frozen) throw new InvalidOperationException("table frozen");
            if (uid == 0) throw new ArgumentException("uid");
            if (SeatOf(uid) != null) throw new InvalidOperationException("already seated");
            if (seat < 0 || seat >= Seats.Length || !Seats[seat].Empty) throw new InvalidOperationException("seat taken");
            if (!Def.Tournament && (stack < Def.MinBuyIn || stack > Def.MaxBuyIn)) throw new InvalidOperationException("buy-in out of range");
            var s = Seats[seat];
            s.Uid = uid; s.Name = name ?? ""; s.Stack = stack;
            s.InHand = false; s.Folded = false; s.AllIn = false; s.Bet = 0; s.Committed = 0; s.Acted = false; s.Hole.Clear();
            s.LeaveAfterHand = false; s.SitOut = false; s.Timeouts = 0; s.LastAction = PokerAct.None;
            TotalIn += stack;
            Emit(PokerEventKind.Seat, seat, uid);
            Audit("sit");
            if (Stage == PokerStage.Idle && NextHandAt == 0) NextHandAt = now + 1000L * Def.NextHandSeconds;
            return s;
        }

        public int FirstFreeSeat()
        {
            for (int i = 0; i < Seats.Length; i++) if (Seats[i].Empty) return i;
            return -1;
        }

        /// <summary>Top up between hands (Auto Top Off / re-buy). Returns chips actually added.</summary>
        public ulong AddChips(uint uid, ulong amount)
        {
            var s = SeatOf(uid);
            if (s == null || amount == 0 || Frozen) return 0;
            if (s.InHand && Stage != PokerStage.Idle) return 0;            // never during a hand
            if (!Def.Tournament && s.Stack + amount > Def.MaxBuyIn) amount = Def.MaxBuyIn > s.Stack ? Def.MaxBuyIn - s.Stack : 0;
            if (amount == 0) return 0;
            s.Stack += amount; TotalIn += amount;
            Emit(PokerEventKind.Seat, s.Index, uid);
            Audit("addchips");
            return amount;
        }

        /// <summary>
        /// Player leaves. In a hand he is folded first; his committed chips stay in the pot. Returns the chips that
        /// leave the table (the module credits them as silver). The seat is freed immediately.
        /// </summary>
        public ulong Leave(uint uid, long now)
        {
            var s = SeatOf(uid);
            if (s == null) return 0;
            if (s.Live && Stage != PokerStage.Idle)
            {
                if (ToAct == s.Index) Act(uid, PokerAct.Fold, 0, now);
                else { s.Folded = true; s.LastAction = PokerAct.Fold; Emit(PokerEventKind.Action, s.Index, uid, PokerAct.Fold, 0); AfterFoldOutOfTurn(now); }
            }
            ulong cash = s.Stack;
            // committed chips of a folded leaver stay in the hand as a "ghost" contribution
            if (s.InHand && Stage != PokerStage.Idle)
            {
                ghosts.Add(new PokerSeat { Index = s.Index, Uid = s.Uid, Name = s.Name, Committed = s.Committed, Bet = s.Bet, InHand = true, Folded = true });
            }
            s.Stack = 0;
            TotalOut += cash;
            Emit(PokerEventKind.Leave, s.Index, uid, PokerAct.None, cash);
            ClearSeat(s);
            Audit("leave");
            return cash;
        }

        readonly List<PokerSeat> ghosts = new List<PokerSeat>();

        void ClearSeat(PokerSeat s)
        {
            s.Uid = 0; s.Name = ""; s.Stack = 0; s.InHand = false; s.Folded = false; s.AllIn = false; s.Bet = 0; s.Committed = 0;
            s.Hole.Clear(); s.LeaveAfterHand = false; s.SitOut = false; s.Timeouts = 0; s.Acted = false; s.LastAction = PokerAct.None;
        }

        public void SetSitOut(uint uid, bool on)
        {
            var s = SeatOf(uid);
            if (s != null) { s.SitOut = on; if (!on) s.Timeouts = 0; }
        }

        // ------------------------------------------------------------------ hand flow
        public void Tick(long now)
        {
            if (Frozen) return;
            if (Stage == PokerStage.Idle)
            {
                if (NextHandAt != 0 && now >= NextHandAt) TryStartHand(now);
                return;
            }
            if (ToAct >= 0 && ActionDeadline != 0 && now >= ActionDeadline)
            {
                var s = Seats[ToAct];
                s.Timeouts++;
                if (s.Timeouts >= 2) s.SitOut = true;
                Act(s.Uid, CanCheck(s) ? PokerAct.Check : PokerAct.Fold, 0, now, true);
            }
        }

        public bool TryStartHand(long now)
        {
            NextHandAt = 0;
            if (Frozen || Stage != PokerStage.Idle) return false;
            var players = new List<PokerSeat>();
            foreach (var s in Seats) if (!s.Empty && s.Stack > 0 && !s.LeaveAfterHand && (!s.SitOut || Def.Tournament)) players.Add(s);
            if (players.Count < 2) return false;

            HandNo++;
            ghosts.Clear();
            Board.Clear();
            sawFlop = false;
            lastAggressor = -1;
            foreach (var s in Seats)
            {
                s.InHand = !s.Empty && s.Stack > 0 && !s.LeaveAfterHand && (!s.SitOut || Def.Tournament);   // tournaments blind away players off
                s.Folded = false; s.AllIn = false; s.Bet = 0; s.Committed = 0; s.Acted = false; s.ActedAtLevel = 0;
                s.Hole.Clear(); s.LastAction = PokerAct.None;
            }
            // button
            Button = NextSeat(Button, x => x.InHand);
            if (players.Count == 2)
            {
                SmallBlindSeat = Button;
                BigBlindSeat = NextSeat(Button, x => x.InHand);
            }
            else
            {
                SmallBlindSeat = NextSeat(Button, x => x.InHand);
                BigBlindSeat = NextSeat(SmallBlindSeat, x => x.InHand);
            }
            deck = DeckFactory();
            Stage = PokerStage.Preflop;
            CurrentBet = 0; MinRaise = Def.BigBlind; FullLevel = 0;
            // blinds
            Put(Seats[SmallBlindSeat], Math.Min(Def.SmallBlind, Seats[SmallBlindSeat].Stack));
            Seats[SmallBlindSeat].LastAction = PokerAct.SmallBlind;
            Put(Seats[BigBlindSeat], Math.Min(Def.BigBlind, Seats[BigBlindSeat].Stack));
            Seats[BigBlindSeat].LastAction = PokerAct.BigBlind;
            CurrentBet = Math.Max(Seats[SmallBlindSeat].Bet, Seats[BigBlindSeat].Bet);
            if (CurrentBet < Def.BigBlind) CurrentBet = Def.BigBlind;   // a short BB all-in still sets the level to call
            FullLevel = Def.BigBlind;
            // deal (one card at a time, starting left of the button)
            int holeCards = Def.Variant == PokerVariant.Omaha ? 4 : 2;
            for (int r = 0; r < holeCards; r++)
            {
                int i = SmallBlindSeat;
                for (int k = 0; k < Seats.Length; k++)
                {
                    if (Seats[i].InHand) Seats[i].Hole.Add(deck.Draw());
                    i = (i + 1) % Seats.Length;
                }
            }
            Emit(PokerEventKind.HandStart, -1, 0);
            Audit("start");
            // first to act preflop: left of BB (heads-up: the button/SB)
            int first = NextSeat(BigBlindSeat, x => x.CanAct);
            BeginRoundOrAdvance(first, now);
            return true;
        }

        void Put(PokerSeat s, ulong amount)
        {
            if (amount > s.Stack) amount = s.Stack;
            s.Stack -= amount; s.Bet += amount; s.Committed += amount;
            if (s.Stack == 0 && s.InHand) s.AllIn = true;
        }

        int NextSeat(int from, Func<PokerSeat, bool> ok)
        {
            int n = Seats.Length;
            for (int k = 1; k <= n; k++)
            {
                int i = ((from < 0 ? -1 : from) + k + n) % n;
                if (ok(Seats[i])) return i;
            }
            return -1;
        }

        int LiveCount { get { int n = 0; foreach (var s in Seats) if (s.Live) n++; return n; } }
        int ActableCount { get { int n = 0; foreach (var s in Seats) if (s.CanAct) n++; return n; } }

        bool RoundComplete()
        {
            foreach (var s in Seats)
            {
                if (!s.CanAct) continue;
                if (!s.Acted || s.Bet < CurrentBet) return false;
            }
            return true;
        }

        void BeginRoundOrAdvance(int candidate, long now)
        {
            if (LiveCount <= 1) { FinishHand(now); return; }
            // only one player can still act and he has matched the bet -> no more betting
            if (ActableCount == 0 || (ActableCount == 1 && Seats.First(x => x.CanAct).Bet >= CurrentBet))
            {
                RunOutBoard(now);
                return;
            }
            if (RoundComplete()) { NextStreet(now); return; }
            int i = candidate;
            if (i < 0 || !Seats[i].CanAct || (Seats[i].Acted && Seats[i].Bet >= CurrentBet)) i = NextToAct(i < 0 ? Button : i - 1);
            SetTurn(i, now);
        }

        int NextToAct(int after)
        {
            return NextSeat(after, x => x.CanAct && (!x.Acted || x.Bet < CurrentBet));
        }

        void SetTurn(int seat, long now)
        {
            ToAct = seat;
            var s = Seats[seat];
            ActionDeadline = now + 1000L * (s.SitOut ? 1 : Def.ActionSeconds);
            Emit(PokerEventKind.Turn, seat, s.Uid);
        }

        /// <summary>Apply a player action. amount = "raise to" total for Bet/Raise (this round), ignored otherwise.</summary>
        public bool Act(uint uid, PokerAct act, ulong amount, long now, bool timeout = false)
        {
            if (Frozen || Stage == PokerStage.Idle || Stage == PokerStage.Showdown || ToAct < 0) return false;
            var s = Seats[ToAct];
            if (s.Uid != uid || !s.CanAct) return false;
            if (!timeout) s.Timeouts = 0;
            ulong toCall = ToCall(s);
            ulong put = 0;
            PokerAct done;
            switch (act)
            {
                case PokerAct.Fold:
                    s.Folded = true; done = PokerAct.Fold; break;
                case PokerAct.Check:
                    if (!CanCheck(s)) return false;
                    done = PokerAct.Check; break;
                case PokerAct.Call:
                    if (toCall == 0) { done = PokerAct.Check; break; }
                    put = toCall; done = put == s.Stack ? PokerAct.AllIn : PokerAct.Call; break;
                case PokerAct.AllIn:
                    put = s.Stack;
                    if (s.Bet + put > CurrentBet && !CanRaise(s)) return false;  // may only call / fold
                    done = PokerAct.AllIn; break;
                case PokerAct.Bet:
                case PokerAct.Raise:
                    {
                        if (!CanRaise(s)) return false;
                        ulong max = MaxRaiseTo(s);
                        if (amount > max) return false;
                        if (amount == max) { put = s.Stack; done = PokerAct.AllIn; break; }
                        if (amount < MinRaiseTo(s) || amount <= CurrentBet) return false;
                        put = amount - s.Bet; done = CurrentBet == 0 ? PokerAct.Bet : PokerAct.Raise;
                        break;
                    }
                default: return false;
            }
            ulong before = CurrentBet;
            if (put > 0) Put(s, put);
            s.Acted = true;
            if (s.Bet > before)
            {
                ulong inc = s.Bet - before;
                if (inc >= MinRaise || before == 0 && s.Bet >= Def.BigBlind)
                {
                    // full bet / raise: reopens the betting
                    if (inc > MinRaise) MinRaise = inc;
                    FullLevel = s.Bet;
                    lastAggressor = s.Index;
                }
                CurrentBet = s.Bet;
            }
            s.ActedAtLevel = FullLevel;
            s.LastAction = done;
            Emit(PokerEventKind.Action, s.Index, s.Uid, done, put);
            Audit("act");
            ToAct = -1;
            if (LiveCount <= 1) { FinishHand(now); return true; }
            int next = NextToAct(s.Index);
            if (next < 0 || RoundComplete()) { BeginRoundOrAdvance(-1, now); return true; }
            if (ActableCount == 1 && Seats[next].Bet >= CurrentBet) { BeginRoundOrAdvance(-1, now); return true; }
            SetTurn(next, now);
            return true;
        }

        void AfterFoldOutOfTurn(long now)
        {
            if (LiveCount <= 1) { ToAct = -1; FinishHand(now); }
        }

        void NextStreet(long now)
        {
            foreach (var s in Seats) { s.Bet = 0; s.Acted = false; s.ActedAtLevel = 0; }
            CurrentBet = 0; MinRaise = Def.BigBlind; FullLevel = 0;
            if (!DealStreet()) { FinishHand(now); return; }
            if (ActableCount <= 1) { RunOutBoard(now); return; }
            int first = NextSeat(Button, x => x.CanAct);
            SetTurn(first, now);
        }

        bool DealStreet()
        {
            switch (Stage)
            {
                case PokerStage.Preflop: deck.Draw(); for (int i = 0; i < 3; i++) Board.Add(deck.Draw()); Stage = PokerStage.Flop; sawFlop = true; break;
                case PokerStage.Flop: deck.Draw(); Board.Add(deck.Draw()); Stage = PokerStage.Turn; break;
                case PokerStage.Turn: deck.Draw(); Board.Add(deck.Draw()); Stage = PokerStage.River; break;
                default: return false;
            }
            Emit(PokerEventKind.Board, -1, 0, PokerAct.None, 0, Stage);
            return true;
        }

        void RunOutBoard(long now)
        {
            ToAct = -1;
            foreach (var s in Seats) { s.Bet = 0; }
            CurrentBet = 0;
            while (Stage != PokerStage.River && DealStreet()) { }
            FinishHand(now);
        }

        void FinishHand(long now)
        {
            ToAct = -1; ActionDeadline = 0;
            var all = new List<PokerSeat>();
            foreach (var s in Seats) if (s.InHand) all.Add(s);
            all.AddRange(ghosts);
            var live = all.Where(x => !x.Folded).ToList();
            var res = new PokerHandResult { HandNo = HandNo, Board = new List<int>(Board) };
            foreach (var s in all) { res.Committed[s.Index] = (res.Committed.ContainsKey(s.Index) ? res.Committed[s.Index] : 0) + s.Committed; res.Uids[s.Index] = s.Uid; }

            // 1) return an uncalled bet (top contributor above everyone else)
            var byC = all.OrderByDescending(x => x.Committed).ToList();
            if (byC.Count >= 1)
            {
                ulong second = byC.Count >= 2 ? byC[1].Committed : 0;
                var top = byC[0];
                if (top.Committed > second && !top.Folded)
                {
                    ulong back = top.Committed - second;
                    top.Committed -= back; top.Stack += back;
                    res.Refund[top.Index] = back;
                }
            }

            // 2) build pots from contribution levels
            var levels = all.Where(x => x.Committed > 0).Select(x => x.Committed).Distinct().OrderBy(x => x).ToList();
            ulong prev = 0;
            foreach (ulong lv in levels)
            {
                var pot = new PokerPot();
                foreach (var s in all) pot.Amount += Math.Min(s.Committed, lv) - Math.Min(s.Committed, prev);
                foreach (var s in live) if (s.Committed >= lv) pot.Eligible.Add(s.Index);
                prev = lv;
                if (pot.Amount == 0) continue;
                if (pot.Eligible.Count == 0 && res.Pots.Count > 0) { res.Pots[res.Pots.Count - 1].Amount += pot.Amount; continue; }
                if (pot.Eligible.Count == 0) { foreach (var s in live) pot.Eligible.Add(s.Index); }
                // merge with the previous pot if the eligible set is identical
                if (res.Pots.Count > 0 && res.Pots[res.Pots.Count - 1].Eligible.SequenceEqual(pot.Eligible)) res.Pots[res.Pots.Count - 1].Amount += pot.Amount;
                else res.Pots.Add(pot);
            }

            // 3) rake (cash tables only, derived values from the config)
            if (!Def.Tournament && Def.RakePercent > 0 && (sawFlop || !Def.NoFlopNoDrop))
            {
                ulong total = 0; foreach (var p in res.Pots) total += p.Amount;
                ulong rake = (ulong)Math.Floor(total * Def.RakePercent / 100.0);
                if (Def.RakeCap > 0 && rake > Def.RakeCap) rake = Def.RakeCap;
                ulong left = rake;
                foreach (var p in res.Pots)
                {
                    ulong r = Math.Min(left, p.Amount);
                    p.Amount -= r; p.Rake = r; left -= r;
                    if (left == 0) break;
                }
                res.Rake = rake - left;
            }

            // 4) showdown & award
            res.Showdown = live.Count > 1;
            if (res.Showdown)
                foreach (var s in live)
                {
                    var hole = SeatHole(s.Index);
                    if (hole.Count == 0) continue;
                    res.Hands[s.Index] = Def.Variant == PokerVariant.Omaha
                        ? PokerEval.BestOmaha(hole, Board)
                        : PokerEval.Best(hole.Concat(Board).ToList());
                }
            foreach (var p in res.Pots)
            {
                if (p.Amount == 0) continue;
                List<int> winners;
                if (!res.Showdown || p.Eligible.Count == 1) winners = new List<int>(p.Eligible.Count == 1 ? p.Eligible : live.Select(x => x.Index).ToList());
                else
                {
                    int best = p.Eligible.Where(i => res.Hands.ContainsKey(i)).Max(i => res.Hands[i].Score);
                    winners = p.Eligible.Where(i => res.Hands.ContainsKey(i) && res.Hands[i].Score == best).ToList();
                }
                // odd chips go to the first winner(s) left of the button
                winners = winners.OrderBy(i => (i - Button - 1 + Seats.Length * 2) % Seats.Length).ToList();
                p.Winners = winners;
                ulong share = p.Amount / (ulong)winners.Count, odd = p.Amount % (ulong)winners.Count;
                for (int k = 0; k < winners.Count; k++)
                {
                    ulong w = share + ((ulong)k < odd ? 1UL : 0UL);
                    res.Won[winners[k]] = (res.Won.ContainsKey(winners[k]) ? res.Won[winners[k]] : 0) + w;
                    var seat = Seats[winners[k]];
                    if (!seat.Empty && seat.InHand && !seat.Folded) seat.Stack += w;
                }
            }
            TotalRake += res.Rake;
            foreach (var kv in res.Committed)
            {
                ulong won = res.Won.ContainsKey(kv.Key) ? res.Won[kv.Key] : 0;
                ulong refund = res.Refund.ContainsKey(kv.Key) ? res.Refund[kv.Key] : 0;
                res.Net[kv.Key] = (long)won + (long)refund - (long)kv.Value;
            }
            foreach (var s in Seats) { s.Bet = 0; s.Committed = 0; }
            ghosts.Clear();
            Stage = PokerStage.Showdown;
            LastResult = res;
            Emit(PokerEventKind.Result, -1, 0, PokerAct.None, 0, PokerStage.Showdown, res);
            // hand over
            Stage = PokerStage.Idle;
            foreach (var s in Seats) { s.InHand = false; s.AllIn = false; s.Folded = false; }
            Audit("finish");
            NextHandAt = now + 1000L * Def.NextHandSeconds;
        }

        public List<int> SeatHole(int seat) { return Seats[seat].Hole; }

        // ------------------------------------------------------------------ integrity
        public bool Audit(string where)
        {
            ulong onTable = 0;
            foreach (var s in Seats) onTable += s.Stack + s.Committed;
            foreach (var g in ghosts) onTable += g.Committed;
            ulong expected = TotalIn - TotalOut - TotalRake;
            if (onTable != expected)
            {
                Frozen = true;
                FrozenWhy = "chip audit failed at " + where + ": on table " + onTable + " expected " + expected;
                Emit(PokerEventKind.Error, -1, 0, PokerAct.None, 0, Stage, null, FrozenWhy);
                return false;
            }
            return true;
        }

        /// <summary>Do not start a hand before 'when' (tournament start).</summary>
        public void HoldUntil(long when)
        {
            if (Stage == PokerStage.Idle && NextHandAt < when) NextHandAt = when;
        }

        public void ExtendTurn(long now, int seconds)
        {
            if (ToAct < 0 || seconds <= 0) return;
            ActionDeadline = Math.Max(ActionDeadline, now) + 1000L * seconds;
        }

        public PokerEvent TurnSnapshot(int seat) { return Build(PokerEventKind.Turn, seat, Seats[seat].Uid); }

        void Emit(PokerEventKind k, int seat, uint uid, PokerAct act = PokerAct.None, ulong amount = 0, PokerStage stage = PokerStage.Idle, PokerHandResult res = null, string text = null)
        {
            Events.Add(Build(k, seat, uid, act, amount, stage, res, text));
        }

        PokerEvent Build(PokerEventKind k, int seat, uint uid, PokerAct act = PokerAct.None, ulong amount = 0, PokerStage stage = PokerStage.Idle, PokerHandResult res = null, string text = null)
        {
            var e = new PokerEvent { Kind = k, Seat = seat, Uid = uid, Act = act, Amount = amount, Stage = stage == PokerStage.Idle ? Stage : stage, Result = res, Text = text, HandNo = HandNo };
            e.Pot = PotTotal;
            foreach (var g in ghosts) e.Pot += g.Committed;
            if (seat >= 0 && seat < Seats.Length) { e.Bet = Seats[seat].Bet; e.Stack = Seats[seat].Stack; }
            if (k == PokerEventKind.Board) e.Cards = Stage == PokerStage.Flop ? Board.GetRange(0, 3) : Board.GetRange(Board.Count - 1, 1);
            if (k == PokerEventKind.HandStart || k == PokerEventKind.Result)
            {
                e.Dealer = Button; e.Sb = SmallBlindSeat; e.Bb = BigBlindSeat;
                e.Uids = new Dictionary<int, uint>(); e.Holes = new Dictionary<int, List<int>>();
                e.Stacks = new Dictionary<int, ulong>(); e.Bets = new Dictionary<int, ulong>();
                foreach (var s in Seats)
                {
                    if (s.Empty) continue;
                    e.Stacks[s.Index] = s.Stack; e.Uids[s.Index] = s.Uid;
                    if (k == PokerEventKind.HandStart && s.InHand) { e.Holes[s.Index] = new List<int>(s.Hole); e.Bets[s.Index] = s.Bet; }
                }
                if (res != null) foreach (var kv in res.Uids) if (!e.Uids.ContainsKey(kv.Key)) e.Uids[kv.Key] = kv.Value;
            }
            if (k == PokerEventKind.Turn && seat >= 0)
            {
                var s = Seats[seat];
                ulong call = ToCall(s);
                bool raise = CanRaise(s);
                uint m = (uint)PokerAct.Fold;
                if (CanCheck(s)) m |= (uint)PokerAct.Check; else m |= (uint)PokerAct.Call;
                if (raise) m |= CurrentBet == 0 ? (uint)PokerAct.Bet : (uint)PokerAct.Raise;
                if (raise || s.Stack <= call) m |= (uint)PokerAct.AllIn;
                if (s.Stack <= call) m &= ~(uint)PokerAct.Call;       // calling = all-in
                e.Mask = m; e.CallAmt = call;
                if (raise)
                {
                    ulong minTo = MinRaiseTo(s), maxTo = MaxRaiseTo(s);
                    ulong basis = s.Bet + call;
                    e.MinAdd = minTo > basis ? minTo - basis : 0;
                    e.MaxAdd = maxTo > basis ? maxTo - basis : 0;
                }
                e.Seconds = s.SitOut ? 1 : Def.ActionSeconds;
            }
            return e;
        }

        public List<PokerEvent> Drain()
        {
            var l = new List<PokerEvent>(Events);
            Events.Clear();
            return l;
        }
    }
}
