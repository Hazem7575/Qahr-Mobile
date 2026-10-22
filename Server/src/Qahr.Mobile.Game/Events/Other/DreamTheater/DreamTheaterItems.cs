// ============================================================================
//  [DreamTheater 2026-10-15 video pass] what every Dream Theater item does when it is used (right-click):
//    Wish Gold Coin 3347476 / Wish Silver Coin 3600697  official MsgBox tWhiteDay2024NPC[Message][3347476|3600697]
//        "You can make 10 / 1 wish(es) at the Wish Fountain. Go there now?" -> Confirm opens the Dream Theater window with the
//        Wish Fountain (WhiteDayLua_OpenMain</N>Wish). After the event (item text): becomes 100 / 10 Chi Points.
//    Wish Twinkle Coin 3347478  item text "10 = 1 Wish Silver Coin, 100 = 1 Wish Gold Coin" -> Fluffy's exchanges; after the
//        event 10 Chi Points.
//    Love Velvet 3347321  official dialog tWhiteDay2024NPC[3347321] (video qiOK 13:26): mount armors (160 / 135), garments
//        (100 / 80 / 30), titles (60, Sincere Love Title Box), make up for short velvets with CPs, convert into Chi Points.
//    Dream Splendid / Starry / Glowy / Lucky Box 3347479-3347482 (local ranking mails): their official item texts.
//    Only Love / Sincere Love / True Love Title Box 3347516-3347518: the title of the item text, by gender.
//    Elite / Deluxe Flower Pack 3310215 / 3310216 (Fluffy): random Rose/Orchid/Lily/Tulip Certificates.
//  Every use: the item is checked and taken FIRST (bag space before), then the effect, then the window is refreshed.
//  Hooks (one line each): MY System\MsgItem\MsgItemUsuagePacket.cs UseItem (OnUse), Game\MsgNpc\Procesor.cs (OnItemDialog).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.DreamTheater
{
    public static partial class DreamTheaterModule
    {
        public const uint TitleOnlyLove = 3347516, TitleSincereLove = 3347517, TitleTrueLove = 3347518;
        static readonly uint[] RankBoxes = { 3347479, 3347480, 3347481, 3347482 };
        static readonly uint[] Certificates = { 3304134, 3304135, 3304136, 3304137 };// Rose / Orchid / Lily / Tulip Certificate

        static bool IsOurItem(uint id)
        {
            var cfg = Cfg;
            return id == cfg.GoldCoin || id == cfg.SilverCoin || id == cfg.TwinkleCoin || id == cfg.LoveVelvet
                   || cfg.Boxes.ContainsKey(id) || id == TitleOnlyLove || id == TitleSincereLove || id == TitleTrueLove || cfg.FlowerPacks.ContainsKey(id);
        }

        /// <summary>MsgItemUsuagePacket.UseItem hook. True = ours.</summary>
        public static bool OnUseItem(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null || !IsOurItem(item.ITEM_ID)) return false;
            try
            {
                if (Throttled(c, "use", 500)) return true;
                var cfg = Cfg; uint id = item.ITEM_ID;
                if (id == cfg.GoldCoin || id == cfg.SilverCoin || id == cfg.TwinkleCoin) { UseCoin(c, stream, id); return true; }
                if (id == cfg.LoveVelvet) { c.ActiveNpc = id; VelvetDialog(c, stream, 0); return true; }
                if (cfg.Boxes.ContainsKey(id)) { OpenRankBox(c, stream, id); return true; }
                if (id == TitleOnlyLove || id == TitleSincereLove || id == TitleTrueLove) { OpenTitleBox(c, stream, id); return true; }
                if (cfg.FlowerPacks.ContainsKey(id)) { OpenFlowerPack(c, stream, id); return true; }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        /// <summary>Procesor hook: the answers of the Love Velvet dialog (npcid = the item id). True = ours.</summary>
        public static bool OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null || npcid != Cfg.LoveVelvet) return false;
            try
            {
                if (option == 255) return true;
                Dlg st;
                if (!_dlg.TryGetValue(c.Player.UID, out st) || st.Npc != npcid || !st.Opts.Contains(option)) return true;
                _dlg.TryRemove(c.Player.UID, out st);
                if (Throttled(c, "velvet", 400)) return true;
                if (CountItem(c, npcid) == 0) { Msg(c, "You don`t have this item in your inventory."); return true; }
                VelvetDialog(c, stream, option);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return true;
        }

        static void Tip(Client.GameClient c, string text) { Lua(c, "Server_MsgTips|" + text); }
        static void AddChi(Client.GameClient c, long n)
        {
            if (c.Player.MyChi == null || n <= 0) return;
            c.Player.MyChi.ChiPoints = (int)Math.Min(int.MaxValue, (long)c.Player.MyChi.ChiPoints + n);
            try { MsgChiInfo.MsgHandleChi.SendInfo(c, MsgChiInfo.Action.Upgrade); } catch { }
        }
        static void RewardPopup(Client.GameClient c, IEnumerable<RItem> items)
        {
            Lua(c, "DlgPublicReward_OpenView|{['Code']=200,['Data']={}}");
            Lua(c, "DlgPublicReward_UpdateView|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", items.Select(x => Q(x.Id + " 0 " + x.Count))) + "},['IsFinish']=1}}");
        }

        // ------------------------------------------------------------------ coins
        static void UseCoin(Client.GameClient c, ServerSockets.Packet stream, uint id)
        {
            var cfg = Cfg;
            if (!Running)
            {
                // official item texts: after 7:59 (US) Mar 25 the coin becomes Chi Points
                int chi;
                if (!cfg.CoinChi.TryGetValue(id, out chi) || chi <= 0 || c.Player.MyChi == null) { Msg(c, "Dream Theater is not open now."); return; }
                lock (_lock)
                {
                    if (!TakeItem(c, id, 1, stream)) return;
                    AddChi(c, chi);
                }
                Log(c, "coin " + id + " -> " + chi + " Chi Points (event closed)");
                Tip(c, "Congrats! You successfully received " + chi + " Chi Points.");
                return;
            }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return; }
            if (id == cfg.TwinkleCoin)
            {
                Msg(c, "10 Wish Twinkle Coins can be exchanged for 1 Wish Silver Coin, and 100 Wish Twinkle Coins can be exchanged for 1 Wish Gold Coin. Talk to Fluffy [TheaterShop] in Twin City (367,444).");
                return;
            }
            string text = id == cfg.GoldCoin ? "You can make 10 wishes at the Wish Fountain. Go there now?" : "You can make 1 wish at the Wish Fountain. Go there now?";
            Lua(c, "Fui_Common_MsgBox_Open|{['Code']=200,['Data']={" + LuaMsg + ",{'',{}},{" + Q(text) + ",{}},{'',{}},{'',{}},{'',{}},{'',{}},{'',{}},-1,{'',{}},{"
                   + Q("WhiteDayLua_OpenMain</N>Wish") + ",{}},{'',{}},{'',{}}}}");
        }

        // ------------------------------------------------------------------ boxes
        static void OpenRankBox(Client.GameClient c, ServerSockets.Packet stream, uint id)
        {
            var items = Cfg.Boxes[id];
            lock (_lock)
            {
                int slots = SlotsFor(items);
                if (!Space(c, slots)) { Msg(c, "Please make " + slots + " free space(s) in your inventory first."); return; }
                if (!TakeItem(c, id, 1, stream)) return;
                string given = Give(c, items, stream);
                Log(c, "open " + ItemName(id) + " -> " + given);
            }
            RewardPopup(c, items);
            RefreshWindow(c);
        }
        static void OpenTitleBox(Client.GameClient c, ServerSockets.Packet stream, uint id)
        {
            bool girl = c.Player.IsGirl();
            // item texts "based on your character": the first title of the pair for female characters, the second for male (DERIVED order)
            var t = id == TitleOnlyLove ? (girl ? MsgTitleStorage.TitleType.WhereLoveBelongs : MsgTitleStorage.TitleType.WhereHeartLies)
                  : id == TitleSincereLove ? (girl ? MsgTitleStorage.TitleType.GracefulasWater : MsgTitleStorage.TitleType.VibrantasFire)
                  : (girl ? MsgTitleStorage.TitleType.WhiteSwanConcerto : MsgTitleStorage.TitleType.BlackSwanSymphony);
            bool known = false; try { known = Database.TitleStorage.Titles.ContainsKey((uint)t); } catch { }
            if (!known) { Msg(c, "This title isn`t available on this server yet. Your box was not used."); return; }
            if (c.Player.SpecialTitles.Contains(t)) { Msg(c, "Sorry, you can't use this item again since you've already got this appearance."); return; }
            lock (_lock)
            {
                if (!TakeItem(c, id, 1, stream)) return;
                c.Player.AddSpecialTitle(t, stream);
            }
            Log(c, "title box " + id + " -> " + t);
            Tip(c, "You`ve got a permanent title, " + TitleName(t) + ".");
        }
        static string TitleName(MsgTitleStorage.TitleType t)
        {
            switch (t)
            {
                case MsgTitleStorage.TitleType.WhereLoveBelongs: return "Where Love Belongs";
                case MsgTitleStorage.TitleType.WhereHeartLies: return "Where Heart Lies";
                case MsgTitleStorage.TitleType.GracefulasWater: return "Graceful as Water";
                case MsgTitleStorage.TitleType.VibrantasFire: return "Vibrant as Fire";
                case MsgTitleStorage.TitleType.WhiteSwanConcerto: return "White Swan Concerto";
                case MsgTitleStorage.TitleType.BlackSwanSymphony: return "Black Swan Symphony";
            }
            return t.ToString();
        }
        static readonly Random _packRnd = new Random();
        static void OpenFlowerPack(Client.GameClient c, ServerSockets.Packet stream, uint id)
        {
            var mm = Cfg.FlowerPacks[id];
            int n; uint cert;
            lock (_packRnd) { n = _packRnd.Next(Math.Max(1, mm[0]), Math.Max(mm[0], mm[1]) + 1); cert = Certificates[_packRnd.Next(Certificates.Length)]; }
            var items = new List<RItem> { new RItem { Id = cert, Count = (ushort)Math.Min(10000, n) } };
            lock (_lock)
            {
                int slots = SlotsFor(items);
                if (!Space(c, slots)) { Msg(c, "Please make " + slots + " free space(s) in your inventory first."); return; }
                if (!TakeItem(c, id, 1, stream)) return;
                string given = Give(c, items, stream);
                Log(c, "open " + ItemName(id) + " -> " + given);
            }
            RewardPopup(c, items);
        }

        // ------------------------------------------------------------------ Love Velvet (official tWhiteDay2024NPC[3347321])
        const byte VMount = 1, VGarment = 2, VTitle = 3, VChi = 4, VChiYes = 5, VBuyBase = 20, VMakeUpBase = 40;
        static readonly ConcurrentDictionary<uint, uint> _makeUp = new ConcurrentDictionary<uint, uint>();
        static void VelvetDialog(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            var cfg = Cfg; uint vel = cfg.LoveVelvet;
            var d = new Game.MsgNpc.Dialog(c, stream);
            var st = NewDlg(c, vel);
            c.ActiveNpc = vel;
            uint have = CountItem(c, vel);
            if (option == 0)
            {
                Text(d, "Wonderful love is always fascinating, and the fruit of love requires careful management. The fancy Love Velvet is the best material for managing the fruit of love.");
                Text(d, "----------Swan Lake Romance Reward---------- [Fruit of Love] [Love Velvets Consumed] " + string.Join(", ", cfg.VelvetGoods.Select(g => ItemName(g.Item) + " " + g.Cost)) + ". You have " + have + " Love Velvet(s).");
                Opt(d, st, "Exchange for mount armors.", VMount);
                Opt(d, st, "Exchange for garments.", VGarment);
                Opt(d, st, "Exchange for titles.", VTitle);
                Opt(d, st, "Convert into Chi Points.", VChi);
                Opt(d, st, "Think twice.", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (option == VMount || option == VGarment || option == VTitle)
            {
                Text(d, option == VMount ? "Please choose the mount armor you want." : option == VGarment ? "Please choose the garment you want." : "Please choose the title you want.");
                for (int i = 0; i < cfg.VelvetGoods.Count; i++)
                    if (cfg.VelvetGoods[i].Group == option) Opt(d, st, ItemName(cfg.VelvetGoods[i].Item) + " (" + cfg.VelvetGoods[i].Cost + ")", (byte)(VBuyBase + i));
                Opt(d, st, "End", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (option == VChi)
            {
                Text(d, "Are you sure about converting all the remaining Love Velvets to Chi Points? (" + have + " x " + cfg.VelvetChi + " Chi Points)");
                Opt(d, st, "Yes.", VChiYes);
                Opt(d, st, "Cancel.", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (option == VChiYes)
            {
                _dlg.TryRemove(c.Player.UID, out st);
                if (have == 0 || c.Player.MyChi == null) { Msg(c, "Since you don`t have enough Love Velvets, you cannot convert them into Chi Points."); return; }
                long chi;
                lock (_lock)
                {
                    if (!TakeItem(c, vel, have, stream)) return;
                    chi = (long)have * cfg.VelvetChi;
                    AddChi(c, chi);
                }
                Log(c, "velvet " + have + " -> " + chi + " Chi Points");
                Tip(c, "Congrats! You successfully received " + chi + " Chi Points.");
                return;
            }
            if (option >= VBuyBase && option < VBuyBase + cfg.VelvetGoods.Count)
            {
                var g = cfg.VelvetGoods[option - VBuyBase];
                if (have >= g.Cost) { _dlg.TryRemove(c.Player.UID, out st); BuyVelvetGood(c, stream, g, 0); return; }
                uint missing = g.Cost - have, cps = missing * cfg.VelvetMakeUpCps;
                _makeUp[c.Player.UID] = g.Item;
                Text(d, "You don`t have enough Love Velvets to exchange for the " + ItemName(g.Item) + ". You don`t have enough Love Velvets. Pay " + cps + " CPs to make up?");
                Opt(d, st, "Make up for short velvets.", (byte)(VMakeUpBase + option - VBuyBase));
                Opt(d, st, "Think about it.", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (option >= VMakeUpBase && option < VMakeUpBase + cfg.VelvetGoods.Count)
            {
                _dlg.TryRemove(c.Player.UID, out st);
                var g = cfg.VelvetGoods[option - VMakeUpBase];
                uint want; if (!_makeUp.TryRemove(c.Player.UID, out want) || want != g.Item) return;
                BuyVelvetGood(c, stream, g, cfg.VelvetMakeUpCps);
            }
        }
        /// <summary>Takes the velvets (and the CPs for the short ones when makeUpCps &gt; 0), then gives 1 good.</summary>
        static void BuyVelvetGood(Client.GameClient c, ServerSockets.Packet stream, VelvetGood g, uint makeUpCps)
        {
            var cfg = Cfg; uint vel = cfg.LoveVelvet;
            lock (_lock)
            {
                uint have = CountItem(c, vel);
                uint use = Math.Min(have, g.Cost), missing = g.Cost - use, cps = missing * makeUpCps;
                if (missing > 0 && makeUpCps == 0) { Msg(c, "You don`t have enough Love Velvets to exchange for the " + ItemName(g.Item) + "."); return; }
                if (cps > 0 && c.Player.ConquerPoints < cps) { Msg(c, "Failed to make up for the short Love Velvets for lack of CPs."); return; }
                if (!Space(c, 1)) { Msg(c, "Please make 1 space in your inventory before activating this item."); return; }
                if (use > 0 && !TakeItem(c, vel, use, stream)) return;
                if (cps > 0) c.Player.ConquerPoints -= cps;
                string given = Give(c, new List<RItem> { new RItem { Id = g.Item, Count = 1 } }, stream);
                Log(c, "velvet exchange " + use + " velvets + " + cps + " CPs -> " + given);
            }
            Tip(c, "Congrats! You received 1 " + ItemName(g.Item) + ".");
            RewardPopup(c, new[] { new RItem { Id = g.Item, Count = 1 } });
        }
    }
}
