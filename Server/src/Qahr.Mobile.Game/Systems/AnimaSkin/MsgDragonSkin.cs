using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Linq;
using System.Collections.Generic;
namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgDragonSkin
    {
        [ProtoContract]
        public class MsgDragonSkinProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2, IsRequired = true)]
            public uint UID;
            [ProtoMember(3, IsRequired = true)]
            public DragonSkin.Style DragonSkinRecord;
            [ProtoMember(4, IsRequired = true)]
            public bool Confirm;
            [ProtoMember(5, IsRequired = true)]
            public MsgItemRecord[] dwParam;
        }
        [ProtoContract]
        public class MsgItemRecord
        {
            [ProtoMember(1, IsRequired = true)]
            public DragonSkin.Style Style;
            [ProtoMember(2, IsRequired = true)]
            public uint Unlock;
        }
        public enum ActionID : int
        {
            Load,
            ViewEquip,
            Unlock,
            Retieve,
            Select,
            Restore,
            Effect,
        }
        public static unsafe ServerSockets.Packet CreateDragonSkinInfo(this ServerSockets.Packet stream, MsgDragonSkinProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgDragonSkin);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgDragonSkin)]
        public static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgDragonSkinProto msg = new MsgDragonSkinProto();
            msg = stream.ProtoBufferDeserialize<MsgDragonSkinProto>(msg);
            switch (msg.Type)
            {
                case ActionID.ViewEquip:
                    {
                    
                        var myTimer = new System.Timers.Timer();
                        if (msg.UID <= 0)
                        {
                            msg.DragonSkinRecord = client.DragonSkin.GetStyle();
                            msg.UID = client.Player.UID;
                            client.Send(stream.CreateDragonSkinInfo(msg));
                        }
                        else if (Pool.GamePoll.ContainsKey(msg.UID))
                        {
                            myTimer.Elapsed += (o, ea) =>
                            {
                                // Runs ~200ms later on a ThreadPool thread, after Game_Receive has already
                                // returned the received `stream` to the recycle bin -> use our own packet.
                                try
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var timerStream = rec.GetStream();
                                        msg.DragonSkinRecord = Pool.GamePoll[msg.UID].DragonSkin.GetStyle();
                                        client.Send(timerStream.CreateDragonSkinInfo(msg));
                                        client.Send(timerStream.CreateDragonSkinInfo(new MsgDragonSkin.MsgDragonSkinProto()
                                        {
                                            Type = ActionID.Effect,
                                            UID = Pool.GamePoll[msg.UID].Player.UID,
                                            DragonSkinRecord = Pool.GamePoll[msg.UID].DragonSkin.GetStyle()
                                        }));
                                    }
                                }
                                finally
                                {
                                    myTimer.Stop();
                                    myTimer.Dispose();
                                }
                            };
                            myTimer.Interval = 200;
                            myTimer.Start();
                        }
                        break;
                    }
                case ActionID.Unlock://unlock
                    {
                        uint AnimaId = 0;
                        switch (msg.DragonSkinRecord)
                        {
                            case DragonSkin.Style.GreenWhoman: AnimaId = 4200016; break;
                            case DragonSkin.Style.BlueDragon: AnimaId = 4200017; break;
                            case DragonSkin.Style.ReadDragon: AnimaId = 4200018; break;
                            default: return;
                        }
                        if (client.Inventory.Contain(AnimaId, 1))
                        {
                            if (client.DragonSkin.Add(new DragonSkin.SkinsInfo() { Style = msg.DragonSkinRecord, Unlocked = 1 }))
                            {
                                msg.UID = client.Player.UID;
                                msg.Confirm = true;
                            }
                            if (msg.Confirm)
                                client.Inventory.Remove(AnimaId, 1, stream);
                        }
                        client.Send(stream.CreateDragonSkinInfo(msg));
                        client.Send(stream.CreateDragonSkinInfo(new MsgDragonSkinProto()
                        {
                            Type = ActionID.Effect,
                            UID = client.Player.UID,
                            DragonSkinRecord = client.DragonSkin.GetStyle()
                        }));
                        break;
                    }
                case ActionID.Select:
                    {
                        DragonSkin.SkinsInfo Skin;
                        if (client.DragonSkin.TryGetItem(msg.DragonSkinRecord, out Skin))
                        {
                            if (client.DragonSkin.Equip(Skin))
                            {
                                msg.Confirm = true;
                                msg.UID = client.Player.UID;
                            }
                        }
                        client.Send(stream.CreateDragonSkinInfo(msg));
                        client.Send(stream.CreateDragonSkinInfo(new MsgDragonSkinProto()
                        {
                            Type = ActionID.Effect,
                            UID = client.Player.UID,
                            DragonSkinRecord = client.DragonSkin.GetStyle()
                        }));
                        break;
                    }
                case ActionID.Load:
                    {
                        client.Send(stream.CreateDragonSkinInfo(msg));
                        client.Send(stream.CreateDragonSkinInfo(new MsgDragonSkinProto()
                        {
                            Type = ActionID.Effect,
                            UID = client.Player.UID,
                            DragonSkinRecord = client.DragonSkin.GetStyle()
                        }));
                        break;
                    }
                case ActionID.Retieve:
                    {
                        var SelectedStyle = client.DragonSkin.Objects.Where(p => p.Style == msg.DragonSkinRecord).FirstOrDefault();
                        if (SelectedStyle != null && SelectedStyle.Unlocked == 1)
                        {
                            uint AnimaId = 0;
                            int Days = 0;
                            switch (msg.DragonSkinRecord)
                            {
                                case DragonSkin.Style.GreenWhoman: AnimaId = 4200016;
                                    Days = 2;
                                    break;
                                case DragonSkin.Style.BlueDragon: AnimaId = 4200017;
                                    break;
                                case DragonSkin.Style.ReadDragon: AnimaId = 4200018;
                                    Days = 5;
                                    break;
                                default: return;
                            }
                            if (SelectedStyle.Equiped > 0)
                            {
                                if (!client.DragonSkin.Unequip(SelectedStyle))
                                    break;
                            }
                            if (Days > 0)
                            {
                                if (client.Inventory.AddAnimaLock(stream, AnimaId, Role.Core.CreateTimer(DateTime.Now.AddDays(Days)), 2))
                                {
                                    msg.Confirm = true;
                                }
                            }
                            else if (client.Inventory.Add(stream, AnimaId, 1))
                                msg.Confirm = true;
                            else
                                msg.Confirm = false;
                            msg.UID = client.Player.UID;
                            if (msg.Confirm)
                                SelectedStyle.Unlocked = 0;
                        }
                        client.Send(stream.CreateDragonSkinInfo(msg));
                        break;
                    }
                case ActionID.Restore:
                    {
                        if (client.DragonSkin.WornObjects.Length > 0)
                        {
                            if (client.DragonSkin.Unequip(client.DragonSkin.WornObjects[0]))
                            {
                                msg.Confirm = true;
                                msg.UID = client.Player.UID;
                            }
                        }
                        client.Send(stream.CreateDragonSkinInfo(msg));
                        break;
                    }
                default:
                    {
                        Console.WriteLine("MsgDragonSkin - > Type {0}", msg.Type);
                        break;
                    }
            }
        }
    }
}