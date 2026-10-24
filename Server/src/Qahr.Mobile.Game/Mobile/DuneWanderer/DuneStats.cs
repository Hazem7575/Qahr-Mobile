using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: the battle stats a Dune Wanderer gets from the system: meridian levels, acupoints (+ the quality boost
    /// of a full meridian), embedded glyphs (base + extra attribute + set effects of the strongest meridian per suit) and
    /// talents. Called from Equip.UpdateStats (one line); cached per player, dropped by DuneStore.Edit.
    /// Attribute ids are the client's (DuneData attributes help in Data\DuneWanderer\meridians.txt).
    /// </summary>
    public static class DuneStats
    {
        const int Attrs = 25;
        const int GlyphBonusAttr = 21;// "embedded glyph base attributes + value/100 %"
        const int DuneClassSort = 3;  // client profession sort of the Dune Wanderer (glyph set classes)
        static readonly ConcurrentDictionary<uint, long[]> cache = new ConcurrentDictionary<uint, long[]>();

        public static void Invalidate(uint uid)
        {
            long[] old;
            cache.TryRemove(uid, out old);
        }

        /// <summary>Equip.UpdateStats: adds the system's stats to Owner.Status.</summary>
        public static void Apply(Client.GameClient c)
        {
            if (c == null || c.Player == null || !Database.AtributesStatus.IsDune(c.Player.Class)) return;
            long[] a;
            try { a = cache.GetOrAdd(c.Player.UID, uid => Compute(DuneStore.Read(uid))); }
            catch (Exception e) { MyConsole.WriteLine("[Dune] stats of " + c.Player.Name + ": " + e.Message, ConsoleColor.Red); return; }
            var s = c.Status;
            s.MaxHitpoints += U(a[1]);
            s.MinAttack += U(a[2]); s.MaxAttack += U(a[2]);
            s.Defence += U(a[3]);
            s.MagicAttack += U(a[4]);
            s.MagicDefence += U(a[5]);
            s.PhysicalDamageIncrease += U(a[6]);
            s.PhysicalDamageDecrease += U(a[7]);
            s.MagicDamageIncrease += U(a[8]);
            s.MagicDamageDecrease += U(a[9]);
            s.CriticalStrike += U(a[10]);
            s.SkillCStrike += U(a[11]);
            s.Immunity += U(a[12]);
            s.Breakthrough += U(a[13]);
            s.Counteraction += U(a[14]);
        }

        static uint U(long v) { return (uint)Math.Max(0, Math.Min(uint.MaxValue, v)); }

        /// <summary>Recomputes the player's stats after a change (new totals reach the client with the stats packet).</summary>
        public static void Refresh(Client.GameClient c)
        {
            Invalidate(c.Player.UID);
            c.Equipment.QueryEquipment(c.Equipment.Alternante);
        }

        public static long[] Compute(DuneState st)
        {
            var a = new long[Attrs];
            AddMeridians(st, a);
            AddTalents(st, a);
            AddGlyphs(st, a);
            return a;
        }

        static void Add(long[] a, int attr, long value)
        {
            if (attr > 0 && attr < a.Length) a[attr] += value;
        }

        static void AddMeridians(DuneState st, long[] a)
        {
            for (int m = 1; m <= DuneData.MeridianCount; m++)
            {
                var level = DuneData.Meridian(m, st.Levels[m]);
                if (level != null) foreach (var x in level.Attrs) Add(a, x.Type, x.Value);
                int boost = FullMeridianBoost(st, m);
                foreach (var t in DuneData.AcupointTypes(m))
                {
                    var acu = DuneData.AcupointAt(t, st.Quality(t));
                    if (acu == null) continue;
                    foreach (var x in acu.Attrs) Add(a, x.Type, x.Value + x.Value * boost / 10000);
                }
            }
        }

        /// <summary>All 6 acupoints owned: + config "boost" rate of the lowest quality (1/10000), else 0.</summary>
        public static int FullMeridianBoost(DuneState st, int meridian)
        {
            int min = int.MaxValue;
            foreach (var t in DuneData.AcupointTypes(meridian)) min = Math.Min(min, st.Quality(t));
            int rate;
            return min > 0 && DuneData.Cfg.Boost.TryGetValue(min, out rate) ? rate : 0;
        }

        static void AddTalents(DuneState st, long[] a)
        {
            for (int tree = 1; tree <= 3; tree++)
                for (int node = 1; node <= 32; node++)
                {
                    if ((st.TalentNodes[tree] & (1u << (node - 1))) == 0) continue;
                    var t = DuneData.Talent(tree, node);
                    if (t != null) foreach (var x in t.Attrs) Add(a, x.Type, x.Value);
                }
        }

        static void AddGlyphs(DuneState st, long[] a)
        {
            var embedded = new List<Tuple<int, DuneState.GlyphItem>>();// (meridian, glyph)
            for (int m = 1; m <= DuneData.MeridianCount; m++)
                for (int h = 1; h <= DuneData.HolesPerMeridian; h++)
                {
                    DuneState.GlyphItem g;
                    if (st.Embedded[m, h] != 0 && st.Glyphs.TryGetValue(st.Embedded[m, h], out g)) embedded.Add(Tuple.Create(m, g));
                }
            // base attributes, + % of the "glyph attribute bonus" (talents / extra attributes)
            var baseAttr = new long[6];
            foreach (var e in embedded)
            {
                baseAttr[1] += e.Item2.Life; baseAttr[2] += e.Item2.Attack; baseAttr[3] += e.Item2.Defence; baseAttr[5] += e.Item2.MagicDefence;
                Add(a, e.Item2.AttrType, e.Item2.AttrValue);
            }
            long bonusPct = a[GlyphBonusAttr];// value/100 %
            var pct = new long[6];
            foreach (var x in StrongestSets(embedded))
            {
                if (x.GlyphAttr > 0 && x.GlyphAttr < pct.Length) pct[x.GlyphAttr] += x.Value;
                else Add(a, x.Attr, x.Value);
            }
            for (int i = 1; i < baseAttr.Length; i++)
                Add(a, i, baseAttr[i] + baseAttr[i] * pct[i] / 100 + baseAttr[i] * bonusPct / 10000);
        }

        /// <summary>Set effects: pieces of one suit in the same meridian; for every suit only its best meridian counts, and
        /// only suits whose set classes are empty (everyone) or include the Dune Wanderer.</summary>
        static IEnumerable<DuneData.SetEffect> StrongestSets(List<Tuple<int, DuneState.GlyphItem>> embedded)
        {
            var best = new Dictionary<int, int>();// suit -> pieces
            foreach (var bySuit in embedded.GroupBy(e => new { e.Item1, Suit = e.Item2.Type / 100 }))
            {
                var type = DuneData.Glyph(bySuit.First().Item2.Type);
                if (type == null || (type.SetClasses.Count > 0 && !type.SetClasses.Contains(DuneClassSort))) continue;
                int n, pieces = bySuit.Count();
                if (!best.TryGetValue(bySuit.Key.Suit, out n) || pieces > n) best[bySuit.Key.Suit] = pieces;
            }
            foreach (var kv in best)
            {
                Dictionary<int, List<DuneData.SetEffect>> bySize;
                List<DuneData.SetEffect> effects;
                if (DuneData.Glyphs.Sets.TryGetValue(kv.Key, out bySize) && bySize.TryGetValue(Math.Min(kv.Value, DuneData.HolesPerMeridian), out effects))
                    foreach (var e in effects) yield return e;
            }
        }

        /// <summary>Break Power level (the client's PoJin overview): what the owned acupoints add.</summary>
        public static int BreakPower(DuneState st)
        {
            int sum = 0;
            foreach (var kv in st.Acupoints)
            {
                var acu = DuneData.AcupointAt(kv.Key, kv.Value);
                if (acu != null) sum += acu.BreakPower;
            }
            return sum;
        }
    }
}
