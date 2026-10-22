// ============================================================================
//  [BandsTrouble 2026-10-14] Summer 2024 "Hot Music Festival" (client tMsg_Info["24Summer"] = 10426). Shown while the
//  festival BandsTrouble runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui, decrypted):
//    button    dialog\fguibtn\dlgmusicfestivalbtn  DlgMusicFestivalBtn (layout 150089_btn, 'Hot Music Festival')
//              shown by Fgiu_MusicFestival_ShowBtn|{Code=200,Data={Rewardflag=0}}, click -> 10426 MusicBox_OpenMain
//    window    fairygui\window\musicfestivalmain  FGUI_MusicFestivalMain_Open|{Code=200,Data={
//                [1] duration text, [2] points, [3] points max, [4] {Music Coin count} (com_prop_1 = item 3600726),
//                [5..9] red dots (ranking, chest, store, auditorium, cross), [10] 5 instrument states,
//                [11] 4 x {roleId, level, upgradeMark, '?' (story available), '!'}, [12] play animation}}
//              role '?' / '!' / click -> MusicBox_ClickWenHao</N>i (story) ; otherwise MusicBox_OpenUplevPage</N>i
//  Stories (Questinfo, quests.json): 1 Miko 7972-7973 then 7985 (Disappeared Stereos), 2 Maria 7974-7975,
//  3 Conner 7976-7977, 4 Antonio: none in this client's Questinfo (7978/7979 missing) -> no '?'.
//  Real data: Music Coin count from the bag, '?' from the player's quest state. Not on this server (system message):
//  points / ranking, Musical Note Chest, instruments, bandsman upgrades, Auditorium, exchange shop, cross-server party.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10426), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using SE = Qahr.Mobile.FestivalStory.StoryEntry;

namespace Qahr.Mobile.BandsTrouble
{
    public static class BandsTroubleUi
    {
        public const uint LuaMsg = 10426;
        public const string Festival = "BandsTrouble";
        public const int MinLevel = 80;
        public const uint MusicCoin = 3600726;
        /// <summary>Bandsman (window role id 1..4) -> story chain (Questinfo ids, in order). Antonio has none in this client.</summary>
        public static readonly uint[][] Chains = { new uint[] { 7972, 7973, 7985 }, new uint[] { 7974, 7975 }, new uint[] { 7976, 7977 }, new uint[0] };
        public const string NotInTown = "Please return to Twin City before participating in the Hot Music Festival.";// official tMusicBox Sys[3]
        public const string Ended = "The splendid Hot Music Festival has come to an end. Thank you all for your enthusiastic participation. See you next time!";// official 33132 Text122

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastEnter = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10426).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("MusicBox_", StringComparison.Ordinal) || func.StartsWith("SummerNpc2024_", StringComparison.Ordinal) || func.StartsWith("SummerActive2024Cross_", StringComparison.Ordinal);
            if (!known) return false;
            if (Throttled(_last, c.Player.UID, 400)) return true;
            if (!Running) { SE.Msg(c, Ended); return true; }
            switch (func)
            {
                case "MusicBox_OpenMain":
                    if (c.Player.Level < MinLevel) { SE.Msg(c, "Your level is less than " + MinLevel + ". Please level up first!"); return true; }
                    Lua(c, OpenData(c));
                    return true;
                case "MusicBox_ClickWenHao":
                case "MusicBox_ClickGanTan":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > 4) return true;
                        if (Chains[i - 1].Length == 0) { SE.Msg(c, "Antonio`s story isn`t open on this server yet."); return true; }
                        if (Throttled(_lastEnter, c.Player.UID, 3000)) return true;
                        SE.Enter(c, Chains[i - 1], MinLevel, NotInTown, "You have already helped this bandsman. Thank you!");
                        return true;
                    }
                case "MusicBox_OpenUplevPage":
                    SE.Msg(c, "Bandsman upgrades aren`t open on this server yet.");
                    return true;
                case "MusicBox_OpenMusicBox":
                case "MusicBox_ClickActivated":
                    SE.Msg(c, "The Musical Note Chest and the legendary instruments aren`t open on this server yet.");
                    return true;
                case "MusicBox_OpenScoreGet":
                    SE.Msg(c, "The Hot Music Ranking isn`t open on this server yet.");
                    return true;
                case "MusicBox_OpenMusicStand":
                    SE.Msg(c, "The Auditorium isn`t open on this server yet.");
                    return true;
                case "SummerNpc2024_OpenExchangeShop":
                    SE.Msg(c, "The Hot Music Festival store isn`t open on this server yet.");
                    return true;
                case "SummerActive2024Cross_EnterCross":
                    SE.Msg(c, "The cross-server party isn`t open on this server yet.");
                    return true;
            }
            if (func.StartsWith("MusicBox_ToClient", StringComparison.Ordinal))
            {
                SE.Msg(c, func.Contains("GuidBuy") ? "Music Coins are earned from the bandsmen`s stories." : "Help the bandsmen with their troubles: click a bandsman with a '?' to start the story.");
                return true;
            }
            return true;// other 24Summer calls: nothing to do
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The FGUI MusicFestivalMain open call with this player's real data.</summary>
        public static string OpenData(Client.GameClient c)
        {
            var sb = new StringBuilder("FGUI_MusicFestivalMain_Open|{['Code']=200,['Data']={'7.9-7.22',0,100,{");
            sb.Append(Count(c, MusicCoin)).Append("},0,0,0,0,0,{0,0,0,0,0},{");
            for (int i = 0; i < 4; i++)
            {
                if (i > 0) sb.Append(',');
                bool open = Chains[i].Length > 0 && SE.NextOf(c, Chains[i]) != 0;
                sb.Append('{').Append(i + 1).Append(",1,0,").Append(open ? 1 : 0).Append(",0}");
            }
            return sb.Append("},0}}").ToString();
        }
    }
}
