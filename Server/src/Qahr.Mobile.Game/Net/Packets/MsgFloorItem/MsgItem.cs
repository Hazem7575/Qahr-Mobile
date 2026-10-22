using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Role;

namespace Qahr.Mobile.Game.MsgFloorItem
{
    public class MsgItem : Role.IMapObj
    {
        public enum ItemType
        {
            Item, Money, Cps, Effect
        }
        public static System.Counter UIDS = new System.Counter(900027);//900027

        public DateTime Expire = new DateTime();

        public uint ItemOwner;
        public bool ToMySelf;
        public uint TimeStamp = 900;
        public string Name { get; set; }
        public bool ExpireMySelf { get { return DateTime.Now > Expire.AddSeconds(20); } }
        public bool AllowDynamic { get; set; }
        public uint IndexInScreen { get; set; }
        public uint DynamicID { get; set; }
        public uint Map { get; set; }
        public bool Alive { get { return Expire.AddSeconds(SpecialSecounds != 0 ? SpecialSecounds : 40) > DateTime.Now; } }
        public DateTime AttackStamp = DateTime.Now;


        public MsgServer.MsgGameItem ItemBase;
        public MsgFloorItem.MsgItemPacket MsgFloor;
        public Role.MapObjectType ObjType { get; set; }
        public bool IsTrap() { return MsgFloor.DropType == MsgDropID.Effect; }

        public uint UID { get { return MsgFloor.m_UID; } set { MsgFloor.m_UID = value; } }
        public ushort X { get { return MsgFloor.m_X; } set { MsgFloor.m_X = value; } }
        public ushort Y
        {
            get { return MsgFloor.m_Y; }
            set { MsgFloor.m_Y = value; }
        }

        public Role.GameMap GMap;
        public ItemType Typ;
        public uint Gold;
        public uint ConquerPoints;
        private int SpecialSecounds = 0;

        public Client.GameClient OwnerEffert = null;
        public Database.MagicType.Magic DBSkill = null;
        public byte SpellSoul = 0;

        public MsgItem(MsgServer.MsgGameItem item, ushort x, ushort y
            , ItemType Mode, uint Amount, uint dinamicid, uint _mapid
            , uint _ItemOwner, bool _ToMySelf, Role.GameMap _map, int specialSecound = 0)
        {
            AllowDynamic = false;
            SpecialSecounds = specialSecound;
            GMap = _map;
            ItemOwner = _ItemOwner;

            ToMySelf = _ToMySelf;
            Map = _mapid;
            DynamicID = dinamicid;
            Typ = Mode;
            MsgFloor = MsgFloorItem.MsgItemPacket.Create();
            if (item != null)
            {
                ItemBase = item;

                if (item.Purification.ItemUID != 0 && item.Purification.InLife)
                    MsgFloor.m_ID = item.Purification.PurificationItemID;
                else
                    MsgFloor.m_ID = item.ITEM_ID;
                MsgFloor.m_Color = (byte)item.Color;
                MsgFloor.Plus = item.Plus;
            }
            MsgFloor.ItemOwnerUID = ItemOwner;
            MsgFloor.m_UID = UIDS.Next;
            MsgFloor.m_X = x;
            MsgFloor.m_Y = y;

            ObjType = MapObjectType.Item;
            switch (Mode)
            {
                case ItemType.Cps: ConquerPoints += Amount; break;
                case ItemType.Money: Gold += Amount; break;
            }
            Expire = DateTime.Now;

        }


        public bool CanSee(IMapObj obj)
        {
            if (DynamicID != obj.DynamicID) return false;
            if (Map != obj.Map) return false;
            if (Core.GetDistance(MsgFloor.m_X, MsgFloor.m_Y, obj.X, obj.Y) >= 18)
                return false;
            return true;
        }
        public unsafe void Send(ServerSockets.Packet msg, IMapObj owner)
        {
            msg = msg.ItemPacketCreate(MsgFloor);
            owner.Send(msg);
        }
        public unsafe void Send(ServerSockets.Packet msg)
        {
            foreach (var user in GMap.View.Roles(MapObjectType.Player, X, Y, p => CanSee(p)))
            {
                user.Send(msg);
            }
        }
        public unsafe void SendAll(ServerSockets.Packet stream, MsgDropID Typ)
        {
            if (Alive)
            {
                MsgFloor.DropType = Typ;
                stream = stream.ItemPacketCreate(MsgFloor);
                Send(stream);
            }
            else if (Typ == MsgDropID.Remove || Typ == MsgDropID.RemoveEffect)
            {
                MsgFloor.DropType = Typ;

                if (MsgFloor.m_ID == MsgItemPacket.SoulOneDaggerStorm || MsgFloor.m_ID == MsgItemPacket.SoulTwoDaggerStorm)
                    MsgFloor.m_ID = MsgItemPacket.NormalDaggerStorm;
               

                stream = stream.ItemPacketCreate(MsgFloor);
                Send(stream);
                if ((GMap.cells[X, Y] & MapFlagType.Item) == MapFlagType.Item)
                    GMap.cells[X, Y] &= ~MapFlagType.Item;
            }
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool View)
        {
            return (stream = stream.ItemPacketCreate(MsgFloor));
        }

       

    }
}
