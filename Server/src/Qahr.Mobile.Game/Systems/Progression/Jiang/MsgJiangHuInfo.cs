using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet JiangHuInfoCreate(this ServerSockets.Packet stream, MsgJiangHuInfo.JiangMode Mode, params string[] items)
        {
            stream.InitWriter();

            stream.Write((byte)Mode);

            stream.Write(items);

            stream.Finalize(GamePackets.MsgOwnKongfuBase);

            return stream;
        }
        public static void GetJiangHuInfo(this ServerSockets.Packet msg, out  MsgJiangHuInfo.JiangMode Mode, out string[] list)
        {
            Mode = (MsgJiangHuInfo.JiangMode)msg.ReadUInt8();
            list = msg.ReadStringList();
        }

    }
    public unsafe class MsgJiangHuInfo
    {
        public enum JiangMode : byte
        {
            IconBar = 0,
            CreateJiang = 1, 
            InfoStauts = 7,
            UpdateTalent = 5,
            ViewJiang = 9,
            RestoreStar = 10, 
            UpdateStar = 11,
            OpenStage = 12, 
            UpdateTime = 13,
            SetName = 14, 
            ProtectionPill = 16,
            Gather = 17, 
            ChangeJiangName = 18,
        }

        [PacketAttribute(GamePackets.MsgOwnKongfuBase)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
#if Jiang
            MsgJiangHuInfo.JiangMode Action;
            string[] list;

            stream.GetJiangHuInfo(out Action, out list);
            
            switch (Action)
            {
                case JiangMode.ProtectionPill:
                    {
                        if (user.Player.MyJiangHu != null && (user.Inventory.Contain(3002029, 1, 1)
                            || user.Inventory.Contain(3002029, 1, 0)))
                        {
                            user.Inventory.Remove(3002029, 1, stream);

                        }
                        else if (user.Player.MyJiangHu != null && (user.Inventory.Contain(3002030, 1, 1)
                            || user.Inventory.Contain(3002030, 1, 0)))
                        {
                            user.Inventory.Remove(3002029, 1, stream);
                        }
                        else if (user.Player.MyJiangHu != null)
                        {
                            user.Player.MyJiangHu.ApplayNewStar(user);
                        }
                        break;
                    }
                case JiangMode.RestoreStar:
                    {
                        if (user.Player.MyJiangHu != null && user.Player.ConquerPoints >= 20)
                        {
                            user.Player.ConquerPoints -= 20;
                            user.Send(stream.JiangHuInfoCreate(Action, "0"));
                        }
                        break;
                    }
                case JiangMode.UpdateStar:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            user.Player.MyJiangHu.ApplayNewStar(user);
                        }
                        break;
                    }
                case JiangMode.ViewJiang:
                    {
                        if (user.OnInterServer == true)
                            break;
                        if (list.Length > 0)
                        {
                            string Str = list[0];
                            uint UID = 0;
                            if (uint.TryParse(Str, out UID))
                            {
                                Client.GameClient Target;
                                if (Pool.GamePoll.TryGetValue(UID, out Target))
                                {
                                    if (Target.Player.MyJiangHu != null)
                                        Target.Player.MyJiangHu.SendStatus(stream, user, Target);
                                    else
                                    {
                                        user.Send(stream.JiangHuStatusCreate(Str));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case JiangMode.UpdateTime:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            user.Player.MyJiangHu.SendInfo(user, JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                        }
                        break;
                    }
                case JiangMode.Gather:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            if (user.Player.MyChi.ChiPoints < 200)
                            {
                                user.SendSysMesage("");
                                break;
                            }
                            user.Player.MyChi.ChiPoints = user.Player.MyChi.ChiPoints - 200;
                            user.Player.MyJiangHu.Talent = (byte)Math.Max(5, user.Player.MyJiangHu.Talent + 1);
                            user.Player.MyJiangHu.SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                        }
                        break;
                    }
                case JiangMode.ChangeJiangName:
                    {
                        if (user.Player.MyJiangHu != null)
                        {
                            if (list.Length > 0)
                            {
                                string Name = list[0];
                                if (Name.Length > 16 || Name.Contains('/') || !BaseFunc.NameStrCheck(Name))
                                {
#if Arabic
                                      user.SendSysMesage("You use a big name, or you name contain a invalid characters");
#else
                                    user.SendSysMesage("You use a big name, or you name contain a invalid characters");
#endif

                                    break;
                                }
                                if (user.Player.ConquerPoints >= 810)
                                {
                                    user.Player.ConquerPoints -= 810;
                                    user.Player.MyJiangHu.CustomizedName = Name;
                                    user.Send(stream.JiangHuStatusCreate(Name, 1, 0/* QahrMobile: 0 = own Kong Fu (mobile summary byte 0x25), not the talent */
                                    , 0, user.Player.SubClass.StudyPoints, user.Player.MyJiangHu.FreeTimeToday));
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                    user.Player.MyJiangHu.SendInfo(user, MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                                    MsgMessage msg = new MsgMessage(user.Player.Name + " has changed his jiang Customize name.", "ALLUSERS", "SYSTEM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.JianHu);
                                    Server.SendGlobalPacket(msg.GetArray(stream));
                                    break;
                                }
                                else
                                {
                                    user.SendSysMesage("You don`t have enough cps!");
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case JiangMode.CreateJiang:
                    {
                        if (user.Player.MyJiangHu == null)
                        {
                            if (list.Length > 0)
                            {
                                string Name = list[0];

                                if (Name.Length > 16 || Name.Contains('/') || !BaseFunc.NameStrCheck(Name))
                                {
#if Arabic
                                      user.SendSysMesage("You use a big name, or you name contain a invalid characters");
#else
                                    user.SendSysMesage("You use a big name, or you name contain a invalid characters");
#endif

                                    break;
                                }
                                user.Player.MyJiangHu = new Role.Instance.JiangHu(user.Player.UID);
                                user.Player.MyJiangHu.CustomizedName = Name;
                                user.Player.MyJiangHu.Name = user.Player.Name;

                                user.Player.MyJiangHu.ActiveJiangMode(user);
                                user.Player.MyJiangHu.Level = (byte)user.Player.Level;

                                user.Player.MyJiangHu.SendInfo(user, JiangMode.SetName, false, user.Player.UID.ToString(), "1", "1");//stage , star


                                user.Send(stream.JiangHuStatusCreate(Name, 1, 0/* QahrMobile: 0 = own Kong Fu (mobile summary byte 0x25), not the talent */
                                    , 0, user.Player.SubClass.StudyPoints, user.Player.MyJiangHu.FreeTimeToday));

                                user.Player.SubClass.AddStudyPoints(user, 100, stream);
                                user.Player.MyJiangHu.CreateTime();
                                user.Player.MyJiangHu.SendInfo(user, JiangMode.UpdateTime, false, user.Player.MyJiangHu.FreeCourse.ToString(), user.Player.MyJiangHu.Time.ToString());
                                user.Player.MyJiangHu.SendInfo(user, JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                            }
                        }
                        break;
                    }
            }
#endif
        }
    }
}
