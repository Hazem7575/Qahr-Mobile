using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role.Instance
{
    public class PerfectionEffect
    {
        public int ToxinEraser, LightOfStamina, DivineGuard, CalmWind, LuckyStrike, StrikeLock, FreeSoul, StraightLife, DrainingTouch, BloodSpawn, CoreStrike, KillingFlash, MirrorOfSin, InvisibleArrow, ShieldBreak, AbsoluteLuck;
        public int PhysicalAttack, PhysicalDefence, MagicAttack, MagicDefense, Parry;

        public void Update(int perfectionLevel, Client.GameClient client)
        {
            ToxinEraser = LightOfStamina = DivineGuard = CalmWind = LuckyStrike = StrikeLock = FreeSoul = StraightLife = DrainingTouch = BloodSpawn = CoreStrike = KillingFlash = MirrorOfSin = InvisibleArrow = ShieldBreak = AbsoluteLuck = 0;
            PhysicalAttack = PhysicalDefence = MagicAttack = MagicDefense = Parry;
            if (perfectionLevel > 0 && perfectionLevel < 649)
            {
                var db = Database.ItemRefineUpgrade.Effects[(ushort)perfectionLevel];
                PhysicalAttack = db.PAttack;
                PhysicalDefence = db.PDefense;
                MagicAttack = db.MAttack;
                MagicDefense = db.MDefense;
                foreach (var effect in db.Effects)
                {
                    byte ID = (byte)(effect / 1000);
                    byte Level = (byte)(effect % 1000);
                    switch (ID)
                    {
                        case 1: ToxinEraser = (int)(25d + Level * 7.5d); break;
                        case 2: Parry = Level * 100; StrikeLock = Level; break;
                        case 3: LuckyStrike = Level * 100; break;
                        case 4: CalmWind = Level; break;
                        case 5: DrainingTouch = Level; break;
                        case 6: BloodSpawn = Level; break;
                        case 7: LightOfStamina = Level; break;
                        case 8: ShieldBreak = Level; break;
                        case 9: KillingFlash = Level; break;
                        case 10: MirrorOfSin = Level; break;
                        case 11: DivineGuard = Level; break;
                        case 12: CoreStrike = Level; break;
                        case 13: InvisibleArrow = Level; break;
                        case 14: FreeSoul = Level; break;
                        case 15: StraightLife = Level; break;
                        case 16: AbsoluteLuck = Level; break;
                    }
                }
            }

            #region Runes
            byte itemLevel = 0;
            if (client.Rune.IsEquipped("Sanctity", ref itemLevel))
                DivineGuard += itemLevel;
            if (client.Rune.IsEquipped("FireBlast", ref itemLevel))
                CoreStrike += itemLevel;
            if (client.Rune.IsEquipped("Serenity", ref itemLevel))
                InvisibleArrow += itemLevel;
            if (client.Rune.IsEquipped("DrainingTouchBooster", ref itemLevel))
                DrainingTouch += itemLevel;
            if (client.Rune.IsEquipped("BloodSpawnBooster", ref itemLevel))
                BloodSpawn += itemLevel;
            if (client.Rune.IsEquipped("FreeSoulBooster", ref itemLevel))
                FreeSoul += itemLevel;
            if (client.Rune.IsEquipped("EnormousCrusher", ref itemLevel))
                ShieldBreak += itemLevel;
            #endregion
        }
    }
}
