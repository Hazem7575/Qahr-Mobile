// ============================================================================
//  [Thanksgiving2022 2026-10-15] HolidayBoxes - open handlers for the holiday group's festival boxes / puzzles.
//  Hook: MY System\MsgItem\MsgItemUsuagePacket.cs UseItem, one tagged line:
//      if (global::Qahr.Mobile.Thanksgiving2022.HolidayBoxes.TryUse(client, item)) return;
//  A handler returns true when it handled the item (opened, or answered why not). Every handler checks bag space
//  BEFORE it removes the box, removes exactly one box, then gives; everything is logged to festivals_log.txt.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Thanksgiving2022
{
    public static class HolidayBoxes
    {
        /// <summary>item id -> handler(client, festival key used for the log). Filled by the festivals' static ctors.</summary>
        static readonly ConcurrentDictionary<uint, Func<Client.GameClient, bool>> _h = new ConcurrentDictionary<uint, Func<Client.GameClient, bool>>();
        static volatile bool _init;
        static readonly object _initLock = new object();

        public static void Register(uint item, Func<Client.GameClient, bool> handler) { _h[item] = handler; }

        /// <summary>Makes sure every festival of the group registered its boxes (their Register calls).</summary>
        static void Init()
        {
            if (_init) return;
            lock (_initLock)
            {
                if (_init) return;
                try { Thanksgiving2022Ui.RegisterBoxes(); } catch (Exception e) { MyConsole.SaveException(e); }
                foreach (var a in Extra) { try { a(); } catch (Exception e) { MyConsole.SaveException(e); } }
                _init = true;
            }
        }
        /// <summary>Other festivals of the group add their RegisterBoxes here (static list, no reflection).</summary>
        static readonly List<Action> Extra = new List<Action>
        {
            () => ThanksgivingTribe2023.ThanksgivingTribe2023Ui.RegisterBoxes(),
            () => VillageRequests.VillageRequestsBoxes.Register(),
            () => TwinklingShard.TwinklingShardBoxes.Register(),
        };

        public static bool Handles(uint id) { Init(); return _h.ContainsKey(id); }

        /// <summary>UseItem hook. True = handled (the caller returns).</summary>
        public static bool TryUse(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            if (c == null || c.Player == null || item == null) return false;
            Init();
            Func<Client.GameClient, bool> h;
            if (!_h.TryGetValue(item.ITEM_ID, out h)) return false;
            if (HKit.Throttled("box", c.Player.UID, 300)) return true;
            try { return h(c); }
            catch (Exception e) { MyConsole.SaveException(e); return true; }
        }

        /// <summary>Standard "open one box": room check, take 1, give, log. Returns true (handled) always.</summary>
        public static bool Open(Client.GameClient c, string festival, uint box, List<HItem> give, string extraText = null, int chi = 0)
        {
            give = give.Where(i => i.Id != 0 && i.Count > 0).ToList();
            if (give.Count == 0 && chi <= 0) { HKit.Msg(c, "This box is empty."); return true; }
            if (!HKit.HasRoom(c, give)) { HKit.Msg(c, "Your inventory is full. Please make " + HKit.SlotsFor(give) + " free space(s) first."); return true; }
            if (HKit.Count(c, box) < 1 || !HKit.Take(c, box, 1)) return true;
            string got = HKit.Give(c, give);
            if (chi > 0) got += (got.Length > 0 ? ", " : "") + HKit.GiveChi(c, chi);
            HKit.Log("BOX", festival, c, HKit.Name(box) + " -> " + got);
            HKit.Msg(c, (extraText ?? ("You opened the " + HKit.Name(box) + " and received: ")) + got + ".");
            return true;
        }

        /// <summary>One weighted pick per roll from a table (derived contents).</summary>
        public static List<HItem> Roll(List<HKit.Prize> table, int rolls)
        {
            var l = new List<HItem>();
            for (int i = 0; i < rolls; i++) { var p = HKit.Draw(table); if (p != null) l.Add(p.Item); }
            return l;
        }
    }
}
