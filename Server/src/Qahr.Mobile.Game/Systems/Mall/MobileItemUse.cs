// ============================================================================
//  QahrMobile: "Use" for phone-only bag items that the PC 7957 UseItem switch does not know (CP boxes, packs,
//  random packs, potency drinks), plus a clear message for items of systems this server does not have.
//  Data: Data\mobile_item_use.txt, one item per line (re-read when the file changes):
//      <id> cps <n>                 n CPs; a BOUND item gives bound CPs instead
//      <id> bcps <n>                n bound CPs
//      <id> potency <n>             n Inner Power potency points (InnerPower.AddPotency)
//      <id> items <spec> <spec> ... every spec is given
//      <id> random <w>*<spec> ...   one spec is picked by weight w
//      <id> msg <text>              the item is kept and the text is shown
//    spec = id:count[:b][:minutes]  (b = bound; the content is also bound when the used item is bound)
//  Hook: MsgItemUsuagePacket.cs UseItem, next to the other box lines:
//      if (global::Qahr.Mobile.Mall.MobileItemUse.OnUse(client, item)) return;
//  Rules: bag space first, then exactly 1 item taken, then the content; logged to festivals_log.txt.
//  Batches: the phone sends one 10137 (action 4) per tap with no count (dwParam/dwParam2 are 0), so each tap opens 1.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qahr.Mobile.MidAutumnReunion2022;

namespace Qahr.Mobile.Mall
{
    public static class MobileItemUse
    {
        sealed class Entry
        {
            public string Kind;
            public uint Amount;
            public string Text;
            public List<ItemSpec> Items = new List<ItemSpec>();
            public List<int> Weights = new List<int>();
        }

        static Dictionary<uint, Entry> _map = new Dictionary<uint, Entry>();
        static DateTime _stamp = DateTime.MinValue;
        static readonly object _sync = new object();
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();

        static string FilePath { get { return Path.Combine(Program.ServerConfig.DbLocation, "mobile_item_use.txt"); } }

        static bool ParseSpec(string s, out ItemSpec it)
        {
            it = new ItemSpec();
            var f = s.Split(':');
            uint id, n = 1, min = 0;
            if (!uint.TryParse(f[0], out id) || id == 0) return false;
            bool bound = false;
            for (int i = 1; i < f.Length; i++)
            {
                if (i == 1 && uint.TryParse(f[i], out n)) continue;
                if (f[i].Equals("b", StringComparison.OrdinalIgnoreCase)) bound = true;
                else uint.TryParse(f[i], out min);
            }
            it = ItemSpec.Of(id, Math.Max(1u, n), bound, min);
            return true;
        }

        static Dictionary<uint, Entry> Map()
        {
            try
            {
                var t = File.Exists(FilePath) ? File.GetLastWriteTimeUtc(FilePath) : DateTime.MinValue;
                if (t == _stamp) return _map;
                lock (_sync)
                {
                    if (t == _stamp) return _map;
                    var map = new Dictionary<uint, Entry>();
                    if (t != DateTime.MinValue)
                        foreach (string line in File.ReadAllLines(FilePath))
                        {
                            string l = line;
                            int hash = l.IndexOf('#');
                            if (hash >= 0) l = l.Substring(0, hash);
                            var p = l.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            uint id;
                            if (p.Length < 3 || !uint.TryParse(p[0], out id)) continue;
                            var e = new Entry { Kind = p[1].ToLowerInvariant() };
                            switch (e.Kind)
                            {
                                case "cps": case "bcps": case "potency":
                                    if (!uint.TryParse(p[2], out e.Amount) || e.Amount == 0) continue;
                                    break;
                                case "msg":
                                    e.Text = string.Join(" ", p.Skip(2));
                                    break;
                                case "items":
                                    foreach (var s in p.Skip(2)) { ItemSpec it; if (ParseSpec(s, out it)) e.Items.Add(it); }
                                    if (e.Items.Count == 0) continue;
                                    break;
                                case "random":
                                    foreach (var s in p.Skip(2))
                                    {
                                        int star = s.IndexOf('*'), w;
                                        ItemSpec it;
                                        if (star <= 0 || !int.TryParse(s.Substring(0, star), out w) || w <= 0 || !ParseSpec(s.Substring(star + 1), out it)) continue;
                                        e.Weights.Add(w); e.Items.Add(it);
                                    }
                                    if (e.Items.Count == 0) continue;
                                    break;
                                default:
                                    continue;
                            }
                            map[id] = e;
                        }
                    _map = map;
                    _stamp = t;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[MobileItemUse] " + e.Message, ConsoleColor.Yellow);
            }
            return _map;
        }

        public static bool OnUse(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            if (c == null || c.Player == null || item == null) return false;
            Entry e;
            if (!Map().TryGetValue(item.ITEM_ID, out e)) return false;
            var io = new ClientIo(c);
            string name = Kit.ItemName(item.ITEM_ID);
            if (e.Kind == "msg") { io.Msg(e.Text); return true; }

            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(c.Player.UID, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[c.Player.UID] = now;
            if (io.Count(item.ITEM_ID) < 1) return true;
            bool boundSrc = item.Bound != 0;

            List<ItemSpec> give = null;
            if (e.Kind == "items") give = e.Items;
            else if (e.Kind == "random") give = new List<ItemSpec> { e.Items[Kit.Pick(e.Weights)] };
            if (give != null)
            {
                give = give.Select(g => { var x = g; x.Bound = g.Bound || boundSrc; return x; }).ToList();
                int need = io.SlotsFor(give);
                if (io.FreeSlots() < need) { io.Msg("Please make " + need + " free space" + (need > 1 ? "s" : "") + " in your inventory first."); return true; }
            }
            if (e.Kind == "potency" && c.Player.InnerPower == null) { io.Msg("Inner Power is not ready yet. Please try again later."); return true; }

            if (!io.Take(item.ITEM_ID, 1)) return true;
            string got;
            bool ok = true;
            switch (e.Kind)
            {
                case "cps":
                case "bcps":
                    if (e.Kind == "bcps" || boundSrc) { c.Player.BoundConquerPoints += (int)e.Amount; got = e.Amount + " bound CPs"; }
                    else { c.Player.ConquerPoints += e.Amount; got = e.Amount + " CPs"; }
                    break;
                case "potency":
                    using (var rec = new ServerSockets.RecycledPacket())
                        c.Player.InnerPower.AddPotency(rec.GetStream(), c, (int)e.Amount);
                    got = e.Amount + " Potency Points";
                    break;
                default:
                    var given = Kit.GiveAll(io, give);
                    ok = given.All(s => !s.StartsWith("FAILED"));
                    got = string.Join(", ", given);
                    break;
            }
            io.Msg(ok ? "You received " + got + "." : "Something went wrong. Please contact a GM.");
            Kit.Log("MobileItemUse", e.Kind.ToUpperInvariant(), io, name + " (" + item.ITEM_ID + (boundSrc ? ", bound" : "") + ") -> " + (ok ? "" : "FAILED ") + got);
            return true;
        }
    }
}
