// ============================================================================
//  [MidAutumn2024 2026-10-14] "Lunar Voyage" - the official 2024 Global Mid-Autumn entry (ERP592820, client msg 10459
//  tMsg_Info["FuiZhongqiu2024"]). Shown while the festival MidAutumnVillage (Moon Palace, Heart Grass 3600735) runs.
//
//  Official client (luaui, decrypted):
//    top bar   fairygui\window\config\fguibtnconfig  FGUI_ActivityMenu["10459"] (img lua_2024qqzqj_btn32_1, EN label "Lunar Voyage")
//              shown by the Fgui top-bar list (LuaPush.TopBarData) and by
//              FguiActivityMenu_UpdateSigle|{['Code']=200,['Data']={['Id']='10459',['Show']=true|false,['Red']=0}}
//              (festival.json icon_show / icon_hide). The old lua button DlgZhongQiu2024Btn (CN-only image) is NOT used.
//    click     10459 MainNpc2024_Interface  ->  Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_FuiZhongQiu2024Main_Open',
//              List={6 x {Lock,Red,Reward={{done,'item'}[,{done,'item'}]}}},Red={moon,bounty,attic}}}
//              + Cmd_FuiZhongQiu2024Main_UpdateItem Data.Item = {'3600735 0 n','3349919 0 n','3349920 0 n'} (bag counts)
//    window    MainNpc2024_InterfaceGoto</N>1..6 (voyage areas), 7 Moon Discovery, 8 Bounty Park, 9 Treasure Attic
//              MidAutumn2024Shop_OpenRule</N>n -> ZhongQiu2024_OpenRule|{Code=200,Data={Id=n}} (texts luaUiRes[10459].Rule)
//              MainNpc2024_InterfaceScore -> Cmd_FuiZhongQiu2024Main_UpdatePoint {Point,Red}
//              MainNpc2024_GuideSet</N>n (guide progress, acknowledged)
//  NOT on this server (answered with a system message): the Jade Rabbit voyage (Exploration window, Voyage Points, endings),
//  the ranking (ZhongQiu2024Kua_StageView), Bounty Park (cross-server) and the Treasure Attic shop. Voyage Points are 0 and
//  every ending is "not achieved" because nothing awards them yet.
//  Config (derived values): Infinity[DB]\MidAutumn2024\lunarvoyage.json.
//
//  Hooks: Base\LuaUi\LuaPush.cs TopBarData (+ ids while running) and HandleCallback (msg 10459), both tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.MidAutumn2024
{
    public static class LunarVoyage
    {
        public const uint LuaMsg = 10459;
        public const string TopBarId = "10459";

        public sealed class Config
        {
            public string Festival = "MidAutumnVillage";
            public int MinLevel = 80;
            public uint[] Currency = { 3600735, 3349919, 3349920 };
            public uint MoonMap = 12597; public ushort MoonX = 264, MoonY = 266;
            public HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 12597 };
            public static readonly int[] Endings = { 1, 1, 1, 2, 2, 2 };// official: areas 4-6 have a hidden 2nd ending (Cn_Res JJ_x_2)
            public string[][] EndingRewards = Endings.Select(n => Enumerable.Repeat("", n).ToArray()).ToArray();
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "MidAutumn2024", "lunarvoyage.json"); } }
        public static Config Cfg
        {
            get
            {
                try
                {
                    var fi = new FileInfo(CfgFile);
                    string stamp = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && stamp == _stamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists) Read(c, FL.ReadJson(fi.FullName));
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[MidAutumn2024] lunarvoyage.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }
        static void Read(Config c, Dictionary<string, object> d)
        {
            if (d == null) return;
            var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
            var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
            var cur = FL.Arr(FL.Val(d, "currency"));
            if (cur != null && cur.Count == 3) c.Currency = cur.Select(FL.U).ToArray();
            var md = FL.Val(d, "moonDiscovery") as Dictionary<string, object>;
            if (md != null) { c.MoonMap = FL.U(FL.Val(md, "map")); c.MoonX = (ushort)FL.U(FL.Val(md, "x")); c.MoonY = (ushort)FL.U(FL.Val(md, "y")); }
            var from = FL.Arr(FL.Val(d, "teleportFromMaps"));
            if (from != null) c.TeleportFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
            var er = FL.Arr(FL.Val(d, "endingRewards"));
            if (er != null)
                for (int i = 0; i < 6 && i < er.Count; i++)
                {
                    var l = FL.Arr(er[i]);
                    if (l == null) continue;
                    for (int j = 0; j < Config.Endings[i] && j < l.Count; j++) c.EndingRewards[i][j] = SafeItem(l[j] == null ? "" : l[j].ToString());
                }
        }
        /// <summary>Only "digits[ digits...]" item strings go to the client.</summary>
        static string SafeItem(string s)
        {
            s = (s ?? "").Trim();
            return s.All(ch => char.IsDigit(ch) || ch == ' ') ? s : "";
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        /// <summary>LuaPush.TopBarData hook: the extra top-bar ids while the festival runs.</summary>
        public static IEnumerable<string> TopBarIds() { return Running ? new[] { TopBarId } : new string[0]; }

        // ------------------------------------------------------------------ callbacks (msg 10459)
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10459).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (func == "MainNpc2024_GuideSet") return true;// guide progress: nothing to store
            if (Throttled(c.Player.UID)) return true;
            if (!Running)
            {
                Msg(c, "The Mid-Autumn event has ended. See you next year!");
                return true;
            }
            var cfg = Cfg;
            switch (func)
            {
                case "MainNpc2024_Interface":
                    if (c.Player.Level < cfg.MinLevel) { Msg(c, "You need to reach Level " + cfg.MinLevel + " to join the Mid-Autumn event."); return true; }
                    Open(c, cfg);
                    return true;
                case "MainNpc2024_InterfaceMaterial":
                    SendItems(c, cfg);
                    return true;
                case "MainNpc2024_InterfaceScore":// [JadeRabbit 2026-10-15] real Voyage Points
                    Lua(c, "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_FuiZhongQiu2024Main_UpdatePoint',['Point']=" + JadeRabbit.JadeRabbitVoyage.Points(c.Player.UID) + ",['Red']=" + (JadeRabbit.JadeRabbitVoyage.StageRed(c.Player.UID) ? 1 : 0) + "}}");
                    return true;
                case "MidAutumn2024Shop_OpenRule":
                    {
                        int id;
                        if (args.Length > 0 && int.TryParse(args[0], out id) && id >= 1 && id <= 4)
                            Lua(c, "ZhongQiu2024_OpenRule|{['Code']=200,['Data']={['Id']=" + id + "}}");
                        return true;
                    }
                case "MainNpc2024_InterfaceGoto":
                    {
                        int id;
                        if (args.Length == 0 || !int.TryParse(args[0], out id)) return true;
                        if (id == 7) { MoonDiscovery(c, cfg); return true; }
                        if (id >= 1 && id <= 6) JadeRabbit.JadeRabbitVoyage.Open(new MidAutumnReunion2022.ClientIo(c), id);// [JadeRabbit 2026-10-15] the Jade Rabbit exploration of area id (all 6); story quests 8066/8067/8073 follow the first ending
                        else if (id == 8) Msg(c, "Bounty Park isn`t open on this server yet.");
                        else if (id == 9) Msg(c, "The Treasure Attic isn`t open on this server yet.");
                        return true;
                    }
                case "ZhongQiu2024Kua_StageView":// [JadeRabbit 2026-10-15] Voyage Point rewards (PointsRank 10459)
                    Lua(c, "Fgui_PointsRank_Open|" + JadeRabbit.JadeRabbitVoyage.StageData(new MidAutumnReunion2022.ClientIo(c)));
                    return true;
                case "ZhongQiu2024Kua_StageReward":
                    { int sid; if (args.Length > 0 && int.TryParse(args[0], out sid)) JadeRabbit.JadeRabbitVoyage.ClaimStage(new MidAutumnReunion2022.ClientIo(c), sid); return true; }
                case "ZhongQiu2024Kua_OpenRankView":
                    JadeRabbit.JadeRabbitVoyage.Leaderboard(new MidAutumnReunion2022.ClientIo(c));
                    return true;
                case "MidAutumn2024Shop_Open":
                    Msg(c, "The Treasure Attic isn`t open on this server yet.");
                    return true;
                case "MidAutumnAdventure_ClickChou": JadeRabbit.JadeRabbitVoyage.Explore(new MidAutumnReunion2022.ClientIo(c)); return true;// [JadeRabbit 2026-10-15]
                case "MidAutumnAdventure_ClickEndChou": JadeRabbit.JadeRabbitVoyage.Terminus(new MidAutumnReunion2022.ClientIo(c)); return true;
                case "MidAutumnAdventure_OpenWay":
                case "MidAutumnAdventure_GetWay":
                    Msg(c, "Heart Grass: complete the daily quests of Fairy Magpie and Fairy Bell in the Moon Palace (Moon Discovery).");
                    return true;
            }
            return false;
        }

        public static string OpenData(Config cfg, Client.GameClient c = null)// [JadeRabbit 2026-10-14] c: per-player ending flags
        {
            var sb = new StringBuilder("Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_FuiZhongQiu2024Main_Open',['List']={");
            for (int i = 0; i < 6; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['Lock']=0,['Red']=0,['Reward']={");
                for (int j = 0; j < Config.Endings[i]; j++)
                {
                    if (j > 0) sb.Append(',');
                    sb.Append('{').Append(c != null ? JadeRabbit.JadeRabbitUi.EndingDone(c, i + 1, j + 1) : 0).Append(',').Append(LuaUi.LuaPush.Q(cfg.EndingRewards[i][j].Length > 0 ? cfg.EndingRewards[i][j] : JadeRabbit.JadeRabbitUi.EndingPreview(i + 1, j + 1))).Append('}');// [JadeRabbit 2026-10-15] 1 = ending j achieved (exploration terminus)
                }
                sb.Append("}}");
            }
            return sb.Append("},['Red']={0,0,0}}}").ToString();
        }
        static void Open(Client.GameClient c, Config cfg)
        {
            Lua(c, OpenData(cfg, c));// [JadeRabbit 2026-10-14]
            SendItems(c, cfg);
        }
        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        static void SendItems(Client.GameClient c, Config cfg)
        {
            var sb = new StringBuilder("Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_FuiZhongQiu2024Main_UpdateItem',['Item']={");
            for (int i = 0; i < cfg.Currency.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('\'').Append(cfg.Currency[i]).Append(" 0 ").Append(Count(c, cfg.Currency[i])).Append('\'');
            }
            Lua(c, sb.Append("}}}").ToString());
        }
        /// <summary>GoTo 7 "Moon Discovery": to the Moon Palace (the herald's destination), only from a city / the palace.</summary>
        static void MoonDiscovery(Client.GameClient c, Config cfg)
        {
            var p = c.Player;
            if (p.Level < cfg.MinLevel) { Msg(c, "You need to reach Level " + cfg.MinLevel + " to visit the Moon Palace."); return; }
            if (!p.Alive || p.DynamicID != 0 || !cfg.TeleportFrom.Contains(p.Map))
            {
                Msg(c, "You can`t go to the Moon Palace from here. Talk to Chang`E in Twin City.");
                return;
            }
            if (cfg.MoonMap == 0 || Pool.ServerMaps[cfg.MoonMap] == null) { Msg(c, "The Moon Palace isn`t open yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, cfg.MoonX, cfg.MoonY, cfg.MoonMap, 0); else c.Teleport(cfg.MoonX, cfg.MoonY, cfg.MoonMap, 0);
        }
    }
}
