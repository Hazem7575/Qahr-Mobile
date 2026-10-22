// ============================================================================
//  [VillageRequests 2026-10-14] The official 2024 "Global Christmas" entry (client msg 10510 tMsg_Info["Christmas24"],
//  ([简体征服][活动脚本]全球圣诞活动 12.17-1.6) shown while the festival VillageRequests (Doomwoods requests 8247-8260) runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["10510"] (img lua_2024qqsd_rukou, EN "Global Christmas"), pushed by
//              festival.json icon_show / icon_hide (FguiActivityMenu_UpdateSigle Id '10510').
//    click     10510 Chrismas2024Main_OpenMain -> window ChristmasMain (fairygui\window\christmasmain):
//              Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_ChristmasMain_Open'}}
//              Fui_24ChristmasMain_UpdateData|{Code=200,Data={Guide=0,Reds={portal,converter,tree},Open={portal,converter,tree}}}
//              (Open 0 = locked, 1 = open)
//    buttons   btn_1 Doom Portal  -> ChristmasDraw2024_EnterFuben; server answers Fui_24Christmas_EnterPortal (portal effect),
//                                    the client then sends ChristmasDraw2024_EnterFubenSuc -> into Doomwoods (instance 1003831)
//              btn_2 Converter    -> ChristmasNPC2024_TranslateMenu (+ _GiftYulang / _PutDaoju / _ReceiveAward / _CailiaoClick)
//              btn_3 Christmas Tree -> DrawClient_OpenView</N>143198 (+ DrawClient_Draw / _SkipClick / _GetWay / _DrawPreview)
//  Texts: Server_Key tChristmasDraw2024_DelText[SnowMan][MapChk / InInstance / FindKSL] (official English).
//  Doomwoods = the festival's QuestEngine instance "Doomwoods" (quests.json: map 1003831, citizens as instance NPCs, one
//  private copy per player). The QuestEngine enters instances only from NPC teleports / quest actions, so the portal
//  calls its EnterInstance through reflection (no change to QuestEngine.cs). NOT on this server (system message, nothing
//  granted): the Material Converter, the Christmas Tree wishes / draws, Xmas Points and rankings.
//  Config (derived values): Infinity[DB]\VillageRequests\villagerequests.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10510), tagged [VillageRequests 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.VillageRequests
{
    public static class VillageRequestsUi
    {
        public const uint LuaMsg = 10510;

        public sealed class Config
        {
            public string Festival = "VillageRequests";
            public int MinLevel = 80;                                       // official "Requirement: Level 80"
            public string Instance = "Doomwoods";                           // quests.json instance name
            public HashSet<uint> EnterFrom = new HashSet<uint> { 1002, 1036 };   // official MapChk "Twin City (Market)"
            public int PortalSeconds = 30;                                  // derived: EnterFubenSuc must follow EnterFuben
        }
        static Config _cfg; static string _stamp;
        static string CfgFile { get { return Path.Combine(Program.ServerConfig.DbLocation, "VillageRequests", "villagerequests.json"); } }
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
                    try { MyConsole.WriteLine("[VillageRequests] villagerequests.json: " + e.Message, ConsoleColor.Yellow); } catch { }
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
            var i = FL.Str(d, "instance"); if (!string.IsNullOrWhiteSpace(i)) c.Instance = i.Trim();
            var from = FL.Arr(FL.Val(d, "enterFromMaps")); if (from != null) c.EnterFrom = new HashSet<uint>(from.Select(FL.U).Where(m => m != 0));
            var ps = FL.Val(d, "portalSeconds"); if (ps != null) c.PortalSeconds = (int)Math.Max(5, Math.Min(120, FL.U(ps)));
        }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Cfg.Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, DateTime> _portal = new ConcurrentDictionary<uint, DateTime>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        const string TxtMapChk = "Please return to Twin City (Market) before adventure!";
        const string TxtInside = "You are already in Doomwoods!";
        const string TxtArrive = "After passing through the portal, you arrived at Doomwoods. Go ask the person ahead for information.";
        const string TxtOver = "Global Christmas has ended. Merry Christmas and see you next year!";

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10510).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool converter = func.StartsWith("ChristmasNPC2024_", StringComparison.Ordinal);
            bool tree = func.StartsWith("DrawClient_", StringComparison.Ordinal);
            if (func != "Chrismas2024Main_OpenMain" && func != "ChristmasDraw2024_EnterFuben" && func != "ChristmasDraw2024_EnterFubenSuc" && !converter && !tree)
                return false;
            if (func != "ChristmasDraw2024_EnterFubenSuc" && Throttled(c.Player.UID)) return true;
            var cfg = Cfg;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, "Please level up to " + cfg.MinLevel + " first."); return true; }
            if (converter) { Msg(c, "The Material Converter isn`t available on this server yet."); return true; }
            if (tree) { Msg(c, "The Christmas Tree wishes aren`t available on this server yet."); return true; }
            switch (func)
            {
                case "Chrismas2024Main_OpenMain": Open(c); return true;
                case "ChristmasDraw2024_EnterFuben":
                    if (!CanEnter(c, cfg)) return true;
                    _portal[c.Player.UID] = DateTime.Now;
                    Lua(c, "Fui_24Christmas_EnterPortal|{['Code']=200}");
                    return true;
                case "ChristmasDraw2024_EnterFubenSuc":
                    {
                        DateTime at;
                        if (!_portal.TryRemove(c.Player.UID, out at) || (DateTime.Now - at).TotalSeconds > cfg.PortalSeconds) return true;// only right after the portal
                        if (!CanEnter(c, cfg)) return true;
                        if (Enter(c, cfg))
                        {
                            Lua(c, "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_ChristmasMain_Close'}}");
                            Msg(c, TxtArrive);
                        }
                        return true;
                    }
            }
            return true;
        }

        public static string OpenCall() { return "Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_ChristmasMain_Open'}}"; }
        public static string DataCall() { return "Fui_24ChristmasMain_UpdateData|{['Code']=200,['Data']={['Guide']=0,['Reds']={0,0,0},['Open']={1,0,0}}}"; }
        static void Open(Client.GameClient c)
        {
            Lua(c, OpenCall());
            Lua(c, DataCall());
        }

        static bool InDoomwoods(Client.GameClient c, Config cfg)
        {
            var def = Def(cfg);
            return def != null && c.Player.Map == def.Map && c.Player.DynamicID != 0;
        }
        static bool CanEnter(Client.GameClient c, Config cfg)
        {
            var p = c.Player;
            if (InDoomwoods(c, cfg)) { Msg(c, TxtInside); return false; }
            if (!p.Alive || p.DynamicID != 0 || !cfg.EnterFrom.Contains(p.Map)) { Msg(c, TxtMapChk); return false; }
            return true;
        }

        static QuestEngine.InstanceDef Def(Config cfg)
        {
            var t = QuestEngine.QuestEngineCore.Current;
            QuestEngine.InstanceDef d;
            return t != null && t.Instances.TryGetValue(cfg.Instance, out d) ? d : null;
        }

        static readonly MethodInfo EnterInstanceM = typeof(QuestEngine.QuestEngineCore).GetMethod("EnterInstance", BindingFlags.NonPublic | BindingFlags.Static, null,
            new[] { typeof(Client.GameClient), typeof(QuestEngine.InstanceDef), typeof(uint), typeof(ServerSockets.Packet) }, null);

        /// <summary>Into the player's private copy of Doomwoods (QuestEngine instance, not bound to a quest).</summary>
        static bool Enter(Client.GameClient c, Config cfg)
        {
            var def = Def(cfg);
            if (def == null || EnterInstanceM == null) { Msg(c, "Doomwoods isn`t open yet."); return false; }
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    return (bool)EnterInstanceM.Invoke(null, new object[] { c, def, 0u, rec.GetStream() });
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                Msg(c, "Doomwoods isn`t open yet.");
                return false;
            }
        }
    }
}
