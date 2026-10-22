using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetOfflineTG(this ServerSockets.Packet stream, out MsgOfflineTG.Mode Mode)
        {
            Mode = (MsgOfflineTG.Mode)stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet OfflineTGCreate(this ServerSockets.Packet stream, MsgOfflineTG.Mode Mode)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(0);


            stream.Finalize(GamePackets.MsgTraining);

            return stream;
        }


    }
    public struct MsgOfflineTG
    {
        public enum Mode : uint
        {
            OnConfirmation = 0,
            Disconnect = 1,
            UnKnow = 2,
            ReplyToConfirmation = 3,
            ClaimExperience = 4
        }

        [PacketAttribute(Game.GamePackets.MsgTraining)]
        public unsafe static void OfflineTG(Client.GameClient client, ServerSockets.Packet packet)
        {
            MsgOfflineTG.Mode Action;

            packet.GetOfflineTG(out Action);

            switch (Action)
            {
                case Mode.OnConfirmation:
                    {
                        if (client.Player.OnMyOwnServer == false)
                            return;
                        if (!client.Player.Alive)
                        {
#if Arabic
                              client.SendSysMesage("Please revive your character.");
#else
                            client.SendSysMesage("Please revive your character.");
#endif

                            break;
                        }
                        if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                      || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                  || client.Player.Map == 1806
                                          || client.Player.Map == 5051 || client.Player.Map == 5052
                        || client.Player.Map == 5053 || client.Player.Map == 5054
                        || client.Player.Map == 5055 || client.Player.Map == 5056
                        || client.Player.Map == 5057 || client.Player.Map == 5058

                        || client.Player.Map == 5061
                        || client.Player.Map == 5062 || client.Player.Map == 5063
                        || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066
                      || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                      || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
                       || Pool.Constants.SkillsNotAvailableHere.Contains(client.Player.Map)
              || client.Player.Map == 1768 || client.Player.Map == 5040 || client.Player.Map == 1770

              || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1138 || client.Player.Map == 10134 || client.Player.Map == 10133 || client.Player.Map == 3071 || client.Player.Map == 11996 || client.Player.Map == 1508 || client.Player.Map == 1507
                            || client.Player.Map == 11 || client.Player.Map == 7 || client.Player.Map == 5 || client.Player.Map == 15 || client.Player.Map == 14 || client.Player.Map == 12 || client.Player.Map == 8 || client.Player.Map == 20 || client.Player.Map == 1730
                            || client.Player.Map == 10134 || client.Player.Map == 12099 || client.Player.Map == 10133 || client.Player.Map == 13140 || client.Player.Map == 12344 || client.Player.Map == 10070 || client.Player.Map == 10504 || client.Player.Map == 12344 || client.Player.Map == 12816 || client.Player.Map == 6521 || client.Player.Map == 95152
                            || client.Player.Map == 12667 || client.Player.Map == 3030 || client.Player.Map == 10741 || client.Player.Map == 12275 || client.Player.Map == 12602 || client.Player.Map == 10741 || client.Player.Map == 2071 || client.Player.Map == 3030 || client.Player.Map == 10133 || client.Player.Map == 12546 || client.Player.Map == 12546 || client.Player.Map == 9855 || client.Player.Map == 66666 || client.Player.Map == 33333 || client.Player.Map == 22222 || client.Player.Map == 88888 || client.Player.Map == 12602 || client.Player.Map == 12605
                            || client.Player.Map == 11222 || client.Player.Map == 2071)
                        {
#if Arabic
                                         client.SendSysMesage("The Offline TG is not allowed on this map.");
#else
                            client.SendSysMesage("The Offline TG is not allowed on this map.");
#endif

                            break;
                        }

                        if (client.Player.Map == 1038 || client.Player.Map == 8009 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                            || client.Player.Map == 6001)
                        {
#if Arabic
                                         client.SendSysMesage("The Offline TG is not allowed on this map.");
#else
                            client.SendSysMesage("The Offline TG is not allowed on this map.");
#endif

                            break;
                        }
                        if (client.Player.Map != 601)
                        {
                            if (client.Player.Map != 1036)
                            {
                                client.Player.PMap = client.Player.Map;
                                client.Player.PMapX = client.Player.X;
                                client.Player.PMapY = client.Player.Y;
                            }
                            client.Map.View.LeaveMap<Role.IMapObj>(client.Player);
                            client.Player.Map = 601;
                            client.Player.X = 64;
                            client.Player.Y = 56;
                            client.Player.JoinOnflineTG = DateTime.Now;
                        }
                        client.Send(packet.OfflineTGCreate(Mode.Disconnect));
                        break;
                    }
                case Mode.ClaimExperience:
                    {
                        if (client.Player.Map == 601)
                        {
                            var T1 = new TimeSpan(DateTime.Now.Ticks);
                            var T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                            ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                            minutes = (ushort)Math.Min((ushort)900, minutes);
                            client.Player.JoinOnflineTG = DateTime.Now;
                            client.GainExpBall(minutes, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Teleport(client.Player.PMapX, client.Player.PMapY, client.Player.PMap);
                        }
                        break;
                    }

            }
        }
    }
}
