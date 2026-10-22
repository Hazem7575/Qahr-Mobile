using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.ServerSockets;

namespace Qahr.Mobile.Role.Instance
{
    public class CollectionStorge
    {
        public ConcurrentDictionary<uint, item> items;
        public ConcurrentDictionary<uint, Scrap> Scraps;
        public class item
        {
            public uint ID = 0;

            public uint Actived = 0;

            public uint Progress = 0;

            public uint RemainSec = 0;

            public byte Use = 0;

        }
        public class Scrap
        {
            public uint ID = 0;

            public uint Counts = 0;
        }
        public Client.GameClient Owner;
        public CollectionStorge(Client.GameClient _Owner)
        {
            Owner = _Owner;
            items = new ConcurrentDictionary<uint, item>();
            Scraps = new ConcurrentDictionary<uint, Scrap>();
        }

        public void Load()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MsgCollectionStorage.ProtoStructure Info = new MsgCollectionStorage.ProtoStructure();

                Info.Action = MsgCollectionStorage.Action.Show;

                if (Owner.Player.CollectionID != 0)
                    Info.ID = Owner.Player.CollectionID;

                if (Owner.Player.PandaID != 0)
                    Info.PandaPonPon = Owner.Player.PandaID;

                Info.Items = new List<MsgCollectionStorage.MsgCollectionStorageInfoPB>();
                Info.ScarpsInfo = new List<MsgCollectionStorage.Scrap>();

                foreach (var itemCollection in items.Values)
                {

                    Info.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                    {
                        ID = itemCollection.ID,
                        Actived = itemCollection.Actived,
                        Progress = itemCollection.Progress,
                        // RemainSec = itemCollection.RemainSec,
                        Use = itemCollection.Use
                    });
                }
                foreach (var ScrapsCollection in Scraps.Values)
                {
                    Info.ScarpsInfo.Add(new MsgCollectionStorage.Scrap()
                    {
                        ID = ScrapsCollection.ID,
                        Counts = ScrapsCollection.Counts
                    });
                }
                Info.UID = Owner.Player.UID;
                Owner.Send(stream.CreatCollection(Info));
            }
        }
        public void LoadingCollectionAnother(Client.GameClient Client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MsgCollectionStorage.ProtoStructure Info = new MsgCollectionStorage.ProtoStructure();

                Info.Action = MsgCollectionStorage.Action.Show;
                var UseIt = items.Values.Where(p => p.Use == 1).FirstOrDefault();
                if (UseIt != null)
                    Info.ID = UseIt.ID;
                else
                    Info.ID = 0;

                Info.Items = new List<MsgCollectionStorage.MsgCollectionStorageInfoPB>();
                Info.ScarpsInfo = new List<MsgCollectionStorage.Scrap>();

                foreach (var itemCollection in items.Values)
                {

                    Info.Items.Add(new MsgCollectionStorage.MsgCollectionStorageInfoPB()
                    {
                        ID = itemCollection.ID,
                        Actived = itemCollection.Actived,
                        Progress = itemCollection.Progress,
                        //RemainSec = itemCollection.RemainSec,
                        Use = itemCollection.Use
                    });
                }
                foreach (var ScrapsCollection in Scraps.Values)
                {
                    Info.ScarpsInfo.Add(new MsgCollectionStorage.Scrap()
                    {
                        ID = ScrapsCollection.ID,
                        Counts = ScrapsCollection.Counts
                    });
                }
                Info.UID = Owner.Player.UID;
                Client.Send(stream.CreatCollection(Info));
            }
        }
        public override string ToString()
        {
            WriteLine file = new WriteLine('/');
            file.Add(items.Count);
            foreach (var Item in items.Values)
            {
                file.Add(Item.ID);
                file.Add(Item.Actived);
                file.Add(Item.Progress);
                //file.Add(Item.RemainSec);
                file.Add(Item.Use);
            }
            file.Add(Scraps.Count);
            foreach (var Scrap in Scraps.Values)
            {
                file.Add(Scrap.ID);
                file.Add(Scrap.Counts);
            }
            return file.Close();
        }
        public void Load(string line)
        {
            ReadLine reader = new ReadLine(line, '/');
            int count = reader.Read(0);
            for (int i = 0; i < count; i++)
            {
                var ItemCollection = new item
                {
                    ID = reader.Read((uint)0),
                    Actived = reader.Read((uint)0),
                    Progress = reader.Read((uint)0),
                    // RemainSec = reader.Read((uint)0),
                    Use = reader.Read((byte)0),
                };
                if (!items.ContainsKey(ItemCollection.ID))
                {
                    items.Add(ItemCollection.ID, ItemCollection);
                }

            }
            int counts = reader.Read(0);
            for (int i = 0; i < counts; i++)
            {
                var ScrapsCollection = new Scrap
                {
                    ID = reader.Read((uint)0),
                    Counts = reader.Read((uint)0),
                };
                if (!Scraps.ContainsKey(ScrapsCollection.ID))
                {
                    Scraps.Add(ScrapsCollection.ID, ScrapsCollection);
                }
            }

        }
    }
}