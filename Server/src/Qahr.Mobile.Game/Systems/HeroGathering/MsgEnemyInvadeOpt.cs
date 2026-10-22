//using Qahr.Mobile.Client;
//using ProtoBuf;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//namespace Qahr.Mobile
//{
//    [ProtoContract]
//    public class MsgEnemyInvadeOpt
//    {
//        public enum TypeID : byte
//        {
//            Login = 0,
//            Start = 1,
//            ClaimRewards = 1,
//            Info = 3,
//            Fight = 4,
//            Change = 5,
//            Show = 7,
//        }
//        [ProtoMember(1, IsRequired = true)]
//        public uint Type = 0;
//        [ProtoMember(2, IsRequired = true)]
//        public uint UserID = 0;
//        [ProtoMember(3, IsRequired = true)]
//        public uint MyPoint = 0;
//        [ProtoMember(4, IsRequired = true)]
//        public uint StartTime = 0;
//        [ProtoMember(5, IsRequired = true)]
//        public uint Chance = 0;
//        [ProtoMember(6, IsRequired = true)]
//        public uint Recovery = 0;
//        [ProtoMember(7, IsRequired = true)]
//        public uint uk7 = 0;
//        [ProtoMember(8, IsRequired = true)]
//        public uint uk8 = 0;
//        [ProtoMember(9, IsRequired = true)]
//        public uint uk9 = 0;
//        [ProtoMember(10, IsRequired = true)]
//        public uint uk10 = 0;
//        [ProtoMember(11, IsRequired = true)]
//        public uint uk11 = 0;
//        [ProtoMember(12, IsRequired = true)]
//        public List<Player> Players = new List<Player>();
//        [ProtoContract]
//        public class Player
//        {
//            [ProtoMember(1, IsRequired = true)]
//            public uint ID = 0;
//            [ProtoMember(2, IsRequired = true)]
//            public uint Point = 0;
//        }
//        public static implicit operator MsgEnemyInvadeOpt(Qahr.Mobile.ServerSockets.Packet stream)
//        {
//            MsgEnemyInvadeOpt pQuery = new MsgEnemyInvadeOpt();
//            pQuery = stream.ProtoBufferDeserialize(pQuery);
//            return pQuery;
//        }
//        public static implicit operator Qahr.Mobile.ServerSockets.Packet(MsgEnemyInvadeOpt obj)
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                stream.InitWriter();
//                stream.ProtoBufferSerialize(obj);
//                stream.Finalize(Qahr.Mobile.Game.GamePackets.MsgEnemyInvadeOpt);
//                return stream;
//            }
//        }
//        [PacketAttribute(Qahr.Mobile.Game.GamePackets.MsgEnemyInvadeOpt)]
//        public static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
//        {
//            //HeroGathering.Load();
//            MsgEnemyInvadeOpt Info = stream;
//            switch ((MsgEnemyInvadeOpt.TypeID)Info.Type)
//            {
//                case MsgEnemyInvadeOpt.TypeID.Fight:
//                    {
//                        if (user.EnemyInvade.Chance >= 1)
//                        {
//                            Client.GameClient bot;
//                            if (Pool.GamePoll.TryGetValue(Info.UserID, out bot))
//                            {
//                                HeroGathering.Match obj = new HeroGathering.Match(HeroGathering.Counter.Next);
//                                obj.Fights = new List<GameClient>();
//                                obj.Fights.Add(user);
//                                obj.Fights.Add(bot);
//                                obj.Import(user);
//                                user.EnemyInvade.GET_Match = obj;
//                                bot.EnemyInvade.GET_Match = obj;
//                                HeroGathering.MatchesRegistered.Add(obj.ID, obj);
//                                user.Send(Info);
//                                user.EnemyInvade.Chance--;
//                                user.EnemyInvade.Recovery(2 * 60 * 60);
//                            }
//                        }
//                        break;
//                    }
//                case MsgEnemyInvadeOpt.TypeID.Info:
//                    {
//                        Info.UserID = user.Player.UID;
//                        Info.MyPoint = user.EnemyInvade.MyPoint;
//                        Info.StartTime = 1619172392;
//                        Info.Chance = user.EnemyInvade.Chance;
//                        Info.Recovery = user.EnemyInvade.GetRecovery;//Recovery
//                        Info.uk7 = 0;//Now
//                        Info.uk8 = 1;
//                        Info.uk9 = 0;
//                        Info.uk10 = 1;
//                        user.Send(Info);
//                        user.EnemyInvade.SetBot();
//                        user.EnemyInvade.SendEnemy();
//                        break;
//                    }
//                case MsgEnemyInvadeOpt.TypeID.Change:
//                    {
//                        user.EnemyInvade.RandomBot();
//                        Info.UserID = user.Player.UID;
//                        Info.MyPoint = user.EnemyInvade.MyPoint;
//                        Info.StartTime = 1619172392;
//                        Info.Chance = user.EnemyInvade.Chance;
//                        Info.Recovery = user.EnemyInvade.GetRecovery;
//                        Info.uk7 = 0;//Now
//                        Info.uk8 = 1;
//                        Info.uk9 = 0;
//                        Info.uk10 = 1;
//                        user.Send(Info);
//                        user.EnemyInvade.SendEnemy();
//                        break;
//                    }

//                default:
//                    Console.WriteLine(" HeroGatheringInfo Info.Type: " + Info.Type);
//                    break;
//            }

//        }
//    }
//}
