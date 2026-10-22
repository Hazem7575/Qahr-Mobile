using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms
{
    public class Rectangle
    {

        public ushort _x1;
        public ushort _x2;
        public ushort _y1;
        public ushort _y2;
        public Rectangle(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            _x1 = x1;
            _x2 = x2;
            _y1 = y1;
            _y2 = y2;
            CreateCenter();
        }


        public ushort centerx;
        public ushort centery;
        public void CreateCenter()
        {
            centerx = _x1;
            centery = _y1;
            var anger = Role.Core.GetAngle(_x1, _y1, _x2, _y2);

            for (int x = 0; x < 4; x++)
                Role.Core.IncXY(anger, ref centerx, ref centery);
        }


        public bool Check(ushort targetx, ushort targety, int range)
        {
            return Role.Core.GetDistance(centerx, centery, targetx, targety) <= range;
        }



    }
}
