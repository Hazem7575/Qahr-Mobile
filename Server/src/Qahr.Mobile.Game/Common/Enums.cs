using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile
{
    public class Enums
    {
        [System.Flags]
        public enum EonspiritItem : ushort
        {
            Null = 0,
            ConquerorXiangYu = 1,
            HuntressArtemis = 2,
            WarmasterLadyEmpyrean = 3,
        }
        [System.Flags]
        public enum EonspiritPosition : ushort
        {
            Inventory = 0,
            EonspiritInventory = 221,
            EonspiritActive = 222,
            EonspiritUnActive = 223,
        }
        [System.Flags]
        public enum DataType : uint
        {
            ArchiveSkill = 120,
        }
        [System.Flags]
        public enum Flag : int
        {
            Whirlwind = 458,
            DivineArrival = 464,
            SwordShot = 465,
            EchoingSwords = 466,
            ImmortalDestroyer = 469,
            DestructivePower = 470,
            ManiacDance = 471,
            SunsetShine = 473,
            SuperFlash = 473,
            ArcherNormalATK = 474,
            EonspiritCurrentEnergy = 477,
            SwordBody = 478,
            AmazingSpeedActive = 479,//بتشتغل لما الاسكلة تتفعل باسيف
            AmazingSpeed = 480,//بتاخد قيم اللير التالت
            BenefitShower = 481,
            CityRazing = 482,
        }
    }
}
