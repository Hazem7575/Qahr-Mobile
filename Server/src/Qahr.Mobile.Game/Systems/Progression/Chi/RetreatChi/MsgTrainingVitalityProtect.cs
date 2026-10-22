using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateExpireNotify(this ServerSockets.Packet stream, List<MsgChiInfo.ChiPowerType> Ids)
        {
            stream.InitWriter();
            stream.Write((uint)Ids.Count);
            foreach (var gate in Ids)
            {
                stream.Write((byte)gate);
                stream.Write((byte)1);
                // QahrMobile: a mobile unit is 2 bytes (slot, state; CMsgTrainingVitalityExpiryNotify::Process), the
                // third byte of the PC form shifted every unit after the first
            }
            stream.Finalize(GamePackets.MsgTrainingVitalityExpiryNotify);
            return stream;
        }
        public static unsafe void GetChiRetreatHandler(this ServerSockets.Packet stream, out MsgTrainingVitalityProtect.RetreatType Type, out MsgChiInfo.ChiPowerType Mode, out uint Item)
        {
            Type = (MsgTrainingVitalityProtect.RetreatType)stream.ReadUInt16();
            Mode = (MsgChiInfo.ChiPowerType)stream.ReadUInt8();
            Item = stream.ReadUInt8();
        }
        public static unsafe ServerSockets.Packet CreateRetreatedChiPacket(this ServerSockets.Packet stream,MsgTrainingVitalityProtect.RetreatType Type, MsgChiInfo.ChiPowerType Mode)
        {
            stream.InitWriter();
            stream.Write((ushort)Type);
            stream.Write((byte)Mode);
            // QahrMobile: CMsgTrainingVitalityProtect::Process reads a u32 at 7 for 1 / 7 (OnProtectSuccess /
            // OnProtectDelaySuccess: the end date, 0 = the "no date" message); without it the seal was read there
            stream.Write((uint)0);
            return stream;
        }
        public static unsafe ServerSockets.Packet ChiRetreatFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTrainingVitalityProtect);
            return stream;
        }
    }
    public unsafe struct MsgTrainingVitalityProtect
    {
        public enum RetreatType
        {
            Activate = 0,
            Retreating = 1,
            Seal = 2,
            Seal2 = 3,
            Restore = 4,
            Restore2 = 5,
            Extend = 6,
            Extend2 = 7,
            CancelRetreat = 8,
        }
        [PacketAttribute(GamePackets.MsgTrainingVitalityProtect)]
        private static void HandleRetreatChi(Client.GameClient user, ServerSockets.Packet stream)
        {
            RetreatType Action;
            MsgChiInfo.ChiPowerType Mode;
            uint item;
            stream.GetChiRetreatHandler(out Action, out Mode, out item);
            switch (Action)
            {
                #region Activate
                case RetreatType.Activate:
                    {
                        if (item == 0 && user.Inventory.Remove(3005360, 1, stream))
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        user.Player.MyChi.DragonPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Dragon.Attributes);
                                        user.Player.MyChi.DragonTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        user.Player.MyChi.PhoenixPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Phoenix.Attributes);
                                        user.Player.MyChi.PhoenixTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        user.Player.MyChi.TigerPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Tiger.Attributes);
                                        user.Player.MyChi.TigerTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        user.Player.MyChi.TurtlePowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Turtle.Attributes);
                                        user.Player.MyChi.TurtleTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                            }

                        }
                        else if (item == 1 && user.Inventory.Remove(3321066, 1, stream))
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        user.Player.MyChi.DragonPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Dragon.Attributes);
                                        user.Player.MyChi.DragonTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        user.Player.MyChi.PhoenixPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Phoenix.Attributes);
                                        user.Player.MyChi.PhoenixTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        user.Player.MyChi.TigerPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Tiger.Attributes);
                                        user.Player.MyChi.TigerTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        user.Player.MyChi.TurtlePowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Turtle.Attributes);
                                        user.Player.MyChi.TurtleTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Retreating, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.SaveClient(user);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                #endregion
                #region Seal
                case RetreatType.Seal:
                    {
                        switch (Mode)
                        {
                            case MsgChiInfo.ChiPowerType.Dragon:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.DragonTime) && user.Player.MyChi.DragonTime != 1)
                                        break;
                                    user.Player.MyChi.DragonPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Dragon.Attributes);
                                    stream.CreateRetreatedChiPacket(RetreatType.Seal2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Phoenix:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.PhoenixTime) && user.Player.MyChi.PhoenixTime != 1)
                                        break;
                                    user.Player.MyChi.PhoenixPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Phoenix.Attributes);
                                    stream.CreateRetreatedChiPacket(RetreatType.Seal2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Tiger:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.TigerTime) && user.Player.MyChi.TigerTime != 1)
                                        break;
                                    user.Player.MyChi.TigerPowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Tiger.Attributes);
                                    stream.CreateRetreatedChiPacket(RetreatType.Seal2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Turtle:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.TurtleTime) && user.Player.MyChi.TurtleTime != 1)
                                        break;
                                    user.Player.MyChi.TurtlePowers = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.Turtle.Attributes);
                                    stream.CreateRetreatedChiPacket(RetreatType.Seal2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion
                #region Extend
                case RetreatType.Extend:
                    {
                        if (item == 0 && user.Inventory.Remove(3005360, 1, stream))
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        user.Player.MyChi.DragonTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        user.Player.MyChi.PhoenixTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        user.Player.MyChi.TigerTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        user.Player.MyChi.TurtleTime = DateTime.Now.AddDays(30).ToBinary();
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                            }
                        }
                        else if (item == 1 && user.Inventory.Remove(3321066, 1, stream))
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        user.Player.MyChi.DragonTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        user.Player.MyChi.PhoenixTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        user.Player.MyChi.TigerTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        user.Player.MyChi.TurtleTime = 1;
                                        stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                                        user.Send(stream.ChiRetreatFinalize());
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                #endregion
                #region Restore
                case RetreatType.Restore:
                    {

                        switch (Mode)
                        {
                            case MsgChiInfo.ChiPowerType.Dragon:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.DragonTime) && user.Player.MyChi.DragonTime != 1)
                                        break;
                                    user.Player.MyChi.Dragon.Attributes = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.DragonPowers);
                                    Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, user.Player.MyChi.Dragon);
                                    stream.CreateRetreatedChiPacket(RetreatType.Restore2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Phoenix:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.PhoenixTime) && user.Player.MyChi.PhoenixTime != 1)
                                        break;
                                    user.Player.MyChi.Phoenix.Attributes = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.PhoenixPowers);
                                    Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Phoenix, user.Player.MyChi.Phoenix);
                                    stream.CreateRetreatedChiPacket(RetreatType.Restore2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Tiger:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.TigerTime) && user.Player.MyChi.TigerTime != 1)
                                        break;
                                    user.Player.MyChi.Tiger.Attributes = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.TigerPowers);
                                    Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Tiger, user.Player.MyChi.Tiger);
                                    stream.CreateRetreatedChiPacket(RetreatType.Restore2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Turtle:
                                {
                                    if (DateTime.Now > DateTime.FromBinary(user.Player.MyChi.TurtleTime) && user.Player.MyChi.TurtleTime != 1)
                                        break;
                                    user.Player.MyChi.Turtle.Attributes = Qahr.Mobile.Role.Instance.Chi.ChiAttribute.ShallowCopy(user.Player.MyChi.TurtlePowers);
                                    Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Turtle, user.Player.MyChi.Turtle);
                                    stream.CreateRetreatedChiPacket(RetreatType.Restore2, Mode);
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion
                #region Restore
                case RetreatType.CancelRetreat:
                    {

                        switch (Mode)
                        {
                            case MsgChiInfo.ChiPowerType.Dragon:
                                {
                                    user.Player.MyChi.DragonPowers = null;
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Phoenix:
                                {
                                    user.Player.MyChi.PhoenixPowers = null;
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Tiger:
                                {
                                    user.Player.MyChi.TurtlePowers = null;
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            case MsgChiInfo.ChiPowerType.Turtle:
                                {
                                    user.Player.MyChi.TigerPowers = null;
                                    user.Send(stream.ChiRetreatFinalize());
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion
                default: MyConsole.WriteLine("Unknown Chi Retreat Action Type [" + Action + "]."); break;
            }
        }
      
    }
}
