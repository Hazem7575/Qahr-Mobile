// ============================================================================
//  [QixiQingqiu2023 2026-10-14] Global Qixi 2023 "Jade Hill" (2023-08-22 00:00 - 2023-09-04 23:59),
//  client tMsg_Info["DlgChineseSaintValentinesDay"] = 10272. Shown while the festival QixiQingqiu2023 runs.
//
//  Official client (luaui\dialog\dlgchinesesaintvalentinesday, luagui\1399.dat, luaUiRes_EN[1399]):
//    button   DlgChineseSaintValentinesDayBtn (Dialog_Info "1399_EnterBTN", anisection 2023qqqx_btn4). It reads
//             g_player_info["DlgChineseSaintValentinesDayBtn"].Head.Func[1], so festival.json icon_show uses Dlg_OpenUI
//             with {Head={Func={'QingQiuMainNpc_OpenMain'}},Data={RedPoint=0}}. Click -> 10272 QingQiuMainNpc_OpenMain.
//    main     Dlg_OpenUI|{Code=200,View='DlgChineseSaintValentinesDayMain',Data={Head={Func[1..6],Time,Guide,StItem},
//             Body={Lock,PageRedpoint,TaskRedpoint,Task,Award[1..7]}}}
//             Func[1] Peachland Life, [2] Fox Tie, [3] Fox Practice, [4] guide end, [5] memory click "</S>{Index,Type}",
//             [6] unlock effect end. [2] QiXiPlay5_OpenQixiMain and [3] MassifDraw_DrawView are the official names
//             (qingqiuactivitymatchfunc / g_function); [1] [4] [5] [6] are server-side only, so ours are named after the
//             official script tQingQiuMainNpc.
//  Real data: Clue Bell 3380015 / Marital Stick 3344212 / Marital Fruit 3600624 bag counts (in the messages).
//  Jade Hill, its errands, the 7 memory plots, Fox Tie matching and the Fox Practice draw are not on this server:
//  everything is at the official start state (all locked) and every action answers with a polite system message.
//  Nothing is granted, nothing is taken. DERIVED: the unlock cost shown under each memory (1 Clue Bell).
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10272), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.QixiQingqiu2023
{
    public static class QixiQingqiu2023Ui
    {
        public const uint LuaMsg = 10272;// tMsg_Info["DlgChineseSaintValentinesDay"]
        public const string Festival = "QixiQingqiu2023";
        public const int MinLevel = 80;// ActBtnConfig[9] NeedLevel, Server_Key tQingQiuMainNpc "Requirement: Level 80"
        public const uint ClueBell = 3380015, MaritalStick = 3344212, MaritalFruit = 3600624;
        public const int StoryCost = 1;// DERIVED: Clue Bells per memory (official count is server-side only)
        public const string Time = "Aug. 22 - Sept. 4";// ActBtnConfig[9] ActTime
        public const string Ended = "The Global Qixi event has ended. See you next year!";

        public const string FOpen = "QingQiuMainNpc_OpenMain", FDaily = "QingQiuMainNpc_OpenDaily", FTie = "QiXiPlay5_OpenQixiMain",
            FDraw = "MassifDraw_DrawView", FGuide = "QingQiuMainNpc_GuideEnd", FStory = "QingQiuMainNpc_Story", FEffect = "QingQiuMainNpc_EffectEnd";

        /// <summary>Server_Key STR_ID_tQingQiuMainNpc[Msg][Story][1..7].</summary>
        public static readonly string[] Stories = { "Griffin Invasion", "Escaped Lily", "Fiery Flora", "Strange Willie", "Ambitious Cooer", "Fickle Vulpes", "Revived Belle" };

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10272).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            string extra = "";
            int s = func.IndexOf("</S>", StringComparison.Ordinal);// Func[4]/[5] append "</S>"..TableToStr(t)
            if (s > 0) { extra = func.Substring(s + 4); func = func.Substring(0, s); }
            if (func != FOpen && func != FDaily && func != FTie && func != FDraw && func != FGuide && func != FStory && func != FEffect
                && !func.StartsWith("QingQiuMainNpc_", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID, 300)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, "Please upgrade to level 80 first."); return true; }// tQingQiuMainNpc[31829][Text132]
            switch (func)
            {
                case FOpen:
                    Lua(c, "Dlg_OpenUI|" + MainData());
                    return true;
                case FDaily:
                    Msg(c, "Peachland Life isn`t open on this server yet. You have " + Count(c, ClueBell) + " Clue Bell(s) and " + Count(c, MaritalStick) + " Marital Stick(s).");
                    return true;
                case FTie:
                    Msg(c, "Fox Tie isn`t open on this server yet. You have " + Count(c, MaritalStick) + " Marital Stick(s).");
                    return true;
                case FDraw:
                    Msg(c, "Fox Practice isn`t open on this server yet. You have " + Count(c, MaritalFruit) + " Marital Fruit(s).");
                    return true;
                case FStory:
                    {
                        var m = Regex.Match(extra ?? "", @"Index'?\]?\s*=\s*'?(\d+)");
                        int i;
                        if (!m.Success || !int.TryParse(m.Groups[1].Value, out i) || i < 1 || i > Stories.Length) return true;
                        Msg(c, "The Jade Hill Memory `" + Stories[i - 1] + "` isn`t open on this server yet.");
                        return true;
                    }
            }
            return true;// guide end / effect end / red-point requests: nothing to do
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The DlgChineseSaintValentinesDayMain data: official start state (pages shown, all 7 memories locked).</summary>
        public static string MainData()
        {
            var sb = new StringBuilder("{['Code']=200,['View']='DlgChineseSaintValentinesDayMain',['Data']={['Head']={['Func']={");
            sb.Append(LuaUi.LuaPush.Q(FDaily)).Append(',').Append(LuaUi.LuaPush.Q(FTie)).Append(',').Append(LuaUi.LuaPush.Q(FDraw)).Append(',')
              .Append(LuaUi.LuaPush.Q(FGuide)).Append(',').Append(LuaUi.LuaPush.Q(FStory)).Append(',').Append(LuaUi.LuaPush.Q(FEffect)).Append("},");
            sb.Append("['Time']=").Append(LuaUi.LuaPush.Q(Time)).Append(",['Guide']=0,['StItem']='");
            for (int i = 0; i < Stories.Length; i++) { if (i > 0) sb.Append(','); sb.Append(ClueBell).Append(" 0 ").Append(StoryCost).Append(" 0"); }
            sb.Append("'},['Body']={['Lock']='1 1 1',['PageRedpoint']='0 0 0',['TaskRedpoint']='0 0 0 0 0 0 0',['Task']='0 0 0 0 0 0 0',['Award']={");
            for (int i = 0; i < Stories.Length; i++) { if (i > 0) sb.Append(','); sb.Append("''"); }// per-memory rewards are server-side only
            return sb.Append("}}}}").ToString();
        }
    }
}
