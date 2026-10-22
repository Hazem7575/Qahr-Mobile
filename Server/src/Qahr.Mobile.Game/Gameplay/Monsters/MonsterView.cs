using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
namespace Qahr.Mobile.Game.MsgMonster
{
    public class MonsterView
    {
        public const int ViewThreshold = 18;

     

        private MonsterRole role;
        public MonsterView(MonsterRole _role)
        {
            role = _role;

        }
        /// <summary>
        /// make sure the collection
        /// if all its in screen ( -> ViewThreshold )
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Role.IMapObj> Roles(Role.GameMap map, Role.MapObjectType typ)
        {
            return map.View.Roles(typ, role.X, role.Y, p => CanSee(p));
        }
        public Role.IMapObj GetTarget(Role.GameMap map, Role.MapObjectType typ)
        {
            // Perf: one pass instead of Count() + OrderByDescending().FirstOrDefault() (2 scans + sort).
            // Same result: OrderByDescending is a stable sort, so its first element is the FIRST
            // object (in scan order) with the highest IndexInScreen -> keep the first max (strict >).
            Role.IMapObj best = null;
            uint bestIndex = 0;
            foreach (var obj in map.View.Roles(typ, role.X, role.Y, p => CanSee(p) && p.ObjType == Role.MapObjectType.Player))
            {
                uint index = obj.IndexInScreen;
                if (best == null || index > bestIndex)
                {
                    best = obj;
                    bestIndex = index;
                }
            }
            return best;
            //return map.View.Roles(typ, role.X, role.Y, p => CanSee(p) && p.ObjType == Role.MapObjectType.Player).OrderByDescending(p => p.IndexInScreen).First();
        }
        public unsafe void SendScreen(ServerSockets.Packet msg, Role.GameMap map)
        {
            foreach (var obj in Roles(map, Role.MapObjectType.Player))
                obj.Send(msg);
        }
       
      
        public bool CanSee(Role.IMapObj obj)
        {
            if (obj.Map != role.Map)
                return false;
            if (obj.DynamicID != role.DynamicID)
                return false;
            if (obj.UID == role.UID)
                return false;
            return GetDistance(obj.X, obj.Y, role.X, role.Y) <= ViewThreshold;
        }

        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short x = 0;
            short y = 0;
            if (X >= X2) x = (short)(X - X2);
            else if (X2 >= X) x = (short)(X2 - X);
            if (Y >= Y2) y = (short)(Y - Y2);
            else if (Y2 >= Y) y = (short)(Y2 - Y);
            if (x > y) return x;
            else return y;
        }

      

        internal void CheckScren()
        {
            
        }
    }
}
