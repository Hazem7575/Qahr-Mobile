// ============================================================================
//  [ArenaSports 2026-10-14] The official Global Mid-Autumn 2025 "Golden Autumn Sports Meet" entry (Athlete Showdown,
//  client msg 10753 tMsg_Info["SportMeet"]). Shown while the festival ArenaSports runs.
//
//  Official client (luaui, decrypted):
//    top bar   fguibtnconfig FGUI_ActivityMenu["150337"] (EN label "Global Workout"), clickfunc
//              AutumnView2025_OpenMainView</N>0 on msg 10753. Pushed by festival.json icon_show / icon_hide.
//    window    interfacecfg 10753001 -> Fui_SportMeet_OpenMainByData, sent through
//              InfiniteReceiverData|{Code=200,Step=1,InterfaceID=10753001,Data={...}} (the proxy needs InterfaceID).
//              sportmeetmediator UpdateMainUI: Data.Flag (0 = first open), Data.Score {cur,max} (Athlete Points; max
//              200,000 = the permanent Neon Focus halo, Server_Key tMidAutumn2025Main [Sys][Role][Permanent]),
//              Data.UserPath (event 1..7, events unlock in order), Data.XP {sybid, Energy} (luauirewardshow 261001000 =
//              item 3354442 "Energy"), Data.Red {7 events, egg, points}.
//    clicks    (all msg 10753) AutumnView2025_OpenRichView</N>0</N>n (event n board), _OpenEggView</N>0 (Egg Smash, 20
//              Athlete Medals 3600807 per egg), _OtherWay</N>1|3|4, MidAutumn2025Main_JoinFinalFromView (Legend Arenas),
//              _EggDraw / _EggDrawSuc / _RewardEggCollect / _SkipClick / _EggRewardPre / _RollDice / _RollDiceSuc /
//              _PlotSkipClick.
//  Real data sent: Athlete Points 0, the first event (Shooting) open, Energy = bag count of 3354442, no red dots.
//  Official texts: Server_Key tAutumnView2025 / tMidAutumn2025Main (NoItem, FutureSport, NoFinal, NoLevel, Text122).
//
//  NOT on this server (system message, nothing invented): the six dice-board events (no board / episode / reward
//  tables in the client; the MainArena map 13060 is not in any GameMap.dat), Legend Arenas, Egg Smash (no egg odds),
//  rankings and the Energy shop.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10753), tagged.
// ============================================================================
using System;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.ArenaSports
{
    public static class ArenaSportsUi
    {
        public const uint LuaMsg = 10753;            // tMsg_Info["SportMeet"]
        public const uint InterfaceMain = 10753001;  // Fui_SportMeet_OpenMainByData
        public const string Festival = "ArenaSports";
        public const int MinLevel = 80;              // tMidAutumn2025Main [34576][Text114] "Requirement: Level 80"
        public const uint AthleteMedal = 3600807, Energy = 3354442;
        public const uint SybEnergy = 261001000;     // luauirewardshow -> 3354442
        public const int ScoreMax = 200000;          // permanent Neon Focus at 200,000 Athlete Points
        public const int EggCost = 20;               // STR_FUI_LUA_SPORTMEET_ZADAN_TXT_1 "Costs 20 items per try"

        // official texts (luacfg Server_Key)
        const string TxtOver = "The Athlete Showdown has concluded successfully. I believe your heroic and graceful presence on the field must have left an unforgettable impression on countless people!";
        const string TxtLevel = "Please upgrade to level 80 first.";
        const string TxtNoItem = "You don`t have enough Athlete Medals. You can get them by participating in events or purchasing Medal Supply Packs.";
        const string TxtFuture = "This event is not yet unlocked. You are currently in [Shooting Match].";
        const string TxtNoFinal = "Pass all six events—Shooting, Swordsmanship, Riding, Brawl, Gymnastics, Shot Put—to unlock legend arenas!";
        static readonly string[] Sports = { "", "Shooting Match", "Swordsmanship Match", "Riding Match", "Brawl Match", "Gymnastics Match", "Shot Put Match", "Legend Arena" };

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 400) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static int Count(Client.GameClient c, uint id) { try { return Math.Max(0, c.Inventory.GetCountItem(id)); } catch { return 0; } }

        /// <summary>LuaPush.HandleCallback (msg 10753). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || msgId != LuaMsg) return false;
            if (!func.StartsWith("AutumnView2025_", StringComparison.Ordinal) && !func.StartsWith("MidAutumn2025Main_", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "AutumnView2025_OpenMainView":
                    LuaUi.LuaPush.Send(c, OpenData(Count(c, Energy)), LuaMsg);
                    return true;
                case "AutumnView2025_OpenRichView":
                    {
                        int n = 0;
                        if (args.Length > 1) int.TryParse(args[1], out n);
                        if (n == 7) Msg(c, TxtNoFinal);
                        else if (n >= 2 && n <= 6) Msg(c, TxtFuture);
                        else if (n == 1) Msg(c, "The " + Sports[1] + " isn`t open on this server yet.");
                        return true;
                    }
                case "MidAutumn2025Main_JoinFinalFromView":
                    Msg(c, TxtNoFinal);
                    return true;
                case "AutumnView2025_OpenEggView":
                case "AutumnView2025_EggDraw":
                case "AutumnView2025_EggRewardPre":
                case "AutumnView2025_RewardEggCollect":
                    Msg(c, Count(c, AthleteMedal) < EggCost ? TxtNoItem : "Egg Smash isn`t open on this server yet.");
                    return true;
                case "AutumnView2025_OtherWay":
                    {
                        string a = args.Length > 0 ? args[0].Trim() : "";
                        if (a == "4") Msg(c, TxtNoItem);
                        else if (a == "3") Msg(c, "Energy isn`t available on this server yet.");
                        else Msg(c, "The Athlete Points rewards and rankings aren`t open on this server yet.");
                        return true;
                    }
            }
            Msg(c, "This part of the Athlete Showdown isn`t open on this server yet.");// dice board, skips, effects
            return true;
        }

        /// <summary>Main window (InterfaceID 10753001). Real data: 0 Athlete Points, first event open, Energy from the bag.</summary>
        public static string OpenData(int energy)
        {
            return "InfiniteReceiverData|{['Code']=200,['Step']=1,['InterfaceID']=" + InterfaceMain
                + ",['Data']={['Flag']=0,['Score']={0," + ScoreMax + "},['UserPath']=1,['XP']={" + SybEnergy + "," + Math.Max(0, energy)
                + "},['Red']={0,0,0,0,0,0,0,0,0}}}";
        }
    }
}
