// ============================================================================
//  [SportsMeet2024 2026-10-14] "CO Sports Games" 2024 (ERP593055, Aug. 6 - Aug. 19, 2024; Cn_Res STR_FUI_SPORTS_TIME,
//  tCoSportNpc2024[33251][Text114]). Client tMsg_Info["FuiSports"] = 10453. Shown while the festival SportsMeet2024 runs.
//
//  Official client (luaui\fairygui\window\fuisportsmain, pointsrank, fguibtnconfig):
//    top bar  FGUI_ActivityMenu['150124'] (img luahdshouna_btn5), clickfunc AugustSportRank_OpenMainDiolog on msg 10453.
//    main     Fgui_CommandKey|{Code=200,Data={CommandKey='Cmd_FuiSportsMain_Open',Point={cur,max},Money={'id 0 n','id 0 n'},
//             Effect=0|1,ItemInfo={4 x 'id 0 n'}}} (FuiSportsMainProxy:InitView).
//    stage    PointsRank window: Fgui_CommandKey|{Code=200,MsgID=10453,Page=1,NoClaim={1..13},CurScore=n,
//             Data={CommandKey='Cmd_PointsRank_Open'}} (pointsrankdata CONST_POINTS_AWARD_DATA[10453], thresholds 500..38000).
//    clicks   CoSportShoot_GoChallenge</N>1..4, AugustSportRank_OpenJiemian</N>1|2, SportsMeetKua2024_EnterSportsMeetKua2024,
//             CoSportNpc2024_OpenBattlePass, CoSportNpc2024_OpenExchangeShop/OpenEmoneyShop</N>33251, AugustSportRank_OpenRankMenumain,
//             AugustSportRank_RuleOpen</N>1, AugustSportRank_OpenBuy</N>1|2, COSportClimbingDraw_*, AugustSportRank_OpenPaiChengxu,
//             AugustSportRank_RankMenuGetTip, CoSportShoot_Trans.
//  Real data: bag counts of Entry Ticket 3349636 / Medal of Honor 3349640 (Money) and the 4 Athletics buffs Precision Glasses
//  3349676 / Pocket Watch 3349677 / Climbing Claw 3349678 / Underwater Accelerator 3349679 (ItemInfo). Sports Points come only
//  from the Honor Lottery (server script, odds not in the client) = 0 for everyone, so no stage reward is claimable.
//  The 4 competitions, Clan War / ranking, cross-server Sports Park, battle pass, stores and lottery are official server scripts
//  (tCoSportShoot / tAugustSportRank / tSportsMeetKua2024 / tCOSportClimbingDraw / tCoSportNpc2024) and their maps are not in
//  FestivalMaps\maps.txt: polite system message, nothing granted.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10453), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.SportsMeet2024
{
    public static class SportsMeet2024Ui
    {
        public const uint LuaMsg = 10453;
        public const string Festival = "SportsMeet2024";
        public const int MinLevel = 80;// official tCoSportNpc2024[33251][Text115] "Requirement: Level 80"
        public const uint EntryTicket = 3349636, MedalOfHonor = 3349640;
        // derived order: the Athletics Buff Box order (Precision Glasses/Pocket Watch/Climbing Claw/Underwater Accelerator)
        public static readonly uint[] Buffs = { 3349676, 3349677, 3349678, 3349679 };
        /// <summary>pointsrankdata CONST_POINTS_AWARD_DATA[10453] thresholds 1..11 (12/13 = repeating bonuses).</summary>
        public static readonly int[] StagePoints = { 500, 1200, 2000, 4500, 6600, 10000, 15000, 20000, 25000, 30000, 38000 };
        // official texts (luacfg Server_Key)
        public const string Ended = "The sports games have come to an end. Looking forward to our next gathering!";// tCoSportNpc2024[33251][Text121]
        const string NoLevel = "Please upgrade to level 80 before participating in the Sports Games.";// tSportsMeetKua2024[Sys][NoLevel]
        static readonly string[] Events = { "Horse Racing", "Precision Shooting", "Synchronized Swimming", "Sky Climbing" };// STR_FUI_SPORTS_BTN_1..4

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

        /// <summary>Sports Points. Only the Honor Lottery gives them (not on this server) = 0.</summary>
        public static int Points(Client.GameClient c) { return 0; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10453).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("AugustSportRank_", StringComparison.Ordinal) || func.StartsWith("CoSportShoot_", StringComparison.Ordinal)
                      || func.StartsWith("CoSportNpc2024_", StringComparison.Ordinal) || func.StartsWith("COSportClimbingDraw_", StringComparison.Ordinal)
                      || func.StartsWith("SportsMeetKua2024_", StringComparison.Ordinal);
            if (!known) return false;
            if (Throttled(c.Player.UID, 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, NoLevel); return true; }
            int n = 0;
            if (args.Length > 0) int.TryParse(args[0], out n);
            switch (func)
            {
                case "AugustSportRank_OpenMainDiolog":
                    Lua(c, MainData(c));
                    return true;
                case "AugustSportRank_OpenRankMenumain":
                    Lua(c, StageData(c));
                    return true;
                case "AugustSportRank_RankMenuGetTip":
                    Msg(c, "You don`t have enough Sports Points to claim this reward yet.");
                    return true;
                case "AugustSportRank_OpenPaiChengxu":
                    Msg(c, "The Local Sports Games Ranking isn`t open on this server yet.");
                    return true;
                case "CoSportShoot_GoChallenge":
                    // client map: 1 Precision Shooting, 2 Synchronized Swimming, 3 Horse Racing, 4 Sky Climbing
                    {
                        string ev = n == 1 ? Events[1] : n == 2 ? Events[2] : n == 3 ? Events[0] : n == 4 ? Events[3] : null;
                        if (ev != null) Msg(c, ev + " isn`t open on this server yet.");
                        return true;
                    }
                case "AugustSportRank_OpenJiemian":
                    Msg(c, (n == 1 ? "The Clan War" : "The Individual Ranking Contest") + " isn`t open for the Sports Games on this server yet.");
                    return true;
                case "SportsMeetKua2024_EnterSportsMeetKua2024":
                    Msg(c, "The Sports Games have not started yet, so the Cross-server Entrance is temporarily closed.");// tSportsMeetKua2024[Sys][NoTime]
                    return true;
                case "CoSportNpc2024_OpenBattlePass":
                    Msg(c, "The Sports Games battle pass isn`t open on this server yet.");
                    return true;
                case "CoSportNpc2024_OpenExchangeShop":
                    Msg(c, "The Competition Badge Exchange Store isn`t open on this server yet.");
                    return true;
                case "CoSportNpc2024_OpenEmoneyShop":
                    Msg(c, "The Sports Games CP Mall page isn`t open on this server yet.");
                    return true;
                case "AugustSportRank_OpenBuy":
                    Msg(c, n == 2 ? "Medals of Honor come from the Sports Games competitions, which aren`t open on this server yet."
                                  : "Please acquire Entry Tickets through daily sign-in, sports day sign-in, lottery, and CP Mall.");// tCoSportNpc2024[33251][Text214]
                    return true;
                case "AugustSportRank_RuleOpen":
                    Msg(c, "The sports games include 4 events: Precision Shooting, Synchronized Swimming, Horse Racing, and Sky Climbing. Participation in each event requires 1 Entry Ticket.");// [33251][Text213/214]
                    return true;
            }
            if (func.StartsWith("COSportClimbingDraw_", StringComparison.Ordinal))
            {
                Msg(c, "The Honor Lottery isn`t open on this server yet.");
                return true;
            }
            return true;// CoSportShoot_Trans and other in-match calls: nothing to do outside the matches
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint t = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) t += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return t;
        }

        static int NextStage(int p)
        {
            foreach (var s in StagePoints) if (p < s) return s;
            return StagePoints[StagePoints.Length - 1];
        }

        /// <summary>FuiSportsMain open with this player's real data.</summary>
        public static string MainData(Client.GameClient c)
        {
            int p = Points(c);
            var sb = new StringBuilder("Fgui_CommandKey|{['Code']=200,['Data']={['CommandKey']='Cmd_FuiSportsMain_Open',");
            sb.Append("['Point']={").Append(p).Append(',').Append(NextStage(p)).Append("},");
            sb.Append("['Money']={'").Append(EntryTicket).Append(" 0 ").Append(Count(c, EntryTicket)).Append("','")
              .Append(MedalOfHonor).Append(" 0 ").Append(Count(c, MedalOfHonor)).Append("'},");
            sb.Append("['Effect']=0,['ItemInfo']={");
            for (int i = 0; i < Buffs.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('\'').Append(Buffs[i]).Append(" 0 ").Append(Count(c, Buffs[i])).Append('\'');
            }
            return sb.Append("}}}").ToString();
        }

        /// <summary>PointsRank (stage rewards) for msg 10453: nothing reached, nothing claimed.</summary>
        public static string StageData(Client.GameClient c)
        {
            int p = Points(c);
            var sb = new StringBuilder("Fgui_CommandKey|{['Code']=200,['MsgID']=10453,['Page']=1,['LastDialog']=0,['CurScore']=");
            sb.Append(p).Append(",['NoClaim']={");
            for (int i = 1; i <= 13; i++) { if (i > 1) sb.Append(','); sb.Append(i); }
            return sb.Append("},['Data']={['CommandKey']='Cmd_PointsRank_Open'}}").ToString();
        }
    }
}
