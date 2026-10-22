using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class HundredWeapons
    {
        private Client.GameClient Owner;
        public Dictionary<Database.MagicType.WeaponsType, HundredWeaponInfo> Objects;
        public List<ushort> TODO_Spells;
        public bool Unlocked;
        public Dictionary<byte, Time32> TriggerStamp;
        public Dictionary<byte, byte> TriggerSeconds;
        public uint SwordAncestorExp;
        public uint SwordAncestorLevel;
        public byte SwordAncestorUnlocked;
        public HundredWeapons(Client.GameClient _own)
        {
            Owner = _own;
            Objects = new Dictionary<Database.MagicType.WeaponsType, HundredWeaponInfo>();
            TODO_Spells = new List<ushort>();
            TriggerSeconds = new Dictionary<byte, byte>()
            {
                {2, 0},
                {3, 0},
                 {4, 0}
            };
            TriggerStamp = new Dictionary<byte, Time32>()
            {
                {2, new Time32()},
                {3, new Time32()},
                 {4, new Time32()}
            };
        }
        public bool Valid()
        {
            return Database.AtributesStatus.IsTrojan(Owner.Player.Class) && Owner.Player.Class % 100 > 0 && (Owner.Player.Level >= 40 || Owner.Player.Reborn > 0);
        }
        public uint TotalScore
        {
            get
            {
                return Unlocked ? (uint)Objects.Values.Sum(i => i.Score) : 0;
            }
        }
        public uint PerfectionScore
        {
            get
            {
                return Unlocked && Valid() ? ((TotalScore / 50) + (uint)(TryGetItem(1) != null ? (TryGetItem(1).Score - TryGetItem(1).Score * 2 / 100) : 0)) : 0;
            }
        }
        public uint PerfectionScorePrecent
        {
            get
            {
                return Unlocked ? ((TotalScore / 50) + (uint)(TryGetItem(1) != null ? (TryGetItem(1).Score - TryGetItem(1).Score * 2 / 100) : 0)) : 0;
            }
        }
        public bool Free(int Position)
        {
            return Objects.Values.Count(i => i.BitVector.Contain(Position)) == 0;
        }
        public bool FreeAppearance(byte Position)
        {
            return Objects.Values.Count(i => i.AppearancePosition == Position) == 0;
        }
        public bool Equip(Database.MagicType.WeaponsType WeaponSubType, int Position, bool update = true)
        {
            if (Objects.ContainsKey(WeaponSubType))
            {
                if (!Objects[WeaponSubType].BitVector.Contain(Position))
                {
                    Objects[WeaponSubType].BitVector.Add(Position);
                    if (update)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Owner.Send(stream.CreateHundredWeaponsOpt(new MsgHundredWeaponsOpt.MsgHundredWeaponsOptProto() { Type = MsgHundredWeaponsOpt.ActionID.Equip, WeaponSubtype = WeaponSubType, Position = (uint)Position })); // [ProtoFix 2026-10-14] proto Position is uint now
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public bool Show(Database.MagicType.WeaponsType WeaponSubType, byte Position)
        {
            if (Objects.ContainsKey(WeaponSubType))
            {
                if (Objects[WeaponSubType].AppearancePosition != Position && Objects[WeaponSubType].Level >= 3)
                {
                    Objects[WeaponSubType].AppearancePosition = Position;
                }
                return true;
            }
            return false;
        }
        public bool Unequip(int Position, bool update = true)
        {
            foreach (var item in Objects.Values)
            {
                if (item.BitVector.Contain(Position))
                {
                    item.BitVector.Remove(Position);
                    if (update)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Owner.Send(stream.CreateHundredWeaponsOpt(new MsgHundredWeaponsOpt.MsgHundredWeaponsOptProto() { Type = MsgHundredWeaponsOpt.ActionID.Unequip, Position = (uint)Position })); // [ProtoFix 2026-10-14] proto Position is uint now
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool Hide(byte Position)
        {
            foreach (var item in Objects.Values)
            {
                if (item.AppearancePosition == Position)
                {
                    item.AppearancePosition = 0;
                    return true;
                }
            }
            return false;
        }
        public bool Hide(Database.MagicType.WeaponsType subtype)
        {
            foreach (var item in Objects.Values)
            {
                if (item.WeaponSubtype == subtype)
                {
                    item.AppearancePosition = 0;
                    return true;
                }
            }
            return false;
        }
        public bool Swap(int Position1, int Position2, bool update = true)
        {
            if (!Free(Position1) && !Free(Position2))
            {
                var item1 = TryGetItem(Position1);
                var item2 = TryGetItem(Position2);
                item1.BitVector.Remove(Position1);
                item2.BitVector.Remove(Position2);
                item1.BitVector.Add(Position2);
                item2.BitVector.Add(Position1);
                if (update)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.Send(stream.CreateHundredWeaponsOpt(new MsgHundredWeaponsOpt.MsgHundredWeaponsOptProto() { Type = MsgHundredWeaponsOpt.ActionID.Swap, Exchange = new MsgHundredWeaponsOpt.ExchangePositions() { Position1 = (uint)Position1, Position2 = (uint)Position2 } })); // [ProtoFix 2026-10-14] proto Position1/2 are uint now
                    }
                }
                return true;
            }
            return false;
        }
        public bool checkDuplication(Database.MagicType.WeaponsType WeaponSubType, int Position, out int duplicatedPosition)
        {
            switch (Position)
            {
                case 2:
                case 3:
                case 4:
                    {
                        for (int i = 2; i <= 4; i++)
                        {
                            if (TryGetItem(i) != null && TryGetItem(i).WeaponSubtype == WeaponSubType)
                            {
                                duplicatedPosition = i;
                                return true;
                            }
                        }
                        break;
                    }

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        for (int i = 5; i <= 9; i++)
                        {
                            if (TryGetItem(i) != null && TryGetItem(i).WeaponSubtype == WeaponSubType)
                            {
                                duplicatedPosition = i;
                                return true;
                            }
                        }
                        break;
                    }

                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    {
                        for (int i = 10; i <= 16; i++)
                        {
                            if (TryGetItem(i) != null && TryGetItem(i).WeaponSubtype == WeaponSubType)
                            {
                                duplicatedPosition = i;
                                return true;
                            }
                        }
                        break;
                    }

                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    {
                        for (int i = 17; i <= 25; i++)
                        {
                            if (TryGetItem(i) != null && TryGetItem(i).WeaponSubtype == WeaponSubType)
                            {
                                duplicatedPosition = i;
                                return true;
                            }
                        }
                        break;
                    }
            }
            duplicatedPosition = 0;
            return false;
        }
        public bool checkDuplicatedAppearance(Database.MagicType.WeaponsType WeaponSubType, byte Position, out byte duplicatedPosition)
        {
            foreach (var item in Objects.Values)
            {
                if (item.WeaponSubtype == WeaponSubType && item.AppearancePosition > 0)
                {
                    duplicatedPosition = item.AppearancePosition;
                    return true;
                }
            }
            duplicatedPosition = 0;
            return false;
        }
        public HundredWeaponInfo TryGetItem(int Position)
        {
            return Objects.Values.Where(i => i.BitVector.Contain(Position)).FirstOrDefault();
        }
        public void UpdateRank()
        {
            if (Unlocked && Valid())
            {
                foreach (var item in Objects.Values)
                {
                    if (item.Level >= 4)
                    {
                        var entry = new Database.HWRank.Entry()
                        {
                            Type = Database.HWRank.GetIndex(item.WeaponSubtype),
                            TotalPoints = item.Score,
                            UID = Owner.Player.UID,
                            Name = Owner.Player.Name,
                            Level = (byte)Owner.Player.Level,
                            Class = Owner.Player.Class,
                            Mesh = Owner.Player.Mesh
                        };
                        entry.AddInfo(Owner);
                        Database.HWRank.Ranks[Database.HWRank.GetIndex(item.WeaponSubtype)].UpdateItem(entry);

                        var entry2 = entry.ShallowCopy();
                        entry2.TotalPoints = TotalScore;
                        Database.HWRank.Ranks[Database.HWRank.Type.Main].UpdateItem(entry2);
                    }
                }
            }
            else Database.HWRank.Remove(Owner.Player.UID);
        }
        public bool ValidWeapon(Database.MagicType.WeaponsType subType, byte Level)
        {
            return (subType == Database.MagicType.WeaponsType.Blade || subType == Database.MagicType.WeaponsType.Sword || subType == Database.MagicType.WeaponsType.Club || subType == Database.MagicType.WeaponsType.Whip || subType == Database.MagicType.WeaponsType.Dagger || subType == Database.MagicType.WeaponsType.Hook || subType == Database.MagicType.WeaponsType.Axe || subType == Database.MagicType.WeaponsType.Hammer || subType == Database.MagicType.WeaponsType.Scepter) && Level >= 1 && Level <= 9;
        }
        public bool ContainsWeapon(Database.MagicType.WeaponsType SubType)
        {
            return Objects.ContainsKey(SubType);
        }
        public void LoadPowerFoucs(Client.GameClient client)
        {
            if (client.HundredWeapons.SwordAncestorUnlocked == 0)
            {
                var obj = new Qahr.Mobile.Game.MsgServer.MsgSwordAncestor();
                obj.Type = 1;
                obj.Level = 0;
                obj.Exp = 0;
                obj.UID = client.Player.UID;
                obj.Unlocked = false;
                client.Send(obj);
            }
            if (client.HundredWeapons.SwordAncestorUnlocked == 1)
            {
                var obj = new Qahr.Mobile.Game.MsgServer.MsgSwordAncestor();
                obj.Type = 1;
                obj.Level = client.HundredWeapons.SwordAncestorLevel;
                obj.Exp = client.HundredWeapons.SwordAncestorExp;
                obj.UID = client.Player.UID;
                obj.Unlocked = true;
                client.Send(obj);
                MsgSwordAncestor.UpdateSkill(client);
            }
        }
        public bool ValidateSpells()
        {
            bool gainedSpells = false;
            if (Unlocked && Valid())
            {
                foreach (var item in Objects.Values)
                {
                    if (item.DBSpell != null)
                    {
                        if (!Owner.MySpells.ClientSpells.ContainsKey(item.DBSpell.ID))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var streamm = rec.GetStream();
                                Owner.MySpells.Add(streamm, item.DBSpell.ID, item.DBSpell.Level);
                                gainedSpells = true;
                            }
                        }
                    }
                }
            }
            var mySpells = Owner.MySpells.ClientSpells.Values.ToArray();
            foreach (var spell in mySpells)
            {
                Dictionary<ushort, Database.MagicType.Magic> Spells;
                if (Pool.Magic.TryGetValue(spell.ID, out Spells))
                {
                    if (Spells.Count > spell.Level)
                    {
                        try
                        {
                            Database.MagicType.Magic Magic = Pool.Magic[spell.ID][spell.Level];
                            if (Magic != null && Magic.isTrojanArchiveSkill)
                            {
                                var weapon = Database.HundredWeapons.HundredWeaponsList.Values.FirstOrDefault(i => i.MagicType == spell.ID);
                                if (weapon != null)
                                {
                                    if (!Unlocked || !Valid() || !ContainsWeapon(weapon.WeaponSubtype))
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var streamm = rec.GetStream();
                                            Owner.MySpells.Remove(spell.ID, streamm);
                                        }
                                    };
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Spellid {0} Spelllevel {1}", spell.ID, spell.Level);
                        }
                    }
                }
            }
            return gainedSpells;
        }
       
        public bool AddWeapon(HundredWeaponInfo item, bool Update = false)
        {
            if (ValidWeapon(item.WeaponSubtype, item.Level))
            {
                if (!ContainsWeapon(item.WeaponSubtype))
                    Objects.Add(item.WeaponSubtype, item);
                else Objects[item.WeaponSubtype] = item;

                ValidateSpells();

                if (Update)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.Send(stream.CreateHundredWeaponsInfo(Owner, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                    }
                }
                UpdateRank();
                return true;
            }
            UpdateRank();
            return false;
        }
        public bool AddWeapon(Database.MagicType.WeaponsType subType, byte Level, bool Update = false)
        {
            HundredWeaponInfo item = new HundredWeaponInfo()
            {
                WeaponSubtype = subType,
                Level = Level,
                BitVector = new BitVector32(1 * 32),
                Attributes = new Dictionary<Database.HundredWeapons.AttributeType, int>()
                { 
                                 { Database.HundredWeapons.AttributeType.Hitpoints, 0 },
                                 { Database.HundredWeapons.AttributeType.PhysicalAttack, 0 },
                                 { Database.HundredWeapons.AttributeType.PhysicalDefense, 0 },
                                 { Database.HundredWeapons.AttributeType.MagicAttack, 0 },
                                 { Database.HundredWeapons.AttributeType.MagicDefense, 0 },
                }
            };
            return AddWeapon(item, Update);

        }
        public bool RemoveWeapon(Database.MagicType.WeaponsType subType, bool Update = false)
        {
            if (ContainsWeapon(subType))
            {
                if (Objects[subType].DBSpell != null)
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey(Objects[subType].DBSpell.ID))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var streamm = rec.GetStream();
                            Owner.MySpells.Add(streamm, Objects[subType].DBSpell.ID, Objects[subType].DBSpell.Level);
                        }
                    }
                }

                Objects.Remove(subType);

                if (Update)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.Send(stream.CreateHundredWeaponsInfo(Owner, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                    }
                }
                Database.HWRank.Ranks[Database.HWRank.GetIndex(subType)].Remove(Owner.Player.UID);
                Database.HWRank.Ranks[Database.HWRank.Type.Main].Remove(Owner.Player.UID);
            }
            return true;
        }
        public bool StageActivated(byte Stage)
        {
            switch (Stage)
            {
                case 1:
                    return TryGetItem(1) != null;
                case 2:
                    return Objects.Values.Count(i => i.BitVector.Contain(2) || i.BitVector.Contain(3) || i.BitVector.Contain(4)) >= 3;
                case 3:
                    return Objects.Values.Count(i => i.BitVector.Contain(5) || i.BitVector.Contain(6) || i.BitVector.Contain(7) || i.BitVector.Contain(8) || i.BitVector.Contain(9)) >= 5;
            }
            return false;
        }
        public ushort GetNextSpell()
        {
            if (!Owner.Player.ContainFlag(MsgUpdate.Flags.ActiveWeapon))
                TODO_Spells.Clear();
            ushort spellID = 0;
            if (TODO_Spells.Count > 0)
            {
                spellID = TODO_Spells.FirstOrDefault();
                TODO_Spells.RemoveAt(0);
            }
            return spellID;
        }
        public bool CanTrigger(byte Stage)
        {
            return Time32.Now >= TriggerStamp[Stage].AddSeconds(TriggerSeconds[Stage]);
        }
        public bool Trigger(byte Stage)
        {
            if (CanTrigger(Stage))
            {
                TriggerSeconds[Stage] = GetWeaponRate(Stage).FirstOrDefault().Value;
                TriggerStamp[Stage] = Time32.Now;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Owner.Send(stream.CreateHundredWeaponsOpt(new MsgHundredWeaponsOpt.MsgHundredWeaponsOptProto() { Type = MsgHundredWeaponsOpt.ActionID.Trigger, TriggeredStage = Stage, TriggeringDuration = TriggerSeconds[Stage] }));
                }
            }
            return false;
        }
        public Dictionary<double, byte> GetWeaponRate(byte stage)
        {
            switch (stage)
            {
                case 1:
                    {
                        if (TotalScore <= 1500)
                            return new Dictionary<double, byte>() { { 50.00, 15 } };
                        else if (TotalScore >= 1501 && TotalScore <= 2000)
                            return new Dictionary<double, byte>() { { 55.00, 15 } };
                        else if (TotalScore >= 2001 && TotalScore <= 2500)
                            return new Dictionary<double, byte>() { { 55.00, 15 } };
                        else if (TotalScore >= 2501 && TotalScore <= 3000)
                            return new Dictionary<double, byte>() { { 60.00, 15 } };
                        else if (TotalScore >= 3001 && TotalScore <= 5000)
                            return new Dictionary<double, byte>() { { 70.00, 15 } };
                        else if (TotalScore >= 5001 && TotalScore <= 10000)
                            return new Dictionary<double, byte>() { { 80.00, 15 } };
                        else if (TotalScore >= 10001 && TotalScore <= 15000)
                            return new Dictionary<double, byte>() { { 90.00, 15 } };
                        else if (TotalScore >= 15001 && TotalScore <= 30000)
                            return new Dictionary<double, byte>() { { 100.00, 10 } };
                        else if (TotalScore >= 30001 && TotalScore <= 40000)
                            return new Dictionary<double, byte>() { { 100.00, 8 } };
                        else if (TotalScore >= 40001 && TotalScore <= 60000)
                            return new Dictionary<double, byte>() { { 100.00, 6 } };
                        else if (TotalScore >= 60001 && TotalScore <= 80000)
                            return new Dictionary<double, byte>() { { 100.00, 4 } };
                        else if (TotalScore >= 80001 && TotalScore <= 100000)
                            return new Dictionary<double, byte>() { { 100.00, 2 } };
                        else if (TotalScore >= 100001 && TotalScore <= 130000)
                            return new Dictionary<double, byte>() { { 100.00, 1 } };
                        else if (TotalScore >= 130001)
                            return new Dictionary<double, byte>() { { 100.00, 0 } };
                        break;
                    }
                case 2:
                    {
                        if (TotalScore <= 1500)
                            return new Dictionary<double, byte>() { { 1.00, 50 } };
                        else if (TotalScore >= 1501 && TotalScore <= 2000)
                            return new Dictionary<double, byte>() { { 1.50, 50 } };
                        else if (TotalScore >= 2001 && TotalScore <= 2500)
                            return new Dictionary<double, byte>() { { 1.50, 50 } };
                        else if (TotalScore >= 2501 && TotalScore <= 3000)
                            return new Dictionary<double, byte>() { { 2.00, 45 } };
                        else if (TotalScore >= 3001 && TotalScore <= 5000)
                            return new Dictionary<double, byte>() { { 3.00, 40 } };
                        else if (TotalScore >= 5001 && TotalScore <= 10000)
                            return new Dictionary<double, byte>() { { 4.00, 35 } };
                        else if (TotalScore >= 10001 && TotalScore <= 15000)
                            return new Dictionary<double, byte>() { { 5.00, 30 } };
                        else if (TotalScore >= 15001 && TotalScore <= 30000)
                            return new Dictionary<double, byte>() { { 10.00, 25 } };
                        else if (TotalScore >= 30001 && TotalScore <= 40000)
                            return new Dictionary<double, byte>() { { 12.00, 20 } };
                        else if (TotalScore >= 40001 && TotalScore <= 60000)
                            return new Dictionary<double, byte>() { { 15.00, 20 } };
                        else if (TotalScore >= 60001 && TotalScore <= 80000)
                            return new Dictionary<double, byte>() { { 20.00, 20 } };
                        else if (TotalScore >= 80001 && TotalScore <= 100000)
                            return new Dictionary<double, byte>() { { 25.00, 20 } };
                        else if (TotalScore >= 100001 && TotalScore <= 130000)
                            return new Dictionary<double, byte>() { { 28.00, 20 } };
                        else if (TotalScore >= 130001)
                            return new Dictionary<double, byte>() { { 30.00, 20 } };
                        break;
                    }
                case 3:
                    {
                        if (TotalScore <= 1500)
                            return new Dictionary<double, byte>() { { 0.20, 90 } };
                        else if (TotalScore >= 1501 && TotalScore <= 2000)
                            return new Dictionary<double, byte>() { { 0.20, 90 } };
                        else if (TotalScore >= 2001 && TotalScore <= 2500)
                            return new Dictionary<double, byte>() { { 0.20, 85 } };
                        else if (TotalScore >= 2501 && TotalScore <= 3000)
                            return new Dictionary<double, byte>() { { 0.40, 80 } };
                        else if (TotalScore >= 3001 && TotalScore <= 5000)
                            return new Dictionary<double, byte>() { { 0.60, 70 } };
                        else if (TotalScore >= 5001 && TotalScore <= 10000)
                            return new Dictionary<double, byte>() { { 0.80, 60 } };
                        else if (TotalScore >= 10001 && TotalScore <= 15000)
                            return new Dictionary<double, byte>() { { 1.00, 55 } };
                        else if (TotalScore >= 15001 && TotalScore <= 30000)
                            return new Dictionary<double, byte>() { { 1.20, 50 } };
                        else if (TotalScore >= 30001 && TotalScore <= 40000)
                            return new Dictionary<double, byte>() { { 1.40, 45 } };
                        else if (TotalScore >= 40001 && TotalScore <= 60000)
                            return new Dictionary<double, byte>() { { 1.60, 40 } };
                        else if (TotalScore >= 60001 && TotalScore <= 80000)
                            return new Dictionary<double, byte>() { { 1.80, 35 } };
                        else if (TotalScore >= 80001 && TotalScore <= 100000)
                            return new Dictionary<double, byte>() { { 2.00, 30 } };
                        else if (TotalScore >= 100001 && TotalScore <= 130000)
                            return new Dictionary<double, byte>() { { 2.20, 30 } };
                        else if (TotalScore >= 130001)
                            return new Dictionary<double, byte>() { { 2.50, 30 } };
                        break;
                    }
            }
            return null;
        }

        public bool ClearWeapons(bool Update = false)
        {
            foreach (var obj in Objects.Values)
                RemoveWeapon(obj.WeaponSubtype);
            if (Update)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Owner.Send(stream.CreateHundredWeaponsInfo(Owner, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                }
            }
            Database.HWRank.Remove(Owner.Player.UID);
            return true;
        }
        public void Load(string line)
        {
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            Unlocked = reader.Read((byte)0) == 1;
            if (Unlocked)
            {
                int count = reader.Read((int)0);
                for (int i = 0; i < count; i++)
                {
                    var item = new HundredWeaponInfo() { WeaponSubtype = (Database.MagicType.WeaponsType)reader.Read((ushort)0), Level = reader.Read((byte)0), Progress = reader.Read((uint)0), BitVector = new BitVector32(1 * 32) };
                    item.BitVector.bits[0] = reader.Read((uint)0);
                    item.AppearancePosition = reader.Read((byte)0);
                    item.Attributes = new Dictionary<Database.HundredWeapons.AttributeType, int>();
                    int attributesCount = reader.Read((int)0);
                    for (int x = 0; x < attributesCount; x++)
                    {
                        var key = (Database.HundredWeapons.AttributeType)reader.Read((byte)0);
                        var val = reader.Read((int)0);
                        if (!item.Attributes.ContainsKey(key))
                            item.Attributes.Add(key, val);
                    }
                    AddWeapon(item);

                }
                SwordAncestorExp = reader.Read((uint)0);
                SwordAncestorLevel = reader.Read((uint)0);
                SwordAncestorUnlocked = reader.Read((byte)0);
            }
            ValidateSpells();
           
        }
        public override string ToString()
        {
            var file = new Database.DBActions.WriteLine('/').Add(Unlocked ? (byte)1 : (byte)0);
            if (Unlocked)
            {
                file.Add((int)Objects.Count);
                foreach (var item in Objects.Values)
                {
                    file.Add((ushort)item.WeaponSubtype);
                    file.Add((byte)item.Level);
                    file.Add((uint)item.Progress);
                    file.Add((uint)item.BitVector.bits[0]);
                    file.Add((byte)item.AppearancePosition);
                    file.Add((int)item.Attributes.Count);
                    foreach (var att in item.Attributes)
                    {
                        file.Add((byte)att.Key);
                        file.Add((int)att.Value);
                    }
                }
                file.Add(SwordAncestorExp);
                file.Add(SwordAncestorLevel);
                file.Add(SwordAncestorUnlocked);
            }
            return file.Close();
        }
    }
}
