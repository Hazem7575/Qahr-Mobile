// ============================================================================
//  [DemonInvasionMadPet 2026-10-15] open handlers for the Mid-Autumn holiday group's boxes that have no script.
//  Contents = the item's own official description ("Open to get 1 +3 Stone (B)", "Use it to receive 1 2-day +5 Stone (B)",
//  "Open to randomly get one from ..."): the boxes come from Hewing Laurel / Mystic Travel / Cultivation rewards.
//  Hook: MY System\MsgItem\MsgItemUsuagePacket.cs UseItem, one tagged line (after the other groups' box lines):
//      if (global::Qahr.Mobile.DemonInvasionMadPet.MidAutumnBoxes.OnUse(client, item)) return;
//  Rules: bag space first, then exactly 1 box taken, then the contents; logged to festivals_log.txt. DERIVED: the boxes
//  open at any time (the 2023 'turns into Chi Points after Oct. 12, 2023' note is ignored because the event reruns yearly);
//  the Medium Awakening Bag picks one of its 10 listed items with equal odds.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.MidAutumnReunion2022;

namespace Qahr.Mobile.DemonInvasionMadPet
{
    public static class MidAutumnBoxes
    {
        /// <summary>box id -> one fixed content (ItemSpec string) or several (one is picked).</summary>
        public static readonly Dictionary<uint, string[]> Boxes = new Dictionary<uint, string[]>
        {
            { 3342770, new[] { "730002 0 1 3" } },            // +2 Stone (B)
            { 3342771, new[] { "730003 0 1 3" } },            // +3 Stone (B)
            { 3342773, new[] { "3009001 0 1 3 2880" } },      // 2-day Bright Star Stone (B)
            { 3342774, new[] { "3009002 0 1 3 2880" } },      // 2-day Radiant Star Stone (B)
            { 3342777, new[] { "3322760 0 1 3 10080" } },     // 7-day 3KK Contribution Medal (B)
            { 3344996, new[] { "3314199 0 1 3 10080" } },     // 7-day 1KK Contribution Medal (B)
            { 3344997, new[] { "3009000 0 1 0 2880" } },      // 2-day Twilight Star Stone
            { 3344998, new[] { "3314553 0 1 3" } },           // Permanent Stone Scrap (B)
            { 3344999, new[] { "730005 0 1 3 2880" } },       // 2-day +5 Stone (B)
            { 3340960, new[] { "3328252 0 1", "3335126 0 1", "3333401 0 1", "3328253 0 1", "3335127 0 1", "3333402 0 1", "3329512 0 1", "3329513 0 1", "3339454 0 1", "3339455 0 1" } },// Medium Awakening Bag
        };
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();

        public static bool OnUse(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            if (c == null || c.Player == null || item == null || !Boxes.ContainsKey(item.ITEM_ID)) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(c.Player.UID, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[c.Player.UID] = now;
            Open(new ClientIo(c), item.ITEM_ID);
            return true;
        }

        /// <summary>Opens one box (harness entry). Returns what was given, or null when refused.</summary>
        public static string Open(IFestIo io, uint box)
        {
            string[] list;
            if (!Boxes.TryGetValue(box, out list) || list.Length == 0) return null;
            var it = ItemSpec.Parse(list[list.Length == 1 ? 0 : Kit.Next(list.Length)]);
            if (io.Count(box) < 1) return null;
            if (io.FreeSlots() < io.SlotsFor(new[] { it })) { io.Msg("Please make 1 free space in your inventory first."); return null; }
            if (!io.Take(box, 1)) return null;
            bool ok = io.Give(it);
            io.Msg(ok ? "You received " + it + "." : "Something went wrong. Please contact a GM.");
            Kit.Log("DemonInvasionMadPet", "BOX", io, Kit.ItemName(box) + " (" + box + ") -> " + (ok ? "" : "FAILED ") + it);
            return ok ? it.ToString() : null;
        }
    }
}
