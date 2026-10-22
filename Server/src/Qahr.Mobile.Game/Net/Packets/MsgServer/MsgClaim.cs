using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {


        public static unsafe ServerSockets.Packet BlessingInfoCreate(this ServerSockets.Packet stream, MsgClaim.MsgBlessingInfo.Action Mode, uint OnlineBlessingTraining
            , uint HuntBlessing)
        {
            stream.InitWriter();

           
            stream.Write((uint)Mode);
            stream.Write(OnlineBlessingTraining);
            stream.Write(HuntBlessing);

            stream.Finalize(GamePackets.MsgGodExp);
            return stream;
        }

        public static unsafe void GetBlessingInfo(this ServerSockets.Packet stream, out MsgClaim.MsgBlessingInfo.Action Mode
            , out uint OnlineBlessingTraining, out uint HuntBlessing)
        {
            uint timerstamp = stream.ReadUInt32();
            Mode = (MsgClaim.MsgBlessingInfo.Action)stream.ReadUInt32();
            OnlineBlessingTraining = stream.ReadUInt32();
            HuntBlessing = stream.ReadUInt32();
        }



        public static unsafe ServerSockets.Packet ClaimCreate(this ServerSockets.Packet stream, MsgClaim.Action Mode, uint Mentor_UID
            , uint OnlineBlessingTraining, uint HuntBlessing, ulong MentorExperience, ushort MentorHeavensBlessing, ushort PlusStone)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(Mentor_UID);
            stream.Write(OnlineBlessingTraining);
            stream.Write(HuntBlessing);
            stream.Write((uint)0);//unknow
            stream.Write(MentorExperience);
            stream.Write(MentorHeavensBlessing);
            stream.Write(PlusStone);
            stream.Write((uint)0);//unknow

            stream.Finalize(GamePackets.MsgContribute);
            return stream;
        }

        public static unsafe void GetClaim(this ServerSockets.Packet stream, out MsgClaim.Action Mode, out uint Mentor_UID
            , out uint OnlineBlessingTraining, out uint HuntBlessing, out ulong MentorExperience, out ushort MentorHeavensBlessing, out ushort PlusStone)
        {
            Mode = (MsgClaim.Action)stream.ReadUInt32();
            Mentor_UID = stream.ReadUInt32();
            OnlineBlessingTraining = stream.ReadUInt32();
            HuntBlessing = stream.ReadUInt32();
            uint unknow = stream.ReadUInt32();//padding?
            MentorExperience = stream.ReadUInt64();
            MentorHeavensBlessing = stream.ReadUInt16();
            PlusStone = stream.ReadUInt16();
        }


    }
    public unsafe struct MsgClaim
    {
        /// <summary>
        /// about OnlineBlessingTraining 
        /// ----------------->>>>>
        ///6 000 000 = 1 expball / 10000
        ///3 000 000 = 0.50
        ///1 500 000 0.25
        ///750 000 = 0.013
        ///375000 = 0.06
        ///187500 = 0.03
        ///93750 = 0.02
        ///46875 = 0.01
        /// </summary>
        public struct MsgBlessingInfo
        {
            public enum Action : uint
            {
                Show = 0,
                ClaimOnlineTraining = 1,
                ClaimHuntBlessing = 2
            }
            [PacketAttribute(Game.GamePackets.MsgGodExp)]
            public unsafe static void ClainInfo(Client.GameClient user, ServerSockets.Packet stream)
            {
                Action Mode;
                uint OnlineBlessingTraining;
                uint HuntBlessing;


                stream.GetBlessingInfo(out Mode, out OnlineBlessingTraining, out HuntBlessing);

                switch (Mode)
                {
                    case Action.Show:
                        {
                            OnlineBlessingTraining = user.Player.OnlineTrainingPoints;
                            HuntBlessing = user.Player.HuntingBlessing;

                            user.Send(stream.BlessingInfoCreate(Action.Show, OnlineBlessingTraining, HuntBlessing));

                            user.Send(stream.ClaimCreate(MsgClaim.Action.Show, user.Player.UID, 0, 0, user.Player.Associate.Mentor_ExpBalls
                                , (ushort)user.Player.Associate.Mentor_Blessing, (ushort)user.Player.Associate.Mentor_Stones));

                            break;
                        }
                    case Action.ClaimOnlineTraining:
                        {
                            if (user.Player.Level < 140)
                            {
                                if (user.Player.OnlineTrainingPoints > 0)
                                {
                                    user.GainExpBall(user.Player.OnlineTrainingPoints / 10000, true, Role.Flags.ExperienceEffect.angelwing);
                                    user.Player.OnlineTrainingPoints = 0;
                                }
                            }
                            else
                            {
#if Arabic
                                  user.SendSysMesage("Sorry,you are high level! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                user.SendSysMesage("Sorry,you are high level! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif

                            }
                            break;
                        }
                    case Action.ClaimHuntBlessing:
                        {
                            if (user.Player.Level < 140)
                            {
                                if (user.Player.HuntingBlessing > 0)
                                {
                                    user.GainExpBall(user.Player.HuntingBlessing / 10000, true, Role.Flags.ExperienceEffect.angelwing);
                                    user.Player.HuntingBlessing = 0;
                                }
                            }
                            else
                            {
#if Arabic
                                user.SendSysMesage("Sorry,you are high level! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                user.SendSysMesage("Sorry,you are high level! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif

                            }
                            break;
                        }
                    default:
                        {
#if TEST
                            if (user.ProjectManager)
                                MyConsole.WriteLine("UnKnow " + Game.GamePackets.ClainInfo + " -> " + Mode);
#endif
                            break;
                        }
                }
            }
        }
        public enum Action : uint
        {
            ClaimExperience = 1,
            ClaimAmountExpBalls = 2,
            ClaimHeavenBlessing = 3,
            ClaimPlus = 4,
            Show = 5
        }


        [PacketAttribute(Game.GamePackets.MsgContribute)]
        public unsafe static void HandlerMentorPrize(Client.GameClient user, ServerSockets.Packet packet)
        {

            Action ActionType;
            uint Mentor_UID;
            uint OnlineBlessingTraining;
            uint HuntBlessing;
            ulong MentorExperience;
            ushort MentorHeavensBlessing;
            ushort PlusStone;

            packet.GetClaim(out ActionType, out Mentor_UID, out OnlineBlessingTraining, out HuntBlessing, out MentorExperience, out MentorHeavensBlessing
                , out PlusStone);

            switch (ActionType)
            {
                case Action.ClaimHeavenBlessing:
                    {
                        user.Player.AddHeavenBlessing(packet, (int)user.Player.Associate.Mentor_Blessing);
                        user.Player.Associate.Mentor_Blessing = 0;

                        goto case Action.Show;
                    }
                case Action.ClaimPlus:
                    {
                        ushort stones = (ushort)(user.Player.Associate.Mentor_Stones / 100);
                        int totake = stones;
                        if (stones > 0)
                        {
                            for (; stones > 0; stones--)
                            {
                                if (!user.Inventory.Add(packet, 730001, 1, 1))
                                    break;
                                if (user.Player.Associate.Mentor_Stones > 100)
                                    user.Player.Associate.Mentor_Stones -= 100;
                                else
                                    user.Player.Associate.Mentor_Stones = 0;
                            }
                        }
                        goto case Action.Show;
                    }
                case Action.ClaimExperience:
                    {
                        user.GainExpBall(user.Player.Associate.Mentor_ExpBalls, true, Role.Flags.ExperienceEffect.angelwing);
                        user.Player.Associate.Mentor_ExpBalls = 0;

                        goto case Action.Show;
                    }
                case Action.ClaimAmountExpBalls:
                    {
                        if (user.Player.Associate.Mentor_ExpBalls >= MentorExperience)
                        {
                            user.GainExpBall(MentorExperience, true, Role.Flags.ExperienceEffect.angelwing);
                            user.Player.Associate.Mentor_ExpBalls -= (uint)MentorExperience;

                        }
                        goto case Action.Show;
                    }
                case Action.Show:
                    {
                        // QahrMobile: the mobile client shows the rewards only for action 5 (CMsgContribute::Process); it was the uid
                        user.Send(packet.ClaimCreate(Action.Show, Mentor_UID, OnlineBlessingTraining, HuntBlessing
                            , user.Player.Associate.Mentor_ExpBalls, (ushort)user.Player.Associate.Mentor_Blessing
                            , (ushort)user.Player.Associate.Mentor_Stones));
                        break;
                    }
                default:
                    {
#if TEST
                        if (user.ProjectManager)
                            MyConsole.WriteLine("UnKnow " + Game.GamePackets.MentorPrize + " -> " + ActionType);
#endif
                        break;
                    }
            }
        }
    }
}
