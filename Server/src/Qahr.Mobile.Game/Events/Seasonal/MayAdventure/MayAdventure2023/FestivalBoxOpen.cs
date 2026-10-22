// ============================================================================
//  [MayAdventure2023 2026-10-15] FestivalBoxOpen: open handlers for festival boxes that have no script.
//  Data: Infinity[DB]\MayAdventure2023\boxes.json (re-read when it changes):
//    { "boxes": [ { "item": 3342013, "fixed": ["3341961 0 6 3", "730005 0 2 3"], "random": [ {"item":"...","weight":n} ],
//                   "_source": "official item text ..." } ] }
//    fixed = always given; random = ONE weighted pick (weights derived). Item strings "id 0 count monopoly" (3 = bound).
//  Contents are the official item texts (itemtype "Open to get ...") where the text lists them; weights are derived.
//  Any festival of the anniversary group (or another agent) can add a box row; the code needs no change.
//
//  Security: the box is taken (exactly 1) only after the bag-space check; then the contents are given. Per-uid lock
//  + 300 ms rate limit, so double-clicks / duplicate packets can never open one box twice.
//  Hook: MY System\MsgItem\MsgItemUsuagePacket.cs, one tagged line before the ItemUse switch:
//        if (global::Qahr.Mobile.MayAdventure2023.FestivalBoxOpen.TryOpen(client, item)) { } else
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.AnnivKit;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.MayAdventure2023
{
    public static class FestivalBoxOpen
    {
        public sealed class Box { public uint Item; public List<Prize> Fixed = new List<Prize>(); public List<Prize> Random = new List<Prize>(); }
        public sealed class BoxCfg { public Dictionary<uint, Box> Boxes = new Dictionary<uint, Box>(); }

        public static readonly Kit.CfgCache<BoxCfg> Config = new Kit.CfgCache<BoxCfg>(System.IO.Path.Combine("MayAdventure2023", "boxes.json"), Read, () => new BoxCfg());

        public static BoxCfg Read(Dictionary<string, object> d)
        {
            var c = new BoxCfg();
            var arr = FL.Arr(FL.Val(d, "boxes"));
            if (arr == null) return c;
            foreach (var o in arr)
            {
                var b = FL.Obj(o); if (b == null) continue;
                uint id = FL.U(FL.Val(b, "item")); if (id == 0) continue;
                var box = new Box { Item = id, Fixed = Kit.ReadPool(FL.Val(b, "fixed")), Random = Kit.ReadPool(FL.Val(b, "random")) };
                if (box.Fixed.Count == 0 && box.Random.Count == 0) continue;
                if (box.Fixed.Any(p => p.Item == id) || box.Random.Any(p => p.Item == id)) continue;// never a box that gives itself
                c.Boxes[id] = box;
            }
            return c;
        }

        /// <summary>Code handlers for boxes whose opening needs more than items (e.g. a Grit Point cost). Registered by the
        /// festival modules of this group in their static constructors (see EnsureRegistered).</summary>
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Func<Client.GameClient, bool>> _code = new System.Collections.Concurrent.ConcurrentDictionary<uint, Func<Client.GameClient, bool>>();
        public static void Register(uint item, Func<Client.GameClient, bool> handler) { if (item != 0 && handler != null) _code[item] = handler; }
        static volatile bool _reg;
        static void EnsureRegistered()
        {
            if (_reg) return;
            _reg = true;
            try { BleakValleyVillage.BleakValleyVillageUi.RegisterBoxes(); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        public static bool Handles(uint item) { EnsureRegistered(); return _code.ContainsKey(item) || Config.Get().Boxes.ContainsKey(item); }

        /// <summary>Hook: true = this box was handled (opened or refused with a message).</summary>
        public static bool TryOpen(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            try
            {
                if (c == null || c.Player == null || item == null) return false;
                EnsureRegistered();
                Func<Client.GameClient, bool> h;
                if (_code.TryGetValue(item.ITEM_ID, out h))
                {
                    if (Kit.Throttled("FestivalBox", c.Player.UID, 300)) return true;
                    return h(c);
                }
                Box box;
                if (!Config.Get().Boxes.TryGetValue(item.ITEM_ID, out box)) return false;
                if (Kit.Throttled("FestivalBox", c.Player.UID, 300)) return true;
                string err; List<Prize> got;
                if (!Open(box, c.Player.UID, new ClientBag(c), out got, out err)) { Kit.Msg(c, err); return true; }
                Kit.Log("FestivalBox", "open", c, Kit.ItemName(box.Item) + " (" + box.Item + ") -> " + string.Join(", ", got));
                Kit.Msg(c, "Congrats! You received " + Kit.Names(got) + "!");
                return true;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return true; }
        }

        /// <summary>The rule (also used by the harness): space first, take 1 box, then give.</summary>
        public static bool Open(Box box, uint uid, IBag bag, out List<Prize> got, out string err)
        {
            got = new List<Prize>(); err = null;
            lock (Kit.Lock("FestivalBox", uid))
            {
                if (bag.Count(box.Item) < 1) { err = "You don`t have this item."; return false; }
                var list = box.Fixed.Select(p => p.Clone()).ToList();
                if (box.Random.Count > 0) { var r = Kit.PickOne(box.Random); if (r != null) list.Add(r); }
                if (list.Count == 0) { err = "This box is empty."; return false; }
                int slots = Kit.Slots(list);
                if (!bag.HasSpace(slots)) { err = "Please make " + slots + " space(s) in your inventory first."; return false; }
                if (!bag.Take(box.Item, 1)) { err = "You don`t have this item."; return false; }
                foreach (var p in list) bag.Give(p);
                got = list;
                return true;
            }
        }
    }
}
