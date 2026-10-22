using System;
using System.Collections.Generic;
using Qahr.Mobile.Client;
using ProtoBuf;

namespace Qahr.Mobile
{
    public static class MsgFamePacket
    {
        [ProtoContract]
        public class MsgFameHall
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action;
            [ProtoMember(2, IsRequired = true)]
            public ulong FamePoints; // [ProtoFix 2026-10-14] client CMsgTravelNotesPB #2 is uint64 (was uint)
            [ProtoMember(3, IsRequired = true)]
            public uint Unk3;
            [ProtoMember(4, IsRequired = true)]
            public uint TimeStamp;
            [ProtoMember(5, IsRequired = true)]
            public uint Unk5;
            [ProtoMember(6, IsRequired = true)]
            public List<Fame> FameDetails = new List<Fame>();

            [ProtoContract]
            public class Fame
            {
                [ProtoMember(1, IsRequired = true)]
                public uint Type;
                [ProtoMember(2, IsRequired = true)]
                public uint TodayPoints;
                [ProtoMember(3, IsRequired = true)]
                public uint DoneQuests;
                [ProtoMember(4, IsRequired = true)]
                public uint Unk4;
            }
            public ServerSockets.Packet ToArray()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.InitWriter();
                    stream.ProtoBufferSerialize(this);
                    stream.Finalize(2317);
                    return stream;
                }
            }
            public static implicit operator ServerSockets.Packet(MsgFameHall obj)
            {
                return obj.ToArray();
            }
        }
        public static unsafe ServerSockets.Packet ActionCreate(this ServerSockets.Packet stream, MsgFameHall pQuery)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pQuery);
            stream.Finalize(2317);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddPoints(this ServerSockets.Packet stream, uint Action, uint points)
        {
            return ActionCreate(stream, new MsgFameHall() { Action = Action, FamePoints = points, Unk5 = 3 });
        }
        public static unsafe ServerSockets.Packet CreateFameLogin(this ServerSockets.Packet stream, uint Action, uint points)
        {
            return ActionCreate(stream, new MsgFameHall() { Action = Action, FamePoints = points, TimeStamp = uint.Parse(DateTime.Now.ToString("yyyyMMdd")), Unk5 = 3 });
        }
        [Packet(2317)]
        private unsafe static void Process(GameClient user, ServerSockets.Packet stream)
        {
            MsgFameHall msg = new MsgFameHall();
            msg = stream.ProtoBufferDeserialize<MsgFameHall>(msg);

            switch (msg.Action)
            {
                case 0:
                    {
                        var fameList = new List<MsgFameHall.Fame>();

                        fameList.Add(new MsgFameHall.Fame()
                        {
                            Type = 1,
                            TodayPoints = 111,
                            DoneQuests = 112,
                            Unk4 = 0
                        });

                        fameList.Add(new MsgFameHall.Fame()
                        {
                            Type = 33,
                            TodayPoints = 221,
                            DoneQuests = 222,
                            Unk4 = 0
                        });
                        msg.FamePoints = user.Player.TableBetDice;
                        msg.FameDetails = fameList;
                        msg.Unk5 = 3;

                        user.Send(msg);
                        break;
                    }
                case 2:
                    {
                        user.Send(msg);
                        break;
                    }
                case 3:
                    {
                        user.Send(msg);
                        break;
                    }
                case 4:
                    {
                        user.Send(stream.CreateFameLogin(4, user.Player.TableBetDice));
                        break;
                    }
                case 5:
                    {
                        user.Send(stream.AddPoints(5, user.Player.TableBetDice));
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}