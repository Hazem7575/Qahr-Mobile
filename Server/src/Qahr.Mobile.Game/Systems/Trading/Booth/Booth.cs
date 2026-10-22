using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Role;
using System;
using System.Collections.Generic;

namespace Qahr.Mobile.Game
{
    public struct BoothItem
    {

        public enum CostType : byte
        {
            Silvers = 1,
            ConquerPoints = 2,
        }
        public MsgGameItem Item;
        public uint Cost;
        public CostType Cost_Type;
    }
    public class Booth
    {
        public static System.Counter BoothCounter = new System.Counter(100000);
        private static Dictionary<uint, Booth> Booths = new Dictionary<uint, Booth>();
        public static Dictionary<uint, Booth> Booths2 = new Dictionary<uint, Booth>();
        public static object SyncRoot = new Object();

        public System.SafeDictionary<uint, BoothItem> ItemList;
        public SobNpc Base;
        public MsgMessage HawkMessage;
        public Booth()
        {
            ItemList = new System.SafeDictionary<uint, BoothItem>();
        }
        public static implicit operator SobNpc(Booth booth)
        {
            return booth.Base;
        }
    }
}
