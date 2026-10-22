using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms
{
    public class MoveCoords
    {
        public static bool InRange(ushort X, ushort Y, byte Range, List<InLineAlgorithm.coords> bas)
        {
            foreach (InLineAlgorithm.coords line in bas)
            {
                byte distance = (byte)InLineAlgorithm.GetDistance((ushort)X, (ushort)Y, (ushort)line.X, (ushort)line.Y);
                if (distance <= Range)
                    return true;
            }
            return false;
        }
        public static List<InLineAlgorithm.coords> CheckBladeTeampsCoords(ushort X, ushort Y, ushort XX, ushort YY, Role.GameMap map, byte distance)
        {
            List<InLineAlgorithm.coords> lis = new List<InLineAlgorithm.coords>();

            int dx = XX - X, dy = YY - Y, steps, k;
            float xincrement, yincrement, x = X, y = Y;

            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            lis.Add(new InLineAlgorithm.coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });

            for (k = 0; k < distance; k++)
            {
                x += xincrement;
                y += yincrement;
                if (map != null && map.ValidLocation((ushort)Math.Round(x), (ushort)Math.Round(y)) == false)
                {
                    break;
                }
                lis.Add(new InLineAlgorithm.coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });
            }
            return lis;
        }
        public static List<InLineAlgorithm.coords> CheckBladeTeampsCoords2(ushort X, ushort Y, ushort XX, ushort YY, Role.GameMap map, byte distance)
        {
            List<InLineAlgorithm.coords> lis = new List<InLineAlgorithm.coords>();

            int dx = XX - X, dy = YY - Y, steps, k;
            float xincrement, yincrement, x = X, y = Y;

            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            lis.Add(new InLineAlgorithm.coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });

            for (k = 0; k < distance; k++)
            {
                x += xincrement;
                y += yincrement;
                lis.Add(new InLineAlgorithm.coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });
            }
            return lis;
        }
        public static InLineAlgorithm.coords CheckBombCoords(ushort X, ushort Y, ushort XX, ushort YY, byte adistance, Role.GameMap map)
        {
            List<InLineAlgorithm.coords> lis = new List<InLineAlgorithm.coords>();
            Role.Flags.ConquerAngle angle = InLineAlgorithm.GetAngle(X, Y, XX, YY);
            byte distance = (byte)InLineAlgorithm.GetDistance(X, Y, XX, YY);
            if (distance > 30) return default(InLineAlgorithm.coords);
            int dx = XX - X, dy = YY - Y, steps, k;
            float xincrement, yincrement, x = X, y = Y;
            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);
            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            lis.Add(new InLineAlgorithm.coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });
            InLineAlgorithm.coords coord = new InLineAlgorithm.coords();
            for (k = 0; k < (distance + adistance); k++)
            {
                x += xincrement;
                y += yincrement;
                if ((map.cells[(ushort)x, (ushort)y] & Role.MapFlagType.Valid) != Role.MapFlagType.Valid)
                    break;
                coord.X = (ushort)x;
                coord.Y = (ushort)y;
            }
            return coord;
        }
    }
}
