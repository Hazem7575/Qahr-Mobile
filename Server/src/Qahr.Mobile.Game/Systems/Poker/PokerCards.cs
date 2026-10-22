// [Poker 2026-10-14] Cards, crypto shuffle and hand evaluation for the New Texas Hold'em tables.
// Pure logic, no networking. Card = 0..51: rank = 2 + card / 4 (2..14, ace high), suit = card % 4.
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Qahr.Mobile.Poker
{
    public static class PokerRng
    {
        static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static ulong NextU64()
        {
            var b = new byte[8];
            lock (rng) rng.GetBytes(b);
            return BitConverter.ToUInt64(b, 0);
        }

        /// <summary>Unbiased integer in [0, n).</summary>
        public static int Next(int n)
        {
            if (n <= 1) return 0;
            ulong range = (ulong)n;
            ulong limit = ulong.MaxValue - (ulong.MaxValue % range);
            ulong v;
            do v = NextU64(); while (v >= limit);
            return (int)(v % range);
        }

        public static ulong NextId()
        {
            ulong v;
            do v = NextU64() & 0x7FFFFFFFFFFFFFFFUL; while (v == 0);
            return v;
        }
    }

    public sealed class PokerDeck
    {
        readonly int[] cards = new int[52];
        int pos;

        public PokerDeck()
        {
            for (int i = 0; i < 52; i++) cards[i] = i;
            // Fisher-Yates with a cryptographic, unbiased RNG.
            for (int i = 51; i > 0; i--)
            {
                int j = PokerRng.Next(i + 1);
                int t = cards[i]; cards[i] = cards[j]; cards[j] = t;
            }
        }

        /// <summary>Test hook: a fixed order (first card dealt first).</summary>
        public PokerDeck(IList<int> order)
        {
            var used = new bool[52];
            int n = 0;
            foreach (int c in order) { if (c < 0 || c > 51 || used[c]) throw new ArgumentException("bad card " + c); used[c] = true; cards[n++] = c; }
            for (int c = 0; c < 52; c++) if (!used[c]) cards[n++] = c;
        }

        public int Remaining { get { return 52 - pos; } }
        public int Draw()
        {
            if (pos >= 52) throw new InvalidOperationException("deck empty");
            return cards[pos++];
        }
    }

    public static class PokerCard
    {
        public static int Rank(int c) { return 2 + c / 4; }
        public static int Suit(int c) { return c % 4; }
        public static int Make(int rank, int suit) { return (rank - 2) * 4 + suit; }
        static readonly string R = "23456789TJQKA", S = "cdhs";
        public static string Name(int c) { return c < 0 || c > 51 ? "??" : R[c / 4].ToString() + S[c % 4]; }
        public static int Parse(string s)
        {
            int r = R.IndexOf(char.ToUpperInvariant(s[0])), su = S.IndexOf(char.ToLowerInvariant(s[1]));
            if (r < 0 || su < 0) throw new ArgumentException(s);
            return r * 4 + su;
        }
    }

    /// <summary>Hand categories, same numbering as the client's STR_NEWTEXAS_CARDTYPE_STR_%d (0 high card .. 9 royal flush).</summary>
    public enum PokerHandType { HighCard = 0, OnePair = 1, TwoPair = 2, Trips = 3, Straight = 4, Flush = 5, FullHouse = 6, Quads = 7, StraightFlush = 8, RoyalFlush = 9 }

    public struct PokerHandValue : IComparable<PokerHandValue>
    {
        /// <summary>Comparable score: category in bits 20..23, then up to 5 ranks of 4 bits each.</summary>
        public int Score;
        public PokerHandType Type;
        /// <summary>The five cards that make the hand (best five).</summary>
        public int[] Best;
        public int CompareTo(PokerHandValue o) { return Score.CompareTo(o.Score); }
        public override string ToString()
        {
            var parts = new List<string>();
            if (Best != null) foreach (int c in Best) parts.Add(PokerCard.Name(c));
            return Type + " [" + string.Join(" ", parts) + "]";
        }
    }

    public static class PokerEval
    {
        /// <summary>Exact evaluation of exactly five cards.</summary>
        public static int Score5(int a, int b, int c, int d, int e, out PokerHandType type)
        {
            int[] r = { PokerCard.Rank(a), PokerCard.Rank(b), PokerCard.Rank(c), PokerCard.Rank(d), PokerCard.Rank(e) };
            int s0 = PokerCard.Suit(a);
            bool flush = PokerCard.Suit(b) == s0 && PokerCard.Suit(c) == s0 && PokerCard.Suit(d) == s0 && PokerCard.Suit(e) == s0;
            Array.Sort(r); Array.Reverse(r); // high -> low
            // count ranks
            var cnt = new int[15];
            foreach (int x in r) cnt[x]++;
            // straight (wheel A-5 counts with 5 high)
            bool straight = false; int top = 0;
            if (cnt[r[0]] == 1 && cnt[r[1]] == 1 && cnt[r[2]] == 1 && cnt[r[3]] == 1 && cnt[r[4]] == 1)
            {
                if (r[0] - r[4] == 4) { straight = true; top = r[0]; }
                else if (r[0] == 14 && r[1] == 5 && r[2] == 4 && r[3] == 3 && r[4] == 2) { straight = true; top = 5; }
            }
            if (straight && flush) { type = top == 14 ? PokerHandType.RoyalFlush : PokerHandType.StraightFlush; return Pack(8, top); }
            // groups ordered by (count desc, rank desc)
            var groups = new List<int[]>();
            for (int x = 14; x >= 2; x--) if (cnt[x] > 0) groups.Add(new[] { cnt[x], x });
            groups.Sort((p, q) => p[0] != q[0] ? q[0].CompareTo(p[0]) : q[1].CompareTo(p[1]));
            var ord = new List<int>();
            foreach (var g in groups) ord.Add(g[1]);
            if (groups[0][0] == 4) { type = PokerHandType.Quads; return Pack(7, ord.ToArray()); }
            if (groups[0][0] == 3 && groups[1][0] == 2) { type = PokerHandType.FullHouse; return Pack(6, ord.ToArray()); }
            if (flush) { type = PokerHandType.Flush; return Pack(5, r); }
            if (straight) { type = PokerHandType.Straight; return Pack(4, top); }
            if (groups[0][0] == 3) { type = PokerHandType.Trips; return Pack(3, ord.ToArray()); }
            if (groups[0][0] == 2 && groups[1][0] == 2) { type = PokerHandType.TwoPair; return Pack(2, ord.ToArray()); }
            if (groups[0][0] == 2) { type = PokerHandType.OnePair; return Pack(1, ord.ToArray()); }
            type = PokerHandType.HighCard; return Pack(0, r);
        }

        static int Pack(int cat, params int[] ranks)
        {
            int v = cat << 20;
            for (int i = 0; i < 5; i++) v |= (i < ranks.Length ? ranks[i] : 0) << (16 - 4 * i);
            return v;
        }

        /// <summary>Best five out of any 5..7 cards (Texas Hold'em: 2 hole + up to 5 board).</summary>
        public static PokerHandValue Best(IList<int> cards)
        {
            int n = cards.Count;
            if (n < 5) throw new ArgumentException("need 5+ cards");
            var best = new PokerHandValue { Score = -1 };
            var idx = new int[5];
            for (idx[0] = 0; idx[0] < n; idx[0]++)
                for (idx[1] = idx[0] + 1; idx[1] < n; idx[1]++)
                    for (idx[2] = idx[1] + 1; idx[2] < n; idx[2]++)
                        for (idx[3] = idx[2] + 1; idx[3] < n; idx[3]++)
                            for (idx[4] = idx[3] + 1; idx[4] < n; idx[4]++)
                            {
                                PokerHandType t;
                                int sc = Score5(cards[idx[0]], cards[idx[1]], cards[idx[2]], cards[idx[3]], cards[idx[4]], out t);
                                if (sc > best.Score)
                                    best = new PokerHandValue { Score = sc, Type = t, Best = new[] { cards[idx[0]], cards[idx[1]], cards[idx[2]], cards[idx[3]], cards[idx[4]] } };
                            }
            return best;
        }

        /// <summary>Omaha: exactly two of the hole cards and exactly three of the board.</summary>
        public static PokerHandValue BestOmaha(IList<int> hole, IList<int> board)
        {
            if (hole.Count < 2 || board.Count < 3) throw new ArgumentException("omaha needs 2+ hole and 3+ board");
            var best = new PokerHandValue { Score = -1 };
            for (int h1 = 0; h1 < hole.Count; h1++)
                for (int h2 = h1 + 1; h2 < hole.Count; h2++)
                    for (int b1 = 0; b1 < board.Count; b1++)
                        for (int b2 = b1 + 1; b2 < board.Count; b2++)
                            for (int b3 = b2 + 1; b3 < board.Count; b3++)
                            {
                                PokerHandType t;
                                int sc = Score5(hole[h1], hole[h2], board[b1], board[b2], board[b3], out t);
                                if (sc > best.Score)
                                    best = new PokerHandValue { Score = sc, Type = t, Best = new[] { hole[h1], hole[h2], board[b1], board[b2], board[b3] } };
                            }
            return best;
        }
    }
}
