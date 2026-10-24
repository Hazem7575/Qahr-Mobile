// ============================================================================
//  [PuppetTheater 2026-10-15, Vault Palace pass] The hero statues of Vault Palace (official map 12818 -> doc 12019):
//    * 12 statue spots = luacfg minimap.lua markers of doc 12019 (Cn_Res STR_INI_LUA_MINIMAP_Target_12019_NAME_1..12),
//      NPC look = npc.ini [NpcType38xx/47xx/49xx] "VaultPalace" (SimpleObj 9970, the glowing spot on the pedestal), one per
//      global event theme. Clicking one opens the official "Vault Palace" ranking text (Server_Key tGloablEventsTemple[MoneyRank]
//      Text111 / Text112[theme] / Text113 / Text114..1113 rows / Text1114 / Text1115) with "Visit." (Option113).
//    * Visit = the official timed gather ("Visiting...", tGloablEventsTemple[Sys][Read]): a progress bar that breaks
//      ("Interrupted") when the player moves or is hit (Role.Player.AddPick / RemovePick, MsgMovement / ActionHandler).
//      A finished visit gives the daily Vault Palace Visit reward (30 SP, once a day) and, at the three spirit statues,
//      the Puppet Adventure fragment of that statue (video 03:05-03:20: "Interrupted" / "Visiting..." at the Mid-Autumn
//      mooncake statue among the SilkSpecters; "You have already obtained the puppet fragment here" at the golden urns).
//    * The ranking: we have one server, so the rows come from OUR festival rankings (Halloween = Strings of Terror local
//      ranking); the Language column is DERIVED ("English", tGloablEventsTemple[Tip][2]); a theme without data shows
//      tGloablEventsTemple[Tip][0] "None".
//    * The No.1 hero model: a player-look statue (Role.Statue SobNpc, the same entity the Elite PK champion statue uses)
//      standing on the theme's spot while that hero has been online since the last restart (DERIVED mechanism).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;

namespace Qahr.Mobile.PuppetTheater
{
    public static class PuppetVault
    {
        public const uint FirstStatue = 8893040, LastStatue = 8893051;
        public const uint VaultMap = 12818;
        // theme k (1..12) = Text112[k]; spot = minimap marker k; npc.ini VaultPalace type of that theme
        public static readonly ushort[][] Spots = {
            null,
            new ushort[] {164,164}, new ushort[] {190,122}, new ushort[] {240,112}, new ushort[] {268,164}, new ushort[] {316,188}, new ushort[] {310,242},
            new ushort[] {268,268}, new ushort[] {242,310}, new ushort[] {190,314}, new ushort[] {164,268}, new ushort[] {114,238}, new ushort[] {122,190} };
        /// <summary>Statue theme -> Puppet Adventure fragment index (0 SilkSpecter / 1 WeepWood / 2 BoneSnapper), -1 = none.
        /// Video: SilkSpecters swarm the Mid-Autumn mooncake (9); WeepWood and BoneSnapper guard golden-urn statues (DERIVED: 7, 1).</summary>
        public static int FragmentOf(int theme) { return theme == 9 ? 0 : theme == 7 ? 1 : theme == 1 ? 2 : -1; }
        public static int ThemeOf(uint npc) { return npc >= FirstStatue && npc <= LastStatue ? (int)(npc - FirstStatue) + 1 : 0; }
        public const int HalloweenTheme = 10;
        public const int VisitSeconds = 5;   // DERIVED (video: the bar runs ~5 s)

        static string K(string k) { return "STR_ID_tGloablEventsTemple[MoneyRank][" + k + "]"; }

        /// <summary>Top 10 (name) of a theme from our own rankings.</summary>
        public static List<string> Top(int theme)
        {
            var l = new List<string>();
            if (theme == HalloweenTheme)
                foreach (var part in PT.St.Get(0, "rank").Split(','))
                {
                    var p = part.Split(':');
                    if (p.Length >= 3 && p[2].Length > 0) l.Add(p[2]);
                    if (l.Count >= 10) break;
                }
            return l;
        }
        public static uint TopUid(int theme)
        {
            if (theme != HalloweenTheme) return 0;
            var first = PT.St.Get(0, "rank").Split(',').FirstOrDefault();
            uint u; return first != null && uint.TryParse(first.Split(':')[0], out u) ? u : 0;
        }

        /// <summary>A statue clicked: the official ranking text + Visit.</summary>
        public static void Open(Client.GameClient c, ServerSockets.Packet s, uint npc)
        {
            int theme = ThemeOf(npc); if (theme == 0) return;
            var top = Top(theme);
            var d = PT.Dialog(c, s, PT.DlgAdv, (cc, ss, o) => { if (o == 1) Visit(cc, ss, npc); });
            var sb = new StringBuilder();
            sb.Append(K("Text111")).Append("@@\n").Append(K("Text112][" + theme)).Append("@@\n").Append(K("Text113")).Append("@@\n");
            string none = "STR_ID_tGloablEventsTemple[Tip][0]";
            for (int i = 0; i < 10; i++)
            {
                string row = i == 0 ? "Text114" : i == 1 ? "Text115" : i == 2 ? "Text116" : i == 3 ? "Text117" : i == 4 ? "Text118" : i == 5 ? "Text119" : "Text11" + (i + 4);
                string name = i < top.Count ? top[i] : "-";
                string lang = i < top.Count ? "English" : "-";   // DERIVED language column
                sb.Append(K(row)).Append("@@").Append(lang).Append("@@").Append(name).Append("@@\n");
            }
            sb.Append(K("Text1114")).Append("@@\n").Append(K("Text1115")).Append("@@\n");
            if (top.Count == 0) sb.Append(none).Append("@@\n");
            foreach (var line in sb.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) d.AddText(line + "\n");   // one Server_Key per text packet
            d.AddOption(K("Option113") + "@@\n", 1).AddOption("STR_ID_tGloablEventsTemple[MoneyRank][Option211]@@\n", 255).FinalizeDialog();
        }

        static readonly ConcurrentDictionary<uint, DateTime> _visiting = new ConcurrentDictionary<uint, DateTime>();
        /// <summary>The official timed visit at a statue ("Visiting..." bar; moving / being hit = "Interrupted").</summary>
        public static string Visit(Client.GameClient c, ServerSockets.Packet s, uint npc)
        {
            int theme = ThemeOf(npc); if (theme == 0) return "bad";
            if (c.Player.Map != VaultMap) return "far";
            if (c.Player.ActivePick) return "busy";
            uint map = c.Player.Map, dyn = c.Player.DynamicID; ushort x = c.Player.X, y = c.Player.Y;
            c.Player.AddPick(s, "Visiting...", (ushort)VisitSeconds);
            System.Threading.Tasks.Task.Delay(VisitSeconds * 1000).ContinueWith(_ =>
            {
                try
                {
                    if (c.Player == null || !c.Player.ActivePick || c.Player.Map != map || c.Player.DynamicID != dyn || c.Player.X != x || c.Player.Y != y) return;
                    c.Player.ActivePick = false;
                    if (!PT.Running) return;
                    using (var rec = new ServerSockets.RecycledPacket()) Visited(c, rec.GetStream(), theme);
                }
                catch (Exception e) { MyConsole.SaveException(e); }
            });
            return "visiting";
        }
        static void Visited(Client.GameClient c, ServerSockets.Packet s, int theme)
        {
            uint uid = c.Player.UID; var cfg = PT.Cfg;
            int frag = FragmentOf(theme);
            lock (Kit.Lock(uid))
            {
                if (PT.St.Get(uid, "vpDay") != PT.Today && cfg.VaultSp > 0)
                {
                    var sp = new Reward(PT.SP, (uint)cfg.VaultSp);
                    if (PT.Space(c, sp)) { PT.St.Set(uid, "vpDay", PT.Today); PT.Give(c, s, "vault palace visit", sp); PT.Msg(c, PT.Fmt(PT.T("Npc[Sys][TiliBao]"), cfg.VaultSp)); }
                }
            }
            // the spirit statue`s fragment only when the visit was made at that statue (not from Orion`s statue view)
            if (frag >= 0 && Math.Abs(c.Player.X - Spots[theme][0]) <= 16 && Math.Abs(c.Player.Y - Spots[theme][1]) <= 16) PuppetNpcs.GotFragment(c, frag);
        }

        // ------------------------------------------------------------------ the No.1 hero on the Halloween spot
        static Role.SobNpc _hero; static uint _heroUid;
        /// <summary>PT.Tick: while the No.1 of the Halloween ranking is online, a statue with his look stands on the spot.</summary>
        public static void TickHero(Client.GameClient c)
        {
            try
            {
                uint top = TopUid(HalloweenTheme);
                if (top == 0 || c.Player.UID != top || (_hero != null && _heroUid == top)) return;
                Role.GameMap map; if (!Pool.ServerMaps.TryGetValue(VaultMap, out map)) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    if (_hero != null) { try { Role.Statue.RemoveStatue(s, c, _hero.UID, _hero); } catch { } _hero = null; }
                    var st = new Role.Statue { user = c, UID = Role.Statue.CounterUID.Next, Action = 0, Static = false };
                    var npc = new Role.SobNpc(st) { ObjType = Role.MapObjectType.SobNpc, UID = st.UID, Name = c.Player.Name, Map = VaultMap };
                    npc.X = Spots[HalloweenTheme][0]; npc.Y = Spots[HalloweenTheme][1];
                    npc.MaxHitPoints = int.MaxValue / 2; npc.HitPoints = int.MaxValue / 2;
                    map.View.EnterMap<Role.IMapObj>(npc);
                    Role.Statue.BroadcastStatue(s, npc, map);
                    _hero = npc; _heroUid = top;
                }
                PT.Log(c, "HERO", "statue on the Global Halloween spot");
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
    }
}

namespace Qahr.Mobile.PuppetTheater
{
    /// <summary>[2026-10-15 news page pass] The Vault Palace Seer (official news image: Seer next to Orion, Server_Key
    /// tGloablEventsTemple[34231]): the 2025 Halloween Crystal panel. Crystals (itemtype OFFICIAL): Ziwei 3600810 = Global
    /// Halloween ranking (local equivalent: local ranking top 10, with the ranking reward), Tianji 3600811 = 40000 Terror
    /// Points, Wuqu 3600812 = all dungeon trials passed (all 10 areas cleared). Each once per player.</summary>
    public static class PuppetSeer
    {
        public const uint Seer = 8893052, Ziwei = 3600810, Tianji = 3600811, Wuqu = 3600812;
        static string K(string k) { return "STR_ID_tGloablEventsTemple[34231][" + k + "]"; }
        static string Prog(uint uid, string key) { return PT.Get(uid, key) == 1 ? "1/1" : "0/1"; }
        public static void Open(Client.GameClient c, ServerSockets.Packet s)
        {
            uint uid = c.Player.UID;
            var d = PT.Dialog(c, s, PT.DlgAdv, (cc, ss, o) => { if (o >= 1 && o <= 3) Overview(cc, ss, o); });
            d.AddText(K("Text111") + "@@\n").AddText(K("Text121") + "@@\n").AddText(K("Text122") + "@@\n");
            d.AddText(K("Text123") + "@@2025 Halloween Crystal@@\n").AddText(K("Text124") + "@@\n");
            d.AddText(K("Text125") + "@@Ziwei Halloween Crystal@@On Global Halloween Ranking   " + Prog(uid, "crZ") + "@@\n");
            d.AddText(K("Text126") + "@@Tianji Halloween Crystal@@Accumulating 40000 Terror Points   " + Prog(uid, "crT") + "@@\n");
            d.AddText(K("Text127") + "@@Wuqu Halloween Crystal@@Passing all Dungeon Trials   " + Prog(uid, "crW") + "@@\n");
            d.AddText(K("Text128") + "@@\n");
            d.AddOption(K("Option121") + "@@\n", 1).AddOption(K("Option122") + "@@\n", 2).AddOption(K("Option123") + "@@\n", 3).AddOption(K("Option124") + "@@\n", 255).FinalizeDialog();
        }
        static void Overview(Client.GameClient c, ServerSockets.Packet s, int kind)
        {
            uint uid = c.Player.UID; string p = (kind + 2).ToString();   // 3 Ziwei / 4 Tianji / 5 Wuqu
            string key = kind == 1 ? "crZ" : kind == 2 ? "crT" : "crW";
            var d = PT.Dialog(c, s, PT.DlgAdv, (cc, ss, o) => { });
            d.AddText(K("Text" + p + "11") + "@@\n").AddText(K("Text" + p + "12") + "@@\n").AddText(K("Text" + p + "13") + "@@\n");
            d.AddText(K("Text" + p + "19") + "@@" + Prog(uid, key) + "@@\n");   // only the 2025 Global Halloween row is tracked here
            d.AddText(K("Text" + p + "115") + "@@\n");
            d.AddOption(K("Option" + p + "11") + "@@\n", 255).FinalizeDialog();
        }
        static void Grant(Client.GameClient c, string key, uint item, string why)
        {
            uint uid = c.Player.UID;
            if (PT.Get(uid, key) == 1) return;
            var r = new Reward(item, 1, true);
            if (!PT.Space(c, r)) { PT.Msg(c, "Make room in your inventory to receive your Halloween Crystal."); return; }
            PT.St.Set(uid, key, 1);
            using (var rec = new ServerSockets.RecycledPacket()) PT.Msg(c, "You obtained " + PT.Give(c, rec.GetStream(), why, r) + ".");
        }
        /// <summary>Called by PT.Tick (cheap): pays the crystals whose condition is met (retries when the bag was full).</summary>
        public static void Check(Client.GameClient c)
        {
            uint uid = c.Player.UID;
            if (PT.Get(uid, "crT") == 0 && PT.Get(uid, "pts") >= 40000) Grant(c, "crT", Tianji, "crystal tianji");
            if (PT.Get(uid, "crW") == 0 && Enumerable.Range(1, 10).All(L => PuppetMaze.Cleared(uid, L))) Grant(c, "crW", Wuqu, "crystal wuqu");
        }
        public static void RankPaid(Client.GameClient c) { Grant(c, "crZ", Ziwei, "crystal ziwei (local ranking)"); }
    }
}
