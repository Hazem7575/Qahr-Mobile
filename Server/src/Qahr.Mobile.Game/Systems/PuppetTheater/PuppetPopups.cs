// ============================================================================
//  [PuppetTheater 2026-10-15, video pass] The official client windows the event uses for its popups, and the
//  reward systems behind the Halloween items:
//    MsgBox   fairygui/window/fuimsgbox (Fui_Common_MsgBox_Open): "Notice" box with Confirm / Cancel server callbacks
//             (video 04:22 encounter box, 4:46 story box, the loss box).
//    Reward   luaui dialog DlgPublicReward ("You received ... Click to go on.", video 01:12 / 01:54 / 06:18 / 07:08).
//    Tip      g_function Server_MsgTips -> LuaUIMgr_MsgTip (the upfloat over the board, video 01:10 / 04:12).
//    Bell     Octave Bell right-click (Server_Key tFragments[3354663][DebrisText], video 08:54-09:22): exchange 30 / 60 / 80 /
//             100 bells, "Use 1 bell", and after the event the CP top-up (600 CPs per missing bell) / 1000 Chi Points per bell.
//    Hairface 3D Puppet Avatar Pack (3-day avatar frame) / 7D Cute Emoji Pack (7-day emoji series) through the server
//             HairfaceStorage (Frame = 9, Emoji = 10), with expiry kept in the festival store and removed by the tick.
//    Class Awakening Box (official tip: choose 1 of 8).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.PuppetTheater
{
    public static class PtUi
    {
        static string Q(string s) { return "'" + (s ?? "").Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n") + "'"; }
        /// <summary>Common "Notice" box. Empty func = the button only closes.</summary>
        public static void MsgBox(Client.GameClient c, string text, string sureText, string sureFunc, string cancelText, string cancelFunc)
        {
            Kit.Lua(c, "Fui_Common_MsgBox_Open|{['Code']=200,['Data']={" + PT.LuaMsg + ",{'',{}},{" + Q(text) + ",{}},{" + Q(sureText ?? "") + ",{}},{" + Q(cancelText ?? "") + ",{}},{'',{}},{'',{}},{'',{}},-1,{'',{}},{"
                + Q(sureFunc ?? "") + ",{}},{" + Q(cancelFunc ?? "") + ",{}},{'',{}}}}", PT.LuaMsg);
        }
        /// <summary>"You received" popup. tipKey = Server_Key id (STR_ID_...@@param@@) or null; closeFunc = msg 10768 call on close.</summary>
        public static void Reward(Client.GameClient c, string tipKey, IEnumerable<Reward> items, string closeFunc = null)
        {
            Kit.Lua(c, "DlgPublicReward_OpenView|{['Code']=200,['Data']={" + (tipKey != null ? "['ShowTip']=" + Q(tipKey) + "," : "")
                + (closeFunc != null ? "['tCloseFunc']={['Id']=" + PT.LuaMsg + ",['Func']=" + Q(closeFunc) + ",['Param']=''}" : "") + "}}", PT.LuaMsg);
            Kit.Lua(c, "DlgPublicReward_UpdateView|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", (items ?? new Reward[0]).Select(x => Q(x.Item + " 0 " + x.Count))) + "},['IsFinish']=1}}", PT.LuaMsg);
        }
        /// <summary>Upfloat tip (keeps the official colour tags).</summary>
        public static void Tip(Client.GameClient c, string text) { if (!string.IsNullOrEmpty(text)) Kit.Lua(c, "Server_MsgTips|" + text, PT.LuaMsg); }
        public static string Names(IEnumerable<Reward> items) { return string.Join(", ", items.Select(x => x.Count + " " + Kit.Name(x.Item))); }
    }

    public static class PuppetEvents
    {
        /// <summary>Board event cell opened: the Notice box; Confirm pays (once) through Event().</summary>
        public static void Ask(Client.GameClient c, int L, int index, char kind, int extra, bool first)
        {
            uint uid = c.Player.UID;
            if (first) PT.St.Set(uid, "pe" + L, index);
            string tip, text;
            if (kind == 'C') { tip = "Exploration[UpfloatTips][107]"; text = "Exploration[ClientTips][Box][1][" + L + "]"; }
            else if (kind == 'A') { tip = "Exploration[UpfloatTips][108]"; text = "Exploration[ClientTips][Artifact][1][" + extra + "]"; }
            else { tip = "Exploration[UpfloatTips][106]"; text = "Exploration[ClientTips][Special][1][" + extra + "]"; }
            string f = "Halloween2025Exploration_Event</N>" + L;
            PtUi.MsgBox(c, PT.Plain(PT.T(tip)) + "\n\n" + PT.T(text), "", f, "", f);
        }
        /// <summary>Halloween2025Exploration_Event</N>L: the pending reward of the event cell (idempotent: cleared before paying).</summary>
        public static string Event(Client.GameClient c, int L)
        {
            uint uid = c.Player.UID; var l = PuppetMaze.Lvl(L); var cfg = PT.Cfg;
            int idx = (int)PT.Get(uid, "pe" + L);
            if (l == null || idx < 1 || idx > 64) return "none";
            int i = idx - 1; char k = l.Cells[i];
            Reward[] r;
            if (k == 'C') r = new[] { new Reward(PT.Soulthread, (uint)Math.Max(1, cfg.ChestSoul)) };
            else if (k == 'A') r = new[] { new Reward(PT.Soulthread, (uint)Math.Max(1, cfg.ArtifactSoul)) };
            else
            {
                var pool = cfg.EncounterReward.Where(x => x.Length >= 2).ToList();
                var row = pool.Count > 0 ? pool[Kit.Next(0, pool.Count - 1)] : new uint[] { 3009100, 2 };
                r = new[] { new Reward(row[0], row[1], true) };
            }
            if (!PT.Space(c, r)) { PtUi.Tip(c, "Please make room in your inventory first."); return "space"; }
            PT.St.Set(uid, "pe" + L, 0);
            using (var rec = new ServerSockets.RecycledPacket()) PT.Give(c, rec.GetStream(), "maze L" + L + " event " + idx, r);
            string names = PtUi.Names(r);
            string key = k == 'C' ? "STR_ID_tHalloween2025Exploration_DelText[ClientTips][Box][2][" + L + "]@@" + names + "@@"
                       : k == 'A' ? "STR_ID_tHalloween2025Exploration_DelText[ClientTips][Artifact][2][" + l.Extra[i] + "]"
                       : "STR_ID_tHalloween2025Exploration_DelText[ClientTips][Special][2][" + l.Extra[i] + "]@@" + names + "@@";
            PtUi.Reward(c, key, r);
            PuppetTheaterUi.Refresh(c);
            return "paid " + idx;
        }

        /// <summary>Story cell: the official portrait story (FguiGuideInfo[10768]["25QQWS_ZGZ_n_1"] chain); when it closes
        /// (ExitFunc) the area's story quest box follows (Server_Key tPlotEditorPlotEvent, video 04:46).</summary>
        public static void StoryCell(Client.GameClient c, int L, int n)
        {
            Festivals.FguiGuide.Show(c, PT.LuaMsg, "25QQWS_ZGZ_" + n + "_1", PT.LuaMsg, "Halloween2025Exploration_StoryNext</N>" + L);
        }
        public static string StoryNext(Client.GameClient c, int L)
        {
            var l = PuppetMaze.Lvl(L); if (l == null || l.Story.Length == 0) return "nostory";
            uint q = FestivalStory.StoryEntry.NextOf(c, l.Story); if (q == 0) return "done";
            string t = PT.T("Plot[" + q + "][MsgBox3]");
            if (q != 9186 || t.Length == 0) t = PT.T("Plot[" + q + "][MsgBox1]", "");
            if (t.Length == 0) t = PT.Fmt("Continue the story: %s?", PuppetMaze.StoryName(c, L, true) ?? "");
            PtUi.MsgBox(c, t, "", "Halloween2025Exploration_StoryGo</N>" + L, "", "");
            return "ask " + q;
        }

        // ------------------------------------------------------------------ Octave Bell (official right-click dialog)
        static readonly uint[] BellCost = { 30, 60, 80, 100 };
        static Reward BellGood(int i)
        {
            switch (i) { case 0: return new Reward(3354682, 1, true); case 1: return new Reward(194155, 1, true); case 2: return new Reward(3354683, 1, false); default: return new Reward(4330005, 1, false); }
        }
        static bool Grace() { string d = DateTime.Now.ToString("MM-dd"); return string.CompareOrdinal(d, "11-17") >= 0 && string.CompareOrdinal(d, "12-01") <= 0; }
        public static bool BellDialog(Client.GameClient c, ServerSockets.Packet s)
        {
            uint bell = PT.Cfg.OctaveBell;
            bool grace = Grace();
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o >= 1 && o <= 4) BellExchange(cc, ss, o - 1, false);
                else if (o >= 11 && o <= 14) BellExchangeAsk(cc, ss, o - 11);
                else if (o == 5) BellUse(cc, ss);
                else if (o == 6) BellChi(cc, ss);
            });
            PT.Say(d, PT.T("Bell[DebrisText][Text111]") + PT.T("Bell[DebrisText][Text112]") + PT.T("Bell[DebrisText][Text113]") + PT.T("Bell[DebrisText][Text115]") + PT.T("Bell[DebrisText][Text114]"));
            if (grace) PT.Say(d, PT.Fmt(PT.T("Bell[DebrisText][Text411]"), "11/17 - 12/01", 1000));
            for (int i = 0; i < 4; i++) d.AddOption(PT.T("Bell[DebrisText][Synthesis][1][" + (i + 1) + "]"), (byte)(i + 1));
            if (grace) for (int i = 0; i < 4; i++) d.AddOption(PT.T("Bell[DebrisText][Synthesis][2][" + (i + 1) + "]"), (byte)(11 + i));
            d.AddOption(PT.T("Bell[DebrisText][Bet]", "Use 1 bell."), 5);
            if (grace) d.AddOption(PT.T("Bell[DebrisText][Overdue]"), 6);
            d.AddOption(PT.T("Bell[DebrisText][Option111]", "Not now."), 255).FinalizeDialog();
            return true;
        }
        static void BellExchangeAsk(Client.GameClient c, ServerSockets.Packet s, int i)
        {
            uint have = PT.Count(c, PT.Cfg.OctaveBell); uint missing = BellCost[i] > have ? BellCost[i] - have : 0;
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) => { if (o == 1) BellExchange(cc, ss, i, true); });
            PT.Say(d, PT.Fmt(PT.T("Bell[DebrisText][Text311]"), missing * 600));
            d.AddOption(PT.T("Bell[DebrisText][Option311]"), 1).AddOption(PT.T("Bell[DebrisText][Option312]"), 255).FinalizeDialog();
        }
        public static string BellExchange(Client.GameClient c, ServerSockets.Packet s, int i, bool payCps)
        {
            if (i < 0 || i > 3) return "bad";
            uint bell = PT.Cfg.OctaveBell, cost = BellCost[i], have = PT.Count(c, bell);
            uint missing = cost > have ? cost - have : 0;
            if (missing > 0 && (!payCps || !Grace())) { PtUi.Tip(c, PT.Fmt("You need %s Octave Bells.", cost)); return "bells"; }
            uint cps = missing * 600;
            if (c.Player.ConquerPoints < cps) { PtUi.Tip(c, "Sorry, you don`t have enough CPs."); return "cps"; }
            var g = BellGood(i);
            if (!Kit.HasSpace(c, 1)) { PtUi.Tip(c, "Please make room in your inventory first."); return "space"; }
            if (cost - missing > 0 && !Kit.Take(c, bell, cost - missing, s)) return "bells";
            if (cps > 0) c.Player.ConquerPoints -= cps;
            if (g.Item / 1000 == 194) c.Inventory.Add(s, g.Item, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, g.Bound);
            else PT.Give(c, s, "bell exchange", g);
            PT.Log(c, "BELL", "exchange " + (cost - missing) + " bells + " + cps + " CPs -> " + g.Item);
            PtUi.Reward(c, null, new[] { g });
            return "exchanged " + g.Item;
        }
        static void BellUse(Client.GameClient c, ServerSockets.Packet s)
        {
            uint bell = PT.Cfg.OctaveBell;
            if (PT.Count(c, bell) < 1 || !Kit.HasSpace(c, 1)) { PtUi.Tip(c, "You need 1 Octave Bell and 1 free slot."); return; }
            if (!Kit.Take(c, bell, 1, s)) return;
            if (Kit.Next(1, 100) <= PT.Cfg.BellDouble) { var r = new Reward(bell, 2, true); PT.Give(c, s, "bell doubled", r); PtUi.Reward(c, null, new[] { r }); }
            else { PT.Log(c, "BELL", "used 1, not doubled"); PtUi.Tip(c, "The bell fades into silence."); }
        }
        static void BellChi(Client.GameClient c, ServerSockets.Packet s)
        {
            uint bell = PT.Cfg.OctaveBell, n = PT.Count(c, bell);
            if (n == 0 || c.Player.MyChi == null) { PtUi.Tip(c, "You have no Octave Bell."); return; }
            if (!Kit.Take(c, bell, n, s)) return;
            c.Player.MyChi.ChiPoints = (int)Math.Min(int.MaxValue, (long)c.Player.MyChi.ChiPoints + n * 1000L);
            try { MsgChiInfo.MsgHandleChi.SendInfo(c, MsgChiInfo.Action.Upgrade); } catch { }
            PT.Log(c, "BELL", n + " bells -> " + n * 1000 + " Chi Points");
            PtUi.Tip(c, "You received " + n * 1000 + " Chi Points.");
        }

        // ------------------------------------------------------------------ avatar frame / emoji (HairfaceStorage, timed)
        public static readonly uint[] PuppetFrames = { 6, 11, 5 };          // Nether Wolf / Fox Shadow / Moon Sleep (hairface_storage_type 9)
        public static readonly uint[] EmojiSeries = { 0, 3, 2, 4 };          // Wardrobe (no package) / Dragon Warrior 3 / Pirate 2 / Panda 4 (EmotionPackage.ini)
        public static string GrantHairface(Client.GameClient c, MsgHairfaceStorage.Type type, uint id, int days, string why)
        {
            if (c.HairfaceStorage == null) return "none";
            uint uid = c.Player.UID;
            string key = (int)type + ":" + id;
            var list = PT.St.Get(uid, "hf").Split(',').Where(x => x.Length > 0).ToList();
            var cur = list.FirstOrDefault(x => x.StartsWith(key + ":", StringComparison.Ordinal));
            bool owned = c.HairfaceStorage.Exists(id, type);
            if (owned && cur == null) return "permanent";                       // already owned for good: nothing to time
            long until = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + days * 86400L;
            if (days <= 0) { if (cur != null) { list.Remove(cur); PT.St.Set(uid, "hf", string.Join(",", list)); } }   // permanent (test restore): no expiry
            else
            {
                if (cur != null) { long old; long.TryParse(cur.Split(':')[2], out old); until = Math.Max(old, DateTimeOffset.UtcNow.ToUnixTimeSeconds()) + days * 86400L; list.Remove(cur); }
                list.Add(key + ":" + until);
                PT.St.Set(uid, "hf", string.Join(",", list));
            }
            if (!owned)
            {
                var tpl = Database.HairfaceStorageTable.Hairfaces.FirstOrDefault(h => h.ID == id && h.Type == type);
                var hf = new Database.HairfaceStorageTable.Hairface
                {
                    Type = type, ID = id, Name = tpl != null ? tpl.Name : why, RareLevel = tpl != null ? tpl.RareLevel : (byte)0, RequiredVIPLevel = 0, Cost = 0,
                    Classes = tpl != null && tpl.Classes != null ? new List<byte>(tpl.Classes) : new List<byte>(), Sex = tpl != null ? tpl.Sex : (byte)0,
                    Colors = new byte[7] { 1, 0, 0, 0, 0, 0, 0 }, Equiped = false, EquippedColor = 0
                };
                c.HairfaceStorage.Add(hf, true);
            }
            try { Database.ServerDatabase.SaveHairfaceStorage(c); } catch { }
            PT.Log(c, "GRANT", why + " -> hairface " + key + " until " + until);
            return "granted";
        }
        /// <summary>Per-player tick (PT.Tick): removes expired timed frames / emojis.</summary>
        public static void Expire(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            string raw = PT.St.Get(uid, "hf"); if (raw.Length == 0 || c.HairfaceStorage == null) return;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var list = raw.Split(',').Where(x => x.Length > 0).ToList(); bool changed = false;
            foreach (var e in list.ToArray())
            {
                var p = e.Split(':'); long until; int t; uint id;
                if (p.Length < 3 || !long.TryParse(p[2], out until) || !int.TryParse(p[0], out t) || !uint.TryParse(p[1], out id)) { list.Remove(e); changed = true; continue; }
                if (until > now) continue;
                c.HairfaceStorage.Remove(id, (MsgHairfaceStorage.Type)t);
                list.Remove(e); changed = true;
                PT.Log(c, "EXPIRE", "hairface " + t + ":" + id);
            }
            if (changed) { PT.St.Set(uid, "hf", string.Join(",", list)); try { Database.ServerDatabase.SaveHairfaceStorage(c); } catch { } }
        }

        // ------------------------------------------------------------------ item handlers added by the video pass
        public static bool UseItem(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s)
        {
            switch (item.ITEM_ID)
            {
                case 3354663: return BellDialog(c, s);
                case 3600815: return Consume(c, s, 3600815, () =>      // 3D Puppet Avatar Pack: a random 3-day avatar frame
                {
                    // a frame the player does not own yet (all owned: the pack is kept)
                    var cand = PuppetFrames.Where(f => c.HairfaceStorage == null || !c.HairfaceStorage.Exists(f, MsgHairfaceStorage.Type.Frame)).ToArray();
                    if (cand.Length == 0) cand = PuppetFrames;
                    uint id = cand[Kit.Next(0, cand.Length - 1)];
                    return GrantHairface(c, MsgHairfaceStorage.Type.Frame, id, 3, "3D Puppet Avatar Pack") == "granted" ? "You obtained a 3-day avatar frame." : null;
                });
                case 3349209: return EmojiChoice(c, s);
                case 3341018: return ClassBox(c, s);
                case 3346828: return Consume(c, s, 3346828, () => { c.Inventory.Add(s, 730002, 1, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 5 * 1440); return "You obtained 1 +2 Stone (B) (5 days)."; }, 1);
            }
            return false;
        }
        static bool Consume(Client.GameClient c, ServerSockets.Packet s, uint id, Func<string> grant, int slots = 0)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (slots > 0 && !Kit.HasSpace(c, slots)) { PtUi.Tip(c, "Please make room in your inventory first."); return true; }
                if (PT.Count(c, id) < 1 || !c.Inventory.Remove(id, 1, s)) return true;
                string msg = grant();
                if (msg == null) { PT.Give(c, s, "refund " + id, new Reward(id, 1)); PtUi.Tip(c, "You already own it."); return true; }
                PtUi.Tip(c, msg);
            }
            return true;
        }
        static bool EmojiChoice(Client.GameClient c, ServerSockets.Packet s)
        {
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o < 1 || o > 4) return;
                if (EmojiSeries[o - 1] == 0) { PtUi.Tip(cc, "Wardrobe Emoji has no emoji package on this client. Please choose another series."); return; }
                Consume(cc, ss, 3349209, () => GrantHairface(cc, MsgHairfaceStorage.Type.Emoji, EmojiSeries[o - 1], 7, "7D Cute Emoji Pack") == "granted" ? "You obtained a 7-day emoji series." : null);
            });
            PT.Say(d, "7D Cute Emoji Pack: choose 1 7-day emoji series.");
            d.AddOption("Wardrobe Emoji.", 1).AddOption("Dragon Warrior Series Emoji.", 2).AddOption("Pirate Series Emoji.", 3).AddOption("Panda Ponpon Series Emoji.", 4).AddOption("Later.", 255).FinalizeDialog();
            return true;
        }
        static readonly uint[][] ClassChoices = { new uint[] { 3316501, 1 }, new uint[] { 3321098, 1 }, new uint[] { 3333400, 2 }, new uint[] { 3335125, 1 }, new uint[] { 3328251, 1 }, new uint[] { 3329511, 1 }, new uint[] { 3346828, 1 }, new uint[] { 3339453, 1 } };
        static bool ClassBox(Client.GameClient c, ServerSockets.Packet s)
        {
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o < 1 || o > ClassChoices.Length) return;
                var r = new Reward(ClassChoices[o - 1][0], ClassChoices[o - 1][1], true);
                Consume(cc, ss, 3341018, () => "You obtained " + PT.Give(cc, ss, "open 3341018", r) + ".", Kit.Slots(new[] { r }));
            });
            PT.Say(d, "Class Awakening Box: choose a reward.");
            string[] n = { "1 Arcane Essence", "1 Legendary Essence", "2 Normal Soul Stones", "1 Gold Trigger", "1 Polar Pebble", "1 Still Zest", "1 +2 Stone (B)", "1 Dawn Orb" };
            for (int i = 0; i < n.Length; i++) d.AddOption(n[i] + ".", (byte)(i + 1));
            d.AddOption("Later.", 255).FinalizeDialog();
            return true;
        }
    }
}
