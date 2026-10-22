// ============================================================================
//  [PuppetTheater 2026-10-15] Puppet Home (training draw), Puppet Finale, the NPCs (Suyi, Elise's Puppet Store,
//  Strange Spectator, Broken Puppet's Puppet Adventure), the points dialog and the event boxes.
//  Official: every text (Server_Key tHalloween2025Npc / Kua / Guai / Lottery), store goods + CP prices
//  (luauirewardshow 302001xxx), training reward lists (301001-301003xxx), box contents (itemtype tips).
//  DERIVED numbers: Infinity[DB]\PuppetTheater\config.json.  See PuppetTheaterGame.cs for the rules of every grant.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.PuppetTheater
{
    // ====================================================================== Puppet Home
    public static class PuppetHome
    {
        public static readonly string[] Names = { "", "Papet", "Blaze Soul", "Gilt Soul", "Abyss Soul", "Stone Soul" };
        static readonly int[] AttrType = { 0, 5, 1, 2, 3, 4 };   // STR_LUAUI_25HALLOWEENDRAW_SKILL_TYPE_n
        public static int Level(uint uid, int k) { return (int)PT.Get(uid, "sk" + k); }
        public static int TotalLevel(uint uid) { int n = 0; for (int k = 1; k <= 5; k++) n += Level(uid, k); return n; }
        /// <summary>Official (video 02:36): all 5 puppets are trainable from day 1.</summary>
        public static bool Awake(uint uid, int k) { return k >= 1 && k <= 5; }
        /// <summary>The 8-item reward list of puppet k (lists are per puppet: config puppetList).</summary>
        public static List<uint[]> ListOf(int k)
        {
            var cfg = PT.Cfg; int t = k >= 1 && k <= 5 ? cfg.PuppetList[k - 1] : 1;
            List<uint[]> l; return cfg.Tiers.TryGetValue(t, out l) ? l : new List<uint[]>();
        }
        static int Batch(Client.GameClient c) { var cfg = PT.Cfg; return (int)Math.Max(1, Math.Min((uint)cfg.MaxUpgPerClick, PT.Count(c, PT.Soulthread) / (uint)cfg.TrainCost)); }

        static string Data(Client.GameClient c)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            var sb = new StringBuilder("{");
            sb.Append("{{").Append(PT.SybSP).Append(',').Append(PT.Count(c, PT.SP)).Append("},{").Append(PT.SybSoul).Append(',').Append(PT.Count(c, PT.Soulthread)).Append("}},");
            sb.Append(Batch(c) * cfg.TrainCost).Append(",{");
            for (int k = 1; k <= 5; k++)
            {
                int lv = Level(uid, k), sel = (int)PT.Get(uid, "sel" + k), ex = (int)PT.Get(uid, "ex" + k);
                var list = ListOf(k);
                string syb = sel >= 1 && sel <= list.Count ? list[sel - 1][0].ToString() : "nil";
                sb.Append('{').Append(lv).Append(',').Append(syb).Append(",{").Append(AttrType[k]).Append(',').Append(lv * cfg.AttrPerLevel[k - 1]).Append("},")
                  .Append(sel > 0 ? 1 : 0).Append(",{").Append(PT.SybSoul).Append(',').Append(ex).Append(',').Append(cfg.ExtraMax).Append("}}");
                if (k < 5) sb.Append(',');
            }
            long pts = PT.Get(uid, "pts");
            sb.Append("},").Append(PT.Get(uid, "skip")).Append(",['Points']={").Append(pts).Append(',').Append(PT.NextMilestone(pts)).Append("},['Show']={");
            for (int k = 1; k <= 5; k++) sb.Append(Awake(uid, k) ? 1 : 0).Append(k < 5 ? "," : "");
            return sb.Append("}}").ToString();
        }
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Reward[]> TrainPopup = new System.Collections.Concurrent.ConcurrentDictionary<uint, Reward[]>();
        public static void Congratulation(Client.GameClient c) { Reward[] r; if (TrainPopup.TryRemove(c.Player.UID, out r)) PtUi.Reward(c, null, r); }
        public static void Open(Client.GameClient c)
        {
            PuppetTheaterUi.ShowSub(c, "draw");
            Kit.Lua(c, "InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=" + PuppetTheaterUi.InterfaceDraw + ",['Data']=" + Data(c) + "}", PT.LuaMsg);
            PT.HomeGuide(c);
        }
        public static void Update(Client.GameClient c) { Kit.Lua(c, "Halloween25Draw_UpdateData|{['Code']=200,['Data']=" + Data(c) + "}", PT.LuaMsg); }

        public static void Choose(Client.GameClient c, int k)
        {
            uint uid = c.Player.UID;
            if (k < 1 || k > 5 || !Awake(uid, k)) return;
            var list = ListOf(k);
            int sel = (int)PT.Get(uid, "sel" + k);
            Kit.Lua(c, "Halloween25Draw_OpenSeletReward|{['Code']=200,['Data']={" + (sel > 0 ? sel.ToString() : "nil") + ",{" + string.Join(",", list.Select(x => x[0])) + "}}}", PT.LuaMsg);
        }
        public static bool Confirm(Client.GameClient c, int k, int idx)
        {
            uint uid = c.Player.UID;
            if (k < 1 || k > 5 || !Awake(uid, k)) return false;
            if (idx < 1 || idx > ListOf(k).Count) return false;
            PT.St.Set(uid, "sel" + k, idx);
            Update(c);
            return true;
        }
        /// <summary>Training (official, video 02:36-07:40): 1 Soulthread per UPG, the button spends them all ("N UPG xN");
        /// every UPG raises a random puppet by 1 level and pays that puppet`s chosen reward; 50 Terror Points per UPG.</summary>
        public static string Train(Client.GameClient c, ServerSockets.Packet s)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            if (Enumerable.Range(1, 5).Any(k => PT.Get(uid, "sel" + k) == 0)) { PT.Msg(c, PT.T("Lottery[MsgBox][NoSelect]")); return "noselect"; }
            int have = (int)(PT.Count(c, PT.Soulthread) / (uint)cfg.TrainCost);
            if (have < 1) { PT.Msg(c, PT.Fmt(PT.T("Lottery[MsgBox][NoItem]"), "Soulthreads", cfg.TrainCost)); return "noitem"; }
            int n = Math.Min(have, cfg.MaxUpgPerClick);
            var lv = new int[6]; for (int k = 1; k <= 5; k++) lv[k] = Level(uid, k);
            var picks = new List<int>(); var gain = new Dictionary<uint, uint>();
            for (int u = 0; u < n; u++)
            {
                var can = Enumerable.Range(1, 5).Where(k => lv[k] < cfg.SkillMax).ToList();
                if (can.Count == 0) break;
                int k2 = can[Kit.Next(0, can.Count - 1)];
                var list = ListOf(k2); int sel = (int)PT.Get(uid, "sel" + k2);
                if (sel < 1 || sel > list.Count) { PT.Msg(c, PT.T("Lottery[MsgBox][NoSelect]")); return "noselect"; }
                var row = list[sel - 1];
                uint amount = Math.Max(1u, row[2] / (uint)cfg.PerUpgDivisor);
                uint cur; gain.TryGetValue(row[1], out cur); gain[row[1]] = cur + amount;
                lv[k2]++; picks.Add(k2);
            }
            if (picks.Count == 0) { PT.Msg(c, "All your puppets have reached the highest level."); return "max"; }
            var rewards = gain.Select(g => new Reward(g.Key, g.Value, true)).ToArray();
            if (!Kit.HasSpace(c, Kit.Slots(rewards))) { PT.Msg(c, "Please make more room in your inventory first."); return "space"; }
            if (!Kit.Take(c, PT.Soulthread, (uint)(picks.Count * cfg.TrainCost), s)) return "noitem";
            PT.St.Set(uid, Enumerable.Range(1, 5).Select(k => Store.KV("sk" + k, lv[k])).ToArray());
            PT.AddPoints(c, cfg.TrainPoints * picks.Count);
            string got = PT.Give(c, s, "train x" + picks.Count, rewards);
            PT.Msg(c, PT.Fmt("%s UPG! You obtained %s and %s Terror Points.", picks.Count, got, cfg.TrainPoints * picks.Count));
            Kit.Lua(c, "Halloween25Draw_ResultReturn|{['Code']=200,['Data']={['Result']={" + string.Join(",", picks) + "}}}", PT.LuaMsg);
            Update(c);
            // official (video 07:08): the "You received" popup after the training animation (ClientCongratulation; at once when skipped)
            if (PT.Get(uid, "skip") == 1) PtUi.Reward(c, null, rewards); else TrainPopup[uid] = rewards;
            return "train " + picks.Count;
        }
    }

    // ====================================================================== Puppet Finale (Underground Cave)
    public static class PuppetFinale
    {
        public static void DayPublic() { Day(); }
        static void Day()
        {
            string d = PT.Today;
            lock (PT.St)
            {
                if (PT.St.Get(0, "fDay") == d) return;
                uint top; uint.TryParse(PT.St.Get(0, "fTop"), out top);
                if (top != 0) PT.St.Set(0, "fPay" + top, PT.St.Get(0, "fDay"));
                PT.St.Set(0, Store.KV("fDay", d), Store.KV("fHp", PT.Cfg.FinalePool), Store.KV("fTop", 0), Store.KV("fTopDmg", 0), Store.KV("fTopName", "Vacant"));
            }
        }
        /// <summary>Yesterday's top damage: 2 Soulthreads + 1D Solar Steel Box (official Text319), once.</summary>
        public static void PayTop(Client.GameClient c, ServerSockets.Packet s)
        {
            Day();
            uint uid = c.Player.UID; var cfg = PT.Cfg;
            string k = "fPay" + uid;
            if (PT.St.Get(0, k).Length == 0) return;
            var r = new[] { new Reward(PT.Soulthread, (uint)Math.Max(1, cfg.FinaleTopSoul)), new Reward(cfg.FinaleTopBox, 1, true) };
            if (!PT.Space(c, r)) { PT.Msg(c, "Make room in your inventory to receive your Puppet Finale Top DMG Reward."); return; }
            PT.St.Set(0, k, "");
            PT.Msg(c, PT.T("Guai[Mail][2][Content]") + " " + PT.Give(c, s, "finale top dmg", r));
        }
        public static void Info(Client.GameClient c, ServerSockets.Packet s)
        {
            Day();
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            long hp = PT.St.GetLong(0, "fHp", cfg.FinalePool);
            bool today = PT.St.Get(uid, "finDay") == PT.Today;
            var d = PT.Dialog(c, s, PT.DlgFinale, (cc, ss, o) => { if (o == 1) Challenge(cc, ss); });
            for (int i = 1; i <= 9; i++) PT.Say(d, PT.T("Npc[34646][Text31" + i + "]"));
            PT.Say(d, PT.T("Npc[34646][Text3110]"));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text3111]"), Math.Max(0, hp), cfg.FinalePool));
            long top = PT.St.GetLong(0, "fTopDmg", 0);
            PT.Say(d, top > 0 ? PT.Fmt(PT.T("Npc[34646][Text3112]"), PT.St.Get(0, "fTopName"), top) : PT.T("Npc[34646][Text3114]"));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text3113]"), PT.St.GetLong(uid, "fDmg" + PT.Today, 0)));
            d.AddOption(today ? PT.T("Npc[34646][Option155]", "Puppet Finale (Challenged).") : PT.Fmt(PT.T("Npc[34646][Option311]"), 0), today ? (byte)255 : (byte)1);
            d.AddOption(PT.T("Npc[34646][Option312]", "Go back."), 255).FinalizeDialog();
        }
        public static string Challenge(Client.GameClient c, ServerSockets.Packet s)
        {
            return PuppetBattle.Finale(c);    // [2026-10-15 video] the official in-window puppet battle
        }
        static string ChallengeCopyOld(Client.GameClient c, ServerSockets.Packet s)
        {
            Day();
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            if (PuppetHome.TotalLevel(uid) < cfg.FinaleMinPuppet) { PT.Msg(c, PT.T("Guai[MsgBox][3]")); return "puppet"; }
            if (PT.St.Get(uid, "finDay") == PT.Today) { PT.Msg(c, PT.T("Guai[MsgBox][2]")); return "done"; }
            if (!PT.InTown(c)) { PT.Msg(c, "Please go to Twin City or the Market to join the Puppet Finale."); return "town"; }
            var old = Arena.Of(uid);
            if (old != null && old.Tag == "PTFinale" && old.Armed && Arena.Alive(old) > 0)
            {
                // [re-entry] same copy, same Marionette (HP and timer kept)
                if (!Arena.Goto(c, old, 133, 150)) { PT.Msg(c, "This place isn`t open yet."); return "nomap"; }
                PT.Msg(c, "You are back in the Puppet Finale.");
                return "reenter";
            }
            bool cleanup = PT.St.GetLong(0, "fHp", cfg.FinalePool) <= 0;
            var cp = Arena.Enter(c, 1004402, 133, 150, "PTFinale", cfg.FinaleMinutes, PT.TwinCity, PT.ExitX, PT.ExitY);
            if (cp == null) { PT.Msg(c, "This place isn`t open yet."); return "nomap"; }
            Arena.ClearMobs(cp);
            var mob = Arena.Spawn(cp, cleanup ? PT.FamNightmare : PT.FamFinale, 133, 157, 2, s);
            if (mob == null) { Arena.Close(cp, true); PT.Msg(c, "The Marionette isn`t ready yet. Please try again later."); return "nomob"; }
            // [2026-10-15] the daily challenge is used up by the WIN (Won); a fall / timeout lets the player try again
            cp.Data = DateTime.Now;
            Arena.Arm(cp, (cc, cpp) => Won(cc, cpp, cleanup));
            PT.Log(c, "FINALE", cleanup ? "cleanup" : "marionette");
            return cleanup ? "cleanup" : "finale";
        }
        static void Won(Client.GameClient c, Copy cp, bool cleanup)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            double secs = cp.Data is DateTime ? (DateTime.Now - (DateTime)cp.Data).TotalSeconds : 9999;
            lock (Kit.Lock(uid))
            {
                var r = new List<Reward>();
                if (PT.Get(uid, "finFirst") == 0 && cfg.FinaleFirst > 0) r.Add(new Reward(PT.Soulthread, (uint)cfg.FinaleFirst));
                else if (cleanup && cfg.FinaleCleanup > 0) r.Add(new Reward(PT.Soulthread, (uint)cfg.FinaleCleanup));
                if (!cleanup)
                {
                    // damage = the boss HP, more for a fast win (DERIVED: the official battle is 10 turn-rounds)
                    long dmg = (long)(cfg.FinaleBossHp * (1.0 + Math.Max(0, cfg.FinaleMinutes * 60 - secs) / (cfg.FinaleMinutes * 60.0)));
                    lock (PT.St)
                    {
                        PT.St.Set(0, "fHp", Math.Max(0, PT.St.GetLong(0, "fHp", cfg.FinalePool) - dmg));
                        PT.St.Set(uid, "fDmg" + PT.Today, PT.St.GetLong(uid, "fDmg" + PT.Today, 0) + dmg);
                        if (dmg > PT.St.GetLong(0, "fTopDmg", 0))
                        {
                            string nm = new string((c.Player.Name ?? "").Where(ch => ch != ';' && ch != '=' && ch != '\t').ToArray());
                            PT.St.Set(0, Store.KV("fTop", uid), Store.KV("fTopDmg", dmg), Store.KV("fTopName", nm));
                        }
                    }
                    PT.Msg(c, PT.Fmt("You dealt %s damage to the Evil Marionette.", dmg));
                }
                PT.St.Set(uid, "finDay", PT.Today);   // one challenge a day (official): counted at the win
                if (r.Count > 0)
                {
                    if (PT.Space(c, r.ToArray()))
                    {
                        PT.St.Set(uid, "finFirst", 1);
                        using (var rec = new ServerSockets.RecycledPacket()) PT.Msg(c, PT.T("Guai[Mail][1][Content]") + " " + PT.Give(c, rec.GetStream(), "finale", r.ToArray()));
                    }
                    else PT.Msg(c, "Your inventory is full: the Puppet Finale reward was not given.");
                }
            }
            Arena.Close(cp, true);
        }
    }

    // ====================================================================== NPCs, store, points, items
    public static class PuppetNpcs
    {
        /// <summary>Milestone row i (0-based) claimed.</summary>
        public static bool Claimed(uint uid, int i) { return PT.St.GetList(uid, "mc").Contains(i); }
        /// <summary>Anything to claim now (red dot): a Terror Box, a milestone or a level reward.</summary>
        public static bool AnyClaim(uint uid)
        {
            var cfg = PT.Cfg; long pts = PT.Get(uid, "pts");
            if (pts / Math.Max(1u, cfg.MilestoneEvery[0]) > PT.Get(uid, "tbx")) return true;
            for (int i = 0; i < cfg.Milestones.Count; i++) if (pts >= cfg.Milestones[i][0] && !Claimed(uid, i)) return true;
            return LevelClaims(uid).Count > 0;
        }
        static List<int> LevelClaims(uint uid)
        {
            var cfg = PT.Cfg; var done = PT.St.GetList(uid, "lr"); var r = new List<int>();
            for (int k = 1; k <= 5; k++)
                for (int i = 0; i < cfg.LevelRewards.Count; i++)
                    if (PuppetHome.Level(uid, k) >= cfg.LevelRewards[i][0] && !done.Contains(k * 100 + i)) r.Add(k * 100 + i);
            return r;
        }

        public static void Open(Client.GameClient c, ServerSockets.Packet s, uint npc)
        {
            if (!PT.Running) return;
            var cfg = PT.Cfg;
            lock (Kit.Lock(c.Player.UID))
            {
                if (npc == PT.Suyi) { Suyi(c, s, 0); return; }
                if (npc == PT.Elise)
                {
                    var d = PT.Dialog(c, s, PT.DlgStore, (cc, ss, o) => { if (o == 1 && PT.InView(cc, PT.Elise)) StoreMenu(cc, ss, true); });
                    if (c.Player.Level < cfg.MinLevel) { PT.Say(d, PT.T("Npc[34647][Text111]") + PT.T("Npc[34647][Text122]")); d.AddOption(PT.T("Npc[34647][Option121]"), 255).FinalizeDialog(); return; }
                    PT.Say(d, PT.T("Npc[34647][Text111]") + PT.T("Npc[34647][Text142]"));
                    d.AddOption(PT.T("Npc[34647][Option141]"), 1).AddOption(PT.T("Npc[34647][Option111]"), 255).FinalizeDialog();
                    return;
                }
                if (npc == PT.Spectator)
                {
                    var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) => { });
                    PT.Say(d, PT.T("Lottery[34655][Text111]") + PT.T("Lottery[34655][Text112]"));
                    d.AddOption(PT.T("Lottery[34655][Option111]"), 255).FinalizeDialog();
                    return;
                }
                if (npc == PT.BrokenPuppet) { Adventure(c, s, 0); return; }
                if (npc == Orion) { OrionMenu(c, s); return; }
                if (npc == PuppetSeer.Seer) { PuppetSeer.Open(c, s); return; }
                if (PuppetVault.ThemeOf(npc) > 0) { PuppetVault.Open(c, s, npc); return; }   // hero statues (Vault Palace pass)
                if (npc == FragmentSpot) { GotFragment(c, 2); return; }
                if (npc == PT.VaultPortal) { Arena.Move(c, cfg.AdvX, (ushort)(cfg.AdvY + 3), cfg.AdvMap, 0); return; }   // [SuyiOfficial 2026-10-15]
            }
        }

        // ------------------------------------------------------------------ Suyi
        static string Duration { get { return PT.Plain(PT.T("CN:STR_LUAUI_25HALLOWEENMAIN_TIME").Replace("Duration: ", "")); } }
        static void Suyi(Client.GameClient c, ServerSockets.Packet s, int page)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            PT.Daily(c, s);
            var d = PT.Dialog(c, s, PT.DlgMain, (cc, ss, o) =>
            {
                if (!PT.InView(cc, PT.Suyi)) return;
                switch (o)
                {
                    case 10: Suyi(cc, ss, 1); break;
                    case 11: Suyi(cc, ss, 2); break;
                    case 12: Suyi(cc, ss, 3); break;
                    case 14: Suyi(cc, ss, 4); break;
                    case 13: PT.St.Set(cc.Player.UID, "intro", 1); Suyi(cc, ss, 0); break;
                    case 1: PuppetTheaterUi.SendMain(cc); break;
                    case 2: PuppetFinale.Info(cc, ss); break;
                    case 3: PuppetRewards.Open(cc, PuppetRewards.TabPoints); break;
                    case 4: Convert(cc, ss, false); break;
                    case 5:
                        Arena.Move(cc, cfg.AdvX, (ushort)(cfg.AdvY + 3), cfg.AdvMap, 0);
                        break;
                }
            });
            PT.Say(d, PT.T("Npc[34646][Text111]"));
            if (c.Player.Level < cfg.MinLevel) { PT.Say(d, PT.T("Npc[34646][Text112]") + PT.Fmt(PT.T("Npc[34646][Text114]"), Duration) + PT.T("Npc[34646][Text115]")); d.AddOption(PT.T("Npc[34646][Option131]"), 255).FinalizeDialog(); return; }
            if (PT.Get(uid, "intro") == 0)
            {
                // official intro: Text112 -> Text141 -> Text211 -> Text212 -> Text213
                string[] text = { "Text112", "Text141", "Text211", "Text212", "Text213" };
                string[] opt = { "Option111", "Option141", "Option211", "Option212", "Option213" };
                byte[] next = { 10, 11, 12, 14, 13 };
                int p = Math.Min(Math.Max(page, 0), 4);
                if (p == 0) PT.Say(d, PT.T("Npc[34646][Text112]") + PT.Fmt(PT.T("Npc[34646][Text114]"), Duration) + PT.T("Npc[34646][Text115]") + PT.T("Npc[34646][Text116]"));
                else PT.Say(d, PT.T("Npc[34646][" + text[p] + "]"));
                d.AddOption(PT.T("Npc[34646][" + opt[p] + "]"), next[p]).FinalizeDialog();
                return;
            }
            bool vault = PT.St.Get(uid, "vpDay") == PT.Today, fin = PT.St.Get(uid, "finDay") == PT.Today, adv = PT.St.Get(uid, "advDay") == PT.Today;
            for (int i = 1; i <= 7; i++) PT.Say(d, PT.T("Npc[34646][Text15" + i + "]"));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text158]"), vault ? 1 : 0));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text159]"), fin ? 1 : 0));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text1510]"), adv ? 1 : 0));
            PT.Say(d, PT.T("Npc[34646][Text1511]"));
            // [SuyiOfficial 2026-10-15] official first-talk page (owner's video shot): rules + duration/requirement/rewards, 4 options only
            for (int i = 2; i <= 4; i++) PT.Say(d, PT.T("Npc[34646][Text151" + i + "]"));
            PT.Say(d, PT.Fmt(PT.T("Npc[34646][Text114]"), Duration) + PT.T("Npc[34646][Text115]") + PT.T("Npc[34646][Text116]"));
            d.AddOption(PT.T("Npc[34646][Option151]"), 1);
            d.AddOption(fin ? PT.T("Npc[34646][Option155]") : PT.T("Npc[34646][Option152]"), 2);
            d.AddOption(PT.T("Npc[34646][Option153]"), 3);
            d.AddOption(PT.T("Npc[34646][Option154]"), 4).FinalizeDialog();   // Vault Palace = the VaultPortal next to the TC stairs (official)
        }

        /// <summary>Convert SP: after every area is cleared, every 10 SP has a chance to become 1 Soulthread (official rule).</summary>
        public static string Convert(Client.GameClient c, ServerSockets.Packet s, bool confirm)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            for (int L = 1; L <= 10; L++) if (!PuppetMaze.Cleared(uid, L)) { PT.Msg(c, PT.T("Exploration[ClientTips][NoAllMaze]")); return "notall"; }
            uint sp = PT.Count(c, PT.SP) / 10 * 10;
            if (sp == 0) { PT.Msg(c, PT.T("Exploration[ClientTips][NoPower]")); return "nosp"; }
            if (!confirm)
            {
                var d = PT.Dialog(c, s, PT.DlgMain, (cc, ss, o) => { if (o == 1 && PT.InView(cc, PT.Suyi)) Convert(cc, ss, true); });
                PT.Say(d, PT.T("Npc[34646][Text411]") + PT.Fmt(PT.T("Npc[34646][Text412]"), sp));
                d.AddOption(PT.T("Npc[34646][Option411]"), 1).AddOption(PT.T("Npc[34646][Option412]"), 255).FinalizeDialog();
                return "ask";
            }
            uint n = 0; for (uint i = 0; i < sp / 10; i++) if (Kit.Next(1, 100) <= cfg.SpConvertChance) n++;
            if (n > 0 && !PT.Space(c, new Reward(PT.Soulthread, n))) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            if (!Kit.Take(c, PT.SP, sp, s)) return "nosp";
            if (n == 0) { PT.Msg(c, PT.T("Exploration[ClientTips][NoGetItem]")); PT.Log(c, "CONVERT", sp + " SP -> 0"); return "zero"; }
            PT.Msg(c, "You obtained " + PT.Give(c, s, "convert " + sp + " SP", new Reward(PT.Soulthread, n)) + ".");
            return "convert " + n;
        }

        // ------------------------------------------------------------------ Puppet Store (Elise)
        public static void StoreMenu(Client.GameClient c, ServerSockets.Packet s, bool atNpc, int guide = 0)
        {
            var cfg = PT.Cfg;
            // [2026-10-15 fix] the window "+" guides: 1 = SP (SP Awakening Pack: 50 SP), 2 = Soulthreads (Random Soulthread Pack)
            if (guide == 1 || guide == 2)
            {
                uint want = guide == 1 ? 3354672u : 3354671u;
                int gi = cfg.Store.FindIndex(x => x[0] == want);
                PT.Msg(c, guide == 1 ? PT.T("Npc[Sys][StoreGuidTiLi]") + " SP Awakening Pack = 50 SP (right-click it in your bag)." : "Random Soulthread Pack = 3/4/8/100 Soulthreads (right-click it in your bag).");
                if (gi >= 0) { BuyAsk(c, s, gi + 1); return; }
            }
            var d = PT.Dialog(c, s, PT.DlgStore, (cc, ss, o) => { if (o >= 1 && o <= cfg.Store.Count) BuyAsk(cc, ss, o); });
            PT.Say(d, PT.T("Npc[34647][Text111]") + PT.T("Npc[34647][Text142]"));
            for (int i = 0; i < cfg.Store.Count && i < 9; i++) d.AddOption(Kit.Name(cfg.Store[i][0]) + " (" + Price(cfg.Store[i]) + ")", (byte)(i + 1));
            d.AddOption(PT.T("Npc[34647][Option111]", "Return later."), 255).FinalizeDialog();
        }
        static uint Currency(uint[] g) { return g.Length > 2 ? g[2] : 0; }
        static string Price(uint[] g) { return Currency(g) == 0 ? g[1] + " CPs" : g[1] + " " + Kit.Name(Currency(g)) + "s"; }
        static void BuyAsk(Client.GameClient c, ServerSockets.Packet s, int i)
        {
            var g = PT.Cfg.Store[i - 1];
            var d = PT.Dialog(c, s, PT.DlgStore, (cc, ss, o) => { if (o == 1) Buy(cc, ss, i); else if (o == 2) StoreMenu(cc, ss, false); });
            PT.Say(d, PT.Fmt("Get 1 %s for %s?", Kit.Name(g[0]), Price(g)));
            d.AddOption("Confirm.", 1).AddOption("Go back.", 2).FinalizeDialog();
        }
        /// <summary>One purchase / exchange (CPs, or Octave Bells for the Puppet garments + Papet, video 08:50).</summary>
        public static string Buy(Client.GameClient c, ServerSockets.Packet s, int i)
        {
            var cfg = PT.Cfg;
            if (i < 1 || i > cfg.Store.Count) return "bad";
            var g = cfg.Store[i - 1];
            if (!Kit.Known(g[0])) return "unknown";
            bool bound = g.Length > 3 && g[3] == 1;
            var r = new Reward(g[0], 1, bound);
            if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            uint cur = Currency(g);
            if (cur == 0)
            {
                if (c.Player.ConquerPoints < g[1]) { PT.Msg(c, "Sorry, you don`t have enough CPs."); return "cps"; }
                c.Player.ConquerPoints -= g[1];
            }
            else
            {
                if (PT.Count(c, cur) < g[1]) { PT.Msg(c, PT.Fmt("Sorry, you need %s %s.", g[1], Kit.Name(cur) + "s")); return "bells"; }
                if (!Kit.Take(c, cur, g[1], s)) return "bells";
            }
            PT.Log(c, "BUY", Kit.Name(g[0]) + " for " + Price(g));
            if (g[0] / 1000 == 194) { c.Inventory.Add(s, g[0], 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound); PT.Msg(c, "You obtained 1 " + Kit.Name(g[0]) + "."); }
            else PT.Msg(c, "You obtained " + PT.Give(c, s, "store", r) + ".");
            PuppetTheaterUi.Refresh(c);
            return "bought";
        }

        // ------------------------------------------------------------------ points & rankings (StageSybTemplate 10768001-004)
        //  Points Reward (video 07:59): a Terror Box for every 1000 points (repeatable) + the milestones;
        //  Level Reward (08:38) per puppet; Ranking (local list).
        public static void Stage(Client.GameClient c, ServerSockets.Packet s, int tab)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID; long pts = PT.Get(uid, "pts");
            var d = PT.Dialog(c, s, PT.DlgStage, (cc, ss, o) =>
            {
                if (o >= 1 && o <= cfg.Milestones.Count) Claim(cc, ss, o);
                else if (o == 50) ClaimEvery(cc, ss);
                else if (o == 60) ClaimLevels(cc, ss);
                else if (o == 100) Stage(cc, ss, 10768002);
                else if (o == 101) Stage(cc, ss, 10768001);
            });
            if (tab == 10768002)
            {
                var sb = new StringBuilder("Terror Points ranking (local):\n");
                int n = 1; foreach (var r in PT.Ranking().Take(10)) sb.Append("No." + n++ + "  " + r.Item3 + "  " + r.Item2 + "\n");
                if (n == 1) sb.Append("Nobody has earned Terror Points yet.\n");
                sb.Append("Your points: " + pts);
                PT.Say(d, sb.ToString());
                d.AddOption("Points rewards.", 101).AddOption("Close.", 255).FinalizeDialog();
                return;
            }
            PT.Say(d, PT.T("Npc[34646][Text1513]"));
            PT.Say(d, "Your Terror Points: " + pts + "\n");
            uint every = Math.Max(1u, cfg.MilestoneEvery[0]);
            long boxes = pts / every - PT.Get(uid, "tbx");
            d.AddOption(PT.Fmt("Claim every %s points: %s %s (%s ready)", every, cfg.MilestoneEvery[2], Kit.Name(cfg.MilestoneEvery[1]), Math.Max(0, boxes)), 50);
            for (int i = 0; i < cfg.Milestones.Count && i < 8; i++)
            {
                var m = cfg.Milestones[i];
                string st = Claimed(uid, i) ? "(claimed)" : pts >= m[0] ? "(claim)" : "(" + pts + "/" + m[0] + ")";
                d.AddOption(m[0] + ": " + m[2] + " " + Kit.Name(m[1]) + " " + st, (byte)(i + 1));
            }
            d.AddOption("Level Reward (" + LevelClaims(uid).Count + " ready).", 60);
            d.AddOption("Local ranking.", 100).AddOption("Close.", 255).FinalizeDialog();
        }
        public static string Claim(Client.GameClient c, ServerSockets.Packet s, int i)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            if (i < 1 || i > cfg.Milestones.Count) return "bad";
            var m = cfg.Milestones[i - 1];
            if (PT.Get(uid, "pts") < m[0]) { PT.Msg(c, "You don`t have enough Terror Points yet."); return "pts"; }
            var list = PT.St.GetList(uid, "mc");
            if (list.Contains(i - 1)) { PT.Msg(c, "You have already claimed this reward."); return "claimed"; }
            var r = new Reward(m[1], m[2], true);
            if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            list.Add(i - 1);
            PT.St.Set(uid, "mc", string.Join(",", list));
            PT.Msg(c, "You obtained " + PT.Give(c, s, "points " + m[0], r) + ".");
            Stage(c, s, 10768001);
            return "claim " + m[0];
        }
        /// <summary>Every 1000 points: one Terror Box each (repeatable; every ready one at once, max 40 per claim).</summary>
        public static string ClaimEvery(Client.GameClient c, ServerSockets.Packet s)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            long ready = PT.Get(uid, "pts") / Math.Max(1u, cfg.MilestoneEvery[0]) - PT.Get(uid, "tbx");
            if (ready <= 0) { PT.Msg(c, "You don`t have enough Terror Points yet."); return "none"; }
            ready = Math.Min(ready, 40);
            var r = new Reward(cfg.MilestoneEvery[1], (uint)(ready * cfg.MilestoneEvery[2]), true);
            if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            PT.St.Set(uid, "tbx", PT.Get(uid, "tbx") + ready);
            PT.Msg(c, "You obtained " + PT.Give(c, s, "points every x" + ready, r) + ".");
            return "boxes " + ready;
        }
        /// <summary>Level Reward: every ready (puppet, level) row, once each.</summary>
        public static string ClaimLevels(Client.GameClient c, ServerSockets.Packet s)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            var ready = LevelClaims(uid);
            if (ready.Count == 0) { PT.Msg(c, "No Level Reward is ready yet. Train your puppets in Puppet Home."); return "none"; }
            var gain = new Dictionary<uint, uint>();
            foreach (var key in ready) { var row = cfg.LevelRewards[key % 100]; uint cur; gain.TryGetValue(row[1], out cur); gain[row[1]] = cur + row[2]; }
            var r = gain.Select(g => new Reward(g.Key, g.Value, true)).ToArray();
            if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            var done = PT.St.GetList(uid, "lr"); done.AddRange(ready);
            PT.St.Set(uid, "lr", string.Join(",", done));
            PT.Msg(c, "You obtained " + PT.Give(c, s, "level rewards " + ready.Count, r) + ".");
            return "levels " + ready.Count;
        }

        // ------------------------------------------------------------------ Puppet Adventure (Broken Puppet, Vault Palace)
        public static int Frags(uint uid) { return PT.St.Get(uid, "fragDay") == PT.Today ? (int)PT.Get(uid, "frags") : 0; }
        static int FragCount(int f) { int n = 0; for (int i = 0; i < 3; i++) if ((f & (1 << i)) != 0) n++; return n; }
        public static string Adventure(Client.GameClient c, ServerSockets.Packet s, int page)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            if (c.Player.Level < cfg.MinLevel) { PT.Msg(c, PT.T("Exploration[Sys][LevelLimit]")); return "level"; }
            // Vault Palace Visit (Suyi's daily list): the first talk of the day here gives SP (official TiliBao text)
            if (PT.St.Get(uid, "vpDay") != PT.Today && cfg.VaultSp > 0 && Kit.NpcInView(c, PT.BrokenPuppet))
            {
                var sp = new Reward(PT.SP, (uint)cfg.VaultSp);
                if (PT.Space(c, sp)) { PT.St.Set(uid, "vpDay", PT.Today); PT.Give(c, s, "vault palace visit", sp); PT.Msg(c, PT.Fmt(PT.T("Npc[Sys][TiliBao]"), cfg.VaultSp)); }
            }
            if (PT.St.Get(uid, "advDay") == PT.Today) { PT.Msg(c, PT.T("Kua[Sys][OverTask]")); return "done"; }
            int f = Frags(uid), n = FragCount(f);
            var d = PT.Dialog(c, s, PT.DlgAdv, (cc, ss, o) =>
            {
                if (!Kit.NpcInView(cc, PT.BrokenPuppet) && o != 1) return;
                if (o == 1) Search(cc, ss);
                else if (o == 2) Adventure(cc, ss, 2);
                else if (o == 3) Adventure(cc, ss, 3);
                else if (o == 4) Submit(cc, ss, 0);
                else if (o >= 11 && o <= 13) Submit(cc, ss, o - 10);
            });
            PT.Say(d, PT.T("Kua[34651][Text111]"));
            if (page == 0 && PT.St.Get(uid, "advMet") != PT.Today)
            {
                PT.St.Set(uid, "advMet", PT.Today);
                PT.Say(d, PT.T("Kua[34651][Text112]") + PT.T("Kua[34651][Text113]"));
                d.AddOption(PT.T("Kua[34651][Option111]"), 2).FinalizeDialog();
                return "intro";
            }
            if (page == 2)
            {
                PT.Say(d, PT.T("Kua[34651][Text211]") + PT.T("Kua[34651][Text212]"));
                d.AddOption(PT.T("Kua[34651][Option211]"), 3).FinalizeDialog();
                return "intro2";
            }
            if (page == 4)
            {
                if (n == 0) { PT.Say(d, PT.T("Kua[34651][Text311]")); d.AddOption(PT.T("Kua[34651][Option311]"), 1).FinalizeDialog(); return "empty"; }
                PT.Say(d, PT.Fmt(PT.T("Kua[34651][Text331]"), n));
                d.AddOption(PT.T("Kua[34651][Option331]"), (byte)(10 + n)).AddOption(PT.T("Kua[34651][Option332]"), 255).FinalizeDialog();
                return "confirm";
            }
            PT.Say(d, PT.T("Kua[34651][Text131]") + PT.T("Kua[34651][Text132]") + PT.Fmt(PT.T("Kua[34651][Text133]"), n) + PT.T("Kua[34651][Text134]"));
            d.AddOption(PT.T("Kua[34651][Option131]"), 1).AddOption(PT.T("Kua[34651][Option132]"), 4).FinalizeDialog();
            return "menu";
        }
        /// <summary>Search for fragments (video 02:44-03:51): the open Vault Palace. Fragment 1 = a SilkSpecter, fragment 2 = a
        /// WeepWood (official monster.dat 23390 / 23392, spawned on the shared map), fragment 3 = the golden statue spot.</summary>
        public static string Search(Client.GameClient c, ServerSockets.Packet s)
        {
            uint uid = c.Player.UID;
            if (PT.St.Get(uid, "advDay") == PT.Today) { PT.Msg(c, PT.T("Kua[Sys][OverTask]")); return "done"; }
            if (FragCount(Frags(uid)) >= 3) { PT.Msg(c, PT.T("Kua[Sys][OverCollect]")); return "full"; }
            PT.Msg(c, "Defeat a SilkSpecter and a WeepWood in the Vault Palace and search the golden statue for the puppet fragments.");
            return "search";
        }
        public const uint SilkSpecter = 23390, WeepWood = 23392, BoneSnapper = 23391, FragmentSpot = 8893006, Orion = 8893005;
        /// <summary>Game\Monster\MonsterRole.cs (one hook line): a kill on the Vault Palace may carry a fragment.</summary>
        public static void OnKill(Game.MsgMonster.MonsterRole mob, Client.GameClient killer)
        {
            try
            {
                if (mob == null || killer == null || killer.Player == null || !PT.Running) return;
                uint fam = mob.Family != null ? mob.Family.ID : 0;
                int i = fam == SilkSpecter ? 0 : fam == WeepWood ? 1 : fam == BoneSnapper ? 2 : -1;
                if (i < 0 || killer.Player.Map != PT.Cfg.AdvMap) return;
                GotFragment(killer, i);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        public static void GotFragment(Client.GameClient c, int i)
        {
            uint uid = c.Player.UID;
            lock (Kit.Lock(uid))
            {
                if (PT.St.Get(uid, "advDay") == PT.Today) return;
                if (c.Player.Level < PT.Cfg.MinLevel) return;
                int f = Frags(uid);
                if ((f & (1 << i)) != 0) { if (i == 2) PT.Msg(c, PT.T("Kua[Sys][OverItem]")); return; }
                f |= 1 << i;
                PT.St.Set(uid, Store.KV("fragDay", PT.Today), Store.KV("frags", f));
                PT.Log(c, "FRAGMENT", i.ToString());
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var d = PT.Dialog(c, rec.GetStream(), PT.DlgAdv, (cc, ss, o) => { if (o == 2) Arena.Move(cc, PT.Cfg.AdvX, (ushort)(PT.Cfg.AdvY + 3), PT.Cfg.AdvMap, 0); });
                    PT.Say(d, PT.T("Kua[34651][Text" + (5 + i) + "11]"));
                    if (FragCount(f) < 3) d.AddOption(PT.T("Kua[34651][Option511]"), 1);
                    else PT.Msg(c, PT.T("Kua[Sys][OverCollect]"));
                    d.AddOption(PT.T("Kua[34651][Option512]"), 2).FinalizeDialog();
                }
            }
        }
        /// <summary>Orion (video 03:51, Vault Palace): View statues (1/1 time per day) / Puppet Adventure.</summary>
        public static void OrionMenu(Client.GameClient c, ServerSockets.Packet s)
        {
            uint uid = c.Player.UID; var cfg = PT.Cfg;
            bool seen = PT.St.Get(uid, "vpDay") == PT.Today;
            var d = PT.Dialog(c, s, PT.DlgAdv, (cc, ss, o) =>
            {
                if (!Kit.NpcInView(cc, Orion)) return;
                if (o == 1) { PuppetVault.Open(cc, ss, PuppetVault.FirstStatue + (uint)PuppetVault.HalloweenTheme - 1); return; }   // View statues: the statue ranking + timed Visit (official flow)
                else if (o == 2) { PT.Msg(cc, "A Broken Puppet seems to need help. Please go check it out."); Search(cc, ss); }
                else if (o == 3) Arena.Move(cc, PT.ExitX, PT.ExitY, PT.TwinCity, 0);
            });
            // official text (video 03:51)
            d.AddText("Orion:\n").AddText("------------------------View Statues------------------------\n");
            PT.Say(d, "Vault Palace is displaying this year`s top heroes in global events. Visit to claim surprise rewards (" + (seen ? 1 : 0) + "/1 time per day).");
            d.AddText("\n------------------------------------------------------------\n");
            PT.Say(d, "There`s a Broken Puppet in the corner of the Vault Palace. Its broken body was taken by ghosts. Helping it might bring an unexpected surprise!");
            d.AddOption("View statues.", 1).AddOption("Puppet Adventure.", 2).AddOption("Return to Twin City.", 3).FinalizeDialog();
        }
        /// <summary>Submit the fragments: 1 = rage or a small payment, 2 = a gift, 3 = choose a reward (once a day).</summary>
        public static string Submit(Client.GameClient c, ServerSockets.Packet s, int n)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            int have = FragCount(Frags(uid));
            if (PT.St.Get(uid, "advDay") == PT.Today) { PT.Msg(c, PT.T("Kua[Sys][OverTask]")); return "done"; }
            if (n == 0 || have == 0) { Adventure(c, s, 4); return "ask"; }
            if (n != have) return "bad";
            if (n == 3)
            {
                // official (video 03:51): the complete puppet gives the Puppet Select Box 3600816
                return Finish(c, s, 3, new Reward(3600816, 1, true), "Kua[34651][Text44");
            }
            if (n == 2) return Finish(c, s, 2, new Reward(PT.Soulthread, (uint)cfg.AdvTwoSoul), "Kua[34651][Text43");
            if (Kit.Next(1, 100) <= cfg.AdvOneChance) return Finish(c, s, 1, null, "Kua[34651][Text41");
            return Finish(c, s, 1, new Reward(PT.Soulthread, (uint)cfg.AdvOneSoul), "Kua[34651][Text42");
        }
        static string Finish(Client.GameClient c, ServerSockets.Packet s, int n, Reward r, string text)
        {
            uint uid = c.Player.UID;
            if (PT.St.Get(uid, "advDay") == PT.Today || FragCount(Frags(uid)) != n) return "done";
            if (r != null && !PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return "space"; }
            PT.St.Set(uid, Store.KV("advDay", PT.Today), Store.KV("frags", 0));
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) => { });
            var sb = new StringBuilder(PT.T("Kua[34651][Text111]"));
            for (int i = 1; i <= 4; i++) sb.Append(PT.T(text + i + "]"));
            PT.Say(d, sb.ToString());
            if (r == null) PT.Msg(c, PT.T("Kua[Sys][Sub1Fail]"));
            else PT.Msg(c, "You obtained " + PT.Give(c, s, "adventure " + n, r) + ".");
            d.AddOption(r == null ? PT.T("Kua[34651][Option411]") : PT.T("Kua[34651][Option421]"), 255).FinalizeDialog();
            return r == null ? "rage" : "reward " + n;
        }

        // ------------------------------------------------------------------ event boxes (official contents from the item tips)
        public static bool UseItem(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s)
        {
            uint id = item.ITEM_ID;
            Reward[] r;
            if (id == PT.SP || id == PT.Soulthread)
            {
                // currencies are counted from the bag: right-click opens where they are spent (SP -> the dungeon window,
                // Soulthread "Use to train puppets" -> Puppet Home). Nothing is consumed here.
                if (Kit.Throttled(c.Player.UID, "ptcur", 1000)) return true;
                if (!PT.Running) { PT.Msg(c, PT.T("Npc[34646][Text121]")); return true; }
                if (c.Player.Level < PT.Cfg.MinLevel) { PT.Msg(c, PT.T("Exploration[Sys][LevelLimit]")); return true; }
                lock (Kit.Lock(c.Player.UID)) { if (id == PT.SP) PuppetTheaterUi.SendMain(c); else PuppetHome.Open(c); }
                return true;
            }
            if (id == 3354673) return SolarBox(c, item, s, 0);
            if (id == 3600816) return SelectBox(c, s);
            if (id == 3354674) return DreamkeyDay(c, s);
            if (id == 3600814) return WeaponPack(c, s);
            if (id == PT.Cfg.OctaveBell) return Bell(c, s);
            if (id == 3354680) return BellBox(c, s);
            if (id == 3354682) return Equip(c, item, s, 194145, 1, true);                               // Mirage Shade (B): the garment
            if (id == 3314207 || id == 3321077) return Liquor(c, item, s, id == 3314207 ? 1000 : 100);
            switch (id)
            {
                case 3354668: r = new[] { new Reward(PT.Soulthread, 46) }; break;              // Small Soulthread Box
                case 3354669: r = new[] { new Reward(PT.Soulthread, 93) }; break;              // Medium Soulthread Box
                case 3354670: r = new[] { new Reward(PT.Soulthread, 187) }; break;             // Grand Soulthread Box
                case 3354671:                                                                    // Random Soulthread Pack 3/4/8/100 (weights DERIVED)
                    {
                        var pk = PT.Cfg.RandomSoulPack.Where(x => x.Length >= 2).ToList();
                        var row = pk.Count > 0 ? Kit.Pick(pk, x => x[1]) : new uint[] { 3, 1 };
                        r = new[] { new Reward(PT.Soulthread, row[0]) }; break;
                    }
                case 3354672: r = new[] { new Reward(PT.SP, 50), new Reward(3328247, 1) }; break; // SP Awakening Pack
                case 3354678:                                                                    // Contribution Pack: 7-day 3KK/6KK/10KK medal (B)
                    r = new[] { Kit.Pick(new[] { Reward.Timed(3346822, 1, true, 10080), Reward.Timed(3346823, 1, true, 10080), Reward.Timed(3346824, 1, true, 10080) }, x => x.Item == 3346822 ? 50u : x.Item == 3346823 ? 35u : 15u) }; break;
                case 3354679:                                                                    // Terror Box (weights DERIVED)
                    r = new[] { Kit.Pick(new[] { new Reward(3347158, 4, true, 30), new Reward(3328247, 50, true, 40), new Reward(3346824, 1, true, 10), new Reward(3009101, 1, true, 20) }, x => x.Weight) }; break;
                case 3346829: return Stone3(c, item, s);                                        // +3 Stone (B): 1 5-day +3 Stone (B)
                case 3354681: return Equip(c, item, s, 194145, 1);                               // Mirage Shade (T): garment
                case 3354683: return Equip(c, item, s, 194155, 1);                               // Mirage Marionette (T): garment (the hairstyle has no item here)
                case 3354689: return Equip(c, item, s, 360468, 2);                               // 2 Violet Dreamkeys (1-handed)
                case 3354690: return Equip(c, item, s, 350230, 1);                               // 1 Violet Dreamkey (2-handed)
                default: return false;
            }
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (!PT.Space(c, r)) { PT.Msg(c, "Please make room in your inventory first."); return true; }
                if (!c.Inventory.Remove(id, 1, s)) return true;
                PT.Msg(c, "You obtained " + PT.Give(c, s, "open " + id, r) + ".");
                PuppetTheaterUi.Refresh(c);
            }
            return true;
        }
        /// <summary>1D Solar Steel Box (official tip): choose 2 1-day Solar Steels (1-handed) (B) or 1 1-day Solar Steel (2-handed) (B).</summary>
        static bool SolarBox(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s, int pick)
        {
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o != 1 && o != 2) return;
                if (Kit.Count(cc, 3354673) == 0) return;
                uint gear = o == 1 ? 360409u : 350195u; int n = o == 1 ? 2 : 1;
                if (!Kit.Known(gear) || !Kit.HasSpace(cc, n)) { PT.Msg(cc, "Please make room in your inventory first."); return; }
                if (!cc.Inventory.Remove(3354673, 1, ss)) return;
                PT.Msg(cc, "You obtained " + PT.Give(cc, ss, "open 3354673", Reward.Timed(gear, (uint)n, true, 1440)) + " (1 day).");
            });
            PT.Say(d, "1D Solar Steel Box: choose your reward.");
            d.AddOption("2 1-day Solar Steels (1-handed) (B).", 1).AddOption("1 1-day Solar Steel (2-handed) (B).", 2).AddOption("Later.", 255).FinalizeDialog();
            return true;
        }
        /// <summary>Tiger / Leopard Chi Liquor (official tip): drink to receive 1,000 / 100 Chi Points.</summary>
        static bool Liquor(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s, int chi)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (c.Player.MyChi == null) { PT.Msg(c, "Unlock Chi first."); return true; }
                if (!c.Inventory.Remove(item.ITEM_ID, 1, s)) return true;
                c.Player.MyChi.ChiPoints = (int)Math.Min(int.MaxValue, (long)c.Player.MyChi.ChiPoints + chi);
                try { MsgChiInfo.MsgHandleChi.SendInfo(c, MsgChiInfo.Action.Upgrade); } catch { }
                PT.Log(c, "GRANT", "use " + item.ITEM_ID + " -> " + chi + " Chi Points");
                PT.Msg(c, "You received " + chi + " Chi Points.");
            }
            return true;
        }
        /// <summary>+3 Stone (B) gift (official tip): 1 5-day +3 Stone (B).</summary>
        static bool Stone3(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (!Kit.Known(730003) || !Kit.HasSpace(c, 1)) { PT.Msg(c, "Please make room in your inventory first."); return true; }
                if (!c.Inventory.Remove(item.ITEM_ID, 1, s)) return true;
                c.Inventory.Add(s, 730003, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 5 * 1440);
                PT.Log(c, "GRANT", "open 3346829 -> +3 Stone 730003 (5 days, bound)");
                PT.Msg(c, "You obtained 1 +3 Stone (B) (5 days).");
            }
            return true;
        }

        /// <summary>Octave Bell (video 08:50): "Use a single bell for a chance to double it." Options Use 1 bell / Not now.</summary>
        static bool Bell(Client.GameClient c, ServerSockets.Packet s)
        {
            uint bell = PT.Cfg.OctaveBell;
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o != 1) return;
                if (PT.Count(cc, bell) < 1 || !Kit.HasSpace(cc, 1)) { PT.Msg(cc, "You need 1 Octave Bell and 1 free slot."); return; }
                if (!Kit.Take(cc, bell, 1, ss)) return;
                if (Kit.Next(1, 100) <= PT.Cfg.BellDouble) { PT.Give(cc, ss, "bell doubled", new Reward(bell, 2, true)); PT.Msg(cc, "The bell rings twice! You obtained 2 Octave Bells."); }
                else { PT.Log(cc, "BELL", "used 1, not doubled"); PT.Msg(cc, "The bell fades into silence."); }
            });
            PT.Say(d, "It`s a tool used by Puppeteers to pacify rogue creations. Collect the Octave Bells to obtain the treasure left by the Puppeteer.\nUse a single bell for a chance to double it.");
            d.AddOption("Use 1 bell.", 1).AddOption("Not now.", 255).FinalizeDialog();
            return true;
        }
        /// <summary>Bell Dreamkey Box (official tip): choose 1 Octave Bell / 1 Dreamkey Fragment.</summary>
        static bool BellBox(Client.GameClient c, ServerSockets.Packet s)
        {
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o != 1 && o != 2) return;
                if (PT.Count(cc, 3354680) < 1 || !Kit.HasSpace(cc, 1)) { PT.Msg(cc, "Please make room in your inventory first."); return; }
                if (!cc.Inventory.Remove(3354680, 1, ss)) return;
                PT.Msg(cc, "You obtained " + PT.Give(cc, ss, "open 3354680", new Reward(o == 1 ? PT.Cfg.OctaveBell : 3354665u, 1, true)) + ".");
            });
            PT.Say(d, "Bell Dreamkey Box: choose your reward.");
            d.AddOption("1 Octave Bell.", 1).AddOption("1 Dreamkey Fragment.", 2).AddOption("Later.", 255).FinalizeDialog();
            return true;
        }

        /// <summary>Puppet Select Box (official tip): choose 1 Terror Weapon Pack (Timed) / 3D Puppet Avatar Pack / 7D Cute Emoji Pack / 1D Violet Dreamkey Box.</summary>
        static bool SelectBox(Client.GameClient c, ServerSockets.Packet s)
        {
            uint[] give = { 3600814, 3600815, 3349209, 3354674 };
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o < 1 || o > 4) return;
                if (o == 2 || o == 3) { PT.Msg(cc, "Avatar frames and emoji series aren`t available on this server yet. Please choose another reward."); return; }
                if (PT.Count(cc, 3600816) < 1 || !Kit.HasSpace(cc, 1)) { PT.Msg(cc, "Please make room in your inventory first."); return; }
                if (!cc.Inventory.Remove(3600816, 1, ss)) return;
                PT.Msg(cc, "You obtained " + PT.Give(cc, ss, "open 3600816", new Reward(give[o - 1], 1, true)) + ".");
            });
            PT.Say(d, "Puppet Select Box: choose 1 reward.");
            d.AddOption("1 Terror Weapon Pack (Timed).", 1).AddOption("1 3D Puppet Avatar Pack.", 2).AddOption("1 7D Cute Emoji Pack.", 3).AddOption("1 1D Violet Dreamkey Box.", 4).AddOption("Later.", 255).FinalizeDialog();
            return true;
        }
        /// <summary>1D Violet Dreamkey Box (official tip): choose 2 1-day Violet Dreamkeys (1-handed) (B) / 1 1-day Violet Dreamkey (2-handed) (B).</summary>
        static bool DreamkeyDay(Client.GameClient c, ServerSockets.Packet s)
        {
            var d = PT.Dialog(c, s, PT.DlgInfo, (cc, ss, o) =>
            {
                if (o != 1 && o != 2) return;
                uint gear = o == 1 ? 360468u : 350230u; int n = o == 1 ? 2 : 1;
                if (PT.Count(cc, 3354674) < 1 || !Kit.HasSpace(cc, n)) { PT.Msg(cc, "Please make room in your inventory first."); return; }
                if (!cc.Inventory.Remove(3354674, 1, ss)) return;
                PT.Msg(cc, "You obtained " + PT.Give(cc, ss, "open 3354674", Reward.Timed(gear, (uint)n, true, 1440)) + " (1 day).");
            });
            PT.Say(d, "1D Violet Dreamkey Box: choose your reward.");
            d.AddOption("2 1-day Violet Dreamkeys (1-handed) (B).", 1).AddOption("1 1-day Violet Dreamkey (2-handed) (B).", 2).AddOption("Later.", 255).FinalizeDialog();
            return true;
        }
        /// <summary>Terror Weapon Pack (Timed) (official tip): one random weapon accessory (Spectre Scythe / Ghostmare / Ghosthunter, 3 days, B).
        /// [2026-10-15] + the 7-day Butcher Knife (360044 ButcherKnife(1-Handed)).</summary>
        static bool WeaponPack(Client.GameClient c, ServerSockets.Packet s)
        {
            uint[] pool = { 360374, 360306, 350168, 360044 };
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (!Kit.HasSpace(c, 1)) { PT.Msg(c, "Please make room in your inventory first."); return true; }
                if (!c.Inventory.Remove(3600814, 1, s)) return true;
                uint w = pool[Kit.Next(0, pool.Length - 1)]; uint days = w == 360044 ? 7u : 3u;
                PT.Msg(c, "You obtained " + PT.Give(c, s, "open 3600814", Reward.Timed(w, 1, true, days * 1440)) + " (" + days + " days).");
            }
            return true;
        }

        static bool Equip(Client.GameClient c, MsgGameItem item, ServerSockets.Packet s, uint gear, int n, bool bound = false)
        {
            lock (Kit.Lock(c.Player.UID))
            {
                if (Kit.Throttled(c.Player.UID, "ptuse", 300)) return true;
                if (!Kit.Known(gear)) { PT.Msg(c, "This item can`t be opened on this server yet."); return true; }
                if (!Kit.HasSpace(c, n)) { PT.Msg(c, "Please make room in your inventory first."); return true; }
                uint id = item.ITEM_ID;
                if (!c.Inventory.Remove(id, 1, s)) return true;
                for (int i = 0; i < n; i++) c.Inventory.Add(s, gear, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);   // permanent, 1% blessed (official tip)
                PT.Log(c, "GRANT", "open " + id + " -> " + n + "x" + gear + " (-1 bless)");
                PT.Msg(c, "You obtained " + n + " " + Kit.Name(gear) + ".");
            }
            return true;
        }
    }
}
