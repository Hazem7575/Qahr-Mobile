using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
  public  static class MsgSecondaryPassword
    {
      public enum ActionID : uint
      {
          VerifiedPassword = 1,
          ForgetPassword = 2, 
          SetNewPass = 3, 
          SendInformation = 4,
          PasswordCorrect = 5, 
          PasswordWrong = 6
      }

      public static unsafe ServerSockets.Packet SecondaryPasswordCreate(this ServerSockets.Packet stream, ActionID Type, uint OldPassowrd, uint NewPassword)
      {
          stream.InitWriter();
          stream.Write((uint)Type);
          stream.Write(OldPassowrd);
          stream.Write(NewPassword);

          stream.Finalize(GamePackets.Msg2ndPsw);
          return stream;
      }
      public static unsafe void GetSecondaryPassword(this ServerSockets.Packet stream, out ActionID Type, out uint OldPassowrd, out uint NewPassword)
      {
         Type = (ActionID)stream.ReadUInt32();
          OldPassowrd = stream.ReadUInt32();
          NewPassword = stream.ReadUInt32();
      }
      // One-time reward for setting the secondary (account-security) password for the
      // first time, mirroring the official "Account Security Center" rewards.
      // Granted only on the very first set (SecurityPassword goes 0 -> value) and never
      // again (SecondaryPasswordReward persists the claim, so Forget/Set can't farm it).
      private static void GrantFirstSetRewards(Client.GameClient user, ServerSockets.Packet stream)
      {
          if (SecondaryPasswordReward.HasRewarded(user.Player.UID))
              return;
          if (!user.Inventory.HaveSpace(3))
          {
              user.SendSysMesage("Your secondary-password reward is waiting: free 3 inventory slots, then set your secondary password again to receive DragonBall(B), BoundMeteorPack and +3Stone(B)Pack.", MsgMessage.ChatMode.System);
              return;
          }
          SecondaryPasswordReward.SetRewarded(user.Player.UID);
          user.Inventory.AddItemWitchStack(3333531, 0, 1, stream); // DragonBall(B)
          user.Inventory.AddItemWitchStack(3320717, 0, 1, stream); // BoundMeteorPack
          user.Inventory.AddItemWitchStack(3301807, 0, 1, stream); // +3Stone(B)Pack
          user.SendSysMesage("Secondary password set! Reward received: DragonBall(B), BoundMeteorPack, +3Stone(B)Pack.", MsgMessage.ChatMode.System);
      }

      [PacketAttribute(GamePackets.Msg2ndPsw)]
      private static void Process(Client.GameClient user, ServerSockets.Packet stream)
      {
          ActionID Type;
          uint OldPassoword;
          uint NewPassoword;
          stream.GetSecondaryPassword(out Type, out OldPassoword, out NewPassoword);

          switch (Type)
          {
              case ActionID.SetNewPass:
                  {
                      if (user.Player.SecurityPassword == 0)
                      {
                          user.Player.SecurityPassword = NewPassoword;
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
                          GrantFirstSetRewards(user, stream);
#if Arabic
                            user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                          user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                        
                      }
                      break;
                  }
              case ActionID.SendInformation:
                  {
                      if (user.Player.SecurityPassword == 0)
                      {
                          user.Player.SecurityPassword = NewPassoword;
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
                          GrantFirstSetRewards(user, stream);
#if Arabic
                           user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                          user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                         
                      }
                      else
                      {
                          if (OldPassoword == user.Player.SecurityPassword)
                          {
                              user.Player.OnReset = 0;
                              user.Player.SecurityPassword = NewPassoword;
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                  user.SendSysMesage("Successfully modified! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                              user.SendSysMesage("Successfully modified! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                          
                          }
                          else
                          {
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 1, 0));
                          }
                      }

                      break;
                  }
              case ActionID.VerifiedPassword:
                  {
                      if (user.Player.SecurityPassword != 0)
                      {
                          if (user.Player.SecurityPassword == NewPassoword)
                          {
                              user.Player.IsCheckedPass = true;
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                      user.SendSysMesage("Secondary verified!", MsgMessage.ChatMode.System);
#else
                              user.SendSysMesage("Secondary verified!", MsgMessage.ChatMode.System);
#endif
                      
                          }
                          else
                          {
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 0, 0));
                          }
                      }
                      break;
                  }
              case ActionID.ForgetPassword:
                  {
                      if (user.Player.SecurityPassword != 0)
                      {
                          user.Player.OnReset = 1;
                          user.Player.ResetSecurityPassowrd = DateTime.Now.AddDays(7);
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                  user.SendSysMesage("Your secondary password will be removed on " + user.Player.ResetSecurityPassowrd.ToString("d/M/yyyy (H:mm)") + ".", MsgMessage.ChatMode.System);
                   
#else
                          user.SendSysMesage("Your secondary password will be removed on " + user.Player.ResetSecurityPassowrd.ToString("d/M/yyyy (H:mm)") + ".", MsgMessage.ChatMode.System);
                   
#endif
                     }
                      break;
                  }
          }
      }
    }
}
