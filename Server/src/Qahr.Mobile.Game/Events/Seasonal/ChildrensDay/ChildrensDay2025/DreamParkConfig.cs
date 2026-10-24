// ============================================================================
//  [ChildrensDay2025 2026-10-15] Dream Park numbers.
//  OFFICIAL (client): Starwish Stone 3600784, Moonglass 3353010, 50 Dream Points per Starwish Stone used (Cn_Res STR_FUI_CHILD_TXT_33),
//    VIP Dream Store at 15000 points (Text157), ranking needs 20000 points (STR_LUAUI_105290_RANKVIEW_CONDITION_1),
//    7 qualities Rundown..Legendary (STR_FUI_CHILD_QUAILTY_1..7), structures max at 1050 Dream Pts (STR_LUAUI_105290_ACHIEVE_1),
//    point stages 1000 (repeat, x/999), 5000, 10000, 20000, 40000, 60000, 80000, 120000 (childermas25rankdata PointConfig),
//    renovation reward per structure = its pack + 20 Moonglass (luauirewardshow 192/193/194001001 + 19x001002),
//    operation revenue tiers 25/50/75/100 Moonglass (luauirewardshow 190001002..005), event Moonglass 5 (190002000),
//    store tables (Data\ChildrensDay2025\stores.json = EmoneySaleShop.lua 342501/342502/34250), box contents (itemtype texts).
//  DERIVED (not in the client, editable in Data\ChildrensDay2025\childrensday2025.json): quality thresholds between
//    0 and the official 1050, event chance, event Moonglass ranges, revenue per quality, stage rewards, fragment recipes,
//    Starwish Pack odds, Vault Palace spots.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.ChildrensDay2025
{
    public sealed class DreamGood
    {
        public uint Goods, Item, CostItem, Cost, Limit; public bool Bound; public string Name = "";
    }

    public sealed class DreamParkConfig
    {
        // ---- official ids
        public const uint StarwishStone = 3600784, Moonglass = 3353010, SweetFragment = 3353011, ConcertoFragment = 3353012;
        public const uint StarwishPack = 3353013, SmallStarwishBox = 3353014, MediumStarwishBox = 3353015, GrandStarwishBox = 3353016;
        public const uint DessertShopPack = 3353017, CarouselPack = 3353018, FerrisWheelPack = 3353019, DreamMaterialPack = 3353020;
        public const uint SweetDreamBoxT = 3353100, SweetDreamBoxB = 3353101, SweetTiaraBoxT = 3353102, Concerto1HBox = 3353104, Concerto2HBox = 3353105;
        public const uint LocalBox1 = 3353031, LocalBox2 = 3353032, LocalBox4 = 3353033, LocalBox7 = 3353034;
        public const uint GlobalBox1 = 3353035, GlobalBox2 = 3353036, GlobalBox4 = 3353037, GlobalBox7 = 3353038;
        // luauirewardshow sys ids the window needs ({sysid, count}; the count is replaced by the window)
        public const uint SysStarwish = 190001000, SysMoonglass = 195001001;
        public static readonly uint[] SysPack = { 0, 192001001, 193001001, 194001001 };      // building 1 Dessert, 2 Ferris, 3 Carousel
        public static readonly uint[] PackItem = { 0, DessertShopPack, FerrisWheelPack, CarouselPack };

        public string Festival = "ChildrensDay2025";
        public int MinLevel = 80;                // official
        public uint PointsPerStone = 50;         // official
        public uint VipPoints = 15000;           // official
        public uint RankPoints = 20000;          // official
        public uint MaxStructurePts = 1050;      // official (max quality)
        public uint RenovateMoonglass = 20;      // official reward show 19x001002
        public bool RenovatePack = true;         // official reward show 19x001001 (derived: one pack per renovation)
        // derived
        public uint[] QualityPts = { 0, 50, 150, 300, 500, 750, 1050 };   // structure Dream Pts needed for quality 1..7 (last = official 1050)
        public uint[] RevenueByQuality = { 25, 25, 50, 50, 75, 75, 100 }; // Moonglass per structure per daily operation (official tiers 25/50/75/100)
        public uint OperationStarwish = 1;       // official preview 190001000 (Starwish x1) in the operation reward
        public int EventChancePct = 15;          // chance per renovation that a sudden event appears on that structure
        public int MaxEventsPerBuilding = 3;
        public uint EventCost = 1;               // decision 1 costs Starwish Stones (it is "a decision" that grants 50 points, official)
        public uint EventMinMg = 10, EventMaxMg = 60;   // decision 1 Moonglass range
        public uint EventSafeMg = 5;             // decision 2 Moonglass (official reward show 190002000 = Moonglass x5)
        public uint FragmentsPerBox = 30;        // Sweet Fragments -> Sweet Dream Box (B); Concerto Fragments -> 2H Concerto Box
        public uint StarwishPackMin = 3, StarwishPackMax = 100;   // official range "3-100"
        public int[] StarwishPackWeights = { 70, 20, 8, 2 };      // derived: 3-10 / 11-30 / 31-60 / 61-100
        // point stages: official thresholds, derived rewards (event items only)
        public uint StageRepeatPts = 1000; public uint StageRepeatMax = 999;
        public uint[] StagePts = { 5000, 10000, 20000, 40000, 60000, 80000, 120000 };
        // Vault Palace (official map id 12818 -> doc 12019 in FestivalMaps\maps.txt); spots derived (walkable, around the centre 227,227)
        public uint VaultMap = 12818; public ushort VaultX = 227, VaultY = 238;
        // Twin City Suyi (derived spot, walkable): where Dream Store clicks send the player
        public ushort SuyiX = 376, SuyiY = 432;

        public List<DreamGood> DreamStore = new List<DreamGood>(), VipStore = new List<DreamGood>(), PuffStore = new List<DreamGood>();

        public uint QualityOf(uint pts)
        {
            uint q = 1;
            for (int i = 1; i < QualityPts.Length && i < 7; i++) if (pts >= QualityPts[i]) q = (uint)(i + 1);
            return q;
        }
        public uint NextQualityPts(uint quality) { return quality >= 7 || quality >= QualityPts.Length ? MaxStructurePts : QualityPts[quality]; }
        public uint Revenue(uint quality) { int i = (int)Math.Max(1, Math.Min(7, quality)) - 1; return i < RevenueByQuality.Length ? RevenueByQuality[i] : 0; }

        // ------------------------------------------------------------------ load
        static DreamParkConfig _cfg; static string _stamp;
        static string Dir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "ChildrensDay2025"); } }
        public static DreamParkConfig Cfg
        {
            get
            {
                try
                {
                    var a = new FileInfo(Path.Combine(Dir, "childrensday2025.json")); var b = new FileInfo(Path.Combine(Dir, "stores.json"));
                    string stamp = (a.Exists ? a.Length + ":" + a.LastWriteTimeUtc.Ticks : "none") + "|" + (b.Exists ? b.Length + ":" + b.LastWriteTimeUtc.Ticks : "none");
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = new DreamParkConfig();
                    if (a.Exists)
                    {
                        var d = FL.ReadJson(a.FullName);
                        var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
                        c.MinLevel = (int)Clamp(d, "minLevel", (uint)c.MinLevel, 1, 255);
                        c.EventChancePct = (int)Clamp(d, "eventChancePct", (uint)c.EventChancePct, 0, 100);
                        c.EventMinMg = Clamp(d, "eventMinMoonglass", c.EventMinMg, 0, 10000);
                        c.EventMaxMg = Math.Max(c.EventMinMg, Clamp(d, "eventMaxMoonglass", c.EventMaxMg, 0, 10000));
                        c.EventSafeMg = Clamp(d, "eventSafeMoonglass", c.EventSafeMg, 0, 10000);
                        c.FragmentsPerBox = Clamp(d, "fragmentsPerBox", c.FragmentsPerBox, 1, 10000);
                        c.OperationStarwish = Clamp(d, "operationStarwish", c.OperationStarwish, 0, 100);
                        var q = Nums(d, "qualityPts"); if (q != null && q.Length == 7 && q[0] == 0 && IsAscending(q)) { c.QualityPts = q; c.MaxStructurePts = q[6]; }
                        var r = Nums(d, "revenueByQuality"); if (r != null && r.Length == 7 && r.All(x => x <= 100000)) c.RevenueByQuality = r;
                        var w = Nums(d, "starwishPackWeights"); if (w != null && w.Length == 4 && w.Sum(x => (long)x) > 0) c.StarwishPackWeights = w.Select(x => (int)Math.Min(1000000u, x)).ToArray();
                    }
                    if (b.Exists)
                    {
                        var d = FL.ReadJson(b.FullName);
                        c.DreamStore = Goods(d, "dreamStore"); c.VipStore = Goods(d, "vipDreamStore"); c.PuffStore = Goods(d, "princePuff");
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[ChildrensDay2025] config: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new DreamParkConfig();
                }
                return _cfg;
            }
        }
        static bool IsAscending(uint[] a) { for (int i = 1; i < a.Length; i++) if (a[i] <= a[i - 1]) return false; return true; }
        static uint Clamp(Dictionary<string, object> d, string k, uint def, uint lo, uint hi)
        {
            var v = FL.Val(d, k); if (v == null) return def;
            uint u = FL.U(v); return Math.Max(lo, Math.Min(hi, u));
        }
        static uint[] Nums(Dictionary<string, object> d, string k)
        {
            var a = FL.Arr(FL.Val(d, k)); if (a == null) return null;
            return a.Select(x => FL.U(x)).ToArray();
        }
        static List<DreamGood> Goods(Dictionary<string, object> d, string k)
        {
            var list = new List<DreamGood>();
            foreach (var o in FL.Arr(FL.Val(d, k)) ?? new List<object>())
            {
                var g = FL.Obj(o); if (g == null) continue;
                var x = new DreamGood
                {
                    Goods = FL.U(FL.Val(g, "goods")), Item = FL.U(FL.Val(g, "item")), CostItem = FL.U(FL.Val(g, "costItem")),
                    Cost = FL.U(FL.Val(g, "cost")), Limit = FL.U(FL.Val(g, "limit")), Bound = FL.Bool(g, "bound") == true, Name = FL.Str(g, "name") ?? ""
                };
                if (x.Goods == 0 || x.Item == 0 || x.Cost == 0 || x.Cost > 100000000) continue;// never free
                if (list.Any(e => e.Goods == x.Goods)) continue;
                list.Add(x);
            }
            return list;
        }
    }
}
