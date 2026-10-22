using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgGouYuAptitude
    {
        [ProtoContract]
        public class MsgGouYuAptitudeProto
        {
            public enum TypeID
            {
                Loading = 0,
                Flag = 1,
                Points = 2,
                ResonanceSusanoo = 4,
            }
            [ProtoMember(1, IsRequired = true)]
            public uint Type = 0;
            [ProtoMember(2, IsRequired = true)]
            public uint UID = 0;
            [ProtoMember(3, IsRequired = true)]
            public ulong Flag = 0;
            [ProtoMember(4, IsRequired = true)]
            public uint Points = 0;
            [ProtoMember(5, IsRequired = true)]
            public uint Fire = 0;
            [ProtoMember(6, IsRequired = true)]
            public uint Water = 0;
            [ProtoMember(7, IsRequired = true)]
            public uint Earth = 0;
            [ProtoMember(8, IsRequired = true)]
            public uint Wind = 0;
            [ProtoMember(9, IsRequired = true)]
            public uint Lightning = 0;
            [ProtoMember(10, IsRequired = true)]
            public uint FireNew = 0;
            [ProtoMember(11, IsRequired = true)]
            public uint WaterNew = 0;
            [ProtoMember(12, IsRequired = true)]
            public uint EarthNew = 0;
            [ProtoMember(13, IsRequired = true)]
            public uint WindNew = 0;
            [ProtoMember(14, IsRequired = true)]
            public uint LightningNew = 0;
            [ProtoMember(15, IsRequired = true)]
            public uint Score = 0;
            [ProtoMember(16, IsRequired = true)]
            public uint FireMastery = 0;
            [ProtoMember(17, IsRequired = true)]
            public uint WaterMastery = 0;
            [ProtoMember(18, IsRequired = true)]
            public uint EarthMastery = 0;
            [ProtoMember(19, IsRequired = true)]
            public uint WindMastery = 0;
            [ProtoMember(20, IsRequired = true)]
            public uint LightningMastery = 0;
            [ProtoMember(21, IsRequired = true)]
            public uint UnlockMastery = 0;
            [ProtoMember(22, IsRequired = true)]
            public uint SusanooLevel = 0; // [ProtoFix 2026-10-14] client #22 is uint32 (was int)
            [ProtoMember(23, IsRequired = true)]
            public uint SusanooExp = 0; // [ProtoFix 2026-10-14] client #23 is uint32 (was int)
            [ProtoMember(24, IsRequired = true)]
            public uint Unknow24 = 0;
            [ProtoMember(25, IsRequired = true)]
            public uint Unknow25 = 0;
            public static void Create(GameClient user, Ninja Ninja, uint UID)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgGouYuAptitudeProto obj = new MsgGouYuAptitudeProto();
                    obj.Type = (uint)MsgGouYuAptitudeProto.TypeID.Loading;
                    obj.UID = UID;
                    obj.Points = Ninja.Exp;
                    obj.Flag = Ninja.Flag;
                    obj.Fire = Ninja.Fire;
                    obj.Water = Ninja.Water;
                    obj.Earth = Ninja.Earth;
                    obj.Wind = Ninja.Wind;
                    obj.Lightning = Ninja.Lightning;
                    obj.FireNew = Ninja.tmp_Fire;
                    obj.WaterNew = Ninja.tmp_Water;
                    obj.EarthNew = Ninja.tmp_Earth;
                    obj.WindNew = Ninja.tmp_Wind;
                    obj.LightningNew = Ninja.tmp_Lightning;
                    obj.Score = Ninja.Score;
                    obj.FireMastery = Ninja.FireMastery;
                    obj.WaterMastery = Ninja.WaterMastery;
                    obj.EarthMastery = Ninja.EarthMastery;
                    obj.WindMastery = Ninja.WindMastery;
                    obj.LightningMastery = Ninja.LightningMastery;
                    obj.UnlockMastery = Ninja.UnlockMastery;
                    obj.SusanooLevel = Ninja.SusanooLevel; // [ProtoFix 2026-10-14] source is uint, no cast
                    obj.SusanooExp = Ninja.SusanooExp; // [ProtoFix 2026-10-14] source is uint, no cast
                    obj.Unknow24 = 0;
                    obj.Unknow25 = 0;
                    user.Send(stream.CreateNinjaUser(obj));
                }
            }

        }
        public static unsafe ServerSockets.Packet CreateNinjaUser(this ServerSockets.Packet stream, MsgGouYuAptitudeProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGouYuAptitude);
            return stream;
        }
        public static void GetNinjaUser(this ServerSockets.Packet stream, out MsgGouYuAptitude.MsgGouYuAptitudeProto pQuery)
        {
            pQuery = new MsgGouYuAptitude.MsgGouYuAptitudeProto();
            pQuery = stream.ProtoBufferDeserialize<MsgGouYuAptitude.MsgGouYuAptitudeProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgGouYuAptitude)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade || client.IsVendor)
                return;
            MsgGouYuAptitude.MsgGouYuAptitudeProto pQuery;
            stream.GetNinjaUser(out pQuery);
            switch ((MsgGouYuAptitudeProto.TypeID)pQuery.Type)
            {
                case MsgGouYuAptitudeProto.TypeID.ResonanceSusanoo:
                    {
                        if (client.MyNinja.Exp >= pQuery.Points)
                        {
                            client.MyNinja.Exp -= pQuery.Points;
                            client.MyNinja.SusanooExp += pQuery.Points;
                            Gouyuxuzuotype.Items xuZuoType;
                            if (Gouyuxuzuotype.gouyu_xuzuo_type.TryGetValue((int)client.MyNinja.SusanooLevel + 1, out xuZuoType))
                            {
                                if (client.MyNinja.SusanooExp >= xuZuoType.RequiredExp)
                                    client.MyNinja.SusanooLevel++;
                            }


                            pQuery.Points = client.MyNinja.Exp;
                            pQuery.SusanooLevel = client.MyNinja.SusanooLevel; // [ProtoFix 2026-10-14] source is uint, no cast
                            pQuery.SusanooExp = client.MyNinja.SusanooExp; // [ProtoFix 2026-10-14] source is uint, no cast

                        }

                        client.Send(stream.CreateNinjaUser(pQuery));
                        RellodSkill(client);
                        break;
                    }
                default:
                    {
                        //Console.WriteLine("[MsgGouYuAptitude] Unable Action  " + pQuery.Type);
                        break;
                    }
            }
        }
        public static void RellodSkill(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Gouyuxuzuotype.Items xuZuoType;
                if (Gouyuxuzuotype.gouyu_xuzuo_type.TryGetValue((int)user.MyNinja.SusanooLevel, out xuZuoType))
                {
                    MsgSpell Spell;
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)xuZuoType.Skills[0].Item1))
                    {
                        if (user.MySpells.ClientSpells.TryGetValue((ushort)xuZuoType.Skills[0].Item1, out Spell))
                        {
                            if (Spell.Level != (ushort)xuZuoType.Skills[0].Item2)
                            {
                                user.MySpells.Add(stream, Spell.ID, (ushort)((ushort)xuZuoType.Skills[0].Item2), Spell.SoulLevel, Spell.PreviousLevel, Spell.Experience);
                            }
                        }
                    }
                    else
                    {
                        user.MySpells.Add(stream, (ushort)xuZuoType.Skills[0].Item1);
                    }
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)xuZuoType.Skills[1].Item1))
                    {
                        if (user.MySpells.ClientSpells.TryGetValue((ushort)xuZuoType.Skills[1].Item1, out Spell))
                        {
                            if (Spell.Level != (ushort)xuZuoType.Skills[1].Item2)
                            {
                                user.MySpells.Add(stream, Spell.ID, (ushort)((ushort)xuZuoType.Skills[1].Item2), Spell.SoulLevel, Spell.PreviousLevel, Spell.Experience);
                            }
                        }
                    }
                    else
                    {
                        user.MySpells.Add(stream, (ushort)xuZuoType.Skills[1].Item1);
                    }


                }

            }
        }
    }
}
