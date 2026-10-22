using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Trade
    {
        public Client.GameClient Owner;
        public Client.GameClient Target;
        public TradeLogger Logger;
        public uint ConquerPoints;
        public ulong Money;
        public bool WindowOpen;
        public bool Confirmed;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> Items;

        public Trade(Client.GameClient _owner)
        {
            Owner = _owner;
            Items = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
            Logger = new TradeLogger(this);
        }

        public bool ItemInTrade(Game.MsgServer.MsgGameItem Dataitem)
        {
            return Items.ContainsKey(Dataitem.ITEM_ID);
        }

        public unsafe void AddConquerPoints(MsgTrade.Trade trade, ServerSockets.Packet stream)
        {

            if (Target.InTrade)
            {
                if (Owner.Player.ConquerPoints >= trade.CPS)
                {
                    Owner.Player.ConquerPoints -= (uint)trade.CPS;
                    ConquerPoints += trade.CPS;

                    trade.Action = (uint)MsgTrade.TradeID.SetCPS;
                    Target.Send(stream.CreateTradeInfo(trade));
                }
            }
        }
        public unsafe void AddMoney(MsgTrade.Trade trade, ServerSockets.Packet stream)
        {

            if (Target.InTrade)
            {
                if (Owner.Player.Money >= (ulong)trade.Money)
                {
                    Owner.Player.Money -= (ulong)trade.Money;
                    Money += trade.Money;
                    trade.Action = (uint)MsgTrade.TradeID.ShowMoney;
                    Target.Send(stream.CreateTradeInfo(trade));
                }
            }
        }
        public bool ValidItems()
        {
            foreach (var item in Items.Values)
            {
                if (!Owner.Inventory.ClientItems.ContainsKey(item.UID))
                {
                    return false;
                }
            }
            if (Money > 0)
            {
                this.Logger.AddedMoney(Owner, Target, (ulong)Money);
            }
            if (ConquerPoints > 0)
            {
                this.Logger.AddedCps(Owner, Target, ConquerPoints);
            }

            foreach (var item in Items.Values)
            {
                this.Logger.AddItem(Owner, Target, item);
            }
            this.Logger.Sort();
            return true;
        }

        public unsafe void AddItem(ServerSockets.Packet stream, MsgTrade.Trade trade, Game.MsgServer.MsgGameItem DataItem)
        {
            if (Target.InTrade)
            {
                if (!Owner.Player.TREPIN2 && Owner.Player.CheckPin() && DataItem.AnimaItemID > 0 || !Owner.Player.TREPIN2 && Owner.Player.CheckPin() && DataItem.ITEM_ID >= 4200001 && DataItem.ITEM_ID <= 4200019)
                {

                    trade.Action = (uint)MsgTrade.TradeID.RemoveItem;
                    Owner.Send(stream.CreateTradeInfo(trade));
                    Owner.Player.MessageBox("You must first activate your security code from [Anima Protection].", null, null, 60);
                    return;
                }
                if (DataItem.Locked != 0)
                {
                    ConcurrentDictionary<uint, Role.Instance.Associate.Member> src;
                    if (!Owner.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Partener, out src))
                    {
                        trade.Action = (uint)MsgTrade.TradeID.RemoveItem;
                        Owner.Send(stream.CreateTradeInfo(trade));
                        Owner.SendSysMesage("unable to trade this item.");
                        return;

                    }
                    else if (!src.ContainsKey(Target.Player.UID))
                    {
                        trade.Action = (uint)MsgTrade.TradeID.RemoveItem;
                        Owner.Send(stream.CreateTradeInfo(trade));
                        Owner.SendSysMesage("unable to trade this item.");
                        return;
                    }
                }

                if (DataItem.Bound >= 1 || Database.ItemType.unabletradeitem.Contains(DataItem.ITEM_ID))
                {
                    trade.Action = (uint)MsgTrade.TradeID.RemoveItem;
                    Owner.Send(stream.CreateTradeInfo(trade));

                    Owner.SendSysMesage("unable to trade this item.");


                    return;
                }
                if (Target.Inventory.HaveSpace((byte)(Items.Count + 1)))
                {
                    DataItem.Mode = Flags.ItemMode.Trade;
                    DataItem.Send(Target, stream);
                    DataItem.Mode = Flags.ItemMode.AddItem;
                    Items.TryAdd(DataItem.UID, DataItem);

                }
                else
                {
                    trade.Action = (uint)MsgTrade.TradeID.RemoveItem;
                    Owner.Send(stream.CreateTradeInfo(trade));
#if Arabic
                    Owner.SendSysMesage("There is not enough room in your partner inventory.");
#else
                    Owner.SendSysMesage("There is not enough room in your partner inventory.");
#endif

                }
            }
        }

        public unsafe void CloseTrade(MsgTrade.Trade trade)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var msg = rec.GetStream();

                if (Target.InTrade)
                {
                    trade.Action = (uint)MsgTrade.TradeID.HideTable;

                    trade.UID = Target.Player.UID;
                    Owner.Send(msg.CreateTradeInfo(trade));

                    trade.UID = Owner.Player.UID;
                    Target.Send(msg.CreateTradeInfo(trade));

                    Owner.Player.targetTrade = 0;
                    Target.Player.targetTrade = 0;

                    Target.MyTrade.DestroyItems(msg);
                    Target.MyTrade = null;

                    Owner.MyTrade.DestroyItems(msg);
                    Owner.MyTrade = null;
                }
            }
        }

        public void DestroyItems(ServerSockets.Packet stream)
        {
            Owner.Player.ConquerPoints += (uint)ConquerPoints;
            Owner.Player.Money += (ulong)Money;

            Owner.Player.SendUpdate(stream, Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);

            foreach (var item in Items.Values)
            {
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
        }

    }
}

