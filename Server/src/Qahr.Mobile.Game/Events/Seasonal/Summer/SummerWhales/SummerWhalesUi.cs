// ============================================================================
//  [SummerWhales 2026-10-14] The official Summer 2022 "Summer Adventure" (Summer Beach Adventure) entry and window, shown
//  while the festival SummerWhales (Data\Festivals\SummerWhales, quests 5113-5115 on map 11829) runs.
//
//  Official client (luaui\dialog\dlgsummeractivities, decrypted):
//    button   DlgSummerBtn (Dialog_Info 1255-btn, EN "Summer Adventure"), pushed by festival.json icon_show:
//             Dlg_OpenUI|{['Code']=200,['View']='DlgSummerBtn',['Data']={['Func']=<func>,['Red']=0}}
//             Its click sends Data.Func RAW: CallServerFution("</F>"..Func) (no ClientManager_Callback, no msgId).
//             The official Func name is not in the client -> we use "SummerActivePage_MainView" (the window's own
//             server calls are SummerActivePage_RankView / SummerActivePage_SynthesisView).
//    window   DlgSummerMain_BindData|{['Code']=200,['Data']={['Lock']=0|1|2,['RedList']={wish,merge},['Guide']=0|1[,['Favor']={cur,max}]}}
//             Lock 0 = Cool Waterfall + Whale Vortex locked ("Complete 3 Cassio`s requests on Summer Beach to unlock."),
//             2 = unlocked. Guide 0 = the "Click to enter Summer Beach" guide (wish / merge buttons hidden).
//             Favor ("Whale Favor Points") = 49 per Whale Wish pray (official DrawInfo); bar max = config favorMax (derived).
//    buttons  (raw calls) SummerNPC2022_EnterMap (Summer Beach), SummerActivePage_RankView</N>1|2, AnniversaryPageAct_KehuDataUpdate
//             (Whale Wish), SummerActivePage_SynthesisView (Ambergris transformation), User_OpenDialog</N>2200 (Cool Waterfall),
//             SummerActive2022Cross_EnterCrossServer (Whale Vortex).
//  Texts: Server_Key tSummerActivePage_DelText[105][n] (official English).
//  [2026-10-15] Whale Wish + Ambergris transformation + the event items: SummerWhalesWish.cs.
//  NOT on this server (system message, nothing granted): rankings, Cool Waterfall, Whale Vortex (cross server).
//  Scallop Beach (quest 5113, official "Feed 5 [mob Scallop][chk_item 3336088,5]"): the Scallop NPC of the festival gives
//  one official Scallop 3336088 per click while 5113 is active and the bag holds fewer than 5 (rate limited).
//  Config (derived values): Data\SummerWhales\summerwhales.json.
//
//  Hooks (tagged [SummerWhales 2026-10-14]):
//    Base\LuaUi\LuaPush.cs HandleCallback  -> if (SummerWhales.SummerWhalesUi.OnRaw(client, data)) return true;
//    Game\MsgNpc\Procesor.cs (before FestivalManager.OnNpc) -> if (SummerWhales.SummerWhalesUi.OnNpc(...)) return;
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.SummerWhales
{
    public static class SummerWhalesUi
    {
        public const string OpenFunc = "SummerActivePage_MainView";

        public sealed class Config
        {
            public string Festival = "SummerWhales";
            public int MinLevel = 80;                                   // official tSummerActivePage_DelText[105][1]
            public uint BeachMap = 11829; public ushort BeachX = 172, BeachY = 153;   // next to Cassio (172,149) - derived
            public HashSet<uint> EnterFrom = new HashSet<uint> { 1002, 11829 };   // official [105][16] "Twin City or Summer Beach"
            public uint[] Requests = { 5111, 5112, 5113, 5114, 5115 };  // Cassio's requests (Questinfo)
            public int UnlockAfter = 3;                                 // official UnlockTip "Complete 3 Cassio`s requests"
            public uint ScallopQuest = 5113, ScallopItem = 3336088; public int ScallopMax = 5;
            public HashSet<uint> ScallopNpcs = new HashSet<uint> { 8857001 };
            public int ScallopCooldownMs = 1500;                        // derived
            public int FavorMax = 9800;                                 // derived: 200 prays x 49 (the Favor bar of DlgSummerMain)
        }
        static Config _cfg; static string _stamp;
        static Dictionary<string, object> _raw = new Dictionary<string, object>();
        /// <summary>The whole summerwhales.json (SummerWhalesWish reads its "wish" / "transform" / "items" parts).</summary>
        public static Dictionary<string, object> RawConfig { get { var c = Cfg; return _raw; } }
        public const string TxtOverPublic = TxtOver;
        static string CfgFile { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "SummerWhales", "summerwhales.json"); } }
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
                    var raw = fi.Exists ? FL.ReadJson(fi.FullName) : null;
                    if (raw != null) Read(c, raw);
                    var fv = raw != null ? FL.Val(raw, "favorMax") : null; if (fv != null) c.FavorMax = (int)Math.Max(1, Math.Min(10000000, FL.U(fv)));
                    _raw = raw ?? new Dictionary<string, object>();
                    _cfg = c; _stamp = stamp;
                }
                catch (Exception e)
                {
                    try { MyConsole.WriteLine("[SummerWhales] summerwhales.json: " + e.Message, ConsoleColor.Yellow); } catch { }
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
            var e = FL.Obj(FL.Val(d, "enter"));
            if (e != null && FL.U(FL.Val(e, "map")) != 0) { c.BeachMap = FL.U(FL.Val(e, "map")); c.BeachX = (ushort)FL.U(FL.Val(e, "x")); c.BeachY = (ushort)FL.U(FL.Val(e, "y")); }
            var from = FL.Arr(FL.Val(d, "enterFromMaps")); if (from != null) c.EnterFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
            var sn = FL.Arr(FL.Val(d, "scallopNpcs")); if (sn != null) c.ScallopNpcs = new HashSet<uint>(sn.Select(FL.U).Where(m => m != 0));
            var cd = FL.Val(d, "scallopCooldownMs"); if (cd != null) c.ScallopCooldownMs = (int)Math.Max(300, Math.Min(60000, FL.U(cd)));
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastScallop = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call); }

        // official texts (Server_Key tSummerActivePage_DelText[105][n])
        const string TxtLevel = "Please level up to 80 first.";
        const string TxtOver = "Summer Beach has finally got its peace back. It`s better not to bother the whales.";
        const string TxtWaterfall = "Cool Waterfall is not available. Please come back later.";
        const string TxtVortex = "Whale Vortex is not available. Please come back later.";
        const string TxtWhere = "Please go to Twin City or Summer Beach to start your Summer Adventure!";

        /// <summary>The raw calls of the Summer window ("</F>Func[</N>arg...]"). True = answered (LuaPush.HandleCallback).</summary>
        public static bool OnRaw(Client.GameClient c, ActionQuery data)
        {
            if (c == null || c.Player == null || data == null || data.Strings == null || data.Strings.Length == 0) return false;
            uint msgId; string mf; string[] ma;
            if (LuaUi.LuaPush.TryParse(data, out msgId, out mf, out ma))// [SummerWhales 2026-10-15] Whale Wish window (10250 AnniversaryPageAct_*, 10139 DrawCongratulation)
            {
                if ((msgId == SummerWhalesWish.MsgSummer23 && Running) || msgId == SummerWhalesWish.MsgValentines)
                    if (SummerWhalesWish.OnMsg(c, msgId, mf, ma)) return true;
                // the shared draw window asks 10139 for the "what I won" page after a Summer2023 Anniversary Wish too
                if (msgId == SummerWhalesWish.MsgValentines && mf == "Lover2025Kua_DrawCongratulation") return Summer2023.Summer2023Cruise.OnCongratulation(c);
                return false;
            }
            string s = data.Strings[0];
            if (s == null || !s.StartsWith("</F>", StringComparison.Ordinal) || s.StartsWith(LuaUi.LuaPush.CallbackPrefix, StringComparison.Ordinal)) return false;
            var parts = s.Substring(4).Split(new[] { "</N>" }, StringSplitOptions.None);
            return OnFunc(c, parts[0], parts.Skip(1).ToArray());
        }

        public static bool OnFunc(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            switch (func)
            {
                case OpenFunc:
                case "SummerNPC2022_EnterMap":
                case "SummerActivePage_RankView":
                case "AnniversaryPageAct_KehuDataUpdate":
                case "SummerActivePage_SynthesisView":
                case "SummerActivePage_Synthesis":
                case "SummerActive2022Cross_EnterCrossServer":
                    break;
                case "User_OpenDialog":
                    if (args.Length == 1 && args[0] == "2200") break;// Cool Waterfall button of this window only
                    return false;
                default:
                    return false;
            }
            bool running = Running;
            if (func == "AnniversaryPageAct_KehuDataUpdate" && !running) return false;// shared name: only ours while Summer runs
            if (Throttled(_last, c.Player.UID, func == "SummerActivePage_Synthesis" ? 1500 : 400)) return true;
            var cfg = Cfg;
            if (!running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case OpenFunc: Open(c, cfg); return true;
                case "SummerNPC2022_EnterMap": Enter(c, cfg); return true;
                case "SummerActivePage_RankView": Msg(c, "The Summer Adventure rankings aren`t available on this server yet."); return true;
                case "AnniversaryPageAct_KehuDataUpdate": SummerWhalesWish.OpenWish(c); return true;
                case "SummerActivePage_SynthesisView": SummerWhalesWish.OpenMerge(c); return true;
                case "SummerActivePage_Synthesis":
                    {
                        int page;
                        if (args.Length != 1 || !int.TryParse(args[0], out page)) return true;
                        string err;
                        using (var rec = new ServerSockets.RecycledPacket()) err = SummerWhalesWish.Merge(c, rec.GetStream(), page);
                        if (err != null && err != "?") Msg(c, err);
                        return true;
                    }
                case "User_OpenDialog": Msg(c, TxtWaterfall); return true;
                case "SummerActive2022Cross_EnterCrossServer": Msg(c, TxtVortex); return true;
            }
            return true;
        }

        static bool Finished(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; } }
        static bool Taken(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; } }
        public static int RequestsDone(Client.GameClient c, Config cfg) { return cfg.Requests.Count(q => Finished(c, q)); }

        public static string MainData(Client.GameClient c, Config cfg)
        {
            int lockState = RequestsDone(c, cfg) >= cfg.UnlockAfter ? 2 : 0;
            int guide = cfg.Requests.Any(q => Taken(c, q)) ? 1 : 0;
            long favor = SummerWhalesWish.Store.Get(c.Player.UID, "favor");
            int wishRed = 0, mergeRed = 0;
            try
            {
                var wc = SummerWhalesWish.Cfg; long draws = SummerWhalesWish.Store.Get(c.Player.UID, "draws"), got = SummerWhalesWish.Store.Get(c.Player.UID, "stages");
                for (int i = 0; i < wc.StageAt.Length; i++) if ((got & SummerWhalesWish.Bit(i)) == 0 && draws >= wc.StageAt[i]) wishRed = 1;
                mergeRed = FestGive.Count(c, SummerWhalesWish.Ambergris) >= wc.AccessoryCost ? 1 : 0;
            }
            catch { }
            return "DlgSummerMain_BindData|{['Code']=200,['Data']={['Lock']=" + lockState + ",['RedList']={" + wishRed + "," + mergeRed + "},['Guide']=" + guide
                + ",['Favor']={" + Math.Min(favor, cfg.FavorMax) + "," + cfg.FavorMax + "}}}";
        }
        static void Open(Client.GameClient c, Config cfg) { Lua(c, MainData(c, cfg)); }

        /// <summary>SummerNPC2022_EnterMap: to Summer Beach (next to Cassio), only from Twin City / Summer Beach.</summary>
        static void Enter(Client.GameClient c, Config cfg)
        {
            var p = c.Player;
            if (!p.Alive || p.DynamicID != 0 || !cfg.EnterFrom.Contains(p.Map)) { Msg(c, TxtWhere); return; }
            if (p.Map == cfg.BeachMap) return;
            if (Pool.ServerMaps[cfg.BeachMap] == null) { Msg(c, "Summer Beach isn`t open yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, cfg.BeachX, cfg.BeachY, cfg.BeachMap, 0); else c.Teleport(cfg.BeachX, cfg.BeachY, cfg.BeachMap, 0);
        }

        // ------------------------------------------------------------------ Scallop NPC (quest 5113)
        /// <summary>Procesor hook (the NPC is in view). True = answered here.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            try
            {
                if (c == null || c.Player == null) return false;
                var cfg = Cfg;
                if (!cfg.ScallopNpcs.Contains(npcid) || option != 0 || !Running) return false;
                if (!c.Player.QuestGUI.IsActiveQuest(cfg.ScallopQuest)) return false;// greeting (festival)
                if (Throttled(_lastScallop, c.Player.UID, cfg.ScallopCooldownMs)) return true;
                int have = c.Inventory.GetCountItem(cfg.ScallopItem);
                string text;
                if (have >= cfg.ScallopMax) text = "You already have " + cfg.ScallopMax + " Scallops. Go and feed Whale King Cassio!";
                else if (!c.Inventory.HaveSpace(1)) text = "Please prepare 1 space(s) in your inventory first.";
                else if (c.Inventory.Add(stream, cfg.ScallopItem, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true))
                    text = "You picked up a Scallop (" + (have + 1) + "/" + cfg.ScallopMax + ").";
                else text = "Please prepare 1 space(s) in your inventory first.";
                var d = new Game.MsgNpc.Dialog(c, stream);
                d.AddText(text).AddOption("OK.", 255).AddAvatar(10).FinalizeDialog();
                return true;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return false;
            }
        }
    }
}
