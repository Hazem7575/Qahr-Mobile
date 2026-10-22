namespace Qahr.Mobile.Database
{
    public class LotteryTable
    {
        public System.Collections.Generic.List<uint> NULLGETITEMS = new System.Collections.Generic.List<uint>() { };

        public uint[] GenerateLotteryPrize(byte ID, Client.GameClient client)
        {
        again:
            uint UID = 0;
            byte count = 0;
            uint rand = 0;
            switch (ID)
            {
                case 1: // ( Celestial Isle ) 
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            rand = 8;
                            UID = (uint)Pool.RandFromGivingNums(3303166, 3303162, 3303173, 723859, 723856, 723855, 1060039, 3303164, 3303172, 3307146, 723725, 723727); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(57.4))
                        {
                            rand = 7;
                            UID = (uint)Pool.RandFromGivingNums(3303158, 3303163, 3000120, 3001411, 722136, 1088000, 3005598); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 6;
                            UID = (uint)Pool.RandFromGivingNums(3303174, 3303154, 3000121, 3000123, 3303159, 3303175, 3001412, 3000122, 3001413, 3303168, 3303177, 3303155, 3200744, 753099, 752099, 751099, 3322219); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 5;
                            UID = (uint)Pool.RandFromGivingNums(3001036, 3303160, 3303176, 3303180, 3303156, 3200739, 3303179, 3200740, 3303167, 3001035);  // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 4;
                            UID = (uint)Pool.RandFromGivingNums(3200741, 3303178, 3303161, 3303157); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 3;
                            UID = (uint)Pool.RandFromGivingNums(3303165, 3303181, 753999, 752999, 751999, 3001034, 3200728, 3303182); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 2;
                            UID = (uint)Pool.RandFromGivingNums(3303204, 3303183, 3323507, 3200743, 3323507, 3323507, 3323507); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 1;
                            UID = (uint)Pool.RandFromGivingNums(3303213, 3303184, 3303170); // All Prize Conquer Online ( Done )
                        }
                        else
                            goto again;
                        break;
                    }
                case 2: // ( Jade Cave )
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            rand = 8;
                            UID = (uint)Pool.RandFromGivingNums(3303166, 3303162, 3303173, 723859, 723856, 723855, 1060039, 3303164, 3303172, 3307146, 723725, 723727); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(57.4))
                        {
                            rand = 7;
                            UID = (uint)Pool.RandFromGivingNums(3303158, 3303163, 3000120, 3001411, 722136, 1088000, 3005598); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 6;
                            UID = (uint)Pool.RandFromGivingNums(3303174, 3303154, 3000121, 3000123, 3303159, 3303175, 3001412, 3000122, 3001413, 3303168, 3303177, 3303155, 3200744, 753099, 752099, 751099, 3322219); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 5;
                            UID = (uint)Pool.RandFromGivingNums(3001036, 3303160, 3303176, 3303180, 3303156, 3200739, 3303179, 3200740, 3303167, 3001035);  // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 4;
                            UID = (uint)Pool.RandFromGivingNums(3200741, 3303178, 3303161, 3303157); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 3;
                            UID = (uint)Pool.RandFromGivingNums(3303165, 3303181, 753999, 752999, 751999, 3001034, 3200728, 3303182); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 2;
                            UID = (uint)Pool.RandFromGivingNums(3303204, 3303183, 3323507, 3200743, 3323507, 3323507, 3323507); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 1;
                            UID = (uint)Pool.RandFromGivingNums(3303213, 3303184, 3303170); // All Prize Conquer Online ( Done )
                        }
                        else
                            goto again;
                        break;
                    }
                case 3:  // ( Meteor Cliff )
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            rand = 8;
                            UID = (uint)Pool.RandFromGivingNums(3303166, 3303162, 3303173, 723859, 723856, 723855, 1060039, 3303164, 3303172, 3307146, 723725, 723727); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(57.4))
                        {
                            rand = 7;
                            UID = (uint)Pool.RandFromGivingNums(3303158, 3303163, 3000120, 3001411, 722136, 1088000, 3005598); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 6;
                            UID = (uint)Pool.RandFromGivingNums(3303174, 3303154, 3000121, 3000123, 3303159, 3303175, 3001412, 3000122, 3001413, 3303168, 3303177, 3303155, 3200744, 753099, 752099, 751099, 3322219); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 5;
                            UID = (uint)Pool.RandFromGivingNums(3001036, 3303160, 3303176, 3303180, 3303156, 3200739, 3303179, 3200740, 3303167, 3001035);  // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 4;
                            UID = (uint)Pool.RandFromGivingNums(3200741, 3303178, 3303161, 3303157); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 3;
                            UID = (uint)Pool.RandFromGivingNums(3303165, 3303181, 753999, 752999, 751999, 3001034, 3200728, 3303182); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 2;
                            UID = (uint)Pool.RandFromGivingNums(3303204, 3303183, 3323507, 3200743, 3323507, 3323507, 3323507); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 1;
                            UID = (uint)Pool.RandFromGivingNums(3303213, 3303184, 3303170); // All Prize Conquer Online ( Done )
                        }
                        else
                            goto again;
                        break;
                    }
                case 4: // ( Pure School )
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            rand = 8;
                            UID = (uint)Pool.RandFromGivingNums(3303166, 3303162, 3303173, 723859, 723856, 723855, 1060039, 3303164, 3303172, 3307146, 723725, 723727); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(57.4))
                        {
                            rand = 7;
                            UID = (uint)Pool.RandFromGivingNums(3303158, 3303163, 3000120, 3001411, 722136, 1088000, 3005598); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 6;
                            UID = (uint)Pool.RandFromGivingNums(3303174, 3303154, 3000121, 3000123, 3303159, 3303175, 3001412, 3000122, 3001413, 3303168, 3303177, 3303155, 3200744, 753099, 752099, 751099, 3322219); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 5;
                            UID = (uint)Pool.RandFromGivingNums(3001036, 3303160, 3303176, 3303180, 3303156, 3200739, 3303179, 3200740, 3303167, 3001035);  // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 4;
                            UID = (uint)Pool.RandFromGivingNums(3200741, 3303178, 3303161, 3303157); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 3;
                            UID = (uint)Pool.RandFromGivingNums(3303165, 3303181, 753999, 752999, 751999, 3001034, 3200728, 3303182); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 2;
                            UID = (uint)Pool.RandFromGivingNums(3303204, 3303183, 3323507, 3200743, 3323507, 3323507, 3323507); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 1;
                            UID = (uint)Pool.RandFromGivingNums(3303213, 3303184, 3303170); // All Prize Conquer Online ( Done )
                        }
                        else
                            goto again;
                        break;
                    }
                case 5: // ( Wisdom Hall )
                    {
                        if (Role.Core.Rate(57.4))
                        {
                            rand = 8;
                            UID = (uint)Pool.RandFromGivingNums(3303166, 3303162, 3303173, 723859, 723856, 723855, 1060039, 3303164, 3303172, 3307146, 723725, 723727); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(57.4))
                        {
                            rand = 7;
                            UID = (uint)Pool.RandFromGivingNums(3303158, 3303163, 3000120, 3001411, 722136, 1088000, 3005598); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 6;
                            UID = (uint)Pool.RandFromGivingNums(3303174, 3303154, 3000121, 3000123, 3303159, 3303175, 3001412, 3000122, 3001413, 3303168, 3303177, 3303155, 3200744, 753099, 752099, 751099, 3322219); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(4.9))
                        {
                            rand = 5;
                            UID = (uint)Pool.RandFromGivingNums(3001036, 3303160, 3303176, 3303180, 3303156, 3200739, 3303179, 3200740, 3303167, 3001035);  // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 4;
                            UID = (uint)Pool.RandFromGivingNums(3200741, 3303178, 3303161, 3303157); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.2))
                        {
                            rand = 3;
                            UID = (uint)Pool.RandFromGivingNums(3303165, 3303181, 753999, 752999, 751999, 3001034, 3200728, 3303182); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 2;
                            UID = (uint)Pool.RandFromGivingNums(3303204, 3303183, 3323507, 3200743, 3323507, 3323507, 3323507); // All Prize Conquer Online ( Done )
                        }
                        else if (Role.Core.Rate(0.1))
                        {
                            rand = 1;
                            UID = (uint)Pool.RandFromGivingNums(3303213, 3303184, 3303170); // All Prize Conquer Online ( Done )
                        }
                        else
                            goto again;
                        break;
                    }
                case 11: // ( Sky Rift )
                    {
                        client.CreateBoxDialog("Sorry, This System Is Closed.");
                        break;
                    }
                    //    if (Role.Core.Rate(57.4))
                    //    {
                    //        rand = 8;
                    //        UID = (uint)Pool.RandFromGivingNums(3316698, 3316699, 3330865, 3316701, 3331667, 3325756);
                    //    }
                    //    else if (Role.Core.Rate(57.4))
                    //    {
                    //        rand = 7;
                    //        UID = (uint)Pool.RandFromGivingNums(3316704, 3325756, 3316706, 3316707, 3316708, 3316709);
                    //    }
                    //    else if (Role.Core.Rate(4.9))
                    //    {
                    //        rand = 6;
                    //        UID = (uint)Pool.RandFromGivingNums(3316710, 3316711, 3316712, 3316713, 3316714, 3316715, 3325980, 3390018, 3316718, 3316719, 3316720);
                    //    }
                    //    else if (Role.Core.Rate(4.9))
                    //    {
                    //        rand = 5;
                    //        UID = (uint)Pool.RandFromGivingNums(3316721, 3316722, 3316723, 3316724, 3316725, 3316726, 3325980, 3316728, 3316729, 3316730, 3316731, 3390019);
                    //    }
                    //    else if (Role.Core.Rate(0.2))
                    //    {
                    //        rand = 4;
                    //        UID = (uint)Pool.RandFromGivingNums(3316732, 3316733, 3316734, 3325989, 3316738, 3316739, 3316740);
                    //    }
                    //    else if (Role.Core.Rate(0.2))
                    //    {
                    //        rand = 3;
                    //        UID = (uint)Pool.RandFromGivingNums(3316741, 3316742, 3316743, 3318571, 3314246, 3325989);
                    //    }
                    //    else if (Role.Core.Rate(0.1))
                    //    {
                    //        rand = 2;
                    //        UID = (uint)Pool.RandFromGivingNums(3314245, 3325992, 3316749, 3316750, 3316751);
                    //    }
                    //    else if (Role.Core.Rate(0.1))
                    //    {
                    //        rand = 1;
                    //        UID = (uint)Pool.RandFromGivingNums(3303170, 3318565, 3318566, 3318567, 3318568, 3326002);
                    //    }
                    //    else
                    //        goto again;
                    //    break;
                    //}
            }
            if (count >= 2)
            {
                ID = (byte)Pool.LiteRandom.Next(1, 5);
                count = 0;
                goto again;
            }
            else if (ID == 0)
            {
                ID = client.Player.GateID;
                count++;
                goto again;
            }
            else
            {
                uint[] matrix = new uint[2];
                matrix[0] = UID;
                matrix[1] = rand;
                return matrix;
            }
        }

        public int LotteryEntry(byte vipLevel)
        {
            return int.MaxValue;
        }

        public Game.MsgServer.MsgGameItem CreateGameItem(uint ITEM_ID)
        {
            var GameItem = new Game.MsgServer.MsgGameItem();
            GameItem.ITEM_ID = ITEM_ID;
            GameItem.Color = (Role.Flags.Color)(byte)Pool.GetRandom.Next(4, 8);
            GameItem.UID = Pool.ITEM_Counter.Next;
            ItemType.DBItem DBItem = null;
            if (Pool.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
            {
                GameItem.Durability = GameItem.MaximDurability = DBItem.Durability;
            }
            return GameItem;
        }
    }
}