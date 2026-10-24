namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// MsgTrade (2029) action numbers of the mobile client (CMsgTrade 10119, action u16 @12). The mobile keeps the old
    /// CO numbering, the server (MsgTrade.TradeID) its own:
    ///   mobile -> server (CHero::ApplyTrade/AcceptTrade send 1 for both, AgreeTrade 10, RefuseTrade 17, ...):
    ///     1 Request / AcceptRequest (decided here: accept when the target already asked us), 2 Close, 6 AddItem,
    ///     7 SetMoney, 10 Accept, 13 SetCPs (server ShowCPS = "I put CPs"), 17 Refuse (= Close)
    ///   server -> mobile (CMsgTrade::Process switch, action-1 in 0..14):
    ///     1 CHero::AddApply (request), 3 OpenTrade (ShowTable), 4 CloseTrade(1) (Finish), 5 CloseTrade(0) (HideTable),
    ///     8 SetPlayerTradeMoney (ShowMoney), 9 SetHeroTradeMoney, 10 TradePlayerAgree (Accept), 11 AddItemFalse
    ///     (RemoveItem), 12 SetPlayerTradeEmoney (partner CPs = server SetCPS), 14 SetHeroTradeEmoney
    /// The outgoing number travels in MsgTrade.Trade.MobileAction (proto field 7), which the 2029 layout puts at @12.
    /// </summary>
    public static class MobileTrade
    {

        /// <summary>Server TradeID -> mobile action.</summary>
        public static uint ToMobile(uint server)
        {
            switch (server)
            {
                case 0: return 1;   // Request -> AddApply
                case 1: return 1;
                case 2: return 5;   // Close -> CloseTrade(0)
                case 3: return 3;   // ShowTable -> OpenTrade
                case 4: return 4;   // Finish -> CloseTrade(1)
                case 5: return 5;   // HideTable -> CloseTrade(0)
                case 7: return 9;   // SetMoney echo -> own money
                case 8: return 8;   // ShowMoney -> partner money
                case 9: return 10;  // Accept -> partner agreed
                case 10: return 11; // RemoveItem (item refused) -> AddItemFalse
                case 11: return 12; // SetCPS (to the partner) -> partner CPs
                case 12: return 14; // ShowCPS echo -> own CPs
                default: return server;
            }
        }

        /// <summary>The mobile has one value at @4; the layout copies it into every field that maps there (UID, ItemUID,
        /// CPS, Money). Keep only the one the action uses, else the echoes carry the wrong value (Money wins @4).</summary>
        public static void Normalize(Game.MsgServer.MsgTrade.Trade t)
        {
            uint v = t.UID != 0 ? t.UID : (t.ItemUID != 0 ? t.ItemUID : (uint)t.Money);
            ulong money = t.Money;
            t.UID = 0; t.ItemUID = 0; t.CPS = 0; t.Money = 0;
            switch (t.Action)
            {
                case 0: case 1: case 2: t.UID = v; break;
                case 6: case 10: t.ItemUID = v; break;
                case 7: t.Money = money; break;
                case 11: case 12: t.CPS = v; break;
            }
        }

        /// <summary>Mobile action -> server TradeID, for a packet that came from a mobile client.</summary>
        public static uint FromMobile(Client.GameClient user, uint mobile, uint targetUid)
        {
            switch (mobile)
            {
                case 1:
                    {
                        Client.GameClient t;
                        if (Pool.GamePoll.TryGetValue(targetUid, out t) && t != null && t.Player != null
                            && t.Player.targetTrade == user.Player.UID && t.MyTrade != null)
                            return 1;               // AcceptRequest
                        return 0;                   // Request
                    }
                case 2: return 2;
                case 6: return 6;
                case 7: return 7;
                case 10: return 9;
                case 13: return 12;                 // the server's "add CPs" case is ShowCPS
                case 17: return 2;                  // refuse = close
                default: return 0xFFFF;             // unknown: ignored by the switch
            }
        }
    }
}
