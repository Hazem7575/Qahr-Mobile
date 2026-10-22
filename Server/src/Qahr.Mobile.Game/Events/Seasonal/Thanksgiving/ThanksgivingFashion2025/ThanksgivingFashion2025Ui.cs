// ============================================================================
//  [ThanksgivingFashion2025 2026-10-14] The official 2025 Global Thanksgiving "Fashion Star" / Top Master Campaign
//  (Skyreach), client msg 10789 tMsg_Info["DaRenXiu"] (ERP635973 全球时尚达人活动 11.25-12.15).
//  Shown while the festival ThanksgivingFashion2025 runs. (Thanksgiving2024 / ThanksgivingTribe2023 are other years.)
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150361"] (img lua_2025qqssdr_btn9, fguiname TalentShow),
//              clickfunc ThanksgivingNpc2025_MainView</N>0 on msg 10789. Pushed by festival.json icon_show / icon_hide.
//    window    g_function Fui_TalentShow_Open(sParam) (interfacecfg 10789001) -> fairygui\window\talentshow:
//                Data = { uitype, {red Main Hall, red Arena}, subdata }   uitype 1 com_rk (Main Hall), 2 com_fyzd,
//                3 com_pingxuan, 4 com_rank, 5 com_ltz, 6 com_txdy (talentshowdata const_uiname).
//              com_rk subdata (fgui_25drx_rkmediator): { {score, max, red}, {sybid, count}, {red x6}, {state x6} }
//                state 0 -> Cn_Res STR_LUAUI_25QQDRX_ZRKJM_TIME_i "Open Time: Nov. 25 - Dec. 8", else "Ended".
//    clicks    (all msg 10789) tabs ThanksgivingNpc2025_MainView</N>0 / ThanksgivingNpc2025_SelectionView</N>0,
//              entries Thanksgiving2025Colorful_GoAndCollect</N>1</N>1, TalentShowPart2_OpenTowerView,
//              TalentShowPart3_OpenMineSweeperView, Thanksgiving2025Hunt_ClientRank, Thanksgiving2025Hunt_ClientOpen,
//              ThanksgivingNpc2025_OpenExchangeStore; "+" TalentShowPart3_ItemGetView; hunt mini window
//              Thanksgiving2025Hunt_ClientBook / _ClientEnter.
//  Texts: luacfg Server_Key tThanksgiving2025Hunt_DelText / tTalentShowPart3 (official English), Cn_Res STR_LUAUI_DARENXIU_*.
//
//  REAL data only: the Skyreach Stone (3600820, luauirewardshow 308001001) count from the bag. Skyreach Points 0,
//  no red dots, every entry shows its official open time while the festival runs.
//  Points bar max = config pointsMax (derived, default 1 = empty bar; the official stage table is server-side).
//  NOT on this server (system message, nothing granted): Splendor Pavilion, Pinnacle Tower, Land of Fortune,
//  Skyreach Hunt (realm map not mapped), rankings, Arena / Top Master voting, Skyreach Vault.
//  Config (optional): Infinity[DB]\ThanksgivingFashion2025\thanksgivingfashion2025.json {festival, pointsMax}.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10789), tagged [ThanksgivingFashion2025 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.ThanksgivingFashion2025
{
    public static class ThanksgivingFashion2025Ui
    {
        public const uint LuaMsg = 10789;               // tMsg_Info["DaRenXiu"]
        public const uint SkyreachStone = 3600820, SybSkyreachStone = 308001001;

        public sealed class Config
        {
            public string Festival = "ThanksgivingFashion2025";
            public uint PointsMax = 1;                  // derived: no official stage table in the client
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "ThanksgivingFashion2025", "thanksgivingfashion2025.json"); } }
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
                        var pm = FL.Val(d, "pointsMax"); if (pm != null) c.PointsMax = Math.Max(1u, Math.Min(100000000u, FL.U(pm)));
                    }
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[ThanksgivingFashion2025] thanksgivingfashion2025.json: " + e.Message, ConsoleColor.Yellow); } catch { }
                    if (_cfg == null) _cfg = new Config();
                }
                return _cfg;
            }
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        // derived (polite, nothing invented)
        const string TxtOver = "The Top Master Campaign isn`t running right now.";
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

        static bool Ours(string func)
        {
            return func.StartsWith("ThanksgivingNpc2025_", StringComparison.Ordinal)
                || func.StartsWith("Thanksgiving2025", StringComparison.Ordinal)
                || func.StartsWith("TalentShowPart", StringComparison.Ordinal);
        }

        /// <summary>LuaPush.HandleCallback (msg 10789). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || !Ours(func)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            switch (func)
            {
                case "ThanksgivingNpc2025_MainView": Lua(c, OpenCall(c, Cfg)); return true;
                case "ThanksgivingNpc2025_SelectionView": Msg(c, "The Arena (Top Master voting)" + TxtNotOpen); return true;
                case "Thanksgiving2025Colorful_GoAndCollect": Msg(c, "Splendor Pavilion" + TxtNotOpen); return true;
                case "TalentShowPart2_OpenTowerView": Msg(c, "Pinnacle Tower" + TxtNotOpen); return true;
                case "TalentShowPart3_OpenMineSweeperView": Msg(c, "Land of Fortune" + TxtNotOpen); return true;
                case "Thanksgiving2025Hunt_ClientRank": Msg(c, "The Top Master rankings" + TxtNotOpen); return true;
                case "Thanksgiving2025Hunt_ClientOpen":
                case "Thanksgiving2025Hunt_ClientEnter":
                case "Thanksgiving2025Hunt_ClientBook": Msg(c, "The Skyreach Hunt" + TxtNotOpen); return true;
                case "ThanksgivingNpc2025_OpenExchangeStore": Msg(c, "The Skyreach Vault" + TxtNotOpen); return true;
                case "TalentShowPart3_ItemGetView": Msg(c, "Getting Skyreach Stones" + TxtNotOpen); return true;
            }
            Msg(c, "This part of the Top Master Campaign" + TxtNotOpen);
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            if (c == null) return 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>Main Hall (uitype 1 com_rk). Real data: Skyreach Stones from the bag; 0 points; all entries open.</summary>
        public static string OpenCall(Client.GameClient c, Config cfg)
        {
            var sb = new StringBuilder("Fui_TalentShow_Open|{['Code']=200,['Data']={1,{0,0},{");
            sb.Append("{0,").Append(cfg.PointsMax).Append(",0},");                                          // Skyreach Points
            sb.Append('{').Append(SybSkyreachStone).Append(',').Append(Count(c, SkyreachStone)).Append("},"); // Skyreach Stones
            sb.Append("{0,0,0,0,0,0},");                                                                     // no red dots
            sb.Append("{0,0,0,0,0,0}");                                                                      // 0 = official open time
            return sb.Append("}}}").ToString();
        }
    }
}
