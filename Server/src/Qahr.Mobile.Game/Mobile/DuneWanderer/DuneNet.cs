using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: packet door of the Dune Wanderer window. Program.Game_Receive offers every packet (one line); the three
    /// Dune ids are decoded here and passed to DuneMeridians / DuneInsight / DuneGlyphs. Also the shared senders and the
    /// full state push (glyph bag, talents, meridians, acupoints) the client needs before the window works.
    /// </summary>
    public static class DuneNet
    {
        const int GlyphsPerPacket = 40;// a full bag (200) is split; the client clears on the first and refreshes on the last

        /// <summary>true = a Dune packet (handled, even when refused).</summary>
        public static bool TryHandle(Client.GameClient c, ServerSockets.Packet stream, ushort packetId)
        {
            if (packetId != DunePackets.Glyph && packetId != DunePackets.Insight && packetId != DunePackets.Meridians) return false;
            if (c == null || c.Player == null || !MobileClient.Is(c) || !Database.AtributesStatus.IsDune(c.Player.Class)) return true;
            try
            {
                lock (c)
                {
                    if (packetId == DunePackets.Meridians) DuneMeridians.OnRequest(c, stream.ProtoBufferDeserialize(new MeridiansMsg()));
                    else if (packetId == DunePackets.Insight) DuneInsight.OnRequest(c, stream.ProtoBufferDeserialize(new InsightMsg()));
                    else DuneGlyphs.OnRequest(c, stream.ProtoBufferDeserialize(new GlyphMsg()));
                }
            }
            catch (EventData.EventStore.EditFailed e)
            {
                MyConsole.WriteLine("[Dune] " + c.Player.Name + ": not saved (" + e.Message + "), nothing changed", ConsoleColor.Red);
                PlayerMsg.Tell(c, "The server could not save this right now. Nothing was changed, please try again.");
            }
            catch (Exception e) { MyConsole.WriteLine("[Dune] " + c.Player.Name + " packet " + packetId + ": " + e, ConsoleColor.Red); }
            return true;
        }

        // ------------------------------------------------------------------ senders
        public static void Send(Client.GameClient c, ushort id, object msg)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                s.InitWriter();
                s.ProtoBufferSerialize(msg);
                s.Finalize(id);
                c.Send(s);
            }
        }

        public static MeridiansMsg Meridians(Client.GameClient c, uint action)
        {
            return new MeridiansMsg { Action = action, IdUser = c.Player.UID };
        }

        public static MeridianInfo MeridianOf(DuneState st, int m)
        {
            return new MeridianInfo
            {
                Id = (uint)m, Level = st.Levels[m], OpenHoles = (uint)st.Holes[m],
                Glyph1 = st.Embedded[m, 1], Glyph2 = st.Embedded[m, 2], Glyph3 = st.Embedded[m, 3]
            };
        }

        /// <summary>Meridians action 0 (all 5, even level 0: the client refuses Insight without them) + 1 (acupoints).</summary>
        public static void SendMeridians(Client.GameClient c, DuneState st)
        {
            var m = Meridians(c, 0);
            m.Data = (uint)st.Switch;
            m.Order = 3;// clear + refresh
            for (int i = 1; i <= DuneData.MeridianCount; i++) m.Meridians.Add(MeridianOf(st, i));
            Send(c, DunePackets.Meridians, m);
            var a = Meridians(c, 1);
            a.Order = 3;
            foreach (var kv in st.Acupoints.OrderBy(k => k.Key)) a.Acupoints.Add(new AcupointInfo { Type = kv.Key, Quality = kv.Value });
            Send(c, DunePackets.Meridians, a);
        }

        /// <summary>Meridians action 8: Yang/Yin point totals (the client never subtracts them itself).</summary>
        public static void SendPoints(Client.GameClient c, DuneState st)
        {
            var m = Meridians(c, 8);
            m.Order = (uint)Math.Min(st.Yang, uint.MaxValue);
            m.Data = (uint)Math.Min(st.Yin, uint.MaxValue);
            Send(c, DunePackets.Meridians, m);
        }

        public static GlyphInfo GlyphOf(DuneState st, DuneState.GlyphItem g)
        {
            return new GlyphInfo
            {
                Id = g.Id, Type = (uint)g.Type, Life = (uint)g.Life, Attack = (uint)g.Attack, Defence = (uint)g.Defence,
                MagicDefence = (uint)g.MagicDefence, AttrType = (uint)g.AttrType, AttrValue = (uint)g.AttrValue, Pos = st.PosOf(g.Id)
            };
        }

        /// <summary>CMsgWenZhang action 0: the whole glyph bag (one packet with order 3 even when empty).</summary>
        public static void SendGlyphBag(Client.GameClient c, DuneState st)
        {
            var all = st.Glyphs.Values.OrderBy(g => g.Id).ToList();
            int chunks = Math.Max(1, (all.Count + GlyphsPerPacket - 1) / GlyphsPerPacket);
            for (int i = 0; i < chunks; i++)
            {
                var msg = new GlyphMsg { Action = 0, IdUser = c.Player.UID, Order = (uint)((i == 0 ? 1 : 0) | (i == chunks - 1 ? 2 : 0)) };
                foreach (var g in all.Skip(i * GlyphsPerPacket).Take(GlyphsPerPacket)) msg.Glyphs.Add(GlyphOf(st, g));
                Send(c, DunePackets.Glyph, msg);
            }
        }

        /// <summary>CMsgWenZhang action 2 (added) or 8 (removed) for a few glyphs.</summary>
        public static void SendGlyphChange(Client.GameClient c, DuneState st, uint action, IEnumerable<DuneState.GlyphItem> glyphs)
        {
            var msg = new GlyphMsg { Action = action, IdUser = c.Player.UID, Order = 3 };
            foreach (var g in glyphs) msg.Glyphs.Add(action == 8 ? new GlyphInfo { Id = g.Id } : GlyphOf(st, g));
            if (msg.Glyphs.Count > 0) Send(c, DunePackets.Glyph, msg);
        }

        /// <summary>CMsgWenZhang action 200: the 3 talent trees (the client never asks for its own).</summary>
        public static void SendTalents(Client.GameClient c, DuneState st)
        {
            var msg = new GlyphMsg { Action = 200, IdUser = c.Player.UID, Order = 2 };
            for (uint t = 1; t <= 3; t++)
                msg.Talents.Add(new TalentInfo { Tree = t, ActiveNodes = st.TalentNodes[t], Points = (uint)Math.Min(st.TalentMarks[t], uint.MaxValue) });
            Send(c, DunePackets.Glyph, msg);
        }

        /// <summary>Everything at login (the client's CMsgWenZhang action 0 query is the trigger).</summary>
        public static void SendAll(Client.GameClient c)
        {
            var st = DuneStore.Read(c.Player.UID);
            SendGlyphBag(c, st);
            SendTalents(c, st);
            SendMeridians(c, st);
            SendPoints(c, st);
        }
    }
}
