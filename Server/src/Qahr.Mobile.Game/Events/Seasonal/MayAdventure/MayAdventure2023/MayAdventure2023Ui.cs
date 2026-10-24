// ============================================================================
//  [MayAdventure2023 2026-10-14 / COMPLETE 2026-10-15] Anniversary 2023 "Anniversary Adventure" (ERP536530, 5.16-5.29),
//  client tMsg_Info["MayAdventure"] = 10236. Shown while the festival MayAdventure2023 runs (festival.json icon_show / icon_hide).
//
//  Official client (luaui\dialog\dlgmayadventure, luagui\1380.dat / 1379.dat, luaUiRes_EN[1380]):
//    button   DlgMayAdventureMainBtn, server show call OpenDlgMayAdventureMainBtn|{Code=200,Data={Red=n}};
//             click -> 10236 AnniversaryAdventure_Open.
//    main     DlgMayAdventureMainUi_OpenViewDatas|{Code=200,Data={RedPoints,PropList,CouragePoints,CurMap,CurStep,SpecialEvent,CostProp}}
//    move     AnniversaryAdventure_Begin</N>1|5 -> DlgMayAdventureMainUi_MoveReturn|{Code=200,Data={Type,CurMap,CurStep(before),PropList,CouragePoints}}
//             (Code != 200 re-enables the buttons); after the walk the client sends AnniversaryAdventure_IntoAdventure</N>n
//             -> DlgMayAdventureMainUi_RewardGuideStep|{Code=200,Data={RewardList={...}}} (DlgRewardFourteen, reopens the board).
//    supplies AnniversaryAdventure_OpenDraw (Treasure Hunt button, also the Courage Medal "+" option): 1 Courage Pearl.
//             The 2023 draw window (DlgGThanksGivingDrawMainUi) was re-wired by the client to Thanksgiving 2023 (msg
//             Thanksgiving23), so the draw is confirmed with a message box and shown with the board's reward window.
//    store    AnniversaryAdventure_OpenExchange[</N>page] -> DlgMayAdventureShop_BindData|{Code=200,Data={HaveMoney,GoodItems,Page}}
//             AnniversaryAdventure_Exchange</N>page</N>i.
//    ranking  AnniversaryAdventure_ViewRankAward -> DlgMayAdventureRank_BindData|{Code=200,Data={ActivityId=10238}} (official
//             EN reward preview), AnniversaryAdventure_ViewRank (the "go to rankings" button) -> top 10 + own rank.
//    ways     AnniversaryAdventure_Way</N>1 Daily Sign-in Pack, </N>2 100/200 Active Points Pack, </N>3 CP Mall.
//    public   MayThemeAdventure_EnterThing1/2 (Ahriman Battle / Lava Land (CS), 16/18/20 h) - their maps are not in
//             FestivalMaps\maps.txt and Lava Land is cross-server: polite message.
//  Rules / numbers: MayAdventureCore.cs; derived values: Data\MayAdventure2023\mayadventure.json.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10236), one tagged line (unchanged since 2026-10-14).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.AnnivKit;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.MayAdventure2023
{
    public static class MayAdventure2023Ui
    {
        public const uint LuaMsg = 10236;
        public const string Festival = "MayAdventure2023";
        public const int MinReborn = 2;// ActBtnConfig[1] NeedMetempsychosis
        public const uint CourageToken = 3341961, CouragePearl = 3341960, CourageMedal = 3341903;
        public const string Ended = "How time flies! The Anniversary Adventure has come to an end.";// tAnniversaryAdventure_DelText[31611] Text121
        public const int ShopPage = 6;

        public static readonly Kit.CfgCache<MayCfg> Config = new Kit.CfgCache<MayCfg>(System.IO.Path.Combine("MayAdventure2023", "mayadventure.json"), MayCfg.Read, MayCfg.Default);
        public static MayCfg Cfg { get { return Config.Get(); } }

        public static bool Running { get { return Kit.Running(Cfg.Festival); } }

        static void Lua(Client.GameClient c, string call) { Kit.Lua(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { Kit.Msg(c, t); }
        static int Arg(string[] a, int i, int def) { int v; return a != null && a.Length > i && int.TryParse(a[i], out v) ? v : def; }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10236).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("AnniversaryAdventure_", StringComparison.Ordinal) || func.StartsWith("MayThemeAdventure_", StringComparison.Ordinal);
            if (!known) return false;
            // the open window polls this every frame during 16:00/18:00/20:00 - answer silently
            if (func == "MayThemeAdventure_RefreshThing" || func == "AnniversaryAdventure_GuideEnd") return true;
            uint uid = c.Player.UID;
            if (Kit.Throttled(Festival, uid, 350))
            {
                if (func == "AnniversaryAdventure_Begin") Lua(c, "DlgMayAdventureMainUi_MoveReturn|{['Code']=0}");// keep the buttons usable
                return true;
            }
            var cfg = Cfg;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Reborn < cfg.MinReborn) { Msg(c, "Requirement: 2nd Rebirth."); return true; }
            try
            {
                switch (func)
                {
                    case "AnniversaryAdventure_Open":
                        lock (MayAdventureCore.Store.Sync) MayAdventureCore.Touch(cfg, uid, c.Player.Name);
                        Lua(c, "DlgMayAdventureMainUi_OpenViewDatas|" + MainData(c, cfg));
                        PayRanks(c, cfg);
                        return true;
                    case "AnniversaryAdventure_Begin":
                        Begin(c, cfg, Arg(args, 0, 0));
                        return true;
                    case "AnniversaryAdventure_IntoAdventure":
                        ShowLast(c, cfg);
                        return true;
                    case "AnniversaryAdventure_OpenDraw":
                        AskDraw(c, cfg);
                        return true;
                    case "AnniversaryAdventure_OpenExchange":
                        OpenShop(c, cfg, Arg(args, 0, 1));
                        return true;
                    case "AnniversaryAdventure_Exchange":
                        Exchange(c, cfg, Arg(args, 0, 0), Arg(args, 1, 0));
                        return true;
                    case "AnniversaryAdventure_ViewRankAward":
                        Lua(c, "DlgMayAdventureRank_BindData|{['Code']=200,['Data']={['ActivityId']=10238}}");
                        return true;
                    case "AnniversaryAdventure_ViewRank":
                        ShowRank(c, cfg);
                        return true;
                    case "AnniversaryAdventure_Way":
                        Way(c, cfg, Arg(args, 0, 0));
                        return true;
                    case "MayThemeAdventure_EnterThing1":
                    case "MayThemeAdventure_EnterThing2":
                        Msg(c, "The public adventures (Ahriman Battle / Lava Land) aren`t open on this server: their maps are not available.");
                        return true;
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }

        // ------------------------------------------------------------------ data builders
        static string PropList(Client.GameClient c, MayCfg cfg)
        {
            return "{'" + cfg.Token + " 0 1," + Kit.Count(c, cfg.Token) + "','" + cfg.Pearl + " 0 1," + Kit.Count(c, cfg.Pearl) + "','" + cfg.Medal + " 0 1," + Kit.Count(c, cfg.Medal) + "'}";
        }

        /// <summary>The DlgMayAdventureMainUi data with this player's real bag counts and board state.</summary>
        public static string MainData(Client.GameClient c, MayCfg cfg = null)
        {
            cfg = cfg ?? Cfg;
            uint uid = c.Player.UID;
            int red = Kit.Count(c, cfg.Pearl) > 0 ? 1 : 0;
            var sb = new StringBuilder("{['Code']=200,['Data']={['RedPoints']={").Append(red).Append(",0,0},['PropList']=").Append(PropList(c, cfg)).Append(',');
            sb.Append("['CouragePoints']=").Append(MayAdventureCore.Points(uid)).Append(",['CurMap']=").Append(MayAdventureCore.Map(uid))
              .Append(",['CurStep']=").Append(MayAdventureCore.Step(uid)).Append(",['SpecialEvent']=0,");
            sb.Append("['CostProp']={'").Append(cfg.Token).Append(" 0 1,1','").Append(cfg.Token).Append(" 0 1,5'}");
            return sb.Append("}}").ToString();
        }

        // ------------------------------------------------------------------ board
        static string Blocked(Client.GameClient c, MayCfg cfg)
        {
            var p = c.Player;
            if (c.Team != null) return "Failed to adventure when you are in a team.";
            if (p.Map != cfg.RequiredMap || p.DynamicID != 0 || !p.Alive) return "Failed to adventure outside Twin City.";
            if (c.IsVendor) return "Failed to adventure during peddling.";
            try { if (p.ContainFlag(MsgUpdate.Flags.FlashingName)) return "Failed to adventure when you are blinking blue."; } catch { }
            return null;
        }

        static void Begin(Client.GameClient c, MayCfg cfg, int n)
        {
            string why = n == 1 || n == 5 ? Blocked(c, cfg) : "";
            if (why != null)
            {
                Lua(c, "DlgMayAdventureMainUi_MoveReturn|{['Code']=0}");
                if (why.Length > 0) Msg(c, why);
                return;
            }
            var o = MayAdventureCore.Move(cfg, c.Player.UID, c.Player.Name, new ClientBag(c), n);
            if (!o.Ok)
            {
                Lua(c, "DlgMayAdventureMainUi_MoveReturn|{['Code']=0}");
                if (!string.IsNullOrEmpty(o.Error)) Msg(c, o.Error);
                return;
            }
            Kit.Log(Festival, "move", c, "x" + n + " " + o.FromMap + ":" + o.FromStep + " -> " + o.ToMap + ":" + o.ToStep + " pts=" + o.Points + " got " + string.Join(", ", o.Given));
            Lua(c, "DlgMayAdventureMainUi_MoveReturn|{['Code']=200,['Data']={['Type']=" + n + ",['CurMap']=" + o.FromMap + ",['CurStep']=" + o.FromStep +
                   ",['PropList']=" + PropList(c, cfg) + ",['CouragePoints']=" + o.Points + "}}");
        }

        /// <summary>After the walk: the rewards window of the last move (nothing granted here - the move already paid).</summary>
        static void ShowLast(Client.GameClient c, MayCfg cfg)
        {
            var l = MayAdventureCore.Last(c.Player.UID);
            if (l.Count == 0) { Lua(c, "DlgMayAdventureMainUi_OpenViewDatas|" + MainData(c, cfg)); return; }
            MayAdventureCore.ClearLast(c.Player.UID);
            ShowRewards(c, l);
            if (l.Count == 1) Msg(c, "Congrats! You have completed the adventure and gained " + cfg.PointsPerAdventure + " Courage Points and " + Kit.Names(l) + "!");
            else
            {
                Msg(c, "You have adventured " + l.Count + " times. Here are your rewards:");
                for (int i = 0; i < l.Count; i++) Msg(c, (i + 1) + "            " + Kit.Names(new[] { l[i] }));
            }
        }
        static void ShowRewards(Client.GameClient c, IEnumerable<Prize> l)
        {
            Lua(c, "DlgMayAdventureMainUi_RewardGuideStep|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", l.Take(14).Select(p => "'" + p.ClientString() + "'")) + "}}}");
        }

        // ------------------------------------------------------------------ supplies (Courage Pearl)
        static void AskDraw(Client.GameClient c, MayCfg cfg)
        {
            long pearls = Kit.Count(c, cfg.Pearl);
            if (pearls < 1) { Msg(c, "Failed to adventure for lack of Courage Pearls."); return; }
            c.Player.MessageBox("Use 1 Courage Pearl to get adventure supplies? (You have " + pearls + ".)", cl => Draw(cl), null, 60);
        }
        static void Draw(Client.GameClient c)
        {
            try
            {
                if (c == null || c.Player == null) return;
                var cfg = Cfg;
                if (!Running) { Msg(c, Ended); return; }
                if (c.Player.Reborn < cfg.MinReborn || Kit.Throttled(Festival + "draw", c.Player.UID, 500)) return;
                var o = MayAdventureCore.Draw(cfg, c.Player.UID, c.Player.Name, new ClientBag(c));
                if (!o.Ok) { if (!string.IsNullOrEmpty(o.Error)) Msg(c, o.Error); return; }
                Kit.Log(Festival, "draw", c, "1 Courage Pearl -> " + string.Join(", ", o.Given) + " pts=" + o.Points);
                Msg(c, "Congrats! You received " + Kit.Names(o.Given) + "!");
                ShowRewards(c, o.Given);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        // ------------------------------------------------------------------ Anniversary Store
        public static string ShopData(Client.GameClient c, MayCfg cfg, int page)
        {
            int pages = Math.Max(1, (cfg.Shop.Count + ShopPage - 1) / ShopPage);
            page = Math.Max(1, Math.Min(pages, page));
            uint uid = c.Player.UID;
            var goods = new List<string>();
            for (int i = (page - 1) * ShopPage; i < Math.Min(cfg.Shop.Count, page * ShopPage); i++)
            {
                var g = cfg.Shop[i];
                string left = g.Limit > 0 ? Math.Max(0, g.Limit - MayAdventureCore.Bought(uid, i)).ToString() : "99999999";
                string lim = g.Limit > 0 ? g.Limit.ToString() : "99999999";
                goods.Add("'" + g.Item.ClientString() + "," + g.Cost + " " + left + " " + lim + "'");// "item,cost left limit" (Public_AnalysisItemAttr splits the part after the comma by spaces)
            }
            return "{['Code']=200,['Data']={['HaveMoney']='" + cfg.Medal + " 0 1," + Kit.Count(c, cfg.Medal) +
                   "',['GoodItems']={" + string.Join(",", goods) + "},['Page']={" + page + "," + pages + "}}}";
        }
        static void OpenShop(Client.GameClient c, MayCfg cfg, int page)
        {
            lock (MayAdventureCore.Store.Sync) MayAdventureCore.Touch(cfg, c.Player.UID, c.Player.Name);
            Lua(c, "DlgMayAdventureShop_BindData|" + ShopData(c, cfg, page));
        }
        static void Exchange(Client.GameClient c, MayCfg cfg, int page, int slot)
        {
            int pages = Math.Max(1, (cfg.Shop.Count + ShopPage - 1) / ShopPage);
            if (page < 1 || page > pages || slot < 1 || slot > ShopPage) return;
            int idx = (page - 1) * ShopPage + (slot - 1);
            if (idx >= cfg.Shop.Count) return;
            var o = MayAdventureCore.Buy(cfg, c.Player.UID, c.Player.Name, new ClientBag(c), idx);
            if (!o.Ok) { if (!string.IsNullOrEmpty(o.Error)) Msg(c, o.Error); OpenShop(c, cfg, page); return; }
            Kit.Log(Festival, "shop", c, "#" + idx + " " + cfg.Shop[idx].Cost + " Courage Medals -> " + string.Join(", ", o.Given));
            Msg(c, "Congrats! You received " + Kit.Names(o.Given) + "!");
            OpenShop(c, cfg, page);
        }

        // ------------------------------------------------------------------ rankings
        static void ShowRank(Client.GameClient c, MayCfg cfg)
        {
            var top = MayAdventureCore.Top(cfg, 10);
            var sb = new StringBuilder("Courage Rankings\n");
            if (top.Count == 0) sb.Append("No one has Courage Points yet.\n");
            for (int i = 0; i < top.Count; i++) sb.Append(i + 1).Append(". ").Append(top[i].Key).Append("  ").Append(top[i].Value).Append('\n');
            int me = MayAdventureCore.RankOf(cfg, c.Player.UID);
            sb.Append("Your Courage Points: ").Append(MayAdventureCore.Points(c.Player.UID)).Append(me > 0 ? "  (rank " + me + ")" : "");
            c.CreateBoxDialog(sb.ToString());
        }

        /// <summary>Official Courage Rankings rewards of a finished run (paid once, the next time the window opens).</summary>
        static void PayRanks(Client.GameClient c, MayCfg cfg)
        {
            var o = MayAdventureCore.ClaimRanks(cfg, c.Player.UID, new ClientBag(c));
            if (o.Given.Count > 0)
            {
                Kit.Log(Festival, "rank", c, "rank " + o.Points + " -> " + string.Join(", ", o.Given));
                Msg(c, "Congrats! For your place on the last Courage Rankings you received " + Kit.Names(o.Given) + "!");
            }
            if (!string.IsNullOrEmpty(o.Error)) Msg(c, o.Error);
        }

        // ------------------------------------------------------------------ ways to get Courage Tokens
        static void Way(Client.GameClient c, MayCfg cfg, int way)
        {
            if (way == 3) { Msg(c, "The Courage boxes of the CP Mall aren`t sold on this server. Get Courage Tokens from the Daily Sign-in Pack and the Active Points Packs."); return; }
            uint ap = 0;
            try { ap = c.Activeness != null ? c.Activeness.ActivityPoints : 0; } catch { }
            var o = MayAdventureCore.Way(cfg, c.Player.UID, c.Player.Name, new ClientBag(c), way, ap);
            if (!o.Ok) { if (!string.IsNullOrEmpty(o.Error)) Msg(c, o.Error); return; }
            if (o.Given.Count == 0) return;
            Kit.Log(Festival, "way" + way, c, "AP=" + ap + " -> " + string.Join(", ", o.Given));
            Msg(c, "Congrats! You received " + Kit.Names(o.Given) + "!");
            Lua(c, "DlgMayAdventureMainUi_UpdatePropNum|{['Code']=200,['Data']={['PropList']=" + PropList(c, cfg) + "}}");
        }
    }
}
