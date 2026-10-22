// ============================================================================
//  [ChildrensDay2025 2026-10-15] Dream Park NPCs (placed by Infinity[DB]\Festivals\ChildrensDay2025\npcs.txt, ids 8866001-8866013):
//    8866001 Suyi (official NpcX 110102, script tChildren2025Npc[34299]) Twin City: guide + today's progress, Open Dream Park,
//            Dream Store (official NewExchangeShopInfo[342501]), VIP Dream Store ([342502], needs 15000 Dream Points), points & ranking.
//    8866002 Prince Puff (NpcX 110104, tChildrenDay2025Part5[34250]) Twin City: park merchandise (EmoneySaleShopInfo[34250], CPs).
//    8866003 Flora (NpcX 110103, tChildren2025Npc[34300]) Vault Palace 12818: Starlight Blessing progress, back to Twin City.
//    8866011..13 Saluting Gun / Firework / Balloon (npc.ini 10181/10182/10183) Vault Palace: the 3 daily Starlight Blessings.
//  Texts = official Server_Key lines. Goods/prices/limits = official tables (Infinity[DB]\ChildrensDay2025\stores.json).
//  DERIVED: NPC spots (no official coordinates in the client; walkable cells checked on the DMap), the Starlight Blessing
//  "mystery reward" (= 1 Starwish Pack, an official event item), store limits counted per character per festival run.
//  Routing: one hook line in Game\MsgNpc\Procesor.cs before FestivalManager.OnNpc (these ids are festival NPCs).
//  Security: the option byte must belong to the menu this module sent (per-uid pending state, used once); store purchases
//  re-check festival, distance, level, VIP points, limit, balance and bag space under DreamParkState.Sync, take the cost
//  first, record the count, then give through Inventory; 300 ms throttle.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;
using Cfg_ = Qahr.Mobile.ChildrensDay2025.DreamParkConfig;

namespace Qahr.Mobile.ChildrensDay2025
{
    public static class DreamParkNpcs
    {
        public const uint Suyi = 8866001, PrincePuff = 8866002, Flora = 8866003, SalutingGun = 8866011, Firework = 8866012, Balloon = 8866013;
        public static bool Mine(uint id) { return id == Suyi || id == PrincePuff || id == Flora || (id >= SalutingGun && id <= Balloon); }

        enum Stage { Main, List, Item }
        sealed class Pending { public uint Npc; public Stage Stage; public int Store; public int Page; public int Good; public DateTime Expires; }
        static readonly ConcurrentDictionary<uint, Pending> _pending = new ConcurrentDictionary<uint, Pending>();
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        public static Action<uint, string> Trace;// harness: every dialog text/option the module sends
        public static bool NoThrottle;

        const byte OptPark = 1, OptStore = 2, OptVip = 3, OptRank = 4, OptPuff = 5, OptTc = 6, OptBack = 7;
        const byte OptGoodBase = 10;     // 10..17 = goods on the page
        const byte OptNext = 20, OptPrev = 21;
        const byte OptBuy1 = 31, OptBuy10 = 32, OptClose = 255;
        const int PerPage = 8;

        sealed class D
        {
            readonly Game.MsgNpc.Dialog _d; readonly uint _uid;
            public D(Client.GameClient c, ServerSockets.Packet s) { _d = new Game.MsgNpc.Dialog(c, s); _uid = c.Player.UID; }
            public D T(string t)
            {
                var tr = Trace; if (tr != null) tr(_uid, "T " + t);
                t = (t ?? "").Trim('\n');
                while (t.Length > 0) { int n = Math.Min(t.Length, 200); _d.AddText(t.Substring(0, n)); t = t.Substring(n); }
                return this;
            }
            public D O(string t, byte id) { var tr = Trace; if (tr != null) tr(_uid, "O" + id + " " + t); _d.AddOption(t.Length > 60 ? t.Substring(0, 57) + "..." : t, id); return this; }
            public void End(ushort avatar = 10) { _d.AddAvatar(avatar).FinalizeDialog(); }
        }
        static void Msg(Client.GameClient c, string t) { var tr = Trace; if (tr != null) tr(c.Player.UID, "M " + t); try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        static bool Throttled(uint uid)
        {
            if (NoThrottle) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[uid] = now;
            return false;
        }

        static Festivals.FestivalNpc NpcDef(uint id)
        {
            var r = Festivals.FestivalManager.Running(Cfg_.Cfg.Festival);
            return r == null ? null : r.Def.Npcs.FirstOrDefault(n => n.Uid == id);
        }
        static bool Near(Client.GameClient c, uint id)
        {
            var n = NpcDef(id);
            return n != null && c.Player.Map == n.Map && Math.Max(Math.Abs(c.Player.X - n.X), Math.Abs(c.Player.Y - n.Y)) <= 18;
        }

        /// <summary>Procesor hook. True = handled.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null || !Mine(npcid)) return false;
            try
            {
                if (!ChildrensDay2025Ui.Running) return false;// festival engine answers (nothing spawned anyway)
                if (Throttled(c.Player.UID)) return true;
                var cfg = Cfg_.Cfg;
                if (option == 0) { Pending old; _pending.TryRemove(c.Player.UID, out old); }
                if (option == OptClose) { Pending old; _pending.TryRemove(c.Player.UID, out old); return true; }
                if (!Near(c, npcid)) return true;
                if (npcid == SalutingGun || npcid == Firework || npcid == Balloon) { Blessing(c, stream, cfg, (int)(npcid - SalutingGun) + 1); return true; }
                if (c.Player.Level < cfg.MinLevel)
                {
                    new D(c, stream).T(npcid == PrincePuff ? "Prince Puff:\nPlease upgrade to level 80 first.\n" : "Suyi:\nPlease upgrade to level 80 first.\n").O("Level up first.", OptClose).End();
                    return true;
                }
                lock (DreamParkState.Sync)
                {
                    if (option == 0) { MainMenu(c, stream, cfg, npcid); return true; }
                    Pending p;
                    if (!_pending.TryGetValue(c.Player.UID, out p) || p.Npc != npcid || p.Expires < DateTime.UtcNow) return true;
                    Route(c, stream, cfg, npcid, p, option);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        static void Arm(uint uid, uint npc, Stage st, int store = 0, int page = 0, int good = -1)
        {
            _pending[uid] = new Pending { Npc = npc, Stage = st, Store = store, Page = page, Good = good, Expires = DateTime.UtcNow.AddMinutes(5) };
        }

        static void MainMenu(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, uint npc)
        {
            var p = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
            if (npc == Suyi)
            {
                new D(c, stream)
                    .T("Suyi:\nFerris wheels turn with laughter and carousels carry nursery rhymes. To attract visitors and prevent accidents, please manage the park diligently and make it famous!\n")
                    .T(" ------------------*Management Guide*------------------\n")
                    .T("1. Visit Dream Park daily to complete Dream Quests;\n")
                    .T("2. Completing daily quests rewards Starwish Stones and 1-3 Sweet Fragments;\n")
                    .T("3. Each park renovation grants 50 Dream Points and Moonglasses;\n")
                    .T("4. Accumulate Dream Points for local/global Dream Ranking;\n")
                    .T("5. VIP Dream Store unlocks at 15,000 Dream Points;\n")
                    .T("6. Perform 1 daily park operation for operation rewards;\n7. Visit Vault Palace for Starlight Blessing and special rewards.\n")
                    .T("------------------*Today`s Dream Progress*------------------\n")
                    .T("[Dream Quest] Today`s quests completed: 0/3;\n")
                    .T("[Vault Palace] Today`s visit completed: " + (p.BlessMask != 0 ? 1 : 0) + "/1;\n")
                    .T("[Starlight Blessing] Blessing rewards obtained today: " + (p.BlessDone ? 1 : 0) + "/1;\n")
                    .T("[Park Operation] Today`s operation completed: " + (p.OpDay == DreamParkState.Today ? 1 : 0) + "/1;\n")
                    .T("Your Dream Points: " + p.Points + "\n")
                    .O("Open Dream Park.", OptPark).O("Open Dream Store.", OptStore).O("Open VIP Dream Store.", OptVip)
                    .O("View points and ranking rewards.", OptRank).O("Leave.", OptClose).End();
                Arm(c.Player.UID, npc, Stage.Main);
            }
            else if (npc == PrincePuff)
            {
                new D(c, stream)
                    .T("Prince Puff:\nDream Park is opening! Full of childhood wonder and fantasy! Come get the trendiest park merchandise and cool collectibles to boost your management and stay ahead!\n")
                    .O("Purchase park merchandise.", OptPuff).O("Leave.", OptClose).End();
                Arm(c.Player.UID, npc, Stage.Main);
            }
            else if (npc == Flora)
            {
                string[] n = { "Saluting Gun", "Firework", "Balloon" };
                var d = new D(c, stream)
                    .T("Flora:\nThere are Starlight Blessings inside Vault Palace. Activating these blessings will earn you a lucky star. Collect all the lucky stars to obtain the surprise reward.\n")
                    .T("------------------------Starlight Blessing------------------------\n")
                    .T("    [Starlight Blessing]     [Progress]\n");
                for (int i = 0; i < 3; i++) d.T("     " + n[i] + "            " + ((p.BlessMask & (1 << i)) != 0 ? 1 : 0) + "/1\n");
                d.T("--------------------------------------------------------\n")
                 .O("Return to Twin City.", OptTc).O("Leave.", OptClose).End();
                Arm(c.Player.UID, npc, Stage.Main);
            }
        }

        static List<DreamGood> StoreOf(Cfg_ cfg, int store) { return store == 1 ? cfg.DreamStore : store == 2 ? cfg.VipStore : cfg.PuffStore; }
        static string StoreName(int store) { return store == 1 ? "Dream Store" : store == 2 ? "VIP Dream Store" : "Park Merchandise"; }

        static void Route(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, uint npc, Pending p, byte option)
        {
            var st = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
            if (p.Stage == Stage.Main)
            {
                if (npc == Suyi && option == OptPark) { Pending u; _pending.TryRemove(c.Player.UID, out u); LuaUi.LuaPush.Send(c, ChildrensDay2025Ui.OpenCall(c, cfg), ChildrensDay2025Ui.LuaMsg); return; }
                if (npc == Suyi && option == OptStore) { ShowList(c, stream, cfg, npc, 1, 0); return; }
                if (npc == Suyi && option == OptVip)
                {
                    if (st.Points < cfg.VipPoints)
                    {
                        new D(c, stream).T("Suyi:\nThe VIP Dream Store unlocks at 15,000 Dream Points. You have " + st.Points + " Dream Points now.\n").O("I see.", OptClose).End();
                        _pending.TryRemove(c.Player.UID, out p); return;
                    }
                    ShowList(c, stream, cfg, npc, 2, 0); return;
                }
                if (npc == Suyi && option == OptRank)
                {
                    _pending.TryRemove(c.Player.UID, out p);
                    LuaUi.LuaPush.Send(c, ChildrensDay2025Ui.PointCall(c, cfg), ChildrensDay2025Ui.LuaMsg); return;
                }
                if (npc == PrincePuff && option == OptPuff) { ShowList(c, stream, cfg, npc, 3, 0); return; }
                if (npc == Flora && option == OptTc)
                {
                    _pending.TryRemove(c.Player.UID, out p);
                    var o = QuestEngine.QuestEngineCore.TeleportOverride;
                    if (o != null) o(c, cfg.SuyiX, cfg.SuyiY, 1002, 0); else c.Teleport(cfg.SuyiX, cfg.SuyiY, 1002, 0);
                    return;
                }
                return;
            }
            var list = StoreOf(cfg, p.Store);
            if (p.Stage == Stage.List)
            {
                if (option == OptNext) { ShowList(c, stream, cfg, npc, p.Store, p.Page + 1); return; }
                if (option == OptPrev) { ShowList(c, stream, cfg, npc, p.Store, p.Page - 1); return; }
                if (option == OptBack) { MainMenu(c, stream, cfg, npc); return; }
                int idx = p.Page * PerPage + (option - OptGoodBase);
                if (option < OptGoodBase || option >= OptGoodBase + PerPage || idx < 0 || idx >= list.Count) return;
                ShowGood(c, stream, cfg, npc, p.Store, p.Page, idx); return;
            }
            if (p.Stage == Stage.Item)
            {
                if (option == OptBack) { ShowList(c, stream, cfg, npc, p.Store, p.Page); return; }
                if (option != OptBuy1 && option != OptBuy10) return;
                if (p.Good < 0 || p.Good >= list.Count) return;
                int store = p.Store, page = p.Page, good = p.Good;
                Pending used; _pending.TryRemove(c.Player.UID, out used);// one purchase per menu
                string r = Buy(c, stream, cfg, npc, store, list[good], option == OptBuy10 ? 10u : 1u);
                new D(c, stream).T(r).O("Back to the list.", OptBack).O("Leave.", OptClose).End();
                Arm(c.Player.UID, npc, Stage.Item, store, page, -1);// only "Back" is valid now
            }
        }

        static string Cost(DreamGood g, uint n) { return (g.Cost * n) + " " + (g.CostItem == 0 ? "CPs" : QuestEngine.QuestGrammar.ItemName(g.CostItem)); }
        static uint Bought(DreamPlayer st, DreamGood g) { uint b; return st.Bought.TryGetValue(g.Goods, out b) ? b : 0; }

        static void ShowList(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, uint npc, int store, int page)
        {
            var list = StoreOf(cfg, store);
            var st = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
            int pages = Math.Max(1, (list.Count + PerPage - 1) / PerPage);
            page = Math.Max(0, Math.Min(pages - 1, page));
            var d = new D(c, stream).T((npc == PrincePuff ? "Prince Puff" : "Suyi") + ":\n" + StoreName(store) + " (page " + (page + 1) + "/" + pages + ")"
                + (store == 3 ? ". You have " + c.Player.ConquerPoints + " CPs.\n" : ". You have " + DreamParkItems.Count(c, Cfg_.Moonglass) + " Moonglasses.\n"));
            for (int i = 0; i < PerPage; i++)
            {
                int idx = page * PerPage + i; if (idx >= list.Count) break;
                var g = list[idx];
                string lim = g.Limit == 0 ? "" : " [" + Bought(st, g) + "/" + g.Limit + "]";
                d.O(QuestEngine.QuestGrammar.ItemName(g.Item) + " - " + Cost(g, 1) + lim, (byte)(OptGoodBase + i));
            }
            if (page + 1 < pages) d.O("Next page.", OptNext);
            if (page > 0) d.O("Previous page.", OptPrev);
            d.O("Back.", OptBack).End();
            Arm(c.Player.UID, npc, Stage.List, store, page);
        }

        static void ShowGood(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, uint npc, int store, int page, int idx)
        {
            var g = StoreOf(cfg, store)[idx];
            var st = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
            uint left = g.Limit == 0 ? uint.MaxValue : g.Limit - Math.Min(g.Limit, Bought(st, g));
            var d = new D(c, stream).T(QuestEngine.QuestGrammar.ItemName(g.Item) + (g.Bound ? " (bound)" : "") + ": " + Cost(g, 1) + " each."
                + (g.Limit == 0 ? "" : " You can buy " + left + " more during this event.") + "\n");
            if (left >= 1) d.O("Buy 1 (" + Cost(g, 1) + ").", OptBuy1);
            if (left >= 10) d.O("Buy 10 (" + Cost(g, 10) + ").", OptBuy10);
            d.O("Back to the list.", OptBack).End();
            Arm(c.Player.UID, npc, Stage.Item, store, page, idx);
        }

        /// <summary>One purchase; everything re-checked here. Caller holds DreamParkState.Sync.</summary>
        public static string Buy(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, uint npc, int store, DreamGood g, uint n)
        {
            if (!ChildrensDay2025Ui.Running) return ChildrensDay2025Ui.TxtOver;
            if (n != 1 && n != 10) return "That offer is gone.";
            if (!Near(c, npc)) return "Come closer.";
            if (c.Player.Level < cfg.MinLevel) return ChildrensDay2025Ui.TxtLevel;
            var st = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
            if (store == 2 && st.Points < cfg.VipPoints) return "The VIP Dream Store unlocks at 15,000 Dream Points.";
            if (!StoreOf(cfg, store).Contains(g)) return "That offer is gone.";
            if (!DreamParkItems.Exists(g.Item)) return "That offer is gone.";
            uint bought = Bought(st, g);
            if (g.Limit != 0 && (ulong)bought + n > g.Limit) return "You can`t buy that many any more (" + bought + "/" + g.Limit + ").";
            ulong cost = (ulong)g.Cost * n;
            if (g.CostItem == 0) { if (c.Player.ConquerPoints < cost) return "You do not have enough CPs."; }
            else if (DreamParkItems.Count(c, g.CostItem) < cost) return "You need " + cost + " " + QuestEngine.QuestGrammar.ItemName(g.CostItem) + ".";
            var give = new DreamParkItems.Grant(g.Item, n, g.Bound);
            int slots = DreamParkItems.Slots(new[] { give });
            if (!DreamParkItems.HaveSpace(c, slots)) return "Your inventory is full. Please make " + slots + " space(s) first!";
            // pay first
            if (g.CostItem == 0) c.Player.ConquerPoints -= (uint)cost;
            else if (!DreamParkItems.Take(c, g.CostItem, (uint)cost, stream)) return "You need " + cost + " " + QuestEngine.QuestGrammar.ItemName(g.CostItem) + ".";
            st.Bought[g.Goods] = bought + n;
            DreamParkState.Save();
            string got = DreamParkItems.Give(c, give, stream);
            DreamParkState.Log(c, "BUY " + StoreName(store) + " goods " + g.Goods + " x" + n + " paid " + Cost(g, n) + " -> " + got);
            return "Done! You received " + got + ".";
        }

        // ------------------------------------------------------------------ Starlight Blessing (Vault Palace)
        static void Blessing(Client.GameClient c, ServerSockets.Packet stream, Cfg_ cfg, int k)
        {
            if (c.Player.Level < cfg.MinLevel) { Msg(c, ChildrensDay2025Ui.TxtLevel); return; }
            string[] act = { "", "You launched a Saluting Gun! Collect {0} more Lucky Star(s) for the mystery reward!", "You lit a Firework! Collect {0} more Lucky Star(s) for the mystery reward!", "You released a Balloon! Collect {0} more Lucky Star(s) for the mystery reward!" };
            lock (DreamParkState.Sync)
            {
                var p = DreamParkState.Get(c.Player.UID, c.Player.Name, ChildrensDay2025Ui.Run);
                if (p.BlessDone) { Msg(c, "You`ve completed today`s Starlight Blessing."); return; }
                if ((p.BlessMask & (1 << (k - 1))) != 0) { Msg(c, "You already activated this blessing today."); return; }
                p.BlessMask |= 1 << (k - 1);
                int left = 3 - new[] { 1, 2, 4 }.Count(b => (p.BlessMask & b) != 0);
                if (left > 0) { DreamParkState.Save(); Msg(c, string.Format(act[k], left)); DreamParkState.Log(c, "BLESS " + k + " mask=" + p.BlessMask); return; }
                var give = new DreamParkItems.Grant(Cfg_.StarwishPack, 1);
                if (!DreamParkItems.HaveSpace(c, 1)) { p.BlessMask &= ~(1 << (k - 1)); Msg(c, "Your inventory is full. Please make 1 space(s) first!"); return; }
                p.BlessDone = true;// recorded before the reward
                DreamParkState.Save();
                string got = DreamParkItems.Give(c, give, stream);
                DreamParkState.Log(c, "BLESS done -> " + got);
                Msg(c, string.Format(act[k], 0));
                Msg(c, "You`ve completed today`s Starlight Blessing and received " + got + "! Talk to Flora to return to Twin City.");
            }
        }
    }
}
