using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgTaskReward
    {
        public static unsafe void GetTaskReward(this ServerSockets.Packet stream, out ActionID Action, out uint dwPram, out byte Count)
        {
            Action = (ActionID)stream.ReadUInt8();//4
            dwPram = stream.ReadUInt32();//5
            Count = stream.ReadUInt8();//9
        }
        public static unsafe ServerSockets.Packet CreateTaskReward(this ServerSockets.Packet stream, ActionID Action, uint dwParam, byte Count)
        {
            stream.InitWriter();
            stream.Write((byte)Action);
            stream.Write(dwParam);
            stream.Write(Count);
            stream.Finalize(GamePackets.MsgTaskReward);
            return stream;
        }
        public static bool Contains(ServerSockets.Packet stream, uint dwParam, ushort max, Client.GameClient client)
        {
            byte unm = 1;
            if (max == 10)
                unm = 10;
            else
                unm = 1;
            if (client.Inventory.HaveSpace(unm))
            {
                if (dwParam == 0775)//FortuneWheel
                {
                    if (client.Inventory.Contain(3301169, max))
                    {
                        client.Inventory.Remove(3301169, max, stream);
                        return true;
                    }
                }
                if (dwParam == 0798)//BlissfulCP(B)Wheel
                {
                    if (client.Inventory.Contain(3320776, max))
                    {
                        client.Inventory.Remove(3320776, max, stream);
                        return true;
                    }
                }
                if (dwParam == 0947)//LunarWheel
                {
                    if (client.Inventory.Contain(3312453, max))
                    {
                        client.Inventory.Remove(3312453, max, stream);
                        return true;
                    }
                }
                if (dwParam == 0948)//AzureWheel
                {
                    if (client.Inventory.Contain(3312454, max))
                    {
                        client.Inventory.Remove(3312454, max, stream);
                        return true;
                    }
                }
                if (dwParam == 0949)//ShadowWheel
                {
                    if (client.Inventory.Contain(3312455, max))
                    {
                        client.Inventory.Remove(3312455, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3342)//SilverEgg//LuckyWheelNpc
                {
                    if (client.Inventory.Contain(3005334, max))
                    {
                        client.Inventory.Remove(3005334, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3344)//GoldenEgg//LuckyWheelNpc
                {
                    if (client.Inventory.Contain(3005333, max))
                    {
                        client.Inventory.Remove(3005333, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3345)//DiamondEgg//LuckyWheelNpc
                {
                    if (client.Inventory.Contain(3005332, max))
                    {
                        client.Inventory.Remove(3005332, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3437)//EliteMiracleStone
                {
                    if (client.Inventory.Contain(3006700, 1))
                    {
                        if (client.Player.Money >= 1000000 * (uint)max)
                        {
                            client.Player.Money -= 1000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 1,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 3438)//RareMiracleStone
                {
                    if (client.Inventory.Contain(3006576, 1))
                    {
                        if (client.Player.Money >= 5000000 * (uint)max)
                        {
                            client.Player.Money -= 5000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 5,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 3439)//SuperMiracleStone
                {
                    if (client.Inventory.Contain(3006577, 1))
                    {
                        if (client.Player.Money >= 10000000 * (uint)max)
                        {
                            client.Player.Money -= 10000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 10,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 3440)//PerfectMiracleStone
                {
                    if (client.Inventory.Contain(3006578, 1))
                    {
                        if (client.Player.Money >= 20000000 * (uint)max)
                        {
                            client.Player.Money -= 20000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 20,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 3589)//BronzeChiBead
                {
                    if (client.Inventory.Contain(3008181, 1))
                    {
                        if (client.Player.MyChi.ChiPoints >= 100 * (uint)max)
                        {
                            client.Player.MyChi.ChiPoints -= 100 * max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 100 Chi Point");
                            return false;
                        }
                    }
                }
                if (dwParam == 3590)//SilverChiBead
                {
                    if (client.Inventory.Contain(3008182, 1))
                    {
                        if (client.Player.MyChi.ChiPoints >= 500 * (uint)max)
                        {
                            client.Player.MyChi.ChiPoints -= 500 * max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 500 Chi Point");
                            return false;
                        }
                    }
                }
                if (dwParam == 3591)//GoldenChiBead
                {
                    if (client.Inventory.Contain(3008183, 1))
                    {
                        if (client.Player.MyChi.ChiPoints >= 1000 * (uint)max)
                        {
                            client.Player.MyChi.ChiPoints -= 1000 * max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 1,000 Chi Point");
                            return false;
                        }
                    }
                }
                if (dwParam == 3592)//DiamondChiBead
                {
                    if (client.Inventory.Contain(3008184, 1))
                    {
                        if (client.Player.MyChi.ChiPoints >= 2000 * (uint)max)
                        {
                            client.Player.MyChi.ChiPoints -= 2000 * max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 2,000 Chi Point");
                            return false;
                        }
                    }
                }
                if (dwParam == 3651) //ConcertedEffortPack
                {
                    if (client.Inventory.Contain(3008733, max))
                    {
                        client.Inventory.Remove(3008733, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3652) //GrandWeaponAccessoryPack
                {
                    if (client.Inventory.Contain(3008734, max))
                    {
                        client.Inventory.Remove(3008734, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3668)//EminentExploitPack
                {
                    if (client.Inventory.Contain(3600023, max))
                    {
                        client.Inventory.Remove(3600023, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3692)//CombatSupplyPack
                {
                    if (client.Inventory.Contain(3200278, max))
                    {
                        client.Inventory.Remove(3200278, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3697)//ShowhandTicket
                {
                    if (client.Inventory.Contain(3200481, max))
                    {
                        client.Inventory.Remove(3200481, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3736)//LegendaryWeaponPack
                {
                    if (client.Inventory.Contain(3300045, max))
                    {
                        client.Inventory.Remove(3300045, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3737)//ElitePKPack
                {
                    if (client.Inventory.Contain(3300049, max))
                    {
                        client.Inventory.Remove(3300049, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3749)//CPLotteryCard
                {
                    if (client.Inventory.Contain(3300657, max))
                    {
                        client.Inventory.Remove(3300657, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3767)//LuxuryWonderPack
                {
                    if (client.Inventory.Contain(3301262, max))
                    {
                        client.Inventory.Remove(3301262, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3804)//TreasureRoulette
                {
                    if (client.Inventory.Contain(3301804, max))
                    {
                        client.Inventory.Remove(3301804, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3852)//AnniversaryFloralCirclet
                {
                    if (client.Inventory.Contain(3303149, max))
                    {
                        client.Inventory.Remove(3303149, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3853)//AnniversaryFloralCirclet
                {
                    if (client.Inventory.Contain(3303116, max))
                    {
                        client.Inventory.Remove(3303116, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3881)//ArmorSoulWheel
                {
                    if (client.Inventory.Contain(3303477, max))
                    {
                        client.Inventory.Remove(3303477, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3882)//WeaponSoulWheel
                {
                    if (client.Inventory.Contain(3303478, max))
                    {
                        client.Inventory.Remove(3303478, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3883)//RefineryMaterialWheel
                {
                    if (client.Inventory.Contain(3303479, max))
                    {
                        client.Inventory.Remove(3303479, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3884)//Lv.1SilverRoulette
                {
                    if (client.Inventory.Contain(3303533, max))
                    {
                        client.Inventory.Remove(3303533, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3885)//Lv.2SilverRoulette
                {
                    if (client.Inventory.Contain(3303534, max))
                    {
                        client.Inventory.Remove(3303534, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3886)//Lv.3SilverRoulette
                {
                    if (client.Inventory.Contain(3303535, max))
                    {
                        client.Inventory.Remove(3303535, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3887)//FortuneStick
                {
                    if (client.Inventory.Contain(3303673, max))
                    {
                        client.Inventory.Remove(3303673, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3890)//Lv.1SilverRoulette
                {
                    if (client.Inventory.Contain(3303805, max))
                    {
                        client.Inventory.Remove(3303805, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3891)//Lv.2SilverRoulette
                {
                    if (client.Inventory.Contain(3303806, max))
                    {
                        client.Inventory.Remove(3303806, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3892)//Lv.3SilverRoulette
                {
                    if (client.Inventory.Contain(3303807, max))
                    {
                        client.Inventory.Remove(3303807, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3908)//Garment Wheel//3908GameHallNpc
                {
                    if (client.Player.ConquerPoints >= 180 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 180 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 3909)//CP Wheel//3909GameHallNpc
                {
                    if (client.Player.ConquerPoints >= 55 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 55 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 3910)//Stone Wheel//3910GameHallNpc
                {
                    if (client.Player.ConquerPoints >= 50 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 50 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 3911)//Chi Wheel//3911GameHallNpc
                {
                    if (client.Player.ConquerPoints >= 50 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 50 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 3912)//Star Stone Wheel//3912GameHallNpc
                {
                    if (client.Player.ConquerPoints >= 60 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 60 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 3939)//SeniorArmorSoulWheel
                {
                    if (client.Inventory.Contain(3307092, max))
                    {
                        client.Inventory.Remove(3307092, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3940)//SeniorWeaponSoulWheel
                {
                    if (client.Inventory.Contain(3307093, max))
                    {
                        client.Inventory.Remove(3307093, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3941)//SeniorRefineryMaterialWheel
                {
                    if (client.Inventory.Contain(3307094, max))
                    {
                        client.Inventory.Remove(3307094, max, stream);
                        return true;
                    }
                }
                ///////////////////////////////////////////////////////////////////
                if (dwParam == 3966)//PlayerCoinPlate
                {
                    if (client.Inventory.Contain(3308665, max))
                    {
                        client.Inventory.Remove(3308665, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3968)//WheelofPlayerCoin
                {
                    if (client.Inventory.Contain(3308666, max))
                    {
                        client.Inventory.Remove(3308666, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3970)//RuneEssencePlate
                {
                    if (client.Inventory.Contain(3308668, max))
                    {
                        client.Inventory.Remove(3308668, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3972)//WheelofRuneEssence
                {
                    if (client.Inventory.Contain(3308669, max))
                    {
                        client.Inventory.Remove(3308669, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3977)//StarStoneWheel
                {
                    if (client.Inventory.Contain(3308988, max))
                    {
                        client.Inventory.Remove(3308988, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3978)//Rune&+StoneWheel
                {
                    if (client.Inventory.Contain(3308989, max))
                    {
                        client.Inventory.Remove(3308989, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3979)//JiangHu&ChiStudyWheel
                {
                    if (client.Inventory.Contain(3308990, max))
                    {
                        client.Inventory.Remove(3308990, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3980)//TreasureWheel
                {
                    if (client.Inventory.Contain(3308991, max))
                    {
                        client.Inventory.Remove(3308991, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3982)//StoneRoulette
                {
                    if (client.Inventory.Contain(3309113, max))
                    {
                        client.Inventory.Remove(3309113, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3983)//StarStoneRoulette
                {
                    if (client.Inventory.Contain(3309114, max))
                    {
                        client.Inventory.Remove(3309114, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3984)//ChiPointRoulette
                {
                    if (client.Inventory.Contain(3309115, max))
                    {
                        client.Inventory.Remove(3309115, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3985)//CPRoulette
                {
                    if (client.Inventory.Contain(3309116, max))
                    {
                        client.Inventory.Remove(3309116, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3988)//+StonePlate
                {
                    if (client.Inventory.Contain(3304264, max))
                    {
                        client.Inventory.Remove(3304264, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3989)//Wheelof+Stone
                {
                    if (client.Inventory.Contain(3304269, max))
                    {
                        client.Inventory.Remove(3304269, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3990)//StarStonePlate
                {
                    if (client.Inventory.Contain(3304265, max))
                    {
                        client.Inventory.Remove(3304265, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3991)//WheelofStarStone
                {
                    if (client.Inventory.Contain(3304270, max))
                    {
                        client.Inventory.Remove(3304270, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3992)//ChiPlate
                {
                    if (client.Inventory.Contain(3304266, max))
                    {
                        client.Inventory.Remove(3304266, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3993)//WheelofChi
                {
                    if (client.Inventory.Contain(3304268, max))
                    {
                        client.Inventory.Remove(3304268, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3994)//CP(B)Plate
                {
                    if (client.Inventory.Contain(3304267, max))
                    {
                        client.Inventory.Remove(3304267, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3995)//WheelofCP(B)
                {
                    if (client.Inventory.Contain(3304271, max))
                    {
                        client.Inventory.Remove(3304271, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3997)//BrightStarWheel
                {
                    if (client.Inventory.Contain(3309760, max))
                    {
                        client.Inventory.Remove(3309760, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3998)//RadiantStarWheel
                {
                    if (client.Inventory.Contain(3309761, max))
                    {
                        client.Inventory.Remove(3309761, max, stream);
                        return true;
                    }
                }
                if (dwParam == 3999)//BrightStarWheel
                {
                    if (client.Inventory.Contain(3309781, max))
                    {
                        client.Inventory.Remove(3309781, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4000)//RadiantStarWheel
                {
                    if (client.Inventory.Contain(3309782, max))
                    {
                        client.Inventory.Remove(3309782, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4008)//WheelofChi
                {
                    if (client.Inventory.Contain(3311011, max))
                    {
                        client.Inventory.Remove(3311011, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4063)//Super Stone +7 Wheel//4063FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 2000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 2000 * (uint)max;
                        return true;
                    }
                    else
                    {
                        client.SendSysMesage("Sorry You dont Have Enough Cps");
                        return false;
                    }
                }
                if (dwParam == 4065)//Super Stone +8 Wheel//4065FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 5000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 5000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4067)//Super Stone+Cps Wheel//4067FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 20000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 20000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4068)//Epic Stone+Cps Wheel//4068FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 50000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 50000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4069)//Super Dragon Stone Wheel//4069FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 1000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 1000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4070)//Super Tough Drill Wheel//4070FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 1000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 1000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4071)//Super Permanent Stone Fragments Wheel//4071FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 5000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 5000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4072)//Epic Permanent Stone Wheel//4072FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 50000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 50000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4073)//Super Meteor Scroll Wheel//4073FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 500 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 500 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4074)//Super Perfection Wheel//4074FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 5000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 5000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4075)//Epic Perfection Wheel//4075FortuneWheelPromoterNpc
                {
                    if (client.Player.ConquerPoints >= 10000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 10000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4096)//16thAnnivBasicTicket
                {
                    if (client.Inventory.Contain(3320922, max))
                    {
                        client.Inventory.Remove(3320922, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4097)//16thAnnivEnhancedTicket
                {
                    if (client.Inventory.Contain(3320923, max))
                    {
                        client.Inventory.Remove(3320923, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4098)//16thAnnivPremiumTicket
                {
                    if (client.Inventory.Contain(3320924, max))
                    {
                        client.Inventory.Remove(3320924, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4126)//WheelofDragon
                {
                    if (client.Inventory.Contain(3321211, max))
                    {
                        client.Inventory.Remove(3321211, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4198)//AnimaPlate
                {
                    if (client.Inventory.Contain(3322684, max))
                    {
                        client.Inventory.Remove(3322684, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4200)//WheelofAnima
                {
                    if (client.Inventory.Contain(3322685, max))
                    {
                        client.Inventory.Remove(3322685, max, stream);
                        return true;
                    }
                }
                //dwparam--Unknown 4202-4203
                if (dwParam == 4207)//TreasureTokenWheel
                {
                    if (client.Inventory.Contain(3313390, max))
                    {
                        client.Inventory.Remove(3313390, max, stream);
                        return true;
                    }
                }
                //dwparam--Unknown 4208-4209
                if (dwParam == 4231)//CPWheel
                {
                    if (client.Inventory.Contain(3323032, max))
                    {
                        client.Inventory.Remove(3323032, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4232)//AnimaWheel
                {
                    if (client.Inventory.Contain(3323033, max))
                    {
                        client.Inventory.Remove(3323033, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4233)//CP(B)Wheel
                {
                    if (client.Inventory.Contain(3323040, max))
                    {
                        client.Inventory.Remove(3323040, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4234)//TreasureTokenWheel
                {
                    if (client.Inventory.Contain(3323046, max))
                    {
                        client.Inventory.Remove(3323046, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4235)//+StoneWheel
                {
                    if (client.Inventory.Contain(3313535, max))
                    {
                        client.Inventory.Remove(3313535, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4236)//StarStoneWheel
                {
                    if (client.Inventory.Contain(3313536, max))
                    {
                        client.Inventory.Remove(3313536, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4237)//ChiWheel
                {
                    if (client.Inventory.Contain(3313537, max))
                    {
                        client.Inventory.Remove(3313537, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4238)//CPs(B)Wheel
                {
                    if (client.Inventory.Contain(3313538, max))
                    {
                        client.Inventory.Remove(3313538, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4259)//PrivilegeWheel
                {
                    if (client.Inventory.Contain(3314383, max))
                    {
                        client.Inventory.Remove(3314383, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4262)//AccessoryWheel
                {
                    if (client.Inventory.Contain(3323481, max))
                    {
                        client.Inventory.Remove(3323481, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4290)//FortuneStyleWheel
                {
                    uint amount = 0;
                    amount = (uint)Pool.GetRandom.Next(4499, 5999);
                    if (client.Player.ConquerPoints >= amount * (uint)max)
                    {
                        client.Player.ConquerPoints -= amount * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4294)//LuckyReturnWheel
                {
                    if (client.Inventory.Contain(3316026, max))
                    {
                        client.Inventory.Remove(3316026, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4300)//P9AnimaWheel
                {
                    if (client.Inventory.Contain(3327447, max))
                    {
                        client.Inventory.Remove(3327447, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4371)//P11AnimaWheel
                {
                    if (client.Inventory.Contain(3327448, max))
                    {
                        client.Inventory.Remove(3327448, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4391)//BonusWheel
                {
                    if (client.Inventory.Contain(3329940, max))
                    {
                        client.Inventory.Remove(3329940, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4399)//FlowerFortuneWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4400)//SplendidWingsWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4401)//ShinyStarStoneWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4402)//MajesticAnimaWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 4479)//HomeCP(B)Wheel
                {
                    if (client.Inventory.Contain(3330502, max))
                    {
                        client.Inventory.Remove(3330502, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4487)//LuckyReturnWheel
                {
                    if (client.Inventory.Contain(3314794, max))
                    {
                        client.Inventory.Remove(3314794, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4499)//SummerFruitWheel
                {
                    if (client.Inventory.Contain(3316235, max))
                    {
                        client.Inventory.Remove(3316235, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4503)//NinpoScrollWheel
                {
                    if (client.Inventory.Contain(3331832, max))
                    {
                        client.Inventory.Remove(3331832, max, stream);
                        return true;
                    }
                }
                if (dwParam == 4504)//WeaponArchiveWheel
                {
                    if (client.Inventory.Contain(3331833, max))
                    {
                        client.Inventory.Remove(3331833, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6801)//LuckyAnniversaryPack
                {
                    if (client.Inventory.Contain(3303396, max))
                    {
                        client.Inventory.Remove(3303396, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6802)//PleasantAnniversaryPack
                {
                    if (client.Inventory.Contain(3303397, max))
                    {
                        client.Inventory.Remove(3303397, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6811)//FortuneWheel
                {
                    if (client.Player.ConquerPoints >= 27 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 27 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6813)//P1StarWheel
                {
                    if (client.Player.ConquerPoints >= 23 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 23 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6814)//P2StarWheel
                {
                    if (client.Player.ConquerPoints >= 75 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 75 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6815)//P3StarWheel
                {
                    if (client.Player.ConquerPoints >= 750 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 750 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6838)//PokerTokenFragmentsPack
                {
                    if (client.Inventory.Contain(3305483, max))
                    {
                        client.Inventory.Remove(3305483, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6840)//ThanksgivingStoneWheel
                {
                    if (client.Inventory.Contain(3305784, max))
                    {
                        client.Inventory.Remove(3305784, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6841)//ThanksgivingStarWheel
                {
                    if (client.Inventory.Contain(3305785, max))
                    {
                        client.Inventory.Remove(3305785, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6842)//ThanksgivingChiWheel
                {
                    if (client.Inventory.Contain(3305786, max))
                    {
                        client.Inventory.Remove(3305786, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6843)//ThanksgivingCP(B)Wheel
                {
                    if (client.Inventory.Contain(3305787, max))
                    {
                        client.Inventory.Remove(3305787, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6844)//PowerBoostWheel
                {
                    if (client.Inventory.Contain(3306071, max))
                    {
                        client.Inventory.Remove(3306071, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6845)//3-StarGarmentWheel
                {
                    if (client.Inventory.Contain(3306072, max))
                    {
                        client.Inventory.Remove(3306072, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6846)//4-StarGarmentWheel
                {
                    if (client.Inventory.Contain(3306073, max))
                    {
                        client.Inventory.Remove(3306073, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6847)//GearForgingWheel
                {
                    if (client.Inventory.Contain(3306076, max))
                    {
                        client.Inventory.Remove(3306076, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6849)//GearForgingWheel
                {
                    if (client.Inventory.Contain(3306108, max))
                    {
                        client.Inventory.Remove(3306108, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6850)//GearForgingWheel
                {
                    if (client.Inventory.Contain(3306109, max))
                    {
                        client.Inventory.Remove(3306109, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6851)//GearForgingWheel
                {
                    if (client.Inventory.Contain(3306131, max))
                    {
                        client.Inventory.Remove(3306131, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6871)//EpicWheelofPirate
                {
                    if (client.Inventory.Contain(3307820, max))
                    {
                        client.Inventory.Remove(3307820, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6873)//FortuneKey
                {
                    if (client.Inventory.Contain(3307796, max))
                    {
                        client.Inventory.Remove(3307796, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6888)//RefinedCPRoulette
                {
                    if (client.Player.ConquerPoints >= 100 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 100 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6889)//UniqueCPRoulette
                {
                    if (client.Player.ConquerPoints >= 500 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 500 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6890)//EliteCPRoulette
                {
                    if (client.Player.ConquerPoints >= 3000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 3000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6891)//SuperCPRoulette
                {
                    if (client.Player.ConquerPoints >= 5000 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 5000 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6892)//DrillWheel
                {
                    if (client.Inventory.Contain(3310741, max))
                    {
                        client.Inventory.Remove(3310741, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6902)//CarnivalWheel
                {
                    if (client.Inventory.Contain(3311122, max))
                    {
                        client.Inventory.Remove(3311122, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6911)//10CPsWheel
                {
                    if (client.Inventory.Contain(3319169, max))
                    {
                        client.Inventory.Remove(3319169, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6912)//100CPsWheel
                {
                    if (client.Inventory.Contain(3319176, max))
                    {
                        client.Inventory.Remove(3319176, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6913)//1000CPsWheel
                {
                    if (client.Inventory.Contain(3319216, max))
                    {
                        client.Inventory.Remove(3319216, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6915)//EliteMiracleStone
                {
                    if (client.Inventory.Contain(3319355, 1))
                    {
                        if (client.Player.Money >= 1000000 * (uint)max)
                        {
                            client.Player.Money -= 1000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 1,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 6916)//RareMiracleStone
                {
                    if (client.Inventory.Contain(3319356, 1))
                    {
                        if (client.Player.Money >= 5000000 * (uint)max)
                        {
                            client.Player.Money -= 5000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 5,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 6917)//SuperMiracleStone
                {
                    if (client.Inventory.Contain(3319357, 1))
                    {
                        if (client.Player.Money >= 10000000 * (uint)max)
                        {
                            client.Player.Money -= 10000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 10,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 6918)//PerfectMiracleStone
                {
                    if (client.Inventory.Contain(3319358, 1))
                    {
                        if (client.Player.Money >= 20000000 * (uint)max)
                        {
                            client.Player.Money -= 20000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 20,000,000 Silver");
                            return false;
                        }
                    }
                }
                if (dwParam == 6943)//DelicateSilverWheel
                {
                    if (client.Inventory.Contain(3320157, max))
                    {
                        client.Inventory.Remove(3320157, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6944)//LuxurySilverWheel
                {
                    if (client.Inventory.Contain(3320158, max))
                    {
                        client.Inventory.Remove(3320158, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6945)//GrandSilverWheel
                {
                    if (client.Inventory.Contain(3320159, max))
                    {
                        client.Inventory.Remove(3320159, max, stream);
                        return true;
                    }
                }
                if (dwParam == 6980)//BlissfulCPWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 6981)//BlissfulAnimaWheel
                {
                    if (client.Player.ConquerPoints >= 99 * (uint)max)
                    {
                        client.Player.ConquerPoints -= 99 * (uint)max;
                        return true;
                    }
                }
                if (dwParam == 7033)//PrimaryAnimaRoulette.
                {
                    if (client.Inventory.Contain(3321215, max))
                    {
                        client.Inventory.Remove(3321215, max, stream);
                        return true;
                    }
                }
                if (dwParam == 7034)//MediumAnimaRoulette.
                {
                    if (client.Inventory.Contain(3321216, max))
                    {
                        client.Inventory.Remove(3321216, max, stream);
                        return true;
                    }
                }
                if (dwParam == 7035)//AdvancedAnimaRoulette.
                {
                    if (client.Inventory.Contain(3321217, max))
                    {
                        client.Inventory.Remove(3321217, max, stream);
                        return true;
                    }
                }
                if (dwParam == 7038)//LuxuryCashWheel
                {
                    if (client.Inventory.Contain(3312755, max))
                    {
                        client.Inventory.Remove(3312755, max, stream);
                        return true;
                    }
                }
                if (dwParam == 7048)//FortuneWheel
                {
                    if (client.Inventory.Contain(3322480, 1))
                    {
                        if (client.Player.Money >= 3000000 * (uint)max)
                        {
                            client.Player.Money -= 3000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 3,000,000 Silver");
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        public enum ActionID : byte
        {
            Open = 0,
            SetReward = 1,
            Draw = 2,
            Claim = 3,
            Redraw = 4,
            Continue = 5,
            Times10Spinning = 7
        }
        [Packet(GamePackets.MsgTaskReward)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            {
                user.CreateBoxDialog("Sorry, This item is Now Closed.");
                return;
            }
            ActionID Action;
            uint dwParam;
            byte Count;
            stream.GetTaskReward(out Action, out dwParam, out Count);
            switch (Action)
            {
                case ActionID.Times10Spinning:
                    {
                        {
                            user.CreateBoxDialog("Sorry, This item is Now Closed.");
                            break;
                        }
                        if (!user.Inventory.HaveSpace(10))
                        {
                            user.SendSysMesage("Please make 10 more spaces in your inventory.");
                            break;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                        {
                            if (MsgTaskReward.Contains(stream, (uint)dwParam, 10, user))
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    user.Player.TaskRewardIndex = (uint)Pool.GetRandom.Next(0, Rewards.Count);
                                    user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                    user.Player.Times10.Add((byte)user.Player.TaskRewardIndex);
                                    user.Inventory.AddItemWitchStack(user.Player.TaskReward, 0, 1, stream, false);
                                    user.Send(stream.CreateTaskReward(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                                    user.Player.TaskReward = 0;
                                }
                            }
                            else
                            {
                                user.SendSysMesage("You don't have 10 of The Same Items in You`r Inventory");
                                return;
                            }
                        }
                        var Items = user.Player.Times10.ToArray();
                        var ten = new MsgTenTimesLotteryReward.TenTimesLotteryReward();
                        ten.ID = new uint[] { Items[0], Items[1], Items[2], Items[3], Items[4], Items[5], Items[6], Items[7], Items[8], Items[9] };
                        user.Send(stream.CreateTenTimesLotteryReward(ten));
                        user.Player.Times10.Clear();
                        break;
                    }
                case ActionID.Continue:
                case ActionID.Draw:
                    {
                        {
                            user.CreateBoxDialog("Sorry, This item is Now Closed.");
                            break;
                        }
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.SendSysMesage("Please make 1 more space in your inventory.");
                            break;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                        {
                            if (MsgTaskReward.Contains(stream, (uint)dwParam, 1, user))
                            {
                                user.Player.TaskRewardIndex = (uint)Pool.GetRandom.Next(0, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Send(stream.CreateTaskReward(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                            }
                            else
                            {
                                user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                            }
                            break;
                        }
                        else
                        {
                            user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                        }
                        break;
                    }
                case ActionID.Claim:
                    {
                        {
                            user.CreateBoxDialog("Sorry, This item is Now Closed.");
                            break;
                        }
                        if (user.Player.TaskReward != 0)
                        {
                            user.SendSysMesage("You received a new item from the roulette prize.", MsgMessage.ChatMode.System);
                            user.Inventory.AddItemWitchStack(user.Player.TaskReward, 0, 1, stream, false);
                            user.Player.TaskReward = 0;
                        }
                        break;
                    }
                case ActionID.Redraw:
                    {
                        {
                            user.CreateBoxDialog("Sorry, This item is Now Closed.");
                            break;
                        }
                        if (user.Player.TaskReward != 0)
                        {
                            if (!user.Inventory.HaveSpace(1))
                            {
                                user.SendSysMesage("Please make 1 more space in your inventory.");
                                return;
                            }
                            user.SendSysMesage("You received a new item from the roulette prize.", MsgMessage.ChatMode.System);
                            user.Inventory.AddItemWitchStack(user.Player.TaskReward, 0, 1, stream, false);
                            user.Player.TaskReward = 0;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                        {
                            if (MsgTaskReward.Contains(stream, (uint)dwParam, 1, user))
                            {
                                user.Player.TaskRewardIndex = (uint)Pool.GetRandom.Next(0, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Send(stream.CreateTaskReward(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                            }
                            else
                            {
                                user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                            }
                            break;
                        }
                        else
                        {
                            user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                        }
                        break;
                    }
            }
        }
    }
}
