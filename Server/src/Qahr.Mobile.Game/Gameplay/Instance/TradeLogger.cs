using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Role.Instance
{
    public class TradeLogger : Object
    {
        private string Logger = string.Empty;
        private Trade OwnerInstance;
        public TradeLogger(Trade instance)
        {
            this.OwnerInstance = instance;
            this.Logger = "[TradeLog]";
        }
        public void AddedMoney(Client.GameClient from, Client.GameClient to, ulong value)
        {
            this.Logger += string.Format("//Money=>[{0}:{1}] Sent {2} Money To [{3}:{4}].", from.Player.UID, from.Player.Name, value, to.Player.UID, to.Player.Name);
        }
        public void AddedCps(Client.GameClient from, Client.GameClient to, uint value)
        {
            this.Logger += string.Format("//CPS=>[[{0}:{1}] Sent {2} ConquerPoints To [{3}:{4}].", from.Player.UID, from.Player.Name, value, to.Player.UID, to.Player.Name);
        }
        public void AddItem(Client.GameClient from, Client.GameClient to, MsgGameItem item)
        {
            this.Logger += string.Format("//Item=>[[{0}:{1}] Sent To [{2}:{3}] Item => {4} StackSize => {5}.", from.Player.UID, from.Player.Name, to.Player.UID, to.Player.Name, Pool.ItemsBase[item.ITEM_ID].Name, item.StackSize);
        }
        public void Sort()
        {
            Database.ServerDatabase.LoginQueue.Enqueue(Logger);
            this.Reset();
        }
        private void Reset()
        {
            this.Logger = "[TradeLog]";
        }
    }
}


