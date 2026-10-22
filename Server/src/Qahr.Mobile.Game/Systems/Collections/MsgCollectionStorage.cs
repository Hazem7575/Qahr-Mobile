using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Collections.Concurrent;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.Client;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game;
using Qahr.Mobile.ServerSockets;
using Qahr.Mobile;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreatCollection(this ServerSockets.Packet stream, MsgCollectionStorage.ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCollectionStorage);
            return stream;

        }

        public static void GetCollection(this ServerSockets.Packet stream, out MsgCollectionStorage.ProtoStructure pQuery)
        {
            pQuery = new MsgCollectionStorage.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgCollectionStorage.ProtoStructure>(pQuery);
        }
    }
    public static class MsgCollectionStorage
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public Action Action = Action.Show;

            [ProtoMember(2, IsRequired = true)]
            public uint ID = 0;

            [ProtoMember(3, IsRequired = true)]
            public List<Scrap> ScarpsInfo;

            [ProtoMember(4, IsRequired = true)]
            public List<MsgCollectionStorageInfoPB> Items = new List<MsgCollectionStorageInfoPB>();

            [ProtoMember(5, IsRequired = true)]
            public uint UID = 0;

            [ProtoMember(6, IsRequired = true)]
            public uint PandaPonPon = 0;
        }
        [ProtoContract]
        public class Scrap
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ID = 0;

            [ProtoMember(2, IsRequired = true)]
            public uint Counts = 0;
        }

        [ProtoContract]
        public class MsgCollectionStorageInfoPB
        {

            [ProtoMember(1, IsRequired = true)]
            public uint ID = 0;

            [ProtoMember(2, IsRequired = true)]
            public uint Actived = 0;

            [ProtoMember(3, IsRequired = true)]
            public uint Progress = 0;

            [ProtoMember(4, IsRequired = true)]
            public uint RemainSec = 0;

            [ProtoMember(5, IsRequired = true)]
            public byte Use = 0;


        }
        [Flags]
        public enum Action : uint
        {
            Show = 0,
            Active = 1,
            Gear = 2,
            Takeoff = 3,
            TakeOut = 4,
            AddToCollection = 5,
            Add = 6,
            TakeOutExpire = 7,
            GiveAway = 8,
            GiveAwayWithCps = 10,
            Summon = 11,
            Recall = 12,
        }
        public static int Timestamp()
        {
            return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static uint Timestamp(uint expiration_date)
        {
            int Time = Timestamp();
            uint expiration = 0;
            if (Time < (long)expiration_date)
                expiration = (uint)((ulong)expiration_date - (ulong)Time);
            return expiration;
        }
        public static bool TryGetValue(Client.GameClient client, uint ItemID)
        {
            Role.Instance.CollectionStorge.Scrap scrap_item;
            Role.Instance.CollectionStorge.item item;
            scrap_item = null;
            if ((ItemID / 10000 == 430 && client.Collection.Scraps.TryGetValue(3326259, out scrap_item))
                 || (ItemID / 10000 == 430 && client.Collection.Scraps.TryGetValue(3326260, out scrap_item))
                 || (ItemID / 10000 == 430 && client.Collection.Scraps.TryGetValue(3326261, out scrap_item))
                 || (ItemID / 10000 == 430 && client.Collection.Scraps.TryGetValue(3326262, out scrap_item))
                 || (ItemID / 10000 == 430 && client.Collection.Scraps.TryGetValue(3326263, out scrap_item))
                 || (ItemID / 10000 == 431 && client.Collection.Scraps.TryGetValue(3336144, out scrap_item))
                 || (ItemID / 10000 == 431 && client.Collection.Scraps.TryGetValue(3336145, out scrap_item))
                 || (ItemID / 10000 == 431 && client.Collection.Scraps.TryGetValue(3336146, out scrap_item))
                 || (ItemID / 10000 == 432 && client.Collection.Scraps.TryGetValue(3326046, out scrap_item))
                 || (ItemID / 10000 == 432 && client.Collection.Scraps.TryGetValue(3336737, out scrap_item))
                 || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3342835, out scrap_item))
                 || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3346022, out scrap_item))
                 || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3348179, out scrap_item))
                 || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3349734, out scrap_item))
                 || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3351450, out scrap_item))
                  || (ItemID / 10000 == 433 && client.Collection.Scraps.TryGetValue(3352547, out scrap_item)))
            {
                return true;
            }
            if (client.Collection.items.TryGetValue(ItemID, out item))
            {
                if (item.Progress >= 100)
                    return true;
                else return false;
            }
            return false;
        }
        [PacketAttribute(GamePackets.MsgCollectionStorage)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
                return;
            }
            ProtoStructure pQuery;
            stream.GetCollection(out pQuery);
            switch ((Action)pQuery.Action)
            {

                case Action.Show:
                    {
                        GameClient user;
                        if (Pool.GamePoll.TryGetValue(pQuery.UID, out user))
                        {
                            foreach (var atitle in user.Collection.items.Values)
                            {
                                {
                                    pQuery.UID = user.Player.UID;

                                    pQuery.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                    {
                                        ID = atitle.ID,
                                        Actived = atitle.Actived,
                                        Progress = atitle.Progress,
                                        // RemainSec = atitle.RemainSec,
                                        Use = atitle.Use
                                    });

                                    client.Send(stream.CreatCollection(pQuery));
                                }
                            }
                        }
                        break;
                    }
                case Action.Summon:
                    {
                        Role.Instance.CollectionStorge.item Info;
                        if (client.Collection.items.TryGetValue(pQuery.ID, out Info))
                        {
                            if (Info.ID == pQuery.ID)
                            {
                                pQuery.UID = client.Player.UID;
                                client.Player.PandaID = pQuery.ID;
                                pQuery.PandaPonPon = pQuery.ID;
                                Info.Use = 1;
                            }
                        }

                        client.Send(stream.CreatCollection(pQuery));
                        break;
                    }
                case Action.Recall:
                    {
                        Role.Instance.CollectionStorge.item Info;
                        if (client.Collection.items.TryGetValue(pQuery.ID, out Info))
                        {
                            if (Info.ID == pQuery.ID)
                            {
                                Info.Use = 0;
                                pQuery.ID = 0;
                                pQuery.PandaPonPon = 0;
                                pQuery.UID = client.Player.UID;
                                client.Player.PandaID = 0;
                                client.Send(stream.CreatCollection(pQuery));
                            }
                        }
                        break;
                    }
                case Action.Active:
                    {
                        var Item = client.Collection.items.Values.Where(p => p.ID == pQuery.ID).FirstOrDefault();
                        if (Item == null)
                        {
                            client.Collection.items.TryAdd(pQuery.ID, new Role.Instance.CollectionStorge.item()
                            {
                                ID = pQuery.ID
                            });
                            Item = client.Collection.items[pQuery.ID];
                            client.Player.CollectionID = Item.ID;
                        }
                        if (Item == null)
                            break;
                        uint amount = 0;
                        Role.Instance.CollectionStorge.Scrap scrap_item = null;
                        if (TryGetValue(client, Item.ID))
                        {
                            if (scrap_item != null)
                                amount += scrap_item.Counts;
                            else
                                amount += Item.Progress;
                            if (amount >= 100)
                            {
                                Item.Actived = 1;

                                if (scrap_item != null)
                                {
                                    scrap_item.Counts -= 100 - Item.Progress;
                                    Item.Progress = 0;
                                }
                                var msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = pQuery.ID
                                };
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = Item.ID,
                                    Progress = Item.Progress,
                                    Actived = Item.Actived
                                });
                                msg.ScarpsInfo = new List<Scrap>();
                                foreach (var scrap in client.Collection.Scraps.Values)
                                {
                                    msg.ScarpsInfo.Add(new Scrap()
                                    {
                                        ID = scrap.ID,
                                        Counts = scrap.Counts
                                    });
                                }
                                client.Send(stream.CreatCollection(msg));
                            }
                        }
                        break;
                    }
                case Action.AddToCollection:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
                            return;
                        }
                        MsgGameItem item = null;
                        if (client.Inventory.TryGetItem(pQuery.ID, out item))
                        {
                            if (item.ITEM_ID == 3326259 || item.ITEM_ID == 3326260 || item.ITEM_ID == 3326261 || item.ITEM_ID == 3326262 || item.ITEM_ID == 3326263 || item.ITEM_ID == 3336144 || item.ITEM_ID == 3336145
                                || item.ITEM_ID == 3336146 || item.ITEM_ID == 3341899 || item.ITEM_ID == 3341900 || item.ITEM_ID == 3344551 || item.ITEM_ID == 3342835 || item.ITEM_ID == 3347144 || item.ITEM_ID == 3348179
                                || item.ITEM_ID == 3351450 || item.ITEM_ID == 3352547 || item.ITEM_ID == 3346022 || item.ITEM_ID == 3349734 || item.ITEM_ID == 3354664)
                            {
                                //client.CreateBoxDialog("Exchange All Scraps From NPC [Collections] In Twin City At Coordinates (398:378)");
                                return;
                            }
                            #region iTEMCollection

                            if (item.ITEM_ID == 4320002 || item.ITEM_ID >= 4300000 && item.ITEM_ID <= 4300004 || item.ITEM_ID >= 4310000 && item.ITEM_ID <= 4310002 || item.ITEM_ID >= 4320000 && item.ITEM_ID <= 4320001 || item.ITEM_ID >= 4330000 && item.ITEM_ID <= 4330004 || item.ITEM_ID >= 4331000 && item.ITEM_ID <= 4332000 || item.ITEM_ID == 4330005 || item.ITEM_ID == 4300005 || item.ITEM_ID == 4330007 || item.ITEM_ID == 4330008 || item.ITEM_ID == 4330009 || item.ITEM_ID == 4330010 || item.ITEM_ID == 4330011
                                || item.ITEM_ID == 4330012 || item.ITEM_ID == 4330013 || item.ITEM_ID == 4330014 || item.ITEM_ID == 4330015 || item.ITEM_ID == 3356710
                                || item.ITEM_ID == 3356765 || item.ITEM_ID == 3356766 || item.ITEM_ID == 3356767 || item.ITEM_ID == 3356768 || item.ITEM_ID == 3356769
                                || item.ITEM_ID == 3356770 || item.ITEM_ID == 3356771 || item.ITEM_ID == 3356772)
                            {
                                if (!client.Collection.items.ContainsKey(item.ITEM_ID))
                                {
                                    TimeSpan timeSpan = item.TimeStamp - DateTime.Now;

                                    var itemsCollection = new Role.Instance.CollectionStorge.item
                                    {
                                        ID = item.ITEM_ID,
                                        Actived = 1,
                                        Progress = item.StackSize,
                                        //RemainSec = (uint)timeSpan.TotalSeconds,
                                        Use = 0
                                    };
                                    client.Collection.items.Add(item.ITEM_ID, itemsCollection);
                                    var Infos = new MsgCollectionStorage.ProtoStructure()
                                    {
                                        Action = Action.Add,
                                        ID = item.ITEM_ID,
                                        ScarpsInfo = new List<Scrap>(),
                                        Items = new List<MsgCollectionStorageInfoPB>()
                                    };
                                    Infos.Items.Add(new MsgCollectionStorageInfoPB()
                                    {
                                        ID = itemsCollection.ID,
                                        Actived = itemsCollection.Actived,
                                        Progress = itemsCollection.Progress,
                                        //  RemainSec = (uint)timeSpan.TotalSeconds,
                                        Use = itemsCollection.Use
                                    });
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                    client.Send(stream.CreatCollection(Infos));
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                    UpdateSkillCollection(client, false);
                                }
                            }
                            else if (item.ITEM_ID >= 3326259 && item.ITEM_ID <= 3326263)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3326260)
                                    key = 4300001;
                                else if (item.ITEM_ID == 3326261)
                                    key = 4300002;
                                else if (item.ITEM_ID == 3326262)
                                    key = 4300003;
                                else if (item.ITEM_ID == 3326263)
                                    key = 4300004;
                                else if (item.ITEM_ID == 3326259)
                                    key = 4300000;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });
                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID >= 3336144 && item.ITEM_ID <= 3336146)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3336144)
                                    key = 4310000;
                                else if (item.ITEM_ID == 3336145)
                                    key = 4310001;
                                else if (item.ITEM_ID == 3336146)
                                    key = 4310002;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });
                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID >= 3341899 && item.ITEM_ID <= 3341900)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3341899)
                                    key = 4320000;
                                else if (item.ITEM_ID == 3341900)
                                    key = 4320001;
                                else if (item.ITEM_ID == 3341900)
                                    key = 4320002;

                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });
                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3342835)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3342835)
                                    key = 4330000;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3351450)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3352547)
                                    key = 4330004;

                                if (item.ITEM_ID == 3354664)
                                    key = 4330005;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3351450)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3351450)
                                    key = 4330003;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3349734)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3349734)
                                    key = 4332000;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3348179)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3348179)
                                    key = 4330002;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3346022)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.ITEM_ID,
                                    PandaPonPon = item.ITEM_ID,
                                };
                                uint key = 0;
                                if (item.ITEM_ID == 3346022)
                                    key = 4331000;
                                Role.Instance.CollectionStorge.item obj;
                                if (client.Collection.items.ContainsKey(key))
                                {
                                    obj = client.Collection.items[key];
                                    if (obj.Actived == 1)
                                        break;
                                    obj.Progress += item.StackSize;
                                }
                                else
                                {
                                    obj = new Role.Instance.CollectionStorge.item()
                                    {
                                        ID = key,
                                        Actived = 0,
                                        Progress = (uint)item.StackSize,
                                        //RemainSec = item.RemainingTime
                                    };
                                    client.Collection.items.TryAdd(obj.ID, obj);
                                }
                                msg.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                                {
                                    ID = obj.ID,
                                    Progress = obj.Progress
                                });

                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            else if (item.ITEM_ID == 3326046 || item.ITEM_ID == 3336737)
                            {
                                MsgCollectionStorage.ProtoStructure msg = new MsgCollectionStorage.ProtoStructure()
                                {
                                    Action = Action.Add,
                                    ID = item.UID
                                };
                                if (client.Collection.Scraps.ContainsKey(item.ITEM_ID))
                                {
                                    var scrap = client.Collection.Scraps[item.ITEM_ID];
                                    scrap.Counts += (uint)item.StackSize;
                                    msg.ScarpsInfo = new List<Scrap>();
                                    msg.ScarpsInfo.Add(new MsgCollectionStorage.Scrap()
                                    {
                                        ID = scrap.ID,
                                        Counts = scrap.Counts
                                    });
                                }
                                else
                                {
                                    client.Collection.Scraps.TryAdd(item.ITEM_ID, new Role.Instance.CollectionStorge.Scrap()
                                    {
                                        ID = item.ITEM_ID,
                                        Counts = (uint)item.StackSize
                                    });
                                    msg.ScarpsInfo = new List<Scrap>();
                                    msg.ScarpsInfo.Add(new MsgCollectionStorage.Scrap()
                                    {
                                        ID = item.ITEM_ID,
                                        Counts = (uint)item.StackSize
                                    });
                                }
                                client.Inventory.Remove(item.ITEM_ID, (uint)item.StackSize, stream);
                                client.Send(stream.CreatCollection(msg));
                            }
                            #endregion
                        }
                        break;
                    }
                case Action.Gear:
                    {
                        Role.Instance.CollectionStorge.item Info;
                        if (client.Collection.items.TryGetValue(pQuery.ID, out Info))
                        {
                            if (Info.ID == pQuery.ID)
                            {
                                client.Player.CollectionID = Info.ID;
                                Info.Use = 1;
                                pQuery.UID = client.Player.UID;
                                client.Send(stream.CreatCollection(pQuery));
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }

                        break;
                    }
                case Action.Takeoff:
                    {
                        Role.Instance.CollectionStorge.item Info;
                        if (client.Collection.items.TryGetValue(pQuery.ID, out Info))
                        {
                            if (Info.ID == pQuery.ID)
                            {
                                client.Player.CollectionID = 0;
                                Info.Use = 0;
                                pQuery.ID = 0;
                                pQuery.UID = client.Player.UID;
                                client.Send(stream.CreatCollection(pQuery));
                            }
                        }

                        break;
                    }
                case Action.TakeOut:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        Role.Instance.CollectionStorge.item Info;
                        if (client.Collection.items.TryGetValue(pQuery.ID, out Info))
                        {
                            if (Info.ID == pQuery.ID)
                            {
                                Info.Use = 0;
                                client.Collection.items.Remove(Info.ID);

                                client.Inventory.AddItemWitchStack(Info.ID, 0, 1, stream);

                                client.Send(stream.CreatCollection(pQuery));
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }
                        break;
                    }

            }
        }
        public static void UpdateSkillCollection(Client.GameClient user, bool Remove = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                int AromaCount = user.Collection.items.Values.Count(p => p.ID >= 4300000 && p.ID <= 4300004);
                int FluteCount = user.Collection.items.Values.Count(p => p.ID >= 4310000 && p.ID <= 4310002);
                int DivineCount = user.Collection.items.Values.Count(p => p.ID >= 4320000 && p.ID <= 4320003);
                int DivineCounts = user.Collection.items.Values.Count(p => p.ID >= 4330000 && p.ID <= 4320004);
                if (Remove)
                {
                    if (AromaCount < 5)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidShelter))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.SolidShelter, stream);
                        }
                    }

                    if (FluteCount < 1)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastShield))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastShield, stream);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastControl))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastControl, stream);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastMastery))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastMastery, stream);
                        }
                    }
                    if (FluteCount == 1)
                    {

                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastControl))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastControl, stream);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastMastery))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastMastery, stream);
                        }
                    }
                    if (FluteCount == 2)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastMastery))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.BeastMastery, stream);
                        }
                    }
                    if (DivineCount < 2)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngelicTones))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.AngelicTones, stream);
                        }
                    }
                    if (DivineCount < 1)
                    {
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EntrancingTones))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.EntrancingTones, stream);
                        }
                        if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngelicTones))
                        {
                            user.MySpells.Remove((ushort)Role.Flags.SpellID.AngelicTones, stream);
                        }
                    }
                }
                else
                {
                    if (AromaCount >= 5)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SolidShelter))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SolidShelter);
                        }
                    }
                    if (FluteCount == 1)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastShield))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BeastShield);
                        }
                    }
                    if (FluteCount == 2)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastControl))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BeastControl);
                        }
                    }
                    if (FluteCount == 3)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BeastMastery))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BeastMastery);
                        }
                    }

                    if (DivineCount == 1)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EntrancingTones))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EntrancingTones);
                        }
                    }
                    if (DivineCount == 2)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngelicTones))
                        {
                            user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AngelicTones);
                        }
                    }
                    if (DivineCounts == 1)
                    {
                        if (!user.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Enums.SpellID.DragonFlight))
                        {
                            user.MySpells.Add(stream, (ushort)Qahr.Mobile.Role.Enums.SpellID.DragonFlight);
                        }
                    }



                }

            }
        }

    }
}
