using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class HouseTable
    {
        

        internal static int CountFurnitures(byte Level)
        {
            switch (Level)
            {
                case 2: return 8;
                case 3: return 9;
                case 4: return 10;
                case 5: return 12;
                case 6: return 20;
            }
            return 0;
        }

        internal static bool InHouse(uint MapID)
        {
            return MapID == 1098 || MapID == 1099 || MapID == 2080 || MapID == 601 || MapID == 3024 || MapID == 3995;
        }
        internal static ushort GetHouseId(byte level)
        {
            if (level == 1)
                return 1098;
            else if (level == 2)
                return 1099;
            else if (level == 3)
                return 2080;
            else if (level == 4)
                return 601;
            else if (level == 5)
                return 3024;
            else if (level == 6)
                return 3995;
            return 0;
        }
    }
}
