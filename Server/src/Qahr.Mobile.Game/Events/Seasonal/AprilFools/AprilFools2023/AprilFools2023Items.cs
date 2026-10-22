// ============================================================================
//  [AprilFools2023 2026-10-15] Right-click handlers of the official "Trick Maze" items (itemtype texts are the rules):
//    3341221 Trick Amulet          "Use it to randomly get clown treasures."            -> Dungeon Breaker draw (+ Trick Points)
//    3341225 Trick Amulet Fragment "Combine 5 fragments into 1 Trick Amulet."
//    3341222 Maze Pearl Pack       "Open to get a Maze Pearl."
//    3600462-3600470 Clown Masks   "Submit it to Lyon to get 10-15 Trick Amulets and <item>." ("Right click to submit.")
//    3341304 Joker Knight Fragment "Combine 30 into Joker Knight (B), 50 into Joker Knight (Eerie)."
//    3341305 Playful Player Fragment "... 30 fragments into a permanent Playful Player." (the 30/90-day titles have no timed-title
//            support on this server: only the permanent one is offered)
//    3341270 Perfect Dice / 3341223 Maze Key / 3341089 Clown Key: used inside the maze (message only).
//  All only while the festival runs (the official items expire with the event). Bag space first, then take, then grant.
// ============================================================================
using System;
using System.Collections.Generic;
using Qahr.Mobile.FoolsKit;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.AprilFools2023
{
    public static class AprilFools2023Items
    {
        static readonly List<Reward>[] MaskGift =
        {
            new List<Reward> { new Reward(3333529, 5, true) },   // 5 bottles of Leopard Chi Liquor (B)
            new List<Reward> { new Reward(3333528, 1, true) },   // 1 Ginseng Essence (B)
            new List<Reward> { new Reward(730003, 1, true) },    // 1 +3 Stone (B)
            new List<Reward> { new Reward(3009001, 1) },         // 1 Bright Star Stone
            new List<Reward> { new Reward(3334365, 10, true) },  // 10 Universal Rune Essences (B)
            new List<Reward> { new Reward(3009102, 1) },         // 1 Moon Fruit
            new List<Reward> { new Reward(3336211, 2) },         // 2 Awakening Item Packs
            new List<Reward> { new Reward(3322760, 1, true) },   // 1 3KK Contribution Medal (B)
            null,                                                // Arrogant: 3 Peonies x2 (gentlemen) / 3 Marbles x2 (ladies)
        };

        static bool Mine(uint id)
        {
            return id == AprilFools2023Ui.TrickAmulet || id == AprilFools2023Ui.TrickFragment || id == 3341222 || id == AprilFools2023Ui.JokerFragment
                || id == AprilFools2023Ui.PlayfulFragment || id == AprilFools2023Ui.PerfectDice || id == AprilFools2023Ui.MazeKey || id == AprilFools2023Ui.ClownKey
                || (id >= AprilFools2023Ui.MaskFirst && id <= AprilFools2023Ui.MaskFirst + 8);
        }

        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            uint id = item.ITEM_ID;
            if (!Mine(id)) return false;
            if (!AprilFools2023Ui.Running) { Kit.Msg(c, "Sorry, the event has not started yet. You can`t use this item."); return true; }
            if (Kit.Throttled(c.Player.UID, "af23use", 500)) return true;
            var cfg = AprilFools2023Ui.Cfg;
            if (c.Player.Level < cfg.MinLevel) { Kit.Msg(c, "Please upgrade to level " + cfg.MinLevel + " first."); return true; }

            if (id == AprilFools2023Ui.TrickAmulet) { AprilFools2023Ui.UseAmulets(c, stream, 1); return true; }
            if (id == AprilFools2023Ui.PerfectDice) { Kit.Msg(c, "The Perfect Dice can only be used in the chess game in Trick Maze. You can explore the maze first."); return true; }
            if (id == AprilFools2023Ui.MazeKey) { Kit.Msg(c, "It can break the seal of the maze. Open Tricky Wit and enter the maze to use it."); return true; }
            if (id == AprilFools2023Ui.ClownKey) { Kit.Msg(c, "Use it to open the treasure box in the Mystery Room of Trick Maze."); return true; }

            lock (Kit.Lock(c.Player.UID))
            {
                if (id == AprilFools2023Ui.TrickFragment)
                {
                    Convert(c, stream, id, 5, new List<Reward> { new Reward(AprilFools2023Ui.TrickAmulet, 1) }, "Failed to combine due to insufficient fragments.");
                    return true;
                }
                if (id == 3341222)
                {
                    Convert(c, stream, id, 1, new List<Reward> { new Reward(AprilFools2023Ui.MazePearl, 1) }, "");
                    return true;
                }
                if (id >= AprilFools2023Ui.MaskFirst && id <= AprilFools2023Ui.MaskFirst + 8)
                {
                    int i = (int)(id - AprilFools2023Ui.MaskFirst);
                    var gift = new List<Reward> { new Reward(AprilFools2023Ui.TrickAmulet, (uint)Kit.Next(10, 15)) };
                    if (i == 8) gift.Add(new Reward(c.Player.IsGirl() ? 759003u : 761003u, 2));
                    else gift.AddRange(MaskGift[i]);
                    Convert(c, stream, id, 1, gift, "");
                    return true;
                }
                if (id == AprilFools2023Ui.JokerFragment || id == AprilFools2023Ui.PlayfulFragment)
                {
                    CombineDialog(c, stream, id);
                    return true;
                }
            }
            return true;
        }

        /// <summary>Takes n of the item and grants the list (space first).</summary>
        static void Convert(Client.GameClient c, ServerSockets.Packet stream, uint id, uint n, List<Reward> gift, string notEnough)
        {
            if (Kit.Count(c, id) < n) { Kit.Msg(c, notEnough.Length > 0 ? notEnough : "Failed to combine due to insufficient items."); return; }
            int slots = Kit.Slots(gift);
            if (!Kit.HasSpace(c, slots)) { Kit.Msg(c, "Your inventory is full. Please make " + slots + " space(s) first."); return; }
            if (!Kit.Take(c, id, n, stream)) return;
            string got = Kit.Give(c, gift, stream, AprilFools2023Ui.Festival, "use " + id + " x" + n);
            Kit.Msg(c, "Congrats! You received " + got + ".");
        }

        // ------------------------------------------------------------------ Joker Knight / Playful Player fragments
        static void CombineDialog(Client.GameClient c, ServerSockets.Packet stream, uint id)
        {
            uint have = Kit.Count(c, id);
            var d = Kit.Dialog(c, stream, AprilFools2023Ui.DlgCombine, OnCombine);
            if (id == AprilFools2023Ui.JokerFragment)
            {
                d.AddText("Joker Knight Fragments owned: " + have + "\nCombine 30 fragments into a 3-star garment, Joker Knight (B), and 50 fragments will get you a 5-star garment, Joker Knight (Eerie).\n");
                d.AddOption("Combine 30 into Joker Knight (B).", 1).AddOption("Combine 50 into Joker Knight (Eerie).", 2);
            }
            else
            {
                d.AddText("Playful Player Fragments Owned: " + have + "\nCombine 30 fragments into 1 permanent title, Playful Player.\n");
                d.AddOption("Get permanent Playful Player.", 3);
            }
            d.AddOption("Choose again.", 255).FinalizeDialog();
        }
        static bool OnCombine(Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            if (!AprilFools2023Ui.Running || option < 1 || option > 3) return true;
            if (Kit.Throttled(c.Player.UID, "af23comb", 800)) return true;
            lock (Kit.Lock(c.Player.UID))
            {
                if (option == 1) Convert(c, stream, AprilFools2023Ui.JokerFragment, 30, new List<Reward> { new Reward(191475, 1, true) }, "Failed to combine due to insufficient fragments.");
                else if (option == 2) Convert(c, stream, AprilFools2023Ui.JokerFragment, 50, new List<Reward> { new Reward(191485, 1) }, "Failed to combine due to insufficient fragments.");
                else
                {
                    var t = MsgTitleStorage.TitleType.PlayfulPlayer;
                    bool known = false; try { known = Database.TitleStorage.Titles.ContainsKey((uint)t); } catch { }
                    if (!known) { Kit.Msg(c, "The Playful Player title isn`t available on this server yet. Your fragments were not used."); return true; }
                    if (c.Player.SpecialTitles.Contains(t)) { Kit.Msg(c, "Sorry, you can't use this item again since you've already got this appearance."); return true; }
                    if (Kit.Count(c, AprilFools2023Ui.PlayfulFragment) < 30) { Kit.Msg(c, "Failed to combine due to insufficient fragments."); return true; }
                    if (!Kit.Take(c, AprilFools2023Ui.PlayfulFragment, 30, stream)) return true;
                    c.Player.AddSpecialTitle(t, stream);
                    Kit.Log(AprilFools2023Ui.Festival, c, "TITLE", "Playful Player (30 fragments)");
                    Kit.Msg(c, "You`ve got a permanent title, Playful Player.");
                }
            }
            return true;
        }
    }
}
