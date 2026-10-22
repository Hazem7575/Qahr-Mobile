using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetActivityClaims(this ServerSockets.Packet stream, out uint IDClaim)
        {
            IDClaim = stream.ReadUInt8();
        }

        public static unsafe ServerSockets.Packet ActivityClaimsCreate(this ServerSockets.Packet stream, uint IDClaim)
        {
            stream.InitWriter();

            stream.Write((byte)IDClaim);

            stream.Finalize(GamePackets.MsgActivityTaskReward);
            return stream;
        }


    }
    public unsafe struct MsgActivityClaims
    {
        [PacketAttribute(GamePackets.MsgActivityTaskReward)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint IDClaim;

            stream.GetActivityClaims(out IDClaim);
            if (global::Qahr.Mobile.Phone.MobileDaily.OnClaim(user, IDClaim, stream)) return;// QahrMobile: mobile chests 10..24 (MobileDaily)
            byte claimedBefore = user.Activeness.ClaimRewards;

            switch (IDClaim)
            {
                case 1:
                    {
                        if (user.Activeness.ClaimRewards == 0)
                        {
                            if (user.Activeness.ActivityPoints >= 100)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(stream, 3005061);
                                    user.Activeness.ClaimRewards = 1;
                                }
                                else
                                {
#if Arabic
                                      user.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                    user.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif
                                  
                                }
                            }
                            else
                            {
#if Arabic
                                    user.CreateBoxDialog("You do not have 100 ActivityPoints.");
#else
                                user.CreateBoxDialog("You do not have 100 ActivityPoints.");
#endif
                            
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (user.Activeness.ClaimRewards == 1)
                        {
                            if (user.Activeness.ActivityPoints >= 210)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(stream, 3005062);
                                    user.Activeness.ClaimRewards = 2;
                                }
                                else
                                {
#if Arabic
                                     user.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                    user.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif
                                   
                                }
                            }
                            else
                            {
#if Arabic
                                user.CreateBoxDialog("You do not have 210 ActivityPoints.");
#else
                                user.CreateBoxDialog("You do not have 210 ActivityPoints.");
#endif
                                
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (user.Activeness.ClaimRewards == 2)
                        {
                            if (user.Activeness.ActivityPoints >= 340)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(stream, 3005064);
                                    user.Activeness.ClaimRewards = 3;
                                }
                                else
                                {
#if Arabic
                                      user.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                    user.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif
                                  
                                }
                            }
                            else
                            {
#if Arabic
                                 user.CreateBoxDialog("You do not have 340 ActivityPoints.");
#else
                                user.CreateBoxDialog("You do not have 340 ActivityPoints.");
#endif
                               
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (user.Activeness.ClaimRewards == 3)
                        {
                            if (user.Activeness.ActivityPoints >= 390)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(stream, 3005065);
                                    user.Activeness.ClaimRewards = 4;
                                }
                                else
                                {
#if Arabic
                                        user.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                    user.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif
                                
                                }
                            }
                            else
                            {
#if Arabic
                                user.CreateBoxDialog("You do not have 390 ActivityPoints.");
#else
                                user.CreateBoxDialog("You do not have 390 ActivityPoints.");
#endif
                                
                            }
                        }
                        break;
                    }

            }

            // echo only when the claim was granted
            if (user.Activeness.ClaimRewards != claimedBefore)
                user.Send(stream.ActivityClaimsCreate(IDClaim));
        }
    }
}
