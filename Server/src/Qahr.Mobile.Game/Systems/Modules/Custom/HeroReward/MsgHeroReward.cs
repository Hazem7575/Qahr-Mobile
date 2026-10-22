using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetHeroReward(this ServerSockets.Packet stream, out MsgHeroReward.ActionID mode, out uint Prize)
        {
            mode = (MsgHeroReward.ActionID)stream.ReadUInt8();
            Prize = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet HeroRewardCreate(this ServerSockets.Packet stream, MsgHeroReward.ActionID mode, uint Prize)
        {
            stream.InitWriter();

            stream.Write((byte)mode);
            stream.Write(Prize);

            stream.Finalize(GamePackets.MsgProcessGoalTaskOpt);

            return stream;
        }

        public static unsafe ServerSockets.Packet HeroRewardlistCreate(this ServerSockets.Packet stream, uint dwParam, byte AllDone, Role.Instance.HeroRewards.Item[] array)
        {
            stream.InitWriter();

            stream.Write(dwParam);
            stream.Write(AllDone);
            if (array != null)
                stream.Write((ushort)array.Length);
            else
                stream.Write((ushort)0);

            if (array != null)
            {
                for (int x = 0; x < array.Length; x++)
                {
                    stream.Write(array[x].UID);
                    stream.Write(array[x].UnKnow);
                    stream.Write(array[x].Claim);
                }
            }

            stream.Finalize(GamePackets.MsgProcessGoalTask);

            return stream;
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MsgHeroReward
    {
        public enum ActionID : byte
        {
            Show = 0,
            ClaimPrize = 1,
            ClaimSubPrize = 2
        }


        [PacketAttribute(GamePackets.MsgProcessGoalTaskOpt)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgHeroReward.ActionID mode;
            uint Prize;

            stream.GetHeroReward(out mode, out Prize);

            if (user.InTrade)
                return;
            
            if (user.Player.Map == 3852)
                return;
            switch (mode)
            {
                case ActionID.Show:
                    {
                        user.CreateBoxDialog("Sorry, the Hero Biography System is now closed.");
                        break;
                        Prize = (uint)Math.Min(Prize, 11);
                        var DBInfo = Pool.TableHeroRewards[Prize];

                        switch (Prize)
                        {
                            #region Novice
                            case 1:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        if (user.Player.Level >= 20)
                                            user.HeroRewards.AddGoal(101);

                                        if (!user.HeroRewards.ContainGoal(102))
                                        {
                                            int count = 0;
                                            foreach (var item in user.Equipment.CurentEquip)
                                            {
                                                if (item.ITEM_ID % 10 >= 6)
                                                    count++;
                                            }
                                            if (count >= 6)
                                                user.HeroRewards.AddGoal(102);
                                        }

                                        //103 add on xp spell count in player.cs

                                        if (user.Player.Class % 10 >= 1)
                                        {
                                            user.HeroRewards.AddGoal(104);
                                            user.HeroRewards.AddGoal(105);
                                        }
                                    }
                                    break;
                                }
                            #endregion
                            #region Rising Star
                            case 2:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        if (user.Player.Level >= 50)
                                            user.HeroRewards.AddGoal(201);

                                        if (!user.HeroRewards.ContainGoal(202))
                                        {
                                            int count = 0;
                                            foreach (var item in user.Equipment.CurentEquip)
                                            {
                                                if (item.ITEM_ID % 10 >= 7)
                                                    count++;
                                            }
                                            if (count >= 6)
                                                user.HeroRewards.AddGoal(202);
                                        }

                                        //203 Make/Join a Team
                                        //204 Win a fight in Arena.
                                        //205 Upgrade equipment with Meteor, once.
                                        //206 Try a bottle of EXP potion,and gain 60 minutes' double EXP time.

                                        if (!user.HeroRewards.ContainGoal(207))
                                        {
                                            if (!user.Equipment.FreeEquip(Role.Flags.ConquerItem.Steed))
                                                user.HeroRewards.AddGoal(207);
                                        }

                                    }
                                    break;
                                }
                            #endregion
                            #region Expert
                            case 3:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        if (user.Player.Level >= 80)
                                            user.HeroRewards.AddGoal(301);

                                        if (!user.HeroRewards.ContainGoal(302))
                                        {
                                            int count = 0;
                                            foreach (var item in user.Equipment.CurentEquip)
                                            {
                                                if (item.ITEM_ID % 10 >= 8)
                                                    count++;
                                            }
                                            if (count >= 6)
                                                user.HeroRewards.AddGoal(302);
                                        }

                                        if (user.Player.GuildID != 0)
                                            user.HeroRewards.AddGoal(303);//303 Create/join a guild.

                                        if (user.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Friends))
                                            if (user.Player.Associate.Associat[Role.Instance.Associate.Friends].Count >= 5)
                                                user.HeroRewards.AddGoal(304); //304 Add at least 5 friends.

                                        //305 Win a battle in Team Qualifier.
                                        //306 Play lottery, once.


                                    }
                                    break;
                                }
                            #endregion
                            #region Standout
                            case 4:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        if (user.Player.Level >= 110)
                                            user.HeroRewards.AddGoal(401);

                                        if (!user.HeroRewards.ContainGoal(402))
                                        {
                                            int count = 0;
                                            foreach (var item in user.Equipment.CurentEquip)
                                            {
                                                if (item.ITEM_ID % 10 >= 9)
                                                    count++;
                                            }
                                            if (count >= 6)
                                                user.HeroRewards.AddGoal(402);
                                        }

                                        //403 Compose equipment with Ú or minor equipment, once.
                                        //404 Go to Ape City and take the challenge of `Dis City`, once.
                                        //405 Compete in the Elite PK Tournament, once.
                                        //406 Compete in the Team PK Tournament, once.
                                        //407 Compete in the Skill Team PK Tournament, once.
                                        if (!user.HeroRewards.ContainGoal(408))
                                        {
                                            int supertalismans = 0;
                                            foreach (var item in user.Equipment.CurentEquip)
                                            {
                                                if (item.Position == (ushort)Role.Flags.ConquerItem.Tower || item.Position == (ushort)Role.Flags.ConquerItem.Fan)
                                                {
                                                    if (item.ITEM_ID % 10 >= 9)
                                                        supertalismans++;
                                                }
                                            }
                                            if (supertalismans == 2)
                                                user.HeroRewards.AddGoal(408);
                                        }
                                        //409 Complete all Daily Quests, once.
                                    }
                                    break;
                                }
                            #endregion
                            #region Famous
                            case 5:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        if (user.Player.Reborn >= 1)
                                            user.HeroRewards.AddGoal(501);

                                        if (!user.HeroRewards.ContainGoal(502))
                                        {

                                            if (user.Equipment.Exist(p => p.SocketOne != Role.Flags.Gem.NoSocket || p.SocketTwo != Role.Flags.Gem.EmptySocket, 1))
                                                user.HeroRewards.AddGoal(502);
                                        }

                                        if (!user.HeroRewards.ContainGoal(503))
                                        {
                                            if (user.Equipment.Exist(p => p.Plus > 1))
                                                user.HeroRewards.AddGoal(503);
                                        }

                                        if (!user.HeroRewards.ContainGoal(504))
                                        {
                                            if (user.Player.SubClass != null)
                                            {
                                                if (user.Player.SubClass.src.Count > 0)
                                                    user.HeroRewards.AddGoal(504);
                                            }
                                        }
                                        //505 Join the Champion`s Arena, once.
                                        //506 Win 20 fights in Arena.
                                        //507 Win 20 battles in the Team Qualifier.

                                    }
                                    break;
                                }
                            #endregion
                            #region Hero
                            case 6:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {

                                        if (!user.HeroRewards.ContainGoal(601))
                                        {
                                            if (user.Equipment.Exist(p => p.SocketOne != Role.Flags.Gem.NoSocket || p.SocketTwo != Role.Flags.Gem.EmptySocket, 3))
                                                user.HeroRewards.AddGoal(601);
                                        }

                                        if (user.Equipment.BattlePower >= 15)
                                            user.HeroRewards.AddGoal(602);


                                        if (!user.HeroRewards.ContainGoal(603))
                                        {
                                            if (user.Equipment.Exist(p => (byte)p.SocketOne % 10 == 3 || (byte)p.SocketTwo % 10 == 3, 1))
                                                user.HeroRewards.AddGoal(603);
                                        }

                                        if (!user.HeroRewards.ContainGoal(604))//604 A random equipped gear has been purified with P1 Dragon Soul.
                                        {
                                            if (user.Equipment.Exist(p => p.Purification.PurificationLevel >= 1))
                                                user.HeroRewards.AddGoal(604);
                                        }

                                        if (!user.HeroRewards.ContainGoal(605))  //605 Unlock Chi system. Total Chi score reaches 500.
                                        {
                                            if (user.Player.MyChi != null)
                                            {
                                                if (user.Player.MyChi.AllScore() >= 500)
                                                    user.HeroRewards.AddGoal(605);
                                            }
                                        }
                                        //606 Enlighten the younger, once.
                                        //607 Join Guild PK Tournament, once.
                                        if (!user.HeroRewards.ContainGoal(608)) //608 Join 2 Sub-classes.
                                        {
                                            if (user.Player.SubClass != null)
                                            {
                                                if (user.Player.SubClass.src.Count >= 2)
                                                    user.HeroRewards.AddGoal(608);
                                            }
                                        }
                                        //609 Sign up for CTF at the Guild Controller, once.

                                    }

                                    break;
                                }
                            #endregion
                            #region Super Hero
                            case 7:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {

                                        if (user.Player.Reborn >= 2)
                                            user.HeroRewards.AddGoal(701);

                                        if (!user.HeroRewards.ContainGoal(702))  //605 Unlock Chi system. Total Chi score reaches 1300.
                                        {
                                            if (user.Player.MyChi != null)
                                            {
                                                if (user.Player.MyChi.AllScore() >= 1300)
                                                    user.HeroRewards.AddGoal(702);
                                            }
                                        }
                                        if (!user.HeroRewards.ContainGoal(703))//Total Jiang Hu score reaches 10,000.
                                        {
                                            if (user.Player.MyJiangHu != null)
                                            {
                                                if (user.Player.MyJiangHu.Inner_Strength > 10000)
                                                    user.HeroRewards.AddGoal(703);
                                            }
                                        }

                                        //704 Buy a house.

                                        if (!user.HeroRewards.ComplateFullStage(705))
                                        {
                                            if (user.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                                                if (user.Player.Associate.Associat[Role.Instance.Associate.Apprentice].Count >= 1)
                                                    user.HeroRewards.AddGoal(705);     //705 Take an apprentice
                                        }

                                        if (user.Equipment.BattlePower >= 50)//Total bonus level of all equipped gears reaches 50.
                                            user.HeroRewards.AddGoal(706);


                                        if (!user.HeroRewards.ContainGoal(707))  //707 All equipped gears have totally 8 embeded Super Gems.
                                        {
                                            if (user.Equipment.Exist(p => (byte)p.SocketOne % 10 == 3 || (byte)p.SocketTwo % 10 == 3, 4))
                                                user.HeroRewards.AddGoal(707);
                                        }


                                        if (!user.HeroRewards.ContainGoal(708))  //708 All equipped gears have totally 8 embeded Gems.
                                        {

                                            if (user.Equipment.Exist(p => p.SocketOne != Role.Flags.Gem.NoSocket && p.SocketOne != Role.Flags.Gem.EmptySocket
                                       && p.SocketTwo != Role.Flags.Gem.EmptySocket && p.SocketTwo != Role.Flags.Gem.NoSocket, 4))
                                                user.HeroRewards.AddGoal(708);
                                        }

                                        //709 Kill Snow Banshee or Terato Dragon, once.

                                        if (user.Player.Spouse != "None")//710 Marry the beloved.
                                            user.HeroRewards.AddGoal(710);

                                        if (!user.HeroRewards.ContainGoal(711)) //711 Join 3 sub-classes.
                                        {
                                            if (user.Player.SubClass != null)
                                            {
                                                if (user.Player.SubClass.src.Count >= 3)
                                                    user.HeroRewards.AddGoal(711);
                                            }
                                        }

                                    }
                                    break;
                                }

                            #endregion
                            #region Talent
                            case 8:
                                {
                                    if (DBInfo.CheckOpen(user.Player.Level, user.Player.Reborn))
                                    {
                                        //801 Rank Top 8 in Elite PK Tournament, once.

                                        if (user.Equipment.BattlePower >= 70)//802 Total bonus level of all equipped gears reaches 70.
                                            user.HeroRewards.AddGoal(802);

                                        if (!user.HeroRewards.ContainGoal(803))  //All equipped gears have totally 12 embedded gems.
                                        {
                                            if (user.Equipment.Exist(p => p.SocketOne != Role.Flags.Gem.NoSocket && p.SocketOne != Role.Flags.Gem.EmptySocket
                                                && p.SocketTwo != Role.Flags.Gem.EmptySocket && p.SocketTwo != Role.Flags.Gem.NoSocket, 6))
                                                user.HeroRewards.AddGoal(803);
                                        }
                                        if (!user.HeroRewards.ContainGoal(804))  //804 Total Chi score reaches 1,400.
                                        {
                                            if (user.Player.MyChi != null)
                                            {
                                                if (user.Player.MyChi.AllScore() >= 1400)
                                                    user.HeroRewards.AddGoal(804);
                                            }
                                        }

                                        if (!user.HeroRewards.ContainGoal(805))//805 Total Jiang Hu score reaches 20000
                                        {
                                            if (user.Player.MyJiangHu != null)
                                            {
                                                if (user.Player.MyJiangHu.Inner_Strength > 20000)
                                                    user.HeroRewards.AddGoal(805);
                                            }
                                        }

                                        if (user.Player.Nobility != null)//806 Nobility donation reaches 30,000,000 Silver.
                                        {
                                            if (user.Player.Nobility.Donation >= 30000000)
                                                user.HeroRewards.AddGoal(806);
                                        }

                                        if (!user.HeroRewards.ContainGoal(807))//807 A random equipped gear has been purified with P4 Dragon Soul.
                                        {
                                            if (user.Equipment.Exist(p => p.Purification.PurificationLevel >= 4))
                                                user.HeroRewards.AddGoal(807);
                                        }

                                        if (!user.HeroRewards.ContainGoal(808))//808 Equip equipment with P1 Refinery.
                                        {
                                            if (user.Equipment.Exist(p => p.Refinary.EffectLevel >= 1))
                                                user.HeroRewards.AddGoal(808);
                                        }
                                    }
                                    break;
                                }
                            #endregion
                            #region Great Talent
                            case 9:
                                {
                                    //901 Rank Top 3 in Elite PK Tournament, once.
                                    if (user.Equipment.BattlePower >= 100)
                                        user.HeroRewards.AddGoal(902);

                                    if (!user.HeroRewards.ContainGoal(903))  //All equipped gears have totally 12 super embedded gems.
                                    {
                                        int count = 0;
                                        int count1 = 0;
                                        user.Equipment.Have(p => (byte)p.SocketOne % 10 == 3, out count1);
                                        count += count1;
                                        user.Equipment.Have(p => (byte)p.SocketTwo % 10 == 3, out count1);
                                        count += count1;

                                        if (count >= 12)
                                            user.HeroRewards.AddGoal(903);
                                    }
                                    if (!user.HeroRewards.ContainGoal(904))  //All equipped gears have totally 12 super embedded gems.
                                    {
                                        int count = 0;

                                        user.Equipment.Have(p => p.SocketOne != Role.Flags.Gem.NoSocket && p.SocketOne != Role.Flags.Gem.EmptySocket
                                           && p.SocketTwo != Role.Flags.Gem.EmptySocket && p.SocketTwo != Role.Flags.Gem.NoSocket, out count);



                                        if (count >= 6)
                                            user.HeroRewards.AddGoal(904);
                                        //  if (user.Equipment.Exist(p => p.SocketOne != Role.Flags.Gem.NoSocket && p.SocketOne != Role.Flags.Gem.EmptySocket
                                        //          && p.SocketTwo != Role.Flags.Gem.EmptySocket && p.SocketTwo != Role.Flags.Gem.NoSocket, 12))
                                        //      user.HeroRewards.AddGoal(904);
                                    }

                                    if (!user.HeroRewards.ContainGoal(905))  //804 Total Chi score reaches 1,450.
                                    {
                                        if (user.Player.MyChi != null)
                                        {
                                            if (user.Player.MyChi.AllScore() >= 1450)
                                                user.HeroRewards.AddGoal(905);
                                        }
                                    }
                                    if (!user.HeroRewards.ContainGoal(906))//805 Total Jiang Hu score reaches 30000
                                    {
                                        if (user.Player.MyJiangHu != null)
                                        {
                                            if (user.Player.MyJiangHu.Inner_Strength > 30000)
                                                user.HeroRewards.AddGoal(906);
                                        }
                                    }
                                    if (user.Player.Nobility != null)//907  Nobility donation reaches 100 million Silver.
                                    {
                                        if (user.Player.Nobility.Donation >= 100000000)
                                            user.HeroRewards.AddGoal(907);
                                    }

                                    if (!user.HeroRewards.ContainGoal(908))  //A random equipped gear has been purified with P6 Dragon Soul.
                                    {
                                        if (user.Equipment.Exist(p => p.Purification.PurificationLevel >= 6))
                                            user.HeroRewards.AddGoal(908);
                                    }

                                    if (!user.HeroRewards.ContainGoal(909))  //Equip equipment with P5 Refinery.
                                    {
                                        if (user.Equipment.Exist(p => p.Refinary.EffectLevel >= 5))
                                            user.HeroRewards.AddGoal(909);
                                    }

                                    break;
                                }
                            #endregion
                            #region GrandMaster
                            case 10:
                                {
                                    if (user.Player.BattlePower >= 350)
                                        user.HeroRewards.AddGoal(1001);

                                    //1002 Win championship in Elite PK Tournament, once.

                                    if (!user.HeroRewards.ContainGoal(1003))  //1003 Total Chi score reaches 1,580.
                                    {
                                        if (user.Player.MyChi != null)
                                        {
                                            if (user.Player.MyChi.AllScore() >= 1580)
                                                user.HeroRewards.AddGoal(1003);
                                        }
                                    }

                                    if (!user.HeroRewards.ContainGoal(1004))//1004 Total Jiang Hu score reaches 50000
                                    {
                                        if (user.Player.MyJiangHu != null)
                                        {
                                            if (user.Player.MyJiangHu.Inner_Strength > 50000)
                                                user.HeroRewards.AddGoal(1004);
                                        }
                                    }

                                    if (user.Player.Nobility != null)//1005  Nobility donation reaches 500 million Silver.
                                    {
                                        if (user.Player.Nobility.Donation >= 500000000)
                                            user.HeroRewards.AddGoal(1005);
                                    }
                                    if (!user.HeroRewards.ContainGoal(1006))  //1006 All equipped gears have totally 16 embeded Super Gems.
                                    {
                                        if (user.Equipment.Exist(p => (byte)p.SocketOne % 10 == 3 && (byte)p.SocketTwo % 10 == 3, 8))
                                            user.HeroRewards.AddGoal(1006);
                                    }

                                    if (user.Equipment.BattlePower >= 120)       //Total bonus level of all equipped gears reaches 120.
                                        user.HeroRewards.AddGoal(1007);

                                    if (!user.HeroRewards.ContainGoal(1008))  //1008 Equip equipment with P7 stabilized soul.
                                    {
                                        if (user.Equipment.Exist(p => (byte)p.Purification.PurificationLevel >= 7))
                                            user.HeroRewards.AddGoal(1008);
                                    }
                                    break;
                                }
                            #endregion
                        }


                        if (user.HeroRewards.FinishStage(Prize))
                            user.HeroRewards.AddGoal(Prize);

                        var goals = user.HeroRewards.ArrayGoals.Values.Where(p => p.UID / 100 == Prize || p.UID == Prize).ToArray();
                        user.Send(stream.HeroRewardlistCreate(Prize, (byte)(user.HeroRewards.ComplateFullStage(Prize) ? 1 : 0), goals));

                        break;
                    }
                case ActionID.ClaimPrize:
                    {
                        user.CreateBoxDialog("Sorry, the Hero Biography System is now closed.");
                        break;
                        if (Pool.TableHeroRewards.ContainsKey(Prize / 100))
                        {
                            var DBInfo = Pool.TableHeroRewards[Prize / 100];

                            Role.Instance.HeroRewards.Item prize;
                            if (DBInfo.ArrayItem.ContainsKey(Prize) && user.HeroRewards.ArrayGoals.TryGetValue(Prize, out prize))
                            {
                                if (prize.Claim == 0)
                                {
                                    byte count = DBInfo.ArrayItem[Prize].GetCount();

                                    if (user.Inventory.HaveSpace(count))
                                    {
                                        prize.Claim = 1;

                                        uint ItemID = DBInfo.ArrayItem[Prize].GetPrize();

                                        byte plus = 0;
                                        if (ItemID >= 730001 && ItemID <= 730008)
                                            plus = (byte)(ItemID % 10);

                                        user.Inventory.Add(stream, ItemID, count, plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                                        user.Send(stream.HeroRewardCreate(mode, Prize));
                                    }
                                    else
                                    {
#if Arabic
                                               user.CreateBoxDialog("Please make " + count.ToString() + " spaces in your inventory.");
#else
                                        user.CreateBoxDialog("Please make " + count.ToString() + " spaces in your inventory.");
#endif

                                    }
                                }
                            }

                        }
                        break;
                    }
                case ActionID.ClaimSubPrize:
                    {
                        user.CreateBoxDialog("Sorry, the Hero Biography System is now closed.");
                        break;
                        if (Pool.TableHeroRewards.ContainsKey(Prize))
                        {
                            var DBInfo = Pool.TableHeroRewards[Prize];

                            Role.Instance.HeroRewards.Item prize;
                            if (user.HeroRewards.ArrayGoals.TryGetValue(Prize, out prize))
                            {
                                if (prize.Claim == 0)
                                {
                                    byte count = DBInfo.GetCount();

                                    if (user.Inventory.HaveSpace(count))
                                    {
                                        prize.Claim = 1;
                                        user.HeroRewards.ArrayGoals[prize.UID] = prize;

                                        uint ItemID = DBInfo.GetPrize();

                                        byte plus = 0;
                                        if (ItemID >= 730001 && ItemID <= 730008)
                                            plus = (byte)(ItemID % 10);

                                        user.Inventory.Add(stream, ItemID, count, plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                                        user.Send(stream.HeroRewardCreate(mode, Prize));
                                    }
                                    else
                                    {

#if Arabic
                                          user.CreateBoxDialog("Please make " + count.ToString() + " spaces in your inventory.");
#else
                                        user.CreateBoxDialog("Please make " + count.ToString() + " spaces in your inventory.");
#endif

                                    }
                                }
                            }

                        }
                        break;
                    }
            }
        }
    }
}
