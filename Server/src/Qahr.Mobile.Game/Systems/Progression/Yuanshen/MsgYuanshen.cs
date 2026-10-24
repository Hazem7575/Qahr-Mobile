using System;
using System.Linq;
using ProtoBuf;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgServer
{
    [ProtoContract]
    public class MsgYuanshen
    {
        [System.Flags]
        public enum ActionID : byte
        {
            OracleComposing = 0,
            Lottery = 1,
            Cultivate = 2,
            CultivateNow = 3,
            Ascend = 4,
            Calling = 5,
            Flag = 7,
            add = 8,
        }
        [ProtoMember(1)]
        public byte Type;
        [ProtoMember(2)]
        public uint CallingType;
        [ProtoMember(3)]
        public uint Member3;
        [ProtoMember(4)]
        public uint Member4;
        [ProtoMember(5)]
        public uint Member5;
        [ProtoMember(6)]
        public uint[] EonspiritItem;
        [ProtoMember(7)]
        public System.Collections.Generic.List<Items> ItemsUpgradeList = new System.Collections.Generic.List<Items>();
        [ProtoContract]
        public class Items
        {
            [ProtoMember(1)]
            public uint UID;
            [ProtoMember(2)]
            public uint Size;
        }
        public ServerSockets.Packet ToArray()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(this);
                stream.Finalize((ushort)GamePackets.MsgYuanshen);
                return stream;
            }
        }
        public static implicit operator ServerSockets.Packet(MsgYuanshen obj)
        {
            return obj.ToArray();
        }
        public static void OpenSkillsGui(GameClient Owner, ServerSockets.Packet msg)
        {

            uint GUI_ID = 1480;
            opengui(Owner, msg, GUI_ID);
        }
        public static void opengui(GameClient client, ServerSockets.Packet msg, uint gui)
        {
            ActionQuery decompose = new ActionQuery();
            decompose.ObjId = client.Player.UID;
            decompose.Type = 126;
            decompose.dwParam = gui;
            decompose.TargetPositionX = client.Player.X;
            decompose.TargetPositionY = client.Player.Y;
            client.Send(msg.ActionCreate(decompose));
        }
        public static void RemvedGUI(Client.GameClient User)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                uint[] Tasks = new uint[] { 07884, 07885, 07886, 13960, 13961, 13962, 13963, 13964, 13965, 13966, 13967, };
                if (global::Qahr.Mobile.Phone.MobileClient.Is(User))// QahrMobile: one list packet (action 3, status 1); accept+finish re-ran the mobile guide per quest
                {
                    global::Qahr.Mobile.Phone.MobileQuestStory.SendFinishedList(User, Tasks);
                    return;
                }
                foreach (var task in Tasks)
                {
                    stream.InitWriter();
                    stream.Write((ushort)1);//Type 4
                    stream.Write((ushort)1);//Count 6
                    stream.Write((uint)0);//Clear 8 
                    stream.Write((uint)0);//CompleteTimes 12
                    stream.Write((uint)0);//RefreshTimes 16
                    stream.Write((uint)0);//LastTime 20
                    stream.Write(task);//Id 24
                    stream.Write((uint)0);//CompleteFlag 28
                    stream.Write(0);//TaskOvertime 32
                    stream.Finalize(Game.GamePackets.QuestList);
                    User.Send(stream);
                }
                foreach (var task in Tasks)
                {
                    stream.InitWriter();
                    stream.Write((ushort)4);//Type 4
                    stream.Write((ushort)1);//Count 6
                    stream.Write((uint)0);//Clear 8 
                    stream.Write((uint)0);//CompleteTimes 12
                    stream.Write((uint)0);//RefreshTimes 16
                    stream.Write((uint)0);//LastTime 20
                    stream.Write(task);//Id 24
                    stream.Write((uint)1);//CompleteFlag 28
                    stream.Write(0);//TaskOvertime 32
                    stream.Finalize(Game.GamePackets.QuestList);
                    User.Send(stream);
                }
            }
        }
        public static void OpenGUI(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var actions = new ActionQuery()
                {
                    ObjId = client.Player.UID,
                    Type = ActionType.AnimaCrest,
                    Strings = new string[1] { "LuaUIMgr_OpenWindow|3877" },
                };
                client.Send(stream.ActionCreate(actions));
            }
        }
        public static void UpdateEnergy(Client.GameClient client, uint level)
        {
            if (client.EonspiritSystem.Count() > 0)
            {
                if (client.Player.Alive)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!client.Player.ContainFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.DivineArrival))
                        {
                            client.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.EonspiritCurrentEnergy, int.MaxValue, false);
                            switch (level)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 12;
                                        break;
                                    }
                                case 4:
                                case 5:
                                case 6:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 13;
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 14;
                                        break;
                                    }
                                case 8:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 15;
                                        break;
                                    }
                                case 9:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 16;
                                        break;
                                    }
                                case 10:
                                    {
                                        client.Player.EonspiritCurrentEnergy += 20;
                                        break;
                                    }
                                default:
                                    {
                                        client.Player.EonspiritCurrentEnergy = 0;
                                        break;
                                    }
                            }
                            if (client.Player.EonspiritCurrentEnergy > 1000)
                                client.Player.EonspiritCurrentEnergy = 1000;
                            var info = new MsgUpdate(stream, client.Player.UID, 1);
                            info.Append(stream, client.Player.EonspiritCurrentEnergy);
                            client.Send(info.GetArray(stream));
                        }
                        else
                        {
                            var info = new MsgUpdate(stream, client.Player.UID, 1);
                            info.Append(stream, 0);
                            client.Send(info.GetArray(stream));
                        }
                    }
                }
            }
        }
        public static uint GetExp(uint itemId, uint size = 0)
        {
            uint baseExp;
            var expValues = new System.Collections.Generic.Dictionary<uint, uint> { { 3348377, 100 }, { 3347151, 200 }, { 3347152, 500 }, { 3347153, 1500 }, { 3347154, 4500 }, { 3347155, 13500 }, { 3347156, 40500 } };
            return expValues.TryGetValue(itemId, out baseExp) ? baseExp * size : 0;
        }
        public static uint GetExpCard(uint itemtype1, uint itemtype2)
        {
            uint Exp;
            uint TotalExp = 0;
            byte level = (byte)(itemtype2 % 100);
            switch (level)
            {
                case 1: TotalExp = 0; break;
                case 2: TotalExp = 200; break;
                case 3: TotalExp = 1200; break;
                case 4: TotalExp = 2700; break;
                case 5: TotalExp = 7200; break;
                case 6: TotalExp = 16200; break;
                case 7: TotalExp = 29700; break;
                case 8: TotalExp = 70200; break;
                case 9: TotalExp = 191700; break;
                case 10: TotalExp = 515700; break;
            }
            if (Math.Floor(itemtype1 / 100.0 % 10000) == Math.Floor(itemtype2 / 100.0 % 10000))
            {
                Exp = ((TotalExp + 200) * 60 / 100);
            }
            else
            {
                Exp = (TotalExp + 200) * 40 / 100;
            }
            return Exp > 0 ? Exp : 0;
        }
        public static bool NewRate(int value)
        {
            if (value >= 100) return true;
            if (value < 1) return false;
            return Pool.GetRandom.Next(100) < value;
        }
        public static bool Rate(double Value)
        {
            if (Value <= 0) return false;
            if (Value >= 100) return true;
            int percentgen = Pool.GetRandom.Next(0, 99);
            int maingen = Pool.GetRandom.Next(0, 100);
            double thepercent = double.Parse(maingen.ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + percentgen.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
            return (thepercent <= Value);
        }
        public static bool ChanceCalc(double chance)
        {
            const int divisor = 10000000;
            const int maxValue = 100 * divisor;
            try
            {
                return Pool.GetRandom.Next(0, maxValue) <= chance * divisor;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChanceCalcAsync(double): {Message}", ex.Message);
                return false;
            }
        }
        private static uint GetCount(uint itemType)
        {
            if (itemType == 0)
                return 1;
            else
                return 10;
        }
        private static uint GetItemID(uint itemType)
        {
            if (itemType == 1)
                return 3347158;
            else
                return 3347157;
        }
        public static bool CheckSpells(ushort ID)
        {
            return System.Enum.IsDefined(typeof(Qahr.Mobile.Role.Enums.SpellID), ID);
        }
        public int SwordBodyData0 = 0;
        public int SwordBodyData1 = 0;
        public int SwordBodyPower = 0;
        public static Qahr.Mobile.Role.Enums.EonspiritItem GetItemName(uint ID)
        {
            switch (ID / 10000)
            {
                case 1001: return Qahr.Mobile.Role.Enums.EonspiritItem.IronArmoredTitan;
                case 1002: return Qahr.Mobile.Role.Enums.EonspiritItem.ConquerorXiangYu;
                case 1003: return Qahr.Mobile.Role.Enums.EonspiritItem.DesertNightblad;
                case 1004: return Qahr.Mobile.Role.Enums.EonspiritItem.HuntressArtemis;
                case 1005: return Qahr.Mobile.Role.Enums.EonspiritItem.FrostWolf;
                case 1006:
                case 1060: return Qahr.Mobile.Role.Enums.EonspiritItem.VictoriousBuddha;
                case 1007: return Qahr.Mobile.Role.Enums.EonspiritItem.Thunderstorm;
                case 1008: return Qahr.Mobile.Role.Enums.EonspiritItem.GoldenDragon;
                case 1009: return Qahr.Mobile.Role.Enums.EonspiritItem.Phoenix;
                case 1010: return Qahr.Mobile.Role.Enums.EonspiritItem.WarmasterLadyEmpyrean;
                default: return Qahr.Mobile.Role.Enums.EonspiritItem.Null;
            }
        }
        private static YuanshenLottery.item GetRandomItem(uint callType)
        {
            var itemType = callType == 1 ? YuanshenLottery.LotteryType.RegularCall : YuanshenLottery.LotteryType.PreciousCalling;
            var items = YuanshenLottery.YuanshenLotteryItem.Values.Where(i => i.Type == itemType).ToArray();
            if (itemType == YuanshenLottery.LotteryType.RegularCall)
            {
                if (NewRate(48)) return items.Where(o => o.ItemID % 1000 == 101).ToArray().GetRandom();
                if (NewRate(60)) return items.Where(o => o.ItemID % 1000 == 102).ToArray().GetRandom();
            }
            else
            {
                if (Rate(1)) return items.Where(o => o.ItemID % 1000 == 106).ToArray().GetRandom();
                if (Rate(1)) return items.Where(o => o.ItemID % 1000 == 105).ToArray().GetRandom();
                if (Rate(1)) return items.Where(o => o.ItemID % 1000 == 104).ToArray().GetRandom();
                if (Rate(1)) return items.Where(o => o.ItemID % 1000 == 103).ToArray().GetRandom();
                if (Rate(1)) return items.Where(o => o.ItemID % 1000 == 102).ToArray().GetRandom();
            }
            return items.Where(o => o.ItemID == GetFallbackItemID(itemType)).ToArray().GetRandom();
        }
        private static uint GetFallbackItemID(YuanshenLottery.LotteryType itemType)
        {
            if (itemType == YuanshenLottery.LotteryType.PreciousCalling)
            {
                if (NewRate(1)) return 3347156;
                else if (NewRate(1)) return 3347155;
                else if (NewRate(2)) return 3347154;
                else if (NewRate(3)) return 3347153;
                else return 3347152;
            }
            else
            {
                return 3348377;
            }
        }
        private static void SendLotteryResults(Client.GameClient client, System.Collections.Generic.List<YuanshenLottery.item> items, uint callType)
        {
            var obj = new MsgYuanshen
            {
                Type = (byte)ActionID.Lottery,
                Member4 = 3,
                EonspiritItem = new uint[items.Count()],
            };
            int i = 0;
            foreach (var item in items)
            {
                obj.EonspiritItem[i] = item.id;
                i++;
            }
            client.Send(obj);
            var obj2 = new MsgYuanshen
            {
                Type = (byte)ActionID.Calling,
                CallingType = (uint)(callType == 1 ? YuanshenLottery.LotteryType.RegularCall : YuanshenLottery.LotteryType.PreciousCalling),
                EonspiritItem = callType == 1 ? new uint[2] { client.Player.RegularCallCount, client.Player.RegularCallCount } : new uint[2] { client.Player.PreciousCallingCount, client.Player.PreciousCallingCount }
            };
            client.Send(obj2);
        }
        private static void ProcessReceivedItems(Client.GameClient client, System.Collections.Generic.List<YuanshenLottery.item> items, ServerSockets.Packet stream)
        {
            foreach (var item in items)
            {
                var newItem = new MsgGameItem
                {
                    Mode = Role.Flags.ItemMode.AddItem,
                    UID = Pool.ITEM_Counter.Next,
                    ITEM_ID = item.ItemID,
                    StackSize = item.CountItem,
                    MaximDurability = 1,
                    Durability = 1,
                    EonspiritActived = 0,
                    Position = Database.ItemType.EonspiritItem.Contains(item.ItemID) ? (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritInventory : (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.Inventory
                };
                if (Database.ItemType.EonspiritItem.Contains(newItem.ITEM_ID))
                {
                    if (client.EonspiritSystem.TryAdd(newItem.UID, newItem))
                    {
                        newItem.Send(client, stream);
                    }
                }
                else
                {
                    client.Inventory.AddItemWitchStack(newItem.ITEM_ID, 0, newItem.StackSize, stream);
                }
            }
        }

        public static bool ProcescMagic(Flags.SpellID SpellID, GameClient User, IMapObj Target, byte Type = 0)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MsgSpell user_spell = null;
                if (User.MySpells.ClientSpells.TryGetValue((ushort)SpellID, out user_spell))
                {
                    MagicType.Magic Magic = Pool.Magic[user_spell.ID][user_spell.Level];
                    float Chance = 0;
                    if (SpellID == Flags.SpellID.Warth)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Damage3;
                        }
                    }
                    if (SpellID == Flags.SpellID.CityRazing)
                    {
                        Chance = Magic.Rate % 1000;
                    }
                    if (SpellID == Flags.SpellID.CrescentChop)
                    {
                        Chance = Magic.Rate / 1000;
                        if (Target is Player)
                        {
                            if (Warth((Target as Player).Owner))
                                Chance = 0;
                        }
                    }
                    if (SpellID == Flags.SpellID.Whirlwind)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }
                    if (SpellID == Flags.SpellID.ManiacDance1)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }
                    //

                    if (SpellID == Flags.SpellID.AirBlocker)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Damage3;
                        }
                    }
                    if (SpellID == Flags.SpellID.AmazingSpeed)
                    {
                        Chance = Magic.Rate % 1000;
                    }


                    if (SpellID == Flags.SpellID.GreatAwakening)
                    {
                        Chance = Magic.Rate / 1000;
                        if (Target is Player)
                        {
                            if (Warth((Target as Player).Owner))
                                Chance = 0;
                        }
                    }
                    if (SpellID == Flags.SpellID.SuperFlash)
                    {
                        Chance = Magic.Rate / 1000;
                        if (Target is Player)
                        {
                            if (Warth((Target as Player).Owner))
                                Chance = 0;
                        }
                    }
                    if (SpellID == Flags.SpellID.BurningSun)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }
                    if (SpellID == Flags.SpellID.SunsetShine)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }

                    //

                    if (SpellID == Flags.SpellID.SwordBody)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Damage3;
                        }
                    }
                    if (SpellID == Flags.SpellID.BenefitShower)
                    {
                        Chance = Magic.Rate % 1000;
                    }
                    if (SpellID == Flags.SpellID.AirBlocker)
                    {
                        Chance = Magic.Rate / 1000;
                        if (Target is Player)
                        {
                            if (Warth((Target as Player).Owner))
                                Chance = 0;
                        }
                    }
                    if (SpellID == Flags.SpellID.EchoingSwords)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }
                    if (SpellID == Flags.SpellID.ImmortalDestroyer)
                    {
                        Chance = Magic.Rate;
                        if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                        {
                            Chance += Magic.Width;
                        }
                    }

                    if (Role.Core.Rate((int)Chance, 100))
                    {
                        MsgAttackPacket.ProcescMagic(User, stream, new InteractQuery
                        {
                            SpellID = user_spell.ID,
                            SpellLevel = user_spell.Level,
                            X = User.Player.X,
                            Y = User.Player.Y,
                            UID = User.Player.UID,
                            OpponentUID = Target.UID
                        }, true);
                        return true;
                    }
                }
            }
            return false;
        }
        public static void SwordBody(GameClient User, MsgSpellAnimation.SpellObj SpellObj)
        {
            if (User.Player.ContainFlag(MsgUpdate.Flags.SwordBody))
            {

                // percent/cap are stored on cast (YuanshenAttack: GDamage/1000 + DamageOnHuman, or GDamage%100 + Damage2 under DivineArrival)
                int SwordBody = MulDiv((int)SpellObj.Damage, (int)User.Player.SwordBodyPercentage, 100);
                SwordBody = Math.Min((int)User.Player.SwordBodyMax, SwordBody);
                SpellObj.Damage = (uint)Math.Max(1, SpellObj.Damage - SwordBody);
            }
        }



        //====================================================
        public static void Magics(Client.GameClient User, IMapObj Target, byte Type = 0)
        {
            //AfterAttack=0
            //AfterDamage=1
            switch (Type)
            {
                case 0:
                    {
                        ProcescMagic(Flags.SpellID.CityRazing, User, Target, Type);
                        ProcescMagic(Flags.SpellID.AmazingSpeed, User, Target, Type);
                        ProcescMagic(Flags.SpellID.GrandDoctrine, User, Target, Type);

                        ProcescMagic(Flags.SpellID.SwordBody, User, Target, Type);
                        ProcescMagic(Flags.SpellID.BenefitShower, User, Target, Type);
                        break;
                    }
                case 1:
                    {
                        ProcescMagic(Flags.SpellID.GrandDoctrine, User, Target, Type);
                        ProcescMagic(Flags.SpellID.CityRazing, User, Target, Type);
                        ProcescMagic(Flags.SpellID.AmazingSpeed, User, Target, Type);

                        ProcescMagic(Flags.SpellID.SwordBody, User, Target, Type);
                        break;
                    }
            }
        }
        //====================================================
        public static int MulDiv(int number, int numerator, int denominator)
        {
            return (int)((long)number * numerator / denominator);
        }
        //====================================================

        //====================================================
        public static bool Warth(Client.GameClient User)
        {
            //Dealing damage grants a 10% chance to ignore Flame Shield and the damage reduction effect of Sword Body. Transform into a Eonspirit to increase the chance to 20%.
            float Chance = 0;
            MsgSpell user_spell = null;
            if (User.MySpells.ClientSpells.TryGetValue((ushort)Flags.SpellID.Warth, out user_spell))
            {
                MagicType.Magic Magic = Pool.Magic[user_spell.ID][user_spell.Level];
                Chance = Magic.Rate;
                if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                {
                    Chance = Magic.StatusData0;
                }
            }
            return Role.Core.Rate((int)Chance, 100);
        }
        //====================================================
        public static bool AirBlocker(Client.GameClient User)
        {

            //Grants a 20% chance to attack flying targets. Transform into a Eonspirit to increase the chance to 40%.
            float Chance = 0;
            MsgSpell user_spell = null;
            if (User.MySpells.ClientSpells.TryGetValue((ushort)Flags.SpellID.AirBlocker, out user_spell))
            {
                MagicType.Magic Magic = Pool.Magic[user_spell.ID][user_spell.Level];
                Chance = Magic.Rate;
                if (User.Player.ContainFlag(MsgUpdate.Flags.Yuanshen))
                {
                    Chance = Magic.StatusData0;
                }
            }
            //return true;
            return Role.Core.Rate((int)Chance, 100);

        }
        private static System.Collections.Generic.List<ushort> Spells = new System.Collections.Generic.List<ushort>() { 25440, 25450, 25410, 25430, 25480, 24790, 24810, 24540, 22840, 24380, 24730, 24740, 24840, 24850, 24690, 24700, 24710, 24680, 24820, 24830, 24420, 24450, 24500, 24520 };
        public static void UpdateSpells(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var item in Spells)
                {
                    client.MySpells.Remove(item, stream);
                }
                foreach (var Item in client.EonspiritSystem.Values.Where(i => i.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive))
                {
                    if (Database.ItemType.EonspiritItem.Contains(Item.ITEM_ID))
                    {
                        var Info = Database.YuanshenAttr.YuanshenAttrItem.Values.Where(i => i.ItemID == Item.ITEM_ID && i.TypeLevel == Item.EonspiritPercentage).FirstOrDefault();
                        if (Info != null)
                        {

                            client.MySpells.Add(stream, Info.IDSkill, Info.LevelSkill);
                            client.MySpells.Add(stream, Info.IDSkill2, Info.LevelSkill2);
                            client.MySpells.Add(stream, Info.IDSkill3, Info.LevelSkill3);
                            client.MySpells.Add(stream, Info.IDSkill4, Info.LevelSkill4);
                            client.MySpells.Add(stream, Info.IDSkill5, Info.LevelSkill5);
                            client.MySpells.Add(stream, Info.IDSkill6, Info.LevelSkill6);
                            client.MySpells.Add(stream, Info.IDSkill7, Info.LevelSkill7);
                            //client.Status.MaxHitpoints += Info.HPValue;
                            //client.Player.HitPoints += (int)Info.HPValue;
                            //client.Status.MinAttack += Info.AttackValue;
                            //client.Status.MinAttack += Info.AttackValue;
                        }
                    }
                }
            }
        }
        [Packet((ushort)GamePackets.MsgYuanshen)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            var Info = new MsgYuanshen();
            Info = stream.ProtoBufferDeserialize(Info);
            switch ((ActionID)Info.Type)
            {
                case ActionID.OracleComposing:
                    {
                        //if (client.Player.UnLockedSystem == false)
                        //    break;
                        uint Amount = (uint)Info.EonspiritItem[0];
                        uint ItemType = (uint)Info.EonspiritItem[1];
                        uint GetCountItem = 0;
                        // Must be '&&': only the 5 real oracle inputs are valid (the old '||' was always true).
                        // Cost is 2 inputs per 1 output; bound Amount so 'Amount*2' can't overflow a ushort.
                        if (ItemType >= 3347151 && ItemType <= 3347155
                            && Amount >= 1 && (ulong)Amount * 2 <= ushort.MaxValue)
                        {
                            if (client.Inventory.HaveSpace((byte)Math.Min(6, Amount)))
                            {
                                // Count the material server-side: dedup client UIDs, bag only, matching ItemType.
                                System.Collections.Generic.HashSet<uint> seenUIDs = new System.Collections.Generic.HashSet<uint>();
                                foreach (var item in Info.ItemsUpgradeList)
                                {
                                    if (!seenUIDs.Add(item.UID))
                                        continue;
                                    MsgGameItem Material;
                                    if (client.Inventory.TryGetItem(item.UID, out Material))
                                    {
                                        if (Material.ITEM_ID == ItemType)
                                            GetCountItem += Material.StackSize;
                                    }
                                }
                                uint OracleCost = Amount * 2;
                                if (GetCountItem >= OracleCost)
                                {
                                    // Remove exactly the server-computed cost of the correct material; abort on failure.
                                    if (!client.Inventory.Remove(ItemType, OracleCost, stream))
                                        break;
                                    uint NightOracle = 0;
                                    uint StarlightOracle = 0;
                                    uint MoonshadeOracle = 0;
                                    uint DawnOracle = 0;
                                    uint LightOracle = 0;
                                    uint BrillianceOracle = 0;
                                    switch (ItemType)
                                    {
                                        case 3347151:
                                            {
                                                for (int x = 0; x < Amount; x++)
                                                {
                                                    float num = 0;
                                                    float[] Rate = new float[] { 10, 30, 90, 1500, 3370, 5000 };
                                                    for (int i = 0; i < Rate.Length; i++)
                                                    {
                                                        num += Rate[i];
                                                        if (ChanceCalc(num / 100f))
                                                        {
                                                            switch (i)
                                                            {
                                                                case 0: BrillianceOracle++; break;
                                                                case 1: LightOracle++; break;
                                                                case 2: DawnOracle++; break;
                                                                case 3: MoonshadeOracle++; break;
                                                                case 4: StarlightOracle++; break;
                                                                case 5: NightOracle++; break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3347152:
                                            {
                                                for (int x = 0; x < Amount; x++)
                                                {
                                                    float num = 0;
                                                    float[] Rate = new float[] { 20, 80, 120, 2680, 7100 };
                                                    for (int i = 0; i < Rate.Length; i++)
                                                    {
                                                        num += Rate[i];
                                                        if (ChanceCalc(num / 100f))
                                                        {
                                                            switch (i)
                                                            {
                                                                case 0: BrillianceOracle++; break;
                                                                case 1: LightOracle++; break;
                                                                case 2: DawnOracle++; break;
                                                                case 3: MoonshadeOracle++; break;
                                                                case 4: StarlightOracle++; break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3347153:
                                            {
                                                for (int x = 0; x < Amount; x++)
                                                {
                                                    float num = 0;
                                                    float[] Rate = new float[] { 100, 300, 2500, 7100 };
                                                    for (int i = 0; i < Rate.Length; i++)
                                                    {
                                                        num += Rate[i];
                                                        if (ChanceCalc(num / 100f))
                                                        {
                                                            switch (i)
                                                            {
                                                                case 0: BrillianceOracle++; break;
                                                                case 1: LightOracle++; break;
                                                                case 2: DawnOracle++; break;
                                                                case 3: MoonshadeOracle++; break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3347154:
                                            {
                                                for (int x = 0; x < Amount; x++)
                                                {
                                                    float num = 0;
                                                    float[] Rate = new float[] { 500, 3000, 6500 };
                                                    for (int i = 0; i < Rate.Length; i++)
                                                    {
                                                        num += Rate[i];
                                                        if (ChanceCalc(num / 100f))
                                                        {
                                                            switch (i)
                                                            {
                                                                case 0: BrillianceOracle++; break;
                                                                case 1: LightOracle++; break;
                                                                case 2: DawnOracle++; break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3347155:
                                            {
                                                for (int x = 0; x < Amount; x++)
                                                {
                                                    float num = 0;
                                                    float[] Rate = new float[] { 5000, 5000 };
                                                    for (int i = 0; i < Rate.Length; i++)
                                                    {
                                                        num += Rate[i];
                                                        if (ChanceCalc(num / 100f))
                                                        {
                                                            switch (i)
                                                            {
                                                                case 0: BrillianceOracle++; break;
                                                                case 1: LightOracle++; break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    var Compose = new MsgYuanshen()
                                    {
                                        Type = (byte)ActionID.OracleComposing,
                                        Member4 = 3,
                                    };
                                    if (NightOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347151, Size = NightOracle });
                                    if (StarlightOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347152, Size = StarlightOracle });
                                    if (MoonshadeOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347153, Size = MoonshadeOracle });
                                    if (DawnOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347154, Size = DawnOracle });
                                    if (LightOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347155, Size = LightOracle });
                                    if (BrillianceOracle != 0)
                                        Compose.ItemsUpgradeList.Add(new Items() { UID = 3347156, Size = BrillianceOracle });
                                    foreach (var item in Compose.ItemsUpgradeList)
                                    {
                                        client.Inventory.AddItemWitchStack(item.UID, 0, (ushort)item.Size, stream);
                                    }
                                    client.Send(Compose);
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Lottery:
                    {
                        //if (client.Player.UnLockedSystem == false)
                        //    break;
                        uint count = GetCount(Info.EonspiritItem[1]);
                        uint itemID = GetItemID(Info.EonspiritItem[0]);
                        if (itemID == 0)
                        {
                            Console.WriteLine("Not Found Any Type Of Lotter New Type is => " + Info.EonspiritItem[0]);
                            break;
                        }
                        if (client.Inventory.Contain(itemID, count))
                        {
                            if (client.Inventory.Remove(itemID, count, stream))
                            {
                                var items = new System.Collections.Generic.List<YuanshenLottery.item>();
                                for (int i = 0; i < (Info.EonspiritItem[1] == 1 ? 10 : 1); i++)
                                {
                                    items.Add(GetRandomItem(Info.EonspiritItem[0]));
                                }
                                if (Info.EonspiritItem[0] == 1)
                                {
                                    client.Player.RegularCallCount++;
                                }
                                else if (Info.EonspiritItem[0] == 2)
                                {
                                    client.Player.PreciousCallingCount++;
                                }
                                SendLotteryResults(client, items, Info.EonspiritItem[0]);
                                ProcessReceivedItems(client, items, stream);
                            }
                        }
                        break;
                    }

                case ActionID.Cultivate:
                    {
                        //if (client.Player.UnLockedSystem == false)
                        //    break;
                        MsgGameItem itemSystem, itemInfo;
                        if (client.EonspiritSystem.TryGetValue(Info.EonspiritItem[0], out itemSystem))
                        {
                            uint totalExp = 0;
                            bool isUpdated = false;
                            // Dedup the client UID list so the same material can't be fed (and counted) twice.
                            System.Collections.Generic.HashSet<uint> seenCultivateUIDs = new System.Collections.Generic.HashSet<uint>();
                            foreach (var item in Info.ItemsUpgradeList)
                            {
                                if (!seenCultivateUIDs.Add(item.UID))
                                    continue;
                                // Use the FULL uint size everywhere (no 16-bit cut), and reject out-of-range sizes
                                // so the EXP can't be credited for more than is actually consumed.
                                uint size = item.Size;
                                if (client.Inventory.TryGetItem(item.UID, out itemInfo)
                                    && size >= 1 && size <= ushort.MaxValue && size <= itemInfo.StackSize
                                    && client.Inventory.Contain(itemInfo.ITEM_ID, size))
                                {
                                    var plus = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == itemSystem.ITEM_ID % 100 && i.TypeLevel == itemSystem.EonspiritPercentage);
                                    if (plus != null)
                                    {
                                        uint exp = GetExp(itemInfo.ITEM_ID, size);
                                        totalExp += exp;
                                        client.Inventory.RemoveStackItem(itemInfo.ITEM_ID, (ushort)size, stream);
                                    }
                                }
                                else if (client.EonspiritSystem.TryGetValue(item.UID, out itemInfo) && client.EonspiritSystem.ContainsKey(itemInfo.UID))
                                {
                                    uint exp = GetExpCard(itemSystem.ITEM_ID, itemInfo.ITEM_ID);
                                    totalExp += exp;
                                    client.EonspiritSystem.Remove(itemInfo.UID);
                                    client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, itemInfo.UID, 0, 0, 0, 0, 0, 0, null));
                                }
                            }
                            if (totalExp > 0)
                            {
                                itemSystem.EonspiritExp += totalExp;
                                var plus = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == itemSystem.ITEM_ID % 100 && i.TypeLevel == itemSystem.EonspiritPercentage);
                                if (plus != null)
                                {
                                    while (itemSystem.EonspiritExp >= plus.Exp)
                                    {
                                        itemSystem.EonspiritExp -= plus.Exp;
                                        itemSystem.EonspiritPercentage = 1;
                                        isUpdated = true;
                                    }
                                }
                                itemSystem.Mode = Role.Flags.ItemMode.Update;
                                itemSystem.Send(client, stream);
                                var response = new MsgYuanshen
                                {
                                    Type = (byte)ActionID.Cultivate,
                                    EonspiritItem = new uint[3] { itemSystem.UID, (uint)(isUpdated ? 1 : 0), isUpdated ? 0 : itemSystem.EonspiritExp }
                                };
                                client.Send(response);
                                if (isUpdated)
                                {
                                    UpdateSpells(client);
                                    uint Exp = 0;
                                    foreach (var msg_item in client.EonspiritSystem.Values)
                                    {
                                        var Infos = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == msg_item.ITEM_ID % 100 && i.TypeLevel == msg_item.EonspiritPercentage);
                                        if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive || msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                                        {
                                            Exp += Infos.Rating;
                                            client.Player.EonspiritPrestrige = Infos.Rating;
                                        }
                                    }
                                    if (Exp > 0)
                                    {
                                        var entry = new Database.YuanshenRank.Entry()
                                        {
                                            Type = Database.YuanshenRank.Type.Overall_EonSpirit,
                                            TotalPoints = Exp,
                                            UID = client.Player.UID,
                                            Name = client.Player.Name,
                                            Level = (byte)client.Player.Level,
                                            Class = client.Player.Class,
                                            Mesh = client.Player.Mesh,
                                        };
                                        entry.AddInfo(client);
                                        Database.YuanshenRank.Ranks[Database.YuanshenRank.Type.Overall_EonSpirit].UpdateItem(entry);
                                    }
                                    else
                                    {
                                        Database.YuanshenRank.Remove(client.Player.UID);
                                    }
                                }
                            }
                        }
                        break;
                    }

                case ActionID.CultivateNow:
                    {
                        //if (client.Player.UnLockedSystem == false)
                        //    break;
                        MsgGameItem itemSystem;
                        if (client.EonspiritSystem.TryGetValue(Info.EonspiritItem[0], out itemSystem))
                        {
                            var plus = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == itemSystem.ITEM_ID % 100 && i.TypeLevel == itemSystem.EonspiritPercentage);
                            if (plus != null)
                            {
                                double rate = ((double)itemSystem.EonspiritExp / plus.Exp) * 100;
                                bool success = Rate(rate);
                                itemSystem.EonspiritExp = 0;
                                itemSystem.EonspiritPercentage = (uint)(success ? 1 : 0);
                                itemSystem.Mode = Role.Flags.ItemMode.Update;
                                itemSystem.Send(client, stream);
                                var response = new MsgYuanshen
                                {
                                    Type = (byte)ActionID.CultivateNow,
                                    EonspiritItem = new uint[4] { itemSystem.UID, (uint)(success ? 1 : 0), (uint)(success ? 1 : 0), 0 }
                                };
                                client.Send(response);
                                if (success)
                                {
                                    UpdateSpells(client);
                                    uint Exp = 0;
                                    foreach (var msg_item in client.EonspiritSystem.Values)
                                    {
                                        var Infos = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == msg_item.ITEM_ID % 100 && i.TypeLevel == msg_item.EonspiritPercentage);
                                        if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive || msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                                        {
                                            Exp += Infos.Rating;
                                            client.Player.EonspiritPrestrige = Infos.Rating;
                                        }
                                    }
                                    if (Exp > 0)
                                    {
                                        var entry = new Database.YuanshenRank.Entry()
                                        {
                                            Type = Database.YuanshenRank.Type.Overall_EonSpirit,
                                            TotalPoints = Exp,
                                            UID = client.Player.UID,
                                            Name = client.Player.Name,
                                            Level = (byte)client.Player.Level,
                                            Class = client.Player.Class,
                                            Mesh = client.Player.Mesh,
                                        };
                                        entry.AddInfo(client);
                                        Database.YuanshenRank.Ranks[Database.YuanshenRank.Type.Overall_EonSpirit].UpdateItem(entry);
                                    }
                                    else
                                    {
                                        Database.YuanshenRank.Remove(client.Player.UID);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Ascend:
                    {
                        //if (client.Player.UnLockedSystem == false)
                        //break;
                        MsgGameItem itemSystem, itemSystem2;
                        if (client.EonspiritSystem.TryGetValue(Info.EonspiritItem[0], out itemSystem) && client.EonspiritSystem.TryGetValue(Info.EonspiritItem[1], out itemSystem2))
                        {
                            bool isSameType = (itemSystem.ITEM_ID % 100 == itemSystem2.ITEM_ID % 100) && (itemSystem.ITEM_ID / 100 == itemSystem2.ITEM_ID / 100);
                            if (isSameType)
                            {
                                client.EonspiritSystem.Remove(itemSystem2.UID);
                                client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, itemSystem2.UID, 0, 0, 0, 0, 0, 0, null));
                                itemSystem.ITEM_ID = Math.Min(itemSystem.ITEM_ID + 1, (itemSystem.ITEM_ID / 100) * 100 + 10);
                                itemSystem.EonspiritExp = 0;
                                itemSystem.EonspiritPercentage = 0;
                                itemSystem.Mode = Role.Flags.ItemMode.Update;
                                itemSystem.Send(client, stream);
                                var response = new MsgYuanshen
                                {
                                    Type = (byte)ActionID.Ascend,
                                    EonspiritItem = new uint[2] { itemSystem.UID, 1 }
                                };
                                client.Send(response);
                                UpdateSpells(client);
                                uint Exp = 0;
                                foreach (var msg_item in client.EonspiritSystem.Values)
                                {
                                    var Infos = Database.YuanshenLevUP.YuanshenLevUPItem.Values.FirstOrDefault(i => i.Level == msg_item.ITEM_ID % 100 && i.TypeLevel == msg_item.EonspiritPercentage);
                                    if (msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive || msg_item.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                                    {
                                        Exp += Infos.Rating;
                                        client.Player.EonspiritPrestrige = Infos.Rating;
                                    }
                                }
                                if (Exp > 0)
                                {
                                    var entry = new Database.YuanshenRank.Entry()
                                    {
                                        Type = Database.YuanshenRank.Type.Overall_EonSpirit,
                                        TotalPoints = Exp,
                                        UID = client.Player.UID,
                                        Name = client.Player.Name,
                                        Level = (byte)client.Player.Level,
                                        Class = client.Player.Class,
                                        Mesh = client.Player.Mesh,
                                    };
                                    entry.AddInfo(client);
                                    Database.YuanshenRank.Ranks[Database.YuanshenRank.Type.Overall_EonSpirit].UpdateItem(entry);
                                }
                                else
                                {
                                    Database.YuanshenRank.Remove(client.Player.UID);
                                }
                            }
                        }
                        break;
                    }

                case ActionID.Calling:
                    {
                        Info.EonspiritItem = new uint[2];
                        Info.EonspiritItem[0] = Info.CallingType == 1 ? client.Player.RegularCallCount : client.Player.PreciousCallingCount;
                        Info.EonspiritItem[1] = Info.CallingType == 1 ? client.Player.RegularCallCount : client.Player.PreciousCallingCount;
                        client.Send(Info);
                        break;
                    }
                case ActionID.add:
                    {

                        client.Send(Info);
                        break;
                    }
                case ActionID.Flag:
                    {
                        Info.EonspiritItem = new uint[1] { 15 };
                        client.Send(Info);
                        break;
                    }
                case (ActionID)6:
                case (ActionID)10:
                    {
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Error in Action => " + Info.Type);
                        break;
                    }
            }
        }
    }
}