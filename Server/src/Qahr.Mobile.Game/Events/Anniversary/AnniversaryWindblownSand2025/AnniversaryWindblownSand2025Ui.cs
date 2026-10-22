// ============================================================================
//  [AnniversaryWindblownSand2025 2026-10-14] Official 2025 CO 22nd Anniversary, two client entries:
//
//  A) Wind-blown Sand / Dune (festival AnniversaryWindblownSand2025, Apr. 22 - May 12, 2025, Cn_Res STR_LUAUI_ANNIVERSARY_MAIN_TIME)
//     top bar  fguibtnconfig FGUI_ActivityMenu["10611"] (img lua_2025qqznq_btn5), clickfunc WindBlownSand2025_OpenMainView,
//              msg 10611 tMsg_Info["Anniversary25"].
//     window   h_fuifunction Anniversary25Main_OpenData -> fairygui\window\anniversary25main:
//              Data[1] = {{sybid,count},..}  Luminut 3600780 (sybid 151001000 = its luauirewardshow show item)
//              Data[2] = points (bar /120000)
//              Data[3] = 7 stage states 0 grey / 1 open / 2 running / 3 cleared (Dune .. Obsession Point)
//              Data[4] = red {points, draw, bless}
//     clicks   (10611) WindBlownSand2025_EnterMainMap, Aniversary2025Part4_GridView</N>n, Aniversary2025Part4_OpenViewChk,
//              AnniversaryChip2025_OpenPrayClient, AnniversaryChip2025_EmoneyShop</N>34164.
//              Ranking / Draw / Bless go to OTHER msg ids (DlgValentinesDay StageTemplate_*, 10510 DrawClient_* 143271,
//              DlgBless) and are left to their owners.
//
//  B) Anniversary Invitation / Jubilee (festival AnniversaryJubilee2025, English CO May 13 - June 9, 2025, ERP627502 【英文征服】)
//     top bar  FGUI_ActivityMenu["10631"] (img lua2025znqtitle_btn), clickfunc AnniversaryCarnival_CelebrationAttend,
//              msg 10631 tMsg_Info["25Jubilee"].
//     window   h_fuifunction Fui_JubileeMain_OpenData -> fairygui\window\jubileemain:
//              Data[1] = 4 x {sybid,count}  Wish Butterfly 3352961, Celebration Medal 3352962, Anniversary Point 3353247,
//                                           Anniv Coin 3353040 (none has a luauirewardshow sybid -> icon 0, count is real)
//              Data[2] = 4 activity states 0 not open / 1 open / 2 ended
//              Data[3] = red {sign-in, points, medal exchange}
//              Data[4] = stage 1 Celebration (May 13-26) / 2 Carnival (May 27 - June 9)  (official Cn_Res TIME_1/TIME_2)
//     clicks   AnniversaryCarnival_Goto</N>1..8, _ViewPhase, _OpenShop, _ExChangeShop; Goto 8 = Anniversary Guide
//              -> Fui_JubileeRule_Open (official guide text, client side). OpenRank goes to 10139 (CastleMystery's msg).
//
//  REAL data only: bag counts. No anniversary system (Dune search, West Journey, wishes, rivalry, shops) exists here:
//  polite system message, nothing granted. Stage state "Dune open" (1,0,0,0,0,0,0) and activity states 0 are DERIVED.
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10611 + 10631).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.AnniversaryWindblownSand2025
{
    public static class AnniversaryWindblownSand2025Ui
    {
        public const uint DuneMsg = 10611;           // tMsg_Info["Anniversary25"]
        public const uint JubileeMsg = 10631;        // tMsg_Info["25Jubilee"]
        public const string DuneFestival = "AnniversaryWindblownSand2025";
        public const string JubileeFestival = "AnniversaryJubilee2025";
        public const int MinLevel = 80;              // official "Requirement: Level 80" (both)

        const uint Luminut = 3600780, LuminutSybId = 151001000;
        static readonly uint[] JubileeItems = { 3352961, 3352962, 3353247, 3353040 };

        // official texts (Server_Key tWindBlownSand2025_DelText / tAnniversaryCarnival_DelText)
        const string TxtDuneOver = "22 years of adventures! Let`s continue wielding swords and laughing through the world! See you next year!";
        const string TxtJubileeOver = "Thank you for journeying with us! Though the anniversary celebration has concluded, we wish you continued adventures in the world of CO. See you next year!";
        const string TxtLevel = "Please reach level 80 first.";
        const string TxtNoCity = "This interface can only be opened in Twin City or Market!";
        const string TxtNotOpen = " isn`t open on this server yet.";

        static bool IsRunning(string key) { try { return Festivals.FestivalManager.IsRunning(key); } catch { return false; } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < 300) return true;
            _last[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }

        /// <summary>LuaPush.HandleCallback (msg 10611 Dune, 10631 Jubilee). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (msgId == DuneMsg) return OnDune(c, func, args);
            if (msgId == JubileeMsg) return OnJubilee(c, func, args);
            return false;
        }

        static bool OnDune(Client.GameClient c, string func, string[] args)
        {
            if (!func.StartsWith("WindBlownSand2025_", StringComparison.Ordinal) && !func.StartsWith("Aniversary2025Part4_", StringComparison.Ordinal)
                && !func.StartsWith("AnniversaryChip2025_", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!IsRunning(DuneFestival)) { Msg(c, TxtDuneOver); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "WindBlownSand2025_OpenMainView":
                    if (c.Player.DynamicID != 0 || (c.Player.Map != 1002 && c.Player.Map != 1036)) { Msg(c, TxtNoCity); return true; }// official [Sys][NoCity]
                    LuaUi.LuaPush.Send(c, DuneCall(c), DuneMsg);
                    return true;
                case "WindBlownSand2025_EnterMainMap": Msg(c, "Dune Search" + TxtNotOpen); return true;
                case "Aniversary2025Part4_GridView":
                case "Aniversary2025Part4_OpenViewChk": Msg(c, "The West Journey" + TxtNotOpen); return true;
                case "AnniversaryChip2025_OpenPrayClient":
                case "AnniversaryChip2025_OpenPray": Msg(c, "Anniversary Blessing" + TxtNotOpen); return true;
                case "AnniversaryChip2025_EmoneyShop": Msg(c, "The anniversary store" + TxtNotOpen); return true;
            }
            Msg(c, "This part of the anniversary" + TxtNotOpen);
            return true;
        }

        static bool OnJubilee(Client.GameClient c, string func, string[] args)
        {
            if (!func.StartsWith("AnniversaryCarnival_", StringComparison.Ordinal)) return false;
            if (Throttled(c.Player.UID)) return true;
            if (!IsRunning(JubileeFestival)) { Msg(c, TxtJubileeOver); return true; }
            if (c.Player.Level < MinLevel) { Msg(c, TxtLevel); return true; }
            switch (func)
            {
                case "AnniversaryCarnival_CelebrationAttend":
                    LuaUi.LuaPush.Send(c, JubileeCall(c), JubileeMsg);
                    return true;
                case "AnniversaryCarnival_Goto":
                    {
                        int n; int.TryParse(args != null && args.Length > 0 ? args[0] : "", out n);
                        switch (n)
                        {
                            case 1: Msg(c, "Anniversary Recall" + TxtNotOpen); return true;
                            case 2: Msg(c, "Anniversary Rivalry" + TxtNotOpen); return true;
                            case 3: Msg(c, "Anniversary Wish" + TxtNotOpen); return true;
                            case 4: Msg(c, "Silver Carnival" + TxtNotOpen); return true;
                            case 5: Msg(c, "Anniversary Sign-in" + TxtNotOpen); return true;
                            case 6: Msg(c, "Anniversary Token" + TxtNotOpen); return true;
                            case 7: Msg(c, "Anniversary Promotion" + TxtNotOpen); return true;
                            case 8: LuaUi.LuaPush.Send(c, "Fui_JubileeRule_Open|{['Code']=200,['Data']={" + Stage() + "}}", JubileeMsg); return true;
                        }
                        break;
                    }
                case "AnniversaryCarnival_ViewPhase": Msg(c, "The points rewards" + TxtNotOpen); return true;
                case "AnniversaryCarnival_OpenShop":
                case "AnniversaryCarnival_ExChangeShop": Msg(c, "The anniversary store" + TxtNotOpen); return true;
            }
            Msg(c, "This part of the anniversary" + TxtNotOpen);
            return true;
        }

        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }

        /// <summary>Official stages: Celebration May 13-26, Carnival May 27 - June 9 (outside that window: Celebration).</summary>
        static int Stage()
        {
            var now = DateTime.Now;
            return (now.Month == 5 && now.Day >= 27) || (now.Month == 6 && now.Day <= 9) ? 2 : 1;
        }

        /// <summary>Anniversary25Main_OpenData: Luminut from the bag, no points, only Dune open.</summary>
        public static string DuneCall(Client.GameClient c)
        {
            var sb = new StringBuilder("Anniversary25Main_OpenData|{['Code']=200,['Data']={");
            sb.Append("{{").Append(LuminutSybId).Append(',').Append(c == null ? 0 : Count(c, Luminut)).Append("}},");
            sb.Append("0,");
            sb.Append("{1,0,0,0,0,0,0},");
            sb.Append("{0,0,0}");
            sb.Append("}}");
            return sb.ToString();
        }

        /// <summary>Fui_JubileeMain_OpenData: the four currencies from the bag, nothing open, official stage by date.</summary>
        public static string JubileeCall(Client.GameClient c)
        {
            var sb = new StringBuilder("Fui_JubileeMain_OpenData|{['Code']=200,['Data']={{");
            for (int i = 0; i < JubileeItems.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{0,").Append(c == null ? 0 : Count(c, JubileeItems[i])).Append('}');
            }
            sb.Append("},{0,0,0,0},{0,0,0},").Append(Stage()).Append("}}");
            return sb.ToString();
        }
    }
}
