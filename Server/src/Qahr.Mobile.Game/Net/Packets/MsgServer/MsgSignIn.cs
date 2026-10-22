using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSignIn
    {

        public static unsafe void GetMsgSignIn(this ServerSockets.Packet stream, out ActionID Action, out uint dwPram)
        {
            Action = (ActionID)stream.ReadUInt8();
            dwPram = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet MsgSignInCreate(this ServerSockets.Packet stream, ActionID Action, byte dwParam, ushort dwParam2, ulong Flags)
        {
            stream.InitWriter();

            stream.Write((byte)Action);
            stream.Write(dwParam);
            stream.Write(dwParam2);
            stream.Write(Flags);
            stream.Finalize(GamePackets.MsgSignIn);
            return stream;
        }
        public enum ActionID : byte
        {
            Show = 3,
            SignIn = 0,
            ClaimReward = 2
        }
        [PacketAttribute(GamePackets.MsgSignIn)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
         
            ActionID Action;
            uint dwPram;
            stream.GetMsgSignIn(out Action, out dwPram);
            switch (Action)
            {
                case ActionID.SignIn:
                    {
                        if (!user.Inventory.HaveSpace(2))
                        {
                            user.SendSysMesage("Please make 2 more spaces in your inventory.");
                            break;
                        }
                        DateTime Now= DateTime.Now;
                        ulong flag = (1ul << (byte)(Now.Day - 1));
                        if ((user.Player.DailySignUpDays & flag) != flag)
                        {
                            user.Player.DailyMonth = (byte)DateTime.Now.Month;
                            user.Player.DailySignUpDays |= flag;
                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                            user.Inventory.Add(stream, 3100011, 1, 0,0,0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                          
                            uint days = user.Player.DailyDays;
                            switch (user.Player.DailySignUpRewards)
                            {
                                case 0:
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make 1 more space in your inventory.");
                                            break;
                                        }
                                        if (days >= 2)
                                        {
                                            user.Player.DailySignUpRewards = 1;

                                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                            user.Inventory.Add(stream, 3100013, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make 1 more space in your inventory.");
                                            break;
                                        }
                                        if (days >= 7)
                                        {
                                            user.Player.DailySignUpRewards = 2;
                                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                            user.Inventory.Add(stream, 3100014, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make 1 more space in your inventory.");
                                            break;
                                        }
                                        if (days >= 14)
                                        {
                                            user.Player.DailySignUpRewards = 3;
                                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                            user.Inventory.Add(stream, 3100015, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make 1 more space in your inventory.");
                                            break;
                                        }
                                        if (days >= 21)
                                        {
                                            user.Player.DailySignUpRewards = 4;
                                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                            user.Inventory.Add(stream, 3100016, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make 1 more space in your inventory.");
                                            break;
                                        }
                                        if (days >= 28)
                                        {
                                            user.Player.DailySignUpRewards = 5;
                                            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                            user.Inventory.Add(stream, 3100017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case ActionID.Show:
                    {
                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                        uint days = user.Player.DailyDays;
                        switch (user.Player.DailySignUpRewards)
                        {
                            case 0:
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    if (days >= 2)
                                    {
                                        user.Player.DailySignUpRewards = 1;

                                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                        user.Inventory.Add(stream, 3100013, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    if (days >= 7)
                                    {
                                        user.Player.DailySignUpRewards = 2;
                                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                        user.Inventory.Add(stream, 3100014, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    if (days >= 14)
                                    {
                                        user.Player.DailySignUpRewards = 3;
                                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                        user.Inventory.Add(stream, 3100015, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    if (days >= 21)
                                    {
                                        user.Player.DailySignUpRewards = 4;
                                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                        user.Inventory.Add(stream, 3100016, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    if (days >= 28)
                                    {
                                        user.Player.DailySignUpRewards = 5;
                                        user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, 0, user.Player.DailySignUpDays));
                                        user.Inventory.Add(stream, 3100017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case ActionID.ClaimReward:
                    {
                      
                       

                        break;
                    }
            }

            //  user.Send(stream.MsgSignInCreate(ActionID.Draw, 1));
            // Console.WriteLine(DateTime.Now.DayOfYear);
        }
    }
}
