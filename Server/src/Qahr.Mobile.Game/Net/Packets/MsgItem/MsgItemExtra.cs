using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetItemExtra(this ServerSockets.Packet stream, out  uint Mode, out uint ItemUID, out   uint ItemArtfact)
        {
            Mode = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
            ItemArtfact = stream.ReadUInt32();
        }

        public static void GetExtraItem(this ServerSockets.Packet stream, out MsgItemExtra.Purification purification, out MsgItemExtra.Refinery refinary)
        {
            purification = new MsgItemExtra.Purification();
            refinary = new MsgItemExtra.Refinery();

            uint count = stream.ReadUInt32();
            for (int x = 0; x < count; x++)
            {
                uint UID = stream.ReadUInt32();
                MsgItemExtra.Typing type = (MsgItemExtra.Typing)stream.ReadUInt32();
                if (type <= MsgItemExtra.Typing.StabilizationEffectRefined)
                {

                    refinary.ItemUID = UID;
                    refinary.Typ = type;
                    refinary.EffectID = stream.ReadUInt32();
                    refinary.EffectLevel = stream.ReadUInt32();
                    refinary.EffectPercent = stream.ReadUInt32();
                    refinary.EffectPercent2 = stream.ReadUInt32();
                    stream.ReadUInt32();//secounds left
                    refinary.EffectDuration = 0;
                    refinary.AddedOn = DateTime.Now;
                    refinary.Available = true;

                }
                else
                {
                    purification.ItemUID = UID;
                    purification.Typ = type;
                    purification.PurificationItemID = stream.ReadUInt32();
                    purification.PurificationLevel = stream.ReadUInt32();
                    purification.EffectPercent2 = stream.ReadUInt32();
                    purification.PurificationDuration = stream.ReadUInt32();
                    stream.ReadUInt32();
                    stream.ReadUInt32();
                    purification.AddedOn = DateTime.Now;
                }
            }
        }
    }

    public class MsgItemExtra
    {
        public class Artefacts
        {
            public const byte Refinary = 0,
                Purification = 1;
        }
        public enum Typing : uint
        {
            RefinaryAdding = 2,
            PermanentRefinery = 3,
            StabilizationEffectRefined = 4,
            PurificationEffect = 5,
            PurificationAdding = 6,
            ExpireTime = 7,
            Stabilization = 8,
            StabilizationEffectPurification = 9
        }

        public struct Purification
        {
            public static Purification ShallowCopy(Purification item)
            {
                return (Purification)item.MemberwiseClone();
            }
            public uint ItemUID;
            public Typing Typ;
            public uint PurificationItemID;
            public uint PurificationLevel;
            public uint EffectPercent2;
            /// <summary>
            /// In minutes.
            /// </summary>
            public uint PurificationDuration;

            public DateTime AddedOn;
            public bool Available;
            public int SecoundsLeft
            {
                get
                {
                    if (PurificationDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(AddedOn.AddSeconds(PurificationDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft > 0)
                        {
                            Available = true;
                            return secondsleft;
                        }
                        else
                            Available = false;
                    }
                    else if (PurificationItemID > 1000)
                        Available = true;

                    return 0;
                }
            }
            public bool InLife
            {
                get
                {

                    int secount = SecoundsLeft;//to create time
                    return Available;
                }
            }
        }

        public struct Refinery
        {
            public static Refinery ShallowCopy(Refinery item)
            {
                return (Refinery)item.MemberwiseClone();
            }
            public uint ItemUID;

            public Typing Typ;
            public uint EffectID;
            public uint EffectLevel;
            public uint EffectPercent;
            public uint EffectPercent2;
            /// <summary>
            /// In minutes.
            /// </summary>
            public uint EffectDuration;
            public DateTime AddedOn;
            public bool Available;

            public int SecoundsLeft
            {
                get
                {
                    if (EffectDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(AddedOn.AddSeconds(EffectDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft > 0)
                        {
                            Available = true;
                            return secondsleft;
                        }
                        else
                            Available = false;
                    }
                    else if (EffectID > 1000)
                        Available = true;

                    return 0;
                }
            }
            public bool InLife
            {
                get
                {

                    int secount = SecoundsLeft;//verified create time
                    return Available;
                }
            }
        }
        public List<Purification> Purifications;
        public List<Refinery> Refinerys;

        public MsgItemExtra()
        {
            Purifications = new List<Purification>();
            Refinerys = new List<Refinery>();

        }

        public unsafe ServerSockets.Packet CreateArray(ServerSockets.Packet stream, bool tointer = false)
        {

            stream.InitWriter();

            stream.Write((uint)(Refinerys.Count + Purifications.Count));

            foreach (var item in Purifications)
            {
                stream.Write(item.ItemUID);//0
                stream.Write((uint)item.Typ);//4
                stream.Write(item.PurificationItemID);//8
                stream.Write(item.PurificationLevel);//12
                stream.Write(item.EffectPercent2);//16
                stream.Write(item.PurificationDuration);//20
                stream.Write(item.SecoundsLeft);//24

                stream.Write((uint)0);//unknow
            }
            foreach (var item in Refinerys)
            {
                stream.Write(item.ItemUID);//0
                stream.Write((uint)item.Typ);//4
                if (tointer)
                    stream.Write(item.EffectID);
                else
                {
                    if (item.EffectID >= 724440 && item.EffectID <= 724444 || item.EffectID == 3004140 || item.EffectID >= 3006165 && item.EffectID <= 3006170)
                        stream.Write((uint)301);
                    else
                        stream.Write(item.EffectID);//8
                }
                stream.Write(item.EffectLevel);//12
                stream.Write(item.EffectPercent);//16
                stream.Write(item.EffectPercent2);//20
                stream.Write(item.SecoundsLeft);//24
                // QahrMobile: CMsgItemStatus::Process steps 32 bytes per entry, like the purification entries above
                if (!tointer)
                    stream.Write((uint)0);//28
            }

            stream.Finalize(GamePackets.MsgItemStatus);

            return stream;
        }

        [PacketAttribute(GamePackets.MsgSolidify)]
        public unsafe static void Stabilization(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint Mode;
            uint ItemUID;
            uint Count;

            packet.GetItemExtra(out Mode, out ItemUID, out Count);

            switch (Mode)
            {
                case Artefacts.Refinary:
                    {
                        Game.MsgServer.MsgGameItem Item;
                        if (client.TryGetItem(ItemUID, out Item))
                        {
                            if (!Item.Refinary.InLife) return;
                            if (Item.Refinary.EffectID == 0) return;

                            uint procent = 0;
                            Queue<Game.MsgServer.MsgGameItem> items = new Queue<Game.MsgServer.MsgGameItem>();
                            for (ushort count = 0; count < Count; count++)
                            {
                                uint stoneUID = packet.ReadUInt32();

                                Game.MsgServer.MsgGameItem stones;
                                if (client.Inventory.TryGetItem(stoneUID, out stones))
                                {
                                    if (stones.ITEM_ID == 723694)
                                        procent += 10;
                                    if (stones.ITEM_ID == 723695)
                                        procent += 100;
                                    items.Enqueue(stones);
                                }
                            }

                            if (procent >= Database.ItemType.RefineryStabilizationPoints((byte)Item.Refinary.EffectLevel))
                            {
                                Item.Refinary.EffectDuration = 0;
                                Item.Refinary.Typ = Typing.StabilizationEffectRefined;

                                MsgItemExtra effect = new MsgItemExtra();
                                effect.Refinerys.Add(Item.Refinary);
                                client.Send(effect.CreateArray(packet));

                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);

                                uint count_remover = (uint)items.Count;
                                for (byte x = 0; x < count_remover; x++)
                                    client.Inventory.Update(items.Dequeue(), Role.Instance.AddMode.REMOVE, packet);

                            }
                        }
                        break;
                    }
                case Artefacts.Purification:
                    {

                        Game.MsgServer.MsgGameItem Item;
                        if (client.TryGetItem(ItemUID, out Item))
                        {
                            if (!Item.Purification.InLife) return;
                            if (Item.Purification.PurificationItemID == 0) return;

                            uint procent = 0;
                            Queue<Game.MsgServer.MsgGameItem> items = new Queue<Game.MsgServer.MsgGameItem>();
                            for (ushort count = 0; count < Count; count++)
                            {
                                uint stoneUID = packet.ReadUInt32();

                                Game.MsgServer.MsgGameItem stones;
                                if (client.Inventory.TryGetItem(stoneUID, out stones))
                                {
                                    if (stones.ITEM_ID == 723694)
                                        procent += 10;
                                    if (stones.ITEM_ID == 723695)
                                        procent += 100;
                                    items.Enqueue(stones);
                                }
                            }

                            if (procent >= Database.ItemType.PurifyStabilizationPoints((byte)Item.Purification.PurificationLevel) || procent / 100 == Count)
                            {
                                Item.Purification.PurificationDuration = 0;
                                Item.Purification.Typ = Typing.StabilizationEffectPurification;

                                MsgItemExtra effect = new MsgItemExtra();
                                effect.Purifications.Add(Item.Purification);
                                client.Send(effect.CreateArray(packet));

                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);

                                uint count_remover = (uint)items.Count;
                                for (byte x = 0; x < count_remover; x++)
                                    client.Inventory.Update(items.Dequeue(), Role.Instance.AddMode.REMOVE, packet);

                            }

                        }
                        break;
                    }
            }
        }

        [PacketAttribute(GamePackets.MsgQuench)]
        public unsafe static void MsgQuench(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint Mode = packet.ReadUInt32();
            uint ItemUID = packet.ReadUInt32();
            uint ItemArtfact = packet.ReadUInt32();
            if (ItemUID == ItemArtfact)
                return;
            switch (Mode)
            {
                case Artefacts.Refinary:
                    {
                        Game.MsgServer.MsgGameItem Item, Artefact;
                        if (client.TryGetItem(ItemUID, out Item) && client.Inventory.TryGetItem(ItemArtfact, out Artefact))
                        {
                            if (Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Fan
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Tower
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RidingCrop
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Relics)
                                return;

                            Database.Rifinery.Item BaseAddingItem;
                            if (Pool.RifineryItems.TryGetValue(Artefact.ITEM_ID, out BaseAddingItem))
                            {

                                if (Database.ItemType.ItemPosition(Item.ITEM_ID) != BaseAddingItem.ForItemPosition && (Database.ItemType.ItemPosition(Item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.LeftWeapon && BaseAddingItem.ForItemPosition != (ushort)Role.Flags.ConquerItem.RightWeapon))
                                    return;
                                if (BaseAddingItem.Name.ToLower() == "2-handed")
                                {
                                    if (!Database.ItemType.IsTwoHand(Item.ITEM_ID))
                                        return;
                                }
                                Item.Refinary = new Refinery();
                                Item.Refinary.AddedOn = DateTime.Now;
                                Item.Refinary.Available = true;
                                Item.Refinary.ItemUID = ItemUID;
                                Item.Refinary.EffectLevel = BaseAddingItem.Level;
                                int days = 7;
                                switch (client.Player.VipLevel)
                                {
                                    case 1: days = 8; break;
                                    case 2: days = 9; break;
                                    case 3: days = 11; break;
                                    default:
                                        days = 15;
                                        break;
                                }

                                Item.Refinary.EffectDuration = (uint)(days * 24 * 60 * 60);
                                Item.Refinary.EffectID = Artefact.ITEM_ID;
                                Item.Refinary.EffectPercent = BaseAddingItem.Procent;
                                Item.Refinary.EffectPercent2 = BaseAddingItem.Procent2;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);
                                //if (BaseAddingItem.Type2 == Database.Rifinery.RefineryType.None)
                                //   client.SendSysMesage("Successfully refined. Received refinery status: "+BaseAddingItem.Type.ToString() +" increases by "+BaseAddingItem.Procent+"% for 8 days.");
                                //Successfully refined. Received refinery status: Block increases by 11%% for 8 days.

                                client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                client.Inventory.Update(Artefact, Role.Instance.AddMode.REMOVE, packet);
                            }
                        }
                        break;
                    }
                case Artefacts.Purification:
                    {
                        Game.MsgServer.MsgGameItem Item, Artefact;
                        if (client.TryGetItem(ItemUID, out Item) && client.Inventory.TryGetItem(ItemArtfact, out Artefact))
                        {
                            if (Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Fan
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Tower
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RidingCrop
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Relics)
                                return;

                            var PurifyInformation = Pool.ItemsBase[Artefact.ITEM_ID];
                            var ItemInformation = Pool.ItemsBase[Item.ITEM_ID];
                            if (PurifyInformation.PurificationLevel > 0)
                            {
                                //       if (ItemInformation.Level >= PurifyInformation.Level)
                                {
                                    if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                                    {
                                        if (client.Inventory.CheckMeteors((byte)PurifyInformation.PurificationMeteorNeed, false, packet))
                                        {
                                            client.Inventory.CheckMeteors((byte)PurifyInformation.PurificationMeteorNeed, true, packet);

                                            Item.Purification = new Purification();
                                            Item.Purification.AddedOn = DateTime.Now;
                                            Item.Purification.ItemUID = ItemUID;
                                            Item.Purification.PurificationLevel = PurifyInformation.PurificationLevel;
                                            Item.Purification.PurificationDuration = 7 * 24 * 60 * 60;
                                            Item.Purification.PurificationItemID = Artefact.ITEM_ID;
                                            Item.Purification.Typ = Typing.PurificationEffect;

                                            MsgItemExtra effect = new MsgItemExtra();
                                            effect.Purifications.Add(Item.Purification);
                                            client.Send(effect.CreateArray(packet));

                                            Item.Mode = Role.Flags.ItemMode.Update;
                                            Item.Send(client, packet);

                                            client.Inventory.Update(Artefact, Role.Instance.AddMode.REMOVE, packet);
                                            if (Item.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

            }
        }
    }
}
