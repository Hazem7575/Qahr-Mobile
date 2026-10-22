// ============================================================================
//  [Summer2025 2026-10-14] Official 2025 Global Summer "Sylvan Whisper" / "Volley Garden" entry
//  (client msg 10707 tMsg_Info["Summer25"], 【简体征服】2025全球暑期活动, English CO July 28 - Aug. 10),
//  shown while the festival Summer2025 runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150306"] (img lua_2025qqsq_btn2), clickfunc
//              "SummerDraw2025_OpenMainView</N>0" on msg 10707. Pushed by festival.json icon_show / icon_hide.
//              (FGUI_ActivityMenu["150437"] lua_yfyh_btn7 also sends on 10707, but its func SummerActive2023Page_OpenMainInterface
//               opens the Dream Cruise window of Summer2023 / Set Sail 2026: NOT this event, not shown here.)
//    window    h_fuifunction SummerVacation25_OpenData(sData) -> fairygui\window\summervacation (pack luaui_25summermain):
//              Data[1] = {{sybid,count},..}  seeds (sybid 225003001 = Midsummer/Morning Seed 3354231; Elf/Volley Seed 3354230 has no sybid)
//              Data[2] = {points}            "Points: %s"
//              Data[3] = {red x4}
//              Data[4] = {cost, quickCost}   seeds per planting / per Quick Planting      (DERIVED: client test data {1,5})
//              Data[5] = targeted flower 0..4
//              Data[6] = planted lands {{land,{type,lv,sybid}},..}
//              Data[7] = {done,max}          "Woodland Watch (%s/%s)"                      (DERIVED max: client test data 3)
//              Data[8] = 4 flower sybids     Lv1 show items of luauirewardshow [225]..[228] (Awakening/Beast/+Stone/Contribution)
//              Data[9] = {state,..}          state 2 -> STR_LUAUI_SUMMERVACATION_TALK_3
//              Data[10]/[11] skip / quick flags
//    preview   SummerDraw2025_RewardPreview -> g_function Fui_Summer25Rward_Open (window Summer25 150303):
//              [1][k] = {22k001000,22k002000,22k003000}, [2] = 230001000, [3] = 229001000 (Sylvan Bloom) - official sybids.
//    highlight ActInfo[10707] "Volley Garden" (10511 LUAViewSummary_ClientEnterButton 10707).
//  Texts: luacfg Server_Key tSummer2025Npc / tSummerDraw2025 (official English), Cn_Res STR_LUAUI_SUMMERVACATION_*.
//
//  REAL data only: seed count from the bag (the seed items 3354230/3354231 are NOT in this client's itemtype, so it is 0).
//  NOT on this server (system message, nothing granted): planting/harvest (no seeds exist), Flickerwood instance,
//  Woodland Watch, Undying Blight, points ranking, shops.
//  Config (optional, derived values): Infinity[DB]\Summer2025\summer2025.json.
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10707 SummerDraw2025_* / Summer2025_* / SummerActive2025*, and 10511 act 10707).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.Summer2025
{
    public static class Summer2025Ui
    {
        public const uint LuaMsg = 10707;            // tMsg_Info["Summer25"]
        public const uint HighlightMsg = 10511;
        public const string HighlightAct = "10707";
        public const uint MidsummerSeed = 3354231;   // 晨曦花种 (luauirewardshow 225003001)
        public const uint ElfSeed = 3354230;         // SummerVacation25_OpenGuideBuy_2 item
        public const uint SeedSybId = 225003001;

        public sealed class Config
        {
            public string Festival = "Summer2025";
            public int MinLevel = 80;                // official "Requirement: Level 80"
            public uint SowCost = 1, QuickCost = 5;  // derived (client test data)
            public uint WatchMax = 3;                // derived (client test data)
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "Summer2025", "summer2025.json"); } }
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
                    if (fi.Exists)
                    {
                        var d = FL.ReadJson(fi.FullName);
                        var f = FL.Str(d, "festival"); if (!string.IsNullOrWhiteSpace(f)) c.Festival = f.Trim();
                        var lv = FL.Val(d, "minLevel"); if (lv != null) c.MinLevel = (int)Math.Min(255, FL.U(lv));
                        var a = FL.Val(d, "sowCost"); if (a != null) c.SowCost = Math.Max(1, Math.Min(999, FL.U(a)));
                        var b = FL.Val(d, "quickCost"); if (b != null) c.QuickCost = Math.Max(1, Math.Min(999, FL.U(b)));
                        var w = FL.Val(d, "woodlandWatchMax"); if (w != null) c.WatchMax = Math.Min(99, FL.U(w));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[Summer2025] summer2025.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (Server_Key tSummer2025Npc[34415], tSummerDraw2025[Sys][NoItem])
        const string TxtOver = "The Sylvan Whisper event has concluded perfectly. Looking forward to our next collaboration!";
        const string TxtLevel = "Please reach level 80 first.";
        const string TxtNoSeed = "You lack sufficient Midsummer Seeds.";
        const string TxtNotOpen = " isn`t open on this server yet.";

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, int> _target = new ConcurrentDictionary<uint, int>();   // Targeted Planting choice (UI only)
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>LuaPush.HandleCallback (msg 10707, and 10511 for the Highlight page 10707). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool open;
            if (msgId == HighlightMsg)
            {
                if (func != "LUAViewSummary_ClientEnterButton" || args == null || args.Length == 0 || args[0] != HighlightAct) return false;
                open = true;
            }
            else
            {
                if (msgId != LuaMsg) return false;
                // SummerActive2023Page_* (button 150437) belongs to Summer2023's Dream Cruise window: not ours
                if (!func.StartsWith("SummerDraw2025_", StringComparison.Ordinal) && !func.StartsWith("Summer2025_", StringComparison.Ordinal)
                    && !func.StartsWith("SummerActive2025", StringComparison.Ordinal)) return false;
                open = func == "SummerDraw2025_OpenMainView";
            }
            if (Throttled(c.Player.UID)) return true;
            if (func == "SummerDraw2025_Skip" || func == "SummerDraw2025_OneClick" || func == "SummerDraw2025_HarvestCongratulation") return true;// client-side flags only
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < Cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            if (open) { Lua(c, OpenCall(c, Cfg)); return true; }
            switch (func)
            {
                case "SummerDraw2025_RewardPreview": Lua(c, PreviewCall()); return true;
                case "SummerDraw2025_SelectConfirm":
                    {
                        int n; if (args == null || args.Length == 0 || !int.TryParse(args[0], out n) || n < 0 || n > 4) return true;
                        _target[c.Player.UID] = n;
                        Lua(c, "SummerVacation25_SelectReturn|{['Code']=200,['Data']={" + n + "}}");
                        return true;
                    }
                case "SummerDraw2025_SowSeeds":
                case "SummerDraw2025_Harvest":
                    // nothing planted and no seeds can exist here (items missing from itemtype): never grants anything
                    Msg(c, TxtNoSeed);
                    return true;
                case "SummerDraw2025_DailyView":
                case "SummerActive2025_MainInto": Msg(c, "Woodland Watch" + TxtNotOpen); return true;
                case "SummerActive2025Clear_EnterGame": Msg(c, "Flickerwood" + TxtNotOpen); return true;
                case "Summer2025_OpenStagePage": Msg(c, "The Points Ranking" + TxtNotOpen); return true;
                case "SummerDraw2025_ExchgangeShop":
                case "SummerDraw2025_EmoneyShop": Msg(c, "The Sylvan Whisper store" + TxtNotOpen); return true;
            }
            Msg(c, "This part of Sylvan Whisper" + TxtNotOpen);
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>SummerVacation25_OpenData: only real data (bag seeds, nothing planted, no points).</summary>
        public static string OpenCall(Client.GameClient c, Config cfg)
        {
            int target = 0;
            if (c != null && c.Player != null) _target.TryGetValue(c.Player.UID, out target);
            var sb = new StringBuilder("SummerVacation25_OpenData|{['Code']=200,['Data']={");
            sb.Append("{{").Append(SeedSybId).Append(',').Append(c == null ? 0 : Count(c, MidsummerSeed)).Append("}},"); // [1]
            sb.Append("{0},");                                                     // [2] points
            sb.Append("{0,0,0,0},");                                               // [3] red points
            sb.Append('{').Append(cfg.SowCost).Append(',').Append(cfg.QuickCost).Append("},"); // [4] derived
            sb.Append(target).Append(',');                                         // [5]
            sb.Append("{},");                                                      // [6] nothing planted
            sb.Append("{0,").Append(cfg.WatchMax).Append("},");                    // [7] Woodland Watch
            sb.Append("{225001000,226001000,227001000,228001000},");               // [8] official sybids
            sb.Append("{2},");                                                     // [9] "Journey to Flickerwood..."
            sb.Append("0,0");                                                      // [10] skip, [11] quick
            sb.Append("}}");
            return sb.ToString();
        }

        /// <summary>Fui_Summer25Rward_Open: the official reward preview (luauirewardshow sybids).</summary>
        public static string PreviewCall()
        {
            return "Fui_Summer25Rward_Open|{['Code']=200,['Data']={{{225001000,225002000,225003000},{226001000,226002000,226003000},"
                 + "{227001000,227002000,227003000},{228001000,228002000,228003000}},230001000,229001000}}";
        }
    }
}
