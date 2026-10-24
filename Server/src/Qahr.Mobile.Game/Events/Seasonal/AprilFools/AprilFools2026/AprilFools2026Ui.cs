// ============================================================================
//  [AprilFools2026 2026-10-14] The official 2026 Global April Fools "Astral Dream" entry (Dream Town / Jack),
//  client ERP660707 2026全球愚人节活动 3.31-4.13. Shown while the festival AprilFools2026 runs.
//  (AprilFools2025 "Fool`s Paradise" and AliceRescue are other years: not touched.)
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150415"] (img lua_2026qqyrj_1) clientfunc: unless the night window
//              Fgui_26QqYrj_Hysr or the day window Foolsday26Day is open, Public_CallServerFunc(tMsg_Info["MidAutumnFestival"]
//              = 10052, "MidAutumnArchery2022_OpenMainButton"). Pushed by festival.json icon_show / icon_hide.
//    night     interfacecfg 1005298 -> Fui_Fgui_26QqYrj_Hysr_Open (1005299 = _Update), sent through
//              InfiniteReceiverData|{Code=200,Step=1,InterfaceID=1005298,Data={...}}. Data (fgui_26qqyrj_hysrmediator):
//                [1] {10 x 0/1}   haze cells, 0 = not dispelled          [2] {{state,boss}x4} 0 fightable / 1 killed, boss 1..6
//                [3] Astral Points (0..100)                               [4] event points
//                [5] {sybid, n}  Serene Stars (luauirewardshow 417001000) [6] {red x3} refine / day-night / points
//                [7] {{sybid,n}x3} final-dispel drops                     [8] {{sybid,n}x3} materials (com_huobi_1..3)
//    clicks    (all msg 10052) btn_sheri -> MidAutumnArchery2022_KillBirdRefh[</N>2], btn_boss/battle ->
//              MidAutumnArchery2022_KillMonster</N>i, btn_zyjt -> MidAutumnArchery2022_TransDay, btn_draw ->
//              ZhongQiu2022Npc_OpenAlchemyInterface, points -> ZhongQiu2022Cross_OpenRankInterface</N>1.
//    refine    h_fuifunction Foolsday26Blast_OpenData(sData) {Item={{Id=sybid,Num=n}x3},Point={cur,max},Red,IsSkip}
//              (foolsday26blastdata: materials 3336640 / 3336641 / 3336642 in this order). Buttons: ZhongQiu2022Npc_Refine,
//              _ClickPlusMaterial, _RewardPre, _ExchagneView, MidAutumnOther_TSStore, back = MidAutumnArchery2022_OpenMainButton.
//    day       Foolsday26Day (MoonFestivalPage_EnterOS / _Pound / _NightEventTip) is reached only through the Astral Wheel.
//  Texts: luacfg Server_Key tMidAutumnArchery2022_DelText (official 2026 English), Cn_Res STR_LUAUI_26QQYRJ_* / 26FOOLSDAYA_*.
//
//  REAL data only: Serene Star / Memory Essence / Dream Crystal / Hope Bouquet counts from the bag. Astral Points 0,
//  event points 0, haze 0/10, no nightmare beasts (state 1 = hidden; which beasts the night holds is server-side).
//  NOT on this server (system message, nothing granted or taken): Dispel Haze, beasts, Astral Wheel / day view,
//  crafting Dream Essences (reward pool server-side), ranking, Astral Store.
//  Config (optional): Data\AprilFools2026\aprilfools2026.json {festival, openFromMaps}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10052, only the functions below), tagged [AprilFools2026 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.AprilFools2026
{
    public static class AprilFools2026Ui
    {
        public const uint LuaMsg = 10052;              // tMsg_Info["MidAutumnFestival"]
        public const uint InterfaceOpen = 1005298;     // Fui_Fgui_26QqYrj_Hysr_Open
        // items (itemtype) and their luauirewardshow ids
        public const uint SereneStar = 3336643, MemoryEssence = 3336640, DreamCrystal = 3336641, HopeBouquet = 3336642;
        public const uint SybSereneStar = 417001000, SybMemoryEssence = 409001000, SybDreamCrystal = 409001001, SybHopeBouquet = 409001002;

        public sealed class Config
        {
            public string Festival = "AprilFools2026";
            public HashSet<uint> OpenFrom = new HashSet<uint> { 1002 };   // official [NoMap] "You can only enter the Astral Dream from Twin City."
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "AprilFools2026", "aprilfools2026.json"); } }
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
                        var from = FL.Arr(FL.Val(d, "openFromMaps")); if (from != null) c.OpenFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[AprilFools2026] aprilfools2026.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // official texts (Server_Key tMidAutumnArchery2022_DelText, 2026 English)
        const string TxtNoMap = "You can only enter the Astral Dream from Twin City.";
        const string TxtNoArrow = "You aren`t carrying any Serene Stars and cannot dispel the nightmare haze.";
        // derived (polite, nothing invented)
        const string TxtOver = "Astral Dream isn`t running right now.";
        const string TxtNotOpen = " isn`t open on this server yet.";

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

        /// <summary>The 10052 functions this event's windows send (10052 is the old Mid-Autumn id: claim only these).</summary>
        static readonly HashSet<string> Funcs = new HashSet<string>(StringComparer.Ordinal)
        {
            "MidAutumnArchery2022_OpenMainButton", "MidAutumnArchery2022_KillBirdRefh", "MidAutumnArchery2022_KillMonster",
            "MidAutumnArchery2022_TransDay", "ZhongQiu2022Npc_OpenAlchemyInterface", "ZhongQiu2022Cross_OpenRankInterface",
            "ZhongQiu2022Npc_RewardPre", "ZhongQiu2022Npc_ClickPlusMaterial", "ZhongQiu2022Npc_Refine", "ZhongQiu2022Npc_ExchagneView",
            "MidAutumnOther_TSStore", "ZhongQiu2022Npc_RewardTip", "ZhongQiu2022Npc_Click", "ZhongQiu2022Npc_SetExchange", "MoonFestivalPage_EnterOS", "MoonFestivalPage_Pound", "MoonFestivalPage_NightEventTip",
        };

        /// <summary>LuaPush.HandleCallback (msg 10052). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Funcs.Contains(func)) return false;
            if (!Running)
            {
                if (func != "MidAutumnArchery2022_OpenMainButton") return false;   // leave the old Mid-Autumn ids to their owner
                if (!Throttled(c.Player.UID)) Msg(c, TxtOver);
                return true;
            }
            if (Throttled(c.Player.UID)) return true;
            switch (func)
            {
                case "MidAutumnArchery2022_OpenMainButton":
                    if (c.Player.DynamicID != 0 || !Cfg.OpenFrom.Contains(c.Player.Map)) { Msg(c, TxtNoMap); return true; }
                    Lua(c, OpenCall(c));
                    return true;
                case "MidAutumnArchery2022_KillBirdRefh":
                    if (Count(c, SereneStar) == 0) { Msg(c, TxtNoArrow); return true; }
                    Msg(c, "Dispelling the nightmare haze" + TxtNotOpen); return true;
                case "MidAutumnArchery2022_KillMonster": Msg(c, "The nightmare beasts" + TxtNotOpen); return true;
                case "MidAutumnArchery2022_TransDay":
                case "MoonFestivalPage_EnterOS":
                case "MoonFestivalPage_Pound":
                case "MoonFestivalPage_NightEventTip": Msg(c, "The Astral Wheel (Day-Night Cycle)" + TxtNotOpen); return true;
                case "ZhongQiu2022Npc_OpenAlchemyInterface": Lua(c, BlastCall(c)); return true;
                case "ZhongQiu2022Cross_OpenRankInterface": Msg(c, "The Astral Dream rankings and rewards" + TxtNotOpen); return true;
                case "MidAutumnOther_TSStore": Msg(c, "The Astral Store" + TxtNotOpen); return true;
                case "ZhongQiu2022Npc_ExchagneView":
                case "ZhongQiu2022Npc_SetExchange": Msg(c, "Dismantling rewards into Astral Coins" + TxtNotOpen); return true;
                case "ZhongQiu2022Npc_Click": return true;                                  // the window's own "Skip" toggle: nothing to store
                case "ZhongQiu2022Npc_RewardTip":
                case "ZhongQiu2022Npc_RewardPre":
                case "ZhongQiu2022Npc_ClickPlusMaterial":
                case "ZhongQiu2022Npc_Refine": Msg(c, "Refining Dream Essences" + TxtNotOpen); return true;
            }
            Msg(c, "This part of Astral Dream" + TxtNotOpen);
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            if (c == null) return 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>Night window (InterfaceID 1005298). Real data: bag counts; no haze dispelled, no beasts, 0 points.</summary>
        public static string OpenCall(Client.GameClient c)
        {
            var sb = new StringBuilder("InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=").Append(InterfaceOpen).Append(",['Data']={");
            sb.Append("{0,0,0,0,0,0,0,0,0,0},");                                           // [1] haze: nothing dispelled
            sb.Append("{{1,0},{1,0},{1,0},{1,0}},");                                       // [2] no beasts here (hidden)
            sb.Append("0,0,");                                                             // [3] Astral Points, [4] event points
            sb.Append('{').Append(SybSereneStar).Append(',').Append(Count(c, SereneStar)).Append("},"); // [5] Serene Stars
            sb.Append("{0,0,0},");                                                         // [6] red dots
            sb.Append("{},");                                                              // [7] no drops
            sb.Append("{{").Append(SybMemoryEssence).Append(',').Append(Count(c, MemoryEssence)).Append("},{")
              .Append(SybDreamCrystal).Append(',').Append(Count(c, DreamCrystal)).Append("},{")
              .Append(SybHopeBouquet).Append(',').Append(Count(c, HopeBouquet)).Append("}}");          // [8] materials
            return sb.Append("}}").ToString();
        }

        /// <summary>Refine Dream Essences (Foolsday26Blast_OpenData). Real material counts; no reward preview.</summary>
        public static string BlastCall(Client.GameClient c)
        {
            var sb = new StringBuilder("Foolsday26Blast_OpenData|{['Code']=200,['Data']={['Item']={");
            sb.Append("{['Id']=").Append(SybMemoryEssence).Append(",['Num']=").Append(Count(c, MemoryEssence)).Append("},");
            sb.Append("{['Id']=").Append(SybDreamCrystal).Append(",['Num']=").Append(Count(c, DreamCrystal)).Append("},");
            sb.Append("{['Id']=").Append(SybHopeBouquet).Append(",['Num']=").Append(Count(c, HopeBouquet)).Append('}');
            sb.Append("},['Point']={0,0},['Red']=0,['IsSkip']=0,['IsFirst']=0,['BtnOn']={0,0}}}");
            return sb.ToString();
        }
    }
}
