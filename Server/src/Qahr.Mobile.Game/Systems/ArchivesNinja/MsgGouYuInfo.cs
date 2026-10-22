using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgGouYuInfo
    {
      
        [ProtoContract]
        public class MsgGouYuInfoProto
        {
            [Flags]
            public enum TypeID
            {
                Load = 0,
                View = 1,
                Add = 2,
                Update = 3,
            }
            [ProtoMember(1, IsRequired = true)]
            public TypeID Type = 0;
            [ProtoMember(2, IsRequired = true)]
            public uint UID = 0;
            [ProtoMember(3, IsRequired = true)]
            public Item[] Items;
            [ProtoContract]
            public class Item
            {
                [ProtoMember(1, IsRequired = true)]
                public uint ID = 0;
                [ProtoMember(2, IsRequired = true)]
                public uint Level = 0;
                [ProtoMember(3, IsRequired = true)]
                public uint Position = 0;
                [ProtoMember(4, IsRequired = true)]
                public uint Points = 0;
                [ProtoMember(5, IsRequired = true)]
                public uint UK = 0;
            }
            public static void Create(GameClient user, Ninja _NinjaSprint, TypeID Type, uint UID)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgGouYuInfoProto obj = new MsgGouYuInfoProto();
                    obj.Type = Type;
                    obj.UID = UID;
                    obj.Items = new MsgGouYuInfoProto.Item[_NinjaSprint.Items.Count];
                    int i = 0; foreach (var Item in _NinjaSprint.Items.Values)
                    {
                        obj.Items[i] = new MsgGouYuInfoProto.Item();
                        obj.Items[i].ID = Item.ItemID;
                        obj.Items[i].Level = Item.Level;
                        obj.Items[i].Position = Item.Position;
                        obj.Items[i].Points = Item.Exp;
                        obj.Items[i].UK = Item.UK; i++;
                    }
                    user.Send(stream.CreateNinjaItem(obj));
                }
            }
            public static void Create(GameClient user, Ninja.Item Item, TypeID Type)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgGouYuInfoProto obj = new MsgGouYuInfoProto();
                    obj.Type = Type;
                    obj.UID = user.Player.UID;
                    obj.Items = new MsgGouYuInfoProto.Item[1];
                    obj.Items[0] = new MsgGouYuInfoProto.Item();
                    obj.Items[0].ID = Item.ItemID;
                    obj.Items[0].Level = Item.Level;
                    obj.Items[0].Position = Item.Position;
                    obj.Items[0].Points = Item.Exp;
                    user.Send(stream.CreateNinjaItem(obj));
                }
            }
        }
        public static unsafe ServerSockets.Packet CreateNinjaItem(this ServerSockets.Packet stream, MsgGouYuInfoProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGouYuInfo);
            return stream;
        }
        public static unsafe void GetNinjaItem(this ServerSockets.Packet stream, out MsgGouYuInfoProto pQuery)
        {
            pQuery = new MsgGouYuInfoProto();
            pQuery = stream.ProtoBufferDeserialize<MsgGouYuInfoProto>(pQuery);
        }
        
    }
}
