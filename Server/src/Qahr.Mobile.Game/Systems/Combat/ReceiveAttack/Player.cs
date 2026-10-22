using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Player
    {
        public unsafe static void ExecutePet(int obj, Client.GameClient client, Role.Player attacked)
        {
            if (Calculate.Base.Rate(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;
            if (attacked.HitPoints <= obj)
            {
                attacked.DeadState = true;
                attacked.Dead(client.Player, X, Y, 0);
            }
            else
            {
                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                attacked.HitPoints -= (int)obj;
            }
        }
        public static void Execute(MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.Player attacked)
        {
            client.Player.AttackHit = true;

            if (attacked.ContainFlag(MsgUpdate.Flags.FineRain1))
                attacked.FineRainHit = true;
            MsgYuanshen.Magics(client, attacked, 0);
            MsgYuanshen.Magics(attacked.Owner, client.Player, 1);
            #region ThundercloudSight
            client.Player.ThundercloudSight = attacked.UID;
            attacked.ThundercloudSight = client.Player.UID;
            #endregion
            var SecoundCollection = client.Collection.items.Values.Where(p => p.ID >= 4330000 && p.ID <= 4330002).ToArray();
            if (SecoundCollection.Length >= 1)
            {
                if (Role.MyMath.Success(2))
                {
                    using (var recss = new ServerSockets.RecycledPacket())
                    {
                        var streamss = recss.GetStream();
                        client.Player.AddFlag((MsgUpdate.Flags)442, (int)4, true);
                        client.Player.SendUpdate(streamss, (MsgUpdate.Flags)442, 4, (uint)200, (uint)150, MsgUpdate.DataType.ArchiveSkill);
                    }
                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                }

            }
            var SecoundCollection3 = attacked.Owner.Collection.items.Values.Where(p => p.ID >= 4331000 && p.ID <= 4331002).ToArray();
            if (SecoundCollection3.Length >= 1)
            {
                if (Role.MyMath.Success(2))
                {
                    using (var recss = new ServerSockets.RecycledPacket())
                    {
                        var streamss = recss.GetStream();
                        attacked.Owner.Player.AddFlag((MsgUpdate.Flags)450, (int)3, true);
                        attacked.Owner.Player.SendUpdate(streamss, (MsgUpdate.Flags)450, 3, (uint)30, (uint)30, MsgUpdate.DataType.ArchiveSkill);
                    }
                    attacked.Owner.Equipment.QueryEquipment(client.Equipment.Alternante);
                }

            }

            #region SheathParry >> DuneWanderer
            if (AtributesStatus.IsDune(attacked.Class))
            {
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.SheathParry))
                {
                    if (Role.Core.Rate(20))
                    {
                        obj.Damage /= 2;
                        attacked.AddSpellFlag(MsgUpdate.Flags.SheathParry, 10, true);
                    }
                }
            }
            #endregion

            #region Whetted Blade
            {
                byte itemLevel = 0;
                int chanceIgnoreImmortal = 0;
                if (client.Rune.IsEquipped("Whetted Blade", ref itemLevel) || client.Rune.IsEquipped("Honed Soul", ref itemLevel))
                {
                    switch (itemLevel)
                    {
                        case 1:
                            {
                                chanceIgnoreImmortal = 1;
                                break;
                            }
                        case 2:
                            {
                                chanceIgnoreImmortal = 2;
                                break;
                            }
                        case 3:
                            {
                                chanceIgnoreImmortal = 3;
                                break;
                            }
                        case 4:
                            {
                                chanceIgnoreImmortal = 4;
                                break;
                            }
                        case 5:
                            {
                                chanceIgnoreImmortal = 5;
                                break;
                            }
                        case 6:
                            {
                                chanceIgnoreImmortal = 6;
                                break;
                            }
                        case 7:
                            {
                                chanceIgnoreImmortal = 7;
                                break;
                            }
                        case 8:
                            {
                                chanceIgnoreImmortal = 8;
                                break;
                            }
                        case 9:
                            {
                                chanceIgnoreImmortal = 10;
                                break;
                            }
                    }
                    if (Role.Core.Rate(chanceIgnoreImmortal))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddFlag(MsgUpdate.Flags.DisableBlock, 10, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DisableBlock, (int)10, (uint)10, 0, MsgUpdate.DataType.ArchiveSkill);

                        }
                    }
                }
            }
            #endregion

            #region Runes System

            #region Blue Rune
            #region IronShield
            bool ignoreShield = false;
            byte cleaveLevel = 0;
            byte peakLevel = 0;
            if (client.Rune.IsEquipped("Wild Cleave", ref cleaveLevel) || client.Rune.IsEquipped("Peakcarver", ref peakLevel))
                ignoreShield = true;

            if ((obj.SpellID != (uint)Role.Flags.SpellID.FlameofDestruction))
            {
                if (!ignoreShield && attacked.ContainFlag(MsgUpdate.Flags.IronShield))
                {
                    int rate = 0;
                    #region Crack
                    if (client.Status.Crack > 0)
                    {
                        MythSoulAttributes.Attribute MythInfo;
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Crack].TryGetValue(client.Status.Crack, out MythInfo))
                        {
                            double IncRate = 0;
                            MythSoulAttributes.Attribute SuperPower;
                            if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                            {
                                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                                {
                                    IncRate = (double)SuperPower.Damage / 100;
                                }
                            }
                            double IncRatee = 0;
                            MythSoulAttributes.Attribute Oracle;
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                            {
                                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                                {
                                    IncRatee = (double)Oracle.Damage / 100;
                                }
                            }
                            if (Calculate.Base.Rate((int)MythInfo.Rate + (int)IncRate - (int)IncRatee))
                            {
                                rate = (int)MythInfo.Rate + (int)IncRate - (int)IncRatee;
                            }
                        }
                    }
                    #endregion
                    bool Fusing = false;
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fusing) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Fusing))
                    {
                        var Fusings = Pool.Magic[(ushort)Role.Flags.SpellIDPirate.Fusing][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellIDPirate.Fusing].Level];
                        if (Calculate.Base.Rate(Fusings.GDamage / 100))
                        {
                            Fusing = true;
                        }
                    }
                    if (!Fusing)
                    {

                        if (!Calculate.Base.Rate(rate))
                        {
                            if (attacked.IronShieldDamage >= obj.Damage)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    attacked.IronShieldDamage -= (uint)obj.Damage;
                                    attacked.SendIronShield(stream);
                                    Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(client.Player, attacked, (uint)obj.Damage);
                                    obj.Damage = 0;
                                }

                            }
                            else
                            {
                                if (obj.Damage > attacked.IronShieldDamage)
                                {
                                    obj.Damage -= attacked.IronShieldDamage;
                                    Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(client.Player, attacked, (uint)obj.Damage);
                                }
                                else
                                    obj.Damage = 0;
                                attacked.IronShieldDamage = 0;
                                attacked.RemoveFlag(MsgUpdate.Flags.IronShield);
                            }
                        }
                        else
                        {
                            obj.Effect = MsgAttackPacket.AttackEffect.CrackMyth;
                        }
                    }
                    else
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            attacked.SendIronShield(stream);
                        }
                    }
                }
            }
            #endregion

            #region Caprice
            byte CapriceLevel = 0;
            byte RateCaprice = 0;
            if (client.Rune.IsEquipped("Caprice", ref CapriceLevel))
            {
                byte level = 0;
                int Rate = 0;
                if (attacked.Owner.Rune.IsEquipped("Consolidation", ref level))
                {
                    switch (level)
                    {
                        case 1: Rate = 30; break;
                        case 2: Rate = 35; break;
                        case 3: Rate = 40; break;
                        case 4: Rate = 45; break;
                        case 5: Rate = 55; break;
                        case 6: Rate = 60; break;
                        case 7: Rate = 65; break;
                        case 8: Rate = 70; break;
                        case 9: Rate = 80; break;
                    }
                    if (!Role.Core.Rate(Rate))
                    {
                        switch (CapriceLevel)
                        {
                            case 1: RateCaprice = 5; break;
                            case 2: RateCaprice = 7; break;
                            case 3: RateCaprice = 9; break;
                            case 4: RateCaprice = 11; break;
                            case 5: RateCaprice = 13; break;
                            case 6: RateCaprice = 15; break;
                            case 7: RateCaprice = 17; break;
                            case 8: RateCaprice = 19; break;
                            case 9: RateCaprice = 21; break;
                            case 10: RateCaprice = 23; break;
                            case 11: RateCaprice = 25; break;
                            case 12: RateCaprice = 27; break;
                            case 13: RateCaprice = 31; break;
                            case 14: RateCaprice = 33; break;
                            case 15: RateCaprice = 35; break;
                            case 16: RateCaprice = 35; break;
                            case 17: RateCaprice = 35; break;
                            case 18: RateCaprice = 35; break;
                            case 19: RateCaprice = 35; break;
                            case 20: RateCaprice = 35; break;
                            case 21: RateCaprice = 35; break;
                            case 22: RateCaprice = 35; break;
                            case 23: RateCaprice = 35; break;
                            case 24: RateCaprice = 38; break;
                            case 25: RateCaprice = 40; break;
                            case 26: RateCaprice = 42; break;
                            case 27: RateCaprice = 45; break;

                        }
                        if (Role.Core.Rate(RateCaprice))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (attacked.Stamina >= 20)
                                {
                                    attacked.Stamina -= 20;
                                    attacked.SendUpdate(stream, attacked.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);

                                }
                            }
                            byte Caprice = (byte)Pool.GetRandom.Next(1, 3);
                            switch (Caprice)
                            {
                                case 1:
                                    {
                                        if (attacked.OnXPSkill() != MsgUpdate.Flags.Normal)
                                            attacked.RemoveFlag(attacked.OnXPSkill());
                                        attacked.RemoveFlag(MsgUpdate.Flags.XPList);
                                        attacked.XPCount = 0;
                                        break;
                                    }
                                case 2:
                                    {
                                        var List = new List<MsgSpell>();
                                        foreach (var Spells in attacked.Owner.MySpells.ClientSpells.Values)
                                        {
                                            if (Spells.IsSpellWithColdTime)
                                            {
                                                Spells.ColdTime = Spells.ColdTime.AddSeconds(1);
                                                if (Spells.GetColdTime > 0)
                                                    List.Add(Spells);
                                            }
                                        }
                                        if (List.Count > 0)
                                        {
                                            var Send = new MsgMagicColdTime.MagicColdTime();
                                            Send.WriteSpells(List);
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                attacked.Send(stream.MagicColdTimeCreate(Send));
                                            }
                                        }
                                        break;
                                    }
                            }




                        }
                    }
                }
            }
            #endregion

            #region Absolution
            client.Player.RemoveFlag(MsgUpdate.Flags.Absolution);
            attacked.RemoveFlag(MsgUpdate.Flags.Absolution);
            #endregion

            #region Neptune'sCurse
            if (attacked.ContainFlag(MsgUpdate.Flags.NeptuneCurse)) obj.Damage = 0;
            else if (client.Player.ContainFlag(MsgUpdate.Flags.NeptuneCurse))
            {
                if (obj.Damage < attacked.HitPoints)
                    obj.Damage /= 2;
            }
            #endregion

            #region BloodTide
            if (!client.Player.ContainFlag(MsgUpdate.Flags.BloodTide))
            {
                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodTide))
                {
                    var BloodTide = Pool.Magic[(ushort)Role.Flags.SpellID.BloodTide][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BloodTide].Level];
                    if (Role.Core.Rate(BloodTide.Rate) && Time32.Now >= client.Player.BloodTideStamp.AddMilliseconds(BloodTide.ColdTime))
                    {
                        client.Player.BloodTideCaptured += (uint)obj.Damage;
                        if (client.Player.BloodTideCaptured >= 35000)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(stream.ActionCreate(new ActionQuery()
                                {
                                    ObjId = client.Player.UID,
                                    dwParam = BloodTide.ID,
                                    Type = 450,
                                    dwParam3 =
                                        client.Player.BloodTideCaptured
                                }));
                            }

                        }

                    }
                }
            }
            #endregion
            #endregion

            #region YellowRune

            #region XPKiller
            if (client.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
            {
                byte level = 0;
                if (client.Rune.IsEquipped("XPKiller", ref level) || client.Rune.IsEquipped("XP Devourer", ref level))
                {
                    if (attacked.XPKiller < level)
                    {
                        attacked.IncreaseXPDuration(-1);
                        attacked.XPKiller++;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            attacked.SendUpdate(stream, (MsgUpdate.Flags)0, 0, 0, 0, MsgUpdate.DataType.XPDuration);
                        }
                    }
                }
            }
            #endregion

            #region XPBooster
            if (client.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
            {
                byte level = 0;
                if (client.Rune.IsEquipped("XPBooster", ref level))
                {
                    if (client.Player.XPBooster > 0)
                    {
                        client.Player.IncreaseXPDuration(1);
                        client.Player.XPBooster--;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, (MsgUpdate.Flags)0, (uint)(client.Player.XPBooster == 0 ? 1 : 0), 0, 1, MsgUpdate.DataType.XPDuration);
                        }
                    }
                }
            }
            #endregion

            #region TidalWave
            if (!client.Player.ContainFlag(MsgUpdate.Flags.TidalWave) && !client.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
            {
                byte level = 0;
                if (client.Rune.IsEquipped("TidalWave", ref level) && Time32.Now >= client.Player.TidalWaveStamp.AddSeconds(60))//Cooldown
                {
                    level = (byte)(15 + level * 5);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.TidalWave, level, true, 5);
                    client.Player.TidalWaveStamp = Time32.Now;
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        attacked.SendUpdate(streamm, MsgUpdate.Flags.TidalWave, level, 0, 0, MsgUpdate.DataType.TidalWave);
                    }
                }
            }
            #endregion

            #region Quench
            if (!attacked.ContainFlag(MsgUpdate.Flags.Quench))
            {
                byte level = 0;
                if (client.Rune.IsEquipped("Quench", ref level) && Time32.Now >= client.Player.QuenchStamp.AddSeconds(60))//Cooldown
                {
                    level = (byte)(15 + level * 5);
                    client.Player.QuenchStamp = Time32.Now;
                    attacked.AddSpellFlag(MsgUpdate.Flags.Quench, level, true, 5);
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        attacked.SendUpdate(streamm, MsgUpdate.Flags.Quench, level, 0, 0, MsgUpdate.DataType.Quench);
                    }
                }
            }
            #endregion
            //#region ReverseMagic
            //byte ReverseMagicLevel = 0;
            //int baseChance2 = 0;
            //float percent = 0f;
            //if (attacked.Owner.Rune.IsEquipped("ReverseMagic", ref ReverseMagicLevel))
            //{
            //    switch (ReverseMagicLevel)
            //    {
            //        case 1: baseChance2 = 5; percent = 0.5f; break;
            //        case 2: baseChance2 = 10; percent = 1f; break;
            //        case 3: baseChance2 = 15; percent = 1.5f; break;
            //        case 4: baseChance2 = 20; percent = 2f; break;
            //        case 5: baseChance2 = 25; percent = 2.5f; break;
            //        case 6: baseChance2 = 30; percent = 3f; break;
            //        case 7: baseChance2 = 35; percent = 3.5f; break;
            //        case 8: baseChance2 = 40; percent = 4f; break;
            //        case 9: baseChance2 = 50; percent = 5f; break;
            //    }
            //    if (Role.Core.Rate(5))
            //    {
            //        client.Player.XPCount = 0;

            //        var xpFlag = client.Player.OnXPSkill();
            //        if (xpFlag != MsgUpdate.Flags.Normal)
            //            client.Player.RemoveFlag(xpFlag);

            //        client.Player.Owner.SendSysMesage(string.Format(
            //            "Reverse Magic activated (Lv.{0}) - {1}% chance! XP cleared!",
            //            ReverseMagicLevel,
            //            percent));

            //        using (var rec = new ServerSockets.RecycledPacket())
            //        {
            //            var stream = rec.GetStream();
            //            client.Player.SendUpdate(stream, 0, MsgUpdate.DataType.XPCircle);
            //        }
            //    }
            //}
            //#endregion

            #region Unswerving Rare Yellow Rune

            byte unswervingLevel = 0;

            if (attacked.Owner.Rune.IsEquipped("Unswerving", ref unswervingLevel))
            {
                int reducePercent = 0;

                switch (unswervingLevel)
                {
                    case 1: reducePercent = 10; break;
                    case 2: reducePercent = 20; break;
                    case 3: reducePercent = 30; break;
                    case 4: reducePercent = 40; break;
                    case 5: reducePercent = 50; break;
                    case 6: reducePercent = 60; break;
                    case 7: reducePercent = 70; break;
                    case 8: reducePercent = 80; break;
                    case 9: reducePercent = 100; break;
                }

                attacked.Owner.SendSysMesage(string.Format(
                    "Unswerving activated (Lv.{0}) - Reduced Unswerving by {1}%!",
                    unswervingLevel,
                    reducePercent));
            }

            #endregion

            #endregion

            #endregion

            #region Spotless Rune
            if (client != null &&
                client.Rune != null)
            {
                byte Level = 0;

                if (client.Rune.IsEquipped("Spotless", ref Level))
                {
                    int chance = 0;
                    double baseChance = 0;

                    switch (Level)
                    {
                        case 1: chance = 5; baseChance = 0.5; break;    // Level ( 1 )
                        case 2: chance = 7; baseChance = 1; break;      // Level ( 2 )
                        case 3: chance = 10; baseChance = 1.5; break;   // Level ( 3 )
                        case 4: chance = 12; baseChance = 2; break;     // Level ( 4 )
                        case 5: chance = 14; baseChance = 2.5; break;   // Level ( 5 )
                        case 6: chance = 16; baseChance = 3; break;     // Level ( 6 )
                        case 7: chance = 18; baseChance = 3.5; break;   // Level ( 7 )
                        case 8: chance = 20; baseChance = 4; break;     // Level ( 8 )
                        case 9: chance = 25; baseChance = 5; break;     // Level ( 9 )
                    }

                    if (Role.Core.Rate(chance))
                    {
                        if (client != null)
                        {
                            attacked.RemoveFlag(MsgUpdate.Flags.Freeze);
                            attacked.RemoveFlag(MsgUpdate.Flags.Stun);

                            client.SendSysMesage(string.Format(
    "You activated Spotless (Lv.{0}) - {1}% chance: Negative effects removed!",
    Level,
    baseChance));
                        }
                    }
                }
            }
            #endregion            

            // #region WhettedBlade (improved activation rates and damage boost)
#region WhettedBlade (improved activation rates and damage boost)
            byte RateBlade = 0;
            byte WhettedBlade = 0;
            if (client.Rune.IsEquipped("Whetted Blade", ref WhettedBlade) || client.Rune.IsEquipped("Honed Soul", ref WhettedBlade))
            {
                // Lower Rate value → higher chance. Higher rune level gives lower Rate.
                switch (WhettedBlade)
                {
                    case 1: RateBlade = 10; break; // 10% chance
                    case 2: RateBlade = 8; break;  // 12.5% chance
                    case 3: RateBlade = 6; break;  // 16.7% chance
                    case 4: RateBlade = 4; break;  // 25% chance
                    case 5: RateBlade = 3; break;  // 33.3% chance
                    case 6: RateBlade = 2; break;  // 50% chance
                    case 7: RateBlade = 1; break;  // 100% chance
                    case 8: RateBlade = 1; break;
                    case 9: RateBlade = 1; break;
                }
                if (Role.Core.Rate(RateBlade))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.DisableBlock))
                        {
                            client.Player.AddFlag(MsgUpdate.Flags.DisableBlock, 10, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DisableBlock, 10, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                        }
                    }
                    // Increase damage by 50% when rune activates
                    obj.Damage = (uint)(obj.Damage * 1.5);
                }
            }
            #endregion
// #endregion


            #region Shackle

            byte ShackleL = 0;
            byte RateShackle = 0;
            if (client.Rune.IsEquipped("Shackle", ref ShackleL) && attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Ride))
            {
                switch (ShackleL)
                {
                    case 1: RateShackle = 10; break;
                    case 2: RateShackle = 20; break;
                    case 3: RateShackle = 30; break;
                    case 4: RateShackle = 40; break;
                    case 5: RateShackle = 50; break;
                    case 6: RateShackle = 60; break;
                    case 7: RateShackle = 70; break;
                    case 8: RateShackle = 80; break;
                    case 9: RateShackle = 100; break;
                }
                if (Role.Core.Rate(RateShackle))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.RuneShackle))
                        {
                            attacked.Owner.Player.AddFlag(MsgUpdate.Flags.RuneShackle, 12, true);
                        }
                    }

                }
            }
            #endregion

            //#region StealthDragon
            //byte RateStealthDragon = 0;
            //byte StealthDragon = 0;
            //if (client.Rune.IsEquipped("Stealth Dragon", ref StealthDragon))
            //{
            //    switch (StealthDragon)
            //    {
            //        case 1: RateStealthDragon = 10; break;
            //        case 2: RateStealthDragon = 13; break;
            //        case 3: RateStealthDragon = 16; break;
            //        case 4: RateStealthDragon = 19; break;
            //        case 5: RateStealthDragon = 22; break;
            //        case 6: RateStealthDragon = 25; break;
            //        case 7: RateStealthDragon = 28; break;
            //        case 8: RateStealthDragon = 31; break;
            //        case 9: RateStealthDragon = 35; break;
            //    }
            //    if (attacked.Owner.Player.BattlePower >= client.Player.BattlePower)
            //    {
            //        int battlerpower = (attacked.Owner.Player.BattlePower - client.Player.BattlePower) * 7;
            //        if (battlerpower < 35)
            //        {

            //            int Rate = RateStealthDragon - battlerpower;
            //            if (Calculate.Base.Rate((int)RateStealthDragon))
            //            {
            //                if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ChaoticDance))
            //                    attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.ChaoticDance);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Calculate.Base.Rate((int)RateStealthDragon))
            //        {
            //            if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ChaoticDance))
            //                attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.ChaoticDance);
            //        }
            //    }
            //}
            //#endregion

            #region CelestialRope
            byte RateCelestialRope = 0;
            byte CelestialRope = 0;
            if (client.Rune.IsEquipped("Celestial Rope", ref CelestialRope))
            {
                switch (CelestialRope)
                {
                    case 1: RateCelestialRope = 15; break;
                    case 2: RateCelestialRope = 20; break;
                    case 3: RateCelestialRope = 25; break;
                    case 4: RateCelestialRope = 30; break;
                    case 5: RateCelestialRope = 35; break;
                    case 6: RateCelestialRope = 40; break;
                    case 7: RateCelestialRope = 45; break;
                    case 8: RateCelestialRope = 50; break;
                    case 9: RateCelestialRope = 55; break;
                }
                if (attacked.Owner.Player.BattlePower >= client.Player.BattlePower)
                {
                    int battlerpower = (attacked.Owner.Player.BattlePower - client.Player.BattlePower) * 10;
                    if (battlerpower < 55)
                    {

                        int Rate = RateCelestialRope - battlerpower;
                        if (Calculate.Base.Rate((int)RateCelestialRope))
                        {
                            if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CelestialDance))
                                attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.CelestialDance);
                        }
                    }
                }
                else
                {
                    if (Calculate.Base.Rate((int)RateCelestialRope))
                    {
                        if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CelestialDance))
                            attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.CelestialDance);
                    }
                }
            }
            #endregion

            #region Seethrough
            double RateSeethrough = 0;
            byte Seethrough = 0;
            if (attacked.Owner.Rune.IsEquipped("Seethrough", ref Seethrough) || attacked.Owner.Rune.IsEquipped("Shadow Flow", ref Seethrough))
            {
                switch (Seethrough)
                {
                    case 1: RateSeethrough = 8; break;
                    case 2: RateSeethrough = 10; break;
                    case 3: RateSeethrough = 12; break;
                    case 4: RateSeethrough = 15; break;
                    case 5: RateSeethrough = 18; break;
                    case 6: RateSeethrough = 20; break;
                    case 7: RateSeethrough = 25; break;
                    case 8: RateSeethrough = 30; break;
                    case 9: RateSeethrough = 35; break;
                }

                if (Calculate.Base.Rate((int)RateSeethrough))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.Owner.Player.AddFlag((MsgUpdate.Flags)439, (int)10, true);
                        attacked.Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)439, (uint)10, (uint)0, (uint)0, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion
            #region TimeReversal
            double RateTimeReversal = 0;
            byte TimeReversal = 0;
            if (client.Rune.IsEquipped("Time Reversal", ref TimeReversal))
            {
                switch (TimeReversal)
                {
                    case 1: RateTimeReversal = 1; break;
                    case 2: RateTimeReversal = 1.1; break;
                    case 3: RateTimeReversal = 1.2; break;
                    case 4: RateTimeReversal = 1.3; break;
                    case 5: RateTimeReversal = 1.5; break;
                    case 6: RateTimeReversal = 2; break;
                    case 7: RateTimeReversal = 2.5; break;
                    case 8: RateTimeReversal = 3; break;
                    case 9: RateTimeReversal = 5; break;
                }
                if (attacked.Owner.Player.BattlePower >= client.Player.BattlePower)
                {
                    int battlerpower = (attacked.Owner.Player.BattlePower - client.Player.BattlePower);
                    if (battlerpower < 5)
                    {

                        int Rate = (int)RateTimeReversal - battlerpower;
                        if (Calculate.Base.Rate((int)RateTimeReversal))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                attacked.Owner.Player.AddFlag((MsgUpdate.Flags)488, (int)3, true);
                                attacked.Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)488, (uint)3, (uint)300, (uint)300, MsgUpdate.DataType.ArchiveSkill);
                            }
                        }
                    }
                }
                else
                {
                    if (Calculate.Base.Rate((int)RateTimeReversal))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            attacked.Owner.Player.AddFlag((MsgUpdate.Flags)488, (int)3, true);
                            attacked.Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)488, (uint)3, (uint)300, (uint)300, MsgUpdate.DataType.ArchiveSkill);
                        }
                    }
                }
            }
            #endregion

            #region AreaOccupier

            if (attacked.Owner.Player.ContainFlag((MsgUpdate.Flags)491))
            {
                ushort DMG = 0;
                attacked.Owner.Player.AreaOccupier++;
                switch (attacked.Owner.Player.AreaOccupier)
                {
                    case 1:
                        {
                            DMG = 5000;
                            break;
                        }
                    case 2:
                        {
                            DMG = 12000;
                            break;
                        }
                    case 3:
                        {
                            DMG = 25000;
                            break;
                        }
                }
                obj.Damage += DMG;
            }
            double RateAreaOccupier = 0;
            byte AreaOccupier = 0;
            if (client.Rune.IsEquipped("Area Occupier", ref AreaOccupier) || client.Rune.IsEquipped("Deathhold", ref AreaOccupier))
            {
                switch (AreaOccupier)
                {
                    case 1: RateAreaOccupier = 20; break;
                    case 2: RateAreaOccupier = 25; break;
                    case 3: RateAreaOccupier = 30; break;
                    case 4: RateAreaOccupier = 35; break;
                    case 5: RateAreaOccupier = 40; break;
                    case 6: RateAreaOccupier = 45; break;
                    case 7: RateAreaOccupier = 50; break;
                    case 8: RateAreaOccupier = 55; break;
                    case 9: RateAreaOccupier = 60; break;
                }
                if (Calculate.Base.Rate((int)RateAreaOccupier))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.Owner.Player.AddFlag((MsgUpdate.Flags)491, (int)5, true);
                        attacked.Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)491, (uint)5, (uint)0, (uint)0, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion
            #region HurricaneSweep
            byte RateHurricaneSweep = 0;
            ushort RateSPEED = 0;
            byte HurricaneSweep = 0;
            if (client.Rune.IsEquipped("Hurricane Sweep", ref HurricaneSweep))
            {
                switch (HurricaneSweep)
                {
                    case 1: RateHurricaneSweep = 10; break;
                    case 2: RateHurricaneSweep = 11; break;
                    case 3: RateHurricaneSweep = 12; break;
                    case 4: RateHurricaneSweep = 13; break;
                    case 5: RateHurricaneSweep = 14; break;
                    case 6: RateHurricaneSweep = 15; break;
                    case 7: RateHurricaneSweep = 16; break;
                    case 8: RateHurricaneSweep = 18; break;
                    case 9: RateHurricaneSweep = 20; break;
                }
                switch (HurricaneSweep)
                {
                    case 1: RateSPEED = 6; break;
                    case 2: RateSPEED = 9; break;
                    case 3: RateSPEED = 12; break;
                    case 4: RateSPEED = 15; break;
                    case 5: RateSPEED = 18; break;
                    case 6: RateSPEED = 21; break;
                    case 7: RateSPEED = 24; break;
                    case 8: RateSPEED = 27; break;
                    case 9: RateSPEED = 30; break;
                }
                //speed by 6% for 5 seconds
                if (attacked.Owner.Player.BattlePower >= client.Player.BattlePower)
                {
                    int battlerpower = (attacked.Owner.Player.BattlePower - client.Player.BattlePower) * 10;
                    if (battlerpower < 20)
                    {

                        int Rate = RateHurricaneSweep - battlerpower;
                        if (Calculate.Base.Rate((int)RateHurricaneSweep))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Cyclone)
                || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ManiacDance)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.FatalStrike)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Ride)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CannonBarrage)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.AmazingSpeed)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Oblivion)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.BladeFlurry)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Accelerated)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CelestialDance)
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.SageMode)//273
               || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Immersion)//303
                || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Sense)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Armed)
                  || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Dark)
                         || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Sailing)
               || attacked.Owner.Player.OnTransform)
                                {
                                    attacked.Owner.Player.AddFlag(MsgUpdate.Flags.HurricaneSweep, (int)5, true);
                                    attacked.Owner.Player.SendUpdate(stream, MsgUpdate.Flags.HurricaneSweep, 5, (uint)RateSPEED, (uint)CelestialRope, MsgUpdate.DataType.ArchiveSkill);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Calculate.Base.Rate((int)RateHurricaneSweep))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Cyclone)
                  || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ManiacDance)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.FatalStrike)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Ride)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CannonBarrage)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.AmazingSpeed)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Oblivion)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.BladeFlurry)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Accelerated)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.CelestialDance)
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.SageMode)//273
                 || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Immersion)//303
                  || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Sense)
                   || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Armed)
                    || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Dark)
                           || attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Sailing)
                 || attacked.Owner.Player.OnTransform)
                            {
                                attacked.Owner.Player.AddFlag(MsgUpdate.Flags.HurricaneSweep, (int)5, true);
                                attacked.Owner.Player.SendUpdate(stream, MsgUpdate.Flags.HurricaneSweep, 5, (uint)RateSPEED, (uint)CelestialRope, MsgUpdate.DataType.ArchiveSkill);
                            }
                        }
                    }
                }
            }
            #endregion

            #region SkySoarer
            byte RateSkySoarer = 0;
            ushort RateHawkeye = 0;
            byte SkySoarer = 0;
            if (client.Rune.IsEquipped("Sky Soarer", ref SkySoarer))
            {
                switch (SkySoarer)
                {
                    case 1: RateSkySoarer = 5; break;
                    case 2: RateSkySoarer = 10; break;
                    case 3: RateSkySoarer = 15; break;
                    case 4: RateSkySoarer = 20; break;
                    case 5: RateSkySoarer = 25; break;
                    case 6: RateSkySoarer = 30; break;
                    case 7: RateSkySoarer = 35; break;
                    case 8: RateSkySoarer = 40; break;
                    case 9: RateSkySoarer = 50; break;
                }
                switch (SkySoarer)
                {
                    case 1: RateHawkeye = 100; break;
                    case 2: RateHawkeye = 200; break;
                    case 3: RateHawkeye = 300; break;
                    case 4: RateHawkeye = 400; break;
                    case 5: RateHawkeye = 500; break;
                    case 6: RateHawkeye = 800; break;
                    case 7: RateHawkeye = 1000; break;
                    case 8: RateHawkeye = 1200; break;
                    case 9: RateHawkeye = 1500; break;
                }
                if (Calculate.Base.Rate((int)RateSkySoarer))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.AddFlag(MsgUpdate.Flags.SkySoarer, (int)10, true);
                        client.Player.SendUpdate(stream, MsgUpdate.Flags.SkySoarer, 10, (uint)RateHawkeye, (uint)SkySoarer, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion

            #region MythSouls

            #region OldMythSoul

            #region Venom
            if (client.Status.VenomLevel > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Venom].TryGetValue(client.Status.VenomLevel, out MythInfo))
                {
                    MsgSpell user_spell = null;
                    double IncRate = 0;
                    MythSoulAttributes.Attribute SuperPower;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    MythSoulAttributes.Attribute Oracle;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                        {
                            IncRatee = (double)Oracle.Damage / 100;
                        }
                    }
                    if (Role.Core.Rate(MythInfo.Rate + IncRate - IncRatee))
                    {
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Venom, out user_spell))
                        {
                            var InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = user_spell.ID;
                            InteractQuery.SpellLevel = user_spell.Level;
                            InteractQuery.X = attacked.X;
                            InteractQuery.Y = attacked.Y;
                            InteractQuery.OpponentUID = attacked.UID;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                MsgServer.MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Bloodthirst
            if (client.Status.BloodthirstLevel > 0)
            {
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    MythSoulAttributes.Attribute MythInfo;
                    if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Bloodthirst].TryGetValue(client.Status.BloodthirstLevel, out MythInfo))
                    {
                        double IncRate = 0;
                        MythSoulAttributes.Attribute SuperPower;
                        if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                        {
                            if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                            {
                                IncRate = (double)SuperPower.Damage / 100;
                            }
                        }
                        double IncRatee = 0;
                        MythSoulAttributes.Attribute Oracle;
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                        {
                            if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                            {
                                IncRatee = (double)Oracle.Damage / 100;
                            }
                        }
                        if (Role.Core.Rate(MythInfo.Rate + IncRate - IncRatee))
                        {

                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            if (client.Player.HitPoints > client.Status.MaxHitpoints)
                                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                            client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                            using (var r = new ServerSockets.RecycledPacket())
                            {
                                var m = r.GetStream();
                                {
                                    client.Player.SendUpdate(m, (int)client.Status.MaxHitpoints, MsgUpdate.DataType.Bloodthirst_Elan);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Elan
            if (attacked.Owner.Status.ElanLevel > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Elan].TryGetValue(client.Status.ElanLevel, out MythInfo))
                {
                    double IncRate = 0; MythSoulAttributes.Attribute SuperPower;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    MythSoulAttributes.Attribute Oracle;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                        {
                            IncRatee = (double)Oracle.Damage / 100;
                        }
                    }
                    if (Role.Core.Rate(MythInfo.Rate + IncRate - IncRatee))
                    {
                        uint ManaInc = 0;
                        ManaInc += (uint)(attacked.Mana * MythInfo.Damage / 100);
                        if (attacked.Owner.Status.MaxMana <= ManaInc + attacked.Mana)
                        {
                            attacked.Mana = (ushort)attacked.Owner.Status.MaxMana;
                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var streamm = recycledPacket.GetStream();
                                attacked.SendUpdate(streamm, attacked.Owner.Status.MaxMana, MsgUpdate.DataType.Bloodthirst_Elan);
                            }
                        }
                        else
                        {
                            attacked.Mana += (ushort)ManaInc;
                        }
                    }
                }
            }
            #endregion

            #region Solid
            if (client.Status.SolidLevel > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Solid].TryGetValue(client.Status.SolidLevel, out MythInfo))
                {
                    double IncRate = 0;
                    MythSoulAttributes.Attribute SuperPower;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    MythSoulAttributes.Attribute Oracle;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                        {
                            IncRatee = (double)Oracle.Damage / 100;
                        }
                    }
                    if (MythInfo != null)
                    {
                        if (Role.Core.Rate(MythInfo.Rate + IncRate - IncRatee))
                        {

                            if (!client.Player.ContainFlag(MsgUpdate.Flags.Solid))
                            {
                                client.Player.AddFlag(MsgUpdate.Flags.Solid, (int)MythInfo.Seconds, true); using (var recycledPacket = new ServerSockets.RecycledPacket())
                                {
                                    var streamm = recycledPacket.GetStream();
                                    client.Player.SendUpdate(streamm, MsgUpdate.Flags.Solid, MythInfo.Seconds, (uint)MythInfo.Damage, 0, MsgUpdate.DataType.ArchiveSkill);
                                    client.Status.Defence += (uint)MythInfo.Damage;

                                }
                            }
                        }
                    }

                }
            }
            #endregion

            #region Meditation
            if (client.Status.MeditationLevel > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Meditation].TryGetValue(client.Status.MeditationLevel, out MythInfo))
                {
                    if (Role.Core.Rate(MythInfo.Rate))
                    {
                        if (attacked.ContainFlag(MsgUpdate.Flags.Stigma))
                            attacked.RemoveFlag(MsgUpdate.Flags.Stigma);
                        if (attacked.ContainFlag(MsgUpdate.Flags.Shield))
                            attacked.RemoveFlag(MsgUpdate.Flags.Shield);
                        if (attacked.ContainFlag(MsgUpdate.Flags.AzureShield))
                            attacked.RemoveFlag(MsgUpdate.Flags.AzureShield);
                        if (attacked.ContainFlag(MsgUpdate.Flags.DragonFlow))
                            attacked.RemoveFlag(MsgUpdate.Flags.DragonFlow);
                        if (attacked.ContainFlag(MsgUpdate.Flags.Slayer))
                            attacked.RemoveFlag(MsgUpdate.Flags.Slayer);
                        if (attacked.ContainFlag(MsgUpdate.Flags.ImmortalForce))
                            attacked.RemoveFlag(MsgUpdate.Flags.ImmortalForce);
                        if (attacked.ContainFlag(MsgUpdate.Flags.LightningShieldActivated))
                            attacked.RemoveFlag(MsgUpdate.Flags.LightningShieldActivated);
                        if (attacked.ContainFlag(MsgUpdate.Flags.SparkShield))
                            attacked.RemoveFlag(MsgUpdate.Flags.SparkShield);
                        if (attacked.ContainFlag(MsgUpdate.Flags.FineRain1))
                            attacked.RemoveFlag(MsgUpdate.Flags.FineRain1);
                        if (attacked.ContainFlag(MsgUpdate.Flags.FineRain2))
                            attacked.RemoveFlag(MsgUpdate.Flags.FineRain2);
                        if (attacked.ContainFlag(MsgUpdate.Flags.Rampage))
                            attacked.RemoveFlag(MsgUpdate.Flags.Rampage);
                        attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante);
                    }
                }
            }
            #endregion

            #endregion

            #region NewMythSoul

            #region Oracle
            if (attacked.Owner.Status.Oracle > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(attacked.Owner.Status.Oracle, out MythInfo))
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddFlag(MsgUpdate.Flags.Oracle, (int)MythInfo.Seconds, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.Oracle, (uint)MythInfo.Seconds, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);
                        }
                    }
                }
            }
            #endregion

            #region Numb
            if (client.Status.Numb > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Numb].TryGetValue(client.Status.Numb, out MythInfo))
                {
                    if (Role.Core.Rate(MythInfo.Rate))
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                attacked.AddFlag(MsgUpdate.Flags.Numb, (int)MythInfo.Seconds, true);
                                attacked.SendUpdate(stream, MsgUpdate.Flags.Numb, (uint)MythInfo.Seconds, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);

                            }
                        }
                    }
                }
            }
            #endregion

            #region Frost
            if (client.Status.Frost > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Frost].TryGetValue(client.Status.Frost, out MythInfo))
                {

                    double RateReduce = 0;
                    if (attacked.BattlePower > client.Player.BattlePower)
                    {
                        RateReduce = attacked.BattlePower - client.Player.BattlePower;
                    }
                    #region Collection

                    if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidShelter))
                    {
                        var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.SolidShelter][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SolidShelter].Level];
                        if (Spell != null)
                            RateReduce += Spell.GDamage / 100;
                    }

                    #endregion

                    if (Role.Core.Rate(MythInfo.Rate - RateReduce)) // Freeze Mythsoul Edit By Mohy ElDin ...
                    {
                        if (Role.Core.Rate(57.4))
                        {

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                //https://co.99.com/news/2022-06-24/mythsoul_mutation.shtml
                                var stream = rec.GetStream();
                                if (!attacked.ContainFlag(MsgUpdate.Flags.FrostStop) && !attacked.ContainFlag(MsgUpdate.Flags.FrostPostion))
                                {
                                    attacked.AddFlag(MsgUpdate.Flags.FrostStop, (int)1, true);
                                    attacked.SendUpdate(stream, MsgUpdate.Flags.FrostStop, (uint)1, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);

                                    attacked.AddFlag(MsgUpdate.Flags.FrostPostion, (int)MythInfo.Damage, true);
                                    attacked.SendUpdate(stream, MsgUpdate.Flags.FrostPostion, (uint)MythInfo.Damage, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Bash
            if (client.Status.Bash > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Bash].TryGetValue(client.Status.Bash, out MythInfo))
                {
                    double IncRate = 0;
                    MythSoulAttributes.Attribute SuperPower;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    MythSoulAttributes.Attribute Oracle;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                        {
                            IncRatee = (double)Oracle.Damage / 100;
                        }
                    }
                    if (Role.Core.Rate(MythInfo.Rate + IncRate - IncRatee))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddFlag(MsgUpdate.Flags.Bash, (int)MythInfo.Seconds, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.Bash, (uint)MythInfo.Seconds, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }

                    }
                }
            }
            #endregion

            #region Luck
            if (client.Status.Luck > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Luck].TryGetValue(client.Status.Luck, out MythInfo))
                {
                    if (Role.Core.Rate(MythInfo.Rate))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddFlag(MsgUpdate.Flags.Luck, (int)MythInfo.Seconds, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.Luck, (uint)MythInfo.Seconds, (uint)MythInfo.Damage, (uint)MythInfo.Level, MsgUpdate.DataType.ArchiveSkill);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                    }
                }
            }
            #endregion

            #endregion
            #endregion

            #region Collection
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastShield))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BeastShield][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BeastShield].Level];

                if (Role.Core.Rate(Spell.Rate))
                {
                    obj.Damage = 1;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = Spell.ID;
                        InteractQuery.SpellLevel = Spell.Level;
                        InteractQuery.X = attacked.X;
                        InteractQuery.Y = attacked.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery);
                    }
                }
            }
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastControl) && !attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.BeastMastery))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BeastControl][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BeastControl].Level];

                if (Role.Core.Rate(Spell.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = Spell.ID;
                        InteractQuery.SpellLevel = Spell.Level;
                        InteractQuery.X = attacked.X;
                        InteractQuery.Y = attacked.Y;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery);
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastControl) && !attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.BeastMastery))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BeastControl][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BeastControl].Level];

                if (Role.Core.Rate(Spell.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = Spell.ID;
                        InteractQuery.SpellLevel = Spell.Level;
                        InteractQuery.X = attacked.X;
                        InteractQuery.Y = attacked.Y;
                        InteractQuery.OpponentUID = client.Player.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery);
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastMastery) && !attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.BeastMastery))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.BeastMastery][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.BeastMastery].Level];

                if (Role.Core.Rate(Spell.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (attacked.AddFlag(MsgUpdate.Flags.BeastMastery, (int)Spell.Duration, true))
                            attacked.SendUpdate(stream, MsgUpdate.Flags.BeastMastery, (uint)Spell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion

            #region Tardiness
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tardiness) && !attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Tardiness))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.Tardiness][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Tardiness].Level];

                if (Role.Core.Rate(Spell.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (attacked.AddFlag(MsgUpdate.Flags.Tardiness, (int)Spell.Damage2 / 10, true))
                            attacked.SendUpdate(stream, MsgUpdate.Flags.Tardiness, (uint)Spell.Damage2 / 10, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion

            #region SupremeLeadership
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SupremeLeadership][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SupremeLeadership;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }

            #endregion

            #region LordThreatPassive
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LordThreatPassive) && client.Player.ContainFlag(MsgUpdate.Flags.Ride))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellIDPirate.LordThreatPassive][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellIDPirate.LordThreatPassive].Level];
                #region Lord Impact
                byte LordImpactL = 0;
                byte Rate = 0;
                if (client.Rune.IsEquipped("Lord Impact", ref LordImpactL))
                {
                    switch (LordImpactL)
                    {
                        case 1: Rate = 1; break;
                        case 2: Rate = 2; break;
                        case 3: Rate = 3; break;
                        case 4: Rate = 4; break;
                        case 5: Rate = 5; break;
                        case 6: Rate = 6; break;
                        case 7: Rate = 7; break;
                        case 8: Rate = 8; break;
                        case 9: Rate = 10; break;
                    }
                }
                #endregion
                if (Role.Core.Rate(Spell.Duration + Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery Attack = new InteractQuery();
                        Attack.SpellID = Spell.ID;
                        Attack.SpellLevel = Spell.Level;
                        Attack.OpponentUID = attacked.UID;
                        Attack.X = attacked.X;
                        Attack.Y = attacked.Y;
                        MsgAttackPacket.ProcescMagic(client, stream, Attack, true);
                    }
                }
            }
            #endregion

            #region HellVortex
            if (!attacked.ContainFlag(MsgUpdate.Flags.HellVortex) && attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HellVortex))
            {
                var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.HellVortex][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.HellVortex].Level];
                if (attacked.Rate(Spell.Damage2))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(attacked.UID, attacked.UID, attacked.X, attacked.Y, Spell.ID, Spell.Level, 0);
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(attacked.Owner);
                        if (attacked.AddFlag(MsgUpdate.Flags.HellVortex, (int)Spell.Duration, true))
                            attacked.SendUpdate(stream, MsgUpdate.Flags.HellVortex, (uint)Spell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                    }
                }
            }
            #endregion

            #region ArchiveTrojan
            client.MyArchives.ReceiveAttack(attacked);
            if (client.Player.ContainFlag(MsgUpdate.Flags.CleanSweep))
                obj.Damage += (uint)(obj.Damage * client.Player.CleanSweepPower / 100);

            if (client.Player.ContainFlag(MsgUpdate.Flags.AxeShadow))
                obj.Damage += (uint)(obj.Damage * client.Player.AxeShadowPower / 100);
            #region HawksAmbition
            if (attacked.ContainFlag(MsgUpdate.Flags.HawksAmbition))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    uint Damages = (uint)(obj.Damage * attacked.Owner.Player.HawksAmbitionPower / 100);
                    var HawksAmbition = Pool.Magic[(ushort)Role.Flags.SpellID.HawksAmbition][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.HawksAmbition].Level];
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = HawksAmbition.ID;
                    InteractQuery.SpellLevel = HawksAmbition.Level;
                    InteractQuery.X = attacked.X;
                    InteractQuery.Y = attacked.Y;
                    InteractQuery.UID = attacked.UID;
                    InteractQuery.OpponentUID = client.Player.UID;
                    MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                }
                obj.Damage -= (uint)(obj.Damage * attacked.Owner.Player.HawksAmbitionPower / 100);
            }
            #endregion


            #endregion

            #region MudWall
            if (attacked.ContainFlag(MsgUpdate.Flags.MudWall))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                    if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MudWall))
                    {
                        if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.MudWall, out DBSpells))
                        {
                            Database.MagicType.Magic DBSpell;
                            if (DBSpells.TryGetValue(attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.MudWall].Level, out DBSpell))
                            {
                                int Rate = DBSpell.DamageOnHuman / 100;
                                Ninja.Item item;
                                if (attacked.Owner.MyNinja.TryGetValueEquip(Ninja.ItemType.MudSigilImpeccable, out item))
                                {
                                    Rate += item.DBItem.Power / 100;
                                }
                                if (Calculate.Base.Rate(Rate))
                                {
                                    obj.Damage -= (uint)(obj.Damage * (DBSpell.Damage2 / 100) / 100);

                                }
                            }
                        }


                    }

                }
            }
            #endregion

            #region Dragon
            MsgSpell Owner_spells = null;
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengRocket))
            {
                if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.KunpengRocket, out Owner_spells))
                {
                    Database.MagicType.Magic SkyRocket = Pool.Magic[Owner_spells.ID][Owner_spells.Level];
                    if (Role.Core.Rate(SkyRocket.Rate))
                    {
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = Owner_spells.ID;
                        InteractQuery.SpellLevel = Owner_spells.Level;
                        InteractQuery.X = attacked.X;
                        InteractQuery.Y = attacked.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.Owner.Player.UID;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                        }
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonHeart))
            {
                Database.MagicType.Magic DragonHeart = Pool.Magic[(ushort)Role.Flags.SpellID.DragonHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level];
                if (Role.Core.Rate(DragonHeart.GDamage))
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.DragonHeart;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengHeart))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level];
                if (Role.Core.Rate(KunpengHeart.Rate))
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengHeart;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengRocket))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengRocket][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level];
                int IncRate = 0;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Spreadyourwings))
                {
                    Database.MagicType.Magic Hitthewaterthreethousand = Pool.Magic[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand].Level];
                    IncRate = Hitthewaterthreethousand.DamageOnHuman;
                }
                if (Role.Core.Rate(KunpengHeart.Rate + IncRate))
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengRocket;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonTransformation))
            {
                Database.MagicType.Magic WildPhoenix = Pool.Magic[(ushort)Role.Flags.SpellID.DragonTransformation][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonTransformation].Level];
                if (attacked.ContainFlag(MsgUpdate.Flags.DragonShift))
                {
                    if (attacked.Owner.Player.DragonShiftCount > 0)
                    {
                        attacked.Owner.Player.DragonShiftCount -= 1;
                        obj.Damage = (obj.Damage * 70) / 100;

                    }

                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniHeart))
            {
                Database.MagicType.Magic SuanniHeart = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level];
                if (Role.Core.Rate(SuanniHeart.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniHeart;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniCommand))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniCommand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniCommand;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            #endregion

            #region ArchiveArcher
            #region ArchShadow
            if (client.MyArchives.isOpen(Archives.TypeID.StoneCracker))
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgSpell Owner_spell = null;
                    if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ArchShadow, out Owner_spell))
                    {
                        Database.MagicType.Magic Archshadow = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                        if (Role.Core.Rate(Archshadow.Rate))
                        {
                            InteractQuery InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = Owner_spell.ID;
                            InteractQuery.SpellLevel = Owner_spell.Level;
                            InteractQuery.X = client.Player.X;
                            InteractQuery.Y = client.Player.Y;
                            InteractQuery.UID = client.Player.UID;
                            InteractQuery.OpponentUID = attacked.UID;
                            MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                        }
                    }

                }

            }
            #endregion

            #region Hunter
            if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Hunter, out Owner_spells))
            {
                Database.MagicType.Magic Hunter = Pool.Magic[Owner_spells.ID][Owner_spells.Level];
                if (Role.Core.Rate(Hunter.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = Owner_spells.ID;
                    InteractQuery.SpellLevel = Owner_spells.Level;

                    InteractQuery.X = attacked.X;
                    InteractQuery.Y = attacked.Y;
                    InteractQuery.UID = client.Player.UID;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                    }
                }

            }
            #endregion
            #region AttackArrowBlades
            if (client.MyArchives.isOpen(Archives.TypeID.StoneCracker) && client.Player.ContainFlag(MsgUpdate.Flags.AttackArrowBlades))
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Database.MagicType.Magic ArrowBlades = Pool.Magic[(ushort)Role.Flags.SpellID.ArrowBlades][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ArrowBlades].Level];
                    obj.Damage += (uint)(obj.Damage * ArrowBlades.Damage2) / 100;
                }
            }
            #endregion


            #endregion

            #region InfiniteMist
            if (Database.AtributesStatus.IsNinja(attacked.Owner.Player.Class))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InfiniteMist))
                    {
                        MsgSpell InfiniteMist;
                        if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.InfiniteMist, out InfiniteMist))
                        {
                            Database.MagicType.Magic Mist = Pool.Magic[InfiniteMist.ID][InfiniteMist.Level];
                            byte rate = 0;
                            byte GameServer = 0;
                            if (client.Rune.IsEquipped("Sky Veil", ref GameServer))
                            {
                                switch (GameServer)
                                {
                                    case 1: rate = 20; break;
                                    case 2: rate = 30; break;
                                    case 3: rate = 40; break;
                                    case 4: rate = 50; break;
                                    case 5: rate = 60; break;
                                    case 6: rate = 70; break;
                                    case 7: rate = 80; break;
                                    case 8: rate = 90; break;
                                    case 9: rate = 100; break;
                                }
                            }
                            if (!Role.Core.Rate(rate))
                            {
                                if (Role.Core.Rate(Mist.Rate))
                                {
                                    client.Player.AddSpellFlag(MsgUpdate.Flags.InfiniteMist, (int)Mist.Duration, true);
                                    client.Player.SendUpdate(stream, MsgUpdate.Flags.InfiniteMist, Mist.Duration, (uint)Mist.Damage, 0, MsgUpdate.DataType.ArchiveSkill);
                                }
                            }
                        }
                    }
                }

            }
            #endregion

            #region Redcurse
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.Redcurse))
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    ServerSockets.Packet stream = recycledPacket.GetStream();
                    if (!attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.ArmorOfImmunity))
                    {
                        if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArmorofImmunity))
                        {
                            #region ArmorOfImmunity
                            MagicType.Magic ArmorOfImmunity = Pool.Magic[(ushort)Role.Flags.SpellID.ArmorofImmunity][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ArmorofImmunity].Level];
                            if (Role.Core.Rate(ArmorOfImmunity.Rate))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = ArmorOfImmunity.ID;
                                InteractQuery.SpellLevel = ArmorOfImmunity.Level;
                                InteractQuery.X = attacked.X;
                                InteractQuery.Y = attacked.Y;
                                InteractQuery.OpponentUID = attacked.Owner.Player.UID;
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                            }
                            #endregion

                        }
                    }

                }

            }
            #endregion

            #region WaterAegisRebirth
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegisRebirth))
            {
                MsgSpell Spell;
                if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WaterAegisRebirth, out Spell))
                {
                    Database.MagicType.Magic WaterAegis = Pool.Magic[Spell.ID][Spell.Level];
                    if (Role.Core.Rate(WaterAegis.Rate / 100))
                    {
                        InteractQuery Attack = new InteractQuery();
                        Attack.SpellID = Spell.ID;
                        Attack.SpellLevel = Spell.Level;
                        Attack.X = (ushort)attacked.X;
                        Attack.Y = (ushort)attacked.Y;
                        Attack.OpponentUID = client.Player.UID;
                        using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = recycledPacket.GetStream();
                            MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                        }

                    }
                }
            }
            #endregion

            #region ArchiveTao

            #region WildPhoenixActivated
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix))
            {
                if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WildPhoenix, out Owner_spells))
                {
                    Database.MagicType.Magic WildPhoenix = Pool.Magic[Owner_spells.ID][Owner_spells.Level];
                    if (attacked.ContainFlag(MsgUpdate.Flags.WildPhoenix))
                    {
                        if (attacked.Owner.Player.WildPhoenixDamage > 0)
                        {
                            attacked.Owner.Player.WildPhoenixDamage -= 1;

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                attacked.Owner.Player.SendUpdate(stream, MsgUpdate.Flags.WildPhoenix, WildPhoenix.Duration, (uint)30110, attacked.Owner.Player.WildPhoenixDamage, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            obj.Damage = 1;
                            if (attacked.Owner.Player.WildPhoenixDamage == 0)
                            {
                                attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.WildPhoenix);
                            }
                        }

                    }

                }
            }
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.BirthDeath))
            {
                List<ushort> ushortList = new List<ushort>();

                MsgSpell user_spell = null;
                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(attacked.X, attacked.Y, client.Player.X, client.Player.Y) <= 1)
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.AblazeBlade, out user_spell))
                    {
                        var Magic = Pool.Magic[user_spell.ID][user_spell.Level];
                        if (Role.Core.Rate(Magic.Rate))
                        {
                            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)(ushort)Role.Flags.SpellID.AblazeBlade))
                            {
                                InteractQuery Attack = new InteractQuery();
                                Attack.SpellID = (ushort)Role.Flags.SpellID.AblazeBlade;
                                Attack.SpellLevel = attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.AblazeBlade].Level;
                                Attack.X = (ushort)attacked.X;
                                Attack.Y = (ushort)attacked.Y;
                                Attack.OpponentUID = client.Player.UID;
                                using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                                {
                                    ServerSockets.Packet stream = recycledPacket.GetStream();
                                    MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region CrackMantra
            if (client.MyArchives.isOpen(Archives.TypeID.Evolution))
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgSpell Owner_spell = null;
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackMantra))
                    {
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.CrackMantra, out Owner_spell))
                        {
                            Database.MagicType.Magic CrackMantra = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                            if (Role.Core.Rate(CrackMantra.Rate / 100))
                            {
                                byte RandomCrackMantra = (byte)Pool.GetRandom.Next(1, 3);
                                switch (RandomCrackMantra)
                                {
                                    case 1:
                                        {
                                            if (!attacked.ContainFlag(MsgUpdate.Flags.CrackMantra1))
                                            {
                                                attacked.AddSpellFlag(MsgUpdate.Flags.CrackMantra1, (int)5, true);
                                                attacked.SendUpdate(stream, MsgUpdate.Flags.CrackMantra1, 5, (uint)CrackMantra.DamageOnHuman, 0, MsgUpdate.DataType.ArchiveSkill);
                                                attacked.Owner.Player.CrackMantra1 = (uint)CrackMantra.DamageOnHuman;
                                                attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante, false);

                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            if (!attacked.ContainFlag(MsgUpdate.Flags.CrackMantra2))
                                            {
                                                attacked.AddSpellFlag(MsgUpdate.Flags.CrackMantra2, (int)5, true);
                                                attacked.SendUpdate(stream, MsgUpdate.Flags.CrackMantra2, 5, (uint)CrackMantra.Damage2, 0, MsgUpdate.DataType.ArchiveSkill);
                                                attacked.Owner.Player.CrackMantra2 = (uint)CrackMantra.Damage2;
                                                attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante, false);
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }

                }

            }
            #endregion

            //#region DeadwoodCurse
            //if (client.MyArchives.isOpen(Archives.TypeID.Vicissitude) && !attacked.ContainFlag(MsgUpdate.Flags.DeadwoodCurse))
            //{
            //    if (client.Player.DeadwoodIncFinal > 0)
            //    {
            //        client.Player.DeadwoodIncFinal = 0;
            //        client.Equipment.QueryEquipment(client.Equipment.Alternante);
            //    }
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();

            //        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DeadwoodCurse))
            //        {
            //            MsgSpell Owner_spell = null;
            //            if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DeadwoodCurse, out Owner_spell))
            //            {
            //                Database.MagicType.Magic DeadwoodCurse = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
            //                if (Role.Core.Rate(DeadwoodCurse.Rate / 100))
            //                {
            //                    attacked.AddFlag(MsgUpdate.Flags.DeadwoodCurse, (int)DeadwoodCurse.Duration, true);
            //                }
            //            }
            //        }

            //    }

            //}
            //if (attacked.ContainFlag(MsgUpdate.Flags.DeadwoodCurse))
            //{
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {

            //        var stream = rec.GetStream();
            //        Database.MagicType.Magic DeadwoodCurse = Pool.Magic[(ushort)Role.Flags.SpellID.DeadwoodCurse][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DeadwoodCurse].Level];
            //        attacked.SendUpdate(stream, MsgUpdate.Flags.DeadwoodCurse, DeadwoodCurse.Duration, (uint)DeadwoodCurse.Damage, (uint)DeadwoodCurse.DamageOnHuman, MsgUpdate.DataType.ArchiveSkill, true);
            //        client.Player.DeadwoodIncFinal += DeadwoodCurse.DamageOnHuman;
            //        if (client.Player.DeadwoodIncFinal >= 5000)
            //        {
            //            client.Player.DeadwoodIncFinal = 5000;
            //            client.Equipment.QueryEquipment(client.Equipment.Alternante);
            //        }
            //        client.Equipment.QueryEquipment(client.Equipment.Alternante);

            //    }
            //}
            //#endregion

            #region Subtitution
            if (attacked.ContainFlag(MsgUpdate.Flags.Substitution) && !attacked.ContainFlag(MsgUpdate.Flags.SubstitutionAttack))
            {
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Substitution))
                {
                    MsgSpell Owner_spell = null;
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Substitution, out Owner_spell))
                    {
                        Database.MagicType.Magic Substitution = Pool.Magic[Owner_spell.ID][Owner_spell.Level];
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            uint DamageMin = (uint)(obj.Damage * Substitution.Damage2 / 100) / 100;
                            if (attacked.SubstitutionDefence > 0)
                            {
                                attacked.SubstitutionDefence -= (int)DamageMin;
                                attacked.SendUpdate(stream, MsgUpdate.Flags.Substitution, (uint)0, (uint)attacked.SubstitutionDefence, (uint)0, MsgUpdate.DataType.AzureShield, true);
                                Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(client.Player, attacked, (uint)DamageMin);
                                obj.Damage -= (uint)DamageMin;

                            }
                            else
                            {
                                attacked.SendUpdate(stream, MsgUpdate.Flags.Substitution, (uint)0, (uint)0, (uint)0, MsgUpdate.DataType.AzureShield, true);
                                Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(client.Player, attacked, (uint)attacked.SubstitutionDefence);
                                obj.Damage -= (uint)attacked.SubstitutionDefence;
                                if (attacked.AddFlag(MsgUpdate.Flags.SubstitutionAttack, 10, true))
                                    attacked.SendUpdate(stream, MsgUpdate.Flags.SubstitutionAttack, (int)10, (uint)0, 0, MsgUpdate.DataType.ArchiveSkill);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Vicissitude
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.Vicissitude))
            {
                #region DivineEmptiness
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineEmptiness))
                {
                    Database.MagicType.Magic DivineEmptiness = Pool.Magic[(ushort)Role.Flags.SpellID.DivineEmptiness][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DivineEmptiness].Level];
                    if (Role.Core.Rate(DivineEmptiness.Rate / 100))
                    {
                        InteractQuery Attack = new InteractQuery();
                        Attack.SpellID = (ushort)Role.Flags.SpellID.DivineEmptiness;
                        Attack.SpellLevel = attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DivineEmptiness].Level;
                        Attack.X = (ushort)attacked.X;
                        Attack.Y = (ushort)attacked.Y;
                        Attack.OpponentUID = client.Player.UID;
                        using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = recycledPacket.GetStream();
                            MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                        }

                    }
                }
                #endregion
            }
            #endregion

            #region HighestGood
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.HighestGood))
            {
                MsgSpell Spell;
                #region WaterAegis
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegis))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WaterAegis, out Spell))
                    {
                        Database.MagicType.Magic WaterAegis = Pool.Magic[Spell.ID][Spell.Level];
                        if (Role.Core.Rate(WaterAegis.Rate / 100))
                        {
                            InteractQuery Attack = new InteractQuery();
                            Attack.SpellID = Spell.ID;
                            Attack.SpellLevel = Spell.Level;
                            Attack.X = (ushort)attacked.X;
                            Attack.Y = (ushort)attacked.Y;
                            Attack.OpponentUID = client.Player.UID;
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                            }

                        }
                    }
                }
                #endregion
                #region FlowKnack

                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlowKnack))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlowKnack, out Spell))
                    {
                        Database.MagicType.Magic FlowKnack = Pool.Magic[Spell.ID][Spell.Level];
                        if (Role.Core.Rate(FlowKnack.Rate / 100))
                        {
                            InteractQuery Attack = new InteractQuery();
                            Attack.SpellID = Spell.ID;
                            Attack.SpellLevel = Spell.Level;
                            Attack.X = (ushort)attacked.X;
                            Attack.Y = (ushort)attacked.Y;
                            Attack.OpponentUID = client.Player.UID;
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                            }

                        }

                    }
                }
                #endregion
            }
            #endregion

            #region Evolution
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.Evolution))
            {
                MsgSpell Spells;
                #region SolidBulwark
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidBulwark))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.SolidBulwark, out Spells))
                    {
                        Database.MagicType.Magic SolidBulwark = Pool.Magic[Spells.ID][Spells.Level];
                        if (Role.Core.Rate(SolidBulwark.Rate / 100))
                        {
                            InteractQuery Attack = new InteractQuery();
                            Attack.SpellID = Spells.ID;
                            Attack.SpellLevel = Spells.Level;
                            Attack.X = (ushort)attacked.X;
                            Attack.Y = (ushort)attacked.Y;
                            Attack.OpponentUID = client.Player.UID;
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, Attack, true);
                            }
                        }

                    }
                }
                #endregion
            }
            #endregion

            #region Thrill
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.Thrill))
            {
                MsgSpell Spells;
                #region FantasyKnack
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FantasyKnack))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FantasyKnack, out Spells))
                    {
                        Database.MagicType.Magic FantasyKnack = Pool.Magic[Spells.ID][Spells.Level];
                        if (Role.Core.Rate(FantasyKnack.Rate / 100))
                        {
                            using (ServerSockets.RecycledPacket recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                ServerSockets.Packet stream = recycledPacket.GetStream();
                                attacked.AddSpellFlag(MsgUpdate.Flags.FantasyKnack, (int)FantasyKnack.Duration, true);
                                attacked.SendUpdate(stream, MsgUpdate.Flags.FantasyKnack, FantasyKnack.Duration, (uint)FantasyKnack.DamageOnHuman, (uint)FantasyKnack.Damage2, MsgUpdate.DataType.ArchiveSkill);
                            }

                        }

                    }
                }
                #endregion
            }
            #endregion

            #region BirthDeath
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.BirthDeath))
            {
                MsgSpell Spells;
                #region FlameShield
                if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlameShield, out Spells))
                {
                    MagicType.Magic FlameShield = Pool.Magic[Spells.ID][Spells.Level];
                    if (Role.Core.Rate(FlameShield.Rate / 100))
                    {
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = Spells.ID;
                        InteractQuery.SpellLevel = Spells.Level;
                        InteractQuery.X = attacked.X;
                        InteractQuery.Y = attacked.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = attacked.UID;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                        }
                    }
                }
                #endregion

            }

            #endregion

            #endregion

            #region DragonPierce
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPierce))
            {
                Database.MagicType.Magic DragonPierces = Pool.Magic[(ushort)Role.Flags.SpellID.DragonPierce][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonPierce].Level];
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    ServerSockets.Packet stream = recycledPacket.GetStream();

                    List<byte> CanUse = new List<byte>();
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit))
                        CanUse.Add(1);
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceTortoise))
                        CanUse.Add(2);
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceBreak))
                        CanUse.Add(3);
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceStamina))
                        CanUse.Add(4);
                    if (CanUse.Count != 0)
                    {
                        switch (CanUse[Pool.GetRandom.Next(0, CanUse.Count)])
                        {
                            case 1:
                                client.Player.AddFlag(MsgUpdate.Flags.DragonPierceCrit, (int)DragonPierces.Duration, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceCrit, (uint)DragonPierces.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                break;
                            case 2:
                                client.Player.AddFlag(MsgUpdate.Flags.DragonPierceTortoise, (int)DragonPierces.Duration, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceTortoise, (uint)DragonPierces.Duration, (uint)DragonPierces.DamageOnHuman, 0, MsgUpdate.DataType.ArchiveSkill);
                                break;
                            case 3:
                                client.Player.AddFlag(MsgUpdate.Flags.DragonPierceBreak, (int)DragonPierces.Duration, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceBreak, (uint)DragonPierces.Duration, (uint)DragonPierces.Damage2, 0, MsgUpdate.DataType.ArchiveSkill);
                                break;
                            case 4:
                                client.Player.AddFlag(MsgUpdate.Flags.DragonPierceStamina, (int)DragonPierces.Duration, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceStamina, (uint)DragonPierces.Duration, (uint)DragonPierces.Damage3, (uint)DragonPierces.Damage3, MsgUpdate.DataType.ArchiveSkill);
                                break;
                        }
                    }

                }
            }

            #endregion

            #region Bloodlust
            if (attacked.Owner.MyArchives.isOpen(Archives.TypeID.Bloodlust))
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    ServerSockets.Packet stream = recycledPacket.GetStream();
                    if (!attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Immersion))
                    {
                        if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Immersion))
                        {
                            #region Immersion
                            MagicType.Magic Immersion = Pool.Magic[(ushort)Role.Flags.SpellID.Immersion][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Immersion].Level];
                            if (Role.Core.Rate(Immersion.Rate))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = Immersion.ID;
                                InteractQuery.SpellLevel = Immersion.Level;
                                InteractQuery.X = attacked.X;
                                InteractQuery.Y = attacked.Y; ;
                                InteractQuery.OpponentUID = attacked.Owner.Player.UID;
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                            }
                            #endregion

                        }
                    }
                    if (!attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Insouciance))
                    {
                        if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Insouciance))
                        {
                            #region Insouciance
                            MagicType.Magic Insouciance = Pool.Magic[(ushort)Role.Flags.SpellID.Insouciance][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Insouciance].Level];
                            if (Role.Core.Rate(Insouciance.Rate))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = Insouciance.ID;
                                InteractQuery.SpellLevel = Insouciance.Level;
                                InteractQuery.X = attacked.X;
                                InteractQuery.Y = attacked.Y; ;
                                InteractQuery.OpponentUID = attacked.Owner.Player.UID;
                                MsgAttackPacket.ProcescMagic(attacked.Owner, stream, InteractQuery, true);
                            }
                            #endregion

                        }
                    }
                }
                if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Immersion))
                {
                    MagicType.Magic Immersion = Pool.Magic[(ushort)Role.Flags.SpellID.Immersion][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Immersion].Level];
                    obj.Damage -= (uint)((obj.Damage * Immersion.GDamage) / 100);
                }
            }
            if (client.MyArchives.isOpen(Archives.TypeID.Bloodlust))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    #region Immersion
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Immersion))
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Immersion))
                        {
                            Database.MagicType.Magic Immersion = Pool.Magic[(ushort)Role.Flags.SpellID.Immersion][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Immersion].Level];
                            if (Role.Core.Rate(Immersion.Damage2))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = Immersion.ID;
                                InteractQuery.SpellLevel = Immersion.Level;
                                InteractQuery.X = attacked.X;
                                InteractQuery.Y = attacked.Y; ;
                                InteractQuery.OpponentUID = client.Player.UID;
                                MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                            }
                        }
                    }
                    #endregion
                    #region Insouciance
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Insouciance))
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Insouciance))
                        {
                            Database.MagicType.Magic Insouciance = Pool.Magic[(ushort)Role.Flags.SpellID.Insouciance][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Insouciance].Level];
                            if (Role.Core.Rate(Insouciance.Damage2))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = Insouciance.ID;
                                InteractQuery.SpellLevel = Insouciance.Level;
                                InteractQuery.X = attacked.X;
                                InteractQuery.Y = attacked.Y; ;
                                InteractQuery.OpponentUID = client.Player.UID;
                                MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region SageMode
            if (Database.AtributesStatus.IsNinja(client.Player.Class))
            {
                foreach (var Ninja in NinjaFile.gouyu_immortals.Values)
                {
                    if ((client.MyNinja.Levels > 17 ? 17 : client.MyNinja.Levels) == Ninja.Level)
                    {
                        if (Role.Core.Rate(Ninja.Rate / 100))
                        {
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.SageMode))
                            {
                                client.MyNinja.SageMode(Ninja.Seconds);
                            }
                        }
                    }
                }
            }
            #endregion

            #region LightningShield
            if (attacked.ContainFlag(MsgUpdate.Flags.LightningShield) && attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LightningShield))
            {
                if (Role.Core.Rate(20))
                {
                    var lightningSpell = Pool.Magic[(ushort)Role.Flags.SpellID.LightningShield][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.LightningShield].Level];
                    attacked.LightningShieldLeft = (uint)(attacked.ContainFlag(MsgUpdate.Flags.ThunderRampage) ? (20000 + (lightningSpell.Level + 1 * 1000)) : 20000);
                    #region FlashShield(Rune Skill)
                    byte itemLevel = 0;
                    if (attacked.Owner.Rune.IsEquipped("FlashShield", ref itemLevel))
                    {
                        uint addition = 5000;
                        switch (itemLevel)
                        {
                            case 2: addition = 6000; break;
                            case 3: addition = 7000; break;
                            case 4: addition = 7500; break;
                            case 5: addition = 8000; break;
                            case 6: addition = 8500; break;
                            case 7: addition = 9000; break;
                            case 8: addition = 9500; break;
                            case 9: addition = 10000; break;
                            case 10: addition = 11000; break;
                            case 11: addition = 11500; break;
                            case 12: addition = 12000; break;
                            case 13: addition = 12500; break;
                            case 14: addition = 13000; break;
                            case 15: addition = 13500; break;
                            case 16: addition = 14000; break;
                            case 17: addition = 14500; break;
                            case 18: addition = 15000; break;
                            case 19: addition = 15500; break;
                            case 20: addition = 16000; break;
                            case 21: addition = 16500; break;
                            case 22: addition = 17000; break;
                            case 23: addition = 17500; break;
                            case 24: addition = 18000; break;
                            case 25: addition = 18500; break;
                            case 26: addition = 19000; break;
                            case 27: addition = 20000; break;
                        }
                        attacked.LightningShieldLeft += addition;
                    }
                    #endregion
                    attacked.AddFlag(MsgUpdate.Flags.LightningShieldActivated, 10, true);
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        attacked.SendUpdate(streamm, MsgUpdate.Flags.LightningShieldActivated, 10, attacked.LightningShieldLeft, lightningSpell.Level, MsgUpdate.DataType.AzureShield);
                    }
                }
            }
            bool Fusingss = false;
            bool Crack = false;
            #region Crack
            if (client.Status.Crack > 0)
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Crack].TryGetValue(client.Status.Crack, out MythInfo))
                {
                    double IncRate = 0;
                    MythSoulAttributes.Attribute SuperPower;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperPower))
                    {
                        if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Superpower].TryGetValue(client.Status.Superpower, out SuperPower))
                        {
                            IncRate = (double)SuperPower.Damage / 100;
                        }
                    }
                    double IncRatee = 0;
                    MythSoulAttributes.Attribute Oracle;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oracle))
                    {
                        if (MythSoulAttributes.Attributes.ContainsKey(MythSoulAttributes.Type.Oracle))
                        {
                            if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Oracle].TryGetValue(client.Status.Oracle, out Oracle))
                            {
                                IncRatee = (double)Oracle.Damage / 100;
                            }
                        }

                    }
                    if (Calculate.Base.Rate((int)MythInfo.Rate + (int)IncRate - (int)IncRatee))
                    {
                        Crack = true;

                    }
                }
            }
            #endregion
            if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fusing) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Fusing))
            {
                var Fusings = Pool.Magic[(ushort)Role.Flags.SpellIDPirate.Fusing][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellIDPirate.Fusing].Level];
                if (Calculate.Base.Rate(Fusings.GDamage / 100))
                {
                    Fusingss = true;
                }
            }
            if (attacked.ContainFlag(MsgUpdate.Flags.LightningShieldActivated))
            {
                if (!Fusingss || !Crack)
                {

                    if (attacked.LightningShieldLeft > 0)
                    {
                        if (obj.Damage >= attacked.LightningShieldLeft)
                        {
                            obj.Damage -= attacked.LightningShieldLeft;
                            attacked.LightningShieldLeft = 0;
                        }
                        else
                        {
                            attacked.LightningShieldLeft -= obj.Damage;
                            obj.Damage = 0;
                        }
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            attacked.SendUpdate(streamm, MsgUpdate.Flags.LightningShieldActivated, (uint)((attacked.BitVector.ArrayFlags[(int)MsgUpdate.Flags.LightningShieldActivated].Timer.AddSeconds(10) - DateTime.Now).TotalSeconds), attacked.LightningShieldLeft, attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.LightningShield].Level, MsgUpdate.DataType.AzureShield);
                        }
                    }
                }
                if (Crack)
                {
                    obj.Effect = MsgAttackPacket.AttackEffect.CrackMyth;
                }
            }


            #endregion

            #region SparkShield
            if (attacked.ContainFlag(MsgUpdate.Flags.SparkShield) && attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SparkShield))
            {
                var SparkShield = Pool.Magic[(ushort)Role.Flags.SpellID.SparkShield][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SparkShield].Level];
                if (Role.Core.Rate(SparkShield.DamageOnHuman))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.SparkShieldDamage = (uint)SparkShield.Damage2;
                        attacked.SparkShieldLevel = SparkShield.Level;
                        attacked.SparkShieldTime = (int)SparkShield.DamageOnMonster;
                        attacked.SparkShieldStamp = DateTime.Now;
                        attacked.AddSpellFlag(MsgUpdate.Flags.SparkShieldActivated, (int)(int)SparkShield.DamageOnMonster, true);

                    }

                }
            }

            #endregion

            #region MudSigilImpeccable
            if (attacked.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MudWall))
            {
                Ninja.Item ITEM;
                if (attacked.Owner.MyNinja.TryGetValueEquip(Ninja.ItemType.MudSigilImpeccable, out ITEM))
                {
                    uint Power = ((obj.Damage) * (uint)ITEM.DBItem.Power / 100) / 100;
                    obj.Damage -= (uint)Power;
                }
            }
            #endregion

            #region SparkShieldActivated
            if (attacked.ContainFlag(MsgUpdate.Flags.SparkShieldActivated))
            {
                if (attacked.SparkShieldDamage >= obj.Damage)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var Action = new InteractQuery()
                        {
                            AtkType = (ushort)MsgAttackPacket.AttackID.BlueDamage,
                            UID = client.Player.UID,
                            OpponentUID = client.Player.UID,
                            X = attacked.X,
                            Y = attacked.Y,
                            Damage = (int)obj.Damage
                        };
                        attacked.View.SendView(stream.InteractionCreate(Action), true);
                        attacked.SparkShieldDamage -= (uint)obj.Damage;
                        attacked.SendSparkShield(stream);
                        obj.Damage = 1;
                    }

                }
                else
                {
                    if (obj.Damage > attacked.SparkShieldDamage)
                        obj.Damage -= attacked.SparkShieldDamage;
                    else
                        obj.Damage = 1;
                    attacked.SparkShieldDamage = 0;
                    attacked.RemoveFlag(MsgUpdate.Flags.SparkShieldActivated);
                }
            }
            #endregion

            #region PerfectionEfect

            #region CalmWind
            if (client.PerfectionStatus.CalmWind > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.CalmWind))
                {
                    foreach (var spell in client.MySpells.ClientSpells.Values)
                    {
                        Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                        if (Pool.Magic.TryGetValue(spell.ID, out DBSpells))
                        {
                            Database.MagicType.Magic DBSpell;
                            if (DBSpells.TryGetValue(spell.Level, out DBSpell))
                            {
                                if (!DBSpell.isRuneSkill)
                                    spell.ColdTime = System.DateTime.Now;
                            }
                        }
                    }
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.CalmWind,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);
                    }
                }
            }
            #endregion

            #region DrainingTouch
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);

                        bool update = false;
                        if (client.Player.HitPoints < client.Player.Owner.Status.MaxHitpoints)
                        {
                            update = true;
                            client.Player.HitPoints = (int)client.Player.Owner.Status.MaxHitpoints;
                        }
                        if (client.Player.Mana < client.Player.Owner.Status.MaxMana)
                        {
                            update = true;
                            client.Player.Mana = (ushort)client.Player.Owner.Status.MaxMana;
                        }
                        if (update)
                        {
                            client.Player.SendUpdateHP();
                            client.Player.SendUpdate(stream, client.Player.Mana, MsgUpdate.DataType.Mana, false);
                        }
                    }
                }
            }
            #endregion

            #region LightOfStamina
            if (client.PerfectionStatus.LightOfStamina > 0)
            {
                if (client.PrestigeLevel > attacked.Owner.PrestigeLevel)
                {
                    if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.LightOfStamina))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.LightOfStamina,
                                Id = attacked.UID,
                                dwParam = attacked.UID
                            }), true);
                            if (client.Player.Stamina < 100)
                            {
                                client.Player.Stamina = 100;
                                client.Player.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina);
                            }
                        }
                    }
                }
            }
            #endregion

            #region KillingFlash
            if (client.PerfectionStatus.KillingFlash > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.KillingFlash))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.KillingFlash,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.XPList) && client.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                        {
                            client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                            client.Player.SendUpdate(stream, 20, MsgUpdate.DataType.XPList);
                        }
                    }
                }
            }
            #endregion

            /* #region  MirrorOfSin
            if (attacked.Owner.PerfectionStatus.MirrorOfSin > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.MirrorOfSin))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.Owner.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.MirrorOfSin,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        if (attacked.Owner.Player.OnXPSkill() == MsgUpdate.Flags.Normal && !attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.XPList))
                        {
                            attacked.Owner.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                            attacked.Owner.Player.SendUpdate(stream, 20, MsgUpdate.DataType.XPList);
                        }
                    }
                }
            }
            #endregion*/

            #region DivineGuard
            if (client.PerfectionStatus.DivineGuard > 0)
            {
                if (client.PrestigeLevel > attacked.Owner.PrestigeLevel)
                {
                    if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DivineGuard))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.DivineGuard,
                                Id = client.Player.UID,
                                dwParam = client.Player.UID
                            }), true);
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.DivineGuard))
                            {
                                client.Player.AddFlag(MsgUpdate.Flags.DivineGuard, 10, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DivineGuard, 10, 0, 0, MsgUpdate.DataType.AzureShield);
                            }
                        }
                    }
                }
            }

            #endregion

            #region BloodSpawn
            if (attacked.Owner.PerfectionStatus.BloodSpawn > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.BloodSpawn))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.BloodSpawn,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        bool update = false;
                        if (attacked.HitPoints < attacked.Owner.Status.MaxHitpoints)
                        {
                            update = true;
                            attacked.HitPoints = (int)attacked.Owner.Status.MaxHitpoints;
                        }
                        if (attacked.Mana < attacked.Owner.Status.MaxMana)
                        {
                            update = true;
                            attacked.Mana = (ushort)attacked.Owner.Status.MaxMana;
                        }
                        if (update)
                        {
                            attacked.SendUpdateHP();
                            attacked.SendUpdate(stream, attacked.Mana, MsgUpdate.DataType.Mana, false);
                        }
                    }
                }
            }
            #endregion


            #endregion

            #region SupremeLeadership
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
            {
                Database.MagicType.Magic SupremeLeadership = Pool.Magic[(ushort)Role.Flags.SpellID.SupremeLeadership][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level];
                if (Role.Core.Rate(SupremeLeadership.DamageOnHuman % 100))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SupremeLeadership;
                        InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level;
                        InteractQuery.X = client.Player.X;
                        InteractQuery.Y = client.Player.Y;
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.OpponentUID = client.Player.UID;
                        MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                    }
                }
            }
            #endregion

            #region Dura
            if (Calculate.Base.Rate(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;
            #endregion
            if (client.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                obj.Effect = MsgAttackPacket.AttackEffect.Destroy;
            #region GrowFromHurt

            MsgSpell user_spell1 = null;
            if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GrowFromHurt, out user_spell1))
            {
                var spell = Pool.Magic[(ushort)Role.Flags.SpellID.GrowFromHurt][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.GrowFromHurt].Level];
                if (Time32.Now >= client.Player.GrowFromHurtStamp.AddMilliseconds((int)spell.ColdTime))
                {
                    if (Role.Core.Rate(spell.Rate))
                    {
                        var InteractQuery = new InteractQuery();
                        InteractQuery.UID = client.Player.UID;
                        InteractQuery.SpellID = user_spell1.ID;
                        InteractQuery.SpellLevel = user_spell1.Level;
                        InteractQuery.X = 0;
                        InteractQuery.Y = 0;
                        InteractQuery.OpponentUID = attacked.UID;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgServer.MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                        }
                    }
                }
            }
            #endregion


            #region OnTransform
            if (client.Fake)
            {
                obj.Damage = (uint)(Pool.GetRandom.Next(30000, 90000));
            }
            #region HoverFeather
            if (Role.Core.Rate(1))
            {
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HoverFeather))
                {
                    if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HoverFeather, out Owner_spells))
                    {
                        Database.MagicType.Magic HoverFeather = Pool.Magic[Owner_spells.ID][Owner_spells.Level];
                        if (attacked.ContainFlag(MsgUpdate.Flags.HoverFather))
                        {
                            if (attacked.Owner.Player.HoverFatherDamage > 0)
                            {
                                attacked.Owner.Player.HoverFatherDamage -= 1;
                                attacked.DeadState = false;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    attacked.Owner.Player.SendUpdate(stream, MsgUpdate.Flags.HoverFather, HoverFeather.Duration, attacked.Owner.Player.HoverFatherDamage, attacked.Owner.Player.HoverFatherDamage, MsgUpdate.DataType.HoverData, true);
                                }
                                obj.Damage = 1;
                                if (attacked.Owner.Player.HoverFatherDamage == 0)
                                {
                                    attacked.Owner.Player.RemoveFlag(MsgUpdate.Flags.HoverFather);
                                }
                            }

                        }

                    }
                }
            }
            #endregion
            if (attacked.HitPoints <= obj.Damage)
            {
                attacked.DeadState = true;
              
                if (attacked.DeadState)
                {
                    if (client.Player.OnTransform)
                    {
                        if (client.Player.TransformInfo != null)
                            client.Player.TransformInfo.FinishTransform();
                    }
                    attacked.Dead(client.Player, attacked.X, attacked.Y, 0);
                }

            }
            else
            {

                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                client.UpdateQualifier(client, attacked.Owner, obj.Damage);
                if (attacked.ContainFlag(MsgUpdate.Flags.FineRain1) && attacked.FineRainPower > 0)
                {
                    if (attacked.FineRainPower >= obj.Damage)
                    {
                        attacked.FineRainPower -= (uint)obj.Damage;
                        return;
                    }
                    else
                    {
                        obj.Damage -= attacked.FineRainPower;
                        attacked.FineRainPower = 0;
                    }
                }

                attacked.HitPoints -= (int)obj.Damage;
                bool FatalBlow = true;
                #region FatalBlow
                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalBlow))
                {
                    if (Database.AtributesStatus.IsTrojan(attacked.Class))
                    {
                        if (attacked.ContainFlag(MsgUpdate.Flags.GrowFromHurt))
                        {
                            FatalBlow = false;
                        }
                    }
                }
                #endregion
                #region GrowFromHurt
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GrowFromHurt))
                {
                    if (attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.GrowFromHurt))
                    {
                        if (FatalBlow)
                        {
                            uint MaxHP = attacked.Owner.Status.MaxHitpoints;
                            attacked.Owner.Player.GrowFromHurtHitpoints = ((obj.Damage) * 10) / 100;
                            attacked.Owner.Player.GrowFromHurtHitpoints = Math.Min(30000, attacked.Owner.Player.GrowFromHurtHitpoints);
                            attacked.Owner.Status.MaxHitpoints += attacked.Owner.Player.GrowFromHurtHitpoints;
                            if (attacked.Owner.Status.MaxHitpoints > MaxHP + 30000)
                                attacked.Owner.Status.MaxHitpoints = MaxHP + 30000;
                            attacked.Owner.Player.HitPoints = (int)Math.Min((uint)attacked.Owner.Player.HitPoints + (uint)obj.Damage, (uint)attacked.Owner.Status.MaxHitpoints);
                        }
                        attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante);
                    }
                }
                #endregion

            }

            #endregion
        }
    }
}
