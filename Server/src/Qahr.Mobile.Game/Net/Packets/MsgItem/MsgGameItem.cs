using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetItemPacketPacket(this ServerSockets.Packet stream, out MsgGameItem item)
        {
            item = new MsgGameItem();
            item.UID = stream.ReadUInt32();//4
            item.ITEM_ID = stream.ReadUInt32();//8
            item.Durability = stream.ReadUInt16();//12
            item.MaximDurability = stream.ReadUInt16();//14
            item.Mode = (Role.Flags.ItemMode)stream.ReadUInt16();//16
            item.Position = stream.ReadUInt8();//18
            item.SocketProgress = stream.ReadUInt32();//19
            item.SocketOne = (Role.Flags.Gem)stream.ReadUInt8();//23
            item.SocketTwo = (Role.Flags.Gem)stream.ReadUInt8();//24
            if (Database.ItemType.isRune(item.ITEM_ID))
            {
                item.SpellID = stream.ReadUInt32();//25
                item.RuneView = stream.ReadUInt8();//29
            }
            else
            {
                item.Effect = (Role.Flags.ItemEffect)stream.ReadUInt16();//25
                stream.SeekForward(sizeof(ushort) + sizeof(byte));//27
            }
            item.Plus = stream.ReadUInt8();//30
            item.Bless = stream.ReadUInt8();//31
            item.Bound = stream.ReadUInt8();//32
            item.Enchant = stream.ReadUInt8();//33
            item.ProgresGreen = stream.ReadUInt8();//34
            stream.SeekForward(sizeof(ushort) + 1);//35
            item.Suspicious = stream.ReadUInt8();//38
            stream.SeekForward(sizeof(byte));//39
            item.Locked = stream.ReadUInt8();//40
            stream.SeekForward(sizeof(byte));//41
            item.PlusProgress = stream.ReadUInt32();//42
            stream.ReadUInt32();//46 RemainingTime
            stream.ReadUInt32();//50 Unknow
            item.StackSize = stream.ReadUInt16();//54
            item.PerfectionLevel = stream.ReadUInt32();//56
            item.PerfectionProgress = stream.ReadUInt32();//60
            item.OwnerUID = stream.ReadUInt32();//64
            item.OwnerName = stream.ReadCString(32);//68
            item.Signature = stream.ReadCString(64);//100
            stream.SeekForward(sizeof(ushort));//164
            item.RuneEXP = stream.ReadUInt32();//166
            if (item.RelicAttributes != null)
                for (int i = 0; i < item.RelicAttributes.Length; i++)
                    item.RelicAttributes[i] = new Role.Instance.RelicAttribute(stream.ReadUInt32());//170 174 178 182 186
            else
                stream.SeekForward(sizeof(ulong) * 3);
            item.AnimaItemID = stream.ReadUInt32();//190
            stream.ReadUInt32();
            item.MythSoulID = stream.ReadUInt32();
            item.MythSoulProgress = stream.ReadUInt32();
            stream.ReadUInt32();
            stream.ReadUInt32();
            item.Mutacion = stream.ReadUInt32();
            item.Mutacion2 = stream.ReadUInt32();
            item.UpsurgeID = stream.ReadUInt32();
            item.YuanshenIndex = stream.ReadUInt32();
            item.YuanshenTime = stream.ReadUInt32();

        }
    }
    public class MsgGameItem
    {
        [ProtoContract]
        public class MsgItemInfoProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ulong UID;

            [ProtoMember(2, IsRequired = true)]
            public ulong ITEM_ID;

            [ProtoMember(3, IsRequired = true)]
            public ulong Durability;

            [ProtoMember(4, IsRequired = true)]
            public ulong MaximDurability;

            [ProtoMember(5, IsRequired = true)]
            public ulong Mode;

            [ProtoMember(6, IsRequired = true)]
            public ulong Member6;//Missing

            [ProtoMember(7, IsRequired = true)]
            public ulong Position;

            [ProtoMember(8, IsRequired = true)]
            public ulong SocketProgress;

            [ProtoMember(9, IsRequired = true)]
            public ulong SocketOne;

            [ProtoMember(10, IsRequired = true)]
            public ulong SocketTwo;

            [ProtoMember(11, IsRequired = true)]
            public ulong SpellID;

            [ProtoMember(12, IsRequired = true)]
            public ulong RuneView;

            [ProtoMember(13, IsRequired = true)]
            public ulong Plus;

            [ProtoMember(14, IsRequired = true)]
            public ulong Bless;

            [ProtoMember(15, IsRequired = true)]
            public ulong Bound;

            [ProtoMember(16, IsRequired = true)]
            public ulong Enchant;

            [ProtoMember(17, IsRequired = true)]
            public ulong ProgresGreen;

            [ProtoMember(18, IsRequired = true)]
            public ulong Suspicious;

            [ProtoMember(19, IsRequired = true)]
            public ulong Locked;

            [ProtoMember(20, IsRequired = true)]
            public ulong PlusProgress;

            [ProtoMember(21, IsRequired = true)]
            public ulong RemainingTime;

            [ProtoMember(22, IsRequired = true)]
            public ulong TimeLeftInMinutes;

            [ProtoMember(23, IsRequired = true)]
            public ulong StackSize;

            [ProtoMember(24, IsRequired = true)]
            public ulong PerfectionLevel;

            [ProtoMember(25, IsRequired = true)]
            public ulong PerfectionProgress;

            [ProtoMember(26, IsRequired = true)]
            public ulong OwnerUID;

            [ProtoMember(27, IsRequired = true)]
            public string OwnerName;

            [ProtoMember(28, IsRequired = true)]
            public string Signature;

            [ProtoMember(29, IsRequired = true)]
            public ulong Member29;//Missing

            [ProtoMember(30, IsRequired = true)]
            public ulong RuneEXP;

            [ProtoMember(31, IsRequired = true)]
            public ulong[] RelicAttributes;

            [ProtoMember(32, IsRequired = true)]
            public ulong AnimaItemID;

            [ProtoMember(33, IsRequired = true)]
            public ulong MythsoulEffect;

            [ProtoMember(34, IsRequired = true)]
            public ulong MythsoulID;

            [ProtoMember(35, IsRequired = true)]
            public ulong MythsoulEXP;

            [ProtoMember(36, IsRequired = true)]
            public ulong Member36;//Missing

            [ProtoMember(37, IsRequired = true)]
            public ulong MythsoulIDNumber2;

            [ProtoMember(38, IsRequired = true)]
            public ulong MythsoulIDNumber3;

            [ProtoMember(39, IsRequired = true)]
            public ulong RuneUnsealState;// Ideal Rune unseal flags (4 = permanent)

            [ProtoMember(40, IsRequired = true)]
            public ulong EonspiritActived;

            [ProtoMember(41, IsRequired = true)]
            public ulong EonspiritTime;

            [ProtoMember(42, IsRequired = true)]
            public ulong SpecialQuenchID;// Highly Refining material itemtype

            [ProtoMember(43, IsRequired = true)]
            public ulong SpecialQuenchExpire;// 0 = permanent
        }

        public Database.ItemType.DBItem DBInfo
        {
            get
            {
                if (Pool.ItemsBase.ContainsKey(ITEM_ID))
                    return Pool.ItemsBase[ITEM_ID];
                return new Database.ItemType.DBItem();
            }
        }
        public bool isYuanshen()
        {
            return ITEM_ID >= 10100101 && ITEM_ID <= 10100110
                || ITEM_ID >= 10020101 && ITEM_ID <= 10020110
                || ITEM_ID >= 10040101 && ITEM_ID <= 10040110
                || ITEM_ID >= 10060101 && ITEM_ID <= 10060110;
        }
        public uint Data1
        {
            get { return RelicAttributes[0]; }
            set { RelicAttributes[0] = new Role.Instance.RelicAttribute(value); }
        }
        public uint Data2
        {
            get { return RelicAttributes[1]; }
            set { RelicAttributes[1] = new Role.Instance.RelicAttribute(value); }
        }
        public uint Data3
        {
            get { return RelicAttributes[2]; }
            set { RelicAttributes[2] = new Role.Instance.RelicAttribute(value); }
        }
        public uint Data4
        {
            get { return RelicAttributes[3]; }
            set { RelicAttributes[3] = new Role.Instance.RelicAttribute(value); }
        }
        public uint Data5
        {
            get { return RelicAttributes[4]; }
            set { RelicAttributes[4] = new Role.Instance.RelicAttribute(value); }
        }
        public string ToLog()
        {
            return "Name: " + Pool.ItemsBase[ITEM_ID].Name + ": UID: " + UID.ToString() + " | " + "ID: " + ITEM_ID.ToString() + " | " + "Plus: " + Plus.ToString() + " | " + "Bless: " + Bless.ToString() + " | " + "Anima: " + AnimaItemID.ToString() + " | " + "Perfection Level: " + PerfectionLevel.ToString() + " | " + "S1: " + SocketOne.ToString() + " | " + "S2: " + SocketTwo.ToString();
        }
        public MsgItemExtra.Purification Purification;
        public MsgItemExtra.Refinery Refinary;

        public ConcurrentDictionary<uint, MsgGameItem> Deposite = new ConcurrentDictionary<uint, MsgGameItem>();



        public void Send(MsgInterServer.PipeClient user, ServerSockets.Packet stream)
        {
            user.Send(ItemCreate(stream, this));
            if (Purification.ItemUID != 0 || Refinary.ItemUID != 0)
            {
                MsgItemExtra extra = new MsgItemExtra();
                if (Purification.InLife)
                {
                    if (Purification.SecoundsLeft == 0)
                        Purification.Typ = MsgItemExtra.Typing.Stabilization;
                    else
                        Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                    extra.Purifications.Add(Purification);
                }
                if (Refinary.InLife)
                {
                    Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                    if (Refinary.EffectDuration == 0)
                        Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                    extra.Refinerys.Add(Refinary);
                }
                user.Send(extra.CreateArray(stream, true));
            }
        }
        public static Counter ItemUID = new Counter(0);
        public unsafe Role.Instance.Inventory Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (ITEM_ID >= 4200012 && ITEM_ID <= 4200018)
            {
                int Count = client.Inventory.GetCountItem(ITEM_ID);
            }
            if (MaximDurability == 0)
            {
                Database.ItemType.DBItem DBItem;
                if (Pool.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
                    MaximDurability = DBItem.Durability;
            }
            ushort position = Database.ItemType.ItemPosition(ITEM_ID);
            if (position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
            {
                Activate = 1;
                TimeLeftInMinutes = uint.MaxValue;
            }
            //ItemSafe(position);
            if (Plus > 0)
            {
                if (position == 0)
                    Plus = 0;
            }
            if (ITEM_ID >= 730001 && ITEM_ID <= 730008)
                Plus = (byte)(ITEM_ID % 10);
            Database.ItemType.DBItem DBItems;

            if (Pool.ItemsBase.TryGetValue(ITEM_ID, out DBItems))
                RuneView = DBItems.RuneView;

            client.Send(ItemCreate(stream, this));
            SendItemExtra(client, stream);
            SendItemLocked(client, stream);

            return client.Inventory;
        }
        public void ItemSafe(ushort Position)
        {
            switch ((Role.Flags.ConquerItem)Position)
            {
                case Role.Flags.ConquerItem.Bottle:
                case Role.Flags.ConquerItem.SteedMount:
                case Role.Flags.ConquerItem.RightWeaponAccessory:
                case Role.Flags.ConquerItem.LeftWeaponAccessory:
                case Role.Flags.ConquerItem.Garment:
                case Role.Flags.ConquerItem.AleternanteRelics:
                case Role.Flags.ConquerItem.AleternanteBottle:
                case Role.Flags.ConquerItem.AleternanteGarment:
                    {
                        Plus = 0;
                        SocketOne = 0;
                        SocketTwo = 0;
                        if (Bless > 1)
                        {
                            Bless = 1;
                        }
                        Enchant = 0;
                        AnimaItemID = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        PerfectionLevel = 0;
                        break;
                    }
                case Role.Flags.ConquerItem.Relic:
                    {
                        Plus = 0;
                        SocketOne = 0;
                        SocketTwo = 0;
                        Enchant = 0;
                        Bless = 0;
                        AnimaItemID = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        PerfectionLevel = 0;
                        break;

                    }
                case Role.Flags.ConquerItem.RedRune:
                case Role.Flags.ConquerItem.BlueRune:
                case Role.Flags.ConquerItem.YellowRune:
                case Role.Flags.ConquerItem.AlternateRedRune:
                case Role.Flags.ConquerItem.AlternateBlueRune:
                case Role.Flags.ConquerItem.AlternateYellowRune:
                case Role.Flags.ConquerItem.RuneBag:
                case Role.Flags.ConquerItem.RunesCollection:
                    {
                        Plus = 0;
                        SocketOne = 0;
                        SocketTwo = 0;
                        Enchant = 0;
                        AnimaItemID = 0;
                        Bless = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        PerfectionLevel = 0;
                        break;
                    }
                case Role.Flags.ConquerItem.Steed:
                case Role.Flags.ConquerItem.RidingCrop:
                    {
                        if (Plus > 12)
                            Plus = 12;
                        if (Bless > 1)
                            Bless = 1;
                        AnimaItemID = 0;
                        SocketOne = 0;
                        SocketTwo = 0;
                        Enchant = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        break;
                    }

                case Role.Flags.ConquerItem.Fan:
                    {
                        if (Plus > 12)
                            Plus = 12;
                        if (Bless > 1)
                            Bless = 1;
                        AnimaItemID = 0;
                        Enchant = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        if (SocketOne != Role.Flags.Gem.NoSocket && SocketOne != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketOne >= Role.Flags.Gem.NormalThunderGem && SocketOne <= Role.Flags.Gem.SuperThunderGem))
                                SocketOne = 0;
                        }
                        if (SocketTwo != Role.Flags.Gem.NoSocket && SocketTwo != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketTwo >= Role.Flags.Gem.NormalThunderGem && SocketTwo <= Role.Flags.Gem.SuperThunderGem))
                                SocketTwo = 0;
                        }
                        break;
                    }
                case Role.Flags.ConquerItem.Tower:
                    {
                        if (Plus > 12)
                            Plus = 12;
                        if (Bless > 1)
                            Bless = 1;
                        AnimaItemID = 0;
                        Enchant = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        if (SocketOne != Role.Flags.Gem.NoSocket && SocketOne != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketOne >= Role.Flags.Gem.NormalGloryGem && SocketOne <= Role.Flags.Gem.SuperGloryGem))
                                SocketOne = 0;
                        }
                        if (SocketTwo != Role.Flags.Gem.NoSocket && SocketTwo != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketTwo >= Role.Flags.Gem.NormalGloryGem && SocketTwo <= Role.Flags.Gem.SuperGloryGem))
                                SocketTwo = 0;
                        }
                        break;
                    }
                case Role.Flags.ConquerItem.Wing:
                    {
                        if (Plus > 12)
                            Plus = 12;
                        if (Bless > 1)
                            Bless = 1;
                        AnimaItemID = 0;
                        Enchant = 0;
                        MythSoulID = 0;
                        MythSoulProgress = 0;
                        UpsurgeID = 0;
                        if (SocketOne != Role.Flags.Gem.NoSocket && SocketOne != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketOne >= Role.Flags.Gem.NormalThunderGem && SocketOne <= Role.Flags.Gem.SuperThunderGem))
                                SocketOne = 0;
                        }
                        if (SocketTwo != Role.Flags.Gem.NoSocket && SocketTwo != Role.Flags.Gem.EmptySocket)
                        {
                            if (!(SocketTwo >= Role.Flags.Gem.NormalGloryGem && SocketTwo <= Role.Flags.Gem.SuperGloryGem))
                                SocketTwo = 0;
                        }
                        break;
                    }
            }
        }
        public void SendItemExtra(Client.GameClient client, ServerSockets.Packet stream)
        {

            if (Purification.ItemUID != 0 || Refinary.ItemUID != 0)
            {
                MsgItemExtra extra = new MsgItemExtra();
                if (Purification.InLife)
                {
                    if (Purification.SecoundsLeft == 0)
                        Purification.Typ = MsgItemExtra.Typing.Stabilization;
                    else
                        Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                    extra.Purifications.Add(Purification);
                }
                if (Refinary.InLife)
                {
                    Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                    if (Refinary.EffectDuration == 0)
                        Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                    extra.Refinerys.Add(Refinary);
                }
                client.Send(extra.CreateArray(stream));
            }

        }
        public void SendItemLocked(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Locked == 2)
            {
                if (client.Player.OnMyOwnServer)
                {
                    if (UnLockTimer == 0)
                    {
                        Locked = 0;
                        Mode = Role.Flags.ItemMode.Update;
                        client.Send(ItemCreate(stream, this));
                    }
                    else
                    {
                        if (DateTime.Now > Role.Core.GetTimer(UnLockTimer))
                        {
                            Locked = 0;
                            Mode = Role.Flags.ItemMode.Update;
                            client.Send(ItemCreate(stream, this));
                        }
                        else
                        {
                            client.Send(stream.ItemLockCreate(UID, MsgItemLock.TypeLock.UnlockDate, 0, (uint)UnLockTimer));
                        }
                    }
                }
            }
        }
        public DateTime TimeStamp;
        public bool Delete { get; set; }
        public ServerSockets.Packet ItemCreate(ServerSockets.Packet stream, MsgGameItem item)
        {
            stream.InitWriter();
            var Info = new MsgItemInfoProto()
            {
                UID = item.UID,
                ITEM_ID = item.ITEM_ID,
                Durability = item.Durability,
                MaximDurability = item.MaximDurability,
                Mode = (ulong)item.Mode,
                Position = (ulong)item.Position,
                SocketProgress = (ulong)item.SocketProgress,
                SocketOne = (ulong)item.SocketOne,
                SocketTwo = (ulong)item.SocketTwo,
                SpellID = (ulong)item.SpellID,
                RuneView = (ulong)item.RuneView,
                Plus = (ulong)item.Plus,
                Bless = (ulong)item.Bless,
                Bound = (ulong)item.Bound,
                Enchant = (ulong)item.Enchant,
                ProgresGreen = (ulong)item.ProgresGreen,
                Suspicious = (ulong)item.Suspicious,
                Locked = (ulong)item.Locked,
                TimeLeftInMinutes = (ulong)item.TimeLeftInMinutes,
                StackSize = (ulong)item.StackSize,
                PerfectionLevel = (ulong)item.PerfectionLevel,
                PerfectionProgress = (ulong)item.PerfectionProgress,
                OwnerUID = (ulong)item.OwnerUID,
                OwnerName = item.OwnerName,
                Signature = item.Signature,
                RuneEXP = (ulong)item.RuneEXP,
                AnimaItemID = item.AnimaItemID,
                MythsoulID = item.MythSoulID,
                MythsoulEffect = item.MythSoulENERGY,
                MythsoulEXP = item.MythSoulProgress,
                MythsoulIDNumber2 = item.Mutacion,
                MythsoulIDNumber3 = item.UpsurgeID,
                SpecialQuenchID = item.SpecialQuenchID,
            };
            if (!Database.ItemType.Relic.Contains(item.ITEM_ID))
            {
                if (item.Activate == 1 && item.TimeLeftInMinutes > 0 && item.TimeLeftInMinutes < uint.MaxValue)
                {
                    TimeSpan timeSpan = item.TimeStamp - DateTime.Now;
                    Info.RemainingTime = (uint)(timeSpan.TotalSeconds);
                }
                else
                    Info.RemainingTime = item.TimeLeftInMinutes;
            }
            else
                Info.RemainingTime = 0;
            if (!Database.ItemType.isRune(item.ITEM_ID))
                Info.PlusProgress = item.PlusProgress;
            else
                Info.PlusProgress = 0;
            if (Database.ItemType.isRune(item.ITEM_ID))
            {
                Info.RuneEXP = item.RuneEXP;
                // Unseal points travel in RelicAttributes[0] (field 31) and the flags in field 39.
                Info.RuneUnsealState = Role.Instance.RuneUnseal.GetFlags(item);
            }
            else
                Info.RuneEXP = 0;
            if (item.RelicAttributes != null)
            {
                Info.RelicAttributes = new ulong[item.RelicAttributes.Length];
                for (int i = 0; i < item.RelicAttributes.Length; i++)
                    Info.RelicAttributes[i] = item.RelicAttributes[i];
            }
            else
            {
                Info.RelicAttributes = new ulong[5];
                Info.RelicAttributes[0] = 0;
                Info.RelicAttributes[1] = 0;
                Info.RelicAttributes[2] = 0;
                Info.RelicAttributes[3] = 0;
                Info.RelicAttributes[4] = 0;
            }

            if (Database.ItemType.EonspiritItem.Contains(item.ITEM_ID))
            {
                Info.EonspiritActived = item.EonspiritActived;
                Info.EonspiritTime = item.EonspiritTime;
            }
            else if (Qahr.Mobile.MY_System.Relic.MainRelic.IsResonancePosition((ushort)Info.Position))
            {
                // رقم خانة الـ Resonance لازم يوصل للعميل، وإلا مايعرفش يرسم العنصر في خانته.
                Info.EonspiritActived = item.EonspiritActived;
            }
            else
            {
                Info.EonspiritActived = 0;
                Info.EonspiritTime = 0;
            }

            stream.ProtoBufferSerialize(Info);
            stream.Finalize(GamePackets.MsgItemInfo);
            return stream;
        }
        public uint EonspiritPercentage;//1 = 100% ^_^ 0 = 0%
        public uint EonspiritExp;//1 exp = 100
        public uint EonspiritActived;//1 = Active ^_^ 0 = NotActive
        public uint EonspiritTime;//Just for Level 9 when you failed to update level 10 you got value of level 10 but for sometimes
        public int AmazingSpeedData1 = 0;
        public int AmazingSpeedPower = 0;
        public int AmazingSpeedData = 0;


        public int SwordBodyData0 = 0;
        public int SwordBodyData1 = 0;
        public int SwordBodyPower = 0;


        public int BenefitShowerData = 0;
        public int BenefitShowerPower = 0;
        public uint RelicResonance;

        public bool IsWeapon
        {
            get
            {
                return (Database.ItemType.ItemPosition(ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeapon
                    || Database.ItemType.ItemPosition(ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon) && !Database.ItemType.IsArrow(ITEM_ID);
            }
        }
        public bool IsEquip
        {
            get
            {
                return Database.ItemType.ItemPosition(ITEM_ID) != 0;
            }
        }
        public uint ItemPoints;
        public int GetPerfectionPosition
        {
            get
            {
                ushort DBPosotion = Database.ItemType.ItemPosition(ITEM_ID);
                if (DBPosotion <= 5)
                    return DBPosotion;
                if (Database.ItemType.IsShield(ITEM_ID))
                    return 4;
                switch (DBPosotion)
                {
                    case (ushort)Role.Flags.ConquerItem.Wing:
                        return 5;
                    case (ushort)Role.Flags.ConquerItem.Ring:
                        return 6;
                    case (ushort)Role.Flags.ConquerItem.RidingCrop:
                        return 7;
                    case (ushort)Role.Flags.ConquerItem.Boots:
                        return 8;
                    case (ushort)Role.Flags.ConquerItem.Steed:
                        return 9;
                    case (ushort)Role.Flags.ConquerItem.Fan:
                        return 10;
                    case (ushort)Role.Flags.ConquerItem.Tower:
                        return 11;
                }
                return -1;

            }
        }
        public uint PerfectionStage
        {
            get
            {
                return PerfectionLevel / 9;
            }
        }
        public uint PerfectionStageStars
        {
            get
            {
                return PerfectionLevel % 9;
            }
        }
        public uint GetPrestigeScore
        {
            get
            {
                return ItemPoints;
            }
        }
        public bool IsTwoHander()
        {
            uint item_type = ITEM_ID / 1000;
            bool check = ((UInt16)item_type >= 500 && (UInt16)item_type <= 580);
            if (check)
            {
                check = (item_type != 900);
                if (check)
                {
                    check = (item_type != 601);
                    if (check)
                    {
                        check = (item_type != 610);
                    }
                }
            }
            return check;
        }
        public uint SpecialQuenchID;
        public ushort Leng;
        public ushort PacketID;
        public bool Equipped;
        public uint UID;
        private uint _id;
        public uint ITEM_ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                if (Database.ItemType.isRune(value))
                {
                    if (Pool.ItemsBase[value].RuneSpellID == value)
                    {
                        SpellID = Pool.ItemsBase[value].RuneSpellID - Pool.ItemsBase[value].RuneSpellID % 100 + 1;
                        Effect = Role.Flags.ItemEffect.RuneEffect;
                    }
                    else
                    {
                        SpellID = Pool.ItemsBase[value].RuneSpellID;
                        Effect = Role.Flags.ItemEffect.None;
                    }
                }

            }
        }
        public ushort Durability;
        public ushort MaximDurability;
        public Role.Flags.ItemMode Mode;
        public ushort Position;
        public uint SocketProgress;
        public uint RemainingTime2;
        public uint RemainingTime;
        public Role.Flags.Gem SocketOne;
        public Role.Flags.Gem SocketTwo;
        public uint SpellID;
        public Role.Flags.ItemEffect Effect;
        public byte Plus;
        public byte Bless;
        public byte Bound;
        public byte Enchant;//36 // Steed  -> ProgresBlue 
        public uint ProgresGreen;//39 // for steed
        public byte Suspicious;
        public byte Locked;
        public Role.Flags.Color Color;
        public uint PlusProgress;//52
        public ushort Inscribed;
        public uint Activate;
        public uint TimeLeftInMinutes;//64
        public ushort StackSize;//68
        public ushort UnKnow;
        public uint WH_ID;
        public uint YuanshenIndex;
        public ulong YuanshenTime;
        public uint OwnerUID = 0;
        public uint PerfectionLevel = 0;
        public ushort PerfectionRank = 0;
        public uint PerfectionProgress = 0;
        public string OwnerName = "";
        public string Signature = "";
        public DateTime EndDate = DateTime.FromBinary(0);
        public uint RuneEXP;
        public Role.Instance.RelicAttribute[] RelicAttributes = new Role.Instance.RelicAttribute[5];
        public uint AnimaItemID;
        public uint MythSoulID;
        public uint MythSoulProgress;
        public uint MythSoulENERGY;
        public uint MutacionID2;
        public uint Mutacion;
        public uint Mutacion2;
        public uint UpsurgeID2;
        public uint UpsurgeID;
        public int UnLockTimer;
        public byte RuneView;
        public Dictionary<uint, string> Agate_map { get; set; }
        public MsgGameItem()
        {
            this.Agate_map = new Dictionary<uint, string>(10);
        }

        public MsgGameItem FakeChatItem(uint ItemID, ushort Durability)
        {
            Game.MsgServer.MsgGameItem GameItem = new Game.MsgServer.MsgGameItem();
            GameItem.UID = 1;
            GameItem.Durability = GameItem.MaximDurability = Durability;
            GameItem.ITEM_ID = ItemID;
            GameItem.Mode = Role.Flags.ItemMode.ChatItem;
            return GameItem;
        }
        public void SendAgate(Client.GameClient client)
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = rec.GetStream();
                stream.InitWriter();
                stream.Write(0);
                stream.Write(this.UID);
                stream.Write((uint)this.Agate_map.Count);
                stream.Write((ulong)this.Agate_map.Count);
                stream.Write((uint)this.Durability);
                stream.Write((uint)this.Agate_map.Count);
                if (this.Agate_map.Count > 0)
                {
                    for (uint key = 0; (long)key < (long)this.Agate_map.Count; ++key)
                    {
                        stream.Write(key);
                        stream.Write(uint.Parse(this.Agate_map[key].Split(new char[1]
            {
              '~'
            })[0].ToString()));
                        stream.Write(uint.Parse(this.Agate_map[key].Split(new char[1]
            {
              '~'
            })[1].ToString()));
                        stream.Write(uint.Parse(this.Agate_map[key].Split(new char[1]
            {
              '~'
            })[2].ToString()));
                        stream.ZeroFill(64);// QahrMobile: a CMsgSuperFlag record is 80 bytes (16 + a 64-byte description at 0x30), was 32
                    }
                }
                stream.Finalize(GamePackets.MsgSuperFlag);
                client.Send(stream);
            }
        }


    }
}
