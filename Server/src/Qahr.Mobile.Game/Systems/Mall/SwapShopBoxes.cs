// ============================================================================
//  QahrMobile: the boxes sold by the phone Mall "Swap Shop" (shop 6, paid with swap tickets, MsgShopClan PayWithItems).
//  Contents: Data\swapshop_boxes.txt "box content minutes bound", built from the client's luacfg\tipsmode.dat
//  ItemCopyMode[box] = item (minutes 0 = permanent, from "30D ..." in the box name). The file is re-read when it changes.
//  Hook: MsgItemUsuagePacket.cs UseItem, next to the other box lines:
//      if (global::Qahr.Mobile.Mall.SwapShopBoxes.OnUse(client, item)) return;
//  Rules: bag space first, then exactly 1 box taken, then the content; logged to festivals_log.txt.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Qahr.Mobile.MidAutumnReunion2022;

namespace Qahr.Mobile.Mall
{
    public static class SwapShopBoxes
    {
        static Dictionary<uint, ItemSpec> _boxes = new Dictionary<uint, ItemSpec>();
        static DateTime _stamp = DateTime.MinValue;
        static readonly object _sync = new object();
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();

        static string FilePath { get { return Path.Combine(Program.ServerConfig.DbLocation, "swapshop_boxes.txt"); } }

        static Dictionary<uint, ItemSpec> Boxes()
        {
            try
            {
                var t = File.Exists(FilePath) ? File.GetLastWriteTimeUtc(FilePath) : DateTime.MinValue;
                if (t == _stamp) return _boxes;
                lock (_sync)
                {
                    if (t == _stamp) return _boxes;
                    var map = new Dictionary<uint, ItemSpec>();
                    if (t != DateTime.MinValue)
                        foreach (string line in File.ReadAllLines(FilePath))
                        {
                            string l = line;
                            int hash = l.IndexOf('#');
                            if (hash >= 0) l = l.Substring(0, hash);
                            var p = l.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            uint box, item, minutes = 0, bound = 0;
                            if (p.Length < 2 || !uint.TryParse(p[0], out box) || !uint.TryParse(p[1], out item)) continue;
                            if (p.Length > 2) uint.TryParse(p[2], out minutes);
                            if (p.Length > 3) uint.TryParse(p[3], out bound);
                            map[box] = ItemSpec.Of(item, 1, bound != 0, minutes);
                        }
                    _boxes = map;
                    _stamp = t;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[SwapShopBoxes] " + e.Message, ConsoleColor.Yellow);
            }
            return _boxes;
        }

        public static bool OnUse(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            if (c == null || c.Player == null || item == null) return false;
            ItemSpec it;
            if (!Boxes().TryGetValue(item.ITEM_ID, out it)) return false;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(c.Player.UID, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[c.Player.UID] = now;
            var io = new ClientIo(c);
            if (io.Count(item.ITEM_ID) < 1) return true;
            if (io.FreeSlots() < io.SlotsFor(new[] { it })) { io.Msg("Please make 1 free space in your inventory first."); return true; }
            if (!io.Take(item.ITEM_ID, 1)) return true;
            bool ok = io.Give(it);
            io.Msg(ok ? "You received " + it + "." : "Something went wrong. Please contact a GM.");
            Kit.Log("SwapShop", "BOX", io, Kit.ItemName(item.ITEM_ID) + " (" + item.ITEM_ID + ") -> " + (ok ? "" : "FAILED ") + it);
            return true;
        }
    }
}
