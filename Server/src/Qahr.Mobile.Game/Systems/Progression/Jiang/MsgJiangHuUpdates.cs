using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static void GetJiangHuUpdates(this ServerSockets.Packet stream, out  MsgJiangHuUpdates.Action Action, out byte Star, out byte Stage, out byte High, out byte KnowledgePill)
        {
            uint freecourse = stream.ReadUInt32();//4
            High = stream.ReadUInt8();//8
            Action = (MsgJiangHuUpdates.Action)stream.ReadUInt8();//9
            Star = stream.ReadUInt8();//10
            Stage = stream.ReadUInt8();//11
            KnowledgePill = stream.ReadUInt8();//12
            stream.ReadUInt16();//13
            stream.ReadUInt8();//15
            stream.ReadUInt8();//16
        }

        public static unsafe ServerSockets.Packet JiangHuUpdatesCreate(this ServerSockets.Packet stream, uint FreeCourse, MsgJiangHuUpdates.Action Action, byte Star, byte Stage, ushort Atribut, byte FreeTimeTodeyUsed, uint RoundBuyPoints, byte High, byte KnowledgePill)
        {
            stream.InitWriter();

            stream.Write(FreeCourse);//4
            stream.Write((byte)High);//8
            stream.Write((byte)Action);//9
            stream.Write(Star);//10
            stream.Write(Stage);//11
            stream.Write(KnowledgePill);//12
            stream.Write(Atribut);//13
            stream.Write(FreeTimeTodeyUsed);//15
            stream.Write(RoundBuyPoints);//16

            stream.Finalize(GamePackets.MsgOwnKongfuImproveFeedback);

            return stream;
        }

    }/*Byte: 27 uShort: 27 UInt: 141492251  Offset = 0
Byte: 111 uShort: 2159 UInt: 34408559  Offset = 2
Byte: 8 uShort: 3336 UInt: 117574920  Offset = 3
Byte: 13 uShort: 525 UInt: 459277  Offset = 4
Byte: 2 uShort: 1794 UInt: 822085378  Offset = 5
Byte: 7 uShort: 7 UInt: 808517639  Offset = 6
Byte: 49 uShort: 12337 UInt: 808464433  Offset = 8
Byte: 48 uShort: 12336 UInt: 808464432  Offset = 9
Byte: 48 uShort: 12336 UInt: 808464432  Offset = 10
Byte: 48 uShort: 12336 UInt: 808464432  Offset = 11
Byte: 48 uShort: 12336 UInt: 170930224  Offset = 12
Byte: 48 uShort: 12336 UInt: 667696  Offset = 13
Byte: 48 uShort: 2608 UInt: 822086192  Offset = 14
Byte: 10 uShort: 10 UInt: 925958154  Offset = 15
Byte: 49 uShort: 14129 UInt: 825243441  Offset = 17
Byte: 55 uShort: 12343 UInt: 858861623  Offset = 18
Byte: 48 uShort: 12592 UInt: 875770160  Offset = 19
Byte: 49 uShort: 13105 UInt: 842281777  Offset = 20
Byte: 51 uShort: 13363 UInt: 892482611  Offset = 21
Byte: 52 uShort: 12852 UInt: 892678708  Offset = 22
Byte: 50 uShort: 13618 UInt: 859125042  Offset = 23
Byte: 53 uShort: 13621*/
    public unsafe struct MsgJiangHuUpdates
    {
        public enum Action : byte
        {
            CreateStar = 0,
            BuyStrength = 1,
            TrainingPill = 2
        }


        [PacketAttribute(GamePackets.MsgOwnKongfuImproveFeedback)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            MsgJiangHuUpdates.Action Mode;
            byte Star;
            byte Stage;
            byte Higher;
            byte KnowledgePill;
            stream.GetJiangHuUpdates(out Mode, out Star, out Stage, out Higher, out KnowledgePill);

            
            switch (Mode)
            {
                case Action.CreateStar:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            if (Star > 9 || Stage > 9)
                                break;
                            if (Star > 1 && !user.Player.MyJiangHu.ArrayStages[Stage - 1].ArrayStars[Star - 2].Activate)
                                break;
                            if (user.Inventory.Contain(3002926, 1))
                            {
                                user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                user.Player.MyJiangHu.OnJiangMode = true;
                                user.Player.MyJiangHu.SendStatus(stream, user, user);
                                user.Player.MyJiangHu.Talent -= 1;
                                user.Player.SubClass.RemoveStudy(user, 20, stream);
                                user.Inventory.Remove(3002926, 1, stream);
                                user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                            }
                            else if (user.Player.SubClass.StudyPoints >= 20 && user.Player.MyJiangHu.FreeTimeToday > 0 && user.Player.MyJiangHu.Talent >= 1 && user.Player.MyJiangHu.FreeCourse >= 10000 && Higher != 1 && Higher != 2)
                            {

                                user.Activeness.IncreaseTask(38);

                                user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                user.Player.MyJiangHu.OnJiangMode = true;
                                user.Player.MyJiangHu.SendStatus(stream, user, user);
                                user.Player.MyJiangHu.Talent -= 1;
                                user.Player.MyJiangHu.FreeCourse -= 10000;
                                user.Player.SubClass.RemoveStudy(user, 20, stream);
                                user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                              
                            }
                            else if (user.Player.MyJiangHu.Talent >= 1  && user.Player.MyJiangHu.FreeCourse >= 10000 && user.Inventory.Contain(3003124, 1))
                            {
                                user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                user.Player.MyJiangHu.OnJiangMode = true;
                                user.Player.MyJiangHu.SendStatus(stream, user, user);
                                user.Player.MyJiangHu.Talent -= 1;
                                user.Player.MyJiangHu.FreeCourse -= 10000;
                                user.Player.SubClass.RemoveStudy(user, 20, stream);
                                user.Inventory.Remove(3003124, 1, stream);
                                  user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                            }
                          
                            #region Higher
                            if (Higher == 1)
                            {
                                if (user.Inventory.Contain(3003125, 1))
                                {
                                    user.Inventory.Remove(3003125, 1, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3003125, 1) && user.Player.ConquerPoints >= 5)
                                {
                                    user.Inventory.Remove(3003125, 1, stream);
                                    user.Player.ConquerPoints -= 5;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                            #region Highst
                            if (Higher == 2)
                            {
                                if (user.Inventory.Contain(3003126, 1))
                                {
                                    user.Inventory.Remove(3003126, 1, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3003126, 1) && user.Player.ConquerPoints >= 50)
                                {
                                    user.Inventory.Remove(3003126, 1, stream);
                                    user.Player.ConquerPoints -= 50;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                            #region knowledgePill
                            if (KnowledgePill == 1)
                            {
                                if (user.Inventory.Contain(3303373, 1))
                                {
                                    user.Inventory.Remove(3303373, 1, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                    user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3303373, 1) && user.Player.ConquerPoints >= 50)
                                {
                                    user.Inventory.Remove(3303373, 1, stream);
                                    user.Player.ConquerPoints -= 50;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                    user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                        }
                        break;
                    }
             
                case Action.BuyStrength:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            if (Star > 9 || Stage > 9)
                                break;
                            if (Star > 1 && !user.Player.MyJiangHu.ArrayStages[Stage - 1].ArrayStars[Star - 2].Activate)
                                break;

                            ushort GetCpsStage = (ushort)((user.Player.MyJiangHu.RoundBuyPoints * 10) + 10);
                            ushort GetCpsHigher = (ushort)((user.Player.MyJiangHu.RoundBuyPoints * 10) + 15);
                            ushort GetCpsHighst = (ushort)((user.Player.MyJiangHu.RoundBuyPoints * 10) + 60);
                            if (user.Player.ConquerPoints >= GetCpsStage)
                            {
                                user.Activeness.IncreaseTask(38);

                                user.Player.ConquerPoints -= GetCpsStage;
                                user.Player.MyJiangHu.OnJiangMode = true;
                                user.Player.MyJiangHu.SendStatus(stream, user, user);
                                user.Player.MyJiangHu.FreeCourse += 10000;
                                user.Player.MyJiangHu.Talent = (byte)Math.Min(5, user.Player.MyJiangHu.Talent + 1);
                               
                                user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);

                                  user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);

                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                            }
                            else
                            {

                                user.SendSysMesage("You do not have " + GetCpsStage + " ConquerPoints with you.");


                            }
                            #region Higher
                            if (Higher == 1)
                            {
                                if (user.Inventory.Contain(3003125, 1) && user.Player.ConquerPoints >= GetCpsHigher)
                                {
                                    user.Inventory.Remove(3003125, 1, stream);
                                    user.Player.ConquerPoints -= GetCpsHigher;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                   
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3003125, 1) && user.Player.ConquerPoints >= GetCpsHigher)
                                {
                                    user.Inventory.Remove(3003125, 1, stream);
                                    user.Player.ConquerPoints -= GetCpsHigher;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                            #region Highst
                            if (Higher == 2)
                            {
                                if (user.Inventory.Contain(3003126, 1) && user.Player.ConquerPoints >= GetCpsHighst)
                                {
                                    user.Inventory.Remove(3003126, 1, stream);
                                    user.Player.ConquerPoints -= GetCpsHighst;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                   
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3003126, 1) && user.Player.ConquerPoints >= GetCpsHighst)
                                {
                                    user.Inventory.Remove(3003126, 1, stream);
                                    user.Player.ConquerPoints -= GetCpsHighst;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                   
                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                            #region knowledgePill
                            if (KnowledgePill == 1)
                            {
                                if (user.Inventory.Contain(3303373, 1))
                                {
                                    user.Inventory.Remove(3303373, 1, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                    user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                else if (!user.Inventory.Contain(3303373, 1) && user.Player.ConquerPoints >= 50)
                                {
                                    user.Inventory.Remove(3303373, 1, stream);
                                    user.Player.ConquerPoints -= 50;
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                    user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                        }
                        break;
                    }
                case Action.TrainingPill:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            if (Star > 9 || Stage > 9)
                                break;
                            if (Star > 1 && !user.Player.MyJiangHu.ArrayStages[Stage - 1].ArrayStars[Star - 2].Activate)
                                break;

                            ushort priceCps = (ushort)((user.Player.MyJiangHu.RoundBuyPoints * 10) + 10); // oficial conquer cps
                            uint NeedPills = (uint)priceCps / 10;


                            #region Higher
                            if (Higher == 1)
                            {
                                if (user.Inventory.Contain(3003125, 1) && (user.Inventory.Contain(3003124, NeedPills) || user.Inventory.Contain(3003124, NeedPills, 1)))
                                {
                                    user.Inventory.Remove(3003125, 1, stream);
                                    user.Inventory.Remove(3003124, NeedPills, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;

                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                                
                            }
                            #endregion
                            #region Highst
                            if (Higher == 2)
                            {
                                if (user.Inventory.Contain(3003126, 1) && (user.Inventory.Contain(3003124, NeedPills) || user.Inventory.Contain(3003124, NeedPills, 1)))
                                {
                                    user.Inventory.Remove(3003126, 1, stream);
                                    user.Inventory.Remove(3003124, NeedPills, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;

                                      user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                               
                            }
                            #endregion
                            #region knowledgePill
                            if (KnowledgePill == 1)
                            {
                                if (user.Inventory.Contain(3303373, 1) && (user.Inventory.Contain(3003124, NeedPills) || user.Inventory.Contain(3003124, NeedPills, 1)))
                                {
                                    user.Inventory.Remove(3303373, 1, stream);
                                    user.Inventory.Remove(3003124, NeedPills, stream);
                                    user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                    user.Player.MyJiangHu.OnJiangMode = true;
                                    user.Player.MyJiangHu.SendStatus(stream, user, user);
                 
                                    user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                    user.Player.MyJiangHu.Talent -= 1;
                                    user.Player.MyJiangHu.FreeCourse -= 10000;
                                    user.Player.SubClass.RemoveStudy(user, 20, stream);
                                    user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                                }
                            }
                            #endregion
                            if (user.Inventory.Contain(3003124, NeedPills) || user.Inventory.Contain(3003124, NeedPills,1))
                            {
                                user.Inventory.Remove(3003124, NeedPills, stream);
                                user.Player.MyJiangHu.RoundBuyPoints = (byte)Math.Min(49, user.Player.MyJiangHu.RoundBuyPoints + 1);
                                user.Player.MyJiangHu.OnJiangMode = true;
                                user.Player.MyJiangHu.SendStatus(stream, user, user);

                                user.Player.MyJiangHu.Level = (byte)user.Player.Level;
                                user.Player.MyJiangHu.Talent -= 1;

                                user.Player.MyJiangHu.CreateRollValue(stream, user, Star, Stage, Higher, KnowledgePill);
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateStar, false, Stage.ToString(), Star.ToString());
                                user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                user.Player.SubClass.UpdateStundyPoints(user, 20, stream);
                            }
                          
                        }
                        break;
                    }
                default: Console.WriteLine("[Jiang] Unknown Action " + Mode); break;
            }

        }
    }
}
