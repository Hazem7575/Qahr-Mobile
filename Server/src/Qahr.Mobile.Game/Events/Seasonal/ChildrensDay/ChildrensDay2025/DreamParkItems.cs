// ============================================================================
//  [ChildrensDay2025 2026-10-15] Item helpers + open handlers of the Dream Park boxes (they had no script).
//  Contents = the OFFICIAL itemtype descriptions (client): Starwish Pack "3-100 Starwish Stones", Small/Medium/Grand Starwish Box
//  42/85/175, Dessert Shop / Carousel / Ferris Wheel / Dream Material Pack "a random reward from ...", Local No.x Dream Box,
//  Sweet Dream / Sweet Tiara / Concerto boxes. Item ids resolved from itemtype.txt (2025 block 33468xx = the "(B)" stones/medals
//  the Dream Store sells). DERIVED: the weights of the random packs, the Starwish Pack odds, fragments needed to combine.
//  Not opened here (reported): Global No.x Dream Box (cross-server ranking only), Sweet Dream Hairstyle (T) (hair storage).
//  Hook: one line at the top of MsgItemUsuagePacket.UseItem.
//  Safety: the box is removed BEFORE the contents are given; bag space is checked first; everything is logged.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.ChildrensDay2025
{
    public static class DreamParkItems
    {
        public struct Grant { public uint Id; public uint Count; public bool Bound; public byte Bless; public uint Minutes; public int Weight;
            public Grant(uint id, uint count, bool bound = false, int weight = 1, byte bless = 0, uint minutes = 0) { Id = id; Count = count; Bound = bound; Weight = weight; Bless = bless; Minutes = minutes; } }

        static readonly Random _rnd = new Random();
        public static Func<int, int> RandOverride;               // harness: (n) -> 0..n-1
        public static int Rand(int n) { var o = RandOverride; if (o != null) return Math.Max(0, Math.Min(n - 1, o(n))); lock (_rnd) return _rnd.Next(n); }

        // ---- random pack tables (official item lists; derived weights)
        public static readonly Grant[] Dessert =
        {
            new Grant(DreamParkConfig.SweetFragment, 1, false, 25), new Grant(DreamParkConfig.SweetFragment, 3, false, 8), new Grant(DreamParkConfig.ConcertoFragment, 1, false, 8),
            new Grant(3346828, 2, true, 15), new Grant(3346831, 1, true, 3),                       // 2 +2 Stones (B) / 1 +5 Stone (B)
            new Grant(3346822, 1, true, 12), new Grant(3346824, 1, true, 5), new Grant(3346825, 1, true, 1),   // 3KK/10KK/100KK Contribution Medal (B)
            new Grant(3321077, 1, true, 12), new Grant(3314207, 1, true, 5), new Grant(3314208, 1, true, 1),   // Leopard/Tiger/Dragon Chi Liquor (B)
            new Grant(3346837, 1, false, 5),                                                        // Radiant Star Stone
        };
        public static readonly Grant[] Carousel =
        {
            new Grant(DreamParkConfig.SweetFragment, 1, false, 25), new Grant(DreamParkConfig.SweetFragment, 3, false, 8), new Grant(DreamParkConfig.ConcertoFragment, 1, false, 8),
            new Grant(3346828, 2, true, 15), new Grant(3346831, 1, true, 3),
            new Grant(4060001, 10, true, 12), new Grant(4060001, 30, true, 5), new Grant(4060001, 100, true, 1),// Universal Rune Essence (B)
            new Grant(3314253, 1, true, 5), new Grant(3314248, 1, true, 1),                         // Yellow Rune Fragment (B) / Yellow Rune Pack (B)
            new Grant(3009100, 2, false, 6), new Grant(3009101, 2, false, 3), new Grant(3009102, 2, false, 1), // Ginseng / Dragon / Moon Fruits
        };
        public static readonly Grant[] Ferris =
        {
            new Grant(DreamParkConfig.SweetFragment, 1, false, 25), new Grant(DreamParkConfig.SweetFragment, 3, false, 8), new Grant(DreamParkConfig.ConcertoFragment, 1, false, 8),
            new Grant(3346828, 2, true, 15), new Grant(3346831, 1, true, 3),
            new Grant(3328247, 20, false, 12), new Grant(3328247, 300, false, 1),                  // Awakening Spirits
            new Grant(3347158, 2, false, 6), new Grant(3347157, 2, false, 2),                       // Shadow Rune / Light Rune
            new Grant(3347151, 10, false, 8), new Grant(3347151, 100, false, 1),                    // Night Oracles
        };
        public static readonly Grant[] Material =
        {
            new Grant(3347158, 4), new Grant(3328247, 50), new Grant(3346824, 1), new Grant(3009100, 3),
        };
        const uint Week = 7 * 24 * 60;
        static Grant[] LocalBox(uint aw, uint ure, uint medals)
        {
            return new[] { new Grant(3328247, aw), new Grant(4060001, ure, true), new Grant(3346825, medals, true, 1, 0, Week) };
        }

        public static Grant Pick(Grant[] table)
        {
            int total = table.Sum(g => Math.Max(0, g.Weight)); if (total <= 0) return table[0];
            int r = Rand(total);
            foreach (var g in table) { int w = Math.Max(0, g.Weight); if (r < w) return g; r -= w; }
            return table[table.Length - 1];
        }

        public static uint StarwishPackRoll(DreamParkConfig cfg)
        {
            uint[][] b = { new uint[] { 3, 10 }, new uint[] { 11, 30 }, new uint[] { 31, 60 }, new uint[] { 61, 100 } };
            int total = cfg.StarwishPackWeights.Sum(); int r = Rand(Math.Max(1, total)), k = 0;
            for (; k < 3; k++) { if (r < cfg.StarwishPackWeights[k]) break; r -= cfg.StarwishPackWeights[k]; }
            uint lo = Math.Max(cfg.StarwishPackMin, b[k][0]), hi = Math.Min(cfg.StarwishPackMax, b[k][1]);
            if (hi < lo) hi = lo;
            return lo + (uint)Rand((int)(hi - lo + 1));
        }

        // ------------------------------------------------------------------ inventory helpers
        public static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        /// <summary>Stack-aware removal (Inventory.Remove removes whole stacks). Caller checked Count first.</summary>
        public static bool Take(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (count == 0) return true;
            if (Count(c, id) < count) return false;
            uint left = count;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).OrderBy(i => i.StackSize).ToArray())
            {
                if (left == 0) break;
                uint stack = item.StackSize > 1 ? item.StackSize : 1u;
                if (stack > left)
                {
                    item.StackSize = (ushort)(stack - left);
                    item.Mode = Role.Flags.ItemMode.Update;
                    try { item.Send(c, stream); } catch { }
                    left = 0;
                }
                else
                {
                    left -= stack;
                    item.StackSize = 1;
                    c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                }
            }
            return left == 0;
        }
        static uint StackOf(uint id)
        {
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && db.StackSize > 1) return db.StackSize;
            return 1;
        }
        public static bool Exists(uint id) { return Pool.ItemsBase != null && Pool.ItemsBase.ContainsKey(id); }
        /// <summary>Free bag slots needed (conservative: never counts on an existing stack).</summary>
        public static int Slots(IEnumerable<Grant> grants)
        {
            int n = 0;
            foreach (var g in grants)
            {
                if (g.Count == 0) continue;
                if (g.Minutes > 0 || g.Bless > 0) n += (int)g.Count;
                else { uint s = StackOf(g.Id); n += (int)((g.Count + s - 1) / s); }
            }
            return n;
        }
        public static bool HaveSpace(Client.GameClient c, int slots) { return slots <= 0 || (slots <= 40 && c.Inventory.HaveSpace((byte)slots)); }

        /// <summary>Gives through the normal inventory methods. Caller checked the space.</summary>
        public static string Give(Client.GameClient c, Grant g, ServerSockets.Packet stream)
        {
            if (g.Count == 0) return "";
            try
            {
                if (g.Minutes > 0 || g.Bless > 0)
                {
                    for (uint i = 0; i < g.Count; i++)
                        c.Inventory.Add(stream, g.Id, 1, 0, g.Bless, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, g.Bound, Role.Flags.ItemEffect.None, false, "", g.Minutes);
                }
                else
                {
                    uint s = StackOf(g.Id), left = g.Count;
                    while (left > 0) { uint n = Math.Min(left, Math.Min(s, 60000u)); c.Inventory.AddItemWitchStack(g.Id, 0, (ushort)n, stream, g.Bound); left -= n; }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); return "FAILED " + g.Id; }
            return QuestEngine.QuestGrammar.ItemName(g.Id) + (g.Count > 1 ? " x" + g.Count : "");
        }
        public static string GiveAll(Client.GameClient c, IEnumerable<Grant> grants, ServerSockets.Packet stream)
        {
            return string.Join(", ", grants.Where(g => g.Count > 0).Select(g => Give(c, g, stream)));
        }

        static void ConsumeOne(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            if (item.StackSize > 1)
            {
                item.StackSize--;
                item.Mode = Role.Flags.ItemMode.Update;
                item.Send(c, stream);
            }
            else c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        public static bool Handles(uint id)
        {
            return (id >= DreamParkConfig.SweetFragment && id <= DreamParkConfig.DreamMaterialPack) || (id >= DreamParkConfig.LocalBox1 && id <= DreamParkConfig.GlobalBox7)
                || id == DreamParkConfig.SweetDreamBoxT || id == DreamParkConfig.SweetDreamBoxB || id == DreamParkConfig.SweetTiaraBoxT
                || id == DreamParkConfig.Concerto1HBox || id == DreamParkConfig.Concerto2HBox;
        }

        /// <summary>MsgItemUsuagePacket.UseItem hook. True = handled (the caller returns).</summary>
        public static bool OnUse(MsgGameItem item, Client.GameClient c, ServerSockets.Packet stream)
        {
            if (item == null || c == null || c.Player == null || !Handles(item.ITEM_ID)) return false;
            try
            {
                lock (DreamParkState.Sync)
                {
                    MsgGameItem live;
                    if (!c.Inventory.TryGetItem(item.UID, out live) || live.ITEM_ID != item.ITEM_ID) return true;
                    var cfg = DreamParkConfig.Cfg;
                    uint id = item.ITEM_ID;
                    var gives = new List<Grant>();
                    uint consume = 1;// boxes: 1 item
                    switch (id)
                    {
                        case DreamParkConfig.StarwishPack: gives.Add(new Grant(DreamParkConfig.StarwishStone, DreamParkItems.StarwishPackRoll(cfg))); break;
                        case DreamParkConfig.SmallStarwishBox: gives.Add(new Grant(DreamParkConfig.StarwishStone, 42)); break;
                        case DreamParkConfig.MediumStarwishBox: gives.Add(new Grant(DreamParkConfig.StarwishStone, 85)); break;
                        case DreamParkConfig.GrandStarwishBox: gives.Add(new Grant(DreamParkConfig.StarwishStone, 175)); break;
                        case DreamParkConfig.DessertShopPack: gives.Add(Pick(Dessert)); break;
                        case DreamParkConfig.CarouselPack: gives.Add(Pick(Carousel)); break;
                        case DreamParkConfig.FerrisWheelPack: gives.Add(Pick(Ferris)); break;
                        case DreamParkConfig.DreamMaterialPack: gives.Add(Pick(Material)); break;
                        case DreamParkConfig.LocalBox1: gives.AddRange(LocalBox(1000, 500, 5)); break;
                        case DreamParkConfig.LocalBox2: gives.AddRange(LocalBox(800, 300, 3)); break;
                        case DreamParkConfig.LocalBox4: gives.AddRange(LocalBox(500, 200, 2)); break;
                        case DreamParkConfig.LocalBox7: gives.AddRange(LocalBox(300, 100, 1)); break;
                        case DreamParkConfig.SweetDreamBoxT: gives.Add(new Grant(193945, 1, false, 1, 1)); break;          // 3-star garment Sweet Dream, 1% blessed, tradable
                        case DreamParkConfig.SweetDreamBoxB: gives.Add(new Grant(193945, 1, true, 1, 1)); break;
                        case DreamParkConfig.SweetTiaraBoxT: gives.Add(new Grant(193955, 1, false, 1, 1)); gives.Add(new Grant(3353039, 1)); break;// + Sweet Dream Hairstyle (T)
                        case DreamParkConfig.Concerto1HBox: gives.Add(new Grant(360458, 2, false, 1, 1)); break;           // 2 Candy Concertos (1-handed) = 360458 (360xxx = 1H, like Bubble Machine 360446)
                        case DreamParkConfig.Concerto2HBox: gives.Add(new Grant(350223, 1, false, 1, 1)); break;           // 1 Candy Concerto (2-handed) = 350223 (350xxx = 2H, like Bubble Machine (2H) 350215)
                        case DreamParkConfig.SweetFragment:
                        case DreamParkConfig.ConcertoFragment:
                            {
                                consume = cfg.FragmentsPerBox;
                                if (Count(c, id) < consume)
                                {
                                    Msg(c, "You need " + consume + " " + QuestEngine.QuestGrammar.ItemName(id) + "s to combine them.");
                                    return true;
                                }
                                gives.Add(id == DreamParkConfig.SweetFragment ? new Grant(193945, 1, true, 1, 1) : new Grant(350223, 1, true, 1, 1));
                                break;
                            }
                        case DreamParkConfig.Moonglass: Msg(c, "Use Moonglasses in the Dream Store (Suyi in Twin City)."); return true;
                        default:
                            if (id >= DreamParkConfig.GlobalBox1 && id <= DreamParkConfig.GlobalBox7) { Msg(c, "The global Dream Ranking (cross-server) isn`t open on this server, so this box can`t be opened here."); return true; }
                            return false;// 3353021-25 / 27-30 belong to other scripts
                    }
                    if (gives.Any(g => !Exists(g.Id))) { Msg(c, "This box can`t be opened right now."); DreamParkState.Log(c, "OPEN " + id + " missing item type"); return true; }
                    int slots = Slots(gives);
                    if (!HaveSpace(c, slots)) { Msg(c, "Your inventory is full. Please make " + slots + " space(s) before activation!"); return true; }
                    // remove first, then give
                    if (consume == 1) ConsumeOne(c, live, stream);
                    else if (!Take(c, id, consume, stream)) return true;
                    string got = GiveAll(c, gives, stream);
                    DreamParkState.Log(c, "OPEN " + id + " x" + consume + " -> " + got);
                    Msg(c, "Congratulations! You obtained " + got + "!");
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }
    }
}
