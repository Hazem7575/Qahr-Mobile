using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.EventData;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: one Dune Wanderer's progress, kept in EventStore event "dune" (MySQL event_data). Keys:
    ///   c:yang c:yin (points)  c:sw (0 Yang / 1 Yin state)  c:draws c:pity  c:skill (special effect end, unix)  c:gid (next glyph id)
    ///   c:m&lt;meridian&gt; (level)  c:a&lt;acupoint&gt; (quality)  c:h&lt;meridian&gt; (open holes mask)  c:e&lt;meridian&gt;&lt;hole&gt; (embedded glyph id)
    ///   c:tv&lt;tree&gt; (talent marks)  c:tn&lt;tree&gt; (active nodes mask)  c:wday + s:w1 s:w2 ("type,ex,got" wishes of that day)
    ///   s:g&lt;id&gt; = "type,life,attack,defence,mdefence,attrType,attrValue" (a glyph)
    /// DuneStore.Edit is the only writer; every change goes through one EventStore.Edit (atomic per player).
    /// </summary>
    public sealed class DuneState
    {
        public sealed class GlyphItem
        {
            public uint Id;
            public int Type, Life, Attack, Defence, MagicDefence, AttrType, AttrValue;
        }

        public sealed class Wish { public int Type, TypeEx; public bool Got; }

        public long Yang, Yin, Draws, Pity, SkillEnd;
        public int Switch, WishDay;
        public uint NextGlyph = 1;
        public readonly int[] Levels = new int[DuneData.MeridianCount + 1];
        public readonly int[] Holes = new int[DuneData.MeridianCount + 1];
        public readonly uint[,] Embedded = new uint[DuneData.MeridianCount + 1, DuneData.HolesPerMeridian + 1];
        public readonly Dictionary<int, int> Acupoints = new Dictionary<int, int>();
        public readonly Dictionary<uint, GlyphItem> Glyphs = new Dictionary<uint, GlyphItem>();
        public readonly long[] TalentMarks = new long[4];
        public readonly uint[] TalentNodes = new uint[4];
        public readonly Wish[] Wishes = { null, new Wish(), new Wish() };

        // ------------------------------------------------------------------ rules on the state
        public int Quality(int acupoint)
        {
            int q;
            return Acupoints.TryGetValue(acupoint, out q) ? q : 0;
        }

        /// <summary>The client's "connected" count: the longest run of owned acupoints m01..m06 in id order.</summary>
        public int Connected(int meridian)
        {
            int best = 0, run = 0;
            foreach (var t in DuneData.AcupointTypes(meridian))
            {
                run = Quality(t) > 0 ? run + 1 : 0;
                best = Math.Max(best, run);
            }
            return best;
        }

        /// <summary>Skill group 1 = main (meridian 1), 2 = Yang (2+3), 3 = Yin (4+5).</summary>
        public int GroupConnected(int group)
        {
            return group == 1 ? Connected(1) : group == 2 ? Connected(2) + Connected(3) : Connected(4) + Connected(5);
        }

        public int SkillLevel(int group)
        {
            DuneData.Skill s;
            if (!DuneData.Cfg.Skills.TryGetValue(group, out s)) return 0;
            int have = GroupConnected(group), max = group == 1 ? 2 : 3, lv = 0;
            for (int i = 1; i <= max && i < s.Needed.Length; i++)
                if (have >= s.Needed[i]) lv = i;
            return lv;
        }

        /// <summary>Where a glyph is embedded: meridian*10 + hole, 0 = in the bag.</summary>
        public uint PosOf(uint glyphId)
        {
            for (int m = 1; m <= DuneData.MeridianCount; m++)
                for (int h = 1; h <= DuneData.HolesPerMeridian; h++)
                    if (Embedded[m, h] == glyphId) return (uint)(m * 10 + h);
            return 0;
        }

        public int BagCount { get { return Glyphs.Keys.Count(id => PosOf(id) == 0); } }

        public void ResetWishesIfNewDay()
        {
            int today = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            if (WishDay == today) return;
            WishDay = today;
            foreach (var w in Wishes.Skip(1)) w.Got = false;
        }

        // ------------------------------------------------------------------ EventSection <-> state
        public static DuneState Read(EventSection s)
        {
            var st = new DuneState
            {
                Yang = s.Counter("yang"), Yin = s.Counter("yin"), Switch = (int)s.Counter("sw"), Draws = s.Counter("draws"),
                Pity = s.Counter("pity"), SkillEnd = s.Counter("skill"), WishDay = (int)s.Counter("wday"),
                NextGlyph = (uint)Math.Max(1, s.Counter("gid"))
            };
            for (int m = 1; m <= DuneData.MeridianCount; m++)
            {
                st.Levels[m] = (int)s.Counter("m" + m);
                st.Holes[m] = (int)s.Counter("h" + m);
                for (int h = 1; h <= DuneData.HolesPerMeridian; h++) st.Embedded[m, h] = (uint)s.Counter("e" + m + h);
                foreach (var t in DuneData.AcupointTypes(m))
                {
                    int q = (int)s.Counter("a" + t);
                    if (q > 0) st.Acupoints[t] = q;
                }
            }
            for (int t = 1; t <= 3; t++) { st.TalentMarks[t] = s.Counter("tv" + t); st.TalentNodes[t] = (uint)s.Counter("tn" + t); }
            for (int w = 1; w <= 2; w++)
            {
                var p = s.Str("w" + w).Split(',');
                if (p.Length == 3) st.Wishes[w] = new Wish { Type = int.Parse(p[0]), TypeEx = int.Parse(p[1]), Got = p[2] == "1" };
            }
            foreach (var kv in s.Strings.Where(k => k.Key.Length > 1 && k.Key[0] == 'g'))
            {
                var p = kv.Value.Split(',');
                uint id;
                if (p.Length != 7 || !uint.TryParse(kv.Key.Substring(1), out id)) continue;
                st.Glyphs[id] = new GlyphItem
                {
                    Id = id, Type = int.Parse(p[0]), Life = int.Parse(p[1]), Attack = int.Parse(p[2]), Defence = int.Parse(p[3]),
                    MagicDefence = int.Parse(p[4]), AttrType = int.Parse(p[5]), AttrValue = int.Parse(p[6])
                };
            }
            return st;
        }

        public void Write(EventSection s)
        {
            Set(s, "yang", Yang); Set(s, "yin", Yin); Set(s, "sw", Switch); Set(s, "draws", Draws); Set(s, "pity", Pity);
            Set(s, "skill", SkillEnd); Set(s, "wday", WishDay); Set(s, "gid", NextGlyph);
            for (int m = 1; m <= DuneData.MeridianCount; m++)
            {
                Set(s, "m" + m, Levels[m]);
                Set(s, "h" + m, Holes[m]);
                for (int h = 1; h <= DuneData.HolesPerMeridian; h++) Set(s, "e" + m + h, Embedded[m, h]);
                foreach (var t in DuneData.AcupointTypes(m)) Set(s, "a" + t, Quality(t));
            }
            for (int t = 1; t <= 3; t++) { Set(s, "tv" + t, TalentMarks[t]); Set(s, "tn" + t, TalentNodes[t]); }
            for (int w = 1; w <= 2; w++)
            {
                var wish = Wishes[w];
                if (wish.Type == 0) s.Strings.Remove("w" + w);
                else s.Strings["w" + w] = wish.Type + "," + wish.TypeEx + "," + (wish.Got ? 1 : 0);
            }
            foreach (var key in s.Strings.Keys.Where(k => k.Length > 1 && k[0] == 'g').ToList())
            {
                uint id;
                if (!uint.TryParse(key.Substring(1), out id) || !Glyphs.ContainsKey(id)) s.Strings.Remove(key);
            }
            foreach (var g in Glyphs.Values)
                s.Strings["g" + g.Id] = string.Join(",", g.Type, g.Life, g.Attack, g.Defence, g.MagicDefence, g.AttrType, g.AttrValue);
        }

        /// <summary>Zero values are not stored (no row for an untouched key).</summary>
        static void Set(EventSection s, string key, long v)
        {
            if (v == 0) s.Counters.Remove(key);
            else s.Counters[key] = v;
        }
    }

    /// <summary>QahrMobile: the only door to DuneState (read a copy, or change it atomically and drop the stat cache).</summary>
    public static class DuneStore
    {
        const string Ev = "dune";

        public static DuneState Read(uint uid)
        {
            return DuneState.Read(EventStore.Section(uid, Ev));
        }

        /// <summary>fn changes the state and returns its result; flushNow = in MySQL before it returns (use it when
        /// items or CPs change hands). Throws EventStore.EditFailed when MySQL is down (nothing is changed then).</summary>
        public static T Edit<T>(uint uid, Func<DuneState, T> fn, bool flushNow)
        {
            var res = EventStore.Edit(uid, Ev, s =>
            {
                var st = DuneState.Read(s);
                var r = fn(st);
                st.Write(s);
                return Tuple.Create(true, r);
            }, flushNow);
            DuneStats.Invalidate(uid);
            return res;
        }
    }
}
