// ============================================================================
//  [HalloweenSenluo2023 2026-10-14 / COMPLETE 2026-10-15] Global Halloween 2023 "Macabre Realm" (ERP559756, 10.24-11.06),
//  client tMsg_Info["SenluoHalloween"] = 10311. Shown while the festival HalloweenSenluo2023 runs (festival.json icons).
//
//  Official client (luaui\dialog\dlghalloween23, luagui\1435.dat, luaUiRes_EN[1435]):
//    button   DlgSenluoHalloweenMainBtn ("1435-btn"); the client's own show call is commented out, so the server shows
//             it with StrToTable|LuaUIMgr_ShowDialog("DlgSenluoHalloweenMainBtn"); click -> 10311 HallowMonster2023_OpenMain.
//    main     DlgSenluoHalloweenMainUi_OpenViewData {Guide,RedPoints,Points,PropItems,ExchangeItem,Level,Choosen,RefreshTime}
//             buttons: HallowMonster2023_EnterInstanceMain (zone 1 Death Sideroad), HalloweenDie_WaiWeiEnter (zone 2 Blood
//             Altar), HalloweenNpc2023_OpenRankInterface, HalloweenNpc2023_OpenExchange, CastleBeat2023_BuyItem 2/1.
//    realm    DlgSenluoHalloweenGameUi_OpenViewData {...,KeyItem,BossState,Palace[1..4]{OpenTime,Pass,Mark,Progress,RewardState}}
//             CastleBeat2023_CastleDetails i -> DlgSenluoPalaceDetailUi_OpenViewData; CastleBeat2023_EnterCostKey i;
//             CastleBeat2023_SweepCastle i; CastleBeat2023_StrategyRewardView i -> DlgSenluoPalaceRewardUi_OpenViewData;
//             CastleBeat2023_StrategyReward i; Halloween2023Lord_OpenBossPage -> DlgSenluoBossDetailUi_OpenViewData;
//             Halloween2023Lord_Into / Halloween2023Lord_Sweep; CastleBeat2023_BossEffectReturn (ack, nothing to do).
//  The gameplay is in HsZones.cs; numbers that the client does not carry are in Infinity[DB]\HalloweenSenluo2023\config.json.
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10311), one tagged line (unchanged).
// ============================================================================
using System;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.HalloweenSenluo2023
{
    public static class HalloweenSenluo2023Ui
    {
        public const uint LuaMsg = 10311;
        public const string Festival = "HalloweenSenluo2023";
        public const uint MacabreFlame = HsZones.Flame, MacabreCoin = HsZones.Coin;
        public const string Ended = "The Macabre Realm event has ended. See you next Halloween!";

        public static bool Running { get { return HsKit.Running(Festival); } }
        public static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }
        static void Msg(Client.GameClient c, string t) { HsKit.Msg(c, t); }

        static int Arg(string[] args, int i)
        {
            if (args == null || i >= args.Length) return 0;
            int v; return int.TryParse((args[i] ?? "").Trim(), out v) && v >= 0 && v <= 1000 ? v : 0;
        }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10311).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            bool known = func.StartsWith("HallowMonster2023_", StringComparison.Ordinal) || func.StartsWith("HalloweenNpc2023_", StringComparison.Ordinal)
                      || func.StartsWith("HalloweenDie_", StringComparison.Ordinal) || func.StartsWith("Halloween2023Lord_", StringComparison.Ordinal)
                      || func.StartsWith("CastleBeat2023_", StringComparison.Ordinal);
            if (!known) return false;
            if (func == "CastleBeat2023_BossEffectReturn") return true;// client ack of the seal / boss effect
            if (HsKit.Throttled(c.Player.UID, "lua", 400)) return true;
            if (!Running) { Msg(c, Ended); return true; }
            if (c.Player.Level < HsConfig.Get().MinLevel) { Msg(c, "Sorry, you need to reach level " + HsConfig.Get().MinLevel + " first."); return true; }
            int i = Arg(args, 0);
            switch (func)
            {
                case "HallowMonster2023_OpenMain": OpenMain(c); return true;
                case "HallowMonster2023_EnterInstanceMain": HsZones.EnterZone(c, 1); return true;
                case "HalloweenDie_WaiWeiEnter": HsZones.EnterZone(c, 2); return true;
                case "HalloweenNpc2023_OpenRankInterface": Msg(c, HsZones.RankText(c)); return true;
                case "HalloweenNpc2023_OpenExchange":
                    Msg(c, "The Macabre Treasury (Exchange Store) is run by Count Dracula in Twin City (" + HsZones.DraculaX + "," + HsZones.DraculaY + ").");
                    return true;
                case "CastleBeat2023_BuyItem": Buy(c, i); return true;
                case "CastleBeat2023_CastleDetails": if (i >= 1 && i <= 4) Lua(c, "DlgSenluoPalaceDetailUi_OpenViewData|" + PalaceDetail(c, i)); return true;
                case "CastleBeat2023_EnterCostKey": HsZones.EnterHall(c, i); return true;
                case "CastleBeat2023_SweepCastle": HsZones.SweepHall(c, i); SendGame(c); return true;
                case "CastleBeat2023_StrategyRewardView": if (i >= 1 && i <= 4) Lua(c, "DlgSenluoPalaceRewardUi_OpenViewData|" + RewardView(c, i)); return true;
                case "CastleBeat2023_StrategyReward": HsZones.ClaimHall(c, i); SendGame(c); return true;
                case "Halloween2023Lord_OpenBossPage": Lua(c, "DlgSenluoBossDetailUi_OpenViewData|" + BossDetail(c)); return true;
                case "Halloween2023Lord_Into": HsZones.EnterLord(c); return true;
                case "Halloween2023Lord_Sweep": HsZones.SweepLord(c); SendGame(c); return true;
            }
            Msg(c, "This part of the Macabre Realm isn`t open on this server yet.");
            return true;
        }

        /// <summary>The entry window, or the Macabre Realm window once today's zone is passed.</summary>
        public static void OpenMain(Client.GameClient c)
        {
            var p = HsState.Of(c);
            using (var rec = new ServerSockets.RecycledPacket()) HsZones.DeliverPending(c, p, rec.GetStream());
            HsState.Save();
            if (p.ZonePassed) { SendGame(c); return; }
            Lua(c, "DlgSenluoHalloweenMainUi_OpenViewData|" + MainData(c));
        }
        public static void SendGame(Client.GameClient c) { Lua(c, "DlgSenluoHalloweenGameUi_OpenViewData|" + GameData(c)); }

        static void Buy(Client.GameClient c, int what)
        {
            if (what != 1 && what != 2) return;
            var cfg = HsConfig.Get();
            uint id = what == 2 ? MacabreFlame : HsZones.Card, price = what == 2 ? cfg.BuyFlameCps : cfg.BuyCardCps;
            if (price == 0) { Msg(c, "This item can`t be bought on this server."); return; }
            if (!HsKit.HasSpace(c, 1)) { Msg(c, "Your inventory is full. Please make some room, first."); return; }
            if (c.Player.ConquerPoints < price) { Msg(c, "Sorry, you don`t have enough CPs."); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                c.Player.ConquerPoints -= price;// deduct first
                string got = HsKit.Give(c, new HsGrant(id, 1, false), stream);
                if (got == null) { c.Player.ConquerPoints += price; Msg(c, "Your inventory is full. Please make some room, first."); return; }
                try { Festivals.FestivalManager.Log("H2023", Festival, c.Player.UID + " " + c.Player.Name + " | bought " + got + " for " + price + " CPs"); } catch { }
                Msg(c, "You bought " + got + " for " + price + " CPs.");
            }
            var p = HsState.Of(c);
            if (p.ZonePassed) Lua(c, "DlgSenluoHalloweenGameUi_UpdatePropInfo|{['Data']=" + PropInfo(c, p) + "}");
            else Lua(c, "DlgSenluoHalloweenMainUi_UpdatePropInfo|{['Data']=" + PropInfo(c, p) + "}");
        }

        static string Item(uint id, uint have) { return "'" + id + " 0 1," + have + "'"; }
        static string Item(uint id, uint have, int cost) { return "'" + id + " 0 1," + have + " " + cost + "'"; }
        static string Reds(Client.GameClient c, HsPlayer p)
        {
            int rank = 0;
            if (HsZones.RankClaimOpen() && !p.RankClaimed) { int r = HsState.RankOf(c.Player.UID); if (r >= 1 && r <= 10) rank = 1; }
            int tre = HsKit.Count(c, MacabreCoin) >= 500 ? 1 : 0;
            return "{" + rank + "," + tre + "}";
        }
        static string PropInfo(Client.GameClient c, HsPlayer p)
        {
            return "{['Points']=" + p.Points + ",['PropItems']=" + Item(MacabreFlame, HsKit.Count(c, MacabreFlame)) + ",['ExchangeItem']=" + Item(MacabreCoin, HsKit.Count(c, MacabreCoin))
                + ",['KeyItem']=" + Item(HsZones.Card, HsKit.Count(c, HsZones.Card)) + ",['RedPoints']=" + Reds(c, p) + "}";
        }

        static int ToMidnight() { var now = HsKit.Now; return (int)Math.Max(0, (now.Date.AddDays(1) - now).TotalSeconds); }

        /// <summary>DlgSenluoHalloweenMainUi data (real: bag counts, points, danger level, today's zone).</summary>
        public static string MainData(Client.GameClient c)
        {
            var p = HsState.Of(c);
            var sb = new StringBuilder("{['Code']=200,['Data']={['Guide']=0,['RedPoints']=").Append(Reds(c, p)).Append(",['Points']=").Append(p.Points).Append(',');
            sb.Append("['PropItems']=").Append(Item(MacabreFlame, HsKit.Count(c, MacabreFlame))).Append(',');
            sb.Append("['ExchangeItem']=").Append(Item(MacabreCoin, HsKit.Count(c, MacabreCoin))).Append(',');
            sb.Append("['Level']=").Append(HsState.Danger(p.LordCount)).Append(",['Choosen']=").Append(p.Zone);
            if (p.Zone != 0) sb.Append(",['RefreshTime']=").Append(ToMidnight());
            return sb.Append("}}").ToString();
        }

        /// <summary>DlgSenluoHalloweenGameUi data (the Macabre Realm: 4 halls + Mighty Lord).</summary>
        public static string GameData(Client.GameClient c)
        {
            var p = HsState.Of(c);
            var sb = new StringBuilder("{['Code']=200,['Data']={['Guide']=0,['RedPoints']=").Append(Reds(c, p)).Append(",['Points']=").Append(p.Points).Append(',');
            sb.Append("['PropItems']=").Append(Item(MacabreFlame, HsKit.Count(c, MacabreFlame))).Append(',');
            sb.Append("['ExchangeItem']=").Append(Item(MacabreCoin, HsKit.Count(c, MacabreCoin))).Append(',');
            sb.Append("['KeyItem']=").Append(Item(HsZones.Card, HsKit.Count(c, HsZones.Card))).Append(',');
            sb.Append("['Level']=").Append(HsState.Danger(p.LordCount)).Append(",['BossState']=").Append(HsZones.LordFree(p) ? 2 : 1).Append(",['Palace']={");
            for (int i = 1; i <= 4; i++)
            {
                int left; HsZones.HallOpen(i, out left);
                int prog = HsState.Progress[i - 1];
                int rs = (p.Claimed & (1 << i)) != 0 ? 2 : HsZones.CanClaimHall(p, i) ? 1 : 0;
                sb.Append("[").Append(i).Append("]={['OpenTime']=").Append(left).Append(",['Pass']=").Append((p.HallPassed & (1 << i)) != 0 ? "true" : "false")
                  .Append(",['Mark']=").Append((p.Seals & (1 << i)) != 0 ? "true" : "false").Append(",['Progress']=").Append(prog).Append(",['RewardState']=").Append(rs).Append('}');
                if (i < 4) sb.Append(',');
            }
            return sb.Append("}}}").ToString();
        }

        static string PalaceDetail(Client.GameClient c, int i)
        {
            var p = HsState.Of(c);
            var cfg = HsConfig.Get();
            var sb = new StringBuilder("{['Code']=200,['Data']={['PalaceId']=").Append(i);
            if (HsState.FirstName[i - 1] != null)
            {
                sb.Append(",['PlayerName']=").Append(HsKit.LuaStr(HsState.FirstName[i - 1]));
                if (HsState.FirstGang[i - 1] != null) sb.Append(",['Gang']=").Append(HsKit.LuaStr(HsState.FirstGang[i - 1]));
            }
            sb.Append(",['FirstReward']='").Append(HsZones.GoldBox).Append(" 0 1'");
            sb.Append(",['Odds']=").Append(cfg.SealOdds[i - 1]);
            sb.Append(",['CostItem']=").Append(Item(MacabreFlame, HsKit.Count(c, MacabreFlame), cfg.HallFlameCost));
            sb.Append(",['Rewrads']={'").Append(HsZones.CountBox[i]).Append(" 0 1','").Append(HsZones.EliteBox).Append(" 0 1','").Append(HsZones.Card).Append(" 0 1'}");
            sb.Append(",['WipeState']=").Append((p.HallPassed & (1 << i)) != 0 ? 1 : 0);
            sb.Append(",['Challenge']=").Append(HsState.Progress[i - 1] >= 100 ? 0 : 1);
            return sb.Append("}}").ToString();
        }

        static string RewardView(Client.GameClient c, int i)
        {
            var p = HsState.Of(c);
            int need = HsConfig.Get().RewardPoints;
            return "{['Code']=200,['Data']={['PalaceId']=" + i + ",['Condition_1']={" + HsState.Progress[i - 1] + "," + (HsState.Progress[i - 1] >= 100 ? "true" : "false") + "}"
                + ",['Condition_2']={" + need + "," + (p.Points >= need ? "true" : "false") + "},['Rewrads']={'" + HsZones.HallReward[i] + " 0 1'}}}";
        }

        static string BossDetail(Client.GameClient c)
        {
            var p = HsState.Of(c);
            var cfg = HsConfig.Get();
            int danger = HsState.Danger(p.LordCount);
            bool free = HsZones.LordFree(p);
            return "{['Code']=200,['Data']={['Rewrads']={'" + HsZones.LordBox(danger) + " 0 1'},['CostItem']=" + Item(HsZones.Card, HsKit.Count(c, HsZones.Card), cfg.LordCardCost)
                + ",['Challenge']=" + (free ? "true" : "false") + ",['Wipe']=" + (free && p.LordBeaten ? "true" : "false") + "}}";
        }
    }
}
