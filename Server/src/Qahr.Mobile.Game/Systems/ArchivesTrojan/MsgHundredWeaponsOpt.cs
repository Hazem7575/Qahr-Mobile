using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgHundredWeaponsOpt
    {
        [ProtoContract]
        public class MsgHundredWeaponsOptProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2, IsRequired = true)]
            public Database.MagicType.WeaponsType WeaponSubtype;
            [ProtoMember(3, IsRequired = true)]
            public uint Position; // [ProtoFix 2026-10-14] client CMsgHundredWeaponOptPB #3 is uint32 (was int)
            [ProtoMember(4, IsRequired = true)]
            public ulong Combination;
            [ProtoMember(5)]
            public byte TriggeredStage;
            [ProtoMember(6)]
            public uint TriggeringDuration; // [ProtoFix 2026-10-14] client #6 is uint32 (was int)
            [ProtoMember(7)]
            public ExchangePositions Exchange;
        }
        [ProtoContract]
        public class ExchangePositions
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Position1; // [ProtoFix 2026-10-14] client CWeaponSwapPB #1 is uint32 (was int)
            [ProtoMember(2, IsRequired = true)]
            public uint Position2; // [ProtoFix 2026-10-14] client CWeaponSwapPB #2 is uint32 (was int)
        }

        public enum ActionID : int
        {
            Equip,
            Unequip,
            Swap,
            Combine,
            Trigger
        }
        public static unsafe ServerSockets.Packet CreateHundredWeaponsOpt(this ServerSockets.Packet stream, MsgHundredWeaponsOptProto proto)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(GamePackets.MsgHundredWeaponsOpt);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgHundredWeaponsOpt)]
        public static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgHundredWeaponsOptProto msg = new MsgHundredWeaponsOptProto();
            msg = stream.ProtoBufferDeserialize<MsgHundredWeaponsOptProto>(msg);
            if (!client.HundredWeapons.Unlocked || !client.HundredWeapons.Valid()) return;
            switch (msg.Type)
            {
                case ActionID.Equip:
                    {
                        if (!client.HundredWeapons.ContainsWeapon(msg.WeaponSubtype)) break;
                        if (client.HundredWeapons.Free((int)msg.Position) || client.HundredWeapons.Unequip((int)msg.Position)) // [ProtoFix 2026-10-14] same int value as before (unchecked)
                        {
                            int duplicatedPosition;
                            if (!client.HundredWeapons.checkDuplication(msg.WeaponSubtype, (int)msg.Position, out duplicatedPosition) || client.HundredWeapons.Unequip(duplicatedPosition)) // [ProtoFix 2026-10-14] Position is uint now
                            {
                                if (client.HundredWeapons.Equip(msg.WeaponSubtype, (int)msg.Position)) // [ProtoFix 2026-10-14] Position is uint now
                                {
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Unequip:
                    {
                        if (!client.HundredWeapons.Free((int)msg.Position) && client.HundredWeapons.Unequip((int)msg.Position)) // [ProtoFix 2026-10-14] Position is uint now
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case ActionID.Swap:
                    {
                        if (client.HundredWeapons.Swap((int)msg.Exchange.Position1, (int)msg.Exchange.Position2)) // [ProtoFix 2026-10-14] Position1/2 are uint now
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case ActionID.Combine:
                    {
                        byte combinationSet = Convert.ToByte(msg.Combination.ToString("X2").FirstOrDefault().ToString());
                        byte combinationReqSpirits = Convert.ToByte(msg.Combination.ToString("X2").LastOrDefault().ToString());
                        Dictionary<int, Database.MagicType.WeaponsType> Weapons = null;
                        switch (combinationSet)
                        {
                            case 1:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {2, Database.MagicType.WeaponsType.Blade},
                                        {3, Database.MagicType.WeaponsType.Hammer},
                                        {4, Database.MagicType.WeaponsType.Club}
                                    };
                                    break;
                                }
                            case 2:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {2, Database.MagicType.WeaponsType.Sword},
                                        {3, Database.MagicType.WeaponsType.Hook},
                                        {4, Database.MagicType.WeaponsType.Axe}
                                    };
                                    break;
                                }
                            case 3:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {2, Database.MagicType.WeaponsType.Hook},
                                        {3, Database.MagicType.WeaponsType.Whip},
                                        {4, Database.MagicType.WeaponsType.Scepter}
                                    };
                                    break;
                                }
                            case 4:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {5, Database.MagicType.WeaponsType.Dagger},
                                        {6, Database.MagicType.WeaponsType.Sword},
                                        {7, Database.MagicType.WeaponsType.Blade},
                                        {8, Database.MagicType.WeaponsType.Hammer},
                                        {9, Database.MagicType.WeaponsType.Whip},
                                    };
                                    break;
                                }
                            case 5:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {5, Database.MagicType.WeaponsType.Dagger},
                                        {6, Database.MagicType.WeaponsType.Sword},
                                        {7, Database.MagicType.WeaponsType.Blade},
                                        {8, Database.MagicType.WeaponsType.Hammer},
                                        {9, Database.MagicType.WeaponsType.Club},
                                    };
                                    break;
                                }
                            case 6:
                                {
                                    Weapons = new Dictionary<int, Database.MagicType.WeaponsType>()
                                    {
                                        {5, Database.MagicType.WeaponsType.Sword},
                                        {6, Database.MagicType.WeaponsType.Dagger},
                                        {7, Database.MagicType.WeaponsType.Club},
                                        {8, Database.MagicType.WeaponsType.Scepter},
                                        {9, Database.MagicType.WeaponsType.Axe},
                                    };
                                    break;
                                }
                        }
                        if (Weapons != null)
                        {
                            foreach (var wep in Weapons)
                                if (client.HundredWeapons.TryGetItem(wep.Key) != null && client.HundredWeapons.TryGetItem(wep.Key).WeaponSubtype != wep.Value)
                                    client.HundredWeapons.Unequip(wep.Key);
                            foreach (var wep in Weapons)
                                client.HundredWeapons.Equip(wep.Value, wep.Key);
                        }
                        break;
                    }
            }
        }
    }
}
