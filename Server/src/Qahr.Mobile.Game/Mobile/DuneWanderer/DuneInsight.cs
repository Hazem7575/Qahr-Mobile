using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: CMsgWenZhangLottery (10396) of the Dune Wanderer Insight tab: draw count (4), wishlist (2 query, 3 set)
    /// and the draw itself (0 = x1, 1 = x10, paid with the config "draw" item). Odds, the Yin/Yang side bonus and the glyph
    /// quality split are in Data\DuneWanderer\insight.txt; the reply names the client's own reward rows (pool 4/5).
    /// Wishes reset daily; after "draw" guarantee draws without a wished reward, the next draw gives one.
    /// </summary>
    public static class DuneInsight
    {
        const int WishAcupoint = 6, WishGlyph = 3, DivineQuality = 3;
        const int RowAcupoint = 6, RowPoints = 7, RowGlyph = 3;
        static readonly Random Rnd = new Random();

        public static void OnRequest(Client.GameClient c, InsightMsg r)
        {
            int pool = r.Pool == 5 ? 5 : 4;
            switch (r.Action)
            {
                case 4: Reply(c, new InsightMsg { Action = 4, Pool = pool, Data = (uint)DuneStore.Read(c.Player.UID).Draws }); break;
                case 2: SendWishes(c, pool); break;
                case 3: SetWish(c, r); break;
                case 0: Draw(c, pool, 1); break;
                case 1: Draw(c, pool, 10); break;
                default: MyConsole.WriteLine("[Dune] " + c.Player.Name + ": unknown insight action " + r.Action, ConsoleColor.Yellow); break;
            }
        }

        static void Reply(Client.GameClient c, InsightMsg m) { DuneNet.Send(c, DunePackets.Insight, m); }

        // ------------------------------------------------------------------ wishlist
        static void SendWishes(Client.GameClient c, int pool)
        {
            var st = DuneStore.Edit(c.Player.UID, s => { s.ResetWishesIfNewDay(); return s; }, false);
            var m = new InsightMsg { Action = 2, Pool = pool };
            for (int i = 1; i <= 2; i++)
                m.Wishes.Add(new WishInfo { Type = (uint)st.Wishes[i].Type, TypeEx = (uint)st.Wishes[i].TypeEx, Amount = st.Wishes[i].Got ? 1u : 0 });
            Reply(c, m);
        }

        static bool IsWishable(int type, int ex)
        {
            if (type == WishAcupoint) return ex >= 1 && ex <= DuneData.MeridianCount;
            var g = type == WishGlyph ? DuneData.Glyph(ex * 100 + DivineQuality) : null;
            return g != null && g.InDunePool;
        }

        static void SetWish(Client.GameClient c, InsightMsg r)
        {
            var w = r.Wishes.FirstOrDefault();
            if (w == null || w.Amount < 1 || w.Amount > 2 || !IsWishable((int)w.Type, (int)w.TypeEx)) return;
            string why = null;
            DuneStore.Edit(c.Player.UID, s =>
            {
                s.ResetWishesIfNewDay();
                var slot = s.Wishes[w.Amount];
                var other = s.Wishes[3 - w.Amount];
                if (s.Draws < DuneData.Cfg.WishUnlock) why = "The wishlist unlocks after " + DuneData.Cfg.WishUnlock + " Insights.";
                else if (slot.Got) why = "Today's wish in this slot was already obtained.";
                else if (other.Type == w.Type && other.TypeEx == w.TypeEx) why = "This is already in your other wish.";
                if (why != null) return s;
                slot.Type = (int)w.Type;
                slot.TypeEx = (int)w.TypeEx;
                return s;
            }, false);
            if (why != null) { PlayerMsg.Tell(c, why); return; }
            Reply(c, new InsightMsg { Action = 3, Pool = 4 });// the client asks for the list (2) itself
        }

        // ------------------------------------------------------------------ draw
        sealed class Outcome
        {
            public DrawResult Result = new DrawResult();
            public int Acupoint;// an acupoint the player gets / upgrades (0 = it became points)
            public int Drawn;// the acupoint drawn (wish check)
            public DuneState.GlyphItem Glyph;
        }

        static void Draw(Client.GameClient c, int pool, int count)
        {
            var cfg = DuneData.Cfg;
            if (c.Inventory.GetCountItem(cfg.DrawItem) < count) { PlayerMsg.Tell(c, "You need " + count + " Meridian Scroll(s)."); FailReply(c, pool, count); return; }
            if (cfg.GlyphBag - DuneStore.Read(c.Player.UID).BagCount < count) { PlayerMsg.Tell(c, "Your glyph bag is full."); FailReply(c, pool, count); return; }
            using (var rec = new ServerSockets.RecycledPacket())
                if (!c.Inventory.Remove(cfg.DrawItem, (uint)count, rec.GetStream())) { FailReply(c, pool, count); return; }
            List<Outcome> outcomes;
            DuneState st;
            try
            {
                outcomes = new List<Outcome>();
                st = DuneStore.Edit(c.Player.UID, s =>
                {
                    s.ResetWishesIfNewDay();
                    for (int i = 0; i < count; i++) outcomes.Add(DrawOne(s, pool));
                    return s;
                }, true);
            }
            catch (EventData.EventStore.EditFailed)
            {
                using (var rec = new ServerSockets.RecycledPacket())// not saved: give the scrolls back
                    Rewards.Give(c, rec.GetStream(), new List<RewardItem> { new RewardItem { Id = cfg.DrawItem, Count = (ushort)count } }, "DuneInsight");
                throw;
            }
            MyConsole.WriteLine("[Dune] " + c.Player.Name + " Insight x" + count + " (" + st.Draws + " draws)", ConsoleColor.Cyan);
            var acu = DuneNet.Meridians(c, 5);
            foreach (var o in outcomes.Where(o => o.Acupoint > 0)) acu.Acupoints.Add(new AcupointInfo { Type = o.Acupoint, Quality = st.Quality(o.Acupoint) });
            if (acu.Acupoints.Count > 0) DuneNet.Send(c, DunePackets.Meridians, acu);
            DuneNet.SendGlyphChange(c, st, 2, outcomes.Where(o => o.Glyph != null).Select(o => o.Glyph));
            DuneNet.SendPoints(c, st);
            var m = new InsightMsg { Action = count == 1 ? 0u : 1u, Data = 1, Pool = pool };
            uint left = (uint)Math.Max(0, DuneData.Cfg.WishGuarantee - st.Pity);
            foreach (var o in outcomes) { o.Result.GuaranteeNum = left; m.Results.Add(o.Result); }
            Reply(c, m);
            if (acu.Acupoints.Count > 0 || outcomes.Any(o => o.Glyph != null)) DuneStats.Refresh(c);
        }

        /// <summary>An empty result list = "draw failed" on the client (it re-enables the buttons).</summary>
        static void FailReply(Client.GameClient c, int pool, int count)
        {
            Reply(c, new InsightMsg { Action = count == 1 ? 0u : 1u, Data = 1, Pool = pool });
        }

        static Outcome DrawOne(DuneState s, int pool)
        {
            s.Draws++;
            bool wishing = s.Draws > DuneData.Cfg.WishUnlock && s.Wishes.Skip(1).Any(w => w.Type != 0 && !w.Got);
            if (wishing) s.Pity++;
            Outcome o;
            var forced = wishing && s.Pity >= DuneData.Cfg.WishGuarantee ? s.Wishes.Skip(1).First(w => w.Type != 0 && !w.Got) : null;
            if (forced != null) o = forced.Type == WishAcupoint ? Acupoint(s, pool, forced.TypeEx * 100 + 1, true) : Glyph(s, pool, forced.TypeEx * 100 + DivineQuality);
            else
            {
                string kind = Pick(DuneData.Insight.Kinds.ToList());
                o = kind == "points" ? Points(s, pool) : kind == "glyph" ? Glyph(s, pool, 0) : Acupoint(s, pool, 0, false);
            }
            foreach (var w in s.Wishes.Skip(1))
                if (w.Type != 0 && !w.Got && IsWished(w, o)) { w.Got = true; s.Pity = 0; }
            return o;
        }

        static bool IsWished(DuneState.Wish w, Outcome o)
        {
            if (w.Type == WishAcupoint) return o.Drawn == w.TypeEx * 100 + 1;
            return o.Glyph != null && o.Glyph.Type == w.TypeEx * 100 + DivineQuality;
        }

        static int StateSide(int meridian) { return meridian == 1 ? -1 : meridian <= 3 ? 0 : 1; }// main / Yang / Yin

        /// <summary>guaranteed = a wish: one quality above what the player has (when there is one).</summary>
        static Outcome Acupoint(DuneState s, int pool, int type, bool guaranteed)
        {
            var rules = DuneData.Insight;
            if (type == 0)
            {
                var types = new List<KeyValuePair<int, int>>();
                for (int m = 1; m <= DuneData.MeridianCount; m++)
                    foreach (var t in DuneData.AcupointTypes(m))
                    {
                        int w = 100;
                        if (t % 100 == 1) w = w * rules.MajorFactor / 100;
                        if (StateSide(m) == s.Switch) w = w * rules.StateFactor / 100;
                        types.Add(new KeyValuePair<int, int>(t, w));
                    }
                type = Pick(types);
            }
            int quality = Pick(rules.Qualities.ToList());
            if (guaranteed) quality = Math.Min(rules.Qualities.Keys.Max(), Math.Max(quality, s.Quality(type) + 1));
            var o = new Outcome { Drawn = type, Result = { RewardId = RowId(pool, RowAcupoint, type, quality) } };
            var acu = DuneData.AcupointAt(type, quality);
            if (s.Quality(type) < quality) { s.Acupoints[type] = quality; o.Acupoint = type; }
            else if (acu != null) { s.Yang += acu.Yang; s.Yin += acu.Yin; }// already owned at this quality or better: points
            return o;
        }

        static Outcome Points(DuneState s, int pool)
        {
            int amount = Pick(DuneData.Insight.Points.ToList());
            int sideW = DuneData.Insight.StateFactor;
            bool yang = Pick(new List<KeyValuePair<bool, int>>
            {
                new KeyValuePair<bool, int>(true, s.Switch == 0 ? sideW : 100),
                new KeyValuePair<bool, int>(false, s.Switch == 1 ? sideW : 100)
            });
            if (yang) s.Yang += amount; else s.Yin += amount;
            return new Outcome { Result = { RewardId = RowId(pool, RowPoints, yang ? 1 : 2, amount) } };
        }

        static Outcome Glyph(DuneState s, int pool, int type)
        {
            int poolBit = 1 << pool;
            if (type == 0)
            {
                var qualities = DuneData.Insight.GlyphQualities;
                var types = DuneData.Glyphs.Types.Values.Where(g => (g.PoolMask & poolBit) != 0 && qualities.ContainsKey(g.Quality))
                    .Select(g => new KeyValuePair<int, int>(g.Type, qualities[g.Quality])).ToList();
                if (types.Count == 0) return Points(s, pool);
                type = Pick(types);
            }
            var g2 = NewGlyph(s, type);
            var o = new Outcome { Glyph = g2, Result = { RewardId = RowId(pool, RowGlyph, 0, 0) } };
            o.Result.GlyphType = (uint)g2.Type; o.Result.Life = (uint)g2.Life; o.Result.Attack = (uint)g2.Attack;
            o.Result.Defence = (uint)g2.Defence; o.Result.MagicDefence = (uint)g2.MagicDefence;
            o.Result.AttrType = (uint)g2.AttrType; o.Result.AttrValue = (uint)g2.AttrValue;
            return o;
        }

        /// <summary>A new glyph in the bag: the type's base attributes + one random extra attribute (glyphs.txt "extra").</summary>
        public static DuneState.GlyphItem NewGlyph(DuneState s, int type)
        {
            var t = DuneData.Glyph(type);
            var g = new DuneState.GlyphItem { Id = s.NextGlyph++, Type = type };
            if (t != null) { g.Life = t.Life; g.Attack = t.Attack; g.Defence = t.Defence; g.MagicDefence = t.MagicDefence; }
            var extras = DuneData.Glyphs.Extras;
            if (extras.Count > 0)
            {
                var x = Pick(extras.Select(e => new KeyValuePair<DuneData.Attr, int>(e.Item1, e.Item2)).ToList());
                g.AttrType = x.Type;
                g.AttrValue = x.Value;
            }
            s.Glyphs[g.Id] = g;
            return g;
        }

        static uint RowId(int pool, int type, int subtype, int amount)
        {
            var row = DuneData.Insight.Rows.FirstOrDefault(r => r.Pool == pool && r.Type == type && r.Subtype == subtype && r.Amount == amount);
            return row != null ? (uint)row.RewardId : 0;
        }

        /// <summary>A weighted random pick (weights &lt;= 0 never win; all zero = the first).</summary>
        internal static T Pick<T>(List<KeyValuePair<T, int>> weights)
        {
            int total = weights.Sum(w => Math.Max(0, w.Value));
            if (total <= 0) return weights[0].Key;
            int roll;
            lock (Rnd) roll = Rnd.Next(total);
            foreach (var w in weights)
            {
                roll -= Math.Max(0, w.Value);
                if (roll < 0) return w.Key;
            }
            return weights[weights.Count - 1].Key;
        }
    }
}
