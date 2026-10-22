using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms
{
    public class RandomFourLayTraps
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
         

        public List<coords> Coords = new List<coords>();


        public RandomFourLayTraps(ushort x, ushort y)
        {

            for (int i = 0; i < 4; i++)
            {

                ushort n_x = x;
                ushort n_y = y;
                int rand = Pool.GetRandom.Next() % 10;
                int remove = 0;
                if (rand < 3)
                    remove = 1;
                UpdatePosition((Role.Flags.ConquerAngle)((i * 2) + remove), (sbyte)Pool.GetRandom.Next(3, 10), ref n_x, ref n_y);
                Coords.Add(new coords(n_x, n_y));
            }
        }

        public static void UpdatePosition(Role.Flags.ConquerAngle Facing, sbyte count, ref ushort x, ref ushort y)
        {
            int xi = 0;
            int yi = 0;
            xi = yi = 0;
            switch (Facing)
            {
                case Role.Flags.ConquerAngle.North: xi -= count; yi -= count; break;
                case Role.Flags.ConquerAngle.South: xi = count; yi = count; break;
                case Role.Flags.ConquerAngle.East: xi = count; yi -= count; break;
                case Role.Flags.ConquerAngle.West: xi -= count; yi = count; break;
                case Role.Flags.ConquerAngle.NorthWest: xi -= count; break;
                case Role.Flags.ConquerAngle.SouthWest: yi = count; break;
                case Role.Flags.ConquerAngle.NorthEast: yi -= count; break;
                case Role.Flags.ConquerAngle.SouthEast: xi = count; break;
            }
            x = (ushort)(x + xi);
            y = (ushort)(y + yi);
        }
    }
}
