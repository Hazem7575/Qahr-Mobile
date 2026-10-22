using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgServer.AttackHandler;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class Archives
    {
        [Flags]
        public enum TwistedFututr : uint
        {
            None = 0,
            Disorder = 1,
            ApePistol = 2,
            BearsCare = 3,
        }
        [Flags]
        public enum TypeID : uint
        {
            None = 0,
            Dragonhowl = 201,
            Bloodlust = 202,
            Redcurse = 203,
            Conception = 301,
            Gobernador = 302,
            Belt = 303,
            StoneCracker = 401,
            ColdMoon = 402,
            ThornCutter = 403,
            HeavenlyTiger = 601,
            MightyDragon = 602,
            CosmicRoc = 603,
            ThunderNut = 701,
            FrozenNut = 702,
            LavaNut = 703,
            Dragon = 801,
            Kunpeng = 802,
            Suanni = 803,
            Vicissitude = 1301,
            HighestGood = 1302,
            Evolution = 1303,
            Thrill = 1304,
            BirthDeath = 1305
        }

        public enum EffectFlags : uint
        {
            None = 0,
            FirstEffect = 1,
            SecondEffect = 2,
            ThirdEffect = 3,

        }

        public enum AirTypeID : uint
        {
            Sense = 701,
            Armed = 702,
            Overlord = 703,
        }

        public NutAtributes Collection;

        public class NutAtributes
        {
            public uint MaxHP;
            public uint PAttack;
            public uint FinalPAttack;
            public uint FinalPDamage;
        }

        public ConcurrentDictionary<TypeID, Item> Items;

        public ConcurrentDictionary<AirTypeID, items> AirPowers;

        public ConcurrentDictionary<uint, JadeList> JadeBag;

        public ConcurrentDictionary<string, double[]> Info;

        public Client.GameClient user;

        public Archives(Client.GameClient _user)
        {
            user = _user;
            Items = new ConcurrentDictionary<TypeID, Item>();
            AirPowers = new ConcurrentDictionary<AirTypeID, items>();
            JadeBag = new ConcurrentDictionary<uint, JadeList>();
            Info = new ConcurrentDictionary<string, double[]>();
        }

        public void SetValue(string type, params double[] data)
        {
            if (!Info.ContainsKey(type))
                Info.TryAdd(type, data);
            else
                Info[type] = data;
        }

        public double[] GetValue(string type)
        {
            if (Info.ContainsKey(type))
                return Info[type];
            return new double[] { };
        }

        public class JadeList
        {
            public uint ItemID;

            public daoqi_dict_type.Item DBJadeList
            {

                get
                {
                    var items = daoqi_dict_type.Items.Values.FirstOrDefault(p => p.ID == this.ItemID);
                    if (items != null)
                        return items;
                    return new daoqi_dict_type.Item();
                }
            }

        }

        public class items
        {
            public AirTypeID Type;
            public uint Level;
            public uint Exp;
        }

        public class Item
        {
            public uint UserID;

            public TypeID ItemID;

            public uint Level;

            public uint Progress;

            public byte Hash;

            public uint dwParam;

            public uint MasteryPoints;

            public uint Debris;

            public Jade[] Jades;

            public Anima[] Animas;

            public Item()
            {
                Animas = new Item.Anima[2];
                Animas[0] = new Item.Anima();
                Animas[1] = new Item.Anima();

                Jades = new Jade[6];
                for (int i = 0; i < 6; i++)
                    Jades[i] = new Jade();
            }

            public class Anima
            {
                public uint[] AnimaID;
                public Anima()
                {
                    AnimaID = new uint[7];
                }
            }

            public class Jade
            {
                public uint JadeID;
            }

            public CombatGear.Items DBItem
            {
                get
                {
                    CombatGear.Items Type;
                    CombatGear.Items Item;
                    if (!CombatGear.TryGetValue((uint)this.ItemID, this.Level, out Type))
                    {
                        Item = new CombatGear.Items();
                    }
                    else
                    {
                        Item = Type;
                    }
                    return Item;
                }
            }

        }


        public void Check_Heaven_Taoist()
        {
            using (var rec = new RecycledPacket())
            {
                if (Database.AtributesStatus.IsTaoist(user.Player.Class))
                {
                    var stream = rec.GetStream();
                    if (PassedHeavenTaoistConditions())
                    {
                        if (Database.AtributesStatus.IsFire(user.Player.Class))
                            user.MySpells.UpdateSpellLevelOrAdd(stream, (ushort)Role.Flags.SpellID.StarChainFire, 1);
                        else if (Database.AtributesStatus.IsWater(user.Player.Class))
                            user.MySpells.UpdateSpellLevelOrAdd(stream, (ushort)Role.Flags.SpellID.StarChainWater, 1);

                        if (user.Player.TaoHeavenState == 1)
                            SetHeavenVector(stream, 1 << 5);
                    }
                }
            }
        }

        public void SetHeavenVector(Packet stream, uint value)
        {
            if (Database.AtributesStatus.IsTaoist(user.Player.Class))
            {
                MsgCombatGearTao.ProtoStructure dao = new MsgCombatGearTao.ProtoStructure();
                dao.Action = MsgCombatGearTao.Action.ActiveNew;
                dao.Postion = user.Player.UID;
                dao.unk4 = 1;
                dao.Skip = value;
                user.HeavenVector = value;

                user.Player.View.SendView(MsgCombatGearTao.CreateTao(stream, dao), true);
                user.Player.View.SendView(user.Player.GetArray(stream, false), true);
            }
        }

        public bool PassedHeavenTaoistConditions()
        {
            if (Database.AtributesStatus.IsTaoist(user.Player.Class))
            {
                uint totallevel = 0;
                foreach (var item in Items.Values.Where(x => x.ItemID >= TypeID.Vicissitude && x.ItemID <= TypeID.BirthDeath))
                {
                    totallevel += item.Level;
                }
                return totallevel >= 60 * 5;
            }
            return false;
        }

        public bool HasWeapon(Archives.TypeID weapon)
        {
            return Items.Values.Where(x => x.ItemID == weapon).FirstOrDefault() != null;
        }

        public Item GetWeapon(Archives.TypeID weapon)
        {
            return Items.Values.Where(x => x.ItemID == weapon).FirstOrDefault();
        }

        public bool TryGetValueEquip(ushort SpellID)
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();

                foreach (var Info in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.ThunderNut && p.ItemID <= Role.Instance.Archives.TypeID.LavaNut).ToArray())
                {
                    if (Items != null)
                    {
                        var DBItem = daoqi_dict_type.Items.Values.Where(p => p.ID == Info.Jades[0].JadeID || p.ID == Info.Jades[1].JadeID || p.ID == Info.Jades[2].JadeID || p.ID == Info.Jades[3].JadeID || p.ID == Info.Jades[4].JadeID || p.ID == Info.Jades[5].JadeID).ToArray();
                        var MagicIds = DBItem.FirstOrDefault(p => p.attr_type1 == 24 && p.attr_value1 / 1000 == SpellID || p.attr_type2 == 24 && p.attr_value2 / 1000 == SpellID);
                        if (MagicIds != null)
                        {
                            return true;

                        }

                    }
                }
                if (user.MySpells.ClientSpells.ContainsKey(SpellID))
                    user.MySpells.ClientSpells.Remove(SpellID);
            }
            return false;
        }
        public void AddItem(TypeID ID, uint Level = 1, uint Exp = 0, byte Equip = 0, uint Transform = 0)
        {

            #region InspiredWarrior


            if (ID == TypeID.Dragonhowl || ID == TypeID.Bloodlust || ID == TypeID.Redcurse)
            {
                if (Level > 50)
                {
                    Level = 50;
                }

                if (!Items.ContainsKey(ID))
                {

                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {

                            UserID = user.Player.UID,

                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }

                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                        LoadingCombatHeart();
                    }

                }
            }
            #endregion
            #region InspiredArcher


            if (ID == TypeID.StoneCracker || ID == TypeID.ThornCutter || ID == TypeID.ColdMoon)
            {
                if (Level > 60)
                {
                    Level = 60;
                }

                if (!Items.ContainsKey(ID))
                {
                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }
                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                    }
                }
            }
            #endregion

            #region InspiredMonk


            if (ID == TypeID.HeavenlyTiger || ID == TypeID.MightyDragon || ID == TypeID.CosmicRoc)
            {
                if (Level > 60)
                {
                    Level = 60;
                }

                if (!Items.ContainsKey(ID))
                {
                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }
                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                    }
                }
            }
            #endregion

            #region InspiredTaoist



            if (ID == TypeID.Vicissitude || ID == TypeID.HighestGood || ID == TypeID.Evolution || ID == TypeID.Thrill || ID == TypeID.BirthDeath)
            {
                if (Level > 60)
                {
                    Level = 60;
                }
                if (!Items.ContainsKey(ID))
                {
                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }
                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                    }
                }
            }
            #endregion

            #region InspiredPirate

            if (ID == TypeID.ThunderNut || ID == TypeID.FrozenNut || ID == TypeID.LavaNut)
            {
                if (!Items.ContainsKey(ID))
                {

                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }
                        if (ID == TypeID.ThunderNut)
                        {
                            var Power = new items();
                            Power.Type = Role.Instance.Archives.AirTypeID.Sense;
                            Power.Level = 1;
                            Power.Exp = 0;
                            user.MyArchives.AirPowers.Add(Role.Instance.Archives.AirTypeID.Sense, Power);
                        }
                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                        UpdateNut();
                    }
                }

            }
            #endregion

            #region Dune

            if (ID == TypeID.Conception || ID == TypeID.Gobernador || ID == TypeID.Belt)
            {
                if (!Items.ContainsKey(ID))
                {
                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }

                        item.Hash = Equip;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                    }
                }

            }
            #endregion
            #region InspiredLeeLong



            if (ID == TypeID.Dragon || ID == TypeID.Kunpeng || ID == TypeID.Suanni)
            {
                if (Level > 60)
                {
                    Level = 60;
                }
                if (!Items.ContainsKey(ID))
                {
                    CombatGear.Items DBitem;
                    if (CombatGear.TryGetValue((uint)ID, Level > 1 ? Level - 1 : Level, out DBitem))
                    {
                        Item item = new Item
                        {
                            UserID = user.Player.UID,
                            ItemID = ID,
                            Level = Level
                        };
                        if (Level != 1)
                        {
                            item.Progress = DBitem.Exp;
                        }
                        item.Hash = 0;
                        item.dwParam = Transform;

                        Items.Add(ID, item);
                        Open(item.ItemID);
                        MsgCombatGear.ProtoStructure.Create(user, item, MsgCombatGear.ProtoStructure.Action.Add);
                        UpdateRank();
                    }
                }
            }
            #endregion
        }
        public void Open(TypeID ID)
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();

                #region InspiredWarrior
                List<ushort> Skills = new List<ushort> { (ushort)17460, (ushort)17470, (ushort)17480, (ushort)17490, (ushort)17640, (ushort)17650, (ushort)17620, (ushort)17500, (ushort)17510, (ushort)17520, (ushort)17530, (ushort)17600, (ushort)17610, (ushort)17540, (ushort)17550, (ushort)17560, (ushort)17570, (ushort)17630, (ushort)17660, (ushort)17580 };

                if (AtributesStatus.IsWarrior(user.Player.Class) && Items.Count > 0)
                {
                    if (Items.ContainsKey(ID))
                    {
                        foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Redcurse))
                        {
                            if (obj.ItemID != ID)
                            {
                                foreach (var Skill in obj.DBItem.Skills)
                                {
                                    if (Skill != 0)
                                    {
                                        user.MySpells.Remove((ushort)(Skill / 1000), stream);
                                    }
                                }

                                obj.Hash = 0;
                            }
                            else
                            {
                                foreach (var Skill in obj.DBItem.Skills)
                                {
                                    if (Skill != 0)
                                    {
                                        user.MySpells.Add(stream, (ushort)(Skill / 1000), (byte)(Skill % 10));
                                    }
                                }
                                obj.Hash = 1;
                            }

                        }
                        #region AddSkill(WarriorMele)
                        if (isOpen(TypeID.Dragonhowl))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK1))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit201NormalATK1, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK2))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit201NormalATK2, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK3))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit201NormalATK3, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackDragonhowl))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.TripleAttackDragonhowl, 0, 0, 0, 0,  false);
                            }
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DragonPierce))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.DragonPierce, 0, 0, 0, 0, false);
                            }

                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK1))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK1, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK2))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK2, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK3))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK3, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackDragonhowl))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackDragonhowl, stream);
                            }
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DragonPierce))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.DragonPierce, stream);
                            }
                        }
                        if (isOpen(TypeID.Bloodlust))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK1))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit202NormalATK1, 0, 0, 0, 0, false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK2))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit202NormalATK2, 0, 0, 0, 0, false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK3))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit202NormalATK3, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackBloodlust))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.TripleAttackBloodlust, 0, 0, 0, 0,  false);
                            }
                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK1))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK1, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK2))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK2, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK3))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK3, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackBloodlust))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackBloodlust, stream);
                            }
                        }
                        if (isOpen(TypeID.Redcurse))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK1))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit203NormalATK1, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK2))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit203NormalATK2, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK3))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.WarSuit203NormalATK3, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackRedcurse))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.TripleAttackRedcurse, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.PowerDash))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.PowerDash, 0, 0, 0, 0,  false);
                            }
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ArmorofImmunity))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.ArmorofImmunity, 0, 0, 0, 0, false);
                            }
                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK1))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK1, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK2))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK2, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK3))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK3, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackRedcurse))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackRedcurse, stream);
                            }
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.PowerDash))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.PowerDash, stream);
                            }
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ArmorofImmunity))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.ArmorofImmunity, stream);
                            }
                        }
                        #endregion
                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                        UpdateRank();
                        if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DefensiveStance))
                        {
                            user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.DefensiveStance);
                        }
                        if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Stigma) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Superman) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.SuperCyclone) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                        {
                            user.Player.ClearFlags();
                        }

                    }
                }
                else
                {
                    foreach (ushort r in Skills)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey(r))
                        {
                            user.MySpells.Remove(r, stream);
                        }

                    }
                }
                #endregion

                #region InspiredArcher
                List<ushort> ushortList2 = new List<ushort>() { (ushort)18220, (ushort)18980, (ushort)10950, (ushort)10890, (ushort)10930, (ushort)18690, (ushort)18670, (ushort)18650, (ushort)18180, (ushort)18200, (ushort)18630, (ushort)18710, (ushort)18730 };
                if (AtributesStatus.IsArcher(user.Player.Class) && this.Items.Count > 0)
                {
                    if (!this.Items.ContainsKey(ID))
                        return;
                    foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.StoneCracker && p.ItemID <= Archives.TypeID.ThornCutter))
                    {
                        if (obj.ItemID != ID)
                        {
                            foreach (uint skill in obj.DBItem.Skills)
                            {
                                if (skill != 0)
                                    user.MySpells.Remove((ushort)(skill / 1000), stream);
                            }
                            obj.Hash = 0;
                        }
                        else
                        {
                            foreach (uint skill in obj.DBItem.Skills)
                            {
                                if (skill != 0)
                                    user.MySpells.Add(stream, (ushort)(skill / 1000), (ushort)(byte)(skill % 100));
                            }
                            obj.Hash = 1;
                        }

                    }
                    if (this.isOpen(Archives.TypeID.StoneCracker))
                    {


                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarSuit401NormalATK1);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarSuit401NormalATK2);

                        if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades))
                            user.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades, StatusFlagsBigVector32.PermanentFlag, false);
                        user.Player.ArrowBladesPower = 2;
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            {
                                user.Player.UpdateArrowBlades(streamm, 0);
                            }
                        }
                    }
                    else
                    {
                        user.Player.ArrowBladesPower = 0;
                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                        {
                            var streamm = recycledPacket.GetStream();
                            {
                                user.Player.UpdateArrowBlades(streamm, 0);
                            }
                        }
                        user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.AttackArrowBlades);
                        user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK1, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK2, stream);
                    }
                    if (this.isOpen(Archives.TypeID.ColdMoon))
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK1))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarSuit402NormalATK2);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK2))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarSuit402NormalATK2);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK3))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarSuit402NormalATK3);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hunter))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Hunter);
                    }
                    else
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK1))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit402NormalATK1, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK2))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit402NormalATK2, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK3))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit402NormalATK3, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hunter))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.Hunter, stream);
                    }
                    if (this.isOpen(Archives.TypeID.ThornCutter))
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK1))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalATK1);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK2))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalATK2);

                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK3))
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalATK3);

                    }
                    else
                    {

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK1))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.NormalATK1, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK2))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.NormalATK2, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK3))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.NormalATK3, stream);
                    }
                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                    this.UpdateRank();
                    if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fly) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Stigma) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Superman) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.SuperCyclone) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                    {
                        user.Player.ClearFlags();
                    }
                }
                else
                {
                    foreach (ushort num in ushortList2)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey(num))
                            user.MySpells.Remove(num, stream);

                    }
                }
                #endregion


                #region GoldenMonk
                #region AddSkill(WarriorMele)
                if (AtributesStatus.IsMonk(user.Player.Class) && Items.Count > 0)
                {
                    if (Items.ContainsKey(ID))
                    {
                        foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.HeavenlyTiger && p.ItemID <= Archives.TypeID.CosmicRoc))
                        {
                            if (obj.ItemID != ID)
                            {
                                foreach (var Skill in obj.DBItem.Skills)
                                {
                                    if (Skill != 0)
                                    {
                                        user.MySpells.Remove((ushort)(Skill / 1000), stream);
                                    }
                                }

                                obj.Hash = 0;
                            }
                            else
                            {
                                foreach (var Skill in obj.DBItem.Skills)
                                {
                                    if (Skill != 0)
                                    {
                                        user.MySpells.Add(stream, (ushort)(Skill / 1000), (byte)(Skill % 10));
                                    }
                                }
                                obj.Hash = 1;
                            }

                        }
                        if (isOpen(TypeID.MightyDragon))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VajraRing))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.VajraRing, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ClawStrike))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.ClawStrike, 0, 0, 0, 0, false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ZenStaff))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.ZenStaff, 0, 0, 0, 0, false);
                            }


                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VajraRing))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.VajraRing, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ClawStrike))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.ClawStrike, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ZenStaff))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.ZenStaff, stream);
                            }

                        }
                        if (isOpen(TypeID.CosmicRoc))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ClapBomb))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.ClapBomb, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VajraPalm))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.VajraPalm, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FlowerTouch))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.FlowerTouch, 0, 0, 0, 0,  false);
                            }


                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ClapBomb))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.ClapBomb, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VajraPalm))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.VajraPalm, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FlowerTouch))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.FlowerTouch, stream);
                            }

                        }
                        if (isOpen(TypeID.HeavenlyTiger))
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.BellShield))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.BellShield, 0, 0, 0, 0,  false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VioletBowl))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.VioletBowl, 0, 0, 0, 0, false);
                            }

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.QuellingRobe))
                            {
                                user.MySpells.Add(stream, (ushort)Flags.SpellID.QuellingRobe, 0, 0, 0, 0, false);
                            }


                        }
                        else
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.BellShield))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.BellShield, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VioletBowl))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.VioletBowl, stream);
                            }

                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.QuellingRobe))
                            {
                                user.MySpells.Remove((ushort)Flags.SpellID.QuellingRobe, stream);
                            }

                        }
                    }
                }
                #endregion
                #endregion

                #region InspiredTaoist
                List<ushort> ushortTaoist = new List<ushort>() { (ushort)20000, (ushort)20200, (ushort)20210, (ushort)20220, (ushort)20230, (ushort)20240, (ushort)20250, (ushort)20260, (ushort)20270, (ushort)20280, (ushort)20290, (ushort)20300, (ushort)20310, (ushort)20320, (ushort)20330, (ushort)20340, (ushort)20350, (ushort)20360, (ushort)20370, (ushort)20380, (ushort)20390, (ushort)20820, (ushort)21190, (ushort)20720, (ushort)20750, (ushort)20730, (ushort)20780, (ushort)20760, (ushort)20800, (ushort)20770, (ushort)20740, (ushort)21330, (ushort)21360, (ushort)21420 };
                if (AtributesStatus.IsTaoist(user.Player.Class) && this.Items.Count > 0)
                {
                    if (Items.ContainsKey(ID))
                    {
                        foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Vicissitude && p.ItemID <= Archives.TypeID.BirthDeath))
                        {
                            if (obj.ItemID != ID)
                            {
                                foreach (uint skill in obj.DBItem.Skills)
                                {
                                    if (skill != 0)
                                        user.MySpells.Remove((ushort)(skill / 1000), stream);
                                }
                                obj.Hash = 0;
                            }
                            else
                            {
                                foreach (uint skill in obj.DBItem.Skills)
                                {
                                    if (skill != 0)
                                        user.MySpells.Add(stream, (ushort)(skill / 1000), (ushort)(byte)(skill % 100));
                                }
                                obj.Hash = 1;
                            }


                        }
                    }
                    #region Remove
                    if (this.isOpen(Archives.TypeID.Vicissitude))
                    {
                        if (AtributesStatus.IsFire(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DivineEmptiness))
                                user.MySpells.Remove((ushort)Flags.SpellID.DivineEmptiness, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FloraWard))
                                user.MySpells.Remove((ushort)Flags.SpellID.FloraWard, stream);

                        }
                        else if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.Substitution))
                                user.MySpells.Remove((ushort)Flags.SpellID.Substitution, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DeadwoodCurse))
                                user.MySpells.Remove((ushort)Flags.SpellID.DeadwoodCurse, stream);
                        }


                    }

                    if (this.isOpen(Archives.TypeID.HighestGood))
                    {
                        if (AtributesStatus.IsFire(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WeepStorm))
                                user.MySpells.Remove((ushort)Flags.SpellID.WeepStorm, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WaterAegis))
                                user.MySpells.Remove((ushort)Flags.SpellID.WaterAegis, stream);

                        }
                        else if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.HolyProtection))
                                user.MySpells.Remove((ushort)Flags.SpellID.HolyProtection, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FlowKnack))
                                user.MySpells.Remove((ushort)Flags.SpellID.FlowKnack, stream);
                        }

                    }

                    if (this.isOpen(Archives.TypeID.Evolution))
                    {
                        if (AtributesStatus.IsFire(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WackeSpirit))
                                user.MySpells.Remove((ushort)Flags.SpellID.WackeSpirit, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.SolidBulwark))
                                user.MySpells.Remove((ushort)Flags.SpellID.SolidBulwark, stream);

                        }
                        else if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.CrackMantra))
                                user.MySpells.Remove((ushort)Flags.SpellID.CrackMantra, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.NobleSpirit))
                                user.MySpells.Remove((ushort)Flags.SpellID.NobleSpirit, stream);
                        }

                    }

                    if (this.isOpen(Archives.TypeID.Thrill))
                    {
                        if (AtributesStatus.IsFire(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.MysticalMelody))
                                user.MySpells.Remove((ushort)Flags.SpellID.MysticalMelody, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FantasyKnack))
                                user.MySpells.Remove((ushort)Flags.SpellID.FantasyKnack, stream);

                        }
                        else if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DivineAttraction))
                                user.MySpells.Remove((ushort)Flags.SpellID.DivineAttraction, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.MagneticLight))
                                user.MySpells.Remove((ushort)Flags.SpellID.MagneticLight, stream);
                        }

                    }

                    if (this.isOpen(Archives.TypeID.BirthDeath))
                    {
                        if (AtributesStatus.IsFire(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FlameShield))
                                user.MySpells.Remove((ushort)Flags.SpellID.FlameShield, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.PhoenixBlessing))
                                user.MySpells.Remove((ushort)Flags.SpellID.PhoenixBlessing, stream);
                        }
                        else if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WildPhoenix))
                                user.MySpells.Remove((ushort)Flags.SpellID.WildPhoenix, stream);
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.AblazeBlade))
                                user.MySpells.Remove((ushort)Flags.SpellID.AblazeBlade, stream);
                        }

                    }
                    #endregion
                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                    this.UpdateRank();

                    if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Stigma) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Superman) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.SuperCyclone) && !user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                    {
                        user.Player.ClearFlags();
                    }
                }
                else
                {
                    foreach (ushort id in ushortTaoist)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey(id))
                            user.MySpells.Remove(id, stream);

                    }
                }
                #endregion

                #region InspiredPirate
                if (AtributesStatus.IsPirate(user.Player.Class) && this.Items.Count > 0)
                {
                    if (Items.ContainsKey(ID))
                    {
                        foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.ThunderNut && p.ItemID <= Archives.TypeID.LavaNut))
                        {
                            if (obj.ItemID != ID)
                            {
                                obj.Hash = 0;
                            }
                            else
                            {
                                foreach (uint skill in obj.DBItem.Skills)
                                {
                                    if (skill > 0)
                                        user.MySpells.Add(stream, (ushort)(skill / 1000), (ushort)(byte)(skill % 10));
                                }
                                obj.Hash = 1;
                            }
                        }
                        this.UpdateRank();
                    }
                }
                #endregion

                #region InspiredLee
                if (AtributesStatus.IsLee(user.Player.Class) && this.Items.Count > 0)
                {
                    if (!this.Items.ContainsKey(ID))
                        return;
                    foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragon && p.ItemID <= Archives.TypeID.Suanni))
                    {

                        foreach (uint skill in obj.DBItem.Skills)
                        {
                            if (skill > 0)
                                user.MySpells.Add(stream, (ushort)(skill / 1000), (ushort)(byte)(skill % 100));
                        }

                    }
                }
                #endregion

                #region Dune
                if (AtributesStatus.IsDune(user.Player.Class) && this.Items.Count > 0)
                {
                    if (!this.Items.ContainsKey(ID))
                        return;
                    foreach (var obj in user.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Conception && p.ItemID <= Archives.TypeID.Belt))
                    {
                        if (obj.ItemID != ID)
                        {
                            foreach (uint skill in obj.DBItem.Skills)
                            {
                                if (skill != 0)
                                    user.MySpells.Remove((ushort)(skill / 1000), stream);
                            }
                            obj.Hash = 0;
                        }
                        else
                        {
                            foreach (uint skill in obj.DBItem.Skills)
                            {
                                if (skill != 0)
                                    user.MySpells.Add(stream, (ushort)(skill / 1000), (ushort)(byte)(skill % 100));
                            }
                            obj.Hash = 1;
                        }

                    }
                }
                #endregion

            }

        }

        public void RemoveSkill()
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                if (Items.Values.Count > 0)
                {
                    foreach (var obj in user.MyArchives.Items.Values)
                    {
                        obj.Hash = 0;

                        #region Warrior
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK1))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK1, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK2))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK2, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK3))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK3, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackDragonhowl))
                            user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackDragonhowl, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DragonPierce))
                            user.MySpells.Remove((ushort)Flags.SpellID.DragonPierce, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK1))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK1, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK2))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK2, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK3))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK3, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackBloodlust))
                            user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackBloodlust, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK1))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK1, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK2))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK2, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK3))
                            user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK3, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackRedcurse))
                            user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackRedcurse, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.PowerDash))
                            user.MySpells.Remove((ushort)Flags.SpellID.PowerDash, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ArmorofImmunity))
                            user.MySpells.Remove((ushort)Flags.SpellID.ArmorofImmunity, stream);
                        #endregion

                        #region NormalATK1_17400
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK1, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeAttack))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.RevengeAttack, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revenge))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.Revenge, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackShot))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.CrackShot, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowBlades))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.ArrowBlades, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK1, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK2, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK2, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hunter))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.Hunter, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK3))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.NormalATK3, stream);
                        #endregion

                        #region Tao
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FloraWard))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.FloraWard, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineEmptiness))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineEmptiness, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Substitution))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.Substitution, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SubstitutionAttack))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.SubstitutionAttack, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DeadwoodCurse))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.DeadwoodCurse, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WeepStorm))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WeepStorm, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegis))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WaterAegis, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HolyProtection))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.HolyProtection, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlowKnack))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.FlowKnack, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WackeSpirit))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WackeSpirit, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidBulwark))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.SolidBulwark, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NobleSpirit))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.NobleSpirit, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackMantra))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.CrackMantra, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MysticalMelody))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.MysticalMelody, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FantasyKnack))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.FantasyKnack, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAttraction))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineAttraction, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagneticLight))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.MagneticLight, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PhoenixBlessing))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.PhoenixBlessing, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlameShield))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.FlameShield, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WildPhoenix, stream);
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AblazeBlade))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.AblazeBlade, stream);
                        #endregion

                        #region Pirate
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Thunderlord))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Thunderlord, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Sense))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Sense, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Overlord))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Overlord, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanction))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanction, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanction))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanction, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Revelator))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Revelator, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ColdBloodline))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ColdBloodline, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.IceAge))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.IceAge, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.TwospiendSpear))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.TwospiendSpear, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.PheasantBeak))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.PheasantBeak, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Spitfire))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Spitfire, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.SpitfirePassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.SpitfirePassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaSea))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaSea, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.StarVolCano))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.StarVolCano, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Diabolize))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Diabolize, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.CaptiveArrow))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.CaptiveArrow, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.GiantGun))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.GiantGun, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanctionPassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanctionPassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.GiantGunPassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.GiantGunPassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Armed))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Armed, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Barrier))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Barrier, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Shell))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Shell, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Storm))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Storm, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Thrash))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Thrash, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderPirate))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderPirate, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Torrent))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Torrent, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Tide))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Tide, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Splash))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Splash, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Sailing))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Sailing, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Vast))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Vast, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderlordAttack))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderlordAttack, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Dark))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Dark, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaNut))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaNut, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderNut))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderNut, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.FrozenNut))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.FrozenNut, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaSeaPassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaSeaPassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.IceAgePassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.IceAgePassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive, stream);

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.DrukylePassive))
                            user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.DrukylePassive, stream);
                        #endregion
                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                        UpdateRank();
                    }
                }
                else
                {

                    #region Warrior
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK1))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK1, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK2))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK2, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK3))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit201NormalATK3, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackDragonhowl))
                        user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackDragonhowl, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DragonPierce))
                        user.MySpells.Remove((ushort)Flags.SpellID.DragonPierce, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK1))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK1, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK2))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK2, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit202NormalATK3))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit202NormalATK3, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackBloodlust))
                        user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackBloodlust, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK1))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK1, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK2))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK2, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit203NormalATK3))
                        user.MySpells.Remove((ushort)Flags.SpellID.WarSuit203NormalATK3, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackRedcurse))
                        user.MySpells.Remove((ushort)Flags.SpellID.TripleAttackRedcurse, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.PowerDash))
                        user.MySpells.Remove((ushort)Flags.SpellID.PowerDash, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ArmorofImmunity))
                        user.MySpells.Remove((ushort)Flags.SpellID.ArmorofImmunity, stream);
                    #endregion

                    #region NormalATK1_17400
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK1, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeAttack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.RevengeAttack, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeAttack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.RevengeAttack, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackShot))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.CrackShot, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowBlades))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.ArrowBlades, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK1, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK2, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WarSuit401NormalATK2, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hunter))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.Hunter, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK3))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.NormalATK3, stream);
                    #endregion

                    #region Tao
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FloraWard))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.FloraWard, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineEmptiness))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineEmptiness, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Substitution))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.Substitution, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SubstitutionAttack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.SubstitutionAttack, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DeadwoodCurse))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.DeadwoodCurse, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WeepStorm))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WeepStorm, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegis))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WaterAegis, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HolyProtection))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.HolyProtection, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlowKnack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.FlowKnack, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WackeSpirit))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WackeSpirit, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidBulwark))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.SolidBulwark, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NobleSpirit))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.NobleSpirit, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackMantra))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.CrackMantra, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MysticalMelody))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.MysticalMelody, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FantasyKnack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.FantasyKnack, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAttraction))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineAttraction, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagneticLight))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.MagneticLight, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PhoenixBlessing))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.PhoenixBlessing, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlameShield))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.FlameShield, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WildPhoenix))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.WildPhoenix, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AblazeBlade))
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.AblazeBlade, stream);
                    #endregion

                    #region Pirate
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Thunderlord))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Thunderlord, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Sense))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Sense, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Overlord))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Overlord, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanction))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanction, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanction))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanction, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Revelator))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Revelator, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ColdBloodline))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ColdBloodline, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.IceAge))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.IceAge, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.TwospiendSpear))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.TwospiendSpear, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.PheasantBeak))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.PheasantBeak, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Spitfire))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Spitfire, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.SpitfirePassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.SpitfirePassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaSea))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaSea, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.StarVolCano))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.StarVolCano, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Diabolize))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Diabolize, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.CaptiveArrow))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.CaptiveArrow, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.GiantGun))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.GiantGun, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.HolySanctionPassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.HolySanctionPassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.TwospiendSpearpassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.GiantGunPassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.GiantGunPassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Armed))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Armed, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Barrier))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Barrier, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Shell))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Shell, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Storm))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Storm, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Thrash))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Thrash, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderPirate))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderPirate, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Torrent))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Torrent, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Tide))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Tide, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Splash))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Splash, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Sailing))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Sailing, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Vast))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Vast, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderlordAttack))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderlordAttack, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Dark))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Dark, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaNut))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaNut, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.ThunderNut))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.ThunderNut, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.FrozenNut))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.FrozenNut, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LavaSeaPassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LavaSeaPassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.IceAgePassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.IceAgePassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive, stream);

                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.DrukylePassive))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.DrukylePassive, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.SandMist))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.SandMist, stream);
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Fusing))
                        user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Fusing, stream);

                    #endregion
                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                    UpdateRank();
                }

            }
        }

        public bool isOpen(TypeID ID)
        {
            if (AtributesStatus.IsWarrior(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID && p.Hash == 1).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsArcher(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID && p.Hash == 1).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsTaoist(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID && p.Hash == 1).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsPirate(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID && p.Hash == 1).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsDune(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID && p.Hash == 1).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsLee(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            if (AtributesStatus.IsMonk(user.Player.Class) && Items.Count > 0)
            {
                var Item = Items.Values.Where(p => p.ItemID == ID).FirstOrDefault();
                if (Item != null)
                    return true;
            }
            return false;
        }

        public Item isOpen()
        {
            Item Item = null;
            if (AtributesStatus.IsWarrior(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Redcurse).FirstOrDefault();
            else if (AtributesStatus.IsArcher(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.StoneCracker && p.ItemID <= Archives.TypeID.ThornCutter).FirstOrDefault();
            else if (AtributesStatus.IsMonk(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.HeavenlyTiger && p.ItemID <= Archives.TypeID.CosmicRoc).FirstOrDefault();
            else if (AtributesStatus.IsTaoist(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.Vicissitude && p.ItemID <= Archives.TypeID.BirthDeath).FirstOrDefault();
            else if (AtributesStatus.IsPirate(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.ThunderNut && p.ItemID <= Archives.TypeID.LavaNut).FirstOrDefault();
            else if (AtributesStatus.IsDune(user.Player.Class))
                Item = Items.Values.Where(p => p.Hash == 1 && p.ItemID >= Archives.TypeID.Conception && p.ItemID <= Archives.TypeID.Belt).FirstOrDefault();
            else if (AtributesStatus.IsLee(user.Player.Class))
                Item = Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragon && p.ItemID <= Archives.TypeID.Suanni).FirstOrDefault();
            return Item;
        }

        public ushort GetSkill(IMapObj Target)
        {
            try
            {
                if (AtributesStatus.IsWarrior(user.Player.Class))
                {
                    #region Dragonhowl
                    if (isOpen(Archives.TypeID.Dragonhowl))
                    {
                        List<ushort> CanUse = new List<ushort>();
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK1))
                        {
                            CanUse.Add((ushort)Flags.SpellID.WarSuit201NormalATK1);
                        }

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK2))
                        {
                            CanUse.Add((ushort)Flags.SpellID.WarSuit201NormalATK2);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.WarSuit201NormalATK3))
                        {
                            CanUse.Add((ushort)Flags.SpellID.WarSuit201NormalATK3);
                        }
                        MsgSpell user_spell = null;
                        Role.IMapObj targets;
                        if (user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.SobNpc)
                              || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Monster)
                            || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Player))
                        {
                            short Range = Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targets.X, targets.Y);
                            if (Range <= 2)
                            {
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.TripleAttackDragonhowl, out user_spell))
                                {
                                    MagicType.Magic TripleAttackDragonhowl = Pool.Magic[user_spell.ID][user_spell.Level];
                                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackDragonhowl))
                                    {
                                        if (Role.Core.Rate(TripleAttackDragonhowl.Rate))
                                        {
                                            return (ushort)Flags.SpellID.TripleAttackDragonhowl;
                                        }
                                    }

                                }
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.IronbonePassive, out user_spell))
                                {
                                    MagicType.Magic IronbonePassive = Pool.Magic[user_spell.ID][user_spell.Level];
                                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.IronbonePassive))
                                    {
                                        if (Role.Core.Rate(IronbonePassive.Rate))
                                        {
                                            return (ushort)Flags.SpellID.IronbonePassive;
                                        }
                                    }

                                }

                            }
                        }
                        return CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                    }
                    #endregion
                    #region Bloodlust
                    if (isOpen(Archives.TypeID.Bloodlust))
                    {
                        List<ushort> CanUse = new List<ushort>();
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit202NormalATK1))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit202NormalATK1);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit202NormalATK2))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit202NormalATK2);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit202NormalATK3))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit202NormalATK3);
                        }
                        Role.IMapObj targets;
                        if (user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.SobNpc)
                                    || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Monster)
                            || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Player))
                        {
                            short Range = Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targets.X, targets.Y);
                            if (Range <= 2)
                            {
                                MsgSpell user_spell = null;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.TripleAttackBloodlust, out user_spell))
                                {
                                    MagicType.Magic TripleAttackBloodlust = Pool.Magic[user_spell.ID][user_spell.Level];
                                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackBloodlust))
                                    {
                                        if (Role.Core.Rate(TripleAttackBloodlust.Rate))
                                        {
                                            return (ushort)Role.Flags.SpellID.TripleAttackBloodlust;
                                        }
                                    }
                                }
                            }
                        }
                        return (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                    }
                    #endregion
                    #region Redcurse
                    if (isOpen(Archives.TypeID.Redcurse))
                    {

                        List<ushort> CanUse = new List<ushort>();
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit203NormalATK1))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit203NormalATK1);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit203NormalATK2))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit203NormalATK2);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit203NormalATK3))
                        {
                            CanUse.Add((ushort)Role.Flags.SpellID.WarSuit203NormalATK3);
                        }
                        Role.IMapObj targets;
                        if (user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.SobNpc)
                                   || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Monster)
                            || user.Player.View.TryGetValue(Target.UID, out targets, Role.MapObjectType.Player))
                        {
                            short Range = Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targets.X, targets.Y);
                            if (Range <= 2)
                            {
                                MsgSpell user_spell = null;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.TripleAttackRedcurse, out user_spell))
                                {
                                    MagicType.Magic TripleAttackRedcurse = Pool.Magic[user_spell.ID][user_spell.Level];
                                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.TripleAttackRedcurse))
                                    {
                                        if (Role.Core.Rate(TripleAttackRedcurse.Rate))
                                        {
                                            return (ushort)Role.Flags.SpellID.TripleAttackRedcurse;
                                        }
                                    }
                                }
                            }
                        }
                        return (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                    }
                    #endregion

                }
                if (AtributesStatus.IsArcher(user.Player.Class))
                {
                    #region StoneCracker
                    if (this.isOpen(Archives.TypeID.StoneCracker))
                    {
                        List<ushort> ushortList = new List<ushort>();

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK1))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit401NormalATK1);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit401NormalATK2);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowBlades))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.ArrowBlades);
                        }
                        return ushortList[Program.GetRandom.Next(0, ushortList.Count)];
                    }
                    #endregion
                    #region ColdMoon
                    if (this.isOpen(Archives.TypeID.ColdMoon))
                    {
                        List<ushort> ushortList = new List<ushort>();
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK1))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit402NormalATK1);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK2))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit402NormalATK2);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit402NormalATK3))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit402NormalATK3);
                        }
                        return ushortList[Program.GetRandom.Next(0, ushortList.Count)];
                    }
                    #endregion
                    #region ThornCutter
                    if (this.isOpen(Archives.TypeID.ThornCutter))
                    {
                        if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(Target.X, Target.Y, user.Player.X, user.Player.Y) <= 1
                            || user.Player.ContainFlag(MsgUpdate.Flags.FireArrow)
                                   || user.Player.ContainFlag(MsgUpdate.Flags.IceArrow)
                                   || user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow)
                                   || user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow)
                                   || user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                        {
                            MsgSpell user_spell = null;
                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ElementalArrow, out user_spell))
                            {
                                if (Core.Rate(Pool.Magic[user_spell.ID][user_spell.Level].Rate))
                                {
                                    if (user.MySpells.ClientSpells.ContainsKey((ushort)(ushort)Role.Flags.SpellID.ElementalArrow))
                                    {
                                        return (ushort)Role.Flags.SpellID.ElementalArrow;
                                    }
                                }
                            }
                        }
                        List<ushort> ushortList = new List<ushort>();
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalATK3))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.NormalATK3);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarSuit401NormalATK2))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.WarSuit401NormalATK2);
                        }
                        return ushortList[Program.GetRandom.Next(0, ushortList.Count)];
                    }
                    #endregion
                }

            }
            catch
            {

            }
            return 0;
        }
        public ushort GetSkillLee(IMapObj Target)
        {
            try
            {
                MsgSpell user_spell = null;
                List<ushort> ushortList = new List<ushort>();
                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DragonRising, out user_spell))
                {
                    MagicType.Magic LongYueYuyuan = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.DragonRising))
                    {
                        if (Role.Core.Rate(LongYueYuyuan.Rate))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.DragonRising);
                        }
                    }
                }
                if (!user.Player.ContainFlag(MsgUpdate.Flags.Spreadyourwings))
                {
                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Hitthewaterthreethousand, out user_spell))
                    {
                        MagicType.Magic Hitthewaterthreethousand = Pool.Magic[user_spell.ID][user_spell.Level];
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.Hitthewaterthreethousand))
                        {
                            if (Role.Core.Rate(Hitthewaterthreethousand.Rate))
                            {
                                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                                {
                                    ServerSockets.Packet stream = rec.GetStream();
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Spreadyourwings, (int)Hitthewaterthreethousand.Duration, true);
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.Spreadyourwings, Hitthewaterthreethousand.Duration, 0, (uint)Hitthewaterthreethousand.DamageOnMonster, MsgUpdate.DataType.ArchiveSkill, true);
                                }
                            }
                        }
                    }
                }

                return ushortList[Program.GetRandom.Next(0, ushortList.Count)];
            }
            catch
            {

            }
            return 0;
        }

        public void Loading()
        {
            if (Items.Count == 0)
            {
                RemoveSkill();
                return;
            }
            if (Items != null)
            {
                #region LoadArchive(Info)

                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    ServerSockets.Packet stream = rec.GetStream();
                    LoadingEffectLeeLong();

                    MsgCombatGear.ProtoStructure obj = new MsgCombatGear.ProtoStructure
                    {
                        Type = (byte)MsgCombatGear.ProtoStructure.Action.Load,
                        UID = user.Player.UID,
                        Items = new MsgCombatGear.ProtoStructure.ConstructsProto[Items.Count],
                        Member4 = 0
                    };
                    int idx = 0;
                    foreach (var Item in Items.Values)
                    {
                        obj.Items[idx] = new MsgCombatGear.ProtoStructure.ConstructsProto
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
                            Debris = Item.Debris
                        };

                        if (obj.Items[idx].ID == 401 && obj.Items[idx].Hash == 1 && AtributesStatus.IsArcher(user.Player.Class))
                        {
                            if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.StoneCracker))
                                user.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.StoneCracker, StatusFlagsBigVector32.PermanentFlag, false);
                            user.Player.ArrowBladesPower = 2;
                            user.Player.UpdateArrowBlades(stream, 0);
                        }
                        idx++;
                    }
                    user.Send(stream.CreateArchives(obj));
                }
                #endregion

                AddSpellWaterAegisRebirth();

                LoadingAirPower();

                UpdateNut();

                UpdateRank();

                LoadingCombatHeart();

                Check_Heaven_Taoist();
                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    ServerSockets.Packet stream = rec.GetStream();
                    user.Player.View.Clear(stream);
                    user.Player.View.Role();
                    user.Player.View.SendView(this.user.Player.GetArray(stream, false), false);
                }
            }
        }

        public void AddSpellWaterAegisRebirth()
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                if (!Database.AtributesStatus.IsWarrior(user.Player.Class) && (Database.AtributesStatus.IsWarrior(user.Player.FirstClass) || Database.AtributesStatus.IsWarrior(user.Player.SecoundeClass)))
                {

                    var WarAegis = user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragonhowl && p.ItemID <= Role.Instance.Archives.TypeID.Redcurse).ToArray();
                    if (WarAegis != null)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarAegis))
                        {
                            ushort Levels = 0;
                            foreach (var Level in user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragonhowl && p.ItemID <= Role.Instance.Archives.TypeID.Redcurse))
                            {
                                Levels += (ushort)Level.Level;
                            }
                            if (Levels > 150)
                                Levels = 150;
                            if (Levels <= 100)
                            {
                                Levels /= 10;
                            }
                            if (Levels > 100)
                            {
                                Levels -= 100;
                                Levels /= 5;
                                Levels += 10;
                            }
                            if (Levels > 19)
                                Levels = 19;
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarAegis))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WarAegis, Levels);
                        }
                    }
                    else
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarAegis))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WarAegis, stream);
                    }

                }

                if (!Database.AtributesStatus.IsArcher(user.Player.Class) && (Database.AtributesStatus.IsArcher(user.Player.FirstClass) || Database.AtributesStatus.IsArcher(user.Player.SecoundeClass)))
                {
                    var Tardiness = user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && p.ItemID <= Role.Instance.Archives.TypeID.ThornCutter).ToArray();
                    if (Tardiness != null)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tardiness))
                        {
                            ushort Levels = 0;
                            foreach (var Level in user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && p.ItemID <= Role.Instance.Archives.TypeID.ThornCutter))
                            {
                                Levels += (ushort)Level.Level;
                            }
                            if (Levels > 150)
                                Levels = 150;
                            if (Levels <= 100)
                            {
                                Levels /= 10;
                            }
                            if (Levels > 100)
                            {
                                Levels -= 100;
                                Levels /= 5;
                                Levels += 10;
                            }
                            if (Levels > 19)
                                Levels = 19;
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tardiness))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tardiness, Levels);
                        }
                    }
                    else
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tardiness))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.Tardiness, stream);
                    }

                }
                if (!Database.AtributesStatus.IsTaoist(user.Player.Class) && (Database.AtributesStatus.IsTaoist(user.Player.FirstClass) || Database.AtributesStatus.IsTaoist(user.Player.SecoundeClass)))
                {

                    var WaterAegis = user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Vicissitude && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath).ToArray();
                    if (WaterAegis != null)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegisRebirth))
                        {
                            ushort Levels = 0;
                            foreach (var Level in user.MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Vicissitude && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath))
                            {
                                Levels += (ushort)Level.Level;
                            }
                            if (Levels > 150)
                                Levels = 150;
                            if (Levels <= 100)
                            {
                                Levels /= 10;
                            }
                            if (Levels > 100)
                            {
                                Levels -= 100;
                                Levels /= 5;
                                Levels += 10;
                            }
                            if (Levels > 19)
                                Levels = 19;
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegisRebirth))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WaterAegisRebirth, Levels);
                        }
                    }
                    else
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterAegisRebirth))
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.WaterAegisRebirth, stream);
                    }

                }

            }

        }

        public void LoadingAirPower()
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                if (AtributesStatus.IsPirate(user.Player.Class))
                {
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Thunderlord, 0);
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.ThunderlordAttack, 0);
                    user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Revelator);
                    user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Fusing);
                    var Action = new MsgPirateOpt.ProtoStructure();
                    Action.Type = (uint)MsgPirateOpt.Type.Login;
                    Action.UID = user.Player.UID;
                    Action.ArchiveType = 0;
                    Action.ItemList = new MsgPirateOpt.ProtoStructure.Items
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
                    user.Send(stream.CreatePirateOpt(Action));

                    UpdatePoints(AirPower);
                    //MsgPirateOpt.UpdateAirPoints(user);
                    MsgPirateOpt.AddSkillAir(user);
                }
            }
        }

        public void LoadingEffectLeeLong()
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                if (AtributesStatus.IsLee(user.Player.Class))
                {
                    if (Effect != EffectFlags.None)
                    {
                        var Info = new MsgCombatGearTao.ProtoStructure();
                        Info.Action = MsgCombatGearTao.Action.LoginEffect;
                        Info.SameEffect = 3;
                        Info.BeastFlags = user.MyArchives.Effect;
                        user.Send(stream.CreateTao(Info));
                    }
                }
            }
        }
        public void LoadingCombatHeart()
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                if (AtributesStatus.IsWarrior(user.Player.Class))
                {
                    var TotalLev = Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Redcurse).Sum(p => p.Level);
                    if (TotalLev >= 90)
                    {
                        user.Send(stream.CreateCombatHeart(new MsgCombatHeart.MsgCombatHeartPB()
                        {
                            Action = MsgCombatHeart.MsgCombatHeartPB.ActionID.Active,
                            Level = user.MyArchives.LevelCombatHeart,
                            Progress = user.MyArchives.LevelCombatProgress,
                            UID = user.Player.UID,
                        }));
                        if (user.MyArchives.LevelCombatHeart > 0)
                        {
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
                            {
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SupremeLeadership, (ushort)user.MyArchives.LevelCombatHeart);
                            }
                            else
                            {
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SupremeLeadership, (ushort)user.MyArchives.LevelCombatHeart);
                            }
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAnnihilation))
                            {
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DivineAnnihilation, (ushort)user.MyArchives.LevelCombatHeart);
                            }
                            else
                            {
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DivineAnnihilation, (ushort)user.MyArchives.LevelCombatHeart);
                            }
                        }
                    }
                    else
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.SupremeLeadership, stream);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAnnihilation))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineAnnihilation, stream);
                        }

                    }
                }
                else
                {
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
                    {
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.SupremeLeadership, stream);
                    }
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAnnihilation))
                    {
                        user.MySpells.Remove((ushort)Role.Flags.SpellID.DivineAnnihilation, stream);
                    }

                }

            }
        }

        public void Close()
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                ServerSockets.Packet stream = recycledPacket.GetStream();
                user.Player.ArrowBladesPower = 0;
                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades))
                    user.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades);
                if (user.Player.ContainFlag(MsgUpdate.Flags.ActiveArrowBlades))
                    user.Player.RemoveFlag(MsgUpdate.Flags.ActiveArrowBlades);

                user.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveArrowBlades, 0, 0, 0, MsgUpdate.DataType.AppendIcon);

                user.Send(stream.CreateArchviesInfo(new MsgCombatGearOpt.ProtoStructure()
                {
                    Type = (uint)MsgCombatGearOpt.ProtoStructure.TypeID.Close
                }));
            }
        }

        #region Score Archive
        public uint TotalScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    score += obj.Progress;
                    score += obj.dwParam * 50000;
                }
                score += (uint)LevelCombatProgress;
                return score;
            }
        }

        public uint DragonhowlScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.Dragonhowl)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint BloodlustScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.Bloodlust)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint RedcurseScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.Redcurse)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint ArcherWeekTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }

        public uint ArcherTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath).ToArray())
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }



        public uint HeavenlyTigerScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.HeavenlyTiger)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint MightyDragonScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.MightyDragon)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint CosmicRocScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == Archives.TypeID.CosmicRoc)
                        score += obj.Progress;
                }
                return score;
            }
        }

        public uint MonkWeekTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }

        public uint MonkTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.HeavenlyTiger && p.ItemID <= Role.Instance.Archives.TypeID.CosmicRoc).ToArray())
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }



        public uint StoneCrackerScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.StoneCracker)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint ColdMoonScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.ColdMoon)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint ThornCutterScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.ThornCutter)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint TaoWeeklyTotalScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values)
                {
                    score += obj.Progress;
                    score += obj.dwParam * 50000;
                }
                return score;
            }
        }

        public uint TaoTotalScore
        {
            get
            {
                uint score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Vicissitude && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath).ToArray())
                {
                    score += obj.Progress + obj.MasteryPoints;
                    score += obj.dwParam * 50000;
                }
                return score;
            }
        }

        public uint VicissitudeScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Vicissitude)
                        Score += obj.Progress + obj.MasteryPoints;
                }
                return Score;
            }
        }

        public uint HighestGoodScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.HighestGood)
                        Score += obj.Progress + obj.MasteryPoints;
                }
                return Score;
            }
        }

        public uint EvolutionScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Evolution)
                        Score += obj.Progress + obj.MasteryPoints;
                }
                return Score;
            }
        }

        public uint ThrillScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Thrill)
                        Score += obj.Progress + obj.MasteryPoints;
                }
                return Score;
            }
        }

        public uint BirthdeathScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.BirthDeath)
                        Score += obj.Progress + obj.MasteryPoints;
                }
                return Score;
            }
        }

        public uint OverallRankings
        {
            get
            {
                var Powers = AirPowers.Values.ToArray();
                uint Score = 0;
                for (int q = 0; q < AirPowers.Count; q++)
                {
                    Score += Powers[q].Exp;

                }
                foreach (var Item in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.ThunderNut && p.ItemID <= Role.Instance.Archives.TypeID.LavaNut).ToArray())
                {
                    Score += Item.MasteryPoints;
                }
                return Score;
            }
        }
        public uint WeeklyPirateRankings
        {
            get
            {
                var Powers = AirPowers.Values.ToArray();
                uint Score = 0;
                for (int q = 0; q < AirPowers.Count; q++)
                {
                    Score += Powers[q].Exp;
                }
                foreach (var Item in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.ThunderNut && p.ItemID <= Role.Instance.Archives.TypeID.LavaNut).ToArray())
                {
                    Score += Item.MasteryPoints;
                }
                return Score;
            }
        }

        public uint LeeLongWeekTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragon && p.ItemID <= Role.Instance.Archives.TypeID.Suanni).ToArray())
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }

        public uint LeeLongTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragon && p.ItemID <= Role.Instance.Archives.TypeID.Suanni).ToArray())
                {
                    Score += obj.Progress;
                    Score += obj.dwParam * 50000;
                }
                return Score;
            }
        }

        public uint DragonScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Dragon)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint KunpengScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Kunpeng)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint SuanniScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Suanni)
                        Score += obj.Progress;
                }
                return Score;
            }
        }


        public uint DuneTotalScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Conception && p.ItemID <= Role.Instance.Archives.TypeID.Belt).ToArray())
                {
                    Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint ConceptionScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Conception)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint GobernadorScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Gobernador)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        public uint BeltScore
        {
            get
            {
                uint Score = 0;
                foreach (Item obj in Items.Values)
                {
                    if (obj.ItemID == TypeID.Belt)
                        Score += obj.Progress;
                }
                return Score;
            }
        }

        #endregion

        public void AddReiki(uint _Points)
        {
            using (var rec = new RecycledPacket())
            {
                var stream = rec.GetStream();
                Reiki += _Points;
                user.Send(stream.CreateTao(new MsgCombatGearTao.ProtoStructure() { Action = MsgCombatGearTao.Action.AddReiki, ReikiPts = Reiki }));
            }
        }
        public void AddDunePoints(uint _Points)
        {
            using (var rec = new RecycledPacket())
            {
                var stream = rec.GetStream();
                user.Player.DunePoints += _Points;

                user.Send(stream.CreateArchives(new MsgCombatGear.ProtoStructure()
                {
                    Type = 5,
                    // [ProtoFix 2026-10-14] client #5 is a single CombatMartialArts
                    MartialInfos = new MsgCombatGear.ProtoStructure.MartialInfo()
                    {
                        Member1 = 3,
                        Member2 = user.Player.DunePoints
                    }
                }));
            }
        }
        public bool AddRune(uint ID)
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                var Items = daoqi_dict_type.Items.Values.FirstOrDefault(p => p.ID == ID);
                if (Items != null)
                {
                    if (JadeBag.Values.FirstOrDefault(p => p.DBJadeList.TypeRune == Items.TypeRune) == null)
                    {
                        JadeList x = new JadeList()
                        {
                            ItemID = ID,
                        };

                        JadeBag.Add(x.ItemID, x);
                        var Msg = new MsgCombatGearTao.ProtoStructure() { Action = MsgCombatGearTao.Action.JadeListPirate };
                        Msg.Items = new List<MsgCombatGearTao.JadeBag>();
                        Msg.Items.Add(new MsgCombatGearTao.JadeBag() { ItemID = x.ItemID });
                        user.Send(stream.CreateTao(Msg));
                        UpdateNut();
                        return true;
                    }
                }

            }
            return false;
        }
        public void AddPoints(uint _Points)
        {
            using (var rec = new RecycledPacket())
            {
                var stream = rec.GetStream();
                AirPower += _Points;
                var Action = new MsgPirateOpt.ProtoStructure();
                Action.Type = (uint)MsgPirateOpt.Type.AddPoints;
                Action.UID = user.Player.UID;
                Action.ItemList = new MsgPirateOpt.ProtoStructure.Items
                {
                    Class = 7,
                    AirPower = AirPower
                };
                user.Send(stream.CreatePirateOpt(Action));
            }

        }

        public void UpdatePoints(uint _Points)
        {
            using (var rec = new RecycledPacket())
            {
                var stream = rec.GetStream();
                AirPower = _Points;
                var Action = new MsgPirateOpt.ProtoStructure();
                Action.Type = (uint)MsgPirateOpt.Type.AddPoints;
                Action.UID = user.Player.UID;
                Action.ItemList = new MsgPirateOpt.ProtoStructure.Items
                {
                    Class = 7,
                    AirPower = AirPower
                };
                user.Send(stream.CreatePirateOpt(Action));
            }

        }
        public bool FireArrowInc = false;


        public bool ActiveMele = false;
        public void Handel(IMapObj target, bool jmp)
        {
            if (target != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    uint use = 0;

                    #region ElementalArrow
                    FireArrowInc = false;
                    ActiveMele = false;
                    if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(target.X, target.Y, user.Player.X, user.Player.Y) <= 1)
                    {
                        ActiveMele = true;
                        if (user.Player.ContainFlag(MsgUpdate.Flags.FireArrow)
                                      && user.Player.ContainFlag(MsgUpdate.Flags.IceArrow)
                                      && user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow)
                                      && user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow)
                                      && user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                        {
                            use = 6;
                            user.Player.RemoveFlag(MsgUpdate.Flags.FireArrow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.IceArrow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.PoisonArrow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.ThunderArrow);
                            user.Player.RemoveFlag(MsgUpdate.Flags.WindArrow);
                        }
                    }
                    #endregion
                    #region HandleFlag
                    if (use == 0)
                    {

                        if (user.Player.ContainFlag(MsgUpdate.Flags.FireArrow))
                        {
                            if (jmp || Role.Core.Rate(20))
                            {
                                if (jmp)
                                    user.Player.RemoveFlag(MsgUpdate.Flags.FireArrow);
                                use = 1;
                            }
                        }
                        else if (user.Player.ContainFlag(MsgUpdate.Flags.IceArrow))
                        {
                            if (jmp || Role.Core.Rate(20))
                            {
                                if (jmp)
                                    user.Player.RemoveFlag(MsgUpdate.Flags.IceArrow);
                                use = 2;
                            }
                        }
                        else if (user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                        {
                            if (jmp || Role.Core.Rate(20))
                            {
                                if (jmp)
                                    user.Player.RemoveFlag(MsgUpdate.Flags.PoisonArrow);
                                use = 3;
                            }
                        }
                        else if (user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                        {
                            if (jmp || Role.Core.Rate(20))
                            {
                                if (jmp)
                                    user.Player.RemoveFlag(MsgUpdate.Flags.ThunderArrow);
                                use = 4;
                            }
                        }
                        else if (user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                        {
                            if (jmp || Role.Core.Rate(20))
                            {
                                if (jmp)
                                    user.Player.RemoveFlag(MsgUpdate.Flags.WindArrow);
                                use = 5;
                            }
                        }
                    }
                    #endregion
                    switch (use)
                    {
                        #region FireArrow
                        case 1:
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = (ushort)Flags.SpellID.FireArrow;
                                InteractQuery.X = target.X;
                                InteractQuery.Y = target.Y;
                                InteractQuery.OpponentUID = target.UID;
                                InteractQuery.UID = user.Player.UID;
                                MsgAttackPacket.ProcescMagic(user, stream, InteractQuery, true);
                                if (use == 6)
                                    goto case 2;
                                break;
                            }
                        #endregion
                        #region IceArrow
                        case 2:
                            {
                                if (use == 6)
                                    goto case 3;
                                ushort lvl = 0;
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ElementalArrow))
                                    lvl = user.MySpells.ClientSpells[(ushort)Flags.SpellID.ElementalArrow].Level;
                                var newspell = Pool.Magic[(ushort)Flags.SpellID.IceArrow][lvl];
                                Role.IMapObj targets;
                                if (newspell != null)
                                {
                                    var MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, target.X, target.Y, newspell.ID, newspell.Level, 0, 1);
                                    if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Monster))
                                    {
                                        var attacked = targets as Game.MsgMonster.MonsterRole;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                        {

                                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                            AnimationObj.UID = targets.UID;
                                            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                    if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Player))
                                    {
                                        var attacked = targets as Role.Player;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                            AnimationObj.UID = targets.UID;
                                            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                            attacked.AddFlag(MsgUpdate.Flags.NoXp, 10, true);
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.NoXp, 10, 1, 0, MsgUpdate.DataType.ArchiveSkill, false);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                    if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.SobNpc))
                                    {
                                        var attacked = targets as Role.SobNpc;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                            AnimationObj.UID = targets.UID;
                                            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                }
                                break;
                            }
                        #endregion
                        #region PoisonArrow
                        case 3:
                            {
                                if (use == 6)
                                    goto case 4;
                                Role.IMapObj targets;
                                var MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, target.X, target.Y, (ushort)Flags.SpellID.PoisonArrow, 1, 0);
                                if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Monster))
                                {
                                    var attacked = targets as Game.MsgMonster.MonsterRole;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                        AnimationObj.UID = targets.UID;
                                        AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                        AnimationObj.Damage = 1000;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Player))
                                {
                                    var attacked = targets as Role.Player;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                        AnimationObj.UID = targets.UID;
                                        AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                        AnimationObj.Damage = 1000;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.SobNpc))
                                {
                                    var attacked = targets as Role.SobNpc;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                        AnimationObj.UID = targets.UID;
                                        AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                        AnimationObj.Damage = 1000;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                                break;
                            }
                        #endregion
                        #region ThunderArrow
                        case 4:
                            {
                                if (use == 6)
                                    goto case 5;

                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ElementalArrow))
                                {
                                    var newspell = Pool.Magic[(ushort)Flags.SpellID.ThunderArrow][user.MySpells.ClientSpells[(ushort)Flags.SpellID.ElementalArrow].Level];
                                    Role.IMapObj targets;
                                    if (newspell != null)
                                    {
                                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, target.X, target.Y, newspell.ID, newspell.Level, 0, 0);
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Monster))
                                        {
                                            var attacked = targets as Game.MsgMonster.MonsterRole;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = targets.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                MsgSpell.Targets.Enqueue(AnimationObj);

                                            }
                                        }
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Player))
                                        {
                                            var attacked = targets as Role.Player;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = targets.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                List<MsgSpell> array = new List<MsgSpell>();
                                                foreach (MsgSpell spell in attacked.Owner.MySpells.ClientSpells.Values)
                                                {
                                                    spell.IsSpellWithColdTime = true;
                                                    spell.ColdTime = DateTime.Now.AddSeconds(5);
                                                    if (spell.GetColdTime > 0)
                                                        array.Add(spell);
                                                }
                                                if (array.Count > 0)
                                                {
                                                    MsgMagicColdTime.MagicColdTime item = new MsgMagicColdTime.MagicColdTime();
                                                    item.WriteSpells(array);
                                                    attacked.Owner.Send(stream.MagicColdTimeCreate(item));
                                                    attacked.AddSpellFlag(MsgUpdate.Flags.DragonFury, 5, false);
                                                    attacked.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DragonFury, 5, 0, 0, MsgUpdate.DataType.ArchiveSkill, false);
                                                }
                                            }
                                        }
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.SobNpc))
                                        {
                                            var attacked = targets as Role.SobNpc;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = targets.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region WindArrow
                        case 5:
                            {
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ElementalArrow))
                                {
                                    var newspell = Pool.Magic[(ushort)Flags.SpellID.WindArrow][user.MySpells.ClientSpells[(ushort)Flags.SpellID.ElementalArrow].Level];
                                    if (newspell != null)
                                    {
                                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, target.X, target.Y, newspell.ID, newspell.Level, 0, 1);
                                        Role.IMapObj targets;
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Monster))
                                        {
                                            var attacked = targets as Game.MsgMonster.MonsterRole;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = target.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;

                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Player))
                                        {
                                            var attacked = targets as Role.Player;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = targets.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;

                                                attacked.AddFlag(MsgUpdate.Flags.WindElementEffect, 10, true);
                                                attacked.SendUpdate(stream, MsgUpdate.Flags.WindElementEffect, 10, 1, 0, MsgUpdate.DataType.ArchiveSkill, false);
                                                attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante);
                                                MsgSpell.Targets.Enqueue(AnimationObj);

                                            }
                                        }
                                        if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.SobNpc))
                                        {
                                            var attacked = targets as Role.SobNpc;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                                AnimationObj.UID = targets.UID;
                                                AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;

                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(user);

                                    }
                                }
                                break;
                            }
                        #endregion
                        case 6:
                            {
                                goto case 1;
                            }

                    }
                    if (ActiveMele)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ElementalArrow))
                        {
                            Role.IMapObj targets;
                            var newspell = Pool.Magic[(ushort)Flags.SpellID.ElementalArrow][user.MySpells.ClientSpells[(ushort)Flags.SpellID.ElementalArrow].Level];
                            if (newspell != null)
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, target.X, target.Y, newspell.ID, newspell.Level, 0, 1);
                                if (user.Player.View.TryGetValue(target.UID, out targets, Role.MapObjectType.Monster))
                                {
                                    Game.MsgMonster.MonsterRole attacked = targets as Game.MsgMonster.MonsterRole;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, newspell))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(user.Player, attacked, newspell, out AnimationObj);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                if (user.Player.View.TryGetValue(target.UID, out targets, MapObjectType.Player))
                                {
                                    var attacked = targets as Role.Player;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, newspell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj();
                                        Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(user.Player, attacked, newspell, out AnimationObj, false, 1);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                                if (user.Player.View.TryGetValue(target.UID, out targets, Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targets as Role.SobNpc;
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, newspell))
                                    {
                                        short distance = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                        if (distance <= newspell.Range)
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(user.Player, attacked, newspell, out AnimationObj);
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }

                                }
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                            }
                        }
                    }
                }
            }
        }
        public bool RuneAttack(Role.Flags.SpellIDPirate x, IMapObj Target)
        {
            switch (x)
            {
                case Role.Flags.SpellIDPirate.Barrier:
                case Role.Flags.SpellIDPirate.Shell:
                case Role.Flags.SpellIDPirate.Storm:
                case Role.Flags.SpellIDPirate.Thrash:
                case Role.Flags.SpellIDPirate.ThunderPirate:
                case Role.Flags.SpellIDPirate.Torrent:
                case Role.Flags.SpellIDPirate.Tide:
                case Role.Flags.SpellIDPirate.Splash:
                case Role.Flags.SpellIDPirate.Sailing:
                case Role.Flags.SpellIDPirate.Vast:
                case Role.Flags.SpellIDPirate.Fusing:
                    {

                        return PirateSkills.ExecuteSkills(user, (ushort)x, user.Player.UID, Target.UID, Target.X, Target.Y);
                    }
            }
            return false;
        }
        public unsafe bool StarFlow(Client.GameClient client, uint UID, uint OpponentUID)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (this.isOpen(TypeID.StoneCracker))
                    {
                        #region StarFlow
                        if (client.Player.StarFlow >= 0 && client.Player.StarFlow <= 2)
                        {
                            client.Player.StarFlow++;
                            if (client.Player.StarFlow == 3)
                            {
                                client.Player.StarFlow = 4;
                                client.Player.RemoveFlag(MsgUpdate.Flags.StarFlow);
                                client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlowEx, 10, true);
                                InteractQuery AttackPaket = new InteractQuery();
                                AttackPaket.OpponentUID = 0;
                                AttackPaket.UID = client.Player.UID;
                                AttackPaket.X = client.Player.X;
                                AttackPaket.Y = client.Player.Y;
                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.StarFlow;
                                MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                            }
                            else
                            {
                                client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlow, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.StarFlow, 0, (uint)client.Player.StarFlow, 0, MsgUpdate.DataType.StarFlow);
                            }
                        }
                        if (client.Player.StarFlow == 4)
                            client.Player.StarFlow = 0;
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
        public bool PermanentFlags(Role.Flags.SpellIDPirate x)
        {
            return PirateSkills.ExecuteSkills(user, (ushort)x, user.Player.UID, user.Player.UID, user.Player.X, user.Player.Y, true);
        }
        public void ReceiveAttack(IMapObj Target)
        {
            if (Target != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (AtributesStatus.IsPirate(user.Player.Class))
                    {

                        if (isOpen(Archives.TypeID.ThunderNut) || isOpen(Archives.TypeID.FrozenNut) || isOpen(Archives.TypeID.LavaNut))
                        {
                            PirateSkills.ExecuteSkills(user, (ushort)Role.Flags.SpellIDPirate.Armed, user.Player.UID, Target.UID, Target.X, Target.Y, true);
                            PirateSkills.ExecuteSkills(user, (ushort)Role.Flags.SpellIDPirate.Sense, user.Player.UID, Target.UID, Target.X, Target.Y, true);
                            PirateSkills.ExecuteSkills(user, (ushort)Role.Flags.SpellIDPirate.Dark, user.Player.UID, Target.UID, Target.X, Target.Y, true);
                            List<ushort> Spell = new List<ushort>();
                            foreach (var x in Items.Values)
                            {
                                foreach (var Rune in x.Jades)
                                {
                                    if (Rune != null)
                                    {
                                        var DBItem = daoqi_dict_type.Items.Values.FirstOrDefault(p => p.ID == (uint)Rune.JadeID);
                                        if (DBItem != null)
                                        {
                                            ushort MagicId1 = (ushort)(DBItem.attr_value1 / 1000);
                                            ushort MagicId2 = (ushort)(DBItem.attr_value2 / 1000);
                                            if (MagicId1 != 0)
                                                Spell.Add(MagicId1);
                                            if (MagicId2 != 0)
                                                Spell.Add(MagicId2);
                                        }

                                    }
                                }
                            }
                            for (int x = 0; x < Spell.Count; x++)
                            {
                                if (RuneAttack((Role.Flags.SpellIDPirate)Spell[Program.GetRandom.Next(0, Spell.Count)], Target))
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #region TwistedFututr
        public uint TwistedFututrOpenRank = 0;


        public uint TwistedFututrExp = 0;
        public uint TwistedFututrLevel = 0;
        public uint TwistedFututrAirPower = 0;
        public uint TwistedFututrLevelSkill = 0;
        public uint TwistedFututrUnlocked = 0;//bool
        public uint TwistedFututrSelectMaterial = 0;
        #endregion
        public void AddRuneSkill(uint JadeID)
        {
            if (AtributesStatus.IsPirate(user.Player.Class))
            {
                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    ServerSockets.Packet stream = rec.GetStream();
                    var DBItem = daoqi_dict_type.Items.Values.FirstOrDefault(p => p.ID == JadeID);
                    if (DBItem != null)
                    {
                        if (DBItem.attr_type1 == 24)
                        {
                            ushort MagicId = (ushort)(DBItem.attr_value1 / 1000);
                            ushort Level = (ushort)(DBItem.attr_value1 % 1000);
                            user.MySpells.Add(stream, MagicId, Level);

                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Revelator)
                                user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Revelator);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Fusing)
                                user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Fusing);

                        }
                        if (DBItem.attr_type2 == 24)
                        {
                            ushort MagicId = (ushort)(DBItem.attr_value2 / 1000);
                            ushort Level = (ushort)(DBItem.attr_value2 % 1000);
                            user.MySpells.Add(stream, MagicId, Level);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Revelator)
                                user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Revelator);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Fusing)
                                user.MyArchives.PermanentFlags(Role.Flags.SpellIDPirate.Fusing);
                        }
                    }
                }
            }
        }
        public void RemoveRuneSkill(uint JadeID)
        {
            if (AtributesStatus.IsPirate(user.Player.Class))
            {
                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    ServerSockets.Packet stream = rec.GetStream();
                    var DBItem = daoqi_dict_type.Items.Values.FirstOrDefault(p => p.ID == JadeID);
                    if (DBItem != null)
                    {
                        if (DBItem.attr_type1 == 24)
                        {
                            ushort MagicId = (ushort)(DBItem.attr_value1 / 1000);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Revelator)
                                user.Player.RemoveFlag(MsgUpdate.Flags.Revelator);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Fusing)
                                user.Player.RemoveFlag(MsgUpdate.Flags.Fusing);
                            user.MySpells.Remove(MagicId, stream);
                        }
                        if (DBItem.attr_type2 == 24)
                        {
                            ushort MagicId = (ushort)(DBItem.attr_value2 / 1000);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Revelator)
                                user.Player.RemoveFlag(MsgUpdate.Flags.Revelator);
                            if (MagicId == (ushort)Role.Flags.SpellIDPirate.Fusing)
                                user.Player.RemoveFlag(MsgUpdate.Flags.Fusing);
                            user.MySpells.Remove(MagicId, stream);
                        }
                    }
                }
            }
        }
        public void UpdateRank()
        {
            #region InspiredWarrior
            if (AtributesStatus.IsWarrior(user.Player.Class))
            {
                if (TotalScore >= 1000)
                {
                    WarriorRank.Entry Weekly = new WarriorRank.Entry()
                    {
                        Type = WarriorRank.Type.WarriorWeekly,
                        TotalPoints = this.TotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Weekly.AddInfo(user);
                    WarriorRank.Ranks[WarriorRank.Type.WarriorWeekly].UpdateItem(Weekly);

                    var entry = new WarriorRank.Entry()
                    {
                        Type = WarriorRank.Type.Overall,
                        TotalPoints = TotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry.AddInfo(user);
                    WarriorRank.Ranks[WarriorRank.Type.Overall].UpdateItem(entry);
                    if (DragonhowlScore >= 1000)
                    {
                        var entry2 = entry.ShallowCopy();
                        entry2.Type = WarriorRank.Type.Dragonhowl;
                        entry2.TotalPoints = DragonhowlScore;
                        WarriorRank.Ranks[WarriorRank.Type.Dragonhowl].UpdateItem(entry2);
                    }
                    if (BloodlustScore >= 1000)
                    {
                        var entry3 = entry.ShallowCopy();
                        entry3.Type = WarriorRank.Type.Bloodlust;
                        entry3.TotalPoints = BloodlustScore;
                        WarriorRank.Ranks[WarriorRank.Type.Bloodlust].UpdateItem(entry3);
                    }
                    if (RedcurseScore >= 1000)
                    {
                        var entry4 = entry.ShallowCopy();
                        entry4.Type = WarriorRank.Type.Redcurse;
                        entry4.TotalPoints = RedcurseScore;
                        WarriorRank.Ranks[WarriorRank.Type.Redcurse].UpdateItem(entry4);
                    }
                }
            }
            else WarriorRank.Remove(user.Player.UID);
            #endregion

            #region InspiredArcher
            if (AtributesStatus.IsArcher(user.Player.Class))
            {
                if (TotalScore >= 1000)
                {
                    ArcherRank.Entry Weekly = new ArcherRank.Entry()
                    {
                        Type = ArcherRank.Type.WeeklyArcher,
                        TotalPoints = this.ArcherWeekTotalScore / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Weekly.AddInfo(user);
                    ArcherRank.Ranks[ArcherRank.Type.WeeklyArcher].UpdateItem(Weekly);

                    ArcherRank.Entry entry5 = new ArcherRank.Entry()
                    {
                        Type = ArcherRank.Type.Overall,
                        TotalPoints = this.ArcherTotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry5.AddInfo(user);
                    ArcherRank.Ranks[ArcherRank.Type.Overall].UpdateItem(entry5);

                    if (this.StoneCrackerScore >= 1000)
                    {
                        ArcherRank.Entry entry7 = entry5.ShallowCopy();
                        entry7.Type = ArcherRank.Type.StoneCracker;
                        entry7.TotalPoints = this.StoneCrackerScore;
                        ArcherRank.Ranks[ArcherRank.Type.StoneCracker].UpdateItem(entry7);
                    }
                    if (this.ColdMoonScore >= 1000)
                    {
                        ArcherRank.Entry entry8 = entry5.ShallowCopy();
                        entry8.Type = ArcherRank.Type.ColdMoon;
                        entry8.TotalPoints = this.ColdMoonScore;
                        ArcherRank.Ranks[ArcherRank.Type.ColdMoon].UpdateItem(entry8);
                    }
                    if (this.ThornCutterScore >= 1000)
                    {
                        ArcherRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = ArcherRank.Type.ThronCutter;
                        entry9.TotalPoints = this.ThornCutterScore;
                        ArcherRank.Ranks[ArcherRank.Type.ThronCutter].UpdateItem(entry9);
                    }
                }
            }
            else
                ArcherRank.Remove(user.Player.UID);
            #endregion


            #region InspiredMonk
            if (AtributesStatus.IsMonk(user.Player.Class))
            {
                if (MonkTotalScore >= 1000)
                {
                    MonkRanks.Entry Weekly = new MonkRanks.Entry()
                    {
                        Type = MonkRanks.Type.WeeklyMonk,
                        TotalPoints = this.MonkWeekTotalScore / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Weekly.AddInfo(user);
                    MonkRanks.Ranks[MonkRanks.Type.WeeklyMonk].UpdateItem(Weekly);

                    MonkRanks.Entry entry5 = new MonkRanks.Entry()
                    {
                        Type = MonkRanks.Type.TotalRanks,
                        TotalPoints = this.MonkTotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry5.AddInfo(user);
                    MonkRanks.Ranks[MonkRanks.Type.TotalRanks].UpdateItem(entry5);

                    if (this.HeavenlyTigerScore >= 1000)
                    {
                        MonkRanks.Entry entry7 = entry5.ShallowCopy();
                        entry7.Type = MonkRanks.Type.HeavenlyTiger;
                        entry7.TotalPoints = this.HeavenlyTigerScore;
                        MonkRanks.Ranks[MonkRanks.Type.HeavenlyTiger].UpdateItem(entry7);
                    }
                    if (this.MightyDragonScore >= 1000)
                    {
                        MonkRanks.Entry entry8 = entry5.ShallowCopy();
                        entry8.Type = MonkRanks.Type.MightyDragon;
                        entry8.TotalPoints = this.MightyDragonScore;
                        MonkRanks.Ranks[MonkRanks.Type.MightyDragon].UpdateItem(entry8);
                    }
                    if (this.CosmicRocScore >= 1000)
                    {
                        MonkRanks.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = MonkRanks.Type.CosmicRoc;
                        entry9.TotalPoints = this.CosmicRocScore;
                        MonkRanks.Ranks[MonkRanks.Type.CosmicRoc].UpdateItem(entry9);
                    }
                }
            }
            else
                MonkRanks.Remove(user.Player.UID);
            #endregion

            #region InspiredWater
            if (AtributesStatus.IsWater(user.Player.Class))
            {
                if (TaoTotalScore >= 1000)
                {
                    WaterRank.Entry WeeklyTao = new WaterRank.Entry()
                    {
                        Type = WaterRank.Type.WeeklyTao,
                        TotalPoints = this.TaoWeeklyTotalScore / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    WeeklyTao.AddInfo(user);
                    WaterRank.Ranks[WaterRank.Type.WeeklyTao].UpdateItem(WeeklyTao);

                    WaterRank.Entry entry5 = new WaterRank.Entry()
                    {
                        Type = WaterRank.Type.Overall,
                        TotalPoints = this.TaoTotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry5.AddInfo(user);
                    WaterRank.Ranks[WaterRank.Type.Overall].UpdateItem(entry5);
                    if (this.VicissitudeScore >= 1000)
                    {
                        WaterRank.Entry entry7 = entry5.ShallowCopy();
                        entry7.Type = WaterRank.Type.Vicissitude;
                        entry7.TotalPoints = this.VicissitudeScore;
                        WaterRank.Ranks[WaterRank.Type.Vicissitude].UpdateItem(entry7);
                    }
                    if (this.HighestGoodScore >= 1000)
                    {
                        WaterRank.Entry entry8 = entry5.ShallowCopy();
                        entry8.Type = WaterRank.Type.HighestGood;
                        entry8.TotalPoints = this.HighestGoodScore;
                        WaterRank.Ranks[WaterRank.Type.HighestGood].UpdateItem(entry8);
                    }
                    if (this.EvolutionScore >= 1000)
                    {
                        WaterRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = WaterRank.Type.Evolution;
                        entry9.TotalPoints = this.EvolutionScore;
                        WaterRank.Ranks[WaterRank.Type.Evolution].UpdateItem(entry9);
                    }
                    if (this.ThrillScore >= 1000)
                    {
                        WaterRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = WaterRank.Type.Thrill;
                        entry9.TotalPoints = this.ThrillScore;
                        WaterRank.Ranks[WaterRank.Type.Thrill].UpdateItem(entry9);
                    }
                    if (this.BirthdeathScore >= 1000)
                    {
                        WaterRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = WaterRank.Type.Birthdeath;
                        entry9.TotalPoints = this.BirthdeathScore;
                        WaterRank.Ranks[WaterRank.Type.Birthdeath].UpdateItem(entry9);
                    }
                }
            }
            else
                WaterRank.Remove(user.Player.UID);
            #endregion

            #region InspiredFire
            if (AtributesStatus.IsFire(user.Player.Class))
            {
                if (TaoTotalScore >= 1000)
                {
                    FireRank.Entry WeeklyTao = new FireRank.Entry()
                    {
                        Type = FireRank.Type.WeeklyTao,
                        TotalPoints = this.TaoWeeklyTotalScore / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    WeeklyTao.AddInfo(user);
                    FireRank.Ranks[FireRank.Type.WeeklyTao].UpdateItem(WeeklyTao);

                    FireRank.Entry entry5 = new FireRank.Entry()
                    {
                        Type = FireRank.Type.Overall,
                        TotalPoints = this.TaoTotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry5.AddInfo(user);
                    FireRank.Ranks[FireRank.Type.Overall].UpdateItem(entry5);
                    if (this.VicissitudeScore >= 1000)
                    {
                        FireRank.Entry entry7 = entry5.ShallowCopy();
                        entry7.Type = FireRank.Type.Vicissitude;
                        entry7.TotalPoints = this.VicissitudeScore;
                        FireRank.Ranks[FireRank.Type.Vicissitude].UpdateItem(entry7);
                    }
                    if (this.HighestGoodScore >= 1000)
                    {
                        FireRank.Entry entry8 = entry5.ShallowCopy();
                        entry8.Type = FireRank.Type.HighestGood;
                        entry8.TotalPoints = this.HighestGoodScore;
                        FireRank.Ranks[FireRank.Type.HighestGood].UpdateItem(entry8);
                    }
                    if (this.EvolutionScore >= 1000)
                    {
                        FireRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = FireRank.Type.Evolution;
                        entry9.TotalPoints = this.EvolutionScore;
                        FireRank.Ranks[FireRank.Type.Evolution].UpdateItem(entry9);
                    }
                    if (this.ThrillScore >= 1000)
                    {
                        FireRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = FireRank.Type.Thrill;
                        entry9.TotalPoints = this.ThrillScore;
                        FireRank.Ranks[FireRank.Type.Thrill].UpdateItem(entry9);
                    }
                    if (this.BirthdeathScore >= 1000)
                    {
                        FireRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = FireRank.Type.Birthdeath;
                        entry9.TotalPoints = this.BirthdeathScore;
                        FireRank.Ranks[FireRank.Type.Birthdeath].UpdateItem(entry9);
                    }
                }
            }
            else
                FireRank.Remove(user.Player.UID);
            #endregion

            #region InspiredPirate
            if (AtributesStatus.IsPirate(user.Player.Class))
            {
                if (OverallRankings >= 10000)
                {
                    PirateRank.Entry Ranks = new PirateRank.Entry()
                    {
                        Type = PirateRank.Type.Overall,
                        TotalPoints = this.OverallRankings,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Ranks.AddInfo(user);
                    PirateRank.Ranks[PirateRank.Type.Overall].UpdateItem(Ranks);
                    PirateRank.Entry Weekly = new PirateRank.Entry()
                    {
                        Type = PirateRank.Type.WeeklyPirate,
                        TotalPoints = this.WeeklyPirateRankings / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Weekly.AddInfo(user);
                    PirateRank.Ranks[PirateRank.Type.WeeklyPirate].UpdateItem(Weekly);
                    uint Rank = PirateRank.GetMyRank(PirateRank.Type.Overall, user.Player.UID);
                    if (Rank == 1)
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Dark))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.Dark, (ushort)8);
                        }
                    }
                    else
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.Dark))
                                user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.Dark, stream);
                        }
                    }
                    uint Rankss = PirateRank.GetMyRank(PirateRank.Type.Overall, user.Player.UID);
                    if (Rankss == 1 || Rankss == 2 || Rankss == 3)
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LordThreat))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellIDPirate.LordThreat);
                        }
                    }
                    else
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDPirate.LordThreat))
                                user.MySpells.Remove((ushort)Role.Flags.SpellIDPirate.LordThreat, stream);
                        }
                    }
                }
            }
            else
                PirateRank.Remove(user.Player.UID);
            #endregion

            #region InspiredLeeLong
            if (AtributesStatus.IsLee(user.Player.Class))
            {
                if (LeeLongWeekTotalScore >= 5000)
                {
                    LeeLongRank.Entry Weekly = new LeeLongRank.Entry()
                    {
                        Type = LeeLongRank.Type.LeeLongWeekly,
                        TotalPoints = this.LeeLongWeekTotalScore / 5,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    Weekly.AddInfo(user);
                    LeeLongRank.Ranks[LeeLongRank.Type.LeeLongWeekly].UpdateItem(Weekly);

                    LeeLongRank.Entry entry5 = new LeeLongRank.Entry()
                    {
                        Type = LeeLongRank.Type.LeeLongOverall,
                        TotalPoints = this.LeeLongTotalScore,
                        UID = user.Player.UID,
                        Name = user.Player.Name,
                        Level = (byte)user.Player.Level,
                        Class = user.Player.Class,
                        Mesh = user.Player.Mesh
                    };
                    entry5.AddInfo(user);
                    LeeLongRank.Ranks[LeeLongRank.Type.LeeLongOverall].UpdateItem(entry5);

                    if (this.DragonScore >= 5000)
                    {
                        LeeLongRank.Entry entry7 = entry5.ShallowCopy();
                        entry7.Type = LeeLongRank.Type.Dragon;
                        entry7.TotalPoints = this.DragonScore;
                        LeeLongRank.Ranks[LeeLongRank.Type.Dragon].UpdateItem(entry7);
                    }
                    if (this.KunpengScore >= 5000)
                    {
                        LeeLongRank.Entry entry8 = entry5.ShallowCopy();
                        entry8.Type = LeeLongRank.Type.Kunpeng;
                        entry8.TotalPoints = this.KunpengScore;
                        LeeLongRank.Ranks[LeeLongRank.Type.Kunpeng].UpdateItem(entry8);
                    }
                    if (this.SuanniScore >= 5000)
                    {
                        LeeLongRank.Entry entry9 = entry5.ShallowCopy();
                        entry9.Type = LeeLongRank.Type.Suanni;
                        entry9.TotalPoints = this.SuanniScore;
                        LeeLongRank.Ranks[LeeLongRank.Type.Suanni].UpdateItem(entry9);
                    }
                    uint Rank = LeeLongRank.GetMyRank(LeeLongRank.Type.LeeLongOverall, user.Player.UID);
                    if (Rank == 1)
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Inchstrength))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Inchstrength, 1);
                        }
                    }
                    else
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Inchstrength))
                                user.MySpells.Remove((ushort)Role.Flags.SpellID.Inchstrength, stream);
                        }
                    }
                    uint Rankss = LeeLongRank.GetMyRank(LeeLongRank.Type.LeeLongOverall, user.Player.UID);
                    if (Rankss == 1 || Rankss == 2 || Rankss == 3)
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (Rankss == 1)
                            {
                                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.OneInchRay))
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 4);
                                else
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 4);
                            }
                            if (Rankss == 2)
                            {
                                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.OneInchRay))
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 3);
                                else
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 3);
                            }
                            if (Rankss == 2)
                            {
                                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.OneInchRay))
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 2);
                                else
                                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.OneInchRay, 2);
                            }

                        }
                    }
                    else
                    {
                        using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                        {
                            ServerSockets.Packet stream = rec.GetStream();
                            if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.OneInchRay))
                                user.MySpells.Remove((ushort)Role.Flags.SpellID.OneInchRay, stream);
                        }
                    }
                }
            }
            else
                LeeLongRank.Remove(user.Player.UID);
            #endregion


            //#region Dune
            //if (AtributesStatus.IsDune(user.Player.Class))
            //{
            //    if (DuneTotalScore >= 5000)
            //    {
            //        DuneRank.Entry Weekly = new DuneRank.Entry()
            //        {
            //            Type = DuneRank.Type.DuneWeekly,
            //            TotalPoints = this.DuneTotalScore,
            //            UID = user.Player.UID,
            //            Name = user.Player.Name,
            //            Level = (byte)user.Player.Level,
            //            Class = user.Player.Class,
            //            Mesh = user.Player.Mesh
            //        };
            //        Weekly.AddInfo(user);
            //        //DuneRank.Ranks[DuneRank.Type.DuneWeekly].UpdateItem(Weekly);

            //        DuneRank.Entry entry5 = new DuneRank.Entry()
            //        {
            //            Type = DuneRank.Type.DuneOverall,
            //            TotalPoints = this.DuneTotalScore,
            //            UID = user.Player.UID,
            //            Name = user.Player.Name,
            //            Level = (byte)user.Player.Level,
            //            Class = user.Player.Class,
            //            Mesh = user.Player.Mesh
            //        };
            //        entry5.AddInfo(user);
            //        DuneRank.Ranks[DuneRank.Type.DuneOverall].UpdateItem(entry5);

            //        if (this.ConceptionScore >= 5000)
            //        {
            //            DuneRank.Entry entry7 = entry5.ShallowCopy();
            //            entry7.Type = DuneRank.Type.Conception;
            //            entry7.TotalPoints = this.ConceptionScore;
            //            DuneRank.Ranks[DuneRank.Type.Conception].UpdateItem(entry7);
            //        }
            //        if (this.GobernadorScore >= 5000)
            //        {
            //            DuneRank.Entry entry8 = entry5.ShallowCopy();
            //            entry8.Type = DuneRank.Type.Gobernador;
            //            entry8.TotalPoints = this.GobernadorScore;
            //            DuneRank.Ranks[DuneRank.Type.Gobernador].UpdateItem(entry8);
            //        }
            //        if (this.BeltScore >= 5000)
            //        {
            //            DuneRank.Entry entry9 = entry5.ShallowCopy();
            //            entry9.Type = DuneRank.Type.Belt;
            //            entry9.TotalPoints = this.BeltScore;
            //            DuneRank.Ranks[DuneRank.Type.Belt].UpdateItem(entry9);
            //        }

            //        if (DuneRank.GetMyRank(DuneRank.Type.DuneWeekly, user.Player.UID) == 1)
            //        {
            //            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            //            {
            //                ServerSockets.Packet stream = rec.GetStream();
            //                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InnerSight))
            //                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.InnerSight, 1);
            //                else
            //                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.InnerSight, 1);
            //            }
            //        }
            //        else
            //        {
            //            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            //            {
            //                ServerSockets.Packet stream = rec.GetStream();
            //                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InnerSight))
            //                    user.MySpells.Remove((ushort)Role.Flags.SpellID.InnerSight, stream);
            //            }
            //        }

            //        var level = -1;
            //        if (DuneRank.GetMyRank(DuneRank.Type.DuneOverall, user.Player.UID) == 1)
            //        {
            //            level += 2;
            //        }
            //        if (DuneRank.GetMyRank(DuneRank.Type.Conception, user.Player.UID) == 1)
            //        {
            //            level += 1;
            //        }
            //        if (DuneRank.GetMyRank(DuneRank.Type.Gobernador, user.Player.UID) == 1)
            //        {
            //            level += 1;
            //        }
            //        if (DuneRank.GetMyRank(DuneRank.Type.Belt, user.Player.UID) == 1)
            //        {
            //            level += 1;
            //        }

            //        if (level > 0)
            //        {
            //            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            //            {
            //                ServerSockets.Packet stream = rec.GetStream();
            //                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DualStrike))
            //                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DualStrike, (ushort)level);
            //                else
            //                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DualStrike, (ushort)level);
            //            }
            //        }
            //        else
            //        {
            //            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            //            {
            //                ServerSockets.Packet stream = rec.GetStream();
            //                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DualStrike))
            //                    user.MySpells.Remove((ushort)Role.Flags.SpellID.DualStrike, stream);
            //            }
            //        }
            //    }
            //}
            //else
            //    DuneRank.Remove(user.Player.UID);
            //#endregion

        }
        public void UpdateNut()
        {
            if (Collection == null) Collection = new NutAtributes();
            for (int type = 700; type < 704; type++)
            {
                int Score = 0;
                switch (type)
                {
                    case 701:
                    case 702:
                    case 703:
                        {

                            Archives.Item obj;
                            if (Items.TryGetValue((Archives.TypeID)type, out obj))
                            {
                                Score += (int)(obj.DBItem.Score + obj.MasteryPoints);
                            }
                            foreach (var Rune in JadeBag.Values)
                            {
                                if (Rune.DBJadeList.TypeCombatGear == type)
                                {
                                    Score += (int)Rune.DBJadeList.score;
                                }
                            }
                            break;
                        }
                    case 700:
                        {
                            foreach (var Rune in JadeBag.Values)
                            {
                                if (Rune.DBJadeList.TypeCombatGear == 0 && Rune.DBJadeList.TypeRune % 100 == 7)
                                {
                                    Score += (int)Rune.DBJadeList.score;
                                }
                            }
                            break;
                        }

                }
                foreach (var x in dict_collection.Runecollection.Values)
                {
                    if (x.Type == type)
                    {

                        if (Score >= x.Min && Score <= x.Mix)
                        {
                            if (x.Attribute == dict_collection.RuneAttribute.MaxHP)
                                Collection.MaxHP = x.Value1;

                            if (x.Attribute == dict_collection.RuneAttribute.PAttack)
                                Collection.PAttack = x.Value1;

                            if (x.Attribute == dict_collection.RuneAttribute.FinalPAttack)
                                Collection.FinalPAttack = x.Value1;

                            if (x.Attribute == dict_collection.RuneAttribute.FinalPDamage)
                                Collection.FinalPDamage = x.Value1;



                        }
                    }

                }
            }
            user.Equipment.QueryEquipment(user.Equipment.Alternante);
        }

        public uint Reiki, MasteryPoints, AirPower, UniversalFragment, RefinePts;

        public EffectFlags Effect = EffectFlags.None;
        public uint LevelCombatHeart = 0;
        public int LevelCombatProgress = 0;
        public override string ToString()
        {
            WriteLine file = new WriteLine('/');

            file.Add(Items.Count);
            foreach (var att in Items.Values)
            {
                file.Add((uint)att.ItemID);
                file.Add(att.Level);
                file.Add(att.Progress);
                file.Add((uint)att.Hash);
                file.Add(att.dwParam);
                file.Add(att.MasteryPoints);

                for (int i = 0; i < 2; i++)
                    for (int x = 0; x < 7; x++)
                        file.Add(att.Animas[i].AnimaID[x]);

                for (int i = 0; i < 6; i++)
                    file.Add(att.Jades[i].JadeID);
            }

            file.Add(AirPowers.Count);
            foreach (var Power in AirPowers.Values)
            {
                file.Add((uint)Power.Type);
                file.Add(Power.Level);
                file.Add(Power.Exp);
            }

            file.Add(user.MyArchives.JadeBag.Values.Count);
            foreach (var item in user.MyArchives.JadeBag.Values)
                file.Add((uint)item.ItemID);

            file.Add(MasteryPoints);
            file.Add(Reiki);
            file.Add(AirPower);
            file.Add(UniversalFragment);
            file.Add(RefinePts);
            file.Add((int)Effect);
            file.Add(LevelCombatHeart);
            file.Add(LevelCombatProgress);

            return file.Close();
        }
        public void Load(string line)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(line))
                {

                    return;
                }

                ReadLine reader = new ReadLine(line, '/');

                int itemCount = reader.Read(0);

                for (int i = 0; i < itemCount; i++)
                {
                    Item item = new Item();

                    item.ItemID = (Archives.TypeID)reader.Read((uint)0);
                    item.Level = reader.Read((uint)0);
                    item.Progress = reader.Read((uint)0);
                    item.Hash = (byte)reader.Read((uint)0);
                    item.dwParam = reader.Read((uint)0);
                    item.MasteryPoints = reader.Read((uint)0);


                    for (int ii = 0; ii < 2; ii++)
                    {
                        for (int x = 0; x < 7; x++)
                        {
                            item.Animas[ii].AnimaID[x] = reader.Read((uint)0);


                        }
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        item.Jades[j].JadeID = reader.Read((uint)0);


                    }

                    if (!Items.ContainsKey(item.ItemID))
                    {
                        Items.Add(item.ItemID, item);

                    }
                }

                int airCount = reader.Read(0);


                for (int i = 0; i < airCount; i++)
                {


                    items Power = new items();

                    Power.Type = (AirTypeID)reader.Read((uint)0);
                    Power.Level = reader.Read((uint)0);
                    Power.Exp = reader.Read((uint)0);



                    if (!AirPowers.ContainsKey(Power.Type))
                    {
                        AirPowers.Add(Power.Type, Power);

                    }
                }

                int jadeCount = reader.Read(0);

                for (int i = 0; i < jadeCount; i++)
                {
                    uint id = reader.Read((uint)0);


                    if (!user.MyArchives.JadeBag.ContainsKey(id))
                    {
                        JadeList jade = new JadeList();
                        jade.ItemID = id;

                        user.MyArchives.JadeBag.Add(id, jade);

                    }
                }

                MasteryPoints = reader.Read((uint)0);
                Reiki = reader.Read((uint)0);
                AirPower = reader.Read((uint)0);
                UniversalFragment = reader.Read((uint)0);
                RefinePts = reader.Read((uint)0);
                Effect = (EffectFlags)reader.Read((int)0);
                LevelCombatHeart = reader.Read((uint)0);
                LevelCombatProgress = reader.Read((int)0);

            }
            catch (Exception ex)
            {
                //Console.WriteLine("========================================");
                Console.WriteLine("[Load] EXCEPTION");
                Console.WriteLine(ex.ToString());
                //Console.WriteLine("========================================");
            }
        }
        public ushort GetSkillMonk(IMapObj Target)
        {
            try
            {
                MsgSpell user_spell = null;
                List<ushort> ushortList = new List<ushort>();
                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ZenStaff, out user_spell))
                {
                    MagicType.Magic ZenStaff = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ZenStaff))
                    {
                        if (Role.Core.Rate(ZenStaff.Rate))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.ZenStaff);
                        }
                    }
                }
                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.VioletBowl, out user_spell))
                {
                    MagicType.Magic VioletBowl = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VioletBowl))
                    {
                        if (Role.Core.Rate(VioletBowl.Rate))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.VioletBowl);
                        }
                    }
                }

                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlowerTouch, out user_spell))
                {
                    MagicType.Magic FlowerTouch = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FlowerTouch))
                    {
                        if (Role.Core.Rate(FlowerTouch.Rate))
                        {
                            ushortList.Add((ushort)Role.Flags.SpellID.FlowerTouch);
                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.VajraPalm, out user_spell))
                            {
                                MagicType.Magic VajraPalm = Pool.Magic[user_spell.ID][user_spell.Level];
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.VajraPalm))
                                {
                                    if (Role.Core.Rate(VajraPalm.Rate))
                                    {
                                        ushortList.Add((ushort)Role.Flags.SpellID.VajraPalm);

                                    }
                                }
                            }

                        }
                    }
                }
                return ushortList[Program.GetRandom.Next(0, ushortList.Count)];
            }
            catch
            {

            }
            return 0;
        }

    }

}
