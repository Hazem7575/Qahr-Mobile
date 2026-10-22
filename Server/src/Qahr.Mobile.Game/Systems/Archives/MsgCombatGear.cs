using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateArchives(this ServerSockets.Packet stream, MsgCombatGear.ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCombatGear);
            return stream;

        }

        public static void GetArchives(this ServerSockets.Packet stream, out MsgCombatGear.ProtoStructure pQuery)
        {
            pQuery = new MsgCombatGear.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgCombatGear.ProtoStructure>(pQuery);
        }

    }

    public class MsgCombatGear
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [Flags]
            public enum Action
            {
                Load = 0,
                Add = 2,
                Update = 3,
                View = 4,
            }
            [ProtoMember(1, IsRequired = true)]
            public byte Type;
            [ProtoMember(2, IsRequired = true)]
            public uint UID;
            [ProtoMember(3, IsRequired = true)]
            public ConstructsProto[] Items;
            [ProtoMember(4, IsRequired = true)]
            public byte Member4;
            // [ProtoFix 2026-10-14] client CMsgCombatGearPB #5 is a SINGLE CombatMartialArts (was List<MartialInfo>).
            // Null = not sent (the old empty list sent nothing either).
            [ProtoMember(5)]
            public MartialInfo MartialInfos;
            [ProtoContract]
            public class MartialInfo
            {
                [ProtoMember(1, IsRequired = true)]
                public byte Member1 { get; set; }

                [ProtoMember(2, IsRequired = true)]
                public uint Member2 { get; set; }
            }
            [ProtoContract]
            public class ConstructsProto
            {
                [ProtoMember(1, IsRequired = true)]
                public ushort ID;
                [ProtoMember(2, IsRequired = true)]
                public byte Level;
                [ProtoMember(3, IsRequired = true)]
                public uint Progress;
                [ProtoMember(4, IsRequired = true)]
                public byte Hash;
                [ProtoMember(5, IsRequired = true)]
                public uint dwParam;
                [ProtoMember(6, IsRequired = true)]
                public uint jade1;
                [ProtoMember(7, IsRequired = true)]
                public uint jade2;
                [ProtoMember(8, IsRequired = true)]
                public uint jade3;
                [ProtoMember(9, IsRequired = true)]
                public uint jade4;
                [ProtoMember(10, IsRequired = true)]
                public uint jade5;
                [ProtoMember(11, IsRequired = true)]
                public uint jade6;
                [ProtoMember(12, IsRequired = true)]
                public uint MasteryPoints = 0;
                [ProtoMember(13, IsRequired = true)]
                public uint Debris = 0;
            }


            public static void Create(GameClient user, Archives.Item Item, Action Type)
            {

                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    ServerSockets.Packet stream = rec.GetStream();
                    ProtoStructure obj = new ProtoStructure
                    {
                        Type = (byte)Type,
                        UID = user.Player.UID,
                        Items = new ConstructsProto[1]
                    };
                    obj.Items[0] = new ConstructsProto
                    {
                        ID = (ushort)Item.ItemID,
                        Level = (byte)Item.Level,
                        Progress = Item.Progress,
                        Hash = Item.Hash,
                        dwParam = Item.dwParam,
                        jade1 = Item.Jades[0].JadeID,
                        jade2 = Item.Jades[1].JadeID,
                        jade3 = Item.Jades[2].JadeID,
                        jade4 = Item.Jades[3].JadeID,
                        jade5 = Item.Jades[4].JadeID,
                        jade6 = Item.Jades[5].JadeID,
                        MasteryPoints = Item.MasteryPoints,
                        Debris = Item.Debris,

                    };
                    user.Send(stream.CreateArchives(obj));
                }
            }


            [PacketAttribute(GamePackets.MsgCombatGear)]
            private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
            {
                ProtoStructure pQuery;
                stream.GetArchives(out pQuery);
                switch (pQuery.Type)
                {
                    case (byte)Action.View:
                        {
                            #region view
                            Client.GameClient Target;
                            if (Pool.GamePoll.TryGetValue(pQuery.UID, out Target))
                            {

                                Archives.Item[] MyArchvies = null;
                                if (AtributesStatus.IsLee(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragon && p.ItemID <= Archives.TypeID.Suanni).ToArray();
                                else if (AtributesStatus.IsPirate(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.ThunderNut && p.ItemID <= Archives.TypeID.LavaNut).ToArray();
                                else if (AtributesStatus.IsTaoist(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Vicissitude && p.ItemID <= Archives.TypeID.BirthDeath).ToArray();
                                else if (AtributesStatus.IsArcher(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.StoneCracker && p.ItemID <= Archives.TypeID.ThornCutter).ToArray();
                                else if (AtributesStatus.IsMonk(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.HeavenlyTiger && p.ItemID <= Archives.TypeID.CosmicRoc).ToArray();
                                else if (AtributesStatus.IsWarrior(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Redcurse).ToArray();
                                else if (AtributesStatus.IsDune(Target.Player.Class))
                                    MyArchvies = Target.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Conception && p.ItemID <= Archives.TypeID.Belt).ToArray();


                                if (MyArchvies != null)
                                {
                                    ProtoStructure obj = new ProtoStructure();
                                    obj.Type = (byte)Action.View;
                                    obj.UID = Target.Player.UID;
                                    obj.Items = new ConstructsProto[Target.MyArchives.Items.Count];
                                    int i = 0; foreach (var Item in MyArchvies)
                                    {
                                        obj.Items[i] = new ConstructsProto();
                                        obj.Items[i].ID = (ushort)Item.ItemID;
                                        obj.Items[i].Level = (byte)Item.Level;
                                        obj.Items[i].Progress = Item.Progress;
                                        obj.Items[i].Hash = Item.Hash;
                                        obj.Items[i].dwParam = Item.dwParam;
                                        obj.Items[i].jade1 = Item.Jades[0].JadeID;
                                        obj.Items[i].jade2 = Item.Jades[1].JadeID;
                                        obj.Items[i].jade3 = Item.Jades[2].JadeID;
                                        obj.Items[i].jade4 = Item.Jades[3].JadeID;
                                        obj.Items[i].jade5 = Item.Jades[4].JadeID;
                                        obj.Items[i].jade6 = Item.Jades[5].JadeID;
                                        obj.Items[i].MasteryPoints = Item.MasteryPoints;
                                        obj.Items[i].Debris = Item.Debris;
                                        i++;
                                    }
                                    user.Send(stream.CreateArchives(obj));

                                }
                            }
                            #endregion
                            break;
                        }
                    case 5:
                        {
                            // [ProtoFix 2026-10-14] single CombatMartialArts
                            pQuery.MartialInfos = new MartialInfo()
                             {
                                 Member1 = 3,
                                 Member2 = user.Player.DunePoints,
                             };
                            user.Send(stream.CreateArchives(pQuery));
                            break;
                        }
                    case 6:
                        {
                            pQuery.UID = user.MyArchives.DuneTotalScore;
                            pQuery.Member4 = 0;
                            user.Send(stream.CreateArchives(pQuery));
                            break;
                        }
                }
            }
        }
    }
}

