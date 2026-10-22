using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgGouYuOpt
    {
        [ProtoContract]
        public class MsgGouYuOptProto
        {
            [Flags]
            public enum TypeID
            {
                Equip = 0,
                Unequip = 1,
                UpLevel = 2,
                BoostAptitude = 4,
                SaveDiscard = 5,
                Hat = 6,
                AddPoints = 7,
                AddMastery = 8,
                InstantUpgrade = 9,
                UnlockMastery = 11,
            }
            [ProtoMember(1, IsRequired = true)]
            public TypeID Type = 0;
            [ProtoMember(2, IsRequired = true)]
            public uint ID = 0;
            [ProtoMember(3, IsRequired = true)]
            public uint Points = 0;
            [ProtoMember(4, IsRequired = true)]
            public uint Position = 0;
            public static MsgGouYuOptProto Info;
            public static void PrintPacket(byte[] packet)
            {
                foreach (byte D in packet)
                {
                    System.Console.Write((Convert.ToString(D, 16)).PadLeft(2, '0').ToUpper() + " ");
                }
                System.Console.Write("\n\n");
            }
            [PacketAttribute(GamePackets.MsgGouYuOpt)]
            private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
            {
                Random Random = new Random();
               
                stream.GetNinjaInfo(out Info);
                switch (Info.Type)
                {
                    case TypeID.Equip:
                        {
                            if (Info.Position == 1 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Dawn)
                                || Info.Position == 2 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Rest)
                                || Info.Position == 3 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Life)
                                || Info.Position == 4 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Pain)
                                || Info.Position == 5 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Limlt)
                                || Info.Position == 6 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_View)
                                || Info.Position == 7 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Shock)
                                || Info.Position == 8 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Death)
                                || Info.Position == 9 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Rest_2)
                                || Info.Position == 10 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Life_2)
                                || Info.Position == 11 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Pain_2)
                                || Info.Position == 12 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Limlt_2)
                                || Info.Position == 13 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_View_2)
                                || Info.Position == 14 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Shock_2)
                                || Info.Position == 15 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Death_2)
                                || Info.Position == 16 && user.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Death_2))
                            {
                                Ninja.Item item;
                                if (user.MyNinja.TryGetValue(Info.ID, out item))
                                {
                                    var Unequip = user.MyNinja.Items.Values.Where(p => p.Position == Info.Position).FirstOrDefault();
                                    if (Unequip != null)
                                    {
                                        Info.Type = TypeID.Unequip;
                                        Info.ID = Unequip.ItemID;
                                        Info.Position = Unequip.Position;
                                        Info.Points = Unequip.Level;
                                        user.Send(stream.CreateNinjaInfo(Info));
                                        Unequip.Position = 0;
                                        
                                    }
                                    Info.Type = TypeID.Equip;
                                    item.Position = Info.Position;
                                    Info.ID = item.ItemID;
                                    Info.Position = item.Position;
                                    Info.Points = item.Level;
                                    user.Send(stream.CreateNinjaInfo(Info));
                                  
                                    user.MyNinja.GetLevel();
                                    user.MyNinja.Alternate();
                                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                }
                            }
                            break;
                        }
                    case TypeID.Unequip:
                        {
                            var item = user.MyNinja.Items.Values.Where(p => p.Position == Info.Position).FirstOrDefault();
                            if (item != null)
                            {
                                item.Position = 0;
                                Info.ID = item.ItemID;
                                Info.Points = item.Level;
                                user.Send(stream.CreateNinjaInfo(Info));
                               
                            }
                            user.MyNinja.GetLevel();
                            user.MyNinja.Alternate();
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case TypeID.Hat:
                        {

                            if (Info.Points == 1)
                            {
                                if (Info.ID != (uint)Game.MsgServer.MsgUpdate.Flags.Overall)
                                {
                                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Fire);
                                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Water);
                                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Earth);
                                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Wind);
                                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Lightning);
                                }
                                user.Player.AddFlag((Game.MsgServer.MsgUpdate.Flags)Info.ID, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else if (Info.Points == 0)
                            {
                                user.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)Info.ID);
                            }
                            break;
                        }
                    case TypeID.UnlockMastery:
                        {
                            switch (Info.ID)
                            {
                                case 1: if (user.MyNinja.Fire_Score >= 14000) user.MyNinja.AddFlagMastery(Ninja.UnlockMasterys.FireMastery); break;
                                case 2: if (user.MyNinja.Water_Score >= 14000) user.MyNinja.AddFlagMastery(Ninja.UnlockMasterys.WaterMastery); break;
                                case 3: if (user.MyNinja.Earth_Score >= 14000) user.MyNinja.AddFlagMastery(Ninja.UnlockMasterys.EarthMastery); break;
                                case 4: if (user.MyNinja.Wind_Score >= 14000) user.MyNinja.AddFlagMastery(Ninja.UnlockMasterys.WindMastery); break;
                                case 5: if (user.MyNinja.Lightning_Score >= 14000) user.MyNinja.AddFlagMastery(Ninja.UnlockMasterys.LightningMastery); break;
                            }
                            MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(user, user.MyNinja, user.Player.UID);
                            user.Send(stream.CreateNinjaInfo(Info));
                            break;
                        }
                    case TypeID.AddMastery:
                        {
                            if (user.MyNinja.Exp >= Info.Points)
                            {
                                switch (Info.ID)
                                {
                                    case 1: user.MyNinja.FireMastery += Info.Points; break;
                                    case 2: user.MyNinja.WaterMastery += Info.Points; break;
                                    case 3: user.MyNinja.EarthMastery += Info.Points; break;
                                    case 4: user.MyNinja.WindMastery += Info.Points; break;
                                    case 5: user.MyNinja.LightningMastery += Info.Points; break;
                                }
                                user.MyNinja.Exp -= Info.Points;
                                user.MyNinja.AddPoints(0);
                                Info.Type = TypeID.UnlockMastery;
                                user.Send(stream.CreateNinjaInfo(Info));
                                user.MyNinja.UpdateRank();
                            }
                            break;
                        }
                    case TypeID.UpLevel:
                        {
                            Ninja.Item item;
                            if (user.MyNinja.TryGetValue(Info.ID, out item))
                            {
                                if (user.MyNinja.Exp >= Info.Points)
                                {
                                    user.MyNinja.Exp -= Info.Points;
                                    item.Exp += Info.Points;
                                    if (item.Exp >= item.DBItem.Exp)
                                    {
                                        item.Exp -= item.DBItem.Exp;
                                        item.Level++;
                                    }
                                    
                                    user.MyNinja.AddPoints(0);
                                    MsgGouYuInfo.MsgGouYuInfoProto.Create(user, item, MsgGouYuInfo.MsgGouYuInfoProto.TypeID.Update);
                                }
                            }
                            user.MyNinja.UpdateRank();
                            user.MyNinja.GetLevel();
                            user.MyNinja.Alternate();
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case TypeID.BoostAptitude:
                        {

                            if (user.MyNinja.Fire == 0 || user.MyNinja.Water == 0 || user.MyNinja.Earth == 0 || user.MyNinja.Wind == 0 || user.MyNinja.Lightning == 0)
                            {
                                user.MyNinja.Fire = 1;
                                user.MyNinja.Water = 1;
                                user.MyNinja.Earth = 1;
                                user.MyNinja.Wind = 1;
                                user.MyNinja.Lightning = 1;
                            }
                            else
                            {
                                var n = user.MyNinja;
                                uint t = n.Fire + n.Water + n.Earth + n.Wind + n.Lightning;
                                uint _i = 0;
                                uint _a = 0;
                                if (t <= 150)
                                {
                                    _i = 3327203;
                                    _a = 1;
                                }
                                if (t >= 150 && t <= 299)
                                {
                                    _i = 3327203;
                                    _a = 2;
                                }
                                if (t >= 300 && t <= 400)
                                {
                                    _i = 3330609;
                                    _a = 1;
                                }
                                if (t > 400)
                                {
                                    _i = 3330608;
                                    _a = 1;
                                }
                                if (_i != 0 && _a != 0)
                                {
                                    if (user.Inventory.Contain(_i, _a))
                                    {
                                        user.MyNinja.tmp_Fire = Math.Min(100, (uint)Random.Next(1, 5) + user.MyNinja.Fire);
                                        user.MyNinja.tmp_Water = Math.Min(100, (uint)Random.Next(1, 5) + user.MyNinja.Water);
                                        user.MyNinja.tmp_Earth = Math.Min(100, (uint)Random.Next(1, 5) + user.MyNinja.Earth);
                                        user.MyNinja.tmp_Wind = Math.Min(100, (uint)Random.Next(1, 5) + user.MyNinja.Wind);
                                        user.MyNinja.tmp_Lightning = Math.Min(100, (uint)Random.Next(1, 5) + user.MyNinja.Lightning);
                                        user.Inventory.Remove(_i, _a, stream);
                                       
                                    }
                                }
                            }
                            MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(user, user.MyNinja, user.Player.UID);
                            user.Send(stream.CreateNinjaInfo(Info));
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case TypeID.SaveDiscard:
                        {
                            switch (Info.ID)
                            {
                                case 1://Save
                                    {
                                        user.MyNinja.Fire = user.MyNinja.tmp_Fire;
                                        user.MyNinja.Water = user.MyNinja.tmp_Water;
                                        user.MyNinja.Earth = user.MyNinja.tmp_Earth;
                                        user.MyNinja.Wind = user.MyNinja.tmp_Wind;
                                        user.MyNinja.Lightning = user.MyNinja.tmp_Lightning;
                                        user.MyNinja.tmp_Fire = 0;
                                        user.MyNinja.tmp_Water = 0;
                                        user.MyNinja.tmp_Earth = 0;
                                        user.MyNinja.tmp_Wind = 0;
                                        user.MyNinja.tmp_Lightning = 0;
                                        break;
                                    }
                                case 0://Discard
                                    {
                                        user.MyNinja.tmp_Fire = 0;
                                        user.MyNinja.tmp_Water = 0;
                                        user.MyNinja.tmp_Earth = 0;
                                        user.MyNinja.tmp_Wind = 0;
                                        user.MyNinja.tmp_Lightning = 0;
                                        break;
                                    }

                            }
                            MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(user, user.MyNinja, user.Player.UID);
                            user.Send(stream.CreateNinjaInfo(Info));
                            user.MyNinja.UpdateRank();
                            break;
                        }
                    case TypeID.AddPoints:
                        {
                            uint Cost = 10; // 1x = 10 CPs
                            uint count = Info.Points;
                            long totalCost = Cost * count;
                            if (count > 5000000) // 5.000.000 Points Max Count
                                return;
                            if (user.Player.ConquerPoints >= (totalCost))
                            {
                                user.MyNinja.AddPoints(count);
                                user.Player.ConquerPoints -= (uint)(totalCost);
                            }
                            else
                            {
                                count = (uint)user.Player.ConquerPoints / Cost;
                                totalCost = Cost * count;
                                if (user.Player.ConquerPoints >= (totalCost))
                                {
                                    user.MyNinja.AddPoints(count);
                                    user.Player.ConquerPoints -= (uint)(totalCost);
                                }

                            }
                            break;
                        }
                    case TypeID.InstantUpgrade:
                        {
                            Ninja.Item item;
                            if (user.MyNinja.TryGetValue(Info.ID, out item))
                            {
                                if ((item.Exp * 100 / item.DBItem.Exp) > Random.Next() % 100)
                                {
                                    item.Exp = 0;
                                    item.Level++;
                                }
                                else
                                {
                                    item.Exp = 0;
                                }
                                
                                MsgGouYuInfo.MsgGouYuInfoProto.Create(user, item, MsgGouYuInfo.MsgGouYuInfoProto.TypeID.Update);
                            }
                            user.MyNinja.UpdateRank();
                            user.MyNinja.GetLevel();
                            user.MyNinja.Alternate();
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    default:
                        {
                            byte[] b = new byte[stream.Size];
                            fixed (byte* ptr = b)
                            {
                                stream.memcpy(ptr, stream.Memory, stream.Size);
                            }
                            PrintPacket(b);
                            break;
                        }

                }
            }
        }
        public static unsafe ServerSockets.Packet CreateNinjaInfo(this ServerSockets.Packet stream, MsgGouYuOptProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGouYuOpt);
            return stream;
        }
        public static unsafe void GetNinjaInfo(this ServerSockets.Packet stream, out MsgGouYuOptProto pQuery)
        {
            pQuery = new MsgGouYuOptProto();
            pQuery = stream.ProtoBufferDeserialize<MsgGouYuOptProto>(pQuery);
        }
    }
}
