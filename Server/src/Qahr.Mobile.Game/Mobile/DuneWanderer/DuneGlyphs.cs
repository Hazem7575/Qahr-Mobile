using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: CMsgWenZhang (10395) of a Dune Wanderer: the glyph bag (0: asked at every login, the trigger of the
    /// full Dune state push; again after a decompose), decompose (6 -> talent marks + Glyph Decompose Boxes), compound
    /// (9: glyphs of one quality -> the next) and the talent actions (200-201, DuneTalents). The bag is the client's shared
    /// heraldry bag. Glyph types and rules: Data\DuneWanderer\glyphs.txt. Called by DuneNet.
    /// </summary>
    public static class DuneGlyphs
    {
        const uint DecomposeBox = 3353139;// wenzhang_type.resolve: +1 per Divine glyph, +1 per 10 Super glyphs
        const int DuneClassSort = 3;

        sealed class Session { public bool Sent; }
        static readonly ConditionalWeakTable<Client.GameClient, Session> sessions = new ConditionalWeakTable<Client.GameClient, Session>();

        public static void OnRequest(Client.GameClient c, GlyphMsg r)
        {
            switch (r.Action)
            {
                case 0: OnBagQuery(c); break;
                case 7: DuneNet.Send(c, DunePackets.Glyph, new GlyphMsg { Action = 7, IdUser = 0 }); break;// recycle count (Trojan only)
                case 6: Decompose(c, r.Glyphs.Select(g => g.Id).ToList()); break;
                case 9: Compound(c, (int)r.IdUser, (int)r.Order, r.Glyphs.Select(g => g.Id).ToList()); break;
                case 200: DuneTalents.SendOther(c, r.IdUser == 0 ? c.Player.UID : r.IdUser); break;
                case 201:
                    var t = r.Talents.FirstOrDefault();
                    if (t != null) DuneTalents.Activate(c, (int)t.Tree, (int)t.ActiveNodes);// node index (1-based)
                    break;
                default: MyConsole.WriteLine("[Dune] " + c.Player.Name + ": unknown glyph action " + r.Action, ConsoleColor.Yellow); break;
            }
        }

        /// <summary>First query of the session (login) = the whole Dune state; later ones = the bag only.</summary>
        static void OnBagQuery(Client.GameClient c)
        {
            var s = sessions.GetOrCreateValue(c);
            if (!s.Sent) { s.Sent = true; DuneNet.SendAll(c); }
            else DuneNet.SendGlyphBag(c, DuneStore.Read(c.Player.UID));
        }

        // ------------------------------------------------------------------ decompose
        /// <summary>Bag glyphs (not embedded, not Dune-exclusive) -> talent marks of the trees in their talent mask and
        /// Glyph Decompose Boxes. The client shows the tips itself and then asks for the bag again (action 0).</summary>
        static void Decompose(Client.GameClient c, List<uint> ids)
        {
            var gained = new long[4];
            int divine = 0, super = 0, count = 0;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                foreach (var id in ids.Distinct())
                {
                    DuneState.GlyphItem g;
                    if (!s.Glyphs.TryGetValue(id, out g) || s.PosOf(id) != 0) continue;
                    var type = DuneData.Glyph(g.Type);
                    if (type != null && type.SetClasses.Count == 1 && type.SetClasses.Contains(DuneClassSort)) continue;// awakening-exclusive
                    s.Glyphs.Remove(id);
                    count++;
                    if (g.Type % 10 == 3) divine++; else if (g.Type % 10 == 2) super++;
                    if (type == null) continue;
                    for (int tree = 1; tree <= 3; tree++)
                        if ((type.TalentMask & (1 << (tree - 1))) != 0) { s.TalentMarks[tree] += type.TalentMarks; gained[tree] += type.TalentMarks; }
                }
                return s;
            }, true);
            DuneNet.Send(c, DunePackets.Glyph, new GlyphMsg { Action = 6, IdUser = c.Player.UID });
            if (count == 0) return;
            for (int tree = 1; tree <= 3; tree++)
                if (gained[tree] > 0) DuneTalents.SendPoints(c, st, tree);
            int boxes = divine + super / 10;
            if (boxes > 0)
                using (var rec = new ServerSockets.RecycledPacket())
                    Rewards.Give(c, rec.GetStream(), new List<RewardItem> { new RewardItem { Id = DecomposeBox, Count = (ushort)boxes } }, "DuneGlyphs");
            MyConsole.WriteLine("[Dune] " + c.Player.Name + " decomposed " + count + " glyph(s), marks +" + gained[1] + "/+" + gained[2] + "/+" + gained[3] + ", boxes " + boxes, ConsoleColor.Cyan);
        }

        // ------------------------------------------------------------------ compound
        /// <summary>Request: quality = the target quality, count = glyphs to make, ids = count x "compound" bag glyphs of
        /// quality - 1. The more of one suit is used, the likelier that suit (only suits that exist one quality up in the
        /// Dune pool). The client does not remove the materials: action 8 does, before the action 9 result.</summary>
        static void Compound(Client.GameClient c, int quality, int count, List<uint> ids)
        {
            var made = new List<DuneState.GlyphItem>();
            var used = new List<DuneState.GlyphItem>();
            string why = null;
            int need;
            if (!DuneData.Glyphs.CompoundNeeded.TryGetValue(quality, out need) || count < 1) return;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                var glyphs = ids.Distinct().Select(id => { DuneState.GlyphItem g; return s.Glyphs.TryGetValue(id, out g) && s.PosOf(id) == 0 ? g : null; })
                    .Where(g => g != null && g.Type % 10 == quality - 1).ToList();
                var targets = DuneData.Glyphs.Types.Values.Where(t => t.InDunePool && t.Quality == quality).ToList();
                if (glyphs.Count < count * need) { why = "Compound needs " + need + " glyphs of the quality below for each new glyph."; return s; }
                if (targets.Count == 0) { why = "These glyphs cannot be compounded."; return s; }
                for (int b = 0; b < count; b++)
                {
                    var batch = glyphs.Skip(b * need).Take(need).ToList();
                    var weights = targets.Select(t => new KeyValuePair<int, int>(t.Type, 1 + 10 * batch.Count(g => g.Type / 100 == t.Suit))).ToList();
                    foreach (var g in batch) { s.Glyphs.Remove(g.Id); used.Add(g); }
                    made.Add(DuneInsight.NewGlyph(s, DuneInsight.Pick(weights)));
                }
                return s;
            }, true);
            if (why != null) { PlayerMsg.Tell(c, why); DuneNet.SendGlyphBag(c, st); return; }
            DuneNet.SendGlyphChange(c, st, 8, used);
            var result = new GlyphMsg { Action = 9, IdUser = c.Player.UID, Order = 3, Ret = true };
            foreach (var g in made) result.Glyphs.Add(DuneNet.GlyphOf(st, g));
            DuneNet.Send(c, DunePackets.Glyph, result);
            MyConsole.WriteLine("[Dune] " + c.Player.Name + " compounded " + used.Count + " glyph(s) into " + made.Count, ConsoleColor.Cyan);
        }
    }
}
