using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Database;
namespace Qahr.Mobile.Role.Instance
{
    public class Equip
    {
        public int Rank, Chance;
        public string Name;
        public uint ID;
        public byte Color;
        public byte Sockets;
        public byte Plus;
        public override string ToString()
        {
            return Rank + " " + Chance + " " + Name + " " + ID + " " + Color + " " + Sockets + " " + Plus;
        }

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
        public uint SoulsPotency = 0;

        public int WeaponsMinAttack = 0;
        public uint ArmorID;
        public bool CreateSpawn = true;
        public bool SuperArmor = false;
        public bool FullSuper
        {
            get
            {
                if (!SuperArmor)
                    return false;
                foreach (var item in CurentEquip)
                {
                    if (item.Position != (ushort)Role.Flags.ConquerItem.Steed
                        && item.Position != (ushort)Role.Flags.ConquerItem.Garment
                        && item.Position != (ushort)Role.Flags.ConquerItem.Bottle
                        && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
                        && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
                        && item.Position != (ushort)Role.Flags.ConquerItem.SteedMount
                        && item.Position != (ushort)Role.Flags.ConquerItem.Relics
                                                && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteRelics

                        && item.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                        && item.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
                    {
                        if (item.ITEM_ID % 10 != 9)
                            return false;
                    }
                }
                return true;
            }
        }

        public bool CanUpdatePerfectionItem(MsgGameItem item)
        {
            int count = CurentEquip.Where(p => p.PerfectionLevel == item.PerfectionLevel).Count();
            uint ItemsC = (uint)(Database.ItemType.IsTwoHand(RightWeapon) ? 0 : 1);
            if (Database.ItemType.IsShield(LeftWeapon))
                ItemsC = 1;
            if (count == 11 + ItemsC)
                return true;
            uint bigLevel = 0;
            foreach (var _item in CurentEquip)
                if (_item.PerfectionLevel > bigLevel)
                    bigLevel = _item.PerfectionLevel;
            return bigLevel >= item.PerfectionLevel;

        }

        public Role.Flags.ItemEffect RightWeaponEffect = Flags.ItemEffect.None;
        public Role.Flags.ItemEffect RingEffect = Flags.ItemEffect.None;
        public Role.Flags.ItemEffect NecklaceEffect = Flags.ItemEffect.None;

        public bool UseMonkEpicWeapon = false;
        public uint ShieldID = 0;
        public uint RidingCrop = 0;
        public uint HeadID;
        public uint RightWeapon = 0;
        public uint LeftWeapon = 0;
        public byte SteedPlus { get { return (byte)Owner.Player.SteedPlus; } }
        public uint SteedPlusPorgres = 0;

        public int rangeR = 0;
        public int rangeL = 0;
        public int SizeAdd = 0;

        public int SpeedR = 0;
        public int SpeedL = 0;
        public int SpeedRing = 0;

        public bool SuperDragonGem = false;
        public bool SuperPheonixGem = false;
        public bool SuperVioletGem = false;
        public bool SuperRaibowGem = false;
        public bool SuperMoonGem = false;
        public bool SuprtTortoiseGem = false;
        public bool HaveBless = false;

        public int AttackSpeed(bool physical)
        {
            // [AttackSpeed] data-driven tuning (Infinity[DB]\attackspeed.txt). OFF by default -> the old code below runs.
            var atkCfg = Qahr.Mobile.Combat.AttackSpeedConfig.Config;
            if (atkCfg != null && atkCfg.Enabled)
                return atkCfg.Compute(Owner.Player.Class, RightWeapon, LeftWeapon, SpeedR, SpeedL, SpeedRing,
                    Owner.Player.Agility,
                    Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone),
                    Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CelestialDance),
                    Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Spreadyourwings));

            int MS_Delay = 200;
            MS_Delay = Math.Max(200, MS_Delay - 100);
            MS_Delay = Math.Max(200, MS_Delay - SpeedR);
            MS_Delay = Math.Max(200, MS_Delay - SpeedL);
            MS_Delay = Math.Max(200, MS_Delay - SpeedRing);
            MS_Delay = Math.Max(200, MS_Delay - Owner.Player.Agility / 2);
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                MS_Delay = Math.Max(200, MS_Delay - 150);
            MS_Delay = Math.Max(200, MS_Delay);
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CelestialDance))
                MS_Delay = 110;
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Spreadyourwings))
                MS_Delay = 100;
            return MS_Delay;
        }

        public int GetAttackRange(int targetSizeAdd)
        {
            var range = 1;

            if (rangeR != 0 && rangeL != 0)
                range = (rangeR + rangeL) / 2;
            else if (rangeR != 0)
                range = rangeR;
            else if (rangeL != 0)
                range = rangeL;

            range += (SizeAdd + targetSizeAdd + 1) / 2;

            return range;
        }

        public bool Alternante = false;

        private Client.GameClient Owner;

        public Equip(Client.GameClient client)
        {
            Owner = client;
        }

        public Game.MsgServer.MsgGameItem[] CurentEquip = new Game.MsgServer.MsgGameItem[0];

        public bool UseEpicTrojanWeapon()
        {
            foreach (var item in CurentEquip)
            {
                if (item != null)
                {
                    if (Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                        return true;
                }
            }
            return false;
        }

        public bool UseEpicNinjaWeapon()
        {
            foreach (var item in CurentEquip)
            {
                if (item != null)
                {
                    if (Database.ItemType.IsNinjaEpicWeapon(item.ITEM_ID))
                        return true;
                }
            }
            return false;
        }

        public uint GetAnima()
        {
            uint Points = 0;

            return Points;
        }

        public unsafe bool Add(ServerSockets.Packet stream, uint ID, Role.Flags.ConquerItem position, byte plus = 0, byte bless = 0, byte Enchant = 0, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None)
        {
            if (FreeEquip(position))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                    ItemDat.Effect = Effect;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    CheakUp(ItemDat);
                    ItemDat.Position = (ushort)position;
                    ItemDat.Mode = Flags.ItemMode.AddItem;

                    ItemDat.Send(Owner, stream);


                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0, 0));

                    return true;

                }
            }
            return false;
        }

        public unsafe bool AddBot(ServerSockets.Packet stream, uint ID, Role.Flags.ConquerItem position, byte plus = 0, byte bless = 0, byte Enchant = 0, Role.Flags.Gem sockone = Flags.Gem.NoSocket, Role.Flags.Gem socktwo = Flags.Gem.NoSocket, uint SoulID = 0, uint purfylevel = 0)
        {
            if (FreeEquip(position))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;

                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);

                    CheakUp(ItemDat);
                    ItemDat.Position = (ushort)position;
                    ItemDat.Mode = Flags.ItemMode.AddItem;
                    ItemDat.Purification.PurificationLevel = purfylevel;
                    ItemDat.Purification.PurificationDuration = 0;
                    ItemDat.Purification.PurificationItemID = SoulID;
                    ItemDat.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                    MsgItemExtra extra = new MsgItemExtra
                    {
                        Purifications = { ItemDat.Purification }
                    };
                    ItemDat.Send(Owner, stream);


                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0, 0));

                    return true;

                }
            }
            return false;
        }

        public unsafe bool AddSteed(ServerSockets.Packet stream, uint ID, Role.Flags.ConquerItem position, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (FreeEquip(position))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;

                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.ProgresGreen = ProgresGreen;
                    ItemDat.Enchant = ProgresBlue;
                    ItemDat.Bless = ProgresRed;
                    ItemDat.StackSize = 1;
                    ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    CheakUp(ItemDat);
                    ItemDat.Position = (ushort)position;
                    ItemDat.Mode = Flags.ItemMode.AddItem;

                    ItemDat.Send(Owner, stream);


                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0, 0));

                    return true;

                }
            }
            return false;
        }

        public unsafe bool Garment(ServerSockets.Packet stream, uint ID)
        {

            Database.ItemType.DBItem DbItem;
            if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Pool.ITEM_Counter.Next;

                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                ItemDat.StackSize = 1;
                ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                CheakUp(ItemDat);
                ItemDat.Position = (ushort)Role.Flags.ConquerItem.Garment;
                ItemDat.Mode = Flags.ItemMode.AddItem;
                ItemDat.Send(Owner, stream);
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0, 0));

                return true;

            }

            return false;
        }

        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Pool.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                {
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                }
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
            if (ItemDat.ITEM_ID / 1000 == 203)
                if (ItemDat.Bless > 1)
                    ItemDat.Bless = 0;
        }

        public bool Exist(Func<Game.MsgServer.MsgGameItem, bool> predicate)
        {
            bool Exist = false;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    Exist = true;
                    break;
                }
            return Exist;
        }

        public void Have(Func<Game.MsgServer.MsgGameItem, bool> predicate, out int count)
        {
            count = 0;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    count++;
                }

        }

        public bool Exist(Func<Game.MsgServer.MsgGameItem, bool> predicate, int count)
        {
            int counter = 0;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    counter++;
                }
            return counter >= count;
        }

        public ICollection<Game.MsgServer.MsgGameItem> AllItems
        {
            get { return ClientItems.Values; }
        }

        public bool Contains(uint UID)
        {
            return ClientItems.ContainsKey(UID);
        }

        public bool TryGetValue(uint UID, out Game.MsgServer.MsgGameItem itemdata)
        {
            return ClientItems.TryGetValue(UID, out itemdata);
        }

        public bool FreeEquip(Role.Flags.ConquerItem position)
        {
            var item = ClientItems.Values.Where(p => p.Position == (ushort)position)
                .FirstOrDefault();
            return item == null;
        }

        public bool TryGetEquip(Role.Flags.ConquerItem position, out Game.MsgServer.MsgGameItem itemdata)
        {

            itemdata = ClientItems.Values.Where(p => p.Position == (ushort)position).FirstOrDefault();
            return itemdata != null;
        }

        public Game.MsgServer.MsgGameItem TryGetEquip(Role.Flags.ConquerItem position)
        {

            return ClientItems.Values.Where(p => p.Position == (ushort)position).FirstOrDefault();
        }

        public bool Remove(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            if (!FreeEquip(position))
            {
                if ((byte)position > 50)
                {
                    return RemoveAlternante(position, stream);
                }
                bool Accept = Owner.Inventory.HaveSpace(1);
                if (Accept)
                {
                    Game.MsgServer.MsgGameItem itemdata;
                    if (TryGetEquip(position, out itemdata))
                    {
                        if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                        {
                            if (position == Flags.ConquerItem.Garment || position == Flags.ConquerItem.SteedMount)
                            {
                                itemdata.Position = 0;
                            }
                            else
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0, 0));
                                itemdata.Position = 0;
                                itemdata.Mode = Flags.ItemMode.AddItem;
                                Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                            }
                        }
                    }
                }
                else
                {
#if Arabic
                     Owner.SendSysMesage("Your Inventory Is Full.");
#else
                    Owner.SendSysMesage("Your Inventory Is Full.");
#endif

                }
                return Accept;
            }
            else
                return false;
        }

        public bool Delete(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            if (!FreeEquip(position))
            {
                if ((byte)position > 50)
                {
                    return DeleteAlternante(position, stream);
                }
                bool Accept = Owner.Inventory.HaveSpace(1);
                if (Accept)
                {
                    Game.MsgServer.MsgGameItem itemdata;
                    if (TryGetEquip(position, out itemdata))
                    {
                        if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                        {
                            if (position == Flags.ConquerItem.Garment || position == Flags.ConquerItem.SteedMount)
                            {
                                itemdata.Position = 0;
                            }
                            else
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0, 0));
                                itemdata.Position = 0;
                                itemdata.Mode = Flags.ItemMode.AddItem;
                                Owner.Inventory.Update(itemdata, AddMode.REMOVE, stream);
                            }
                        }
                    }
                }
                else
                {
#if Arabic
                     Owner.SendSysMesage("Your Inventory Is Full.");
#else
                    Owner.SendSysMesage("Your Inventory Is Full.");
#endif

                }
                return Accept;
            }
            else
                return false;
        }

        public bool DeleteAlternante(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            bool Accept = Owner.Inventory.HaveSpace(1);
            if (Accept)
            {
                Game.MsgServer.MsgGameItem itemdata;
                if (TryGetEquip(position, out itemdata))
                {
                    if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                    {
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0, 0));
                        itemdata.Position = 0;
                        itemdata.Mode = Flags.ItemMode.AddItem;
                        Owner.Inventory.Update(itemdata, AddMode.REMOVE, stream);
                    }
                }
            }
            else
            {
#if Arabic
                     Owner.SendSysMesage("Your Inventory Is Full.");
#else
                Owner.SendSysMesage("Your Inventory Is Full.");
#endif

            }
            return Accept;
        }

        public bool RemoveAlternante(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            bool Accept = Owner.Inventory.HaveSpace(1);
            if (Accept)
            {
                Game.MsgServer.MsgGameItem itemdata;
                if (TryGetEquip(position, out itemdata))
                {
                    if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                    {
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0, 0));
                        itemdata.Position = 0;
                        itemdata.Mode = Flags.ItemMode.AddItem;
                        Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                    }
                }
            }
            else
            {
#if Arabic
                     Owner.SendSysMesage("Your Inventory Is Full.");
#else
                Owner.SendSysMesage("Your Inventory Is Full.");
#endif

            }
            return Accept;
        }

        public void Add(Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            CheakUp(item);

            if (item.Position > 50)
            {
                AddAlternante(item, stream);
                return;
            }
            ClientItems.TryAdd(item.UID, item);
            item.Mode = Flags.ItemMode.AddItem;
            item.Send(Owner, stream);
            if (item.Position == (ushort)Role.Flags.ConquerItem.SteedMount || item.Position == (ushort)Role.Flags.ConquerItem.Garment)
            {
                Owner.MyWardrobe.AddItem(item);
            }

        }

        public void AddAlternante(Game.MsgServer.MsgGameItem itemdata, ServerSockets.Packet stream)
        {
            ClientItems.TryAdd(itemdata.UID, itemdata);
            itemdata.Mode = Flags.ItemMode.AddItem;

            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0, 0));

            itemdata.Send(Owner, stream);
        }

        public void Show(ServerSockets.Packet stream)
        {
            foreach (var item in ClientItems.Values.OrderBy(i => i.Position))
            {

                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            QueryEquipment(Alternante);
        }

        public unsafe void ClearItemSpawn()
        {
            Owner.Player.ClearItemsSpawn();
        }

        public unsafe void AddSpawn(Game.MsgServer.MsgGameItem DataItem)
        {

            switch ((Role.Flags.ConquerItem)DataItem.Position)
            {
                case Role.Flags.ConquerItem.Bottle:
                case Role.Flags.ConquerItem.AleternanteBottle:
                    {
                        Owner.Player.Bottle = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteArmor:
                case Role.Flags.ConquerItem.Armor:
                    {
                        Owner.Player.ArmorId = DataItem.ITEM_ID;
                        Owner.Player.ColorArmor = (ushort)DataItem.Color;
                        Owner.Player.ArmorSoul = DataItem.Purification.PurificationItemID;

                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteHead:
                case Role.Flags.ConquerItem.Head:
                    {

                        Owner.Player.HeadId = DataItem.ITEM_ID;
                        Owner.Player.ColorHelment = (ushort)DataItem.Color;
                        Owner.Player.HeadSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteLeftWeapon:
                case Role.Flags.ConquerItem.LeftWeapon:
                    {
                        Owner.Player.LeftWeaponId = DataItem.ITEM_ID;
                        Owner.Player.LeftWeapsonSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.LeftWeaponAccessory:
                    {
                        Owner.Player.LeftWeaponAccessoryId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteRightWeapon:
                case Role.Flags.ConquerItem.RightWeapon:
                    {
                        Owner.Player.RightWeaponId = DataItem.ITEM_ID;
                        Owner.Player.ColorShield = (ushort)DataItem.Color;
                        Owner.Player.RightWeapsonSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.RightWeaponAccessory:
                    {
                        Owner.Player.RightWeaponAccessoryId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.Steed:
                    {
                        Owner.Player.SteedId = DataItem.ITEM_ID;
                        Owner.Player.SteedColor = DataItem.SocketProgress;
                        Owner.Player.SteedPlus = DataItem.Plus;
                        SteedPlusPorgres = DataItem.PlusProgress;
                        break;
                    }
                case Role.Flags.ConquerItem.SteedMount:
                    {
                        Owner.Player.MountArmorId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteGarment:
                case Role.Flags.ConquerItem.Garment:
                    {

                        Owner.Player.GarmentId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.Wing:
                    {
                        if (DataItem.Plus == 12)
                            Owner.Player.WingProgress = DataItem.PlusProgress;
                        Owner.Player.WingId = DataItem.ITEM_ID;
                        Owner.Player.WingPlus = DataItem.Plus;
                        break;
                    }

            }
            if (Owner.Player.SpecialGarment != 0)
                Owner.Player.GarmentId = Owner.Player.SpecialGarment;
        }

        public unsafe void UpdateStats(Game.MsgServer.MsgGameItem[] MyGear, ServerSockets.Packet stream)
        {

            try
            {
                Owner.PrestigeLevel = 0;
                SoulsPotency = 0;
                rangeR = rangeL = SizeAdd = 0;
                SpeedR = SpeedL = SpeedRing = 0;
                RightWeapon = 0;
                LeftWeapon = 0;
                UseMonkEpicWeapon = false;
                SuperArmor = false;
                HeadID = 0;
                ShieldID = 0;
                WeaponsMinAttack = 0;
                HaveBless = false;
                RingEffect = Flags.ItemEffect.None;
                RightWeaponEffect = Flags.ItemEffect.None;
                SteedPlusPorgres = 0;
                Owner.Status.MaxVigor = 0;
                RidingCrop = 0;

                if (CreateSpawn)
                {
                    lock (CurentEquip)
                        CurentEquip = MyGear;
                    ClearItemSpawn();
                }
                Owner.Status = new MsgStatus();
                Owner.Status.UID = Owner.Player.UID;

                Owner.Status.MaxAttack = (ushort)(Owner.Player.Strength + 1);
                Owner.Status.MinAttack = (ushort)(Owner.Player.Strength);
                Owner.Status.MagicAttack = Owner.Player.Spirit;

                Owner.Gems = new ushort[13];


                foreach (var item in MyGear)
                {

                    if (item.Bless > 7)
                        item.Bless = 7;
                    if (item.Enchant > 255)
                        item.Enchant = 255;
                    if (item.Plus > 12)
                        item.Plus = 12;
                    if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Head
                        || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.AleternanteHead)
                        HeadID = item.ITEM_ID;

                    try
                    {

                        if (item.Durability == 0)
                            continue;

                        ushort ItemPostion = (ushort)(item.Position % 50);


                        if (CreateSpawn)
                            AddSpawn(item);

                        Owner.PrestigeLevel += item.PerfectionLevel;


                        LoadMythsoul(item);
                        if (item.Bless >= 1)
                            HaveBless = true;

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Armor)
                        {
                            SuperArmor = (item.ITEM_ID % 10) == 9;

                            ArmorID = item.ITEM_ID;
                        }
                        if (item.SocketOne != Role.Flags.Gem.NoSocket && item.SocketOne != Role.Flags.Gem.EmptySocket)
                        {
                            if (item.SocketOne == Role.Flags.Gem.SuperTortoiseGem)
                                SuprtTortoiseGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperDragonGem)
                                SuperDragonGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperPhoenixGem)
                                SuperPheonixGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperVioletGem)
                                SuperVioletGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperRainbowGem)
                                SuperRaibowGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperMoonGem)
                                SuperMoonGem = true;
                        }

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.RidingCrop)
                        {
                            RidingCrop = item.ITEM_ID;
                            Owner.Status.MaxVigor += 1000;
                        }
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                        {
                            if (Database.ItemType.IsMonkEpicWeapon(item.ITEM_ID))
                                UseMonkEpicWeapon = true;
                            LeftWeapon = item.ITEM_ID;
                            if (Database.ItemType.IsShield(item.ITEM_ID))
                                ShieldID = item.ITEM_ID;
                        }
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                        {
                            if (Database.ItemType.IsMonkEpicWeapon(item.ITEM_ID))
                                UseMonkEpicWeapon = true;
                            RightWeaponEffect = item.Effect;
                            RightWeapon = item.ITEM_ID;
                        }


                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
                            RingEffect = item.Effect;

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Necklace)
                            NecklaceEffect = item.Effect;

                        AddGem(item.SocketOne);
                        AddGem(item.SocketTwo);



                        var DBItem = Pool.ItemsBase[item.ITEM_ID];
                        item.ItemPoints = Database.ItemType.GetItemPoints(DBItem, item);
                        if (item.GetPerfectionPosition != -1)
                        {
                            if (item.OwnerUID != 0)
                            {
                                if (Database.GroupServerList.MyServerInfo.ID == Owner.Player.ServerID)
                                    Database.RankItems.RankPoll[(uint)item.GetPerfectionPosition].AddItem(item);
                            }
                        }



                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                        {
                            Owner.Status.PhysicalDamageIncrease += DBItem.MaxAttack;
                            Owner.Status.MagicDamageIncrease += DBItem.MagicAttack;
                        }
                        else
                        {
                            if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
                            {
                                SpeedRing = DBItem.Frequency;
                            }
                            if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                            {
                                rangeL = DBItem.AttackRange;
                                SpeedL = DBItem.Frequency;

                                #region Tempered Glaive
                                uint rate = 0;
                                byte TemperedGlaive = 0;
                                if (Owner.Rune.IsEquipped("Tempered Glaive", ref TemperedGlaive) || Owner.Rune.IsEquipped("Honed Soul", ref TemperedGlaive))
                                {
                                    switch (TemperedGlaive)
                                    {
                                        case 1: rate = 55; break;
                                        case 2: rate = 56; break;
                                        case 3: rate = 58; break;
                                        case 4: rate = 60; break;
                                        case 5: rate = 62; break;
                                        case 6: rate = 64; break;
                                        case 7: rate = 66; break;
                                        case 8: rate = 68; break;
                                        case 9: rate = 70; break;
                                    }
                                    uint add = (uint)(DBItem.MaxAttack / 2) * rate / 100;
                                    Owner.Status.MaxAttack += (uint)add;
                                    Owner.Status.MinAttack += (uint)add;
                                }
                                #endregion
                                WeaponsMinAttack += (int)(DBItem.MaxAttack / 2);
                                Owner.Status.MaxAttack += (uint)(DBItem.MaxAttack / 2);
                                Owner.Status.MinAttack += (uint)(DBItem.MinAttack / 2);
                                Owner.Status.MagicAttack += (uint)(DBItem.MagicAttack / 2);

                            }
                            else
                            {
                                if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                                {
                                    WeaponsMinAttack += DBItem.MinAttack;
                                    rangeR = DBItem.AttackRange;
                                    SpeedR = DBItem.Frequency;
                                    #region WideSwipe
                                    if (Database.ItemType.IsTwoHand(item.ITEM_ID))
                                    {
                                        byte itemLevel = 0;
                                        int WideSwipe = 0;
                                        if (Owner.Rune.IsEquipped("WideSwipe", ref itemLevel))
                                        {
                                            switch (itemLevel)
                                            {
                                                case 1:
                                                    {
                                                        WideSwipe = 1;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        WideSwipe = 2;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        WideSwipe = 3;
                                                        break;
                                                    }
                                                case 4:
                                                    {
                                                        WideSwipe = 4;
                                                        break;
                                                    }
                                                case 5:
                                                    {
                                                        WideSwipe = 5;
                                                        break;
                                                    }
                                                case 6:
                                                    {
                                                        WideSwipe = 6;
                                                        break;
                                                    }
                                                case 7:
                                                    {
                                                        WideSwipe = 7;
                                                        break;
                                                    }
                                                case 8:
                                                    {
                                                        WideSwipe = 8;
                                                        break;
                                                    }
                                                case 9:
                                                    {
                                                        WideSwipe = 10;
                                                        break;
                                                    }
                                            }
                                            Owner.Status.MaxAttack += (uint)(DBItem.MaxAttack * WideSwipe / 100);
                                            Owner.Status.MinAttack += (uint)(DBItem.MinAttack * WideSwipe / 100);
                                        }
                                    }
                                    #endregion
                                }

                                Owner.Status.MaxAttack += DBItem.MaxAttack;
                                Owner.Status.MinAttack += DBItem.MinAttack;
                                Owner.Status.MagicAttack += DBItem.MagicAttack;
                            }
                        }
                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                        {
                            Owner.Status.MagicDamageDecrease += DBItem.MagicDefence;
                            Owner.Status.PhysicalDamageDecrease += DBItem.PhysicalDefence;
                        }
                        else
                        {
                            Owner.Status.Immunity += DBItem.Imunity;
                            Owner.Status.CriticalStrike += DBItem.Crytical;
                            Owner.Status.SkillCStrike += DBItem.SCrytical;
                            Owner.Status.Breakthrough += DBItem.BreackTrough;
                            Owner.Status.Counteraction += DBItem.ConterAction;
                            Owner.Status.MDefence += (byte)DBItem.MagicDefence;
                            Owner.Status.Defence += DBItem.PhysicalDefence;
                            if (DBItem.ID == 2169165 || DBItem.ID == 2169645 || DBItem.ID == 2100075 || DBItem.ID == 2100095)
                            {
                                Owner.Status.DashRate += 300;
                                Owner.Status.Resist += 300;

                            }

                            #region P1 Jade Kylin
                            if (DBItem.ID == 2169805)
                            {
                                Owner.Status.DashRate += 15;
                                Owner.Status.Resist += 15;

                            }
                            #endregion

                            #region P2 Jade Kylin
                            if (DBItem.ID == 2169815)
                            {
                                Owner.Status.DashRate += 30;
                                Owner.Status.Resist += 30;

                            }
                            #endregion

                            #region P3 Jade Kylin
                            if (DBItem.ID == 2169825)
                            {
                                Owner.Status.DashRate += 45;
                                Owner.Status.Resist += 45;

                            }
                            #endregion

                            #region P4 Jade Kylin
                            if (DBItem.ID == 2169835)
                            {
                                Owner.Status.DashRate += 60;
                                Owner.Status.Resist += 60;

                            }
                            #endregion

                            #region P5 Jade Kylin
                            if (DBItem.ID == 2169845)
                            {
                                Owner.Status.DashRate += 75;
                                Owner.Status.Resist += 75;

                            }
                            #endregion

                            #region P6 Jade Kylin
                            if (DBItem.ID == 2169855)
                            {
                                Owner.Status.DashRate += 90;
                                Owner.Status.Resist += 90;

                            }
                            #endregion

                            #region P7 Jade Kylin
                            if (DBItem.ID == 2169865)
                            {
                                Owner.Status.DashRate += 105;
                                Owner.Status.Resist += 105;

                            }
                            #endregion

                            #region P8 Jade Kylin
                            if (DBItem.ID == 2169875)
                            {
                                Owner.Status.DashRate += 120;
                                Owner.Status.Resist += 120;

                            }
                            #endregion

                            #region P9 Jade Kylin
                            if (DBItem.ID == 2169885)
                            {
                                Owner.Status.DashRate += 135;
                                Owner.Status.Resist += 135;

                            }
                            #endregion

                            #region P10 Jade Kylin
                            if (DBItem.ID == 2169895)
                            {
                                Owner.Status.DashRate += 150;
                                Owner.Status.Resist += 150;

                            }
                            #endregion

                            #region P11 Jade Kylin
                            if (DBItem.ID == 2169905)
                            {
                                Owner.Status.DashRate += 180;
                                Owner.Status.Resist += 180;

                            }
                            #endregion

                            #region P12 Jade Kylin
                            if (DBItem.ID == 2169915)
                            {
                                Owner.Status.DashRate += 210;
                                Owner.Status.Resist += 210;

                            }
                            #endregion

                            #region P13 Jade Kylin
                            if (DBItem.ID == 2169925)
                            {
                                Owner.Status.DashRate += 240;
                                Owner.Status.Resist += 240;

                            }
                            #endregion

                            #region P14 Jade Kylin
                            if (DBItem.ID == 2169935)
                            {
                                Owner.Status.DashRate += 270;
                                Owner.Status.Resist += 270;

                            }
                            #endregion

                            #region P15 Jade Kylin
                            if (DBItem.ID == 2169945)
                            {
                                Owner.Status.DashRate += 300;
                                Owner.Status.Resist += 300;

                            }
                            #endregion


                        }

                        if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
                        {
                            Owner.Status.Dodge += DBItem.Dodge;
                            Owner.Status.AgilityAtack += DBItem.Frequency;
                            Owner.Status.ItemBless += item.Bless;

                            #region Circle of Life
                            uint rate = 0;
                            byte CircleofLife = 0;
                            if (Owner.Rune.IsEquipped("Circle of Life", ref CircleofLife) || Owner.Rune.IsEquipped("Supreme Armor", ref CircleofLife))
                            {
                                switch (CircleofLife)
                                {
                                    case 1: rate = 50; break;
                                    case 2: rate = 60; break;
                                    case 3: rate = 80; break;
                                    case 4: rate = 100; break;
                                    case 5: rate = 120; break;
                                    case 6: rate = 140; break;
                                    case 7: rate = 160; break;
                                    case 8: rate = 180; break;
                                    case 9: rate = 200; break;
                                }
                                uint Add = (uint)(item.Enchant * rate / 100);
                                Owner.Status.MaxHitpoints += Add;




                            }
                            #endregion
                            Owner.Status.MaxHitpoints += item.Enchant;
                        }
                        Owner.Status.MaxHitpoints += DBItem.ItemHP;
                        Owner.Status.MaxMana += DBItem.ItemMP;

                        if (item.Purification.InLife)
                        {
                            var purificare = Pool.ItemsBase[item.Purification.PurificationItemID];
                            Owner.Status.MaxAttack += purificare.MaxAttack;
                            Owner.Status.MinAttack += purificare.MinAttack;
                            Owner.Status.MagicAttack += purificare.MagicAttack;
                            Owner.Status.MDefence += (byte)purificare.MagicDefence;
                            Owner.Status.Defence += purificare.PhysicalDefence;

                            Owner.Status.CriticalStrike += purificare.Crytical;
                            Owner.Status.SkillCStrike += purificare.SCrytical;
                            Owner.Status.Immunity += purificare.Imunity;
                            Owner.Status.Penetration += purificare.Penetration;
                            Owner.Status.Block += purificare.Block;
                            Owner.Status.Breakthrough += purificare.BreackTrough;
                            Owner.Status.Counteraction += purificare.ConterAction;
                            Owner.Status.Detoxication += purificare.Detoxication;

                            Owner.Status.MetalResistance += purificare.MetalResistance;
                            Owner.Status.WoodResistance += purificare.WoodResistance;
                            Owner.Status.FireResistance += purificare.FireResistance;
                            Owner.Status.EarthResistance += purificare.EarthResistance;
                            Owner.Status.WaterResistance += purificare.WaterResistance;

                            Owner.Status.MaxHitpoints += purificare.ItemHP;
                            Owner.Status.MaxMana += purificare.ItemMP;
                            SoulsPotency += item.Purification.PurificationLevel;
                        }

                        if (item.Refinary.InLife)
                        {
                            IncreaseRifainaryStatus(item.Refinary.EffectID);
                        }

                        if (item.SocketOne != Flags.Gem.NoSocket && item.SocketOne != Flags.Gem.EmptySocket)
                        {
                            if (item.SocketOne == Flags.Gem.NormalTortoiseGem)
                            {
                                Owner.Status.Damage += 2;
                            }
                            if (item.SocketOne == Flags.Gem.RefinedTortoiseGem)
                            {
                                Owner.Status.Damage += 4;
                            }
                            if (item.SocketOne == Flags.Gem.SuperTortoiseGem)
                            {
                                Owner.Status.Damage += 6;
                            }
                        }
                        if (item.SocketTwo != Flags.Gem.NoSocket && item.SocketTwo != Flags.Gem.EmptySocket)
                        {
                            if (item.SocketTwo == Flags.Gem.NormalTortoiseGem)
                            {
                                Owner.Status.Damage += 2;
                            }
                            if (item.SocketTwo == Flags.Gem.RefinedTortoiseGem)
                            {
                                Owner.Status.Damage += 4;
                            }
                            if (item.SocketTwo == Flags.Gem.SuperTortoiseGem)
                            {
                                Owner.Status.Damage += 6;
                            }
                        }
                        if (item.Plus > 0 && item.Plus < 13)
                        {
                            byte AnimaPercent = (byte)(item.AnimaItemID > 0 ? Pool.ItemsBase[item.AnimaItemID].ItemHP / 100 : 0);
                            var extraitematributes = DBItem.Plus[item.Plus];
                            if (extraitematributes != null)
                            {
                                if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon || ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                                {
                                    Owner.Status.Accuracy += (uint)(extraitematributes.Agility + (extraitematributes.Agility * AnimaPercent / 100));
                                }

                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Steed)
                                    Owner.Status.MaxVigor = extraitematributes.Agility;

                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                                {
                                    Owner.Status.PhysicalDamageIncrease += extraitematributes.MaxAttack;
                                    Owner.Status.MagicDamageIncrease += extraitematributes.MagicAttack;
                                }
                                else
                                {
                                    if (ItemPostion == (byte)Role.Flags.ConquerItem.LeftWeapon)
                                    {
                                        Owner.Status.MaxAttack += ((uint)(extraitematributes.MaxAttack + (extraitematributes.MaxAttack * AnimaPercent / 100)) / 2);
                                        Owner.Status.MinAttack += ((uint)(extraitematributes.MinAttack + (extraitematributes.MinAttack * AnimaPercent / 100)) / 2);
                                        Owner.Status.MagicAttack += ((uint)(extraitematributes.MagicAttack + (extraitematributes.MagicAttack * AnimaPercent / 100)) / 2);
                                    }
                                    else
                                    {
                                        Owner.Status.MaxAttack += (uint)(extraitematributes.MaxAttack + (extraitematributes.MaxAttack * AnimaPercent / 100));
                                        Owner.Status.MinAttack += (uint)(extraitematributes.MinAttack + (extraitematributes.MinAttack * AnimaPercent / 100));
                                        Owner.Status.MagicAttack += (uint)(extraitematributes.MagicAttack + (extraitematributes.MagicAttack * AnimaPercent / 100));
                                    }
                                }
                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                                {
                                    Owner.Status.MagicDamageDecrease += extraitematributes.MagicDefence;
                                    Owner.Status.PhysicalDamageDecrease += extraitematributes.PhysicalDefence;
                                }
                                else
                                {
                                    Owner.Status.MagicDefence += (uint)(extraitematributes.MagicDefence + (extraitematributes.MagicDefence * AnimaPercent / 100));
                                    Owner.Status.Defence += (uint)(extraitematributes.PhysicalDefence + (extraitematributes.PhysicalDefence * AnimaPercent / 100));
                                }


                                if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
                                {
                                    Owner.Status.Dodge += (uint)(extraitematributes.Dodge + (extraitematributes.Dodge * AnimaPercent / 100));

                                }
                                Owner.Status.MaxHitpoints += (uint)(extraitematributes.ItemHP + (extraitematributes.ItemHP * AnimaPercent / 100));
                            }
                        }
                        if (item.AnimaItemID > 0)
                        {
                            var anima = Pool.ItemsBase[item.AnimaItemID];
                            Owner.Status.PhysicalDamageDecrease += anima.PhysicalDamageDecrease;
                            Owner.Status.MagicDamageDecrease += anima.MagicDamageDecrease;
                            Owner.Status.PhysicalDamageIncrease += anima.PhysicalDamageIncrease;
                            Owner.Status.MagicDamageIncrease += anima.MagicDamageIncrease;
                            Owner.Status.TotalAnimaLevel += (ushort)(item.AnimaItemID % 100);
                        }
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Relics)
                        {
                            var Relics = new List<MsgGameItem>();
                            Relics.Add(item);
                            Relics.AddRange(Owner.MinorRelics.Values.Where(i =>
                                Qahr.Mobile.MY_System.Relic.MainRelic.IsResonancePosition(i.Position) &&
                                i.EonspiritActived >= 1 && i.EonspiritActived <= 4)
                                .GroupBy(i => i.EonspiritActived)
                                .Select(g => g.OrderBy(i => i.UID).First()));

                            uint relicCriticalStrike = 0;
                            uint relicSkillCStrike = 0;
                            uint relicImmunity = 0;
                            uint relicBreakthrough = 0;
                            uint relicCounteraction = 0;
                            uint relicHP = 0;
                            uint relicPAttackMin = 0;
                            uint relicPAttackMax = 0;
                            uint relicMAttack = 0;
                            uint relicMDefense = 0;
                            uint relicFinalPAttack = 0;
                            uint relicFinalMAttack = 0;
                            uint relicFinalPDamage = 0;
                            uint relicFinalMDamage = 0;
                            uint relicLuckyStrike = 0;

                            // DashRate الناتج من CriticalStrike / SkillCriticalStrike
                            uint relicDashRate = 0;

                            foreach (var attrs in Relics)
                            {
                                foreach (var attr in attrs.RelicAttributes.Where(i => i > 0))
                                {
                                    switch (attr.Type)
                                    {
                                        case MsgChiInfo.ChiAttribute.CriticalStrike:
                                            relicCriticalStrike += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.SkillCriticalStrike:
                                            relicSkillCStrike += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.DashRate:
                                            relicDashRate += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.Counteraction:
                                            relicCounteraction += (attr.Value / 10);
                                            break;

                                        case MsgChiInfo.ChiAttribute.AddAttack:
                                            relicPAttackMin += attr.Value;
                                            relicPAttackMax += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.AddMagicAttack:
                                            relicMAttack += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.AddMagicDefense:
                                            relicMDefense += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.Breakthrough:
                                            relicBreakthrough += (attr.Value / 10);
                                            break;

                                        case MsgChiInfo.ChiAttribute.HPAdd:
                                            relicHP += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.Immunity:
                                            relicImmunity += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.MagicDamageDecrease:
                                            relicFinalMDamage += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.MagicDamageIncrease:
                                            relicFinalMAttack += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.PhysicalDamageDecrease:
                                            relicFinalPDamage += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.PhysicalDamageIncrease:
                                            relicFinalPAttack += attr.Value;
                                            break;

                                        case MsgChiInfo.ChiAttribute.LuckyStrike:
                                            relicLuckyStrike += attr.Value;
                                            break;
                                    }
                                }
                            }

                            relicCriticalStrike =
                                Math.Min(relicCriticalStrike, RelicLimit.CriticalStrike);

                            relicSkillCStrike =
                                Math.Min(relicSkillCStrike, RelicLimit.SkillCStrike);

                            relicImmunity =
                                Math.Min(relicImmunity, RelicLimit.Immunity);

                            relicBreakthrough =
                                Math.Min(relicBreakthrough, RelicLimit.Breakthrough);

                            relicCounteraction =
                                Math.Min(relicCounteraction, RelicLimit.Counteraction);

                            relicLuckyStrike =
                                Math.Min(relicLuckyStrike, RelicLimit.LuckyStrike);

                            relicHP =
                                Math.Min(relicHP, RelicLimit.MaxHP);

                            relicPAttackMin =
                                Math.Min(relicPAttackMin, RelicLimit.PhysicalAttack);

                            relicPAttackMax =
                                Math.Min(relicPAttackMax, RelicLimit.PhysicalAttack);

                            relicMAttack =
                                Math.Min(relicMAttack, RelicLimit.MagicAttack);

                            relicMDefense =
                                Math.Min(relicMDefense, RelicLimit.MagicDefense);

                            relicFinalPAttack =
                                Math.Min(relicFinalPAttack, RelicLimit.FinalPAttack);

                            relicFinalMAttack =
                                Math.Min(relicFinalMAttack, RelicLimit.FinalMAttack);

                            relicFinalPDamage =
                                Math.Min(relicFinalPDamage, RelicLimit.FinalPDamage);

                            relicFinalMDamage =
                                Math.Min(relicFinalMDamage, RelicLimit.FinalMDamage);

                            // إضافة الـ Relic Stats إلى Status
                            Owner.Status.CriticalStrike += relicCriticalStrike;
                            Owner.Status.SkillCStrike += relicSkillCStrike;

                            // ⭐ DashRate
                            Owner.Status.DashRate += relicDashRate;

                            Owner.Status.Immunity += relicImmunity;
                            Owner.Status.Breakthrough += relicBreakthrough;
                            Owner.Status.Counteraction += relicCounteraction;
                            Owner.Status.LuckyStrike += relicLuckyStrike;

                            Owner.Status.MaxHitpoints += relicHP;
                            Owner.Status.MinAttack += relicPAttackMin;
                            Owner.Status.MaxAttack += relicPAttackMax;
                            Owner.Status.MagicAttack += relicMAttack;
                            Owner.Status.MagicDefence += relicMDefense;

                            Owner.Status.PhysicalDamageIncrease += relicFinalPAttack;
                            Owner.Status.MagicDamageIncrease += relicFinalMAttack;
                            Owner.Status.PhysicalDamageDecrease += relicFinalPDamage;
                            Owner.Status.MagicDamageDecrease += relicFinalMDamage;

                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                }
                Owner.Status.RuneScore = Owner.Rune.Score;
                Owner.Status.MagicDefence += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);
                Owner.Status.PhysicalDamageDecrease += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);
                Owner.Status.PhysicalDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);
                Owner.Status.MagicDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);
                Owner.MedalStorage.Update(Owner.Status);
                CalculateBattlePower();
                #region New System
                if (Owner.Player.ContainFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.DivineArrival))
                {
                    foreach (var Item in Owner.EonspiritSystem.Values.Where(i => i.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive))
                    {
                        if (Database.ItemType.EonspiritItem.Contains(Item.ITEM_ID))
                        {
                            var Info = Database.YuanshenAttr.YuanshenAttrItem.Values.Where(i => i.ItemID == Item.ITEM_ID && i.TypeLevel == Item.EonspiritPercentage).FirstOrDefault();
                            if (Info != null)
                            {
                                //Owner.Status.MaxHitpoints += Info.HPValue;
                                //Owner.Player.HitPoints += (int)Info.HPValue;
                                //Owner.Status.MinAttack += Info.AttackValue;
                                //Owner.Status.MaxAttack += Info.AttackValue;
                            }
                        }
                    }
                }
                #endregion
                MythTable.GetMythSouls(Owner);
                AddBeastsAtributes();
                AddProfessionAtributes();
                AddChiAtribute(Owner.Player.MyChi);
                AddInnerPowerAtributes(Owner.Player.InnerPower);
                Owner.CreatePrestigePoints();
                AddPerfectionAtributes();
                AddHWStats();
                AddAstredgeStats();
                AddAstredgeStats2();
                AddArchiveNinjaStats(Owner);
                AddArchives(Owner);
                AddGuildSkillStats(Owner);
                AddMythSoulAtributes();
                AddJadeAtributes();
                RunePirateAtributes();
                Owner.Status.MaxHitpoints += Owner.CalculateHitPoint();
                Owner.Status.MaxMana += Owner.CalculateMana();

                Owner.Vigor = (ushort)Math.Min((int)Owner.Vigor, (int)Owner.Status.MaxVigor);
                if (CreateSpawn)
                    Owner.Send(stream.ServerInfoCreate(Owner.Vigor));


                if (CreateSpawn)
                    Owner.Player.View.SendView(Owner.Player.GetArray(stream, false), false);
                Owner.Player.CheckAura();

                if (!this.Owner.Fake)
                {
                    Owner.Player.SubClass.UpdateStatus(Owner);
                    #region VIP [ 7 ]
                    if (false && Owner.Player.VipLevel == 7) // الحاجات الى بيزودها الفي اي بي 7 // [CombatAudit 2026-10-14] disabled: official VIP gives no combat stats (no attack/defense/crit text in any of the 633 official STR_VIP* strings)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            if (Owner.Player.VipLevel == 7)
                            {
                                Owner.Status.MaxHitpoints += 3000;
                                Owner.Status.MinAttack += 1200;
                                Owner.Status.MaxAttack += 1200;
                                Owner.Status.MagicAttack += 1200;
                                Owner.Status.Defence += 2000;
                                Owner.Status.Immunity += 300;
                                Owner.Status.CriticalStrike += 300;
                                Owner.Status.Breakthrough += 100;
                                Owner.Status.Counteraction += 100;
                            }
                        }
                    }
                }
                    #endregion
                Owner.Player.SubClass.UpdateStatus(Owner);

                if (Owner.Player.MyJiangHu != null)
                    Owner.Player.MyJiangHu.CreateStatusAtributes(Owner);
                Owner.HundredWeapons.UpdateRank();
                Owner.MyNinja.UpdateRank();
                Owner.MyArchives.UpdateRank();
                Owner.MyAstredge.UpdateRank();


                #region PerfectionStatus
                Owner.Status.MaxAttack += (uint)Owner.PerfectionStatus.PhysicalAttack;
                Owner.Status.MinAttack += (uint)Owner.PerfectionStatus.PhysicalAttack;
                Owner.Status.MagicAttack += (uint)Owner.PerfectionStatus.MagicAttack;
                Owner.Status.MagicDefence += (uint)Owner.PerfectionStatus.MagicDefense;
                Owner.Status.Defence += (uint)Owner.PerfectionStatus.PhysicalDefence;
                Owner.Status.LuckyStrike += (uint)Owner.PerfectionStatus.LuckyStrike;
                Owner.Status.Parry += (uint)Owner.PerfectionStatus.Parry;
                #endregion
                #region Wardrobe
                if (Owner.MyWardrobe != null)
                {
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Garment].Count >= 30)
                    {
                        Owner.Status.MagicAttack += 300;
                        Owner.Status.MagicDefence += 100;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Garment].Count >= 50)
                    {
                        Owner.Status.Defence += 100;
                        Owner.Status.MinAttack += 100;
                        Owner.Status.MaxAttack += 100;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Garment].Count >= 100)
                    {
                        Owner.Status.MagicAttack += 300;
                        Owner.Status.MagicDefence += 100;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Garment].Count >= 130)
                    {
                        Owner.Status.MinAttack += 200;
                        Owner.Status.MaxAttack += 200;
                        Owner.Status.Defence += 200;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Mount].Count >= 30)
                    {
                        Owner.Status.MagicAttack += 500;
                        Owner.Status.MagicDefence += 200;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Mount].Count >= 50)
                    {
                        Owner.Status.Defence += 150;
                        Owner.Status.MinAttack += 150;
                        Owner.Status.MaxAttack += 150;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Mount].Count >= 100)
                    {
                        Owner.Status.MagicAttack += 500;
                        Owner.Status.MagicDefence += 200;
                    }
                    if (Owner.MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Mount].Count >= 130)
                    {
                        Owner.Status.MinAttack += 300;
                        Owner.Status.MaxAttack += 300;
                        Owner.Status.Defence += 300;
                    }
                }
                #endregion
                #region Collection
                if (Owner.Collection.items != null)
                {
                    if (Owner.Collection.items != null)
                    {
                        var FirstCollection = Owner.Collection.items.Values.Where(p => p.ID >= 4300000 && p.ID <= 4300005).ToArray();
                        if (FirstCollection.Length >= 1)
                        {
                            Owner.Status.Parry += 100;
                        }
                        if (FirstCollection.Length >= 2)
                        {
                            Owner.Status.Immunity += 100;
                            Owner.Status.Defence += 200;
                            Owner.Status.MDefence += 100;
                            Owner.Status.LuckyStrike += 100;
                        }
                        if (FirstCollection.Length >= 3)
                        {
                            Owner.Status.Counteraction += 20;
                            Owner.Status.CriticalStrike += 100;
                            Owner.Status.PhysicalDamageDecrease += 500;
                            Owner.Status.MagicDamageDecrease += 300;
                        }
                        if (FirstCollection.Length >= 4)
                        {
                            Owner.Status.Parry += 100;
                            Owner.Status.PhysicalDamageIncrease += 500;
                            Owner.Status.MagicDamageIncrease += 300;
                        }
                        var SecoundCollection = Owner.Collection.items.Values.Where(p => p.ID >= 4320000 && p.ID <= 4320002).ToArray();
                        if (SecoundCollection.Length >= 1)
                        {
                            Owner.Player.AddFlag((MsgUpdate.Flags)439, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                            Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)439, (uint)Role.StatusFlagsBigVector32.PermanentFlag, (uint)5, (uint)5, MsgUpdate.DataType.ArchiveSkill);
                        }
                        else
                        {
                            if (Owner.Player.ContainFlag((MsgUpdate.Flags)439))
                                Owner.Player.RemoveFlag((MsgUpdate.Flags)439);
                        }

                    }
                }
                #endregion
                #region WildDash
                if (Owner.Player.ContainFlag((MsgUpdate.Flags)442))
                {

                    this.Owner.Status.MaxHitpoints += (uint)200;

                    this.Owner.Status.MaxAttack += (uint)150;

                    this.Owner.Status.MinAttack += (uint)150;

                }
                #endregion
                AddRuneAtributes();
                #region WildDash
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.WildDash) && Owner.Status.MaxHitpoints < 85000)
                {
                    MsgSpell WildDash = null;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WildDash, out WildDash))
                    {
                        Database.MagicType.Magic DBSpell = Pool.Magic[WildDash.ID][WildDash.Level];

                        this.Owner.Status.MaxHitpoints += (uint)DBSpell.DamageOnMonster;
                        if (Owner.Status.MaxHitpoints > 85000)
                            Owner.Status.MaxHitpoints = 85000;
                    }

                }
                #endregion
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.CrackMantra1))
                {
                    Owner.Status.Defence -= Owner.Player.CrackMantra1;

                }
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.CrackMantra2))
                {
                    Owner.Status.MagicDefence -= Owner.Player.CrackMantra2;
                }
                MsgGameItem items;
                if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out items))
                {
                    bool twohand = Database.ItemType.IsTwoHand(items.ITEM_ID);
                    if (twohand && Owner.Player.LeftWeaponId == 0 || Database.ItemType.IsBacksword(items.ITEM_ID) || Database.ItemType.IsTaoistEpicWeapon(items.ITEM_ID) && Owner.Player.LeftWeaponId == 0)
                    {
                        Owner.PrestigeLevel += (items.PerfectionLevel);
                    }
                }
                Owner.Status.PrestigeLevel = Owner.PrestigeLevel;


                #region Eight-Span Mirror Susano Ninja Archive
                if (Owner.MySpells != null &&
                    Owner.MySpells.ClientSpells != null &&
                    Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EightSpanMirror))
                {
                    var spell = Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.EightSpanMirror];

                    if (Pool.Magic.ContainsKey((ushort)Role.Flags.SpellID.EightSpanMirror) &&
                        Pool.Magic[(ushort)Role.Flags.SpellID.EightSpanMirror].ContainsKey(spell.Level))
                    {
                        if (Owner.Status != null && Owner.Status.Damage < 55)
                        {
                            int percent = 0;

                            switch (spell.Level)
                            {
                                case 1: percent = 6; break;
                                case 2: percent = 8; break;
                                case 3: percent = 10; break;
                                case 4: percent = 12; break;
                                case 5: percent = 14; break;
                            }

                            int cap = 51 + (spell.Level - 1);
                            if (percent > cap)
                                percent = cap;

                            Owner.Status.Damage = (uint)Math.Max(Owner.Status.Damage - percent, 0);

                            Owner.SendSysMesage(string.Format(
"You activated Eight-Span Mirror (Lv.{0}) - Tortoise Effect +{1}%",
spell.Level, percent));
                        }
                    }
                }
                #endregion


                #region Ten-Fist Sword Susano Ninja Archive
                if (Owner.MySpells != null &&
                    Owner.MySpells.ClientSpells != null &&
                    Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TenFistSword))
                {
                    var spell = Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.TenFistSword];

                    if (Pool.Magic.ContainsKey((ushort)Role.Flags.SpellID.TenFistSword) &&
                        Pool.Magic[(ushort)Role.Flags.SpellID.TenFistSword].ContainsKey(spell.Level))
                    {
                        if (Owner.Status != null && Owner.Status.Damage < 55)
                        {
                            int ignore = spell.Level;

                            int finalDamage = (int)Owner.Status.Damage - ignore;

                            if (finalDamage < 50)
                                finalDamage = 50;

                            Owner.Status.Damage = (uint)finalDamage;

                            Owner.SendSysMesage(string.Format(
"You activated Ten-Fist Sword (Lv.{0}) - Ignore Tortoise Effect {1}% (Min 50% Cap)",
spell.Level, ignore));
                        }
                    }
                }
                #endregion


                #region GodDmgMode - Safe God Stats Boost (GM ONLY)
                if (Owner.GodDmgMode && Owner.IsGm)
                {
                    // Use int.MaxValue as safe cap to avoid uint overflow → negative HP death loop
                    const uint GOD_BOOST = 1000000000u;
                    const uint SAFE_MAX = 1100000000u; // stays within int range safely

                    Owner.Status.MaxHitpoints = Math.Min(Owner.Status.MaxHitpoints + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MaxMana = Math.Min(Owner.Status.MaxMana + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MaxAttack = Math.Min(Owner.Status.MaxAttack + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MinAttack = Math.Min(Owner.Status.MinAttack + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Defence = Math.Min(Owner.Status.Defence + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MagicAttack = Math.Min(Owner.Status.MagicAttack + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MagicDefence = Math.Min(Owner.Status.MagicDefence + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MDefence = Math.Min(Owner.Status.MDefence + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Immunity = Math.Min(Owner.Status.Immunity + GOD_BOOST, SAFE_MAX);
                    Owner.Status.CriticalStrike = Math.Min(Owner.Status.CriticalStrike + GOD_BOOST, SAFE_MAX);
                    Owner.Status.SkillCStrike = Math.Min(Owner.Status.SkillCStrike + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Penetration = Math.Min(Owner.Status.Penetration + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Block = Math.Min(Owner.Status.Block + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Breakthrough = Math.Min(Owner.Status.Breakthrough + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Counteraction = Math.Min(Owner.Status.Counteraction + GOD_BOOST, SAFE_MAX);
                    Owner.Status.PhysicalDamageIncrease = Math.Min(Owner.Status.PhysicalDamageIncrease + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MagicDamageIncrease = Math.Min(Owner.Status.MagicDamageIncrease + GOD_BOOST, SAFE_MAX);
                    Owner.Status.PhysicalDamageDecrease = Math.Min(Owner.Status.PhysicalDamageDecrease + GOD_BOOST, SAFE_MAX);
                    Owner.Status.MagicDamageDecrease = Math.Min(Owner.Status.MagicDamageDecrease + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Dodge = Math.Min(Owner.Status.Dodge + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Accuracy = Math.Min(Owner.Status.Accuracy + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Damage = Math.Min(Owner.Status.Damage + GOD_BOOST, SAFE_MAX);
                    Owner.Status.LuckyStrike = Math.Min(Owner.Status.LuckyStrike + GOD_BOOST, SAFE_MAX);
                    Owner.Status.DashRate = Math.Min(Owner.Status.DashRate + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Parry = Math.Min(Owner.Status.Parry + GOD_BOOST, SAFE_MAX);
                    Owner.Status.Resist = Math.Min(Owner.Status.Resist + GOD_BOOST, SAFE_MAX);

                    // Clamp HitPoints safely within int range — prevents negative HP death loop
                    Owner.Player.HitPoints = (int)Math.Min(Owner.Status.MaxHitpoints, (uint)int.MaxValue);
                }
                else
                {
                    // When GodDmgMode is OFF, ensure HitPoints doesn't exceed natural MaxHitpoints
                    // (prevents leftover overflow HP from previous GodDmgMode session)
                    if (Owner.Player.HitPoints > (int)Owner.Status.MaxHitpoints && Owner.Status.MaxHitpoints > 0)
                        Owner.Player.HitPoints = (int)Owner.Status.MaxHitpoints;
                }
                #endregion

                Owner.Send(stream.StatusCreate(Owner.Status));
                if (Database.ItemType.IsArcherEpicWeapon(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarburstArrows))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarburstArrows, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrescentShadow))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrescentShadow, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NebulousHunt))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NebulousHunt, 0, 0);
                }
                if (Database.ItemType.IsMonkEpicWeapon(LeftWeapon) || Database.ItemType.IsMonkEpicWeapon(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UpSweep))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.UpSweep, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DownSweep))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DownSweep, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Strike))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Strike, 0, 0);
                }
                if (Database.ItemType.IsPirateEpicWeapon(LeftWeapon) || Database.ItemType.IsPirateEpicWeapon(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LeftChop))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LeftChop, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RightChop))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RightChop, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Gunfire))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Gunfire, 0, 0);
                }
                if (Database.ItemType.IsWarriorEpicWeapons(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LeftHook))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LeftHook, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RightHook))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RightHook, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StraightFist))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StraightFist, 0, 0);
                }

                if (Owner.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                {
                    var DBSpells = Pool.Magic[(ushort)Role.Flags.SpellID.FreezingPelter];
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FreezingPelter, out clientspell))
                    {
                        var DBSpell = DBSpells[(ushort)Math.Min(clientspell.Level, DBSpells.Count - 1)];
                        Owner.Status.MinAttack += (uint)(WeaponsMinAttack * DBSpell.Damage2 / 100);
                    }
                }


            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public void AddChiAtribute(Role.Instance.Chi chi, uint percent = 100)
        {

            Owner.Status.CriticalStrike += chi.CriticalStrike * percent / 100;
            Owner.Status.SkillCStrike += chi.SkillCriticalStrike * percent / 100;
            Owner.Status.Immunity += chi.Immunity * percent / 100;
            Owner.Status.Counteraction += chi.Counteraction * percent / 100;
            Owner.Status.Breakthrough += chi.Breakthrough * percent / 100;
            Owner.Status.MaxHitpoints += chi.MaxLife * percent / 100;
            Owner.Status.MaxAttack += chi.AddAttack * percent / 100;
            Owner.Status.MinAttack += chi.AddAttack * percent / 100;
            Owner.Status.MagicAttack += chi.AddMagicAttack * percent / 100;
            Owner.Status.MagicDefence += chi.AddMagicDefense * percent / 100;

            Owner.Status.PhysicalDamageIncrease += chi.FinalAttack * percent / 100;
            Owner.Status.PhysicalDamageDecrease += chi.FinalDefense * percent / 100;
            Owner.Status.MagicDamageIncrease += chi.FinalMagicAttack * percent / 100;
            Owner.Status.MagicDamageDecrease += chi.FinalMagicDefense * percent / 100;


        }

        public void AddProfessionAtributes()
        {
            if (this.Owner.Fake)
                return;
            if (!Database.ProfessionTable.Benefits.ContainsKey(Owner.Player.Class)) return;
            var attributes = Database.ProfessionTable.Benefits[Owner.Player.Class];
            foreach (var a in attributes.AttributeValue.Where(i => i.Type != MsgChiInfo.ChiAttribute.None))
            {
                switch (a.Type)
                {
                    case MsgChiInfo.ChiAttribute.HPAdd:
                        Owner.Status.MaxHitpoints += a.Value;
                        break;
                    case MsgChiInfo.ChiAttribute.AddAttack:
                        {
                            Owner.Status.MinAttack += a.Value;
                            Owner.Status.MaxAttack += a.Value;
                            break;
                        }
                    case MsgChiInfo.ChiAttribute.AddMagicAttack: Owner.Status.MagicAttack += a.Value; break;
                    case MsgChiInfo.ChiAttribute.Breakthrough: Owner.Status.Breakthrough += (uint)(a.Value / 10); break;
                    case MsgChiInfo.ChiAttribute.Counteraction: Owner.Status.Counteraction += (uint)(a.Value / 10); break;
                    case MsgChiInfo.ChiAttribute.CriticalStrike: Owner.Status.CriticalStrike += a.Value; break;
                    case MsgChiInfo.ChiAttribute.Immunity: Owner.Status.Immunity += a.Value; break;
                    case MsgChiInfo.ChiAttribute.SkillCriticalStrike: Owner.Status.SkillCStrike += a.Value; break;
                }
            }
            byte prestigeRank = (byte)Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(Owner.Player.Class), Owner.Player.UID);
            if (Database.ProfessionTable.TitleBenefits.Count(i => i.Value.Class == Owner.Player.Class / 1000 && i.Value.Rank == prestigeRank) > 0)
            {
                var titleAttributes = Database.ProfessionTable.TitleBenefits.Values.Where(i => i.Class == Owner.Player.Class / 1000 && i.Rank == prestigeRank).FirstOrDefault();
                foreach (var a in titleAttributes.AttributeValue.Where(i => i.Type != MsgChiInfo.ChiAttribute.None))
                {
                    switch (a.Type)
                    {
                        case MsgChiInfo.ChiAttribute.HPAdd:
                            Owner.Status.MaxHitpoints += a.Value;
                            break;
                        case MsgChiInfo.ChiAttribute.AddAttack:
                            {
                                Owner.Status.MinAttack += a.Value;
                                Owner.Status.MaxAttack += a.Value;
                                break;
                            }
                        case MsgChiInfo.ChiAttribute.AddMagicAttack: Owner.Status.MagicAttack += a.Value; break;
                        case MsgChiInfo.ChiAttribute.Breakthrough: Owner.Status.Breakthrough += (uint)(a.Value / 10); break;
                        case MsgChiInfo.ChiAttribute.Counteraction: Owner.Status.Counteraction += (uint)(a.Value / 10); break;
                        case MsgChiInfo.ChiAttribute.CriticalStrike: Owner.Status.CriticalStrike += a.Value; break;
                        case MsgChiInfo.ChiAttribute.Immunity: Owner.Status.Immunity += a.Value; break;
                        case MsgChiInfo.ChiAttribute.SkillCriticalStrike: Owner.Status.SkillCStrike += a.Value; break;
                    }
                }
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Owner.Send(stream.CreateProfLevUp(new MsgProfLevUp.MsgProfLevUpProto() { Type = 1, Rank = prestigeRank }));
            }
        }

        public void AddRuneAtributes()
        {

            var All = Database.RuneTable.Attributes.Values.Where(p => p.Score <= Owner.Rune.Score && p.Score >= Database.RuneTable.Attributes.Values.Where(o => o.Score <= Owner.Rune.Score).OrderByDescending(o => o.Score).FirstOrDefault().Score).ToArray();
            foreach (var Attr in All)
            {
                switch (Attr.Type)
                {
                    case Database.RuneTable.RuneAttribute.HPAdd: Owner.Status.MaxHitpoints += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.AddAttack: Owner.Status.MaxAttack += Attr.Value; Owner.Status.MinAttack += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.AddMagicAttack: Owner.Status.MagicAttack += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.SkillCriticalStrike: Owner.Status.CriticalStrike += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.CriticalStrike: Owner.Status.SkillCStrike += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.Immunity: Owner.Status.Immunity += Attr.Value; break;
                    case Database.RuneTable.RuneAttribute.Breakthrough: Owner.Status.Breakthrough += Attr.Value / 10; break;
                    case Database.RuneTable.RuneAttribute.Counteraction: Owner.Status.Counteraction += Attr.Value / 10; break;
                }
            }
            #region BlueRune(Atributes)
            #region Rampage
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Rampage) && Owner.Rune.IsEquipped("Rampage"))
            {
                if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Rampage))
                {
                    uint MaxHP = (uint)((Owner.Status.MaxHitpoints * 250) / 100);
                    Owner.Status.MaxHitpoints = MaxHP;

                }
            }
            #endregion
            #region All-round
            byte itemLevel = 0;
            if (Owner.Rune.IsEquipped("All-round", ref itemLevel)
                && Database.AtributesStatus.IsWarrior(Owner.Player.Class)
                && (Database.ItemType.IsShield(Owner.Equipment.LeftWeapon)))
            {
                uint PhysicalAttack = 0;
                uint PhysicalDefense = 0;
                uint MagicDefense = 0;
                switch (itemLevel)
                {
                    case 1: PhysicalAttack = 300; PhysicalDefense = 3000; MagicDefense = 500; break;
                    case 2: PhysicalAttack = 350; PhysicalDefense = 3100; MagicDefense = 550; break;
                    case 3: PhysicalAttack = 400; PhysicalDefense = 3200; MagicDefense = 600; break;
                    case 4: PhysicalAttack = 450; PhysicalDefense = 3300; MagicDefense = 650; break;
                    case 5: PhysicalAttack = 500; PhysicalDefense = 3400; MagicDefense = 700; break;
                    case 6: PhysicalAttack = 550; PhysicalDefense = 3500; MagicDefense = 750; break;
                    case 7: PhysicalAttack = 600; PhysicalDefense = 3600; MagicDefense = 800; break;
                    case 8: PhysicalAttack = 650; PhysicalDefense = 3700; MagicDefense = 850; break;
                    case 9: PhysicalAttack = 700; PhysicalDefense = 3800; MagicDefense = 900; break;
                    case 10: PhysicalAttack = 750; PhysicalDefense = 3900; MagicDefense = 950; break;
                    case 11: PhysicalAttack = 800; PhysicalDefense = 4000; MagicDefense = 1000; break;
                    case 12: PhysicalAttack = 850; PhysicalDefense = 4100; MagicDefense = 1050; break;
                    case 13: PhysicalAttack = 900; PhysicalDefense = 4200; MagicDefense = 1100; break;
                    case 14: PhysicalAttack = 950; PhysicalDefense = 4300; MagicDefense = 1150; break;
                    case 15: PhysicalAttack = 1000; PhysicalDefense = 4400; MagicDefense = 1200; break;
                    case 16: PhysicalAttack = 1100; PhysicalDefense = 4500; MagicDefense = 1300; break;
                    case 17: PhysicalAttack = 1200; PhysicalDefense = 5000; MagicDefense = 1400; break;
                    case 18: PhysicalAttack = 1300; PhysicalDefense = 5500; MagicDefense = 1500; break;
                    case 19: PhysicalAttack = 1400; PhysicalDefense = 6000; MagicDefense = 1600; break;
                    case 20: PhysicalAttack = 1500; PhysicalDefense = 6500; MagicDefense = 1800; break;
                    case 21: PhysicalAttack = 1600; PhysicalDefense = 7000; MagicDefense = 2000; break;
                    case 22: PhysicalAttack = 1800; PhysicalDefense = 7500; MagicDefense = 2500; break;
                    case 23: PhysicalAttack = 2000; PhysicalDefense = 8000; MagicDefense = 3000; break;
                    case 24: PhysicalAttack = 2200; PhysicalDefense = 8500; MagicDefense = 3500; break;
                    case 25: PhysicalAttack = 2500; PhysicalDefense = 9000; MagicDefense = 4000; break;
                    case 26: PhysicalAttack = 2700; PhysicalDefense = 9500; MagicDefense = 4500; break;
                    case 27: PhysicalAttack = 3000; PhysicalDefense = 10000; MagicDefense = 5000; break;
                }
                Owner.Status.MinAttack += PhysicalAttack;
                Owner.Status.MaxAttack += PhysicalAttack;
                Owner.Status.Defence += PhysicalDefense;
                Owner.Status.MagicDefence += MagicDefense;
            }
            #endregion
            #region FineRain
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.FineRain1))
            {
                Owner.Status.MaxHitpoints += Owner.Player.FineRainPower;
            }
            #endregion
            #region RiseofTaoism
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.RiseofTaoism) && Owner.Rune.IsEquipped("RiseofTaoism"))
            {
                Owner.Status.MaxHitpoints += 50000;
            }
            #endregion
            #region FuryStrike
            if (Role.Core.GetDistance(Owner.Player.X, Owner.Player.Y, Owner.Player.X, Owner.Player.Y) < 3)
            {
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.IronGuard) && Owner.Rune.IsEquipped("FuryStrike"))
                {
                    Owner.Status.MaxHitpoints += Owner.Player.FuryStrikeHP;

                    Owner.Status.MaxAttack += (uint)((double)Owner.Status.MaxAttack * (double)Owner.Player.FuryStrikeAttack / 100d);
                    Owner.Status.MinAttack += (uint)((double)Owner.Status.MinAttack * (double)Owner.Player.FuryStrikeAttack / 100d);
                    Owner.Status.MagicAttack += (uint)((double)Owner.Status.MagicAttack * (double)Owner.Player.FuryStrikeAttack / 100d);
                }
            }
            #endregion
            #region BloodTide
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.BloodTide) && Owner.Rune.IsEquipped("BloodTide"))
            {
                Owner.Status.MaxHitpoints += Owner.Player.BloodTideHP;
            }

            #endregion
            #endregion

            #region Wonder(Rune Skill)
            byte itemLevel2 = 0;
            if (Owner.Rune.IsEquipped("Wonder", ref itemLevel2) || Owner.Rune.IsEquipped("Supreme Armor", ref itemLevel2) && Owner.Status.Block < 3000)
            {
                uint Block = 0;
                switch (itemLevel2)
                {
                    case 1: Block = 1000; break;
                    case 2: Block = 1300; break;
                    case 3: Block = 1500; break;
                    case 4: Block = 1600; break;
                    case 5: Block = 1800; break;
                    case 6: Block = 2000; break;
                    case 7: Block = 2200; break;
                    case 8: Block = 2500; break;
                    case 9: Block = 3000; break;
                }
                Owner.Status.Block += Block;
            }
            #endregion




            #region Nature'sChant(Rune Skill)    >> The Rune Edit By Mohy ElDin <<

            foreach (var prof in Owner.MyProfs.ClientProf.Values)
            {
                if (prof.ID == (uint)Database.MagicType.WeaponsType.Backsword || prof.ID == (uint)Database.MagicType.WeaponsType.MagicSword || prof.ID == (uint)Database.MagicType.WeaponsType.EpicTao)
                {
                    byte itemLevel5 = 0;
                    if (Owner.Rune.IsEquipped("Nature`sChant", ref itemLevel5) || Owner.Rune.IsEquipped("Cosmic Balance", ref itemLevel5) && prof.Level != 0 && prof.Level >= 13)
                    {
                        uint AddMagicAttack = 0;
                        switch (itemLevel5)
                        {
                            case 1: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 10d / 250d); break;
                            case 2: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 11d / 250d); break;
                            case 3: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 12d / 250d); break;
                            case 4: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 13d / 250d); break;
                            case 5: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 14d / 250d); break;
                            case 6: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 15d / 250d); break;
                            case 7: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 16d / 250d); break;
                            case 8: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 18d / 250d); break;
                            case 9: AddMagicAttack = (uint)((double)Owner.Status.MagicAttack * 20d / 250d); break;
                        }
                        Owner.Status.MagicAttack += AddMagicAttack;
                    }
                }
            }
            #endregion


            #region Justice Guard Rune

            byte JusticeLevel = 0;

            if (Owner.Rune != null &&
                Owner.Rune.IsEquipped("JusticeGuard", ref JusticeLevel))
            {
                int percent = 0;

                switch (JusticeLevel)
                {
                    case 1: percent = 5; break;
                    case 2: percent = 6; break;
                    case 3: percent = 7; break;
                    case 4: percent = 8; break;
                    case 5: percent = 9; break;
                    case 6: percent = 10; break;
                    case 7: percent = 12; break;
                    case 8: percent = 15; break;
                    case 9: percent = 25; break;
                }

                if (Owner.Status != null)
                {
                    Owner.Status.MagicDefence += (uint)(Owner.Status.MagicDefence * percent / 50);

                    Owner.SendSysMesage(
                        "You activated Justice Guard (Lv.{JusticeLevel}) - M-Defense +{percent}%");
                }
            }

            #endregion

            #region Front Break Rune
            byte FrontBreakLevel = 0;
            if (Owner != null &&
                Owner.Rune != null &&
                Owner.Rune.IsEquipped("FrontBreak", ref FrontBreakLevel))
            {
                int addBreak = 0;
                switch (FrontBreakLevel)
                {
                    case 1: addBreak = 1; break;   // Level ( 1 )
                    case 2: addBreak = 2; break;   // Level ( 2 )
                    case 3: addBreak = 3; break;   // Level ( 3 )
                    case 4: addBreak = 4; break;   // Level ( 4 )
                    case 5: addBreak = 5; break;   // Level ( 5 )
                    case 6: addBreak = 6; break;   // Level ( 6 )
                    case 7: addBreak = 7; break;   // Level ( 7 )
                    case 8: addBreak = 8; break;   // Level ( 8 )
                    case 9: addBreak = 10; break;  // Level ( 9 )
                }
                Owner.Status.Breakthrough += (uint)addBreak;
                Owner.SendSysMesage(string.Format(
     "You activated Front Break (Lv.{0}) - Break Chance +{1}% (10s)",
     FrontBreakLevel,
     addBreak));
            }
            #endregion
            #region SurgingForce
            byte SurgingForceL = 0;
            ushort INCP = 0;
            if (Owner.Rune.IsEquipped("SurgingForce", ref SurgingForceL))
            {
                switch (SurgingForceL)
                {
                    case 1: INCP = 500; break;
                    case 2: INCP = 600; break;
                    case 3: INCP = 700; break;
                    case 4: INCP = 800; break;
                    case 5: INCP = 900; break;
                    case 6: INCP = 1000; break;
                    case 7: INCP = 1200; break;
                    case 8: INCP = 1500; break;
                    case 9: INCP = 2000; break;
                }
                Owner.Status.Penetration += INCP;
            }
            #endregion
            #region EnormousCrusher(Rune Skill)
            byte EnormousCrusherL = 0;
            if (Owner.Rune.IsEquipped("EnormousCrusher", ref EnormousCrusherL))
            {
                switch (EnormousCrusherL)
                {
                    case 1: Owner.PerfectionStatus.ShieldBreak += 1; break;
                    case 2: Owner.PerfectionStatus.ShieldBreak += 2; break;
                    case 3: Owner.PerfectionStatus.ShieldBreak += 3; break;
                    case 4: Owner.PerfectionStatus.ShieldBreak += 4; break;
                    case 5: Owner.PerfectionStatus.ShieldBreak += 5; break;
                    case 6: Owner.PerfectionStatus.ShieldBreak += 6; break;
                    case 7: Owner.PerfectionStatus.ShieldBreak += 7; break;
                    case 8: Owner.PerfectionStatus.ShieldBreak += 8; break;
                    case 9: Owner.PerfectionStatus.ShieldBreak += 9; break;
                }

            }
            #endregion

            #region Impregnability Rune

            byte ImpregnabilityLevel = 0;
            int chance = 0;
            int addAntiBreak = 0;

            if (Owner != null &&
                Owner.Rune != null &&
                Owner.Rune.IsEquipped("Impregnability", ref ImpregnabilityLevel))
            {
                switch (ImpregnabilityLevel)
                {
                    case 1: chance = 10; addAntiBreak = 30; break;
                    case 2: chance = 11; addAntiBreak = 32; break;
                    case 3: chance = 12; addAntiBreak = 34; break;
                    case 4: chance = 13; addAntiBreak = 36; break;
                    case 5: chance = 14; addAntiBreak = 38; break;
                    case 6: chance = 15; addAntiBreak = 40; break;
                    case 7: chance = 16; addAntiBreak = 42; break;
                    case 8: chance = 18; addAntiBreak = 45; break;
                    case 9: chance = 20; addAntiBreak = 50; break;
                }

                if (Role.Core.Rate(chance))
                {
                    int duration = 10;
                    if (Owner.ImpregnabilityEndTime > DateTime.Now)
                    {
                        Owner.Status.Counteraction -= Owner.ImpregnabilityValue;
                        if (Owner.Status.Counteraction < 0)
                            Owner.Status.Counteraction = 0;
                    }

                    Owner.Status.Counteraction += (uint)addAntiBreak;

                    Owner.ImpregnabilityValue = (uint)addAntiBreak;
                    Owner.ImpregnabilityEndTime = DateTime.Now.AddSeconds(duration);

                    Owner.SendSysMesage(string.Format(
                        "Impregnability activated (Lv.{0}) - +{1}% Anti-Break for 10s!",
                        ImpregnabilityLevel,
                        addAntiBreak));
                }
            }

            #endregion
            #region DeadlySight

            itemLevel = 0;

            if (Owner.Rune.IsEquipped("DeadlySight", ref itemLevel))
            {
                uint[] AccuracyBonus =
    {
        0,
        50,
        60,
        70,
        90,
        100,
        120,
        140,
        150,
        200
    };

                if (itemLevel >= 1 && itemLevel <= 9)
                {
                    Owner.Status.Accuracy += AccuracyBonus[itemLevel];

                    Owner.SendSysMesage(string.Format(
                        "You activated Deadly Sight (Lv.{0}) - Accuracy +{1}",
                        itemLevel,
                        AccuracyBonus[itemLevel]));
                }
            }

            #endregion
            #region RedRune
            #region Wonder
            if (Owner.Rune.IsEquipped("Sacrifice")
               && !Database.ItemType.IsKnife(Owner.Player.RightWeaponId) && !Database.ItemType.IsKnife(Owner.Player.LeftWeaponId)
                && Owner.Player.ContainFlag(MsgUpdate.Flags.Sacrifice)
               )
            {

                double FinalPAttackInc = (uint)(Owner.Status.MaxHitpoints * 40) / 100;
                Owner.Status.MaxHitpoints -= (uint)FinalPAttackInc;
                FinalPAttackInc = FinalPAttackInc * 1.3;
                Owner.Status.PhysicalDamageIncrease += (uint)FinalPAttackInc;
            }
            #endregion
            #endregion
            #region GrowFromHurt
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.GrowFromHurt))
            {
                Owner.Status.MaxHitpoints += Owner.Player.GrowFromHurtHitpoints;
                Owner.Player.HitPoints += (int)Owner.Player.GrowFromHurtHitpoints;
            }
            #endregion
            try
            {
                #region Ideals Runes
                if (Owner.Rune.IdealCollection("Supreme Armor"))
                {
                    Owner.Status.Immunity += 100;
                }
                if (Owner.Rune.IdealCollection("Indestructible Balance"))
                {
                    Owner.Status.CriticalStrike += 100;
                    Owner.Status.SkillCStrike += 100;
                }
                if (Owner.Rune.IdealCollection("Shadow Flow"))
                {
                    Owner.Status.Immunity += 100;
                }
                if (Owner.Rune.IdealCollection("Tough Steel"))
                {
                    Owner.Status.MaxHitpoints += 300;
                }
                if (Owner.Rune.IdealCollection("Unbreakable"))
                {
                    Owner.Status.Immunity += 100;
                }
                if (Owner.Rune.IdealCollection("Cosmic Balance"))
                {
                    Owner.Status.PhysicalDamageIncrease += 500;
                    Owner.Status.MagicDamageIncrease += 500;
                }
                if (Owner.Rune.IdealCollection("Invincible Resolve"))
                {
                    Owner.Status.PhysicalDamageDecrease += 500;
                }
                if (Owner.Rune.IdealCollection("Tortoise Predator"))
                {
                    Owner.Status.MaxHitpoints += 300;
                    Owner.Status.MaxAttack += 100;
                }
                if (Owner.Rune.IdealCollection("Honed Soul"))
                {
                    Owner.Status.Immunity += 100;
                    Owner.Status.CriticalStrike += 100;
                }
                if (Owner.Rune.IdealCollection("Battle Reaper"))
                {
                    Owner.Status.Immunity += 100;
                }
                if (Owner.Rune.IdealCollection("XP Devourer"))
                {
                    Owner.Status.CriticalStrike += 100;
                    Owner.Status.SkillCStrike += 100;
                }
                if (Owner.Rune.IdealCollection("Deathhold"))
                {
                    Owner.Status.CriticalStrike += 100;
                    Owner.Status.SkillCStrike += 100;
                }
                if (Owner.Rune.IdealCollection("Peakcarver"))
                {
                    Owner.Status.Defence += 500;
                }
                if (Owner.Rune.IdealCollection("Soulrazor"))
                {
                    Owner.Status.MaxHitpoints += 300;
                }
                if (Owner.Rune.IdealCollection("Sanctified Pierce"))
                {
                    Owner.Status.PhysicalDamageDecrease += 500;
                }
                #endregion
            }
            catch
            {
            }

        }

        public void AddBeastsAtributes()
        {
            if (this.Owner.Fake)
                return;
            if (!Owner.Beasts.Activated) return;
            var attributes = Database.BeastsAtrribute.Attributes[Owner.Beasts.Level];
            Owner.Status.MaxHitpoints += attributes.HPAdd;

            Owner.Status.MinAttack += attributes.PAttack;
            Owner.Status.MaxAttack += attributes.PAttack;

            Owner.Status.MagicAttack += attributes.MAttack;
            Owner.Status.Defence += attributes.PDefense;
            Owner.Status.MagicDefence += attributes.MDefense;
            Owner.Status.MagicDamageIncrease += (ushort)attributes.FinalMDmgDealt;
            Owner.Status.PhysicalDamageIncrease += (ushort)attributes.FinalPDmgDealt;
            Owner.Status.PhysicalDamageDecrease += (ushort)attributes.FinalPDmgReceived;
            Owner.Status.MagicDamageDecrease += (ushort)attributes.FinalMDmgReceived;
        }

        public void AddGuildSkillStats(Client.GameClient client)
        {


            if (Owner.Player.MyGuild != null && Owner.Player.MyGuildMember != null && Owner.Player.Map == 1038)
            {
                foreach (var skill in Owner.GuildSkill.Skills.Values)
                {
                    cq_syn_skill_type.skill_type skill_type;
                    if (cq_syn_skill_type.TryGetValue(skill.ID, skill.Level, out skill_type))
                    {
                        switch ((cq_syn_skill_type.Type)skill.ID)
                        {
                            case cq_syn_skill_type.Type.MaxHP: Owner.Status.MaxHitpoints += skill_type.data1; break;
                            case cq_syn_skill_type.Type.PAttack:
                                {
                                    Owner.Status.MaxAttack += skill_type.data1;
                                    Owner.Status.MinAttack += skill_type.data1;
                                    break;
                                }
                            case cq_syn_skill_type.Type.PDefense: Owner.Status.Defence += skill_type.data1; break;
                            case cq_syn_skill_type.Type.MAttack: Owner.Status.MagicAttack += skill_type.data1; break;
                            case cq_syn_skill_type.Type.MDefense: Owner.Status.MagicDefence += skill_type.data1; break;
                            case cq_syn_skill_type.Type.FinalPAttack: Owner.Status.PhysicalDamageIncrease += skill_type.data1; break;
                            case cq_syn_skill_type.Type.FinalPDamage: Owner.Status.PhysicalDamageDecrease += skill_type.data1; break;
                            case cq_syn_skill_type.Type.FinalMAttack: Owner.Status.MagicDamageIncrease += skill_type.data1; break;
                            case cq_syn_skill_type.Type.FinalMDamage: Owner.Status.MagicDamageDecrease += skill_type.data1; break;
                            case cq_syn_skill_type.Type.PStrike: Owner.Status.CriticalStrike += skill_type.data1; break;
                            case cq_syn_skill_type.Type.MStrike: Owner.Status.SkillCStrike += skill_type.data1; break;
                            case cq_syn_skill_type.Type.Immunity: Owner.Status.Immunity += skill_type.data1; break;
                            case cq_syn_skill_type.Type.Break: Owner.Status.Breakthrough += skill_type.data1 / 10; break;
                            case cq_syn_skill_type.Type.AntiBreak: Owner.Status.Counteraction += skill_type.data1 / 10; break;
                        }
                    }
                }
            }

        }

        public void AddPerfectionAtributes()
        {

            foreach (var val in Database.ItemRefineUpgrade.EffectsEX.Values)
            {
                if (Exist(p => p.ITEM_ID == val.ItemID))
                {
                    if (Owner.PrestigeLevel >= val.ReqLevel)
                    {
                        switch (val.RefineType)
                        {
                            case (uint)MsgRefineEffect.RefineEffects.ToxinEraserLevel:
                                Owner.PerfectionStatus.ToxinEraser += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.AbsoluteLuck:
                                Owner.PerfectionStatus.AbsoluteLuck += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.BloodSpawn:
                                Owner.PerfectionStatus.BloodSpawn += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.CalmWind:
                                Owner.PerfectionStatus.CalmWind += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.CoreStrike:
                                Owner.PerfectionStatus.CoreStrike += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.DivineGuard:
                                Owner.PerfectionStatus.DivineGuard += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.DrainingTouch:
                                Owner.PerfectionStatus.DrainingTouch += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.FreeSoul:
                                Owner.PerfectionStatus.FreeSoul += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.InvisbleArrow:
                                Owner.PerfectionStatus.InvisibleArrow += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.KillingFlash:
                                Owner.PerfectionStatus.KillingFlash += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.LightOfStamina:
                                Owner.PerfectionStatus.LightOfStamina += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.LuckyStrike:
                                Owner.PerfectionStatus.LuckyStrike += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.MirrorOfSin:
                                Owner.PerfectionStatus.MirrorOfSin += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.ShiledBreak:
                                Owner.PerfectionStatus.ShieldBreak += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.StraightLife:
                                Owner.PerfectionStatus.StraightLife += (int)val.RefineValue;
                                break;
                            case (uint)MsgRefineEffect.RefineEffects.StrikeLockLevel:
                                Owner.PerfectionStatus.StrikeLock += (int)val.RefineValue;
                                break;
                            case 18:
                                Owner.Status.MinAttack += val.RefineValue;
                                Owner.Status.MaxAttack += val.RefineValue;
                                break;
                            case 19:
                                Owner.Status.Defence += (ushort)val.RefineValue;
                                break;
                            case 22:
                                Owner.Status.MaxHitpoints += val.RefineValue;
                                break;
                        }
                    }
                }
            }
        }

        public void AddInnerPowerAtributes(Role.Instance.InnerPower Inner, uint percent = 100)
        {

            Owner.Status.CriticalStrike += Inner.CriticalStrike * percent / 100;
            Owner.Status.SkillCStrike += Inner.SkillCriticalStrike * percent / 100;
            Owner.Status.Immunity += Inner.Immunity * percent / 100;
            Owner.Status.Counteraction += Inner.Counteraction / 10;
            Owner.Status.Breakthrough += Inner.Breakthrough / 10;
            Owner.Status.MaxHitpoints += Inner.MaxLife * percent / 100;
            Owner.Status.MaxAttack += Inner.AddAttack * percent / 100;
            Owner.Status.MinAttack += Inner.AddAttack * percent / 100;
            Owner.Status.MagicAttack += Inner.AddMagicAttack * percent / 100;
            Owner.Status.MagicDefence += Inner.AddMagicDefense * percent / 100;

            Owner.Status.PhysicalDamageIncrease += Inner.FinalAttack * percent / 100;
            Owner.Status.PhysicalDamageDecrease += Inner.FinalDefense * percent / 100;
            Owner.Status.MagicDamageIncrease += Inner.FinalMagicAttack * percent / 100;
            Owner.Status.MagicDamageDecrease += Inner.FinalMagicDefense * percent / 100;



            Owner.Status.Defence += Inner.Defence * percent / 100;
        }

        public unsafe void SendMentorShare(ServerSockets.Packet stream)
        {

            Game.MsgServer.MsgApprenticeInformation Information = Game.MsgServer.MsgApprenticeInformation.Create();
            Information.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
            if (Owner.Player.MyMentor != null)
            {
                if (Owner.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Mentor))
                {
                    if (Owner.Player.MyMentor.MyClient != null)
                    {
                        if (Owner.Player.Associate.Associat[Role.Instance.Associate.Mentor].ContainsKey(Owner.Player.MyMentor.MyUID))
                        {
                            Role.Player mentor = Owner.Player.MyMentor.MyClient.Player;
                            Owner.Player.SetMentorBattlePowers(mentor.GetShareBattlePowers((uint)Owner.Player.RealBattlePower), (uint)mentor.RealBattlePower);

                            Information.Mentor_ID = mentor.UID;
                            Information.Apprentice_ID = Owner.Player.UID;
                            Information.Enrole_date = (uint)Owner.Player.Associate.Associat[Role.Instance.Associate.Mentor][mentor.UID].Timer;
                            Information.Fill(mentor.Owner);
                            Information.Shared_Battle_Power = mentor.GetShareBattlePowers((uint)Owner.Player.RealBattlePower);
                            Information.WriteString(mentor.Name, Owner.Player.Spouse, Owner.Player.Name);
                            Owner.Send(Information.GetArray(stream));
                        }
                    }

                }
            }
            if (!Owner.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                return;

            foreach (var Apprentice in Owner.Player.Associate.OnlineApprentice.Values)
            {
                if (!Owner.Player.Associate.Associat[Role.Instance.Associate.Apprentice].ContainsKey(Apprentice.Player.UID))
                    continue;
                Role.Player target = Apprentice.Player;

                target.SetMentorBattlePowers(Owner.Player.GetShareBattlePowers((uint)target.RealBattlePower), (uint)Owner.Player.RealBattlePower);

                Information.Apprentice_ID = target.UID;
                Information.Enrole_date = (uint)Owner.Player.Associate.Associat[Role.Instance.Associate.Apprentice][target.UID].Timer;
                Information.Level = (byte)Owner.Player.Level;
                Information.Class = Owner.Player.Class;
                Information.PkPoints = Owner.Player.PKPoints;
                Information.Mesh = Owner.Player.Mesh;
                Information.Online = 1;
                Information.Shared_Battle_Power = Owner.Player.GetShareBattlePowers((uint)target.RealBattlePower);
                Information.WriteString(Owner.Player.Name, Owner.Player.Spouse, target.Name);
                target.Owner.Send(Information.GetArray(stream));
            }

        }

        public void IncreaseRifainaryStatus(uint ID)
        {
            Database.Rifinery.Item refinery;
            if (Pool.RifineryItems.TryGetValue(ID, out refinery))
            {
                // var refinery = Server.RifineryItems[ID];
                switch (refinery.Type)
                {
                    case Database.Rifinery.RefineryType.CriticalStrike:
                        {
                            Owner.Status.CriticalStrike += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.SkillCriticalStrike:
                        {
                            Owner.Status.SkillCStrike += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Break:
                        {
                            Owner.Status.Breakthrough += refinery.Procent * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Detoxication:
                        {
                            Owner.Status.Detoxication += refinery.Procent;
                            break;
                        }
                    case Database.Rifinery.RefineryType.MDefence:
                        {
                            Owner.Status.PhysicalDamageDecrease += refinery.Procent;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Block:
                        {
                            Owner.Status.Block += refinery.Procent * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Immunity:
                        {
                            Owner.Status.Immunity += refinery.Procent * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Penetration:
                        {
                            Owner.Status.Penetration += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Intensification:
                        {
                            Owner.Status.MaxHitpoints += refinery.Procent;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Counteraction:
                        {
                            Owner.Status.Counteraction += refinery.Procent * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMDamage:
                        {

                            Owner.Status.PhysicalDamageIncrease += refinery.Procent;

                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMAttack:
                        {
                            Owner.Status.MagicDamageIncrease += refinery.Procent;
                            break;
                        }
                }
                switch (refinery.Type2)
                {
                    case Database.Rifinery.RefineryType.CriticalStrike:
                        {
                            Owner.Status.CriticalStrike += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.SkillCriticalStrike:
                        {
                            Owner.Status.SkillCStrike += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Break:
                        {
                            Owner.Status.Breakthrough += refinery.Procent2 * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Detoxication:
                        {
                            Owner.Status.Detoxication += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.MDefence:
                        {
                            Owner.Status.PhysicalDamageDecrease += refinery.Procent2;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Block:
                        {
                            Owner.Status.Block += refinery.Procent2;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Immunity:
                        {
                            Owner.Status.Immunity += refinery.Procent2 * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Penetration:
                        {
                            Owner.Status.Penetration += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Intensification:
                        {
                            Owner.Status.MaxHitpoints += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Counteraction:
                        {
                            Owner.Status.Counteraction += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMDamage:
                        {

                            Owner.Status.PhysicalDamageIncrease += refinery.Procent2;

                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMAttack:
                        {
                            Owner.Status.MagicDamageIncrease += refinery.Procent2;
                            break;
                        }
                }
            }
            else
            {
                Console.WriteLine("error refinery id " + ID);
            }
        }
        public void AddAstredgeStats2()
        {
            if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperSpeedGun))
            {
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.SuperSpeedGun))
                {
                    this.Owner.Status.MaxHitpoints += Owner.Player.IncreasingHP;
                    this.Owner.Player.HitPoints = (int)Owner.Status.MaxHitpoints;
                }

            }
        }
        public void AddAstredgeStats()
        {
            if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChaoticDance))
            {
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.ChaoticDance))
                {
                    this.Owner.Status.MaxHitpoints += Owner.Player.IncreasingHP;
                    this.Owner.Player.HitPoints = (int)Owner.Status.MaxHitpoints;
                }

            }
            Astredge.Item[] MyAstredge = null;
            MyAstredge = this.Owner.MyAstredge.Items.Values.Where(p => p.ItemID >= Astredge.TypeID.ViodragonClub && p.ItemID <= Astredge.TypeID.WarlordSpear).ToArray();
            if (MyAstredge != null)
            {
                foreach (var Item in MyAstredge)
                {
                    if (Item.ItemID == Astredge.TypeID.ViodragonClub)
                    {
                        this.Owner.Status.MaxHitpoints += Item.Astredge.Status;

                        if ((AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 1)
                            || (AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 2)
                           || (AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 3))
                        {
                            this.Owner.Status.CriticalStrike += 100;
                            this.Owner.Status.SkillCStrike += 100;
                            this.Owner.Status.Immunity += 100;
                        }

                    }
                    else
                    {
                        if ((AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 1)
                              || (AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 2)
                             || (AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, this.Owner.Player.UID) == 3))
                        {
                            this.Owner.Status.DashRate += 100;
                            this.Owner.Status.Resist += 100;
                        }
                    }

                }
                if ((AstredgeRank.GetMyRank(AstredgeRank.Type.AstredgeWeekly, this.Owner.Player.UID) == 1)
                           || (AstredgeRank.GetMyRank(AstredgeRank.Type.AstredgeWeekly, this.Owner.Player.UID) == 2)
                          || (AstredgeRank.GetMyRank(AstredgeRank.Type.AstredgeWeekly, this.Owner.Player.UID) == 3))
                {
                    this.Owner.Status.PhysicalDamageIncrease += 500;
                    this.Owner.Status.PhysicalDamageDecrease += 500;
                }
            }

        }
        #region AllArchiveAtributes
        public void AddHWStats()
        {
            if (!Database.AtributesStatus.IsTrojan(Owner.Player.Class)) return;

            var MyWeapone = Owner.HundredWeapons.Objects.Values.FirstOrDefault(p => p.BitVector.Contain(1));
            if (MyWeapone != null)
            {
                Owner.Status.MaxHitpoints += (uint)MyWeapone.Attributes[Database.HundredWeapons.AttributeType.Hitpoints] + ((uint)MyWeapone.DBInfo.Attributes[Database.HundredWeapons.AttributeType.Hitpoints] / 2);
                Owner.Status.MinAttack += (uint)MyWeapone.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] + ((uint)MyWeapone.DBInfo.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] / 2);
                Owner.Status.MaxAttack += (uint)MyWeapone.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] + ((uint)MyWeapone.DBInfo.Attributes[Database.HundredWeapons.AttributeType.PhysicalAttack] / 2);
                Owner.Status.Defence += (uint)MyWeapone.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense] + ((uint)MyWeapone.DBInfo.Attributes[Database.HundredWeapons.AttributeType.PhysicalDefense] / 2);

            }


        }

        public void AddArchiveNinjaStats(Client.GameClient client)
        {
            if (this.Owner.Fake)
                return;

            if (Owner.MyNinja.Valid())
            {
                #region MudWall
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MudWall))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.MudWall, out clientspell))
                    {

                        Database.MagicType.Magic MudWall = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.Defence += (uint)MudWall.GDamage / 10000;
                        Owner.Status.MagicDefence += (uint)(MudWall.GDamage % 1000);
                        #region MudSigilSolidity
                        Ninja.Item ITEM;
                        if (Owner.MyNinja.TryGetValueEquip(Ninja.ItemType.MudSigilSolidity, out ITEM))
                        {
                            Owner.Status.Defence += (uint)ITEM.DBItem.Power / 10000;
                            Owner.Status.MagicDefence += (uint)ITEM.DBItem.Power % 10000;

                        }
                        #endregion

                    }

                }
                #endregion
                if (Owner.Player.NiniaP0 > 0)
                {
                    foreach (var n in NinjaFile.gouyu_immortals.Values)
                    {
                        if ((Owner.MyNinja.Levels > 17 ? 17 : Owner.MyNinja.Levels) == n.Level)
                        {
                            Owner.Status.MaxHitpoints += n.MaxHP;
                            Owner.Status.MaxAttack += n.PAttack;
                            Owner.Status.MinAttack += n.PAttack;
                            Owner.Status.Defence += n.PDefense;
                            Owner.Status.MagicDefence += n.MDefense;
                        }
                    }
                }

                var DashRate = Owner.MyNinja.Fire + Owner.MyNinja.Water + Owner.MyNinja.Earth + Owner.MyNinja.Wind + Owner.MyNinja.Lightning;
                if (DashRate >= 150)
                    Owner.Status.DashRate += 50;
                if (DashRate >= 300)
                    Owner.Status.DashRate += 50;
                if (DashRate >= 400)
                    Owner.Status.DashRate += 50;
                if (DashRate >= 500)
                    Owner.Status.DashRate += 100;

                if (Owner.MyNinja.Fire_Score >= 14000)
                    Owner.Status.DashRate += 50;
                if (Owner.MyNinja.Water_Score >= 14000)
                    Owner.Status.DashRate += 50;
                if (Owner.MyNinja.Earth_Score >= 14000)
                    Owner.Status.DashRate += 50;
                if (Owner.MyNinja.Wind_Score >= 14000)
                    Owner.Status.DashRate += 50;
                if (Owner.MyNinja.Lightning_Score >= 14000)
                    Owner.Status.DashRate += 50;


                uint NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Overall, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 1000;
                NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Fire, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 200;
                NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Water, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 200;
                NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Earth, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 200;
                NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Wind, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 200;
                NinjaRank = Database.NinjaRank.GetMyRank(Database.NinjaRank.Type.Lightning, Owner.Player.UID);
                if (NinjaRank == 1) Owner.Status.DashRate += 200;

            }
        }

        public void AddArchives(Client.GameClient client)
        {

            if (Owner.Player.ContainFlag(MsgUpdate.Flags.WindElementEffect))
            {
                var newspell = Pool.Magic[(ushort)Flags.SpellID.WindArrow][19];
                Owner.Status.Defence -= (uint)newspell.GDamage;
            }
            if (Owner.Player.ContainFlag(MsgUpdate.Flags.DefenseDecreasing))
            {
                Owner.Status.Defence -= Owner.Player.PDefence;
                Owner.Status.MagicDefence -= Owner.Player.MDefence;
            }
            if (Owner.Player.ContainFlag(MsgUpdate.Flags.SupremeLeadership))
            {
                uint Power;
                int Sec;
                Game.MsgServer.AttackHandler.SupremeLeadership.GetUpdateAmount(client.Player.SupremeLeadershipCount, out Power, out Sec);
                Owner.Status.MaxHitpoints += (Power / 1000) * 10;
                Owner.Status.Defence += (Power % 1000) * 10;
            }
            #region BounsWarrior
            if (AtributesStatus.IsWarrior(this.Owner.Player.Class))
            {

                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    this.Owner.Status.MaxHitpoints += obj.DBItem.MaxHP;
                    this.Owner.Status.MaxAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MinAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MagicDefence += obj.DBItem.MDefense;
                    this.Owner.Status.Defence += obj.DBItem.PDefense;
                    if (WarriorRank.GetMyRank(WarriorRank.Type.Overall, Owner.Player.UID) == 1)
                    {
                        Owner.Status.LuckyStrike += 200;
                        Owner.Status.Parry += 350;

                    }
                    if (WarriorRank.GetMyRank(WarriorRank.Type.Dragonhowl, Owner.Player.UID) == 1)
                        Owner.Status.Parry += 50;
                    if (WarriorRank.GetMyRank(WarriorRank.Type.Bloodlust, Owner.Player.UID) == 1)
                        Owner.Status.Parry += 50;
                    if (WarriorRank.GetMyRank(WarriorRank.Type.Redcurse, Owner.Player.UID) == 1)
                        Owner.Status.Parry += 50;


                }
                var Items = Owner.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragonhowl && p.ItemID <= Archives.TypeID.Redcurse).ToArray();
                foreach (var Bouns in Items)
                {
                    if (Bouns.Hash != 1)
                    {
                        this.Owner.Status.MaxHitpoints += (Bouns.DBItem.MaxHP * 5) / 100;
                        this.Owner.Status.MaxAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MinAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MagicDefence += (Bouns.DBItem.MDefense * 5) / 100;
                        this.Owner.Status.Defence += (Bouns.DBItem.PDefense * 5) / 100;
                    }
                }

                #region ShieldBlock
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.ShieldBlock))// ShieldBlock
                {
                    Owner.Status.Block += Owner.Player.ShieldBlockDamage * 100;
                }
                #endregion

            }
            #endregion
            #region BounsArcher
            if (AtributesStatus.IsArcher(this.Owner.Player.Class))
            {
                #region ElementalArrow
                MsgSpell ElementalArrowData = null;
                if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ElementalArrowData, out ElementalArrowData))
                {
                    Database.MagicType.Magic DBSpell = Pool.Magic[ElementalArrowData.ID][ElementalArrowData.Level];
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.FireArrow))
                    {
                        Owner.Status.MaxAttack += (uint)DBSpell.Damage;
                        Owner.Status.MinAttack += (uint)DBSpell.Damage;
                    }
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.IceArrow))
                    {
                        Owner.Status.Defence += (uint)DBSpell.DamageOnHuman;
                        Owner.Status.MagicDefence += (uint)DBSpell.Damage2;
                    }
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                    {
                        Owner.Status.Breakthrough += (uint)DBSpell.Damage3 * 10;
                        Owner.Status.Counteraction += (uint)DBSpell.Damage3 * 10;
                    }
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                    {
                        Owner.Status.MaxAttack += (uint)1500;
                    }
                }
                #endregion
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    this.Owner.Status.MaxHitpoints += obj.DBItem.MaxHP;
                    this.Owner.Status.MaxAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MinAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MagicDefence += obj.DBItem.MDefense;
                    this.Owner.Status.Defence += obj.DBItem.PDefense;
                    if (ArcherRank.GetMyRank(ArcherRank.Type.WeeklyArcher, this.Owner.Player.UID) == 1)
                    {
                        this.Owner.Status.MinAttack += 500;
                        this.Owner.Status.MaxAttack += 500;
                        this.Owner.Status.Defence += 500;
                        this.Owner.Status.MagicDefence += 300;
                    }
                    if (ArcherRank.GetMyRank(ArcherRank.Type.Overall, this.Owner.Player.UID) == 1)
                    {
                        this.Owner.Status.LuckyStrike += 200;
                        this.Owner.Status.Parry += 350;
                    }
                    if (ArcherRank.GetMyRank(ArcherRank.Type.StoneCracker, this.Owner.Player.UID) == 1)
                        this.Owner.Status.Parry += 50;
                    if (ArcherRank.GetMyRank(ArcherRank.Type.ColdMoon, this.Owner.Player.UID) == 1)
                        this.Owner.Status.Parry += 50;
                    if (ArcherRank.GetMyRank(ArcherRank.Type.ThronCutter, this.Owner.Player.UID) == 1)
                        this.Owner.Status.Parry += 50;
                }
                var Items = Owner.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.StoneCracker && p.ItemID <= Archives.TypeID.ThornCutter).ToArray();
                foreach (var Bouns in Items)
                {
                    if (Bouns.Hash != 1)
                    {
                        this.Owner.Status.MaxHitpoints += (Bouns.DBItem.MaxHP * 5) / 100;
                        this.Owner.Status.MaxAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MinAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MagicDefence += (Bouns.DBItem.MDefense * 5) / 100;
                        this.Owner.Status.Defence += (Bouns.DBItem.PDefense * 5) / 100;
                    }
                }
            }
            #endregion
            #region BounsTao
            if (AtributesStatus.IsTaoist(this.Owner.Player.Class))
            {

                #region HolyProtection
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.HolyProtection))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HolyProtection, out clientspell))
                    {
                        Database.MagicType.Magic HolyProtection = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.ItemBless += (uint)HolyProtection.DamageOnHuman;
                        Owner.Status.ItemBless = (uint)Math.Min(Owner.Status.ItemBless, 60);
                    }

                }
                #endregion
                #region PhoenixBlessing
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.PhoenixBlessing))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.PhoenixBlessing, out clientspell))
                    {
                        Database.MagicType.Magic PhoenixBlessing = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.ItemBless += (uint)PhoenixBlessing.DamageOnHuman;
                        Owner.Status.ItemBless = (uint)Math.Min(Owner.Status.ItemBless, 60);

                    }

                }
                #endregion
                #region FlameShield
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.FlameShield))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlameShield, out clientspell))
                    {
                        Database.MagicType.Magic FlameShield = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.PhysicalDamageDecrease += (uint)FlameShield.DamageOnHuman;
                        Owner.Status.MagicDamageDecrease += (uint)FlameShield.Damage2;

                    }
                }
                #endregion
                #region FlowKnack
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.FlowKnack))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlowKnack, out clientspell))
                    {
                        Database.MagicType.Magic FlowKnack = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.PhysicalDamageDecrease += (uint)FlowKnack.Damage;
                        Owner.Status.MagicDamageDecrease += (uint)FlowKnack.DamageOnMonster;


                    }
                }
                #endregion
                #region DeadwoodCurse
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DeadwoodCurse))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DeadwoodCurse, out clientspell))
                    {
                        Database.MagicType.Magic DeadwoodCurse = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.MagicDamageIncrease += (uint)DeadwoodCurse.DamageOnHuman;

                    }
                }
                #endregion
                #region DivineEmptiness
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DivineEmptiness))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DivineEmptiness, out clientspell))
                    {
                        Database.MagicType.Magic DivineEmptiness = Pool.Magic[clientspell.ID][clientspell.Level];
                        Owner.Status.DodgeRate = (ushort)Math.Min(2500, DivineEmptiness.DamageOnHuman);
                    }
                }
                #endregion
                #region WeepStorm
                if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeepStorm))
                {
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WeepStorm, out clientspell))
                    {
                        Database.MagicType.Magic WeepStorm = Pool.Magic[clientspell.ID][clientspell.Level];

                        Owner.Status.Damage += (ushort)((Owner.Status.Damage * WeepStorm.Sector / 10) / 100);
                    }
                }
                #endregion
                #region Deadwood
                if (client.MyArchives.isOpen(Archives.TypeID.Vicissitude) && client.Player.DeadwoodIncFinal > 0)
                {
                    Owner.Status.MagicDamageIncrease += (uint)client.Player.DeadwoodIncFinal;
                }
                #endregion
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    this.Owner.Status.MaxHitpoints += obj.DBItem.MaxHP;
                    this.Owner.Status.MaxAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MinAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MagicDefence += obj.DBItem.MDefense;
                    this.Owner.Status.Defence += obj.DBItem.PDefense;
                    if (AtributesStatus.IsWater(this.Owner.Player.Class))
                    {
                        if (WaterRank.GetMyRank(WaterRank.Type.WeeklyTao, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Defence += 500;
                            this.Owner.Status.MagicDefence += 500;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.Overall, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 350;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.Vicissitude, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 50;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.HighestGood, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 50;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.Evolution, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 50;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.Thrill, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 50;
                        }
                        if (WaterRank.GetMyRank(WaterRank.Type.Birthdeath, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Parry += 50;
                        }
                    }
                    if (AtributesStatus.IsFire(this.Owner.Player.Class))
                    {
                        if (FireRank.GetMyRank(FireRank.Type.WeeklyTao, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.Status.Defence += 500;
                            this.Owner.Status.MagicDefence += 500;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.Overall, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 4;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.Vicissitude, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 1;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.HighestGood, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 1;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.Evolution, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 1;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.Thrill, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 1;
                        }
                        if (FireRank.GetMyRank(FireRank.Type.Birthdeath, this.Owner.Player.UID) == 1)
                        {
                            this.Owner.PerfectionStatus.CoreStrike += 1;
                        }
                    }
                }
                var Items = Owner.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Vicissitude && p.ItemID <= Archives.TypeID.BirthDeath).ToArray();
                foreach (var Bouns in Items)
                {
                    if (Bouns.Hash != 1)
                    {
                        this.Owner.Status.MaxHitpoints += (Bouns.DBItem.MaxHP * 5) / 100;
                        this.Owner.Status.MaxAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MinAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MagicDefence += (Bouns.DBItem.MDefense * 5) / 100;
                        this.Owner.Status.Defence += (Bouns.DBItem.PDefense * 5) / 100;
                    }
                }
            }
            #endregion

            #region BounsMonk
            if (AtributesStatus.IsMonk(this.Owner.Player.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    this.Owner.Status.MaxHitpoints += obj.DBItem.MaxHP;
                    this.Owner.Status.MaxAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MinAttack += obj.DBItem.PAttack;
                    this.Owner.Status.MagicDefence += obj.DBItem.MDefense;
                    this.Owner.Status.Defence += obj.DBItem.PDefense;
                }
                var Items = Owner.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.HeavenlyTiger && p.ItemID <= Archives.TypeID.CosmicRoc).ToArray();
                foreach (var Bouns in Items)
                {
                    if (Bouns.Hash != 1)
                    {
                        this.Owner.Status.MaxHitpoints += (Bouns.DBItem.MaxHP * 5) / 100;
                        this.Owner.Status.MaxAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MinAttack += (Bouns.DBItem.PAttack * 5) / 100;
                        this.Owner.Status.MagicDefence += (Bouns.DBItem.MDefense * 5) / 100;
                        this.Owner.Status.Defence += (Bouns.DBItem.PDefense * 5) / 100;
                    }
                }
            }
            #endregion
            #region Dragon
            if (AtributesStatus.IsLee(this.Owner.Player.Class))
            {
                var Items = Owner.MyArchives.Items.Values.Where(p => p.ItemID >= Archives.TypeID.Dragon && p.ItemID <= Archives.TypeID.Suanni).ToArray();
                foreach (var Bouns in Items)
                {
                    this.Owner.Status.MaxHitpoints += Bouns.DBItem.MaxHP;
                    this.Owner.Status.MaxAttack += Bouns.DBItem.PAttack;
                    this.Owner.Status.MinAttack += Bouns.DBItem.PAttack;
                    this.Owner.Status.MagicDefence += Bouns.DBItem.MDefense;
                    this.Owner.Status.Defence += Bouns.DBItem.PDefense;

                }
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.KunpengHeart))
                    Owner.Status.DodgeRate += (uint)Owner.MyArchives.GetValue("KunpengHeart")[0];
            }
            #endregion
            #region BonusDune
            if (AtributesStatus.IsDune(Owner.Player.Class))
            {
                foreach (var item in Owner.MyArchives.Items.Values)
                {
                    switch (item.ItemID)
                    {
                        case Archives.TypeID.Conception:
                            Owner.Status.MaxAttack += item.DBItem.PAttack;
                            Owner.Status.MinAttack += item.DBItem.PAttack;
                            break;

                        case Archives.TypeID.Belt:
                            Owner.Status.MaxHitpoints += item.DBItem.MaxHP;
                            break;
                        case Archives.TypeID.Gobernador:
                            Owner.Status.Defence += item.DBItem.PDefense;
                            break;

                    }
                }
            }
            #endregion
        }

        public void RunePirateAtributes()
        {
            if (!AtributesStatus.IsPirate(Owner.Player.Class)) return;
            #region Skills
            #region Sense
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)412))
                Owner.Status.DodgeRate += (uint)Owner.MyArchives.GetValue("Sense")[1];
            #endregion
            #region Diabolize
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)394))
                Owner.Status.DodgeRate += (uint)Owner.MyArchives.GetValue("Diabolize")[0];
            #endregion
            #region Barrier
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)397))//397
                Owner.Status.Counteraction += (uint)Owner.MyArchives.GetValue("Barrier")[0];
            #endregion
            #region Shell
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)407))//407
                Owner.Status.Immunity += (uint)Owner.MyArchives.GetValue("Shell")[0];
            #endregion
            #region Storm
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)402))//402

                Owner.Status.CriticalStrike += (uint)Owner.MyArchives.GetValue("Storm")[0];
            #endregion
            #region Thrash
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)403))//403
                Owner.Status.PhysicalDamageDecrease += (uint)Owner.MyArchives.GetValue("Thrash")[0];
            #endregion
            #region ColdBloodline
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)410))//401
                Owner.Status.Defence += (uint)Owner.MyArchives.GetValue("ColdBloodline")[0];
            #endregion
            #region Thunder
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)401))//401
                Owner.Status.PhysicalDamageDecrease += (uint)Owner.MyArchives.GetValue("Thunder")[0];
            #endregion
            #region Torrent
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)400))//400
            {
                Owner.Status.MinAttack += (uint)Owner.MyArchives.GetValue("Torrent")[0];
                Owner.Status.MaxAttack += (uint)Owner.MyArchives.GetValue("Torrent")[0];
            }

            #endregion
            #region Tide
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)399))//399
                Owner.Status.Defence += (uint)Owner.MyArchives.GetValue("Tide")[0];
            #endregion
            #region Splash
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)406))//406
                Owner.Status.MaxHitpoints += (uint)Owner.MyArchives.GetValue("Splash")[0];
            #endregion
            #region Sailing
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)419))//419
                Owner.Status.Block += (uint)Owner.MyArchives.GetValue("Sailing")[0] * 100;
            #endregion
            #region Vast
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)398))//398
                Owner.Status.Breakthrough += (uint)Owner.MyArchives.GetValue("Vast")[0];
            #endregion
            #region Revelator
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)408))//398
                Owner.Status.DodgeRate += (uint)Owner.MyArchives.GetValue("Revelator")[0];
            #endregion
            #endregion
            var Atributes = Owner.MyArchives.Collection;
            if (Atributes != null)
            {
                Owner.Status.MaxHitpoints += Atributes.MaxHP;
                Owner.Status.MinAttack += Atributes.PAttack;
                Owner.Status.MaxAttack += Atributes.PAttack;
                Owner.Status.PhysicalDamageIncrease += Atributes.FinalPAttack;
                Owner.Status.PhysicalDamageDecrease += Atributes.FinalPDamage;
            }
        }

        public void AddJadeAtributes()
        {

            var items = Owner.MyArchives.isOpen();
            if (AtributesStatus.IsTaoist(Owner.Player.Class))
                if (items != null)
                {
                    for (int i = 0; i < items.Jades.Length; i++)
                    {
                        if (items.Jades[i].JadeID > 0)
                        {
                            daoqi_dict_type.Item dao;
                            if (daoqi_dict_type.Items.TryGetValue((uint)items.Jades[i].JadeID, out dao))
                            {
                                foreach (var item2 in dao.Attributes)
                                {
                                    switch ((daoqi_dict_type.AttrType)item2.Type)
                                    {
                                        case daoqi_dict_type.AttrType.None:
                                            break;
                                        case daoqi_dict_type.AttrType.MaxHP:
                                            Owner.Status.MaxHitpoints += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.PAttack:
                                            Owner.Status.MinAttack += item2.Value;
                                            Owner.Status.MaxAttack += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.PDefense:
                                            Owner.Status.Defence += item2.Value;//add this
                                            break;
                                        case daoqi_dict_type.AttrType.MAttack:
                                            Owner.Status.MagicAttack += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.MDefense:
                                            Owner.Status.MagicDefence += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.FinalPAttack:
                                            Owner.Status.PhysicalDamageIncrease += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.FinalPDamage:
                                            Owner.Status.PhysicalDamageDecrease += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.FinalMAttack:
                                            Owner.Status.MagicDamageIncrease += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.FinalMDamage:
                                            Owner.Status.MagicDamageDecrease += item2.Value;

                                            break;
                                        case daoqi_dict_type.AttrType.PStrike:
                                            Owner.Status.CriticalStrike += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.MStrike:
                                            Owner.Status.SkillCStrike += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.Immunity:
                                            Owner.Status.Immunity += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.Break:
                                            Owner.Status.Breakthrough += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.AntiBreak:
                                            Owner.Status.Counteraction += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.Parry:
                                            Owner.Status.Parry += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.LuckyStrike:
                                            Owner.Status.LuckyStrike += item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.CoreStrike:
                                            Owner.PerfectionStatus.CoreStrike += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.InvisibleArrow://perfection atributs
                                            Owner.PerfectionStatus.InvisibleArrow += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.DrainingTouch:
                                            Owner.PerfectionStatus.DrainingTouch += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.BloodSpawn:
                                            Owner.PerfectionStatus.BloodSpawn += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.KillingFlash:
                                            Owner.PerfectionStatus.KillingFlash += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.MirrorOfSin:
                                            Owner.PerfectionStatus.MirrorOfSin += (int)item2.Value;
                                            break;
                                        case daoqi_dict_type.AttrType.MaxMP:
                                            Owner.Status.MaxMana += item2.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                        }
                    }
                }

        }
        #endregion

        #region MythSoul
        public void AddMythSoulAtributes()
        {
            if (Owner.Player.ContainFlag(MsgUpdate.Flags.Bash))
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Bash].TryGetValue(Owner.Status.Bash, out MythInfo))
                {
                    Owner.Status.CriticalStrike += (uint)MythInfo.Damage;
                    Owner.Status.SkillCStrike += (uint)MythInfo.Damage;
                    Owner.Status.Counteraction += (uint)(MythInfo.Damage / 10);

                }
            }
            if (Owner.Player.ContainFlag(MsgUpdate.Flags.Luck))
            {
                MythSoulAttributes.Attribute MythInfo;
                if (MythSoulAttributes.Attributes[MythSoulAttributes.Type.Luck].TryGetValue(Owner.Status.Luck, out MythInfo))
                {
                    Owner.Status.Parry += (uint)MythInfo.Damage;

                }
            }
            MythTable.MythSoulEXP attr;
            foreach (var item in Owner.MythSoulBag.Values)
            {

                if (MythTable.MythSoulExpList.TryGetValue(item.ITEM_ID, out attr))
                {
                    switch (attr.type)
                    {
                        case Database.MythTable.MythAttribute.MaxHP:

                            Owner.Status.MaxHitpoints += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.PAttack:

                            Owner.Status.MinAttack += attr.value;
                            Owner.Status.MaxAttack += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.PDefense:

                            Owner.Status.Defence += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.MAttack:

                            Owner.Status.MagicAttack += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.MDefense:

                            Owner.Status.MagicDefence += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.FinalPAttack:

                            Owner.Status.PhysicalDamageIncrease += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.FinalPDamage:

                            Owner.Status.PhysicalDamageDecrease += attr.value;
                            break;

                        case Database.MythTable.MythAttribute.FinalMAttack:

                            Owner.Status.MagicDamageIncrease += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.FinalMDamage:

                            Owner.Status.MagicDamageDecrease += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.PStrike:

                            Owner.Status.CriticalStrike += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.MStrike:

                            Owner.Status.SkillCStrike += attr.value;
                            break;
                        case Database.MythTable.MythAttribute.Break:

                            Owner.Status.Breakthrough += (uint)(attr.value / 10);
                            break;
                        case Database.MythTable.MythAttribute.AntiBreak:

                            Owner.Status.Counteraction += (uint)(attr.value / 10);
                            break;
                        case Database.MythTable.MythAttribute.Immunity:

                            Owner.Status.Immunity += attr.value;
                            break;

                    }
                }

            }
        }

        public void LoadMythsoul(MsgGameItem Item)
        {
            if (Item.MythSoulID != 0)
            {
                byte Level = (byte)(Item.MythSoulID % 10);
                uint Type = (Item.MythSoulID % 1000) - Level;
                switch (Type)
                {
                    case 10:
                        Owner.Status.VenomLevel += Level;
                        break;
                    case 40:
                        Owner.Status.BloodthirstLevel += Level;
                        break;
                    case 80:
                        Owner.Status.EtherealLevel += Level;
                        break;
                    case 170:
                        Owner.Status.ElanLevel += Level;
                        break;
                    case 180:
                        Owner.Status.SolidLevel += Level;
                        break;
                    case 20:
                        Owner.Status.HawkeyeLevel += Level;
                        break;
                    case 60:
                        Owner.Status.SweepLevel += Level;
                        break;
                    case 30:
                        Owner.Status.EdgeLevel += Level;
                        break;
                    case 140:
                        Owner.Status.MeditationLevel += Level;
                        break;
                    case 340:
                        Owner.Status.Discipline += Level;
                        break;
                    case 350:
                        Owner.Status.Safeguard += Level;
                        break;
                    case 360:
                        Owner.Status.Demolition += Level;
                        break;

                }
            }
            if (Item.Mutacion != 0)
            {
                byte Level = (byte)(Item.Mutacion % 10);
                uint Type = (Item.Mutacion % 1000) - Level;
                switch (Type)
                {
                    case 250:
                        Owner.Status.Superpower += Level;
                        break;
                    case 260:
                        Owner.Status.Oracle += Level;
                        break;
                    case 270:
                        Owner.Status.Numb += Level;
                        break;
                    case 280:
                        Owner.Status.Frost += Level;
                        break;
                    case 290:
                        Owner.Status.Bash += Level;
                        break;
                    case 310:
                        Owner.Status.Luck += Level;
                        break;
                    case 320:
                        Owner.Status.Crack += Level;
                        break;
                    case 330:
                        Owner.Status.Vigour += Level;
                        break;
                }
            }
            if (Item.Mutacion2 != 0)
            {
                byte Level = (byte)(Item.Mutacion2 % 10);
                uint Type = (Item.Mutacion2 % 1000) - Level;
                switch (Type)
                {
                    case 340:
                        {
                            switch (Level)
                            {
                                case 1: Owner.Status.DashRate += 100; break;
                                case 2: Owner.Status.DashRate += 200; break;
                                case 3: Owner.Status.DashRate += 300; break;
                                case 4: Owner.Status.DashRate += 400; break;
                                case 5: Owner.Status.DashRate += 500; break;
                                case 6: Owner.Status.DashRate += 600; break;
                                default: Owner.Status.DashRate = 0; break;
                            }
                            break;
                        }
                    case 350:
                        {
                            switch (Level)
                            {
                                case 1: Owner.Status.Resist += 100; break;
                                case 2: Owner.Status.Resist += 200; break;
                                case 3: Owner.Status.Resist += 300; break;
                                case 4: Owner.Status.Resist += 400; break;
                                case 5: Owner.Status.Resist += 500; break;
                                case 6: Owner.Status.Resist += 600; break;
                                default: Owner.Status.Resist = 0; break;
                            }
                            break;

                        }

                }
            }
        }
        #endregion



        public void AddGem(Role.Flags.Gem gem)
        {
            switch (gem)
            {
                case Role.Flags.Gem.NormalThunderGem: Owner.AddGem(gem, 100); break;
                case Role.Flags.Gem.RefinedThunderGem: Owner.AddGem(gem, 300); break;
                case Role.Flags.Gem.SuperThunderGem: Owner.AddGem(gem, 500); break;

                case Role.Flags.Gem.NormalGloryGem: Owner.AddGem(gem, 100); break;
                case Role.Flags.Gem.RefinedGloryGem: Owner.AddGem(gem, 300); break;
                case Role.Flags.Gem.SuperGloryGem: Owner.AddGem(gem, 500); break;

                case Role.Flags.Gem.NormalPhoenixGem:
                case Role.Flags.Gem.RefinedPhoenixGem:
                case Role.Flags.Gem.SuperPhoenixGem: Owner.AddGem(gem, 15); break;

                case Role.Flags.Gem.NormalDragonGem: Owner.AddGem(gem, 5); break;
                case Role.Flags.Gem.RefinedDragonGem: Owner.AddGem(gem, 10); break;
                case Role.Flags.Gem.SuperDragonGem: Owner.AddGem(gem, 15); break;

                case Role.Flags.Gem.NormalTortoiseGem: Owner.AddGem(gem, 2); break;
                case Role.Flags.Gem.RefinedTortoiseGem: Owner.AddGem(gem, 4); break;
                case Role.Flags.Gem.SuperTortoiseGem: Owner.AddGem(gem, 6); break;


                case Role.Flags.Gem.NormalRainbowGem: Owner.AddGem(gem, 10); break;
                case Role.Flags.Gem.RefinedRainbowGem: Owner.AddGem(gem, 15); break;
                case Role.Flags.Gem.SuperRainbowGem: Owner.AddGem(gem, 25); break;


                case Role.Flags.Gem.NormalInfinityGem: Owner.AddGem(gem, 500); break;
                case Role.Flags.Gem.RefinedInfinityGem: Owner.AddGem(gem, 1500); break;
                case Role.Flags.Gem.SuperInfinityGem: Owner.AddGem(gem, 3000); break;
            }
        }

        public int BattlePower = 0;

        public void CalculateBattlePower()
        {

            BattlePower = 0;
            int val = 0;
            int val_item = 0;

            foreach (var item in CurentEquip)
            {
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Relics)
                    continue;
                if (Database.ItemType.IsHossu(item.ITEM_ID))
                    continue;
                val_item = 0;
                byte Quality = (byte)(item.ITEM_ID % 10);
                switch (Quality)
                {
                    case 9: val_item += 4; break;
                    case 8: val_item += 3; break;
                    case 7: val_item += 2; break;
                    case 6: val_item += 1; break;
                }
                val_item += item.Plus;

                if (item.SocketOne != Role.Flags.Gem.NoSocket)
                    val_item += 1;
                if ((byte)(((byte)item.SocketOne % 10) - 3) == 0)
                    val_item += 1;
                if (item.SocketTwo != Role.Flags.Gem.NoSocket)
                    val_item += 1;
                if ((byte)(((byte)item.SocketTwo % 10) - 3) == 0)
                    val_item += 1;
                switch (item.AnimaItemID % 100)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                    case 3:
                        val_item += 1;
                        break;
                    case 19:
                        val_item += 16;
                        break;
                    default:
                        val_item += (byte)((item.AnimaItemID % 100) - 2);
                        break;
                }
                if (Database.ItemType.IsBacksword(item.ITEM_ID) || Database.ItemType.IsTaoistEpicWeapon(item.ITEM_ID)
                    || (Database.ItemType.IsTwoHand(item.ITEM_ID) && FreeEquip(Flags.ConquerItem.LeftWeapon) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon)))
                    val_item *= 2;

                val += val_item;

            }
            BattlePower = val;
        }

        private Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem> ComputeWeapons()
        {
            if (!Alternante)
            {
                return new Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem>(
                TryGetEquip(Flags.ConquerItem.RightWeapon),
                TryGetEquip(Flags.ConquerItem.LeftWeapon));
            }
            else
            {
                if (FreeEquip(Flags.ConquerItem.AleternanteRightWeapon))
                {
                    return new Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem>(
                    TryGetEquip(Flags.ConquerItem.RightWeapon),
                    TryGetEquip(Flags.ConquerItem.LeftWeapon));
                }
                else
                {
                    if (FreeEquip(Flags.ConquerItem.RightWeapon))
                    {
                        return new Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem>(
                        TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon),
                        TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon));
                    }
                    else
                    {
                        if (!FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                        {
                            return new Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem>(
                            TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon),
                            TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon));
                        }
                        else
                        {
                            if (FreeEquip(Flags.ConquerItem.LeftWeapon))
                            {
                                return new Tuple<Game.MsgServer.MsgGameItem, Game.MsgServer.MsgGameItem>(
                                TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon),
                                    null);
                            }
                            else
                            {
                                Game.MsgServer.MsgGameItem aRight = TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon),
                                             nLeft = TryGetEquip(Flags.ConquerItem.LeftWeapon);
                                if (Database.ItemType.IsTwoHand(aRight.ITEM_ID))
                                {
                                    if (Database.ItemType.IsArrow(nLeft.ITEM_ID))
                                    {
                                        if (Database.ItemType.IsBow(aRight.ITEM_ID))
                                        {
                                            return new Tuple<Game.MsgServer.MsgGameItem,
                                                Game.MsgServer.MsgGameItem>(aRight, nLeft);
                                        }
                                        else
                                        {
                                            return new Tuple<Game.MsgServer.MsgGameItem,
                                                Game.MsgServer.MsgGameItem>(aRight, null);
                                        }
                                    }
                                    else
                                    {
                                        if (Database.ItemType.IsShield(nLeft.ITEM_ID))
                                        {
                                            if (!Owner.MySpells.ClientSpells.ContainsKey(10311))
                                            {

                                                return new Tuple<Game.MsgServer.MsgGameItem,
                                                    Game.MsgServer.MsgGameItem>(aRight, null);
                                            }
                                            else
                                            {
                                                return new Tuple<Game.MsgServer.MsgGameItem,
                                                    Game.MsgServer.MsgGameItem>(aRight, nLeft);
                                            }
                                        }
                                        else
                                        {
                                            return new Tuple<Game.MsgServer.MsgGameItem,
                                                Game.MsgServer.MsgGameItem>(aRight, null);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!Database.ItemType.IsTwoHand(nLeft.ITEM_ID))
                                    {
                                        return new Tuple<Game.MsgServer.MsgGameItem,
                                            Game.MsgServer.MsgGameItem>(aRight, nLeft);
                                    }
                                    else
                                    {
                                        return new Tuple<Game.MsgServer.MsgGameItem,
                                            Game.MsgServer.MsgGameItem>(aRight, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnDequeue()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                try
                {
                    Dictionary<uint, Game.MsgServer.MsgGameItem> statusitens = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                    foreach (var it in AllItems)
                        if (it.Position < 50)
                            if (!statusitens.ContainsKey(it.Position))
                                statusitens.Add(it.Position, it);
                    if (Alternante)
                    {
                        if (!FreeEquip(Flags.ConquerItem.RightWeapon) && !FreeEquip(Flags.ConquerItem.LeftWeapon))
                        {
                            MsgGameItem _left;
                            TryGetEquip(Flags.ConquerItem.LeftWeapon, out _left);
                            MsgGameItem _right;
                            TryGetEquip(Flags.ConquerItem.LeftWeapon, out _right);
                            if (Database.ItemType.IsKnife(_left.ITEM_ID) && Database.ItemType.IsKnife(_right.ITEM_ID))
                            {
                                MsgGameItem _bow;
                                if (TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon, out _bow) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                                {
                                    foreach (var it in AllItems)
                                        if (it.Position > 50)
                                        {
                                            if (statusitens.ContainsKey((ushort)(it.Position - 50)))
                                                statusitens.Remove((ushort)(it.Position - 50));
                                            statusitens.Add((ushort)(it.Position - 50), it);
                                        }
                                    if (statusitens.ContainsKey((ushort)Flags.ConquerItem.LeftWeapon))
                                    {
                                        statusitens.Remove((ushort)Flags.ConquerItem.LeftWeapon);
                                    }
                                    goto jmp;
                                }
                            }
                        }
                        foreach (var it in AllItems)
                            if (it.Position > 50)
                            {
                                if (it.Position == (byte)Role.Flags.ConquerItem.AleternanteRightWeapon)
                                {
                                    if (Database.ItemType.IsTwoHand(it.ITEM_ID))
                                    {
                                        if (Database.ItemType.IsBow(it.ITEM_ID) == false)
                                        {
                                            if (statusitens.ContainsKey((ushort)(it.Position - 49)))
                                            {
                                                statusitens.Remove((ushort)(it.Position - 49));
                                                Remove((Role.Flags.ConquerItem)((it.Position - 49)), stream);
                                            }
                                        }
                                    }
                                }
                                if (statusitens.ContainsKey((ushort)(it.Position - 50)))
                                    statusitens.Remove((ushort)(it.Position - 50));
                                statusitens.Add((ushort)(it.Position - 50), it);
                            }
                    }
                jmp:
                    AppendItems(CreateSpawn, statusitens.Values.ToArray(), stream);
                    UpdateStats(statusitens.Values.ToArray(), stream);
                    Owner.Player.HitPoints = Math.Min((int)Owner.Player.HitPoints, (int)Owner.Status.MaxHitpoints);
                    if (Owner.Player.OnTransform && Owner.Player.TransformInfo != null)
                        Owner.Player.TransformInfo.UpdateStatus();
                    else
                        Owner.Player.SendUpdateHP();

                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
            }
        }

        public void AppendItems(bool CreateSpawn, Game.MsgServer.MsgGameItem[] Items, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgShowEquipment ShowEquip = new MsgShowEquipment();
            ShowEquip.UID = Game.MsgServer.MsgShowEquipment.Show;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);

            if (CreateSpawn)
            {
                foreach (var item in Items)
                {
                    if (item != null)
                    {
                        switch ((Role.Flags.ConquerItem)item.Position)
                        {
                            case Flags.ConquerItem.Ring:
                            case Flags.ConquerItem.AleternanteRing: ShowEquip.Ring = item.UID; break;
                            case Flags.ConquerItem.AleternanteHead:
                            case Flags.ConquerItem.Head: ShowEquip.Head = item.UID; break;
                            case Flags.ConquerItem.AleternanteNecklace:
                            case Flags.ConquerItem.Necklace: ShowEquip.Necklace = item.UID; break;
                            case Flags.ConquerItem.AleternanteRightWeapon:
                            case Flags.ConquerItem.RightWeapon: ShowEquip.RightWeapon = item.UID; break;
                            case Flags.ConquerItem.AleternanteLeftWeapon:
                            case Flags.ConquerItem.LeftWeapon: ShowEquip.LeftWeapon = item.UID; break;
                            case Flags.ConquerItem.AleternanteArmor:
                            case Flags.ConquerItem.Armor:
                                {
                                    ShowEquip.Armor = item.UID;
                                    break;
                                }
                            case Flags.ConquerItem.AleternanteBoots:
                            case Flags.ConquerItem.Boots: ShowEquip.Boots = item.UID; break;
                            case Flags.ConquerItem.AleternanteBottle:
                            case Flags.ConquerItem.Bottle: ShowEquip.Bottle = item.UID; break;
                            case Flags.ConquerItem.SteedMount: ShowEquip.SteedMount = item.UID; break;
                            case Flags.ConquerItem.AleternanteGarment:
                            case Flags.ConquerItem.Garment:
                                {

                                    ShowEquip.Garment = item.UID;
                                    break;
                                }
                            case Flags.ConquerItem.RidingCrop: ShowEquip.RidingCrop = item.UID; break;
                            case Flags.ConquerItem.LeftWeaponAccessory: ShowEquip.LeftWeaponAccessory = item.UID; break;
                            case Flags.ConquerItem.RightWeaponAccessory: ShowEquip.RightWeaponAccessory = item.UID; break;
                            case Flags.ConquerItem.Wing: ShowEquip.Wing = item.UID; break;
                            case Flags.ConquerItem.AleternanteRelics:
                            case Flags.ConquerItem.Relics:
                                {
                                    ShowEquip.Relic = item.UID; break;
                                }
                        }
                    }
                }
                if (Owner.Player.SpecialGarment != 0)
                    ShowEquip.Garment = uint.MaxValue - 1;
                Owner.Send(stream.ShowEquipmentCreate(ShowEquip));


            }
        }

        public bool UseWindWalkerWeapons()
        {
            return Database.ItemType.IsWindWalkerWeapon(LeftWeapon) && Database.ItemType.IsWindWalkerWeapon(RightWeapon);
        }

        public unsafe void SendAlowAlternante(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgShowEquipment ShowEquip = new MsgShowEquipment();
            ShowEquip.UID = Game.MsgServer.MsgShowEquipment.AlternanteAllow;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);
            Owner.Send(stream.ShowEquipmentCreate(ShowEquip));


        }

        public unsafe void QueryEquipment(bool Alternantes, bool CallItems = true)
        {
            this.Alternante = Alternantes;
            CreateSpawn = CallItems;
            OnDequeue();

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Owner.UpdatePerfectionLevel(stream);

            }

            if (UseWindWalkerWeapons())
            {
                Owner.Player.AddFlag(MsgUpdate.Flags.WindWalkerFan, Role.StatusFlagsBigVector32.PermanentFlag, false);
            }
            else
                Owner.Player.RemoveFlag(MsgUpdate.Flags.WindWalkerFan);
        }
        public unsafe void PerfLevelUpdate(ServerSockets.Packet stream)
        {
            Owner.Status.PrestigeLevel = Math.Min(648, Owner.PrestigeLevel);
            Owner.Send(stream.StatusCreate(Owner.Status));
        }




        public unsafe bool AddEx(ServerSockets.Packet stream, uint ID, Role.Flags.ConquerItem position, byte plus = 0, byte bless = 0, byte Enchant = 0
           , Role.Flags.Gem sockone = Flags.Gem.NoSocket
            , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, uint PerfectionLevel = 0, uint AnimaID = 0
            , uint PurificationItemID = 0
            , uint EffectID = 0)
        {
            if (FreeEquip(position))
            {
                Database.ItemType.DBItem DbItem;
                if (Pool.ItemsBase.TryGetValue(ID, out DbItem))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Pool.ITEM_Counter.Next;
                    ItemDat.Effect = Effect;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = 1;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Pool.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    CheakUp(ItemDat);

                    ItemDat.Position = (ushort)position;
                    ItemDat.Mode = Flags.ItemMode.AddItem;
                    ItemDat.Send(Owner, stream);



                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0, 0));

                    return true;

                }
            }
            return false;
        }
    }
}
