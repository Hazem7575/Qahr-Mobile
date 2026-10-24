using System;
using System.Collections.Generic;
using Qahr.Mobile.Phone;

namespace Qahr.Mobile.DuneWanderer
{
    /// <summary>
    /// QahrMobile: CMsgMeridians (10438) requests of the Dune Wanderer window: open (8, 10), Cultivation upgrade (4),
    /// Yin/Yang switch (7), CPs -> points (9), glyph holes (11 open, 12 embed, 13 remove), the main skill's special effect
    /// (14 activate, 15 query) and another player's view (2). Rules from Data\DuneWanderer (DuneData); state in DuneStore.
    /// Called by DuneNet. Every cost is checked first and paid exactly once.
    /// </summary>
    public static class DuneMeridians
    {
        const int Ok = 1;

        public static void OnRequest(Client.GameClient c, MeridiansMsg r)
        {
            switch (r.Action)
            {
                case 8: Open(c); break;
                case 10: SendBreakPower(c, c.Player.UID, DuneStore.Read(c.Player.UID)); break;
                case 4: Upgrade(c, (int)r.Data); break;
                case 7: Switch(c); break;
                case 9: Exchange(c, r.Order, r.Data); break;
                case 11: OpenHole(c, (int)r.Data, (int)r.IdUser); break;
                case 12: Embed(c, r.Data, (int)r.IdUser, (int)r.Order); break;
                case 13: Remove(c, (int)r.IdUser, (int)r.Order); break;
                case 14: ActivateSkill(c); break;
                case 15: SendSkillTime(c, DuneStore.Read(c.Player.UID)); break;
                case 2: ViewOther(c, r.IdUser); break;
                default: MyConsole.WriteLine("[Dune] " + c.Player.Name + ": unknown meridians action " + r.Action, ConsoleColor.Yellow); break;
            }
        }

        static void Open(Client.GameClient c)
        {
            var st = DuneStore.Read(c.Player.UID);
            DuneNet.SendMeridians(c, st);
            DuneNet.SendPoints(c, st);
        }

        static void SendBreakPower(Client.GameClient c, uint uid, DuneState st)
        {
            var m = DuneNet.Meridians(c, 10);
            m.IdUser = uid;
            m.Data = (uint)DuneStats.BreakPower(st);
            DuneNet.Send(c, DunePackets.Meridians, m);
        }

        static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }

        static void SendSkillTime(Client.GameClient c, DuneState st)
        {
            var m = DuneNet.Meridians(c, 15);
            m.Data = st.SkillEnd > Now ? (uint)st.SkillEnd : 0;
            DuneNet.Send(c, DunePackets.Meridians, m);
        }

        // ------------------------------------------------------------------ Cultivation
        static void Upgrade(Client.GameClient c, int meridian)
        {
            if (meridian < 1 || meridian > DuneData.MeridianCount) return;
            string why = null;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                var next = DuneData.Meridian(meridian, s.Levels[meridian] + 1);
                if (next == null) why = "This meridian is at its highest level.";
                else if (meridian != 1 && s.Levels[1] < next.MainNeeded) why = "Raise the Conception-Vessel to level " + next.MainNeeded + " first.";
                else if (s.Yang < next.Yang || s.Yin < next.Yin) why = "Not enough Yang/Yin points.";
                if (why != null) return s;
                s.Yang -= next.Yang;
                s.Yin -= next.Yin;
                s.Levels[meridian]++;
                return s;
            }, true);
            if (why != null) { PlayerMsg.Tell(c, why); return; }
            var m = DuneNet.Meridians(c, 4);
            m.Ret = Ok;
            m.Data = (uint)meridian;
            m.Meridians.Add(DuneNet.MeridianOf(st, meridian));
            DuneNet.Send(c, DunePackets.Meridians, m);
            DuneNet.SendPoints(c, st);
            DuneStats.Refresh(c);
        }

        static void Exchange(Client.GameClient c, uint type, uint amount)
        {
            if (amount == 0 || amount > 1000000 || (type != 1 && type != 2)) return;
            long cost = amount * DuneData.Cfg.ExchangeCps;
            if (c.Player.ConquerPoints < cost) { PlayerMsg.Tell(c, "You need " + cost + " CPs."); return; }
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                if (type == 1) s.Yang += amount; else s.Yin += amount;
                return s;
            }, true);
            c.Player.ConquerPoints -= (uint)cost;
            MyConsole.WriteLine("[Dune] " + c.Player.Name + " bought " + amount + (type == 1 ? " Yang" : " Yin") + " points for " + cost + " CPs", ConsoleColor.Cyan);
            var m = DuneNet.Meridians(c, 9);
            m.Ret = Ok;
            DuneNet.Send(c, DunePackets.Meridians, m);
            DuneNet.SendPoints(c, st);
        }

        /// <summary>MobileItemUse: a Yang/Yin Amulet (config "amulet") adds its points. true = handled.</summary>
        public static bool OnUseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            Tuple<bool, int> amulet;
            if (!DuneData.Cfg.Amulets.TryGetValue(item.ITEM_ID, out amulet)) return false;
            if (!Database.AtributesStatus.IsDune(c.Player.Class)) { PlayerMsg.Tell(c, "Only a Dune Wanderer can use this."); return true; }
            using (var rec = new ServerSockets.RecycledPacket())
                if (!c.Inventory.Remove(item.ITEM_ID, 1, rec.GetStream())) return true;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                if (amulet.Item1) s.Yang += amulet.Item2; else s.Yin += amulet.Item2;
                return s;
            }, true);
            PlayerMsg.Tell(c, "+" + amulet.Item2 + (amulet.Item1 ? " Yang" : " Yin") + " point(s).");
            DuneNet.SendPoints(c, st);
            return true;
        }

        // ------------------------------------------------------------------ Yin / Yang
        static void Switch(Client.GameClient c)
        {
            var cfg = DuneData.Cfg;
            if (c.Inventory.GetCountItem(cfg.SwitchItem) < cfg.SwitchAmount) { PlayerMsg.Tell(c, "You need " + cfg.SwitchAmount + " Yin-Yang Amulets."); return; }
            var st = DuneStore.Edit(c.Player.UID, s => { s.Switch = s.Switch == 0 ? 1 : 0; return s; }, true);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                if (!c.Inventory.Remove(cfg.SwitchItem, (uint)cfg.SwitchAmount, rec.GetStream()))
                {
                    DuneStore.Edit(c.Player.UID, s => { s.Switch = s.Switch == 0 ? 1 : 0; return s; }, true);// not paid: undo
                    return;
                }
            }
            var m = DuneNet.Meridians(c, 7);
            m.Ret = st.Switch;
            DuneNet.Send(c, DunePackets.Meridians, m);
        }

        // ------------------------------------------------------------------ glyph holes
        static bool ValidHole(int meridian, int hole)
        {
            return meridian >= 1 && meridian <= DuneData.MeridianCount && hole >= 1 && hole <= DuneData.HolesPerMeridian;
        }

        static void OpenHole(Client.GameClient c, int meridian, int hole)
        {
            if (!ValidHole(meridian, hole)) return;
            bool ok = false;
            DuneStore.Edit(c.Player.UID, s =>
            {
                int[] need;
                if (!DuneData.Cfg.Holes.TryGetValue(meridian, out need) || s.Connected(meridian) < need[hole - 1]) return s;
                s.Holes[meridian] |= 1 << (hole - 1);
                ok = true;
                return s;
            }, false);
            if (!ok) { PlayerMsg.Tell(c, "Connect more acupoints of this meridian first."); return; }
            var m = DuneNet.Meridians(c, 11);
            m.Ret = Ok;
            m.Data = (uint)meridian;
            m.IdUser = (uint)hole;
            DuneNet.Send(c, DunePackets.Meridians, m);
        }

        static void Embed(Client.GameClient c, uint glyphId, int meridian, int hole)
        {
            if (!ValidHole(meridian, hole)) { Restore(c); return; }
            var moved = new List<uint>();
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                DuneState.GlyphItem g;
                var type = s.Glyphs.TryGetValue(glyphId, out g) ? DuneData.Glyph(g.Type) : null;
                if (type == null || !type.InDunePool || (s.Holes[meridian] & (1 << (hole - 1))) == 0) return s;
                uint was = s.PosOf(glyphId);
                if (was != 0) s.Embedded[was / 10, was % 10] = 0;// moved from another hole
                if (s.Embedded[meridian, hole] != 0) moved.Add(s.Embedded[meridian, hole]);// the old one goes back to the bag
                s.Embedded[meridian, hole] = glyphId;
                moved.Add(glyphId);
                return s;
            }, false);
            if (moved.Count == 0) { PlayerMsg.Tell(c, "This glyph cannot go into that hole."); Restore(c); return; }
            var m = DuneNet.Meridians(c, 12);
            m.Ret = Ok;
            m.Data = glyphId;
            m.IdUser = (uint)meridian;
            m.Order = (uint)hole;
            DuneNet.Send(c, DunePackets.Meridians, m);
            DuneNet.SendGlyphChange(c, st, 2, moved.ConvertAll(id => st.Glyphs[id]));
            DuneStats.Refresh(c);
        }

        static void Remove(Client.GameClient c, int meridian, int hole)
        {
            if (!ValidHole(meridian, hole)) { Restore(c); return; }
            uint old = 0;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                old = s.Embedded[meridian, hole];
                s.Embedded[meridian, hole] = 0;
                return s;
            }, false);
            if (old == 0) { Restore(c); return; }
            var m = DuneNet.Meridians(c, 13);
            m.Ret = Ok;
            m.IdUser = (uint)meridian;
            m.Order = (uint)hole;
            DuneNet.Send(c, DunePackets.Meridians, m);
            DuneState.GlyphItem g;
            if (st.Glyphs.TryGetValue(old, out g)) DuneNet.SendGlyphChange(c, st, 2, new[] { g });
            DuneStats.Refresh(c);
        }

        /// <summary>The client changes holes before it asks: a refused change is undone by sending the real state.</summary>
        static void Restore(Client.GameClient c)
        {
            var st = DuneStore.Read(c.Player.UID);
            DuneNet.SendMeridians(c, st);
            DuneNet.SendGlyphBag(c, st);
        }

        // ------------------------------------------------------------------ main skill special effect
        static void ActivateSkill(Client.GameClient c)
        {
            string why = null;
            var st = DuneStore.Edit(c.Player.UID, s =>
            {
                DuneData.Skill skill;
                if (!DuneData.Cfg.Skills.TryGetValue(1, out skill)) { why = "Not available."; return s; }
                long cost = skill.Cost[Math.Min(s.SkillLevel(1), skill.Cost.Length - 1)];
                if (s.SkillEnd > Now) why = "The special effect is still active.";
                else if (s.Yang < cost || s.Yin < cost) why = "You need " + cost + " Yang and " + cost + " Yin points.";
                if (why != null) return s;
                s.Yang -= cost;
                s.Yin -= cost;
                s.SkillEnd = Now + DuneData.Cfg.SkillDays * 86400L;
                return s;
            }, true);
            if (why != null) { PlayerMsg.Tell(c, why); return; }
            var m = DuneNet.Meridians(c, 14);
            m.Data = (uint)st.SkillEnd;
            DuneNet.Send(c, DunePackets.Meridians, m);
            DuneNet.SendPoints(c, st);
        }

        // ------------------------------------------------------------------ another player
        static void ViewOther(Client.GameClient c, uint uid)
        {
            if (uid == 0 || uid == c.Player.UID) { Open(c); return; }
            var st = DuneStore.Read(uid);
            var m = DuneNet.Meridians(c, 3);
            m.IdUser = uid;
            m.Data = (uint)st.Switch;
            m.Order = 3;
            for (int i = 1; i <= DuneData.MeridianCount; i++) m.Meridians.Add(DuneNet.MeridianOf(st, i));
            DuneNet.Send(c, DunePackets.Meridians, m);
            var a = DuneNet.Meridians(c, 1);
            a.IdUser = uid;
            a.Order = 3;
            foreach (var kv in st.Acupoints) a.Acupoints.Add(new AcupointInfo { Type = kv.Key, Quality = kv.Value });
            DuneNet.Send(c, DunePackets.Meridians, a);
            SendBreakPower(c, uid, st);
        }
    }
}
