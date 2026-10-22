using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetVipHandler(this ServerSockets.Packet stream, out MsgVipHandler.VIPTeleportTypes TeleportType, out MsgVipHandler.VIPTeleportLocations Locations, out uint Countdown
            , out string name)
        {
            TeleportType = (MsgVipHandler.VIPTeleportTypes)stream.ReadUInt32();
            Locations = (MsgVipHandler.VIPTeleportLocations)stream.ReadUInt32();
            Countdown = stream.ReadUInt32();

            ushort size = stream.ReadUInt16();
            name = stream.ReadCString(size);
        }
        public static unsafe ServerSockets.Packet VipHandlerCreate(this ServerSockets.Packet stream, MsgVipHandler.VIPTeleportTypes TeleportType, MsgVipHandler.VIPTeleportLocations Locations, uint Countdown
         , string name)
        {

            stream.InitWriter();

            stream.Write((uint)TeleportType);
            stream.Write((uint)Locations);
            stream.Write(Countdown);

            stream.Write((ushort)name.Length);
            stream.Write(name, name.Length);

            stream.Finalize(Game.GamePackets.MsgVipHandler);

            return stream;
        }
    }
    public unsafe struct MsgVipHandler
    {
        public enum VIPTeleportTypes : uint
        {
            SelfTeleport = 0,
            TeamTeleport = 1,
            TeammateConfirmation = 2,
            TeammateTeleport = 3
        }
        public enum VIPTeleportLocations : uint
        {
            TwinCity = 1,
            PhoenixCastle = 2,
            ApeCity = 3,
            DesertCity = 4,
            BirdIland = 5,
            TCSquare = 6,
            WPAltar = 7,
            WPApparation = 8,
            WPPoltergiest = 9,
            WPTurtledove = 10,
            PCSqaure = 11,
            MFWaterCave = 12,
            MFVillage = 13,
            MFLake = 14,
            MFMineCave = 15,
            MFBridge = 16,
            MFToApeCity = 17,
            ACSquare = 18,
            ACSouth = 19,
            ACEast = 20,
            ACNorth = 21,
            ACWest = 22,
            BISquare = 23,
            BINorth = 24,
            BIWest = 25,
            BIEast = 26,
            IslandVillage = 27,
            DCSquare = 28,
            DCSouth = 29,
            DCMoonSpring = 30,
            DCAncientMaze = 31
        }

        [PacketAttribute(GamePackets.MsgVipHandler)]
        public unsafe static void Handler(Client.GameClient user, ServerSockets.Packet packet)
        {

            VIPTeleportTypes TeleportType;
            VIPTeleportLocations Locations;
            uint Countdown;
            string name;

            packet.GetVipHandler(out TeleportType, out Locations, out Countdown, out name);

            switch (TeleportType)
            {
                case VIPTeleportTypes.SelfTeleport:
                    {
                        if (DateTime.Now > user.LastVIPTeleport.AddSeconds(1))
                        {
                            Teleport(user, Locations);
                            user.LastVIPTeleport = DateTime.Now;
                        }
                        else
                        {
#if Arabic
                                    user.SendSysMesage("You recently used the City Teleport service. Please wait " + (user.LastVIPTeleport.AddSeconds(180).AllSeconds() - DateTime.Now.AllSeconds()).ToString() + " seconds to use it again.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    
#else
                            user.SendSysMesage("You recently used the City Teleport service. Please wait " + (user.LastVIPTeleport.AddSeconds(1).AllSeconds() - DateTime.Now.AllSeconds()).ToString() + " seconds to use it again.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

#endif
                        }
                        break;
                    }
                case VIPTeleportTypes.TeamTeleport:
                    {
                        if (DateTime.Now > user.LastVIPTeamTeleport.AddSeconds(1))
                        {
                            if (user.Team != null)
                            {
                                packet = packet.VipHandlerCreate(VIPTeleportTypes.TeammateConfirmation, Locations, 15, user.Player.Name);

                                foreach (var member in user.Team.Temates)
                                {
                                    if (member.client.Player.UID != user.Player.UID)
                                    {
                                        Qahr.Mobile.Phone.MobileVipConfirm.Offer(member.client.Player.UID, Locations, 15);// QahrMobile: the answer must match a request
                                        member.client.Send(packet);
                                    }
                                }
                            }
                            Teleport(user, Locations);
                            user.LastVIPTeamTeleport = DateTime.Now;
                        }
                        else
                        {
#if Arabic
                                   user.SendSysMesage("You`ve recently used the City Teleport (Team) service. Please wait " + (user.LastVIPTeamTeleport.AddSeconds(300).AllSeconds() - DateTime.Now.AllSeconds()).ToString() + " seconds to send the request for City Teleport (Team) again.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                     
#else
                            user.SendSysMesage("You`ve recently used the City Teleport (Team) service. Please wait " + (user.LastVIPTeamTeleport.AddSeconds(1).AllSeconds() - DateTime.Now.AllSeconds()).ToString() + " seconds to send the request for City Teleport (Team) again.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

#endif
                        }
                        break;
                    }
                case VIPTeleportTypes.TeammateTeleport:
                    {
                        if (!Qahr.Mobile.Phone.MobileVipConfirm.Take(user.Player.UID, Locations)) break;// QahrMobile: only after a team teleport request
                        Teleport(user, Locations);
                        break;
                    }
            }
        }

        public static void Teleport(Client.GameClient user, VIPTeleportLocations Location)
        {
            if (user.Player.Map == 10137 || user.Player.Map == 10250)
            {
                user.SendSysMesage("Teleport function can only be enabled when you`re in the major cities, Dungeon, sea, labyrinth, Frozen Grotto, Fiery Dragon region or Tower of Mystery.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            switch (Location)
            {
                case VIPTeleportLocations.TwinCity:
                case VIPTeleportLocations.TCSquare: user.Teleport(410, 354, 1002); break;
                case VIPTeleportLocations.WPAltar: user.Teleport(439, 666, 1002); break;
                case VIPTeleportLocations.WPApparation: user.Teleport(250, 656, 1002); break;
                case VIPTeleportLocations.WPPoltergiest: user.Teleport(104, 581, 1002); break;
                case VIPTeleportLocations.WPTurtledove: user.Teleport(416, 608, 1002); break;

                case VIPTeleportLocations.PhoenixCastle:
                case VIPTeleportLocations.PCSqaure: user.Teleport(184, 275, 1011); break;
                case VIPTeleportLocations.MFWaterCave: user.Teleport(380, 31, 1011); break;
                case VIPTeleportLocations.MFVillage: user.Teleport(785, 472, 1011); break;
                case VIPTeleportLocations.MFLake: user.Teleport(369, 568, 1011); break;
                case VIPTeleportLocations.MFMineCave: user.Teleport(924, 560, 1011); break;
                case VIPTeleportLocations.MFBridge: user.Teleport(648, 567, 1011); break;
                case VIPTeleportLocations.MFToApeCity: user.Teleport(475, 841, 1011); break;

                case VIPTeleportLocations.ApeCity:
                case VIPTeleportLocations.ACSquare: user.Teleport(567, 567, 1020); break;
                case VIPTeleportLocations.ACSouth: user.Teleport(699, 640, 1020); break;
                case VIPTeleportLocations.ACEast: user.Teleport(624, 337, 1020); break;
                case VIPTeleportLocations.ACNorth: user.Teleport(200, 224, 1020); break;
                case VIPTeleportLocations.ACWest: user.Teleport(322, 621, 1020); break;

                case VIPTeleportLocations.DesertCity:
                case VIPTeleportLocations.DCSquare: user.Teleport(502, 655, 1000); break;
                case VIPTeleportLocations.DCSouth: user.Teleport(758, 750, 1000); break;
                case VIPTeleportLocations.DCMoonSpring: user.Teleport(291, 450, 1000); break;
                case VIPTeleportLocations.DCAncientMaze: user.Teleport(87, 321, 1000); break;

                case VIPTeleportLocations.BirdIland:
                case VIPTeleportLocations.BISquare: user.Teleport(725, 574, 1015); break;
                case VIPTeleportLocations.IslandVillage: user.Teleport(329, 375, 1015); break;
                case VIPTeleportLocations.BIWest: user.Teleport(520, 730, 1015); break;
                case VIPTeleportLocations.BINorth: user.Teleport(400, 303, 1015); break;
                case VIPTeleportLocations.BIEast: user.Teleport(572, 370, 1015); break;
                // QahrMobile: mobile-only points of viptrans.ini (Data\mobile_layouts\vip.txt maps them to 32-34)
                default: Qahr.Mobile.Phone.MobileVip.UnknownTeleport(user, (uint)Location); break;
            }
        }


    }
}
