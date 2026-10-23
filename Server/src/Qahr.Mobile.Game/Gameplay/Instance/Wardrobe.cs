using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Client;


namespace Qahr.Mobile.Role.Instance
{
    public class CoatColorRule
    {
        public System.Collections.Concurrent.ConcurrentDictionary<uint, List<CoatColor>> Items;
        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems;
        public Client.GameClient Owner;
        public CoatColorRule(Client.GameClient _owner)
        {
            Owner = _owner;
            Items = new System.Collections.Concurrent.ConcurrentDictionary<uint, List<CoatColor>>();
            ClientItems = new ConcurrentDictionary<uint, MsgGameItem>();
        }
        public class CoatColor
        {
            public uint ItemType;
            public uint Time;
            public uint ColorType;
            public uint RGB;
            public uint ExpiryDate;
        }
        private Random pRandom = new Random();
        public void RandomChange(uint Id)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MsgGameItem item;
                if (Owner.MyWardrobe.TryGetItem(Id, out item))
                {
                    uint ItemType = item.ITEM_ID;
                    Coat_Color_Rule.ItemColor DB;
                    if (Coat_Color_Rule.Info.TryGetValue(ItemType, out DB))
                    {
                        bool Pay = false;
                        if (Owner.Inventory.Contain(DB.ItemType1, DB.Amount1))
                        {
                            Owner.Inventory.Remove(DB.ItemType1, DB.Amount1, stream);
                            Pay = true;
                        }
                        else if (Owner.Player.ConquerPoints >= DB.RandomChange_CPs)
                        {
                            Owner.Player.ConquerPoints -= (uint)DB.RandomChange_CPs;
                            Pay = true;
                        }
                        if (Pay)
                        {
                            List<uint> x = new List<uint>();
                            x.Add(DB.Color1);
                            x.Add(DB.Color2);
                            x.Add(DB.Color3);
                            x.Add(DB.Color4);
                            if (!Items.ContainsKey(ItemType))
                                Items.Add(ItemType, new List<CoatColor>());
                            if (Items.ContainsKey(ItemType))
                            {
                                
                                DateTime UnixEpoch = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                                uint Timestamp = (uint)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
                                uint num = 0;
                                while (Items[ItemType].FirstOrDefault(p => p.Time == Timestamp) != null)
                                {
                                    Timestamp++;
                                    num++;
                                    if (num >= 300)
                                        break;
                                }
                                uint m_Color = (uint)System.Drawing.Color.FromArgb(pRandom.Next(0, 256), pRandom.Next(0, 256), pRandom.Next(0, 256)).ToArgb();
                                var pColor = new CoatColor()
                                {
                                    ItemType = ItemType,
                                    ColorType = 0,
                                    Time = Timestamp,
                                    ExpiryDate = Role.Core.Rate(1) ? 0 : Timestamp + 7 * 24 * 60 * 60,
                                    RGB = Role.Core.Rate(100) ? x.Count != 0 ? x[pRandom.Next(0, x.Count)] : m_Color : m_Color

                                };

                                Items[ItemType].Add(pColor);
                                var msg = new MsgCoatStorage.CoatStorage()
                                {
                                    ActionID = (uint)MsgCoatStorage.Types.RandomChange,
                                    ID = Id,
                                    Remove = 1,
                                    dwpram2 = 32,
                                };
                                msg.MountColor.Add(new MsgCoatStorage.ItemColor
                                {
                                    ITEMID = Id,
                                    ColorID = pColor.ColorType,
                                    TIMEITEM = pColor.Time,
                                    ExpiryTIME = pColor.ExpiryDate,
                                    RGB = pColor.RGB
                                });
                                Owner.Send(stream.CreateCoatStorage(msg));
                            }
                        }
                    }
                }
            }
        }
        public void LoadColors(uint Id)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Items.ContainsKey(Id))
                {
                    var msg = new MsgCoatStorage.CoatStorage()
                    {
                        ActionID = (uint)MsgCoatStorage.Types.LoadColor,
                        ID = Id,
                        dwpram2 = 1,
                        Remove = 1,
                    };
                    MsgGameItem item;
                    if (Owner.MyWardrobe.TryGetType(Id, out item))
                    {
                    }
                    foreach (var pColor in Items[Id])
                    {

                        DateTime UnixEpoch = new DateTime(1970, 1, 1);
                        uint Timestamp = (uint)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
                        if (pColor.ExpiryDate != 0 && pColor.ExpiryDate < Timestamp || item != null && item.RelicAttributes[0] == pColor.RGB)
                        {
                            continue;
                        }
                        msg.MountColor.Add(new MsgCoatStorage.ItemColor
                        {
                            ITEMID = pColor.ItemType,
                            ColorID = pColor.ColorType,//??
                            TIMEITEM = pColor.Time,
                            ExpiryTIME = pColor.ExpiryDate,
                            RGB = pColor.RGB
                        });
                        if (msg.MountColor.Count == 20)
                        {
                            Owner.Send(stream.CreateCoatStorage(msg));
                            msg = new MsgCoatStorage.CoatStorage()
                            {
                                ActionID = (uint)MsgCoatStorage.Types.LoadColor,
                                dwpram2 = 1,
                                ID = Id,
                                Remove = 1,
                            };
                        }
                    }
                    Owner.Send(stream.CreateCoatStorage(msg));
                }
            }
        }
        public void ColorFixing(uint Id)
        {
            MsgGameItem item;
            if (Owner.MyWardrobe.TryGetItem(Id, out item))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Coat_Color_Rule.ItemColor DB;
                    if (Coat_Color_Rule.Info.TryGetValue(item.ITEM_ID, out DB))
                    {
                        bool Pay = false;
                        if (Owner.Inventory.Contain(DB.ItemType1, DB.Amount1))
                        {
                            Owner.Inventory.Remove(DB.ItemType1, DB.Amount1, stream);
                            Pay = true;
                        }
                        else if (Owner.Player.ConquerPoints >= DB.RandomChange_CPs)
                        {
                            Owner.Player.ConquerPoints -= (uint)DB.RandomChange_CPs;
                            Pay = true;
                        }
                        if (Pay)
                        {
                            if (Items.ContainsKey(item.ITEM_ID))
                            {
                                foreach (var pColor in Items[item.ITEM_ID])
                                {
                                    if (pColor.RGB == item.RelicAttributes[0].Data)
                                    {
                                        item.RelicAttributes[2].Data = 0;
                                        pColor.ExpiryDate = 0;
                                        var msg = new MsgCoatStorage.CoatStorage()
                                        {
                                            ActionID = (uint)MsgCoatStorage.Types.ColorFix,
                                            ID = Id,
                                        };
                                        Owner.Send(stream.CreateCoatStorage(msg));
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(Owner, stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void ColorFixing2(uint Id, long Time)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Coat_Color_Rule.ItemColor DB;
                if (Coat_Color_Rule.Info.TryGetValue(Id, out DB))
                {
                    bool Pay = false;
                    if (Owner.Inventory.Contain(DB.ItemType1, DB.Amount1))
                    {
                        Owner.Inventory.Remove(DB.ItemType1, DB.Amount1, stream);
                        Pay = true;
                    }
                    else if (Owner.Player.ConquerPoints >= DB.RandomChange_CPs)
                    {
                        Owner.Player.ConquerPoints -= (uint)DB.RandomChange_CPs;
                        Pay = true;
                    }
                    if (Pay)
                    {
                        if (Items.ContainsKey(Id))
                        {
                            foreach (var pColor in Items[Id])
                            {
                                if (pColor.Time == Time)
                                {
                                    pColor.ExpiryDate = 0;
                                    var msg = new MsgCoatStorage.CoatStorage()
                                    {
                                        ActionID = (uint)MsgCoatStorage.Types.STORAGE_COLOR_FIXING2,
                                        ID = Id,
                                    };
                                    Owner.Send(stream.CreateCoatStorage(msg));

                                }
                            }
                        }
                    }
                }
            }
        }
        public void EquipColor(uint Id, long Time)
        {
            MsgGameItem pitem;
            if (Owner.MyWardrobe.TryGetItem(Id, out pitem))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Items.ContainsKey(pitem.ITEM_ID))
                    {
                        var msg = new MsgCoatStorage.CoatStorage()
                        {
                            ActionID = (uint)MsgCoatStorage.Types.STORAGE_EQUIP_COLOR,
                            ID = Id,
                        };
                        if (Time == -1)
                        {
                            var m_item = Items[pitem.ITEM_ID].FirstOrDefault(p => p.ExpiryDate == pitem.RelicAttributes[2].Data && p.RGB == pitem.RelicAttributes[0].Data);
                            if (m_item != null)
                            {
                                DateTime UnixEpoch = new DateTime(1970, 1, 1);
                                uint Timestamp = (uint)(DateTime.UtcNow - UnixEpoch).TotalSeconds;

                                m_item.Time = (uint)Timestamp;
                                msg.MountColor.Add(new MsgCoatStorage.ItemColor { TIMEITEM = m_item.Time });
                            }
                            pitem.RelicAttributes[0].Data = 0;
                            pitem.RelicAttributes[2].Data = 0;
                            Owner.Player.MountArmorColor = pitem.RelicAttributes[0].Data;
                            if (pitem.Position == 17)
                            {
                                MsgUpdate packet = new MsgUpdate(stream, Owner.Player.UID, 1);
                                packet.Append(stream, (MsgUpdate.DataType)135, 0);
                                packet.myUpdates[0].Values = new ulong[4] { pitem.RelicAttributes[0].Data, 0, pitem.RelicAttributes[2].Data, 0 };
                                packet.myUpdates[0].SpellLevel = (int)pitem.ITEM_ID;
                                stream = packet.GetArray(stream);
                                Owner.Player.View.SendView(stream, true);

                            }
                            else
                            {
                                pitem.Mode = Role.Flags.ItemMode.Update;
                                pitem.Send(Owner, stream);
                            }
                            Owner.Send(stream.CreateCoatStorage(msg));
                        }
                        else
                        {
                            var pColor = Items[pitem.ITEM_ID].FirstOrDefault(p => p.Time == Time);
                            if (pColor != null)
                            {
                                var m_item = Items[pitem.ITEM_ID].FirstOrDefault(p => p.ExpiryDate == pitem.RelicAttributes[2].Data && p.RGB == pitem.RelicAttributes[0].Data);
                                if (m_item != null)
                                {
                                    pColor.Time = m_item.Time;
                                    m_item.Time = (uint)Time;

                                }
                                pitem.RelicAttributes[0].Data = pColor.RGB;
                                pitem.RelicAttributes[2].Data = pColor.ExpiryDate;
                                Owner.Player.MountArmorColor = pitem.RelicAttributes[0].Data;
                            }
                            if (pitem.Position == 17)
                            {
                                MsgUpdate packet = new MsgUpdate(stream, Owner.Player.UID, 1);
                                packet.Append(stream, (MsgUpdate.DataType)135, 0);
                                packet.myUpdates[0].Values = new ulong[4] { pitem.RelicAttributes[0].Data, 0, pitem.RelicAttributes[2].Data, 0 };
                                packet.myUpdates[0].SpellLevel = (int)pitem.ITEM_ID;
                                stream = packet.GetArray(stream);
                                Owner.Player.View.SendView(stream, true);

                            }
                            else
                            {
                                pitem.Mode = Role.Flags.ItemMode.Update;
                                pitem.Send(Owner, stream);
                            }
                            Owner.Send(stream.CreateCoatStorage(msg));
                        }
                    }
                }
            }
        }
        public void Load(uint UID)
        {
            Qahr.Mobile.WindowsAPI.IniFile ini = new Qahr.Mobile.WindowsAPI.IniFile("\\CoatColorRule\\" + UID + ".ini");
            int Count = ini.ReadInt32("CoatColorRule", "Count", 0);
            for (int num = 0; num < Count; num++)
            {
                int ItemType = ini.ReadInt32("CoatColorRule", "ItemType" + num.ToString(), 0);
                int ItemCount = ini.ReadInt32("CoatColorRule", "ItemCount" + num.ToString(), 0);
                for (int num2 = 0; num2 < ItemCount; num2++)
                {
                    uint ItemType0 = ini.ReadUInt32(ItemType.ToString(), "ItemType" + num2.ToString(), 0);
                    uint TimeCreate = ini.ReadUInt32(ItemType.ToString(), "TimeCreate" + num2.ToString(), 0);
                    uint ExpiryDate = ini.ReadUInt32(ItemType.ToString(), "ExpiryDate" + num2.ToString(), 0);
                    uint RGB = ini.ReadUInt32(ItemType.ToString(), "RGB" + num2.ToString(), 0);
                    uint ColorType = ini.ReadUInt32(ItemType.ToString(), "ColorType" + num2.ToString(), 0);
                    DateTime UnixEpoch = new DateTime(1970, 1, 1);
                    uint Timestamp = (uint)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
                    if (ExpiryDate == 0 || ExpiryDate > Timestamp)
                    {
                        if (!Items.ContainsKey((uint)ItemType0))
                            Items.Add((uint)ItemType0, new List<CoatColor>());
                        Items[(uint)ItemType0].Add(new CoatColor()
                        {
                            ColorType = ColorType,
                            ExpiryDate = ExpiryDate,
                            ItemType = ItemType0,
                            RGB = RGB,
                            Time = TimeCreate,

                        });

                    }
                }
            }
        }
        public void Save(uint UID)
        {
            Qahr.Mobile.WindowsAPI.IniFile ini = new Qahr.Mobile.WindowsAPI.IniFile("\\CoatColorRule\\" + UID + ".ini");

            ini.Write("CoatColorRule", "Count", Items.Values.Count);
            uint unm = 0;
            foreach (var item in Items)
            {
                ini.Write("CoatColorRule", "ItemType" + unm.ToString(), item.Key);
                ini.Write("CoatColorRule", "ItemCount" + unm.ToString(), item.Value.Count);
                unm++;
                uint unm2 = 0;
                foreach (var pColor in item.Value)
                {
                    ini.Write(pColor.ItemType.ToString(), "ItemType" + unm2.ToString(), pColor.ItemType);
                    ini.Write(pColor.ItemType.ToString(), "TimeCreate" + unm2.ToString(), pColor.Time);
                    ini.Write(pColor.ItemType.ToString(), "ExpiryDate" + unm2.ToString(), pColor.ExpiryDate);
                    ini.Write(pColor.ItemType.ToString(), "RGB" + unm2.ToString(), pColor.RGB);
                    ini.Write(pColor.ItemType.ToString(), "ColorType" + unm2.ToString(), pColor.ColorType);
                    unm2++;
                }
            }

        }
    }
    public class Wardrobe
    {
        public enum ItemsType : byte
        {
            Garment = 0,
            Mount = 1,
            Accessory = 2,// QahrMobile: the mobile wardrobe (CWrapPackageMgr) also keeps weapon accessories (positions 15/16)
            Count = 3
        }

        public ItemsType GetItemType(uint ID)
        {
            if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                return ItemsType.Mount;
            if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Garment)
                return ItemsType.Garment;
            if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                || Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
                return ItemsType.Accessory;// QahrMobile: only the mobile handler (MobileWardrobe) deposits these
            return ItemsType.Count;
        }
        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>[] Items;

        public bool HaveItems()
        {
            foreach (var item in Items)
            {
                if (item.Count > 0)
                    return true;
            }
            return false;
        }

        public Client.GameClient Owner;

        public Wardrobe(Client.GameClient _owner)
        {
            Owner = _owner;
            Items = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>[(int)ItemsType.Count];
            for (int x = 0; x < (int)ItemsType.Count; x++)
                Items[x] = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
        }
        public bool Contain(uint ID)
        {
            foreach (var objects in Items)
            {
                var dictionary = objects;
                foreach (var item in dictionary.Values)
                    if (item.ITEM_ID == ID)
                        return true;
            }
            return false;
        }
        public bool ContainUID(uint UID)
        {
            foreach (var objects in Items)
            {
                var dictionary = objects;
                foreach (var item in dictionary.Values)
                    if (item.UID == UID)
                        return true;
            }
            return false;
        }
        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            Items[(byte)ItemsType.Garment].TryGetValue(UID, out item);
            if (item != null)
                return true;
            Items[(byte)ItemsType.Mount].TryGetValue(UID, out item);
            if (item != null)
                return true;
            Items[(byte)ItemsType.Accessory].TryGetValue(UID, out item);// QahrMobile
            if (item != null)
                return true;
            item = null;
            return false;

        }


        public bool TryGetType(uint Type, out Game.MsgServer.MsgGameItem item)
        {
            item = Items[(byte)ItemsType.Garment].Values.FirstOrDefault(p => p.ITEM_ID == Type);
            if (item != null)
                return true;
            item = Items[(byte)ItemsType.Mount].Values.FirstOrDefault(p => p.ITEM_ID == Type);
            if (item != null)
                return true;
            item = null;
            return false;
        }
        public bool AddItem(Game.MsgServer.MsgGameItem item)
        {
            ItemsType type = GetItemType(item.ITEM_ID);
            if (type == ItemsType.Count)
                return false;

            var dictinary = Items[(int)type];
            if (!dictinary.ContainsKey(item.UID))
            {
                item.WH_ID = 100;
                dictinary.TryAdd(item.UID, item);
                return true;
            }
            return false;
        }
        public bool RemoveItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            item = null;
            foreach (var objects in Items)
            {
                var dictionary = objects;
                bool accrepter = dictionary.TryRemove(UID, out item);
                if (accrepter)
                    break;
            }

            if (item != null)
            {
                item.WH_ID = 0;
                item.Position = 0;
            }

            return item != null;
        }
        public void SendToClient(ServerSockets.Packet stream)
        {
            foreach (var objects in Items)
            {
                var items = objects.Values.Where(p => p.Position == 0).ToList();
                int Counts = items.Count;
                for (int i = 0; i < (items.Count / 10) + 1; i++)
                {
                    MsgCoatStorage.CoatStorage obj = new MsgCoatStorage.CoatStorage();
                    obj.Item = new MsgCoatStorage.ItemStorage[Counts > 10 ? 10 : Counts];
                    for (int x = 0; x < obj.Item.Length && Counts > 0; x++)
                    {
                        obj.Item[x] = new MsgCoatStorage.ItemStorage(items[Counts - 1]);
                        Counts--;
                    }
                    Owner.Send(stream.CreateCoatStorage(obj));
                }
            }
        }
        public void LoadAnthoerWardrobe(ServerSockets.Packet stream, GameClient Client)
        {
            foreach (var objects in Items)
            {
                var items = objects.Values.Where(p => p.Position == 0).ToList();
                int Counts = items.Count;
                for (int i = 0; i < (items.Count / 10) + 1; i++)
                {
                    MsgCoatStorage.CoatStorage obj = new MsgCoatStorage.CoatStorage();
                    obj.ActionID = (uint)MsgCoatStorage.Types.LoadAnthoerWardrobe;
                    obj.Item = new MsgCoatStorage.ItemStorage[Counts > 10 ? 10 : Counts];
                    for (int x = 0; x < obj.Item.Length && Counts > 0; x++)
                    {
                        obj.Item[x] = new MsgCoatStorage.ItemStorage(items[Counts - 1]);
                        Counts--;
                        Client.Send(stream.CreateCoatStorage(obj));
                    }
                }
            }
        }
        public void SendItem(ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();

            Owner.Send(stream.CreateCoatStorage(store));
        }
        public IEnumerable<Game.MsgServer.MsgGameItem> GetAllItems()
        {
            foreach (var objects in Items)
            {
                foreach (var item in objects.Values)
                {
                    yield return item;
                }
                }
        }
        public int GetCountItems()
        {
            int count = 0;
            foreach (var objects in Items)
                count += objects.Count;
            return count;
        }

    }
}
