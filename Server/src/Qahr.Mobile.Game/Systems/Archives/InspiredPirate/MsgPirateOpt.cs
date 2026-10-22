using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgPirateOpt
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;//Action
            [ProtoMember(2, IsRequired = true)]
            public uint UID;//UID
            [ProtoMember(3, IsRequired = true)]
            public uint ArchiveType;//ArchiveType
            // [ProtoFix 2026-10-14] client CMsgVigorPB #4 is REPEATED CVigorInfoPB (was a single Items).
            // ItemsList converts implicitly from one Items, so existing "ItemList = new Items{...}" senders keep working.
            [ProtoMember(4)]
            public ItemsList ItemList;//ItemList

            // [ProtoFix 2026-10-14]
            public class ItemsList : List<Items>
            {
                public static implicit operator ItemsList(Items item)
                {
                    var list = new ItemsList();
                    if (item != null)
                        list.Add(item);
                    return list;
                }
            }

            [ProtoContract]
            public class Items
            {
                [ProtoMember(1, IsRequired = true)]
                public uint Class;
                [ProtoMember(2, IsRequired = true)]
                public uint SenseLevel;//SenseLevel
                [ProtoMember(3, IsRequired = true)]
                public uint SenseExp;//SenseExp
                [ProtoMember(4, IsRequired = true)]
                public uint ArmedLevel;//ArmedLevel
                [ProtoMember(5, IsRequired = true)]
                public uint ArmedExp;//ArmedExp
                [ProtoMember(6, IsRequired = true)]
                public uint OverlordLevel;//OverlordLevel
                [ProtoMember(7, IsRequired = true)]
                public uint OverlordExp;//OverlordExp
                [ProtoMember(8, IsRequired = true)]
                public uint AirPower;

            }
        }
        public enum Type : uint
        {
            Login = 0,
            view = 1,
            UpdateAirPower = 3,
            UpdateLv = 4,
            AddPoints = 5,

        }
        public static unsafe ServerSockets.Packet CreatePirateOpt(this ServerSockets.Packet stream, ProtoStructure obj)
        {
           
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgVigor);
            return stream;
        }
        public static unsafe void GetPirateOpt(this ServerSockets.Packet stream, out ProtoStructure pQuery)
        {
            pQuery = new ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<ProtoStructure>(pQuery);
        }
        public static readonly uint[] Exp = new uint[] { 0, 40000, 120000, 200000, 280000, 360000, 480000, 640000, 828000 };
        [PacketAttribute(GamePackets.MsgVigor)]
        private unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade || client.IsVendor)
                return;
            ProtoStructure pQuery;
            stream.GetPirateOpt(out pQuery);
            switch (pQuery.Type)
            {
                case (uint)Type.view:
                    {
                        Client.GameClient Target;
                        if (Pool.GamePoll.TryGetValue(pQuery.UID, out Target))
                        {

                            var Action = new MsgPirateOpt.ProtoStructure();
                            Action.Type = (uint)MsgPirateOpt.Type.view;
                            Action.UID = Target.Player.UID;
                            Action.ArchiveType = 0;
                            Action.ItemList = new MsgPirateOpt.ProtoStructure.Items
                            {
                                Class = Target.Player.Class / 1000,
                                SenseLevel = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Sense) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Sense].Level : 1,
                                SenseExp = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Sense) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Sense].Exp : 1,

                                ArmedLevel = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Armed) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Armed].Level : 1,
                                ArmedExp = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Armed) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Armed].Exp : 1,

                                OverlordLevel = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Overlord) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Level : 1,
                                OverlordExp = Target.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Overlord) ? Target.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Exp : 1,

                                AirPower = client.MyArchives.AirPower
                            };
                            client.Send(stream.CreatePirateOpt(Action));
                        }
                        break;
                    }
               case  (uint)Type.UpdateLv:
                    {
                        Archives.items items;
                        if (client.MyArchives.AirPowers.TryGetValue((Archives.AirTypeID)pQuery.ArchiveType, out items))
                        {
                            ADDPowers(client, pQuery.ArchiveType, items.Level, items.Exp);
                            for (int i = 0; i < pQuery.UID; i++)
                            {
                                if (client.MyArchives.AirPower >= 1000)
                                {
                                    client.MyArchives.AirPower -= 1000;
                                    items.Exp += (uint)Program.GetRandom.Next(750, 1250);
                                }
                            }
                            UpdatePoints(client);
                            while (items.Level < Exp.Length && items.Exp >= Exp[items.Level])
                            {
                                items.Level++;
                                if (items.Level == Exp.Length)
                                {
                                    items.Exp = Exp[items.Level - 1];
                                    break;
                                }
                            }
                            UpdateAirPoints(client);
                            AddSkillAir(client);
                            client.MyArchives.UpdateRank();
                        }
                        break;
                    }
              

                default:
                    MyConsole.WriteLine("MsgPirateOpt Not Find: " + pQuery.Type);
                    break;
            }
        }
        public static void ADDPowers(Client.GameClient client, uint type, uint Level, uint Exp)
        {
            if (type == 701 && Level == 1 && Exp == 0)
            {
                var Power = new Archives.items();
                Power.Level = 1;
                Power.Exp = 0;
                Power.Type = (Archives.AirTypeID)702;
                client.MyArchives.AirPowers.Add((Archives.AirTypeID)702, Power);
               
              
            }
            else if (type == 701 && Level == 1 && Exp >= 1000 && Exp <= 11000)
            {
                var Power = new Archives.items();
                Power.Level = 1;
                Power.Exp = 0;
                Power.Type = (Archives.AirTypeID)703;
                client.MyArchives.AirPowers.Add((Archives.AirTypeID)703, Power);
            }
        }
        public static void UpdateAirPoints(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
               
                var pQuery = new ProtoStructure();
                pQuery.Type = (uint)Type.UpdateAirPower;
                pQuery.UID = 1;
                pQuery.ItemList = new ProtoStructure.Items
                {
                    Class = user.Player.Class / 1000,
                    SenseLevel = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Sense) ? user.MyArchives.AirPowers[Archives.AirTypeID.Sense].Level : 0,
                    SenseExp = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Sense) ? user.MyArchives.AirPowers[Archives.AirTypeID.Sense].Exp : 0,
                    ArmedLevel = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Armed) ? user.MyArchives.AirPowers[Archives.AirTypeID.Armed].Level : 0,
                    ArmedExp = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Armed) ? user.MyArchives.AirPowers[Archives.AirTypeID.Armed].Exp : 0,
                    OverlordLevel = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Overlord) ? user.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Level : 0,
                    OverlordExp = user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Overlord) ? user.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Exp : 0,
                    AirPower = user.MyArchives.AirPower

                };
                user.Send(stream.CreatePirateOpt(pQuery));
            }
        }
        public static void AddSkillAir(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Sense))
                {
                    if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Sense))
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Sense, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Sense].Level));
                    }
                    else
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Sense, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Sense].Level));
                    }
                }
                if (user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Armed))
                {
                    if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Armed))
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Armed, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Armed].Level));

                    }
                    else
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Armed, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Armed].Level));
                    }
                }
                if (user.MyArchives.AirPowers.ContainsKey(Archives.AirTypeID.Overlord))
                {
                    if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Overlord))
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Overlord, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Level));
                    }
                    else
                    {
                        user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Overlord, (ushort)(user.MyArchives.AirPowers[Archives.AirTypeID.Overlord].Level));
                    }
                }

            }
        }
        public static void UpdatePoints(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var pQuery = new ProtoStructure();
                pQuery.Type = (uint)Type.AddPoints;
                pQuery.UID = user.Player.UID;
                pQuery.ItemList = new ProtoStructure.Items { Class = user.Player.Class / 1000, AirPower = user.MyArchives.AirPower };
                user.Send(stream.CreatePirateOpt(pQuery));
            }
        }
     
    
       
    }
}