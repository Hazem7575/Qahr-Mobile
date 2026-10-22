using Qahr.Mobile.Client;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateGift(this ServerSockets.Packet stream, MsgRemotePrize obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgRemotePrize);
            return stream;
        }
    }
    [ProtoContract]
    public class MsgRemotePrize
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Type = 0;
        [ProtoMember(2, IsRequired = true)]
        public uint ServerID = 0;
        [ProtoMember(3, IsRequired = true)]
        public uint UserID = 0;
        [ProtoMember(4)]
        public uint Mesh = 0;
        [ProtoMember(5)]
        public string Name = "";
        [ProtoMember(6)]
        public ulong Money = 0;

        [PacketAttribute(GamePackets.MsgRemotePrize)]
        private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgRemotePrize Info = new MsgRemotePrize();
            Info = stream.ProtoBufferDeserialize<MsgRemotePrize>(Info);
            switch (Info.Type)
            {
                case 0://Search
                    {
                        Client.GameClient GETUSER;
                        if (Pool.GamePoll.TryGetValue(Info.UserID, out GETUSER))
                        {
                            Info.Mesh = GETUSER.Player.Mesh;
                            Info.Name = GETUSER.Player.Name;
                            user.Send(stream.CreateGift(Info));
                        }
                        else
                        {
                            Info.Type = 2;//Offline
                            user.Send(stream.CreateGift(Info));
                        }
                        break;
                    }
                case 1://Send
                    {
                        Client.GameClient GETUSER;
                        if (Pool.GamePoll.TryGetValue(Info.UserID, out GETUSER))
                        {
                            if (Info.Money >= 100000)
                            {
                                if (Info.Money < 0)
                                    return;
                                if (user.Player.Money >= Info.Money)
                                {
                                    user.Player.Money -= Info.Money;
                                    GETUSER.Player.Money += Info.Money;
                                    user.Send(stream.CreateGift(Info));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}