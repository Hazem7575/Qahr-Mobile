// ============================================================================
//  [LuaUi] server -> client Lua UI calls, the same way the official server drives the client's Lua windows.
//
//  Server -> client: MsgAction 2099 (protobuf ActionQuery), Type 258 (ActionType.AnimaCrest), Strings = { "Func|arg" }
//                    (same packet as MY System\Yuanshen\MsgYuanshen.cs OpenGUI). A reply to a client callback carries
//                    dwParam = the callback's msgId (as the LakeMonsterDrop replies in MsgDataPacket.AnimaCrest).
//  Client -> server: the same packet, Strings = { "</F>ClientManager_Callback</N>{msgId}</S>{func}[</N>{arg}...]" },
//                    answered in Game\MsgServer\MsgDataPacket.cs AnimaCrest.
//
//  Top bar (Fgui activity menu): FguiActivityMenu_OpenData|{['Code']=200,['Data']={{['Id']='1249',['Red']=0},...}}
//      sent on login; the client asks again with 10107 ButtonStorage_Refresh -> FguiActivityMenu_UpdateList (same data).
//      1249 Anima Crest. (1274 Quell Demons and 150214 FuiActCenter are fguibtnconfig ids too, but the official 2024
//      layout shows Quell Demons in the activity-menu container, see OpenActivityMenu.)
//  Festival icons: festival.json icon_show / icon_hide (Events\Framework), pushed on start / stop and on login.
// ============================================================================
using System;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.LuaUi
{
    public static class LuaPush
    {
        public const ushort LuaType = 258;// ActionType.AnimaCrest
        public const string CallbackPrefix = "</F>ClientManager_Callback</N>";
        public const uint MsgButtonStorage = 10107, MsgNewServerBoss = 10053, MsgHighlights = 10511, MsgActMenu = 10219;

        /// <summary>Top-bar Fgui buttons pushed on login (ids of the client's fguibtnconfig). Official 2024 layout: only
        /// Anima Crest here; Quell Demons (1274) and Dream Theater (1466) live in the activity-menu container (below).</summary>
        public static readonly string[] TopBarIds = { "1249" };

        /// <summary>Test seam (null on the server): receives every call instead of the socket.</summary>
        public static Action<Client.GameClient, string, uint> SendOverride;

        /// <summary>One Lua call to one player. dwParam = the callback msgId when this answers a client callback.</summary>
        public static bool Send(Client.GameClient client, string call, uint dwParam = 0)
        {
            if (client == null || client.Player == null || string.IsNullOrEmpty(call)) return false;
            try
            {
                var o = SendOverride;
                if (o != null) { o(client, call, dwParam); return true; }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var action = new ActionQuery()
                    {
                        ObjId = client.Player.UID,
                        Type = LuaType,
                        dwParam = dwParam,
                        Timestamp = (uint)Environment.TickCount,
                        Strings = new string[1] { call },
                    };
                    client.Send(stream.ActionCreate(action));
                }
                return true;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return false;
            }
        }

        /// <summary>One Lua call to every online player. Returns how many got it.</summary>
        public static int Broadcast(string call, uint dwParam = 0)
        {
            if (string.IsNullOrEmpty(call) || Pool.GamePoll == null) return 0;
            int n = 0;
            foreach (var c in Pool.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && Send(c, call, dwParam)) n++;
            return n;
        }

        // ------------------------------------------------------------------ top bar
        public static string TopBarData()
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            var ids = TopBarIds.Concat(MidAutumn2024.LunarVoyage.TopBarIds()).ToArray();// [MidAutumn2024 2026-10-14] Lunar Voyage (10459) while the festival runs
            for (int i = 0; i < ids.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['Id']='").Append(ids[i]).Append("',['Red']=0}");
            }
            return sb.Append("}}").ToString();
        }
        /// <summary>update=false: login (FguiActivityMenu_OpenData); true: the client's 10107 refresh (FguiActivityMenu_UpdateList).</summary>
        public static bool SendTopBar(Client.GameClient client, bool update)
        {
            if (update && global::Qahr.Mobile.Phone.MobileEventsMenu.Refresh(client))
                return true;// QahrMobile: the mobile bar (DlgActreceiveUI_OpenUIBindData)
            return Send(client, (update ? "FguiActivityMenu_UpdateList|" : "FguiActivityMenu_OpenData|") + TopBarData(), update ? MsgButtonStorage : 0);
        }

        /// <summary>Login hook (MsgLoginHandler, after UpdateVip): top bar + the icons of the running festivals.</summary>
        public static void OnLogin(Client.GameClient client)
        {
            try
            {
                if (global::Qahr.Mobile.Phone.MobileEventsMenu.OnLogin(client))
                {
                    Festivals.FestivalManager.SendIcons(client);
                    return;// QahrMobile: DlgActreceiveUI_Open instead of the PC Fgui top bar / activity-menu calls
                }
                SendTopBar(client, false);
                OpenActivityMenu(client);
                Festivals.FestivalManager.SendIcons(client);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        // ------------------------------------------------------------------ activity-menu container (msg 10219 = tMsg_Info["ActMenuBtn"])
        // Official 2024 container (client luaui\dialog\dlgactivitybtn + publicfiles\datafiles\actbtnconfig, restored by our
        // client patch): DlgActivityMenuMainBtn (Dialog 1362) opens DlgActivityMenuMainUi, the group of event buttons.
        //   login:   DlgActivityMenuMainBtn_OpenBtn|{['Code']=200,['Data']={['CurTime']=unix,['Role_Mete']=,['Role_Level']=,['ServerArea']=1}}
        //   open:    the client asks every ActBtnConfig SpecialFunc on 10219 with "</N>{BtnId}":
        //              NewServerBossAct_UiRed</N>1274 (Quell Demons), WhiteDayLua_Login</N>1466 (Dream Theater)
        //   answer:  DlgActreceiveUI_CallLuaFucnBack|{['Code']=200,['Data']={['IsShow']=0|1,['BtnId']=id}}   (dwParam 10219)
        //   red dot: DlgActivityMenuMainBtn_RedPointReturnDatas|{['Code']=200,['Data']={['BtnId']=id,['RedPointShow']=0|1}}
        //   clicks:  1274 -> 10053 NewServerBossAct_OpenInterface, 1466 -> 10369 WhiteDayLua_OpenMain.
        public const int ActMenuMinLevel = 80;
        public static bool OpenActivityMenu(Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            return Send(client, "DlgActivityMenuMainBtn_OpenBtn|{['Code']=200,['Data']={['CurTime']=" + now + ",['Role_Mete']=" + client.Player.Reborn
                + ",['Role_Level']=" + client.Player.Level + ",['ServerArea']=1}}");
        }
        static void ActMenuAnswer(Client.GameClient client, uint btnId, bool show)
        {
            Send(client, "DlgActreceiveUI_CallLuaFucnBack|{['Code']=200,['Data']={['IsShow']=" + (show ? 1 : 0) + ",['BtnId']=" + btnId + "}}", MsgActMenu);
        }
        /// <summary>10219 SpecialFunc requests of the container. True = answered.</summary>
        static bool ActMenuCallback(Client.GameClient client, string func, string[] args)
        {
            bool lv = client.Player.Level >= ActMenuMinLevel;
            switch (func)
            {
                case "NewServerBossAct_UiRed":// Quell Demons (1274)
                    ActMenuAnswer(client, 1274, lv);
                    return true;
                case "WhiteDayLua_Login":// Dream Theater (1466): only while the festival runs
                    {
                        bool show = lv && DreamTheater.DreamTheaterModule.Running;
                        ActMenuAnswer(client, 1466, show);
                        if (show)
                            Send(client, "DlgActivityMenuMainBtn_RedPointReturnDatas|{['Code']=200,['Data']={['BtnId']=1466,['RedPointShow']="
                                + (DreamTheater.DreamTheaterModule.HasClaimableReward(client) ? 1 : 0) + "}}", MsgActMenu);
                        return true;
                    }
            }
            return false;
        }

        // ------------------------------------------------------------------ Quell Demons (10053 = tMsg_Info["CoolServerBoss"])
        // Official window DlgCoolClothesBossUi (client luaui\dialog\dlgcoolclothesboss, Dialog 1274), Code must be 100:
        //   DlgCoolClothesBossUi_OpenMainView / _UpdateMainView|{['Code']=100,['Data']={['ServerType']=1,['BossIndex']=0..4,
        //   ['BossTime']=seconds to the next appearance,['AllBossState']={5 x 0|1 (1 = on the map now)},['KillNum']=weekly kills}}
        //   Bosses and times = luaUiRes["1274"]["Main"] bossch/cxsj: 0 Highest Ruler 20:00 + 21:30 daily, 1 Ghost General :00,
        //   2 Hercules Ax :20, 3 Xuanyuan Master :40 each hour, 4 Devourer Sunday 22:00 after 50,000 demons in a week.
        //   ServerType 1 = the 5-boss layout (the EN client has the Devourer texts).
        // NOT on this server yet (so the window shows the schedule only): the Infernix Ruins boss spawns, the weekly demon
        // counter, the damage ranking, the Infernix Crystals / Trophy (NewServerBossAct_CupInterface), the exchange shop
        // (NewServerBossAct_GotoExchangShop) and "Go challenge" (NewServerBossAct_ChgMap). AllBossState is therefore all 0
        // (no boss is alive) and KillNum 0 (no demon counted); those callbacks stay unanswered (unknown_packets log).
        public static readonly int[] QuellHighestRulerMinutes = { 20 * 60, 21 * 60 + 30 };
        static int QuellSecondsTo(int index, DateTime now)
        {
            DateTime next;
            switch (index)
            {
                case 0:
                    next = DateTime.MaxValue;
                    for (int d = 0; d <= 1; d++)
                        foreach (int m in QuellHighestRulerMinutes)
                        {
                            var t = now.Date.AddDays(d).AddMinutes(m);
                            if (t > now && t < next) next = t;
                        }
                    break;
                case 1: case 2: case 3:
                    {
                        int minute = (index - 1) * 20;
                        next = now.Date.AddHours(now.Hour).AddMinutes(minute);
                        if (next <= now) next = next.AddHours(1);
                        break;
                    }
                default:
                    {
                        int days = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
                        next = now.Date.AddDays(days).AddHours(22);
                        if (next <= now) next = next.AddDays(7);
                        break;
                    }
            }
            return (int)Math.Max(1, Math.Ceiling((next - now).TotalSeconds));
        }
        static string QuellDemonsData(int index)
        {
            var now = DateTime.Now;
            if (index < 0 || index > 4)
            {
                index = 1;// the boss that appears first
                for (int i = 0; i <= 3; i++) if (QuellSecondsTo(i, now) < QuellSecondsTo(index, now)) index = i;
            }
            return "{['Code']=100,['Data']={['ServerType']=1,['BossIndex']=" + index + ",['BossTime']=" + QuellSecondsTo(index, now)
                 + ",['AllBossState']={0,0,0,0,0}" + (index == 4 ? ",['KillNum']=0" : "") + "}}";
        }
        /// <summary>Reply to 10053 NewServerBossAct_OpenInterface (top icon 1274).</summary>
        public static bool OpenQuellDemons(Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            if (QuellDemons.QuellDemonsModule.Open(client)) return true;// [QuellDemons 2026-10-14] live boss state (falls back to the schedule below when the module is off)
            if (client.Player.Level < 80)
            {
                try { client.SendSysMesage("You should reach at least Level 80 before you can participate in the Quell Demons event.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch (Exception err) { MyConsole.SaveException(err); }
                return true;
            }
            return Send(client, "DlgCoolClothesBossUi_OpenMainView|" + QuellDemonsData(-1), MsgNewServerBoss);
        }
        /// <summary>10053 NewServerBossAct_UpateInterface</N>{index} (boss page switch / countdown end).</summary>
        public static bool UpdateQuellDemons(Client.GameClient client, int index)
        {
            if (client == null || client.Player == null || client.Player.Level < 80) return false;
            return Send(client, "DlgCoolClothesBossUi_UpdateMainView|" + QuellDemonsData(Math.Max(0, Math.Min(4, index))), MsgNewServerBoss);
        }

        // ------------------------------------------------------------------ Highlight Events (10511 = tMsg_Info["FuiActCenter"])
        // Click on top icon 150214 -> LUAViewSummary_MainView. Reply (g_function FuiActCenter_Open):
        //   FuiActCenter_Open|{['Code']=200,['Data']={['ServerType']=0,['Info']={{['Id']='<ActInfo id>',['Flag']=0,['IsSelect']=1,['Red']=0},...}}}
        // The pages are the client's ActInfo ids (luaui\fairygui\window\config\actinfo.dat); a page is listed only while a
        // festival that names it in festival.json highlight_acts runs. No page with child tabs / server data is wired yet.
        public static bool OpenHighlights(Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            var acts = Festivals.FestivalManager.RunningHighlightActs();
            if (acts.Count == 0)
            {
                try { client.SendSysMesage("There are no highlight events right now.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch (Exception err) { MyConsole.SaveException(err); }
                return true;
            }
            var sb = new StringBuilder("FuiActCenter_Open|{['Code']=200,['Data']={['ServerType']=0,['Info']={");
            for (int i = 0; i < acts.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{['Id']=").Append(Q(acts[i])).Append(",['Flag']=0,['IsSelect']=").Append(i == 0 ? 1 : 0).Append(",['Red']=0}");
            }
            sb.Append("}}}");
            return Send(client, sb.ToString(), MsgHighlights);
        }

        // ------------------------------------------------------------------ callbacks
        public static string Callback(uint msgId, string func) { return CallbackPrefix + msgId + "</S>" + func; }

        /// <summary>A raw engine call "</F>{func}" (no ClientManager_Callback), e.g. the download reward button.</summary>
        public static bool TryParseRaw(ActionQuery data, out string func)
        {
            func = null;
            if (data == null || data.Strings == null || data.Strings.Length == 0) return false;
            string s = data.Strings[0];
            if (s == null || !s.StartsWith("</F>", StringComparison.Ordinal) || s.StartsWith(CallbackPrefix, StringComparison.Ordinal)) return false;
            func = s.Substring(4).Split(new[] { "</N>" }, StringSplitOptions.None)[0];
            return func.Length > 0;
        }

        /// <summary>Splits "</F>ClientManager_Callback</N>{msgId}</S>{func}</N>{arg}..." (false for anything else).</summary>
        public static bool TryParse(ActionQuery data, out uint msgId, out string func, out string[] args)
        {
            msgId = 0; func = null; args = new string[0];
            if (data == null || data.Strings == null || data.Strings.Length == 0) return false;
            string s = data.Strings[0];
            if (s == null || !s.StartsWith(CallbackPrefix, StringComparison.Ordinal)) return false;
            s = s.Substring(CallbackPrefix.Length);
            int sep = s.IndexOf("</S>", StringComparison.Ordinal);
            if (sep <= 0 || !uint.TryParse(s.Substring(0, sep), out msgId)) return false;
            var parts = s.Substring(sep + 4).Split(new[] { "</N>" }, StringSplitOptions.None);
            func = parts[0];
            args = parts.Skip(1).ToArray();
            return func.Length > 0;
        }

        /// <summary>MsgDataPacket.AnimaCrest: the callbacks not matched there by an exact string. True = answered.</summary>
        public static bool HandleCallback(Client.GameClient client, ActionQuery data)
        {
            if (client == null || client.Player == null) return false;
            try
            {
                uint msgId; string func; string[] args;
                if (SummerWhales.SummerWhalesUi.OnRaw(client, data)) return true;// [SummerWhales 2026-10-14] raw "</F>Func" calls of DlgSummerBtn / DlgSummerMain
                string rawFunc;
                if (TryParseRaw(data, out rawFunc) && global::Qahr.Mobile.Phone.MobileDownloadReward.OnRawCall(client, rawFunc)) return true;// QahrMobile: "Download & Get Rewards" claim (Mobile\Welfare\MobileDownloadReward.cs)
                if (!TryParse(data, out msgId, out func, out args)) return false;
                if (msgId == MsgButtonStorage && global::Qahr.Mobile.Phone.MobileEventsMenu.OnCallback(client, func)) return true;// QahrMobile: ButtonStorage_DailyButton / _LimitedButton
                if (msgId == MsgActMenu) return ActMenuCallback(client, func, args);
                if (msgId == global::Qahr.Mobile.Phone.MobileHud.LuaMsg && global::Qahr.Mobile.Phone.MobileHud.OnLua(client, func, args)) return true;// QahrMobile: new HUD icons of kind "server" (Data\mobile_hud.txt, msg 19991)
                if (msgId == global::Qahr.Mobile.Phone.VipPoints.IconMsg && global::Qahr.Mobile.Phone.VipPoints.OnIcon(client, func)) return true;// QahrMobile: main-screen VIP icon = the VIP menu (VipPoints.cs)
                if (msgId == DreamTheater.DreamTheaterModule.LuaMsg && DreamTheater.DreamTheaterModule.OnLua(client, func, args)) return true;
                if (msgId == MsgNewServerBoss && QuellDemons.QuellDemonsModule.OnLua(client, func, args)) return true;// [QuellDemons 2026-10-14] UpateInterface / CupInterface / GotoExchangShop / ChgMap
                if (msgId == MsgNewServerBoss && func == "NewServerBossAct_UpateInterface")
                {
                    int i; UpdateQuellDemons(client, args.Length > 0 && int.TryParse(args[0], out i) ? i : 0);
                    return true;
                }
                if (msgId == MsgHighlights && func == "LUAViewSummary_MainView") return OpenHighlights(client);
                if (msgId == MidAutumn2024.LunarVoyage.LuaMsg && MidAutumn2024.LunarVoyage.OnLua(client, func, args)) return true;// [MidAutumn2024 2026-10-14] Lunar Voyage window
                if ((msgId == NewYear2023.SpringFestival2023.LuaMsg || msgId == 10250 || msgId == 10139) && NewYear2023.SpringHooks.OnLua(client, msgId, func, args)) return true;// [NewYear2023 2026-10-14/15] Spring Festival 2023 window + its wish window (SpringFestival2023Main_* on 10250, own pending Lover2025Kua_DrawCongratulation on 10139)
                if ((msgId == DragonSaga.DragonSagaUi.LuaMsg || msgId == MsgHighlights) && DragonSaga.DragonSagaUi.OnLua(client, msgId, func, args)) return true;// [DragonSaga 2026-10-14] Dragonfest window + Highlight page 10978
                if ((msgId == SwordSaga.SwordSagaUi.LuaMsg || msgId == MsgHighlights) && SwordSaga.SwordSagaUi.OnLua(client, msgId, func, args)) return true;// [SwordSaga 2026-10-14] Feathered Romance window + Highlight page 10950
                if (msgId == WenwenDetective.WenwenDetectiveUi.LuaMsg && WenwenDetective.WenwenDetectiveUi.OnLua(client, func, args)) return true;// [WenwenDetective 2026-10-14] Goosie Adventure window (Titan Realm)
                if (msgId == CrisisSeries.CrisisSeriesUi.LuaMsg && CrisisSeries.CrisisSeriesUi.OnLua(client, func, args)) return true;// [CrisisSeries 2026-10-14] Castle Startle window (WhiteDay25)
                if ((msgId == AprilFools2025.AprilFools2025Ui.LuaMsg || msgId == AprilFools2025.AprilFools2025Ui.DrawMsg) && AprilFools2025.AprilFools2025Ui.OnLua(client, msgId, func, args)) return true;// [AprilFools2025 2026-10-14] Fool`s Paradise window (25FoolsDay 10568) + Wonder Box DrawClient_*</N>10568 on 10510 (before VillageRequests)
                if (msgId == ChildrensDay2025.ChildrensDay2025Ui.LuaMsg && ChildrensDay2025.ChildrensDay2025Ui.OnLua(client, func, args)) return true;// [ChildrensDay2025 2026-10-14] Dream Park window (ChildrenDay 10664)
                if (msgId == VillageRequests.VillageRequestsUi.LuaMsg && VillageRequests.VillageRequestsUi.OnLua(client, func, args)) return true;// [VillageRequests 2026-10-14] Global Christmas window (Doom Portal -> Doomwoods)
                if (msgId == BandsTrouble.BandsTroubleUi.LuaMsg && BandsTrouble.BandsTroubleUi.OnLua(client, func, args)) return true;// [BandsTrouble 2026-10-14] Hot Music Festival window (24Summer 10426)
                if (msgId == CastleMystery.CastleMysteryUi.LuaMsg && CastleMystery.CastleMysteryUi.OnLua(client, func, args)) return true;// [CastleMystery 2026-10-14] Castle Startle Fgui 10139 (Lover2025Kua)
                if ((msgId == TwinklingShard.TwinklingShardUi.LuaMsg || msgId == MsgHighlights) && TwinklingShard.TwinklingShardUi.OnLua(client, msgId, func, args)) return true;// [TwinklingShard 2026-10-14] Twinkling Fest window (Christmas25 10831) + Highlight page 10831
                if (msgId == WhiteSnakeLegend.WhiteSnakeLegendUi.LuaMsg && WhiteSnakeLegend.WhiteSnakeLegendUi.OnLua(client, func, args)) return true;// [WhiteSnakeLegend 2026-10-14] Bless All window (NewYear25 10527)
                if (msgId == PuppetTheater.PuppetBattle.EndMsg && PuppetTheater.PuppetBattle.OnEnd(client, func, args)) return true;// [PuppetTheater 2026-10-15] puppet battle window end/skip of OUR fights (ServerParam PTB..)
                if (msgId == Halloween2024.Halloween2024Ui.LuaMsg && Halloween2024.Halloween2024Ui.OnLua(client, func, args)) return true;// [Halloween2024 2026-10-14] Global Halloween window (Fui24Halloween 10482)
                if (msgId == Thanksgiving2024.Thanksgiving2024Ui.LuaMsg && Thanksgiving2024.Thanksgiving2024Ui.OnLua(client, func, args)) return true;// [Thanksgiving2024 2026-10-14] Global Thanksgiving window (Fui24Ge 10486)
                if (msgId == QixiInterplane2024.QixiInterplane2024Ui.LuaMsg && QixiInterplane2024.QixiInterplane2024Ui.OnLua(client, func, args)) return true;// [QixiInterplane2024 2026-10-14] Global Qixi 2024 Interplane window (Fui24Qixi 10448)
                if (msgId == SpringFestival2024.SpringFestival2024Ui.LuaMsg && SpringFestival2024.SpringFestival2024Ui.OnLua(client, func, args)) return true;// [SpringFestival2024 2026-10-14] Spring Festival 2024 window (24SpringFestival 10358)
                if (msgId == Christmas2023.Christmas2023Ui.LuaMsg && Christmas2023.Christmas2023Ui.OnLua(client, func, args)) return true;// [Christmas2023 2026-10-14] Time Exploration window (Dlg23Christmas 10326)
            if (msgId == AprilFools2023.AprilFools2023Ui.StageMsg && AprilFools2023.AprilFools2023Ui.OnStageLua(client, func, args)) return true;// [AprilFools2023 2026-10-15] Tricky Wit stage rewards / rankings (shared DlgFoolsDayPointsRankUi on 10311, only when opened from 10113; before HalloweenSenluo2023)
                if (msgId == HalloweenSenluo2023.HalloweenSenluo2023Ui.LuaMsg && HalloweenSenluo2023.HalloweenSenluo2023Ui.OnLua(client, func, args)) return true;// [HalloweenSenluo2023 2026-10-14] Macabre Realm window (SenluoHalloween 10311)
                if ((msgId == SpringFestival2026.SpringFestival2026Ui.LuaMsg || msgId == MsgHighlights) && SpringFestival2026.SpringFestival2026Ui.OnLua(client, msgId, func, args)) return true;// [SpringFestival2026 2026-10-14] Steed Legend window (Chunjie26 10850) + Highlight page 10850
                if (msgId == Qixi2025.Qixi2025Ui.LuaMsg && Qixi2025.Qixi2025Ui.OnLua(client, msgId, func, args)) return true;// [Qixi2025 2026-10-14] Love Cycles window (Qixi25 10726)
                if ((msgId == MidAutumnReunion2022.MidAutumnReunion2022Ui.LuaMsg || msgId == MidAutumnReunion2022.MidAutumnReunion2022Ui.CommonMsg) && MidAutumnReunion2022.MidAutumnReunion2022Ui.OnLua(client, msgId, func, args)) return true;// [MidAutumnReunion2022 2026-10-15] Reunion Under Moon (MidAutumnFestival 10052 + MidAutumn_Common 10049 elixir/stages, only while it runs; before AprilFools2026)
                if (msgId == AprilFools2026.AprilFools2026Ui.LuaMsg && AprilFools2026.AprilFools2026Ui.OnLua(client, func, args)) return true;// [AprilFools2026 2026-10-14] Astral Dream window (MidAutumnFestival 10052, 26QqYrj)
                if (msgId == AliceRescue.AliceRescueUi.LuaMsg && AliceRescue.AliceRescueUi.OnLua(client, func, args)) return true;// [AliceRescue 2026-10-14] Wonderland window (DlgFoolsDay24 10376)
                if ((msgId == DemonInvasionMadPet.DemonInvasionMadPetUi.LuaMsg || msgId == DemonInvasionMadPet.DemonInvasionMadPetUi.RankMsg) && DemonInvasionMadPet.DemonInvasionMadPetUi.OnLua(client, msgId, func, args)) return true;// [DemonInvasionMadPet 2026-10-15] Hewing Laurel (MidAutumnFestival 10052: MidTree2023_*, MidAutumnKua_*/ZQExplorePage_* while running; Cultivation stages 10295)
                if (msgId == AnniversaryOtherworld2024.AnniversaryOtherworld2024Ui.LuaMsg && AnniversaryOtherworld2024.AnniversaryOtherworld2024Ui.OnLua(client, func, args)) return true;// [AnniversaryOtherworld2024 2026-10-14] Anniversary Cake Gala window (24Anniversary 10389)
                if (msgId == SportsMeet2024.SportsMeet2024Ui.LuaMsg && SportsMeet2024.SportsMeet2024Ui.OnLua(client, func, args)) return true;// [SportsMeet2024 2026-10-14] CO Sports window (FuiSports 10453)
                if (msgId == Summer2023.Summer2023Ui.LuaMsg && Summer2023.Summer2023Ui.OnLua(client, func, args)) return true;// [Summer2023 2026-10-14] Dream Cruise window (Summer23 10250)
                if (msgId == QixiQingqiu2023.QixiQingqiu2023Ui.LuaMsg && QixiQingqiu2023.QixiQingqiu2023Ui.OnLua(client, func, args)) return true;// [QixiQingqiu2023 2026-10-14] Jade Hill window (DlgChineseSaintValentinesDay 10272)
                if (msgId == MayAdventure2023.MayAdventure2023Ui.LuaMsg && MayAdventure2023.MayAdventure2023Ui.OnLua(client, func, args)) return true;// [MayAdventure2023 2026-10-14] Anniversary Adventure window (MayAdventure 10236)
                if (msgId == ThanksgivingTribe2023.ThanksgivingTribe2023Ui.LuaMsg && ThanksgivingTribe2023.ThanksgivingTribe2023Ui.OnLua(client, func, args)) return true;// [ThanksgivingTribe2023 2026-10-14] Tribe Fight window (Thanksgiving23 10319)
                if ((msgId == WorldCup2026.WorldCup2026Ui.LuaMsg || msgId == MsgHighlights) && WorldCup2026.WorldCup2026Ui.OnLua(client, msgId, func, args)) return true;// [WorldCup2026 2026-10-14] Pitch Glory window (Foot26 10900, both halves) + Highlight page 10900
                if ((msgId == Anniversary2026.Anniversary2026Ui.LuaMsg || msgId == MsgHighlights) && Anniversary2026.Anniversary2026Ui.OnLua(client, msgId, func, args)) return true;// [Anniversary2026 2026-10-14] Anniversary Sail window (ChouJiang26ZNQ 10883) + Highlight page 10883
                if (msgId == ThanksgivingFashion2025.ThanksgivingFashion2025Ui.LuaMsg && ThanksgivingFashion2025.ThanksgivingFashion2025Ui.OnLua(client, func, args)) return true;// [ThanksgivingFashion2025 2026-10-14] Top Master Campaign window (DaRenXiu 10789)
                if (msgId == ArenaSports.ArenaSportsUi.LuaMsg && ArenaSports.ArenaSportsUi.OnLua(client, msgId, func, args)) return true;// [ArenaSports 2026-10-14] Athlete Showdown window (SportMeet 10753)
                if (msgId == PuppetTheater.PuppetTheaterUi.LuaMsg && PuppetTheater.PuppetTheaterUi.OnLua(client, msgId, func, args)) return true;// [PuppetTheater 2026-10-14] Strings of Terror window (25Halloween 10768)
                if ((msgId == JellyfishSkyCity.JellyfishSkyCityUi.LuaMsg || msgId == MsgHighlights || msgId == JellyfishSkyCity.JellyfishSkyCityUi.StageMsg) && JellyfishSkyCity.JellyfishSkyCityUi.OnLua(client, msgId, func, args)) return true;// [JellyfishSkyCity 2026-10-14] Tidal Serenade window (26WhiteDay 10871) + Highlight page 10871 + StageSybTemplate 150411001 on 10768
                if (msgId == BleakValleyVillage.BleakValleyVillageUi.LuaMsg && BleakValleyVillage.BleakValleyVillageUi.OnLua(client, msgId, func, args)) return true;// [BleakValleyVillage 2026-10-14] Celebration Realm window (Anniversary 10217)
                if (msgId == BleakValleyVillage.BleakValleyVillageUi.CommonMsg && BleakValleyVillage.BleakValleyVillageUi.OnLua(client, msgId, func, args)) return true;// [BleakValleyVillage 2026-10-15] Celebration Monument cumulative rewards (AnniversaryCommon 10218)
                if (msgId == Christmas2022.Christmas2022Ui.LuaMsg && Christmas2022.Christmas2022Ui.OnLua(client, func, args)) return true;// [Christmas2022 2026-10-14] Xmas Miracle Factory window (DlgChristmas 10153)
                if (msgId == Thanksgiving2022.Thanksgiving2022Ui.LuaMsg && Thanksgiving2022.Thanksgiving2022Ui.OnLua(client, func, args)) return true;// [Thanksgiving2022 2026-10-14] Battlefront of Subjugation window (GlobalThanksgiving 10133)
                if (msgId == Valentine2023.Valentine2023Ui.LuaMsg && Valentine2023.Valentine2023Ui.OnLua(client, func, args)) return true;// [Valentine2023 2026-10-14] Starlight Recital window (ValentinesDayConcert_* on 10139, after CastleMystery)
                if (msgId == AprilFools2023.AprilFools2023Ui.LuaMsg && AprilFools2023.AprilFools2023Ui.OnLua(client, func, args)) return true;// [AprilFools2023 2026-10-14] Tricky Wit maze window (HalloweenMaze 10113)
                if (msgId == QixiRomance2022.QixiRomance2022Ui.LuaMsg && QixiRomance2022.QixiRomance2022Ui.OnLua(client, func, args)) return true;// [QixiRomance2022 2026-10-14] Romantic Season window (QiXiFestival 10044)
                if (msgId == BruceLeePreheat.BruceLeePreheatUi.LuaMsg && BruceLeePreheat.BruceLeePreheatUi.OnLua(client, func, args)) return true;// [BruceLeePreheat 2026-10-14] Beast Scroll window (AwakenBruceLee 10127)
                if ((msgId == Summer2025.Summer2025Ui.LuaMsg || msgId == MsgHighlights) && Summer2025.Summer2025Ui.OnLua(client, msgId, func, args)) return true;// [Summer2025 2026-10-14] Sylvan Whisper window (Summer25 10707, SummerDraw2025_*; not 150437 SummerActive2023Page_*) + Highlight page 10707
                if ((msgId == AnniversaryWindblownSand2025.AnniversaryWindblownSand2025Ui.DuneMsg || msgId == AnniversaryWindblownSand2025.AnniversaryWindblownSand2025Ui.JubileeMsg) && AnniversaryWindblownSand2025.AnniversaryWindblownSand2025Ui.OnLua(client, msgId, func, args)) return true;// [AnniversaryWindblownSand2025 2026-10-14] Anniversary 2025 Dune window (Anniversary25 10611) + Jubilee window (25Jubilee 10631, festival AnniversaryJubilee2025)
                var fest = Festivals.FestivalManager.RunningByIconClick(msgId, func);// a festival icon without its own window
                if (fest != null)
                {
                    try { client.SendSysMesage(fest.StartEn, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch (Exception err) { MyConsole.SaveException(err); }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return false;
            }
        }

        /// <summary>A Lua single-quoted string literal.</summary>
        public static string Q(string s)
        {
            return "'" + (s ?? "").Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ") + "'";
        }

        /// <summary>The client's StrToTable deletes every "local" in the text and splits the call at '|'.</summary>
        public static string SafeArg(string s)
        {
            return s.Replace("local", "Local").Replace("|", "/");
        }

        const int ScriptChunk = 600;// bigger Lua pushes (about 1400 bytes) never reached the client

        /// <summary>
        /// Runs a server-side Lua script on the client: sent in chunks through StrToTable, then pcall(load(...)).
        /// The source must contain no "local" and no '|'. <paramref name="tag"/> names the temporary global.
        /// </summary>
        public static bool SendScript(Client.GameClient client, string tag, string source)
        {
            string src = source.Replace("\r", "");
            string v = tag + "_Src";
            for (int i = 0; i < src.Length; i += ScriptChunk)
            {
                string part = src.Substring(i, Math.Min(ScriptChunk, src.Length - i));
                if (!Send(client, "StrToTable|(function(s) " + v + " = " + (i == 0 ? "" : "(" + v + " or '') .. ") + "s end)([==[" + part + "]==])"))
                    return false;
            }
            return Send(client, "StrToTable|(function() " + tag + "_Ok = pcall(load(" + v + ")) " + v + " = nil end)()");
        }
    }
}
