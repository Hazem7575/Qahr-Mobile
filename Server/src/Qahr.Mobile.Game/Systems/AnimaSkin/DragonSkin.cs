using Qahr.Mobile.Client;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
namespace Qahr.Mobile.Role.Instance
{
    public class DragonSkin
    {
        public enum Style : int
        {
            GoldDragon = 0,
            ReadDragon = 1,
            GreenWhoman = 2,
            BlueDragon = 3,
        }
        public class SkinsInfo
        {
            public Style Style;
            public uint Unlocked;
            public uint Equiped;
            public int UnLockTimer;
        }
        public Client.GameClient Owner;
        private Dictionary<Style, SkinsInfo> Skins;
        private SkinsInfo[] objects;
        public Style MainStyle;
        public DragonSkin(Client.GameClient client)
        {
            Owner = client;
            Skins = new Dictionary<Style, SkinsInfo>()
            {
                { Style.GoldDragon , new SkinsInfo(){ Style = Style.GoldDragon, UnLockTimer = 0 } },
                { Style.ReadDragon , new SkinsInfo(){ Style = Style.ReadDragon, UnLockTimer = 0 }},
                { Style.GreenWhoman , new SkinsInfo(){ Style = Style.GreenWhoman, UnLockTimer = 0 }},
                { Style.BlueDragon , new SkinsInfo(){ Style = Style.BlueDragon, UnLockTimer = 0 }},
            };
            objects = new SkinsInfo[0];
            objects = Skins.Values.ToArray();
            MainStyle = Style.GoldDragon;
        }
        public SkinsInfo[] Objects
        {
            get
            {
                return objects;
            }
        }
        public SkinsInfo[] WornObjects
        {
            get
            {
                List<SkinsInfo> myList = new List<SkinsInfo>();
                foreach (var item in objects)
                {
                    if (item.Unlocked == 0) continue;
                    if (item.Equiped > 0)
                        myList.Add(item);
                }
                return myList.ToArray();
            }
        }
        public SkinsInfo TryGetItem(Style ST)
        {
            return Objects.Where(i => i.Style == ST).FirstOrDefault();
        }
        public bool TryGetItem(Style ST, out SkinsInfo item)
        {
            foreach (SkinsInfo i in objects)
            {
                if (i != null)
                    if (i.Style == ST)
                    {
                        item = i;
                        return true;
                    }
            }
            item = null;
            return false;
        }
        public bool Add(SkinsInfo item)
        {
            if (item.Style == Style.GoldDragon) return true;
            if (!Skins.ContainsKey(item.Style))
            {
                Skins.Add(item.Style, item);
                objects = Skins.Values.ToArray();
                return true;
            }
            else if (Skins.ContainsKey(item.Style))
            {
                Skins[item.Style] = item;
                objects = Skins.Values.ToArray();
                return true;
            }
            return false;
        }
        public bool Equip(SkinsInfo item)
        {
            if (Skins.ContainsKey(item.Style))
            {
                if (WornObjects.Length > 0)
                    Unequip(WornObjects[0]);
                Skins[item.Style].Equiped = 1;
                objects = Skins.Values.ToArray();
                MainStyle = item.Style;
                return true;
            }
            return false;
        }
        public bool Unequip(SkinsInfo item)
        {
            if (Skins.ContainsKey(item.Style))
            {
                Skins[item.Style].Equiped = 0;
                objects = Skins.Values.ToArray();
                MainStyle = Style.GoldDragon;
                return true;
            }
            return false;
        }
        public Style GetStyle()
        {
            if (WornObjects.Length > 0)
                return WornObjects[0].Style;
            return Style.GoldDragon;
        }
        public void RequestInfo(ServerSockets.Packet stream)
        {
            int Count = Objects.Count(i => i.Unlocked == 1);

            var msg = new MsgDragonSkin.MsgDragonSkinProto()
            {
                UID = Owner.Player.UID,
                Type = MsgDragonSkin.ActionID.Load,
                DragonSkinRecord = MainStyle,
                dwParam = new MsgDragonSkin.MsgItemRecord[Count],
            };
            for (int i = 0; i < Count; i++)
            {
                var obj = Objects.Where(w => w.Unlocked == 1).OrderBy(w => w.Style).ToArray()[i];
                msg.dwParam[i] = new MsgDragonSkin.MsgItemRecord()
                {
                    Style = obj.Style,
                    Unlock = obj.Unlocked,
                };
            }
            Owner.Send(stream.CreateDragonSkinInfo(msg));
        }
        public void Load(string line)
        {
            if (line == "")
                return;
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            MainStyle = (Style)reader.Read((uint)0);
            int count = reader.Read((int)0);
            for (int i = 0; i < count; i++)
            {
                Style Style = (Style)reader.Read((uint)0);
                uint Unlocked = reader.Read((uint)0);
                uint Equiped = reader.Read((uint)0);
                Add(new SkinsInfo() { Style = Style, Unlocked = Unlocked, Equiped = Equiped});
            }
        }
        public override string ToString()
        {
            var file = new Database.DBActions.WriteLine('/');
            file.Add((uint)MainStyle);
            file.Add(objects.Length);
            foreach (var item in objects)
            {
                file.Add((uint)item.Style);
                file.Add(item.Unlocked);
                file.Add(item.Equiped);
            }
            return file.Close();
        }
    }
}