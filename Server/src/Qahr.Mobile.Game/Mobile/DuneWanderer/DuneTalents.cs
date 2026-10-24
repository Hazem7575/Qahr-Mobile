using System;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: the Talents tab of the Dune Wanderer window (the client's heraldry talent dialog, CMsgWenZhang 200-202).
    /// 3 trees of linear nodes (Data\DuneWanderer\talents.txt); a node costs talent marks of its tree (from decomposing
    /// glyphs) and silver. Called by DuneGlyphs for those actions.
    /// </summary>
    public static class DuneTalents
    {
        const int Trees = 3;

        /// <summary>Action 201 (tree, node): activate the next node of a tree.</summary>
        public static void Activate(Client.GameClient c, int tree, int node)
        {
            var t = tree >= 1 && tree <= Trees ? DuneData.Talent(tree, node) : null;
            if (t == null) return;
            if (c.Player.Money < (ulong)t.Silver) { PlayerMsg.Tell(c, "You need " + t.Silver + " silver."); return; }
            string why = null;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                uint bit = 1u << (node - 1);
                if ((s.TalentNodes[tree] & bit) != 0) why = "This talent is already active.";
                else if (t.Before > 0 && (s.TalentNodes[tree] & (1u << (t.Before - 1))) == 0) why = "Activate the talent before it first.";
                else if (s.TalentMarks[tree] < t.Marks) why = "You need " + t.Marks + " talent marks of this tree (decompose glyphs).";
                if (why != null) return s;
                s.TalentMarks[tree] -= t.Marks;
                s.TalentNodes[tree] |= bit;
                return s;
            }, true);
            if (why != null) { PlayerMsg.Tell(c, why); return; }
            c.Player.Money -= (ulong)t.Silver;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Player.SendUpdate(rec.GetStream(), c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
            var done = new GlyphMsg { Action = 201, IdUser = c.Player.UID, Ret = true };
            done.Talents.Add(new TalentInfo { Tree = (uint)tree, ActiveNodes = st.TalentNodes[tree], Points = (uint)st.TalentMarks[tree] });
            DuneNet.Send(c, DunePackets.Glyph, done);
            SendPoints(c, st, tree);
            DuneStats.Refresh(c);
        }

        /// <summary>Action 202: the talent marks of one tree.</summary>
        public static void SendPoints(Client.GameClient c, DuneState st, int tree)
        {
            var m = new GlyphMsg { Action = 202, IdUser = c.Player.UID };
            m.Talents.Add(new TalentInfo { Tree = (uint)tree, ActiveNodes = st.TalentNodes[tree], Points = (uint)Math.Min(st.TalentMarks[tree], uint.MaxValue) });
            DuneNet.Send(c, DunePackets.Glyph, m);
        }

        /// <summary>Action 200 for another player (the "view player" dialog asks for it).</summary>
        public static void SendOther(Client.GameClient c, uint uid)
        {
            var st = DuneStore.Read(uid);
            var m = new GlyphMsg { Action = 200, IdUser = uid, Order = 2 };
            for (uint t = 1; t <= Trees; t++)
                m.Talents.Add(new TalentInfo { Tree = t, ActiveNodes = st.TalentNodes[t], Points = (uint)Math.Min(st.TalentMarks[t], uint.MaxValue) });
            DuneNet.Send(c, DunePackets.Glyph, m);
        }
    }
}
