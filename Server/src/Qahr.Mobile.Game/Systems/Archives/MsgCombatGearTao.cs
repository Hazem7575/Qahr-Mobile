using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgCombatGearTao
    {

        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public Action Action;

            [ProtoMember(2, IsRequired = true)]
            public uint Pos;

            [ProtoMember(3, IsRequired = true)]
            public uint ReikiPts;

            [ProtoMember(4, IsRequired = true)]
            public uint Skip;

            [ProtoMember(5, IsRequired = true)]
            public uint unk1;

            [ProtoMember(6, IsRequired = true)]
            public uint unk2;

            [ProtoMember(7, IsRequired = true)]
            public uint MasteryPointss;

            [ProtoMember(8, IsRequired = true)]
            public uint Count;

            [ProtoMember(9, IsRequired = true)]
            public uint UniversalFragment;

            // [ProtoFix 2026-10-14] client CMsgDaoQiPB.PrirateLottery #10 is a single uint32 (was List<uint>, never filled).
            // Not IsRequired so 0 is omitted, same bytes as the old empty list.
            [ProtoMember(10)]
            public uint PrirateLottery;

            [ProtoMember(11, IsRequired = true)]
            public uint Postion;

            [ProtoMember(12, IsRequired = true)]
            public uint unk4;

            [ProtoMember(13, IsRequired = true)]
            public List<JadeBag> Items = new List<JadeBag>();

            [ProtoMember(14, IsRequired = true)]
            public long SameEffect;

            [ProtoMember(15, IsRequired = true)]
            public Role.Instance.Archives.EffectFlags BeastFlags;//Flags

            [ProtoMember(16, IsRequired = true)]
            public uint UID;//1006448

        }

        [ProtoContract]
        public class JadeBag
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ItemID;

            [ProtoMember(2, IsRequired = true)]
            public uint unk5;
        }
        public static unsafe ServerSockets.Packet CMsgDaoQiOneTime(this ServerSockets.Packet stream, bool done)
        {
            stream.InitWriter();
            var SD = new ProtoStructure();
            SD.Action = (Action)12;
            //if (done)
            //    SD.Skip = 31;
            //else
            SD.Skip = 23;
            stream.ProtoBufferSerialize(SD);
            stream.Finalize(GamePackets.MsgDaoQi);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateTao(this ServerSockets.Packet stream, ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgDaoQi);
            return stream;
        }
        public static unsafe void GetTao(this ServerSockets.Packet stream, out ProtoStructure pQuery)
        {
            pQuery = new ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<ProtoStructure>(pQuery);
        }

        public enum Action : uint
        {
            Login = 0,
            JadeList = 1,
            BuyReiki = 2,
            AddReiki = 3,
            Draw = 4,
            EquipJade = 5,
            UnequipJade = 6,
            JadeListPirate = 7,
            Replace = 8,
            UpdateRunes = 9,
            Into = 11,
            ActiveNew = 14,
            Unk = 15,
            LoginEffect = 16,
            FlagEffect = 17,

        }
        [PacketAttribute(GamePackets.MsgDaoQi)]
        private unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade)
                return;
            if (client.Player.ObjInteraction != null)
                return;
            if (client.InTrade  || client.IsVendor || !client.Player.Alive)
                return;
            ProtoStructure pQuery;
            stream.GetTao(out pQuery);
            switch (pQuery.Action)
            {
                case Action.Login:
                    {
                        pQuery.ReikiPts = client.MyArchives.Reiki;
                        pQuery.MasteryPointss = client.MyArchives.MasteryPoints;
                        pQuery.UniversalFragment = client.MyArchives.UniversalFragment;

                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            pQuery.Skip = client.HeavenVector;

                        if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            pQuery.Skip = 12;
                        }

                        client.Send(stream.CreateTao(pQuery));
                        break;
                    }
                case Action.JadeList:
                    {
                        foreach (var rune in client.MyArchives.JadeBag.Values)
                        {
                            pQuery.Items.Add(new JadeBag { ItemID = rune.ItemID });
                        }
                        client.Send(stream.CreateTao(pQuery));

                        break;
                    }
                case Action.BuyReiki:
                    {
                        uint Cost = 1;
                        uint count = pQuery.Postion * 15;
                        long totalCost = Cost * count;
                        int Amount = (int)totalCost;
                        if (Amount < 0)
                            return;
                        if (client.Player.ConquerPoints >= (totalCost))
                        {
                            client.MyArchives.AddReiki(count);
                            client.Player.ConquerPoints -= (uint)(totalCost);
                        }
                        else
                        {
                            totalCost = Cost * count / 15;
                            if (client.Player.ConquerPoints >= (totalCost))
                            {
                                client.MyArchives.AddReiki(count);
                                client.Player.ConquerPoints -= (uint)(totalCost);
                            }

                        }
                        break;
                    }
                case Action.Draw:
                    {
                        if (client.MyArchives.JadeBag.Count == 0)
                        {
                            pQuery.Pos = 1;
                            pQuery.Count = 2000;
                            var x = new List<uint>() { 113, 113, 1013, 1413, 4313, 2513, 4313, 413, 2213, 2813 };
                            pQuery.Items = new List<JadeBag>();
                            for (int i = 0; i < x.Count; i++)
                            {
                                pQuery.Items.Add(new JadeBag { ItemID = x[i] });
                                client.MyArchives.AddRune(x[i]);
                            }
                            client.MyArchives.MasteryPoints = 2000;
                            client.Send(stream.CreateTao(pQuery));
                            break;
                        }
                        var cost = pQuery.Postion == 1 ? 13500 : 1500;
                        #region AutoReiki
                        if (pQuery.unk4 == 1)
                        {
                            if (client.MyArchives.Reiki < cost)
                            {

                                var Diff = cost - client.MyArchives.Reiki;
                                var DiffCost = pQuery.Skip == 1 ? Diff / 15 : Diff * 6999;
                                int Amount = (int)DiffCost;
                                if (Amount < 0)
                                    return;
                                if (pQuery.Skip == 1 && client.Player.ConquerPoints >= DiffCost)
                                {
                                    client.Player.ConquerPoints -= (uint)DiffCost;
                                    client.MyArchives.Reiki += (uint)Diff;
                                    UpdateReiki(client);
                                }
                                else if (pQuery.Skip == 0 && client.Player.Money >= (ulong)DiffCost)
                                {
                                    client.Player.Money -= (ulong)DiffCost;
                                    client.MyArchives.Reiki += (uint)Diff;
                                    UpdateReiki(client);
                                }
                            }
                        }
                        #endregion
                        if (client.MyArchives.Reiki >= cost)
                        {
                            client.MyArchives.Reiki -= (uint)cost;
                            UpdateReiki(client);
                            pQuery.Pos = 1;
                            var times = pQuery.Postion == 1 ? 10 : 1;
                            var daoqi = daoqi_dict_type.Items.Values.ToArray();
                            pQuery.Postion = 0;
                            pQuery.Items = new List<JadeBag>();
                            for (int i = 0; i < times; i++)
                            {
                                daoqi_dict_type.Item selected = null;
                                if (Role.Core.Rate(1))
                                {
                                    var list = daoqi.Where(p => p.Type == daoqi_dict_type.Type.Sacred && p.attr_type1 < 24).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(2))
                                {
                                    var list = daoqi.Where(p => p.Type == daoqi_dict_type.Type.Super && p.attr_type1 < 24).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else if (Role.Core.Rate(9))
                                {
                                    var list = daoqi.Where(p => p.Type == daoqi_dict_type.Type.Elite && p.attr_type1 < 24).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                else
                                {
                                    var list = daoqi.Where(p => p.Type == daoqi_dict_type.Type.Refined && p.attr_type1 < 24).ToArray();
                                    selected = list[Program.GetRandom.Next(list.Length)];
                                }
                                if (!client.MyArchives.AddRune(selected.ID))
                                {
                                    pQuery.Postion += selected.points;
                                    client.MyArchives.MasteryPoints += selected.points;
                                }
                                pQuery.Items.Add(new JadeBag { ItemID = selected.ID });
                            }
                            client.Send(stream.CreateTao(pQuery));
                        }

                        break;
                    }
                case Action.EquipJade:
                    {
                        Archives.Item weapon;
                        var Item = pQuery.Items.FirstOrDefault();
                        if (client.MyArchives.Items.TryGetValue((Archives.TypeID)pQuery.Pos, out weapon))
                        {
                            for (int i = 0; i < weapon.Jades.Length; i++)
                            {
                                if (weapon.Jades[i].JadeID == Item.ItemID)
                                    return;
                            }
                            switch (pQuery.Postion)
                            {
                                case 1:
                                    weapon.Jades[0].JadeID = Item.ItemID;
                                    break;
                                case 2:
                                    weapon.Jades[1].JadeID = Item.ItemID;
                                    break;
                                case 3:
                                    weapon.Jades[2].JadeID = Item.ItemID;
                                    break;
                                case 4:
                                    weapon.Jades[3].JadeID = Item.ItemID;
                                    break;
                                case 5:
                                    weapon.Jades[4].JadeID = Item.ItemID;
                                    break;
                                case 6:
                                    weapon.Jades[5].JadeID = Item.ItemID;
                                    break;
                            }
                            pQuery.unk4 = 1;
                            var Rune = weapon.Jades.ToArray()[pQuery.Postion - 1];
                            client.MyArchives.AddRuneSkill((uint)Rune.JadeID);
                            client.Send(stream.CreateTao(pQuery));
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);

                        }
                        break;
                    }
                case Action.UnequipJade:
                    {

                        Archives.Item Pweapon;
                        if (client.MyArchives.Items.TryGetValue((Archives.TypeID)pQuery.Pos, out Pweapon))
                        {
                            var Rune = Pweapon.Jades.ToArray()[pQuery.Postion - 1];
                            uint IDRune = (uint)Rune.JadeID;
                            switch (pQuery.Postion)
                            {
                                case 1:
                                    Pweapon.Jades[0].JadeID = 0;
                                    break;
                                case 2:
                                    Pweapon.Jades[1].JadeID = 0;
                                    break;
                                case 3:
                                    Pweapon.Jades[2].JadeID = 0;
                                    break;
                                case 4:
                                    Pweapon.Jades[3].JadeID = 0;
                                    break;
                                case 5:

                                    Pweapon.Jades[4].JadeID = 0;
                                    break;
                                case 6:
                                    Pweapon.Jades[5].JadeID = 0;
                                    break;
                            }
                            pQuery.unk4 = 1;
                            client.Send(stream.CreateTao(pQuery));
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            client.MyArchives.RemoveRuneSkill(IDRune);

                        }

                        break;
                    }
                case Action.Replace:
                    {
                        Archives.Item weapon;
                        if (client.MyArchives.Items.TryGetValue((Archives.TypeID)pQuery.Pos, out weapon))
                        {
                            for (int i = 0; i < pQuery.Items.Count; i++)
                            {
                                weapon.Jades[i].JadeID = pQuery.Items[i].ItemID;

                                pQuery.unk4 = 1;
                            }
                        }
                        client.Send(stream.CreateTao(pQuery));
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case Action.UpdateRunes:
                    {

                        var BlackJade = client.MyArchives.JadeBag.Values.FirstOrDefault(p => p.ItemID == pQuery.Pos);
                        if (BlackJade != null)
                        {
                            if (BlackJade.ItemID % 1000 < 900 && client.MyArchives.UniversalFragment >= BlackJade.DBJadeList.Exp)
                            {
                                BlackJade.ItemID += 100;
                                pQuery.UniversalFragment = BlackJade.ItemID;
                                pQuery.Postion = 1;
                                client.MyArchives.UniversalFragment -= BlackJade.DBJadeList.Exp;
                                MsgCombatGearTao.AddFragemnt(client);
                                client.Send(stream.CreateTao(pQuery));
                                client.MyArchives.UpdateNut();
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);


                            }
                        }
                        break;
                    }
                case Action.FlagEffect:
                    {
                        if (pQuery.BeastFlags != Archives.EffectFlags.None)
                            client.MyArchives.Effect = (Archives.EffectFlags)pQuery.BeastFlags;
                        else
                            client.MyArchives.Effect = (Archives.EffectFlags)pQuery.BeastFlags;
                        pQuery.UID = client.Player.UID;
                        client.Player.View.SendView(client.Player.GetArray(stream, false), true);
                        client.Send(stream.CreateTao(pQuery));
                        break;
                    }
                case Action.ActiveNew:
                    {
                        if (client.MyArchives.PassedHeavenTaoistConditions())
                        {
                            if (pQuery.Skip == 1)
                            {
                                client.Player.TaoHeavenState = 1;
                                if (Database.AtributesStatus.IsFire(client.Player.Class))
                                    client.MySpells.UpdateSpellLevelOrAdd(stream, (ushort)Role.Flags.SpellID.StarChainFire, 1);
                                else if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    client.MySpells.UpdateSpellLevelOrAdd(stream, (ushort)Role.Flags.SpellID.StarChainWater, 1);
                                client.MyArchives.SetHeavenVector(stream, 1 << 5);
                            }
                            else
                            {
                                client.Player.TaoHeavenState = 0;
                                client.MyArchives.SetHeavenVector(stream, 0);
                            }
                        }

                        break;
                    }
                case Action.Unk:
                    {
                        pQuery.UID = client.Player.UID;
                        client.Send(stream.CreateTao(pQuery));
                        break;
                    }

            }
        }
        public static void AddFragemnt(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var pQuery = new ProtoStructure();
                pQuery.Action = Action.Into;
                pQuery.Skip = 12;
                pQuery.UniversalFragment = user.MyArchives.UniversalFragment;
                user.Send(stream.CreateTao(pQuery));
            }
        }
        public static void UpdateReiki(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                client.Send(stream.CreateTao(new MsgCombatGearTao.ProtoStructure() { Action = MsgCombatGearTao.Action.AddReiki, ReikiPts = client.MyArchives.Reiki }));
            }
        }
    }
}