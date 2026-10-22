using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgProfLevUp
    {
        [ProtoContract]
        public class MsgProfLevUpProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;

            [ProtoMember(2, IsRequired = true)]
            public uint EntityID;

            [ProtoMember(3, IsRequired = true)]
            public uint Rank;

            [ProtoMember(4, IsRequired = true)]
            public RankPromotion[] Ranking;
        }

        [ProtoContract]
        public class RankPromotion
        {
            [ProtoMember(1, IsRequired = true)]
            public uint EntityID;

            [ProtoMember(2, IsRequired = true)]
            public uint Rank;

            // [ProtoFix 2026-10-14] client CMsgTitleOwnerInfo #3 is a single bytes field (was List<byte[]>)
            [ProtoMember(3, IsRequired = true)]
            public byte[] Name = new byte[0];
        }
        public static unsafe ServerSockets.Packet CreateProfLevUp(this ServerSockets.Packet stream, MsgProfLevUpProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgProfLevUp);

            return stream;
        }
        public static unsafe void GetProfLevUp(this ServerSockets.Packet stream, out MsgProfLevUpProto pQuery)
        {
            pQuery = new MsgProfLevUpProto();
            pQuery = stream.ProtoBufferDeserialize<MsgProfLevUpProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgProfLevUp)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgProfLevUpProto Info;
            stream.GetProfLevUp(out Info);
            if (Info.Type == 0)//Promote
            {
                if (client.Player.Class % 100 >= 57) return;
                var data = Database.ProfessionTable.ProfLevelUPList.Values.Where(i => i.Class == client.Player.Class).FirstOrDefault();
                if (data != null)
                {
                    if (client.MyPrestigePoints >= data.PrestigeScore && client.Player.Level >= data.Level && client.Player.ClassExperience >= data.EXP)
                    {
                        {
                            if (Database.AtributesStatus.IsThunderStriker(client.Player.Class) && client.Player.SavePromote >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote;
                                client.Player.SavePromote = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsArcher(client.Player.Class) && client.Player.SavePromote1 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote1;
                                client.Player.SavePromote1 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsFire(client.Player.Class) && client.Player.SavePromote2 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote2;
                                client.Player.SavePromote2 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsLee(client.Player.Class) && client.Player.SavePromote3 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote3;
                                client.Player.SavePromote3 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsMonk(client.Player.Class) && client.Player.SavePromote4 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote4;
                                client.Player.SavePromote4 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsNinja(client.Player.Class) && client.Player.SavePromote5 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote5;
                                client.Player.SavePromote5 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsPirate(client.Player.Class) && client.Player.SavePromote6 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote6;
                                client.Player.SavePromote6 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class) && client.Player.SavePromote8 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote8;
                                client.Player.SavePromote8 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsWarrior(client.Player.Class) && client.Player.SavePromote9 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote9;
                                client.Player.SavePromote9 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsWater(client.Player.Class) && client.Player.SavePromote10 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote10;
                                client.Player.SavePromote10 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && client.Player.SavePromote11 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote11;
                                client.Player.SavePromote11 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            if (Database.AtributesStatus.IsDune(client.Player.Class) && client.Player.SavePromote12 >= 1)
                            {
                                client.Player.Class = client.Player.SavePromote12;
                                client.Player.SavePromote12 = 0;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                            else
                            {
                                client.Player.ClassExperience -= data.EXP;
                                client.Player.Class++;
                                Info.EntityID = client.Player.UID;
                                client.Send(stream.CreateProfLevUp(Info));
                            }
                        }
                    }
                }
            }

            else
            {
                var type = Database.PrestigeRanking.GetIndex(client.Player.Class);
                var rank = Database.PrestigeRanking.Ranks[type];
                var array = rank.Items.Values.OrderByDescending(p => p.TotalPoints).Take(3).ToArray();
                Info.Ranking = new RankPromotion[array.Length];
                for (int i = 0; i < Info.Ranking.Length; i++)
                {
                    if (array[i] != null)
                    {
                        Info.Ranking[i] = new RankPromotion();
                        Info.Ranking[i].Rank = (uint)i + 1;
                        Info.Ranking[i].EntityID = array[i].UID;
                        Info.Ranking[i].Name = global::Qahr.Mobile.AnsiText.Value.GetBytes(array[i].Name); // [ProtoFix 2026-10-14] single bytes
                    }
                }
                client.Send(stream.CreateProfLevUp(Info));
            }
        }
    }
}