// ============================================================================
//  [AnniversaryOtherworld2024 2026-10-14] CO Anniversary 2024 "Anniversary Cake Gala" (Otherworld Adventure),
//  official duration Apr. 23 - May 13, 2024 (luaUiRes_EN[1482].main.ActTime; items expire May 13, 2024).
//  Client tMsg_Info["24Anniversary"] = 10389. Shown while the festival AnniversaryOtherworld2024 runs.
//
//  Official client (luaui\dialog\dlganniversary\dlg24anniversary*, luagui 1482):
//    button   Dlg24AnniversaryBtn (Dialog_Info "1482-btn"); server show call Dlg24Anniversary_ShowBtn|{Code=200,Data={Rewardflag=0|1}}
//             (festival.json icon_show); hide = LuaUIMgr_HideDialog("Dlg24AnniversaryBtn").
//             click -> 10389 Interface2024_OpenMain.
//    main     Dlg24AnniversaryMainUi_MainOpen|{Code=200,Data={Score=,ScoreRed=0|1,ItemInfo='3600703 0 n',HandIndex=0..7,
//             Actlist={7 x {State=0 locked|1 unlocking|2 unlocked|3 red,IsShow=0|1}}}}
//             sites: 1 Gala Gourmet store, 2 Verdant Forest, 3 Bakery, 4 Lounge, 5 Coastal Area, 6 Candy Valley (CS), 7 Sea Ruins.
//    clicks   Interface2024_Click</N>1..7, Interface2024_ClickReward (cake), CultivateMonsterCamp_Rank / _OpenMain / _ViewDispatch /
//             _LeveUp (beast camp), Anniversary2024Npc_ExchangeShop</N>36841, Anniversary2024Npc_EmoneyShop</N>36846,
//             GloablEventsTemple_EnterTempleSuc (Vault Palace), Aniversary2024Bakery_ShopNotEnoughTip, Interface2024_Guide*/GuideSet.
//  Real data: Super Gourd (3600703) bag count. Gala Points: nothing on this server earns them (beast capture / feeding / bakery
//  are official server scripts tAnniversary2024Npc / tCultivateMonsterCamp whose tables are not in the client) = 0.
//  Every action that needs those systems answers with a polite system message; nothing is granted.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10389), one tagged line.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.AnniversaryOtherworld2024
{
    public static class AnniversaryOtherworld2024Ui
    {
        public const uint LuaMsg = 10389;
        public const string Festival = "AnniversaryOtherworld2024";
        public const int MinLevel = 80;// official "Requirement: Level 80" (tAnniversary2024Npc[36841][Text115])
        public const uint SuperGourd = 3600703, CommonSugarBean = 3348129, SuperSugarBean = 3348130;
        // official texts (luacfg Server_Key tAnniversary2024Npc)
        public const string Ended = "The Anniversary Cake Gala has successfully concluded! See you next time!";// [36841][Text122]
        const string NoLevel = "Please come back after reaching level 80.";
        static readonly string[] Sites = { "Gala Gourmet", "Verdant Forest", "Bakery", "Lounge", "Coastal Area", "Candy Valley (CS)", "Sea Ruins" };

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

        /// <summary>Gala Points. No source on this server (beast feeding missing) = 0.</summary>
        public static int Points(Client.GameClient c) { return 0; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10389).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("Interface2024_", StringComparison.Ordinal) || func.StartsWith("CultivateMonsterCamp_", StringComparison.Ordinal)
                      || func.StartsWith("Anniversary2024Npc_", StringComparison.Ordinal) || func.StartsWith("Aniversary2024Bakery_", StringComparison.Ordinal)
                      || func == "GloablEventsTemple_EnterTempleSuc";
            if (!known) return false;
            if (func.StartsWith("Interface2024_Guide", StringComparison.Ordinal)) return true;// guide steps: nothing to do
            if (Throttled(c.Player.UID, 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, NoLevel); return true; }
            switch (func)
            {
                case "Interface2024_OpenMain":
                    Lua(c, "Dlg24AnniversaryMainUi_MainOpen|" + MainData(c));
                    return true;
                case "Interface2024_Click":
                    {
                        int i;
                        if (args.Length == 0 || !int.TryParse(args[0], out i) || i < 1 || i > Sites.Length) return true;
                        Msg(c, Sites[i - 1] + " isn`t open on this server yet.");
                        return true;
                    }
                case "Interface2024_ClickReward":
                    Msg(c, "No Festive Cakes have been baked yet. The Bakery isn`t open on this server yet.");
                    return true;
                case "CultivateMonsterCamp_Rank":
                    Msg(c, "The Anniversary Cake Gala ranking isn`t open on this server yet.");
                    return true;
                case "Anniversary2024Npc_ExchangeShop":
                    Msg(c, "The Gala Gourmet exchange isn`t open on this server yet.");
                    return true;
                case "Anniversary2024Npc_EmoneyShop":
                    Msg(c, "The Anniversary Store isn`t open on this server yet.");
                    return true;
                case "GloablEventsTemple_EnterTempleSuc":
                    Msg(c, "Vault Palace isn`t open on this server yet.");
                    return true;
                case "Aniversary2024Bakery_ShopNotEnoughTip":
                    Msg(c, "You don`t have enough Festive Cakes.");
                    return true;
            }
            if (func.StartsWith("CultivateMonsterCamp_", StringComparison.Ordinal))
            {
                Msg(c, "You haven`t captured the beast yet. Beast capturing isn`t open on this server yet.");
                return true;
            }
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>The Dlg24AnniversaryMainUi data with this player's real data.</summary>
        public static string MainData(Client.GameClient c)
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            sb.Append("['Score']=").Append(Points(c)).Append(",['ScoreRed']=0,");
            sb.Append("['ItemInfo']='").Append(SuperGourd).Append(" 0 ").Append(Count(c, SuperGourd)).Append("',");
            sb.Append("['HandIndex']=0,['Actlist']={");
            // derived: every site open (State 2) so each click gets an answer; no red points (nothing claimable)
            for (int i = 0; i < Sites.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['State']=2,['IsShow']=1}");
            }
            return sb.Append("}}}").ToString();
        }
    }
}
