using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class MsgRelation
    {
        public static unsafe ServerSockets.Packet RelationCreate(this ServerSockets.Packet stream, Role.Player Requester, Role.Player Receiver)
        {
            stream.InitWriter();
            stream.Write(Requester.UID);//4
            stream.Write(Receiver.UID);//8
            stream.Write((uint)Requester.Level);//12
            stream.Write(Requester.BattlePower);//16
            stream.Write(Receiver.SpouseUID == Requester.SpouseUID && Receiver.SpouseUID > 0);//20
            stream.Write(Receiver.Associate.Contain(Role.Instance.Associate.Friends, Requester.UID));//21
            stream.Write(Receiver.Associate.Contain(Role.Instance.Associate.Partener, Requester.UID));//22
            stream.Write(Receiver.Associate.Contain(Role.Instance.Associate.Mentor, Requester.UID));//23
            stream.Write(false);//24
            stream.Write(Receiver.Owner.Team != null && Receiver.Owner.Team.Members.ContainsKey(Requester.UID));//25
            stream.Write(Receiver.MyGuild != null && Receiver.MyGuild.Members.ContainsKey(Requester.UID));//26
            stream.Write(Receiver.Associate.Contain(Role.Instance.Associate.Enemy, Requester.UID));//27
            stream.Write((byte)0);//28
            stream.Write(Requester.Name, 34);//29
            stream.Finalize(GamePackets.MsgRelation);

            return stream;
        }
    }
}