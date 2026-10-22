using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms
{
    public class LayTrapThree
    {
        public struct coords
        {
            public int X;
            public int Y;

            public coords(double x, double y)
            {
                this.X = (int)x;
                this.Y = (int)y;
            }
        }

        public ushort X1 { get; set; }
        public ushort Y1 { get; set; }
        public ushort X2 { get; set; }
        public ushort Y2 { get; set; }
        public byte Direction { get; set; }

        public List<coords> LCoords;
        public byte MaxDistance = 30;
        public LayTrapThree(ushort X1, ushort X2, ushort Y1, ushort Y2, byte MaxDistance)
        {

            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;

            this.MaxDistance = MaxDistance;
            LCoords = LineCoords(X1, Y1, X2, Y2);

        }

        List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty)
        {
            return linedda(userx, usery, shotx, shoty);
        }
        List<coords> linedda(int xa, int ya, int xb, int yb)
        {
            int dx = xb - xa, dy = yb - ya, steps, k;
            float xincrement, yincrement, x = xa, y = ya;

            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            List<coords> ThisLine = new List<coords>();
            ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));

            for (k = 0; k < MaxDistance; k++)
            {
                x += xincrement;
                y += yincrement;
                ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));
            }



            return GenerateTrapCoord(ThisLine);
        }

        List<coords> GenerateTrapCoord(List<coords> LineCoord)
        {
            List<coords> ThreeCoords = new List<coords>();
            for (int x = 3; x < LineCoord.Count; x += 7)
            {
                if (ThreeCoords.Count == 3)
                    break;
                if (LineCoord.Count > x)
                {
                    ThreeCoords.Add(LineCoord[x]);
                }
            }
            return ThreeCoords;
        }
    }
}
