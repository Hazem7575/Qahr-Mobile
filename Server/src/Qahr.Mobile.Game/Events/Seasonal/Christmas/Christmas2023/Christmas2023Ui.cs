// ============================================================================
//  [Christmas2023 2026-10-14] Global Christmas 2023 "Time Exploration" (ERP566869, Dec. 19, 2023 - Jan. 8, 2024),
//  client tMsg_Info["Dlg23Christmas"] = 10326. Shown while the festival Christmas2023 runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui\dialog\dlg23christmas, luagui\1447.dat, luaUiRes_EN[1447]):
//    button   Dlg23ChristmasBtn (Dialog_Info "1447-btn", anisection 2023qqsd_btn8); shown by the server with
//             StrToTable|LuaUIMgr_ShowDialog("Dlg23ChristmasBtn"); click -> 10326 Christmas2023View_OpenMainView
//             (Dlg23ChristmasBtn_Enter / g_function Dlg23ChristmasBtn_EnterMain).
//    main     Dlg23ChristmasMain_BindData|{Code=200,Data={ItemNum={'3600669 0 n','3600670 0 n'},Point=,RewardGet=0|1,
//             Red={..},PathInfo={'lock,progress,box' x3}}}  (1 Past, 2 Present, 3 Future; lock 0/1; box -1 none, 0 claimable, 1 claimed)
//             Lev={level,exp,max} is left out on purpose: the Time Level table is server-side only (not in the client).
//    points   Dlg23ChristmasPoint_BindData|{Code=200,Data={ClaimState={11 x -1|0|1}}} (thresholds = luaUiRes_EN[1447].Point,
//             rewards = Dialog_Info["1447-point-config"].ItemPoint)
//    rank     Dlg23ChristmasRank_BindData|{Code=200,Data={}} (static reward preview of Dialog_Info["1447-rank-config"])
//  Real data: Time Hourglass (3600669) / Time Chain (3600670) bag counts. Time Points: nothing on this server earns them
//  (the Time Gate adventures are not here), so every player has 0, no space is explored, no box / stage reward is
//  claimable. Every action that would need the missing adventures answers with a polite system message; nothing is granted.
//  Quest 7607 "Repairing Snowman" (Questinfo TypeId 5, prize 20 Magic Snowballs 3600381) needs the Xmas Dungeon, whose map
//  is unknown (not in FestivalMaps\maps.txt): listed in festival.json _quests, not playable.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10326), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Christmas2023
{
    public static class Christmas2023Ui
    {
        public const uint LuaMsg = 10326;
        public const string Festival = "Christmas2023";
        public const int MinLevel = 80;// official ActBtnConfig[15] NeedLevel
        public const uint TimeHourglass = 3600669, TimeChain = 3600670;
        /// <summary>luaUiRes_EN[1447].Point[1..10] thresholds (stage 11 = every 5000 more after 88888).</summary>
        public static readonly int[] StagePoints = { 500, 1500, 3000, 5000, 10000, 15000, 20000, 30000, 50000, 88888, 93888 };
        public const string Ended = "The Global Christmas event has ended. See you next time!";

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

        /// <summary>Time Points of a player. No source on this server yet (Time Gate adventures missing) = 0.</summary>
        public static int Points(Client.GameClient c) { return 0; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10326).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("Christmas2023View_", StringComparison.Ordinal) || func.StartsWith("ChrismasSnowman_", StringComparison.Ordinal)
                      || func.StartsWith("ChristmasPast_", StringComparison.Ordinal);
            if (!known) return false;
            if (Throttled(c.Player.UID, 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, "Your level is less than " + MinLevel + ". Please level up first!"); return true; }
            switch (func)
            {
                case "Christmas2023View_OpenMainView":
                case "Christmas2023View_ButtonView":
                    Lua(c, "Dlg23ChristmasMain_BindData|" + MainData(c));
                    return true;
                case "Christmas2023View_OpenStageView":
                    Lua(c, "Dlg23ChristmasPoint_BindData|" + PointData(c));
                    return true;
                case "Christmas2023View_OpenScoreView":
                    Lua(c, "Dlg23ChristmasRank_BindData|{['Code']=200,['Data']={['Page']=1}}");
                    return true;
                case "Christmas2023View_OpenRankView":
                    Msg(c, "The Time Point Ranking isn`t open on this server yet.");
                    return true;
                case "Christmas2023View_StageReward":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > StagePoints.Length) return true;
                        if (Points(c) < StagePoints[i - 1]) Msg(c, "You don`t have enough Time Points to claim this reward yet.");
                        return true;// no Time Points source -> nothing is ever claimable
                    }
                case "Christmas2023View_ViewEnter":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > 3) return true;
                        if (i == 1) Msg(c, "Please come to explore the Space of Past after the exploration progress of the Space of Future reaches 30%.");// luaUiRes_EN[1447].Lock1
                        else if (i == 2) Msg(c, "Please come to explore the Space of Present after the exploration progress of both Space of Future and Space of Past reaches 100%.");// Lock2
                        else Msg(c, "The Time Gate adventures aren`t open on this server yet.");
                        return true;
                    }
                case "Christmas2023View_BoxReward":
                case "Christmas2023View_TimeReward":
                    Msg(c, "You haven`t reached this reward yet. Complete Time Exploration first.");
                    return true;
                case "Christmas2023View_OpenExchangeStore":
                    Msg(c, "The Christmas exchange store isn`t open on this server yet.");
                    return true;
                case "ChrismasSnowman_EnterCross":
                    Msg(c, "The cross-server Christmas event isn`t open on this server yet.");
                    return true;
                case "Christmas2023View_ItemGuide":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > 4) return true;
                        string[] how = { "Chatting with Colonel Louis", "Visiting Vault Palace", "Buying from CP Mall", "Exchanging Animae" };// luaUiRes_EN[1447].Access
                        Msg(c, how[i - 1] + " isn`t available for Time Hourglasses on this server yet.");
                        return true;
                    }
            }
            return true;// guide calls (ChristmasPast_ThingOne, ChrismasSnowman_DoPhotoEvent, GuideEnd): nothing to do
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The Dlg23ChristmasMain data with this player's real data.</summary>
        public static string MainData(Client.GameClient c)
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['ItemNum']={'").Append(TimeHourglass).Append(" 0 ").Append(Count(c, TimeHourglass)).Append("','")
              .Append(TimeChain).Append(" 0 ").Append(Count(c, TimeChain)).Append("'},");
            sb.Append("['Point']=").Append(Points(c)).Append(',');
            sb.Append("['RewardGet']=0,['Red']={0,0,0,0,0},");// ef_red_1..5 (1447.dat): 3 gates, cross, points - nothing to claim
            // official start state: Future open, Past and Present locked (luaUiRes_EN[1447].Tip1), no progress, no box
            sb.Append("['PathInfo']={'0,0,-1','0,0,-1','1,0,-1'}");
            return sb.Append("}}").ToString();
        }

        public static string PointData(Client.GameClient c)
        {
            int p = Points(c);
            var sb = new StringBuilder("{['Code']=200,['Data']={['ClaimState']={");
            for (int i = 0; i < StagePoints.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(p >= StagePoints[i] ? 1 : 0);
            }
            return sb.Append("}}}").ToString();
        }
    }
}
