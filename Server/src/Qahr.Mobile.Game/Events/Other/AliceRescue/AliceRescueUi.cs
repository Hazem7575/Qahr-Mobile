// ============================================================================
//  [AliceRescue 2026-10-14] the official April Fools 2024 "Wonderland" window (Alice / Mirror World).
//  Client: luaui\dialog\dlgfoolsday24 (ERP576876), Dialog_Info 1475, button DlgAliceFoolsDayMainBtn (EN label "Wonderland"),
//  tMsg_Info["DlgFoolsDay24"] = 10376. Strings luaUiRes[1475] ("1475-3" txt_time "Duration: Apr. 2 - Apr. 15").
//  Shown while the festival AliceRescue runs (festival.json icon_show / icon_hide, FestivalManager / FestivalIcons).
//
//  Official client:
//    click    AprilFoolsDayPage2024_OpenMainPage -> DlgAliceFoolsDayMainUi_OpenViewDatas|{Code=200,Data={PropItems={3 x 'id 0 1,count'},
//             Points,Guide,RedPoints={5 x 0|1},Lock}}  PropItems: 1 Dreamy Star 3347775, 2 Dream Pearl 3600702, 3 Treasure Ticket 3347776
//             (DlgAliceFoolsDay_AddItem 1/2/3). Lock: any value locks the "slay dragon" entry frame (lua checks presence only).
//    other    AprilFoolsDayPage2024_Login / _OpenStagePage / _OpenRankPage, AprilFoolsDayMainNpc_ExchangeShop / _NpcFind</N>32750,
//             AprilFoolsDayKua_EnterOS / _XunLi, CastleBeat2023_EnterMirror, AprilFoolKillDragon_OpenDragon.
//  On this server: the window shows the REAL bag counts of the three official items. The Mirror World exploration
//  (tCastleBeat2023: floors, fettered partners, Mirror Chests), the dragon slaying (Holy Sword / Wonderland Points), the
//  Treasure Ticket shop, the points rewards, the rankings, Memory Garden (CS) and Vault Palace have no server data here:
//  Points 0, no red dots, the dragon entry locked, and a polite system message (official texts where the client has them).
//  Nothing is consumed or granted.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10376), tagged [AliceRescue 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.AliceRescue
{
    public static class AliceRescueUi
    {
        public const uint LuaMsg = 10376;// tMsg_Info["DlgFoolsDay24"]
        public const string Festival = "AliceRescue";
        public const int MinLevel = 80;// official tAprilFoolsDayMainNpc_DelText[32750][Text114] "Requirement: Level 80"
        public const uint DreamyStar = 3347775, DreamPearl = 3600702, TreasureTicket = 3347776;
        public const int MirrorPearls = 5;// official luaUiRes[1475]["rules"] "Every time you enter the Mirror World, 5 Dream Pearls will be consumed."

        // official texts (client Server_Key / luaUiRes)
        public const string EndText = "Evil Jabba has been defeated, and the wonderland is gradually returning to peace. It`s time to say goodbye to this fantastic and exciting adventure! Looking forward to the next wonderful journey with you!";
        public const string LevelText = "Please upgrade to level 80 first.";
        public const string NoPearlText = "Failed to enter the Mirror World for lack of 5 Dream Pearls. Go learn about how to obtain Dream Pearls.";

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10376).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (func == "AprilFoolsDayPage2024_Login") return true;// red-dot query of the button: no red dot on this server
            if (Throttled(c.Player.UID, 400)) return true;
            if (!Running) { Msg(c, EndText); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, LevelText); return true; }
            switch (func)
            {
                case "AprilFoolsDayPage2024_OpenMainPage":
                    LuaUi.LuaPush.Send(c, MainData(c), LuaMsg);
                    return true;
                case "CastleBeat2023_EnterMirror":
                    if (Count(c, DreamPearl) < MirrorPearls) Msg(c, NoPearlText);
                    else Msg(c, "The Mirror World isn`t open on this server yet. Your Dream Pearls were not used.");
                    return true;
                case "AprilFoolKillDragon_OpenDragon":
                    Msg(c, "The Holy Sword can`t strike Jabba on this server yet.");
                    return true;
                case "AprilFoolsDayPage2024_OpenStagePage":
                    Msg(c, "The Wonderland Points rewards aren`t open on this server yet.");
                    return true;
                case "AprilFoolsDayPage2024_OpenRankPage":
                    Msg(c, "The Wonderland Ranking isn`t open on this server yet.");
                    return true;
                case "AprilFoolsDayMainNpc_ExchangeShop":
                    Msg(c, "The Treasure Ticket exchange isn`t open on this server yet.");
                    return true;
                case "AprilFoolsDayKua_EnterOS":
                    Msg(c, "Memory Garden (CS) isn`t open on this server yet.");
                    return true;
                case "AprilFoolsDayKua_XunLi":
                    Msg(c, "Vault Palace isn`t open on this server yet.");
                    return true;
                case "AprilFoolsDayMainNpc_NpcFind":
                    Msg(c, "Tritar isn`t in Twin City on this server yet.");
                    return true;
            }
            return false;
        }

        public static string MainData(Client.GameClient c)
        {
            return "DlgAliceFoolsDayMainUi_OpenViewDatas|{['Code']=200,['Data']={['PropItems']={'" + DreamyStar + " 0 1," + Count(c, DreamyStar)
                + "','" + DreamPearl + " 0 1," + Count(c, DreamPearl) + "','" + TreasureTicket + " 0 1," + Count(c, TreasureTicket)
                + "'},['Points']=0,['Guide']=0,['RedPoints']={0,0,0,0,0},['Lock']=1}}";
        }
    }
}
