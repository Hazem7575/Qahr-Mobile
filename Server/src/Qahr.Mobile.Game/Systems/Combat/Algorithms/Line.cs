using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms
{
    public class Line
    {
        public class Coords
        {
            public ushort x;
            public ushort y;
        }

        private ushort X1;
        private ushort X2;
        private ushort Y1;
        private ushort Y2;
        private int Range;

        private List<Coords> coords = new List<Coords>();
        public Line(ushort _X1, ushort _Y1, ushort _X2, ushort _Y2, int _range)
        {
            X1 = _X1;
            Y1 = _Y1;
            X2 = _X2;
            Y2 = _Y2;
            Range = _range;
            byte[] directions = GetDirections();
            CreateCoords(directions);
        }
        private void CreateCoords(byte[] directions)
        {
            if (directions == null)
                return;

            byte Leftdir = directions[0];
            byte Rightdir = directions[1];


            ushort _x = X2;
            ushort _y = Y2;
            coords.Add(new Coords() { x = _x, y = _y });
            for (int x = 0; x < Range; x++)
            {
                Role.Core.IncXY((Role.Flags.ConquerAngle)Leftdir, ref _x, ref _y);
                coords.Add(new Coords() { x = _x, y = _y });
            }

            _x = X2;
            _y = Y2;
            for (int x = 0; x < Range; x++)
            {
                Role.Core.IncXY((Role.Flags.ConquerAngle)Rightdir, ref _x, ref _y);
                coords.Add(new Coords() { x = _x, y = _y });
            }
        }

        public bool InLine(ushort x, ushort y)
        {
            foreach (var obj in coords)
            {
                if (obj.x == x && obj.y == y)
                    return true;
            }
            return false;

        }



        private byte[] GetDirections()
        {
            byte dir = (byte)Role.Core.GetAngle(X1, Y1, X2, Y2);
            switch (dir)
            {

                case 7:
                case 3: return new byte[2] { 1, 5 };

                case 0:
                case 4: return new byte[2] { 2, 6 };

                case 1:
                case 5: return new byte[2] { 3, 7 };

                case 2:
                case 6: return new byte[2] { 4, 0 };


            }
            return null;

        }

    }
}
