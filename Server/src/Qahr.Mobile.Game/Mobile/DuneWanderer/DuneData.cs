using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: the Dune Wanderer tables, the only reader of Data\DuneWanderer\*.txt (each file re-read when saved).
    /// meridians/acupoints/talents are copies of the client's tables (both sides must agree); config, insight weights,
    /// glyph sets and extra attributes are ours. Used by every Dune* class.
    /// </summary>
    public static class DuneData
    {
        public const int MeridianCount = 5, AcupointsPerMeridian = 6, HolesPerMeridian = 3;

        public sealed class Attr { public int Type; public int Value; }

        public sealed class MeridianLevel
        {
            public int Meridian, Level, Kind, MainNeeded, Score;
            public long Yang, Yin;
            public List<Attr> Attrs;
        }

        public sealed class Acupoint
        {
            public int Type, Quality, Kind, Score, BreakPower;
            public long Yang, Yin;
            public List<Attr> Attrs;
            public bool IsMajor { get { return Kind == 2; } }
            public int Meridian { get { return Type / 100; } }
        }

        public sealed class Skill { public int Group, MobileSkill; public int[] Needed; public long[] Cost; }

        public sealed class Config
        {
            public readonly Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();
            public readonly Dictionary<int, int[]> Holes = new Dictionary<int, int[]>();
            public readonly Dictionary<int, int> Boost = new Dictionary<int, int>();
            public readonly Dictionary<uint, Tuple<bool, int>> Amulets = new Dictionary<uint, Tuple<bool, int>>();// item -> (yang, points)
            public uint SwitchItem, DrawItem;
            public int SwitchAmount, WishUnlock = 200, WishGuarantee = 70, SkillDays = 7, GlyphBag = 200;
            public long ExchangeCps = 10;
        }

        public sealed class PoolRow { public int Pool, RewardId, Type, Subtype, Amount; }

        public sealed class InsightRules
        {
            public readonly List<PoolRow> Rows = new List<PoolRow>();
            public readonly Dictionary<string, int> Kinds = new Dictionary<string, int>();
            public readonly Dictionary<int, int> Qualities = new Dictionary<int, int>();
            public readonly Dictionary<int, int> Points = new Dictionary<int, int>();
            public readonly Dictionary<int, int> GlyphQualities = new Dictionary<int, int>();
            public int MajorFactor = 50, StateFactor = 150;
        }

        public sealed class GlyphType
        {
            public int Type, Suit, Score, Life, Attack, Defence, MagicDefence, PoolMask, TalentMask, TalentMarks;
            public HashSet<int> SetClasses = new HashSet<int>();
            public int Quality { get { return Type % 10; } }
            public bool InDunePool { get { return (PoolMask & (1 << 4 | 1 << 5)) != 0; } }
        }

        /// <summary>One set effect: Attr = stat attr, or GlyphAttr = a base attribute of the embedded glyphs (+Value %).</summary>
        public sealed class SetEffect { public int Attr, GlyphAttr, Value; }

        public sealed class GlyphRules
        {
            public readonly Dictionary<int, GlyphType> Types = new Dictionary<int, GlyphType>();
            public readonly Dictionary<int, Dictionary<int, List<SetEffect>>> Sets = new Dictionary<int, Dictionary<int, List<SetEffect>>>();
            public readonly List<Tuple<Attr, int>> Extras = new List<Tuple<Attr, int>>();
            public readonly Dictionary<int, int> CompoundNeeded = new Dictionary<int, int>();
        }

        public sealed class TalentNode { public int Tree, Node, Before, Marks; public long Silver; public List<Attr> Attrs; }

        // ------------------------------------------------------------------ files
        const string Dir = "DuneWanderer\\";
        static readonly DataFile<Dictionary<string, MeridianLevel>> MeridianFile = new DataFile<Dictionary<string, MeridianLevel>>(Dir + "meridians.txt", "Dune", ParseMeridians);
        static readonly DataFile<Dictionary<string, Acupoint>> AcupointFile = new DataFile<Dictionary<string, Acupoint>>(Dir + "acupoints.txt", "Dune", ParseAcupoints);
        static readonly DataFile<Config> ConfigFile = new DataFile<Config>(Dir + "config.txt", "Dune", ParseConfig);
        static readonly DataFile<InsightRules> InsightFile = new DataFile<InsightRules>(Dir + "insight.txt", "Dune", ParseInsight);
        static readonly DataFile<GlyphRules> GlyphFile = new DataFile<GlyphRules>(Dir + "glyphs.txt", "Dune", ParseGlyphs);
        static readonly DataFile<Dictionary<string, TalentNode>> TalentFile = new DataFile<Dictionary<string, TalentNode>>(Dir + "talents.txt", "Dune", ParseTalents);

        public static Config Cfg { get { return ConfigFile.Value; } }
        public static InsightRules Insight { get { return InsightFile.Value; } }
        public static GlyphRules Glyphs { get { return GlyphFile.Value; } }

        public static MeridianLevel Meridian(int meridian, int level)
        {
            MeridianLevel m;
            return MeridianFile.Value.TryGetValue(meridian + ":" + level, out m) ? m : null;
        }

        public static Acupoint AcupointAt(int type, int quality)
        {
            Acupoint a;
            return AcupointFile.Value.TryGetValue(type + ":" + quality, out a) ? a : null;
        }

        public static IEnumerable<int> AcupointTypes(int meridian)
        {
            for (int i = 1; i <= AcupointsPerMeridian; i++) yield return meridian * 100 + i;
        }

        public static GlyphType Glyph(int type)
        {
            GlyphType g;
            return Glyphs.Types.TryGetValue(type, out g) ? g : null;
        }

        public static TalentNode Talent(int tree, int node)
        {
            TalentNode t;
            return TalentFile.Value.TryGetValue(tree + ":" + node, out t) ? t : null;
        }

        // ------------------------------------------------------------------ parsers (a bad line is logged and skipped)
        static List<Attr> ParseAttrs(string[] p, int from)
        {
            var list = new List<Attr>();
            for (int i = from; i < p.Length; i++)
            {
                var kv = p[i].Split(':');
                if (kv.Length == 2) list.Add(new Attr { Type = int.Parse(kv[0]), Value = int.Parse(kv[1]) });
            }
            return list;
        }

        static void Each(string[] lines, string file, Action<string[]> fn)
        {
            foreach (var line in DataFile.Clean(lines))
            {
                try { fn(DataFile.Words(line)); }
                catch (Exception e) { MyConsole.WriteLine("[Dune] " + file + ": bad line '" + line + "': " + e.Message, ConsoleColor.Red); }
            }
        }

        static Dictionary<string, MeridianLevel> ParseMeridians(string[] lines)
        {
            var d = new Dictionary<string, MeridianLevel>();
            Each(lines, "meridians.txt", p =>
            {
                var m = new MeridianLevel
                {
                    Meridian = int.Parse(p[0]), Level = int.Parse(p[1]), Kind = int.Parse(p[2]), MainNeeded = int.Parse(p[3]),
                    Yang = long.Parse(p[4]), Yin = long.Parse(p[5]), Score = int.Parse(p[6]), Attrs = ParseAttrs(p, 7)
                };
                d[m.Meridian + ":" + m.Level] = m;
            });
            return d;
        }

        static Dictionary<string, Acupoint> ParseAcupoints(string[] lines)
        {
            var d = new Dictionary<string, Acupoint>();
            Each(lines, "acupoints.txt", p =>
            {
                var a = new Acupoint
                {
                    Type = int.Parse(p[0]), Quality = int.Parse(p[1]), Kind = int.Parse(p[2]), Yang = long.Parse(p[3]),
                    Yin = long.Parse(p[4]), Score = int.Parse(p[5]), BreakPower = int.Parse(p[6]), Attrs = ParseAttrs(p, 7)
                };
                d[a.Type + ":" + a.Quality] = a;
            });
            return d;
        }

        static Config ParseConfig(string[] lines)
        {
            var c = new Config();
            Each(lines, "config.txt", p =>
            {
                switch (p[0])
                {
                    case "skill":
                        c.Skills[int.Parse(p[1])] = new Skill
                        {
                            Group = int.Parse(p[1]), MobileSkill = int.Parse(p[2]),
                            Needed = p.Skip(3).Take(4).Select(int.Parse).ToArray(), Cost = p.Skip(7).Take(4).Select(long.Parse).ToArray()
                        };
                        break;
                    case "hole": c.Holes[int.Parse(p[1])] = p.Skip(2).Take(HolesPerMeridian).Select(int.Parse).ToArray(); break;
                    case "boost": c.Boost[int.Parse(p[1])] = int.Parse(p[2]); break;
                    case "switch": c.SwitchItem = uint.Parse(p[1]); c.SwitchAmount = int.Parse(p[2]); break;
                    case "exchange": c.ExchangeCps = long.Parse(p[1]); break;
                    case "draw": c.DrawItem = uint.Parse(p[1]); c.WishUnlock = int.Parse(p[2]); c.WishGuarantee = int.Parse(p[3]); break;
                    case "skilldays": c.SkillDays = int.Parse(p[1]); break;
                    case "glyphbag": c.GlyphBag = int.Parse(p[1]); break;
                    case "amulet": c.Amulets[uint.Parse(p[1])] = Tuple.Create(p[2] == "yang", int.Parse(p[3])); break;
                    default: throw new FormatException("unknown key " + p[0]);
                }
            });
            return c;
        }

        static InsightRules ParseInsight(string[] lines)
        {
            var r = new InsightRules();
            Each(lines, "insight.txt", p =>
            {
                switch (p[0])
                {
                    case "row":
                        r.Rows.Add(new PoolRow { Pool = int.Parse(p[1]), RewardId = int.Parse(p[2]), Type = int.Parse(p[3]), Subtype = int.Parse(p[4]), Amount = int.Parse(p[5]) });
                        break;
                    case "kind": r.Kinds[p[1]] = int.Parse(p[2]); break;
                    case "quality": r.Qualities[int.Parse(p[1])] = int.Parse(p[2]); break;
                    case "major": r.MajorFactor = int.Parse(p[1]); break;
                    case "state": r.StateFactor = int.Parse(p[1]); break;
                    case "points": r.Points[int.Parse(p[1])] = int.Parse(p[2]); break;
                    case "glyphquality": r.GlyphQualities[int.Parse(p[1])] = int.Parse(p[2]); break;
                    default: throw new FormatException("unknown key " + p[0]);
                }
            });
            return r;
        }

        static GlyphRules ParseGlyphs(string[] lines)
        {
            var g = new GlyphRules();
            Each(lines, "glyphs.txt", p =>
            {
                switch (p[0])
                {
                    case "glyph":
                        var t = new GlyphType
                        {
                            Type = int.Parse(p[1]), Suit = int.Parse(p[2]), Score = int.Parse(p[3]), Life = int.Parse(p[4]), Attack = int.Parse(p[5]),
                            Defence = int.Parse(p[6]), MagicDefence = int.Parse(p[7]), PoolMask = int.Parse(p[8]), TalentMask = int.Parse(p[9]),
                            TalentMarks = int.Parse(p[10])
                        };
                        if (p.Length > 11)
                            foreach (var s in p[11].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) t.SetClasses.Add(int.Parse(s));
                        t.SetClasses.Remove(0);
                        g.Types[t.Type] = t;
                        break;
                    case "set":
                        var bySize = new Dictionary<int, List<SetEffect>>();
                        foreach (var spec in p.Skip(2))
                        {
                            int colon = spec.IndexOf(':');
                            var effects = new List<SetEffect>();
                            foreach (var e in spec.Substring(colon + 1).Split(','))
                            {
                                var kv = e.Split(':');
                                bool glyphAttr = kv[0].StartsWith("g");
                                int attr = int.Parse(glyphAttr ? kv[0].Substring(1) : kv[0]);
                                effects.Add(new SetEffect { Attr = glyphAttr ? 0 : attr, GlyphAttr = glyphAttr ? attr : 0, Value = int.Parse(kv[1]) });
                            }
                            bySize[int.Parse(spec.Substring(0, colon))] = effects;
                        }
                        g.Sets[int.Parse(p[1])] = bySize;
                        break;
                    case "extra": g.Extras.Add(Tuple.Create(new Attr { Type = int.Parse(p[1]), Value = int.Parse(p[2]) }, int.Parse(p[3]))); break;
                    case "compound": g.CompoundNeeded[int.Parse(p[1])] = int.Parse(p[2]); break;
                    default: throw new FormatException("unknown key " + p[0]);
                }
            });
            return g;
        }

        static Dictionary<string, TalentNode> ParseTalents(string[] lines)
        {
            var d = new Dictionary<string, TalentNode>();
            Each(lines, "talents.txt", p =>
            {
                var t = new TalentNode { Tree = int.Parse(p[0]), Node = int.Parse(p[1]), Before = int.Parse(p[2]), Marks = int.Parse(p[3]), Silver = long.Parse(p[8]) };
                t.Attrs = new List<Attr>();
                if (int.Parse(p[4]) > 0) t.Attrs.Add(new Attr { Type = int.Parse(p[4]), Value = int.Parse(p[5]) });
                if (int.Parse(p[6]) > 0) t.Attrs.Add(new Attr { Type = int.Parse(p[6]), Value = int.Parse(p[7]) });
                d[t.Tree + ":" + t.Node] = t;
            });
            return d;
        }
    }
}
