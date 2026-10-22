using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgUserAbilityScore
    {
        [ProtoContract]
        public class UserAbilityScore
        {
            [ProtoMember(1, IsRequired = true)]
            public uint type;
            [ProtoMember(2, IsRequired = true)]
            public uint UID;
            [ProtoMember(3, IsRequired = true)]
            public uint Level;
            [ProtoMember(4, IsRequired = true)]
            public AbilityScore[] Items;
        }
        [ProtoContract]
        public class AbilityScore
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Position;
            [ProtoMember(2, IsRequired = true)]
            public uint Points;

        }
        public static unsafe ServerSockets.Packet UserAbilityScoreCreate(this ServerSockets.Packet stream, UserAbilityScore obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgUserAbilityScore);

            return stream;
        }
        public static unsafe void GetUserAbilityScore(this ServerSockets.Packet stream, out UserAbilityScore pQuery)
        {
            pQuery = new UserAbilityScore();
            pQuery = stream.ProtoBufferDeserialize<UserAbilityScore>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgUserAbilityScore)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            UserAbilityScore msg;
            stream.GetUserAbilityScore(out msg);
            if (msg.UID != user.Player.UID)
            {
                var PrestigeEntry = Database.PrestigeRanking.GetInfo(msg.UID);
                if (PrestigeEntry != null)
                {
                    Client.GameClient target;
                    if (Pool.GamePoll.TryGetValue(msg.UID, out target))
                    {
                        MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                        info.type = 1;
                        info.Level = PrestigeEntry.Level;
                        info.UID = PrestigeEntry.UID;
                        info.Items = new MsgUserAbilityScore.AbilityScore[PrestigeEntry.Points.Length];
                        for (int x = 0; x < PrestigeEntry.Points.Length; x++)
                        {
                            info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                            info.Items[x].Position = (uint)(x + 1);
                            info.Items[x].Points = PrestigeEntry.Points[x];
                        }
                        user.Send(stream.UserAbilityScoreCreate(info));
                    }
                    else
                    {
                        MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                        info.type = 1;
                        info.Level = 0;
                        info.UID = 0;
                        user.Send(stream.UserAbilityScoreCreate(info));
                    }
                }
                else
                {
                    Client.GameClient target;
                    if (Pool.GamePoll.TryGetValue(msg.UID, out target))
                    {
                        MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                        info.type = 1;
                        info.Level = target.Player.Level;
                        info.UID = target.Player.UID;
                        info.Items = new MsgUserAbilityScore.AbilityScore[target.PrestigePoints.Length];
                        for (int x = 0; x < target.PrestigePoints.Length; x++)
                        {
                            info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                            info.Items[x].Position = (uint)(x + 1);
                            info.Items[x].Points = target.PrestigePoints[x];
                        }
                        user.Send(stream.UserAbilityScoreCreate(info));
                    }
                    else
                    {
                        MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                        info.type = 1;
                        info.Level = 0;
                        info.UID = 0;
                        user.Send(stream.UserAbilityScoreCreate(info));
                    }
                }
            }
            else
            {
                MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = user.Player.Level;
                info.UID = user.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[user.PrestigePoints.Length];
                for (int x = 0; x < user.PrestigePoints.Length; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = user.PrestigePoints[x];

                }
                user.Send(stream.UserAbilityScoreCreate(info));
            }
        }

    }
}
